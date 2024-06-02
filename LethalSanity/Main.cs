using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalSanity.Modules;
using LethalSex_Core;
using LethalSex_Core.Modules;
using static LethalSex_Core.Main;

namespace LethalSanity
{
	[BepInPlugin(modGUID, modName, modVersion)]
	[BepInProcess("Lethal Company.exe")]
	[BepInDependency("com.Github.IGNOREDSOUL.LethalSex.LethalSexCore", BepInDependency.DependencyFlags.HardDependency)]
	public class Main : BaseUnityPlugin
	{
		internal readonly Harmony harmony = new Harmony(modGUID);
		internal const string modGUID = "com.github.IGNOREDSOUL.LethalSex.LethalSanity";
		internal const string modName = "LethalSanity";
		internal const string modVersion = "1.4.0";

		internal static Main instance { get; private set; }

		// internal static AssetBundle bundle { get; private set; } /* Not yet */
		internal static LethalMod mod { get; private set; }

		internal static ManualLogSource mls { get; private set; }

		private void Awake()
		{
			// Set instance to this
			instance = this;

			// Set ManualLogSource
			mls = BepInEx.Logging.Logger.CreateLogSource(modName);

			// Do config shit
			//new Config().Init();

			// Register mod
			mod = RegisterMod(modName, modVersion, "IGNOREDSOUL");

			// Register modules
			mod.RegisterModule(typeof(InsanityHandler));
			//mod.RegisterModule(typeof(CameraLeaning)); /* Not registered due to bad code */
			//mod.RegisterModule(typeof(FakeItemsV2));
			mod.RegisterModule(typeof(PostProcessing));
			//mod.RegisterModule(typeof(TestModule));
		}
	}
}