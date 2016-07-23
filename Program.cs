using YGOSharp.OCGWrapper;
using System;
using System.IO;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot
{
    public class Program
    {
        public static short ProVersion = 0x133A;
        public static int PlayerNameSize = 20;

        internal static Random Rand;
        internal static void Main()
        {
#if !DEBUG
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex);
            }
#else
            Run();
#endif
        }

        public static void Init(string databasePath)
        {
            Rand = new Random();
            DecksManager.Init();
            InitCardsManager(databasePath);
        }

        private static void Run()
        {
            Init("cards.cdb");
            GameClient client;
            string EnvName = Environment.GetEnvironmentVariable("YGOPRO_NAME");
            if (EnvName != null)
            {
                ProVersion = Int16.Parse(Environment.GetEnvironmentVariable("YGOPRO_VERSION"));
                client = new GameClient(EnvName, Environment.GetEnvironmentVariable("YGOPRO_DECK"), Environment.GetEnvironmentVariable("YGOPRO_HOST"), Int32.Parse(Environment.GetEnvironmentVariable("YGOPRO_PORT")));
            }
            else
            {
                client = new GameClient("谜之剑士LV4", "Blue-Eyes", "127.0.0.1", 7911);
            }
            client.Start();
            while (client.Connection.IsConnected)
            {
                client.Tick();
                Thread.Sleep(30);
            }
        }

        private static void InitCardsManager(string databasePath)
        {
            string currentPath = Path.GetFullPath(".");
            string absolutePath = Path.Combine(currentPath, databasePath);
            NamedCardsManager.Init(absolutePath);
        }
    }
}
