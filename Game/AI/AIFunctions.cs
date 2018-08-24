﻿using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
namespace WindBot.Game.AI
{
    public class AIFunctions
    {
        public Duel Duel { get; private set; }
        public ClientField Bot { get; private set; }
        public ClientField Enemy { get; private set; }

        public AIFunctions(Duel duel)
        {
            Duel = duel;
            Bot = Duel.Fields[0];
            Enemy = Duel.Fields[1];
        }

        public static int CompareCardAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.Attack < cardB.Attack)
                return -1;
            if (cardA.Attack == cardB.Attack)
                return 0;
            return 1;
        }

        public static int CompareDefensePower(ClientCard cardA, ClientCard cardB)
        {
            if (cardA == null && cardB == null)
                return 0;
            if (cardA == null)
                return -1;
            if (cardB == null)
                return 1;
            int powerA = cardA.GetDefensePower();
            int powerB = cardB.GetDefensePower();
            if (powerA < powerB)
                return -1;
            if (powerA == powerB)
                return 0;
            return 1;
        }

        /// <summary>
        /// Get the total ATK Monster of the player.
        /// </summary>
        public int GetTotalAttackingMonsterAttack(int player)
        {
            int atk = 0;
            foreach (ClientCard m in Duel.Fields[player].GetMonsters())
            {
                if (m.IsAttack())
                    atk += m.Attack;
            }
            return atk;
        }
        /// <summary>
        /// Get the best ATK or DEF power of the field.
        /// </summary>
        /// <param name="field">Bot or Enemy.</param>
        /// <param name="onlyATK">Only calculate attack.</param>
        public int GetBestPower(ClientField field, bool onlyATK = false)
        {
            int bestPower = -1;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = field.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                int newPower = card.GetDefensePower();
                if (newPower > bestPower)
                    bestPower = newPower;
            }
            return bestPower;
        }

        public int GetBestAttack(ClientField field)
        {
            return GetBestPower(field, true);
        }

        public bool IsOneEnemyBetterThanValue(int value, bool onlyATK)
        {
            int bestValue = -1;
            bool nomonster = true;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                nomonster = false;
                int enemyValue = card.GetDefensePower();
                if (enemyValue > bestValue)
                    bestValue = enemyValue;
            }
            if (nomonster) return false;
            return bestValue > value;
        }

        public bool IsAllEnemyBetterThanValue(int value, bool onlyATK)
        {
            bool nomonster = true;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                nomonster = false;
                int enemyValue = card.GetDefensePower();
                if (enemyValue <= value)
                    return false;
            }
            return !nomonster;
        }

        /// <summary>
        /// Deprecated, use IsOneEnemyBetter and IsAllEnemyBetter instead.
        /// </summary>
        public bool IsEnemyBetter(bool onlyATK, bool all)
        {
            if (all)
                return IsAllEnemyBetter(onlyATK);
            else
                return IsOneEnemyBetter(onlyATK);
        }

        /// <summary>
        /// Is there an enemy monster who has better power than the best power of the bot's?
        /// </summary>
        /// <param name="onlyATK">Only calculate attack.</param>
        public bool IsOneEnemyBetter(bool onlyATK = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return IsOneEnemyBetterThanValue(bestBotPower, onlyATK);
        }

        /// <summary>
        /// Do all enemy monsters have better power than the best power of the bot's?
        /// </summary>
        /// <param name="onlyATK">Only calculate attack.</param>
        public bool IsAllEnemyBetter(bool onlyATK = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return IsAllEnemyBetterThanValue(bestBotPower, onlyATK);
        }

        public ClientCard GetBestBotMonster(bool onlyATK = false)
        {
            int bestPower = -1;
            ClientCard bestMonster = null;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Bot.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                int newPower = card.GetDefensePower();
                if (newPower > bestPower)
                {
                    bestPower = newPower;
                    bestMonster = card;
                }
            }
            return bestMonster;
        }
		
        public ClientCard GetWorstBotMonster(bool onlyATK = false)
        {
            int WorstPower = -1;
            ClientCard WorstMonster = null;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Bot.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                int newPower = card.GetDefensePower();
                if (newPower < WorstPower)
                {
                    WorstPower = newPower;
                    WorstMonster = card;
                }
            }
            return WorstMonster;
        }
		
        public ClientCard GetOneEnemyBetterThanValue(int value, bool onlyATK = false, bool canBeTarget = false)
        {
            ClientCard bestCard = null;
            int bestValue = value;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card?.Data == null || (canBeTarget && card.IsShouldNotBeTarget())) continue;
                if (onlyATK && card.IsDefense()) continue;
                int enemyValue = card.GetDefensePower();
                if (enemyValue >= bestValue)
                {
                    bestCard = card;
                    bestValue = enemyValue;
                }
            }
            return bestCard;
        }

        public ClientCard GetOneEnemyBetterThanMyBest(bool onlyATK = false, bool canBeTarget = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return GetOneEnemyBetterThanValue(bestBotPower, onlyATK, canBeTarget);
        }

        public ClientCard GetProblematicEnemyCard(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.SpellZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;

            if (attack == 0)
                attack = GetBestAttack(Bot);
            return GetOneEnemyBetterThanValue(attack, true, canBeTarget);
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;

            if (attack == 0)
                attack = GetBestAttack(Bot);
            return GetOneEnemyBetterThanValue(attack, true, canBeTarget);
        }

        public ClientCard GetProblematicEnemySpell()
        {
            ClientCard card = Enemy.SpellZone.GetFloodgate();
            return card;
        }

        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null)
                return card;

            card = GetBestEnemySpell(onlyFaceup);
            if (card != null)
                return card;

            return null;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count > 0 && !onlyFaceup)
                return monsters[0];

            return null;
        }

        public ClientCard GetWorstEnemyMonster(bool onlyATK = false)
        {
            int WorstPower = -1;
            ClientCard WorstMonster = null;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card?.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                int newPower = card.GetDefensePower();
                if (newPower < WorstPower)
                {
                    WorstPower = newPower;
                    WorstMonster = card;
                }
            }
            return WorstMonster;
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false)
        {
            ClientCard card = GetProblematicEnemySpell();
            if (card != null)
                return card;

            List<ClientCard> spells = Enemy.GetSpells();

            foreach (ClientCard ecard in spells)
            {
                if (ecard.IsFaceup() && ecard.HasType(CardType.Continuous)||
                    ecard.IsFaceup() && ecard.HasType(CardType.Field))
                    return ecard;
            }

            if (spells.Count > 0 && !onlyFaceup)
                return spells[0];

            return null;
        }

        public ClientCard GetPZone(int player, int id)
        {
            if (Duel.IsNewRule)
            {
                return Duel.Fields[player].SpellZone[id*4];
            }
            else
            {
                return Duel.Fields[player].SpellZone[6+id];
            }
        }

        public int GetStringId(int id, int option)
        {
            return id * 16 + option;
        }

        public bool IsTurn1OrMain2()
        {
            return Duel.Turn == 1 || Duel.Phase == DuelPhase.Main2;
        }

        public bool IsChainTarget(ClientCard card)
        {
            return Duel.ChainTargets.Any(card.Equals);
        }

        public bool IsChainTargetOnly(ClientCard card)
        {
            return Duel.ChainTargetOnly.Count == 1 && card.Equals(Duel.ChainTargetOnly[0]);
        }

        public bool ChainContainsCard(int id)
        {
            return Duel.CurrentChain.Any(card => card.Id == id);
        }

        public int ChainCountPlayer(int player)
        {
            return Duel.CurrentChain.Count(card => card.Controller == player);
        }

        public bool ChainContainPlayer(int player)
        {
            return Duel.CurrentChain.Any(card => card.Controller == player);
        }

        public bool HasChainedTrap(int player)
        {
            return Duel.CurrentChain.Any(card => card.Controller == player && card.HasType(CardType.Trap));
        }

        public ClientCard GetLastChainCard()
        {
            return Duel.CurrentChain.LastOrDefault();
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(ClientCard preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            if (cards.IndexOf(preferred) > 0 && selected.Count < max)
            {
                selected.Add(preferred);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(int preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card.Id== preferred && selected.Count < max)
                    selected.Add(card);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(IList<ClientCard> preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> avail = cards.ToList(); // clone
            while (preferred.Count > 0 && avail.IndexOf(preferred[0]) > 0 && selected.Count < max)
            {
                ClientCard card = preferred[0];
                preferred.Remove(card);
                avail.Remove(card);
                selected.Add(card);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(IList<int> preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            foreach (int id in preferred)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.Id == id && selected.Count < max && selected.IndexOf(card) <= 0)
                        selected.Add(card);
                }
                if (selected.Count >= max)
                    break;
            }

            return selected;
        }

        /// <summary>
        /// Check and fix selected to make sure it meet the count requirement.
        /// </summary>
        public IList<ClientCard> CheckSelectCount(IList<ClientCard> _selected, IList<ClientCard> cards, int min, int max)
        {
            var selected = _selected.ToList();
            if (selected.Count < min)
            {
                foreach (ClientCard card in cards)
                {
                    if (!selected.Contains(card))
                        selected.Add(card);
                    if (selected.Count >= max)
                        break;
                }
            }
            while (selected.Count > max)
            {
                selected.RemoveAt(selected.Count - 1);
            }

            return selected;
        }
    }
}