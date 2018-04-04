﻿using System;
using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI
{
    public abstract class Executor
    {
        public string Deck { get; set; }
        public Duel Duel { get; private set; }
        public IList<CardExecutor> Executors { get; private set; }
        public GameAI AI { get; private set; }

        protected MainPhase Main { get; private set; }
        protected BattlePhase Battle { get; private set; }

        protected ExecutorType Type { get; private set; }
        protected ClientCard Card { get; private set; }
        protected int ActivateDescription { get; private set; }

        protected ClientField Bot { get; private set; }
        protected ClientField Enemy { get; private set; }

        protected Executor(GameAI ai, Duel duel)
        {
            Duel = duel;
            AI = ai;
            Executors = new List<CardExecutor>();

            Bot = Duel.Fields[0];
            Enemy = Duel.Fields[1];
        }

        public virtual int OnRockPaperScissors()
        {
            return Program.Rand.Next(1, 4);
        }

        public virtual bool OnSelectHand()
        {
            return Program.Rand.Next(2) > 0;
        }

        /// <summary>
        /// Called when the AI has to decide if it should attack
        /// </summary>
        /// <param name="attackers">List of monsters that can attcack.</param>
        /// <param name="defenders">List of monsters of enemy.</param>
        /// <returns>A new BattlePhaseAction containing the action to do.</returns>
        public virtual BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (attackers.Count == 0)
                return AI.ToMainPhase2();

            if (defenders.Count == 0)
            {
                for (int i = attackers.Count - 1; i >= 0; --i)
                {
                    ClientCard attacker = attackers[i];
                    if (attacker.Attack > 0)
                        return AI.Attack(attacker, null);
                }
            }
            else
            {
                for (int i = defenders.Count - 1; i >= 0; --i)
                {
                    ClientCard defender = defenders[i];
                    for (int j = 0; j < attackers.Count; ++j)
                    {
                        ClientCard attacker = attackers[j];
                        attacker.RealPower = attacker.Attack;
                        defender.RealPower = defender.GetDefensePower();
                        if (!OnPreBattleBetween(attacker, defender))
                            continue;
                        if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && j == attackers.Count - 1))
                            return AI.Attack(attacker, defender);
                    }
                }

                for (int i = attackers.Count - 1; i >= 0; --i)
                {
                    ClientCard attacker = attackers[i];
                    if (attacker.CanDirectAttack)
                        return AI.Attack(attacker, null);
                }
            }

            if (!Battle.CanMainPhaseTwo)
                return AI.Attack(attackers[0], (defenders.Count == 0) ? null : defenders[0]);

            return AI.ToMainPhase2();
        }

        public virtual bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            // Overrided in DefalultExecutor
            return true;
        }

        public virtual void OnChaining(int player, ClientCard card)
        {
            
        }

        public virtual void OnChainEnd()
        {
            
        }

        public virtual void OnNewTurn()
        {
            // Some AI need do something on new turn
        }

        public virtual IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, int hint, bool mode)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectFusionMaterial(IList<ClientCard> cards, int min, int max)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectSynchroMaterial(IList<ClientCard> cards, int sum, int min, int max)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectLinkMaterial(IList<ClientCard> cards, int min, int max)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectRitualTribute(IList<ClientCard> cards, int sum, int min, int max)
        {
            // For overriding
            return null;
        }

        public virtual IList<ClientCard> OnSelectPendulumSummon(IList<ClientCard> cards, int max)
        {
            // For overriding
            return null;
        }

        public virtual bool OnSelectYesNo(int desc)
        {
            return true;
        }

        public virtual int OnSelectOption(IList<int> options)
        {
            return -1;
        }

        public void SetMain(MainPhase main)
        {
            Main = main;
        }

        public void SetBattle(BattlePhase battle)
        {
            Battle = battle;
        }

        /// <summary>
        /// Set global variables Type, Card, ActivateDescription for Executor
        /// </summary>
        public void SetCard(ExecutorType type, ClientCard card, int description)
        {
            Type = type;
            Card = card;
            ActivateDescription = description;
        }

        /// <summary>
        /// Do the action for the card if func return true.
        /// </summary>
        public void AddExecutor(ExecutorType type, int cardId, Func<bool> func)
        {
            Executors.Add(new CardExecutor(type, cardId, func));
        }

        /// <summary>
        /// Do the action for the card if available.
        /// </summary>
        public void AddExecutor(ExecutorType type, int cardId)
        {
            Executors.Add(new CardExecutor(type, cardId, null));
        }

        /// <summary>
        /// Do the action for every card if func return true.
        /// </summary>
        public void AddExecutor(ExecutorType type, Func<bool> func)
        {
            Executors.Add(new CardExecutor(type, -1, func));
        }

        /// <summary>
        /// Do the action for every card if no other Executor is added to it.
        /// </summary>
        public void AddExecutor(ExecutorType type)
        {
            Executors.Add(new CardExecutor(type, -1, DefaultNoExecutor));
        }

        private bool DefaultNoExecutor()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return true;
        }
    }
}