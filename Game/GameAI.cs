using OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot.Game.AI;

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
        /// Called when the duel starts.
        /// </summary>
        public void OnStart()
        {
            _dialogs.SendDuelStart();
        }

        /// <summary>
        /// Called when the AI won the rock-paper-scissors.
        /// </summary>
        /// <returns>True if the AI should begin, false otherwise.</returns>
        public bool OnSelectHand()
        {
            return Executor.OnSelectHand();
        }

        /// <summary>
        /// Called when it's a new turn.
        /// </summary>
        public void OnNewTurn()
        {
        }

        /// <summary>
        /// Called when it's a new phase.
        /// </summary>
        public void OnNewPhase()
        {
            m_selector = null;
            m_nextSelector = null;
            m_option = -1;
            m_position = CardPosition.FaceUpAttack;
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Draw)
                _dialogs.SendNewTurn();
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

            List<ClientCard> attackers = new List<ClientCard>(battle.AttackableCards);
            attackers.Sort(AIFunctions.CompareCardAttack);

            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(AIFunctions.CompareDefensePower);

            return Executor.OnBattle(attackers, defenders);
        }

        /// <summary>
        /// Called when the AI has to select one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimal quantity.</param>
        /// <param name="max">Maximal quantity.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the selected cards.</returns>
        public IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            // Check for the executor.
            IList<ClientCard> result = Executor.OnSelectCard(cards, min,max,cancelable);
            if (result != null)
                return result;

            // Update the next selector.
            CardSelector selector = GetSelectedCards();

            // If we selected a card, use this card.
            if (selector != null)
                return selector.Select(cards, min, max);

            // Always select the first available cards and choose the minimum.
            IList<ClientCard> selected = new List<ClientCard>();

            for (int i = 0; i < min; ++i)
                selected.Add(cards[i]);

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
        /// Called when the AI has to choose to activate or not an effect.
        /// </summary>
        /// <param name="card">Card to activate.</param>
        /// <returns>True for yes, false for no.</returns>
        public bool OnSelectEffectYn(ClientCard card)
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
                        if (Utils.IsEnnemyBetter(true, true) && Utils.IsAllEnnemyBetterThanValue(card.Attack + 300, false) &&
                            main.MonsterSetableCards.Contains(card))
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
            if (m_option != -1)
                return m_option;
            return 0; // Always select the first option.
        }

        /// <summary>
        /// Called when the AI has to select a card position.
        /// </summary>
        /// <param name="cardId">Id of the card to position on the field.</param>
        /// <param name="positions">List of available positions.</param>
        /// <returns>Selected position.</returns>
        public CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            // Selects the selected position if available, the first available otherwise.
            if (positions.Contains(m_position))
            {
                CardPosition old = m_position;
                m_position = CardPosition.FaceUpAttack;
                return old;
            }
            return positions[0];
        }

        /// <summary>
        /// Called when the AI has to tribute for a synchro monster.
        /// </summary>
        /// <param name="cards">Available cards.</param>
        /// <param name="sum">Result of the operation.</param>
        /// <param name="min">Minimum cards.</param>
        /// <param name="max">Maximum cards.</param>
        /// <returns></returns>
        public IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max)
        {
            // Always return one card. The first available.
            foreach (ClientCard card in cards)
            {
                if (card.Level == sum)
                    return new[] { card };
            }
            // However return everything, that may work.
            return cards;
        }

        /// <summary>
        /// Called when the AI has to tribute one or more cards.
        /// </summary>
        /// <param name="cards">List of available cards.</param>
        /// <param name="min">Minimal quantity.</param>
        /// <param name="max">Maximal quantity.</param>
        /// <param name="cancelable">True if you can return an empty list.</param>
        /// <returns>A new list containing the tributed cards.</returns>
        public IList<ClientCard> OnSelectTribute(IList<ClientCard> cards, int min, int max, bool cancelable)
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
            return Executor.OnSelectYesNo(desc);
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
        private CardPosition m_position = CardPosition.FaceUpAttack;
        private int m_option;
        private int m_number;
        private int m_announce;
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

        public CardSelector GetSelectedCards()
        {
            CardSelector selected = m_selector;
            if (m_nextSelector != null)
            {
                m_selector = m_nextSelector;
                m_nextSelector = null;
            }
            else
                m_selector = null;

            return selected;
        }

        public void SelectPosition(CardPosition pos)
        {
            m_position = pos;
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
            if (defender != null)
            {
                string cardName = defender.Name ?? "monster";
                _dialogs.SendAttack(attacker.Name, cardName);
                SelectCard(defender);
            }
            else
                _dialogs.SendDirectAttack(attacker.Name);
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