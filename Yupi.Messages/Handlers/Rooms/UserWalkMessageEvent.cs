﻿using System;



namespace Yupi.Messages.Rooms
{
	public class UserWalkMessageEvent : AbstractHandler
	{
		public override void HandleMessage ( Yupi.Protocol.ISession<Yupi.Model.Domain.Habbo> session, Yupi.Protocol.Buffers.ClientMessage request, Yupi.Protocol.IRouter router)
		{
			/*
			Room currentRoom = session.GetHabbo().CurrentRoom;

			RoomUser roomUserByHabbo = currentRoom?.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);

			if (roomUserByHabbo == null || !roomUserByHabbo.CanWalk)
				return;

			int targetX = request.GetInteger();
			int targetY = request.GetInteger();

			if (targetX == roomUserByHabbo.X && targetY == roomUserByHabbo.Y)
				return;

			roomUserByHabbo.MoveTo(targetX, targetY);

			if (!roomUserByHabbo.RidingHorse)
				return;

			RoomUser roomUserByVirtualId = currentRoom.GetRoomUserManager().GetRoomUserByVirtualId((int) roomUserByHabbo.HorseId);

			roomUserByVirtualId.MoveTo(targetX, targetY);
			*/
			throw new NotImplementedException ();
		}
	}
}

