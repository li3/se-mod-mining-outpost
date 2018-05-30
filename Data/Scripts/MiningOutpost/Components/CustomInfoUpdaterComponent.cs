using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System.Text;
using VRage.Game;
using VRage.Game.Components;

namespace Li3.MiningOutpost.Components
{
	public class CustomInfoUpdaterComponent : MyEntityComponentBase
	{
		private const int UPDATES_PER_HOUR = (60 * 60 * 60) / 100;
		private readonly MyDefinitionId ElectricityId = MyResourceDistributorComponent.ElectricityId;

		private MyResourceSinkComponentBase ResourceSink;
		private DirectionalOreFinderComponent Finder;
		private OreSpawnerComponent Spawner;
		private MiningOutpostSettingsReader Reader;

		public string Message { get; set; }

		public void Init(
				MyResourceSinkComponentBase sink, 
				DirectionalOreFinderComponent finder, 
				OreSpawnerComponent spawner,
				MiningOutpostSettingsReader settings
			)
		{
			ResourceSink = sink;
			Finder = finder;
			Spawner = spawner;
			Reader = settings;
		}

		public void AppendingCustomInfo(IMyTerminalBlock block, StringBuilder builder)
		{
			var amount = Spawner.CalculateAmountToSpawn();

			builder.AppendFormat("Target Power Utilization: {0} MW\n", Reader.Settings.PowerUtilization);
			builder.AppendFormat("Required Input: {0} MW\n",  ResourceSink.RequiredInputByType(ElectricityId));
			builder.AppendLine("\nExtracting:");
			foreach (var kvp in Spawner.AmountsSpawnedLastUpdate)
			{
				builder.AppendFormat("  {0}: {1:F2} kg/hour\n", kvp.Key, kvp.Value * UPDATES_PER_HOUR);
			}

		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);

		public object RequiredInput { get; private set; }
	}
}
