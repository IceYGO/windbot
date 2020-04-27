using System;
#if LIBWINDBOT
using Android.Util;
#endif

namespace WindBot
{
    public static class Logger
    {
        public static void WriteLine(string message)
        {
#if !LIBWINDBOT
            Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
#else
			Log.Info("Edoprowindbot", "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
#endif
        }
        public static void DebugWriteLine(string message)
        {
#if DEBUG
#if !LIBWINDBOT
            Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
#else
            Log.Debug("Edoprowindbot", "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
#endif
#endif
        }
        public static void WriteErrorLine(string message)
        {
#if !LIBWINDBOT
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Error.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
            Console.ResetColor();
#else
            Log.Error("Edoprowindbot", "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
#endif
        }
    }
}