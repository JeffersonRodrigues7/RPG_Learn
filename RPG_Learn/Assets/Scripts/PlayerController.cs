using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speeds for walking and running
        [SerializeField] private float walkSpeed = 5.0f;
        [SerializeField] private float runSpeed = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        // Input and state variables
        private Vector2 movementInput;  // Armazena a entrada de movimento do jogador

        private Animator animator;
        private Rigidbody rb;           // Refer�ncia ao componente Rigidbody
        private PlayerInput playerInput;

        private bool isWalking;
        private int isWalkingHash;

        private bool isRunning = false; // Flag para determinar se o jogador est� correndo
        private int isRunningHash;

        private void Awake()
        {
            playerInput = new PlayerInput();
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>(); // Obt�m o componente Rigidbody anexado ao jogador
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
        }

        private void OnEnable()
        {
            // Registra callbacks para as entradas
            playerInput.Enable();
            playerInput.CharacterControls.Movement.performed += Move; // Callback de entrada de movimento
            playerInput.CharacterControls.Movement.canceled += StopMoving; // Callback para parar o movimento
            playerInput.CharacterControls.Run.started += StartRunning; // Callback para iniciar a corrida
            playerInput.CharacterControls.Run.canceled += StopRunning; // Callback para parar a corrida
        }

        private void OnDisable()
        {
            // Cancela o registro de callbacks quando o script � desativado
            playerInput.Disable();
            playerInput.CharacterControls.Movement.performed -= Move;
            playerInput.CharacterControls.Movement.canceled -= StopMoving;
            playerInput.CharacterControls.Run.started -= StartRunning;
            playerInput.CharacterControls.Run.canceled -= StopRunning;
        }

        private void Move(InputAction.CallbackContext context)
        {    
            isWalking = true;
            movementInput = context.ReadValue<Vector2>(); // Obt�m a entrada de movimento do contexto de entrada
            animator.SetBool(isWalkingHash, true);
            Debug.Log($"Movimentando {movementInput}");
        }

        private void StopMoving(InputAction.CallbackContext context)
        {
            isWalking = false;
            movementInput = Vector2.zero; // Reseta a entrada de movimento quando o jogador para de se mover
            animator.SetBool(isWalkingHash, false);
            Debug.Log($"Parando de se movimentar {movementInput}");
        }

        private void StartRunning(InputAction.CallbackContext context)
        {
            isRunning = true; // Define a sinaliza��o de corrida como verdadeira quando o jogador come�a a correr
            animator.SetBool(isRunningHash, true);
            Debug.Log($"Come�ando a correr {isRunning}");
        }

        private void StopRunning(InputAction.CallbackContext context)
        {
            isRunning = false; // Define a sinaliza��o de corrida como falsa quando o jogador para de correr
            animator.SetBool(isRunningHash, false);
            Debug.Log($"Parando de correr {isRunning}");
        }

        private void FixedUpdate()
        {
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            Vector3 movementInputDirection = new Vector3(movementInput.x, 0.0f, movementInput.y).normalized;

            // Transforma a dire��o do movimento no espa�o da c�mera
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

            // Aplica a movimenta��o ao Rigidbody na dire��o da c�mera
            rb.velocity = desiredMoveDirection * currentSpeed;

            // Rota��o do jogador com base na dire��o do movimento
            if (movementInputDirection != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

}

