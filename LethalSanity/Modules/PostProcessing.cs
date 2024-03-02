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
        public static PostProcessing Module { get; private set; } = null!;

        /// <summary>
        /// Assign Module to this instance.
        /// </summary>
/*        protected override void OnRegister()
        {
            // Unregister this module if disabled in the config.
            if (!Config.PP_ToggleModule) { ManualUnregister(); return; }

            base.OnRegister();
            Module = this;
        }*/

        /// <summary>
        /// If the module unregisters, unload everything.
        /// </summary>
        protected override void Unregister()
        {
            // Try to destroy the object, if it cannot doesn't matter cause it doesnt exist lol.
            Extensions.TryDestroy(VolumeObject);

            base.Unregister();
            Module = null;
        }

        /// <summary>
        /// When the player loads
        /// </summary>
        /// <param name="LocalPlayer"></param>
        protected override void OnLocalPlayerStart(PlayerControllerB LocalPlayer) => LocalPlayer.gameObject.GetOrAddComponent<PostProcessing>();

        internal ColorAdjustments CAComp { get; private set; }
        internal ChromaticAberration ChrAbComp { get; private set; }
        internal DepthOfField DOFComp { get; private set; }
        internal FilmGrain FilmGrainComp { get; private set; }
        internal LensDistortion LensDistComp { get; private set; }
        internal Vignette VignetteComp { get; private set; }
        internal GameObject VolumeObject { get; private set; }
        internal VolumeProfile VolumeProfile { get; private set; }

        internal bool InsanityLvl1 { get; set; }
        internal bool InsanityLvl2 { get; set; }
        internal bool InsanityLvl3 { get; set; }

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
            LocalPlayer.MaxInsanity = Config.PP_PostPriority;

            // Add all effects to profile
            ChrAbComp = VolumeProfile.Add<ChromaticAberration>(); ChrAbComp.name = "LS-ChrAbComp";
            LensDistComp = VolumeProfile.Add<LensDistortion>(); LensDistComp.name = "LS-LensDistComp";
            FilmGrainComp = VolumeProfile.Add<FilmGrain>(); FilmGrainComp.name = "LS-FilmGrainComp";
            VignetteComp = VolumeProfile.Add<Vignette>(); VignetteComp.name = "LS-VignetteComp"; VignetteComp.opacity.value = 0.5f; VignetteComp.smoothness.value = 25f;
            DOFComp = VolumeProfile.Add<DepthOfField>(); DOFComp.name = "LS-DOFComp"; DOFComp.farFocusStart.Override(2000); DOFComp.farFocusEnd.Override(2000);
            CAComp = VolumeProfile.Add<ColorAdjustments>(); CAComp.name = "LS-CAComp";
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}