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

        ~LethalClass() => Unregister();

        #region ===================[ BaseClass Registration ]===================

        protected virtual void OnRegister()
        { Main.mls.LogFatal("Registered: " + this.GetType().Name); }

        protected virtual void Register()
        {
            OnRegister();
            if (!LethalClasses.Contains(this)) LethalClasses.Add(this);
        }

        /// <summary>
        /// When the module is unregistered, this virtual is called
        /// </summary>
        protected virtual void OnUnregister()
        { Main.mls.LogFatal($"Unregistered: {GetType().Name}"); }

        /// <summary>
        /// When the module wants to unregister, this virtual is called
        /// </summary>
        protected virtual void Unregister()
        {
            OnUnregister();
            if (LethalClasses.Contains(this)) LethalClasses.Remove(this);
        }

        /// <summary>
        /// When the module is wants to unregister from an outside source, this method is called
        /// </summary>
        public void ManualUnregister() => Unregister();

        #endregion ===================[ BaseClass Registration ]===================

        // I made these cause I was too lazy to write 'ConsoleManager.Log($"Behaviour: "NAME" has been enabled");' everytime
        // ..... so yeah... lmfaoooo

        /// <summary>
        /// When the component / gameObject is enabled
        /// </summary>
        protected virtual void Enable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been enabled and registered");
        }

        /// <summary>
        /// When the component / gameObject is disabled
        /// </summary>
        protected virtual void Disable()
        {
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been disable and unregistered");
        }

        /// <summary>
        /// When the component / gameObject is destroyed
        /// </summary>
        protected virtual void Destroy()
        {
            LethalClasses.Remove(this);
            ConsoleManager.Log($"Behaviour: {GetType().Name} has been destroyed and unregistered");
        }

        protected virtual void Awake()
        {
            Register();
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
        protected virtual void OnHUDStart()
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
        protected virtual void OnHUDAwake()
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
        protected virtual void OnHUDUpdate()
        { }

        /// <summary>
        /// Method called when the lobby (<seealso cref="StartOfRound"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) start event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/> for the official Unity API documentation
        /// </summary>
        protected virtual void OnLobbyStart()
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
        protected virtual void OnLobbyAwake()
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) start event.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        protected virtual void OnLocalPlayerStart(PlayerControllerB LocalPlayer)
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) starts to grab an object.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) BeginGrabObject method.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        protected virtual void OnGrabObject(GrabbableObject obj)
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="StartOfRound"/>) 'OnShipLandedMiscEvents' is called.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) 'OnShipLandedMiscEvents' method.
        /// <br/><br/>
        /// See: <seealso cref="StartOfRound"/> for more info
        /// </summary>
        protected virtual void OnShipLand()
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) fucking dies.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) 'isPlayerDead' field.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        protected virtual void OnPlayerDie()
        { }

        #endregion ==================[ Virtual Voids ]==================

        private static void start()
        {
            ConsoleManager.Log($"Registerd Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]");
            Main.mls.LogFatal($"Registerd Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]");
        }

        private static void update()
        {
            if ((bool)LocalPlayer.PlayerController?.isPlayerDead) _handle_OnPlayerDie();
        }
    }
}