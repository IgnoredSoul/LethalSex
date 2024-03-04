using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace LethalSex_Core
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        private const string modGUID = "LethalSex-Core";
        private const string modName = "LethalSex-Core";
        internal const string modVersion = "1.3.0";

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

            // Patch
            LethalClass.RegisterAll();
            harmony.PatchAll(typeof(LethalClass));

            // Print watermark
            mls.LogError(waterMark);
        }
    }
}