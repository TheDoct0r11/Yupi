using System;
using System.Linq;
using Yupi.Core.Io.Logger;
using Yupi.Core.Security;
using Yupi.Core.Security.BlackWords.Structs;
using Yupi.Data.Base.Adapters.Interfaces;
using Yupi.Game.Rooms.User;
using Yupi.Game.Users;
using Yupi.Game.Users.Data.Models;
using Yupi.Game.Users.Factories;
using Yupi.Messages;
using Yupi.Messages.Enums;
using Yupi.Messages.Handlers;
using Yupi.Messages.Parsers;
using Yupi.Net.Packets;

namespace Yupi.Game.GameClients.Interfaces
{
    /// <summary>
    ///     Class GameClient.
    /// </summary>
    public class GameClient
    {
        /// <summary>
        ///     The _connection
        /// </summary>
        private ConnectionData _connection;

        /// <summary>
        ///     The _disconnected
        /// </summary>
        private bool _disconnected;

        /// <summary>
        ///     The _habbo
        /// </summary>
        private Habbo _habbo;

        /// <summary>
        ///     The _message handler
        /// </summary>
        private GameClientMessageHandler _messageHandler;

        /// <summary>
        ///     The current room user identifier
        /// </summary>
        internal int CurrentRoomUserId;

        /// <summary>
        ///     The designed handler
        /// </summary>
        internal int DesignedHandler = 1;

        /// <summary>
        ///     The machine identifier
        /// </summary>
        internal string MachineId;

        /// <summary>
        ///     The packet parser
        /// </summary>
        internal ServerPacketParser PacketParser;

        /// <summary>
        ///     The publicist count
        /// </summary>
        internal byte PublicistCount;

        /// <summary>
        ///     The time pinged received
        /// </summary>
        internal DateTime TimePingedReceived;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameClient" /> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="connection">The connection.</param>
        internal GameClient(uint clientId, ConnectionData connection)
        {
            ConnectionId = clientId;
            _connection = connection;
            CurrentRoomUserId = -1;
            PacketParser = new ServerPacketParser();
        }

        /// <summary>
        ///     Gets the connection identifier.
        /// </summary>
        /// <value>The connection identifier.</value>
        internal uint ConnectionId { get; private set; }

        /// <summary>
        ///     Handles the publicist.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="message">The message.</param>
        /// <param name="method">The method.</param>
        /// <param name="settings">The settings.</param>
        internal void HandlePublicist(string word, string message, string method, BlackWordTypeSettings settings)
        {
            Habbo userPublicist = GetHabbo();

            if (userPublicist != null)
            {
                ServerMessage serverMessage;

                if (userPublicist.Rank < 5 && settings.MaxAdvices == PublicistCount++ && settings.AutoBan)
                {
                    serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("SuperNotificationMessageComposer"));
                    serverMessage.AppendString("staffcloud");
                    serverMessage.AppendInteger(2);
                    serverMessage.AppendString("title");
                    serverMessage.AppendString("Staff Internal Alert");
                    serverMessage.AppendString("message");
                    serverMessage.AppendString("O usu�rio " + userPublicist.UserName + " Fo Banido por Divulgar. A �ltima palavra foi: " + word + ", na frase: " + message);

                    Yupi.GetGame().GetClientManager().StaffAlert(serverMessage);

                    Yupi.GetGame().GetBanManager().BanUser(this, userPublicist.UserName, 788922000.0, "Voc� est� divulgando Hoteis. Ser� banido para sempre..", true, true);

                    return;
                }

                string alert = settings.Alert.Replace("{0}", userPublicist.UserName);

                alert = alert.Replace("{1}", userPublicist.Id.ToString());
                alert = alert.Replace("{2}", word);
                alert = alert.Replace("{3}", message);
                alert = alert.Replace("{4}", method);

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("UsersClassificationMessageComposer"));
                serverMessage.AppendInteger(1);
                serverMessage.AppendInteger(userPublicist.Id);
                serverMessage.AppendString(userPublicist.UserName);
                serverMessage.AppendString("BadWord: " + word);

