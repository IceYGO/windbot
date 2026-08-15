using System.Linq;
using System.Collections.Generic;
using System.Threading;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class GameAI
    {
        public GameClient Game { get; private set; }
        public Duel Duel { get; private set; }
        public Executor Executor { get; set; }

        private Dialogs _dialogs;

        // record activated count to prevent infinite actions
        private Dictionary<int, int> _activatedCards;

        private bool _selectingPendulumSummon;

        private ClientCard _pendingAttacker;
        private ClientCard _pendingAttackTarget;
        private ISet<ClientCard> _attackersWithInvalidPreselectedTarget = new HashSet<ClientCard>();

        public GameAI(GameClient game, Duel duel)
        {
            Game = game;
            Duel = duel;

            _dialogs = new Dialogs(game);
            _activatedCards = new Dictionary<int, int>();
        }

        private void CheckSurrender()
        {
            foreach (CardExecutor exec in Executor.Executors)
            {
                if (exec.Type == ExecutorType.Surrender && exec.Func())
                {
                    _dialogs.SendSurrender();
                    Game.Surrender();
                }
            }
        }

        /// <summary>
        /// Called when the AI got the error message.
        /// </summary>
        public void OnRetry()
        {
            _dialogs.SendSorry();
        }

        public void OnDeckError(string card)
        {
            _dialogs.SendDeckSorry(card);
            Thread.Sleep(1000);
            _dialogs.SendSurrender();
            Game.Connection.Close();
        }

        /// <summary>
        /// Called when the AI join the game.
        /// </summary>
        public void OnJoinGame()
        {
            _dialogs.SendWelcome();
        }

        /// <summary>
        /// Called when the duel starts.
        /// </summary>
        public void OnStart()
        {
            _dialogs.SendDuelStart();
        }

        /// <summary>
        /// Customized called when the AI do something in a duel.
        /// </summary>
        public void SendCustomChat(int index, params object[] opts)
        {
            _dialogs.SendCustomChat(index, opts);
        }

        /// <summary>
        /// Called when the AI do the rock-paper-scissors.
        /// </summary>
        /// <returns>1 for Scissors, 2 for Rock, 3 for Paper.</returns>
        public int OnRockPaperScissors()
        {
            return Executor.OnRockPaperScissors();
        }

        /// <summary>
        /// Called when the AI won the rock-paper-scissors.
        /// </summary>
        /// <returns>True if the AI should begin first, false otherwise.</returns>
        public bool OnSelectHand()
        {
            return Executor.OnSelectHand();
        }

        /// <summary>
        /// Called when any player draw card.
        /// </summary>
        public void OnDraw(int player)
        {
            Executor.OnDraw(player);
        }

        /// <summary>
        /// Called when it's a new turn.
        /// </summary>
        public void OnNewTurn()
        {
            _activatedCards.Clear();
            Executor.OnNewTurn();
        }

        /// <summary>
        /// Called when it's a new phase.
        /// </summary>
        public void OnNewPhase()
        {
            _pendingAttacker = null;
            _pendingAttackTarget = null;
            _attackersWithInvalidPreselectedTarget.Clear();
            ClearSelections();
            _selectingPendulumSummon = false;
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Draw)
            {
                _dialogs.SendNewTurn();
            }
            Executor.OnNewPhase();
            CheckSurrender();
        }

        public void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            Executor.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        /// <summary>
        /// Called when an attack has been declared and its battling monsters have been recorded.
        /// </summary>
        public void OnAttack()
        {
            _pendingAttacker = null;
            _pendingAttackTarget = null;
            _attackersWithInvalidPreselectedTarget.Clear();
        }

        /// <summary>
        /// Called when the AI got attack directly.
        /// </summary>
        public void OnDirectAttack(ClientCard card)
        {
            _dialogs.SendOnDirectAttack(card.Name);
            CheckSurrender();
        }

        /// <summary>
        /// Called when a chain is executed.
        /// </summary>
        /// <param name="card">Card who is chained.</param>
        /// <param name="player">Player who is currently chaining.</param>
        public void OnChaining(ClientCard card, int player)
        {
            Executor.OnChaining(player,card);
        }

        public void OnSummoning()
        {
            Executor.OnSummoning();
        }

        public void OnChainSolved(int chainIndex)
        {
            Executor.OnChainSolved(chainIndex);
        }

        /// <summary>
        /// Called when card is successfully special summoned.
        /// Used on monsters that can only special summoned once per turn.
        /// </summary>
        public void OnSpSummoned()
        {
            Executor.OnSpSummoned();
        }

        public void OnSpSummoning()
        {
            _selectingPendulumSummon = false;
            Executor.OnSpSummoning();
        }
        
        /// <summary>
        /// Called when a chain has been solved.
        /// </summary>
        public void OnChainEnd()
        {
            ClearSelections();
            Executor.OnChainEnd();
            CheckSurrender();
        }

        private void ClearSelections()
        {
            m_selector.Clear();
            m_position.Clear();
            m_selector_pointer = -1;
            m_materialSelector = null;
            m_materialSelectorHint = 0;
            m_place = 0;
            m_option = -1;
            m_number = -1;
            m_announce = 0;
            m_yesno = -1;
            m_attributes.Clear();
            m_races.Clear();
        }

        /// <summary>
        /// Called when a PlayerHint message is received (e.g. effect description add/remove hints).
        /// </summary>
        /// <param name="player">Player index</param>
        /// <param name="hintType">Hint type, see PlayerHintType (DescAdd=6, DescRemove=7)</param>
        /// <param name="description">Effect description id (peffect->description)</param>
        public void OnPlayerHint(int player, int hintType, int description)
        {
            Executor.OnPlayerHint(player, hintType, description);
        }

        /// <summary>
        /// Called when a zone hint is received.
        /// </summary>
        /// <param name="player">Player index.</param>
        /// <param name="zone">Zone data (hinted zones, bit field).</param>
        public void OnHintZone(int player, int zone)
        {
            Executor.OnHintZone(player, zone);
        }

        /// <summary>
        /// Called when receiving annouce
        /// </summary>
        /// <param name="player">Player who announce.</param>
        /// <param name="data">Annouced info.</param>
        public void OnReceivingAnnouce(int player, int data)
        {
            Executor.OnReceivingAnnouce(player, data);
        }

        /// <summary>
        /// Called when the AI has to do something during the battle phase.
        /// </summary>
        /// <param name="battle">Informations about usable cards.</param>
        /// <returns>A new BattlePhaseAction containing the action to do.</returns>
        public BattlePhaseAction OnSelectBattleCmd(BattlePhase battle)
        {
            _pendingAttacker = null;
            _pendingAttackTarget = null;

            foreach (CardExecutor exec in Executor.Executors)
            {
                if (exec.Type == ExecutorType.GoToMainPhase2 && battle.CanMainPhaseTwo && exec.Func()) // check if should enter main phase 2 directly
                {
                    return ToMainPhase2();
                }
                if (exec.Type == ExecutorType.GoToEndPhase && battle.CanEndPhase && exec.Func()) // check if should enter end phase directly
                {
                    return ToEndPhase();
                }
                for (int i = 0; i < battle.ActivableCards.Count; ++i)
                {
                    ClientCard card = battle.ActivableCards[i];
                    if (ShouldExecute(exec, card, ExecutorType.Activate, battle.ActivableDescs[i]))
                    {
                        _dialogs.SendChaining(card.Name);
                        return new BattlePhaseAction(BattlePhaseAction.BattleAction.Activate, card.ActionIndex);
                    }
                }
            }

            // Sort the attackers and defenders, make monster with higher battle power go first.
            List<ClientCard> attackers = battle.AttackableCards
                .Where(card => !_attackersWithInvalidPreselectedTarget.Contains(card))
                .ToList();
            attackers.Sort(CardContainer.CompareCardAttackPower);
            attackers.Reverse();

            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(CardContainer.CompareDefensePower);
            defenders.Reverse();

            if (attackers.Count > 0)
            { // bad indent, just to reduce the diff

            // Let executor decide which card should attack first.
            ClientCard selected = Executor.OnSelectAttacker(attackers, defenders);
            if (selected != null && attackers.Contains(selected))
            {
                attackers.Remove(selected);
                attackers.Insert(0, selected);
            }

            // Check for the executor.
            BattlePhaseAction result = Executor.OnBattle(attackers, defenders);
            if (result != null)
                return result;

            if (defenders.Count == 0)
            {
                // Attack with the monster with the lowest attack first
                ClientCard attacker = attackers[attackers.Count - 1];
                return Attack(attacker, null);
            }
            else
            {
                for (int k = 0; k < attackers.Count; ++k)
                {
                    ClientCard attacker = attackers[k];
                    attacker.IsLastAttacker = (k == attackers.Count - 1);
                    result = Executor.OnSelectAttackTarget(attacker, defenders);
                    if (result != null)
                        return result;
                }
            }

            } // end of if (attackers.Count > 0)

            if (battle.CanMainPhaseTwo)
                return ToMainPhase2();
            if (battle.CanEndPhase)
                return ToEndPhase();

            Logger.DebugWriteLine("No monster to attack, but can't leave battle phase.", true);
            BattlePhaseAction fallbackAction = Attack(battle.AttackableCards[0], (defenders.Count == 0) ? null : defenders[0]);
            _pendingAttacker = null; // don't fall into the preselected attack target logic which can cancel the attack and cause an infinite loop
            _pendingAttackTarget = null;
            return fallbackAction;
        }

        /// <summary>
        /// Called when the AI has to select one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimal quantity.</param>
        /// <param name="max">Maximal quantity.</param>
        /// <param name="hint">The hint message of the select.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the selected cards.</returns>
        public IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            IList<ClientCard> result;

            // Check for the pendulum summon selection first.
            if (hint == HintMsg.SpSummon && _selectingPendulumSummon)
            {
                result = Executor.OnSelectPendulumSummon(cards, min, max);
                result = ValidateCardSelection(result, cards, min, max, cancelable);
                if (result != null)
                    return result;
            }

            // Attack target selection uses GameMessage.SelectCard.
            // Some scripts also use this HintMsg so check _pendingAttacker.
            if (hint == HintMsg.AttackTarget && _pendingAttacker != null)
            {
                if (_pendingAttackTarget != null && cards.Contains(_pendingAttackTarget))
                {
                    var target = new List<ClientCard> { _pendingAttackTarget };
                    return target;
                }
                else
                {
                    // TODO: Avoid this by redefining the attack logic.
                    Logger.DebugWriteLine("The preselected attack target is not in the list of legal targets.", true);
                    if (cancelable)
                    {
                        _attackersWithInvalidPreselectedTarget.Add(_pendingAttacker);
                        _pendingAttacker = null;
                        _pendingAttackTarget = null;
                        return new List<ClientCard>();
                    }
                    // else: use default selection below, which will attack the monster we don't want.
                }
            }

            // Check for the executor.
            result = Executor.OnSelectCard(cards, min, max, hint, cancelable);
            result = ValidateCardSelection(result, cards, min, max, cancelable);
            if (result != null)
                return result;

            CardSelector selector = null;
            if (hint == HintMsg.FusionMaterial || hint == HintMsg.SynchroMaterial || hint == HintMsg.XyzMaterial || hint == HintMsg.LinkMaterial)
            {
                if (m_materialSelector != null)
                {
                    //Logger.DebugWriteLine("m_materialSelector");
                    selector = m_materialSelector;
                }
                else
                {
                    if (hint == HintMsg.FusionMaterial)
                        result = Executor.OnSelectFusionMaterial(cards, min, max);
                    if (hint == HintMsg.SynchroMaterial)
                        result = Executor.OnSelectSynchroMaterial(cards, 0, min, max);
                    if (hint == HintMsg.XyzMaterial)
                        result = Executor.OnSelectXyzMaterial(cards, min, max);
                    if (hint == HintMsg.LinkMaterial)
                        result = Executor.OnSelectLinkMaterial(cards, min, max);

                    result = ValidateCardSelection(result, cards, min, max, cancelable);
                    if (result != null)
                        return result;

                    // Update the next selector.
                    selector = GetSelectedCards();
                }
            }
            else
            {
                if (m_materialSelector != null && hint == m_materialSelectorHint)
                {
                    //Logger.DebugWriteLine("m_materialSelector hint match");
                    selector = m_materialSelector;
                }
                else
                {
                    // Update the next selector.
                    selector = GetSelectedCards();
                }
            }

            // If we selected a card, use this card.
            if (selector != null)
                return selector.Select(cards, min, max);

            // Always select the first available cards and choose the minimum.
            IList<ClientCard> selected = new List<ClientCard>();
            if (cards.Count >= min)
            {
                for (int i = 0; i < min; ++i)
                    selected.Add(cards[i]);
            }

            if (hint == HintMsg.AttackTarget && cancelable)
                Logger.DebugWriteLine("Attack target selection not covered by _pendingAttacker.", true);

            return selected;
        }

        private IList<ClientCard> ValidateCardSelection(IList<ClientCard> selected, IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            if (selected == null)
                return null;

            bool validCount = selected.Count >= min && selected.Count <= max;
            if (cancelable && selected.Count == 0)
                validCount = true;

            if (!validCount || selected.Distinct().Count() != selected.Count || selected.Any(card => card == null || !cards.Contains(card)))
            {
                Logger.WriteErrorLine("Invalid card selection returned by executor, using default selection.");
                return null;
            }

            return selected;
        }

        /// <summary>
        /// Called when the AI can chain (activate) a card.
        /// </summary>
        /// <param name="cards">List of activable cards.</param>
        /// <param name="descs">List of effect descriptions.</param>
        /// <param name="forced">You can't return -1 if this param is true.</param>
        /// <param name="timing">Current hint timing</param>
        /// <returns>Index of the activated card or -1.</returns>
        public int OnSelectChain(IList<ClientCard> cards, IList<int> descs, IList<bool> forces, int timing = -1)
        {
            Executor.OnSelectChain(cards);
            foreach (CardExecutor exec in Executor.Executors)
            {
                for (int i = 0; i < cards.Count; ++i)
                {
                    ClientCard card = cards[i];
                    if (ShouldExecute(exec, card, ExecutorType.Activate, descs[i], timing))
                    {
                        _dialogs.SendChaining(card.Name);
                        return i;
                    }
                }
            }
            for (int i = 0; i < forces.Count; ++i)
            {
                if (forces[i])
                {
                    // If the card is forced, we have to activate it.
                    _dialogs.SendChaining(cards[i].Name);
                    return i;
                }
            }
            // Don't do anything.
            return -1;
        }
        
        /// <summary>
        /// Called when the AI has to use one or more counters.
        /// </summary>
        /// <param name="type">Type of counter to use.</param>
        /// <param name="quantity">Quantity of counter to select.</param>
        /// <param name="cards">List of available cards.</param>
        /// <param name="counters">List of available counters.</param>
        /// <returns>List of used counters.</returns>
        public IList<int> OnSelectCounter(int type, int quantity, IList<ClientCard> cards, IList<int> counters)
        {
            // Always select the first available counters.
            int[] used = new int[counters.Count];
            int i = 0;
            while (quantity > 0)
            {
                if (counters[i] >= quantity)
                {
                    used[i] = quantity;
                    quantity = 0;
                }
                else
                {
                    used[i] = counters[i];
                    quantity -= counters[i];
                }
                i++;
            }
            return used;
        }

        /// <summary>
        /// Called when the AI has to sort cards.
        /// </summary>
        /// <param name="cards">Cards to sort.</param>
        /// <returns>List of sorted cards.</returns>
        public IList<ClientCard> OnCardSorting(IList<ClientCard> cards)
        {

            IList<ClientCard> result = Executor.OnCardSorting(cards);
            if (result != null)
                return result;
            result = new List<ClientCard>();
            // TODO: use selector
            result = cards.ToList();
            return result;
        }

        /// <summary>
        /// Called when the AI has to choose to activate or not an effect.
        /// </summary>
        /// <param name="card">Card to activate.</param>
        /// <returns>True for yes, false for no.</returns>
        public bool OnSelectEffectYn(ClientCard card, int desc)
        {
            foreach (CardExecutor exec in Executor.Executors)
            {
                if (ShouldExecute(exec, card, ExecutorType.Activate, desc))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Called when the AI has to do something during the main phase.
        /// </summary>
        /// <param name="main">A lot of informations about the available actions.</param>
        /// <returns>A new MainPhaseAction containing the action to do.</returns>
        public MainPhaseAction OnSelectIdleCmd(MainPhase main)
        {
            _selectingPendulumSummon = false;
            CheckSurrender();
            foreach (CardExecutor exec in Executor.Executors)
            {
            	if (exec.Type == ExecutorType.GoToEndPhase && main.CanEndPhase && exec.Func()) // check if should enter end phase directly
                {
                    _dialogs.SendEndTurn();
                    return new MainPhaseAction(MainPhaseAction.MainAction.ToEndPhase);
                }
                if (exec.Type==ExecutorType.GoToBattlePhase && main.CanBattlePhase && exec.Func()) // check if should enter battle phase directly
                {
                    return new MainPhaseAction(MainPhaseAction.MainAction.ToBattlePhase);
                }
                // NOTICE: GoToBattlePhase and GoToEndPhase has no "card" can be accessed to ShouldExecute(), so instead use exec.Func() to check ...
                // enter end phase and enter battle pahse is in higher priority. 

                for (int i = 0; i < main.ActivableCards.Count; ++i)
                {
                    ClientCard card = main.ActivableCards[i];
                    if (ShouldExecute(exec, card, ExecutorType.Activate, main.ActivableDescs[i]))
                    {
                        _dialogs.SendActivate(card.Name);
                        return new MainPhaseAction(MainPhaseAction.MainAction.Activate, card.ActionActivateIndex[main.ActivableDescs[i]]);
                    }
                }
                foreach (ClientCard card in main.MonsterSetableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.MonsterSet))
                    {
                        _dialogs.SendSetMonster();
                        return new MainPhaseAction(MainPhaseAction.MainAction.SetMonster, card.ActionIndex);
                    }
                }
                foreach (ClientCard card in main.ReposableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.Repos))
                        return new MainPhaseAction(MainPhaseAction.MainAction.Repos, card.ActionIndex);
                }
                foreach (ClientCard card in main.SpecialSummonableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.SpSummon))
                    {
                        ClientCard leftScale = Executor.Util.GetPZone(0, 0);
                        ClientCard rightScale = Executor.Util.GetPZone(0, 1);
                        _selectingPendulumSummon = card.HasType(CardType.Pendulum)
                            && (card == leftScale || card == rightScale);
                        if (!_selectingPendulumSummon)
                            _dialogs.SendSummon(card.Name);
                        return new MainPhaseAction(MainPhaseAction.MainAction.SpSummon, card.ActionIndex);
                    }
                }
                foreach (ClientCard card in main.SummonableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.Summon))
                    {
                        _dialogs.SendSummon(card.Name);
                        return new MainPhaseAction(MainPhaseAction.MainAction.Summon, card.ActionIndex);
                    }
                    if (ShouldExecute(exec, card, ExecutorType.SummonOrSet))
                    {
                        if (main.MonsterSetableCards.Contains(card) && Executor.OnSelectMonsterSummonOrSet(card))
                        {
                            _dialogs.SendSetMonster();
                            return new MainPhaseAction(MainPhaseAction.MainAction.SetMonster, card.ActionIndex);
                        }
                        _dialogs.SendSummon(card.Name);
                        return new MainPhaseAction(MainPhaseAction.MainAction.Summon, card.ActionIndex);
                    }
                }
                foreach (ClientCard card in main.SpellSetableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.SpellSet))
                        return new MainPhaseAction(MainPhaseAction.MainAction.SetSpell, card.ActionIndex);
                }
            }

            if (main.CanBattlePhase && (Duel.Fields[0].HasAttackingMonster() || !main.CanEndPhase))
                return new MainPhaseAction(MainPhaseAction.MainAction.ToBattlePhase);

            _dialogs.SendEndTurn();
            return new MainPhaseAction(MainPhaseAction.MainAction.ToEndPhase); 
        }

        /// <summary>
        /// Called when the AI has to select an option.
        /// </summary>
        /// <param name="options">List of available options.</param>
        /// <returns>Index of the selected option.</returns>
        public int OnSelectOption(IList<int> options)
        {
            int selectorSelected = m_option;
            m_option = -1;

            int result = Executor.OnSelectOption(options);
            if (result >= 0 && result < options.Count)
                return result;

            if (selectorSelected >= 0 && selectorSelected < options.Count)
                return selectorSelected;

            return 0; // Always select the first option.
        }

        public uint OnSelectPlace(int cardId, int count, uint available)
        {
            if (count > 1)
            {
                m_place = 0;
                return OnSelectDisfield(cardId, count, available);
            }

            int player;
            CardLocation location;
            int filter;
            if ((available & 0x7fu) != 0)
            {
                player = 0;
                location = CardLocation.MonsterZone;
                filter = (int)(available & (uint)Zones.MonsterZones);
            }
            else if ((available & 0x1f00u) != 0)
            {
                player = 0;
                location = CardLocation.SpellZone;
                filter = (int)((available >> 8) & (uint)Zones.SpellZones);
            }
            else if ((available & 0x2000u) != 0)
            {
                player = 0;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else if ((available & 0xc000u) != 0)
            {
                player = 0;
                location = CardLocation.PendulumZone;
                filter = (int)((available >> 14) & (uint)Zones.PendulumZones);
            }
            else if ((available & 0x7f0000u) != 0)
            {
                player = 1;
                location = CardLocation.MonsterZone;
                filter = (int)((available >> 16) & (uint)Zones.MonsterZones);
            }
            else if ((available & 0x1f000000u) != 0)
            {
                player = 1;
                location = CardLocation.SpellZone;
                filter = (int)((available >> 24) & (uint)Zones.SpellZones);
            }
            else if ((available & 0x20000000u) != 0)
            {
                player = 1;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else
            {
                player = 1;
                location = CardLocation.PendulumZone;
                filter = (int)((available >> 30) & (uint)Zones.PendulumZones);
            }

            int selector_selected = m_place;
            m_place = 0;

            int executor_selected = Executor.OnSelectPlace(cardId, player, location, filter);

            if ((executor_selected & filter) > 0)
                filter &= executor_selected;
            else if ((selector_selected & filter) > 0)
                filter &= selector_selected;

            // TODO: Some helpers for prefering linked zones or non-linked zones

            int sequence = 0;
            if (location != CardLocation.PendulumZone && location != CardLocation.FieldZone)
            {
                if ((filter & Zones.z2) != 0) sequence = 2;
                else if ((filter & Zones.z1) != 0) sequence = 1;
                else if ((filter & Zones.z3) != 0) sequence = 3;
                else if ((filter & Zones.z0) != 0) sequence = 0;
                else if ((filter & Zones.z4) != 0) sequence = 4;
                else if ((filter & Zones.z6) != 0) sequence = 6;
                else if ((filter & Zones.z5) != 0) sequence = 5;
            }
            else
            {
                location = CardLocation.SpellZone;
                if ((filter & Zones.FieldZone) != 0) sequence = 5;
                if ((filter & Zones.z0) != 0) sequence = 6;
                if ((filter & Zones.z1) != 0) sequence = 7;
            }
            return 1u << (player * 16 + (location == CardLocation.MonsterZone ? 0 : 8) + sequence);
        }

        public uint OnSelectDisfield(int hint, int count, uint available)
        {
            int required = System.Math.Max(1, count);
            uint executorSelected = Executor.OnSelectDisfield(hint, count, available) & available;
            int executorSelectedCount = 0;
            for (uint pending = executorSelected; pending != 0; pending &= pending - 1u)
                ++executorSelectedCount;
            bool selectsMirroredExtraMonsterZones =
                (executorSelected & ((1u << 5) | (1u << 22))) == ((1u << 5) | (1u << 22))
                || (executorSelected & ((1u << 6) | (1u << 21))) == ((1u << 6) | (1u << 21));
            if (executorSelectedCount == required && !selectsMirroredExtraMonsterZones)
                return executorSelected;

            // Select zones in central order, opponent's zones first.
            IList<uint> zones = new List<uint>();
            int[] monsterZoneOrder = { 2, 1, 3, 0, 4, 6, 5 };
            int[] spellZoneOrder = { 2, 1, 3, 0, 4 };
            foreach (int player in new[] { 1, 0 })
            {
                int offset = player * 16;
                foreach (int sequence in monsterZoneOrder)
                {
                    uint zone = 1u << (offset + sequence);
                    if ((available & zone) != 0)
                        zones.Add(zone);
                }
                foreach (int sequence in spellZoneOrder)
                {
                    uint zone = 1u << (offset + 8 + sequence);
                    if ((available & zone) != 0)
                        zones.Add(zone);
                }
            }
            foreach (int player in new[] { 0, 1 })
            {
                int offset = player * 16 + 8;
                foreach (int sequence in new[] { 5, 7, 6 })
                {
                    uint zone = 1u << (offset + sequence);
                    if ((available & zone) != 0)
                        zones.Add(zone);
                }
            }

            uint selected = 0;
            int remaining = required;
            foreach (uint zone in zones)
            {
                if (remaining == 0)
                    break;

                // Each extra monster zone has two protocol coordinates, but selecting both is invalid.
                if ((zone == (1u << 5) && (selected & (1u << 22)) != 0)
                    || (zone == (1u << 22) && (selected & (1u << 5)) != 0)
                    || (zone == (1u << 6) && (selected & (1u << 21)) != 0)
                    || (zone == (1u << 21) && (selected & (1u << 6)) != 0))
                    continue;

                selected |= zone;
                --remaining;
            }
            return selected;
        }

        /// <summary>
        /// Called when the AI has to select a card position.
        /// </summary>
        /// <param name="cardId">Id of the card to position on the field.</param>
        /// <param name="positions">List of available positions.</param>
        /// <returns>Selected position.</returns>
        public CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            CardPosition executor_selected = Executor.OnSelectPosition(cardId, positions);

            CardPosition selector_selected = GetSelectedPosition();

            // Selects the selected position if available, the first available otherwise.
            if (positions.Contains(executor_selected))
                return executor_selected;
            if (positions.Contains(selector_selected))
                return selector_selected;

            return positions[0];
        }

        /// <summary>
        /// Called when the AI has to tribute for a synchro monster or ritual monster.
        /// </summary>
        /// <param name="cards">Available optional cards.</param>
        /// <param name="mandatoryCards">Cards that must be included.</param>
        /// <param name="sum">Result of the operation.</param>
        /// <param name="min">Minimum cards.</param>
        /// <param name="max">Maximum cards.</param>
        /// <param name="mode">True for exact equal.</param>
        /// <returns></returns>
        public IList<ClientCard> OnSelectSum(IList<ClientCard> cards, IList<ClientCard> mandatoryCards,
            int sum, int min, int max, int hint, bool mode)
        {
            int optionalSum = sum - mandatoryCards.Sum(card => card.OpParam1);
            IList<ClientCard> selected = Executor.OnSelectSum(cards, optionalSum, min, max, hint, mode);
            if (IsValidSumSelection(selected, cards, mandatoryCards, sum, min, max, mode))
                return selected;

            if (hint == HintMsg.Release || hint == HintMsg.SynchroMaterial)
            {
                if (m_materialSelector != null)
                {
                    CardSelector selector = m_materialSelector;
                    selected = selector.Select(cards, min, max);
                }
                else
                {
                    switch (hint)
                    {
                        case HintMsg.SynchroMaterial:
                            selected = Executor.OnSelectSynchroMaterial(cards, optionalSum, min, max);
                            break;
                        case HintMsg.Release:
                            selected = Executor.OnSelectRitualTribute(cards, optionalSum, min, max);
                            break;
                    }
                }
                if (IsValidSumSelection(selected, cards, mandatoryCards, sum, min, max, mode))
                    return selected;
            }

            IList<ClientCard> orderedCards = new List<ClientCard>();
            if (selected != null)
            {
                foreach (ClientCard card in selected)
                {
                    if (card != null && cards.Contains(card) && !orderedCards.Contains(card))
                        orderedCards.Add(card);
                }
            }
            foreach (ClientCard card in cards)
            {
                if (!orderedCards.Contains(card))
                    orderedCards.Add(card);
            }

            selected = FindSumSelection(orderedCards, mandatoryCards, sum, min, max, mode);
            if (selected != null)
                return selected;

            Logger.WriteErrorLine("Fail to select sum.");
            return new List<ClientCard>();
        }

        private bool CanReachSum(IList<ClientCard> cards, int index, long currentSum, int min, int max)
        {
            if (currentSum > max)
                return false;
            if (index == cards.Count)
                return currentSum >= min && currentSum <= max;

            ClientCard card = cards[index];
            if (CanReachSum(cards, index + 1, currentSum + card.OpParam1, min, max))
                return true;
            return card.OpParam2 > 0 && card.OpParam2 != card.OpParam1
                && CanReachSum(cards, index + 1, currentSum + card.OpParam2, min, max);
        }

        private bool IsValidSumSelection(IList<ClientCard> selected, IList<ClientCard> cards,
            IList<ClientCard> mandatoryCards, int sum, int min, int max, bool mode)
        {
            if (selected == null || selected.Distinct().Count() != selected.Count
                || selected.Any(card => card == null || !cards.Contains(card)))
                return false;

            if (mode && (selected.Count < min || selected.Count > max))
                return false;

            IList<ClientCard> allSelected = mandatoryCards.Concat(selected).ToList();
            if (mode)
                return CanReachSum(allSelected, 0, 0, sum, sum);

            // OCGCore's greater-than mode accepts only a minimal set: its maximum
            // possible sum reaches the target, but removing the smallest minimum
            // contribution would no longer reach it.
            if (allSelected.Count == 0)
                return sum <= 0;

            long minimumSum = 0;
            long maximumSum = 0;
            int smallestMinimum = int.MaxValue;
            foreach (ClientCard card in allSelected)
            {
                int minimum = card.OpParam2 > 0 ? System.Math.Min(card.OpParam1, card.OpParam2) : card.OpParam1;
                int maximum = System.Math.Max(card.OpParam1, card.OpParam2);
                minimumSum += minimum;
                maximumSum += maximum;
                smallestMinimum = System.Math.Min(smallestMinimum, minimum);
            }
            return IsValidGreaterSum(minimumSum, maximumSum, smallestMinimum, sum);
        }

        private IList<ClientCard> FindSumSelection(IList<ClientCard> cards, IList<ClientCard> mandatoryCards,
            int sum, int min, int max, bool mode)
        {
            if (!mode)
            {
                long minimumSum = 0;
                long maximumSum = 0;
                int smallestMinimum = int.MaxValue;
                foreach (ClientCard card in mandatoryCards)
                {
                    int minimum = card.OpParam2 > 0 ? System.Math.Min(card.OpParam1, card.OpParam2) : card.OpParam1;
                    minimumSum += minimum;
                    maximumSum += System.Math.Max(card.OpParam1, card.OpParam2);
                    smallestMinimum = System.Math.Min(smallestMinimum, minimum);
                }

                IList<ClientCard> result = new List<ClientCard>();
                long[] remainingMaximums = new long[cards.Count + 1];
                for (int i = cards.Count - 1; i >= 0; --i)
                {
                    remainingMaximums[i] = remainingMaximums[i + 1]
                        + System.Math.Max(cards[i].OpParam1, cards[i].OpParam2);
                }
                return TrySelectGreaterSum(cards, remainingMaximums, sum, 0, minimumSum, maximumSum,
                    smallestMinimum, mandatoryCards.Count, result) ? result : null;
            }

            HashSet<long> mandatorySums = new HashSet<long> { 0 };
            foreach (ClientCard card in mandatoryCards)
            {
                HashSet<long> nextSums = new HashSet<long>();
                foreach (long current in mandatorySums)
                {
                    if (current + card.OpParam1 <= sum)
                        nextSums.Add(current + card.OpParam1);
                    if (card.OpParam2 > 0 && card.OpParam2 != card.OpParam1 && current + card.OpParam2 <= sum)
                        nextSums.Add(current + card.OpParam2);
                }
                mandatorySums = nextSums;
            }

            HashSet<long> optionalSums = new HashSet<long>(mandatorySums.Select(value => (long)sum - value));
            long maximumOptionalSum = optionalSums.Count > 0 ? optionalSums.Max() : -1;
            int maximumCount = System.Math.Min(max, cards.Count);
            for (int count = min; count <= maximumCount; ++count)
            {
                IList<ClientCard> result = new List<ClientCard>();
                var failed = new HashSet<System.Tuple<int, int, long>>();
                if (TrySelectCardsBySum(cards, optionalSums, maximumOptionalSum, 0, count, 0, result, failed))
                    return result;
            }
            return null;
        }

        private bool TrySelectCardsBySum(IList<ClientCard> cards, ISet<long> targetSums, long maximumTarget,
            int index, int remainingCount, long currentSum, IList<ClientCard> result,
            ISet<System.Tuple<int, int, long>> failed)
        {
            if (remainingCount == 0)
                return targetSums.Contains(currentSum);
            if (currentSum > maximumTarget || cards.Count - index < remainingCount)
                return false;

            var state = System.Tuple.Create(index, remainingCount, currentSum);
            if (failed.Contains(state))
                return false;

            ClientCard card = cards[index];
            result.Add(card);
            if (card.OpParam2 > 0 && card.OpParam2 != card.OpParam1
                && TrySelectCardsBySum(cards, targetSums, maximumTarget, index + 1, remainingCount - 1,
                    currentSum + card.OpParam2, result, failed))
                return true;
            if (TrySelectCardsBySum(cards, targetSums, maximumTarget, index + 1, remainingCount - 1,
                currentSum + card.OpParam1, result, failed))
                return true;
            result.RemoveAt(result.Count - 1);

            if (TrySelectCardsBySum(cards, targetSums, maximumTarget, index + 1, remainingCount, currentSum, result, failed))
                return true;

            failed.Add(state);
            return false;
        }

        private bool TrySelectGreaterSum(IList<ClientCard> cards, IList<long> remainingMaximums,
            int sum, int index, long minimumSum, long maximumSum, int smallestMinimum,
            int selectedCount, IList<ClientCard> result)
        {
            if (selectedCount > 0 && IsValidGreaterSum(minimumSum, maximumSum, smallestMinimum, sum))
                return true;
            if (selectedCount > 0 && minimumSum - smallestMinimum >= sum)
                return false;
            if (index >= cards.Count || maximumSum + remainingMaximums[index] < sum)
                return false;

            ClientCard card = cards[index];
            int minimum = card.OpParam2 > 0 ? System.Math.Min(card.OpParam1, card.OpParam2) : card.OpParam1;
            result.Add(card);
            if (TrySelectGreaterSum(cards, remainingMaximums, sum, index + 1, minimumSum + minimum,
                maximumSum + System.Math.Max(card.OpParam1, card.OpParam2),
                System.Math.Min(smallestMinimum, minimum), selectedCount + 1, result))
                return true;
            result.RemoveAt(result.Count - 1);

            return TrySelectGreaterSum(cards, remainingMaximums, sum, index + 1, minimumSum, maximumSum,
                smallestMinimum, selectedCount, result);
        }

        private bool IsValidGreaterSum(long minimumSum, long maximumSum, int smallestMinimum, int sum)
        {
            return maximumSum >= sum && minimumSum - smallestMinimum < sum;
        }

        /// <summary>
        /// Called when the AI has to tribute one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimum tribute value.</param>
        /// <param name="max">Maximum tribute value.</param>
        /// <param name="hint">The hint message of the select.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the tributed cards.</returns>
        public IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            List<ClientCard> sorted = new List<ClientCard>();
            sorted.AddRange(cards);
            sorted.Sort(CardContainer.CompareCardAttack);

            IList<ClientCard> selected = FindTributeSelection(sorted, min, max);
            if (selected != null)
                return selected;

            Logger.WriteErrorLine("Fail to select tribute.");
            return new List<ClientCard>();
        }

        public bool IsValidTributeSelection(IList<ClientCard> selected, int min, int max)
        {
            return selected != null && CanReachSum(selected, 0, 0, min, max);
        }

        public IList<ClientCard> FindTributeSelection(IList<ClientCard> cards, int min, int max)
        {
            ISet<long> targetSums = new HashSet<long>();
            for (int value = min; value <= max; ++value)
                targetSums.Add(value);

            int maximumCount = System.Math.Min(max, cards.Count);
            for (int count = 0; count <= maximumCount; ++count)
            {
                IList<ClientCard> selected = new List<ClientCard>();
                ISet<System.Tuple<int, int, long>> failed = new HashSet<System.Tuple<int, int, long>>();
                if (TrySelectCardsBySum(cards, targetSums, max, 0, count, 0, selected, failed))
                    return selected;
            }
            return null;
        }

        /// <summary>
        /// Called when the AI has to select yes or no.
        /// </summary>
        /// <param name="desc">Id of the question.</param>
        /// <returns>True for yes, false for no.</returns>
        public bool OnSelectYesNo(int desc)
        {
            int selected = m_yesno;
            m_yesno = -1;
            if (selected != -1)
                return selected > 0;
            return Executor.OnSelectYesNo(desc);
        }

        /// <summary>
        /// Called when the AI has to select if to continue attacking when replay.
        /// </summary>
        /// <returns>True for yes, false for no.</returns>
        public bool OnSelectBattleReplay()
        {
            return Executor.OnSelectBattleReplay();
        }

        /// <summary>
        /// Called when the AI has to declare a card.
        /// </summary>
        /// <param name="avail">Available card's ids.</param>
        /// <returns>Id of the selected card.</returns>
        public int OnAnnounceCard(IList<int> avail)
        {
            int announced = m_announce;
            m_announce = 0;

            int selected = Executor.OnAnnounceCard(avail);
            if (avail.Contains(selected))
                return selected;
            if (avail.Contains(announced))
                return announced;
            else if (announced > 0)
                Logger.WriteErrorLine("Pre-announced card cant be used: " + announced);
            return avail[0];
        }

        // _ Others functions _
        // Those functions are used by the AI behavior.

        
        private CardSelector m_materialSelector;
        private int m_materialSelectorHint;
        private int m_place;
        private int m_option;
        private int m_number;
        private int m_announce;
        private int m_yesno;
        private IList<CardAttribute> m_attributes = new List<CardAttribute>();
        private IList<CardSelector> m_selector = new List<CardSelector>();
        private IList<CardPosition> m_position = new List<CardPosition>();
        private int m_selector_pointer = -1;
        private IList<CardRace> m_races = new List<CardRace>();

        public void SelectCard(ClientCard card)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(card));
        }

        public void SelectCard(IList<ClientCard> cards)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(cards));
        }

        public void SelectCard(int cardId)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(cardId));
        }

        public void SelectCard(IList<int> ids)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(ids));
        }

        public void SelectCard(params int[] ids)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(ids));
        }

        public void SelectCard(CardLocation loc)
        {
            m_selector_pointer = m_selector.Count();
            m_selector.Add(new CardSelector(loc));
        }

        public void SelectNextCard(ClientCard card)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(card));
        }

        public void SelectNextCard(IList<ClientCard> cards)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cards));
        }

        public void SelectNextCard(int cardId)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cardId));
        }

        public void SelectNextCard(IList<int> ids)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectNextCard(params int[] ids)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectNextCard(CardLocation loc)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(loc));
        }

        public void SelectThirdCard(ClientCard card)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(card));
        }

        public void SelectThirdCard(IList<ClientCard> cards)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cards));
        }

        public void SelectThirdCard(int cardId)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cardId));
        }

        public void SelectThirdCard(IList<int> ids)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectThirdCard(params int[] ids)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectThirdCard(CardLocation loc)
        {
            if (m_selector_pointer == -1)
            {
                //Logger.WriteErrorLine("Called SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(loc));
        }

        public void SelectMaterials(ClientCard card, int hint = 0)
        {
            m_materialSelector = new CardSelector(card);
            m_materialSelectorHint = hint;
        }

        public void SelectMaterials(IList<ClientCard> cards, int hint = 0)
        {
            m_materialSelector = new CardSelector(cards);
            m_materialSelectorHint = hint;
        }

        public void SelectMaterials(int cardId, int hint = 0)
        {
            m_materialSelector = new CardSelector(cardId);
            m_materialSelectorHint = hint;
        }

        public void SelectMaterials(IList<int> ids, int hint = 0)
        {
            m_materialSelector = new CardSelector(ids);
            m_materialSelectorHint = hint;
        }

        public void SelectMaterials(CardLocation loc, int hint = 0)
        {
            m_materialSelector = new CardSelector(loc);
            m_materialSelectorHint = hint;
        }

        public void CleanSelectMaterials()
        {
            m_materialSelector = null;
            m_materialSelectorHint = 0;
        }

        public bool HaveSelectedCards()
        {
            return m_selector.Count > 0 || m_materialSelector != null;
        }

        public bool HaveSelectedPosition()
        {
            return m_position.Count > 0;
        }

        public CardSelector GetSelectedCards()
        {
            CardSelector selected = null;
            if (m_selector.Count > 0)
            {
                selected = m_selector[m_selector.Count - 1];
                m_selector.RemoveAt(m_selector.Count - 1);
            }
            return selected;
        }

        public CardPosition GetSelectedPosition()
        {
            CardPosition selected = CardPosition.FaceUpAttack;
            if (m_position.Count > 0)
            {
                selected = m_position[0];
                m_position.RemoveAt(0);
            }
            return selected;
        }

        public void SelectPosition(CardPosition pos)
        {
            m_position.Add(pos);
        }

        public void SelectPlace(int zones)
        {
            m_place = zones;
        }

        public void SelectOption(int opt)
        {
            m_option = opt;
        }

        public void SelectNumber(int number)
        {
            m_number = number;
        }

        public void SelectAttribute(CardAttribute attribute)
        {
            m_attributes.Clear();
            m_attributes.Add(attribute);
        }

        public void SelectAttributes(CardAttribute[] attributes)
        {
            m_attributes.Clear();
            foreach (CardAttribute attribute in attributes)
                m_attributes.Add(attribute);
        }

        public void SelectRace(CardRace race)
        {
            m_races.Clear();
            m_races.Add(race);
        }

        public void SelectRaces(CardRace[] races)
        {
            m_races.Clear();
            foreach (CardRace race in races)
                m_races.Add(race);
        }

        public void SelectAnnounceID(int id)
        {
            m_announce = id;
        }

        public void SelectYesNo(bool opt)
        {
            m_yesno = opt ? 1 : 0;
        }

        /// <summary>
        /// Called when the AI has to declare a number.
        /// </summary>
        /// <param name="numbers">List of available numbers.</param>
        /// <returns>Index of the selected number.</returns>
        public int OnAnnounceNumber(IList<int> numbers)
        {
            int selected = m_number;
            m_number = -1;
            if (numbers.Contains(selected))
                return numbers.IndexOf(selected);

            return Program.Rand.Next(0, numbers.Count); // Returns a random number.
        }

        /// <summary>
        /// Called when the AI has to declare one or more attributes.
        /// </summary>
        /// <param name="count">Quantity of attributes to declare.</param>
        /// <param name="attributes">List of available attributes.</param>
        /// <returns>A list of the selected attributes.</returns>
        public virtual IList<CardAttribute> OnAnnounceAttrib(int count, IList<CardAttribute> attributes)
        {
            IList<CardAttribute> foundAttributes = m_attributes.Where(attributes.Contains).Distinct().Take(count).ToList();
            m_attributes.Clear();
            foreach (CardAttribute attribute in attributes)
            {
                if (foundAttributes.Count >= count)
                    break;
                if (!foundAttributes.Contains(attribute))
                    foundAttributes.Add(attribute);
            }

            return foundAttributes;
        }

        /// <summary>
        /// Called when the AI has to declare one or more races.
        /// </summary>
        /// <param name="count">Quantity of races to declare.</param>
        /// <param name="races">List of available races.</param>
        /// <returns>A list of the selected races.</returns>
        public virtual IList<CardRace> OnAnnounceRace(int count, IList<CardRace> races)
        {
            IList<CardRace> foundRaces = m_races.Where(races.Contains).Distinct().Take(count).ToList();
            m_races.Clear();
            foreach (CardRace race in races)
            {
                if (foundRaces.Count >= count)
                    break;
                if (!foundRaces.Contains(race))
                    foundRaces.Add(race);
            }

            return foundRaces;
        }

        public BattlePhaseAction Attack(ClientCard attacker, ClientCard defender)
        {
            if (defender != null)
            {
                string cardName = defender.Name ?? "monster";
                _dialogs.SendAttack(attacker.Name, cardName);
            }
            else
            {
                _dialogs.SendDirectAttack(attacker.Name);
            }
            _pendingAttacker = attacker;
            _pendingAttackTarget = defender;
            return new BattlePhaseAction(BattlePhaseAction.BattleAction.Attack, attacker.ActionIndex);
        }

        public BattlePhaseAction ToEndPhase()
        {
            _dialogs.SendEndTurn();
            return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToEndPhase);
        }
        public BattlePhaseAction ToMainPhase2()
        {
            return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToMainPhaseTwo);
        }

        private bool ShouldExecute(CardExecutor exec, ClientCard card, ExecutorType type, int desc = -1, int timing = -1)
        {
            Executor.SetCard(type, card, desc, timing);
            if (card.Id != 0 && type == ExecutorType.Activate)
            {
                if (_activatedCards.ContainsKey(card.Id) && _activatedCards[card.Id] >= 9)
                    return false;
                if (!Executor.OnPreActivate(card))
                    return false;
            }
            bool result = card != null && exec.Type == type &&
                (exec.CardId == -1 || card.IsOriginalCode(exec.CardId)) &&
                (exec.Func == null || exec.Func());
            if (card.Id != 0 && type == ExecutorType.Activate && result)
            {
                int count = card.IsDisabled() ? 3 : 1;
                if (!_activatedCards.ContainsKey(card.Id))
                {
                    _activatedCards.Add(card.Id, count);
                }
                else
                {
                    _activatedCards[card.Id] += count;
                }
            }
            return result;
        }
    }
}
