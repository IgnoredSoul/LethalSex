using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;
using UnityEngine.Rendering.HighDefinition;

namespace LethalSex.Modules
{
	internal class Behaviour_NakedAndAfraid : LethalClass
	{
		internal static Behaviour_NakedAndAfraid _instance { get; private set; }

		private static bool InsanityLvl1 = false;
		private static bool InsanityLvl2 = false;
		private static bool InsanityLvl3 = false;

		internal static ChromaticAberration ChrAbComp { get; private set; }
		internal static LensDistortion LensDistComp { get; private set; }
		internal static VolumeProfile VolumeProfile { get; private set; }
		internal static ColorAdjustments CAComp { get; private set; }
		internal static FilmGrain FilmGrainComp { get; private set; }
		internal static GameObject VolumeObject { get; private set; }
		internal static Vignette VignetteComp { get; private set; }
		internal static DepthOfField DOFComp { get; private set; }
		internal static Volume VolumeComp { get; private set; }

        // When player controller starts add this component
        internal override async void HUDManager_Start()
        {
            while (!Extensions.GetLocalPlayer()) await Task.Delay(250);
            Extensions.GetLocalPlayer().GetOrAddComponent<Behaviour_NakedAndAfraid>();
        }

        // When component is added
        async void Start()
		{
			// Set instance
			_instance = this;

			// Duplicate existing Volume
			GameObject OrigVolume;
			while ((OrigVolume = GameObject.Find("Systems/Rendering/VolumeMain")) == null) await Task.Delay(250);

			VolumeObject = Instantiate(OrigVolume, OrigVolume.transform.parent);
			VolumeObject.name = "LS_Volume";

			// Make new VolumeProfile if it doesnt exist
			VolumeProfile = new VolumeProfile();
			VolumeProfile.name = "LS_VolumeProfile";

			// Assign profile
			VolumeObject.GetComponentInChildren<Volume>().profile = VolumeProfile;

			// Set max insanity
			Extensions.GetLocalPlayerController().maxInsanityLevel = 65;

			// Add all effects
			ChrAbComp = VolumeProfile.Add<ChromaticAberration>(); ChrAbComp.name = "LS_ChrAbComp";
			LensDistComp = VolumeProfile.Add<LensDistortion>(); LensDistComp.name = "LS_LensDistComp";
			FilmGrainComp = VolumeProfile.Add<FilmGrain>(); FilmGrainComp.name = "LS_FilmGrainComp";
			VignetteComp = VolumeProfile.Add<Vignette>(); VignetteComp.name = "LS_VignetteComp"; VignetteComp.opacity.value = 0.5f; VignetteComp.smoothness.value = 25f;
			DOFComp = VolumeProfile.Add<DepthOfField>(); DOFComp.name = "LS_DOFComp";
			CAComp = VolumeProfile.Add<ColorAdjustments>(); CAComp.name = "LS_CAComp";
		}

