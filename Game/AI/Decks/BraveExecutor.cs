using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("Brave", "AI_Brave")]
    class BraveExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int WanderingGryphonRider = 2563463;
            public const int DestinyHeroDasher = 81866673;
            public const int NemesesCorridor = 72090076;
            public const int DestinyHeroCelestial = 63362460;
            public const int AquamancerOfTheSanctuary = 30680659;
            public const int Sangan = 26202165;
            public const int CrusadiaArboria = 91646304;
            public const int AshBlossomJoyousSpring = 14558127;
            public const int MechaPhantomBeastOLion = 72291078;
            public const int MaxxC = 23434538;
            public const int JetSynchron = 9742784;
            public const int EffectVeiler = 97268402;
            public const int RiteofAramesia = 3285551;
            public const int HarpiesFeatherDuster = 18144506;
            public const int FusionDestiny = 52947044;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int CalledByTheGrave = 24224830;
            public const int CrossoutDesignator = 65681983;
            public const int JourneyOfDestiny = 39568067;
            public const int DracobackTheDragonSteed = 38745520;
            public const int InfiniteImpermanence = 10045474;
            public const int SolemnStrike = 40605147;
            public const int ThunderDragonColossus = 15291624;
            public const int DestinyHeroDestroyPhoenixEnforcer = 60461804;
            public const int BaronessDeFleur = 84815190;
            public const int VirtualWorldKyubiShenshen = 92519087;
            public const int BorreloadSavageDragon = 27548199;
            public const int CoralDragon = 42566602;
            public const int TGHyperLibrarian = 90953320;
            public const int TGWonderMagician = 98558751;
            public const int CupidPitch = 21915012;
            public const int MechaPhantomBeastAuroradon = 44097050;
            public const int CrystronHalqifibrax = 50588353;
            public const int PredaplantVerteAnaconda = 70369116;
            public const int LinkSpider = 98978921;
            public const int SalamangreatAlmiraj = 60303245;

            public const int BraveToken = 3285552;
            public const int MechaPhantomBeastToken = 31533705;
            public const int PrimalBeingToken = 27204312;
        }

        public BraveExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.WanderingGryphonRider, WanderingGryphonRiderCounter);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutDesignatorEffect);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.BaronessDeFleur, BaronessDeFleurEffect);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonColossus);

            AddExecutor(ExecutorType.Activate, CardId.DracobackTheDragonSteed, DracobackTheDragonSteedBounce);

            AddExecutor(ExecutorType.Activate, CardId.DestinyHeroDestroyPhoenixEnforcer, DestinyHeroDestroyPhoenixEnforcerEffect);

            AddExecutor(ExecutorType.Activate, CardId.RiteofAramesia, RiteofAramesiaEffect);
            AddExecutor(ExecutorType.Activate, CardId.AquamancerOfTheSanctuary, AquamancerOfTheSanctuarySearchEffect);
            AddExecutor(ExecutorType.Activate, CardId.JourneyOfDestiny, JourneyOfDestinyActivate);
            AddExecutor(ExecutorType.Activate, CardId.WanderingGryphonRider, WanderingGryphonRiderSummon);

            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialFirst);

            AddExecutor(ExecutorType.Summon, CardId.Sangan);

            AddExecutor(ExecutorType.Summon, CardId.MechaPhantomBeastOLion);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastOLion, MechaPhantomBeastOLionEffect);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron);

            AddExecutor(ExecutorType.Activate, CardId.JourneyOfDestiny, JourneyOfDestinyEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj, SalamangreatAlmirajSummonFirst);

            AddExecutor(ExecutorType.SpSummon, CardId.CrusadiaArboria, CrusadiaArboriaSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronHalqifibrax, CrystronNeedlefiberSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrystronHalqifibrax, CrystronNeedlefiberEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonSummon);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonEffect);

            AddExecutor(ExecutorType.Activate, CardId.AquamancerOfTheSanctuary, AquamancerOfTheSanctuarySummonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.TGHyperLibrarian, TGHyperLibrarianSummon);
            AddExecutor(ExecutorType.Activate, CardId.TGHyperLibrarian);

            AddExecutor(ExecutorType.SpSummon, CardId.CupidPitch, CupidPitchSummon);
            AddExecutor(ExecutorType.Activate, CardId.CupidPitch, CupidPitchEffect);

            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchronEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj, SalamangreatAlmirajSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpiderSummon);

            AddExecutor(ExecutorType.Activate, CardId.DracobackTheDragonSteed, DracobackTheDragonSteedEquip);

            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon);

            AddExecutor(ExecutorType.Activate, CardId.NemesesCorridor);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus);

            AddExecutor(ExecutorType.Summon, CardId.CrusadiaArboria, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossomJoyousSpring, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.AquamancerOfTheSanctuary, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.MaxxC, SummonForMaterial);

            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.BaronessDeFleur, BaronessDeFleurSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.VirtualWorldKyubiShenshen, VirtualWorldKyubiShenshenSummon);
            AddExecutor(ExecutorType.Activate, CardId.VirtualWorldKyubiShenshen, VirtualWorldKyubiShenshenEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CoralDragon);
            AddExecutor(ExecutorType.Activate, CardId.CoralDragon, CoralDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.TGWonderMagician, TGWonderMagicianEffect);

            AddExecutor(ExecutorType.Activate, CardId.FusionDestiny, FusionDestinyEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaSummon);
            AddExecutor(ExecutorType.Activate, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaEffect);

            AddExecutor(ExecutorType.Summon, CardId.NemesesCorridor, SummonForMaterial);
            AddExecutor(ExecutorType.Summon, CardId.DestinyHeroCelestial, SummonForMaterial);

            AddExecutor(ExecutorType.Activate, CardId.DestinyHeroDasher, DestinyHeroDasherEffect);
            AddExecutor(ExecutorType.Activate, CardId.DestinyHeroCelestial, DestinyHeroCelestialEffect);

            AddExecutor(ExecutorType.Repos, MonsterRepos);

            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.CrossoutDesignator, TrapSet);

            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);
            AddExecutor(ExecutorType.SpellSet, SetForCelestial);
        }

        private bool BeastOLionUsed = false;
        private bool JetSynchronUsed = false;
        private bool FusionDestinyUsed = false;
        private bool BaronessDeFleurUsed = false;
        private ClientCard PhoenixTarget = null;
        private int PhoenixSelectingTarget = 0;

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            BeastOLionUsed = false;
            JetSynchronUsed = false;
            FusionDestinyUsed = false;
            PhoenixTarget = null;
            PhoenixSelectingTarget = 0;
            base.OnNewTurn();
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
                if (Util.IsTurn1OrMain2() && cardData.Attack <= 2500)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.SalamangreatAlmiraj)
                {
                    return available & Zones.ExtraMonsterZones;
                }
                if (cardId == CardId.TGHyperLibrarian && Bot.GetMonsterCount() >= 3)
                {
                    return available & Zones.ExtraMonsterZones;
                }
                return available & Zones.MainMonsterZones & ~Bot.GetLinkedZones() & ~(Zones.z2 + Zones.z4); // preserve place for arboria
            }
            return 0;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (hint != HintMsg.Destroy)
                PhoenixSelectingTarget = 0;

            if (hint == HintMsg.AddToHand && max == 1)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(CardId.CrusadiaArboria))
                        return new List<ClientCard>(new[] { card });
                    if (card.IsCode(CardId.NemesesCorridor))
                        return new List<ClientCard>(new[] { card });
                }
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(CardId.MaxxC) && !Bot.HasInHand(CardId.MaxxC))
                        return new List<ClientCard>(new[] { card });
                    if (card.IsCode(CardId.AshBlossomJoyousSpring) && !Bot.HasInHand(CardId.AshBlossomJoyousSpring))
                        return new List<ClientCard>(new[] { card });
                    if (card.IsCode(CardId.EffectVeiler) && !Bot.HasInHand(CardId.EffectVeiler))
                        return new List<ClientCard>(new[] { card });
                }
            }
            if (hint == HintMsg.SpSummon && max == 1)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(CardId.DestinyHeroDestroyPhoenixEnforcer))
                        return new List<ClientCard>(new[] { card });
                }
            }
            if (hint == HintMsg.Destroy && max == 1)
            {
                PhoenixSelectingTarget++;
                if (PhoenixSelectingTarget >= 2 && !cards.Contains(PhoenixTarget))
                {
                    ClientCard target = Util.GetProblematicEnemyCard();
                    if (target == null || !cards.Contains(target))
                        target = Util.GetBestEnemyCard();
                    if (target != null && cards.Contains(target))
                        return new List<ClientCard>(new[] { target });
                }
            }
            if (hint == HintMsg.LinkMaterial && cancelable && min == 0)
            {
                // TODO: not working
                return new List<ClientCard>();
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectOption(IList<int> options)
        {
            if (options.Count == 2 && options[0] == Util.GetStringId(CardId.CupidPitch, 1))
                return 0;
            else return base.OnSelectOption(options);
        }

        private IList<int> GetHandCost()
        {
            List<int> result = new List<int> { };
            if (Bot.HasInMonstersZone(CardId.BraveToken))
                result.Add(CardId.DracobackTheDragonSteed);
            if (Bot.Hand.Count(card => card.IsCode(CardId.FusionDestiny)) >= 2)
                result.Add(CardId.FusionDestiny);
            if (Bot.Hand.Count(card => card.IsCode(CardId.Sangan)) >= 2)
                result.Add(CardId.Sangan);
            if (Bot.Hand.Count(card => card.IsCode(CardId.RiteofAramesia)) >= 2)
                result.Add(CardId.RiteofAramesia);
            if (Bot.Hand.Count(card => card.IsCode(CardId.AquamancerOfTheSanctuary)) >= 2)
                result.Add(CardId.AquamancerOfTheSanctuary);
            if (Bot.HasInGraveyardOrInBanished(CardId.DestinyHeroCelestial) || !Bot.HasInExtra(CardId.DestinyHeroDestroyPhoenixEnforcer))
                result.Add(CardId.DestinyHeroDasher);
            if (Bot.HasInGraveyardOrInBanished(CardId.DestinyHeroDasher) || !Bot.HasInExtra(CardId.DestinyHeroDestroyPhoenixEnforcer))
                result.Add(CardId.DestinyHeroCelestial);
            if (Bot.Hand.Count(card => card.IsCode(CardId.AshBlossomJoyousSpring)) >= 2)
                result.Add(CardId.AshBlossomJoyousSpring);
            if (Bot.Hand.Count(card => card.IsCode(CardId.CrossoutDesignator)) >= 2)
                result.Add(CardId.CrossoutDesignator);
            if (Bot.HasInHand(result))
                return result;
            result.AddRange(new int[]{
                CardId.HarpiesFeatherDuster,
                CardId.MechaPhantomBeastOLion,
                CardId.Sangan,
                CardId.AshBlossomJoyousSpring,
                CardId.MaxxC,
                CardId.EffectVeiler,
                CardId.CrossoutDesignator,
                CardId.CalledByTheGrave,
                CardId.RiteofAramesia,
                CardId.AquamancerOfTheSanctuary,
                CardId.InfiniteImpermanence,
                CardId.SolemnStrike
            });
            return result;
        }

        private bool WanderingGryphonRiderCounter()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            return Duel.LastChainPlayer == 1;
        }

        private bool CrossoutDesignatorEffect()
        {
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard == null || Duel.LastChainPlayer != 1) return false;
            return CrossoutDesignatorCheck(LastChainCard, CardId.AquamancerOfTheSanctuary, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.Sangan, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.AshBlossomJoyousSpring, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.MaxxC, 2)
                || CrossoutDesignatorCheck(LastChainCard, CardId.EffectVeiler, 1)
                || CrossoutDesignatorCheck(LastChainCard, CardId.RiteofAramesia, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.HarpiesFeatherDuster, 1)
                || CrossoutDesignatorCheck(LastChainCard, CardId.FusionDestiny, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.FoolishBurial, 1)
                || CrossoutDesignatorCheck(LastChainCard, CardId.MonsterReborn, 1)
                || CrossoutDesignatorCheck(LastChainCard, CardId.CalledByTheGrave, 2)
                || CrossoutDesignatorCheck(LastChainCard, CardId.CrossoutDesignator, 3)
                || CrossoutDesignatorCheck(LastChainCard, CardId.InfiniteImpermanence, 3)
                ;
        }

        private bool CrossoutDesignatorCheck(ClientCard LastChainCard, int id, int count)
        {
            if (LastChainCard.IsCode(id) && Bot.GetRemainingCount(id, count) > 0)
            {
                AI.SelectAnnounceID(id);
                return true;
            }
            return false;
        }

        private bool DestinyHeroDestroyPhoenixEnforcerEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            else
            {
                ClientCard target = Util.GetProblematicEnemyCard(2500);
                if (target != null && !Util.ChainContainPlayer(0))
                {
                    AI.SelectCard(CardId.DestinyHeroDestroyPhoenixEnforcer);
                    AI.SelectNextCard(target);
                    return true;
                }
                target = Util.GetBestEnemyCard();
                if (target == null)
                    return false;
                if (DefaultOnBecomeTarget() || Bot.UnderAttack || Duel.Phase == DuelPhase.End
                    || (Duel.Player == 0 && Util.IsTurn1OrMain2())
                    || (Duel.Player == 1 && Enemy.GetMonsterCount() >= 2))
                {
                    PhoenixTarget = target;
                    AI.SelectCard(CardId.DestinyHeroDestroyPhoenixEnforcer);
                    AI.SelectNextCard(target);
                    return true;
                }
                return false;
            }
        }

        private bool DracobackTheDragonSteedBounce()
        {
            if (Card.Location != CardLocation.SpellZone)
                return false;
            ClientCard target = Util.GetProblematicEnemyCard();
            AI.SelectCard(target);
            return true;
        }

        private bool DracobackTheDragonSteedEquip()
        {
            if (Card.Location == CardLocation.SpellZone)
                return false;
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Bot.HasInMonstersZone(CardId.BraveToken, faceUp: true))
            {
                AI.SelectCard(CardId.BraveToken);
                return true;
            }
            return false;
        }

        private bool BorreloadSavageDragonSummon()
        {
            int[] materials = new[] {
                CardId.CupidPitch,

                CardId.MechaPhantomBeastToken,
                CardId.AquamancerOfTheSanctuary
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool BorreloadSavageDragonEffect()
        {
            if (ActivateDescription == -1)
            {
                AI.SelectCard(new[] { CardId.MechaPhantomBeastAuroradon, CardId.CrystronHalqifibrax, CardId.PredaplantVerteAnaconda });
                return true;
            }
            else
            {
                return true;
            }
        }

        private bool RiteofAramesiaEffect()
        {
            AI.SelectYesNo(true);
            return true;
        }

        private bool WanderingGryphonRiderSummon()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            return Bot.HasInMonstersZone(CardId.BraveToken) || (Duel.Player == 0 && (Duel.LastChainPlayer == -1 || Bot.HasInSpellZone(CardId.JourneyOfDestiny)));
        }

        private bool JourneyOfDestinyActivate()
        {
            return Card.Location == CardLocation.Hand;
        }

        private bool JourneyOfDestinyEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.JourneyOfDestiny, 1))
            {
                // search equip to hand
                AI.SelectOption(0);
                return true;
            }
            else
            {
                // search rider or aquamancer
                if (Bot.GetRemainingCount(CardId.WanderingGryphonRider, 1) == 0 || Bot.GetHandCount() == 0 || !Bot.HasInMonstersZone(CardId.BraveToken))
                {
                    AI.SelectCard(CardId.AquamancerOfTheSanctuary);
                    if (Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.AquamancerOfTheSanctuary))
                        AI.SelectNextCard(CardId.AquamancerOfTheSanctuary);
                    else
                        AI.SelectNextCard(GetHandCost());
                }
                else
                {
                    AI.SelectCard(CardId.WanderingGryphonRider);
                    AI.SelectNextCard(GetHandCost());
                }
                return true;
            }
        }

        private bool AquamancerOfTheSanctuarySearchEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(CardLocation.Deck);
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.AquamancerOfTheSanctuary, 0))
            {
                // summon
                return false;
            }
            else
            {
                // search
                return !Bot.HasInHand(CardId.RiteofAramesia) && !Bot.HasInMonstersZone(CardId.BraveToken);
            }
        }

        private bool AquamancerOfTheSanctuarySummonEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.AquamancerOfTheSanctuary, 0))
            {
                // summon
                return Bot.GetMonsterCount() <= 3;
            }
            return false;
        }

        private bool MechaPhantomBeastOLionEffect()
        {
            if (Bot.GetMonsterCount() >= 3
                && ((Bot.HasInExtra(CardId.MechaPhantomBeastAuroradon) && Bot.HasInMonstersZone(CardId.CrystronHalqifibrax))
                    || Bot.HasInMonstersZone(CardId.MechaPhantomBeastAuroradon)))
                return false;
            if (ActivateDescription == -1)
            {
                BeastOLionUsed = true;
                return true;
            }
            return !BeastOLionUsed;
        }

        private bool CrusadiaArboriaSummon()
        {
            return !Bot.GetMonsters().Any(card => card.IsFaceup() && card.IsTuner());
        }

        private bool CrystronNeedlefiberSummon()
        {
            if (JetSynchronUsed && !Bot.HasInMonstersZone(CardId.MechaPhantomBeastOLion))
                return false;
            List<int> materials = new List<int>{
                CardId.Sangan,
                _CardId.GamecieltheSeaTurtleKaiju,
                CardId.PredaplantVerteAnaconda,
                CardId.SalamangreatAlmiraj,
                CardId.LinkSpider,
                CardId.MaxxC,
                CardId.MechaPhantomBeastToken,
                CardId.AquamancerOfTheSanctuary,

                CardId.CrusadiaArboria,
                CardId.MechaPhantomBeastOLion,
                CardId.JetSynchron,
                CardId.AshBlossomJoyousSpring,
                CardId.EffectVeiler
            };
            if (!Bot.HasInMonstersZone(CardId.BraveToken) || !Bot.HasInMonstersZone(CardId.WanderingGryphonRider))
            {
                materials.Add(CardId.BraveToken);
                materials.Add(CardId.WanderingGryphonRider);
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool CrystronNeedlefiberEffect()
        {
            if (Duel.Player == 0)
            {
                AI.SelectCard(CardId.JetSynchron, CardId.MechaPhantomBeastOLion, CardId.EffectVeiler);
                return true;
            }
            else
            {
                if (Enemy.GetSpells().Any(card => card.IsFacedown() || card.HasType(CardType.Continuous) || card.HasType(CardType.Field) || card.HasType(CardType.Equip)))
                {
                    AI.SelectCard(CardId.TGWonderMagician);
                }
                else
                {
                    AI.SelectCard(CardId.CoralDragon);
                }
                return true;
            }
        }

        private bool TGWonderMagicianEffect()
        {
            ClientCard target = Util.GetProblematicEnemySpell();
            if (target == null)
                target = Enemy.GetSpells().Find(card => card.IsFacedown() || card.HasType(CardType.Continuous) || card.HasType(CardType.Field) || card.HasType(CardType.Equip));
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool MechaPhantomBeastAuroradonSummon()
        {
            return Bot.GetMonsterCount() <= 4; // has 3 field for tokens
        }

        private bool MechaPhantomBeastAuroradonEffect()
        {
            if (ActivateDescription == -1)
                return true;
            else
            {
                AI.SelectOption(1); // release 2 monsters
                AI.SelectCard(CardId.MechaPhantomBeastAuroradon);
                AI.SelectNextCard(CardId.MechaPhantomBeastToken);
                return true;
            }
        }

        private bool TGHyperLibrarianSummon()
        {
            int[] materials = new[] {
                CardId.MechaPhantomBeastOLion,

                CardId.MechaPhantomBeastToken,
                CardId.AquamancerOfTheSanctuary
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 3)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool JetSynchronEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int[] materials = new[] {
                CardId.MechaPhantomBeastToken
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2 || Bot.GetMonsterCount() <= 3)
            {
                JetSynchronUsed = true;
                AI.SelectCard(GetHandCost());
                return true;
            }
            return false;
        }

        private bool CupidPitchSummon()
        {
            int[] materials = new[] {
                CardId.JetSynchron,
                CardId.EffectVeiler,

                CardId.MechaPhantomBeastToken,
                CardId.AquamancerOfTheSanctuary,

                CardId.Sangan,
                CardId.TGHyperLibrarian,
                CardId.BraveToken
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 3)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool CupidPitchEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectOption(1); // level up
            }
            else
            {
                AI.SelectCard(CardId.NemesesCorridor);
            }
            return true;
        }

        private bool SalamangreatAlmirajSummonFirst()
        {
            int[] materials = new[] {
                CardId.Sangan
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials) && !card.IsSpecialSummoned) == 0)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool SalamangreatAlmirajSummon()
        {
            if (PhoenixNotAvail())
                return false;
            int[] materials = new[] {
                CardId.MechaPhantomBeastOLion,
                CardId.JetSynchron
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials) && !card.IsSpecialSummoned) == 0)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool LinkSpiderSummon()
        {
            if (PhoenixNotAvail())
                return false;
            List<int> materials = new List<int>{
                CardId.MechaPhantomBeastToken
            };
            if (!Bot.HasInMonstersZone(CardId.BraveToken) || !Bot.HasInMonstersZone(CardId.WanderingGryphonRider))
            {
                materials.Add(CardId.BraveToken);
            }
            if (Bot.GetMonsters().Any(card => card.IsCode(CardId.PrimalBeingToken) && card.Attack <= 4000))
            {
                materials.Add(CardId.PrimalBeingToken);
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) == 0)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool NeedMonster()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true) || PhoenixNotAvail())
                return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.Level >= 8) > 0)
                return false;
            if (Bot.GetMonsterCount() == 0 && Bot.Hand.GetMatchingCardsCount(card => card.Level <= 4) == 0)
                return false;
            if (Bot.GetMonsterCount() >= 2)
                return false;

            return true;
        }

        private bool SummonForMaterial()
        {
            if (Bot.HasInMonstersZone(CardId.PredaplantVerteAnaconda, true) || !Bot.HasInExtra(CardId.PredaplantVerteAnaconda))
                return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => (card.HasType(CardType.Effect) || card.IsTuner()) && card.Level < 8) == 1)
                return true;
            return false;
        }

        private bool PhoenixNotAvail()
        {
            return Bot.LifePoints <= 2000 || Bot.GetRemainingCount(CardId.FusionDestiny, 3) == 0 || Bot.HasInHand(CardId.FusionDestiny)
                || !Bot.HasInExtra(CardId.PredaplantVerteAnaconda) || !Bot.HasInExtra(CardId.DestinyHeroDestroyPhoenixEnforcer)
                || (Bot.GetRemainingCount(CardId.DestinyHeroCelestial, 1) == 0 && !Bot.HasInHand(CardId.DestinyHeroCelestial))
                || (Bot.GetRemainingCount(CardId.DestinyHeroDasher, 1) == 0 && !Bot.HasInHand(CardId.DestinyHeroDasher));
        }

        private bool PredaplantVerteAnacondaSummon()
        {
            if (PhoenixNotAvail())
                return false;

            List<int> materials = new List<int>{
                _CardId.GamecieltheSeaTurtleKaiju,
                CardId.AquamancerOfTheSanctuary,
                CardId.Sangan,
                CardId.SalamangreatAlmiraj,
                CardId.LinkSpider,
                CardId.NemesesCorridor,
                CardId.DestinyHeroCelestial,
                CardId.DestinyHeroDasher,
                CardId.CrusadiaArboria,
                CardId.AshBlossomJoyousSpring,
                CardId.MechaPhantomBeastOLion,
                CardId.MaxxC,
                CardId.JetSynchron,
                CardId.EffectVeiler,
                CardId.CrystronHalqifibrax,
                CardId.TGHyperLibrarian
            };
            if (!Bot.HasInMonstersZone(CardId.BraveToken) || !Bot.HasInMonstersZone(CardId.WanderingGryphonRider))
            {
                materials.Add(CardId.WanderingGryphonRider);
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool PredaplantVerteAnacondaEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.PredaplantVerteAnaconda, 0))
                return false;
            FusionDestinyUsed = true;
            AI.SelectCard(CardId.FusionDestiny);
            AI.SelectMaterials(CardLocation.Deck);
            return true;
        }

        private bool FusionDestinyEffect()
        {
            FusionDestinyUsed = true;
            return true;
        }

        private bool FoolishBurialFirst()
        {
            if (!Bot.HasInHand(CardId.RiteofAramesia) && !Bot.HasInHandOrInGraveyard(CardId.AquamancerOfTheSanctuary) && !Bot.HasInMonstersZone(CardId.BraveToken))
            {
                AI.SelectCard(CardId.AquamancerOfTheSanctuary);
                return true;
            }
            return false;
        }

        private bool FoolishBurialEffect()
        {
            if (FusionDestinyUsed)
                return false;

            if (!NeedMonster())
                return false;

            AI.SelectCard(new[] {
                CardId.MechaPhantomBeastOLion
            });
            return true;
        }

        private bool MonsterRebornEffect()
        {
            if (Bot.HasInGraveyard(CardId.BaronessDeFleur))
            {
                AI.SelectCard(CardId.BaronessDeFleur);
                return true;
            }
            else if (Bot.HasInGraveyard(CardId.WanderingGryphonRider))
            {
                AI.SelectCard(CardId.WanderingGryphonRider);
                return true;
            }
            else
            {
                if (!NeedMonster())
                    return false;

                AI.SelectCard(new[] {
                    CardId.Sangan,
                    CardId.MechaPhantomBeastOLion,
                    CardId.CrusadiaArboria,
                    CardId.AshBlossomJoyousSpring
                });
                return true;
            }
        }

        private bool DestinyHeroDasherEffect()
        {
            return Bot.Hand.Count(card => card.IsMonster()) > 1;
        }

        private bool DestinyHeroCelestialEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (!Bot.HasInGraveyard(CardId.DestinyHeroDasher))
                return false;
            AI.SelectCard(CardId.DestinyHeroDasher);
            return true;
        }

        private bool BaronessDeFleurSummon()
        {
            int[] materials = new[] {
                CardId.CupidPitch,
                CardId.TGWonderMagician,

                CardId.TGHyperLibrarian
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            if (!Bot.HasInMonstersZone(CardId.BraveToken))
            {
                materials = new[] {
                    CardId.AshBlossomJoyousSpring,
                    CardId.CrusadiaArboria,

                    CardId.WanderingGryphonRider
                };
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            if (!Bot.HasInMonstersZone(CardId.WanderingGryphonRider) || Bot.HasInHand(CardId.RiteofAramesia))
            {
                materials = new[] {
                    CardId.CoralDragon,

                    CardId.BraveToken
                };
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            return false;
        }

        private bool BaronessDeFleurEffect()
        {
            if (Duel.LastChainPlayer == 0)
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                BaronessDeFleurUsed = true;
                return true;
            }
            if (Duel.Phase == DuelPhase.Standby && BaronessDeFleurUsed)
            {
                BaronessDeFleurUsed = false;
                return true;
            }
            if (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
            {
                ClientCard target = Util.GetProblematicEnemyCard(canBeTarget: true);
                if (target == null)
                    target = Util.GetBestEnemyCard(canBeTarget: true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        private bool VirtualWorldKyubiShenshenSummon()
        {
            if (Bot.HasInMonstersZone(CardId.DestinyHeroDestroyPhoenixEnforcer))
                return false;

            int[] materials = new[] {
                CardId.CoralDragon,

                CardId.AquamancerOfTheSanctuary,
                CardId.Sangan
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            if (!Bot.HasInMonstersZone(CardId.BraveToken))
            {
                materials = new[] {
                    CardId.MechaPhantomBeastOLion,

                    CardId.WanderingGryphonRider
                };
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            return false;
        }

        private bool VirtualWorldKyubiShenshenEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.MonsterZone && Bot.HasInBanished(CardId.AquamancerOfTheSanctuary))
            {
                AI.SelectCard(CardId.AquamancerOfTheSanctuary);
                return true;
            }
            else
            {
                int[] costs = new[] {
                    CardId.NemesesCorridor,
                    CardId.Sangan,
                    CardId.CrusadiaArboria,
                    CardId.AshBlossomJoyousSpring,
                    CardId.MechaPhantomBeastOLion,
                    CardId.MaxxC,
                    CardId.EffectVeiler,
                    CardId.ThunderDragonColossus,
                    CardId.BorreloadSavageDragon,
                    CardId.CoralDragon,
                    CardId.TGHyperLibrarian,
                    CardId.TGWonderMagician,
                    CardId.CupidPitch,
                    CardId.CrystronHalqifibrax,
                    CardId.PredaplantVerteAnaconda,
                    CardId.LinkSpider,
                    CardId.SalamangreatAlmiraj
                };
                AI.SelectCard(costs);
                AI.SelectNextCard(costs);
                return true;
            }
        }

        private bool CoralDragonEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            ClientCard target = Util.GetProblematicEnemyCard(canBeTarget: true);
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool TrapSet()
        {
            AI.SelectPlace(Zones.z0 + Zones.z1 + Zones.z3 + Zones.z4);
            return true;
        }

        private bool SetForCelestial()
        {
            return !FusionDestinyUsed && Bot.HasInGraveyard(CardId.DestinyHeroCelestial) && Bot.HasInGraveyard(CardId.DestinyHeroDasher) && TrapSet();
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
    }
}
