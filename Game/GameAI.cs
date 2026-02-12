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
            m_selector.Clear();
            m_position.Clear();
            m_selector_pointer = -1;
            m_materialSelector = null;
            m_materialSelectorHint = 0;
            m_option = -1;
            m_yesno = -1;
            m_announce = 0;
           
            m_place = 0;
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
        
        /// <summary>
        /// Called when a chain has been solved.
        /// </summary>
        public void OnChainEnd()
        {
            m_selector.Clear();
            m_selector_pointer = -1;
            Executor.OnChainEnd();
            CheckSurrender();
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

            // Sort the attackers and defenders, make monster with higher attack go first.
            List<ClientCard> attackers = new List<ClientCard>(battle.AttackableCards);
            attackers.Sort(CardContainer.CompareCardAttack);
            attackers.Reverse();

            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(CardContainer.CompareDefensePower);
            defenders.Reverse();

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

            if (attackers.Count == 0)
                return ToMainPhase2();

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

            if (!battle.CanMainPhaseTwo)
                return Attack(attackers[0], (defenders.Count == 0) ? null : defenders[0]);

            return ToMainPhase2();
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
            // Check for the executor.
            IList<ClientCard> result = Executor.OnSelectCard(cards, min, max, hint, cancelable);
            if (result != null)
                return result;

            if (hint == HintMsg.SpSummon && min == 1 && max > min) // pendulum summon
            {
                result = Executor.OnSelectPendulumSummon(cards, max);
                if (result != null)
                    return result;
            }

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

            if (hint == HintMsg.AttackTarget && cancelable) return selected;

            if (cards.Count >= min)
            {
                for (int i = 0; i < min; ++i)
                    selected.Add(cards[i]);
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

            if (main.CanBattlePhase && Duel.Fields[0].HasAttackingMonster())
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
            int result = Executor.OnSelectOption(options);
            if (result != -1)
                return result;

            if (m_option != -1 && m_option < options.Count)
                return m_option;

            return 0; // Always select the first option.
        }

        public int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            int selector_selected = m_place;
            m_place = 0;

            int executor_selected = Executor.OnSelectPlace(cardId, player, location, available);

            if ((executor_selected & available) > 0)
                return executor_selected & available;
            if ((selector_selected & available) > 0)
                return selector_selected & available;

            // TODO: LinkedZones

            return 0;
        }

        /// <summary>
        /// Called when the AI has to select a card position.
        /// </summary>
        /// <param name="cardId">Id of the card to position on the field.</param>
        /// <param name="positions">List of available positions.</param>
        /// <returns>Selected position.</returns>
        public CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            CardPosition selector_selected = GetSelectedPosition();

            CardPosition executor_selected = Executor.OnSelectPosition(cardId, positions);

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
        /// <param name="cards">Available cards.</param>
        /// <param name="sum">Result of the operation.</param>
        /// <param name="min">Minimum cards.</param>
        /// <param name="max">Maximum cards.</param>
        /// <param name="mode">True for exact equal.</param>
        /// <returns></returns>
        public IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, int hint, bool mode)
        {
            IList<ClientCard> selected = Executor.OnSelectSum(cards, sum, min, max, hint, mode);
            if (selected != null)
            {
                return selected;
            }

            if (hint == HintMsg.Release || hint == HintMsg.SynchroMaterial)
            {
                if (m_materialSelector != null)
                {
                    selected = m_materialSelector.Select(cards, min, max);
                }
                else
                {
                    switch (hint)
                    {
                        case HintMsg.SynchroMaterial:
                            selected = Executor.OnSelectSynchroMaterial(cards, sum, min, max);
                            break;
                        case HintMsg.Release:
                            selected = Executor.OnSelectRitualTribute(cards, sum, min, max);
                            break;
                    }
                }
                if (selected != null)
                {
                    int s1 = 0, s2 = 0;
                    foreach (ClientCard card in selected)
                    {
                        s1 += card.OpParam1;
                        s2 += (card.OpParam2 != 0) ? card.OpParam2 : card.OpParam1;
                    }
                    if ((mode && (s1 == sum || s2 == sum)) || (!mode && (s1 >= sum || s2 >= sum)))
                    {
                        return selected;
                    }
                }
            }

            if (mode)
            {
                // equal

                if (sum == 0 && min == 0)
                {
                    return new List<ClientCard>();
                }

                if (min <= 1)
                {
                    // try special level first
                    foreach (ClientCard card in cards)
                    {
                        if (card.OpParam2 == sum)
                        {
                            return new[] { card };
                        }
                    }
                    // try level equal
                    foreach (ClientCard card in cards)
                    {
                        if (card.OpParam1 == sum)
                        {
                            return new[] { card };
                        }
                    }
                }

                // try all
                int s1 = 0, s2 = 0;
                foreach (ClientCard card in cards)
                {
                    s1 += card.OpParam1;
                    s2 += (card.OpParam2 != 0) ? card.OpParam2 : card.OpParam1;
                }
                if (s1 == sum || s2 == sum)
                {
                    return cards;
                }

                // try all combinations
                int i = (min <= 1) ? 2 : min;
                while (i <= max && i <= cards.Count)
                {
                    IEnumerable<IEnumerable<ClientCard>> combos = CardContainer.GetCombinations(cards, i);

                    foreach (IEnumerable<ClientCard> combo in combos)
                    {
                        Logger.DebugWriteLine("--");
                        s1 = 0;
                        s2 = 0;
                        foreach (ClientCard card in combo)
                        {
                            s1 += card.OpParam1;
                            s2 += (card.OpParam2 != 0) ? card.OpParam2 : card.OpParam1;
                        }
                        if (s1 == sum || s2 == sum)
                        {
                            return combo.ToList();
                        }
                    }
                    i++;
                }
            }
            else
            {
                // larger
                if (min <= 1)
                {
                    // try special level first
                    foreach (ClientCard card in cards)
                    {
                        if (card.OpParam2 >= sum)
                        {
                            return new[] { card };
                        }
                    }
                    // try level equal
                    foreach (ClientCard card in cards)
                    {
                        if (card.OpParam1 >= sum)
                        {
                            return new[] { card };
                        }
                    }
                }

                // try all combinations
                int i = (min <= 1) ? 2 : min;
                while (i <= max && i <= cards.Count)
                {
                    IEnumerable<IEnumerable<ClientCard>> combos = CardContainer.GetCombinations(cards, i);

                    foreach (IEnumerable<ClientCard> combo in combos)
                    {
                        Logger.DebugWriteLine("----");
                        int s1 = 0, s2 = 0;
                        foreach (ClientCard card in combo)
                        {
                            s1 += card.OpParam1;
                            s2 += (card.OpParam2 != 0) ? card.OpParam2 : card.OpParam1;
                        }
                        if (s1 >= sum || s2 >= sum)
                        {
                            return combo.ToList();
                        }
                    }
                    i++;
                }
            }

            Logger.WriteErrorLine("Fail to select sum.");
            return new List<ClientCard>();
        }

        /// <summary>
        /// Called when the AI has to tribute one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimal quantity.</param>
        /// <param name="max">Maximal quantity.</param>
        /// <param name="hint">The hint message of the select.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the tributed cards.</returns>
        public IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // Always choose the minimum and lowest atk.
            List<ClientCard> sorted = new List<ClientCard>();
            sorted.AddRange(cards);
            sorted.Sort(CardContainer.CompareCardAttack);

            IList<ClientCard> selected = new List<ClientCard>();

            for (int i = 0; i < min && i < sorted.Count; ++i)
                selected.Add(sorted[i]);

            return selected;
        }

        /// <summary>
        /// Called when the AI has to select yes or no.
        /// </summary>
        /// <param name="desc">Id of the question.</param>
        /// <returns>True for yes, false for no.</returns>
        public bool OnSelectYesNo(int desc)
        {
            if (m_yesno != -1)
                return m_yesno > 0;
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
            int selected = Executor.OnAnnounceCard(avail);
            if (avail.Contains(selected))
                return selected;
            if (avail.Contains(m_announce))
                return m_announce;
            else if (m_announce > 0)
                Logger.WriteErrorLine("Pre-announced card cant be used: " + m_announce);
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
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(card));
        }

        public void SelectNextCard(IList<ClientCard> cards)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cards));
        }

        public void SelectNextCard(int cardId)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cardId));
        }

        public void SelectNextCard(IList<int> ids)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectNextCard(params int[] ids)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectNextCard(CardLocation loc)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectNextCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(loc));
        }

        public void SelectThirdCard(ClientCard card)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(card));
        }

        public void SelectThirdCard(IList<ClientCard> cards)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cards));
        }

        public void SelectThirdCard(int cardId)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(cardId));
        }

        public void SelectThirdCard(IList<int> ids)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectThirdCard(params int[] ids)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
                m_selector_pointer = 0;
            }
            m_selector.Insert(m_selector_pointer, new CardSelector(ids));
        }

        public void SelectThirdCard(CardLocation loc)
        {
            if (m_selector_pointer == -1)
            {
                Logger.WriteErrorLine("Error: Call SelectThirdCard() before SelectCard()");
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
            if (numbers.Contains(m_number))
                return numbers.IndexOf(m_number);

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
            IList<CardAttribute> foundAttributes = m_attributes.Where(attributes.Contains).ToList();
            if (foundAttributes.Count > 0)
                return foundAttributes;

            return attributes; // Returns the first available Attribute.
        }

        /// <summary>
        /// Called when the AI has to declare one or more races.
        /// </summary>
        /// <param name="count">Quantity of races to declare.</param>
        /// <param name="races">List of available races.</param>
        /// <returns>A list of the selected races.</returns>
        public virtual IList<CardRace> OnAnnounceRace(int count, IList<CardRace> races)
        {
            IList<CardRace> foundRaces = m_races.Where(races.Contains).ToList();
            if (foundRaces.Count > 0)
                return foundRaces;

            return races; // Returns the first available Races.
        }

        public BattlePhaseAction Attack(ClientCard attacker, ClientCard defender)
        {
            Executor.SetCard(0, attacker, -1);
            if (defender != null)
            {
                string cardName = defender.Name ?? "monster";
                attacker.ShouldDirectAttack = false;
                _dialogs.SendAttack(attacker.Name, cardName);
                SelectCard(defender);
            }
            else
            {
                attacker.ShouldDirectAttack = true;
                _dialogs.SendDirectAttack(attacker.Name);
            }
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
                (exec.CardId == -1 || exec.CardId == card.Id) &&
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
