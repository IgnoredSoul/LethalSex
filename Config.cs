using BepInEx;
using System.IO;
using BepInEx.Configuration;
using UnityEngine.InputSystem;

namespace LethalSex
{
    internal class Config
    {
        private static ConfigFile config { get; set; }
        internal static Key MuteKey { get; private set; }
        internal static bool ToggleMute { get; private set; }
        public void Init()
        {
            var filePath = Path.Combine(Paths.ConfigPath, "LethalSex.cfg");
            config = new ConfigFile(filePath, true);

            ToggleMute = config.Bind<bool>("Toggle Mute", "ToggleMute", true, "Toggle the voice hud / toggle mute").Value;
            MuteKey = config.Bind<Key>("Advanced HUD", "ToggleMuteKey", Key.M, "Toggle mute key... Any key on your keyboard.").Value;
        }
    }
}
