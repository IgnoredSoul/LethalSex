using GameNetcodeStuff;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalSex_Core
{
    /// <summary>
    /// This class was used for my Obfuscation project but I started to prefer it more than the original shit.
    /// </summary>
    public static class NumberUtils
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

    /// <summary>
    /// Just some helpful tools
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get or add component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="Log"></param>
        /// <returns>Component (T)</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject, bool Log = false) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (!component)
            {
                component = gameObject.AddComponent<T>();
                if (Log) ConsoleManager.Log($"Added {component.name}");
            }

            return component;
        }

        /// <summary>
        /// Try and destroy gameObject
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if it destroyed gameObject or not</returns>
        public static bool TryDestroy(this GameObject o)
        {
            bool result;
            try
            {
                UnityEngine.Object.Destroy(o);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Try and destroy gameObject
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if it destroyed gameObject or not</returns>
        public static bool TryDestroy(this Transform o)
        {
            bool result;
            try
            {
                UnityEngine.Object.Destroy(o);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Try and destroy component
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if it destroyed component or not</returns>
        public static bool TryDestroy(this Component o)
        {
            bool result;
            try
            {
                UnityEngine.Object.Destroy(o);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get object path of gameObject.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>Path to gameObject</returns>
        public static string GetObjPath(this Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// I am too dumb to do this any other way so.... this is what I've god :3
        /// </summary>
        private static Dictionary<string, CancellationTokenSource> ctsDict = new Dictionary<string, CancellationTokenSource>();

        /// <summary>
        /// Smoothy change a value to another value using tasks. The previous task will stop and restart with new values when same task name is started
        /// </summary>
        /// <param name="ActionName"></param>
        /// <param name="action"></param>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        public static async void SmoothIncrementValue(string ActionName, Action<float> action, float start, float target, float duration)
        {
            // Cancel the task if it's already running
            if (ctsDict.ContainsKey(ActionName))
            {
                ctsDict[ActionName].Cancel();
                ctsDict[ActionName].Dispose();
                ConsoleManager.Log($"Cancelling smooth increment task: {ActionName}", "Warn", Color.yellow);
                ctsDict.Remove(ActionName);
            }

            // Create a new cancellation token source for the task
            ctsDict.Add(ActionName, new CancellationTokenSource());

            // Start the task
            Task smoothIncrementTask = SmoothIncrementValueTask((value) =>
            {
                action(value);
            }, start, target, duration, ctsDict[ActionName].Token);

            // Wait for the task to complete (or be canceled)
            await smoothIncrementTask;

            // Clean up the CancellationTokenSource and remove action from dict
            ctsDict.Remove(ActionName);
        }

        private static async Task SmoothIncrementValueTask(Action<float> action, float start, float target, float duration, CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;
            float currentValue;

            while (elapsedTime < duration)
            {
                // Check for cancellation
                if (cancellationToken.IsCancellationRequested)
                {
                    // Perform cleanup or any necessary actions before exiting
                    return;
                }

                currentValue = Mathf.Lerp(start, target, elapsedTime / duration);
                action(currentValue);

                elapsedTime += Time.deltaTime;

                await Task.Yield();
            }

            currentValue = target;
            action(currentValue);
        }

        public static JObject JsonPackage(JObject[] objects, string[] keys)
        {
            if (objects.Length != keys.Length)
            {
                Debug.LogError("The objects array and keys array are not the same length.");
                return JObject.Parse(JsonConvert.SerializeObject(new Dictionary<string, string>()));
            }

            JObject result = new JObject();

            for (int i = 0; i < objects.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(keys[i]))
                {
                    JObject currentObject = result;
                    string[] embeddingKeys = keys[i].Split('.');
                    for (int j = 0; j < embeddingKeys.Length - 1; j++)
                    {
                        if (currentObject[embeddingKeys[j]] == null)
                        {
                            currentObject[embeddingKeys[j]] = new JObject();
                        }
                        currentObject = (JObject)currentObject[embeddingKeys[j]];
                    }
                    currentObject[embeddingKeys.Last()] = objects[i];
                }
                else
                {
                    result.Merge(objects[i]);
                }
            }

            return result;
        }

        public static void InsertArray(object[] items, JObject dest, string key)
        {
            dest[key] = new JObject()[key] = new JArray(items);
        }

        public static void InsertList(object[] values, string[] keys, JObject dest, string key)
        {
            if (values.Length != keys.Length)
            {
                Debug.LogError("The values array and keys array are not the same length.");
                Debug.LogError($"[{string.Join(", ", values.ToList().Select(obj => obj))}], [{string.Join(", ", keys.ToList().Select(obj => obj))}]");
                return;
            }

            dest[key] = JObject.FromObject(values.Zip(keys, (value, key) => new { Key = key, Value = value }).ToDictionary(item => item.Key, item => item.Value));
        }

        public static JObject CreateEntry(Action<JObject> entry)
        {
            JObject res = new JObject();
            entry(res);
            return res;
        }

        public static JObject CreateEntry(Action<JObject> entry, string key)
        {
            JObject res = new JObject();
            entry(res);

            JObject result = new JObject();
            result[key] = res;
            return result;
        }

        public static JObject CreateEntry(Action<JObject> entry, JObject dest, string key)
        {
            JObject res = new JObject();
            entry(res);
            dest[key] = res;
            return res;
        }
    }

    /// <summary>
    /// Easier way for me to get shit from the player ¯\_(ツ)_/¯
    /// </summary>
    public static class LocalPlayer
    {
        public static PlayerControllerB PlayerController
        {
            get
            {
                return StartOfRound.Instance?.localPlayerController;
            }
        }

        public static async Task<PlayerControllerB> PlayerControllerAsync(int maxIter = 25, int delay = 250)
        {
            int iterCount = 0;
            PlayerControllerB player = null;

            do
            {
                // If gameObject exists else null
                player = (StartOfRound.Instance?.localPlayerController);

                // wait 250ms | 0.25s if player doesnt exist
                if (!player)
                    await Task.Delay(delay);
            } while (++iterCount < maxIter || !player); // Iter counter so it doesnt sit here for ever

            return player;
        }

        public static GameObject Player
        {
            get
            {
                return PlayerController?.gameObject;
            }
        }

        public static async Task<GameObject> PlayerAsync(int maxIter = 25, int delay = 250)
        {
            int iterCount = 0;
            GameObject player = null;

            do
            {
                // If gameObject exists else null
                player = (StartOfRound.Instance?.localPlayerController?.gameObject);

                // wait 250ms | 0.25s if player doesnt exist
                if (!player)
                    await Task.Delay(delay);
            } while (++iterCount < maxIter || !player); // Iter counter so it doesnt sit here for ever

            return player;
        }

        public static float Insanity
        {
            get
            {
                return PlayerController?.insanityLevel ?? -1;
            }
            set
            {
                if (PlayerController) PlayerController.insanityLevel = value;
            }
        }

        public static async Task<float> InsanityAsync(int maxIter = 25, int delay = 250)
        {
            int iterCount = 0;
            float insanity;
            do
            {
                // If sanity exists else -1
                insanity = StartOfRound.Instance?.localPlayerController?.insanityLevel ?? -1;

                // wait 250ms | 0.25s if sanity is below 0
                if (insanity < 0)
                    await Task.Delay(delay);
            } while (++iterCount < maxIter || insanity < 0); // Iter counter so it doesnt sit here for ever

            return insanity;
        }

        public static float MaxInsanity
        {
            get
            {
                return PlayerController?.maxInsanityLevel ?? -1;
            }
            set
            {
                if (PlayerController) PlayerController.maxInsanityLevel = value;
            }
        }

        public static async Task<float> MaxInsanityAsync(int maxIter = 25, int delay = 250)
        {
            int iterCount = 0;
            float insanity;
            do
            {
                // If sanity exists else -1
                insanity = StartOfRound.Instance?.localPlayerController?.maxInsanityLevel ?? -1;

                // wait 250ms | 0.25s if sanity is below 0
                if (insanity < 0)
                    await Task.Delay(delay);
            } while (++iterCount < maxIter || insanity < 0); // Iter counter so it doesnt sit here for ever

            return insanity;
        }

        public static Camera Camera
        {
            get
            {
                return Player?.GetComponentInChildren<Camera>();
            }
        }

        public static async Task<Camera> CameraAsync(int maxIter = 25, int delay = 250)
        {
            int iterCount = 0;
            Camera cam = null;

            do
            {
                // If camera exists else null
                cam = (Player?.GetComponentInChildren<Camera>());

                // wait 250ms | 0.25s if player doesnt exist
                if (!cam)
                    await Task.Delay(delay);
            } while (++iterCount < maxIter || !cam); // Iter counter so it doesnt sit here for ever

            return cam;
        }

        public static bool IsNearOtherPlayers => PlayersNearMe().Length > 0;

        public static PlayerControllerB[] PlayersNearMe(float rad = 10f)
        {
            PlayerControllerB[] l = null;

            foreach (PlayerControllerB player in Resources.FindObjectsOfTypeAll<PlayerControllerB>())
                if (Vector3.Distance(player.transform.position, Player.transform.position) <= rad)
                    l[l.Length] = player;

            return l;
        }

        public static bool IsMenuOpen => PlayerController.quickMenuManager.isMenuOpen;
        public static bool IsTermOpen => PlayerController.inTerminalMenu;
    }
}