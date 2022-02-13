using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks {
    [Deck("Swordsoul", "AI_Swordsoul")]
    class SwordsoulExecutor : DefaultExecutor {

        #region Card IDs

        public class CardId {
            // tenyi core
            public const int TenyiAshuna = 87052196;
            public const int TenyiVishuda = 23431858;
            public const int TenyiAdhara = 98159737;

            // swordsoul core
            public const int SwordsoulLongYuan = 93490856;
            public const int SwordsoulTaia = 56495147;
            public const int SwordsoulMoYe = 20001443;

            // handtraps + effect negates
            public const int AshBlossomAndJoyousSpring = 14558127;
            public const int InfiniteImpermanence = 10045474;
            public const int ForbiddenChalice = 25789292;

            //swordsoul support
            public const int SwordsoulEmergence = 56465981;
            public const int SwordsoulSummit = 93850690;
            public const int SwordsoulBlackout = 14821890;

            public const int IncredibleEcclesia = 55273560;
            public const int PotOfDesires = 35261759;

            // tenyi support
            public const int VesselForDragonCycle = 65124425;

            // extra deck
            public const int SwordsoulChengying = 96633955;
            public const int SwordsoulChixiao = 69248256;

            public const int YangZingChaofeng = 19048328;
            public const int YangZingBaxia = 83755611;
            public const int YangZingYazi = 43202238;

            public const int BlackRoseDragon = 73580471;

            public const int TenyiMonk = 32519092;
            public const int TenyiDracoBeserker = 05041348;

            public const int RuddyRoseDragon = 40139997;
            public const int BaronneDeFluer = 84815190;
            public const int AdamancipatorDragnite = 09464441;

            // token
            public const int SwordsoulToken = 20001444;
        }

        private readonly int[] Wyrms =
        {
            CardId.TenyiAshuna,
            CardId.TenyiVishuda,
            CardId.TenyiAdhara,
            CardId.SwordsoulLongYuan,
            CardId.SwordsoulTaia,
            CardId.SwordsoulMoYe,
            CardId.SwordsoulChengying,
            CardId.SwordsoulChixiao,
            CardId.YangZingChaofeng,
            CardId.YangZingBaxia,
            CardId.YangZingYazi,
            CardId.TenyiMonk,
            CardId.SwordsoulToken
        };

        private readonly int[] SwordSouls =
        {
            CardId.SwordsoulLongYuan,
            CardId.SwordsoulTaia,
            CardId.SwordsoulMoYe,
            CardId.SwordsoulChengying,
            CardId.SwordsoulChixiao,
            CardId.SwordsoulEmergence,
            CardId.SwordsoulSummit,
            CardId.SwordsoulBlackout,
            CardId.SwordsoulToken
        };

        private readonly int[] Tenyis =
        {
            CardId.TenyiAshuna,
            CardId.TenyiVishuda,
            CardId.TenyiAdhara,
            CardId.TenyiMonk
        };

        #endregion Card IDs

        #region Activate Effect Flags

        private enum ActivatedEffect {
            None = 0b000,
            First = 0b001,
            Second = 0b010,
            Third = 0b100
        }

        private bool NormalSummonUsed = false;

        private ActivatedEffect MoYeActivated = ActivatedEffect.None;
        private ActivatedEffect LongYuanActivated = ActivatedEffect.None;
        private ActivatedEffect TaiaActivated = ActivatedEffect.None;
        private ActivatedEffect ChixiaoActivated = ActivatedEffect.None;
        private ActivatedEffect ChengyingActivated = ActivatedEffect.None;

        private ActivatedEffect EcclesiaActivated = ActivatedEffect.None;

        private ActivatedEffect AshunaActivated = ActivatedEffect.None;
        private ActivatedEffect VishudaActivated = ActivatedEffect.None;
        private ActivatedEffect AdharaActivated = ActivatedEffect.None;
        private ActivatedEffect BaxiaActivated = ActivatedEffect.None;

        private ActivatedEffect EmergenceActivated = ActivatedEffect.None;
        private ActivatedEffect SummitActivated = ActivatedEffect.None;

        private ActivatedEffect VesselActivated = ActivatedEffect.None;

        private ActivatedEffect BaronneActivated = ActivatedEffect.None;

        private Stack<ClientCard> EffectChain = new Stack<ClientCard>();

        public override void OnNewTurn() {
            NormalSummonUsed = false;

            MoYeActivated = ActivatedEffect.None;
            LongYuanActivated = ActivatedEffect.None;
            TaiaActivated = ActivatedEffect.None;

            ChixiaoActivated = ActivatedEffect.None;
            ChengyingActivated = ActivatedEffect.None;

            EcclesiaActivated = ActivatedEffect.None;

            EmergenceActivated = ActivatedEffect.None;
            SummitActivated = ActivatedEffect.None;

            VesselActivated = ActivatedEffect.None;

            AshunaActivated = ActivatedEffect.None;
            VishudaActivated = ActivatedEffect.None;
            AdharaActivated = ActivatedEffect.None;

            BaxiaActivated = ActivatedEffect.None;

            BaronneActivated &= ActivatedEffect.Second;

            EffectChain.Clear();

            // activation counts do not get reset between sessions so we can only activate the same card 9 times before the game prevents the AI from selecting the card
            ResetActivatedCount();
        }    

        #endregion Activate Effect Flags

        public SwordsoulExecutor(GameAI ai, Duel duel)
            : base(ai, duel) {

            // negates            
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulChixiao, ActivateChixiaoNegate);
            AddExecutor(ExecutorType.Activate, CardId.RuddyRoseDragon, ActivateRuddyRoseNegate);
            AddExecutor(ExecutorType.Activate, CardId.TenyiDracoBeserker, DefaultDisableMonster);
            AddExecutor(ExecutorType.Activate, CardId.AdamancipatorDragnite, ActivateDragniteNegate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenChalice, DefaultDisableMonster);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFluer, ActivateBaronneNegate);

            // triggers
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulChengying, ActivateChengyingEffects);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulChixiao, ActivateChixiaoSearch);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulEmergence, ActivateEmergencLevelDown);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSummit, ActivateSummitLevelDown);
            AddExecutor(ExecutorType.Activate, CardId.VesselForDragonCycle, ActivateVesselSearch);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulTaia, ActivateTaiaMill);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulLongYuan, ActivateLongyuanDamage);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulMoYe, ActivateMoYeDraw);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulBlackout, BlackoutActivateSummon);
            AddExecutor(ExecutorType.Activate, CardId.YangZingYazi, ActivateYaziSearch);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulMoYe, ActivateMoYeSummon);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFluer, ActivateBaronneRevive);
            AddExecutor(ExecutorType.Activate, CardId.YangZingChaofeng, ActivateChaofengSearchEffects);

            // removal activations
            AddExecutor(ExecutorType.Activate, CardId.YangZingYazi, ActivateYaziDestruction);
            AddExecutor(ExecutorType.Activate, CardId.RuddyRoseDragon, ActivateRuddyRoseBanish);
            AddExecutor(ExecutorType.Activate, CardId.BlackRoseDragon, ActivateBlackroseDestroy);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFluer, ActivateBaronneDestroy);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulBlackout, BlackoutActivateDestroy);
            AddExecutor(ExecutorType.Activate, CardId.TenyiVishuda, ActivateVishudaInGrave);

            // other activations
            AddExecutor(ExecutorType.Activate, CardId.TenyiAshuna, SpecialSummonAshuna);
            AddExecutor(ExecutorType.Activate, CardId.TenyiVishuda, SpecialSummonVishuda);
            AddExecutor(ExecutorType.Activate, CardId.TenyiAdhara, SpecialSummonAdhara);
            AddExecutor(ExecutorType.Activate, CardId.TenyiAdhara, ActivateGraveAdhara);
            AddExecutor(ExecutorType.Activate, CardId.TenyiAshuna, ActivateAshunaInGrave);
            AddExecutor(ExecutorType.Activate, CardId.IncredibleEcclesia, ActivateEcclesiaSearch);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulTaia, ActivateTaiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.YangZingBaxia, BaxiaActivatedRevive);
            AddExecutor(ExecutorType.Activate, CardId.YangZingBaxia, BaxiaActivatedShuffle);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulLongYuan, ActivateLongYuanSummon);

            // special summons
            AddExecutor(ExecutorType.SpSummon, CardId.IncredibleEcclesia, SpecialSummonEcclesia);
            AddExecutor(ExecutorType.SpSummon, CardId.TenyiMonk, SpecialSummonMonk);
            AddExecutor(ExecutorType.SpSummon, CardId.YangZingBaxia, SummonBaxia);
            AddExecutor(ExecutorType.SpSummon, CardId.YangZingChaofeng, SummonChaofeng);
            AddExecutor(ExecutorType.SpSummon, CardId.SwordsoulChixiao, SummonChixiao);
            AddExecutor(ExecutorType.SpSummon, CardId.AdamancipatorDragnite, SummonDragnite);
            AddExecutor(ExecutorType.SpSummon, CardId.SwordsoulChengying, SummonChengying);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronneDeFluer, SummonBaronne);
            AddExecutor(ExecutorType.SpSummon, CardId.RuddyRoseDragon, SummonRuddyRose);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackRoseDragon, SummonBlackRose);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackRoseDragon, ShouldSummonBlackRose);
            AddExecutor(ExecutorType.SpSummon, CardId.YangZingYazi, SummonYazi);
            AddExecutor(ExecutorType.SpSummon, CardId.TenyiDracoBeserker, SummonDraco);

            // summons
            AddExecutor(ExecutorType.Summon, CardId.IncredibleEcclesia, SummonEcclesia);
            AddExecutor(ExecutorType.Summon, CardId.SwordsoulTaia, SummonTaia);
            AddExecutor(ExecutorType.Summon, CardId.SwordsoulMoYe, SummonMoYe);
            AddExecutor(ExecutorType.Summon, CardId.TenyiAdhara, SummonAdhara);

            // spell activations
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulEmergence, ActivateEmergenceSearch);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSummit, ActivateSummit);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, ActivatePotOfDesires);

            // set traps
            AddExecutor(ExecutorType.SpellSet, CardId.SwordsoulBlackout, SetBlackout);
            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, CardId.ForbiddenChalice, DefaultSpellSet);

            // util stuff
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand() {
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable) {
            ClientCard currentCard = GetCurrentSearchCardFromChain();
            if(currentCard == null)
                return base.OnSelectCard(cards, min, max, hint, cancelable);

            if(currentCard.IsCode(CardId.SwordsoulChixiao) && ChixiaoActivated.HasFlag(ActivatedEffect.First)) {
                ClientCard selected = ChixiaoSearchSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.SwordsoulTaia) && TaiaActivated.HasFlag(ActivatedEffect.Second)) {
                ClientCard selected = TaiaMillSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.IncredibleEcclesia) && EcclesiaActivated.HasFlag(ActivatedEffect.Second)) {
                ClientCard selected = EcclesiaSearchSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.TenyiAshuna) && AshunaActivated.HasFlag(ActivatedEffect.Second)) {
                ClientCard selected = AshunaSearchSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.VesselForDragonCycle) && VesselActivated.HasFlag(ActivatedEffect.First)) {
                ClientCard selected = VesselMillSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.VesselForDragonCycle) && VesselActivated.HasFlag(ActivatedEffect.Second)) {
                ClientCard selected = VesselSearchSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.SwordsoulChengying)) {
                List<ClientCard> selected = SelectChengyingTargets(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return selected;
            }

            if(currentCard.IsCode(CardId.YangZingYazi)) {
                ClientCard selected = YaziSearchSelection(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.YangZingChaofeng)) {
                ClientCard selected = SelectChaofengTarget(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            if(currentCard.IsCode(CardId.BaronneDeFluer)) {
                ClientCard selected = BaronneDestroyTarget(cards);
                if(selected == null)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                return new List<ClientCard>() { selected };
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }


        private bool ActivatePotOfDesires() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Card.IsDisabled())
                return false;

            // on turn one we want to avoid breaking deck combos if we have plays
            if(Duel.Turn == 1) {
                if(ShouldActivateGraveAshuna())
                    return false;

                if(ShouldSpecialSummonEcclesia())
                    return false;

                if(CanActivateTaiaFromField())
                    return false;

                if(ShouldSummonChixiao())
                    return false;

                if(ShouldSummonYazi())
                    return false;

                if(ShouldSummonBaronne())
                    return false;

                if(ShouldSummonBlackRose())
                    return false;

                if(ShouldSummonDragnite())
                    return false;

                if(CanActivateYaziDestruction())
                    return false;
            }

            bool hasTenyiExtendersInHand = Bot.HasInHand(CardId.TenyiAdhara) || Bot.HasInHand(CardId.TenyiVishuda);
            bool hasAnyTenyiInHand = Bot.HasInHand(CardId.TenyiAshuna) || hasTenyiExtendersInHand;

            if(Bot.HasInHand(CardId.TenyiAshuna) && hasTenyiExtendersInHand)
                return false;

            if(Bot.HasInHand(CardId.VesselForDragonCycle) && hasAnyTenyiInHand)
                return false;

            return DefaultPotOfDesires();
        }

        #region Vessel Code

        private bool ActivateVesselSearch() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Bot.GetMonsters().Count == 0)
                return false;

            if(Bot.MonsterZone.GetMatchingCardsCount(card => !card.HasType(CardType.Effect)) == 0)
                return false;

            if(ShouldSpecialSummonEcclesia())
                return false;

            if(CanActivateTaiaFromField())
                return false;

            if(ShouldSummonChixiao())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            VesselActivated |= ActivatedEffect.First;
            return true;
        }

        private ClientCard VesselMillSelection(IList<ClientCard> cards) {
            VesselActivated |= ActivatedEffect.Second;

            bool shouldSearch = true;

            // Make sure there are materials for ashuna in the deck while we are searching for mill
            if(Bot.Hand.ContainsCardWithId(CardId.TenyiVishuda) && cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiAdhara)) == 1)
                shouldSearch = false;

            if(Bot.Hand.ContainsCardWithId(CardId.TenyiAdhara) && cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiVishuda)) == 1)
                shouldSearch = false;

            AI.SelectYesNo(shouldSearch);

            if(!Bot.HasInGraveyard(CardId.TenyiAshuna) && cards.ContainsCardWithId(CardId.TenyiAshuna))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAshuna));

            if(cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiVishuda)) > 1)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiVishuda));

            if(cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiAdhara)) > 1)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            if(cards.ContainsCardWithId(CardId.SwordsoulLongYuan))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulLongYuan));

            if(cards.ContainsCardWithId(CardId.SwordsoulMoYe))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(cards.ContainsCardWithId(CardId.SwordsoulTaia))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            return null;
        }

        private ClientCard VesselSearchSelection(IList<ClientCard> cards) {
            if(!VishudaActivated.HasFlag(ActivatedEffect.First) && cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiVishuda)) > 1)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiVishuda));

            if(!AdharaActivated.HasFlag(ActivatedEffect.First) && cards.GetMatchingCardsCount(card => card.IsCode(CardId.TenyiAdhara)) > 1)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            if(cards.ContainsCardWithId(CardId.TenyiVishuda))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiVishuda));

            if(cards.ContainsCardWithId(CardId.TenyiAdhara))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            if(cards.ContainsCardWithId(CardId.TenyiAshuna))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAshuna));

            return null;
        }

        #endregion Vessel Code 

        #region Ashuna Code

        private bool SpecialSummonAshuna() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(HasEffectMonster())
                return false;

            if(AshunaActivated.HasFlag(ActivatedEffect.First))
                return false;

            // Do the combo no matter what if we have two tenyi           
            bool usableVishuna = Bot.Hand.ContainsCardWithId(CardId.TenyiVishuda);

            bool usableAdhara = Bot.Hand.ContainsCardWithId(CardId.TenyiAdhara) || Bot.Hand.ContainsCardWithId(CardId.VesselForDragonCycle);
            if(usableVishuna || usableAdhara) {
                AshunaActivated |= ActivatedEffect.First;
                return true;
            }

            // Moye combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulMoYe) && SoulswordMaterialCountInHand() <= 2)
                return false;

            // Longyuan combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulLongYuan) && SoulswordMaterialCountInHand() <= 2)
                return false;

            // Ecclesia combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.IncredibleEcclesia))
                return false;

            AshunaActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ShouldActivateGraveAshuna() {
            if(!Bot.Graveyard.ContainsCardWithId(CardId.TenyiAshuna))
                return false;

            if(!HasNonEffectMonster())
                return false;

            if(Bot.HasInMonstersZone(CardId.TenyiAshuna))
                return false;

            if(AshunaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(CanActivateTaiaFromField())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonChixiao())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonDragnite())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            // if we have a  tenyi summoned then trigger for the other one
            bool shouldTriggerAdhara = Bot.HasInMonstersZone(CardId.TenyiVishuda);
            bool shouldTriggerVishuda = Bot.HasInMonstersZone(CardId.TenyiAdhara);

            if(shouldTriggerAdhara || shouldTriggerVishuda)
                return true;

            if(Bot.HasInMonstersZone(CardId.SwordsoulToken)) {
                bool hasMaterial = Bot.HasInMonstersZone(CardId.SwordsoulTaia) || Bot.HasInMonstersZone(CardId.SwordsoulMoYe);
                if(!hasMaterial)
                    return false;
            }

            return true;
        }

        private bool ActivateAshunaInGrave() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(!ShouldActivateGraveAshuna())
                return false;

            AshunaActivated |= ActivatedEffect.Second;
            return true;
        }

        private ClientCard AshunaSearchSelection(IList<ClientCard> cards) {
            if(Bot.HasInMonstersZone(CardId.TenyiAdhara) && cards.ContainsCardWithId(CardId.TenyiVishuda))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiVishuda));

            if(Bot.HasInMonstersZone(CardId.TenyiVishuda) && cards.ContainsCardWithId(CardId.TenyiAdhara))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            if(cards.Count > 0)
                return cards[Rand.Next(cards.Count)];

            return null;
        }

        #endregion Ashuna Code

        #region Vishuda Code

        private bool SpecialSummonVishuda() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(HasEffectMonster())
                return false;

            if(VishudaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(ActivateDescription == Util.GetStringId(CardId.TenyiVishuda, 1))
                return false;

            bool ashunaReady = !AshunaActivated.HasFlag(ActivatedEffect.Second) && Bot.HasInGraveyard(CardId.TenyiAshuna);
            bool vesselReady = !VesselActivated.HasFlag(ActivatedEffect.First) && Bot.HasInHand(CardId.VesselForDragonCycle);

            // Always activate if able if there is a Ashuna in grave or Vessel in hand
            if(ashunaReady || vesselReady) {
                AI.SelectYesNo(true);
                VishudaActivated |= ActivatedEffect.First;
                return true;
            }

            if(!AshunaActivated.HasFlag(ActivatedEffect.First) && Bot.HasInHand(CardId.TenyiAshuna))
                return false;

            // Moye combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulMoYe) && SoulswordMaterialCountInHand() <= 2)
                return false;

            // Longyuan combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulLongYuan) && SoulswordMaterialCountInHand() <= 2)
                return false;

            // Ecclesia combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.IncredibleEcclesia))
                return false;

            AI.SelectPosition(CardPosition.Defence);
            AI.SelectYesNo(true);
            VishudaActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateVishudaInGrave() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!HasNonEffectMonster())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(CanSummonChenying())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(Enemy.GetMonsters().Count == 0)
                return false;

            ClientCard target = SelectAnEnemyCardForRemoval();
            if(target == null)
                return false;

            AI.SelectCard(target);

            VishudaActivated |= ActivatedEffect.Second;
            return true;
        }

        #endregion Vishuda Code

        #region Adhara Code

        private bool SummonAdhara() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(ShouldActivateGraveAshuna())
                return false;

            if(NormalSummonUsed)
                return false;

            if(!Bot.HasInMonstersZone(CardId.TenyiVishuda))
                return false;

            NormalSummonUsed = true;
            return true;
        }

        private bool SpecialSummonAdhara() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(HasEffectMonster())
                return false;

            if(AdharaActivated.HasFlag(ActivatedEffect.First))
                return false;

            bool ashunaReady = !AshunaActivated.HasFlag(ActivatedEffect.Second) && Bot.HasInGraveyard(CardId.TenyiAshuna);
            bool vesselReady = !VesselActivated.HasFlag(ActivatedEffect.First) && Bot.HasInHand(CardId.VesselForDragonCycle);

            // Always activate if able if there is a Ashuna in grave or Vessel in hand
            if(ashunaReady || vesselReady) {
                AdharaActivated |= ActivatedEffect.First;
                return true;
            }

            if(!AshunaActivated.HasFlag(ActivatedEffect.First) && Bot.HasInHand(CardId.TenyiAshuna))
                return false;

            // Moye combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulMoYe) && SoulswordMaterialCountInHand() == 2)
                return false;

            // Longyuan combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.SwordsoulLongYuan) && SoulswordMaterialCountInHand() == 2)
                return false;

            // Ecclesia combo is better
            if(Bot.Hand.ContainsCardWithId(CardId.IncredibleEcclesia))
                return false;

            AdharaActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateGraveAdhara() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(!HasNonEffectMonster())
                return false;

            if(AdharaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            ClientCard moye = Bot.Banished.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulMoYe));
            if(!NormalSummonUsed && moye != null && MoYeActivated.HasFlag(ActivatedEffect.First)) {
                AdharaActivated |= ActivatedEffect.Second;
                AI.SelectCard(moye);
                return true;
            }

            ClientCard taia = Bot.Banished.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulTaia));
            if(!NormalSummonUsed && taia != null && TaiaActivated.HasFlag(ActivatedEffect.First)) {
                AdharaActivated |= ActivatedEffect.Second;
                AI.SelectCard(taia);
                return true;
            }

            ClientCard longyuan = Bot.Banished.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulLongYuan));
            if(longyuan != null && LongYuanActivated.HasFlag(ActivatedEffect.First)) {
                AdharaActivated |= ActivatedEffect.Second;
                AI.SelectCard(longyuan);
                return true;
            }

            return false;
        }

        #endregion Adhara Code

        #region Monk Code

        private bool SpecialSummonMonk() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSpecialSummonMonk())
                return false;

            return true;
        }

        private bool ShouldSpecialSummonMonk() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.TenyiMonk))
                return false;

            int[] monkMaterial = new int[] {
                CardId.TenyiAshuna,
                CardId.TenyiVishuda,
                CardId.TenyiAdhara
            };

            if(!Bot.MonsterZone.IsExistingMatchingCard(card => monkMaterial.Contains(card.Id)))
                return false;

            if(Bot.HasInMonstersZone(CardId.TenyiMonk))
                return false;

            if(Bot.HasInMonstersZone(CardId.YangZingBaxia))
                return false;

            return true;
        }

        #endregion Monk Code

        #region Draco Beserker Code

        private bool SummonDraco() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSpecialSummonDraco())
                return false;

            return true;
        }

        private bool ShouldSpecialSummonDraco() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.TenyiDracoBeserker))
                return false;

            if(ShouldSpecialSummonMonk())
                return false;

            if(ShouldSummonDragnite())
                return false;

            if(ShouldSummonChixiao())
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_8_Swordsoul) == null)
                return false;

            return true;
        }

        #endregion Monk Code

        #region Ecclesia Code

        private bool SpecialSummonEcclesia() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(!ShouldSpecialSummonEcclesia())
                return false;

            EcclesiaActivated |= ActivatedEffect.First;
            return true;
        }

        private bool SummonEcclesia() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(ShouldActivateGraveAshuna() && !HasEffectMonster())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(NormalSummonUsed)
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            if(Bot.HasInHand(CardId.SwordsoulMoYe) && CanActivateMoYeFromHand())
                return false;

            if(!EcclesiaActivated.HasFlag(ActivatedEffect.First) && Enemy.GetMonsterCount() > Bot.GetMonsterCount())
                return false;

            if(EcclesiaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            NormalSummonUsed = true;
            return true;
        }

        private bool ShouldSpecialSummonEcclesia() {
            if(Enemy.GetMonsterCount() <= Bot.GetMonsterCount())
                return false;

            if(ShouldActivateGraveAshuna())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            if(EcclesiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(EcclesiaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(!Bot.Hand.ContainsCardWithId(CardId.IncredibleEcclesia))
                return false;

            return true;
        }

        private bool ActivateEcclesiaSearch() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Bot.Deck.Count == 0)
                return false;

            if(EcclesiaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            EcclesiaActivated |= ActivatedEffect.Second;
            return true;
        }

        private ClientCard EcclesiaSearchSelection(IList<ClientCard> cards) {
            if(cards.ContainsCardWithId(CardId.SwordsoulTaia) && CanActivateTaiaFromHand())
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            if(cards.ContainsCardWithId(CardId.SwordsoulMoYe) && HasSoulswordMaterialInHand())
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(cards.ContainsCardWithId(CardId.SwordsoulTaia))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            if(cards.ContainsCardWithId(CardId.SwordsoulLongYuan)) {
                AI.SelectPosition(CardPosition.Defence);
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulLongYuan));
            }

            if(cards.Count > 0)
                return cards[Rand.Next(cards.Count)];

            return null;
        }

        #endregion      

        #region Chengying Code

        private bool SummonChengying() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!CanSummonChenying())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_10));
            return true;
        }

        private bool CanSummonChenying() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.SwordsoulChengying))
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_10) == null)
                return false;

            return true;
        }

        private bool ActivateChengyingEffects() {
            if(Card.IsDisabled())
                return false;

            // always activate either of Chegnying effects
            return Card.Location.HasFlag(CardLocation.MonsterZone);
        }

        private List<ClientCard> SelectChengyingTargets(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            if(cards[0].Location.HasFlag(CardLocation.Grave) && cards[0].Owner == 0)
                return SelectChengyingAvoid(cards);

            return SelectChengyingBanish(cards);
        }

        private List<ClientCard> SelectChengyingAvoid(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            // blackout is just good to remove
            if(cards.ContainsCardWithId(CardId.SwordsoulBlackout))
                return new List<ClientCard>() { cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout)) };

            // these don't do much so we can remove
            int[] preferRemoving = new int[] {
                CardId.PotOfDesires,
                CardId.ForbiddenChalice,
                CardId.InfiniteImpermanence,
                CardId.VesselForDragonCycle,
                CardId.SwordsoulEmergence,
                CardId.SwordsoulSummit
            };

            ClientCard preferredRemove = cards.GetFirstMatchingCard(card => preferRemoving.Contains(card.Id));
            if(preferredRemove != null)
                return new List<ClientCard>() { preferredRemove };

            // we don't have great options, just avoid these cards
            int[] avoidRemoving = new int[] {
                CardId.TenyiAdhara,
                CardId.TenyiAshuna,
                CardId.TenyiVishuda,
                CardId.SwordsoulMoYe,
                CardId.SwordsoulTaia,
                CardId.SwordsoulChengying,
                CardId.SwordsoulChixiao
            };

            preferredRemove = cards.GetFirstMatchingCard(card => !avoidRemoving.Contains(card.Id));
            if(preferredRemove != null)
                return new List<ClientCard>() { preferredRemove };

            ChengyingActivated |= ActivatedEffect.First;

            return null;
        }

        private List<ClientCard> SelectChengyingBanish(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            ClientCard card = SelectAnEnemyCardForRemoval();
            if(card == null)
                return null;

            ChengyingActivated |= ActivatedEffect.Second;
            return new List<ClientCard>() { card };
        }

        #endregion Chengying

        #region Chixiao Code

        private bool SummonChixiao() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonChixiao())
                return false;

            List<ClientCard> targets = GetSynchroMaterials(TargetSynchroLevel.Level_8_Swordsoul);
            if(targets != null) {
                AI.SelectMaterials(targets);
                return true;
            }

            targets = GetSynchroMaterials(TargetSynchroLevel.Level_8_Tenki);
            if(targets != null) {
                AI.SelectMaterials(targets);
                return true;
            }

            return false;
        }

        private bool ShouldSummonChixiao() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.SwordsoulChixiao))
                return false;

            if(ChixiaoActivated.HasFlag(ActivatedEffect.First))
                return false;

            bool hasSwordsoulMats = GetSynchroMaterials(TargetSynchroLevel.Level_8_Swordsoul) != null;
            bool hasTenkiMats = GetSynchroMaterials(TargetSynchroLevel.Level_8_Tenki) != null && !Bot.ExtraDeck.ContainsCardWithId(CardId.YangZingBaxia);
            if(!hasSwordsoulMats && !hasTenkiMats)
                return false;

            return true;
        }

        private bool ActivateChixiaoSearch() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!InPostSummonEffect(CardId.SwordsoulChixiao))
                return false;

            if(Bot.Deck.Count == 0)
                return false;

            if(ChixiaoActivated.HasFlag(ActivatedEffect.First))
                return false;

            ChixiaoActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateChixiaoNegate() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(InPostSummonEffect(CardId.SwordsoulChixiao))
                return false;

            if(Enemy.GetMonsters().Count == 0)
                return false;

            if(ChixiaoActivated.HasFlag(ActivatedEffect.Second))
                return false;

            ClientCard cost = null;

            if(Bot.HasInGraveyard(CardId.SwordsoulBlackout))
                cost = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout));

            if(cost == null && Bot.HasInGraveyard(CardId.SwordsoulEmergence))
                cost = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulEmergence));

            if(cost == null && Bot.HasInGraveyard(CardId.SwordsoulTaia))
                cost = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            if(cost == null) {
                IList<ClientCard> possibleCost = Bot.Graveyard.GetMatchingCards(card => Wyrms.Contains(card.Id));
                if(possibleCost.Count > 0)
                    cost = possibleCost[Rand.Next(possibleCost.Count)];
            }

            if(cost == null) {
                IList<ClientCard> possibleCost = Bot.Hand.GetMatchingCards(card => Wyrms.Contains(card.Id));
                if(possibleCost.Count > 0)
                    cost = possibleCost[Rand.Next(possibleCost.Count)];
            }

            if(cost == null)
                return false;

            AI.SelectCard(cost);

            if(!MonsterNegateNext())
                return false;

            ChixiaoActivated |= ActivatedEffect.Second;
            return true;
        }

        private ClientCard ChixiaoSearchSelection(IList<ClientCard> cards) {
            AI.SelectOption(0);

            if(NormalSummonUsed && Bot.HasInMonstersZone(CardId.TenyiMonk) && !Bot.HasInHand(CardId.SwordsoulBlackout))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout));

            if(NormalSummonUsed && Bot.HasInMonstersZone(CardId.SwordsoulChengying) && !Bot.HasInHand(CardId.SwordsoulBlackout))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout));

            // extenders
            bool hasActivateableLongyuan = !LongYuanActivated.HasFlag(ActivatedEffect.First) && Bot.HasInHand(CardId.SwordsoulLongYuan) && HasSoulswordMaterialInHand(CardId.SwordsoulLongYuan);
            bool isDeckLonyaunActivatable = !LongYuanActivated.HasFlag(ActivatedEffect.First) && cards.ContainsCardWithId(CardId.SwordsoulLongYuan) && HasSoulswordMaterialInHand();
            if(isDeckLonyaunActivatable && !hasActivateableLongyuan)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulLongYuan));

            if(!Bot.HasInHand(CardId.SwordsoulSummit) && cards.ContainsCardWithId(CardId.SwordsoulSummit) && GetBestSummitTargetInGrave().ShouldSearch)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulSummit));

            if(!Bot.HasInHand(CardId.SwordsoulMoYe) && cards.ContainsCardWithId(CardId.SwordsoulMoYe) && CanActivateMoYeFromDeck() && !NormalSummonUsed)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(!Bot.HasInHand(CardId.SwordsoulTaia) && cards.ContainsCardWithId(CardId.SwordsoulTaia) && CanActivateTaiaFromHand() && !NormalSummonUsed)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            // any other target
            if(cards.ContainsCardWithId(CardId.SwordsoulBlackout))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout));

            if(cards.ContainsCardWithId(CardId.SwordsoulSummit))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulBlackout));

            if(cards.ContainsCardWithId(CardId.SwordsoulLongYuan))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulLongYuan));

            if(cards.ContainsCardWithId(CardId.SwordsoulMoYe))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(cards.ContainsCardWithId(CardId.SwordsoulTaia))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            if(cards.ContainsCardWithId(CardId.SwordsoulSummit))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulSummit));

            if(cards.Count > 0)
                return cards[Rand.Next(cards.Count)];

            return null;
        }

        #endregion Chixiao

        #region Longyaun Code

        private bool ActivateLongYuanSummon() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Card.IsDisabled())
                return false;

            if(LongYuanActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(Bot.HasInMonstersZone(CardId.SwordsoulLongYuan))
                return false;

            if(!HasSoulswordMaterialInHand(CardId.SwordsoulLongYuan))
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            if(ShouldSpecialSummonEcclesia())
                return false;

            if(CanActivateTaiaFromField())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonChixiao())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            if(ShouldSpecialSummonMonk())
                return false;

            if(CanSummonChaoFeng())
                return false;

            if(ShouldSummonBaxia())
                return false;

            List<int> priorityOfDiscards = new List<int>();
            priorityOfDiscards.Add(CardId.SwordsoulLongYuan);

            if(NormalSummonUsed || MoYeActivated.HasFlag(ActivatedEffect.First))
                priorityOfDiscards.Add(CardId.SwordsoulMoYe);

            if(NormalSummonUsed || TaiaActivated.HasFlag(ActivatedEffect.First))
                priorityOfDiscards.Add(CardId.SwordsoulTaia);

            priorityOfDiscards.AddRange(Tenyis);
            priorityOfDiscards.AddRange(SwordSouls);
            priorityOfDiscards.AddRange(Wyrms);

            AI.SelectPosition(CardPosition.Defence);
            AI.SelectCard(priorityOfDiscards);
            AI.SelectYesNo(true);

            LongYuanActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateLongyuanDamage() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(Card.IsDisabled())
                return false;

            if(LongYuanActivated.HasFlag(ActivatedEffect.Second))
                return false;

            LongYuanActivated |= ActivatedEffect.Second;
            return true;
        }

        #endregion

        #region Taia Code

        private bool ActivateTaiaSummon() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!CanActivateTaiaFromField())
                return false;

            if(Bot.HasInGraveyard(CardId.SwordsoulEmergence)) {
                AI.SelectCard(CardId.SwordsoulEmergence);
                TaiaActivated |= ActivatedEffect.First;
                return true;
            }

            if(Bot.HasInGraveyard(CardId.SwordsoulSummit)) {
                AI.SelectCard(CardId.SwordsoulEmergence);
                TaiaActivated |= ActivatedEffect.First;
                return true;
            }

            if(Bot.HasInGraveyard(CardId.SwordsoulBlackout)) {
                AI.SelectCard(CardId.SwordsoulBlackout);
                TaiaActivated |= ActivatedEffect.First;
                return true;
            }

            if(Bot.HasInGraveyard(CardId.SwordsoulTaia)) {
                AI.SelectCard(CardId.SwordsoulTaia);
                TaiaActivated |= ActivatedEffect.First;
                return true;
            }

            if(!Bot.HasInGraveyard(Wyrms))
                return false;

            int[] goodTenyis = new int[] {
                CardId.TenyiAdhara,
                CardId.TenyiAshuna,
                CardId.TenyiVishuda
            };

            IOrderedEnumerable<ClientCard> possibilities = Bot.Graveyard.GetMatchingCards(card => card.HasRace(CardRace.Wyrm) && !goodTenyis.Contains(card.Id)).OrderBy(card => card.Attack);
            if(possibilities.Count() > 0)
                AI.SelectCard(possibilities.First());

            TaiaActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateTaiaMill() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(Card.IsDisabled())
                return false;

            TaiaActivated |= ActivatedEffect.Second;
            return true;
        }

        private ClientCard TaiaMillSelection(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            if(cards.ContainsCardWithId(CardId.SwordsoulMoYe)) { 
                if(ShouldSummonDragnite() || Bot.HasInMonstersZone(CardId.AdamancipatorDragnite))
                    return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

                if(!Bot.HasInGraveyard(CardId.SwordsoulMoYe))
                    return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));
            }

            bool canSeedVishuda = !Bot.HasInGraveyard(CardId.TenyiVishuda) && cards.ContainsCardWithId(CardId.TenyiVishuda);
            bool canActivateVishuda = VishudaActivated.HasFlag(ActivatedEffect.Second) && Enemy.GetMonsters().Count > 0;
            if(canSeedVishuda && canActivateVishuda)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiVishuda));

            bool canSeedAshuna = !Bot.HasInGraveyard(CardId.TenyiAshuna) && cards.ContainsCardWithId(CardId.TenyiAshuna);
            bool canActivateAshuna = AshunaActivated.HasFlag(ActivatedEffect.Second);
            if(canSeedAshuna && canActivateAshuna)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAshuna));

            bool canSeedAdhara = !Bot.HasInGraveyard(CardId.TenyiAdhara) && cards.ContainsCardWithId(CardId.TenyiAdhara);
            bool canActivateAdhara = AdharaActivated.HasFlag(ActivatedEffect.Second);
            if(canSeedAdhara && canActivateAdhara)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            if(cards.ContainsCardWithId(CardId.SwordsoulMoYe))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            return cards[Rand.Next(cards.Count)];
        }

        private bool CanActivateTaiaFromHand() {
            if(TaiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!Bot.HasInGraveyard(SwordSouls) && !Bot.HasInGraveyard(Wyrms))
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            return true;
        }

        private bool CanActivateTaiaFromGrave() {
            if(TaiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!Bot.Graveyard.IsExistingMatchingCard(card => SwordSouls.Contains(card.Id) || Wyrms.Contains(card.Id), 2))
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            return true;
        }

        private bool SummonTaia() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(ShouldSpecialSummonEcclesia())
                return false;

            if(ShouldActivateGraveAshuna())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(NormalSummonUsed)
                return false;

            if(!CanActivateTaiaFromHand())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonYazi())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            NormalSummonUsed = true;
            return true;
        }

        private bool CanActivateTaiaFromField() {
            if(!Bot.HasInMonstersZone(CardId.SwordsoulTaia))
                return false;

            if(TaiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!Bot.HasInGraveyard(SwordSouls) && !Bot.HasInGraveyard(Wyrms))
                return false;

            if(EmptyMainMonsterZones() == 0)
                return false;

            return true;
        }

        #endregion Taia

        #region Moye Code    

        private bool SummonMoYe() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(ShouldActivateGraveAshuna())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(ShouldSpecialSummonEcclesia())
                return false;

            if(NormalSummonUsed)
                return false;

            if(!CanActivateMoYeFromHand())
                return false;

            if(CanActivateYaziDestruction())
                return false;

            if(ShouldSummonBlackRose())
                return false;

            if(ShouldSummonYazi())
                return false;

            NormalSummonUsed = true;
            return true;
        }

        private bool ActivateMoYeSummon() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!CanActivateMoYeFromHand())
                return false;

            AI.SelectCard(CardLocation.Hand);

            MoYeActivated |= ActivatedEffect.First;
            return true;
        }

        private bool ActivateMoYeDraw() {
            if(!Card.Location.HasFlag(CardLocation.Grave))
                return false;

            if(Card.IsDisabled())
                return false;

            if(MoYeActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(Bot.Deck.Count == 0)
                return false;

            MoYeActivated |= ActivatedEffect.Second;
            return true;
        }

        private bool CanActivateMoYeFromHand() {
            if(MoYeActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!HasSoulswordMaterialInHand(CardId.SwordsoulMoYe))
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            return true;
        }

        private bool CanActivateMoYeFromDeck() {
            if(MoYeActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!HasSoulswordMaterialInHand())
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            return true;
        }

        private bool CanActivateMoYeFromSummit() {
            if(MoYeActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!HasSoulswordMaterialInHand(CardId.SwordsoulSummit))
                return false;

            if(EmptyMainMonsterZones() < 2)
                return false;

            return true;
        }

        #endregion Moye

        #region Emergence Code

        private bool ActivateEmergenceSearch() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Card.IsDisabled())
                return false;

            if(EmergenceActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!Bot.HasInHand(CardId.SwordsoulTaia) && !TaiaActivated.HasFlag(ActivatedEffect.First) && !NormalSummonUsed) {
                AI.SelectCard(CardId.SwordsoulTaia);
                EmergenceActivated |= ActivatedEffect.First;
                return true;
            }

            if(!Bot.HasInHand(CardId.SwordsoulMoYe) && !MoYeActivated.HasFlag(ActivatedEffect.First) && !NormalSummonUsed) {
                AI.SelectCard(CardId.SwordsoulMoYe);
                EmergenceActivated |= ActivatedEffect.First;
                return true;
            }

            if(!Bot.HasInHand(CardId.SwordsoulLongYuan) && !LongYuanActivated.HasFlag(ActivatedEffect.First)) {
                AI.SelectCard(CardId.SwordsoulLongYuan);
                EmergenceActivated |= ActivatedEffect.First;
                return true;
            }

            return false;
        }

        private bool ActivateEmergencLevelDown() {
            if(!Card.Location.HasFlag(CardLocation.Removed))
                return false;

            if(Card.IsDisabled())
                return false;

            if(EmergenceActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(Enemy.GetFieldCount() == 0)
                return false;

            if(!TriggeLevelDownForYazi())
                return false;

            EmergenceActivated |= ActivatedEffect.Second;
            return true;
        }

        #endregion

        #region Blackout Code

        private bool SetBlackout() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Bot.GetMonsters().GetMatchingCardsCount(card => card.HasRace(CardRace.Wyrm)) == 0)
                return false;

            return DefaultSpellSet();
        }

        private bool BlackoutActivateDestroy() {
            if(!Card.Location.HasFlag(CardLocation.SpellZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Enemy.GetFieldCount() < 2)
                return false;

            if(CanSummonChenying())
                return false;

            if(ShouldSummonBaronne())
                return false;

            if(ShouldSummonChixiao())
                return false;

            int[] disallowedTargets = new int[] {
                CardId.SwordsoulChixiao,
                CardId.TenyiDracoBeserker,
                CardId.YangZingChaofeng
            };

            List<ClientCard> targets = new List<ClientCard>();

            IList<ClientCard> myCard = Bot.MonsterZone.GetMatchingCards(card => Wyrms.Contains(card.Id) && !disallowedTargets.Contains(card.Id));
            if(myCard.Count == 0)
                return false;

            if(Bot.HasInMonstersZone(CardId.TenyiAdhara))
                targets.Add(myCard.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara)));
            else if(Bot.HasInMonstersZone(CardId.SwordsoulChengying) && !ChengyingActivated.HasFlag(ActivatedEffect.First))
                targets.Add(myCard.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulChengying)));
            else
                targets.Add(myCard.OrderBy(card => card.Attack).First());

            ClientCard enemy1 = SelectAnEnemyCardForRemoval();
            if(enemy1 == null)
                return false;

            ClientCard enemy2 = SelectAnEnemyCardForRemoval(new List<ClientCard>() { enemy1 });
            if(enemy2 == null)
                return false;

            targets.Add(enemy1);
            targets.Add(enemy2);

            if(targets.Count < 3)
                return false;

            AI.SelectCard(targets);
            return true;
        }

        private bool BlackoutActivateSummon() {
            if(Card.IsDisabled())
                return false;

            return Card.Location.HasFlag(CardLocation.Removed);
        }

        #endregion

        #region Summit Code

        private bool ActivateSummit() {
            if(!Card.Location.HasFlag(CardLocation.Hand))
                return false;

            if(Card.IsDisabled())
                return false;

            if(SummitActivated.HasFlag(ActivatedEffect.First))
                return false;

            SummitTargetResult result = GetBestSummitTargetInGrave();
            if(!result.HasTarget)
                return false;

            if(!result.IsPowerful)
                return false;

            SummitActivated |= ActivatedEffect.First;
            AI.SelectCard(result.Card);
            return true;
        }

        private bool ActivateSummitLevelDown() {
            if(!Card.Location.HasFlag(CardLocation.Removed))
                return false;

            if(Card.IsDisabled())
                return false;

            if(SummitActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(Enemy.GetFieldCount() == 0)
                return false;

            if(!TriggeLevelDownForYazi())
                return false;

            SummitActivated |= ActivatedEffect.Second;
            return true;
        }

        private SummitTargetResult GetBestSummitTargetInGrave() {
            //check powerful cards
            if(Bot.HasInGraveyard(CardId.SwordsoulChengying))
                return SummitTargetResult.PowerfulTarget(CardId.SwordsoulChengying);

            if(Bot.HasInGraveyard(CardId.SwordsoulChixiao))
                return SummitTargetResult.PowerfulTarget(CardId.SwordsoulChixiao);

            if(Bot.HasInGraveyard(CardId.SwordsoulTaia) && CanActivateTaiaFromGrave())
                return SummitTargetResult.PowerfulTarget(CardId.SwordsoulTaia);

            if(Bot.HasInGraveyard(CardId.SwordsoulMoYe) && CanActivateMoYeFromSummit())
                return SummitTargetResult.PowerfulTarget(CardId.SwordsoulMoYe);

            bool hasSynchro = Bot.MonsterZone.IsExistingMatchingCard(card => card.HasType(CardType.Synchro) && card.IsFaceup());
            IList<ClientCard> possibleCards = Bot.Graveyard.GetMatchingCards(card => {
                if(hasSynchro && card.HasRace(CardRace.Wyrm))
                    return true;

                if(SwordSouls.Contains(card.Id) && card.IsMonster())
                    return true;

                return false;
            });

            if(possibleCards.Count == 0)
                return new SummitTargetResult { HasTarget = false };

            // Get highest attack card
            int cardId = possibleCards.OrderByDescending(card => { return card.Attack; }).First().Id;
            return new SummitTargetResult {
                Card = cardId,
                HasTarget = true,
                IsPowerful = false
            };
        }

        private struct SummitTargetResult {
            public bool HasTarget { get; set; }
            public int Card { get; set; }
            public bool IsPowerful { get; set; }

            public bool ShouldSearch => HasTarget && IsPowerful;

            public static SummitTargetResult PowerfulTarget(int cardId) {
                return new SummitTargetResult {
                    Card = cardId,
                    HasTarget = true,
                    IsPowerful = true
                };
            }
        }

        #endregion Summit Code

        #region Baxia Code

        private bool SummonBaxia() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonBaxia())
                return false;

            List<ClientCard> materials = GetSynchroMaterials(TargetSynchroLevel.Level_8_Tenki);
            if(materials == null)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool ShouldSummonBaxia() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.YangZingBaxia))
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_8_Tenki) == null)
                return false;

            return true;
        }

        private bool BaxiaActivatedShuffle() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(BaxiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(!InPostSummonEffect(CardId.YangZingBaxia))
                return false;

            List<ClientCard> targets = new List<ClientCard>();
            ClientCard target1 = SelectAnEnemyCardForRemoval();
            if(target1 != null)
                targets.Add(target1);

            ClientCard target2 = SelectAnEnemyCardForRemoval(new List<ClientCard>() { target1 });
            if(target2 != null)
                targets.Add(target2);

            BaxiaActivated |= ActivatedEffect.First;

            if(targets.Count == 0) {
                AI.SelectCard(new int[0]);
                return false;
            }

            AI.SelectCard(targets);
            AI.SelectYesNo(true);
            return true;
        }

        private bool BaxiaActivatedRevive() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(!BaxiaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(BaxiaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(!Bot.HasInMonstersZone(CardId.TenyiMonk))
                return false;

            if(!Bot.HasInGraveyard(CardId.TenyiAdhara))
                return false;

            ClientCard monk = Bot.MonsterZone.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiMonk));
            ClientCard adhara = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));
            AI.SelectCard(monk);
            AI.SelectNextCard(adhara);

            BaxiaActivated |= ActivatedEffect.Second;
            return true;
        }

        #endregion Baxia Code

        #region Ruddy Rose Code

        private bool SummonRuddyRose() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonRuddyRose())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_10));
            return true;
        }

        private bool ShouldSummonRuddyRose() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.RuddyRoseDragon))
                return false;

            if(Enemy.Graveyard.Count < 10)
                return false;

            if(AshunaActivated.HasFlag(ActivatedEffect.First))
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_10) == null)
                return false;

            return true;
        }

        private bool ActivateRuddyRoseBanish() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!InPostSummonEffect(CardId.RuddyRoseDragon))
                return false;

            return true;
        }

        private bool ActivateRuddyRoseNegate() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Util.GetLastChainCard() == null)
                return false;

            AI.SelectYesNo(true);
            return DefaultTrap();
        }

        #endregion Ruddy Rose Code

        #region Baronne Code

        private bool SummonBaronne() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(ShouldSummonRuddyRose())
                return false;

            if(!ShouldSummonBaronne())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_10));
            return true;
        }

        private bool ShouldSummonBaronne() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.BaronneDeFluer))
                return false;

            if(Enemy.GetFieldCount() == 0 && !Bot.HasInMonstersZone(CardId.SwordsoulChengying))
                return false;

            if(AshunaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_10) == null)
                return false;

            return true;
        }

        private bool ActivateBaronneDestroy() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            bool inMain = Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2;
            if(!inMain)
                return false;

            if(Util.GetLastChainCard() != null)
                return false;

            if(Enemy.GetFieldCount() == 0)
                return false;

            if(BaronneActivated.HasFlag(ActivatedEffect.First))
                return false;

            ClientCard target = SelectAnEnemyCardForRemoval();
            if(target == null)
                return false;

            BaronneActivated |= ActivatedEffect.First;
            AI.SelectCard(target);
            return true;
        }

        private ClientCard BaronneDestroyTarget(IList<ClientCard> cards) {
            List<ClientCard> targetAttempts = new List<ClientCard>();
            for(int i = 0; i < Enemy.GetFieldCount(); i++) { 
                ClientCard target = SelectAnEnemyCardForRemoval(targetAttempts);
                if(target != null && cards.Contains(target))
                    return target;

                targetAttempts.Add(target);
            }

            return null;
        }

        private bool ActivateBaronneNegate() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Util.GetLastChainCard() == null)
                return false;

            if(BaronneActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(DefaultTrap()) {
                BaronneActivated |= ActivatedEffect.Second;
                return true;
            }

            return false;
        }

        private bool ActivateBaronneRevive() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!BaronneActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(Duel.Phase != DuelPhase.Standby)
                return false;

            ClientCard target = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiDracoBeserker));
            if(target != null) {
                AI.SelectCard(target);
                BaronneActivated &= ~ActivatedEffect.Second;
                BaronneActivated |= ActivatedEffect.Third;
                return true;
            }

            target = Bot.Graveyard.GetFirstMatchingCard(card => card.IsCode(CardId.AdamancipatorDragnite));
            if(target != null) {
                AI.SelectCard(target);
                BaronneActivated &= ~ActivatedEffect.Second;
                BaronneActivated |= ActivatedEffect.Third;
                return true;
            }

            return false;
        }

        #endregion Baronne Code

        #region Yazi Code

        private bool SummonYazi() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonYazi())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_7));
            return true;
        }

        private bool ShouldSummonYazi() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.YangZingYazi))
                return false;

            if(ShouldSummonBlackRose())
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_7) == null)
                return false;

            if(Enemy.GetFieldCount() == 0)
                return false;

            return true;
        }

        private bool ActivateYaziDestruction() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(!CanActivateYaziDestruction())
                return false;

            ClientCard target = SelectAnEnemyCardForRemoval();
            if(target == null)
                return false;

            AI.SelectCard(Card);
            AI.SelectNextCard(target);
            return true;
        }

        private bool CanActivateYaziDestruction() {
            if(!Bot.HasInMonstersZone(CardId.YangZingYazi))
                return false;

            if(Enemy.GetFieldCount() == 0)
                return false;

            return true;
        }

        private bool ActivateYaziSearch() {
            if(Card.IsDisabled())
                return false;

            return Card.Location.HasFlag(CardLocation.Grave);
        }

        private ClientCard YaziSearchSelection(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            if(!cards[0].Location.HasFlag(CardLocation.Deck))
                return null;

            if(CanActivateMoYeFromDeck() && cards.ContainsCardWithId(CardId.SwordsoulMoYe))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(CanActivateTaiaFromHand() && cards.ContainsCardWithId(CardId.SwordsoulTaia))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulTaia));

            if(LongYuanActivated.HasFlag(ActivatedEffect.First) && cards.ContainsCardWithId(CardId.SwordsoulLongYuan))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulLongYuan));

            return null;
        }

        private bool TriggeLevelDownForYazi() {
            ClientCard selected = Bot.GetMonsters().GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulTaia));
            if(selected == null)
                selected = Bot.GetMonsters().GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulMoYe));

            if(selected == null)
                return false;

            // blackrose condition, so we don't care about activations
            if(Enemy.GetFieldCount() - Bot.GetFieldCount() >= 2) {
                AI.SelectCard(selected);
                AI.SelectOption(1);
                return true;
            }

            bool hasSynchroMaterial = Bot.HasInMonstersZone(CardId.SwordsoulToken) || (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.SwordsoulTaia);
            if(!hasSynchroMaterial)
                return false;

            if(selected.Id == CardId.SwordsoulTaia && !CanActivateMoYeFromDeck())
                return false;

            if(selected.Id == CardId.SwordsoulMoYe && !CanActivateTaiaFromHand())
                return false;

            AI.SelectCard(selected);
            AI.SelectOption(1);

            return true;
        }

        #endregion Yazi Code

        #region Chaofeng Code

        private bool SummonChaofeng() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!CanSummonChaoFeng())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_9));
            return true;
        }

        private bool ActivateChaofengSearchEffects() {
            if(Card.IsDisabled())
                return false;

            AI.SelectYesNo(true);
            return true;
        }

        private bool CanSummonChaoFeng() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.YangZingChaofeng))
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_9) == null)
                return false;

            return true;
        }

        private ClientCard SelectChaofengTarget(IList<ClientCard> cards) {
            if(cards.Count == 0)
                return null;

            if(Card.Location.HasFlag(CardLocation.Grave))
                return SelectChaofengEffectForHand(cards);

            return SelectChaofengEffectForField(cards);
        }

        private ClientCard SelectChaofengEffectForHand(IList<ClientCard> cards) {
            bool canSummonEcclesia = !NormalSummonUsed || !EcclesiaActivated.HasFlag(ActivatedEffect.First);
            bool canActivateEcclesia = canSummonEcclesia && !EcclesiaActivated.HasFlag(ActivatedEffect.Second);
            bool doesWantEcclesia = Duel.Player == 1 || canActivateEcclesia;
            if(cards.ContainsCardWithId(CardId.IncredibleEcclesia) && doesWantEcclesia)
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.IncredibleEcclesia));

            if(cards.ContainsCardWithId(CardId.AshBlossomAndJoyousSpring))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.AshBlossomAndJoyousSpring));

            if(cards.ContainsCardWithId(CardId.TenyiAdhara))
                return cards.GetFirstMatchingCard(card => card.IsCode(CardId.TenyiAdhara));

            return null;
        }

        private ClientCard SelectChaofengEffectForField(IList<ClientCard> cards) {
            // there is no decision to be made here, we have a single valid card of each attribute
            return cards[0];
        }

        #endregion Chaofeng Code

        #region Dragnite Code

        private bool SummonDragnite() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonDragnite())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_8_Swordsoul));
            return true;
        }

        private bool ShouldSummonDragnite() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.AdamancipatorDragnite))
                return false;

            if(!Bot.HasInMonstersZone(CardId.SwordsoulChixiao))
                return false;

            bool canActivateDragnite = Bot.HasInMonstersZone(CardId.SwordsoulMoYe) || Bot.Graveyard.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Water));
            if(!canActivateDragnite)
                return false;

            if(AshunaActivated.HasFlag(ActivatedEffect.Second))
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_8_Swordsoul) == null)
                return false;

            return true;
        }

        private bool ActivateDragniteNegate() {
            if(!Card.Location.HasFlag(CardLocation.MonsterZone))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Util.GetLastChainCard() == null)
                return false;

            return DefaultTrap();
        }

        #endregion Dragnite Code

        #region Black Rose Code

        private bool SummonBlackRose() {
            if(!Card.Location.HasFlag(CardLocation.Extra))
                return false;

            if(!ShouldSummonBlackRose())
                return false;

            AI.SelectMaterials(GetSynchroMaterials(TargetSynchroLevel.Level_7));
            return true;
        }

        private bool ShouldSummonBlackRose() {
            if(!Bot.ExtraDeck.ContainsCardWithId(CardId.BlackRoseDragon))
                return false;

            if(Enemy.GetFieldCount() - Bot.GetFieldCount() < 2)
                return false;

            if(GetSynchroMaterials(TargetSynchroLevel.Level_7) == null)
                return false;

            return true;
        }

        private bool ActivateBlackroseDestroy() {
            if(!Bot.HasInMonstersZone(CardId.BlackRoseDragon))
                return false;

            if(Card.IsDisabled())
                return false;

            if(Enemy.GetFieldCount() - Bot.GetFieldCount() < 3)
                return false;

            return true;
        }

        #endregion Black Rose Code

        #region Utils

        private bool InPostSummonEffect(int cardId) {
            IList<ClientCard> summons = Util.Duel.LastSummonedCards;
            if(summons.Count == 0)
                return false;

            ClientCard card = summons.Last();
            return card.IsCode(cardId) && card.Owner == 0;
        }

        private bool HasSoulswordMaterialInHand(int activedSoulsword) {
            // If there is more than one then we can activate just fine
            if(Bot.Hand.IsExistingMatchingCard(card => card.IsCode(activedSoulsword), 2))
                return true;

            if(Bot.Hand.IsExistingMatchingCard(card => !card.IsCode(activedSoulsword) && card.IsCode(Wyrms)))
                return true;

            return Bot.Hand.IsExistingMatchingCard(card => !card.IsCode(activedSoulsword) && card.IsCode(SwordSouls));
        }

        private bool HasSoulswordMaterialInHand() {
            return SoulswordMaterialCountInHand() > 0;
        }

        private int SoulswordMaterialCountInHand() {
            return Bot.Hand.GetMatchingCardsCount(card => SwordSouls.Contains(card.Id) || card.HasRace(CardRace.Wyrm));
        }

        private int EmptyMainMonsterZones() {
            return 5 - Bot.GetMonstersInMainZone().Count;
        }

        private ClientCard SelectAnEnemyCardForRemoval() {
            return SelectAnEnemyCardForRemoval(new List<ClientCard>());
        }

        private ClientCard SelectEnemyMonsterForRemoval(List<ClientCard> exclude) {
            ClientCard bestTarget = Util.GetProblematicEnemyCard(canBeTarget: true);
            if(bestTarget != null && bestTarget.IsMonster() && !exclude.Contains(bestTarget))
                return bestTarget;

            IList<ClientCard> monsters = Enemy.GetMonsters().GetMatchingCards(card => !card.IsFacedown() && !exclude.Contains(card));
            if(monsters.Count > 0)
                return monsters.OrderByDescending(card => card.Attack).First();

            // facedowns can't be filtered normally, so try again for facedowns
            monsters = Enemy.GetMonsters().Where(card => card.IsFacedown() && !exclude.Contains(card)).ToList();
            if(monsters.Count > 0)
                return monsters[Rand.Next(monsters.Count)];

            return null;
        }

        private ClientCard SelectAnEnemyCardForRemoval(List<ClientCard> exclude) {
            ClientCard bestTarget = Util.GetProblematicEnemyCard(canBeTarget: true);
            if(bestTarget != null && !exclude.Contains(bestTarget))
                return bestTarget;

            ClientCard monster = SelectEnemyMonsterForRemoval(exclude);
            if(monster != null)
                return monster;

            IList<ClientCard> spells = Enemy.GetSpells().GetMatchingCards(card => !card.IsFacedown() && !exclude.Contains(card) && !isNonPermamentSpell(card));
            if(spells.Count > 0)
                return spells[Rand.Next(spells.Count)];

            // check the spell facedowns
            spells = Enemy.GetSpells().Where(card => !exclude.Contains(card) && card.IsFacedown()).ToList();
            if(spells.Count > 0)
                return spells[Rand.Next(spells.Count)];

            return null;
        }

        private bool isNonPermamentSpell(ClientCard card) {
            // normal spell
            if(card.Type == (int)CardType.Spell)
                return true;

            // quickplay spell
            if(card.HasType(CardType.Spell | CardType.QuickPlay))
                return true;

            return false;
        }

        private bool MonsterNegateNext() {
            if(Duel.Player == 1) {
                ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                if(target != null) {
                    AI.SelectNextCard(target);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            if(LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone &&
                !LastChainCard.IsDisabled() && !LastChainCard.IsShouldNotBeTarget() && !LastChainCard.IsShouldNotBeSpellTrapTarget()) {
                AI.SelectNextCard(LastChainCard);
                return true;
            }

            if(Bot.BattlingMonster != null && Enemy.BattlingMonster != null) {
                if(!Enemy.BattlingMonster.IsDisabled() && Enemy.BattlingMonster.IsCode(_CardId.EaterOfMillions)) {
                    AI.SelectNextCard(Enemy.BattlingMonster);
                    return true;
                }
            }

            if(Duel.Phase == DuelPhase.BattleStart && Duel.Player == 1 &&
                Enemy.HasInMonstersZone(_CardId.NumberS39UtopiaTheLightning, true)) {
                AI.SelectNextCard(_CardId.NumberS39UtopiaTheLightning);
                return true;
            }

            return false;
        }

        private bool HasNonEffectMonster() {
            return Bot.MonsterZone.IsExistingMatchingCard(card => !card.HasType(CardType.Effect) && card.IsFaceup());
        }

        private bool HasEffectMonster() {
            return Bot.MonsterZone.IsExistingMatchingCard(card => card.HasType(CardType.Effect) && card.IsFaceup());
        }

        private enum TargetSynchroLevel : int {
            Level_10 = 10,
            Level_9 = 9,
            Level_8_Swordsoul = 8,
            Level_8_Tenki = -8,
            Level_7 = 7
        }

        private List<ClientCard> GetSynchroMaterials(TargetSynchroLevel level) {
            List<ClientCard> materials = new List<ClientCard>();

            switch(level) {
                case TargetSynchroLevel.Level_10:
                    if(Bot.HasInMonstersZone(CardId.SwordsoulToken)) {
                        materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulToken)));

                        if(Bot.HasInMonstersZone(CardId.SwordsoulLongYuan))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulLongYuan)));
                    }
                    break;
                case TargetSynchroLevel.Level_9:
                    if(Bot.HasInMonstersZone(CardId.YangZingBaxia)) {
                        materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.YangZingBaxia)));

                        if(Bot.HasInMonstersZone(CardId.TenyiAdhara))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.TenyiAdhara)));
                    }
                    break;
                case TargetSynchroLevel.Level_8_Swordsoul:
                    if(Bot.HasInMonstersZone(CardId.SwordsoulToken)) {
                        materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulToken)));

                        if(Bot.HasInMonstersZone(CardId.SwordsoulMoYe))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulMoYe)));
                        else if(Bot.HasInMonstersZone(CardId.SwordsoulTaia))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulTaia)));
                    }
                    break;
                case TargetSynchroLevel.Level_8_Tenki:
                    if(Bot.HasInMonstersZone(CardId.TenyiAdhara)) {
                        materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.TenyiAdhara)));

                        if(Bot.HasInMonstersZone(CardId.TenyiVishuda))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.TenyiVishuda)));
                        else if(Bot.HasInMonstersZone(CardId.TenyiAshuna))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.TenyiAshuna)));
                    }
                    break;
                case TargetSynchroLevel.Level_7:
                    if(Bot.HasInMonstersZone(CardId.SwordsoulToken)) {
                        materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulToken)));

                        if(Bot.HasInMonstersZone(CardId.SwordsoulMoYe))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulMoYe)));
                        else if(Bot.HasInMonstersZone(CardId.SwordsoulTaia))
                            materials.Add(Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.SwordsoulTaia)));
                    }
                    break;
            }

            if(!VerifySynchroMaterials(materials, level))
                return null;

            return materials;
        }

        private ClientCard GetCurrentSearchCardFromChain() {
            int[] searchCards = new int[] {
                CardId.SwordsoulChixiao,
                CardId.SwordsoulTaia,
                CardId.IncredibleEcclesia,
                CardId.TenyiAshuna,
                CardId.VesselForDragonCycle,
                CardId.SwordsoulChengying,
                CardId.YangZingYazi,
                CardId.YangZingChaofeng
            };

            if(EffectChain.Count == 0) {
                foreach(ClientCard card in Duel.CurrentChain) {
                    if(card.Owner == 0 && searchCards.Contains(card.Id))
                        EffectChain.Push(card);
                }
            }

            if(EffectChain.Count > 0)
                return EffectChain.Pop();

            return Util.GetLastChainCard();
        }

        public override void OnChainEnd() {
            EffectChain.Clear();
        }

        private bool VerifySynchroMaterials(List<ClientCard> materials, TargetSynchroLevel level) {
            if(materials.Count != 2)
                return false;

            int combinedLevel = materials[0].Level + materials[1].Level;
            if(combinedLevel != Math.Abs((int)level))
                return false;

            return true;
        }

        private void ResetActivatedCount() {
            typeof(GameAI)
                .GetField("_activatedCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(AI, new Dictionary<int, int>());
        }

        #endregion Utils
    }
}
