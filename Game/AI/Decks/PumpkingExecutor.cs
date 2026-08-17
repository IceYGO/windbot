using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Pumpking", "AI_Pumpking")]
    public class PumpkingExecutor : DefaultExecutor
    {
        public class CardId
        {

            public const int EldlichTheGoldenLord = 95440946;
            public const int DoomkingBalerdroch = 39185163;
            public const int ArmyOfTheHaunted = 18078153;
            public const int GreatMammothOfTheNetherworld = 80461466;
            public const int ChangshiTheSpiridao = 76352503;
            public const int OfficiatingReverie = 49828011;
            public const int PumpkingTheKingOfGraveGhosts = 81684048;
            public const int StareOfTheSnakeHair = 54077752;

            public const int Hublot = 29081251;

            public const int Mezuki = 92826944;
            public const int VampireGrace = 40607210;


            public const int GhostBelle = 73642296;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;


            public const int FoolishBurial = 81439173;
            public const int Terraforming = 73628505;
            public const int EctoplasmicFortification = 16734927;
            public const int DeltaOfInvitation = 3129133;
            public const int VortexOfTime = 42138622;
            public const int CallOfTheHaunted = 97077563;
            public const int InfiniteImpermanence = 10045474;
            public const int DominusImpulse = 40366667;
            public const int UpstartGoblin = 70368879;


            public const int EldlichTheMadGoldenLord = 74889525;
            public const int FallenAngelOfTheGoldenLand = 43143567;
            public const int Varudras = 70636044;
            public const int TheUndyingLegion = 43355214;
            public const int DhampirVampireSheridan = 32302078;
            public const int EvolzarLars = 35103106;
            public const int WollowFounderOfTheDrudgeDragons = 45935145;
            public const int PumpkingTheGreatGhostKing = 17856505;
            public const int OfficiatorOfDoomSamuel = 76213610;
            public const int VampireSucker = 37129797;
            public const int FlyingMary = 95784714;
            public const int GravityController = 23656668;


            public const int CrystalWingSynchroDragon = 50954680;
            public const int DarkMagician = 46986414;
            public const int EternalSoul = 48680970;
        }

        private readonly HashSet<int> activatedThisTurn = new HashSet<int>();
        private readonly List<ClientCard> currentNegateCardList = new List<ClientCard>();


        private bool dominusImpulseHandLock = false;

        private const int PumpkingHandMarker = CardId.PumpkingTheKingOfGraveGhosts + 1000;

        private enum PumpkingComboState
        {
            None,
            PreparingPumpking,
            HublotSummoned,
            HublotResolved,
            PumpkingReady,
            CallReady,
            PumpkingRevived,
            ChangshiSummoned,
            SamuelSummoned,
            SamuelRevived,
            GreatPumpkingSummoned,
            UndyingSummoned
        }


        private enum StrategicGoal
        {
            None,
            SecurePumpking,
            ProduceLevel6Bodies,
            CompleteNormalPumpkingRoute,
            CompleteUraraRoute,
            CompleteEldlichRoute,
            NegateImmediateThreat,
            RemoveImmediateThreat,
            BuildVarudras,
            EnableEldlichWithNormalZombie,
            PreserveEndboard
        }

        private enum ComboRoute
        {
            None,
            NormalPumpking,
            UraraRecovery,
            EldlichRank10,
            VarudrasExtension,
            BrickEldlich,
            PumpkingRecoveryLoop
        }

        private enum InterruptMode
        {
            Hold,
            SamuelReactiveNegate,
            SamuelReactiveReviveBeforeControl,
            SamuelPreemptNegate,
            SamuelPreemptSnakehair,
            SamuelPreemptMammoth,
            VarudrasHardNegate
        }

        private enum VarudrasDestroyMode
        {
            None,
            TriggeredEnemyRemoval,
            PostNegateRemoval
        }

        private enum SurplusRank6Plan
        {
            None,
            SheridanRemoval,
            LarsNegate,
            WollowPower
        }

        private sealed class InterruptPlan
        {
            public InterruptMode Mode;
            public ClientCard ChainSource;


            public ClientCard EnemyTarget;
            public ClientCard SamuelReviveTarget;
            public ClientCard SamuelDisableTarget;
            public ClientCard FollowUpTarget;
            public string Reason;
        }

        private PumpkingComboState pumpkingComboState = PumpkingComboState.None;
        private StrategicGoal currentStrategicGoal = StrategicGoal.None;
        private ComboRoute currentComboRoute = ComboRoute.None;
        private InterruptMode pendingInterruptMode = InterruptMode.Hold;
        private int plannedSamuelReviveId = 0;
        private bool plannedSamuelReviveResolved = false;
        private ClientCard freshEnemyMonster = null;
        private ClientCard freshEnemyFaceupCard = null;
        private bool enemyCommitmentWindow = false;
        private int enemyCommitmentTurn = -1;
        private ClientCard pendingSnakehairDisableTarget = null;
        private ClientCard pendingMammothDestroyTarget = null;
        private bool ectoplasmicSearchUsed = false;
        private bool pumpkingSearchSucceeded = false;
        private bool callSetByPumpking = false;
        private bool pumpkingHandEffectAttempted = false;


        private bool pumpkingHandSelectionPending = false;
        private bool pumpkingCallPromptCompleted = false;
        private bool pumpkingDiscardSelfRequired = false;
        private ClientCard pendingPumpkingHandCard = null;
        private bool pumpkingSummonEffectAttempted = false;
        private bool pumpkingSummonEffectResolved = false;
        private bool changshiMillAttempted = false;
        private bool changshiMillResolved = false;
        private bool samuelReviveResolved = false;
        private bool greatPumpkingSearchAttempted = false;
        private bool greatPumpkingSearchResolved = false;


        private bool greatPumpkingSearchWindowPending = false;
        private int selectedGreatPumpkingSearchId = 0;
        private bool greatPumpkingBounceAttempted = false;
        private bool greatPumpkingBounceResolved = false;


        private bool sheridanRemovalAttemptedThisTurn = false;
        private bool vampireGraceRouteActive = false;
        private bool vampireGraceReviveCommittedThisTurn = false;
        private bool zombieLockedThisTurn = false;


        private bool doomkingOptionPending = false;
        private bool doomkingPreferNegate = false;
        private VarudrasDestroyMode pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
        private ClientCard pendingVarudrasDestroyTarget = null;
        private ClientCard varudrasNegatedChainSource = null;
        private bool varudrasNegatedSourceWasAlreadyFaceup = false;


        private readonly List<ClientCard> opponentFaceupBeforeCurrentChain =
            new List<ClientCard>();


        private bool samuelReviveSelectionPending = false;


        private bool samuelOwnTurnValueRevivePending = false;


        private bool reverieOverlaySelectionPending = false;


        private bool samuelFieldEffectCommittedThisTurn = false;


        private bool samuelOpponentNegatePending = false;
        private ClientCard samuelNegateTarget = null;


        private bool mezukiReviveSelectionPending = false;


        private bool mezukiLevel6ExtensionPending = false;


        private bool armySpecialSummonEffectCommittedThisTurn = false;


        private bool eldlichHandSelectionPending = false;
        private bool eldlichHandCostPromptCompleted = false;


        private ClientCard pendingEldlichGraveFieldCost = null;


        private ClientCard freshSetCallByPumpkingInstance = null;


        private ClientCard spentDeltaFieldInstance = null;


        private ClientCard pendingDeltaTokenFieldInstance = null;


        private bool changshiHandRescueRouteActive = false;
        private bool changshiHandRescueEldlichLoaded = false;


        private bool ashReplayLineActive = false;


        private bool eldlichRouteActive = false;
        private bool eldlichRouteMarySummoned = false;


        private bool eldlichRouteRank10CommitPending = false;


        private bool flyingMaryEldlichReviveSelectionPending = false;


        private bool flyingMaryComebackPumpkingPending = false;


        private bool callReviveSelectionPending = false;
        private int plannedCallReviveId = 0;


        private ClientCard pendingInfiniteImpermanenceTarget = null;

        private bool samuelGraveRecycleSelectionPending = false;


        private ClientCard pendingUndyingTarget = null;


        private string lastSuckerMaterialPlanLog = null;


        private bool pumpkingStarterSeenThisDuel = false;

        private int selectedHublotSendId = 0;
        private int selectedHublotRecoverId = 0;
        private bool selectedHublotRecover = false;
        private int selectedHublotXyzId = 0;
        private int selectedChangshiMillId = 0;
        private int selectedDeltaSendId = 0;
        private int selectedFoolishBurialSendId = 0;
        private int selectedSamuelReviveId = 0;
        private int samuelRevivedCardId = 0;
        private int summonCount = 1;

        public PumpkingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {


            AddExecutor(ExecutorType.Activate, CardId.GhostBelle, GhostBelleActivate);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusImpulse, DominusImpulseActivate);
            AddExecutor(ExecutorType.Activate, CardId.VortexOfTime, VortexOfTimeActivate);
            AddExecutor(ExecutorType.Activate, CardId.DoomkingBalerdroch, DoomkingBalerdrochActivate);


            AddExecutor(ExecutorType.Activate, CardId.OfficiatorOfDoomSamuel, OfficiatorOfDoomSamuelActivate);
            AddExecutor(ExecutorType.Activate, CardId.Varudras, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.EvolzarLars, EvolzarLarsActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheUndyingLegion, TheUndyingLegionActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheGreatGhostKing, PumpkingGreatGhostKingActivate);


            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingActivate);
            AddExecutor(ExecutorType.Activate, CardId.StareOfTheSnakeHair, StareOfTheSnakeHairHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.EctoplasmicFortification, EctoplasmicFortificationActivate);


            AddExecutor(ExecutorType.Summon, CardId.Hublot, HublotSummon);


            AddExecutor(ExecutorType.Summon, CardId.Mezuki, BrickZombieNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, BrickZombieNormalSummon);
            AddExecutor(ExecutorType.Activate, CardId.Hublot, HublotActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingSummonActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChangshiTheSpiridao, ChangshiTheSpiridaoActivate);


            AddExecutor(ExecutorType.Activate, CardId.OfficiatingReverie, OfficiatingReverieActivate);


            AddExecutor(ExecutorType.Activate, CardId.DeltaOfInvitation, DeltaOfInvitationActivate);


            AddExecutor(ExecutorType.Activate, CardId.EldlichTheGoldenLord, EldlichTheGoldenLordActivate);
            AddExecutor(ExecutorType.Activate, CardId.ArmyOfTheHaunted, ArmyOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.Mezuki, MezukiActivate);
            AddExecutor(ExecutorType.Activate, CardId.VampireGrace, VampireGraceActivate);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialActivate);
            AddExecutor(ExecutorType.Activate, CardId.GreatMammothOfTheNetherworld, GreatMammothActivate);
            AddExecutor(ExecutorType.Activate, CardId.StareOfTheSnakeHair, StareOfTheSnakeHairFieldActivate);


            AddExecutor(ExecutorType.SpSummon, CardId.Varudras, VarudrasSummon);


            AddExecutor(ExecutorType.SpSummon, CardId.FallenAngelOfTheGoldenLand, FallenAngelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FlyingMary, FlyingMaryEldlichRouteSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.OfficiatorOfDoomSamuel, OfficiatorOfDoomSamuelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PumpkingTheGreatGhostKing, PumpkingGreatGhostKingSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DhampirVampireSheridan, DhampirVampireSheridanSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.EvolzarLars, EvolzarLarsSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.WollowFounderOfTheDrudgeDragons, WollowSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.TheUndyingLegion, TheUndyingLegionSummon);


            AddExecutor(ExecutorType.SpSummon, CardId.FlyingMary, FlyingMarySummon);


            AddExecutor(ExecutorType.Activate, CardId.DhampirVampireSheridan, DhampirVampireSheridanActivate);
            AddExecutor(ExecutorType.Activate, CardId.WollowFounderOfTheDrudgeDragons, WollowActivate);
            AddExecutor(ExecutorType.Activate, CardId.FallenAngelOfTheGoldenLand, FallenAngelActivate);
            AddExecutor(ExecutorType.Activate, CardId.EldlichTheMadGoldenLord, EldlichTheMadGoldenLordActivate);
            AddExecutor(ExecutorType.Activate, CardId.FlyingMary, FlyingMaryActivate);

            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }


        private bool IsHublot(ClientCard card)
        {
            return card != null && card.IsCode(CardId.Hublot);
        }

        private bool IsHublotId(int id)
        {
            return id == CardId.Hublot;
        }

        [System.Diagnostics.Conditional("PUMPKING_DEBUG")]
        private void DebugRoute(string message)
        {
        }

        [System.Diagnostics.Conditional("PUMPKING_DEBUG")]
        private void DebugCards(string label, IEnumerable<ClientCard> cards)
        {
        }

        private void SetStrategicPlan(
            StrategicGoal goal,
            ComboRoute route,
            string reason)
        {
            if (goal != currentStrategicGoal || route != currentComboRoute)
            {
                DebugRoute("PLAN goal=" + goal + " route=" + route
                    + " reason=" + reason);
            }

            currentStrategicGoal = goal;
            currentComboRoute = route;
        }

        private bool HasCorePumpkingEndboard()
        {
            return Bot.HasInMonstersZone(CardId.TheUndyingLegion, faceUp: true)
                && Bot.HasInMonstersZone(CardId.OfficiatorOfDoomSamuel, faceUp: true);
        }

        private bool IsLaterTurnRecoveryState()
        {


            return Duel.Player == 0
                && Duel.Turn > 2
                && !HasCorePumpkingEndboard()
                && !HasConfirmedOpenStateBattleLethal()
                && !changshiHandRescueRouteActive
                && !eldlichRouteRank10CommitPending;
        }

        private bool CanBuildSurplusVarudras()
        {
            if (zombieLockedThisTurn || !Bot.HasInExtra(CardId.Varudras))
                return false;
            if (GetRank10Materials().Count != 2)
                return false;


            return eldlichRouteRank10CommitPending
                || eldlichRouteMarySummoned
                || HasCorePumpkingEndboard();
        }

        private bool CanEnableEldlichWithNormalZombie()
        {
            if (Duel.Player != 0 || summonCount <= 0 || zombieLockedThisTurn
                || dominusImpulseHandLock || !HasOpenMainMonsterZone())
            {
                return false;
            }
            if (HasDirectPumpkingLineAvailable())
                return false;
            if (!HasEldlichRouteExtraDeck())
                return false;

            bool fieldAccess = HasFaceupFieldSpell()
                || Bot.HasInHand(CardId.DeltaOfInvitation)
                || Bot.HasInHand(CardId.Terraforming);
            bool eldlichAccess = Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInDeck(CardId.EldlichTheGoldenLord);
            return fieldAccess && eldlichAccess;
        }

        private void RecalculateStrategicPlan(string reason)
        {
            ObservePumpkingStarterState();

            if (Duel.Player == 1)
            {
                if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 1)
                {
                    SetStrategicPlan(StrategicGoal.NegateImmediateThreat,
                        currentComboRoute, reason);
                    return;
                }

                if (enemyCommitmentWindow)
                {
                    SetStrategicPlan(StrategicGoal.RemoveImmediateThreat,
                        currentComboRoute, reason);
                    return;
                }

                SetStrategicPlan(StrategicGoal.PreserveEndboard,
                    currentComboRoute, reason);
                return;
            }

            if (ashReplayLineActive)
            {
                SetStrategicPlan(StrategicGoal.CompleteUraraRoute,
                    ComboRoute.UraraRecovery, reason);
                return;
            }


            if (eldlichRouteRank10CommitPending)
            {
                SetStrategicPlan(StrategicGoal.CompleteEldlichRoute,
                    ComboRoute.EldlichRank10, reason);
                return;
            }


            if (IsPumpkingComboInProgress())
            {
                int requiredLevel6Bodies = HasSamuelOnField() ? 2 : 3;
                StrategicGoal goal = CountFreeLevel6ForGreatPumpking()
                        < requiredLevel6Bodies
                    ? StrategicGoal.ProduceLevel6Bodies
                    : StrategicGoal.CompleteNormalPumpkingRoute;
                SetStrategicPlan(goal, ComboRoute.NormalPumpking, reason);
                return;
            }

            if (eldlichRouteActive || eldlichRouteMarySummoned)
            {
                SetStrategicPlan(StrategicGoal.CompleteEldlichRoute,
                    ComboRoute.EldlichRank10, reason);
                return;
            }

            if (CanBuildSurplusVarudras())
            {
                SetStrategicPlan(StrategicGoal.BuildVarudras,
                    ComboRoute.VarudrasExtension, reason);
                return;
            }

            if (HasCorePumpkingEndboard())
            {
                SetStrategicPlan(StrategicGoal.PreserveEndboard,
                    ComboRoute.PumpkingRecoveryLoop, reason);
                return;
            }

            if (HasDirectPumpkingLineAvailable())
            {
                SetStrategicPlan(StrategicGoal.SecurePumpking,
                    ComboRoute.NormalPumpking, reason);
                return;
            }

            if (CanEnableEldlichWithNormalZombie())
            {
                SetStrategicPlan(StrategicGoal.EnableEldlichWithNormalZombie,
                    ComboRoute.BrickEldlich, reason);
                return;
            }

            SetStrategicPlan(StrategicGoal.None, ComboRoute.None, reason);
        }

        private bool MatchesCard(ClientCard left, ClientCard right)
        {
            if (left == null || right == null)
                return false;
            return left == right
                || (left.Controller == right.Controller
                    && left.Location == right.Location
                    && left.Sequence == right.Sequence
                    && left.Id == right.Id);
        }

        private ClientCard FindMatchingCandidate(
            IEnumerable<ClientCard> cards,
            ClientCard target)
        {
            if (cards == null || target == null)
                return null;
            return cards.FirstOrDefault(c => MatchesCard(c, target));
        }

        private void ClearEnemyCommitment(string reason)
        {
            if (enemyCommitmentWindow)
                DebugRoute("COMMITMENT clear reason=" + reason);
            freshEnemyMonster = null;
            freshEnemyFaceupCard = null;
            enemyCommitmentWindow = false;
            enemyCommitmentTurn = -1;
        }

        private bool IsLiveEnemyFieldCard(ClientCard card)
        {
            return card != null && card.Controller == 1 && card.IsOnField();
        }

        private bool IsLiveEnemyMonster(ClientCard card)
        {
            return IsLiveEnemyFieldCard(card)
                && card.Location == CardLocation.MonsterZone
                && card.IsFaceup();
        }

        private bool IsSamuelReactiveSource(ClientCard source)
        {
            return source != null
                && source.Controller == 1
                && source.Location == CardLocation.MonsterZone
                && source.IsFaceup()
                && !source.IsDisabled()
                && IsOpponentChainWorthNegating(source);
        }

        private bool ShouldPreemptWithSnakehair(ClientCard monster)
        {
            if (!IsLiveEnemyMonster(monster) || !monster.IsAttack()
                || monster.IsShouldNotBeTarget() || monster.IsDisabled())
            {
                return false;
            }

            return monster.IsCode(CardId.EldlichTheMadGoldenLord)
                || IsKnownLiveNegateMonster(monster)
                || monster.IsMonsterShouldBeDisabledBeforeItUseEffect()
                || monster.IsFloodgate()
                || monster.IsMonsterDangerous()
                || monster.IsMonsterInvincible();
        }

        private ClientCard GetMammothPreemptTarget()
        {
            ClientCard fresh = freshEnemyFaceupCard;
            if (CanMammothDestroyTarget(fresh))
            {
                bool continuousProblem = fresh.IsMonster()
                    ? fresh.IsFloodgate() || fresh.IsMonsterDangerous()
                        || fresh.IsMonsterInvincible()
                        || fresh.IsMonsterShouldBeDisabledBeforeItUseEffect()
                    : fresh.HasType(CardType.Field | CardType.Continuous | CardType.Equip)
                        || fresh.IsFloodgate();
                if (continuousProblem)
                    return fresh;
            }

            ClientCard publicThreat = GetEnemyFieldPriority()
                .FirstOrDefault(c => CanMammothDestroyTarget(c)
                    && (c.IsFloodgate()
                        || (c.IsMonster()
                            && (c.IsMonsterDangerous()
                                || c.IsMonsterShouldBeDisabledBeforeItUseEffect()))));
            if (publicThreat != null)
                return publicThreat;


            ClientCard setBackrow = Enemy.GetSpells()
                .Where(c => c != null && c.IsFacedown()
                    && CanMammothDestroyTarget(c))
                .OrderBy(c => c.Sequence)
                .FirstOrDefault();
            if (setBackrow != null)
                return setBackrow;

            return Enemy.GetSpells()
                .Where(CanMammothDestroyTarget)
                .OrderBy(c => c.Sequence)
                .FirstOrDefault();
        }

        private ClientCard GetSamuelPumpkingBridgeTarget(
            IEnumerable<ClientCard> reviveTargets,
            int deckFollowUpId)
        {
            if (reviveTargets == null
                || pumpkingSummonEffectAttempted
                || pumpkingSummonEffectResolved
                || GetOpenMainMonsterZoneCount() < 2
                || !Bot.HasInDeck(deckFollowUpId))
            {
                return null;
            }

            return reviveTargets.FirstOrDefault(c => c != null
                && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                && IsZombie(c)
                && c.IsCanRevive());
        }

        private ClientCard GetMadGoldenControlTarget()
        {
            if (Duel.CurrentChain.Count == 0 || Duel.LastChainPlayer != 1)
                return null;

            ClientCard source = Util.GetLastChainCard();
            if (source == null
                || !source.IsCode(CardId.EldlichTheMadGoldenLord)
                || source.Controller != 1
                || source.Location != CardLocation.MonsterZone)
            {
                return null;
            }


            return Duel.ChainTargets.LastOrDefault(c => c != null
                && c.Controller == 0
                && c.Location == CardLocation.MonsterZone
                && c.IsFaceup());
        }

        private ClientCard GetSamuelEmergencyReviveBeforeControlCandidate(
            IEnumerable<ClientCard> cards)
        {
            if (cards == null)
                return null;

            List<ClientCard> legal = cards
                .Where(c => c != null && IsZombie(c) && c.IsCanRevive())
                .ToList();
            if (legal.Count == 0)
                return null;


            ClientCard pumpking = legal.FirstOrDefault(c =>
                c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking != null)
                return pumpking;

            int[] priority =
            {
                CardId.PumpkingTheGreatGhostKing,
                CardId.OfficiatingReverie,
                CardId.VampireGrace,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.Hublot,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao
            };

            foreach (int id in priority)
            {
                ClientCard target = legal.FirstOrDefault(c => c.IsCode(id));
                if (target != null)
                    return target;
            }

            return legal.OrderByDescending(c => c.Attack).FirstOrDefault();
        }

        private bool IsWorthDisablingWithSamuel(
            ClientCard monster,
            bool isCurrentChainSource)
        {
            if (!IsLiveEnemyMonster(monster) || monster.IsDisabled())
                return false;


            if (isCurrentChainSource)
                return IsOpponentChainWorthNegating(monster);

            return monster.IsCode(CardId.EldlichTheMadGoldenLord)
                || IsKnownLiveNegateMonster(monster)
                || monster.IsMonsterShouldBeDisabledBeforeItUseEffect()
                || monster.IsFloodgate()
                || monster.IsMonsterInvincible();
        }

        private int ScoreSamuelDisableTarget(
            ClientCard monster,
            ClientCard preferredChainSource)
        {
            if (monster == null)
                return int.MinValue;

            int score = 0;
            if (MatchesCard(monster, preferredChainSource))
                score += 10000;
            if (IsKnownLiveNegateMonster(monster))
                score += 5000;
            if (monster.IsMonsterShouldBeDisabledBeforeItUseEffect())
                score += 3600;
            if (monster.IsFloodgate())
                score += 3200;
            if (monster.IsMonsterInvincible())
                score += 2600;
            if (monster.IsCode(CardId.EldlichTheMadGoldenLord))
                score += 2200;
            score += Math.Max(0, monster.Attack);
            return score;
        }

        private ClientCard GetSamuelOptionalDisableTargetForRevive(
            ClientCard reviveTarget,
            ClientCard preferredChainSource)
        {
            if (reviveTarget == null)
                return null;

            int coveredAttack = Math.Max(0, reviveTarget.Attack);
            return Enemy.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && Math.Max(0, c.Attack) <= coveredAttack
                    && IsWorthDisablingWithSamuel(
                        c,
                        MatchesCard(c, preferredChainSource)))
                .OrderByDescending(c => ScoreSamuelDisableTarget(
                    c, preferredChainSource))
                .FirstOrDefault();
        }

        private ClientCard GetBestSamuelDirectDisableTarget(
            IEnumerable<ClientCard> reviveTargets,
            ClientCard preferredChainSource,
            out ClientCard selectedRevive)
        {
            selectedRevive = null;
            if (reviveTargets == null)
                return null;

            List<ClientCard> orderedTargets = Enemy.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && IsWorthDisablingWithSamuel(
                        c,
                        MatchesCard(c, preferredChainSource)))
                .OrderByDescending(c => ScoreSamuelDisableTarget(
                    c, preferredChainSource))
                .ToList();

            foreach (ClientCard target in orderedTargets)
            {
                ClientCard revive = GetSamuelOpponentTurnReviveCandidate(
                    reviveTargets, target);
                if (revive == null)
                    continue;

                selectedRevive = revive;
                return target;
            }

            return null;
        }

        private ClientCard GetSamuelSnakehairInteractionTarget(
            ClientCard preferredChainSource)
        {
            bool chainTargetsEnemyGraveMonster = Duel.CurrentChain.Count > 0
                && Duel.LastChainPlayer == 1
                && Duel.ChainTargets.Any(c => c != null
                    && c.Controller == 1
                    && c.Location == CardLocation.Grave
                    && c.IsMonster());

            if (IsLiveEnemyMonster(preferredChainSource)
                && preferredChainSource.IsAttack()
                && !preferredChainSource.IsShouldNotBeTarget()
                && (IsWorthDisablingWithSamuel(preferredChainSource, true)
                    || chainTargetsEnemyGraveMonster))
            {
                return preferredChainSource;
            }

            return Enemy.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && c.IsAttack()
                    && !c.IsDisabled()
                    && !c.IsShouldNotBeTarget()
                    && IsWorthDisablingWithSamuel(c, false))
                .OrderByDescending(c => ScoreSamuelDisableTarget(c, null))
                .FirstOrDefault();
        }

        private bool IsWorthSamuelMammothTarget(ClientCard card)
        {
            if (!CanMammothDestroyTarget(card))
                return false;

            if (card.IsMonster())
            {
                return IsKnownLiveNegateMonster(card)
                    || card.IsMonsterShouldBeDisabledBeforeItUseEffect()
                    || card.IsFloodgate()
                    || card.IsMonsterDangerous();
            }

            if (card.IsFacedown() && card.Location == CardLocation.SpellZone)
                return true;

            return card.IsFaceup()
                && (card.IsFloodgate()
                    || card.HasType(
                        CardType.Field | CardType.Continuous | CardType.Equip));
        }

        private ClientCard GetSamuelMammothInteractionTarget(
            ClientCard preferredChainSource)
        {
            if (IsWorthSamuelMammothTarget(preferredChainSource))
                return preferredChainSource;

            if (IsWorthSamuelMammothTarget(freshEnemyFaceupCard))
                return freshEnemyFaceupCard;

            return Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .Where(IsWorthSamuelMammothTarget)
                .OrderBy(c => c.IsMonster() && IsKnownLiveNegateMonster(c) ? 0 : 1)
                .ThenBy(c => c.IsMonster()
                    && c.IsMonsterShouldBeDisabledBeforeItUseEffect() ? 0 : 1)
                .ThenByDescending(c => c.IsMonster() ? c.GetDefensePower() : 0)
                .FirstOrDefault();
        }

        private ClientCard GetSamuelBridgeReviveTarget(
            IEnumerable<ClientCard> reviveTargets,
            int directReviveId,
            int pumpkingDeckFollowUpId)
        {
            if (reviveTargets == null)
                return null;

            if (!activatedThisTurn.Contains(directReviveId))
            {
                ClientCard direct = reviveTargets.FirstOrDefault(c => c != null
                    && c.IsCode(directReviveId)
                    && IsZombie(c)
                    && c.IsCanRevive());
                if (direct != null)
                    return direct;
            }

            return GetSamuelPumpkingBridgeTarget(
                reviveTargets, pumpkingDeckFollowUpId);
        }

        private InterruptPlan BuildSamuelBridgePlan(
            IEnumerable<ClientCard> reviveTargets,
            ClientCard chainSource)
        {
            ClientCard snakehairTarget = GetSamuelSnakehairInteractionTarget(
                chainSource);
            if (snakehairTarget != null)
            {
                ClientCard revive = GetSamuelBridgeReviveTarget(
                    reviveTargets,
                    CardId.StareOfTheSnakeHair,
                    CardId.StareOfTheSnakeHair);
                if (revive != null)
                {
                    return new InterruptPlan
                    {
                        Mode = InterruptMode.SamuelPreemptSnakehair,
                        ChainSource = chainSource,
                        EnemyTarget = snakehairTarget,
                        FollowUpTarget = snakehairTarget,
                        SamuelReviveTarget = revive,
                        SamuelDisableTarget = GetSamuelOptionalDisableTargetForRevive(
                            revive, chainSource),
                        Reason = revive.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                            ? "revive Pumpking to fetch Snakehair for a threat Samuel cannot directly cover"
                            : "revive Snakehair to stop a meaningful attack-position threat"
                    };
                }
            }

            ClientCard mammothTarget = GetSamuelMammothInteractionTarget(
                chainSource);
            if (mammothTarget != null
                && !activatedThisTurn.Contains(CardId.GreatMammothOfTheNetherworld))
            {
                ClientCard revive = GetSamuelBridgeReviveTarget(
                    reviveTargets,
                    CardId.GreatMammothOfTheNetherworld,
                    CardId.GreatMammothOfTheNetherworld);
                if (revive != null)
                {
                    return new InterruptPlan
                    {
                        Mode = InterruptMode.SamuelPreemptMammoth,
                        ChainSource = chainSource,
                        EnemyTarget = mammothTarget,
                        FollowUpTarget = mammothTarget,
                        SamuelReviveTarget = revive,
                        SamuelDisableTarget = GetSamuelOptionalDisableTargetForRevive(
                            revive, chainSource),
                        Reason = revive.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                            ? "revive Pumpking to fetch Mammoth for a field threat Samuel cannot directly cover"
                            : "revive Mammoth to remove a meaningful field threat"
                    };
                }
            }

            return null;
        }

        private InterruptPlan BuildSamuelOpponentPlan()
        {
            if (Duel.Player != 1
                || IsFriendlyChainInProgress()
                || (Duel.Phase != DuelPhase.Main1
                    && Duel.Phase != DuelPhase.Main2)
                || !HasSamuelOnField()
                || samuelFieldEffectCommittedThisTurn
                || !HasOpenMainMonsterZone())
            {
                return null;
            }

            ClientCard samuel = Bot.GetMonsters().FirstOrDefault(c => c != null
                && c.IsFaceup()
                && c.IsCode(CardId.OfficiatorOfDoomSamuel));
            if (samuel == null || samuel.IsDisabled()
                || WillCardEffectBeNegated(samuel)
                || samuel.Overlays == null || samuel.Overlays.Count <= 0)
            {
                return null;
            }

            List<ClientCard> reviveTargets = Bot.Graveyard
                .Where(c => c != null && c != samuel && IsZombie(c) && c.IsCanRevive())
                .ToList();
            if (reviveTargets.Count == 0)
                return null;

            ClientCard chainSource = Duel.CurrentChain.Count > 0
                && Duel.LastChainPlayer == 1
                    ? Util.GetLastChainCard() : null;

            if (chainSource != null)
            {
                ClientCard madGoldenControlTarget = GetMadGoldenControlTarget();
                if (madGoldenControlTarget != null)
                {
                    ClientCard emergencyRevive =
                        GetSamuelEmergencyReviveBeforeControlCandidate(reviveTargets);
                    if (emergencyRevive != null)
                    {
                        return new InterruptPlan
                        {
                            Mode = InterruptMode.SamuelReactiveReviveBeforeControl,
                            ChainSource = chainSource,
                            EnemyTarget = madGoldenControlTarget,
                            SamuelReviveTarget = emergencyRevive,
                            SamuelDisableTarget = GetSamuelOptionalDisableTargetForRevive(
                                emergencyRevive, chainSource),
                            Reason = "Mad Golden targeted our field monster; spend Samuel and revive value before control changes"
                        };
                    }
                }


                if (IsSamuelReactiveSource(chainSource))
                {
                    ClientCard directRevive = GetSamuelOpponentTurnReviveCandidate(
                        reviveTargets, chainSource);
                    if (directRevive != null)
                    {
                        return new InterruptPlan
                        {
                            Mode = InterruptMode.SamuelReactiveNegate,
                            ChainSource = chainSource,
                            EnemyTarget = chainSource,
                            SamuelReviveTarget = directRevive,
                            SamuelDisableTarget = chainSource,
                            Reason = "revive a Zombie whose ATK covers the current monster chain source"
                        };
                    }
                }


                InterruptPlan bridge = BuildSamuelBridgePlan(
                    reviveTargets, chainSource);
                if (bridge != null)
                    return bridge;


                ClientCard otherRevive;
                ClientCard otherTarget = GetBestSamuelDirectDisableTarget(
                    reviveTargets, null, out otherRevive);
                if (otherTarget != null && otherRevive != null)
                {
                    return new InterruptPlan
                    {
                        Mode = InterruptMode.SamuelPreemptNegate,
                        ChainSource = chainSource,
                        EnemyTarget = otherTarget,
                        SamuelReviveTarget = otherRevive,
                        SamuelDisableTarget = otherTarget,
                        Reason = "current source is not coverable; revive value and disable another live field threat"
                    };
                }

                return null;
            }

            if (!enemyCommitmentWindow || enemyCommitmentTurn != Duel.Turn)
                return null;


            ClientCard selectedRevive;
            ClientCard disableTarget = GetBestSamuelDirectDisableTarget(
                reviveTargets, null, out selectedRevive);
            if (disableTarget != null && selectedRevive != null)
            {
                return new InterruptPlan
                {
                    Mode = InterruptMode.SamuelPreemptNegate,
                    EnemyTarget = disableTarget,
                    SamuelReviveTarget = selectedRevive,
                    SamuelDisableTarget = disableTarget,
                    Reason = "revive a sufficient-ATK Zombie and disable the best live field threat"
                };
            }


            return BuildSamuelBridgePlan(reviveTargets, null);
        }

        private void CommitSamuelInterruptPlan(InterruptPlan plan)
        {
            pendingInterruptMode = plan != null ? plan.Mode : InterruptMode.Hold;
            plannedSamuelReviveId = plan != null && plan.SamuelReviveTarget != null
                ? plan.SamuelReviveTarget.Id : 0;
            samuelNegateTarget = plan != null
                ? plan.SamuelDisableTarget : null;


            samuelOpponentNegatePending = plan != null
                && samuelNegateTarget != null
                && samuelNegateTarget.IsMonster()
                && plan.SamuelReviveTarget != null
                && plan.SamuelReviveTarget.Attack
                    >= Math.Max(0, samuelNegateTarget.Attack);

            if (plan != null && plan.Mode == InterruptMode.SamuelPreemptSnakehair)
                pendingSnakehairDisableTarget = plan.FollowUpTarget;
            if (plan != null && plan.Mode == InterruptMode.SamuelPreemptMammoth)
                pendingMammothDestroyTarget = plan.FollowUpTarget;

            samuelReviveSelectionPending = plan != null;
            samuelFieldEffectCommittedThisTurn = plan != null;
            plannedSamuelReviveResolved = false;
            selectedSamuelReviveId = 0;
            if (plan != null)
            {
                SetStrategicPlan(
                    plan.Mode == InterruptMode.SamuelReactiveNegate
                        || plan.Mode == InterruptMode.SamuelPreemptNegate
                        || plan.Mode == InterruptMode.SamuelPreemptSnakehair
                        ? StrategicGoal.NegateImmediateThreat
                        : plan.Mode == InterruptMode.SamuelReactiveReviveBeforeControl
                            ? StrategicGoal.PreserveEndboard
                            : StrategicGoal.RemoveImmediateThreat,
                    currentComboRoute,
                    plan.Reason);
                DebugRoute("INTERRUPT mode=" + plan.Mode
                    + " revive=" + plannedSamuelReviveId
                    + " target=" + (plan.EnemyTarget != null
                        ? plan.EnemyTarget.Id.ToString() : "0")
                    + " reason=" + plan.Reason);
                ClearEnemyCommitment("Samuel plan committed");
            }
        }

        private void ClearSamuelInterruptPlan()
        {
            pendingInterruptMode = InterruptMode.Hold;
            plannedSamuelReviveId = 0;
            plannedSamuelReviveResolved = false;
            samuelReviveSelectionPending = false;
            samuelOwnTurnValueRevivePending = false;
            samuelOpponentNegatePending = false;
            samuelNegateTarget = null;
        }

        private bool CanVarudrasNegateCurrentChain()
        {
            if (Duel.LastChainPlayer != 1 || Duel.CurrentChain.Count == 0)
                return false;
            ClientCard last = Util.GetLastChainCard();
            if (!IsOpponentChainWorthNegating(last))
                return false;
            if (last.IsCode(CardId.UpstartGoblin))
            {
                DebugRoute("HOLD Varudras: do not spend material on Upstart Goblin");
                return false;
            }

            InterruptPlan samuelPlan = BuildSamuelOpponentPlan();
            return samuelPlan == null
                || samuelPlan.Mode != InterruptMode.SamuelReactiveNegate;
        }

        private void RefreshOpponentFaceupBeforeChainSnapshot()
        {
            opponentFaceupBeforeCurrentChain.Clear();
            opponentFaceupBeforeCurrentChain.AddRange(
                Enemy.GetMonsters().Concat(Enemy.GetSpells())
                    .Where(c => c != null && c.IsOnField() && c.IsFaceup()));
        }

        private void TrackOpponentFaceupMonsterAtOpenState(ClientCard card)
        {
            if (card == null || Duel.CurrentChain.Count > 0)
                return;

            opponentFaceupBeforeCurrentChain.RemoveAll(c => MatchesCard(c, card));
            if (card.Controller == 1
                && card.Location == CardLocation.MonsterZone
                && card.IsFaceup())
            {
                opponentFaceupBeforeCurrentChain.Add(card);
            }
        }

        private bool WasAlreadyFaceupBeforeCurrentChain(ClientCard card)
        {
            return card != null
                && opponentFaceupBeforeCurrentChain.Any(c => MatchesCard(c, card));
        }

        private void ClearVarudrasDestroyPlan()
        {
            pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
            pendingVarudrasDestroyTarget = null;
            varudrasNegatedChainSource = null;
            varudrasNegatedSourceWasAlreadyFaceup = false;
        }

        private bool IsLiveVarudrasEnemyTarget(ClientCard card)
        {
            return card != null
                && card.Controller == 1
                && card.IsOnField();
        }

        private ClientCard GetPersistentVarudrasNegatedSource(
            IEnumerable<ClientCard> source = null)
        {
            if (!varudrasNegatedSourceWasAlreadyFaceup
                || varudrasNegatedChainSource == null)
            {
                return null;
            }

            IEnumerable<ClientCard> pool = source
                ?? Enemy.GetMonsters().Concat(Enemy.GetSpells());
            return pool.FirstOrDefault(c => IsLiveVarudrasEnemyTarget(c)
                && MatchesCard(c, varudrasNegatedChainSource));
        }

        private ClientCard FindOtherVarudrasEnemyTarget(
            IEnumerable<ClientCard> source = null)
        {
            List<ClientCard> pool = (source
                    ?? Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                .Where(IsLiveVarudrasEnemyTarget)
                .Where(c => !MatchesCard(c, varudrasNegatedChainSource))
                .ToList();

            return GetEnemyFieldPriority(pool, false)
                .FirstOrDefault(IsLiveVarudrasEnemyTarget);
        }


        private bool NeedsNonZombieExtraDeckSummon()
        {
            return !zombieLockedThisTurn && !CanAcceptZombieLock();
        }

        private bool HasVampireGraceInRouteState()
        {
            if (Bot.HasInGraveyard(CardId.VampireGrace)
                || Bot.HasInMonstersZone(CardId.VampireGrace, faceUp: true))
            {
                return true;
            }


            return Bot.GetMonsters().Any(xyz => xyz != null
                && xyz.Overlays != null
                && xyz.Overlays.Contains(CardId.VampireGrace));
        }

        private bool IsVampireGraceRouteLive()
        {
            return vampireGraceRouteActive && HasVampireGraceInRouteState();
        }

        private bool IsUnlinkedFaceupCall(ClientCard card)
        {
            return card != null
                && card.Controller == 0
                && card.Location == CardLocation.SpellZone
                && card.IsFaceup()
                && card.IsCode(CardId.CallOfTheHaunted)
                && GetCallLinkedMonster(card) == null;
        }

        private ClientCard GetCallLinkedMonster(ClientCard call)
        {
            if (call == null
                || !call.IsCode(CardId.CallOfTheHaunted)
                || call.TargetCards == null)
            {
                return null;
            }


            return Bot.GetMonsters().FirstOrDefault(monster => monster != null
                && monster.Controller == 0
                && monster.Location == CardLocation.MonsterZone
                && call.TargetCards.Any(target => MatchesCard(monster, target)));
        }

        private bool CanCommitFreshSetCallToEldlichRoute()
        {
            return Duel.Player == 0
                && callSetByPumpking
                && !zombieLockedThisTurn
                && !dominusImpulseHandLock
                && HasEldlichRouteExtraDeck()
                && HasEldlichLinkSeedOnField()
                && HasOpenMainMonsterZone()
                && Bot.HasInGraveyard(CardId.EldlichTheGoldenLord);
        }

        private bool IsFreshSetCallForEldlich(ClientCard card)
        {
            return card != null
                && card.Controller == 0
                && card.Location == CardLocation.SpellZone
                && card.IsFacedown()
                && card.IsCode(CardId.CallOfTheHaunted)
                && MatchesCard(card, freshSetCallByPumpkingInstance)
                && CanCommitFreshSetCallToEldlichRoute();
        }

        private bool IsSpentDeltaFieldCost(ClientCard card)
        {
            return card != null
                && card.Controller == 0
                && card.Location == CardLocation.SpellZone
                && card.IsFaceup()
                && card.IsCode(CardId.DeltaOfInvitation)
                && MatchesCard(card, spentDeltaFieldInstance);
        }

        private int GetEldlichGraveFieldCostScore(ClientCard card)
        {
            if (card == null
                || card.Controller != 0
                || card.Location != CardLocation.SpellZone)
            {
                return int.MaxValue;
            }


            if (IsUnlinkedFaceupCall(card))
                return 0;
            if (IsSpentDeltaFieldCost(card))
                return 10;
            if (card.IsCode(CardId.FlyingMary))
                return 20;
            if (card.IsFacedown() && card.IsCode(CardId.VortexOfTime))
                return 30;
            if (IsFreshSetCallForEldlich(card))
                return 40;

            return int.MaxValue;
        }

        private ClientCard GetBestEldlichGraveFieldCost(
            IEnumerable<ClientCard> source = null)
        {
            IEnumerable<ClientCard> pool = source ?? Bot.GetSpells();
            return pool
                .Where(c => GetEldlichGraveFieldCostScore(c) < int.MaxValue)
                .OrderBy(GetEldlichGraveFieldCostScore)
                .ThenBy(c => c.Sequence)
                .FirstOrDefault();
        }

        private bool HasSafeEldlichGraveFieldCost()
        {
            return GetBestEldlichGraveFieldCost() != null;
        }

        private bool PlanVarudrasPostNegateDestroy()
        {
            pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
            pendingVarudrasDestroyTarget = null;


            ClientCard persistentSource = GetPersistentVarudrasNegatedSource();
            if (persistentSource != null)
            {
                pendingVarudrasDestroyMode =
                    VarudrasDestroyMode.PostNegateRemoval;
                pendingVarudrasDestroyTarget = persistentSource;
                DebugRoute("VARUDRAS post-negate plan=source target="
                    + persistentSource.Id
                    + " reason=source was already face-up before Chain");
                return true;
            }


            ClientCard otherEnemyTarget = FindOtherVarudrasEnemyTarget();
            if (otherEnemyTarget != null)
            {
                pendingVarudrasDestroyMode =
                    VarudrasDestroyMode.PostNegateRemoval;
                pendingVarudrasDestroyTarget = otherEnemyTarget;
                DebugRoute("VARUDRAS post-negate plan=other enemy target="
                    + otherEnemyTarget.Id
                    + " reason=negated source will not remain");
                return true;
            }

            DebugRoute("VARUDRAS post-negate destroy=False reason="
                + "negated source will leave and no other enemy field card");
            return false;
        }

        private ClientCard FindBestVarudrasTargetFromPrompt(
            IList<ClientCard> cards)
        {
            if (cards == null)
                return null;

            ClientCard persistentSource =
                GetPersistentVarudrasNegatedSource(cards);
            if (persistentSource != null)
                return persistentSource;

            return FindOtherVarudrasEnemyTarget(cards);
        }

        private IList<ClientCard> SelectVarudrasDestroyTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || cards.Count == 0)
            {
                ClearVarudrasDestroyPlan();
                return null;
            }

            ClientCard planned =
                FindMatchingCandidate(cards, pendingVarudrasDestroyTarget);
            if (IsLiveVarudrasEnemyTarget(planned))
            {
                string targetStage = pendingVarudrasDestroyMode
                    == VarudrasDestroyMode.TriggeredEnemyRemoval
                    ? "triggered" : "post-negate";
                DebugRoute("VARUDRAS " + targetStage + " target="
                    + planned.Id
                    + " mode=" + pendingVarudrasDestroyMode);
                ClearVarudrasDestroyPlan();
                return Util.CheckSelectCount(
                    new List<ClientCard> { planned }, cards, min, max);
            }


            ClientCard recalculated = FindBestVarudrasTargetFromPrompt(cards);
            if (recalculated != null)
            {
                DebugRoute("VARUDRAS recalculated enemy target="
                    + recalculated.Id
                    + " mode=" + pendingVarudrasDestroyMode);
                ClearVarudrasDestroyPlan();
                return Util.CheckSelectCount(
                    new List<ClientCard> { recalculated }, cards, min, max);
            }

            DebugRoute("ERROR Varudras destroy prompt has no enemy target; refuse friendly fallback");
            ClearVarudrasDestroyPlan();
            return null;
        }

        private bool HasSmallPumpkingOnField()
        {
            return Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true);
        }

        private bool IsFriendlyChainInProgress()
        {
            return Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 0;
        }

        private bool HasChangshiOnField()
        {
            return Bot.HasInMonstersZone(CardId.ChangshiTheSpiridao, faceUp: true);
        }

        private bool HasSamuelOnField()
        {
            return Bot.HasInMonstersZone(CardId.OfficiatorOfDoomSamuel, faceUp: true);
        }

        private bool HasGreatPumpkingOnField()
        {
            return Bot.HasInMonstersZone(CardId.PumpkingTheGreatGhostKing, faceUp: true);
        }

        private bool HasSettableCallForPumpking()
        {
            return Bot.HasInDeck(CardId.CallOfTheHaunted)
                || Bot.Graveyard.Any(c => c != null
                    && c.IsCode(CardId.CallOfTheHaunted));
        }

        private bool HasPumpkingDeckSummonTargetInDeck()
        {
            return Bot.HasInDeck(
                CardId.Hublot,
                CardId.GreatMammothOfTheNetherworld,
                CardId.ChangshiTheSpiridao,
                CardId.StareOfTheSnakeHair,
                CardId.OfficiatingReverie,
                CardId.ArmyOfTheHaunted,
                CardId.VampireGrace
            );
        }

        private bool CanUsePumpkingHandEffectNow()
        {
            return Duel.Player == 0
                && HasPumpkingInHand()
                && !pumpkingHandEffectAttempted
                && HasSettableCallForPumpking()
                && HasPumpkingDeckSummonTargetInDeck()
                && HasOpenSpellZone()
                && GetOpenMainMonsterZoneCount() >= 2;
        }

        private bool CanRecoverPumpkingFromHublotNow()
        {


            return Duel.Player == 0
                && !HasPumpkingInHand()
                && !pumpkingHandEffectAttempted
                && HasSettableCallForPumpking()
                && HasPumpkingDeckSummonTargetInDeck()
                && HasOpenSpellZone()
                && GetOpenMainMonsterZoneCount() >= 2;
        }

        private bool HasImmediatePumpkingActionPending()
        {
            if (CanUsePumpkingHandEffectNow())
                return true;


            bool unusedSetCallAvailable = callSetByPumpking
                && Bot.GetSpells().Any(c => c != null
                    && c.IsCode(CardId.CallOfTheHaunted)
                    && c.IsFacedown());
            if (unusedSetCallAvailable
                && HasPumpkingInGrave()
                && !pumpkingSummonEffectAttempted
                && GetOpenMainMonsterZoneCount() >= 2
                && HasPumpkingDeckSummonTargetInDeck())
            {
                return true;
            }

            if (HasSmallPumpkingOnField()
                && !HasChangshiOnField()
                && !pumpkingSummonEffectAttempted
                && HasOpenMainMonsterZone()
                && HasPumpkingDeckSummonTargetInDeck())
            {
                return true;
            }
            if (HasChangshiOnField() && !changshiMillAttempted)
                return true;
            return false;
        }
        private bool CanTakeProductivePumpkingMainDeckStepNow()
        {
            if (Duel.Player != 0)
                return false;


            if (summonCount > 0
                && HasOpenMainMonsterZone()
                && HasHublotInHand())
            {
                return true;
            }

            return HasImmediatePumpkingActionPending();
        }

        private bool CanTakeProductivePumpkingActionNow()
        {
            return CanTakeProductivePumpkingMainDeckStepNow()
                || CanTakeProductivePumpkingExtraDeckStep();
        }

        private bool HasSafeGreatPumpkingBounceTarget()
        {
            if (!HasGreatPumpkingOnField())
                return false;


            if (GetEnemyFieldPriority().Count > 0)
                return true;


            return GetGreatPumpkingOwnUtilityBounceTarget(
                Bot.GetMonsters().Concat(Bot.GetSpells())) != null;
        }

        private ClientCard GetGreatPumpkingOwnUtilityBounceTarget(
            IEnumerable<ClientCard> source)
        {
            if (source == null)
                return null;

            ClientCard ash = source.FirstOrDefault(c => c != null
                && c.Controller == 0
                && c.Location == CardLocation.MonsterZone
                && c.IsFaceup()
                && c.IsCode(CardId.AshBlossom));
            if (ash != null)
                return ash;

            ClientCard gravityController = source.FirstOrDefault(c => c != null
                && c.Controller == 0
                && c.Location == CardLocation.MonsterZone
                && c.IsFaceup()
                && c.IsCode(CardId.GravityController));
            if (gravityController != null)
                return gravityController;

            List<ClientCard> faceupCalls = source.Where(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.SpellZone
                    && c.IsFaceup()
                    && c.IsCode(CardId.CallOfTheHaunted))
                .ToList();
            if (faceupCalls.Count >= 2)
            {


                return faceupCalls.FirstOrDefault(c => GetCallLinkedMonster(c) == null);
            }

            return null;
        }

        private bool HasFaceupCall()
        {
            return Bot.HasInSpellZone(CardId.CallOfTheHaunted, notDisabled: true, faceUp: true);
        }

        private bool HasAnyCall()
        {
            return Bot.HasInHand(CardId.CallOfTheHaunted)
                || Bot.HasInSpellZone(CardId.CallOfTheHaunted)
                || Bot.HasInGraveyard(CardId.CallOfTheHaunted);
        }

        private bool HasFaceupFieldSpell()
        {
            return Bot.SpellZone[5] != null && Bot.SpellZone[5].IsFaceup();
        }

        private bool HasOpenMainMonsterZone()
        {
            return Bot.MonsterZone.Take(5).Any(c => c == null);
        }

        private int GetOpenMainMonsterZoneCount()
        {
            return Bot.MonsterZone.Take(5).Count(c => c == null);
        }

        private bool HasOpenSpellZone()
        {
            return Bot.GetSpellCountWithoutField() < 5;
        }

        private bool IsZombie(ClientCard card)
        {
            return card != null && card.IsMonster() && card.HasRace(CardRace.Zombie);
        }

        private bool IsLevel6Zombie(ClientCard card)
        {
            return IsZombie(card)
                && card.Level == 6
                && !card.HasType(CardType.Xyz | CardType.Link);
        }

        private bool IsKnownRank6ZombieXyz(ClientCard card)
        {


            return card != null
                && card.IsFaceup()
                && IsZombie(card)
                && card.HasType(CardType.Xyz)
                && card.IsCode(
                    CardId.OfficiatorOfDoomSamuel,
                    CardId.PumpkingTheGreatGhostKing,
                    CardId.DhampirVampireSheridan,
                    CardId.WollowFounderOfTheDrudgeDragons);
        }

        private List<ClientCard> GetUndyingOverlayBases()
        {
            return Bot.GetMonsters()
                .Where(c => IsKnownRank6ZombieXyz(c)
                    && c.Overlays != null
                    && c.Overlays.Count >= 1)
                .ToList();
        }

        private bool CanMammothDestroyTarget(ClientCard card)
        {
            if (!IsLiveEnemyFieldCard(card) || card.IsShouldNotBeTarget())
                return false;


            if (card.Location == CardLocation.MonsterZone)
            {
                return card.IsFaceup()
                    && !card.IsMonsterInvincible();
            }

            return card.Location == CardLocation.SpellZone;
        }

        private bool CanUseEarthMonsterEffects()
        {
            return !dominusImpulseHandLock;
        }

        private bool CanUseWindMonsterEffects()
        {
            return !dominusImpulseHandLock;
        }

        private bool CanUseLightMonsterEffects()
        {
            return !dominusImpulseHandLock;
        }

        private bool WillCardEffectBeNegated(ClientCard card = null)
        {
            return DefaultCheckWhetherCardEffectWillBeNegated(card ?? Card);
        }

        private bool IsOpponentChainWorthNegating(ClientCard card)
        {
            if (card == null || card.Controller != 1 || card.IsDisabled())
                return false;
            if (currentNegateCardList.Contains(card))
                return false;


            return true;
        }

        private List<ClientCard> GetEnemyFieldPriority(IEnumerable<ClientCard> source = null, bool targetableOnly = true)
        {
            List<ClientCard> pool = (source ?? Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                .Where(c => c != null && c.Controller == 1 && c.IsOnField())
                .ToList();

            if (targetableOnly)
                pool.RemoveAll(c => c.IsShouldNotBeTarget());

            List<ClientCard> result = new List<ClientCard>();
            ClientCard last = Util.GetLastChainCard();
            if (last != null && pool.Contains(last) && !last.IsDisabled())
                result.Add(last);

            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFaceup() && !c.IsDisabled()
                    && c.IsMonsterShouldBeDisabledBeforeItUseEffect())
                .OrderByDescending(c => c.GetDefensePower()));

            result.AddRange(pool.Where(c => c.IsSpell() || c.IsTrap())
                .Where(c => c.IsFaceup() && c.HasType(CardType.Field | CardType.Continuous | CardType.Equip)));

            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFaceup())
                .OrderByDescending(c => c.GetDefensePower()));
            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap()) && c.IsFacedown()));
            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFacedown()));
            result.AddRange(pool);

            return result.Distinct().ToList();
        }

        private List<ClientCard> GetGreatPumpkingEnemyBouncePriority(
            IEnumerable<ClientCard> source = null)
        {
            List<ClientCard> pool = (source
                    ?? Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                .Where(c => c != null
                    && c.Controller == 1
                    && c.IsOnField()
                    && !c.IsShouldNotBeTarget())
                .ToList();

            List<ClientCard> result = new List<ClientCard>();


            result.AddRange(pool.Where(c => c.IsMonster()
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && (IsKnownLiveNegateMonster(c)
                        || c.IsFloodgate()
                        || c.IsMonsterDangerous()
                        || c.IsMonsterInvincible()
                        || c.IsMonsterShouldBeDisabledBeforeItUseEffect()))
                .OrderByDescending(c => IsKnownLiveNegateMonster(c))
                .ThenByDescending(c => c.IsFloodgate())
                .ThenByDescending(c => c.GetDefensePower()));


            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap())
                && c.IsFaceup()
                && c.IsFloodgate()));


            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap())
                && c.IsFacedown()));


            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFaceup())
                .OrderByDescending(c => c.GetDefensePower()));
            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFacedown()));
            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap())
                && c.IsFaceup()));
            result.AddRange(pool);

            return result.Distinct().ToList();
        }

        private bool IsKnownLiveNegateMonster(ClientCard card)
        {
            return card != null
                && card.Controller == 1
                && card.Location == CardLocation.MonsterZone
                && card.IsFaceup()
                && !card.IsDisabled()
                && card.IsCode(
                    CardId.Varudras,
                    CardId.EvolzarLars,
                    CardId.OfficiatorOfDoomSamuel,
                    CardId.DoomkingBalerdroch,
                    CardId.CrystalWingSynchroDragon);
        }

        private List<ClientCard> GetEldlichHandTargetPriority(
            IEnumerable<ClientCard> source)
        {
            List<ClientCard> pool = (source ?? Enemy.GetMonsters().Concat(Enemy.GetSpells()))
                .Where(c => c != null
                    && c.Controller == 1
                    && c.IsOnField()
                    && !c.IsShouldNotBeTarget())
                .ToList();

            List<ClientCard> result = new List<ClientCard>();
            ClientCard last = Util.GetLastChainCard();
            if (last != null && pool.Contains(last) && last.IsMonster()
                && last.IsFaceup() && !last.IsDisabled())
            {
                result.Add(last);
            }


            result.AddRange(pool.Where(IsKnownLiveNegateMonster)
                .OrderByDescending(c => c.GetDefensePower()));
            result.AddRange(pool.Where(c => c.IsMonster()
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && (c.IsMonsterShouldBeDisabledBeforeItUseEffect()
                        || c.IsFloodgate()
                        || c.IsMonsterDangerous()
                        || c.IsMonsterInvincible()))
                .OrderByDescending(c => c.GetDefensePower()));

            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap())
                && c.IsFaceup()
                && (c.IsFloodgate()
                    || c.HasType(CardType.Field | CardType.Continuous | CardType.Equip))));
            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFaceup())
                .OrderByDescending(c => c.GetDefensePower()));
            result.AddRange(pool.Where(c => (c.IsSpell() || c.IsTrap()) && c.IsFacedown()));
            result.AddRange(pool.Where(c => c.IsMonster() && c.IsFacedown()));
            result.AddRange(pool);

            return result.Distinct().ToList();
        }

        private IList<ClientCard> SelectEldlichHandFieldTarget(
            IList<ClientCard> cards, int min, int max)
        {
            if (cards == null || min <= 0)
                return null;

            List<ClientCard> result = GetEldlichHandTargetPriority(cards)
                .Where(cards.Contains)
                .ToList();
            if (result.Count < min)
                return null;

            int take = Math.Min(min, Math.Min(max, result.Count));
            return result.Take(take).ToList();
        }

        private List<ClientCard> GetEnemyGravePriority(IEnumerable<ClientCard> source = null)
        {
            List<ClientCard> pool = (source ?? Enemy.Graveyard)
                .Where(c => c != null && c.Controller == 1 && c.Location == CardLocation.Grave)
                .ToList();

            ClientCard chainTarget = Duel.ChainTargets.LastOrDefault(c => c != null && pool.Contains(c));
            List<ClientCard> result = new List<ClientCard>();
            if (chainTarget != null)
                result.Add(chainTarget);

            result.AddRange(pool.Where(c => c.IsMonster() && c.HasType(CardType.Effect))
                .OrderByDescending(c => c.Attack));
            result.AddRange(pool.Where(c => c.IsSpell() || c.IsTrap()));
            result.AddRange(pool);
            return result.Distinct().ToList();
        }

        private IList<ClientCard> SelectByIdPriority(
            IList<ClientCard> cards,
            int min,
            int max,
            params int[] ids)
        {
            List<ClientCard> result = new List<ClientCard>();
            foreach (int id in ids)
            {
                ClientCard target = cards.FirstOrDefault(c => c.IsCode(id));
                if (target != null && !result.Contains(target))
                    result.Add(target);
                if (result.Count >= max)
                    break;
            }
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private IList<ClientCard> SelectHublotXyzTarget(
            IList<ClientCard> cards,
            int min,
            int max,
            params int[] ids)
        {
            IList<ClientCard> selected = SelectByIdPriority(cards, min, max, ids);
            ClientCard target = selected != null ? selected.FirstOrDefault() : null;
            selectedHublotXyzId = target != null ? target.Id : 0;
            return selected;
        }

        private IList<ClientCard> SelectEnemyField(
            IList<ClientCard> cards,
            int min,
            int max,
            bool targetableOnly = true)
        {
            List<ClientCard> result = GetEnemyFieldPriority(cards, targetableOnly)
                .Where(cards.Contains)
                .Take(max)
                .ToList();
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private IList<ClientCard> SelectEnemyGrave(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> result = GetEnemyGravePriority(cards)
                .Where(cards.Contains)
                .Take(max)
                .ToList();
            return Util.CheckSelectCount(result, cards, min, max);
        }



        private bool HasPumpkingInHand()
        {
            return Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts);
        }

        private bool HasPumpkingInGrave()
        {
            return Bot.HasInGraveyard(CardId.PumpkingTheKingOfGraveGhosts);
        }

        private bool HasPumpkingAccessible()
        {
            return HasPumpkingInHand() || HasPumpkingInGrave()
                || Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true);
        }

        private bool HasHublotOnField()
        {
            return Bot.GetMonsters().Any(c => IsHublot(c) && c.IsFaceup());
        }

        private bool HasHublotInHand()
        {


            return Bot.Hand.Any(IsHublot);
        }

        private int GetEctoplasmicSearchTargetId()
        {


            if (!HasHublotInHand())
            {
                if (Bot.HasInDeck(CardId.Hublot))
                    return CardId.Hublot;

                if (!HasPumpkingInHand()
                    && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts))
                {
                    return CardId.PumpkingTheKingOfGraveGhosts;
                }

                return 0;
            }

            if (!HasPumpkingInHand()
                && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts))
            {
                return CardId.PumpkingTheKingOfGraveGhosts;
            }

            return 0;
        }

        private bool CanPlanVampireSuckerBridge()
        {
            if (Duel.Player != 0 || Duel.Turn <= 1
                || !Bot.HasInExtra(CardId.VampireSucker)
                || !Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
            {
                return false;
            }


            if (Bot.HasInMonstersZone(CardId.FallenAngelOfTheGoldenLand, faceUp: true))
                return false;

            return Bot.Hand.Any(c => c.IsSpell() || c.IsTrap())
                || Bot.GetSpells().Any(c => c.IsSpell() || c.IsTrap());
        }

        private bool IsSpentZombieXyz(ClientCard card)
        {
            return card != null && card.IsFaceup() && IsZombie(card)
                && card.HasType(CardType.Xyz)
                && (card.Overlays == null || card.Overlays.Count == 0);
        }

        private bool IsExpendableZombieLinkMaterial(ClientCard card)
        {
            return IsDeltaToken(card) || IsSpentZombieXyz(card);
        }

        private bool IsInExtraMonsterZone(ClientCard card)
        {
            return card != null && card.Location == CardLocation.MonsterZone
                && card.Sequence >= 5;
        }

        private bool IsVampireSuckerCandidateMaterial(ClientCard card)
        {
            return card != null && card.IsFaceup() && IsZombie(card)
                && !card.HasType(CardType.Link);
        }

        private ClientCard GetNonLinkEmzOccupant()
        {
            return Bot.GetMonsters().FirstOrDefault(c => c != null
                && c.IsFaceup() && IsInExtraMonsterZone(c)
                && !c.HasType(CardType.Link));
        }

        private ClientCard GetMandatoryVampireSuckerEmzMaterial()
        {


            bool existingLink = Bot.GetMonsters().Any(c => c != null
                && c.IsFaceup() && c.HasType(CardType.Link));
            if (existingLink)
                return null;

            ClientCard occupant = GetNonLinkEmzOccupant();
            return IsVampireSuckerCandidateMaterial(occupant) ? occupant : null;
        }

        private bool HasUnusableVampireSuckerEmzBlocker()
        {
            bool existingLink = Bot.GetMonsters().Any(c => c != null
                && c.IsFaceup() && c.HasType(CardType.Link));
            if (existingLink)
                return false;

            ClientCard occupant = GetNonLinkEmzOccupant();
            return occupant != null && !IsVampireSuckerCandidateMaterial(occupant);
        }

        private int GetVampireSuckerMaterialLoss(ClientCard card)
        {
            if (card == null)
                return 9999;
            if (IsDeltaToken(card))
                return 0;

            if (card.HasType(CardType.Xyz))
            {
                int overlays = card.Overlays == null ? 0 : card.Overlays.Count;
                if (overlays == 0)
                    return 2;


                int baseLoss;
                if (card.IsCode(CardId.TheUndyingLegion))
                    baseLoss = 260;
                else if (card.IsCode(CardId.OfficiatorOfDoomSamuel))
                    baseLoss = 220;
                else if (card.IsCode(CardId.PumpkingTheGreatGhostKing))
                    baseLoss = 210;
                else if (card.IsCode(CardId.DhampirVampireSheridan))
                    baseLoss = GetEnemyFieldPriority().Count > 0 ? 210 : 170;
                else if (card.IsCode(CardId.WollowFounderOfTheDrudgeDragons))
                    baseLoss = 140 + Enemy.Graveyard.Count * 10;
                else
                    baseLoss = 170;

                return baseLoss + overlays * 45;
            }


            return 45 + GetZombieLinkMaterialValue(card) * 4;
        }

        private int GetVampireSuckerBridgeValue(ClientCard mandatoryEmz)
        {


            int value = 150;
            if (Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
                value += 45;
            if (GetOpenMainMonsterZoneCount() <= 1)
                value += 35;
            if (mandatoryEmz != null && IsSpentZombieXyz(mandatoryEmz))
                value += 35;
            return value;
        }

        private List<ClientCard> GetVampireSuckerMaterialPlan()
        {
            if (HasUnusableVampireSuckerEmzBlocker())
            {
                DebugRoute("HOLD Sucker: non-Zombie/non-material monster blocks the EMZ");
                return new List<ClientCard>();
            }

            List<ClientCard> candidates = Bot.GetMonsters()
                .Where(IsVampireSuckerCandidateMaterial)
                .OrderBy(GetVampireSuckerMaterialLoss)
                .ThenBy(GetZombieLinkMaterialValue)
                .ThenBy(c => c.Attack)
                .ToList();
            if (candidates.Count < 2)
                return new List<ClientCard>();

            ClientCard mandatoryEmz = GetMandatoryVampireSuckerEmzMaterial();
            List<ClientCard> plan = new List<ClientCard>();
            if (mandatoryEmz != null)
            {
                plan.Add(mandatoryEmz);
                ClientCard partner = candidates.FirstOrDefault(c => c != mandatoryEmz);
                if (partner == null)
                    return new List<ClientCard>();
                plan.Add(partner);
            }
            else
            {
                plan.AddRange(candidates.Take(2));
            }

            int loss = plan.Sum(GetVampireSuckerMaterialLoss);
            int bridgeValue = GetVampireSuckerBridgeValue(mandatoryEmz);
            string planLog = "SUCKER material plan="
                + string.Join(",", plan.Select(c => c.Id + "#" + c.Sequence
                    + "(mat=" + (c.Overlays == null ? 0 : c.Overlays.Count) + ")").ToArray())
                + " loss=" + loss + " value=" + bridgeValue
                + " mandatoryEmz=" + (mandatoryEmz == null ? 0 : mandatoryEmz.Id);
            if (planLog != lastSuckerMaterialPlanLog)
            {
                DebugRoute(planLog);
                lastSuckerMaterialPlanLog = planLog;
            }

            return loss <= bridgeValue ? plan : new List<ClientCard>();
        }

        private ClientCard GetVampireSuckerTokenPartner()
        {
            if (HasUnusableVampireSuckerEmzBlocker())
                return null;

            List<ClientCard> candidates = Bot.GetMonsters()
                .Where(IsVampireSuckerCandidateMaterial)
                .OrderBy(GetVampireSuckerMaterialLoss)
                .ThenBy(GetZombieLinkMaterialValue)
                .ThenBy(c => c.Attack)
                .ToList();
            if (candidates.Count == 0)
                return null;

            ClientCard mandatoryEmz = GetMandatoryVampireSuckerEmzMaterial();
            return mandatoryEmz ?? candidates.First();
        }

        private bool IsVampireSuckerTokenPlanWorthwhile(out int tokenLoss)
        {
            tokenLoss = int.MaxValue;
            ClientCard partner = GetVampireSuckerTokenPartner();
            if (partner == null)
                return false;

            ClientCard mandatoryEmz = GetMandatoryVampireSuckerEmzMaterial();
            tokenLoss = GetVampireSuckerMaterialLoss(partner);
            int bridgeValue = GetVampireSuckerBridgeValue(mandatoryEmz);
            DebugRoute("SUCKER token plan partner=" + partner.Id + "#" + partner.Sequence
                + " mat=" + (partner.Overlays == null ? 0 : partner.Overlays.Count)
                + " loss=" + tokenLoss + " value=" + bridgeValue
                + " mandatoryEmz=" + (mandatoryEmz == null ? 0 : mandatoryEmz.Id));
            return tokenLoss <= bridgeValue;
        }

        private bool HasFaceupFallenAngel()
        {
            return Bot.HasInMonstersZone(CardId.FallenAngelOfTheGoldenLand, faceUp: true);
        }

        private bool HasDedicatedFlyingMaryRank10ExtraDeck()
        {
            return Bot.HasInExtra(CardId.FlyingMary)
                && Bot.HasInExtra(CardId.Varudras);
        }

        private bool HasRecoverableEldlichForFlyingMary()
        {
            return Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.Banished.Any(c => c != null
                    && c.IsCode(CardId.EldlichTheGoldenLord));
        }

        private int GetFlyingMaryPartnerLoss(ClientCard card)
        {
            if (card == null)
                return 9999;
            if (IsDeltaToken(card))
                return 0;
            if (IsSpentZombieXyz(card))
                return 5;
            if (card.HasType(CardType.Xyz))
                return GetVampireSuckerMaterialLoss(card);


            if (card.Level < 5)
                return 35;
            if (card.IsCode(CardId.Hublot))
                return 95;
            if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
                return 150;
            return 110 + GetZombieLinkMaterialValue(card) * 2;
        }

        private bool ShouldCreateDeltaTokenForFlyingMaryRank10()
        {
            if (!HasDedicatedFlyingMaryRank10ExtraDeck()
                || Bot.GetMonsters().Any(IsDeltaToken))
            {
                return false;
            }

            bool routeAvailable = eldlichRouteActive
                || eldlichRouteMarySummoned
                || currentComboRoute == ComboRoute.EldlichRank10
                || currentComboRoute == ComboRoute.BrickEldlich;
            if (!routeAvailable)
                return false;

            bool fallenAlreadyPresent = HasFaceupFallenAngel();
            if (!fallenAlreadyPresent
                && !Bot.HasInExtra(CardId.FallenAngelOfTheGoldenLand))
            {
                return false;
            }

            bool eldlichOnField = Bot.HasInMonstersZone(
                CardId.EldlichTheGoldenLord, faceUp: true);
            bool eldlichReady = HasRecoverableEldlichForFlyingMary()
                || eldlichOnField;
            if (!eldlichReady)
                return false;


            int requiredOpenZones = (fallenAlreadyPresent || eldlichOnField) ? 1 : 2;
            if (GetOpenMainMonsterZoneCount() < requiredOpenZones)
                return false;

            List<ClientCard> partners = Bot.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && !c.IsCode(CardId.EldlichTheGoldenLord)
                    && !c.IsCode(CardId.FallenAngelOfTheGoldenLand)
                    && IsAllowedZombieLinkMaterial(c))
                .OrderBy(GetFlyingMaryPartnerLoss)
                .ThenBy(GetZombieLinkMaterialValue)
                .ToList();

            if (partners.Count == 0)
            {
                DebugRoute("Delta Token value: supplies missing Flying Mary partner");
                return true;
            }

            ClientCard cheapest = partners[0];
            int loss = GetFlyingMaryPartnerLoss(cheapest);
            if (loss >= 80)
            {
                DebugRoute("Delta Token value: preserve Flying Mary partner="
                    + cheapest.Id + " loss=" + loss);
                return true;
            }

            DebugRoute("HOLD Delta Token: cheap Flying Mary partner already exists id="
                + cheapest.Id + " loss=" + loss);
            return false;
        }

        private bool CanAssemblePersistentFlyingMaryRoute()
        {
            if (Bot.HasInMonstersZone(CardId.FlyingMary, faceUp: true))
                return HasRecoverableEldlichForFlyingMary();
            if (!HasFaceupFallenAngel()
                || !HasRecoverableEldlichForFlyingMary()
                || !Bot.HasInExtra(CardId.FlyingMary))
            {
                return false;
            }

            List<ClientCard> materials = GetFlyingMaryEldlichMaterials();
            if (materials.Count == 2)
                return HasFlyingMaryRank10FollowupZoneCapacity(materials);


            return Bot.HasInSpellZone(CardId.DeltaOfInvitation, faceUp: true)
                && HasOpenMainMonsterZone();
        }

        private void RestorePersistentEldlichRouteState()
        {
            bool rank10AlreadyMade = Bot.HasInMonstersZone(CardId.Varudras, faceUp: true);
            if (rank10AlreadyMade)
                return;

            bool maryOnField = Bot.HasInMonstersZone(CardId.FlyingMary, faceUp: true);
            bool fallenOnField = HasFaceupFallenAngel();
            bool eldlichRecoverable = HasRecoverableEldlichForFlyingMary();

            if (maryOnField && eldlichRecoverable)
            {
                eldlichRouteActive = true;
                eldlichRouteMarySummoned = true;
                eldlichRouteRank10CommitPending = true;
                DebugRoute("RESTORE Flying Mary Rank 10 commit from persistent board");
                return;
            }

            if (fallenOnField && eldlichRecoverable
                && Bot.HasInExtra(CardId.FlyingMary)
                && CanAssemblePersistentFlyingMaryRoute())
            {
                eldlichRouteActive = true;
                eldlichRouteRank10CommitPending = true;
                DebugRoute("RESTORE Fallen Angel -> Flying Mary Rank 10 commit");
            }
        }

        private bool ShouldCreateDeltaTokenNow()
        {
            if (Duel.Player != 0 || !HasOpenMainMonsterZone())
                return false;


            if (HasImmediatePumpkingActionPending()
                || IsPumpkingComboInProgress()
                || currentComboRoute == ComboRoute.NormalPumpking
                || currentComboRoute == ComboRoute.UraraRecovery
                || currentStrategicGoal == StrategicGoal.SecurePumpking
                || currentStrategicGoal == StrategicGoal.ProduceLevel6Bodies
                || currentStrategicGoal == StrategicGoal.CompleteNormalPumpkingRoute
                || currentStrategicGoal == StrategicGoal.CompleteUraraRoute)
            {
                return false;
            }

            bool eldlichLinkRoute = currentComboRoute == ComboRoute.EldlichRank10
                || currentComboRoute == ComboRoute.BrickEldlich
                || eldlichRouteActive;
            if (!eldlichLinkRoute || Bot.GetMonsters().Any(IsDeltaToken))
                return false;


            if (ShouldCreateDeltaTokenForFlyingMaryRank10())
                return true;

            if (!CanPlanVampireSuckerBridge())
                return false;

            int tokenLoss;
            if (!IsVampireSuckerTokenPlanWorthwhile(out tokenLoss))
            {
                DebugRoute("HOLD Delta Token: mandatory EMZ/live material costs more than Sucker draw");
                return false;
            }

            List<ClientCard> noTokenPlan = GetVampireSuckerMaterialPlan();
            if (noTokenPlan.Count < 2)
            {
                DebugRoute("Delta Token value: creates the only profitable legal Sucker pair");
                return true;
            }

            int noTokenLoss = noTokenPlan.Sum(GetVampireSuckerMaterialLoss);


            if (tokenLoss + 30 < noTokenLoss)
            {
                DebugRoute("Delta Token value: lowers Sucker material loss from "
                    + noTokenLoss + " to " + tokenLoss);
                return true;
            }

            DebugRoute("HOLD Delta Token: existing Sucker pair is already efficient");
            return false;
        }

        private bool ShouldPreferVampireSuckerOverFlyingMary()
        {
            if (!Bot.HasInExtra(CardId.VampireSucker))
                return false;

            if (eldlichRouteRank10CommitPending
                || eldlichRouteMarySummoned
                || (eldlichRouteActive && HasFaceupFallenAngel()))
            {
                return false;
            }

            List<ClientCard> plan = GetVampireSuckerMaterialPlan();
            if (!CanPlanVampireSuckerBridge() || plan.Count != 2)
                return false;


            bool surplusDrawBridge = HasCorePumpkingEndboard()
                && plan.All(c => IsDeltaToken(c) || IsSpentZombieXyz(c))
                && plan.Sum(GetVampireSuckerMaterialLoss) <= 10;


            bool recoveryDrawBridge = IsLaterTurnRecoveryState()
                && GetFlyingMaryPumpkingResetMaterials().Count == 2
                && plan.Sum(GetVampireSuckerMaterialLoss) <= 400;

            return surplusDrawBridge || recoveryDrawBridge;
        }

        private bool ShouldFlyingMaryRevivePumpkingForComeback()
        {


            if (Duel.Player != 0 || Duel.Turn <= 2
                || pumpkingSummonEffectAttempted
                || pumpkingSummonEffectResolved)
            {
                return false;
            }

            return Bot.Graveyard.Any(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                    && IsZombie(c)
                    && c.Level >= 5
                    && c.IsCanRevive())
                || Bot.Banished.Any(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                    && IsZombie(c)
                    && c.Level >= 5);
        }

        private bool CanSnakehairReachPumpkingBeforeHublot()
        {


            bool ectoplasmicHasStarterTarget =
                (!HasHublotInHand() && Bot.HasInDeck(CardId.Hublot))
                || (HasHublotInHand() && !HasPumpkingInHand()
                    && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts));

            return Bot.GetMonsterCount() == 0
                && !ectoplasmicSearchUsed
                && !Bot.HasInHand(CardId.EctoplasmicFortification)
                && Bot.HasInHand(CardId.StareOfTheSnakeHair)
                && Bot.HasInDeck(CardId.EctoplasmicFortification)
                && ectoplasmicHasStarterTarget
                && DefaultCheckWhetherBotCanSearch();
        }

        private bool CanEctoplasmicReachPumpkingBeforeHublot()
        {
            return !ectoplasmicSearchUsed
                && !HasPumpkingInHand()
                && Bot.GetMonsterCount() == 0
                && Bot.HasInHand(CardId.EctoplasmicFortification)
                && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts)
                && DefaultCheckWhetherBotCanSearch();
        }

        private bool ShouldDelayHublotForPumpkingSearch()
        {
            return CanSnakehairReachPumpkingBeforeHublot()
                || CanEctoplasmicReachPumpkingBeforeHublot();
        }

        private bool HasUsefulEldlichCost()
        {
            return Bot.Hand.Any(c => c != null && (c.IsSpell() || c.IsTrap()))
                || Bot.GetSpells().Any(c => c != null);
        }

        private bool HasSeriousEnemyProblem()
        {
            return Enemy.GetMonsters().Any(c => c != null && c.IsFaceup()
                    && (c.IsFloodgate() || c.IsMonsterDangerous() || c.IsMonsterInvincible()))
                || Enemy.GetSpells().Any(c => c != null && c.IsFaceup() && c.IsFloodgate());
        }

        private bool ShouldRecoverEldlichFromHublot()
        {
            return HasSeriousEnemyProblem() && HasUsefulEldlichCost() && CanUseLightMonsterEffects();
        }

        private bool CanPayVampireGraceCost()
        {
            return Bot.LifePoints > 2000;
        }

        private bool HasVampireGraceRank6Payoff()
        {
            return Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                || Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                || Bot.HasInExtra(CardId.DhampirVampireSheridan)
                || Bot.HasInExtra(CardId.EvolzarLars)
                || Bot.HasInExtra(CardId.WollowFounderOfTheDrudgeDragons);
        }

        private bool CanPrimeVampireGraceFromHublot(IList<ClientCard> cards)
        {
            return Duel.Player == 0
                && !changshiHandRescueRouteActive
                && CanPayVampireGraceCost()
                && HasVampireGraceRank6Payoff()
                && HasPumpkingInHand()
                && !pumpkingHandEffectAttempted
                && HasSettableCallForPumpking()
                && HasOpenSpellZone()
                && GetOpenMainMonsterZoneCount() >= 3
                && Bot.HasInDeck(CardId.EldlichTheGoldenLord)
                && cards != null
                && cards.Any(c => c != null && c.IsCode(CardId.VampireGrace));
        }

        private bool CanPrimeVampireGraceFromChangshi()
        {
            if (Duel.Player != 0
                || !CanPayVampireGraceCost()
                || !HasVampireGraceRank6Payoff()
                || GetOpenMainMonsterZoneCount() < 2
                || Bot.HasInGraveyard(CardId.VampireGrace)
                || Bot.HasInMonstersZone(CardId.VampireGrace, faceUp: true))
            {
                return false;
            }

            bool armyTrigger = HasFaceupCall()
                && !armySpecialSummonEffectCommittedThisTurn
                && (Bot.HasInHand(CardId.ArmyOfTheHaunted)
                    || Bot.Graveyard.Any(c => c != null
                        && c.IsCode(CardId.ArmyOfTheHaunted)
                        && c.IsCanRevive()));
            bool reverieTrigger = Bot.HasInHand(CardId.OfficiatingReverie)
                && Bot.Hand.Count > 1;
            return armyTrigger || reverieTrigger;
        }

        private bool CanDiscardVampireGraceForReverie()
        {
            return Duel.Player == 0
                && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                && CanPayVampireGraceCost()
                && HasVampireGraceRank6Payoff()
                && GetOpenMainMonsterZoneCount() >= 2;
        }

        private bool CanStartChangshiHandRescueRoute(IList<ClientCard> cards)
        {
            if (Duel.Player != 0 || changshiHandRescueRouteActive
                || !Bot.HasInHand(CardId.ChangshiTheSpiridao)
                || Bot.HasInHand(CardId.OfficiatingReverie))
            {
                return false;
            }


            bool hublotAccessible = HasHublotInHand()
                || Bot.HasInMonstersZone(CardId.Hublot, faceUp: true)
                || Bot.HasInDeck(CardId.Hublot)
                || (cards != null && cards.Any(c => c != null && IsHublot(c)));
            bool reverieAvailable = Bot.HasInDeck(CardId.OfficiatingReverie)
                || (cards != null && cards.Any(c => c != null
                    && c.Location == CardLocation.Deck
                    && c.IsCode(CardId.OfficiatingReverie)));
            bool pumpkingAvailable = HasPumpkingAccessible()
                || Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts);
            bool callAvailable = HasAnyCall() || HasSettableCallForPumpking();

            return hublotAccessible
                && reverieAvailable
                && pumpkingAvailable
                && callAvailable
                && Bot.HasInDeck(CardId.ArmyOfTheHaunted)
                && Bot.HasInDeck(CardId.EldlichTheGoldenLord)
                && Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                && Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                && Bot.HasInExtra(CardId.TheUndyingLegion)
                && HasEldlichRouteExtraDeck();
        }

        private bool CanContinueChangshiHandRescueBeforeUndying()
        {
            if (!changshiHandRescueRouteActive
                || changshiHandRescueEldlichLoaded
                || Duel.Player != 0)
            {
                return false;
            }

            if (HasChangshiOnField()
                && !changshiMillAttempted
                && Bot.HasInDeck(CardId.EldlichTheGoldenLord))
            {
                return true;
            }

            ClientCard samuel = Bot.GetMonsters().FirstOrDefault(c => c != null
                && c.IsCode(CardId.OfficiatorOfDoomSamuel)
                && c.IsFaceup());
            if (samuel != null
                && samuel.Overlays != null
                && samuel.Overlays.Count > 0
                && !samuelFieldEffectCommittedThisTurn
                && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao))
            {
                return true;
            }

            if (HasGreatPumpkingOnField())
            {
                if (CanUsePumpkingHandEffectNow()
                    || HasImmediatePumpkingActionPending())
                {
                    return true;
                }

                List<ClientCard> materials = GetSamuelMaterials();
                if (!HasSamuelOnField()
                    && materials.Count == 2
                    && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao))
                {
                    return true;
                }
            }

            bool hublotAndReverieReady = Bot.GetMonsters().Any(c => c != null
                    && c.IsFaceup() && IsHublot(c))
                && Bot.HasInMonstersZone(CardId.OfficiatingReverie, faceUp: true);
            if (!HasGreatPumpkingOnField() && hublotAndReverieReady)
                return true;

            return Bot.HasInHand(CardId.OfficiatingReverie)
                && Bot.HasInHand(CardId.ChangshiTheSpiridao);
        }

        private int GetHublotSendTargetId(IList<ClientCard> cards)
        {
            if (changshiHandRescueRouteActive
                && Bot.HasInHand(CardId.ChangshiTheSpiridao)
                && cards.Any(c => c != null
                    && c.IsCode(CardId.OfficiatingReverie)))
            {
                DebugRoute("Hublot rescue send: Reverie for Changshi discard");
                return CardId.OfficiatingReverie;
            }

            if (CanStartChangshiHandRescueRoute(cards))
            {
                changshiHandRescueRouteActive = true;
                changshiHandRescueEldlichLoaded = false;
                DebugRoute("START Changshi-hand rescue: Hublot send/recover Reverie");
                return CardId.OfficiatingReverie;
            }

            if (CanPrimeVampireGraceFromHublot(cards))
            {
                vampireGraceRouteActive = true;
                DebugRoute("START Vampire Grace route: Hublot loads Grace before Pumpking/Changshi");
                return CardId.VampireGrace;
            }

            bool pumpkingReady = HasPumpkingInHand() || HasPumpkingInGrave()
                || pumpkingSearchSucceeded;

            if (!pumpkingReady)
            {
                ClientCard pumpking = cards.FirstOrDefault(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                if (pumpking != null)
                    return CardId.PumpkingTheKingOfGraveGhosts;
            }

            if (!Bot.HasInGraveyard(CardId.OfficiatingReverie)
                && cards.Any(c => c.IsCode(CardId.OfficiatingReverie)))
            {
                return CardId.OfficiatingReverie;
            }

            if (!Bot.HasInGraveyard(CardId.ArmyOfTheHaunted)
                && cards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                return CardId.ArmyOfTheHaunted;
            }


            if (!HasFaceupFieldSpell()
                && !Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                && cards.Any(c => c.IsCode(CardId.EldlichTheGoldenLord)))
            {
                return CardId.EldlichTheGoldenLord;
            }

            if (HasFaceupFieldSpell()
                && !Bot.HasInGraveyard(CardId.DoomkingBalerdroch)
                && cards.Any(c => c.IsCode(CardId.DoomkingBalerdroch)))
            {
                return CardId.DoomkingBalerdroch;
            }

            if (cards.Any(c => c.IsCode(CardId.Mezuki)))
                return CardId.Mezuki;
            if (cards.Any(c => c.IsCode(CardId.ChangshiTheSpiridao)))
                return CardId.ChangshiTheSpiridao;
            if (cards.Any(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)))
                return CardId.PumpkingTheKingOfGraveGhosts;

            return cards.FirstOrDefault()?.Id ?? 0;
        }

        private bool CanAcceptZombieLock()
        {
            if (zombieLockedThisTurn)
                return true;


            if (Bot.HasInExtra(CardId.Varudras)
                && Bot.GetMonsters().Count(c => c.IsFaceup() && c.Level == 10
                    && !c.HasType(CardType.Xyz | CardType.Link)) >= 2)
            {
                return false;
            }

            return true;
        }
        private static readonly int[] ChangshiKnownDeckTargets =
        {
            CardId.VampireGrace,
            CardId.AshBlossom,
            CardId.Mezuki,
            CardId.Hublot,
            CardId.PumpkingTheKingOfGraveGhosts,
            CardId.GreatMammothOfTheNetherworld,
            CardId.StareOfTheSnakeHair,
            CardId.ArmyOfTheHaunted,
            CardId.DoomkingBalerdroch,
            CardId.EldlichTheGoldenLord,
            CardId.OfficiatingReverie
        };

        private bool HasChangshiDeckTargetAvailable()
        {


            return GetChangshiMillTargetId(null, false) != 0;
        }

        private int GetChangshiMillTargetId(
            IList<ClientCard> cards,
            bool logDecision = true)
        {


            List<ClientCard> deckCards = cards == null
                ? new List<ClientCard>()
                : cards.Where(c => c != null && c.Location == CardLocation.Deck).ToList();
            Func<int, bool> available = id => cards == null
                ? Bot.HasInDeck(id)
                : deckCards.Any(c => c.IsCode(id));

            if (changshiHandRescueRouteActive
                && HasSamuelOnField()
                && available(CardId.EldlichTheGoldenLord))
            {
                if (logDecision)
                    DebugRoute("Changshi rescue target: Eldlich from Deck");
                return CardId.EldlichTheGoldenLord;
            }

            if (IsVampireGraceRouteLive()
                && available(CardId.EldlichTheGoldenLord))
            {
                if (logDecision)
                    DebugRoute("Changshi Grace route target: Eldlich from Deck");
                return CardId.EldlichTheGoldenLord;
            }

            if (ShouldStartAshReplayLine(available(CardId.AshBlossom)))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Ash Blossom from Deck (Urara route)");
                return CardId.AshBlossom;
            }

            bool hasLevel6ReviveTarget = Bot.Graveyard.Any(c => IsLevel6Zombie(c) && c.IsCanRevive()
                && !c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));

            if (available(CardId.VampireGrace)
                && CanPrimeVampireGraceFromChangshi())
            {
                vampireGraceRouteActive = true;
                if (logDecision)
                    DebugRoute("Changshi target: Vampire Grace with guaranteed Army/Reverie trigger");
                return CardId.VampireGrace;
            }


            if (Bot.HasInHand(CardId.OfficiatingReverie)
                && available(CardId.ArmyOfTheHaunted))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Army from Deck; preserve hand Reverie extender");
                return CardId.ArmyOfTheHaunted;
            }


            if (!hasLevel6ReviveTarget
                && available(CardId.OfficiatingReverie))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Reverie from Deck for Samuel revive");
                return CardId.OfficiatingReverie;
            }


            if (Duel.Turn >= 2
                && Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive())
                && available(CardId.Mezuki))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Mezuki from Deck (turn 2+ extender)");
                return CardId.Mezuki;
            }


            if (HasFaceupFieldSpell()
                && !Bot.HasInGraveyard(CardId.DoomkingBalerdroch)
                && !Bot.HasInMonstersZone(CardId.DoomkingBalerdroch, faceUp: true)
                && available(CardId.DoomkingBalerdroch))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Doomking from Deck for self-revive");
                return CardId.DoomkingBalerdroch;
            }

            if (available(CardId.ArmyOfTheHaunted))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Army from Deck fallback");
                return CardId.ArmyOfTheHaunted;
            }
            if (available(CardId.EldlichTheGoldenLord))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Eldlich from Deck fallback");
                return CardId.EldlichTheGoldenLord;
            }
            if (available(CardId.Mezuki))
            {
                if (logDecision)
                    DebugRoute("Changshi target: Mezuki from Deck fallback");
                return CardId.Mezuki;
            }
            if (available(CardId.OfficiatingReverie))
                return CardId.OfficiatingReverie;
            if (available(CardId.GreatMammothOfTheNetherworld))
                return CardId.GreatMammothOfTheNetherworld;
            if (available(CardId.PumpkingTheKingOfGraveGhosts))
                return CardId.PumpkingTheKingOfGraveGhosts;
            if (available(CardId.Hublot))
                return CardId.Hublot;
            if (available(CardId.StareOfTheSnakeHair))
                return CardId.StareOfTheSnakeHair;


            if (logDecision)
                DebugRoute("HOLD Changshi mill: no profitable Deck target");
            return 0;
        }

        private IList<ClientCard> SelectPumpkingDeckSummonTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (Duel.Player == 1)
            {


                if (pendingSnakehairDisableTarget != null
                    && IsLiveEnemyMonster(pendingSnakehairDisableTarget)
                    && pendingSnakehairDisableTarget.IsAttack()
                    && cards.Any(c => c.IsCode(CardId.StareOfTheSnakeHair)))
                {
                    DebugRoute("Pumpking opponent-turn bridge: fetch Snakehair target="
                        + pendingSnakehairDisableTarget.Id);
                    return SelectByIdPriority(cards, min, max,
                        CardId.StareOfTheSnakeHair);
                }

                if (pendingMammothDestroyTarget != null
                    && IsLiveEnemyFieldCard(pendingMammothDestroyTarget)
                    && !pendingMammothDestroyTarget.IsShouldNotBeTarget()
                    && cards.Any(c => c.IsCode(CardId.GreatMammothOfTheNetherworld)))
                {
                    DebugRoute("Pumpking opponent-turn bridge: fetch Mammoth target="
                        + pendingMammothDestroyTarget.Id);
                    return SelectByIdPriority(cards, min, max,
                        CardId.GreatMammothOfTheNetherworld);
                }

                ClientCard dangerousAttack = Enemy.GetMonsters()
                    .Where(ShouldPreemptWithSnakehair)
                    .OrderByDescending(c => c.IsMonsterDangerous())
                    .ThenByDescending(c => c.Attack)
                    .FirstOrDefault();
                if (dangerousAttack != null
                    && cards.Any(c => c.IsCode(CardId.StareOfTheSnakeHair)))
                {
                    pendingSnakehairDisableTarget = dangerousAttack;
                    DebugRoute("Pumpking opponent-turn summon: Snakehair target="
                        + dangerousAttack.Id);
                    return SelectByIdPriority(cards, min, max,
                        CardId.StareOfTheSnakeHair);
                }

                ClientCard mammothTarget = GetMammothPreemptTarget();
                if (mammothTarget != null
                    && cards.Any(c => c.IsCode(CardId.GreatMammothOfTheNetherworld)))
                {
                    pendingMammothDestroyTarget = mammothTarget;
                    DebugRoute("Pumpking opponent-turn summon: Mammoth target="
                        + mammothTarget.Id);
                    return SelectByIdPriority(cards, min, max,
                        CardId.GreatMammothOfTheNetherworld);
                }


                DebugRoute("Pumpking opponent-turn target disappeared: forced recovery body");
                return SelectByIdPriority(cards, min, max,
                    CardId.ArmyOfTheHaunted,
                    CardId.OfficiatingReverie,
                    CardId.ChangshiTheSpiridao,
                    CardId.StareOfTheSnakeHair,
                    CardId.GreatMammothOfTheNetherworld,
                    CardId.Hublot);
            }

            if (!changshiHandRescueRouteActive
                && CanStartChangshiHandRescueRoute(cards)
                && cards.Any(c => c != null && IsHublot(c)))
            {
                changshiHandRescueRouteActive = true;
                changshiHandRescueEldlichLoaded = false;
                DebugRoute("START Changshi-hand rescue: Pumpking fetch Hublot");
                return SelectByIdPriority(cards, min, max, CardId.Hublot);
            }

            if (changshiHandRescueRouteActive
                && Bot.HasInHand(CardId.ChangshiTheSpiridao)
                && cards.Any(c => c != null && IsHublot(c)))
            {
                DebugRoute("Pumpking rescue summon target: Hublot; convert Changshi hand brick");
                return SelectByIdPriority(cards, min, max, CardId.Hublot);
            }

            if (changshiHandRescueRouteActive
                && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao)
                && cards.Any(c => c != null && c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Pumpking rescue summon target: Army; preserve Changshi for Samuel");
                return SelectByIdPriority(cards, min, max,
                    CardId.ArmyOfTheHaunted);
            }

            return SelectByIdPriority(cards, min, max,
                CardId.ChangshiTheSpiridao,
                CardId.ArmyOfTheHaunted,
                CardId.OfficiatingReverie,
                CardId.VampireGrace,
                CardId.Hublot,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair);
        }

        private IList<ClientCard> SelectChangshiDeckTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            List<ClientCard> deckCards = cards
                .Where(c => c != null && c.Location == CardLocation.Deck)
                .ToList();
            selectedChangshiMillId = GetChangshiMillTargetId(deckCards);

            ClientCard target = selectedChangshiMillId == 0
                ? null
                : deckCards.FirstOrDefault(c => c.IsCode(selectedChangshiMillId));
            if (target == null)
            {
                DebugRoute("BLOCK Changshi selection: no profitable Deck candidate; never spend a hand card");
                return null;
            }

            selectedChangshiMillId = target.Id;
            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
        }

        private bool IsPumpkingComboInProgress()
        {
            return pumpkingComboState != PumpkingComboState.None
                && pumpkingComboState != PumpkingComboState.UndyingSummoned;
        }

        private bool CanContinueCorePumpkingExtraDeckLineNow()
        {
            if (Duel.Player != 0 || zombieLockedThisTurn)
                return false;


            if (HasImmediatePumpkingActionPending())
                return true;

            if (ashReplayLineActive)
            {
                if (!HasGreatPumpkingOnField()
                    && Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                    && GetGreatPumpkingMaterials().Count == 2)
                {
                    return true;
                }

                if (HasGreatPumpkingOnField()
                    && !HasSamuelOnField()
                    && Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                    && GetSamuelMaterials().Count == 2)
                {
                    return true;
                }
            }
            else
            {


                if (!HasGreatPumpkingOnField()
                    && Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                    && GetRank6Materials(true).Count >= 2)
                {
                    return true;
                }

                if (!HasSamuelOnField()
                    && Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel))
                {
                    List<ClientCard> samuelMaterials = GetSamuelMaterials();
                    if (samuelMaterials.Count == 2
                        && CanNormalSamuelFirstReachGreatPumpking(samuelMaterials))
                    {
                        return true;
                    }
                }
            }


            if (Bot.HasInExtra(CardId.TheUndyingLegion)
                && GetUndyingOverlayBases().Count > 0)
            {
                return true;
            }

            return false;
        }

        private bool ShouldHoldGenericRank6ForPumpkingCombo()
        {
            if (!IsPumpkingComboInProgress())
                return false;

            if (CanContinueCorePumpkingExtraDeckLineNow())
                return true;


            DebugRoute("RELEASE stale Pumpking combo state=" + pumpkingComboState
                + ": no executable Great Pumpking/Samuel/Undying step; allow generic Rank 6");
            pumpkingComboState = PumpkingComboState.None;
            if (currentComboRoute == ComboRoute.NormalPumpking
                || currentComboRoute == ComboRoute.UraraRecovery)
            {
                currentComboRoute = ComboRoute.None;
            }
            return false;
        }

        private int CountFreeLevel6ForGreatPumpking()
        {
            return Bot.GetMonsters().Count(c => IsLevel6Zombie(c));
        }

        private bool CanMakeLink2WithoutGreatPumpking()
        {
            int otherZombieCount = Bot.GetMonsters().Count(c => c.IsFaceup() && IsZombie(c)
                && !c.IsCode(CardId.PumpkingTheGreatGhostKing));
            return otherZombieCount >= 2
                && (Bot.HasInExtra(CardId.FlyingMary) || Bot.HasInExtra(CardId.VampireSucker));
        }

        private bool CanTakeProductivePumpkingExtraDeckStep()
        {
            if (Duel.Player != 0 || zombieLockedThisTurn)
                return false;


            bool liveGreatPumpkingSearch = Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                && !HasGreatPumpkingOnField()
                && !greatPumpkingSearchAttempted
                && !greatPumpkingSearchResolved
                && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts)
                && DefaultCheckWhetherBotCanSearch();
            if (liveGreatPumpkingSearch && GetRank6Materials(true).Count >= 2)
                return true;


            if (HasGreatPumpkingOnField() && !HasSamuelOnField())
            {
                if (Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                    && GetRank6Materials(false).Count >= 2)
                {
                    return true;
                }

                if (HasUnusedArmySpecialSummonAvailable()
                    && NeedsAdditionalLevel6BodyForCurrentPlan())
                {
                    return true;
                }
            }


            if (HasGreatPumpkingOnField() && HasSamuelOnField()
                && Bot.HasInExtra(CardId.TheUndyingLegion))
            {
                return true;
            }


            return HasImmediatePumpkingActionPending();
        }

        private bool ShouldHoldRank6ForFlyingMaryRank10()
        {
            if (!eldlichRouteRank10CommitPending)
                return false;


            return !Bot.HasInMonstersZone(CardId.Varudras, faceUp: true);
        }

        private bool ShouldContinueEldlichLine()
        {
            if (zombieLockedThisTurn || dominusImpulseHandLock)
                return false;

            bool eldlichAvailable = Bot.HasInHand(CardId.EldlichTheGoldenLord)
                || Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true);
            bool fieldAvailable = HasFaceupFieldSpell()
                || Bot.HasInHand(CardId.DeltaOfInvitation)
                || Bot.HasInHand(CardId.Terraforming);
            return eldlichAvailable || fieldAvailable;
        }

        private bool HasDirectPumpkingLineAvailable()
        {
            return HasPumpkingAccessible()
                || HasHublotInHand()
                || CanSnakehairReachPumpkingBeforeHublot()
                || CanEctoplasmicReachPumpkingBeforeHublot();
        }

        private void ObservePumpkingStarterState()
        {
            if (pumpkingStarterSeenThisDuel)
                return;

            bool snakehairBridge = Bot.HasInHand(CardId.StareOfTheSnakeHair)
                && (Bot.HasInHand(CardId.EctoplasmicFortification)
                    || Bot.HasInDeck(CardId.EctoplasmicFortification));
            if (Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts)
                || HasHublotInHand()
                || Bot.HasInHand(CardId.EctoplasmicFortification)
                || snakehairBridge
                || pumpkingComboState != PumpkingComboState.None)
            {
                pumpkingStarterSeenThisDuel = true;
            }
        }

        private bool HasEldlichLinkSeedOnField()
        {
            return Bot.GetMonsters().Any(c => c.IsFaceup() && IsZombie(c)
                && !c.IsCode(CardId.EldlichTheGoldenLord)
                && !c.HasType(CardType.Link));
        }

        private bool HasEldlichRouteExtraDeck()
        {
            return Bot.HasInExtra(CardId.FallenAngelOfTheGoldenLand)
                && Bot.HasInExtra(CardId.FlyingMary)
                && Bot.HasInExtra(CardId.EldlichTheMadGoldenLord)
                && Bot.HasInExtra(CardId.Varudras);
        }

        private bool HasEldlichCostBesidesDelta()
        {
            return Bot.Hand.Any(c => c != null
                    && !c.IsCode(CardId.DeltaOfInvitation)
                    && (c.IsSpell() || c.IsTrap()))
                || Bot.GetSpells().Any(c => c != null
                    && !c.IsCode(CardId.DeltaOfInvitation));
        }

        private bool CanStartEldlichRoute()
        {
            if (zombieLockedThisTurn || dominusImpulseHandLock
                || !HasEldlichRouteExtraDeck() || !HasOpenMainMonsterZone())
            {
                return false;
            }

            bool eldlichCanReachGrave = Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInDeck(CardId.EldlichTheGoldenLord);
            if (!eldlichCanReachGrave)
                return false;


            return HasEldlichLinkSeedOnField() || HasEldlichCostBesidesDelta();
        }

        private bool CanContinueEldlichRouteFromCurrentBoard()
        {
            if (zombieLockedThisTurn || dominusImpulseHandLock || !HasEldlichRouteExtraDeck())
                return false;
            if (!HasEldlichLinkSeedOnField())
                return false;

            if (Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
                return true;


            return Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                && HasUsefulEldlichCost()
                && HasOpenMainMonsterZone();
        }

        private bool ShouldStartAshReplayLine(bool ashAvailable)
        {


            if (Duel.Turn != 1 || !HasFaceupCall() || !HasHublotOnField())
                return false;
            if (!Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true)
                || !Bot.HasInMonstersZone(CardId.ChangshiTheSpiridao, faceUp: true))
                return false;
            bool armyAvailable = Bot.HasInHand(CardId.ArmyOfTheHaunted)
                || Bot.HasInGraveyard(CardId.ArmyOfTheHaunted)
                || Bot.HasInDeck(CardId.ArmyOfTheHaunted);
            if (!ashAvailable || !armyAvailable)
                return false;
            if (!Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                || !Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                || !Bot.HasInExtra(CardId.TheUndyingLegion))
                return false;


            return !CanContinueEldlichRouteFromCurrentBoard();
        }

        private IList<ClientCard> SelectPumpkingDiscard(IList<ClientCard> cards, int min, int max)
        {
            if (!HasPumpkingInGrave())
            {
                ClientCard self = cards.FirstOrDefault(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                if (self != null)
                {
                    DebugRoute("Pumpking discard target: self");
                    return Util.CheckSelectCount(new List<ClientCard> { self }, cards, min, max);
                }
            }

            List<ClientCard> nonPumpking = cards
                .Where(c => !c.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
                .ToList();
            DebugRoute("Pumpking discard target: preserve Pumpking already in GY");
            if (nonPumpking.Count > 0)
                return SelectDiscard(nonPumpking, min, Math.Min(max, nonPumpking.Count));
            return SelectDiscard(cards, min, max);
        }

        private ClientCard GetSamuelOwnTurnValueReviveTarget(
            IEnumerable<ClientCard> source,
            bool commitFollowUpTarget = false)
        {
            if (source == null
                || Duel.Player != 0
                || samuelFieldEffectCommittedThisTurn
                || HasConfirmedOpenStateBattleLethal()
                || ShouldHoldRank6ForFlyingMaryRank10()
                || eldlichRouteRank10CommitPending)
            {
                return null;
            }

            List<ClientCard> pool = source
                .Where(c => c != null
                    && IsZombie(c)
                    && c.IsCanRevive())
                .ToList();
            if (pool.Count == 0)
                return null;


            bool hasPumpkingDeckFollowUp = Bot.HasInDeck(
                CardId.ChangshiTheSpiridao,
                CardId.ArmyOfTheHaunted,
                CardId.OfficiatingReverie,
                CardId.Hublot,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair
            );
            if (!pumpkingSummonEffectAttempted
                && !pumpkingSummonEffectResolved
                && GetOpenMainMonsterZoneCount() >= 2
                && hasPumpkingDeckFollowUp)
            {
                ClientCard pumpking = pool.FirstOrDefault(c =>
                    c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                if (pumpking != null)
                    return pumpking;
            }


            if (!greatPumpkingSearchAttempted && !greatPumpkingSearchResolved)
            {
                ClientCard great = pool.FirstOrDefault(c =>
                    c.IsCode(CardId.PumpkingTheGreatGhostKing));
                if (great != null)
                    return great;
            }


            ClientCard mammothTarget = GetMammothPreemptTarget();
            if (mammothTarget != null)
            {
                ClientCard mammoth = pool.FirstOrDefault(c =>
                    c.IsCode(CardId.GreatMammothOfTheNetherworld));
                if (mammoth != null)
                {
                    if (commitFollowUpTarget)
                        pendingMammothDestroyTarget = mammothTarget;
                    return mammoth;
                }
            }

            ClientCard snakehairTarget = Enemy.GetMonsters()
                .Where(ShouldPreemptWithSnakehair)
                .OrderByDescending(c => IsKnownLiveNegateMonster(c))
                .ThenByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.Attack)
                .FirstOrDefault();
            if (snakehairTarget != null)
            {
                ClientCard snakehair = pool.FirstOrDefault(c =>
                    c.IsCode(CardId.StareOfTheSnakeHair));
                if (snakehair != null)
                {
                    if (commitFollowUpTarget)
                        pendingSnakehairDisableTarget = snakehairTarget;
                    return snakehair;
                }
            }


            bool hasExistingLevel6Partner = Bot.GetMonsters().Any(c =>
                c != null
                && c != Card
                && c.IsFaceup()
                && !c.HasType(CardType.Xyz | CardType.Link)
                && IsLevel6Zombie(c));
            if (!hasExistingLevel6Partner)
                return null;

            int[] priority =
            {
                CardId.OfficiatingReverie,
                CardId.VampireGrace,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.Hublot,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.PumpkingTheKingOfGraveGhosts
            };
            foreach (int id in priority)
            {
                ClientCard target = pool.FirstOrDefault(c =>
                    c.IsCode(id) && IsLevel6Zombie(c));
                if (target != null)
                    return target;
            }

            return pool.FirstOrDefault(IsLevel6Zombie);
        }

        private IList<ClientCard> SelectSamuelReviveTarget(IList<ClientCard> cards, int min, int max)
        {
            ClientCard target = null;

            if (pendingInterruptMode != InterruptMode.Hold)
            {
                target = cards.FirstOrDefault(c => c != null
                    && c.IsCode(plannedSamuelReviveId));
                if (target == null)
                {
                    DebugRoute("ERROR Samuel planned revive target unavailable mode="
                        + pendingInterruptMode + " id=" + plannedSamuelReviveId);
                    return null;
                }
            }
            else if (samuelOwnTurnValueRevivePending
                && plannedSamuelReviveId != 0)
            {
                target = cards.FirstOrDefault(c => c != null
                    && c.IsCode(plannedSamuelReviveId));
                if (target == null)
                {
                    DebugRoute("ERROR Samuel own-turn value target unavailable id="
                        + plannedSamuelReviveId);
                    samuelOwnTurnValueRevivePending = false;
                    return null;
                }
                samuelOwnTurnValueRevivePending = false;
            }
            else if (ashReplayLineActive)
            {


                target = cards.FirstOrDefault(c => c.IsCode(CardId.AshBlossom));
                if (target == null)
                {
                    DebugRoute("ERROR Samuel Urara target prompt has no Ash Blossom");
                    return null;
                }
            }
            else
            {


                List<ClientCard> level6Targets = cards
                    .Where(c => c != null && IsLevel6Zombie(c)
                        && !c.IsCode(CardId.AshBlossom))
                    .ToList();
                if (level6Targets.Count < min)
                {
                    DebugRoute("HOLD Samuel revive: no legal Level 6 continuation");
                    return null;
                }

                int[] priority =
                {
                    CardId.OfficiatingReverie,
                    CardId.GreatMammothOfTheNetherworld,
                    CardId.StareOfTheSnakeHair,
                    CardId.Hublot,
                    CardId.ArmyOfTheHaunted,
                    CardId.ChangshiTheSpiridao,
                    CardId.PumpkingTheKingOfGraveGhosts
                };

                foreach (int id in priority)
                {
                    target = level6Targets.FirstOrDefault(c => c.IsCode(id));
                    if (target != null)
                        break;
                }

                if (target == null)
                    target = level6Targets.FirstOrDefault();
            }

            selectedSamuelReviveId = target != null ? target.Id : 0;
            DebugRoute("Samuel revive target=" + selectedSamuelReviveId
                + (pendingInterruptMode != InterruptMode.Hold
                    ? " mode=" + pendingInterruptMode : string.Empty));
            if (target == null)
                return null;

            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
        }

        private IList<ClientCard> SelectGreatPumpkingSearchTarget(
            IList<ClientCard> cards, int min, int max)
        {
            List<int> priority = new List<int>();

            if (changshiHandRescueRouteActive
                && !pumpkingHandEffectAttempted
                && cards.Any(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)))
            {
                priority.Add(CardId.PumpkingTheKingOfGraveGhosts);
                DebugRoute("Great Pumpking rescue search: small Pumpking first");
            }


            bool needSmallPumpkingStarter = !pumpkingHandEffectAttempted
                && !HasPumpkingAccessible()
                && cards.Any(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (needSmallPumpkingStarter)
            {
                priority.Add(CardId.PumpkingTheKingOfGraveGhosts);
                DebugRoute("Great Pumpking search: recover route with small Pumpking first");
            }

            bool armyMakesImmediateSamuel = cards.Any(c => c != null
                    && c.IsCode(CardId.ArmyOfTheHaunted))
                && Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                && !HasSamuelOnField()
                && !armySpecialSummonEffectCommittedThisTurn
                && HasOpenMainMonsterZone()
                && Bot.GetMonsters().Any(c => c != null
                    && c.IsFaceup()
                    && !c.HasType(CardType.Xyz | CardType.Link)
                    && IsLevel6Zombie(c));
            if (armyMakesImmediateSamuel)
            {
                priority.Add(CardId.ArmyOfTheHaunted);
                DebugRoute("Great Pumpking search: Army first for immediate Samuel");
            }

            if (Duel.Turn == 1)
            {
                priority.Add(CardId.ArmyOfTheHaunted);
            }
            else
            {
                if (!activatedThisTurn.Contains(CardId.StareOfTheSnakeHair))
                    priority.Add(CardId.StareOfTheSnakeHair);
                priority.Add(CardId.EctoplasmicFortification);
            }

            if (!HasAnyCall())
                priority.Add(CardId.CallOfTheHaunted);
            if (HasFaceupCall() && !Bot.HasInHand(CardId.VortexOfTime)
                && !Bot.HasInSpellZone(CardId.VortexOfTime))
            {
                priority.Add(CardId.VortexOfTime);
            }
            priority.AddRange(new[]
            {
                CardId.ArmyOfTheHaunted,
                CardId.PumpkingTheKingOfGraveGhosts,
                CardId.StareOfTheSnakeHair,
                CardId.EctoplasmicFortification,
                CardId.VortexOfTime,
                CardId.CallOfTheHaunted
            });

            IList<ClientCard> selected = SelectByIdPriority(
                cards, min, max, priority.ToArray());
            ClientCard target = selected != null ? selected.FirstOrDefault() : null;
            selectedGreatPumpkingSearchId = target != null ? target.Id : 0;
            DebugRoute("Great Pumpking search priority="
                + string.Join(",", priority.Select(id => id.ToString()).ToArray())
                + " selected=" + selectedGreatPumpkingSearchId);
            return selected;
        }

        private IList<ClientCard> SelectGreatPumpkingBounceTargets(
            IList<ClientCard> cards, int min, int max)
        {
            if (cards == null || cards.Count == 0)
                return null;

            List<ClientCard> enemies = GetGreatPumpkingEnemyBouncePriority(cards)
                .Where(c => c != null && c.Controller == 1)
                .Distinct()
                .Take(max)
                .ToList();

            List<ClientCard> result = new List<ClientCard>();


            if (enemies.Count >= 2)
            {
                result.AddRange(enemies.Take(2));
            }
            else if (enemies.Count == 1)
            {
                result.Add(enemies[0]);
                if (result.Count < max)
                {
                    ClientCard ownUtility =
                        GetGreatPumpkingOwnUtilityBounceTarget(cards);
                    if (ownUtility != null)
                        result.Add(ownUtility);
                }
            }
            else
            {
                ClientCard ownUtility = GetGreatPumpkingOwnUtilityBounceTarget(cards);
                if (ownUtility != null)
                    result.Add(ownUtility);
            }

            if (result.Count < min)
            {
                DebugRoute("BLOCK Great Pumpking bounce selection: no profitable target");
                return null;
            }

            DebugRoute("Great Pumpking bounce targets: "
                + string.Join(",", result.Select(c => c.Id.ToString()).ToArray()));
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private IList<ClientCard> SelectXyzDetachMaterial(
            IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> ordered = new List<ClientCard>();


            bool reverieInGrave = Bot.HasInGraveyard(CardId.OfficiatingReverie);
            if (reverieInGrave)
            {
                ordered.AddRange(cards.Where(c => c != null
                    && c.HasType(CardType.Xyz)
                    && !ordered.Contains(c)));
            }


            bool pumpkingInGrave = HasPumpkingInGrave();
            if (!pumpkingInGrave)
            {
                ordered.AddRange(cards.Where(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                    && !ordered.Contains(c)));
            }

            int[] namedPriority =
            {
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.OfficiatingReverie,
                CardId.GreatMammothOfTheNetherworld
            };
            foreach (int id in namedPriority)
            {
                ordered.AddRange(cards.Where(c => c != null
                    && c.IsCode(id)
                    && !ordered.Contains(c)));
            }


            ordered.AddRange(cards.Where(c => c != null && !ordered.Contains(c))
                .OrderBy(GetMaterialValue)
                .ThenBy(c => c.Attack));

            IList<ClientCard> selected = Util.CheckSelectCount(ordered, cards, min, max);
            if (selected != null)
            {
                DebugRoute("XYZ DETACH priority="
                    + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray())
                    + "; reverieInGY=" + reverieInGrave
                    + "; pumpkingInGY=" + pumpkingInGrave);
            }
            return selected;
        }

        private IList<ClientCard> SelectDiscard(IList<ClientCard> cards, int min, int max)
        {


            int[] priority =
            {
                CardId.DoomkingBalerdroch,
                CardId.Mezuki,
                CardId.VampireGrace,
                CardId.OfficiatingReverie,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.EldlichTheGoldenLord,
                CardId.GreatMammothOfTheNetherworld,
                CardId.DeltaOfInvitation,
                CardId.StareOfTheSnakeHair,
                CardId.Hublot,
                CardId.PumpkingTheKingOfGraveGhosts,
                CardId.CallOfTheHaunted,
                CardId.EctoplasmicFortification,
                CardId.VortexOfTime,
                CardId.GhostBelle,
                CardId.MaxxC,
                CardId.AshBlossom,
                CardId.InfiniteImpermanence,
                CardId.DominusImpulse
            };
            return SelectByIdPriority(cards, min, max, priority);
        }

        private int GetReverieDiscardScore(
            ClientCard card,
            IList<ClientCard> candidates)
        {
            if (card == null)
                return int.MaxValue;

            int score = 500;
            int copies = candidates.Count(c => c != null && c.IsCode(card.Id));


            if (card.IsCode(CardId.Mezuki))
                return 0;


            if (card.IsCode(CardId.DoomkingBalerdroch))
                score = HasFaceupFieldSpell() ? 10 : 110;
            else if (card.IsCode(CardId.ArmyOfTheHaunted))
                score = HasFaceupCall() ? 15 : 105;
            else if (card.IsCode(CardId.EldlichTheGoldenLord))
                score = HasFaceupFieldSpell() ? 20 : 115;
            else if (card.IsCode(CardId.VampireGrace))
                score = CanDiscardVampireGraceForReverie() ? 5 : 170;
            else if (card.IsCode(CardId.GreatMammothOfTheNetherworld))
                score = 55;
            else if (card.IsCode(CardId.ChangshiTheSpiridao))
                score = 70;
            else if (IsGenericHandTrap(card))
            {


                score = activatedThisTurn.Contains(card.Id) ? 30 : 80;
            }
            else
            {
                score = GetGenericHandDispositionScore(card, candidates);
            }


            if (copies > 1)
                score -= 45 + Math.Min(copies - 1, 3) * 10;


            if (card.IsCode(CardId.Hublot,
                    CardId.StareOfTheSnakeHair,
                    CardId.EctoplasmicFortification))
            {
                score += 260;
            }
            if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                && !HasPumpkingInGrave())
            {
                score += 220;
            }
            if (card.IsCode(CardId.CallOfTheHaunted) && !HasFaceupCall())
                score += 190;
            if (card.IsCode(CardId.DeltaOfInvitation) && !HasFaceupFieldSpell())
                score += 170;

            return score;
        }

        private IList<ClientCard> SelectReverieDiscard(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || cards.Count < min)
                return null;

            if (changshiHandRescueRouteActive)
            {
                ClientCard changshi = cards.FirstOrDefault(c => c != null
                    && c.IsCode(CardId.ChangshiTheSpiridao));
                if (changshi != null)
                {
                    DebugRoute("REVERIE RESCUE DISCARD Changshi");
                    return Util.CheckSelectCount(
                        new List<ClientCard> { changshi }, cards, min, max);
                }

                DebugRoute("ABORT Changshi-hand rescue: Reverie prompt has no Changshi");
                changshiHandRescueRouteActive = false;
            }

            List<ClientCard> ordered = cards
                .Where(c => c != null)
                .OrderBy(c => GetReverieDiscardScore(c, cards))
                .ThenByDescending(c => cards.Count(x => x != null && x.IsCode(c.Id)))
                .ThenBy(c => c.Id)
                .ToList();

            IList<ClientCard> selected = Util.CheckSelectCount(ordered, cards, min, max);
            if (selected != null)
            {
                if (selected.Any(c => c != null && c.IsCode(CardId.VampireGrace)))
                {
                    vampireGraceRouteActive = true;
                    DebugRoute("REVERIE DISCARD Vampire Grace: Reverie summon will open Grace trigger");
                }
                DebugRoute("REVERIE DISCARD selected="
                    + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray()));
            }
            return selected;
        }

        private bool IsGenericHandTrap(ClientCard card)
        {
            return card != null && card.IsCode(
                CardId.GhostBelle,
                CardId.MaxxC,
                CardId.AshBlossom,
                CardId.InfiniteImpermanence,
                CardId.DominusImpulse);
        }

        private bool HasGenericGraveyardValue(ClientCard card)
        {
            if (card == null)
                return false;
            if (card.IsCode(CardId.VampireGrace))
                return CanDiscardVampireGraceForReverie();

            return card.IsCode(
                CardId.DoomkingBalerdroch,
                CardId.Mezuki,
                CardId.OfficiatingReverie,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.EldlichTheGoldenLord,
                CardId.GreatMammothOfTheNetherworld);
        }

        private int GetGenericHandDispositionScore(
            ClientCard card,
            IList<ClientCard> handCandidates)
        {
            if (card == null)
                return int.MaxValue;

            int score = 500;
            int copies = handCandidates.Count(c => c != null && c.IsCode(card.Id));


            if (copies > 1)
                score -= 500 + Math.Min(copies - 1, 3) * 20;


            if (IsGenericHandTrap(card) && activatedThisTurn.Contains(card.Id))
                score -= 400;


            if (HasGenericGraveyardValue(card))
                score -= 180;


            if (IsGenericHandTrap(card))
                score -= 40;
            if (card.IsCode(CardId.Terraforming)
                && (HasFaceupFieldSpell()
                    || Bot.HasInHand(CardId.DeltaOfInvitation)))
            {
                score -= 140;
            }
            if (card.IsCode(CardId.CallOfTheHaunted) && HasFaceupCall())
                score -= 100;


            if (card.IsCode(CardId.Hublot,
                    CardId.StareOfTheSnakeHair,
                    CardId.EctoplasmicFortification))
            {
                score += 320;
            }
            if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
            {
                score += HasPumpkingInGrave() ? 80 : 260;
                if (HasFaceupCall())
                    score -= 120;
            }
            if (card.IsCode(CardId.DeltaOfInvitation) && !HasFaceupFieldSpell())
                score += 180;
            if (card.IsCode(CardId.CallOfTheHaunted) && !HasFaceupCall())
                score += 160;

            return score;
        }

        private IList<ClientCard> SelectGenericHandDisposition(
            IList<ClientCard> cards,
            int min,
            int max,
            int hint)
        {
            if (cards == null || min <= 0)
                return null;

            List<ClientCard> hand = cards
                .Where(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Hand)
                .ToList();
            if (hand.Count != cards.Count || hand.Count < min)
                return null;

            List<ClientCard> ordered = hand
                .OrderBy(c => GetGenericHandDispositionScore(c, hand))
                .ThenByDescending(c => hand.Count(x => x != null && x.IsCode(c.Id)))
                .ThenBy(c => c.Id)
                .ToList();


            int take = Math.Min(Math.Max(min, 1), Math.Min(max, ordered.Count));
            List<ClientCard> selected = ordered.Take(take).ToList();
            DebugRoute("GENERIC HAND DISPOSITION hint=" + hint
                + " selected="
                + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray()));
            return Util.CheckSelectCount(selected, cards, min, max);
        }

        private IList<ClientCard> SelectEldlichHandSpellTrapCost(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            List<ClientCard> costs = cards
                .Where(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Hand
                    && (c.IsSpell() || c.IsTrap()))
                .OrderBy(c => activatedThisTurn.Contains(c.Id) ? 0 : 1)
                .ThenByDescending(c => cards.Count(x => x != null && x.IsCode(c.Id)))
                .ThenBy(c => c.IsCode(CardId.CallOfTheHaunted) && !HasFaceupCall() ? 1 : 0)
                .ThenBy(c => c.IsCode(CardId.DeltaOfInvitation) && !HasFaceupFieldSpell() ? 1 : 0)
                .ThenBy(c => GetGenericHandDispositionScore(c, cards))
                .ThenBy(c => c.Id)
                .ToList();

            if (costs.Count < min)
                return null;

            int take = Math.Min(min, Math.Min(max, costs.Count));
            List<ClientCard> selected = costs.Take(take).ToList();
            DebugRoute("ELDLICH HAND COST selected="
                + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray()));
            return selected;
        }

        private IList<ClientCard> SelectStrictEnemyTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            List<ClientCard> result = new List<ClientCard>();
            result.AddRange(GetEnemyFieldPriority(cards, false).Where(cards.Contains));
            result.AddRange(GetEnemyGravePriority(cards).Where(cards.Contains));
            result.AddRange(cards.Where(c => c != null && c.Controller == 1));
            result = result.Distinct().ToList();

            if (result.Count < min)
                return null;


            int take = Math.Min(min, Math.Min(max, result.Count));
            return result.Take(take).ToList();
        }

        private ClientCard GetBestInfiniteImpermanenceTarget()
        {
            ClientCard last = Util.GetLastChainCard();
            if (last != null
                && last.Controller == 1
                && last.Location == CardLocation.MonsterZone
                && last.IsFaceup()
                && !last.IsDisabled()
                && !last.IsShouldNotBeTarget())
            {
                return last;
            }

            return Enemy.GetMonsters()
                .Where(c => c != null
                    && c.Controller == 1
                    && c.Location == CardLocation.MonsterZone
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && !c.IsShouldNotBeTarget()
                    && !currentNegateCardList.Contains(c))
                .OrderBy(c => IsKnownLiveNegateMonster(c) ? 0 : 1)
                .ThenBy(c => c.IsMonsterShouldBeDisabledBeforeItUseEffect() ? 0 : 1)
                .ThenByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.GetDefensePower())
                .FirstOrDefault();
        }

        private IList<ClientCard> SelectInfiniteImpermanenceTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            ClientCard exact = FindMatchingCandidate(
                cards, pendingInfiniteImpermanenceTarget);
            if (exact != null)
            {
                pendingInfiniteImpermanenceTarget = null;
                DebugRoute("Infinite Impermanence target=" + exact.Id);
                return Util.CheckSelectCount(
                    new List<ClientCard> { exact }, cards, min, max);
            }

            List<ClientCard> legal = cards
                .Where(c => c != null
                    && c.Controller == 1
                    && c.Location == CardLocation.MonsterZone
                    && c.IsFaceup()
                    && !c.IsDisabled())
                .OrderBy(c => IsKnownLiveNegateMonster(c) ? 0 : 1)
                .ThenBy(c => c.IsMonsterShouldBeDisabledBeforeItUseEffect() ? 0 : 1)
                .ThenByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.GetDefensePower())
                .ToList();

            pendingInfiniteImpermanenceTarget = null;
            if (legal.Count > 0)
            {
                DebugRoute("Infinite Impermanence fallback target=" + legal[0].Id);
                return Util.CheckSelectCount(legal, cards, min, max);
            }
            return null;
        }

        private ClientCard GetOpponentTurnCallReviveTarget(
            IEnumerable<ClientCard> revivable)
        {
            if (revivable == null
                || Duel.CurrentChain.Count == 0
                || Duel.LastChainPlayer != 1)
            {
                return null;
            }

            List<ClientCard> pool = revivable.Where(c => c != null).ToList();
            ClientCard chainSource = Util.GetLastChainCard();


            bool chainTargetsEnemyGraveMonster = Duel.ChainTargets.Any(c =>
                c != null
                && c.Controller == 1
                && c.Location == CardLocation.Grave
                && c.IsMonster());

            ClientCard snakehairTarget = null;
            if (IsLiveEnemyMonster(chainSource)
                && chainSource.IsAttack()
                && !chainSource.IsDisabled()
                && !chainSource.IsShouldNotBeTarget()
                && (ShouldPreemptWithSnakehair(chainSource)
                    || chainTargetsEnemyGraveMonster))
            {
                snakehairTarget = chainSource;
            }

            if (snakehairTarget == null)
            {
                snakehairTarget = Enemy.GetMonsters()
                    .Where(ShouldPreemptWithSnakehair)
                    .OrderBy(c => IsKnownLiveNegateMonster(c) ? 0 : 1)
                    .ThenByDescending(c => c.IsMonsterDangerous())
                    .ThenByDescending(c => c.Attack)
                    .FirstOrDefault();
            }

            ClientCard mammothTarget = GetMammothPreemptTarget();


            ClientCard pumpking = pool.FirstOrDefault(c => c.IsCode(
                CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking != null && !pumpkingSummonEffectAttempted)
            {
                if (snakehairTarget != null
                    && Bot.HasInDeck(CardId.StareOfTheSnakeHair))
                {
                    pendingSnakehairDisableTarget = snakehairTarget;
                    pendingMammothDestroyTarget = null;
                    DebugRoute("Call plan: Pumpking -> Snakehair target="
                        + snakehairTarget.Id);
                    return pumpking;
                }

                if (mammothTarget != null
                    && Bot.HasInDeck(CardId.GreatMammothOfTheNetherworld))
                {
                    pendingMammothDestroyTarget = mammothTarget;
                    pendingSnakehairDisableTarget = null;
                    DebugRoute("Call plan: Pumpking -> Mammoth target="
                        + mammothTarget.Id);
                    return pumpking;
                }
            }

            ClientCard snakehair = pool.FirstOrDefault(c => c.IsCode(
                CardId.StareOfTheSnakeHair));
            if (snakehair != null && snakehairTarget != null)
            {
                pendingSnakehairDisableTarget = snakehairTarget;
                pendingMammothDestroyTarget = null;
                DebugRoute("Call plan: revive Snakehair target="
                    + snakehairTarget.Id);
                return snakehair;
            }

            ClientCard mammoth = pool.FirstOrDefault(c => c.IsCode(
                CardId.GreatMammothOfTheNetherworld));
            if (mammoth != null && mammothTarget != null)
            {
                pendingMammothDestroyTarget = mammothTarget;
                pendingSnakehairDisableTarget = null;
                DebugRoute("Call plan: revive Mammoth target="
                    + mammothTarget.Id);
                return mammoth;
            }

            return null;
        }

        private bool IsAvailableOwnTurnCallTarget(ClientCard card)
        {
            if (card == null)
                return false;


            return !card.IsCode(CardId.ArmyOfTheHaunted)
                || armySpecialSummonEffectCommittedThisTurn;
        }

        private bool HasConfirmedOpenStateBattleLethal()
        {
            return Duel.CurrentChain.Count == 0
                && HasConfirmedLethalWithoutRank6Overlay();
        }

        private ClientCard GetOwnTurnCallLethalTarget(
            IEnumerable<ClientCard> revivable)
        {
            if (revivable == null
                || Duel.CurrentChain.Count > 0
                || !BattlePhaseIsAvailableThisTurn()
                || Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
            {
                return null;
            }

            int currentAttack = GetPotentialDirectBattleDamage();
            int missingDamage = Enemy.LifePoints - currentAttack;
            if (missingDamage <= 0)
                return null;

            return revivable
                .Where(c => c != null
                    && c.Attack > 0
                    && IsAvailableOwnTurnCallTarget(c))
                .OrderByDescending(c => c.Attack)
                .FirstOrDefault(c => c.Attack >= missingDamage);
        }

        private IList<ClientCard> SelectCallReviveTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            ClientCard target = plannedCallReviveId != 0
                ? cards.FirstOrDefault(c => c != null
                    && c.IsCode(plannedCallReviveId))
                : null;
            if (target == null)
            {
                target = Duel.Player == 1
                    ? GetOpponentTurnCallReviveTarget(cards)
                    : cards.FirstOrDefault(c => c.IsCode(
                        CardId.PumpkingTheKingOfGraveGhosts));
            }

            if (Duel.Player == 0
                && target != null
                && !IsAvailableOwnTurnCallTarget(target))
            {
                target = null;
            }

            callReviveSelectionPending = false;
            plannedCallReviveId = 0;
            if (target == null)
                return null;

            DebugRoute("Call revive target=" + target.Id);
            return Util.CheckSelectCount(
                new List<ClientCard> { target }, cards, min, max);
        }

        private IList<ClientCard> SelectSamuelGraveRecycleTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            List<ClientCard> own = cards.Where(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Grave
                    && c.IsMonster())
                .ToList();

            List<ClientCard> priority = new List<ClientCard>();
            priority.AddRange(own.Where(c => c.IsCode(CardId.ChangshiTheSpiridao)));
            priority.AddRange(own.Where(c => c.HasType(
                    CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
                .OrderBy(c => c.IsCode(CardId.OfficiatorOfDoomSamuel) ? 0 : 1)
                .ThenBy(c => c.IsCode(CardId.PumpkingTheGreatGhostKing) ? 0 : 1)
                .ThenBy(c => c.IsCode(CardId.TheUndyingLegion) ? 0 : 1));


            priority.AddRange(GetEnemyGravePriority(cards).Where(cards.Contains));
            priority.AddRange(own);
            priority = priority.Distinct().ToList();

            samuelGraveRecycleSelectionPending = false;
            if (priority.Count == 0)
                return null;

            DebugRoute("Samuel GY recycle target=" + priority[0].Id
                + " controller=" + priority[0].Controller);
            return Util.CheckSelectCount(priority, cards, min, max);
        }

        private int ScoreUndyingFieldRemovalTarget(ClientCard card)
        {
            if (card == null
                || card.Controller != 1
                || card.Location != CardLocation.MonsterZone
                || !card.IsFaceup()
                || !card.IsAttack()
                || card.IsShouldNotBeTarget())
            {
                return int.MinValue;
            }

            int score = card.GetDefensePower();
            if (IsKnownLiveNegateMonster(card))
                score += 5000;
            if (card.IsMonsterShouldBeDisabledBeforeItUseEffect())
                score += 3000;
            if (card.IsFloodgate())
                score += 2600;
            if (card.IsMonsterDangerous())
                score += 2200;
            if (card.IsMonsterInvincible())
                score += 1800;
            if (card.HasType(CardType.Xyz))
            {
                int materials = card.Overlays == null ? 0 : card.Overlays.Count;
                score += 700 + materials * 500;
            }
            return score;
        }

        private ClientCard GetUndyingOpenStateFieldTarget()
        {


            return Enemy.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && c.IsAttack()
                    && !c.IsShouldNotBeTarget()
                    && (IsKnownLiveNegateMonster(c)
                        || c.IsMonsterShouldBeDisabledBeforeItUseEffect()
                        || c.IsFloodgate()
                        || c.IsMonsterDangerous()
                        || c.IsMonsterInvincible()
                        || (c.HasType(CardType.Xyz)
                            && c.Overlays != null
                            && c.Overlays.Count > 0)))
                .OrderByDescending(ScoreUndyingFieldRemovalTarget)
                .FirstOrDefault();
        }

        private ClientCard GetUndyingReactiveTarget()
        {
            if (Duel.Player != 1
                || (Duel.Phase != DuelPhase.Main1
                    && Duel.Phase != DuelPhase.Main2))
            {
                return null;
            }

            if (IsFriendlyChainInProgress())
                return null;

            if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 1)
            {
                ClientCard last = Util.GetLastChainCard();
                if (last == null)
                    return null;


                ClientCard announcedGraveTarget = Duel.ChainTargets.LastOrDefault(c =>
                    c != null
                    && c.Controller == 1
                    && c.Location == CardLocation.Grave
                    && c.IsMonster());
                if (announcedGraveTarget != null)
                {
                    DebugRoute("Undying intercept opposing GY target="
                        + announcedGraveTarget.Id + " source=" + last.Id);
                    return announcedGraveTarget;
                }

                if (last.IsCode(CardId.CallOfTheHaunted))
                {
                    ClientCard reviveTarget = Enemy.Graveyard
                        .Where(c => c != null && c.IsMonster())
                        .OrderBy(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts) ? 0 : 1)
                        .ThenByDescending(c => c.IsMonsterDangerous())
                        .ThenByDescending(c => c.Attack)
                        .FirstOrDefault();
                    if (reviveTarget != null)
                        return reviveTarget;
                }


                if (last.Controller == 1
                    && last.Location == CardLocation.Grave
                    && last.IsMonster())
                {
                    DebugRoute("HOLD Undying: absorbing activated GY source does not stop effect id="
                        + last.Id);
                    return null;
                }

                if (last.Controller == 1
                    && last.Location == CardLocation.MonsterZone
                    && last.IsFaceup()
                    && last.IsAttack()
                    && !last.IsShouldNotBeTarget()
                    && (ShouldPreemptWithSnakehair(last)
                        || IsKnownLiveNegateMonster(last)))
                {
                    return last;
                }


                ClientCard fieldTarget = GetUndyingOpenStateFieldTarget();
                if (fieldTarget != null)
                {
                    DebugRoute("Undying chain-window field removal target="
                        + fieldTarget.Id);
                    return fieldTarget;
                }

                return null;
            }

            ClientCard openStateTarget = GetUndyingOpenStateFieldTarget();
            if (openStateTarget != null)
            {
                DebugRoute("Undying open-state field removal target="
                    + openStateTarget.Id);
            }
            return openStateTarget;
        }

        private IList<ClientCard> SelectUndyingReactiveTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            ClientCard exact = FindMatchingCandidate(cards, pendingUndyingTarget);
            pendingUndyingTarget = null;
            if (exact != null)
            {
                DebugRoute("Undying reactive target=" + exact.Id
                    + " location=" + exact.Location);
                return Util.CheckSelectCount(
                    new List<ClientCard> { exact }, cards, min, max);
            }

            List<ClientCard> fallback = new List<ClientCard>();
            fallback.AddRange(GetEnemyGravePriority(cards).Where(cards.Contains));
            fallback.AddRange(GetEnemyFieldPriority(cards, false).Where(cards.Contains));
            fallback = fallback.Distinct().ToList();
            return Util.CheckSelectCount(fallback, cards, min, max);
        }

        private bool IsHostileTargetHint(int hint)
        {
            return hint == HintMsg.Destroy
                || hint == HintMsg.ToGrave
                || hint == HintMsg.ReturnToHand
                || hint == HintMsg.Remove
                || hint == HintMsg.Disable;
        }

        private bool CanMammothTriggerAfterReverieRevive()
        {


            return HasFaceupCall() || Bot.GetMonsters().Any(c => c != null
                && c.IsFaceup() && IsZombie(c)
                && !c.IsCode(CardId.GreatMammothOfTheNetherworld));
        }

        private ClientCard GetReverieSnakehairTarget()
        {
            ClientCard chainSource = Util.GetLastChainCard();
            if (chainSource != null && chainSource.Controller == 1
                && IsLiveEnemyMonster(chainSource) && chainSource.IsAttack()
                && !chainSource.IsDisabled() && !chainSource.IsShouldNotBeTarget())
            {
                return chainSource;
            }

            return Enemy.GetMonsters()
                .Where(ShouldPreemptWithSnakehair)
                .OrderByDescending(c => c.IsMonsterShouldBeDisabledBeforeItUseEffect())
                .ThenByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.Attack)
                .FirstOrDefault();
        }

        private int GetReverieZombieXyzScore(ClientCard card)
        {
            if (card == null || !card.HasType(CardType.Xyz) || !IsZombie(card))
                return int.MinValue;


            if (card.IsCode(CardId.PumpkingTheGreatGhostKing))
            {
                bool searchStillLive = !greatPumpkingSearchAttempted
                    && !greatPumpkingSearchResolved;
                int searchValue = searchStillLive ? 180 : 0;
                int callProtection = HasFaceupCall() ? 40 : 0;
                return 300 + searchValue + callProtection;
            }

            if (card.IsCode(CardId.OfficiatorOfDoomSamuel))
            {
                bool usefulRecycleExists = Bot.Graveyard.Any(c => c != null
                    && (c.IsCode(CardId.ChangshiTheSpiridao)
                        || c.HasType(CardType.Fusion | CardType.Synchro
                            | CardType.Xyz | CardType.Link)));
                return 340 + (usefulRecycleExists ? 40 : 0);
            }

            return int.MinValue;
        }

        private IList<ClientCard> SelectReverieReviveTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;


            ClientCard snakehairTarget = GetReverieSnakehairTarget();
            if (snakehairTarget != null
                && cards.Any(c => c.IsCode(CardId.StareOfTheSnakeHair)))
            {
                pendingSnakehairDisableTarget = snakehairTarget;
                DebugRoute("Reverie revive target=Snakehair interaction target="
                    + snakehairTarget.Id);
                return SelectByIdPriority(cards, min, max,
                    CardId.StareOfTheSnakeHair);
            }

            ClientCard mammothTarget = GetMammothPreemptTarget()
                ?? GetEnemyFieldPriority().FirstOrDefault(c =>
                    c != null && !c.IsShouldNotBeTarget());
            if (mammothTarget != null && CanMammothTriggerAfterReverieRevive()
                && cards.Any(c => c.IsCode(CardId.GreatMammothOfTheNetherworld)))
            {
                pendingMammothDestroyTarget = mammothTarget;
                DebugRoute("Reverie revive target=Mammoth interaction target="
                    + mammothTarget.Id);
                return SelectByIdPriority(cards, min, max,
                    CardId.GreatMammothOfTheNetherworld);
            }


            ClientCard xyzTarget = cards
                .Where(c => GetReverieZombieXyzScore(c) > int.MinValue)
                .OrderByDescending(GetReverieZombieXyzScore)
                .ThenByDescending(c => c.Defense)
                .FirstOrDefault();
            if (xyzTarget != null)
            {
                DebugRoute("Reverie revive target=Xyz " + xyzTarget.Id
                    + " score=" + GetReverieZombieXyzScore(xyzTarget));
                return Util.CheckSelectCount(
                    new List<ClientCard> { xyzTarget }, cards, min, max);
            }

            DebugRoute("Reverie revive fallback: no interaction or Zombie Xyz candidate");
            return SelectZombieToRevive(cards, min, max);
        }

        private IList<ClientCard> SelectZombieToRevive(IList<ClientCard> cards, int min, int max)
        {


            IList<ClientCard> selectable = cards;
            if (HasFaceupFieldSpell())
            {
                selectable = cards
                    .Where(c => !c.IsCode(CardId.DoomkingBalerdroch))
                    .ToList();
                if (selectable.Count < min)
                    return null;
            }

            bool enemyHasFieldCard = Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0;
            bool enemyHasAttackMonster = Enemy.GetMonsters().Any(c => c.IsFaceup() && c.IsAttack());

            List<int> priority = new List<int>();
            if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                priority.Add(CardId.GreatMammothOfTheNetherworld);
            if (enemyHasAttackMonster)
                priority.Add(CardId.StareOfTheSnakeHair);

            priority.AddRange(new[]
            {
                CardId.PumpkingTheKingOfGraveGhosts,
                CardId.Hublot,
                CardId.DoomkingBalerdroch,
                CardId.EldlichTheGoldenLord,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.OfficiatingReverie,
                CardId.VampireGrace,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair
            });


            if (!enemyHasFieldCard)
                priority.Remove(CardId.GreatMammothOfTheNetherworld);

            return SelectByIdPriority(selectable, min, max, priority.ToArray());
        }

        private bool IsWorthwhileMezukiReviveTarget(ClientCard card)
        {
            if (card == null || !IsZombie(card) || !card.IsCanRevive()
                || card.IsCode(CardId.AshBlossom))
            {
                return false;
            }

            if (HasFaceupFieldSpell() && card.IsCode(CardId.DoomkingBalerdroch))
                return false;

            return true;
        }

        private IList<ClientCard> SelectMezukiReviveTarget(
            IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> selectable = cards
                .Where(IsWorthwhileMezukiReviveTarget)
                .Where(c => !mezukiLevel6ExtensionPending
                    || CanMezukiReviveCreateImmediateFollowUp(c))
                .ToList();
            if (selectable.Count < min)
            {
                DebugRoute("ERROR Mezuki target prompt has no planned Level 6 Zombie");
                mezukiLevel6ExtensionPending = false;
                return null;
            }

            List<int> priority = new List<int>();
            if (!HasSmallPumpkingOnField())
                priority.Add(CardId.PumpkingTheKingOfGraveGhosts);
            if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                priority.Add(CardId.GreatMammothOfTheNetherworld);
            if (Enemy.GetMonsters().Any(c => c.IsFaceup() && c.IsAttack()))
                priority.Add(CardId.StareOfTheSnakeHair);

            priority.AddRange(new[]
            {
                CardId.Hublot,
                CardId.ChangshiTheSpiridao,
                CardId.ArmyOfTheHaunted,
                CardId.OfficiatingReverie,
                CardId.EldlichTheGoldenLord,
                CardId.DoomkingBalerdroch,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.PumpkingTheKingOfGraveGhosts
            });

            IList<ClientCard> selected = SelectByIdPriority(
                selectable, min, max, priority.ToArray());
            if (selected != null)
            {
                DebugRoute("Mezuki Level 6 revive target="
                    + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray()));
            }
            mezukiLevel6ExtensionPending = false;
            return selected;
        }

        private ClientCard GetSamuelOpponentTurnReviveCandidate(
            IEnumerable<ClientCard> cards, ClientCard negateTarget)
        {
            if (cards == null || negateTarget == null)
                return null;

            int requiredAttack = Math.Max(0, negateTarget.Attack);
            List<ClientCard> legal = cards
                .Where(c => c != null
                    && IsZombie(c)
                    && c.IsCanRevive()
                    && Math.Max(0, c.Attack) >= requiredAttack)
                .ToList();
            if (legal.Count == 0)
                return null;


            int[] priority =
            {
                CardId.PumpkingTheKingOfGraveGhosts,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.Hublot,
                CardId.DoomkingBalerdroch,
                CardId.EldlichTheGoldenLord,
                CardId.EldlichTheMadGoldenLord,
                CardId.PumpkingTheGreatGhostKing,
                CardId.OfficiatingReverie
            };

            foreach (int id in priority)
            {
                ClientCard target = legal.FirstOrDefault(c => c.IsCode(id));
                if (target != null)
                    return target;
            }

            return legal
                .OrderByDescending(c => c.Attack)
                .FirstOrDefault();
        }

        private int GetMaterialValue(ClientCard card)
        {
            if (card == null) return 999;
            if (card.IsCode(CardId.GreatMammothOfTheNetherworld)) return 0;
            if (card.IsCode(CardId.StareOfTheSnakeHair)) return 1;
            if (card.IsCode(CardId.ArmyOfTheHaunted)) return 2;
            if (card.IsCode(CardId.VampireGrace)) return 3;
            if (card.IsCode(CardId.ChangshiTheSpiridao)) return 3;
            if (card.IsCode(CardId.OfficiatingReverie)) return 4;
            if (IsHublot(card)) return 5;
            if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts)) return 6;
            if (card.IsCode(CardId.DoomkingBalerdroch)) return 20;
            if (card.IsCode(CardId.EldlichTheGoldenLord)) return 21;
            return 10;
        }

        private List<ClientCard> GetRank6Materials(bool zombiesOnly)
        {
            return Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.Level == 6 && !c.HasType(CardType.Xyz | CardType.Link)
                    && (!zombiesOnly || IsZombie(c)))
                .OrderBy(GetMaterialValue)
                .ThenBy(c => c.Attack)
                .ToList();
        }

        private bool SelectRank6Materials(bool zombiesOnly)
        {
            List<ClientCard> materials = GetRank6Materials(zombiesOnly);
            if (materials.Count < 2)
                return false;
            AI.SelectMaterials(materials.Take(2).ToList());
            return true;
        }

        private ClientCard GetCallLinkedLevel6(
            IEnumerable<ClientCard> candidates)
        {
            if (candidates == null)
                return null;

            List<ClientCard> legal = candidates
                .Where(c => c != null && c.IsFaceup() && c.Level == 6
                    && !c.HasType(CardType.Xyz | CardType.Link))
                .ToList();
            foreach (ClientCard call in Bot.GetSpells().Where(c => c != null
                && c.IsFaceup() && c.IsCode(CardId.CallOfTheHaunted)))
            {
                ClientCard linked = GetCallLinkedMonster(call);
                ClientCard match = legal.FirstOrDefault(c => MatchesCard(c, linked));
                if (match != null)
                    return match;
            }
            return null;
        }

        private int GetChangshiRescueMaterialRoleScore(ClientCard card)
        {
            if (card == null)
                return 999;
            if (GetCallLinkedLevel6(new[] { card }) != null)
                return 0;


            if (card.IsCode(CardId.VampireGrace))
                return 5;
            if (card.IsDisabled())
                return 10;
            if (IsHublot(card))
                return 15;
            if (card.IsCode(CardId.ChangshiTheSpiridao)
                && !changshiMillResolved)
            {
                return 100;
            }
            return 20;
        }

        private List<ClientCard> GetChangshiRescueRank6Materials(
            IEnumerable<ClientCard> candidates)
        {
            List<ClientCard> legal = (candidates ?? Enumerable.Empty<ClientCard>())
                .Where(c => c != null && c.IsFaceup() && c.Level == 6
                    && !c.HasType(CardType.Xyz | CardType.Link)
                    && IsZombie(c))
                .ToList();
            if (legal.Count < 2)
                return new List<ClientCard>();

            List<ClientCard> selected = new List<ClientCard>();
            ClientCard linked = GetCallLinkedLevel6(legal);
            if (linked != null)
                selected.Add(linked);

            selected.AddRange(legal
                .Where(c => !selected.Contains(c))
                .OrderBy(GetChangshiRescueMaterialRoleScore)
                .ThenBy(c => c.Attack)
                .ThenBy(c => c.Sequence)
                .Take(2 - selected.Count));

            return selected.Count == 2
                ? selected
                : new List<ClientCard>();
        }

        private List<ClientCard> GetSamuelMaterials()
        {
            if (IsVampireGraceRouteLive())
            {


                if (!HasGreatPumpkingOnField())
                    return new List<ClientCard>();
                return GetChangshiRescueRank6Materials(Bot.GetMonsters());
            }

            if (changshiHandRescueRouteActive
                && HasGreatPumpkingOnField()
                && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao))
            {


                return GetChangshiRescueRank6Materials(Bot.GetMonsters());
            }

            if (ashReplayLineActive)
            {


                List<ClientCard> ashMaterials = Bot.GetMonsters()
                    .Where(c => c.IsFaceup() && c.Level == 6
                        && !c.HasType(CardType.Xyz | CardType.Link))
                    .OrderBy(c => c.IsCode(CardId.ArmyOfTheHaunted) ? 0
                        : c.IsCode(CardId.ChangshiTheSpiridao) ? 1
                        : c.IsCode(CardId.OfficiatingReverie) ? 2
                        : c.IsCode(CardId.GreatMammothOfTheNetherworld) ? 3
                        : IsHublot(c) ? 9 : 5)
                    .ThenBy(GetMaterialValue)
                    .ToList();

                ClientCard army = ashMaterials.FirstOrDefault(c =>
                    c.IsCode(CardId.ArmyOfTheHaunted));
                ClientCard changshi = ashMaterials.FirstOrDefault(c =>
                    c.IsCode(CardId.ChangshiTheSpiridao));
                if (army != null && changshi != null)
                    return new List<ClientCard> { army, changshi };

                if (changshi != null)
                {
                    ClientCard second = ashMaterials.FirstOrDefault(c => c != changshi);
                    if (second != null)
                        return new List<ClientCard> { changshi, second };
                }

                return ashMaterials.Take(2).ToList();
            }

            ClientCard pumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.Level == 6 && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking == null)
                return new List<ClientCard>();

            ClientCard secondNormal = Bot.GetMonsters()
                .Where(c => c != pumpking && c.IsFaceup() && c.Level == 6
                    && !c.HasType(CardType.Xyz | CardType.Link))
                .OrderBy(c => c.IsCode(CardId.ChangshiTheSpiridao) ? 0 : 1)
                .ThenBy(c => IsHublot(c) ? 10 : GetMaterialValue(c))
                .FirstOrDefault();
            if (secondNormal == null)
                return new List<ClientCard>();

            return new List<ClientCard> { pumpking, secondNormal };
        }

        private bool CanNormalSamuelFirstReachGreatPumpking(
            IList<ClientCard> materials)
        {
            if (ashReplayLineActive || materials == null || materials.Count != 2)
                return ashReplayLineActive;

            int remainingLevel6 = Bot.GetMonsters().Count(c =>
                IsLevel6Zombie(c) && !materials.Contains(c));
            if (remainingLevel6 <= 0)
                return false;

            return Bot.Graveyard.Any(c => c != null
                && IsLevel6Zombie(c)
                && c.IsCanRevive()
                && !c.IsCode(CardId.AshBlossom));
        }

        private List<ClientCard> GetGreatPumpkingMaterials()
        {
            if (IsVampireGraceRouteLive()
                && !HasGreatPumpkingOnField()
                && GetRank6Materials(true).Count >= 4)
            {


                return GetChangshiRescueRank6Materials(Bot.GetMonsters());
            }

            if (changshiHandRescueRouteActive
                && !HasGreatPumpkingOnField()
                && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao))
            {


                return GetChangshiRescueRank6Materials(Bot.GetMonsters());
            }

            if (ashReplayLineActive)
            {
                ClientCard ashLineHublot = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                    && c.Level == 6 && IsHublot(c));
                ClientCard pumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                    && c.Level == 6 && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                bool hasChangshi = Bot.HasInMonstersZone(CardId.ChangshiTheSpiridao, faceUp: true);
                if (ashLineHublot != null && pumpking != null && hasChangshi)
                    return new List<ClientCard> { ashLineHublot, pumpking };
                return new List<ClientCard>();
            }

            if (!Bot.HasInMonstersZone(CardId.OfficiatorOfDoomSamuel, faceUp: true)
                || !HasPumpkingInGrave())
            {
                return new List<ClientCard>();
            }

            ClientCard hublot = Bot.GetMonsters().FirstOrDefault(c =>
                c.IsFaceup() && c.Level == 6 && IsHublot(c));
            ClientCard revived = Bot.GetMonsters().FirstOrDefault(c =>
                c.IsFaceup() && c.Level == 6 && c.IsCode(samuelRevivedCardId));
            if (hublot != null && revived != null && hublot != revived)
                return new List<ClientCard> { hublot, revived };


            List<ClientCard> fallback = GetRank6Materials(true)
                .Where(c => !c.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
                .OrderBy(c => IsHublot(c) ? 0 : 1)
                .ThenBy(c => c.IsCode(samuelRevivedCardId) ? 0 : 1)
                .ThenBy(GetMaterialValue)
                .Take(2)
                .ToList();
            return fallback.Count == 2 ? fallback : new List<ClientCard>();
        }

        private List<ClientCard> GetRank10Materials()
        {
            List<ClientCard> materials = Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.Level == 10
                    && !c.HasType(CardType.Xyz | CardType.Link))
                .OrderBy(c => c.IsCode(CardId.EldlichTheMadGoldenLord) ? 0
                    : c.IsCode(CardId.EldlichTheGoldenLord) ? 1 : 2)
                .ThenBy(c => c.Attack)
                .Take(2)
                .ToList();
            return materials.Count == 2 ? materials : new List<ClientCard>();
        }

        private bool IsDeltaToken(ClientCard card)
        {
            return card != null && card.HasType(CardType.Token) && IsZombie(card);
        }

        private bool IsReverieRecoverableXyz(ClientCard card)
        {


            return IsSpentZombieXyz(card);
        }

        private bool IsAllowedZombieLinkMaterial(ClientCard card)
        {
            if (card == null || !card.IsFaceup() || !IsZombie(card)
                || card.HasType(CardType.Link))
            {
                return false;
            }


            if (card.HasType(CardType.Xyz))
                return IsSpentZombieXyz(card);

            return true;
        }

        private int GetZombieLinkMaterialValue(ClientCard card)
        {
            if (IsDeltaToken(card)) return 0;


            if (IsSpentZombieXyz(card)) return 1;
            if (card != null && card.Level != 6 && !card.HasType(CardType.Xyz)) return 2;
            if (card != null && card.IsCode(CardId.GreatMammothOfTheNetherworld)) return 10;
            if (card != null && card.IsCode(CardId.OfficiatingReverie)) return 11;
            if (card != null && card.IsCode(CardId.ChangshiTheSpiridao)) return 12;
            if (card != null && card.IsCode(CardId.ArmyOfTheHaunted)) return 13;
            if (card != null && IsHublot(card)) return 14;
            if (card != null && card.IsCode(CardId.PumpkingTheKingOfGraveGhosts)) return 15;
            return 20;
        }

        private List<ClientCard> GetPreferredZombieLinkMaterials(ClientCard required, int total)
        {
            List<ClientCard> result = new List<ClientCard>();
            if (required != null)
                result.Add(required);

            result.AddRange(Bot.GetMonsters()
                .Where(c => c != required && IsAllowedZombieLinkMaterial(c))
                .OrderBy(GetZombieLinkMaterialValue)
                .ThenBy(GetMaterialValue)
                .ThenBy(c => c.Attack)
                .Take(Math.Max(0, total - result.Count)));

            return result.Count == total ? result : new List<ClientCard>();
        }

        private List<ClientCard> GetFlyingMaryEldlichMaterials()
        {
            ClientCard fallen = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.FallenAngelOfTheGoldenLand));
            if (fallen == null)
                return new List<ClientCard>();


            return GetPreferredZombieLinkMaterials(fallen, 2);
        }

        private List<ClientCard> GetLinkZombieMaterials()
        {
            return Bot.GetMonsters()
                .Where(IsAllowedZombieLinkMaterial)
                .OrderBy(GetZombieLinkMaterialValue)
                .ThenBy(GetMaterialValue)
                .ThenBy(c => c.Attack)
                .ToList();
        }

        private void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = true)
        {
            card = card ?? Card;
            if (card != null && card.Location == CardLocation.SpellZone)
                return;

            List<int> zones = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (Bot.SpellZone[i] != null)
                    continue;
                if (avoidImpermanence && infiniteImpermanenceNegatedColumns.Contains(i))
                    continue;
                zones.Add(i);
            }


            foreach (int zoneId in zones)
            {
                ClientCard opposite = Enemy.SpellZone[4 - zoneId];
                if (opposite != null && opposite.IsFacedown())
                    continue;
                AI.SelectPlace(1 << zoneId);
                return;
            }

            if (zones.Count > 0)
            {
                AI.SelectPlace(1 << zones[Program.Rand.Next(zones.Count)]);
                return;
            }

            AI.SelectPlace(0);
        }


        private bool GhostBelleActivate()
        {
            if (!CanUseEarthMonsterEffects() || WillCardEffectBeNegated())
                return false;
            return DefaultGhostBelleAndHauntedMansion();
        }

        private bool MaxxCActivate()
        {
            if (!CanUseEarthMonsterEffects())
                return false;
            return DefaultMaxxC();
        }

        private bool InfiniteImpermanenceActivate()
        {
            if (!DefaultInfiniteImpermanence())
                return false;

            ClientCard target = GetBestInfiniteImpermanenceTarget();
            if (target == null)
                return false;

            pendingInfiniteImpermanenceTarget = target;
            currentNegateCardList.Add(target);
            DebugRoute("ACCEPT Infinite Impermanence target=" + target.Id);
            SelectSTPlace(Card, true);
            return true;
        }

        private bool DominusImpulseActivate()
        {
            if (WillCardEffectBeNegated())
                return false;


            if (Card.Location == CardLocation.Hand)
            {
                DebugRoute("HOLD Dominus in hand: preserve LIGHT/EARTH/WIND engine; Set it first");
                return false;
            }

            ClientCard last = Util.GetLastChainCard();
            if (!IsOpponentChainWorthNegating(last))
                return false;

            currentNegateCardList.Add(last);
            SelectSTPlace(Card, true);
            DebugRoute("ACCEPT set Dominus negate id=" + last.Id);
            return true;
        }

        private bool VortexOfTimeActivate()
        {
            if (WillCardEffectBeNegated() || Duel.LastChainPlayer != 1)
                return false;
            if (!HasFaceupCall() || !Bot.GetMonsters().Any(IsZombie))
                return false;

            ClientCard last = Util.GetLastChainCard();
            if (last == null || last.IsDisabled())
                return false;

            int ourZombieCount = Bot.GetMonsters().Count(IsZombie);
            bool emergency = last.IsMonsterShouldBeDisabledBeforeItUseEffect()
                || GetEnemyFieldPriority().Count > ourZombieCount
                || Enemy.GetMonsters().Sum(c => Math.Max(0, c.Attack)) >= Bot.LifePoints;


            return ourZombieCount <= 1 || emergency;
        }

        private bool DoomkingBalerdrochActivate()
        {
            if (WillCardEffectBeNegated())
                return false;


            if (Card.Location == CardLocation.Grave)
                return HasFaceupFieldSpell() && HasOpenMainMonsterZone();

            ClientCard last = Util.GetLastChainCard();
            if (last == null || last.IsCode(CardId.DoomkingBalerdroch))
                return false;


            if (Duel.LastChainPlayer == 1 && Duel.CurrentChain.Count > 0 && !last.IsDisabled())
            {
                doomkingOptionPending = true;
                doomkingPreferNegate = true;
                currentNegateCardList.Add(last);
                DebugRoute("ACCEPT Doomking: prefer NEGATE opponent chain id=" + last.Id
                    + " location=" + last.Location);
                return true;
            }

            if (IsFriendlyChainInProgress())
                DebugRoute("HOLD Doomking: do not chain to our own Zombie effect");

            return false;
        }

        private bool VarudrasActivate()
        {
            if (WillCardEffectBeNegated())
                return false;
            if (IsFriendlyChainInProgress())
            {
                DebugRoute("HOLD Varudras: do not chain to our own effect");
                return false;
            }

            int negateOrBattleDesc = Util.GetStringId(CardId.Varudras, 1);
            int destroyedDesc = Util.GetStringId(CardId.Varudras, 2);
            bool hasEnemyFieldCard = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                .Any(c => c != null && c.IsOnField());

            DebugRoute("Varudras desc=" + ActivateDescription
                + " chainPlayer=" + Duel.LastChainPlayer
                + " chainCount=" + Duel.CurrentChain.Count
                + " enemyField=" + hasEnemyFieldCard);


            bool opponentChain = Duel.LastChainPlayer == 1
                && Duel.CurrentChain.Count > 0;
            if (opponentChain
                && (ActivateDescription == negateOrBattleDesc
                    || ActivateDescription == -1))
            {
                ClientCard last = Util.GetLastChainCard();
                if (!CanVarudrasNegateCurrentChain())
                {
                    DebugRoute("HOLD Varudras: Samuel or another narrower answer covers chain id="
                        + (last != null ? last.Id : 0));
                    return false;
                }

                currentNegateCardList.Add(last);
                pendingInterruptMode = InterruptMode.VarudrasHardNegate;
                bool sourceWasAlreadyFaceup =
                    WasAlreadyFaceupBeforeCurrentChain(last);
                ClearVarudrasDestroyPlan();
                varudrasNegatedChainSource = last;
                varudrasNegatedSourceWasAlreadyFaceup =
                    sourceWasAlreadyFaceup;
                SetStrategicPlan(StrategicGoal.NegateImmediateThreat,
                    currentComboRoute, "Varudras hard negate");
                ClearEnemyCommitment("opponent chain started");
                DebugRoute("ACCEPT Varudras hard negate id="
                    + (last != null ? last.Id : 0)
                    + " sourceAlreadyFaceup="
                    + varudrasNegatedSourceWasAlreadyFaceup);
                return true;
            }


            if (!opponentChain
                && (ActivateDescription == negateOrBattleDesc
                    || ActivateDescription == destroyedDesc
                    || ActivateDescription == -1))
            {
                ClearVarudrasDestroyPlan();
                ClientCard enemyTarget = GetEnemyFieldPriority(
                        Enemy.GetMonsters().Concat(Enemy.GetSpells()).ToList(),
                        false)
                    .FirstOrDefault(c => c != null
                        && c.Controller == 1
                        && c.IsOnField());
                if (enemyTarget == null)
                {
                    DebugRoute("DECLINE Varudras destroy: no enemy field card");
                    return false;
                }

                pendingVarudrasDestroyMode =
                    VarudrasDestroyMode.TriggeredEnemyRemoval;
                pendingVarudrasDestroyTarget = enemyTarget;
                DebugRoute("ACCEPT Varudras destroy: enemy target="
                    + enemyTarget.Id);
                return true;
            }

            return false;
        }

        private bool EvolzarLarsActivate()
        {
            if (WillCardEffectBeNegated() || Duel.LastChainPlayer != 1)
                return false;
            return Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeTarget())
                || Enemy.GetSpells().Any(c => c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeTarget());
        }


        private bool TerraformingActivate()
        {
            if (WillCardEffectBeNegated() || !DefaultCheckWhetherBotCanSearch())
                return false;
            if (Bot.HasInHand(CardId.DeltaOfInvitation)
                || Bot.HasInSpellZone(CardId.DeltaOfInvitation, faceUp: true))
            {
                return false;
            }
            SelectSTPlace(Card, true);
            return true;
        }

        private bool DeltaOfInvitationActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                bool createToken = ShouldCreateDeltaTokenNow();
                pendingDeltaTokenFieldInstance = createToken ? Card : null;
                DebugRoute(createToken
                    ? "ACCEPT Delta Token: planned Flying Mary/Sucker Link material"
                    : "HOLD Delta Token: preserve Main Monster Zone");
                return createToken;
            }


            if (CanTakeProductivePumpkingActionNow())
            {
                DebugRoute("HOLD Delta activation: complete available Hublot/Pumpking action first");
                return false;
            }


            if (!CanStartEldlichRoute())
            {
                DebugRoute("HOLD Delta activation: no complete Eldlich/Flying Mary route available");
                return false;
            }


            SelectSTPlace(Card, true);
            return true;
        }

        private bool EctoplasmicFortificationActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Bot.GetMonsterCount() == 0 && DefaultCheckWhetherBotCanSearch())
            {
                int searchTarget = GetEctoplasmicSearchTargetId();
                if (searchTarget != 0)
                {
                    DebugRoute("ACCEPT Ectoplasmic search target=" + searchTarget
                        + "; hublotInHand=" + HasHublotInHand());
                    SelectSTPlace(Card, true);
                    return true;
                }
            }

            if (Bot.GetMonsters().Any(IsZombie) && HasFaceupCall())
            {
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }

        private bool StareOfTheSnakeHairHandActivate()
        {
            if (Card.Location != CardLocation.Hand || WillCardEffectBeNegated())
                return false;


            if (ectoplasmicSearchUsed
                || Bot.HasInHand(CardId.EctoplasmicFortification)
                || !Bot.HasInDeck(CardId.EctoplasmicFortification))
            {
                return false;
            }

            bool ectoplasmicHasStarterTarget =
                (!HasHublotInHand() && Bot.HasInDeck(CardId.Hublot))
                || (HasHublotInHand() && !HasPumpkingInHand()
                    && Bot.HasInDeck(CardId.PumpkingTheKingOfGraveGhosts));
            if (!ectoplasmicHasStarterTarget)
                return false;

            DebugRoute("ACCEPT Snakehair hand effect: search Ectoplasmic first");
            return DefaultCheckWhetherBotCanSearch();
        }

        private bool StareOfTheSnakeHairFieldActivate()
        {
            if (Card.Location != CardLocation.MonsterZone || WillCardEffectBeNegated())
                return false;

            if (pendingSnakehairDisableTarget != null)
            {
                if (IsLiveEnemyMonster(pendingSnakehairDisableTarget)
                    && pendingSnakehairDisableTarget.IsAttack()
                    && !pendingSnakehairDisableTarget.IsShouldNotBeTarget())
                {
                    DebugRoute("ACCEPT Snakehair pre-emptive trigger target="
                        + pendingSnakehairDisableTarget.Id);
                    return true;
                }
                pendingSnakehairDisableTarget = null;
            }

            return Enemy.GetMonsters().Any(c => c.IsFaceup() && c.IsAttack()
                && !c.IsDisabled() && !c.IsShouldNotBeTarget());
        }
        private bool PumpkingHandActivate()
        {
            if (Card.Location != CardLocation.Hand
                || WillCardEffectBeNegated()
                || HasConfirmedOpenStateBattleLethal())
            {
                return false;
            }
            if (!HasOpenSpellZone()
                || !HasSettableCallForPumpking()
                || !HasPumpkingDeckSummonTargetInDeck()
                || pumpkingHandEffectAttempted)
            {
                return false;
            }


            if (Duel.Player == 0 && GetOpenMainMonsterZoneCount() < 2)
            {
                DebugRoute("HOLD Pumpking hand effect: need two open Main Monster Zones for Call + Deck summon");
                return false;
            }


            if (summonCount > 0
                && !HasHublotOnField()
                && HasHublotInHand()
                && HasOpenMainMonsterZone())
            {
                return false;
            }

            pumpkingHandEffectAttempted = true;
            pumpkingHandSelectionPending = true;
            pumpkingCallPromptCompleted = false;
            pumpkingDiscardSelfRequired = !HasPumpkingInGrave();
            pendingPumpkingHandCard = Card;
            DebugRoute("ACCEPT Pumpking hand effect: Set Call; discard self="
                + pumpkingDiscardSelfRequired + "; hublotInHand=" + HasHublotInHand());
            DebugCards("HAND WHEN PUMPKING ACCEPTED", Bot.Hand);
            return true;
        }
        private bool PumpkingSummonActivate()
        {
            if (Card.Location != CardLocation.MonsterZone || WillCardEffectBeNegated())
                return false;
            if (!HasOpenMainMonsterZone() || pumpkingSummonEffectAttempted)
                return false;

            if (Duel.Player == 1)
            {
                bool hasSnakehairLine = Bot.HasInDeck(CardId.StareOfTheSnakeHair)
                    && ((pendingSnakehairDisableTarget != null
                            && IsLiveEnemyMonster(pendingSnakehairDisableTarget)
                            && pendingSnakehairDisableTarget.IsAttack())
                        || Enemy.GetMonsters().Any(ShouldPreemptWithSnakehair));
                bool hasMammothLine = Bot.HasInDeck(CardId.GreatMammothOfTheNetherworld)
                    && ((pendingMammothDestroyTarget != null
                            && CanMammothDestroyTarget(pendingMammothDestroyTarget))
                        || GetMammothPreemptTarget() != null);


                if (!hasSnakehairLine && !hasMammothLine)
                {
                    DebugRoute("HOLD opponent-turn Pumpking: no live Snakehair/Mammoth interaction");
                    return false;
                }

                pumpkingSummonEffectAttempted = true;
                DebugRoute("ACCEPT opponent-turn Pumpking: live interaction available");
                return true;
            }

            pumpkingSummonEffectAttempted = true;
            DebugRoute("ACCEPT revived Pumpking trigger: summon Changshi");
            return true;
        }
        private bool ShouldDelayFoolishForGreatPumpkingSearch()
        {


            if (greatPumpkingSearchWindowPending
                || (greatPumpkingSearchAttempted && !greatPumpkingSearchResolved)
                || (HasGreatPumpkingOnField() && !greatPumpkingSearchResolved))
            {
                return true;
            }


            if (CanTakeProductivePumpkingExtraDeckStep())
                return true;


            return ashReplayLineActive
                && !greatPumpkingSearchResolved
                && Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                && GetGreatPumpkingMaterials().Count == 2;
        }

        private bool IsFoolishCandidateAvailable(
            int cardId,
            IEnumerable<ClientCard> candidates = null)
        {
            return candidates == null
                ? Bot.HasInDeck(cardId)
                : candidates.Any(c => c != null && c.IsCode(cardId));
        }

        private bool CanUseFoolishForMissingMezukiBody()
        {
            if (!CanUseEarthMonsterEffects() || !HasOpenMainMonsterZone())
                return false;

            bool hasImmediateLevel6Revive = Bot.Graveyard.Any(c =>
                IsWorthwhileMezukiReviveTarget(c) && IsLevel6Zombie(c));
            if (!hasImmediateLevel6Revive)
                return false;

            return currentStrategicGoal == StrategicGoal.ProduceLevel6Bodies
                || NeedsAdditionalLevel6BodyForCurrentPlan()
                || (IsPumpkingComboInProgress()
                    && !CanTakeProductivePumpkingActionNow());
        }

        private bool CanUseFoolishForMissingEldlich()
        {
            if (!CanUseLightMonsterEffects()
                || !HasOpenMainMonsterZone()
                || !HasUsefulEldlichCost()
                || Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
            {
                return false;
            }

            return CanStartEldlichRoute() || HasSeriousEnemyProblem();
        }


        private bool CanUseFoolishForDoomkingValue()
        {


            return HasFaceupFieldSpell()
                && !Bot.HasInGraveyard(CardId.DoomkingBalerdroch)
                && !Bot.HasInMonstersZone(CardId.DoomkingBalerdroch, faceUp: true);
        }

        private bool CanUseFoolishForVampireGrace()
        {
            if (!CanPayVampireGraceCost()
                || !HasVampireGraceRank6Payoff()
                || GetOpenMainMonsterZoneCount() < 2
                || Bot.HasInGraveyard(CardId.VampireGrace)
                || Bot.HasInMonstersZone(CardId.VampireGrace, faceUp: true))
            {
                return false;
            }


            bool armyTrigger = HasFaceupCall()
                && !armySpecialSummonEffectCommittedThisTurn
                && (Bot.HasInHand(CardId.ArmyOfTheHaunted)
                    || Bot.Graveyard.Any(c => c != null
                        && c.IsCode(CardId.ArmyOfTheHaunted)
                        && c.IsCanRevive()));
            bool reverieTrigger = Bot.HasInHand(CardId.OfficiatingReverie)
                && Bot.Hand.Count > 1;
            bool pumpkingTrigger = HasSmallPumpkingOnField()
                && !pumpkingSummonEffectAttempted
                && HasPumpkingDeckSummonTargetInDeck();
            return armyTrigger || reverieTrigger || pumpkingTrigger;
        }

        private int GetFoolishBurialPlannedTargetId(
            IEnumerable<ClientCard> candidates = null)
        {

            if (IsFoolishCandidateAvailable(CardId.Mezuki, candidates)
                && CanUseFoolishForMissingMezukiBody())
            {
                return CardId.Mezuki;
            }

            if (IsFoolishCandidateAvailable(CardId.EldlichTheGoldenLord, candidates)
                && CanUseFoolishForMissingEldlich())
            {
                return CardId.EldlichTheGoldenLord;
            }


            if (IsFoolishCandidateAvailable(CardId.DoomkingBalerdroch, candidates)
                && CanUseFoolishForDoomkingValue())
            {
                return CardId.DoomkingBalerdroch;
            }


            if (IsFoolishCandidateAvailable(CardId.VampireGrace, candidates)
                && CanUseFoolishForVampireGrace())
            {
                return CardId.VampireGrace;
            }

            return 0;
        }

        private string GetFoolishBurialPlanReason(int targetId)
        {
            if (targetId == CardId.Mezuki)
                return "supply missing Level 6 extender";
            if (targetId == CardId.EldlichTheGoldenLord)
                return "supply missing Eldlich route piece";
            if (targetId == CardId.DoomkingBalerdroch)
                return "prepare Doomking self-revive under Field Spell";
            if (targetId == CardId.VampireGrace)
                return "low-priority Grace extender with guaranteed trigger";
            return "no profitable target";
        }

        private bool FoolishBurialActivate()
        {
            if (WillCardEffectBeNegated() || HasImmediatePumpkingActionPending()
                || eldlichRouteRank10CommitPending)
                return false;

            if (HasConfirmedOpenStateBattleLethal())
            {
                DebugRoute("HOLD Foolish Burial: current open-state board is already lethal");
                return false;
            }

            if (ShouldDelayFoolishForGreatPumpkingSearch())
            {
                DebugRoute("HOLD Foolish Burial: finish Great Pumpking/Army core line first");
                return false;
            }


            if (summonCount > 0 && HasHublotInHand()
                && HasOpenMainMonsterZone() && !ShouldDelayHublotForPumpkingSearch())
            {
                return false;
            }

            selectedFoolishBurialSendId = GetFoolishBurialPlannedTargetId();
            if (selectedFoolishBurialSendId == 0)
            {
                DebugRoute("HOLD Foolish Burial: no missing piece or profitable GY target");
                return false;
            }

            SelectSTPlace(Card, true);
            DebugRoute("ACCEPT Foolish Burial target=" + selectedFoolishBurialSendId
                + " reason=" + GetFoolishBurialPlanReason(selectedFoolishBurialSendId));
            return true;
        }
        private bool HublotSummon()
        {
            if (DefaultCheckWhetherCardWillBeNegatedOnField(Card) || !HasOpenMainMonsterZone() || summonCount <= 0)
                return false;
            if (ShouldDelayHublotForPumpkingSearch())
                return false;

            RecalculateStrategicPlan("evaluate Hublot Normal Summon");
            if (currentComboRoute != ComboRoute.NormalPumpking)
                return false;

            DebugRoute("ACCEPT Normal Summon Hublot for goal="
                + currentStrategicGoal);
            return true;
        }
        private bool BrickZombieNormalSummon()
        {
            if (Card == null || !IsZombie(Card) || summonCount <= 0
                || !HasOpenMainMonsterZone())
            {
                return false;
            }

            RecalculateStrategicPlan("brick Zombie Normal Summon");
            if (currentStrategicGoal != StrategicGoal.EnableEldlichWithNormalZombie
                || currentComboRoute != ComboRoute.BrickEldlich)
            {
                return false;
            }


            if (!Card.IsCode(CardId.Mezuki, CardId.AshBlossom))
                return false;

            DebugRoute("ACCEPT brick Zombie Normal Summon id=" + Card.Id
                + " to seed Eldlich/Flying Mary");
            return true;
        }

        private bool HublotActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            int xyzDescription = Util.GetStringId(Card.Id, 2);
            if (ActivateDescription != xyzDescription)
            {
                DebugRoute("ACCEPT Hublot mill/recover effect");
                return true;
            }


            if (Duel.Player == 1)
            {
                DebugRoute("ACCEPT Hublot opponent-turn Xyz trigger");
                return true;
            }


            SurplusRank6Plan recoveryRank6Plan = GetSurplusRank6Plan();
            if (IsLaterTurnRecoveryState()
                && (recoveryRank6Plan == SurplusRank6Plan.LarsNegate
                    || recoveryRank6Plan == SurplusRank6Plan.SheridanRemoval))
            {
                DebugRoute("HOLD Hublot Xyz trigger: preserve Level 6 pair for generic recovery Rank 6="
                    + recoveryRank6Plan);
                return false;
            }


            if (samuelReviveResolved && HasSamuelOnField()
                && CountFreeLevel6ForGreatPumpking() >= 2)
            {
                DebugRoute("ACCEPT Hublot Xyz trigger after Samuel revive");
                return true;
            }

            if (HasSmallPumpkingOnField() || IsPumpkingComboInProgress())
                return false;

            bool useFallback = Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0
                && CountFreeLevel6ForGreatPumpking() >= 2;
            if (useFallback)
                DebugRoute("ACCEPT Hublot fallback Xyz trigger");
            return useFallback;
        }
        private int GetReverieOverlayReloadScore(ClientCard card)
        {
            if (card == null || card.Controller != 0 || !card.IsFaceup()
                || !card.IsOnField() || !IsZombie(card)
                || !card.HasType(CardType.Xyz))
            {
                return int.MinValue;
            }

            int materials = card.Overlays == null ? 0 : card.Overlays.Count;


            int score = materials == 0 ? 10000 : -materials * 1000;


            if (card.IsCode(CardId.PumpkingTheGreatGhostKing))
                score += Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0 ? 520 : 360;
            else if (card.IsCode(CardId.OfficiatorOfDoomSamuel))
                score += HasOpenMainMonsterZone()
                    && Bot.Graveyard.Any(c => c != null && IsZombie(c) && c.IsCanRevive())
                        ? 500 : 340;
            else if (card.IsCode(CardId.DhampirVampireSheridan))
                score += Enemy.GetMonsterCount() > 0 ? 430 : 220;
            else if (card.IsCode(CardId.WollowFounderOfTheDrudgeDragons))
                score += Enemy.Graveyard.Count > 0 ? 400 : 210;
            else if (card.IsCode(CardId.TheUndyingLegion))
                score += 40;
            else
                score += 180;

            return score;
        }

        private IList<ClientCard> SelectReverieOverlayReloadTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (cards == null || min <= 0)
                return null;

            ClientCard target = cards
                .Where(c => GetReverieOverlayReloadScore(c) > int.MinValue)
                .OrderByDescending(GetReverieOverlayReloadScore)
                .ThenBy(c => c.Overlays == null ? 0 : c.Overlays.Count)
                .ThenByDescending(c => c.Attack)
                .FirstOrDefault();
            if (target == null)
                return null;

            int materials = target.Overlays == null ? 0 : target.Overlays.Count;
            DebugRoute("Reverie banished reload target=" + target.Id
                + " materials=" + materials
                + " score=" + GetReverieOverlayReloadScore(target));
            reverieOverlaySelectionPending = false;
            return Util.CheckSelectCount(
                new List<ClientCard> { target }, cards, min, max);
        }

        private int GetCommittedRank10ReservedMainMonsterZones()
        {
            if (!eldlichRouteRank10CommitPending)
                return 0;

            int reserved = 0;
            if (!Bot.HasInMonstersZone(CardId.EldlichTheMadGoldenLord, faceUp: true))
                reserved++;
            if (!Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
                reserved++;
            return reserved;
        }

        private int GetFriendlyChainReservedMainMonsterZones()
        {
            if (!IsFriendlyChainInProgress())
                return 0;

            ClientCard source = Util.GetLastChainCard();
            if (source == null)
                return 0;

            if (source.IsCode(CardId.FallenAngelOfTheGoldenLand))
                return 2;
            if (source.IsCode(CardId.PumpkingTheKingOfGraveGhosts,
                    CardId.CallOfTheHaunted,
                    CardId.ArmyOfTheHaunted,
                    CardId.Mezuki,
                    CardId.EldlichTheGoldenLord,
                    CardId.FlyingMary))
            {
                return 1;
            }

            return 0;
        }

        private int GetReservedMainMonsterZonesForPriorityRoute()
        {
            return Math.Max(GetCommittedRank10ReservedMainMonsterZones(),
                GetFriendlyChainReservedMainMonsterZones());
        }

        private bool HasSurplusMainMonsterZoneForReverie()
        {
            int freeZones = GetOpenMainMonsterZoneCount();
            int reservedZones = GetReservedMainMonsterZonesForPriorityRoute();
            bool available = freeZones - reservedZones > 0;
            if (!available)
            {
                DebugRoute("HOLD Reverie: free MZ=" + freeZones
                    + " reserved for priority route=" + reservedZones);
            }
            return available;
        }

        private bool OfficiatingReverieActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player != 0
                    || HasImmediatePumpkingActionPending()
                    || HasConfirmedOpenStateBattleLethal())
                {
                    return false;
                }


                if (Bot.Hand.Count <= 1 || !HasSurplusMainMonsterZoneForReverie())
                    return false;

                if (changshiHandRescueRouteActive
                    && Bot.HasInHand(CardId.ChangshiTheSpiridao))
                {
                    DebugRoute("ACCEPT Reverie rescue: discard Changshi and Special Summon");
                    return true;
                }

                bool pumpkingLineExtender = HasSmallPumpkingOnField()
                    && HasChangshiOnField()
                    && !HasSamuelOnField()
                    && !HasGreatPumpkingOnField();
                if (pumpkingLineExtender)
                {
                    DebugRoute("ACCEPT Reverie from hand after Pumpking/Changshi setup");
                    return true;
                }

                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                if (HasConfirmedOpenStateBattleLethal()
                    || !HasSurplusMainMonsterZoneForReverie())
                {
                    return false;
                }

                return Bot.Graveyard.Any(c => c != Card
                    && IsZombie(c)
                    && c.IsCanRevive());
            }

            if (Card.Location == CardLocation.Removed)
            {
                bool canReload = Bot.GetMonsters().Any(c => c.IsFaceup()
                    && IsZombie(c) && c.HasType(CardType.Xyz));
                reverieOverlaySelectionPending = canReload;
                if (canReload)
                    DebugRoute("ACCEPT Reverie banished effect: reload spent Zombie Xyz");
                return canReload;
            }

            return false;
        }
        private bool ArmyOfTheHauntedActivate()
        {
            if (WillCardEffectBeNegated())
                return false;
            if (eldlichRouteRank10CommitPending)
            {
                DebugRoute("HOLD Army: finish committed Rank 10 first");
                return false;
            }

            if ((Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
                && HasFaceupCall())
            {


                if (!HasUnusedArmySpecialSummonAvailable())
                    return false;

                armySpecialSummonEffectCommittedThisTurn = true;
                DebugRoute("ACCEPT Army Special Summon before considering Mezuki");
                return true;
            }

            return Card.Location == CardLocation.Grave
                && HasOpenSpellZone()
                && Bot.HasInGraveyard(CardId.CallOfTheHaunted);
        }

        private bool HasUnusedArmySpecialSummonAvailable()
        {
            if (armySpecialSummonEffectCommittedThisTurn
                || !HasFaceupCall()
                || !HasOpenMainMonsterZone())
            {
                return false;
            }

            bool armyAvailable = Bot.Hand.Any(c => c != null
                    && c.IsCode(CardId.ArmyOfTheHaunted))
                || Bot.Graveyard.Any(c => c != null
                    && c.IsCode(CardId.ArmyOfTheHaunted)
                    && c.IsCanRevive());
            if (!armyAvailable)
                return false;


            if (ashReplayLineActive)
            {
                return HasGreatPumpkingOnField()
                    && greatPumpkingSearchResolved
                    && NeedsAdditionalLevel6BodyForCurrentPlan();
            }

            return !HasImmediatePumpkingActionPending()
                && NeedsAdditionalLevel6BodyForCurrentPlan();
        }

        private bool CallOfTheHauntedActivate()
        {
            if (WillCardEffectBeNegated() || !HasOpenMainMonsterZone())
                return false;
            if (Duel.Player == 1 && IsFriendlyChainInProgress())
            {
                DebugRoute("HOLD Call: do not chain to our own effect");
                return false;
            }

            List<ClientCard> revivable = Bot.Graveyard
                .Where(c => c.IsMonster() && c.IsCanRevive())
                .Where(c => !HasFaceupFieldSpell()
                    || !c.IsCode(CardId.DoomkingBalerdroch))
                .ToList();
            if (revivable.Count == 0)
            {
                if (HasFaceupFieldSpell()
                    && Bot.HasInGraveyard(CardId.DoomkingBalerdroch))
                {
                    DebugRoute("HOLD Call of the Haunted: Doomking will self-revive under Field Spell");
                }
                return false;
            }

            if (Duel.Player == 0)
            {


                bool pumpkingComboRevive = callSetByPumpking
                    && Card.IsFacedown()
                    && HasPumpkingInGrave()
                    && !pumpkingSummonEffectAttempted
                    && GetOpenMainMonsterZoneCount() >= 2
                    && HasPumpkingDeckSummonTargetInDeck();
                if (pumpkingComboRevive)
                {
                    plannedCallReviveId = CardId.PumpkingTheKingOfGraveGhosts;
                    callReviveSelectionPending = true;
                    DebugRoute("ACCEPT Call of the Haunted immediately: revive Pumpking");
                    return true;
                }


                if (Duel.CurrentChain.Count > 0)
                {
                    DebugRoute("HOLD Call lethal/extender: wait for open state");
                    return false;
                }

                ClientCard lethalTarget = GetOwnTurnCallLethalTarget(revivable);
                if (lethalTarget != null)
                {
                    plannedCallReviveId = lethalTarget.Id;
                    callReviveSelectionPending = true;
                    DebugRoute("ACCEPT Call for lethal: revive=" + lethalTarget.Id
                        + " atk=" + lethalTarget.Attack
                        + " enemyLP=" + Enemy.LifePoints);
                    return true;
                }

                return false;
            }


            if (Duel.CurrentChain.Count == 0 || Duel.LastChainPlayer != 1)
            {
                DebugRoute("HOLD Call: wait for opposing chain");
                return false;
            }

            ClientCard target = GetOpponentTurnCallReviveTarget(revivable);
            if (target == null)
            {
                DebugRoute("HOLD Call: no concrete Snakehair/Mammoth interaction target");
                return false;
            }

            plannedCallReviveId = target.Id;
            callReviveSelectionPending = true;
            DebugRoute("ACCEPT Call on opponent chain: planned revive=" + target.Id);
            return true;
        }
        private bool NeedsAdditionalLevel6BodyForCurrentPlan()
        {
            if (Duel.Player != 0 || HasCorePumpkingEndboard())
                return false;

            int requiredBodies = 0;


            if (ashReplayLineActive || currentComboRoute == ComboRoute.UraraRecovery)
            {
                if (!HasGreatPumpkingOnField() || HasSamuelOnField())
                    return false;

                requiredBodies = 2;
            }
            else if (HasGreatPumpkingOnField() && !HasSamuelOnField())
            {
                requiredBodies = 2;
            }
            else if (HasSamuelOnField() && !HasGreatPumpkingOnField())
            {
                requiredBodies = 2;
            }
            else if (!HasSamuelOnField() && !HasGreatPumpkingOnField()
                && (IsPumpkingComboInProgress()
                    || currentComboRoute == ComboRoute.NormalPumpking))
            {
                requiredBodies = 3;
            }

            if (requiredBodies > 0
                && CountFreeLevel6ForGreatPumpking() < requiredBodies)
            {
                return true;
            }


            return !HasSmallPumpkingOnField()
                && !pumpkingSummonEffectAttempted
                && Bot.Graveyard.Any(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                    && c.IsCanRevive());
        }

        private bool CanMezukiReviveCreateImmediateFollowUp(ClientCard target)
        {
            if (!IsWorthwhileMezukiReviveTarget(target) || !IsLevel6Zombie(target))
                return false;


            if (target.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                && !pumpkingSummonEffectAttempted
                && GetOpenMainMonsterZoneCount() >= 2
                && HasPumpkingDeckSummonTargetInDeck())
            {
                return true;
            }


            int resultingFreeLevel6 = CountFreeLevel6ForGreatPumpking() + 1;
            if (resultingFreeLevel6 < 2)
                return false;

            if (HasGreatPumpkingOnField() && !HasSamuelOnField()
                && Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel))
            {
                return true;
            }

            if (HasSamuelOnField() && !HasGreatPumpkingOnField()
                && Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing))
            {
                return true;
            }

            return Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                || Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                || Bot.HasInExtra(CardId.DhampirVampireSheridan)
                || Bot.HasInExtra(CardId.EvolzarLars)
                || Bot.HasInExtra(CardId.WollowFounderOfTheDrudgeDragons);
        }

        private bool MezukiActivate()
        {
            if (!CanUseEarthMonsterEffects() || WillCardEffectBeNegated()
                || eldlichRouteRank10CommitPending)
                return false;
            if (Card.Location != CardLocation.Grave || !HasOpenMainMonsterZone())
                return false;
            if (HasImmediatePumpkingActionPending())
                return false;


            if (HasUnusedArmySpecialSummonAvailable())
            {
                DebugRoute("HOLD Mezuki: unused Army Special Summon must resolve first");
                return false;
            }

            RecalculateStrategicPlan("evaluate Mezuki Level 6 extension");
            if (!NeedsAdditionalLevel6BodyForCurrentPlan())
            {
                DebugRoute("HOLD Mezuki: current plan does not need another Level 6 body");
                return false;
            }

            bool hasLevel6Target = Bot.Graveyard.Any(c => c != Card
                && CanMezukiReviveCreateImmediateFollowUp(c));
            if (!hasLevel6Target)
            {
                DebugRoute("HOLD Mezuki: no revive target has an immediate follow-up");
                return false;
            }

            mezukiReviveSelectionPending = true;
            mezukiLevel6ExtensionPending = true;
            DebugRoute("ACCEPT Mezuki: revive completes an executable follow-up");
            return true;
        }

        private bool VampireGraceActivate()
        {
            if (eldlichRouteRank10CommitPending)
                return false;


            if (WillCardEffectBeNegated()
                || Card.Location != CardLocation.Grave
                || vampireGraceReviveCommittedThisTurn
                || Duel.Player != 0
                || (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                || !CanPayVampireGraceCost()
                || !HasOpenMainMonsterZone()
                || !HasVampireGraceRank6Payoff())
            {
                return false;
            }


            if (CountFreeLevel6ForGreatPumpking() < 1)
            {
                DebugRoute("HOLD Vampire Grace: paid revive has no immediate Rank 6 partner");
                return false;
            }

            vampireGraceReviveCommittedThisTurn = true;
            vampireGraceRouteActive = true;
            DebugRoute("ACCEPT Vampire Grace GY revive: immediate Level 6 pair available");
            return true;
        }
        private bool ChangshiTheSpiridaoActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player != 0 || changshiMillAttempted)
                    return false;
                if (!HasChangshiDeckTargetAvailable())
                {
                    DebugRoute("HOLD Changshi mill: no known Zombie remains in Deck; preserve hand");
                    return false;
                }
                changshiMillAttempted = true;
                DebugRoute("ACCEPT Changshi mill from Deck only");
                return true;
            }

            if (Card.Location == CardLocation.Removed)
                return HasOpenMainMonsterZone() && Bot.Graveyard.Any(c => IsZombie(c));

            return false;
        }

        private bool EldlichTheGoldenLordActivate()
        {
            if (!CanUseLightMonsterEffects() || WillCardEffectBeNegated())
                return false;

            bool hasHandSpellTrapCost = Bot.Hand.Any(c => c != Card
                && (c.IsSpell() || c.IsTrap()));

            if (Card.Location == CardLocation.Hand)
            {
                if (HasConfirmedOpenStateBattleLethal())
                    return false;

                bool canActivate = hasHandSpellTrapCost
                    && GetEnemyFieldPriority().Count > 0;
                if (!canActivate)
                    return false;

                eldlichHandSelectionPending = true;
                eldlichHandCostPromptCompleted = false;
                DebugRoute("ACCEPT Eldlich hand effect: enemy-only field target");
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                if (HasConfirmedOpenStateBattleLethal())
                {
                    DebugRoute("HOLD Eldlich GY effect: current board is already lethal");
                    return false;
                }

                if (CanTakeProductivePumpkingActionNow())
                {
                    DebugRoute("HOLD Eldlich GY effect: finish available Hublot/Pumpking action first");
                    return false;
                }


                ClientCard exactCost = GetBestEldlichGraveFieldCost();
                if (!HasOpenMainMonsterZone() || exactCost == null)
                {
                    pendingEldlichGraveFieldCost = null;
                    DebugRoute("HOLD Eldlich GY effect: no whitelisted field cost");
                    return false;
                }

                pendingEldlichGraveFieldCost = exactCost;
                DebugRoute("COMMIT Eldlich GY cost instance=" + exactCost.Id
                    + " seq=" + exactCost.Sequence);
                return true;
            }

            return false;
        }

        private bool GreatMammothActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (pendingMammothDestroyTarget != null)
            {
                if (CanMammothDestroyTarget(pendingMammothDestroyTarget))
                {
                    DebugRoute("ACCEPT Mammoth pre-emptive trigger target="
                        + pendingMammothDestroyTarget.Id);
                    return true;
                }
                DebugRoute("CANCEL Mammoth target: protected, untargetable, or left field");
                pendingMammothDestroyTarget = null;
            }

            ClientCard target = GetMammothPreemptTarget();
            if (target == null)
            {
                DebugRoute("HOLD Mammoth: no destructible meaningful target");
                return false;
            }

            pendingMammothDestroyTarget = target;
            DebugRoute(Duel.Player == 1
                ? "ACCEPT Mammoth opponent-turn trigger target=" + target.Id
                : "ACCEPT Mammoth own-turn trigger target=" + target.Id);
            return true;
        }


        private bool PumpkingGreatGhostKingSummon()
        {

            if (ShouldHoldRank6ForFlyingMaryRank10())
            {
                DebugRoute("HOLD Great Pumpking: finish Flying Mary Rank 10 line first");
                return false;
            }

            RecalculateStrategicPlan("evaluate Great Pumpking Xyz");

            if (ashReplayLineActive)
            {
                if (!Bot.HasInGraveyard(CardId.AshBlossom) || HasGreatPumpkingOnField())
                    return false;

                List<ClientCard> replayMaterials = GetGreatPumpkingMaterials();
                if (replayMaterials.Count != 2)
                    return false;

                AI.SelectMaterials(replayMaterials);
                DebugRoute("ACCEPT Urara-route Great Pumpking: Hublot + small Pumpking");
                return true;
            }


            List<ClientCard> comboMaterials = GetGreatPumpkingMaterials();
            if (comboMaterials.Count == 2)
            {
                AI.SelectMaterials(comboMaterials);
                DebugRoute(changshiHandRescueRouteActive
                    ? "ACCEPT rescue Great Pumpking: linked Level 6 + Level 6"
                    : IsVampireGraceRouteLive()
                        ? "ACCEPT Vampire Grace route Great Pumpking: linked Level 6 + Level 6"
                        : "ACCEPT normal-route Great Pumpking after Samuel revive");
                return true;
            }


            List<ClientCard> plannedSamuelMaterials = GetSamuelMaterials();
            bool samuelRouteAvailable = plannedSamuelMaterials.Count == 2
                && CanNormalSamuelFirstReachGreatPumpking(plannedSamuelMaterials);
            if (samuelRouteAvailable && !HasSamuelOnField())
                return false;

            List<ClientCard> fallback = GetRank6Materials(true).Take(2).ToList();
            if (fallback.Count != 2)
                return false;

            AI.SelectMaterials(fallback);
            DebugRoute("ACCEPT fallback Great Pumpking; prevent end-phase pass");
            return true;
        }

        private int GetPotentialAttackContribution(ClientCard card)
        {
            if (card == null || !card.IsFaceup() || card.Attack <= 0)
                return 0;
            if (card.IsAttack())
                return card.Attack;


            if (BattlePhaseIsAvailableThisTurn()
                && card.IsCode(CardId.OfficiatorOfDoomSamuel, CardId.Varudras))
            {
                return card.Attack;
            }

            return 0;
        }

        private int GetPotentialDirectBattleDamage(
            IEnumerable<ClientCard> excluded = null)
        {
            HashSet<ClientCard> excludedSet = excluded == null
                ? new HashSet<ClientCard>()
                : new HashSet<ClientCard>(excluded.Where(c => c != null));
            return Bot.GetMonsters()
                .Where(c => c != null && !excludedSet.Contains(c))
                .Sum(GetPotentialAttackContribution);
        }

        private bool HasConfirmedLethalWithoutRank6Overlay()
        {
            return BattlePhaseIsAvailableThisTurn()
                && Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0
                && GetPotentialDirectBattleDamage() >= Enemy.LifePoints;
        }

        private ClientCard GetSheridanSurplusRemovalTarget()
        {
            return GetEnemyFieldPriority(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()).ToList(),
                    true)
                .FirstOrDefault(c => c != null
                    && c.Controller == 1
                    && c.IsOnField()
                    && c.IsFaceup());
        }

        private bool WouldWollowPowerMatter(List<ClientCard> materials)
        {
            if (materials == null || materials.Count != 2
                || Enemy.Graveyard.Count <= 0)
            {
                return false;
            }

            NamedCard wollowData = NamedCard.Get(
                CardId.WollowFounderOfTheDrudgeDragons);
            int wollowBaseAttack = wollowData == null ? 0 : wollowData.Attack;
            int boost = Enemy.Graveyard.Count * 100;
            List<ClientCard> remainingAttackers = Bot.GetMonsters()
                .Where(c => c != null
                    && !materials.Contains(c)
                    && GetPotentialAttackContribution(c) > 0)
                .ToList();

            int currentDamage = GetPotentialDirectBattleDamage();
            int materialDamage = materials.Sum(GetPotentialAttackContribution);
            int projectedDamage = currentDamage - materialDamage
                + wollowBaseAttack
                + boost * (remainingAttackers.Count + 1);


            if (BattlePhaseIsAvailableThisTurn()
                && Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0
                && currentDamage < Enemy.LifePoints
                && projectedDamage >= Enemy.LifePoints)
            {
                return true;
            }


            int enemyPower = Enemy.GetMonsters()
                .Where(c => c != null && c.IsFaceup())
                .Select(c => c.GetDefensePower())
                .DefaultIfEmpty(0)
                .Max();
            int currentPower = Bot.GetMonsters()
                .Where(c => c != null)
                .Select(GetPotentialAttackContribution)
                .DefaultIfEmpty(0)
                .Max();
            int projectedPower = Math.Max(
                wollowBaseAttack + boost,
                remainingAttackers
                    .Select(c => c.Attack + boost)
                    .DefaultIfEmpty(0)
                    .Max());
            return enemyPower > 0
                && currentPower <= enemyPower
                && projectedPower > enemyPower;
        }

        private SurplusRank6Plan GetSurplusRank6Plan()
        {
            List<ClientCard> materials = GetRank6Materials(false)
                .Take(2)
                .ToList();
            if (materials.Count != 2)
                return SurplusRank6Plan.None;


            if (HasConfirmedLethalWithoutRank6Overlay())
                return SurplusRank6Plan.None;


            if (Bot.HasInExtra(CardId.DhampirVampireSheridan)
                && GetSheridanSurplusRemovalTarget() != null)
            {
                return SurplusRank6Plan.SheridanRemoval;
            }


            if (IsLaterTurnRecoveryState()
                && Bot.HasInExtra(CardId.EvolzarLars))
            {
                return SurplusRank6Plan.LarsNegate;
            }


            if (Bot.HasInExtra(CardId.WollowFounderOfTheDrudgeDragons)
                && WouldWollowPowerMatter(materials))
            {
                return SurplusRank6Plan.WollowPower;
            }


            return Bot.HasInExtra(CardId.EvolzarLars)
                ? SurplusRank6Plan.LarsNegate
                : SurplusRank6Plan.None;
        }

        private bool DhampirVampireSheridanSummon()
        {

            if (ShouldHoldRank6ForFlyingMaryRank10()
                || ShouldHoldGenericRank6ForPumpkingCombo()
                || GetSurplusRank6Plan() != SurplusRank6Plan.SheridanRemoval)
            {
                return false;
            }

            ClientCard target = GetSheridanSurplusRemovalTarget();
            if (target == null || !SelectRank6Materials(zombiesOnly: false))
                return false;

            DebugRoute("ACCEPT surplus Rank 6 Sheridan: remove enemy card id="
                + target.Id);
            return true;
        }

        private bool OfficiatorOfDoomSamuelSummon()
        {

            if (ShouldHoldRank6ForFlyingMaryRank10())
                return false;

            RecalculateStrategicPlan("evaluate Samuel Xyz");
            if (ashReplayLineActive
                && currentComboRoute != ComboRoute.UraraRecovery)
            {
                return false;
            }
            if (!ashReplayLineActive
                && currentComboRoute != ComboRoute.NormalPumpking)
            {
                return false;
            }

            if (ashReplayLineActive)
            {
                if (!HasGreatPumpkingOnField() || !Bot.HasInGraveyard(CardId.AshBlossom))
                    return false;
            }
            else if (!IsVampireGraceRouteLive()
                && !Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive()))
            {
                return false;
            }

            List<ClientCard> materials = GetSamuelMaterials();
            if (materials.Count != 2)
                return false;

            bool changshiRescueSamuel = changshiHandRescueRouteActive
                && HasGreatPumpkingOnField()
                && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao)
                && materials.Count == 2;
            bool vampireGraceSamuel = IsVampireGraceRouteLive()
                && HasGreatPumpkingOnField()
                && materials.Count == 2;

            if (!ashReplayLineActive
                && !changshiRescueSamuel
                && !vampireGraceSamuel
                && !CanNormalSamuelFirstReachGreatPumpking(materials))
            {
                DebugRoute("HOLD Samuel summon: would leave fewer than two Level 6 bodies for Great Pumpking");
                return false;
            }

            AI.SelectMaterials(materials);
            DebugRoute(changshiRescueSamuel
                ? "ACCEPT rescue Samuel: remaining Level 6 + Level 6; revive Changshi next"
                : vampireGraceSamuel
                    ? "ACCEPT Vampire Grace route Samuel: remaining Level 6 + Level 6"
                    : "ACCEPT Xyz Samuel with " + materials[0].Id + "," + materials[1].Id);
            return true;
        }

        private bool EvolzarLarsSummon()
        {
            if (ShouldHoldRank6ForFlyingMaryRank10()
                || ShouldHoldGenericRank6ForPumpkingCombo()
                || GetSurplusRank6Plan() != SurplusRank6Plan.LarsNegate)
            {
                return false;
            }

            if (!SelectRank6Materials(zombiesOnly: false))
                return false;

            DebugRoute("ACCEPT surplus Rank 6 Lars: non-lethal board needs next-turn negate");
            return true;
        }

        private bool WollowSummon()
        {

            if (ShouldHoldRank6ForFlyingMaryRank10()
                || ShouldHoldGenericRank6ForPumpkingCombo()
                || GetSurplusRank6Plan() != SurplusRank6Plan.WollowPower)
            {
                return false;
            }

            if (!SelectRank6Materials(zombiesOnly: false))
                return false;

            DebugRoute("ACCEPT surplus Rank 6 Wollow: graveyard boost changes battle math");
            return true;
        }

        private bool TheUndyingLegionSummon()
        {
            if (ShouldHoldRank6ForFlyingMaryRank10())
                return false;

            if (changshiHandRescueRouteActive
                && !changshiHandRescueEldlichLoaded)
            {
                if (CanContinueChangshiHandRescueBeforeUndying())
                {
                    DebugRoute("HOLD Undying: finish Samuel -> Changshi -> Eldlich rescue line");
                    return false;
                }

                DebugRoute("RELEASE Changshi-hand rescue: no executable pre-Undying step remains");
                changshiHandRescueRouteActive = false;
            }

            List<ClientCard> bases = GetUndyingOverlayBases();
            if (bases.Count == 0)
            {
                DebugRoute("HOLD Undying summon: no legal Rank 6 Zombie Xyz base with material");
                return false;
            }

            ClientCard samuel = bases.FirstOrDefault(c =>
                c.IsCode(CardId.OfficiatorOfDoomSamuel));
            if (samuel != null
                && !samuelFieldEffectCommittedThisTurn)
            {
                ClientCard valueTarget = GetSamuelOwnTurnValueReviveTarget(
                    Bot.Graveyard.Where(c => c != samuel));
                if (valueTarget != null)
                {
                    DebugRoute("HOLD Undying: Samuel still has profitable revive target="
                        + valueTarget.Id);
                    return false;
                }
            }

            ClientCard sheridan = bases.FirstOrDefault(c =>
                c.IsCode(CardId.DhampirVampireSheridan));
            if (sheridan != null
                && !sheridanRemovalAttemptedThisTurn
                && GetSheridanSurplusRemovalTarget() != null)
            {
                DebugRoute("HOLD Undying: Sheridan still has a live removal target");
                return false;
            }

            ClientCard greatPumpking = bases.FirstOrDefault(c =>
                c.IsCode(CardId.PumpkingTheGreatGhostKing));
            if (greatPumpking != null
                && HasSafeGreatPumpkingBounceTarget()
                && !greatPumpkingBounceAttempted)
            {


                return false;
            }

            if (ashReplayLineActive
                && !Bot.HasInHand(CardId.AshBlossom)
                && !greatPumpkingBounceAttempted)
            {
                return false;
            }

            ClientCard baseXyz = greatPumpking ?? bases
                .Where(c => !c.IsCode(CardId.PumpkingTheGreatGhostKing)
                    || greatPumpkingBounceAttempted
                    || !HasSafeGreatPumpkingBounceTarget())
                .OrderBy(c => c.Overlays.Count)
                .ThenBy(GetMaterialValue)
                .FirstOrDefault();
            if (baseXyz == null)
                return false;

            AI.SelectMaterials(new List<ClientCard> { baseXyz });
            DebugRoute("ACCEPT overlay Undying on loaded Rank 6 base="
                + baseXyz.Id + " materials=" + baseXyz.Overlays.Count);
            return true;
        }

        private bool VarudrasSummon()
        {
            if (!eldlichRouteRank10CommitPending)
                RecalculateStrategicPlan("evaluate Varudras extension");

            if (zombieLockedThisTurn || !Bot.HasInExtra(CardId.Varudras))
                return false;

            List<ClientCard> materials = GetRank10Materials();
            if (materials.Count != 2)
            {
                if (eldlichRouteRank10CommitPending)
                    DebugRoute("HOLD committed Rank 10: waiting for second Level 10 body");
                return false;
            }

            if (!eldlichRouteRank10CommitPending && !CanBuildSurplusVarudras())
                return false;

            AI.SelectMaterials(materials);
            DebugRoute("ACCEPT Varudras: consume ready Level 10 pair before any other action");
            return true;
        }

        private bool FallenAngelSummon()
        {

            if (zombieLockedThisTurn
                || !CanUseLightMonsterEffects()
                || HasConfirmedOpenStateBattleLethal())
            {
                return false;
            }
            if (CanTakeProductivePumpkingExtraDeckStep())
            {
                DebugRoute("HOLD Fallen Angel: complete available Great Pumpking/Pumpking action first");
                return false;
            }
            if (!HasEldlichRouteExtraDeck() || !HasEldlichLinkSeedOnField())
                return false;


            ClientCard eldlich = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.EldlichTheGoldenLord));
            if (eldlich == null)
                return false;

            AI.SelectMaterials(new List<ClientCard> { eldlich }, HintMsg.Release);
            eldlichRouteActive = true;
            SetStrategicPlan(StrategicGoal.CompleteEldlichRoute,
                ComboRoute.EldlichRank10, "Fallen Angel accepted");
            DebugRoute("ACCEPT Fallen Angel: reserve board for Flying Mary Rank 10 route");
            return true;
        }

        private int GetProjectedOpenMainMonsterZonesAfterFlyingMary(
            IList<ClientCard> materials)
        {
            int projected = GetOpenMainMonsterZoneCount();
            if (materials != null)
            {
                projected += materials.Count(c => c != null
                    && c.Location == CardLocation.MonsterZone
                    && c.Sequence < 5);
            }

            ClientCard emzOccupant = Bot.GetMonsters().FirstOrDefault(IsInExtraMonsterZone);
            bool maryCanUseEmz = emzOccupant == null
                || (materials != null && materials.Contains(emzOccupant));
            if (!maryCanUseEmz)
                projected--;

            return projected;
        }

        private bool HasFlyingMaryRank10FollowupZoneCapacity(
            IList<ClientCard> materials)
        {
            int projected = GetProjectedOpenMainMonsterZonesAfterFlyingMary(materials);
            if (projected >= 2)
                return true;

            DebugRoute("HOLD Flying Mary Rank 10 commit: projected free MZ="
                + projected + " need=2 for Mad Golden + Eldlich");
            return false;
        }

        private bool FlyingMaryEldlichRouteSummon()
        {

            if (!eldlichRouteActive || !HasRecoverableEldlichForFlyingMary())
                return false;

            List<ClientCard> materials = GetFlyingMaryEldlichMaterials();
            if (materials.Count != 2
                || !HasFlyingMaryRank10FollowupZoneCapacity(materials)
                || HasConfirmedOpenStateBattleLethal())
            {
                return false;
            }

            AI.SelectMaterials(materials);
            eldlichRouteRank10CommitPending = true;
            DebugRoute("ACCEPT Flying Mary Eldlich route: hard-lock Rank 10 finish");
            return true;
        }

        private List<ClientCard> GetFlyingMaryPumpkingResetMaterials()
        {


            if (Duel.Player != 0 || Duel.Turn <= 2
                || pumpkingSummonEffectAttempted || pumpkingSummonEffectResolved
                || !HasOpenSpellZone())
            {
                return new List<ClientCard>();
            }

            ClientCard pumpking = Bot.GetMonsters().FirstOrDefault(c => c != null
                && c.IsFaceup()
                && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                && IsZombie(c));
            if (pumpking == null)
                return new List<ClientCard>();

            ClientCard spentPartner = Bot.GetMonsters()
                .Where(c => c != null && c != pumpking
                    && IsAllowedZombieLinkMaterial(c)
                    && (IsSpentZombieXyz(c) || IsDeltaToken(c)))
                .OrderBy(GetZombieLinkMaterialValue)
                .ThenBy(GetMaterialValue)
                .ThenBy(c => c.Attack)
                .FirstOrDefault();
            if (spentPartner == null)
                return new List<ClientCard>();

            return new List<ClientCard> { spentPartner, pumpking };
        }

        private bool FlyingMarySummon()
        {

            if (eldlichRouteRank10CommitPending)
            {
                DebugRoute("HOLD generic Flying Mary: finish committed Rank 10 first");
                return false;
            }

            if (HasConfirmedOpenStateBattleLethal())
                return false;

            if (!eldlichRouteRank10CommitPending
                && ShouldPreferVampireSuckerOverFlyingMary())
            {
                DebugRoute("HOLD Flying Mary: Vampire Sucker should precede Eldlich revival");
                return false;
            }

            List<ClientCard> pumpkingResetMaterials =
                GetFlyingMaryPumpkingResetMaterials();
            if (pumpkingResetMaterials.Count == 2)
            {
                AI.SelectMaterials(pumpkingResetMaterials);
                DebugRoute("ACCEPT Flying Mary board reset: spent Zombie Xyz + Pumpking");
                return true;
            }


            if (Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
            {
                List<ClientCard> eldlichMaterials = GetFlyingMaryEldlichMaterials();
                if (eldlichMaterials.Count == 2)
                {
                    AI.SelectMaterials(eldlichMaterials);
                    return true;
                }
            }


            if (Duel.Turn >= 2 && Bot.HasInGraveyard(CardId.PumpkingTheKingOfGraveGhosts))
            {
                List<ClientCard> materials = GetLinkZombieMaterials()
                    .Take(2)
                    .ToList();
                if (materials.Count == 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }

            return false;
        }

        private bool VampireSuckerSummon()
        {
            if (ShouldHoldRank6ForFlyingMaryRank10())
            {
                DebugRoute("HOLD Vampire Sucker: finish committed Flying Mary Rank 10 line");
                return false;
            }


            if (!ShouldPreferVampireSuckerOverFlyingMary())
            {
                DebugRoute("HOLD Vampire Sucker: no safe draw/recovery bridge");
                return false;
            }

            List<ClientCard> drawMaterials = GetVampireSuckerMaterialPlan();
            if (drawMaterials.Count != 2)
                return false;

            AI.SelectMaterials(drawMaterials);
            DebugRoute(IsLaterTurnRecoveryState()
                ? "ACCEPT Vampire Sucker: recovery draw bridge before generic Flying Mary"
                : "ACCEPT Vampire Sucker: protected board plus disposable materials");
            return true;
        }


        private bool PumpkingGreatGhostKingActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            int searchDescription = Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 1);
            int bounceDescription = Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 2);
            bool explicitBounce = ActivateDescription == bounceDescription;
            bool searchWindow = ActivateDescription == searchDescription
                || (greatPumpkingSearchWindowPending && !explicitBounce);


            if (searchWindow && !greatPumpkingSearchAttempted
                && !greatPumpkingSearchResolved)
            {
                greatPumpkingSearchAttempted = true;
                greatPumpkingSearchWindowPending = false;
                DebugRoute("ACCEPT Great Pumpking on-summon search; desc="
                    + ActivateDescription);
                return true;
            }

            if (explicitBounce)
            {


                if (ashReplayLineActive
                    && !Bot.HasInMonstersZone(CardId.AshBlossom, faceUp: true))
                {
                    return false;
                }
                if (greatPumpkingBounceAttempted || !HasSafeGreatPumpkingBounceTarget())
                    return false;
                greatPumpkingBounceAttempted = true;
                DebugRoute("ACCEPT Great Pumpking bounce");
                return true;
            }

            return false;
        }

        private bool DhampirVampireSheridanActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.MonsterZone
                && Card.Overlays != null
                && Card.Overlays.Count > 0
                && GetSheridanSurplusRemovalTarget() != null)
            {
                sheridanRemovalAttemptedThisTurn = true;
                DebugRoute("ACCEPT Sheridan removal before any Undying overlay");
                return true;
            }


            return Card.Location == CardLocation.Grave
                && HasOpenMainMonsterZone()
                && Enemy.Graveyard.Any(c => c.IsMonster());
        }

        private bool OfficiatorOfDoomSamuelActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.Grave)
            {
                bool hasRecycleTarget = Bot.Graveyard.Any(c => c.IsMonster())
                    || Enemy.Graveyard.Any(c => c.IsMonster());
                samuelGraveRecycleSelectionPending = hasRecycleTarget;
                return hasRecycleTarget;
            }

            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;
            if (Card.Overlays.Count <= 0 || !HasOpenMainMonsterZone())
                return false;

            List<ClientCard> reviveTargets = Bot.Graveyard
                .Where(c => c != Card && IsZombie(c) && c.IsCanRevive())
                .ToList();
            if (reviveTargets.Count == 0)
                return false;

            if (Duel.Player == 1)
            {
                InterruptPlan plan = BuildSamuelOpponentPlan();
                if (plan == null)
                {
                    DebugRoute("HOLD Samuel: no reactive/pre-emptive plan");
                    return false;
                }

                CommitSamuelInterruptPlan(plan);
                return true;
            }

            ClientCard ownTurnValueTarget = null;

            if (changshiHandRescueRouteActive
                && !changshiHandRescueEldlichLoaded)
            {
                ownTurnValueTarget = reviveTargets.FirstOrDefault(c => c != null
                    && c.IsCode(CardId.ChangshiTheSpiridao));
                if (ownTurnValueTarget == null)
                {
                    DebugRoute("HOLD rescue Samuel: Changshi is not a legal revive target");
                    return false;
                }
            }


            else if (ashReplayLineActive)
            {
                if (!reviveTargets.Any(c => c.IsCode(CardId.AshBlossom)))
                {
                    DebugRoute("HOLD Samuel revive: Urara route has no legal Ash target");
                    return false;
                }
            }
            else
            {
                bool completesGreatPumpking = CountFreeLevel6ForGreatPumpking() >= 1
                    && reviveTargets.Any(c => IsLevel6Zombie(c)
                        && !c.IsCode(CardId.AshBlossom));
                if (!completesGreatPumpking)
                {
                    ownTurnValueTarget = GetSamuelOwnTurnValueReviveTarget(
                        reviveTargets, commitFollowUpTarget: true);
                    if (ownTurnValueTarget == null)
                    {
                        DebugRoute("HOLD Samuel revive: no profitable own-turn value target");
                        return false;
                    }
                }
            }

            pendingInterruptMode = InterruptMode.Hold;
            plannedSamuelReviveId = ownTurnValueTarget != null
                ? ownTurnValueTarget.Id : 0;
            plannedSamuelReviveResolved = false;
            samuelOpponentNegatePending = false;
            samuelNegateTarget = null;
            samuelReviveSelectionPending = true;
            samuelOwnTurnValueRevivePending = ownTurnValueTarget != null;
            samuelFieldEffectCommittedThisTurn = true;
            selectedSamuelReviveId = 0;
            DebugRoute(changshiHandRescueRouteActive
                && ownTurnValueTarget != null
                && ownTurnValueTarget.IsCode(CardId.ChangshiTheSpiridao)
                    ? "ACCEPT rescue Samuel revive: lock Changshi target"
                : ashReplayLineActive
                    ? "ACCEPT Samuel revive: lock Ash Blossom target"
                    : ownTurnValueTarget != null
                        ? "ACCEPT Samuel revive before Undying: value target="
                            + ownTurnValueTarget.Id
                        : "ACCEPT Samuel revive: Level 6 continuation");
            return true;
        }

        private bool WollowActivate()
        {
            if (WillCardEffectBeNegated() || Enemy.Graveyard.Count == 0
                || IsFriendlyChainInProgress())
            {
                return false;
            }

            int desc = ActivateDescription;
            bool shuffleEffect = desc == Util.GetStringId(
                CardId.WollowFounderOfTheDrudgeDragons, 0);
            bool summonOrSetEffect = desc == Util.GetStringId(
                CardId.WollowFounderOfTheDrudgeDragons, 1);

            if (shuffleEffect && Card.Overlays.Count < 1)
                return false;
            if (summonOrSetEffect
                && (Card.Overlays.Count < 2 || !HasOpenMainMonsterZone()))
            {
                return false;
            }

            ClientCard lastChainCard = Duel.CurrentChain.Count > 0
                && Duel.LastChainPlayer == 1
                ? Util.GetLastChainCard()
                : null;
            bool chainTargetsEnemyGraveMonster = Duel.CurrentChain.Count > 0
                && Duel.LastChainPlayer == 1
                && Duel.ChainTargets.Any(c => c != null
                    && c.Controller == 1
                    && c.Location == CardLocation.Grave
                    && c.IsMonster());
            bool reactingToEnemyGrave = lastChainCard != null
                && (lastChainCard.Location == CardLocation.Grave
                    || lastChainCard.Location == CardLocation.Removed
                    || chainTargetsEnemyGraveMonster);
            bool lateWindow = Duel.Phase >= DuelPhase.Main2;


            if (chainTargetsEnemyGraveMonster && summonOrSetEffect)
            {
                DebugRoute("HOLD Wollow summon/set mode: wait for graveyard-interrupt option");
                return false;
            }


            bool shouldActivate = reactingToEnemyGrave || lateWindow;
            DebugRoute(shouldActivate
                ? chainTargetsEnemyGraveMonster
                    ? "ACCEPT Wollow: intercept announced enemy GY revival target"
                    : "ACCEPT Wollow at reactive/late window"
                : "HOLD Wollow: preserve Quick Effect for meaningful timing");
            return shouldActivate;
        }

        private bool TheUndyingLegionActivate()
        {
            if (WillCardEffectBeNegated() || Duel.Player != 1)
                return false;
            if (IsFriendlyChainInProgress())
            {
                DebugRoute("HOLD Undying: do not chain to our own effect");
                return false;
            }
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;

            ClientCard target = GetUndyingReactiveTarget();
            if (target == null)
            {
                DebugRoute("HOLD Undying: wait for a meaningful opposing chain");
                return false;
            }

            pendingUndyingTarget = target;
            DebugRoute("ACCEPT Undying reactive attach target=" + target.Id
                + " location=" + target.Location);
            return true;
        }


        private bool FallenAngelActivate()
        {
            bool activate = Card.Location == CardLocation.Grave && !WillCardEffectBeNegated();
            if (activate && eldlichRouteRank10CommitPending)
                DebugRoute("ACCEPT Fallen Angel GY trigger: summon Mad Golden for Rank 10 line");
            return activate;
        }

        private bool IsMadGoldenControlTargetSafe(ClientCard target)
        {
            if (target == null
                || target.Controller != 1
                || target.Location != CardLocation.MonsterZone
                || !target.IsFaceup()
                || target.IsShouldNotBeTarget()
                || target.IsMonsterInvincible())
            {
                return false;
            }


            bool protectedDarkMagician = target.IsCode(CardId.DarkMagician)
                && Enemy.GetSpells().Any(c => c != null
                    && c.IsFaceup()
                    && !c.IsDisabled()
                    && c.IsCode(CardId.EternalSoul));
            return !protectedDarkMagician;
        }

        private ClientCard GetMadGoldenSafeControlTarget(
            IEnumerable<ClientCard> source = null)
        {
            return (source ?? Enemy.GetMonsters())
                .Where(IsMadGoldenControlTargetSafe)
                .OrderByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.IsFloodgate())
                .ThenByDescending(c => c.GetDefensePower())
                .FirstOrDefault();
        }

        private bool EldlichTheMadGoldenLordActivate()
        {
            if (WillCardEffectBeNegated() || eldlichRouteRank10CommitPending)
                return false;
            return Bot.GetMonsters().Any(c => c != Card && IsZombie(c))
                && GetMadGoldenSafeControlTarget() != null;
        }

        private bool FlyingMaryActivate()
        {
            if (WillCardEffectBeNegated())
                return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (eldlichRouteRank10CommitPending
                    && (Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                        || Bot.Banished.Any(c => c != null && c.IsCode(CardId.EldlichTheGoldenLord))))
                {
                    flyingMaryComebackPumpkingPending = false;
                    flyingMaryEldlichReviveSelectionPending = true;
                    DebugRoute("ACCEPT Flying Mary Eldlich revive: lock Eldlich target for Rank 10 line");
                    return true;
                }

                if (ShouldFlyingMaryRevivePumpkingForComeback())
                {
                    flyingMaryComebackPumpkingPending = true;
                    DebugRoute("ACCEPT Flying Mary comeback: revive Pumpking; "
                        + "turn=" + Duel.Turn
                        + " pumpkingSummonEffectUsed="
                        + pumpkingSummonEffectAttempted);
                    return true;
                }

                flyingMaryComebackPumpkingPending = false;
                return Bot.Graveyard.Any(c => IsZombie(c) && c.Level >= 5 && c.IsCanRevive())
                    || Bot.Banished.Any(c => IsZombie(c) && c.Level >= 5);
            }

            if (Card.Location == CardLocation.SpellZone)
            {
                int highestZombieAttack = Bot.GetMonsters()
                    .Where(c => IsZombie(c) && c.Level >= 5)
                    .Select(c => c.Attack)
                    .DefaultIfEmpty(0)
                    .Max();
                return Enemy.GetMonsters().Any(c => c.Attack <= highestZombieAttack && !c.IsShouldNotBeTarget());
            }

            return false;
        }

        private bool VampireSuckerActivate()
        {
            if (WillCardEffectBeNegated())
                return false;


            if (ActivateDescription == Util.GetStringId(CardId.VampireSucker, 0))
            {
                DebugRoute("HOLD Vampire Sucker opponent-GY revive effect");
                return false;
            }

            return true;
        }


        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards,
            int min,
            int max,
            int hint,
            bool cancelable)
        {
            ChainInfo chain = Duel.GetCurrentSolvingChainInfo();
            int activateId = chain != null ? chain.ActivateId : 0;

            bool keyPrompt = pumpkingHandSelectionPending
                || eldlichHandSelectionPending
                || pendingEldlichGraveFieldCost != null
                || samuelReviveSelectionPending
                || reverieOverlaySelectionPending
                || samuelGraveRecycleSelectionPending
                || mezukiReviveSelectionPending
                || flyingMaryEldlichReviveSelectionPending
                || flyingMaryComebackPumpkingPending
                || callReviveSelectionPending
                || pendingInfiniteImpermanenceTarget != null
                || pendingUndyingTarget != null
                || eldlichRouteRank10CommitPending
                || activateId == CardId.PumpkingTheKingOfGraveGhosts
                || IsHublotId(activateId)
                || activateId == CardId.ChangshiTheSpiridao
                || activateId == CardId.OfficiatorOfDoomSamuel
                || activateId == CardId.PumpkingTheGreatGhostKing
                || activateId == CardId.FoolishBurial;
            if (keyPrompt)
            {
                DebugRoute("SELECT prompt activateId=" + activateId
                    + " hint=" + hint + " min=" + min + " max=" + max
                    + " pendingPump=" + pumpkingHandSelectionPending);
                DebugCards("CANDIDATES", cards);
            }


            if (pendingEldlichGraveFieldCost != null
                && hint == HintMsg.ToGrave
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.SpellZone
                    && (c.IsSpell() || c.IsTrap())))
            {
                ClientCard selectedCost = FindMatchingCandidate(
                    cards, pendingEldlichGraveFieldCost);


                if (selectedCost == null
                    || GetEldlichGraveFieldCostScore(selectedCost) == int.MaxValue)
                {
                    selectedCost = cards
                        .Where(c => GetEldlichGraveFieldCostScore(c) < int.MaxValue)
                        .OrderBy(GetEldlichGraveFieldCostScore)
                        .ThenBy(c => c.Sequence)
                        .FirstOrDefault();
                }

                pendingEldlichGraveFieldCost = null;
                if (selectedCost == null)
                {
                    DebugRoute("ERROR Eldlich GY exact cost vanished; no whitelist fallback");
                    return null;
                }

                ClientCard linked = GetCallLinkedMonster(selectedCost);
                DebugRoute("LOCK Eldlich GY cost instance=" + selectedCost.Id
                    + " seq=" + selectedCost.Sequence
                    + " linkedMonster=" + (linked == null ? 0 : linked.Id)
                    + " score=" + GetEldlichGraveFieldCostScore(selectedCost));
                return Util.CheckSelectCount(
                    new List<ClientCard> { selectedCost }, cards, min, max);
            }


            bool reverieOverlayPrompt = reverieOverlaySelectionPending
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.IsOnField()
                    && c.IsFaceup()
                    && IsZombie(c)
                    && c.HasType(CardType.Xyz));
            if (reverieOverlayPrompt)
            {
                IList<ClientCard> reload = SelectReverieOverlayReloadTarget(
                    cards, min, max);
                if (reload != null)
                    return reload;
            }


            if (eldlichRouteRank10CommitPending
                && hint == HintMsg.SpSummon
                && cards != null
                && cards.Count > 0)
            {
                ClientCard mad = cards.FirstOrDefault(c => c != null
                    && c.IsCode(CardId.EldlichTheMadGoldenLord));
                if (mad != null)
                {
                    DebugRoute("Fallen Angel target fallback: Mad Golden for committed Rank 10 line");
                    return Util.CheckSelectCount(
                        new List<ClientCard> { mad }, cards, min, max);
                }
            }


            if (greatPumpkingSearchAttempted && !greatPumpkingSearchResolved
                && hint == HintMsg.AddToHand
                && cards.Any(c => c.Location == CardLocation.Deck))
            {
                return SelectGreatPumpkingSearchTarget(cards, min, max);
            }


            if (hint == HintMsg.RemoveXyz)
                return SelectXyzDetachMaterial(cards, min, max);

            if (callReviveSelectionPending
                && hint == HintMsg.SpSummon
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Grave))
            {
                IList<ClientCard> target = SelectCallReviveTarget(cards, min, max);
                if (target != null)
                    return target;
            }

            if (pendingInfiniteImpermanenceTarget != null
                && hint == HintMsg.Disable)
            {
                IList<ClientCard> target = SelectInfiniteImpermanenceTarget(
                    cards, min, max);
                if (target != null)
                    return target;
            }

            if (samuelGraveRecycleSelectionPending
                && hint == HintMsg.ToDeck
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Location == CardLocation.Grave))
            {
                IList<ClientCard> target = SelectSamuelGraveRecycleTarget(
                    cards, min, max);
                if (target != null)
                    return target;
            }

            if (pendingUndyingTarget != null
                && hint == HintMsg.XyzMaterial)
            {
                IList<ClientCard> target = SelectUndyingReactiveTarget(
                    cards, min, max);
                if (target != null)
                    return target;
            }


            if (samuelReviveSelectionPending
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Grave)
                && (hint == HintMsg.SpSummon
                    || (ashReplayLineActive
                        && cards.Any(c => c.IsCode(CardId.AshBlossom)))))
            {
                IList<ClientCard> target = SelectSamuelReviveTarget(cards, min, max);
                if (target != null)
                {
                    samuelReviveSelectionPending = false;
                    return target;
                }
            }

            if (mezukiReviveSelectionPending
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Grave)
                && hint == HintMsg.SpSummon)
            {
                IList<ClientCard> target = SelectMezukiReviveTarget(cards, min, max);
                mezukiReviveSelectionPending = false;
                if (target != null)
                    return target;
            }


            if (flyingMaryEldlichReviveSelectionPending
                && hint == HintMsg.Target
                && cards != null
                && cards.Count > 0)
            {
                ClientCard eldlich = cards.FirstOrDefault(c => c != null
                    && c.Controller == 0
                    && c.IsCode(CardId.EldlichTheGoldenLord)
                    && (c.Location == CardLocation.Grave
                        || c.Location == CardLocation.Removed));
                if (eldlich != null)
                {
                    flyingMaryEldlichReviveSelectionPending = false;
                    DebugRoute("Flying Mary committed target: Eldlich");
                    return Util.CheckSelectCount(
                        new List<ClientCard> { eldlich }, cards, min, max);
                }

                DebugRoute("ABORT Flying Mary Rank 10 target lock: Eldlich is no longer legal");
                flyingMaryEldlichReviveSelectionPending = false;
            }


            if (flyingMaryComebackPumpkingPending
                && hint == HintMsg.SpSummon
                && cards != null
                && cards.Count > 0)
            {
                ClientCard pumpking = cards.FirstOrDefault(c => c != null
                    && c.Controller == 0
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                if (pumpking != null)
                {
                    flyingMaryComebackPumpkingPending = false;
                    DebugRoute("Flying Mary comeback target: Pumpking");
                    return Util.CheckSelectCount(
                        new List<ClientCard> { pumpking }, cards, min, max);
                }
            }

            if (samuelOpponentNegatePending
                && hint == HintMsg.Disable
                && cards != null
                && cards.Count > 0)
            {
                ClientCard exact = cards.FirstOrDefault(c =>
                    c == samuelNegateTarget
                    || (c != null && samuelNegateTarget != null
                        && c.Controller == samuelNegateTarget.Controller
                        && c.Location == samuelNegateTarget.Location
                        && c.Sequence == samuelNegateTarget.Sequence
                        && c.Id == samuelNegateTarget.Id));
                if (exact != null)
                {
                    samuelOpponentNegatePending = false;
                    DebugRoute("Samuel negate target=" + exact.Id);
                    return Util.CheckSelectCount(
                        new List<ClientCard> { exact }, cards, min, max);
                }
            }

            if (pendingSnakehairDisableTarget != null
                && hint == HintMsg.Disable
                && cards != null
                && cards.Count > 0)
            {
                ClientCard exact = FindMatchingCandidate(
                    cards, pendingSnakehairDisableTarget);
                if (exact != null)
                {
                    DebugRoute("Snakehair disable target=" + exact.Id);
                    pendingSnakehairDisableTarget = null;
                    return Util.CheckSelectCount(
                        new List<ClientCard> { exact }, cards, min, max);
                }
            }

            if (pendingMammothDestroyTarget != null
                && hint == HintMsg.Destroy
                && cards != null
                && cards.Count > 0)
            {
                ClientCard exact = FindMatchingCandidate(
                    cards, pendingMammothDestroyTarget);
                if (exact != null)
                {
                    DebugRoute("Mammoth destroy target=" + exact.Id);
                    pendingMammothDestroyTarget = null;
                    return Util.CheckSelectCount(
                        new List<ClientCard> { exact }, cards, min, max);
                }
            }


            if (pumpkingHandSelectionPending)
            {


                if (!pumpkingCallPromptCompleted
                    && cards.Any(c => c.IsCode(CardId.CallOfTheHaunted))
                    && (hint == HintMsg.Set
                        || cards.All(c => c.Location != CardLocation.Hand)))
                {
                    pumpkingCallPromptCompleted = true;
                    DebugRoute("PENDING Pumpking prompt 1/2: select Call of the Haunted");
                    return SelectByIdPriority(cards, min, max, CardId.CallOfTheHaunted);
                }

                bool handDiscardPrompt = pumpkingCallPromptCompleted
                    && cards.Count > 0
                    && cards.All(c => c.Location == CardLocation.Hand);
                if (handDiscardPrompt
                    || hint == HintMsg.Discard
                    || hint == HintMsg.ToGrave)
                {
                    IList<ClientCard> discard;
                    if (pumpkingDiscardSelfRequired)
                    {
                        ClientCard self = cards.FirstOrDefault(c =>
                            pendingPumpkingHandCard != null && c == pendingPumpkingHandCard);
                        if (self == null)
                        {
                            self = cards.FirstOrDefault(c =>
                                c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                        }
                        if (self != null)
                        {
                            discard = Util.CheckSelectCount(
                                new List<ClientCard> { self }, cards, min, max);
                            DebugRoute("PENDING Pumpking prompt 2/2: forced Pumpking discard; hint=" + hint);
                            pumpkingHandSelectionPending = false;
                            pumpkingCallPromptCompleted = false;
                            pendingPumpkingHandCard = null;
                            return discard;
                        }
                    }

                    discard = SelectPumpkingDiscard(cards, min, max);
                    DebugRoute("PENDING Pumpking prompt 2/2: contextual discard; hint=" + hint);
                    pumpkingHandSelectionPending = false;
                    pumpkingCallPromptCompleted = false;
                    pendingPumpkingHandCard = null;
                    return discard;
                }
            }


            if (greatPumpkingBounceAttempted
                && !greatPumpkingBounceResolved
                && hint == HintMsg.ReturnToHand)
            {
                return SelectGreatPumpkingBounceTargets(cards, min, max);
            }


            if (eldlichHandSelectionPending && hint == HintMsg.ToGrave)
            {
                bool allOurHand = cards != null
                    && cards.Count > 0
                    && cards.All(c => c != null
                        && c.Controller == 0
                        && c.Location == CardLocation.Hand);
                if (!eldlichHandCostPromptCompleted && allOurHand)
                {
                    IList<ClientCard> cost = SelectEldlichHandSpellTrapCost(cards, min, max);
                    if (cost != null)
                    {
                        eldlichHandCostPromptCompleted = true;
                        return cost;
                    }
                }

                bool fieldTargetPrompt = eldlichHandCostPromptCompleted
                    && cards != null
                    && cards.Any(c => c != null && c.IsOnField());
                if (fieldTargetPrompt)
                {
                    IList<ClientCard> enemyTarget = SelectEldlichHandFieldTarget(cards, min, max);
                    if (enemyTarget != null)
                    {
                        DebugRoute("ELDLICH HAND TARGET priority enemy="
                            + string.Join(",", enemyTarget.Select(c => c.Id.ToString()).ToArray()));
                        eldlichHandSelectionPending = false;
                        eldlichHandCostPromptCompleted = false;
                        return enemyTarget;
                    }

                    DebugRoute("ERROR Eldlich target prompt contained no enemy card; refusing self target");
                    eldlichHandSelectionPending = false;
                    eldlichHandCostPromptCompleted = false;
                    return null;
                }
            }

            bool varudrasFieldDestroyPrompt = cards != null
                && cards.Count > 0
                && cards.All(c => c != null && c.IsOnField())
                && (pendingVarudrasDestroyMode != VarudrasDestroyMode.None
                    || (activateId == CardId.Varudras
                        && hint == HintMsg.Destroy));
            if (varudrasFieldDestroyPrompt)
            {
                return SelectVarudrasDestroyTarget(cards, min, max);
            }

            if (IsHublotId(activateId))
            {
                if (hint == HintMsg.ToGrave)
                {
                    selectedHublotSendId = GetHublotSendTargetId(cards);
                    selectedHublotRecoverId = 0;
                    if (changshiHandRescueRouteActive
                        && selectedHublotSendId == CardId.OfficiatingReverie)
                    {
                        selectedHublotRecoverId = CardId.OfficiatingReverie;
                        DebugRoute("Hublot rescue recover: Reverie to discard Changshi");
                    }
                    else if (CanRecoverPumpkingFromHublotNow()
                        && (HasPumpkingInGrave()
                            || selectedHublotSendId == CardId.PumpkingTheKingOfGraveGhosts))
                    {
                        selectedHublotRecoverId = CardId.PumpkingTheKingOfGraveGhosts;
                    }
                    else if ((HasPumpkingInGrave()
                            || selectedHublotSendId == CardId.PumpkingTheKingOfGraveGhosts)
                        && !HasPumpkingInHand())
                    {
                        DebugRoute("Hublot skip Pumpking recovery: no executable Call line");
                    }
                    else if (selectedHublotSendId == CardId.EldlichTheGoldenLord
                        && ShouldRecoverEldlichFromHublot())
                    {
                        selectedHublotRecoverId = CardId.EldlichTheGoldenLord;
                    }
                    selectedHublotRecover = selectedHublotRecoverId != 0;
                    DebugRoute("Hublot send=" + selectedHublotSendId
                        + " recover=" + selectedHublotRecoverId);
                    return SelectByIdPriority(cards, min, max, selectedHublotSendId);
                }

                if (hint == HintMsg.AddToHand)
                {
                    return SelectByIdPriority(cards, min, max,
                        selectedHublotRecoverId,
                        CardId.PumpkingTheKingOfGraveGhosts,
                        CardId.EldlichTheGoldenLord,
                        selectedHublotSendId);
                }

                if (hint == HintMsg.SpSummon)
                {
                    if (changshiHandRescueRouteActive
                        && Bot.HasInGraveyard(CardId.ChangshiTheSpiridao)
                        && !HasGreatPumpkingOnField())
                    {
                        DebugRoute("Hublot rescue Xyz target: Great Pumpking first");
                        return SelectHublotXyzTarget(cards, min, max,
                            CardId.PumpkingTheGreatGhostKing,
                            CardId.OfficiatorOfDoomSamuel);
                    }

                    if (IsVampireGraceRouteLive()
                        && !HasGreatPumpkingOnField())
                    {
                        DebugRoute("Hublot Grace Xyz target: Great Pumpking first to release Call");
                        return SelectHublotXyzTarget(cards, min, max,
                            CardId.PumpkingTheGreatGhostKing,
                            CardId.OfficiatorOfDoomSamuel);
                    }

                    if (Duel.Player == 1)
                    {
                        if (!Bot.HasInMonstersZone(CardId.OfficiatorOfDoomSamuel, faceUp: true))
                        {
                            return SelectHublotXyzTarget(cards, min, max,
                                CardId.OfficiatorOfDoomSamuel,
                                HasFaceupCall() ? CardId.PumpkingTheGreatGhostKing : 0,
                                CardId.WollowFounderOfTheDrudgeDragons);
                        }
                        if (HasFaceupCall())
                        {
                            return SelectHublotXyzTarget(cards, min, max,
                                CardId.PumpkingTheGreatGhostKing,
                                CardId.WollowFounderOfTheDrudgeDragons);
                        }
                    }

                    if (samuelReviveResolved)
                    {
                        return SelectHublotXyzTarget(cards, min, max,
                            CardId.PumpkingTheGreatGhostKing,
                            CardId.OfficiatorOfDoomSamuel,
                            CardId.DhampirVampireSheridan,
                            CardId.WollowFounderOfTheDrudgeDragons);
                    }

                    return SelectHublotXyzTarget(cards, min, max,
                        CardId.OfficiatorOfDoomSamuel,
                        CardId.PumpkingTheGreatGhostKing,
                        CardId.DhampirVampireSheridan,
                        CardId.WollowFounderOfTheDrudgeDragons);
                }
            }

            switch (activateId)
            {
                case CardId.PumpkingTheKingOfGraveGhosts:
                    if (hint == HintMsg.Set)
                        return SelectByIdPriority(cards, min, max, CardId.CallOfTheHaunted);
                    if (hint == HintMsg.Discard || hint == HintMsg.ToGrave)
                        return SelectPumpkingDiscard(cards, min, max);
                    if (hint == HintMsg.SpSummon)
                        return SelectPumpkingDeckSummonTarget(cards, min, max);
                    break;

                case CardId.StareOfTheSnakeHair:
                    if (hint == HintMsg.AddToHand)
                    {


                        DebugRoute("Snakehair search target: Ectoplasmic first");
                        return SelectByIdPriority(cards, min, max,
                            CardId.EctoplasmicFortification,
                            CardId.CallOfTheHaunted,
                            CardId.VortexOfTime);
                    }
                    if (hint == HintMsg.Disable
                        && pendingSnakehairDisableTarget != null)
                    {
                        ClientCard exact = FindMatchingCandidate(
                            cards, pendingSnakehairDisableTarget);
                        if (exact != null)
                        {
                            pendingSnakehairDisableTarget = null;
                            return Util.CheckSelectCount(
                                new List<ClientCard> { exact }, cards, min, max);
                        }
                    }
                    return SelectEnemyField(cards, min, max);

                case CardId.EctoplasmicFortification:
                    if (hint == HintMsg.AddToHand)
                    {
                        int searchTarget = GetEctoplasmicSearchTargetId();
                        if (searchTarget == CardId.Hublot)
                        {
                            DebugRoute("Ectoplasmic search target: Hublot (no Hublot in hand)");
                            return SelectByIdPriority(cards, min, max,
                                CardId.Hublot,
                                CardId.PumpkingTheKingOfGraveGhosts,
                                CardId.ArmyOfTheHaunted,
                                CardId.ChangshiTheSpiridao,
                                CardId.OfficiatingReverie);
                        }

                        DebugRoute("Ectoplasmic search target: Pumpking (Hublot already in hand)");
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.Hublot,
                            CardId.ArmyOfTheHaunted,
                            CardId.ChangshiTheSpiridao,
                            CardId.OfficiatingReverie);
                    }
                    break;

                case CardId.DeltaOfInvitation:
                    if (hint == HintMsg.ToGrave)
                    {
                        if (CanStartEldlichRoute()
                            && cards.Any(c => c.IsCode(CardId.EldlichTheGoldenLord)))
                        {
                            selectedDeltaSendId = CardId.EldlichTheGoldenLord;
                            return SelectByIdPriority(cards, min, max,
                                CardId.EldlichTheGoldenLord);
                        }


                        selectedDeltaSendId = cards.Any(c => c.IsCode(CardId.DoomkingBalerdroch))
                            ? CardId.DoomkingBalerdroch
                            : CardId.EldlichTheGoldenLord;
                        return SelectByIdPriority(cards, min, max,
                            selectedDeltaSendId);
                    }
                    break;

                case CardId.FoolishBurial:
                    if (hint == HintMsg.ToGrave)
                    {
                        int plannedTarget = selectedFoolishBurialSendId;
                        if (plannedTarget == 0
                            || !cards.Any(c => c != null && c.IsCode(plannedTarget)))
                        {
                            plannedTarget = GetFoolishBurialPlannedTargetId(cards);
                        }

                        if (plannedTarget == 0)
                        {
                            DebugRoute("ERROR Foolish prompt lost its profitable planned target");
                            return null;
                        }

                        selectedFoolishBurialSendId = plannedTarget;
                        DebugRoute("Foolish committed target=" + plannedTarget
                            + " reason=" + GetFoolishBurialPlanReason(plannedTarget));
                        return SelectByIdPriority(cards, min, max, plannedTarget);
                    }
                    break;

                case CardId.OfficiatingReverie:
                    if (hint == HintMsg.Discard || hint == HintMsg.ToGrave)
                        return SelectReverieDiscard(cards, min, max);
                    if (hint == HintMsg.SpSummon)
                        return SelectReverieReviveTarget(cards, min, max);
                    if (hint == HintMsg.XyzMaterial)
                        return SelectReverieOverlayReloadTarget(cards, min, max);
                    break;

                case CardId.ArmyOfTheHaunted:
                    if (hint == HintMsg.Set)
                        return SelectByIdPriority(cards, min, max, CardId.CallOfTheHaunted);
                    break;

                case CardId.CallOfTheHaunted:
                    if (hint == HintMsg.SpSummon)
                        return SelectCallReviveTarget(cards, min, max);
                    break;

                case CardId.Mezuki:
                    if (hint == HintMsg.SpSummon)
                        return SelectMezukiReviveTarget(cards, min, max);
                    break;


                case CardId.ChangshiTheSpiridao:
                    if (hint == HintMsg.ToGrave)
                        return SelectChangshiDeckTarget(cards, min, max);
                    if (hint == HintMsg.Remove)
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.VampireGrace,
                            CardId.Mezuki,
                            CardId.ArmyOfTheHaunted,
                            CardId.GreatMammothOfTheNetherworld,
                            CardId.StareOfTheSnakeHair,
                            CardId.DoomkingBalerdroch,
                            CardId.PumpkingTheKingOfGraveGhosts);
                    }
                    break;

                case CardId.GreatMammothOfTheNetherworld:
                    if (hint == HintMsg.Destroy)
                    {
                        if (pendingMammothDestroyTarget != null)
                        {
                            ClientCard exact = FindMatchingCandidate(
                                cards, pendingMammothDestroyTarget);
                            if (exact != null)
                            {
                                pendingMammothDestroyTarget = null;
                                return Util.CheckSelectCount(
                                    new List<ClientCard> { exact }, cards, min, max);
                            }
                        }
                        return SelectEnemyField(cards, min, max);
                    }
                    break;

                case CardId.EldlichTheGoldenLord:
                    if (hint == HintMsg.SpSummon)
                    {
                        DebugRoute("Eldlich GY effect summon target: Eldlich");
                        return SelectByIdPriority(cards, min, max,
                            CardId.EldlichTheGoldenLord,
                            CardId.EldlichTheMadGoldenLord,
                            CardId.DoomkingBalerdroch);
                    }
                    if (hint == HintMsg.ToGrave)
                    {
                        if (chain != null && chain.ActivateLocation == CardLocation.Hand)
                        {
                            bool handCostPrompt = cards.All(c => c.Location == CardLocation.Hand);
                            if (handCostPrompt)
                            {
                                List<ClientCard> handCosts = cards
                                    .Where(c => c.Controller == 0 && (c.IsSpell() || c.IsTrap()))
                                    .OrderBy(c => c.IsCode(CardId.FoolishBurial) ? 0 : 1)
                                    .ThenBy(c => c.IsCode(CardId.Terraforming) ? 0 : 1)
                                    .ThenBy(c => c.IsCode(CardId.DeltaOfInvitation) ? 1 : 2)
                                    .ThenBy(c => c.IsCode(CardId.CallOfTheHaunted) ? 10 : 1)
                                    .ToList();
                                return Util.CheckSelectCount(handCosts, cards, min, max);
                            }
                            return SelectEnemyField(cards, min, max);
                        }


                        ClientCard selectedCost = FindMatchingCandidate(
                            cards, pendingEldlichGraveFieldCost);
                        if (selectedCost == null
                            || GetEldlichGraveFieldCostScore(selectedCost) == int.MaxValue)
                        {
                            selectedCost = cards
                                .Where(c => GetEldlichGraveFieldCostScore(c) < int.MaxValue)
                                .OrderBy(GetEldlichGraveFieldCostScore)
                                .ThenBy(c => c.Sequence)
                                .FirstOrDefault();
                        }
                        pendingEldlichGraveFieldCost = null;
                        if (selectedCost == null)
                        {
                            DebugRoute("ERROR Eldlich GY cost prompt has no committed safe instance");
                            return null;
                        }

                        ClientCard linked = GetCallLinkedMonster(selectedCost);
                        DebugRoute("Eldlich GY cost instance=" + selectedCost.Id
                            + " seq=" + selectedCost.Sequence
                            + " linkedMonster=" + (linked == null ? 0 : linked.Id)
                            + " score=" + GetEldlichGraveFieldCostScore(selectedCost));
                        return Util.CheckSelectCount(
                            new List<ClientCard> { selectedCost }, cards, min, max);
                    }
                    break;

                case CardId.DoomkingBalerdroch:
                    if (hint == HintMsg.Remove)
                    {
                        List<ClientCard> field = GetEnemyFieldPriority(cards).Where(cards.Contains).ToList();
                        if (field.Count > 0)
                            return Util.CheckSelectCount(field, cards, min, max);
                        return SelectEnemyGrave(cards, min, max);
                    }
                    break;

                case CardId.PumpkingTheGreatGhostKing:
                    if (hint == HintMsg.AddToHand)
                        return SelectGreatPumpkingSearchTarget(cards, min, max);
                    if (hint == HintMsg.ReturnToHand)
                        return SelectGreatPumpkingBounceTargets(cards, min, max);
                    break;

                case CardId.DhampirVampireSheridan:
                    if (hint == HintMsg.ToGrave)
                        return SelectEnemyField(cards, min, max);
                    if (hint == HintMsg.SpSummon)
                        return SelectEnemyGrave(cards, min, max);
                    break;

                case CardId.OfficiatorOfDoomSamuel:
                    if (hint == HintMsg.SpSummon)
                        return SelectSamuelReviveTarget(cards, min, max);
                    if (hint == HintMsg.Disable)
                    {
                        if (samuelOpponentNegatePending && samuelNegateTarget != null)
                        {
                            ClientCard exact = FindMatchingCandidate(
                                cards, samuelNegateTarget);
                            if (exact != null)
                            {
                                samuelOpponentNegatePending = false;
                                samuelNegateTarget = null;
                                DebugRoute("Samuel negate target=" + exact.Id);
                                return Util.CheckSelectCount(
                                    new List<ClientCard> { exact }, cards, min, max);
                            }
                        }


                        List<ClientCard> meaningful = cards
                            .Where(c => IsWorthDisablingWithSamuel(c, false))
                            .OrderByDescending(c => ScoreSamuelDisableTarget(c, null))
                            .ToList();
                        samuelOpponentNegatePending = false;
                        samuelNegateTarget = null;
                        if (meaningful.Count > 0)
                        {
                            DebugRoute("Samuel negate fallback target="
                                + meaningful[0].Id);
                            return Util.CheckSelectCount(
                                meaningful, cards, min, max);
                        }

                        DebugRoute("ERROR Samuel disable prompt lost planned meaningful target");
                        return null;
                    }
                    if (cards.All(c => c.Location == CardLocation.Grave))
                        return SelectSamuelGraveRecycleTarget(cards, min, max);
                    break;

                case CardId.WollowFounderOfTheDrudgeDragons:
                    if (cards.Any(c => c.Location == CardLocation.Grave))
                        return SelectEnemyGrave(cards, min, max);
                    return SelectEnemyField(cards, min, max);

                case CardId.TheUndyingLegion:
                    return SelectUndyingReactiveTarget(cards, min, max);

                case CardId.EvolzarLars:
                    return SelectEnemyField(cards, min, max);


                case CardId.FallenAngelOfTheGoldenLand:
                    if (hint == HintMsg.SpSummon)
                    {
                        ClientCard mad = cards.FirstOrDefault(c => c != null
                            && c.IsCode(CardId.EldlichTheMadGoldenLord));
                        if (mad != null)
                        {
                            DebugRoute("Fallen Angel target: Mad Golden for committed Rank 10 line");
                            return Util.CheckSelectCount(
                                new List<ClientCard> { mad }, cards, min, max);
                        }
                        if (eldlichRouteRank10CommitPending)
                        {
                            DebugRoute("ABORT Flying Mary Rank 10 commit: Mad Golden is not a legal Fallen Angel target");
                            eldlichRouteRank10CommitPending = false;
                        }
                        return SelectByIdPriority(cards, min, max,
                            CardId.EldlichTheGoldenLord,
                            CardId.FallenAngelOfTheGoldenLand);
                    }
                    break;

                case CardId.EldlichTheMadGoldenLord:
                    if (hint == HintMsg.Release)
                    {
                        List<ClientCard> tribute = cards.Where(c => c.Controller == 0 && IsZombie(c) && !c.IsCode(CardId.EldlichTheMadGoldenLord))
                            .OrderBy(GetMaterialValue).ToList();
                        return Util.CheckSelectCount(tribute, cards, min, max);
                    }
                    ClientCard safeMadGoldenTarget = GetMadGoldenSafeControlTarget(cards);
                    if (safeMadGoldenTarget == null)
                    {
                        DebugRoute("BLOCK Mad Golden target: no monster will receive the control effect");
                        return null;
                    }
                    return Util.CheckSelectCount(
                        new List<ClientCard> { safeMadGoldenTarget }, cards, min, max);

                case CardId.FlyingMary:
                    if (hint == HintMsg.Target)
                    {
                        if (eldlichRouteRank10CommitPending
                            && cards.Any(c => c != null
                                && c.IsCode(CardId.EldlichTheGoldenLord)))
                        {
                            flyingMaryEldlichReviveSelectionPending = false;
                            DebugRoute("Flying Mary target priority: committed Eldlich");
                            return SelectByIdPriority(cards, min, max,
                                CardId.EldlichTheGoldenLord,
                                CardId.PumpkingTheKingOfGraveGhosts,
                                CardId.DoomkingBalerdroch);
                        }

                        if (ShouldFlyingMaryRevivePumpkingForComeback()
                            && cards.Any(c => c != null
                                && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)))
                        {
                            flyingMaryComebackPumpkingPending = false;
                            DebugRoute("Flying Mary target priority: Pumpking comeback");
                            return SelectByIdPriority(cards, min, max,
                                CardId.PumpkingTheKingOfGraveGhosts,
                                CardId.EldlichTheGoldenLord,
                                CardId.DoomkingBalerdroch);
                        }

                        if (eldlichRouteActive || eldlichRouteMarySummoned)
                        {
                            return SelectByIdPriority(cards, min, max,
                                CardId.EldlichTheGoldenLord,
                                CardId.PumpkingTheKingOfGraveGhosts);
                        }
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.EldlichTheGoldenLord,
                            CardId.DoomkingBalerdroch);
                    }
                    if (hint == HintMsg.Destroy)
                        return SelectEnemyField(cards, min, max);
                    break;

                case CardId.VampireSucker:
                    if (hint == HintMsg.SpSummon)
                        return SelectEnemyGrave(cards, min, max);
                    break;

                case CardId.InfiniteImpermanence:
                    return SelectInfiniteImpermanenceTarget(cards, min, max);
            }


            if (IsHostileTargetHint(hint)
                && cards != null
                && cards.Any(c => c != null && c.Controller == 1))
            {
                IList<ClientCard> enemyTarget = SelectStrictEnemyTarget(cards, min, max);
                if (enemyTarget != null)
                {
                    DebugRoute("GENERIC ENEMY TARGET SAFETY hint=" + hint
                        + " selected="
                        + string.Join(",", enemyTarget.Select(c => c.Id.ToString()).ToArray()));
                    return enemyTarget;
                }
            }


            if (!pumpkingHandSelectionPending
                && (hint == HintMsg.ToDeck
                    || hint == HintMsg.ToGrave
                    || hint == HintMsg.Discard)
                && cards != null
                && cards.Count > 0
                && cards.All(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.Hand))
            {
                IList<ClientCard> genericHandSelection =
                    SelectGenericHandDisposition(cards, min, max, hint);
                if (genericHandSelection != null)
                    return genericHandSelection;
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(
            IList<ClientCard> cards, int min, int max)
        {
            if (selectedHublotXyzId != 0)
            {
                if ((changshiHandRescueRouteActive || IsVampireGraceRouteLive())
                    && (selectedHublotXyzId == CardId.PumpkingTheGreatGhostKing
                        || selectedHublotXyzId == CardId.OfficiatorOfDoomSamuel))
                {
                    List<ClientCard> roleMaterials =
                        GetChangshiRescueRank6Materials(cards);
                    if (roleMaterials.Count == 2)
                    {
                        DebugRoute("XYZ rescue materials: "
                            + (GetCallLinkedLevel6(roleMaterials) != null
                                ? "linked Level 6 + Level 6"
                                : "Level 6 + Level 6"));
                        return Util.CheckSelectCount(roleMaterials, cards, min, max);
                    }
                }

                List<ClientCard> selected = new List<ClientCard>();
                ClientCard hublot = cards.FirstOrDefault(IsHublot);

                if (selectedHublotXyzId == CardId.OfficiatorOfDoomSamuel)
                {


                    ClientCard pumpking = cards.FirstOrDefault(c =>
                        c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
                    if (pumpking != null)
                        selected.Add(pumpking);
                    if (hublot != null && !selected.Contains(hublot))
                        selected.Add(hublot);
                }
                else if (selectedHublotXyzId == CardId.PumpkingTheGreatGhostKing)
                {
                    if (hublot != null)
                        selected.Add(hublot);

                    ClientCard second = cards
                        .Where(c => c != hublot)
                        .OrderBy(c => c.IsCode(samuelRevivedCardId) ? 0 : 1)
                        .ThenBy(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                            && HasPumpkingInGrave() ? 100 : GetMaterialValue(c))
                        .ThenBy(c => c.Attack)
                        .FirstOrDefault();
                    if (second != null)
                        selected.Add(second);
                }
                else
                {
                    if (hublot != null)
                        selected.Add(hublot);
                    selected.AddRange(cards
                        .Where(c => !selected.Contains(c))
                        .OrderBy(GetMaterialValue)
                        .ThenBy(c => c.Attack)
                        .Take(Math.Max(0, max - selected.Count)));
                }

                if (selected.Count < max)
                {
                    selected.AddRange(cards
                        .Where(c => !selected.Contains(c))
                        .OrderBy(GetMaterialValue)
                        .ThenBy(c => c.Attack)
                        .Take(max - selected.Count));
                }

                return Util.CheckSelectCount(selected, cards, min, max);
            }

            return base.OnSelectXyzMaterial(cards, min, max);
        }

        public override IList<ClientCard> OnSelectLinkMaterial(
            IList<ClientCard> cards, int min, int max)
        {
            ClientCard liveUndying = cards.FirstOrDefault(c =>
                c.IsCode(CardId.TheUndyingLegion)
                && c.Overlays != null && c.Overlays.Count > 0);
            if (liveUndying != null)
            {
                List<ClientCard> safeMaterials = cards
                    .Where(c => c != liveUndying)
                    .OrderBy(GetZombieLinkMaterialValue)
                    .ThenBy(GetMaterialValue)
                    .ThenBy(c => c.Attack)
                    .Take(max)
                    .ToList();
                if (safeMaterials.Count >= min)
                {
                    DebugRoute("LINK MATERIAL SAFETY: preserve live-material Undying");
                    return Util.CheckSelectCount(safeMaterials, cards, min, max);
                }
            }

            return base.OnSelectLinkMaterial(cards, min, max);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override int OnSelectOption(IList<int> options)
        {


            int hublotNoTribute = options.IndexOf(
                Util.GetStringId(CardId.Hublot, 0));
            if (Duel.Player == 0 && HasHublotInHand() && hublotNoTribute >= 0)
            {
                DebugRoute("HUBLOT SUMMON OPTION: choose no-Tribute procedure index="
                    + hublotNoTribute);
                return hublotNoTribute;
            }

            ChainInfo chain = Duel.GetCurrentSolvingChainInfo();
            if (doomkingOptionPending
                || (chain != null && chain.ActivateController == 0))
            {
                if (doomkingOptionPending
                    || (chain != null && chain.ActivateId == CardId.DoomkingBalerdroch))
                {


                    int negate = options.IndexOf(Util.GetStringId(CardId.DoomkingBalerdroch, 1));
                    int banish = options.IndexOf(Util.GetStringId(CardId.DoomkingBalerdroch, 2));
                    DebugRoute("DOOMKING OPTION preferNegate=" + doomkingPreferNegate
                        + " negateIndex=" + negate + " banishIndex=" + banish
                        + " optionCount=" + options.Count);

                    int selected = -1;
                    if (doomkingPreferNegate)
                    {
                        selected = negate >= 0 ? negate : (options.Count > 0 ? 0 : -1);
                    }
                    else
                    {
                        selected = banish >= 0 ? banish
                            : (options.Count > 1 ? 1 : (options.Count > 0 ? 0 : -1));
                    }

                    doomkingOptionPending = false;
                    doomkingPreferNegate = false;
                    if (selected >= 0)
                        return selected;
                }

                if (chain != null && chain.ActivateId == CardId.EctoplasmicFortification)
                {
                    int search = options.IndexOf(Util.GetStringId(CardId.EctoplasmicFortification, 1));
                    int boost = options.IndexOf(Util.GetStringId(CardId.EctoplasmicFortification, 2));
                    if (Bot.GetMonsterCount() == 0 && search >= 0)
                        return search;
                    if (boost >= 0)
                        return boost;
                    if (search >= 0)
                        return search;
                    if (options.Count > 0)
                        return 0;
                }
            }

            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.DeltaOfInvitation, 3))
            {
                return Bot.HasInDeck(CardId.EldlichTheGoldenLord)
                    || Bot.HasInDeck(CardId.DoomkingBalerdroch);
            }

            if (desc == Util.GetStringId(CardId.Hublot, 3))
            {
                return selectedHublotRecover;
            }

            if (desc == Util.GetStringId(CardId.OfficiatorOfDoomSamuel, 2))
            {
                bool negate = samuelOpponentNegatePending
                    && samuelNegateTarget != null
                    && samuelNegateTarget.Controller == 1
                    && samuelNegateTarget.Location == CardLocation.MonsterZone
                    && samuelNegateTarget.IsFaceup()
                    && !samuelNegateTarget.IsDisabled();
                if (!negate && Duel.Player == 0)
                {


                    ClientCard ownTurnTarget = Enemy.GetMonsters()
                        .Where(c => c != null
                            && c.IsFaceup()
                            && !c.IsDisabled()
                            && IsWorthDisablingWithSamuel(c, false))
                        .OrderByDescending(c => ScoreSamuelDisableTarget(c, null))
                        .FirstOrDefault();
                    samuelNegateTarget = ownTurnTarget;
                    negate = ownTurnTarget != null;
                }
                DebugRoute("Samuel optional negate=" + negate
                    + " target=" + (samuelNegateTarget != null
                        ? samuelNegateTarget.Id.ToString() : "0"));
                return negate;
            }

            if (desc == Util.GetStringId(CardId.Varudras, 3))
            {
                bool acceptDestroy = PlanVarudrasPostNegateDestroy();
                DebugRoute("VARUDRAS post-negate destroy=" + acceptDestroy);
                return acceptDestroy;
            }

            return base.OnSelectYesNo(desc);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                List<int> order = new List<int>();

                if (cardId == CardId.TheUndyingLegion)
                {
                    if (ShouldContinueEldlichLine())
                        order.AddRange(new[] { 0, 2, 4, 1, 3, 5, 6 });
                    else
                        order.AddRange(new[] { 5, 6, 0, 2, 4, 1, 3 });
                }
                else
                {
                    order.AddRange(new[] { 0, 2, 4, 1, 3, 5, 6 });
                }

                foreach (int zoneId in order)
                {
                    int zone = 1 << zoneId;
                    if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                        return zone;
                }
            }

            return base.OnSelectPlace(cardId, player, location, available);
        }

        private bool BattlePhaseIsAvailableThisTurn()
        {


            return Duel.Player == 0
                && Duel.Turn > 1
                && Duel.Phase == DuelPhase.Main1;
        }

        private bool ShouldUseSamuelAttackForConfirmedLethal(
            ClientCard existingSamuel = null)
        {
            if (!BattlePhaseIsAvailableThisTurn()
                || Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
            {
                return false;
            }

            int samuelAttack = existingSamuel != null
                ? existingSamuel.Attack
                : (NamedCard.Get(CardId.OfficiatorOfDoomSamuel) == null
                    ? 0
                    : NamedCard.Get(CardId.OfficiatorOfDoomSamuel).Attack);
            int otherDamage = Bot.GetMonsters()
                .Where(c => c != null
                    && c != existingSamuel
                    && !c.IsCode(CardId.OfficiatorOfDoomSamuel))
                .Sum(GetPotentialAttackContribution);


            return otherDamage < Enemy.LifePoints
                && otherDamage + samuelAttack >= Enemy.LifePoints;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            bool canAttack = positions.Contains(CardPosition.FaceUpAttack);
            bool canDefend = positions.Contains(CardPosition.FaceUpDefence);
            if (!canAttack || !canDefend)
                return base.OnSelectPosition(cardId, positions);

            NamedCard data = NamedCard.Get(cardId);


            if (cardId == CardId.TheUndyingLegion)
            {
                DebugRoute("POSITION Undying: Attack; DEF is 0");
                return CardPosition.FaceUpAttack;
            }


            if (cardId == CardId.OfficiatorOfDoomSamuel)
            {
                if (ShouldUseSamuelAttackForConfirmedLethal())
                {
                    DebugRoute("POSITION Samuel: Attack for confirmed lethal");
                    return CardPosition.FaceUpAttack;
                }

                DebugRoute("POSITION Samuel: Defence; preserve interaction body");
                return CardPosition.FaceUpDefence;
            }


            if (Duel.Player == 1)
            {
                DebugRoute("POSITION " + cardId + ": Defence on opponent turn");
                return CardPosition.FaceUpDefence;
            }


            if (Duel.Turn == 1)
            {
                DebugRoute("POSITION " + cardId + ": Defence on first turn; no Battle Phase");
                return CardPosition.FaceUpDefence;
            }


            if (cardId == CardId.Varudras)
            {
                if (BattlePhaseIsAvailableThisTurn())
                {
                    DebugRoute("POSITION Varudras: Attack; Battle Phase available");
                    return CardPosition.FaceUpAttack;
                }

                DebugRoute("POSITION Varudras: Defence; no Battle Phase available");
                return CardPosition.FaceUpDefence;
            }


            if (BattlePhaseIsAvailableThisTurn()
                && data != null
                && data.Attack > 0)
            {
                return CardPosition.FaceUpAttack;
            }


            if (Duel.Phase >= DuelPhase.Main2
                || data == null
                || data.Defense >= data.Attack)
            {
                return CardPosition.FaceUpDefence;
            }

            return CardPosition.FaceUpAttack;
        }

        private bool MonsterRepos()
        {
            if (Card == null || !Card.IsMonster() || Duel.Player != 0)
                return false;


            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {


                if (Card.IsCode(CardId.OfficiatorOfDoomSamuel))
                {
                    if (Card.IsDefense()
                        && ShouldUseSamuelAttackForConfirmedLethal(Card))
                    {
                        DebugRoute("REPOS Samuel: Defence -> Attack for confirmed lethal");
                        return true;
                    }
                    return false;
                }


                if (Card.IsCode(CardId.Varudras))
                {
                    if (Card.IsDefense())
                    {
                        DebugRoute("REPOS Varudras: Defence -> Attack before Battle Phase");
                        return true;
                    }
                    return false;
                }


                return Card.IsDefense() && Card.Attack > 0;
            }


            if (Duel.Phase >= DuelPhase.Main2
                && !Card.IsCode(CardId.Varudras))
            {
                if (Card.IsCode(CardId.OfficiatorOfDoomSamuel))
                    return Card.IsAttack();
                return Card.IsAttack() && Card.Defense > Card.Attack;
            }

            return false;
        }

        private bool SpellSet()
        {
            if (Card == null || !HasOpenSpellZone())
                return false;

            if (Card.IsTrap())
            {


                SelectSTPlace(Card, true);
                return true;
            }

            if (Card.HasType(CardType.QuickPlay))
            {
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }


        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null)
            {
                bool movedToOurMonsterZone = currentControler == 0
                    && (currentLocation & (int)CardLocation.MonsterZone) != 0;
                bool movedToOurSpellZone = currentControler == 0
                    && (currentLocation & (int)CardLocation.SpellZone) != 0;
                bool movedFromOurSpellZone = previousControler == 0
                    && (previousLocation & (int)CardLocation.SpellZone) != 0;
                bool movedFromOurHand = previousControler == 0
                    && (previousLocation & (int)CardLocation.Hand) != 0;
                bool movedToOurHand = currentControler == 0
                    && (currentLocation & (int)CardLocation.Hand) != 0;
                bool movedToEnemyMonsterZone = currentControler == 1
                    && (currentLocation & (int)CardLocation.MonsterZone) != 0;
                bool movedToEnemySpellZone = currentControler == 1
                    && (currentLocation & (int)CardLocation.SpellZone) != 0;

                bool remainsOnEnemyFieldForSnapshot = currentControler == 1
                    && ((currentLocation & (int)CardLocation.MonsterZone) != 0
                        || (currentLocation & (int)CardLocation.SpellZone) != 0);
                if (!remainsOnEnemyFieldForSnapshot || !card.IsFaceup())
                {
                    opponentFaceupBeforeCurrentChain.RemoveAll(c =>
                        MatchesCard(c, card));
                }
                else if (movedToEnemyMonsterZone)
                {


                    TrackOpponentFaceupMonsterAtOpenState(card);
                }

                if (card.IsCode(CardId.CallOfTheHaunted))
                {
                    if (movedToOurSpellZone
                        && card.IsFacedown()
                        && pumpkingHandSelectionPending)
                    {
                        freshSetCallByPumpkingInstance = card;
                    }
                    else if (MatchesCard(card, freshSetCallByPumpkingInstance)
                        && (!movedToOurSpellZone || !card.IsFacedown()))
                    {
                        freshSetCallByPumpkingInstance = null;
                    }
                }

                if (card.IsCode(CardId.DeltaOfInvitation))
                {
                    if (movedToOurSpellZone && !movedFromOurSpellZone)
                        spentDeltaFieldInstance = null;
                    else if (movedFromOurSpellZone && !movedToOurSpellZone)
                    {
                        if (MatchesCard(card, spentDeltaFieldInstance))
                            spentDeltaFieldInstance = null;
                        if (MatchesCard(card, pendingDeltaTokenFieldInstance))
                            pendingDeltaTokenFieldInstance = null;
                    }
                }

                if (movedToOurMonsterZone && IsDeltaToken(card))
                {
                    spentDeltaFieldInstance = Bot.GetSpells().FirstOrDefault(c => c != null
                        && c.IsCode(CardId.DeltaOfInvitation)
                        && c.IsFaceup());
                    if (spentDeltaFieldInstance != null)
                    {
                        DebugRoute("MARK Delta field spent after Token instance seq="
                            + spentDeltaFieldInstance.Sequence);
                    }
                }


                if (Duel.Player == 1 && movedToEnemyMonsterZone && card.IsFaceup())
                {
                    freshEnemyMonster = card;
                    freshEnemyFaceupCard = card;
                    enemyCommitmentWindow = true;
                    enemyCommitmentTurn = Duel.Turn;
                    DebugRoute("COMMITMENT enemy monster=" + card.Id
                        + " atk=" + card.Attack);
                    RecalculateStrategicPlan("enemy monster entered field");
                }
                else if (Duel.Player == 1 && movedToEnemySpellZone && card.IsFaceup())
                {
                    freshEnemyFaceupCard = card;
                    enemyCommitmentWindow = true;
                    enemyCommitmentTurn = Duel.Turn;
                    DebugRoute("COMMITMENT enemy face-up S/T=" + card.Id);
                    RecalculateStrategicPlan("enemy face-up S/T entered field");
                }

                bool remainsOnEnemyField = currentControler == 1
                    && ((currentLocation & (int)CardLocation.MonsterZone) != 0
                        || (currentLocation & (int)CardLocation.SpellZone) != 0);
                if ((MatchesCard(card, freshEnemyMonster)
                        || MatchesCard(card, freshEnemyFaceupCard))
                    && !remainsOnEnemyField)
                {
                    ClearEnemyCommitment("tracked card left opponent field");
                }
                if (MatchesCard(card, pendingSnakehairDisableTarget)
                    && !remainsOnEnemyField)
                {
                    pendingSnakehairDisableTarget = null;
                }
                if (MatchesCard(card, pendingMammothDestroyTarget)
                    && !remainsOnEnemyField)
                {
                    pendingMammothDestroyTarget = null;
                }

                if (movedToOurHand)
                {
                    if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts,
                        CardId.Hublot,
                        CardId.EctoplasmicFortification))
                    {
                        pumpkingStarterSeenThisDuel = true;
                    }

                    if (greatPumpkingSearchAttempted
                        && selectedGreatPumpkingSearchId != 0
                        && card.IsCode(selectedGreatPumpkingSearchId)
                        && (previousLocation & (int)CardLocation.Deck) != 0)
                    {
                        greatPumpkingSearchResolved = true;
                        greatPumpkingSearchWindowPending = false;
                        DebugRoute("RESOLVED Great Pumpking search by move id=" + card.Id);
                    }

                    if (ashReplayLineActive
                        && card.IsCode(CardId.AshBlossom)
                        && (previousLocation & (int)CardLocation.MonsterZone) != 0)
                    {
                        ashReplayLineActive = false;
                    }
                }

                if (movedToOurMonsterZone)
                {
                    if (movedFromOurHand && !card.IsSpecialSummoned)
                        summonCount = 0;

                    if ((previousLocation & (int)CardLocation.Grave) != 0
                        && selectedSamuelReviveId != 0
                        && card.IsCode(selectedSamuelReviveId))
                    {
                        samuelRevivedCardId = card.Id;
                        samuelReviveResolved = true;
                        plannedSamuelReviveResolved = pendingInterruptMode != InterruptMode.Hold;
                        samuelReviveSelectionPending = false;
                        pumpkingComboState = PumpkingComboState.SamuelRevived;
                        DebugRoute("RESOLVED Samuel revive by move id=" + card.Id);
                    }

                    if (IsHublot(card))
                    {
                        if (movedFromOurHand && !card.IsSpecialSummoned)
                            summonCount = 0;
                        pumpkingStarterSeenThisDuel = true;
                        pumpkingComboState = PumpkingComboState.HublotSummoned;
                    }
                    else if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
                    {
                        if ((previousLocation & (int)CardLocation.Grave) != 0)
                            pumpkingComboState = PumpkingComboState.PumpkingRevived;
                    }
                    else if (card.IsCode(CardId.ChangshiTheSpiridao))
                    {
                        pumpkingComboState = PumpkingComboState.ChangshiSummoned;
                    }
                    else if (card.IsCode(CardId.OfficiatorOfDoomSamuel))
                    {
                        pumpkingComboState = PumpkingComboState.SamuelSummoned;
                    }
                    else if (card.IsCode(CardId.PumpkingTheGreatGhostKing))
                    {
                        pumpkingComboState = PumpkingComboState.GreatPumpkingSummoned;
                        greatPumpkingSearchWindowPending = true;
                        DebugRoute("OPEN Great Pumpking on-summon search window");
                    }
                    else if (card.IsCode(CardId.TheUndyingLegion))
                    {
                        pumpkingComboState = PumpkingComboState.UndyingSummoned;
                    }
                    else if (card.IsCode(CardId.FallenAngelOfTheGoldenLand))
                    {
                        eldlichRouteActive = true;
                        if (HasRecoverableEldlichForFlyingMary()
                            && Bot.HasInExtra(CardId.FlyingMary)
                            && CanAssemblePersistentFlyingMaryRoute())
                        {
                            eldlichRouteRank10CommitPending = true;
                            DebugRoute("COMMIT Fallen Angel route: Flying Mary then Rank 10 before generic fallback");
                        }
                    }
                    else if (card.IsCode(CardId.FlyingMary))
                    {
                        eldlichRouteMarySummoned = true;
                        if (eldlichRouteActive
                            || HasRecoverableEldlichForFlyingMary())
                        {
                            eldlichRouteActive = true;
                            eldlichRouteRank10CommitPending = true;
                            DebugRoute("COMMIT Flying Mary route: finish Level 10 Xyz before Rank 6 fallback");
                        }
                    }
                    else if (card.IsCode(CardId.Varudras))
                    {
                        if (eldlichRouteRank10CommitPending)
                            DebugRoute("RESOLVED Flying Mary Rank 10 line with Xyz=" + card.Id);
                        eldlichRouteRank10CommitPending = false;
                        eldlichRouteActive = false;
                        eldlichRouteMarySummoned = false;
                        flyingMaryEldlichReviveSelectionPending = false;
                    }
                }

                if (Duel.Turn > 0 && (movedToOurMonsterZone || movedToOurHand))
                    RecalculateStrategicPlan("our visible state changed");
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnDraw(int player)
        {
            base.OnDraw(player);
            if (player == 0)
            {
                ObservePumpkingStarterState();
                RecalculateStrategicPlan("draw");
            }
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo chain = Duel.GetCurrentSolvingChainInfo();
            bool solvingChainNegated = Duel.IsCurrentSolvingChainNegated();
            if (chain != null
                && chain.ActivateId == CardId.DeltaOfInvitation
                && solvingChainNegated)
            {
                pendingDeltaTokenFieldInstance = null;
            }
            if (chain != null && chain.ActivateController == 0 && !solvingChainNegated)
            {
                activatedThisTurn.Add(chain.ActivateId);

                if (chain.ActivateId == CardId.StareOfTheSnakeHair
                    && chain.ActivateLocation == CardLocation.MonsterZone)
                {
                    pendingSnakehairDisableTarget = null;
                    DebugRoute("RESOLVED Snakehair pre-emptive disable");
                }

                if (chain.ActivateId == CardId.GreatMammothOfTheNetherworld)
                {
                    pendingMammothDestroyTarget = null;
                    DebugRoute("RESOLVED Mammoth pre-emptive destroy");
                }

                if (chain.ActivateId == CardId.DominusImpulse
                    && chain.ActivateLocation == CardLocation.Hand)
                {
                    dominusImpulseHandLock = true;
                }

                if (chain.ActivateId == CardId.StareOfTheSnakeHair
                    && chain.ActivateLocation == CardLocation.Hand)
                {
                    pumpkingComboState = PumpkingComboState.PreparingPumpking;
                }

                if (chain.ActivateId == CardId.EctoplasmicFortification)
                {
                    ectoplasmicSearchUsed = true;
                    if (Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts))
                    {
                        pumpkingSearchSucceeded = true;
                        pumpkingComboState = PumpkingComboState.PumpkingReady;
                    }
                    else if (HasHublotInHand())
                    {
                        pumpkingStarterSeenThisDuel = true;
                        pumpkingComboState = PumpkingComboState.PreparingPumpking;
                    }
                }

                if (chain.ActivateId == CardId.DeltaOfInvitation)
                {
                    if (pendingDeltaTokenFieldInstance != null)
                    {
                        spentDeltaFieldInstance = Bot.GetSpells().FirstOrDefault(c => c != null
                            && c.IsCode(CardId.DeltaOfInvitation)
                            && c.IsFaceup()
                            && MatchesCard(c, pendingDeltaTokenFieldInstance))
                            ?? Bot.GetSpells().FirstOrDefault(c => c != null
                                && c.IsCode(CardId.DeltaOfInvitation)
                                && c.IsFaceup());
                        pendingDeltaTokenFieldInstance = null;
                        if (spentDeltaFieldInstance != null)
                        {
                            DebugRoute("MARK Delta field spent after resolved Token effect seq="
                                + spentDeltaFieldInstance.Sequence);
                        }
                    }

                    if (selectedDeltaSendId == CardId.EldlichTheGoldenLord
                        && Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
                    {
                        eldlichRouteActive = true;
                    }
                    else if (selectedDeltaSendId == CardId.DoomkingBalerdroch)
                    {
                        eldlichRouteActive = false;
                    }
                }

                if (IsHublotId(chain.ActivateId))
                {
                    if (chain.ActivateDescription == Util.GetStringId(chain.ActivateId, 1))
                    {
                        if (Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts))
                            pumpkingSearchSucceeded = true;
                        pumpkingComboState = PumpkingComboState.HublotResolved;
                    }
                }

                if (chain.ActivateId == CardId.EldlichTheGoldenLord
                    && chain.ActivateLocation == CardLocation.Hand)
                {
                    eldlichHandSelectionPending = false;
                    eldlichHandCostPromptCompleted = false;
                }

                if (chain.ActivateId == CardId.PumpkingTheKingOfGraveGhosts)
                {
                    if (chain.ActivateLocation == CardLocation.Hand
                        && pumpkingHandEffectAttempted
                        && !activatedThisTurn.Contains(PumpkingHandMarker))
                    {
                        activatedThisTurn.Add(PumpkingHandMarker);
                        pumpkingHandSelectionPending = false;
                        pumpkingCallPromptCompleted = false;
                        pendingPumpkingHandCard = null;
                        callSetByPumpking = Bot.HasInSpellZone(CardId.CallOfTheHaunted);
                        if (callSetByPumpking && freshSetCallByPumpkingInstance == null)
                        {
                            List<ClientCard> setCalls = Bot.GetSpells()
                                .Where(c => c != null
                                    && c.IsCode(CardId.CallOfTheHaunted)
                                    && c.IsFacedown())
                                .ToList();
                            if (setCalls.Count == 1)
                                freshSetCallByPumpkingInstance = setCalls[0];
                        }
                        pumpkingStarterSeenThisDuel = true;

                        if (callSetByPumpking && HasPumpkingInGrave())
                        {
                            pumpkingComboState = PumpkingComboState.CallReady;
                            DebugRoute("RESOLVED Pumpking hand effect: Call set and Pumpking in GY");
                        }
                        else
                        {
                            DebugRoute("RESOLVED Pumpking hand effect INCOMPLETE: callSet="
                                + callSetByPumpking + " pumpInGY=" + HasPumpkingInGrave());
                            DebugCards("HAND AFTER PUMPKING", Bot.Hand);
                            DebugCards("GY AFTER PUMPKING", Bot.Graveyard);
                        }
                    }
                    else if (chain.ActivateLocation == CardLocation.MonsterZone)
                    {
                        pumpkingSummonEffectResolved = true;
                        pumpkingComboState = PumpkingComboState.ChangshiSummoned;
                        DebugRoute("RESOLVED Pumpking summon effect");
                    }
                }

                if (chain.ActivateId == CardId.CallOfTheHaunted
                    && Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true))
                {
                    pumpkingComboState = PumpkingComboState.PumpkingRevived;
                }

                if (chain.ActivateId == CardId.FoolishBurial
                    && selectedFoolishBurialSendId == CardId.VampireGrace
                    && Bot.HasInGraveyard(CardId.VampireGrace))
                {
                    vampireGraceRouteActive = true;
                    DebugRoute("RESOLVED Foolish Grace setup: wait for guaranteed Zombie summon trigger");
                }

                if (chain.ActivateId == CardId.ChangshiTheSpiridao)
                {
                    changshiMillResolved = true;
                    DebugRoute("RESOLVED Changshi mill=" + selectedChangshiMillId);
                    if (changshiHandRescueRouteActive
                        && selectedChangshiMillId == CardId.EldlichTheGoldenLord
                        && Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
                    {
                        changshiHandRescueEldlichLoaded = true;
                        eldlichRouteActive = true;
                        DebugRoute("RESOLVED Changshi-hand rescue: Eldlich loaded; Undying then Rank 10");
                    }
                    if (selectedChangshiMillId == CardId.AshBlossom
                        && Bot.HasInGraveyard(CardId.AshBlossom))
                    {
                        ashReplayLineActive = true;
                    }
                }

                if (chain.ActivateId == CardId.OfficiatorOfDoomSamuel
                    && selectedSamuelReviveId != 0)
                {


                    if (Bot.HasInMonstersZone(selectedSamuelReviveId, faceUp: true))
                    {
                        samuelReviveResolved = true;
                        plannedSamuelReviveResolved = pendingInterruptMode != InterruptMode.Hold;
                        samuelRevivedCardId = selectedSamuelReviveId;
                        pumpkingComboState = PumpkingComboState.SamuelRevived;
                        DebugRoute("RESOLVED Samuel revive by chain id="
                            + selectedSamuelReviveId + "; desc="
                            + chain.ActivateDescription);
                    }
                    else
                    {
                        DebugRoute("Samuel revive did not place selected target; id="
                            + selectedSamuelReviveId + "; desc="
                            + chain.ActivateDescription);
                    }
                    samuelReviveSelectionPending = false;
                }

                if (chain.ActivateId == CardId.PumpkingTheGreatGhostKing)
                {
                    int bounceDescription = Util.GetStringId(
                        CardId.PumpkingTheGreatGhostKing, 2);
                    if (chain.ActivateDescription == bounceDescription)
                    {
                        greatPumpkingBounceResolved = true;
                        DebugRoute("RESOLVED Great Pumpking bounce");
                        if (ashReplayLineActive && Bot.HasInHand(CardId.AshBlossom))
                        {
                            ashReplayLineActive = false;
                        }
                    }
                    else if (greatPumpkingSearchAttempted)
                    {


                        greatPumpkingSearchResolved = true;
                        greatPumpkingSearchWindowPending = false;
                        DebugRoute("RESOLVED Great Pumpking search; desc="
                            + chain.ActivateDescription);
                    }
                }


                RecalculateStrategicPlan("chain solved id=" + chain.ActivateId);
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            currentNegateCardList.Clear();
            selectedHublotXyzId = 0;
            selectedChangshiMillId = 0;
            selectedDeltaSendId = 0;
            pendingDeltaTokenFieldInstance = null;
            selectedFoolishBurialSendId = 0;
            selectedSamuelReviveId = 0;
            pumpkingHandSelectionPending = false;
            pumpkingCallPromptCompleted = false;
            pumpkingDiscardSelfRequired = false;
            pendingPumpkingHandCard = null;
            doomkingOptionPending = false;
            doomkingPreferNegate = false;
            ClearVarudrasDestroyPlan();
            if ((pendingInterruptMode == InterruptMode.SamuelPreemptSnakehair
                    || pendingInterruptMode == InterruptMode.SamuelPreemptMammoth)
                && plannedSamuelReviveId != 0
                && !plannedSamuelReviveResolved)
            {
                pendingSnakehairDisableTarget = null;
                pendingMammothDestroyTarget = null;
                DebugRoute("CANCEL pre-emptive follow-up: Samuel revive failed");
            }
            ClearSamuelInterruptPlan();
            mezukiReviveSelectionPending = false;
            reverieOverlaySelectionPending = false;
            mezukiLevel6ExtensionPending = false;
            eldlichHandSelectionPending = false;
            eldlichHandCostPromptCompleted = false;
            pendingEldlichGraveFieldCost = null;
            flyingMaryComebackPumpkingPending = false;
            callReviveSelectionPending = false;
            plannedCallReviveId = 0;
            pendingInfiniteImpermanenceTarget = null;
            samuelGraveRecycleSelectionPending = false;
            pendingUndyingTarget = null;
            if (vampireGraceRouteActive && !IsVampireGraceRouteLive())
            {
                vampireGraceRouteActive = false;
                DebugRoute("CLEAR Vampire Grace route: Grace no longer exists in field/GY/overlay route state");
            }
            base.OnChainEnd();
            RefreshOpponentFaceupBeforeChainSnapshot();
        }

        public override void OnNewTurn()
        {
            activatedThisTurn.Clear();
            currentNegateCardList.Clear();
            pumpkingComboState = PumpkingComboState.None;
            pumpkingHandEffectAttempted = false;
            pumpkingHandSelectionPending = false;
            pumpkingCallPromptCompleted = false;
            pumpkingDiscardSelfRequired = false;
            pendingPumpkingHandCard = null;
            doomkingOptionPending = false;
            doomkingPreferNegate = false;
            ClearVarudrasDestroyPlan();
            ClearSamuelInterruptPlan();
            samuelFieldEffectCommittedThisTurn = false;
            pendingSnakehairDisableTarget = null;
            pendingMammothDestroyTarget = null;
            ClearEnemyCommitment("new turn");
            currentStrategicGoal = StrategicGoal.None;
            currentComboRoute = ComboRoute.None;
            mezukiReviveSelectionPending = false;
            reverieOverlaySelectionPending = false;
            mezukiLevel6ExtensionPending = false;
            armySpecialSummonEffectCommittedThisTurn = false;
            eldlichHandSelectionPending = false;
            eldlichHandCostPromptCompleted = false;
            pendingEldlichGraveFieldCost = null;
            pumpkingSummonEffectAttempted = false;
            pumpkingSummonEffectResolved = false;
            changshiMillAttempted = false;
            changshiMillResolved = false;
            greatPumpkingSearchAttempted = false;
            greatPumpkingSearchWindowPending = false;
            selectedGreatPumpkingSearchId = 0;
            greatPumpkingBounceAttempted = false;
            sheridanRemovalAttemptedThisTurn = false;
            ectoplasmicSearchUsed = false;
            pumpkingSearchSucceeded = false;
            callSetByPumpking = false;
            freshSetCallByPumpkingInstance = null;
            samuelReviveResolved = false;
            greatPumpkingSearchResolved = false;
            greatPumpkingBounceResolved = false;
            vampireGraceRouteActive = false;
            vampireGraceReviveCommittedThisTurn = false;
            zombieLockedThisTurn = false;
            changshiHandRescueRouteActive = false;
            changshiHandRescueEldlichLoaded = false;
            ashReplayLineActive = false;
            eldlichRouteActive = false;
            eldlichRouteMarySummoned = false;
            eldlichRouteRank10CommitPending = false;
            flyingMaryEldlichReviveSelectionPending = false;
            flyingMaryComebackPumpkingPending = false;
            callReviveSelectionPending = false;
            plannedCallReviveId = 0;
            pendingInfiniteImpermanenceTarget = null;
            samuelGraveRecycleSelectionPending = false;
            pendingUndyingTarget = null;
            lastSuckerMaterialPlanLog = null;
            selectedHublotSendId = 0;
            selectedHublotRecoverId = 0;
            selectedHublotRecover = false;
            selectedHublotXyzId = 0;
            selectedChangshiMillId = 0;
            selectedDeltaSendId = 0;
            pendingDeltaTokenFieldInstance = null;
            selectedFoolishBurialSendId = 0;
            selectedSamuelReviveId = 0;
            samuelRevivedCardId = 0;
            summonCount = 1;
            base.OnNewTurn();
            RefreshOpponentFaceupBeforeChainSnapshot();
            RestorePersistentEldlichRouteState();
            ObservePumpkingStarterState();
            RecalculateStrategicPlan("new turn");
            DebugRoute("NEW TURN " + Duel.Turn + " starterSeen=" + pumpkingStarterSeenThisDuel
                + " goal=" + currentStrategicGoal + " route=" + currentComboRoute);
            DebugCards("HAND", Bot.Hand);
        }
    }
}
