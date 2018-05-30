using Li3.MiningOutpost.Models;
using System;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace Li3.MiningOutpost.Components
{
	public class OreSpawnerComponent : MyEntityComponentBase
	{
		public const float ORE_TO_SPAWN_PER_HOUR = 100000f;
		public const float TICKS_PER_HOUR = 60 * 60 * 60;

		private static Func<float> AmountZero = () => 0f;
		public Func<float> CalculateAmountToSpawn { get; set; }
		public Dictionary<string, float> AmountsSpawnedLastUpdate { get; private set; }

		public OreSpawnerComponent()
		{
			CalculateAmountToSpawn = AmountZero;
			AmountsSpawnedLastUpdate = new Dictionary<string, float>();
		}

		public void Update()
		{
			AmountsSpawnedLastUpdate.Clear();
			var oreFinder = Entity.Components.Get<DirectionalOreFinderComponent>();
			var inventory = Entity.GetInventory();

			if (ComponentsAreReady(oreFinder, inventory))
			{
				if (oreFinder.OresDetected.Count > 0)
				{
					var amount = CalculateAmountToSpawn();
					if (amount > 0)
					{
						SpawnOresIntoInventory(inventory, oreFinder.OresDetected, amount);
					}
				}
			}
		}

		private static bool ComponentsAreReady(DirectionalOreFinderComponent oreFinder, IMyInventory inventory)
		{
			return oreFinder != null && inventory != null;
		}

		private void SpawnOresIntoInventory(IMyInventory inventory, List<VoxelDefinitionAndBuilder> voxels, float amount)
		{
			voxels.ForEach(voxel => {
				if (!inventory.IsFull) {
					var amountToAdd = (MyFixedPoint)amount * voxel.Definition.MinedOreRatio;
					if (inventory.CanItemsBeAdded(amountToAdd, voxel.Builder.GetId()))
					{
						AmountsSpawnedLastUpdate.Add(voxel.Definition.MinedOre, (float)amountToAdd);
						inventory.AddItems(amountToAdd, voxel.Builder);
					}
				}
			});
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
