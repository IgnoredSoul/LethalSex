using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace LethalSanity
{
    internal class Config
    {
        private static ConfigFile config { get; set; }

        #region Post processing

        internal static bool PP_TogglePost { get; private set; }
        internal static int PP_PostPriority { get; private set; }

        #endregion Post processing

        #region Camera Leaning

        internal static bool CL_ToggleLeanModule { get; private set; }
        internal static float CL_maxLeanAngle { get; private set; }
        internal static float CL_threshold { get; private set; }

        #endregion Camera Leaning

        #region Fake Items

        internal static bool FI_ToggleFakeItem { get; private set; }
        internal static int FI_MinItems { get; private set; }
        internal static int FI_MaxItems { get; private set; }
        internal static float FI_SpawnDelay { get; private set; }

        #endregion Fake Items

        #region Camera Shake

        internal static bool CS_ToggleCamShake { get; private set; }

        #endregion Camera Shake

        #region Stalker Hallucination

        internal static bool SH_ToggleStalker { get; private set; }
        internal static float SH_StalkDelay { get; private set; }

        #endregion Stalker Hallucination

        internal void Init()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "LethalSanity.cfg"), true);

            PP_TogglePost = config.Bind("Post Processing", "Toggle Post Processing Module", true, "Toggle on and off the post processing module. This will completely remove insanity visual effects from the mod.").Value;
            PP_PostPriority = config.Bind("Post Processing", "Change Post Priority", 0, "Changing the priority means to override every other post processing effect. This includes other mods and base game. Increasing means higher priority, decreasing means less priority.").Value;

            CL_ToggleLeanModule = config.Bind("Leaning", "Toggle Lean Module", true, "Toggle on and off the lean module. This will completely remove leaning from the mod.").Value;
            CL_maxLeanAngle = config.Bind("Leaning", "Max Lean Angle", 35f, "Max amount of lean the camera can lean to.").Value;
            CL_threshold = config.Bind("Leaning", "Mouse Threshold", 35f, "How fast the mouse has to move before the lean is applied").Value;

            FI_ToggleFakeItem = config.Bind("Fake Items", "Toggle Fake Items Module", true, "Toggle on and off the fake items module. This will completely remove fake item hallucinations from the mod.").Value;
            FI_MinItems = config.Bind("Fake Items", "Min Items", 1, "Minimum amount of fake items that will spawn").Value;
            FI_MaxItems = config.Bind("Fake Items", "Max Items", 3, "Maximum amount of fake items that will spawn").Value;
            FI_SpawnDelay = config.Bind("Fake Items", "Spawn Delay", 60, "Respawn fakes after x ± rnd(5)").Value;

            CS_ToggleCamShake = config.Bind("Camera Shake", "Toggle Camera Shake Module", true, "Toggle on and off the camera shake module. This will completely remove any added camera shaking from the mod.").Value;

            //SH_ToggleStalker = config.Bind("Stalker", "Toggle Stalker Module", true, "Toggle on and off the stalker module. This will completely remove the insanity creature *'(stalker)'* from the mod.").Value;
            SH_StalkDelay = config.Bind("Stalker", "Change Stalker Delay", 60, "Changing this will set how long the stalker will hide for. x ± rnd(5)").Value;
        }
    }
}