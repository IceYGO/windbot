using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Evilswarm", "AI_Evilswarm")]
    public class EvilswarmExecutor : DefaultExecutor
    {
        public enum CardId
        {
            黑洞 = 53129443,
            宇宙旋风 = 8267140,
            侵略的泛发感染 = 27541267,
            神之宣告 = 41420027,
            神之警告 = 84749824,
            神之通告 = 40605147
        }

        public EvilswarmExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.宇宙旋风, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之宣告, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之警告, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之通告, DefaultSolemnStrike);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.侵略的泛发感染);
            AddExecutor(ExecutorType.Activate, DefaultDontChainMyself);
            AddExecutor(ExecutorType.Summon);
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        // will be added soon...?
    }
}