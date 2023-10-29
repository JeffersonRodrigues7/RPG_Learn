using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        #region ==================== VARIABLES DECLARATION ====================

        [Header("Movimentação")]
        [SerializeField] private float walkSpeed = 5.0f; //Velocidade do jogador ao andar
        [SerializeField] private float runSpeed = 10.0f; //Velocidade do jogador ao correr
        [SerializeField] private float rotationSpeed = 10.0f; //Velocidade de rotação do jogador

        [SerializeField] private float jumpVelocity = 1.0f; //Velocidade com a qual o objeto irá pular
        [SerializeField] private float jumpSlowdown = 1.0f; //Valor com o qual o player vai perdendo a velocidade no pulo

        [Tooltip("Distância com a qual o jogador consegue interagir com o mapa através do mouse")]
        [SerializeField] private float mouseInputDistance = 50.0f;

        private PlayerInput playerInput; //Componente playerInput
        private Animator animator; //Componente animator
        private Rigidbody rb; //Componente rigidibody
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent

        private Camera cam; //Camera principal do jogo

        private Vector3 playerPosition; //Posição atual do player
        private Vector3 movementPosition;  //Posição para qual o player irá se mover

        private bool isWalking = false; //Flag que indica que o objetando está se movendo
        private float currentSpeed = 3.0f; //Velocidade atual do jogador
        private bool navMeshRemainingPath = false; // Flag que indica se o objeto deve continuar a se movimentar

        private bool isKeyboardMoving = false;  // Flag para determinar se o jogador está andando via teclado
        private bool isMouseMoving = false;  // Flag para determinar se o jogador está andando via mouse
        private int isWalkingHash; //Hash da String que se refere a animação de Walk

        private bool isRunning = false; // Flag para determinar se o jogador está correndo
        private int isRunningHash; //Hash da String que se refere a animação de Running

        private bool isJumping = false; // Flag para determinar se o jogador está pulando
        private bool isFalling = false; // Flag para determinar se o jogador está caindo
        private int isJumpingHash; //Hash da String que se refere a animação de Jumping

        private float currentJumpVelocity = 0;
        #endregion

        #region ==================== BEGIN/END SCRIPT ====================

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            playerInput = new PlayerInput();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar animações e obtém a câmera principal do jogo
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
            isJumpingHash = Animator.StringToHash("isJumping");
            cam = Camera.main;

            playerPosition = transform.position;
            currentJumpVelocity = jumpVelocity;
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
            //playerPosition = transform.position;

            //Debug.Log(rb.velocity);

            //if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            //{  
            //    isJumping = true;
            //}

            updateMoveParameters();
        }

        private void FixedUpdate()
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed;

            //if (isJumping)
            //{
            //    currentJumpVelocity -= jumpVelocity * jumpSlowdown * Time.fixedDeltaTime;
            //    rb.velocity = new Vector3(0, currentJumpVelocity, 0);
            //}

            if (isMouseMoving) //Se estivermos nos movendo via mouse
            {
                doMouseMovimentation();
            }

            else if (isKeyboardMoving) //Se estivermos no movendo via teclado
            {
                doKeyboardMovimentation();
            }
        }

        ////Detecta quando o personagem toca no chão(pode ser necessário configurar colisores ou Raycast para isso).
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.CompareTag("Ground"))  // "Ground" é a tag do objeto que representa o chão.
        //    {
        //        currentJumpVelocity = jumpVelocity;
        //        isJumping = false;  // O personagem não está mais pulando quando toca no chão.
        //        //animator.SetBool(isJumpingHash, false);
        //        //navMeshAgent.enabled = true;
        //        Debug.Log("HERE");
        //    }
        //}

        #endregion

        #region ==================== MOVIMENTAÇÃO ====================

        //Faz movimentação via Mouse
        private void doMouseMovimentation()
        {
            // Cria um raio a partir da posição do mouse convertida para o espaço da tela.
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Realiza um raio para detectar colisões no mundo e armazena as informações na variável 'hit'.
            if (Physics.Raycast(ray, out RaycastHit hit, mouseInputDistance))
            {
                movementPosition = hit.point; // Define a posição de movimento com base no ponto onde o raio colidiu com o objeto.
            }

            MoveToPosition(movementPosition); // Chama a função 'MoveToPosition' para mover o jogador até a nova posição definida.

            navMeshRemainingPath = true; //Indica que temos um caminho a percorrer 

            if (navMeshAgent.enabled)
            {
                navMeshAgent.speed = currentSpeed; // Atualiza a velocidade do NavMeshAgent para a velocidade atual (corrida ou caminhada).
            }
        }

        //Faz movimentação via teclado
        private void doKeyboardMovimentation()
        {
            // Cria um vetor de direção a partir da posição de movimento do jogador, com a componente y zerada, e normaliza-o.
            Vector3 movementInputDirection = new Vector3(movementPosition.x, 0.0f, movementPosition.y).normalized;

            // Transforma a direção do movimento no espaço da câmera
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;

            // Define as componentes y de cameraForward e cameraRight como zero para manter o movimento no plano horizontal.
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normaliza os vetores da câmera para garantir que tenham comprimento igual a 1.
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcula a direção de movimento desejada no espaço da câmera.
            Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

            // Define a velocidade do Rigidbody com base na direção de movimento desejada e na velocidade atual.
            rb.velocity = desiredMoveDirection * currentSpeed;

            // Se houver uma direção de movimento (diferente de zero), realiza a rotação do jogador.
            if (movementInputDirection != Vector3.zero)
            {
                // Calcula uma nova rotação com base na direção de movimento desejada.
                Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);

                // Aplica uma rotação suave (Slerp) do jogador em direção à nova rotação.
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

            }
        }

        //Função responsável por cuidar da animação do jogador
        private void updateMoveParameters()
        {
            //Verifica se o player chegou na posição via navmesh
            if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
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

        //Atualização animação de mvimento
        private void updateMoveAnimation(bool isWalking, bool isRunning)
        {
            animator.SetBool(isWalkingHash, isWalking);//Para que ele chegue na animação de correr, é necessário que esteja andando
            animator.SetBool(isRunningHash, isRunning);
        }

        // Move o jogador para a posição de destino usando o NavMeshAgent
        private void MoveToPosition(Vector3 destination)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        #endregion

        #region ==================== CALLBACKS DE INPUT ====================

        /**Callback que reseta o caminho do NavMeshAgent, ativa a flag de movimento via teclado e atualiza a posição de movimento, 
         * Keyboard movemente tem prioridade sobre o MouseMovement*/
        private void KeyboardMove(InputAction.CallbackContext context)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.ResetPath();
            }
            isMouseMoving = false;
            isKeyboardMoving = true;
            movementPosition = context.ReadValue<Vector2>();
        }

        // Desativa a flag de movimento via teclado, reseta a posição de movimento e para a animação de corrida
        private void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isKeyboardMoving = false;
            movementPosition = Vector2.zero;
        }

        // Ativa a flag de movimento via mouse e a animação de caminhar
        private void MouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = true;
        }

        // Desativa a flag de movimento via mouse, reseta a posição de movimento e para a animação de corrida
        private void StopMouseMove(InputAction.CallbackContext context)
        {
            isMouseMoving = false;
            movementPosition = Vector2.zero;
        }

        // Define a flag de corrida como verdadeira e inicia a animação de corrida
        private void Run(InputAction.CallbackContext context)
        {
            isRunning = true;
        }

        // Para a animação de corrida, chamado quando usuário solta o shift ou quando para de andar
        private void StopRun(InputAction.CallbackContext context)
        {
            isRunning = false;
        }

        //Inicia animação de jumping
        private void Jump(InputAction.CallbackContext context)
        {
            //animator.SetBool(isJumpingHash, true);
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //isJumping = true;
        }

        // Chamado quando soltamos o espaço
        private void StopJump(InputAction.CallbackContext context)
        {
            //animator.SetBool(isJumpingHash, false);
            //isJumping = false;
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
            playerInput.CharacterControls.Jump.started += Jump; // Callback para iniciar a Pulo
            playerInput.CharacterControls.Jump.canceled += StopJump; // Callback para indicar que soltamos botão de pulo
        }

        // Cancela o registro de callbacks quando o script é desativado
        private void disablePlayerInputs()
        {
            playerInput.Disable();
            playerInput.CharacterControls.KeyboardWalk.performed -= KeyboardMove;
            playerInput.CharacterControls.KeyboardWalk.canceled -= StopKeyboardMove;
            playerInput.CharacterControls.MouseWalk.performed -= MouseMove;
            playerInput.CharacterControls.MouseWalk.canceled -= StopMouseMove;
            playerInput.CharacterControls.Run.started -= Run;
            playerInput.CharacterControls.Run.canceled -= StopRun;
            playerInput.CharacterControls.Jump.started -= Jump;
            playerInput.CharacterControls.Jump.canceled -= StopJump;
        }

        #endregion
    }
}

