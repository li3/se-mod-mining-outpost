using Li3.MiningOutpost.Components;
using Li3.MiningOutpost.Models;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.ModAPI;

namespace Li3.MiningOutpost.Utilities
{
	public class SettingsManager
	{
		const string SettingsVariableKey = "Li3.MiningOutpost.Settings";

		public static void Sync(IMyEntity entity, SettingsCache cache)
		{
			if (cache.Data.ContainsKey(entity.EntityId))
				return;

			var settings = entity.Components.Get<MiningOutpostUiComponent>().Settings;
			if (settings == null)
				return;

			settings.EntityId = entity.EntityId;
			cache.Data.Add(entity.EntityId, settings);
		}

		public static void Remove(IMyEntity entity, SettingsCache cache)
		{
			cache.Data.Remove(entity.EntityId);
		}

		public static void SaveToDisk(SettingsCache cache)
		{
			var data = MyAPIGateway.Utilities.SerializeToXML(cache.Data);
			MyAPIGateway.Utilities.SetVariable(SettingsVariableKey, data);
		}

		public static void PopulateCacheFromDisk(SettingsCache cache)
		{
			string data;
			MyAPIGateway.Utilities.GetVariable(SettingsVariableKey, out data);

			var settings = MyAPIGateway.Utilities.SerializeFromXML<Dictionary<long, MiningOutpostUiSettings>>(data);
			cache.Data.Clear();

			foreach (var kvp in settings)
				cache.Data.Add(kvp.Key, kvp.Value);
		}

		public static MiningOutpostUiSettings GetSettingsForEntity(IMyEntity entity, SettingsCache cache)
		{
			if (cache.Data.ContainsKey(entity.EntityId))
			{
				return cache.Data[entity.EntityId];
			}

			return new MiningOutpostUiSettings();
		}
	}
}
