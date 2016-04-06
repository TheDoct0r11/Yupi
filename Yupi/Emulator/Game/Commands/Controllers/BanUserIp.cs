﻿using System.Linq;
using Yupi.Emulator.Core.Io.Logger;
using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class BanUser. This class cannot be inherited.
    /// </summary>
     public sealed class BanUserIp : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BanUser" /> class.
        /// </summary>
        public BanUserIp()
        {
            MinRank = 4;
            Description = "Ban a user by IP!";
            Usage = ":ipban [USERNAME] [REASON]";
            MinParams = -1;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            GameClient user = Yupi.GetGame().GetClientManager().GetClientByUserName(pms[0]);

            if (user == null)
            {
                session.SendWhisper(Yupi.GetLanguage().GetVar("user_not_found"));
                return true;
            }
            if (user.GetHabbo().Rank >= session.GetHabbo().Rank)
            {
                session.SendWhisper(Yupi.GetLanguage().GetVar("user_is_higher_rank"));
                return true;
            }
            try
            {
                Yupi.GetGame()
                    .GetBanManager()
                    .BanUser(user, session.GetHabbo().UserName, 788922000.0, string.Join(" ", pms.Skip(2)),
                        true, false);
            }
            catch
            {
                YupiLogManager.LogException($"An error occurred when {session.GetHabbo().UserName} tried to ban {user.GetHabbo().UserName}", "Failed Banning User.", "Yupi.User");
            }

            return true;
        }
    }
}