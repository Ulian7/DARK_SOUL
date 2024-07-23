using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Ulian
{

    public class HandEquipmentSlotUI : MonoBehaviour
    {
        private UIManager uiManager;
        public Image icon ;
        public Sprite originIcon;
        private WeaponItem item;
        
        public bool rightHandSlot0;
        public bool rightHandSlot1;
        public bool rightHandSlot2;
        public bool leftHandSlot0;
        public bool leftHandSlot1;
        public bool leftHandSlot2;

        public void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        public void AddItem(WeaponItem newItem)
        {
            if (newItem == null)
            {
                return;
            }
                    
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
        }

        public void ClearItem()
        {
            item = null;
            icon.sprite = originIcon;
        }

        public void SelectThisSlot()
        {
            if (rightHandSlot0)
            {
                uiManager.rightHandSlot0Selected = true;
            }
            else if (rightHandSlot1)
            {
                uiManager.rightHandSlot1Selected = true;
            }
            else if (rightHandSlot2)
            {
                uiManager.rightHandSlot2Selected = true;
            }
            else if (leftHandSlot0)
            {
                uiManager.leftHandSlot0Selected = true;
            }
            else if (leftHandSlot1)
            {
                uiManager.leftHandSlot1Selected = true;
            }
            else
            {
                uiManager.leftHandSlot2Selected = true;
            }
        }
    }
}
