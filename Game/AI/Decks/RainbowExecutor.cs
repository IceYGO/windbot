using System;
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
            public const int MysteryShellDragon = 18108166;
            public const int PhantomGryphon = 74852097;
            public const int MasterPendulumTheDracoslayer = 75195825;
            public const int AngelTrumpeter = 87979586;
            public const int MetalfoesGoldriver = 33256280;
            public const int MegalosmasherX = 81823360;
            public const int RescueRabbit = 85138716;

            public const int UnexpectedDai = 911883;
            public const int HarpiesFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
            public const int MonsterReborn = 83764718;
            public const int SmashingGround = 97169186;

            public const int QuakingMirrorForce = 40838625;
            public const int DrowningMirrorForce = 47475363;
            public const int BlazingMirrorForce = 75249652;
            public const int StormingMirrorForce = 5650082;
            public const int MirrorForce = 44095762;
            public const int DarkMirrorForce = 20522190;
            public const int BottomlessTrapHole = 29401950;
            public const int TraptrixTrapHoleNightmare = 29616929;
            public const int StarlightRoad = 58120309;

            public const int ScarlightRedDragonArchfiend = 80666118;
            public const int IgnisterProminenceTheBlastingDracoslayer = 18239909;
            public const int StardustDragon = 44508094;
            public const int NumberS39UtopiatheLightning = 56832966;
            public const int Number37HopeWovenDragonSpiderShark = 37279508;
            public const int Number39Utopia = 84013237;
            public const int EvolzarLaggia = 74294676;
            public const int Number59CrookedCook = 82697249;
            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int StarliegePaladynamo = 61344030;
            public const int LightningChidori = 22653490;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int GagagaCowboy = 12014404;
            public const int EvilswarmNightmare = 359563;
            public const int TraptrixRafflesia = 6511113;
        }

        private bool NormalSummoned = false;

        public RainbowExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.UnexpectedDai, UnexpectedDaiEffect);

            AddExecutor(ExecutorType.Summon, CardId.RescueRabbit, RescueRabbitSummon);
            AddExecutor(ExecutorType.Activate, CardId.RescueRabbit, RescueRabbitEffect);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, DefaultPotOfDesires);

            AddExecutor(ExecutorType.Summon, CardId.AngelTrumpeter, AngelTrumpeterSummon);
            AddExecutor(ExecutorType.Summon, CardId.MegalosmasherX, MegalosmasherXSummon);
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
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiatheLightning, DefaultNumberS39UtopiaTheLightningEffect);

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
            base.OnNewTurn();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (Bot.HasInMonstersZone(CardId.Number37HopeWovenDragonSpiderShark, true, true))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            // select cards with same name (summoned by rescue rabbit)
            Logger.DebugWriteLine("OnSelectXyzMaterial " + cards.Count + " " + min + " " + max);
            IList<ClientCard> result = new List<ClientCard>();
            foreach (ClientCard card1 in cards)
            {
                foreach (ClientCard card2 in cards)
                {
                    if (card1.IsCode(card2.Id) && !card1.Equals(card2))
                    {
                        result.Add(card1);
                        result.Add(card2);
                        break;
                    }
                }
                if (result.Count > 0)
                    break;
            }
            
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private bool UnexpectedDaiEffect()
        {
            if (Bot.HasInHand(CardId.RescueRabbit) || NormalSummoned)
                AI.SelectCard(
                    CardId.MysteryShellDragon,
                    CardId.PhantomGryphon,
                    CardId.MegalosmasherX
                    );
            else if (Util.IsTurn1OrMain2())
            {
                if (Bot.HasInHand(CardId.MysteryShellDragon))
                    AI.SelectCard(CardId.MysteryShellDragon);
                else if (Bot.HasInHand(CardId.MegalosmasherX))
                    AI.SelectCard(CardId.MegalosmasherX);
                else if (Bot.HasInHand(CardId.AngelTrumpeter))
                    AI.SelectCard(CardId.AngelTrumpeter);
            }
            else
            {
                if (Bot.HasInHand(CardId.MegalosmasherX))
                    AI.SelectCard(CardId.MegalosmasherX);
                else if (Bot.HasInHand(CardId.MasterPendulumTheDracoslayer))
                    AI.SelectCard(CardId.MasterPendulumTheDracoslayer);
                else if (Bot.HasInHand(CardId.PhantomGryphon))
                    AI.SelectCard(CardId.PhantomGryphon);
                else if (Bot.HasInHand(CardId.AngelTrumpeter))
                    AI.SelectCard(CardId.MetalfoesGoldriver, CardId.MasterPendulumTheDracoslayer);
            }
            return true;
        }

        private bool RescueRabbitSummon()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Util.GetBotAvailZonesFromExtraDeck() > 0
                || !Enemy.MonsterZone.IsExistingMatchingCard(card => card.GetDefensePower() >= 1900)
                || Enemy.MonsterZone.GetMatchingCardsCount(card => card.GetDefensePower() < 1900) > Bot.MonsterZone.GetMatchingCardsCount(card => card.Attack >= 1900);
        }

        private bool RescueRabbitEffect()
        {
            if (Util.IsTurn1OrMain2())
            {
                AI.SelectCard(
                    CardId.MegalosmasherX,
                    CardId.MysteryShellDragon
                    );
            }
            else
            {
                AI.SelectCard(
                    CardId.MasterPendulumTheDracoslayer,
                    CardId.PhantomGryphon,
                    CardId.MegalosmasherX,
                    CardId.MetalfoesGoldriver,
                    CardId.AngelTrumpeter
                    );
            }
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
        private bool MegalosmasherXSummon()
        {
            return Bot.HasInMonstersZone(CardId.MegalosmasherX);
        }
        private bool NormalSummon()
        {
            return Card.Id != CardId.RescueRabbit;
        }

        private bool GagagaCowboySummon()
        {
            if (Enemy.LifePoints <= 800)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool IgnisterProminenceTheBlastingDracoslayerSummon()
        {
            return Util.GetProblematicEnemyCard() != null;
        }

        private bool IgnisterProminenceTheBlastingDracoslayerEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.IgnisterProminenceTheBlastingDracoslayer, 1))
                return true;
            ClientCard target1 = null;
            ClientCard target2 = Util.GetProblematicEnemyCard();
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
            return Util.IsAllEnemyBetterThanValue(1700, false) && !Util.IsOneEnemyBetterThanValue(3600, true);
        }

        private bool LightningChidoriSummon()
        {
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster.IsFacedown())
                {
                    return true;
                }
            }
            foreach (ClientCard spell in Enemy.GetSpells())
            {
                if (spell.IsFacedown())
                {
                    return true;
                }
            }

            return Util.GetProblematicEnemyCard() != null;
        }

        private bool LightningChidoriEffect()
        {
            ClientCard problematicCard = Util.GetProblematicEnemyCard();
            AI.SelectCard(0);
            AI.SelectNextCard(problematicCard);
            return true;
        }

        private bool EvolzarLaggiaSummon()
        {
            return (Util.IsAllEnemyBetterThanValue(2000, false) && !Util.IsOneEnemyBetterThanValue(2400, true)) || Util.IsTurn1OrMain2();
        }

        private bool EvilswarmNightmareSummon()
        {
            if (Util.IsTurn1OrMain2())
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool TraptrixRafflesiaSummon()
        {
            if (Util.IsTurn1OrMain2() && (Bot.GetRemainingCount(CardId.BottomlessTrapHole, 1) + Bot.GetRemainingCount(CardId.TraptrixTrapHoleNightmare, 1)) > 0)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool Number59CrookedCookSummon()
        {
            return ((Bot.GetMonsterCount() + Bot.GetSpellCount() - 2) <= 1) &&
                ((Util.IsOneEnemyBetter() && !Util.IsOneEnemyBetterThanValue(2300, true)) || Util.IsTurn1OrMain2());
        }

        private bool Number59CrookedCookEffect()
        {
            if (Duel.Player == 0)
            {
                if (Util.IsChainTarget(Card))
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
            ClientCard result = Util.GetOneEnemyBetterThanValue(2000, true);
            if (result != null)
            {
                AI.SelectCard(0);
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
