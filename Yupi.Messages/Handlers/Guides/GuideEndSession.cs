﻿using System;
using Yupi.Model.Domain;


namespace Yupi.Messages.Guides
{
	// TODO Rename
	public class GuideEndSession : AbstractHandler
	{
		public override void HandleMessage ( Yupi.Model.Domain.Habbo session, Yupi.Protocol.Buffers.ClientMessage message, Yupi.Protocol.IRouter router)
		{
			Habbo requester = session.GuideOtherUser;

			// TODO Test & Fixme !!!

			router.GetComposer<OnGuideSessionDetachedMessageComposer> ().Compose (requester, 2);
			router.GetComposer<OnGuideSessionDetachedMessageComposer> ().Compose (session, 0);

			requester.GuideOtherUser = null;
			session.GuideOtherUser = null;
		}
	}
}

