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
        private Transform player;
        

        private int isWalkingHash; //Hash da String que se refere a animação de Walk

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            player = GameObject.FindGameObjectWithTag("Player").transform; //Capturando objeto



            isWalkingHash = Animator.StringToHash("isWalking");
            animator.runtimeAnimatorController = animatorOverrideController;


            animator.SetBool(isWalkingHash, true);//Para que ele chegue na animação de correr, é necessário que esteja andando
        }

        void Update()
        {
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position);
            }
        }
    }

}
