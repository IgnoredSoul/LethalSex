using System;
using BepInEx;
using System.IO;
using BepInEx.Configuration;
using LethalSex_Core.Modules;

namespace LethalSanity
{
	internal class Config
	{
		internal int PP { get; set; }
		internal Tuple<bool, float, float> VC { get; set; }
		internal Tuple<bool, float, float> FGC { get; set; }
		internal Tuple<bool, float, float> CAC { get; set; }
		internal Tuple<bool, float, float> LDC { get; set; }
		internal Tuple<bool, float, float> DOFC { get; set; }
		internal Tuple<bool, float, float> SC { get; set; }

		internal Config()
		{
			// =====================================================[ Create / Read ]===================================================== \\
			ConfigFile _config = new(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true, new(Main.modGUID, Main.modName, Main.modVersion));

			// ====================================================[ Post Processing ]==================================================== \\
			PP = _config.Bind("Post Processing", "Priority", 1, "In Unity, the post-processing priority value determines which volume's effects are applied first when multiple volumes overlap. Higher priority values take precedence, allowing for specific area effects to override global ones.\nSet the value higher if effects are being wacky.").Value;
			VC = ConvertInput(_config.Bind("Post Processing", "Vignette activation", "true, 25, 3", "Toggle, insanity level, offset"));
			FGC = ConvertInput(_config.Bind("Post Processing", "Film Grain activation", "true, 30, 5", "Toggle, insanity level, offset"));
			CAC = ConvertInput(_config.Bind("Post Processing", "Chromatic Aberation activation", "true, 40, 5", "Toggle, insanity level, offset"));
			LDC = ConvertInput(_config.Bind("Post Processing", "Lens Distortion activation", "true, 35, 4", "Toggle, insanity level, offset"));
			DOFC = ConvertInput(_config.Bind("Post Processing", "Depth of Field activation", "true, 50, 5", "Toggle, insanity level, offset"));
			SC = ConvertInput(_config.Bind("Post Processing", "Saturation activation", "true, 50, 3", "Toggle, insanity level, offset"));
		}

		/// <summary>
		/// First we try to convert the user's values.<br/>
		/// If we cannot convert the user's values, we use default values.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static Tuple<bool, float, float> ConvertInput(ConfigEntry<string> input)
		{
			try
			{
				// First, if the input is valid
				string[]? rtrn = input.Value?.Replace(" ", "").Split(',');
				if (rtrn?.Length != 3) throw new ArgumentException("Invalid amount. Defaulting.");

				// Second, run TryParses
				if (!bool.TryParse(rtrn[0], out _)) throw new ArgumentException($"({input.Definition.Key}) Item 1: {rtrn[0]}, does not parse into a bool.");
				if (!float.TryParse(rtrn[1], out _)) throw new ArgumentException($"({input.Definition.Key}) Item 2: {rtrn[1]}, does not parse into a float.");
				if (!float.TryParse(rtrn[2], out _)) throw new ArgumentException($"({input.Definition.Key}) Item 3: {rtrn[2]}, does not parse into a float.");

				// Return tuple with converted values
				return new(bool.Parse(rtrn[0]), float.Parse(rtrn[1]), float.Parse(rtrn[2]));
			}
			catch (ArgumentException e) { Main.mls.LogFatal(e); ConsoleManager.Error($"Error in config: {e}"); };

			string[] rtrn2 = input.DefaultValue.ToString().Replace(" ", "").Split(',');
			return new(bool.Parse(rtrn2[0]), float.Parse(rtrn2[1]), float.Parse(rtrn2[2]));
		}
	}
}