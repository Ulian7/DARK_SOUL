using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class EnemyManager : CharacterManager
    {
        public EnemyLocomotionManager enemyLocomotionManager;
        public EnemyAnimatorManager enemyAnimatorManager;
        public bool isPerformingAction;
        [Header("A.I Settings")] 
        public float detectionRadius = 20;

        public float maxDetectionAngle = 50;
        public float minDetectionAngle = -50;
        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void FixedUpdate()
        {
            HandleCurrentAction();
        }

        private void HandleCurrentAction()
        {
            if (enemyLocomotionManager == null)
            {
                enemyLocomotionManager.HandleDetection();
            }
            else
            {
                //enemyLocomotionManager.HandleMoveToTarget();
            }
        }
    }
}
