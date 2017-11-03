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
        public enum CardId
        {
            LavaGolem = 102380,
            ReflectBounder = 2851070,
            FencingFireFerret = 97396380,
            BlastSphere = 26302522,
            Marshmallon = 31305911,
            SpiritReaper = 23205979,
            NaturiaBeans = 44789585,
            ThunderShort = 20264508,
            Ookazi = 19523799,
            GoblinThief = 45311864,
            TremendousFire = 46918794,
            SwordsOfRevealingLight = 72302403,
            SupremacyBerry = 98380593,
            ChainEnergy = 79323590,
            DarkRoomofNightmare = 85562745,
            PoisonOfTheOldMan = 8842266,
            OjamaTrio = 29843091,
            Ceasefire = 36468556,
            MagicCylinder = 62279055,
            MinorGoblinOfficial = 1918087,
            ChainBurst = 48276469,
            SkullInvitation = 98139712
        }

        public BurnExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Set traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Activate Spells
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkRoomofNightmare);
            AddExecutor(ExecutorType.Activate, (int)CardId.Ookazi);
            AddExecutor(ExecutorType.Activate, (int)CardId.GoblinThief);
            AddExecutor(ExecutorType.Activate, (int)CardId.TremendousFire);
            AddExecutor(ExecutorType.Activate, (int)CardId.SwordsOfRevealingLight, SwordsOfRevealingLight);
            AddExecutor(ExecutorType.Activate, (int)CardId.SupremacyBerry, SupremacyBerry);
            AddExecutor(ExecutorType.Activate, (int)CardId.PoisonOfTheOldMan, PoisonOfTheOldMan);
            AddExecutor(ExecutorType.Activate, (int)CardId.ThunderShort, ThunderShort);

            // Hello, my name is Lava Golem
            AddExecutor(ExecutorType.SpSummon, (int)CardId.LavaGolem, LavaGolem);

            // Set an invincible monster
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Marshmallon, SetInvincibleMonster);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.SpiritReaper, SetInvincibleMonster);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.BlastSphere);

            // Set other monsters
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.FencingFireFerret);
            AddExecutor(ExecutorType.Summon, (int)CardId.ReflectBounder);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.NaturiaBeans);

            // We're a coward
            AddExecutor(ExecutorType.Repos, ReposEverything);

            // Chain traps
            AddExecutor(ExecutorType.Activate, (int)CardId.MagicCylinder, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.Ceasefire, Ceasefire);
            AddExecutor(ExecutorType.Activate, (int)CardId.OjamaTrio);
            AddExecutor(ExecutorType.Activate, (int)CardId.MinorGoblinOfficial);
            AddExecutor(ExecutorType.Activate, (int)CardId.ChainBurst);
            AddExecutor(ExecutorType.Activate, (int)CardId.SkullInvitation);
            AddExecutor(ExecutorType.Activate, (int)CardId.ChainEnergy);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        private bool SwordsOfRevealingLight()
        {
            int count = Bot.SpellZone.GetCardCount((int)CardId.SwordsOfRevealingLight);
            return count == 0;
        }

        private bool SupremacyBerry()
        {
            return Duel.LifePoints[0] < Duel.LifePoints[1];
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
                if (card.Id == (int)CardId.Marshmallon || card.Id == (int)CardId.SpiritReaper)
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
            if (Card.Id == (int)CardId.ReflectBounder)
                return Card.IsDefense();
            if (Card.Id == (int)CardId.FencingFireFerret)
                return DefaultMonsterRepos();
            if (Card.IsAttack())
                return true;
            return false;
        }
    }
}