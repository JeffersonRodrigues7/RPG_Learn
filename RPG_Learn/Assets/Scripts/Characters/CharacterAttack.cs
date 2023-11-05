using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Attack
{
    public class CharacterAttack : MonoBehaviour
    {
        private Animator animator; // Componente Animator.

        private bool isMeleeAttacking = false; // Flag para determinar se o character está usando o melee attack
        private int isMeleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private void Start()
        {
            animator = GetComponent<Animator>();
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
        }

        public void startAttack()
        {
           
            if(isMeleeAttacking == false)
            {
                animator.SetTrigger(isMeleeAttackingHash);
                isMeleeAttacking = true;
            }
        }

        public void stopBestialAttack()
        {
            Debug.Log("StopBestialAttack");
            isMeleeAttacking = false;
        }
    }

}
