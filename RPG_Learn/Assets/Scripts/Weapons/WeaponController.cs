using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Character.Health;


namespace RPG.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        private bool isAttacking = false; // Flag para determinar se a arma está atacando

        public bool IsAttacking { set { isAttacking = value; } }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttacking)
            {
                Debug.Log(other.name);
                CharacterHealth characterHealth = other.gameObject?.GetComponent<CharacterHealth>();
                if(characterHealth != null)
                {
                    characterHealth.takeDamage(10f);
                }
            }  
        }
    }

}
