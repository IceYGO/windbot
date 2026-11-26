using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Ashtra", "AI_Ashtra")]
    class AshtraExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int TheGreatGallantBandit = 24203749;
            public const int AkasunaTheBattleAshtra = 38331244;
            public const int KuzuhaTheHexAshtra = 65325949;
            public const int YoaThePlagueAshtra = 1710647;
            public const int MikumariTheBanisherAshtra = 38114652;
            public const int LilithLadyOfLament = 23898021;
            public const int MaxxC = 23434538;
            public const int DrollLockBird = 94145021;
            public const int TheFallenTheVirtuous = 30271097;
            public const int FiendsmithsRequiem = 2463794;
            public const int DarkTimeWizard = 40235813;
            public const int AshtrashenGateToTheWorldsBeyond = 2906939;
            public const int InfiniteImpermanence = 10045474;
            public const int AshtrasDomain = 37839434;
            public const int DominusImpulse = 40366667;
            public const int DominusSpiral = 42091632;
            public const int SongsOfTheDominators = 58053438;
            public const int AshtrasWeb = 64455720;
            public const int FoolishGraverobber = 65118318;
            public const int MindMirrorForce = 66247039;
            public const int AshtrasHowl = 91444835;
            public const int SolemnAccusation = 78114463;
            public const int RedEyesBlackDragonExceed = 17242022;
            public const int AlbionTheBrandedDragon = 87746184;
            public const int PsychicEndPunisher = 60465049;
            public const int SwordsoulSupremeSovereignChengying = 96633955;
            public const int BaronneDeFleur = 84815190;
            public const int AshtrajaSusanoo = 17020474;
            public const int EkhajarDescendantDragonOfTheIceBarrier = 65424481;
            public const int AshtrayaMikumariNoMikoto = 43419178;
            public const int AdamancipatorRisenDragite = 9464441;
            public const int EcclesiaAndTheDarkDragon = 78397661;
            public const int SuperStarslayerTYPHONSkyCrisis = 93039339;
        }

        public AshtraExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Responses precede engine actions; summon-success triggers must not wait
            // for a stale MainPhase candidate list.
            AddExecutor(ExecutorType.Summon, CardId.TheGreatGallantBandit, GallantThiefSummon);
            AddExecutor(ExecutorType.Activate, CardId.TheGreatGallantBandit, GallantThiefActivate);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur, BaronneActivate);
            AddExecutor(ExecutorType.Activate, CardId.AdamancipatorRisenDragite, DragiteActivate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnAccusation, SolemnActivate);
            AddExecutor(ExecutorType.Activate, CardId.MindMirrorForce, MindMirrorForceActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusImpulse, OpponentResponse);
            AddExecutor(ExecutorType.Activate, CardId.SongsOfTheDominators, OpponentResponse);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, ImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusSpiral, SpiralActivate);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrollLockBird, DrollActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshtrasDomain, AshtraTrapInterruption);
            AddExecutor(ExecutorType.Activate, CardId.AshtrasHowl, AshtraTrapInterruption);
            // Idle commands can set cards and summon from hand before using
            // ignition effects. Trigger windows cannot offer these actions.
            AddExecutor(ExecutorType.SpellSet, AshtraSetupSpellSet);
            // Use one starter before exposing the rest of our hand to the coin
            // toss. These entries never postpone a summon-success trigger.
            AddExecutor(ExecutorType.Activate, CardId.DarkTimeWizard, TimeWizardComboActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.RedEyesBlackDragonExceed, RedEyesComboSummon);
            AddExecutor(ExecutorType.Activate, CardId.MikumariTheBanisherAshtra, TimeWizardStarterActivate);
            AddExecutor(ExecutorType.Activate, CardId.YoaThePlagueAshtra, TimeWizardStarterActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.MikumariTheBanisherAshtra, AshtraHandSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.YoaThePlagueAshtra, AshtraHandSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AkasunaTheBattleAshtra, AshtraHandSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KuzuhaTheHexAshtra, AshtraHandSummon);
            AddExecutor(ExecutorType.Activate, CardId.AshtrayaMikumariNoMikoto, MikumariNoMikotoActivate);
            AddExecutor(ExecutorType.Activate, CardId.MikumariTheBanisherAshtra, MikumariActivate);
            AddExecutor(ExecutorType.Activate, CardId.YoaThePlagueAshtra, YoaActivate);
            AddExecutor(ExecutorType.Activate, CardId.AkasunaTheBattleAshtra, AkasunaActivate);
            AddExecutor(ExecutorType.Activate, CardId.KuzuhaTheHexAshtra, KuzuhaActivate);
            AddExecutor(ExecutorType.Summon, CardId.LilithLadyOfLament, LilithSummon);
            AddExecutor(ExecutorType.Activate, CardId.LilithLadyOfLament, LilithActivate);
            AddExecutor(ExecutorType.Activate, CardId.EkhajarDescendantDragonOfTheIceBarrier, EkhajarActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSupremeSovereignChengying, ChengyingActivate);
            AddExecutor(ExecutorType.Activate, CardId.PsychicEndPunisher, PunisherActivate);
            AddExecutor(ExecutorType.Activate, CardId.RedEyesBlackDragonExceed, RedEyesActivate);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon, AlbionActivate);
            AddExecutor(ExecutorType.Activate, CardId.EcclesiaAndTheDarkDragon, EcclesiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFallenTheVirtuous, FallenActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshtrashenGateToTheWorldsBeyond, GateActivate);
            AddExecutor(ExecutorType.Activate, CardId.FoolishGraverobber, GraverobberActivate);
            AddExecutor(ExecutorType.Activate, CardId.DarkTimeWizard, TimeWizardActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshtrasDomain, AshtraTrapActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshtrasWeb, AshtraTrapActivate);
            AddExecutor(ExecutorType.Activate, CardId.AshtrasHowl, AshtraTrapActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.RedEyesBlackDragonExceed, RedEyesSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AshtrayaMikumariNoMikoto, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronneDeFleur, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SwordsoulSupremeSovereignChengying, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PsychicEndPunisher, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AdamancipatorRisenDragite, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.EkhajarDescendantDragonOfTheIceBarrier, SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AshtrajaSusanoo, SynchroSummon);
            AddExecutor(ExecutorType.Activate, CardId.AshtrajaSusanoo, SusanooActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperStarslayerTYPHONSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.SuperStarslayerTYPHONSkyCrisis, TyphonActivate);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, AshtraSpellSet);
        }

        private readonly HashSet<int> _usedEffects = new HashSet<int>();
        // Normal Traps that Lilith can reveal in this deck; Counter Traps and
        // spells (including Solemn Accusation and Graverobber) are not eligible.
        private readonly int[] _lilithTrapIds = new int[] {
            CardId.InfiniteImpermanence, CardId.AshtrasDomain, CardId.DominusImpulse,
            CardId.DominusSpiral, CardId.SongsOfTheDominators, CardId.AshtrasWeb,
            CardId.MindMirrorForce, CardId.AshtrasHowl };
        private readonly HashSet<int> _resolvedChainIndices = new HashSet<int>();
        private readonly Dictionary<int, List<ClientCard>> _reservedRemoval =
            new Dictionary<int, List<ClientCard>>();
        private readonly HashSet<ClientCard> _spentBaronne = new HashSet<ClientCard>();
        private readonly Dictionary<int, int> _plannedTrapOptions = new Dictionary<int, int>();
        private readonly HashSet<int> _gateTargetSelected = new HashSet<int>();
        private readonly Dictionary<int, List<ClientCard>> _susanooRecoveryTargets =
            new Dictionary<int, List<ClientCard>>();
        private readonly Dictionary<int, List<ClientCard>> _susanooRecoverySets =
            new Dictionary<int, List<ClientCard>>();
        private readonly HashSet<ClientCard> _ekhajarSummonPending = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _ekhajarBouncePending = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _ekhajarSummonedInChain = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _ekhajarBounceInChain = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _synchroSummonedEkhajar = new HashSet<ClientCard>();
        private int _pendingHowlChainIndex;
        private readonly HashSet<ClientCard> _mikumariSummonPending = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _mikumariSetPending = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _mikumariSummonedInChain = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _mikumariSetInChain = new HashSet<ClientCard>();
        private readonly Dictionary<ClientCard, int> _acceptedMikumariEffects =
            new Dictionary<ClientCard, int>();
        private bool _timeSearchUsed;
        private bool _timeCoinUsed;
        private readonly Dictionary<int, int> _plannedTimeWizardOptions = new Dictionary<int, int>();
        private int _pendingTimeWizardChainIndex;
        private bool _timeWizardDestroyedMonster;
        private bool _redEyesSummonedThisTurn;
        private int _pendingSummonId;
        private int _susanooCostCount;
        private int _susanooCostBudget;
        private int _susanooRemovedCount;
        private readonly HashSet<ClientCard> _susanooSummonPending = new HashSet<ClientCard>();
        private readonly HashSet<ClientCard> _susanooSummonedInChain = new HashSet<ClientCard>();
        private bool _chengyingReplacementPending;
        private bool _albionSentToGraveThisTurn;

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            _usedEffects.Clear();
            _resolvedChainIndices.Clear();
            _timeSearchUsed = false;
            _timeCoinUsed = false;
            _plannedTimeWizardOptions.Clear();
            _pendingTimeWizardChainIndex = 0;
            _timeWizardDestroyedMonster = false;
            _redEyesSummonedThisTurn = false;
            _pendingSummonId = 0;
            _plannedTrapOptions.Clear();
            _gateTargetSelected.Clear();
            _susanooRecoveryTargets.Clear();
            _susanooRecoverySets.Clear();
            _susanooSummonPending.Clear();
            _susanooSummonedInChain.Clear();
            _chengyingReplacementPending = false;
            _albionSentToGraveThisTurn = false;
            _ekhajarSummonPending.Clear();
            _ekhajarBouncePending.Clear();
            _ekhajarSummonedInChain.Clear();
            _ekhajarBounceInChain.Clear();
            _pendingHowlChainIndex = 0;
            _mikumariSummonPending.Clear();
            _mikumariSetPending.Clear();
            _mikumariSummonedInChain.Clear();
            _mikumariSetInChain.Clear();
            _acceptedMikumariEffects.Clear();
            base.OnNewTurn();
        }

        public override void OnNewPhase()
        {
            _susanooSummonPending.Clear();
            _susanooSummonedInChain.Clear();
            _chengyingReplacementPending = false;
            _ekhajarSummonPending.Clear();
            _ekhajarBouncePending.Clear();
            _ekhajarSummonedInChain.Clear();
            _ekhajarBounceInChain.Clear();
            _mikumariSummonPending.Clear();
            _mikumariSetPending.Clear();
            _mikumariSummonedInChain.Clear();
            _mikumariSetInChain.Clear();
            _acceptedMikumariEffects.Clear();
            base.OnNewPhase();
        }

        public override void OnSpSummoned()
        {
            // Graveyard revivals do not create Susanoo's Synchro-summon trigger.
            foreach (ClientCard summoned in Duel.LastSummonedCards.Where(c => c.Controller == 0 &&
                c.IsCode(CardId.AshtrajaSusanoo) && c.LastLocation == CardLocation.Extra))
            {
                _susanooSummonPending.Add(summoned);
                if (Duel.CurrentChain.Count > 0)
                    _susanooSummonedInChain.Add(summoned);
            }
            foreach (ClientCard summoned in Duel.LastSummonedCards.Where(c => c.Controller == 0 &&
                c.IsCode(CardId.EkhajarDescendantDragonOfTheIceBarrier)))
            {
                if (summoned.LastLocation == CardLocation.Extra)
                    _synchroSummonedEkhajar.Add(summoned);
                else
                    _synchroSummonedEkhajar.Remove(summoned);
                _ekhajarSummonPending.Add(summoned);
                if (Duel.CurrentChain.Count > 0)
                    _ekhajarSummonedInChain.Add(summoned);
            }
            if (Duel.LastSummonedCards.Any(c => c.Controller == 1))
            {
                foreach (ClientCard monster in Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    c.IsCode(CardId.EkhajarDescendantDragonOfTheIceBarrier)))
                {
                    _ekhajarBouncePending.Add(monster);
                    if (Duel.CurrentChain.Count > 0)
                        _ekhajarBounceInChain.Add(monster);
                }
            }
            if (Duel.LastSummonedCards.Any(c => c.Controller == 0 &&
                c.IsCode(CardId.RedEyesBlackDragonExceed)))
                _redEyesSummonedThisTurn = true;
            foreach (ClientCard summoned in Duel.LastSummonedCards.Where(c => c.Controller == 0 &&
                c.IsCode(CardId.AshtrayaMikumariNoMikoto)))
            {
                _mikumariSummonPending.Add(summoned);
                if (Duel.CurrentChain.Count > 0)
                    _mikumariSummonedInChain.Add(summoned);
            }
            base.OnSpSummoned();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 0 && card != null && card.IsCode(CardId.AshtrajaSusanoo))
            {
                _susanooSummonPending.Remove(card);
                _susanooSummonedInChain.Remove(card);
            }
            // The incoming link has not been appended yet. Cover all visible
            // monsters this non-targeting effect would set, not only the one
            // that made it worthwhile. Do not reserve an already-active monster
            // effect: setting it does not replace a necessary negate.
            if (player == 0 && card != null && card.IsCode(CardId.AkasunaTheBattleAshtra) &&
                (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                _reservedRemoval[Duel.CurrentChain.Count + 1] =
                    Enemy.GetMonsters().Where(c => CanAkasunaSet(c) &&
                        !Duel.CurrentChainInfo.Where((info, index) =>
                            info.ActivatePlayer == 1 && info.RelatedCard == c &&
                            info.ActivateLocation == CardLocation.MonsterZone &&
                            !Duel.NegatedChainIndexList.Contains(index + 1)).Any()).ToList();
            if (player == 0 && card != null && card.IsCode(CardId.AshtrasHowl))
                _pendingHowlChainIndex = 0;
            if (player == 0 && card != null && card.IsCode(CardId.DarkTimeWizard))
            {
                int option;
                if (_plannedTimeWizardOptions.TryGetValue(Duel.CurrentChain.Count + 1, out option))
                {
                    // A single legal branch may not produce SelectOption. Count
                    // usage on activation even if the effect is later negated.
                    if (option == 1)
                        _timeSearchUsed = true;
                    else if (option == 2)
                        _timeCoinUsed = true;
                }
                _pendingTimeWizardChainIndex = 0;
            }
            // The incoming ChainInfo is not in the list yet. Use the callback
            // arguments to record the event, not the previous chain link.
            if (player == 1 && card != null && card.IsMonster())
            {
                foreach (ClientCard monster in Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    c.IsCode(CardId.AshtrayaMikumariNoMikoto)))
                {
                    _mikumariSetPending.Add(monster);
                    _mikumariSetInChain.Add(monster);
                }
            }
            int effect;
            if (player == 0 && card != null && _acceptedMikumariEffects.TryGetValue(card, out effect))
            {
                if (effect == 0)
                {
                    _mikumariSummonPending.Remove(card);
                    _mikumariSummonedInChain.Remove(card);
                }
                else
                {
                    _mikumariSetPending.Remove(card);
                    _mikumariSetInChain.Remove(card);
                }
                _acceptedMikumariEffects.Remove(card);
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            _chengyingReplacementPending = false;
            _resolvedChainIndices.Add(chainIndex);
            if (chainIndex > 0 && chainIndex <= Duel.CurrentChainInfo.Count)
            {
                ChainInfo info = Duel.CurrentChainInfo[chainIndex - 1];
                if (info.ActivatePlayer == 0 && info.ActivateDescription >= 0)
                    _usedEffects.Add(info.ActivateDescription);
            }
            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            _chengyingReplacementPending = false;
            _resolvedChainIndices.Clear();
            // A Synchro summon within the previous chain triggers only after
            // that chain ends. Keep new events, but expire older declined ones.
            _susanooSummonPending.Clear();
            _susanooSummonPending.UnionWith(_susanooSummonedInChain);
            _susanooSummonedInChain.Clear();
            _reservedRemoval.Clear();
            _gateTargetSelected.Clear();
            _susanooRecoveryTargets.Clear();
            _susanooRecoverySets.Clear();
            _ekhajarSummonPending.Clear();
            _ekhajarSummonPending.UnionWith(_ekhajarSummonedInChain);
            _ekhajarBouncePending.Clear();
            _ekhajarBouncePending.UnionWith(_ekhajarBounceInChain);
            _ekhajarSummonedInChain.Clear();
            _ekhajarBounceInChain.Clear();
            _susanooCostCount = 0;
            _susanooCostBudget = 0;
            _susanooRemovedCount = 0;
            _plannedTrapOptions.Clear();
            _pendingHowlChainIndex = 0;
            _plannedTimeWizardOptions.Clear();
            _pendingTimeWizardChainIndex = 0;
            // Retain events produced in the chain that just ended: their delayed
            // trigger window comes next. Older, declined events must expire.
            _mikumariSummonPending.Clear();
            _mikumariSummonPending.UnionWith(_mikumariSummonedInChain);
            _mikumariSetPending.Clear();
            _mikumariSetPending.UnionWith(_mikumariSetInChain);
            _mikumariSummonedInChain.Clear();
            _mikumariSetInChain.Clear();
            _acceptedMikumariEffects.Clear();
            base.OnChainEnd();
        }

        public override void OnSpSummoning()
        {
            _pendingSummonId = 0;
            base.OnSpSummoning();
        }

        public override void OnSummoning()
        {
            _pendingSummonId = 0;
            base.OnSummoning();
        }

        public override void OnMove(ClientCard card, int previousControler,
            int previousLocation, int currentControler, int currentLocation)
        {
            // Preserve the actual send-to-Graveyard event for the whole turn,
            // even if Albion subsequently leaves the Graveyard.
            if (card != null && card.IsCode(CardId.AlbionTheBrandedDragon) &&
                currentControler == 0 && currentLocation == (int)CardLocation.Grave &&
                previousLocation != (int)CardLocation.Grave)
                _albionSentToGraveThisTurn = true;
            if (previousLocation == (int)CardLocation.MonsterZone &&
                currentLocation != (int)CardLocation.MonsterZone)
            {
                ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
                int option;
                // The coin operation only destroys monsters; observing their
                // actual departure is safer than predicting which side loses.
                if (solving != null && solving.ActivatePlayer == 0 &&
                    solving.IsActivateCode(CardId.DarkTimeWizard) &&
                    !Duel.NegatedChainIndexList.Contains(Duel.SolvingChainIndex) &&
                    _plannedTimeWizardOptions.TryGetValue(Duel.SolvingChainIndex, out option) && option == 2)
                    _timeWizardDestroyedMonster = true;
                _spentBaronne.Remove(card);
                _synchroSummonedEkhajar.Remove(card);
                _mikumariSummonPending.Remove(card);
                _mikumariSetPending.Remove(card);
                _mikumariSummonedInChain.Remove(card);
                _mikumariSetInChain.Remove(card);
                _acceptedMikumariEffects.Remove(card);
                _ekhajarSummonPending.Remove(card);
                _ekhajarBouncePending.Remove(card);
                _ekhajarSummonedInChain.Remove(card);
                _ekhajarBounceInChain.Remove(card);
                _susanooSummonPending.Remove(card);
                _susanooSummonedInChain.Remove(card);
            }
            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override bool OnPreActivate(ClientCard card)
        {
            if (card.Controller == 0 && card.Location == CardLocation.MonsterZone &&
                card.IsFaceup() && card.IsDisabled())
                return false;
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Draw ||
                Duel.Phase == DuelPhase.Standby) && CanSummonGallantThief(
                    Bot.Hand.FirstOrDefault(c => c.IsCode(CardId.TheGreatGallantBandit))))
                return false;
            return base.OnPreActivate(card);
        }

        private bool StopExtraSummons()
        {
            // DefaultExecutor records successful resolutions, not activations.
            // Its shared state also ignores a negated Maxx C and resets per turn.
            return Duel.Player == 0 && enemyResolvedEffectIdList.Contains(CardId.MaxxC) &&
                !Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                    (c.IsFloodgate() || c.Attack >= 3000));
        }

        private bool AcceptEffect(int offset)
        {
            _usedEffects.Add(Util.GetStringId(Card.Id, offset));
            return true;
        }

        private bool IsDescription(int offset)
        {
            return ActivateDescription == Util.GetStringId(Card.Id, offset);
        }

        private bool CanRespondToOpponent()
        {
            ChainInfo latest = Duel.CurrentChainInfo.LastOrDefault();
            return latest != null && latest.ActivatePlayer == 1 &&
                !Duel.NegatedChainIndexList.Contains(Duel.CurrentChainInfo.Count);
        }

        private bool OpponentResponse()
        {
            // These locks apply only when activating the trap from the hand;
            // a previously Set copy keeps the existing response policy.
            if (Card.Location == CardLocation.Hand)
            {
                if (Card.IsCode(CardId.DominusImpulse) &&
                    (Bot.Hand.Any(c => c.IsMonster() &&
                        c.HasAttribute(CardAttribute.Wind | CardAttribute.Earth)) ||
                    Bot.GetMonsters().Concat(Bot.Graveyard).Concat(Bot.Banished).Any(c =>
                        c.HasAttribute(CardAttribute.Light | CardAttribute.Wind | CardAttribute.Earth) &&
                        HasUsefulRestrictedMonsterEffect(c))))
                    return false;
                if (Card.IsCode(CardId.SongsOfTheDominators) &&
                    (Bot.HasInHand(CardId.MaxxC) || Bot.HasInHand(CardId.DrollLockBird) ||
                        _albionSentToGraveThisTurn || Bot.GetMonsters().Concat(Bot.Graveyard)
                            .Concat(Bot.Banished).Any(HasUsefulOffFieldMonsterEffect)))
                    return false;
            }
            return CanRespondToOpponent() &&
                !IsOpponentChainCovered(Duel.CurrentChainInfo.Count) &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool HasUsefulRestrictedMonsterEffect(ClientCard monster)
        {
            if (monster.Location != CardLocation.MonsterZone)
                return HasUsefulOffFieldMonsterEffect(monster);
            if (!monster.IsFaceup() || DefaultCheckWhetherCardIsNegated(monster))
                return false;
            if (IsProtectedSynchroMaterial(monster))
                return true;
            if (monster.IsCode(CardId.PsychicEndPunisher))
                return !_usedEffects.Contains(Util.GetStringId(monster.Id, 1)) &&
                    Bot.LifePoints != Enemy.LifePoints ||
                    !_usedEffects.Contains(Util.GetStringId(monster.Id, 0)) && Bot.LifePoints > 1000 &&
                    Bot.GetMonsters().Any(c => c != monster && !c.IsExtraCard()) &&
                    OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, false).Any();
            if (_usedEffects.Contains(Util.GetStringId(monster.Id, 1)))
                return false;
            if (monster.IsCode(CardId.MikumariTheBanisherAshtra))
                return Bot.HasInDeck(CardId.YoaThePlagueAshtra, CardId.AkasunaTheBattleAshtra,
                    CardId.KuzuhaTheHexAshtra);
            return monster.IsCode(CardId.YoaThePlagueAshtra) &&
                Bot.GetSpellCountWithoutField() < 5 && Bot.HasInDeck(CardId.AshtrasDomain,
                    CardId.AshtrasWeb, CardId.AshtrasHowl);
        }

        private bool HasUsefulOffFieldMonsterEffect(ClientCard monster)
        {
            if (monster.Location == CardLocation.Grave)
            {
                if (monster.IsCode(CardId.AlbionTheBrandedDragon))
                    return _albionSentToGraveThisTurn && Bot.HasInDeck(CardId.TheFallenTheVirtuous);
                return monster.IsCode(CardId.EcclesiaAndTheDarkDragon) &&
                    !_usedEffects.Contains(Util.GetStringId(monster.Id, 1)) &&
                    Bot.Graveyard.Concat(Bot.Banished).Any(c => c.HasType(CardType.Fusion) &&
                        c.Level == 8 && (c.Location != CardLocation.Removed || c.IsFaceup())) &&
                    OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, true).Any();
            }
            // Songs also lasts through the next End Phase. Protect visible field
            // resources with a useful prospective GY effect, not arbitrary milled
            // monsters whose one-shot send-to-GY trigger window has already passed.
            if (monster.Location != CardLocation.MonsterZone || !monster.IsFaceup())
                return false;
            if (monster.IsCode(CardId.MikumariTheBanisherAshtra))
                return !_usedEffects.Contains(Util.GetStringId(monster.Id, 2)) &&
                    Bot.GetSpellCountWithoutField() < 5 && Bot.Graveyard.Any(c =>
                        c.IsTrap() && !c.HasType(CardType.Continuous | CardType.Counter)) &&
                    Bot.HasInExtra(new int[] { CardId.AshtrayaMikumariNoMikoto, CardId.AshtrajaSusanoo });
            return monster.IsCode(CardId.EkhajarDescendantDragonOfTheIceBarrier) &&
                _synchroSummonedEkhajar.Contains(monster) &&
                Duel.Player == 1 && !_usedEffects.Contains(Util.GetStringId(monster.Id, 2)) &&
                Enemy.Hand.Count + Enemy.GetMonsterCount() + Enemy.GetSpellCount() +
                    Enemy.Graveyard.Count > 0;
        }

        private bool IsOpponentChainCovered(int chainIndex)
        {
            if (chainIndex <= 0 || chainIndex > Duel.CurrentChainInfo.Count ||
                Duel.CurrentChainInfo[chainIndex - 1].ActivatePlayer != 1)
                return false;
            if (Duel.NegatedChainIndexList.Contains(chainIndex))
                return true;
            ChainInfo opponent = Duel.CurrentChainInfo[chainIndex - 1];
            for (int index = chainIndex; index < Duel.CurrentChainInfo.Count; index++)
            {
                ChainInfo own = Duel.CurrentChainInfo[index];
                if (own.ActivatePlayer != 0 || _resolvedChainIndices.Contains(index + 1) ||
                    Duel.NegatedChainIndexList.Contains(index + 1))
                    continue;
                ClientCard source = own.RelatedCard;
                // Leaving the field does not cancel an activated effect. Name
                // negation (Called by/Crossout), however, still applies off-field.
                if (source != null && DefaultCheckWhetherCardIsNegated(source))
                    continue;
                if (own.IsActivateCode(CardId.InfiniteImpermanence) &&
                    own.ActivateLocation == CardLocation.SpellZone &&
                    opponent.ActivateLocation == CardLocation.MonsterZone &&
                    opponent.RelatedCard != null && opponent.RelatedCard.Controller == 1 &&
                    opponent.RelatedCard.Location == CardLocation.MonsterZone &&
                    opponent.RelatedCard.IsFaceup() && own.Targets.Contains(opponent.RelatedCard))
                    return true;
                // Negating the preceding link is not the same as negating every
                // effect of its card. A later opponent insertion remains uncovered.
                if (index != chainIndex)
                    continue;
                if ((own.IsActivateCode(CardId.BaronneDeFleur, CardId.AdamancipatorRisenDragite,
                        CardId.TheGreatGallantBandit) &&
                    own.ActivateDescription == Util.GetStringId(own.ActivateId, 1)) ||
                    (own.IsActivateCode(CardId.DominusImpulse, CardId.SongsOfTheDominators,
                        CardId.SolemnAccusation) &&
                    own.ActivateDescription == Util.GetStringId(own.ActivateId, 0)))
                    return true;
                int option;
                if (own.IsActivateCode(CardId.AshtrasDomain) &&
                    _plannedTrapOptions.TryGetValue(index + 1, out option) && option == 2)
                    return true;
            }
            return false;
        }

        private bool MaxxCActivate()
        {
            return DefaultMaxxC();
        }

        private bool DrollActivate()
        {
            // The legal post-search window can arrive after LastChainPlayer
            // has been reset at chain end. The server supplies legal candidates.
            return Duel.Player == 1 &&
                !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool ImpermanenceActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            ChainInfo latest = Duel.CurrentChainInfo.LastOrDefault();
            return latest != null && latest.ActivatePlayer == 1 &&
                !IsOpponentChainCovered(Duel.CurrentChainInfo.Count) &&
                latest.ActivateLocation == CardLocation.MonsterZone &&
                latest.RelatedCard != null && latest.RelatedCard.Controller == 1 &&
                latest.RelatedCard.Location == CardLocation.MonsterZone &&
                !latest.RelatedCard.IsDisabled() && CanTarget(latest.RelatedCard, false) &&
                !IsRemovalReserved(latest.RelatedCard);
        }

        private bool SpiralActivate()
        {
            return !DefaultCheckWhetherCardIsNegated(Card) &&
                OrderedEnemyCards(Enemy.GetMonsters().Where(IsSpiralTargetAllowed), true, false).Any() &&
                (CanRespondToOpponent() || Duel.Player == 0 ||
                    Duel.Phase >= DuelPhase.BattleStart && Duel.Phase <= DuelPhase.Battle);
        }

        private bool IsSpiralTargetAllowed(ClientCard target)
        {
            if (target.Controller != 1 || target.Location != CardLocation.MonsterZone ||
                !target.IsMonster())
                return false;
            if (Bot.Graveyard.Any(c => c.IsTrap()))
                return true;
            // Preserve the user's hard Extra Deck-only rule. The revival option
            // can still use ANOTHER GY monster after an Extra Deck card is returned.
            if (!target.IsExtraCard())
                return false;
            bool riskyRevival = (target.Sequence < 5 || Enemy.GetMonstersInMainZone().Count < 5) &&
                Enemy.Graveyard.Any(c => c.Id != 0 && c.IsMonster() && c.IsCanRevive() &&
                    (c.IsFloodgate() || c.IsMonsterDangerous() || c.IsMonsterShouldBeDisabledBeforeItUseEffect() ||
                    c.Attack >= 2500 || c.Defense >= 2500 || c.HasType(CardType.Link) && c.LinkCount >= 3 ||
                    c.HasType(CardType.Xyz) && c.Rank >= 4 ||
                    c.HasType(CardType.Fusion | CardType.Synchro) && c.Level >= 8));
            if (!riskyRevival)
                return true;
            if (target.IsFaceup() && !target.IsDisabled() &&
                (target.IsFloodgate() || target.IsMonsterDangerous() ||
                    target.IsMonsterShouldBeDisabledBeforeItUseEffect()))
                return true;
            if (Duel.CurrentChainInfo.Where((info, index) => info.ActivatePlayer == 1 &&
                info.ActivateLocation == CardLocation.MonsterZone && info.RelatedCard == target &&
                !IsOpponentChainCovered(index + 1) && !_resolvedChainIndices.Contains(index + 1)).Any() &&
                !target.IsDisabled())
                return true;
            ClientCard defender = Bot.BattlingMonster;
            int damage = defender == null ? target.GetAttackPower() :
                defender.IsAttack() ? target.GetAttackPower() - defender.Attack : 0;
            return Duel.Player == 1 && Enemy.BattlingMonster == target && damage >= Bot.LifePoints;
        }

        private bool SolemnActivate()
        {
            return Bot.LifePoints > 1500 && OpponentResponse();
        }

        private bool MindMirrorForceActivate()
        {
            return !DefaultCheckWhetherCardIsNegated(Card) &&
                Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c => c.IsFaceup()) &&
                (CanRespondToOpponent() || Enemy.BattlingMonster != null);
        }

        private bool GallantThiefSummon()
        {
            if (!CanSummonGallantThief(Card))
                return false;
            _pendingSummonId = CardId.TheGreatGallantBandit;
            return true;
        }

        private bool CanSummonGallantThief(ClientCard gallantThief)
        {
            return gallantThief != null && gallantThief.IsCode(CardId.TheGreatGallantBandit) &&
                Bot.Hand.Contains(gallantThief) && Bot.GetMonsterCount() == 0 &&
                Enemy.GetMonsterCount() > 0 && (Enemy.GetMonsterCount() >= 2 ||
                    Bot.Hand.Any(c => c != gallantThief && c.IsMonster()));
        }

        private bool GallantThiefActivate()
        {
            if (Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;

            ChainInfo latest = Duel.CurrentChainInfo.LastOrDefault();
            // A monster can leave its activation location as cost. Use the
            // snapshot, not the mutable card, to recognize remote effects.
            bool canNegateRemoteEffect = CanRespondToOpponent() && latest != null &&
                latest.HasLocation(CardLocation.Hand | CardLocation.Grave | CardLocation.Removed);
            bool canChangeBattlePositions = Duel.Phase >= DuelPhase.BattleStart &&
                Duel.Phase < DuelPhase.Main2 && Enemy.GetMonsters().Any(c =>
                    c.IsAttack() && !c.HasType(CardType.Link));
            if (IsDescription(1))
                return canNegateRemoteEffect;
            if (IsDescription(2))
                return canChangeBattlePositions;
            return ActivateDescription == -1 &&
                (canNegateRemoteEffect || canChangeBattlePositions);
        }

        private bool AshtraHandSummon()
        {
            if (StopExtraSummons())
                return false;
            _pendingSummonId = Card.Id;
            return true;
        }

        private bool CanUseLilithSearch(ClientCard spentSpell = null)
        {
            // A resolving Graverobber normally vacates its own Spell/Trap slot
            // before the revived Lilith can start a new chain.
            bool expectedVacancy = spentSpell != null && spentSpell.Controller == 0 &&
                spentSpell.Location == CardLocation.SpellZone && spentSpell.Sequence < 5 &&
                spentSpell.IsCode(CardId.FoolishGraverobber) && Bot.GetSpells().Contains(spentSpell);
            return !_usedEffects.Contains(Util.GetStringId(CardId.LilithLadyOfLament, 0)) &&
                !DefaultCheckWhetherCardIdIsNegated(CardId.LilithLadyOfLament) &&
                Bot.GetSpellCountWithoutField() - (expectedVacancy ? 1 : 0) < 5 &&
                _lilithTrapIds.Sum(id => Bot.GetCardCountInDeck(id)) >= 3;
        }

        private bool LilithSummon()
        {
            // Gallant Thief and the Ashtra hand starters keep their earlier
            // entries. Do not spend the Normal Summon on a 1000-ATK body alone.
            return Card.Location == CardLocation.Hand && CanUseLilithSearch();
        }

        private bool LilithActivate()
        {
            if (Card.Location != CardLocation.MonsterZone ||
                DefaultCheckWhetherCardIsNegated(Card) || !CanUseLilithSearch() ||
                !Bot.GetMonsters().Any(c => LilithTributePriority(c) < 100))
                return false;
            // The script has one field activation and no effect description.
            // Use a deck-local usage key rather than guessing a string offset.
            return AcceptEffect(0);
        }

        private int LilithTributePriority(ClientCard monster)
        {
            if (monster.Controller != 0 || monster.Location != CardLocation.MonsterZone ||
                !monster.IsMonster() || !monster.HasAttribute(CardAttribute.Dark))
                return 100;
            if (monster.IsCode(CardId.LilithLadyOfLament))
                return 0;
            if (!monster.IsExtraCard() && monster.IsDisabled())
                return 1;
            return 100;
        }

        private bool MikumariActivate()
        {
            if (Card.Location == CardLocation.Grave)
                return Bot.GetSpellCountWithoutField() < 5 && AcceptEffect(2);
            return !DefaultCheckWhetherCardIsNegated(Card) && AcceptEffect(1);
        }

        private bool YoaActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Duel.Player == 0)
                return Bot.GetSpellCountWithoutField() < 5 && AcceptEffect(1);
            // Changing an opponent's level is useful against a visible tuner /
            // non-tuner board. Do not spend this effect on arbitrary Link/Xyz cards.
            return Enemy.GetMonsters().Any(c => c.IsFaceup() && c.Level > 0 &&
                CanTarget(c, true)) && Enemy.GetMonsters().Any(c => c.HasType(CardType.Tuner)) &&
                AcceptEffect(2);
        }

        private bool AkasunaActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Duel.Phase == DuelPhase.End)
                return Bot.GetSpellCountWithoutField() < 5 && AcceptEffect(2);
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2 ||
                !IsDescription(1) || _usedEffects.Contains(Util.GetStringId(Card.Id, 1)))
                return false;
            // Own-turn plays and opponent Main Phase free-chain windows are
            // both useful. Setting a monster does not negate its active link.
            if (Duel.CurrentChain.Count > 0 && !CanRespondToOpponent())
                return false;
            List<ClientCard> monsters = Enemy.GetMonsters().Where(c =>
                CanAkasunaSet(c) && !IsRemovalReserved(c)).ToList();
            if (monsters.Count == 0)
                return false;
            int costPriority = Bot.Hand.Concat(Bot.GetSpells()).Select(AkasunaCostPriority)
                .DefaultIfEmpty(1000).Min();
            if (costPriority >= 1000)
                return false;
            bool important = monsters.Any(c => !c.IsDisabled() &&
                (c.IsFloodgate() || c.IsMonsterDangerous() ||
                    c.IsMonsterShouldBeDisabledBeforeItUseEffect()));
            int liveEffects = monsters.Count(c => !c.IsDisabled() && c.HasType(CardType.Effect));
            bool combat = false;
            if (Duel.Player == 0 && Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1)
            {
                List<ClientCard> attackers = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                    (c.IsAttack() || c.IsMonsterAttackWhileInDefPos()) && !c.Attacked &&
                    c.GetAttackPower() > 0).ToList();
                if (attackers.Count > 0)
                {
                    int strongest = attackers.Max(c => c.GetAttackPower());
                    // Do not assume hidden DEF, piercing or a future attacker.
                    combat = monsters.Any(c => c.IsAttack() && c.Attack >= strongest &&
                        c.Attack >= 1800 && c.Defense < strongest);
                }
            }
            if (!important && liveEffects < 2 && !combat)
                return false;
            // A unique premium interruption is only traded for a known threat
            // or at least three live effects, not an ordinary combat obstacle.
            if (costPriority >= 100 && !important && liveEffects < 3)
                return false;
            return AcceptEffect(1);
        }

        private bool CanAkasunaSet(ClientCard monster)
        {
            // This is a non-targeting position change: targeting protection is
            // not a reason to reject it. Opaque immunity remains engine-owned.
            return monster != null && monster.Controller == 1 &&
                monster.Location == CardLocation.MonsterZone && monster.IsFaceup() &&
                !monster.HasType(CardType.Link | CardType.Token);
        }

        private int AkasunaCostPriority(ClientCard trap)
        {
            if (trap == null || trap.Controller != 0 || !trap.IsTrap() ||
                trap.Location != CardLocation.Hand &&
                (trap.Location != CardLocation.SpellZone || !trap.IsFacedown()))
                return 1000;
            if (Bot.Hand.Concat(Bot.GetSpells()).Count(c => c.IsCode(trap.Id)) > 1)
                return 0;
            // Only name-limited traps lose their next activation to a prior use.
            // Imperm, Mind Mirror and Solemn are not spent merely by card name.
            if ((IsAshtraTrap(trap) || trap.IsCode(CardId.DominusImpulse,
                CardId.DominusSpiral, CardId.SongsOfTheDominators)) &&
                _usedEffects.Contains(Util.GetStringId(trap.Id, 0)))
                return 10;
            if (IsAshtraTrap(trap) &&
                (Bot.GetMonsters().Any(c => c.IsCode(CardId.AshtrayaMikumariNoMikoto) &&
                    c.IsFaceup() && !c.IsDisabled()) ||
                Bot.HasInGraveyard(CardId.MikumariTheBanisherAshtra)))
                return 20;
            if (trap.IsCode(CardId.MindMirrorForce, CardId.SolemnAccusation,
                CardId.DominusImpulse, CardId.DominusSpiral, CardId.InfiniteImpermanence) ||
                IsAshtraTrap(trap))
                return 100;
            return 30;
        }

        private bool KuzuhaActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Duel.Phase == DuelPhase.End)
                return Bot.GetSpellCountWithoutField() < 5 && AcceptEffect(2);
            return OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()),
                true, true).Any() && AcceptEffect(1);
        }

        private bool GateActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Card.Location == CardLocation.Hand || Card.IsFacedown())
                return !Bot.HasInSpellZone(CardId.AshtrashenGateToTheWorldsBeyond);
            if (IsDescription(1) || ActivateDescription == -1)
                return OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()),
                    true, false).Any() && AcceptEffect(1);
            if (!IsDescription(0))
                return false;
            return Bot.GetMonsters().Concat(Bot.GetSpells()).Any(c =>
                CanPreserveGateSetCard(c, true)) && AcceptEffect(0);
        }

        private bool CanPreserveGateSetCard(ClientCard sentCard, bool idleCheck = false)
        {
            if (sentCard == null || sentCard.Controller != 0 || !sentCard.IsFacedown() ||
                sentCard.Location != CardLocation.MonsterZone &&
                sentCard.Location != CardLocation.SpellZone)
                return false;
            // Keep a face-down on-field card for the hand summon procedure.
            // A face-down monster also counts; this is not limited to backrow.
            if (Bot.GetMonsters().Concat(Bot.GetSpells()).Any(c => c != sentCard &&
                c.IsFacedown() && !c.HasType(CardType.Field)))
                return true;
            int spellCount = Bot.GetSpellCountWithoutField() -
                (sentCard.Location == CardLocation.SpellZone && !sentCard.HasType(CardType.Field) ? 1 : 0);
            if (spellCount >= AshtraSetLimit())
                return false;
            List<ClientCard> handSets = Bot.Hand.Where(c => c.IsTrap() ||
                c.HasType(CardType.QuickPlay)).ToList();
            if (!idleCheck)
                return handSets.Count > 0;
            // Only idle-command evaluation may read the current legal set list.
            // If all five zones are occupied, sending a backrow frees a zone;
            // the absent pre-send candidate alone does not imply a set lock.
            return Duel.MainPhase.SpellSetableCards.Any(handSets.Contains) ||
                Bot.GetSpellCountWithoutField() >= 5 &&
                sentCard.Location == CardLocation.SpellZone && handSets.Count > 0;
        }

        private bool MikumariNoMikotoActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            int effect = IsDescription(0) ? 0 : IsDescription(1) ? 1 : -1;
            if (ActivateDescription == -1)
            {
                bool summonPending = _mikumariSummonPending.Contains(Card);
                bool setPending = _mikumariSetPending.Contains(Card);
                // Unknown descriptions are only safe when the visible events
                // identify exactly one branch; never assume -1 means offset 0.
                if (summonPending == setPending)
                    return false;
                effect = summonPending ? 0 : 1;
            }
            if (effect < 0)
                return false;
            // Sending a field card is compulsory after the optional mill.
            // Do not start this effect just to lose one of our own resources.
            bool accepted = effect == 1 ? Bot.GetSpellCountWithoutField() < 5 :
                Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c => !IsRemovalReserved(c));
            if (accepted)
                _acceptedMikumariEffects[Card] = effect;
            return accepted;
        }

        private bool CanUseSusanooClear(IList<ClientCard> materials = null)
        {
            if (IsRemovalReservedOnAllEnemies() ||
                !Bot.Graveyard.Any(c => c.IsTrap()) &&
                (materials == null || !materials.Any(c =>
                    ((c.Data != null ? c.Data.Type : c.Type) & (int)CardType.Trap) != 0 &&
                    WillSynchroMaterialReachGraveyard(c))))
                return false;
            return Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c =>
                !IsRemovalReserved(c) && (c.IsFloodgate() || c.IsFaceup() &&
                    (c.IsShouldNotBeTarget() || c.IsMonsterDangerous() || c.Attack >= 3000))) ||
                Enemy.GetMonsters().Concat(Enemy.GetSpells()).Count(c => !IsRemovalReserved(c)) >= 3;
        }

        private bool SusanooActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            int effect = IsDescription(0) ? 0 : IsDescription(1) ? 1 : -1;
            if (ActivateDescription == -1)
            {
                // Summon-success takes precedence over the phase: traps can
                // perform a Synchro summon even during the End Phase.
                if (_susanooSummonPending.Contains(Card))
                    effect = 0;
                else if (Duel.Phase == DuelPhase.End)
                    effect = 1;
            }
            if (effect == 1)
                return Bot.Graveyard.Concat(Bot.Banished).Any(c =>
                    IsAshtraMonster(c) || IsAshtraTrap(c)) && AcceptEffect(1);
            if (effect != 0)
                return false;
            return CanUseSusanooClear() && AcceptEffect(0);
        }

        private bool CanReleaseSusanooForRecovery(ClientCard source)
        {
            // Trading two resources must not remove our only useful defender.
            return source != null && source.IsDisabled() ||
                Bot.GetMonsters().Any(c => c != source && c.IsFaceup() && !c.IsDisabled() &&
                    (IsProtectedSynchroMaterial(c) ||
                    c.IsCode(CardId.SwordsoulSupremeSovereignChengying,
                        CardId.PsychicEndPunisher, CardId.RedEyesBlackDragonExceed, CardId.AshtrajaSusanoo)));
        }

        private int SusanooRecoveryPriority(ClientCard card)
        {
            if (IsAshtraTrap(card))
                return TrapPriority(card);
            return 10 + SearchPriority(card);
        }

        private bool CanUseDomainNegate(ChainInfo chain)
        {
            return chain != null && chain.ActivatePlayer == 1 &&
                (chain.ActivateType & (int)(CardType.Spell | CardType.Trap)) != 0 &&
                !IsOpponentChainCovered(Duel.CurrentChainInfo.IndexOf(chain) + 1) &&
                !Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c => c.IsFacedown());
        }

        private bool AshtraTrapInterruption()
        {
            if (DefaultCheckWhetherCardIsNegated(Card) ||
                Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0)
                return false;
            if (Card.IsCode(CardId.AshtrasDomain))
                return CanUseDomainNegate(Duel.CurrentChainInfo.LastOrDefault());
            // Ordinary trap plays wait for an earlier archetype trap to finish.
            bool destroy = !Duel.CurrentChainInfo.Any(i => i.ActivatePlayer == 0 &&
                i.IsActivateCode(CardId.AshtrasDomain, CardId.AshtrasWeb, CardId.AshtrasHowl)) &&
                Card.IsCode(CardId.AshtrasHowl) &&
                Bot.GetMonsters().Concat(Bot.Graveyard).Any(IsAshtraMonster) &&
                Enemy.GetMonsters().Any(IsWorthHowlDestroying) &&
                (CanRespondToOpponent() || Duel.Player == 0);
            if (destroy)
            {
                _pendingHowlChainIndex = Duel.CurrentChain.Count + 1;
                _plannedTrapOptions[_pendingHowlChainIndex] = 2;
            }
            return destroy;
        }

        private bool AshtraTrapActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card) ||
                Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0)
                return false;
            // A direct Spell/Trap negate is the only exception to waiting for
            // our earlier archetype trap to finish when the opponent responds.
            if (AshtraTrapInterruption())
                return true;
            if (Duel.CurrentChainInfo.Any(i => i.ActivatePlayer == 0 &&
                i.IsActivateCode(CardId.AshtrasDomain, CardId.AshtrasWeb, CardId.AshtrasHowl)))
                return false;
            if (Card.IsCode(CardId.AshtrasWeb) && CanUseWebRevival() &&
                (Bot.GetMonsterCount() == 0 || CanRespondToOpponent() ||
                    Duel.Phase == DuelPhase.End || !CanUseTrapTuner(Card)))
            {
                _plannedTrapOptions[Duel.CurrentChain.Count + 1] = 2;
                return true;
            }
            // A trap-tuner is not summoned without a useful non-tuner to pair
            // with. During Maxx C, retain these traps for their other branches.
            if (StopExtraSummons() || !CanUseTrapTuner(Card))
                return false;
            if (Card.IsCode(CardId.AshtrasWeb))
                _plannedTrapOptions[Duel.CurrentChain.Count + 1] = 1;
            if (Card.IsCode(CardId.AshtrasHowl))
            {
                _pendingHowlChainIndex = Duel.CurrentChain.Count + 1;
                _plannedTrapOptions[_pendingHowlChainIndex] = 1;
            }
            return true;
        }

        private bool CanUseWebRevival()
        {
            return (Duel.Player == 0 || Duel.Phase == DuelPhase.End) &&
                !StopExtraSummons() && Bot.GetMonstersInMainZone().Count < 5 &&
                Bot.Graveyard.Any(c => IsAshtraMonster(c) && c.IsCanRevive());
        }

        private bool IsWorthHowlDestroying(ClientCard monster)
        {
            if (monster == null || monster.Controller != 1 ||
                monster.Location != CardLocation.MonsterZone || !monster.IsMonster() ||
                IsRemovalReserved(monster) || !CanTarget(monster, false))
                return false;
            // Merely starting an effect, being an Extra Deck monster or having
            // high ATK does not make destruction a useful interruption.
            if (monster.IsFaceup() && !monster.IsDisabled() &&
                (monster.IsFloodgate() || monster.IsMonsterDangerous() ||
                    monster.IsMonsterShouldBeDisabledBeforeItUseEffect()))
                return true;
            if (Duel.Player == 1 && Enemy.BattlingMonster == monster)
            {
                ClientCard defender = Bot.BattlingMonster;
                int damage = defender == null ? monster.GetAttackPower() :
                    defender.IsAttack() ? monster.GetAttackPower() - defender.Attack : 0;
                return damage > 0 && (damage >= Bot.LifePoints || damage >= 1800);
            }
            if (Duel.Player != 0 || Duel.Turn <= 1 ||
                Duel.Phase != DuelPhase.Main1 &&
                (Duel.Phase < DuelPhase.BattleStart || Duel.Phase > DuelPhase.Battle))
                return false;
            List<ClientCard> attackers = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                (c.IsAttack() || c.IsMonsterAttackWhileInDefPos()) && !c.Attacked &&
                c.GetAttackPower() > 0).ToList();
            if (attackers.Count == 0)
                return false;
            // Removing the sole blocker can open a visible lethal attack.
            if (Enemy.GetMonsterCount() == 1 && attackers.Any(c =>
                c.GetAttackPower() >= Enemy.LifePoints))
                return true;
            return monster.IsFaceup() && monster.GetDefensePower() >= 2500 &&
                monster.GetDefensePower() >= attackers.Max(c => c.GetAttackPower());
        }

        private bool CanUseTrapTuner(ClientCard trap)
        {
            return trap != null &&
                Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c => !IsRemovalReserved(c)) &&
                Bot.GetMonsters().Any(c => c.IsFaceup() && !c.HasType(CardType.Tuner) &&
                c.Level > 0 && !c.IsMonsterNotBeSynchroMaterial() && !IsProtectedSynchroMaterial(c) &&
                Bot.ExtraDeck.Any(e => e.HasType(CardType.Synchro) && e.Level == c.Level + 1 &&
                    SynchroPriority(e, new List<ClientCard> { c, trap }) < 100));
        }

        private bool TimeWizardActivate()
        {
            if ((Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) ||
                DefaultCheckWhetherCardIsNegated(Card))
                return false;
            bool search = !_timeSearchUsed && Bot.HasInDeck(CardId.FoolishGraverobber) &&
                !Bot.HasInHand(CardId.FoolishGraverobber);
            bool coin = CanUseTimeWizardCoin(null);
            // Coin wins over resource-search only in the deliberately prepared
            // starter chain. Ordinary searches have no monster-count threshold.
            if (coin && HasTimeWizardStarterInChain())
                return PlanTimeWizardOption(2);
            if (search)
                return PlanTimeWizardOption(1);
            return coin && PlanTimeWizardOption(2);
        }

        private bool TimeWizardComboActivate()
        {
            return HasTimeWizardStarterInChain() && CanUseTimeWizardCoin(null) &&
                !DefaultCheckWhetherCardIsNegated(Card) && PlanTimeWizardOption(2);
        }

        private bool TimeWizardStarterActivate()
        {
            if (Duel.CurrentChain.Count != 0 || Card.Location != CardLocation.MonsterZone ||
                !IsDescription(1) || DefaultCheckWhetherCardIsNegated(Card) ||
                _usedEffects.Contains(Util.GetStringId(Card.Id, 1)) || !CanUseTimeWizardCoin(Card))
                return false;
            // This is an idle-command check, so this candidate list is current.
            // Never read it in a trigger or chain-selection callback.
            if (!Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.DarkTimeWizard) &&
                !DefaultCheckWhetherCardIsNegated(c)))
                return false;
            return Card.IsCode(CardId.MikumariTheBanisherAshtra) ? MikumariActivate() : YoaActivate();
        }

        private bool PlanTimeWizardOption(int option)
        {
            _pendingTimeWizardChainIndex = Duel.CurrentChain.Count + 1;
            _plannedTimeWizardOptions[_pendingTimeWizardChainIndex] = option;
            return true;
        }

        private bool HasTimeWizardStarterInChain()
        {
            return Duel.Player == 0 && Duel.CurrentChainInfo.Where((info, index) =>
                info.ActivatePlayer == 0 && info.ActivateLocation == CardLocation.MonsterZone &&
                info.IsActivateCode(CardId.MikumariTheBanisherAshtra, CardId.YoaThePlagueAshtra) &&
                info.ActivateDescription == Util.GetStringId(info.ActivateId, 1) &&
                !Duel.NegatedChainIndexList.Contains(index + 1)).Any();
        }

        private bool CanUseTimeWizardCoin(ClientCard startingMonster)
        {
            if (_timeCoinUsed || _plannedTimeWizardOptions.Values.Contains(2) ||
                Enemy.GetMonsterCount() < 2 || Bot.GetMonsterCount() >= Enemy.GetMonsterCount() ||
                !Enemy.GetMonsters().Any(c => !IsRemovalReserved(c)))
                return false;
            if (Duel.Player == 1)
            {
                if (Bot.GetMonsterCount() == 0)
                    return true;
                // Last resort only at a visible lethal attack, not a speculative
                // prediction that the opponent might clear our board later.
                ClientCard attacker = Enemy.BattlingMonster;
                ClientCard defender = Bot.BattlingMonster;
                return (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.BattleStep) &&
                    attacker != null && defender != null && defender.IsAttack() &&
                    attacker.Attack - defender.Attack >= Bot.LifePoints &&
                    !IsRemovalReserved(attacker) && !Bot.GetSpells().Any(c =>
                        c.IsFacedown() && IsAshtraTrap(c) && CanUseTrapTuner(c));
            }
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;
            if (Bot.GetMonsterCount() == 0)
                return true;
            if (StopExtraSummons() || _redEyesSummonedThisTurn ||
                !Bot.HasInExtra(CardId.RedEyesBlackDragonExceed))
                return false;
            return Bot.GetMonsters().All(c => c.IsFaceup() &&
                c.IsCode(CardId.YoaThePlagueAshtra, CardId.MikumariTheBanisherAshtra) &&
                (c == startingMonster || _usedEffects.Contains(Util.GetStringId(c.Id, 1))));
        }

        private bool GraverobberActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Card.Location == CardLocation.Grave)
                return Bot.GetSpellCountWithoutField() < AshtraSetLimit() &&
                    Bot.Graveyard.Concat(Enemy.Graveyard).Any(c => GraverobberSetPriority(c) < 100);
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone)
                return false;
            return !StopExtraSummons() && Bot.GetMonstersInMainZone().Count < 5 &&
                !DefaultCheckWhetherBotWillBeBanished(CardType.Spell, CardLocation.Deck) &&
                Bot.Graveyard.Concat(Enemy.Graveyard).Any(c => GraverobberRevivePriority(c, false, Card) < 100);
        }

        private int GraverobberRevivePriority(ClientCard monster, bool serverCandidate = false,
            ClientCard spentSpell = null)
        {
            if (monster == null || monster.Location != CardLocation.Grave || !monster.IsMonster() ||
                !serverCandidate && !monster.IsCanRevive())
                return 100;
            // Prefer Lilith in either turn only while her search still has a
            // payoff; a spent search or a full backrow is not a useful revival.
            if (monster.Controller == 0 && monster.IsCode(CardId.LilithLadyOfLament))
            {
                ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
                if (spentSpell == null && solving != null && solving.ActivatePlayer == 0 &&
                    solving.IsActivateCode(CardId.FoolishGraverobber))
                    spentSpell = solving.RelatedCard;
                return CanUseLilithSearch(spentSpell) && !DefaultCheckWhetherCardIsNegated(monster) ? 0 : 100;
            }
            if (monster.Controller == 0 && monster.HasType(CardType.Synchro))
                return 1;
            if (monster.Controller == 1 && monster.Level >= 8 &&
                monster.HasType(CardType.Synchro | CardType.Fusion))
                return 2;
            if (monster.Controller != 0)
                return 100;
            foreach (ClientCard synchro in Bot.ExtraDeck.Where(c => c.HasType(CardType.Synchro)))
            {
                List<ClientCard> materials = FindSynchroMaterials(synchro.Level,
                    synchro.IsCode(CardId.AdamancipatorRisenDragite) &&
                    !Bot.Graveyard.Any(c => c.Attribute == (int)CardAttribute.Water), monster);
                if (materials != null && SynchroPriority(synchro, materials) < 100)
                    return 3;
            }
            // With no useful synchro revival/material, save the card on the
            // opponent's turn. On our turn an unused resource starter is useful.
            if (Duel.Player == 0 && monster.IsCode(CardId.MikumariTheBanisherAshtra,
                CardId.YoaThePlagueAshtra) &&
                !_usedEffects.Contains(Util.GetStringId(monster.Id, 1)) &&
                !DefaultCheckWhetherCardIsNegated(monster))
            {
                if (monster.IsCode(CardId.YoaThePlagueAshtra))
                    return Bot.GetSpellCountWithoutField() < 5 && Bot.HasInDeck(new int[] {
                        CardId.AshtrasDomain, CardId.AshtrasWeb, CardId.AshtrasHowl }) ? 4 : 100;
                return Bot.HasInDeck(new int[] { CardId.MikumariTheBanisherAshtra,
                    CardId.YoaThePlagueAshtra }) ? 4 : 100;
            }
            return 100;
        }

        private int GraverobberSetPriority(ClientCard spell)
        {
            if (spell == null || spell.Location != CardLocation.Grave ||
                !spell.IsSpell() && !spell.IsTrap())
                return 100;
            if (Bot.HasInHand(spell.Id) || Bot.HasInSpellZone(spell.Id))
                return 100;
            if (spell.IsCode(CardId.DarkTimeWizard))
                return 0;
            // Only foreign cards with existing executors and no required foreign
            // engine are accepted. Own unlisted traps are not legal in this script.
            if (spell.Controller != 1)
                return 100;
            if (spell.IsCode(CardId.MindMirrorForce))
                return 1;
            if (spell.IsCode(CardId.SolemnAccusation))
                return Bot.LifePoints > 1500 ? 2 : 100;
            if (spell.IsCode(CardId.InfiniteImpermanence))
                return 3;
            if (spell.IsCode(CardId.TheFallenTheVirtuous))
                return Bot.HasInExtra(new int[] { CardId.AlbionTheBrandedDragon,
                    CardId.EcclesiaAndTheDarkDragon }) ? 4 : 100;
            return 100;
        }

        private bool FallenActivate()
        {
            return !DefaultCheckWhetherCardIsNegated(Card) &&
                Bot.HasInExtra(new int[] { CardId.AlbionTheBrandedDragon,
                    CardId.EcclesiaAndTheDarkDragon }) &&
                OrderedFallenTargets(Enemy.GetMonsters().Concat(Enemy.GetSpells())).Any();
        }

        private bool EcclesiaActivate()
        {
            return Card.Location == CardLocation.Grave &&
                Bot.Graveyard.Concat(Bot.Banished).Any(c => c.HasType(CardType.Fusion) &&
                    c.Level == 8) &&
                OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, true).Any();
        }

        private bool AlbionActivate()
        {
            return Card.Location == CardLocation.Grave && Bot.HasInDeck(CardId.TheFallenTheVirtuous);
        }

        private bool RedEyesSummon()
        {
            if (Duel.Player != 0 || _redEyesSummonedThisTurn || StopExtraSummons())
                return false;
            if (!Enemy.GetMonsters().Any(c => c.IsFaceup()) &&
                !Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsExtraCard()))
                return false;
            _pendingSummonId = CardId.RedEyesBlackDragonExceed;
            return true;
        }

        private bool RedEyesComboSummon()
        {
            // Still runs only against a legal server-provided summon candidate.
            // A negated coin, failed destruction or unavailable tribute cannot
            // manufacture that candidate locally.
            return _timeWizardDestroyedMonster && RedEyesSummon();
        }

        private bool RedEyesActivate()
        {
            return !DefaultCheckWhetherCardIsNegated(Card) && !StopExtraSummons();
        }

        private bool BaronneActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (IsDescription(1) && CanRespondToOpponent() &&
                !IsOpponentChainCovered(Duel.CurrentChainInfo.Count))
            {
                _spentBaronne.Add(Card);
                return true;
            }
            if (IsDescription(2) || Duel.Phase == DuelPhase.Standby)
                return _spentBaronne.Contains(Card) && !StopExtraSummons();
            return IsDescription(0) && OrderedEnemyCards(
                Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, true).Any();
        }

        private bool DragiteActivate()
        {
            // There are no Rock monsters in this deck: the excavation branch
            // cannot return cards and should not be fired just to reorder it.
            return IsDescription(1) && OpponentResponse();
        }

        private bool ChengyingActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            // The script's destruction replacement uses system description 96
            // and does not create a chain link of its own.
            if (ActivateDescription == 96)
                _chengyingReplacementPending = true;
            return true;
        }

        private bool EkhajarActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (Card.Location == CardLocation.Grave)
                return Enemy.Hand.Count + Enemy.GetMonsterCount() + Enemy.GetSpellCount() +
                    Enemy.Graveyard.Count > 0 && AcceptEffect(2);
            int effect = IsDescription(0) ? 0 : IsDescription(1) ? 1 : -1;
            if (ActivateDescription == -1)
            {
                bool summon = _ekhajarSummonPending.Contains(Card);
                bool bounce = _ekhajarBouncePending.Contains(Card);
                if (summon == bounce)
                    return false;
                effect = summon ? 0 : 1;
            }
            if (effect < 0 || _usedEffects.Contains(Util.GetStringId(Card.Id, effect)))
                return false;
            List<ClientCard> enemies = OrderedEnemyCards(Enemy.GetMonsters()
                .Concat(Enemy.GetSpells()).Concat(Enemy.Graveyard), false, true).ToList();
            bool worthwhile = effect == 0 ? enemies.Count >= 2 &&
                enemies.Any(c => EkhajarTargetPriority(c) < 100) :
                enemies.Concat(Bot.GetMonsters()).Concat(Bot.Graveyard)
                    .Any(c => EkhajarTargetPriority(c) < 100);
            // The grave replacement is script-dependent and cannot be inferred
            // from this deck. Actual cost prompts still use server candidates.
            if (!worthwhile || !CanPayEkhajarCost(enemies, effect == 0))
                return false;
            if (effect == 0)
            {
                _ekhajarSummonPending.Remove(Card);
                _ekhajarSummonedInChain.Remove(Card);
            }
            else
            {
                _ekhajarBouncePending.Remove(Card);
                _ekhajarBounceInChain.Remove(Card);
            }
            return AcceptEffect(effect);
        }

        private int EkhajarTargetPriority(ClientCard card)
        {
            if (card == null || IsRemovalReserved(card))
                return 100;
            if (card.Controller == 1)
            {
                if (card.Location == CardLocation.Grave)
                {
                    bool activating = Duel.CurrentChainInfo.Where((info, index) =>
                        info.ActivatePlayer == 1 && info.RelatedCard == card &&
                        info.ActivateLocation == CardLocation.Grave &&
                        !Duel.NegatedChainIndexList.Contains(index + 1)).Any();
                    return activating ? 2 : card.IsExtraCard() ? 15 : 100;
                }
                return card.IsFacedown() || IsWorthDestroying(card) ? ThreatPriority(card) : 100;
            }
            // Never bounce our Extra Deck monsters merely to recover a card.
            if (!IsAshtraMonster(card) || card.IsExtraCard())
                return 100;
            if (card.Location == CardLocation.MonsterZone &&
                Duel.CurrentChainInfo.Where((info, index) => info.ActivatePlayer == 1 &&
                    info.Targets.Contains(card) && !Duel.NegatedChainIndexList.Contains(index + 1)).Any())
                return 0;
            if (card.Location == CardLocation.Grave && Duel.Player == 0 &&
                !Bot.HasInHand(card.Id) && !_usedEffects.Contains(Util.GetStringId(card.Id, 1)) &&
                Bot.GetMonsters().Concat(Bot.GetSpells()).Any(c => c.IsFacedown()))
                return 30 + SearchPriority(card);
            return 100;
        }

        private int EkhajarCostPriority(ClientCard card, bool discard)
        {
            if (discard && card.Location == CardLocation.Grave)
                return 0; // Only offered if a legal discard replacement exists.
            if (Bot.Hand.Count(c => c.IsCode(card.Id)) > 1)
                return 5;
            if (discard && IsAshtraTrap(card) &&
                (Bot.HasInMonstersZone(new int[] { CardId.AshtrayaMikumariNoMikoto, CardId.AshtrajaSusanoo }) ||
                Bot.HasInGraveyard(CardId.AshtrayaMikumariNoMikoto)))
                return 10;
            if (_usedEffects.Contains(Util.GetStringId(card.Id, 1)))
                return 20;
            if (card.IsTrap() && !card.IsCode(CardId.MindMirrorForce,
                CardId.SolemnAccusation, CardId.InfiniteImpermanence, CardId.DominusImpulse))
                return 30 + TrapPriority(card);
            return 100;
        }

        private bool CanPayEkhajarCost(IEnumerable<ClientCard> enemies, bool discard)
        {
            bool urgent = enemies.Any(c => c.Location != CardLocation.Grave && c.IsFaceup() &&
                !c.IsDisabled() && (c.IsFloodgate() || c.IsMonsterDangerous() ||
                c.IsMonsterShouldBeDisabledBeforeItUseEffect()));
            return Bot.Hand.Any(c => EkhajarCostPriority(c, discard) < 100 || urgent);
        }

        private bool PunisherActivate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;
            if (IsDescription(1) || Duel.Phase == DuelPhase.BattleStart)
                return true;
            return Bot.LifePoints > 1000 && Bot.GetMonsters().Any(c => c != Card &&
                !c.IsExtraCard()) && OrderedEnemyCards(
                    Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, false).Any();
        }

        private bool TyphonSummon()
        {
            return !StopExtraSummons() && Enemy.GetMonsters().Any(c => c.IsFaceup() &&
                c.IsExtraCard() && c.Attack >= 3000) &&
                (Duel.Phase == DuelPhase.Main2 || !Duel.MainPhase.SpecialSummonableCards.Any(c =>
                    c != Card && IsAshtraMonster(c)));
        }

        private bool TyphonActivate()
        {
            return !DefaultCheckWhetherCardIsNegated(Card) &&
                OrderedEnemyCards(Enemy.GetMonsters(), false, false).Any();
        }

        private bool SynchroSummon()
        {
            if (StopExtraSummons())
                return false;
            List<ClientCard> materials = FindSynchroMaterialsFor(Card);
            if (materials == null)
                return false;
            if (materials.Any(c => c.IsExtraCard() && !c.IsDisabled()) &&
                Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0)
                return false;
            int priority = SynchroPriority(Card);
            if (priority >= 100)
                return false;
            if (Duel.MainPhase.SpecialSummonableCards.Any(c => c != Card &&
                c.HasType(CardType.Synchro) && SynchroPriority(c) < priority &&
                FindSynchroMaterialsFor(c) != null))
                return false;
            AI.SelectMaterials(materials, HintMsg.SynchroMaterial);
            return true;
        }

        private int SynchroPriority(ClientCard card)
        {
            return SynchroPriority(card, FindSynchroMaterialsFor(card));
        }

        private int SynchroPriority(ClientCard card, IList<ClientCard> materials)
        {
            if (card.IsCode(CardId.AshtrayaMikumariNoMikoto))
                return !Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c =>
                    !IsRemovalReserved(c)) ? 100 : Bot.HasInMonstersZone(card.Id) ? 80 : 0;
            if (card.IsCode(CardId.BaronneDeFleur))
                return 10;
            if (card.IsCode(CardId.SwordsoulSupremeSovereignChengying))
                return 20;
            if (card.IsCode(CardId.PsychicEndPunisher))
                return Enemy.GetMonsterCount() > 0 ? 25 : 70;
            if (card.IsCode(CardId.AdamancipatorRisenDragite))
                return Bot.Graveyard.Any(c => c.Attribute == (int)CardAttribute.Water) ||
                    materials != null && materials.Any(c =>
                        (c.Data != null ? c.Data.Attribute : c.Attribute) == (int)CardAttribute.Water &&
                        WillSynchroMaterialReachGraveyard(c))
                    ? 30 : 100;
            if (card.IsCode(CardId.EkhajarDescendantDragonOfTheIceBarrier))
            {
                List<ClientCard> enemies = OrderedEnemyCards(Enemy.GetMonsters()
                    .Concat(Enemy.GetSpells()).Concat(Enemy.Graveyard), false, true).ToList();
                return CanPayEkhajarCost(enemies, true) &&
                    enemies.Count >= 2 && enemies.Any(c => EkhajarTargetPriority(c) < 100) ? 40 : 100;
            }
            if (card.IsCode(CardId.AshtrajaSusanoo))
            {
                if (!CanUseSusanooClear(materials))
                    return 100;
                // Decide whether other removal should come first BEFORE
                // summoning. The summon-success trigger cannot be postponed.
                bool canUseFallen = Bot.HasInHand(CardId.TheFallenTheVirtuous) &&
                    !_usedEffects.Contains(Util.GetStringId(CardId.TheFallenTheVirtuous, 0)) &&
                    !DefaultCheckWhetherCardIdIsNegated(CardId.TheFallenTheVirtuous) &&
                    Bot.HasInExtra(new int[] { CardId.AlbionTheBrandedDragon,
                        CardId.EcclesiaAndTheDarkDragon }) &&
                    OrderedFallenTargets(Enemy.GetMonsters().Concat(Enemy.GetSpells())).Any();
                bool canUseKuzuha = Bot.GetMonsters().Any(c =>
                    c.IsCode(CardId.KuzuhaTheHexAshtra) && c.IsFaceup() &&
                    !DefaultCheckWhetherCardIsNegated(c)) &&
                    !_usedEffects.Contains(Util.GetStringId(CardId.KuzuhaTheHexAshtra, 1)) &&
                    Bot.GetMonsters().Concat(Bot.GetSpells()).Concat(Enemy.GetMonsters())
                        .Concat(Enemy.GetSpells()).Count(c => c.IsFacedown()) >= 2 &&
                    OrderedEnemyCards(Enemy.GetMonsters().Concat(Enemy.GetSpells()), true, true).Any();
                return Duel.Player == 0 && (canUseFallen || canUseKuzuha) ? 100 : 50;
            }
            return 100;
        }

        private List<ClientCard> FindSynchroMaterialsFor(ClientCard synchro)
        {
            return FindSynchroMaterials(synchro.Level,
                synchro.IsCode(CardId.AdamancipatorRisenDragite) &&
                !Bot.Graveyard.Any(c => c.Attribute == (int)CardAttribute.Water));
        }

        private List<ClientCard> FindSynchroMaterials(int level, bool requireWater = false,
            ClientCard revivedMonster = null)
        {
            List<ClientCard> monsters = Bot.GetMonsters().Where(c => c.IsFaceup() &&
                c.Level > 0 && !c.IsMonsterNotBeSynchroMaterial()).ToList();
            if (revivedMonster != null && revivedMonster.Level > 0 &&
                !revivedMonster.IsMonsterNotBeSynchroMaterial())
                monsters.Add(revivedMonster);
            List<ClientCard> best = null;
            int bestValue = int.MaxValue;
            // Search visible monsters plus an optional prospective revival;
            // that revival must participate, not merely accompany an existing
            // combination. This also supports levels changed by Yoa.
            for (int mask = 1; mask < (1 << monsters.Count); ++mask)
            {
                List<ClientCard> group = new List<ClientCard>();
                for (int i = 0; i < monsters.Count; ++i)
                    if ((mask & (1 << i)) != 0)
                        group.Add(monsters[i]);
                if (revivedMonster != null && !group.Contains(revivedMonster) ||
                    group.Count < 2 || group.Sum(c => c.Level) != level ||
                    group.Count(c => c.HasType(CardType.Tuner)) != 1)
                    continue;
                // A known banish replacement must not be counted as a future
                // WATER card in the GY. Non-water combinations remain legal
                // when Dragite's requirement is already met by the GY.
                if (requireWater && !group.Any(c =>
                    (c.Data != null ? c.Data.Attribute : c.Attribute) == (int)CardAttribute.Water &&
                    WillSynchroMaterialReachGraveyard(c)))
                    continue;
                if (group.Any(IsProtectedSynchroMaterial) ||
                    group.Any(c => c.IsCode(CardId.AshtrayaMikumariNoMikoto) &&
                        !DefaultCheckWhetherCardIsNegated(c)) &&
                    !monsters.Any(c => c.IsCode(CardId.AshtrayaMikumariNoMikoto) &&
                        !DefaultCheckWhetherCardIsNegated(c) && !group.Contains(c)))
                    continue;
                int value = group.Sum(MaterialPriority);
                if (value < bestValue)
                {
                    bestValue = value;
                    best = group;
                }
            }
            return best;
        }

        private bool WillSynchroMaterialReachGraveyard(ClientCard card)
        {
            // Trap monsters return to their original trap type in the GY.
            // Use that type for redirects such as Dimensional Fissure.
            CardType originalType = (CardType)(card.Data != null ? card.Data.Type : card.Type);
            return (originalType & (CardType.Token | CardType.Pendulum)) == 0 &&
                !DefaultCheckWhetherBotWillBeBanished(originalType, CardLocation.MonsterZone);
        }

        private bool IsProtectedSynchroMaterial(ClientCard card)
        {
            return !DefaultCheckWhetherCardIsNegated(card) &&
                (card.IsCode(CardId.BaronneDeFleur) && !_spentBaronne.Contains(card) ||
                card.IsCode(CardId.AdamancipatorRisenDragite) &&
                    !_usedEffects.Contains(Util.GetStringId(card.Id, 1)) ||
                card.IsCode(CardId.EkhajarDescendantDragonOfTheIceBarrier) &&
                    !_usedEffects.Contains(Util.GetStringId(card.Id, 1)) ||
                card.IsCode(CardId.AshtrayaMikumariNoMikoto) &&
                    Bot.GetMonsters().Count(m => m.IsCode(card.Id) &&
                        !DefaultCheckWhetherCardIsNegated(m)) == 1);
        }

        private int MaterialPriority(ClientCard card)
        {
            if (card.HasType(CardType.Tuner) && card.Level == 1)
                return 0;
            if (card.IsDisabled())
                return 5;
            if (card.IsCode(CardId.MikumariTheBanisherAshtra))
                return 10;
            if (card.IsCode(CardId.YoaThePlagueAshtra))
                return 20;
            if (!card.IsExtraCard())
                return 30;
            return card.IsCode(CardId.AshtrayaMikumariNoMikoto) ? 150 : 200;
        }

        private bool AshtraSetupSpellSet()
        {
            return Card.IsTrap() && !Card.IsCode(CardId.SongsOfTheDominators) && AshtraSpellSet();
        }

        private bool AshtraSpellSet()
        {
            if (!Card.IsTrap() && !Card.HasType(CardType.QuickPlay))
                return false;
            if (Bot.GetSpellCountWithoutField() >= AshtraSetLimit())
                return false;
            return !Duel.MainPhase.SpellSetableCards.Any(c => c != Card &&
                c.IsTrap() && TrapPriority(c) < TrapPriority(Card));
        }

        private int AshtraSetLimit()
        {
            return Bot.HasInGraveyard(CardId.AlbionTheBrandedDragon) ||
                CanUseLilithSearch() && (Bot.HasInHand(CardId.LilithLadyOfLament) ||
                    Bot.HasInMonstersZone(CardId.LilithLadyOfLament)) ||
                Bot.GetMonsters().Any(c => c.IsCode(CardId.YoaThePlagueAshtra,
                    CardId.AkasunaTheBattleAshtra)) ? 4 : 5;
        }

        private bool IsAshtraMonster(ClientCard card)
        {
            return card.IsMonster() && card.HasSetcode(0x1e4);
        }

        private bool IsAshtraTrap(ClientCard card)
        {
            return card.IsCode(CardId.AshtrasDomain, CardId.AshtrasWeb, CardId.AshtrasHowl);
        }

        private int TrapPriority(ClientCard card)
        {
            int priority = card.IsCode(CardId.MindMirrorForce) ? 0 :
                card.IsCode(CardId.SolemnAccusation) ? 1 :
                card.IsCode(CardId.AshtrasDomain) ? 2 :
                card.IsCode(CardId.AshtrasWeb) ? 3 : card.IsCode(CardId.AshtrasHowl) ? 4 :
                card.IsCode(CardId.DominusImpulse) ? 5 :
                card.IsCode(CardId.InfiniteImpermanence) ? 6 : 7;
            if (Bot.GetSpells().Any(c => c.IsCode(card.Id)))
                priority += 20;
            return priority;
        }

        private int SearchPriority(ClientCard card)
        {
            int priority = card.IsCode(CardId.MikumariTheBanisherAshtra) ? 0 :
                card.IsCode(CardId.YoaThePlagueAshtra) ? 1 :
                card.IsCode(CardId.AkasunaTheBattleAshtra) ? 2 :
                card.IsCode(CardId.KuzuhaTheHexAshtra) ? 3 : TrapPriority(card) + 4;
            if (Bot.HasInHand(card.Id) || Bot.GetMonsters().Any(c => c.IsCode(card.Id)))
                priority += 30;
            if (_usedEffects.Contains(Util.GetStringId(card.Id, 1)))
                priority += 20;
            return priority;
        }

        private bool CanTarget(ClientCard card, bool monsterEffect)
        {
            return !card.IsShouldNotBeTarget() && !(monsterEffect ?
                card.IsShouldNotBeMonsterTarget() : card.IsShouldNotBeSpellTrapTarget());
        }

        private bool IsRemovalReserved(ClientCard card)
        {
            return _reservedRemoval.Any(entry =>
                !Duel.NegatedChainIndexList.Contains(entry.Key) && entry.Value.Contains(card));
        }

        private bool IsRemovalReservedOnAllEnemies()
        {
            return !Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(c => !IsRemovalReserved(c));
        }

        private int ThreatPriority(ClientCard card)
        {
            if (card.IsFaceup() && card.IsFloodgate())
                return 0;
            if (Duel.CurrentChainInfo.Any(i => i.ActivatePlayer == 1 && i.RelatedCard == card &&
                i.ActivateLocation == CardLocation.MonsterZone) && !card.IsDisabled())
                return 1;
            if (card.IsMonsterDangerous())
                return 2;
            if (card.IsExtraCard() && (!card.HasType(CardType.Link) || card.LinkCount > 1))
                return 3;
            if (card.IsFacedown())
                return 4;
            if (card.IsMonster())
                return 5;
            return card.HasType(CardType.Continuous) || card.HasType(CardType.Field) ? 3 : 8;
        }

        private IEnumerable<ClientCard> OrderedEnemyCards(IEnumerable<ClientCard> cards,
            bool targeting, bool monsterEffect)
        {
            return cards.Where(c => c.Controller == 1 && !IsRemovalReserved(c) &&
                (!targeting || CanTarget(c, monsterEffect)))
                .OrderBy(ThreatPriority).ThenByDescending(c => c.GetDefensePower());
        }

        private bool IsWorthDestroying(ClientCard card)
        {
            if (card == null || card.Controller != 1 || !card.IsFaceup())
                return false;
            if (card.IsMonster())
                return (card.IsExtraCard() && (!card.HasType(CardType.Link) || card.LinkCount >= 2)) ||
                    card.IsFloodgate() || card.IsMonsterDangerous() ||
                    card.IsMonsterShouldBeDisabledBeforeItUseEffect() ||
                    (card.Attack + card.Defense >= 2700 &&
                        (card.Attack >= 1800 || card.Defense >= 2100));
            return card.HasType(CardType.Continuous | CardType.Field);
        }

        private IEnumerable<ClientCard> OrderedFallenTargets(IEnumerable<ClientCard> cards)
        {
            return cards.Where(c => IsWorthDestroying(c) && !IsRemovalReserved(c) &&
                CanTarget(c, false)).Distinct().OrderByDescending(FallenTargetPriority)
                .ThenByDescending(c => Math.Max(c.Attack, c.Defense));
        }

        private int FallenTargetPriority(ClientCard card)
        {
            if (!card.IsMonster())
                return card.IsFloodgate() ? 1000 : card.HasType(CardType.Field) ? 850 : 800;

            int priority = card.IsFloodgate() ? 1000 : card.IsMonsterDangerous() ? 950 :
                card.IsMonsterShouldBeDisabledBeforeItUseEffect() ? 900 :
                card.IsExtraCard() && (!card.HasType(CardType.Link) || card.LinkCount >= 2) ? 750 : 500;
            if (card.IsDisabled())
                priority -= 350;
            // Requiem can tribute itself in response. It remains a fallback,
            // but is less reliable than a target that cannot publicly escape.
            if (card.IsCode(CardId.FiendsmithsRequiem) && !card.IsDisabled())
                priority -= 450;
            return priority;
        }

        private IList<ClientCard> TakeSelection(IEnumerable<ClientCard> ordered,
            int min, int max, int desired)
        {
            List<ClientCard> candidates = ordered.Distinct().ToList();
            if (candidates.Count < min)
                return null;
            return candidates.Take(Math.Min(max, Math.Max(min, desired))).ToList();
        }

        private IList<ClientCard> SelectEnemy(IList<ClientCard> cards, int min, int max,
            bool targeting, bool monsterEffect, bool reserve)
        {
            IList<ClientCard> selected = TakeSelection(OrderedEnemyCards(cards, targeting,
                monsterEffect), min, max, max);
            if (selected != null && reserve)
                _reservedRemoval[Duel.CurrentChain.Count] = selected.ToList();
            return selected;
        }

        public override IList<ClientCard> OnSelectTribute(IList<ClientCard> cards,
            int min, int max, int hint, bool cancelable)
        {
            ClientCard activating = Duel.GetCurrentChainCard();
            if (activating != null && activating.Controller == 0 &&
                activating.IsCode(CardId.LilithLadyOfLament))
                return TakeSelection(cards.OrderBy(LilithTributePriority)
                    .ThenBy(MaterialPriority), min, max, min);
            if (_pendingSummonId != CardId.TheGreatGallantBandit)
                return base.OnSelectTribute(cards, min, max, hint, cancelable);
            // The actual tribute prompt, not AI.SelectMaterials, controls the
            // Gallant Thief's alternative normal summon. Own hand monsters are
            // allowed only when the opposing tributes do not meet the minimum.
            IList<ClientCard> selected = TakeSelection(cards.OrderBy(c => c.Controller == 1 ? 0 : 1)
                .ThenBy(c => c.Controller == 1 ? ThreatPriority(c) : MaterialPriority(c)),
                min, max, min);
            _pendingSummonId = 0;
            return selected ?? (cancelable ? new List<ClientCard>() : null);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards,
            int min, int max, int hint, bool cancelable)
        {
            if (_chengyingReplacementPending)
            {
                _chengyingReplacementPending = false;
                // Consume only the immediate replacement prompt, before
                // interpreting the (possibly opposing) resolving chain source.
                if (hint == HintMsg.Remove && min == 1 && max == 1 && cards.Count > 0 &&
                    cards.All(c => c.Controller == 0 && c.Location == CardLocation.Grave))
                    return TakeSelection(cards.OrderBy(c =>
                        IsAshtraMonster(c) || c.HasType(CardType.Synchro) ||
                        c.IsCode(CardId.MindMirrorForce, CardId.SolemnAccusation) ? 2 :
                        IsAshtraTrap(c) ? 1 : 0), min, max, 1);
            }
            ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
            ClientCard activating = Duel.GetCurrentChainCard();
            if (solving == null && activating == null)
            {
                if (hint == HintMsg.ReturnToHand && _pendingSummonId != 0)
                    return TakeSelection(cards.Where(c => c.Controller == 0 && c.IsFacedown())
                        .OrderByDescending(TrapPriority), min, max, min);
                if (hint == HintMsg.Release && _pendingSummonId == CardId.RedEyesBlackDragonExceed)
                    return TakeSelection(cards.OrderBy(c => c.Controller == 1 ? 0 : 1)
                        .ThenBy(c => c.Controller == 1 ? ThreatPriority(c) :
                            _usedEffects.Contains(Util.GetStringId(c.Id, 1)) ? 0 : 1)
                        .ThenBy(MaterialPriority), min, max, min);
                return base.OnSelectCard(cards, min, max, hint, cancelable);
            }
            int id = solving != null ? solving.ActivateId : activating.Id;
            int player = solving != null ? solving.ActivatePlayer : activating.Controller;
            if (player != 0)
                return base.OnSelectCard(cards, min, max, hint, cancelable);

            if (id == CardId.LilithLadyOfLament)
            {
                if (solving == null && hint == HintMsg.Release)
                    return TakeSelection(cards.OrderBy(LilithTributePriority)
                        .ThenBy(MaterialPriority), min, max, min);
                if (solving != null && hint == HintMsg.Set && cards.All(c =>
                    c.Controller == 0 && c.Location == CardLocation.Deck &&
                    c.Type == (int)CardType.Trap))
                    // These are the three REVEALED traps, not a chosen Set card.
                    // Multiple copies of the best-ranked trap are valid choices.
                    return TakeSelection(cards.OrderBy(TrapPriority), min, max, 3);
            }

            if (hint == HintMsg.SynchroMaterial)
                return null;
            if (hint == HintMsg.SpSummon && cards.All(c => c.Location == CardLocation.Extra) &&
                (id == CardId.AshtrasDomain || id == CardId.AshtrasWeb || id == CardId.AshtrasHowl))
            {
                ClientCard synchro = cards.OrderBy(SynchroPriority).FirstOrDefault();
                if (synchro == null)
                    return null;
                List<ClientCard> materials = FindSynchroMaterialsFor(synchro);
                if (materials != null)
                    AI.SelectMaterials(materials, HintMsg.SynchroMaterial);
                return TakeSelection(new ClientCard[] { synchro }, min, max, 1);
            }

            if (id == CardId.TheFallenTheVirtuous)
            {
                if (hint == HintMsg.ToGrave)
                    return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.AlbionTheBrandedDragon) ?
                        0 : c.IsCode(CardId.EcclesiaAndTheDarkDragon) ? 1 : 2), min, max, 1);
                if (hint == HintMsg.Destroy || hint == HintMsg.Target)
                {
                    IEnumerable<ClientCard> ordered = OrderedFallenTargets(cards);
                    IEnumerable<ClientCard> fallback = OrderedEnemyCards(cards, true, false)
                        .Where(c => c.IsFaceup()).OrderByDescending(FallenTargetPriority);
                    IList<ClientCard> selected = TakeSelection(ordered.Concat(fallback), min, max, 1);
                    if (selected != null && solving == null)
                        _reservedRemoval[Duel.CurrentChain.Count] = selected.ToList();
                    return selected;
                }
            }
            if (id == CardId.EcclesiaAndTheDarkDragon && hint == HintMsg.ToDeck)
            {
                if (cards.All(c => c.Location == CardLocation.Grave || c.Location == CardLocation.Removed))
                    return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.AlbionTheBrandedDragon) ?
                        0 : 1), min, max, 1);
                return SelectEnemy(cards, min, max, true, true, true);
            }
            if (id == CardId.SuperStarslayerTYPHONSkyCrisis)
            {
                if (hint == HintMsg.ReturnToHand)
                    return SelectEnemy(cards, min, max, false, true, false);
                if (hint == HintMsg.RemoveXyz || cards.All(c => c.Location == CardLocation.Overlay))
                    return TakeSelection(cards, min, max, 1);
            }
            if (id == CardId.AlbionTheBrandedDragon)
                return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.TheFallenTheVirtuous) ?
                    0 : 1), min, max, 1);
            if (id == CardId.MikumariTheBanisherAshtra && hint == HintMsg.AddToHand)
                return TakeSelection(cards.OrderBy(SearchPriority), min, max, 1);
            if ((id == CardId.MikumariTheBanisherAshtra || id == CardId.YoaThePlagueAshtra ||
                id == CardId.AkasunaTheBattleAshtra || id == CardId.KuzuhaTheHexAshtra ||
                id == CardId.AshtrayaMikumariNoMikoto) && hint == HintMsg.Set)
                return TakeSelection(cards.OrderBy(c => id == CardId.AshtrayaMikumariNoMikoto ?
                    c.IsCode(CardId.MindMirrorForce) ? 0 :
                    c.IsCode(CardId.SolemnAccusation) ? 1 : 2 : 0)
                    .ThenBy(TrapPriority), min, max, 1);
            if (id == CardId.AkasunaTheBattleAshtra && hint == HintMsg.ToGrave)
                return TakeSelection(cards.OrderBy(AkasunaCostPriority)
                    .ThenBy(c => c.Location == CardLocation.Hand ? 0 : 1)
                    .ThenByDescending(TrapPriority), min, max, 1);
            if (id == CardId.KuzuhaTheHexAshtra && hint == HintMsg.Destroy)
                return SelectEnemy(cards, min, max, true, true, true);
            if (id == CardId.YoaThePlagueAshtra && hint == HintMsg.Faceup)
                return SelectEnemy(cards, min, max, true, true, false);

            if (id == CardId.AshtrashenGateToTheWorldsBeyond)
            {
                if (hint == HintMsg.ReturnToHand)
                    return SelectEnemy(cards, min, max, true, false, true);
                if (hint == HintMsg.ToGrave)
                {
                    int chainIndex = Duel.CurrentChain.Count;
                    // SelectTarget may issue repeated SelectUnselect prompts.
                    // Finish after one, rather than spending the retained set.
                    if (min == 0 && _gateTargetSelected.Contains(chainIndex))
                        return new List<ClientCard>();
                    IList<ClientCard> selected = TakeSelection(cards.Where(c =>
                        CanPreserveGateSetCard(c))
                        .OrderBy(c => c.IsCode(CardId.FoolishGraverobber) ? 0 : 1)
                        .ThenByDescending(TrapPriority), min, max, 1);
                    if (selected == null)
                    {
                        if (cancelable)
                            return new List<ClientCard>();
                        // Once the server requires a mandatory response, obey
                        // its legal candidates even if an earlier safeguard
                        // became impossible. Do not return an invalid count.
                        selected = TakeSelection(cards.Where(c => c.Controller == 0 &&
                            c.IsFacedown()).OrderByDescending(TrapPriority), min, max, 1);
                    }
                    if (selected != null && selected.Count > 0)
                        _gateTargetSelected.Add(chainIndex);
                    return selected;
                }
                if (hint == HintMsg.AddToHand)
                {
                    // SelectSubGroup requires different names. Already selected
                    // names are tracked in the engine's unselectable group.
                    return TakeSelection(cards.OrderBy(SearchPriority).GroupBy(c => c.Id)
                        .Select(g => g.First()), min, max, min);
                }
            }
            if (id == CardId.AshtrayaMikumariNoMikoto && hint == HintMsg.ToGrave)
            {
                if (cards.All(c => c.Location == CardLocation.Deck))
                    return TakeSelection(cards.OrderBy(c => IsAshtraTrap(c) ? 0 : 1)
                        .ThenBy(TrapPriority), min, max, 1);
                // Even if every enemy is already reserved, an enemy is still
                // better than our own card for this mandatory field selection.
                IList<ClientCard> enemy = TakeSelection(cards.Where(c => c.Controller == 1)
                    .OrderBy(c => IsRemovalReserved(c) ? 1 : 0).ThenBy(ThreatPriority)
                    .ThenByDescending(c => c.GetDefensePower()), min, max, 1);
                if (enemy != null)
                    return enemy;
                return TakeSelection(cards.OrderBy(c => c.IsTrap() && c.IsFacedown() ? 0 :
                    c.IsCode(CardId.AshtrayaMikumariNoMikoto) ? 3 : 2)
                    .ThenByDescending(TrapPriority), min, max, 1);
            }
            if (id == CardId.AshtrajaSusanoo)
            {
                if (hint == HintMsg.Remove && solving == null)
                {
                    if (_susanooCostBudget == 0)
                        _susanooCostBudget = Math.Max(1, Math.Min(
                            Enemy.GetMonsters().Concat(Enemy.GetSpells()).Count(c => !IsRemovalReserved(c)),
                            Bot.Graveyard.Count(c => c.IsTrap())));
                    // SelectMatchingCard can emit repeated SelectUnselect
                    // prompts. Finish once the chosen cost reaches the number
                    // of useful enemies, rather than banishing the whole Graveyard.
                    if (min == 0 && _susanooCostCount >= _susanooCostBudget)
                        return new List<ClientCard>();
                    IList<ClientCard> cost = TakeSelection(cards.OrderBy(c =>
                        c.IsCode(CardId.MindMirrorForce, CardId.SolemnAccusation) ? 100 :
                        IsAshtraTrap(c) ? 20 : 0), min, max,
                        Math.Max(1, _susanooCostBudget - _susanooCostCount));
                    if (cost != null)
                        _susanooCostCount += cost.Count;
                    return cost;
                }
                if (hint == HintMsg.Remove)
                {
                    if (min == 0 && _susanooRemovedCount >= _susanooCostCount)
                        return new List<ClientCard>();
                    // Pay for opponents, not our own cards. If they leave after
                    // payment, a mandatory exact-count prompt still needs a
                    // legal least-damaging fallback; we cannot undo the cost.
                    IList<ClientCard> selected = TakeSelection(OrderedEnemyCards(cards, false, true)
                        .Concat(cards.Where(c => c.Controller == 1).OrderBy(ThreatPriority))
                        .Concat(cards.Where(c => c.Controller == 0).OrderBy(MaterialPriority)),
                        min, max, Math.Max(min, _susanooCostCount - _susanooRemovedCount));
                    if (selected != null)
                        _susanooRemovedCount += selected.Count;
                    return selected;
                }
                int recoveryIndex = solving != null ? Duel.SolvingChainIndex : Duel.CurrentChain.Count;
                if (hint == HintMsg.OperateCard)
                {
                    List<ClientCard> previous;
                    if (!_susanooRecoveryTargets.TryGetValue(recoveryIndex, out previous))
                    {
                        previous = new List<ClientCard>();
                        _susanooRecoveryTargets[recoveryIndex] = previous;
                    }
                    int desired = CanReleaseSusanooForRecovery(solving != null ?
                        solving.RelatedCard : activating) ? 2 : 1;
                    if (min == 0 && previous.Count >= desired)
                        return new List<ClientCard>();
                    IList<ClientCard> selected = TakeSelection(cards.Where(c => !previous.Contains(c))
                        .OrderBy(SusanooRecoveryPriority), min, max, Math.Max(0, desired - previous.Count));
                    if (selected != null)
                        previous.AddRange(selected);
                    return selected;
                }
                if (hint == HintMsg.Set)
                {
                    List<ClientCard> previous;
                    if (!_susanooRecoverySets.TryGetValue(recoveryIndex, out previous))
                    {
                        previous = new List<ClientCard>();
                        _susanooRecoverySets[recoveryIndex] = previous;
                    }
                    // Return monsters to hand rather than setting them without
                    // their summon triggers, or giving Maxx C a needless draw.
                    int free = Math.Max(0, 5 - Bot.GetSpellCountWithoutField() - previous.Count);
                    IList<ClientCard> selected = TakeSelection(cards.Where(c => c.IsTrap() &&
                        !previous.Contains(c)).OrderBy(SusanooRecoveryPriority), min, max, Math.Min(free, max));
                    if (free == 0 && min == 0)
                        return new List<ClientCard>();
                    if (selected != null)
                        previous.AddRange(selected);
                    return selected ?? (min == 0 ? new List<ClientCard>() : null);
                }
            }
            if (id == CardId.DarkTimeWizard && hint == HintMsg.AddToHand)
                return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.FoolishGraverobber) ?
                    0 : 1), min, max, 1);
            if (id == CardId.FoolishGraverobber)
            {
                if (hint == HintMsg.ToGrave)
                    return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.DarkTimeWizard) ?
                        0 : 1), min, max, 1);
                if (hint == HintMsg.Set)
                    return TakeSelection(cards.OrderBy(GraverobberSetPriority), min, max, 1);
                if (hint == HintMsg.SpSummon)
                    return TakeSelection(cards.OrderBy(c => GraverobberRevivePriority(c, true))
                        .ThenBy(SearchPriority).ThenByDescending(c => c.Attack), min, max, 1);
            }
            if (id == CardId.EkhajarDescendantDragonOfTheIceBarrier)
            {
                if (hint == HintMsg.Discard || hint == HintMsg.Remove && solving == null)
                    return TakeSelection(cards.OrderBy(c => EkhajarCostPriority(c, hint == HintMsg.Discard))
                        .ThenByDescending(TrapPriority), min, max, 1);
                if (hint == HintMsg.Remove || hint == HintMsg.ReturnToHand)
                {
                    IEnumerable<ClientCard> preferred = hint == HintMsg.ReturnToHand ?
                        cards.Where(c => EkhajarTargetPriority(c) < 100).OrderBy(EkhajarTargetPriority) :
                        OrderedEnemyCards(cards, false, true).OrderBy(EkhajarTargetPriority);
                    IList<ClientCard> selected = TakeSelection(preferred.Concat(cards.Where(c =>
                        c.Controller == 1).OrderBy(ThreatPriority)), min, max, min);
                    if (selected != null)
                        _reservedRemoval[Duel.SolvingChainIndex] = selected.Where(c => c.Controller == 1).ToList();
                    return selected;
                }
            }
            if (id == CardId.SwordsoulSupremeSovereignChengying && hint == HintMsg.Remove)
            {
                if (cards.All(c => c.Controller == 0))
                    return TakeSelection(cards.OrderBy(c => IsAshtraTrap(c) ? 1 : 0), min, max, 1);
                return SelectEnemy(cards, min, max, false, true, false);
            }
            if (id == CardId.PsychicEndPunisher && hint == HintMsg.Remove)
            {
                if (cards.All(c => c.Controller == 0))
                    return TakeSelection(cards.OrderBy(MaterialPriority), min, max, 1);
                return SelectEnemy(cards, min, max, true, true, true);
            }
            if (id == CardId.BaronneDeFleur && hint == HintMsg.Destroy)
                return SelectEnemy(cards, min, max, true, true, true);
            if (id == CardId.DominusSpiral && hint == HintMsg.ReturnToHand)
                return SelectEnemy(cards.Where(IsSpiralTargetAllowed).ToList(), min, max, true, false, true);
            if (id == CardId.AshtrasHowl && hint == HintMsg.Destroy)
            {
                // Reapply the value filter at the actual target prompt. If an
                // already-accepted activation now forces a selection, obey its
                // minimum rather than returning an illegal empty response.
                IList<ClientCard> selected = TakeSelection(
                    OrderedEnemyCards(cards, true, false).OrderBy(c => IsWorthHowlDestroying(c) ? 0 : 1),
                    min, max, 1);
                if (selected != null)
                    _reservedRemoval[Duel.CurrentChain.Count] = selected.ToList();
                return selected;
            }
            if (id == CardId.InfiniteImpermanence)
            {
                // Prefer the immediately preceding field effect, not another
                // monster with a higher ATK elsewhere on the board.
                ChainInfo previous = Duel.CurrentChainInfo.Count > 1 ?
                    Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 2] : null;
                ClientCard target = previous != null ? previous.RelatedCard : null;
                IList<ClientCard> selected = TakeSelection(OrderedEnemyCards(cards.Where(c =>
                    !c.IsDisabled()), true, false).OrderBy(c => c == target ? 0 : 1),
                    min, max, 1);
                if (selected != null)
                    _reservedRemoval[Duel.CurrentChain.Count] = selected.ToList();
                return selected;
            }
            if (id == CardId.SolemnAccusation && hint == HintMsg.Confirm)
                return TakeSelection(cards.OrderByDescending(TrapPriority), min, max, 1);
            if (hint == HintMsg.SpSummon && (id == CardId.AshtrasWeb ||
                id == CardId.RedEyesBlackDragonExceed ||
                id == CardId.BaronneDeFleur))
                return TakeSelection(cards.OrderBy(c => IsAshtraMonster(c) ? 0 : 1)
                    .ThenBy(SearchPriority).ThenByDescending(c => c.Attack), min, max, 1);
            if (id == CardId.SongsOfTheDominators && hint == HintMsg.AddToHand)
                return TakeSelection(cards.OrderBy(c => c.IsCode(CardId.DominusImpulse) ?
                    0 : 1), min, max, 1);
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectOption(IList<int> options)
        {
            if (options.Contains(Util.GetStringId(CardId.AshtrasHowl, 1)) ||
                options.Contains(Util.GetStringId(CardId.AshtrasHowl, 2)))
            {
                ChainInfo resolvingHowl = Duel.GetCurrentSolvingChainInfo();
                int chainIndex = resolvingHowl != null ? Duel.SolvingChainIndex :
                    _pendingHowlChainIndex != 0 ? _pendingHowlChainIndex : Duel.CurrentChain.Count;
                int preferredHowl;
                if (!_plannedTrapOptions.TryGetValue(chainIndex, out preferredHowl))
                    preferredHowl = Enemy.GetMonsters().Any(IsWorthHowlDestroying) ? 2 : 1;
                int selectedHowl = options.IndexOf(Util.GetStringId(CardId.AshtrasHowl, preferredHowl));
                if (selectedHowl < 0)
                    selectedHowl = 0;
                _plannedTrapOptions[chainIndex] = options[selectedHowl] ==
                    Util.GetStringId(CardId.AshtrasHowl, 2) ? 2 : 1;
                return selectedHowl;
            }
            // The script chooses its activation branch before MSG_CHAINING may
            // arrive. Its unique option descriptions identify this prompt even
            // when GetCurrentChainCard still refers to our starter (or is null).
            if (options.Contains(Util.GetStringId(CardId.DarkTimeWizard, 1)) ||
                options.Contains(Util.GetStringId(CardId.DarkTimeWizard, 2)))
            {
                ChainInfo resolvingWizard = Duel.GetCurrentSolvingChainInfo();
                int chainIndex = resolvingWizard != null ? Duel.SolvingChainIndex :
                    _pendingTimeWizardChainIndex != 0 ? _pendingTimeWizardChainIndex : Duel.CurrentChain.Count;
                int option;
                if (!_plannedTimeWizardOptions.TryGetValue(chainIndex, out option))
                    option = !_timeSearchUsed && Bot.HasInDeck(CardId.FoolishGraverobber) &&
                        !Bot.HasInHand(CardId.FoolishGraverobber) ? 1 : 2;
                int selectedOption = options.IndexOf(Util.GetStringId(CardId.DarkTimeWizard, option));
                if (selectedOption < 0)
                    selectedOption = 0;
                option = options[selectedOption] == Util.GetStringId(CardId.DarkTimeWizard, 1) ? 1 : 2;
                _plannedTimeWizardOptions[chainIndex] = option;
                if (option == 1)
                    _timeSearchUsed = true;
                else
                    _timeCoinUsed = true;
                return selectedOption;
            }
            // These are script-internal options, not separate activation descs.
            // Use the newest activation while building a chain; otherwise use
            // the exact resolving snapshot, never the last unresolved card.
            ChainInfo solving = Duel.GetCurrentSolvingChainInfo();
            ClientCard activating = Duel.GetCurrentChainCard();
            int id = solving != null ? solving.ActivateId : activating != null ? activating.Id : 0;
            int player = solving != null ? solving.ActivatePlayer : activating != null ? activating.Controller : -1;
            if (player != 0)
                return base.OnSelectOption(options);
            int preferred = -1;
            if (id == CardId.TheFallenTheVirtuous || id == CardId.SolemnAccusation)
                preferred = 1;
            else if (id == CardId.YoaThePlagueAshtra)
                preferred = 4;
            else if (id == CardId.AshtrasDomain)
            {
                // Our trap has already been appended when its option is
                // selected. Negate only the preceding opponent Spell/Trap link.
                ChainInfo previous = Duel.CurrentChainInfo.Count > 1 ?
                    Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 2] : null;
                preferred = CanUseDomainNegate(previous) ? 2 : 1;
            }
            else if (id == CardId.AshtrasWeb)
            {
                int chainIndex = solving != null ? Duel.SolvingChainIndex : Duel.CurrentChain.Count;
                if (!CanUseWebRevival())
                    preferred = 1;
                else if (!_plannedTrapOptions.TryGetValue(chainIndex, out preferred))
                    preferred = (Bot.GetMonsterCount() == 0 || CanRespondToOpponent() ||
                        Duel.Phase == DuelPhase.End ||
                        !CanUseTrapTuner(activating ?? (solving != null ? solving.RelatedCard : null))) ? 2 : 1;
            }

            int selected = preferred < 0 ? -1 : options.IndexOf(Util.GetStringId(id, preferred));
            if (selected < 0 && preferred >= 0 && options.Count > 0)
                selected = 0; // Only the server's offered branches are legal.
            if (selected >= 0)
            {
                if (id == CardId.AshtrasDomain)
                    _plannedTrapOptions[solving != null ? Duel.SolvingChainIndex : Duel.CurrentChain.Count] =
                        options[selected] == Util.GetStringId(id, 2) ? 2 : 1;
                return selected;
            }
            // Albion and Susanoo use shared system descriptions for add/set.
            if (id == CardId.AshtrajaSusanoo)
            {
                List<ClientCard> targets;
                int chainIndex = solving != null ? Duel.SolvingChainIndex : Duel.CurrentChain.Count;
                bool recoverMonster = _susanooRecoveryTargets.TryGetValue(chainIndex, out targets) &&
                    targets.Count == 1 && targets[0].IsMonster();
                int option = !recoverMonster && Bot.GetSpellCountWithoutField() < 5 ? 1153 : 1190;
                if (options.Contains(option))
                    return options.IndexOf(option);
            }
            if (id == CardId.AlbionTheBrandedDragon &&
                Bot.GetSpellCountWithoutField() < 5 && options.Contains(1153))
                return options.IndexOf(1153);
            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.EkhajarDescendantDragonOfTheIceBarrier, 3))
            {
                // The hand branch is RANDOM, never inspect hidden hand IDs.
                // Choose visible threats first; otherwise take the random card.
                return Enemy.Hand.Count > 0 && !Enemy.GetMonsters().Concat(Enemy.GetSpells())
                    .Concat(Enemy.Graveyard).Any(c => EkhajarTargetPriority(c) < 100);
            }
            if (desc == Util.GetStringId(CardId.AshtrasDomain, 3) ||
                desc == Util.GetStringId(CardId.AshtrasWeb, 3) ||
                desc == Util.GetStringId(CardId.AshtrasHowl, 3))
                return !StopExtraSummons() && Bot.ExtraDeck.Any(c =>
                    c.HasType(CardType.Synchro) && SynchroPriority(c) < 100 &&
                    FindSynchroMaterialsFor(c) != null);
            if (desc == Util.GetStringId(CardId.FoolishGraverobber, 2))
                return !StopExtraSummons() && Bot.GetMonstersInMainZone().Count < 5 &&
                    Bot.Graveyard.Concat(Enemy.Graveyard).Any(c => GraverobberRevivePriority(c) < 100);
            return base.OnSelectYesNo(desc);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if ((available & (1 << 5)) != 0)
                    return 1 << 5;
                if ((available & (1 << 6)) != 0)
                    return 1 << 6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            CardPosition preferred = cardId == CardId.AshtrasDomain || cardId == CardId.AshtrasWeb ||
                cardId == CardId.AshtrasHowl ? CardPosition.FaceUpDefence : CardPosition.FaceUpAttack;
            return positions.Contains(preferred) ? preferred : base.OnSelectPosition(cardId, positions);
        }
    }
}
