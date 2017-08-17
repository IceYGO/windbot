using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Graydle", "AI_Graydle")]
    public class GraydleExecutor : DefaultExecutor
    {
        public enum CardId
        {
            黑洞 = 53129443,
            宇宙旋风 = 8267140,
            神之宣告 = 41420027,
            神之警告 = 84749824,
            神之通告 = 40605147
        }

        public GraydleExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.宇宙旋风, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之宣告, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之警告, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之通告, DefaultTrap);
            AddExecutor(ExecutorType.Activate, DontChainMyself);
            AddExecutor(ExecutorType.MonsterSet);
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        private bool DontChainMyself()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return LastChainPlayer != 0;
        }

        // will be added soon...?
    }
}