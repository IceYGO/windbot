using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("DarkMagician", "AI_DarkMagician")]
    public class DarkMagicianExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DarkMagician = 46986414;
            public const int GrinderGolem = 75732622;
            public const int MagicianOfLllusion = 35191415;
            public const int ApprenticeLllusionMagician = 30603688;
            public const int WindwitchGlassBell = 71007216;
            public const int MagiciansRod = 7084129;
            public const int WindwitchIceBell = 43722862;
            public const int AshBlossom = 14558127;
            public const int SpellbookMagicianOfProphecy = 14824019;
            public const int MaxxC = 23434538;
            public const int WindwitchSnowBell = 70117860;

            public const int TheEyeOfTimaeus = 1784686;
            public const int DarkMagicAttack = 2314238;
            public const int SpellbookOfKnowledge = 23314220;
            public const int UpstartGoblin = 70368879;
            public const int SpellbookOfSecrets = 89739383;
            public const int DarkMagicInheritance = 41735184;
            public const int LllusionMagic = 73616671;
            //public const int Scapegoat = 73915051;
            public const int DarkMagicalCircle = 47222536;
            public const int WonderWand = 67775894;
            public const int MagicianNavigation = 7922915;
            public const int EternalSoul = 48680970;
            public const int SolemnStrike = 40605147;

            public const int DarkMagicianTheDragonKnight = 41721210;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int OddEyesWingDragon = 58074177;
            public const int ClearWingFastDragon = 90036274;
            public const int WindwitchWinterBell = 14577226;
            public const int OddEyesAbsoluteDragon = 16691074;
            public const int Dracossack = 22110647;
            public const int BigEye = 80117527;
            public const int TroymarePhoenix = 2857636;
            public const int TroymareCerberus = 75452921;
            public const int ApprenticeWitchling = 71384012;
            public const int VentriloauistsClaraAndLucika = 1482001;
            /*public const int EbonLllusionMagician = 96471335;
            public const int BorreloadDragon = 31833038;
            public const int SaryujaSkullDread = 74997493;
            public const int Hidaruma = 64514892;
            public const int AkashicMagician = 28776350;
            public const int SecurityDragon = 99111753;
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;*/

            public const int HarpiesFeatherDuster = 18144506;
            public const int ElShaddollWinda = 94977269;
            public const int DarkHole = 53129443;
            public const int Ultimate = 86221741;
            public const int LockBird = 94145021;
            public const int Ghost = 59438930;
            public const int GiantRex = 80280944;
            public const int UltimateConductorTytanno = 18940556;
            public const int SummonSorceress = 61665245;
            public const int CrystronNeedlefiber = 50588353;
            public const int FirewallDragon = 5043010;
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
        }

        public DarkMagicianExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrikeeff);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, ChainEnemy);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCeff);
            //AddExecutor(ExecutorType.Activate, CardId.Scapegoat,Scapegoateff);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin, UpstartGoblineff);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicalCircle, DarkMagicalCircleeff);
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfSecrets, SpellbookOfSecreteff);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicInheritance, DarkMagicInheritanceeff);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicAttack, DarkMagicAttackeff);
            //trap set
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike);
            AddExecutor(ExecutorType.SpellSet, CardId.MagicianNavigation, MagicianNavigationset);
            AddExecutor(ExecutorType.SpellSet, CardId.EternalSoul, EternalSoulset);
            /*AddExecutor(ExecutorType.SpellSet, CardId.Scapegoat, Scapegoatset);            
            //sheep
            AddExecutor(ExecutorType.SpSummon, CardId.Hidaruma, Hidarumasp);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonsp);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragoneff);*/
            //plan A             
            AddExecutor(ExecutorType.Activate, CardId.WindwitchIceBell, WindwitchIceBelleff);
            AddExecutor(ExecutorType.Activate, CardId.WindwitchGlassBell, WindwitchGlassBelleff);
            AddExecutor(ExecutorType.Activate, CardId.WindwitchSnowBell, WindwitchSnowBellsp);
            AddExecutor(ExecutorType.SpSummon, CardId.WindwitchWinterBell, WindwitchWinterBellsp);
            AddExecutor(ExecutorType.Activate, CardId.WindwitchWinterBell, WindwitchWinterBelleff);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragonsp);
            // if fail
            AddExecutor(ExecutorType.SpSummon, CardId.ClearWingFastDragon, ClearWingFastDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.ClearWingFastDragon, ClearWingFastDragoneff);
            // plan B
            //AddExecutor(ExecutorType.Activate, CardId.GrinderGolem, GrinderGolemeff);
            // AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            //AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpidersp);
            //AddExecutor(ExecutorType.SpSummon, CardId.AkashicMagician);
            //plan C
            AddExecutor(ExecutorType.SpSummon, CardId.OddEyesAbsoluteDragon, OddEyesAbsoluteDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.OddEyesAbsoluteDragon, OddEyesAbsoluteDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.OddEyesWingDragon);
            //summon
            AddExecutor(ExecutorType.Summon, CardId.WindwitchGlassBell, WindwitchGlassBellsummonfirst);
            AddExecutor(ExecutorType.Summon, CardId.SpellbookMagicianOfProphecy, SpellbookMagicianOfProphecysummon);
            AddExecutor(ExecutorType.Activate, CardId.SpellbookMagicianOfProphecy, SpellbookMagicianOfProphecyeff);
            AddExecutor(ExecutorType.Summon, CardId.MagiciansRod, MagiciansRodsummon);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansRod, MagiciansRodeff);
            AddExecutor(ExecutorType.Summon, CardId.WindwitchGlassBell, WindwitchGlassBellsummon);
            //activate
            AddExecutor(ExecutorType.Activate, CardId.LllusionMagic, LllusionMagiceff);
            AddExecutor(ExecutorType.SpellSet, CardId.LllusionMagic, LllusionMagicset);
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfKnowledge, SpellbookOfKnowledgeeff);
            AddExecutor(ExecutorType.Activate, CardId.WonderWand, WonderWandeff);
            AddExecutor(ExecutorType.Activate, CardId.TheEyeOfTimaeus, TheEyeOfTimaeuseff);
            AddExecutor(ExecutorType.SpSummon, CardId.ApprenticeLllusionMagician, ApprenticeLllusionMagiciansp);
            AddExecutor(ExecutorType.Activate, CardId.ApprenticeLllusionMagician, ApprenticeLllusionMagicianeff);
            //other thing                     
            AddExecutor(ExecutorType.Activate, CardId.MagicianOfLllusion);
            AddExecutor(ExecutorType.Activate, CardId.MagicianNavigation, MagicianNavigationeff);
            AddExecutor(ExecutorType.Activate, CardId.EternalSoul, EternalSouleff);
            AddExecutor(ExecutorType.SpSummon, CardId.BigEye, BigEyesp);
            AddExecutor(ExecutorType.Activate, CardId.BigEye, BigEyeeff);
            AddExecutor(ExecutorType.SpSummon, CardId.Dracossack, Dracossacksp);
            AddExecutor(ExecutorType.Activate, CardId.Dracossack, Dracossackeff);
            AddExecutor(ExecutorType.SpSummon, CardId.ApprenticeWitchling, ApprenticeWitchlingsp);
            AddExecutor(ExecutorType.Activate, CardId.ApprenticeWitchling, ApprenticeWitchlingeff);
            AddExecutor(ExecutorType.SpSummon, CardId.VentriloauistsClaraAndLucika, VentriloauistsClaraAndLucikasp);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }
        private void EternalSoulSelect()
        {
            AI.SelectPosition(CardPosition.FaceUpAttack);
            /*
            if (Enemy.HasInMonstersZone(CardId.MekkKnightMorningStar))
            {
                int MekkKnightZone = 0;
                int BotZone = 0;
                for (int i = 0; i <= 6; i++)
                {
                    if (Enemy.MonsterZone[i] != null && Enemy.MonsterZone[i].IsCode(CardId.MekkKnightMorningStar))
                    {
                        MekkKnightZone = i;
                        break;
                    }
                }
                if (Bot.MonsterZone[GetReverseColumnMainZone(MekkKnightZone)] == null)
                {
                    BotZone = GetReverseColumnMainZone(MekkKnightZone);
                    AI.SelectPlace(ReverseZoneTo16bit(BotZone));
                }
                else
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        if (!NoJackKnightColumn(i))
                        {
                            if (Bot.MonsterZone[GetReverseColumnMainZone(i)] == null)
                            {
                                AI.SelectPlace(ReverseZoneTo16bit(GetReverseColumnMainZone(i)));
                                break;
                            }
                        }
                    }
                }                
                Logger.DebugWriteLine("******************MekkKnightMorningStar");
            }
            else
            {
                for (int i = 0; i <= 6; i++)
                {
                    if (!NoJackKnightColumn(i))
                    {
                        if (Bot.MonsterZone[GetReverseColumnMainZone(i)] == null)
                        {
                            AI.SelectPlace(ReverseZoneTo16bit(GetReverseColumnMainZone(i)));
                            break;
                        } 
                    }
                }
            }
            */
        }
        int attackerzone = -1;
        int defenderzone = -1;
        bool Secret_used = false;
        bool plan_A = false;
        bool plan_C = false;
        int maxxc_done = 0;
        int lockbird_done = 0;
        int ghost_done = 0;
        bool maxxc_used = false;
        bool lockbird_used = false;
        bool ghost_used = false;
        bool WindwitchGlassBelleff_used = false;
        int ApprenticeLllusionMagician_count = 0;
        bool Spellbook_summon = false;
        bool Rod_summon = false;
        bool GlassBell_summon = false;
        bool magician_sp = false;
        bool soul_used = false;
        bool big_attack = false;
        bool big_attack_used = false;
        bool CrystalWingSynchroDragon_used = false;
        public override void OnNewPhase()
        {
            //Util.UpdateLinkedZone();
            //Logger.DebugWriteLine("Zones.CheckLinkedPointZones= " + Zones.CheckLinkedPointZones);
            //Logger.DebugWriteLine("Zones.CheckMutualEnemyZoneCount= " + Zones.CheckMutualEnemyZoneCount);
            plan_C = false;
            ApprenticeLllusionMagician_count = 0;
            foreach (ClientCard count in Bot.GetMonsters())
            {
                if (count.IsCode(CardId.ApprenticeLllusionMagician) && count.IsFaceup())
                    ApprenticeLllusionMagician_count++;
            }
            foreach (ClientCard dangerous in Enemy.GetMonsters())
            {
                if (dangerous != null && dangerous.IsShouldNotBeTarget() && dangerous.Attack > 2500 &&
                    !Bot.HasInHandOrHasInMonstersZone(CardId.ApprenticeLllusionMagician))
                {
                    plan_C = true;
                    Logger.DebugWriteLine("*********dangerous = " + dangerous.Id);
                }
            }
            if (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy) &&
              Bot.HasInHand(CardId.MagiciansRod) &&
              Bot.HasInHand(CardId.WindwitchGlassBell))
            {
                if (Bot.HasInHand(CardId.SpellbookOfKnowledge) ||
                    Bot.HasInHand(CardId.WonderWand))
                    Rod_summon = true;
                else
                    Spellbook_summon = true;
            }
            else if
                 (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy) &&
                 Bot.HasInHand(CardId.MagiciansRod))
            {
                if (Bot.HasInSpellZone(CardId.EternalSoul) &&
                    !(Bot.HasInHand(CardId.DarkMagician) || Bot.HasInHand(CardId.DarkMagician)))
                    Rod_summon = true;
                else if (Bot.HasInHand(CardId.SpellbookOfKnowledge) ||
                    Bot.HasInHand(CardId.WonderWand))
                    Rod_summon = true;
                else
                    Spellbook_summon = true;
            }
            else if
                  (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy) &&
                 Bot.HasInHand(CardId.WindwitchGlassBell))
            {
                if (plan_A)
                    Rod_summon = true;
                else
                    GlassBell_summon = true;
            }
            else if
                  (Bot.HasInHand(CardId.MagiciansRod) &&
                 Bot.HasInHand(CardId.WindwitchGlassBell))
            {
                if (plan_A)
                    Rod_summon = true;
                else
                    GlassBell_summon = true;
            }
            else
            {
                Spellbook_summon = true;
                Rod_summon = true;
                GlassBell_summon = true;
            }
        }
        public override void OnNewTurn()
        {
            CrystalWingSynchroDragon_used = false;
            Secret_used = false;
            maxxc_used = false;
            lockbird_used = false;
            ghost_used = false;
            WindwitchGlassBelleff_used = false;
            Spellbook_summon = false;
            Rod_summon = false;
            GlassBell_summon = false;
            magician_sp = false;
            big_attack = false;
            big_attack_used = false;
            soul_used = false;
            base.OnNewTurn();
        }
        public int GetTotalATK(IList<ClientCard> list)
        {

            int atk = 0;
            foreach (ClientCard c in list)
            {
                if (c == null) continue;
                atk += c.Attack;
            }
            return atk;
        }

        private bool WindwitchIceBelleff()
        {
            if (lockbird_used) return false;
            if (Enemy.HasInMonstersZone(CardId.ElShaddollWinda)) return false;
            if (maxxc_used) return false;
            if (WindwitchGlassBelleff_used) return false;
            //AI.SelectPlace(Zones.z2, 1);
            if (Bot.GetRemainingCount(CardId.WindwitchGlassBell, 2) >= 1)
                AI.SelectCard(CardId.WindwitchGlassBell);
            else if (Bot.HasInHand(CardId.WindwitchGlassBell))
                AI.SelectCard(CardId.WindwitchSnowBell);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool WindwitchGlassBelleff()
        {
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell))
            {
                int ghost_count = 0;
                foreach (ClientCard check in Enemy.Graveyard)
                {
                    if (check.IsCode(CardId.Ghost))
                        ghost_count++;
                }
                if (ghost_count != ghost_done)
                    AI.SelectCard(CardId.WindwitchIceBell);
                else
                    AI.SelectCard(CardId.WindwitchSnowBell);
            }
            else
                AI.SelectCard(CardId.WindwitchIceBell);
            WindwitchGlassBelleff_used = true;
            return true;
        }

        private bool WindwitchSnowBellsp()
        {
            if (maxxc_used) return false;
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool WindwitchWinterBellsp()
        {
            if (maxxc_used) return false;
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                 Bot.HasInMonstersZone(CardId.WindwitchGlassBell) &&
                 Bot.HasInMonstersZone(CardId.WindwitchSnowBell))
            {
                //AI.SelectPlace(Zones.z5, Zones.ExtraMonsterZones);
                AI.SelectCard(CardId.WindwitchIceBell, CardId.WindwitchGlassBell);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            return false;
        }

        private bool WindwitchWinterBelleff()
        {
            AI.SelectCard(CardId.WindwitchGlassBell);
            return true;
        }

        private bool ClearWingFastDragonsp()
        {
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
            {

                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            return false;
        }

        private bool ClearWingFastDragoneff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player == 1)
                    return DefaultTrap();
                return true;
            }
            return false;
        }
        private bool CrystalWingSynchroDragonsp()
        {
            if (Bot.HasInMonstersZone(CardId.WindwitchSnowBell) && Bot.HasInMonstersZone(CardId.WindwitchWinterBell))
            {
                //AI.SelectPlace(Zones.z5, Zones.ExtraMonsterZones);
                plan_A = true;
                return true;
            }
            return false;
        }

        /* private bool GrinderGolemeff()
         {

             if (plan_A) return false;
             AI.SelectPosition(CardPosition.FaceUpDefence);
             if (Bot.GetMonstersExtraZoneCount() == 0)
                 return true;
             if (Bot.HasInMonstersZone(CardId.AkashicMagician) ||
                 Bot.HasInMonstersZone(CardId.SecurityDragon))
                 return true;
             return false;
         }*/

        /*  private bool Linkuribohsp()
          {
              if (Bot.HasInMonstersZone(CardId.GrinderGolem + 1))
              {
                  AI.SelectCard(CardId.GrinderGolem + 1);
                  return true;
              }
              return false;
          }

          private bool LinkSpidersp()
          {
              if(Bot.HasInMonstersZone(CardId.GrinderGolem+1))
              {
                  AI.SelectCard(CardId.GrinderGolem + 1);
                  return true;
              }
              return false;
          }*/

        private bool OddEyesAbsoluteDragonsp()
        {
            if (plan_C)
            {
                return true;
            }
            return false;
        }

        private bool OddEyesAbsoluteDragoneff()
        {
            Logger.DebugWriteLine("OddEyesAbsoluteDragonef 1");
            if (Card.Location == CardLocation.MonsterZone/*ActivateDescription == Util.GetStringId(CardId.OddEyesAbsoluteDragon, 0)*/)
            {
                Logger.DebugWriteLine("OddEyesAbsoluteDragonef 2");
                return Duel.Player == 1;
            }
            else if (Card.Location == CardLocation.Grave/*ActivateDescription == Util.GetStringId(CardId.OddEyesAbsoluteDragon, 0)*/)
            {
                Logger.DebugWriteLine("OddEyesAbsoluteDragonef 3");
                AI.SelectCard(CardId.OddEyesWingDragon);
                return true;
            }
            return false;
        }

        private bool SolemnStrikeeff()
        {
            if (Bot.LifePoints > 1500 && Duel.LastChainPlayer == 1)
                return true;
            if (DefaultOnlyHorusSpSummoning()) return false;
            return false;
        }

        private bool ChainEnemy()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Util.GetLastChainCard() != null &&
                Util.GetLastChainCard().IsCode(CardId.UpstartGoblin))
                return false;
            return Duel.LastChainPlayer == 1;
        }

        private bool CrystalWingSynchroDragoneff()
        {
            if (Duel.LastChainPlayer == 1)
            {
                CrystalWingSynchroDragon_used = true;
                return true;
            }
            return false;
        }

        private bool MaxxCeff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player == 1;
        }
        /*
        private bool Scapegoatset()
        {
            if (Bot.HasInSpellZone(CardId.Scapegoat)) return false;
            return (Bot.GetMonsterCount() - Bot.GetMonstersExtraZoneCount()) < 2;
        }

        public bool Scapegoateff()
        {
            if (Duel.Player == 0) return false;           
            if (DefaultOnBecomeTarget() && !Enemy.HasInMonstersZone(CardId.UltimateConductorTytanno))
            {
                Logger.DebugWriteLine("*************************sheepeff");
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.CrystalWingSynchroDragon)) return false;     
            if(Duel.Phase == DuelPhase.End)
            {
                Logger.DebugWriteLine("*************************sheepeff");
                return true;
            }
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                
                int total_atk = 0;
                List<ClientCard> enemy_monster = Enemy.GetMonsters();
                foreach (ClientCard m in enemy_monster)
                {
                    if (m.IsAttack() && !m.Attacked) total_atk += m.Attack;
                }
                if (total_atk >= Bot.LifePoints && !Enemy.HasInMonstersZone(CardId.UltimateConductorTytanno)) return true;
            }
            return false;
        }
        
        private bool Hidarumasp()
        {
            if (!Bot.HasInMonstersZone(CardId.Scapegoat + 1)) return false;
            if(Bot.MonsterZone[5]==null)
            {
                AI.SelectCard(new[] { CardId.Scapegoat + 1, CardId.Scapegoat + 1 });               
                return true;
            }
            if (Bot.MonsterZone[6] == null)
            {
                AI.SelectCard(new[] { CardId.Scapegoat + 1, CardId.Scapegoat + 1 });                
                return true;
            }
            return false;

        }

        private bool Linkuribohsp()
        {
            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (!c.IsCode(CardId.WindwitchSnowBell) && c.Level == 1 && !c.IsCode(CardId.LinkSpider) && !c.IsCode(CardId.Linkuriboh))
                {
                    AI.SelectCard(c);
                    return true;
                }
            }
            return false;
        }


        private bool Linkuriboheff()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Linkuriboh)) return false;
            if (Bot.HasInMonstersZone(CardId.WindwitchSnowBell)) return false;
            return true;
        }
        private bool BorreloadDragonsp()
        {
            if(Bot.HasInMonstersZone(CardId.Hidaruma)&&
                Bot.HasInMonstersZone(CardId.Linkuriboh))
            {
                AI.SelectCard(new[] { CardId.Hidaruma, CardId.Linkuriboh, CardId.LinkSpider ,CardId.Linkuriboh});
                return true;
            }
            
            return false;
        }

       
        private bool BorreloadDragoneff()
        {
            if (ActivateDescription == -1)
            {
                ClientCard enemy_monster = Enemy.BattlingMonster;
                if (enemy_monster != null && enemy_monster.HasPosition(CardPosition.Attack))
                {
                    return enemy_monster.Attack > 2000;
                }
                return true;
            };
            ClientCard BestEnemy = Util.GetBestEnemyMonster(true,true);            
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            AI.SelectCard(BestEnemy);
            return true;  
        }*/
        private bool EternalSoulset()
        {
            if (Bot.GetHandCount() > 6) return true;
            if (!Bot.HasInSpellZone(CardId.EternalSoul))
                return true;
            return false;
        }

        private bool EternalSouleff()
        {
            IList<ClientCard> grave = Bot.Graveyard;
            IList<ClientCard> magician = new List<ClientCard>();
            foreach (ClientCard check in grave)
            {
                if (check.IsCode(CardId.DarkMagician))
                {
                    magician.Add(check);
                }
            }
            if (Util.IsChainTarget(Card) && Bot.GetMonsterCount() == 0)
            {
                AI.SelectYesNo(false);
                return true;
            }
            if (Util.ChainCountPlayer(0) > 0) return false;

            if (Enemy.HasInSpellZone(CardId.HarpiesFeatherDuster) && Card.IsFacedown())
                return false;

            foreach (ClientCard target in Duel.ChainTargets)
            {
                if ((target.IsCode(CardId.DarkMagician, CardId.DarkMagicianTheDragonKnight))
                    && Card.IsFacedown())
                {
                    AI.SelectYesNo(false);
                    return true;
                }
            }

            if (Enemy.HasInSpellZone(CardId.DarkHole) && Card.IsFacedown() &&
                (Bot.HasInMonstersZone(CardId.DarkMagician) || Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)))
            {
                AI.SelectYesNo(false);
                return true;
            }
            if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) &&
                !Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight) && !plan_C)
            {
                EternalSoulSelect();
                AI.SelectCard(CardId.DarkMagicianTheDragonKnight);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInSpellZone(CardId.DarkMagicalCircle) &&
                (Enemy.HasInMonstersZone(CardId.SummonSorceress) || Enemy.HasInMonstersZone(CardId.FirewallDragon)))
            {
                soul_used = true;
                magician_sp = true;
                EternalSoulSelect();
                AI.SelectCard(magician);
                return true;
            }
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Enemy.GetMonsterCount() > 0)
            {
                if (Card.IsFacedown() && Bot.HasInMonstersZone(CardId.VentriloauistsClaraAndLucika))
                {
                    AI.SelectYesNo(false);
                    return true;
                }
                if (Card.IsFacedown() &&
                (Bot.HasInMonstersZone(CardId.DarkMagician) || Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)))
                {
                    AI.SelectYesNo(false);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    EternalSoulSelect();
                    AI.SelectCard(magician);
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    EternalSoulSelect();
                    return true;
                }
            }
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Main1)
            {
                if (Bot.HasInHand(CardId.DarkMagicalCircle) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                    return false;
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(magician);
                    EternalSoulSelect();
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    EternalSoulSelect();
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.End)
            {
                if (Card.IsFacedown() && Bot.HasInMonstersZone(CardId.VentriloauistsClaraAndLucika))
                {
                    AI.SelectYesNo(false);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(magician);
                    EternalSoulSelect();
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    EternalSoulSelect();
                    return true;
                }
                return true;
            }
            return false;
        }

        private bool MagicianNavigationset()
        {
            if (Bot.GetHandCount() > 6) return true;
            if (Bot.HasInSpellZone(CardId.LllusionMagic)) return true;
            if (Bot.HasInHand(CardId.DarkMagician) && !Bot.HasInSpellZone(CardId.MagicianNavigation))
                return true;
            return false;
        }

        private bool MagicianNavigationeff()
        {
            bool spell_act = false;
            IList<ClientCard> spell = new List<ClientCard>();
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetSpells())
                {
                    if (Util.GetLastChainCard() == check)
                    {
                        spell.Add(check);
                        spell_act = true;
                        break;
                    }
                }
            }
            bool soul_faceup = false;
            foreach (ClientCard check in Bot.GetSpells())
            {
                if (check.IsCode(CardId.EternalSoul) && check.IsFaceup())
                {
                    soul_faceup = true;
                }
            }
            if (Card.Location == CardLocation.Grave && spell_act)
            {
                Logger.DebugWriteLine("**********************Navigationeff***********");
                AI.SelectCard(spell);
                return true;
            }
            if (Util.IsChainTarget(Card))
            {
                AI.SelectPlace(Zones.z0 | Zones.z4);
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = Util.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                else
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                magician_sp = true;
                return UniqueFaceupSpell();
            }
            if (DefaultOnBecomeTarget() && !soul_faceup)
            {
                AI.SelectPlace(Zones.z0 | Zones.z4);
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = Util.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                else
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                magician_sp = true;
                return true;
            }
            if (Duel.Player == 0 && Card.Location == CardLocation.SpellZone && !maxxc_used && Bot.HasInHand(CardId.DarkMagician))
            {
                AI.SelectPlace(Zones.z0 | Zones.z4);
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = Util.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                else
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                magician_sp = true;
                return UniqueFaceupSpell();
            }
            if (Duel.Player == 1 && Bot.HasInSpellZone(CardId.DarkMagicalCircle) &&
                (Enemy.HasInMonstersZone(CardId.SummonSorceress) || Enemy.HasInMonstersZone(CardId.FirewallDragon))
                && Card.Location == CardLocation.SpellZone)
            {
                AI.SelectPlace(Zones.z0 | Zones.z4);
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = Util.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                else
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                magician_sp = true;
                return UniqueFaceupSpell();
            }
            if (Enemy.GetFieldCount() > 0 &&
                (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End) &&
                Card.Location == CardLocation.SpellZone && !maxxc_used)
            {
                AI.SelectPlace(Zones.z0 | Zones.z4);
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = Util.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                else
                    AI.SelectNextCard(CardId.ApprenticeLllusionMagician, CardId.DarkMagician, CardId.MagicianOfLllusion);
                magician_sp = true;
                return UniqueFaceupSpell();
            }

            return false;
        }
        private bool DarkMagicalCircleeff()
        {
            if (Card.Location == CardLocation.Hand)
            {
                //AI.SelectPlace(Zones.z2, 2);
                if (Bot.LifePoints <= 4000)
                    return true;
                return UniqueFaceupSpell();
            }
            else
            {
                if (magician_sp)
                {
                    AI.SelectCard(Util.GetBestEnemyCard(false, true));
                    if (Util.GetBestEnemyCard(false, true) != null)
                        Logger.DebugWriteLine("*************SelectCard= " + Util.GetBestEnemyCard(false, true).Id);
                    magician_sp = false;
                }
            }
            return true;
        }
        private bool LllusionMagicset()
        {
            if (Bot.GetMonsterCount() >= 1 &&
                !(Bot.GetMonsterCount() == 1 && Bot.HasInMonstersZone(CardId.CrystalWingSynchroDragon)) &&
                !(Bot.GetMonsterCount() == 1 && Bot.HasInMonstersZone(CardId.ClearWingFastDragon)) &&
                !(Bot.GetMonsterCount() == 1 && Bot.HasInMonstersZone(CardId.VentriloauistsClaraAndLucika)))
                return true;
            return false;
        }
        private bool LllusionMagiceff()
        {
            if (lockbird_used) return false;
            if (Duel.LastChainPlayer == 0) return false;
            ClientCard target = null;
            bool soul_exist = false;
            //AI.SelectPlace(Zones.z2, 2);
            foreach (ClientCard m in Bot.GetSpells())
            {
                if (m.IsCode(CardId.EternalSoul) && m.IsFaceup())
                    soul_exist = true;
            }
            if (!soul_used && soul_exist)
            {
                if (Bot.HasInMonstersZone(CardId.MagiciansRod))
                {
                    AI.SelectCard(CardId.MagiciansRod);
                    AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                    return true;
                }
            }
            if (Duel.Player == 0)
            {
                int ghost_count = 0;
                foreach (ClientCard check in Enemy.Graveyard)
                {
                    if (check.IsCode(CardId.Ghost))
                        ghost_count++;
                }
                if (ghost_count != ghost_done)
                {
                    if (Duel.CurrentChain.Count >= 2 && Util.GetLastChainCard().IsCode(0))
                    {
                        AI.SelectCard(CardId.MagiciansRod);
                        AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                        return true;
                    }
                }
                int count = 0;
                foreach (ClientCard m in Bot.GetMonsters())
                {
                    if (Util.IsChainTarget(m))
                    {
                        count++;
                        target = m;
                        Logger.DebugWriteLine("************IsChainTarget= " + target.Id);
                        break;
                    }
                }
                if (count == 0) return false;
                if ((target.IsCode(CardId.WindwitchGlassBell, CardId.WindwitchIceBell)) &&
                    Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                    Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
                    return false;
                AI.SelectCard(target);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }

            if (Bot.HasInMonstersZone(CardId.MagiciansRod) || Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy))
            {
                AI.SelectCard(CardId.MagiciansRod, CardId.SpellbookMagicianOfProphecy);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
            {
                AI.SelectCard(CardId.WindwitchGlassBell);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.WindwitchIceBell))
            {
                AI.SelectCard(CardId.WindwitchIceBell);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.WindwitchSnowBell))
            {
                AI.SelectCard(CardId.WindwitchSnowBell);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy))
            {
                AI.SelectCard(CardId.SpellbookMagicianOfProphecy);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) &&
               (Bot.HasInSpellZone(CardId.EternalSoul) || Bot.HasInSpellZone(CardId.MagicianNavigation)))
            {
                AI.SelectCard(CardId.ApprenticeLllusionMagician);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }

            if ((Bot.GetRemainingCount(CardId.DarkMagician, 3) > 1 || Bot.HasInGraveyard(CardId.DarkMagician)) &&
                Bot.HasInSpellZone(CardId.MagicianNavigation) &&
                (Bot.HasInMonstersZone(CardId.DarkMagician) || Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician)) &&
                Duel.Player == 1 && !Bot.HasInHand(CardId.DarkMagician))
            {
                AI.SelectCard(CardId.DarkMagician, CardId.ApprenticeLllusionMagician);
                AI.SelectNextCard(CardId.DarkMagician, CardId.DarkMagician);
                return true;
            }
            return false;
        }
        private bool SpellbookMagicianOfProphecyeff()
        {
            Logger.DebugWriteLine("*********Secret_used= " + Secret_used);
            if (Secret_used)
                AI.SelectCard(CardId.SpellbookOfKnowledge);
            else
                AI.SelectCard(CardId.SpellbookOfSecrets, CardId.SpellbookOfKnowledge);
            return true;
        }

        private bool TheEyeOfTimaeuseff()
        {
            //AI.SelectPlace(Zones.z2, 2);
            return true;
        }

        private bool UpstartGoblineff()
        {
            //AI.SelectPlace(Zones.z2, 2);
            return true;
        }
        private bool SpellbookOfSecreteff()
        {
            if (lockbird_used) return false;
            //AI.SelectPlace(Zones.z2, 2);
            Secret_used = true;
            if (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy))
                AI.SelectCard(CardId.SpellbookOfKnowledge);
            else
                AI.SelectCard(CardId.SpellbookMagicianOfProphecy);
            return true;
        }

        private bool SpellbookOfKnowledgeeff()
        {
            int count = 0;
            foreach (ClientCard check in Bot.GetMonsters())
            {
                if (!check.IsCode(CardId.CrystalWingSynchroDragon))
                    count++;
            }
            Logger.DebugWriteLine("%%%%%%%%%%%%%%%%SpellCaster= " + count);
            if (lockbird_used) return false;
            if (Bot.HasInSpellZone(CardId.LllusionMagic) && count < 2)
                return false;
            //AI.SelectPlace(Zones.z2, 2);
            if (Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy) ||
                Bot.HasInMonstersZone(CardId.MagiciansRod) ||
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell) ||
                Bot.HasInMonstersZone(CardId.WindwitchIceBell))
            {
                AI.SelectCard(CardId.SpellbookMagicianOfProphecy, CardId.MagiciansRod, CardId.WindwitchGlassBell);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetSpellCount() < 2 && Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectCard(CardId.ApprenticeLllusionMagician);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.DarkMagician) &&
                    Bot.HasInSpellZone(CardId.EternalSoul) && Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectCard(CardId.DarkMagician);
                return true;
            }
            return false;
        }

        private bool WonderWandeff()
        {
            if (lockbird_used) return false;
            int count = 0;
            foreach (ClientCard check in Bot.GetMonsters())
            {
                if (!check.IsCode(CardId.CrystalWingSynchroDragon))
                    count++;
            }
            Logger.DebugWriteLine("%%%%%%%%%%%%%%%%SpellCaster= " + count);
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInSpellZone(CardId.LllusionMagic) && count < 2)
                    return false;
                //AI.SelectPlace(Zones.z2, 2);
                if (Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy) ||
                Bot.HasInMonstersZone(CardId.MagiciansRod) ||
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell) ||
                Bot.HasInMonstersZone(CardId.WindwitchIceBell))
                {
                    AI.SelectCard(
                        CardId.SpellbookMagicianOfProphecy,
                        CardId.MagiciansRod,
                        CardId.WindwitchGlassBell,
                        CardId.WindwitchIceBell
                        );
                    return UniqueFaceupSpell();
                }

                if (Bot.HasInMonstersZone(CardId.DarkMagician) &&
                    Bot.HasInSpellZone(CardId.EternalSoul) && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.DarkMagician);
                    return UniqueFaceupSpell();
                }
                if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetSpellCount() < 2 && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.ApprenticeLllusionMagician);
                    return UniqueFaceupSpell();
                }
                if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetHandCount() <= 3 && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.ApprenticeLllusionMagician);
                    return UniqueFaceupSpell();
                }
            }
            else
            {
                if (Duel.Turn != 1)
                {
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                    Util.GetBestEnemyMonster(true, true) == null)
                        return false;
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                        Util.GetBestEnemyMonster().IsFacedown())
                        return true;
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                        Util.GetBestBotMonster(true) != null &&
                        Util.GetBestBotMonster(true).Attack > Util.GetBestEnemyMonster(true).Attack)
                        return false;
                }
                return true;
            }
            return false;
        }

        private bool ApprenticeLllusionMagiciansp()
        {
            //AI.SelectPlace(Zones.z2, 1);
            if (Bot.HasInHand(CardId.DarkMagician) && !Bot.HasInSpellZone(CardId.MagicianNavigation))
            {
                if (Bot.GetRemainingCount(CardId.DarkMagician, 3) > 0)
                {
                    AI.SelectCard(CardId.DarkMagician);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }
                return false;
            }
            if ((Bot.HasInHand(CardId.SpellbookOfSecrets) ||
                  Bot.HasInHand(CardId.DarkMagicAttack)))
            {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectCard(CardId.SpellbookOfSecrets, CardId.DarkMagicAttack);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician))
                return false;
            int count = 0;
            foreach (ClientCard check in Bot.Hand)
            {
                if (check.IsCode(CardId.WonderWand))
                    count++;
            }
            if (count >= 2)
            {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectCard(CardId.WonderWand);
                return true;
            }
            if(!Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation)&&
                !Bot.HasInHand(CardId.DarkMagician) && Bot.GetHandCount()>2&&
                Bot.GetMonsterCount()==0)
            {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectCard(CardId.MagicianOfLllusion, CardId.ApprenticeLllusionMagician, CardId.TheEyeOfTimaeus, CardId.DarkMagicInheritance, CardId.WonderWand);
                return true;
            }
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DarkMagician))
            {
                if (Bot.HasInHandOrInSpellZone(CardId.LllusionMagic) && Bot.GetMonsterCount() >= 1)
                    return false;
                AI.SelectPosition(CardPosition.FaceUpAttack);
                int Navigation_count = 0;
                foreach (ClientCard Navigation in Bot.Hand)
                {
                    if (Navigation.IsCode(CardId.MagicianNavigation))
                        Navigation_count++;
                }
                if (Navigation_count >= 2)
                {
                    AI.SelectCard(CardId.MagicianNavigation);
                    return true;
                }
                AI.SelectCard(CardId.MagicianOfLllusion, CardId.ApprenticeLllusionMagician, CardId.TheEyeOfTimaeus, CardId.DarkMagicInheritance, CardId.WonderWand);
                return true;
            }
            return false;
        }
        private bool ApprenticeLllusionMagicianeff()
        {
            if (Util.ChainContainsCard(CardId.ApprenticeLllusionMagician)) return false;
            if (Duel.Phase == DuelPhase.Battle ||
                Duel.Phase == DuelPhase.BattleStart ||
                Duel.Phase == DuelPhase.BattleStep ||
                Duel.Phase == DuelPhase.Damage ||
                Duel.Phase == DuelPhase.DamageCal
               )
            {
                if (ActivateDescription == -1)
                {
                    Logger.DebugWriteLine("ApprenticeLllusionMagicianadd");
                    return true;
                }
                if (Card.IsDisabled()) return false;
                if ((Bot.BattlingMonster == null)) return false;
                if ((Enemy.BattlingMonster == null)) return false;
                if (Bot.BattlingMonster.Attack < Enemy.BattlingMonster.Attack)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        private bool SpellbookMagicianOfProphecysummon()
        {
            //AI.SelectPlace(Zones.z2, 1);
            if (lockbird_used) return false;
            if (Spellbook_summon)
            {
                if (Secret_used)
                    AI.SelectCard(CardId.SpellbookOfKnowledge);
                else
                    AI.SelectCard(CardId.SpellbookOfSecrets, CardId.SpellbookOfKnowledge);
                return true;
            }
            return false;
        }
        private bool MagiciansRodsummon()
        {
            if (lockbird_used) return false;
            //AI.SelectPlace(Zones.z2, 1);
            if (Rod_summon) return true;
            return true;
        }

        private bool DarkMagicAttackeff()
        {
            //AI.SelectPlace(Zones.z1, 2);
            return DefaultHarpiesFeatherDusterFirst();
        }
        private bool DarkMagicInheritanceeff()
        {
            if (lockbird_used) return false;
            IList<ClientCard> grave = Bot.Graveyard;
            IList<ClientCard> spell = new List<ClientCard>();
            int count = 0;
            foreach (ClientCard check in grave)
            {
                if (Card.HasType(CardType.Spell))
                {
                    spell.Add(check);
                    count++;
                }
            }
            if (count >= 2)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(spell);
                if (Bot.HasInHandOrInSpellZone(CardId.EternalSoul) && Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle))
                    if (Bot.GetRemainingCount(CardId.DarkMagician, 3) >= 2 && !Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.LllusionMagic))
                    {
                        AI.SelectNextCard(CardId.LllusionMagic);
                        return true;
                    }

                if (Bot.HasInHand(CardId.ApprenticeLllusionMagician) &&
                  (!Bot.HasInHandOrInSpellZone(CardId.EternalSoul) || !Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation)))
                {
                    AI.SelectNextCard(CardId.MagicianNavigation);
                    return true;
                }
                if (Bot.HasInHandOrInSpellZone(CardId.EternalSoul) && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DarkMagician) &&
                    !Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.LllusionMagic))
                {
                    AI.SelectNextCard(CardId.LllusionMagic);
                    return true;
                }

                if (Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation) &&
                    !Bot.HasInHand(CardId.DarkMagician) &&
                    !Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                    Bot.GetRemainingCount(CardId.LllusionMagic, 1) > 0)
                {
                    AI.SelectNextCard(CardId.LllusionMagic);
                    return true;
                }

                if ((Bot.HasInHandOrInSpellZone(CardId.EternalSoul) || Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation)) &&
                    !Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle))
                {
                    AI.SelectNextCard(CardId.DarkMagicalCircle);
                    return true;
                }
                if (Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle))
                {
                    if (Bot.HasInGraveyard(CardId.MagicianNavigation))
                    {
                        AI.SelectNextCard(CardId.EternalSoul, CardId.MagicianNavigation, CardId.DarkMagicalCircle);
                    }
                    else
                        AI.SelectNextCard(CardId.EternalSoul, CardId.MagicianNavigation, CardId.DarkMagicalCircle);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.MagicianNavigation))
                {
                    AI.SelectNextCard(CardId.EternalSoul, CardId.DarkMagicalCircle, CardId.MagicianNavigation);
                }
                else
                    AI.SelectNextCard(CardId.MagicianNavigation, CardId.DarkMagicalCircle, CardId.EternalSoul);
                return true;
            }
            return false;
        }
        private bool MagiciansRodeff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.HasInHandOrInSpellZone(CardId.EternalSoul) && Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle))
                    if (Bot.GetRemainingCount(CardId.DarkMagician, 3) >= 2 && Bot.GetRemainingCount(CardId.LllusionMagic, 1) > 0)
                    {
                        AI.SelectCard(CardId.LllusionMagic);
                        return true;
                    }

                if (Bot.HasInHand(CardId.ApprenticeLllusionMagician) &&
                  !Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation) &&
                  Bot.GetRemainingCount(CardId.MagicianNavigation, 3) > 0)
                {
                    AI.SelectCard(CardId.MagicianNavigation);
                    return true;
                }

                if (Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                    !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DarkMagician) &&
                    Bot.GetRemainingCount(CardId.LllusionMagic, 1) > 0)
                {
                    AI.SelectCard(CardId.LllusionMagic);
                    return true;
                }

                if (Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation) &&
                    !Bot.HasInHand(CardId.DarkMagician) &&
                    !Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                    Bot.GetRemainingCount(CardId.LllusionMagic, 1) > 0)
                {
                    AI.SelectCard(CardId.LllusionMagic);
                    return true;
                }
                if (!Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                    Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle) &&
                    Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation) &&
                    Bot.GetRemainingCount(CardId.EternalSoul, 3) > 0)
                {
                    AI.SelectCard(CardId.EternalSoul);
                    return true;
                }
                if ((Bot.HasInHandOrInSpellZone(CardId.EternalSoul) || Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation)) &&
                    !Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle) &&
                    Bot.GetRemainingCount(CardId.DarkMagicalCircle, 3) > 0)
                {
                    AI.SelectCard(CardId.DarkMagicalCircle);
                    return true;
                }
                if (!Bot.HasInHandOrInSpellZone(CardId.EternalSoul) &&
                    !Bot.HasInHandOrInSpellZone(CardId.MagicianNavigation))
                {
                    if (Bot.HasInHand(CardId.DarkMagician) &&
                        !Bot.HasInGraveyard(CardId.MagicianNavigation) &&
                        Bot.GetRemainingCount(CardId.MagicianNavigation, 3) > 0
                        )
                        AI.SelectCard(CardId.MagicianNavigation);
                    else if (!Bot.HasInHandOrInSpellZone(CardId.DarkMagicalCircle))
                        AI.SelectCard(CardId.DarkMagicalCircle);
                    else
                        AI.SelectCard(CardId.EternalSoul);
                    return true;
                }
                if (!Bot.HasInHand(CardId.MagicianNavigation))
                {
                    AI.SelectCard(CardId.MagicianNavigation);
                    return true;
                }
                if (!Bot.HasInHand(CardId.DarkMagicalCircle))
                {
                    AI.SelectCard(CardId.DarkMagicalCircle);
                    return true;
                }
                if (!Bot.HasInHand(CardId.EternalSoul))
                {
                    AI.SelectCard(CardId.EternalSoul);
                    return true;
                }
                AI.SelectCard(CardId.LllusionMagic, CardId.EternalSoul, CardId.DarkMagicalCircle, CardId.MagicianNavigation);
                return true;
            }
            else
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (Bot.HasInMonstersZone(CardId.VentriloauistsClaraAndLucika))
                {
                    AI.SelectCard(CardId.VentriloauistsClaraAndLucika);
                    return true;
                }
                int Enemy_atk = 0;
                IList<ClientCard> list = new List<ClientCard>();
                foreach (ClientCard monster in Enemy.GetMonsters())
                {
                    if (monster.IsAttack())
                        list.Add(monster);
                }
                Enemy_atk = GetTotalATK(list);
                int bot_atk = 0;
                IList<ClientCard> list_1 = new List<ClientCard>();
                foreach (ClientCard monster in Bot.GetMonsters())
                {
                    if (Util.GetWorstBotMonster(true) != null)
                    {
                        if (monster.IsAttack() && monster.Id != Util.GetWorstBotMonster(true).Id)
                            list_1.Add(monster);
                    }
                }
                bot_atk = GetTotalATK(list);
                if (Bot.HasInHand(CardId.MagiciansRod)) return false;
                if (Bot.HasInMonstersZone(CardId.ApprenticeWitchling) && Bot.GetMonsterCount() == 1 && Bot.HasInSpellZone(CardId.EternalSoul))
                    return false;
                if (Bot.LifePoints <= (Enemy_atk - bot_atk) &&
                    Bot.GetMonsterCount() > 1) return false;
                if ((Bot.LifePoints - Enemy_atk <= 1000) &&
                    Bot.GetMonsterCount() == 1) return false;
                AI.SelectCard(CardId.VentriloauistsClaraAndLucika, CardId.SpellbookMagicianOfProphecy, CardId.WindwitchGlassBell, CardId.WindwitchIceBell, CardId.MagiciansRod, CardId.DarkMagician, CardId.MagicianOfLllusion);
                return true;
            }
        }

        private bool WindwitchGlassBellsummonfirst()
        {
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                Bot.HasInMonstersZone(CardId.WindwitchSnowBell) &&
                !Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
                return true;
            return false;
        }
        private bool WindwitchGlassBellsummon()
        {
            if (lockbird_used) return false;
            if (!plan_A && (Bot.HasInGraveyard(CardId.WindwitchGlassBell) || Bot.HasInMonstersZone(CardId.WindwitchGlassBell)))
                return false;
            //AI.SelectPlace(Zones.z2, 1);
            if (GlassBell_summon && Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                !Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
                return true;
            if (WindwitchGlassBelleff_used) return false;
            if (GlassBell_summon) return true;
            return false;
        }
        private bool BigEyesp()
        {
            if (plan_C) return false;
            if (Util.IsOneEnemyBetterThanValue(2500, false) &&
                !Bot.HasInHandOrHasInMonstersZone(CardId.ApprenticeLllusionMagician))
            {
                //AI.SelectPlace(Zones.z5, Zones.ExtraMonsterZones);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            return false;
        }

        private bool BigEyeeff()
        {
            ClientCard target = Util.GetBestEnemyMonster(false, true);
            if (target != null && target.Attack >= 2500)
            {
                AI.SelectCard(CardId.DarkMagician);
                AI.SelectNextCard(target);
                return true;
            }
            return false;

        }
        private bool Dracossacksp()
        {
            if (plan_C) return false;
            if (Util.IsOneEnemyBetterThanValue(2500, false) &&
                !Bot.HasInHandOrHasInMonstersZone(CardId.ApprenticeLllusionMagician))
            {
                //AI.SelectPlace(Zones.z5, Zones.ExtraMonsterZones);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            return false;
        }

        private bool Dracossackeff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Dracossack, 0))
            {
                AI.SelectCard(CardId.DarkMagician);
                return true;

            }
            ClientCard target = Util.GetBestEnemyCard(false, true);
            if (target != null)
            {
                AI.SelectCard(CardId.Dracossack + 1);
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        private bool ApprenticeWitchlingsp()
        {
            int rod_count = 0;
            foreach (ClientCard rod in Bot.GetMonsters())
            {
                if (rod.IsCode(CardId.MagiciansRod))
                    rod_count++;
            }
            if (rod_count >= 2)
            {
                AI.SelectCard(CardId.MagiciansRod, CardId.MagiciansRod);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.DarkMagician) &&
                Bot.HasInMonstersZone(CardId.MagiciansRod) &&
                (Bot.HasInSpellZone(CardId.EternalSoul) || Bot.GetMonsterCount() >= 4)
                && Duel.Phase == DuelPhase.Main2)
            {
                if (rod_count >= 2)
                    AI.SelectCard(CardId.MagiciansRod, CardId.MagiciansRod);
                else
                    AI.SelectCard(CardId.MagiciansRod, CardId.DarkMagician);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.MagiciansRod) &&
                Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) &&
                (Bot.HasInSpellZone(CardId.EternalSoul) || Bot.HasInSpellZone(CardId.MagicianNavigation))
                && Duel.Phase == DuelPhase.Main2)
            {
                if (rod_count >= 2)
                    AI.SelectCard(CardId.MagiciansRod, CardId.MagiciansRod);
                else
                    AI.SelectCard(CardId.MagiciansRod, CardId.DarkMagician);
                return true;
            }
            return false;
        }

        private bool ApprenticeWitchlingeff()
        {
            AI.SelectCard(CardId.MagiciansRod, CardId.DarkMagician, CardId.ApprenticeLllusionMagician);
            return true;
        }
        public override bool OnSelectHand()
        {
            return true;
        }

        private bool VentriloauistsClaraAndLucikasp()
        {
            if (Bot.HasInSpellZone(CardId.LllusionMagic)) return false;
            if (Bot.HasInMonstersZone(CardId.MagiciansRod) && !Bot.HasInGraveyard(CardId.MagiciansRod) &&
                (Bot.HasInSpellZone(CardId.EternalSoul) || Bot.HasInSpellZone(CardId.MagicianNavigation)))
            {
                AI.SelectCard(CardId.MagiciansRod);
                return true;
            }
            return false;
        }
        public override void OnChaining(int player, ClientCard card)
        {
            base.OnChaining(player, card);
        }


        public override void OnChainEnd()
        {
            /*if (Enemy.MonsterZone[5] != null)
            {
                Logger.DebugWriteLine("%%%%%%%%%%%%%%%%Enemy.MonsterZone[5].LinkMarker= " + Enemy.MonsterZone[5].LinkMarker);
                Logger.DebugWriteLine("%%%%%%%%%%%%%%%%Enemy.MonsterZone[5].LinkLevel= " + Enemy.MonsterZone[5].LinkLevel);                
            }
                
            if (Enemy.MonsterZone[6] != null)
            {
                Logger.DebugWriteLine("%%%%%%%%%%%%%%%%Enemy.MonsterZone[6].LinkMarker= " + Enemy.MonsterZone[6].LinkMarker);
                Logger.DebugWriteLine("%%%%%%%%%%%%%%%%Enemy.MonsterZone[6].LinkLevel= " + Enemy.MonsterZone[6].LinkLevel);
            }
            for (int i = 0; i < 6; i++)
            {
                if (Enemy.MonsterZone[i] != null)
                    Logger.DebugWriteLine("++++++++MONSTER ZONE[" + i + "]= " + Enemy.MonsterZone[i].Attack);
            }
            for (int i = 0; i < 6; i++)
            {
                if (Bot.MonsterZone[i] != null)
                    Logger.DebugWriteLine("++++++++MONSTER ZONE[" + i + "]= " + Bot.MonsterZone[i].Id);
            }
            for (int i = 0; i < 4; i++)
            {
                if (Bot.SpellZone[i] != null)
                    Logger.DebugWriteLine("++++++++SpellZone[" + i + "]= " + Bot.SpellZone[i].Id);
            }*/
            if (Util.ChainContainsCard(CardId.MaxxC))
                maxxc_used = true;
            if ((Duel.CurrentChain.Count >= 1 && Util.GetLastChainCard().Id == 0) ||
                (Duel.CurrentChain.Count == 2 && !Util.ChainContainPlayer(0) && Duel.CurrentChain[0].Id == 0))
            {
                Logger.DebugWriteLine("current chain = " + Duel.CurrentChain.Count);
                Logger.DebugWriteLine("******last chain card= " + Util.GetLastChainCard().Id);
                int maxxc_count = 0;
                foreach (ClientCard check in Enemy.Graveyard)
                {
                    if (check.IsCode(CardId.MaxxC))
                        maxxc_count++;
                }
                if (maxxc_count != maxxc_done)
                {
                    Logger.DebugWriteLine("************************last chain card= " + Util.GetLastChainCard().Id);
                    maxxc_used = true;
                }               
                int lockbird_count = 0;
                foreach (ClientCard check in Enemy.Graveyard)
                {
                    if (check.IsCode(CardId.LockBird))
                        lockbird_count++;
                }
                if (lockbird_count != lockbird_done)
                {
                    Logger.DebugWriteLine("************************last chain card= " + Util.GetLastChainCard().Id);
                    lockbird_used = true;
                }
                int ghost_count = 0;
                foreach (ClientCard check in Enemy.Graveyard)
                {
                    if (check.IsCode(CardId.Ghost))
                        ghost_count++;
                }
                if (ghost_count != ghost_done)
                {
                    Logger.DebugWriteLine("************************last chain card= " + Util.GetLastChainCard().Id);
                    ghost_used = true;
                }
                if (ghost_used && Util.ChainContainsCard(CardId.WindwitchGlassBell))
                {
                    AI.SelectCard(CardId.WindwitchIceBell);
                    Logger.DebugWriteLine("***********WindwitchGlassBell*********************");
                }

            }
            foreach (ClientCard dangerous in Enemy.GetMonsters())
            {
                if (dangerous != null && dangerous.IsShouldNotBeTarget() &&
                    (dangerous.Attack > 2500 || dangerous.Defense > 2500) &&
                    !Bot.HasInHandOrHasInMonstersZone(CardId.ApprenticeLllusionMagician))
                {
                    plan_C = true;
                    Logger.DebugWriteLine("*********dangerous = " + dangerous.Id);
                }
            }
            int count = 0;
            foreach (ClientCard check in Enemy.Graveyard)
            {
                if (check.IsCode(CardId.MaxxC))
                    count++;
            }
            maxxc_done = count;
            count = 0;
            foreach (ClientCard check in Enemy.Graveyard)
            {
                if (check.IsCode(CardId.LockBird))
                    count++;
            }
            lockbird_done = count;
            count = 0;
            foreach (ClientCard check in Enemy.Graveyard)
            {
                if (check.IsCode(CardId.Ghost))
                    count++;
            }
            ghost_done = count;
            base.OnChainEnd();
        }


        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            /*
            if (Enemy.HasInMonstersZone(CardId.MekkKnightMorningStar))
            {
                attackerzone = -1;
                defenderzone = -1;
                for (int a = 0; a <= 6; a++)
                    for (int b = 0; b <= 6; b++)
                    {
                        if (Bot.MonsterZone[a] != null && Enemy.MonsterZone[b] != null &&
                            SameMonsterColumn(a, b) &&
                            Bot.MonsterZone[a].IsCode(attacker.Id) && Enemy.MonsterZone[b].IsCode(defender.Id))
                        {
                            attackerzone = a;
                            defenderzone = b;
                        }
                    }
                Logger.DebugWriteLine("**********attack_zone= " + attackerzone + "  defender_zone= " + defenderzone);
                if (!SameMonsterColumn(attackerzone, defenderzone) && IsJackKnightMonster(defenderzone))
                {
                    Logger.DebugWriteLine("**********cant attack ");
                    return false;
                }
            }
            */
            //Logger.DebugWriteLine("@@@@@@@@@@@@@@@@@@@ApprenticeLllusionMagician= " + ApprenticeLllusionMagician_count);            
            if (Bot.HasInSpellZone(CardId.OddEyesWingDragon))
                big_attack = true;
            if (Duel.Player == 0 && Bot.GetMonsterCount() >= 2 && plan_C)
            {
                Logger.DebugWriteLine("*********dangerous********************* ");
                if (attacker.IsCode(CardId.OddEyesAbsoluteDragon, CardId.OddEyesWingDragon))
                    attacker.RealPower = 9999;
            }
            if ((attacker.IsCode(CardId.DarkMagician, CardId.MagiciansRod, CardId.BigEye, CardId.ApprenticeWitchling)) &&
                Bot.HasInHandOrHasInMonstersZone(CardId.ApprenticeLllusionMagician))
            {
                attacker.RealPower += 2000;
            }
            if (attacker.IsCode(CardId.ApprenticeLllusionMagician) && ApprenticeLllusionMagician_count >= 2)
            {
                attacker.RealPower += 2000;
            }
            if (attacker.IsCode(CardId.DarkMagician, CardId.DarkMagicianTheDragonKnight) && Bot.HasInSpellZone(CardId.EternalSoul))
            {
                return true;
            }
            if (attacker.IsCode(CardId.CrystalWingSynchroDragon))
            {
                if (defender.Level >= 5)
                    attacker.RealPower = 9999;
                if (CrystalWingSynchroDragon_used == false)
                    return true;
            }
            if (!big_attack_used && big_attack)
            {
                attacker.RealPower = 9999;
                big_attack_used = true;
                return true;
            }
            if (attacker.IsCode(CardId.ApprenticeLllusionMagician))
                Logger.DebugWriteLine("@@@@@@@@@@@@@@@@@@@ApprenticeLllusionMagician= " + attacker.RealPower);
            if (Bot.HasInSpellZone(CardId.EternalSoul) && attacker.IsCode(CardId.DarkMagician, CardId.DarkMagicianTheDragonKnight, CardId.MagicianOfLllusion))
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }
        /*
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            for (int i = 0; i < defenders.Count; ++i)
            {
                ClientCard defender = defenders[i];
                if (Enemy.HasInMonstersZone(CardId.MekkKnightMorningStar))
                {
                    for (int b = 0; b <= 6; b++)
                    {
                        if (Enemy.MonsterZone[b] != null &&
                            SameMonsterColumn(attackerzone, b) &&
                            Bot.MonsterZone[attackerzone].IsCode(attacker.Id) && Enemy.MonsterZone[b].IsCode(defender.Id))
                        {
                            defenderzone = b;
                        }
                    }
                    if (defenderzone == -1)
                    {
                        Logger.DebugWriteLine("**********firstattackerzone= " + attackerzone + "  firstTargetzone= " + defenderzone);

                        return null;
                    }
                }
            }
            defenderzone = -1;
            return base.OnSelectAttackTarget(attacker,defenders);
        }
        */
        /*
        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {            
            for (int i = 0; i < attackers.Count; ++i)
            {                
                ClientCard attacker = attackers[i];
                for(int j = 0;j < defenders.Count;++j)
                {
                    ClientCard defender = defenders[j];
                    if (Enemy.HasInMonstersZone(CardId.MekkKnightMorningStar))
                    {
                        attackerzone = -1;
                        defenderzone = -1;
                        for(int a = 0;a <= 6;a++)
                            for(int b = 0;b <= 6;b++)
                            {
                                if (Bot.MonsterZone[a] != null && Enemy.MonsterZone[b]!=null &&
                                    SameMonsterColumn(a,b) && 
                                    Bot.MonsterZone[a].IsCode(attacker.Id) && Enemy.MonsterZone[b].IsCode(defender.Id))
                                {
                                    attackerzone = a;
                                    defenderzone = b;
                                }
                            }
                           
                        if (defenderzone != -1)                            
                            {                                
                                Logger.DebugWriteLine("**********firstattackerzone= " + attackerzone + "  firstdefenderzone= " + defenderzone);
                                return attacker;
                            }                       
                    }
                }                
            }
            return base.OnSelectAttacker(attackers,defenders);
        }
        */
        public bool MonsterRepos()
        {
            if (Bot.HasInMonstersZone(CardId.OddEyesWingDragon) || 
                Bot.HasInSpellZone(CardId.OddEyesWingDragon) || 
                Bot.HasInMonstersZone(CardId.OddEyesAbsoluteDragon))
            {
                if (Card.IsAttack())
                    return false;
            }
            if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) || (Bot.HasInHand(CardId.ApprenticeLllusionMagician)))
            {
                if (Card.IsAttack())
                    return false;
            }
            if (Card.IsFacedown())
                return true;
            return base.DefaultMonsterRepos();
        }

    }
}