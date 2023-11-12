using RPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Projectile
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private float damage = 15f;

        private Vector3 target = Vector3.zero;

        HealthController healthController;

        public string EnemyTag { set { enemyTag = value; } }

        void Update()
        {
            if(target != null)
            {
                transform.LookAt(target);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        public void SetTarget(Vector3 _target)
        {
            target = _target;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals(enemyTag))
            {
                healthController = other.gameObject?.GetComponent<HealthController>();

                if (healthController != null)
                {
                    healthController.takeDamage(damage);
                }
                Destroy(gameObject);
            }
            
        }

    }

}

