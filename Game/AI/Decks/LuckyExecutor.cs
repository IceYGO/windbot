using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Lucky", "AI_Test", "Test")]
    public class LuckyExecutor : DefaultExecutor
    {
        public LuckyExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpSummon, ImFeelingLucky);
            AddExecutor(ExecutorType.Activate, ImFeelingLucky);
            AddExecutor(ExecutorType.SummonOrSet, ImFeelingLucky);
            AddExecutor(ExecutorType.SpellSet, ImFeelingLucky);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, int hint, bool cancelable)
        {
            if (Duel.Phase == DuelPhase.BattleStart)
                return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> cards = new List<ClientCard>(_cards);
            IList<ClientCard> selected = new List<ClientCard>();

            if (max > cards.Count)
                max = cards.Count;

            // select random cards
            while (selected.Count < max)
            {
                ClientCard card = cards[Program.Rand.Next(cards.Count)];
                selected.Add(card);
                cards.Remove(card);
            }

            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            return Program.Rand.Next(options.Count);
        }

        private bool ImFeelingLucky()
        {
            return Program.Rand.Next(9) >= 3 && DefaultDontChainMyself();
        }
    }
}