using System;

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
<<<<<<< HEAD
<<<<<<< HEAD
            using (FileStream fs = new FileStream(@Path.GetFullPath("log.txt"), FileMode.OpenOrCreate, FileAccess.Write))
              {
                  using (StreamWriter sw = new StreamWriter(fs))
                 {
                     sw.BaseStream.Seek(0, SeekOrigin.End);
                     sw.WriteLine("{0}", "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
                     sw.Flush();
                 }
             }
=======
>>>>>>> parent of 61647f1... Logger output
=======
>>>>>>> parent of 61647f1... Logger output
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