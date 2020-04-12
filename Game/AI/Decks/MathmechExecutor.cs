using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("MathMech", "AI_Mathmech", "NotFinished")]
    public class MathmechExecutor : DefaultExecutor
    {

        public MathmechExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            
        }

    }
}
