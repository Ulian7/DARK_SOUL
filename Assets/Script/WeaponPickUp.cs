using System.Collections;
using System.Collections.Generic;
using Ulian;
using UnityEngine;

namespace Ulian
{
    public class WeaponPickUp : Interactable
    {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
            PlayerLocomotion playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            AnimatorHandler animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

            //playerLocomotion.characterController.SimpleMove(Vector3.zero);
            animatorHandler.PlayTargetAnimation("Pick Up Item", true, false);
            playerInventory.weaponsInventory.Add(weapon);
            Destroy(gameObject);
        }
    }
}