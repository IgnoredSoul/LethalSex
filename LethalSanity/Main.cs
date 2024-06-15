using BepInEx;
using HarmonyLib;
using LethalSex_Core;
using BepInEx.Logging;
using LethalSanity.Modules;
using static LethalSex_Core.Main;

namespace LethalSanity
{
	[BepInPlugin(modGUID, modName, modVersion)]
	[BepInProcess("Lethal Company.exe")]
	[BepInDependency("com.GitHub.IGNOREDSOUL.LethalSex.LethalSexCore", BepInDependency.DependencyFlags.HardDependency)]
	public class Main : BaseUnityPlugin
	{
		internal readonly Harmony harmony = new Harmony(modGUID);
		internal const string modGUID = "com.GitHub.IGNOREDSOUL.LethalSex.LethalSanity";
		internal const string modName = "LethalSanity";
		internal const string modVersion = "2.0.0";

		public static Main instance { get; private set; }
		internal static LethalMod mod { get; private set; }
		internal static ManualLogSource mls { get; private set; }
		internal static Config config { get; private set; }

		private void Awake()
		{
			// Set instance to this
			instance = this;

			// Set ManualLogSource
			mls = BepInEx.Logging.Logger.CreateLogSource(modName);

			// Do config shit
			config = new Config();

			// Register mod
			mod = RegisterMod(modName, modVersion, "IGNOREDSOUL");

			// Register modules
			mod.RegisterModule(typeof(PostProcessing));

			//mod.RegisterModule(typeof(CameraLeaning));	/* Not registered due to bad code */
			//mod.RegisterModule(typeof(FakeItemsV2));		/* Not registered due to bad code */
			//mod.RegisterModule(typeof(TestModule));		/* Not registered... wonder why? */
		}
	}
}