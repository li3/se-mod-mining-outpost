using Li3.MiningOutpost.Models;
using System.Collections.Generic;

namespace Li3.MiningOutpost.Utilities
{
	public class SettingsCache
	{
		private static SettingsCache instance = null;
		public Dictionary<long, MiningOutpostUiSettings> Data { get; set; }

		public static SettingsCache Instance
		{
			get
			{
				if (instance == null)
					instance = new SettingsCache();

				return instance;
			}
		}

		private SettingsCache()
		{
			Data = new Dictionary<long, MiningOutpostUiSettings>();
			SettingsManager.PopulateCacheFromDisk(this);
		}
	}
}
