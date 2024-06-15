using BepInEx;
using System.IO;
using BepInEx.Configuration;

namespace LethalSex_Core
{
	internal class Config
	{
		private static ConfigFile config { get; set; }

		internal static bool ToggleDebugConsole { get; private set; }

		internal void Init()
		{
			config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSexCore.cfg"), true);

			ToggleDebugConsole = config.Bind("Console", "Toggle debug console", false, "Allow the console to be toggled on and off? (F10)").Value;
		}
	}
}