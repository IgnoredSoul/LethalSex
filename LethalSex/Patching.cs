using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using static LethalSex_Core.Extensions;
using System.Collections;
using System.Threading.Tasks;
using LethalSex_Core.Modules;

namespace LethalSex_Core
{
	[HarmonyPatch]
	public static class Patching
	{
		/// <summary>
		/// Handles the patch for HUDManager's Awake method.
		/// </summary>
		[HarmonyPatch(typeof(HUDManager), "Awake")]
		[HarmonyPostfix]
		private static void _handle_OnHUDAwake()
		{
			TryCatch("An error occoured at _handle_OnHUDAwake", () =>
			{
				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnHUDAwake. Failed to loop through module list", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnHUDAwake. Failed to send OnHUDAwake", () =>
							{
								// Invoke method
								m?.OnHUDAwake();
							});
						});
					});
				});
			});
		}

		/// <summary>
		/// Handles the patch for HUDManager's Start method.
		/// <br/>
		/// Also doubles for the OnLocalPlayerStart virtual method.
		/// </summary>
		[HarmonyPatch(typeof(HUDManager), "Start")]
		[HarmonyPostfix]
		private static void _handle_OnHUDStart()
		{
			TryCatch("An error occoured at _handle_OnHUDStart", () =>
			{
				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnHUDStart. Failed to loop through module list", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnHUDStart. Failed to send OnHUDStart", () =>
							{
								// Invoke method
								m?.OnHUDStart();
							});
						});
					});
				});
			});

			TryCatch("An error occoured at _handle_OnHUDStart | 2", () =>
			{
				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnHUDStart. Failed to loop through module list | 2", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnHUDStart | 2. Failed to send OnLocalPlayerStart", async () =>
							{
								// Invoke method if player controller exists
								await LocalPlayer.PlayerControllerAsync()
								.ContinueWith(task => { if (task.Result) m?.OnLocalPlayerStart(task.Result); });
							});
						});
					});
				});
			});
		}

		/// <summary>
		/// Handles the patch for StartOfRound's OnShipLandedMiscEvents method.
		/// </summary>
		[HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
		[HarmonyPostfix]
		private static void _handle_OnShipLand()
		{
			TryCatch("An error occoured at _handle_OnShipLand", () =>
			{
				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnShipLand. Failed to loop through module list", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnShipLand. Failed to send OnShipLand", () =>
							{
								// Invoke method
								m?.OnShipLand();
							});
						});
					});
				});
			});
		}

		/// <summary>
		/// Handles the patch for PlayerControllerB's isPlayerDead 'method'.
		/// </summary>
		[HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
		[HarmonyPostfix]
		private static void _handle_OnPlayerDie()
		{
			TryCatch("An error occoured at _handle_OnPlayerDie", () =>
			{
				TryCatch("An error occoured at _handle_OnPlayerDie. Failed to get localplayer", () =>
				{
					if ((bool)!LocalPlayer.PlayerController?.isPlayerDead) return;
				});

				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnPlayerDie. Failed to loop through module list", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnPlayerDie. Failed to send OnLocalPlayerDie", () =>
							{
								// Invoke method
								m?.OnLocalPlayerDie();
							});
						});
					});
				});
			});
		}

		/// <summary>
		/// Handles the patch for PlayerControllerB's BeginGrabObject method.
		/// </summary>
		[HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
		[HarmonyPrefix]
		private static void _handle_OnGrabObject()
		{
			TryCatch("An error occoured at _handle_OnGrabObject", () =>
			{
				// Loop through mods
				Main.LethalMods.ForEach(mod =>
				{
					TryCatch("An error occoured at _handle_OnGrabObject. Failed to loop through module list", () =>
					{
						// Loop through modules
						mod.Modules.ForEach(m =>
						{
							TryCatch("An error occoured at _handle_OnGrabObject. Failed to send OnGrabObject", () =>
							{
								// Cast ray
								Ray interactRay = new Ray(LocalPlayer.PlayerController.gameplayCamera.transform.position, LocalPlayer.PlayerController.gameplayCamera.transform.forward);

								// Raycast hit and invoke method
								if (Physics.Raycast(interactRay, out RaycastHit hit, LocalPlayer.PlayerController.grabDistance, 832))
									if (hit.collider?.transform.gameObject?.GetComponent<GrabbableObject>() != null)
										m?.OnGrabObject(hit.collider.transform.gameObject.GetComponent<GrabbableObject>());
							});
						});
					});
				});
			});
		}

		/*        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
				[HarmonyPrefix]
				private static bool _handle_OnDamagePlayer(int damageNumber, bool hasDamageSFX = true, bool callRPC = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false, Vector3 force = default)
				{
					bool __res = true;
					TryCatch("An error occoured at _handle_OnDamagePlayer", () =>
					{
						// Loop through mods
						Main.LethalMods.ForEach(mod =>
						{
							TryCatch("An error occoured at _handle_OnDamagePlayer. Failed to loop through module list", () =>
							{
								// Loop through modules
								mod.Modules.ForEach(m =>
								{
									TryCatch("An error occoured at _handle_OnDamagePlayer. Failed to send OnDamagePlayer", () =>
									{
										// Invoke method
										__res = m.OnDamagePlayer(ref damageNumber, ref hasDamageSFX, ref callRPC, ref causeOfDeath, ref deathAnimation, ref fallDamage, ref force);
									});
								});
							});
						});
					});

					ConsoleManager.Log($"Res: {__res}");
					return __res;
				}*/
	}
}