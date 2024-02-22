using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LethalSex_Core
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        private const string modGUID = "LethalSex-Core";
        private const string modName = "LethalSex-Core";
        private const string modVersion = "0.0.1";

        private const string waterMark = @"
 ▄▀▀▀▀▄     ▄▀▀█▄▄▄▄  ▄▀▀▀█▀▀▄  ▄▀▀▄ ▄▄   ▄▀▀█▄   ▄▀▀▀▀▄         ▄▀▀▀▀▄  ▄▀▀█▄▄▄▄  ▄▀▀▄  ▄▀▄
█    █     ▐  ▄▀   ▐ █    █  ▐ █  █   ▄▀ ▐ ▄▀ ▀▄ █    █         █ █   ▐ ▐  ▄▀   ▐ █    █   █
▐    █       █▄▄▄▄▄  ▐   █     ▐  █▄▄▄█    █▄▄▄█ ▐    █            ▀▄     █▄▄▄▄▄  ▐     ▀▄▀
    █        █    ▌     █         █   █   ▄▀   █     █          ▀▄   █    █    ▌       ▄▀ █
  ▄▀▄▄▄▄▄▄▀ ▄▀▄▄▄▄    ▄▀         ▄▀  ▄▀  █   ▄▀    ▄▀▄▄▄▄▄▄▀     █▀▀▀    ▄▀▄▄▄▄       █  ▄▀
  █         █    ▐   █          █   █    ▐   ▐     █             ▐       █    ▐     ▄▀  ▄▀
  ▐         ▐        ▐ IGNORED  ▐   ▐   SOUL       ▐                     ▐         █    ▐    ";

        public static Main Instance { get; private set; }
        public static AssetBundle bundle { get; set; }
        public static ManualLogSource mls { get; set; }

        private void Awake()
        {
            // Set instance to this
            while (!Instance) Instance = this;

            // Set console source name
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Load bundle
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.lethalsex_core);

            // Load config
            new Config().Init();

            // Print watermark
            mls.LogError(waterMark);

            // Patch
            LethalClass.Patch();
            harmony.PatchAll(typeof(LethalClass));
        }
    }
}