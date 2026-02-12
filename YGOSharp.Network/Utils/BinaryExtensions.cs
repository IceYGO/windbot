using System;
using System.IO;
using System.Text;

namespace YGOSharp.Network.Utils
{
    public static class BinaryExtensions
    {
        // fixed length strings
        public static void WriteUnicode(this BinaryWriter writer, string text, int len)
        {
            byte[] unicode = Encoding.Unicode.GetBytes(text);
            byte[] result = new byte[len * 2];
            int copy = unicode.Length;
            if (unicode.Length > len * 2 - 2)
            {
                copy = len * 2 - 2;
#if DEBUG
                throw new ArgumentException("String '" + text + "' is too long for fixed length " + len + ".");
#endif
            }
            Array.Copy(unicode, result, copy);
            writer.Write(result);
        }

        // variable length strings
        public static void WriteUnicodeAutoLength(this BinaryWriter writer, string text, int maxlen)
        {
            byte[] result = Encoding.Unicode.GetBytes(text + "\0");
            int len = result.Length / 2;
            if (len > maxlen)
            {
                len = maxlen;
                result[len * 2 - 2] = 0;
                result[len * 2 - 1] = 0;
#if DEBUG
                throw new ArgumentException("String '" + text + "' is too long for max length " + maxlen + ".");
#endif
            }
            writer.Write(result, 0, len * 2);
        }

        public static string ReadUnicode(this BinaryReader reader, int len)
        {
            byte[] unicode = reader.ReadBytes(len * 2);
            string text = Encoding.Unicode.GetString(unicode);
            int index = text.IndexOf('\0');
            if (index > 0) text = text.Substring(0, index);
            return text;
        }

        public static byte[] ReadToEnd(this BinaryReader reader)
        {
            return reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
        }
    }
}
