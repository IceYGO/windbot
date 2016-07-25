using System;

namespace WindBot
{
    public static class Logger
    {
        public static void WriteLine(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message);
        }
        public static void WriteErrorLine(string message)
        {
            Console.Error.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message);
        }
    }
}