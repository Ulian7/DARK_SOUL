using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Ulian
{

    public class CameraHandler : MonoBehaviour
    {
        public CinemachineBrain cinemachineBrain;
        public GameObject lockOn;
        public CinemachineVirtualCamera freelook;
        public bool updateFreelook = true;
        public CinemachineVirtualCamera virtualCamera;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private LayerMask obstacleLayer;
        
        private InputHandler inputHandler;
        public Transform currentLockOnTarget;
        public Transform nearestLockOnTarget;
        public Transform leftLockOnTarget;
        public Transform rightLockOnTarget;
        public float loseTargetCount = 0;
        private float maximunLockOnDistance = 20;
        
        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        
        private float lookAngle;
        private float pivotAngle;
        public float minPivot = -35;
        public float maxPivot = 35;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            inputHandler = GameObject.Find("Player").GetComponent<InputHandler>();
            obstacleLayer = LayerMask.NameToLayer("Obstacle");
            lookAngle = transform.parent.localEulerAngles.y;
            cinemachineBrain = FindObjectOfType<CinemachineBrain>().GetComponent<CinemachineBrain>();
            lockOn = GameObject.Find("LockOn");
            lockOn.SetActive(false);
        }
        
        private void FixedUpdate()
        {
            if (inputHandler.lockOnShowed)
            {
                lockOn.GetComponent<Image>().rectTransform.position =
                    Camera.main.WorldToScreenPoint(currentLockOnTarget.position);
            }
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (inputHandler.lockOnFlag == false)
            {
                lookAngle += (mouseXInput * lookSpeed) * delta;
                pivotAngle -= (mouseYInput * pivotSpeed) * delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Linecast(virtualCamera.LookAt.position, currentLockOnTarget.position,
                        out hit) && hit.transform.gameObject.layer == obstacleLayer)
                {
           
                    loseTargetCount += Time.deltaTime;
                    if (loseTargetCount > 1.5)
                    {
                        loseTargetCount = 0;
                        inputHandler.lockOnFlag = false;
                        lockOn.SetActive(false);
                        ClearTargets();
                        return;
                    }
                }
                else
                {
                    loseTargetCount = 0;
                }

                Vector3 dir = currentLockOnTarget.position - virtualCamera.LookAt.position;
                dir.Normalize();
                dir.y = 0;
                Quaternion  targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                lookAngle = eulerAngle.y;
                myTransform.rotation = targetRotation;
                
                dir = currentLockOnTarget.position - virtualCamera.LookAt.position;
                dir.Normalize();
                targetRotation = Quaternion.LookRotation(dir);
                eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                pivotAngle = eulerAngle.x;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }

            if (updateFreelook)
            {
                freelook.transform.position = virtualCamera.transform.position;
                freelook.transform.rotation = virtualCamera.transform.rotation;
            }
        }

        public void SwitchToLockOn()
        {
            updateFreelook = false;
            Vector3 dir = currentLockOnTarget.position - virtualCamera.LookAt.position;
            dir.Normalize();
            dir.y = 0;
            Quaternion  targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            lookAngle = eulerAngle.y;
            myTransform.rotation = targetRotation;
                
            dir = currentLockOnTarget.position - virtualCamera.LookAt.position;
            dir.Normalize();
            targetRotation = Quaternion.LookRotation(dir);
            eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            pivotAngle = eulerAngle.x;
            cameraPivotTransform.localEulerAngles = eulerAngle;
            
            cinemachineBrain.m_DefaultBlend.m_Time = 0.3f;
            freelook.Priority = 9;
        }

        public void SwitchToFreeLook(bool changeTarget = false)
        {
            freelook.transform.position = virtualCamera.transform.position;
            freelook.transform.rotation = virtualCamera.transform.rotation;
            if (!changeTarget)
            {
                updateFreelook = true;
            }

            cinemachineBrain.m_DefaultBlend.m_Time = 0;
            freelook.Priority = 11;
        }
        
        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float leftAngle = 120;
            float rightAngle = -120;
            Collider[] colliders = Physics.OverlapSphere(transform.parent.position, 25);
            RaycastHit hit;
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    float distanceFromTarget =
                        Vector3.Distance(transform.parent.position, character.transform.position);
                    if (!character.CompareTag("Player") && distanceFromTarget <= maximunLockOnDistance)
                    {
                        if (Physics.Linecast(virtualCamera.transform.position, character.lockOnTransform.position,
                                out hit))
                        {
                            if (hit.transform.gameObject.layer == obstacleLayer)
                            {
                                continue;
                            }
                        }
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

        public void ClearTargets()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
        }

        public void ShowLockOn()
        {
            lockOn.GetComponent<Image>().rectTransform.position =
                Camera.main.WorldToScreenPoint(currentLockOnTarget.position);
            lockOn.SetActive(true);
        }

        public void HideLockOn()
        {
            lockOn.SetActive(false);
        }
    }
}
