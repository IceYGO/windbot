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
            public static int AncientGearGolem = 83104731;
            public static int Frostosaurus = 6631034;
            public static int AlexandriteDragon = 43096270;
            public static int GeneWarpedWarwolf = 69247929;
            public static int GearGolemTheMovingFortress = 30190809;
            public static int EvilswarmHeliotrope = 77542832;
            public static int LusterDragon = 11091375;
            public static int InsectKnight = 35052053;
            public static int ArchfiendSoldier = 49881766;

            public static int HeavyStorm = 19613556;
            public static int DarkHole = 53129443;
            public static int Raigeki = 12580477;
            public static int HammerShot = 26412047;
            public static int Fissure = 66788016;
            public static int SwordsOfRevealingLight = 72302403;
            public static int DoubleSummon = 43422537;

            public static int MirrorForce = 44095762;
            public static int DimensionalPrison = 70342110;

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

            AddExecutor(ExecutorType.Summon, CardId.AncientGearGolem, DefaultTributeSummon);
            AddExecutor(ExecutorType.Summon, CardId.Frostosaurus, DefaultTributeSummon);
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

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == CardId.AncientGearGolem)
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
            return AI.Utils.IsOneEnemyBetter(true);
        }
    }
}