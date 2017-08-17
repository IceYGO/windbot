namespace WindBot.Game.AI.Decks
{
    [Deck("OldSchool", "AI_OldSchool")]
    public class OldSchoolExecutor : DefaultExecutor
    {
        public enum CardId
        {
            Raigeki = 12580477
        }

        public OldSchoolExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, 19613556, DefaultHeavyStorm);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Activate, 53129443, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, 26412047, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, 66788016);
            AddExecutor(ExecutorType.Activate, 72302403, SwordsOfRevealingLight);
            AddExecutor(ExecutorType.Activate, 43422537, DoubleSummon);

            AddExecutor(ExecutorType.Summon, 83104731, DefaultTributeSummon);
            AddExecutor(ExecutorType.Summon, 6631034, DefaultTributeSummon);
            AddExecutor(ExecutorType.SummonOrSet, 43096270);
            AddExecutor(ExecutorType.SummonOrSet, 69247929);
            AddExecutor(ExecutorType.MonsterSet, 30190809);
            AddExecutor(ExecutorType.SummonOrSet, 77542832);
            AddExecutor(ExecutorType.SummonOrSet, 11091375);
            AddExecutor(ExecutorType.SummonOrSet, 35052053);
            AddExecutor(ExecutorType.SummonOrSet, 49881766);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, 44095762, DefaultTrap);
            AddExecutor(ExecutorType.Activate, 70342110, DefaultTrap);
        }

        private int _lastDoubleSummon;

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == 83104731)
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool DoubleSummon()
        {
            if (_lastDoubleSummon == Duel.Turn)
                return false;

            if (Main.SummonableCards.Count == 0)
                return false;

            if (Main.SummonableCards.Count == 1 && Main.SummonableCards[0].Level < 5)
            {
                bool canTribute = false;
                foreach (ClientCard handCard in Bot.Hand)
                {
                    if (handCard.IsMonster() && handCard.Level > 4 && handCard.Level < 6)
                        canTribute = true;
                }
                if (!canTribute)
                    return false;
            }

            int monsters = 0;
            foreach (ClientCard handCard in Bot.Hand)
            {
                if (handCard.IsMonster())
                    monsters++;
            }
            if (monsters <= 1)
                return false;

            _lastDoubleSummon = Duel.Turn;
            return true;
        }

        private bool SwordsOfRevealingLight()
        {
            foreach (ClientCard handCard in Enemy.GetMonsters())
            {
                if (handCard.IsFacedown())
                    return true;
            }
            return AI.Utils.IsEnemyBetter(true, false);
        }
    }
}