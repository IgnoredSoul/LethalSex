using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalSex.Modules
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class DevUtils
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update()
        {
            if (Keyboard.current.numpad9Key.isPressed)
            {
                Extensions.GetLocalPlayer().transform.position = new Vector3(-110.6346f, 2.9663f, -18.0714f);
                LogHandler.Warn("Teleporting player");
            }

            if(Keyboard.current.numpad1Key.isPressed)
            {
                Extensions.GetLocalPlayerController().insanityLevel = 34f;
            }
        }
    }
}
