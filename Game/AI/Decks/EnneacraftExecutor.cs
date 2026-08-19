using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Enneacraft", "AI_Enneacraft")]
    class EnneacraftExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Atori = 54842941;
            public const int Deftero = 92171126;
            public const int Ekto = 29570824;
            public const int Proto = 56187077;
            public const int Trito = 44716748;
            public const int Enato = 55965529;
            public const int Aiza = 82359538;
            public const int Asta = 28454232;
            public const int Archa = 81237046;
            public const int Atil = 71801447;
            public const int Release = 54020393;
            public const int Reverth = 80015408;
            public const int Enneapolis = 17621695;
            public const int Reset = 19504025;
            public const int MaxxC = 23434538;
        }

        private const int EnneacraftSetcode = 0x1d4;
        private const int PendulumActivateDescription = 1160;

        private static readonly int[] Level1Ids =
        {
            CardId.Deftero,
            CardId.Ekto,
            CardId.Proto,
            CardId.Trito,
            CardId.Enato
        };

        private static readonly int[] Level9Ids =
        {
            CardId.Atori,
            CardId.Aiza,
            CardId.Asta,
            CardId.Archa,
            CardId.Atil
        };

        private static readonly int[] MonsterIds =
        {
            CardId.Atori,
            CardId.Deftero,
            CardId.Ekto,
            CardId.Proto,
            CardId.Trito,
            CardId.Enato,
            CardId.Aiza,
            CardId.Asta,
            CardId.Archa,
            CardId.Atil
        };

        private static readonly int[] CoreFourIds =
        {
            CardId.Atori,
            CardId.Deftero,
            CardId.Asta,
            CardId.Proto
        };

        private static readonly int[] RouteLevel1Priority =
        {
            CardId.Trito,
            CardId.Enato,
            CardId.Ekto,
            CardId.Deftero,
            CardId.Proto
        };

        private static readonly int[] BonusPriority =
        {
            CardId.Aiza,
            CardId.Archa,
            CardId.Atil,
            CardId.Trito,
            CardId.Enato,
            CardId.Ekto,
            CardId.Deftero,
            CardId.Proto,
            CardId.Atori,
            CardId.Asta
        };

        private enum TurnPlanStep
        {
            None,
            OpeningPreSearch,
            OpeningPreScale,
            OpeningFieldAccess,
            OpeningPolisReturn,
            OpeningPolisReturnPending,
            OpeningPostSearch,
            OpeningReverth,
            OpeningBoard,
            OpeningFinalScales,
            RecoverySearch,
            RecoveryReverth,
            RecoveryFieldAccess,
            RecoveryScale,
            RecoveryFlip,
            RecoveryPolisReturn,
            RecoveryBoard,
            RecoveryFinalScales,
            Complete
        }

        private enum SelectionMode
        {
            None,
            RevealThree,
            SearchOne,
            ScaleFromDeck,
            RecoverFromExtra,
            SpecialSet,
            ReverthShuffle,
            EnneapolisOpeningReturn,
            EnneapolisRecoveryReturn,
            EnneapolisFromDeckOrGrave,
            ResetSetFaceup,
            ReverthFlip,
            EnemyMonster,
            EnemySpellTrap,
            EnemyGraveOrBanished,
            AizaReturn
        }

        private enum OptionMode
        {
            None,
            ReverthGrave,
            EnneapolisDestination
        }

        private enum EnneapolisDestination
        {
            Hand,
            PendulumZone
        }

        private int ownTurnCount;
        private readonly HashSet<int> pendulumSearchAttempted = new HashSet<int>();
        private bool resetMainAttempted;
        private bool enneapolisReturnAttempted;
        private bool openingPairReturned;
        private int postReturnSearchAttempts;
        private bool reverthMainAttempted;
        private bool releaseMainAttempted;
        private bool releaseGravePending;
        private bool releaseGraveSelectionMade;
        private bool recoveryFlipStarted;
        private bool recoveryHadFacedownAtTurnStart;
        private bool recoveryReturnPending;
        private bool recoveryReturnResolved;
        private bool finalScalePhaseStarted;
        private bool openingReturnPending;
        private int scaleRouteInterruptions;
        private bool scaleRouteAborted;
        private bool searchResolutionPending;
        private int searchResolutionSourceId;
        private int searchResolutionMovedId;
        private ClientCard searchResolutionSourceCard;

        private SelectionMode selectionMode;
        private OptionMode optionMode;
        private EnneapolisDestination enneapolisDestination;
        private int desiredSearchId;
        private bool pendingAstaOptionalBanish;
        private bool pendingTritoSpecialSet;
        private bool reverthGravePending;

        private readonly HashSet<ClientCard> monstersSetThisTurn = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> unresolvedRouteScalePlacements = new HashSet<ClientCard>();
        private readonly Dictionary<int, int> openingReturnExpected = new Dictionary<int, int>();
        private int openingReturnMovedCount;
        private readonly Dictionary<int, int> recoveryReturnExpected = new Dictionary<int, int>();
        private int recoveryReturnMovedCount;
        private readonly Dictionary<int, int> enemyGraveBehaviorScores = new Dictionary<int, int>();
        private readonly Dictionary<int, int> enemyGraveLastActivityTurn = new Dictionary<int, int>();

        public EnneacraftExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);

            foreach (int id in Level9Ids)
                AddExecutor(ExecutorType.Activate, id, Level9ReactiveEffect);
            foreach (int id in Level1Ids)
                AddExecutor(ExecutorType.Activate, id, Level1ReactiveEffect);
            foreach (int id in Level9Ids)
                AddExecutor(ExecutorType.Activate, id, Level9PendulumEffect);

            AddExecutor(ExecutorType.Activate, CardId.Reset, ResetGraveEffect);
            AddExecutor(ExecutorType.Activate, CardId.Reverth, ReverthGraveEffect);

            foreach (int id in Level1Ids)
                AddExecutor(ExecutorType.Activate, id, Level1PendulumSearch);
            foreach (int id in MonsterIds)
                AddExecutor(ExecutorType.Activate, id, RoutePendulumPlacement);

            AddExecutor(ExecutorType.Activate, CardId.Release, ReleaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.Reset, ResetMainEffect);
            AddExecutor(ExecutorType.Activate, CardId.Enneapolis, EnneapolisPostFlipEffect);
            AddExecutor(ExecutorType.Activate, CardId.Enneapolis, EnneapolisMainEffect);
            AddExecutor(ExecutorType.Activate, CardId.Reverth, ReverthMainEffect);

            foreach (int id in MonsterIds)
                AddExecutor(ExecutorType.Activate, id, HandSpecialSetEffect);

            AddExecutor(ExecutorType.Repos, OwnTurnFlip);
            AddExecutor(ExecutorType.MonsterSet, FallbackMonsterSet);

            foreach (int id in MonsterIds)
                AddExecutor(ExecutorType.Activate, id, FinalPendulumPlacement);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            if (Duel.Player == 0)
            {
                ownTurnCount++;
                monstersSetThisTurn.Clear();
                resetMainAttempted = false;
                enneapolisReturnAttempted = false;
                reverthMainAttempted = false;
                releaseMainAttempted = false;
                releaseGravePending = false;
                releaseGraveSelectionMade = false;
                recoveryFlipStarted = false;
                recoveryReturnPending = false;
                recoveryReturnResolved = false;
                recoveryReturnExpected.Clear();
                recoveryReturnMovedCount = 0;
                finalScalePhaseStarted = false;
                openingReturnPending = false;
                scaleRouteInterruptions = 0;
                scaleRouteAborted = false;
                unresolvedRouteScalePlacements.Clear();
                searchResolutionPending = false;
                searchResolutionSourceId = 0;
                searchResolutionMovedId = 0;
                searchResolutionSourceCard = null;
                recoveryHadFacedownAtTurnStart = ownTurnCount >= 2
                    && GetFacedownEnneacraftMonsters().Any();

                if (ownTurnCount == 1)
                {
                    openingPairReturned = false;
                    postReturnSearchAttempts = 0;
                    openingReturnExpected.Clear();
                    openingReturnMovedCount = 0;
                }
            }

            pendulumSearchAttempted.Clear();
            selectionMode = SelectionMode.None;
            optionMode = OptionMode.None;
            desiredSearchId = 0;
            pendingAstaOptionalBanish = false;
            pendingTritoSpecialSet = false;
            reverthGravePending = false;

            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 1 && card != null && card.Id != 0)
            {
                AddEnemyGraveBehaviorScore(card.Id, 8);

                if ((card.Location & CardLocation.Grave) != 0)
                    AddEnemyGraveBehaviorScore(card.Id, 220);
                else if ((card.Location & CardLocation.Removed) != 0)
                    AddEnemyGraveBehaviorScore(card.Id, 240);
            }

            base.OnChaining(player, card);
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            TrackEnemyGraveBehavior(card, previousControler, previousLocation,
                currentControler, currentLocation);

            bool expectedOpeningReturn = card != null && openingReturnPending
                && previousControler == 0 && currentControler == 0
                && (previousLocation & (int)(CardLocation.Onfield
                    | CardLocation.PendulumZone | CardLocation.FieldZone)) != 0
                && (currentLocation & (int)CardLocation.Hand) != 0
                && openingReturnExpected.ContainsKey(card.Id)
                && openingReturnExpected[card.Id] > 0;

            if (card != null && unresolvedRouteScalePlacements.Contains(card)
                && previousControler == 0
                && (previousLocation & (int)(CardLocation.SpellZone
                    | CardLocation.PendulumZone)) != 0
                && (currentControler != 0
                    || (currentLocation & (int)(CardLocation.SpellZone
                        | CardLocation.PendulumZone)) == 0))
            {
                unresolvedRouteScalePlacements.Remove(card);
                if (IsOwnMainPhase() && !expectedOpeningReturn)
                    RegisterScaleRouteInterruption(card);
            }

            if (searchResolutionPending && card != null && card.Id != 0
                && previousControler == 0 && currentControler == 0
                && (previousLocation & (int)CardLocation.Deck) != 0
                && (currentLocation & (int)CardLocation.Hand) != 0)
            {
                searchResolutionMovedId = card.Id;
            }

            if (card != null && card.Id != 0
                && previousControler == 0 && currentControler == 0
                && (previousLocation & (int)CardLocation.Hand) != 0
                && (currentLocation & (int)CardLocation.MonsterZone) != 0
                && IsEnneacraftMonster(card))
            {
                monstersSetThisTurn.Add(card);
            }

            if (card != null
                && previousControler == 0
                && (previousLocation & (int)CardLocation.MonsterZone) != 0
                && (currentControler != 0
                    || (currentLocation & (int)CardLocation.MonsterZone) == 0))
            {
                monstersSetThisTurn.Remove(card);
            }

            if (card != null && openingReturnExpected.Count > 0
                && previousControler == 0 && currentControler == 0
                && (previousLocation & (int)(CardLocation.Onfield
                    | CardLocation.PendulumZone | CardLocation.FieldZone)) != 0
                && (currentLocation & (int)CardLocation.Hand) != 0)
            {
                int remaining;
                if (openingReturnExpected.TryGetValue(card.Id, out remaining) && remaining > 0)
                {
                    openingReturnExpected[card.Id] = remaining - 1;
                    openingReturnMovedCount++;
                }
            }

            if (card != null && recoveryReturnExpected.Count > 0
                && previousControler == 0 && currentControler == 0
                && (previousLocation & (int)CardLocation.MonsterZone) != 0
                && (currentLocation & (int)CardLocation.Hand) != 0)
            {
                int remaining;
                if (recoveryReturnExpected.TryGetValue(card.Id, out remaining) && remaining > 0)
                {
                    recoveryReturnExpected[card.Id] = remaining - 1;
                    recoveryReturnMovedCount++;
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnChainEnd()
        {
            if (openingReturnPending)
            {
                if (openingReturnMovedCount >= 2)
                {
                    openingPairReturned = true;
                }
                else
                {
                    openingPairReturned = false;
                    enneapolisReturnAttempted = false;
                    finalScalePhaseStarted = false;
                    scaleRouteAborted = true;
                }

                openingReturnPending = false;
                openingReturnExpected.Clear();
                openingReturnMovedCount = 0;
            }

            if (recoveryReturnPending)
            {
                recoveryReturnResolved = recoveryReturnMovedCount > 0;
                recoveryReturnPending = false;
                recoveryReturnExpected.Clear();
                recoveryReturnMovedCount = 0;
            }

            if (searchResolutionPending)
            {
                if (searchResolutionMovedId != 0)
                    unresolvedRouteScalePlacements.Remove(searchResolutionSourceCard);
                searchResolutionPending = false;
                searchResolutionSourceId = 0;
                searchResolutionMovedId = 0;
                searchResolutionSourceCard = null;
            }

            if (releaseGravePending)
            {
                releaseGravePending = false;
                releaseGraveSelectionMade = false;
            }

            selectionMode = SelectionMode.None;
            desiredSearchId = 0;
            optionMode = OptionMode.None;
            pendingAstaOptionalBanish = false;
            pendingTritoSpecialSet = false;
            reverthGravePending = false;
            base.OnChainEnd();
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max,
            int hint, bool cancelable)
        {
            if (cards == null || cards.Count == 0)
                return base.OnSelectCard(cards, min, max, hint, cancelable);

            IList<ClientCard> result = null;
            SelectionMode mode = selectionMode;
            ClientCard solvingCard = Duel.GetCurrentSolvingChainCard();

            if (solvingCard != null && solvingCard.Controller == 0)
            {
                if (solvingCard.IsCode(CardId.Enneapolis))
                {
                    result = SelectEnneapolisFlippedMonster(cards, min, max);
                    return Checked(result, cards, min, max);
                }

                result = SelectFlipEffectCards(solvingCard, cards, min, max);
                if (result != null)
                    return Checked(result, cards, min, max);

                if (solvingCard.IsCode(CardId.Trito) && pendingTritoSpecialSet
                    && cards.Any(c => c != null && c.Controller == 0
                        && c.Location == CardLocation.Hand && IsEnneacraftMonster(c)))
                {
                    result = SelectFaceDownSPSummonMonster(cards, min, max);
                    pendingTritoSpecialSet = false;
                    return Checked(result, cards, min, max);
                }
            }

            if (mode == SelectionMode.None && reverthGravePending)
            {
                if (cards.Any(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.Hand && IsEnneacraftMonster(c)))
                {
                    result = SelectFaceDownSPSummonMonster(cards, min, max);
                    reverthGravePending = false;
                    return Checked(result, cards, min, max);
                }
                if (cards.Any(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone && c.IsFacedown()))
                {
                    result = SelectReverthFlipTarget(cards, min, max);
                    reverthGravePending = false;
                    return Checked(result, cards, min, max);
                }
            }

            if (mode == SelectionMode.None)
                return base.OnSelectCard(cards, min, max, hint, cancelable);

            switch (mode)
            {
                case SelectionMode.RevealThree:
                    result = SelectRevealThree(cards, min, max);
                    break;
                case SelectionMode.SearchOne:
                    result = SelectSearchOne(cards, min, max);
                    break;
                case SelectionMode.ScaleFromDeck:
                    result = SelectScaleFromDeck(cards, min, max);
                    break;
                case SelectionMode.RecoverFromExtra:
                    result = SelectRecoverFromExtra(cards, min, max);
                    break;
                case SelectionMode.SpecialSet:
                    result = SelectFaceDownSPSummonMonster(cards, min, max);
                    break;
                case SelectionMode.ReverthShuffle:
                    result = SelectReverthShuffle(cards, min, max);
                    break;
                case SelectionMode.EnneapolisOpeningReturn:
                    result = SelectOpeningPolisReturn(cards, min, max);
                    break;
                case SelectionMode.EnneapolisRecoveryReturn:
                    result = SelectRecoveryPolisReturn(cards, min, max);
                    break;
                case SelectionMode.EnneapolisFromDeckOrGrave:
                    result = SelectByPriority(cards, new[] { CardId.Enneapolis }, min, max);
                    break;
                case SelectionMode.ResetSetFaceup:
                    result = SelectResetSetTargets(cards, min, max);
                    break;
                case SelectionMode.ReverthFlip:
                    result = SelectReverthFlipTarget(cards, min, max);
                    break;
                case SelectionMode.EnemyMonster:
                    result = SelectEnemyMonster(cards, min, max);
                    break;
                case SelectionMode.EnemySpellTrap:
                    result = SelectEnemySpellTrap(cards, min, max);
                    break;
                case SelectionMode.EnemyGraveOrBanished:
                    result = SelectEnemyGraveOrBanished(cards, min, max);
                    break;
                case SelectionMode.AizaReturn:
                    result = SelectAizaReturn(cards, min, max);
                    break;
            }

            selectionMode = SelectionMode.None;
            desiredSearchId = 0;

            return Checked(result, cards, min, max);
        }

        public override int OnSelectOption(IList<int> options)
        {
            if (options == null || options.Count == 0)
                return base.OnSelectOption(options);

            if (optionMode == OptionMode.ReverthGrave)
            {
                int setDesc = Util.GetStringId(CardId.Reverth, 2);
                int flipDesc = Util.GetStringId(CardId.Reverth, 3);
                int setIndex = IndexOfOption(options, setDesc);
                int flipIndex = IndexOfOption(options, flipDesc);
                bool preferSet = OpenMonsterZones() > 0
                    && Bot.Hand.Any(c => c != null && IsCore(c.Id) && !HasCoreOnMonsterZone(c.Id));
                int index;
                bool chooseSet;

                if (preferSet && setIndex >= 0)
                {
                    index = setIndex;
                    chooseSet = true;
                }
                else if (flipIndex >= 0)
                {
                    index = flipIndex;
                    chooseSet = false;
                }
                else
                {
                    index = setIndex >= 0 ? setIndex : 0;
                    chooseSet = setIndex >= 0;
                }

                optionMode = OptionMode.None;
                reverthGravePending = false;
                selectionMode = chooseSet ? SelectionMode.SpecialSet : SelectionMode.ReverthFlip;
                return index;
            }

            if (optionMode == OptionMode.EnneapolisDestination)
            {
                int wanted = Util.GetStringId(CardId.Enneapolis,
                    enneapolisDestination == EnneapolisDestination.PendulumZone ? 3 : 4);
                int index = IndexOfOption(options, wanted);
                optionMode = OptionMode.None;
                if (index >= 0)
                    return index;
            }

            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (pendingAstaOptionalBanish)
            {
                pendingAstaOptionalBanish = false;
                return true;
            }
            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (IsEnneacraftMonsterId(cardId) && positions != null
                && positions.Contains(CardPosition.FaceDownDefence))
                return CardPosition.FaceDownDefence;
            return base.OnSelectPosition(cardId, positions);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                int preferred = PreferredMonsterZone(cardId);
                int bit = 1 << preferred;
                if ((available & bit) != 0)
                    return bit;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        private bool Level1ReactiveEffect()
        {
            if (Card == null || !IsLevel1(Card.Id)
                || Card.Location != CardLocation.MonsterZone || !Card.IsFacedown())
                return false;
            if (ActivateDescription != Util.GetStringId(Card.Id, 2))
                return false;

            selectionMode = SelectionMode.SearchOne;
            desiredSearchId = ChooseReactiveSearchTarget();
            searchResolutionPending = true;
            searchResolutionSourceId = Card.Id;
            searchResolutionMovedId = 0;
            searchResolutionSourceCard = Card;
            return true;
        }

        private bool Level9ReactiveEffect()
        {
            if (Card == null || !IsLevel9(Card.Id)
                || Card.Location != CardLocation.MonsterZone || !Card.IsFacedown())
                return false;
            if (ActivateDescription != Util.GetStringId(Card.Id, 2))
                return false;

            if (Card.IsCode(CardId.Aiza))
                selectionMode = SelectionMode.AizaReturn;
            if (Card.IsCode(CardId.Asta))
                pendingAstaOptionalBanish = true;

            return true;
        }

        private bool Level9PendulumEffect()
        {
            if (Card == null || !IsLevel9(Card.Id) || !IsPendulumScale(Card)
                || ActivateDescription != Util.GetStringId(Card.Id, 0))
                return false;
            if (!Enemy.GetMonsters().Any())
                return false;

            selectionMode = SelectionMode.EnemyMonster;
            return true;
        }

        private bool ResetGraveEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Reset)
                || Card.Location != CardLocation.Grave
                || ActivateDescription != Util.GetStringId(CardId.Reset, 1))
                return false;
            if (PendulumScaleCount() == 0
                || !GetFaceupEnneacraftMonsters().Any())
                return false;

            selectionMode = SelectionMode.ResetSetFaceup;
            return true;
        }

        private bool ReverthGraveEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Reverth)
                || Card.Location != CardLocation.Grave)
                return false;
            bool canSet = OpenMonsterZones() > 0 && Bot.Hand.Any(IsEnneacraftMonster);
            bool canFlip = GetFacedownEnneacraftMonsters().Any();
            if (!canSet && !canFlip)
                return false;

            optionMode = OptionMode.ReverthGrave;
            reverthGravePending = true;
            return true;
        }

        private bool EnneapolisPostFlipEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Enneapolis)
                || !IsCurrentFieldSpell(Card)
                || (ActivateDescription != -1
                    && ActivateDescription != Util.GetStringId(CardId.Enneapolis, 1)))
                return false;


            if (Duel.Player == 0 && ownTurnCount >= 2 && recoveryFlipStarted)
                return false;

            return true;
        }

        private bool EnneapolisMainEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Enneapolis))
                return false;

            if (!IsCurrentFieldSpell(Card) || Card.IsFacedown())
            {
                if (!IsSpellActivationFromHandOrSet(Card) || IsEnneapolisOnField())
                    return false;
                if (!IsOwnMainPhase())
                    return false;
                TurnPlanStep fieldStep = GetTurnPlanStep();
                if (fieldStep != TurnPlanStep.OpeningFieldAccess
                    && fieldStep != TurnPlanStep.RecoveryFieldAccess)
                    return false;

                return true;
            }

            if (ActivateDescription != Util.GetStringId(CardId.Enneapolis, 0)
                || !IsOwnMainPhase() || enneapolisReturnAttempted)
                return false;

            TurnPlanStep plan = GetTurnPlanStep();
            if (plan == TurnPlanStep.OpeningPolisReturn)
            {
                enneapolisReturnAttempted = true;
                openingReturnPending = true;
                openingReturnExpected.Clear();
                openingReturnMovedCount = 0;
                selectionMode = SelectionMode.EnneapolisOpeningReturn;
                return true;
            }

            if (plan == TurnPlanStep.RecoveryPolisReturn)
            {
                enneapolisReturnAttempted = true;
                recoveryReturnPending = true;
                recoveryReturnResolved = false;
                selectionMode = SelectionMode.EnneapolisRecoveryReturn;
                return true;
            }

            return false;
        }

        private bool Level1PendulumSearch()
        {
            if (Card == null || !IsLevel1(Card.Id) || !IsPendulumScale(Card))
                return false;
            if (ActivateDescription != Util.GetStringId(Card.Id, 0))
                return false;
            if (!IsOwnMainPhase() || Bot.LifePoints <= 900
                || pendulumSearchAttempted.Contains(Card.Id))
                return false;

            TurnPlanStep step = GetTurnPlanStep();
            if (step != TurnPlanStep.OpeningPreSearch
                && step != TurnPlanStep.OpeningPostSearch
                && step != TurnPlanStep.RecoverySearch)
                return false;

            desiredSearchId = ChoosePendulumSearchTarget(Card.Id);
            selectionMode = SelectionMode.RevealThree;
            pendulumSearchAttempted.Add(Card.Id);
            if (step == TurnPlanStep.OpeningPostSearch)
                postReturnSearchAttempts++;
            searchResolutionPending = true;
            searchResolutionSourceId = Card.Id;
            searchResolutionMovedId = 0;
            searchResolutionSourceCard = Card;
            return true;
        }

        private bool RoutePendulumPlacement()
        {
            if (!IsOwnMainPhase() || !IsPendulumPlacementPrompt(Card)
                || Card == null || !IsLevel1(Card.Id)
                || PendulumScaleCount() >= 2 || ShouldPrioritizeBoardOverScales())
                return false;
            if (Object.ReferenceEquals(Card, GetProtectedBoardAnchor(Bot.Hand)))
                return false;

            TurnPlanStep step = GetTurnPlanStep();
            if (step != TurnPlanStep.OpeningPreScale
                && step != TurnPlanStep.OpeningPostSearch
                && step != TurnPlanStep.RecoveryScale)
                return false;

            List<ClientCard> candidates;
            if (step == TurnPlanStep.OpeningPreScale)
                candidates = GetPreFieldScaleCandidates();
            else if (step == TurnPlanStep.OpeningPostSearch)
                candidates = GetPostReturnScaleCandidates();
            else
                candidates = GetRecoveryRouteScaleCandidates();

            if (candidates.Count == 0 || !Object.ReferenceEquals(candidates[0], Card))
                return false;

            unresolvedRouteScalePlacements.Add(Card);
            return true;
        }

        private bool ReleaseEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Release))
                return false;

            if (Card.Location == CardLocation.Grave
                && ActivateDescription == Util.GetStringId(CardId.Release, 1))
            {
                if (!IsOwnMainPhase() || releaseGravePending || !NeedLevel1Resource())
                    return false;
                if (NeedsBoardAnchor() && Bot.Hand.Any(IsEnneacraftMonster))
                    return false;
                TurnPlanStep graveStep = GetTurnPlanStep();
                if (graveStep != TurnPlanStep.OpeningPostSearch
                    && graveStep != TurnPlanStep.RecoverySearch
                    && graveStep != TurnPlanStep.RecoveryScale)
                    return false;

                releaseGravePending = true;
                releaseGraveSelectionMade = false;
                selectionMode = SelectionMode.RecoverFromExtra;
                return true;
            }

            if (!IsSpellActivationFromHandOrSet(Card)
                || ActivateDescription != Util.GetStringId(CardId.Release, 0))
                return false;
            if (!IsOwnMainPhase() || releaseMainAttempted || PendulumScaleCount() >= 2
                || ShouldPrioritizeBoardOverScales())
                return false;
            TurnPlanStep mainStep = GetTurnPlanStep();
            if (mainStep != TurnPlanStep.OpeningPreScale
                && mainStep != TurnPlanStep.OpeningPostSearch
                && mainStep != TurnPlanStep.RecoveryScale)
                return false;
            if (!ShouldUseReleaseForScale())
                return false;

            releaseMainAttempted = true;
            selectionMode = SelectionMode.ScaleFromDeck;
            return true;
        }

        private bool ResetMainEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Reset)
                || !IsSpellActivationFromHandOrSet(Card)
                || ActivateDescription != Util.GetStringId(CardId.Reset, 0))
                return false;
            if (!IsOwnMainPhase() || resetMainAttempted || IsEnneapolisOnField())
                return false;
            if (GetTurnPlanStep() != TurnPlanStep.OpeningFieldAccess
                && GetTurnPlanStep() != TurnPlanStep.RecoveryFieldAccess)
                return false;

            resetMainAttempted = true;
            selectionMode = SelectionMode.EnneapolisFromDeckOrGrave;
            return true;
        }

        private bool ReverthMainEffect()
        {
            if (Card == null || !Card.IsCode(CardId.Reverth)
                || !IsSpellActivationFromHandOrSet(Card)
                || ActivateDescription != Util.GetStringId(CardId.Reverth, 0))
                return false;
            if (!IsOwnMainPhase() || reverthMainAttempted)
                return false;

            IList<ClientCard> safe = BuildSafeReverthSelection(GetReverthEligibleVisibleCards());
            if (safe.Count == 0)
                return false;

            TurnPlanStep plan = GetTurnPlanStep();
            if (plan != TurnPlanStep.OpeningReverth
                && plan != TurnPlanStep.RecoveryReverth)
                return false;

            reverthMainAttempted = true;
            selectionMode = SelectionMode.ReverthShuffle;
            return true;
        }

        private bool HandSpecialSetEffect()
        {
            if (Card == null || !IsEnneacraftMonster(Card)
                || Card.Location != CardLocation.Hand
                || ActivateDescription != Util.GetStringId(Card.Id, 1))
                return false;
            TurnPlanStep plan = GetTurnPlanStep();
            if (!IsOwnMainPhase() || OpenMonsterZones() <= 0
                || (plan != TurnPlanStep.OpeningBoard
                    && plan != TurnPlanStep.RecoveryBoard))
                return false;
            if (!GetDeployableHandMonsters().Any())
                return false;

            selectionMode = SelectionMode.SpecialSet;
            return true;
        }

        private bool OwnTurnFlip()
        {
            if (Card == null || !IsOwnMainPhase() || ownTurnCount < 2
                || Card.Location != CardLocation.MonsterZone
                || !Card.IsFacedown() || !IsEnneacraftMonster(Card)
                || monstersSetThisTurn.Contains(Card)
                || recoveryReturnResolved)
                return false;
            if (GetTurnPlanStep() != TurnPlanStep.RecoveryFlip)
                return false;

            recoveryFlipStarted = true;
            return true;
        }

        private bool FinalPendulumPlacement()
        {
            if (!IsOwnMainPhase() || !IsPendulumPlacementPrompt(Card)
                || Card == null || !IsLevel1(Card.Id)
                || PendulumScaleCount() >= 2)
                return false;
            if (Object.ReferenceEquals(Card, GetProtectedBoardAnchor(Bot.Hand)))
                return false;

            TurnPlanStep plan = GetTurnPlanStep();
            if (plan != TurnPlanStep.OpeningFinalScales
                && plan != TurnPlanStep.RecoveryFinalScales)
                return false;

            List<ClientCard> candidates = GetFinalScaleCandidates();
            if (candidates.Count == 0 || !Object.ReferenceEquals(candidates[0], Card))
                return false;

            finalScalePhaseStarted = true;
            unresolvedRouteScalePlacements.Add(Card);
            return true;
        }

        private bool FallbackMonsterSet()
        {
            if (Card == null || !IsOwnMainPhase() || !IsEnneacraftMonster(Card)
                || Card.Location != CardLocation.Hand || OpenMonsterZones() <= 0)
                return false;
            TurnPlanStep plan = GetTurnPlanStep();
            if (plan != TurnPlanStep.OpeningBoard
                && plan != TurnPlanStep.RecoveryBoard)
                return false;

            List<ClientCard> candidates = GetDeployableHandMonsters()
                .OrderBy(DeploymentPriority)
                .ToList();
            return candidates.Count > 0 && candidates[0].Id == Card.Id;
        }

        private TurnPlanStep GetTurnPlanStep()
        {
            if (!IsOwnMainPhase())
                return TurnPlanStep.None;
            if (ownTurnCount == 1)
                return GetOpeningPlanStep();
            if (ownTurnCount >= 2)
                return GetRecoveryPlanStep();
            return TurnPlanStep.None;
        }

        private TurnPlanStep GetOpeningPlanStep()
        {
            if (openingReturnPending)
                return TurnPlanStep.OpeningPolisReturnPending;

            if (!openingPairReturned)
            {
                if (IsEnneapolisOnField() && PendulumScaleCount() >= 2
                    && !enneapolisReturnAttempted)
                    return TurnPlanStep.OpeningPolisReturn;

                if (GetUnusedSearchScales().Any()
                    && pendulumSearchAttempted.Count < 2)
                    return TurnPlanStep.OpeningPreSearch;

                if (!ShouldPrioritizeBoardOverScales() && PendulumScaleCount() < 2
                    && (GetPreFieldScaleCandidates().Any()
                        || Bot.HasInHand(CardId.Release) && ShouldUseReleaseForScale()))
                    return TurnPlanStep.OpeningPreScale;

                if (!IsEnneapolisOnField()
                    && (Bot.HasInHand(CardId.Reset) || Bot.HasInHand(CardId.Enneapolis)))
                    return TurnPlanStep.OpeningFieldAccess;

                return GetOpeningFinishStep();
            }

            if (NeedPostReturnSearch() && postReturnSearchAttempts < 2)
            {
                if (GetUnusedSearchScales().Any())
                    return TurnPlanStep.OpeningPostSearch;
                if (!ShouldPrioritizeBoardOverScales() && PendulumScaleCount() < 2
                    && (GetPostReturnScaleCandidates().Any()
                        || Bot.HasInHand(CardId.Release) && ShouldUseReleaseForScale()))
                    return TurnPlanStep.OpeningPostSearch;
            }

            if (!reverthMainAttempted && Bot.HasInHand(CardId.Reverth)
                && BuildSafeReverthSelection(GetReverthEligibleVisibleCards()).Count > 0)
                return TurnPlanStep.OpeningReverth;

            return GetOpeningFinishStep();
        }

        private TurnPlanStep GetOpeningFinishStep()
        {
            if (NeedsBoardAnchor() && Bot.Hand.Any(IsEnneacraftMonster))
                return TurnPlanStep.OpeningBoard;
            if (OpenMonsterZones() > 0 && GetDeployableHandMonsters().Any())
                return TurnPlanStep.OpeningBoard;
            if (PendulumScaleCount() < 2 && GetFinalScaleCandidates().Any())
                return TurnPlanStep.OpeningFinalScales;
            return TurnPlanStep.Complete;
        }

        private TurnPlanStep GetRecoveryPlanStep()
        {
            if (NeedsBoardAnchor() && Bot.Hand.Any(IsEnneacraftMonster))
                return TurnPlanStep.RecoveryBoard;

            if (recoveryReturnResolved)
                return GetRecoveryFinishStep();

            if (!recoveryFlipStarted)
            {
                if (NeedRecoverySearch())
                {
                    if (GetUnusedSearchScales().Any())
                        return TurnPlanStep.RecoverySearch;
                    if (!ShouldPrioritizeBoardOverScales() && PendulumScaleCount() < 2
                        && (GetRecoverySearchScaleCandidates().Any()
                            || Bot.HasInHand(CardId.Release) && ShouldUseReleaseForScale()))
                        return TurnPlanStep.RecoveryScale;
                }

                if (!reverthMainAttempted && Bot.HasInHand(CardId.Reverth)
                    && NeedReverthRecovery()
                    && BuildSafeReverthSelection(GetReverthEligibleVisibleCards()).Count > 0)
                    return TurnPlanStep.RecoveryReverth;

                if (!IsEnneapolisOnField()
                    && (Bot.HasInHand(CardId.Reset) || Bot.HasInHand(CardId.Enneapolis)))
                    return TurnPlanStep.RecoveryFieldAccess;

                if (!ShouldPrioritizeBoardOverScales() && PendulumScaleCount() < 2
                    && GetRecoveryRouteScaleCandidates().Any())
                    return TurnPlanStep.RecoveryScale;
            }

            if (GetFlippableFacedownEnneacraftMonsters().Any())
                return TurnPlanStep.RecoveryFlip;

            if (GetFaceupEnneacraftMonsters().Any() && IsEnneapolisOnField()
                && !enneapolisReturnAttempted)
                return TurnPlanStep.RecoveryPolisReturn;

            return GetRecoveryFinishStep();
        }

        private TurnPlanStep GetRecoveryFinishStep()
        {
            if (NeedsBoardAnchor() && Bot.Hand.Any(IsEnneacraftMonster))
                return TurnPlanStep.RecoveryBoard;
            if (OpenMonsterZones() > 0 && GetDeployableHandMonsters().Any())
                return TurnPlanStep.RecoveryBoard;
            if (PendulumScaleCount() < 2 && GetFinalScaleCandidates().Any())
                return TurnPlanStep.RecoveryFinalScales;
            return TurnPlanStep.Complete;
        }

        private bool NeedPostReturnSearch()
        {
            return MissingCoreIds().Count > 0 || !Bot.HasInHand(CardId.Reverth);
        }

        private bool NeedRecoverySearch()
        {
            if (MissingCoreIds().Count > 0)
                return true;
            if (!IsEnneapolisOnField() && !Bot.HasInHand(CardId.Enneapolis)
                && !Bot.HasInHand(CardId.Reset))
                return true;
            if (!Bot.HasInHand(CardId.Reverth) && NeedReverthRecovery())
                return true;
            return false;
        }

        private bool NeedReverthRecovery()
        {
            if (MissingCoreIds().Count > 0)
                return true;
            if (GetFaceupLevel9SpellZoneCards().Any())
                return true;
            return BuildSafeReverthSelection(GetReverthEligibleVisibleCards()).Count >= 2;
        }

        private bool ShouldUseReleaseForScale()
        {
            if (releaseMainAttempted || ShouldPrioritizeBoardOverScales())
                return false;
            if (PendulumScaleCount() >= 2)
                return false;
            if (!HasFreshLevel1SearchId())
                return false;

            if (ownTurnCount == 1)
            {
                if (!openingPairReturned)
                    return pendulumSearchAttempted.Count < 2 && !GetPreFieldScaleCandidates().Any();
                return postReturnSearchAttempts < 2 && NeedPostReturnSearch()
                    && !GetPostReturnScaleCandidates().Any();
            }

            if (ownTurnCount >= 2 && !recoveryFlipStarted)
                return NeedRecoverySearch() && !GetRecoverySearchScaleCandidates().Any();

            return false;
        }

        private bool NeedLevel1Resource()
        {
            if (ownTurnCount == 1)
                return openingPairReturned && !reverthMainAttempted
                    && postReturnSearchAttempts < 2 && NeedPostReturnSearch();
            return ownTurnCount >= 2 && (NeedRecoverySearch() || PendulumScaleCount() < 2);
        }

        private bool HasFreshLevel1SearchId()
        {
            return Level1Ids.Any(id => !pendulumSearchAttempted.Contains(id));
        }

        private bool IsFreshLevel1SearchCard(ClientCard card)
        {
            return card != null && IsLevel1(card.Id)
                && !pendulumSearchAttempted.Contains(card.Id);
        }

        private List<ClientCard> GetDeployableHandMonsters()
        {
            List<ClientCard> handMonsters = Bot.Hand.Where(IsEnneacraftMonster)
                .OrderBy(DeploymentPriority)
                .ToList();
            HashSet<ClientCard> reserved = new HashSet<ClientCard>(GetReservedScaleCards());
            return handMonsters.Where(c => !reserved.Contains(c)).ToList();
        }

        private List<ClientCard> GetReservedScaleCards()
        {
            int reserveCount = Math.Max(0, 2 - PendulumScaleCount());
            if (reserveCount == 0)
                return new List<ClientCard>();

            List<ClientCard> scaleCandidates = Bot.Hand.Where(c => c != null
                    && IsEnneacraftMonster(c) && IsLevel1(c.Id))
                .ToList();
            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            int protectedScaleCount = protectedAnchor != null
                && scaleCandidates.Contains(protectedAnchor) ? 1 : 0;
            int availableForScales = scaleCandidates.Count - protectedScaleCount;
            reserveCount = Math.Min(reserveCount, Math.Max(0, availableForScales));

            return scaleCandidates.Where(c => !Object.ReferenceEquals(c, protectedAnchor))
                .OrderBy(ScaleReservationPriority)
                .ThenBy(FinalScalePriority)
                .Take(reserveCount)
                .ToList();
        }

        private ClientCard GetProtectedBoardAnchor(IEnumerable<ClientCard> source)
        {
            if (!NeedsBoardAnchor())
                return null;

            return source.Where(c => c != null && IsEnneacraftMonster(c))
                .OrderBy(c => c.Id == CardId.Atori ? 0 : 1)
                .ThenBy(DeploymentPriority)
                .FirstOrDefault();
        }

        private bool NeedsBoardAnchor()
        {
            return OpenMonsterZones() > 0
                && !Bot.GetMonsters().Any(c => c != null && IsEnneacraftMonster(c));
        }

        private bool ShouldPrioritizeBoardOverScales()
        {
            return scaleRouteAborted && NeedsBoardAnchor()
                && Bot.Hand.Any(IsEnneacraftMonster);
        }

        private int ScaleReservationPriority(ClientCard card)
        {
            if (card == null || !IsLevel1(card.Id))
                return Int32.MaxValue;
            bool represented = HasCoreOnMonsterZone(card.Id)
                || Bot.Hand.Count(c => c != null && c.Id == card.Id) >= 2;
            if (!IsCore(card.Id))
                return 0;
            if (represented)
                return 10;
            return 40;
        }

        private List<ClientCard> GetRecoveryRouteScaleCandidates()
        {
            if (ShouldPrioritizeBoardOverScales())
                return new List<ClientCard>();

            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            List<ClientCard> safe = GetScaleCandidates(false)
                .Where(c => !Object.ReferenceEquals(c, protectedAnchor))
                .ToList();
            if (safe.Count > 0)
                return safe;
            return GetScaleCandidates(true)
                .Where(c => !Object.ReferenceEquals(c, protectedAnchor))
                .ToList();
        }

        private void RegisterScaleRouteInterruption(ClientCard card)
        {
            scaleRouteInterruptions++;
            if (scaleRouteInterruptions >= 2)
                scaleRouteAborted = true;
        }

        private List<ClientCard> GetPreFieldScaleCandidates()
        {
            if (PendulumScaleCount() >= 2 || ShouldPrioritizeBoardOverScales())
                return new List<ClientCard>();

            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            return Bot.Hand.Where(c => c != null && IsLevel1(c.Id)
                    && !pendulumSearchAttempted.Contains(c.Id)
                    && !Object.ReferenceEquals(c, protectedAnchor))
                .OrderBy(RouteScalePriority)
                .ToList();
        }

        private List<ClientCard> GetPostReturnScaleCandidates()
        {
            if (PendulumScaleCount() >= 2 || ShouldPrioritizeBoardOverScales())
                return new List<ClientCard>();
            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            return Bot.Hand.Where(c => c != null && IsLevel1(c.Id)
                    && !pendulumSearchAttempted.Contains(c.Id)
                    && !Object.ReferenceEquals(c, protectedAnchor))
                .OrderBy(c => IsCore(c.Id) ? 1 : 0)
                .ThenBy(RouteScalePriority)
                .ToList();
        }

        private List<ClientCard> GetRecoverySearchScaleCandidates()
        {
            if (PendulumScaleCount() >= 2 || ShouldPrioritizeBoardOverScales())
                return new List<ClientCard>();
            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            return Bot.Hand.Where(c => c != null && IsLevel1(c.Id)
                    && !pendulumSearchAttempted.Contains(c.Id)
                    && CanUseCoreAsTemporaryScale(c)
                    && !Object.ReferenceEquals(c, protectedAnchor))
                .OrderBy(RouteScalePriority)
                .ToList();
        }

        private List<ClientCard> GetScaleCandidates(bool allowLastCore)
        {
            return Bot.Hand.Where(c => c != null && IsLevel1(c.Id)
                    && IsEnneacraftMonster(c))
                .Where(c => allowLastCore || CanUseCoreAsScale(c))
                .OrderBy(FinalScalePriority)
                .ToList();
        }

        private List<ClientCard> GetFinalScaleCandidates()
        {
            List<ClientCard> reserved = GetReservedScaleCards();
            if (reserved.Count > 0)
                return reserved.OrderBy(FinalScalePriority).ToList();

            ClientCard protectedAnchor = GetProtectedBoardAnchor(Bot.Hand);
            List<ClientCard> safe = GetScaleCandidates(false)
                .Where(c => !Object.ReferenceEquals(c, protectedAnchor))
                .ToList();
            if (safe.Count > 0)
                return safe;
            return GetScaleCandidates(true)
                .Where(c => !Object.ReferenceEquals(c, protectedAnchor))
                .ToList();
        }

        private bool CanUseCoreAsTemporaryScale(ClientCard card)
        {
            if (card == null || !IsCore(card.Id))
                return true;
            if (HasCoreOnMonsterZone(card.Id))
                return true;
            return Bot.Hand.Count(c => c != null && c.Id == card.Id) >= 2;
        }

        private bool CanUseCoreAsScale(ClientCard card)
        {
            if (card == null || !IsCore(card.Id))
                return true;
            if (HasCoreOnMonsterZone(card.Id))
                return true;
            return Bot.Hand.Count(c => c != null && c.Id == card.Id) >= 2;
        }

        private int ChoosePendulumSearchTarget(int sourceId)
        {
            if (ownTurnCount == 1 && !openingPairReturned)
            {
                if (pendulumSearchAttempted.Count == 0)
                    return ChooseFreshLevel1Target(sourceId);
                if (!IsEnneapolisOnField() && !Bot.HasInHand(CardId.Reset)
                    && !Bot.HasInHand(CardId.Enneapolis))
                    return CardId.Reset;
                return ChooseFreshLevel1Target(sourceId);
            }

            List<int> missing = MissingCoreIds();
            if (ownTurnCount == 1 && openingPairReturned)
            {
                if (postReturnSearchAttempts == 0 && missing.Count > 0)
                    return missing[0];
                if (!Bot.HasInHand(CardId.Reverth))
                    return CardId.Reverth;
                if (missing.Count > 0)
                    return missing[0];
            }
            else
            {
                if (missing.Count > 0)
                    return missing[0];
                if (!Bot.HasInHand(CardId.Reverth))
                    return CardId.Reverth;
            }
            if (!IsEnneapolisOnField() && !Bot.HasInHand(CardId.Enneapolis)
                && !Bot.HasInHand(CardId.Reset))
                return CardId.Reset;
            return CardId.Aiza;
        }

        private int ChooseFreshLevel1Target(int sourceId)
        {
            foreach (int id in RouteLevel1Priority)
            {
                if (id == sourceId)
                    continue;
                if (!HasVisibleOwnCard(id))
                    return id;
            }
            foreach (int id in RouteLevel1Priority)
            {
                if (id != sourceId)
                    return id;
            }
            return CardId.Trito;
        }

        private int ChooseReactiveSearchTarget()
        {
            List<int> missing = MissingCoreIds();
            if (missing.Count > 0)
                return missing[0];
            if (!Bot.HasInHand(CardId.Reverth))
                return CardId.Reverth;
            if (!IsEnneapolisOnField() && !Bot.HasInHand(CardId.Reset))
                return CardId.Reset;
            return CardId.Aiza;
        }

        private IList<ClientCard> SelectRevealThree(IList<ClientCard> cards, int min, int max)
        {
            int count = Math.Min(max, 3);
            List<ClientCard> selected = cards.Where(c => c != null && c.Id == desiredSearchId)
                .Take(count)
                .ToList();

            foreach (ClientCard card in OrderSearchCandidates(cards))
            {
                if (selected.Count >= count)
                    break;
                if (!selected.Contains(card))
                    selected.Add(card);
            }
            return selected;
        }

        private IList<ClientCard> SelectSearchOne(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = cards.Where(c => c != null && c.Id == desiredSearchId)
                .Take(1)
                .ToList();
            if (selected.Count == 0)
                selected.Add(OrderSearchCandidates(cards).First());
            return selected;
        }

        private IEnumerable<ClientCard> OrderSearchCandidates(IList<ClientCard> cards)
        {
            List<int> priority = new List<int>();
            AddUnique(priority, desiredSearchId);
            if (desiredSearchId == CardId.Reset)
                AddUnique(priority, CardId.Enneapolis);
            else if (desiredSearchId == CardId.Enneapolis)
                AddUnique(priority, CardId.Reset);
            foreach (int id in MissingCoreIds())
                AddUnique(priority, id);
            AddUnique(priority, CardId.Reverth);
            AddUnique(priority, CardId.Reset);
            foreach (int id in RouteLevel1Priority)
                AddUnique(priority, id);
            AddUnique(priority, CardId.Aiza);
            AddUnique(priority, CardId.Archa);
            AddUnique(priority, CardId.Atil);
            AddUnique(priority, CardId.Release);
            AddUnique(priority, CardId.Enneapolis);

            return cards.Where(c => c != null)
                .OrderBy(c => PriorityIndex(priority, c.Id));
        }

        private IList<ClientCard> SelectScaleFromDeck(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = cards.Where(IsFreshLevel1SearchCard)
                .OrderBy(RouteScalePriority)
                .Take(1)
                .ToList();
            if (selected.Count == 0)
            {
                selected = cards.Where(c => c != null && IsLevel1(c.Id))
                    .OrderBy(c => pendulumSearchAttempted.Contains(c.Id) ? 1 : 0)
                    .ThenBy(RouteScalePriority)
                    .Take(1)
                    .ToList();
            }
            if (selected.Count == 0)
                selected.Add(cards[0]);
            if (selected.Count > 0)
                unresolvedRouteScalePlacements.Add(selected[0]);
            return selected;
        }

        private IList<ClientCard> SelectRecoverFromExtra(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = cards.Where(IsFreshLevel1SearchCard)
                .OrderBy(RouteScalePriority)
                .Take(1)
                .ToList();
            if (selected.Count == 0)
            {
                selected = cards.Where(c => c != null && IsLevel1(c.Id))
                    .OrderBy(c => pendulumSearchAttempted.Contains(c.Id) ? 1 : 0)
                    .ThenBy(RouteScalePriority)
                    .Take(1)
                    .ToList();
            }
            if (selected.Count == 0)
                selected.Add(cards[0]);
            releaseGraveSelectionMade = selected.Count > 0;
            return selected;
        }

        private IList<ClientCard> SelectFaceDownSPSummonMonster(IList<ClientCard> cards,
            int min, int max)
        {
            HashSet<ClientCard> deployable = new HashSet<ClientCard>(GetDeployableHandMonsters());
            List<ClientCard> selected = cards.Where(c => c != null && deployable.Contains(c))
                .OrderBy(DeploymentPriority)
                .Take(1)
                .ToList();
            if (selected.Count == 0)
            {
                selected = cards.Where(IsEnneacraftMonster)
                    .OrderBy(DeploymentPriority)
                    .Take(1)
                    .ToList();
            }
            if (selected.Count == 0)
                selected.Add(cards[0]);
            return selected;
        }

        private IList<ClientCard> SelectReverthShuffle(IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = BuildSafeReverthSelection(cards);
            return selected.Take(max).ToList();
        }

        private IList<ClientCard> BuildSafeReverthSelection(IEnumerable<ClientCard> source)
        {
            List<ClientCard> candidates = source.Where(c => c != null && IsEnneacraftMonster(c)
                    && (c.Location == CardLocation.Hand || c.IsFaceup()))
                .Distinct()
                .ToList();
            HashSet<ClientCard> protectedCore = new HashSet<ClientCard>();

            foreach (int coreId in CoreFourIds)
            {
                if (Bot.MonsterZone.Any(c => c != null && c.Id == coreId && c.IsFacedown()))
                    continue;

                IEnumerable<ClientCard> keepCandidates = candidates.Where(c => c.Id == coreId);
                if (ownTurnCount == 1 && openingPairReturned)
                    keepCandidates = keepCandidates.Where(c => !IsPendulumScale(c));

                ClientCard keep = keepCandidates.OrderBy(CoreKeepLocationPriority)
                    .FirstOrDefault();
                if (keep != null)
                    protectedCore.Add(keep);
            }

            return candidates.Where(c => !protectedCore.Contains(c))
                .OrderBy(ReverthShufflePriority)
                .ToList();
        }

        private IList<ClientCard> GetReverthEligibleVisibleCards()
        {
            List<ClientCard> cards = new List<ClientCard>();
            cards.AddRange(Bot.Hand.Where(IsEnneacraftMonster));
            cards.AddRange(Bot.GetMonsters().Where(c => c != null && c.IsFaceup()
                && IsEnneacraftMonster(c)));
            cards.AddRange(GetPendulumScales().Where(IsEnneacraftMonster));
            return cards.Distinct().ToList();
        }

        private IList<ClientCard> SelectOpeningPolisReturn(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> scales = cards.Where(c => c != null && IsPendulumScale(c))
                .OrderBy(c => c.Sequence)
                .Take(2)
                .ToList();

            openingReturnExpected.Clear();
            openingReturnMovedCount = 0;
            foreach (ClientCard card in scales)
            {
                int count;
                openingReturnExpected.TryGetValue(card.Id, out count);
                openingReturnExpected[card.Id] = count + 1;
            }
            return scales;
        }

        private IList<ClientCard> SelectRecoveryPolisReturn(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = cards.Where(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone
                    && c.IsFaceup() && IsEnneacraftMonster(c))
                .OrderBy(DeploymentPriority)
                .Take(max)
                .ToList();

            recoveryReturnExpected.Clear();
            recoveryReturnMovedCount = 0;
            foreach (ClientCard card in selected)
            {
                int count;
                recoveryReturnExpected.TryGetValue(card.Id, out count);
                recoveryReturnExpected[card.Id] = count + 1;
            }
            return selected;
        }

        private IList<ClientCard> SelectEnneapolisFlippedMonster(IList<ClientCard> cards,
            int min, int max)
        {
            ClientCard target = cards.Where(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone && IsEnneacraftMonster(c))
                .OrderBy(PolisRecyclePriority)
                .FirstOrDefault();
            if (target == null)
                target = cards[0];

            enneapolisDestination = CanMoveFlippedToScale(target)
                ? EnneapolisDestination.PendulumZone
                : EnneapolisDestination.Hand;
            optionMode = OptionMode.EnneapolisDestination;
            return new List<ClientCard> { target };
        }

        private IList<ClientCard> SelectResetSetTargets(IList<ClientCard> cards, int min, int max)
        {
            int count = Math.Min(max, Math.Max(min, PendulumScaleCount()));
            return cards.Where(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone && c.IsFaceup()
                    && IsEnneacraftMonster(c))
                .OrderBy(DeploymentPriority)
                .Take(count)
                .ToList();
        }

        private IList<ClientCard> SelectReverthFlipTarget(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selected = cards.Where(c => c != null && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone && c.IsFacedown()
                    && IsEnneacraftMonster(c))
                .OrderBy(FlipPriority)
                .Take(1)
                .ToList();
            if (selected.Count == 0)
                selected.Add(cards[0]);
            return selected;
        }

        private IList<ClientCard> SelectEnemyMonster(IList<ClientCard> cards, int min, int max)
        {
            ClientCard target = Util.GetProblematicEnemyCard();
            if (target == null || !cards.Contains(target))
                target = Util.GetBestEnemyCard();
            if (target == null || !cards.Contains(target))
                target = cards.Where(c => c != null && c.Controller == 1)
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault();
            if (target == null)
                target = cards[0];
            return new List<ClientCard> { target };
        }

        private IList<ClientCard> SelectEnemySpellTrap(IList<ClientCard> cards, int min, int max)
        {
            ClientCard target = Util.GetBestEnemyCard();
            if (target == null || !cards.Contains(target))
                target = cards.FirstOrDefault(c => c != null && c.Controller == 1);
            if (target == null)
                target = cards[0];
            return new List<ClientCard> { target };
        }

        private IList<ClientCard> SelectEnemyGraveOrBanished(IList<ClientCard> cards, int min, int max)
        {
            int count = Math.Min(max, Math.Max(min, 3));
            List<ClientCard> ranked = cards.Where(c => c != null && c.Controller == 1)
                .OrderByDescending(EnemyGraveResourceScore)
                .ThenByDescending(c => c.Attack)
                .ToList();
            List<ClientCard> selected = ranked.Take(count).ToList();

            if (selected.Count < min)
            {
                foreach (ClientCard card in cards.Where(c => c != null && !selected.Contains(c)))
                {
                    selected.Add(card);
                    if (selected.Count >= min)
                        break;
                }
            }
            return selected;
        }

        private int EnemyGraveResourceScore(ClientCard card)
        {
            if (card == null || card.Id == 0)
                return Int32.MinValue;

            int score = GetEnemyGraveBehaviorScore(card.Id);
            int lastActivityTurn;
            if (enemyGraveLastActivityTurn.TryGetValue(card.Id, out lastActivityTurn))
            {
                int age = Math.Max(0, Duel.Turn - lastActivityTurn);
                if (age == 0)
                    score += 55;
                else if (age <= 2)
                    score += 25;
            }

            bool monster = card.HasType(CardType.Monster);
            bool extraDeckMonster = card.HasType(CardType.Fusion)
                || card.HasType(CardType.Synchro)
                || card.HasType(CardType.Xyz)
                || card.HasType(CardType.Link);

            if ((card.Location & CardLocation.Removed) != 0)
            {
                score += card.IsFacedown() ? -120 : 65;
            }
            else if ((card.Location & CardLocation.Grave) != 0)
            {
                score += 40;
            }

            if (monster)
            {
                score += 55;
                score += extraDeckMonster ? 15 : 45;

                if (!extraDeckMonster && card.Attack >= 0 && card.Attack <= 2000)
                    score += 15;
            }
            else
            {

                score += card.HasType(CardType.Trap) ? 42 : 36;
            }

            return score;
        }

        private void TrackEnemyGraveBehavior(ClientCard card, int previousControler,
            int previousLocation, int currentControler, int currentLocation)
        {
            if (card == null || card.Id == 0)
                return;

            bool wasEnemyCard = previousControler == 1 || currentControler == 1;
            if (!wasEnemyCard)
                return;

            bool fromGrave = (previousLocation & (int)CardLocation.Grave) != 0;
            bool fromRemoved = (previousLocation & (int)CardLocation.Removed) != 0;
            bool toGrave = (currentLocation & (int)CardLocation.Grave) != 0;
            bool toRemoved = (currentLocation & (int)CardLocation.Removed) != 0;

            if (fromGrave || fromRemoved)
            {
                int score = 35;
                if (currentControler == 1)
                {
                    if ((currentLocation & (int)(CardLocation.Hand
                        | CardLocation.MonsterZone | CardLocation.SpellZone)) != 0)
                    {
                        score = 190;
                    }
                    else if (fromGrave && toRemoved)
                    {

                        score = 165;
                    }
                    else if ((currentLocation & (int)(CardLocation.Deck
                        | CardLocation.Extra)) != 0)
                    {
                        score = 95;
                    }
                }

                AddEnemyGraveBehaviorScore(card.Id, score);
            }
            else if (previousControler == 1
                && (previousLocation & (int)CardLocation.Deck) != 0
                && (toGrave || toRemoved))
            {

                AddEnemyGraveBehaviorScore(card.Id, 45);
            }
            else if (previousControler == 1 && (toGrave || toRemoved))
            {

                AddEnemyGraveBehaviorScore(card.Id, 18);
            }
        }

        private void AddEnemyGraveBehaviorScore(int cardId, int amount)
        {
            if (cardId == 0 || amount <= 0)
                return;

            int current;
            enemyGraveBehaviorScores.TryGetValue(cardId, out current);
            enemyGraveBehaviorScores[cardId] = Math.Min(700, current + amount);
            enemyGraveLastActivityTurn[cardId] = Duel.Turn;
        }

        private int GetEnemyGraveBehaviorScore(int cardId)
        {
            int score;
            return enemyGraveBehaviorScores.TryGetValue(cardId, out score) ? score : 0;
        }

        private IList<ClientCard> SelectAizaReturn(IList<ClientCard> cards, int min, int max)
        {
            int limit = Math.Min(max, 3);
            List<ClientCard> selected = new List<ClientCard>();

            foreach (ClientCard card in cards.Where(c => c != null && c.Controller == 0
                && Duel.ChainTargets.Contains(c)).OrderBy(c => IsCore(c.Id) ? 0 : 1))
            {
                if (selected.Count >= limit)
                    break;
                selected.Add(card);
            }

            foreach (ClientCard card in cards.Where(c => c != null && c.Controller == 1)
                .OrderByDescending(c => c.Attack))
            {
                if (selected.Count >= limit)
                    break;
                if (!selected.Contains(card))
                    selected.Add(card);
            }

            foreach (ClientCard card in cards.Where(c => c != null && c.Controller == 0)
                .OrderBy(c => IsCore(c.Id) ? 1 : 0))
            {
                if (selected.Count >= min || selected.Count >= limit)
                    break;
                if (!selected.Contains(card))
                    selected.Add(card);
            }
            return selected;
        }

        private IList<ClientCard> SelectFlipEffectCards(ClientCard solvingCard,
            IList<ClientCard> cards, int min, int max)
        {
            if ((solvingCard.IsCode(CardId.Ekto) || solvingCard.IsCode(CardId.Proto))
                && cards.Any(c => c != null && c.Controller == 1
                    && c.Location == CardLocation.MonsterZone))
            {
                return SelectEnemyMonster(cards, min, max);
            }

            if (solvingCard.IsCode(CardId.Trito)
                && cards.Any(c => c != null && c.Controller == 1
                    && c.Location == CardLocation.SpellZone))
            {
                pendingTritoSpecialSet = true;
                return SelectEnemySpellTrap(cards, min, max);
            }

            if (solvingCard.IsCode(CardId.Enato)
                && cards.Any(c => c != null && c.Controller == 1
                    && (c.Location == CardLocation.Grave
                        || c.Location == CardLocation.Removed)))
            {
                return SelectEnemyGraveOrBanished(cards, min, max);
            }

            return null;
        }

        private bool CanMoveFlippedToScale(ClientCard card)
        {
            if (card == null || PendulumScaleCount() >= 2)
                return false;
            if (!IsLevel1(card.Id))
                return false;
            if (IsCore(card.Id) && !HasCoreOnMonsterZoneExcept(card.Id, card)
                && !Bot.Hand.Any(c => c != null && c.Id == card.Id))
                return false;
            return true;
        }

        private List<ClientCard> GetUnusedSearchScales()
        {
            return GetPendulumScales().Where(c => c != null && IsLevel1(c.Id)
                    && !pendulumSearchAttempted.Contains(c.Id))
                .ToList();
        }

        private List<ClientCard> GetPendulumScales()
        {
            List<ClientCard> result = new List<ClientCard>();
            ClientCard left = Util.GetPZone(0, 0);
            ClientCard right = Util.GetPZone(0, 1);
            if (left != null)
                result.Add(left);
            if (right != null && !Object.ReferenceEquals(left, right))
                result.Add(right);

            return result.Where(c => c.IsFaceup() && c.HasType(CardType.Pendulum))
                .ToList();
        }

        private ClientCard GetFieldSpell()
        {
            return Bot.GetFieldSpellCard();
        }

        private bool IsCurrentFieldSpell(ClientCard card)
        {
            ClientCard field = GetFieldSpell();
            return field != null && card != null
                && (Object.ReferenceEquals(field, card)
                    || card.Location == CardLocation.FieldZone && field.Id == card.Id);
        }

        private bool IsEnneapolisOnField()
        {
            ClientCard field = GetFieldSpell();
            return field != null && field.IsCode(CardId.Enneapolis) && field.IsFaceup();
        }

        private bool IsPendulumScale(ClientCard card)
        {
            return card != null && GetPendulumScales().Any(scale =>
                Object.ReferenceEquals(scale, card)
                || scale.Sequence == card.Sequence && scale.Id == card.Id);
        }

        private int PendulumScaleCount()
        {
            return GetPendulumScales().Count;
        }

        private List<ClientCard> GetFacedownEnneacraftMonsters()
        {
            return Bot.GetMonsters().Where(c => c != null && c.IsFacedown()
                && IsEnneacraftMonster(c)).ToList();
        }

        private List<ClientCard> GetFlippableFacedownEnneacraftMonsters()
        {
            return GetFacedownEnneacraftMonsters()
                .Where(c => !monstersSetThisTurn.Contains(c))
                .ToList();
        }

        private List<ClientCard> GetFaceupEnneacraftMonsters()
        {
            return Bot.GetMonsters().Where(c => c != null && c.IsFaceup()
                && IsEnneacraftMonster(c)).ToList();
        }

        private IEnumerable<ClientCard> GetFaceupLevel9SpellZoneCards()
        {
            return Bot.GetSpells().Where(c => c != null && c.IsFaceup()
                && IsLevel9(c.Id) && IsEnneacraftMonster(c));
        }

        private int OpenMonsterZones()
        {
            int count = 0;
            for (int i = 0; i < 5 && i < Bot.MonsterZone.Length; ++i)
            {
                if (Bot.MonsterZone[i] == null)
                    count++;
            }
            return count;
        }

        private List<int> MissingCoreIds()
        {
            List<int> result = new List<int>();
            foreach (int id in CoreFourIds)
            {
                if (!Bot.Hand.Any(c => c != null && c.Id == id)
                    && !Bot.MonsterZone.Any(c => c != null && c.Id == id))
                    result.Add(id);
            }
            return result;
        }

        private bool HasCoreOnMonsterZone(int id)
        {
            return Bot.MonsterZone.Any(c => c != null && c.Id == id);
        }

        private bool HasCoreOnMonsterZoneExcept(int id, ClientCard except)
        {
            return Bot.MonsterZone.Any(c => c != null && !Object.ReferenceEquals(c, except)
                && c.Id == id);
        }

        private bool HasVisibleOwnCard(int id)
        {
            return Bot.Hand.Any(c => c != null && c.Id == id)
                || Bot.MonsterZone.Any(c => c != null && c.Id == id)
                || Bot.SpellZone.Any(c => c != null && c.Id == id)
                || Bot.Graveyard.Any(c => c != null && c.Id == id)
                || Bot.Banished.Any(c => c != null && c.Id == id);
        }

        private bool IsOwnMainPhase()
        {
            return Duel.Player == 0
                && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2);
        }

        private bool IsSpellActivationFromHandOrSet(ClientCard card)
        {
            return card != null && (card.Location == CardLocation.Hand
                || (card.Location == CardLocation.SpellZone
                    || card.Location == CardLocation.FieldZone) && card.IsFacedown());
        }

        private bool IsPendulumPlacementPrompt(ClientCard card)
        {
            if (card == null || card.Location != CardLocation.Hand
                || !IsEnneacraftMonster(card) || !card.HasType(CardType.Pendulum))
                return false;
            return ActivateDescription == PendulumActivateDescription;
        }

        private bool IsEnneacraftMonster(ClientCard card)
        {
            return card != null && IsEnneacraftMonsterId(card.Id)
                && card.HasType(CardType.Monster)
                && card.HasSetcode(EnneacraftSetcode);
        }

        private bool IsEnneacraftMonsterId(int id)
        {
            return MonsterIds.Contains(id);
        }

        private bool IsLevel1(int id)
        {
            return Level1Ids.Contains(id);
        }

        private bool IsLevel9(int id)
        {
            return Level9Ids.Contains(id);
        }

        private bool IsCore(int id)
        {
            return CoreFourIds.Contains(id);
        }

        private int RouteScalePriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            int index = Array.IndexOf(RouteLevel1Priority, card.Id);
            if (index >= 0)
                return index;
            index = Array.IndexOf(BonusPriority, card.Id);
            if (index >= 0)
                return 100 + index;
            return 1000;
        }

        private int FinalScalePriority(ClientCard card)
        {
            if (card == null || !IsLevel1(card.Id))
                return Int32.MaxValue;
            int baseScore = 0;
            if (IsCore(card.Id) && !HasCoreOnMonsterZone(card.Id))
                baseScore += 50;
            return baseScore + RouteScalePriority(card);
        }

        private int DeploymentPriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            int index = Array.IndexOf(CoreFourIds, card.Id);
            if (index >= 0 && !HasCoreOnMonsterZone(card.Id))
                return index;
            index = Array.IndexOf(BonusPriority, card.Id);
            if (index >= 0)
                return 100 + index;
            return 1000;
        }

        private int FlipPriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            int index = Array.IndexOf(CoreFourIds, card.Id);
            if (index >= 0)
                return index;
            index = Array.IndexOf(BonusPriority, card.Id);
            if (index >= 0)
                return 100 + index;
            return 1000;
        }

        private int PolisRecyclePriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            if (!IsCore(card.Id) && IsLevel1(card.Id) && PendulumScaleCount() < 2)
                return 0;
            int index = Array.IndexOf(CoreFourIds, card.Id);
            if (index >= 0)
                return 100 + index;
            return 200 + FlipPriority(card);
        }

        private int CoreKeepLocationPriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            if (card.Location == CardLocation.Hand)
                return 0;
            if (card.Location == CardLocation.MonsterZone)
                return 1;
            if (IsPendulumScale(card))
                return 2;
            return 3;
        }

        private int ReverthShufflePriority(ClientCard card)
        {
            if (card == null)
                return Int32.MaxValue;
            if (IsLevel9(card.Id) && card.Location == CardLocation.SpellZone)
                return 0;
            if (IsPendulumScale(card))
                return 1;
            if (!IsCore(card.Id))
                return 10 + RouteScalePriority(card);
            return 100 + DeploymentPriority(card);
        }

        private int PreferredMonsterZone(int id)
        {
            if (id == CardId.Asta)
                return 0;
            if (id == CardId.Atori)
                return 1;
            if (id == CardId.Deftero)
                return 2;
            if (id == CardId.Proto)
                return 3;
            return 4;
        }

        private int IndexOfOption(IList<int> options, int value)
        {
            for (int i = 0; i < options.Count; ++i)
            {
                if (options[i] == value)
                    return i;
            }
            return -1;
        }

        private IList<ClientCard> SelectByPriority(IList<ClientCard> cards, IEnumerable<int> priority,
            int min, int max)
        {
            List<int> ids = priority.ToList();
            return cards.Where(c => c != null)
                .OrderBy(c => PriorityIndex(ids, c.Id))
                .Take(Math.Max(min, 1))
                .ToList();
        }

        private int PriorityIndex(IList<int> priority, int id)
        {
            int index = priority.IndexOf(id);
            return index >= 0 ? index : 10000;
        }

        private void AddUnique(IList<int> list, int id)
        {
            if (id != 0 && !list.Contains(id))
                list.Add(id);
        }

        private IList<ClientCard> Checked(IList<ClientCard> selected, IList<ClientCard> cards,
            int min, int max)
        {
            if (selected == null)
                return null;
            return Util.CheckSelectCount(selected, cards, min, max);
        }

    }
}
