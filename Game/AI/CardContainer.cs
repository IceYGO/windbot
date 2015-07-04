using System.Collections.Generic;
using WindBot.Game.Enums;

namespace WindBot.Game.AI
{
    public static class CardContainer
    {
        public static ClientCard GetHighestAttackMonster(this IEnumerable<ClientCard> cards)
        {
            int highestAtk = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null) continue;
                if (card.HasType(CardType.Monster) && card.Attack > highestAtk)
                {
                    highestAtk = card.Attack;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetHighestDefenseMonster(this IEnumerable<ClientCard> cards)
        {
            int highestDef = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null) continue;
                if (card.HasType(CardType.Monster) && card.Defense > highestDef)
                {
                    highestDef = card.Defense;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetLowestAttackMonster(this IEnumerable<ClientCard> cards)
        {
            int lowestAtk = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null) continue;
                if (lowestAtk == 0 && card.HasType(CardType.Monster) ||
                    card.HasType(CardType.Monster) && card.Attack < lowestAtk)
                {
                    lowestAtk = card.Attack;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetLowestDefenseMonster(this IEnumerable<ClientCard> cards)
        {
            int lowestDef = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null) continue;
                if (lowestDef == 0 && card.HasType(CardType.Monster) ||
                    card.HasType(CardType.Monster) && card.Defense < lowestDef)
                {
                    lowestDef = card.Defense;
                    selected = card;
                }
            }
            return selected;
        }

        public static bool ContainsMonsterWithLevel(this IEnumerable<ClientCard> cards, int level)
        {
            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (!card.HasType(CardType.Xyz) && card.Level == level)
                    return true;
            }
            return false;
        }

        public static bool ContainsMonsterWithRank(this IEnumerable<ClientCard> cards, int rank)
        {
            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.HasType(CardType.Xyz) && card.Rank == rank)
                    return true;
            }
            return false;
        }

        public static bool ContainsCardWithId(this IEnumerable<ClientCard> cards, int id)
        {
            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.Id == id)
                    return true;
            }
            return false;
        }

        public static int GetCardCount(this IEnumerable<ClientCard> cards, int id)
        {
            int count = 0;
            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.Id == id)
                    count++;
            }
            return count;
        }

        public static IList<ClientCard> GetMonsters(this IEnumerable<ClientCard> cards)
        {
            IList<ClientCard> cardlist = new List<ClientCard>();

            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.HasType(CardType.Monster))
                    cardlist.Add(card);

            }
            return cardlist;
        }

        public static ClientCard GetInvincibleMonster(this IEnumerable<ClientCard> cards)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsMonsterInvincible())
                    return card;
            }
            return null;
        }

        public static ClientCard GetNegateAttackSpell(this IEnumerable<ClientCard> cards)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsSpellNegateAttack())
                    return card;
            }
            return null;
        }
    }
}