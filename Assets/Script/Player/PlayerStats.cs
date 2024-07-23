using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class PlayerStats : CharacterStats
    {
        public float targetStamina = 100;
        public float recoverSpeed = 10;
        
        public HealthBar healthBar;
        public StaminaBar staminaBar;
        AnimatorHandler animatorHandler;
        // Start is called before the first frame update
        void Start()
        {
            healthBar = GameObject.Find("Player UI").GetComponentInChildren<HealthBar>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            
            staminaBar = GameObject.Find("Player UI").GetComponentInChildren<StaminaBar>();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
            
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                animatorHandler.PlayTargetAnimation("Dead_01", true, false);
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            staminaBar.SetCurrentStamina(currentStamina);
        }

        public void RecoverStamina(float delta)
        {
            currentStamina = Mathf.Min(currentStamina + delta * recoverSpeed, targetStamina);
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }
}