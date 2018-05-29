using Li3.MiningOutpost.Models;
using VRage.Game.Components;
using VRage.ModAPI;

namespace Li3.MiningOutpost.Components
{
	public class MiningOutpostUiComponent : MyEntityComponentBase
	{
		public MiningOutpostUiSettings Settings { get; }

		public MiningOutpostUiComponent()
		{
			Settings = new MiningOutpostUiSettings();
		}

		public void Init(IMyEntity entity)
		{
		}

		public void UpdateSettings(MiningOutpostUiSettings Settings)
		{
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
