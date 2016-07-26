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
    public class Program
    {
#if DEBUG
        public static bool DebugMode = true;
#else
        public static bool DebugMode = false;
#endif

        internal static Random Rand;

        internal static void Main(string[] args)
        {
            InitDatas("cards.cdb");

            int argc = args.Length;

            // If the first commandline parameter is DebugMode
            if (argc > 0 && args[0] == "DebugMode")
            {
                DebugMode = true;
                // Shift the args array to skip the first parameter
                argc--;
                Array.Copy(args, 1, args, 0, argc);
            }

            // Only one parameter will make Windbot run as a server, use the parameter as port
            // provide a http interface to create bot.
            // eg. http://127.0.0.1:2399/?name=%E2%91%A8&deck=Blue-Eyes&host=127.0.0.1&port=7911&dialog=cirno.zh-CN&version=4922
            if (argc == 1)
            {
                RunAsServer(Int32.Parse(args[0]));
            }

            // Use all five parameters to run Windbot
            // The parameters should be name, deck, server ip, server port, password
            // eg. WindBot.exe "My Bot" "Zexal Weapons" 127.0.0.1 7911 ""
            else if (argc == 5)
            {
                RunFromArgs(args);
            }

            // Use environment variables to run Windbot
            // List of variables required:
            // YGOPRO_HOST
            // YGOPRO_PORT
            // YGOPRO_NAME
            // YGOPRO_DECK
            //
            // List of variables optional:
            // YGOPRO_VERSION
            // YGOPRO_DIALOG
            // YGOPRO_PASSWORD
            //
            // eg. (cmd)
            // set YGOPRO_VERSION=4922
            // set YGOPRO_HOST=127.0.0.1
            // set YGOPRO_PORT=7911
            // set YGOPRO_NAME=Meow
            // set YGOPRO_DECK=Blue-Eyes
            // set YGOPRO_DIALOG=zh-CN
            // WindBot.exe
            else if (Environment.GetEnvironmentVariable("YGOPRO_NAME") != null)
            {
                RunFromEnv();
            }

            // Else, run from the default settings
            else
            {
                WindBotInfo Info = new WindBotInfo();
                Run(Info);
            }
        }

        public static void InitDatas(string databasePath)
        {
            Rand = new Random();
            DecksManager.Init();
            string absolutePath = Path.GetFullPath(databasePath);
            NamedCardsManager.Init(absolutePath);
        }

        private static void RunFromArgs(string[] args)
        {
            WindBotInfo Info = new WindBotInfo();
            Info.Name = args[0];
            Info.Deck = args[1];
            Info.Host = args[2];
            Info.Port = Int32.Parse(args[3]);
            Info.HostInfo = args[4];
            Run(Info);
        }

        private static void RunFromEnv()
        {
            WindBotInfo Info = new WindBotInfo();
            Info.Name = Environment.GetEnvironmentVariable("YGOPRO_NAME");
            Info.Deck = Environment.GetEnvironmentVariable("YGOPRO_DECK");
            Info.Host = Environment.GetEnvironmentVariable("YGOPRO_HOST");
            Info.Port = Int32.Parse(Environment.GetEnvironmentVariable("YGOPRO_PORT"));
            string EnvDialog = Environment.GetEnvironmentVariable("YGOPRO_DIALOG");
            if (EnvDialog != null)
                Info.Dialog = EnvDialog;
            string EnvVersion = Environment.GetEnvironmentVariable("YGOPRO_VERSION");
            if (EnvVersion != null)
                Info.Version = Int16.Parse(EnvVersion);
            string EnvPassword = Environment.GetEnvironmentVariable("YGOPRO_PASSWORD");
            if (EnvPassword != null)
                Info.HostInfo = EnvPassword;

            GameClient client = new GameClient(Info);
            client.Start();
            while (client.Connection.IsConnected)
            {
                client.Tick();
                Thread.Sleep(30);
            }
        }

        private static void RunAsServer(int ServerPort)
        {
            using (HttpListener MainServer = new HttpListener())
            {
                MainServer.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                MainServer.Prefixes.Add("http://127.0.0.1:2399/");
                MainServer.Start();
                Logger.WriteLine("Windbot Server Start Successed.");
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

                        if (Info.Name == null || Info.Deck == null || Info.Host == null || port == null)
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
                                if (!DebugMode)
                                    Logger.WriteErrorLine("Start Thread Error: " + ex);
                                else
                                    throw ex;
                            }
                            ctx.Response.StatusCode = 200;
                            ctx.Response.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!DebugMode)
                            Logger.WriteErrorLine("Parse Http Request Error: " + ex);
                        else
                            throw ex;
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
                    catch (Exception ex)
                    {
                        if (!DebugMode)
                            Logger.WriteErrorLine("Tick Error: " + ex);
                        else
                            throw ex;
                    }
                }
                Logger.DebugWriteLine(client.Username + " end.");
                //System.Threading.Thread.CurrentThread.Abort();
            }
            //catch (ThreadAbortException)
            //{
                //if (DebugMode)
                //    throw ex;
            //}
            catch (Exception ex)
            {
                if (!DebugMode)
                    Logger.WriteErrorLine("Run Error: " + ex);
                else
                    throw ex;
            }
        }
    }
}
