using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public bool a_Input;
        public bool b_Input;
        public bool rb_Input;
        public bool shift_Input = false;
        public bool jump_Input;
        public bool inventory_Input;
        
        public next_state inputBuffer = next_state.Null;
        public float horizontalBuffer = 0;
        public float verticalBuffer = 0;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryFlag;
        public float rollInputTimer;

        private float scollValue;

        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        private UIManager uiManager;

        Vector2 movementInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                //TODO: 为什么Horizontal和Vertical只会是整数
                inputActions.PlayMovement.Movement.performed +=
                    inputActions => movementInput = inputActions.ReadValue<Vector2>();
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
            
            MoveInput(delta);
            RollInput(delta);
            AttackInput(delta);
            QuickSlotsInput();
            InteractingButtonInput();
            JumpInput();
            InventoryInput();   
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
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;

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

            inputBuffer = next_state.Null;
        }

        private void QuickSlotsInput()
        {
            if (shift_Input)
            {
                playerInventory.ChangeWeapon();
            }
        }

        private void InteractingButtonInput()
        {
            inputActions.PlayerActions.A.performed += i => a_Input = true;
        }

        private void JumpInput()
        {
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
        }

        private void InventoryInput()
        {
            inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.hudWindow.SetActive(true);
                }
            }
        }
    }
}
