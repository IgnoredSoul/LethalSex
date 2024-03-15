using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace LethalSanity
{
    internal class Config
    {
        private static ConfigFile config { get; set; }

        internal static bool ToggleLeanModule { get; private set; }
        internal static float CL_MaxLean { get; private set; } // Maximum angle to lean
        internal static float CL_Threshold { get; private set; } // Mouse speed to lean

        internal static bool ToggleFIModule { get; private set; }
        internal static int FI_Min { get; private set; }
        internal static int FI_Max { get; private set; }
        internal static float FI_Rad { get; private set; }
        internal static bool FI_Respawning { get; private set; }
        internal static float FI_Delay { get; private set; }
        internal static float FI_RespawnDelayMin { get; private set; }
        internal static float FI_RespawnDelayMax { get; private set; }

        internal void Init()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true);

            ToggleLeanModule = config.Bind("Leaning", "Toggle Lean Module", true, "Toggle on and off the lean module. This will completely remove leaning from the mod.").Value;
            CL_MaxLean = config.Bind("Leaning", "Max Lean Angle", 35f, "Max amount of lean the camera can lean to.").Value;
            CL_Threshold = config.Bind("Leaning", "Mouse Threshold", 35f, "How fast the mouse has to move before the lean is applied").Value;

            ToggleFIModule = config.Bind("Fake Items", "Toggle Fake Items Module", true, "Toggle on and off the fake items module. This will completely remove fake items from the mod.").Value;
            FI_Min = config.Bind("Fake Items", "Min Amount", 1, "Min amount of items that will spawn.").Value;
            FI_Max = config.Bind("Fake Items", "Max Amount", 3, "Max amount of items that will spawn.").Value;
            FI_Rad = config.Bind("Fake Items", "Radius", 35, "Distance the items can spawn.").Value;
            FI_Respawning = config.Bind("Fake Items", "Respawn", true, "Can the items respawn after time").Value;
            FI_Delay = config.Bind("Fake Items", "Delay", 30, "The delay before items can spawn").Value;
            FI_RespawnDelayMin = config.Bind("Fake Items", "Min Time", 40, "Min time items will take to respawn").Value;
            FI_RespawnDelayMax = config.Bind("Fake Items", "Max Time", 80, "Max time items will take to respawn").Value;
        }
    }
}