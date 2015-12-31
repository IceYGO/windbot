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
        public const short ProVersion = 0x1338;

        public static Random Rand;

        public static void Main()
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

        private static void Run()
        {
            Rand = new Random();
            DecksManager.Init();
            InitCardsManager();

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

        private static void InitCardsManager()
        {
            string currentPath = Path.GetFullPath(".");
            string absolutePath = Path.Combine(currentPath, "cards.cdb");
            NamedCardsManager.Init(absolutePath);
        }
    }
}
