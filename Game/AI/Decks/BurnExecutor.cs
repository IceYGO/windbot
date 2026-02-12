using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Burn", "AI_Burn", "Easy")]
    public class BurnExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int LavaGolem = 102380;
            public const int ReflectBounder = 2851070;
            public const int FencingFireFerret = 97396380;
            public const int BlastSphere = 26302522;
            public const int Marshmallon = 31305911;
            public const int SpiritReaper = 23205979;
            public const int NaturiaBeans = 44789585;
            public const int ThunderShort = 20264508;
            public const int Ookazi = 19523799;
            public const int GoblinThief = 45311864;
            public const int TremendousFire = 46918794;
            public const int SwordsOfRevealingLight = 72302403;
            public const int SupremacyBerry = 98380593;
            public const int ChainEnergy = 79323590;
            public const int DarkRoomofNightmare = 85562745;
            public const int PoisonOfTheOldMan = 8842266;
            public const int OjamaTrio = 29843091;
            public const int Ceasefire = 36468556;
            public const int MagicCylinder = 62279055;
            public const int MinorGoblinOfficial = 1918087;
            public const int ChainBurst = 48276469;
            public const int SkullInvitation = 98139712;
        }

        public BurnExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Set traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Activate Spells
            AddExecutor(ExecutorType.Activate, CardId.DarkRoomofNightmare);
            AddExecutor(ExecutorType.Activate, CardId.Ookazi);
            AddExecutor(ExecutorType.Activate, CardId.GoblinThief);
            AddExecutor(ExecutorType.Activate, CardId.TremendousFire);
            AddExecutor(ExecutorType.Activate, CardId.SwordsOfRevealingLight, SwordsOfRevealingLight);
            AddExecutor(ExecutorType.Activate, CardId.SupremacyBerry, SupremacyBerry);
            AddExecutor(ExecutorType.Activate, CardId.PoisonOfTheOldMan, PoisonOfTheOldMan);
            AddExecutor(ExecutorType.Activate, CardId.ThunderShort, ThunderShort);

            // Hello, my name is Lava Golem
            AddExecutor(ExecutorType.SpSummon, CardId.LavaGolem, LavaGolem);

            // Set an invincible monster
            AddExecutor(ExecutorType.MonsterSet, CardId.Marshmallon, SetInvincibleMonster);
            AddExecutor(ExecutorType.MonsterSet, CardId.SpiritReaper, SetInvincibleMonster);
            AddExecutor(ExecutorType.MonsterSet, CardId.BlastSphere);

            // Set other monsters
            AddExecutor(ExecutorType.SummonOrSet, CardId.FencingFireFerret);
            AddExecutor(ExecutorType.Summon, CardId.ReflectBounder);
            AddExecutor(ExecutorType.MonsterSet, CardId.NaturiaBeans);

            // We're a coward
            AddExecutor(ExecutorType.Repos, ReposEverything);

            // Chain traps
            AddExecutor(ExecutorType.Activate, CardId.MagicCylinder, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.Ceasefire, Ceasefire);
            AddExecutor(ExecutorType.Activate, CardId.OjamaTrio);
            AddExecutor(ExecutorType.Activate, CardId.MinorGoblinOfficial);
            AddExecutor(ExecutorType.Activate, CardId.ChainBurst);
            AddExecutor(ExecutorType.Activate, CardId.SkullInvitation);
            AddExecutor(ExecutorType.Activate, CardId.ChainEnergy);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        private bool SwordsOfRevealingLight()
        {
            int count = Bot.SpellZone.GetCardCount(CardId.SwordsOfRevealingLight);
            return count == 0;
        }

        private bool SupremacyBerry()
        {
            return Bot.LifePoints < Enemy.LifePoints;
        }

        private bool PoisonOfTheOldMan()
        {
            AI.SelectOption(1);
            return true;
        }

        private bool ThunderShort()
        {
            return Enemy.GetMonsterCount() >= 3;
        }

        private bool SetInvincibleMonster()
        {
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.IsCode(CardId.Marshmallon, CardId.SpiritReaper))
                {
                    return false;
                }
            }
            return true;
        }

        private bool LavaGolem()
        {
            bool found = false;
            foreach (ClientCard card in Enemy.GetMonsters())
            {
                if (card.Attack > 2000)
                    found = true;
            }
            return found;
        }

        private bool Ceasefire()
        {
            return Bot.GetMonsterCount() + Enemy.GetMonsterCount() >= 3;
        }

        private bool ReposEverything()
        {
            if (Card.IsCode(CardId.ReflectBounder))
                return Card.IsDefense();
            if (Card.IsCode(CardId.FencingFireFerret))
                return DefaultMonsterRepos();
            if (Card.IsAttack())
                return true;
            return false;
        }
    }
}