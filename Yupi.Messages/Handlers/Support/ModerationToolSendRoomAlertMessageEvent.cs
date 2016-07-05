﻿using System;

using Yupi.Messages.Notification;

namespace Yupi.Messages.Support
{
	public class ModerationToolSendRoomAlertMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Emulator.Game.GameClients.Interfaces.GameClient session, Yupi.Protocol.Buffers.ClientMessage request, Yupi.Protocol.IRouter router)
		{
			if (!session.GetHabbo().HasFuse("fuse_alert"))
				return;

			// TODO Unused
			request.GetInteger();

			string message = request.GetString();

			Yupi.Messages.Rooms room = session.GetHabbo().CurrentRoom;

			router.GetComposer<SuperNotificationMessageComposer> ().Compose (room, "", message, "", "", "admin", 3);
		}
	}
}

