using System;
using System.Text;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace LethalSex_Core.Modules
{
    public class ConsoleManager : LethalModule
    {
        public override void OnRegister()
        {
            if (!Config.ToggleDebugConsole) Mod.UnregisterModule(this);
            Module = this;
        }

        /// <summary>
        /// Load console object if not already loaded into scene
        /// </summary>
        public override void OnHUDAwake()
        {
            // If console already exists
            if (ConsoleObject) return;

            // Load prefab and instantiate it
            ConsoleObject = Instantiate(Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/console/ls-console.prefab"), GameObject.Find("Systems/UI/Canvas/").transform);

            // Get TMProUGUI
            ConsoleText = ConsoleObject.transform.Find("ConsoleArea/Viewport/ConsoleText").GetComponent<TextMeshProUGUI>();

            // Get ScrollRect
            ConsoleScroll = ConsoleObject.transform.Find("ConsoleArea").GetComponent<ScrollRect>();

            // Update text
            if (ConsoleText && ConsoleScroll) ConsoleText.text = _consoleTextBuffer.ToString();

            // Add ConsoleManager component
            component = ConsoleObject.AddComponent<ConsoleManager>();

            instance = this;
        }

        /// <summary>
        /// Check if the player presses f10, toggle the inverse active state of the console object
        /// </summary>
        private void Update()
        {
            if (Keyboard.current.f10Key.wasPressedThisFrame && ConsoleObject)
                ConsoleObject?.transform.GetChild(0).gameObject.SetActive(!ConsoleObject.transform.GetChild(0).gameObject.activeInHierarchy);

            if (Keyboard.current.spaceKey.IsPressed() && ConsoleScroll)
                ConsoleScroll.verticalNormalizedPosition = 0;
        }

        private void OnEnable() => base.Enabled();

        private void OnDisable() => base.Disabled();

        private void OnDestroy() => base.Destroyed();

        public static ConsoleManager instance { get; private set; }
        public static ConsoleManager component { get; private set; }

        private static StringBuilder _consoleTextBuffer = new StringBuilder();
        public static TextMeshProUGUI ConsoleText { get; private set; }
        public static GameObject ConsoleObject { get; private set; }
        public static ScrollRect ConsoleScroll { get; private set; }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(object msg) => Log(msg, "Info", Color.cyan);

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg) => Log(msg, "Error", Color.red);

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(object msg) => Log(msg, "Warn", Color.yellow);

        /// <summary>
        /// Log information with specified color
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void Log(object msg, Color color) => Log(msg, "Info", color);

        /// <summary>
        /// Log information with specified prefix
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prefix"></param>
        public static void Log(object msg, object prefix) => Log(msg, prefix, Color.cyan);

        /// <summary>
        /// Log with specified prefix and color
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prefix"></param>
        /// <param name="color"></param>
        public static void Log(object msg, object prefix, Color color)
        {
            _consoleTextBuffer.Append($"[{DateTimeOffset.Now:hh:mm:ss:ff}] [{Extensions.ColorWrap($"{prefix}", Extensions.HexColor(color))}] ~> {msg}\n");
            if (ConsoleObject)
            {
                if (ConsoleText && ConsoleScroll)
                    ConsoleText.text = _consoleTextBuffer.ToString();
            }
        }
    }
}