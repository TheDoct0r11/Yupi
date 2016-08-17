﻿using System;
using Yupi.Controller;
using Yupi.Model.Domain;
using Yupi.Messages.Contracts;

namespace Yupi.Messages.Navigator
{
	public class EnterPrivateRoomMessageEvent : AbstractHandler
	{
		private RoomManager RoomManager;

		public override void HandleMessage ( Yupi.Model.Domain.Habbo session, Yupi.Protocol.Buffers.ClientMessage request, Yupi.Protocol.IRouter router)
		{
			int roomId = request.GetInteger();

			string pWd = request.GetString();

			if (session.Room != null) {
				RoomManager.RemoveUser (session.RoomEntity);
			}

			Room room = RoomManager.LoadOrGet (roomId);

			if (room == null) {
				return;
			}

			if (room.GetUserCount() >= room.Data.UsersMax
			    && !session.Info.HasPermission ("fuse_enter_full_rooms")
			    && room.Data.Owner != session.Info) {

				/// TODO Remove on other room load / disconnect
				room.Queue.Add (session.Info);

				router.GetComposer<RoomQueueComposer> ().Compose (session, room.Queue.Count);
			} else if (!session.Info.HasPermission ("fuse_enter_any_room")
			          && room.Data.BannedUsers.Contains (session.Info)) {
				router.GetComposer<RoomEnterErrorMessageComposer> ().Compose (session, RoomEnterErrorMessageComposer.Error.BANNED);
				router.GetComposer<OutOfRoomMessageComposer> ().Compose (session);
			} else {
				router.GetComposer<PrepareRoomMessageComposer> ().Compose (session);

				bool isReload = false;
				// TODO Extract to controller!

				if (!isReload
				    && !session.Info.HasPermission ("fuse_enter_any_room")
					&& !room.HasOwnerRights (session.Info)
					&& session.TeleportingTo != room.Data) {

					switch (room.Data.State) {
					case RoomState.BELL:

						if (room.GetUserCount () == 0) {
							router.GetComposer<DoorbellNoOneMessageComposer> ().Compose (session);
						} else {
							// TODO String.Empty == 'I am ringing'
							router.GetComposer<DoorbellMessageComposer> ().Compose (session, string.Empty);

							foreach (Habbo user in room.GetSessions()) {
								if (room.HasRights (user.Info)) {
									user.Router.GetComposer<DoorbellMessageComposer> ().Compose (user, session.Info.UserName);
								}
							}
						}

						return;

					case RoomState.LOCKED:
						if (pWd != room.Data.Password) {
							router.GetComposer<RoomErrorMessageComposer> ().Compose (session, -100002);
							router.GetComposer<OutOfRoomMessageComposer> ().Compose (session);
							return;
						}
						break;
					}
				}

				room.GroupsInRoom.Add (session.Info.FavouriteGroup);

				router.GetComposer<RoomGroupMessageComposer> ().Compose (session, room.GroupsInRoom);
				router.GetComposer<InitialRoomInfoMessageComposer> ().Compose (session, room.Data);

				if (session.Info.SpectatorMode) {
					router.GetComposer<SpectatorModeMessageComposer> ().Compose (session);
				}

				router.GetComposer<RoomSpacesMessageComposer> ()
					.Compose (session, RoomSpacesMessageComposer.RoomSpacesType.Wallpaper, room.Data);
				
				router.GetComposer<RoomSpacesMessageComposer> ()
					.Compose (session, RoomSpacesMessageComposer.RoomSpacesType.Floor, room.Data);
				
				router.GetComposer<RoomSpacesMessageComposer> ()
					.Compose (session, RoomSpacesMessageComposer.RoomSpacesType.Landscape, room.Data);

				// TODO Magic numbers!
				int rightsLevel = 0;

				if (room.HasOwnerRights (session.Info)) {
					rightsLevel = 4;
					router.GetComposer<HasOwnerRightsMessageComposer> ().Compose (session);
				} else if (room.HasRights (session.Info)) {
					rightsLevel = 1;
				}

				router.GetComposer<RoomRightsLevelMessageComposer> ().Compose (session, rightsLevel);
				router.GetComposer<RoomRatingMessageComposer> ().Compose (session, room.Data.Score, room.CanVote(session.Info));
				router.GetComposer<RoomUpdateMessageComposer> ().Compose (session, room.Data.Id);

				session.Info.RecentlyVisitedRooms.Add (room.Data);
				session.Room = room;
				// TODO Add room entity?
			}
		}
	}
}

