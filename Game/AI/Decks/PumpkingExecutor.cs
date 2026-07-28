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

        private PumpkingComboState pumpkingComboState = PumpkingComboState.None;
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
        private bool greatPumpkingBounceAttempted = false;
        private bool greatPumpkingBounceResolved = false;
        private bool quicksilverLineActive = false;
        private bool quicksilverLoadedBloom = false;
        private bool zombieLockedThisTurn = false;

        // Prompt state must be captured when the executor accepts the effect.
        // Duel.LastChainPlayer / solving-chain metadata can change before the
        // subsequent option or target selection prompt arrives.
        private bool doomkingOptionPending = false;
        private bool doomkingPreferNegate = false;
        private bool varudrasDestroySelectionPending = false;

        // Changshi -> Ash replay branch:
        // Great Pumpking first, search Army, Army + Changshi make Samuel,
        // Samuel revives Ash, then Great Pumpking returns Ash to the hand.
        private bool ashReplayLineActive = false;

        // Delta -> Eldlich -> Fallen Angel -> Flying Mary -> Rank 10 route.
        private bool eldlichRouteActive = false;
        private bool eldlichRouteMarySummoned = false;

        // Once a real Pumpking starter has been seen, Quicksilver is disabled for
        // the rest of the Duel. This flag intentionally does not reset each turn.
        private bool pumpkingStarterSeenThisDuel = false;

        private int selectedHublotSendId = 0;
        private int selectedHublotRecoverId = 0;
        private bool selectedHublotRecover = false;
        private int selectedHublotXyzId = 0;
        private int selectedChangshiMillId = 0;
        private int selectedDeltaSendId = 0;
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
            AddExecutor(ExecutorType.Activate, CardId.Varudras, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.EvolzarLars, EvolzarLarsActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheUndyingLegion, TheUndyingLegionActivate);
            AddExecutor(ExecutorType.Activate, CardId.OfficiatorOfDoomSamuel, OfficiatorOfDoomSamuelActivate);
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
            AddExecutor(ExecutorType.Activate, CardId.Hublot, HublotActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.PumpkingTheKingOfGraveGhosts, PumpkingSummonActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChangshiTheSpiridao, ChangshiTheSpiridaoActivate);

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
            AddExecutor(ExecutorType.Activate, CardId.OfficiatingReverie, OfficiatingReverieActivate);
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
            AddExecutor(ExecutorType.SpSummon, CardId.FlyingMary, FlyingMarySummon);
            AddExecutor(ExecutorType.SpSummon, CardId.VampireSucker, VampireSuckerSummon);

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

        private bool HasSmallPumpkingOnField()
        {
            return Bot.HasInMonstersZone(CardId.PumpkingTheKingOfGraveGhosts, faceUp: true);
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

        private bool CanUsePumpkingHandEffectNow()
        {
            return Duel.Player == 0
                && HasPumpkingInHand()
                && !pumpkingHandEffectAttempted
                && Bot.Hand.Count > 1
                && HasOpenSpellZone();
        }

        private bool HasImmediatePumpkingActionPending()
        {
            if (CanUsePumpkingHandEffectNow())
                return true;
            if (callSetByPumpking && HasPumpkingInGrave() && HasOpenMainMonsterZone())
                return true;
            if (HasSmallPumpkingOnField() && !HasChangshiOnField()
                && !pumpkingSummonEffectAttempted && HasOpenMainMonsterZone())
            {
                return true;
            }
            if (HasChangshiOnField() && !changshiMillAttempted)
                return true;
            return false;
        }
        private bool HasSafeGreatPumpkingBounceTarget()
        {
            ClientCard greatPumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.PumpkingTheGreatGhostKing));
            if (greatPumpking == null)
                return false;

            // The Urara route must save the once-per-turn bounce for Ash.
            if (ashReplayLineActive)
                return Bot.HasInMonstersZone(CardId.AshBlossom, faceUp: true);

            if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                return true;
            if (Bot.HasInMonstersZone(CardId.AshBlossom, faceUp: true))
                return true;

            // Only spend the effect on our own card when the small Pumpking is
            // trapped as material and must be detached back to the GY.
            if (!greatPumpking.Overlays.Contains(CardId.PumpkingTheKingOfGraveGhosts))
                return false;

            return Bot.GetSpells().Any(c => c != null)
                || Bot.GetMonsters().Any(c => c != greatPumpking && c.IsFaceup());
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

        private bool CanSnakehairReachPumpkingBeforeHublot()
        {
            return !HasPumpkingInHand()
                && Bot.GetMonsterCount() == 0
                && !ectoplasmicSearchUsed
                && !Bot.HasInHand(CardId.EctoplasmicFortification)
                && Bot.HasInHand(CardId.StareOfTheSnakeHair)
                && CheckRemainInDeck(CardId.EctoplasmicFortification) > 0
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
        private int GetChangshiMillTargetId(IList<ClientCard> cards)
        {
            if (ShouldStartAshReplayLine(cards))
            {
                DebugRoute("Changshi target: Ash Blossom (Urara route)");
                return CardId.AshBlossom;
            }

            bool hasLevel6ReviveTarget = Bot.Graveyard.Any(c => IsLevel6Zombie(c) && c.IsCanRevive()
                && !c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));

            // The normal Samuel-first route needs a Level 6 Zombie in the GY.
            // Hublot normally supplied Reverie; if it did not, Changshi does.
            if (!hasLevel6ReviveTarget
                && cards.Any(c => c.IsCode(CardId.OfficiatingReverie)))
            {
                DebugRoute("Changshi target: Reverie for Samuel revive");
                return CardId.OfficiatingReverie;
            }

            // Quicksilver fallback has no Hublot body. Army is the direct Level 6
            // extender while Call remains face-up.
            if (quicksilverLineActive && !HasHublotOnField() && HasFaceupCall()
                && cards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Changshi target: Army for Quicksilver fallback");
                return CardId.ArmyOfTheHaunted;
            }

            // Army is normally searched by Great Pumpking. Mill it only when
            // Reverie is already in hand, matching the confirmed deck rule.
            if (Bot.HasInHand(CardId.OfficiatingReverie)
                && cards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
            {
                DebugRoute("Changshi target: Army because Reverie is in hand");
                return CardId.ArmyOfTheHaunted;
            }

            if (Duel.Turn >= 2
                && Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive())
                && cards.Any(c => c.IsCode(CardId.Mezuki)))
            {
                DebugRoute("Changshi target: Mezuki (turn 2+ extender)");
                return CardId.Mezuki;
            }

            // Bloom is only used before Pumpking's hand effect and only after all
            // required non-Zombie Extra Deck summons are complete.
            if (!pumpkingHandEffectAttempted
                && !activatedThisTurn.Contains(PumpkingHandMarker)
                && CanAcceptZombieLock()
                && DefaultCheckWhetherBotCanSearch()
                && cards.Any(c => c.IsCode(CardId.GlowUpBloom)))
            {
                DebugRoute("Changshi target: Glow-Up Bloom");
                return CardId.GlowUpBloom;
            }

            if (cards.Any(c => c.IsCode(CardId.Mezuki)))
            {
                DebugRoute("Changshi target: Mezuki fallback");
                return CardId.Mezuki;
            }
            if (cards.Any(c => c.IsCode(CardId.EldlichTheGoldenLord)))
                return CardId.EldlichTheGoldenLord;
            if (cards.Any(c => c.IsCode(CardId.DoomkingBalerdroch)))
                return CardId.DoomkingBalerdroch;
            if (cards.Any(c => c.IsCode(CardId.OfficiatingReverie)))
                return CardId.OfficiatingReverie;
            if (cards.Any(c => c.IsCode(CardId.ArmyOfTheHaunted)))
                return CardId.ArmyOfTheHaunted;

            return cards.FirstOrDefault() != null ? cards.First().Id : 0;
        }

        private bool IsPumpkingComboInProgress()
        {
            return pumpkingComboState != PumpkingComboState.None
                && pumpkingComboState != PumpkingComboState.UndyingSummoned;
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

        private bool CanStartEldlichRoute()
        {
            if (zombieLockedThisTurn || dominusImpulseHandLock)
                return false;
            if (!HasEldlichLinkSeedOnField() || !HasEldlichRouteExtraDeck())
                return false;
            if (!HasOpenMainMonsterZone())
                return false;

            bool eldlichCanReachGrave = Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || CheckRemainInDeck(CardId.EldlichTheGoldenLord) > 0;
            return eldlichCanReachGrave;
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

            if (ashReplayLineActive)
                target = cards.FirstOrDefault(c => c.IsCode(CardId.AshBlossom));

            // Outside the fixed Urara branch, revive Ash only when two other
            // Level 6 bodies already remain and the Eldlich route is not realistic.
            if (target == null
                && CountFreeLevel6ForGreatPumpking() >= 2
                && !CanContinueEldlichRouteFromCurrentBoard())
            {
                target = cards.FirstOrDefault(c => c.IsCode(CardId.AshBlossom));
            }

            int[] priority =
            {
                CardId.OfficiatingReverie,
                CardId.ArmyOfTheHaunted,
                CardId.ChangshiTheSpiridao,
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.Hublot,
                CardId.DoomkingBalerdroch,
                CardId.EldlichTheGoldenLord,
                CardId.PumpkingTheKingOfGraveGhosts
            };

            if (target == null)
            {
                foreach (int id in priority)
                {
                    target = cards.FirstOrDefault(c => c.IsCode(id));
                    if (target != null)
                        break;
                }
            }

            if (target == null)
                target = cards.FirstOrDefault();

            selectedSamuelReviveId = target != null ? target.Id : 0;
            DebugRoute("Samuel revive target=" + selectedSamuelReviveId);
            if (target == null)
                return base.OnSelectCard(cards, min, max, HintMsg.SpSummon, false);

            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
        }
        private IList<ClientCard> SelectGreatPumpkingBounceTargets(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> result = new List<ClientCard>();
            ClientCard solvingGreatPumpking = Duel.GetCurrentSolvingChainCard();
            if (solvingGreatPumpking == null
                || !solvingGreatPumpking.IsCode(CardId.PumpkingTheGreatGhostKing))
            {
                solvingGreatPumpking = Bot.GetMonsters().FirstOrDefault(c =>
                    c.IsFaceup() && c.IsCode(CardId.PumpkingTheGreatGhostKing));
            }

            if (ashReplayLineActive)
            {
                ClientCard ash = cards.FirstOrDefault(c => c.Controller == 0
                    && c.Location == CardLocation.MonsterZone
                    && c.IsCode(CardId.AshBlossom));
                if (ash != null)
                    result.Add(ash);
            }

            result.AddRange(GetEnemyFieldPriority(cards, true)
                .Where(c => c != solvingGreatPumpking && !result.Contains(c))
                .Take(Math.Max(0, max - result.Count)));

            if (result.Count < max)
            {
                ClientCard ash = cards.FirstOrDefault(c => c.Controller == 0
                    && c.Location == CardLocation.MonsterZone
                    && c.IsCode(CardId.AshBlossom));
                if (ash != null && !result.Contains(ash))
                    result.Add(ash);
            }

            ClientCard greatPumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.PumpkingTheGreatGhostKing));
            bool mustDetachSmallPumpking = greatPumpking != null
                && greatPumpking.Overlays.Contains(CardId.PumpkingTheKingOfGraveGhosts);

            if (mustDetachSmallPumpking && result.Count < max)
            {
                int[] ownSpellPriority =
                {
                    CardId.CallOfTheHaunted,
                    CardId.DeltaOfInvitation,
                    CardId.EctoplasmicFortification,
                    CardId.VortexOfTime,
                    CardId.FoolishBurial,
                    CardId.Terraforming
                };
                foreach (int id in ownSpellPriority)
                {
                    ClientCard target = cards.FirstOrDefault(c => c != solvingGreatPumpking
                        && c.Controller == 0 && c.Location == CardLocation.SpellZone
                        && c.IsCode(id));
                    if (target != null && !result.Contains(target))
                    {
                        result.Add(target);
                        break;
                    }
                }

                if (result.Count < min)
                {
                    ClientCard anyOwnSpell = cards.FirstOrDefault(c => c != solvingGreatPumpking
                        && c.Controller == 0 && c.Location == CardLocation.SpellZone
                        && !result.Contains(c));
                    if (anyOwnSpell != null)
                        result.Add(anyOwnSpell);
                }

                if (result.Count < min)
                {
                    ClientCard expendableMonster = cards
                        .Where(c => c != solvingGreatPumpking && c.Controller == 0
                            && c.Location == CardLocation.MonsterZone
                            && !c.IsCode(
                                CardId.PumpkingTheGreatGhostKing,
                                CardId.OfficiatorOfDoomSamuel,
                                CardId.EldlichTheGoldenLord,
                                CardId.EldlichTheMadGoldenLord))
                        .OrderBy(GetMaterialValue)
                        .ThenBy(c => c.Attack)
                        .FirstOrDefault();
                    if (expendableMonster != null)
                        result.Add(expendableMonster);
                }
            }

            if (result.Count < min)
            {
                result.AddRange(cards.Where(c => c != solvingGreatPumpking && !result.Contains(c))
                    .Take(min - result.Count));
            }

            DebugRoute("Great Pumpking bounce targets: "
                + string.Join(",", result.Select(c => c.Id.ToString()).ToArray()));
            return Util.CheckSelectCount(result, cards, min, max);
        }

        private IList<ClientCard> SelectDiscard(IList<ClientCard> cards, int min, int max)
        {
            // Cards that gain value in the GY are deliberately listed first.
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

        private IList<ClientCard> SelectZombieToRevive(IList<ClientCard> cards, int min, int max)
        {
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

            return SelectByIdPriority(cards, min, max, priority.ToArray());
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
                ClientCard army = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                    && c.Level == 6 && c.IsCode(CardId.ArmyOfTheHaunted));
                ClientCard changshi = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                    && c.Level == 6 && c.IsCode(CardId.ChangshiTheSpiridao));
                if (army != null && changshi != null)
                    return new List<ClientCard> { army, changshi };
                return new List<ClientCard>();
            }

            ClientCard pumpking = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.Level == 6 && c.IsCode(CardId.PumpkingTheKingOfGraveGhosts));
            if (pumpking == null)
                return new List<ClientCard>();

            ClientCard second = Bot.GetMonsters()
                .Where(c => c != pumpking && c.IsFaceup() && c.Level == 6
                    && !c.HasType(CardType.Xyz | CardType.Link))
                .OrderBy(c => c.IsCode(CardId.ChangshiTheSpiridao) ? 0 : 1)
                .ThenBy(c => IsHublot(c) ? 10 : GetMaterialValue(c))
                .FirstOrDefault();
            if (second == null)
                return new List<ClientCard>();

            return new List<ClientCard> { pumpking, second };
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

        private List<ClientCard> GetFlyingMaryEldlichMaterials()
        {
            ClientCard fallen = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.FallenAngelOfTheGoldenLand));
            if (fallen == null)
                return new List<ClientCard>();

            ClientCard samuel = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.OfficiatorOfDoomSamuel));
            if (samuel != null && Bot.HasInGraveyard(CardId.OfficiatingReverie))
                return new List<ClientCard> { fallen, samuel };

            ClientCard otherZombie = Bot.GetMonsters()
                .Where(c => c != fallen && c.IsFaceup() && IsZombie(c)
                    && !c.HasType(CardType.Link)
                    && !c.IsCode(CardId.PumpkingTheGreatGhostKing))
                .OrderBy(GetMaterialValue)
                .ThenBy(c => c.Attack)
                .FirstOrDefault();
            if (otherZombie == null)
                return new List<ClientCard>();

            return new List<ClientCard> { fallen, otherZombie };
        }

        private List<ClientCard> GetLinkZombieMaterials()
        {
            return Bot.GetMonsters()
                .Where(c => c.IsFaceup() && IsZombie(c) && !c.HasType(CardType.Link))
                .OrderBy(GetMaterialValue)
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
            ClientCard target = Util.GetLastChainCard();
            if (target != null && target.Controller == 1)
                currentNegateCardList.Add(target);
            SelectSTPlace(Card, true);
            return true;
        }

        private bool DominusImpulseActivate()
        {
            if (IsCardEffectNegated())
                return false;

            // Same policy as AI_Apophis: if a set copy is also activable, use that
            // copy before activating one from the hand and accepting the Attribute lock.
            if (Duel.MainPhase.ActivableCards.Any(c => c != null && c != Card
                && c.IsCode(CardId.DominusImpulse) && c.IsOnField()
                && !infiniteImpermanenceNegatedColumns.Contains(c.Sequence)))
            {
                return false;
            }

            ClientCard last = Util.GetLastChainCard();
            if (!IsOpponentChainWorthNegating(last))
                return false;

            currentNegateCardList.Add(last);
            SelectSTPlace(Card, true);
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

            // A friendly Zombie effect may trigger Doomking only for the banish
            // option, and only when an opponent monster is actually available.
            bool canBanishOpponentMonster = Enemy.GetMonsters().Any()
                || Enemy.Graveyard.Any(c => c != null && c.IsMonster());
            if (Duel.LastChainPlayer == 0 && canBanishOpponentMonster)
            {
                doomkingOptionPending = true;
                doomkingPreferNegate = false;
                DebugRoute("ACCEPT Doomking: friendly trigger, prefer BANISH");
                return true;
            }

            return false;
        }

        private bool VarudrasActivate()
        {
            if (IsCardEffectNegated())
                return false;

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
            if (ActivateDescription == negateOrBattleDesc
                && Duel.LastChainPlayer == 1
                && Duel.CurrentChain.Count > 0)
            {
                ClientCard last = Util.GetLastChainCard();
                if (!IsOpponentChainWorthNegating(last))
                    return false;
                currentNegateCardList.Add(last);
                varudrasDestroySelectionPending = false;
                DebugRoute("ACCEPT Varudras negate id=" + (last != null ? last.Id : 0));
                return true;
            }

            // Battle-start destroy and the trigger after Varudras is destroyed are
            // optional. Record the target prompt now because solving-chain metadata
            // is not reliable when the subsequent HINTMSG_DESTROY prompt arrives.
            if (ActivateDescription == negateOrBattleDesc
                || ActivateDescription == destroyedDesc
                || ActivateDescription == -1)
            {
                if (!hasEnemyFieldCard)
                {
                    varudrasDestroySelectionPending = false;
                    DebugRoute("DECLINE Varudras destroy: no enemy field card");
                    return false;
                }

                varudrasDestroySelectionPending = true;
                DebugRoute("ACCEPT Varudras destroy: enemy target reserved");
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
                return HasOpenMainMonsterZone();

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
                // Locked route rule: Snakehair/Ectoplasmic are used before Hublot
                // to put Pumpking in the hand. Hublot then mills a different Zombie
                // for GY setup instead of being forced to mill Pumpking.
                bool needPumpking = !HasPumpkingInHand()
                    && CheckRemainInDeck(CardId.PumpkingTheKingOfGraveGhosts) > 0;

                if (needPumpking)
                {
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

            // Snakehair is used before Hublot only as the bridge into
            // Ectoplasmic Fortification, which then searches Pumpking.
            if (HasPumpkingInHand() || Bot.HasInHand(CardId.EctoplasmicFortification))
                return false;
            if (CheckRemainInDeck(CardId.EctoplasmicFortification) <= 0)
                return false;

            return DefaultCheckWhetherBotCanSearch();
        }

        private bool StareOfTheSnakeHairFieldActivate()
        {
            if (Card.Location != CardLocation.MonsterZone || IsCardEffectNegated())
                return false;

            return Enemy.GetMonsters().Any(c => c.IsFaceup() && c.IsAttack()
                && !c.IsDisabled() && !c.IsShouldNotBeTarget());
        }
        private bool PumpkingHandActivate()
        {
            if (Card.Location != CardLocation.Hand || IsCardEffectNegated())
                return false;
            if (!HasOpenSpellZone() || Bot.Hand.Count <= 1 || pumpkingHandEffectAttempted)
                return false;

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

            pumpkingSummonEffectAttempted = true;
            DebugRoute("ACCEPT revived Pumpking trigger: summon Changshi");
            return true;
        }
        private bool FoolishBurialActivate()
        {
            if (IsCardEffectNegated() || HasImmediatePumpkingActionPending())
                return false;

            // Do not use Foolish before an unused Hublot Normal Summon. Hublot is
            // the starter and its own mill can prepare the required GY resource.
            if (summonCount > 0 && HasHublotInHand()
                && HasOpenMainMonsterZone() && !ShouldDelayHublotForPumpkingSearch())
            {
                return false;
            }

            SelectSTPlace(Card, true);
            DebugRoute("ACCEPT Foolish Burial fallback");
            return true;
        }
        private bool HublotSummon()
        {
            if (IsCardEffectNegated() || !HasOpenMainMonsterZone() || summonCount <= 0)
                return false;
            if (ShouldDelayHublotForPumpkingSearch())
                return false;

            DebugRoute("ACCEPT Normal Summon Hublot");
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
        private bool OfficiatingReverieActivate()
        {
            if (IsCardEffectNegated())
                return false;

            if (Card.Location == CardLocation.Hand)
            {
                if (HasImmediatePumpkingActionPending())
                    return false;
                return Bot.Hand.Count > 1 && HasOpenMainMonsterZone();
            }

            if (Card.Location == CardLocation.Grave)
                return HasOpenMainMonsterZone()
                    && Bot.Graveyard.Any(c => c != Card && IsZombie(c) && c.IsCanRevive());

            if (Card.Location == CardLocation.Removed)
                return Bot.GetMonsters().Any(c => c.IsFaceup() && IsZombie(c) && c.HasType(CardType.Xyz));

            return false;
        }
        private bool ArmyOfTheHauntedActivate()
        {
            if (IsCardEffectNegated())
                return false;

            if ((Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
                && HasFaceupCall())
            {
                // In the Urara route Army is summoned only after Great Pumpking
                // has resolved its turn-1 search. Outside that branch, do not let
                // Army cut in before Pumpking/Changshi/Samuel.
                if (ashReplayLineActive)
                    return HasGreatPumpkingOnField() && greatPumpkingSearchResolved
                        && HasOpenMainMonsterZone();
                if (HasImmediatePumpkingActionPending())
                    return false;
                if (HasSmallPumpkingOnField() && HasChangshiOnField()
                    && !HasSamuelOnField() && !HasGreatPumpkingOnField())
                {
                    return false;
                }
                return HasOpenMainMonsterZone();
            }

            return Card.Location == CardLocation.Grave
                && HasOpenSpellZone()
                && Bot.HasInGraveyard(CardId.CallOfTheHaunted);
        }

        private bool CallOfTheHauntedActivate()
        {
            if (IsCardEffectNegated() || !HasOpenMainMonsterZone())
                return false;

            List<ClientCard> revivable = Bot.Graveyard
                .Where(c => c.IsMonster() && c.IsCanRevive())
                .ToList();
            if (revivable.Count == 0)
                return false;

            if (Duel.Player == 0)
            {
                // Pumpking's Lua grants the freshly Set Call permission to activate
                // this turn. Do not depend on route flags here: after Pumpking is in
                // the GY, the legal Call action must be taken immediately.
                if (HasPumpkingInGrave())
                {
                    DebugRoute("ACCEPT Call of the Haunted immediately: revive Pumpking");
                    return true;
                }

                return revivable.Any(c => c.IsCode(
                    CardId.PumpkingTheKingOfGraveGhosts,
                    CardId.Hublot,
                    CardId.ArmyOfTheHaunted,
                    CardId.ChangshiTheSpiridao,
                    CardId.OfficiatingReverie));
            }

            return revivable.Any(c => c.IsCode(
                CardId.GreatMammothOfTheNetherworld,
                CardId.StareOfTheSnakeHair,
                CardId.DoomkingBalerdroch,
                CardId.PumpkingTheKingOfGraveGhosts,
                CardId.Hublot,
                CardId.Hublot));
        }
        private bool MezukiActivate()
        {
            if (!CanUseEarthMonsterEffects() || IsCardEffectNegated())
                return false;
            if (Card.Location != CardLocation.Grave || !HasOpenMainMonsterZone())
                return false;
            if (HasImmediatePumpkingActionPending())
                return false;
            if (HasSmallPumpkingOnField() && HasChangshiOnField()
                && !HasSamuelOnField() && !HasGreatPumpkingOnField())
            {
                return false;
            }
            return Bot.Graveyard.Any(c => c != Card && IsZombie(c) && c.IsCanRevive());
        }

        private bool GlowUpBloomActivate()
        {
            if (IsCardEffectNegated() || Card.Location != CardLocation.Grave)
                return false;
            if (!DefaultCheckWhetherBotCanSearch())
                return false;

            if (quicksilverLineActive)
            {
                // Gravity Controller has already sent Quicksilver and Bloom to the GY.
                return !Bot.GetMonsters().Any(c => c.IsCode(CardId.MercuriumTheLivingQuicksilver));
            }

            return CanAcceptZombieLock();
        }
        private bool ChangshiTheSpiridaoActivate()
        {
            if (IsCardEffectNegated())
                return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player != 0 || changshiMillAttempted)
                    return false;
                changshiMillAttempted = true;
                DebugRoute("ACCEPT Changshi mill");
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

            bool hasSpellTrapCost = Bot.Hand.Any(c => c != Card && (c.IsSpell() || c.IsTrap()))
                || Bot.GetSpells().Any(c => c != Card);

            if (Card.Location == CardLocation.Hand)
                return hasSpellTrapCost && GetEnemyFieldPriority().Count > 0;

            if (Card.Location == CardLocation.Grave)
                return hasSpellTrapCost && HasOpenMainMonsterZone();

            return false;
        }

        private bool GreatMammothActivate()
        {
            if (IsCardEffectNegated())
                return false;
            return GetEnemyFieldPriority().Count > 0;
        }

        // =====================================================================
        // Extra Deck summon decisions
        // =====================================================================
        private bool PumpkingGreatGhostKingSummon()
        {
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

            // If Samuel cannot be made or its route was interrupted, never pass with
            // two legal Level 6 Zombies. Make Great Pumpking as the safe fallback.
            bool samuelRouteAvailable = GetSamuelMaterials().Count == 2
                && Bot.Graveyard.Any(c => IsZombie(c) && c.IsCanRevive());
            if (samuelRouteAvailable && !HasSamuelOnField())
                return false;

            List<ClientCard> fallback = GetRank6Materials(true).Take(2).ToList();
            if (fallback.Count != 2)
                return false;

            AI.SelectMaterials(fallback);
            DebugRoute("ACCEPT fallback Great Pumpking; prevent end-phase pass");
            return true;
        }

        private bool DhampirVampireSheridanSummon()
        {
            if (IsPumpkingComboInProgress())
                return false;

            ClientCard threat = Util.GetProblematicEnemyMonster(0, true)
                ?? Enemy.GetMonsters().Where(c => c.IsFaceup())
                    .OrderByDescending(c => c.GetDefensePower())
                    .FirstOrDefault();
            if (threat == null)
                return false;

            return SelectRank6Materials(zombiesOnly: false);
        }
        private bool OfficiatorOfDoomSamuelSummon()
        {
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

            AI.SelectMaterials(materials);
            DebugRoute("ACCEPT Xyz Samuel with " + materials[0].Id + "," + materials[1].Id);
            return true;
        }

        private bool EvolzarLarsSummon()
        {
            if (IsPumpkingComboInProgress())
                return false;

            // Turn 1 fallback board, or a later board that clearly needs a negate.
            bool needsNegate = Duel.Turn == 1
                || Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled())
                || Enemy.GetSpells().Any(c => c.IsFaceup() && !c.IsDisabled());
            if (!needsNegate)
                return false;

            return SelectRank6Materials(zombiesOnly: false);
        }

        private bool WollowSummon()
        {
            if (IsPumpkingComboInProgress())
                return false;

            bool graveyardMatchup = Enemy.Graveyard.Any(c => c.IsMonster()
                && c.HasType(CardType.Effect));
            int enemyPower = Enemy.GetMonsters()
                .Where(c => c.IsFaceup())
                .Select(c => c.GetDefensePower())
                .DefaultIfEmpty(0)
                .Max();
            int ourPower = Bot.GetMonsters()
                .Where(c => c.IsFaceup())
                .Select(c => c.GetDefensePower())
                .DefaultIfEmpty(0)
                .Max();
            if (!graveyardMatchup && enemyPower <= ourPower)
                return false;

            return SelectRank6Materials(zombiesOnly: false);
        }
        private bool TheUndyingLegionSummon()
        {
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
            if (zombieLockedThisTurn)
                return false;

            // No Pumpking starter means the first Rank 10 pair is reserved for
            // Quicksilver -> Bloom -> Gravity Controller. Once a starter has ever
            // been seen, Quicksilver stays disabled and Varudras is preferred.
            if (!pumpkingStarterSeenThisDuel && ShouldUseQuicksilverFallback())
                return false;

            List<ClientCard> materials = GetRank10Materials();
            if (materials.Count != 2)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool MercuriumSummon()
        {
            if (!ShouldUseQuicksilverFallback())
                return false;

            List<ClientCard> materials = GetRank10Materials();
            if (materials.Count != 2)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool FallenAngelSummon()
        {
            if (zombieLockedThisTurn || !CanUseLightMonsterEffects())
                return false;
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
            return true;
        }

        private bool FlyingMaryEldlichRouteSummon()
        {
            if (!eldlichRouteActive || !Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
                return false;

            List<ClientCard> materials = GetFlyingMaryEldlichMaterials();
            if (materials.Count != 2)
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool FlyingMarySummon()
        {
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
            // revive Pumpking to restart the combo. Never consume Great Pumpking.
            if (Duel.Turn >= 2 && Bot.HasInGraveyard(CardId.PumpkingTheKingOfGraveGhosts))
            {
                List<ClientCard> materials = GetLinkZombieMaterials()
                    .Where(c => !c.IsCode(CardId.PumpkingTheGreatGhostKing))
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
            // Rare fallback only: Reverie in GY, Samuel plus another Zombie that
            // is not Great Pumpking, and no Eldlich on the field or in the GY.
            if (!Bot.HasInGraveyard(CardId.OfficiatingReverie)
                || Bot.HasInGraveyard(CardId.EldlichTheGoldenLord)
                || Bot.HasInMonstersZone(CardId.EldlichTheGoldenLord, faceUp: true))
            {
                return false;
            }

            ClientCard samuel = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup()
                && c.IsCode(CardId.OfficiatorOfDoomSamuel));
            ClientCard otherZombie = Bot.GetMonsters()
                .Where(c => c != samuel && c.IsFaceup() && IsZombie(c)
                    && !c.HasType(CardType.Link)
                    && !c.IsCode(CardId.PumpkingTheGreatGhostKing))
                .OrderBy(GetMaterialValue)
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

            if (ActivateDescription == searchDescription)
            {
                greatPumpkingSearchAttempted = true;
                DebugRoute("ACCEPT Great Pumpking search");
                return true;
            }

            if (ActivateDescription == bounceDescription)
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
                return Bot.Graveyard.Any(c => c.IsMonster()) || Enemy.Graveyard.Any(c => c.IsMonster());

            if (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
            {
                return Card.Overlays.Count > 0
                    && HasOpenMainMonsterZone()
                    && Bot.Graveyard.Any(c => c != Card && IsZombie(c) && c.IsCanRevive());
            }

            return false;
        }

        private bool WollowActivate()
        {
            if (IsCardEffectNegated())
                return false;
            return Enemy.Graveyard.Count > 0;
        }

        private bool TheUndyingLegionActivate()
        {
            if (IsCardEffectNegated() || Duel.Player != 1)
                return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;

            return Enemy.GetMonsters().Any(c => c.IsFaceup() && c.IsAttack() && !c.IsShouldNotBeTarget())
                || Enemy.Graveyard.Any(c => c.IsMonster());
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
            return Card.Location == CardLocation.Grave && !IsCardEffectNegated();
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

            // Its GY summon ignition and its draw trigger are both beneficial.
            if (Duel.Player == 0 && Enemy.Graveyard.Any(c => c.IsMonster()) && HasOpenMainMonsterZone())
                return true;
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
                || activateId == CardId.PumpkingTheKingOfGraveGhosts
                || IsHublotId(activateId)
                || activateId == CardId.ChangshiTheSpiridao
                || activateId == CardId.OfficiatorOfDoomSamuel
                || activateId == CardId.PumpkingTheGreatGhostKing;
            if (keyPrompt)
            {
                DebugRoute("SELECT prompt activateId=" + activateId
                    + " hint=" + hint + " min=" + min + " max=" + max
                    + " pendingPump=" + pumpkingHandSelectionPending);
                DebugCards("CANDIDATES", cards);
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

            if (varudrasDestroySelectionPending
                || (activateId == CardId.Varudras && hint == HintMsg.Destroy))
            {
                List<ClientCard> enemyTargets = GetEnemyFieldPriority(cards, false)
                    .Where(c => c != null && c.Controller == 1 && c.IsOnField())
                    .Where(cards.Contains)
                    .Take(max)
                    .ToList();
                DebugCards("VARUDRAS ENEMY TARGETS", enemyTargets);
                if (enemyTargets.Count >= min)
                {
                    varudrasDestroySelectionPending = false;
                    return Util.CheckSelectCount(enemyTargets, cards, min, max);
                }

                // The activation and post-negate Yes/No handlers reject the effect
                // without an enemy field card, so reaching this branch indicates a
                // protocol/state mismatch. Never intentionally queue a friendly card.
                varudrasDestroySelectionPending = false;
                DebugRoute("ERROR Varudras target prompt contained no enemy card; refusing self target");
                return null;
            }

            if (IsHublotId(activateId))
            {
                if (hint == HintMsg.ToGrave)
                {
                    selectedHublotSendId = GetHublotSendTargetId(cards);
                    selectedHublotRecoverId = 0;
                    if (!HasPumpkingInHand()
                        && (HasPumpkingInGrave()
                            || selectedHublotSendId == CardId.PumpkingTheKingOfGraveGhosts))
                    {
                        selectedHublotRecoverId = CardId.PumpkingTheKingOfGraveGhosts;
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
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.ChangshiTheSpiridao,
                            CardId.ArmyOfTheHaunted,
                            CardId.OfficiatingReverie,
                            CardId.Hublot,
                            CardId.GreatMammothOfTheNetherworld,
                            CardId.StareOfTheSnakeHair);
                    }
                    break;

                case CardId.StareOfTheSnakeHair:
                    if (hint == HintMsg.AddToHand)
                    {
                        if (!HasPumpkingInHand()
                            && CheckRemainInDeck(CardId.EctoplasmicFortification) > 0)
                        {
                            return SelectByIdPriority(cards, min, max,
                                CardId.EctoplasmicFortification,
                                CardId.CallOfTheHaunted,
                                CardId.VortexOfTime);
                        }

                        return SelectByIdPriority(cards, min, max,
                            CardId.CallOfTheHaunted,
                            CardId.VortexOfTime,
                            CardId.EctoplasmicFortification);
                    }
                    return SelectEnemyField(cards, min, max);

                case CardId.EctoplasmicFortification:
                    if (hint == HintMsg.AddToHand)
                    {
                        // Latest confirmed route rule: Ectoplasmic searches Pumpking.
                        // Hublot is only a fallback if Pumpking is not in the actual
                        // candidate list supplied by the server.
                        DebugRoute("Ectoplasmic search target: Pumpking");
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
                        List<int> priority = new List<int>();
                        if (!HasDirectPumpkingLineAvailable()
                            && CanAcceptZombieLock()
                            && DefaultCheckWhetherBotCanSearch())
                        {
                            priority.Add(CardId.GlowUpBloom);
                        }
                        if (HasFaceupFieldSpell()
                            && !Bot.HasInGraveyard(CardId.EldlichTheGoldenLord))
                        {
                            priority.Add(CardId.EldlichTheGoldenLord);
                        }
                        priority.AddRange(new[]
                        {
                            CardId.OfficiatingReverie,
                            CardId.Mezuki,
                            CardId.ArmyOfTheHaunted,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.DoomkingBalerdroch,
                            CardId.ChangshiTheSpiridao
                        });
                        return SelectByIdPriority(cards, min, max, priority.ToArray());
                    }
                    break;

                case CardId.OfficiatingReverie:
                    if (hint == HintMsg.Discard || hint == HintMsg.ToGrave)
                        return SelectDiscard(cards, min, max);
                    if (hint == HintMsg.SpSummon)
                        return SelectZombieToRevive(cards, min, max);
                    if (hint == HintMsg.XyzMaterial)
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheGreatGhostKing,
                            CardId.TheUndyingLegion,
                            CardId.OfficiatorOfDoomSamuel,
                            CardId.DhampirVampireSheridan,
                            CardId.WollowFounderOfTheDrudgeDragons);
                    }
                    break;

                case CardId.ArmyOfTheHaunted:
                    if (hint == HintMsg.Set)
                        return SelectByIdPriority(cards, min, max, CardId.CallOfTheHaunted);
                    break;

                case CardId.CallOfTheHaunted:
                    if (hint == HintMsg.SpSummon)
                    {
                        if (Duel.Player == 0 && HasPumpkingInGrave())
                        {
                            return SelectByIdPriority(cards, min, max,
                                CardId.PumpkingTheKingOfGraveGhosts,
                                CardId.GreatMammothOfTheNetherworld,
                                CardId.StareOfTheSnakeHair,
                                CardId.Hublot,
                                CardId.Hublot);
                        }
                        return SelectZombieToRevive(cards, min, max);
                    }
                    break;

                case CardId.Mezuki:
                    if (hint == HintMsg.SpSummon)
                        return SelectZombieToRevive(cards, min, max);
                    break;

                case CardId.GlowUpBloom:
                    if (hint == HintMsg.AddToHand || hint == HintMsg.SpSummon)
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.EldlichTheGoldenLord,
                            CardId.DoomkingBalerdroch,
                            CardId.GreatMammothOfTheNetherworld,
                            CardId.StareOfTheSnakeHair);
                    }
                    break;

                case CardId.ChangshiTheSpiridao:
                    if (hint == HintMsg.ToGrave)
                    {
                        selectedChangshiMillId = GetChangshiMillTargetId(cards);
                        return SelectByIdPriority(cards, min, max, selectedChangshiMillId);
                    }
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
                        return SelectEnemyField(cards, min, max);
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

                        // Grave effect cost: send face-up Delta first so Delta loops
                        // itself back to the hand after Eldlich returns and summons.
                        List<ClientCard> fieldCosts = cards.Where(c => c.Controller == 0
                            && (c.IsSpell() || c.IsTrap()))
                            .OrderBy(c => c.IsCode(CardId.DeltaOfInvitation) ? 0 : 1)
                            .ThenBy(c => c.IsCode(CardId.CallOfTheHaunted) ? 10 : 1)
                            .ToList();
                        return Util.CheckSelectCount(fieldCosts, cards, min, max);
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
                    if (hint == HintMsg.RemoveXyz)
                    {
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.Hublot,
                            CardId.OfficiatingReverie,
                            CardId.ArmyOfTheHaunted);
                    }
                    if (hint == HintMsg.AddToHand)
                    {
                        List<int> priority = new List<int>();
                        if (Duel.Turn == 1)
                        {
                            // Turn 1: Army is always the Great Pumpking search.
                            priority.Add(CardId.ArmyOfTheHaunted);
                        }
                        else
                        {
                            // Later turns: use Snakehair only while its hand effect
                            // has not been used, then bridge into Ectoplasmic.
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
                            CardId.StareOfTheSnakeHair,
                            CardId.EctoplasmicFortification,
                            CardId.VortexOfTime,
                            CardId.CallOfTheHaunted
                        });
                        DebugRoute("Great Pumpking search priority="
                            + string.Join(",", priority.Select(id => id.ToString()).ToArray()));
                        return SelectByIdPriority(cards, min, max, priority.ToArray());
                    }
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
                    if (hint == HintMsg.RemoveXyz)
                    {
                        if (ashReplayLineActive)
                        {
                            return SelectByIdPriority(cards, min, max,
                                CardId.ArmyOfTheHaunted,
                                CardId.ChangshiTheSpiridao);
                        }
                        return SelectByIdPriority(cards, min, max,
                            CardId.PumpkingTheKingOfGraveGhosts,
                            CardId.ChangshiTheSpiridao,
                            CardId.OfficiatingReverie,
                            CardId.ArmyOfTheHaunted);
                    }
                    if (hint == HintMsg.SpSummon)
                        return SelectSamuelReviveTarget(cards, min, max);
                    if (hint == HintMsg.Disable)
                        return SelectEnemyField(cards, min, max);
                    if (cards.All(c => c.Location == CardLocation.Grave))
                    {
                        List<ClientCard> enemy = GetEnemyGravePriority(cards).Where(cards.Contains).ToList();
                        if (enemy.Count > 0)
                            return Util.CheckSelectCount(enemy, cards, min, max);
                        return SelectByIdPriority(cards, min, max,
                            CardId.GlowUpBloom,
                            CardId.Mezuki,
                            CardId.OfficiatingReverie,
                            CardId.ArmyOfTheHaunted);
                    }
                    break;

                case CardId.WollowFounderOfTheDrudgeDragons:
                case CardId.TheUndyingLegion:
                    if (cards.Any(c => c.Location == CardLocation.Grave))
                        return SelectEnemyGrave(cards, min, max);
                    return SelectEnemyField(cards, min, max);

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
                        return SelectByIdPriority(cards, min, max,
                            CardId.EldlichTheMadGoldenLord,
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
                    if (hint == HintMsg.SpSummon)
                    {
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
                    return SelectEnemyField(cards, min, max);
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

            return base.OnSelectLinkMaterial(cards, min, max);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override int OnSelectOption(IList<int> options)
        {
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
                return Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled());

            if (desc == Util.GetStringId(CardId.Varudras, 3))
            {
                bool acceptDestroy = Enemy.GetMonsters().Concat(Enemy.GetSpells())
                    .Any(c => c != null && c.IsOnField());
                varudrasDestroySelectionPending = acceptDestroy;
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

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            NamedCard data = NamedCard.Get(cardId);
            if (data != null && positions.Contains(CardPosition.FaceUpDefence))
            {
                if (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2)
                {
                    if (data.Defense >= data.Attack)
                        return CardPosition.FaceUpDefence;
                }
                if (Duel.Player == 1 && data.Defense > 0)
                    return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }

        private bool MonsterRepos()
        {
            if (Card == null || !Card.IsMonster())
                return false;

            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Main1
                && Card.Attack > Card.Defense && Enemy.GetMonsterCount() == 0)
            {
                return Card.IsDefense();
            }

            if (Card.Defense >= Card.Attack)
                return Card.IsAttack() && Util.IsTurn1OrMain2();

            return Card.IsDefense() && Duel.Player == 0 && Duel.Phase == DuelPhase.Main1;
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

                if (movedToOurHand)
                {
                    if (card.IsCode(CardId.PumpkingTheKingOfGraveGhosts,
                        CardId.Hublot,
                        CardId.EctoplasmicFortification))
                    {
                        pumpkingStarterSeenThisDuel = true;
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
                    if ((previousLocation & (int)CardLocation.Grave) != 0
                        && selectedSamuelReviveId != 0
                        && card.IsCode(selectedSamuelReviveId))
                    {
                        samuelRevivedCardId = card.Id;
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
                    }
                    else if (card.IsCode(CardId.TheUndyingLegion))
                    {
                        pumpkingComboState = PumpkingComboState.UndyingSummoned;
                    }
                    else if (card.IsCode(CardId.MercuriumTheLivingQuicksilver))
                    {
                        quicksilverLineActive = true;
                        pumpkingComboState = PumpkingComboState.QuicksilverSummoned;
                    }
                    else if (card.IsCode(CardId.GravityController))
                    {
                        pumpkingComboState = PumpkingComboState.GravityControllerSummoned;
                    }
                    else if (card.IsCode(CardId.FallenAngelOfTheGoldenLand))
                    {
                        eldlichRouteActive = true;
                    }
                    else if (card.IsCode(CardId.FlyingMary))
                    {
                        eldlichRouteMarySummoned = true;
                    }
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnDraw(int player)
        {
            base.OnDraw(player);
            if (player == 0)
                ObservePumpkingStarterState();
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo chain = Duel.GetCurrentSolvingChainInfo();
            if (chain != null && chain.ActivateController == 0 && !Duel.IsCurrentSolvingChainNegated())
            {
                activatedThisTurn.Add(chain.ActivateId);

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

                if (chain.ActivateId == CardId.EctoplasmicFortification
                    && Bot.HasInHand(CardId.PumpkingTheKingOfGraveGhosts))
                {
                    ectoplasmicSearchUsed = true;
                    pumpkingSearchSucceeded = true;
                    pumpkingComboState = PumpkingComboState.PumpkingReady;
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

                if (chain.ActivateId == CardId.PumpkingTheKingOfGraveGhosts)
                {
                    if (chain.ActivateLocation == CardLocation.Hand)
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
                    && chain.ActivateDescription == Util.GetStringId(CardId.OfficiatorOfDoomSamuel, 0))
                {
                    samuelReviveResolved = true;
                    if (selectedSamuelReviveId != 0
                        && Bot.HasInMonstersZone(selectedSamuelReviveId, faceUp: true))
                    {
                        samuelRevivedCardId = selectedSamuelReviveId;
                    }
                    pumpkingComboState = PumpkingComboState.SamuelRevived;
                }

                if (chain.ActivateId == CardId.PumpkingTheGreatGhostKing)
                {
                    if (chain.ActivateDescription == Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 1))
                    {
                        greatPumpkingSearchResolved = true;
                        DebugRoute("RESOLVED Great Pumpking search");
                    }
                    if (chain.ActivateDescription == Util.GetStringId(CardId.PumpkingTheGreatGhostKing, 2))
                    {
                        greatPumpkingBounceResolved = true;
                        DebugRoute("RESOLVED Great Pumpking bounce");
                        if (ashReplayLineActive && Bot.HasInHand(CardId.AshBlossom))
                        {
                            ashReplayLineActive = false;
                        }
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
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            currentNegateCardList.Clear();
            selectedHublotXyzId = 0;
            selectedChangshiMillId = 0;
            selectedDeltaSendId = 0;
            selectedSamuelReviveId = 0;
            pumpkingHandSelectionPending = false;
            pumpkingCallPromptCompleted = false;
            pumpkingDiscardSelfRequired = false;
            pendingPumpkingHandCard = null;
            doomkingOptionPending = false;
            doomkingPreferNegate = false;
            varudrasDestroySelectionPending = false;
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
            varudrasDestroySelectionPending = false;
            pumpkingSummonEffectAttempted = false;
            pumpkingSummonEffectResolved = false;
            changshiMillAttempted = false;
            changshiMillResolved = false;
            greatPumpkingSearchAttempted = false;
            greatPumpkingBounceAttempted = false;
            ectoplasmicSearchUsed = false;
            pumpkingSearchSucceeded = false;
            callSetByPumpking = false;
            samuelReviveResolved = false;
            greatPumpkingSearchResolved = false;
            greatPumpkingBounceResolved = false;
            quicksilverLineActive = false;
            quicksilverLoadedBloom = false;
            zombieLockedThisTurn = false;
            ashReplayLineActive = false;
            eldlichRouteActive = false;
            eldlichRouteMarySummoned = false;
            selectedHublotSendId = 0;
            selectedHublotRecoverId = 0;
            selectedHublotRecover = false;
            selectedHublotXyzId = 0;
            selectedChangshiMillId = 0;
            selectedDeltaSendId = 0;
            selectedSamuelReviveId = 0;
            samuelRevivedCardId = 0;
            summonCount = 1;
            base.OnNewTurn();
            ObservePumpkingStarterState();
            DebugRoute("NEW TURN " + Duel.Turn + " starterSeen=" + pumpkingStarterSeenThisDuel);
            DebugCards("HAND", Bot.Hand);
        }
    }
}
