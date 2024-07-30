using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Ulian
{
    public enum next_state 
    {
        Null,
        Roll,
        Attack,
        Item
    }
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        public float mouseX_pre;
        
        public bool a_Input;
        public bool b_Input;
        public bool rb_Input;
        public bool shift_Input = false;
        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        
        public next_state inputBuffer = next_state.Null;
        public float horizontalBuffer = 0;
        public float verticalBuffer = 0;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool turnLeftFlag;
        public bool turnRightFlag;
        public bool inventoryFlag;
        public float rollInputTimer;
        public float changeLockTime;
        public bool lockOnShowed;
            
        private float scollValue;
        private float countTime;
        private float threshold = 0.5f;
        
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        
        public CameraHandler cameraHandler;
        private UIManager uiManager;
        
        Vector2 movementInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (cameraHandler != null)
            {
                cameraHandler.HandleCameraRotation(Time.deltaTime, mouseX, mouseY);
            }
        }

        private void OnMove(InputAction.CallbackContext i)
        {
            movementInput = i.ReadValue<Vector2>();
        }

        private void OnMouseX(InputAction.CallbackContext i)
        {
            mouseX = i.ReadValue<Vector2>().x;
        }
        
        private void OnMouseY(InputAction.CallbackContext i)
        {
            mouseY = i.ReadValue<Vector2>().y;
        }

        private void OnRB(InputAction.CallbackContext i)
        {
            rb_Input = true;
        }

        private void OnA(InputAction.CallbackContext i)
        {
            a_Input = true;
        }
        
        private void OnJump(InputAction.CallbackContext i)
        {
            jump_Input = true;
        }
        
        private void OnLockOn(InputAction.CallbackContext i)
        {
            lockOn_Input = true;
        }
        
        
        
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayMovement.Movement.performed += OnMove;
                inputActions.PlayMovement.Camera.performed += OnMouseX;
                inputActions.PlayMovement.Camera.performed += OnMouseY;
                inputActions.PlayerActions.RB.performed += OnRB;
                inputActions.PlayerActions.A.performed += OnA;
                inputActions.PlayerActions.Jump.performed += OnJump;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += OnLockOn;
            }

            inputActions.Enable();
        }
        
        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                shift_Input = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                shift_Input = false;
            }
            
            if (inputBuffer != next_state.Null)
            {
                countTime += Time.deltaTime;
                if (countTime > threshold)
                {
                    countTime = 0;
                    inputBuffer = next_state.Null;
                }
            }
            else
            {
                countTime = 0;
            }

            if (lockOnFlag)
            {
                if (mouseX > 0)
                {
                    if (mouseX_pre >= 0)
                    {
                        changeLockTime += Time.deltaTime;
                    }
                    else
                    {
                        changeLockTime = 0;
                    }
                }
                else if (mouseX < 0)
                {
                    if (mouseX_pre <= 0)
                    {
                        changeLockTime += Time.deltaTime;
                    }
                    else
                    {
                        changeLockTime = 0;
                    }
                }

                mouseX_pre = mouseX;
                if (changeLockTime > 1f)
                {
                    changeLockTime = 0;
                    if (mouseX_pre < 0)
                    {
                        turnLeftFlag = true;
                    }
                    else
                    {
                        turnRightFlag = true;
                    }

                    changeLockTime = 0;
                }
            }

            MoveInput(delta);
            RollInput(delta);
            AttackInput(delta);
            QuickSlotsInput();
            InventoryInput();   
            LockOnInput();
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        }

        private void RollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (b_Input)
            {
                rollInputTimer += delta;
                sprintFlag = true;
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        private void AttackInput(float delta)
        {
            if (!rb_Input && inputBuffer != next_state.Attack)
                return;
            if (playerManager.canDoCombo)
            {
                if (inputBuffer == next_state.Null)
                {
                    inputBuffer = next_state.Attack;
                    return;
                }

                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                {
                    if (inputBuffer == next_state.Null)
                        inputBuffer = next_state.Attack;
                    return;
                }

                playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        private void QuickSlotsInput()
        {
            if (shift_Input)
            {
                playerInventory.ChangeWeapon();
            }
        }

        private void InventoryInput()
        {
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag)
                {
                    ClearDelegation();
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                    Cursor.visible = true;
                }
                else
                {
                    RecoverDelegation();
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.hudWindow.SetActive(true);
                    Cursor.visible = false;
                }
            }
        }

        private void LockOnInput()
        {
            if (lockOn_Input)
            {
                if (!lockOnFlag)
                {
                    lockOn_Input = false;
                    cameraHandler.HandleLockOn();
                    if (cameraHandler.nearestLockOnTarget != null)
                    {
                        cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget; 
                        cameraHandler.SwitchToLockOn();
                        StartCoroutine("LockOn");
                        lockOnFlag = true;
                    }
                }
                else
                {
                    cameraHandler.SwitchToFreeLook();
                    lockOnShowed = false;
                    cameraHandler.HideLockOn();
                    cameraHandler.ClearTargets();
                    lockOn_Input = false;
                    lockOnFlag = false;
                }
            }

            if (lockOnFlag)
            {
                if (turnLeftFlag)
                {
                    turnLeftFlag = false;
                    cameraHandler.HandleLockOn();
                    if (cameraHandler.leftLockOnTarget != null)
                    {
                        cameraHandler.updateFreelook = false;
                        cameraHandler.SwitchToFreeLook(true);
                        cameraHandler.HideLockOn();
                        cameraHandler.currentLockOnTarget = cameraHandler.leftLockOnTarget;
                        StartCoroutine("Wait");
                    }
                }
                else if (turnRightFlag)
                {
                    turnRightFlag = false;
                    cameraHandler.HandleLockOn();
                    if (cameraHandler.rightLockOnTarget != null)
                    {
                        cameraHandler.updateFreelook = false;
                        cameraHandler.SwitchToFreeLook(true);
                        cameraHandler.HideLockOn();
                        cameraHandler.currentLockOnTarget = cameraHandler.rightLockOnTarget;
                        StartCoroutine("Wait");
                    }
                }
            }
        }

        private IEnumerator LockOn()
        {
            yield return new WaitForSeconds(0.3f);
            cameraHandler.ShowLockOn();
            lockOnShowed = true;
        }
        
        private IEnumerator Wait()
        {
            yield return null;
            cameraHandler.SwitchToLockOn();
            StartCoroutine("LockOn");
        }

        private void ClearDelegation()
        {
            inputActions.PlayMovement.Movement.performed -= OnMove;
            inputActions.PlayMovement.Camera.performed -= OnMouseX;
            inputActions.PlayMovement.Camera.performed -= OnMouseY;
            inputActions.PlayerActions.RB.performed -= OnRB;
            inputActions.PlayerActions.A.performed -= OnA;
            inputActions.PlayerActions.Jump.performed -= OnJump;
            inputActions.PlayerActions.LockOn.performed -= OnLockOn;
        }

        private void RecoverDelegation()
        {
            inputActions.PlayMovement.Movement.performed += OnMove;
            inputActions.PlayMovement.Camera.performed += OnMouseX;
            inputActions.PlayMovement.Camera.performed += OnMouseY;
            inputActions.PlayerActions.RB.performed += OnRB;
            inputActions.PlayerActions.A.performed += OnA;
            inputActions.PlayerActions.Jump.performed += OnJump;
            inputActions.PlayerActions.LockOn.performed += OnLockOn;
        }
    }
}
