using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.3f;
        public string interactableText;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
        }
    }
}
