using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LethalSanity
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        internal const string modGUID = "LethalSanity";
        internal const string modName = "com.github.IGNOREDSOUL";
        internal const string modVersion = "1.4.0";

        internal static Main instance { get; private set; }
        internal static AssetBundle bundle { get; private set; }

        private void Awake()
        {
            // Load bundle
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.lethalsanity);

            // Load config
            new Config();

            instance = this; /* Set instance to this */
        }
    }
}