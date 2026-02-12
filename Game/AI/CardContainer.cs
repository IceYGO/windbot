using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;
using System;
using System.Linq;

namespace WindBot.Game.AI
{
    public static class CardContainer
    {
        public static int CompareCardAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.Attack < cardB.Attack)
                return -1;
            if (cardA.Attack == cardB.Attack)
                return 0;
            return 1;
        }

        public static int CompareCardLevel(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.Level < cardB.Level)
                return -1;
            if (cardA.Level == cardB.Level)
                return 0;
            return 1;
        }

        public static int CompareCardLink(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.LinkCount < cardB.LinkCount)
                return -1;
            if (cardA.LinkCount == cardB.LinkCount)
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

        public static ClientCard GetHighestAttackMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards
                .Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && !(canBeTarget && card.IsShouldNotBeTarget()))
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
        }

        public static ClientCard GetHighestDefenseMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards
                .Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && !(canBeTarget && card.IsShouldNotBeTarget()))
                .OrderByDescending(card => card.Defense)
                .FirstOrDefault();
        }

        public static ClientCard GetLowestAttackMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards
                .Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && !(canBeTarget && card.IsShouldNotBeTarget()))
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
        }

        public static ClientCard GetLowestDefenseMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards
                .Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && !(canBeTarget && card.IsShouldNotBeTarget()))
                .OrderBy(card => card.Defense)
                .FirstOrDefault();
        }

        public static bool ContainsMonsterWithLevel(this IEnumerable<ClientCard> cards, int level)
        {
            return cards.Where(card => card?.Data != null).Any(card => !card.HasType(CardType.Xyz) && card.Level == level);
        }

        public static bool ContainsMonsterWithRank(this IEnumerable<ClientCard> cards, int rank)
        {
            return cards.Where(card => card?.Data != null).Any(card => card.HasType(CardType.Xyz) && card.Rank == rank);
        }

        public static bool ContainsCardWithId(this IEnumerable<ClientCard> cards, int id)
        {
            return cards.Where(card => card?.Data != null).Any(card => card.IsCode(id));
        }

        public static int GetCardCount(this IEnumerable<ClientCard> cards, int id)
        {
            return cards.Where(card => card?.Data != null).Count(card => card.IsCode(id));
        }

        public static List<ClientCard> GetMonsters(this IEnumerable<ClientCard> cards)
        {
            return cards.Where(card => card?.Data != null && card.HasType(CardType.Monster)).ToList();
        }

        public static List<ClientCard> GetFaceupPendulumMonsters(this IEnumerable<ClientCard> cards)
        {
            return cards.Where(card => card?.Data != null && card.HasType(CardType.Monster) && card.IsFaceup() && card.HasType(CardType.Pendulum)).ToList();
        }

        public static ClientCard GetInvincibleMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards.FirstOrDefault(card => card?.Data != null && card.IsMonsterInvincible() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()));
        }

        public static ClientCard GetDangerousMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards.FirstOrDefault(card => card?.Data != null && card.IsMonsterDangerous() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()));
        }

        public static ClientCard GetFloodgate(this IEnumerable<ClientCard> cards, bool canBeTarget = false)
        {
            return cards.FirstOrDefault(card => card?.Data != null && card.IsFloodgate() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()));
        }

        public static ClientCard GetFirstMatchingCard(this IEnumerable<ClientCard> cards, Func<ClientCard, bool> filter)
        {
            return cards.FirstOrDefault(card => card?.Data != null && filter.Invoke(card));
        }

        public static ClientCard GetFirstMatchingFaceupCard(this IEnumerable<ClientCard> cards, Func<ClientCard, bool> filter)
        {
            return cards.FirstOrDefault(card => card?.Data != null && card.IsFaceup() && filter.Invoke(card));
        }

        public static IList<ClientCard> GetMatchingCards(this IEnumerable<ClientCard> cards, Func<ClientCard, bool> filter)
        {
            return cards.Where(card => card?.Data != null && filter.Invoke(card)).ToList();
        }

        public static int GetMatchingCardsCount(this IEnumerable<ClientCard> cards, Func<ClientCard, bool> filter)
        {
            return cards.Count(card => card?.Data != null && filter.Invoke(card));
        }

        public static bool IsExistingMatchingCard(this IEnumerable<ClientCard> cards, Func<ClientCard, bool> filter, int count = 1)
        {
            return cards.GetMatchingCardsCount(filter) >= count;
        }

        public static ClientCard GetShouldBeDisabledBeforeItUseEffectMonster(this IEnumerable<ClientCard> cards, bool canBeTarget = true)
        {
            return cards.FirstOrDefault(card => card?.Data != null && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && (!canBeTarget || !card.IsShouldNotBeTarget()));
        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).GetCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}