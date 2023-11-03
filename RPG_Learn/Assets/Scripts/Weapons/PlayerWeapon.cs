using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Player.Weapon
{
    public class PlayerWeapon : MonoBehaviour
    {
        private bool isAttacking = false; // Flag para determinar se a arma está atacando

        public bool IsAttacking { set { isAttacking = value; } }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttacking)
            {
                Debug.Log(other.name);
            }  
        }
    }

}
