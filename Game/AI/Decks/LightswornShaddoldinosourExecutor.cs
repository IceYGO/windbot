using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("LightswornShaddoldinosour", "AI_LightswornShaddoldinosour", "ver0.1")]
    public class LightswornShaddoldinosour : DefaultExecutor
    {
        public class CardId
        {
            //monster
            public const int UltimateConductorTytanno = 18940556;
            public const int DogorantheMadFlameKaiju = 93332803;
            public const int GamecieltheSeaTurtleKaiju = 55063751;
            public const int RadiantheMultidimensionalKaiju = 28674152;
            public const int OvertexCoatls = 41782653;
            public const int ShaddollBeast = 3717252;
            public const int GiantRex = 80280944;
            public const int ShaddollDragon = 77723643;
            public const int FairyTailSnow = 55623480;
            public const int KeeperOfDragonicMagic = 48048590;
            public const int ShaddollSquamata = 30328508;
            public const int SouleatingOviraptor = 44335251;
            public const int Raiden = 77558536;
            public const int Lumina = 95503687;
            public const int ShaddollHedgehog = 4939890;
            public const int AshBlossom = 14558127;
            public const int GhostOgre = 59438930;
            public const int ShaddollFalco = 37445295;
            public const int MaxxC = 23434538;
            public const int PlaguespreaderZombie = 33420078;
            public const int GlowUpBulb = 67441435;

            //spell
            public const int AllureofDarkness = 1475311;
            public const int ThatGrassLooksgreener = 11110587;
            public const int HarpiesFeatherDuster = 18144506;
            public const int DoubleEvolutionPill = 38179121;
            public const int ShaddollFusion = 44394295;
            public const int PotOfAvarice = 67169062;
            public const int FoolishBurial = 81439173;
            public const int MonsterReborn = 83764718;
            public const int ChargeOfTheLightBrigade = 94886282;
            public const int InterruptedKaijuSlumber = 99330325;
            public const int ElShaddollFusion = 6417578;

            //trap
            public const int infiniteTransience = 10045474;
            public const int LostWind = 74003290;
            public const int SinisterShadowGames = 77505534;
            public const int ShaddollCore = 4904633;

            //extra
            public const int ElShaddollShekhinaga = 74822425;
            public const int ElShaddollConstruct = 20366274;
            public const int ElShaddollGrysra = 48424886;
            public const int ElShaddollWinda = 94977269;
            public const int CrystalWingSynchroDragon = 50954680;
            public const int ScarlightRedDragon = 80666118;
            public const int Michael = 4779823;
            public const int BlackRoseMoonlightDragon = 33698022;
            public const int RedWyvern = 76547525;
            public const int CoralDragon = 42566602;
            public const int TG_WonderMagician = 98558751;
            public const int MinervaTheExalte = 30100551;
            public const int Sdulldeat = 74997493;
            public const int CrystronNeedlefiber = 50588353;
        }

        

        public LightswornShaddoldinosour(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
            
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC,MaxxC);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.infiniteTransience, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.ThatGrassLooksgreener);
            AddExecutor(ExecutorType.Summon, CardId.SouleatingOviraptor);
            AddExecutor(ExecutorType.Activate, CardId.SouleatingOviraptor, SouleatingOviraptoreff);
            AddExecutor(ExecutorType.Activate, CardId.AllureofDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, CardId.PotOfAvarice, PotofAvariceeff);            
            AddExecutor(ExecutorType.Activate, CardId.ChargeOfTheLightBrigade, ChargeOfTheLightBrigadeEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFusion, ShaddollFusioneff);
            //Reborn
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, RebornEffect);
            //Normal Summon
            AddExecutor(ExecutorType.Summon, CardId.Raiden);
            AddExecutor(ExecutorType.Activate, CardId.Raiden);
            AddExecutor(ExecutorType.Summon , CardId.KeeperOfDragonicMagic);
            AddExecutor(ExecutorType.Activate, CardId.KeeperOfDragonicMagic, KeeperOfDragonicMagiceff);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollSquamata);
            AddExecutor(ExecutorType.MonsterSet, CardId.GlowUpBulb);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollHedgehog);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollDragon);
            AddExecutor(ExecutorType.Summon, CardId.FairyTailSnow,FairyTailSnowsummon);
            AddExecutor(ExecutorType.Activate, CardId.FairyTailSnow, FairyTailSnoweff);
            AddExecutor(ExecutorType.Summon, CardId.Lumina);
            AddExecutor(ExecutorType.Activate, CardId.Lumina);
            //activate
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbeff);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefibereff);
            AddExecutor(ExecutorType.Activate, CardId.TG_WonderMagician);
            AddExecutor(ExecutorType.Activate, CardId.CoralDragon, CoralDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.RedWyvern, RedWyverneff);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.BlackRoseMoonlightDragon, BlackRoseMoonlightDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.Sdulldeat, Sdulldeateff);
            AddExecutor(ExecutorType.Activate, CardId.Michael, Michaeleff);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragon, ScarlightRedDragoneff);
            //Sp Summon
            //AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefibersp);
            AddExecutor(ExecutorType.SpSummon, CardId.UltimateConductorTytanno, UltimateConductorTytannosp);
            AddExecutor(ExecutorType.Activate, CardId.UltimateConductorTytanno, UltimateConductorTytannoeff);
            AddExecutor(ExecutorType.Activate, CardId.DoubleEvolutionPill, DoubleEvolutionPilleff);
            AddExecutor(ExecutorType.SpSummon, CardId.MinervaTheExalte);
            AddExecutor(ExecutorType.Activate, CardId.MinervaTheExalte, MinervaTheExaltedEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);         

            //activate chain
            AddExecutor(ExecutorType.Activate, CardId.OvertexCoatls, OvertexCoatlseff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollBeast);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFalco, ShaddollFalcoeff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollSquamata, ShaddollSquamataeff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollDragon, ShaddollDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollHedgehog, ShaddollHedgehogeff);
            AddExecutor(ExecutorType.Activate, CardId.GiantRex);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollConstruct, ElShaddollConstructeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollGrysra, ElShaddollGrysraeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollShekhinaga, ElShaddollShekhinagaeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollWinda);            
            //spellset
            AddExecutor(ExecutorType.SpellSet, CardId.MonsterReborn, spellset);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfAvarice, spellset);
            AddExecutor(ExecutorType.SpellSet, CardId.ThatGrassLooksgreener, spellset);
            //trapset
            AddExecutor(ExecutorType.SpellSet, CardId.LostWind, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SinisterShadowGames, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.ShaddollCore);
            AddExecutor(ExecutorType.SpellSet, CardId.infiniteTransience, SetIsFieldEmpty);
            //trap activate
            AddExecutor(ExecutorType.Activate, CardId.LostWind, LostWindeff);
            AddExecutor(ExecutorType.Activate, CardId.SinisterShadowGames, SinisterShadowGameseff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollCore, ShaddollCoreeff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }
        public int[] all_List()
        {
            return new[]
            {
                CardId.UltimateConductorTytanno,
                CardId.DogorantheMadFlameKaiju,
                CardId.GamecieltheSeaTurtleKaiju,
                CardId.RadiantheMultidimensionalKaiju,
                CardId.OvertexCoatls,
                CardId.ShaddollBeast,
                CardId.GiantRex,
                CardId.ShaddollDragon,
                CardId.FairyTailSnow,
                CardId.KeeperOfDragonicMagic,
                CardId.ShaddollSquamata,
                CardId.SouleatingOviraptor,
                CardId.Raiden,
                CardId.Lumina,
                CardId.ShaddollHedgehog,
                CardId.AshBlossom,
                CardId.GhostOgre,
                CardId.ShaddollFalco,
                CardId.MaxxC,
                CardId.PlaguespreaderZombie,
                CardId.GlowUpBulb,

                CardId.AllureofDarkness,
                CardId.ThatGrassLooksgreener,
                CardId.HarpiesFeatherDuster,
                CardId.DoubleEvolutionPill,
                CardId.ShaddollFusion,
                CardId.PotOfAvarice,
                CardId.FoolishBurial,
                CardId.MonsterReborn,
                CardId.ChargeOfTheLightBrigade,
                CardId.InterruptedKaijuSlumber,
                CardId.ElShaddollFusion,

                CardId.infiniteTransience,
                CardId.LostWind,
                CardId.SinisterShadowGames,
                CardId.ShaddollCore,


            };
        }
        public int[] Useless_List()
        {
            return new[]
            {
                CardId.GlowUpBulb,
                CardId.PlaguespreaderZombie,
                CardId.InterruptedKaijuSlumber,
                CardId.ChargeOfTheLightBrigade,
                CardId.FoolishBurial,
                CardId.HarpiesFeatherDuster,
                CardId.ThatGrassLooksgreener,
                CardId.FairyTailSnow,
                CardId.GiantRex,
                CardId.Lumina,
                CardId.OvertexCoatls,

            };
        }
        int Ultimate_ss = 0;
        bool Pillused = false;
        bool CrystronNeedlefibereff_used = false;
        bool OvertexCoatlseff_used = false;

        public override void OnNewTurn()
        {
            Pillused = false;
            OvertexCoatlseff_used = false;
            CrystronNeedlefibereff_used = false;
        }

        
        private bool UltimateConductorTytannoeff()
        {


            if (Duel.Phase == DuelPhase.Main1)
            {
                IList<int> targets = new[] {
                CardId.OvertexCoatls,
                CardId.ShaddollBeast,
                CardId.ShaddollSquamata,
                CardId.ShaddollHedgehog,
                CardId.ShaddollDragon,
                CardId.ShaddollFalco,
                CardId.GlowUpBulb,
                CardId.PlaguespreaderZombie,
                CardId.FairyTailSnow,
                CardId.KeeperOfDragonicMagic,
                CardId.Raiden,
                CardId.Lumina,
                CardId.DogorantheMadFlameKaiju,
                CardId.GamecieltheSeaTurtleKaiju,
                CardId.RadiantheMultidimensionalKaiju,
                CardId.GiantRex,               
                CardId.SouleatingOviraptor,
                CardId.Raiden,
                CardId.Lumina,
                CardId.AshBlossom,
                CardId.GhostOgre,
                CardId.MaxxC,
                };
                if (!Bot.HasInHand(targets) || !Bot.HasInMonstersZone(targets))
                {                    
                    return false;
                }
                AI.SelectCard(targets);
                return true;
            }
            if (Duel.Phase == DuelPhase.BattleStart)
            {
                AI.SelectYesNo(true);
                return true;
            }
            return false;    
            
        }

        private bool UltimateConductorTytannosp()
        {
            Ultimate_ss++;
            Pillused = true;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.Id == CardId.UltimateConductorTytanno && card.IsFaceup())
                    return false;
            }
            return true;

        }

        private bool KeeperOfDragonicMagiceff()
        {
            if (ActivateDescription == -1)
            {
                AI.SelectCard(Useless_List());
                return true;
            }


            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.Id == CardId.ElShaddollConstruct && Card.IsAttack())
                return false;
            return base.DefaultMonsterRepos();
        }

        private bool OvertexCoatlseff()
        {
            if (Card.Location == CardLocation.MonsterZone) return false;
            OvertexCoatlseff_used = true;
            return true;
        }

        private bool DoubleEvolutionPilleff()
        {          
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.Id == CardId.UltimateConductorTytanno && card.IsFaceup())
                    return false;
            }
            if (Pillused == true) return false;
            Pillused = true;
            IList<int> targets = new[] {
                    CardId.GiantRex,
                    CardId.DogorantheMadFlameKaiju,
                    CardId.GamecieltheSeaTurtleKaiju,
                    CardId.RadiantheMultidimensionalKaiju,
                    CardId.OvertexCoatls,
                    CardId.SouleatingOviraptor,
                    CardId.UltimateConductorTytanno,
                };
            if (Bot.HasInGraveyard(targets))
            {
                AI.SelectCard(new[] {
                    CardId.GiantRex,
                    CardId.DogorantheMadFlameKaiju,
                    CardId.OvertexCoatls,
                    CardId.GamecieltheSeaTurtleKaiju,
                    CardId.RadiantheMultidimensionalKaiju,
                    CardId.SouleatingOviraptor,
                    CardId.UltimateConductorTytanno,
                });
            }
            else
            {
                AI.SelectCard(new[] {
                    CardId.GiantRex,
                    CardId.DogorantheMadFlameKaiju,
                    CardId.GamecieltheSeaTurtleKaiju,
                    CardId.RadiantheMultidimensionalKaiju,
                    CardId.OvertexCoatls,
                    CardId.SouleatingOviraptor,
                    CardId.UltimateConductorTytanno,
                });
            }
            IList<int> targets2 = new[] {
                    CardId.GiantRex,
                    CardId.DogorantheMadFlameKaiju,
                    CardId.GamecieltheSeaTurtleKaiju,
                    CardId.RadiantheMultidimensionalKaiju,
                    CardId.OvertexCoatls,
                    CardId.SouleatingOviraptor,
                    CardId.UltimateConductorTytanno,
                };
            if (Bot.HasInGraveyard(targets))
            {
                AI.SelectNextCard(new[] {
                    CardId.ShaddollBeast,
                    CardId.ShaddollDragon,
                    CardId.KeeperOfDragonicMagic,
                    CardId.ShaddollSquamata,
                    CardId.SouleatingOviraptor,
                    CardId.Raiden,
                    CardId.Lumina,
                    CardId.ShaddollHedgehog,
                    CardId.AshBlossom,
                    CardId.GhostOgre,
                    CardId.ShaddollFalco,
                    CardId.MaxxC,
                    CardId.PlaguespreaderZombie,
                    CardId.GlowUpBulb,
                    CardId.FairyTailSnow,
                });
            }
            else
                AI.SelectNextCard(new[] {
                    CardId.ShaddollBeast,
                    CardId.ShaddollDragon,
                    CardId.KeeperOfDragonicMagic,
                    CardId.ShaddollSquamata,
                    CardId.SouleatingOviraptor,
                    CardId.Raiden,
                    CardId.Lumina,
                    CardId.ShaddollHedgehog,
                    CardId.AshBlossom,
                    CardId.GhostOgre,
                    CardId.ShaddollFalco,
                    CardId.MaxxC,
                    CardId.PlaguespreaderZombie,
                    CardId.GlowUpBulb,
                    CardId.FairyTailSnow,
                });

            AI.SelectThirdCard(new[] {
                    CardId.UltimateConductorTytanno,

                });

            return Enemy.GetMonsterCount() >= 1;
        }

        
        private bool FairyTailSnowsummon()
        {

            
            return Enemy.GetMonsterCount()>=2;
        }


        private bool FairyTailSnoweff()
        {

            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            return false;
        }


        private bool SouleatingOviraptoreff()
        {
            if (!OvertexCoatlseff_used)
            {
                AI.SelectCard(CardId.OvertexCoatls);
                AI.SelectYesNo(false);
            }
            else
            {
                AI.SelectCard(CardId.UltimateConductorTytanno);
                AI.SelectYesNo(true);
            }
            return true;
        }

        private bool GlowUpBulbeff()
        {
            if (Bot.HasInMonstersZone(CardId.Lumina) ||
               Bot.HasInMonstersZone(CardId.FairyTailSnow) ||
               Bot.HasInMonstersZone(CardId.KeeperOfDragonicMagic) ||
               Bot.HasInMonstersZone(CardId.SouleatingOviraptor) ||
               Bot.HasInMonstersZone(CardId.GiantRex) ||
               Bot.HasInMonstersZone(CardId.Raiden)
               )
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;   
        }       
       
        
        private bool AllureofDarkness()
        {
            IList<ClientCard> materials = Bot.Hand;
           // IList<ClientCard> check = new List<ClientCard>();
            ClientCard mat = null;
            foreach (ClientCard card in materials)
            {
                if (card.HasAttribute(CardAttribute.Dark))
                {
                    mat = card;
                    break;
                }
            }
            if (mat != null)
            {
                return true;
            }
            return false;
        }


        private bool spellset()
        {
            return Bot.Hand.Count > 6;
        }


        private bool RebornEffect()
        {
            if(Bot.HasInGraveyard(CardId.UltimateConductorTytanno)&&Ultimate_ss>0)
            {
                AI.SelectCard(CardId.UltimateConductorTytanno);
                return true;
            }
            IList<int> targets = new[] {                    
                    CardId.ElShaddollConstruct,
                    CardId.DogorantheMadFlameKaiju,
                    CardId.GamecieltheSeaTurtleKaiju,
                    CardId.SouleatingOviraptor,
                };
            if (!Bot.HasInGraveyard(targets))
            {
                return false;
            }
            AI.SelectCard(targets);
            return true;
        }


        private bool PotofAvariceeff()
        {
            return true;
        }

        private bool MaxxC()
        {
            return Duel.Player == 1;
        }


        private bool SetIsFieldEmpty()
        {
            return !Bot.IsFieldEmpty();
        }


        private bool TrapSetWhenZoneFree()
        {
            return Bot.GetSpellCountWithoutField() < 4;
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (!Bot.HasInHand(CardId.Raiden))
                AI.SelectCard(CardId.Raiden);
            else
                AI.SelectCard(new[]
                {
                    CardId.Lumina,

                });
            return true;
        }


        // all Shaddoll 
        private bool ElShaddollConstructeff()
        {
           /* if (Duel.Phase == DuelPhase.Battle)
                if (Enemy.BattlingMonster.Attack < 2800)
                    return false;*/
            if(ActivateDescription==-1)
            {
                AI.SelectCard(CardId.ShaddollSquamata);
            }
            return true;
        }


        private bool ElShaddollShekhinagaeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            else
            {
                if (DefaultBreakthroughSkill())
                {
                    AI.SelectCard(new[]
                    {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,
                }
                );
                }
                else
                    return false;
            }
            return true;
        }


        private bool ElShaddollGrysraeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;           
        return true;
        }

        private bool ShaddollFusioneff()
        {
            if (Bot.HasInMonstersZone(CardId.ElShaddollConstruct) ||
               Bot.HasInMonstersZone(CardId.ElShaddollGrysra) ||
               Bot.HasInMonstersZone(CardId.ElShaddollShekhinaga) ||
               Bot.HasInMonstersZone(CardId.ElShaddollWinda ))
                return false;

            bool deck_check = false;
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Synchro) || monster.HasType(CardType.Fusion) || monster.HasType(CardType.Xyz))
                    deck_check = true;
            }

            if (deck_check)
            {
                AI.SelectCard(new[]
                {
                    CardId.ElShaddollConstruct,
                    CardId.ElShaddollShekhinaga,
                    CardId.ElShaddollGrysra,
                    CardId.ElShaddollWinda
                });
                AI.SelectNextCard(new[]
                      {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,

                });
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            if (!Bot.IsFieldEmpty()) return false;


            foreach (ClientCard monster in Bot.Hand)
            {
                if (monster.HasAttribute(CardAttribute.Light))
                {
                    AI.SelectCard(CardId.ElShaddollConstruct);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }

            }
            List<ClientCard> material_1 = Bot.GetMonsters();
            foreach (ClientCard monster in material_1)
            {
                if (monster.HasAttribute(CardAttribute.Light))
                {
                    AI.SelectCard(CardId.ElShaddollConstruct);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }

            }
            return false;

        }

        private bool SinisterShadowGameseff()
        {

            AI.SelectCard(new[]
            {
                CardId.ShaddollBeast,

            });


            return true;
        }

        private bool ShaddollCoreeff()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if (Enemy.HasAttackingMonster() && Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart)
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }

                return false;
            }
            return true;
        }


        private bool ShaddollFalcoeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            else
            {
                AI.SelectCard(new[]
                {
                CardId.ElShaddollConstruct,
                CardId.ElShaddollShekhinaga,
                CardId.ElShaddollGrysra,
                CardId.ElShaddollWinda,
                CardId.ShaddollSquamata,
                }
                );

            }
            return true;
        }


        private bool ShaddollHedgehogeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
            {
                AI.SelectCard(new[]
                {
                CardId.ShaddollSquamata,
                });
            }
            else
            {
                AI.SelectCard(new[] { CardId.ShaddollFusion, CardId.SinisterShadowGames });
            }
            return true;
        }


        private bool ShaddollDragoneff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                ClientCard target = AI.Utils.GetBestEnemyCard();
                AI.SelectCard(target);
                return true;
            }
            else
            {
                ClientCard target = AI.Utils.GetBestEnemySpell();
                AI.SelectCard(target);
                return true;
            }
        }


        private bool ShaddollSquamataeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
            {
                AI.SelectCard(new[]
                {
                CardId.ShaddollBeast,
                });
            }
            else
            {
                ClientCard target = AI.Utils.GetBestEnemyMonster();
                AI.SelectCard(target);
                if (Enemy.GetMonsterCount() == 0)
                    return false;

            }
            return true;
        }

        private bool LostWindeff()
        {
            List<ClientCard> check = Enemy.GetMonsters();
            foreach (ClientCard m in check)
            {
                if (m.Attack>=2000) return DefaultBreakthroughSkill();
            }
            return false;            
        }

        private bool FoolishBurialEffect()
        {
            if (Bot.GetRemainingCount(CardId.DoubleEvolutionPill, 3) > 0)
            {
                if (!OvertexCoatlseff_used)
                {
                    AI.SelectCard(new[]
                        {
                        CardId.OvertexCoatls,
                    });
                    return true;
                }
                return false;
            }
            else
            {
                AI.SelectCard(new[]
                    {                        
                        CardId.ShaddollSquamata,
                        CardId.FairyTailSnow,
                    });
            }
            return true;
        }      
       
       
        public bool Hand_act_eff()
        {
            //if (Card.Id == CardId.Urara && Bot.HasInHand(CardId.LockBird) && Bot.HasInSpellZone(CardId.Re)) return false;
            if (Card.Id == CardId.GhostOgre && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.GhostOgre)) return false;
            return (Duel.LastChainPlayer == 1);
        }
        //other extra

        private bool Michaeleff()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Bot.LifePoints <= 1000) return false;
            return true;
        }

        private bool MinervaTheExaltedEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            else
            {
                IList<ClientCard> targets = new List<ClientCard>();

                ClientCard target1 = AI.Utils.GetBestEnemyMonster();
                if (target1 != null)
                    targets.Add(target1);
                ClientCard target2 = AI.Utils.GetBestEnemySpell();
                if (target2 != null)
                    targets.Add(target2);

                foreach (ClientCard target in Enemy.GetMonsters())
                {
                    if (targets.Count >= 3)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                foreach (ClientCard target in Enemy.GetSpells())
                {
                    if (targets.Count >= 3)
                        break;
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
                if (targets.Count == 0)
                    return false;

                AI.SelectNextCard(targets);
                return true;
            }
        }


        public bool CrystronNeedlefibersp()
        {
            if (Bot.HasInMonstersZone(CardId.ElShaddollConstruct) ||
                Bot.HasInMonstersZone(CardId.ElShaddollGrysra) ||
                Bot.HasInMonstersZone(CardId.ElShaddollShekhinaga) ||
                Bot.HasInMonstersZone(CardId.ElShaddollWinda))
                return false;

            if (CrystronNeedlefibereff_used) return false;
            if (Bot.HasInMonstersZone(CardId.CrystronNeedlefiber)) return false;
            if (Bot.HasInMonstersZone(CardId.FairyTailSnow) ||
                Bot.HasInMonstersZone(CardId.Lumina) ||
                Bot.HasInMonstersZone(CardId.KeeperOfDragonicMagic) ||
                Bot.HasInMonstersZone(CardId.SouleatingOviraptor) ||
                Bot.HasInMonstersZone(CardId.Raiden)
                )
            {
                AI.SelectCard(new[]
                    {
                    CardId.KeeperOfDragonicMagic,
                    CardId.Lumina,
                    CardId.FairyTailSnow,
                    CardId.SouleatingOviraptor,
                    CardId.Raiden,
                    CardId.GiantRex,
                    });
                AI.SelectNextCard(CardId.GlowUpBulb);
            }
            return true;
        }

        public bool CrystronNeedlefibereff()
        {
            if (Duel.Player == 0)
            {

                CrystronNeedlefibereff_used = true;
                AI.SelectCard(new[] { CardId.GhostOgre, CardId.GlowUpBulb, CardId.PlaguespreaderZombie, CardId.ShaddollFalco });
                return true;
            }
            else if (AI.Utils.IsChainTarget(Card) || AI.Utils.GetProblematicEnemySpell() != null) return true;
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && AI.Utils.IsOneEnemyBetterThanValue(1500, true))
            {
                if (AI.Utils.IsOneEnemyBetterThanValue(1900, true))
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                else
                {
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                }
                return true;
            }
            return false;
        }


        private bool ScarlightRedDragoneff()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            ClientCard target1 = AI.Utils.GetBestEnemyMonster();
            if (target1 != null)
            {
                targets.Add(target1);
                AI.SelectCard(targets);
                return true;
            }
            return false;
        }


        private bool CrystalWingSynchroDragoneff()
        {
            return Duel.LastChainPlayer != 0;
        }

        private bool Sdulldeateff()
        {
           /* if (snake_four_s)
            {
                snake_four_s = false;
                AI.SelectCard(Useless_List());
                return true;
            }
            //if (ActivateDescription == AI.Utils.GetStringId(CardId.snake, 2)) return true;
            if (ActivateDescription == AI.Utils.GetStringId(CardId.snake, 1))
            {
                foreach (ClientCard hand in Bot.Hand)
                {
                    if (hand.Id == CardId.Red || hand.Id == CardId.Pink)
                    {
                        AI.SelectCard(hand);
                        return true;
                    }
                    if (hand.Id == CardId.Urara || hand.Id == CardId.Ghost)
                    {
                        if (Tuner_ss())
                        {
                            AI.SelectCard(hand);
                            return true;
                        }
                    }
                }
            }*/
            return false;
        }
      
        private bool BlackRoseMoonlightDragoneff()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            ClientCard target1 = AI.Utils.GetBestEnemyMonster();
            if (target1 != null)
            {
                targets.Add(target1);
                AI.SelectCard(targets);
                return true;
            }
            return false;

        }


        private bool RedWyverneff()
        {
            IList<ClientCard> check = Enemy.MonsterZone;
            ClientCard best = null;
            foreach (ClientCard monster in check)
            {
                if (monster.Attack >= 2400) best = monster;
            }
            if (best != null)
            {
                AI.SelectCard(best);
                return true;
            }
            return false;
        }

        private bool CoralDragoneff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            IList<ClientCard> targets = new List<ClientCard>();

            ClientCard target1 = AI.Utils.GetBestEnemyMonster();
            if (target1 != null)
                targets.Add(target1);
            ClientCard target2 = AI.Utils.GetBestEnemySpell();
            if (target2 != null)
                targets.Add(target2);
            else if (AI.Utils.IsChainTarget(Card) || AI.Utils.GetProblematicEnemySpell() != null)
            {
                AI.SelectCard(targets);
                return true;
            }
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && AI.Utils.IsOneEnemyBetterThanValue(2400, true))
            {
                AI.SelectCard(targets);
                return true;
            }
            return false;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.Id == CardId.ElShaddollConstruct && !attacker.IsDisabled()) // TODO: && defender.IsSpecialSummoned
                    attacker.RealPower = 9999;
                if (attacker.Id == CardId.UltimateConductorTytanno && !attacker.IsDisabled() && defender.IsDefense())
                    attacker.RealPower = 9999;
            }            
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override bool OnSelectHand()
        {
            return true;
        }
        /*
        private bool GoblindberghSummon()
        {
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!card.Equals(Card) && card.Level == 4)
                    return true;
            }
            return false;
        }*/


    }
}