using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Ulian
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        private PlayerInventory playerInventory;
        private WeaponSlotManager weaponSlotManager;
        private UIManager uiManager;
        public EquipmentWindowUI equipmentWindowUI;
        public GameObject weaponInventoryWindow;
        
        public Image icon ;
        private WeaponItem item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            if (uiManager.rightHandSlot0Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[0] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]); 
                }
                playerInventory.weaponsInRightHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.rightHandSlot1Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[1] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]); 
                }
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.rightHandSlot2Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[2] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[2]); 
                }
                playerInventory.weaponsInRightHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot0Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[0] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]); 
                }
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot1Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[1] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]); 
                }
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else
            {
                if (playerInventory.weaponsInLeftHandSlots[2] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[2]); 
                }
                playerInventory.weaponsInLeftHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
            }

            playerInventory.rightWeapon =
                playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
            
            playerInventory.leftWeapon =
                playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
            
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
            uiManager.UpdateUI();
            uiManager.ResetAllSelectedSlots();
            uiManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
            equipmentWindowUI.GameObject().SetActive(true);
            weaponInventoryWindow.SetActive(false);
        }
    }
}