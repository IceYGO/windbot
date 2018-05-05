using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("DarkMagician", "AI_DarkMagician", "NotFinished")]
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
            public const int SpellbookOfSecrets = 89739383;
            public const int DarkMagicInheritance = 41735184;
            public const int LllusionMagic = 73616671;
            public const int Scapegoat = 73915051;
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
            public const int EbonLllusionMagician = 96471335;
            public const int BorreloadDragon = 31833038;
            public const int SaryujaSkullDread = 74997493;
            public const int Hidaruma = 64514892;
            public const int AkashicMagician = 28776350;
            public const int SecurityDragon = 99111753;
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;

            public const int HarpiesFeatherDuster = 18144506;
            public const int ElShaddollWinda = 94977269;
            public const int DarkHole = 53129443;
            public const int Ultimate = 86221741;
            public const int LockBird = 94145021;
            public const int Ghost = 59438930;
            public const int GiantRex = 80280944;
            public const int UltimateConductorTytanno = 18940556;
        }

        public DarkMagicianExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrikeeff);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, ChainEnemy);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, ChainEnemy);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCeff);
            AddExecutor(ExecutorType.Activate, CardId.Scapegoat,Scapegoateff);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfSecrets,SpellbookOfSecreteff);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicInheritance, DarkMagicInheritanceeff);            
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicAttack, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicalCircle, DarkMagicalCircleeff);
            //trap set
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike);
            AddExecutor(ExecutorType.SpellSet, CardId.EternalSoul, EternalSoulset);            
            AddExecutor(ExecutorType.SpellSet, CardId.MagicianNavigation, MagicianNavigationset);
            AddExecutor(ExecutorType.SpellSet, CardId.Scapegoat, Scapegoatset);            
            //sheep
            AddExecutor(ExecutorType.SpSummon, CardId.Hidaruma, Hidarumasp);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonsp);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragoneff);
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
            AddExecutor(ExecutorType.Activate, CardId.TheEyeOfTimaeus);
            AddExecutor(ExecutorType.SpSummon, CardId.ApprenticeLllusionMagician, ApprenticeLllusionMagiciansp);
            AddExecutor(ExecutorType.Activate, CardId.ApprenticeLllusionMagician, ApprenticeLllusionMagicianeff);
            //other thing                     
            AddExecutor(ExecutorType.Activate, CardId.MagicianOfLllusion);
            AddExecutor(ExecutorType.Activate, CardId.MagicianNavigation, MagicianNavigationeff);
            AddExecutor(ExecutorType.Activate, CardId.EternalSoul, EternalSouleff);
            AddExecutor(ExecutorType.SpSummon, CardId.Dracossack, Dracossacksp);
            AddExecutor(ExecutorType.Activate, CardId.Dracossack, Dracossackeff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }
        bool Secret_used = false;
        bool plan_A = false;
        bool plan_C = false;
        int maxxc_done = 0;
        int lockbird_done = 0;
        bool maxxc_used = false;
        bool lockbird_used = false;
        bool WindwitchGlassBelleff_used = false;
        bool Spellbook_summon = false;
        bool Rod_summon = false;
        bool GlassBell_summon = false;
        bool magician_sp = false;
        bool soul_used = false;
        bool big_attack = false;
        bool big_attack_used = false;
        public override void OnNewPhase()
        {
            bool dangerous = false;
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.Id == CardId.Ultimate && !(Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) || Bot.HasInHand(CardId.ApprenticeLllusionMagician)))
                {
                    dangerous = true;
                }
            }
            if (dangerous)
                plan_C = true;
            if (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy)&&
              Bot.HasInHand(CardId.MagiciansRod)&&
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
                if(Bot.HasInSpellZone(CardId.EternalSoul)&& 
                    !(Bot.HasInHand(CardId.DarkMagician)||Bot.HasInHand(CardId.DarkMagician)))
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
            plan_C = false;
            Secret_used = false;            
            maxxc_used = false;
            lockbird_used = false;
            WindwitchGlassBelleff_used = false;
            Spellbook_summon = false;
            Rod_summon = false;
            GlassBell_summon = false;
            magician_sp = false;
            big_attack = false;
            big_attack_used = false;
            soul_used = false;
        }

       
        private bool WindwitchIceBelleff()
        {
            if (lockbird_used) return false;
            if (Enemy.HasInMonstersZone(CardId.ElShaddollWinda)) return false;
            if (maxxc_used) return false;
            if (WindwitchGlassBelleff_used) return false;
            AI.SelectCard(CardId.WindwitchGlassBell);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool WindwitchGlassBelleff()
        {
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell))
            {
                if (AI.Utils.ChainContainsCard(CardId.Ghost))
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
            if (Bot.HasInMonstersZone(CardId.WindwitchIceBell) &&
                 Bot.HasInMonstersZone(CardId.WindwitchGlassBell) &&
                 Bot.HasInMonstersZone(CardId.WindwitchSnowBell))
            {
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
            if(Card.Location==CardLocation.MonsterZone)
            {
                if(Duel.Player==1)
                    return DefaultTrap();
                return true;
            }
            return false;
        }
        private bool CrystalWingSynchroDragonsp()
        {
            if(Bot.HasInMonstersZone(CardId.WindwitchSnowBell)&&Bot.HasInMonstersZone(CardId.WindwitchWinterBell))
            {
                plan_A = true;
                return true;
            }
            return false;
        }

        private bool GrinderGolemeff()
        {
            
            if (plan_A) return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            if (Bot.GetMonstersExtraZoneCount() == 0)
                return true;
            if (Bot.HasInMonstersZone(CardId.AkashicMagician) ||
                Bot.HasInMonstersZone(CardId.SecurityDragon))
                return true;
            return false;
        }

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
                return true;
            return false;
        }

        private bool OddEyesAbsoluteDragoneff()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Card.Location == CardLocation.MonsterZone)
                return Duel.Player == 1;
            return false;
        }
           
        private bool SolemnStrikeeff()
        {
            if (Bot.LifePoints > 1500 && Duel.LastChainPlayer==1)
                return true;
            return false;
        }

        private bool ChainEnemy()
        {
            return Duel.LastChainPlayer == 1;
        }

        private bool MaxxCeff()
        {
            return Duel.Player == 1;
        }

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
                if (c.Id != CardId.WindwitchSnowBell && c.Level == 1 && c.Id != CardId.LinkSpider && c.Id != CardId.Linkuriboh)
                {
                    AI.SelectCard(c);
                    return true;
                }
            }
            return false;
        }


        private bool Linkuriboheff()
        {
            if (Duel.LastChainPlayer == 0 && AI.Utils.GetLastChainCard().Id == CardId.Linkuriboh) return false;
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
            ClientCard BestEnemy = AI.Utils.GetBestEnemyMonster(true,true);            
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            AI.SelectCard(BestEnemy);
            return true;  
        }
        private bool EternalSoulset()
        {
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
                if (check.Id == CardId.DarkMagician || check.Id == CardId.DarkMagicianTheDragonKnight)
                {
                    magician.Add(check);
                }
            }
            if (AI.Utils.IsChainTarget(Card) && Bot.GetMonsterCount() == 0)
            {
                AI.SelectYesNo(false);
                return true;
            }
            if (/*AI.Utils.ChainContainsCard(CardId.MagicianNavigation) ||
                AI.Utils.ChainContainsCard(CardId.WindwitchGlassBell)||
                AI.Utils.ChainContainsCard(CardId.DarkMagicalCircle)||
                AI.Utils.ChainContainsCard(CardId.MagiciansRod)||
                AI.Utils.ChainContainsCard(CardId.SpellbookMagicianOfProphecy)*/
                AI.Utils.ChainCountPlayer(0)>0
                )
                return false;
            if (Enemy.HasInSpellZone(CardId.HarpiesFeatherDuster) && Card.IsFacedown())
                return false;
            if (Enemy.HasInSpellZone(CardId.DarkHole) && Card.IsFacedown())
            {
                AI.SelectYesNo(false);
                return true;
            }                
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart)
            {
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(magician);
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    return true;
                }
            }
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.Main1)
            {
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(magician);
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.End)
            {
                if (Bot.HasInGraveyard(CardId.DarkMagicianTheDragonKnight) ||
                    Bot.HasInGraveyard(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(magician);
                    return true;
                }
                if (Bot.HasInHand(CardId.DarkMagician))
                {
                    soul_used = true;
                    magician_sp = true;
                    AI.SelectCard(CardId.DarkMagician);
                    return true;
                }
                return true;
            }
            return false;
        }

        private bool MagicianNavigationset()
        {
            if (Bot.HasInSpellZone(CardId.LllusionMagic)) return true;
            if (Bot.HasInHand(CardId.DarkMagician) && !Bot.HasInSpellZone(CardId.MagicianNavigation))
                return true;
            return false;
        }

        private bool MagicianNavigationeff()
        {
            bool spell_act = false;
            IList<ClientCard> spell = new List<ClientCard>();
            if(Duel.LastChainPlayer==1)
            {
                foreach (ClientCard check in Enemy.GetSpells())
                {
                    if (AI.Utils.GetLastChainCard()==check)
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
                if (check.Id==CardId.EternalSoul &&check.IsFaceup())
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
            if (AI.Utils.IsChainTarget(Card))
            {
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = AI.Utils.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                else
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                magician_sp = true;
                return true;
            }
            if (DefaultOnBecomeTarget() && !soul_faceup)
            {
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = AI.Utils.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                else
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                magician_sp = true;
                return true;
            }
                
            if (Enemy.GetMonsterCount()>=1 &&Duel.Phase==DuelPhase.BattleStart&& Card.Location==CardLocation.SpellZone && !maxxc_used)
            {
                AI.SelectCard(CardId.DarkMagician);
                ClientCard check = AI.Utils.GetOneEnemyBetterThanValue(2500, true);
                if (check != null)
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                else
                    AI.SelectNextCard(new[] {
                        CardId.ApprenticeLllusionMagician,
                        CardId.DarkMagician,
                        CardId.MagicianOfLllusion});
                magician_sp = true;
                return true;
            }
            
            return false;
        }
        private bool DarkMagicalCircleeff()
        {
            if(Card.Location==CardLocation.Hand)
            {
                if (Bot.LifePoints <= 4000)
                    return true;
                return UniqueFaceupSpell();
            }
            else if(magician_sp)
            {
                AI.SelectCard(AI.Utils.GetBestEnemyCard(false,true,CardId.GiantRex));
                magician_sp = false;
            }
            return true;
        }
        private bool LllusionMagicset()
        {
            if (Bot.GetMonsterCount() >= 1)
                return true;
            return false;
        }
        private bool LllusionMagiceff()
        {
            if (lockbird_used) return false;
            if (Duel.LastChainPlayer == 0) return false;
            bool soul_exist = false;
            foreach (ClientCard m in Bot.GetSpells())
            {
                if (m.Id == CardId.EternalSoul && m.IsFaceup())
                    soul_exist = true;
            }
            if (!soul_used && soul_exist)
            {
                if (Bot.HasInMonstersZone(CardId.MagiciansRod))
                {
                    AI.SelectCard(CardId.MagiciansRod);
                    AI.SelectNextCard(new[] { CardId.DarkMagician, CardId.DarkMagician });
                    return true;
                } 
            }
            if (Duel.Player == 0)
            {
                int count = 0;
                foreach (ClientCard m in Bot.GetMonsters())
                {
                    if (AI.Utils.IsChainTarget(m))
                        count++;
                }
                if (count == 0)
                    return false;                    
            }           
            
            if(Bot.HasInMonstersZone(CardId.MagiciansRod))
            {
                AI.SelectCard(CardId.MagiciansRod);
                AI.SelectNextCard(new[] { CardId.DarkMagician, CardId.DarkMagician });
                return true;
            }
            if(Duel.Player==1 && Bot.HasInMonstersZone(CardId.WindwitchGlassBell))
            {
                AI.SelectCard(CardId.WindwitchGlassBell);
                AI.SelectNextCard(new[] { CardId.DarkMagician, CardId.DarkMagician });
                return true;
            }
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.SpellbookOfSecrets))
            {
                AI.SelectCard(CardId.SpellbookOfSecrets);
                AI.SelectNextCard(new[] { CardId.DarkMagician, CardId.DarkMagician });
                return true;
            }
            return false;
        }
        private bool SpellbookMagicianOfProphecyeff()
        {
            Logger.DebugWriteLine("*********Secret_used= " + Secret_used);
            if(Secret_used)
            AI.SelectCard(CardId.SpellbookOfKnowledge);
            else
                AI.SelectCard(new[] { CardId.SpellbookOfSecrets,CardId.SpellbookOfKnowledge });
            return true;
        }
        private bool SpellbookOfSecreteff()
        {
            if (lockbird_used) return false;
            Secret_used = true;
            if (Bot.HasInHand(CardId.SpellbookMagicianOfProphecy))
                AI.SelectCard(CardId.SpellbookOfKnowledge);
            else
                AI.SelectCard(CardId.SpellbookMagicianOfProphecy);
            return true;
        }

        private bool SpellbookOfKnowledgeeff()
        {
            if (lockbird_used) return false;
            if (Bot.HasInSpellZone(CardId.LllusionMagic) && Bot.GetMonsterCount() < 2)
                return false;
            if(Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy)||
                Bot.HasInMonstersZone(CardId.MagiciansRod)||
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell)||
                Bot.HasInMonstersZone(CardId.WindwitchIceBell))
            {
                AI.SelectCard(new[]
                {
                    CardId.SpellbookMagicianOfProphecy,
                    CardId.MagiciansRod,
                    CardId.WindwitchGlassBell,
                });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetSpellCount() < 2 && Duel.Phase==DuelPhase.Main2)
            {
                AI.SelectCard(CardId.ApprenticeLllusionMagician);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.DarkMagician) &&
                    Bot.HasInMonstersZone(CardId.EternalSoul) && Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectCard(CardId.DarkMagician);
                return true;
            }
            return false;
        }

        private bool WonderWandeff()
        {
            if (lockbird_used) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInSpellZone(CardId.LllusionMagic) && Bot.GetMonsterCount() < 2)
                    return false;
                if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetSpellCount() < 2 && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.ApprenticeLllusionMagician);
                    return DefaultUniqueTrap();
                }
                if (Bot.HasInMonstersZone(CardId.SpellbookMagicianOfProphecy) ||
                Bot.HasInMonstersZone(CardId.MagiciansRod) ||
                Bot.HasInMonstersZone(CardId.WindwitchGlassBell) ||
                Bot.HasInMonstersZone(CardId.WindwitchIceBell))
                {
                    AI.SelectCard(new[]
                    {
                    CardId.SpellbookMagicianOfProphecy,
                    CardId.MagiciansRod,
                    CardId.WindwitchGlassBell,
                });
                    return DefaultUniqueTrap();
                }
                if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician) && Bot.GetHandCount() <= 3 && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.ApprenticeLllusionMagician);
                    return DefaultUniqueTrap();
                }

                if (Bot.HasInMonstersZone(CardId.DarkMagician) &&
                    Bot.HasInMonstersZone(CardId.EternalSoul) && Duel.Phase == DuelPhase.Main2)
                {
                    AI.SelectCard(CardId.DarkMagician);
                    return DefaultUniqueTrap();
                }
            }
            else
            {                
                if(Duel.Turn!=1)
                {
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                    AI.Utils.GetBestEnemyMonster(true,true) == null)
                        return false;
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                        AI.Utils.GetBestEnemyMonster().IsFacedown())
                        return true;
                    if (Duel.Phase == DuelPhase.Main1 && Enemy.GetSpellCountWithoutField() == 0 &&
                        AI.Utils.GetBestBotMonster()!=null &&
                        AI.Utils.GetBestBotMonster(true).Attack > AI.Utils.GetBestEnemyMonster(true).Attack)
                        return false;
                }                
                return true;
            }
            return false;
        }

        private bool ApprenticeLllusionMagiciansp()
        {
            if (Bot.HasInHand(CardId.SpellbookOfSecrets) ||
                  Bot.HasInHand(CardId.DarkMagician) ||
                  Bot.HasInHand(CardId.DarkMagicAttack))
            {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectCard(new[]
                {
                        CardId.SpellbookOfSecrets,
                        CardId.DarkMagician,
                        CardId.DarkMagicAttack,
                    });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician))
                return false;
            if (!Bot.HasInHand(CardId.DarkMagician))
                {
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    AI.SelectCard(new[]
                    {
                        CardId.TheEyeOfTimaeus,
                        CardId.ApprenticeLllusionMagician,
                        CardId.MagicianNavigation,
                    });
                    return true;
                }                
            return false;
        }
        private bool ApprenticeLllusionMagicianeff()
        {
            if (AI.Utils.ChainContainsCard(CardId.ApprenticeLllusionMagician)) return false;
            if (Duel.Phase == DuelPhase.Battle ||
                Duel.Phase == DuelPhase.BattleStart||
                Duel.Phase == DuelPhase.BattleStep ||
                Duel.Phase == DuelPhase.Damage||
                Duel.Phase == DuelPhase.DamageCal
                )
            {                
                if ((Bot.BattlingMonster == null))return false;
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
            if (lockbird_used) return false;
            if (Spellbook_summon)
            {
                if (Secret_used)
                    AI.SelectCard(CardId.SpellbookOfKnowledge);
                else
                    AI.SelectCard(new[] { CardId.SpellbookOfSecrets, CardId.SpellbookOfKnowledge });
                return true;
            } 
            return false;
        }       
        private bool MagiciansRodsummon()
        {
            if (lockbird_used) return false;
            if (Rod_summon) return true;
            return true;
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
            if (count>=2)
                if (Bot.HasInSpellZone(CardId.EternalSoul) && Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                    if (!Bot.HasInHand(CardId.DarkMagician) && !Bot.HasInGraveyard(CardId.DarkMagician))
                    {
                        AI.SelectCard(spell);
                        AI.SelectNextCard(CardId.LllusionMagic);
                        return true;
                    }

                if ((Bot.HasInSpellZone(CardId.MagicianNavigation) || Bot.HasInSpellZone(CardId.EternalSoul))
                    && !Bot.HasInHand(CardId.DarkMagician))
                {
                    AI.SelectCard(spell);
                    AI.SelectNextCard(CardId.LllusionMagic);
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                {
                    AI.SelectCard(spell);
                    AI.SelectNextCard(new[] {
                    CardId.EternalSoul,
                    CardId.MagicianNavigation,
                    CardId.DarkMagicalCircle});
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.EternalSoul) || Bot.HasInSpellZone(CardId.MagicianNavigation) ||
                    Bot.HasInSpellZone(CardId.MagicianNavigation) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                {
                    AI.SelectCard(spell);
                    AI.SelectNextCard(CardId.DarkMagicalCircle);
                    return true;
                }

            return false;    
        }
        private bool MagiciansRodeff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if(Bot.HasInSpellZone(CardId.EternalSoul)&&Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                    if(!Bot.HasInHand(CardId.DarkMagician)&&!Bot.HasInGraveyard(CardId.DarkMagician)&&
                        !Bot.HasInMonstersZone(CardId.DarkMagician) && Bot.GetRemainingCount(CardId.LllusionMagic, 1) == 1)
                    {
                        AI.SelectCard(CardId.LllusionMagic);
                        return true;
                    }
                
                if ((Bot.HasInHand(CardId.MagicianNavigation) || Bot.HasInSpellZone(CardId.MagicianNavigation)||
                    Bot.HasInSpellZone(CardId.EternalSoul))
                    && !(Bot.HasInHand(CardId.DarkMagician) || Bot.HasInGraveyard(CardId.DarkMagician))
                    &&Bot.GetRemainingCount(CardId.LllusionMagic,1)==1)
                {
                    AI.SelectCard(CardId.LllusionMagic);
                    return true;
                }
                if ((Bot.HasInSpellZone(CardId.EternalSoul) || Bot.HasInSpellZone(CardId.MagicianNavigation) ||
                    Bot.HasInHand(CardId.MagicianNavigation)) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                {
                    AI.SelectCard(CardId.DarkMagicalCircle);
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                {
                    AI.SelectCard(new[] {
                    CardId.EternalSoul,
                    CardId.MagicianNavigation,
                    CardId.DarkMagicalCircle});
                    return true;
                }
                
                AI.SelectCard(new[]
                {
                    CardId.EternalSoul,
                    CardId.DarkMagicalCircle,
                    CardId.MagicianNavigation,
                });
                return true;
            }
            else
                return false;            
        }

        private bool WindwitchGlassBellsummon()
        {
            if (lockbird_used) return false;
            if (!plan_A && (Bot.HasInGraveyard(CardId.WindwitchGlassBell) || Bot.HasInMonstersZone(CardId.WindwitchGlassBell)))
                return false;
            if (WindwitchGlassBelleff_used) return false;
            if (GlassBell_summon) return true;
            return false;
        }

        private bool Dracossacksp()
        {
            if (plan_C) return false;
            if (AI.Utils.IsOneEnemyBetterThanValue(2500, false) && 
                !Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician))
            {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }                
            return false;
        }

        private bool Dracossackeff()
        {
            if(ActivateDescription==AI.Utils.GetStringId(CardId.Dracossack,0))
            {
                AI.SelectCard(CardId.DarkMagician);
                return true;

            }
            ClientCard target = AI.Utils.GetBestEnemyCard(false,true);
            if (target != null)
            {
                AI.SelectCard(CardId.Dracossack + 1);
                AI.SelectNextCard(target);
                return true;
            }
            return false;
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnChainEnd()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Enemy.MonsterZone[i] != null)
                    Logger.DebugWriteLine("++++++++MONSTER ZONE[" + i + "]= " + Enemy.MonsterZone[i].Id);
            }
               
            if (Duel.CurrentChain.Count==1 &&  AI.Utils.GetLastChainCard().Id==0)
             {
                Logger.DebugWriteLine("current chain = " + Duel.CurrentChain.Count);
                Logger.DebugWriteLine("******last chain card= " + AI.Utils.GetLastChainCard().Id);
                int maxxc_count = 0;
                    foreach (ClientCard check in Enemy.Graveyard)
                    {
                    if (check.Id == CardId.MaxxC)
                        maxxc_count++;
                    }
                    if (maxxc_count!=maxxc_done) 
                    {
                        Logger.DebugWriteLine("************************last chain card= " + AI.Utils.GetLastChainCard().Id);
                        maxxc_used = true;
                    }
                    int lockbird_count = 0;
                    foreach (ClientCard check in Enemy.Graveyard)
                    {
                        if (check.Id == CardId.LockBird)
                            lockbird_count++;
                    }
                if (lockbird_count !=lockbird_done)
                {
                    Logger.DebugWriteLine("************************last chain card= " + AI.Utils.GetLastChainCard().Id);
                    lockbird_used = true;
                }
            } 
            bool dangerous = false;
            // Logger.DebugWriteLine("*********dangerous= "+dangerous);
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.Id==CardId.Ultimate && !(Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician)||Bot.HasInHand(CardId.ApprenticeLllusionMagician)))
                {
                    dangerous = true;
                }
            }
            if (dangerous)
                plan_C = true;
            int count = 0;
            foreach (ClientCard check in Enemy.Graveyard)
            {
                if (check.Id == CardId.MaxxC)
                    count++;
            }
            maxxc_done = count;
            count = 0;
            foreach (ClientCard check in Enemy.Graveyard)
            {
                if (check.Id == CardId.LockBird)
                    count++;
            }
            lockbird_done = count;
        }


        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            bool attack_improve = false;
            int count = 0;
            foreach (ClientCard check in Bot.GetMonsters())
            {
                if (Bot.HasInMonstersZone(CardId.ApprenticeLllusionMagician))
                {
                    attack_improve = true;
                    count++;
                }
            }
            if (Bot.HasInSpellZone(CardId.OddEyesWingDragon))
                big_attack = true;
            if(Duel.Player==0 && Bot.GetMonsterCount()>=2 && plan_C)
            {
                Logger.DebugWriteLine("*********dangerous********************* ");
                if (attacker.Id == CardId.OddEyesAbsoluteDragon || attacker.Id == CardId.OddEyesWingDragon)
                    attacker.RealPower = 9999;
            }
            if (attacker.Race.Equals(CardRace.SpellCaster)&& attacker.HasAttribute(CardAttribute.Dark) &&
                attack_improve && attacker.Id!=CardId.ApprenticeLllusionMagician)
            {                
                attacker.RealPower += 2000;    
            }
            if(attacker.Id==CardId.ApprenticeLllusionMagician && count>=2)
            {
                attacker.RealPower += 2000;
            }
            if (attacker.Id == CardId.CrystalWingSynchroDragon && defender.Level >= 5)
                attacker.RealPower = 9999;
            if (!big_attack_used && big_attack && !(attacker.Id == CardId.OddEyesAbsoluteDragon || attacker.Id == CardId.OddEyesWingDragon))
            {
                attacker.RealPower += defender.RealPower;
                big_attack_used = true;
                return true;
            }
            if (Bot.HasInSpellZone(CardId.EternalSoul) && 
                (attacker.Id == CardId.DarkMagician || attacker.Id == CardId.DarkMagicianTheDragonKnight || attacker.Id == CardId.MagicianOfLllusion))
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

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
            return base.DefaultMonsterRepos();
        }

    }
}