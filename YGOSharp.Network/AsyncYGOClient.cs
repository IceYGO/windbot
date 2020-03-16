using System.IO;
using YGOSharp.Network.Enums;

namespace YGOSharp.Network
{
    public class AsyncYGOClient : AsyncBinaryClient
    {
        public AsyncYGOClient()
            : base(new NetworkClient())
        {
        }

        public AsyncYGOClient(NetworkClient client)
            : base(client)
        {
        }

        public void Send(BinaryWriter writer)
        {
            Send(((MemoryStream)writer.BaseStream).ToArray());
        }

        public void Send(CtosMessage message)
        {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write((byte)message);
                Send(writer);
            }
        }

        public void Send(CtosMessage message, int value)
        {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write((byte)message);
                writer.Write(value);
                Send(writer);
            }
        }
    }
}
