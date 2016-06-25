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
            AddExecutor(ExecutorType.Activate, DontChainMyself);
            AddExecutor(ExecutorType.SummonOrSet);
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet);
        }

        private bool DontChainMyself()
        {
            return LastChainPlayer != 0;
        }
    }
}