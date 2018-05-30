using Li3.MiningOutpost.Models;
using Sandbox.ModAPI;
using System;
using VRage.Game.Components;

namespace Li3.MiningOutpost.Components
{
	public class MiningOutpostSettingsReader : MyEntityComponentBase
	{
		public const float MAX_MEGAWATTS = 1000.0f;

		public MiningOutpostSettings Settings { get; }

		public MiningOutpostSettingsReader()
		{
			Settings = new MiningOutpostSettings();
		}

		public void Update() {
			try
			{
				ReadInSettings();
			}
			catch (Exception)
			{
				WriteSettingsToCustomData();
			}
		}

		private void WriteSettingsToCustomData()
		{
			var buffer = MyAPIGateway.Utilities.SerializeToXML(Settings);
			(Entity as IMyTerminalBlock).CustomData = buffer;
		}

		private void ReadInSettings()
		{
			var buffer = (Entity as IMyTerminalBlock).CustomData;

			if (string.IsNullOrWhiteSpace(buffer))
				WriteSettingsToCustomData();

			var deserialized = MyAPIGateway.Utilities.SerializeFromXML<MiningOutpostSettings>(buffer);
			UpdateSettings(deserialized);
		}

		public void UpdateSettings(MiningOutpostSettings other)
		{
			Settings.Enabled = other.Enabled;
			Settings.PowerUtilization = Clamp(other.PowerUtilization, 0.0f, MAX_MEGAWATTS);
		}

		private float Clamp(float value, float min, float max)
		{
			return Math.Max(Math.Min(value, max), min);
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
