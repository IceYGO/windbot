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
            public const int InfiniteImpermanence = 10045474;
            public const int MulcharmyFuwalos = 42141493;
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
        private bool _usedDachsSearch;
        private bool _usedSwenSearch;
        private bool _usedKroseaSearch;
        private bool _seaSpiritSummoned;
        private bool _marineQuickPlayTriggerPending;
        private bool _marineTrapPlacementPending;
        private bool _marineTrapPlacementResolved;
        private bool _enemyDrollResolved;
        private bool _botDrawHandTrapResolved;
        private bool _fallenPreferRevive;
        private int _dropletCostCount;
        private readonly HashSet<int> _activatedRadiantCardsThisTurn = new HashSet<int>();

        public RadiantTyphoonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Ecclesia's graveyard effect is a high-priority Main Phase resource
            // conversion. Its two targets are selected from the server candidates
            // in SelectActivationCard, where the two target requests can be told apart.
            AddExecutor(ExecutorType.Activate, CardId.EcclesiaAndTheDarkDragon, EcclesiaActivate);

            // An empty field with Gallant Thief in hand and an opponent monster is
            // a committed summon line. Its summon has priority over every voluntary
            // main-phase activation.
            AddExecutor(ExecutorType.Summon, CardId.TheWorldsGreatestGallantThief, GallantThiefSummon);

            // Free or low-cost chain interaction.
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonVaruroonTheVibrantVortex, VibrantVortexActivate);
            AddExecutor(ExecutorType.Activate, CardId.RadiantTyphoonMandate, MandateActivate);
            AddExecutor(ExecutorType.Activate, CardId.TotemBird, TotemBirdActivate);
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
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonFonixTheGreatFlame, PostExpansionNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonVaruroonTheVibrantVortex, PostExpansionNormalSummon);

            // Main-deck engine. Meghala takes priority over Swen, while Swen takes
            // priority over Dachs. Summon triggers should resolve before material use.
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonMeghala, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonSwen, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantTyphoonDachs, SmallRadiantSpecialSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonSwen, SmallRadiantNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonDachs, SmallRadiantNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonKrosea, KroseaNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.RadiantTyphoonMeghala, SmallRadiantNormalSummon);

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

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, RadiantSpellSet);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override bool OnPreActivate(ClientCard card)
        {
            if (Duel.Player == 0 && ShouldPrioritizeGallantThiefSummon() &&
                (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
            {
                return false;
            }
            return base.OnPreActivate(card);
        }

        public override void OnNewTurn()
        {
            _usedMeghalaDeckSummon = false;
            _usedMeghalaHandSummon = false;
            _usedDachsSearch = false;
            _usedSwenSearch = false;
            _usedKroseaSearch = false;
            _seaSpiritSummoned = false;
            _marineQuickPlayTriggerPending = false;
            _marineTrapPlacementPending = false;
            _marineTrapPlacementResolved = false;
            _enemyDrollResolved = false;
            _botDrawHandTrapResolved = false;
            _fallenPreferRevive = false;
            _dropletCostCount = 0;
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
            base.OnChaining(player, card);
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null && card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) &&
                (previousLocation & (int)CardLocation.MonsterZone) != 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) == 0)
            {
                _marineQuickPlayTriggerPending = false;
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
                    if (isTrapPlacement && !Duel.IsCurrentSolvingChainNegated())
                    {
                        _marineTrapPlacementResolved = true;
                    }
                    _marineTrapPlacementPending = false;
                }

                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentChain.IsActivateCode(CardId.DrollLockBird))
                    {
                        _enemyDrollResolved = true;
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
            _fallenPreferRevive = false;
            _dropletCostCount = 0;
            base.OnChainEnd();
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
            if (Duel.Player == 0 || DefaultCheckWhetherCardIsNegated(Card))
            {
                return false;
            }
            return !_botDrawHandTrapResolved;
        }

        private bool TotemBirdActivate()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            return Duel.LastChainPlayer == 1 && lastChainCard != null &&
                (lastChainCard.IsSpell() || lastChainCard.IsTrap()) && !lastChainCard.IsDisabled();
        }

        private bool VibrantVortexActivate()
        {
            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 0) || Card.Location == CardLocation.Hand)
            {
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 1) || Card.Location == CardLocation.MonsterZone)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                return Duel.LastChainPlayer == 1 && lastChainCard != null && lastChainCard.IsMonster() && !lastChainCard.IsDisabled();
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheVibrantVortex, 2) || Card.Location == CardLocation.Grave)
            {
                return Duel.CurrentChain.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon));
            }
            return false;
        }

        private bool FonixActivate()
        {
            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 0) || Card.Location == CardLocation.Hand)
            {
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 1) || Card.Location == CardLocation.MonsterZone)
            {
                List<ClientCard> targets = GetOrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                    .Take(2).ToList();
                if (targets.Count == 0)
                {
                    return false;
                }
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonFonixTheGreatFlame, 2) || Card.Location == CardLocation.Grave)
            {
                return Duel.CurrentChain.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon));
            }
            return false;
        }

        private bool MandateActivate()
        {
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
                bool shouldRecycle = quickPlays.Count >= 3 && quickPlays.Any(IsRadiantCard);
                if (shouldRecycle)
                {
                    _activatedRadiantCardsThisTurn.Add(Card.Id);
                }
                return shouldRecycle;
            }

            if (IsDescription(CardId.RadiantTyphoonMandate, 1))
            {
                if (!CanUseMandateToNegateChainOne())
                {
                    return false;
                }
                ClientCard target = GetOpponentChainOneFieldCard();
                if (target == null)
                {
                    return false;
                }
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
            if (Duel.LastChainPlayer != 1)
            {
                return false;
            }

            bool enemyHasExtraDeckMonster = Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
            bool botHasEstablishedExtraDeckMonster = Bot.GetMonsters().Any(c => c.IsFaceup() &&
                (c.HasType(CardType.Xyz) || (c.HasType(CardType.Link) && c.LinkCount >= 3)));
            bool extraDeckBoardCondition = enemyHasExtraDeckMonster && !botHasEstablishedExtraDeckMonster;
            bool backrowBoardCondition = Enemy.GetSpellCount() >= 3;
            return extraDeckBoardCondition || backrowBoardCondition;
        }

        private bool MysticalSpaceTyphoonActivate()
        {
            if (Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return false;
            }

            // With a face-up Mandate, first put another Radiant quick-play into
            // the chain. MST then destroys that same quick-play so Mandate can
            // negate the opponent's chain-one card.
            if (ShouldActivateRadiantQuickPlayForMandate())
            {
                return false;
            }

            ClientCard target = GetMysticalSpaceTyphoonTarget();
            if (target == null)
            {
                return false;
            }
            _activatedRadiantCardsThisTurn.Add(Card.Id);
            return true;
        }

        private ClientCard GetMysticalSpaceTyphoonTarget()
        {
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

            ClientCard enemyChainOne = GetOpponentChainOneFieldCard();
            if (enemyChainOne != null && (enemyChainOne.IsSpell() || enemyChainOne.IsTrap()))
            {
                return enemyChainOne;
            }

            // If no Radiant quick-play is available, follow the opponent's chain
            // by destroying an opposing spell/trap. The Mandate effect, when
            // legal, will still target the chain-one card above.
            ClientCard enemyTarget = GetBestMandateMstTarget();
            if (enemyTarget != null)
            {
                return enemyTarget;
            }

            if (activatedRadiant != null && CanUseMysticalSpaceTyphoonOnOwnCard())
            {
                return activatedRadiant;
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

            return enemySpells.FirstOrDefault(c => c.IsFaceup() &&
                c.IsSpell() && c.HasType(CardType.Continuous));
        }

        private ClientCard GetBestMandateMstTarget()
        {
            ClientCard facedown = Enemy.GetSpells().FirstOrDefault(c => c.IsFacedown());
            if (facedown != null)
            {
                return facedown;
            }

            ClientCard chainCard = Duel.CurrentChainInfo.Skip(1).FirstOrDefault(c =>
                c.ActivatePlayer == 1 && c.RelatedCard != null && c.RelatedCard.IsOnField() &&
                (c.RelatedCard.IsSpell() || c.RelatedCard.IsTrap()))?.RelatedCard;
            if (chainCard != null)
            {
                return chainCard;
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
            return Bot.GetSpells().Any(c => c.IsCode(CardId.RadiantTyphoonMandate) &&
                c.IsFaceup() && !c.IsDisabled());
        }

        private ClientCard GetOwnChainRadiantQuickPlay()
        {
            return Duel.CurrentChain.Reverse().FirstOrDefault(c => c != null && c.Controller == 0 &&
                c.IsOnField() && c.Location == CardLocation.SpellZone && IsRadiantQuickPlay(c));
        }

        private ClientCard GetOpponentChainOneFieldCard()
        {
            if (Duel.CurrentChainInfo.Count == 0 || Duel.CurrentChainInfo[0].ActivatePlayer != 1)
            {
                return null;
            }

            ChainInfo chainOne = Duel.CurrentChainInfo[0];
            ClientCard related = chainOne.RelatedCard;
            ClientCard fieldCard = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .FirstOrDefault(c => c == related);
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

        private bool CanBuildMandateChainLine()
        {
            return HasFaceupMandate() && GetOwnChainRadiantQuickPlay() != null &&
                GetOpponentChainOneFieldCard() != null;
        }

        private bool CanUseMandateToNegateChainOne()
        {
            return CanBuildMandateChainLine() &&
                Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon));
        }

        private bool HasOtherRadiantQuickPlayInHand()
        {
            return Bot.Hand.Any(c => c != Card && IsRadiantQuickPlay(c));
        }

        private bool ShouldActivateRadiantQuickPlayForMandate()
        {
            return HasFaceupMandate() && GetOpponentChainOneFieldCard() != null &&
                !Duel.CurrentChain.Any(c => c.Controller == 0 && IsRadiantQuickPlay(c)) &&
                !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.MysticalSpaceTyphoon)) &&
                HasOtherRadiantQuickPlayInHand();
        }

        private bool ForbiddenDropletActivate()
        {
            List<ClientCard> enemyTargets = Enemy.GetMonsters().Where(c => c.IsFaceup() && !c.IsDisabled()).ToList();
            if (enemyTargets.Count == 0)
            {
                return false;
            }

            ChainInfo lastChain = Duel.CurrentChainInfo.LastOrDefault();
            bool respondingToEnemyFieldMonster = lastChain != null && lastChain.ActivatePlayer == 1 &&
                (lastChain.ActivateType & (int)CardType.Monster) != 0 &&
                lastChain.HasLocation(CardLocation.MonsterZone);
            bool respondingToOwnFieldSpell = lastChain != null && lastChain.ActivatePlayer == 0 &&
                (lastChain.ActivateType & (int)CardType.Spell) != 0 &&
                lastChain.HasLocation(CardLocation.SpellZone | CardLocation.FieldZone);
            if (!respondingToEnemyFieldMonster && !respondingToOwnFieldSpell)
            {
                return false;
            }

            return Bot.Hand.Any(c => c != Card && !c.IsCode(CardId.RadiantTyphoonMeghala)) ||
                Bot.GetMonsters().Any(c => c != Card && !c.IsCode(CardId.RadiantTyphoonMeghala)) ||
                Bot.GetSpells().Any(c => c != Card);
        }

        private bool SuperPolymerizationActivate()
        {
            if (Bot.Hand.All(c => c == Card))
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
            ClientCard problem = Util.GetProblematicEnemyMonster(0, true) ?? Util.GetBestEnemyMonster(true, true);
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

        private int GetFusionForPair(ClientCard first, ClientCard second)
        {
            if (Bot.HasInExtra(CardId.GaruraWingsOfResonantLife) && first.Race == second.Race &&
                first.Attribute == second.Attribute && first.Id != second.Id)
            {
                return CardId.GaruraWingsOfResonantLife;
            }
            if (Bot.HasInExtra(CardId.MudragonOfTheSwamp) && first.Attribute == second.Attribute && first.Race != second.Race)
            {
                return CardId.MudragonOfTheSwamp;
            }
            if (Bot.HasInExtra(CardId.FavoriteHEROFlameWingman) && first.Race == second.Race && first.Attribute != second.Attribute)
            {
                return CardId.FavoriteHEROFlameWingman;
            }
            if (Bot.HasInExtra(CardId.FavoriteHEROShiningFlareWingman) &&
                (first.HasType(CardType.Fusion) || second.HasType(CardType.Fusion)))
            {
                return CardId.FavoriteHEROShiningFlareWingman;
            }
            return 0;
        }

        private bool TheFallenTheVirtuousActivate()
        {
            bool canDestroy = Bot.HasInExtra(CardId.AlbionTheBrandedDragon) &&
                Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c =>
                    c.IsFaceup() && !c.IsShouldNotBeTarget() && !c.IsShouldNotBeSpellTrapTarget());
            bool hasEcclesia = Bot.GetMonsters().Concat(Bot.Graveyard)
                .Any(c => c.IsCode(CardId.EcclesiaAndTheDarkDragon));
            bool canRevive = hasEcclesia && (Bot.Graveyard.Count + Enemy.Graveyard.Count > 0);
            if (!canDestroy && !canRevive)
            {
                return false;
            }

            _fallenPreferRevive = !canDestroy && canRevive;
            return true;
        }

        private bool GallantThiefActivate()
        {
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
            // Krosea has an activated hand effect and a separate on-summon trigger.
            // The latter can be offered with ActivateDescription == -1, so location is
            // the reliable discriminator after the server has supplied a legal effect.
            if (Card.Location == CardLocation.Hand)
            {
                return true;
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
            // Its self summon is a SpSummon procedure. Any legal Activate candidate
            // while Meghala is in the monster zone is therefore its deck-summon trigger.
            if (Card.Location != CardLocation.MonsterZone || _usedMeghalaDeckSummon)
            {
                return false;
            }
            _usedMeghalaDeckSummon = true;
            return true;
        }

        private bool SwenActivate()
        {
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
            if (!IsMainPhase() || Bot.GetMonsterCount() >= 5)
            {
                return false;
            }

            // If Meghala is still in hand, its higher-priority executor was not a
            // legal candidate. Keep Swen for a normal summon and its search instead.
            if (Card.IsCode(CardId.RadiantTyphoonSwen) &&
                Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonMeghala)))
            {
                return false;
            }

            // With Swen and Dachs together, preserve Dachs and use Swen first.
            if (Card.IsCode(CardId.RadiantTyphoonDachs) &&
                Bot.Hand.Any(c => c.IsCode(CardId.RadiantTyphoonSwen)))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                _usedMeghalaHandSummon = true;
            }
            return true;
        }

        private bool SmallRadiantNormalSummon()
        {
            return IsMainPhase();
        }

        private bool KroseaNormalSummon()
        {
            if (!IsMainPhase() || _enemyDrollResolved || HasRadiantSpellInHand() ||
                !Bot.GetMonsters().Any(CanUseAsKroseaTribute))
            {
                return false;
            }
            return true;
        }

        private bool GallantThiefSummon()
        {
            return IsMainPhase() && Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() > 0;
        }

        private bool RadiantQuickPlayMstStarterActivate()
        {
            if (Card.Location == CardLocation.Grave || !NeedMstStarter())
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
            if (Card.Location == CardLocation.Grave || IsDescription(Card.Id, 1))
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (Card.IsCode(CardId.RadiantTyphoonAscendance) && !CanActivateAscendanceNow())
            {
                return false;
            }

            if (ShouldActivateRadiantQuickPlayForMandate() && Card.Location == CardLocation.Hand)
            {
                return AcceptRadiantQuickPlayActivation();
            }

            if (Bot.HasInHand(CardId.RadiantTyphoonMeghala))
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
            if (Card.Location != CardLocation.Hand || !ShouldActivateRadiantQuickPlayForMandate())
            {
                return false;
            }
            return CanActivateAscendanceNow() && AcceptRadiantQuickPlayActivation();
        }

        private bool AcceptRadiantQuickPlayActivation()
        {
            _activatedRadiantCardsThisTurn.Add(Card.Id);
            return true;
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
            return Enemy.GetSpellCount() > 0 && !Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon);
        }

        private bool CanActivateAscendanceNow()
        {
            if (!Card.IsCode(CardId.RadiantTyphoonAscendance) || Enemy.GetSpellCount() > 0)
            {
                return true;
            }

            return !Bot.Hand.Concat(Bot.GetSpells()).Any(c => c != Card && IsRadiantQuickPlay(c));
        }

        private bool SeaSpiritSummon()
        {
            if (!IsMainPhase() || !CanSummonSeaSpiritNow())
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
            if (IsDescription(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 0) ||
                IsMarineEidolonSummonTrigger())
            {
                return true;
            }

            if (IsDescription(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 1))
            {
                ClientCard enemyTarget = Util.GetProblematicEnemyMonster(0, true) ?? Util.GetBestEnemyMonster(true, true);
                ClientCard ownTarget = Bot.GetMonsters().Where(c => c.IsFaceup() && IsRadiantMonster(c) && c != Card)
                    .OrderBy(GetMaterialPriority).FirstOrDefault();
                if (ownTarget == null)
                {
                    ownTarget = Card;
                }
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

        private bool PostExpansionNormalSummon()
        {
            if (!IsPostExpansionNormalSummonWindow() || Card == null ||
                !IsRadiantCard(Card) || !Card.IsMonster() ||
                Bot.GetMonsters().Any(c => c.IsCode(Card.Id)))
            {
                return false;
            }

            if (Card.IsCode(CardId.RadiantTyphoonKrosea) && HasRadiantSpellInHand())
            {
                return false;
            }
            return true;
        }

        private bool TotemBirdSummon()
        {
            if (!IsMainPhase())
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
            if (!IsMainPhase() || (Duel.Turn <= 1 && Duel.Phase != DuelPhase.Main2))
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.Level == 9 && c.IsFaceup() &&
                    CanUseAsExtraDeckMaterial(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2 || (Enemy.Hand.Count == 0 && Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0))
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool EnterblathnirActivate()
        {
            return Enemy.Hand.Count > 0 || Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0 || Enemy.Graveyard.Count > 0;
        }

        private bool SPLittleKnightSummon()
        {
            if (!IsMainPhase() || Util.GetProblematicEnemyCard(0, true) == null)
            {
                return false;
            }

            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    c.HasType(CardType.Effect) && CanUseAsLinkMaterial(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool HraesvelgrSummon()
        {
            if (!IsMainPhase())
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
            ClientCard target = Enemy.Graveyard.Where(c => c.IsMonster())
                .OrderByDescending(c => c.Attack).FirstOrDefault();
            if (target == null)
            {
                return false;
            }
            AI.SelectCard(target);
            return true;
        }

        private bool WynnSummon()
        {
            if (!IsMainPhase() || !Enemy.Graveyard.Any(c => c.Attribute == (int)CardAttribute.Wind))
            {
                return false;
            }
            return SelectGenericLinkTwoMaterials(true);
        }

        private bool WynnActivate()
        {
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
            if (Duel.IsFirst || !IsMainPhase() || Duel.Phase != DuelPhase.Main1)
            {
                return false;
            }
            int currentAttack = Bot.GetMonsters().Where(c => c.IsAttack()).Sum(c => Math.Max(0, c.Attack));
            int windAttackers = Bot.GetMonsters().Count(c => c.IsAttack() && c.Attribute == (int)CardAttribute.Wind);
            if (Enemy.GetMonsterCount() > 0 || currentAttack + windAttackers * 500 < Enemy.LifePoints)
            {
                return false;
            }
            return SelectGenericLinkTwoMaterials(true);
        }

        private bool GreatflyActivate()
        {
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

        private bool SelectGenericLinkTwoMaterials(bool requireWind)
        {
            List<ClientCard> materials = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    CanUseAsLinkMaterial(c) &&
                    (!requireWind || c.Attribute == (int)CardAttribute.Wind) && !ShouldWaitForRadiantTrigger(c))
                .OrderBy(GetMaterialPriority).Take(2).ToList();
            if (materials.Count < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool TyphonSummon()
        {
            if (!IsMainPhase())
            {
                return false;
            }
            bool emergency = Util.GetProblematicEnemyMonster(3000, true) != null ||
                Enemy.GetMonsterCount() > Bot.GetMonsterCount() + 1;
            return emergency && (Duel.Phase == DuelPhase.Main2 || !CanContinueRadiantEngine());
        }

        private bool TyphonActivate()
        {
            ClientCard target = Util.GetProblematicEnemyMonster(0, true) ?? Util.GetBestEnemyMonster(false, true);
            if (target == null)
            {
                return false;
            }
            AI.SelectCard(target);
            return true;
        }

        private bool CanContinueRadiantEngine()
        {
            return Bot.Hand.Any(IsRadiantCard) || Bot.GetMonsters().Any(c => IsRadiantMonster(c) && !ShouldWaitForRadiantTrigger(c));
        }

        private bool RadiantSpellSet()
        {
            if (Duel.Phase != DuelPhase.Main2 && Duel.Turn > 1)
            {
                return false;
            }
            return Card.IsCode(CardId.MysticalSpaceTyphoon, CardId.RadiantTyphoonVision,
                CardId.RadiantTyphoonAscendance, CardId.RadiantTyphoonChant,
                CardId.RadiantTyphoonMandate, CardId.ForbiddenDroplet,
                CardId.SuperPolymerization, CardId.TheFallenTheVirtuous,
                CardId.InfiniteImpermanence);
        }

        public override int OnSelectOption(IList<int> options)
        {
            int selected;

            if (ContainsOption(options, CardId.TheFallenTheVirtuous, 1, 2))
            {
                int preferredOffset = _fallenPreferRevive ? 2 : 1;
                selected = GetOptionIndex(options, CardId.TheFallenTheVirtuous, preferredOffset);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonVision, 2, 3))
            {
                bool safeToDiscard = Bot.Hand.Any(c => c != Card && (IsRadiantCard(c) || c.HasType(CardType.QuickPlay)));
                int preferredOffset = (NeedMstStarter() || ShouldPrioritizeMstAgainstBackrow()) ? 3 :
                    (!_enemyDrollResolved && (safeToDiscard || Bot.Hand.Count >= 3) ? 2 : 3);
                selected = GetOptionIndex(options, CardId.RadiantTyphoonVision, preferredOffset);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonChant, 2, 3))
            {
                int preferredOffset = ShouldUseChantMstSearch() ? 2 : 3;
                selected = GetOptionIndex(options, CardId.RadiantTyphoonChant, preferredOffset);
                if (selected >= 0)
                {
                    return selected;
                }
            }

            if (ContainsOption(options, CardId.RadiantTyphoonAscendance, 2, 3))
            {
                int preferredOffset = (NeedMstStarter() || ShouldPrioritizeMstAgainstBackrow()) ? 3 :
                    (!_enemyDrollResolved && HasRadiantReviveTarget() && Bot.GetMonsterCount() < 5 ? 2 : 3);
                selected = GetOptionIndex(options, CardId.RadiantTyphoonAscendance, preferredOffset);
                if (selected >= 0)
                {
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
            }

            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
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
            return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);
        }

        public override IList<ClientCard> OnSelectSynchroMaterial(IList<ClientCard> cards, int sum, int min, int max)
        {
            return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            return SelectExtraDeckMaterialsWithoutMeghala(cards, min, max);
        }

        public override IList<ClientCard> OnSelectLinkMaterial(IList<ClientCard> cards, int min, int max)
        {
            return SelectLinkMaterials(cards, min, max);
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

            if (Card != null && Card.IsCode(CardId.TheWorldsGreatestGallantThief) &&
                (hint == HintMsg.Release || hint == HintMsg.Tribute))
            {
                List<ClientCard> tributePriority = new List<ClientCard>();
                tributePriority.AddRange(GetOrderedEnemyCards(cards.Where(c => c.Controller == 1)));
                tributePriority.AddRange(cards.Where(c => c.Controller == 0).OrderBy(GetDiscardPriority));
                return SelectCount(tributePriority, cards, min, max, Math.Max(min, 2));
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
                ClientCard problem = Util.GetProblematicEnemyMonster(0, true);
                int targetCount = Enemy.GetMonsters().Count(c => c.IsFaceup() && !c.IsDisabled() &&
                    (c == problem || c.Attack >= 2500));
                int desired = Math.Min(max, Math.Max(min, Math.Min(2, Math.Max(1, targetCount))));
                List<ClientCard> costs = cards.Where(c => c.Controller == 0 &&
                        !c.IsCode(CardId.RadiantTyphoonMeghala))
                    .OrderBy(GetDropletCostPriority).ToList();
                _dropletCostCount = desired;
                return SelectCount(costs, cards, min, max, desired);
            }

            if (chain.IsActivateCode(CardId.TheFallenTheVirtuous))
            {
                if (_fallenPreferRevive && (hint == HintMsg.SpSummon || hint == HintMsg.Target))
                {
                    List<ClientCard> targets = cards.OrderByDescending(c => Math.Max(c.Attack, c.Defense)).ToList();
                    return SelectCount(targets, cards, min, max, 1);
                }
                if (hint == HintMsg.ToGrave)
                {
                    return SelectByIds(cards, min, max, 1, CardId.AlbionTheBrandedDragon);
                }
                if (hint == HintMsg.Destroy || hint == HintMsg.Target)
                {
                    List<ClientCard> targets = GetOrderedEnemyCards(cards.Where(c => c.Controller == 1 && c.IsFaceup()));
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
                ClientCard chainOneTarget = GetOpponentChainOneFieldCard();
                if (chainOneTarget != null && cards.Contains(chainOneTarget))
                {
                    return new List<ClientCard> { chainOneTarget };
                }
            }

            if (chain.IsActivateCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) &&
                chain.ActivateDescription == Util.GetStringId(CardId.RadiantTyphoonVaruroonTheMarineEidolon, 1))
            {
                ClientCard ownTarget = cards.Where(c => c.Controller == 0 && IsRadiantMonster(c))
                    .OrderBy(GetMaterialPriority).FirstOrDefault();
                ClientCard enemyTarget = GetOrderedEnemyCards(cards.Where(c => c.Controller == 1)).FirstOrDefault();
                if (ownTarget != null && enemyTarget != null)
                {
                    return new List<ClientCard> { ownTarget, enemyTarget };
                }
            }

            return null;
        }

        private IList<ClientCard> SelectResolutionCard(ChainInfo chain, IList<ClientCard> cards, int min, int max, int hint)
        {
            if (chain.IsActivateCode(CardId.ForbiddenDroplet))
            {
                int desired = _dropletCostCount > 0 ? _dropletCostCount : min;
                List<ClientCard> targets = GetOrderedEnemyCards(cards.Where(c => c.Controller == 1));
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
            if (chain.IsActivateCode(CardId.RadiantTyphoonVision) && hint == HintMsg.Discard)
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
            if (chain.IsActivateCode(CardId.TheFallenTheVirtuous) && _fallenPreferRevive)
            {
                List<ClientCard> targets = cards.OrderByDescending(c => Math.Max(c.Attack, c.Defense)).ToList();
                return SelectCount(targets, cards, min, max, 1);
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
                CardId.RadiantTyphoonMandate,
                CardId.MysticalSpaceTyphoon);
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

            List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
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
            if ((ShouldUseChantMstSearch() || cards.All(c => c.IsCode(CardId.MysticalSpaceTyphoon))) &&
                cards.Any(c => c.IsCode(CardId.MysticalSpaceTyphoon)))
            {
                return SelectMstForHand(cards, min, max);
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
            if (cards.Any(c => c.IsMonster()))
            {
            List<int> priority = BuildSearchPriorityWithoutFieldMeghala(
                    CardId.RadiantTyphoonSwen,
                    CardId.RadiantTyphoonDachs,
                    CardId.RadiantTyphoonKrosea,
                    CardId.RadiantTyphoonMeghala);
                return SelectByIds(cards, min, max, 1, priority.ToArray());
            }
            return SelectMstForHand(cards, min, max);
        }

        private IList<ClientCard> SelectVisionDiscard(IList<ClientCard> cards, int min, int max)
        {
            // Vision draws two cards and then asks for one discard. Keep this
            // ranking in one method so later hand-value rules can be added
            // without changing the resolution/selection routing above.
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

            // A card already used this turn is the first discard candidate,
            // including cards whose once-per-turn flag is tracked separately
            // from the activation-id set (Swen, Dachs, Krosea and Meghala).
            if (WasRadiantEffectUsedThisTurn(card.Id))
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
            List<ClientCard> priority = new List<ClientCard>();
            priority.AddRange(cards.Where(IsRadiantQuickPlay));
            priority.AddRange(cards.Where(c => c.IsCode(CardId.TheFallenTheVirtuous)));
            priority.AddRange(cards.Where(c => c.IsCode(CardId.SuperPolymerization)));
            priority.AddRange(cards.Where(c => c.IsCode(CardId.ForbiddenDroplet)));
            priority.AddRange(cards.Where(c => !c.IsCode(CardId.MysticalSpaceTyphoon) && !priority.Contains(c)));
            // MST is always recycled last. Selecting exactly three naturally keeps
            // at least one in the graveyard whenever enough alternatives exist.
            priority.AddRange(cards.Where(c => c.IsCode(CardId.MysticalSpaceTyphoon)));
            return SelectCount(priority, cards, min, max, Math.Max(min, 3));
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
                _marineTrapPlacementResolved)
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

        private bool ShouldPrioritizeGallantThiefSummon()
        {
            return Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() > 0 &&
                Bot.HasInHand(CardId.TheWorldsGreatestGallantThief);
        }

        private bool NeedMstStarter()
        {
            return !_enemyDrollResolved && Enemy.GetSpellCount() > 0 &&
                !Bot.HasInHand(CardId.MysticalSpaceTyphoon) &&
                !Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon);
        }

        private bool CanSpecialSummonMeghalaFromHandNow()
        {
            if (!IsMainPhase() || _usedMeghalaHandSummon || Bot.GetMonsterCount() >= 5 ||
                !Bot.HasInHand(CardId.RadiantTyphoonMeghala))
            {
                return false;
            }
            return Bot.HasInGraveyard(CardId.MysticalSpaceTyphoon) || Enemy.GetSpellCount() == 0;
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

                bool unused = !WasRadiantEffectUsedThisTurn(id);
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

        private bool CanUseAsKroseaTribute(ClientCard card)
        {
            return card != null && card.Controller == 0 &&
                card.Location == CardLocation.MonsterZone && card.IsMonster() &&
                !card.HasType(CardType.Link) && !card.HasType(CardType.Xyz);
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
                return _usedMeghalaDeckSummon;
            }
            return _activatedRadiantCardsThisTurn.Contains(cardId);
        }

        private bool HasRadiantReviveTarget()
        {
            return Bot.Graveyard.Any(c => IsRadiantCard(c) && c.IsMonster() && c.Level <= 6);
        }

        private bool NeedRadiantStarter()
        {
            return !HasAccessible(CardId.RadiantTyphoonSwen) ||
                !HasAccessible(CardId.RadiantTyphoonDachs) ||
                Bot.GetMonsterCount() == 0;
        }

        private bool ShouldWaitForRadiantTrigger(ClientCard card)
        {
            if (card == null || card.IsDisabled() || _enemyDrollResolved)
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

        private bool CanSummonSeaSpiritNow()
        {
            if (_seaSpiritSummoned || !Bot.HasInExtra(CardId.RadiantTyphoonVaruroonTheMarineEidolon))
            {
                return false;
            }
            return Bot.GetMonsters().Count(c => IsRadiantMonster(c) && CanUseAsLinkMaterial(c) &&
                !ShouldWaitForRadiantTrigger(c)) >= 2;
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
            return !card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) ||
                _marineTrapPlacementResolved;
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
            if (card.IsCode(CardId.RadiantTyphoonVaruroonTheMarineEidolon) && Bot.HasInSpellZone(CardId.RadiantTyphoonMandate))
            {
                return 5;
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
            if (card == null || card.IsCode(CardId.RadiantTyphoonMeghala))
            {
                return 1000;
            }
            if (card.IsSpell() && (card.Location == CardLocation.SpellZone ||
                card.Location == CardLocation.FieldZone))
            {
                return 0;
            }
            if (card.Location == CardLocation.MonsterZone && (card.Level == 3 || card.Level == 6))
            {
                if (card.IsCode(CardId.RadiantTyphoonSwen) && _usedSwenSearch)
                {
                    return 10;
                }
                if (card.IsCode(CardId.RadiantTyphoonDachs) && _usedDachsSearch)
                {
                    return 11;
                }
                if (card.IsCode(CardId.RadiantTyphoonKrosea) && _usedKroseaSearch)
                {
                    return 12;
                }
            }
            return 100 + GetDiscardPriority(card);
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
