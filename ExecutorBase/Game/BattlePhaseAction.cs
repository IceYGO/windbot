namespace WindBot.Game
{
    public class BattlePhaseAction
    {
        public enum BattleAction
        {
            Activate = 0,
            Attack = 1,
            ToMainPhaseTwo = 2,
            ToEndPhase = 3
        }

        public BattleAction Action { get; private set; }
        public int Index { get; private set; }

        public BattlePhaseAction(BattleAction action)
        {
            Action = action;
            Index = 0;
        }

        public BattlePhaseAction(BattleAction action, int[] indexes)
        {
            Action = action;
            Index = indexes[(int)action];
        }

        public int ToValue()
        {
            return (Index << 16) + (int)Action;
        }
    }
}