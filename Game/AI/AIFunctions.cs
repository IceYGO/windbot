using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

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

        public int GetBestAttack(ClientField field, bool onlyatk)
        {
            int bestAtk = -1;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = field.MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                int ennemyValue = card.GetDefensePower();
                if (ennemyValue > bestAtk)
                    bestAtk = ennemyValue;
            }
            return bestAtk;
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
            for (int i = 0; i < 7; ++i)
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
            for (int i = 0; i < 7; ++i)
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

        public ClientCard GetOneEnnemyBetterThanValue(int value, bool onlyatk)
        {
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Duel.Fields[1].MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                int ennemyValue = card.GetDefensePower();
                if (ennemyValue >= value)
                    return card;
            }
            return null;
        }

        public ClientCard GetProblematicCard(int attack = 0)
        {
            ClientCard card = Duel.Fields[1].MonsterZone.GetInvincibleMonster();
            if (card != null)
                return card;
            card = Duel.Fields[1].MonsterZone.GetFloodgate();
            if (card != null)
                return card;
            card = Duel.Fields[1].SpellZone.GetFloodgate();
            if (card != null)
                return card;
            if (attack == 0)
                attack = GetBestAttack(Duel.Fields[0], true);
            return GetOneEnnemyBetterThanValue(attack, true);
        }

        public ClientCard GetProblematicMonsterCard(int attack = 0)
        {
            ClientCard card = Duel.Fields[1].MonsterZone.GetInvincibleMonster();
            if (card != null)
                return card;
            card = Duel.Fields[1].MonsterZone.GetFloodgate();
            if (card != null)
                return card;
            if (attack == 0)
                attack = GetBestAttack(Duel.Fields[0], true);
            return GetOneEnnemyBetterThanValue(attack, true);
        }

        public ClientCard GetProblematicSpellCard()
        {
            ClientCard card = Duel.Fields[1].SpellZone.GetNegateAttackSpell();
            if (card != null)
                return card;
            card = Duel.Fields[1].SpellZone.GetFloodgate();
            return card;
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
    }
}