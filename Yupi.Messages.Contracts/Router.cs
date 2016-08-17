﻿using System;
using System.Linq;
using System.Collections.Generic;
using Yupi.Protocol.Buffers;
using Yupi.Net;
using Yupi.Protocol;
using Yupi.Model.Domain;
using Yupi.Messages.Contracts;
using System.IO;
using System.Reflection;

namespace Yupi.Messages
{
	public class Router : Yupi.Protocol.IRouter
	{
		// TODO Remove
		public static Router Default;

		private static readonly log4net.ILog Logger = log4net.LogManager
			.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Dictionary<short, AbstractHandler> Incoming;
		private Dictionary<Type, IComposer> Outgoing;

		private PacketLibrary library;
		private ServerMessagePool pool;

		private Assembly MessageAssembly;

		public Router (string release, string configDir, Assembly messageAssembly)
		{
			library = new PacketLibrary (release, configDir);
			pool = new ServerMessagePool ();

			MessageAssembly = messageAssembly;
			LoadHandlers ();
			LoadComposers ();
		}

		public T GetComposer<T>() {
			IComposer composer;
			Outgoing.TryGetValue (typeof(T), out composer);

			if (composer == null) {
				Logger.ErrorFormat ("Invalid composer {0}", typeof(T).Name);
			}

			return (T)composer;
		}

		public void Handle (Habbo session, ClientMessage message) {
			AbstractHandler handler;
			Incoming.TryGetValue (message.Id, out handler);

			if (handler == null) {
				Logger.WarnFormat ("Unknown incoming message {0}", message.Id);
			} else {
				if (Logger.IsDebugEnabled) {
					Logger.DebugFormat ("Handle {0} for [{1}]", handler.GetType ().Name, session.Session.RemoteAddress);
				}
				handler.HandleMessage (session, message, this);
			}
		}
		// TODO Fix handler names in *.incoming
		private void LoadHandlers() {
			Incoming = new Dictionary<short, AbstractHandler> ();

			IEnumerable<Type> handlers = GetImplementing <AbstractHandler>();

			foreach (Type handlerType in handlers) {
				short id = library.GetIncomingId (handlerType.Name);

				if (id != 0) {
					AbstractHandler handler = (AbstractHandler)Activator.CreateInstance(handlerType);
					Incoming.Add (id, handler);
				}
			}
		}

		private void LoadComposers() {
			Outgoing = new Dictionary<Type, IComposer> ();

			IEnumerable<Type> composers = GetImplementing <IComposer>();

			foreach (Type composerType in composers) {
				short id = library.GetOutgoingId (composerType.Name);

				if (id != 0) {
					IComposer composer = (IComposer)Activator.CreateInstance (composerType);
					composer.Init (id, pool);

					// TODO Remove one Add
					Outgoing.Add (composerType, composer);
					if (composerType.BaseType.Namespace.StartsWith ("Yupi.Messages.Contracts")
						&& !composerType.BaseType.Name.StartsWith("AbstractComposer")) {
						Outgoing.Add (composerType.BaseType, composer);
					}
				}
			}
		}

		private IEnumerable<Type> GetImplementing<T>() {
			return MessageAssembly.GetTypes ().Where (p => typeof(T).IsAssignableFrom (p) && p.GetConstructor(Type.EmptyTypes) != null);
		}
	}
}