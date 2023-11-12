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

        private Vector3 hit = Vector3.zero;

        public string EnemyTag { set { enemyTag = value; } }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(GetAimLocation());
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            
        }

        public void SetTarget(Vector3 target)
        {
            this.hit = target;
        }

        private Vector3 GetAimLocation()
        {
            return hit;


        }
    }

}

