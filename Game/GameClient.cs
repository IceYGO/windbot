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

        private string _serverHost;
        private int _serverPort;
        
        private string _roomInfos;

        private GameBehavior _behavior;

        public GameClient(string username = "Windbot", string deck = "Blue-Eyes", string serverHost = "127.0.0.1", int serverPort = 7911, string dialog = "default", string roomInfos = "")
        {
            Username = username;
            Deck = deck;
            Dialog = dialog;
            _serverHost = serverHost;
            _serverPort = serverPort;
            _roomInfos = roomInfos;
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
            packet.WriteUnicode(Username, Program.PlayerNameSize);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = GamePacketFactory.Create(CtosMessage.JoinGame);
            packet.Write(Program.ProVersion);
            packet.Write(junk);
            packet.WriteUnicode(_roomInfos, 30);
            Connection.Send(packet);
        }

        public void Tick()
        {
            Connection.Update();
        }

        public void Chat(string message)
        {
            byte[] content = Encoding.Unicode.GetBytes(message + "\0");
            BinaryWriter chat = GamePacketFactory.Create(CtosMessage.Chat);
            chat.Write(content);
            Connection.Send(chat);
        }

        private void OnPacketReceived(BinaryReader reader)
        {
            _behavior.OnPacket(reader);
        }
    }
}