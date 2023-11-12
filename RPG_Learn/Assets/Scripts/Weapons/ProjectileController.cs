using RPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Projectile
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float speed = 1;

        private HealthController target = null;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if( target != null)
            {
                transform.LookAt(GetAimLocation());
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        public void SetTarget(HealthController target)
        {
            this.target = target;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule != null)
            {
                return target.transform.position + Vector3.up * targetCapsule.height / 2;
            }
            else
            {
                return target.transform.position;
            }
            
        }
    }

}

