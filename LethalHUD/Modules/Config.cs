using BepInEx.Configuration;
using BepInEx;
using System.IO;

namespace LethalHUD.Modules
{
    internal class Config
    {
        private static ConfigFile config { get; set; }

        internal static bool ToggleFPS { get; private set; }
        internal static bool ToggleHP { get; private set; }
        internal static bool ToggleSanity { get; private set; }
        internal static bool ToggleVoice { get; private set; }

        internal void Init()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalHUD.cfg"), true);

            ToggleHP = config.Bind("Health", "Display HP Image", false, "Enabling this will display your HP as a heart inside your character").Value;

            ToggleSanity = config.Bind("Sanity", "Display Sanity Image", false, "Enabling this will display your sanity as a brain inside your character").Value;

            ToggleVoice = config.Bind("Voice", "Enable toggle mute", true, "Enabling this will make your push to talk key a push to mute key if 'voice activated' is on").Value;
        }
    }
}