﻿using System;

namespace Yupi.Messages.User
{
	public class GetHotelViewHallOfFameMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Emulator.Game.GameClients.Interfaces.GameClient session, Yupi.Protocol.Buffers.ClientMessage message, Yupi.Protocol.IRouter router)
		{
			string code = message.GetString();
		}
	}
}

