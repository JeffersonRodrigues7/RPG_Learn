using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Velocidades de movimento
        [SerializeField] private float walkSpeed = 5.0f;
        [SerializeField] private float runSpeed = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        private Vector3 movementInput;  // Armazena a entrada de movimento do jogador

        private Animator animator;
        private Rigidbody rb;           // Referência ao componente Rigidbody
        private PlayerInput playerInput;

        private bool isWalking = false;
        private int isWalkingHash;

        private bool isMouseMove = false;

        private bool isRunning = false; // Flag para determinar se o jogador está correndo
        private int isRunningHash;

        public float stoppingDistance = 1.0f;
        private NavMeshAgent navMeshAgent;
        public Transform target;


        private void Awake()
        {
            playerInput = new PlayerInput();
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>(); // Obtém o componente Rigidbody anexado ao jogador
            navMeshAgent = GetComponent<NavMeshAgent>();
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
        }

        private void OnEnable()
        {
            // Registra callbacks para as entradas
            playerInput.Enable();
            playerInput.CharacterControls.KeyboardMovement.performed += KeyboardMove; // Callback de entrada de movimento via teclado
            playerInput.CharacterControls.KeyboardMovement.canceled += StopKeyboardMove; // Callback para parar o movimento via teclado
            playerInput.CharacterControls.MouseMovement.performed += MouseMove; // Callback de entrada de movimento via Mouse
            playerInput.CharacterControls.MouseMovement.canceled += StopMouseMove; // Callback para parar o movimento via Mouse
            playerInput.CharacterControls.Run.started += Run; // Callback para iniciar a corrida
            playerInput.CharacterControls.Run.canceled += StopRun; // Callback para parar a corrida
        }

        private void OnDisable()
        {
            // Cancela o registro de callbacks quando o script é desativado
            playerInput.Disable();
            playerInput.CharacterControls.KeyboardMovement.performed -= KeyboardMove;
            playerInput.CharacterControls.KeyboardMovement.canceled -= StopKeyboardMove;
            playerInput.CharacterControls.MouseMovement.performed -= MouseMove; // Callback de entrada de movimento via Mouse
            playerInput.CharacterControls.MouseMovement.canceled -= StopMouseMove; // Callback para parar o movimento via Mouse
            playerInput.CharacterControls.Run.started -= Run;
            playerInput.CharacterControls.Run.canceled -= StopRun;
        }

        private void FixedUpdate()
        {
            if (isMouseMove)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    movementInput = hit.point;
                }

                MoveToPosition(movementInput);
            }

            else
            {
                
                float currentSpeed = isRunning ? runSpeed : walkSpeed;
                Vector3 movementInputDirection = new Vector3(movementInput.x, 0.0f, movementInput.y).normalized;

                // Transforma a direção do movimento no espaço da câmera
                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraRight = Camera.main.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();
                Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

                // Aplica a movimentação ao Rigidbody na direção da câmera
                rb.velocity = desiredMoveDirection * currentSpeed;

                // Rotação do jogador com base na direção do movimento
                if (movementInputDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                }
            }

        }

        void MoveToPosition(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }

        private void KeyboardMove(InputAction.CallbackContext context)
        {
            navMeshAgent.ResetPath();
            isWalking = true;
            movementInput = context.ReadValue<Vector2>(); // Obtém a entrada de movimento do contexto de entrada
            animator.SetBool(isWalkingHash, true);
            //Debug.Log($"Movimentando {movementInput}");
        }

        private void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isWalking = false;
            movementInput = Vector2.zero; // Reseta a entrada de movimento quando o jogador para de se mover
            stopRunAnimation();
            animator.SetBool(isWalkingHash, false);
            //Debug.Log($"Parando de se movimentar {movementInput}");
        }     
        
        private void MouseMove(InputAction.CallbackContext context)
        {
            isMouseMove = true;
            animator.SetBool(isWalkingHash, true);
            Debug.Log($"Movimentando {movementInput}");
        }

        private void StopMouseMove(InputAction.CallbackContext context)
        {
            isMouseMove = false;
            movementInput = Vector2.zero; // Reseta a entrada de movimento quando o jogador para de se mover
            stopRunAnimation();
            animator.SetBool(isWalkingHash, false);
            Debug.Log($"Parando de se movimentar {movementInput}");
        }



        private void Run(InputAction.CallbackContext context)
        {
            if (isWalking)
            {
                isRunning = true; // Define a sinalização de corrida como verdadeira quando o jogador começa a correr
                animator.SetBool(isRunningHash, true);
                //Debug.Log($"Começando a correr {isRunning}");
            }
        }

        private void StopRun(InputAction.CallbackContext context)
        {
            stopRunAnimation();
        }

        private void stopRunAnimation()
        {
            isRunning = false; // Define a sinalização de corrida como falsa quando o jogador para de correr
            animator.SetBool(isRunningHash, false);
            //Debug.Log($"Parando de correr {isRunning}");
        }
    }

}

