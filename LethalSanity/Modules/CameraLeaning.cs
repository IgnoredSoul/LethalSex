using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine.InputSystem;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class CameraLeaning : LethalClass
    {
        /// <summary>
        /// CameraLeaning component Module
        /// </summary>
        public static CameraLeaning Module { get; private set; }

        protected override void OnRegister()
        {
            if (!Config.config.CL.Enabled) Unregister();
        }

        /// <summary>
        /// When the local player component is initiated, assign and create a new instance of the component onto the camera
        /// </summary>
        /// <param name="_LocalPlayer"></param>
        protected override void OnLocalPlayerStart(PlayerControllerB _LocalPlayer) => Module = _LocalPlayer.gameplayCamera.gameObject.GetOrAddComponent<CameraLeaning>();

        /// <summary>
        /// Update each frame and calculate the camera lean rotation based on the mouse movement and other factors that multiply the leaning
        /// </summary>
        private void Update()
        {
            if (!LocalPlayer.Player) return;

            Vector3 eulerAngles = LocalPlayer.Camera.transform.localRotation.eulerAngles;
            Vector2 vector = Mouse.current.delta.ReadValue();

            if (!LocalPlayer.IsMenuOpen && !LocalPlayer.IsTermOpen && (vector.x >= Config.config.CL.Threshold || vector.x <= -Config.config.CL.Threshold))
            {
                float multiplier = Config.config.CL_EX.Default;
                if (LocalPlayer.PlayerController.isSprinting)
                    multiplier += Config.config.CL_EX.Sprint;
                if (StartOfRound.Instance.fearLevel >= 0.4f)
                    multiplier += Config.config.CL_EX.Fear;
                if (LocalPlayer.Insanity >= 60f)
                    multiplier += Config.config.CL_EX.Insanity;
                if ((int)TimeOfDay.Instance.dayMode >= Config.config.CL_EX.Daymode)
                    multiplier += Config.config.CL_EX.Day;

                // Check if its negative or not before multipling then clamp to (CONFIG.MAXLEAN)
                eulerAngles.z += Mathf.Clamp(multiplier * ((vector.x < 0f) ? (vector.x - Config.config.CL.Threshold) : (vector.x + Config.config.CL.Threshold)), -Config.config.CL.Max, Config.config.CL.Max);
            }
            else
                eulerAngles.z = 0f;

            // Do the leaning shit
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngles), Config.config.CL.Reset * Time.deltaTime);
        }

        private void OnDestroy() => base.Destroy();

        private void OnDisable() => base.Disable();

        private void OnEnable() => base.Enable();
    }
}