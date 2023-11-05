using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Character.Health
{
    public class CharacterHealth : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Other")]
        [SerializeField] private float currentHealth;
        [SerializeField] private Slider healthSlider;

        public float MaxHealth { set { maxHealth = value; } }

        private void Start()
        {
            if(healthSlider == null)
            {
                healthSlider = GetComponentInChildren<Slider>();
            }

            currentHealth = maxHealth;
            updateHealthUI();
        }

        public void takeDamage(float damage)
        {
            currentHealth -= damage;
            updateHealthUI();
        }

        private void updateHealthUI()
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth / maxHealth;
            }
        }
    }

}

