using RPG.Health;
using RPG.Projectile;
using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace RPG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        #region VARIABLES DECLARATION
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private ProjectileController projectileController = null;
        [SerializeField] private List<string> projectileTagsToExclude = new List<string> { "Weapon", "Detection" };
        [SerializeField] private Transform ArrowParents;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        ProjectileController projectileInstance;

        private bool isUsingSword = false;//�true-> weapon atual � a espada; false -> weapon atua�l � o arco

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

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
            projectileTagsToExclude.Add(gameObject.tag);
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            spawnWeapon(swordPrefab, rightHandTransform);
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            isUsingSword = !isUsingSword;

            if (weapon != null)
            {
                Destroy(weapon);
            }

            if(weaponPrefab != null)
            {
                
                weapon = Instantiate(weaponPrefab, hand);
                weaponController = weapon.GetComponent<WeaponController>();
                weaponController.EnemyTag = "Enemy";
            }

        }

        #endregion

        #region  FUN��ES DE ANIMA��O
        //Chamado atrav�s da anima��o de ataque
        public void activeAttack()
        {

            weaponController.IsAttacking = true;
        }

        public void shootArrow()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (RaycastHit hit in hits)
            {
                if (!projectileTagsToExclude.Contains(hit.collider.tag))
                {
                    projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity, ArrowParents);
                    projectileInstance.SetTarget(hit.point, "Enemy");
                    Destroy(projectileInstance.gameObject, 10f);
                    return;
                }
            }

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileInstance.SetTarget(worldMousePosition, "Enemy");
            Destroy(projectileInstance.gameObject, 10f);

        }

        //Chamado atrav�s da anima��o de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false;
            isRangedAttacking = false;
            weaponController.IsAttacking = false;
        }
        #endregion

        #region  CALLBACKS DE INPUT 

        //Inicia anima��o de melee attack
        public void Attack(InputAction.CallbackContext context)
        {
            if (isUsingSword)
            {
                animator.SetTrigger(meleeAttackingHash);
                isMeleeAttacking = true;
            }
            else
            {
                animator.SetTrigger(rangedAttackingHash);
                isRangedAttacking = true;
            }

        }

        // Chamado quando soltamos o bot�o de melee attack
        public void StopAttack(InputAction.CallbackContext context)
        {

        }

        //Inicia anima��o de melee attack
        public void ChangeWeapon(InputAction.CallbackContext context)
        {
            if (isUsingSword)
            {
                spawnWeapon(bowPrefab, leftHandTransform);
            }

            else
            {
                spawnWeapon(swordPrefab, rightHandTransform);
            }
        }

        // Chamado quando soltamos o bot�o de melee attack
        public void StopChangeWeapon(InputAction.CallbackContext context)
        {

        }

        #endregion

    }

}
