using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
        }
    }

}
