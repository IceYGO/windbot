using System.Net;
using System.Text;
using WindBot.Game.Network;
using WindBot.Game.Network.Enums;

namespace WindBot.Game
{
    public class GameClient
    {
        public GameConnection Connection { get; private set; }
        public string Username;
        public string Deck;

        private string _serverHost;
        private int _serverPort;
        private string _roomInfos;

        private GameBehavior _behavior;

        public GameClient(string username, string deck, string serverHost, int serverPort, string roomInfos)
        {
            Username = username;
            Deck = deck;
            _serverHost = serverHost;
            _serverPort = serverPort;
            _roomInfos = roomInfos;
        }

        public void Start()
        {
            Connection = new GameConnection(IPAddress.Parse(_serverHost), _serverPort);
            _behavior = new GameBehavior(this);

            GameClientPacket packet = new GameClientPacket(CtosMessage.PlayerInfo);
            packet.Write(Username, 20);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = new GameClientPacket(CtosMessage.JoinGame);
            packet.Write(Program.ProVersion);
            packet.Write(junk);
            packet.Write(_roomInfos, 30);
            Connection.Send(packet);
        }

        public void Tick()
        {
            if (!Connection.IsConnected)
            {
                return;
            }
            while (Connection.HasPacket())
            {
                GameServerPacket packet = Connection.Receive();
                _behavior.OnPacket(packet);
            }
        }

        public void Chat(string message)
        {
            byte[] content = Encoding.Unicode.GetBytes(message + "\0");
            GameClientPacket chat = new GameClientPacket(CtosMessage.Chat);
            chat.Write(content);
            Connection.Send(chat);
        }
    }
}