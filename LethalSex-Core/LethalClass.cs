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

        internal static void Patch()
        {
            // Try getting every thing yk
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    // Create instances of the types and add them to the list
                    LethalClasses.AddRange(a.GetTypes().Where(type => type.IsSubclassOf(typeof(LethalClass))).Select(type => (LethalClass)Activator.CreateInstance(type)));

                ConsoleManager.Log($"Lethal Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "Init", Color.magenta);
            }
            catch (Exception ex) { ConsoleManager.Log($"Error getting every SubClass?;\n{ex}", "Err", Color.red); }
        }

        private void Update()
        {
            if (LocalPlayer.PlayerController && LocalPlayer.PlayerController.isPlayerDead) _handle_OnPlayerDie();
        }

        /// <summary>
        /// I made these cause I was too lazy to write 'ConsoleManager.Log($"Behaviour: "NAME" has been enabled");' everytime
        /// ..... so yeah... lmfaoooo
        /// </summary>
        public virtual void Enabled() => ConsoleManager.Log($"Behaviour: {GetType().Name} has been enabled");

        public virtual void Disabled() => ConsoleManager.Log($"Behaviour: {GetType().Name} has been disable");

        public virtual void Destroyed() => ConsoleManager.Log($"Behaviour: {GetType().Name} has been destroyed");

        #region =====================[ Patches ]=====================

        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        private static void _handle_OnHUDStart()
        {
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
        private static void _handle_ONHUDAwake()
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

        #endregion =====================[ Patches ]=====================

        #region ==================[ Virtual Voids ]==================

        /// <summary>
        /// Method called when the Heads-Up Display (<seealso cref="HUDManager"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="HUDManager"/>) start event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void OnHUDStart()
        { }

        /// <summary>
        /// Method called when the Heads-Up Display (<seealso cref="HUDManager"/>) object is initialized,
        /// before it is shown on the screen.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="HUDManager"/>) awake event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void OnHUDAwake()
        { }

        /// <summary>
        /// Method called when the Heads-Up Display (<seealso cref="HUDManager"/>) object is initialized,
        /// before it is shown on the screen.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="HUDManager"/>) awake event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void OnHUDUpdate()
        { }

        /// <summary>
        /// Method called when the lobby (<seealso cref="StartOfRound"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) start event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void OnLobbyStart()
        { }

        /// <summary>
        /// Method called when the Heads-Up Display (<seealso cref="StartOfRound"/>) object is initialized,
        /// before it is shown on the screen.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) awake event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void OnLobbyAwake()
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) start event.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        public virtual void OnLocalPlayerStart(PlayerControllerB LocalPlayer)
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) starts to grab an object.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) BeginGrabObject method.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        public virtual void OnGrabObject(GrabbableObject obj)
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="StartOfRound"/>) 'OnShipLandedMiscEvents' is called.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) 'OnShipLandedMiscEvents' method.
        /// <br/><br/>
        /// See: <seealso cref="StartOfRound"/> for more info
        /// </summary>
        public virtual void OnShipLand()
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) fucking dies.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) 'isPlayerDead' field.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        public virtual void OnPlayerDie()
        { }

        #endregion ==================[ Virtual Voids ]==================
    }
}