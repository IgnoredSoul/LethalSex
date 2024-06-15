using System;
using LethalSex_Core.Modules;
using System.Collections.Generic;

namespace LethalSex_Core
{
	public class LethalMod
	{
		/// <summary>
		/// The name of the mod
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The author of the mod
		/// </summary>
		public string Author { get; private set; }

		/// <summary>
		/// The mod version
		/// </summary>
		public string Version { get; private set; }

		/// <summary>
		/// The name of the assembly
		/// </summary>
		public string AssemblyName { get; private set; }

		/// <summary>
		/// The name of the mod
		/// </summary>
		/// <returns>the list of registered modules associated with the registered mod</returns>
		public List<LethalModule> Modules { get; private set; }

		/// <summary>
		/// Constructor for registering a new mod
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="modName"></param>
		/// <param name="modVersion"></param>
		/// <param name="modAuthor"></param>
		public LethalMod(string assemblyName, string modName, string modVersion, string modAuthor)
		{
			Modules = new();
			Name = modName;
			Author = modAuthor;
			Version = modVersion;
			AssemblyName = assemblyName;
		}

		public LethalModule? RegisterModule(Type module, string moduleName)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		{
			try
			{
				// Check if the module already exists
				if (Modules.Exists(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase)))
				{
					Main.mls.LogError($"[{moduleName}] already exists in [{Name}]");
					return null;
				}

				// Create a new instance
				LethalModule newModule = (LethalModule)Activator.CreateInstance(module);

				// Set parent mod
				newModule.Mod = this;
				newModule.ModuleName = moduleName;

				// Add to list
				Modules.Add(newModule);

				// Tell mod its now registered
				newModule.OnRegister();

				// Log
				Main.mls.LogWarning($"[{Name}] loaded module of: [{moduleName}]");
				ConsoleManager.Warn($"[{Name}] loaded module of: [{moduleName}]");

				return newModule;
			}
			catch (Exception e)
			{
				Main.mls.LogError($"[{Name}] failed to load module of: [{moduleName}]\n{e}\n");
				ConsoleManager.Error($"[{Name}] failed to load module of: [{moduleName}]\n{e}\n");
				return null;
			}
		}

		public LethalModule? RegisterModule(Type module) => RegisterModule(module, module.Name);

		public void UnregisterModule(string moduleName)
		{
			if (Modules.Exists(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase)))
			{
				Modules.ForEach((m) =>
				{
					if (string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
						UnregisterModule(m);
				});
			}
			else
			{
				Main.mls.LogError($"Could not unregister [{moduleName}] since it cannot be found");
				ConsoleManager.Error($"Could not unregister [{moduleName}] since it cannot be found");
			}
		}

		public void UnregisterModule(Type module)
		{
			foreach (LethalModule _module in Modules)
				if (_module.GetType() == module)
					UnregisterModule(_module);
			Main.mls.LogError($"Could not unregister [{module.Name}] since it cannot be found");
		}

		public void UnregisterModule(LethalModule module)
		{
			try
			{
				if (Modules.Contains(module))
				{
					module.Module = null;
					Modules.Remove(module);
					Main.mls.LogWarning($"[{module.ModuleName}] has been unregistered");
				}
				else Main.mls.LogError($"Could not unregister [{module.ModuleName}] since it cannot be found");
			}
			catch (Exception e) { Main.mls.LogError($"Failed to unregister a module from [{module.ModuleName}]\n{e}\n"); ConsoleManager.Error($"Failed to unregister a module from  [{module.ModuleName}]\n{e}\n"); }
		}
	}
}