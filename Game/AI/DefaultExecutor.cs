using System;
using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI
{
    public abstract class DefaultExecutor : Executor
    {
        protected class _CardId
        {
            public const int JizukirutheStarDestroyingKaiju = 63941210;
            public const int ThunderKingtheLightningstrikeKaiju = 48770333;
            public const int DogorantheMadFlameKaiju = 93332803;
            public const int RadiantheMultidimensionalKaiju = 28674152;
            public const int GadarlatheMysteryDustKaiju = 36956512;
            public const int KumongoustheStickyStringKaiju = 29726552;
            public const int GamecieltheSeaTurtleKaiju = 55063751;
            public const int SuperAntiKaijuWarMachineMechaDogoran = 84769941;

            public const int UltimateConductorTytanno = 18940556;

            public const int DupeFrog = 46239604;
            public const int MaraudingCaptain = 2460565;

            public const int BlackRoseDragon = 73580471;
            public const int JudgmentDragon = 57774843;
            public const int TopologicTrisbaena = 72529749;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int HarpiesFeatherDuster = 18144506;
            public const int DarkMagicAttack = 2314238;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int CosmicCyclone = 8267140;
            public const int ChickenGame = 67616300;

            public const int InfiniteTransience = 10045474;

            public const int CastelTheSkyblasterMusketeer = 82633039;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int NumberS39UtopiaTheLightning = 56832966;
            public const int Number39Utopia = 84013237;
            public const int UltimayaTzolkin = 1686814;

            public const int VampireFräulein = 6039967;
            public const int InjectionFairyLily = 79575620;

            public const int JackKnightOfTheLavenderDust = 28692962;
            public const int JackKnightOfTheCobaltDepths = 92204263;
            public const int JackKnightOfTheCrimsonLotus = 56809158;
            public const int JackKnightOfTheGoldenBlossom = 29415459;
            public const int JackKnightOfTheVerdantGale = 66022706;
            public const int JackKnightOfTheAmberShade = 93020401;
            public const int JackKnightOfTheAzureSky = 20537097;
            public const int MekkKnightMorningStar = 72006609;
            public const int JackKnightOfTheWorldScar = 38502358;
            public const int WhisperOfTheWorldLegacy = 62530723;
            public const int TrueDepthsOfTheWorldLegacy = 98935722;
            public const int KeyToTheWorldLegacy = 2930675;

            public const int BlueEyesChaosMAXDragon = 55410871;

            public const int EaterOfMillions = 63845230;
        }

        protected DefaultExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.ChickenGame, DefaultChickenGame);
        }

        public int ReverseBotEnemyZone(int zone=-1)
        {
            if (zone == 0)
                return 4;
            if (zone == 1)
                return 3;
            if (zone == 2)
                return 2;
            if (zone == 3)
                return 1;
            if (zone == 4)
                return 0;
            if (zone == 5)
                return 6;
            if (zone == 6)
                return 5;
            return -1;
        }

        public int GetReverseColumnMainZone(int zone=-1)
        {
            if (zone == 0)
                return 4;
            if (zone == 1 || zone == 5)
                return 3;
            if (zone == 2)
                return 2;
            if (zone == 3 || zone == 6)
                return 3;
            if (zone == 4)
                return 0;
            return -1;
        }
        public int ReverseZone16bitTo32bit(int zone=-1)
        {
            if (zone == Zones.z0)
                return 0;
            if (zone == Zones.z1)
                return 1;
            if (zone == Zones.z2)
                return 2;
            if (zone == Zones.z3)
                return 3;
            if (zone == Zones.z4)
                return 4;
            if (zone == Zones.z5)
                return 5;
            if (zone == Zones.z6)
                return 6;
            return -1;
        }
        public int ReverseZoneTo16bit(int zone=-1)
        {
            if (zone == 0)
                return Zones.z0;
            if (zone == 1)
                return Zones.z1;
            if (zone == 2)
                return Zones.z2;
            if (zone == 3)
                return Zones.z3;
            if (zone == 4)
                return Zones.z4;
            if (zone == 5)
                return Zones.z5;
            if (zone == 6)
                return Zones.z6;
            return -1;          
        }
        public bool IsJackKnightMonster(int enemyzone)
        {
            if (enemyzone == -1)
                return false;
            if (Enemy.MonsterZone[enemyzone] == null)
                return false;
            if (Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheAmberShade &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheAzureSky &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheCobaltDepths &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheCrimsonLotus &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheGoldenBlossom &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheLavenderDust &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheVerdantGale &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheWorldScar &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.MekkKnightMorningStar)
                return false ;
            return true;
        }

        public bool NoJackKnightColumn(int enemyzone)
        {
            if (Enemy.MonsterZone[enemyzone] == null &&
                (enemyzone == 2 || enemyzone == 4 || enemyzone == 0))
                return true;
            if(enemyzone==1 || enemyzone == 5)
            {
                if (Enemy.MonsterZone[1] != null &&(
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheAmberShade ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheAzureSky ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheCobaltDepths ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheCrimsonLotus ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheGoldenBlossom ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheLavenderDust ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheVerdantGale ||
                    Enemy.MonsterZone[1].Id == _CardId.JackKnightOfTheWorldScar ||
                    Enemy.MonsterZone[1].Id == _CardId.MekkKnightMorningStar))
                    return false;
                if (Enemy.MonsterZone[5] != null &&
                    (Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheAmberShade ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheAzureSky ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheCobaltDepths ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheCrimsonLotus ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheGoldenBlossom ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheLavenderDust ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheVerdantGale ||
                    Enemy.MonsterZone[5].Id == _CardId.JackKnightOfTheWorldScar ||
                    Enemy.MonsterZone[5].Id == _CardId.MekkKnightMorningStar))
                    return false;
                return true;
            }
            if (enemyzone == 3 || enemyzone == 6)
            {
                if (Enemy.MonsterZone[3] != null &&
                    (Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheAmberShade ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheAzureSky ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheCobaltDepths ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheCrimsonLotus ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheGoldenBlossom ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheLavenderDust ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheVerdantGale ||
                    Enemy.MonsterZone[3].Id == _CardId.JackKnightOfTheWorldScar ||
                    Enemy.MonsterZone[3].Id == _CardId.MekkKnightMorningStar))
                    return false;
                if (Enemy.MonsterZone[6] != null &&
                    (Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheAmberShade ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheAzureSky ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheCobaltDepths ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheCrimsonLotus ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheGoldenBlossom ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheLavenderDust ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheVerdantGale ||
                    Enemy.MonsterZone[6].Id == _CardId.JackKnightOfTheWorldScar ||
                    Enemy.MonsterZone[6].Id == _CardId.MekkKnightMorningStar))
                    return false;
                return true;
            }
            if (Enemy.MonsterZone[enemyzone] != null &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheAmberShade &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheAzureSky &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheCobaltDepths &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheCrimsonLotus &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheGoldenBlossom &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheLavenderDust &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheVerdantGale &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.JackKnightOfTheWorldScar &&
               Enemy.MonsterZone[enemyzone].Id != _CardId.MekkKnightMorningStar)
                return true;                
            return false;
        }        

        public bool SameMonsterColumn(int botzone,int enemyzone)
        {
            if (botzone == 4 && enemyzone == 0)
                return true;
            if ((botzone== 3 ||botzone == 6) && (enemyzone == 1 || enemyzone == 5))
                return true;
            if (botzone == 2 && enemyzone == 2)
                return true;
            if ((botzone == 1 || botzone == 5) && (enemyzone == 3 || enemyzone == 6))
                return true;
            if (botzone == 0 && enemyzone == 4)
                return true;
            return false;
        }

        /// <summary>
        /// Decide which card should the attacker attack.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defenders">Cards that defend.</param>
        /// <returns>BattlePhaseAction including the target, or null (in this situation, GameAI will check the next attacker)</returns>
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {        
            for (int i = 0; i < defenders.Count; ++i)
            {
                ClientCard defender = defenders[i];

                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();
                if (!OnPreBattleBetween(attacker, defender))
                    continue;
                if (AI.Utils.GetWorstEnemyMonster(true) != null)
                {
                    ClientCard FirstDefender = AI.Utils.GetWorstEnemyMonster(true);
                    if ((attacker.RealPower - FirstDefender.RealPower) >= Enemy.LifePoints && FirstDefender.IsAttack())
                    {
                        if (FirstDefender == Enemy.MonsterZone.GetDangerousMonster() && FirstDefender.IsDisabled())
                        {
                            if (defender != FirstDefender)
                                return null;
                        }
                        if (FirstDefender != Enemy.MonsterZone.GetDangerousMonster())
                        {
                            if (defender != FirstDefender)
                                return null;
                        }
                    }
                }
                if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            return null;
        }

        /// <summary>
        /// Decide whether to declare attack between attacker and defender.
        /// Can be overrided to update the RealPower of attacker for cards like Honest.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defender">Card that defend.</param>
        /// <returns>false if the attack shouldn't be done.</returns>
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (attacker.RealPower <= 0)
                return false;
            if (defender.Id == _CardId.JackKnightOfTheCobaltDepths &&
                (defender.Attack == 0 || defender.Attack == 1200))
                defender.RealPower += 2400;
            if (!attacker.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (defender.IsMonsterInvincible() && defender.IsDefense())
                    return false;

                if (defender.IsMonsterDangerous())
                {
                    bool canignoreit = false;
                    if (attacker.Id == _CardId.UltimateConductorTytanno && !attacker.IsDisabled() && defender.IsDefense())
                        canignoreit = true;
                    if (attacker.Id == _CardId.UltimateConductorTytanno && defender.IsFacedown())
                        canignoreit = true;
                    if (!canignoreit)
                        return false;
                }

                if (defender.Id == _CardId.CrystalWingSynchroDragon && defender.IsAttack() && !defender.IsDisabled() && attacker.Level >= 5)
                    return false;

                if (defender.Id == _CardId.NumberS39UtopiaTheLightning && defender.IsAttack() && !defender.IsDisabled() && defender.HasXyzMaterial(2, _CardId.Number39Utopia))
                    defender.RealPower = 5000;
                
                if (defender.Id == _CardId.VampireFräulein && !defender.IsDisabled())
                    defender.RealPower += (Enemy.LifePoints > 3000) ? 3000 : (Enemy.LifePoints - 100);

                if (defender.Id == _CardId.InjectionFairyLily && !defender.IsDisabled() && Enemy.LifePoints > 2000)
                    defender.RealPower += 3000;
            }

            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.Id == _CardId.NumberS39UtopiaTheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, _CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }

            if (Enemy.HasInMonstersZone(_CardId.DupeFrog, true) && defender.Id != _CardId.DupeFrog)
                return false;

            if (Enemy.HasInMonstersZone(_CardId.MaraudingCaptain, true) && defender.Id != _CardId.MaraudingCaptain && defender.Race == (int)CardRace.Warrior)
                return false;

            if (defender.Id == _CardId.UltimayaTzolkin && !defender.IsDisabled())
            {
                List<ClientCard> monsters = Enemy.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.HasType(CardType.Synchro))
                        return false;
                }
            }
            if (Enemy.HasInMonstersZone(_CardId.MekkKnightMorningStar))
            {
                int attackerzone = -1;
                int defenderzone = -1;
                for (int a = 0; a <= 6; a++)
                    for (int b = 0; b <= 6; b++)
                    {
                        if (Bot.MonsterZone[a] != null && Enemy.MonsterZone[b] != null &&
                            SameMonsterColumn(a, b) &&
                            Bot.MonsterZone[a].Id == attacker.Id && Enemy.MonsterZone[b].Id == defender.Id)
                        {
                            attackerzone = a;
                            defenderzone = b;
                        }
                    }               
                if (!SameMonsterColumn(attackerzone, defenderzone) && IsJackKnightMonster(defenderzone))
                {                  
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Called when the AI has to select a card position.
        /// </summary>
        /// <param name="cardId">Id of the card to position on the field.</param>
        /// <param name="positions">List of available positions.</param>
        /// <returns>Selected position, or 0 if no position is set for this card.</returns>
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack == 0)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }
              
        public override int OnSelectPlace(int cardId, int player, int location, int available)
        {
            //Logger.DebugWriteLine("%%%%%%WindBot.Global.m_zone = " + Duel.Global.m_zone);
            //Logger.DebugWriteLine("%%%%%%WindBot.Global.intial_zone = " + (byte)Duel.Global.intial_zone);
            //Logger.DebugWriteLine("%%%%%%sWindBot.Global.m_type = " + Duel.Global.m_type);
            Duel.Global.m_zone = 0;
            if ((Duel.Global.m_type == 1 || Duel.Global.m_type == Zones.MainMonsterZones) &&
                Enemy.HasInSpellZone(_CardId.TrueDepthsOfTheWorldLegacy))
            {
                for (int i = 4; i >= 0; i--)
                {
                    if (Bot.MonsterZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] ==null &&
                        NoJackKnightColumn(ReverseBotEnemyZone(ReverseZone16bitTo32bit(Duel.Global.intial_zone))))
                        break;
                    if (NoJackKnightColumn(i) && Bot.MonsterZone[ReverseBotEnemyZone(i)] == null)
                    {
                        Duel.Global.m_zone = ReverseZoneTo16bit(ReverseBotEnemyZone(i));
                        Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                        break;
                    }
                    Logger.DebugWriteLine("%%%%%%selsectplacecheck"+ i+ " = " + Duel.Global.m_zone);
                }                
            }
            if ((Duel.Global.m_type == 1 || Duel.Global.m_type == Zones.MainMonsterZones) && 
                Enemy.MonsterZone[5] != null &&
                Enemy.MonsterZone[5].HasLinkMarker((int)LinkMarker.Top))
            {                
                for(int i = 4; i >= 0; i--)
                {
                    if (Bot.MonsterZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] == null &&
                        Duel.Global.intial_zone != Zones.z3)
                        break;
                    if (i == 3) continue;
                    if (Bot.MonsterZone[i] == null)
                    {
                        Duel.Global.m_zone = ReverseZoneTo16bit(i);
                        Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                        break;
                    }
                    Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                }      
            }
            if ((Duel.Global.m_type == 1 || Duel.Global.m_type == Zones.MainMonsterZones) && 
                Enemy.MonsterZone[6] != null &&
                Enemy.MonsterZone[6].HasLinkMarker((int)LinkMarker.Top))
            {                
                for (int i = 4; i >= 0; i--)
                {
                    if (Bot.MonsterZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] == null &&
                        Duel.Global.intial_zone != Zones.z1)
                        break;
                    if (i == 1) continue;
                    if (Bot.MonsterZone[i] == null)
                    {
                        Duel.Global.m_zone = ReverseZoneTo16bit(i);
                        break;
                    }
                    Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                }
            }
            if ((Duel.Global.m_type == 2 || Duel.Global.m_type==(int)CardType.Spell) && 
                Enemy.HasInSpellZone(_CardId.WhisperOfTheWorldLegacy))
            {
                for (int i = 4; i >= 0; i--)
                {
                    //Logger.DebugWriteLine("%%%%%%NoJackKnightColumn" + i + " = " + ReverseBotEnemyZone(ReverseZone16bitTo32bit(Duel.Global.intial_zone)));
                    if (Bot.SpellZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] == null &&
                        NoJackKnightColumn(ReverseBotEnemyZone(ReverseZone16bitTo32bit(Duel.Global.intial_zone))))
                        break;
                    if (NoJackKnightColumn(i) && Bot.SpellZone[ReverseBotEnemyZone(i)] == null)
                    {                        
                        Duel.Global.m_zone = ReverseZoneTo16bit(ReverseBotEnemyZone(i));
                        Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                        break;
                    }
                    Logger.DebugWriteLine("%%%%%%selsectplacecheck" + i + " = " + Duel.Global.m_zone);
                }
            }
            if(((Duel.Global.m_type == 2 || Duel.Global.m_type == (int)CardType.Spell) ||
                (Duel.Global.m_type == 4 || Duel.Global.m_type == (int)CardType.Trap)) &&
                Duel.Global.InfiniteTransience_zone!=-1)
            {
                for(int i = 4; i >= 0; i--)
                {                   
                    if (Bot.SpellZone[i] == null && ReverseBotEnemyZone(i) != Duel.Global.InfiniteTransience_zone)
                        Duel.Global.m_zone = ReverseZoneTo16bit(i);
                }
            }
                   
            if ((Duel.Global.m_type == 4 || Duel.Global.m_type == (int)CardType.Trap) &&
                Enemy.HasInSpellZone(_CardId.KeyToTheWorldLegacy))
            {
                for (int i = 4; i >= 0 ; i--)
                {
                    
                    if (Bot.SpellZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] == null &&
                        NoJackKnightColumn(ReverseBotEnemyZone(ReverseZone16bitTo32bit(Duel.Global.intial_zone))))
                         break;
                    if (NoJackKnightColumn(i) && Bot.SpellZone[ReverseBotEnemyZone(i)] == null)
                    {
                        Duel.Global.m_zone = ReverseZoneTo16bit(ReverseBotEnemyZone(i));
                        break;
                    }
                }                
            }
            if ((Duel.Global.m_type == 3 || Duel.Global.m_type==Zones.ExtraMonsterZones) && 
                Enemy.HasInSpellZone(_CardId.TrueDepthsOfTheWorldLegacy))
            {
                for (int i = 5; i < 6; i++)
                {
                    if (Bot.MonsterZone[ReverseZone16bitTo32bit(Duel.Global.intial_zone)] == null &&
                        NoJackKnightColumn(ReverseBotEnemyZone(ReverseZone16bitTo32bit(Duel.Global.intial_zone)))
                        )
                        break;
                    if (NoJackKnightColumn(i) && Bot.MonsterZone[ReverseBotEnemyZone(i)] == null)
                    {
                        Duel.Global.m_zone = ReverseZoneTo16bit(ReverseBotEnemyZone(i));
                        break;
                    }
                }                
            }            
            if ((Duel.Global.m_zone & available) > 0)
                return Duel.Global.m_zone & available;
            return base.OnSelectPlace(cardId,player,location,available);
        }
        public override bool OnSelectBattleReplay()
        {
            if (Bot.BattlingMonster == null)
                return false;
            List<ClientCard> defenders = new List<ClientCard>(Duel.Fields[1].GetMonsters());
            defenders.Sort(AIFunctions.CompareDefensePower);
            defenders.Reverse();
            BattlePhaseAction result = OnSelectAttackTarget(Bot.BattlingMonster, defenders);
            if (result != null && result.Action == BattlePhaseAction.BattleAction.Attack)
            {
                return true;
            }
            return false;
        }

        public override void OnNewPhase()
        {
            
        }

        public override void OnChaining(int player, ClientCard card)
        {
            Duel.Global.InfiniteTransience_zone = -1;
            if (Enemy.HasInSpellZone(_CardId.InfiniteTransience))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].Id == _CardId.InfiniteTransience)
                        Duel.Global.InfiniteTransience_zone = i;
                }
                Logger.DebugWriteLine("*******HasInfiniteTransience*************");
            }
        }

        public override void OnChainEnd()
        {
            
        }

        /// <summary>
        /// Destroy face-down cards first, in our turn.
        /// </summary>
        protected bool DefaultMysticalSpaceTyphoon()
        {
            foreach (ClientCard card in Duel.CurrentChain)
                if (card.Id == _CardId.MysticalSpaceTyphoon)
                    return false;

            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;

            ClientCard selected = Enemy.SpellZone.GetFloodgate();

            if (selected == null)
            {
                foreach (ClientCard card in spells)
                {
                    if (Duel.Player == 1 && !card.HasType(CardType.Continuous))
                        continue;
                    selected = card;
                    if (Duel.Player == 0 && card.IsFacedown())
                        break;
                }
            }

            if (selected == null)
                return false;
            AI.SelectCard(selected);
            return true;
        }

        /// <summary>
        /// Destroy face-down cards first, in our turn.
        /// </summary>
        protected bool DefaultCosmicCyclone()
        {
            foreach (ClientCard card in Duel.CurrentChain)
                if (card.Id == _CardId.CosmicCyclone)
                    return false;
            return (Bot.LifePoints > 1000) && DefaultMysticalSpaceTyphoon();
        }

        /// <summary>
        /// Activate if avail.
        /// </summary>
        protected bool DefaultGalaxyCyclone()
        {
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;

            ClientCard selected = null;

            if (Card.Location == CardLocation.Grave)
            {
                selected = AI.Utils.GetBestEnemySpell(true);
            }
            else
            {
                foreach (ClientCard card in spells)
                {
                    if (card.IsFacedown())
                    {
                        selected = card;
                        break;
                    }
                }
            }

            if (selected == null)
                return false;

            AI.SelectCard(selected);
            return true;
        }

        /// <summary>
        /// Set the highest ATK level 4+ effect enemy monster.
        /// </summary>
        protected bool DefaultBookOfMoon()
        {
            if (AI.Utils.IsAllEnemyBetter(true))
            {
                ClientCard monster = Enemy.GetMonsters().GetHighestAttackMonster();
                if (monster != null && monster.HasType(CardType.Effect) && !monster.HasType(CardType.Link) && (monster.HasType(CardType.Xyz) || monster.Level > 4))
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return problematic monster, and if this card become target, return any enemy monster.
        /// </summary>
        protected bool DefaultCompulsoryEvacuationDevice()
        {
            ClientCard target = AI.Utils.GetProblematicEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            if (AI.Utils.IsChainTarget(Card))
            {
                ClientCard monster = AI.Utils.GetBestEnemyMonster();
                if (monster != null)
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Revive the best monster when we don't have better one in field.
        /// </summary>
        protected bool DefaultCallOfTheHaunted()
        {
            if (!AI.Utils.IsAllEnemyBetter(true))
                return false;
            ClientCard selected = null;
            int BestAtk = 0;
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card.Attack > BestAtk)
                {
                    BestAtk = card.Attack;
                    selected = card;
                }
            }
            AI.SelectCard(selected);
            return true;
        }
        /// <summary>
        /// Default InfiniteImpermanence effect
        /// </summary>
        protected bool DefaultInfiniteImpermanence()
        {           
            AI.SelectPlace(Zones.z2, 4);
            ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if (AI.Utils.GetLastChainCard() == check)
                    {
                        target = check;
                        break;
                    }
                }
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            if (Bot.BattlingMonster != null && Enemy.BattlingMonster != null)
            {
                if (Enemy.BattlingMonster.Id == _CardId.EaterOfMillions)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Chain the enemy monster, or disable monster like Rescue Rabbit.
        /// </summary>
        protected bool DefaultBreakthroughSkill()
        {
            if (!DefaultUniqueTrap())
                return false;

            if (Duel.Player == 1)
            {
                foreach (ClientCard target in Enemy.GetMonsters())
                {
                    if (target.IsMonsterShouldBeDisabledBeforeItUseEffect())
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
            }

            ClientCard LastChainCard = AI.Utils.GetLastChainCard();

            if (LastChainCard == null)
                return false;
            if (LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget())
                return false;
            AI.SelectCard(LastChainCard);
            return true;
        }

        /// <summary>
        /// Activate only except this card is the target or we summon monsters.
        /// </summary>
        protected bool DefaultSolemnJudgment()
        {
            return !AI.Utils.IsChainTargetOnly(Card) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnWarning()
        {
            return (Bot.LifePoints > 2000) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate only except we summon monsters.
        /// </summary>
        protected bool DefaultSolemnStrike()
        {
            return (Bot.LifePoints > 1500) && !(Duel.Player == 0 && Duel.LastChainPlayer == -1) && DefaultTrap();
        }

        /// <summary>
        /// Activate when all enemy monsters have better ATK.
        /// </summary>
        protected bool DefaultTorrentialTribute()
        {
            return !AI.Utils.HasChainedTrap(0) && AI.Utils.IsAllEnemyBetter(true);
        }

        /// <summary>
        /// Activate enemy have more S&T.
        /// </summary>
        protected bool DefaultHeavyStorm()
        {
            return Bot.GetSpellCount() < Enemy.GetSpellCount();
        }

        /// <summary>
        /// Activate before other winds, if enemy have more than 2 S&T.
        /// </summary>
        protected bool DefaultHarpiesFeatherDusterFirst()
        {
            return Enemy.GetSpellCount() >= 2;
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK.
        /// </summary>
        protected bool DefaultHammerShot()
        {
            return AI.Utils.IsOneEnemyBetter(true);
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultDarkHole()
        {
            return AI.Utils.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultRaigeki()
        {
            return AI.Utils.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when one enemy monsters have better ATK or DEF.
        /// </summary>
        protected bool DefaultSmashingGround()
        {
            return AI.Utils.IsOneEnemyBetter();
        }

        /// <summary>
        /// Activate when we have more than 15 cards in deck.
        /// </summary>
        protected bool DefaultPotOfDesires()
        {
            return Bot.Deck.Count > 15;
        }

        /// <summary>
        /// Set traps only and avoid block the activation of other cards.
        /// </summary>
        protected bool DefaultSpellSet()
        {
            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay)) && Bot.GetSpellCountWithoutField() < 4;
        }

        /// <summary>
        /// Summon with tributes ATK lower.
        /// </summary>
        protected bool DefaultTributeSummon()
        {
            if (!UniqueFaceupMonster())
                return false;
            int tributecount = (int)Math.Ceiling((Card.Level - 4.0d) / 2.0d);
            for (int j = 0; j < 7; ++j)
            {
                ClientCard tributeCard = Bot.MonsterZone[j];
                if (tributeCard == null) continue;
                if (tributeCard.GetDefensePower() < Card.Attack)
                    tributecount--;
            }
            return tributecount <= 0;
        }

        /// <summary>
        /// Activate when we have no field.
        /// </summary>
        protected bool DefaultField()
        {
            return Bot.SpellZone[5] == null;
        }

        /// <summary>
        /// Turn if all enemy is better.
        /// </summary>
        protected bool DefaultMonsterRepos()
        {
            if (Card.IsFaceup() && Card.IsDefense() && Card.Attack == 0)
                return false;
            if (Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon) &&
                Card.IsAttack() && (4000-Card.Defense)*2>(4000 - Card.Attack))
                return false;
            if (Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon) &&
                Card.IsDefense() && Card.IsFaceup() &&
               (4000 - Card.Defense) * 2 > (4000 - Card.Attack))
                return true;
            bool enemyBetter = AI.Utils.IsAllEnemyBetter(true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        /// <summary>
        /// if spell/trap is the target or enermy activate HarpiesFeatherDuster
        /// </summary>
        protected bool DefaultOnBecomeTarget()
        {
            if (AI.Utils.IsChainTarget(Card)) return true;
            if (AI.Utils.ChainContainsCard(_CardId.TopologicTrisbaena)) return true;
            if (AI.Utils.ChainContainsCard(_CardId.BlackRoseDragon)) return true;
            if (AI.Utils.ChainContainsCard(_CardId.JudgmentDragon)) return true;
            if (AI.Utils.ChainContainsCard(_CardId.EvilswarmExcitonKnight)) return true;
            if (Enemy.HasInSpellZone(_CardId.HarpiesFeatherDuster, true)) return true;
            if (Enemy.HasInSpellZone(_CardId.DarkMagicAttack, true)) return true;
            return false;
        }
        /// <summary>
        /// Chain enemy activation or summon.
        /// </summary>
        protected bool DefaultTrap()
        {
            return (Duel.LastChainPlayer == -1 && Duel.LastSummonPlayer != 0) || Duel.LastChainPlayer == 1;
        }

        /// <summary>
        /// Activate when avail and no other our trap card in this chain or face-up.
        /// </summary>
        protected bool DefaultUniqueTrap()
        {
            if (AI.Utils.HasChainedTrap(0))
                return false;

            return UniqueFaceupSpell();
        }

        /// <summary>
        /// Activate when avail and no other our trap card in this chain 
        /// </summary>
        protected bool DefaultTrapOnly()
        {
            if (AI.Utils.HasChainedTrap(0))
                return false;

            return true;
        }
        /// <summary>
        /// Check no other our spell or trap card with same name face-up.
        /// </summary>
        protected bool UniqueFaceupSpell()
        {
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.Id == Card.Id && card.IsFaceup())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check no other our monster card with same name face-up.
        /// </summary>
        protected bool UniqueFaceupMonster()
        {
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.Id == Card.Id && card.IsFaceup())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Dumb way to avoid the bot chain in mess.
        /// </summary>
        protected bool DefaultDontChainMyself()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Duel.LastChainPlayer != 0;
        }

        /// <summary>
        /// Draw when we have lower LP, or destroy it. Can be overrided.
        /// </summary>
        protected bool DefaultChickenGame()
        {
            int count = 0;
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    count++;
            }
            if (count > 1 || Bot.LifePoints <= 1000)
                return false;
            if (Bot.LifePoints <= Enemy.LifePoints && ActivateDescription == AI.Utils.GetStringId(_CardId.ChickenGame, 0))
                return true;
            if (Bot.LifePoints > Enemy.LifePoints && ActivateDescription == AI.Utils.GetStringId(_CardId.ChickenGame, 1))
                return true;
            return false;
        }

        /// <summary>
        /// Draw when we have Dark monster in hand,and banish random one. Can be overrided.
        /// </summary>
        protected bool DefaultAllureofDarkness()
        {
            IList<ClientCard> condition = Bot.Hand;
            IList<ClientCard> check = new List<ClientCard>();
            ClientCard con = null;
            foreach (ClientCard card in condition)
            {
                if (card.HasAttribute(CardAttribute.Dark))
                {
                    con = card;
                    break;
                }
            }
            if (con != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultDimensionalBarrier()
        {
            const int RITUAL = 0;
            const int FUSION = 1;
            const int SYNCHRO = 2;
            const int XYZ = 3;
            const int PENDULUM = 4;
            if (Duel.Player != 0)
            {
                List<ClientCard> monsters = Enemy.GetMonsters();
                int[] levels = new int[13];
                bool tuner = false;
                bool nontuner = false;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.HasType(CardType.Tuner))
                        tuner = true;
                    else if (!monster.HasType(CardType.Xyz))
                        nontuner = true;
                    if (monster.IsOneForXyz())
                    {
                        AI.SelectOption(XYZ);
                        return true;
                    }
                    levels[monster.Level] = levels[monster.Level] + 1;
                }
                if (tuner && nontuner)
                {
                    AI.SelectOption(SYNCHRO);
                    return true;
                }
                for (int i=1; i<=12; i++)
                {
                    if (levels[i]>1)
                    {
                        AI.SelectOption(XYZ);
                        return true;
                    }
                }
                ClientCard l = Enemy.SpellZone[6];
                ClientCard r = Enemy.SpellZone[7];
                if (l != null && r != null && l.LScale != r.RScale)
                {
                    AI.SelectOption(PENDULUM);
                    return true;
                }
            }
            ClientCard lastchaincard = AI.Utils.GetLastChainCard();
            if (Duel.LastChainPlayer == 1 && lastchaincard != null && !lastchaincard.IsDisabled())
            {
                if (lastchaincard.HasType(CardType.Ritual))
                {
                    AI.SelectOption(RITUAL);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Fusion))
                {
                    AI.SelectOption(FUSION);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Synchro))
                {
                    AI.SelectOption(SYNCHRO);
                    return true;
                }
                if (lastchaincard.HasType(CardType.Xyz))
                {
                    AI.SelectOption(XYZ);
                    return true;
                }
                if (lastchaincard.IsFusionSpell())
                {
                    AI.SelectOption(FUSION);
                    return true;
                }
            }
            if (AI.Utils.IsChainTarget(Card))
            {
                AI.SelectOption(XYZ);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clever enough
        /// </summary>
        protected bool DefaultInterruptedKaijuSlumber()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(new[]
                {
                    _CardId.GamecieltheSeaTurtleKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju,
                    _CardId.JizukirutheStarDestroyingKaiju,
                });
                return true;
            }
            AI.SelectCard(new[]
                {
                    _CardId.JizukirutheStarDestroyingKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GamecieltheSeaTurtleKaiju,
                });
            AI.SelectNextCard(new[]
                {
                    _CardId.SuperAntiKaijuWarMachineMechaDogoran,
                    _CardId.GamecieltheSeaTurtleKaiju,
                    _CardId.KumongoustheStickyStringKaiju,
                    _CardId.GadarlatheMysteryDustKaiju,
                    _CardId.RadiantheMultidimensionalKaiju,
                    _CardId.DogorantheMadFlameKaiju,
                    _CardId.ThunderKingtheLightningstrikeKaiju,
                    
                });
            return DefaultDarkHole();
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultKaijuSpsummon()
        {
            IList<int> kaijus = new[] {
                _CardId.JizukirutheStarDestroyingKaiju,
                _CardId.GadarlatheMysteryDustKaiju,
                _CardId.GamecieltheSeaTurtleKaiju,
                _CardId.RadiantheMultidimensionalKaiju,
                _CardId.KumongoustheStickyStringKaiju,
                _CardId.ThunderKingtheLightningstrikeKaiju,
                _CardId.DogorantheMadFlameKaiju,
                _CardId.SuperAntiKaijuWarMachineMechaDogoran
            };
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (kaijus.Contains(monster.Id))
                    return Card.GetDefensePower() > monster.GetDefensePower();
            }
            ClientCard card = Enemy.MonsterZone.GetFloodgate();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = Enemy.MonsterZone.GetDangerousMonster();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = AI.Utils.GetOneEnemyBetterThanValue(Card.GetDefensePower());
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Summon when we don't have monster attack higher than enemy's.
        /// </summary>
        protected bool DefaultNumberS39UtopiaTheLightningSummon()
        {
            int bestBotAttack = AI.Utils.GetBestAttack(Bot);
            return AI.Utils.IsOneEnemyBetterThanValue(bestBotAttack, false);
        }

        /// <summary>
        /// Activate if the card is attack pos, and its attack is below 5000, when the enemy monster is attack pos or not useless faceup defense pos
        /// </summary>
        protected bool DefaultNumberS39UtopiaTheLightningEffect()
        {
            return Card.IsAttack() && Card.Attack < 5000 && (Enemy.BattlingMonster.IsAttack() || Enemy.BattlingMonster.IsFacedown() || Enemy.BattlingMonster.GetDefensePower() >= Card.Attack);
        }

        /// <summary>
        /// Summon when it can and should use effect.
        /// </summary>
        protected bool DefaultEvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && DefaultEvilswarmExcitonKnightEffect();
        }

        /// <summary>
        /// Activate when we have less cards than enemy's, or the atk sum of we is lower than enemy's.
        /// </summary>
        protected bool DefaultEvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            if (selfCount < oppoCount)
                return true;

            int selfAttack = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                selfAttack += monster.GetDefensePower();
            }

            int oppoAttack = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                oppoAttack += monster.GetDefensePower();
            }

            return selfAttack < oppoAttack;
        }

        /// <summary>
        /// Summon in main2, or when the attack of we is lower than enemy's, but not when enemy have monster higher than 2500.
        /// </summary>
        protected bool DefaultStardustDragonSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot);
            int oppoBestAttack = AI.Utils.GetBestPower(Enemy);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 2500) || AI.Utils.IsTurn1OrMain2();
        }

        /// <summary>
        /// Negate enemy's destroy effect, and revive from grave.
        /// </summary>
        protected bool DefaultStardustDragonEffect()
        {
            return (Card.Location == CardLocation.Grave) || Duel.LastChainPlayer == 1;
        }

        /// <summary>
        /// Summon when enemy have card which we must solve.
        /// </summary>
        protected bool DefaultCastelTheSkyblasterMusketeerSummon()
        {
            return AI.Utils.GetProblematicEnemyCard() != null;
        }

        /// <summary>
        /// Bounce the problematic enemy card. Ignore the 1st effect.
        /// </summary>
        protected bool DefaultCastelTheSkyblasterMusketeerEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId(_CardId.CastelTheSkyblasterMusketeer, 0))
                return false;
            ClientCard target = AI.Utils.GetProblematicEnemyCard();
            if (target != null)
            {
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Summon when it should use effect, or when the attack of we is lower than enemy's, but not when enemy have monster higher than 3000.
        /// </summary>
        protected bool DefaultScarlightRedDragonArchfiendSummon()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot);
            int oppoBestAttack = AI.Utils.GetBestPower(Enemy);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || DefaultScarlightRedDragonArchfiendEffect();
        }

        /// <summary>
        /// Activate when we have less monsters than enemy, or when enemy have more than 3 monsters.
        /// </summary>
        protected bool DefaultScarlightRedDragonArchfiendEffect()
        {
            int selfCount = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                // The bot don't know if the card is special summoned, so let we assume all monsters are special summoned
                if (!monster.Equals(Card) && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    selfCount++;
            }

            int oppoCount = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    oppoCount++;
            }

            return (oppoCount > 0 && selfCount <= oppoCount) || oppoCount >= 3;
        }

        /// <summary>
        /// Clever enough.
        /// </summary>
        protected bool DefaultHonestEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return Bot.BattlingMonster.IsAttack() &&
                    (((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Attack) || Bot.BattlingMonster.Attack >= Enemy.LifePoints)
                    || ((Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Defense) && (Bot.BattlingMonster.Attack + Enemy.BattlingMonster.Attack > Enemy.BattlingMonster.Defense)));
            }
            else return AI.Utils.IsTurn1OrMain2();
        }


    }
}
