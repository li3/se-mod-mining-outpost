using Li3.MiningOutpost.Components;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.ModAPI;

namespace Li3.MiningOutpost.Models
{
	public class MiningOutpostUiSettings
	{
		[ProtoMember(1)]
		public long EntityId = 0;

		[ProtoMember(2)]
		public bool Enabled = true;

		public void Invoke()
		{
			IMyEntity entity;
			if (MyAPIGateway.Entities.TryGetEntityById(EntityId, out entity))
			{
				var component = entity.Components.Get<MiningOutpostUiComponent>();
				component.UpdateSettings(this);
			}
		}
	}
}
