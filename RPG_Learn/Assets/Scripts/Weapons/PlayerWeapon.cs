using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Character.Health;


namespace RPG.Player.Weapon
{
    public class PlayerWeapon : MonoBehaviour
    {
        private bool isAttacking = false; // Flag para determinar se a arma est� atacando

        public bool IsAttacking { set { isAttacking = value; } }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttacking)
            {
                CharacterHealth characterHealth = other.gameObject?.GetComponent<CharacterHealth>();
                if(characterHealth != null)
                {
                    characterHealth.takeDamage(10f);
                }
            }  
        }
    }

}
