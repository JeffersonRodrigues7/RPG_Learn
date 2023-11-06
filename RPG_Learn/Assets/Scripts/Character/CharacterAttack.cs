using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Weapon;

namespace RPG.Character.Attack
{
    public class CharacterAttack : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float damage = 100f; 

        [Header("Other")]
        [SerializeField] private GameObject weaponPrefab; // Prefab do objeto de arma
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma ser� anexada

        private Animator animator;
        private GameObject weapon; 
        private WeaponController weaponController; // Controlador da arma

        private bool isMeleeAttacking = false; // Flag para determinar se o personagem est� usando o ataque corpo a corpo
        private int isMeleeAttackingHash; // Hash da string que se refere � anima��o de ataque corpo a corpo

        public float Damage { set { damage = value; } }

        private void Start()
        {
            animator = GetComponent<Animator>();
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obt�m o hash da string da anima��o de ataque corpo a corpo
            spawnWeapon(); 
        }

        private void spawnWeapon()
        {
            weapon = Instantiate(weaponPrefab, rightHandTransform); // Instancia a arma no ponto especificado
            if (weapon != null)
            {
                weaponController = weapon.GetComponent<WeaponController>(); // Obt�m o controlador da arma
                weaponController.EnemyTag = "Player"; // Define a tag do inimigo
            }
        }

        public void startAttackAnimation()
        {
            if (isMeleeAttacking == false)
            {
                animator.SetTrigger(isMeleeAttackingHash); // Inicia a anima��o de ataque corpo a corpo
                isMeleeAttacking = true;
            }
        }

        // Chamado pela anima��o de ataque
        public void activeAttack()
        {
            weaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        // Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false; // Desativa o ataque corpo a corpo
            weaponController.IsAttacking = false; // Desativa o ataque da arma
        }
    }
}