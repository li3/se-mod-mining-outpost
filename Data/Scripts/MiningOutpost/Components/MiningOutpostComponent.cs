using Li3.MiningOutpost.Decorator;
using Sandbox.ModAPI;
using System;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace Li3.MiningOutpost.Components
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), true, new string[] { "Base_Mining_Outpost" })]
	public class MiningOutpostComponent : MyGameLogicComponent
	{
		private Action updater;
		private MiningOutpostComponentDecorator decorator;
		private CustomInfoUpdaterComponent info;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;

			decorator = new MiningOutpostComponentDecorator();
			updater = decorator.Decorate(Entity);
			info = Entity.Components.Get<CustomInfoUpdaterComponent>();

			(Entity as IMyTerminalBlock).AppendingCustomInfo += info.AppendingCustomInfo;

			base.Init(objectBuilder);
		}

		public override void UpdateAfterSimulation100()
		{
			updater();
			(Entity as IMyTerminalBlock).RefreshCustomInfo();

			base.UpdateAfterSimulation100();
		}

		public override void Close()
		{
			(Entity as IMyTerminalBlock).AppendingCustomInfo -= info.AppendingCustomInfo;

			base.Close();
		}
	}
}