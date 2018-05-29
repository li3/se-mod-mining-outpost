using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Li3.MiningOutpost
{
	class MiningOutpostCustomInfo
	{
		private string Message;
		private float Power;

		public MiningOutpostCustomInfo()
		{
		}

		public void SetMessage(string message)
		{
			Message = message;
		}

		public void ClearMessage()
		{
			Message = "";
		}

		public void SetPowerNeeded(float power)
		{
			Power = power;
		}

		public bool HasError()
		{
			return !String.IsNullOrWhiteSpace(Message);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendFormat("Required Input: {0:F2} kW", Power * 1000f); 

			if (HasError())
			{
				builder.AppendFormat("Error: {0}\n", Message);
			}

			return builder.ToString();
		}
	}
}
