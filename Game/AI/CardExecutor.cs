using System;
using YGOSharp.OCGWrapper;

namespace WindBot.Game.AI
{
    public class CardExecutor
    {
        public int CardId { get; private set; }
        public ExecutorType Type { get; private set; }
        public Func<bool> Func { get; private set; }

        public CardExecutor(ExecutorType type, int cardId, Func<bool> func)
        {
            NamedCard card = NamedCard.Get(cardId);
            CardId = card != null && NamedCard.IsAltartAlias(card.Id, card.Alias) ? card.Alias : cardId;
            Type = type;
            Func = func;
        }
    }
}