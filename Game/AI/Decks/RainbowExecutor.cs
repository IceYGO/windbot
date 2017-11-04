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
        public class CardId
        {
            public static int MysteryShellDragon = 18108166;
            public static int PhantomGryphon = 74852097;
            public static int MasterPendulumTheDracoslayer = 75195825;
            public static int AngelTrumpeter = 87979586;
            public static int MetalfoesGoldriver = 33256280;
            public static int Kabazauls = 51934376;
            public static int RescueRabbit = 85138716;

            public static int UnexpectedDai = 911883;
            public static int HarpiesFeatherDuster = 18144506;
            public static int PotOfDesires = 35261759;
            public static int MonsterReborn = 83764718;
            public static int SmashingGround = 97169186;

            public static int QuakingMirrorForce = 40838625;
            public static int DrowningMirrorForce = 47475363;
            public static int BlazingMirrorForce = 75249652;
            public static int StormingMirrorForce = 5650082;
            public static int MirrorForce = 44095762;
            public static int DarkMirrorForce = 20522190;
            public static int BottomlessTrapHole = 29401950;
            public static int TraptrixTrapHoleNightmare = 29616929;
            public static int StarlightRoad = 58120309;

            public static int ScarlightRedDragonArchfiend = 80666118;
            public static int IgnisterProminenceTheBlastingDracoslayer = 18239909;
            public static int StardustDragon = 44508094;
            public static int NumberS39UtopiatheLightning = 56832966;
            public static int Number37HopeWovenDragonSpiderShark = 37279508;
            public static int Number39Utopia = 84013237;
            public static int EvolzarLaggia = 74294676;
            public static int Number59CrookedCook = 82697249;
            public static int CastelTheSkyblasterMusketeer = 82633039;
            public static int StarliegePaladynamo = 61344030;
            public static int LightningChidori = 22653490;
            public static int EvilswarmExcitonKnight = 46772449;
            public static int GagagaCowboy = 12014404;
            public static int EvilswarmNightmare = 359563;
            public static int TraptrixRafflesia = 6511113;
        }

        private bool NormalSummoned = false;

        public RainbowExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.UnexpectedDai, UnexpectedDaiEffect);

            AddExecutor(ExecutorType.Summon, CardId.RescueRabbit);
            AddExecutor(ExecutorType.Activate, CardId.RescueRabbit, RescueRabbitEffect);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, DefaultPotOfDesires);

            AddExecutor(ExecutorType.Summon, CardId.AngelTrumpeter, AngelTrumpeterSummon);
            AddExecutor(ExecutorType.Summon, CardId.Kabazauls, KabazaulsSummon);
            AddExecutor(ExecutorType.Summon, CardId.MasterPendulumTheDracoslayer, MasterPendulumTheDracoslayerSummon);
            AddExecutor(ExecutorType.Summon, CardId.MysteryShellDragon, MysteryShellDragonSummon);
            AddExecutor(ExecutorType.Summon, CardId.PhantomGryphon, PhantomGryphonSummon);
            AddExecutor(ExecutorType.Summon, CardId.MetalfoesGoldriver, MetalfoesGoldriverSummon);

            AddExecutor(ExecutorType.Summon, NormalSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerSummon);
            AddExecutor(ExecutorType.Activate, CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.GagagaCowboy, GagagaCowboySummon);
            AddExecutor(ExecutorType.Activate, CardId.GagagaCowboy);
            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.EvolzarLaggia, EvolzarLaggiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.EvolzarLaggia, EvolzarLaggiaEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmNightmare, EvilswarmNightmareSummon);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmNightmare);
            AddExecutor(ExecutorType.SpSummon, CardId.StarliegePaladynamo, StarliegePaladynamoSummon);
            AddExecutor(ExecutorType.Activate, CardId.StarliegePaladynamo, StarliegePaladynamoEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.LightningChidori, LightningChidoriSummon);
            AddExecutor(ExecutorType.Activate, CardId.LightningChidori, LightningChidoriEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Number37HopeWovenDragonSpiderShark, Number37HopeWovenDragonSpiderSharkSummon);
            AddExecutor(ExecutorType.Activate, CardId.Number37HopeWovenDragonSpiderShark);
            AddExecutor(ExecutorType.SpSummon, CardId.TraptrixRafflesia, TraptrixRafflesiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.TraptrixRafflesia);

            AddExecutor(ExecutorType.Activate, CardId.SmashingGround, DefaultSmashingGround);

            AddExecutor(ExecutorType.SpSummon, CardId.CastelTheSkyblasterMusketeer, DefaultCastelTheSkyblasterMusketeerSummon);
            AddExecutor(ExecutorType.Activate, CardId.CastelTheSkyblasterMusketeer, DefaultCastelTheSkyblasterMusketeerEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerSummon);
            AddExecutor(ExecutorType.Activate, CardId.IgnisterProminenceTheBlastingDracoslayer, IgnisterProminenceTheBlastingDracoslayerEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendSummon);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragonArchfiend, DefaultScarlightRedDragonArchfiendEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, DefaultNumberS39UtopiaTheLightningSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning);

            AddExecutor(ExecutorType.SpSummon, CardId.StardustDragon, DefaultStardustDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.StardustDragon, DefaultStardustDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Number59CrookedCook, Number59CrookedCookSummon);
            AddExecutor(ExecutorType.Activate, CardId.Number59CrookedCook, Number59CrookedCookEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.StarlightRoad, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.QuakingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.BlazingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.StormingMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.MirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.DarkMirrorForce, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.BottomlessTrapHole, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.TraptrixTrapHoleNightmare, TrapSet);

            AddExecutor(ExecutorType.Activate, CardId.StarlightRoad, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.QuakingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.DrowningMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.BlazingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.StormingMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.DarkMirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.TraptrixTrapHoleNightmare, DefaultUniqueTrap);

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
            if (!(defender.Id == CardId.NumberS39UtopiatheLightning))
            {
                if (attacker.Id == CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, CardId.Number39Utopia))
                    attacker.RealPower = 5000;
                if (Bot.HasInMonstersZone(CardId.Number37HopeWovenDragonSpiderShark, true, true))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool UnexpectedDaiEffect()
        {
            if (Bot.HasInHand(CardId.RescueRabbit) || NormalSummoned)
                AI.SelectCard(new[]
                {
                    CardId.MysteryShellDragon,
                    CardId.PhantomGryphon
                });
            else if (AI.Utils.IsTurn1OrMain2())
            {
                if (Bot.HasInHand(CardId.MysteryShellDragon))
                    AI.SelectCard(CardId.MysteryShellDragon);
                else if (Bot.HasInHand(CardId.Kabazauls))
                    AI.SelectCard(CardId.Kabazauls);
                else if (Bot.HasInHand(CardId.AngelTrumpeter))
                    AI.SelectCard(CardId.AngelTrumpeter);
            }
            else
            {
                if (Bot.HasInHand(CardId.Kabazauls))
                    AI.SelectCard(CardId.Kabazauls);
                else if (Bot.HasInHand(CardId.MasterPendulumTheDracoslayer))
                    AI.SelectCard(CardId.MasterPendulumTheDracoslayer);
                else if (Bot.HasInHand(CardId.PhantomGryphon))
                    AI.SelectCard(CardId.PhantomGryphon);
                else if (Bot.HasInHand(CardId.AngelTrumpeter))
                    AI.SelectCard(new[]
                    {
                        CardId.MetalfoesGoldriver,
                        CardId.MasterPendulumTheDracoslayer
                    });
            }
            return true;
        }

        private bool RescueRabbitEffect()
        {
            if (AI.Utils.IsTurn1OrMain2())
                AI.SelectCard(new[]
                    {
                        CardId.Kabazauls,
                        CardId.MysteryShellDragon
                    });
            else
                AI.SelectCard(new[]
                    {
                        CardId.MasterPendulumTheDracoslayer,
                        CardId.PhantomGryphon,
                        CardId.Kabazauls,
                        CardId.MetalfoesGoldriver,
                        CardId.AngelTrumpeter
                    });
            return true;
        }

        private bool MysteryShellDragonSummon()
        {
            return Bot.HasInMonstersZone(CardId.MysteryShellDragon);
        }
        private bool PhantomGryphonSummon()
        {
            return Bot.HasInMonstersZone(CardId.PhantomGryphon);
        }
        private bool MasterPendulumTheDracoslayerSummon()
        {
            return Bot.HasInMonstersZone(CardId.MasterPendulumTheDracoslayer);
        }
        private bool AngelTrumpeterSummon()
        {
            return Bot.HasInMonstersZone(CardId.AngelTrumpeter);
        }
        private bool MetalfoesGoldriverSummon()
        {
            return Bot.HasInMonstersZone(CardId.MetalfoesGoldriver);
        }
        private bool KabazaulsSummon()
        {
            return Bot.HasInMonstersZone(CardId.Kabazauls);
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

        private bool IgnisterProminenceTheBlastingDracoslayerSummon()
        {
            return AI.Utils.GetProblematicEnemyCard() != null;
        }

        private bool IgnisterProminenceTheBlastingDracoslayerEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId(CardId.IgnisterProminenceTheBlastingDracoslayer, 1))
                return true;
            ClientCard target1 = null;
            ClientCard target2 = AI.Utils.GetProblematicEnemyCard();
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

            return AI.Utils.GetProblematicEnemyCard() != null;
        }

        private bool LightningChidoriEffect()
        {
            ClientCard problematicCard = AI.Utils.GetProblematicEnemyCard();
            AI.SelectNextCard(problematicCard);
            return true;
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
            if (AI.Utils.IsTurn1OrMain2() && (Bot.GetRemainingCount(CardId.BottomlessTrapHole, 1) + Bot.GetRemainingCount(CardId.TraptrixTrapHoleNightmare, 1)) > 0)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool Number59CrookedCookSummon()
        {
            return ((Bot.GetMonsterCount() + Bot.GetSpellCount() - 2) <= 1) &&
                ((AI.Utils.IsOneEnemyBetter() && !AI.Utils.IsOneEnemyBetterThanValue(2300, true)) || AI.Utils.IsTurn1OrMain2());
        }

        private bool Number59CrookedCookEffect()
        {
            if (Duel.Player == 0)
            {
                if (AI.Utils.IsChainTarget(Card))
                    return true;
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

        private bool TrapSet()
        {
            return !Bot.HasInMonstersZone(CardId.Number59CrookedCook, true, true);
        }
    }
}
