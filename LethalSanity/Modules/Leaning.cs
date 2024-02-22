﻿using GameNetcodeStuff;
using LethalSex_Core;
using UnityEngine;
using UnityEngine.InputSystem;
using static LethalSex_Core.DevMenuManager;

namespace LethalSanity.Modules
{
    public class Leaning : LethalClass
    {
        public static Leaning instance { get; private set; }

        public override void LocalPlayer_Start(PlayerControllerB _LocalPlayer) => instance = LocalPlayer.Camera.gameObject.GetOrAddComponent<Leaning>();

        public float maxLeanAngle = 35f; // Maximum angle to lean
        public float threshold = 35f; // Mouse speed to lean

        private void Start()
        {
            DMSection sect = new("Eh");

            new DMSlider(sect, (v) => LocalPlayer.Insanity = v, 0, 65, false, "Insanity");
        }

        private void Update()
        {
            if (!LocalPlayer.Player) return;

            // Get the current camera rotation
            Vector3 curRot = LocalPlayer.Camera.transform.localRotation.eulerAngles;

            // Read mouse input
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            // If the menu isnt open and the mouse is moving past the threshold
            if (!LocalPlayer.PlayerController.quickMenuManager.isMenuOpen && (mouseDelta.x >= threshold || mouseDelta.x <= -threshold))
            {
                float LeanMultiplier = 0.05f;

                // If the player is sprinting
                if (LocalPlayer.PlayerController.isSprinting) LeanMultiplier += 0.05f;

                // If the player is sprinting in fear
                if (StartOfRound.Instance.fearLevel >= 0.4f) LeanMultiplier += 0.05f;

                // If the player is fucking insane
                if (LocalPlayer.Insanity >= 60f) LeanMultiplier += 0.05f;

                // Calculate the amount of lean based on the mouse movement and apply the lean in the direction of movement
                curRot.z += Mathf.Clamp((mouseDelta.x * LeanMultiplier), -maxLeanAngle, maxLeanAngle);
            }
            else
            {
                // Set the rotation to 0 so it straightens the camera
                curRot.z = 0;
            }

            // Smoothly interpolate between the current rotation and the target rotation
            Quaternion targetRotation = Quaternion.Euler(curRot);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, (5 * Time.deltaTime));
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}