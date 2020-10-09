using System;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Sample", "AI_Sample")]
    public class SampleExecutor : DefaultExecutor
    {
    public SampleExecutor(GameAI ai, Duel duel)
        : base(ai, duel)
    {
    }
}
}