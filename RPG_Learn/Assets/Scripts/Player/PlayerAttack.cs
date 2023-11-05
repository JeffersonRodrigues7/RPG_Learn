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

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int isMeleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }

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
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            spawnWeapon();
        }

        private void spawnWeapon()
        {
            weapon = Instantiate(weaponPrefab, rightHandTransform);
            if(weapon != null) weaponController = weapon.GetComponent<WeaponController>();
        }

        #endregion

        #region  FUN��ES DE ANIMA��O
        //Chamado atrav�s da anima��o de ataque
        public void activeAttack()
        {
            weaponController.IsAttacking = true;
        }

        //Chamado atrav�s da anima��o de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false;
            weaponController.IsAttacking = false;
        }
        #endregion

        #region  CALLBACKS DE INPUT 

        //Inicia anima��o de melee attack
        public void MeleeAttack(InputAction.CallbackContext context)
        {
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
