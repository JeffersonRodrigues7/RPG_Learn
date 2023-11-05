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
        [SerializeField] private float walkSpeed = 0;
        [SerializeField] private float chaseSpeed = 0;
        [SerializeField] private float cooldownTimeAfterChase = 2f;
        [SerializeField] private float arrivalDistance = 0.1f; // Dist�ncia para considerar que o personagem chegou � posi��o final.
        [SerializeField] private Transform[] patrolPoints;

        private enum CharacterState//Define o estado atual do personagem
        {
            Chasing,
            MovingToLastKnownEnemyPosition,
            Searching,
            ReturningToOriginalPosition,
            Patrolling
        }

        private CharacterState currentCharacterState;

        private Animator animator; // Componente Animator.
        private NavMeshAgent navMeshAgent;
        private Transform target;
        private Coroutine goBackToOriginalPositionVariable;

        private Vector3 originalPosition;

        private int isWalkingHash; // Hash da String que se refere � anima��o de Walk.
        private bool isWalking = false;

        private int initialPatrolPoint = 0; //Ponto inicial de patrulha
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
            isWalkingHash = Animator.StringToHash("isWalking");
            originalPosition = transform.position;
            currentCharacterState = CharacterState.Patrolling;

            navMeshAgent.speed = walkSpeed;
            patrolPointsLength = patrolPoints.Length;
        }

        private void Update()
        {

            switch (currentCharacterState)
            {
                case CharacterState.Patrolling: // Personagem parado na posi��o original.
                    if(patrolPointsLength > 1 && !isWalking) //Se existe mais um ponto de patrulha e ele ainda n�o est� na anima��o de andar
                    {
                        isWalking = true;
                        animator.SetBool(isWalkingHash, isWalking);
                    }
                    doPatrolling();
                    break;

                case CharacterState.Chasing:// Perseguindo algum alvo
                    navMeshAgent.SetDestination(target.position);
                    break;

                case CharacterState.MovingToLastKnownEnemyPosition: // Movendo-se para a �ltima posi��o conhecida do inimigo.
                    if (navMeshAgent.remainingDistance <= arrivalDistance) // Inicia a corrotina para retornar � posi��o original e muda para o estado de busca.
                    {
                        goBackToOriginalPositionVariable = StartCoroutine(goBackToOriginalPosition());
                        currentCharacterState = CharacterState.Searching;
                    }
                    break;

                case CharacterState.Searching: //Personagem est� parado atento ao inimigo que saiu de seu range de chase

                    break;

                case CharacterState.ReturningToOriginalPosition: // Retornando � posi��o original.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        // Limpa o caminho, desativa a anima��o de caminhar e muda para o estado de repouso.
                        navMeshAgent.ResetPath();
                        isWalking = false;
                        animator.SetBool(isWalkingHash, isWalking);
                        currentCharacterState = CharacterState.Patrolling;

                        currentPatrolPoint = initialPatrolPoint;
                        navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);//Enviando personagem para o primeiro ponto de patrulha
                    }
                    break;

                default:
                    break;
            }
        }

        private void doPatrolling()
        {
            if (navMeshAgent.remainingDistance <= arrivalDistance)
            {
                currentPatrolPoint++;
                if(currentPatrolPoint >= patrolPointsLength)
                {
                    currentPatrolPoint = 0;
                }

                navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
            }
        }

        public void startChase(Transform transform)
        {
            // Inicia a persegui��o.
            currentCharacterState = CharacterState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            // Se estiver no meio de uma corrotina de retorno � posi��o original, interrompa-a.
            if (goBackToOriginalPositionVariable != null)
            {
                StopCoroutine(goBackToOriginalPositionVariable);
                goBackToOriginalPositionVariable = null;
            }

            target = transform;
            isWalking = true;
            animator.SetBool(isWalkingHash, isWalking);
        }

        public void stopChase()
        {
            // Interrompe a persegui��o e muda para o estado de movimento para a �ltima posi��o conhecida do inimigo.
            target = null;
            navMeshAgent.speed = walkSpeed;
            currentCharacterState = CharacterState.MovingToLastKnownEnemyPosition;
        }

        private IEnumerator goBackToOriginalPosition()
        {
            // Corrotina para retornar � posi��o original ap�s a persegui��o.
            navMeshAgent.ResetPath();
            isWalking = false;
            animator.SetBool(isWalkingHash, isWalking);

            yield return new WaitForSeconds(cooldownTimeAfterChase);

            // Ap�s o tempo de espera, entra no estado de retornar � posi��o original
            currentCharacterState = CharacterState.ReturningToOriginalPosition;
            navMeshAgent.SetDestination(originalPosition);
            isWalking = true;
            animator.SetBool(isWalkingHash, isWalking);
        }
    }
}