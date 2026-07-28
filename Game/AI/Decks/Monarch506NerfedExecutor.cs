using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Monarch506Nerfed", "AI_Monarch506")]
    class Monarch506NerfedExecutor : Monarch506Executor
    {
        public Monarch506NerfedExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            /*
            命运英雄 圆盘人：原版可当回合复活、可重复触发且属于必发效果；削弱版限制当回合复活并记录一决斗一次。
            三眼怪：削弱模式避免优先检索需要当回合发动效果的卡。
            死之卡组破坏病毒：原版不受伤害归零副作用影响，并考虑三回合持续收益；削弱版保留更谨慎的发动条件。
            洗脑：原版允许选择任意合法表侧怪兽；削弱版只考虑可通常召唤/盖放的怪兽。
            */
            UseNerfedCardEffects = true;
        }
    }
}
