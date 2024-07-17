using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ulian
{
    public class DamagePlayer : MonoBehaviour
    {
        private bool isTriggering = false;
        public int damage = 25;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isTriggering = true;
                StartCoroutine(TriggerStayCoroutine(other));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isTriggering = false;
            }
        }

        private IEnumerator TriggerStayCoroutine(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStats playerStatus = other.GetComponent<PlayerStats>();
                while (isTriggering)
                {
                    yield return new WaitForSeconds(0.5f);
                    playerStatus.TakeDamage(damage);
                }
            }
        }
    }
}
