using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ulian
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;
        public float rollCost = 20;
        public float sprintCost = 5;
        private PlayerStats playerStats;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public CharacterController characterController;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f;
        [SerializeField]
        float minimumDistanceNeedToBeginFall = 1;
        [SerializeField]
        float groundDetectionRayDistance = 0.3f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            characterController = GetComponent<CharacterController>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);

            playerStats = playerManager.GetComponent<PlayerStats>();
        }

        #region Movement

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if (playerManager.isInteracting)
                return;

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.y = 0;
            moveDirection.Normalize();

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && playerStats.currentStamina < sprintCost * delta)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                moveDirection *= speed;
            }

            characterController.SimpleMove(moveDirection);

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (!inputHandler.rollFlag && inputHandler.inputBuffer != next_state.Roll)
                return;
            
            if (animatorHandler.anim.GetBool("isInteracting"))
            {
                if (inputHandler.inputBuffer == next_state.Null && inputHandler.rollFlag)
                {
                    if (inputHandler.inputBuffer == next_state.Null)
                    {
                        inputHandler.inputBuffer = next_state.Roll;
                        inputHandler.verticalBuffer = inputHandler.vertical;
                        inputHandler.horizontalBuffer = inputHandler.horizontal;
                    }
                }

                return;
            }

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
            }
            else
            {
                moveDirection = cameraObject.forward * inputHandler.verticalBuffer;
                moveDirection += cameraObject.right * inputHandler.horizontalBuffer;
            }


            if (playerStats.currentStamina < rollCost)
                return;
                
            if (moveDirection != Vector3.zero)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true, false);
                playerStats.TakeStaminaDamage(20);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true, false);
                playerStats.TakeStaminaDamage(10);
            }

            inputHandler.inputBuffer = next_state.Null;
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            origin = origin - myTransform.forward * groundDetectionRayDistance;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeedToBeginFall, Color.red, 0.1f, false);
            if (!characterController.isGrounded)
            {
                if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeedToBeginFall, ignoreForGroundCheck))
                {
                    playerManager.isGrounded = true;

                    if (playerManager.isInAir)
                    {
                        if (inAirTimer > 0.4f)
                        {
                            animatorHandler.PlayTargetAnimation("Land", true, false);
                        }
                        else
                        {
                            animatorHandler.PlayTargetAnimation("Empty", false, false);
                            inAirTimer = 0;
                        }
                        playerManager.isInAir = false;
                    }
                }
                else
                {
                    if (playerManager.isGrounded)
                    {
                        playerManager.isGrounded = false;
                    }

                    if (playerManager.isInAir == false)
                    {
                        playerManager.isInAir = true;
                    }
                    
                    if (playerManager.isInteracting == false && inAirTimer > 0.2f)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true, false);
                    }
                }
            }
        }

        public void HandleJumping()
        {
            if (playerManager.isInteracting || !playerManager.canJump)
                return;
            if (inputHandler.jump_Input)
            {
                if (inputHandler.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
                animatorHandler.PlayTargetAnimation("Jump", false, false);
            }
        }
        #endregion
    }
}