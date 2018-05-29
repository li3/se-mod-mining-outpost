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

namespace Li3.MiningOutpost.Factories
{

	public class MiningOutpostComponentDecorator
	{
		private MyDefinitionId ElectricityId = MyResourceDistributorComponent.ElectricityId;
		public IMyEntity Entity { get; private set; }

		public Action Decorate(IMyEntity entity)
		{
			Entity = entity;

			Func<float> requiredPower = () => 0.1f;

			var sink = AddResourceSinkToEntity(requiredPower);
			var inventory = AddInventoryToEntity();
			var conditional = AddConditionalToEntity();
			var ui = AddMiningOutpostUiComponent();
			var lights = AddWorkingLightsComponent();
			var scanner = AddDirectionalOreFinderComponent();
			var spawner = AddOreSpawnerComponent();

			SetupConditionalSubscribers(requiredPower, sink, conditional, ui, lights, scanner);

			return () =>
			{
				sink.Update();
				conditional.Update();
				scanner.Update();
				spawner.Update();
			};
		}

		private void SetupConditionalSubscribers(Func<float> requiredPower, MyResourceSinkComponent sink, ConiditonalComponent conditional, MiningOutpostUiComponent ui, WorkingLightsComponent lights, DirectionalOreFinderComponent scanner)
		{
			conditional.Conditions.Add((e) => sink.IsPowerAvailable(ElectricityId, requiredPower()));
			conditional.Conditions.Add((e) => ui.Settings.Enabled);

			conditional.Subscribers.Add((p, n) => scanner.ShouldDoWork = n);
			conditional.Subscribers.Add((p, n) => lights.SetState((n) ? WorkingLightsComponent.LightState.On : WorkingLightsComponent.LightState.Off));
		}

		private MyResourceSinkComponent AddResourceSinkToEntity(Func<float> requiredPower)
		{
			var resourceSink = new MyResourceSinkComponent();
			var resourceSinkInfo = new MyResourceSinkInfo()
			{
				ResourceTypeId = ElectricityId,
				MaxRequiredInput = 0.2f,
				RequiredInputFunc = requiredPower
			};
			Entity.Components.Add(resourceSink);
			resourceSink.Init(MyStringHash.GetOrCompute("Utility"), resourceSinkInfo);

			return resourceSink;
		}

		private IMyInventory AddInventoryToEntity()
		{
			if (Entity.GetInventory() == null)
			{
				var inventory = new MyInventory(1.0f, new Vector3(1.0f), MyInventoryFlags.CanSend);
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

		private MiningOutpostUiComponent AddMiningOutpostUiComponent()
		{
			var ui = new MiningOutpostUiComponent();
			Entity.Components.Add(ui);

			ui.Init(Entity);

			return ui;
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

		private OreSpawnerComponent AddOreSpawnerComponent()
		{
			var oreSpawner = new OreSpawnerComponent();
			Entity.Components.Add(oreSpawner);

			oreSpawner.CalculateAmountToSpawn = () => 1;

			return oreSpawner;
		}
	}
}
