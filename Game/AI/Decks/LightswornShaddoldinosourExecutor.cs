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
            public const int KeeeperOfDragonicMagic = 48048590;
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
            public const int PotOfAvarice = 67169026;
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

        bool Pillused = false;

        public LightswornShaddoldinosour(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
           
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC,MaxxC);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.infiniteTransience, infiniteTransience);
            AddExecutor(ExecutorType.Activate, CardId.ThatGrassLooksgreener);
            AddExecutor(ExecutorType.Summon, CardId.SouleatingOviraptor);
            AddExecutor(ExecutorType.Activate, CardId.SouleatingOviraptor, SouleatingOviraptor);            
            AddExecutor(ExecutorType.Activate, CardId.AllureofDarkness, AllureofDarkness);
            AddExecutor(ExecutorType.Activate, CardId.PotOfAvarice, PotofAvarice);
            // AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);
            AddExecutor(ExecutorType.Activate, CardId.ChargeOfTheLightBrigade, ChargeOfTheLightBrigadeEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFusion, ShaddollFusion);
            //Reborn
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, RebornEffect);
            //Normal Summon
            AddExecutor(ExecutorType.Summon, CardId.Raiden);
            AddExecutor(ExecutorType.Activate, CardId.Raiden);
            
            AddExecutor(ExecutorType.Summon , CardId.KeeeperOfDragonicMagic);
            AddExecutor(ExecutorType.Activate, CardId.KeeeperOfDragonicMagic, KeeeperOfDragonicMagic);            
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollSquamata);
            AddExecutor(ExecutorType.MonsterSet, CardId.GlowUpBulb);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollHedgehog);
            AddExecutor(ExecutorType.MonsterSet, CardId.ShaddollDragon);
            AddExecutor(ExecutorType.Summon, CardId.FairyTailSnow,FairyTailSnow);
            AddExecutor(ExecutorType.Activate, CardId.FairyTailSnow, FairyTailSnow);
            AddExecutor(ExecutorType.Summon, CardId.Lumina);
            AddExecutor(ExecutorType.Activate, CardId.Lumina);
            //Sp Summon
            AddExecutor(ExecutorType.SpSummon, CardId.UltimateConductorTytanno, UltimateConductorTytannosp);
            AddExecutor(ExecutorType.Activate, CardId.UltimateConductorTytanno, UltimateConductorTytanno);
            AddExecutor(ExecutorType.Activate, CardId.DoubleEvolutionPill, DoubleEvolutionPill);
            AddExecutor(ExecutorType.SpSummon, CardId.MinervaTheExalte);
            AddExecutor(ExecutorType.Activate, CardId.MinervaTheExalte, MinervaTheExaltedEffect);
            
            
            
            //activate
            AddExecutor(ExecutorType.Activate , CardId.GlowUpBulb,GlowUpBulb);
            
            //activate chain
            AddExecutor(ExecutorType.Activate, CardId.OvertexCoatls);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollBeast);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollFalco, ShaddollFalco);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollSquamata, ShaddollSquamata);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollDragon);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollHedgehog);
            AddExecutor(ExecutorType.Activate, CardId.GiantRex);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollConstruct);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollGrysra);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollShekhinaga);
            AddExecutor(ExecutorType.Activate, CardId.ElShaddollWinda);
            AddExecutor(ExecutorType.Activate, CardId.TG_WonderMagician);
            //spellset
            AddExecutor(ExecutorType.SpellSet, CardId.MonsterReborn, spellset);
            AddExecutor(ExecutorType.SpellSet, CardId.PotOfAvarice, spellset);
            AddExecutor(ExecutorType.SpellSet, CardId.ThatGrassLooksgreener, spellset);
            //trap
            AddExecutor(ExecutorType.SpellSet, CardId.LostWind, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.SinisterShadowGames, TrapSetWhenZoneFree);
            AddExecutor(ExecutorType.SpellSet, CardId.ShaddollCore);         
            AddExecutor(ExecutorType.SpellSet, CardId.infiniteTransience, SetIsFieldEmpty);
            //trap activate
            AddExecutor(ExecutorType.Activate, CardId.LostWind, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.SinisterShadowGames, SinisterShadowGames);
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
                CardId.KeeeperOfDragonicMagic,
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

        private bool UltimateConductorTytanno()
        {
            AI.SelectCard(new[]
            {
                CardId.OvertexCoatls,
                CardId.ShaddollBeast,
                CardId.ShaddollSquamata,
                CardId.ShaddollHedgehog,
                CardId.ShaddollDragon,
                CardId.GlowUpBulb,
                CardId.PlaguespreaderZombie,
                CardId.FairyTailSnow,
            });

            
            return true;
        }
        private bool UltimateConductorTytannosp()
        {
            Pillused = true;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.Id == Card.Id && card.IsFaceup())
                    return false;
            }
            return true;

        }
        private bool KeeeperOfDragonicMagic()
        {
            AI.SelectCard(Useless_List());
            
            return true;
        }
        private bool MonsterRepos()
        {
            if (Card.Id == CardId.ElShaddollConstruct && Card.IsAttack())
                return false;
            return base.DefaultMonsterRepos();
        }
        public override void OnNewTurn()
        {
            Pillused = false;
        }
        private bool ShaddollCoreeff()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if(Enemy.HasAttackingMonster())
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
                
                return false;
            }
            return true;
        }
        private bool FairyTailSnow()
        {
            
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            return false;
        }
        private bool SouleatingOviraptor()
        {
            AI.SelectCard(CardId.OvertexCoatls);
            AI.SelectYesNo(false);
            return true;
        }
        private bool GlowUpBulb()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool ShaddollFusion()
        {
            if (Enemy.GetMonstersExtraZoneCount() != 0)
            {
                IList<ClientCard> materials0 = Bot.Deck;
                IList<ClientCard> materials1 = Bot.Deck;
                IList<ClientCard> mats = new List<ClientCard>();

                ClientCard mat = null;
                foreach (ClientCard card in materials0)
                {
                    if (card.HasAttribute(CardAttribute.Light))
                    {
                        mat = card;
                        break;
                    }
                }
                foreach (ClientCard card in materials1)
                {
                    AI.SelectCard(new[]
                      {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,

                });
                }
                if (mat != null)
                {
                    mats.Add(mat);

                    AI.SelectMaterials(mats);
                    AI.SelectCard(CardId.ElShaddollConstruct);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }


                foreach (ClientCard card in materials0)
                {
                    if (card.HasAttribute(CardAttribute.Earth))
                    {
                        mat = card;
                        break;
                    }
                }
                foreach (ClientCard card in materials1)
                {
                    AI.SelectCard(new[]
                      {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,

                });
                }
                if (mat != null)
                {
                    mats.Add(mat);

                    AI.SelectMaterials(mats);
                    AI.SelectCard(CardId.ElShaddollShekhinaga);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }


                foreach (ClientCard card in materials0)
                {
                    if (card.HasAttribute(CardAttribute.Fire))
                    {
                        mat = card;
                        break;
                    }
                }
                foreach (ClientCard card in materials1)
                {
                    AI.SelectCard(new[]
                      {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,

                });
                }
                if (mat != null)
                {
                    mats.Add(mat);

                    AI.SelectMaterials(mats);
                    AI.SelectCard(CardId.ElShaddollGrysra);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }


                foreach (ClientCard card in materials0)
                {
                    if (card.HasAttribute(CardAttribute.Dark))
                    {
                        mat = card;
                        break;
                    }
                }
                foreach (ClientCard card in materials1)
                {
                    AI.SelectCard(new[]
                      {
                    CardId.ShaddollBeast,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollDragon,
                    CardId.ShaddollFalco,

                });
                }
                if (mat != null)
                {
                    mats.Add(mat);

                    AI.SelectMaterials(mats);
                    AI.SelectCard(CardId.ElShaddollWinda);
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                    return true;
                }
            }
            else
            {
                if (!Bot.IsFieldEmpty())
                    return false;
                AI.SelectCard(CardId.ElShaddollConstruct);
                AI.SelectPosition(CardPosition.FaceUpAttack);
            }
            return true;

        }
       

       /* private ClientCard GetAleisterInGrave()
        {
            foreach (ClientCard card in Enemy.Graveyard)
            {
                if (card.Id == CardId.AleisterTheInvoker)
                {
                    return card;
                }
            }
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card.Id == CardId.AleisterTheInvoker)
                {
                    return card;
                }
            }
            return null;
        }*/
        private bool DoubleEvolutionPill()
        {
            if (Pillused == true)return false;
            Pillused = true;
            if (Bot.HasInGraveyard(CardId.UltimateConductorTytanno) )
            {
                AI.SelectCard(CardId.UltimateConductorTytanno);
            }
        

                AI.SelectThirdCard(new[] {
                    CardId.UltimateConductorTytanno,
                    
                });
            
            return Enemy.GetMonsterCount()>=1;
        }
        
        
        private bool ShaddollCore()
        {
            return Bot.HasInGraveyard(CardId.ShaddollFusion);
        }
        private bool AllureofDarkness()
        {
            IList<ClientCard> materials = Bot.Hand;
            IList<ClientCard> check = new List<ClientCard>();
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
            IList<int> targets = new[] {
                    CardId.UltimateConductorTytanno,
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
        private bool PotofAvarice()
        {
            return true;
        }
        private bool infiniteTransience()
        {
            return LastChainPlayer == 1;
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
        private bool SinisterShadowGames()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            else
            {
                AI.SelectCard(new[]
            {
                CardId.ShaddollBeast,
                CardId.ShaddollCore,
            }

                );
            }
            return true;
        }

        private bool ShaddollFalco()
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
        private bool ShaddollSquamata()
        {
            AI.SelectCard(new[]
            {
                CardId.ShaddollBeast,
                
            }
                );
            return true;
          
        }
        private bool FoolishBurialEffect()
        {
           
                AI.SelectCard(new[]
                {
                    CardId.OvertexCoatls,
                    CardId.ShaddollSquamata,
                    CardId.ShaddollBeast,
                    CardId.ShaddollCore,
                    CardId.ShaddollHedgehog,
                    CardId.ShaddollFalco,
                    CardId.ShaddollDragon,
                    CardId.FairyTailSnow,
                });
            return true;
        }
        

        private bool GoblindberghSummon()
        {
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!card.Equals(Card) && card.Level == 4)
                    return true;
            }
            return false;
        }

       

      
        private bool PerformageTrickClownEffect()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        public bool Hand_act_eff()
        {
            //if (Card.Id == CardId.Urara && Bot.HasInHand(CardId.LockBird) && Bot.HasInSpellZone(CardId.Re)) return false;
            if (Card.Id == CardId.GhostOgre && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.GhostOgre)) return false;
            return (LastChainPlayer == 1);
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
        private bool HonestEffect()
        {
            return Duel.Phase != DuelPhase.Main1;
        }

    }
}