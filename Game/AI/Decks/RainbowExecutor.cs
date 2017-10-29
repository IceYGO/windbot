using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Rainbow", "AI_Rainbow")]
    class RainbowExecutor : DefaultExecutor
    {
        public enum CardId
        {
            MysteryShellDragon = 18108166,
            PhantomGryphon = 74852097,
            MasterPendulumTheDracoslayer = 75195825,
            AngelTrumpeter = 87979586,
            MetalfoesGoldriver = 33256280,
            Kabazauls = 51934376,
            RescueRabbit = 85138716,

            UnexpectedDai = 911883,
            HarpiesFeatherDuster = 18144506,
            PotOfDesires = 35261759,
            MonsterReborn = 83764718,
            SmashingGround = 97169186,

            QuakingMirrorForce = 40838625,
            DrowningMirrorForce = 47475363,
            BlazingMirrorForce = 75249652,
            StormingMirrorForce = 5650082,
            MirrorForce = 44095762,
            DarkMirrorForce = 20522190,
            BottomlessTrapHole = 29401950,
            TraptrixTrapHoleNightmare = 29616929,
            StarlightRoad = 58120309,

            ScarlightRedDragonArchfiend = 80666118,
            IgnisterProminenceTheBlastingDracoslayer = 18239909,
            StardustDragon = 44508094,
            NumberS39UtopiatheLightning = 56832966,
            Number37HopeWovenDragonSpiderShark = 37279508,
            Number39Utopia = 84013237,
            EvolzarLaggia = 74294676,
            Number59CrookedCook = 82697249,
            CastelTheSkyblasterMusketeer = 82633039,
            StarliegePaladynamo = 61344030,
            LightningChidori = 22653490,
            EvilswarmExcitonKnight = 46772449,
            GagagaCowboy = 12014404,
            EvilswarmNightmare = 359563,
            TraptrixRafflesia = 6511113
        }

        private bool NormalSummoned = false;

        public RainbowExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, (int)CardId.UnexpectedDai, UnexpectedDaiEffect);

            AddExecutor(ExecutorType.Summon, (int)CardId.RescueRabbit);
            AddExecutor(ExecutorType.Activate, (int)CardId.RescueRabbit, RescueRabbitEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.PotOfDesires, DefaultPotOfDesires);

            AddExecutor(ExecutorType.Summon, (int)CardId.AngelTrumpeter, AngelTrumpeterSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.Kabazauls, KabazaulsSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.MasterPendulumTheDracoslayer, MasterPendulumTheDracoslayerSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.MysteryShellDragon, MysteryShellDragonSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.PhantomGryphon, PhantomGryphonSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.MetalfoesGoldriver, MetalfoesGoldriverSummon);

            AddExecutor(ExecutorType.Summon, NormalSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.GagagaCowboy);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvolzarLaggia, EvolzarLaggiaSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvolzarLaggia, EvolzarLaggiaEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvilswarmNightmare, EvilswarmNightmareSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvilswarmNightmare);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.StarliegePaladynamo, StarliegePaladynamoSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.StarliegePaladynamo, StarliegePaladynamoEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.LightningChidori, LightningChidoriSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.LightningChidori, LightningChidoriEffect);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number37HopeWovenDragonSpiderShark, Number37HopeWovenDragonSpiderSharkSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number37HopeWovenDragonSpiderShark);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.TraptrixRafflesia, TraptrixRafflesiaSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.TraptrixRafflesia);

            AddExecutor(ExecutorType.Activate, (int)CardId.SmashingGround, DefaultSmashingGround);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.CastelTheSkyblasterMusketeer, CastelTheSkyblasterMusketeerSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CastelTheSkyblasterMusketeer, CastelTheSkyblasterMusketeerEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.ScarlightRedDragonArchfiend, ScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.ScarlightRedDragonArchfiend, ScarlightRedDragonArchfiendEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia, NumberS39UtopiatheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.StardustDragon, StardustDragonSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.StardustDragon, StardustDragonEffect);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number59CrookedCook, Number59CrookedCookSummon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number59CrookedCook, Number59CrookedCookEffect);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.StarlightRoad, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.QuakingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DrowningMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.BlazingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.StormingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.MirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DarkMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.BottomlessTrapHole, TrapSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.TraptrixTrapHoleNightmare, TrapSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.QuakingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DrowningMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.BlazingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.StormingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.TraptrixTrapHoleNightmare, DefaultUniqueTrap);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override void OnNewTurn()
        {
            NormalSummoned = false;
        }
        
        public override bool OnSelectHand()
        {
            return Program.Rand.Next(2) > 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == (int)CardId.NumberS39UtopiatheLightning))
            {
                if (attacker.Id == (int)CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.Number39Utopia))
                    attacker.RealPower = 5000;
                if (Bot.HasInMonstersZone((int)CardId.Number37HopeWovenDragonSpiderShark, true, true))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool UnexpectedDaiEffect()
        {
            if (Bot.HasInHand((int)CardId.RescueRabbit) || NormalSummoned)
                AI.SelectCard(new[]
                {
                    (int)CardId.MysteryShellDragon,
                    (int)CardId.PhantomGryphon
                });
            else if (AI.Utils.IsTurn1OrMain2())
            {
                if (Bot.HasInHand((int)CardId.MysteryShellDragon))
                    AI.SelectCard((int)CardId.MysteryShellDragon);
                else if (Bot.HasInHand((int)CardId.Kabazauls))
                    AI.SelectCard((int)CardId.Kabazauls);
                else if (Bot.HasInHand((int)CardId.AngelTrumpeter))
                    AI.SelectCard((int)CardId.AngelTrumpeter);
            }
            else
            {
                if (Bot.HasInHand((int)CardId.Kabazauls))
                    AI.SelectCard((int)CardId.Kabazauls);
                else if (Bot.HasInHand((int)CardId.MasterPendulumTheDracoslayer))
                    AI.SelectCard((int)CardId.MasterPendulumTheDracoslayer);
                else if (Bot.HasInHand((int)CardId.PhantomGryphon))
                    AI.SelectCard((int)CardId.PhantomGryphon);
                else if (Bot.HasInHand((int)CardId.AngelTrumpeter))
                    AI.SelectCard(new[]
                    {
                        (int)CardId.MetalfoesGoldriver,
                        (int)CardId.MasterPendulumTheDracoslayer
                    });
            }
            return true;
        }

        private bool RescueRabbitEffect()
        {
            if (AI.Utils.IsTurn1OrMain2())
                AI.SelectCard(new[]
                    {
                        (int)CardId.Kabazauls,
                        (int)CardId.MysteryShellDragon
                    });
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.MasterPendulumTheDracoslayer,
                        (int)CardId.PhantomGryphon,
                        (int)CardId.Kabazauls,
                        (int)CardId.MetalfoesGoldriver,
                        (int)CardId.AngelTrumpeter
                    });
            return true;
        }

        private bool MysteryShellDragonSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.MysteryShellDragon);
        }
        private bool PhantomGryphonSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.PhantomGryphon);
        }
        private bool MasterPendulumTheDracoslayerSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.MasterPendulumTheDracoslayer);
        }
        private bool AngelTrumpeterSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.AngelTrumpeter);
        }
        private bool MetalfoesGoldriverSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.MetalfoesGoldriver);
        }
        private bool KabazaulsSummon()
        {
            return Bot.HasInMonstersZone((int)CardId.Kabazauls);
        }
        private bool NormalSummon()
        {
            return true;
        }

        private bool GagagaCowboySummon()
        {
            if (Duel.LifePoints[1] <= 800)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool EvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && EvilswarmExcitonKnightEffect();
        }

        private bool EvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            int selfAttack = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                selfAttack += monster.GetDefensePower();
            }

            int oppoAttack = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                oppoAttack += monster.GetDefensePower();
            }

            return (selfCount < oppoCount) || (selfAttack < oppoAttack);
        }

        private bool ScarlightRedDragonArchfiendSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot, true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Enemy, false);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || ScarlightRedDragonArchfiendEffect();
        }

        private bool ScarlightRedDragonArchfiendEffect()
        {
            int selfCount = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!monster.Equals(Card) && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    selfCount++;
            }

            int oppoCount = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                // 没有办法获取特殊召唤的状态，只好默认全部是特招的
                if (monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    oppoCount++;
            }

            return (oppoCount > 0 && selfCount <= oppoCount) || oppoCount > 2;
        }

        private bool CastelTheSkyblasterMusketeerSummon()
        {
            return AI.Utils.GetProblematicCard() != null;
        }

        private bool CastelTheSkyblasterMusketeerEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.CastelTheSkyblasterMusketeer, 0))
                return false;
            AI.SelectNextCard(AI.Utils.GetProblematicCard());
            return true;
        }

        private bool IgnisterProminenceTheBlastingDracoslayerSummon()
        {
            return AI.Utils.GetProblematicCard() != null;
        }

        private bool IgnisterProminenceTheBlastingDracoslayerEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.IgnisterProminenceTheBlastingDracoslayer, 1))
                return true;
            ClientCard target1 = null;
            ClientCard target2 = AI.Utils.GetProblematicCard();
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.HasType(CardType.Pendulum) && !spell.Equals(target2))
                {
                    target1 = spell;
                    break;
                }
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Pendulum) && !monster.Equals(target2))
                {
                    target1 = monster;
                    break;
                }
            }
            if (target2 == null && target1 != null)
            {
                foreach (ClientCard spell in spells)
                {
                    if (!spell.Equals(target1))
                    {
                        target2 = spell;
                        break;
                    }
                }
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.Equals(target1))
                    {
                        target2 = monster;
                        break;
                    }
                }
            }
            if (target2 == null)
                return false;
            AI.SelectCard(target1);
            AI.SelectNextCard(target2);
            return true;
        }

        private bool Number37HopeWovenDragonSpiderSharkSummon()
        {
            return AI.Utils.IsAllEnemyBetterThanValue(1700, false) && !AI.Utils.IsOneEnemyBetterThanValue(3600, true);
        }

        private bool LightningChidoriSummon()
        {
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsFacedown())
                {
                    return true;
                }
            }
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    return true;
                }
            }

            return AI.Utils.GetProblematicCard() != null;
        }

        private bool LightningChidoriEffect()
        {
            ClientCard problematicCard = AI.Utils.GetProblematicCard();
            AI.SelectNextCard(problematicCard);
            return true;
        }

        private bool StardustDragonSummon()
        {
            return (AI.Utils.IsEnemyBetter(false, false) && !AI.Utils.IsOneEnemyBetterThanValue(2400, true)) || AI.Utils.IsTurn1OrMain2();
        }

        private bool StardustDragonEffect()
        {
            return (Card.Location == CardLocation.Grave) || DefaultTrap();
        }

        private bool EvolzarLaggiaSummon()
        {
            return (AI.Utils.IsAllEnemyBetterThanValue(1700, false) && !AI.Utils.IsOneEnemyBetterThanValue(2400, true)) || AI.Utils.IsTurn1OrMain2();
        }

        private bool EvilswarmNightmareSummon()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool TraptrixRafflesiaSummon()
        {
            if (AI.Utils.IsTurn1OrMain2() && (Bot.GetRemainingCount((int)CardId.BottomlessTrapHole, 1) + Bot.GetRemainingCount((int)CardId.TraptrixTrapHoleNightmare, 1)) > 0)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool Number59CrookedCookSummon()
        {
            return ((Bot.GetMonsterCount() + Bot.GetSpellCount() - 2) <= 1) &&
                ((AI.Utils.IsEnemyBetter(false, false) && !AI.Utils.IsOneEnemyBetterThanValue(2300, true)) || AI.Utils.IsTurn1OrMain2());
        }

        private bool Number59CrookedCookEffect()
        {
            if (Duel.Player == 0)
            {
                foreach (ClientCard card in Duel.ChainTargets)
                {
                    if (Card.Equals(card))
                        return true;
                }
            }
            else
            {
                if ((Bot.GetMonsterCount() + Bot.GetSpellCount() -1) <= 1)
                    return true;
            }
            return false;
        }

        private bool EvolzarLaggiaEffect()
        {
            return DefaultTrap();
        }

        private bool StarliegePaladynamoSummon()
        {
            return StarliegePaladynamoEffect();
        }

        private bool StarliegePaladynamoEffect()
        {
            ClientCard result = AI.Utils.GetOneEnemyBetterThanValue(2000, true);
            if (result != null)
            {
                AI.SelectNextCard(result);
                return true;
            }
            return false;
        }

        private bool NumberS39UtopiatheLightningSummon()
        {
            return AI.Utils.IsEnemyBetter(false, false);
        }

        private bool TrapSet()
        {
            return !Bot.HasInMonstersZone((int)CardId.Number59CrookedCook, true, true);
        }
    }
}
