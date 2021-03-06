﻿// ---------------------------------------------------------------------------------
// <copyright file="ConfirmLoveLockMessageEvent.cs" company="https://github.com/sant0ro/Yupi">
//   Copyright (c) 2016 Claudio Santoro, TheDoctor
// </copyright>
// <license>
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </license>
// ---------------------------------------------------------------------------------
namespace Yupi.Messages.Items
{
    using System;

    public class ConfirmLoveLockMessageEvent : AbstractHandler
    {
        #region Methods

        public override void HandleMessage(Yupi.Model.Domain.Habbo session, Yupi.Protocol.Buffers.ClientMessage request,
            Yupi.Protocol.IRouter router)
        {
            /*
            uint pId = Request.GetUInt32();
            bool confirmLoveLock = Request.GetBool();

            Room room = Session.GetHabbo().CurrentRoom;

            RoomItem item = room?.GetRoomItemHandler().GetItem(pId);
            if (item == null || item.GetBaseItem().InteractionType != Interaction.LoveShuffler)
                return;

            uint userIdOne = item.InteractingUser;
            uint userIdTwo = item.InteractingUser2;
            RoomUser userOne = room.GetRoomUserManager().GetRoomUserByHabbo(userIdOne);
            RoomUser userTwo = room.GetRoomUserManager().GetRoomUserByHabbo(userIdTwo);

            if (userOne == null && userTwo == null)
            {
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if (userOne == null)
            {
                userTwo.CanWalk = true;
                userTwo.GetClient().SendNotif("Your partner has left the room or has cancelled the love lock.");
                userTwo.LoveLockPartner = 0;
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if (userTwo == null)
            {
                userOne.CanWalk = true;
                userOne.GetClient().SendNotif("Your partner has left the room or has cancelled the love lock.");
                userOne.LoveLockPartner = 0;
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if (!confirmLoveLock)
            {
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;

                userOne.LoveLockPartner = 0;
                userOne.CanWalk = true;
                userTwo.LoveLockPartner = 0;
                userTwo.CanWalk = true;
                return;
            }

            SimpleServerMessageBuffer loock = new SimpleServerMessageBuffer(PacketLibraryManager.OutgoingHandler("LoveLockDialogueSetLockedMessageComposer"));
            loock.AppendInteger(item.Id);

            if (userIdOne == Session.GetHabbo().Id)
            {
                userOne.GetClient().Send(loock);
                userOne.LoveLockPartner = userIdTwo;
            }
            else if (userIdTwo == Session.GetHabbo().Id)
            {
                userTwo.GetClient().Send(loock);
                userTwo.LoveLockPartner = userIdOne;
            }

            // Now check if both of the users have confirmed.
            if (userOne.LoveLockPartner == 0 || userTwo.LoveLockPartner == 0)
                return;

            item.ExtraData = $"1{'\u0005'}{userOne.GetUserName()}{'\u0005'}{userTwo.GetUserName()}{'\u0005'}{userOne.GetClient().GetHabbo().Look}{'\u0005'}{userTwo.GetClient().GetHabbo().Look}{'\u0005'}{DateTime.Now.ToString("dd/MM/yyyy")}";

            userOne.LoveLockPartner = 0;
            userTwo.LoveLockPartner = 0;
            item.InteractingUser = 0;
            item.InteractingUser2 = 0;

            item.UpdateState(true, false);

            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("UPDATE items_rooms SET extra_data = @extraData WHERE id = @id");
                queryReactor.AddParameter("extraData", item.ExtraData);
                queryReactor.AddParameter("id", item.Id);
                queryReactor.RunQuery();
            }

            SimpleServerMessageBuffer messageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.OutgoingHandler("UpdateRoomItemMessageComposer"));
            item.Serialize(messageBuffer);
            room.Send(messageBuffer);

            loock = new SimpleServerMessageBuffer(PacketLibraryManager.OutgoingHandler("LoveLockDialogueCloseMessageComposer"));
            loock.AppendInteger(item.Id);
            userOne.GetClient().Send(loock);
            userTwo.GetClient().Send(loock);
            userOne.CanWalk = true;
            userTwo.CanWalk = true;

            */
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}