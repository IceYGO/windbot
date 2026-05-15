using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Web;
using System.Diagnostics;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;

namespace WindBot
{
    public class Program
    {
        internal static Random Rand;
        internal static bool ServerMode;

        internal static void Main(string[] args)
        {
            Logger.WriteLine("WindBot starting...");

            Config.Load(args);

            string databasePath = Config.GetString("DbPath", "cards.cdb");

            InitDatas(databasePath);

            ServerMode = Config.GetBool("ServerMode", false);

            if (ServerMode)
            {
                // Run in server mode, provide a http interface to create bot.
                int serverPort = Config.GetInt("ServerPort", 2399);
                RunAsServer(serverPort);
            }
            else
            {
                // Join the host specified on the command line.
                if (args.Length == 0)
                {
                    Logger.WriteErrorLine("=== WARN ===");
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
                // In case windbot is placed in a folder under ygopro folder
                absolutePath = Path.GetFullPath("../" + databasePath);
            if (!File.Exists(absolutePath))
                // In case windbot is placed in a folder under ygopro2 folder
                absolutePath = Path.GetFullPath("../cdb/" + databasePath);
            if (!File.Exists(absolutePath))
            {
                Logger.WriteErrorLine("Can't find cards database file.");
                Logger.WriteErrorLine("Please place cards.cdb next to WindBot.exe or Bot.exe .");
                Logger.WriteLine("Press any key to quit...");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            NamedCardsManager.Init(absolutePath);
        }

        private static void RunFromArgs()
        {
            WindBotInfo Info = new WindBotInfo();
            Info.Name = Config.GetString("Name", Info.Name);
            Info.Deck = Config.GetString("Deck", Info.Deck);
            Info.DeckFile = Config.GetString("DeckFile", Info.DeckFile);
            Info.Dialog = Config.GetString("Dialog", Info.Dialog);
            Info.Host = Config.GetString("Host", Info.Host);
            Info.Port = Config.GetInt("Port", Info.Port);
            Info.HostInfo = Config.GetString("HostInfo", Info.HostInfo);
            Info.Version = Config.GetInt("Version", Info.Version);
            Info.Hand = Config.GetInt("Hand", Info.Hand);
            Info.Debug = Config.GetBool("Debug", Info.Debug);
            Info.Chat = Config.GetBool("Chat", Info.Chat);
            Run(Info);
        }

        private static void RunAsServer(int ServerPort)
        {
            using (HttpListener MainServer = new HttpListener())
            {
                MainServer.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                MainServer.Prefixes.Add("http://+:" + ServerPort + "/");
                MainServer.Start();
                Logger.WriteLine("WindBot server start successed.");
                Logger.WriteLine("HTTP GET http://127.0.0.1:" + ServerPort + "/?name=WindBot&host=127.0.0.1&port=7911 to call the bot.");
                while (true)
                {
                    try
                    {
                    HttpListenerContext ctx = MainServer.GetContext();
                    var queryParams = HttpUtility.ParseQueryString(ctx.Request.Url.Query);

                    WindBotInfo Info = new WindBotInfo();
                    Info.Name = queryParams.Get("name");
                    Info.Deck = queryParams.Get("deck");
                    Info.Host = queryParams.Get("host");
                    string port = queryParams.Get("port");
                    if (port != null)
                        Info.Port = Int32.Parse(port);
                    string deckfile = queryParams.Get("deckfile");
                    if (deckfile != null)
                        Info.DeckFile = deckfile;
                    string dialog = queryParams.Get("dialog");
                    if (dialog != null)
                        Info.Dialog = dialog;
                    string version = queryParams.Get("version");
                    if (version != null)
                        Info.Version = Int16.Parse(version);
                    string password = queryParams.Get("password");
                    if (password != null)
                        Info.HostInfo = password;
                    string hand = queryParams.Get("hand");
                    if (hand != null)
                        Info.Hand = Int32.Parse(hand);
                    string debug = queryParams.Get("debug");
                    if (debug != null)
                        Info.Debug= bool.Parse(debug);
                    string chat = queryParams.Get("chat");
                    if (chat != null)
                        Info.Chat = bool.Parse(chat);

                    if (Info.Name == null || Info.Host == null || port == null)
                    {
                        ctx.Response.StatusCode = 400;
                        ctx.Response.Close();
                    }
                    else
                    {
                        ctx.Response.StatusCode = 200;
                        try
                        {
                            Thread workThread = new Thread(new ParameterizedThreadStart(Run));
                            workThread.Start(Info);
                        }
                        catch (Exception ex)
                        {
                            if (Debugger.IsAttached)
                                throw;
                            Logger.WriteErrorLine("Start Thread Error: " + ex);
                            ctx.Response.StatusCode = 500;
                        }
                        ctx.Response.Close();
                    }
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                            throw;
                        Logger.WriteErrorLine("Parse Http Request Error: " + ex);
                    }
                }
            }
        }

        private static void Run(object o)
        {
            // All errors should be caught instead of causing the program to crash.
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
                        #if DEBUG
                            Thread.Sleep(1);
                        #else
                            Thread.Sleep(30);
                        #endif
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                            throw;
                        Logger.WriteErrorLine("Tick Error: " + ex);
                    }
                }
                Logger.DebugWriteLine(client.Username + " end.");
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    throw;
                Logger.WriteErrorLine("Run Error: " + ex);
            }
        }

        public static FileStream ReadFile(string directory, string filename, string extension)
        {
            string tryfilename = filename + "." + extension;
            string fullpath = Path.Combine(directory, tryfilename);
            if (!File.Exists(fullpath))
                fullpath = filename;
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../", filename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../deck/", filename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../", tryfilename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../deck/", tryfilename);
            return new FileStream(fullpath, FileMode.Open, FileAccess.Read);
        }
    }
}
