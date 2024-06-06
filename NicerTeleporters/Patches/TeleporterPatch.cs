using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;


namespace NicerTeleporters.Patches
{

    // This class controls all behaviour changes to do with the normal teleporter.
    [HarmonyPatch]
    internal class TeleporterPatch
    {

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ShipTeleporter), nameof(ShipTeleporter.PressTeleportButtonClientRpc))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            // Looking for start of beamUpPlayer coroutine to insert altered method.

            return new CodeMatcher(instructions)
                .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ShipTeleporter), "beamUpPlayer")))
                .ThrowIfInvalid("Method not found.")
                .SetOperandAndAdvance(AccessTools.Method(typeof(TeleporterPatch), nameof(TeleporterPatch.betterBeamUpPlayer))).InstructionEnumeration();
        }

        static IEnumerator betterBeamUpPlayer(ShipTeleporter __instance)
        {
            // Getting necessary private methods.

            MethodInfo SetPlayerTeleporterId = typeof(ShipTeleporter).GetMethod("SetPlayerTeleporterId", BindingFlags.NonPublic | BindingFlags.Instance);

            // Altered method to stop players dropping items.

            __instance.shipTeleporterAudio.PlayOneShot(__instance.teleporterSpinSFX);
            PlayerControllerB playerToBeamUp = StartOfRound.Instance.mapScreen.targetedPlayer;
            if (playerToBeamUp == null)
            {
                Debug.Log("Targeted player is null");
                yield break;
            }
            if (playerToBeamUp.redirectToEnemy != null)
            {
                Debug.Log($"Attemping to teleport enemy '{playerToBeamUp.redirectToEnemy.gameObject.name}' (tied to player #{playerToBeamUp.playerClientId}) to ship.");
                if (StartOfRound.Instance.shipIsLeaving)
                {
                    Debug.Log($"Ship could not teleport enemy '{playerToBeamUp.redirectToEnemy.gameObject.name}' (tied to player #{playerToBeamUp.playerClientId}) because the ship is leaving the nav mesh.");
                }
                playerToBeamUp.redirectToEnemy.ShipTeleportEnemy();
                yield return new WaitForSeconds(3f);
                __instance.shipTeleporterAudio.PlayOneShot(__instance.teleporterBeamUpSFX);
                if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
                {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                }
            }
            SetPlayerTeleporterId.Invoke(__instance, new object[] { playerToBeamUp, -1 });
            if (playerToBeamUp.deadBody != null)
            {
                if (playerToBeamUp.deadBody.beamUpParticle == null)
                {
                    yield break;
                }
                playerToBeamUp.deadBody.beamUpParticle.Play();
                playerToBeamUp.deadBody.bodyAudio.PlayOneShot(__instance.beamUpPlayerBodySFX);
            }
            else
            {
                playerToBeamUp.beamUpParticle.Play();
                playerToBeamUp.movementAudio.PlayOneShot(__instance.beamUpPlayerBodySFX);
            }
            Debug.Log("Teleport A");
            yield return new WaitForSeconds(3f);
            bool flag = false;
            if (playerToBeamUp.deadBody != null)
            {
                if (playerToBeamUp.deadBody.grabBodyObject == null || !playerToBeamUp.deadBody.grabBodyObject.isHeldByEnemy)
                {
                    flag = true;
                    playerToBeamUp.deadBody.attachedTo = null;
                    playerToBeamUp.deadBody.attachedLimb = null;
                    playerToBeamUp.deadBody.secondaryAttachedLimb = null;
                    playerToBeamUp.deadBody.secondaryAttachedTo = null;
                    playerToBeamUp.deadBody.SetRagdollPositionSafely(__instance.teleporterPosition.position, disableSpecialEffects: true);
                    playerToBeamUp.deadBody.transform.SetParent(StartOfRound.Instance.elevatorTransform, worldPositionStays: true);
                    if (playerToBeamUp.deadBody.grabBodyObject != null && playerToBeamUp.deadBody.grabBodyObject.isHeld && playerToBeamUp.deadBody.grabBodyObject.playerHeldBy != null)
                    {
                        playerToBeamUp.deadBody.grabBodyObject.playerHeldBy.DropAllHeldItems();
                    }
                }
            }
            else
            {
                flag = true;
                Utils.DropMostHeldItems(ref playerToBeamUp, true, false);
                if ((bool)UnityEngine.Object.FindObjectOfType<AudioReverbPresets>())
                {
                    UnityEngine.Object.FindObjectOfType<AudioReverbPresets>().audioPresets[3].ChangeAudioReverbForPlayer(playerToBeamUp);
                }
                playerToBeamUp.isInElevator = true;
                playerToBeamUp.isInHangarShipRoom = true;
                playerToBeamUp.isInsideFactory = false;
                playerToBeamUp.averageVelocity = 0f;
                playerToBeamUp.velocityLastFrame = Vector3.zero;
                playerToBeamUp.TeleportPlayer(__instance.teleporterPosition.position, withRotation: true, 160f);
            }
            Debug.Log("Teleport B");
            SetPlayerTeleporterId.Invoke(__instance, new object[] { playerToBeamUp, -1 });
            if (flag)
            {
                __instance.shipTeleporterAudio.PlayOneShot(__instance.teleporterBeamUpSFX);
                if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
                {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                }
            }
            Debug.Log("Teleport C");

        }





    }
}
