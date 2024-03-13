using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LethalSanity
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    [BepInDependency("com.github.LethalSex.LethalSexCore.IGNOREDSOUL", "1.3.0")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        internal const string modGUID = "com.github.LethalSex.LethalSanity.IGNOREDSOUL";
        internal const string modName = "LethalSanity";
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