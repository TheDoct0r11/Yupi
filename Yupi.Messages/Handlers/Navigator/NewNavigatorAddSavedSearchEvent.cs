﻿using System;
using Yupi.Messages.Contracts;
using Yupi.Util;
using Yupi.Model.Domain;
using Yupi.Model.Repository;
using Yupi.Model;


namespace Yupi.Messages.Navigator
{
	public class NewNavigatorAddSavedSearchEvent : AbstractHandler
	{
		private Repository<UserInfo> UserRepository;

		public NewNavigatorAddSavedSearchEvent ()
		{
			UserRepository = DependencyFactory.Resolve<Repository<UserInfo>> ();
		}

		public override void HandleMessage ( Yupi.Protocol.ISession<Yupi.Model.Domain.Habbo> session, Yupi.Protocol.Buffers.ClientMessage request, Yupi.Protocol.IRouter router)
		{
			// TODO Refactor
			string value1 = request.GetString();

			string value2 = request.GetString();

			UserSearchLog naviLog = new UserSearchLog() {
				Value1 = value1,
				Value2 = value2
			};
					
			session.UserData.Info.NavigatorLog.Add(naviLog);

			UserRepository.Save (session.UserData.Info);

			router.GetComposer<NavigatorSavedSearchesComposer> ().Compose (session, session.UserData.Info.NavigatorLog);
		}
	}
}

