using System;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LethalSex.Modules
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class AudioHandler
    {
        internal static GameObject AudioObject { get; private set; }
        internal static AudioSource AudioSource { get; private set; }
        internal static float TimeSinceLastPlayed => (Time.time - timePlayed);
        private static float timePlayed { get; set; }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start()
        {
            Task.Run(async () =>
            {
                while (!Extensions.GetLocalPlayer()) await Task.Delay(250);

                //Copy MovementAudio and place the object under the HangarShip
                AudioObject = Extensions.EasyInstantiate(Extensions.GetLocalPlayer()?.transform.Find("Audios/MovementAudio")?.gameObject, GameObject.Find("Environment/HangarShip"));

                // Rename the object
                AudioObject.name = "LS_AudioObject";
                LogHandler.Msg(LogHandler.MessageColors.DarkCyan, "Created new audio object");

                // Get the audio source
                AudioSource = AudioObject.GetComponent<AudioSource>();
                LogHandler.Msg(LogHandler.MessageColors.DarkCyan, "Obtained audio source");

                AudioSource.spatialize = true;
                LogHandler.Msg(LogHandler.MessageColors.DarkCyan, "Audio Source finished settping up");

                Setup_Sounds();
                LogHandler.Msg(LogHandler.MessageColors.DarkCyan, "Audio dictionary finished settping up");
            });
        }

        internal static Dictionary<string, AudioData> Knocking = new Dictionary<string, AudioData>();
        internal static Dictionary<string, AudioData> Ambience = new Dictionary<string, AudioData>();
        internal static Dictionary<string, AudioData> Laughing = new Dictionary<string, AudioData>();
        internal static Dictionary<string, AudioData> Enemies  = new Dictionary<string, AudioData>();
        private static void Setup_Sounds()
        {
            // Clear lists just in case
            Knocking.Clear(); Ambience.Clear(); Laughing.Clear();

            // Knocking
            Knocking.Add("Knocking_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Knocking_1.wav"), 25, 30, 0.2f, 0.3f, 0.95f, 1.25f));
            Knocking.Add("Knocking_2", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Knocking_2.wav"), 25, 30, 0.2f, 0.3f, 0.95f, 1.25f));

            // Laughing
            Laughing.Add("GirlChuckle", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/GirlChuckle.wav"), 15, 20, 0.16f, 0.2f, 0.95f, 1.05f));
            Laughing.Add("DemonLaugh", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/DemonLaugh.wav"), 16, 20, 0.26f, 0.38f, 0.95f, 1f));
            Laughing.Add("TunnelLaugh", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/TunnelLaugh.wav"), 20, 30, 0.47f, 0.55f, 0.95f, 1f));
            Laughing.Add("Laugh_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Laugh_2.ogg"), 20, 30, 0.12f, 0.22f, 0.75f, 1.15f));
            Laughing.Add("Laugh_2", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Laugh_3.ogg"), 20, 30, 0.12f, 0.22f, 0.75f, 1.15f));

            // Ambience
            Ambience.Add("DoorsPsst", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/DoorsPsst.wav"), 1, 5, 0.45f, 0.55f, 0.85f, 1.05f));
            Ambience.Add("GhostSigh", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/GhostSigh.wav"), 6, 10, 0.18f, 0.35f, 0.95f, 1.25f));
            //Sounds.Add("GirlHello", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/GirlHello.wav"), 8, 10, 0.12f, 0.22f, 0.75f, 1.15f));
            Ambience.Add("BumpOutsideShip_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/BumpOutsideShip_1.ogg"), 8, 10, 0.12f, 0.22f, 0.75f, 1.15f));
            Ambience.Add("Creak_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Creak_1.ogg"), 20, 26, 0.12f, 0.22f, 0.75f, 1.15f));
            Ambience.Add("Creak_2", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Creak_2.ogg"), 20, 26, 0.12f, 0.22f, 0.75f, 1.15f));
            Ambience.Add("DoorSlam_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/DoorSlam_1.ogg"), 10, 30, 0.52f, 0.82f, 0.95f, 1.05f));
            Ambience.Add("DoorSlam_2", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/DoorSlam_2.ogg"), 10, 30, 0.52f, 0.82f, 0.95f, 1.05f));
            Ambience.Add("Mic_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Mic_1.ogg"), 30, 35, 0.12f, 0.22f, 0.95f, 1.05f));
            Ambience.Add("Mic_2", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/Mic_2.ogg"), 30, 35, 0.12f, 0.22f, 0.95f, 1.05f));
            Ambience.Add("WarningHUD_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/WarningHUD_1.ogg"), 20, 26, 0.42f, 0.72f, 0.95f, 1.05f));

            // Enemies
            Enemies.Add("BurrowingGrowl_1", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/BurrowingGrowl_1.ogg"), 20, 30, 0.12f, 0.22f, 0.75f, 1.15f));
            Enemies.Add("CentipedeWalk", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/CentipedeWalk.ogg"), 10, 30, 0.12f, 0.22f, 0.75f, 1.15f));
            Enemies.Add("TurretFireDistance", new AudioData(Main.bundle.LoadAsset<AudioClip>("assets/LS/Sounds/TurretFireDistance.ogg"), 15, 20, 0.12f, 0.22f, 0.75f, 1.15f));
        }

        [HarmonyPatch("OnDestroy")]
        [HarmonyPostfix]
        private static void OnDestroy_AudioHandler()
        {
            LogHandler.Msg(LogHandler.MessageColors.BrightMagenta, "Audio Object has been destroyed");
        }

        internal class AudioData
        {
            // Clip data
            AudioClip clip;

            // Volume data
            float minVol;
            float maxVol;

            // Pitch data
            float minPitch;
            float maxPitch;

            // Range data
            public float minRange;
            public float maxRange;

            // Constructor for making new skitz data
            public AudioData(AudioClip clip, float minRange, float maxRange, float minVol, float maxVol, float minPitch, float maxPitch)
            {
                this.clip = clip;

                this.minRange = minRange;
                this.maxRange = maxRange;

                this.minVol = minVol;
                this.maxVol = maxVol;

                this.minPitch = minPitch;
                this.maxPitch = maxPitch;
            }

            /// <summary>
            /// Plays a sound at a random point around the player.
            /// </summary>
            /// <param name="audioSource"></param>
            /// <param name="startPos"></param>
            public void PlaySound(AudioSource audioSource, Vector3 PlayerPos)
            {
                try
                {
                    // If its already playing, return;
                    if (audioSource == null || audioSource.isPlaying) return;

                    /// Randomize position
                    // Calculate a random angle within the circle range
                    float randomAngle = NumberUtils.Next(0f, 360f);
                    float radians = Mathf.Deg2Rad * randomAngle;

                    float randomPoint = NumberUtils.Next(minRange, maxRange);

                    // Calculate the position based on the random angle
                    float x = randomPoint * Mathf.Cos(radians);
                    float z = randomPoint * Mathf.Sin(radians);

                    // Reset position and set offsets for spatial
                    audioSource.transform.position = PlayerPos;
                    audioSource.transform.position += new Vector3(x, 0, z);

                    // Set clip
                    audioSource.clip = clip;

                    // Set random pitch
                    audioSource.pitch = NumberUtils.Next(minPitch, maxPitch);

                    // Set random volume
                    audioSource.volume = NumberUtils.Next(minVol, maxVol);

                    // Play
                    audioSource.Play();

                    LogHandler.Msg(LogHandler.MessageColors.BrightGreen, $"Clip: {clip.name} | Pos: {x}, 0, {z} | Range: {minRange}, {maxRange} | Pitch: {audioSource.pitch} | Vol: {audioSource.volume}");

                    timePlayed = Time.time;

                }
                catch (Exception e) { LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Tried PlaySound() - {clip.name}:\n{e}"); }
            }

            /// <summary>
            /// Plays a sound at a random point around the player.
            /// </summary>
            /// <param name="audioSource"></param>
            /// <param name="PlayerPos"></param>
            /// <param name="minRange"></param>
            /// <param name="maxRange"></param>
            /// <param name="minVol"></param>
            /// <param name="maxVol"></param>
            /// <param name="minPitch"></param>
            /// <param name="maxPitch"></param>
            public void PlaySound(AudioSource audioSource, Vector3 PlayerPos, float minRange, float maxRange, float minVol, float maxVol, float minPitch, float maxPitch)
            {
                try
                {
                    // If its already playing, return;
                    if (audioSource == null || audioSource.isPlaying) return;

                    /// Randomize position
                    // Calculate a random angle within the circle range
                    float randomAngle = NumberUtils.Next(0f, 360f);
                    float radians = Mathf.Deg2Rad * randomAngle;

                    float randomPoint = NumberUtils.Next(minRange, maxRange);

                    // Calculate the position based on the random angle
                    float x = randomPoint * Mathf.Cos(radians);
                    float z = randomPoint * Mathf.Sin(radians);

                    // Reset position and set offsets for spatial
                    audioSource.transform.position = PlayerPos;
                    audioSource.transform.position += new Vector3(x, 0, z);

                    // Set clip
                    audioSource.clip = clip;

                    // Set random pitch
                    audioSource.pitch = NumberUtils.Next(minPitch, maxPitch);

                    // Set random volume
                    audioSource.volume = NumberUtils.Next(minVol, maxVol);

                    // Play
                    audioSource.Play();

                    LogHandler.Msg(LogHandler.MessageColors.BrightGreen, $"Clip: {clip.name} | Pos: {x}, 0, {z} | Range: {minRange}, {maxRange} | Pitch: {audioSource.pitch} | Vol: {audioSource.volume}");

                    timePlayed = Time.time;

                }
                catch (Exception e) { LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Tried PlaySound() - {clip.name}:\n{e}"); }
            }

            /// <summary>
            /// Plays a sound at a random point around the player.
            /// </summary>
            /// <param name="audioSource"></param>
            /// <param name="PlayerPos"></param>
            /// <param name="range"></param>
            /// <param name="vol"></param>
            /// <param name="pitch"></param>
            public void PlaySound(AudioSource audioSource, Vector3 PlayerPos, float range, float vol, float pitch)
            {
                try
                {
                    // If its already playing, return;
                    if (audioSource == null || audioSource.isPlaying) return;

                    /// Randomize position
                    // Calculate a random angle within the circle range
                    float randomAngle = NumberUtils.Next(0f, 360f);
                    float radians = Mathf.Deg2Rad * randomAngle;

                    float randomPoint = range;

                    // Calculate the position based on the random angle
                    float x = randomPoint * Mathf.Cos(radians);
                    float z = randomPoint * Mathf.Sin(radians);

                    // Reset position and set offsets for spatial
                    audioSource.transform.position = PlayerPos;
                    audioSource.transform.position += new Vector3(x, 0, z);

                    // Set clip
                    audioSource.clip = clip;

                    // Set random pitch
                    audioSource.pitch = pitch;

                    // Set random volume
                    audioSource.volume = vol;

                    // Play
                    audioSource.Play();

                    LogHandler.Msg(LogHandler.MessageColors.BrightGreen, $"Clip: {clip.name} | Pos: {x}, 0, {z} | Range: {range} | Pitch: {pitch} | Vol: {vol}");

                    timePlayed = Time.time;

                }
                catch (Exception e) { LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Tried PlaySound() - {clip.name}:\n{e}"); }
            }
        }
    }

    [HarmonyPatch]
    internal class LethalClass : MonoBehaviour
    {
        private static List<LethalClass> LethalClasses = new List<LethalClass>();
        internal static void Patch()
        {
            // Try getting every thing yk
            try
            {
                LethalClasses = Assembly.GetExecutingAssembly().GetTypes().Where(o => o.IsSubclassOf(typeof(LethalClass))).Select(g => (LethalClass)Activator.CreateInstance(g)).ToList();
            }
            catch (Exception ex) { LogHandler.Err($"Getting every SubClass?;\n{ex}"); }

            // Ehhhh
            LogHandler.Msg($"LethalClasses List Length: {LethalClasses.Count}");
        }
        internal virtual void Enabled()
        {
            LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Behaviour: {GetType().Name} has been enabled");
        }
        internal virtual void Disabled()
        {
            LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Behaviour: {GetType().Name} has been disable");
        }
        internal virtual void Destroyed()
        {
            LogHandler.Msg(LogHandler.MessageColors.BrightRed, $"Behaviour: {GetType().Name} has been removed / destroyed");
        }

        #region ==================[ OVERRIDE VOIDS ]==================

        #region ====================[ HUDManager ]====================

        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        private static void _HUDManager_Start()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.HUDManager_Start();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"HUDManager_Start;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_HUDManager_Start();\n{ex}"); }
        }
        internal virtual void HUDManager_Start() { }

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        private static void _HUDManager_Awake()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.HUDManager_Awake();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"HUDManager_Awake;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_HUDManager_Awake();\n{ex}"); }
        }
        internal virtual void HUDManager_Awake() { }

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void _HUDManager_Update()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.HUDManager_Update();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"HUDManager_Update;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_HUDManager_Update();\n{ex}"); }
        }
        internal virtual void HUDManager_Update() { }

        [HarmonyPatch(typeof(HUDManager), "OnEnable")]
        [HarmonyPostfix]
        private static void _HUDManager_OnEnable()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.HUDManager_OnEnable();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"HUDManager_OnEnable;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_HUDManager_OnEnable();\n{ex}"); }
        }
        internal virtual void HUDManager_OnEnable() { }

        [HarmonyPatch(typeof(HUDManager), "OnDisable")]
        [HarmonyPostfix]
        private static void _HUDManager_OnDisable()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.HUDManager_OnDisable();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"HUDManager_OnDisable;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_HUDManager_OnDisable();\n{ex}"); }
        }
        internal virtual void HUDManager_OnDisable() { }

        #endregion

        #region ================[ StartOfRound Start ]================

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void _StartOfRound_Start()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.StartOfRound_Start();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"StartOfRound_Start;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_StartOfRound_Start();\n{ex}"); }
        }
        internal virtual void StartOfRound_Start() { }

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        private static void _StartOfRound_Awake()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.StartOfRound_Awake();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"StartOfRound_Awake;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_StartOfRound_Awake();\n{ex}"); }
        }
        internal virtual void StartOfRound_Awake() { }

        [HarmonyPatch(typeof(StartOfRound), "Update")]
        [HarmonyPostfix]
        private static void _StartOfRound_Update()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.StartOfRound_Update();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"StartOfRound_Update;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_StartOfRound_Update();\n{ex}"); }
        }
        internal virtual void StartOfRound_Update() { }

        [HarmonyPatch(typeof(StartOfRound), "OnEnable")]
        [HarmonyPostfix]
        private static void _StartOfRound_OnEnable()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.StartOfRound_OnEnable();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"StartOfRound_OnEnable;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_StartOfRound_OnEnable();\n{ex}"); }
        }
        internal virtual void StartOfRound_OnEnable() { }

        [HarmonyPatch(typeof(StartOfRound), "OnDisable")]
        [HarmonyPostfix]
        private static void _StartOfRound_OnDisable()
        {
            try
            {
                foreach (LethalClass patch in LethalClasses)
                {
                    try
                    {
                        patch?.StartOfRound_OnDisable();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.Err($"StartOfRound_OnDisable;\n{ex}");
                    }
                }
            }
            catch (Exception ex) { LogHandler.Err($"_StartOfRound_OnDisable();\n{ex}"); }
        }
        internal virtual void StartOfRound_OnDisable() { }

        #endregion

        #endregion
    }
    internal class NumberUtils
    {
        /// <summary>
        /// Define a new Random
        /// </summary>
        public static readonly System.Random random = new System.Random(GenerateTrulyRandomNumber());

        /// <summary>
        /// Generates a truly random number
        /// </summary>
        /// <returns></returns>
        public static int GenerateTrulyRandomNumber()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4]; // 32 bits
                rng.GetBytes(bytes);

                // Convert the random bytes to an integer and ensure it falls within the specified range
                int randomInt = BitConverter.ToInt32(bytes, 0);
                return Math.Abs(randomInt % (50 - 10)) + 10;
            }
        }

        /// <summary>
        /// Return ueaj
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static bool Chance(int percentage = 50)
        {
            if (percentage < 1 || percentage > 100) return false;
            return (Next(0, 100) < percentage);
        }

        /// <summary>
        /// Generate and returns a random Int64 value
        /// </summary>
        /// <returns></returns>
        public static long GenInt64()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            long randomInt64 = BitConverter.ToInt64(buffer, 0);
            return randomInt64;
        }

        /// <summary>
        /// Returns a number from 0 to max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Next(int max = int.MaxValue) => random.Next(0, (max + 1));

        /// <summary>
        /// Returns a number from min to max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Next(int min = 0, int max = int.MaxValue) => random.Next(min, (max + 1));

        /// <summary>
        /// Returns a number from 0 to max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Next(float max = float.MaxValue) => (float)(NextDouble() * max);

        /// <summary>
        /// Returns a number from min to max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Next(float min = 0f, float max = float.MaxValue) => (float)(NextDouble() * (max - min) + min);

        /// <summary>
        /// Returns a number from 0 to (max - 1)
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int NextL(int max) => random.Next(0, max);

        /// <summary>
        /// Returns a number from min to (max - 1)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int NextL(int min, int max) => random.Next(min, max);

        /// <summary>
        /// Generate and returns a random double value
        /// </summary>
        /// <returns></returns>
        public static double NextDouble() => random.NextDouble();
    }
    internal class LogHandler
    {
        internal static void Msg(object message) => Main.mls.LogInfo(Combiner(MessageColors.White, message));
        internal static void Msg(MessageColors color, object message) => Main.mls.LogInfo(Combiner(color, message));
        internal static void Warn(object message) => Main.mls.LogWarning(Combiner(MessageColors.White, message));
        internal static void Warn(MessageColors color, object message) => Main.mls.LogWarning(Combiner(color, message));
        internal static void Err(object message) => Main.mls.LogError(Combiner(MessageColors.White, message));
        internal static void Err(MessageColors color, object message) => Main.mls.LogError(Combiner(color, message));

        internal enum MessageColors
        {
            White = 1,
            DarkRed = 2,
            DarkGreen = 3,
            DarkYellow = 4,
            DarkBlue = 5,
            DarkMagenta = 6,
            DarkCyan = 7,
            BrightBlack = 8,
            BrightRed = 9,
            BrightGreen = 10,
            BrightYellow = 11,
            BrightBlue = 12,
            BrightMagenta = 13,
            BrightCyan = 14,
            Bold = 15,
            Underline = 16,
            Invert = 17
        }

        protected private static string Combiner(MessageColors Color, object Message)
        {
            string returnString = "";
            switch (Color)
            {
                case MessageColors.White:
                    returnString += ($"[97m{Message}[0m");
                    break;
                case MessageColors.DarkRed:
                    returnString += ($"[31m{Message}[0m");
                    break;
                case MessageColors.DarkGreen:
                    returnString += ($"[32m{Message}[0m");
                    break;
                case MessageColors.DarkYellow:
                    returnString += ($"[33m{Message}[0m");
                    break;
                case MessageColors.DarkBlue:
                    returnString += ($"[34m{Message}[0m");
                    break;
                case MessageColors.DarkMagenta:
                    returnString += ($"[35m{Message}[0m");
                    break;
                case MessageColors.DarkCyan:
                    returnString += ($"[36m{Message}[0m");
                    break;
                case MessageColors.BrightBlack:
                    returnString += ($"[90m{Message}[0m");
                    break;
                case MessageColors.BrightRed:
                    returnString += ($"[91m{Message}[0m");
                    break;
                case MessageColors.BrightGreen:
                    returnString += ($"[92m{Message}[0m");
                    break;
                case MessageColors.BrightYellow:
                    returnString += ($"[93m{Message}[0m");
                    break;
                case MessageColors.BrightBlue:
                    returnString += ($"[94m{Message}[0m");
                    break;
                case MessageColors.BrightMagenta:
                    returnString += ($"[95m{Message}[0m");
                    break;
                case MessageColors.BrightCyan:
                    returnString += ($"[96m{Message}[0m");
                    break;
                default:
                    returnString += ($"[97m{Message}[0m");
                    break;
            }

            return (returnString);
        }
    }
}
