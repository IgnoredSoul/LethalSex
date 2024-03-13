using BepInEx;
using HarmonyLib;
using LethalHUD.Modules;
using UnityEngine;

namespace LethalHUD
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        private const string modGUID = "LethalHUD";
        private const string modName = "LethalHUD";
        private const string modVersion = "0.0.1";

        internal static Main instance { get; private set; }

        internal static AssetBundle bundle { get; private set; }

        private void Awake()
        {
            // Set instance to this
            instance = this;

            // Load bundle
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.lethalhud);

            // Load config
            new Config().Init();
        }
    }
}