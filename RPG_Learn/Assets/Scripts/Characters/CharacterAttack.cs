using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Attack
{
    public class CharacterAttack : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float damage = 100f;

        [Header("Other")]
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Transform rightHandTransform;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        private bool isMeleeAttacking = false; // Flag para determinar se o character está usando o melee attack
        private int isMeleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        public float Damage { set { damage = value; } }

        private void Start()
        {
            animator = GetComponent<Animator>();
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            spawnWeapon();
        }

        private void spawnWeapon()
        {
            weapon = Instantiate(weaponPrefab, rightHandTransform);
            if (weapon != null) weaponController = weapon.GetComponent<WeaponController>();
        }

        public void startAttackAnimation()
        {
           
            if(isMeleeAttacking == false)
            {
                animator.SetTrigger(isMeleeAttackingHash);
                isMeleeAttacking = true;
            }
        }

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
    }

}
