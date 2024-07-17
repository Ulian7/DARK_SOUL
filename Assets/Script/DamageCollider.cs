using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        public int currentWeaponDamage = 30;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

       public void EnableDamageCollider()
        {
            damageCollider.enabled = true ;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collsion)
        {
            if (collsion.tag == "Player" && tag != "Player's")
            {
                PlayerStats playerStats = collsion.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if (collsion.tag == "Enemy")
            {
                EnemyStats enemyStats = collsion.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}