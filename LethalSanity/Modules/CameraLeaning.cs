using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalSanity.Modules
{
    internal class CameraLeaning : LethalClass
    {
        public static CameraLeaning instance { get; private set; }

        public override void OnLocalPlayerStart(PlayerControllerB _LocalPlayer)
        {
            if (!Config.CL_ToggleLeanModule) return;
            instance = LocalPlayer.Camera.gameObject.GetOrAddComponent<CameraLeaning>();
        }

        private void Update()
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
    }
}