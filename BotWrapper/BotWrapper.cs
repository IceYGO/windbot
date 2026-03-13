using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

namespace BotWrapper
{
    class BotWrapper
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, int uType);

        const int MB_ICONERROR = 0x00000010;

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

                string arg = args[0];
                Match match = Regex.Match(arg, "Random=(.*)");
                if (match.Success)
                {
                    string randomFlag = match.Groups[1].Value;
                    ReadBots();
                    arg = GetRandomBot(randomFlag);
                    if (arg == "")
                    {
                        MessageBox((IntPtr)0, "Can't find random bot with this flag!\n\nA totally random bot will appear instead.", "WindBot", MB_ICONERROR);
                    }
                }
                arg = arg.Replace("'", "\"");
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
                MessageBox((IntPtr)0, "WindBot can't be started!", "WindBot", MB_ICONERROR);
            }
        }

        public class BotInfo
        {
            public string name;
            public string command;
            public string desc;
            public string[] flags;
        }

        static public IList<BotInfo> Bots = new List<BotInfo>();

        static void ReadBots()
        {
            using (StreamReader reader = new StreamReader("bot.conf"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.Length > 0 && line[0] == '!')
                    {
                        BotInfo newBot = new BotInfo();
                        newBot.name = line;
                        newBot.command = reader.ReadLine().Trim();
                        newBot.desc = reader.ReadLine().Trim();
                        line = reader.ReadLine().Trim();
                        newBot.flags = line.Split(' ');
                        Bots.Add(newBot);
                    }
                }
            }
        }

        static string GetRandomBot(string flag)
        {
            IList<BotInfo> foundBots = Bots.Where(bot => bot.flags.Contains(flag)).ToList();
            if (foundBots.Count > 0)
            {
                Random rand = new Random();
                BotInfo bot = foundBots[rand.Next(foundBots.Count)];
                return bot.command;
            }
            return "";
        }
    }
}
