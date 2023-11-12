using RPG.Health;
using RPG.Projectile;
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
        [SerializeField] private ProjectileController projectileController = null;
        [SerializeField] private List<string> projectileTagsToExclude = new List<string> { "Weapon", "Detection" };

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
            projectileTagsToExclude.Add(gameObject.tag);
            isMeleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            spawnWeapon();
        }

        private void spawnWeapon()
        {
            weapon = Instantiate(weaponPrefab, rightHandTransform);
            if (weapon != null) 
            {
                weaponController = weapon.GetComponent<WeaponController>();
                weaponController.EnemyTag = "Enemy";
            }
            
        }

        #endregion

        #region  ATTACK

        private bool HasProjectile()
        {
            return projectileController != null;
        }

        private void LaunchProjectile(Transform rightHand, Vector3 target)
        {
            ProjectileController projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(target);
        }

        #endregion

        #region  FUNÇÕES DE ANIMAÇÃO
        //Chamado através da animação de ataque
        public void activeAttack()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            //foreach (RaycastHit hit in hits)
            //{
            //    Debug.Log("Ponto Inicial: " + hit.transform.tag);
            //}

            foreach (RaycastHit hit in hits)
            {

                if (!projectileTagsToExclude.Contains(hit.collider.tag))
                {
                    Debug.Log("Ponto Inicial: " + hit.transform.tag);
                    LaunchProjectile(rightHandTransform, hit.point);
                    break;
                }
            }
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
