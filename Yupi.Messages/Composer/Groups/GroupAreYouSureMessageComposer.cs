﻿using System;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Groups
{
	public class GroupAreYouSureMessageComposer : AbstractComposer<uint>
	{
		public override void Compose (Yupi.Protocol.ISender session, uint userId)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendInteger(userId);
				message.AppendInteger(0);
				session.Send (message);
			}
		}
	}
}
