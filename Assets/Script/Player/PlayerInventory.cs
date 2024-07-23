using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulian
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public int capacity = 2;
        
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = 0;
        public int currentLeftWeaponIndex = 0;

        public List<WeaponItem> weaponsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeWeapon()
        {
            float scollValue = Input.GetAxis("Mouse ScrollWheel");
            if (scollValue > 0)
            {
                int temp = (currentRightWeaponIndex + 1) % 2;
                if (weaponsInRightHandSlots[temp] != null)
                {
                    rightWeapon = weaponsInRightHandSlots[temp];
                    weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
                    currentRightWeaponIndex = temp;
                }
            }
            else
            {
                
            }
        }
    }
}