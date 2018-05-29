using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;
using System.Linq;

namespace WindBot.Game.AI
{
    public static class CardContainer
    {
        public static ClientCard GetHighestAttackMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            int highestAtk = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null || card.IsFacedown() || (canBeTarget && card.IsShouldNotBeTarget())) continue;
                if (card.HasType(CardType.Monster) && card.Attack > highestAtk)
                {
                    highestAtk = card.Attack;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetHighestDefenseMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            int highestDef = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null || card.IsFacedown() || (canBeTarget && card.IsShouldNotBeTarget())) continue;
                if (card.HasType(CardType.Monster) && card.Defense > highestDef)
                {
                    highestDef = card.Defense;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetLowestAttackMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            int lowestAtk = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null || card.IsFacedown() || (canBeTarget && card.IsShouldNotBeTarget())) continue;
                if (lowestAtk == 0 && card.HasType(CardType.Monster) ||
                    card.HasType(CardType.Monster) && card.Attack < lowestAtk)
                {
                    lowestAtk = card.Attack;
                    selected = card;
                }
            }
            return selected;
        }

        public static ClientCard GetLowestDefenseMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            int lowestDef = 0;
            ClientCard selected = null;
            foreach (ClientCard card in cards)
            {
                if (card == null || card.Data == null || card.IsFacedown() || (canBeTarget && card.IsShouldNotBeTarget())) continue;
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

        public static List<ClientCard> GetMonsters(this IEnumerable<ClientCard> cards)
        {
            List<ClientCard> cardlist = new List<ClientCard>();

            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.HasType(CardType.Monster))
                    cardlist.Add(card);
            }
            return cardlist;
        }

        public static List<ClientCard> GetFaceupPendulumMonsters(this IEnumerable<ClientCard> cards)
        {
            List<ClientCard> cardlist = new List<ClientCard>();

            foreach (ClientCard card in cards)
            {
                if (card == null)
                    continue;
                if (card.HasType(CardType.Monster) && card.IsFaceup() && card.HasType(CardType.Pendulum))
                    cardlist.Add(card);
            }
            return cardlist;
        }

        public static ClientCard GetInvincibleMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsMonsterInvincible() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()))
                    return card;
            }
            return null;
        }

        public static ClientCard GetDangerousMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsMonsterDangerous() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()))
                    return card;
            }
            return null;
        }

        public static ClientCard GetFloodgate(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            foreach (ClientCard card in cards)
            {
                if (card != null && card.IsFloodgate() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()))
                    return card;
            }
            return null;
        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).GetCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}