using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using System.Reflection;
using System.Collections.Generic;

namespace LethalSex_Core
{
	[BepInPlugin(modGUID, modName, modVersion)]
	[BepInProcess("Lethal Company.exe")]
	public class Main : BaseUnityPlugin
	{
		internal readonly Harmony harmony = new(modGUID);
		internal const string modVersion = "2.0.0";
		private const string modGUID = "com.Github.IGNOREDSOUL.LethalSex.LethalSexCore";
		private const string modName = "LethalSex-Core";

		private const string waterMark = @"
 ▄▀▀▀▀▄     ▄▀▀█▄▄▄▄  ▄▀▀▀█▀▀▄  ▄▀▀▄ ▄▄   ▄▀▀█▄   ▄▀▀▀▀▄         ▄▀▀▀▀▄  ▄▀▀█▄▄▄▄  ▄▀▀▄  ▄▀▄
█    █     ▐  ▄▀   ▐ █    █  ▐ █  █   ▄▀ ▐ ▄▀ ▀▄ █    █         █ █   ▐ ▐  ▄▀   ▐ █    █   █
▐    █       █▄▄▄▄▄  ▐   █     ▐  █▄▄▄█    █▄▄▄█ ▐    █            ▀▄     █▄▄▄▄▄  ▐     ▀▄▀
    █        █    ▌     █         █   █   ▄▀   █     █          ▀▄   █    █    ▌       ▄▀ █
  ▄▀▄▄▄▄▄▄▀ ▄▀▄▄▄▄    ▄▀         ▄▀  ▄▀  █   ▄▀    ▄▀▄▄▄▄▄▄▀     █▀▀▀    ▄▀▄▄▄▄       █  ▄▀
  █         █    ▐   █          █   █    ▐   ▐     █             ▐       █    ▐     ▄▀  ▄▀
  ▐         ▐        ▐ IGNORED  ▐   ▐   SOUL       ▐                     ▐         █    ▐    ";

		public static Main instance { get; private set; }
		public static ManualLogSource mls { get; private set; }
		internal static AssetBundle bundle { get; private set; }
		internal static LethalMod CoreMod { get; private set; }

		protected internal static List<LethalMod> LethalMods = new();

		private void Awake()
		{
			// Set instance
			while (!instance) instance = this;

			// Set ManualLogSource
			mls = BepInEx.Logging.Logger.CreateLogSource(modName);

			// Print watermark
			mls.LogError(waterMark);

			// Load bundle
			bundle = AssetBundle.LoadFromMemory(Properties.resources.lethalsex_core);

			// Do config
			new Config().Init();

			// Patch
			harmony.PatchAll(typeof(Patching));

			// Register new mod
			CoreMod = RegisterMod(modName, modVersion, "IGNOREDSOUL");

			CoreMod.RegisterModule(typeof(Modules.ConsoleManager));
			//CoreMod.RegisterModule(typeof(Modules.DevHelper));
		}

		public static LethalMod RegisterMod(string modName, string modVersion, string modAuthor)
		{
			LethalMod newMod = new LethalMod(Assembly.GetCallingAssembly().GetName().Name, modName, modVersion, modAuthor);

			mls.LogMessage($"[{modAuthor}] created a new mod of: [{modName} v{modVersion}]"); // Log to BepInEx console
			Modules.ConsoleManager.Log($"[{modAuthor}] created a new mod of: [{modName} v{modVersion}]"); // Log to my console

			LethalMods.Add(newMod);

			return newMod;
		}
	}
}