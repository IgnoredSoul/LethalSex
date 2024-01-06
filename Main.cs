using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using LethalSex.Modules;
using System.Runtime.InteropServices;

namespace LethalSex
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        #region Console Coloring

        /// <summary>
        /// Defines a constant named ENABLE_VIRTUAL_TERMINAL_PROCESSING with a hexadecimal value 0x0004.
        /// This constant represents a flag used to enable virtual terminal processing in the console, specifically for supporting ANSI escape codes.
        /// </summary>
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        /// <summary>
        /// Attribute indicating that the following method (GetStdHandle) is a declaration of a function from an external library (kernel32.dll).
        /// It specifies that the function may set the last error code if an error occurs.
        /// 
        /// Declaration of the GetStdHandle function from kernel32.dll.
        /// This function retrieves a handle to a standard device, and the nStdHandle parameter represents the standard device identifier (e.g., STD_OUTPUT_HANDLE).
        /// </summary>
        /// <param name="nStdHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// Declaration of the GetConsoleMode function.
        /// It retrieves the current mode of the console associated with the specified handle (hConsoleHandle).
        /// The current mode is returned in the lpMode parameter.
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="lpMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// Declaration of the SetConsoleMode function.
        /// It sets the new mode of the console associated with the specified handle (hConsoleHandle) to the value specified in the dwMode parameter.
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #endregion

        private const string modGUID = "LethalSex";
        private const string modName = "LethalSex";
        private const string modVersion = "0.1.1";

        internal static ManualLogSource mls { get; private set; }
        internal static Main Instance { get; private set; }
        internal static AssetBundle bundle { get; set; }

        internal readonly Harmony harmony = new Harmony(modGUID);

        void Awake()
        {
            // Set instance to this
            while(!Instance) Instance = this;

            ///<summary>
            /// This function retrieves a handle to the specified standard device (in this case, the standard output handle of the console).
            /// The value -11 is a constant representing STD_OUTPUT_HANDLE. The result is stored in the consoleHandle variable, which is of type IntPtr.
            ///</summary>
            IntPtr consoleHandle = GetStdHandle(-11);

            if (consoleHandle != IntPtr.Zero)
            {
                ///<summary>
                /// Retrieves the current mode of the console associated with the specified handle (consoleHandle).
                /// The current console mode is stored in the consoleMode variable.
                ///</summary>
                if (GetConsoleMode(consoleHandle, out uint consoleMode))
                {
                    ///<summary>
                    /// Adds the ENABLE_VIRTUAL_TERMINAL_PROCESSING flag to the consoleMode.
                    /// This flag enables processing of ANSI escape codes, allowing for more advanced formatting and styling in the console.
                    ///</summary>
                    consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

                    ///<summary>
                    /// Sets the new console mode with the modified consoleMode, including the ENABLE_VIRTUAL_TERMINAL_PROCESSING flag.
                    /// This step enables the support for ANSI escape codes in the console.
                    ///</summary>
                    SetConsoleMode(consoleHandle, consoleMode);
                }
            }

            // Set console source name
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Load bundle
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.lethalsexbundle);

            // Load config
            new Config().Init();

            // Patch
            harmony.PatchAll();
            LethalClass.Patch();

            // Print logo thing cause funny
            LogHandler.Msg(LogHandler.MessageColors.BrightMagenta, "\n /$$      /$$$$$$$$/$$$$$$$$/$$   /$$ /$$$$$$ /$$              /$$$$$$ /$$$$$$$$/$$   /$$\n| $$     | $$_____|__  $$__| $$  | $$/$$__  $| $$             /$$__  $| $$_____| $$  / $$\n| $$     | $$        | $$  | $$  | $| $$  \\ $| $$            | $$  \\__| $$     |  $$/ $$/\n| $$     | $$$$$     | $$  | $$$$$$$| $$$$$$$| $$            |  $$$$$$| $$$$$   \\  $$$$/ \n| $$     | $$__/     | $$  | $$__  $| $$__  $| $$             \\____  $| $$__/    >$$  $$ \n| $$     | $$        | $$  | $$  | $| $$  | $| $$    IGNORED  /$$  \\ $| $$      /$$/\\  $$\n| $$$$$$$| $$$$$$$$  | $$  | $$  | $| $$  | $| $$$$$$$$ SOUL |  $$$$$$| $$$$$$$| $$  \\ $$\n|________|________/  |__/  |__/  |__|__/  |__|________/       \\______/|________|__/  |__/");
        }
    }
}
