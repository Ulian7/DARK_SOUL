using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class EnemyStats : CharacterStats
    {
        public Animator animator;
        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                animator.Play("Dead_01");
            }
            else
            {
                animator.Play("Damage_01");
            }
        }
    }
}