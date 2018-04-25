using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("MirrorPhatomKnight", "AI_MirrorPhatomKnight", "Normal")]
    public class MirrorPhatomKnightExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DarkestDiabolosLordOfTheLair = 50383626;
            public const int AhrimaKingOfWickeness = 86377375;
            public const int LilithLadyOfLament = 23898021;
            public const int TourGuideFromTheUnderworld = 10802915;
            public const int PhantomKnightsOfRaggedGloves = 63821877;
            public const int MalebrancheOfTheBurningAbyss = 84764038;
            public const int PhantomKnightsOfAncientCloak = 90432163;
            public const int PhantomKnightsOfSilentBoots = 36426778;

            public const int TheFangOfCritias = 11082056;
            public const int HarpiesFeatherDuster = 18144506;
            public const int MonsterReborn = 83764718;
            public const int BurialFromADifferentDimension = 48976825;
            public const int LairOfDarkness = 59160188;

            public const int ThePhantomKnightsOfMistClaws = 9336190;
            public const int ShrubSerpent = 10813327;
            public const int MirrrorForce = 44095762;
            public const int JarOfAvarice = 98954106;
            public const int PhantomKnightsFogBlade = 25542642;
            public const int MirrorForceLauncher = 101005069;

            public const int NaturiaExterio = 99916754;
            public const int MirrorForceDragon = 84687358;
            public const int FlowerCardianLightflare = 87460579;
            public const int CrysralWingSynchroDragon = 50954680;
            public const int RaidraptorUltimateFalcon = 86221741;
            public const int ThePhantomKnoghtsOfBreakSword = 62709239;
            public const int TravelerOfTheBurningAbyss = 83531441;
            public const int TopologicBomberDragon = 5821478;
            public const int SummonSorceress = 61665245;
            public const int AkashicMagician = 28776350;
            public const int BlachAngelOfTheBurningAbyss = 58699500;
        }

        public MirrorPhatomKnightExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AhrimaKingOfWickeness,AhrimaKingOfWickenesseff);
            //ns
            AddExecutor(ExecutorType.Summon, CardId.TourGuideFromTheUnderworld);
            AddExecutor(ExecutorType.Activate, CardId.TourGuideFromTheUnderworld, TourGuideFromTheUnderworldeff);
            //chain
            AddExecutor(ExecutorType.Activate, CardId.DarkestDiabolosLordOfTheLair, DarkestDiabolosLordOfTheLaireff);
            //spellact
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.LairOfDarkness, LairOfDarknesseff);
            //trapset
            AddExecutor(ExecutorType.SpellSet, Spellseteff);
            //trapact
            AddExecutor(ExecutorType.Activate, CardId.JarOfAvarice, JarOfAvariceeff);
        }
        public int[] all_List()
        {
            return new[]
            {
                CardId.DarkestDiabolosLordOfTheLair,
                CardId.AhrimaKingOfWickeness,
                CardId.LilithLadyOfLament,
                CardId.TourGuideFromTheUnderworld,
                CardId.PhantomKnightsOfRaggedGloves,
                CardId.MalebrancheOfTheBurningAbyss,
                CardId.PhantomKnightsOfAncientCloak,
                CardId.PhantomKnightsOfSilentBoots,

                CardId.TheFangOfCritias,
                CardId.HarpiesFeatherDuster,
                CardId.MonsterReborn,
                CardId.BurialFromADifferentDimension,
                CardId.LairOfDarkness,

                CardId.ThePhantomKnightsOfMistClaws,
                CardId.ShrubSerpent,
                CardId.MirrrorForce,
                CardId.JarOfAvarice,
                CardId.PhantomKnightsFogBlade,
                CardId.MirrorForceLauncher,

                CardId.NaturiaExterio,
                CardId.MirrorForceDragon,
                CardId.FlowerCardianLightflare,
                CardId.CrysralWingSynchroDragon,
                CardId.RaidraptorUltimateFalcon,
                CardId.ThePhantomKnoghtsOfBreakSword,
                CardId.TravelerOfTheBurningAbyss,
                CardId.TopologicBomberDragon,
                CardId.SummonSorceress,
                CardId.AkashicMagician,
                CardId.BlachAngelOfTheBurningAbyss,              
    };
        }
        
        public int GetTotalATK(IList<ClientCard> list)
        {
            int atk = 0;
            foreach (ClientCard monster in list)
            {
                if (monster == null) continue;
                atk += monster.Attack;
            }
            return atk;
        }
        
        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
                       
           

        }
        public override void OnNewPhase()
        {/*
            preventcount = 0;
            battleprevent = false;
            OjamaTrioused = false;
           
            IList<ClientCard> trap = Bot.GetSpells();
            IList<ClientCard> monster = Bot.GetMonsters();

            foreach (ClientCard card in trap)
            {
                if (Has_prevent_list_0(card.Id))
                    {
                    preventcount++;
                    battleprevent = true;
                }

            }
            foreach (ClientCard card in monster)
            {
                if (Has_prevent_list_1(card.Id))
                {
                    preventcount++;
                    battleprevent = true;
                }

            }

            expected_blood = 0;
            one_turn_kill = false;
            one_turn_kill_1 = false;
            OjamaTrioused_draw = false;
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
                if (card.Id == CardId.AccuulatedFortune)
                    HasAccuulatedFortune++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.SecretBlast)
                    blast_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.SectetBarrel)
                    barrel_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.JustDesserts)
                    just_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.ChainStrike)
                    strike_count++;

            }
            foreach (ClientCard card in Bot.GetSpells())
            {
                if (card.Id == CardId.RecklessGreed)
                    greed_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.Waboku)
                    Waboku_count++;

            }
            foreach (ClientCard card in check)
            {
                if (card.Id == CardId.ThreateningRoar)
                    Roar_count++;

            }
            if (Bot.HasInSpellZone(CardId.OjamaTrio) && Enemy.GetMonsterCount() <= 2 && Enemy.GetMonsterCount() >= 1)
            {
                if (HasAccuulatedFortune>0) OjamaTrioused_draw = true;
            }

            expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count);
            //if (Enemy.LifePoints <= expected_blood && Duel.Player == 1) one_turn_kill = true;
            if (greed_count >= 2) greed_count = 1;
            if (blast_count >= 2) blast_count = 1;
            if (just_count >= 2) just_count = 1;
            if (barrel_count >= 2) barrel_count = 1;
            if (Waboku_count >= 2) Waboku_count = 1;
            if (Roar_count >= 2) Roar_count = 1;
            if (strike_count >= 2) strike_count = 1;
            int currentchain = 0;
            if (OjamaTrioused_draw)
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + Roar_count + greed_count + strike_count + Ojama_count;
            else
                currentchain = Duel.CurrentChain.Count + blast_count + just_count + barrel_count + Waboku_count + Waboku_count + greed_count + Roar_count + strike_count;
            //if (currentchain >= 3 && Duel.Player == 1) drawfirst = true;
            currentchain = Duel.CurrentChain.Count+ blast_count + just_count+barrel_count;
            expected_blood = (Enemy.GetMonsterCount() * 500 * just_count + Enemy.GetFieldHandCount() * 200 * barrel_count + Enemy.GetFieldCount() * 300 * blast_count+(currentchain+1)*400);
            //if (Enemy.LifePoints <= expected_blood && Duel.Player==1) one_turn_kill_1 = true;
*/
        }
             

        private bool LairOfDarknesseff()
        {
            if (Card.Location == CardLocation.SpellZone) 
            return true;

            return DefaultUniqueTrap();     
        }


        private bool AhrimaKingOfWickenesseff()
        {
            return true;
        }

        private bool DarkestDiabolosLordOfTheLaireff()
        {
            if (Card.Location != CardLocation.MonsterZone)
                return true;
            if(Bot.HasInMonstersZone(CardId.LairOfDarkness+1))
            {
                AI.SelectCard(CardId.LairOfDarkness + 1);
                return true;
            }
            return false;
        }


        private bool TourGuideFromTheUnderworldeff()
        {
            AI.SelectCard(new[] {
                CardId.MalebrancheOfTheBurningAbyss,
                CardId.LilithLadyOfLament,
            });
            return true;
        }

        private bool Spellseteff()
        {
            if (Card.HasType(CardType.Spell)) return false;
            if (Card.Id == CardId.MirrrorForce || 
                Card.Id == CardId.PhantomKnightsFogBlade||
                Card.Id==CardId.MirrorForceLauncher
                ) return true;
            if (Bot.GetSpellCountWithoutField() > 4) return false;

            return true;
        }

        private bool JarOfAvariceeff()
        {
            if (Bot.Graveyard.Count >= 10)
                return true;
            return false;
        }
       /* public bool Grass_ss()
        {
            // judge whether can ss from exdeck
            bool judge = (Bot.ExtraDeck.Count > 0);
            if (Enemy.GetMonstersExtraZoneCount() > 1) judge = false; // exlink
            if (Bot.GetMonstersExtraZoneCount() >= 1)
            {
                foreach (ClientCard card in Bot.GetMonstersInExtraZone())
                {
                    if (getLinkMarker(card.Id) == 5) judge = false;
                }
            }
            // can ss from exdeck
            if (judge)
            {
                bool fornextss = AI.Utils.ChainContainsCard(CardId.Grass);
                IList<ClientCard> ex = Bot.ExtraDeck;
                ClientCard ex_best = null;
                foreach (ClientCard ex_card in ex)
                {
                    if (!fornextss)
                    {
                        if (ex_best == null || ex_card.Attack > ex_best.Attack) ex_best = ex_card;
                    }
                    else
                    {
                        if (getLinkMarker(ex_card.Id) != 5 && (ex_best == null || ex_card.Attack > ex_best.Attack)) ex_best = ex_card;
                    }
                }
                if (ex_best != null)
                {
                    AI.SelectCard(ex_best);
                }
            }
            if (!judge || AI.Utils.ChainContainsCard(CardId.Grass))
            {
                // cannot ss from exdeck or have more than 1 grass in chain
                int[] secondselect = new[]
                {
                    CardId.Borrel,
                    CardId.Ultimate,
                    CardId.Abyss,
                    CardId.Cardian,
                    CardId.Exterio,
                    CardId.Ghost,
                    CardId.White,
                    CardId.Red,
                    CardId.Yellow,
                    CardId.Pink
                };
                if (!AI.Utils.ChainContainsCard(CardId.Grass))
                {
                    if (!judge && Bot.GetRemainingCount(CardId.Ghost, 2) > 0)
                    {
                        AI.SelectCard(CardId.Ghost);
                        AI.SelectPosition(CardPosition.FaceUpDefence);
                    }
                    else
                        AI.SelectCard(secondselect);
                }
                else
                {
                    if (!judge)
                        AI.SelectCard(secondselect);
                    AI.SelectNextCard(secondselect);
                    AI.SelectThirdCard(secondselect);
                }
            }
            return true;
        }*/
        /*    private bool OjamaTrioset()
            {
                if (Bot.HasInSpellZone(CardId.OjamaTrio)) return false;           
                return true;
            }
            private bool BrunSpellSet()
            {
                if (Card.Id == CardId.OjamaTrio && Bot.HasInSpellZone(CardId.OjamaTrio))return false;
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
                if (drawfirst) return true;
                if (must_chain()) return DefaultUniqueTrap();
                if (prevent_used || Duel.Phase != DuelPhase.BattleStart) return false;
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
                if (drawfirst)
                {
                    Linkuribohused = false;
                    return true;
                }                
                if (must_chain())
                {
                    Linkuribohused = false;
                    return DefaultUniqueTrap();
                }            
                if (prevent_used||Duel.Player == 0||Duel.Phase!=DuelPhase.BattleStart) return false;
                prevent_used = true;
                Linkuribohused = false;
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
                int count=0;
                foreach (ClientCard card in Bot.GetSpells())
                {
                    if (card.Id == CardId.RecklessGreed)
                        count++;

                }           
                bool Demiseused = AI.Utils.ChainContainsCard(CardId.CardOfDemise);
                if (drawfirst) return DefaultUniqueTrap();
                if (must_chain() && count > 1) return true;
                if (Demiseused) return false;             
                if (count > 1) return true;
                if (Bot.LifePoints <= 2000) return true;
                if (Bot.GetHandCount() <1 && Duel.Player==0 && Duel.Phase!=DuelPhase.Standby) return true;
                return false;
            }
            private bool SectetBarreleff()
            {
                if (drawfirst) return DefaultUniqueTrap();
                if (one_turn_kill_1) return DefaultUniqueTrap();
                if (one_turn_kill) return DefaultUniqueTrap();
                if (must_chain()) return true;
                int count = Enemy.GetFieldHandCount();
                if (Enemy.LifePoints < count * 200) return true;
                if (count >= 8) return true;
                return false;
            }
            private bool SecretBlasteff()
            {
                if (drawfirst) return DefaultUniqueTrap();
                if (one_turn_kill_1) return DefaultUniqueTrap();
                if (one_turn_kill) return DefaultUniqueTrap();
                if (must_chain()) return true;
                int count = Enemy.GetFieldCount();
                if (Enemy.LifePoints < count * 300) return true;
                if (count >= 5) return true;
                return false;

            }

            private bool OjamaTrioeff()
            {
                return OjamaTrioused||OjamaTrioused_draw;
            }
            private bool JustDessertseff()
            {
                if (Duel.Player == 0) return false;
                if (drawfirst) return DefaultUniqueTrap();
                if (one_turn_kill_1) return DefaultUniqueTrap();
                if (one_turn_kill) return DefaultUniqueTrap();
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

                if (drawfirst) return true;
                if (must_chain()) return true;
                int chain = Duel.CurrentChain.Count;
                if (strike_count >= 2 && chain >= 2) return true;
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

                IList<ClientCard> newlist = new List<ClientCard>();
                foreach (ClientCard newmonster in Enemy.GetMonsters())
                {
                    newlist.Add(newmonster);
                }
                if (!Linkuribohused) return false;
                if(Enemy.BattlingMonster!=null)
                {
                    if (Enemy.BattlingMonster.Attack > 1800 && Bot.HasInSpellZone(CardId.MagicCylinder)) return false;
                }            
                if (GetTotalATK(newlist) >= 3000 && Bot.HasInSpellZone(CardId.BlazingMirrorForce)) return false;           
                if (AI.Utils.GetLastChainCard() == null) return true;            
                if (AI.Utils.GetLastChainCard().Id == CardId.Linkuriboh)return false;
                return true;
            }

            public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
            {
                if (attacker.Id == CardId.Linkuriboh && defender.IsFacedown()) return false;
                return base.OnPreBattleBetween(attacker,defender);
            }*/
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