using System;
using System.Linq;
using System.Collections.Generic;

namespace LethalSex_Core.Modules
{
	internal class SanityEventManager : LethalModule
	{
		public static SanityEventManager Instance { get; private set; }

		public override void OnRegister() => Instance = this;

		protected List<Event> events = new List<Event>();

		public void New(float sanity, Action onAct = null, Action offAct = null) => events.Add(new Event(sanity, onAct, offAct));

		public override void OnSanityChanged(float sanity)
		{
			// Items that are in the range of ± 3 of sanity
			// var inboundItems = events.Where(i => i.Sanity > sanity - 3 && i.Sanity < sanity + 3).ToList();

			// Events that are below the sanity
			foreach (var item in events.Where(i => i.Sanity < sanity).ToArray())
			{
				// Get index position
				int index = events.IndexOf(item);

				// If valid and has not been applied
				if (index != -1 && !events[index].Applied)
				{
					// Copy
					var modifiedEvent = events[index];

					// Set as applied
					modifiedEvent.Applied = true;

					// Set
					events[index] = modifiedEvent;

					// Invoke
					modifiedEvent.OnAction?.Invoke();
				}
			}

			// Events that are above the sanity
			foreach (var item in events.Where(i => i.Sanity >= sanity).ToArray())
			{
				// Get index position
				int index = events.IndexOf(item);

				// If valid and has been applied
				if (index != -1 && events[index].Applied)
				{
					// Copy
					var modifiedEvent = events[index];

					// Set as applied
					modifiedEvent.Applied = false;

					// Set
					events[index] = modifiedEvent;

					// Invoke
					modifiedEvent.OffAction?.Invoke();
				}
			}
		}
	}
}