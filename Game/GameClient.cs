using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;

namespace WindBot.Game
{
    public class GameClient
    {
        public YGOClient Connection { get; private set; }
        public string Username;
        public string Deck;
        public string DeckFile;
        public string Dialog;
        public int Hand;
        public bool Debug;
        public bool _chat;
        public int RoomId;
        public CreateGameInfo CreateGame;
        private string _serverHost;
        private int _serverPort;
        private int _proVersion;

        private string _roomInfo;

        private GameBehavior _behavior;

        public GameClient(WindBotInfo Info)
        {
            Username = Info.Name;
            Deck = Info.Deck;
            DeckFile = Info.DeckFile;
            Dialog = Info.Dialog;
            Hand = Info.Hand;
            Debug = Info.Debug;
            _chat = Info.Chat;
            RoomId = Info.RoomId;
            CreateGame = Info.CreateGame;
            _serverHost = Info.Host;
            _serverPort = Info.Port;
            _roomInfo = Info.HostInfo;
            _proVersion = Info.Version;
        }

        public void Start()
        {
            Connection = new YGOClient();
            _behavior = new GameBehavior(this);

            Connection.Connected += OnConnected;
            Connection.PacketReceived += OnPacketReceived;

            IPAddress target_address;
            try
            {
                target_address = IPAddress.Parse(_serverHost);
            }
            catch (System.Exception)
            {
                IPHostEntry _hostEntry = Dns.GetHostEntry(_serverHost);
                target_address = _hostEntry.AddressList.FirstOrDefault(findIPv4 => findIPv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            Connection.Connect(target_address, _serverPort);
        }

        private void OnConnected()
        {
            BinaryWriter packet = GamePacketFactory.Create(CtosMessage.PlayerInfo);
            packet.WriteUnicode(Username, 20);
            Connection.Send(packet);
            byte[] padding2 = { 0xAA, 0xBB };
            if(CreateGame == null)
            {
                packet = GamePacketFactory.Create(CtosMessage.JoinGame);
                packet.Write((short)_proVersion);
                packet.Write(padding2);
                packet.Write(RoomId);
                packet.WriteUnicode(_roomInfo, 20);
                packet.Write(_proVersion);
                Connection.Send(packet);
                return;
            }
            byte[] unused = { 0x00, 0x00 };
            byte[] padding3 = { 0xAA, 0xBB, 0xCC };
            packet = GamePacketFactory.Create(CtosMessage.CreateGame);
            // hostInfo
            packet.Write(CreateGame.banlistHash);
            packet.Write(CreateGame.allowed);
            packet.Write(unused); // mode & duelRule
            packet.Write((byte)(CreateGame.dontCheckDeck ? 1 : 0));
            packet.Write((byte)(CreateGame.dontShuffleDeck ? 1 : 0));
            packet.Write(padding3);
            packet.Write(CreateGame.startingLP);
            packet.Write(CreateGame.startingDrawCount);
            packet.Write(CreateGame.drawCountPerTurn);
            packet.Write(CreateGame.timeLimitInSeconds);
            packet.Write((uint)((CreateGame.duelFlags >> 32) & 0xFFFFFFFF));
            packet.Write((uint)4043399681); // handshake
            packet.Write(_proVersion); // version
            packet.Write(CreateGame.t0Count);
            packet.Write(CreateGame.t1Count);
            packet.Write(CreateGame.bestOf);
            packet.Write((uint)(CreateGame.duelFlags & 0xFFFFFFFF));
            packet.Write(CreateGame.forb);
            packet.Write(CreateGame.extraRules);
            packet.Write(padding2);
            // name
            packet.WriteUnicode("", 20); // UNUSED
            // pass
            packet.WriteUnicode(_roomInfo, 20);
            // notes
            const int MAX_NOTES_LENGTH = 200;
            try
            {
                // Write notes in UTF8 format making sure to always write exactly
                // MAX_NOTES_LENGTH bytes.
                byte[] content = Encoding.UTF8.GetBytes(CreateGame.notes + "\0");
                if(content.Length > MAX_NOTES_LENGTH)
                    throw new Exception();
                packet.Write(content);
                for(int i = MAX_NOTES_LENGTH - content.Length; i > 0; i--)
                    packet.Write((byte)0);
            }
            catch(Exception)
            {
                Logger.WriteErrorLine("Warning: Unable to encode CreateGame.notes, sending empty string instead.");
                for (int i = 0; i < (MAX_NOTES_LENGTH / 8); i++) packet.Write((ulong)0);
            }
            Connection.Send(packet);
        }

        public void Tick()
        {
            Connection.Update();
        }

        public void Chat(string message, bool forced)
        {
            if (!forced && !_chat)
                return;
            byte[] content = Encoding.Unicode.GetBytes(message + "\0");
            BinaryWriter chat = GamePacketFactory.Create(CtosMessage.Chat);
            chat.Write(content);
            Connection.Send(chat);
        }

        public void Log(string message, int type)
        {
            if(type == 0)
            {
                Logger.WriteLine(message);
            } else if (type == 1)
            {
                Logger.DebugWriteLine(message);
            }
            else if (type == 2)
            {
                Logger.WriteErrorLine(message);
            }
        }

        private void OnPacketReceived(BinaryReader reader)
        {
            _behavior.OnPacket(reader);
        }
    }
}
