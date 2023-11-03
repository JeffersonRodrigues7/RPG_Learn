using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Character.Health
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        public Slider healthSlider;

        void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }

        public void TakeDamage(float damage)
        {
            Debug.Log(damage);
            currentHealth -= damage;
            UpdateHealthUI();
        }

        void UpdateHealthUI()
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth / maxHealth;
            }
        }
    }

}

