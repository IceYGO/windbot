using System;
using System.IO;

namespace WindBot
{
    public static class Logger
    {
        public static void WriteLine(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
        }
        public static void DebugWriteLine(string message)
        {
#if DEBUG
            Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
            using (FileStream fs = new FileStream(@"d:\windbot-log.txt", FileMode.OpenOrCreate, FileAccess.Write))
              {
                  using (StreamWriter sw = new StreamWriter(fs))
                 {
                     sw.BaseStream.Seek(0, SeekOrigin.End);
                     sw.WriteLine("{0}", "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
                     sw.Flush();
                 }
             }
#endif
        }
        public static void WriteErrorLine(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Error.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
            Console.ResetColor();
        }
    }
}