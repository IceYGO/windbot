using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("GrenMajuThunderBoarder", "AI_GrenMajuThunderBoarder", "Normal")]
    public class GrenMajuThunderBoarderExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int InspectBoarder = 15397015;
            public const int ThunderKingRaiOh = 71564252;
            public const int GhostReaperAndWinterCherries = 62015408;
            public const int GrenMajuDaEizo = 36584821;
            public const int MaxxC = 23434538;
            public const int EaterOfMillions = 63845230;

            public const int HarpieFeatherDuster = 18144506;
            public const int PotOfDesires = 35261759;
            public const int CardOfDemise = 59750328;
            public const int UpstartGoblin = 70368879;
            public const int PotOfDuality = 98645731;
            public const int Scapegoat = 73915051;
            public const int MoonMirrorShield = 19508728;
            public const int InfiniteImpermanence = 10045474;
            public const int WakingTheDragon = 10813327;
            public const int EvenlyMatched = 15693423;
            public const int HeavyStormDuster = 23924608;
            public const int MacroCosmos = 30241314;
            public const int AntiSpellFragrance = 58921041;
            public const int ImperialOrder = 61740673;
            public const int UnendingNightmare= 69452756;
            public const int SolemnWarning = 84749824;
            public const int SolemStrike= 40605147;
            public const int SolemnJudgment = 41420027;
            public const int DarkBribe = 77538567;

            public const int RaidraptorUltimateFalcon = 86221741;
            public const int BorreloadDragon = 31833038;
            public const int BirrelswordDragon = 85289965;
            public const int FirewallDragon = 5043010;
            public const int BingirsuTheWorldChaliceWarrior = 30194529;
            public const int TopologicTrisbaena = 72529749;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int HeavymetalfoesElectrumite= 24094258;
            public const int KnightmareCerberus = 75452921;
            public const int CrystronNeedlefiber = 50588353;
            public const int MissusRadiant= 3987233;
            public const int BrandishMaidenKagari= 63288573;
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;
        }

        public GrenMajuThunderBoarderExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {            
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched, EvenlyMatchedeff);
            //Sticker
            AddExecutor(ExecutorType.Activate, CardId.MacroCosmos, MacroCosmoseff);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance, AntiSpellFragranceeff);
            //counter
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCeff);            
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, InfiniteImpermanenceeff);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.HeavyStormDuster, HeavyStormDustereff);
            AddExecutor(ExecutorType.Activate, CardId.UnendingNightmare, UnendingNightmareeff);
            AddExecutor(ExecutorType.Activate, CardId.DarkBribe);
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialOrdereff);
            AddExecutor(ExecutorType.Activate, CardId.ThunderKingRaiOh, ThunderKingRaiOheff);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);
            //first do
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin, UpstartGoblineff);
            AddExecutor(ExecutorType.Activate, CardId.HarpieFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityeff);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, PotOfDesireseff);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseeff);   
            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadDragon, BorreloadDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadDragon, BorreloadDragoneff);            
            AddExecutor(ExecutorType.Activate, CardId.EaterOfMillions, EaterOfMillionseff);
            AddExecutor(ExecutorType.Activate, CardId.WakingTheDragon, WakingTheDragoneff);
            AddExecutor(ExecutorType.SpSummon, CardId.MissusRadiant, MissusRadiantsp);
            AddExecutor(ExecutorType.Activate, CardId.MissusRadiant, MissusRadianteff);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            // normal summon
            AddExecutor(ExecutorType.Summon, CardId.InspectBoarder, InspectBoardersummon);
            AddExecutor(ExecutorType.Summon, CardId.GrenMajuDaEizo, GrenMajuDaEizosummon);
            AddExecutor(ExecutorType.Summon, CardId.ThunderKingRaiOh, ThunderKingRaiOhsummon);
            AddExecutor(ExecutorType.SpSummon, CardId.EaterOfMillions, EaterOfMillionssp);
            //spell
            AddExecutor(ExecutorType.Activate, CardId.MoonMirrorShield, MoonMirrorShieldeff);
            AddExecutor(ExecutorType.Activate, CardId.Scapegoat, Scapegoateff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            //set
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }
        bool CardOfDemiseeff_used = false;
        bool plan_A = false;
        bool eater_eff = false;
        public override void OnNewTurn()
        {
            eater_eff = false;
            CardOfDemiseeff_used = false;            
            if (Bot.HasInHand(CardId.EvenlyMatched) && Duel.Turn == 2 && Enemy.GetFieldCount()>=2)
            {
                Logger.DebugWriteLine("***********plan_A");
                plan_A = true;
                //todo:Duel.Global.ToBattlePhase = true;
            }

        }

        public override void OnNewPhase()
        {
            foreach (ClientCard check in Bot.GetMonsters())
            {
                if (check.HasType(CardType.Fusion) || check.HasType(CardType.Xyz) ||
                    check.HasType(CardType.Synchro) || check.HasType(CardType.Link) ||
                    check.HasType(CardType.Ritual))
                {
                    eater_eff = true;
                    break;
                }
            }
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.HasType(CardType.Fusion) || check.HasType(CardType.Xyz) ||
                    check.HasType(CardType.Synchro) || check.HasType(CardType.Link) ||
                    check.HasType(CardType.Ritual))
                {
                    eater_eff = true;
                    break;
                }
            }
            base.OnNewPhase();
        }

        private bool SpellWillBeNegated()
        {
            ClientCard card = null;
            foreach (ClientCard check in Bot.GetSpells())
            {
                if(check.Id==CardId.ImperialOrder && !check.IsDisabled())
                    card = check;
            }
                
            if (card!=null && card.IsFaceup())
                return true;
            if (Enemy.HasInSpellZone(CardId.ImperialOrder, true))
                return true;
            return false;
        }  

        private bool MacroCosmoseff()
        {
           
            return (Duel.LastChainPlayer == 1 || Duel.LastSummonPlayer == 1 || Duel.Player == 0) && UniqueFaceupSpell();
        }

        private bool AntiSpellFragranceeff()
        {
           
            int spell_count = 0;
            foreach(ClientCard check in Bot.Hand)
            {
                if (check.HasType(CardType.Spell))
                    spell_count++;
            }
            if (spell_count >= 2) return false;
            return Duel.Player == 1 && UniqueFaceupSpell();
        }

        private bool MaxxCeff()
        {           
            return Duel.Player == 1;
        }

        private bool EvenlyMatchedeff()
        {
            // todo:Duel.Global.ToBattlePhase = false;
            plan_A = false;
            return true;
        }
        private bool InfiniteImpermanenceeff()
        {
            if (plan_A) return false;
            AI.SelectPlace(Zones.z2);
            ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
            if(target!=null)
            {
                AI.SelectCard(target);
                return true;
            }
            if(Duel.LastChainPlayer==1)
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if(AI.Utils.GetLastChainCard()==check)
                    {
                        target = check;
                        break;
                    }
                }
                if(target!=null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        private bool HeavyStormDustereff()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard check in Enemy.GetSpells())
            {
                if (check.HasType(CardType.Continuous) || check.HasType(CardType.Field))
                    targets.Add(check);
            }
            foreach (ClientCard check in Enemy.GetSpells())
            {
                if (!check.HasType(CardType.Continuous) && !check.HasType(CardType.Field))
                    targets.Add(check);
            }
            if(DefaultOnBecomeTarget())
            {
                AI.SelectCard(targets);
                return true;
            }
            if(AI.Utils.GetLastChainCard()!=null && 
                (AI.Utils.GetLastChainCard().HasType(CardType.Continuous)||
                AI.Utils.GetLastChainCard().HasType(CardType.Field)) &&
                Duel.LastChainPlayer==1)               
                {
                AI.SelectCard(targets);
                return true;
                }
            return false;
        }
        private bool UnendingNightmareeff()
        {
          
            ClientCard card = null;
            foreach(ClientCard check in Enemy.GetSpells())
            {
                if (check.HasType(CardType.Continuous) || check.HasType(CardType.Field))                   
                    card = check;
                break;
            }
            if(card!=null && Bot.LifePoints>1000)
            {
                AI.SelectCard(card);
                return true;
            }
            return false;
        }

        private bool ImperialOrdereff()
        {
           
            if (Duel.LastChainPlayer == 1)
            {
                foreach(ClientCard check in Enemy.SpellZone)
                {
                    if (AI.Utils.GetLastChainCard() == check)
                        return true;
                }
            }
            return false;
        }

        private bool UpstartGoblineff()
        {            
            return !SpellWillBeNegated();
        }

        private bool PotOfDualityeff()
        {
            if (SpellWillBeNegated())
                return false;
            int count = 0;
            if (Bot.GetMonsterCount() > 0)
                count = 1;
            foreach(ClientCard card in Bot.Hand)
            {
                if (card.HasType(CardType.Monster))
                    count++;
            }
            if (count == 0)
                AI.SelectCard(new[]
                {
                    CardId.PotOfDesires,
                    CardId.InspectBoarder,
                    CardId.ThunderKingRaiOh,
                    CardId.EaterOfMillions,
                    CardId.GrenMajuDaEizo,
                });
            else
            {
                AI.SelectCard(new[]
                {
                    CardId.PotOfDesires,
                    CardId.CardOfDemise,
                    CardId.SolemnJudgment,
                    CardId.SolemnWarning,
                    CardId.SolemStrike,
                    CardId.InfiniteImpermanence,
                });
            }
            return true;
        }

        private bool PotOfDesireseff()
        {
            return Bot.Deck.Count > 14 && !SpellWillBeNegated();
        }

        private bool CardOfDemiseeff()
        {
            if(Bot.Hand.Count == 1 && Bot.GetSpellCountWithoutField() <= 3 && !SpellWillBeNegated())
            {
                CardOfDemiseeff_used = true;
                return true;
            }
            return false;
        }

        private bool MoonMirrorShieldeff()
        {
            if(Card.Location==CardLocation.Hand)
            {
                if (Bot.GetMonsterCount() == 0) return false;
                return !SpellWillBeNegated();
            }
            if(Card.Location==CardLocation.Grave)
            {
                return true;
            }
            return false;
        }

        private bool Scapegoateff()
        {
            if (SpellWillBeNegated()) return false;
            if (Duel.Player == 0) return false;
            if (Duel.Phase == DuelPhase.End) return true;
            if (Duel.LastChainPlayer == 1 && (AI.Utils.IsChainTarget(Card) || (DefaultOnBecomeTarget() && !Bot.HasInSpellZone(CardId.WakingTheDragon)))) return true;
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int total_atk = 0;
                List<ClientCard> enemy_monster = Enemy.GetMonsters();
                foreach (ClientCard m in enemy_monster)
                {
                    if (m.IsAttack() && !m.Attacked) total_atk += m.Attack;
                }
                if (total_atk >= Bot.LifePoints) return true;
            }
            return false;        
        }

        private bool InspectBoardersummon()
        {
            if (plan_A) return false;
            AI.SelectPlace(Zones.z4 | Zones.z0);
            return true;
        }
        private bool GrenMajuDaEizosummon()
        {
            if (Duel.Turn == 1) return false;
            if (plan_A) return false;
            AI.SelectPlace(Zones.z4 | Zones.z0);
            return Bot.Banished.Count >= 6;
        }

        private bool ThunderKingRaiOhsummon()
        {
            if (plan_A) return false;
            AI.SelectPlace(Zones.z4 | Zones.z0);
            return true;
        }

        private bool ThunderKingRaiOheff()
        {
            foreach (ClientCard card in Duel.SummoningCards)
            {
                if (card.Attack >= 1900)
                    return true;
            }
            return false;
        }

        private bool BorreloadDragonsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.Id != CardId.EaterOfMillions)
                    material_list.Add(monster);
                if (material_list.Count == 3) break;
            }
            if(material_list.Count>=3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        public bool BorreloadDragoneff()
        {
            if (ActivateDescription == -1)
            {
                ClientCard enemy_monster = Enemy.BattlingMonster;
                if (enemy_monster != null && enemy_monster.HasPosition(CardPosition.Attack))
                {
                    return (Card.Attack - enemy_monster.Attack < Enemy.LifePoints);
                }
                return true;
            };
            ClientCard BestEnemy = AI.Utils.GetBestEnemyMonster(true);
            ClientCard WorstBot = Bot.GetMonsters().GetLowestAttackMonster();
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            if (WorstBot == null || WorstBot.HasPosition(CardPosition.FaceDown)) return false;
            if (BestEnemy.Attack >= WorstBot.RealPower)
            {
                AI.SelectCard(BestEnemy);
                return true;
            }
            return false;
        }

        private bool EaterOfMillionssp()
        {
            if (Bot.HasInMonstersZone(CardId.InspectBoarder) && !eater_eff) return false;
            if (plan_A) return false;
            if (AI.Utils.GetProblematicEnemyMonster() == null && Bot.ExtraDeck.Count < 5) return false;
            if (Bot.GetMonstersInMainZone().Count >= 5) return false;
            if (AI.Utils.IsTurn1OrMain2()) return false;
            AI.SelectPosition(CardPosition.FaceUpAttack);
            IList<ClientCard> targets = new List<ClientCard>();            
            foreach (ClientCard e_c in Bot.ExtraDeck)
            {
                targets.Add(e_c);
                if (targets.Count >= 5)
                {
                    AI.SelectCard(targets);
                    /*AI.SelectCard(new[] {
                        CardId.BingirsuTheWorldChaliceWarrior,
                        CardId.TopologicTrisbaena,
                        CardId.KnightmareCerberus,
                        CardId.KnightmarePhoenix,
                        CardId.KnightmareUnicorn,
                        CardId.BrandishMaidenKagari,
                        CardId.HeavymetalfoesElectrumite,
                        CardId.CrystronNeedlefiber,
                        CardId.FirewallDragon,
                        CardId.BirrelswordDragon,
                        CardId.RaidraptorUltimateFalcon,
                    });*/
                    AI.SelectPlace(Zones.z4 | Zones.z0);
                    return true;
                }
            }
            Logger.DebugWriteLine("*** Eater use up the extra deck.");
            foreach (ClientCard s_c in Bot.GetSpells())
            {
                targets.Add(s_c);
                if (targets.Count >= 5)
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            return false;
        }

        private bool EaterOfMillionseff()
        {
            //if (Enemy.BattlingMonster.HasPosition(CardPosition.Attack) && (Bot.BattlingMonster.Attack - Enemy.BattlingMonster.GetDefensePower() >= Enemy.LifePoints)) return false;
            return true;
        }

        private bool WakingTheDragoneff()
        {
            AI.SelectCard(new[] { CardId.RaidraptorUltimateFalcon });
            return true;
        }

        private bool MissusRadiantsp()
        {                       
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Earth) && monster.Level==1 && monster.Id!=CardId.EaterOfMillions)
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.MissusRadiant)) return false;
            AI.SelectMaterials(material_list);
            AI.SelectPlace(Zones.z5 | Zones.z6);
            return true;
        }

        private bool MissusRadianteff()
        {
            AI.SelectCard(new[]
           {
                CardId.MaxxC,
                CardId.MissusRadiant,
            });
            return true;
        }

        private bool Linkuribohsp()
        {
            
            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (c.Id != CardId.EaterOfMillions && c.Level == 1 && c.Id != CardId.Linkuriboh && c.Id != CardId.LinkSpider)
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
            return true;
        }
        private bool MonsterRepos()
        {
            if (Card.Id == CardId.EaterOfMillions && Card.IsAttack()) return false;
            return DefaultMonsterRepos();
        }

        private bool SpellSet()
        {           
            if (plan_A) return false;
            int count = 0;
            foreach(ClientCard check in Bot.Hand)
            {
                if (check.Id == CardId.CardOfDemise)
                    count++;
            }
            if (count == 2 && Bot.Hand.Count == 2 && Bot.GetSpellCountWithoutField() <= 2)
                return true;            
            if (Card.Id == CardId.MacroCosmos && Bot.HasInSpellZone(CardId.MacroCosmos)) return false;
            if (Card.Id == CardId.AntiSpellFragrance && Bot.HasInSpellZone(CardId.AntiSpellFragrance)) return false;
            if (CardOfDemiseeff_used)return true;
            //if (Duel.Turn > 1 && Duel.Phase != DuelPhase.Main2) return false;
            if (Card.Id == CardId.EvenlyMatched && (Enemy.GetFieldCount() - Bot.GetFieldCount()) < 0) return false;
            if (Card.Id == CardId.AntiSpellFragrance && Bot.HasInSpellZone(CardId.AntiSpellFragrance)) return false;
            if (Card.Id == CardId.MacroCosmos && Bot.HasInSpellZone(CardId.MacroCosmos)) return false;
            if (Card.Id == CardId.InfiniteImpermanence)
                return Bot.GetFieldCount() > 0 && Bot.GetSpellCountWithoutField() < 4;
            if (Card.Id == CardId.Scapegoat)
                return true;
            if (Card.HasType(CardType.Trap))
                return Bot.GetSpellCountWithoutField() < 4;
            if(Bot.HasInSpellZone(CardId.AntiSpellFragrance,true))
            {
                if (Card.Id == CardId.UpstartGoblin || Card.Id == CardId.PotOfDesires || Card.Id==CardId.PotOfDuality) return true;
                if (Card.Id == CardId.CardOfDemise && Bot.HasInSpellZone(CardId.CardOfDemise)) return false;
                if (Card.HasType(CardType.Spell))
                    return Bot.GetSpellCountWithoutField() < 4;
            }
            return false;
        }
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {          
            
            if (attacker.Id==CardId.EaterOfMillions && (Bot.HasInMonstersZone(CardId.InspectBoarder) && eater_eff))
            {
                attacker.RealPower = 9999;
                return true;
            }
            if (attacker.Id == CardId.EaterOfMillions && !Bot.HasInMonstersZone(CardId.InspectBoarder))
            {
                attacker.RealPower = 9999;
                return true;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }
        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.Id == CardId.BirrelswordDragon || attacker.Id == CardId.EaterOfMillions) return attacker;
            }
            return null;
        }
        public override bool OnSelectHand()
        {
            return true;
        }        
    }
}