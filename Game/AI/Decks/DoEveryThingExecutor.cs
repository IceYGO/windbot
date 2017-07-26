﻿using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Test", "AI_Test")]
    public class DoEverythingExecutor : DefaultExecutor
    {
        public enum CardId
        {
            LeoWizard = 4392470,
            Bunilla = 69380702
        }

        public DoEverythingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Activate, DontChainMyself);
            AddExecutor(ExecutorType.SummonOrSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            IList<ClientCard> selected = new List<ClientCard>();

            // select the last cards
            for (int i = 1; i <= max; ++i)
                selected.Add(cards[cards.Count-i]);

            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            return Program.Rand.Next(options.Count);
        }

        private bool DontChainMyself()
        {
            return LastChainPlayer != 0;
        }
    }
}