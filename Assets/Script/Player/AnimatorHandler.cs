using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ulian
{
    public class AnimatorHandler : AnimatorManager
    {
        PlayerManager playerManager;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        PlayerAttacker playerAttacker;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            playerAttacker = GetComponentInParent<PlayerAttacker>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            anim.SetBool("canJump", true);
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            float v = 0;

            if (verticalMovement > 0)
            {
                v = 0.5f;
            }
            else if (verticalMovement < 0)
            {
                v = -0.5f;
            }
            
            float h = 0;

            if (horizontalMovement > 0)
            {
                h = 0.5f;
            }
            else if (horizontalMovement < 0)
            {
                h = -0.5f;
            }

            if (isSprinting && verticalMovement != 0)
            {
                v = 1;
                h = horizontalMovement;
            }
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool isAttacking)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            if (!isAttacking)
            {
                playerAttacker.lastAttack = "";
            }
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotate()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;
            
            
            float delta = Time.deltaTime;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.characterController.SimpleMove(velocity);
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableJump()
        {
            anim.SetBool("canJump", true);
        }
        
        public void DisableJump()
        {
            anim.SetBool("canJump", false);
        }
    }
}