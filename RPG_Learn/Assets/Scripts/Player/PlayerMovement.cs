using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        #region ==================== VARIABLES DECLARATION ====================

        [Header("Movimenta��o")]
        [Tooltip("Velocidade do jogador ao andar")]
        [SerializeField][Min(0)] private float walkSpeed = 5.0f;

        [Tooltip("Velocidade do jogador ao correr")]
        [SerializeField][Min(0)] private float runSpeed = 10.0f;

        [Tooltip("Velocidade de rota��o do jogador")]
        [SerializeField][Min(0)] private float rotationSpeed = 10.0f;

        [Tooltip("Dist�ncia com a qual o jogador consegue interagir com o mapa atrav�s do mouse")]
        [SerializeField][Min(0)] private float mouseInputDistance = 50.0f;

        private PlayerInput playerInput; //Componente playerInput
        private Animator animator; //Componente animator
        private Rigidbody rb; //Componente rigidibody
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent

        private Camera cam; //Camera principal do jogo

        private Vector3 movementPosition;  //Posi��o para qual o player ir� se mover

        private bool isWalking = false; //Flag que indica que o objetando est� se movendo
        private float currentSpeed = 3.0f; //Velocidade atual do jogador
        private bool navMeshRemainingPath = false; // Flag que indica se o objeto deve continuar a se movimentar

        private bool isKeyboardMoving = false;  // Flag para determinar se o jogador est� andando via teclado
        private bool isMouseMoving = false;  // Flag para determinar se o jogador est� andando via mouse
        private int isWalkingHash; //Hash da String que se refere a anima��o de Walk

        private bool isRunning = false; // Flag para determinar se o jogador est� correndo
        private int isRunningHash; //Hash da String que se refere a anima��o de Running

        #endregion

        #region ==================== BEGIN/END SCRIPT ====================

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            playerInput = new PlayerInput();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar anima��es e obt�m a c�mera principal do jogo
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
            cam = Camera.main;
        }

        private void OnEnable()
        {
            // Ativa o registro de inputs do jogador
            enablePlayerInputs();
        }

        private void OnDisable()
        {
            // Desativa o registro de inputs do jogador
            disablePlayerInputs();
        }

        #endregion

        #region ==================== UPDATES ====================

        private void Update()
        {
            updateMoveParameters();
        }

        private void FixedUpdate()
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed; 

            if (isMouseMoving) //Se estivermos nos movendo via mouse
            {
                doMouseMovimentation();
            }

            else if(isKeyboardMoving) //Se estivermos no movendo via teclado
            {
                doKeyboardMovimentation();
            }
        }

        #endregion

        #region ==================== MOVIMENTA��O ====================

        //Faz movimenta��o via Mouse
        private void doMouseMovimentation()
        {
            // Cria um raio a partir da posi��o do mouse convertida para o espa�o da tela.
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Realiza um raio para detectar colis�es no mundo e armazena as informa��es na vari�vel 'hit'.
            if (Physics.Raycast(ray, out RaycastHit hit, mouseInputDistance))
            {
                movementPosition = hit.point; // Define a posi��o de movimento com base no ponto onde o raio colidiu com o objeto.
            }

            MoveToPosition(movementPosition); // Chama a fun��o 'MoveToPosition' para mover o jogador at� a nova posi��o definida.

            navMeshRemainingPath = true; //Indica que temos um caminho a percorrer 

            navMeshAgent.speed = currentSpeed; // Atualiza a velocidade do NavMeshAgent para a velocidade atual (corrida ou caminhada).
        }

        //Faz movimenta��o via teclado
        private void doKeyboardMovimentation()
        {
            // Cria um vetor de dire��o a partir da posi��o de movimento do jogador, com a componente y zerada, e normaliza-o.
            Vector3 movementInputDirection = new Vector3(movementPosition.x, 0.0f, movementPosition.y).normalized;

            // Transforma a dire��o do movimento no espa�o da c�mera
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;

            // Define as componentes y de cameraForward e cameraRight como zero para manter o movimento no plano horizontal.
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normaliza os vetores da c�mera para garantir que tenham comprimento igual a 1.
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcula a dire��o de movimento desejada no espa�o da c�mera.
            Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

            // Define a velocidade do Rigidbody com base na dire��o de movimento desejada e na velocidade atual.
            rb.velocity = desiredMoveDirection * currentSpeed;

            // Se houver uma dire��o de movimento (diferente de zero), realiza a rota��o do jogador.
            if (movementInputDirection != Vector3.zero)
            {
                // Calcula uma nova rota��o com base na dire��o de movimento desejada.
                Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);

                Debug.Log("Rotacionando");
                // Aplica uma rota��o suave (Slerp) do jogador em dire��o � nova rota��o.
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

            }
        }

        //Fun��o respons�vel por cuidar da anima��o do jogador
        private void updateMoveParameters()
        {
            Debug.Log(navMeshAgent.remainingDistance + " - " + navMeshAgent.stoppingDistance);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.ResetPath();
                navMeshRemainingPath = false;
            }

            isWalking = isKeyboardMoving || isMouseMoving || navMeshRemainingPath;

            if (isWalking && isRunning)
            {
                updateMoveAnimation(true, true);
            }
            else if (isWalking)
            {
                updateMoveAnimation(true, false);
            }
            else
            {
                updateMoveAnimation(false, false);
            }
        }

        //Atualiza��o anima��o de mvimento
        private void updateMoveAnimation(bool isWalking, bool isRunning)
        {
            animator.SetBool(isWalkingHash, isWalking);//Para que ele chegue na anima��o de correr, � necess�rio que esteja andando
            animator.SetBool(isRunningHash, isRunning);
        }

        // Move o jogador para a posi��o de destino usando o NavMeshAgent
        private void MoveToPosition(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }

        #endregion

        #region ==================== CALLBACKS DE INPUT ====================

        /**Callback que reseta o caminho do NavMeshAgent, ativa a flag de movimento via teclado e atualiza a posi��o de movimento, 
         * Keyboard movemente tem prioridade sobre o MouseMovement*/
        private void KeyboardMove(InputAction.CallbackContext context)
        {
            navMeshAgent.ResetPath();
            isMouseMoving = false;
            isKeyboardMoving = true;
            movementPosition = context.ReadValue<Vector2>();
        }

        // Desativa a flag de movimento via teclado, reseta a posi��o de movimento e para a anima��o de corrida
        private void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isKeyboardMoving = false;
            movementPosition = Vector2.zero;
        }

        // Ativa a flag de movimento via mouse e a anima��o de caminhar
        private void MouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = true;
        }

        // Desativa a flag de movimento via mouse, reseta a posi��o de movimento e para a anima��o de corrida
        private void StopMouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = false;
            movementPosition = Vector2.zero;
        }

        // Define a flag de corrida como verdadeira e inicia a anima��o de corrida
        private void Run(InputAction.CallbackContext context)
        {
            isRunning = true;
        }

        // Para a anima��o de corrida, chamado quando usu�rio solta o shift ou quando para de andar
        private void StopRun(InputAction.CallbackContext context)
        {
            isRunning = false;
        }

        #endregion

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

