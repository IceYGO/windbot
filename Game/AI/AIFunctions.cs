using System.Collections.Generic;

namespace WindBot.Game.AI
{
    public class AIFunctions
    {
        public Duel Duel { get; private set; }

        public AIFunctions(Duel duel)
        {
            Duel = duel;
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

        public bool IsEnnemyBetter(bool onlyatk, bool all)
        {
            if (Duel.Fields[1].GetMonsterCount() == 0)
                return false;
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            monsters.Sort(CompareCardAttack);
            int bestAtk = -1;
            if (monsters.Count > 0)
                bestAtk = monsters[monsters.Count - 1].Attack;
            if (all)
                return IsAllEnnemyBetterThanValue(bestAtk, onlyatk);
            return IsOneEnnemyBetterThanValue(bestAtk, onlyatk);
        }

        public bool IsOneEnnemyBetterThanValue(int value, bool onlyatk)
        {
            int bestValue = -1;
            bool nomonster = true;
            for (int i = 0; i < 5; ++i)
            {
                ClientCard card = Duel.Fields[1].MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                nomonster = false;
                int ennemyValue = card.GetDefensePower();
                if (ennemyValue > bestValue)
                    bestValue = ennemyValue;
            }
            if (nomonster) return false;
            return bestValue > value;
        }

        public bool IsAllEnnemyBetterThanValue(int value, bool onlyatk)
        {
            bool nomonster = true;
            for (int i = 0; i < 5; ++i)
            {
                ClientCard card = Duel.Fields[1].MonsterZone[i];
                if (card == null || card.Data == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                nomonster = false;
                int ennemyValue = card.GetDefensePower();
                if (ennemyValue <= value)
                    return false;
            }
            return !nomonster;
        }
    }
}