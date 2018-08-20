﻿using System.Linq;
using System.Collections.Generic;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class GameAI
    {
        public GameClient Game { get; private set; }
        public Duel Duel { get; private set; }
        public Executor Executor { get; set; }
        public AIFunctions Utils { get; private set; }

        private Dialogs _dialogs;

        public GameAI(GameClient game, Duel duel)
        {
            Game = game;
            Duel = duel;
            Utils = new AIFunctions(duel);

            _dialogs = new Dialogs(game);
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
            Executor.OnNewTurn();
        }

        /// <summary>
        /// Called when it's a new phase.
        /// </summary>
        public void OnNewPhase()
        {
            m_selector = null;
            m_nextSelector = null;
            m_thirdSelector = null;
            m_materialSelector = null;
            m_option = -1;
            m_yesno = -1;
            m_position = CardPosition.FaceUpAttack;
            m_place = 0;
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Draw)
            {
                _dialogs.SendNewTurn();
            }
            Executor.OnNewPhase();
        }

        /// <summary>
        /// Called when the AI got attack directly.
        /// </summary>
        public void OnDirectAttack(ClientCard card)
        {
            _dialogs.SendOnDirectAttack(card.Name);
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
        
        /// <summary>
        /// Called when a chain has been solved.
        /// </summary>
        public void OnChainEnd()
        {
            Executor.OnChainEnd();
        }

        /// <summary>
        /// Called when the AI has to do something during the battle phase.
        /// </summary>
        /// <param name="battle">Informations about usable cards.</param>
        /// <returns>A new BattlePhaseAction containing the action to do.</returns>
        public BattlePhaseAction OnSelectBattleCmd(BattlePhase battle)
        {
            Executor.SetBattle(battle);
            foreach (CardExecutor exec in Executor.Executors)
            {
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
            attackers.Sort(AIFunctions.CompareCardAttack);
            attackers.Reverse();

            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(AIFunctions.CompareDefensePower);
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
                for (int i = attackers.Count - 1; i >= 0; --i)
                {
                    ClientCard attacker = attackers[i];
                    if (attacker.Attack > 0)
                        return Attack(attacker, null);
                }
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
            const int HINTMSG_FMATERIAL = 511;
            const int HINTMSG_SMATERIAL = 512;
            const int HINTMSG_XMATERIAL = 513;
            const int HINTMSG_LMATERIAL = 533;
            const int HINTMSG_SPSUMMON = 509;

            // Check for the executor.
            IList<ClientCard> result = Executor.OnSelectCard(cards, min, max, hint, cancelable);
            if (result != null)
                return result;

            if (hint == HINTMSG_SPSUMMON && min == 1 && max > min) // pendulum summon
            {
                result = Executor.OnSelectPendulumSummon(cards, max);
                if (result != null)
                    return result;
            }

            CardSelector selector = null;
            if (hint == HINTMSG_FMATERIAL || hint == HINTMSG_SMATERIAL || hint == HINTMSG_XMATERIAL || hint == HINTMSG_LMATERIAL)
            {
                if (m_materialSelector != null)
                {
                    //Logger.DebugWriteLine("m_materialSelector");
                    selector = m_materialSelector;
                }
                else
                {
                    if (hint == HINTMSG_FMATERIAL)
                        result = Executor.OnSelectFusionMaterial(cards, min, max);
                    if (hint == HINTMSG_SMATERIAL)
                        result = Executor.OnSelectSynchroMaterial(cards, 0, min, max);
                    if (hint == HINTMSG_XMATERIAL)
                        result = Executor.OnSelectXyzMaterial(cards, min, max);
                    if (hint == HINTMSG_LMATERIAL)
                        result = Executor.OnSelectLinkMaterial(cards, min, max);

                    if (result != null)
                        return result;

                    // Update the next selector.
                    selector = GetSelectedCards();
                }
            }
            else
            {
                // Update the next selector.
                selector = GetSelectedCards();
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
            return selected;
        }

        /// <summary>
        /// Called when the AI can chain (activate) a card.
        /// </summary>
        /// <param name="cards">List of activable cards.</param>
        /// <param name="descs">List of effect descriptions.</param>
        /// <param name="forced">You can't return -1 if this param is true.</param>
        /// <returns>Index of the activated card or -1.</returns>
        public int OnSelectChain(IList<ClientCard> cards, IList<int> descs, bool forced)
        {
            foreach (CardExecutor exec in Executor.Executors)
            {
                for (int i = 0; i < cards.Count; ++i)
                {
                    ClientCard card = cards[i];
                    if (ShouldExecute(exec, card, ExecutorType.Activate, descs[i]))
                    {
                        _dialogs.SendChaining(card.Name);
                        return i;
                    }
                }
            }
            // If we're forced to chain, we chain the first card. However don't do anything.
            return forced ? 0 : -1;
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
            for (int i = 0; i < cards.Count; i++)
            {
                result.Add(cards[i]);
            }
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
                if (ShouldExecute(exec, card, ExecutorType.Activate))
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
            Executor.SetMain(main);
            foreach (CardExecutor exec in Executor.Executors)
            {
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
                        Duel.LastSummonPlayer = 0;
                        return new MainPhaseAction(MainPhaseAction.MainAction.SpSummon, card.ActionIndex);
                    }
                }
                foreach (ClientCard card in main.SummonableCards)
                {
                    if (ShouldExecute(exec, card, ExecutorType.Summon))
                    {
                        _dialogs.SendSummon(card.Name);
                        Duel.LastSummonPlayer = 0;
                        return new MainPhaseAction(MainPhaseAction.MainAction.Summon, card.ActionIndex);
                    }
                    if (ShouldExecute(exec, card, ExecutorType.SummonOrSet))
                    {
                        if (Utils.IsAllEnemyBetter(true) && Utils.IsAllEnemyBetterThanValue(card.Attack + 300, false) &&
                            main.MonsterSetableCards.Contains(card))
                        {
                            _dialogs.SendSetMonster();
                            return new MainPhaseAction(MainPhaseAction.MainAction.SetMonster, card.ActionIndex);
                        }
                        _dialogs.SendSummon(card.Name);
                        Duel.LastSummonPlayer = 0;
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
            if (m_option != -1 && m_option < options.Count)
                return m_option;

            int result = Executor.OnSelectOption(options);
            if (result != -1)
                return result;

            return 0; // Always select the first option.
        }

        public int OnSelectPlace(int cardId, int player, int location, int available)
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
            CardPosition selector_selected = m_position;
            m_position = CardPosition.FaceUpAttack;

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
            const int HINTMSG_RELEASE = 500;
            const int HINTMSG_SMATERIAL = 512;

            IList<ClientCard> selected = Executor.OnSelectSum(cards, sum, min, max, hint, mode);
            if (selected != null)
            {
                return selected;
            }

            if (hint == HINTMSG_RELEASE || hint == HINTMSG_SMATERIAL)
            {
                if (m_materialSelector != null)
                {
                    selected = m_materialSelector.Select(cards, min, max);
                }
                else
                {
                    if (hint == HINTMSG_SMATERIAL)
                        selected = Executor.OnSelectSynchroMaterial(cards, sum, min, max);
                    if (hint == HINTMSG_RELEASE)
                        selected = Executor.OnSelectRitualTribute(cards, sum, min, max);
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
            sorted.Sort(AIFunctions.CompareCardAttack);

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
        /// <returns>Id of the selected card.</returns>
        public int OnAnnounceCard()
        {
            if (m_announce == 0)
                return 89631139; // Blue-eyes white dragon
            return m_announce;
        }

        // _ Others functions _
        // Those functions are used by the AI behavior.

        private CardSelector m_selector;
        private CardSelector m_nextSelector;
        private CardSelector m_thirdSelector;
        private CardSelector m_materialSelector;
        private CardPosition m_position = CardPosition.FaceUpAttack;
        private int m_place;
        private int m_option;
        private int m_number;
        private int m_announce;
        private int m_yesno;
        private IList<CardAttribute> m_attributes = new List<CardAttribute>();
        private IList<CardRace> m_races = new List<CardRace>();

        public void SelectCard(ClientCard card)
        {
            m_selector = new CardSelector(card);
        }

        public void SelectCard(IList<ClientCard> cards)
        {
            m_selector = new CardSelector(cards);
        }

        public void SelectCard(int cardId)
        {
            m_selector = new CardSelector(cardId);
        }

        public void SelectCard(IList<int> ids)
        {
            m_selector = new CardSelector(ids);
        }

        public void SelectCard(CardLocation loc)
        {
            m_selector = new CardSelector(loc);
        }

        public void SelectNextCard(ClientCard card)
        {
            m_nextSelector = new CardSelector(card);
        }

        public void SelectNextCard(IList<ClientCard> cards)
        {
            m_nextSelector = new CardSelector(cards);
        }

        public void SelectNextCard(int cardId)
        {
            m_nextSelector = new CardSelector(cardId);
        }

        public void SelectNextCard(IList<int> ids)
        {
            m_nextSelector = new CardSelector(ids);
        }

        public void SelectNextCard(CardLocation loc)
        {
            m_nextSelector = new CardSelector(loc);
        }

        public void SelectThirdCard(ClientCard card)
        {
            m_thirdSelector = new CardSelector(card);
        }

        public void SelectThirdCard(IList<ClientCard> cards)
        {
            m_thirdSelector = new CardSelector(cards);
        }

        public void SelectThirdCard(int cardId)
        {
            m_thirdSelector = new CardSelector(cardId);
        }

        public void SelectThirdCard(IList<int> ids)
        {
            m_thirdSelector = new CardSelector(ids);
        }

        public void SelectThirdCard(CardLocation loc)
        {
            m_thirdSelector = new CardSelector(loc);
        }

        public void SelectMaterials(ClientCard card)
        {
            m_materialSelector = new CardSelector(card);
        }

        public void SelectMaterials(IList<ClientCard> cards)
        {
            m_materialSelector = new CardSelector(cards);
        }

        public void SelectMaterials(int cardId)
        {
            m_materialSelector = new CardSelector(cardId);
        }

        public void SelectMaterials(IList<int> ids)
        {
            m_materialSelector = new CardSelector(ids);
        }

        public void SelectMaterials(CardLocation loc)
        {
            m_materialSelector = new CardSelector(loc);
        }

        public void CleanSelectMaterials()
        {
            m_materialSelector = null;
        }

        public CardSelector GetSelectedCards()
        {
            CardSelector selected = m_selector;
            m_selector = null;
            if (m_nextSelector != null)
            {
                m_selector = m_nextSelector;
                m_nextSelector = null;
                if (m_thirdSelector != null)
                {
                    m_nextSelector = m_thirdSelector;
                    m_thirdSelector = null;
                }
            }
            return selected;
        }

        public void SelectPosition(CardPosition pos)
        {
            m_position = pos;
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
            IList<CardAttribute> foundAttributes = new List<CardAttribute>();
            foreach (CardAttribute attribute in m_attributes)
            {
                if(attributes.Contains(attribute))
                    foundAttributes.Add(attribute);
            }
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
            IList<CardRace> foundRaces = new List<CardRace>();
            foreach (CardRace race in m_races)
            {
                if (races.Contains(race))
                    foundRaces.Add(race);
            }
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

        private bool ShouldExecute(CardExecutor exec, ClientCard card, ExecutorType type, int desc = -1)
        {
            Executor.SetCard(type, card, desc);
            if (card != null &&
                exec.Type == type &&
                (exec.CardId == -1 || exec.CardId == card.Id) &&
                (exec.Func == null || exec.Func()))
                return true;
            return false;
        }
    }
}