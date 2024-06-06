using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NicerTeleporters.Patches
{
    // This class controls all behaviour changes to do with the inverse teleporter.
    [HarmonyPatch]
    internal class InverseTeleporterPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTeleporter), "Awake")]
        private static void Awake(ShipTeleporter __instance)
        {
            if (__instance.isInverseTeleporter)
            {
                __instance.cooldownAmount = 45f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTeleporter), "TeleportPlayerOutWithInverseTeleporter")]
        static bool inverseTeleport(int playerObj, ref Vector3 teleportPos, ShipTeleporter __instance)
        {
            // Getting necessary private methods.

            MethodInfo teleportBodyOut = typeof(ShipTeleporter).GetMethod("teleportBodyOut", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo SetPlayerTeleporterId = typeof(ShipTeleporter).GetMethod("SetPlayerTeleporterId", BindingFlags.NonPublic | BindingFlags.Instance);

            // Altered method to stop players dropping items.

            if (StartOfRound.Instance.allPlayerScripts[playerObj].isPlayerDead)
            {
                __instance.StartCoroutine((IEnumerator)teleportBodyOut.Invoke(__instance, new object[] { playerObj, teleportPos }));
                return false;
            }

            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerObj];
            SetPlayerTeleporterId.Invoke(__instance, new object[] { playerControllerB, -1 });

            Utils.DropMostHeldItems(ref playerControllerB, true, false);

            if ((bool)UnityEngine.Object.FindObjectOfType<AudioReverbPresets>())
            {
                UnityEngine.Object.FindObjectOfType<AudioReverbPresets>().audioPresets[2].ChangeAudioReverbForPlayer(playerControllerB);
            }

            playerControllerB.isInElevator = false;
            playerControllerB.isInHangarShipRoom = false;
            playerControllerB.isInsideFactory = true;
            playerControllerB.averageVelocity = 0f;
            playerControllerB.velocityLastFrame = Vector3.zero;

            StartOfRound.Instance.allPlayerScripts[playerObj].TeleportPlayer(teleportPos);
            StartOfRound.Instance.allPlayerScripts[playerObj].beamOutParticle.Play();
            __instance.shipTeleporterAudio.PlayOneShot(__instance.teleporterBeamUpSFX);
            StartOfRound.Instance.allPlayerScripts[playerObj].movementAudio.PlayOneShot(__instance.teleporterBeamUpSFX);

            if (playerControllerB == GameNetworkManager.Instance.localPlayerController)
            {
                Debug.Log("Teleporter shaking camera");
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }

            return false;
        }

    }

}
