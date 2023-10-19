using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using NaughtyAttributes;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region ==================== VARIABLES DECLARATION ====================

        [Header("Movimenta��o")]
        [Tooltip("Velocidade do jogador ao andar")]
        [SerializeField][Min(0)] private float walkSpeed = 5.0f;

        [Tooltip("Velocidade do jogador ao correr")]
        [SerializeField][Min(0)] private float runSpeed = 10.0f;

        [Tooltip("Velocidade de rota��o do jogador")]
        [SerializeField][Min(0)] private float rotationSpeed = 10.0f;

        private PlayerInput playerInput; //Componente playerInput
        private Animator animator; //Componente animator
        private Rigidbody rb; //Componente rigidibody
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent

        private Camera cam; //Camera principal do jogo

        private Vector3 movementPosition;  //Posi��o para qual o player ir� se mover

        private bool isKeyboardWalking = false;  // Flag para determinar se o jogador est� andando via teclado
        private bool isMouseMoving = false;  // Flag para determinar se o jogador est� andando via mouse
        private int isWalkingHash; //Hash da String que se refere a anima��o de Walk

        private bool isRunning = false; // Flag para determinar se o jogador est� correndo
        private int isRunningHash; //Hash da String que se refere a anima��o de Running

        #endregion

        #region ==================== BEGIN/END SCRIPT ====================

        private void Awake()
        {
            playerInput = new PlayerInput();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>(); // Obt�m o componente Rigidbody anexado ao jogador
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");

            cam = Camera.main;
        }

        private void OnEnable()
        {
            enablePlayerInputs();
        }

        private void OnDisable()
        {
            disablePlayerInputs();
        }

        #endregion

        private void FixedUpdate()
        {
            if (isMouseMoving) //Movimenta��o via mouse
            {
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    movementPosition = hit.point;
                }

                MoveToPosition(movementPosition);
            }

            else //Movimenta��o via teclado
            {
                float currentSpeed = isRunning ? runSpeed : walkSpeed;
                Vector3 movementInputDirection = new Vector3(movementPosition.x, 0.0f, movementPosition.y).normalized;

                // Transforma a dire��o do movimento no espa�o da c�mera
                Vector3 cameraForward = cam.transform.forward;
                Vector3 cameraRight = cam.transform.right;
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

        void MoveToPosition(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }

        private void KeyboardMove(InputAction.CallbackContext context)
        {
            navMeshAgent.ResetPath();
            isKeyboardWalking = true;
            movementPosition = context.ReadValue<Vector2>(); // Obt�m a entrada de movimento do contexto de entrada
            animator.SetBool(isWalkingHash, true);
            //Debug.Log($"Movimentando {movementPosition}");
        }

        private void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isKeyboardWalking = false;
            movementPosition = Vector2.zero; // Reseta a entrada de movimento quando o jogador para de se mover
            stopRunAnimation();
            animator.SetBool(isWalkingHash, false);
            //Debug.Log($"Parando de se movimentar {movementPosition}");
        }     
        
        private void MouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = true;
            animator.SetBool(isWalkingHash, true);
            Debug.Log($"Movimentando {movementPosition}");
        }

        private void StopMouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = false;
            movementPosition = Vector2.zero; // Reseta a entrada de movimento quando o jogador para de se mover
            stopRunAnimation();
            animator.SetBool(isWalkingHash, false);
            Debug.Log($"Parando de se movimentar {movementPosition}");
        }

        private void Run(InputAction.CallbackContext context)
        {
            if (isKeyboardWalking)
            {
                isRunning = true; // Define a sinaliza��o de corrida como verdadeira quando o jogador come�a a correr
                animator.SetBool(isRunningHash, true);
                //Debug.Log($"Come�ando a correr {isRunning}");
            }
        }

        private void StopRun(InputAction.CallbackContext context)
        {
            stopRunAnimation();
        }

        private void stopRunAnimation()
        {
            isRunning = false; // Define a sinaliza��o de corrida como falsa quando o jogador para de correr
            animator.SetBool(isRunningHash, false);
            //Debug.Log($"Parando de correr {isRunning}");
        }

        #region ==================== ENABLE/DISABLE PLAYER INPUTS ====================

        // Registra callbacks para as entradas
        private void enablePlayerInputs()
        {
            playerInput.Enable();
            playerInput.CharacterControls.KeyboardWalk.performed += KeyboardMove; // Callback de entrada de movimento via teclado
            playerInput.CharacterControls.KeyboardWalk.canceled += StopKeyboardMove; // Callback para parar o movimento via teclado
            playerInput.CharacterControls.MouseWalk.performed += MouseMove; // Callback de entrada de movimento via Mouse
            playerInput.CharacterControls.MouseWalk.canceled += StopMouseMove; // Callback para parar o movimento via Mouse
            playerInput.CharacterControls.Run.started += Run; // Callback para iniciar a corrida
            playerInput.CharacterControls.Run.canceled += StopRun; // Callback para parar a corrida
        }

        // Cancela o registro de callbacks quando o script � desativado
        private void disablePlayerInputs()
        {
            playerInput.Disable();
            playerInput.CharacterControls.KeyboardWalk.performed -= KeyboardMove;
            playerInput.CharacterControls.KeyboardWalk.canceled -= StopKeyboardMove;
            playerInput.CharacterControls.MouseWalk.performed -= MouseMove;
            playerInput.CharacterControls.MouseWalk.canceled -= StopMouseMove;
            playerInput.CharacterControls.Run.started -= Run;
            playerInput.CharacterControls.Run.canceled -= StopRun;
        }

        #endregion
    }
}

