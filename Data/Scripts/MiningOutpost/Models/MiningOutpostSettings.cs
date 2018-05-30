using ProtoBuf;

namespace Li3.MiningOutpost.Models
{
	public class MiningOutpostSettings
	{
		[ProtoMember(2)]
		public bool Enabled = true;

		[ProtoMember(3)]
		public float PowerUtilization = 1.0f;
	}
}
