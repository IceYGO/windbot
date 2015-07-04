using System.IO;
using WindBot.Game.Network.Enums;

namespace WindBot.Game.Network
{
    public class GameClientPacket
    {
        private BinaryWriter _writer;
        private MemoryStream _stream;

        public GameClientPacket(CtosMessage message)
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
            _writer.Write((byte)message);
        }

        public byte[] GetContent()
        {
            return _stream.ToArray();
        }

        public void Write(byte[] array)
        {
            _writer.Write(array);
        }

        public void Write(sbyte value)
        {
            _writer.Write(value);
        }

        public void Write(byte value)
        {
            _writer.Write(value);
        }

        public void Write(short value)
        {
            _writer.Write(value);
        }

        public void Write(int value)
        {
            _writer.Write(value);
        }

        public void Write(string text, int len)
        {
            _writer.WriteUnicode(text, len);
        }
    }
}