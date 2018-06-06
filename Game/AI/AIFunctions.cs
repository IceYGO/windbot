using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;
namespace WindBot.Game.AI
{
    public class AIFunctions
    {
        public Duel Duel { get; private set; }
        public ClientField Bot { get; private set; }
        public ClientField Enemy { get; private set; }
        private static int bot_0_co_count;
        private static int bot_1_co_count;
        private static int bot_2_co_count;
        private static int bot_3_co_count;
        private static int bot_4_co_count;
        private static int bot_5_co_count;
        private static int bot_6_co_count;
        private static int enemy_0_co_count;
        private static int enemy_1_co_count;
        private static int enemy_2_co_count;
        private static int enemy_3_co_count;
        private static int enemy_4_co_count;
        private static int enemy_5_co_count;
        private static int enemy_6_co_count;
        private class __CardId
        {
            public const int DarkMagician = 46986414;
            public const int DarkMagicianTheDragonKnight = 41721210;
            public const int EternalSoul = 48680970;
        }
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
                if (card == null || card.Data == null) continue;
                if (onlyATK && card.IsDefense()) continue;
                int newPower = card.GetDefensePower();
                if (newPower > bestPower)
                    bestPower = newPower;
            }
            return bestPower;
        }

        public ClientCard GetLastSummonMonster()
        {
            if (Duel.LastSummonPlayer != -1)
                return Duel.LastSummonMonster;
            return null;
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
                if (card == null || card.Data == null) continue;
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
                if (card == null || card.Data == null) continue;
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

        public ClientCard GetEquipedMonster(int Id)
        {

            ClientCard card = null;
            return card;
        }

        public ClientCard GetBestBotMonster(bool onlyATK = false)
        {
            int bestPower = -1;
            ClientCard bestMonster = null;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Bot.MonsterZone[i];
                if (card == null || card.Data == null) continue;
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
                if (card == null || card.Data == null) continue;
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
                if (card == null || card.Data == null || 
                    (canBeTarget && card.IsShouldNotBeTarget()) ||
                    card == IsUnffectCardWithCondition(canBeTarget)
                    ) continue;
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

        

        public ClientCard IsUnffectCardWithCondition(bool canBeTarget)
        {
            if (!canBeTarget) return null;
            ClientCard card = null;
            if(Enemy.HasInSpellZone(__CardId.EternalSoul))
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if (check.Id == __CardId.DarkMagician)
                        card = check;
                    if (check.Id == __CardId.DarkMagicianTheDragonKnight)
                        card = check;

                }
            }
            return card;
        }


        public ClientCard GetOneEnemyBetterThanMyBest(bool onlyATK = false, bool canBeTarget = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return GetOneEnemyBetterThanValue(bestBotPower, onlyATK, canBeTarget);
        }

        public ClientCard GetProblematicEnemyCard(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null && card!=IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.SpellZone.GetFloodgate(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            if (attack == 0)
                attack = GetBestAttack(Bot);
            return GetOneEnemyBetterThanValue(attack, true, canBeTarget);
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
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
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = GetBestEnemySpell(onlyFaceup);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            return null;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null && card != IsUnffectCardWithCondition(canBeTarget))
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
                if (card == null || card.Data == null) continue;
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

        private int Math_Pow(int num,int n)
        {
            int result = num;
            for (int i = 0; i < n-1; i++)
                result *= result;
            return result;
        }

        private void UpdateMutualZoneCount()
        {
            int temp = Zones.CheckMutualBotZoneCount;
            temp |= bot_6_co_count;
            temp |= (bot_5_co_count << 3);
            temp |= (bot_4_co_count << 6);
            temp |= (bot_3_co_count << 9);
            temp |= (bot_2_co_count << 12);
            temp |= (bot_1_co_count << 15);
            temp |= (bot_0_co_count << 18);
            Zones.CheckMutualBotZoneCount = temp;
            temp = Zones.CheckMutualEnemyZoneCount;
            temp |= enemy_6_co_count;
            temp |= (enemy_5_co_count << 3);
            temp |= (enemy_4_co_count << 6);
            temp |= (enemy_3_co_count << 9);
            temp |= (enemy_2_co_count << 12);
            temp |= (enemy_1_co_count << 15);
            temp |= (enemy_0_co_count << 18);
            Zones.CheckMutualEnemyZoneCount = temp;
        }

        private void UpdateColinkZone(int zone,int player)
        {
            if(player==1)
            {
                if (zone == 0)
                    enemy_0_co_count++;
                if (zone == 1)
                    enemy_1_co_count++;
                if (zone == 2)
                    enemy_2_co_count++;
                if (zone == 3)
                    enemy_3_co_count++;
                if (zone == 4)
                    enemy_4_co_count++;
                if (zone == 5)
                    enemy_5_co_count++;
                if (zone == 6)
                    enemy_6_co_count++;
            }
            if(player==0)
            {
                if (zone == 0)
                    bot_0_co_count++;
                if (zone == 1)
                    bot_1_co_count++;
                if (zone == 2)
                    bot_2_co_count++;
                if (zone == 3)
                    bot_3_co_count++;
                if (zone == 4)
                    bot_4_co_count++;
                if (zone == 5)
                    bot_5_co_count++;
                if (zone == 6)
                    bot_6_co_count++;
            }
        }
        public void UpdateLinkedZone()
        {
            int temp = Zones.CheckLinkedPointZones;
            int temp_1 = Zones.CheckMutualBotZoneCount;
            int temp_2 = Zones.CheckMutualEnemyZoneCount;
            bot_0_co_count = 0;
            bot_1_co_count = 0;
            bot_2_co_count = 0;
            bot_3_co_count = 0;
            bot_4_co_count = 0;
            bot_5_co_count = 0;
            bot_6_co_count = 0;
            enemy_0_co_count = 0;
            enemy_1_co_count = 0;
            enemy_2_co_count = 0;
            enemy_3_co_count = 0;
            enemy_4_co_count = 0;
            enemy_5_co_count = 0;
            enemy_6_co_count = 0;
            for (int i=0;i<7;i++)
            {
                if(Enemy.MonsterZone[i]!=null && Enemy.MonsterZone[i].HasType(CardType.Link))
                {                    
                    ClientCard card = Enemy.MonsterZone[i];
                    if (i > 0 && i <= 4 && card.HasLinkMarker((int)LinkMarker.Left))
                    {
                        temp |= Math_Pow(2, i - 1);
                        if (Enemy.MonsterZone[i - 1]!=null &&
                            Enemy.MonsterZone[i - 1].HasLinkMarker((int)LinkMarker.Right))
                            UpdateColinkZone(i, 1);
                    }  
                    
                    if (i <= 3 && card.HasLinkMarker((int)LinkMarker.Right))
                    {
                        temp |= Math_Pow(2, i + 1);
                        if (Enemy.MonsterZone[i + 1]!=null &&
                            Enemy.MonsterZone[i + 1].HasLinkMarker((int)LinkMarker.Right))
                            UpdateColinkZone(i, 1);
                    }

                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.Bottom))
                    {
                        temp |= Math_Pow(2, 1);
                        if (Enemy.MonsterZone[1]!=null &&
                            Enemy.MonsterZone[1].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 1);
                    }                     

                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.Bottom))
                    {
                        temp |= Math_Pow(2, 3);
                        if (Enemy.MonsterZone[3] != null &&
                            Enemy.MonsterZone[3].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 1);
                    }                      

                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.BottomLeft))
                    {
                        temp |= Math_Pow(2, 0);
                        if (Enemy.MonsterZone[0] != null &&
                            Enemy.MonsterZone[0].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.BottomLeft))
                    {
                        temp |= Math_Pow(2, 4);
                        if (Enemy.MonsterZone[4] != null &&
                            Enemy.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.BottomRight))
                    {
                        temp |= Math_Pow(2, 2);
                        if (Enemy.MonsterZone[2] != null &&
                            Enemy.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.BottomRight))
                    {
                        temp |= Math_Pow(2, 4);
                        if (Enemy.MonsterZone[4] != null &&
                            Enemy.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.Top))
                    {
                        temp |= (Math_Pow(2, 3) << 8);
                        if (Bot.MonsterZone[3] != null &&
                            Bot.MonsterZone[3].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.Top))
                    {
                        temp |= (Math_Pow(2, 1) << 8);
                        if (Bot.MonsterZone[1] != null &&
                            Bot.MonsterZone[1].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.TopLeft))
                    {
                        temp |= (Math_Pow(2, 4) << 8);
                        if (Bot.MonsterZone[4] != null &&
                            Bot.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.TopLeft))
                    {
                        temp |= (Math_Pow(2, 2) << 8);
                        if (Bot.MonsterZone[2] != null &&
                            Bot.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.TopRight))
                    {
                        temp |= (Math_Pow(2, 2) << 8);
                        if (Bot.MonsterZone[2] != null &&
                            Bot.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.TopRight))
                    {
                        temp |= (Math_Pow(2, 0) << 8);
                        if (Bot.MonsterZone[0] != null &&
                            Bot.MonsterZone[0].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 0);
                    }
                        
                }
                if (Bot.MonsterZone[i] != null && Bot.MonsterZone[i].HasType(CardType.Link))
                {                   

                    ClientCard card = Bot.MonsterZone[i];
                    if (i > 0 && i <= 4 && card.HasLinkMarker((int)LinkMarker.Left))
                    {
                        temp |= (Math_Pow(2, i - 1) << 8);
                        if(Bot.MonsterZone[i-1] != null &&
                            Bot.MonsterZone[i-1].HasLinkMarker((int)LinkMarker.Right))
                        UpdateColinkZone(i, 0);
                    }
                        
                    if (i <= 3 && card.HasLinkMarker((int)LinkMarker.Right))
                    {
                        temp |= (Math_Pow(2, i + 1) << 8);
                        if(Bot.MonsterZone[i+1] != null &&
                            Bot.MonsterZone[i+1].HasLinkMarker((int)LinkMarker.Left))
                        UpdateColinkZone(i, 0);
                    }                        

                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.Bottom))
                    {
                        temp |= (Math_Pow(2, 1) << 8);
                        if (Bot.MonsterZone[1] != null &&
                            Bot.MonsterZone[1].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.Bottom))
                    {
                        temp |= (Math_Pow(2, 3) << 8);
                        if (Bot.MonsterZone[3] != null &&
                            Bot.MonsterZone[3].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 0);
                    }
                       
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.BottomLeft))
                    {
                        temp |= (Math_Pow(2, 0) << 8);
                        if (Bot.MonsterZone[0] != null &&
                            Bot.MonsterZone[0].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.BottomLeft))
                    {
                        temp |= (Math_Pow(2, 4) << 8);
                        if (Bot.MonsterZone[4] != null &&
                            Bot.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 0);
                    }
                       
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.BottomRight))
                    {
                        temp |= (Math_Pow(2, 2) << 8);
                        if (Bot.MonsterZone[2] != null &&
                            Bot.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.BottomRight))
                    {
                        temp |= (Math_Pow(2, 4) << 8);
                        if (Bot.MonsterZone[4] != null &&
                            Bot.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 0);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.Top))
                    {
                        temp |= Math_Pow(2, 3);
                        if (Enemy.MonsterZone[4] != null &&
                            Enemy.MonsterZone[4].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.Top))
                    {
                        temp |= Math_Pow(2, 1);
                        if (Enemy.MonsterZone[3] != null &&
                            Enemy.MonsterZone[1].HasLinkMarker((int)LinkMarker.Top))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.TopLeft))
                    {
                        temp |= Math_Pow(2, 4);
                        if (Enemy.MonsterZone[4] != null &&
                            Enemy.MonsterZone[4].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.TopLeft))
                    {
                        temp |= Math_Pow(2, 2);
                        if (Enemy.MonsterZone[2] != null &&
                            Enemy.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopLeft))
                            UpdateColinkZone(i, 1);
                    }
                        
                    if (i == 5 && card.HasLinkMarker((int)LinkMarker.TopRight))
                    {
                        temp |= Math_Pow(2, 2);
                        if (Enemy.MonsterZone[2] != null &&
                            Enemy.MonsterZone[2].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 1);
                    }
                       
                    if (i == 6 && card.HasLinkMarker((int)LinkMarker.TopRight))
                    {
                        temp |= Math_Pow(2, 0);
                        if (Enemy.MonsterZone[0] != null &&
                            Enemy.MonsterZone[0].HasLinkMarker((int)LinkMarker.TopRight))
                            UpdateColinkZone(i, 1);
                    }
                        
                }
            }
            //Logger.DebugWriteLine("temp= "+temp);
            Zones.CheckLinkedPointZones = temp;
            UpdateMutualZoneCount();
        }

        public int GetCoLinkCount(ClientCard card, int player,int zone=-1)
        {
            if (!card.HasType(CardType.Link))
                return 0;           
            UpdateLinkedZone();
            int count = 0;
            int checkzone = -1;
            if(player==1)
            {
                if(zone==-1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (Enemy.MonsterZone[i] == card)
                        {
                            checkzone = i;
                            break;
                        }
                    }
                }
                Logger.DebugWriteLine("checkzone= " + checkzone);                
                if (checkzone == 6)
                {
                    /*count = Zones.CheckMutualEnemyZoneCount << 19;
                    Logger.DebugWriteLine("count= "+count);
                    count = count >> 19;*/
                    count = enemy_6_co_count;
                }
                if (checkzone == 0)
                {                    
                    count = (Zones.CheckMutualEnemyZoneCount) >> 19;
                    count = enemy_0_co_count;
                }
                if (checkzone == 1)
                    count = enemy_1_co_count;
                if (checkzone == 2)
                    count = enemy_2_co_count;
                if (checkzone == 3)
                    count = enemy_3_co_count;
                if (checkzone == 4)
                    count = enemy_4_co_count;
                if (checkzone == 5)
                    count = enemy_5_co_count;
                    
               /* else
                {
                    Logger.DebugWriteLine("count= " + Zones.CheckMutualEnemyZoneCount);
                    count = Zones.CheckMutualEnemyZoneCount >> 1;
                    Logger.DebugWriteLine("count= " + count);
                    count <<= 2;
                    Logger.DebugWriteLine("count= " + count);
                    count >>= 1;
                    Logger.DebugWriteLine("count= " + count);
                    count = Zones.CheckMutualEnemyZoneCount << checkzone * 3 + 1;
                    Logger.DebugWriteLine("count= " + count);
                    count = count >> 19;
                }*/ 
            }
            if (player == 0)
            {
                if (zone == -1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (Bot.MonsterZone[i] == card)
                        {
                            checkzone = i;
                            break;
                        }
                    }
                }
                if (checkzone == 6)
                {
                    count = Zones.CheckMutualBotZoneCount << 19;
                    Logger.DebugWriteLine("count= " + count);
                    count = count >> 19;
                }
                if (checkzone == 0)
                    count = Zones.CheckMutualBotZoneCount >> 19;
                else
                {
                    count = Zones.CheckMutualBotZoneCount << checkzone * 3;
                    Logger.DebugWriteLine("count= " + count);
                    count = count >> 19;
                }
            }
            Logger.DebugWriteLine("count= " + count);
            return (int)count;
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
            foreach (ClientCard target in Duel.ChainTargets)
            {
                if (card.Equals(target))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsChainTargetOnly(ClientCard card)
        {
            return Duel.ChainTargets.Count == 1 && card.Equals(Duel.ChainTargets[0]);
        }

        public bool ChainContainsCard(int id)
        {
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Id == id)
                    return true;
            }
            return false;
        }

        public int ChainCountPlayer(int player)
        {
            int count = 0;
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Controller == player)
                    count++;
            }
            return count;
        }

        public bool ChainContainPlayer(int player)
        {            
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Controller == player)
                    return true;
            }
            return false;
        }

        public bool HasChainedTrap(int player)
        {
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Controller == player && card.HasType(CardType.Trap))
                    return true;
            }
            return false;
        }

        public ClientCard GetLastChainCard()
        {
            if (Duel.CurrentChain.Count > 0)
                return Duel.CurrentChain[Duel.CurrentChain.Count - 1];
            return null;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public void SelectPreferredCards(IList<ClientCard> selected, ClientCard preferred, IList<ClientCard> cards, int min, int max)
        {
            if (cards.IndexOf(preferred) > 0 && selected.Count < max)
            {
                selected.Add(preferred);
            }
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public void SelectPreferredCards(IList<ClientCard> selected, int preferred, IList<ClientCard> cards, int min, int max)
        {
            foreach (ClientCard card in cards)
            {
                if (card.Id== preferred && selected.Count < max)
                    selected.Add(card);
            }
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public void SelectPreferredCards(IList<ClientCard> selected, IList<ClientCard> preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> avail = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                // clone
                avail.Add(card);
            }
            while (preferred.Count > 0 && avail.IndexOf(preferred[0]) > 0 && selected.Count < max)
            {
                ClientCard card = preferred[0];
                preferred.Remove(card);
                avail.Remove(card);
                selected.Add(card);
            }
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public void SelectPreferredCards(IList<ClientCard> selected, IList<int> preferred, IList<ClientCard> cards, int min, int max)
        {
            for (int i = 0; i < preferred.Count; i++)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.Id == preferred[i] && selected.Count < max && selected.IndexOf(card) <= 0)
                        selected.Add(card);
                }
                if (selected.Count >= max)
                    break;
            }
        }

        /// <summary>
        /// Check and fix selected to make sure it meet the count requirement.
        /// </summary>
        public void CheckSelectCount(IList<ClientCard> selected, IList<ClientCard> cards, int min, int max)
        {
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
        }
    }
}