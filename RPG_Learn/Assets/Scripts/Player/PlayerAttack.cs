using RPG.Weapon;
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
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Transform rightHandTransform;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int isMeleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar animações
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            spawnWeapon();
        }

        private void spawnWeapon()
        {
            weapon = Instantiate(weaponPrefab, rightHandTransform);
            if(weapon != null) weaponController = weapon.GetComponent<WeaponController>();
        }

        #endregion

        #region  FUNÇÕES DE ANIMAÇÃO
        //Chamado através da animação de ataque
        public void activeAttack()
        {
            weaponController.IsAttacking = true;
        }

        //Chamado através da animação de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false;
            weaponController.IsAttacking = false;
        }
        #endregion

        #region  CALLBACKS DE INPUT 

        //Inicia animação de melee attack
        public void MeleeAttack(InputAction.CallbackContext context)
        {
            animator.SetTrigger(isMeleeAttackingHash);
            isMeleeAttacking = true;
        }

        // Chamado quando soltamos o botão de melee attack
        public void StopMeleeAttack(InputAction.CallbackContext context)
        {

        }

        #endregion

    }

}
