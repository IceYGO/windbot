using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
namespace WindBot.Game.AI
{
    public class AIUtil
    {
        public Duel Duel { get; private set; }
        public ClientField Bot { get; private set; }
        public ClientField Enemy { get; private set; }

        public AIUtil(Duel duel)
        {
            Duel = duel;
            Bot = Duel.Fields[0];
            Enemy = Duel.Fields[1];
        }

        /// <summary>
        /// Get the total ATK Monster of the player.
        /// </summary>
        public int GetTotalAttackingMonsterAttack(int player)
        {
            return Duel.Fields[player].GetMonsters().Where(m => m.IsAttack()).Sum(m => (int?)m.Attack) ?? 0;
        }
        /// <summary>
        /// Get the best ATK or DEF power of the field.
        /// </summary>
        /// <param name="field">Bot or Enemy.</param>
        /// <param name="onlyATK">Only calculate attack.</param>
        public int GetBestPower(ClientField field, bool onlyATK = false)
        {
            return field.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .Max(card => (int?)card.GetDefensePower()) ?? -1;
        }

        public int GetBestAttack(ClientField field)
        {
            return GetBestPower(field, true);
        }

        public bool IsOneEnemyBetterThanValue(int value, bool onlyATK)
        {
            return Enemy.MonsterZone.GetMonsters()
                .Any(card => card.GetDefensePower() > value && (!onlyATK || card.IsAttack()));
        }

        public bool IsAllEnemyBetterThanValue(int value, bool onlyATK)
        {
            List<ClientCard> monsters = Enemy.MonsterZone.GetMonsters();
            return monsters.Count > 0 && monsters
                .All(card => card.GetDefensePower() > value && (!onlyATK || card.IsAttack()));
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
            return Bot.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderByDescending(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetWorstBotMonster(bool onlyATK = false)
        {
            return Bot.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderBy(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetOneEnemyBetterThanValue(int value, bool onlyATK = false, bool canBeTarget = false)
        {
            return Enemy.MonsterZone.GetMonsters()
                .FirstOrDefault(card => card.GetDefensePower() >= value && (!onlyATK || card.IsAttack()) && (!canBeTarget || !card.IsShouldNotBeTarget()));
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
            return Enemy.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderBy(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false)
        {
            ClientCard card = GetProblematicEnemySpell();
            if (card != null)
                return card;

            var spells = Enemy.GetSpells();

            card = spells.FirstOrDefault(ecard => ecard.IsFaceup() && (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field)));
            if (card != null)
                return card;

            if (spells.Count > 0 && !onlyFaceup)
                return spells[0];

            return null;
        }

        public ClientCard GetPZone(int player, int id)
        {
            if (Duel.IsNewRule)
            {
                return Duel.Fields[player].SpellZone[id * 4];
            }
            else
            {
                return Duel.Fields[player].SpellZone[6 + id];
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

        public int GetBotAvailZonesFromExtraDeck(IList<ClientCard> remove)
        {
            ClientCard[] BotMZone = (ClientCard[])Bot.MonsterZone.Clone();
            ClientCard[] EnemyMZone = (ClientCard[])Enemy.MonsterZone.Clone();
            for (int i = 0; i < 7; i++)
            {
                if (remove.Contains(BotMZone[i])) BotMZone[i] = null;
                if (remove.Contains(EnemyMZone[i])) EnemyMZone[i] = null;
            }

            if (!Duel.IsNewRule || Duel.IsNewRule2020)
                return Zones.MainMonsterZones;

            int result = 0;

            if (BotMZone[5] == null && BotMZone[6] == null)
            {
                if (EnemyMZone[5] == null)
                    result |= Zones.z6;
                if (EnemyMZone[6] == null)
                    result |= Zones.z5;
            }

            if (BotMZone[0] == null &&
                ((BotMZone[1]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.BottomLeft) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.TopRight) ?? false)))
                result |= Zones.z0;

            if (BotMZone[1] == null &&
                ((BotMZone[0]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[2]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.Bottom) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.Top) ?? false)))
                result |= Zones.z1;

            if (BotMZone[2] == null &&
                ((BotMZone[1]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[3]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.BottomRight) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.TopLeft) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.BottomLeft) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.TopRight) ?? false)))
                result |= Zones.z2;

            if (BotMZone[3] == null &&
                ((BotMZone[2]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[4]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.Bottom) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.Top) ?? false)))
                result |= Zones.z3;

            if (BotMZone[4] == null &&
                ((BotMZone[3]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.BottomRight) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.TopLeft) ?? false)))
                result |= Zones.z4;

            return result;
        }

        public int GetBotAvailZonesFromExtraDeck(ClientCard remove)
        {
            return GetBotAvailZonesFromExtraDeck(new[] { remove });
        }

        public int GetBotAvailZonesFromExtraDeck()
        {
            return GetBotAvailZonesFromExtraDeck(new List<ClientCard>());
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
            return Duel.CurrentChain.Any(card => card.IsCode(id));
        }

        public bool ChainContainsCard(int[] ids)
        {
            return Duel.CurrentChain.Any(card => card.IsCode(ids));
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
                if (card.IsCode(preferred) && selected.Count < max)
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
                    if (card.IsCode(id) && selected.Count < max && selected.IndexOf(card) <= 0)
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
            var selected = _selected.Distinct().ToList();
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