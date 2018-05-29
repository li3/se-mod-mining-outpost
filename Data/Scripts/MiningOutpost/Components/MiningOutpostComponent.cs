using Li3.MiningOutpost.Factories;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using System;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace Li3.MiningOutpost.Components
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_ControlPanel), true, new string[] { "Base_Mining_Outpost" })]
	public class MiningOutpostComponent : MyGameLogicComponent
	{
		private Action updater;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;

			MyVisualScriptLogicProvider.ShowNotification("Init", 2000);
			updater = new MiningOutpostComponentDecorator().Decorate(Entity);

			base.Init(objectBuilder);
		}

		public override void UpdateAfterSimulation100()
		{
			MyVisualScriptLogicProvider.ShowNotification("Tick", 2000);
			updater();
			base.UpdateAfterSimulation100();
		}
	}
}