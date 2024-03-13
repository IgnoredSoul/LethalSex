using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalSex_Core
{
    /// <summary>
    /// Represents a base class for managing and coordinating modules.
    /// </summary>
    [HarmonyPatch]/* I dont know how harmony fucking works so idk if this is required or not 💀💀💀 */
    public abstract class LethalClass : MonoBehaviour
    {
        /// <summary>
        /// List containing all instances of classes derived from LethalClass.
        /// </summary>
        protected static List<LethalClass> LethalClasses = new List<LethalClass>();

        /// <summary>
        /// Indicates whether this module is registered in the list of LethalClasses.
        /// </summary>
        public bool Registered => LethalClasses.Contains(this);

        #region ===================[ BaseClass Registration ]===================

        /// <summary>
        /// Registers all subclasses of LethalClass.
        /// </summary>
        internal static void RegisterAll()
        {
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    LethalClasses.AddRange(a.GetTypes().Where(type => type.IsSubclassOf(typeof(LethalClass))).Select(type => (LethalClass)Activator.CreateInstance(type)));

                _handle_OnRegister();

                ConsoleManager.Log($"All Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "LethalClass", Color.magenta);
            }
            catch (Exception ex)
            {
                ConsoleManager.LogErr($"Error getting subclasses: {ex}");
            }
        }

        /// <summary>
        /// Unregisters this module from the list of LethalClasses.
        /// </summary>
        protected void Unregister()
        {
            LethalClasses.Remove(this);
            ConsoleManager.Log($"Registered: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "LethalClass", Color.magenta);
        }

        #endregion ===================[ BaseClass Registration ]===================

        /// <summary>
        /// Called when the behavior is enabled.
        /// </summary>
        protected virtual void Enable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been enabled");
        }

        /// <summary>
        /// Called when the behavior is disabled.
        /// </summary>
        protected virtual void Disable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been disabled");
        }

        /// <summary>
        /// Called when the behavior is destroyed.
        /// </summary>
        protected virtual void Destroy()
        {
            LethalClasses.Remove(this);
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been destroyed");
        }

        #region =====================[ Patches ]=====================

        /// <summary>
        /// Handles the patch for HUDManager's Start method.
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        private static void _handle_OnHUDStart()
        {
            start();

            LethalClasses.ForEach(async c =>
            {
                try
                {
                    // Call for HUDManager Start
                    c?.OnHUDStart();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnHUDStart;\n{e}\n"); }

                try
                {
                    // Call LocalPlayer Start
                    c?.OnLocalPlayerStart(await LocalPlayer.PlayerControllerAsync());
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnLocalPlayerStart;\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for HUDManager's Awake method.
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        private static void _handle_OnHUDAwake()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call HUDManager Awake
                    c?.OnHUDAwake();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnHUDAwake;\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for HUDManager's Update method.
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void _handle_OnHUDUpdate()
        {
            update();

            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for HUDManager Update
                    c?.OnHUDUpdate();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnHUDUpdate;\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for StartOfRound's Start method.
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void _handle_OnLobbyStart()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for StartOfRound Start
                    c?.OnLobbyStart();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnLobbyStart;\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for StartOfRound's Awake method.
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void _handle_OnLobbyAwake()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for StartOfRound Awake
                    c?.OnLobbyAwake();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnLobbyAwake\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for PlayerControllerB's BeginGrabObject method.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPrefix]
        private static void _handle_OnGrabObject()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    Ray interactRay = new Ray(LocalPlayer.PlayerController.gameplayCamera.transform.position, LocalPlayer.PlayerController.gameplayCamera.transform.forward);

                    RaycastHit hit;

                    Physics.Raycast(interactRay, out hit, LocalPlayer.PlayerController.grabDistance, 832);

                    if (hit.collider?.transform.gameObject?.GetComponent<GrabbableObject>() == null) return;

                    c?.OnGrabObject(hit.collider.transform.gameObject.GetComponent<GrabbableObject>());
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnGrabObject\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for StartOfRound's OnShipLandedMiscEvents method.
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        [HarmonyPostfix]
        private static void _handle_OnShipLand()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for OnShipLand Awake
                    c?.OnShipLand();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnShipLand\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for PlayerControllerB's isPlayerDead 'method'.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
        [HarmonyPostfix]
        private static void _handle_OnPlayerDie()
        {
            if (!LocalPlayer.PlayerController.isPlayerDead) return; // Just in case ig
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for OnShipLand Awake
                    c?.OnPlayerDie();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnPlayerDie\n{e}\n"); }
            });
        }

        /// <summary>
        /// Handles the patch for registering all modules after initialization.
        /// </summary>
        private static void _handle_OnRegister()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Called after all modules have registered
                    c?.OnRegister();
                }
                catch (Exception e) { ConsoleManager.LogErr($"Error calling OnRegister\n{e}\n"); }
            });
        }

        #endregion =====================[ Patches ]=====================

        #region ==================[ Virtual Voids ]==================

        /// <summary>
        /// Called after all subclasses have registered.
        /// </summary>
        protected virtual void OnRegister()
        { }

        /// <summary>
        /// Called when the HUDManager starts.
        /// </summary>
        protected virtual void OnHUDStart()
        { }

        /// <summary>
        /// Called when the HUDManager awakens.
        /// </summary>
        protected virtual void OnHUDAwake()
        { }

        /// <summary>
        /// Called when the HUDManager updates.
        /// </summary>
        protected virtual void OnHUDUpdate()
        { }

        /// <summary>
        /// Called when the lobby starts.
        /// </summary>
        protected virtual void OnLobbyStart()
        { }

        /// <summary>
        /// Called when the lobby awakens.
        /// </summary>
        protected virtual void OnLobbyAwake()
        { }

        /// <summary>
        /// Called when the local player starts.
        /// </summary>
        protected virtual void OnLocalPlayerStart(PlayerControllerB _LocalPlayer)
        { }

        /// <summary>
        /// Called when an object is grabbed.
        /// </summary>
        protected virtual void OnGrabObject(GrabbableObject obj)
        { }

        /// <summary>
        /// Called when the ship lands.
        /// </summary>
        protected virtual void OnShipLand()
        { }

        /// <summary>
        /// Called when the player dies.
        /// </summary>
        protected virtual void OnPlayerDie()
        { }

        #endregion ==================[ Virtual Voids ]==================

        private static void start()
        { }

        private static void update()
        {
            if ((bool)LocalPlayer.PlayerController?.isPlayerDead) _handle_OnPlayerDie();
        }
    }
}