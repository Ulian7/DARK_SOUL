using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ulian
{
    public class EquipmentWindowUI : MonoBehaviour
    {
        public bool rightHandSlot0Selected;
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool leftHandSlot0Selected;
        public bool leftHandSlot1Selected;
        public bool leftHandSlot2Selected;

        private HandEquipmentSlotUI[] handEquipmentSlotUI;

        private void Start()
        {
            handEquipmentSlotUI = GetComponentsInChildren<HandEquipmentSlotUI>();
        }

        public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
        {
            for (int i = 0; i < handEquipmentSlotUI.Length; i++)
            {
                HandEquipmentSlotUI temp = handEquipmentSlotUI[i];
                if (temp.rightHandSlot0)
                {
                    temp.AddItem(playerInventory.weaponsInRightHandSlots[0]);
                }
                else if (temp.rightHandSlot1)
                {
                    temp.AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if (temp.rightHandSlot2)
                {
                    temp.AddItem(playerInventory.weaponsInRightHandSlots[2]);
                }
                else if (temp.leftHandSlot0)
                {
                    temp.AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else if (temp.leftHandSlot1)
                {
                    temp.AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
                else 
                {
                    temp.AddItem(playerInventory.weaponsInLeftHandSlots[2]);
                }
            }

            
        }

        public void SelectRightHandSlot0()
        {
            rightHandSlot0Selected = true;
        }
        
        public void SelectRightHandSlot1()
        {
            rightHandSlot1Selected = true;
        }
        
        public void SelectRightHandSlot2()
        {
            rightHandSlot2Selected = true;
        }
        
        public void SelectLeftHandSlot0()
        {
            leftHandSlot0Selected = true;
        }
        
        public void SelectLeftHandSlot1()
        {
            leftHandSlot1Selected = true;
        }
        
        public void SelectLeftHandSlot2()
        {
            leftHandSlot2Selected = true;
        }
    }
}