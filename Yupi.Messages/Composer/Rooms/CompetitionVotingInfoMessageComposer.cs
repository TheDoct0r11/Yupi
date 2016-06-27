﻿using System;
using Yupi.Emulator.Game.Rooms.Competitions.Models;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Rooms
{
	public class CompetitionVotingInfoMessageComposer : AbstractComposer<RoomCompetition, int, int>
	{
		public override void Compose (Yupi.Protocol.ISender session, RoomCompetition competition, int userVotes, int status = 0)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendInteger(competition.Id);
				message.AppendString(competition.Name);
				message.AppendInteger(status); // 0 : vote - 1 : can't vote - 2 : you need the vote badge
				message.AppendInteger(userVotes);
				session.Send (message);
			}
		}
	}
}
