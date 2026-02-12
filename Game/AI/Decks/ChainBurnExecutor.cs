using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ChainBurn", "AI_ChainBurn")]
    public class ChainBurnExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int SandaionTheTimelord = 33015627;
            public const int MichionTimelord = 7733560;
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
            AddExecutor(ExecutorType.Summon, CardId.MichionTimelord, MichionTimelordsummon);
            AddExecutor(ExecutorType.Summon, CardId.SandaionTheTimelord, SandaionTheTimelord_summon);            
            AddExecutor(ExecutorType.Summon, CardId.Mathematician);
            AddExecutor(ExecutorType.Activate, CardId.Mathematician, Mathematicianeff);
            AddExecutor(ExecutorType.MonsterSet, CardId.DiceJar);
            AddExecutor(ExecutorType.Activate, CardId.DiceJar);
            AddExecutor(ExecutorType.Summon, CardId.CardcarD);
            AddExecutor(ExecutorType.Summon, CardId.AbouluteKingBackJack, AbouluteKingBackJacksummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.AbouluteKingBackJack);
            
            AddExecutor(ExecutorType.Activate, CardId.MichionTimelord);
            AddExecutor(ExecutorType.Activate, CardId.SandaionTheTimelord, SandaionTheTimelordeff);
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
            //battle
            AddExecutor(ExecutorType.Activate, CardId.BlazingMirrorForce, BlazingMirrorForceeff);
            AddExecutor(ExecutorType.Activate, CardId.MagicCylinder, MagicCylindereff);
            AddExecutor(ExecutorType.Activate, CardId.ThreateningRoar, ThreateningRoareff);
            AddExecutor(ExecutorType.Activate, CardId.Waboku, Wabokueff);
            AddExecutor(ExecutorType.Activate, CardId.BattleFader, BattleFadereff);                      
            AddExecutor(ExecutorType.Activate, CardId.RingOfDestruction, Ring_act);
            //chain            
            AddExecutor(ExecutorType.Activate, CardId.JustDesserts, JustDessertseff);            
            AddExecutor(ExecutorType.Activate, CardId.Ceasefire, Ceasefireeff);
            AddExecutor(ExecutorType.Activate, CardId.SecretBlast, SecretBlasteff);
            AddExecutor(ExecutorType.Activate, CardId.SectetBarrel, SectetBarreleff);
            AddExecutor(ExecutorType.Activate, CardId.RecklessGreed, RecklessGreedeff);
            AddExecutor(ExecutorType.Activate, CardId.OjamaTrio, OjamaTrioeff);
            AddExecutor(ExecutorType.Activate, CardId.AbouluteKingBackJack, AbouluteKingBackJackeff);
            AddExecutor(ExecutorType.Activate, CardId.ChainStrike, ChainStrikeeff);
            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);

        }
        public int[] all_List()
        {
            return new[]
            {
                CardId.SandaionTheTimelord,
                CardId.MichionTimelord,
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
            CardId.MichionTimelord,
            CardId.SandaionTheTimelord,
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
        public int[] pot_list()
        {
            return new[]
            {
                CardId.PotOfDesires,                
                CardId.MichionTimelord,
                CardId.SandaionTheTimelord,
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
        public bool Has_prevent_list_0(int id)
        {
            return (
                    id == CardId.Waboku ||
                    id == CardId.ThreateningRoar||
                    id == CardId.MagicCylinder||
                    id == CardId.BlazingMirrorForce||
                    id == CardId.RingOfDestruction
                   );
        }
        public bool Has_prevent_list_1(int id)
        {
            return (id == CardId.SandaionTheTimelord ||
                    id == CardId.BattleFader ||
                    id == CardId.MichionTimelord
                   );
        }
        bool no_sp = false;
        bool one_turn_kill = false;
        bool one_turn_kill_1 = false;
        int expected_blood = 0;
        bool prevent_used = false;
        int preventcount = 0;        
        bool OjamaTrioused = false;        
        bool OjamaTrioused_draw = false;
        bool OjamaTrioused_do = false;
        bool drawfirst = false;
        bool Linkuribohused = true;
        bool Timelord_check = false;
        int Waboku_count = 0;
        int Roar_count = 0;
        int strike_count = 0;        
        int greed_count = 0;
        int blast_count = 0;
        int barrel_count = 0;
        int just_count = 0;
        int Ojama_count = 0;
        int HasAccuulatedFortune = 0;

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            if (Bot.HasInHand(CardId.SandaionTheTimelord) ||Bot.HasInHand(CardId.MichionTimelord))
                Logger.DebugWriteLine("2222222222222222SandaionTheTimelord");
            no_sp = false;
            prevent_used = false;
            Linkuribohused = true;
            Timelord_check = false;
            base.OnNewTurn();
        }
        public override void OnNewPhase()
        {
            preventcount = 0;            
            OjamaTrioused = false;

            IList<ClientCard> trap = Bot.GetSpells();
            IList<ClientCard> monster = Bot.GetMonsters();

            foreach (ClientCard card in trap)
            {
                if (Has_prevent_list_0(card.Id))
                {
                    preventcount++;                    
                }
            }
            foreach (ClientCard card in monster)
            {
                if (Has_prevent_list_1(card.Id))
                {
                    preventcount++;                   
                }
            }
            foreach (ClientCard card in monster)
            {
                if (Bot.HasInMonstersZone(CardId.SandaionTheTimelord) ||
                    Bot.HasInMonstersZone(CardId.MichionTimelord))
                {
                    prevent_used = true;
                    Timelord_check = true;
                }
            }
            if(prevent_used && Timelord_check)
            {
                if (!Bot.HasInMonstersZone(CardId.SandaionTheTimelord) ||
                    !Bot.HasInMonstersZone(CardId.MichionTimelord))
                    prevent_used = false;
            }
            expected_blood = 0;
            one_turn_kill = false;
            one_turn_kill_1 = false;
            OjamaTrioused_draw = false;
            OjamaTrioused_do = false;
            drawfirst = false;
            HasAccuulatedFortune = 0;
            strike_count = 0;            
            greed_count = 0;
            blast_count = 0;
            barrel_count = 0;
            just_count = 0;
            Waboku_count = 0;
            Roar_count = 0;
            Ojama_count = 0;

            IList<ClientCard> check = Bot.GetSpells();
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.AccuulatedFortune))
                    HasAccuulatedFortune++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.SecretBlast))
                    blast_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.SectetBarrel))
                    barrel_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.JustDesserts))
                    just_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.ChainStrike))
                    strike_count++;

            }
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(CardId.RecklessGreed))
                    greed_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.Waboku))
                    Waboku_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.IsCode(CardId.ThreateningRoar))
                    Roar_count++;

            }            
            expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count);           
            if (Enemy.LifePoints <= expected_blood && Duel.Player == 1)
            {
                Logger.DebugWriteLine(" one_turn_kill");
                one_turn_kill = true;
            }
            expected_blood = 0;
            if (greed_count >= 2) greed_count = 1;
            if (blast_count >= 2) blast_count = 1;
            if (just_count >= 2) just_count = 1;
            if (barrel_count >= 2) barrel_count = 1;
            if (Waboku_count >= 2) Waboku_count = 1;
            if (Roar_count >= 2) Roar_count = 1;
            int currentchain = 0;
            if (OjamaTrioused_do)
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + Roar_count + greed_count + Ojama_count;
            else
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + greed_count + Roar_count;
            //if (currentchain >= 3 && Duel.Player == 1) drawfirst = true;
            if (Bot.HasInSpellZone(CardId.ChainStrike))
            {
                if (strike_count == 1)
                {
                    if (OjamaTrioused_do)
                        expected_blood = ((Enemy.GetMonsterCount() + 3) * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1) * 400);
                    else
                        expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1) * 400);
                }
                else
                {
                    if (OjamaTrioused_do)
                        expected_blood = ((Enemy.GetMonsterCount() + 3) * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1 + currentchain + 2) * 400);
                    else
                        expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1 + currentchain + 2) * 400);
                }
                if (!one_turn_kill && Enemy.LifePoints <= expected_blood && Duel.Player == 1)
                {
                    Logger.DebugWriteLine(" %%%%%%%%%%%%%%%%%one_turn_kill_1");
                    one_turn_kill_1 = true;
                    OjamaTrioused = true;
                }
            }
            }


        private bool must_chain()
        {
            if (Util.IsChainTarget(Card)) return true;
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card.IsCode(CardId.HarpiesFeatherDuster)&&card.IsFaceup())
                {
                    return true;
                }
            }
            return false;
        }
        private bool OjamaTrioset()
        {
            if (Bot.HasInSpellZone(CardId.OjamaTrio)) return false;           
            return true;
        }
        private bool BrunSpellSet()
        {
            if (Card.IsCode(CardId.OjamaTrio) && Bot.HasInSpellZone(CardId.OjamaTrio))return false;
            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay)) && Bot.GetSpellCountWithoutField() < 5;
        }
        private bool MichionTimelordsummon()
        {
            if (Duel.Turn == 1)
                return false;
            return true;
        }

        private bool SandaionTheTimelord_summon()
        {
            Logger.DebugWriteLine("&&&&&&&&&SandaionTheTimelord_summon");
            return true;            
        }
        private bool AbouluteKingBackJacksummon()
        {
            return !no_sp;
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
            no_sp = true;
            
            AI.SelectCard(pot_list());
            return true;
        }
        
        private bool ThreateningRoareff()
        {
            if (one_turn_kill_1) return UniqueFaceupSpell();
            if (drawfirst) return true;
            if (DefaultOnBecomeTarget())
            {
                prevent_used = true;
                return true;
            }
            if (prevent_used || Duel.Phase != DuelPhase.BattleStart) return false;
            prevent_used = true;
            return DefaultUniqueTrap();
        }
        private bool SandaionTheTimelordeff()
        {
            Logger.DebugWriteLine("***********SandaionTheTimelordeff");
            return true;
        }
        private bool Wabokueff()
        {
            if (one_turn_kill_1) return UniqueFaceupSpell();
            if (drawfirst)
            {
                Linkuribohused = false;
                return true;
            }                
            if (DefaultOnBecomeTarget())
            {
                Linkuribohused = false;
                prevent_used = true;
                return true;
            }            
            if (prevent_used||Duel.Player == 0||Duel.Phase!=DuelPhase.BattleStart) return false;
            prevent_used = true;
            Linkuribohused = false;
            return DefaultUniqueTrap();
        }
        private bool BattleFadereff()
        {
            if (Util.ChainContainsCard(CardId.BlazingMirrorForce) || Util.ChainContainsCard(CardId.MagicCylinder))
                return false;
            if (prevent_used || Duel.Player == 0) return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            prevent_used = true;
            return true;
        }

        private bool BlazingMirrorForceeff()
        {
            if (prevent_used) return false;
            IList<ClientCard> list = new List<ClientCard>();
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster.IsAttack())
                    list.Add(monster);
            }
            if (GetTotalATK(list) / 2 >= Bot.LifePoints) return false;
            Logger.DebugWriteLine("!!!!!!!!BlazingMirrorForceeff" + GetTotalATK(list) / 2);
            if (GetTotalATK(list) / 2 >= Enemy.LifePoints) return DefaultUniqueTrap();
            if (GetTotalATK(list) < 3000) return false;  
            prevent_used = true;
            return DefaultUniqueTrap();
                        
        }

        private bool MagicCylindereff()
        {
            if (prevent_used) return false;
            if (Bot.LifePoints <= Enemy.BattlingMonster.Attack) return DefaultUniqueTrap();
            if (Enemy.LifePoints <= Enemy.BattlingMonster.Attack) return DefaultUniqueTrap();
            if (Enemy.BattlingMonster.Attack>1800) return DefaultUniqueTrap();
            return false;
        }
        public bool Ring_act()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard() != null ) return false;
            ClientCard target = Util.GetProblematicEnemyMonster();
            if (target == null && Util.IsChainTarget(Card))
            {
                target = Util.GetBestEnemyMonster(true, true);
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
            if (one_turn_kill_1) return UniqueFaceupSpell();
            int count=0;
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(CardId.RecklessGreed))
                    count++;

            }           
            bool Demiseused = Util.ChainContainsCard(CardId.CardOfDemise);
            if (drawfirst) return UniqueFaceupSpell();
            if (DefaultOnBecomeTarget() && count > 1) return true;
            if (Demiseused) return false;             
            if (count > 1) return true;
            if (Bot.LifePoints <= 3000) return true;
            if (Bot.GetHandCount() <1 && Duel.Player==0 && Duel.Phase!=DuelPhase.Standby) return true;
            return false;
        }
        private bool SectetBarreleff()
        {
            if (DefaultOnBecomeTarget()) return true;
            if (Duel.Player == 0) return false;
            if (drawfirst) return true;
            if (one_turn_kill_1) return UniqueFaceupSpell();
            if (one_turn_kill) return true;
            if (DefaultOnBecomeTarget()) return true;
            int count = Enemy.GetFieldHandCount();
            int monster_count = Enemy.GetMonsterCount() - Enemy.GetMonstersExtraZoneCount();
            if (Enemy.LifePoints < count * 200) return true;
            if (Bot.HasInSpellZone(CardId.OjamaTrio) && monster_count <= 2 && monster_count >= 1)
            {
                if (count + 3 >= 8)
                {
                    OjamaTrioused = true;
                    return true;
                }
            }
            if (count >= 8) return true;
            return false;
        }
        private bool SecretBlasteff()
        {
            if (DefaultOnBecomeTarget()) return true;
            if (Duel.Player == 0) return false;
            if (drawfirst) return UniqueFaceupSpell();
            if (one_turn_kill_1) return UniqueFaceupSpell();
            if (one_turn_kill) return true;            
            int count = Enemy.GetFieldCount();
            int monster_count = Enemy.GetMonsterCount() - Enemy.GetMonstersExtraZoneCount();
            if (Enemy.LifePoints < count * 300) return true;
            if(Bot.HasInSpellZone(CardId.OjamaTrio) && monster_count <= 2 && monster_count >= 1 )
            {
                if(count+3>=5)
                {
                    OjamaTrioused = true;
                    return true;
                }
            }
            if (count >= 5) return true;
            return false;

        }
        
        private bool OjamaTrioeff()
        {
            return OjamaTrioused||OjamaTrioused_draw;
        }
        private bool JustDessertseff()
        {
            if (DefaultOnBecomeTarget()) return true;
            if (Duel.Player == 0) return false;
            if (drawfirst) return UniqueFaceupSpell();
            if (one_turn_kill_1) return UniqueFaceupSpell();
            if (one_turn_kill) return true;            
            int count = Enemy.GetMonsterCount()-Enemy.GetMonstersExtraZoneCount();
            if (Enemy.LifePoints <= count * 500) return true;
            if (Bot.HasInSpellZone(CardId.OjamaTrio) && count <= 2 && count >= 1)
            {
                OjamaTrioused = true;
                return true;
            }
            if (count >= 3) return true;
            return false;
        }
        private bool ChainStrikeeff()
        {
            if (one_turn_kill) return true;
            if (one_turn_kill_1) return true;
            if (drawfirst) return true;
            if (DefaultOnBecomeTarget()) return true;
            int chain = Duel.CurrentChain.Count;
            if (strike_count >= 2 && chain >= 2) return true;
            if (Enemy.LifePoints <= (chain + 1) * 400) return true;
            if (Duel.CurrentChain.Count >= 3) return true;
            return false;
        }

        private bool BalanceOfJudgmenteff()
        {
            if (DefaultOnBecomeTarget()) return true;
            int count = (Enemy.GetFieldCount() - Bot.GetFieldHandCount());
            if ( count>= 2)return true;
            return false;
        }
        private bool CardOfDemiseeff()
        {
            foreach (ClientCard card in Bot.GetMonsters())
            {
                if (card.IsCode(CardId.CardcarD) && card.IsFaceup())
                    return false;
            }
            if (Bot.GetHandCount() == 1 && Bot.GetSpellCountWithoutField() <= 3)
            {
                no_sp = true;
                return true;
            }
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
                if (card.IsCode(CardId.DiceJar) && card.IsFacedown())
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
            IList<ClientCard> newlist = new List<ClientCard>();
            foreach (ClientCard newmonster in Enemy.GetMonsters())
            {
                if (newmonster.IsAttack())
                    newlist.Add(newmonster);
            }
            if (!Linkuribohused) return false;
            if(Enemy.BattlingMonster!=null)
            {
                if (Enemy.BattlingMonster.Attack > 1800 && Bot.HasInSpellZone(CardId.MagicCylinder)) return false;
            }
            if (GetTotalATK(newlist) / 2 >= Bot.LifePoints && Bot.HasInSpellZone(CardId.BlazingMirrorForce))
                return true;
            if (GetTotalATK(newlist) / 2 >= Enemy.LifePoints && Bot.HasInSpellZone(CardId.BlazingMirrorForce))
                return false;
            if (Util.GetLastChainCard() == null) return true;
            if (Util.GetLastChainCard().IsCode(CardId.Linkuriboh)) return false;
            return true;
        }
        public bool MonsterRepos()
        {
            if (Card.IsFacedown() && !Card.IsCode(CardId.DiceJar))
                return true;
            return base.DefaultMonsterRepos();
        }
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (attacker.IsCode(CardId.Linkuriboh) && defender.IsFacedown()) return false;
            if (attacker.IsCode(CardId.SandaionTheTimelord) && !attacker.IsDisabled())
            {
                attacker.RealPower = 9999;
                return true;
            }
            if(attacker.IsCode(CardId.MichionTimelord) && !attacker.IsDisabled())
            {
                attacker.RealPower = 9999;
                return true;
            }
            return base.OnPreBattleBetween(attacker,defender);
        }

        public override void OnChaining(int player, ClientCard card)
        {            
            expected_blood = 0;
            one_turn_kill = false;
            one_turn_kill_1 = false;
            OjamaTrioused_draw = false;
            OjamaTrioused_do = false;
            drawfirst = false;
            HasAccuulatedFortune = 0;
            strike_count = 0;           
            greed_count = 0;
            blast_count = 0;
            barrel_count = 0;
            just_count = 0;
            Waboku_count = 0;
            Roar_count = 0;
            Ojama_count = 0;

            IList<ClientCard> check = Bot.GetSpells();
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.AccuulatedFortune))
                    HasAccuulatedFortune++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.SecretBlast))
                    blast_count++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.SectetBarrel))
                    barrel_count++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.JustDesserts))
                    just_count++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.ChainStrike))
                    strike_count++;

            }
            foreach (ClientCard card1 in Bot.GetSpells())
            {
                if (card1.IsCode(CardId.RecklessGreed))
                    greed_count++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.Waboku))
                    Waboku_count++;

            }
            foreach (ClientCard card1 in check)
            {
                if (card1.IsCode(CardId.ThreateningRoar))
                    Roar_count++;

            }
            /*if (Bot.HasInSpellZone(CardId.OjamaTrio) && Enemy.GetMonsterCount() <= 2 && Enemy.GetMonsterCount() >= 1)
            {
                if (HasAccuulatedFortune > 0) OjamaTrioused_draw = true;
            }*/
            if (Bot.HasInSpellZone(CardId.OjamaTrio) && (Enemy.GetMonsterCount() - Enemy.GetMonstersExtraZoneCount()) <= 2 && 
                (Enemy.GetMonsterCount() - Enemy.GetMonstersExtraZoneCount()) >= 1)
            {
                 OjamaTrioused_do = true;
            }
            expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count);
            if (Enemy.LifePoints <= expected_blood && Duel.Player == 1)
            {
                Logger.DebugWriteLine(" %%%%%%%%%%%%%%%%%one_turn_kill");
                one_turn_kill = true;
            }
            expected_blood = 0;
            if (greed_count >= 2) greed_count = 1;
            if (blast_count >= 2) blast_count = 1;
            if (just_count >= 2) just_count = 1;
            if (barrel_count >= 2) barrel_count = 1;
            if (Waboku_count >= 2) Waboku_count = 1;
            if (Roar_count >= 2) Roar_count = 1;            
            int currentchain = 0;
            if (OjamaTrioused_do)
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + Roar_count + greed_count + Ojama_count;
            else
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + greed_count + Roar_count ;
            //if (currentchain >= 3 && Duel.Player == 1) drawfirst = true;
            if(Bot.HasInSpellZone(CardId.ChainStrike))
            {
                if (strike_count == 1)
                {
                    if (OjamaTrioused_do)
                        expected_blood = ((Enemy.GetMonsterCount() + 3) * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1) * 400);
                    else
                        expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1) * 400);
                }
                else
                {
                    if (OjamaTrioused_do)
                        expected_blood = ((Enemy.GetMonsterCount() + 3) * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1 + currentchain + 2) * 400);
                    else
                        expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count + (currentchain + 1 + currentchain + 2) * 400);
                }
                if (!one_turn_kill && Enemy.LifePoints <= expected_blood && Duel.Player == 1)
                {
                    Logger.DebugWriteLine(" %%%%%%%%%%%%%%%%%one_turn_kill_1");
                    one_turn_kill_1 = true;
                    OjamaTrioused = true;
                }
            }
            base.OnChaining(player, card);           
        }
    }
}