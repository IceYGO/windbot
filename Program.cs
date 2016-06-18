using OCGWrapper;
using System;
using System.IO;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot
{
    public class Program
    {
        public static short ProVersion = 0x1330;
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

            // Start two clients and connect them to the same server. Which deck is gonna win?
            GameClient clientA = new GameClient("Wind", "Horus", "127.0.0.1", 7911);
            GameClient clientB = new GameClient("Fire", "OldSchool", "127.0.0.1", 7911);
            clientA.Start();
            clientB.Start();
            while (clientA.Connection.IsConnected || clientB.Connection.IsConnected)
            {
                clientA.Tick();
                clientB.Tick();
                Thread.Sleep(1);
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
