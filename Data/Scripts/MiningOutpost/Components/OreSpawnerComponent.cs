using Sandbox.Game;
using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace Li3.MiningOutpost.Components
{
	public class OreSpawnerComponent : MyEntityComponentBase
	{
		private static Func<int> AmountZero = () => 0;
		public Func<int> CalculateAmountToSpawn { get; set; }

		public OreSpawnerComponent()
		{
			CalculateAmountToSpawn = AmountZero;
		}

		public void Update()
		{
			var oreFinder = Entity.Components.Get<DirectionalOreFinderComponent>();
			var inventory = Entity.GetInventory();

			if (ComponentsAreReady(oreFinder, inventory))
			{
				if (oreFinder.OresDetected.Count > 0)
				{
					var amount = CalculateAmountToSpawn();
					if (amount > 0)
					{
						MyVisualScriptLogicProvider.ShowNotification(string.Format("Amount: {0}", amount), 2000);
						SpawnOresIntoInventory(inventory, oreFinder.OresDetected, amount);
					}
				}
			}
		}

		private static bool ComponentsAreReady(DirectionalOreFinderComponent oreFinder, IMyInventory inventory)
		{
			return oreFinder != null && inventory != null;
		}

		private void SpawnOresIntoInventory(IMyInventory inventory, List<MyObjectBuilder_Ore> builders, int amount)
		{
			builders.ForEach(builder => {
				if (!inventory.IsFull)
					inventory.AddItems(amount, builder);
			});
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
