using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        private Animator animator; //Componente animator
        private NavMeshAgent navMeshAgent;
        private Transform target;

        private bool isMoving;
        private int isWalkingHash; //Hash da String que se refere a animação de Walk

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            isWalkingHash = Animator.StringToHash("isWalking");
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void Update()
        {
            if (target != null && isMoving)
            {
                navMeshAgent.SetDestination(target.position);
            }
        }

        public void startMoving(Transform transform)
        {
            target = transform;
            isMoving = true;
            animator.SetBool(isWalkingHash, true);
        }

        public void stopMoving()
        {
            target = null;
            isMoving = false;
            navMeshAgent.ResetPath();
            animator.SetBool(isWalkingHash, false);
        }
    }

}
