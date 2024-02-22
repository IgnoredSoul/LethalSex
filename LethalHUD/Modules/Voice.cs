using Dissonance;
using GameNetcodeStuff;
using LethalSex_Core;
using System.Collections.Generic;
using UnityEngine;

namespace LethalHUD.Modules
{
    internal class Voice : LethalClass
    {
        public override void LocalPlayer_Start(PlayerControllerB LocalPlayer)
        {
            if (!Config.ToggleVoice) return;

            instance = LocalPlayer.gameObject.AddComponent<Voice>();
        }

        internal static Voice instance { get; private set; }

        private bool AllowedToMute = true;
        private bool ToggleState { get; set; }

        private List<Sprite> Speaker_Icons = new List<Sprite>()
        {
            Main.bundle.LoadAsset<Sprite>("assets/lethalhud/Speaker_Muted.png"),
            Main.bundle.LoadAsset<Sprite>("assets/lethalhud/Speaker_None.png"),
            Main.bundle.LoadAsset<Sprite>("assets/lethalhud/Speaker_Low.png"),
            Main.bundle.LoadAsset<Sprite>("assets/lethalhud/Speaker_High.png")
        };

        private void HandleToggleMute()
        {
            if (!IngamePlayerSettings.Instance.playerInput.actions.FindAction("VoiceButton", false).triggered) return;

            // When:
            //      Push to talk
            //      Is in terminal
            //      Not allowed to mute
            if (IngamePlayerSettings.Instance.settings.pushToTalk || !AllowedToMute) return;

            // Invert the state of the mic
            IngamePlayerSettings.Instance.settings.micEnabled = !IngamePlayerSettings.Instance.settings.micEnabled;

            // Log
            ConsoleManager.Log($"Is muted?: {!IngamePlayerSettings.Instance.settings.micEnabled}");

            // Set current state bool to what it is now
            ToggleState = IngamePlayerSettings.Instance.settings.micEnabled;

            // Change sprite
            HUDManager.Instance.PTTIcon.sprite = Speaker_Icons[0];
        }

        private void Update()
        {
            // If its push to talk. Just fucking skip.
            if (IngamePlayerSettings.Instance.settings.pushToTalk) return;

            // Handle muting before we continue
            HandleToggleMute();

            // Enable the PTT icon cause it forces it to be false.
            HUDManager.Instance.PTTIcon.enabled = true;

            // If (the mic is disabled) OR (module isnt present) return;
            if (!IngamePlayerSettings.Instance.settings.micEnabled || !StartOfRound.Instance?.voiceChatModule) return;

            // Make it fully visable
            HUDManager.Instance.PTTIcon.color = new Color(255f, 255f, 255, 0.8f);

            // Get the local player
            VoicePlayerState player = StartOfRound.Instance.voiceChatModule.FindPlayer(StartOfRound.Instance.voiceChatModule.LocalPlayerName);

            if (player.IsSpeaking) // If the player is speaking
                if (Mathf.Clamp(player.Amplitude * 35f, 0.01f, 1f) <= 0.19f) // If the player is quiet
                    HUDManager.Instance.PTTIcon.sprite = Speaker_Icons[2];
                else // If the player is loud
                    HUDManager.Instance.PTTIcon.sprite = Speaker_Icons[3];
            else // If the player isnt speaking
                HUDManager.Instance.PTTIcon.sprite = Speaker_Icons[1];
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        // Calls for logging
        private void OnEnable()
        {
            // If the mic was muted when left off
            if (!IngamePlayerSettings.Instance.settings.micEnabled)
                HUDManager.Instance.PTTIcon.sprite = Main.bundle.LoadAsset<Sprite>("assets/LS/Sprites/Speaker_Muted.png");

            base.Enabled();
        }
    }
}