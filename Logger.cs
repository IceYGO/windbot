using System;

namespace WindBot
{
    public static class Logger
    {
        private static readonly object _syncRoot = new object();

        // Each server-mode bot is driven by its own worker thread, so its log context must not be shared.
        [ThreadStatic]
        private static Func<string> _contextProvider;

        public static void SetContext(Func<string> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public static void ClearContext()
        {
            _contextProvider = null;
        }

        public static void WriteLine(string message)
        {
            lock (_syncRoot)
                Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
        }

        public static void DebugWriteLine(string message, bool printStackTrace = false)
        {
#if DEBUG
            lock (_syncRoot)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);
                if (printStackTrace)
                    Console.WriteLine(new System.Diagnostics.StackTrace(1, true).ToString());
            }
#endif
        }

        public static void WriteErrorLine(string message)
        {
            WriteErrorLine(message, null, new System.Diagnostics.StackTrace(1, true).ToString());
        }

        public static void WriteErrorLine(string message, Exception exception)
        {
            WriteErrorLine(message, exception, null);
        }

        private static void WriteErrorLine(string message, Exception exception, string callStack)
        {
            lock (_syncRoot)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Error.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + message);

                string context = _contextProvider?.Invoke();
                if (!string.IsNullOrEmpty(context))
                    Console.Error.WriteLine("Context: " + context);
                if (exception != null)
                    Console.Error.WriteLine(exception);
                else if (!string.IsNullOrEmpty(callStack))
                {
                    Console.Error.WriteLine("Call stack:");
                    Console.Error.WriteLine(callStack);
                }

                Console.ResetColor();
            }
        }
    }
}
