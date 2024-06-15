using System;
using UnityEngine;
using GameNetcodeStuff;
using LethalSex_Core.Modules;

namespace LethalSex_Core
{
	public abstract class LethalModule : MonoBehaviour
	{
		#region Module information

		public string ModuleName { get; set; }
		public LethalMod Mod { get; set; }
		public LethalModule Module { get; set; }

		public LethalModule() => Module = this;

		#endregion Module information

		#region Virtuals

		public virtual void OnRegister()
		{ }

		/// <summary>
		/// Called when the behavior is enabled.
		/// </summary>
		protected virtual void Enabled()
		{
			Main.mls.LogWarning($"Behaviour: {GetType().Name} has been enabled");
			ConsoleManager.Warn($"Behaviour: {GetType().Name} has been enabled");
		}

		/// <summary>
		/// Called when the behavior is disabled.
		/// </summary>
		protected virtual void Disabled()
		{
			Main.mls.LogWarning($"Behaviour: {GetType().Name} has been disabled");
			ConsoleManager.Warn($"Behaviour: {GetType().Name} has been disabled");
		}

		/// <summary>
		/// Called when the behavior is destroyed.
		/// </summary>
		protected virtual void Destroyed()
		{
			Main.mls.LogWarning($"Behaviour: {GetType().Name} has been destroyed");
			ConsoleManager.Warn($"Behaviour: {GetType().Name} has been destroyed");
		}

		/// <summary>
		/// Called when the HUDManager awakens.
		/// </summary>
		public virtual void OnHUDAwake()
		{ }

		/// <summary>
		/// Called when the HUDManager starts.
		/// </summary>
		public virtual void OnHUDStart()
		{ }

		/// <summary>
		/// Called when the local player starts.
		/// </summary>
		public virtual void OnLocalStart(PlayerControllerB _LocalPlayer)
		{ }

		/// <summary>
		/// Called when the ship lands.
		/// </summary>
		public virtual void OnShipLand()
		{ }

		/// <summary>
		/// Called when an object is grabbed.
		/// </summary>
		public virtual void OnGrabObject(GrabbableObject obj)
		{ }

		/// <summary>
		/// Called when the local player dies.
		/// </summary>
		public virtual void OnLocalDie()
		{ }

		public virtual bool OnDamagePlayer(ref int damageNumber, ref bool hasDamageSFX, ref bool callRPC, ref CauseOfDeath causeOfDeath, ref int deathAnimation, ref bool fallDamage, ref Vector3 force)
		{ return true; }

		public virtual void OnSanityChanged(float sanity)
		{ }

		#endregion Virtuals

		public void AddSanityEvent(float sanity, Action onAct = null, Action offAct = null) => SanityEventManager.Instance.New(sanity, onAct, offAct);

		public struct Event
		{
			public float Sanity;
			public Action OnAction;
			public Action OffAction;
			public bool Applied;

			internal Event(float sanity, Action onAct = null, Action offAct = null)
			{
				Sanity = sanity;
				OnAction = onAct;
				OffAction = offAct;
				Applied = false;
			}
		}
	}
}