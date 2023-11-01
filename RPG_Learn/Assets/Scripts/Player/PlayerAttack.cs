using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        #region VARIABLES DECLARATION
        private Animator animator; //Componente animator

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int isMeleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar anima��es
            Debug.Log("hERE");
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
        }
        #endregion

        #region  CALLBACKS DE INPUT 

        //Inicia anima��o de melee attack
        public void MeleeAttack(InputAction.CallbackContext context)
        {
            Debug.Log("Attack");
            animator.SetTrigger(isMeleeAttackingHash);
            isMeleeAttacking = true;
        }

        // Chamado quando soltamos o bot�o de melee attack
        public void StopMeleeAttack(InputAction.CallbackContext context)
        {

        }

        #endregion

    }

}
