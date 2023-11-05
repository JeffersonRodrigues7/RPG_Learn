using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Health;


namespace RPG.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private string enemyTag = "Enemy";

        private bool isAttacking = false; // Flag para determinar se a arma está atacando

        public bool IsAttacking { set { isAttacking = value; } }
        public string EnemyTag { set { enemyTag = value; } }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttacking)
            {
                if(other.tag == enemyTag)
                {
                    HealthController healthController = other.gameObject?.GetComponent<HealthController>();

                    if (healthController != null)
                    {
                        healthController.takeDamage(damage);
                    }
                }

            }  
        }
    }

}
