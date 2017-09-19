using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("MokeyMokey", "AI_MokeyMokey")]
    public class MokeyMokeyExecutor : DefaultExecutor
    {
        public enum CardId
        {
            LeoWizard = 4392470,
            Bunilla = 69380702
        }

        private int RockCount = 0;

        public MokeyMokeyExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Summon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        public override int OnRockPaperScissors()
        {
            RockCount++;
            if (RockCount <= 3)
                return 2;
            else
                return base.OnRockPaperScissors();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            return attacker.Attack > 0;
        }
    }
}