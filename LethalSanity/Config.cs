using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace LethalSanity
{
    internal class Config
    {
        private static ConfigFile config { get; set; }

        internal static bool ToggleLeanModule { get; private set; }
        internal static float maxLeanAngle { get; private set; } // Maximum angle to lean
        internal static float threshold { get; private set; } // Mouse speed to lean

        internal void Init()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true);

            ToggleLeanModule = config.Bind("Leaning", "Toggle Lean Module", true, "Toggle on and off the lean module. This will completely remove leaning from the mod.").Value;
            maxLeanAngle = config.Bind("Leaning", "Max Lean Angle", (float)35f, "Max amount of lean the camera can lean to.").Value;
            threshold = config.Bind("Leaning", "Mouse Threshold", (float)35f, "How fast the mouse has to move before the lean is applied").Value;
        }
    }
}