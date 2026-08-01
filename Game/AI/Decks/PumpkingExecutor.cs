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
            // Main Deck — Zombies
            public const int EldlichTheGoldenLord = 95440946;
            public const int DoomkingBalerdroch = 39185163;
            public const int ArmyOfTheHaunted = 18078153;
            public const int GreatMammothOfTheNetherworld = 80461466;
            public const int ChangshiTheSpiridao = 76352503;
            public const int OfficiatingReverie = 49828011;
            public const int PumpkingTheKingOfGraveGhosts = 81684048;
            public const int StareOfTheSnakeHair = 54077752;

            // MDPro3 currently exposes Hublot with the temporary custom ID.
            // Change this single constant when the official database ID goes live.
            public const int Hublot = 101306030;

            public const int Mezuki = 92826944;
            public const int GlowUpBloom = 92964816;

            // Main Deck — hand traps
            public const int MulcharmyPurulia = 84192580;
            public const int MulcharmyFuwalos = 42141493;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;

            // Main Deck — Spells/Traps
            public const int FoolishBurial = 81439173;
            public const int Terraforming = 73628505;
            public const int EctoplasmicFortification = 16734927;
            public const int DeltaOfInvitation = 3129133;
            public const int VortexOfTime = 42138622;
            public const int CallOfTheHaunted = 97077563;
            public const int InfiniteImpermanence = 10045474;
            public const int DominusImpulse = 40366667;
            public const int UpstartGoblin = 70368879;

            // Extra Deck
            public const int EldlichTheMadGoldenLord = 74889525;
            public const int FallenAngelOfTheGoldenLand = 43143567;
            public const int MercuriumTheLivingQuicksilver = 22984000;
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

            // Common opposing boss used by threat-aware interaction planners.
            public const int CrystalWingSynchroDragon = 50954680;
        }

        private readonly HashSet<int> activatedThisTurn = new HashSet<int>();
        private readonly List<ClientCard> currentNegateCardList = new List<ClientCard>();

        // Dominus Impulse activated from the hand prevents LIGHT/EARTH/WIND monster
        // effects for the rest of the Duel. This flag intentionally does not reset.
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
            UndyingSummoned,
            QuicksilverSummoned,
            GravityControllerSummoned
        }

        // V19 strategic layer: executors no longer decide only from "can this card
        // activate?". They consult the current goal/route and record the exact
        // purpose of reactive and pre-emptive interaction before any prompt arrives.
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
            BuildQuicksilverFallback,
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
            QuicksilverFallback,
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
            PostNegateRemoval,
            PostNegateSelfValue
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
            // Primary strategic target. For a direct Samuel negate this is the
            // monster Samuel disables; for a bridge plan it is the target that
            // Snakehair/Mammoth will answer after the revive resolves.
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
        // The Pumpking hand effect has two separate prompts (Set, then discard).
        // Track the accepted effect directly instead of relying only on
        // Duel.GetCurrentSolvingChainInfo(), which can be unavailable between prompts.
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
        // Great Pumpking's on-summon trigger can arrive with ActivateDescription=-1
        // on MDPro3. Track the summon window instead of depending only on StringId.
        private bool greatPumpkingSearchWindowPending = false;
        private int selectedGreatPumpkingSearchId = 0;
        private bool greatPumpkingBounceAttempted = false;
        private bool greatPumpkingBounceResolved = false;
        private bool quicksilverLineActive = false;
        private bool quicksilverLoadedBloom = false;
        private bool glowUpBloomEffectCommittedThisTurn = false;
        private bool zombieLockedThisTurn = false;

        // Prompt state must be captured when the executor accepts the effect.
        // Duel.LastChainPlayer / solving-chain metadata can change before the
        // subsequent option or target selection prompt arrives.
        private bool doomkingOptionPending = false;
        private bool doomkingPreferNegate = false;
        private VarudrasDestroyMode pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
        private ClientCard pendingVarudrasDestroyTarget = null;
        private ClientCard varudrasNegatedChainSource = null;
        // Samuel selects its revive target after the detach-cost prompt. MDPro3
        // may not expose a solving-chain card during that target prompt, so the
        // accepted activation must carry an explicit pending state.
        private bool samuelReviveSelectionPending = false;
        // Reverie's banished Standby effect targets a face-up Zombie Xyz, but the
        // Lua prompt is HINTMSG_TARGET rather than HINTMSG_XYZMaterial. Preserve
        // the accepted activation so MDPro3 cannot fall through to a generic target.
        private bool reverieOverlaySelectionPending = false;
        // Samuel's field Quick Effect and its GY trigger use different HOPT keys in
        // Lua. Track only the field effect here; the generic activated-card set
        // must not let Samuel's GY trigger incorrectly consume the field interrupt.
        private bool samuelFieldEffectCommittedThisTurn = false;
        // On the opponent's turn Samuel evaluates the total result of the revive:
        // direct ATK-covered disable, Snakehair/Mammoth interaction, or a Pumpking
        // bridge. The optional Samuel disable is only one part of that plan.
        private bool samuelOpponentNegatePending = false;
        private ClientCard samuelNegateTarget = null;
        // Mezuki's target prompt can also lose solving-chain metadata on MDPro3.
        // Keep its accepted revive flow separate from Samuel and generic prompts.
        private bool mezukiReviveSelectionPending = false;
        // Mezuki is not a generic value button. It is committed only when the
        // current plan is short of a Level 6 Zombie, and its target prompt must
        // preserve that purpose even when MDPro3 reports activateId=0.
        private bool mezukiLevel6ExtensionPending = false;
        // Army's hand/GY Ignition Effect is the free Level 6 body and must be
        // committed before Mezuki spends a graveyard extender. Track this effect
        // separately from Army's GY Set effect because they use different HOPT keys.
        private bool armySpecialSummonEffectCommittedThisTurn = false;
        // Eldlich's hand effect has two HINTMSG_TOGRAVE prompts: first choose
        // a Spell/Trap cost from our hand, then choose a card on the field.
        // MDPro3 can lose the solving-chain metadata between those prompts, so
        // preserve the accepted hand-effect flow explicitly.
        private bool eldlichHandSelectionPending = false;
        private bool eldlichHandCostPromptCompleted = false;

        // Changshi -> Ash replay branch:
        // Great Pumpking first, search Army, Army + Changshi make Samuel,
        // Samuel revives Ash, then Great Pumpking returns Ash to the hand.
        private bool ashReplayLineActive = false;

        // Delta -> Eldlich -> Fallen Angel -> Flying Mary -> Rank 10 route.
        private bool eldlichRouteActive = false;
        private bool eldlichRouteMarySummoned = false;
        // Once the dedicated Flying Mary line has begun, do not fall back into
        // Rank 6 summons. Mary must revive Eldlich and the two Level 10 bodies
        // must be converted into Varudras (or Quicksilver if Varudras is absent).
        private bool eldlichRouteRank10CommitPending = false;
        // Flying Mary's Lua uses HINTMSG_TARGET, and MDPro3 may report
        // activateId=0 for that target prompt. Preserve the committed Eldlich
        // revive explicitly so generic Level 5+ Zombie selection cannot choose
        // Army, Pumpking, or another legal body and break the Rank 10 finish.
        private bool flyingMaryEldlichReviveSelectionPending = false;
        // On our second or later turn, Flying Mary is a comeback bridge into the
        // unused Pumpking Special-Summon effect. Preserve this intent across the
        // target prompt because MDPro3 can temporarily report activateId=0.
        private bool flyingMaryComebackPumpkingPending = false;
        // Opponent-turn Call is held for an opposing chain and carries a locked
        // revive purpose into the target prompt, which may arrive with activateId=0.
        private bool callReviveSelectionPending = false;
        private int plannedCallReviveId = 0;
        // Infinite Impermanence must negate the intended live monster, normally
        // the current opposing chain source, rather than a generic field target.
        private ClientCard pendingInfiniteImpermanenceTarget = null;
        // Samuel's GY trigger is a recycle effect, not generic grave disruption.
        private bool samuelGraveRecycleSelectionPending = false;
        // Undying is saved for a real opposing action (especially a revival
        // target) and records that exact monster before the target prompt.
        private ClientCard pendingUndyingTarget = null;
        // Executor polling can ask for the same Sucker material plan repeatedly
        // without any board change. Cache only the diagnostic text; the planner
        // itself is still recalculated every time.
        private string lastSuckerMaterialPlanLog = null;

        // Once a real Pumpking starter has been seen, Quicksilver is disabled for
        // the rest of the Duel. This flag intentionally does not reset each turn.
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
            // -----------------------------------------------------------------
            // Counters and hand traps. Dominus logic is adapted from AI_Apophis:
            // prefer an already-set copy, inspect the opponent's current chain,
            // and mark the chain card to avoid spending multiple negates on it.
            // -----------------------------------------------------------------
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyPurulia, MulcharmyPuruliaActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, MulcharmyFuwalosActivate);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusImpulse, DominusImpulseActivate);
            AddExecutor(ExecutorType.Activate, CardId.VortexOfTime, VortexOfTimeActivate);
            AddExecutor(ExecutorType.Activate, CardId.DoomkingBalerdroch, DoomkingBalerdrochActivate);
            // Samuel is narrower than Varudras: it can answer only a face-up
            // on-field monster in the Main Phase and needs a suitable Zombie in
            // the GY. Query it first, then preserve Varudras for chains Samuel
            // cannot cover (Spell/Trap, hand/GY effects, or excessive ATK).
            AddExecutor(ExecutorType.Activate, CardId.OfficiatorOfDoomSamuel, OfficiatorOfDoomSamuelActivate);
            AddExecutor(ExecutorType.Activate, CardId.Varudras, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.EvolzarLars, EvolzarLarsActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheUndyingLegion, TheUndyingLegionActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheGreatGhostKing, PumpkingGreatGhostKingActivate);

            // -----------------------------------------------------------------
            // Searchers and starters.
            // -----------------------------------------------------------------
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingActivate);
            AddExecutor(ExecutorType.Activate, CardId.StareOfTheSnakeHair, StareOfTheSnakeHairHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.EctoplasmicFortification, EctoplasmicFortificationActivate);

            // Pumpking line: resolve the hand searchers before committing the Normal
            // Summon. Once Pumpking is already available, Hublot becomes a GY setup
            // card instead of always milling/recovering Pumpking.
            AddExecutor(ExecutorType.Summon, CardId.Hublot, HublotSummon);
            // Brick fallback: with no Pumpking starter and no Quicksilver line,
            // commit the cheapest legal Zombie Normal Summon so Delta/Eldlich can
            // still leave a second Zombie beside Fallen Angel for Flying Mary.
            AddExecutor(ExecutorType.Summon, CardId.Mezuki, BrickZombieNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.GlowUpBloom, BrickZombieNormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, BrickZombieNormalSummon);
            AddExecutor(ExecutorType.Activate, CardId.Hublot, HublotActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingSummonActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChangshiTheSpiridao, ChangshiTheSpiridaoActivate);
            // Reverie is checked immediately after the forced Pumpking/Changshi
            // steps. Its own guard keeps it from cutting ahead of those effects,
            // but once Changshi has milled it can extend from the hand before Delta
            // or a generic Extra Deck fallback consumes the board.
            AddExecutor(ExecutorType.Activate, CardId.OfficiatingReverie, OfficiatingReverieActivate);

            // Delta is checked after an available Hublot/Pumpking action. This lets
            // Hublot become the Zombie Link seed before Delta commits to Eldlich.
            AddExecutor(ExecutorType.Activate, CardId.DeltaOfInvitation, DeltaOfInvitationActivate);

            // -----------------------------------------------------------------
            // Main Deck extenders and grave effects.
            // -----------------------------------------------------------------
            // Generic extenders and Foolish are intentionally below the immediate
            // Pumpking steps above. Their own conditions also refuse to cut across
            // an accepted Pumpking/Call/Changshi action.
            AddExecutor(ExecutorType.Activate, CardId.EldlichTheGoldenLord, EldlichTheGoldenLordActivate);
            AddExecutor(ExecutorType.Activate, CardId.ArmyOfTheHaunted, ArmyOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.Mezuki, MezukiActivate);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBloom, GlowUpBloomActivate);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialActivate);
            AddExecutor(ExecutorType.Activate, CardId.GreatMammothOfTheNetherworld, GreatMammothActivate);
            AddExecutor(ExecutorType.Activate, CardId.StareOfTheSnakeHair, StareOfTheSnakeHairFieldActivate);

            // -----------------------------------------------------------------
            // Rank 6 / Rank 7 / Rank 10 and Link lines.
            // -----------------------------------------------------------------
            // Finish any available Rank 10 line before accepting a Zombie lock.
            AddExecutor(ExecutorType.SpSummon, CardId.Varudras, VarudrasSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.MercuriumTheLivingQuicksilver, MercuriumSummon);

            // Reserve the board for the confirmed Eldlich route before generic
            // Rank 6 summons are allowed to consume its Zombie link seed.
            AddExecutor(ExecutorType.SpSummon, CardId.FallenAngelOfTheGoldenLand, FallenAngelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FlyingMary, FlyingMaryEldlichRouteSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.OfficiatorOfDoomSamuel, OfficiatorOfDoomSamuelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PumpkingTheGreatGhostKing, PumpkingGreatGhostKingSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DhampirVampireSheridan, DhampirVampireSheridanSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.EvolzarLars, EvolzarLarsSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.WollowFounderOfTheDrudgeDragons, WollowSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.TheUndyingLegion, TheUndyingLegionSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.GravityController, GravityControllerSummon);
            // On later turns, establish Vampire Sucker before reviving Eldlich so
            // the GY summon draws a card. Generic Flying Mary remains the comeback
            // option only when that draw line is unavailable.
            AddExecutor(ExecutorType.SpSummon, CardId.VampireSucker, VampireSuckerSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FlyingMary, FlyingMarySummon);

            // Extra Deck effects.
            AddExecutor(ExecutorType.Activate, CardId.DhampirVampireSheridan, DhampirVampireSheridanActivate);
            AddExecutor(ExecutorType.Activate, CardId.WollowFounderOfTheDrudgeDragons, WollowActivate);
            AddExecutor(ExecutorType.Activate, CardId.MercuriumTheLivingQuicksilver, MercuriumActivate);
            AddExecutor(ExecutorType.Activate, CardId.FallenAngelOfTheGoldenLand, FallenAngelActivate);
            AddExecutor(ExecutorType.Activate, CardId.EldlichTheMadGoldenLord, EldlichTheMadGoldenLordActivate);
            AddExecutor(ExecutorType.Activate, CardId.FlyingMary, FlyingMaryActivate);
            AddExecutor(ExecutorType.Activate, CardId.VampireSucker, VampireSuckerActivate);

            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        // =====================================================================
        // Common helpers
        // =====================================================================

        private bool IsHublot(ClientCard card)
        {
            return card != null && card.IsCode(CardId.Hublot);
        }

        private bool IsHublotId(int id)
        {
            return id == CardId.Hublot;
        }

        private void DebugRoute(string message)
        {
            Logger.DebugWriteLine("[Pumpking] " + message);
        }

        private void DebugCards(string label, IEnumerable<ClientCard> cards)
        {
            if (cards == null)
            {
                DebugRoute(label + "=<null>");
                return;
            }

            DebugRoute(label + "=" + string.Join(",",
                cards.Where(c => c != null)
                    .Select(c => c.Id + "@" + c.Location + "#" + c.Sequence)
                    .ToArray()));
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

        private bool CanBuildSurplusVarudras()
        {
            if (zombieLockedThisTurn || !Bot.HasInExtra(CardId.Varudras))
                return false;
            if (!pumpkingStarterSeenThisDuel && ShouldUseQuicksilverFallback())
                return false;
            if (GetRank10Materials().Count != 2)
                return false;

            // Completing the confirmed Eldlich route is valid. Outside that line,
            // Rank 10 bodies are "surplus" only after the protected Pumpking board
            // is already present; never cannibalise the route while it is building.
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
            if (HasDirectPumpkingLineAvailable() || ShouldUseQuicksilverFallback())
                return false;
            if (!HasEldlichRouteExtraDeck())
                return false;

            bool fieldAccess = HasFaceupFieldSpell()
                || Bot.HasInHand(CardId.DeltaOfInvitation)
                || Bot.HasInHand(CardId.Terraforming);
            bool eldlichAccess = Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || CheckRemainInDeck(CardId.EldlichTheGoldenLord) > 0;
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

            // A Flying Mary commitment is the point of no return: finish the
            // Level 10 Xyz before considering any Pumpking recovery branch.
            if (eldlichRouteRank10CommitPending)
            {
                SetStrategicPlan(StrategicGoal.CompleteEldlichRoute,
                    ComboRoute.EldlichRank10, reason);
                return;
            }

            // Merely sending Eldlich with Delta must not overwrite a live
            // Pumpking combo. Finish the concrete Rank 6 sequence first.
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

            if (ShouldUseQuicksilverFallback())
            {
                SetStrategicPlan(StrategicGoal.BuildQuicksilverFallback,
                    ComboRoute.QuicksilverFallback, reason);
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
            if (IsLiveEnemyFieldCard(fresh) && fresh.IsFaceup()
                && !fresh.IsShouldNotBeTarget())
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

            return GetEnemyFieldPriority()
                .FirstOrDefault(c => c != null && c.Controller == 1
                    && c.IsFaceup()
                    && !c.IsShouldNotBeTarget()
                    && (c.IsFloodgate()
                        || (c.IsMonster()
                            && (c.IsMonsterDangerous() || c.IsMonsterInvincible()
                                || c.IsMonsterShouldBeDisabledBeforeItUseEffect()))));
        }

        private ClientCard GetSamuelPumpkingBridgeTarget(
            IEnumerable<ClientCard> reviveTargets,
            int deckFollowUpId)
        {
            if (reviveTargets == null
                || pumpkingSummonEffectAttempted
                || pumpkingSummonEffectResolved
                || GetOpenMainMonsterZoneCount() < 2
                || CheckRemainInDeck(deckFollowUpId) <= 0)
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

            // Mad Golden targets a face-up monster we control before either
            // player can respond. Chain Samuel now so its material and revival
            // are not lost even when the revived Zombie cannot disable Mad Golden.
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

            // Pumpking is the best emergency body: after the current chain it can
            // turn this non-negating Samuel activation into Mammoth/Snakehair or
            // Hublot setup through its Special-Summon trigger.
            ClientCard pumpking = legal.FirstOrDefault(c =>
                c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking != null)
                return pumpking;

            int[] priority =
            {
                CardId.PumpkingTheGreatGhostKing,
                CardId.OfficiatingReverie,
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

            // During an opposing monster chain, stopping the actual chain source
            // is useful even when the generic card database does not classify the
            // monster as a boss. In an open state, however, do not spend Samuel on
            // a body whose relevant effect is not live on the field (Assault Beast
            // is the regression that exposed this distinction).
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
            if (!IsLiveEnemyFieldCard(card) || card.IsShouldNotBeTarget())
                return false;

            if (card.IsMonster())
            {
                return card.IsFaceup()
                    && (IsKnownLiveNegateMonster(card)
                        || card.IsMonsterShouldBeDisabledBeforeItUseEffect()
                        || card.IsFloodgate()
                        || card.IsMonsterInvincible());
            }

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
                || IsCardEffectNegated(samuel)
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

                // First choice: actually negate the current on-field monster chain
                // when a legal Zombie revive has enough ATK. The target determines
                // the required ATK; enemy ATK is never used as a value threshold.
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

                // A boss such as Crystal Wing may exceed every revivable Zombie's
                // ATK. Samuel can still be correct because the revive itself creates
                // Snakehair/Mammoth interaction, directly or through Pumpking.
                InterruptPlan bridge = BuildSamuelBridgePlan(
                    reviveTargets, chainSource);
                if (bridge != null)
                    return bridge;

                // If the chain source cannot be covered and no bridge is available,
                // Samuel may still disable another genuinely valuable face-up monster.
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

            // Open-state planning scans the whole opposing board. Do not equate
            // "freshly summoned" with "worth negating" and do not spend Samuel
            // merely because a low-value body happens to fit the ATK condition.
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

            // No direct ATK-covered disable does not mean "hold". Check whether
            // the revive creates interaction through Snakehair/Mammoth or Pumpking.
            return BuildSamuelBridgePlan(reviveTargets, null);
        }

        private void CommitSamuelInterruptPlan(InterruptPlan plan)
        {
            pendingInterruptMode = plan != null ? plan.Mode : InterruptMode.Hold;
            plannedSamuelReviveId = plan != null && plan.SamuelReviveTarget != null
                ? plan.SamuelReviveTarget.Id : 0;
            samuelNegateTarget = plan != null
                ? plan.SamuelDisableTarget : null;
            // The revive is the primary value. Samuel's own disable is an optional
            // bonus and may coexist with a separate Snakehair/Mammoth follow-up.
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

        private void ClearVarudrasDestroyPlan()
        {
            pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
            pendingVarudrasDestroyTarget = null;
            varudrasNegatedChainSource = null;
        }

        private bool WillLeaveFieldAfterVarudrasNegate(ClientCard card)
        {
            if (card == null || !card.IsOnField())
                return false;

            // Negating the activation of a Spell/Trap that is itself in the
            // Spell & Trap Zone removes that activation source before Varudras'
            // optional follow-up target prompt is reached.
            return card.Location == CardLocation.SpellZone
                && (card.IsSpell() || card.IsTrap());
        }

        private ClientCard FindVarudrasEnemyTargetAfterNegate()
        {
            // The optional follow-up costs another material. Spend it only on a
            // known face-up card that is confirmed to survive the negated chain.
            // Never answer Yes based on an anonymous facedown card (Id 0).
            return GetEnemyFieldPriority(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()).ToList(),
                    false)
                .FirstOrDefault(c => c != null
                    && c.Controller == 1
                    && c.IsOnField()
                    && c.IsFaceup()
                    && c.Id != 0
                    && !(MatchesCard(c, varudrasNegatedChainSource)
                        && WillLeaveFieldAfterVarudrasNegate(c)));
        }

        private bool HasLegalBloomSearchTargetInDeck()
        {
            return GetGlowUpBloomSearchPriority()
                .Any(id => CheckRemainInDeck(id) > 0);
        }

        private bool NeedsNonZombieExtraDeckSummon()
        {
            return !zombieLockedThisTurn && !CanAcceptZombieLock();
        }

        private bool CanProfitablyDestroyBloomWithVarudras(ClientCard bloom = null)
        {
            if (bloom == null)
            {
                bloom = Bot.GetMonsters().FirstOrDefault(c => c != null
                    && c.IsCode(CardId.GlowUpBloom)
                    && c.IsFaceup());
            }

            if (bloom == null
                || bloom.Controller != 0
                || bloom.Location != CardLocation.MonsterZone
                || !bloom.IsFaceup())
            {
                return false;
            }

            if (glowUpBloomEffectCommittedThisTurn
                || activatedThisTurn.Contains(CardId.GlowUpBloom)
                || !DefaultCheckWhetherBotCanSearch()
                || !HasLegalBloomSearchTargetInDeck())
            {
                return false;
            }

            // On the opponent's turn the Bloom lock expires in that End Phase,
            // before our next turn begins.
            if (Duel.Player == 1)
                return true;

            // On our turn, self-pop Bloom only after the protected Pumpking board
            // exists and no Varudras/Quicksilver conversion is still pending.
            return HasCorePumpkingEndboard()
                && !NeedsNonZombieExtraDeckSummon();
        }

        private bool IsUnlinkedFaceupCall(ClientCard card)
        {
            return card != null
                && card.Controller == 0
                && card.Location == CardLocation.SpellZone
                && card.IsFaceup()
                && card.IsCode(CardId.CallOfTheHaunted)
                && (card.TargetCards == null || card.TargetCards.Count == 0);
        }

        private ClientCard GetCallLinkedMonster(ClientCard call)
        {
            if (call == null
                || !call.IsCode(CardId.CallOfTheHaunted)
                || call.TargetCards == null)
            {
                return null;
            }

            return call.TargetCards.FirstOrDefault(c => c != null
                && c.Controller == 0
                && c.Location == CardLocation.MonsterZone);
        }

        private bool IsLinkedFaceupCall(ClientCard card)
        {
            return card != null
                && card.Controller == 0
                && card.Location == CardLocation.SpellZone
                && card.IsFaceup()
                && card.IsCode(CardId.CallOfTheHaunted)
                && GetCallLinkedMonster(card) != null;
        }

        private bool IsExpendableLinkedCallForEldlich(ClientCard call)
        {
            ClientCard linked = GetCallLinkedMonster(call);
            if (linked == null)
                return true;

            // A linked Call is normally protected because sending the exact Trap
            // instance destroys the monster it is sustaining. Only a genuinely
            // spent utility body may be released for the Eldlich route.
            return IsSpentZombieXyz(linked)
                || linked.IsCode(CardId.GravityController)
                || (linked.IsCode(CardId.GlowUpBloom)
                    && (glowUpBloomEffectCommittedThisTurn
                        || activatedThisTurn.Contains(CardId.GlowUpBloom)));
        }

        private int GetEldlichGraveFieldCostScore(ClientCard card)
        {
            if (card == null
                || card.Controller != 0
                || card.Location != CardLocation.SpellZone
                || (!card.IsSpell() && !card.IsTrap()))
            {
                return int.MaxValue;
            }

            if (IsUnlinkedFaceupCall(card))
                return 0;
            if (card.IsFaceup() && card.IsCode(CardId.DeltaOfInvitation))
                return 10;
            if (card.IsFaceup() && !card.IsCode(CardId.CallOfTheHaunted))
                return 20;
            if (card.IsFacedown() && !card.IsCode(CardId.CallOfTheHaunted))
                return 40;

            if (IsLinkedFaceupCall(card))
            {
                ClientCard linked = GetCallLinkedMonster(card);
                int linkedLoss = linked == null ? 0 : GetMaterialValue(linked);
                return 1000 + linkedLoss;
            }

            // A Set Call has no linked monster and is safer than sacrificing a
            // face-up Call that is holding a monster on the field.
            if (card.IsCode(CardId.CallOfTheHaunted))
                return 60;

            return 100;
        }

        private ClientCard GetBestEldlichGraveFieldCost(
            IEnumerable<ClientCard> source = null)
        {
            IEnumerable<ClientCard> pool = source ?? Bot.GetSpells();
            return pool
                .Where(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.SpellZone
                    && (c.IsSpell() || c.IsTrap()))
                .OrderBy(GetEldlichGraveFieldCostScore)
                .ThenBy(c => c.Sequence)
                .FirstOrDefault();
        }

        private bool HasSafeEldlichGraveFieldCost()
        {
            ClientCard cost = GetBestEldlichGraveFieldCost();
            if (cost == null)
                return false;

            return !IsLinkedFaceupCall(cost)
                || IsExpendableLinkedCallForEldlich(cost);
        }

        private ClientCard FindProfitableVarudrasSelfDestroyTarget(
            IEnumerable<ClientCard> source = null)
        {
            List<ClientCard> pool = (source
                    ?? Bot.GetMonsters().Concat(Bot.GetSpells()))
                .Where(c => c != null && c.Controller == 0 && c.IsOnField())
                .ToList();

            ClientCard bloom = pool.FirstOrDefault(c =>
                c.IsCode(CardId.GlowUpBloom)
                && c.Location == CardLocation.MonsterZone
                && c.IsFaceup());
            if (CanProfitablyDestroyBloomWithVarudras(bloom))
                return bloom;

            ClientCard gravityController = pool.FirstOrDefault(c =>
                c.IsCode(CardId.GravityController)
                && c.Location == CardLocation.MonsterZone
                && c.IsFaceup());
            if (gravityController != null)
                return gravityController;

            return pool.FirstOrDefault(IsUnlinkedFaceupCall);
        }

        private string GetVarudrasSelfDestroyReason(ClientCard target)
        {
            if (target == null)
                return "unknown self-value";
            if (target.IsCode(CardId.GlowUpBloom))
            {
                return Duel.Player == 1
                    ? "opponent-turn Bloom trigger"
                    : "safe Bloom trigger after core board";
            }
            if (target.IsCode(CardId.GravityController))
                return "spent Gravity Controller";
            if (target.IsCode(CardId.CallOfTheHaunted))
                return "face-up Call with no linked monster";
            return "approved self-value";
        }

        private bool PlanVarudrasPostNegateDestroy()
        {
            pendingVarudrasDestroyMode = VarudrasDestroyMode.None;
            pendingVarudrasDestroyTarget = null;

            ClientCard enemyTarget = FindVarudrasEnemyTargetAfterNegate();
            if (enemyTarget != null)
            {
                pendingVarudrasDestroyMode =
                    VarudrasDestroyMode.PostNegateRemoval;
                pendingVarudrasDestroyTarget = enemyTarget;
                DebugRoute("VARUDRAS post-negate plan=enemy target="
                    + enemyTarget.Id
                    + " reason=surviving enemy field card");
                return true;
            }

            ClientCard selfValueTarget =
                FindProfitableVarudrasSelfDestroyTarget();
            if (selfValueTarget != null)
            {
                pendingVarudrasDestroyMode =
                    VarudrasDestroyMode.PostNegateSelfValue;
                pendingVarudrasDestroyTarget = selfValueTarget;
                DebugRoute("VARUDRAS post-negate plan=self-value target="
                    + selfValueTarget.Id
                    + " reason=" + GetVarudrasSelfDestroyReason(selfValueTarget));
                return true;
            }

            DebugRoute("VARUDRAS post-negate destroy=False reason="
                + "no surviving enemy target or profitable self target");
            return false;
        }

        private ClientCard FindBestVarudrasTargetFromPrompt(
            IList<ClientCard> cards)
        {
            if (cards == null)
                return null;

            bool postNegate = pendingVarudrasDestroyMode
                == VarudrasDestroyMode.PostNegateRemoval
                || pendingVarudrasDestroyMode
                    == VarudrasDestroyMode.PostNegateSelfValue
                || pendingVarudrasDestroyMode == VarudrasDestroyMode.None;

            ClientCard enemyTarget = GetEnemyFieldPriority(cards, false)
                .Where(cards.Contains)
                .FirstOrDefault(c => c != null
                    && c.Controller == 1
                    && c.IsOnField()
                    && (!postNegate || (c.IsFaceup() && c.Id != 0))
                    && !(MatchesCard(c, varudrasNegatedChainSource)
                        && WillLeaveFieldAfterVarudrasNegate(c)));
            if (enemyTarget != null)
                return enemyTarget;

            if (pendingVarudrasDestroyMode == VarudrasDestroyMode.PostNegateRemoval
                || pendingVarudrasDestroyMode == VarudrasDestroyMode.PostNegateSelfValue
                || pendingVarudrasDestroyMode == VarudrasDestroyMode.None)
            {
                return FindProfitableVarudrasSelfDestroyTarget(cards);
            }

            return null;
        }

        private int ScoreVarudrasDestroyLoss(ClientCard card)
        {
            if (card == null)
                return int.MaxValue;
            if (card.Controller == 1)
                return -10000;

            if (card.IsCode(CardId.GlowUpBloom))
                return CanProfitablyDestroyBloomWithVarudras(card) ? 0 : 8800;
            if (card.IsCode(CardId.GravityController))
                return 10;
            if (IsUnlinkedFaceupCall(card))
                return 20;

            if (card.IsCode(CardId.Varudras))
                return 10000;
            if (card.IsCode(CardId.TheUndyingLegion))
                return 9900;
            if (card.IsCode(CardId.OfficiatorOfDoomSamuel))
                return 9800;
            if (card.IsCode(CardId.PumpkingTheGreatGhostKing))
                return 9700;
            if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts))
                return 9600;
            if (card.IsCode(CardId.EldlichTheGoldenLord))
                return 9500;
            if (card.IsCode(CardId.CallOfTheHaunted))
                return 9400;
            if (IsLevel6Zombie(card))
                return 9000;

            if (card.IsSpell() || card.IsTrap())
                return card.IsFacedown() ? 1000 : 2500;

            return 4000 + Math.Max(0, card.GetDefensePower());
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
            if (planned != null && planned.IsOnField())
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

            // The field can change after the Yes/No answer. Re-plan only from the
            // candidates the actual mandatory prompt supplies.
            ClientCard recalculated = FindBestVarudrasTargetFromPrompt(cards);
            if (recalculated != null)
            {
                DebugRoute("VARUDRAS recalculated mandatory target="
                    + recalculated.Id
                    + " mode=" + pendingVarudrasDestroyMode);
                ClearVarudrasDestroyPlan();
                return Util.CheckSelectCount(
                    new List<ClientCard> { recalculated }, cards, min, max);
            }

            // Lua forces one field card after Yes. Never return null and let the
            // engine pick an arbitrary friendly card.
            ClientCard lowestLoss = cards
                .Where(c => c != null && c.IsOnField())
                .OrderBy(ScoreVarudrasDestroyLoss)
                .ThenBy(c => c.Id)
                .FirstOrDefault();
            if (lowestLoss == null)
                lowestLoss = cards.First(c => c != null);

            DebugRoute("VARUDRAS emergency mandatory target="
                + lowestLoss.Id);
            ClearVarudrasDestroyPlan();
            return Util.CheckSelectCount(
                new List<ClientCard> { lowestLoss }, cards, min, max);
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
            // Pumpking's Lua can Set Call only from the Deck or Graveyard.
            // A copy already in the hand or on the field does not make the hand
            // effect executable.
            return CheckRemainInDeck(CardId.CallOfTheHaunted) > 0
                || Bot.Graveyard.Any(c => c != null
                    && c.IsCode(CardId.CallOfTheHaunted));
        }

        private bool HasPumpkingDeckSummonTargetInDeck()
        {
            // Pumpking summons a Level 6 Zombie whose ATK is not 1950.
            return CheckRemainInDeck(CardId.Hublot) > 0
                || CheckRemainInDeck(CardId.GreatMammothOfTheNetherworld) > 0
                || CheckRemainInDeck(CardId.ChangshiTheSpiridao) > 0
                || CheckRemainInDeck(CardId.StareOfTheSnakeHair) > 0
                || CheckRemainInDeck(CardId.OfficiatingReverie) > 0
                || CheckRemainInDeck(CardId.ArmyOfTheHaunted) > 0;
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
            // Recover Pumpking only when the recovered card can immediately turn
            // into the Call -> Pumpking -> Level 6 line. Otherwise it is a brick
            // in hand and must not hold Delta/Eldlich behind a false Pumpking gate.
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

            // callSetByPumpking is a historical route marker. Once the Set Call
            // has already been flipped face-up/resolved (or has left the field),
            // it is no longer an executable Pumpking action and must not keep
            // Delta/Foolish/Extra Deck lines locked for the rest of the turn.
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

            // The Normal Summon is a concrete action, not merely a route marker.
            // Delta/Eldlich must not cut in after Ectoplasmic has already put
            // Hublot in the hand while the no-Tribute summon is still available.
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

            // Any opposing field card is a profitable target. With only one such
            // card the selector may pair it with one explicitly approved own card.
            if (GetEnemyFieldPriority().Count > 0)
                return true;

            // The only own-only use is the established Urara recovery line.
            return ashReplayLineActive
                && Bot.HasInMonstersZone(CardId.AshBlossom, faceUp: true);
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

            int faceupCallCount = Bot.GetSpells().Count(c => c != null
                && c.IsFaceup()
                && c.IsCode(CardId.CallOfTheHaunted));
            if (faceupCallCount >= 2)
            {
                return source.FirstOrDefault(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.SpellZone
                    && c.IsFaceup()
                    && c.IsCode(CardId.CallOfTheHaunted));
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

        private bool IsCardEffectNegated(ClientCard card = null)
        {
            return DefaultCheckWhetherCardIsNegated(card ?? Card);
        }

        private bool IsOpponentChainWorthNegating(ClientCard card)
        {
            if (card == null || card.Controller != 1 || card.IsDisabled())
                return false;
            if (currentNegateCardList.Contains(card))
                return false;

            // Do not waste a negate on a card that is already being negated in the
            // same chain. Otherwise, an activable Dominus/Imperm is considered live.
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

            // Eldlich is sorcery-speed removal. Remove a live monster negate or
            // monster floodgate before spending the effect on a Spell/Trap.
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

        private int GetInitialDeckCount(int id)
        {
            if (id == CardId.PumpkingTheKingOfGraveGhosts
                || id == CardId.StareOfTheSnakeHair
                || IsHublotId(id)
                || id == CardId.EctoplasmicFortification
                || id == CardId.DeltaOfInvitation
                || id == CardId.InfiniteImpermanence)
            {
                return 3;
            }

            if (id == CardId.MulcharmyPurulia
                || id == CardId.MulcharmyFuwalos
                || id == CardId.AshBlossom
                || id == CardId.CallOfTheHaunted
                || id == CardId.DominusImpulse)
            {
                return 2;
            }

            return 1;
        }

        private int CheckRemainInDeck(int id)
        {
            return Bot.GetRemainingCount(id, GetInitialDeckCount(id));
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
            // Use ClientCard.IsCode through IsHublot rather than ClientField.HasInHand
            // so aliases/card-database mappings are handled consistently.
            return Bot.Hand.Any(IsHublot);
        }

        private int GetEctoplasmicSearchTargetId()
        {
            // Patch 01 route rule:
            // 1) no Hublot in hand -> search Hublot;
            // 2) Hublot already in hand -> search Pumpking.
            // Fall back only when the primary target is no longer in the Deck.
            if (!HasHublotInHand())
            {
                if (CheckRemainInDeck(CardId.Hublot) > 0)
                    return CardId.Hublot;

                if (!HasPumpkingInHand()
                    && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0)
                {
                    return CardId.PumpkingTheKingOfGraveGhosts;
                }

                return 0;
            }

            if (!HasPumpkingInHand()
                && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0)
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

            // Fallen Angel already confirms the dedicated Flying Mary/Rank 10 line.
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
            // Without an existing Link monster opening a Main Monster Zone, a
            // non-Link monster occupying our EMZ has to be used to free that zone
            // before Vampire Sucker can be Link Summoned there. This is the key
            // case missed by a simple "two zero-material Xyz" count.
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

                // Live Xyz interaction is substantially more valuable than one
                // Sucker draw. Undying is weighted highest because its materials
                // represent a future opponent-turn removal, especially when it is
                // the EMZ body that would be mandatory Link material.
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

            // Main Deck Zombies may still be combo bodies or interaction. Keep
            // their ordering aligned with the existing material-value policy.
            return 45 + GetZombieLinkMaterialValue(card) * 4;
        }

        private int GetVampireSuckerBridgeValue(ClientCard mandatoryEmz)
        {
            // The bridge produces Sucker, converts Eldlich's GY summon into one
            // draw, and frees one net monster zone. Increase its value when board
            // space is tight or when it clears an already-spent EMZ body.
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
                && (Bot.HasInExtra(CardId.Varudras)
                    || Bot.HasInExtra(CardId.MercuriumTheLivingQuicksilver));
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

            // A used low-Level body is an acceptable Mary seed. Preserve the
            // Level 6 engine and small Pumpking when Delta can supply a Token.
            if (card.IsCode(CardId.GlowUpBloom))
                return 30;
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

            // Before Fallen Angel is made, the Token and Eldlich each need a Main
            // Monster Zone. If Eldlich or Fallen is already on the field, only the
            // Token needs a free zone because the next Link/Tribute frees space.
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

            // A face-up Delta can still supply the missing partner before Mary.
            return Bot.HasInSpellZone(CardId.DeltaOfInvitation, faceUp: true)
                && HasOpenMainMonsterZone();
        }

        private void RestorePersistentEldlichRouteState()
        {
            bool rank10AlreadyMade = Bot.HasInMonstersZone(CardId.Varudras, faceUp: true)
                || Bot.HasInMonstersZone(CardId.MercuriumTheLivingQuicksilver, faceUp: true);
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

            // A Level 5 Token is not a Rank 6 body. Do not occupy a Main Monster
            // Zone while Pumpking, Army, Mezuki, or the Urara route still needs
            // room to produce Level 6 monsters.
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

            // Delta's Token is first a Flying Mary resource. On turn 1 this
            // preserves live Samuel/Undying while Fallen Angel + Token become Mary.
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
            // Do not spend a Main Monster Zone for a negligible gain. Create the
            // Token only when it saves a genuinely live body rather than replacing
            // one already-spent Xyz with another cheap material.
            if (tokenLoss + 30 < noTokenLoss)
            {
                DebugRoute("Delta Token value: lowers Sucker material loss from "
                    + noTokenLoss + " to " + tokenLoss);
                return true;
            }

            DebugRoute("HOLD Delta Token: existing Sucker pair is already efficient");
            return false;
        }

        private int[] GetGlowUpBloomSearchPriority()
        {
            List<int> priority = new List<int>();

            bool pumpkingStillFresh = !pumpkingHandEffectAttempted
                && !pumpkingSummonEffectAttempted
                && !pumpkingSummonEffectResolved
                && !HasPumpkingInHand()
                && !HasSmallPumpkingOnField()
                && !HasPumpkingInGrave();

            // Army is the best searchable body while Call is face-up, especially
            // when the current route still needs free Level 6 monsters.
            if (HasFaceupCall() && !armySpecialSummonEffectCommittedThisTurn
                && NeedsAdditionalLevel6BodyForCurrentPlan())
            {
                priority.Add(CardId.ArmyOfTheHaunted);
            }

            ClientCard attackThreat = Enemy.GetMonsters().FirstOrDefault(c =>
                c.IsFaceup() && c.IsAttack() && !c.IsDisabled()
                && !c.IsShouldNotBeTarget());
            if (attackThreat != null)
                priority.Add(CardId.StareOfTheSnakeHair);

            // Mammoth is interaction only when Special Summoned by Pumpking or
            // Reverie. Bloom normally adds its target to the hand, where Mammoth
            // is a brick, so it is deliberately excluded from every Bloom search
            // priority even when an enemy field card exists.

            if (HasFaceupFieldSpell()
                && !Bot.HasInHand(CardId.DoomkingBalerdroch)
                && !Bot.HasInGraveyard(CardId.DoomkingBalerdroch)
                && !Bot.HasInMonstersZone(CardId.DoomkingBalerdroch, faceUp: true))
            {
                priority.Add(CardId.DoomkingBalerdroch);
            }

            if ((currentComboRoute == ComboRoute.EldlichRank10 || eldlichRouteActive)
                && !Bot.HasInHand(CardId.EldlichTheGoldenLord)
                && !Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                && !Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
            {
                priority.Add(CardId.EldlichTheGoldenLord);
            }

            if (pumpkingStillFresh)
                priority.Add(CardId.PumpkingTheKingOfGraveGhosts);

            priority.AddRange(new[]
            {
                CardId.ArmyOfTheHaunted,
                CardId.StareOfTheSnakeHair,
                CardId.ChangshiTheSpiridao,
                CardId.OfficiatingReverie,
                CardId.Hublot,
                CardId.EldlichTheGoldenLord,
                CardId.DoomkingBalerdroch
            });

            // Pumpking is deliberately the final fallback after its effects were
            // already spent this turn; never waste Bloom on a redundant copy first.
            priority.Add(CardId.PumpkingTheKingOfGraveGhosts);
            return priority.Distinct().ToArray();
        }

        private bool ShouldPreferVampireSuckerOverFlyingMary()
        {
            if (eldlichRouteRank10CommitPending
                || eldlichRouteMarySummoned
                || (eldlichRouteActive && HasFaceupFallenAngel()))
            {
                return false;
            }

            return CanPlanVampireSuckerBridge()
                && GetVampireSuckerMaterialPlan().Count == 2;
        }

        private bool ShouldFlyingMaryRevivePumpkingForComeback()
        {
            // Turn numbers 1 and 2 are always each player's first turn. Therefore
            // Duel.Turn > 2 means this is our second-or-later turn regardless of
            // whether the Bot went first or second. The relevant Pumpking effect is
            // its Special-Summon trigger, not the hand effect that Sets Call.
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
            // Snakehair's hand effect always bridges through Ectoplasmic first.
            // Ectoplasmic then searches the missing starter: Hublot when absent,
            // otherwise Pumpking. Do not require Pumpking to be missing here,
            // because Snakehair -> Ectoplasmic -> Hublot is also a valid opener.
            bool ectoplasmicHasStarterTarget =
                (!HasHublotInHand() && CheckRemainInDeck(CardId.Hublot) > 0)
                || (HasHublotInHand() && !HasPumpkingInHand()
                    && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0);

            return Bot.GetMonsterCount() == 0
                && !ectoplasmicSearchUsed
                && !Bot.HasInHand(CardId.EctoplasmicFortification)
                && Bot.HasInHand(CardId.StareOfTheSnakeHair)
                && CheckRemainInDeck(CardId.EctoplasmicFortification) > 0
                && ectoplasmicHasStarterTarget
                && DefaultCheckWhetherBotCanSearch();
        }

        private bool CanEctoplasmicReachPumpkingBeforeHublot()
        {
            return !HasPumpkingInHand()
                && Bot.GetMonsterCount() == 0
                && Bot.HasInHand(CardId.EctoplasmicFortification)
                && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0
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

        private int GetHublotSendTargetId(IList<ClientCard> cards)
        {
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

            // With no Field Spell, prepare the Eldlich engine. Recover it only when
            // an opposing floodgate/problem card makes the hand effect immediately useful.
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

            // Quicksilver must become Gravity Controller before Bloom resolves.
            if (Bot.GetMonsters().Any(c => c.IsCode(CardId.MercuriumTheLivingQuicksilver)))
                return false;

            // Do not lock before converting an already-available pair of Level 10s.
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
            CardId.GlowUpBloom,
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
            return ChangshiKnownDeckTargets.Any(id => CheckRemainInDeck(id) > 0);
        }

        private int GetChangshiMillTargetId(IList<ClientCard> cards)
        {
            // Changshi is allowed to send from hand or Deck, but spending a hand
            // card here is almost never worth it. Restrict every route decision to
            // candidates that the server explicitly reports in the Deck.
            List<ClientCard> deckCards = cards
                .Where(c => c != null && c.Location == CardLocation.Deck)
                .ToList();
            if (deckCards.Count == 0)
                return 0;

            if (ShouldStartAshReplayLine(deckCards))
            {
                DebugRoute("Changshi target: Ash Blossom from Deck (Urara route)");
                return CardId.AshBlossom;
            }

            bool hasLevel6ReviveTarget = Bot.Graveyard.Any(c => IsLevel6Zombie(c) && c.IsCanRevive()
                && !c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));

            // When Reverie is already in hand, keep it there so it can discard a
            // useful GY card and Special Summon itself. Army is the preferred Deck
            // mill because Samuel can revive it, leaving Army + Reverie for Great
            // Pumpking after Samuel detached the small Pumpking.
            if (Bot.HasInHand(CardId.OfficiatingReverie)
                && deckCards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Changshi target: Army from Deck; preserve hand Reverie extender");
                return CardId.ArmyOfTheHaunted;
            }

            // The normal Samuel-first route needs a Level 6 Zombie in the GY.
            // Hublot normally supplied Reverie; if it did not and Reverie is still
            // in the Deck, Changshi may send that Deck copy.
            if (!hasLevel6ReviveTarget
                && deckCards.Any(c => c.IsCode(CardId.OfficiatingReverie)))
            {
                DebugRoute("Changshi target: Reverie from Deck for Samuel revive");
                return CardId.OfficiatingReverie;
            }

            // Quicksilver fallback has no Hublot body. Army is the direct Level 6
            // extender while Call remains face-up.
            if (quicksilverLineActive && !HasHublotOnField() && HasFaceupCall()
                && deckCards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Changshi target: Army from Deck for Quicksilver fallback");
                return CardId.ArmyOfTheHaunted;
            }

            if (Duel.Turn >= 2
                && Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive())
                && deckCards.Any(c => c.IsCode(CardId.Mezuki)))
            {
                DebugRoute("Changshi target: Mezuki from Deck (turn 2+ extender)");
                return CardId.Mezuki;
            }

            // Bloom is only used before Pumpking's hand effect and only after all
            // required non-Zombie Extra Deck summons are complete.
            if (!pumpkingHandEffectAttempted
                && !activatedThisTurn.Contains(PumpkingHandMarker)
                && CanAcceptZombieLock()
                && DefaultCheckWhetherBotCanSearch()
                && deckCards.Any(c => c.IsCode(CardId.GlowUpBloom)))
            {
                DebugRoute("Changshi target: Glow-Up Bloom from Deck");
                return CardId.GlowUpBloom;
            }

            if (deckCards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Changshi target: Army from Deck fallback");
                return CardId.ArmyOfTheHaunted;
            }
            if (deckCards.Any(c => c.IsCode(CardId.EldlichTheGoldenLord)))
            {
                DebugRoute("Changshi target: Eldlich from Deck fallback");
                return CardId.EldlichTheGoldenLord;
            }
            if (deckCards.Any(c => c.IsCode(CardId.Mezuki)))
            {
                DebugRoute("Changshi target: Mezuki from Deck fallback");
                return CardId.Mezuki;
            }
            if (deckCards.Any(c => c.IsCode(CardId.DoomkingBalerdroch)))
                return CardId.DoomkingBalerdroch;
            if (deckCards.Any(c => c.IsCode(CardId.OfficiatingReverie)))
                return CardId.OfficiatingReverie;
            if (deckCards.Any(c => c.IsCode(CardId.GreatMammothOfTheNetherworld)))
                return CardId.GreatMammothOfTheNetherworld;
            if (deckCards.Any(c => c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)))
                return CardId.PumpkingTheKingOfGraveGhosts;
            if (deckCards.Any(c => IsHublot(c)))
                return CardId.Hublot;
            if (deckCards.Any(c => c.IsCode(CardId.StareOfTheSnakeHair)))
                return CardId.StareOfTheSnakeHair;

            return deckCards[0].Id;
        }

        private IList<ClientCard> SelectPumpkingDeckSummonTarget(
            IList<ClientCard> cards,
            int min,
            int max)
        {
            if (Duel.Player == 1)
            {
                // A Samuel -> Pumpking bridge already locked the intended
                // follow-up target. Honour that plan before recalculating generic
                // opponent-turn priorities from the possibly changed field state.
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

                // The activation guard normally prevents reaching this branch.
                // If the target disappeared while the chain was resolving, choose a
                // recovery body but never default to Hublot as planned interaction.
                DebugRoute("Pumpking opponent-turn target disappeared: forced recovery body");
                return SelectByIdPriority(cards, min, max,
                    CardId.ArmyOfTheHaunted,
                    CardId.OfficiatingReverie,
                    CardId.ChangshiTheSpiridao,
                    CardId.StareOfTheSnakeHair,
                    CardId.GreatMammothOfTheNetherworld,
                    CardId.Hublot);
            }

            return SelectByIdPriority(cards, min, max,
                CardId.ChangshiTheSpiridao,
                CardId.ArmyOfTheHaunted,
                CardId.OfficiatingReverie,
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

            ClientCard target = deckCards.FirstOrDefault(c => c.IsCode(selectedChangshiMillId))
                ?? deckCards.FirstOrDefault();
            if (target == null)
            {
                DebugRoute("BLOCK Changshi selection: no Deck candidate; never spend a hand card");
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

            // Never abandon a concrete in-engine action merely because the route
            // state looks old. Resolve Pumpking/Call/Changshi first, then reassess.
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
                // Great Pumpking's executor has a legal two-body fallback even
                // after the ideal Samuel-first route has been disrupted.
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

            // Do not let a generic Rank 6 cut in before the intended overlay.
            if (HasGreatPumpkingOnField()
                && HasSamuelOnField()
                && Bot.HasInExtra(CardId.TheUndyingLegion))
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

            // The route marker can survive after Great Pumpking/Samuel were used as
            // Link material or otherwise left the Extra Deck. Keeping that stale
            // marker blocks Sheridan, Lars, and Wollow in both Main Phases.
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

            // Two free Level 6 Zombies are already a guaranteed Great Pumpking.
            // When its search is still live and a small Pumpking remains in Deck,
            // resolve this line before Delta commits the board to Eldlich.
            bool liveGreatPumpkingSearch = Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                && !HasGreatPumpkingOnField()
                && !greatPumpkingSearchAttempted
                && !greatPumpkingSearchResolved
                && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0
                && DefaultCheckWhetherBotCanSearch();
            if (liveGreatPumpkingSearch && GetRank6Materials(true).Count >= 2)
                return true;

            // Once Great Pumpking exists, finish the Samuel half of the board.
            // Army is checked explicitly because its executor is below Delta.
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

            // Great Pumpking + Samuel is not the finished Pumpking board while
            // Undying is still available to overlay the Great Pumpking.
            if (HasGreatPumpkingOnField() && HasSamuelOnField()
                && Bot.HasInExtra(CardId.TheUndyingLegion))
            {
                return true;
            }

            // Delta also must not cut between concrete Pumpking actions that are
            // already available now. This is action-based, so a disrupted route
            // will release the hold instead of deadlocking for the rest of the turn.
            return HasImmediatePumpkingActionPending();
        }

        private bool ShouldHoldRank6ForFlyingMaryRank10()
        {
            if (!eldlichRouteRank10CommitPending)
                return false;

            // The commitment ends only after a Rank 10 Xyz is actually present.
            return !Bot.HasInMonstersZone(CardId.Varudras, faceUp: true)
                && !Bot.HasInMonstersZone(CardId.MercuriumTheLivingQuicksilver, faceUp: true);
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
                    || CheckRemainInDeck(CardId.EctoplasmicFortification) > 0);
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
                && (Bot.HasInExtra(CardId.Varudras)
                    || Bot.HasInExtra(CardId.MercuriumTheLivingQuicksilver));
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
                || CheckRemainInDeck(CardId.EldlichTheGoldenLord) > 0;
            if (!eldlichCanReachGrave)
                return false;

            // With a Zombie already face-up, Delta can make its Token first and
            // then pay Eldlich by sending Delta itself. Without a Zombie seed,
            // another Spell/Trap is required so Eldlich can enter the field before
            // Delta creates the Token. These are alternative routes, not cumulative
            // requirements.
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

            // Eldlich in the GY is not enough by itself. The route still needs a
            // Spell/Trap to send, a free Main Monster Zone, and the surviving
            // Zombie that will Link with Fallen Angel into Flying Mary.
            return Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                && HasUsefulEldlichCost()
                && HasOpenMainMonsterZone();
        }

        private bool ShouldStartAshReplayLine(IList<ClientCard> cards)
        {
            // Urara branch: Hublot + revived Pumpking + Changshi are present,
            // Army remains searchable, and the board cannot realistically convert
            // into the Eldlich/Flying Mary Rank 10 route. Pumpking may have reached
            // the hand through Hublot, Snakehair/Ectoplasmic, or another legal line.
            if (Duel.Turn != 1 || !HasFaceupCall() || !HasHublotOnField())
                return false;
            if (!Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true)
                || !Bot.HasInMonstersZone(CardId.ChangshiTheSpiridao, faceUp: true))
                return false;
            bool armyAvailable = Bot.HasInHand(CardId.ArmyOfTheHaunted)
                || Bot.HasInGraveyard(CardId.ArmyOfTheHaunted)
                || CheckRemainInDeck(CardId.ArmyOfTheHaunted) > 0;
            if (!cards.Any(c => c.IsCode(CardId.AshBlossom)) || !armyAvailable)
                return false;
            if (!Bot.HasInExtra(CardId.PumpkingTheGreatGhostKing)
                || !Bot.HasInExtra(CardId.OfficiatorOfDoomSamuel)
                || !Bot.HasInExtra(CardId.TheUndyingLegion))
                return false;

            // Use this replay branch only when the board cannot realistically
            // convert into the Eldlich/Flying Mary Rank 10 route.
            return !CanContinueEldlichRouteFromCurrentBoard();
        }

        private bool ShouldUseQuicksilverFallback()
        {
            if (zombieLockedThisTurn || !Bot.HasInExtra(CardId.GravityController))
                return false;
            if (pumpkingStarterSeenThisDuel || HasDirectPumpkingLineAvailable())
                return false;
            if (CheckRemainInDeck(CardId.GlowUpBloom) <= 0)
                return false;

            return Bot.GetMonsters().Count(c => c.IsFaceup() && c.Level == 10
                && !c.HasType(CardType.Xyz | CardType.Link)) >= 2;
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
            else if (ashReplayLineActive)
            {
                // Fixed Urara route: Changshi milled Ash specifically so Samuel
                // can revive it and Great Pumpking can return it to the hand.
                target = cards.FirstOrDefault(c => c.IsCode(CardId.AshBlossom));
                if (target == null)
                {
                    DebugRoute("ERROR Samuel Urara target prompt has no Ash Blossom");
                    return null;
                }
            }
            else
            {
                // Normal route: Samuel exists to restore a Level 6 body so the
                // remaining Level 6 plus that monster can become Great Pumpking.
                // Never revive Ash or an off-level monster in this branch.
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

            // Recovery route after disruption: if two Level 6 bodies made the
            // fallback Great Pumpking but no small Pumpking is accessible yet,
            // search the small Pumpking first. It Sets Call, revives itself, and
            // restarts Changshi -> Army -> Samuel before Eldlich is committed.
            bool needSmallPumpkingStarter = !pumpkingHandEffectAttempted
                && !HasPumpkingAccessible()
                && cards.Any(c => c != null
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (needSmallPumpkingStarter)
            {
                priority.Add(CardId.PumpkingTheKingOfGraveGhosts);
                DebugRoute("Great Pumpking search: recover route with small Pumpking first");
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

            List<ClientCard> enemies = GetEnemyFieldPriority(cards, true)
                .Where(c => c != null && c.Controller == 1)
                .Distinct()
                .Take(max)
                .ToList();

            List<ClientCard> result = new List<ClientCard>();

            // Two opposing problem cards always have priority over recycling one
            // of our own cards.
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
            else if (ashReplayLineActive)
            {
                // Own-only activation is reserved for the Urara recovery route.
                ClientCard ash = cards.FirstOrDefault(c => c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone
                    && c.IsFaceup()
                    && c.IsCode(CardId.AshBlossom));
                if (ash != null)
                    result.Add(ash);
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

            // Highest priority: when Reverie is already in the GY, detach an Xyz
            // monster used as material first. Reverie can replace the spent Xyz
            // body, so named Level 6 resources are more valuable to preserve.
            bool reverieInGrave = Bot.HasInGraveyard(CardId.OfficiatingReverie);
            if (reverieInGrave)
            {
                ordered.AddRange(cards.Where(c => c != null
                    && c.HasType(CardType.Xyz)
                    && !ordered.Contains(c)));
            }

            // Next, put the first small Pumpking into the GY. Once another small
            // Pumpking is already there, do not force an additional copy off the
            // current Xyz before the named grave-value materials below.
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

            // Everything else is expendable only after the explicit priorities.
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
            // Generic discard order for effects that do not have a route-specific
            // cost selector. Route-specific effects such as Reverie override this.
            int[] priority =
            {
                CardId.DoomkingBalerdroch,
                CardId.Mezuki,
                CardId.GlowUpBloom,
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
                CardId.MulcharmyPurulia,
                CardId.MulcharmyFuwalos,
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

            // Best cost: Mezuki immediately becomes an extender in the GY.
            if (card.IsCode(CardId.Mezuki))
                return 0;

            // These cards are deliberately sent only when their GY condition is
            // already live. Do not throw away an Eldlich/Doomking line blindly.
            if (card.IsCode(CardId.DoomkingBalerdroch))
                score = HasFaceupFieldSpell() ? 10 : 110;
            else if (card.IsCode(CardId.ArmyOfTheHaunted))
                score = HasFaceupCall() ? 15 : 105;
            else if (card.IsCode(CardId.EldlichTheGoldenLord))
                score = HasFaceupFieldSpell() ? 20 : 115;
            else if (card.IsCode(CardId.GlowUpBloom))
                score = CanAcceptZombieLock() ? 25 : 180;
            else if (card.IsCode(CardId.GreatMammothOfTheNetherworld))
                score = 55;
            else if (card.IsCode(CardId.ChangshiTheSpiridao))
                score = 70;
            else if (IsGenericHandTrap(card))
            {
                // Reverie must still be allowed to summon when Mezuki is absent.
                // Spent hand traps and spare Mulcharmy/Maxx/Ash/Imperm/Dominus are
                // acceptable costs before sacrificing a live combo starter.
                score = activatedThisTurn.Contains(card.Id) ? 30 : 80;
            }
            else
            {
                score = GetGenericHandDispositionScore(card, candidates);
            }

            // Spare copies are safer to discard regardless of category.
            if (copies > 1)
                score -= 45 + Math.Min(copies - 1, 3) * 10;

            // Preserve live Pumpking starters and the cards that secure Call/Field.
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

            List<ClientCard> ordered = cards
                .Where(c => c != null)
                .OrderBy(c => GetReverieDiscardScore(c, cards))
                .ThenByDescending(c => cards.Count(x => x != null && x.IsCode(c.Id)))
                .ThenBy(c => c.Id)
                .ToList();

            IList<ClientCard> selected = Util.CheckSelectCount(ordered, cards, min, max);
            if (selected != null)
            {
                DebugRoute("REVERIE DISCARD selected="
                    + string.Join(",", selected.Select(c => c.Id.ToString()).ToArray()));
            }
            return selected;
        }

        private bool IsGenericHandTrap(ClientCard card)
        {
            return card != null && card.IsCode(
                CardId.MulcharmyPurulia,
                CardId.MulcharmyFuwalos,
                CardId.MaxxC,
                CardId.AshBlossom,
                CardId.InfiniteImpermanence,
                CardId.DominusImpulse);
        }

        private bool HasGenericGraveyardValue(ClientCard card)
        {
            if (card == null)
                return false;
            if (card.IsCode(CardId.GlowUpBloom))
                return CanAcceptZombieLock();

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

            // The Murakumo-style safety net must be willing to give up a spare
            // copy. This is the key case that the Yubel report exposed.
            if (copies > 1)
                score -= 500 + Math.Min(copies - 1, 3) * 20;

            // A hand trap that already resolved this turn has the least remaining
            // value in the hand. Keep this above the generic duplicate rule.
            if (IsGenericHandTrap(card) && activatedThisTurn.Contains(card.Id))
                score -= 400;

            // Prefer cards that continue to function or become stronger in the GY.
            if (HasGenericGraveyardValue(card))
                score -= 180;

            // Redundant utility cards are safer than live engine starters.
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

            // Preserve the cards that start or secure the Pumpking route. A spare
            // copy can still move ahead of these through the duplicate modifier.
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

            // Satisfy only the mandatory minimum. Generic prompts should never
            // throw away extra cards merely because the server allows up to max.
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

            // Safety fallbacks select only the mandatory count. Card-specific
            // handlers still control effects that intentionally choose more cards.
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

            // Snakehair must be planned against a concrete Attack Position
            // monster. A monster effect that targets an opposing GY monster
            // (Dux -> Phalanx) is also a high-value target even when the generic
            // danger classifier does not know the archetype.
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

            // Pumpking is selected only after its exact Deck follow-up has been
            // locked. This removes the old "any opposing chain is useful" fallback
            // that spent Call on Upstart Goblin and later produced target=0.
            ClientCard pumpking = pool.FirstOrDefault(c => c.IsCode(
                CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking != null && !pumpkingSummonEffectAttempted)
            {
                if (snakehairTarget != null
                    && CheckRemainInDeck(CardId.StareOfTheSnakeHair) > 0)
                {
                    pendingSnakehairDisableTarget = snakehairTarget;
                    pendingMammothDestroyTarget = null;
                    DebugRoute("Call plan: Pumpking -> Snakehair target="
                        + snakehairTarget.Id);
                    return pumpking;
                }

                if (mammothTarget != null
                    && CheckRemainInDeck(CardId.GreatMammothOfTheNetherworld) > 0)
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

            // Army can Special Summon itself from the hand/GY while a face-up Call
            // exists. Do not consume it as Call's target until that once-per-turn
            // action has already been committed this turn.
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

            // Only disturb the opposing GY after our Changshi / Extra Deck
            // recycling objectives are unavailable.
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
            // Undying costs one or two materials, so proactive use is reserved for
            // a real boss/negate/floodgate or an Xyz whose existing materials are
            // also stripped by the Lua operation before it becomes our material.
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

                // First inspect the announced target, not only the chain source. If
                // Dux targets Phalanx (or another opposing effect targets a monster
                // in its GY), overlaying that target makes the effect lose it.
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

                // Undying is removal, not a negate. Do not spend it on a GY source
                // whose already-activated effect will resolve anyway.
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

                // Lua allows any opposing face-up Attack Position monster, not just
                // the chain source. Remove the best live boss while the window is open.
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
            // Mammoth needs another face-up Zombie or Call of the Haunted already
            // face-up. Do not revive it as "interaction" when its trigger would be dead.
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

            // Reverie revives an Xyz with no material. Great Pumpking still gives
            // its Special-Summon search immediately, while Samuel remains useful as
            // a body whose later trip to the GY recycles a monster. Undying,
            // Sheridan, Wollow, and the other Xyz lose the effect that justified
            // summoning them once their material count is zero, so do not revive
            // them merely for raw ATK/DEF.
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

            // Immediate interaction outranks generic recovery. Snakehair answers a
            // live attack-position monster effect; Mammoth removes the best field
            // problem only when its own summon trigger is live.
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

            // With no immediate interaction, the useful no-material Xyz revives
            // are Great Pumpking and Samuel. Do not revive Undying or another Xyz
            // whose relevant activated effect is dead without material.
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
            // Doomking revives itself during the Standby Phase while any Field Spell
            // is face-up. Spending Call on it in that state wastes the reusable trap
            // and removes a stronger Pumpking follow-up. Filter it from the legal
            // preference pool instead of merely lowering its priority.
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
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair
            });

            // If there is no opponent card, avoid prioritising Mammoth merely for ATK.
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
                .Where(c => !mezukiLevel6ExtensionPending || IsLevel6Zombie(c))
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

            // Prefer bodies whose Special Summon creates additional value, but do
            // not artificially stop at the four main-deck names. A properly
            // summoned Zombie boss in the GY may be the only body whose ATK can
            // legally cover the intended Samuel disable target.
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

        private List<ClientCard> GetSamuelMaterials()
        {
            if (ashReplayLineActive)
            {
                // Confirmed Urara route: Army + Changshi is the preferred pair.
                // If Army was interrupted or has not reached the field, do not pass
                // with two legal Level 6 bodies: keep Changshi and use the cheapest
                // remaining Level 6 Zombie (normally Reverie).
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

            // Legal fallback for non-ideal boards: preserve Samuel and never put
            // the small Pumpking back under an Xyz after Samuel detached it.
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
            // Kept as the route-facing name used by older helpers. Material value
            // no longer depends on Reverie being in the GY: every Zombie Xyz with
            // zero material is already spent and may be converted immediately.
            return IsSpentZombieXyz(card);
        }

        private bool IsAllowedZombieLinkMaterial(ClientCard card)
        {
            if (card == null || !card.IsFaceup() || !IsZombie(card)
                || card.HasType(CardType.Link))
            {
                return false;
            }

            // Every zero-material Zombie Xyz is spent, including Samuel, Sheridan,
            // Great Pumpking, or Undying. Preserve only Xyz monsters that still
            // have material and therefore still have usable effects.
            if (card.HasType(CardType.Xyz))
                return IsSpentZombieXyz(card);

            return true;
        }

        private int GetZombieLinkMaterialValue(ClientCard card)
        {
            if (IsDeltaToken(card)) return 0;
            // All zero-material Zombie Xyz monsters are equally spent. Use them
            // before any Main Deck body that still carries summon/setup value.
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

            // Fallen is mandatory for this route. The second material follows the
            // global policy: Delta Token first, then Samuel/spent Xyz with Reverie
            // in GY, and Level 6 bodies only as the final fallback.
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

            // Prefer a column that is not directly opposite a set enemy card.
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

        // =====================================================================
        // Generic cards and interaction
        // =====================================================================

        private bool MulcharmyPuruliaActivate()
        {
            if (IsCardEffectNegated())
                return false;
            return Duel.Player == 1;
        }

        private bool MulcharmyFuwalosActivate()
        {
            if (!CanUseWindMonsterEffects() || IsCardEffectNegated())
                return false;
            return Duel.Player == 1;
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
            if (IsCardEffectNegated())
                return false;

            // This deck repeatedly needs LIGHT Eldlich and EARTH Mezuki effects.
            // Activating Dominus from the hand applies a permanent LIGHT/EARTH/WIND
            // activation lock, which is far more damaging than stopping an ordinary
            // extender such as Phalanx. Hold the hand copy and Set it on our turn;
            // a Set Dominus keeps the negate without accepting that lock.
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
            if (IsCardEffectNegated() || Duel.LastChainPlayer != 1)
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

            // The tails result banishes all our monsters, so only gamble readily
            // with a small board or against a genuinely important activation.
            return ourZombieCount <= 1 || emergency;
        }

        private bool DoomkingBalerdrochActivate()
        {
            if (IsCardEffectNegated())
                return false;

            // Standby Phase self-revival.
            if (Card.Location == CardLocation.Grave)
                return HasFaceupFieldSpell() && HasOpenMainMonsterZone();

            ClientCard last = Util.GetLastChainCard();
            if (last == null || last.IsCode(CardId.DoomkingBalerdroch))
                return false;

            // Capture the desired option now. At the later SelectOption prompt,
            // LastChainPlayer may already point at Doomking itself rather than the
            // opponent's original Zombie effect.
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
            if (IsCardEffectNegated())
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

            // Same effect-description split used by YubelExecutor: StringId 1 is
            // shared by the quick negate and battle-start trigger. A live opponent
            // chain identifies the negate branch.
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
                ClearVarudrasDestroyPlan();
                varudrasNegatedChainSource = last;
                SetStrategicPlan(StrategicGoal.NegateImmediateThreat,
                    currentComboRoute, "Varudras hard negate");
                ClearEnemyCommitment("opponent chain started");
                DebugRoute("ACCEPT Varudras hard negate id="
                    + (last != null ? last.Id : 0));
                return true;
            }

            // Battle-start destroy and the trigger after Varudras is destroyed are
            // optional. Record the target prompt now because solving-chain metadata
            // is not reliable when the subsequent HINTMSG_DESTROY prompt arrives.
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
            if (IsCardEffectNegated() || Duel.LastChainPlayer != 1)
                return false;
            return Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeTarget())
                || Enemy.GetSpells().Any(c => c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeTarget());
        }

        // =====================================================================
        // Starters and Main Deck engine
        // =====================================================================

        private bool TerraformingActivate()
        {
            if (IsCardEffectNegated() || !DefaultCheckWhetherBotCanSearch())
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
            if (IsCardEffectNegated())
                return false;

            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                bool createToken = ShouldCreateDeltaTokenNow();
                DebugRoute(createToken
                    ? "ACCEPT Delta Token: planned Flying Mary/Sucker Link material"
                    : "HOLD Delta Token: preserve Main Monster Zone");
                return createToken;
            }

            // A ready Rank 6 Pumpking line has priority over committing Delta.
            // In particular, Hublot + Reverie should become Great Pumpking, search
            // the small Pumpking, and continue that combo before Eldlich starts.
            if (CanTakeProductivePumpkingActionNow())
            {
                DebugRoute("HOLD Delta activation: complete available Hublot/Pumpking action first");
                return false;
            }

            // Do not open Delta merely to send Eldlich and stop. Route readiness
            // accepts either a face-up Zombie (Token first, then Delta pays Eldlich)
            // or a separate Spell/Trap (Eldlich first, then Token).
            if (!CanStartEldlichRoute())
            {
                DebugRoute("HOLD Delta activation: no complete Eldlich/Flying Mary route available");
                return false;
            }

            // Field/Eldlich must be established before any optional Bloom lock.
            SelectSTPlace(Card, true);
            return true;
        }

        private bool EctoplasmicFortificationActivate()
        {
            if (IsCardEffectNegated())
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
            if (Card.Location != CardLocation.Hand || IsCardEffectNegated())
                return false;

            // Hard route rule: Snakehair from the hand searches Ectoplasmic
            // Fortification first. Ectoplasmic then chooses Pumpking or Hublot
            // from the actual hand/Deck state. Having Pumpking already must not
            // redirect Snakehair into Call or Vortex.
            if (ectoplasmicSearchUsed
                || Bot.HasInHand(CardId.EctoplasmicFortification)
                || CheckRemainInDeck(CardId.EctoplasmicFortification) <= 0)
            {
                return false;
            }

            bool ectoplasmicHasStarterTarget =
                (!HasHublotInHand() && CheckRemainInDeck(CardId.Hublot) > 0)
                || (HasHublotInHand() && !HasPumpkingInHand()
                    && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0);
            if (!ectoplasmicHasStarterTarget)
                return false;

            DebugRoute("ACCEPT Snakehair hand effect: search Ectoplasmic first");
            return DefaultCheckWhetherBotCanSearch();
        }

        private bool StareOfTheSnakeHairFieldActivate()
        {
            if (Card.Location != CardLocation.MonsterZone || IsCardEffectNegated())
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
                || IsCardEffectNegated()
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

            // The hand effect immediately sets Call, Call revives Pumpking, and
            // Pumpking then needs a second free Main Monster Zone for its Deck
            // summon. With only one zone this line merely clogs the last slot.
            if (Duel.Player == 0 && GetOpenMainMonsterZoneCount() < 2)
            {
                DebugRoute("HOLD Pumpking hand effect: need two open Main Monster Zones for Call + Deck summon");
                return false;
            }

            // Hublot is the ideal Normal Summon. Its executor is registered first,
            // so this guard is only needed when the same idle prompt is re-queried.
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
            if (Card.Location != CardLocation.MonsterZone || IsCardEffectNegated())
                return false;
            if (!HasOpenMainMonsterZone() || pumpkingSummonEffectAttempted)
                return false;

            if (Duel.Player == 1)
            {
                bool hasSnakehairLine = CheckRemainInDeck(CardId.StareOfTheSnakeHair) > 0
                    && ((pendingSnakehairDisableTarget != null
                            && IsLiveEnemyMonster(pendingSnakehairDisableTarget)
                            && pendingSnakehairDisableTarget.IsAttack())
                        || Enemy.GetMonsters().Any(ShouldPreemptWithSnakehair));
                bool hasMammothLine = CheckRemainInDeck(CardId.GreatMammothOfTheNetherworld) > 0
                    && ((pendingMammothDestroyTarget != null
                            && IsLiveEnemyFieldCard(pendingMammothDestroyTarget)
                            && !pendingMammothDestroyTarget.IsShouldNotBeTarget())
                        || GetMammothPreemptTarget() != null);

                // Do not convert Pumpking into a setup-only Hublot on the
                // opponent's turn. Use this once-per-turn trigger only when the
                // summoned monster produces immediate interaction.
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
            // Once Great Pumpking is on the field, let its delayed search finish
            // before committing Foolish. The search result (normally Army in this
            // route) determines what the graveyard still actually needs.
            if (greatPumpkingSearchWindowPending
                || (greatPumpkingSearchAttempted && !greatPumpkingSearchResolved)
                || (HasGreatPumpkingOnField() && !greatPumpkingSearchResolved))
            {
                return true;
            }

            // Once Great Pumpking has searched Army, finish the concrete
            // Army -> Samuel -> Great Pumpking/Undying sequence before deciding
            // what Foolish should send. Otherwise Bloom is spent to search a card
            // the active route already supplies for free.
            if (CanTakeProductivePumpkingExtraDeckStep())
                return true;

            // In the Urara replay route the exact Hublot + small Pumpking pair is
            // already ready to make Great Pumpking. Do not fire Foolish in the idle
            // window between Changshi resolving and that Xyz Summon.
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
                ? CheckRemainInDeck(cardId) > 0
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

        private bool CanUseFoolishBloomAsEmergencyStarter()
        {
            if (glowUpBloomEffectCommittedThisTurn
                || activatedThisTurn.Contains(CardId.GlowUpBloom)
                || !CanAcceptZombieLock()
                || !DefaultCheckWhetherBotCanSearch()
                || !HasLegalBloomSearchTargetInDeck()
                || currentComboRoute == ComboRoute.BrickEldlich
                || currentComboRoute == ComboRoute.EldlichRank10
                || eldlichRouteActive
                || eldlichRouteMarySummoned
                || eldlichRouteRank10CommitPending)
            {
                return false;
            }

            // Bloom is an emergency starter, not generic graveyard value. Use it
            // only when the current Pumpking route was disrupted and is missing a
            // body, or when the hand has no executable Pumpking/Eldlich route.
            bool disruptedMissingPiece = IsPumpkingComboInProgress()
                && NeedsAdditionalLevel6BodyForCurrentPlan()
                && !CanTakeProductivePumpkingActionNow();
            bool deadHand = !HasDirectPumpkingLineAvailable()
                && !CanTakeProductivePumpkingActionNow()
                && !CanStartEldlichRoute()
                && !CanContinueEldlichRouteFromCurrentBoard()
                && currentStrategicGoal == StrategicGoal.None;

            return disruptedMissingPiece || deadHand;
        }

        private bool CanUseFoolishForDoomkingValue()
        {
            // Under a face-up Field Spell, Doomking will revive itself in the next
            // Standby Phase. Without that concrete payoff it is not worth spending
            // the reserve Foolish Burial merely to park a card in the GY.
            return HasFaceupFieldSpell()
                && !Bot.HasInGraveyard(CardId.DoomkingBalerdroch)
                && !Bot.HasInMonstersZone(CardId.DoomkingBalerdroch, faceUp: true);
        }

        private int GetFoolishBurialPlannedTargetId(
            IEnumerable<ClientCard> candidates = null)
        {
            // First repair a concrete missing combo piece.
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

            // Bloom is allowed only as the explicit emergency starter described
            // above. It is never a generic fallback after the main combo finished.
            if (IsFoolishCandidateAvailable(CardId.GlowUpBloom, candidates)
                && CanUseFoolishBloomAsEmergencyStarter())
            {
                return CardId.GlowUpBloom;
            }

            if (IsFoolishCandidateAvailable(CardId.DoomkingBalerdroch, candidates)
                && CanUseFoolishForDoomkingValue())
            {
                return CardId.DoomkingBalerdroch;
            }

            return 0;
        }

        private string GetFoolishBurialPlanReason(int targetId)
        {
            if (targetId == CardId.Mezuki)
                return "supply missing Level 6 extender";
            if (targetId == CardId.EldlichTheGoldenLord)
                return "supply missing Eldlich route piece";
            if (targetId == CardId.GlowUpBloom)
                return "emergency starter after dead/disrupted line";
            if (targetId == CardId.DoomkingBalerdroch)
                return "prepare Doomking self-revive under Field Spell";
            return "no profitable target";
        }

        private bool FoolishBurialActivate()
        {
            if (IsCardEffectNegated() || HasImmediatePumpkingActionPending())
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

            // Do not use Foolish before an unused Hublot Normal Summon. Hublot is
            // the starter and its own mill can prepare the required GY resource.
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
            if (IsCardEffectNegated() || !HasOpenMainMonsterZone() || summonCount <= 0)
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

            // These are the deliberately expendable low-Level Zombies registered
            // for this fallback. Hublot and every Pumpking starter are handled by
            // their own higher-priority executors and are never spent as a brick.
            if (!Card.IsCode(CardId.GlowUpBloom, CardId.Mezuki, CardId.AshBlossom))
                return false;

            DebugRoute("ACCEPT brick Zombie Normal Summon id=" + Card.Id
                + " to seed Eldlich/Flying Mary");
            return true;
        }

        private bool HublotActivate()
        {
            if (IsCardEffectNegated())
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

            // Decline the trigger caused by Call reviving Pumpking. Pumpking must
            // resolve first and summon Changshi. In the normal route Hublot's Xyz
            // trigger is saved for the monster revived by Samuel.
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

            // The purpose of the banished effect is to reload a spent Xyz. A
            // zero-material Xyz always beats adding another material to a live one.
            int score = materials == 0 ? 10000 : -materials * 1000;

            // Among equally spent Xyz, prefer effects that can create value during
            // our current turn. Undying is deliberately last because its attach
            // effect matters only on the opponent's turn.
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
            if (IsCardEffectNegated())
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player != 0
                    || HasImmediatePumpkingActionPending()
                    || HasConfirmedOpenStateBattleLethal())
                {
                    return false;
                }
                // Reverie only needs any other discardable card. Mezuki is the
                // preferred cost, never a requirement for activating this extender.
                if (Bot.Hand.Count <= 1 || !HasSurplusMainMonsterZoneForReverie())
                    return false;

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
            if (IsCardEffectNegated())
                return false;

            if ((Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
                && HasFaceupCall())
            {
                // Use exactly the same executability test as the Mezuki gate.
                // This prevents "wait for Army" from remaining true when Army's
                // own executor would decline the Special Summon.
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

            // In the Urara route Army is legal only after Great Pumpking's search
            // resolves. Outside that route it must not cut across a concrete
            // Pumpking/Call/Changshi action. Most importantly, the gate is true
            // only when the current plan still needs the Level 6 body.
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
            if (IsCardEffectNegated() || !HasOpenMainMonsterZone())
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
                // Pumpking's freshly Set Call is the dedicated combo bridge. It
                // remains the one own-turn exception to the generic open-state
                // rule because the entire Call -> Pumpking -> Deck summon line is
                // planned as one route.
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

                // Lethal/value Call is not an interrupt. Never chain it into our
                // own Great Pumpking search, bounce, Fallen Angel, or any other
                // friendly effect. Re-evaluate only after the board is stable.
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

            // On the opponent's turn, never flip Call in an empty chain merely
            // because a legal monster exists. Wait for an opposing commitment and
            // revive only an interaction body; Army is not an interrupt target.
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

            // Urara route is not automatically Mezuki-free. Great Pumpking must
            // stay separate, while Samuel still needs two free Level 6 Zombies.
            // Army normally supplies one of them; when Army was unavailable,
            // negated, removed, or did not leave enough bodies, Mezuki may repair
            // the route after Army has had its Special-Summon opportunity.
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

            // Later-turn comeback: reviving an unused small Pumpking is itself the
            // Level 6 body that restarts the Pumpking/Changshi sequence.
            return !HasSmallPumpkingOnField()
                && !pumpkingSummonEffectAttempted
                && Bot.Graveyard.Any(c => c != Card
                    && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts)
                    && c.IsCanRevive());
        }

        private bool MezukiActivate()
        {
            if (!CanUseEarthMonsterEffects() || IsCardEffectNegated())
                return false;
            if (Card.Location != CardLocation.Grave || !HasOpenMainMonsterZone())
                return false;
            if (HasImmediatePumpkingActionPending())
                return false;

            // Army is a no-cost Level 6 body under face-up Call. Never banish
            // Mezuki before Army has used (or lost) that Special-Summon action.
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
                && IsWorthwhileMezukiReviveTarget(c)
                && IsLevel6Zombie(c));
            if (!hasLevel6Target)
            {
                DebugRoute("HOLD Mezuki: no worthwhile Level 6 Zombie target");
                return false;
            }

            mezukiReviveSelectionPending = true;
            mezukiLevel6ExtensionPending = true;
            DebugRoute("ACCEPT Mezuki: planned Level 6 body is still missing");
            return true;
        }

        private bool GlowUpBloomActivate()
        {
            if (IsCardEffectNegated()
                || Card.Location != CardLocation.Grave
                || glowUpBloomEffectCommittedThisTurn)
            {
                return false;
            }
            if (!DefaultCheckWhetherBotCanSearch())
                return false;

            bool acceptBloom;

            // A Varudras self-pop on the opponent's turn is deliberate value: the
            // Zombie-only restriction expires in that End Phase before our turn.
            if (Duel.Player == 1)
            {
                acceptBloom = HasLegalBloomSearchTargetInDeck();
                if (acceptBloom)
                    glowUpBloomEffectCommittedThisTurn = true;
                return acceptBloom;
            }

            // A Bloom used as the emergency Normal-Summoned Zombie must remain a
            // silent Link seed. Taking its search here would Zombie-lock the Duel
            // before the Eldlich route can Xyz Varudras/Quicksilver.
            if (currentComboRoute == ComboRoute.BrickEldlich
                || currentComboRoute == ComboRoute.EldlichRank10
                || eldlichRouteActive || eldlichRouteMarySummoned)
            {
                DebugRoute("HOLD Bloom: preserve non-Zombie Rank 10 finish");
                return false;
            }

            if (quicksilverLineActive)
            {
                // Gravity Controller has already sent Quicksilver and Bloom to the GY.
                acceptBloom = !Bot.GetMonsters().Any(c =>
                    c.IsCode(CardId.MercuriumTheLivingQuicksilver));
            }
            else
            {
                acceptBloom = CanAcceptZombieLock();
            }

            if (acceptBloom)
                glowUpBloomEffectCommittedThisTurn = true;
            return acceptBloom;
        }
        private bool ChangshiTheSpiridaoActivate()
        {
            if (IsCardEffectNegated())
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
            if (!CanUseLightMonsterEffects() || IsCardEffectNegated())
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

                // On later turns, establish Vampire Sucker before Eldlich returns
                // so the Special Summon from the GY converts into a draw.
                if (!Bot.HasInMonstersZone(CardId.VampireSucker, faceUp: true)
                    && ShouldPreferVampireSuckerOverFlyingMary())
                {
                    DebugRoute("HOLD Eldlich GY effect: Link Vampire Sucker first");
                    return false;
                }

                if (!HasOpenMainMonsterZone() || !HasSafeEldlichGraveFieldCost())
                {
                    DebugRoute("HOLD Eldlich GY effect: no safe exact field S/T cost");
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool GreatMammothActivate()
        {
            if (IsCardEffectNegated())
                return false;

            if (pendingMammothDestroyTarget != null)
            {
                if (IsLiveEnemyFieldCard(pendingMammothDestroyTarget)
                    && !pendingMammothDestroyTarget.IsShouldNotBeTarget())
                {
                    DebugRoute("ACCEPT Mammoth pre-emptive trigger target="
                        + pendingMammothDestroyTarget.Id);
                    return true;
                }
                pendingMammothDestroyTarget = null;
            }

            if (Duel.Player == 1)
            {
                ClientCard target = GetMammothPreemptTarget();
                if (target == null)
                    return false;

                pendingMammothDestroyTarget = target;
                DebugRoute("ACCEPT Mammoth opponent-turn trigger target="
                    + target.Id);
                return true;
            }

            return GetEnemyFieldPriority().Count > 0;
        }

        // =====================================================================
        // Extra Deck summon decisions
        // =====================================================================
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

            // Normal route: Samuel first, detach Pumpking, revive a Level 6, then
            // Hublot + the revived monster become Great Pumpking.
            List<ClientCard> comboMaterials = GetGreatPumpkingMaterials();
            if (comboMaterials.Count == 2)
            {
                AI.SelectMaterials(comboMaterials);
                DebugRoute("ACCEPT normal-route Great Pumpking after Samuel revive");
                return true;
            }

            // If Samuel cannot still leave a third Level 6 body for Great Pumpking,
            // do not consume the only two monsters. Make Great Pumpking first as the
            // safe fallback instead.
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

            // These two cards have explicit Reposition rules before battle. Other
            // Defence Position monsters are not assumed to be legally movable.
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
                    && c.IsOnField());
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

            // Wollow is the power choice when its graveyard scaling converts a
            // non-lethal clear board into lethal damage.
            if (BattlePhaseIsAvailableThisTurn()
                && Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0
                && currentDamage < Enemy.LifePoints
                && projectedDamage >= Enemy.LifePoints)
            {
                return true;
            }

            // If Sheridan has no legal target (for example an untargetable boss),
            // use Wollow only when the boost actually lets one of our attackers
            // cross the opposing battle threshold.
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

            // Do not compress two attackers when the current field already has a
            // clean, confirmed lethal line.
            if (HasConfirmedLethalWithoutRank6Overlay())
                return SurplusRank6Plan.None;

            // Sheridan is the removal choice whenever a legal opposing field card
            // should be sent away now.
            if (Bot.HasInExtra(CardId.DhampirVampireSheridan)
                && GetSheridanSurplusRemovalTarget() != null)
            {
                return SurplusRank6Plan.SheridanRemoval;
            }

            // Wollow is selected only when its actual graveyard boost changes the
            // battle result; it is not a generic graveyard-matchup fallback.
            if (Bot.HasInExtra(CardId.WollowFounderOfTheDrudgeDragons)
                && WouldWollowPowerMatter(materials))
            {
                return SurplusRank6Plan.WollowPower;
            }

            // With no immediate removal or power finish, a non-lethal board should
            // become Lars so the leftover Level 6 bodies provide a negate next turn.
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
            else if (!Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive()))
            {
                return false;
            }

            List<ClientCard> materials = GetSamuelMaterials();
            if (materials.Count != 2)
                return false;

            if (!ashReplayLineActive
                && !CanNormalSamuelFirstReachGreatPumpking(materials))
            {
                DebugRoute("HOLD Samuel summon: would leave fewer than two Level 6 bodies for Great Pumpking");
                return false;
            }

            AI.SelectMaterials(materials);
            DebugRoute("ACCEPT Xyz Samuel with " + materials[0].Id + "," + materials[1].Id);
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

            ClientCard greatPumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.PumpkingTheGreatGhostKing));
            if (greatPumpking == null)
                return false;

            if (HasSafeGreatPumpkingBounceTarget()
                && !greatPumpkingBounceAttempted
                && greatPumpking.Overlays.Count > 0)
            {
                return false;
            }

            // A negated bounce must not force an End Phase pass. The route waits
            // until the bounce has at least been attempted, then overlays Undying.
            if (ashReplayLineActive && !Bot.HasInHand(CardId.AshBlossom)
                && !greatPumpkingBounceAttempted)
            {
                return false;
            }

            AI.SelectMaterials(new List<ClientCard> { greatPumpking });
            DebugRoute("ACCEPT overlay Undying on Great Pumpking");
            return true;
        }

        private bool VarudrasSummon()
        {
            RecalculateStrategicPlan("evaluate Varudras extension");
            if (!CanBuildSurplusVarudras())
                return false;

            List<ClientCard> materials = GetRank10Materials();
            if (materials.Count != 2)
                return false;

            AI.SelectMaterials(materials);
            DebugRoute("ACCEPT Varudras as surplus/confirmed Eldlich Rank 10 finish");
            return true;
        }

        private bool MercuriumSummon()
        {
            RecalculateStrategicPlan("evaluate Quicksilver fallback");
            bool finishingEldlichRank10 = eldlichRouteRank10CommitPending
                && !Bot.HasInExtra(CardId.Varudras);
            if (!finishingEldlichRank10
                && (currentStrategicGoal != StrategicGoal.BuildQuicksilverFallback
                    || currentComboRoute != ComboRoute.QuicksilverFallback
                    || !ShouldUseQuicksilverFallback()))
            {
                return false;
            }

            List<ClientCard> materials = GetRank10Materials();
            if (materials.Count != 2)
                return false;

            AI.SelectMaterials(materials);
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

            // The full route explicitly tributes Eldlich itself:
            // Eldlich returns from the GY, Special Summons, then becomes the
            // release for Fallen Angel while another Zombie remains for Link 2.
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
            // Later-turn board reset: a live small Pumpking plus a spent Zombie
            // Xyz can become Flying Mary. Pumpking is then in the GY when Mary's
            // ignition effect is checked, so Mary revives it and restarts the
            // Pumpking summon trigger instead of leaving two dead bodies on board.
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

            // Eldlich route: Fallen Angel plus one Zombie. When the confirmed
            // Pumpking continuation exists, Samuel is the preferred second material.
            if (Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
            {
                List<ClientCard> eldlichMaterials = GetFlyingMaryEldlichMaterials();
                if (eldlichMaterials.Count == 2)
                {
                    AI.SelectMaterials(eldlichMaterials);
                    return true;
                }
            }

            // Turn 2+ utility line: Link two expendable Zombies into Mary, then
            // revive Pumpking to restart the combo. A spent Xyz (or Samuel) is
            // allowed only while Reverie is in the GY, so the lost body is replaced.
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

            // Never replace the intended turn-1 end board (Undying + Samuel).
            if (Duel.Turn <= 1 || IsPumpkingComboInProgress())
                return false;

            // Later-turn Eldlich value line: make Sucker first, then let Eldlich's
            // GY effect Special Summon itself and trigger Sucker's draw. This is
            // preferred over generic Flying Mary unless Fallen Angel already
            // confirms the dedicated Mary/Rank 10 route.
            if (ShouldPreferVampireSuckerOverFlyingMary())
            {
                List<ClientCard> drawMaterials = GetVampireSuckerMaterialPlan();
                if (drawMaterials.Count == 2)
                {
                    AI.SelectMaterials(drawMaterials);
                    DebugRoute("ACCEPT Vampire Sucker before Eldlich GY revival with value-approved materials");
                    return true;
                }
            }

            // Existing turn-2+ cleanup line used to clear a crowded board and
            // restart Pumpking through a recoverable Samuel/Reverie package.
            bool pumpkingLoopAvailable = HasDirectPumpkingLineAvailable()
                || HasPumpkingInGrave();
            bool boardNeedsCleanup = Bot.GetMonsterCount() >= 3;
            if (!pumpkingLoopAvailable || !boardNeedsCleanup)
                return false;

            if (!Bot.HasInGraveyard(CardId.OfficiatingReverie)
                || Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
            {
                return false;
            }

            ClientCard samuel = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.OfficiatorOfDoomSamuel));
            ClientCard otherZombie = Bot.GetMonsters()
                .Where(c => c != samuel && IsAllowedZombieLinkMaterial(c))
                .OrderBy(GetZombieLinkMaterialValue)
                .ThenBy(GetMaterialValue)
                .FirstOrDefault();
            if (samuel == null || otherZombie == null)
                return false;

            AI.SelectMaterials(new List<ClientCard> { samuel, otherZombie });
            return true;
        }

        private bool GravityControllerSummon()
        {
            ClientCard quicksilver = Bot.MonsterZone.Skip(5)
                .FirstOrDefault(c => c != null && c.IsFaceup()
                    && c.IsCode(CardId.MercuriumTheLivingQuicksilver));
            if (quicksilver == null)
                return false;

            // Hard rule: Gravity Controller is never made from any other Extra Deck monster.
            if (!quicksilver.Overlays.Contains(CardId.GlowUpBloom) && !quicksilverLoadedBloom)
                return false;

            AI.SelectMaterials(new List<ClientCard> { quicksilver });
            return true;
        }

        // =====================================================================
        // Extra Deck effects
        // =====================================================================
        private bool PumpkingGreatGhostKingActivate()
        {
            if (IsCardEffectNegated())
                return false;

            int searchDescription = Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 1);
            int bounceDescription = Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 2);
            bool explicitBounce = ActivateDescription == bounceDescription;
            bool searchWindow = ActivateDescription == searchDescription
                || (greatPumpkingSearchWindowPending && !explicitBounce);

            // Lua: the search is the delayed EVENT_SPSUMMON_SUCCESS trigger. Some
            // MDPro3 builds report that action with description -1, so the verified
            // summon window is the source of truth.
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
                // In the Urara line, do not spend this effect before Samuel has
                // revived Ash. Otherwise Ash can never be recovered to the hand.
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
            if (IsCardEffectNegated())
                return false;

            // Both the send-to-GY ignition effect and the revive trigger are useful.
            if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                return true;
            return HasOpenMainMonsterZone() && Enemy.Graveyard.Any(c => c.IsMonster());
        }

        private bool OfficiatorOfDoomSamuelActivate()
        {
            if (IsCardEffectNegated())
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

            // In the fixed Urara route, do not spend Samuel unless Ash is still a
            // legal revive target.
            if (ashReplayLineActive)
            {
                if (!reviveTargets.Any(c => c.IsCode(CardId.AshBlossom)))
                {
                    DebugRoute("HOLD Samuel revive: Urara route has no legal Ash target");
                    return false;
                }
            }
            else
            {
                // Normal route uses Samuel only when a Level 6 remains on the field
                // and another Level 6 can be revived to complete Great Pumpking.
                if (CountFreeLevel6ForGreatPumpking() < 1
                    || !reviveTargets.Any(c => IsLevel6Zombie(c)
                        && !c.IsCode(CardId.AshBlossom)))
                {
                    DebugRoute("HOLD Samuel revive: cannot complete Great Pumpking");
                    return false;
                }
            }

            pendingInterruptMode = InterruptMode.Hold;
            plannedSamuelReviveId = 0;
            plannedSamuelReviveResolved = false;
            samuelOpponentNegatePending = false;
            samuelNegateTarget = null;
            samuelReviveSelectionPending = true;
            samuelFieldEffectCommittedThisTurn = true;
            selectedSamuelReviveId = 0;
            DebugRoute(ashReplayLineActive
                ? "ACCEPT Samuel revive: lock Ash Blossom target"
                : "ACCEPT Samuel revive: Level 6 continuation");
            return true;
        }

        private bool WollowActivate()
        {
            if (IsCardEffectNegated() || Enemy.Graveyard.Count == 0
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
            bool reactingToEnemyGrave = lastChainCard != null
                && (lastChainCard.Location == CardLocation.Grave
                    || lastChainCard.Location == CardLocation.Removed);
            bool lateWindow = Duel.Phase >= DuelPhase.Main2;

            // Do not fire immediately after Xyz Summon in Main Phase 1 merely
            // because Wollow is a Quick Effect. Use it while answering an enemy
            // GY action, or bank the interaction until Main Phase 2 / End Phase.
            bool shouldActivate = reactingToEnemyGrave || lateWindow;
            DebugRoute(shouldActivate
                ? "ACCEPT Wollow at reactive/late window"
                : "HOLD Wollow: preserve Quick Effect for meaningful timing");
            return shouldActivate;
        }

        private bool TheUndyingLegionActivate()
        {
            if (IsCardEffectNegated() || Duel.Player != 1)
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

        private bool MercuriumActivate()
        {
            if (IsCardEffectNegated())
                return false;

            // The fallback line needs the Xyz-Summon trigger to load Bloom.
            if (quicksilverLineActive && Card.Location == CardLocation.MonsterZone)
                return CheckRemainInDeck(CardId.GlowUpBloom) > 0;

            return true;
        }

        private bool FallenAngelActivate()
        {
            bool activate = Card.Location == CardLocation.Grave && !IsCardEffectNegated();
            if (activate && eldlichRouteRank10CommitPending)
                DebugRoute("ACCEPT Fallen Angel GY trigger: summon Mad Golden for Rank 10 line");
            return activate;
        }

        private bool EldlichTheMadGoldenLordActivate()
        {
            if (IsCardEffectNegated())
                return false;
            return Bot.GetMonsters().Any(c => c != Card && IsZombie(c))
                && Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsShouldNotBeTarget());
        }

        private bool FlyingMaryActivate()
        {
            if (IsCardEffectNegated())
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
            if (IsCardEffectNegated())
                return false;

            // Never use Sucker's ignition effect that revives an opponent's
            // monster. It gives the opponent a body and is outside this deck's
            // intended line. Keep accepting the separate mandatory draw trigger.
            if (ActivateDescription == Util.GetStringId(CardId.VampireSucker, 0))
            {
                DebugRoute("HOLD Vampire Sucker opponent-GY revive effect");
                return false;
            }

            return true;
        }

        // =====================================================================
        // Selection logic
        // =====================================================================

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

            // Reverie's banished effect uses HINTMSG_TARGET in Lua, not an Xyz-
            // material hint. Intercept the legal candidate set before any generic
            // target selector can place it under Undying by accident.
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

            // Fallen Angel's trigger can also lose activateId in MDPro3. During
            // the committed Mary route, a legal Mad Golden candidate identifies
            // this prompt unambiguously; lock it before generic SpSummon logic.
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

            // Candidate-driven fallback for MDPro3 builds that temporarily lose the
            // solving-chain card between Great Pumpking's trigger and its search.
            if (greatPumpkingSearchAttempted && !greatPumpkingSearchResolved
                && hint == HintMsg.AddToHand
                && cards.Any(c => c.Location == CardLocation.Deck))
            {
                return SelectGreatPumpkingSearchTarget(cards, min, max);
            }

            // Apply one detach policy to every Xyz effect. The specific Urara
            // Samuel exception is handled inside the helper. Keep Samuel's target
            // pending through this cost prompt; its revive target is selected next.
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

            // Candidate-driven Samuel target selection. MDPro3 can report
            // activateId=0 between Samuel's detach cost and HINTMSG_SPSUMMON, so
            // do not depend on the current solving-chain lookup here.
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

            // Flying Mary's committed Eldlich target prompt uses HINTMSG_TARGET
            // in Lua and may arrive with activateId=0. Lock Eldlich before any
            // generic Level 5+ Zombie selector can choose Army/Pumpking instead.
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

            // Flying Mary's comeback target prompt may also arrive with
            // activateId=0. On our second-or-later turn, if Pumpking's Special-
            // Summon effect has not been spent, revive Pumpking before Eldlich or
            // any generic Level 5+ Zombie.
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

            // Pumpking's Lua asks for Set first and discard second. During these
            // two selection messages the solving-chain lookup is not reliable on
            // every core build, so the accepted-action flag is the source of truth.
            if (pumpkingHandSelectionPending)
            {
                // Lua order is fixed: first select Call from Deck/GY, then select
                // one card from the hand to discard. The second prompt has no
                // HINT_SELECTMSG in the official script and therefore arrives as
                // hint=0 on MDPro3. Track the completed first prompt instead of
                // relying on HintMsg.Discard.
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

            // Great Pumpking's return prompt can also lose activateId. Preserve the
            // Urara self-bounce rule before applying any generic enemy-target guard.
            if (greatPumpkingBounceAttempted
                && !greatPumpkingBounceResolved
                && hint == HintMsg.ReturnToHand)
            {
                return SelectGreatPumpkingBounceTargets(cards, min, max);
            }

            // Eldlich hand effect: prompt 1 is a Spell/Trap cost from our hand;
            // prompt 2 is the actual field target. Never let the Murakumo-style
            // generic discard selector or the base selector confuse the two.
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
                    if (CanRecoverPumpkingFromHublotNow()
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
                        // Never let the generic Call/Vortex priorities override
                        // the intended starter chain: Snakehair -> Ectoplasmic ->
                        // Pumpking or Hublot.
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

                        // Zombie lock, insufficient monsters, or missing Extra Deck
                        // pieces: send Doomking and stop the Eldlich line here.
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

                case CardId.GlowUpBloom:
                    if (hint == HintMsg.AddToHand || hint == HintMsg.SpSummon)
                    {
                        int[] priority = GetGlowUpBloomSearchPriority();
                        DebugRoute("Bloom search priority="
                            + string.Join(",", priority.Select(id => id.ToString()).ToArray()));
                        IList<ClientCard> nonMammoth = cards
                            .Where(c => !c.IsCode(CardId.GreatMammothOfTheNetherworld))
                            .ToList();
                        if (nonMammoth.Count > 0)
                            return SelectByIdPriority(nonMammoth, min, max, priority);
                        return SelectByIdPriority(cards, min, max, priority);
                    }
                    break;

                case CardId.ChangshiTheSpiridao:
                    if (hint == HintMsg.ToGrave)
                        return SelectChangshiDeckTarget(cards, min, max);
                    if (hint == HintMsg.Remove)
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.GlowUpBloom,
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

                        // Grave effect cost is selected by the exact field
                        // instance. An unlinked Call is expendable; a different
                        // face-up Call holding a monster is protected even though
                        // both cards share the same CardId.
                        List<ClientCard> fieldCosts = cards
                            .Where(c => c != null
                                && c.Controller == 0
                                && c.Location == CardLocation.SpellZone
                                && (c.IsSpell() || c.IsTrap()))
                            .OrderBy(GetEldlichGraveFieldCostScore)
                            .ThenBy(c => c.Sequence)
                            .ToList();
                        if (fieldCosts.Count == 0)
                            return null;

                        ClientCard selectedCost = fieldCosts[0];
                        if (IsLinkedFaceupCall(selectedCost)
                            && !IsExpendableLinkedCallForEldlich(selectedCost))
                        {
                            // Activation normally prevents this state. If the field
                            // changed after activation, still select the lowest-loss
                            // exact instance rather than returning null to a mandatory
                            // cost prompt and letting the engine choose arbitrarily.
                            DebugRoute("WARNING Eldlich GY mandatory cost fell back to protected linked Call");
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
                                DebugRoute("Samuel negate target=" + exact.Id);
                                return Util.CheckSelectCount(
                                    new List<ClientCard> { exact }, cards, min, max);
                            }
                        }

                        // The optional Yes/No guard should make this path rare.
                        // Never fall through to the generic enemy selector and turn
                        // a bridge revive into a random negate on a low-value body.
                        List<ClientCard> meaningful = cards
                            .Where(c => IsWorthDisablingWithSamuel(c, false))
                            .OrderByDescending(c => ScoreSamuelDisableTarget(c, null))
                            .ToList();
                        samuelOpponentNegatePending = false;
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

                case CardId.MercuriumTheLivingQuicksilver:
                    if (hint == HintMsg.XyzMaterial)
                    {
                        ClientCard bloom = cards.FirstOrDefault(c => c.IsCode(CardId.GlowUpBloom));
                        if (bloom != null)
                            return Util.CheckSelectCount(new List<ClientCard> { bloom }, cards, min, max);
                        return SelectByIdPriority(cards, min, max,
                            CardId.GlowUpBloom,
                            CardId.OfficiatingReverie,
                            CardId.ArmyOfTheHaunted,
                            CardId.Mezuki,
                            CardId.ChangshiTheSpiridao);
                    }
                    break;

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
                        List<ClientCard> tribute = cards.Where(c => c.Controller == 0 && IsZombie(c) && c != Card)
                            .OrderBy(GetMaterialValue).ToList();
                        return Util.CheckSelectCount(tribute, cards, min, max);
                    }
                    return SelectEnemyField(cards, min, max);

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

            // Last-resort protection for target prompts whose solving-chain
            // metadata disappeared. When the legal candidate list contains an
            // opponent card, never fall through to a selector that can choose our
            // own field card for a hostile removal/negation effect.
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

            // Generic Murakumo-style hand disposition safety net, adapted from
            // the Ryzeal selector pattern. Run it only after every card-specific
            // flow above has had a chance to handle its own cost/target prompt.
            // Pumpking's hint=0 discard is intentionally excluded because its
            // two-step pending flow is handled at the top of this method.
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
                List<ClientCard> selected = new List<ClientCard>();
                ClientCard hublot = cards.FirstOrDefault(IsHublot);

                if (selectedHublotXyzId == CardId.OfficiatorOfDoomSamuel)
                {
                    // Samuel should hold Pumpking whenever possible so its first
                    // detach puts Pumpking back in the GY. Hublot is the other body.
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
            if (quicksilverLineActive)
            {
                ClientCard quicksilver = cards.FirstOrDefault(c =>
                    c.IsCode(CardId.MercuriumTheLivingQuicksilver));
                if (quicksilver != null && min <= 1)
                {
                    return Util.CheckSelectCount(
                        new List<ClientCard> { quicksilver }, cards, min, max);
                }
            }

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
            // Hublot presents the ordinary Tribute Summon and its custom
            // no-Tribute Summon Procedure as separate choices. The custom Lua
            // description is StringId 0 (normally the second visible option).
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
                    // Lua order: StringId 1 = negate, StringId 2 = banish. Use the
                    // choice captured at activation time instead of LastChainPlayer,
                    // which is no longer reliable during this prompt.
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
                return CheckRemainInDeck(CardId.EldlichTheGoldenLord) > 0
                    || CheckRemainInDeck(CardId.DoomkingBalerdroch) > 0;
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
                    // On our turn the revive is a combo action; take the optional
                    // negate when the Lua has confirmed a legal enemy monster exists.
                    negate = Enemy.GetMonsters().Any(c =>
                        c.IsFaceup() && !c.IsDisabled());
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

                if (cardId == CardId.MercuriumTheLivingQuicksilver)
                {
                    order.AddRange(new[] { 5, 6, 0, 2, 4, 1, 3 });
                }
                else if (cardId == CardId.TheUndyingLegion)
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
            // The starting player's first turn has no Battle Phase. On every
            // later own turn, Main Phase 1 can still lead into battle.
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

            // Samuel leaves Defence only when its own damage is the exact reason
            // the current clear-board battle becomes lethal.
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

            // Undying has 0 DEF. Defence Position never protects it and only
            // removes its ability to apply battle pressure on the next turn.
            if (cardId == CardId.TheUndyingLegion)
            {
                DebugRoute("POSITION Undying: Attack; DEF is 0");
                return CardPosition.FaceUpAttack;
            }

            // Samuel is an interaction body, not a generic attacker. Keep it in
            // Defence unless its own 2400 damage is required for a confirmed
            // clear-board lethal this turn.
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

            // Never expose a newly summoned monster in Attack Position on the
            // opponent's turn when its effect does not require that position.
            if (Duel.Player == 1)
            {
                DebugRoute("POSITION " + cardId + ": Defence on opponent turn");
                return CardPosition.FaceUpDefence;
            }

            // Going first means there is no Battle Phase. Previous logic used
            // 'enemy has no monsters' as a reason to choose Attack Position,
            // which did the exact opposite of what a first-turn end board needs.
            if (Duel.Turn == 1)
            {
                DebugRoute("POSITION " + cardId + ": Defence on first turn; no Battle Phase");
                return CardPosition.FaceUpDefence;
            }

            // Varudras needs to be the attacker for its EVENT_BATTLE_START
            // destruction. Put it in Attack Position only when battle is actually
            // available this turn; otherwise keep it protected in Defence.
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

            // In Main Phase 1 of a later turn, monsters with real ATK should be
            // able to attack directly or clear a monster. The absence of an enemy
            // monster is no longer a special-case position rule.
            if (BattlePhaseIsAvailableThisTurn()
                && data != null
                && data.Attack > 0)
            {
                return CardPosition.FaceUpAttack;
            }

            // Monsters summoned after battle, or passive bodies whose DEF is at
            // least as useful as their ATK, enter Defence Position. Equal stats
            // also prefer Defence when no attack can happen this turn.
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

            // No monster should be rotated into Attack Position on the first
            // turn because the Battle Phase does not exist.
            if (Duel.Turn == 1)
                return false;

            if (Duel.Phase == DuelPhase.Main1)
            {
                // Samuel stays in Defence unless moving it is what creates a
                // confirmed lethal line. Do not expose it merely because a Battle
                // Phase exists or the opponent currently has no monster.
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

                // Varudras must attack to unlock its battle-start removal. This
                // is the only generic battle-role override; it no longer stays in
                // Attack Position blindly across turns.
                if (Card.IsCode(CardId.Varudras))
                {
                    if (Card.IsDefense())
                    {
                        DebugRoute("REPOS Varudras: Defence -> Attack before Battle Phase");
                        return true;
                    }
                    return false;
                }

                // Other monsters move to Attack Position only because an actual
                // Battle Phase is available, never merely because the opponent
                // currently controls no monsters.
                return Card.IsDefense() && Card.Attack > 0;
            }

            // After battle, protect a body only when it did not attack and has a
            // genuinely better DEF stat. The engine will reject illegal changes
            // for monsters that attacked or changed position already. Varudras
            // remains in Attack Position so its intended battle posture is clear.
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
                // Setting Dominus avoids the permanent Attribute lock from a hand
                // activation whenever we have the opportunity to prepare it.
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

        // =====================================================================
        // Turn/chain bookkeeping
        // =====================================================================

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null)
            {
                bool movedToOurMonsterZone = currentControler == 0
                    && (currentLocation & (int)CardLocation.MonsterZone) != 0;
                bool movedFromOurHand = previousControler == 0
                    && (previousLocation & (int)CardLocation.Hand) != 0;
                bool movedToOurHand = currentControler == 0
                    && (currentLocation & (int)CardLocation.Hand) != 0;
                bool movedToEnemyMonsterZone = currentControler == 1
                    && (currentLocation & (int)CardLocation.MonsterZone) != 0;
                bool movedToEnemySpellZone = currentControler == 1
                    && (currentLocation & (int)CardLocation.SpellZone) != 0;

                // Commitment windows are public-information timing markers. They
                // let Samuel revive Snakehair/Mammoth immediately after a visible
                // threat enters the field, before the opponent reaches an open
                // state for an Ignition Effect. Facedown cards are never inferred.
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
                    else if (card.IsCode(CardId.MercuriumTheLivingQuicksilver))
                    {
                        quicksilverLineActive = true;
                        pumpkingComboState = PumpkingComboState.QuicksilverSummoned;
                        if (eldlichRouteRank10CommitPending)
                        {
                            DebugRoute("RESOLVED Flying Mary Rank 10 line with Xyz=" + card.Id);
                            eldlichRouteRank10CommitPending = false;
                            eldlichRouteActive = false;
                            eldlichRouteMarySummoned = false;
                            flyingMaryEldlichReviveSelectionPending = false;
                        }
                    }
                    else if (card.IsCode(CardId.GravityController))
                    {
                        pumpkingComboState = PumpkingComboState.GravityControllerSummoned;
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
            if (chain != null && chain.ActivateController == 0 && !Duel.IsCurrentSolvingChainNegated())
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

                if (chain.ActivateId == CardId.ChangshiTheSpiridao)
                {
                    changshiMillResolved = true;
                    DebugRoute("RESOLVED Changshi mill=" + selectedChangshiMillId);
                    if (selectedChangshiMillId == CardId.AshBlossom
                        && Bot.HasInGraveyard(CardId.AshBlossom))
                    {
                        ashReplayLineActive = true;
                    }
                }

                if (chain.ActivateId == CardId.OfficiatorOfDoomSamuel
                    && selectedSamuelReviveId != 0)
                {
                    // Some MDPro3 builds report this Quick Effect with description
                    // -1. Confirm success from the selected monster actually being
                    // on our field instead of trusting the description value.
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
                        // The on-summon trigger may resolve with description -1.
                        // Once the accepted Great Pumpking chain resolves and it was
                        // not the bounce effect, the search is complete.
                        greatPumpkingSearchResolved = true;
                        greatPumpkingSearchWindowPending = false;
                        DebugRoute("RESOLVED Great Pumpking search; desc="
                            + chain.ActivateDescription);
                    }
                }

                if (chain.ActivateId == CardId.MercuriumTheLivingQuicksilver)
                {
                    ClientCard quicksilver = Bot.GetMonsters().FirstOrDefault(c =>
                        c.IsCode(CardId.MercuriumTheLivingQuicksilver));
                    quicksilverLoadedBloom = quicksilver != null
                        && quicksilver.Overlays.Contains(CardId.GlowUpBloom);
                }

                if (chain.ActivateId == CardId.GlowUpBloom)
                {
                    zombieLockedThisTurn = true;
                    pumpkingStarterSeenThisDuel = true;
                    if (Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts))
                    {
                        pumpkingSearchSucceeded = true;
                        pumpkingComboState = PumpkingComboState.PumpkingReady;
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
            flyingMaryComebackPumpkingPending = false;
            callReviveSelectionPending = false;
            plannedCallReviveId = 0;
            pendingInfiniteImpermanenceTarget = null;
            samuelGraveRecycleSelectionPending = false;
            pendingUndyingTarget = null;
            base.OnChainEnd();
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
            pumpkingSummonEffectAttempted = false;
            pumpkingSummonEffectResolved = false;
            changshiMillAttempted = false;
            changshiMillResolved = false;
            greatPumpkingSearchAttempted = false;
            greatPumpkingSearchWindowPending = false;
            selectedGreatPumpkingSearchId = 0;
            greatPumpkingBounceAttempted = false;
            ectoplasmicSearchUsed = false;
            pumpkingSearchSucceeded = false;
            callSetByPumpking = false;
            samuelReviveResolved = false;
            greatPumpkingSearchResolved = false;
            greatPumpkingBounceResolved = false;
            quicksilverLineActive = false;
            quicksilverLoadedBloom = false;
            glowUpBloomEffectCommittedThisTurn = false;
            zombieLockedThisTurn = false;
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
            selectedFoolishBurialSendId = 0;
            selectedSamuelReviveId = 0;
            samuelRevivedCardId = 0;
            summonCount = 1;
            base.OnNewTurn();
            RestorePersistentEldlichRouteState();
            ObservePumpkingStarterState();
            RecalculateStrategicPlan("new turn");
            DebugRoute("NEW TURN " + Duel.Turn + " starterSeen=" + pumpkingStarterSeenThisDuel
                + " goal=" + currentStrategicGoal + " route=" + currentComboRoute);
            DebugCards("HAND", Bot.Hand);
        }
    }
}
