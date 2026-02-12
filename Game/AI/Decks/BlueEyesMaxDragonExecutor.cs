using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("BlueEyesMaxDragon", "AI_BlueEyesMaxDragon")]
    public class BlueEyesMaxDragonExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int BlueEyesWhiteDragon = 89631139;
            public const int BlueEyesAlternativeWhiteDragon = 38517737;
            public const int DeviritualTalismandra = 80701178;
            public const int ManguOfTheTenTousandHands = 95492061;
            public const int DevirrtualCandoll = 53303460;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;
            public const int BlueEyesChaosMaxDragon = 55410871;

            public const int CreatureSwap= 31036355;
            public const int TheMelodyOfAwakeningDragon = 48800175;
            public const int UpstartGoblin = 70368879;
            public const int ChaosForm = 21082832;
            public const int AdvancedRitualArt = 46052429;
            public const int CalledByTheGrave = 24224830;
            public const int Scapegoat = 73915051;
            public const int InfiniteImpermanence = 10045474;
            public const int RecklessGreed = 37576645;

            public const int BorreloadDragon = 31833038;
            public const int BirrelswordDragon = 85289965;
            public const int KnightmareGryphon = 65330383;
            public const int MissusRadiant = 3987233;            
            public const int LinkSpider = 98978921;
            public const int Linkuriboh = 41999284;

            public const int LockBird = 94145021;
            public const int Ghost = 59438930;


        }

        public BlueEyesMaxDragonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //counter
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCeff);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByTheGraveeff);
            //first
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);           
            AddExecutor(ExecutorType.Activate, CardId.BlueEyesAlternativeWhiteDragon, BlueEyesAlternativeWhiteDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.CreatureSwap, CreatureSwapeff);
            AddExecutor(ExecutorType.Activate, CardId.TheMelodyOfAwakeningDragon, TheMelodyOfAwakeningDragoneff);
            //summon
            AddExecutor(ExecutorType.Summon, CardId.ManguOfTheTenTousandHands);
            AddExecutor(ExecutorType.Activate, CardId.ManguOfTheTenTousandHands, TenTousandHandseff);
            AddExecutor(ExecutorType.Activate, DeviritualCheck);
            //ritual summon
            AddExecutor(ExecutorType.Activate, CardId.AdvancedRitualArt);
            AddExecutor(ExecutorType.Activate, CardId.ChaosForm, ChaosFormeff);
            //sp summon
            AddExecutor(ExecutorType.SpSummon, CardId.MissusRadiant, MissusRadiantsp);
            AddExecutor(ExecutorType.Activate, CardId.MissusRadiant, MissusRadianteff);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.SpSummon, CardId.BirrelswordDragon, BirrelswordDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BirrelswordDragon, BirrelswordDragoneff);
            //set
            AddExecutor(ExecutorType.Activate, CardId.TheMelodyOfAwakeningDragon, TheMelodyOfAwakeningDragoneffsecond);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            //
            AddExecutor(ExecutorType.Activate, CardId.RecklessGreed, RecklessGreedeff);
            
            AddExecutor(ExecutorType.Activate, CardId.Scapegoat, Scapegoateff);
        }
        int Talismandra_count = 0;
        int Candoll_count = 0;
        bool Talismandra_used = false;
        bool Candoll_used = false;
        int RitualArt_count = 0;
        int ChaosForm_count = 0;
        int MaxDragon_count = 0;
        int TheMelody_count = 0;
        public override void OnNewTurn()
        {
            Talismandra_used = false;
            Candoll_used = false;
            base.OnNewTurn();
        }
        private void Count_check()
        {
            TheMelody_count = 0;
            Talismandra_count = 0;
            Candoll_count = 0;
            RitualArt_count = 0;
            ChaosForm_count = 0;
            MaxDragon_count = 0;
            foreach (ClientCard check in Bot.Hand)
            {
                if (check.IsCode(CardId.AdvancedRitualArt))
                    RitualArt_count++;
                if (check.IsCode(CardId.ChaosForm))
                    ChaosForm_count++;
                if (check.IsCode(CardId.DevirrtualCandoll))
                    Candoll_count++;
                if (check.IsCode(CardId.DeviritualTalismandra))
                    Talismandra_count++;
                if (check.IsCode(CardId.BlueEyesChaosMaxDragon))
                    MaxDragon_count++;
                if (check.IsCode(CardId.TheMelodyOfAwakeningDragon))
                    TheMelody_count++;
            }
        }        

        private bool MaxxCeff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player == 1;
        }

        private bool CalledByTheGraveeff()
        {
            if(Duel.LastChainPlayer==1)
            {
                ClientCard lastCard = Util.GetLastChainCard();
                if (lastCard.IsCode(CardId.MaxxC))
                {
                    AI.SelectCard(CardId.MaxxC);
                    if(Util.ChainContainsCard(CardId.TheMelodyOfAwakeningDragon))
                        AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                    return UniqueFaceupSpell();
                }
                if (lastCard.IsCode(CardId.LockBird))
                {
                    AI.SelectCard(CardId.LockBird);
                    if (Util.ChainContainsCard(CardId.TheMelodyOfAwakeningDragon))
                        AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                    return UniqueFaceupSpell();
                }
                if (lastCard.IsCode(CardId.Ghost))
                {
                    AI.SelectCard(CardId.Ghost);
                    if (Util.ChainContainsCard(CardId.TheMelodyOfAwakeningDragon))
                        AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                    return UniqueFaceupSpell();
                }
                if (lastCard.IsCode(CardId.AshBlossom))
                {
                    AI.SelectCard(CardId.AshBlossom);
                    if (Util.ChainContainsCard(CardId.TheMelodyOfAwakeningDragon))
                        AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                    return UniqueFaceupSpell();
                }
            }
            return false;
        }
        private bool BlueEyesAlternativeWhiteDragoneff()
        {
            if(Card.Location==CardLocation.Hand)
            {
                if (Duel.Turn == 1)
                    return false;
                return true;
            }
            else
            {
                if(Util.GetProblematicEnemyMonster(3000,true)!=null)
                {
                    AI.SelectCard(Util.GetProblematicEnemyMonster(3000, true));
                    return true;
                }
            }
            return false;
        }

        private bool CreatureSwapeff()
        {
            if(Bot.HasInMonstersZone(CardId.BlueEyesChaosMaxDragon,true) && Duel.Phase==DuelPhase.Main1 &&
                (Bot.HasInMonstersZone(CardId.DeviritualTalismandra) || Bot.HasInMonstersZone(CardId.DevirrtualCandoll)))
            {
                AI.SelectCard(CardId.DevirrtualCandoll, CardId.DeviritualTalismandra);
                return true;
            }
            return false;
        }
        private bool TheMelodyOfAwakeningDragoneff()
        {
            Count_check();
            if(TheMelody_count>=2 && Bot.GetRemainingCount(CardId.BlueEyesChaosMaxDragon,3)>0)
            {
                AI.SelectCard(CardId.TheMelodyOfAwakeningDragon);
                AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                return true;
            }
            if(Bot.HasInHand(CardId.BlueEyesWhiteDragon) && Bot.GetRemainingCount(CardId.BlueEyesChaosMaxDragon, 3) > 0)
            {
                AI.SelectCard(CardId.BlueEyesWhiteDragon);
                AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                return true;
            }
            return false;
        }
        private bool TheMelodyOfAwakeningDragoneffsecond()
        {
            Count_check();
            if (RitualArtCanUse() && Bot.GetRemainingCount(CardId.BlueEyesChaosMaxDragon, 3) > 0 &&
                !Bot.HasInHand(CardId.BlueEyesChaosMaxDragon) && Bot.Hand.Count>=3)
            {
                if(RitualArt_count>=2)
                {
                    foreach (ClientCard m in Bot.Hand)
                    {
                        if (m.IsCode(CardId.AdvancedRitualArt))
                        AI.SelectCard(m);
                    }
                }
                foreach(ClientCard m in Bot.Hand)
                {
                    if (!m.IsCode(CardId.AdvancedRitualArt))
                    AI.SelectCard(m);
                }
                AI.SelectNextCard(CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesAlternativeWhiteDragon);
                return true;
            }            
            return false;
        }
        private bool TenTousandHandseff()
        {
            Count_check();
            if(Talismandra_count>=2 && Bot.GetRemainingCount(CardId.BlueEyesChaosMaxDragon,3)>0)
            {
                AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                return true;
            }
            if(Candoll_count>=2 || MaxDragon_count >= 2)
            {
                if(RitualArtCanUse() && Bot.GetRemainingCount(CardId.AdvancedRitualArt,3)>0)
                {
                    AI.SelectCard(CardId.AdvancedRitualArt);
                    return true;
                }
                if(ChaosFormCanUse() && Bot.GetRemainingCount(CardId.ChaosForm,1)>0)
                {
                    AI.SelectCard(CardId.ChaosForm);
                    return true;
                }
            }            
            if(RitualArt_count+ChaosForm_count>=2)
            {
                AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                return true;
            }
            if(Candoll_count+Talismandra_count>1)
            {
                if (MaxDragon_count >= 1)
                {
                    if (RitualArtCanUse() && Bot.GetRemainingCount(CardId.AdvancedRitualArt, 3) > 0)
                    {
                        AI.SelectCard(CardId.AdvancedRitualArt);
                        return true;
                    }
                    if (ChaosFormCanUse() && Bot.GetRemainingCount(CardId.ChaosForm, 1) > 0)
                    {
                        AI.SelectCard(CardId.ChaosForm);
                        return true;
                    }
                }
                if(Bot.HasInHand(CardId.AdvancedRitualArt) || Bot.HasInHand(CardId.ChaosForm))
                {
                    AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                    return true;
                }
            }
            if (ChaosForm_count >= 1)
            {
                if (RitualArtCanUse() && Bot.GetRemainingCount(CardId.AdvancedRitualArt, 3) > 0)
                {
                    AI.SelectCard(CardId.AdvancedRitualArt);
                    return true;
                }
                if (ChaosFormCanUse() && Bot.GetRemainingCount(CardId.ChaosForm, 1) > 0)
                {
                    AI.SelectCard(CardId.ChaosForm);
                    return true;
                }
            }
            if (Talismandra_count>=1)
            {
                AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                return true;
            }
            if(MaxDragon_count>=1)
            {
                if (RitualArtCanUse() && Bot.GetRemainingCount(CardId.AdvancedRitualArt, 3) > 0)
                {
                    AI.SelectCard(CardId.AdvancedRitualArt);
                    return true;
                }
                if (ChaosFormCanUse() && Bot.GetRemainingCount(CardId.ChaosForm, 1) > 0)
                {
                    AI.SelectCard(CardId.ChaosForm);
                    return true;
                }
            }            
            if (RitualArtCanUse() && Bot.GetRemainingCount(CardId.AdvancedRitualArt, 3) > 0)
            {
                AI.SelectCard(CardId.AdvancedRitualArt);                
            }
            if (ChaosFormCanUse() && Bot.GetRemainingCount(CardId.ChaosForm, 1) > 0)
            {
                AI.SelectCard(CardId.ChaosForm);                
            }
            return true;
        }

        private bool RitualArtCanUse()
        {
            return Bot.GetRemainingCount(CardId.BlueEyesWhiteDragon,2)>0;
        }

        private bool ChaosFormCanUse()
        {
            ClientCard check = null;
            foreach (ClientCard m in Bot.GetGraveyardMonsters())
            {
                if (m.IsCode(CardId.BlueEyesAlternativeWhiteDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesWhiteDragon))
                    check = m;
            }
            
            foreach (ClientCard m in Bot.Hand)
            {
                if (m.IsCode(CardId.BlueEyesWhiteDragon))
                    check = m;
            }
            if (check != null)
            {
                
                return true;
            }
            return false;
        }

        private bool DeviritualCheck()
        {
            Count_check();
            if(Card.IsCode(CardId.DeviritualTalismandra, CardId.DevirrtualCandoll))
            {
                if (Card.Location == CardLocation.MonsterZone)
                {
                    if(RitualArtCanUse())
                    {
                        AI.SelectCard(CardId.AdvancedRitualArt);                        
                    }
                    else
                    {
                        AI.SelectCard(CardId.ChaosForm);
                    }
                    return true;
                }
                if(Card.Location==CardLocation.Hand)
                {                    
                    if(Card.IsCode(CardId.DevirrtualCandoll))
                    {
                        if (MaxDragon_count >= 2 && Talismandra_count >= 1 || Candoll_used)
                            return false;
                    }
                    if(Card.IsCode(CardId.DeviritualTalismandra))
                    {
                        if (RitualArt_count + ChaosForm_count >= 2 && Candoll_count >= 1 || Talismandra_used)
                            return false;
                        Talismandra_used = true;
                        return true;
                    }
                    if(RitualArtCanUse())
                    {
                        Candoll_used = true;
                        AI.SelectCard(CardId.AdvancedRitualArt);
                        return true;
                    }
                    if (ChaosFormCanUse())
                    {
                        Candoll_used = true;
                        AI.SelectCard(CardId.ChaosForm);
                        return true;
                    }
                    return true;
                }
            }
            return false;

        }
        private bool ChaosFormeff()
        {
            ClientCard check = null;
            foreach(ClientCard m in Bot.Graveyard)
            {
                if (m.IsCode(CardId.BlueEyesAlternativeWhiteDragon, CardId.BlueEyesChaosMaxDragon, CardId.BlueEyesWhiteDragon))
                    check = m;
                
            }
            
            if(check != null)
            {
                AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                AI.SelectNextCard(check);
                return true;
            }
            foreach(ClientCard m in Bot.Hand)
            {
                if (m.IsCode(CardId.BlueEyesWhiteDragon))
                    check = m;
            }           
            if (check != null)
            {
                AI.SelectCard(CardId.BlueEyesChaosMaxDragon);
                AI.SelectNextCard(check);
                return true;
            }
            return false;
        }
        private bool MissusRadiantsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Earth) && monster.Level == 1)
                    material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Bot.HasInMonstersZone(CardId.MissusRadiant)) return false;
            AI.SelectMaterials(material_list);
            if (Bot.MonsterZone[0] == null && Bot.MonsterZone[2] == null && Bot.MonsterZone[5] == null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            return true;
        }

        private bool MissusRadianteff()
        {
            AI.SelectCard(CardId.MaxxC, CardId.MissusRadiant);
            return true;
        }

        private bool Linkuribohsp()
        {
            foreach (ClientCard c in Bot.GetMonsters())
            {
                if (!c.IsCode(CardId.Linkuriboh) && c.Level == 1)
                {
                    AI.SelectMaterials(c);
                    return true;
                }
            }
            return false;
        }

        private bool Linkuriboheff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Linkuriboh)) return false;
            return true;
        }
        private bool BirrelswordDragonsp()
        {
           
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
                if (m.IsCode(CardId.Linkuriboh) || m.Level==1)
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

        private bool BirrelswordDragoneff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BirrelswordDragon, 0))
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
                        if (check.IsAttack())
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
        private bool SpellSet()
        {
            if (Card.IsCode(CardId.InfiniteImpermanence))
                return !Bot.IsFieldEmpty();
            if (Card.IsCode(CardId.RecklessGreed))
                return true;
            if (Card.IsCode(CardId.Scapegoat))
                return true;
            return false;
        }

        private bool RecklessGreedeff()
        {           
            int count = 0;
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(CardId.RecklessGreed))
                    count++;
            }            
            if (DefaultOnBecomeTarget()) return true;            
            if(Duel.Player==0 && Duel.Phase>=DuelPhase.Main1)
            {
                if (Bot.LifePoints <= 4000 || count>=2)
                    return true;
            }
            return false;
        }

        private bool Scapegoateff()
        {            
            if (Duel.Player == 0) return false;
            if (Duel.Phase == DuelPhase.End) return true;
            if (Duel.LastChainPlayer == 1 && DefaultOnBecomeTarget()) return true;
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
        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {           
            for (int i = 0; i < attackers.Count; ++i)
            {                
                ClientCard attacker = attackers[i];
                if (attacker.IsCode(CardId.BlueEyesChaosMaxDragon))
                {
                    Logger.DebugWriteLine(attacker.Name);
                    return attacker;
                }               
            }
            return base.OnSelectAttacker(attackers, defenders);
        }
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if(attacker.IsCode(CardId.BlueEyesChaosMaxDragon) && !attacker.IsDisabled() &&
                Enemy.HasInMonstersZone(new[] {CardId.DeviritualTalismandra,CardId.DevirrtualCandoll }))
            {              
                for (int i = 0; i < defenders.Count; i++)
                {
                    ClientCard defender = defenders[i];                    
                    attacker.RealPower = attacker.Attack;
                    defender.RealPower = defender.GetDefensePower();
                    if (!OnPreBattleBetween(attacker, defender))
                        continue;                    
                    if (defender.IsCode(CardId.DevirrtualCandoll, CardId.DeviritualTalismandra))
                    {
                        return AI.Attack(attacker, defender);                                          
                    }                   
                }                
            }
            return base.OnSelectAttackTarget(attacker, defenders);
        }
        public override bool OnSelectHand()
        {
            return false;
        }

    }
}
