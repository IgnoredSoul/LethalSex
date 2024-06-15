using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using static LethalSex_Core.Extensions;

namespace LethalSex_Core
{
	[HarmonyPatch]
	public static class Patching
	{
		/// <summary>
		/// When the class 'HUDManager' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(HUDManager), "Start")]
		[HarmonyPostfix]
		private static void ONHUDSTART()
		{
			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONHUDSTART. Failed to send OnHUDStart to module: {module.ModuleName}", () =>
					{
						module?.OnHUDStart();   // Invoke method
					});
				});
			});
		}

		/// <summary>
		/// When the class 'HUDManager' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(HUDManager), "Awake")]
		[HarmonyPostfix]
		private static void ONHUDAWAKE()
		{
			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONHUDAWAKE. Failed to send OnHUDAwake to module: {module.ModuleName}", () =>
					{
						module?.OnHUDAwake();   // Invoke method
					});
				});
			});
		}

		/// <summary>
		/// When the class 'HUDManager' component calls the method.
		/// <br/><b><u>POSTFIX</u></b><br/>
		/// <b><u>NO VIRTUAL / OVERRIDE</u></b>
		/// </summary>
		[HarmonyPatch(typeof(HUDManager), "Update")]
		[HarmonyPostfix]
		private static void ONHUDUPDATE()
		{
			ONSANITYCHANGED();
		}

		/// <summary>
		/// When the class 'StartOfRound' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
		[HarmonyPostfix]
		private static void ONSHIPLAND()
		{
			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONSHIPLAND. Failed to send OnShipLand to module: {module.ModuleName}", () =>
					{
						module?.OnShipLand();   // Invoke method
					});
				});
			});
		}

		/// <summary>
		/// When the class 'PlayerControllerB' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
		[HarmonyPostfix]
		private static void ONLOCALSTART(PlayerControllerB __instance)
		{
			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONPLAYERSTART. Failed to send OnHUDAwake to module: {module.ModuleName}", () =>
					{
						module?.OnLocalStart(__instance);   // Invoke method
					});
				});
			});
		}

		/// <summary>
		/// When the class 'PlayerControllerB' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
		[HarmonyPostfix]
		private static void ONLOCALDIE()
		{
			// Since any player can invoke this method, we need to check if its us first.
			if ((bool)!LocalPlayer.PlayerController?.isPlayerDead) return;

			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONKILLPLAYER. Failed to send OnLocalPlayerDie to module: {module.ModuleName}", () =>
					{
						module?.OnLocalDie();   // Invoke method
					});
				});
			});
		}

		/// <summary>
		/// When the class 'PlayerControllerB' component calls the method.
		/// <br/><b><u>POSTFIX</u></b>
		/// </summary>
		[HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
		[HarmonyPostfix]
		private static void ONGRABOBJECT()
		{
			// Since any player can invoke this method, we need to check if its us first.
			if ((bool)!LocalPlayer.PlayerController?.isPlayerDead || !LocalPlayer.Interected) return;

			// Loop through registered mod's list.
			Main.LethalMods.ForEach(mod =>
			{
				// Loop through modules registered inside the mod.
				mod.Modules.ForEach(module =>
				{
					// Handle errors that may arise when invoking the method.
					TryCatch($"An error occoured at ONGRABOBJECT. Failed to send OnGrabObject to module: {module.ModuleName}", () =>
					{
						// Cast ray from our position downwards.
						Ray interactRay = new Ray(LocalPlayer.PlayerController.gameplayCamera.transform.position, LocalPlayer.PlayerController.gameplayCamera.transform.forward);

						// Raycast down at the length of the item(?) and its layer.
						if (Physics.Raycast(interactRay, out RaycastHit hit, LocalPlayer.PlayerController.grabDistance, 832))
							if (hit.collider?.transform.gameObject?.GetComponent<GrabbableObject>() != null) // Check if the object we hit has the component of 'GrabbaleObject'
								module?.OnGrabObject(hit.collider.transform.gameObject.GetComponent<GrabbableObject>()); // Invoke with the object
					});
				});
			});
		}

		private static float prev_san { get; set; } = -666f;

		private static void ONSANITYCHANGED()
		{
			// If the player is null or player is dead
			if (!LocalPlayer.Player || (bool)LocalPlayer.PlayerController?.isPlayerDead) return;

			// If the previous value is not the same as current value
			if (prev_san != float.Parse(LocalPlayer.Insanity.ToString("0.0")))
			{
				// Set previous value to current value
				prev_san = float.Parse(LocalPlayer.Insanity.ToString("0.0"));

				// Loop through registered mod's list.
				Main.LethalMods.ForEach(mod =>
				{
					// Loop through modules registered inside the mod.
					mod.Modules.ForEach(module =>
					{
						// Handle errors that may arise when invoking the method.
						TryCatch($"An error occoured at ONSANITYCHANGED. Failed to send OnSanityChanged to module: {module.ModuleName}", () =>
						{
							module?.OnSanityChanged(LocalPlayer.Insanity);   // Invoke method
						});
					});
				});
			}
		}
	}
}