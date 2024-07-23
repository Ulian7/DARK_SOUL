using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ulian
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        private EnemyManager enemyManager;

        private CharacterStats currentTarget;
        private LayerMask detectionLayer; 
        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
        }

        public void HandleDetection()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();
                if (characterStats != null)
                {
                    Vector3 targetDir = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDir, transform.forward);

                    if (viewableAngle > enemyManager.minDetectionAngle &&
                        viewableAngle < enemyManager.maxDetectionAngle)
                    {
                        currentTarget = characterStats;
                    }
                }
            }
        }
    }
}
