using System;

namespace WindBot.Game.AI
{
    public class CardExecutor
    {
        public int CardId { get; private set; }
        public ExecutorType Type { get; private set; }
        public Func<bool> Func { get; private set; }

        public CardExecutor(ExecutorType type, int cardId, Func<bool> func)
        {
            CardId = cardId;
            Type = type;
            Func = func;
        }
    }
}