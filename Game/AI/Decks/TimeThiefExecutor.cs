using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using System.Reflection;

namespace WindBot.Game.AI.Decks
{
    [Deck("TimeThief", "AI_Timethief")]
    public class TimeThiefExecutor : DefaultExecutor
    {
        public class Monsters
        {
            //monsters
            public const int TimeThiefWinder = 56308388;
            public const int TimeThiefBezelShip = 82496079;
            public const int TimeThiefCronocorder = 74578720;
            public const int TimeThiefRegulator = 19891131;
            public const int PhotonTrasher = 65367484;
            public const int PerformTrickClown = 67696066;
        }
        
        public class Spells
        {
            // spells
            public const int UpstartGoblin = 70368879;
            public const int Raigeki = 12580477;
            public const int FoolishBurial = 81439173;
            public const int TimeThiefStartup = 10877309;
            public const int TimeThiefHack = 81670445;
        }
        public class Traps
        {
            //traps
            public const int XyzReborn = 26708437;
            public const int XyzExtreme = 57319935;
            public const int TimeThiefRetrograte = 76587747;
            public const int PhantomKnightsShade = 98827725;
            public const int TimeThiefFlyBack = 18678554;
        }
        public class XYZs
        {
            //xyz
            public const int TimeThiefRedoer = 55285840;
            public const int TimeThiefPerpetua = 59208943;
            public const int CrazyBox = 42421606;
        }
        

        
        public TimeThiefExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // executors
            //Spell activate
            AddExecutor(ExecutorType.Activate,Spells.UpstartGoblin);
            AddExecutor(ExecutorType.Activate,Spells.FoolishBurial,FoolishBurialTarget);
            AddExecutor(ExecutorType.Activate,Spells.TimeThiefStartup,TimeThiefStartupEffect);
            AddExecutor(ExecutorType.Activate,Spells.TimeThiefHack);  
            
            // trap executors set
            AddExecutor(ExecutorType.SpellSet,Traps.XyzExtreme);
            AddExecutor(ExecutorType.SpellSet,Traps.XyzReborn);
            AddExecutor(ExecutorType.SpellSet,Traps.PhantomKnightsShade);
            AddExecutor(ExecutorType.SpellSet,Traps.TimeThiefRetrograte);
            AddExecutor(ExecutorType.SpellSet,Traps.TimeThiefFlyBack);
            
            //normal summons
            AddExecutor(ExecutorType.Summon,Monsters.TimeThiefRegulator  );
            AddExecutor(ExecutorType.SpSummon, Monsters.PhotonTrasher, SummonToDef );
            AddExecutor(ExecutorType.Summon,Monsters.TimeThiefWinder );
            AddExecutor(ExecutorType.Summon,Monsters.TimeThiefBezelShip );
            AddExecutor(ExecutorType.Summon,Monsters.PerformTrickClown );
            AddExecutor(ExecutorType.Summon,Monsters.TimeThiefCronocorder );
            //xyz summons
            AddExecutor(ExecutorType.SpSummon,XYZs.TimeThiefRedoer);
            AddExecutor(ExecutorType.SpSummon,XYZs.TimeThiefPerpetua);
            // activate trap
            AddExecutor(ExecutorType.Activate,Traps.PhantomKnightsShade);
            AddExecutor(ExecutorType.Activate,Traps.XyzExtreme , XyzExtremeEffect);
            AddExecutor(ExecutorType.Activate,Traps.XyzReborn , XyzRebornEffect);
            AddExecutor(ExecutorType.Activate,Traps.TimeThiefRetrograte , RetrograteEffect);
            AddExecutor(ExecutorType.Activate,Traps.TimeThiefFlyBack );
            
            //xyz effects
            AddExecutor(ExecutorType.Activate,XYZs.TimeThiefRedoer,RedoerEffect);
            AddExecutor(ExecutorType.Activate,XYZs.TimeThiefPerpetua , PerpertuaEffect);
            
