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
        /// Define global 'Module'
        /// </summary>
        public static ConsoleManager Module { get; private set; }

        /// <summary>
        /// When the 'Module' is registered, check the config and see if the module should be disabled
        /// </summary>
        protected override void OnRegister()
        {
            if (!Config.ToggleDebugConsole) Unregister();
        }

        /// <summary>
        /// Initialize the console gameObject into the scene
        /// </summary>
        protected override void OnHUDAwake()
        {
            // If console already exists
            if (ConsoleObject) return;

            // Load prefab and instantiate it
            ConsoleObject = Instantiate(Main.bundle.LoadAsset<GameObject>("assets/lethalsex-core/console/ls-console.prefab"), GameObject.Find("Systems/UI/Canvas/").transform);

            // Get TMProUGUI
            ConsoleText = ConsoleObject.transform.Find("ConsoleArea/Viewport/ConsoleText").GetComponent<TextMeshProUGUI>();

            // Get ScrollRect
            ConsoleScroll = ConsoleObject.transform.Find("ConsoleArea").GetComponent<ScrollRect>();

            // Change version text
            ConsoleObject.transform.Find("ConsoleArea/ConsoleBackground/GameVersion").GetComponent<TextMeshProUGUI>().text = $"v{Main.modVersion}";

            // Update text
            if (ConsoleText && ConsoleScroll) ConsoleText.text = _consoleTextBuffer.ToString();

            // Add ConsoleManager component
            Module = ConsoleObject.AddComponent<ConsoleManager>();
        }

        /// <summary>
        /// After each frame update, check if the user presses the F10 key to show / hide the console
        /// </summary>
        private void LateUpdate()
        {
            if (Keyboard.current.f10Key.wasPressedThisFrame && ConsoleObject)
                ConsoleObject?.transform.GetChild(0).gameObject.SetActive(!ConsoleObject.transform.GetChild(0).gameObject.activeInHierarchy);
        }

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnEnable() => base.Enable();

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnDisable() => base.Disable();

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnDestroy() => base.Destroy();

        /// <summary>
        /// String buffer for the console text
        /// </summary>
        private static StringBuilder _consoleTextBuffer = new StringBuilder();

        /// <summary>
        /// Reference for the console text
        /// </summary>
        public static TextMeshProUGUI ConsoleText { get; set; }

        /// <summary>
        /// Reference to the gameObject
        /// </summary>
        public static GameObject ConsoleObject { get; set; }

        /// <summary>
        /// Reference for the scrollrect
        /// </summary>
        public static ScrollRect ConsoleScroll { get; set; }

        /// <summary>
        /// Wraps text in hex color for inline coloring.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hexCode"></param>
        /// <returns>String wrapped in inline hex color</returns>
        public static string WIC(string text, string hexCode) => $"<color={hexCode}>{text}</color>";

        /// <summary>
        /// UnityEngine / System Color to hex value
        /// </summary>
        /// <param name="baseColor"></param>
        /// <returns>Returns the hex value of a color</returns>
        public static string CTH(Color baseColor)
        {
            // Broken into part for easier reading
            string r = Convert.ToInt32(baseColor.r * 255f).ToString("X2");
            string g = Convert.ToInt32(baseColor.g * 255f).ToString("X2");
            string b = Convert.ToInt32(baseColor.b * 255f).ToString("X2");

            return $"#{r}{g}{b}";
        }

        /// <summary>
        /// Log string</br>
        /// <i>With info tag</i>
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(object msg) => Log(msg, "Info", Color.cyan);

        /// <summary>
        /// Log string</br>
        /// <i>With warn tag</i>
        /// </summary>
        /// <param name="msg"></param>
        public static void LogWrn(object msg) => Log(msg, "Warn", Color.yellow);

        /// <summary>
        /// Log string</br>
        /// <i>With info err</i>
        /// </summary>
        /// <param name="msg"></param>
        public static void LogErr(object msg) => Log(msg, "Err", Color.red);

        /// <summary>
        /// Log string</br>
        /// <i>With msg tag</i>
        /// </summary>
        /// <param name="msg"></param>
        public static void LogMsg(object msg) => Log(msg, "Msg", Color.blue);

        /// <summary>
        /// Log string with color
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void Log(object msg, Color color) => Log(msg, "Info", color);

        /// <summary>
        /// Log string with color and defined action / prefix </br>
        /// An example use of the prefix is stating errors, warning, info and messages
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
            // Append text to buffer
            _consoleTextBuffer.Append($"[{DateTimeOffset.Now.ToString("hh:mm:ss:ff")}] [{WIC($"{prefix}", CTH(color))}] ~> {msg}\n");

            // Making sure everything is present
            if (ConsoleObject && ConsoleText && ConsoleScroll)
                ConsoleText.text = _consoleTextBuffer.ToString();
        }
    }

    public class DevMenuManager : LethalClass
    {
        /// <summary>
        /// Global Module
        /// </summary>
        public static DevMenuManager Module { get; private set; }

        /// <summary>
        /// When the 'Module' is registered, check the config and see if the module should be disabled
        /// </summary>
        protected override void OnRegister()
        {
            if (!Config.ToggleDevMenu) Unregister();
        }

        /// <summary>
        /// Initialize the devmenu gameObject into the scene
        /// </summary>
        protected override void OnHUDAwake()
        {
            // If console already exists or ToggleDebugConsole is disabled, exit.
            if (DevMenuObject) return;

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
            Module = DevMenuObject.AddComponent<DevMenuManager>();
        }

        /// <summary>
        /// After each frame update, check if the user presses the F11 key to show / hide the devmenu
        /// </summary>
        private void LateUpdate()
        {
            if (Keyboard.current.f11Key.wasPressedThisFrame)
                DevMenuObject?.transform.GetChild(0).gameObject.SetActive(!DevMenuObject.transform.GetChild(0).gameObject.activeInHierarchy);
        }

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnEnable() => base.Enable();

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnDisable() => base.Disable();

        /// <summary>
        /// MODULE LOGGING [DEFAULT]
        /// </summary>
        private void OnDestroy() => base.Destroy();

        /// <summary>
        /// Prefab for instantiating new sections
        /// </summary>
        private static GameObject SectionPrefab { get; set; }

        /// <summary>
        /// Prefab for instantiating new buttons
        /// </summary>
        private static GameObject ButtonPrefab { get; set; }

        /// <summary>
        /// Prefab for instantiating new sliders
        /// </summary>
        private static GameObject SliderPrefab { get; set; }

        /// <summary>
        /// Refereance to the content area
        /// </summary>
        private static GameObject Content { get; set; }

        /// <summary>
        /// Reference to the gameObject
        /// </summary>
        private static GameObject DevMenuObject { get; set; }

        /// <summary>
        /// Creates a new section
        /// </summary>
        public class DMSection
        {
            /// <summary>
            /// Gets the GameObject representing the entire section.
            /// </summary>
            protected GameObject Section { get; private set; }

            /// <summary>
            /// Gets the GameObject representing the name plate within the section.
            /// </summary>
            protected TextMeshProUGUI NamePlate { get; private set; }

            /// <summary>
            /// Gets the GameObject representing the info plate within the section.
            /// </summary>
            protected TextMeshProUGUI InfoPlate { get; private set; }

            /// <summary>
            /// Gets the GameObject representing the top section of the section.
            /// </summary>
            protected GameObject TopSection { get; private set; }

            /// <summary>
            /// Gets the GameObject representing the bottom section of the section.
            /// </summary>
            protected GameObject BottomSection { get; private set; }

            /// <summary>
            /// Constructs a new DMSection instance.
            /// </summary>
            /// <param name="sectionName">The name or label for the section.</param>
            public DMSection(object sectionName, object name = null, object info = null)
            {
                // Create new section and retrieve references to its components.
                Section = Instantiate(SectionPrefab, Content.transform);

                // Save top section gameObject
                TopSection = Section.transform.Find("Top Sect").gameObject;

                // Save bottom section gameObject
                BottomSection = Section.transform.Find("Bottom Sect").gameObject;

                // Save name TMPUGUI -> Set text
                (NamePlate = TopSection.transform.Find("Name").GetComponent<TextMeshProUGUI>()).text = name.ToString();

                // Save info TMPUGUI -> Set text
                (InfoPlate = TopSection.transform.Find("Info").GetComponent<TextMeshProUGUI>()).text = info.ToString();

                // Rename section GameObject with hash and provided name.
                Section.name = $"{sectionName}-{NumberUtils.String128(sectionName)}";
            }

            /// <summary>
            /// Gets the GameObject representing the entire section.
            /// </summary>
            public GameObject section => Section;

            /// <summary>
            /// Gets the GameObject representing the name plate within the section.
            /// </summary>
            public TextMeshProUGUI name => NamePlate;

            /// <summary>
            /// Gets the GameObject representing the info plate within the section.
            /// </summary>
            public TextMeshProUGUI info => InfoPlate;

            /// <summary>
            /// Gets the GameObject representing the top section of the section.
            /// </summary>
            public GameObject topsection => TopSection;

            /// <summary>
            /// Gets the GameObject representing the bottom section of the section.
            /// </summary>
            public GameObject bottomsection => BottomSection;
        }

        /// <summary>
        /// Creates a new button for a section
        /// </summary>
        public class DMButton
        {
            /// <summary>
            /// Reference to the Button component of the DMButton.
            /// </summary>
            private Button button;

            /// <summary>
            /// Reference to the TextMeshProUGUI component used for labeling the button.
            /// </summary>
            private TextMeshProUGUI label;

            /// <summary>
            /// Constructs a new DMButton instance.
            /// </summary>
            /// <param name="section">The DMSection to which the button belongs.</param>
            /// <param name="action">The action to be performed when the button is clicked.</param>
            /// <param name="buttonName">The name or label for the button.</param>
            public DMButton(DMSection section, Action action, object buttonName)
            {
                // Instantiate new button and save Button component reference.
                button = Instantiate(ButtonPrefab, section.bottomsection.transform).GetComponent<Button>();

                // Rename button GameObject with hash and provided button name.
                button.name = $"{buttonName}-{NumberUtils.String128(buttonName)}";

                // Save TextMeshProUGUI component reference for the button label and set its text.
                (label = button.transform.Find("Text").GetComponent<TextMeshProUGUI>()).text = buttonName.ToString();

                // Set button action.
                SetAction(button, action);
            }

            /// <summary>
            /// Disables the label associated with the button, if any.
            /// </summary>
            public void DisableLabel()
            {
                if (label != null)
                    label.gameObject.SetActive(false);
            }

            /// <summary>
            /// Sets the text of the label associated with the button.
            /// </summary>
            /// <param name="str">The text to set for the label.</param>
            public void SetLabel(object str) => label.text = str.ToString();

            /// <summary>
            /// Private method to set the action to be performed when the button is clicked.
            /// </summary>
            /// <param name="btn"></param>
            /// <param name="action"></param>
            private void SetAction(Button btn, Action action) => btn.onClick.AddListener(() => action());

            /// <summary>
            /// Gets the Button component of the DMButton.
            /// </summary>
            public Button Button => button;

            /// <summary>
            /// Gets the TextMeshProUGUI component used for labeling the button.
            /// </summary>
            public TextMeshProUGUI Label => label;
        }

        /// <summary>
        /// Creates a new slider for a section
        /// </summary>
        public class DMSlider
        {
            /// <summary>
            /// Gets the Slider component of the DMSlider.
            /// </summary>
            protected Slider Slider { get; private set; }

            /// <summary>
            /// Gets the TextMeshProUGUI component used for displaying additional information about the slider.
            /// </summary>
            protected TextMeshProUGUI Info { get; private set; }

            /// <summary>
            /// Gets the TextMeshProUGUI component used for displaying the percentage value of the slider.
            /// </summary>
            protected TextMeshProUGUI Percent { get; private set; }

            /// <summary>
            /// Constructs a new DMSlider instance.
            /// </summary>
            /// <param name="section">The DMSection to which the slider belongs.</param>
            /// <param name="action">The action to be performed when the slider value changes.</param>
            /// <param name="min">The minimum value of the slider.</param>
            /// <param name="max">The maximum value of the slider.</param>
            /// <param name="whole">Specifies whether the slider should only accept whole numbers.</param>
            /// <param name="sliderName">The name or label for the slider.</param>
            /// <param name="defaultValue">The default value for the slider (optional, default is 0).</param>
            /// <exception cref="Exception">Thrown when the minimum value is larger than the maximum value.</exception>
            public DMSlider(DMSection section, Action<float> action, float min, float max, bool whole, object sliderName, float defaultValue = 0)
            {
                if (min > max) throw new Exception("Min cannot be larger than Max");

                // Instantiate new slider and save Slider component reference.
                Slider = Instantiate(SliderPrefab, section.bottomsection.transform).GetComponent<Slider>();

                // Set slider values.
                Slider.minValue = defaultValue;
                Slider.value = defaultValue;
                Slider.maxValue = max;
                Slider.wholeNumbers = whole;

                // Rename slider GameObject with hash and provided slider name.
                Slider.name = $"{sliderName}-{NumberUtils.String128(sliderName.ToString())}";

                // Set info name and save TextMeshProUGUI component reference for additional information display.
                (Info = Slider.transform.Find("Info").GetComponent<TextMeshProUGUI>()).text = sliderName.ToString();

                // Save TextMeshProUGUI component reference for percentage display.
                Percent = Slider.transform.Find("Percentage").GetComponent<TextMeshProUGUI>();

                // Set slider action.
                SetAction(action);
            }

            /// <summary>
            /// Sets the action to be performed when the slider value changes.
            /// </summary>
            /// <param name="action">The action to be performed.</param>
            private void SetAction(Action<float> action)
            {
                Slider.onValueChanged = new Slider.SliderEvent();
                Slider.onValueChanged.AddListener(new UnityAction<float>(action));
                Slider.onValueChanged.AddListener(new UnityAction<float>((float val) => Percent.text = (val / Slider.maxValue * 100).ToString("F2") + "%"));
            }

            /// <summary>
            /// Gets the Slider component of the DMSlider.
            /// </summary>
            public Slider GetSlider() => Slider;

            /// <summary>
            /// Gets the TextMeshProUGUI component used for displaying additional information about the slider.
            /// </summary>
            public TextMeshProUGUI GetInfo() => Info;

            /// <summary>
            /// Gets the TextMeshProUGUI component used for displaying the percentage value of the slider.
            /// </summary>
            public TextMeshProUGUI GetPercent() => Percent;
        }
    }
}