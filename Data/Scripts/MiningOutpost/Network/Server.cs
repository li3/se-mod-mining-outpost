using Li3.MiningOutpost.Models;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace Li3.MiningOutpost.Network
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
	public class Server : MySessionComponentBase
	{
		public static readonly ushort SyncSettingsMessage = 39842;
		private bool IsInitialized = false;

		protected override void UnloadData()
		{
			try
			{
				MyAPIGateway.Multiplayer.UnregisterMessageHandler(SyncSettingsMessage, HandleSyncSettingsMessage);
			}
			catch (Exception) { }

			base.UnloadData();
		}

		public override void UpdateBeforeSimulation()
		{
			if (!IsInitialized && MyAPIGateway.Session != null)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			IsInitialized = true;
			MyAPIGateway.Multiplayer.RegisterMessageHandler(SyncSettingsMessage, HandleSyncSettingsMessage);
		}

		public static void HandleSyncSettingsMessage(byte[] raw)
		{
			try
			{
				var message = MyAPIGateway.Utilities.SerializeFromBinary<MiningOutpostUiSettings>(raw);
			}
			catch (Exception) { }
		}

		public static void SyncSettings(MiningOutpostUiSettings settings)
		{
			var message = MyAPIGateway.Utilities.SerializeToBinary(settings);
			SendToAll(SyncSettingsMessage, message);
		}

		private static void SendToAll(ushort id, byte[] message)
		{
			if (!MyAPIGateway.Multiplayer.IsServer)
				SendToServer(id, message);

			SendToPlayers(id, message);
		}

		private static void SendToServer(ushort id, byte[] message)
		{
			MyAPIGateway.Multiplayer.SendMessageToServer(id, message);
		}

		private static void SendToPlayers(ushort id, byte[] message)
		{
			List<IMyPlayer> players = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(players, p => p != null && !IsHost(p));

			players.ForEach(o => MyAPIGateway.Multiplayer.SendMessageTo(id, message, o.SteamUserId));
		}

		private static bool IsHost(IMyPlayer player)
		{
			return MyAPIGateway.Multiplayer.IsServerPlayer(player.Client);
		}
	}
}
