using System.Collections.Generic;

namespace WindBot.Game
{
    public class MainPhase
    {
        public IList<ClientCard> SummonableCards { get; private set; }
        public IList<ClientCard> SpecialSummonableCards { get; private set; }
        public IList<ClientCard> ReposableCards { get; private set; }
        public IList<ClientCard> MonsterSetableCards { get; private set; }
        public IList<ClientCard> SpellSetableCards { get; private set; }
        public IList<ClientCard> ActivableCards { get; private set; }
        public IList<int> ActivableDescs { get; private set; }
        public bool CanBattlePhase { get; set; }
        public bool CanEndPhase { get; set; }

        public MainPhase()
        {
            SummonableCards = new List<ClientCard>();
            SpecialSummonableCards = new List<ClientCard>();
            ReposableCards = new List<ClientCard>();
            MonsterSetableCards = new List<ClientCard>();
            SpellSetableCards = new List<ClientCard>();
            ActivableCards = new List<ClientCard>();
            ActivableDescs = new List<int>();
        }
    }
}