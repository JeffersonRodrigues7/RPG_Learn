using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float walkSpeed = 0; // Velocidade de caminhada do personagem
        [SerializeField] private float chaseSpeed = 0; // Velocidade de perseguição do personagem
        [SerializeField] private float cooldownTimeAfterChase = 2f; // Tempo de espera após a perseguição
        [SerializeField] private float arrivalDistance = 0.1f; // Distância para considerar que o personagem chegou à posição final
        [SerializeField] private Transform[] patrolPoints; // Pontos de patrulha do personagem

        private enum CharacterState // Estados possiveis
        {
            Patrolling,
            Chasing,
            MovingToLastKnownEnemyPosition,
            Searching,
            ReturningToOriginalPosition
        }

        private CharacterState currentCharacterState;

        private Animator animator; 
        private NavMeshAgent navMeshAgent;
        private Transform target; // Alvo atual do personagem
        private Coroutine goBackToOriginalPositionVariable; // Corrotina para retornar à posição original
        private Vector3 originalPosition; // Posição original do personagem

        private int isWalkingHash; // Hash da String que se refere à animação de Walk
        private bool isWalking = false;

        private int initialPatrolPoint = 0; // Ponto inicial de patrulha
        private int currentPatrolPoint = 0;
        private int patrolPointsLength = 0;

        public float WalkSpeed { set { walkSpeed = value; } }
        public float ChaseSpeed { set { chaseSpeed = value; } }
        public float CooldownTimeAfterChase { set { cooldownTimeAfterChase = value; } }
        public float ArrivalDistance { set { arrivalDistance = value; } }
        public Transform[] PatrolPoints { set { patrolPoints = value; } }

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            isWalkingHash = Animator.StringToHash("isWalking"); // Obtém o hash da string da animação de caminhada
            originalPosition = transform.position; // Registra a posição original do personagem
            currentCharacterState = CharacterState.Patrolling; // Define o estado inicial como "Patrolling"
            navMeshAgent.speed = walkSpeed; 

            if (patrolPoints != null) patrolPointsLength = patrolPoints.Length; // Registra o número de pontos de patrulha se existirem.
        }

        private void Update()
        {
            switch (currentCharacterState)
            {
                case CharacterState.Patrolling: // Personagem parado na posição original.
                    if (patrolPointsLength > 1 && !isWalking) // Se existir mais de um ponto de patrulha e o personagem não estiver caminhando
                    {
                        setWalkingAnimation(true);
                    }
                    doPatrolling(); // Executa a patrulha
                    break;

                case CharacterState.Chasing: // Perseguindo algum alvo
                    navMeshAgent.SetDestination(target.position);
                    break;

                case CharacterState.MovingToLastKnownEnemyPosition: // Movendo-se para a última posição conhecida do inimigo.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        goBackToOriginalPositionVariable = StartCoroutine(goBackToOriginalPosition()); // Inicia a corrotina para retornar à posição original
                        currentCharacterState = CharacterState.Searching; // Muda para o estado de busca
                    }
                    break;

                case CharacterState.Searching: // Personagem está parado atento ao inimigo que saiu de seu alcance de perseguição
                    
                    break;

                case CharacterState.ReturningToOriginalPosition: // Retornando à posição original.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        navMeshAgent.ResetPath(); // Limpa o caminho
                        setWalkingAnimation(false); 
                        currentCharacterState = CharacterState.Patrolling; // Muda para o estado de patrulha

                        currentPatrolPoint = initialPatrolPoint;
                        navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position); // Enviando o personagem para o primeiro ponto de patrulha
                    }
                    break;

                default:
                    break;
            }
        }

        // Função que realizar a patrulha do personagem
        private void doPatrolling()
        {
            if (patrolPointsLength > 0) //Se houve algum ponto de patrulha
            {
                if (navMeshAgent.remainingDistance <= arrivalDistance) //Se personagem chegou a algum ponto de patrulha
                {
                    currentPatrolPoint++;
                    if (currentPatrolPoint >= patrolPointsLength) //Se estamos no ultimo ponto, voltamos pro inicio
                    {
                        currentPatrolPoint = 0;
                    }
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
                }
            }
            else
            {
                setWalkingAnimation(false); // Se não houver pontos de patrulha, desativa a animação de caminhada
            }
        }

        private void setWalkingAnimation(bool _isWalking)
        {
            if (isWalking != _isWalking)
            {
                isWalking = _isWalking;
                animator.SetBool(isWalkingHash, isWalking); // Define o parâmetro da animação de caminhada
            }
        }

        // Inicia a perseguição.
        public void startChase(Transform transform)
        {

            currentCharacterState = CharacterState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            // Se estiver no meio de uma corrotina de retorno à posição original, interrompa-a.
            if (goBackToOriginalPositionVariable != null)
            {
                StopCoroutine(goBackToOriginalPositionVariable);
                goBackToOriginalPositionVariable = null;
            }

            target = transform; // Define o alvo da perseguição
            setWalkingAnimation(true); 
        }

        // Interrompe a perseguição e muda para o estado de movimento para a última posição conhecida do inimigo.
        public void stopChase()
        {

            target = null;
            navMeshAgent.speed = walkSpeed;
            currentCharacterState = CharacterState.MovingToLastKnownEnemyPosition;
        }

        // Corrotina para retornar à posição original após a perseguição.
        private IEnumerator goBackToOriginalPosition()
        {

            navMeshAgent.ResetPath(); // Limpa o caminho
            setWalkingAnimation(false); 

            yield return new WaitForSeconds(cooldownTimeAfterChase);

            // Após o tempo de espera, entra no estado de retornar à posição original
            currentCharacterState = CharacterState.ReturningToOriginalPosition;
            navMeshAgent.SetDestination(originalPosition);
            setWalkingAnimation(true);
        }
    }
}