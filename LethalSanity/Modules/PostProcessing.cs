﻿using UnityEngine;
using LethalSex_Core;
using UnityEngine.Rendering;
using LethalSex_Core.Modules;
using UnityEngine.Rendering.HighDefinition;

namespace LethalSanity.Modules
{
	internal class PostProcessing : LethalModule
	{
		/// <summary>
		/// Instance of the class
		/// </summary>
		public static PostProcessing instance { get; private set; }

		/// <summary>
		/// Instance of the monobehaviour
		/// </summary>
		public static PostProcessing component { get; private set; }

		/// <summary>
		/// Instance of the volume object
		/// </summary>
		private GameObject VolumeObject { get; set; }

		/// <summary>
		/// Instance of the volume profile
		/// </summary>
		private VolumeProfile VolumeProfile { get; set; }

		/// <summary>
		/// When registering the module, check the config to see if the user wants this module disabled
		/// </summary>
		public override void OnRegister() => instance = this;

		/// <summary>
		/// When the local player initializes
		/// </summary>
		/// <param name="_LocalPlayer"></param>
		public override void OnShipLand() => component = LocalPlayer.Player.AddComponent<PostProcessing>();

		private void Start()
		{
			VolumeObject = Instantiate(
				GameObject.Find("Systems/Rendering/VolumeMain"),
				GameObject.Find("Systems/Rendering/VolumeMain").transform.parent
			);                                                                      // Make new Volume Object

			VolumeObject.name = "LS-Volume";                                        // Rename object

			VolumeProfile = new VolumeProfile();                                    // Make new VolumeProfilet

			VolumeProfile.name = "LS-VolumeProfile";                                // Name new VolumeProfile

			VolumeObject.GetComponentInChildren<Volume>().priority = Main.config.PP;// Set the volume's priority

			VolumeObject.GetComponentInChildren<Volume>().profile = VolumeProfile;  // Assign profile

			MakeDOF();
			MakeChrAb();
			MakeVignette();
			MakeLensDist();
			MakeFilmGrain();
			MakeSaturation();
		}

