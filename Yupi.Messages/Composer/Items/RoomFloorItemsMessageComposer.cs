﻿using System;
using System.Collections.Generic;
using Yupi.Emulator.Game.Items.Interfaces;
using Yupi.Emulator.Game.Rooms.Data.Models;
using Yupi.Emulator.Game.Groups.Structs;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Items
{
	public class RoomFloorItemsMessageComposer : AbstractComposer <RoomData, IReadOnlyDictionary<uint, RoomItem>>
	{
		public override void Compose (Yupi.Protocol.ISender session, RoomData data, IReadOnlyDictionary<uint, RoomItem> items)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				if (data.Group != null) {
					if (data.Group.AdminOnlyDeco == 1u) {
						message.AppendInteger (data.Group.Admins.Count + 1);


						foreach (GroupMember member in data.Group.Admins.Values) {
							if (member != null) {
								message.AppendInteger (member.Id);
								message.AppendString (Yupi.GetHabboById (member.Id).UserName);
							}
						}

						message.AppendInteger (data.OwnerId);
						message.AppendString (data.Owner);
					} else {

						message.AppendInteger (data.Group.Members.Count + 1);

						foreach (GroupMember member in data.Group.Members.Values) {
							message.AppendInteger (member.Id);
							message.AppendString (Yupi.GetHabboById (member.Id).UserName);
						}
					}
				} else {
					message.AppendInteger (1);
					message.AppendInteger (data.OwnerId);
					message.AppendString (data.Owner);
				}

				message.AppendInteger (items.Count);
			
				foreach (KeyValuePair<uint, RoomItem> roomItem in items)
					roomItem.Value.Serialize (message);

				session.Send (message);
			}
		}
	}
}
