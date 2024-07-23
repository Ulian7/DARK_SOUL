using System;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Ulian
{
    public class CameraManager : MonoBehaviour
    {
        private CinemachineFreeLook cinemachineFreeLook;
        private InputHandler inputHandler;
        public Transform currentLockOnTarget;
        public Transform nearestLockOnTarget;
        public Transform leftLockOnTarget;
        public Transform rightLockOnTarget;
        public Transform follow;
        public Transform lookat;
        public CinemachineFreeLook adjustCamera;
        private float maximunLockOnDistance = 15;
        private void Awake()
        {
            cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
            inputHandler = GameObject.Find("Player").GetComponent<InputHandler>();
            follow = GameObject.Find("Ghost(Clone)").transform;
            lookat = follow.Find("LookAt");
            cinemachineFreeLook.Follow = follow;
            cinemachineFreeLook.LookAt = lookat;
            cinemachineFreeLook.m_YAxis.Value = 0.7f;
            adjustCamera = GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>();
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float leftAngle = 100;
            float rightAngle = -100;
            Collider[] colliders = Physics.OverlapSphere(transform.parent.position, 25);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    float distanceFromTarget =
                        Vector3.Distance(transform.parent.position, character.transform.position);
                    if (character.transform.root != transform.parent.root && distanceFromTarget <= maximunLockOnDistance)
                    {
                        if (distanceFromTarget < shortestDistance)
                        {
                            shortestDistance = distanceFromTarget;
                            nearestLockOnTarget = character.lockOnTransform;
                        }

                        if (inputHandler.lockOnFlag)
                        {
                            Vector2 now = new Vector2(currentLockOnTarget.position.x - transform.parent.position.x,
                                currentLockOnTarget.position.z - transform.parent.position.z);
                            Vector2 next = new Vector2(character.transform.position.x - transform.parent.position.x,
                                character.transform.position.z - transform.parent.position.z);
                            float angle = Vector2.SignedAngle(now, next);
                            if (angle > 0 && angle <= 135 && angle < leftAngle)
                            {
                                leftAngle = angle;
                                leftLockOnTarget = character.lockOnTransform;
                            }

                            if (angle>= -135 && angle < 0 && angle > rightAngle)
                            {
                                rightAngle = angle;
                                rightLockOnTarget = character.lockOnTransform;
                            }
                        }
                    }
                }
            }
        }

        public void SwitchToLockOn()
        {
            if (adjustCamera.Priority > 9)
            {
                adjustCamera.Priority = 9;
            }
        }

        public void SwitchToFreeLook()
        {
            Vector3 dir = lookat.position - currentLockOnTarget.position;
            Vector3 targetPos = dir.normalized * 3 + lookat.position;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            adjustCamera.ForceCameraPosition(targetPos, rotation);
            adjustCamera.Priority = 11;
        }
        
        public void AdjustPosition()
        {
            //cinemachineFreeLook.Follow.LookAt(cinemachineFreeLook.LookAt);
            Vector3 dir = lookat.position - currentLockOnTarget.position;
            Vector3 targetPos = dir.normalized * 3 + lookat.position;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            cinemachineFreeLook.ForceCameraPosition(targetPos, rotation);
            SwitchToLockOn();
        }

        public void EnableFreeLook()    
        {
            adjustCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            adjustCamera.m_XAxis.m_InputAxisName = "Mouse X";
        }

        public void DisableFreeLook()
        {
            adjustCamera.m_YAxis.m_InputAxisName = "";
            adjustCamera.m_XAxis.m_InputAxisName = "";
        }
    }
}
