using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;

namespace WindBot
{
    public class WindBot
    {
        public static void InitAndroid(string assetPath)
        {
            Program.Rand = new Random();
            Program.AssetPath = assetPath;
            DecksManager.Init();
            try
            {
                NamedCardsManager.Init(assetPath + "/cards.cdb");
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e.ToString());
            }
        }

        private static IList<string> ParseArgs(string arg)
        {
            return Regex.Split(arg, "(?<=^[^\']*(?:\'[^\']*\'[^\']*)*) (?=(?:[^\']*\'[^\']*\')*[^\']*$)").ToList(); // https://stackoverflow.com/questions/4780728/regex-split-string-preserving-quotes/
        }

        public static void RunAndroid(string arg)
        {
            IList<string> args = ParseArgs(arg);
            WindBotInfo Info = new WindBotInfo();
            foreach (string param in args)
            {
                string[] p = Regex.Split(param, "[=]");
                p[1] = p[1].Replace("'", "");
                if (p[0] == "Name") Info.Name = p[1];
                if (p[0] == "Deck") Info.Deck = p[1];
                if (p[0] == "Dialog") Info.Dialog = p[1];
                if (p[0] == "Port") Info.Port = int.Parse(p[1]);
                if (p[0] == "Hand") Info.Hand = int.Parse(p[1]);
                if (p[0] == "Host") Info.Host = p[1];
                if (p[0] == "HostInfo") Info.HostInfo = p[1];
                if (p[0] == "Version") Info.Version = int.Parse(p[1]);
                if (p[0] == "Chat") Info.Chat = int.Parse(p[1]) != 0;
                if (p[0] == "Debug") Info.Debug = int.Parse(p[1]) != 0;
            }
            Thread workThread = new Thread(new ParameterizedThreadStart(Run));
            workThread.Start(Info);
        }

        private static void Run(object o)
        {
#if !DEBUG
            try
            {
                //all errors will be catched instead of causing the program to crash.
#endif
                WindBotInfo Info = (WindBotInfo)o;
                GameClient client = new GameClient(Info);
                client.Start();
                Logger.DebugWriteLine(client.Username + " started.");
                while (client.Connection.IsConnected)
                {
#if !DEBUG
                    try
                    {
#endif
                        client.Tick();
                        Thread.Sleep(30);
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Tick Error: " + ex);
                    }
#endif
                }
                Logger.DebugWriteLine(client.Username + " end.");
#if !DEBUG
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Run Error: " + ex);
            }
#endif
        }
    }

    public class Program
    {
        public static string AssetPath;
        internal static Random Rand;
    }
}
