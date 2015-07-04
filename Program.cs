using System;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;

namespace WindBot
{
    public class Program
    {
        public const short ProVersion = 0x1335;

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
            CardsManager.Init();
            DecksManager.Init();

            // Start two clients and connect them to the same room. Which deck is gonna win?
            GameClient clientA = new GameClient("Wind", "Horus", "127.0.0.1", 13254, "000");
            GameClient clientB = new GameClient("Fire", "OldSchool", "127.0.0.1", 13254, "000");
            clientA.Start();
            clientB.Start();
            while (clientA.Connection.IsConnected || clientB.Connection.IsConnected)
            {
                clientA.Tick();
                clientB.Tick();
                Thread.Sleep(1);
            }
        }
    }
}
