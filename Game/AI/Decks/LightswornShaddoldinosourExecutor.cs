using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("LightswornShaddoldinosour", "AI_LightswornShaddoldinosour")]
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
            //public const int ElShaddollFusion = 6417578;

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
            AddExecutor(ExecutorType.Activate, CardId.InterruptedKaijuSlumber, InterruptedKaijuSlumbereff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFusion, ShaddollFusioneff);
            //Normal Summon            
            AddExecutor(ExecutorType.Summon, CardId.Raiden);
            AddExecutor(ExecutorType.Activate, CardId.Raiden);            
            AddExecutor(ExecutorType.Summon , CardId.KeeperOfDragonicMagic);
            AddExecutor(ExecutorType.Activate, CardId.KeeperOfDragonicMagic, KeeperOfDragonicMagiceff);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollSquamata);
            AddExecutor(ExecutorType.MonsterSet, CardId.GlowUpBulb);
            AddExecutor(ExecutorType.Summon, CardId.Lumina, Luminasummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollHedgehog);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollDragon);
            AddExecutor(ExecutorType.Summon, CardId.FairyTailSnow,FairyTailSnowsummon);
            AddExecutor(ExecutorType.Activate, CardId.FairyTailSnow, FairyTailSnoweff);            
            AddExecutor(ExecutorType.Activate, CardId.Lumina, Luminaeff);
            //activate
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbeff);            
            AddExecutor(ExecutorType.Activate, CardId.TG_WonderMagician, TG_WonderMagicianeff);
            AddExecutor(ExecutorType.Activate, CardId.CoralDragon, CoralDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.RedWyvern, RedWyverneff);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.BlackRoseMoonlightDragon, BlackRoseMoonlightDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.Sdulldeat, Sdulldeateff);
            AddExecutor(ExecutorType.Activate, CardId.Michael, Michaeleff);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragon, ScarlightRedDragoneff);
            //Sp Summon
            
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefibereff);
            AddExecutor(ExecutorType.SpSummon, CardId.UltimateConductorTytanno, UltimateConductorTytannosp);
            AddExecutor(ExecutorType.Activate, CardId.UltimateConductorTytanno, UltimateConductorTytannoeff);
            AddExecutor(ExecutorType.Activate, CardId.DoubleEvolutionPill, DoubleEvolutionPilleff);
            //extra
            AddExecutor(ExecutorType.SpSummon, CardId.CrystalWingSynchroDragon);
            AddExecutor(ExecutorType.Activate, CardId.CrystalWingSynchroDragon, CrystalWingSynchroDragoneff);
            AddExecutor(ExecutorType.SpSummon, CardId.ScarlightRedDragon, ScarlightRedDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.ScarlightRedDragon, ScarlightRedDragoneff);
            AddExecutor(ExecutorType.SpSummon, CardId.Michael, Michaelsp);
            AddExecutor(ExecutorType.Activate, CardId.Michael, Michaeleff);
            AddExecutor(ExecutorType.SpSummon, CardId.RedWyvern, RedWyvernsp);
            AddExecutor(ExecutorType.Activate, CardId.RedWyvern, RedWyverneff);
            AddExecutor(ExecutorType.SpSummon, CardId.MinervaTheExalte);
            AddExecutor(ExecutorType.Activate, CardId.MinervaTheExalte, MinervaTheExaltedEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefibersp);
            //Kaiju
            AddExecutor(ExecutorType.SpSummon, CardId.GamecieltheSeaTurtleKaiju, GamecieltheSeaTurtleKaijusp);            
            AddExecutor(ExecutorType.SpSummon, CardId.RadiantheMultidimensionalKaiju, RadiantheMultidimensionalKaijusp);
            AddExecutor(ExecutorType.SpSummon, CardId.DogorantheMadFlameKaiju, DogorantheMadFlameKaijusp);
            //Reborn
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, Reborneff);
            //activate chain
            AddExecutor(ExecutorType.Activate, CardId.OvertexCoatls, OvertexCoatlseff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollCore, ShaddollCoreeff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollBeast, ShaddollBeasteff);            
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFalco, ShaddollFalcoeff);            
            AddExecutor(ExecutorType.Activate, CardId.ShaddollDragon, ShaddollDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollHedgehog, ShaddollHedgehogeff);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollSquamata, ShaddollSquamataeff);
            AddExecutor(ExecutorType.Activate, CardId.GiantRex);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollConstruct, ElShaddollConstructeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollGrysra, ElShaddollGrysraeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollShekhinaga, ElShaddollShekhinagaeff);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollWinda);            
            //spellset          
            AddExecutor(ExecutorType.SpellSet, CardId.ThatGrassLooksgreener, SpellSetZone);
            AddExecutor(ExecutorType.SpellSet, SpellSetZone);
            //trapset
            AddExecutor(ExecutorType.SpellSet, CardId.LostWind);
            AddExecutor(ExecutorType.SpellSet, CardId.SinisterShadowGames);
            AddExecutor(ExecutorType.SpellSet, CardId.ShaddollCore);
            AddExecutor(ExecutorType.SpellSet, CardId.infiniteTransience, SetIsFieldEmpty);
            //trap activate
            AddExecutor(ExecutorType.Activate, CardId.LostWind, LostWindeff);
            AddExecutor(ExecutorType.Activate, CardId.SinisterShadowGames, SinisterShadowGameseff);
            
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
                //CardId.ElShaddollFusion,

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
                CardId.ChargeOfTheLightBrigade,                
                CardId.ThatGrassLooksgreener,
                CardId.HarpiesFeatherDuster,
                CardId.FairyTailSnow,
                CardId.GiantRex,
                CardId.Lumina,
                CardId.OvertexCoatls,
                CardId.InterruptedKaijuSlumber,                
                CardId.FoolishBurial,
            };
        }
        int Ultimate_ss = 0;
        int Enemy_atk = 0;
        bool Pillused = false;
        bool CrystronNeedlefibereff_used = false;
        bool OvertexCoatlseff_used = false;
        bool ShaddollBeast_used = false;
        bool ShaddollFalco_used = false;
        bool ShaddollSquamata_used = false;
        bool ShaddollDragon_used = false;
        bool ShaddollHedgehog_used = false;

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

        public override void OnNewPhase()
        {
            Enemy_atk = 0;
            IList<ClientCard> list = new List<ClientCard>();
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if(monster.IsAttack())
                list.Add(monster);
            }
            //if (GetTotalATK(list) / 2 >= Bot.LifePoints) return false;
            Enemy_atk = GetTotalATK(list);
            //SLogger.DebugWriteLine("++++++++++++++++++" + Enemy_atk + "++++++++++++");
        }
        public override void OnNewTurn()
        {
            Pillused = false;
            OvertexCoatlseff_used = false;
            CrystronNeedlefibereff_used = false;
            ShaddollBeast_used = false;
            ShaddollFalco_used = false;
            ShaddollSquamata_used = false;
            ShaddollDragon_used = false;
            ShaddollHedgehog_used = false;
            base.OnNewTurn();
        }

        private bool Luminasummon()
        {
            if (Bot.Deck.Count >= 20) return true;
            IList<ClientCard> extra = Bot.GetMonstersInExtraZone();
            if (extra != null)
                foreach (ClientCard monster in extra)
                    if (!monster.HasType(CardType.Link))
                        return false;            
            if (Bot.LifePoints <= 3000) return true;
            if (Bot.HasInGraveyard(CardId.Raiden)) return true;
            return false;
        }
        private bool Luminaeff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.HasInGraveyard(CardId.Raiden))
            {
                AI.SelectCard(Useless_List());
                AI.SelectNextCard(CardId.Raiden);
                return true;
            }
            return false;
        }


        private bool UltimateConductorTytannoeff()
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
                CardId.DogorantheMadFlameKaiju,
                CardId.GamecieltheSeaTurtleKaiju,
                CardId.RadiantheMultidimensionalKaiju,
                CardId.GiantRex,
                CardId.ShaddollCore,
                CardId.SouleatingOviraptor,
                CardId.Raiden,
                CardId.Lumina,
                CardId.AshBlossom,
                CardId.GhostOgre,
                CardId.MaxxC,
                };

            if (Duel.Phase == DuelPhase.Main1)
            {
                if(Duel.Player==0)
                {
                    int count = 0;
                    IList<ClientCard> check = Enemy.GetMonsters();
                    foreach (ClientCard monster in check)
                        if (monster.Attack > 2500 || monster == Enemy.MonsterZone.GetDangerousMonster())
                            count++;
                    if(count==0)return false;
                }               
                if (!Bot.HasInHand(targets))
                {
                    if(!Bot.HasInMonstersZone(targets))
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

        private bool GamecieltheSeaTurtleKaijusp()
        {
            if (!Bot.HasInMonstersZone(CardId.UltimateConductorTytanno))
                return DefaultKaijuSpsummon();
            return false;
        }

        private bool RadiantheMultidimensionalKaijusp()
        {
            if (Enemy.HasInMonstersZone(CardId.GamecieltheSeaTurtleKaiju)) return true;
            if (Bot.HasInHand(CardId.DogorantheMadFlameKaiju) && !Bot.HasInMonstersZone(CardId.UltimateConductorTytanno)) return DefaultKaijuSpsummon();
            return false;
        }


        private bool DogorantheMadFlameKaijusp()
        {
            if (Enemy.HasInMonstersZone(CardId.GamecieltheSeaTurtleKaiju)) return true;
            if (Enemy.HasInMonstersZone(CardId.RadiantheMultidimensionalKaiju)) return true;
            return false;
        }


        private bool InterruptedKaijuSlumbereff()
        {
            if (Enemy.GetMonsterCount() - Bot.GetMonsterCount() >= 2 )
                return DefaultInterruptedKaijuSlumber();
            return false;
        }
        private bool UltimateConductorTytannosp()
        {
            
            Pillused = true;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.IsCode(CardId.UltimateConductorTytanno) && card.IsFaceup())
                    return false;
            }
            Ultimate_ss++;
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
            if (Card.IsCode(CardId.UltimateConductorTytanno) && Card.IsFacedown()) return true;
            if (Card.IsCode(CardId.ElShaddollConstruct) && Card.IsFacedown()) return true;
            if (Card.IsCode(CardId.ElShaddollConstruct) && Card.IsAttack()) return false;
            if (Card.IsCode(CardId.GlowUpBulb) && Card.IsDefense()) return false;
            if (Card.IsCode(CardId.ShaddollDragon) && Card.IsFacedown() && Enemy.GetMonsterCount() >= 0) return true;
            if (Card.IsCode(CardId.ShaddollSquamata) && Card.IsFacedown() && Enemy.GetMonsterCount() >= 0) return true;
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
                if (card.IsCode(CardId.UltimateConductorTytanno) && card.IsFaceup())
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
                AI.SelectCard(CardId.GiantRex, CardId.DogorantheMadFlameKaiju, CardId.OvertexCoatls, CardId.GamecieltheSeaTurtleKaiju, CardId.RadiantheMultidimensionalKaiju, CardId.SouleatingOviraptor, CardId.UltimateConductorTytanno);
            }
            else
            {
                AI.SelectCard(CardId.GiantRex, CardId.DogorantheMadFlameKaiju, CardId.GamecieltheSeaTurtleKaiju, CardId.RadiantheMultidimensionalKaiju, CardId.OvertexCoatls, CardId.SouleatingOviraptor, CardId.UltimateConductorTytanno);
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
                AI.SelectNextCard(CardId.ShaddollBeast, CardId.ShaddollDragon, CardId.KeeperOfDragonicMagic, CardId.ShaddollSquamata, CardId.SouleatingOviraptor, CardId.Raiden, CardId.Lumina, CardId.ShaddollHedgehog, CardId.AshBlossom, CardId.GhostOgre, CardId.ShaddollFalco, CardId.MaxxC, CardId.PlaguespreaderZombie, CardId.GlowUpBulb, CardId.FairyTailSnow);
            }
            else
                AI.SelectNextCard(CardId.ShaddollBeast, CardId.ShaddollDragon, CardId.KeeperOfDragonicMagic, CardId.ShaddollSquamata, CardId.SouleatingOviraptor, CardId.Raiden, CardId.Lumina, CardId.ShaddollHedgehog, CardId.AshBlossom, CardId.GhostOgre, CardId.ShaddollFalco, CardId.MaxxC, CardId.PlaguespreaderZombie, CardId.GlowUpBulb, CardId.FairyTailSnow);

            AI.SelectThirdCard(new[] {
                    CardId.UltimateConductorTytanno,

                });

            return Enemy.GetMonsterCount() >= 1;
        }

        
        private bool FairyTailSnowsummon()
        {
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if(target != null)
            {
                return true;
            }            
            return false;
        }


        private bool FairyTailSnoweff()
        {

            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                return true;
            }
            else
            {
               
                int spell_count = 0;
                IList<ClientCard> grave = Bot.Graveyard;               
                IList<ClientCard> all = new List<ClientCard>();
                foreach (ClientCard check in grave)
                {
                    if (check.IsCode(CardId.GiantRex))
                    {
                        all.Add(check);
                    }
                }
                foreach (ClientCard check in grave)
                    {
                        if(check.HasType(CardType.Spell)||check.HasType(CardType.Trap))
                        {
                            spell_count++;
                            all.Add(check);
                        }                        
                    }
                foreach (ClientCard check in grave)
                {
                    if (check.HasType(CardType.Monster))
                    {                       
                        all.Add(check);
                    }
                }
                if (Util.ChainContainsCard(CardId.FairyTailSnow)) return false;

                if ( Duel.Player == 1  && Duel.Phase == DuelPhase.BattleStart && Bot.BattlingMonster == null && Enemy_atk >=Bot.LifePoints ||
                    Duel.Player == 0 && Duel.Phase==DuelPhase.BattleStart && Enemy.BattlingMonster == null && Enemy.LifePoints<=1850
                    )
                {                  
                    AI.SelectCard(all);
                    AI.SelectNextCard(Util.GetBestEnemyMonster());
                    return true;
                }
            }
            return false;
        }


        private bool SouleatingOviraptoreff()
        {
            if (!OvertexCoatlseff_used && Bot.GetRemainingCount(CardId.OvertexCoatls, 3) > 0)
            {
                AI.SelectCard(CardId.OvertexCoatls);
                AI.SelectOption(0);
            }
            else
            {
                AI.SelectCard(CardId.UltimateConductorTytanno);
                AI.SelectOption(1);
            }
            return true;
        }

        private bool GlowUpBulbeff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            IList<ClientCard> check = Bot.GetMonstersInExtraZone();
            foreach (ClientCard monster in check)
                if (monster.HasType(CardType.Fusion)) return false;       
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
       
        private bool TG_WonderMagicianeff()
        {
            return true;
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
             

        private bool Reborneff()
        {
            if(Bot.HasInGraveyard(CardId.UltimateConductorTytanno)&&Ultimate_ss>0)
            {
                AI.SelectCard(CardId.UltimateConductorTytanno);
                return true;
            }
            if (!Util.IsOneEnemyBetter(true)) return false;
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
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player == 1;
        }


        private bool SetIsFieldEmpty()
        {
            return !Bot.IsFieldEmpty();
        }


        private bool SpellSetZone()
        {
            return (Bot.GetHandCount()>6 && Duel.Phase==DuelPhase.Main2);
        }

        private bool ChargeOfTheLightBrigadeEffect()
        {
            if (Bot.HasInGraveyard(CardId.Raiden) || Bot.HasInHand(CardId.Raiden))
                AI.SelectCard(CardId.Lumina);
            else
                AI.SelectCard(CardId.Raiden);
            return true;
        }


        // all Shaddoll 
        private bool SinisterShadowGameseff()
        {
            if (Bot.HasInGraveyard(CardId.ShaddollFusion))
                AI.SelectCard(CardId.ShaddollCore);
            else
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
                
                if (Duel.Player == 1 && Bot.BattlingMonster == null && Duel.Phase==DuelPhase.BattleStart|| DefaultOnBecomeTarget())
                {
                    Logger.DebugWriteLine("+++++++++++ShaddollCoreeffdododoo++++++++++");
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
                return false;
            }
            return true;
        }


        private bool ShaddollFusioneff()
        {
            List<ClientCard> extra_zone_check = Bot.GetMonstersInExtraZone();
            foreach (ClientCard extra_monster in extra_zone_check)
                if (extra_monster.HasType(CardType.Xyz) || extra_monster.HasType(CardType.Fusion) || extra_monster.HasType(CardType.Synchro)) return false;

            bool deck_check = false;
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Synchro) || monster.HasType(CardType.Fusion) || monster.HasType(CardType.Xyz) || monster.HasType(CardType.Link))
                    deck_check = true;
            }

            if (deck_check)
            {
                AI.SelectCard(
                    CardId.ElShaddollConstruct,
                    CardId.ElShaddollShekhinaga,
                    CardId.ElShaddollGrysra,
                    CardId.ElShaddollWinda
                    );
                AI.SelectNextCard(
                    CardId.ShaddollSquamata,
                    CardId.ShaddollBeast,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,
                    CardId.FairyTailSnow
                    );
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            if (Enemy.GetMonsterCount() == 0)
            {
                int dark_count = 0;
                IList<ClientCard> m0 = Bot.Hand;
                IList<ClientCard> m1 = Bot.MonsterZone;
                IList<ClientCard> all = new List<ClientCard>();
                foreach (ClientCard monster in m0)
                {
                    if (dark_count == 2) break;
                    if (monster.HasAttribute(CardAttribute.Dark))
                    {
                        dark_count++;
                        all.Add(monster);
                    }
                }
                foreach (ClientCard monster in m1)
                {
                    if (dark_count == 2) break;
                    if (monster != null)
                    {
                        if (monster.HasAttribute(CardAttribute.Dark))
                        {
                            dark_count++;
                            all.Add(monster);
                        }
                    }


                }
                if (dark_count == 2)
                {
                    AI.SelectCard(CardId.ElShaddollWinda);
                    AI.SelectMaterials(all);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }
            }
            if (!Util.IsOneEnemyBetter()) return false;


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
                if (monster == null) break;
                if (monster.HasAttribute(CardAttribute.Light))
                {
                    AI.SelectCard(CardId.ElShaddollConstruct);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }

            }
            return false;

        }

        
        private bool ElShaddollShekhinagaeff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            else
            {
                if (DefaultBreakthroughSkill())
                {
                    AI.SelectCard(
                        CardId.ShaddollBeast,
                        CardId.ShaddollSquamata,
                        CardId.ShaddollHedgehog,
                        CardId.ShaddollDragon,
                        CardId.ShaddollFalco
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


        private bool ElShaddollConstructeff()
        {

            if (!ShaddollBeast_used)
                AI.SelectCard(CardId.ShaddollBeast);
            else
                AI.SelectCard(CardId.ShaddollFalco);

            return true;
        }


        private bool ShaddollSquamataeff()
        {
            ShaddollSquamata_used = true;
            if (Card.Location != CardLocation.MonsterZone)
            {
                if(Util.ChainContainsCard(CardId.ElShaddollConstruct))
                {
                    if (!Bot.HasInHand(CardId.ShaddollFusion) && Bot.HasInGraveyard(CardId.ShaddollFusion))
                    AI.SelectNextCard(CardId.ShaddollCore);
                    if (!ShaddollBeast_used) AI.SelectNextCard(CardId.ShaddollBeast);
                    else if (!ShaddollFalco_used) AI.SelectNextCard(CardId.ShaddollFalco);
                    else  if(!ShaddollHedgehog_used) AI.SelectNextCard(CardId.ShaddollHedgehog);                    
                }
                else
                {
                    if (!Bot.HasInHand(CardId.ShaddollFusion) && Bot.HasInGraveyard(CardId.ShaddollFusion))
                        AI.SelectCard(CardId.ShaddollCore);
                    if (!ShaddollBeast_used) AI.SelectCard(CardId.ShaddollBeast);
                    else if (!ShaddollFalco_used) AI.SelectCard(CardId.ShaddollFalco);
                    else if (!ShaddollHedgehog_used) AI.SelectCard(CardId.ShaddollHedgehog);
                }

            }
            else
            {
                if (Enemy.GetMonsterCount() == 0) return false;
                ClientCard target = Util.GetBestEnemyMonster();
                AI.SelectCard(target);
            }
            return true;
        }
        

        private bool ShaddollBeasteff()
        {
            ShaddollBeast_used = true;
            return true;
        }


        private bool ShaddollFalcoeff()
        {
            ShaddollFalco_used = true;
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            else
            {
                AI.SelectCard(
                    CardId.ElShaddollConstruct,
                    CardId.ElShaddollShekhinaga,
                    CardId.ElShaddollGrysra,
                    CardId.ElShaddollWinda,
                    CardId.ShaddollSquamata
                    );

            }
            return true;
        }


        private bool ShaddollHedgehogeff()
        {
            ShaddollHedgehog_used = true;
            if (Card.Location != CardLocation.MonsterZone)
            {
                if (Util.ChainContainsCard(CardId.ElShaddollConstruct))
                {
                    AI.SelectNextCard(
                        CardId.ShaddollFalco,
                        CardId.ShaddollSquamata,
                        CardId.ShaddollDragon
                        );

                }
                else
                {
                    AI.SelectCard(
                        CardId.ShaddollSquamata,
                        CardId.ShaddollDragon
                        );
                }

            }
            else
            {
                AI.SelectCard(
                    CardId.ShaddollFusion,
                    CardId.SinisterShadowGames
                    );
            }
            return true;
        }


        private bool ShaddollDragoneff()
        {
            ShaddollDragon_used = true;
            if (Card.Location == CardLocation.MonsterZone)
            {
                ClientCard target = Util.GetBestEnemyCard();
                AI.SelectCard(target);
                return true;
            }
            else
            {
                if (Enemy.GetSpellCount() == 0) return false;
                ClientCard target = Util.GetBestEnemySpell();
                AI.SelectCard(target);
                return true;
            }
        }
        
        
        private bool LostWindeff()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
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
                AI.SelectCard(CardId.ShaddollSquamata, CardId.FairyTailSnow);
            }
            return true;
        }      
       
       
        public bool Hand_act_eff()
        {
            //if (Card.IsCode(CardId.Urara) && Bot.HasInHand(CardId.LockBird) && Bot.HasInSpellZone(CardId.Re)) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.IsCode(CardId.GhostOgre) && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.GhostOgre)) return false;
            return (Duel.LastChainPlayer == 1);
        }
        //other extra

        private bool Michaelsp()
        {
            IList<int> targets = new[] {
                   CardId.Raiden,
                   CardId.Lumina
                };
            if (!Bot.HasInMonstersZone(targets))
                return false;
            AI.SelectCard(targets);
            return true;
        }
        private bool Michaeleff()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Bot.LifePoints <= 1000) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            ClientCard select = Util.GetBestEnemyCard();
            if (select == null) return false;
            if(select!=null)
            {
                
                AI.SelectCard(select);
                return true;                    
            }            
            return false;
        }

        private bool MinervaTheExaltedEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.Deck.Count <= 10) return false;
                return true;
            }
            else
            {
                IList<ClientCard> targets = new List<ClientCard>();

                ClientCard target1 = Util.GetBestEnemyMonster();
                if (target1 != null)
                    targets.Add(target1);
                ClientCard target2 = Util.GetBestEnemySpell();
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

                AI.SelectCard(0);
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
            IList<int> check = new[]
            {
                CardId.GlowUpBulb,
                CardId.FairyTailSnow,
                CardId.KeeperOfDragonicMagic,
                CardId.SouleatingOviraptor,
                CardId.GiantRex,
                CardId.Lumina,
                CardId.Raiden,

            };
            int count=0;
            foreach (ClientCard monster in Bot.GetMonsters())
                if (monster.IsCode(CardId.GlowUpBulb, CardId.FairyTailSnow, CardId.KeeperOfDragonicMagic,
                    CardId.SouleatingOviraptor, CardId.GiantRex, CardId.Lumina, CardId.Raiden))
                    count++;
            if (!Bot.HasInMonstersZone(CardId.GlowUpBulb) || count<2)
                return false;
            AI.SelectCard(check);
            AI.SelectNextCard(check);
           
            return true;
        }

        public bool CrystronNeedlefibereff()
        {
            bool DarkHole = false;
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card.IsCode(53129443) && card.IsFaceup())
                {
                    DarkHole = true;
                }
            }
            if (Duel.Player == 0)
            {

                CrystronNeedlefibereff_used = true;
                AI.SelectCard(
                    CardId.GhostOgre,
                    CardId.GlowUpBulb,
                    CardId.PlaguespreaderZombie,
                    CardId.ShaddollFalco
                    );
                return true;
            }
            
            else if (DarkHole || Util.IsChainTarget(Card) || Util.GetProblematicEnemySpell() != null)
            {
                AI.SelectCard(CardId.TG_WonderMagician);
                return true;
            }
                
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Util.IsOneEnemyBetterThanValue(1500, true))
            {
                AI.SelectCard(CardId.TG_WonderMagician);
                if (Util.IsOneEnemyBetterThanValue(1900, true))
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

        private bool ScarlightRedDragonsp()
        {
            return false;
        }

        private bool ScarlightRedDragoneff()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            ClientCard target1 = Util.GetBestEnemyMonster();
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
            //if (ActivateDescription == Util.GetStringId(CardId.snake, 2)) return true;
            if (ActivateDescription == Util.GetStringId(CardId.snake, 1))
            {
                foreach (ClientCard hand in Bot.Hand)
                {
                    if (hand.IsCode(CardId.Red, CardId.Pink))
                    {
                        AI.SelectCard(hand);
                        return true;
                    }
                    if (hand.IsCode(CardId.Urara, CardId.Ghost))
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
            ClientCard target1 = Util.GetBestEnemyMonster();
            if (target1 != null)
            {
                targets.Add(target1);
                AI.SelectCard(targets);
                return true;
            }
            return false;

        }

        private bool RedWyvernsp()
        {
            return false;
        }

        private bool RedWyverneff()
        {
            IList<ClientCard> check = Enemy.GetMonsters();
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

            ClientCard target1 = Util.GetBestEnemyMonster();
            if (target1 != null)
                targets.Add(target1);
            ClientCard target2 = Util.GetBestEnemySpell();
            if (target2 != null)
                targets.Add(target2);
            else if (Util.IsChainTarget(Card) || Util.GetProblematicEnemySpell() != null)
            {
                AI.SelectCard(targets);
                return true;
            }
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Util.IsOneEnemyBetterThanValue(2400, true))
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
                if (attacker.IsCode(CardId.ElShaddollConstruct) && !attacker.IsDisabled()) // TODO: && defender.IsSpecialSummoned
                    attacker.RealPower = 9999;
                if (attacker.IsCode(CardId.UltimateConductorTytanno) && !attacker.IsDisabled() && defender.IsDefense())
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