                Yupi.GetGame()?.GetClientManager()?.StaffAlert(serverMessage);

                if (Yupi.GetGame().GetClientManager() == null)
                    return;

                foreach (GameClient client in Yupi.GetGame().GetClientManager().Clients.Values)
                {
                    if (client?.GetHabbo() == null)
                        continue;

                    if (client.CurrentRoomUserId == 0)
                        continue;

                    if (client.GetHabbo().Rank < 5)
                        continue;

                    serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("WhisperMessageComposer"));
                    serverMessage.AppendInteger(client.CurrentRoomUserId);
                    serverMessage.AppendString(alert);
                    serverMessage.AppendInteger(0);
                    serverMessage.AppendInteger(34);
                    serverMessage.AppendInteger(0);
                    serverMessage.AppendInteger(true);

                    client.SendMessage(serverMessage);
                }
            }
        }

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        /// <returns>ConnectionInformation.</returns>
        internal ConnectionData GetConnection() => _connection;

        /// <summary>
        ///     Gets the message handler.
        /// </summary>
        /// <returns>GameClientMessageHandler.</returns>
        internal GameClientMessageHandler GetMessageHandler() => _messageHandler;

        /// <summary>
        ///     Gets the habbo.
        /// </summary>
        /// <returns>Habbo.</returns>
        internal Habbo GetHabbo() => _habbo;

        /// <summary>
        ///     Starts the connection.
        /// </summary>
        internal void StartConnection()
        {
            if (_connection == null)
                return;

            TimePingedReceived = DateTime.Now;

            InitialPacketParser packetParser = _connection.Parser as InitialPacketParser;

            if (packetParser != null)
                packetParser.PolicyRequest += PolicyRequest;

            InitialPacketParser initialPacketParser = _connection.Parser as InitialPacketParser;

            if (initialPacketParser != null)
                initialPacketParser.SwitchParserRequest += SwitchParserRequest;

            _connection.StartPacketProcessing();
        }

        /// <summary>
        ///     Initializes the handler.
        /// </summary>
        internal void InitHandler()
        {
            _messageHandler = new GameClientMessageHandler(this);
        }

        /// <summary>
        ///     Tries the login.
        /// </summary>
        /// <param name="authTicket">The authentication ticket.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool TryLogin(string authTicket)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authTicket))
                    return false;

                string ip = GetConnection().GetIp();

                if (string.IsNullOrEmpty(ip))
                    return false;

                uint errorCode;

                UserData userData = UserDataFactory.GetUserData(authTicket, out errorCode);

                if (userData?.User == null)
                    return false;

                if (errorCode == 1 || errorCode == 2)
                    return false;

                Yupi.GetGame().GetClientManager().RegisterClient(this, userData.UserId, userData.User.UserName);

                _habbo = userData.User;

                userData.User.LoadData(userData);
 
                bool isBanned = Yupi.GetGame().GetBanManager().CheckIfIsBanned(userData.User.UserName, ip, MachineId);

                if (isBanned)
                {
                    string banReason = Yupi.GetGame().GetBanManager().GetBanReason(userData.User.UserName, ip, MachineId);

                    SendNotifWithScroll(banReason);

                    using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery($"SELECT ip_last FROM users WHERE id = {GetHabbo().Id} LIMIT 1");

                        string supaString = queryReactor.GetString();

                        queryReactor.SetQuery($"SELECT COUNT(0) FROM users_bans_access WHERE user_id={_habbo.Id} LIMIT 1");
                        int integer = queryReactor.GetInteger();

                        if (integer > 0)
                            queryReactor.RunFastQuery("UPDATE users_bans_access SET attempts = attempts + 1, ip='" + supaString + "' WHERE user_id=" + GetHabbo().Id + " LIMIT 1");
                        else
                            queryReactor.RunFastQuery("INSERT INTO users_bans_access (user_id, ip) VALUES (" + GetHabbo().Id + ", '" + supaString + "')");
                    }

                    return false;
                }

                using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
                    queryReactor.RunFastQuery($"UPDATE users SET ip_last='{ip}' WHERE id={GetHabbo().Id}");

                userData.User.Init(this, userData);

                QueuedServerMessage queuedServerMessage = new QueuedServerMessage(_connection);

                ServerMessage serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("UniqueMachineIDMessageComposer"));

                serverMessage.AppendString(MachineId);
                queuedServerMessage.AppendResponse(serverMessage);

                queuedServerMessage.AppendResponse(
                    new ServerMessage(PacketLibraryManager.OutgoingRequest("AuthenticationOKMessageComposer")));

                ServerMessage serverMessage2 = new ServerMessage(PacketLibraryManager.OutgoingRequest("HomeRoomMessageComposer"));

                serverMessage2.AppendInteger(_habbo.HomeRoom);
                serverMessage2.AppendInteger(_habbo.HomeRoom);
                queuedServerMessage.AppendResponse(serverMessage2);

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("MinimailCountMessageComposer"));

                serverMessage.AppendInteger(_habbo.MinimailUnreadMessages);
                queuedServerMessage.AppendResponse(serverMessage);

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("FavouriteRoomsMessageComposer"));

                serverMessage.AppendInteger(30);

                if (userData.User.FavoriteRooms == null || !userData.User.FavoriteRooms.Any())
                    serverMessage.AppendInteger(0);
                else
                {
                    serverMessage.AppendInteger(userData.User.FavoriteRooms.Count);

                    foreach (uint i in userData.User.FavoriteRooms)
                        serverMessage.AppendInteger(i);
                }

                queuedServerMessage.AppendResponse(serverMessage);

                ServerMessage rightsMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("UserClubRightsMessageComposer"));

                rightsMessage.AppendInteger(userData.User.GetSubscriptionManager().HasSubscription ? 2 : 0);
                rightsMessage.AppendInteger(userData.User.Rank);
                rightsMessage.AppendInteger(0);
                queuedServerMessage.AppendResponse(rightsMessage);

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("EnableNotificationsMessageComposer"));
                serverMessage.AppendBool(true); //isOpen
                serverMessage.AppendBool(false);
                queuedServerMessage.AppendResponse(serverMessage);

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("EnableTradingMessageComposer"));
                serverMessage.AppendBool(true);
                queuedServerMessage.AppendResponse(serverMessage);
                userData.User.UpdateCreditsBalance();

                serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("ActivityPointsMessageComposer"));
                serverMessage.AppendInteger(2);
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(userData.User.Duckets);
                serverMessage.AppendInteger(5);
                serverMessage.AppendInteger(userData.User.Diamonds);
                queuedServerMessage.AppendResponse(serverMessage);

                if (userData.User.HasFuse("fuse_mod"))
                    queuedServerMessage.AppendResponse(Yupi.GetGame().GetModerationTool().SerializeTool(this));

                queuedServerMessage.AppendResponse(Yupi.GetGame().GetAchievementManager().AchievementDataCached);

                queuedServerMessage.AppendResponse(GetHabbo().GetAvatarEffectsInventoryComponent().GetPacket());
                queuedServerMessage.SendResponse();

                Yupi.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(this);
                Yupi.GetGame().GetAchievementManager().TryProgressRegistrationAchievements(this);
                Yupi.GetGame().GetAchievementManager().TryProgressLoginAchievements(this);

                return true;
            }
            catch (Exception ex)
            {
                YupiLogManager.LogException(ex, "Registered Login Exception.", "Yupi.Users");
            }

            return false;
        }

        /// <summary>
        ///     Sends the notif with scroll.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendNotifWithScroll(string message)
        {
            ServerMessage serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("MOTDNotificationMessageComposer"));

            serverMessage.AppendInteger(1);
            serverMessage.AppendString(message);
            SendMessage(serverMessage);
        }

        /// <summary>
        ///     Sends the broadcast message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendBroadcastMessage(string message)
        {
            ServerMessage serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("BroadcastNotifMessageComposer"));

            serverMessage.AppendString(message);
            serverMessage.AppendString(string.Empty);
            SendMessage(serverMessage);
        }

        /// <summary>
        ///     Sends the moderator message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendModeratorMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            ServerMessage serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("AlertNotificationMessageComposer"));

            serverMessage.AppendString(message);
            serverMessage.AppendString(string.Empty);

            SendMessage(serverMessage);
        }

        /// <summary>
        ///     Sends the whisper.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="fromWired"></param>
        internal void SendWhisper(string message, bool fromWired = false)
        {
            if (GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser roomUserByHabbo = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().UserName);

            if (roomUserByHabbo == null)
                return;

            ServerMessage whisp = new ServerMessage(PacketLibraryManager.OutgoingRequest("WhisperMessageComposer"));

            whisp.AppendInteger(roomUserByHabbo.VirtualId);
            whisp.AppendString(message);
            whisp.AppendInteger(0);
            whisp.AppendInteger(fromWired ? 34 : roomUserByHabbo.LastBubble);
            whisp.AppendInteger(0);
            whisp.AppendInteger(fromWired);

            SendMessage(whisp);
        }

        /// <summary>
        ///     Sends the notif.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="picture">The picture.</param>
        internal void SendNotif(string message, string title = "Aviso", string picture = "")
        {
            SendMessage(GetBytesNotif(message, title, picture));
        }

        /// <summary>
        ///     Gets the bytes notif.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="picture">The picture.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesNotif(string message, string title = "Aviso", string picture = "")
        {
            using (
                ServerMessage serverMessage = new ServerMessage(PacketLibraryManager.OutgoingRequest("SuperNotificationMessageComposer"))
                )
            {
                serverMessage.AppendString(picture);
                serverMessage.AppendInteger(4);
                serverMessage.AppendString("title");
                serverMessage.AppendString(title);
                serverMessage.AppendString("message");
                serverMessage.AppendString(message);
                serverMessage.AppendString("linkUrl");
                serverMessage.AppendString("event:");
                serverMessage.AppendString("linkTitle");
                serverMessage.AppendString("ok");

                return serverMessage.GetReversedBytes();
            }
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        internal void Stop()
        {
            if (GetMessageHandler() != null)
                GetMessageHandler().Destroy();

            if (GetHabbo() != null)
                GetHabbo().OnDisconnect("disconnect");

            CurrentRoomUserId = -1;
            _messageHandler = null;
            _habbo = null;
            _connection = null;
        }

        /// <summary>
        ///     Disconnects the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        internal void Disconnect(string reason)
        {
            if (GetHabbo() != null)
            {
                GetHabbo().RunDbUpdate();

                GetHabbo().OnDisconnect(reason);
            }

            if (_disconnected)
                return;

            _connection?.Dispose();
            _disconnected = true;
        }

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendMessage(ServerMessage message)
        {
            if (message == null)
                return;

            if (GetConnection() == null)
                return;

            byte[] bytes = message.GetReversedBytes();

            GetConnection().SendData(bytes);
        }

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        internal void SendMessage(byte[] bytes)
        {
            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="type">The type.</param>
        internal void SendMessage(StaticMessage type)
        {
            if (GetConnection() == null)
                return;

            GetConnection().SendData(StaticMessagesManager.Get(type));
        }

        /// <summary>
        ///     Switches the parser request.
        /// </summary>
        private void SwitchParserRequest(byte[] data, int amountOfBytes)
        {
            if (_connection == null)
                return;

            if (_messageHandler == null)
                InitHandler();

            PacketParser.SetConnection(_connection, this);

            _connection.Parser.Dispose();
            _connection.Parser = PacketParser;
            _connection.Parser.HandlePacketData(data, amountOfBytes);
        }

        /// <summary>
        ///     Policies the request.
        /// </summary>
        private void PolicyRequest()
        {
            _connection.SendData(CrossDomainSettings.XmlPolicyBytes);
        }
    }
}