		// LateUpdate
		void LateUpdate()
		{
            #region Level 1
            // When:
            //		Insanity is greater than 35
            //		Insanity Level 1 has not been checked
            if (Extensions.GetLocalPlayerController().insanityLevel >= 35)
			{
				if(!InsanityLvl1)
				{
					LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Applying level 1");

					Extensions.SmoothIncrementValue("FilmGrain", (value) =>
					{
						FilmGrainComp.intensity.Override(value);
					}, FilmGrainComp.intensity.value, 1.6f, 15f);

					Extensions.SmoothIncrementValue("Vignette", (value) =>
					{
						VignetteComp.intensity.Override(value);
					}, VignetteComp.intensity.value, 0.5f, 25f);
					InsanityLvl1 = true;
				}

				if(NumberUtils.Chance(1))
				{
					if(AudioHandler.TimeSinceLastPlayed >= 25f)
                    {
                        LogHandler.Msg($"Playing cause {AudioHandler.TimeSinceLastPlayed} is above 5");
                        AudioHandler.Ambience[AudioHandler.Ambience.ElementAt(NumberUtils.Next(AudioHandler.Ambience.Count - 1)).Key].PlaySound(AudioHandler.AudioSource, StartOfRound.Instance.localPlayerController.transform.position);
                        LogHandler.Msg($"Set last played {AudioHandler.TimeSinceLastPlayed}");
                    }
					else
					{
						LogHandler.Msg($"Didnt play cause {AudioHandler.TimeSinceLastPlayed} is below 5");
					}
				}
			}

            // When:
            //		Insanity is less than 45
            //		Insanity Level 1 has been checked
            if (Extensions.GetLocalPlayerController().insanityLevel < 35 && InsanityLvl1)
			{
				LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Removing level 1");

				Extensions.SmoothIncrementValue("FilmGrain", (value) =>
				{
					FilmGrainComp.intensity.Override(value);
				}, FilmGrainComp.intensity.value, 0f, 0.75f);

				Extensions.SmoothIncrementValue("Vignette", (value) =>
				{
					VignetteComp.intensity.Override(value);
				}, VignetteComp.intensity.value, 0.1f, 0.75f);
				InsanityLvl1 = false;
			}
            #endregion

            #region Level 2
            // When:
            //		Insanity is greater than 45
            //		Insanity Level 2 has not been checked
            if (Extensions.GetLocalPlayerController().insanityLevel >= 45)
			{
				if(!InsanityLvl2)
				{
					LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Applying level 2");

					Extensions.SmoothIncrementValue("ChrAb", (value) =>
					{
						ChrAbComp.intensity.Override(value);
					}, ChrAbComp.intensity.value, 1f, 20f);

					Extensions.SmoothIncrementValue("LensDist", (value) =>
					{
						LensDistComp.intensity.Override(value);
					}, LensDistComp.intensity.value, 0.3f, 17f);
					InsanityLvl2 = true;
                }

                if (NumberUtils.Chance(1))
                {
                    if (AudioHandler.TimeSinceLastPlayed >= 25f)
                    {
                        AudioHandler.Enemies[AudioHandler.Enemies.ElementAt(NumberUtils.Next(AudioHandler.Enemies.Count - 1)).Key].PlaySound(AudioHandler.AudioSource, StartOfRound.Instance.localPlayerController.transform.position);
                    }
                }

                if (NumberUtils.Chance(1))
                {
                    if (AudioHandler.TimeSinceLastPlayed >= 25f)
                    {
                        AudioHandler.Laughing[AudioHandler.Laughing.ElementAt(NumberUtils.Next(AudioHandler.Laughing.Count - 1)).Key].PlaySound(AudioHandler.AudioSource, StartOfRound.Instance.localPlayerController.transform.position);
                    }
                }
            }

            // When:
            //		Insanity is less than 45
            //		Insanity Level 2 has been checked
            if (Extensions.GetLocalPlayerController().insanityLevel < 45 && InsanityLvl2)
			{
				LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Removing level 2");

				Extensions.SmoothIncrementValue("ChrAb", (value) =>
				{
					ChrAbComp.intensity.Override(value);
				}, ChrAbComp.intensity.value, 0f, 0.75f);

				Extensions.SmoothIncrementValue("LensDist", (value) =>
				{
					LensDistComp.intensity.Override(value);
				}, LensDistComp.intensity.value, 0f, 0.75f);
				InsanityLvl2 = false;
			}
            #endregion

            #region Level 3 
            // When:
            //		Insanity is greater than 65
            //		Insanity Level 3 has not been checked
            if (Extensions.GetLocalPlayerController().insanityLevel >= 65)
			{
				if(!InsanityLvl3)
				{
					LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Applying level 3");

					Extensions.SmoothIncrementValue("DOFStart", (value) =>
					{
						DOFComp.farFocusStart.Override(value);
					}, DOFComp.farFocusStart.value, 7, 20f);

					Extensions.SmoothIncrementValue("DOFEnd", (value) =>
					{
						DOFComp.farFocusEnd.Override(value);
					}, DOFComp.farFocusEnd.value, 1, 17f);

					Extensions.SmoothIncrementValue("CA", (value) =>
					{
						CAComp.saturation.Override(value);
					}, CAComp.saturation.value, -60f, 25f);
					InsanityLvl3 = true;
				}

                if (NumberUtils.Chance(2))
                {
                    if (AudioHandler.TimeSinceLastPlayed >= 25f)
                    {
                        AudioHandler.Knocking[AudioHandler.Knocking.ElementAt(NumberUtils.Next(AudioHandler.Knocking.Count - 1)).Key].PlaySound(AudioHandler.AudioSource, StartOfRound.Instance.localPlayerController.transform.position);
                    }
                }
            }

            // When:
            //		Insanity is less than 65
            //		Insanity Level 3 has been checked
            if (Extensions.GetLocalPlayerController().insanityLevel < 65 && InsanityLvl3)
			{
				LogHandler.Msg(LogHandler.MessageColors.BrightCyan, "Removing level 3");

				Extensions.SmoothIncrementValue("DOFStart", (value) =>
				{
					DOFComp.farFocusStart.Override(value);
				}, DOFComp.farFocusStart.value, 2000, 0.75f);

				Extensions.SmoothIncrementValue("DOFEnd", (value) =>
				{
					DOFComp.farFocusEnd.Override(value);
				}, DOFComp.farFocusEnd.value, 2000, 0.75f);

				Extensions.SmoothIncrementValue("CA", (value) =>
				{
					CAComp.saturation.Override(value);
				}, CAComp.saturation.value, 0f, 0.75f);
				InsanityLvl3 = false;
			}
			#endregion
		}

		// Calls for logging
		void OnEnable() =>  base.Enabled();
		void OnDisable() => base.Disabled();
		void OnDestroy() => base.Destroyed();
	}
}
