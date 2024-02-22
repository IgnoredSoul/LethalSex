using GameNetcodeStuff;
using LethalSex_Core;

using System.Collections;
using UnityEngine;

namespace LethalSanity.Modules
{
    internal class CameraShake : LethalClass
    {
        internal static CameraShake instance { get; private set; }

        public override void LocalPlayer_Start(PlayerControllerB _LocalPlayer) => instance = LocalPlayer.Camera.gameObject.GetOrAddComponent<CameraShake>();

        internal float wobbleAmount { get; set; }
        private Coroutine wobbleCoro { get; set; }

        internal void HandleWobble()
        {
            if (wobbleCoro != null) StopCoroutine(wobbleCoro); wobbleCoro = null;
            StartCoroutine(DoWobble());

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

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}