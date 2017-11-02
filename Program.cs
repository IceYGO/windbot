using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Web;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;

namespace WindBot
{
    public class Program
    {
        // in safe mode, all errors will be catched instead of causing the program to crash.
#if DEBUG
        public static bool SafeMode = false;
#else
        public static bool SafeMode = true;
#endif

        internal static Random Rand;

        internal static void Main(string[] args)
        {
            Logger.WriteLine("WindBot starting...");

            Config.Load(args);

            string databasePath = Config.GetString("DbPath", "cards.cdb");

            InitDatas(databasePath);

            bool serverMode = Config.GetBool("ServerMode", false);

            if (serverMode)
            {
                int serverPort = Config.GetInt("ServerPort", 2399);
                RunAsServer(serverPort);
            }
            else
            {
                if (args.Length == 0)
                {
                    Logger.WriteLine("=== WARN ===");
                    Logger.WriteLine("No input found, tring to connect to localhost YGOPro host.");
                    Logger.WriteLine("If it fail, the program will quit sliently.");
                }
                RunFromArgs();
            }
        }

        public static void InitDatas(string databasePath)
        {
            Rand = new Random();
            DecksManager.Init();
            string absolutePath = Path.GetFullPath(databasePath);
            if (!File.Exists(absolutePath))
                absolutePath = Path.GetFullPath("../" + databasePath);
            if (!File.Exists(absolutePath))
                Logger.WriteErrorLine("Can't find cards database file. Please place cards.cdb next to WindBot.exe .");
            NamedCardsManager.Init(absolutePath);
        }

        private static void RunFromArgs()
        {
            WindBotInfo Info = new WindBotInfo();
            Info.Name = Config.GetString("Name", Info.Name);
            Info.Deck = Config.GetString("Deck", Info.Deck);
            Info.Dialog = Config.GetString("Dialog", Info.Dialog);
            Info.Host = Config.GetString("Host", Info.Host);
            Info.Port = Config.GetInt("Port", Info.Port);
            Info.HostInfo = Config.GetString("HostInfo", Info.HostInfo);
            Info.Version = Config.GetInt("Version", Info.Version);
            Info.Hand = Config.GetInt("Hand", Info.Hand);
            Run(Info);
        }

        private static void RunAsServer(int ServerPort)
        {
            using (HttpListener MainServer = new HttpListener())
            {
                MainServer.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                MainServer.Prefixes.Add("http://127.0.0.1:" + ServerPort + "/");
                MainServer.Start();
                Logger.WriteLine("WindBot server start successed.");
                Logger.WriteLine("HTTP GET http://127.0.0.1:" + ServerPort + "/?name=WindBot&host=127.0.0.1&port=7911 to call the bot.");
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
                        string port = HttpUtility.ParseQueryString(RawUrl).Get("port");
                        if (port != null)
                            Info.Port = Int32.Parse(port);
                        string dialog = HttpUtility.ParseQueryString(RawUrl).Get("dialog");
                        if (dialog != null)
                            Info.Dialog = dialog;
                        string version = HttpUtility.ParseQueryString(RawUrl).Get("version");
                        if (version != null)
                            Info.Version = Int16.Parse(version);
                        string password = HttpUtility.ParseQueryString(RawUrl).Get("password");
                        if (password != null)
                            Info.HostInfo = password;
                        string hand = HttpUtility.ParseQueryString(RawUrl).Get("hand");
                        if (hand != null)
                            Info.Hand = Int32.Parse(hand);

                        if (Info.Name == null || Info.Host == null || port == null)
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
                            catch (Exception ex) when (SafeMode)
                            {
                                Logger.WriteErrorLine("Start Thread Error: " + ex);
                            }
                            ctx.Response.StatusCode = 200;
                            ctx.Response.Close();
                        }
                    }
                    catch (Exception ex) when (SafeMode)
                    {
                        Logger.WriteErrorLine("Parse Http Request Error: " + ex);
                    }
                }
            }
        }

        private static void Run(object o)
        {
            try
            {
                WindBotInfo Info = (WindBotInfo)o;
                GameClient client = new GameClient(Info);
                client.Start();
                Logger.DebugWriteLine(client.Username + " started.");
                while (client.Connection.IsConnected)
                {
                    try
                    {
                        client.Tick();
                        Thread.Sleep(30);
                    }
                    catch (Exception ex) when (SafeMode)
                    {
                        Logger.WriteErrorLine("Tick Error: " + ex);
                    }
                }
                Logger.DebugWriteLine(client.Username + " end.");
            }
            catch (Exception ex) when (SafeMode)
            {
                Logger.WriteErrorLine("Run Error: " + ex);
            }
        }
    }
}
