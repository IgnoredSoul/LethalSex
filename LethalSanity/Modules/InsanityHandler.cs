using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine.InputSystem;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class InsanityHandler : LethalClass
    {
        public static InsanityHandler Module { get; private set; } = null!;

        /// <summary>
        /// Assign Module to this instance.
        /// </summary>
/*        protected override void OnRegister()
        {
            base.OnRegister();
            Module = this;
        }*/

        /// <summary>
        /// When the local <seealso cref="PlayerControllerB"/> starts, change settings for it.
        /// </summary>
        /// <param name="LocalPlayer"></param>
        protected override void OnLocalPlayerStart(PlayerControllerB LocalPlayer)
        {
            // Set new max insanity
            LocalPlayer.maxInsanityLevel = Config.IO_MaxInsanity;
        }

        private void Update()
        {
            if ((bool)CameraLeaning.Module?.Registered) HCL();
        }

        private void LateUpdate()
        {
            if ((bool)PostProcessing.Module?.Registered) HPP();
            if ((bool)CameraShake.Module?.Registered) HCS();
        }

        // ===================[ Handle Post Processing ]=================== \\
        private void HPP()
        {
            // Check if the config is setup properly
            if (Config.PP_Level1 > Config.PP_Level2 || Config.PP_Level1 > Config.PP_Level3)
            {
                LethalSex_Core.Main.mls.LogFatal(
                    Config.PP_Level1 > Config.PP_Level3 ? "PostProcessing Level 1 cannot be greater than Level 3" : "PostProcessing Level 1 cannot be greater than Level 2 and/or Level 3"
                );

                PostProcessing.Module?.ManualUnregister();
            }
            // Check if the config is setup properly 2
            if (Config.PP_Level2 > Config.PP_Level3)
            {
                LethalSex_Core.Main.mls.LogFatal("PostProcessing Level 2 cannot be greater than Level 3");
                PostProcessing.Module?.ManualUnregister();
            }

            // Dont do anything if the player isnt present.
            if (!LocalPlayer.PlayerController) return;

            #region Level 1

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL1)
            //		Insanity Level 1 has been checked
            if (LocalPlayer.Insanity < Config.PP_Level1 && PostProcessing.Module.InsanityLvl1)
            {
                // Set this level of insanity to false
                PostProcessing.Module.InsanityLvl1 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", PostProcessing.Module.FilmGrainComp.intensity.Override, PostProcessing.Module.FilmGrainComp.intensity.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("Vignette", PostProcessing.Module.VignetteComp.intensity.Override, PostProcessing.Module.VignetteComp.intensity.value, 0.1f, 0.75f);
            }

            // When:
            //		Insanity is greater than (CONFIG.PP_LEVEL1)
            //		Insanity Level 1 has not been checked
            if (LocalPlayer.Insanity >= Config.PP_Level1 && !PostProcessing.Module.InsanityLvl1)
            {
                // Set this level of insanity to true
                PostProcessing.Module.InsanityLvl1 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("FilmGrain", PostProcessing.Module.FilmGrainComp.intensity.Override, PostProcessing.Module.FilmGrainComp.intensity.value, 1.6f, 15f);
                Extensions.SmoothIncrementValue("Vignette", PostProcessing.Module.VignetteComp.intensity.Override, PostProcessing.Module.VignetteComp.intensity.value, 0.5f, 25f);

                // Shake the camera slightly
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }

            #endregion Level 1

            #region Level 2

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has been checked
            if (LocalPlayer.Insanity < Config.PP_Level2 && PostProcessing.Module.InsanityLvl2)
            {
                // Set this level of insanity to false
                PostProcessing.Module.InsanityLvl2 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", PostProcessing.Module.ChrAbComp.intensity.Override, PostProcessing.Module.ChrAbComp.intensity.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("LensDist", PostProcessing.Module.LensDistComp.intensity.Override, PostProcessing.Module.LensDistComp.intensity.value, 0f, 0.75f);
            }

            // When:
            //		Insanity is greater than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has not been checked
            if (LocalPlayer.Insanity >= Config.PP_Level2 && !PostProcessing.Module.InsanityLvl2)
            {
                // Set this level of insanity to true
                PostProcessing.Module.InsanityLvl2 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("ChrAb", PostProcessing.Module.ChrAbComp.intensity.Override, PostProcessing.Module.ChrAbComp.intensity.value, 1f, 20f);
                Extensions.SmoothIncrementValue("LensDist", PostProcessing.Module.LensDistComp.intensity.Override, PostProcessing.Module.LensDistComp.intensity.value, 0.55f, 27f);
            }

            #endregion Level 2

            #region Level 3

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has been checked
            if (LocalPlayer.Insanity < Config.PP_Level3 && PostProcessing.Module.InsanityLvl3)
            {
                // Set this level of insanity to false
                PostProcessing.Module.InsanityLvl3 = false;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", PostProcessing.Module.DOFComp.farFocusStart.Override, PostProcessing.Module.DOFComp.farFocusStart.value, 2000, 0.75f);
                Extensions.SmoothIncrementValue("DOFEnd", PostProcessing.Module.DOFComp.farFocusEnd.Override, PostProcessing.Module.DOFComp.farFocusEnd.value, 2000, 0.75f);
                Extensions.SmoothIncrementValue("CA", PostProcessing.Module.CAComp.saturation.Override, PostProcessing.Module.CAComp.saturation.value, 0f, 0.75f);
                Extensions.SmoothIncrementValue("LensDist", PostProcessing.Module.LensDistComp.intensity.Override, PostProcessing.Module.LensDistComp.intensity.value, 0.3f, 17f);
            }

            // When:
            //		Insanity is greater than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has not been checked
            if (LocalPlayer.Insanity >= Config.PP_Level3 && !PostProcessing.Module.InsanityLvl3)
            {
                // Set this level of insanity to true
                PostProcessing.Module.InsanityLvl3 = true;

                // Apply override's
                Extensions.SmoothIncrementValue("DOFStart", PostProcessing.Module.DOFComp.farFocusStart.Override, PostProcessing.Module.DOFComp.farFocusStart.value, 5, 20f);
                Extensions.SmoothIncrementValue("DOFEnd", PostProcessing.Module.DOFComp.farFocusEnd.Override, PostProcessing.Module.DOFComp.farFocusEnd.value, 25, 20f);
                Extensions.SmoothIncrementValue("CA", PostProcessing.Module.CAComp.saturation.Override, PostProcessing.Module.CAComp.saturation.value, -65f, 25f);
            }

            #endregion Level 3
        }

        // ====================[ Handle Camera Shake ]==================== \\
        private void HCS()
        {
            // Dont do anything if the player isnt present.
            if (!LocalPlayer.PlayerController) return;

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has been checked
            if (LocalPlayer.Insanity < Config.PP_Level2 && PostProcessing.Module.InsanityLvl2)
            {
                // Change the config for the wobble effect, disabling the camera shake
                CameraShake.Module.wobbleAmount = 0;
            }

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL2)
            //		Insanity Level 2 has been checked
            if (LocalPlayer.Insanity >= Config.PP_Level2 && !PostProcessing.Module.InsanityLvl2)
            {
                // Start the wobble effect and configure it
                CameraShake.Module.wobbleAmount = 0.05f;
                CameraShake.Module.HandleWobble();
            }

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has been checked
            if (LocalPlayer.Insanity < Config.PP_Level3 && PostProcessing.Module.InsanityLvl3)
            {
                // Change the config for the wobble effect
                CameraShake.Module.wobbleAmount = 0.05f;
            }

            // When:
            //		Insanity is less than (CONFIG.PP_LEVEL3)
            //		Insanity Level 3 has been checked
            if (LocalPlayer.Insanity >= Config.PP_Level3 && !PostProcessing.Module.InsanityLvl3)
            {
                // Change the config for the wobble effect
                CameraShake.Module.wobbleAmount = 0.15f;
            }
        }

        // ===================[ Handle Camera Leaning ]=================== \\
        private void HCL()
        {
            if (!LocalPlayer.Player) return;

            Vector3 eulerAngles = LocalPlayer.Camera.transform.localRotation.eulerAngles;
            Vector2 vector = Mouse.current.delta.ReadValue();
            if (!LocalPlayer.IsMenuOpen && !LocalPlayer.IsTermOpen && (vector.x >= Config.CL_threshold || vector.x <= -Config.CL_threshold))
            {
                float multiplier = 0.05f;
                if (LocalPlayer.PlayerController.isSprinting)
                    multiplier += 0.05f;
                if (StartOfRound.Instance.fearLevel >= 0.4f)
                    multiplier += 0.05f;
                if (LocalPlayer.Insanity >= 60f)
                    multiplier += 0.05f;
                if (TimeOfDay.Instance.dayMode >= DayMode.Sundown)
                    multiplier += 0.05f;

                // Check if its negative or not before multipling then clamp to max lean
                eulerAngles.z += Mathf.Clamp(multiplier * ((vector.x < 0f) ? (vector.x - Config.CL_threshold) : (vector.x + Config.CL_threshold)), -Config.CL_maxLeanAngle, Config.CL_maxLeanAngle);
            }
            else
                eulerAngles.z = 0f;

            // Do the leaning shit
            Quaternion quaternion = Quaternion.Euler(eulerAngles);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, quaternion, 5f * Time.deltaTime);
        }

        // ================[ Handle Stalker Hallucination ]================ \\
        private void HSH()
        {
            StalkerHallucination.Module.LookAt();
            if (LocalPlayer.PlayerController.HasLineOfSightToPosition(base.transform.position + Vector3.up * 0.5f, 30f, 60, -1f))
            {
                StalkerHallucination.Module.animator.SetBool("anger", true);
                StalkerHallucination.Module.animator.SetBool("sneak", false);
                StalkerHallucination.Module.animator.SetFloat("speedMultiplier", Vector3.ClampMagnitude(transform.position - StalkerHallucination.Module.previousPosition, 1f).sqrMagnitude / (Time.deltaTime * 2f));

                StalkerHallucination.Module.RunAway();

                if (!StalkerHallucination.Module.runningAway)
                {
                    StartCoroutine(StalkerHallucination.Module.EvadeTimer());
                }
            }
            else if (!StalkerHallucination.Module.runningAway)
            {
                StalkerHallucination.Module.animator.SetBool("anger", false);
                StalkerHallucination.Module.animator.SetBool("sneak", true);
                StalkerHallucination.Module.animator.SetFloat("speedMultiplier", Vector3.ClampMagnitude(transform.position - StalkerHallucination.Module.previousPosition, 1f).sqrMagnitude / (Time.deltaTime * 4f));

                StalkerHallucination.Module.FollowPlayer();
            }
            StalkerHallucination.Module.previousPosition = transform.position;
        }
    }
}