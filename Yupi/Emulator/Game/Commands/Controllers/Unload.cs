﻿using System.Collections.Generic;
using System.Linq;
using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms.User;
using Yupi.Emulator.Messages;
using Yupi.Emulator.Messages.Buffers;
using Yupi.Emulator.Messages.Parsers;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class Unload. This class cannot be inherited.
    /// </summary>
    internal sealed class Unload : Command
    {
        /// <summary>
        ///     The _re enter
        /// </summary>
        private readonly bool _reEnter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Unload" /> class.
        /// </summary>
        /// <param name="reEnter">if set to <c>true</c> [re enter].</param>
        public Unload(bool reEnter = false)
        {
            MinRank = -1;
            Description = "Unloads the current room!";
            Usage = reEnter ? ":reload" : ":unload";
            MinParams = 0;
            _reEnter = reEnter;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            uint roomId = session.GetHabbo().CurrentRoom.RoomId;
            List<RoomUser> users = new List<RoomUser>(session.GetHabbo().CurrentRoom.GetRoomUserManager().UserList.Values);

            Yupi.GetGame().GetRoomManager().UnloadRoom(session.GetHabbo().CurrentRoom, "Unload command");

            if (!_reEnter)
                return true;

            Yupi.GetGame().GetRoomManager().LoadRoom(roomId);

            SimpleServerMessageBuffer roomFwd = new SimpleServerMessageBuffer(PacketLibraryManager.OutgoingRequest("RoomForwardMessageComposer"));
            roomFwd.AppendInteger(roomId);

            byte[] data = roomFwd.GetReversedBytes();

            foreach (RoomUser user in users.Where(user => user != null && user.GetClient() != null))
                user.GetClient().SendMessage(data);
            return true;
        }
    }
}