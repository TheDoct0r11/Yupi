﻿using System;
using System.Collections.Generic;
using Yupi.Emulator.Game.SoundMachine;
using Yupi.Emulator.Game.SoundMachine.Songs;

namespace Yupi.Messages.Music
{
	public class GetMusicDataMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Emulator.Game.GameClients.Interfaces.GameClient session, Yupi.Protocol.Buffers.ClientMessage message, Router router)
		{
			int count = message.GetInteger();

			List<SongData> songs = new List<SongData>();

			for (int i = 0; i < count; i++)
			{
				SongData song = SoundMachineSongManager.GetSong(message.GetUInt32());

				if (song != null)
					songs.Add(song);
			}

			router.GetComposer<SongsMessageComposer>().Compose(session, songs);
		}
	}
}
