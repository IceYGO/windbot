using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("SkyStriker", "AI_SkyStriker")]
    public class SkyStrikerExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Raye = 26077387;
            public const int Kagari = 63288573;
            public const int Shizuku = 90673288;
            public const int Hayate = 8491308;
            public const int Token = 52340445;

            public const int Engage = 63166095;
            public const int HornetDrones = 52340444;
            public const int WidowAnchor = 98338152;
            public const int SharkCannon = 51227866;
            public const int Afterburners = 99550630;
            public const int JammingWaves = 25955749;
            public const int Multirole = 24010609;
            public const int HerculesBase = 97616504;
            public const int AreaZero = 50005218;

            public const int AshBlossom = 14558127;
            public const int GhostOgre = 59438930;
            public const int MaxxC = 23434538;
            public const int GlowUpBulb = 67441435;
            public const int EffectVeiler = 97268402;

            public const int ReinforcementOfTheArmy = 32807846;
            public const int FoolishBurialGoods = 35726888;
            public const int UpstartGoblin = 70368879;
            public const int MetalfoesFusion = 73594093;
            public const int TwinTwisters = 43898403;
            public const int SolemnJudgment = 41420027;
            public const int SolemnWarning = 84749824;

            public const int BorrelswordDragon = 85289965;
            public const int TopologicBomberDragon = 5821478;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int KnightmareCerberus = 75452921;
            public const int CrystronHalqifibrax = 50588353;
            public const int Linkuriboh = 41999284;
        }

        private bool KagariSummoned;
        private bool ShizukuSummoned;
        private bool HayateSummoned;
        private bool BorrelswordAttackEffectUsed;
        private bool BorrelswordPositionEffectUsed;
        private bool GlowUpBulbEffectUsed; // Do not reset it
        private bool BorrelswordComboStarted;
        private bool TopologicComboStarted;
        private bool NormalSummoned;
        private bool RayeGraveEffectUsed;
        private bool AreaZeroSetForJamming;
        private bool WidowAnchorTakeControl;
        private bool SharkCannonBanishOnly;
        private ClientCard WidowAnchorTarget;
        private ClientCard SharkCannonTarget;
        private readonly List<ClientCard> CurrentChainHandledCards = new List<ClientCard>();
        private readonly List<ClientCard> MultiroleSetCards = new List<ClientCard>();
        private readonly List<ClientCard> ControlledEnemyCards = new List<ClientCard>();

        public SkyStrikerExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);

            AddExecutor(ExecutorType.Activate, CardId.Raye, RayeEffect);
            AddExecutor(ExecutorType.Activate, CardId.TwinTwisters, TwinTwistersEffect);

            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurialGoods, FoolishBurialGoodsEffect);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);

            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleActivation);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffectFirst);
            AddExecutor(ExecutorType.Activate, CardId.Afterburners, AfterburnersEffect);
            AddExecutor(ExecutorType.Activate, CardId.JammingWaves, JammingWavesEffect);
            AddExecutor(ExecutorType.SpellSet, CardId.AreaZero, AreaZeroSetForJammingWaves);

            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect);
            AddExecutor(ExecutorType.Activate, CardId.SharkCannon, SharkCannonEffect);
            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleIgnitionEffect);

            AddExecutor(ExecutorType.Activate, CardId.Engage, EngageEffect);
            AddExecutor(ExecutorType.Activate, CardId.HornetDrones, HornetDronesEffect);

            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion, MetalfoesFusionEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, LinkuribohSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Kagari, KagariSummon);
            AddExecutor(ExecutorType.Activate, CardId.Kagari, KagariEffect);

            AddExecutor(ExecutorType.Summon, CardId.GlowUpBulb, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.GhostOgre, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, TunerSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronHalqifibrax, CrystronHalqifibraxSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrystronHalqifibrax, CrystronHalqifibraxEffect);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, LinkuribohEffect);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BorrelswordDragon, BorrelswordDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.BorrelswordDragon, BorrelswordDragonEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix, KnightmarePhoenixEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus, KnightmareCerberusSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareCerberus, KnightmareCerberusEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.TopologicBomberDragon, TopologicBomberDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixUnlockSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus, KnightmareCerberusUnlockSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.Hayate, HayateSummon);
            AddExecutor(ExecutorType.Activate, CardId.Hayate, HayateEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Shizuku, ShizukuSummon);
            AddExecutor(ExecutorType.Activate, CardId.AreaZero, AreaZeroEffect);
            AddExecutor(ExecutorType.Summon, CardId.Raye, RayeSummon);

            AddExecutor(ExecutorType.Activate, CardId.HerculesBase, HerculesBaseEffect);

            // When Widow Anchor is absent from the GY, Shizuku can search it first,
            // use it on Shizuku, and let Multirole set it immediately afterwards.
            AddExecutor(ExecutorType.Activate, CardId.Shizuku, ShizukuFirstEndPhaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEndPhaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleEndPhaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.Shizuku, ShizukuEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.HornetDrones, SetHornetDrones);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnJudgment, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.WidowAnchor, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.SharkCannon, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.TwinTwisters, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.MetalfoesFusion, Util.IsTurn1OrMain2);
            AddExecutor(ExecutorType.SpellSet, CardId.HerculesBase, SetWhenHandIsFull);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            KagariSummoned = false;
            ShizukuSummoned = false;
            HayateSummoned = false;
            BorrelswordAttackEffectUsed = false;
            BorrelswordPositionEffectUsed = false;
            BorrelswordComboStarted = false;
            TopologicComboStarted = false;
            NormalSummoned = false;
            RayeGraveEffectUsed = false;
            AreaZeroSetForJamming = false;
            WidowAnchorTakeControl = false;
            SharkCannonBanishOnly = false;
            WidowAnchorTarget = null;
            SharkCannonTarget = null;
            ControlledEnemyCards.RemoveAll(card => card == null || card.Controller != 0 ||
                card.Location != CardLocation.MonsterZone);
            CurrentChainHandledCards.Clear();
            base.OnNewTurn();
        }

        public override void OnSummoning()
        {
            if (Duel.LastSummonPlayer == 0)
                NormalSummoned = true;
            base.OnSummoning();
        }

        public override void OnChainEnd()
        {
            CurrentChainHandledCards.Clear();
            base.OnChainEnd();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
            if (card != null && MultiroleSetCards.Contains(card) &&
                (currentControler != 0 || currentLocation != (int)CardLocation.SpellZone))
                MultiroleSetCards.Remove(card);

            if (card != null && ControlledEnemyCards.Contains(card) &&
                (currentControler != 0 ||
                (currentLocation & (int)CardLocation.MonsterZone) == 0))
                ControlledEnemyCards.Remove(card);

            if (card != null && previousControler == 0 && currentControler == 0 &&
                previousLocation == (int)CardLocation.Grave &&
                currentLocation == (int)CardLocation.SpellZone &&
                solving != null && solving.ActivatePlayer == 0 &&
                solving.IsActivateCode(CardId.Multirole))
                MultiroleSetCards.Add(card);

            bool gainedEnemyMonster = card != null && previousControler == 1 && currentControler == 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) != 0 &&
                solving != null && solving.ActivatePlayer == 0 &&
                solving.IsActivateCode(CardId.WidowAnchor, CardId.SharkCannon);
            if (gainedEnemyMonster)
            {
                if (!ControlledEnemyCards.Contains(card))
                    ControlledEnemyCards.Add(card);
                if (solving.IsActivateCode(CardId.WidowAnchor))
                    WidowAnchorTarget = card;
            }

            if (card != null && currentControler == 0 &&
                (previousLocation & (int)CardLocation.MonsterZone) == 0 &&
                (currentLocation & (int)CardLocation.MonsterZone) != 0)
            {
                if (card.IsCode(CardId.Kagari))
                    KagariSummoned = true;
                else if (card.IsCode(CardId.Shizuku))
                    ShizukuSummoned = true;
                else if (card.IsCode(CardId.Hayate))
                    HayateSummoned = true;
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max,
            int hint, bool cancelable)
        {
            ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
            if (solving != null && solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.Multirole))
            {
                List<ClientCard> ordered = OrderCardsById(cards, new[]
                {
                    CardId.Engage,
                    CardId.WidowAnchor,
                    CardId.SharkCannon,
                    CardId.HornetDrones,
                    CardId.Afterburners,
                    CardId.JammingWaves
                }, true);
                return Util.CheckSelectCount(ordered, cards, min, max);
            }

            if (solving != null && solving.ActivatePlayer == 0 && solving.IsActivateCode(CardId.AreaZero))
            {
                List<ClientCard> ordered = OrderCardsById(cards, GetSkyStrikerSearchPriority(true), false);
                return Util.CheckSelectCount(ordered, cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.TopologicBomberDragon &&
                    (available & Zones.ExtraMonsterZones) != 0)
                    return available & Zones.ExtraMonsterZones;

                bool shouldTriggerTopologic = false;
                int topologicLinkedZones = 0;
                ClientCard topologic = Bot.GetMonsters().FirstOrDefault(card =>
                    card.IsCode(CardId.TopologicBomberDragon) && !card.IsDisabled());
                if (topologic != null)
                {
                    topologicLinkedZones = topologic.GetLinkedZones() & available & Zones.MainMonsterZones;
                    shouldTriggerTopologic = topologicLinkedZones != 0 &&
                        (Enemy.GetMonstersInMainZone().Count >= 2 ||
                        Util.GetProblematicEnemyMonster()?.Sequence < 5);
                }

                if (shouldTriggerTopologic)
                {
                    return topologicLinkedZones & -topologicLinkedZones;
                }

                bool shouldAvoidTopologicLinkedZones = topologicLinkedZones != 0 && (available & ~topologicLinkedZones) != 0;
                if (shouldAvoidTopologicLinkedZones)
                    available &= ~topologicLinkedZones;

                if ((cardId == CardId.KnightmarePhoenix || cardId == CardId.KnightmareCerberus) &&
                    (available & Zones.ExtraMonsterZones) != 0)
                    return available & Zones.ExtraMonsterZones;

                if (cardId == CardId.Hayate && (available & Zones.z5) != 0)
                    return Zones.z5;

                ClientCard hayate = Bot.MonsterZone.GetFirstMatchingCard(card => card.IsCode(CardId.Hayate));
                if ((hayate != null && cardId == CardId.Linkuriboh) ||
                    (hayate != null && cardId == CardId.CrystronHalqifibrax) ||
                    (hayate != null && cardId == CardId.BorrelswordDragon))
                {
                    int linked = hayate.GetLinkedZones() & available & Zones.MainMonsterZones;
                    if (linked != 0)
                        return linked & -linked;
                }

                if (shouldAvoidTopologicLinkedZones)
                    return available;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            ClientCard hayate = attackers.FirstOrDefault(card => card.IsCode(CardId.Hayate));
            if (hayate != null && GetHayateCrashTarget(hayate, defenders) != null)
                return hayate;

            if (Bot.HasInMonstersZone(CardId.BorrelswordDragon, true))
            {
                ClientCard raye = attackers.FirstOrDefault(card => card.IsCode(CardId.Raye));
                if (raye != null)
                    return raye;

                ClientCard borrelsword = attackers.FirstOrDefault(card => card.IsCode(CardId.BorrelswordDragon));
                if (borrelsword != null)
                    return borrelsword;
            }
            return base.OnSelectAttacker(attackers, defenders);
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (defenders.Count == 0 && Bot.HasInMonstersZone(CardId.BorrelswordDragon, true))
            {
                ClientCard raye = attackers.FirstOrDefault(card => card.IsCode(CardId.Raye));
                if (raye != null)
                    return AI.Attack(raye, null);
            }
            return base.OnBattle(attackers, defenders);
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (attacker.IsCode(CardId.Hayate))
            {
                ClientCard target = GetHayateCrashTarget(attacker, defenders);
                if (target != null)
                {
                    return AI.Attack(attacker, target);
                }
                if (attacker.CanDirectAttack)
                    return AI.Attack(attacker, null);
            }
            return base.OnSelectAttackTarget(attacker, defenders);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle() &&
                attacker.IsCode(CardId.BorrelswordDragon) && !attacker.IsDisabled() &&
                !BorrelswordAttackEffectUsed && defender.IsFaceup())
            {
                int halfAttack = (defender.Attack + 1) / 2;
                attacker.RealPower += halfAttack;
                if (defender.IsAttack())
                    defender.RealPower = halfAttack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.Engage, 0))
                return true;
            if (desc == Util.GetStringId(CardId.AreaZero, 2))
                return true;
            if (desc == Util.GetStringId(CardId.WidowAnchor, 0))
            {
                if (Duel.Player == 1)
                {
                    if (Bot.UnderAttack && WidowAnchorTakeControl &&
                        WidowAnchorTarget == Enemy.BattlingMonster)
                        return true;
                    return !Bot.GetSpells().Any(card => card.IsFacedown() &&
                        card.IsCode(CardId.WidowAnchor));
                }
                return WidowAnchorTakeControl && WidowAnchorTarget != null;
            }
            if (desc == Util.GetStringId(CardId.SharkCannon, 0))
                return !SharkCannonBanishOnly && ShouldReviveWithSharkCannon(SharkCannonTarget);
            if (desc == Util.GetStringId(CardId.KnightmarePhoenix, 1) ||
                desc == Util.GetStringId(CardId.KnightmareCerberus, 1) ||
                desc == Util.GetStringId(CardId.KnightmareUnicorn, 1))
                return true;
            if (desc == Util.GetStringId(CardId.JammingWaves, 0))
            {
                ClientCard target = Util.GetBestEnemyMonster();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }
            if (desc == Util.GetStringId(CardId.Afterburners, 0))
            {
                ClientCard target = Util.GetBestEnemySpell();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }
            return base.OnSelectYesNo(desc);
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            AI.SelectCard(CardId.Raye);
            return true;
        }

        private bool FoolishBurialGoodsEffect()
        {
            if (Bot.HasInDeck(CardId.MetalfoesFusion))
                AI.SelectCard(CardId.MetalfoesFusion);
            else if (!KagariSummoned && Bot.HasInExtra(CardId.Kagari))
                AI.SelectCard(CardId.Engage, CardId.WidowAnchor, CardId.HornetDrones);
            else
                AI.SelectCard(CardId.ReinforcementOfTheArmy, CardId.UpstartGoblin, CardId.TwinTwisters);
            return true;
        }

        private bool TwinTwistersEffect()
        {
            if (Util.ChainContainsCard(CardId.TwinTwisters))
                return false;

            List<ClientCard> targets = Enemy.GetSpells()
                .Where(card => card.IsFloodgate() || card.IsFacedown() ||
                    card.HasType(CardType.Continuous) || card.HasType(CardType.Equip) ||
                    card.HasType(CardType.Field) || card.HasType(CardType.Pendulum))
                .Where(card => !IsCurrentChainHandled(card))
                .Distinct()
                .ToList();
            if (targets.Count == 0)
                return false;

            bool hasPriorityTarget = targets.Any(card => card.IsFloodgate() || card.IsFaceup());
            if (Duel.Player == 1 && !hasPriorityTarget && Duel.Phase != DuelPhase.End &&
                targets.Count < 2)
                return false;

            targets = targets
                .OrderBy(card => card.IsFloodgate() ? 0 : card.IsFaceup() ? 1 : 2)
                .Take(2)
                .ToList();

            foreach (ClientCard target in targets)
                MarkCurrentChainHandled(target);
            SelectDiscard();
            AI.SelectNextCard(targets);
            return true;
        }

        private bool MultiroleActivation()
        {
            return Card.Location == CardLocation.Hand || Card.IsFacedown();
        }

        private bool MultiroleIgnitionEffect()
        {
            if (Card.Location != CardLocation.SpellZone || Card.IsFacedown() ||
                (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2))
                return false;

            ClientCard stolen = Bot.GetMonsters().FirstOrDefault(ControlledEnemyCards.Contains);
            if (stolen != null && Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectCard(stolen);
                return true;
            }

            ClientCard raye = Bot.GetMonsters().FirstOrDefault(card =>
                card.IsCode(CardId.Raye) && Bot.GetMonstersExtraZoneCount() == 0);
            if (raye != null && HasAvailableRayeExtraDeckTarget())
            {
                AI.SelectCard(raye);
                return true;
            }

            ClientCard areaZero = Bot.GetFieldSpellCard();
            if (areaZero != null && areaZero.IsCode(CardId.AreaZero) &&
                Bot.GetMonsterCount() == 0 && Bot.HasInDeck(CardId.Raye))
            {
                AI.SelectCard(areaZero);
                return true;
            }

            bool preserveHornetDrones = HasTopologicComboPayoff() &&
                (TopologicComboStarted || ShouldStartTopologicCombo());
            ClientCard spentSpell = Bot.GetSpells().Where(card =>
                card != Card && card.IsSpell() && !MultiroleSetCards.Contains(card) &&
                !card.IsCode(CardId.WidowAnchor, CardId.Engage) &&
                (!preserveHornetDrones || !card.IsCode(CardId.HornetDrones)))
                .OrderBy(card => card.Id == CardId.MetalfoesFusion ? 0 : 1).FirstOrDefault();
            if (spentSpell != null &&
                (Bot.Graveyard.Count(card => card.IsSpell()) == 2 || spentSpell.Id == CardId.MetalfoesFusion))
            {
                AI.SelectCard(spentSpell);
                return true;
            }
            return false;
        }

        private bool MultiroleEndPhaseEffect()
        {
            return Duel.Phase == DuelPhase.End;
        }

        private bool AfterburnersEffect()
        {
            ClientCard target = GetEnemyMonsterTarget(false);
            if (target == null)
                return false;
            MarkCurrentChainHandled(target);
            AI.SelectCard(target);
            return true;
        }

        private bool JammingWavesEffect()
        {
            ClientCard target = Enemy.GetSpells().FirstOrDefault(card => card.IsFacedown());
            if (target == null)
            {
                ClientCard field = Bot.GetFieldSpellCard();
                if (field != null && field.IsFacedown())
                    target = field;
            }
            if (target == null)
                return false;
            AreaZeroSetForJamming = false;
            MarkCurrentChainHandled(target);
            AI.SelectCard(target);
            return true;
        }

        private bool EngageEffect()
        {
            AI.SelectCard(GetSkyStrikerSearchPriority(false));
            return true;
        }

        private bool HornetDronesEffect()
        {
            if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon, true) &&
                Enemy.GetMonstersInMainZone().Count >= 2 &&
                !Duel.CurrentChainInfo.Any(chain => chain.ActivatePlayer == 0 &&
                    chain.IsActivateCode(CardId.TopologicBomberDragon, CardId.HornetDrones)))
                return true;

            if (Duel.CurrentChain.Count > 0 || Duel.Player == 1)
                return false;

            if (Duel.Phase != DuelPhase.Main1)
                return false;

            if (Bot.GetMonstersExtraZoneCount() == 0)
                return true;

            if (!Bot.HasInMonstersZone(CardId.Linkuriboh))
            {
                if (!ShouldStartBorrelswordCombo(true))
                    return false;
                BorrelswordComboStarted = true;
                return true;
            }

            return false;
        }

        private bool WidowAnchorEffectFirst()
        {
            if (Duel.Player != 0 || Duel.CurrentChain.Count > 0)
                return false;

            ClientCard target = GetEnemyMonsterTarget(true);
            if (target == null)
                return false;

            bool canTakeControl = CanImmediatelyUseControlledMonster(target,
                Duel.Phase < DuelPhase.Main2);
            bool shouldNegate = target.IsFloodgate() || target.IsMonsterDangerous() ||
                target.IsMonsterInvincible() || target.IsMonsterShouldBeDisabledBeforeItUseEffect();
            if (!shouldNegate && !canTakeControl)
                return false;
            if (!canTakeControl && Bot.HasInHand(CardId.Afterburners))
                return false;

            WidowAnchorTarget = target;
            WidowAnchorTakeControl = canTakeControl;
            MarkCurrentChainHandled(target);
            AI.SelectCard(target);
            return true;
        }

        private bool WidowAnchorEffect()
        {
            if (Duel.CurrentChain.Any(card => card.Controller == 0 && card.IsCode(CardId.WidowAnchor)))
                return false;

            ClientCard target = Enemy.BattlingMonster;
            if (Duel.CurrentChain.Count == 0 && Duel.Player == 1 && Bot.UnderAttack &&
                HaveThreeSpellsInGrave() && target != null &&
                target.HasType(CardType.Effect) && !target.IsDisabled() &&
                !target.IsShouldNotBeTarget() && !target.IsShouldNotBeSpellTrapTarget())
            {
                ClientCard defender = Bot.BattlingMonster;
                int defenderPower = defender == null ? 0 : defender.GetDefensePower();
                bool wouldDestroyDefender = defender != null &&
                    (target.Attack > defenderPower || target.Attack == defenderPower && defender.IsAttack());
                int battleDamage = defender == null ? target.Attack :
                    defender.IsAttack() ? Math.Max(0, target.Attack - defenderPower) : 0;
                bool shouldStopAttack = battleDamage >= 2000 || battleDamage >= Bot.LifePoints ||
                    wouldDestroyDefender && !IsDisposableMonster(defender);
                if (shouldStopAttack)
                {
                    WidowAnchorTarget = target;
                    WidowAnchorTakeControl = true;
                    MarkCurrentChainHandled(target);
                    AI.SelectCard(target);
                    return true;
                }
            }

            target = DefaultGetDisableMonsterTarget();
            if (target != null)
            {
                WidowAnchorTarget = target;
                WidowAnchorTakeControl = CanImmediatelyUseControlledMonster(target,
                    Duel.Player == 0 && Duel.Phase < DuelPhase.Main2);
                MarkCurrentChainHandled(target);
                AI.SelectCard(target);
                return true;
            }

            if (Duel.CurrentChain.Count > 0 || !HaveThreeSpellsInGrave() || Duel.Player != 0 ||
                Duel.Phase < DuelPhase.Main1 || Duel.Phase >= DuelPhase.Main2)
                return false;

            target = GetEnemyMonsterTarget(false);
            if (target == null || target.IsDisabled() || target.HasType(CardType.Normal))
                return false;
            if (!CanImmediatelyUseControlledMonster(target, true))
                return false;
            WidowAnchorTakeControl = true;
            WidowAnchorTarget = target;
            MarkCurrentChainHandled(target);
            AI.SelectCard(target);
            return true;
        }

        private bool CanImmediatelyUseControlledMonster(ClientCard target, bool canAttack)
        {
            if (target == null || target.Controller != 1 || !HaveThreeSpellsInGrave() || Duel.Phase == DuelPhase.End)
                return false;
            if (Duel.Player == 1)
                return true;

            if (target.IsCode(CardId.Raye, CardId.Kagari, CardId.Shizuku, CardId.Hayate))
            {
                bool canSummonKagari = !target.IsCode(CardId.Kagari) && !KagariSummoned &&
                    Bot.HasInExtra(CardId.Kagari);
                bool canSummonHayate = !target.IsCode(CardId.Hayate) && !HayateSummoned &&
                    Bot.HasInExtra(CardId.Hayate);
                if (canSummonKagari || canSummonHayate)
                    return true;
            }

            List<ClientCard> monsters = Bot.GetMonsters()
                .Where(card => card.IsFaceup() && !card.IsCode(CardId.BorrelswordDragon))
                .ToList();
            bool hasDifferentNameMaterial = monsters.Any(card => card.Id != target.Id);
            if (hasDifferentNameMaterial)
            {
                bool canUsePhoenix = Bot.HasInExtra(CardId.KnightmarePhoenix) &&
                    Util.GetProblematicEnemySpell() != null;
                ClientCard cerberusTarget = GetProblematicKnightmareCerberusTarget();
                bool canUseCerberus = Bot.HasInExtra(CardId.KnightmareCerberus) &&
                    cerberusTarget != null && cerberusTarget != target;
                bool canUnlockSpell =
                    Bot.HasInHandOrInSpellZone(new[]
                    {
                        CardId.Engage,
                        CardId.HornetDrones,
                        CardId.Afterburners,
                        CardId.JammingWaves,
                        CardId.SharkCannon
                    }) && Bot.HasInExtra(new[]
                    {
                        CardId.KnightmarePhoenix,
                        CardId.KnightmareCerberus
                    });
                if (canUsePhoenix || canUseCerberus || canUnlockSpell)
                    return true;
            }

            if (canAttack && target.Attack >= 2000)
                return true;

            if (Bot.HasInExtra(CardId.KnightmareUnicorn) &&
                Util.GetProblematicEnemyCard(0, true) != null)
            {
                if (monsters.Any(card => card.Id != target.Id &&
                    (card.HasType(CardType.Link) && card.LinkCount == 2 ||
                        target.HasType(CardType.Link) && target.LinkCount == 2)))
                    return true;
                if (monsters.Where(card => card.Id != target.Id)
                    .Select(card => card.Id).Distinct().Count() >= 2)
                    return true;
            }

            if (!Bot.HasInExtra(CardId.BorrelswordDragon) || !target.HasType(CardType.Effect))
                return false;
            List<ClientCard> effectMonsters = monsters
                .Where(card => card.HasType(CardType.Effect))
                .ToList();
            effectMonsters.Add(target);
            int subsetCount = 1 << effectMonsters.Count;
            for (int mask = 1; mask < subsetCount; mask++)
            {
                List<ClientCard> selected = new List<ClientCard>();
                for (int i = 0; i < effectMonsters.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        selected.Add(effectMonsters[i]);
                }
                if (selected.Count >= 3 && selected.Count <= 4 &&
                    selected.Contains(target) && CanMakeLinkRating(selected, 4))
                    return true;
            }
            return false;
        }

        private bool SharkCannonEffect()
        {
            if (Util.ChainContainsCard(CardId.SharkCannon))
                return false;

            List<ClientCard> targets = Enemy.GetGraveyardMonsters()
                .OrderByDescending(card => card.Attack)
                .ToList();

            ChainInfo lastChain = Duel.CurrentChainInfo.LastOrDefault();
            if (lastChain != null && lastChain.ActivatePlayer == 1)
            {
                ClientCard chainTarget = targets.FirstOrDefault(card =>
                    lastChain.Targets.Contains(card));
                if (chainTarget == null && lastChain.HasLocation(CardLocation.Grave) &&
                    lastChain.RelatedCard != null && targets.Contains(lastChain.RelatedCard))
                    chainTarget = lastChain.RelatedCard;
                if (chainTarget != null)
                {
                    SharkCannonBanishOnly = true;
                    SharkCannonTarget = chainTarget;
                    AI.SelectCard(chainTarget);
                    return true;
                }
            }

            if (Duel.CurrentChain.Count > 0)
                return false;

            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1 ||
                !EmptyMainMonsterZone() || !HaveThreeSpellsInGrave())
                return false;

            if (Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) &&
                !BorrelswordPositionEffectUsed)
            {
                ClientCard borrelswordTarget = targets.FirstOrDefault(card =>
                    card.IsCanRevive() && IsBorrelswordTargetableMonster(card));
                if (borrelswordTarget != null)
                {
                    SharkCannonBanishOnly = false;
                    SharkCannonTarget = borrelswordTarget;
                    AI.SelectCard(borrelswordTarget);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }
            }

            if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon, true))
            {
                bool shouldTriggerTopologic = Enemy.GetMonstersInMainZone().Count >= 2 ||
                        Util.GetProblematicEnemyMonster()?.Sequence < 5;
                if (!shouldTriggerTopologic)
                    return false;
            }

            ClientCard target = targets.FirstOrDefault(ShouldReviveWithSharkCannon);
            if (target == null)
                return false;

            SharkCannonBanishOnly = false;
            SharkCannonTarget = target;
            AI.SelectCard(target);
            return true;
        }

        private bool ShouldReviveWithSharkCannon(ClientCard target)
        {
            if (!target.IsCanRevive())
                return false;
            if (Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) &&
                !BorrelswordPositionEffectUsed && IsBorrelswordTargetableMonster(target))
                return true;
            return CanImmediatelyUseControlledMonster(target, false);
        }

        private bool HerculesBaseEffect()
        {
            if (Card.Location == CardLocation.SpellZone && !Card.IsFacedown())
                return true;

            if (Card.Location == CardLocation.Grave)
            {
                List<ClientCard> targets = Bot.Graveyard
                    .Where(card => !card.IsCode(CardId.HerculesBase) && card.HasSetcode(0x115))
                    .ToList();
                AI.SelectCard(OrderCardsById(targets, new[]
                {
                    CardId.Engage,
                    CardId.Kagari,
                    CardId.Shizuku,
                    CardId.WidowAnchor,
                    CardId.HornetDrones,
                    CardId.SharkCannon,
                    CardId.Afterburners,
                    CardId.JammingWaves,
                    CardId.Hayate,
                    CardId.Raye,
                    CardId.AreaZero,
                    CardId.Multirole
                }, false));
                return true;
            }

            if (Util.IsTurn1OrMain2())
                return false;
            ClientCard monster = Util.GetBestBotMonster(true);
            if (monster == null)
                return false;

            int beatable = Enemy.GetMonsters().Count(target =>
                target.GetDefensePower() < monster.Attack && !target.IsMonsterInvincible());
            if (beatable == 0 || (beatable < 2 && !HaveThreeSpellsInGrave()))
                return false;
            AI.SelectCard(monster);
            return true;
        }

        private bool AreaZeroSetForJammingWaves()
        {
            bool shouldSet = Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 && EmptyMainMonsterZone() &&
                GetSpellCountInGrave() <= 2 && Bot.HasInHand(CardId.JammingWaves) &&
                Bot.HasInDeck(CardId.Raye) && !Enemy.GetSpells().Any(card => card.IsFacedown());
            if (shouldSet)
                AreaZeroSetForJamming = true;
            return shouldSet;
        }

        private bool AreaZeroEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return Bot.GetFieldSpellCard() == null;
            if (Card.IsFacedown())
                return !AreaZeroSetForJamming;
            if (Card.Location == CardLocation.Grave)
            {
                return Bot.GetMonsterCount() == 0 || !EmptyMainMonsterZone() || Duel.Player == 1;
            }

            if (Card.Location != CardLocation.SpellZone && Card.Location != CardLocation.FieldZone)
                return false;

            List<ClientCard> mainMonsters = Bot.GetMonstersInMainZone();
            bool hasBlockedSpell = HasUsefulBlockedSkyStrikerSpell();
            ClientCard controlledMonster = mainMonsters.FirstOrDefault(card =>
                ControlledEnemyCards.Contains(card));
            if (controlledMonster != null &&
                (hasBlockedSpell || Duel.Phase == DuelPhase.Main2))
            {
                AI.SelectCard(controlledMonster);
                return true;
            }

            ClientCard disposableMonster = mainMonsters
                .Where(IsDisposableMonster)
                .OrderBy(GetLinkMaterialValue)
                .FirstOrDefault();
            if (disposableMonster != null &&
                (hasBlockedSpell || Duel.Phase == DuelPhase.Main2))
            {
                AI.SelectCard(disposableMonster);
                return true;
            }

            ClientCard raye = mainMonsters.FirstOrDefault(card => card.IsCode(CardId.Raye));
            if (raye != null && mainMonsters.Count == 1 && Bot.GetMonstersExtraZoneCount() == 0 &&
                HasAvailableRayeExtraDeckTarget())
            {
                AI.SelectCard(raye);
                return true;
            }

            if (Duel.Phase != DuelPhase.Main2 && !SetWhenHandIsFull())
                return false;
            ClientCard spentSpell = Bot.GetSpells().FirstOrDefault(card =>
                card != Card && card.IsSpell() &&
                !card.IsCode(CardId.Multirole, CardId.Engage, CardId.WidowAnchor,
                    CardId.HornetDrones, CardId.SharkCannon));
            if (spentSpell == null)
                return false;
            AI.SelectCard(spentSpell);
            return true;
        }

        private bool RayeSummon()
        {
            return Bot.GetMonstersExtraZoneCount() == 0 ||
                (Enemy.GetMonsterCount() == 0 && Duel.Phase == DuelPhase.Main1);
        }

        private bool HasAvailableRayeExtraDeckTarget()
        {
            return (!KagariSummoned && Bot.HasInExtra(CardId.Kagari)) ||
                (!ShizukuSummoned && Bot.HasInExtra(CardId.Shizuku)) ||
                (!HayateSummoned && Bot.HasInExtra(CardId.Hayate));
        }

        private bool RayeEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Card.Location == CardLocation.Grave)
            {
                if (RayeGraveEffectUsed)
                    return false;
                RayeGraveEffectUsed = true;
                AI.SelectPosition(Duel.Player == 0 ? CardPosition.FaceUpAttack : CardPosition.FaceUpDefence);
                return true;
            }
            bool shouldTag = Util.IsChainTarget(Card) ||
                Card == Bot.BattlingMonster && Duel.Player == 1 ||
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main2;

            if (Duel.Player == 0 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && Card.Attacked)
            {
                if (Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) &&
                    !BorrelswordPositionEffectUsed)
                    return false;
                shouldTag = true;
            }

            if (!shouldTag)
                return false;
            RayeSelectTarget();
            return true;
        }

        private void RayeSelectTarget()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 &&
                Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) && !ShizukuSummoned)
            {
                AI.SelectCard(CardId.Shizuku, CardId.Kagari, CardId.Hayate);
                return;
            }

            if (!KagariSummoned && Bot.HasInGraveyard(new[]
            {
                CardId.Engage,
                CardId.HornetDrones,
                CardId.WidowAnchor,
                CardId.Afterburners,
                CardId.JammingWaves
            }))
                AI.SelectCard(CardId.Kagari, CardId.Shizuku, CardId.Hayate);
            else
                AI.SelectCard(CardId.Shizuku, CardId.Kagari, CardId.Hayate);
        }

        private bool KagariSummon()
        {
            if (KagariSummoned)
                return false;
            if (!Bot.HasInGraveyard(new[]
            {
                CardId.Engage,
                CardId.HornetDrones,
                CardId.WidowAnchor,
                CardId.Afterburners,
                CardId.JammingWaves
            }))
                return false;
            KagariSummoned = true;
            return true;
        }

        private bool KagariEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            ClientCard target = Bot.GetGraveyardSpells().FirstOrDefault(card => card.IsCode(CardId.Engage));
            if (target == null && EmptyMainMonsterZone() && Util.GetProblematicEnemyMonster() != null)
                target = Bot.GetGraveyardSpells().FirstOrDefault(card => card.IsCode(CardId.Afterburners));
            if (target == null && EmptyMainMonsterZone() && Util.GetProblematicEnemySpell() != null)
                target = Bot.GetGraveyardSpells().FirstOrDefault(card => card.IsCode(CardId.JammingWaves));
            if (target != null)
                AI.SelectCard(target);
            else
                AI.SelectCard(CardId.HornetDrones, CardId.WidowAnchor, CardId.SharkCannon, CardId.Engage);
            return true;
        }

        private bool HayateSummon()
        {
            if (Util.IsTurn1OrMain2())
                return false;
            if (HaveThreeSpellsInGrave() && !Util.IsAllEnemyBetter())
                return false;
            HayateSummoned = true;
            return true;
        }

        private bool HayateEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) &&
                !Bot.HasInGraveyard(CardId.Raye) && Bot.HasInDeck(CardId.Raye))
                AI.SelectCard(CardId.Raye);
            else if (!KagariSummoned && !Bot.HasInGraveyard(CardId.Engage) && Bot.HasInDeck(CardId.Engage))
                AI.SelectCard(CardId.Engage);
            else if (!HaveThreeSpellsInGrave())
                AI.SelectCard(CardId.Engage, CardId.WidowAnchor, CardId.HornetDrones, CardId.SharkCannon);
            else if (!Bot.HasInGraveyard(CardId.Raye) && Bot.HasInDeck(CardId.Raye))
                AI.SelectCard(CardId.Raye);
            else
                AI.SelectCard(CardId.Engage, CardId.WidowAnchor, CardId.HornetDrones, CardId.SharkCannon);
            return true;
        }

        private ClientCard GetHayateCrashTarget(ClientCard hayate, IList<ClientCard> defenders)
        {
            if (Duel.Player != 0 || Duel.Phase <= DuelPhase.Main1 || Duel.Phase >= DuelPhase.Main2)
                return null;
            if (!Bot.HasInMonstersZone(CardId.BorrelswordDragon, true) ||
                RayeGraveEffectUsed || hayate.IsDisabled() ||
                !Bot.HasInGraveyard(CardId.Raye))
                return null;

            return defenders
                .Where(card => IsHayateCrashTarget(hayate, card))
                .OrderBy(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        private bool IsHayateCrashTarget(ClientCard hayate, ClientCard target)
        {
            if (hayate == null || target == null || !target.IsAttack() ||
                target.IsMonsterHasPreventActivationEffectInBattle())
                return false;
            int hayatePower = hayate.GetAttackPower();
            return target.GetDefensePower() >= hayatePower &&
                target.GetDefensePower() - hayatePower < Bot.LifePoints;
        }

        private bool IsBorrelswordPositionTarget(ClientCard target)
        {
            return target != null && target.IsFaceup() && target.IsAttack() &&
                IsBorrelswordTargetableMonster(target);
        }

        private bool IsBorrelswordTargetableMonster(ClientCard target)
        {
            return target != null && !target.HasType(CardType.Link) &&
                !target.IsShouldNotBeTarget() && !target.IsShouldNotBeMonsterTarget();
        }

        private bool CanPrepareBorrelswordTargetWithSharkCannon()
        {
            return HaveThreeSpellsInGrave() &&
                Bot.HasInHandOrInSpellZone(CardId.SharkCannon) &&
                Enemy.GetGraveyardMonsters().Any(card =>
                    card.IsCanRevive() && IsBorrelswordTargetableMonster(card));
        }

        private bool CanUseGlowUpBulbEffect()
        {
            return !GlowUpBulbEffectUsed &&
                (Bot.HasInDeck(CardId.GlowUpBulb) ||
                    Bot.HasInHandOrInGraveyard(CardId.GlowUpBulb));
        }

        private bool ShouldStartBorrelswordCombo(bool hornetWillProvideMaterial)
        {
            if (Duel.Player != 0 || Duel.Turn == 1 || Duel.Phase != DuelPhase.Main1 ||
                NormalSummoned ||
                !Bot.HasInExtra(CardId.CrystronHalqifibrax) ||
                !Bot.HasInExtra(CardId.BorrelswordDragon) ||
                !Bot.Hand.Any(card => card.IsTuner()) ||
                !CanUseGlowUpBulbEffect())
                return false;

            bool canBuildCombo = Bot.GetMonsterCount() >= 2 ||
                Bot.HasInMonstersZone(CardId.Linkuriboh) ||
                Bot.HasInMonstersZone(CardId.Token) && Bot.HasInExtra(CardId.Linkuriboh) ||
                hornetWillProvideMaterial && Bot.HasInExtra(CardId.Linkuriboh);
            if (!canBuildCombo)
                return false;

            ClientCard hayate = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.Hayate));
            if (hayate != null)
            {
                return !hayate.IsDisabled() && !RayeGraveEffectUsed &&
                    Bot.HasInGraveyard(CardId.Raye) &&
                    Enemy.GetMonsters().Any(card => IsHayateCrashTarget(hayate, card));
            }

            int enemyPositionTargets = Enemy.GetMonsters().Count(IsBorrelswordPositionTarget);
            bool hasOwnPositionTarget = Bot.GetMonsters().Any(IsBorrelswordPositionTarget);
            return enemyPositionTargets >= 3 || hasOwnPositionTarget ||
                CanPrepareBorrelswordTargetWithSharkCannon();
        }

        private bool ShouldStartTopologicCombo()
        {
            ClientCard material = Bot.GetMonsters().FirstOrDefault(card =>
                card.IsFaceup() && (IsDisposableMonster(card) ||
                    card.IsCode(CardId.Raye, CardId.Kagari, CardId.Hayate, CardId.Shizuku)));
            bool hasNormalSummonTuner = Bot.Hand.Any(card => card.IsTuner());
            return Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                !BorrelswordComboStarted && !TopologicComboStarted && !NormalSummoned &&
                material != null && hasNormalSummonTuner &&
                HasTopologicComboPayoff() &&
                Bot.HasInExtra(CardId.CrystronHalqifibrax) &&
                Bot.HasInExtra(CardId.KnightmareUnicorn) &&
                Bot.HasInExtra(CardId.TopologicBomberDragon) &&
                CanUseGlowUpBulbEffect();
        }

        private bool HasTopologicComboPayoff()
        {
            return (Util.IsTurn1OrMain2() || Enemy.GetMonstersInMainZone().Count >= 2) &&
                Bot.HasInHandOrInSpellZone(CardId.HornetDrones);
        }

        private bool MetalfoesFusionEffect()
        {
            return Bot.Graveyard.Count(card => card.IsSpell()) > 3 || Bot.GetHandCount() < 3;
        }

        private bool LinkuribohSummon()
        {
            ClientCard token = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.Token));
            if (token == null || !BorrelswordComboStarted ||
                !Bot.HasInExtra(CardId.BorrelswordDragon))
                return false;
            AI.SelectMaterials(token);
            return true;
        }

        private bool TunerSummon()
        {
            if (!Bot.HasInExtra(CardId.CrystronHalqifibrax)) return false;
            if (Bot.GetMonsterCount() == 0) return false;

            bool borrelswordCombo = BorrelswordComboStarted &&
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                Bot.HasInMonstersZone(CardId.Linkuriboh) &&
                Bot.HasInExtra(CardId.BorrelswordDragon);
            if (borrelswordCombo)
                return true;

            if (Card.IsCode(CardId.GlowUpBulb) &&
                !Bot.HasInDeck(new[] { CardId.EffectVeiler, CardId.GhostOgre, CardId.AshBlossom }) &&
                !Bot.HasInHand(new[] { CardId.EffectVeiler, CardId.GhostOgre, CardId.AshBlossom }))
                return false;

            bool topologicCombo = ShouldStartTopologicCombo();
            if (topologicCombo)
            {
                TopologicComboStarted = true;
                return true;
            }

            borrelswordCombo = ShouldStartBorrelswordCombo(false);
            if (borrelswordCombo)
            {
                BorrelswordComboStarted = true;
                return true;
            }

            return Bot.GetMonsterCount() > 0 && !Bot.ExtraDeck.Any(card => card.HasSetcode(0x115));
        }

        private bool CrystronHalqifibraxSummon()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1)
                return false;

            if (BorrelswordComboStarted && Bot.HasInMonstersZone(CardId.Linkuriboh))
            {
                ClientCard tuner = Bot.GetMonsters().FirstOrDefault(card =>
                    card.IsTuner() && card.IsFaceup() && !card.IsCode(CardId.Hayate));
                ClientCard linkuriboh = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.Linkuriboh));
                if (tuner == null || linkuriboh == null)
                    return false;
                AI.SelectMaterials(new[] { tuner, linkuriboh });
                return true;
            }

            if (BorrelswordComboStarted ||
                (TopologicComboStarted && HasTopologicComboPayoff()))
            {
                ClientCard tuner = Bot.GetMonsters().FirstOrDefault(card => card.IsTuner() && card.IsFaceup());
                ClientCard material = Bot.GetMonsters()
                    .Where(card => card != tuner && card.IsFaceup())
                    .OrderBy(GetLinkMaterialValue)
                    .FirstOrDefault();
                if (tuner == null || material == null)
                    return false;
                AI.SelectMaterials(new[] { tuner, material });
                return true;
            }

            List<ClientCard> comboMaterials = Bot.GetMonsters()
                .Where(card => card.IsFaceup())
                .OrderBy(card => card.Attack)
                .ToList();
            AI.SelectMaterials(comboMaterials);
            return true;
        }

        private bool CrystronHalqifibraxEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            AI.SelectCard(CardId.GlowUpBulb, CardId.EffectVeiler, CardId.GhostOgre, CardId.AshBlossom);
            return true;
        }

        private bool LinkuribohEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Card.Location != CardLocation.Grave)
                return Duel.Player == 1 && Bot.BattlingMonster != null;
            if (IsTopologicComboLinkSummonReady())
                return false;
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.Main1 ||
                !Bot.HasInMonstersZone(CardId.CrystronHalqifibrax))
                return false;

            ClientCard tribute = Bot.GetMonsters().FirstOrDefault(card =>
                card.Level == 1 && card.IsTuner() && card.IsFaceup() && !card.IsCode(CardId.Linkuriboh));
            if (tribute == null)
                return false;
            AI.SelectCard(tribute);
            return true;
        }

        private bool GlowUpBulbEffect()
        {
            bool summonForBorrelsword = Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                !BorrelswordPositionEffectUsed &&
                Bot.MonsterZone.Concat(Enemy.MonsterZone).Count(
                    card => card != null && card.IsAttack() && !card.HasType(CardType.Link)) == 0 &&
                Bot.HasInMonstersZone(CardId.BorrelswordDragon, true);
            bool summon = summonForBorrelsword ||
                Bot.HasInMonstersZone(CardId.KnightmareUnicorn) ||
                (Bot.HasInMonstersZone(CardId.CrystronHalqifibrax) &&
                    Bot.HasInMonstersZone(CardId.Linkuriboh));
            if (summon)
            {
                GlowUpBulbEffectUsed = true;
                AI.SelectPosition(summonForBorrelsword ?
                    CardPosition.FaceUpAttack : CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool BorrelswordDragonSummon()
        {
            if (Util.IsTurn1OrMain2())
                return false;

            bool committedCombo = BorrelswordComboStarted;
            bool canFinishDirectly = Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 3000;
            bool worthwhileBattle = Enemy.GetMonsters().Count(IsBorrelswordPositionTarget) >= 3 ||
                Bot.GetMonsters().Any(IsBorrelswordPositionTarget) ||
                CanPrepareBorrelswordTargetWithSharkCannon();
            if (!committedCombo && !canFinishDirectly && !worthwhileBattle)
                return false;

            ClientCard halqifibrax = Bot.GetMonsters().FirstOrDefault(card =>
                card.IsCode(CardId.CrystronHalqifibrax));
            ClientCard linkuriboh = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.Linkuriboh));
            ClientCard bulb = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.GlowUpBulb));
            if (halqifibrax != null && linkuriboh != null && bulb != null)
            {
                List<ClientCard> comboMaterials = new List<ClientCard> { halqifibrax, linkuriboh, bulb };
                if (Util.GetBotAvailZonesFromExtraDeck(comboMaterials) > 0)
                {
                    AI.SelectMaterials(comboMaterials);
                    return true;
                }
            }

            ClientCard positionTarget = Bot.GetMonsters()
                .Where(IsBorrelswordPositionTarget)
                .OrderBy(card => ControlledEnemyCards.Contains(card) ? 0 : 1)
                .ThenBy(card => card.Attack)
                .FirstOrDefault();
            List<ClientCard> materials = positionTarget == null ? null :
                GetLinkMaterials(4, 3, 4, true, false, positionTarget);
            if (materials == null)
                materials = GetLinkMaterials(4, 3, 4, true, false);
            if (materials == null)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool BorrelswordDragonEffect()
        {
            if (ActivateDescription == -1 ||
                ActivateDescription == Util.GetStringId(CardId.BorrelswordDragon, 1))
            {
                BorrelswordAttackEffectUsed = true;
                return true;
            }

            if (ActivateDescription != Util.GetStringId(CardId.BorrelswordDragon, 0))
                return false;

            ClientCard target = null;
            if (Duel.Player == 0 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                target = Bot.GetMonsters().FirstOrDefault(card =>
                    card.IsCode(CardId.Raye) && card.IsAttack() && card.Attacked);
                if (target == null)
                    target = Bot.GetMonsters().FirstOrDefault(card =>
                        card.IsAttack() && card.Attacked && !card.HasType(CardType.Link));
                if (target == null && Card.Attacked)
                    target = Enemy.GetMonsters()
                        .Where(IsBorrelswordPositionTarget)
                        .OrderBy(card => card.Attack)
                        .FirstOrDefault();
            }
            else if (Duel.Player == 1)
            {
                target = Enemy.GetMonsters()
                    .Where(IsBorrelswordPositionTarget)
                    .OrderByDescending(card => card.Attack)
                    .FirstOrDefault();
            }

            if (target == null)
                return false;
            BorrelswordPositionEffectUsed = true;
            AI.SelectCard(target);
            return true;
        }

        private bool KnightmarePhoenixSummon()
        {
            if (IsTopologicComboLinkSummonReady())
                return false;
            if (Util.GetProblematicEnemySpell() == null)
                return false;
            return SelectKnightmareLinkMaterials(false);
        }

        private bool KnightmarePhoenixEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            ClientCard target = Util.GetProblematicEnemySpell();
            if (target == null || IsCurrentChainHandled(target) || Bot.Hand.Count == 0)
                return false;
            MarkCurrentChainHandled(target);
            SelectDiscard();
            AI.SelectNextCard(target);
            return true;
        }

        private bool KnightmareCerberusSummon()
        {
            if (IsTopologicComboLinkSummonReady())
                return false;
            if (GetProblematicKnightmareCerberusTarget() == null)
                return false;
            return SelectKnightmareLinkMaterials(false);
        }

        private bool KnightmareCerberusEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            ClientCard target = GetProblematicKnightmareCerberusTarget();
            if (target == null || Bot.Hand.Count == 0)
                return false;
            MarkCurrentChainHandled(target);
            SelectDiscard();
            AI.SelectNextCard(target);
            return true;
        }

        private bool KnightmarePhoenixUnlockSummon()
        {
            if (!ShouldUnlockMainMonsterZones())
                return false;
            if (Util.GetProblematicEnemySpell() == null &&
                GetProblematicKnightmareCerberusTarget() != null)
                return false;
            return SelectKnightmareLinkMaterials(true);
        }

        private bool KnightmareCerberusUnlockSummon()
        {
            return ShouldUnlockMainMonsterZones() && SelectKnightmareLinkMaterials(true);
        }

        private bool ShouldUnlockMainMonsterZones()
        {
            if (Duel.Player != 0 || Duel.CurrentChain.Count > 0 ||
                Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2 ||
                Bot.GetMonsterCount() > 2 || EmptyMainMonsterZone())
                return false;

            return HasUsefulBlockedSkyStrikerSpell() ||
                (Bot.GetMonsters().Any(IsDisposableMonster) && Enemy.GetMonsterCount() > 0);
        }

        private bool SelectKnightmareLinkMaterials(bool requireExtraMonsterZone)
        {
            List<ClientCard> materials;
            if (requireExtraMonsterZone)
            {
                materials = Bot.GetMonsters()
                    .Where(card => card.IsFaceup() && !card.IsCode(CardId.BorrelswordDragon))
                    .OrderBy(GetLinkMaterialValue)
                    .ToList();
                if (materials.Count != 2 || materials.Select(card => card.Id).Distinct().Count() != 2 ||
                    !CanMakeLinkRating(materials, 2))
                    return false;
            }
            else
            {
                materials = GetLinkMaterials(2, 2, 2, false, true);
            }
            if (materials == null)
                return false;
            int available = Util.GetBotAvailZonesFromExtraDeck(materials);
            if (requireExtraMonsterZone && !Duel.IsNewRule2020 &&
                (available & Zones.ExtraMonsterZones) == 0)
                return false;
            AI.SelectMaterials(materials);
            if (requireExtraMonsterZone)
                AI.SelectPlace(Zones.ExtraMonsterZones);
            return true;
        }

        private ClientCard GetProblematicKnightmareCerberusTarget()
        {
            List<ClientCard> targets = Enemy.GetMonstersInMainZone()
                .Where(card => card.IsSpecialSummoned && !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget() && !IsCurrentChainHandled(card))
                .ToList();
            ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
            if (problematic != null && targets.Contains(problematic))
                return problematic;
            return null;
        }

        private bool IsTopologicComboLinkSummonReady()
        {
            if (!TopologicComboStarted || !HasTopologicComboPayoff())
                return false;
            ClientCard halqifibrax = Bot.GetMonsters().FirstOrDefault(card =>
                card.IsCode(CardId.CrystronHalqifibrax));
            bool canSummonUnicorn = halqifibrax != null &&
                Bot.GetMonsters().Any(card => card != halqifibrax && card.IsTuner() && card.IsFaceup()) &&
                Bot.HasInExtra(CardId.KnightmareUnicorn);
            bool canSummonTopologic = Bot.HasInMonstersZone(CardId.KnightmareUnicorn) &&
                Bot.HasInMonstersZone(CardId.GlowUpBulb) &&
                Bot.HasInExtra(CardId.TopologicBomberDragon);
            return canSummonUnicorn || canSummonTopologic;
        }

        private bool KnightmareUnicornSummon()
        {
            ClientCard halqifibrax = Bot.GetMonsters().FirstOrDefault(card =>
                card.IsCode(CardId.CrystronHalqifibrax));
            ClientCard tuner = Bot.GetMonsters().FirstOrDefault(card =>
                card != halqifibrax && card.IsTuner() && card.IsFaceup());
            if (halqifibrax != null && tuner != null)
            {
                List<ClientCard> comboMaterials = new List<ClientCard> { halqifibrax, tuner };
                if (Util.GetBotAvailZonesFromExtraDeck(comboMaterials) != 0)
                {
                    AI.SelectMaterials(comboMaterials);
                    return true;
                }
            }

            if (Util.GetProblematicEnemyCard(0, true) == null)
                return false;
            List<ClientCard> materials = GetLinkMaterials(3, 2, 3, false, true);
            if (materials == null)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool KnightmareUnicornEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (TopologicComboStarted && HasTopologicComboPayoff() && Bot.Hand.Count < 2)
                return false;

            ClientCard target = Util.GetProblematicEnemyCard(0, true) ?? Util.GetBestEnemyCard(false, true);
            if (target == null || IsCurrentChainHandled(target) ||
                BorrelswordComboStarted && Enemy.GetMonsterCount() == 1)
                return false;
            MarkCurrentChainHandled(target);
            SelectDiscard();
            AI.SelectNextCard(target);
            return true;
        }

        private bool TopologicBomberDragonSummon()
        {
            if (TopologicComboStarted && HasTopologicComboPayoff())
            {
                ClientCard unicorn = Bot.GetMonsters().FirstOrDefault(card =>
                    card.IsCode(CardId.KnightmareUnicorn));
                ClientCard bulb = Bot.GetMonsters().FirstOrDefault(card =>
                    card.IsCode(CardId.GlowUpBulb));
                if (unicorn != null && bulb != null)
                {
                    List<ClientCard> comboMaterials = new List<ClientCard> { unicorn, bulb };
                    if (Util.GetBotAvailZonesFromExtraDeck(comboMaterials) != 0)
                    {
                        AI.SelectMaterials(comboMaterials);
                        return true;
                    }
                }
            }

            if (Bot.HasInMonstersZone(CardId.Shizuku) ||
                Bot.HasInExtra(CardId.Shizuku) && Bot.GetMonsters().Any(card =>
                    card.IsCode(CardId.Raye, CardId.Kagari, CardId.Hayate)))
                return false;
            List<ClientCard> materials = GetLinkMaterials(4, 2, 4, true, false);
            if (materials == null)
                return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private bool ShizukuSummon()
        {
            if (ShizukuSummoned || !Util.IsTurn1OrMain2())
                return false;
            ShizukuSummoned = true;
            return true;
        }

        private bool ShizukuFirstEndPhaseEffect()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.End ||
                DefaultCheckWhetherCardIsNegated(Card) ||
                !Bot.HasInSpellZone(CardId.Multirole, true, true) ||
                Bot.HasInGraveyard(CardId.WidowAnchor) || !Bot.HasInDeck(CardId.WidowAnchor) ||
                Bot.GetSpellCountWithoutField() >= 5)
                return false;
            AI.SelectCard(CardId.WidowAnchor);
            return true;
        }

        private bool WidowAnchorEndPhaseEffect()
        {
            if (Duel.Player != 0 || Duel.Phase != DuelPhase.End ||
                Card.Location != CardLocation.Hand ||
                !Bot.HasInSpellZone(CardId.Multirole, true, true) ||
                Bot.HasInGraveyard(CardId.WidowAnchor) ||
                Bot.GetSpellCountWithoutField() >= 5)
                return false;
            AI.SelectCard(CardId.Shizuku);
            return true;
        }

        private bool ShizukuEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            AI.SelectCard(GetSkyStrikerSearchPriority(true));
            return true;
        }

        private int[] GetSkyStrikerSearchPriority(bool includeEngage)
        {
            List<int> result = new List<int>();
            if (includeEngage)
                result.Add(CardId.Engage);
            if (!Bot.HasInHandOrInSpellZone(CardId.HornetDrones))
                result.Add(CardId.HornetDrones);
            if (Util.GetProblematicEnemyMonster() != null)
                result.Add(CardId.WidowAnchor);
            if (EmptyMainMonsterZone() && Util.GetProblematicEnemyMonster() != null)
                result.Add(CardId.Afterburners);
            if (EmptyMainMonsterZone() && Enemy.GetSpells().Any(card =>
                card.IsFacedown() && !IsCurrentChainHandled(card)))
                result.Add(CardId.JammingWaves);
            if (!Bot.HasInHand(CardId.Raye) && !Bot.HasInMonstersZone(CardId.Raye) &&
                !Bot.HasInGraveyard(CardId.Raye))
                result.Add(CardId.Raye);
            if (!Bot.HasInHandOrInSpellZone(CardId.Multirole))
                result.Add(CardId.Multirole);
            if (!Bot.HasInHandOrInSpellZone(CardId.WidowAnchor))
                result.Add(CardId.WidowAnchor);
            result.AddRange(new[]
            {
                CardId.HornetDrones,
                CardId.WidowAnchor,
                CardId.SharkCannon,
                CardId.Afterburners,
                CardId.JammingWaves,
                CardId.AreaZero,
                CardId.Multirole,
                CardId.Raye
            });
            return result.Distinct().Where(Bot.HasInDeck).ToArray();
        }

        private ClientCard GetEnemyMonsterTarget(bool problematicOnly)
        {
            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(card => card.IsFaceup() && !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeSpellTrapTarget() && !IsCurrentChainHandled(card) &&
                    (card != WidowAnchorTarget || !card.IsDisabled()))
                .ToList();

            ClientCard target = candidates.GetFloodgate() ??
                candidates.GetDangerousMonster() ??
                candidates.GetInvincibleMonster() ??
                candidates.GetShouldBeDisabledBeforeItUseEffectMonster();
            if (target != null)
                return target;

            if (problematicOnly)
            {
                int bestBotAttack = Util.GetBestAttack(Bot);
                return candidates
                    .Where(card => card.IsAttack() && card.GetDefensePower() >= bestBotAttack)
                    .OrderByDescending(card => card.GetDefensePower())
                    .FirstOrDefault();
            }

            return candidates.GetHighestAttackMonster();
        }

        private bool IsCurrentChainHandled(ClientCard card)
        {
            return card != null && CurrentChainHandledCards.Contains(card);
        }

        private void MarkCurrentChainHandled(ClientCard card)
        {
            if (card != null && !CurrentChainHandledCards.Contains(card))
                CurrentChainHandledCards.Add(card);
        }

        private bool HasUsefulBlockedSkyStrikerSpell()
        {
            if (EmptyMainMonsterZone())
                return false;
            ClientCard problematicMonster = GetEnemyMonsterTarget(true);
            return Bot.HasInHandOrInSpellZone(CardId.Engage) ||
                (Bot.HasInHandOrInSpellZone(CardId.Afterburners) && problematicMonster != null) ||
                (Bot.HasInHandOrInSpellZone(CardId.JammingWaves) &&
                    Enemy.GetSpells().Any(card => card.IsFacedown() && !IsCurrentChainHandled(card))) ||
                (Bot.HasInHandOrInSpellZone(CardId.WidowAnchor) && problematicMonster != null &&
                    !problematicMonster.IsDisabled() && !problematicMonster.HasType(CardType.Normal));
        }

        private bool IsDisposableMonster(ClientCard card)
        {
            if (card == null)
                return false;
            if (ControlledEnemyCards.Contains(card) ||
                card.IsCode(CardId.Token, CardId.GlowUpBulb, CardId.Linkuriboh, CardId.AshBlossom,
                    CardId.GhostOgre, CardId.EffectVeiler, CardId.MaxxC))
                return true;
            return !card.IsCode(CardId.Raye, CardId.Kagari, CardId.Hayate, CardId.Shizuku,
                CardId.CrystronHalqifibrax, CardId.BorrelswordDragon,
                CardId.TopologicBomberDragon, CardId.KnightmareUnicorn);
        }

        private void SelectDiscard()
        {
            List<int> priorities = new List<int>
            {
                CardId.MetalfoesFusion,
                CardId.GlowUpBulb
            };
            if (!Bot.HasInGraveyard(CardId.Raye))
                priorities.Add(CardId.Raye);
            priorities.AddRange(new[]
            {
                CardId.ReinforcementOfTheArmy,
                CardId.FoolishBurialGoods,
                CardId.UpstartGoblin,
                CardId.HerculesBase
            });
            if (HasTopologicComboPayoff() &&
                (TopologicComboStarted || ShouldStartTopologicCombo()) &&
                Bot.HasInHand(CardId.HornetDrones))
            {
                ClientCard discard = Bot.Hand
                    .Where(card => card != Card && !card.IsCode(CardId.HornetDrones))
                    .OrderBy(card =>
                    {
                        int index = priorities.IndexOf(card.Id);
                        return index < 0 ? int.MaxValue : index;
                    })
                    .FirstOrDefault();
                if (discard != null)
                {
                    AI.SelectCard(discard);
                    return;
                }
            }
            priorities.Add(CardId.HornetDrones);
            AI.SelectCard(priorities);
        }

        private List<ClientCard> OrderCardsById(IList<ClientCard> cards, IList<int> ids, bool uniqueNames)
        {
            List<ClientCard> result = new List<ClientCard>();
            HashSet<int> addedIds = new HashSet<int>();
            foreach (int id in ids)
            {
                ClientCard card = cards.FirstOrDefault(candidate =>
                    candidate.IsCode(id) && (!uniqueNames || !addedIds.Contains(candidate.Id)));
                if (card != null)
                {
                    result.Add(card);
                    addedIds.Add(card.Id);
                }
            }
            foreach (ClientCard card in cards)
            {
                if (!result.Contains(card) && (!uniqueNames || !addedIds.Contains(card.Id)))
                {
                    result.Add(card);
                    addedIds.Add(card.Id);
                }
            }
            return result;
        }

        private List<ClientCard> GetLinkMaterials(int linkRating, int minCount, int maxCount,
            bool effectOnly, bool uniqueNames, ClientCard preservedCard = null)
        {
            List<ClientCard> candidates = Bot.GetMonsters()
                .Where(card => card.IsFaceup() && (!effectOnly || card.HasType(CardType.Effect)) &&
                    card != preservedCard && !card.IsCode(CardId.BorrelswordDragon))
                .OrderBy(GetLinkMaterialValue)
                .ToList();

            List<ClientCard> best = null;
            int bestScore = int.MaxValue;
            int subsetCount = 1 << candidates.Count;
            for (int mask = 1; mask < subsetCount; mask++)
            {
                List<ClientCard> selected = new List<ClientCard>();
                for (int i = 0; i < candidates.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        selected.Add(candidates[i]);
                }
                if (selected.Count < minCount || selected.Count > maxCount)
                    continue;
                if (uniqueNames && selected.Select(card => card.Id).Distinct().Count() != selected.Count)
                    continue;
                if (!CanMakeLinkRating(selected, linkRating) ||
                    Util.GetBotAvailZonesFromExtraDeck(selected) == 0)
                    continue;

                int score = selected.Sum(GetLinkMaterialValue);
                if (score < bestScore)
                {
                    best = selected;
                    bestScore = score;
                }
            }
            return best;
        }

        private bool CanMakeLinkRating(IList<ClientCard> materials, int rating)
        {
            int modeCount = 1 << materials.Count;
            for (int mask = 0; mask < modeCount; mask++)
            {
                int total = 0;
                for (int i = 0; i < materials.Count; i++)
                {
                    ClientCard card = materials[i];
                    if ((mask & (1 << i)) != 0 && card.HasType(CardType.Link))
                        total += card.LinkCount;
                    else
                        total++;
                }
                if (total == rating)
                    return true;
            }
            return false;
        }

        private int GetLinkMaterialValue(ClientCard card)
        {
            if (ControlledEnemyCards.Contains(card) || card.Controller == 1 ||
                card.IsCode(CardId.Token, CardId.GlowUpBulb))
                return 0;
            if (card.IsCode(CardId.Linkuriboh))
                return 100;
            if (card.IsCode(CardId.AshBlossom, CardId.GhostOgre, CardId.EffectVeiler))
                return 200;
            if (card.IsCode(CardId.Kagari, CardId.Hayate, CardId.Shizuku))
                return 1500 + card.LinkCount * 100;
            return Math.Max(300, card.Attack) + card.LinkCount * 100;
        }

        private bool SetHornetDrones()
        {
            return Util.IsTurn1OrMain2() && (SetWhenHandIsFull() || Bot.HasInMonstersZone(CardId.TopologicBomberDragon, true));
        }

        private bool SetWhenHandIsFull()
        {
            return Bot.GetSpellCountWithoutField() < 4 && Bot.Hand.Count > 6;
        }

        private bool EmptyMainMonsterZone()
        {
            return Bot.GetMonstersInMainZone().Count == 0;
        }

        private int GetSpellCountInGrave()
        {
            return Bot.GetGraveyardSpells().Count;
        }

        private bool HaveThreeSpellsInGrave()
        {
            return GetSpellCountInGrave() >= 3;
        }
    }
}
