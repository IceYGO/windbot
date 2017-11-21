using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BotWrapper
{
    class BotWrapper
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, int uType);

        static void Main(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetFullPath("WindBot");
            startInfo.FileName = startInfo.WorkingDirectory + "\\WindBot.exe";

            if (args.Length == 3)
            {
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                string arg = args[0].Replace("'","\"");
                if (int.Parse(args[1]) == 1)
                {
                    arg += " Hand=1";
                }
                arg += " Port=" + args[2];
                startInfo.Arguments = arg;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                MessageBox((IntPtr)0, "WindBot can't be started!", "WindBot", 0x00000010); // MB_ICONERROR
            }
        }
    }
}
