using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static System.String;

namespace LethalSanity
{
    internal class Config
    {
        private ConfigFile config { get; set; }
        private ConfigFile dconfig { get; set; }

        public Config()
        {
            Init();
            Init_Danger();
        }

        #region Insanity Options

        internal static float IO_MaxInsanity { get; private set; }

        #endregion Insanity Options

        #region Post processing

        internal static bool PP_ToggleModule { get; private set; }
        internal static int PP_PostPriority { get; private set; }
        internal static float PP_Level1 { get; private set; }
        internal static float PP_Level2 { get; private set; }
        internal static float PP_Level3 { get; private set; }

        #endregion Post processing

        #region Camera Leaning

        internal static bool CL_ToggleModule { get; private set; }
        internal static float CL_maxLeanAngle { get; private set; }
        internal static float CL_threshold { get; private set; }
        internal static float CL_ResetSpeed { get; private set; }

        #endregion Camera Leaning

        #region Fake Items

        internal static bool FI_ToggleModule { get; private set; }
        internal static int FI_MinItems { get; private set; }
        internal static int FI_MaxItems { get; private set; }
        internal static bool FI_DoRespawn { get; private set; }
        internal static float FI_SpawnDelay { get; private set; }
        internal static string[] FI_Blacklist { get; private set; }
        internal static float FI_SpawnRadius { get; private set; }
        internal static bool FI_LogScrap { get; private set; }
        internal static int FI_LvlStartStop { get; private set; }

        #endregion Fake Items

        #region Camera Shake

        internal static bool CS_ToggleModule { get; private set; }
        internal float[] CS_Amount { get; private set; }

        #endregion Camera Shake

        #region Stalker Hallucination

        internal static bool SH_ToggleModule { get; private set; }
        internal static float SH_StalkDelay { get; private set; }

        #endregion Stalker Hallucination

        private void Init()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true);

            // ======================[ Post Processing ]====================== \\
            PP_ToggleModule = config.Bind("Post Processing", "Toggle Post Processing Module", true, "Toggle on and off the post processing module. This will completely remove insanity visual effects from the mod.").Value;

            // =======================[ Camera Leaning ]======================= \\
            CL_ToggleModule = config.Bind("Leaning", "Toggle Lean Module", true, "Toggle on and off the lean module. This will completely remove leaning from the mod.").Value;
            CL_maxLeanAngle = config.Bind("Leaning", "Max Lean Angle", 35f, "Max amount of lean the camera can lean to.").Value;
            CL_threshold = config.Bind("Leaning", "Mouse Threshold", 35f, "How fast the mouse has to move before the lean is applied").Value;
            CL_ResetSpeed = config.Bind("Leaning", "Lerp Time", 5f, "How fast the lerp should take.").Value;

            // =========================[ Fake Items ]========================= \\
            FI_ToggleModule = config.Bind("Fake Items", "Toggle Fake Items Module", true, "Toggle on and off the fake items module. This will completely remove fake item hallucinations from the mod.").Value;
            FI_MinItems = config.Bind("Fake Items", "Min Items", 1, "Minimum amount of fake items that will spawn").Value;
            FI_MaxItems = config.Bind("Fake Items", "Max Items", 3, "Maximum amount of fake items that will spawn").Value;

            // ========================[ Camera Shake ]======================== \\
            CS_ToggleModule = config.Bind("Camera Shake", "Toggle Camera Shake Module", true, "Toggle on and off the camera shake module. This will completely remove any added camera shaking from the mod.").Value;

            // ===================[ Stalker Hallucination ]=================== \\
            //SH_ToggleStalker = config.Bind("Stalker", "Toggle Stalker Module", true, "Toggle on and off the stalker module. This will completely remove the insanity creature *'(stalker)'* from the mod.").Value;
        }

        private void Init_Danger()
        {
            dconfig = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, "LethalSanity_Danger.cfg"), true);

            // ====================== [Post Processing] ====================== \\
            PP_PostPriority = dconfig.Bind("Post Processing", "Change Post Priority", 0, "Changing the priority means to override every other post processing effect. This includes other mods and base game. Increasing means higher priority, decreasing means less priority.").Value;

            PP_Level1 = dconfig.Bind("Post Processing", "Change Level 1", 35f, "Changing this value will change when the insanity level 1 will trigger. When the Insanity level has reaches this number, level 1 will start.").Value;
            PP_Level2 = dconfig.Bind("Post Processing", "Change Level 2", 45f, "Changing this value will change when the insanity level 2 will trigger. When the Insanity level has reaches this number, level 2 will start.").Value;
            PP_Level3 = dconfig.Bind("Post Processing", "Change Level 3", 65f, "Changing this value will change when the insanity level 3 will trigger. When the Insanity level has reaches this number, level 3 will start.").Value;

            /*            string[] levels = dconfig.Bind("Post Processing", "Levels at which the insanity will take effect on.", "35f,45f,65f", "',' to separate each item. (no spaces)").Value.Split(',');
                        string JLevels = JsonConvert.SerializeObject(new Dictionary<string, string>()
                        {
                                { "Level1", levels[0] },
                                { "Level2", levels[1] },
                                { "Level3", levels[2] },
                        });*/

            // ======================[ Insanity Options ]====================== \\
            IO_MaxInsanity = dconfig.Bind("Insanity Options", "Change Max Insanity", 65f, "This will set the max insanity").Value;

            // =========================[ Fake Items ]========================= \\
            FI_DoRespawn = dconfig.Bind("Fake Items", "Toggle respawning after delay", true, "Should the items respawn after a certain time?").Value;
            FI_SpawnDelay = dconfig.Bind("Fake Items", "Spawn Delay", 60, "Respawn fakes after x ± rnd(5.0f)").Value;
            FI_Blacklist = dconfig.Bind("Fake Items", "Blacklist", "", "When blaclisting items use ',' to separate each item. (Non-case sensitive and no space)").Value.ToLower().Split(',');
            FI_SpawnRadius = dconfig.Bind("Fake Items", "Spawn Radius", 25, "How far can the items spawn.").Value;
            FI_LogScrap = dconfig.Bind("Fake Items", "Log Scrap", false, "When blacklisting scrap, print the names into the dev console.").Value;
            FI_LvlStartStop = dconfig.Bind("Fake Items", "Fake Scrap Spawn Level", 1, "The desired insanity level the scrap will spawn at. (0, 1, 2, 3)").Value;

            // ===================[ Stalker Hallucination ]=================== \\
            //SH_StalkDelay = config.Bind("Stalker", "Change Stalker Delay", 60, "Changing this will set how long the stalker will hide for. x ± rnd(5.0f)").Value;
        }
    }
}