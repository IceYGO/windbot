using System.Collections.Generic;
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

        public int GetBestAttack(ClientField field, bool onlyatk)
        {
            int bestAtk = -1;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = field.MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                int enemyValue = card.GetDefensePower();
                if (enemyValue > bestAtk)
                    bestAtk = enemyValue;
            }
            return bestAtk;
        }

        public bool IsEnemyBetter(bool onlyatk, bool all)
        {
            if (Enemy.GetMonsterCount() == 0)
                return false;
            List<ClientCard> monsters = Bot.GetMonsters();
            monsters.Sort(CompareCardAttack);
            int bestAtk = -1;
            if (monsters.Count > 0)
                bestAtk = monsters[monsters.Count - 1].Attack;
            if (all)
                return IsAllEnemyBetterThanValue(bestAtk, onlyatk);
            return IsOneEnemyBetterThanValue(bestAtk, onlyatk);
        }

        public bool IsOneEnemyBetterThanValue(int value, bool onlyatk)
        {
            int bestValue = -1;
            bool nomonster = true;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                nomonster = false;
                int enemyValue = card.GetDefensePower();
                if (enemyValue > bestValue)
                    bestValue = enemyValue;
            }
            if (nomonster) return false;
            return bestValue > value;
        }

        public bool IsAllEnemyBetterThanValue(int value, bool onlyatk)
        {
            bool nomonster = true;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card == null || card.Data == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                nomonster = false;
                int enemyValue = card.GetDefensePower();
                if (enemyValue <= value)
                    return false;
            }
            return !nomonster;
        }

        public ClientCard GetOneEnemyBetterThanValue(int value, bool onlyatk)
        {
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Enemy.MonsterZone[i];
                if (card == null) continue;
                if (onlyatk && card.IsDefense()) continue;
                int enemyValue = card.GetDefensePower();
                if (enemyValue >= value)
                    return card;
            }
            return null;
        }

        public ClientCard GetAnyEnemyMonster()
        {
            List<ClientCard> monsters = Enemy.GetMonsters();
            ClientCard hmonster = monsters.GetHighestAttackMonster();
            if (hmonster != null)
            {
                return hmonster;
            }
            foreach (ClientCard monster in monsters)
            {
                return monster;
            }
            return null;
        }

        public ClientCard GetProblematicCard(int attack = 0)
        {
            ClientCard card = Enemy.MonsterZone.GetInvincibleMonster();
            if (card != null)
                return card;
            card = Enemy.MonsterZone.GetFloodgate();
            if (card != null)
                return card;
            card = Enemy.SpellZone.GetFloodgate();
            if (card != null)
                return card;
            if (attack == 0)
                attack = GetBestAttack(Bot, true);
            return GetOneEnemyBetterThanValue(attack, true);
        }

        public ClientCard GetBestEnemyCard()
        {
            ClientCard card = GetProblematicCard();
            if (card != null)
                return card;
            card = Enemy.MonsterZone.GetHighestAttackMonster();
            if (card != null)
                return card;
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count > 0)
                return spells[0];
            return null;
        }

        public ClientCard GetProblematicMonsterCard(int attack = 0)
        {
            ClientCard card = Enemy.MonsterZone.GetInvincibleMonster();
            if (card != null)
                return card;
            card = Enemy.MonsterZone.GetFloodgate();
            if (card != null)
                return card;
            if (attack == 0)
                attack = GetBestAttack(Bot, true);
            return GetOneEnemyBetterThanValue(attack, true);
        }

        public ClientCard GetProblematicSpellCard()
        {
            ClientCard card = Enemy.SpellZone.GetNegateAttackSpell();
            if (card != null)
                return card;
            card = Enemy.SpellZone.GetFloodgate();
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