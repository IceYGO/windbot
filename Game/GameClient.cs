using System.Text;
using YGOSharp.Network;
using YGOSharp.Network.Enums;

namespace WindBot.Game
{
    public class GameClient
    {
        public CoreClient Connection { get; private set; }
        public string Username;
        public string Deck;

        private string _serverHost;
        private int _serverPort;
        private string _roomInfos;

        private GameBehavior _behavior;

        public GameClient(string username, string deck, string serverHost, int serverPort, string roomInfos = "")
        {
            Username = username;
            Deck = deck;
            _serverHost = serverHost;
            _serverPort = serverPort;
            _roomInfos = roomInfos;
        }

        public void Start()
        {
            Connection = new CoreClient(_serverHost, _serverPort);
            Connection.MessageReceived += OnMessageReceived;

            _behavior = new GameBehavior(this);

            GamePacketWriter packet = new GamePacketWriter(CtosMessage.PlayerInfo);
            packet.Write(Username, 20);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = new GamePacketWriter(CtosMessage.JoinGame);
            packet.Write(Program.ProVersion);
            packet.Write(junk);
            packet.Write(_roomInfos, 30);
            Connection.Send(packet);
        }

        public void Tick()
        {
            Connection.UpdateNetwork();
            Connection.Update();
        }

        public void Chat(string message)
        {
            byte[] content = Encoding.Unicode.GetBytes(message + "\0");
            GamePacketWriter chat = new GamePacketWriter(CtosMessage.Chat);
            chat.Write(content);
            Connection.Send(chat);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            _behavior.OnPacket(e.Message);
        }
    }
}