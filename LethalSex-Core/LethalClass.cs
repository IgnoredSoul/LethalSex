using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalSex_Core
{
    // I dont know how harmony fucking works so idk if this is required or not 💀💀💀
    [HarmonyPatch]
    public abstract class LethalClass : MonoBehaviour
    {
        protected static List<LethalClass> LethalClasses = new List<LethalClass>();
        public bool Registered => LethalClasses.Contains(this);

        #region ===================[ BaseClass Registration ]===================

        internal static void RegisterAll()
        {
            // Try getting every thing yk
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    // Create instances of the types and add them to the list
                    LethalClasses.AddRange(a.GetTypes().Where(type => type.IsSubclassOf(typeof(LethalClass))).Select(type => (LethalClass)Activator.CreateInstance(type)));

                _handle_OnRegister();

                ConsoleManager.Log($"All Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "LethalClass", Color.magenta);
            }
            catch (Exception ex) { ConsoleManager.Log($"Error getting every SubClass?;\n{ex}", "Err", Color.red); }
        }

        protected void Unregister()
        {
            LethalClasses.Remove(this);
            ConsoleManager.Log($"Registered: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "LethalClass", Color.magenta);
        }

        #endregion ===================[ BaseClass Registration ]===================

        protected virtual void Enable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been enabled and registered");
        }

        protected virtual void Disable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been disable and unregistered");
        }

        protected virtual void Destroy()
        {
            LethalClasses.Remove(this);
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been destroyed and unregistered");
        }

        #region =====================[ Patches ]=====================

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnHUDStart;\n{e}\n", "Err", Color.red); }

                try
                {
                    // Call LocalPlayer Start
                    c?.OnLocalPlayerStart(await LocalPlayer.PlayerControllerAsync());
                }
                catch (Exception e) { ConsoleManager.Log($"Error calling OnLocalPlayerStart;\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnHUDAwake;\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnHUDUpdate;\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnLobbyStart;\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnLobbyAwake\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnGrabObject\n{e}\n", "Err", Color.red); }
            });
        }

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
                catch (Exception e) { ConsoleManager.Log($"Error calling OnShipLand\n{e}\n", "Err", Color.red); }
            });
        }

        //[HarmonyPatch(typeof(PlayerControllerB), "isPlayerDead")]
        //[HarmonyPostfix]
        private static void _handle_OnPlayerDie()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for OnShipLand Awake
                    c?.OnPlayerDie();
                }
                catch (Exception e) { ConsoleManager.Log($"Error calling OnPlayerDie\n{e}\n", "Err", Color.red); }
            });
        }

        private static void _handle_OnRegister()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Called after all modules have registered
                    c?.OnRegister();
                }
                catch (Exception e) { ConsoleManager.Log($"Error calling OnRegister\n{e}\n", "Err", Color.red); }
            });
        }

        #endregion =====================[ Patches ]=====================

        #region ==================[ Virtual Voids ]==================

        protected virtual void OnRegister()
        { }

        protected virtual void OnHUDStart()
        { }

        protected virtual void OnHUDAwake()
        { }

        protected virtual void OnHUDUpdate()
        { }

        protected virtual void OnLobbyStart()
        { }

        protected virtual void OnLobbyAwake()
        { }

        protected virtual void OnLocalPlayerStart(PlayerControllerB _LocalPlayer)
        { }

        protected virtual void OnGrabObject(GrabbableObject obj)
        { }

        protected virtual void OnShipLand()
        { }

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