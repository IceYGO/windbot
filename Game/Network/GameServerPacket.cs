using System.IO;
using WindBot.Game.Network.Enums;

namespace WindBot.Game.Network
{
    public class GameServerPacket
    {
        public byte[] Content { get; private set; }

        private BinaryReader _reader;

        public GameServerPacket(byte[] content)
        {
            Content = content;
            _reader = new BinaryReader(new MemoryStream(Content));
        }

        public StocMessage ReadStoc()
        {
            return (StocMessage)_reader.ReadByte();
        }

        public GameMessage ReadGameMsg()
        {
            return (GameMessage)_reader.ReadByte();
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public byte[] ReadToEnd()
        {
            return _reader.ReadBytes((int)_reader.BaseStream.Length - (int)_reader.BaseStream.Position);
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public string ReadUnicode(int len)
        {
            return _reader.ReadUnicode(len);
        }

        public long GetPosition()
        {
            return _reader.BaseStream.Position;
        }

        public void SetPosition(long pos)
        {
            _reader.BaseStream.Position = pos;
        }
    }
}