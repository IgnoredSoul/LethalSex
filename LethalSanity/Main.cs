using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using LethalSanity.Modules;
using LethalSex_Core;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace LethalSanity
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Lethal Company.exe")]
    public class Main : BaseUnityPlugin
    {
        internal readonly Harmony harmony = new Harmony(modGUID);
        private const string modGUID = "LethalSanity";
        private const string modName = "LethalSanity";
        private const string modVersion = "0.1.2";

        internal static Main instance { get; private set; }
        internal static AssetBundle bundle { get; private set; }

        private void Awake()
        {
            // Load bundle
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.lethalsanity);

            instance = this; /* Set instance to this */
        }
    }

    /// <summary>
    /// Temp - Easier to see my sanity for testing
    /// </summary>
    public class FPSHUD : LethalClass
    {
        public override void LocalPlayer_Start(PlayerControllerB LocalPlayer) => Instance = LocalPlayer.gameObject.GetOrAddComponent<FPSHUD>();

        private TextMeshProUGUI FPSMPro { get; set; }
        private GameObject FPSObj { get; set; }
        public static FPSHUD Instance { get; private set; }

        private async void Awake()
        {
            // Wait till WeightUI exists and clone it.
            do
            {
                FPSObj = Instantiate(
                    GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/WeightUI"),
                    GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/WeightUI").transform.parent
                );

                await Task.Delay(500);
            } while (!FPSObj);

            // Set objects name
            FPSObj.name = "LS-FPSUI";

            // Set the position to be above the weight
            FPSObj.transform.localPosition += new Vector3(0, -15, 0);

            // Assign internal ref to this component
            FPSMPro = FPSObj.GetComponentInChildren<TextMeshProUGUI>();

            // Set objects name
            FPSObj.GetComponentInChildren<TextMeshProUGUI>().gameObject.name = "LS-FPS";
        }

        private void Update()
        {
            if (FPSMPro)
                FPSMPro.text = $"{LocalPlayer.Insanity} •";
        }

        private void OnDestroy() => base.Destroyed();

        private void OnDisable() => base.Disabled();

        private void OnEnable() => base.Enabled();
    }
}