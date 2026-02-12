using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("OldSchool", "AI_OldSchool", "Easy")]
    public class OldSchoolExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int AncientGearGolem = 83104731;
            public const int Frostosaurus = 6631034;
            public const int AlexandriteDragon = 43096270;
            public const int GeneWarpedWarwolf = 69247929;
            public const int GearGolemTheMovingFortress = 30190809;
            public const int EvilswarmHeliotrope = 77542832;
            public const int LusterDragon = 11091375;
            public const int InsectKnight = 35052053;
            public const int ArchfiendSoldier = 49881766;

            public const int HeavyStorm = 19613556;
            public const int DarkHole = 53129443;
            public const int Raigeki = 12580477;
            public const int HammerShot = 26412047;
            public const int Fissure = 66788016;
            public const int SwordsOfRevealingLight = 72302403;
            public const int DoubleSummon = 43422537;

            public const int MirrorForce = 44095762;
            public const int DimensionalPrison = 70342110;

        }

        public OldSchoolExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, DefaultHeavyStorm);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.HammerShot, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, CardId.Fissure);
            AddExecutor(ExecutorType.Activate, CardId.SwordsOfRevealingLight, SwordsOfRevealingLight);
            AddExecutor(ExecutorType.Activate, CardId.DoubleSummon, DoubleSummon);

            AddExecutor(ExecutorType.Summon, CardId.AncientGearGolem, DefaultMonsterSummon);
            AddExecutor(ExecutorType.Summon, CardId.Frostosaurus, DefaultMonsterSummon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AlexandriteDragon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.GeneWarpedWarwolf);
            AddExecutor(ExecutorType.MonsterSet, CardId.GearGolemTheMovingFortress);
            AddExecutor(ExecutorType.SummonOrSet, CardId.EvilswarmHeliotrope);
            AddExecutor(ExecutorType.SummonOrSet, CardId.LusterDragon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.InsectKnight);
            AddExecutor(ExecutorType.SummonOrSet, CardId.ArchfiendSoldier);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalPrison, DefaultTrap);
        }

        private int _lastDoubleSummon;

        private bool DoubleSummon()
        {
            if (_lastDoubleSummon == Duel.Turn)
                return false;

            if (Duel.MainPhase.SummonableCards.Count == 0)
                return false;

            if (Duel.MainPhase.SummonableCards.Count == 1 && Duel.MainPhase.SummonableCards[0].Level < 5)
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
            return Util.IsOneEnemyBetter(true);
        }
    }
}