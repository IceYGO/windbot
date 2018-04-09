﻿using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ChainBurn", "AI_ChainBurn", "Normal")]
    public class ChainBurnExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int SandaionTheTimloard = 100227025;
            public const int Mathematician = 41386308;
            public const int DiceJar = 3549275;
            public const int CardcarD = 45812361;
            public const int BattleFader = 19665973;
            public const int AbouluteKingBackJack = 60990740;

            public const int PotOfDesires = 35261759;
            public const int CardOfDemise = 59750328;
            public const int PotOfDuality = 98645731;
            public const int ChainStrike = 91623717;

            public const int Waboku = 12607053;
            public const int SecretBlast = 18252559;
            public const int JustDesserts = 24068492;
            public const int SectetBarrel = 27053506;
            public const int OjamaTrio = 29843091;
            public const int ThreateningRoar = 36361633;
            public const int Ceasefire = 36468556;
            public const int RecklessGreed = 37576645;
            public const int MagicCylinder = 62279055;
            public const int BalanceOfJudgment = 67443336;
            public const int BlazingMirrorForce = 75249652;
            public const int RingOfDestruction = 83555666;
            public const int AccuulatedFortune = 98444741;

            public const int Linkuriboh = 41999284;
            public const int HarpiesFeatherDuster = 18144506;
        }

        public ChainBurnExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //first add
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityeff);
            //normal summon
            AddExecutor(ExecutorType.Summon, CardId.Mathematician);
            AddExecutor(ExecutorType.Activate, CardId.Mathematician, Mathematicianeff);
            AddExecutor(ExecutorType.MonsterSet, CardId.DiceJar);
            AddExecutor(ExecutorType.Activate, CardId.DiceJar);
            AddExecutor(ExecutorType.Summon, CardId.AbouluteKingBackJack, AbouluteKingBackJacksummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.AbouluteKingBackJack);
            AddExecutor(ExecutorType.Activate, CardId.AbouluteKingBackJack, AbouluteKingBackJackeff);
            AddExecutor(ExecutorType.Summon, CardId.CardcarD);
            AddExecutor(ExecutorType.Summon, CardId.SandaionTheTimloard, SandaionTheTimloard_summon);
            AddExecutor(ExecutorType.Activate, CardId.SandaionTheTimloard, SandaionTheTimloardeff);
            // Set traps
            AddExecutor(ExecutorType.SpellSet, CardId.Waboku);
            AddExecutor(ExecutorType.SpellSet, CardId.ThreateningRoar);
            AddExecutor(ExecutorType.SpellSet, CardId.BlazingMirrorForce);
            AddExecutor(ExecutorType.SpellSet, CardId.OjamaTrio, OjamaTrioset);
            AddExecutor(ExecutorType.SpellSet, BrunSpellSet);
            //afer set
            AddExecutor(ExecutorType.Activate, CardId.CardcarD);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise, CardOfDemiseeff);
            //activate trap
            AddExecutor(ExecutorType.Activate, CardId.BalanceOfJudgment, BalanceOfJudgmenteff);
            AddExecutor(ExecutorType.Activate, CardId.AccuulatedFortune);
            AddExecutor(ExecutorType.Activate, CardId.ChainStrike, ChainStrikeeff);
            //battle 

            AddExecutor(ExecutorType.Activate, CardId.ThreateningRoar, ThreateningRoareff);
            AddExecutor(ExecutorType.Activate, CardId.Waboku, Wabokueff);
            AddExecutor(ExecutorType.Activate, CardId.BattleFader, BattleFadereff);
            AddExecutor(ExecutorType.Activate, CardId.MagicCylinder, MagicCylindereff);
            AddExecutor(ExecutorType.Activate, CardId.BlazingMirrorForce, BlazingMirrorForceeff);
            AddExecutor(ExecutorType.Activate, CardId.RingOfDestruction, Ring_act);
            //chain
            AddExecutor(ExecutorType.Activate, CardId.JustDesserts, JustDessertseff);
            AddExecutor(ExecutorType.Activate, CardId.OjamaTrio, OjamaTrioeff);
            AddExecutor(ExecutorType.Activate, CardId.Ceasefire, Ceasefireeff);
            AddExecutor(ExecutorType.Activate, CardId.SecretBlast, SecretBlasteff);
            AddExecutor(ExecutorType.Activate, CardId.SectetBarrel, SectetBarreleff);
            AddExecutor(ExecutorType.Activate, CardId.RecklessGreed, RecklessGreedeff);

            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);

        }
        public int[] all_List()
        {
            return new[]
            {
                CardId.SandaionTheTimloard,
                CardId.Mathematician,
                CardId.DiceJar,
                CardId.CardcarD,
                CardId.BattleFader,
                CardId.AbouluteKingBackJack,

                CardId.PotOfDesires,
                CardId.CardOfDemise,
                CardId.PotOfDuality,
                CardId.ChainStrike,

                CardId.Waboku,
                CardId.SecretBlast,
                CardId.JustDesserts,
                CardId.OjamaTrio,
                CardId.SectetBarrel,
                CardId.ThreateningRoar,
                CardId.Ceasefire,
                CardId.RecklessGreed,
                CardId.MagicCylinder,
                CardId.BalanceOfJudgment,
                CardId.BlazingMirrorForce,
                CardId.RingOfDestruction,
                CardId.AccuulatedFortune,
    };
        }
        public int[] AbouluteKingBackJack_List_1()
        {
            return new[] {
            CardId.BlazingMirrorForce,
            CardId.Waboku,
            CardId.ThreateningRoar,
            CardId.MagicCylinder,
            CardId.RingOfDestruction,            
            CardId.RecklessGreed,
            CardId.SecretBlast,
            CardId.JustDesserts,
            CardId.OjamaTrio,                
            CardId.SectetBarrel,                
            CardId.Ceasefire,            
            CardId.BalanceOfJudgment,                      
            CardId.AccuulatedFortune,   
        };
    }
        public int[] AbouluteKingBackJack_List_2()
        {
            return new[] {
            CardId.SandaionTheTimloard,
            CardId.PotOfDesires,
            CardId.Mathematician,
            CardId.DiceJar,
            CardId.CardcarD,
            CardId.BattleFader,        
            CardId.BlazingMirrorForce,
            CardId.Waboku,
            CardId.ThreateningRoar,
            CardId.MagicCylinder,
            CardId.RingOfDestruction,
            CardId.RecklessGreed,
            CardId.SecretBlast,
            CardId.JustDesserts,
            CardId.OjamaTrio,
            CardId.SectetBarrel,
            CardId.Ceasefire,
            CardId.BalanceOfJudgment,
            CardId.AccuulatedFortune,
        };
        }
        public int[] now_List()
        {
            return new[]
            {
                
                CardId.Waboku,
                CardId.SecretBlast,
                CardId.JustDesserts,
                CardId.SectetBarrel,
                CardId.ThreateningRoar,
                CardId.Ceasefire,
                CardId.RecklessGreed,
                CardId.RingOfDestruction,
              

    };
        }
        public int[] prevent_list()
        {
            return new[]
            {
                CardId.SandaionTheTimloard,
                CardId.BattleFader,

                CardId.Waboku,
                CardId.ThreateningRoar,
                CardId.MagicCylinder,
                CardId.BlazingMirrorForce,
                CardId.RingOfDestruction,
    };
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
        public bool Has_prevent_list(int id)
        {
            return (id == CardId.SandaionTheTimloard ||
                    id == CardId.BattleFader ||
                    id == CardId.Waboku ||
                    id == CardId.ThreateningRoar||
                    id == CardId.MagicCylinder||
                    id == CardId.BlazingMirrorForce||
                    id == CardId.RingOfDestruction
                   );
        }
        bool pot_used = false;
        bool one_turn_kill = false;
        int expected_blood = 0;
        bool prevent_used = false; 
        int preventcount = 0;        
        bool battleprevent = false;
        bool OjamaTrioused = false;
        bool HasAccuulatedFortune = false;
        int blast_count = 0;
        int barrel_count = 0;
        int just_count = 0;
        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            
            
            pot_used = false;
            prevent_used = false;
            battleprevent = false;
            OjamaTrioused = false;
          
        }
        public override void OnNewPhase()
        {
            preventcount = 0;
            battleprevent = false;
            HasAccuulatedFortune = false;
            IList<ClientCard> trap = Bot.SpellZone;
            IList<ClientCard> monster = Bot.MonsterZone;
            foreach(ClientCard card in trap)
            {
                if (card.Id == CardId.AccuulatedFortune) HasAccuulatedFortune = true;
            }
            foreach (ClientCard card in trap)
            {
                if (Has_prevent_list(card.Id))
                    {
                    preventcount++;
                    battleprevent = true;
                }

            }
            foreach (ClientCard card in monster)
            {
                if (Has_prevent_list(card.Id))
                {
                    preventcount++;
                    battleprevent = true;
                }

            }
            expected_blood = 0;
            one_turn_kill = false;
            blast_count = 0;
            barrel_count = 0;
            just_count = 0;
            IList<ClientCard> check = Bot.SpellZone;
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.SecretBlast)
                    blast_count++;
                break;
            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.SectetBarrel)
                    barrel_count++;
                break;
            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.JustDesserts)
                    just_count++;
                break;
            }
            expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count);
            if (Enemy.LifePoints <= expected_blood)
                one_turn_kill = true;
        }
        
        
        private bool must_chain()
        {
            if (AI.Utils.IsChainTarget(Card)) return true;
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card.Id == CardId.HarpiesFeatherDuster&&card.IsFaceup())
                {
                    return true;
                }
            }
            return false;
        }
        private bool BrunSpellSet()
        {
            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay)) && Bot.GetSpellCountWithoutField() < 5;
        }
        private bool SandaionTheTimloard_summon()
        {
            if (!battleprevent)
                return true;
            return false;
        }
        private bool AbouluteKingBackJacksummon()
        {
            return !pot_used;
        }
        private bool AbouluteKingBackJackeff()
        {
            if (ActivateDescription == -1)
            {

                AI.SelectCard(AbouluteKingBackJack_List_1());
                AI.SelectNextCard(AbouluteKingBackJack_List_2());
            }
                
            return true;
            
        }
        private bool PotOfDualityeff()
        {
            pot_used = true;
            AI.SelectCard(prevent_list());
            return true;
        }
        private bool BlazingMirrorForceeff()
        {
            if (prevent_used) return false;
            IList<ClientCard> list = new List<ClientCard>();
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                list.Add(monster);
            }
            //if (GetTotalATK(list) / 2 >= Bot.LifePoints) return false;
            if (GetTotalATK(list) < 3000) return false;           
            return Enemy.HasAttackingMonster() && DefaultUniqueTrap();
        }
        private bool ThreateningRoareff()
        {
            
            if (must_chain()) return true;
            if (prevent_used||Duel.Phase!=DuelPhase.Main1) return false;
            prevent_used = true;
            return DefaultUniqueTrap();
        }
        private bool SandaionTheTimloardeff()
        {
            prevent_used = true;
            return true;
        }
        private bool Wabokueff()
        {
            if (must_chain()) return true;
            if (prevent_used||Duel.Player == 0||Duel.Phase!=DuelPhase.BattleStart) return false;
            prevent_used = true;
            return DefaultUniqueTrap();
        }
        private bool BattleFadereff()
        {
            if (prevent_used || Duel.Player == 0) return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            prevent_used = true;
            return true;
        }
        private bool MagicCylindereff()
        {
            if (prevent_used) return false;
            if (Bot.LifePoints <= Enemy.BattlingMonster.Attack) return DefaultUniqueTrap();
            if(Enemy.BattlingMonster.Attack>1800) return DefaultUniqueTrap();
            return false;
        }
        public bool Ring_act()
        {
            if (Duel.LastChainPlayer == 0 && AI.Utils.GetLastChainCard() != null ) return false;
            ClientCard target = AI.Utils.GetProblematicEnemyMonster();
            if (target == null && AI.Utils.IsChainTarget(Card))
            {
                target = AI.Utils.GetBestEnemyMonster();
            }
            if (target != null)
            {
                if (Bot.LifePoints <= target.Attack) return false;
                AI.SelectCard(target);
                return true;
            }
            return false;
        }
        private bool RecklessGreedeff()
        {
            int count = 0;            
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.Id == CardId.RecklessGreed)
                    count++;
                break;
            }
            if (count > 1) return true;
            if (Bot.LifePoints <= 2000) return true;
            if (Bot.GetHandCount() <1 && Duel.Player==0 && Duel.Phase!=DuelPhase.Standby) return true;
            return false;
        }
        private bool SectetBarreleff()
        {
            if (one_turn_kill) return true;
            if (must_chain()) return true;
            int count = Enemy.GetFieldHandCount();
            if (Enemy.LifePoints < count * 200) return true;
            if (count >= 8) return true;
            return false;
        }
        private bool SecretBlasteff()
        {
            if (one_turn_kill) return true;
            if (must_chain()) return true;
            int count = Enemy.GetFieldCount();
            if (Enemy.LifePoints < count * 300) return true;
            if (count >= 5) return true;
            return false;
            
        }
        private bool OjamaTrioset()
        {
            if (Bot.HasInSpellZone(CardId.OjamaTrio)) return false;
            return true;
        }
        private bool OjamaTrioeff()
        {
            return OjamaTrioused;
        }
        private bool JustDessertseff()
        {
            if (one_turn_kill) return true;
            if (must_chain()) return true;
            int count = Enemy.GetMonsterCount();
            if (Enemy.LifePoints <= count * 500) return true;
            if (Bot.HasInSpellZone(CardId.OjamaTrio) && count <= 2 && count >= 1)
            {
                OjamaTrioused = true;
                return true;
            }
            if (count >= 4) return true;
            return false;
        }
        private bool ChainStrikeeff()
        {
            if (must_chain()) return true;
            int chain = Duel.CurrentChain.Count;
            if (Enemy.LifePoints <= (chain + 1) * 400) return true;
            if (Duel.CurrentChain.Count >= 3) return true;
            return false;
        }
        
        private bool BalanceOfJudgmenteff()
        {
            if (must_chain()) return true;
            int count = (Enemy.GetFieldCount() - Bot.GetFieldHandCount());
            if ( count>= 2)return true;
            return false;
        }
        private bool CardOfDemiseeff()
        {
            if (Bot.GetHandCount() == 1 && Bot.GetSpellCountWithoutField() <= 3)
                return true;
            return false;
        }
        private bool Mathematicianeff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(CardId.AbouluteKingBackJack);
                return true;
            }
            return true;
                
        }
        private bool DiceJarfacedown()
        {
            
            foreach (ClientCard card in Bot.GetMonsters())          
          
            {
                if (card.Id == CardId.DiceJar && card.IsFacedown())
                    return true;
                break;
            }
            return false;
        }
        private bool Ceasefireeff()
        {
            if (Enemy.GetMonsterCount() >= 3) return true;
            if (DiceJarfacedown()) return false;
            if ((Bot.GetMonsterCount() + Enemy.GetMonsterCount()) >= 4) return true;
            return false;
        }
        private bool Linkuriboheff()
        {
            ClientCard lastchaincard = AI.Utils.GetLastChainCard();
            if (lastchaincard == null) return true;
            if (lastchaincard.Id == CardId.Linkuriboh) return false;
            return true;
        }
        /*private bool SwordsOfRevealingLight()
        {
            int count = Bot.SpellZone.GetCardCount(CardId.SwordsOfRevealingLight);
            return count == 0;
        }*/

        /*
         private bool SetInvincibleMonster()
         {
             foreach (ClientCard card in Bot.GetMonsters())
             {
                 if (card.Id == CardId.Marshmallon || card.Id == CardId.SpiritReaper)
                 {
                     return false;
                 }
             }
             return true;
         }*/


        /*
        private bool ReposEverything()
        {
            if (Card.Id == CardId.ReflectBounder)
                return Card.IsDefense();
            if (Card.Id == CardId.FencingFireFerret)
                return DefaultMonsterRepos();
            if (Card.IsAttack())
                return true;
            return false;
        }*/
    }
}