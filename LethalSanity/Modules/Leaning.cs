using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalSanity.Modules
{
    public class Leaning : LethalClass
    {
        public static Leaning instance { get; private set; }

        public override void OnLocalPlayerStart(PlayerControllerB _LocalPlayer)
        {
            if (!Config.ToggleLeanModule) return;
            instance = LocalPlayer.Camera.gameObject.GetOrAddComponent<Leaning>();
        }

        private void Update()
        {
            if (!LocalPlayer.Player) return;

            Vector3 eulerAngles = LocalPlayer.Camera.transform.localRotation.eulerAngles;
            Vector2 vector = Mouse.current.delta.ReadValue();

            if (!LocalPlayer.IsMenuOpen && !LocalPlayer.IsTermOpen && (vector.x >= Config.CL_Threshold || vector.x <= -Config.CL_Threshold))
            {
                float LeanMultiplier = 0.05f;

                // If the player is sprinting
                if (LocalPlayer.PlayerController.isSprinting) LeanMultiplier += 0.05f;

                // If the player is sprinting in fear
                if (StartOfRound.Instance.fearLevel >= 0.4f) LeanMultiplier += 0.05f;

                // If the player is fucking insane
                if (LocalPlayer.Insanity >= 60f) LeanMultiplier += 0.05f;

                // Check if its negative or not before multipling then clamp to (CONFIG.MAXLEAN)
                eulerAngles.z += Mathf.Clamp(LeanMultiplier * ((vector.x < 0f) ? (vector.x - Config.CL_Threshold) : (vector.x + Config.CL_Threshold)), -Config.CL_MaxLean, Config.CL_MaxLean);
            }
            else
                eulerAngles.z = 0f;

            // Do the leaning shit
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngles), 5 * Time.deltaTime);
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}