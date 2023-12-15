﻿using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NicerTeleporters
{
    internal class Utils
    {
        internal static void dropMostHeldItems(ref PlayerControllerB __instance, bool itemsFall = true, bool disconnecting = false)
        {

            MethodInfo SetSpecialGrabAnimationBool = typeof(PlayerControllerB).GetMethod("SetSpecialGrabAnimationBool", BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < __instance.ItemSlots.Length; i++)
            {
                GrabbableObject grabbableObject = __instance.ItemSlots[i];
                if (grabbableObject != null)
                {
                    List<string> keepList = new List<string> { "Shovel", "WalkieTalkie", "KeyItem", "FlashlightItem", "BoomboxItem" };

                    if (keepList.Contains(grabbableObject.GetType().ToString()))
                    {
                        continue;
                    }

                    if (itemsFall)
                    {
                        grabbableObject.parentObject = null;
                        grabbableObject.heldByPlayerOnServer = false;
                        if (__instance.isInElevator)
                        {
                            grabbableObject.transform.SetParent(__instance.playersManager.elevatorTransform, true);
                        }
                        else
                        {
                            grabbableObject.transform.SetParent(__instance.playersManager.propsContainer, true);
                        }
                        __instance.SetItemInElevator(__instance.isInHangarShipRoom, __instance.isInElevator, grabbableObject);
                        grabbableObject.EnablePhysics(true);
                        grabbableObject.EnableItemMeshes(true);
                        grabbableObject.transform.localScale = grabbableObject.originalScale;
                        grabbableObject.isHeld = false;
                        grabbableObject.isPocketed = false;
                        grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
                        grabbableObject.FallToGround(true);
                        grabbableObject.fallTime = UnityEngine.Random.Range(-0.3f, 0.05f);
                        if (__instance.IsOwner)
                        {
                            grabbableObject.DiscardItemOnClient();
                        }
                        else if (!grabbableObject.itemProperties.syncDiscardFunction)
                        {
                            grabbableObject.playerHeldBy = null;
                        }
                    }
                    if (__instance.IsOwner && !disconnecting)
                    {
                        HUDManager.Instance.holdingTwoHandedItem.enabled = false;
                        HUDManager.Instance.itemSlotIcons[i].enabled = false;
                        HUDManager.Instance.ClearControlTips();
                        __instance.activatingItem = false;
                    }
                    __instance.ItemSlots[i] = null;
                }
            }
            if (__instance.isHoldingObject)
            {
                __instance.isHoldingObject = false;
                if (__instance.currentlyHeldObjectServer != null)
                {
                    SetSpecialGrabAnimationBool.Invoke(__instance, new object[] { false, __instance.currentlyHeldObjectServer });
                }
                __instance.playerBodyAnimator.SetBool("cancelHolding", true);
                __instance.playerBodyAnimator.SetTrigger("Throw");
            }
            __instance.activatingItem = false;
            __instance.twoHanded = false;
            __instance.carryWeight = 1f;
            __instance.currentlyHeldObjectServer = null;
        }
        

    }
}