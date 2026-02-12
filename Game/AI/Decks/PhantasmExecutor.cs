using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Phantasm", "AI_Phantasm")]
    public class PhantasmExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int MegalosmasherX = 81823360;
            public const int AshBlossom = 14558127;
            public const int EaterOfMillions = 63845230;

            public const int HarpieFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
            public const int FossilDig = 47325505;
            public const int CardOfDemise = 59750328;
            public const int Terraforming = 73628505;
            public const int PotOfDuality = 98645731;
            public const int Scapegoat = 73915051;
            public const int PacifisThePhantasmCity = 2819435;

            public const int InfiniteImpermanence = 10045474;
            public const int PhantasmSprialBattle = 34302287;
            public const int DrowningMirrorForce = 47475363;
            public const int StarlightRoad = 58120309;
            public const int PhantasmSpiralPower = 61397885;
            public const int Metaverse = 89208725;
            public const int SeaStealthAttack = 19089195;
            public const int GozenMatch = 53334471;
            public const int SkillDrain = 82732705;
            public const int TheHugeRevolutionIsOver = 99188141;

            public const int StardustDragon = 44508094;
            public const int TopologicBomberDragon = 5821478;
            public const int BorreloadDragon = 31833038;
            public const int BorrelswordDragon = 85289965;
            public const int KnightmareGryphon = 65330383;
            public const int TopologicTrisbaena = 72529749;
            public const int SummonSorceress = 61665245;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int KnightmareCerberus = 75452921;
            public const int CrystronNeedlefiber = 50588353;
            public const int MissusRadiant = 3987233;
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;

            public const int ElShaddollWinda = 94977269;
            public const int BrandishSkillJammingWave = 25955749;
            public const int BrandishSkillAfterburner = 99550630;
            public const int EternalSoul = 48680970;
            public const int SuperboltThunderDragon = 15291624;
        }

        public PhantasmExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.StarlightRoad, PreventFeatherDustereff);
            AddExecutor(ExecutorType.Activate, CardId.TheHugeRevolutionIsOver, PreventFeatherDustereff);
            AddExecutor(ExecutorType.Activate, _CardId.GhostBelle, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, DefaultGhostOgreAndSnowRabbit);

            //trap activate  
            AddExecutor(ExecutorType.Activate, CardId.SeaStealthAttack, SeaStealthAttackeff);
            AddExecutor(ExecutorType.Activate, CardId.PhantasmSprialBattle, PhantasmSprialBattleeff);
            AddExecutor(ExecutorType.Activate, CardId.PhantasmSpiralPower, PhantasmSpiralPowereff);
            AddExecutor(ExecutorType.Activate, CardId.DrowningMirrorForce, DrowningMirrorForceeff);
            AddExecutor(ExecutorType.Activate, CardId.GozenMatch, GozenMatcheff);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain, SkillDraineff);
            AddExecutor(ExecutorType.Activate, CardId.Metaverse, Metaverseeff);
            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.BorrelswordDragon, BorrelswordDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BorrelswordDragon, BorrelswordDragoneff);           
            AddExecutor(ExecutorType.SpSummon, CardId.MissusRadiant, MissusRadiantsp);
            AddExecutor(ExecutorType.Activate, CardId.MissusRadiant, MissusRadianteff);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            //first           
            AddExecutor(ExecutorType.Activate, CardId.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.FossilDig, FossilDigeff);           
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, Terraformingeff);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityeff);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, PotOfDesireseff);
            AddExecutor(ExecutorType.Activate, CardId.PacifisThePhantasmCity, PacifisThePhantasmCityeff);
            //summon
            AddExecutor(ExecutorType.Summon, CardId.MegalosmasherX, MegalosmasherXsummon);
            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.EaterOfMillions, EaterOfMillionssp);
            AddExecutor(ExecutorType.Activate, CardId.EaterOfMillions, EaterOfMillionseff);
            //other

            AddExecutor(ExecutorType.Activate, CardId.Scapegoat, DefaultScapegoat);
            AddExecutor(ExecutorType.SpellSet, CardId.SeaStealthAttack, NoSetAlreadyDone);
            AddExecutor(ExecutorType.SpellSet, CardId.StarlightRoad, StarlightRoadset);
            AddExecutor(ExecutorType.SpellSet, CardId.TheHugeRevolutionIsOver, TheHugeRevolutionIsOverset);
            AddExecutor(ExecutorType.SpellSet, CardId.DrowningMirrorForce);
            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, InfiniteImpermanenceset);
            AddExecutor(ExecutorType.SpellSet, CardId.Scapegoat, NoSetAlreadyDone);
            AddExecutor(ExecutorType.SpellSet, CardId.GozenMatch, NoSetAlreadyDone);
            AddExecutor(ExecutorType.SpellSet, CardId.SkillDrain, NoSetAlreadyDone);
            AddExecutor(ExecutorType.SpellSet, CardId.Metaverse);
            AddExecutor(ExecutorType.SpellSet, SpellSeteff);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseeff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }
        bool summon_used = false;
        bool CardOfDemiseeff_used = false;
        bool SeaStealthAttackeff_used = false;
        public override void OnNewTurn()
        {            
            summon_used = false;
            CardOfDemiseeff_used = false;
            SeaStealthAttackeff_used = false;
            base.OnNewTurn();
        }
        private bool PreventFeatherDustereff()
        {
            return Duel.LastChainPlayer == 1;          
        }

        private bool GoToBattlePhase()
        {           
            if (Enemy.GetMonsterCount() == 0)
            {
                if (Util.GetTotalAttackingMonsterAttack(0) >= Enemy.LifePoints)
                {                   
                    return true;
                }
            }
            return false;
        }

        private bool PhantasmSprialBattleeff()
        {
            if (DefaultOnBecomeTarget() && Card.Location==CardLocation.SpellZone)
            {
                AI.SelectCard(Util.GetBestEnemyCard(false,true));
                return true;
            }
            if(Enemy.HasInSpellZone(CardId.EternalSoul))
            {
                AI.SelectCard(CardId.EternalSoul);
                return UniqueFaceupSpell();
            }
            if(Bot.UnderAttack && Bot.BattlingMonster != null && Bot.BattlingMonster.IsCode(CardId.MegalosmasherX))
            {
                AI.SelectCard(Enemy.BattlingMonster);
                return UniqueFaceupSpell();
            }
            if (Bot.GetMonsterCount() > 0 && !Bot.HasInSpellZone(CardId.SeaStealthAttack) &&
                Util.IsOneEnemyBetterThanValue(2000, false) && Duel.Phase==DuelPhase.BattleStart)
            {
                AI.SelectCard(Util.GetBestEnemyMonster(true,true));
                return UniqueFaceupSpell();
            }
            if (Util.GetProblematicEnemyCard(9999,true)!=null)
            {
                if (Util.GetProblematicEnemyCard(9999, true).IsCode(CardId.ElShaddollWinda) &&
                    !Util.GetProblematicEnemyCard(9999, true).IsDisabled())
                    return false;
                AI.SelectCard(Util.GetProblematicEnemyCard(9999, true));                
                return UniqueFaceupSpell();
            }
            return false;
        }

        private bool PhantasmSpiralPowereff()
        {
            if (DefaultOnBecomeTarget() && Card.Location == CardLocation.SpellZone) return true;
            if(Duel.Player == 0 || (Duel.Player==1 && Bot.BattlingMonster!=null))
            {
                if(Enemy.HasInMonstersZone(CardId.ElShaddollWinda))
                {
                    AI.SelectCard(CardId.ElShaddollWinda);
                    return UniqueFaceupSpell();
                }
                if(Enemy.HasInMonstersZone(CardId.SuperboltThunderDragon))
                {
                    AI.SelectCard(CardId.SuperboltThunderDragon);
                    return UniqueFaceupSpell();
                }
            }            
            return DefaultInfiniteImpermanence() && UniqueFaceupSpell();
        }

        private bool DrowningMirrorForceeff()
        {
            int count = 0;
            foreach(ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsAttack()) count++;                    
            }
            if (Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints)
                return true;
            return count >= 2;
        }

        private bool GozenMatcheff()
        {
            if (Bot.GetMonsterCount() >= 4 || Bot.HasInSpellZone(CardId.Scapegoat)) return false;
            if (DefaultOnBecomeTarget()) return true;
            int dark_count = 0;
            int Divine_count = 0;
            int Earth_count = 0;
            int Fire_count = 0;
            int Light_count = 0;
            int Water_count = 0;
            int Wind_count = 0;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.HasAttribute(CardAttribute.Dark)) dark_count++;
                if (m.HasAttribute(CardAttribute.Divine)) Divine_count++;
                if (m.HasAttribute(CardAttribute.Earth)) Earth_count++;
                if (m.HasAttribute(CardAttribute.Fire)) Fire_count++;
                if (m.HasAttribute(CardAttribute.Light)) Light_count++;
                if (m.HasAttribute(CardAttribute.Water)) Water_count++;
                if (m.HasAttribute(CardAttribute.Wind)) Wind_count++;
            }
            if (dark_count > 1) dark_count = 1;
            if (Divine_count > 1) Divine_count = 1;
            if (Earth_count > 1) Earth_count = 1;
            if (Fire_count > 1) Fire_count = 1;
            if (Light_count > 1) Light_count = 1;
            if (Water_count > 1) Water_count = 1;
            if (Wind_count > 1) Wind_count = 1;
            return ((dark_count + Divine_count + Earth_count + Fire_count + Light_count + Water_count + Wind_count) >= 2 && UniqueFaceupSpell());
        }

        private bool SkillDraineff()
        {
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().Location == CardLocation.MonsterZone)
                return UniqueFaceupSpell();
            return false;
        }

        private bool Metaverseeff()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (!Bot.HasInSpellZone(CardId.PacifisThePhantasmCity))
            {
                AI.SelectOption(1);
                return UniqueFaceupSpell();
            }
            else
            {
                AI.SelectOption(0);
                return UniqueFaceupSpell();                    
            }
        }

        private bool CardOfDemiseeff()
        {
            if (DefaultSpellWillBeNegated()) return false;
            AI.SelectPlace(Zones.z2);
            if(Card.Location==CardLocation.Hand)
            {
                if (Bot.Hand.Count <= 1 && Bot.GetSpellCountWithoutField() <= 3)
                {
                    CardOfDemiseeff_used = true;
                    return true;
                }
            }
            else
            {
                if (Bot.Hand.Count <= 1 && Bot.GetSpellCountWithoutField() <= 4)
                {
                    CardOfDemiseeff_used = true;
                    return true;
                }
            }
            return false;
        }

        private bool FossilDigeff()
        {
            if (DefaultSpellWillBeNegated()) return false;
            if (CardOfDemiseeff_used && summon_used) return false;
            return true;
        }

        private bool PotOfDualityeff()
        {
            if(!Bot.HasInHandOrInSpellZone(CardId.PacifisThePhantasmCity) &&
                !Bot.HasInHandOrInSpellZone(CardId.Metaverse))
            {
                if(Bot.HasInGraveyard(CardId.PacifisThePhantasmCity) && !Bot.HasInHandOrInSpellZone(CardId.SeaStealthAttack))
                {
                    AI.SelectCard(
                        CardId.SeaStealthAttack,
                        CardId.PacifisThePhantasmCity,
                        CardId.Terraforming,
                        CardId.Metaverse,
                        CardId.CardOfDemise,
                        CardId.Scapegoat
                        );
                }
                else
                {
                    AI.SelectCard(
                        CardId.PacifisThePhantasmCity,
                        CardId.Terraforming,
                        CardId.Metaverse,
                        CardId.CardOfDemise,
                        CardId.Scapegoat
                        );
                }
                
            }
            else if(!Bot.HasInHandOrInSpellZone(CardId.SeaStealthAttack))
            {
                AI.SelectCard(
                    CardId.SeaStealthAttack,
                    CardId.CardOfDemise,
                    CardId.PotOfDesires,
                    CardId.Scapegoat
                    );
            }
            else
            {
                AI.SelectCard(
                    CardId.CardOfDemise,
                    CardId.PotOfDesires,
                    CardId.Scapegoat
                    );
            }
            return true;
        }
        private bool Terraformingeff()
        {
            if (DefaultSpellWillBeNegated()) return false;
            if (CardOfDemiseeff_used && Bot.HasInSpellZone(CardId.PacifisThePhantasmCity)) return false;
            return true;
        }
        
        private bool PacifisThePhantasmCityeff()
        {
            if (DefaultSpellWillBeNegated()) return false;
            if(Card.Location==CardLocation.Hand)
            {
                if (Bot.HasInSpellZone(CardId.PacifisThePhantasmCity))
                    return false;
                return true;
            }
            else
            {
                ClientCard target = null;
                foreach(ClientCard s in Bot.GetSpells())
                {
                    if(s.IsCode(CardId.SeaStealthAttack) && Card.IsFaceup())
                    {
                        target = s;
                        break;
                    }
                }
                foreach(ClientCard m in Bot.GetMonsters())
                {
                    if(m.HasAttribute(CardAttribute.Water))
                    {
                        if (target != null && !SeaStealthAttackeff_used)
                        {
                            if (Util.IsChainTarget(Card) || Util.IsChainTarget(target))
                                return false;
                        }
                        break;
                    }
                }
                AI.SelectPlace(Zones.z1 | Zones.z3);
                AI.SelectCard(CardId.PhantasmSprialBattle);
                return true;
            }
        }

        private bool MegalosmasherXsummon()
        {
            AI.SelectPlace(Zones.z1 | Zones.z3);
            summon_used = true;
            return true;
        }

        private bool BorrelswordDragonsp()
        {
           
            if (!Bot.HasInMonstersZone(CardId.MissusRadiant))
                return false;
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.IsCode(CardId.MissusRadiant))
                {
                    material_list.Add(m);
                    break;
                }
            }
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.IsCode(CardId.Linkuriboh, CardId.LinkSpider))
                {
                    material_list.Add(m);
                    if (material_list.Count == 3)
                        break;
                }
            }
            if (material_list.Count == 3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        private bool BorrelswordDragoneff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BorrelswordDragon, 0))
            {               
                if (Util.IsChainTarget(Card) && Util.GetBestEnemyMonster(true, true) != null)
                {
                    AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                    return true;
                }
                if (Duel.Player == 1 && Bot.BattlingMonster == Card)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player == 1 && Bot.BattlingMonster != null &&
                    (Enemy.BattlingMonster.Attack - Bot.BattlingMonster.Attack) >= Bot.LifePoints)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                {
                    foreach (ClientCard check in Enemy.GetMonsters())
                    {
                        if (check.IsAttack() && !check.HasType(CardType.Link))
                        {
                            AI.SelectCard(check);
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;
        }
        private bool EaterOfMillionssp()
        {
            if (Bot.MonsterZone[1] == null)
                AI.SelectPlace(Zones.z1);
            else
                AI.SelectPlace(Zones.z3);
            if (Enemy.HasInMonstersZone(CardId.KnightmareGryphon, true)) return false;         
            if (Util.GetProblematicEnemyMonster() == null && Bot.ExtraDeck.Count < 5) return false;
            if (Bot.GetMonstersInMainZone().Count >= 5) return false;
            if (Util.IsTurn1OrMain2()) return false;
            AI.SelectPosition(CardPosition.FaceUpAttack);
            IList<ClientCard> material_list = new List<ClientCard>();
            if(Bot.HasInExtra(CardId.BorreloadDragon))
            {
                AI.SelectMaterials(new[] {
                    CardId.TopologicBomberDragon,
                    CardId.TopologicTrisbaena,
                    CardId.KnightmareGryphon,
                    CardId.SummonSorceress,
                    CardId.BorreloadDragon
                }, HintMsg.Remove);
            }
            else 
            {               
                foreach(ClientCard m in Bot.ExtraDeck)
                {
                    if (material_list.Count == 5) break;
                    material_list.Add(m);
                }
                AI.SelectMaterials(material_list, HintMsg.Remove);
            }
            return true;
        }

        private bool EaterOfMillionseff()
        {
            if (Enemy.BattlingMonster.HasPosition(CardPosition.Attack) && (Bot.BattlingMonster.Attack - Enemy.BattlingMonster.GetDefensePower() >= Enemy.LifePoints)) return false;
            return true;
        }

        private bool MissusRadiantsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Earth) && monster.Level == 1 && !monster.IsCode(CardId.EaterOfMillions))
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.MissusRadiant)) return false;
            AI.SelectMaterials(material_list);
            if ((Bot.MonsterZone[0] == null || Bot.MonsterZone[0].Level==1) &&
                (Bot.MonsterZone[2] == null || Bot.MonsterZone[2].Level == 1)&&
                Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool MissusRadianteff()
        {
            AI.SelectCard(new[]
           {              
                CardId.MissusRadiant,
            });
            return true;
        }

        private bool Linkuribohsp()
        {
            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (!c.IsCode(CardId.EaterOfMillions, CardId.Linkuriboh) && c.Level == 1)
                {
                    AI.SelectMaterials(c);
                    return true;
                }
            }
            return false;
        }

        private bool Linkuriboheff()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Linkuriboh)) return false;
            return true;
        }
        private bool SeaStealthAttackeff()
        {            
            if (DefaultOnBecomeTarget())
            {
                AI.SelectCard(CardId.MegalosmasherX);
                SeaStealthAttackeff_used = true;
                return true;
            }
            if ((Card.IsFacedown() && Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.PacifisThePhantasmCity)))
            {
                if (!Bot.HasInSpellZone(CardId.PacifisThePhantasmCity))
                {
                    if(Bot.HasInGraveyard(CardId.PacifisThePhantasmCity))
                    {
                        foreach (ClientCard s in Bot.GetGraveyardSpells())
                        {
                            if (s.IsCode(CardId.PacifisThePhantasmCity))
                            {
                                AI.SelectYesNo(true);
                                AI.SelectCard(s);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (ClientCard s in Bot.Hand)
                        {
                            if (s.IsCode(CardId.PacifisThePhantasmCity))
                            {
                                AI.SelectYesNo(true);
                                AI.SelectCard(s);
                                break;
                            }
                        }
                    }

                }
                else
                    AI.SelectYesNo(false);
                return UniqueFaceupSpell();
            }
            else if(Card.IsFaceup())
            {
                ClientCard target = null;
                foreach(ClientCard s in Bot.GetSpells())
                {
                    if (s.IsCode(CardId.PacifisThePhantasmCity))
                        target = s;
                }
                if (target != null && Util.IsChainTarget(target))
                {
                    SeaStealthAttackeff_used = true;
                    return true;
                }                
                target = Util.GetLastChainCard();
                if(target!=null)
                {
                    if(target.IsCode(CardId.BrandishSkillAfterburner))
                    {
                        AI.SelectCard(CardId.MegalosmasherX);
                        SeaStealthAttackeff_used = true;
                        return true;
                    }
                    if(Enemy.GetGraveyardSpells().Count>=3 && target.IsCode(CardId.BrandishSkillJammingWave))
                    {
                        AI.SelectCard(CardId.MegalosmasherX);
                        SeaStealthAttackeff_used = true;
                        return true;
                    }
                }
            }
            return false;
        }   

        private bool PotOfDesireseff()
        {
            return Bot.Deck.Count >= 18;
        }       
       
        private bool StarlightRoadset()
        {
            if (Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster())
                return false;
            if (Bot.HasInSpellZone(CardId.TheHugeRevolutionIsOver)) return false;
            return true;
        }

        private bool TheHugeRevolutionIsOverset()
        {
            if (Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster())
                return false;
            if (Bot.HasInSpellZone(CardId.StarlightRoad)) return false;
            return true;
        }

        private bool InfiniteImpermanenceset()
        {
            return !Bot.IsFieldEmpty();
        }
        
        private bool NoSetAlreadyDone()
        {
            if (Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster())
                return false;
            if (Bot.HasInSpellZone(Card.Id)) return false;
            return true;
        }

        private bool SpellSeteff()
        { 
            if (Card.HasType(CardType.Field)) return false;
            if (CardOfDemiseeff_used) return true;
            if(Bot.HasInHandOrInSpellZone(CardId.CardOfDemise) && !CardOfDemiseeff_used)
            {
                int hand_spell_count = 0;
                foreach(ClientCard s in Bot.Hand)
                {
                    if (s.HasType(CardType.Trap) || s.HasType(CardType.Spell) && !s.HasType(CardType.Field))
                        hand_spell_count++;
                }
                int zone_count = 5 - Bot.GetSpellCountWithoutField();
                return zone_count- hand_spell_count >= 1;
            }
            if(Card.IsCode(CardId.PhantasmSprialBattle, CardId.PhantasmSpiralPower))
            {
                if (Bot.HasInMonstersZone(CardId.MegalosmasherX) &&
                    !Bot.HasInHandOrInSpellZone(CardId.PacifisThePhantasmCity) &&
                    !Bot.HasInHandOrInSpellZone(CardId.Metaverse))
                    return true;
            }
            return false;
        }

        private bool MonsterRepos()
        {
            if (Card.Level >= 5)
            {
                foreach (ClientCard s in Bot.GetSpells())
                {
                    if (s.IsFaceup() && s.IsCode(CardId.SeaStealthAttack) &&
                        Bot.HasInSpellZone(CardId.PacifisThePhantasmCity) &&
                        Card.IsAttack())
                        return false;
                }
            }
            if (Card.IsCode(CardId.EaterOfMillions) && !Card.IsDisabled() && Card.IsAttack())
                return false;
            return DefaultMonsterRepos();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if(attacker.IsCode(CardId.PacifisThePhantasmCity+1) && defender.IsCode(CardId.EaterOfMillions))
            {
                if (attacker.RealPower >= defender.RealPower) return true;
            }
            if(attacker.Level>=5)
            {
                foreach(ClientCard s in Bot.GetSpells())
                {
                    if (s.IsFaceup() && s.IsCode(CardId.SeaStealthAttack) && Bot.HasInSpellZone(CardId.PacifisThePhantasmCity))
                    { 
                        attacker.RealPower = 9999;
                        if (defender.IsCode(CardId.EaterOfMillions)) return true;
                    }
                       
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.IsCode(CardId.EaterOfMillions)) return attacker;
            }
            return null;
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        
    }
}