            //monster effects
            AddExecutor(ExecutorType.Activate,Monsters.TimeThiefRegulator , RegulatorEffect);
            AddExecutor(ExecutorType.Activate,Monsters.TimeThiefWinder);
            AddExecutor(ExecutorType.Activate,Monsters.TimeThiefCronocorder);
            AddExecutor(ExecutorType.Activate,Monsters.PerformTrickClown, TrickClownEffect);
            AddExecutor(ExecutorType.Activate,Monsters.TimeThiefBezelShip);   
        }

        private bool SummonToDef()
        {
            AI.SelectPosition(CardPosition.Defence);
            return true;
        }
        

        private bool RegulatorEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(Monsters.TimeThiefCronocorder);
                AI.SelectCard(Monsters.TimeThiefWinder);
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        private bool PerpertuaEffect()
        {
            if (Bot.HasInGraveyard(XYZs.TimeThiefRedoer))
            {
                AI.SelectCard(XYZs.TimeThiefRedoer);
                return true;
            }

            if (Bot.HasInMonstersZone(XYZs.TimeThiefRedoer))
            {
                AI.SelectCard(Monsters.TimeThiefBezelShip);
                AI.SelectNextCard(XYZs.TimeThiefRedoer);
                return true;
            }

            return false;
        }

        private int _totalAttack;
        private int _totalBotAttack;
        private bool RedoerEffect()
        {

            List<ClientCard> enemy = Enemy.GetMonstersInMainZone();
            List<int> units = Card.Overlays;
            if (Duel.Phase == DuelPhase.Standby && (AI.Executor.Util.GetStringId(XYZs.TimeThiefRedoer,0) ==
                                                    ActivateDescription))
            {
                
                return true;
            }

            try
            {
                if (Bot.HasInSpellZone(Traps.XyzReborn))
                {
                    return false;
                }

                if (Bot.HasInSpellZone(Traps.XyzExtreme))
                {
                    return false;
                }

                for (int i = 0; i < enemy.Count; i++)
                {
                    _totalAttack += enemy[i].Attack;
                }

                foreach (var t in Bot.GetMonsters())
                {
                    _totalBotAttack += t.Attack;
                }

                if (_totalAttack > Bot.LifePoints + _totalBotAttack)
                {
                    return false;
                }
                
            

                foreach (var t in enemy)
                {
                    if (t.Attack < 2400 || !t.IsAttack()) continue;
                    try
                    {
                        AI.SelectCard(t.Id);
                        AI.SelectCard(t.Id);
                    }
                    catch{}

                    return true;
                }
            }
            catch{}

            if (Bot.UnderAttack)
            {
                //AI.SelectCard(Util.GetBestEnemyMonster());
                return true;
            }

            return false;
            
        }

        private bool RetrograteEffect()
        {
            if (Card.Owner== 1)
            {
                return true;
            }
            return false;
            
        }

        private bool XyzRebornEffect()
        {
            if (Bot.HasInGraveyard(XYZs.TimeThiefRedoer))
            {
                AI.SelectCard(XYZs.TimeThiefRedoer);
                return true;
            }
            return true;
            
        }
        //function
        private bool XyzExtremeEffect()
        {
            AI.SelectCard(XYZs.CrazyBox);
            return true;
        }
        private bool TimeThiefStartupEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInHand(Monsters.TimeThiefRegulator) && !(Bot.GetMonsterCount() > 0))
                {
                    AI.SelectCard(Monsters.TimeThiefRegulator);
                    return true;
                }
                if(Bot.HasInHand(Monsters.TimeThiefWinder) && Bot.GetMonsterCount()>1)
                {
                    AI.SelectCard(Monsters.TimeThiefWinder);
                    return true;
                }
                return true;
                
            }
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(Monsters.TimeThiefCronocorder);
                AI.SelectCard(Spells.TimeThiefHack);
                AI.SelectCard(Traps.TimeThiefFlyBack);
                return true;
            }
 
                return false;
            
        }
        private bool FoolishBurialTarget()
        {
            AI.SelectCard(Monsters.PerformTrickClown);
            return true;
        }
        
        private bool TrickClownEffect()
        {
            if (Bot.LifePoints <= 1000)
            {
                return false;
            }
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }



    }

}