		/// <summary>
		/// Prepares the Vignette effect.
		/// </summary>
		private void MakeVignette()
		{
			// If the effect should be turned off.
			if (!Main.config.VC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.VC.Item2, Main.config.VC.Item3, 65);

			// Set up Vignette effect
			Vignette VignetteComp = VolumeProfile.Add<Vignette>();
			VignetteComp.smoothness.value = 1;
			VignetteComp.roundness.value = 0.842f;
			VignetteComp.name = "LS-VignetteComp";

			// Set up Vignette event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(0.45f, 0.6f);
				float time = NumberUtils.NextF(10f, 25f);
				ConsoleManager.Log($"Applying Vignette ({val}/{time})");
				Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, val, time);
			}, () =>
			{
				float time = NumberUtils.NextF(0.5f, 2);
				ConsoleManager.Log($"Resetting Vignette (0/{time})");
				Extensions.SmoothIncrementValue("Vignette", VignetteComp.intensity.Override, VignetteComp.intensity.value, 0, time);
			});

			ConsoleManager.Log($"Vignette applies at lvl: {insanitylvl}");
		}

		/// <summary>
		/// Prepares the Film Grain effect.
		/// </summary>
		private void MakeFilmGrain()
		{
			// If the effect should be turned off.
			if (!Main.config.FGC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.FGC.Item2, Main.config.FGC.Item3, 65);

			// Set up Film Grain effect
			FilmGrain FilmGrainComp = VolumeProfile.Add<FilmGrain>();
			FilmGrainComp.name = "LS-FilmGrainComp";

			// Set up Film Grain event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(0.5f, 1f);
				float time = NumberUtils.NextF(10, 20);
				ConsoleManager.Log($"Applying FilmGrain ({val}/{time})");
				Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, val, time);
			}, () =>
			{
				float time = NumberUtils.NextF(0.5f, 2);
				ConsoleManager.Log($"Resetting FilmGrain (0/{time})");
				Extensions.SmoothIncrementValue("FilmGrain", FilmGrainComp.intensity.Override, FilmGrainComp.intensity.value, 0f, time);
			});
			ConsoleManager.Log($"Film Grain applies at lvl: {insanitylvl}");
		}

		/// <summary>
		/// Prepares the Chromatic Aberration effect.
		/// </summary>
		private void MakeChrAb()
		{
			// If the effect should be turned off.
			if (!Main.config.CAC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.CAC.Item2, Main.config.CAC.Item3, 65);

			// Set up Chromatic Aberration effect
			ChromaticAberration ChrAbComp = VolumeProfile.Add<ChromaticAberration>();
			ChrAbComp.name = "LS-ChrAbComp";

			// Set up Chromatic Aberration event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(1f, 2f);
				float time = NumberUtils.NextF(10, 20);
				ConsoleManager.Log($"Applying Chromatic Aberation ({val}/{time})");
				Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, NumberUtils.NextF(0.5f, 1.5f), NumberUtils.NextF(10, 20));
			}, () =>
			{
				float time = NumberUtils.NextF(0.5f, 2);
				ConsoleManager.Log($"Resetting Chromatic Aberration (0/{time})");
				Extensions.SmoothIncrementValue("ChrAb", ChrAbComp.intensity.Override, ChrAbComp.intensity.value, 0f, NumberUtils.NextF(0.5f, 2));
			});

			ConsoleManager.Log($"Chromatic Aberration applies at lvl: {insanitylvl}");
		}

		/// <summary>
		/// Prepares the Lens Distortion effect.
		/// </summary>
		private void MakeLensDist()
		{
			// If the effect should be turned off.
			if (!Main.config.LDC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.LDC.Item2, Main.config.LDC.Item3, 65);

			// Set up Lens Distortion effect.
			LensDistortion LensDistComp = VolumeProfile.Add<LensDistortion>();
			LensDistComp.name = "LS-LensDistComp";

			// Set up Lens Distortion event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(0.4f, 0.6f);
				float time = NumberUtils.NextF(20, 30);
				ConsoleManager.Log($"Applying Lens Distortion ({val}/{time})");
				Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, val, time);
			}, () =>
			{
				float time = NumberUtils.NextF(0.5f, 2);
				ConsoleManager.Log($"Resetting Lens Distortion (0/{time})");
				Extensions.SmoothIncrementValue("LensDist", LensDistComp.intensity.Override, LensDistComp.intensity.value, 0, time);
			});

			ConsoleManager.Log($"Lens Distortion applies at lvl: {insanitylvl}");
		}

		/// <summary>
		/// Prepares the Depth of Field effect.
		/// </summary>
		private void MakeDOF()
		{
			// If the effect should be turned off.
			if (!Main.config.DOFC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.DOFC.Item2, Main.config.DOFC.Item3, 65);

			// Set up Depth of Field effect
			DepthOfField DOFComp = VolumeProfile.Add<DepthOfField>();
			DOFComp.farFocusStart.Override(2000);
			DOFComp.farFocusEnd.Override(2000);
			DOFComp.name = "LS-DOFComp";

			// Set up Depth of Field event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(3, 8);
				float time = NumberUtils.NextF(10, 17);
				ConsoleManager.Log($"Applying Depth of Field (Start) ({val}/{time})");
				Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, val, time);

				float val_ = NumberUtils.NextF(10, 15);
				float time_ = NumberUtils.NextF(18, 25);
				ConsoleManager.Log($"Applying Depth of Field (End) ({val_}/{time_})");
				Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, val_, time_);
			}, () =>
			{
				float time = NumberUtils.NextF(10, 13);
				ConsoleManager.Log($"Resseting Depth of Field (Start) (2000/{time})");
				Extensions.SmoothIncrementValue("DOFStart", DOFComp.farFocusStart.Override, DOFComp.farFocusStart.value, 2000f, time);

				float time_ = NumberUtils.NextF(13, 16);
				ConsoleManager.Log($"Resseting Depth of Field (End) (2000/{time_})");
				Extensions.SmoothIncrementValue("DOFEnd", DOFComp.farFocusEnd.Override, DOFComp.farFocusEnd.value, 2000, time_);
			});

			ConsoleManager.Log($"Depth Of Field applies at lvl: {insanitylvl}");
		}

		/// <summary>
		/// Prepares the Color Adjustments effect.
		/// </summary>
		private void MakeSaturation()
		{
			// If the effect should be turned off.
			if (!Main.config.SC.Item1) return;

			// Get random insanity level to activate
			float insanitylvl = NumberUtils.NextO(Main.config.SC.Item2, Main.config.SC.Item3, 65);

			// Set up Color Adjustments effect.
			ColorAdjustments CAComp = VolumeProfile.Add<ColorAdjustments>();
			CAComp.name = "LS-CAComp";

			// Set up Color Adjustments event
			AddSanityEvent(insanitylvl, () =>
			{
				float val = NumberUtils.NextF(50, 70);
				float time = NumberUtils.NextF(18, 28);
				ConsoleManager.Log($"Applying Color Adjustments ({val}/{time})");
				Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, -val, time);
			}, () =>
			{
				float time = NumberUtils.NextF(0.5f, 2);
				ConsoleManager.Log($"Resseting Color Adjustments (0/{time})");
				Extensions.SmoothIncrementValue("CA", CAComp.saturation.Override, CAComp.saturation.value, 0, time);
			});

			ConsoleManager.Log($"Color Adjustments applies at lvl: {insanitylvl}");
		}

		#region "On" Unity Message's

		private void OnDestroy() => base.Destroyed();

		private void OnDisable() => base.Disabled();

		private void OnEnable() => base.Enabled();

		#endregion "On" Unity Message's
	}
}