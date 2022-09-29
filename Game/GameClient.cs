using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;
using DnsClient;
using DnsClient.Protocol;

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
        private string _serverHost;
        private int _serverPort;
        private short _proVersion;

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
            _serverHost = Info.Host;
            _serverPort = Info.Port;
            _roomInfo = Info.HostInfo;
            _proVersion = (short)Info.Version;
        }

        private IPAddress LookupHost(string hostname)
        {
            IPAddress address;
            try
            {
                address = IPAddress.Parse(hostname);
            }
            catch (System.Exception)
            {
                IPHostEntry _hostEntry = Dns.GetHostEntry(hostname);
                address = _hostEntry.AddressList.FirstOrDefault(findIPv4 => findIPv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            if (Debug)
                Logger.WriteLine($"Resolved {hostname} to {address.ToString()}");
            return address;
        }

        private ConnectionInfo ParseHost()
        {
            var conn = new ConnectionInfo();
            if (_serverPort == 0)
            {
                var srvAddress = $"_ygopro._tcp.{_serverHost}";
                if (Debug)
                    Logger.WriteLine($"Resolving SRV record {srvAddress}");
                var srvResult = new LookupClient().Query(srvAddress, QueryType.SRV);
                if (!srvResult.HasError)
                {
                    var record = srvResult.Answers.OfType<SrvRecord>()
                        .FirstOrDefault();
                    if (record != null)
                    {
                        if (Debug)
                            Logger.WriteLine($"Resolved {srvAddress} to {record.Target}:{record.Port}");
                        var additionalRecord = srvResult.Additionals
                            .FirstOrDefault(p => p.DomainName.Equals(record.Target));
                        if (additionalRecord is ARecord aRecord)
                        {
                            conn.host = aRecord.Address;
                        } else if (additionalRecord is CNameRecord cNameRecord)
                        {
                            conn.host = LookupHost(cNameRecord.CanonicalName);
                        }
                        else
                        {
                            conn.host = LookupHost(record.Target);
                        }
                        conn.port = record.Port;
                        return conn;
                    }
                }

                conn.port = 7911;
            }
            conn.host = LookupHost(_serverHost);
            return conn;
        }

        public void Start()
        {
            Connection = new YGOClient();
            _behavior = new GameBehavior(this);

            Connection.Connected += OnConnected;
            Connection.PacketReceived += OnPacketReceived;

            var conn = ParseHost();   
            Connection.Connect(conn.host, conn.port);
        }

        private void OnConnected()
        {
            BinaryWriter packet = GamePacketFactory.Create(CtosMessage.PlayerInfo);
            packet.WriteUnicode(Username, 20);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = GamePacketFactory.Create(CtosMessage.JoinGame);
            packet.Write(_proVersion);
            packet.Write(junk);
            packet.WriteUnicode(_roomInfo, 30);
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