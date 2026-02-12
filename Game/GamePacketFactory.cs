using System.IO;
using YGOSharp.Network.Enums;

namespace WindBot.Game
{
    public class GamePacketFactory
    {
        public static BinaryWriter Create(CtosMessage message)
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            writer.Write((byte)message);
            return writer;
        }
    }
}
