using GameNetcodeStuff;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace LethalSex_Core
{
    /// <summary>
    /// Represents a dynamic configuration object that provides access to properties with dynamic types.
    /// </summary>
    public class DynConfig : DynamicObject
    {
        private readonly JObject _jsonObject;

        /// <summary>
        /// Constructor :3
        /// </summary>
        /// <param name="jsonObject"></param>
        public DynConfig(JObject jsonObject) => _jsonObject = jsonObject;

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (_jsonObject == null)
            {
                result = null;
                return false;
            }

            var property = _jsonObject.Property(binder.Name);
            if (property != null)
            {
                var value = property.Value;
                if (value is JObject nestedObject || value is JArray)
                {
                    result = new DynConfig((JObject)value);
                    return true;
                }
                else
                {
                    string[] keyValue = value.ToString().Split(':');
                    if (keyValue.Length == 2)
                    {
                        switch (keyValue[0].ToLower())
                        {
                            case "i":
                                if (int.TryParse(keyValue[1], out int ivalue))
                                {
                                    result = ivalue;
                                    return true;
                                }
                                break;

                            case "b":
                                if (bool.TryParse(keyValue[1], out bool bvalue))
                                {
                                    result = bvalue;
                                    return true;
                                }
                                break;

                            case "f":
                                if (float.TryParse(keyValue[1].TrimEnd('f'), out float fvalue))
                                {
                                    result = fvalue;
                                    return true;
                                }
                                break;

                            case "s":
                                result = keyValue[1];
                                return true;
                        }
                    }
                    else
                    {
                        result = value?.ToString();
                        return true;
                    }
                }
            }

            Main.mls.LogError($"DynConfig error:\n{binder.Name} does not exist and has returned null.");
            ConsoleManager.LogErr($"DynConfig error:\n{binder.Name} does not exist and has returned null.");

            result = null;
            return true; // Indicate that the property was not found
        }

        /// <inheritdoc/>
        public override string ToString() => _jsonObject.ToString();
    }

    /// <summary>
    /// This class was used for my Obfuscation project but I started to prefer it more than the original shit.
    /// </summary>
    public static class NumberUtils
    {
        /// <summary>
        /// Represents a shared instance of a random number generator.
        /// </summary>
        public static readonly System.Random random = new System.Random(GenerateTrulyRandomNumber());

        /// <summary>
        /// Generates a truly random number using cryptographic random number generation.
        /// </summary>
        /// <returns>A truly random number within a specified range.</returns>
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
        /// Determines if an event with a given probability occurs.
        /// </summary>
        /// <param name="percentage">The probability of the event occurring, in percentage.</param>
        /// <returns>True if the event occurs, otherwise false.</returns>
        public static bool Chance(int percentage = 50)
        {
            if (percentage < 0 || percentage > 100) throw new Exception("Uh...?");
            return (Next(0, 100) < percentage);
        }

        /// <summary>
        /// Determines if an event with a given probability occurs.
        /// </summary>
        /// <param name="percentage">The probability of the event occurring, in percentage.</param>
        /// <returns>True if the event occurs, otherwise false.</returns>
        public static bool Chance(float percentage = 50f)
        {
            if (percentage < 0f || percentage > 100f) throw new Exception("Uh...?");
            return (NextF(0f, 100f) < percentage);
        }

        /// <summary>
        /// Generates a random 64-bit integer.
        /// </summary>
        /// <returns>A randomly generated 64-bit integer.</returns>
        public static long GenInt64()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            long randomInt64 = BitConverter.ToInt64(buffer, 0);
            return randomInt64;
        }

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A random non-negative integer.</returns>
        public static int Next() => Next(0, int.MaxValue);

        /// <summary>
        /// Returns a random integer less than the specified maximum value. Mainly used for arrays / lists
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random number to be generated.</param>
        /// <returns>A random integer less than <paramref name="max"/>.</returns>
        public static int NextL(int max) => Next(0, max + 1);

        /// <summary>
        /// Returns a random integer less than the specified maximum value.
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random number to be generated.</param>
        /// <returns>A random integer less than <paramref name="max"/>.</returns>
        public static int Next(int max) => Next(0, max);

        /// <summary>
        /// Returns a random integer within the specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number to be generated.</param>
        /// <param name="max">The exclusive upper bound of the random number to be generated.</param>
        /// <returns>A random integer within the specified range.</returns>
        public static int Next(int min, int max) => random.Next(min, max);

        /// <summary>
        /// Returns a random float number between 0.0 and the maximum value representable by a float (not inclusive).
        /// </summary>
        /// <returns>A random float number between 0.0 and the maximum value representable by a float (not inclusive).</returns>
        public static float NextF() => NextF(0, float.MaxValue);

        /// <summary>
        /// Returns a random float number less than the specified maximum value.
        /// </summary>
        /// <param name="max">The maximum value of the random float number to be generated.</param>
        /// <returns>A random float number less than <paramref name="max"/>.</returns>
        public static float NextF(float max) => NextF(0, max);

        /// <summary>
        /// Returns a random float number within the specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random float number to be generated.</param>
        /// <param name="max">The exclusive upper bound of the random float number to be generated.</param>
        /// <returns>A random float number within the specified range.</returns>
        public static float NextF(float min, float max) => (float)((NextD() * (max - min)) + min);

        /// <summary>
        /// Returns a random double number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double number between 0.0 and 1.0.</returns>
        public static double NextD() => random.NextDouble();

        /// <summary>
        /// Generates a 128-bit string hash based on the given value.
        /// </summary>
        /// <param name="val">The value to be hashed.</param>
        /// <returns>A 128-bit string hash.</returns>
        public static string String128(object val)
        {
            Hash128 h = new();
            h.Append(val.ToString());
            return h.ToString();
        }
    }

    /// <summary>
    /// Just some helpful tools
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Retrieves or adds a component of type T to the specified GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve or add.</typeparam>
        /// <param name="gameObject">The GameObject to operate on.</param>
        /// <param name="Log">Specifies whether to log a message when adding the component (optional, default is false).</param>
        /// <returns>The retrieved or newly added component of type T.</returns>
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
        /// Attempts to destroy a UnityEngine.Object and returns whether the operation was successful.
        /// </summary>
        /// <param name="obj">The UnityEngine.Object to destroy.</param>
        /// <returns>True if the UnityEngine.Object was destroyed successfully, otherwise false.</returns>
        public static bool TryDestroy(this UnityEngine.Object o)
        {
            try
            {
                UnityEngine.Object.Destroy(o);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the hierarchical path of a GameObject.
        /// </summary>
        /// <param name="transform">The Transform component of the GameObject.</param>
        /// <returns>The hierarchical path of the GameObject.</returns>
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
        /// Smoothly changes a value to another value using asynchronous tasks.
        /// </summary>
        /// <param name="ActionName">The name of the action.</param>
        /// <param name="action">The action to perform during the transition.</param>
        /// <param name="start">The starting value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="duration">The duration of the transition.</param>
        public static async void SmoothIncrementValue(string ActionName, Action<float> action, float start, float target, float duration)
        {
            // Cancel the task if it's already running
            if (ctsDict.ContainsKey(ActionName))
            {
                ctsDict[ActionName].Cancel();
                ctsDict[ActionName].Dispose();
                ConsoleManager.LogWrn($"Cancelling smooth increment task: {ActionName}");
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

        /// <summary>
        /// Creates a JObject entry using the specified action.
        /// </summary>
        /// <param name="entry">The action to perform to create the JObject entry.</param>
        /// <returns>A JObject containing the entry data.</returns>
        public static JObject CreateEntry(Action<JObject> entry)
        {
            // Create JObject for the item data
            JObject item = new();

            // Do action with the item data
            entry(item);

            // Return the item data
            return item;
        }

        /// <summary>
        /// Creates a JObject entry with a specified key using the specified action and adds it to a destination JObject.
        /// </summary>
        /// <param name="entry">The action to perform to create the JObject entry.</param>
        /// <param name="dest">The destination JObject to add the entry to.</param>
        /// <param name="key">The key to associate with the entry in the destination JObject.</param>
        /// <returns>A JObject containing the entry data with the specified key.</returns>
        public static JObject CreateEntry(this JObject _dest, Action<JObject> entry, string _name)
        {
            JObject res = new();
            entry(res);
            _dest[_name] = res;
            return res;
        }

        /// <summary>
        /// Creates a shelf in the specified JObject with the provided name, keys, and items.
        /// </summary>
        /// <param name="_dest">The JObject where the shelf will be created.</param>
        /// <param name="_name">The name of the shelf.</param>
        /// <param name="_keys">A comma-separated string representing keys for the items.</param>
        /// <param name="_items">A string representing items separated by commas.</param>
        public static void CreateShelf(this JObject _dest, string _name, string _keys, string _items)
        {
            // If the items string is empty or the keys string is empty
            if (_items == string.Empty || _keys == string.Empty || _name == string.Empty || _dest == null) return;

            // Zip items with keys
            Dictionary<string, string> keyValuePairs = ValueValidator(_items).Zip(_keys.Replace(" ", "").Split(','), (item, key) => new { Item = item, Key = key })
                .ToDictionary(x => x.Key, x => x.Item);

            // Iterate through each item and give them a room inside the house (_dest)
            _dest[_name.Replace(" ", "")] = new JObject(keyValuePairs.Select(kv => new JProperty(kv.Key, kv.Value)));
        }

        /// <summary>
        /// Creates a shelf in the specified JObject with the provided name, keys, and items.<br/>
        /// This will also check if the items match the expected value type
        /// </summary>
        /// <param name="_dest"></param>
        /// <param name="_name"></param>
        /// <param name="_keys"></param>
        /// <param name="_items"></param>
        /// <param name="_expected"></param>
        /// <exception cref="Exception"></exception>
        public static void CreateShelf(this JObject _dest, string _name, string _keys, string _items, char[] _expected)
        {
            // If the items string is empty or the keys string is empty
            if (_items == string.Empty || _keys == string.Empty || _name == string.Empty || _dest == null) return;

            string[] items = ValueValidator(_items);
            for (int i = 0; i < items.Length; i++)
                if (items[i][0] != _expected[i])
                    throw new Exception($"Item: '{items[i]}', does not match the expected value of {_expected[i]}.");

            // Zip items with keys
            Dictionary<string, string> keyValuePairs = items.Zip(_keys.Replace(" ", "").Split(','), (item, key) => new { Item = item, Key = key })
                .ToDictionary(x => x.Key, x => x.Item);

            // Iterate through each item and give them a room inside the house (_dest)
            _dest[_name.Replace(" ", "")] = new JObject(keyValuePairs.Select(kv => new JProperty(kv.Key, kv.Value)));
        }

        /// <summary>
        /// Creates a box in the specified JObject with the provided name and items.
        /// </summary>
        /// <param name="_dest">The JObject where the box will be created.</param>
        /// <param name="_name">The name of the box.</param>
        /// <param name="_items">A string representing items separated by commas.</param>
        public static void CreateBox(this JObject _dest, string _name, string _items)
        {
            // If the items string is empty or the keys string is empty
            if (_items == string.Empty || _name == string.Empty || _dest == null) return;

            // Iterate through each item and give them a room inside the house (_dest)
            _dest[_name.Replace(" ", "")] = new JArray(ValueValidator(_items));
        }

        /// <summary>
        /// Creates a box in the specified JObject with the provided name and items.<br/>
        /// This will also check if the items match the expected value type
        /// </summary>
        /// <param name="_dest"></param>
        /// <param name="_name"></param>
        /// <param name="_items"></param>
        /// <param name="_expected"></param>
        /// <exception cref="Exception"></exception>
        public static void CreateBox(this JObject _dest, string _name, string _items, char[] _expected)
        {
            // If the items string is empty or the keys string is empty
            if (_items == string.Empty || _name == string.Empty || _dest == null) return;

            string[] items = ValueValidator(_items);
            for (int i = 0; i < items.Length; i++)
                if (items[i][0] != _expected[i])
                    throw new Exception($"Item: '{items[i]}', does not match the expected value of '{_expected[i]}:'.");

            // Iterate through each item and give them a room inside the house (_dest)
            _dest[_name.Replace(" ", "")] = new JArray(items);
        }

        /// <summary>
        /// Validates the values and returns an array of strings.
        /// </summary>
        /// <param name="args">The string representing items or a single item.</param>
        /// <returns>An array of strings containing validated items.</returns>
        private static string[] ValueValidator(string args)
        {
            // Create new list of strings
            List<string> l = new();

            // Make regex for checking the value keys
            Regex vk = new Regex(@"([sbif]):(.+)", RegexOptions.IgnoreCase);

            // Check if its a list of items
            if (args.Contains(','))
                foreach (var item in args.Replace(" ", "").Split(','))
                    SwitchParse(item, ref l);
            else
                SwitchParse(args, ref l);

            void SwitchParse(string value, ref List<string> l)
            {
                if (!vk.Match(value).Success)
                {
                    Main.mls.LogError($"Value: '{value}', either doesnt contain an identifier or is incorrect. Converting to 's:'");
                    value = $"s:{value}";
                }
                string[] keyValue = value.Split(':');
                switch (keyValue[0].ToLower())
                {
                    case "i":
                        if (!int.TryParse(keyValue[1], out int intValue))
                            // If the identifier is a int but the value isnt, fuck shit up.
                            throw new Exception($"Parsed int arg, '{value}', is not correct. The indentifier does not match the value.");
                        break;

                    case "b":
                        if (!bool.TryParse(keyValue[1], out bool boolValue))
                            // If the identifier is a int but the value isnt, fuck shit up.
                            throw new Exception($"Parsed bool arg, '{value}', is not correct. The indentifier does not match the value.");
                        break;

                    case "f":
                        if (!float.TryParse(keyValue[1].TrimEnd('f'), out float floatValue))
                            // If the identifier is a float but the value isnt, fuck shit up.
                            throw new Exception($"Parsed float arg, '{value}', is not correct. The indentifier does not match the value.");
                        break;

                    case "s":
                        break;

                    default:
                        throw new Exception($"Parsed arg, '{value}', is not correct. Please append an identifier");
                }

                l.Add(value);
            }

            return l.ToArray();
        }
    }

    /// <summary>
    /// Easier way for me to get shit from the player ¯\_(ツ)_/¯ <br/>
    /// Provides static access to various properties and methods related to the local player.
    /// </summary>
    public static class LocalPlayer
    {
        /// <summary>
        /// Gets the <see cref="PlayerControllerB"/> instance representing the local player controller.
        /// </summary>
        public static PlayerControllerB PlayerController
        {
            get
            {
                return StartOfRound.Instance?.localPlayerController;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="PlayerControllerB"/> instance representing the local player controller.
        /// </summary>
        /// <param name="maxIter">Maximum number of iterations to wait for.</param>
        /// <param name="delay">Delay between iterations in milliseconds.</param>
        /// <returns>A task representing the asynchronous operation that yields the local player controller.</returns>
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

        /// <summary>
        /// Gets the <see cref="GameObject"/> representing the local player.
        /// </summary>
        public static GameObject Player
        {
            get
            {
                return PlayerController?.gameObject;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="GameObject"/> representing the local player.
        /// </summary>
        /// <param name="maxIter">Maximum number of iterations to wait for.</param>
        /// <param name="delay">Delay between iterations in milliseconds.</param>
        /// <returns>A task representing the asynchronous operation that yields the local player.</returns>
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

        /// <summary>
        /// Gets the insanity level of the local player.
        /// </summary>
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

        /// <summary>
        /// Asynchronously retrieves the insanity level of the local player.
        /// </summary>
        /// <param name="maxIter">Maximum number of iterations to wait for.</param>
        /// <param name="delay">Delay between iterations in milliseconds.</param>
        /// <returns>A task representing the asynchronous operation that yields the insanity level of the local player.</returns>
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

        /// <summary>
        /// Gets the maximum insanity level of the local player.
        /// </summary>
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

        /// <summary>
        /// Asynchronously retrieves the maximum insanity level of the local player.
        /// </summary>
        /// <param name="maxIter">Maximum number of iterations to wait for.</param>
        /// <param name="delay">Delay between iterations in milliseconds.</param>
        /// <returns>A task representing the asynchronous operation that yields the maximum insanity level of the local player.</returns>
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

        /// <summary>
        /// Gets the camera attached to the local player.
        /// </summary>
        public static Camera Camera
        {
            get
            {
                return Player?.GetComponentInChildren<Camera>();
            }
        }

        /// <summary>
        /// Asynchronously retrieves the camera attached to the local player.
        /// </summary>
        /// <param name="maxIter">Maximum number of iterations to wait for.</param>
        /// <param name="delay">Delay between iterations in milliseconds.</param>
        /// <returns>A task representing the asynchronous operation that yields the camera attached to the local player.</returns>
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

        /// <summary>
        /// Indicates whether the local player is near other players within a specified radius.
        /// </summary>
        public static bool IsNearOtherPlayers => PlayersNearMe().Length > 0;

        /// <summary>
        /// Retrieves an array of player controllers representing other players near the local player within a specified radius.
        /// </summary>
        /// <param name="rad">The radius within which to search for other players.</param>
        /// <returns>An array of player controllers representing other players near the local player.</returns>
        public static PlayerControllerB[] PlayersNearMe(float rad = 10f)
        {
            PlayerControllerB[] l = null;

            foreach (PlayerControllerB player in Resources.FindObjectsOfTypeAll<PlayerControllerB>())
                if (Vector3.Distance(player.transform.position, Player.transform.position) <= rad)
                    l[l.Length] = player;

            return l;
        }

        /// <summary>
        /// Indicates whether the quick menu is open for the local player.
        /// </summary>
        public static bool IsMenuOpen => PlayerController.quickMenuManager.isMenuOpen;

        /// <summary>
        /// Indicates whether the terminal menu is open for the local player.
        /// </summary>
        public static bool IsTermOpen => PlayerController.inTerminalMenu;
    }
}