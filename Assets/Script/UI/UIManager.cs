using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class UIManager : MonoBehaviour
    {
        public PlayerInventory playerInventory;
        public EquipmentWindowUI equipmentWindowUI;

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        public GameObject equipmentWindow;
        public GameObject weaponInventoryWindow;

        [Header("Equipment Window Slot Selected")]
        public bool rightHandSlot0Selected;
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool leftHandSlot0Selected;
        public bool leftHandSlot1Selected;
        public bool leftHandSlot2Selected;
        
        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab;
        public Transform weaponInventorySlotsParent;
        public WeaponInventorySlot[] weaponInventorySlots;

        private void Awake()
        {
            equipmentWindowUI = FindObjectOfType<EquipmentWindowUI>();
        }

        private void Start()
        {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
        }

        public void UpdateUI()
        {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++)
            {
                if (i < playerInventory.weaponsInventory.Count)
                {
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                    {
                        GameObject slot = Instantiate(weaponInventorySlotPrefab);
                        slot.transform.parent = weaponInventorySlotsParent;
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }
            #endregion
        }
        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }

        public void CloseAllInventoryWindows()
        {
            weaponInventoryWindow.SetActive(false);
            equipmentWindow.SetActive(false);
        }

        public void InitializeEquipmentWindow()
        {
            equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
            equipmentWindow.SetActive(false);
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot0Selected = false;
            rightHandSlot1Selected = false;
            rightHandSlot2Selected = false;
            
            leftHandSlot0Selected = false;
            leftHandSlot1Selected = false;
            leftHandSlot2Selected = false;
        }
    }
}
