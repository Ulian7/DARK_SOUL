using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ulian
{
    public class GhostLocomotion : MonoBehaviour
    {
        public Transform player;
        public float speed = 0.3f;
        void Update()
        {
            Vector3 position = Vector3.Lerp(this.transform.position, player.position, speed * Time.deltaTime);
            this.transform.position = position;
        }
    }
}
