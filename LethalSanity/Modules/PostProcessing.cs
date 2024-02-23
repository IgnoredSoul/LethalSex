using GameNetcodeStuff;
using LethalSex_Core;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace LethalSanity.Modules
{
    public class PostProcessing : LethalClass
    {
        internal static PostProcessing instance { get; private set; }

        // When the player loads
        public override void OnLocalPlayerStart(PlayerControllerB LocalPlayer) => instance = LocalPlayer.gameObject.GetOrAddComponent<PostProcessing>();

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
            LocalPlayer.MaxInsanity = 65f;

            // Add all effects to profile
            ChrAbComp = VolumeProfile.Add<ChromaticAberration>(); ChrAbComp.name = "LS-ChrAbComp";
            LensDistComp = VolumeProfile.Add<LensDistortion>(); LensDistComp.name = "LS-LensDistComp";
            FilmGrainComp = VolumeProfile.Add<FilmGrain>(); FilmGrainComp.name = "LS-FilmGrainComp";
            VignetteComp = VolumeProfile.Add<Vignette>(); VignetteComp.name = "LS-VignetteComp"; VignetteComp.opacity.value = 0.5f; VignetteComp.smoothness.value = 25f;
            DOFComp = VolumeProfile.Add<DepthOfField>(); DOFComp.name = "LS-DOFComp"; DOFComp.farFocusStart.Override(2000); DOFComp.farFocusEnd.Override(2000);
            CAComp = VolumeProfile.Add<ColorAdjustments>(); CAComp.name = "LS-CAComp";
        }

        private void LateUpdate()
        {
            // Just to make sure no errors arise
            if (!LocalPlayer.PlayerController) return;

            #region Level 1

            // When:
            //		Insanity is less than 45
            //		Insanity Level 1 has been checked
            if (LocalPlayer.Insanity < 35 && InsanityLvl1)
            {
                // Set this level of insanity to false
                InsanityLvl1 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, 0.1f, 0.75f);

                // Log to console
                ConsoleManager.Log("R; Stage: 1 | SL: Ambience");
            }

            // When:
            //		Insanity is greater than 35
            //		Insanity Level 1 has not been checked
            if (LocalPlayer.Insanity >= 35 && !InsanityLvl1)
            {
                // Set this level of insanity to true
                InsanityLvl1 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, 1.6f, 15f);
                Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, 0.5f, 25f);

                // Log to console
                ConsoleManager.Log("A; Stage: 1 | SL: Ambience");

                // Shake the camera slightly
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }

            #endregion Level 1

            /*
             * 26/12/23
             * At level two, the fake bracken will spawn. It will not despawn ever after that.
             * I may make the bracken increase in speed and what not based on sanity.
            */

            #region Level 2

            // When:
            //		Insanity is less than 45
            //		Insanity Level 2 has been checked
            if (LocalPlayer.Insanity < 45 && InsanityLvl2)
            {
                // Set this level of insanity to false
                InsanityLvl2 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, 0f, 0.75f);

                // Change the config for the wobble effect
                CameraShake.instance.wobbleAmount = 0;

                // Log to console
                ConsoleManager.Log("R; Stage: 2 | SL: Enemy, Laughing | SI: 20 | SP: 5 | WA: 0");
            }

            // When:
            //		Insanity is greater than 45
            //		Insanity Level 2 has not been checked
            if (LocalPlayer.Insanity >= 45 && !InsanityLvl2)
            {
                // Set this level of insanity to true
                InsanityLvl2 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, 1f, 20f);
                Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, 0.55f, 27f);

                // Start the wobble effect and configure it
                CameraShake.instance.wobbleAmount = 0.05f;
                CameraShake.instance.HandleWobble();

                // Log to console
                ConsoleManager.Log("A; Stage: 2 | SL: Enemy, Laughing | SI: 15 | SP: 15");
            }

            #endregion Level 2

            #region Level 3

            // When:
            //		Insanity is less than 65
            //		Insanity Level 3 has been checked
            if (LocalPlayer.Insanity < 65 && InsanityLvl3)
            {
                // Set this level of insanity to false
                InsanityLvl3 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, 2000, 0.75f);
                Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, 2000, 0.75f);
                Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, 0.3f, 17f);

                // Change the config for the wobble effect
                CameraShake.instance.wobbleAmount = 0.05f;

                // Log to console
                ConsoleManager.Log("R; Stage: 3 | SL: Knocking | SI: 15 | SP: 15 | WA: 0.05f");
            }

            // When:
            //		Insanity is greater than 65
            //		Insanity Level 3 has not been checked
            if (LocalPlayer.Insanity >= 65 && !InsanityLvl3)
            {
                // Set this level of insanity to true
                InsanityLvl3 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, 5, 20f);
                Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, 25, 20f);
                Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, -65f, 25f);

                // Change the config for the wobble effect
                CameraShake.instance.wobbleAmount = 0.15f;

                // Log to console
                ConsoleManager.Log("A; Stage: 3 | SL: Knocking | SI: 5 | SP: 25 | WA: 0.15f");
            }

            #endregion Level 3

            /*
             * 2/12/23
             * Maybe some 30 second timer will start when at level 3?
             * Vision will fade out and will slowly die?
             *
             * 22/2/24
             * ehh maybe not, single player would be almost impossible
             * but adds danger to being alone...
             * IM SO CONFLICTED AAAAAAAAAAAAAAA
            */
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}