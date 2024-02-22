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
        private static List<LethalClass> LethalClasses = new List<LethalClass>();

        internal static void Patch()
        {
            // Try getting every thing yk
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    // Create instances of the types and add them to the list
                    LethalClasses.AddRange(a.GetTypes().Where(type => type.IsSubclassOf(typeof(LethalClass))).Select(type => (LethalClass)Activator.CreateInstance(type)));

                ConsoleManager.Log($"Lethal Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]", "Init", Color.magenta);

                Main.mls.LogMessage($"Lethal Classes: [{string.Join(", ", LethalClasses.ToList().Select(obj => obj.GetType().Name))}]");
            }
            catch (Exception ex) { ConsoleManager.Log($"Error getting every SubClass?;\n{ex}", "Err", Color.red); }
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
        private static void _handle_HUD_Start()
        {
            LethalClasses.ForEach(async c =>
            {
                try
                {
                    // Call for HUDManager Start
                    c?.HUD_Start();
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling HUD_Start;\n{e}\n"); }

                try
                {
                    // Call LocalPlayer Start
                    c?.LocalPlayer_Start(await LocalPlayer.PlayerControllerAsync());
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling LocalPlayer_Start;\n{e}\n"); }
            });
        }

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        private static void _handle_HUD_Awake()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call HUDManager Awake
                    c?.HUD_Awake();
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling HUD_Awake;\n{e}\n"); }
            });
        }

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void _handle_HUD_Update()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for HUDManager Update
                    c?.HUD_Update();
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling HUD_Update;\n{e}\n"); }
            });
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void _handle_Lobby_Start()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for StartOfRound Start
                    c?.Lobby_Start();
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling Lobby_Start;\n{e}\n"); }
            });
        }

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void _handle_Lobby_Awake()
        {
            LethalClasses.ForEach(c =>
            {
                try
                {
                    // Call for StartOfRound Awake
                    c?.Lobby_Awake();
                }
                catch (Exception e) { Main.mls.LogFatal($"Error calling Lobby_Awake\n{e}\n"); }
            });
        }

        //[HarmonyPatch(typeof(DevMenuManager), "Start")]
        //[HarmonyPostfix]
        //private static void _handle_DevMenu_Start()
        //{
        //    LethalClasses.ForEach(c =>
        //    {
        //        try
        //        {
        //            // Call for StartOfRound Awake
        //            c?.DevMenu_Start();
        //        }
        //        catch (Exception e) { Main.mls.LogFatal($"Error calling DevMenu_Start\n{e}\n"); }
        //    });
        //}

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
        public virtual void HUD_Start()
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
        public virtual void HUD_Awake()
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
        public virtual void HUD_Update()
        { }

        /// <summary>
        /// Method called when the lobby (<seealso cref="StartOfRound"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="StartOfRound"/>) start event.
        /// <br/><br/>
        /// See: <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/> for the official Unity API documentation
        /// </summary>
        public virtual void Lobby_Start()
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
        public virtual void Lobby_Awake()
        { }

        /// <summary>
        /// Method called when the local player (<seealso cref="PlayerControllerB"/>) system starts.
        /// <br/>
        /// This method can be overridden in derived classes to perform specific actions
        /// to the (<seealso cref="PlayerControllerB"/>) start event.
        /// <br/><br/>
        /// See: <seealso cref="PlayerControllerB"/> for more info
        /// </summary>
        public virtual void LocalPlayer_Start(PlayerControllerB LocalPlayer)
        { }

        public virtual void DevMenu_Start()
        { }

        #endregion ==================[ Virtual Voids ]==================
    }
}