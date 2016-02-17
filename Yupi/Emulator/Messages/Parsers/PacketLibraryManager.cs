using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yupi.Emulator.Core.Io.Logger;
using Yupi.Emulator.Core.Settings;
using Yupi.Emulator.Messages.Handlers;
using Yupi.Emulator.Messages.Library;

namespace Yupi.Emulator.Messages.Parsers
{
    internal static class PacketLibraryManager
    {
        public delegate void ParamLess();

        internal static Dictionary<int, StaticRequestHandler> Incoming;
        internal static Dictionary<string, string> Library;
        internal static Dictionary<string, int> Outgoing;

        private static List<uint> _registeredOutoings;

        internal static int CountReleases;
        internal static string ReleaseName;

        public static int OutgoingRequest(string packetName)
        {
            int packetId;

            if (Outgoing.TryGetValue(packetName, out packetId))
                return packetId;

           YupiLogManager.LogWarning("Outgoing " + packetName + " doesn't exist.", "Yupi.Communication");

            return -1;
        }

        public static void Configure()
        {
            Incoming = new Dictionary<int, StaticRequestHandler>();
            Library = new Dictionary<string, string>();
            Outgoing = new Dictionary<string, int>();

            ReleaseName = ServerConfigurationSettings.Data["client.build"];
        }

        public static void Register()
        {
            RegisterLibrary();
            RegisterOutgoing();
            RegisterIncoming();
        }

        public static void Init()
        {
            Configure();

            Register();
        }

        public static void HandlePacket(GameClientMessageHandler handler, ClientMessage message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            if (Incoming.ContainsKey(message.Id))
            {
                if (Yupi.PacketDebugMode)
                {
                    Console.WriteLine();
                    Console.Write("INCOMING ");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("HANDLED ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(message.Id + Environment.NewLine + message);

                    if (message.Length > 0)
                        Console.WriteLine();

                    Console.WriteLine();
                }

                StaticRequestHandler staticRequestHandler = Incoming[message.Id];

                staticRequestHandler(handler);
            }
            else if (Yupi.PacketDebugMode)
            {
                Console.WriteLine();
                Console.Write("INCOMING ");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("REFUSED ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(message.Id + Environment.NewLine + message);

                if (message.Length > 0)
                    Console.WriteLine();

                Console.WriteLine();
            }
        }

        internal static void ReloadDictionarys()
        {
            Incoming.Clear();
            Outgoing.Clear();
            Library.Clear();

            Register();
        }

        internal static void RegisterIncoming()
        {
            CountReleases = 0;

            string[] filePaths = Directory.GetFiles($@"{Yupi.YupiVariablesDirectory}\Packets\{ReleaseName}", "*.incoming");

            foreach (string[] fileContents in filePaths.Select(currentFile => File.ReadAllLines(currentFile, Encoding.UTF8)))
            {
                CountReleases++;

                foreach (string[] fields in fileContents.Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("[")).Select(line => line.Replace(" ", string.Empty).Split('=')))
                {
                    string packetName = fields[0];

                    if (fields[1].Contains('/'))
                        fields[1] = fields[1].Split('/')[0];

                    int packetId = fields[1].ToLower().Contains('x') ? Convert.ToInt32(fields[1], 16) : Convert.ToInt32(fields[1]);

                    if (!Library.ContainsKey(packetName))
                        continue;

                    string libValue = Library[packetName];

                    PacketLibrary.GetProperty del = (PacketLibrary.GetProperty) Delegate.CreateDelegate(typeof (PacketLibrary.GetProperty), typeof (PacketLibrary), libValue);

                    if (!Incoming.ContainsKey(packetId))
                        Incoming.Add(packetId, new StaticRequestHandler(del));
                }
            }
        }

        internal static void RegisterOutgoing()
        {
            _registeredOutoings = new List<uint>();

            string[] filePaths = Directory.GetFiles($@"{Yupi.YupiVariablesDirectory}\Packets\{ReleaseName}", "*.outgoing");

            foreach (string[] fields in filePaths.Select(File.ReadAllLines).SelectMany(fileContents => fileContents.Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("[")).Select(line => line.Replace(" ", string.Empty).Split('='))))
            {
                if (fields[1].Contains('/'))
                    fields[1] = fields[1].Split('/')[0];

                string packetName = fields[0];
                int packetId = int.Parse(fields[1]);

                if (packetId != -1)
                {
                    if (!_registeredOutoings.Contains((uint) packetId))
                        _registeredOutoings.Add((uint) packetId);
                }

                Outgoing.Add(packetName, packetId);
            }

            _registeredOutoings.Clear();
            _registeredOutoings = null;
        }

        internal static void RegisterLibrary()
        {
            string[] filePaths = Directory.GetFiles($@"{Yupi.YupiVariablesDirectory}\Packets\{ReleaseName}", "*.library");

            foreach (string[] fields in filePaths.Select(File.ReadAllLines).SelectMany(fileContents => fileContents.Select(line => line.Split('='))))
            {
                if (fields[1].Contains('/'))
                    fields[1] = fields[1].Split('/')[0];

                string incomingName = fields[0];
                string libraryName = fields[1];

                Library.Add(incomingName, libraryName);
            }
        }

        internal delegate void StaticRequestHandler(GameClientMessageHandler handler);
    }
}