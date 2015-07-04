using System;
using System.IO;
using System.Text;

namespace WindBot.Game.Network
{
    public static class BinaryExtensions
    {
        public static void WriteUnicode(this BinaryWriter writer, string text, int len)
        {
            byte[] unicode = Encoding.Unicode.GetBytes(text);
            byte[] result = new byte[len * 2];
            int max = len * 2 - 2;
            Array.Copy(unicode, result, unicode.Length > max ? max : unicode.Length);
            writer.Write(result);
        }

        public static string ReadUnicode(this BinaryReader reader, int len)
        {
            byte[] unicode = reader.ReadBytes(len*2);
            string text = Encoding.Unicode.GetString(unicode);
            if (text.Contains("\0"))
                text = text.Substring(0, text.IndexOf('\0'));
            return text;
        }
    }
}