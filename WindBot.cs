using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WindBot
{
    public class WindBot
    {
        public static void InitAndroid(string assetPath)
        {
            NamedCardsManager.SetThreadSafe();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Program.Rand = new Random();
            Program.AssetPath = assetPath;
            DecksManager.Init();
        }

        public static void AddDatabase(string databasePath)
        {
            try
            {
                NamedCardsManager.LoadDatabase(databasePath);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Failed loading database: " + databasePath + " error: " + ex);
            }
        }

        [DataContract]
        public class LaunchData
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Deck { get; set; }
            [DataMember]
            public string DeckFile { get; set; }
            [DataMember]
            public string Dialog { get; set; }
            [DataMember]
            public string Host { get; set; }
            [DataMember]
            public string Port { get; set; }
            [DataMember]
            public string HostInfo { get; set; }
            [DataMember]
            public string Version { get; set; }
            [DataMember]
            public string Hand { get; set; }
            [DataMember]
            public string Debug { get; set; }
            [DataMember]
            public string Chat { get; set; }
        }

        public static void RunAndroid(string arg)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            WindBotInfo Info = new WindBotInfo();
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LaunchData));
                MemoryStream json = new MemoryStream(Encoding.Unicode.GetBytes(arg));
                LaunchData data = (LaunchData)serializer.ReadObject(json);
                if (data.Name != null) Info.Name = data.Name;
                if (data.Deck != null) Info.Deck = data.Deck;
                if (data.DeckFile != null) Info.DeckFile = data.DeckFile;
                if (data.Dialog != null) Info.Dialog = data.Dialog;
                if (data.Port != null) Info.Port = int.Parse(data.Port);
                if (data.Hand != null) Info.Hand = int.Parse(data.Hand);
                if (data.Host != null) Info.Host = data.Host;
                if (data.HostInfo != null) Info.HostInfo = data.HostInfo;
                if (data.Version != null) Info.Version = int.Parse(data.Version);
                if (data.Chat != null) Info.Chat = int.Parse(data.Chat) != 0;
                if (data.Debug != null) Info.Debug = int.Parse(data.Debug) != 0;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Argument parsing error: " + ex);
            }
            Thread workThread = new Thread(new ParameterizedThreadStart(Run));
            workThread.Start(Info);
        }

        private static void Run(object o)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
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
                        client.Chat("I crashed, check the crash.log file in the WindBot folder", true);
                        using (StreamWriter sw = File.AppendText(Path.Combine(Program.AssetPath, "crash.log")))
                        {
                            sw.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] Tick Error: " + ex);
                        }
                        client.Connection.Close();
                        return;
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
