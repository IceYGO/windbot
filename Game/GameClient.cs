using System.IO;
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
        public string Dialog;
        public int Hand;
        public bool Debug;
        public bool _chat;
        private string _serverHost;
        private int _serverPort;
        private int _proVersion;

        private string _roomInfo;

        private GameBehavior _behavior;

        public GameClient(WindBotInfo Info)
        {
            Username = Info.Name;
            Deck = Info.Deck;
            Dialog = Info.Dialog;
            Hand = Info.Hand;
            Debug = Info.Debug;
            _chat = Info.Chat;
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

            Connection.Connect(IPAddress.Parse(_serverHost), _serverPort);
        }

        private void OnConnected()
        {
            BinaryWriter packet = GamePacketFactory.Create(CtosMessage.PlayerInfo);
            packet.WriteUnicode(Username, 20);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = GamePacketFactory.Create(CtosMessage.JoinGame);
            packet.Write((short)_proVersion);
            packet.Write(junk);
            packet.WriteUnicode(_roomInfo, 20);
            packet.Write(_proVersion);
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