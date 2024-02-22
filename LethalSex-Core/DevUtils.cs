using GameNetcodeStuff;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LethalSex_Core
{
    public class ConsoleManager : LethalClass
    {
        /// <summary>
        /// Load console object if not already loaded into scene
        /// </summary>
        public override void HUD_Awake()
        {
            // If console already exists or ToggleDebugConsole is disabled, exit.
            if (ConsoleObject || !Config.ToggleDebugConsole) return;

            // Load prefab and instantiate it
            ConsoleObject = Instantiate(Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/console/ls-console.prefab"), GameObject.Find("Systems/UI/Canvas/").transform);

            // Get TMProUGUI
            ConsoleText = ConsoleObject.transform.Find("ConsoleArea/Viewport/ConsoleText").GetComponent<TextMeshProUGUI>();

            // Get ScrollRect
            ConsoleScroll = ConsoleObject.transform.Find("ConsoleArea").GetComponent<ScrollRect>();

            // Update text
            if (ConsoleText && ConsoleScroll) ConsoleText.text = _consoleTextBuffer.ToString();

            // Add ConsoleManager component
            instance = ConsoleObject.AddComponent<ConsoleManager>();
        }

        /// <summary>
        /// Check if the player presses f10, toggle the inverse active state of the console object
        /// </summary>
        private void Update()
        {
            if (Keyboard.current.f10Key.wasPressedThisFrame)
                ConsoleObject?.transform.GetChild(0).gameObject.SetActive(!ConsoleObject.transform.GetChild(0).gameObject.activeInHierarchy);
        }

        private void OnEnable() => base.Enabled();

        private void OnDisable() => base.Disabled();

        private void OnDestroy() => base.Destroyed();

        public static ConsoleManager instance { get; private set; }

        private static StringBuilder _consoleTextBuffer = new StringBuilder();
        public static TextMeshProUGUI ConsoleText { get; set; }
        public static GameObject ConsoleObject { get; set; }
        public static ScrollRect ConsoleScroll { get; set; }

        /// <summary>
        /// Wraps text in hex color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hexCode"></param>
        /// <returns>String wrapped in color</returns>
        private static string WIC(string text, string hexCode) => $"<color={hexCode}>{text}</color>";

        /// <summary>
        /// Color to hex value
        /// </summary>
        /// <param name="baseColor"></param>
        /// <returns>Returns the hex value of a color</returns>
        private static string CTH(Color baseColor)
        {
            return "#" + Convert.ToInt32(baseColor.r * 255f).ToString("X2") + Convert.ToInt32(baseColor.g * 255f).ToString("X2") + Convert.ToInt32(baseColor.b * 255f).ToString("X2");
        }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(object msg) => Log(msg, "Info", Color.cyan);

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void Log(object msg, Color color) => Log(msg, "Info", color);

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prefix"></param>
        public static void Log(object msg, object prefix) => Log(msg, prefix, Color.cyan);

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prefix"></param>
        /// <param name="color"></param>
        public static void Log(object msg, object prefix, Color color)
        {
            _consoleTextBuffer.Append($"[{DateTimeOffset.Now.ToString("hh:mm:ss:ff")}] [{WIC($"{prefix}", CTH(color))}] ~> {msg}\n");
            if (ConsoleText && ConsoleScroll)
                ConsoleText.text = _consoleTextBuffer.ToString();
            else if (!ConsoleText) Main.mls.LogFatal("ConsoleText Is Missing?!");
            else if (!ConsoleScroll) Main.mls.LogFatal("ConsoleScroll Is Missing?!");
            else Main.mls.LogFatal("UHHHHHHHHHHHHHHH!?");
        }
    }

    public class DevMenuManager : LethalClass
    {
        public override void HUD_Awake()
        {
            // If console already exists or ToggleDebugConsole is disabled, exit.
            if (DevMenuObject || !Config.ToggleDevMenu) return;

            // Load prefab and instantiate it
            DevMenuObject = Instantiate(Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/devmenu/ls-devmenu.prefab"), GameObject.Find("Systems/UI/Canvas/").transform);

            // Get content gameObject
            Content = DevMenuObject.transform.Find("Scroll/View/Content").gameObject;

            // SectionPrefab = Content.transform.Find("Position").gameObject;
            SectionPrefab = Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/devmenu/section.prefab");

            //ButtonPrefab = Content.transform.Find("Position/TP_Ship").gameObject;
            ButtonPrefab = Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/devmenu/button.prefab");

            //SliderPrefab = Content.transform.Find("Money/Slider").gameObject;
            SliderPrefab = Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/devmenu/slider.prefab");

            // Add ConsoleManager component
            instance = DevMenuObject.AddComponent<DevMenuManager>();
        }

        private void Update()
        {
            if (Keyboard.current.f11Key.wasPressedThisFrame)
                DevMenuObject?.transform.GetChild(0).gameObject.SetActive(!DevMenuObject.transform.GetChild(0).gameObject.activeInHierarchy);
        }

        private void OnEnable() => base.Enabled();

        private void OnDisable() => base.Disabled();

        private void OnDestroy() => base.Destroyed();

        public static DevMenuManager instance { get; private set; }
        private static GameObject SectionPrefab { get; set; }
        private static GameObject ButtonPrefab { get; set; }
        private static GameObject SliderPrefab { get; set; }
        private static GameObject Content { get; set; }
        private static GameObject DevMenuObject { get; set; }

        public static void SetAction(Button btn, Action action)
        {
            btn.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            if (action != null) btn.GetComponent<Button>().onClick.AddListener(new UnityAction(action));
        }

        public static void SetAction(Slider slider, Action<float> action, Transform PercentLabel)
        {
            slider.onValueChanged = new Slider.SliderEvent();
            slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(action));
            slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>((float val) =>
            {
                PercentLabel.GetComponent<TextMeshProUGUI>().text = (val / slider.maxValue * 100).ToString("F2") + "%";
            }));
        }

        public class DMSection
        {
            protected GameObject Section { get; private set; }
            protected GameObject Name { get; private set; }
            protected GameObject Info { get; private set; }
            protected GameObject TopSect { get; private set; }
            protected GameObject BottonSect { get; private set; }

            public DMSection(object SectionName)
            {
                // Create new section
                Section = Instantiate(SectionPrefab, Content.transform);
                TopSect = Section.transform.Find("Top Sect").gameObject;
                BottonSect = Section.transform.Find("Bottom Sect").gameObject;
                Name = TopSect.transform.Find("Name").gameObject;
                Info = TopSect.transform.Find("Info").gameObject;

                // Rename gameObject with hash
                var hash = new Hash128();
                hash.Append(SectionName.ToString());
                Section.name = $"{SectionName}-{hash}";

                // Set name
                Name.GetComponent<TextMeshProUGUI>().text = SectionName.ToString();
            }

            public GameObject gameObject => Section;
            public GameObject name => Name;
            public GameObject info => Info;
            public GameObject topsect => TopSect;
            public GameObject bottonsect => BottonSect;
        }

        public class DMButton
        {
            protected static GameObject _Button { get; private set; }
            protected static GameObject _Label { get; private set; }

            public DMButton(DMSection section, Action action, object ButtonName)
            {
                // Create new button
                _Button = Instantiate(ButtonPrefab, section.bottonsect.transform);

                // Rename gameObject with hash
                var hash = new Hash128();
                hash.Append(ButtonName.ToString());
                _Button.name = $"{ButtonName}-{hash}";
                _Button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = ButtonName.ToString();

                // Set button action
                SetAction(_Button.GetComponent<Button>(), action);
            }

            public void DisableLabel() => _Label.SetActive(false);

            public void SetLabel(object str) => _Label.GetComponent<TextMeshProUGUI>().text = $"{str}";

            public GameObject Button => _Button;

            public GameObject Label => Label;
        }

        public class DMSlider
        {
            protected GameObject slider { get; private set; }
            protected Slider slider_comp { get; private set; }
            protected static TextMeshProUGUI Label { get; private set; }

            public DMSlider(DMSection section, Action<float> action, float min, float max, bool whole, object SliderName, float defaultValue = 0)
            {
                if (min > max) throw new Exception("Min cannot be larger than Max");

                // Create new button
                slider = Instantiate(SliderPrefab, section.bottonsect.transform);
                slider_comp = slider.GetComponentInChildren<Slider>();

                // Set slider values
                slider_comp.minValue = defaultValue;
                slider_comp.value = defaultValue;
                slider_comp.maxValue = max;
                slider_comp.wholeNumbers = whole;

                // Rename gameObject with hash
                var hash = new Hash128();
                hash.Append(SliderName.ToString());
                slider.name = $"{SliderName}-{hash}";

                // Set info name
                slider.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = SliderName.ToString();

                // Set percentage
                slider.transform.Find("Percentage").GetComponent<TextMeshProUGUI>().text = "0%";

                // Set slider action
                SetAction(slider_comp, action, slider.transform.Find("Percentage")); ;
            }
        }
    }
}