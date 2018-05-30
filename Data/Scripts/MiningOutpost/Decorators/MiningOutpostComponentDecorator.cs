using System;
using Li3.MiningOutpost.Components;
using Sandbox.Game;
using Sandbox.Game.EntityComponents;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace Li3.MiningOutpost.Decorator
{

	public class MiningOutpostComponentDecorator
	{
		public IMyEntity Entity { get; private set; }

		private MyDefinitionId ElectricityId = MyResourceDistributorComponent.ElectricityId;

		public Action Decorate(IMyEntity entity)
		{
			Entity = entity;
			var settings = AddMiningOutpostSettingsReader();
			MyResourceSinkInfo resourceSinkInfo = MakeSinkInfo(settings);

			var sink = AddResourceSinkToEntity(resourceSinkInfo);
			var inventory = AddInventoryToEntity();
			var conditional = AddConditionalToEntity();
			var lights = AddWorkingLightsComponent();
			var finder = AddDirectionalOreFinderComponent();
			var spawner = AddOreSpawnerComponent(resourceSinkInfo);
			var info = AddCustomInfoUpdaterComponent(sink, finder, spawner, settings);

			SetupConditionalSubscribers(resourceSinkInfo, sink, conditional, settings, lights, finder);

			return () =>
			{
				settings.Update();
				sink.Update();
				conditional.Update();
				finder.Update();
				spawner.Update();
			};
		}

		private CustomInfoUpdaterComponent AddCustomInfoUpdaterComponent(MyResourceSinkComponent sink, DirectionalOreFinderComponent finder, OreSpawnerComponent spawner, MiningOutpostSettingsReader settings)
		{
			var info = new CustomInfoUpdaterComponent();
			info.Init(sink, finder, spawner, settings);

			Entity.Components.Add(info);

			return info;
		}

		private MyResourceSinkInfo MakeSinkInfo(MiningOutpostSettingsReader settings)
		{
			return new MyResourceSinkInfo()
			{
				ResourceTypeId = ElectricityId,
				MaxRequiredInput = MiningOutpostSettingsReader.MAX_MEGAWATTS,
				RequiredInputFunc = () => settings.Settings.PowerUtilization
			};
		}

		private void SetupConditionalSubscribers(MyResourceSinkInfo resourceSinkInfo, MyResourceSinkComponent sink, ConiditonalComponent conditional, MiningOutpostSettingsReader settings, WorkingLightsComponent lights, DirectionalOreFinderComponent scanner)
		{
			conditional.Conditions.Add((e) => settings.Settings.Enabled);
			conditional.Conditions.Add((e) => sink.IsPowerAvailable(ElectricityId, resourceSinkInfo.RequiredInputFunc()));

			conditional.Subscribers.Add((p, n) => scanner.ShouldDoWork = n);
			conditional.Subscribers.Add((p, n) => lights.SetState((n) ? WorkingLightsComponent.LightState.On : WorkingLightsComponent.LightState.Off));
		}

		private MyResourceSinkComponent AddResourceSinkToEntity(MyResourceSinkInfo resourceSinkInfo)
		{
			var resourceSink = new MyResourceSinkComponent();
			resourceSink.Init(MyStringHash.GetOrCompute("Utility"), resourceSinkInfo);

			Entity.Components.Add(resourceSink);

			return resourceSink;
		}

		private IMyInventory AddInventoryToEntity()
		{
			if (Entity.GetInventory() == null)
			{
				var inventory = new MyInventory(1.0f, new Vector3(1.0f), MyInventoryFlags.CanSend | MyInventoryFlags.CanReceive);
				Entity.Components.Add<MyInventoryBase>(inventory);

				return inventory;
			}

			return Entity.GetInventory();
		}

		private ConiditonalComponent AddConditionalToEntity()
		{
			var conditional = new ConiditonalComponent();
			Entity.Components.Add(conditional);

			conditional.Init(Entity, "IsWorking");

			return conditional;
		}

		private MiningOutpostSettingsReader AddMiningOutpostSettingsReader()
		{
			var settings = new MiningOutpostSettingsReader();
			Entity.Components.Add(settings);

			return settings;
		}

		private WorkingLightsComponent AddWorkingLightsComponent()
		{
			var worklights = new WorkingLightsComponent();
			Entity.Components.Add(worklights);

			worklights.Init(Entity, new string[] { "Emissive" });

			return worklights;
		}

		private DirectionalOreFinderComponent AddDirectionalOreFinderComponent()
		{
			var directionalOreFinder = new DirectionalOreFinderComponent();
			Entity.Components.Add(directionalOreFinder);

			directionalOreFinder.Distance = 10f;
			directionalOreFinder.ScanDirection = new Vector3D(0, -1, 0);

			return directionalOreFinder;
		}

		private OreSpawnerComponent AddOreSpawnerComponent(MyResourceSinkInfo resourceSinkInfo)
		{
			var oreSpawner = new OreSpawnerComponent();
			Entity.Components.Add(oreSpawner);

			oreSpawner.CalculateAmountToSpawn = () =>
			{
				var power = resourceSinkInfo.RequiredInputFunc();
				var ratio = power / MiningOutpostSettingsReader.MAX_MEGAWATTS;
				var multiplier = Logistics(ratio);
				var orePerHourToExtract = OreSpawnerComponent.ORE_TO_SPAWN_PER_HOUR * multiplier;
				var orePerTick = orePerHourToExtract / OreSpawnerComponent.TICKS_PER_HOUR;
				var orePerUpdate = orePerTick * 100;

				return orePerUpdate;
			};

			return oreSpawner;
		}

		private static float Logistics(float signal, float A = -1f, float K = 1f, float B = 2f, float v = 1f, float Q = 1f, float C = 1f)
		{
			var top = K - A;
			var exponent = Math.Exp(-B * signal);
			var bottom = Math.Pow(C + Q * exponent, 1 / v);

			return (float)(A + top / bottom);
		}
	}
}
