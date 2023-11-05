using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] private float cooldownTimeAfterChase = 2f;
        [SerializeField] private float arrivalDistance = 0.1f; // Distância para considerar que o personagem chegou à posição final.

        private enum CharacterState//Define o estado atual do personagem
        {
            Chasing,
            MovingToLastKnownEnemyPosition,
            Searching,
            ReturningToOriginalPosition,
            Idle
        }

        private CharacterState currentCharacterState;

        private Animator animator; // Componente Animator.
        private NavMeshAgent navMeshAgent;
        private Transform target;
        private Coroutine goBackToOriginalPositionVariable;

        private Vector3 originalPosition;

        private int isWalkingHash; // Hash da String que se refere à animação de Walk.

        public float walkSpeed = 0;
        public float chaseSpeed = 0;

        public float WalkSpeed { set { walkSpeed = value; } }
        public float ChaseSpeed { set { chaseSpeed = value; } }


        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            isWalkingHash = Animator.StringToHash("isWalking");
            animator.runtimeAnimatorController = animatorOverrideController;
            originalPosition = transform.position;
            currentCharacterState = CharacterState.Idle;

            Debug.Log(walkSpeed);
            navMeshAgent.speed = walkSpeed;
        }

        private void Update()
        {
            switch (currentCharacterState)
            {
                case CharacterState.Chasing:// Perseguindo algum alvo
                    navMeshAgent.SetDestination(target.position);
                    break;

                case CharacterState.MovingToLastKnownEnemyPosition: // Movendo-se para a última posição conhecida do inimigo.
                    if (navMeshAgent.remainingDistance <= arrivalDistance) // Inicia a corrotina para retornar à posição original e muda para o estado de busca.
                    {
                        goBackToOriginalPositionVariable = StartCoroutine(goBackToOriginalPosition());
                        currentCharacterState = CharacterState.Searching;
                    }
                    break;

                case CharacterState.Searching: //Personagem está parado atento ao inimigo que saiu de seu range de chase

                    break;

                case CharacterState.ReturningToOriginalPosition: // Retornando à posição original.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        // Limpa o caminho, desativa a animação de caminhar e muda para o estado de repouso.
                        navMeshAgent.ResetPath();
                        animator.SetBool(isWalkingHash, false);
                        currentCharacterState = CharacterState.Idle;
                    }
                    break;

                case CharacterState.Idle: // Personagem parado na posição original.
                    break;

                default:
                    break;
            }
        }

        public void startChase(Transform transform)
        {
            // Inicia a perseguição.
            currentCharacterState = CharacterState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            // Se estiver no meio de uma corrotina de retorno à posição original, interrompa-a.
            if (goBackToOriginalPositionVariable != null)
            {
                StopCoroutine(goBackToOriginalPositionVariable);
                goBackToOriginalPositionVariable = null;
            }

            target = transform;
            animator.SetBool(isWalkingHash, true);
        }

        public void stopChase()
        {
            // Interrompe a perseguição e muda para o estado de movimento para a última posição conhecida do inimigo.
            target = null;
            navMeshAgent.speed = walkSpeed;
            currentCharacterState = CharacterState.MovingToLastKnownEnemyPosition;
        }

        private IEnumerator goBackToOriginalPosition()
        {
            // Corrotina para retornar à posição original após a perseguição.
            navMeshAgent.ResetPath();
            animator.SetBool(isWalkingHash, false);

            yield return new WaitForSeconds(cooldownTimeAfterChase);

            // Após o tempo de espera, entra no estado de retornar à posição original
            currentCharacterState = CharacterState.ReturningToOriginalPosition;
            navMeshAgent.SetDestination(originalPosition);
            animator.SetBool(isWalkingHash, true);
        }
    }
}