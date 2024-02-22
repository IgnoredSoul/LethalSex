using GameNetcodeStuff;
using System;
using System.Collections.Generic;
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
    }
}