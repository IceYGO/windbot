using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("RadiantTyphoon", "AI_RadiantTyphoon")]
    class RadiantTyphoonExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int TheWorldsGreatestGallantThief = 24203749;
            public const int RadiantTyphoonFonixTheGreatFlame = 85315450;
            public const int ShiinaTwinTempestsOfCelestialThunder = 12197223;
            public const int RadiantTyphoonVaruroonTheVibrantVortex = 53927851;
            public const int RadiantTyphoonKrosea = 16922142;
            public const int RadiantTyphoonSwen = 80538047;
            public const int RadiantTyphoonDachs = 54143349;
            public const int RadiantTyphoonMeghala = 27755794;

            public const int AshBlossomJoyousSpring = 14558127;
            public const int MaxxC = 23434538;
            public const int CalledByTheGrave = 24224830;
            public const int EffectVeiler = 97268402;
            public const int InfiniteImpermanence = 10045474;
            public const int MulcharmyFuwalos = 42141493;
            public const int MulcharmyPurulia = 84192580;
            public const int DrollLockBird = 94145021;

            public const int MysticalSpaceTyphoon = 5318639;
            public const int RadiantTyphoonVision = 20508881;
            public const int ForbiddenDroplet = 24299458;
            public const int RadiantTyphoonAscendance = 25940932;
            public const int TheFallenTheVirtuous = 30271097;
            public const int SuperPolymerization = 48130397;
            public const int RadiantTyphoonChant = 67115133;
            public const int RadiantTyphoonMandate = 53813120;

            public const int FavoriteHEROShiningFlareWingman = 87758525;
            public const int AlbionTheBrandedDragon = 87746184;
            public const int FavoriteHEROFlameWingman = 13243124;
            public const int GaruraWingsOfResonantLife = 11765832;
            public const int MudragonOfTheSwamp = 54757758;
            public const int EcclesiaAndTheDarkDragon = 78397661;
            public const int SuperStarslayerTYPHONSkyCrisis = 93039339;
            public const int PhantomFortressEnterblathnir = 95113856;
            public const int TotemBird = 71068247;
            public const int HraesvelgrTheDesperateDoomEagle = 49105782;
            public const int WynnTheWindCharmerVerdant = 30674956;
            public const int SPLittleKnight = 29301450;
            public const int Greatfly = 90512490;
            public const int RadiantTyphoonVaruroonTheMarineEidolon = 39341885;
        }

        private const int SetcodeRadiantTyphoon = 0x1d1;

        private bool _usedMeghalaDeckSummon;
        private bool _usedMeghalaHandSummon;
        private bool _usedFonixHandSummon;
        private bool _usedVortexHandSummon;
        private bool _usedFonixFieldEffect;
        private bool _usedVortexFieldEffect;
        private bool _usedSwenHandSummon;
        private bool _usedDachsHandSummon;
        private bool _usedDachsSearch;
        private bool _usedSwenSearch;
        private bool _usedKroseaSearch;
        private bool _usedChantMonsterSearch;
        private bool _usedChantMstSearch;
        private bool _usedVisionDraw;
        private bool _usedVisionMstSearch;
        private bool _usedAscendanceRevive;
        private bool _usedAscendanceMstSearch;
        private bool _seaSpiritSummoned;
        private bool _marineQuickPlayTriggerPending;
        private bool _marineTrapPlacementPending;
        private bool _marineTrapPlacementResolvedThisTurn;
        private bool _marineMandatePayoffSecured;
        private bool _fonixMstTriggerPending;
        private bool _vortexMstTriggerPending;
        private bool _enemyDrollResolved;
        private bool _botDrawHandTrapResolved;
        private bool _enemyPuruliaResolved;
        private bool _enemyMaxxCResolved;
        private bool _enemyFuwalosResolved;
        private bool _botSummonedFromHandAfterPurulia;
        private bool _mustStartMain1WithMonsterSummon;
        private bool _mstOfferedInCurrentChainSelection;
        private bool _radiantQuickPlayOfferedInCurrentChainSelection;
        private bool _selectingGallantThiefTributes;
        private bool _skipGallantThiefSummonThisTurn;
        private ClientCard _fallenDodgeTarget;
        private ClientCard _mysticalSpaceTyphoonTarget;
        private ClientCard _mandateNegationTarget;
        private int _dropletCostCount;
        private int _favoriteHEROFusionTargetId;
        private readonly Dictionary<int, int> _radiantResolutionEffectOffsets =
            new Dictionary<int, int>();
        private readonly HashSet<int> _activatedRadiantCardsThisTurn = new HashSet<int>();
        private readonly HashSet<ClientCard> _botFacedownSpellsSetFromGrave =
            new HashSet<ClientCard>();

        public RadiantTyphoonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Ecclesia's graveyard effect is a high-priority Main Phase resource
            // conversion. Its two targets are selected from the server candidates
            // in SelectActivationCard, where the two target requests can be told apart.
            AddExecutor(ExecutorType.Activate, CardId.EcclesiaAndTheDarkDragon, EcclesiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.GaruraWingsOfResonantLife, GaruraActivate);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon, AlbionActivate);

            // An empty field with Gallant Thief in hand and an opponent monster is
            // a committed summon line. Its summon has priority over every voluntary
            // main-phase activation.
            AddExecutor(ExecutorType.Summon, CardId.TheWorldsGreatestGallantThief, GallantThiefSummon);

            // Free or low-cost chain interaction.
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            // Once the two-card breakthrough line is complete, Droplet must win
            // the next response check so later optional triggers do not insert a
            // third card before the planned cost payment.
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, TwoCardDropletBreakthroughActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVaruroonTheVibrantVortex, VibrantVortexActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonMandate, MandateActivate);
            AddExecutor(ExecutorType.Activate, CardId.TotemBird, TotemBirdActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFallenTheVirtuous, TheFallenNegationDodgeActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonFonixTheGreatFlame, FonixActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPLittleKnightActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, MulcharmyFuwalosActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrollLockBird, DrollLockBirdActivate);
            AddExecutor(ExecutorType.Activate, CardId.ShiinaTwinTempestsOfCelestialThunder, ShiinaActivate);

            // Quick-play interaction and board breakers.
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, MysticalSpaceTyphoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, ForbiddenDropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperPolymerization, SuperPolymerizationActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFallenTheVirtuous, TheFallenTheVirtuousActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheWorldsGreatestGallantThief, GallantThiefActivate);

            // Radiant Typhoon triggers. Vortex and Fonix are intentionally near the
            // top so hand special-summon triggers win during the opponent's turn.
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVaruroonTheMarineEidolon, SeaSpiritActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonKrosea, KroseaActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonMeghala, MeghalaActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonSwen, SwenActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonDachs, DachsActivate);

            // Under a face-up Mandate, put a Radiant quick-play into the
            // opponent's chain before considering a new main-phase line.
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonChant, RadiantQuickPlayMandateChainActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVision, RadiantQuickPlayMandateChainActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonAscendance, RadiantQuickPlayMandateChainActivate);

            // When at least two usable non-Meghala Radiant Typhoon monsters are
            // already on the field, prioritize the archetype Extra Deck summon
            // before starting another main-deck line.
            AddExecutor(ExecutorType.SpSummon, CardId.Greatfly, GreatflyPrioritySummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonVaruroonTheMarineEidolon, SeaSpiritSummon);

            // When MST is missing against backrow, use an archetype quick-play before
            // spending the normal summon on a monster search.
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonChant, RadiantQuickPlayMstStarterActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVision, RadiantQuickPlayMstStarterActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonAscendance, RadiantQuickPlayMstStarterActivate);

            // After the Marine line has completed, use the remaining normal summon
            // on a Radiant Typhoon monster that is not yet on the field. Meghala is
            // checked first, so it wins when several normal summons are legal.
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonMeghala, PostExpansionNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonSwen, PostExpansionNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonDachs, PostExpansionNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonKrosea, PostExpansionNormalSummon);

            // Main-deck engine. Meghala takes priority over Swen, while Swen takes
            // priority over Dachs. Summon triggers should resolve before material use.
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonMeghala, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonSwen, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonDachs, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonSwen, SmallRadiantNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonDachs, SmallRadiantNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonMeghala, SmallRadiantNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonKrosea, KroseaNormalSummon);

            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonChant, RadiantQuickPlayActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVision, RadiantQuickPlayActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonAscendance, RadiantQuickPlayActivate);

            // Extra deck. Establish the archetype Link before generic conversions.
            AddExecutor(ExecutorType.SpSummon, CardId.TotemBird, TotemBirdSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PhantomFortressEnterblathnir, EnterblathnirSummon);
            AddExecutor(ExecutorType.Activate, CardId.PhantomFortressEnterblathnir, EnterblathnirActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPLittleKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HraesvelgrTheDesperateDoomEagle, HraesvelgrSummon);
            AddExecutor(ExecutorType.Activate, CardId.HraesvelgrTheDesperateDoomEagle, HraesvelgrActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.WynnTheWindCharmerVerdant, WynnSummon);
            AddExecutor(ExecutorType.Activate, CardId.WynnTheWindCharmerVerdant, WynnActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.Greatfly, GreatflySummon);
            AddExecutor(ExecutorType.Activate, CardId.Greatfly, GreatflyActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperStarslayerTYPHONSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.SuperStarslayerTYPHONSkyCrisis, TyphonActivate);
            AddExecutor(ExecutorType.Activate, CardId.MudragonOfTheSwamp, MudragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.FavoriteHEROFlameWingman, FavoriteHEROFlameWingmanActivate);
            AddExecutor(ExecutorType.Activate, CardId.FavoriteHEROShiningFlareWingman, FavoriteHEROShiningFlareWingmanActivate);

            AddExecutor(ExecutorType.Repos, RadiantMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, RadiantSpellSet);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override bool OnPreActivate(ClientCard card)
        {
            if (card != null && Duel.Player == 0 &&
                Duel.CurrentChain.Count == 0 &&
                ShouldPrioritizeMonsterSummonAgainstExtraDeck() &&
                (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby) &&
                card.IsSpell())
            {
                return false;
            }

            if (ShouldRequireMain1MonsterSummon())
            {
                return false;
            }

            if (Duel.Player == 0 && ShouldPrioritizeGallantThiefSummon() &&
                (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
            {
                return false;
            }
            return base.OnPreActivate(card);
        }

        public override void OnNewPhase()
        {
            _mustStartMain1WithMonsterSummon = Duel.Player == 0 &&
                Duel.Phase == DuelPhase.Main1 &&
                ShouldPrioritizeMonsterSummonAgainstExtraDeck();
            base.OnNewPhase();
        }

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            _mstOfferedInCurrentChainSelection = cards.Any(c => c != null &&
                c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon));
            _radiantQuickPlayOfferedInCurrentChainSelection = cards.Any(c => c != null &&
                c.Controller == 0 && IsRadiantQuickPlay(c));
            base.OnSelectChain(cards);
        }

        public override void OnNewTurn()
        {
            _usedMeghalaDeckSummon = false;
            _usedMeghalaHandSummon = false;
            _usedFonixHandSummon = false;
            _usedVortexHandSummon = false;
            _usedFonixFieldEffect = false;
            _usedVortexFieldEffect = false;
            _usedSwenHandSummon = false;
            _usedDachsHandSummon = false;
            _usedDachsSearch = false;
            _usedSwenSearch = false;
            _usedKroseaSearch = false;
            _usedChantMonsterSearch = false;
            _usedChantMstSearch = false;
            _usedVisionDraw = false;
            _usedVisionMstSearch = false;
            _usedAscendanceRevive = false;
            _usedAscendanceMstSearch = false;
            _seaSpiritSummoned = false;
            _marineQuickPlayTriggerPending = false;
            _marineTrapPlacementPending = false;
            _marineTrapPlacementResolvedThisTurn = false;
            _fonixMstTriggerPending = false;
            _vortexMstTriggerPending = false;
            _enemyDrollResolved = false;
            _botDrawHandTrapResolved = false;
            _enemyPuruliaResolved = false;
            _enemyMaxxCResolved = false;
            _enemyFuwalosResolved = false;
            _botSummonedFromHandAfterPurulia = false;
            _mustStartMain1WithMonsterSummon = false;
            _mstOfferedInCurrentChainSelection = false;
            _radiantQuickPlayOfferedInCurrentChainSelection = false;
            _selectingGallantThiefTributes = false;
            _skipGallantThiefSummonThisTurn = false;
            _fallenDodgeTarget = null;
            _mysticalSpaceTyphoonTarget = null;
            _mandateNegationTarget = null;
            _dropletCostCount = 0;
            _radiantResolutionEffectOffsets.Clear();
            _activatedRadiantCardsThisTurn.Clear();
            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            // Marine's third effect is EVENT_CHAINING. The trigger is offered as a
            // new response after that chain, so CurrentChainInfo is no longer a
            // reliable source when the trigger is queried. Record the actual
            // Quick-Play activation and keep it through OnChainEnd.
            if (card != null && card.HasType(CardType.QuickPlay) &&
                Bot.GetMonsters().Any(c => c != null &&
                    c.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) && c.IsFaceup()))
            {
                _marineQuickPlayTriggerPending = true;
            }
            if (card != null && card.IsCode(CardId.MysticalSpaceTyphoon))
            {
                if (Bot.Graveyard.Any(c => c.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame)))
                {
                    _fonixMstTriggerPending = true;
                }
                if (Bot.Graveyard.Any(c => c.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex)))
                {
                    _vortexMstTriggerPending = true;
                }
            }
            base.OnChaining(player, card);
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null && _botFacedownSpellsSetFromGrave.Contains(card) &&
                (currentControler != 0 || (currentLocation & (int)CardLocation.SpellZone) == 0 ||
                 !card.IsFacedown()))
            {
                _botFacedownSpellsSetFromGrave.Remove(card);
            }
            if (card != null && currentControler == 0 &&
                (previousLocation & (int)CardLocation.Grave) != 0 &&
                (currentLocation & (int)CardLocation.SpellZone) != 0 && card.IsFacedown())
            {
                _botFacedownSpellsSetFromGrave.Add(card);
            }
            if (card != null && card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) &&
                (previousLocation & (int)CardLocation.MonsterZone) != 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) == 0)
            {
                _marineQuickPlayTriggerPending = false;
            }
            if (card != null && card.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame) &&
                (previousLocation & (int)CardLocation.Grave) != 0 &&
                (currentLocation & (int)CardLocation.Grave) == 0)
            {
                _fonixMstTriggerPending = false;
            }
            if (card != null && card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex) &&
                (previousLocation & (int)CardLocation.Grave) != 0 &&
                (currentLocation & (int)CardLocation.Grave) == 0)
            {
                _vortexMstTriggerPending = false;
            }
            if (card != null && card.IsCode(CardId.RadiantTyphoonMandate) &&
                currentControler == 0 &&
                (currentLocation & (int)CardLocation.SpellZone) != 0 && card.IsFaceup())
            {
                _marineMandatePayoffSecured = true;
                if (_marineTrapPlacementPending)
                {
                    _marineTrapPlacementResolvedThisTurn = true;
                }
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null)
            {
                if (currentChain.ActivatePlayer == 0)
                {
                    _activatedRadiantCardsThisTurn.Add(currentChain.ActivateId);
                }

                if (currentChain.ActivatePlayer == 0 &&
                    currentChain.IsActivateCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
                {
                    bool isTrapPlacement = currentChain.ActivateDescription ==
                        Util.GetStringId(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 2) ||
                        _marineTrapPlacementPending;
                    if (isTrapPlacement && !Duel.IsCurrentSolvingChainNegated() &&
                        HasEstablishedMandate())
                    {
                        _marineTrapPlacementResolvedThisTurn = true;
                        _marineMandatePayoffSecured = true;
                    }
                    _marineTrapPlacementPending = false;
                }

                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentChain.IsActivateCode(CardId.DrollLockBird))
                    {
                        _enemyDrollResolved = true;
                    }

                    if (currentChain.ActivatePlayer == 1 &&
                        currentChain.IsActivateCode(CardId.MulcharmyPurulia))
                    {
                        _enemyPuruliaResolved = true;
                    }

                    if (currentChain.ActivatePlayer == 1 &&
                        currentChain.IsActivateCode(CardId.MaxxC))
                    {
                        _enemyMaxxCResolved = true;
                    }

                    if (currentChain.ActivatePlayer == 1 &&
                        currentChain.IsActivateCode(CardId.MulcharmyFuwalos))
                    {
                        _enemyFuwalosResolved = true;
                    }

                    if (currentChain.ActivatePlayer == 0 && currentChain.IsActivateCode(CardId.MaxxC, CardId.MulcharmyFuwalos))
                    {
                        _botDrawHandTrapResolved = true;
                    }
                }
            }
            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            _marineTrapPlacementPending = false;
            _fallenDodgeTarget = null;
            _mysticalSpaceTyphoonTarget = null;
            _mandateNegationTarget = null;
            _mstOfferedInCurrentChainSelection = false;
            _radiantQuickPlayOfferedInCurrentChainSelection = false;
            _radiantResolutionEffectOffsets.Clear();
            _dropletCostCount = 0;
            _favoriteHEROFusionTargetId = 0;
            base.OnChainEnd();
        }

        public override void OnSummoning()
        {
            _selectingGallantThiefTributes = false;
            _mustStartMain1WithMonsterSummon = false;
            if (_enemyPuruliaResolved && Duel.LastSummonPlayer == 0 &&
                Duel.SummoningCards.Any(c => c != null && c.Controller == 0 &&
                    (c.LastLocation & CardLocation.Hand) != 0))
            {
                _botSummonedFromHandAfterPurulia = true;
            }
            base.OnSummoning();
        }

        public override void OnSpSummoning()
        {
            _mustStartMain1WithMonsterSummon = false;
            base.OnSpSummoning();
        }

        public override void OnSpSummoned()
        {
            if (_enemyPuruliaResolved && Duel.LastSummonPlayer == 0 &&
                Duel.LastSummonedCards.Any(c => c != null && c.Controller == 0 &&
                    (c.LastLocation & CardLocation.Hand) != 0))
            {
                _botSummonedFromHandAfterPurulia = true;
            }
            base.OnSpSummoned();
        }

        private bool MaxxCActivate()
        {
            if (_enemyDrollResolved || Duel.CurrentChain.Any(c => c.IsCode(CardId.DrollLockBird)))
            {
                return false;
            }
            return DefaultMaxxC();
        }

        private bool MulcharmyFuwalosActivate()
        {
            if (Duel.Player == 0 || _enemyDrollResolved || Duel.Phase > DuelPhase.Main1)
            {
                return false;
            }
            return !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool DrollLockBirdActivate()
        {
            if (Duel.Player != 1 || DefaultCheckWhetherCardIsNegated(Card))
            {
                return false;
            }
            return !_botDrawHandTrapResolved;
        }

        private bool TotemBirdActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            ClientCard lastChainCard = Util.GetLastChainCard();
            return Duel.LastChainPlayer == 1 && lastChainCard != null &&
                (lastChainCard.IsSpell() || lastChainCard.IsTrap()) && !lastChainCard.IsDisabled();
        }

        private bool VibrantVortexActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 0) || Card.Location == CardLocation.Hand)
            {
                bool shouldActivate = CanSummonFromHandAfterPurulia() &&
                    !ShouldStopRadiantSpecialSummon(CardLocation.Hand);
                if (shouldActivate)
                {
                    _usedVortexHandSummon = true;
                }
                return shouldActivate;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 1) || Card.Location == CardLocation.MonsterZone)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                bool shouldActivate = Duel.LastChainPlayer == 1 && lastChainCard != null &&
                    lastChainCard.IsMonster() && !lastChainCard.IsDisabled();
                if (shouldActivate)
                {
                    _usedVortexFieldEffect = true;
                }
                return shouldActivate;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 2) || Card.Location == CardLocation.Grave)
            {
                bool shouldActivate = IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 2) ||
                    ActivateDescription == -1 && _vortexMstTriggerPending;
                if (shouldActivate)
                {
                    _vortexMstTriggerPending = false;
                }
                return shouldActivate && !ShouldStopRadiantSpecialSummon(CardLocation.Grave);
            }
            return false;
        }

        private bool FonixActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 0) || Card.Location == CardLocation.Hand)
            {
                bool shouldActivate = CanSummonFromHandAfterPurulia() &&
                    !ShouldStopRadiantSpecialSummon(CardLocation.Hand);
                if (shouldActivate)
                {
                    _usedFonixHandSummon = true;
                }
                return shouldActivate;
            }

            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 1) || Card.Location == CardLocation.MonsterZone)
            {
                List<ClientCard> targets = GetOrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                    .Take(2).ToList();
                if (targets.Count == 0)
                {
                    return false;
                }
                _usedFonixFieldEffect = true;
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 2) || Card.Location == CardLocation.Grave)
            {
                bool shouldActivate = IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 2) ||
                    ActivateDescription == -1 && _fonixMstTriggerPending;
                if (shouldActivate)
                {
                    _fonixMstTriggerPending = false;
                }
                return shouldActivate && !ShouldStopRadiantSpecialSummon(CardLocation.Grave);
            }
            return false;
        }

        private bool MandateActivate()
        {
            // A face-down Continuous Spell is being offered as a card
            // activation, not as one of Mandate's face-up effect descriptions.
            // Do not let a non-(-1) description make the set card fall through
            // to the effect-specific branches; the server has already confirmed
            // that flipping this card is legal.
            if (Card != null && Card.IsFacedown())
            {
                bool shouldActivate = !Bot.GetSpells().Any(c => c != Card &&
                    c.IsCode(CardId.RadiantTyphoonMandate) && c.IsFaceup());
                if (shouldActivate)
                {
                    _activatedRadiantCardsThisTurn.Add(Card.Id);
                }
                return shouldActivate;
            }

            if (ActivateDescription == -1)
            {
                bool shouldActivate = !Bot.GetSpells().Any(c => c != Card &&
                    c.IsCode(CardId.RadiantTyphoonMandate) && c.IsFaceup());
                if (shouldActivate)
                {
                    _activatedRadiantCardsThisTurn.Add(Card.Id);
                }
                return shouldActivate;
            }

            if (IsDescription(CardId.RadiantTyphoonMandate, 0))
            {
                List<ClientCard> quickPlays = Bot.Graveyard.Where(c => c.HasType(CardType.QuickPlay)).ToList();
                List<ClientCard> recyclableQuickPlays = GetMandateRecycleCandidates(quickPlays);
                bool shouldRecycle = quickPlays.Count >= 4 && recyclableQuickPlays.Count >= 3 &&
                    recyclableQuickPlays.Any(IsRadiantCard);
                if (shouldRecycle)
                {
                    _activatedRadiantCardsThisTurn.Add(Card.Id);
                }
                return shouldRecycle;
            }

            if (IsDescription(CardId.RadiantTyphoonMandate, 1))
            {
                if (!CanUseMandateToNegateCurrentChain())
                {
                    return false;
                }
                ClientCard target = GetBestMandateNegationTarget();
                if (target == null)
                {
                    return false;
                }
                _mandateNegationTarget = target;
                _activatedRadiantCardsThisTurn.Add(Card.Id);
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonMandate, 2) || Card.Location == CardLocation.Grave)
            {
                _activatedRadiantCardsThisTurn.Add(Card.Id);
                return true;
            }
            return false;
        }

        private bool SPLittleKnightActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.SPLittleKnight, 0))
            {
                ClientCard target = Util.GetProblematicEnemyCard(0, true) ?? Util.GetBestEnemyCard(false, true);
                if (target == null)
                {
                    return false;
                }
                AI.SelectCard(target);
                return true;
            }

            if (IsDescription(CardId.SPLittleKnight, 1))
            {
                ClientCard enemyTarget = Util.GetProblematicEnemyMonster(0, true) ?? Util.GetBestEnemyMonster(true, true);
                ClientCard ownTarget = Bot.GetMonsters().Where(c => c != Card && c.IsFaceup())
                    .OrderBy(GetMaterialPriority).FirstOrDefault();
                if (ownTarget == null && Card.IsFaceup())
                {
                    ownTarget = Card;
                }
                if (enemyTarget == null || ownTarget == null)
                {
                    return false;
                }
                AI.SelectCard(new List<ClientCard> { ownTarget, enemyTarget });
                return Duel.LastChainPlayer == 1;
            }
            return false;
        }

        private bool ShiinaActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            ChainInfo lastChain = Duel.CurrentChainInfo.LastOrDefault();
            if (lastChain == null || lastChain.ActivatePlayer != 1)
            {
                return false;
            }

            bool opponentActivatedMonster = IsShiinaMonsterActivation(lastChain);
            bool opponentActivatedSpellTrap = IsShiinaSpellTrapActivation(lastChain);
            if (opponentActivatedMonster == opponentActivatedSpellTrap)
            {
                // Do not guess when the protocol does not provide a unique
                // active card type. Shiina's script uses re:IsActiveType(),
                // so a static card type alone is not sufficient for Pendulum
                // or other cards that can act from different zones.
                return false;
            }

            if (opponentActivatedMonster)
            {
                bool enemyHasExtraDeckMonster = Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                    c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
                bool botHasUnusedXyzMonster = Bot.GetMonsters().Any(c => c.IsFaceup() &&
                    !c.IsDisabled() && c.HasType(CardType.Xyz) &&
                    !WasRadiantEffectUsedThisTurn(c.Id));
                bool botHasUnusedLinkMonster = Bot.GetMonsters().Any(c => c.IsFaceup() &&
                    !c.IsDisabled() && c.HasType(CardType.Link) &&
                    !WasRadiantEffectUsedThisTurn(c.Id));
                return enemyHasExtraDeckMonster && !botHasUnusedXyzMonster &&
                    !botHasUnusedLinkMonster &&
                    CanSafelyReturnOwnCardsWithShiina(true);
            }

            if (Duel.Player == 1 && !CanActivateShiinaSpellTrapEffectOnOpponentTurn())
            {
                return false;
            }
            return Enemy.GetSpellCount() >= 3 && CanSafelyReturnOwnCardsWithShiina(false);
        }

        private bool IsShiinaMonsterActivation(ChainInfo chain)
        {
            if (chain == null)
            {
                return false;
            }
            if (chain.HasLocation(CardLocation.MonsterZone))
            {
                return true;
            }
            if (chain.HasLocation(CardLocation.SpellZone | CardLocation.FieldZone))
            {
                return false;
            }
            return (chain.ActivateType & (int)CardType.Monster) != 0 &&
                (chain.ActivateType & (int)(CardType.Spell | CardType.Trap)) == 0;
        }

        private bool IsShiinaSpellTrapActivation(ChainInfo chain)
        {
            if (chain == null)
            {
                return false;
            }
            if (chain.HasLocation(CardLocation.MonsterZone))
            {
                return false;
            }
            if (chain.HasLocation(CardLocation.SpellZone | CardLocation.FieldZone))
            {
                return true;
            }
            return (chain.ActivateType & (int)(CardType.Spell | CardType.Trap)) != 0 &&
                (chain.ActivateType & (int)CardType.Monster) == 0;
        }

        private bool CanSafelyReturnOwnCardsWithShiina(bool returnsMonsters)
        {
            if (returnsMonsters)
            {
                return Bot.GetMonsters().Where(c => c.IsFaceup())
                    .All(IsSafeShiinaOwnMonster);
            }

            return Bot.GetSpells().All(IsSafeShiinaOwnSpellTrap);
        }

        private bool IsSafeShiinaOwnMonster(ClientCard card)
        {
            if (card == null)
            {
                return true;
            }
            if (card.IsCode(CardId.ShiinaTwinTempestsOfCelestialThunder) &&
                card.Location == CardLocation.MonsterZone)
            {
                // Shiina's own script explicitly excludes a Shiina in our
                // Monster Zone from the monster return group.
                return true;
            }

            // A negated monster no longer has the interruption value that the
            // Shiina safety check is intended to preserve.
            if (card.IsDisabled())
            {
                return true;
            }

            // Returning an Extra Deck monster sends it back to the Extra Deck,
            // not to the hand. It is dangerous until that monster has already
            // used its relevant effect this turn.
            if (card.IsExtraCard())
            {
                return WasRadiantEffectUsedThisTurn(card.Id);
            }

            // Non-Radiant Main Deck monsters are not part of this deck's Shiina
            // protection policy. They can be returned without blocking the
            // monster-type activation.
            if (!IsRadiantMonster(card))
            {
                return true;
            }

            // Swen, Dachs and Krosea already resolve their important summon
            // effects when they appear. Their unused status must not prevent
            // Shiina from returning them.
            if (card.IsCode(CardId.RadiantTyphoonSwen, CardId.RadiantTyphoonDachs,
                CardId.RadiantTyphoonKrosea))
            {
                return true;
            }

            // Meghala, Fonix and Vortex are the important Main Deck
            // interruption/resources that should remain on the field until
            // their relevant effect has been used for this turn.
            if (card.IsCode(CardId.RadiantTyphoonMeghala,
                CardId.RadiantTyphoonFonixTheGreatFlame,
                CardId.RadiantTyphoonVaruroonTheVibrantVortex))
            {
                if (card.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex))
                {
                    // Returning Fonix/Vortex is safe when their hand effect
                    // is still available, because the hand special summon
                    // line survives. If that hand effect was already used,
                    // the field effect must also have been used first.
                    return !WasRadiantHandEffectUsedThisTurn(card.Id) ||
                        WasRadiantFieldEffectUsedThisTurn(card.Id);
                }
                if (card.IsCode(CardId.RadiantTyphoonMeghala))
                {
                    // On our turn Meghala is treated like Swen, Dachs and
                    // Krosea: its unused field effect does not by itself
                    // prevent Shiina from returning the monster.
                    if (Duel.Player != 1)
                    {
                        return true;
                    }

                    // On the opponent's turn, keep Meghala on the field only
                    // while its field effect is still available and a set
                    // Radiant Quick-Play or MST can immediately make that
                    // effect valuable. Other facedown non-Radiant Quick-Play
                    // Spells do not affect this exception.
                    return _usedMeghalaDeckSummon ||
                        !HasFaceDownRadiantOrMstQuickPlaySpell();
                }
                return WasRadiantEffectUsedThisTurn(card.Id);
            }

            // Other Radiant Main Deck monsters are not classified as a Shiina
            // interruption lock. Preserve the explicit important-monster list
            // above without making every archetype monster block the effect.
            return true;
        }

        private bool CanActivateShiinaSpellTrapEffectOnOpponentTurn()
        {
            // On our turn the special facedown restriction does not apply.
            if (Duel.Player != 1)
            {
                return true;
            }

            // The Spell/Trap effect returns every field Spell/Trap. On the
            // opponent's turn this is allowed only when there is no facedown
            // card we would lose, or every facedown card was set from our
            // Graveyard by an effect. Tracking the ClientCard instance avoids
            // treating an unrelated facedown card with the same code as safe.
            return Bot.GetSpells().Where(c => c != null && c.IsFacedown())
                .All(c => _botFacedownSpellsSetFromGrave.Contains(c));
        }

        private bool IsSafeShiinaOwnSpellTrap(ClientCard card)
        {
            if (card == null)
            {
                return true;
            }
            return !card.IsCode(CardId.RadiantTyphoonMandate) || !card.IsFaceup();
        }

        private bool WasRadiantHandEffectUsedThisTurn(int cardId)
        {
            if (cardId == CardId.RadiantTyphoonMeghala)
            {
                return _usedMeghalaHandSummon;
            }
            if (cardId == CardId.RadiantTyphoonFonixTheGreatFlame)
            {
                return _usedFonixHandSummon;
            }
            if (cardId == CardId.RadiantTyphoonVaruroonTheVibrantVortex)
            {
                return _usedVortexHandSummon;
            }
            return false;
        }

        private bool WasRadiantFieldEffectUsedThisTurn(int cardId)
        {
            if (cardId == CardId.RadiantTyphoonMeghala)
            {
                return _usedMeghalaDeckSummon;
            }
            if (cardId == CardId.RadiantTyphoonFonixTheGreatFlame)
            {
                return _usedFonixFieldEffect;
            }
            if (cardId == CardId.RadiantTyphoonVaruroonTheVibrantVortex)
            {
                return _usedVortexFieldEffect;
            }
            return false;
        }

        private bool HasFaceDownRadiantOrMstQuickPlaySpell()
        {
            return Bot.GetSpells().Any(c => c != null && c.IsFacedown() &&
                (IsRadiantQuickPlay(c) || c.IsCode(CardId.MysticalSpaceTyphoon)));
        }

        private bool MysticalSpaceTyphoonActivate()
        {
            if (IsTwoCardDropletChainComplete())
            {
                return false;
            }

            if (Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return false;
            }

            // Do not add a redundant MST when the current chain already has an
            // effective negation for the latest opponent chain link.
            if (HasEffectiveOwnNegationForCurrentChain())
            {
                return false;
            }

            // With a face-up Mandate, first put another Radiant quick-play into
            // the chain. MST then destroys that same quick-play so Mandate can
            // negate the opponent's relevant face-up field card in this chain.
            if (ShouldActivateRadiantQuickPlayForMandate())
            {
                return false;
            }

            ClientCard target = GetMysticalSpaceTyphoonTarget();
            if (target == null)
            {
                return false;
            }
            _mysticalSpaceTyphoonTarget = target;
            _activatedRadiantCardsThisTurn.Add(Card.Id);
            return true;
        }

        private ClientCard GetMysticalSpaceTyphoonTarget()
        {
            if (_mysticalSpaceTyphoonTarget != null && _mysticalSpaceTyphoonTarget.IsOnField())
            {
                return _mysticalSpaceTyphoonTarget;
            }

            if (Duel.Player == 0 && Enemy.GetSpellCount() == 0)
            {
                ClientCard ownAscendance = GetOwnChainRadiantQuickPlay();
                if (ownAscendance != null && ownAscendance.IsCode(CardId.RadiantTyphoonAscendance))
                {
                    return ownAscendance;
                }
            }

            bool hasMandate = HasFaceupMandate();
            if (!hasMandate)
            {
                return GetMysticalSpaceTyphoonTargetWithoutMandate();
            }

            ClientCard activatedRadiant = GetOwnChainRadiantQuickPlay();
            if (activatedRadiant != null && CanBuildMandateChainLine())
            {
                return activatedRadiant;
            }

            ClientCard opponentChainTarget = GetBestOpponentMandateChainTarget();
            if (opponentChainTarget != null &&
                (opponentChainTarget.IsSpell() || opponentChainTarget.IsTrap()))
            {
                return opponentChainTarget;
            }

            // If no Radiant quick-play is available, follow the opponent's chain
            // by destroying an opposing spell/trap. The Mandate effect, when
            // legal, will still target the relevant opponent field card above.
            ClientCard enemyTarget = GetBestMandateMstTarget();
            if (enemyTarget != null)
            {
                return enemyTarget;
            }

            ClientCard ownMandate = GetFaceupMandate();
            if (ownMandate != null && ShouldUseMstToTriggerMandateAgainstOpponentMonster())
            {
                return ownMandate;
            }

            if (activatedRadiant != null && CanUseMysticalSpaceTyphoonOnOwnCard())
            {
                return activatedRadiant;
            }

            if (ownMandate != null && ShouldUseMstToTriggerMeghala())
            {
                return ownMandate;
            }

            return null;
        }

        private ClientCard GetMysticalSpaceTyphoonTargetWithoutMandate()
        {
            List<ClientCard> enemySpells = Enemy.GetSpells();
            ClientCard facedown = enemySpells.FirstOrDefault(c => c.IsFacedown());
            if (facedown != null)
            {
                return facedown;
            }

            ClientCard field = enemySpells.FirstOrDefault(c => c.IsFaceup() && c.HasType(CardType.Field));
            if (field != null)
            {
                return field;
            }

            ClientCard continuousTrap = enemySpells.FirstOrDefault(c => c.IsFaceup() &&
                c.IsTrap() && c.HasType(CardType.Continuous));
            if (continuousTrap != null)
            {
                return continuousTrap;
            }

            ClientCard continuousSpell = enemySpells.FirstOrDefault(c => c.IsFaceup() &&
                c.IsSpell() && c.HasType(CardType.Continuous));
            if (continuousSpell != null)
            {
                return continuousSpell;
            }

            return null;
        }

        private ClientCard GetBestMandateMstTarget()
        {
            ClientCard activeOpponentCard = GetBestOpponentMandateChainTarget();
            if (activeOpponentCard != null &&
                (activeOpponentCard.IsSpell() || activeOpponentCard.IsTrap()))
            {
                return activeOpponentCard;
            }

            ClientCard facedown = Enemy.GetSpells().FirstOrDefault(c => c.IsFacedown());
            if (facedown != null)
            {
                return facedown;
            }

            return Enemy.GetSpells().FirstOrDefault(c => c.IsFaceup() &&
                (c.HasType(CardType.Field) || c.HasType(CardType.Continuous))) ??
                Enemy.GetSpells().FirstOrDefault(c => c.IsFaceup());
        }

        private bool HasMysticalSpaceTyphoonPayoff()
        {
            return Bot.HasInMonstersZone(CardId.RadiantTyphoonMeghala) ||
                Bot.HasInMonstersZone(CardId.RadiantTyphoonVaruroonTheMarineEidolon) ||
                Bot.HasInMonstersZone(CardId.RadiantTyphoonFonixTheGreatFlame) ||
                Bot.Graveyard.Any(c => c.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex, CardId.RadiantTyphoonFonixTheGreatFlame));
        }

        private bool CanUseMysticalSpaceTyphoonOnOwnCard()
        {
            if (CanBuildMandateChainLine())
            {
                return true;
            }

            if (!IsRadiantExpansionComplete())
            {
                return false;
            }

            return HasMysticalSpaceTyphoonPayoff() || IsRespondingToEnemyTrap();
        }

        private bool IsRadiantExpansionComplete()
        {
            if (Duel.Player != 0)
            {
                return Bot.GetMonsters().Any(IsRadiantMonster) && !HasPendingRadiantExpansion();
            }

            if (Duel.Phase == DuelPhase.Main2)
            {
                return !HasPendingRadiantExpansion();
            }

            bool hasMarineEidolon = _seaSpiritSummoned || Bot.GetMonsters().Any(c =>
                c.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon));
            return hasMarineEidolon && !HasPendingRadiantExpansion() &&
                !Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonKrosea));
        }

        private bool IsPostExpansionNormalSummonWindow()
        {
            bool hasMarineEidolon = _seaSpiritSummoned || Bot.GetMonsters().Any(c =>
                c.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) && c.IsFaceup());
            return Duel.Player == 0 && IsMainPhase() && hasMarineEidolon &&
                Bot.GetMonsterCount() < 5 && !HasPendingRadiantExpansion();
        }

        private bool HasPendingRadiantExpansion()
        {
            return Bot.GetMonsters().Any(c => IsRadiantMonster(c) && ShouldWaitForRadiantTrigger(c));
        }

        private bool IsRespondingToEnemyTrap()
        {
            return Duel.CurrentChainInfo.Any(c => c.ActivatePlayer == 1 &&
                (c.ActivateType & (int)CardType.Trap) != 0);
        }

        private bool HasFaceupMandate()
        {
            return GetFaceupMandate() != null;
        }

        private bool HasEstablishedMandate()
        {
            return Bot.GetSpells().Any(c => c.IsCode(CardId.RadiantTyphoonMandate) &&
                c.IsFaceup());
        }

        private ClientCard GetFaceupMandate()
        {
            return Bot.GetSpells().FirstOrDefault(c => c.IsCode(CardId.RadiantTyphoonMandate) &&
                c.IsFaceup() && !c.IsDisabled());
        }

        private bool ShouldUseMstToTriggerMeghala()
        {
            if (Card == null || !Card.IsCode(CardId.MysticalSpaceTyphoon) ||
                Card.Location != CardLocation.Hand || Duel.Player != 0 || !IsMainPhase() ||
                Duel.CurrentChain.Count != 0 || Enemy.GetSpellCount() != 0)
            {
                return false;
            }

            if (!Bot.GetMonsters().Any(c => c.IsCode(CardId.RadiantTyphoonMeghala) &&
                c.IsFaceup() && !c.IsDisabled() &&
                !_usedMeghalaDeckSummon))
            {
                return false;
            }

            return !HasOtherSuitableActivationEffect();
        }

        private bool ShouldUseMstToTriggerMandateAgainstOpponentMonster()
        {
            if (Card == null || !Card.IsCode(CardId.MysticalSpaceTyphoon) ||
                Duel.Player != 1 || Card.Location != CardLocation.SpellZone ||
                !Card.IsFacedown() || !HasMstResourceForMandateMonsterLine() ||
                !HasFaceupMandate() || Duel.CurrentChain.Count == 0 ||
                Enemy.GetSpellCount() != 0 || _radiantQuickPlayOfferedInCurrentChainSelection ||
                Duel.CurrentChain.Any(c => c.Controller == 0 && IsRadiantQuickPlay(c)))
            {
                return false;
            }

            ClientCard opponentSource = GetLatestOpponentFieldCardForChain();
            return opponentSource != null && opponentSource.IsMonster() &&
                !opponentSource.IsDisabled();
        }

        private bool HasMstResourceForMandateMonsterLine()
        {
            int mstCount = GetFaceDownMysticalSpaceTyphoonCount();
            if (mstCount == 0)
            {
                return false;
            }

            // Keep the existing last-copy line. When several MST are set and
            // no other Radiant Quick-Play is available, spend only one MST and
            // preserve the remaining copies for later back-row interaction.
            return mstCount == 1 || !HasFaceDownRadiantQuickPlayBesidesMst();
        }

        private bool HasFaceDownRadiantQuickPlayBesidesMst()
        {
            return Bot.GetSpells().Any(c => c != null && c.IsFacedown() &&
                IsRadiantQuickPlay(c));
        }

        private bool HasOtherSuitableActivationEffect()
        {
            if (Duel.MainPhase == null)
            {
                return false;
            }

            return Duel.MainPhase.ActivableCards.Any(c => c != null &&
                !c.IsCode(CardId.MysticalSpaceTyphoon, CardId.RadiantTyphoonMandate,
                    CardId.RadiantTyphoonMeghala) && !AreSameVisibleCard(c, Card));
        }

        private ClientCard GetOwnChainRadiantQuickPlay()
        {
            return Duel.CurrentChain.Reverse().FirstOrDefault(c => c != null && c.Controller == 0 &&
                c.IsOnField() && c.Location == CardLocation.SpellZone && IsRadiantQuickPlay(c));
        }

        private ClientCard GetOpponentFieldCardForChain(ChainInfo chain)
        {
            if (chain == null || chain.ActivatePlayer != 1)
            {
                return null;
            }

            ClientCard related = chain.RelatedCard;
            ClientCard fieldCard = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .FirstOrDefault(c => c == related);
            if (fieldCard == null && chain.ActivateController == 1 &&
                (chain.HasLocation(CardLocation.MonsterZone) ||
                 chain.HasLocation(CardLocation.SpellZone) ||
                 chain.HasLocation(CardLocation.FieldZone)))
            {
                fieldCard = Enemy.GetMonsters().Concat(Enemy.GetSpells()).FirstOrDefault(c =>
                    c.Location == chain.ActivateLocation && c.Sequence == chain.ActivateSequence);
            }
            if (fieldCard == null && related != null && related.IsOnField() && related.Controller == 1)
            {
                fieldCard = related;
            }
            if (fieldCard == null || !fieldCard.IsFaceup())
            {
                return null;
            }
            return fieldCard;
        }

        private ClientCard GetBestOpponentMandateChainTarget()
        {
            for (int i = Duel.CurrentChainInfo.Count - 1; i >= 0; --i)
            {
                if (Duel.NegatedChainIndexList.Contains(i + 1))
                {
                    continue;
                }

                ClientCard target = GetOpponentFieldCardForChain(Duel.CurrentChainInfo[i]);
                if (target != null && !target.IsDisabled())
                {
                    return target;
                }
            }
            return null;
        }

        private ClientCard GetLatestOpponentFieldCardForChain()
        {
            for (int i = Duel.CurrentChainInfo.Count - 1; i >= 0; --i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain == null || chain.ActivatePlayer != 1 ||
                    Duel.NegatedChainIndexList.Contains(i + 1))
                {
                    continue;
                }

                ClientCard fieldCard = GetOpponentFieldCardForChain(chain);
                if (fieldCard != null)
                {
                    return fieldCard;
                }
            }
            return null;
        }

        private ClientCard GetBestMandateNegationTarget()
        {
            ClientCard chainTarget = GetBestOpponentMandateChainTarget();
            if (chainTarget != null)
            {
                return chainTarget;
            }

            return GetOrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .Where(c => c.IsFaceup() && !c.IsDisabled())).FirstOrDefault();
        }

        private bool HasLiveOpponentChain()
        {
            for (int i = 0; i < Duel.CurrentChainInfo.Count; ++i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain != null && chain.ActivatePlayer == 1 &&
                    !Duel.NegatedChainIndexList.Contains(i + 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CanBuildMandateChainLine()
        {
            return HasLiveOpponentChain() && !HasEffectiveOwnNegationForCurrentChain() &&
                HasFaceupMandate() &&
                GetOwnChainRadiantQuickPlay() != null &&
                GetLatestOpponentFieldCardForChain() != null;
        }

        private bool CanUseMandateToNegateCurrentChain()
        {
            ChainInfo latestChain = Duel.CurrentChainInfo.LastOrDefault();
            return HasLiveOpponentChain() && !HasEffectiveOwnNegationForCurrentChain() &&
                HasFaceupMandate() &&
                GetLatestOpponentFieldCardForChain() != null &&
                latestChain != null && latestChain.IsActivateCode(CardId.MysticalSpaceTyphoon);
        }

        private bool ShouldActivateRadiantQuickPlayForMandate()
        {
            return HasLiveOpponentChain() && !HasEffectiveOwnNegationForCurrentChain() &&
                HasFaceupMandate() &&
                GetLatestOpponentFieldCardForChain() != null &&
                !Duel.CurrentChain.Any(c => c.Controller == 0 && IsRadiantQuickPlay(c)) &&
                !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon)) &&
                _mstOfferedInCurrentChainSelection &&
                _radiantQuickPlayOfferedInCurrentChainSelection;
        }

        private bool HasEffectiveOwnNegationForCurrentChain()
        {
            int latestOpponentIndex = -1;
            for (int i = 0; i < Duel.CurrentChainInfo.Count; ++i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain != null && chain.ActivatePlayer == 1)
                {
                    latestOpponentIndex = i;
                }
            }

            if (latestOpponentIndex < 0)
            {
                return false;
            }

            // The core may already have reported that this link is negated
            // before the next response query. Treat that as covered as well.
            if (Duel.NegatedChainIndexList.Contains(latestOpponentIndex + 1))
            {
                return true;
            }

            for (int i = latestOpponentIndex + 1; i < Duel.CurrentChainInfo.Count; ++i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain == null || chain.ActivatePlayer != 0 ||
                    Duel.NegatedChainIndexList.Contains(i + 1))
                {
                    continue;
                }

                if (chain.IsActivateCode(CardId.TotemBird))
                {
                    return IsOpponentSpellOrTrapEffectChain(Duel.CurrentChainInfo[latestOpponentIndex]);
                }

                if (chain.IsActivateCode(CardId.AshBlossomJoyousSpring))
                {
                    // The server only offers Ash when the latest opponent
                    // effect is a legal Ash target.
                    return true;
                }

                if (chain.IsActivateCode(CardId.InfiniteImpermanence,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex))
                {
                    bool isVortexFieldEffect = !chain.IsActivateCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex) ||
                        chain.HasLocation(CardLocation.MonsterZone) ||
                        chain.ActivateDescription ==
                        Util.GetStringId(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 1);
                    return isVortexFieldEffect &&
                        IsOpponentMonsterEffectChain(Duel.CurrentChainInfo[latestOpponentIndex]);
                }

                if (chain.IsActivateCode(CardId.ForbiddenDroplet))
                {
                    // Droplet may have been chained to our own card, so use
                    // the latest opponent link and its available targets.
                    return IsOpponentMonsterEffectChain(Duel.CurrentChainInfo[latestOpponentIndex]) &&
                        GetDropletTargetMonsters().Count > 0;
                }

                if (chain.IsActivateCode(CardId.RadiantTyphoonMandate) &&
                    chain.ActivateDescription == Util.GetStringId(CardId.RadiantTyphoonMandate, 1))
                {
                    // Mandate negates the related chain effects of its selected
                    // face-up card, so compare that target with the latest link.
                    ClientCard opponentSource = GetOpponentFieldCardForChain(
                        Duel.CurrentChainInfo[latestOpponentIndex]);
                    if (opponentSource == null)
                    {
                        continue;
                    }

                    bool targetsLatestOpponentSource = chain.Targets.Any(target =>
                        AreSameVisibleCard(target, opponentSource));
                    if (!targetsLatestOpponentSource && _mandateNegationTarget != null)
                    {
                        targetsLatestOpponentSource = AreSameVisibleCard(
                            _mandateNegationTarget, opponentSource);
                    }
                    if (targetsLatestOpponentSource)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsOpponentSpellOrTrapEffectChain(ChainInfo chain)
        {
            return chain != null && chain.ActivatePlayer == 1 &&
                ((chain.ActivateType & (int)CardType.Spell) != 0 ||
                 (chain.ActivateType & (int)CardType.Trap) != 0);
        }

        private bool ForbiddenDropletActivate()
        {
            List<ClientCard> enemyTargets = GetDropletTargetMonsters();
            if (enemyTargets.Count == 0)
            {
                return false;
            }

            ChainInfo lastChain = Duel.CurrentChainInfo.LastOrDefault();
            ClientCard respondingMonster = GetChainSourceCard(lastChain);
            bool respondingToEnemyFieldMonster = respondingMonster != null &&
                respondingMonster.IsFaceup() && respondingMonster.HasType(CardType.Effect) &&
                !respondingMonster.IsDisabled();
            bool respondingToOwnFieldSpell = lastChain != null && lastChain.ActivatePlayer == 0 &&
                (lastChain.ActivateType & (int)CardType.Spell) != 0 &&
                lastChain.HasLocation(CardLocation.SpellZone | CardLocation.FieldZone);
            bool respondingToOwnQuickPlay = lastChain != null && lastChain.ActivatePlayer == 0 &&
                lastChain.HasType(CardType.QuickPlay);
            bool respondingToOwnSmallRadiant = lastChain != null && lastChain.ActivatePlayer == 0 &&
                lastChain.HasType(CardType.Monster) &&
                lastChain.IsActivateCode(CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs, CardId.RadiantTyphoonMeghala);
            if (Duel.Player == 0 && !respondingToOwnQuickPlay && !respondingToOwnSmallRadiant)
            {
                return false;
            }
            if (!respondingToEnemyFieldMonster && !respondingToOwnFieldSpell && !respondingToOwnQuickPlay &&
                !respondingToOwnSmallRadiant)
            {
                return false;
            }

            if (Duel.Player == 0 && !Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                (c.IsExtraCard() || c.Attack >= 2500)))
            {
                return false;
            }

            if (CanUseTwoCardDropletBreakthrough() && enemyTargets.Count < 2)
            {
                return false;
            }

            return Bot.Hand.Concat(Bot.GetMonsters()).Concat(Bot.GetSpells())
                .Any(IsDropletCostCandidate);
        }

        private bool TwoCardDropletBreakthroughActivate()
        {
            return CanUseTwoCardDropletBreakthrough() && ForbiddenDropletActivate();
        }

        private bool CanUseTwoCardDropletBreakthrough()
        {
            return IsGoingSecondBreakthroughTurn() &&
                GetOwnDropletChainCards().Count >= 2 &&
                GetDropletTargetMonsters().Count >= 2;
        }

        private bool ShouldContinueTwoCardDropletChain()
        {
            return IsGoingSecondBreakthroughTurn() && Bot.HasInHand(CardId.ForbiddenDroplet) &&
                GetOwnDropletChainCards().Count == 1 &&
                GetDropletTargetMonsters().Count >= 2 &&
                Card != null && !Duel.CurrentChain.Contains(Card);
        }

        private bool IsTwoCardDropletChainComplete()
        {
            return IsGoingSecondBreakthroughTurn() &&
                GetOwnDropletChainCards().Count >= 2 &&
                GetDropletTargetMonsters().Count >= 2;
        }

        private bool IsGoingSecondBreakthroughTurn()
        {
            return !Duel.IsFirst && Duel.Player == 0 && Duel.Turn > 1 && IsMainPhase();
        }

        private List<ClientCard> GetDropletTargetMonsters()
        {
            return GetOrderedDropletTargets(Enemy.GetMonsters());
        }

        private List<ClientCard> GetOrderedDropletTargets(IEnumerable<ClientCard> cards)
        {
            ClientCard problem = Util.GetProblematicEnemyMonster(0, false);
            return cards.Where(c => c != null && c.Controller == 1 && c.IsFaceup() &&
                    c.HasType(CardType.Effect) && !c.IsDisabled())
                .OrderByDescending(c => c == problem)
                .ThenByDescending(c => c.IsFloodgate() || c.IsMonsterDangerous() ||
                    c.IsMonsterInvincible() || c.IsMonsterShouldBeDisabledBeforeItUseEffect())
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense))
                .ToList();
        }

        private List<ClientCard> GetOwnDropletChainCards()
        {
            return Duel.CurrentChain.Where(IsDropletChainCostCandidate).Distinct().ToList();
        }

        private bool IsCurrentDropletChainCard(ClientCard card)
        {
            if (!IsDropletCostCandidate(card))
            {
                return false;
            }

            return Duel.CurrentChain.Any(chainCard =>
                IsDropletChainCostCandidate(chainCard) && AreSameVisibleCard(chainCard, card));
        }

        private bool IsDropletChainCostCandidate(ClientCard card)
        {
            return card != null && card.Controller == 0 &&
                !card.IsCode(CardId.ForbiddenDroplet, CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonMandate) && !card.IsFacedown() &&
                (card.Location == CardLocation.Hand || card.IsOnField());
        }

        private bool AreSameVisibleCard(ClientCard first, ClientCard second)
        {
            return first == second || (first != null && second != null &&
                first.Id == second.Id && first.Controller == second.Controller &&
                first.Location == second.Location && first.Sequence == second.Sequence);
        }

        private bool SuperPolymerizationActivate()
        {
            if (Bot.Hand.All(c => c == Card) || ShouldStopRadiantSpecialSummon(CardLocation.Extra))
            {
                return false;
            }

            int fusionId;
            IList<ClientCard> fusionMaterials;
            if (!TryGetSuperPolymerizationPlan(out fusionId, out fusionMaterials))
            {
                return false;
            }

            List<ClientCard> discardPriority = Bot.Hand.Where(c => c != Card)
                .OrderBy(GetDiscardPriority).ToList();
            AI.SelectCard(discardPriority);
            AI.SelectNextCard(fusionId);
            AI.SelectMaterials(fusionMaterials);
            return true;
        }

        private bool TryGetSuperPolymerizationPlan(out int fusionId, out IList<ClientCard> fusionMaterials)
        {
            fusionId = 0;
            fusionMaterials = null;
            List<ClientCard> available = Bot.GetMonsters().Concat(Enemy.GetMonsters())
                .Where(c => c.IsFaceup() &&
                    !(c.Controller == 0 && c.IsCode(CardId.RadiantTyphoonMeghala))).ToList();
            ClientCard problem = Util.GetProblematicEnemyMonster(0, false) ??
                Util.GetBestEnemyMonster(true, false);
            int bestScore = Int32.MinValue;

            for (int i = 0; i < available.Count; ++i)
            {
                for (int j = i + 1; j < available.Count; ++j)
                {
                    ClientCard first = available[i];
                    ClientCard second = available[j];
                    if (first.Controller == 0 && second.Controller == 0)
                    {
                        continue;
                    }

                    int candidateFusion = GetFusionForPair(first, second);
                    if (candidateFusion == 0)
                    {
                        continue;
                    }

                    int enemyMaterialCount = (first.Controller == 1 ? 1 : 0) + (second.Controller == 1 ? 1 : 0);
                    bool containsProblem = problem != null && (first == problem || second == problem);
                    if (enemyMaterialCount < 2 && !containsProblem)
                    {
                        continue;
                    }

                    int score = enemyMaterialCount * 100;
                    if (containsProblem)
                    {
                        score += 80;
                    }
                    if (first.Controller == 1)
                    {
                        score += Math.Max(first.Attack, first.Defense) / 100;
                    }
                    else
                    {
                        score -= GetMaterialPriority(first) * 5;
                    }
                    if (second.Controller == 1)
                    {
                        score += Math.Max(second.Attack, second.Defense) / 100;
                    }
                    else
                    {
                        score -= GetMaterialPriority(second) * 5;
                    }

                    if (score > bestScore)
                    {
                        bestScore = score;
                        fusionId = candidateFusion;
                        fusionMaterials = new List<ClientCard> { first, second };
                    }
                }
            }
            return fusionId != 0;
        }

        private bool MudragonActivate()
        {
            if (IsDisabledOwnFieldMonster() || Card == null || Card.Location != CardLocation.MonsterZone ||
                !Card.IsFaceup())
            {
                return false;
            }

            // Mudragon's attribute-change effect can also be activated during the
            // opponent's turn. The server has already filtered the legal timing;
            // whenever it offers this sole effect, declare Wind for the Radiant
            // Typhoon lines that use this Fusion monster.
            AI.SelectAttribute(CardAttribute.Wind);
            return true;
        }

        private int GetFusionForPair(ClientCard first, ClientCard second)
        {
            if (Bot.HasInExtra(CardId.GaruraWingsOfResonantLife) &&
                CanUseGaruraMaterials(first, second))
            {
                return CardId.GaruraWingsOfResonantLife;
            }
            if (Bot.HasInExtra(CardId.MudragonOfTheSwamp) &&
                CanUseMudragonMaterials(first, second))
            {
                return CardId.MudragonOfTheSwamp;
            }
            if (Bot.HasInExtra(CardId.FavoriteHEROFlameWingman) &&
                CanUseFlameWingmanMaterials(first, second))
            {
                return CardId.FavoriteHEROFlameWingman;
            }
            if (Bot.HasInExtra(CardId.FavoriteHEROShiningFlareWingman) &&
                CanUseShiningFlareWingmanMaterials(first, second))
            {
                return CardId.FavoriteHEROShiningFlareWingman;
            }
            return 0;
        }

        private bool CanUseGaruraMaterials(ClientCard first, ClientCard second)
        {
            return IsFaceupSuperPolyMonster(first) && IsFaceupSuperPolyMonster(second) &&
                first.Race == second.Race && first.Attribute == second.Attribute &&
                first.Id != second.Id;
        }

        private bool CanUseMudragonMaterials(ClientCard first, ClientCard second)
        {
            return IsFaceupSuperPolyMonster(first) && IsFaceupSuperPolyMonster(second) &&
                first.Attribute == second.Attribute && first.Race != second.Race;
        }

        private bool CanUseFlameWingmanMaterials(ClientCard first, ClientCard second)
        {
            return IsFaceupSuperPolyMonster(first) && IsFaceupSuperPolyMonster(second) &&
                first.Race == second.Race && first.Attribute != second.Attribute;
        }

        private bool CanUseShiningFlareWingmanMaterials(ClientCard first, ClientCard second)
        {
            return IsFaceupSuperPolyMonster(first) && IsFaceupSuperPolyMonster(second) &&
                (first.HasType(CardType.Fusion) || second.HasType(CardType.Fusion));
        }

        private bool IsFaceupSuperPolyMonster(ClientCard card)
        {
            return card != null && card.IsMonster() && card.IsFaceup() &&
                card.Location == CardLocation.MonsterZone;
        }

        private bool TheFallenTheVirtuousActivate()
        {
            if (!HasFallenExtraDeckCost())
            {
                return false;
            }

            ClientCard dodgeTarget = GetFallenNegationDodgeTarget();
            if (dodgeTarget != null)
            {
                _fallenDodgeTarget = dodgeTarget;
                return true;
            }

            bool canDestroy = GetOrderedFallenTargets(Enemy.GetMonsters().Concat(Enemy.GetSpells())).Count > 0;
            if (canDestroy)
            {
                _fallenDodgeTarget = null;
            }
            return canDestroy;
        }

        private bool TheFallenNegationDodgeActivate()
        {
            ClientCard dodgeTarget = GetFallenNegationDodgeTarget();
            if (dodgeTarget == null || !HasFallenExtraDeckCost())
            {
                return false;
            }

            _fallenDodgeTarget = dodgeTarget;
            return true;
        }

        private bool HasFallenExtraDeckCost()
        {
            return Bot.HasInExtra(CardId.AlbionTheBrandedDragon) ||
                Bot.HasInExtra(CardId.EcclesiaAndTheDarkDragon);
        }

        private ClientCard GetFallenNegationDodgeTarget()
        {
            ChainInfo lastChain = Duel.CurrentChainInfo.LastOrDefault();
            if (lastChain == null || lastChain.ActivatePlayer != 1 ||
                !lastChain.IsActivateCode(CardId.EffectVeiler, CardId.InfiniteImpermanence))
            {
                return null;
            }

            return Duel.LastChainTargets.FirstOrDefault(c => c != null && c.Controller == 0 &&
                c.Location == CardLocation.MonsterZone && c.IsFaceup());
        }

        private bool GallantThiefActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.TheWorldsGreatestGallantThief, 1))
            {
                return Duel.LastChainPlayer == 1;
            }

            if (IsDescription(CardId.TheWorldsGreatestGallantThief, 2))
            {
                return Enemy.GetMonsters().Any(c => c.IsAttack());
            }
            return false;
        }

        private bool KroseaActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            // Krosea has an activated hand effect and a separate on-summon trigger.
            // The latter can be offered with ActivateDescription == -1, so location is
            // the reliable discriminator after the server has supplied a legal effect.
            if (Card.Location == CardLocation.Hand)
            {
                return CanSummonFromHandAfterPurulia();
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (_enemyDrollResolved)
                {
                    return false;
                }
                _usedKroseaSearch = true;
                return true;
            }
            return false;
        }

        private bool MeghalaActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            // Its self summon is a SpSummon procedure. Any legal Activate candidate
            // while Meghala is in the monster zone is therefore its deck-summon trigger.
            if (Card.Location != CardLocation.MonsterZone || _usedMeghalaDeckSummon ||
                ShouldStopRadiantSpecialSummon(CardLocation.Deck))
            {
                return false;
            }
            _usedMeghalaDeckSummon = true;
            return true;
        }

        private bool SwenActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            // Swen has only one activated effect; do not reject its summon trigger when
            // the protocol omits the effect description and reports -1.
            if (Card.Location != CardLocation.MonsterZone || _usedSwenSearch || _enemyDrollResolved)
            {
                return false;
            }
            _usedSwenSearch = true;
            return true;
        }

        private bool DachsActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            // Dachs has only one activated effect; the legal field candidate is its
            // normal/special-summon search trigger regardless of description encoding.
            if (Card.Location != CardLocation.MonsterZone || _usedDachsSearch || _enemyDrollResolved)
            {
                return false;
            }
            _usedDachsSearch = true;
            return true;
        }

        private bool SmallRadiantSpecialSummon()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() || Bot.GetMonsterCount() >= 5 ||
                ShouldStopRadiantSpecialSummon(CardLocation.Hand))
            {
                return false;
            }

            // Keep Swen for a normal summon and its search only when a hand Meghala
            // is also present in the server-provided special-summon candidates.
            if (Card.IsCode(CardId.RadiantTyphoonSwen) &&
                Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala)) &&
                IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonMeghala))
            {
                return false;
            }

            // With Swen and Dachs together, preserve Dachs only when Swen is also
            // present in the server-provided special-summon candidates.
            if (Card.IsCode(CardId.RadiantTyphoonDachs) &&
                Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonSwen)) &&
                IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonSwen))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                _usedMeghalaHandSummon = true;
            }
            if (Card.IsCode(CardId.RadiantTyphoonSwen))
            {
                _usedSwenHandSummon = true;
            }
            if (Card.IsCode(CardId.RadiantTyphoonDachs))
            {
                _usedDachsHandSummon = true;
            }
            return true;
        }

        private bool SmallRadiantNormalSummon()
        {
            return CanSummonFromHandAfterPurulia() && IsMainPhase();
        }

        private bool KroseaNormalSummon()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() || _enemyDrollResolved || HasRadiantSpellInHand() ||
                !CanUsePreferredKroseaTribute())
            {
                return false;
            }
            return true;
        }

        private bool GallantThiefSummon()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() ||
                Bot.GetMonsterCount() != 0 || _skipGallantThiefSummonThisTurn ||
                !CanUseEnemyTributesForGallantThief())
            {
                return false;
            }

            _selectingGallantThiefTributes = true;
            return true;
        }

        private bool CanUseEnemyTributesForGallantThief()
        {
            List<ClientCard> usableMonsters = Enemy.GetMonsters()
                .Where(monster => !monster.IsMonsterNotBeSummonTribute()).ToList();
            return usableMonsters.Count >= 2 || usableMonsters.Any(IsSuitableGallantThiefTribute);
        }

        private bool IsSuitableGallantThiefTribute(ClientCard monster)
        {
            return monster != null && monster.IsFaceup() &&
                ((!monster.IsDisabled() && monster.IsFloodgate()) ||
                    monster.IsMonsterDangerous() || monster.IsMonsterInvincible() ||
                    monster.Attack >= 3000);
        }

        private bool RadiantQuickPlayMstStarterActivate()
        {
            if (Card.Location != CardLocation.Grave &&
                (!HasUnusedRadiantQuickPlayEffect(Card.Id) ||
                 ShouldDelayDuplicateRadiantQuickPlay() ||
                 ShouldDelayRadiantMstSearchUntilExpansion()))
            {
                return false;
            }

            if (Card.Location == CardLocation.Grave || !NeedMstStarter() ||
                ShouldDelayOpponentAscendanceMstStarter())
            {
                return false;
            }

            if (IsTwoCardDropletChainComplete())
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonAscendance) && !CanActivateAscendanceInCurrentTurn())
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonAscendance) && !CanActivateAscendanceNow())
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonVision) && !CanActivateVisionInCurrentTurn())
            {
                return false;
            }

            if (IsMainPhase() && Duel.CurrentChain.Count == 0 && CanSummonSeaSpiritNow())
            {
                return false;
            }

            if (CanSpecialSummonMeghalaFromHandNow())
            {
                return false;
            }

            return AcceptRadiantQuickPlayActivation();
        }

        private bool RadiantQuickPlayActivate()
        {
            if (Card.Location != CardLocation.Grave &&
                (!HasUnusedRadiantQuickPlayEffect(Card.Id) ||
                 ShouldDelayDuplicateRadiantQuickPlay() ||
                 ShouldDelayRadiantMstSearchUntilExpansion()))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonVision) && !CanActivateVisionInCurrentTurn())
            {
                return false;
            }

            if (Card.Location == CardLocation.Grave)
            {
                return AcceptRadiantQuickPlayActivation();
            }

            // On the opponent's turn, Ascendance's MST-starter route is a
            // response line, not an opening action. A hand/field Ascendance
            // must wait for an effective opponent field chain.
            if (Card.IsCode(CardId.RadiantTyphoonAscendance) && NeedMstStarter() &&
                ShouldDelayOpponentAscendanceMstStarter())
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonAscendance) &&
                (!CanActivateAscendanceInCurrentTurn() || !CanActivateAscendanceNow()))
            {
                return false;
            }

            if (ShouldContinueTwoCardDropletChain())
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (IsTwoCardDropletChainComplete())
            {
                return false;
            }

            if (IsDescription(Card.Id, 1))
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (ShouldActivateRadiantQuickPlayForMandate())
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (CanSummonFromHandAfterPurulia() && Bot.HasInHand(CardId.RadiantTyphoonMeghala))
            {
                if (CanSpecialSummonMeghalaFromHandNow() || !NeedMstStarter())
                {
                    return false;
                }
            }

            if (Util.IsChainTarget(Card))
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (NeedMstStarter())
            {
                return AcceptRadiantQuickPlayActivation();
            }

            // Going first, Vision should be used as early as the turn-one Draw Phase.
            if (Card.IsCode(CardId.RadiantTyphoonVision) && Duel.Player == 0 &&
                Duel.Turn == 1 && Duel.Phase == DuelPhase.Draw)
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (ShouldPrioritizeMstAgainstBackrow())
            {
                return AcceptRadiantQuickPlayActivation();
            }

            bool hasPayoff = HasQuickPlayActivationPayoff();
            if (_enemyDrollResolved && !hasPayoff)
            {
                return false;
            }

            if (IsMainPhase() && Duel.CurrentChain.Count == 0 && CanSummonSeaSpiritNow())
            {
                return false;
            }

            if (IsMainPhase() && Duel.Player == 0)
            {
                return AcceptRadiantQuickPlayActivation();
            }
            if (hasPayoff && Duel.CurrentChain.Count > 0)
            {
                return AcceptRadiantQuickPlayActivation();
            }
            return false;
        }

        private bool RadiantQuickPlayMandateChainActivate()
        {
            if (Card.Location != CardLocation.Grave &&
                (!HasUnusedRadiantQuickPlayEffect(Card.Id) ||
                 ShouldDelayDuplicateRadiantQuickPlay()))
            {
                return false;
            }

            if (!ShouldActivateRadiantQuickPlayForMandate())
            {
                return false;
            }

            if (IsTwoCardDropletChainComplete())
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonVision) && !CanActivateVisionInCurrentTurn())
            {
                return false;
            }

            return CanActivateAscendanceInCurrentTurn() && CanActivateAscendanceNow() &&
                AcceptRadiantQuickPlayActivation();
        }

        private bool AcceptRadiantQuickPlayActivation()
        {
            _activatedRadiantCardsThisTurn.Add(Card.Id);
            return true;
        }

        private bool HasUnusedRadiantQuickPlayEffect(int cardId)
        {
            if (cardId == CardId.RadiantTyphoonChant)
            {
                return !_usedChantMonsterSearch || !_usedChantMstSearch;
            }
            if (cardId == CardId.RadiantTyphoonVision)
            {
                return !_usedVisionDraw || !_usedVisionMstSearch;
            }
            if (cardId == CardId.RadiantTyphoonAscendance)
            {
                return !_usedAscendanceRevive || !_usedAscendanceMstSearch;
            }
            return true;
        }

        private bool IsUnusedRadiantQuickPlayEffect(int cardId, int offset)
        {
            if (cardId == CardId.RadiantTyphoonChant)
            {
                return offset == 2 ? !_usedChantMonsterSearch :
                    offset == 3 && !_usedChantMstSearch;
            }
            if (cardId == CardId.RadiantTyphoonVision)
            {
                return offset == 2 ? !_usedVisionDraw :
                    offset == 3 && !_usedVisionMstSearch;
            }
            if (cardId == CardId.RadiantTyphoonAscendance)
            {
                return offset == 2 ? !_usedAscendanceRevive :
                    offset == 3 && !_usedAscendanceMstSearch;
            }
            return false;
        }

        private bool HasSameRadiantQuickPlayInCurrentChain(int cardId)
        {
            return Duel.CurrentChain.Any(c => c != null && c.Controller == 0 &&
                c.IsCode(cardId));
        }

        private bool ShouldDelayDuplicateRadiantQuickPlay()
        {
            if (Card == null || Card.Location == CardLocation.Grave || Duel.Player != 0 ||
                Duel.CurrentChain.Count == 0 || HasLiveOpponentChain() ||
                !IsRadiantQuickPlay(Card))
            {
                return false;
            }

            return HasSameRadiantQuickPlayInCurrentChain(Card.Id) &&
                !IsRadiantExpansionComplete();
        }

        private bool ShouldDelayRadiantMstSearchUntilExpansion()
        {
            if (Card == null || Card.Location == CardLocation.Grave || Duel.Player != 0 ||
                HasLiveOpponentChain() || IsRadiantExpansionComplete() ||
                Enemy.GetSpellCount() > 0)
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonChant))
            {
                return _usedChantMonsterSearch && !_usedChantMstSearch;
            }
            if (Card.IsCode(CardId.RadiantTyphoonVision))
            {
                return _usedVisionDraw && !_usedVisionMstSearch;
            }
            if (Card.IsCode(CardId.RadiantTyphoonAscendance))
            {
                return _usedAscendanceRevive && !_usedAscendanceMstSearch;
            }
            return false;
        }

        private bool HasQuickPlayActivationPayoff()
        {
            return Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonFonixTheGreatFlame)) ||
                Bot.GetMonsters().Any(c => c.IsCode(CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonVaruroonTheMarineEidolon,
                    CardId.RadiantTyphoonFonixTheGreatFlame));
        }

        private bool ShouldPrioritizeMstAgainstBackrow()
        {
            return Duel.Player == 0 && Duel.Turn > 1 && Enemy.GetSpellCount() > 0 && !_enemyDrollResolved;
        }

        private bool ShouldUseChantMstSearch()
        {
            if (_usedChantMstSearch)
            {
                return false;
            }
            if (_usedChantMonsterSearch && IsRadiantExpansionComplete())
            {
                return true;
            }
            return Enemy.GetSpellCount() > 0 && !Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon);
        }

        private int GetPreferredVisionEffectOffset()
        {
            bool canDraw = !_usedVisionDraw;
            bool canSearchMst = !_usedVisionMstSearch;
            if (!canDraw)
            {
                return canSearchMst ? 3 : -1;
            }
            if (!canSearchMst)
            {
                return 2;
            }

            if (NeedMstStarter() || ShouldPrioritizeMstAgainstBackrow())
            {
                return 3;
            }
            // When there is no opposing backrow, Vision's draw-two/discard-one
            // effect is the engine action. Its MST search is a later resource
            // action, after the current expansion has finished.
            return 2;
        }

        private int GetPreferredChantEffectOffset()
        {
            bool canSearchMonster = !_usedChantMonsterSearch;
            bool canSearchMst = !_usedChantMstSearch;
            if (!canSearchMonster)
            {
                return canSearchMst ? 3 : -1;
            }
            if (!canSearchMst)
            {
                return 2;
            }
            return ShouldUseChantMstSearch() ? 3 : 2;
        }

        private int GetPreferredAscendanceEffectOffset()
        {
            bool canRevive = !_usedAscendanceRevive;
            bool canSearchMst = !_usedAscendanceMstSearch;
            if (Duel.Player != 0)
            {
                return canRevive ? 2 : -1;
            }
            if (!canRevive)
            {
                return canSearchMst ? 3 : -1;
            }
            if (!canSearchMst)
            {
                return 2;
            }
            if (NeedMstStarter() || ShouldPrioritizeMstAgainstBackrow())
            {
                return 3;
            }
            return CanUseAscendanceReviveNow() ? 2 : 3;
        }

        private void RegisterRadiantQuickPlayEffectSelection(int cardId, int offset)
        {
            int chainIndex = GetCurrentRadiantChainIndex();
            if (chainIndex > 0)
            {
                _radiantResolutionEffectOffsets[chainIndex] = offset;
            }

            if (cardId == CardId.RadiantTyphoonChant)
            {
                if (offset == 2)
                {
                    _usedChantMonsterSearch = true;
                }
                else if (offset == 3)
                {
                    _usedChantMstSearch = true;
                }
            }
            else if (cardId == CardId.RadiantTyphoonVision)
            {
                if (offset == 2)
                {
                    _usedVisionDraw = true;
                }
                else if (offset == 3)
                {
                    _usedVisionMstSearch = true;
                }
            }
            else if (cardId == CardId.RadiantTyphoonAscendance)
            {
                if (offset == 2)
                {
                    _usedAscendanceRevive = true;
                }
                else if (offset == 3)
                {
                    _usedAscendanceMstSearch = true;
                }
            }
        }

        private int GetCurrentRadiantChainIndex()
        {
            if (Duel.SolvingChainIndex > 0)
            {
                return Duel.SolvingChainIndex;
            }

            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            if (currentChainCard != null)
            {
                for (int i = 0; i < Duel.CurrentChain.Count; ++i)
                {
                    if (Duel.CurrentChain[i] == currentChainCard)
                    {
                        return i + 1;
                    }
                }
            }

            return Duel.CurrentChainInfo.Count;
        }

        private bool IsRadiantResolutionEffect(int cardId, int offset)
        {
            int selectedOffset;
            return Duel.SolvingChainIndex > 0 &&
                _radiantResolutionEffectOffsets.TryGetValue(Duel.SolvingChainIndex,
                    out selectedOffset) && selectedOffset == offset &&
                Duel.GetCurrentSolvingChainInfo() != null &&
                Duel.GetCurrentSolvingChainInfo().IsActivateCode(cardId);
        }

        private bool CanActivateAscendanceNow()
        {
            if (!Card.IsCode(CardId.RadiantTyphoonAscendance))
            {
                return true;
            }

            // Ascendance is a late engine action. If the server still offers a
            // direct hand Special Summon for Swen, Dachs, or Meghala, let that
            // summon happen first even when Ascendance is also being offered as
            // the MST starter against opposing backrow.
            if (HasDirectSmallRadiantSpecialSummonNow())
            {
                return false;
            }

            // Other Radiant Quick-Play Spells have priority over Ascendance. Use
            // the visible hand/field cards here because this is the same
            // late-activation policy used by the existing main-phase logic.
            if (Bot.Hand.Concat(Bot.GetSpells()).Any(c => c != Card && IsRadiantQuickPlay(c)))
            {
                return false;
            }

            return true;
        }

        private bool CanActivateAscendanceInCurrentTurn()
        {
            if (!Card.IsCode(CardId.RadiantTyphoonAscendance))
            {
                return true;
            }

            if (Duel.Player != 0 && (Bot.GetMonsterCount() >= 5 || !HasRadiantReviveTarget()))
            {
                return false;
            }

            if (Duel.Player != 0 && _usedAscendanceRevive)
            {
                return false;
            }

            return !IsInChainWithOwnRadiantEffect();
        }

        private bool CanActivateVisionInCurrentTurn()
        {
            if (!Card.IsCode(CardId.RadiantTyphoonVision))
            {
                return true;
            }

            return !IsInChainWithOwnRadiantMonsterEffect();
        }

        private bool IsInChainWithOwnRadiantEffect()
        {
            return Duel.CurrentChainInfo.Any(chain => chain.ActivatePlayer == 0 &&
                (IsOwnRadiantMonsterEffectChain(chain) ||
                 chain.IsActivateCode(CardId.RadiantTyphoonChant,
                     CardId.RadiantTyphoonVision, CardId.RadiantTyphoonAscendance)));
        }

        private bool IsInChainWithOwnRadiantMonsterEffect()
        {
            return Duel.CurrentChainInfo.Any(IsOwnRadiantMonsterEffectChain);
        }

        private bool IsOwnRadiantMonsterEffectChain(ChainInfo chain)
        {
            if (chain == null || chain.ActivatePlayer != 0)
            {
                return false;
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonFonixTheGreatFlame,
                CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                CardId.RadiantTyphoonKrosea, CardId.RadiantTyphoonSwen,
                CardId.RadiantTyphoonDachs, CardId.RadiantTyphoonMeghala,
                CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                return true;
            }

            if (!chain.HasType(CardType.Monster))
            {
                return false;
            }

            if (chain.RelatedCard != null && IsRadiantCard(chain.RelatedCard))
            {
                return true;
            }

            return false;
        }

        private bool SeaSpiritSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                !CanSummonSeaSpiritNow())
            {
                return false;
            }

            if (ShouldPrioritizeKroseaTributeOverSeaSpirit())
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(IsRadiantMonster)
                .Where(CanUseAsLinkMaterial)
                .Where(c => !ShouldWaitForRadiantTrigger(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2)
            {
                return false;
            }

            AI.SelectMaterials(materials);
            _seaSpiritSummoned = true;
            return true;
        }

        private bool SeaSpiritActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 0) ||
                IsMarineEidolonSummonTrigger())
            {
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 1))
            {
                ClientCard enemyTarget = GetMarineEidolonEnemyTarget(Enemy.GetMonsters());
                ClientCard ownTarget = GetMarineEidolonOwnTarget(Bot.GetMonsters());
                if (enemyTarget == null || ownTarget == null)
                {
                    return false;
                }
                return true;
            }

            if (IsMarineEidolonQuickPlayTrigger())
            {
                if (Bot.HasInSpellZone(CardId.RadiantTyphoonMandate))
                {
                    return false;
                }
                _marineQuickPlayTriggerPending = false;
                _marineTrapPlacementPending = true;
                return true;
            }
            return false;
        }

        private bool EcclesiaActivate()
        {
            return IsMainPhase() && Card != null &&
                Card.IsCode(CardId.EcclesiaAndTheDarkDragon) &&
                Card.Location == CardLocation.Grave;
        }

        private bool GaruraActivate()
        {
            return Card != null && Card.IsCode(CardId.GaruraWingsOfResonantLife) &&
                Card.Location == CardLocation.Grave;
        }

        private bool AlbionActivate()
        {
            return Card != null && Card.IsCode(CardId.AlbionTheBrandedDragon) &&
                Card.Location == CardLocation.Grave && Duel.Phase == DuelPhase.End;
        }

        private bool FavoriteHEROFlameWingmanActivate()
        {
            if (IsDisabledOwnFieldMonster() || Card == null || !Card.IsCode(CardId.FavoriteHEROFlameWingman) ||
                Card.Location != CardLocation.MonsterZone || !Card.IsFaceup())
            {
                return false;
            }

            // The attack-announcement effect is only offered when its battle
            // target is face-up with at least 2200 ATK.
            if (IsDescription(CardId.FavoriteHEROFlameWingman, 0))
            {
                return true;
            }

            // Use the Quick Effect as the intended upgrade into Shining Flare
            // Wingman. Flame Wingman itself supplies the Fusion-monster material;
            // preserve Meghala when choosing the other face-up field material.
            if (IsDescription(CardId.FavoriteHEROFlameWingman, 1))
            {
                return !ShouldStopRadiantSpecialSummon(CardLocation.Extra) &&
                    Bot.HasInExtra(CardId.FavoriteHEROShiningFlareWingman) &&
                    Bot.GetMonsters().Any(c => c != Card && c.IsFaceup() &&
                        CanUseAsExtraDeckMaterial(c));
            }
            return false;
        }

        private bool FavoriteHEROShiningFlareWingmanActivate()
        {
            if (IsDisabledOwnFieldMonster() || Card == null || !Card.IsCode(CardId.FavoriteHEROShiningFlareWingman) ||
                Card.Location != CardLocation.MonsterZone || !Card.IsFaceup())
            {
                return false;
            }

            if (IsDescription(CardId.FavoriteHEROShiningFlareWingman, 0))
            {
                return !_enemyDrollResolved;
            }

            // Its battle-destruction damage is mandatory. Normally the server
            // forces it directly, but accept it if several mandatory triggers
            // are being ordered through the executor list.
            return IsDescription(CardId.FavoriteHEROShiningFlareWingman, 1);
        }

        private bool PostExpansionNormalSummon()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsPostExpansionNormalSummonWindow() || Card == null ||
                !IsRadiantCard(Card) || !Card.IsMonster() ||
                Bot.GetMonsters().Any(c => c.IsCode(Card.Id)))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                CardId.RadiantTyphoonFonixTheGreatFlame))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonKrosea) && HasRadiantSpellInHand())
            {
                return false;
            }
            if (Card.IsCode(CardId.RadiantTyphoonKrosea) && !CanUsePreferredKroseaTribute())
            {
                return false;
            }
            return true;
        }

        private bool TotemBirdSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra))
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.Level == 3 &&
                    CanUseAsExtraDeckMaterial(c) &&
                    c.Attribute == (int)CardAttribute.Wind && !ShouldWaitForRadiantTrigger(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool EnterblathnirSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                (Duel.Turn <= 1 && Duel.Phase != DuelPhase.Main2))
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.Level == 9 && c.IsFaceup() &&
                    CanUseAsExtraDeckMaterial(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2 || !HasEnterblathnirFieldOrHandTarget())
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool EnterblathnirActivate()
        {
            return Enemy.Hand.Count > 0 || Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0 ||
                Enemy.Graveyard.Count > 0 || Enemy.Deck.Count > 0;
        }

        private bool HasEnterblathnirFieldOrHandTarget()
        {
            // The summon itself is only worthwhile when the opponent has a
            // visible field card or a hand card. Graveyard and Deck are kept as
            // resolution fallbacks after the Extra Deck monster is established.
            return Enemy.Hand.Count > 0 || Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0;
        }

        private bool SPLittleKnightSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                Util.GetProblematicEnemyCard(0, true) == null)
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    c.HasType(CardType.Effect) && CanUseAsLinkMaterial(c))
                .OrderBy(GetMaterialPriority).ToList();
            ClientCard linkMaterial = materials.FirstOrDefault(c => c.IsCode(
                CardId.WynnTheWindCharmerVerdant,
                CardId.RadiantTyphoonVaruroonTheMarineEidolon));
            ClientCard nonExtraMaterial = materials.FirstOrDefault(c =>
                c != linkMaterial && !c.IsExtraCard());
            if (linkMaterial == null || nonExtraMaterial == null)
            {
                return false;
            }

            AI.SelectMaterials(new List<ClientCard> { linkMaterial, nonExtraMaterial });
            return true;
        }

        private bool HraesvelgrSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra))
            {
                return false;
            }

            List<ClientCard> candidates = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    CanUseAsLinkMaterial(c) &&
                    c.Attribute == (int)CardAttribute.Wind && !ShouldWaitForRadiantTrigger(c) &&
                    !IsValuableDisruptionMonster(c))
                .OrderBy(GetMaterialPriority).ToList();
            for (int i = 0; i < candidates.Count; ++i)
            {
                for (int j = i + 1; j < candidates.Count; ++j)
                {
                    int firstLink = Math.Max(1, candidates[i].LinkCount);
                    int secondLink = Math.Max(1, candidates[j].LinkCount);
                    if (firstLink + secondLink == 3)
                    {
                        AI.SelectMaterials(new List<ClientCard> { candidates[i], candidates[j] });
                        return true;
                    }
                }
            }

            if (candidates.Count >= 3)
            {
                AI.SelectMaterials(candidates.Take(3).ToList());
                return true;
            }
            return false;
        }

        private bool HraesvelgrActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            bool opponentActivatedFromGrave = Duel.CurrentChainInfo.Any(chain =>
                chain.ActivatePlayer == 1 && chain.HasLocation(CardLocation.Grave));
            bool opponentGraveCardTargeted = Duel.CurrentChainInfo.Any(chain =>
                    chain.ActivatePlayer == 1 && chain.Targets.Any(c => c != null &&
                        c.Controller == 1 && c.Location == CardLocation.Grave)) ||
                Duel.ChainTargets.Concat(Duel.LastChainTargets).Any(c => c != null &&
                    c.Controller == 1 && c.Location == CardLocation.Grave);
            if (!opponentActivatedFromGrave && !opponentGraveCardTargeted)
            {
                return false;
            }

            ClientCard target = Enemy.Graveyard.Where(c => c.IsMonster() &&
                    !c.IsShouldNotBeTarget() && !c.IsShouldNotBeMonsterTarget())
                .OrderByDescending(c => c.Attack)
                .ThenByDescending(c => c.Defense)
                .FirstOrDefault();
            if (target == null)
            {
                return false;
            }
            AI.SelectCard(target);
            return true;
        }

        private bool WynnSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                !Enemy.Graveyard.Any(c => c.Attribute == (int)CardAttribute.Wind))
            {
                return false;
            }
            return SelectGenericLinkTwoMaterials(true);
        }

        private bool WynnActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            if (IsDescription(CardId.WynnTheWindCharmerVerdant, 0))
            {
                ClientCard target = Enemy.Graveyard.Where(c => c.IsMonster() &&
                        c.Attribute == (int)CardAttribute.Wind)
                    .OrderByDescending(c => c.Attack).FirstOrDefault();
                if (target == null)
                {
                    return false;
                }
                AI.SelectCard(target);
                return true;
            }
            if (IsDescription(CardId.WynnTheWindCharmerVerdant, 1))
            {
                return !_enemyDrollResolved;
            }
            return false;
        }

        private bool GreatflySummon()
        {
            if (Duel.IsFirst || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                !IsMainPhase() || Duel.Phase != DuelPhase.Main1 ||
                Enemy.GetMonsterCount() > 0)
            {
                return false;
            }

            List<ClientCard> materials = GetGenericLinkTwoMaterials(true,
                CardId.RadiantTyphoonFonixTheGreatFlame);
            if (materials.Count < 2)
            {
                return false;
            }

            List<ClientCard> remainingAttackers = Bot.GetMonsters().Where(c => c.IsAttack() &&
                !materials.Contains(c)).ToList();
            int postSummonAttack = remainingAttackers.Sum(c => Math.Max(0, c.Attack));
            postSummonAttack += remainingAttackers.Count(c =>
                c.Attribute == (int)CardAttribute.Wind) * 500;

            ClientCard greatfly = Bot.ExtraDeck.FirstOrDefault(c => c.IsCode(CardId.Greatfly));
            int greatflyBaseAttack = greatfly == null || greatfly.Attack <= 0 ? 1400 : greatfly.Attack;
            postSummonAttack += greatflyBaseAttack + 500;
            if (postSummonAttack < Enemy.LifePoints)
            {
                return false;
            }

            AI.SelectMaterials(materials);
            return true;
        }

        private bool GreatflyPrioritySummon()
        {
            if (Duel.Turn <= 1 || ShouldStopRadiantSpecialSummon(CardLocation.Extra) ||
                !IsMainPhase() || Bot.GetMonsterCount() < 5)
            {
                return false;
            }
            return SelectGenericLinkTwoMaterials(true, CardId.RadiantTyphoonFonixTheGreatFlame);
        }

        private bool GreatflyActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            ClientCard target = Bot.Graveyard.Where(c => c.IsMonster() &&
                    c.Attribute == (int)CardAttribute.Wind)
                .OrderByDescending(c => c.Attack).FirstOrDefault();
            if (target == null)
            {
                return false;
            }
            AI.SelectCard(target);
            return true;
        }

        private List<ClientCard> GetGenericLinkTwoMaterials(bool requireWind, int excludedCardId = 0)
        {
            return Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    CanUseAsLinkMaterial(c) &&
                    (!requireWind || c.Attribute == (int)CardAttribute.Wind) &&
                    (excludedCardId == 0 || !c.IsCode(excludedCardId)) &&
                    !ShouldWaitForRadiantTrigger(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
        }

        private bool SelectGenericLinkTwoMaterials(bool requireWind, int excludedCardId = 0)
        {
            List<ClientCard> materials = GetGenericLinkTwoMaterials(requireWind, excludedCardId);
            if (materials.Count < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool TyphonSummon()
        {
            if (!IsMainPhase() || ShouldStopRadiantSpecialSummon(CardLocation.Extra))
            {
                return false;
            }
            bool emergency = Enemy.GetMonsters().Any(c => c != null && c.IsFaceup() &&
                c.IsExtraCard() && c.Attack >= 3000);
            return emergency && (Duel.Phase == DuelPhase.Main2 || !CanContinueRadiantEngine());
        }

        private bool TyphonActivate()
        {
            if (IsDisabledOwnFieldMonster())
            {
                return false;
            }

            ClientCard target = Util.GetProblematicEnemyMonster(0, false) ?? Util.GetBestEnemyMonster(false, false);
            if (target == null)
            {
                return false;
            }
            // TY-PHON's return effect is non-targeting. Do not queue a card at
            // activation time: the server supplies the legal return candidates
            // only when the effect resolves. SelectResolutionCard will then
            // restrict that candidate list to opponent monsters.
            // The server asks for TY-PHON's single overlay material separately.
            AI.SelectCard(Card.Overlays);
            return true;
        }

        private bool CanContinueRadiantEngine()
        {
            return Bot.Hand.Any(IsRadiantCard) || Bot.GetMonsters().Any(c => IsRadiantMonster(c) && !ShouldWaitForRadiantTrigger(c));
        }

        private bool RadiantSpellSet()
        {
            if (ShouldRequireMain1MonsterSummon())
            {
                return false;
            }

            if (!IsRadiantSetPriorityCard(Card))
            {
                return DefaultSpellSet() && IsPreferredRadiantSpellSetCandidate(Card);
            }

            if (Duel.Turn > 1 && Duel.Phase != DuelPhase.Main2)
            {
                return false;
            }

            return IsPreferredRadiantSpellSetCandidate(Card);
        }

        private bool RadiantMonsterRepos()
        {
            return !ShouldRequireMain1MonsterSummon() && DefaultMonsterRepos();
        }

        private bool IsRadiantSetPriorityCard(ClientCard card)
        {
            return card != null && card.IsCode(CardId.MysticalSpaceTyphoon,
                CardId.RadiantTyphoonVision, CardId.RadiantTyphoonAscendance,
                CardId.RadiantTyphoonChant, CardId.RadiantTyphoonMandate,
                CardId.ForbiddenDroplet, CardId.SuperPolymerization,
                CardId.TheFallenTheVirtuous, CardId.InfiniteImpermanence);
        }

        private bool IsPreferredRadiantSpellSetCandidate(ClientCard card)
        {
            if (card == null || Duel.MainPhase == null ||
                Duel.MainPhase.SpellSetableCards == null)
            {
                return true;
            }

            int currentPriority = GetRadiantSpellSetPriority(card);
            foreach (ClientCard candidate in Duel.MainPhase.SpellSetableCards)
            {
                if (candidate == null || candidate == card ||
                    (IsRadiantSetPriorityCard(candidate) && Duel.Turn > 1 &&
                        Duel.Phase != DuelPhase.Main2))
                {
                    continue;
                }

                if (GetRadiantSpellSetPriority(candidate) < currentPriority)
                {
                    return false;
                }
            }
            return true;
        }

        private int GetRadiantSpellSetPriority(ClientCard card)
        {
            if (card == null)
            {
                return 1000;
            }

            int priority;
            if (ShouldPrioritizeRadiantQuickPlaySet() &&
                card.IsCode(CardId.RadiantTyphoonAscendance,
                    CardId.RadiantTyphoonVision, CardId.RadiantTyphoonChant))
            {
                priority = card.IsCode(CardId.RadiantTyphoonAscendance) ? 0 :
                    card.IsCode(CardId.RadiantTyphoonVision) ? 1 : 2;
            }
            else if (card.IsCode(CardId.ForbiddenDroplet))
            {
                priority = 0;
            }
            else if (card.IsCode(CardId.SuperPolymerization))
            {
                priority = 1;
            }
            else if (card.IsCode(CardId.TheFallenTheVirtuous))
            {
                priority = 2;
            }
            else if (card.IsCode(CardId.RadiantTyphoonMandate))
            {
                priority = 3;
            }
            else if (card.IsCode(CardId.MysticalSpaceTyphoon))
            {
                priority = 4;
            }
            else if (card.IsCode(CardId.RadiantTyphoonAscendance))
            {
                priority = 5;
            }
            else if (card.IsCode(CardId.RadiantTyphoonVision))
            {
                priority = 6;
            }
            else if (card.IsCode(CardId.RadiantTyphoonChant))
            {
                priority = 7;
            }
            else
            {
                priority = 8;
            }

            // MST is deliberately allowed to be set repeatedly. Every other
            // same-name facedown card is moved behind all non-duplicate
            // candidates, including the generic "other" category.
            if (!card.IsCode(CardId.MysticalSpaceTyphoon) &&
                HasFaceDownSpellWithSameName(card))
            {
                priority += 100;
            }
            return priority;
        }

        private bool HasFaceDownSpellWithSameName(ClientCard card)
        {
            return card != null && Bot.GetSpells().Any(c => c != null &&
                c.IsFacedown() && c.IsCode(card.Id));
        }

        private bool ShouldPrioritizeRadiantQuickPlaySet()
        {
            return HasFaceupMandate() && Bot.GetSpellCountWithoutField() == 4 &&
                !Bot.GetSpells().Any(c => c != null && c.IsFacedown() &&
                    c.IsCode(CardId.RadiantTyphoonAscendance,
                        CardId.RadiantTyphoonVision, CardId.RadiantTyphoonChant));
        }

        public override int OnSelectOption(IList<int> options)
        {
            int selected;

            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null && solvingChain.IsActivateCode(CardId.AlbionTheBrandedDragon))
            {
                // 1153 = Set, 1190 = Add to hand. Preserve the interruption for
                // the opponent's turn whenever the server offers a legal Set.
                selected = options.IndexOf(1153);
                if (selected >= 0)
                {
                    return selected;
                }
                selected = options.IndexOf(1190);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.TheFallenTheVirtuous, 1, 2))
            {
                // Deck-specific policy: use the destruction branch to start the
                // Albion/Ecclesia resource loop; the shared name once-per-turn
                // makes the revival branch substantially less valuable here.
                selected = GetOptionIndex(options, CardId.TheFallenTheVirtuous, 1);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonVision, 2, 3))
            {
                int preferredOffset = GetPreferredVisionEffectOffset();
                selected = preferredOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonVision, preferredOffset) : -1;
                if (selected >= 0)
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonVision,
                        preferredOffset);
                    return selected;
                }
                int fallbackOffset = preferredOffset == 2 ? 3 :
                    preferredOffset == 3 ? 2 : -1;
                selected = fallbackOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonVision, fallbackOffset) : -1;
                if (selected >= 0 && IsUnusedRadiantQuickPlayEffect(
                    CardId.RadiantTyphoonVision, fallbackOffset))
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonVision,
                        fallbackOffset);
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonChant, 2, 3))
            {
                // cards.cdb stores Chant's monster search at offset 2 and its
                // Mystical Space Typhoon search at offset 3.
                int preferredOffset = GetPreferredChantEffectOffset();
                selected = preferredOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonChant, preferredOffset) : -1;
                if (selected >= 0)
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonChant,
                        preferredOffset);
                    return selected;
                }
                int fallbackOffset = preferredOffset == 2 ? 3 :
                    preferredOffset == 3 ? 2 : -1;
                selected = fallbackOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonChant, fallbackOffset) : -1;
                if (selected >= 0 && IsUnusedRadiantQuickPlayEffect(CardId.RadiantTyphoonChant,
                    fallbackOffset))
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonChant,
                        fallbackOffset);
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonAscendance, 2, 3))
            {
                int preferredOffset = GetPreferredAscendanceEffectOffset();
                selected = preferredOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonAscendance, preferredOffset) : -1;
                if (selected >= 0)
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonAscendance,
                        preferredOffset);
                    return selected;
                }
                int fallbackOffset = preferredOffset == 2 ? 3 :
                    preferredOffset == 3 ? 2 : -1;
                selected = fallbackOffset >= 0
                    ? GetOptionIndex(options, CardId.RadiantTyphoonAscendance, fallbackOffset) : -1;
                if (selected >= 0 && IsUnusedRadiantQuickPlayEffect(
                    CardId.RadiantTyphoonAscendance, fallbackOffset))
                {
                    RegisterRadiantQuickPlayEffectSelection(CardId.RadiantTyphoonAscendance,
                        fallbackOffset);
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.PhantomFortressEnterblathnir, 1, 2, 3, 4))
            {
                if (Duel.Turn <= 1 && Enemy.Hand.Count > 0)
                {
                    selected = GetOptionIndex(options, CardId.PhantomFortressEnterblathnir, 2);
                    if (selected >= 0)
                    {
                        return selected;
                    }
                }
                if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                {
                    selected = GetOptionIndex(options, CardId.PhantomFortressEnterblathnir, 1);
                    if (selected >= 0)
                    {
                        return selected;
                    }
                }
                selected = GetOptionIndex(options, CardId.PhantomFortressEnterblathnir, 3);
                if (selected >= 0)
                {
                    return selected;
                }
                selected = GetOptionIndex(options, CardId.PhantomFortressEnterblathnir, 4);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.FavoriteHEROFlameWingman, 2))
            {
                return CanSummonFromHandAfterPurulia() && Bot.GetMonsterCount() < 5 &&
                    Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala,
                        CardId.RadiantTyphoonSwen, CardId.RadiantTyphoonDachs));
            }
            if (desc == Util.GetStringId(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 3))
            {
                ChainInfo enemyMonsterEffect = Duel.CurrentChainInfo.LastOrDefault(info =>
                    info.ActivatePlayer == 1 && (info.ActivateType & (int)CardType.Monster) != 0);
                return enemyMonsterEffect != null && enemyMonsterEffect.RelatedCard != null &&
                    enemyMonsterEffect.RelatedCard.IsOnField();
            }
            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == CardId.RadiantTyphoonVaruroonTheVibrantVortex &&
                positions.Contains(CardPosition.FaceUpDefence))
            {
                return CardPosition.FaceUpDefence;
            }
            if (cardId == CardId.RadiantTyphoonMeghala && Duel.Turn <= 1 &&
                positions.Contains(CardPosition.FaceUpDefence))
            {
                return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public override IList<ClientCard> OnSelectFusionMaterial(IList<ClientCard> cards, int min, int max)
        {
            if (_favoriteHEROFusionTargetId == CardId.FavoriteHEROShiningFlareWingman)
            {
                List<ClientCard> priority = cards.Where(CanUseAsExtraDeckMaterial)
                    .OrderBy(c => c.IsCode(CardId.FavoriteHEROFlameWingman) ? 0 :
                        c.HasType(CardType.Fusion) ? 1 : 2)
                    .ThenBy(GetMaterialPriority).ToList();
                return SelectCount(priority, cards, min, max, min);
            }
            return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);
        }

        public override IList<ClientCard> OnSelectSynchroMaterial(IList<ClientCard> cards,
            IList<ClientCard> mandatoryCards, int sum, int min, int max)
        {
            if (sum == 0)
                return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);

            List<ClientCard> materials = cards.Where(CanUseAsExtraDeckMaterial)
                .OrderBy(GetMaterialPriority).ToList();
            return AI.FindSumSelection(materials, mandatoryCards, sum, min, max, true); // null on failure, use default at that case
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);
        }

        public override IList<ClientCard> OnSelectLinkMaterial(IList<ClientCard> cards, int min, int max)
        {
            return SelectLinkMaterials(cards, min, max);
        }

        public override IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max,
            int hint, bool cancelable)
        {
            if (!_selectingGallantThiefTributes)
            {
                return null;
            }

            _selectingGallantThiefTributes = false;
            List<ClientCard> enemyTributes = cards.Where(c => c.Controller == 1)
                .OrderByDescending(IsSuitableGallantThiefTribute)
                .ThenByDescending(c => c.IsFacedown() ? 2500 : c.Attack).ToList();
            if (enemyTributes.Count < 2 && enemyTributes.Count(IsSuitableGallantThiefTribute) == 0 && cancelable)
            {
                Logger.DebugWriteLine("No suitable Gallant Thief tribute candidates; skipping summon this turn.");
                _skipGallantThiefSummonThisTurn = true;
                return new List<ClientCard>();
            }

            if (enemyTributes.Count >= max)
            {
                return enemyTributes.Take(max).ToList();
            }

            List<ClientCard> ownTributes = cards.Where(c => c.Controller == 0)
                .OrderByDescending(c => c.IsCode(CardId.TheWorldsGreatestGallantThief))
                .ThenBy(c => c.Attack).ToList();
            enemyTributes.AddRange(ownTributes);

            IList<ClientCard> selected = AI.FindTributeSelection(enemyTributes, min, max, false);
            if (selected != null)
            {
                if (selected.All(c => c.Controller == 0))
                    Logger.WriteErrorLine("Failed to select enemy monsters for Gallant Thief tribute, and can't cancel summon, please check.");
                return selected;
            }

            // something went wrong
            Logger.WriteErrorLine("Failed to select ANY monster for Gallant Thief tribute, please check.");
            if (cancelable)
            {
                _skipGallantThiefSummonThisTurn = true;
                return new List<ClientCard>();
            }
            return null;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo buildingChain = GetBuildingChainInfo();
            if (buildingChain != null && buildingChain.ActivatePlayer == 0)
            {
                IList<ClientCard> activationSelection = SelectActivationCard(buildingChain, cards, min, max, hint);
                if (activationSelection != null)
                {
                    return activationSelection;
                }
            }

            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();
            if (solvingChain != null && solvingChain.ActivatePlayer == 0)
            {
                IList<ClientCard> resolutionSelection = SelectResolutionCard(solvingChain, cards, min, max, hint);
                if (resolutionSelection != null)
                {
                    return resolutionSelection;
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        private IList<ClientCard> SelectActivationCard(ChainInfo chain, IList<ClientCard> cards, int min, int max, int hint)
        {
            if (chain.IsActivateCode(CardId.ForbiddenDroplet) &&
                (hint == HintMsg.ToGrave || hint == HintMsg.Discard))
            {
                ClientCard problem = Util.GetProblematicEnemyMonster(0, false);
                List<ClientCard> enemyTargets = GetDropletTargetMonsters();
                int targetCount = enemyTargets.Count(c => c == problem || c.Attack >= 2500);
                bool twoCardBreakthrough = CanUseTwoCardDropletBreakthrough();
                int desired = twoCardBreakthrough ? 2 :
                    Math.Min(max, Math.Max(min, Math.Min(2, Math.Max(1, targetCount))));
                List<ClientCard> chainCosts = cards.Where(IsCurrentDropletChainCard)
                    .OrderBy(GetDropletCostPriority).ToList();
                List<ClientCard> costs = twoCardBreakthrough ? chainCosts
                    .Concat(cards.Where(IsDropletCostCandidate)
                        .Where(c => !chainCosts.Contains(c))
                        .OrderBy(GetDropletCostPriority))
                    .ToList() : cards.Where(IsDropletCostCandidate)
                        .OrderBy(GetDropletCostPriority).ToList();
                // If a chained card was negated and left the field before the
                // selection request, it is no longer a legal cost. Prefer the
                // remaining eligible cards instead of retaining a stale object.
                if (twoCardBreakthrough && chainCosts.Count < 2)
                {
                    desired = Math.Min(max, Math.Max(min, 2));
                }
                desired = Math.Min(desired, costs.Count);
                if (desired < min)
                {
                    return null;
                }
                IList<ClientCard> selected = SelectCount(costs, cards, min, max, desired);
                _dropletCostCount = selected == null ? 0 : selected.Count;
                return selected;
            }

            if (chain.IsActivateCode(CardId.TheFallenTheVirtuous))
            {
                if (hint == HintMsg.ToGrave)
                {
                    return SelectByIds(cards, min, max, 1,
                        CardId.AlbionTheBrandedDragon,
                        CardId.EcclesiaAndTheDarkDragon);
                }
                if (hint == HintMsg.Destroy || hint == HintMsg.Target)
                {
                    if (_fallenDodgeTarget != null)
                    {
                        ClientCard dodgeTarget = cards.FirstOrDefault(c => c == _fallenDodgeTarget);
                        if (dodgeTarget == null)
                        {
                            dodgeTarget = cards.FirstOrDefault(c => c.Controller == _fallenDodgeTarget.Controller &&
                                c.Location == _fallenDodgeTarget.Location &&
                                c.Sequence == _fallenDodgeTarget.Sequence);
                        }
                        if (dodgeTarget != null)
                        {
                            return new List<ClientCard> { dodgeTarget };
                        }
                    }
                    List<ClientCard> targets = GetOrderedFallenTargets(cards.Where(c => c.Controller == 1));
                    return SelectCount(targets, cards, min, max, 1);
                }
            }

            if (chain.IsActivateCode(CardId.EcclesiaAndTheDarkDragon) &&
                (hint == HintMsg.Target || hint == HintMsg.ToDeck))
            {
                // The script selects the Level 8 Fusion first, then a card on
                // the field. The server supplies the legal candidates for each
                // request, so apply the priority only within that request.
                List<ClientCard> fusionTargets = cards.Where(IsEcclesiaFusionTarget)
                    .OrderByDescending(c => Math.Max(c.Attack, c.Defense)).ToList();
                if (fusionTargets.Count > 0)
                {
                    return SelectCount(fusionTargets, cards, min, max, 1);
                }

                List<ClientCard> fieldTargets = GetOrderedEcclesiaFieldTargets(cards);
                return SelectCount(fieldTargets, cards, min, max, 1);
            }

            if (chain.IsActivateCode(CardId.MysticalSpaceTyphoon))
            {
                ClientCard storedTarget = FindMatchingCard(cards, _mysticalSpaceTyphoonTarget);
                if (storedTarget != null)
                {
                    return new List<ClientCard> { storedTarget };
                }
                ClientCard preferred = GetMysticalSpaceTyphoonTarget();
                if (preferred != null && cards.Contains(preferred))
                {
                    return new List<ClientCard> { preferred };
                }
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonFonixTheGreatFlame))
            {
                List<ClientCard> targets = GetOrderedEnemyCards(cards.Where(c => c.Controller == 1));
                return SelectCount(targets, cards, min, max, Math.Min(2, max));
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonMandate))
            {
                if (chain.ActivateDescription == Util.GetStringId(CardId.RadiantTyphoonMandate, 0))
                {
                    return SelectMandateRecycle(cards, min, max);
                }
                ClientCard mandateTarget = FindMatchingCard(cards, _mandateNegationTarget);
                if (mandateTarget == null)
                {
                    mandateTarget = FindMatchingCard(cards, GetBestMandateNegationTarget());
                }
                if (mandateTarget != null)
                {
                    _mandateNegationTarget = mandateTarget;
                    return new List<ClientCard> { mandateTarget };
                }
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) &&
                chain.ActivateDescription == Util.GetStringId(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 1))
            {
                return SelectMarineEidolonTargets(chain, cards, min, max);
            }

            return null;
        }

        private IList<ClientCard> SelectMarineEidolonTargets(ChainInfo chain,
            IList<ClientCard> cards, int min, int max)
        {
            ClientCard enemyTarget = GetMarineEidolonEnemyTarget(cards);
            ClientCard ownTarget = GetMarineEidolonOwnTarget(cards);
            bool enemyTargetAlreadySelected = chain != null && chain.Targets.Any(c =>
                c != null && c.Controller == 1 && c.IsMonster() && c.IsFaceup() &&
                c.Attack >= 2000);

            // The card script can request the two targets separately. Select the
            // opponent target first, then the face-up Radiant Typhoon target in
            // the next request. If both are in one candidate list, preserve the
            // same order in a single response.
            if (!enemyTargetAlreadySelected && enemyTarget != null)
            {
                if (ownTarget != null && min <= 2 && max >= 2)
                {
                    return new List<ClientCard> { enemyTarget, ownTarget };
                }
                return SelectCount(new List<ClientCard> { enemyTarget }, cards, min, max, 1);
            }

            if (ownTarget != null)
            {
                return SelectCount(new List<ClientCard> { ownTarget }, cards, min, max, 1);
            }
            return null;
        }

        private ClientCard GetMarineEidolonOwnTarget(IEnumerable<ClientCard> source)
        {
            return source.Where(c => c != null && c.Controller == 0 && IsRadiantMonster(c))
                .Where(c => !c.IsShouldNotBeTarget())
                .OrderBy(GetMarineEidolonOwnTargetPriority)
                .ThenBy(GetMaterialPriority)
                .FirstOrDefault();
        }

        private ClientCard GetMarineEidolonEnemyTarget(IEnumerable<ClientCard> source)
        {
            return source.Where(c => c != null && c.Controller == 1 && c.IsMonster() && c.IsFaceup() &&
                    c.Attack >= 2000 && !c.IsShouldNotBeTarget())
                .OrderByDescending(c => c.Attack)
                .ThenByDescending(c => c.Defense)
                .FirstOrDefault();
        }

        private int GetMarineEidolonOwnTargetPriority(ClientCard card)
        {
            if (card == null)
            {
                return 100;
            }
            if (card.IsCode(CardId.RadiantTyphoonDachs))
            {
                return 0;
            }
            if (card.IsCode(CardId.RadiantTyphoonSwen))
            {
                return 1;
            }
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                return 2;
            }
            if (card.IsCode(CardId.RadiantTyphoonKrosea))
            {
                return 3;
            }
            if (card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return 4;
            }
            return 5;
        }

        private bool IsOpponentMonsterEffectChain(ChainInfo chain)
        {
            return chain != null && chain.ActivatePlayer == 1 &&
                (chain.ActivateType & (int)CardType.Monster) != 0 &&
                chain.HasLocation(CardLocation.MonsterZone);
        }

        private ClientCard GetChainSourceCard(ChainInfo chain)
        {
            if (!IsOpponentMonsterEffectChain(chain))
            {
                return null;
            }

            ClientCard source = Enemy.GetMonsters().FirstOrDefault(c => c != null &&
                c.Location == chain.ActivateLocation && c.Sequence == chain.ActivateSequence);
            if (source != null)
            {
                return source;
            }
            return Enemy.GetMonsters().FirstOrDefault(c => c == chain.RelatedCard);
        }

        private ClientCard GetResolvingOpponentMonsterEffectSource()
        {
            int end = Duel.CurrentChainInfo.Count;
            if (Duel.SolvingChainIndex > 0)
            {
                end = Math.Min(end, Duel.SolvingChainIndex - 1);
            }

            for (int i = end - 1; i >= 0; --i)
            {
                ClientCard source = GetChainSourceCard(Duel.CurrentChainInfo[i]);
                if (source != null)
                {
                    return source;
                }
            }
            return null;
        }

        private ClientCard FindMatchingCard(IList<ClientCard> cards, ClientCard source)
        {
            if (source == null)
            {
                return null;
            }
            ClientCard exact = cards.FirstOrDefault(c => c == source);
            if (exact != null)
            {
                return exact;
            }
            return cards.FirstOrDefault(c => c != null && c.Controller == source.Controller &&
                c.Location == source.Location && c.Sequence == source.Sequence);
        }

        private IList<ClientCard> SelectResolutionCard(ChainInfo chain, IList<ClientCard> cards, int min, int max, int hint)
        {
            if (chain.IsActivateCode(CardId.SuperStarslayerTYPHONSkyCrisis))
            {
                // TY-PHON returns a monster without targeting. The candidate
                // list can therefore contain monsters from both fields. Never
                // let the generic first-card fallback choose one of our own
                // monsters while an opponent monster is available.
                List<ClientCard> enemyCandidates = cards.Where(c => c != null &&
                    c.Controller == 1 && (c.IsMonster() ||
                        c.Location == CardLocation.MonsterZone)).ToList();
                if (enemyCandidates.Count == 0)
                {
                    // The opponent monster used to justify activation may have
                    // left the field while the chain was resolving. At that
                    // point the server may leave only our monsters as legal
                    // candidates; do not manufacture an invalid selection.
                    return null;
                }

                List<ClientCard> priority = new List<ClientCard>();
                ClientCard preferred = FindMatchingCard(enemyCandidates,
                    Util.GetProblematicEnemyMonster(0, false));
                if (preferred == null)
                {
                    preferred = FindMatchingCard(enemyCandidates,
                        Util.GetBestEnemyMonster(false, false));
                }
                if (preferred != null)
                {
                    priority.Add(preferred);
                }

                priority.AddRange(enemyCandidates.Where(c => !priority.Contains(c))
                    .OrderByDescending(c => c.IsFaceup())
                    .ThenByDescending(c => c.Attack)
                    .ThenByDescending(c => c.Defense));
                return SelectCount(priority, cards, min, max, 1);
            }

            if (chain.IsActivateCode(CardId.FavoriteHEROShiningFlareWingman))
            {
                List<ClientCard> priority = cards.OrderBy(GetShiningFlareRecyclePriority)
                    .ThenBy(GetMaterialPriority).ToList();
                return SelectCount(priority, cards, min, max, 5);
            }

            if (chain.IsActivateCode(CardId.FavoriteHEROFlameWingman))
            {
                ClientCard shiningFlareWingman = cards.FirstOrDefault(c =>
                    c.IsCode(CardId.FavoriteHEROShiningFlareWingman));
                if (shiningFlareWingman != null)
                {
                    _favoriteHEROFusionTargetId = CardId.FavoriteHEROShiningFlareWingman;
                    return SelectCount(new List<ClientCard> { shiningFlareWingman },
                        cards, min, max, 1);
                }

                if (cards.Any(c => c.Location == CardLocation.Hand && c.IsMonster()))
                {
                    return SelectFavoriteHEROFlameWingmanHandSummon(cards, min, max);
                }
            }

            if (chain.IsActivateCode(CardId.AlbionTheBrandedDragon))
            {
                return SelectByIds(cards, min, max, 1, CardId.TheFallenTheVirtuous);
            }

            if (chain.IsActivateCode(CardId.ForbiddenDroplet))
            {
                int desired = _dropletCostCount > 0 ? _dropletCostCount : min;
                List<ClientCard> targets = new List<ClientCard>();
                ClientCard respondingMonster = GetResolvingOpponentMonsterEffectSource();
                ClientCard respondingMonsterCandidate = FindMatchingCard(cards, respondingMonster);
                if (respondingMonsterCandidate != null)
                {
                    targets.Add(respondingMonsterCandidate);
                }
                targets.AddRange(GetOrderedDropletTargets(cards.Where(c => c.Controller == 1 &&
                    !targets.Contains(c))));
                return SelectCount(targets, cards, min, max, desired);
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonMeghala))
            {
                return SelectMeghalaSummon(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonDachs))
            {
                return SelectDachsSearch(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonSwen))
            {
                return SelectSwenSearch(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonKrosea))
            {
                return SelectKroseaSearch(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonChant))
            {
                return SelectChantSearch(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonAscendance))
            {
                return SelectAscendance(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonVision) &&
                IsRadiantResolutionEffect(CardId.RadiantTyphoonVision, 3))
            {
                return SelectMstForHand(cards, min, max);
            }
            bool isVisionHandDiscard = min == 1 && max == 1 && cards.Count > 0 &&
                cards.All(c => c != null && c.Controller == 0 && c.Location == CardLocation.Hand);
            if (chain.IsActivateCode(CardId.RadiantTyphoonVision) && isVisionHandDiscard &&
                (hint == HintMsg.Discard || hint == HintMsg.ToGrave || hint == 0))
            {
                return SelectVisionDiscard(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonVision) &&
                cards.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return SelectByIds(cards, min, max, 1,
                    BuildSearchPriorityWithoutFieldMeghala(CardId.MysticalSpaceTyphoon).ToArray());
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonMandate) && hint == HintMsg.ToDeck)
            {
                return SelectMandateRecycle(cards, min, max);
            }
            if (chain.IsActivateCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.MysticalSpaceTyphoon,
                    CardId.RadiantTyphoonMandate);
                return SelectByIds(cards, min, max, 1, priority.ToArray());
            }
            if (chain.IsActivateCode(CardId.WynnTheWindCharmerVerdant) && hint == HintMsg.AddToHand)
            {
                List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonKrosea);
                return SelectByIds(cards, min, max, 1, priority.ToArray());
            }
            if (chain.IsActivateCode(CardId.PhantomFortressEnterblathnir))
            {
                List<ClientCard> targets = GetOrderedEnemyCards(cards.Where(c => c.Controller == 1));
                if (targets.Count == 0)
                {
                    targets = cards.ToList();
                }
                return SelectCount(targets, cards, min, max, 1);
            }
            return null;
        }

        private IList<ClientCard> SelectFavoriteHEROFlameWingmanHandSummon(IList<ClientCard> cards,
            int min, int max)
        {
            List<int> ids = BuildSearchPriorityWithoutFieldMeghala(
                CardId.RadiantTyphoonMeghala,
                CardId.RadiantTyphoonSwen,
                CardId.RadiantTyphoonDachs,
                CardId.AshBlossomJoyousSpring,
                CardId.MaxxC,
                CardId.DrollLockBird);
            List<ClientCard> priority = new List<ClientCard>();
            foreach (int id in ids)
            {
                priority.AddRange(cards.Where(c => c.IsCode(id) && !priority.Contains(c)));
            }
            priority.AddRange(cards.Where(c => !priority.Contains(c)).OrderBy(GetDiscardPriority));
            return SelectCount(priority, cards, min, max, 1);
        }

        private int GetShiningFlareRecyclePriority(ClientCard card)
        {
            if (card == null)
            {
                return 1000;
            }
            if (card.IsCode(CardId.AlbionTheBrandedDragon, CardId.EcclesiaAndTheDarkDragon))
            {
                return 100;
            }
            if (card.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame,
                CardId.RadiantTyphoonVaruroonTheVibrantVortex))
            {
                return 90;
            }
            if (card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
            {
                return 0;
            }
            if (IsRadiantCard(card) &&
                (card.IsCode(CardId.RadiantTyphoonMeghala)
                    ? WasRadiantFieldEffectUsedThisTurn(card.Id)
                    : WasRadiantEffectUsedThisTurn(card.Id)))
            {
                return 10;
            }
            if (IsRadiantCard(card))
            {
                return 70;
            }
            return 30;
        }

        private IList<ClientCard> SelectMeghalaSummon(IList<ClientCard> cards, int min, int max)
        {
            List<int> priority;
            if (Duel.Player == 0)
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex);
            }
            else
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs);
            }
            return SelectByIds(cards, min, max, 1, priority.ToArray());
        }

        private IList<ClientCard> SelectDachsSearch(IList<ClientCard> cards, int min, int max)
        {
            if (NeedMstStarter() && cards.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return SelectMstForHand(cards, min, max);
            }

            List<int> priority;
            if (HasRadiantQuickPlayInHand())
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.MysticalSpaceTyphoon);
            }
            else
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.MysticalSpaceTyphoon);
            }
            return SelectByIds(cards, min, max, 1, priority.ToArray());
        }

        private IList<ClientCard> SelectSwenSearch(IList<ClientCard> cards, int min, int max)
        {
            if (NeedMstStarter() && cards.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return SelectMstForHand(cards, min, max);
            }

            List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
                CardId.RadiantTyphoonChant,
                CardId.RadiantTyphoonVision,
                CardId.RadiantTyphoonAscendance,
                CardId.MysticalSpaceTyphoon,
                CardId.RadiantTyphoonMandate);
            return SelectByIds(cards, min, max, 1, priority.ToArray());
        }

        private IList<ClientCard> SelectKroseaSearch(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> result = new List<ClientCard>();
            if (NeedMstStarter())
            {
                ClientCard priorityMst = GetPreferredMst(cards, true);
                if (priorityMst != null)
                {
                    result.Add(priorityMst);
                }
            }

            List<int> priority;
            if (ShouldPreferKroseaAscendanceSearch(cards))
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonAscendance,
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonVision,
                    CardId.RadiantTyphoonChant,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.RadiantTyphoonMandate,
                    CardId.RadiantTyphoonKrosea,
                    CardId.MysticalSpaceTyphoon);
            }
            else
            {
                priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonMeghala,
                    CardId.RadiantTyphoonVision,
                    CardId.RadiantTyphoonChant,
                    CardId.RadiantTyphoonAscendance,
                    CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                    CardId.RadiantTyphoonFonixTheGreatFlame,
                    CardId.RadiantTyphoonMandate,
                    CardId.RadiantTyphoonKrosea,
                    CardId.MysticalSpaceTyphoon);
            }

            if (result.Count < max)
            {
                IList<ClientCard> radiantSelection = SelectByIds(cards, 0, 1, 1, priority.ToArray());
                if (radiantSelection != null)
                {
                    result.AddRange(radiantSelection.Where(c => IsRadiantCard(c) &&
                        !c.IsCode(CardId.RadiantTyphoonKrosea) && !result.Contains(c)));
                }
            }

            ClientCard mst = GetPreferredMst(cards, result.Count < min);
            if (mst != null && result.Count < max && !result.Contains(mst))
            {
                result.Add(mst);
            }
            return SelectCount(result, cards, min, max, Math.Min(max, Math.Max(min, result.Count)));
        }

        private IList<ClientCard> SelectChantSearch(IList<ClientCard> cards, int min, int max)
        {
            if (IsRadiantResolutionEffect(CardId.RadiantTyphoonChant, 3))
            {
                return SelectMstForHand(cards, min, max);
            }
            if (!IsRadiantResolutionEffect(CardId.RadiantTyphoonChant, 2) &&
                (ShouldUseChantMstSearch() || cards.All(c => c.IsCode(CardId.MysticalSpaceTyphoon))) &&
                cards.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return SelectMstForHand(cards, min, max);
            }

            bool hasRadiantMonsterInHand = Bot.Hand.Any(c => c != null &&
                IsRadiantCard(c) && c.IsMonster());
            bool hasOtherRadiantQuickPlayInHand = Bot.Hand.Any(IsRadiantQuickPlay);
            if (Bot.GetMonsterCount() == 0 && !hasRadiantMonsterInHand)
            {
                List<int> emptyFieldPriority;
                if (hasOtherRadiantQuickPlayInHand)
                {
                    emptyFieldPriority = BuildSearchPriorityWithoutFieldMeghala(
                        CardId.RadiantTyphoonDachs,
                        CardId.RadiantTyphoonMeghala,
                        CardId.RadiantTyphoonSwen,
                        CardId.MysticalSpaceTyphoon);
                }
                else
                {
                    emptyFieldPriority = BuildSearchPriorityWithoutFieldMeghala(
                        CardId.RadiantTyphoonDachs,
                        CardId.RadiantTyphoonSwen,
                        CardId.RadiantTyphoonMeghala,
                        CardId.MysticalSpaceTyphoon);
                }
                return SelectByIds(cards, min, max, 1, emptyFieldPriority.ToArray());
            }

            List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
                CardId.RadiantTyphoonSwen,
                CardId.RadiantTyphoonDachs,
                CardId.RadiantTyphoonMeghala,
                CardId.MysticalSpaceTyphoon);
            return SelectByIds(cards, min, max, 1, priority.ToArray());
        }

        private IList<ClientCard> SelectAscendance(IList<ClientCard> cards, int min, int max)
        {
            if (IsRadiantResolutionEffect(CardId.RadiantTyphoonAscendance, 3))
            {
                return SelectMstForHand(cards, min, max);
            }

            List<ClientCard> reviveTargets = cards.Where(c => c != null && IsRadiantCard(c) &&
                    c.IsMonster() && c.Level <= 6 &&
                    (Duel.Player != 0 || (!(c.IsCode(CardId.RadiantTyphoonMeghala)
                        ? WasRadiantFieldEffectUsedThisTurn(c.Id)
                        : WasRadiantEffectUsedThisTurn(c.Id)) &&
                        !Bot.HasInHand(c.Id))))
                .OrderBy(GetAscendanceRevivePriority)
                .ThenBy(GetMaterialPriority)
                .ToList();
            if (reviveTargets.Count > 0)
            {
                return SelectCount(reviveTargets, cards, min, max, 1);
            }

            List<ClientCard> fallbackTargets = cards.Where(c => c != null && IsRadiantCard(c) &&
                    c.IsMonster() && c.Level <= 6)
                .OrderBy(GetAscendanceRevivePriority)
                .ThenBy(GetMaterialPriority)
                .ToList();
            if (fallbackTargets.Count > 0)
            {
                return SelectCount(fallbackTargets, cards, min, max, 1);
            }
            return SelectMstForHand(cards, min, max);
        }

        private int GetAscendanceRevivePriority(ClientCard card)
        {
            if (card == null)
            {
                return 100;
            }
            if (card.IsCode(CardId.RadiantTyphoonSwen))
            {
                return 0;
            }
            if (card.IsCode(CardId.RadiantTyphoonDachs))
            {
                return 1;
            }
            if (card.IsCode(CardId.RadiantTyphoonKrosea))
            {
                return 2;
            }
            if (card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return 3;
            }
            return 4;
        }

        private IList<ClientCard> SelectVisionDiscard(IList<ClientCard> cards, int min, int max)
        {
            // Vision draws two cards and then asks for one discard. Some server
            // builds expose this hand selection as Discard, ToGrave, or without
            // a select hint, so the caller also verifies the exact one-card
            // hand-candidate shape before using this ranking.
            List<ClientCard> priority = cards.OrderBy(GetVisionDiscardPriority)
                .ThenBy(c => c.Id)
                .ToList();
            return SelectCount(priority, cards, min, max, Math.Max(min, 1));
        }

        private int GetVisionDiscardPriority(ClientCard card)
        {
            if (card == null)
            {
                return 1000;
            }

            // A card already used this turn is the first discard candidate.
            // Meghala remains useful while either its hand summon or field
            // trigger is still available, so spend it only after both are used.
            bool effectSpent = card.IsCode(CardId.RadiantTyphoonMeghala)
                ? WasRadiantHandEffectUsedThisTurn(card.Id) &&
                    WasRadiantFieldEffectUsedThisTurn(card.Id)
                : WasRadiantEffectUsedThisTurn(card.Id);
            if (effectSpent)
            {
                return 0;
            }
            if (card.IsCode(CardId.MysticalSpaceTyphoon))
            {
                return 10;
            }
            if (card.IsCode(CardId.RadiantTyphoonMandate))
            {
                return 20;
            }
            if (card.IsCode(CardId.RadiantTyphoonVision))
            {
                return 30;
            }
            if (card.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame))
            {
                return 40;
            }
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex))
            {
                return 50;
            }
            if (card.HasType(CardType.QuickPlay))
            {
                return 60;
            }
            if (IsRadiantCard(card) && card.IsSpell())
            {
                return 70;
            }
            if (IsRadiantCard(card) && card.IsMonster())
            {
                return 80;
            }
            return 90;
        }

        private IList<ClientCard> SelectMandateRecycle(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> recyclable = GetMandateRecycleCandidates(cards);
            List<ClientCard> priority = new List<ClientCard>();
            priority.AddRange(recyclable.Where(IsRadiantQuickPlay));
            priority.AddRange(recyclable.Where(c => c.IsCode(CardId.TheFallenTheVirtuous)));
            priority.AddRange(recyclable.Where(c => c.IsCode(CardId.SuperPolymerization)));
            priority.AddRange(recyclable.Where(c => c.IsCode(CardId.ForbiddenDroplet)));
            priority.AddRange(recyclable.Where(c => !c.IsCode(CardId.MysticalSpaceTyphoon) &&
                !priority.Contains(c)));
            // MST is always recycled last. Selecting exactly three naturally keeps
            // at least one in the graveyard whenever enough alternatives exist.
            priority.AddRange(recyclable.Where(c => c.IsCode(CardId.MysticalSpaceTyphoon)));
            return SelectCount(priority, cards, min, max, Math.Max(min, 3));
        }

        private List<ClientCard> GetMandateRecycleCandidates(IEnumerable<ClientCard> cards)
        {
            return cards.Where(c => c != null && c.HasType(CardType.QuickPlay) &&
                !IsCurrentChainRadiantQuickPlay(c)).ToList();
        }

        private bool IsCurrentChainRadiantQuickPlay(ClientCard card)
        {
            if (card == null || !IsRadiantQuickPlay(card))
            {
                return false;
            }

            return Duel.CurrentChain.Any(chainCard => chainCard != null &&
                chainCard.Controller == 0 && IsRadiantQuickPlay(chainCard) &&
                AreSameVisibleCard(chainCard, card));
        }

        private IList<ClientCard> SelectMstForHand(IList<ClientCard> cards, int min, int max)
        {
            ClientCard mst = GetPreferredMst(cards, true);
            if (mst == null)
            {
                return null;
            }
            return SelectCount(new List<ClientCard> { mst }, cards, min, max, 1);
        }

        private ClientCard GetPreferredMst(IEnumerable<ClientCard> cards, bool allowLastGraveCopy)
        {
            ClientCard nonGraveMst = cards.FirstOrDefault(c =>
                c.IsCode(CardId.MysticalSpaceTyphoon) && c.Location != CardLocation.Grave);
            if (nonGraveMst != null)
            {
                return nonGraveMst;
            }

            List<ClientCard> graveMsts = cards.Where(c =>
                c.IsCode(CardId.MysticalSpaceTyphoon) && c.Location == CardLocation.Grave).ToList();
            if (graveMsts.Count > 1 || allowLastGraveCopy)
            {
                return graveMsts.FirstOrDefault();
            }
            return null;
        }

        private ChainInfo GetBuildingChainInfo()
        {
            if (Duel.GetCurrentChainCard() == null || Duel.CurrentChainInfo.Count == 0)
            {
                return null;
            }
            return Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
        }

        private bool IsDescription(int cardId, int offset)
        {
            return ActivateDescription == Util.GetStringId(cardId, offset);
        }

        private bool IsDisabledOwnFieldMonster()
        {
            return Card != null && Card.Controller == 0 &&
                Card.Location == CardLocation.MonsterZone && Card.IsDisabled();
        }

        private bool IsMarineEidolonSummonTrigger()
        {
            return Card != null && Card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) &&
                Card.Controller == 0 && Card.Location == CardLocation.MonsterZone &&
                ActivateDescription == -1 &&
                Duel.LastSummonPlayer == 0 &&
                Duel.LastSummonedCards.Any(c => c != null && c.Controller == 0 &&
                    c.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon));
        }

        private bool IsMarineEidolonQuickPlayTrigger()
        {
            if (Card == null || !Card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) ||
                Card.Controller != 0 || Card.Location != CardLocation.MonsterZone ||
                _marineTrapPlacementResolvedThisTurn)
            {
                return false;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 2))
            {
                return true;
            }

            if (!_marineQuickPlayTriggerPending || Card.IsDisabled())
            {
                return false;
            }

            // Some trigger candidates use -1 instead of the effect string id.
            // This trigger is a new chain after the Quick-Play chain has ended;
            // use the event recorded by OnChaining instead of CurrentChainInfo.
            return ActivateDescription == -1 && !IsMarineEidolonSummonTrigger() &&
                _marineQuickPlayTriggerPending;
        }

        private bool ContainsOption(IList<int> options, int cardId, params int[] offsets)
        {
            foreach (int offset in offsets)
            {
                if (options.Contains(Util.GetStringId(cardId, offset)))
                {
                    return true;
                }
            }
            return false;
        }

        private int GetOptionIndex(IList<int> options, int cardId, int offset)
        {
            return options.IndexOf(Util.GetStringId(cardId, offset));
        }

        private bool IsMainPhase()
        {
            return Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2;
        }

        private bool CanSummonFromHandAfterPurulia()
        {
            return !_enemyPuruliaResolved || !_botSummonedFromHandAfterPurulia;
        }

        private bool ShouldStopRadiantSpecialSummon(CardLocation summonLocation)
        {
            bool lockBirdActive = _enemyDrollResolved || Duel.CurrentChainInfo.Any(chain =>
                chain != null && chain.ActivatePlayer == 0 &&
                chain.IsActivateCode(CardId.DrollLockBird));
            if (lockBirdActive || (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2))
            {
                return false;
            }

            // Maxx "C" starts restricting Special Summons only after its chain
            // has resolved successfully. While it is still being chained, keep
            // the hand-triggered Special Summon responses available.
            bool enemyMaxxCActive = _enemyMaxxCResolved;
            bool enemyFuwalosActive = _enemyFuwalosResolved || Duel.CurrentChainInfo.Any(chain =>
                chain != null && chain.ActivatePlayer == 1 &&
                chain.IsActivateCode(CardId.MulcharmyFuwalos));
            bool atAdvantage = Util.GetProblematicEnemyMonster() == null &&
                (Duel.Player == 0 || Bot.GetMonsterCount() > 0);

            if (enemyMaxxCActive && atAdvantage)
            {
                return true;
            }

            return enemyFuwalosActive && (summonLocation & (CardLocation.Deck | CardLocation.Extra)) != 0;
        }

        private bool ShouldPrioritizeGallantThiefSummon()
        {
            return Bot.GetMonsterCount() == 0 && CanUseEnemyTributesForGallantThief() &&
                Bot.HasInHand(CardId.TheWorldsGreatestGallantThief);
        }

        private bool ShouldPrioritizeMonsterSummonAgainstExtraDeck()
        {
            return Duel.Player == 0 && Bot.HasInHand(CardId.ForbiddenDroplet) &&
                Enemy.GetMonsters().Count(c => c != null && c.IsExtraCard()) >= 2;
        }

        private bool HasMainPhaseMonsterSummonCandidate()
        {
            return Duel.MainPhase != null &&
                (Duel.MainPhase.SummonableCards.Any(c =>
                    IsMainPhaseMonsterSummonCandidate(c, false)) ||
                 Duel.MainPhase.SpecialSummonableCards.Any(c =>
                    IsMainPhaseMonsterSummonCandidate(c, true)));
        }

        private bool IsMainPhaseMonsterSummonCandidate(ClientCard card, bool specialSummon)
        {
            if (card == null)
            {
                return false;
            }

            if (!CanSummonFromHandAfterPurulia())
            {
                return false;
            }

            if (specialSummon)
            {
                if (Bot.GetMonsterCount() >= 5 || IsMaxxCStoppingRadiantSpecialSummon())
                {
                    return false;
                }

                if (card.IsCode(CardId.RadiantTyphoonSwen) &&
                    Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala)) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonMeghala))
                {
                    return false;
                }

                if (card.IsCode(CardId.RadiantTyphoonDachs) &&
                    Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonSwen)) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonSwen))
                {
                    return false;
                }
            }

            if (card.IsCode(CardId.RadiantTyphoonSwen, CardId.RadiantTyphoonDachs,
                CardId.RadiantTyphoonMeghala))
            {
                return !specialSummon || IsMainPhase();
            }

            return !specialSummon && card.IsCode(CardId.TheWorldsGreatestGallantThief) &&
                Enemy.GetMonsterCount() >= 2;
        }

        private bool IsMaxxCStoppingRadiantSpecialSummon()
        {
            return _enemyMaxxCResolved && Util.GetProblematicEnemyMonster() == null &&
                (Duel.Player == 0 || Bot.GetMonsterCount() > 0);
        }

        private bool ShouldRequireMain1MonsterSummon()
        {
            return _mustStartMain1WithMonsterSummon && Duel.Player == 0 &&
                Duel.Phase == DuelPhase.Main1 && Duel.CurrentChain.Count == 0 &&
                HasMainPhaseMonsterSummonCandidate();
        }

        private bool NeedMstStarter()
        {
            if (_enemyDrollResolved || Enemy.GetSpellCount() == 0)
            {
                return false;
            }

            // During the opponent's turn, a Mystical Space Typhoon in our
            // hand or Graveyard cannot be activated immediately. Only a
            // usable Set MST counts as an available response, and the
            // starter route itself must answer an effective opponent field
            // chain rather than begin the turn proactively.
            if (Duel.Player == 1)
            {
                return !HasFaceDownMysticalSpaceTyphoon() &&
                    HasLiveOpponentFieldChain();
            }

            // During our turn, MST in hand is directly usable and a Graveyard
            // copy can be recovered by the deck's own effects, so retain the
            // broader resource check here.
            return !Bot.HasInHand(CardId.MysticalSpaceTyphoon) &&
                !Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon) &&
                !HasFaceDownMysticalSpaceTyphoon();
        }

        private bool HasFaceDownMysticalSpaceTyphoon()
        {
            return Bot.GetSpells().Any(c => c != null &&
                c.IsCode(CardId.MysticalSpaceTyphoon) && c.IsFacedown() &&
                !c.IsDisabled());
        }

        private int GetFaceDownMysticalSpaceTyphoonCount()
        {
            return Bot.GetSpells().Count(c => c != null &&
                c.IsCode(CardId.MysticalSpaceTyphoon) && c.IsFacedown() &&
                !c.IsDisabled());
        }

        private bool HasLiveOpponentFieldChain()
        {
            if (!HasLiveOpponentChain())
            {
                return false;
            }

            ClientCard source = GetLatestOpponentFieldCardForChain();
            return source != null && !source.IsDisabled();
        }

        private bool ShouldDelayOpponentAscendanceMstStarter()
        {
            return Card != null && Card.IsCode(CardId.RadiantTyphoonAscendance) &&
                Duel.Player == 1 && !HasLiveOpponentFieldChain();
        }

        private bool CanSpecialSummonMeghalaFromHandNow()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() || _usedMeghalaHandSummon ||
                Bot.GetMonsterCount() >= 5 ||
                !Bot.HasInHand(CardId.RadiantTyphoonMeghala) ||
                !IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonMeghala))
            {
                return false;
            }
            return Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon) || Enemy.GetSpellCount() == 0;
        }

        private bool CanSpecialSummonSmallRadiantFromHandNow()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() || Bot.GetMonsterCount() >= 5 ||
                (!Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon) && Enemy.GetSpellCount() > 0))
            {
                return false;
            }

            return (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala) && !_usedMeghalaHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonMeghala)) ||
                (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonSwen) && !_usedSwenHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonSwen)) ||
                (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonDachs) && !_usedDachsHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonDachs));
        }

        private bool HasDirectSmallRadiantSpecialSummonNow()
        {
            if (!CanSummonFromHandAfterPurulia() || !IsMainPhase() || Bot.GetMonsterCount() >= 5 ||
                ShouldStopRadiantSpecialSummon(CardLocation.Hand))
            {
                return false;
            }

            return (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala) && !_usedMeghalaHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonMeghala)) ||
                (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonSwen) && !_usedSwenHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonSwen)) ||
                (Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonDachs) && !_usedDachsHandSummon) &&
                    IsCurrentSpecialSummonCandidate(CardId.RadiantTyphoonDachs));
        }

        private bool IsRadiantCard(ClientCard card)
        {
            return card != null && card.HasSetcode(SetcodeRadiantTyphoon);
        }

        private bool IsRadiantMonster(ClientCard card)
        {
            return IsRadiantCard(card) && card.IsMonster() && card.IsFaceup();
        }

        private bool IsRadiantQuickPlay(ClientCard card)
        {
            return card != null && card.IsCode(CardId.RadiantTyphoonVision,
                CardId.RadiantTyphoonAscendance, CardId.RadiantTyphoonChant);
        }

        private bool HasAccessible(int cardId)
        {
            return Bot.Hand.Any(c => c.IsCode(cardId)) ||
                Bot.GetMonsters().Any(c => c.IsCode(cardId)) ||
                Bot.GetSpells().Any(c => c.IsCode(cardId)) ||
                Bot.Graveyard.Any(c => c.IsCode(cardId));
        }

        private List<int> BuildSearchPriority(params int[] ids)
        {
            List<int> priority = new List<int>();
            AddSearchPriorityPass(priority, ids, true, true);
            AddSearchPriorityPass(priority, ids, true, false);
            AddSearchPriorityPass(priority, ids, false, true);
            AddSearchPriorityPass(priority, ids, false, false);
            return priority;
        }

        private List<int> BuildSearchPriorityWithoutFieldMeghala(params int[] ids)
        {
            if (Bot.GetMonsters().Any(c => c.IsCode(CardId.RadiantTyphoonMeghala)))
            {
                ids = ids.Where(id => id != CardId.RadiantTyphoonMeghala).ToArray();
            }
            return BuildSearchPriority(ids);
        }

        private void AddSearchPriorityPass(List<int> priority, int[] ids, bool requireUnused, bool requireNotInHand)
        {
            foreach (int id in ids)
            {
                if (priority.Contains(id))
                {
                    continue;
                }

                bool unused = id == CardId.RadiantTyphoonMeghala
                    ? !WasRadiantHandEffectUsedThisTurn(id) ||
                        !WasRadiantFieldEffectUsedThisTurn(id)
                    : !WasRadiantEffectUsedThisTurn(id);
                bool notInHand = !Bot.HasInHand(id);
                if ((!requireUnused || unused) && (!requireNotInHand || notInHand))
                {
                    priority.Add(id);
                }
            }
        }

        private bool HasRadiantQuickPlayAvailable()
        {
            return Bot.Hand.Concat(Bot.GetSpells()).Any(IsRadiantQuickPlay);
        }

        private bool HasRadiantQuickPlayInHand()
        {
            return Bot.Hand.Any(IsRadiantQuickPlay);
        }

        private bool HasRadiantSpellInHand()
        {
            return Bot.Hand.Any(c => IsRadiantCard(c) && c.IsSpell());
        }

        private bool ShouldPreferKroseaAscendanceSearch(IList<ClientCard> cards)
        {
            return cards.Any(c => c.IsCode(CardId.RadiantTyphoonAscendance)) &&
                WasRadiantEffectUsedThisTurn(CardId.RadiantTyphoonSwen) &&
                WasRadiantEffectUsedThisTurn(CardId.RadiantTyphoonDachs) &&
                WasRadiantEffectUsedThisTurn(CardId.RadiantTyphoonChant) &&
                !HasRadiantSpellInHand() &&
                !Bot.HasInHand(new[] { CardId.RadiantTyphoonAscendance, CardId.RadiantTyphoonMeghala }) &&
                !WasRadiantEffectUsedThisTurn(CardId.RadiantTyphoonAscendance) &&
                !_usedMeghalaDeckSummon && !_usedMeghalaHandSummon;
        }

        private bool CanUseAsKroseaTribute(ClientCard card)
        {
            return card != null && card.Controller == 0 &&
                card.Location == CardLocation.MonsterZone && card.IsMonster() &&
                !card.HasType(CardType.Link) && !card.HasType(CardType.Xyz);
        }

        private bool CanUsePreferredKroseaTribute()
        {
            List<ClientCard> genericCandidates = Bot.GetMonsters().Where(c => c != null && c.IsMonster())
                .OrderBy(c => c.Attack).ToList();
            ClientCard preferredChoice = Bot.GetMonsters().Where(CanUseAsKroseaTribute)
                .OrderBy(c => c.IsCode(CardId.RadiantTyphoonDachs) ? 0 :
                    c.IsCode(CardId.RadiantTyphoonSwen) ? 1 : 2)
                .ThenBy(c => c.Attack).FirstOrDefault();
            if (genericCandidates.Count == 0 || preferredChoice == null)
            {
                return false;
            }

            int lowestAttack = genericCandidates[0].Attack;
            List<ClientCard> lowestAttackCandidates = genericCandidates.Where(c => c.Attack == lowestAttack).ToList();
            return lowestAttackCandidates.Count == 1 && lowestAttackCandidates[0] == preferredChoice;
        }

        private bool WasRadiantEffectUsedThisTurn(int cardId)
        {
            if (cardId == CardId.RadiantTyphoonSwen)
            {
                return _usedSwenSearch;
            }
            if (cardId == CardId.RadiantTyphoonDachs)
            {
                return _usedDachsSearch;
            }
            if (cardId == CardId.RadiantTyphoonKrosea)
            {
                return _usedKroseaSearch;
            }
            if (cardId == CardId.RadiantTyphoonMeghala)
            {
                // Callers that care about one of Meghala's independent effects
                // use the hand/field helpers instead of this combined result.
                return WasRadiantHandEffectUsedThisTurn(cardId) ||
                    WasRadiantFieldEffectUsedThisTurn(cardId);
            }
            if (cardId == CardId.RadiantTyphoonFonixTheGreatFlame)
            {
                return _usedFonixHandSummon ||
                    _activatedRadiantCardsThisTurn.Contains(cardId);
            }
            if (cardId == CardId.RadiantTyphoonVaruroonTheVibrantVortex)
            {
                return _usedVortexHandSummon ||
                    _activatedRadiantCardsThisTurn.Contains(cardId);
            }
            return _activatedRadiantCardsThisTurn.Contains(cardId);
        }

        private bool HasRadiantReviveTarget()
        {
            return Bot.Graveyard.Any(c => IsRadiantCard(c) && c.IsMonster() && c.Level <= 6);
        }

        private bool CanUseAscendanceReviveNow()
        {
            if (Bot.GetMonsterCount() >= 5 || ShouldStopRadiantSpecialSummon(CardLocation.Grave))
            {
                return false;
            }

            return Bot.Graveyard.Any(c => IsRadiantCard(c) && c.IsMonster() && c.Level <= 6 &&
                !(c.IsCode(CardId.RadiantTyphoonMeghala)
                    ? WasRadiantFieldEffectUsedThisTurn(c.Id)
                    : WasRadiantEffectUsedThisTurn(c.Id)) &&
                !Bot.HasInHand(c.Id));
        }

        private bool NeedRadiantStarter()
        {
            return !HasAccessible(CardId.RadiantTyphoonSwen) ||
                !HasAccessible(CardId.RadiantTyphoonDachs) ||
                Bot.GetMonsterCount() == 0;
        }

        private bool ShouldWaitForRadiantTrigger(ClientCard card)
        {
            if (card == null || card.IsDisabled() || _enemyDrollResolved ||
                !IsCurrentRadiantTriggerCandidate(card))
            {
                return false;
            }
            if (card.IsCode(CardId.RadiantTyphoonSwen))
            {
                return !_usedSwenSearch;
            }
            if (card.IsCode(CardId.RadiantTyphoonDachs))
            {
                return !_usedDachsSearch;
            }
            if (card.IsCode(CardId.RadiantTyphoonKrosea))
            {
                return !_usedKroseaSearch;
            }
            if (card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return !_usedMeghalaDeckSummon && HasRadiantQuickPlayAvailable();
            }
            return false;
        }

        private bool IsCurrentSpecialSummonCandidate(int cardId)
        {
            if (Duel.MainPhase == null || Duel.CurrentChain.Count != 0)
            {
                return false;
            }
            return Duel.MainPhase.SpecialSummonableCards.Any(c => c != null && c.IsCode(cardId));
        }

        private bool IsCurrentRadiantTriggerCandidate(ClientCard card)
        {
            if (Duel.MainPhase == null || Duel.CurrentChain.Count != 0)
            {
                return false;
            }
            return Duel.MainPhase.ActivableCards.Any(c => AreSameVisibleCard(c, card));
        }

        private bool CanSummonSeaSpiritNow()
        {
            if (_seaSpiritSummoned || !Bot.HasInExtra(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                return false;
            }
            return Bot.GetMonsters().Count(c => IsRadiantMonster(c) && CanUseAsLinkMaterial(c) &&
                !ShouldWaitForRadiantTrigger(c)) >= 2;
        }

        private bool ShouldPrioritizeKroseaTributeOverSeaSpirit()
        {
            if (Duel.Player != 0 || Enemy.GetSpellCount() != 0 || HasRadiantSpellInHand() ||
                HasRadiantQuickPlayAvailable() ||
                _enemyDrollResolved || !Bot.HasInHand(CardId.RadiantTyphoonKrosea) ||
                WasRadiantEffectUsedThisTurn(CardId.RadiantTyphoonKrosea))
            {
                return false;
            }

            int linkableRadiantCount = Bot.GetMonsters().Count(c => IsRadiantMonster(c) &&
                CanUseAsLinkMaterial(c) && !ShouldWaitForRadiantTrigger(c));
            return linkableRadiantCount == 2 && CanUsePreferredKroseaTribute();
        }

        private bool CanUseAsExtraDeckMaterial(ClientCard card)
        {
            return card != null && !card.IsCode(CardId.RadiantTyphoonMeghala);
        }

        private bool CanUseAsLinkMaterial(ClientCard card)
        {
            if (!CanUseAsExtraDeckMaterial(card) || card.IsCode(CardId.HraesvelgrTheDesperateDoomEagle))
            {
                return false;
            }
            if (!card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                return true;
            }

            // Preserve Marine until the deck has secured the intended Mandate
            // payoff. Once secured, do not lock it out of material use on later
            // turns merely because the turn-scoped trigger flag was reset.
            return _marineTrapPlacementResolvedThisTurn ||
                _marineMandatePayoffSecured || HasEstablishedMandate();
        }

        private IList<ClientCard> SelectExtraDeckMaterialsWithoutMeghala(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> materials = cards.Where(CanUseAsExtraDeckMaterial)
                .OrderBy(GetMaterialPriority).Take(min).ToList();
            if (materials.Count < min)
            {
                return null;
            }
            return materials;
        }

        private IList<ClientCard> SelectLinkMaterials(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> materials = cards.Where(CanUseAsLinkMaterial)
                .OrderBy(GetMaterialPriority).Take(min).ToList();
            if (materials.Count < min)
            {
                return null;
            }
            return materials;
        }

        private bool IsValuableDisruptionMonster(ClientCard card)
        {
            return card != null && card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                CardId.RadiantTyphoonFonixTheGreatFlame,
                CardId.ShiinaTwinTempestsOfCelestialThunder,
                CardId.TotemBird);
        }

        private int GetMaterialPriority(ClientCard card)
        {
            if (card == null)
            {
                return 100;
            }
            if (card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return 1000;
            }
            if (card.IsDisabled())
            {
                return 0;
            }
            if (card.IsCode(CardId.RadiantTyphoonSwen) && _usedSwenSearch)
            {
                return 1;
            }
            if (card.IsCode(CardId.RadiantTyphoonDachs) && _usedDachsSearch)
            {
                return 2;
            }
            if (card.IsCode(CardId.RadiantTyphoonKrosea) && _usedKroseaSearch)
            {
                return 3;
            }
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                bool canStillDisrupt = Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                    c.Attack >= 2000);
                if (canStillDisrupt)
                {
                    return 30;
                }
                if (_marineMandatePayoffSecured || HasEstablishedMandate())
                {
                    return 5;
                }
            }
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex,
                CardId.RadiantTyphoonFonixTheGreatFlame,
                CardId.ShiinaTwinTempestsOfCelestialThunder,
                CardId.TotemBird))
            {
                return 30;
            }
            return 10 + Math.Max(card.Attack, card.Defense) / 1000;
        }

        private int GetDropletCostPriority(ClientCard card)
        {
            if (!IsDropletCostCandidate(card))
            {
                return 1000;
            }

            bool isActivatingRadiantQuickPlay = IsRadiantQuickPlay(card) &&
                Duel.CurrentChain.Contains(card);
            if (isActivatingRadiantQuickPlay)
            {
                return 0;
            }

            bool isSpentOrActivatingMonster = card.IsMonster() && IsDropletNoFutureValue(card);
            if (isSpentOrActivatingMonster && card.IsCode(CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs, CardId.RadiantTyphoonKrosea))
            {
                return 10 + (card.IsCode(CardId.RadiantTyphoonSwen) ? 1 :
                    card.IsCode(CardId.RadiantTyphoonDachs) ? 2 : 3);
            }

            if (isSpentOrActivatingMonster)
            {
                return 20 + GetDiscardPriority(card);
            }

            if (card.IsOnField())
            {
                return 30 + GetDiscardPriority(card);
            }

            // Cards still in hand are the final fallback because they retain
            // future summon, search or interruption value whenever possible.
            return 40 + GetDiscardPriority(card);
        }

        private bool IsDropletNoFutureValue(ClientCard card)
        {
            return card != null && (card.IsDisabled() || Duel.CurrentChain.Contains(card) ||
                IsRadiantCard(card) && WasRadiantEffectUsedThisTurn(card.Id));
        }

        private bool IsDropletCostCandidate(ClientCard card)
        {
            // Set cards are legal Droplet costs, but this deck deliberately
            // preserves its Set interaction and the face-up Mandate engine.
            return card != null && card.Controller == 0 &&
                !card.IsCode(CardId.RadiantTyphoonMeghala, CardId.RadiantTyphoonMandate) &&
                !card.IsFacedown();
        }

        private int GetDiscardPriority(ClientCard card)
        {
            if (card == null)
            {
                return 100;
            }
            if (card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return 1000;
            }
            if (card.Location == CardLocation.MonsterZone)
            {
                if (card.IsDisabled())
                {
                    return 0;
                }
                return IsValuableDisruptionMonster(card) ? 50 : GetMaterialPriority(card) + 5;
            }
            if (card.Location == CardLocation.SpellZone)
            {
                if (IsRadiantQuickPlay(card) && Duel.CurrentChain.Contains(card))
                {
                    return 0;
                }
                if (card.IsFaceup() && !card.IsCode(CardId.RadiantTyphoonMandate))
                {
                    return 2;
                }
                return card.IsCode(CardId.RadiantTyphoonMandate) ? 40 : 12;
            }
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheVibrantVortex))
            {
                return 0;
            }
            if (card.IsCode(CardId.RadiantTyphoonFonixTheGreatFlame))
            {
                return 1;
            }
            if (Bot.Hand.Count(c => c.IsCode(card.Id)) > 1)
            {
                return 2;
            }
            if (card.IsCode(CardId.TheWorldsGreatestGallantThief, CardId.ShiinaTwinTempestsOfCelestialThunder))
            {
                return 3;
            }
            if (card.IsCode(CardId.DrollLockBird, CardId.MaxxC, CardId.AshBlossomJoyousSpring, CardId.MulcharmyFuwalos))
            {
                return Duel.Player == 0 ? 4 : 8;
            }
            if (IsRadiantQuickPlay(card))
            {
                return 6;
            }
            if (card.IsCode(CardId.MysticalSpaceTyphoon))
            {
                return 20;
            }
            if (IsRadiantCard(card))
            {
                return 15;
            }
            return 10;
        }

        private List<ClientCard> GetOrderedEnemyCards(IEnumerable<ClientCard> source)
        {
            ClientCard problem = Util.GetProblematicEnemyCard(0, true);
            return source.Where(c => c != null).Distinct()
                .OrderByDescending(c => c == problem)
                .ThenByDescending(c => c.IsFaceup())
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense))
                .ToList();
        }

        private List<ClientCard> GetOrderedFallenTargets(IEnumerable<ClientCard> source)
        {
            ClientCard activeSource = GetLatestOpponentFieldCardForChain();
            List<ClientCard> candidates = source.Where(c => !IsAlreadyHandledByMysticalSpaceTyphoon(c))
                .Where(IsWorthwhileFallenTarget)
                .Distinct().ToList();

            return candidates
                .OrderByDescending(c => AreSameVisibleCard(c, activeSource))
                .ThenByDescending(c => c.IsMonster() && c.IsExtraCard())
                .ThenByDescending(c => c.IsMonster() &&
                    (c.IsFloodgate() || c.IsMonsterDangerous() ||
                        c.IsMonsterShouldBeDisabledBeforeItUseEffect()))
                .ThenByDescending(c => c.HasType(CardType.Field | CardType.Continuous))
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense))
                .ToList();
        }

        private bool IsAlreadyHandledByMysticalSpaceTyphoon(ClientCard card)
        {
            if (card == null || _mysticalSpaceTyphoonTarget == null ||
                !AreSameVisibleCard(card, _mysticalSpaceTyphoonTarget) ||
                !_mysticalSpaceTyphoonTarget.IsOnField())
            {
                return false;
            }

            for (int i = 0; i < Duel.CurrentChainInfo.Count; ++i)
            {
                ChainInfo chain = Duel.CurrentChainInfo[i];
                if (chain.ActivatePlayer == 0 && chain.IsActivateCode(CardId.MysticalSpaceTyphoon) &&
                    !Duel.NegatedChainIndexList.Contains(i + 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsWorthwhileFallenTarget(ClientCard card)
        {
            if (card == null || card.Controller != 1 || !card.IsFaceup() ||
                card.IsShouldNotBeTarget() || card.IsDisabled())
            {
                return false;
            }

            if (card.IsMonster())
            {
                return !card.IsShouldNotBeMonsterTarget() &&
                    (card.IsExtraCard() || card.IsFloodgate() || card.IsMonsterDangerous() ||
                        card.IsMonsterShouldBeDisabledBeforeItUseEffect() ||
                        card.Attack >= 1800 || card.Defense >= 1800);
            }

            // Destroying a normal/Quick-Play Spell or a normal Trap after it
            // has been activated usually does not negate its effect. Keep The
            // Fallen for persistent field cards that retain value on the field.
            return !card.IsShouldNotBeSpellTrapTarget() &&
                card.HasType(CardType.Field | CardType.Continuous);
        }

        private bool IsEcclesiaFusionTarget(ClientCard card)
        {
            return card != null && card.Controller == 0 &&
                (card.Location == CardLocation.Grave || card.Location == CardLocation.Removed) &&
                card.Level == 8 && card.HasType(CardType.Fusion) &&
                !card.IsShouldNotBeTarget();
        }

        private bool IsEcclesiaFieldTarget(ClientCard card)
        {
            if (card == null || !card.IsOnField() || card.IsShouldNotBeTarget())
            {
                return false;
            }

            if (IsEcclesiaMonsterCard(card))
            {
                return !card.IsShouldNotBeMonsterTarget();
            }
            return !card.IsShouldNotBeSpellTrapTarget();
        }

        private bool IsEcclesiaMonsterCard(ClientCard card)
        {
            // A facedown monster may have Type == 0, but its monster-zone
            // location is still public protocol information.
            return card != null && (card.IsMonster() || card.Location == CardLocation.MonsterZone);
        }

        private bool IsEcclesiaThreateningExtraMonster(ClientCard card)
        {
            if (!IsEcclesiaFieldTarget(card) || card.Controller != 1 ||
                !card.IsMonster() || !card.IsFaceup() ||
                !card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
            {
                return false;
            }

            ClientCard problem = Util.GetProblematicEnemyMonster(0, true);
            return card == problem || card.IsMonsterDangerous() || card.IsMonsterInvincible() ||
                card.IsFloodgate() || card.IsMonsterShouldBeDisabledBeforeItUseEffect() ||
                card.Attack >= 2500;
        }

        private List<ClientCard> GetOrderedEcclesiaFieldTargets(IEnumerable<ClientCard> source)
        {
            List<ClientCard> candidates = source.Where(IsEcclesiaFieldTarget).Distinct().ToList();
            List<ClientCard> result = new List<ClientCard>();

            result.AddRange(candidates.Where(IsEcclesiaThreateningExtraMonster)
                .OrderByDescending(c => Math.Max(c.Attack, c.Defense)));
            result.AddRange(candidates.Where(c => c.Controller == 1 && !IsEcclesiaMonsterCard(c) && c.IsFacedown()));
            result.AddRange(candidates.Where(c => c.Controller == 1 && IsEcclesiaMonsterCard(c) &&
                !IsEcclesiaThreateningExtraMonster(c))
                .OrderByDescending(c => Math.Max(c.Attack, c.Defense)));
            result.AddRange(candidates.Where(c => c.Controller == 1 && !IsEcclesiaMonsterCard(c) && !c.IsFacedown())
                .OrderByDescending(c => Math.Max(c.Attack, c.Defense)));
            result.AddRange(candidates.Where(c => c.Controller == 0)
                .OrderBy(GetMaterialPriority));
            return result.Distinct().ToList();
        }

        private IList<ClientCard> SelectByIds(IList<ClientCard> cards, int min, int max, int desired, params int[] ids)
        {
            List<ClientCard> selected = new List<ClientCard>();
            foreach (int id in ids)
            {
                ClientCard card = cards.FirstOrDefault(c => c.IsCode(id) && !selected.Contains(c));
                if (card != null)
                {
                    selected.Add(card);
                }
                if (selected.Count >= desired)
                {
                    break;
                }
            }
            return SelectCount(selected, cards, min, max, desired);
        }

        private IList<ClientCard> SelectCount(IList<ClientCard> priority, IList<ClientCard> cards, int min, int max, int desired)
        {
            int count = Math.Min(max, Math.Max(min, desired));
            if (count == 0)
            {
                return min == 0 ? new List<ClientCard>() : null;
            }
            List<ClientCard> selected = priority.Where(cards.Contains).Distinct().Take(count).ToList();
            return Util.CheckSelectCount(selected, cards, min, count);
        }
    }
}
