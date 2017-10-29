using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("Evilswarm", "AI_Evilswarm")]
    public class EvilswarmExecutor : DefaultExecutor
    {
        public enum CardId
        {
            DarkHole = 53129443,
            CosmicCyclone = 8267140,
            InfestationPandemic = 27541267,
            SolemnJudgment = 41420027,
            SolemnWarning = 84749824,
            SolemnStrike = 40605147
        }

        public EvilswarmExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.InfestationPandemic);
            AddExecutor(ExecutorType.Activate, DefaultDontChainMyself);
            AddExecutor(ExecutorType.Summon);
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        // will be added soon...?
    }
}