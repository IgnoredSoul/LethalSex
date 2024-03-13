using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine;
using System.Threading.Tasks;

namespace LethalSanity.Modules
{
    internal class PostProcessing : LethalClass
    {
        internal static PostProcessing Module { get; private set; }

        protected override void OnRegister()
        {
            if (!Config.config.PP.Enabled) Unregister();
        }

        // When the player loads
        protected override void OnLocalPlayerStart(PlayerControllerB LocalPlayer) => Module = LocalPlayer.gameObject.GetOrAddComponent<PostProcessing>();

        private ColorAdjustments CAComp { get; set; }
        private ChromaticAberration ChrAbComp { get; set; }
        private DepthOfField DOFComp { get; set; }
        private FilmGrain FilmGrainComp { get; set; }
        private LensDistortion LensDistComp { get; set; }
        private Vignette VignetteComp { get; set; }
        private GameObject VolumeObject { get; set; }
        private VolumeProfile VolumeProfile { get; set; }

        private bool InsanityLvl1 { get; set; }
        private bool InsanityLvl2 { get; set; }
        private bool InsanityLvl3 { get; set; }

        private async void Start()
        {
            // Wait till VolumeMain exists and clone it.
            do
            {
                VolumeObject = Instantiate(
                    GameObject.Find("Systems/Rendering/VolumeMain"),
                    GameObject.Find("Systems/Rendering/VolumeMain").transform.parent
                );
                await Task.Delay(500);
            } while (!VolumeObject);

            // Rename object
            VolumeObject.name = "LS-Volume";

            // Make new VolumeProfilet
            VolumeProfile = new VolumeProfile();

            // Name new VolumeProfile
            VolumeProfile.name = "LS-VolumeProfile";

            // Assign profile
            VolumeObject.GetComponentInChildren<Volume>().profile = VolumeProfile;

            // Set max insanity
            LocalPlayer.MaxInsanity = Config.config.IO.MaxInsanity;

            // Add all effects to profile
            ChrAbComp = VolumeProfile.Add<ChromaticAberration>(); ChrAbComp.name = "LS-ChrAbComp";
            LensDistComp = VolumeProfile.Add<LensDistortion>(); LensDistComp.name = "LS-LensDistComp";
            FilmGrainComp = VolumeProfile.Add<FilmGrain>(); FilmGrainComp.name = "LS-FilmGrainComp";
            VignetteComp = VolumeProfile.Add<Vignette>(); VignetteComp.name = "LS-VignetteComp";
            VignetteComp.opacity.value = Config.config.Vignette.Opacity; VignetteComp.smoothness.value = Config.config.Vignette.Smoothness;
            DOFComp = VolumeProfile.Add<DepthOfField>(); DOFComp.name = "LS-DOFComp"; DOFComp.farFocusStart.Override(2000); DOFComp.farFocusEnd.Override(2000);
            CAComp = VolumeProfile.Add<ColorAdjustments>(); CAComp.name = "LS-CAComp";
        }

        private void LateUpdate()
        {
            // Just to make sure no errors arise
            if (!LocalPlayer.PlayerController) return;

            #region Level 1

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL1)
            //		Insanity Level 1 has been checked
            if (LocalPlayer.Insanity < Config.config.IO.Lvl1 && InsanityLvl1)
            {
                // Set this level of insanity to false
                InsanityLvl1 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, (float)Config.config.FilmGrain.ResetVal, (float)Config.config.FilmGrain.ResetSpeed);
                Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, (float)Config.config.Vignette.ResetVal, (float)Config.config.Vignette.ResetSpeed);
            }

            // When:
            //		Insanity is greater than 35
            //		Insanity Level 1 has not been checked
            if (LocalPlayer.Insanity >= Config.config.IO.Lvl1 && !InsanityLvl1)
            {
                // Set this level of insanity to true
                InsanityLvl1 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, (float)Config.config.FilmGrain.ApplyVal, (float)Config.config.FilmGrain.ApplySpeed);
                Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, (float)Config.config.Vignette.ApplyVal, (float)Config.config.Vignette.ApplySpeed);

                // Shake the camera slightly
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }

            #endregion Level 1

            #region Level 2

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has been checked
            if (LocalPlayer.Insanity < Config.config.IO.Lvl2 && InsanityLvl2)
            {
                // Set this level of insanity to false
                InsanityLvl2 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, (float)Config.config.ChrAb.ResetVal, (float)Config.config.ChrAb.ResetSpeed);
                Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, (float)Config.config.LensDist.ResetVal, (float)Config.config.LensDist.ResetSpeed);
            }

            // When:
            //		Insanity is greater than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has not been checked
            if (LocalPlayer.Insanity >= Config.config.IO.Lvl2 && !InsanityLvl2)
            {
                // Set this level of insanity to true
                InsanityLvl2 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, (float)Config.config.ChrAb.ApplyVal, (float)Config.config.ChrAb.ApplySpeed);
                Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, (float)Config.config.LensDist.ApplyVal, (float)Config.config.LensDist.ApplySpeed);
            }

            #endregion Level 2

            #region Level 3

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has been checked
            if (LocalPlayer.Insanity < Config.config.IO.Lvl3 && InsanityLvl3)
            {
                // Set this level of insanity to false
                InsanityLvl3 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, (float)Config.config.DOP.SResetVal, (float)Config.config.DOP.SResetSpeed);
                Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, (float)Config.config.DOP.EResetVal, (float)Config.config.DOP.EResetSpeed);
                Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, (float)Config.config.CA.ResetVal, (float)Config.config.CA.ResetSpeed);
            }

            // When:
            //		Insanity is greater than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has not been checked
            if (LocalPlayer.Insanity >= Config.config.IO.Lvl3 && !InsanityLvl3)
            {
                // Set this level of insanity to true
                InsanityLvl3 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, (float)Config.config.DOP.SApplyVal, (float)Config.config.DOP.SApplySpeed);
                Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, (float)Config.config.DOP.EApplyVal, (float)Config.config.DOP.EApplySpeed);
                Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, (float)Config.config.CA.ApplyVal, (float)Config.config.CA.ApplySpeed);
            }

            #endregion Level 3
        }

        private void OnDestroy() => base.Destroy();

        private void OnDisable() => base.Disable();

        private void OnEnable() => base.Enable();
    }
}