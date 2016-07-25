using YGOSharp.OCGWrapper;
using System;
using System.IO;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using System.Net;
using System.Web;

namespace WindBot
{
    public class WindBotInfo
    {
        public string Name { get; set; }
        public string Deck { get; set; }
        public string Host { get; set; }
        public string Dialog { get; set; }
        public int Port { get; set; }
        public int Version { get; set; }
    }

    public class Program
    {
        public static short ProVersion = 0x133A;
        public static int PlayerNameSize = 20;

        internal static Random Rand;

        internal static void Main()
        {
            Init("cards.cdb");
            using (HttpListener MainServer = new HttpListener())
            {
                MainServer.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                MainServer.Prefixes.Add("http://127.0.0.1:2399/");
                MainServer.Start();
                Console.WriteLine("Windbot Server Start Successed.");
                while (true)
                {
                    try
                    {
                        HttpListenerContext ctx = MainServer.GetContext();

                        WindBotInfo Info = new WindBotInfo();
                        string RawUrl = Path.GetFileName(ctx.Request.RawUrl);
                        Info.Name = HttpUtility.ParseQueryString(RawUrl).Get("name");
                        Info.Deck = HttpUtility.ParseQueryString(RawUrl).Get("deck");
                        Info.Host = HttpUtility.ParseQueryString(RawUrl).Get("host");
                        Info.Dialog = HttpUtility.ParseQueryString(RawUrl).Get("dialog");
                        string port = HttpUtility.ParseQueryString(RawUrl).Get("port");
                        if (port != null)
                            Info.Port = Int32.Parse(port);
                        string version = HttpUtility.ParseQueryString(RawUrl).Get("version");
                        if (version != null)
                            Info.Version = Int16.Parse(version);

                        if (Info.Name == null || Info.Deck == null || Info.Host == null || Info.Dialog == null || port == null || version == null)
                        {
                            ctx.Response.StatusCode = 400;
                            ctx.Response.Close();
                        }
                        else
                        {
                            try
                            {
                                Thread workThread = new Thread(new ParameterizedThreadStart(Run));
                                workThread.Start(Info);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteErrorLine("Start Thread Error: " + ex);
                            }
                            ctx.Response.StatusCode = 200;
                            ctx.Response.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Parse Http Request Error: " + ex);
                    }
                }
            }
        }

        public static void Init(string databasePath)
        {
            Rand = new Random();
            DecksManager.Init();
            InitCardsManager(databasePath);
        }

        private static void InitCardsManager(string databasePath)
        {
            string currentPath = Path.GetFullPath(".");
            string absolutePath = Path.Combine(currentPath, databasePath);
            NamedCardsManager.Init(absolutePath);
        }

        private static void Run(object o)
        {
#if !DEBUG
            try
            {
                WindBotInfo Info = (WindBotInfo)o;
                GameClient client = new GameClient(Info.Name, Info.Deck, Info.Host, Info.Port, Info.Dialog);
                client.Start();
                Logger.WriteLine(client.Username + " started.");
                while (client.Connection.IsConnected)
                {
                    try
                    {
                        client.Tick();
                        Thread.Sleep(30);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLine("Tick Error: " + ex);
                    }
                }
                Logger.WriteLine(client.Username + " end.");
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine("Run Error: " + ex);
            }
#else
            WindBotInfo Info = (WindBotInfo)o;
            GameClient client = new GameClient(Info.Name, Info.Deck, Info.Host, Info.Port, Info.Dialog);
            client.Start();
            Logger.WriteLine(client.Username + " started.");
            while (client.Connection.IsConnected)
            {
                client.Tick();
                Thread.Sleep(30);
            }
            Logger.WriteLine(client.Username + " end.");
#endif
        }
    }
}
