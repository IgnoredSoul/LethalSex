using GameNetcodeStuff;
using LethalSex_Core;
using System.Collections;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class CameraShake : LethalClass
    {
        /// <summary>
        /// CameraShake component Module
        /// </summary>
        public static CameraShake Module { get; private set; }

        protected override void OnRegister()
        {
            if (!Config.CS_ToggleModule) Unregister();
        }

        /// <summary>
        /// When the local player component is initiated, assign and create a new instance of the component onto the camera
        /// </summary>
        /// <param name="_LocalPlayer"></param>
        protected override void OnLocalPlayerStart(PlayerControllerB _LocalPlayer) => Module = _LocalPlayer.gameplayCamera.gameObject.GetOrAddComponent<CameraShake>();

        /// <summary>
        /// How much the camera should wobble. (Low floats)
        /// </summary>
        internal float wobbleAmount { get; set; }

        private Coroutine wobbleCoro { get; set; }

        /// <summary>
        /// Handling and executing the camera wobble
        /// </summary>
        internal void HandleWobble()
        {
            // Stop and reset previous wobble
            if (wobbleCoro != null) StopCoroutine(wobbleCoro); wobbleCoro = null;

            // Start wobbling
            StartCoroutine(DoWobble());

            // Wobble!!!
            IEnumerator DoWobble()
            {
                while (wobbleAmount > 0)
                {
                    // Calculate the wobble effect using Perlin noise
                    float x = NumberUtils.Next(-wobbleAmount, wobbleAmount);
                    float y = NumberUtils.Next(-wobbleAmount, wobbleAmount);

                    // Apply the wobble to the camera's rotation
                    transform.localPosition = Vector3.Lerp(transform.localPosition, new(x, y, 0), Time.deltaTime * 1.275f);

                    yield return new WaitForEndOfFrame();
                }

                // Reset the camera's position
                transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        private void OnDestroy() => base.Destroy();

        private void OnDisable() => base.Disable();

        private void OnEnable() => base.Enable();
    }
}