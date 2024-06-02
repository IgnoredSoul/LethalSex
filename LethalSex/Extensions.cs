using System;
using System.Text;
using UnityEngine;
using GameNetcodeStuff;
using System.Threading;
using System.Threading.Tasks;
using LethalSex_Core.Modules;
using System.Collections.Generic;
using System.Security.Cryptography;
using static UnityEngine.InputSystem.InputRemoting;
using System.Diagnostics;
using UnityEngine.AI;

namespace LethalSex_Core
{
	/// <summary>
	/// This class was used for my Obfuscation project but I started to prefer it more than the original shit.
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
				if (Log) Main.mls.LogError($"Added {component.name}");
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
		/// Joins an array and returns it as a string
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static string Join(this object[] array)
		{
			return String.Join(", ", array);
		}

		/// <summary>
		/// Wraps text in hex color
		/// </summary>
		/// <param name="text"></param>
		/// <param name="hexCode"></param>
		/// <returns>String wrapped in color</returns>
		public static string ColorWrap(string text, string hexCode) => $"<color={hexCode}>{text}</color>";

		/// <summary>
		/// Color to hex value
		/// </summary>
		/// <param name="baseColor"></param>
		/// <returns>Returns the hex value of a color</returns>
		public static string HexColor(Color baseColor)
		{
			return "#" + Convert.ToInt32(baseColor.r * 255f).ToString("X2") + Convert.ToInt32(baseColor.g * 255f).ToString("X2") + Convert.ToInt32(baseColor.b * 255f).ToString("X2");
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
				Main.mls.LogWarning($"Cancelling smooth increment task: {ActionName}");
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
		/// tbh idk
		/// </summary>
		/// <param name="action"></param>
		/// <param name="msg"></param>
		/// <returns cref="bool">True if worked, false if failed.</returns>
		/// <exception cref="Exception"></exception>
		public static bool TryCatch(object description, Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (Exception e)
			{
				var method = new StackTrace().GetFrame(1).GetMethod(); // Get the calling method
				var className = method.DeclaringType?.FullName;
				var methodName = method.Name;

				string detailedMessage = $"An error occoured in {className}.{methodName}:\nDescription: {description}\nLog: {e.Message}\n";

				// Throw error into ingame console and bepinex console.
				ConsoleManager.Error(detailedMessage);
				return false;
				throw new Exception(detailedMessage);
			}
		}

		public static Vector3 GetRandomNavMeshPositionInRadiusSpherical(Vector3 origin, float minRadius, float maxRadius, float rayHit = 20, float samHit = 10)
		{
			for (int i = 0; i < 10; i++)
			{
				// Calculate bullshit
				var sin = Mathf.Sin(Time.time * NumberUtils.NextF(1, 361));
				var cos = Mathf.Cos(Time.time * NumberUtils.NextF(1, 361));

				// Make new vector3 position using the sin and cos
				Vector3 pos = new Vector3(NumberUtils.NextF(minRadius, maxRadius) * sin, UnityEngine.Random.insideUnitSphere.y * maxRadius, NumberUtils.NextF(minRadius, maxRadius) * cos);

				pos += origin;

				// Cast a ray downwards from above the random position
				Vector3 rayStart = pos + Vector3.up * 20f; // Start the ray 10 units above the random position
				Ray ray = new Ray(rayStart, Vector3.down);

				if (Physics.Raycast(ray, out RaycastHit hit, rayHit))
				{
					if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, samHit, NavMesh.AllAreas))
					{
						return navHit.position;
					}
					else ConsoleManager.Log("Sample not hit");
				}
				else ConsoleManager.Log("Ray not hit");
			}

			// If no valid position is found, return an inf position and handle it after
			return new Vector3(0, -10000, 0);
		}
	}

	/// <summary>
	/// Just some helpful tools
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
		public static int NextL(int max) => Next(0, max - 1);

		/// <summary>
		/// Returns a random integer less than the specified maximum value.
		/// </summary>
		/// <param name="max">The exclusive upper bound of the random number to be generated.</param>
		/// <returns>A random integer less than <paramref name="max"/>.</returns>
		public static int Next(int max) => Next(0, max + 1);

		/// <summary>
		/// Returns a random integer within the specified range.
		/// </summary>
		/// <param name="min">The inclusive lower bound of the random number to be generated.</param>
		/// <param name="max">The exclusive upper bound of the random number to be generated.</param>
		/// <returns>A random integer within the specified range.</returns>
		public static int Next(int min, int max) => random.Next(min, max + 1);

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
		public static float NextF(float max) => NextF(0, max + 1);

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
		public static string String128(object input)
		{
			Hash128 h = new();
			h.Append(input.ToString());
			return h.ToString();
		}

		/// <summary>
		/// Returns a random vector3 position between the desired bounds.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 NextV3(float x, float y, float z) => new Vector3(NextF(-x, x), NextF(-y, y), NextF(-z, z));

		public static string MD5(object input)
		{
			using (MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input.ToString());
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
					sb.Append(hashBytes[i].ToString("x2"));
				return sb.ToString();
			}
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
				return GameNetworkManager.Instance?.localPlayerController ?? null;
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
			if (PlayerController != null) return PlayerController;
			int iterCount = 0;
			PlayerControllerB player = null;

			do
			{
				// If gameObject exists else null
				player = (GameNetworkManager.Instance?.localPlayerController);

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