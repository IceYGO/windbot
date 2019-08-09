using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("BrandishLord", "AI_BrandishLord", "Normal")]
    public class BrandishLordExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int LordOfTheLair = 50383626;
            public const int BrandishMaidenRei = 26077387;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;
            public const int JetSynchron = 9742784;
            public const int GlowUpBulb = 67441435;
            public const int EffectVeiler = 97268402;
            public const int GhostRabbit = 59438930;
            public const int BrandishSkillJammingWave = 25955749;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int FoolishBurialGoods = 35726888;
            public const int TheMelodyOfAwakeningDragon = 48800175;
            public const int BrandishStartUpEngage = 63166095;
            public const int MetalfoesFusion = 73594093;
            public const int Terraforming = 73628505;
            public const int BrandishSkillAfterburner = 99550630;
            public const int Typhoon = 5318639;
            public const int TwinTwisters = 43898403;
            public const int HornetBit = 52340444;
            public const int Sharkcannon = 51227866;            
            public const int WidowAnchor = 98338152;
            public const int MultiRoll = 24010609;
            public const int HerculesBase = 97616504;
            public const int AreaZero = 50005218;

            public const int TorrentialTribute = 53582587;
            public const int SolemnStrike = 40605147;
            public const int InfiniteImpermanence = 10045474;

            public const int HiSpeedroidChanbara = 42110604;
            public const int TopologicBomberDragon = 5821478;
            public const int BorreloadDragon = 31833038;
            public const int BirrelswordDragon = 85289965;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int SummonSorceress = 61665245;
            public const int CrystronNeedlefiber = 50588353;           
            public const int KnightmareCerberus = 75452921;
            public const int BrandishMaidenHayate = 8491308;
            public const int BrandishMaidenShizuku = 90673288;
            public const int BrandishMaidenKagari = 63288573;            
            public const int Linkuriboh = 41999284;
            public const int NingirsuTheWorldChaliceWarrior = 30194529;


            public const int TopologicTrisbaena = 72529749;            
            public const int TroymareUnicorn = 38342335;
            public const int TroymarePhoenix = 2857636;

            public const int SolemnJudgment = 41420027;
            public const int SolemnWarning = 84749824;
            public const int UpstartGoblin = 70368879;
            public const int MacroCosmos = 30241314;
            public const int LockBird = 94145021;
            public const int GrenMajuDaEizo = 36584821;            
            public const int EaterOfMillions = 63845230;
            public const int UltimateConductorTytanno = 18940556;
            public const int DarkHole = 53129443;
            public const int GalaxySoldier = 46659709;
            public const int HeavymetalfoesElectrumite = 24094258;
            public const int ThatGrassLooksgreener = 11110587;
            public const int ElShaddollWinda = 94977269;
        }
        bool IsPendulumDeck = true;
        bool WaitHeavymetalfoes = false;
        int SpellZone_count = 0;        
        bool enemy_no_used_spell = false;
        bool save_resource = false;
        bool Rei_eff_used = false;
        bool Rei_grave_used = false;
        bool lord_eff_used = false;
        bool lord_sp_used = false;
        bool lord_can_do = false;
        bool lord_had_exist = false;        
        bool Plan_A = false;
        bool Plan_A_1 = false;
        bool Plan_A_2 = false;
        bool Plan_A_3 = false;
        bool Plan_B = false;
        bool Plan_B_1 = false;
        bool Plan_B_2 = false;
        bool Plan_B_3 = false;
        bool Plan_B_4 = false;
        bool Plan_B_5 = false;
        bool Plan_B_6 = false;
        bool Plan_C = false;
        bool Plan_C_1 = false;
        bool Plan_D = false;
        bool summon_used = false;
        bool KagariSummoned = false;
        bool ShizukuSummoned = false;
        bool HayateSummoned = false;
        bool AreaZero_used = false;
        bool MultiRoll_used = false;
        bool MaxxC_used = false;
        bool Lockbird_used = false;
        bool Widow_control = true;
        bool CrystronNeedlefiberSp_used = false;
        bool GlowUpBulbeff_used = false;
        bool base_draw_first = false;
        IList<ClientCard> basetargets = new List<ClientCard>();
        public BrandishLordExecutor(GameAI ai, Duel duel)
            : base(ai, duel)            
        {
            
            //counter 
            AddExecutor(ExecutorType.Activate, CardId.Typhoon, Typhooneffcounter);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, TorrentialTributeeff);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrikeeff);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffectFirst);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.Sharkcannon, Sharkcannoneff);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence,InfiniteImpermanenceeff);            
            AddExecutor(ExecutorType.Activate, CardId.TwinTwisters, TwinTwistersEffect);            
                     
            //first set
            AddExecutor(ExecutorType.SpellSet, CardId.HerculesBase, BaseSetFirst);
            AddExecutor(ExecutorType.Activate, CardId.NingirsuTheWorldChaliceWarrior, WarriorCleaneff);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillJammingWave, BrandishSkillJammingWaveEffFirst);
            //restart resource
            AddExecutor(ExecutorType.Activate, CardId.HerculesBase, HerculesBaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.AreaZero, ResourceRestart);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, ResourceRestart);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, MultiRollEffectFirst);
            AddExecutor(ExecutorType.Activate, NotBotField);
            AddExecutor(ExecutorType.Activate, CardId.AreaZero, AreaZeroSetFirst);            
            //first do
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DarkHoleeff);
            AddExecutor(ExecutorType.Activate, CardId.LordOfTheLair, LordOfTheLaireff);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin); 
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurialGoods, FoolishBurialGoodsEffect);        
            AddExecutor(ExecutorType.Activate,CardId.TheMelodyOfAwakeningDragon, TheMelodyOfAwakeningDragoneff);
            //clean
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillAfterburner, BrandishSkillAfterburnerEffect_A);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillJammingWave, BrandishSkillJammingWaveEffect_A);
            //plan a 
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenHayate, HayateSpFirst);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect_A2);
            AddExecutor(ExecutorType.Activate, CardId.HornetBit, HornetBitEffectFirst);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp);
            AddExecutor(ExecutorType.Summon, CardId.GlowUpBulb, GlowUpBulbsummon);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.GhostRabbit, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, TunerSummon);            
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp_A2);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbeff);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff);
            AddExecutor(ExecutorType.SpSummon, CardId.BirrelswordDragon, BirrelswordDragonsp);
            AddExecutor(ExecutorType.Activate, CardId.BirrelswordDragon, BirrelswordDragoneff);
            AddExecutor(ExecutorType.Activate, CardId.Typhoon, Typhooneff);
            //search
            AddExecutor(ExecutorType.Activate, CardId.BrandishStartUpEngage, BrandishStartUpEngageEffect);
            //plan b 3 plan b 5
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect_B3B5);
            //plan b
            AddExecutor(ExecutorType.Activate, CardId.HornetBit, HornetBitEffectFirst_B3B5);
            AddExecutor(ExecutorType.Activate, CardId.HornetBit, HornetBitEffectFirst_B);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, TunerSummon_B);
            AddExecutor(ExecutorType.Summon, CardId.GhostRabbit, TunerSummon_B);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, TunerSummon_B);
            AddExecutor(ExecutorType.Summon, CardId.GlowUpBulb, TunerSummon_B);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp_B);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp_B5);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect_B);
            AddExecutor(ExecutorType.SpSummon, CardId.SummonSorceress, SummonSorceresssp);
            AddExecutor(ExecutorType.Activate, CardId.SummonSorceress, SummonSorceresseff);
            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchroneff);
            AddExecutor(ExecutorType.SpSummon, CardId.HiSpeedroidChanbara, HiSpeedroidChanbarasp);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp_B);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboheff_B);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenHayate, HayateSp_B);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenShizuku, ShizukuSp_B);
            AddExecutor(ExecutorType.SpSummon, CardId.TopologicBomberDragon, TopologicBomberDragonsp);
            //plan c
            AddExecutor(ExecutorType.Summon, CardId.GlowUpBulb, GlowUpBulbsummon_C);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, TunerSummon_C);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, TunerSummon_C);
            AddExecutor(ExecutorType.Summon, CardId.GhostRabbit, TunerSummon_C);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, TunerSummon_C);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp_C);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect);
            AddExecutor(ExecutorType.Activate, CardId.GlowUpBulb, GlowUpBulbeff_C);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuribohsp_C);
            //clean
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillAfterburner, BrandishSkillAfterburnerEffect);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillJammingWave, BrandishSkillJammingWaveEffect);            
            
            //plan b 2
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, TunerSummon_B2);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect_B2);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenHayate, HayateSp_B2);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenShizuku, ShizukuSp_B2);

            //To battle phase
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            //card control           
            AddExecutor(ExecutorType.Activate, CardId.AreaZero, AreaZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenRei, BrandishMaidenReiEffect);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, MultiRollEffect);
            //spell set
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike);
            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, InfiniteImpermanenceset);
            //sp
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenKagari, KagariSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenKagari, KagariEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenHayate, HayateSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenHayate, HayateEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenShizuku, ShizukuSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenShizuku, ShizukuEffect);            
            //summon
            AddExecutor(ExecutorType.Summon, CardId.BrandishMaidenRei, BrandishMaidenReiSummon);
            AddExecutor(ExecutorType.Activate, CardId.HornetBit, HornetBitEffect);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, MultiRollsummon);
            AddExecutor(ExecutorType.Activate, CardId.Typhoon, Typhoonsummon);
            AddExecutor(ExecutorType.Activate, CardId.GhostRabbit, GhostRabbitsummon);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillJammingWave, BrandishSkillJammingWavesummon);
            //plan fail 
            AddExecutor(ExecutorType.SpSummon, CardId.NingirsuTheWorldChaliceWarrior, NingirsuTheWorldChaliceWarriorsp);

            //spell set
            AddExecutor(ExecutorType.SpellSet, CardId.TorrentialTribute, TorrentialTributeset);
            AddExecutor(ExecutorType.SpellSet, CardId.WidowAnchor, WidowAnchorset);
            AddExecutor(ExecutorType.SpellSet, CardId.HornetBit, HornetBitset);            
            AddExecutor(ExecutorType.SpellSet, SpellSet);            
            
            //otherthing           
            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffectWin);
            AddExecutor(ExecutorType.Repos, MonsterRepos);

        }     

        public override void OnNewPhase()
        {
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.Turn<=6)
                SpellZone_count += Enemy.GetSpellCount();
            if (SpellZone_count <= 4 && Duel.Turn ==7)
                enemy_no_used_spell = true;
            CheckPlan();
            if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.LordOfTheLair))
                lord_had_exist = true;
            base.OnNewPhase();
        }

        public override void OnNewTurn()
        {
            base_draw_first = false;
            save_resource = false;
            Rei_eff_used = false;
            Rei_grave_used = false;
            lord_eff_used = false;
            lord_sp_used = false;
            lord_can_do = false;
            CrystronNeedlefiberSp_used = false;
            KagariSummoned = false;
            ShizukuSummoned = false;
            HayateSummoned = false;
            Widow_control = true;        
            MaxxC_used = false;
            Lockbird_used = false;
            AreaZero_used = false;
            MultiRoll_used = false;
            summon_used = false;            
            Plan_A = false;
            Plan_A_1 = false;
            Plan_A_2 = false;
            Plan_A_3 = false;
            Plan_B = false;
            Plan_B_1 = false;
            Plan_B_2 = false;
            Plan_B_3 = false;
            Plan_B_4 = false;
            Plan_B_5 = false;
            Plan_B_6 = false;
            Plan_C = false;
            Plan_C_1 = false;
            Plan_D = false;
        }
        private bool Plan_A_check()
        {
            if (Duel.Turn>1 &&
                !Enemy.HasInMonstersZone(_CardId.UltimateConductorTytanno,true) &&
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
             !GlowUpBulbeff_used && GlowUpBulbExist() &&
             HasRemainTuner() && HasInHandTuner() &&
             !summon_used && !GetPlan() &&
             !Rei_eff_used && !Rei_grave_used &&
              Bot.GetMonstersInMainZone().Count == 0 &&
             !HayateSummoned && Bot.GetMonstersExtraZoneCount() >= 1 &&
             Bot.HasInExtra(CardId.BrandishMaidenHayate) &&
             Bot.HasInExtra(CardId.CrystronNeedlefiber) &&
             Bot.HasInExtra(CardId.BirrelswordDragon) &&
             Util.GetOneEnemyBetterThanValue(1500, true) != null &&             
             (Util.GetOneEnemyBetterThanValue(1500, true).Attack - 1500) < Bot.LifePoints &&
             EnemyMonsterSafe_AA1A2())
                return true;
            return false;
        }
        private bool Plan_B_check()
        {            
            if (!GetPlan() && Duel.Turn > 1 &&
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                Bot.Hand.Count >= 3 &&
                !summon_used &&
                SpellCanUse(CardId.HornetBit) &&
                Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0 &&                
                (!HayateSummoned || !ShizukuSummoned) &&
                Bot.HasInExtra(CardId.CrystronNeedlefiber) &&
                Bot.HasInExtra(CardId.SummonSorceress) &&
                (Bot.HasInExtra(CardId.BrandishMaidenHayate) || Bot.HasInExtra(CardId.BrandishMaidenShizuku)) &&
                EnemySetSpellCount() <= 2)
                return true;
            return false;
        }
        private void CheckPlan()
        {
            if (Bot.GetMonstersInMainZone().Count == 0 &&                
                Plan_A_check() &&
                Plan_A_Clean() &&
                SpellsCountInGrave()>=3 &&                
                SpellCanUse(CardId.WidowAnchor))
            {
                
                Plan_A_2 = true;
                Logger.DebugWriteLine("***********Plan_A_2");
            }            
            if (Bot.GetMonstersInMainZone().Count == 0 &&
                SpellCanUse(CardId.HornetBit) &&                
                Plan_A_check())               
            {
                
                Plan_A = true;
                Logger.DebugWriteLine("***********Plan_A");
            }
            if (Bot.HasInMonstersZone(CardId.HornetBit + 1) &&
                Bot.GetMonstersInMainZone().Count == 1 &&
                Plan_A_check())                
            {
               
                Plan_A_1 = true;
                Logger.DebugWriteLine("***********Plan_A_1");
            }
            /*if (!Plan_A && !Plan_A_1 &&
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                Bot.GetMonstersInMainZone().Count == 0 &&
                Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0 &&
                Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0 &&
                HasInHandTuner_B() &&
                !summon_used &&
                Bot.HasInHandOrInSpellZone(CardId.HornetBit) &&
                Bot.GetMonstersExtraZoneCount() >= 1 &&
                KagariSummoned &&
                Bot.HasInExtra(CardId.CrystronNeedlefiber) &&
                Bot.HasInExtra(CardId.SummonSorceress) &&
                Bot.HasInExtra(CardId.TopologicBomberDragon) &&
                (Bot.HasInExtra(CardId.Linkuriboh) || Bot.HasInGraveyard(CardId.Linkuriboh)) &&
                EnemySetSpellCount() <= 1 &&
                EnemyMonsterSafe_B()
                )
            {
                Plan_B = true;
                Logger.DebugWriteLine("***********Plan_B");
            }*/
            if (Plan_B_check() &&
                Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0 &&               
                HasInHandTuner_B() &&                
                Bot.HasInSpellZone(CardId.WidowAnchor) &&
                SpellsCountInGrave() >= 3 &&
                Bot.GetMonstersExtraZoneCount() >= 1 &&
                (KagariSummoned || save_resource) &&
                EnemyMonsterSafe_B3()
                )
            {
               
                Plan_B_3 = true;
                Logger.DebugWriteLine("***********Plan_B_3");
            }
            if (Plan_B_check() &&
                Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0 &&
                HasInHandTuner_B() &&
                SpellsCountInGrave() >= 3 &&                
                SpellCanUse(CardId.WidowAnchor) &&                
                Bot.GetMonsterCount()==0 &&
                EnemyMonsterSafe_B5()
                )
            {
              
                Plan_B_5 = true;
                Logger.DebugWriteLine("***********Plan_B_5");
            }
            if (Plan_B_check() &&               
                Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0 &&                
                HasInHandTuner_B() &&
                Bot.GetMonstersExtraZoneCount() >= 1 &&
                (KagariSummoned || save_resource) &&
                EnemyMonsterSafe_B1B2()
                )
            {
                
                Plan_B_1 = true;
                Logger.DebugWriteLine("***********Plan_B_1");
            }

            if (Plan_B_check() &&
                Bot.HasInHand(CardId.JetSynchron) && 
                HasRemainLevel_1_Tuner() &&
                Bot.GetMonstersExtraZoneCount() >= 1 &&
                (KagariSummoned || save_resource) &&
                EnemyMonsterSafe_B1B2()
                )
            {
                
                Plan_B_2 = true;
                Logger.DebugWriteLine("***********Plan_B_2");
            }           

            if (!GetPlan() && !summon_used && Duel.Turn > 1 &&
                Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 &&
                Bot.GetMonstersInMainZone().Count == 0 &&                
                HasRemainTuner() && HasInHandTuner() &&
                !GlowUpBulbeff_used && GlowUpBulbExist() &&
                (KagariSummoned || save_resource) &&
                Bot.GetMonstersExtraZoneCount() >= 1 &&
                Bot.HasInExtra(CardId.CrystronNeedlefiber) &&
                Bot.HasInExtra(CardId.BirrelswordDragon) &&
                Bot.HasInExtra(CardId.Linkuriboh) &&
                EnemyMonsterSafe_C() 
                )
            {
                
                Plan_C = true;
                Logger.DebugWriteLine("***********Plan_C");
            }
        }
        
        private bool GoToBattlePhase()
        {
            if(Enemy.GetMonsterCount()==0)
            {
                if(Util.GetTotalAttackingMonsterAttack(0)>=Enemy.LifePoints)
                {                   
                    return true;
                }
            }
            return false;
        }

        private bool DarkHoleeff()
        {
            if (Bot.GetMonsterCount() == 1 && Enemy.GetMonsterCount() >= 3 && 
                (BrandishMonsterRestart() || Bot.HasInHandOrInSpellZone(CardId.BrandishStartUpEngage)))
                return true;
            if (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() >= 2)
                return true;
            return false;
        }

        private bool TorrentialTributeeff()
        {
            if (Duel.Player == 1 &&
                Bot.GetMonsterCount() == 1 && Bot.GetMonstersExtraZoneCount() == 1 &&                
                Enemy.GetMonsterCount() >= 3 
                )
                return UniqueFaceupSpell();
            if (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() >= 2)
                return UniqueFaceupSpell();
            return false;
        }
        private bool SolemnStrikeeff()
        {
            if (Enemy.HasInSpellZone(CardId.ThatGrassLooksgreener))
                return false;
            /*if (Duel.LastSummonMonster != null && Duel.LastSummonMonster.Id == CardId.Linkuriboh)
                return false;*/
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.Linkuriboh)
                return false;
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.GalaxySoldier && Enemy.Hand.Count >= 3)
                return false;
            return DefaultSolemnStrike();
        }
        //plan a

       
        private bool GlowUpBulbExist()
        {
            if (Bot.HasInHand(CardId.GlowUpBulb))
                return true;
            if (Bot.GetRemainingCount(CardId.GlowUpBulb, 1) > 0)
                return true;
            return false;
        }
        private bool HasInHandTuner()
        {
            if (Bot.HasInHand(CardId.GhostRabbit))
                return true;
            if (Bot.HasInHand(CardId.AshBlossom))
                return true;
            if (Bot.HasInHand(CardId.GlowUpBulb))
                return true;
            if (Bot.HasInHand(CardId.EffectVeiler))
                return true;
            if (Bot.HasInHand(CardId.JetSynchron))
                return true;
            return false;
        }
        private bool HasInHandTuner_B()
        {
            if (Bot.HasInHand(CardId.GhostRabbit))
                return true;
            if (Bot.HasInHand(CardId.AshBlossom))
                return true;
            if (Bot.HasInHand(CardId.GlowUpBulb))
                return true;
            if (Bot.HasInHand(CardId.EffectVeiler))
                return true;
            return false;
        }
        private bool HasRemainTuner()
        {
            if (Bot.GetRemainingCount(CardId.AshBlossom, 2) > 0)
                return true;
            if (Bot.GetRemainingCount(CardId.GlowUpBulb, 1) > 0)
                return true;
            if (Bot.GetRemainingCount(CardId.EffectVeiler, 2) > 0)
                return true;
            if (Bot.GetRemainingCount(CardId.GhostRabbit, 2) > 0)
                return true;
            if (Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0)
                return true;
            return false;
        }
        private bool HasRemainLevel_1_Tuner()
        {            
            if (Bot.GetRemainingCount(CardId.GlowUpBulb, 1) > 0)
                return true;
            if (Bot.GetRemainingCount(CardId.EffectVeiler, 2) > 0)
                return true;            
            if (Bot.GetRemainingCount(CardId.JetSynchron, 1) > 0)
                return true;
            return false;
        }

        private bool HayateSpFirst()
        {
            if (Plan_A || Plan_A_1 || Plan_A_2)
            {
                Logger.DebugWriteLine("***HayateSpFirst");
                HayateSummoned = true;
                return true;
            }
            return false;
        }
        private bool GlowUpBulbsummon()
        {
            if (Plan_A || Plan_A_1|| Plan_A_2)
            {
                if(!Bot.HasInMonstersZone(CardId.BrandishMaidenHayate))
                {
                    Plan_A = false;
                    Plan_A_1 = false;
                    Plan_A_2 = false;
                    return false;
                }
                summon_used = true;
                return true;
            }
            return false;
        }
        private bool TunerSummon()
        {
            if (Plan_A || Plan_A_1 ||Plan_A_2)
            {
                if (!Bot.HasInMonstersZone(CardId.BrandishMaidenHayate))
                {
                    Plan_A = false;
                    Plan_A_1 = false;
                    Plan_A_2 = false;
                    return false;
                }
                summon_used = true;
                return true;
            }
            return false;
        }

        private bool GlowUpBulbeff()
        {
            if (Plan_A || Plan_A_1 || Plan_A_2)
            {
                if (!Bot.HasInMonstersZone(CardId.CrystronNeedlefiber))
                    return false;
                GlowUpBulbeff_used = true;
                return true;
            }           
            return false;
        }

        private bool Linkuribohsp()
        {
            if(!(Bot.HasInMonstersZone(CardId.BrandishMaidenHayate) && Bot.HasInMonstersZone(CardId.HornetBit+1)))
                {
                Plan_A = false;
                Plan_A_1 = false;
                Plan_A_2 = false;
                return false;
            }
            if (Plan_A || Plan_A_1 || Plan_A_2) return true;
            return false;
        }
        private bool Linkuriboheff()
        {
            if (Plan_A || Plan_A_1|| Plan_A_2) return true;
            return false;
        }
        private bool CrystronNeedlefiberSp()
        {
            if (CrystronNeedlefiberSp_used) return false;
            if (!(Plan_A || Plan_A_1)) return false;
            if (Bot.MonsterZone[5] != null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            if (Bot.HasInMonstersZone(CardId.Linkuriboh) && Bot.HasInMonstersZone(CardId.GlowUpBulb))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] { CardId.GlowUpBulb, CardId.Linkuriboh });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Linkuriboh) && Bot.HasInMonstersZone(CardId.AshBlossom))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] { CardId.AshBlossom, CardId.Linkuriboh });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Linkuriboh) && Bot.HasInMonstersZone(CardId.EffectVeiler))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] { CardId.EffectVeiler, CardId.Linkuriboh });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Linkuriboh) && Bot.HasInMonstersZone(CardId.GhostRabbit))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] { CardId.Linkuriboh, CardId.GhostRabbit });
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Linkuriboh) && Bot.HasInMonstersZone(CardId.JetSynchron))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] { CardId.Linkuriboh, CardId.JetSynchron });
                return true;
            }
            return false;
        }

        private bool CrystronNeedlefiberEffect()
        {
            if (!(Plan_A || Plan_A_1 ||Plan_A_2|| Plan_C)) return false;
            AI.SelectCard(new[] { CardId.GlowUpBulb,CardId.EffectVeiler,CardId.GhostRabbit, CardId.AshBlossom });
            return true;
        }

        private bool BirrelswordDragonsp()
        {
            if (!(Plan_A || Plan_A_1 ||Plan_A_2||Plan_C)) return false;
            if (!Bot.HasInMonstersZone(CardId.CrystronNeedlefiber))
                return false;
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if(m.Id==CardId.CrystronNeedlefiber)
                {
                    material_list.Add(m);
                    break;
                }
            }
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.Id == CardId.Linkuriboh || m.HasType(CardType.Tuner))
                {
                    material_list.Add(m);
                    if(material_list.Count==3)
                        break;
                }
            }
            if(material_list.Count == 3)
            {
                AI.SelectMaterials(material_list);
                return true;
            }           
            return false;
        }

        private bool BirrelswordDragoneff()
        {           
            if(ActivateDescription == Util.GetStringId(CardId.BirrelswordDragon, 0))
            {                
                if(Plan_A || Plan_A_1 || Plan_A_2)
                {
                    if(Bot.HasInMonstersZone(CardId.BrandishMaidenRei) && Card.Attacked)
                    {
                        lord_can_do = true;
                        ClientCard target = null;
                        foreach (ClientCard check in Bot.GetMonsters())
                        {
                            if (check.Id == CardId.BrandishMaidenRei)
                                target = check;
                        }
                        AI.SelectCard(CardId.BrandishMaidenRei);
                        return true;
                    }
                    return false;
                }
                if(Util.IsChainTarget(Card) && Util.GetBestEnemyMonster(true,true)!=null)
                {
                    AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                    return true;
                }
                if(Duel.Player==1 && Bot.BattlingMonster==Card)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player == 1 && Bot.BattlingMonster !=null && 
                    (Enemy.BattlingMonster.Attack-Bot.BattlingMonster.Attack)>=Bot.LifePoints)
                {
                    AI.SelectCard(Enemy.BattlingMonster);
                    return true;
                }
                if (Duel.Player==0 && Duel.Phase==DuelPhase.BattleStart)
                {
                    foreach(ClientCard check in Enemy.GetMonsters())
                    {
                        if(check.IsAttack() && !check.HasType(CardType.Link))
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
        //plan a2
        private bool WidowAnchorEffect_A2()
        {
            if (!Plan_A_2) return false;
            if (Util.GetLastChainCard() != null && Util.ChainContainsCard(CardId.BrandishMaidenKagari))
                return false;
            AI.SelectCard(Util.GetBestEnemyMonster(true, true).Id);
            return UniqueFaceupSpell();
        }
        //plan b
        private bool EnemyMonsterSafe_AA1A2()
        {            
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.IsMonsterDangerous())
                    return false;
                if (check.IsMonsterInvincible())
                    return false;                
            } 
            return true;
        }
        private bool EnemyMonsterSafe_B()
        {
            if (Enemy.GetMonsterCount() > 1) return false;            
            int damage = 0;

            foreach(ClientCard check in Enemy.GetMonsters())
            {                
                if (check.IsMonsterDangerous())
                    return false;
                if (check.IsMonsterInvincible())
                    return false;
                if (check.Attack >= 3000)
                    return false;
                if (check.IsFaceup() && check.IsDefense())
                    damage = check.GetDefensePower();
                if (check.IsFacedown() && Enemy.LifePoints > 4600)
                    return false;
            }
            if (damage != 0 && (2200 + 2400 + damage) < Enemy.LifePoints)
                return false;
            if (3000 + 2400 + 2200 < Enemy.LifePoints)
                return false;
            return true;
        }
        private bool EnemyMonsterSafe_B1B2()
        {
            int m_atk = 0;
            if (Enemy.GetMonsterCount() > 1) return false;
            if (Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 8500) return true;
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.IsMonsterDangerous())
                    return false;
                if (check.IsMonsterInvincible())
                    return false;
                if (check.Attack >= 2400)
                    return false;
                if (check.IsFaceup() && check.IsDefense() && check.GetDefensePower() >= 2400)
                    return false;
                if (check.IsDefense() && Enemy.LifePoints > (4600+1500))
                    return false;
                if (check.IsAttack())
                    m_atk = check.GetDefensePower();
            }
            if (m_atk != 0 && (8500 - m_atk) < Enemy.LifePoints)
                return false;
            if (m_atk == 0 && (8500 - 2400) < Enemy.LifePoints)
                return false;            
            return true;            
        }
        private bool EnemyMonsterSafe_B3()
        {           
            if (Enemy.GetMonsterCount() != 1) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsFacedown()) return false;
                if (m.IsShouldNotBeSpellTrapTarget() || m.IsShouldNotBeTarget()) return false;
                if (m.HasType(CardType.Effect) && m.IsAttack() && !m.IsDisabled() && Enemy.LifePoints <=(m.Attack + 8500))
                    return true;
                if (m.HasType(CardType.Effect) && m.IsDefense() && !m.IsDisabled() && Enemy.LifePoints <= 8500)
                    return true;
            }
            return false;
        }
        private bool EnemyMonsterSafe_B5()
        {
            if (Enemy.GetMonsterCount() != 1) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsFacedown()) return false;
                if (m.IsShouldNotBeSpellTrapTarget() || m.IsShouldNotBeTarget()) return false;                
                if (m.HasType(CardType.Effect)&& !m.IsDisabled() && Enemy.LifePoints <= 8500)
                    return true;
            }
            return false;
        }
        private bool EnemyMonsterSafe_C()
        {            
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.IsShouldNotBeTarget() && check.Attack > 2100)
                {
                    
                    Logger.DebugWriteLine("*********dangerous = " + check.Name);
                    return true;
                }
            }
            int atk_m_count = 0;
            foreach (ClientCard check in Enemy.GetMonsters())
            {
                if (check.Id == CardId.UltimateConductorTytanno)
                    return false;
                if (check.IsMonsterDangerous() || check.IsMonsterInvincible())
                    return false;
                if (check.IsAttack())
                    atk_m_count++;
                if (!check.IsDefense() &&  Enemy.GetMonsterCount() <= 1 && 
                    !check.HasType(CardType.Link) && Enemy.LifePoints <= 3000)
                {
                    Plan_C_1 = true;
                    return true;
                }
            }
            
            if (atk_m_count >= 2 && BrandishMonsterRestart())
            {
                Plan_C_1 = true;
                return true;
            }
                
            return false;
        }
            

        private int EnemySetSpellCount()
        {
            int count = 0;
            foreach(ClientCard check in Enemy.GetSpells())
            {
                if (check.IsFacedown())
                    count++;
            }
            return count;
        }
        private bool TunerSummon_B2()
        {
            if (Plan_B_2)
            {
                if (!Bot.HasInMonstersZone(CardId.HornetBit + 1))
                {                    
                    Plan_B_2 = false;
                    return false;
                }
                summon_used = true;
                return true;
            }
            return false;
        }
        private bool TunerSummon_B()
        {
            if (Plan_B || Plan_B_1 || Plan_B_3 || Plan_B_5)
            {
                if(!Bot.HasInMonstersZone(CardId.HornetBit+1))
                {
                    Plan_B = false;
                    Plan_B_1 = false;
                    Plan_B_3  = false;
                    Plan_B_5 = false;

                    return false;
                }                
                summon_used = true;
                return true;
            }
            return false;
        }
        private bool CrystronNeedlefiberSp_B()
        {
            if (CrystronNeedlefiberSp_used) return false;
            if (!(Plan_B || Plan_B_1 || Plan_B_2 || Plan_B_3)) return false;
            
            if (Bot.HasInMonstersZone(new[]{
                CardId.BrandishMaidenHayate,
                CardId.BrandishMaidenKagari,
                CardId.BrandishMaidenShizuku
            }) && Bot.HasInMonstersZone(new[]{
                CardId.EffectVeiler,
                CardId.GhostRabbit,
                CardId.AshBlossom,
                CardId.GlowUpBulb,
                CardId.JetSynchron
                }))
            {
                CrystronNeedlefiberSp_used = true;
                AI.SelectCard(new[] {
                    CardId.BrandishMaidenHayate,
                    CardId.BrandishMaidenKagari,
                    CardId.BrandishMaidenShizuku,
                    CardId.EffectVeiler,
                    CardId.GhostRabbit,
                    CardId.AshBlossom,
                    CardId.GlowUpBulb,
                    CardId.JetSynchron
                });
                if (Bot.MonsterZone[5] != null)
                    AI.SelectPlace(Zones.z5);
                else
                    AI.SelectPlace(Zones.z6);
                CrystronNeedlefiberSp_used = true;
                return true;
            }            
            return false;
        }
        private bool CrystronNeedlefiberSp_A2()
        {
            if (CrystronNeedlefiberSp_used) return false;
            if (!Plan_A_2) return false;
            IList<ClientCard> material_list = new List<ClientCard>();

            if (GetNotBotMonster() != null)
                material_list.Add(GetNotBotMonster());
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.Id == CardId.EffectVeiler ||
                    m.Id == CardId.GhostRabbit ||
                    m.Id == CardId.AshBlossom ||
                    m.Id == CardId.GlowUpBulb ||
                    m.Id == CardId.JetSynchron)
                {
                    material_list.Add(m);
                    if (material_list.Count == 2)
                        break;
                }
            }
            if (Bot.MonsterZone[5] != null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            CrystronNeedlefiberSp_used = true;
            if (material_list.Count != 2)
                return false;
            return true;
        }
        private bool CrystronNeedlefiberSp_B5()
        {
            if (CrystronNeedlefiberSp_used) return false;
            if (!Plan_B_5) return false;
            IList<ClientCard> material_list = new List<ClientCard>();

            if (GetNotBotMonster() != null)           
                material_list.Add(GetNotBotMonster());           
            foreach (ClientCard m in Bot.GetMonsters())
            {                
                if (m.Id == CardId.EffectVeiler ||
                    m.Id == CardId.GhostRabbit ||
                    m.Id == CardId.AshBlossom ||
                    m.Id==CardId.GlowUpBulb ||
                    m.Id == CardId.JetSynchron)
                {
                    material_list.Add(m);
                    if(material_list.Count==2)
                        break;
                }
            }
            if (Bot.MonsterZone[5] != null)
                AI.SelectPlace(Zones.z5);
            else
                AI.SelectPlace(Zones.z6);
            CrystronNeedlefiberSp_used = true;
            if (material_list.Count != 2)
                return false;
            return true;            
        }
        private bool CrystronNeedlefiberEffect_B()
        {
            if (Plan_B || Plan_B_1 || Plan_B_3 || Plan_B_5)
            {
                AI.SelectCard(CardId.JetSynchron);
                return true;
            }
            return false;
        }
        private bool SummonSorceresssp()
        {
            if (!Plan_B && !Plan_B_1 && !Plan_B_2 && !Plan_B_3 && !Plan_B_5) return false;
            if(Bot.HasInMonstersZone(CardId.JetSynchron) &&
                Bot.HasInMonstersZone(CardId.CrystronNeedlefiber))
            {
                AI.SelectCard(new[] { CardId.JetSynchron,CardId.CrystronNeedlefiber });
                if (Bot.MonsterZone[5]!=null && Bot.MonsterZone[5].Id == CardId.CrystronNeedlefiber)
                    AI.SelectPlace(Zones.z5);
                else
                    AI.SelectPlace(Zones.z6);
                return true;
            }
            return false;
        }
        private bool SummonSorceresseff()
        {
            if (ActivateDescription == -1)
                return false;
            return true;
        }

        private bool JetSynchroneff()
        {
            if (Plan_B || Plan_B_1 || Plan_B_2 || Plan_B_3 || Plan_B_5)
            {               
                AI.SelectCard(GetDiscardHand());
                AI.SelectPosition(CardPosition.FaceUpDefence);                
                return true;
            }
            return false;
        }

        private bool HiSpeedroidChanbarasp()
        {
            if (!(Plan_B || Plan_B_1 || Plan_B_2 || Plan_B_3 || Plan_B_5)) return false;
            if(Bot.HasInMonstersZone(CardId.BrandishMaidenRei) && 
                Bot.HasInMonstersZone(new[] { CardId.JetSynchron,CardId.EffectVeiler,CardId.GlowUpBulb}))
            {
                IList<ClientCard> material_list = new List<ClientCard>();
                foreach (ClientCard m in Bot.GetMonsters())
                {
                    if (m.Id == CardId.BrandishMaidenRei || m.Id==CardId.JetSynchron ||
                         m.Id == CardId.EffectVeiler || m.Id == CardId.GlowUpBulb)
                    {
                        material_list.Add(m);
                        if (material_list.Count == 2)
                            break;
                    }
                    
                }
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        private bool Linkuribohsp_B()
        {           
            if (Plan_B) return true;
            return false;
        }
        private bool Linkuriboheff_B()
        {
            if (Plan_B) return true;
            return false;
        }

        private bool HayateSp_B()
        {
            if((Plan_B_1 || Plan_B_3 || Plan_B_5) && Bot.HasInMonstersZone(CardId.HornetBit+1))
            {
                HayateSummoned = true;
                AI.SelectCard(CardId.HornetBit);
                return true;
            }
            return false;
        }

        private bool ShizukuSp_B()
        {
            
            if ((Plan_B_1 || Plan_B_3 || Plan_B_5) && Bot.HasInMonstersZone(CardId.HornetBit+1))
            {
                ShizukuSummoned = true;
                AI.SelectCard(CardId.HornetBit);
                return true;
            }
            return false;
        }
        private bool TopologicBomberDragonsp()
        {
            if(Plan_B && Bot.HasInMonstersZone(CardId.SummonSorceress) && Bot.HasInMonstersZone(CardId.Linkuriboh))
            {
                AI.SelectCard(new[] { CardId.SummonSorceress, CardId.Linkuriboh });
                return true;
            }
            if (Plan_B_1 && Bot.HasInMonstersZone(CardId.SummonSorceress) && Bot.HasInMonstersZone(CardId.HiSpeedroidChanbara))
            {
                AI.SelectCard(new[] { CardId.SummonSorceress, CardId.HiSpeedroidChanbara });
                return true;
            }
            return false;
        }
        // plan b2 
        private bool CrystronNeedlefiberEffect_B2()
        {
            if (Plan_B_2)
            {
                if (Bot.MonsterZone[5] != null)
                    AI.SelectPlace(Zones.z3);
                else
                    AI.SelectPlace(Zones.z1);
                AI.SelectCard(new[] { CardId.EffectVeiler, CardId.GlowUpBulb});
                return true;
            }
            return false;
        }
        private bool HiSpeedroidChanbarasp_B2()
        {
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenRei) && Bot.HasInMonstersZone(new[] { CardId.EffectVeiler, CardId.GlowUpBulb }))
            {
                AI.SelectCard(new[] { CardId.BrandishMaidenRei});
                return true;
            }
            return false;
        }
        private bool HayateSp_B2()
        {
            if (Plan_B_2 && Bot.HasInMonstersZone(CardId.HornetBit+1))
            {
                HayateSummoned = true;
                AI.SelectCard(CardId.HornetBit);
                return true;
            }
            return false;
        }

        private bool ShizukuSp_B2()
        {

            if (Plan_B_2 && Bot.HasInMonstersZone(CardId.HornetBit+1))
            {
                ShizukuSummoned = true;
                AI.SelectCard(CardId.HornetBit);
                return true;
            }
            return false;
        }

        //plan b 3
        private bool WidowAnchorEffect_B3B5()
        {
            if (!(Plan_B_3 || Plan_B_5)) return false;
            if (Util.GetLastChainCard() != null && Util.ChainContainsCard(CardId.BrandishMaidenKagari))
                return false;
            AI.SelectCard(Util.GetBestEnemyMonster(true, true).Id);            
            return UniqueFaceupSpell();
        }
       
        //plan c
        private bool GlowUpBulbsummon_C()
        {
            if (Plan_C)
            {
                summon_used = true;
                return true;
            }
            return false;
        }
        private bool TunerSummon_C()
        {
            if (Plan_C)
            {
                summon_used = true;
                return true;
            }
            return false;
        }

        private bool GlowUpBulbeff_C()
        {
            if (Plan_C)
            {
                GlowUpBulbeff_used = true;
                return true;
            }
            return false;
        }

        private bool Linkuribohsp_C()
        {
            if(Plan_C && !GlowUpBulbeff_used && Bot.HasInMonstersZone(CardId.GlowUpBulb))
            {
                AI.SelectCard(CardId.GlowUpBulb);
                return true;
            }
            return false;
        }
        
        private bool CrystronNeedlefiberSp_C()
        {           
            if (CrystronNeedlefiberSp_used) return false;
            if (!Plan_C) return false;
            if (Bot.GetMonsterCount() == 2)
            {
                CrystronNeedlefiberSp_used = true;
                return true;
            }
            return false;
        }
        
        //************************************************************************
                
        private bool GhostRabbitsummon()
        {
            if (Util.GetLastChainCard() != null &&
                Util.GetLastChainCard().Id == CardId.AreaZero &&
                !BrandishMonsterExist() && !BrandishMonsterRestart() && Bot.GetMonsterCount() == 0)
                return true;
            return false;
        }        
       
        private bool InfiniteImpermanenceeff()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsDisabled())
                return false;
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.GalaxySoldier && Enemy.Hand.Count >= 3) return false;
            if (Util.ChainContainsCard(CardId.WidowAnchor))
                return false;
            return DefaultInfiniteImpermanence();
        }

        private bool WidowAnchorEffectWin()
        {

            if (SpellsCountInGrave() < 3)
                return false;
            if (Duel.Phase != DuelPhase.BattleStart || Duel.Player != 0) return false;
            int total_atk = 0;
            int total_damage = 0;
            foreach(ClientCard check in Bot.GetMonsters())
            {
                if (check.IsAttack())
                    total_damage += check.Attack;
            }
            foreach(ClientCard check in Enemy.GetMonsters())
            {
                if (check.IsDefense() || check.IsMonsterDangerous() || check.IsMonsterInvincible())
                    return false;
                total_atk += check.Attack;
            }
            ClientCard m = Util.GetBestEnemyMonster(true, true);
            if(m!=null && m.HasType(CardType.Effect) && (total_damage-total_atk+m.Attack)>=Enemy.LifePoints)
            {
                AI.SelectCard(m.Id);
                Logger.DebugWriteLine("*****WidowAnchorEffectWin check");
                return UniqueFaceupSpell();              
            }
            return false;  
        }
        private bool WidowAnchorEffectFirst()
        {
            int count = 0;
            foreach(ClientCard check in Bot.GetSpells())
            {
                if (check.Id == CardId.WidowAnchor)
                    count++;
            }
            if (count >= 2)
                return WidowAnchorEffect();
            return false;
        }
        private bool WidowAnchorEffect()
        {
            Widow_control = true;
            if(DefaultOnBecomeTarget() && Util.GetBestEnemyMonster(true,true)!=null)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                return true;
            }
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsDisabled())
                return false;
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.GalaxySoldier && Enemy.Hand.Count >= 3) return false;
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.InfiniteImpermanence)
                return false;
            if (Duel.LastChainPlayer == 0) return false;
            if(Duel.Player==0 && Enemy.HasInMonstersZone(CardId.ElShaddollWinda,true))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.ElShaddollWinda);
                return UniqueFaceupSpell();
            }
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.Main1 &&
                NegateCount() == 1 && Enemy.HasInMonstersZone(CardId.UltimateConductorTytanno, true))
                return false;
            if(Bot.BattlingMonster!=null && Enemy.BattlingMonster!=null)
            {
                if (Enemy.BattlingMonster.IsDisabled()) return false;
                
                if (Enemy.BattlingMonster.Id == CardId.EaterOfMillions ||
                    Enemy.BattlingMonster.Id == CardId.GrenMajuDaEizo)
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    Widow_control = false;
                    AI.SelectCard(Enemy.BattlingMonster);
                    return UniqueFaceupSpell();
                }
                if(Duel.Player==1 && Enemy.BattlingMonster.Id== _CardId.NumberS39UtopiaTheLightning)
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(Enemy.BattlingMonster);
                    return UniqueFaceupSpell();
                }
                if (Duel.Player == 1 && Enemy.BattlingMonster.Id == _CardId.UltimateConductorTytanno)
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(Enemy.BattlingMonster);
                    return UniqueFaceupSpell();
                }
                if (Enemy.BattlingMonster.IsMonsterDangerous() && SpellsCountInGrave() >= 3)
                {
                    {
                        //AI.SelectPlace(Zones.z2, 2);
                        AI.SelectCard(Enemy.BattlingMonster);
                        return UniqueFaceupSpell();
                    }
                }
            }            
            int total_atk = 0;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsAttack())
                    total_atk += m.Attack;
            }
            if (Duel.Player == 1 && Duel.Phase==DuelPhase.BattleStart &&
                Bot.BattlingMonster!=null && Enemy.BattlingMonster != null &&
                Bot.BattlingMonster.IsAttack() && Bot.HasInMonstersZone(Bot.BattlingMonster.Id) &&
                SpellsCountInGrave()>=3 && 
                (Enemy.BattlingMonster.Attack-Bot.BattlingMonster.Attack)>Bot.LifePoints)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(Enemy.BattlingMonster);
                return UniqueFaceupSpell();
            }
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart &&
                Bot.BattlingMonster == null && Enemy.BattlingMonster != null &&                
                SpellsCountInGrave() >= 3 &&
               Enemy.BattlingMonster.Attack > Bot.LifePoints)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(Util.GetBestEnemyMonster(true,true));
                return UniqueFaceupSpell();
            }

            ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
            if (target != null)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(target);
                return UniqueFaceupSpell();
            }
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if (Util.GetLastChainCard() == check)
                    {
                        target = check;
                        break;
                    }
                }
                if (target != null)
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(target);
                    return UniqueFaceupSpell();
                }
            }
            return false;
        }

        private bool Sharkcannoneff()
        {
            if(Duel.ChainTargetOnly[0]!=null && Duel.LastChainPlayer==1 && 
                Duel.ChainTargetOnly[0].Location==CardLocation.Grave)
            {
                AI.SelectCard(Duel.ChainTargetOnly[0]);
                AI.SelectYesNo(false);
                return true;
            }
            return false;
        }
        private bool Typhoonsummon()
        {           
            if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon))
                return false;
            if (Duel.Phase !=DuelPhase.Main1 && Duel.Phase!=DuelPhase.Main2) return false;
            if(Bot.HasInSpellZone(CardId.AreaZero) &&
                !Bot.HasInSpellZone(CardId.MultiRoll) && 
                !BrandishMonsterExist() &&
                Bot.GetMonstersExtraZoneCount()==0 &&
                Duel.Player==0 &&
                Duel.Phase>=DuelPhase.Main1 &&
                Util.ChainCountPlayer(0) == 0 &&
                Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0 &&
                !(Bot.HasInHand(CardId.BrandishMaidenRei) && !summon_used))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.AreaZero);
                return true;
            }
            return false;
        }
        private bool Typhooneff()
        {
            if (enemy_no_used_spell)
            {
                if (Bot.HasInSpellZone(CardId.HerculesBase))
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(CardId.HerculesBase);
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(CardId.MetalfoesFusion);
                    return true;
                }
            }
            return false;
        }
        private bool Typhooneffcounter()
        {
            
            if (Util.ChainContainsCard(CardId.TwinTwisters)) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            if(Bot.HasInSpellZone(CardId.HerculesBase) && !Bot.HasInSpellZone(CardId.AreaZero)
                && !Bot.HasInSpellZone(CardId.MultiRoll))
            {
                AI.SelectCard(CardId.HerculesBase);
                return UniqueFaceupSpell();
            }
            int count = 0;
            foreach (ClientCard check in Enemy.GetSpells())
            {
                if (check.Type == 16777218)
                    count++;
            }
            if(count==2)
            {
                targets.Add(Util.GetPZone(1, 1));
                AI.SelectCard(targets);
                return UniqueFaceupSpell();
            }             
            
            foreach (ClientCard check in Enemy.GetSpells())
            {                
                if (check.HasType(CardType.Continuous) || check.HasType(CardType.Field) && check.Id!=CardId.AreaZero)
                {
                    targets.Add(check);
                    AI.SelectCard(check);
                    return UniqueFaceupSpell();
                }
            }           
            foreach (ClientCard check in Enemy.GetSpells())
            {
                targets.Add(check);
            }
            if(DefaultOnBecomeTarget() && targets.Count>0)
            {
                AI.SelectCard(targets);
                return true;
            }
            return false;
        }
        private bool TwinTwistersEffect()
        {
            int count=0;
            if (Util.ChainContainsCard(CardId.Typhoon) ||
                Util.ChainContainsCard(CardId.TwinTwisters) ||
                Util.ChainContainsCard(CardId.BrandishSkillJammingWave) ||
                Util.ChainContainsCard(CardId.BrandishSkillAfterburner))
                return false;
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard check in Enemy.GetSpells())
            {
                if (check.HasType(CardType.Continuous) || check.HasType(CardType.Field))
                {
                    targets.Add(check);
                    count++;
                } 
            }
            if(count>=2)
            {
                AI.SelectCard(GetDiscardHand());
                AI.SelectNextCard(targets);
                //AI.SelectPlace(Zones.z2, 2);
                return true;
            }
            foreach (ClientCard check in Enemy.GetSpells())
            {
                if (!check.HasType(CardType.Continuous) && !check.HasType(CardType.Field))
                    targets.Add(check);
            }
            if (DefaultOnBecomeTarget() && targets.Count>=2)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(GetDiscardHand());
                AI.SelectNextCard(targets);
                return true;
            }
            if (Util.GetLastChainCard() != null &&
                (Util.GetLastChainCard().HasType(CardType.Continuous) ||
                Util.GetLastChainCard().HasType(CardType.Field)) &&
                Duel.LastChainPlayer == 1)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(GetDiscardHand());
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        private bool FoolishBurialGoodsEffect()
        {
            if(!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.BrandishStartUpEngage) &&
                (Bot.HasInHandOrHasInMonstersZone(CardId.BrandishMaidenRei) || 
                Bot.HasInHandOrInSpellZone(CardId.HornetBit)))
                {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.BrandishStartUpEngage);
                return true;
            }
            if (Bot.HasInSpellZoneOrInGraveyard(CardId.MetalfoesFusion))
                return false;
            AI.SelectCard(new[]{
                CardId.MetalfoesFusion,               
            });
            //AI.SelectPlace(Zones.z2, 2);
            return true;
        }


        private bool TheMelodyOfAwakeningDragoneff()
        {
            if(Bot.HasInHand(CardId.LordOfTheLair) && Bot.GetRemainingCount(CardId.LordOfTheLair,3)>=2)
            {
                AI.SelectCard(CardId.LordOfTheLair);
                AI.SelectNextCard(new[] { CardId.LordOfTheLair, CardId.LordOfTheLair});
                return true;
            }
            int count = 0;
            foreach(ClientCard card in Bot.Hand )
            {
                if (card.Id == CardId.TheMelodyOfAwakeningDragon)
                    count++;
            }
            if(count>=2 && Bot.GetRemainingCount(CardId.LordOfTheLair,3)>0)
            {
                AI.SelectCard(CardId.TheMelodyOfAwakeningDragon);
                AI.SelectNextCard(new[] { CardId.LordOfTheLair, CardId.LordOfTheLair });
                return true;
            }                
                    
            if(!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LordOfTheLair) && 
                Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.BrandishMaidenRei)
                && Bot.GetRemainingCount(CardId.LordOfTheLair, 3) > 0
                )
            {
                AI.SelectCard(GetDiscardHand());
                AI.SelectNextCard(new[] { CardId.LordOfTheLair, CardId.LordOfTheLair });
                return true;
            }
            return false;            
        }
        private bool Plan_A_Clean()
        {
            int m_count = 0;
            foreach(ClientCard check in Enemy.GetMonsters())
            {
                if (check.IsAttack() && check.Attack >= 1500)
                    m_count++;
            }
            if (m_count >= 2) return true;
            return false;
        }
        private bool BrandishSkillAfterburnerEffect_A()
        {
            if (Plan_A && Plan_A_Clean())
                return BrandishSkillAfterburnerEffect();
            return false;
        }
        private bool BrandishSkillJammingWaveEffect_A()
        {
            if (Plan_A && Plan_A_Clean())
                return BrandishSkillJammingWaveEffect();
            return false;
        }
        private bool BrandishSkillJammingWaveEffFirst()
        {
            if (!GetPlan())
                return BrandishSkillJammingWaveEffect();
            return false;
        }
        private bool BrandishSkillAfterburnerEffect()
        {
            if (Enemy.HasInMonstersZone(CardId.ElShaddollWinda, true))
                return false;
            if(Enemy.HasInMonstersZone(CardId.Linkuriboh) && GetPlan())
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.Linkuriboh);
                return true;
            }
            if (Plan_C_1 && Enemy.GetMonsterCount() == 2 && 
                Duel.Phase==DuelPhase.Main1 &&
                Bot.HasInMonstersZone(CardId.BirrelswordDragon)) return false;
            ClientCard target = Util.GetProblematicEnemyMonster(0, true);
                if (target != null)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(target);
                return true;
            }

            if(SpellsCountInGrave()>=3 && Enemy.GetSpellCount()>=1 && Enemy.GetMonsterCount()>=1 &&
                 Util.GetBestEnemyMonster(true, true)!=null)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(Util.GetBestEnemyMonster(true, true));
                return true;
            }

            target = Util.GetBestEnemyMonster(true, true);
            if (target != null)
            {
                if (Enemy.GetMonsterCount() >= 2 && target.Attack>=1600)
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(target);
                    return true;
                }
                if (target.Attack<=2000)
                    return false;
                {
                    //AI.SelectPlace(Zones.z2, 2);
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        private bool BrandishSkillJammingWavesummon()
        {
            foreach(ClientCard check in Bot.GetSpells())
            {
                if(check.Id==CardId.AreaZero && check.IsFacedown() &&
                    Bot.GetMonsterCount()==0 && !BrandishMonsterRestart() &&
                    Bot.GetRemainingCount(CardId.BrandishMaidenRei,3)>0)
                {
                    Logger.DebugWriteLine("BrandishSkillJammingWavesummon");
                    AI.SelectCard(CardId.AreaZero);
                    return true;
                }
            }
            return false;
        }
        private bool BrandishSkillJammingWaveEffect()
        {
            if (Plan_C_1 && Enemy.GetMonsterCount() == 2 &&
                Duel.Phase == DuelPhase.Main1 &&
                Bot.HasInMonstersZone(CardId.BirrelswordDragon)) return false;
            if (SpellsCountInGrave()<3 || Enemy.GetMonsterCount()==0)
                return false;
            ClientCard target = null;

            if (Bot.HasInSpellZone(CardId.HerculesBase))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.HerculesBase);
                return true;
            }

            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card.IsFacedown())
                {
                    target = card;
                    break;
                }
            }
            if (target != null)
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(target);
                return true;
            }
            
            if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.MetalfoesFusion);
                return true;
            }
            if(Bot.LifePoints<=1500 && Bot.HasInSpellZone(CardId.SolemnStrike))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.SolemnStrike);
                return true;
            }
            if(enemy_no_used_spell && Bot.HasInSpellZone(CardId.Typhoon))
            {
                //AI.SelectPlace(Zones.z2, 2);
                AI.SelectCard(CardId.Typhoon);
                return true;
            }
            return false;
        }
        
        private bool BrandishStartUpEngageEffect()
        {
            if(Bot.GetSpellCountWithoutField()==5)
            {
                int target1 = EngageGetCardToSearch();
                if (target1 > 0)
                    AI.SelectCard(target1);
                else
                    AI.SelectCard(new[] {
                    CardId.HornetBit,
                    CardId.BrandishSkillAfterburner,
                    CardId.WidowAnchor,
                    CardId.BrandishSkillJammingWave,
                    CardId.HerculesBase,
                    CardId.AreaZero,
                    CardId.MultiRoll,
                    CardId.BrandishMaidenRei
                });
                return true;
            }
            if (Lockbird_used) return false;
            if (Bot.Hand.Count >= 7) return false;
            int target = EngageGetCardToSearch();
            if (target > 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(new[] {
                    CardId.HornetBit,
                    CardId.BrandishSkillAfterburner,
                    CardId.WidowAnchor,
                    CardId.BrandishSkillJammingWave,
                    CardId.HerculesBase,
                    CardId.AreaZero,
                    CardId.MultiRoll,
                    CardId.BrandishMaidenRei
                });
            //AI.SelectPlace(Zones.z2, 2);
            return true;
        }
        private bool HornetBitEffectFirst_B()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Bot.GetMonstersExtraZoneCount() == 0) return false;
            if (Plan_B || Plan_B_1 || Plan_B_2)
            {
                if (Util.GetLastChainCard() != null && 
                    (Util.ChainContainsCard(CardId.AreaZero)|| Util.ChainContainsCard(CardId.BrandishMaidenKagari)))
                    return false;
                if (Bot.MonsterZone[5] != null)
                    AI.SelectPlace(Zones.z2);
                return UniqueFaceupSpell();
            }
            return false;
        }
        private bool HornetBitEffectFirst_B3B5()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Plan_B_3 || Plan_B_5)
            {
                if (Util.GetLastChainCard() != null &&
                    (Util.ChainContainsCard(CardId.AreaZero) || Util.ChainContainsCard(CardId.BrandishMaidenKagari)))
                    return false;
                if (Bot.MonsterZone[5] != null)
                    AI.SelectPlace(Zones.z2);
                return UniqueFaceupSpell();
            }
            return false;
        }
        private bool HornetBitEffectFirst()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;
            if ((Plan_A ||Plan_A_2) && Duel.Phase==DuelPhase.Main1)
            {                
                if(!Bot.HasInMonstersZone(CardId.BrandishMaidenHayate))
                {                   
                    return false;
                }
                if (Util.GetLastChainCard() != null && Util.ChainContainsCard(CardId.AreaZero))
                    return false;
                if (Bot.MonsterZone[5] != null)
                    AI.SelectPlace(Zones.z0);
                else
                    AI.SelectPlace(Zones.z2);
                return UniqueFaceupSpell();
            }                
            return false;
        }
        private bool HornetBitEffect()
        {            
            if (Util.GetLastChainCard() != null && Util.ChainContainsCard(CardId.AreaZero))
                return false;
            if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon))
                return false;
            if (DefaultOnBecomeTarget() && Bot.GetMonstersExtraZoneCount()==0)
            {
                //AI.SelectPlace(Zones.z2, 2);
                Logger.DebugWriteLine("OnBecomeTargetHornetBitEffect");
                return true;
            }               
            if (Duel.Phase < DuelPhase.Main1) return false;
            if (Duel.CurrentChain.Count > 0) return false;
            if (Duel.LastChainPlayer == 0 || Duel.Player==1 ||               
                Util.ChainContainsCard(CardId.BrandishMaidenRei)) return false;
            if (Bot.GetMonstersExtraZoneCount() >= 1) return false;
            if (!BrandishMonsterExist())
            {                
                if ((Bot.HasInHand(CardId.BrandishMaidenRei)||Bot.HasInHand(CardId.BrandishStartUpEngage)) && !summon_used)
                    return false;
                Logger.DebugWriteLine("HornetBitEffect");
                return UniqueFaceupSpell();
            }                
            return false;
        }
        

        private bool HerculesBaseEffect()
        {    
            if(Card.Location==CardLocation.SpellZone)
            {                
                if (SpellsCountInGrave() < 3)
                    return false;
                if (!Bot.HasInSpellZone(CardId.MultiRoll) ||MultiRoll_used)
                    return false;
                if (GetPlan())
                    return false;
                if(Duel.Phase==DuelPhase.Main1 && Bot.HasInMonstersZone(CardId.BrandishMaidenKagari) &&
                    HayateSummoned && Util.GetWorstEnemyMonster()!=null &&
                    Util.GetWorstEnemyMonster().GetDefensePower()<Util.GetBestBotMonster().Attack)
                {
                    Logger.DebugWriteLine("***********base_draw_first");
                    base_draw_first = true;
                    AI.SelectCard(CardId.BrandishMaidenKagari);
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                base_draw_first = false;                
                AI.SelectCard(basetargets);              
                return true;
            }  
            return false;
        }

        private ClientCard GetNotBotMonster()
        {
            foreach(ClientCard check in Bot.GetMonstersInMainZone())
            {
                if ((check.Id != CardId.LordOfTheLair &&
                    check.Id != CardId.BrandishMaidenRei &&
                    check.Id != CardId.HornetBit+1 &&
                    check.Id!=CardId.HiSpeedroidChanbara &&
                    check.Id!=CardId.JetSynchron &&
                    check.Id!=CardId.GlowUpBulb &&
                    check.Id != CardId.EffectVeiler &&
                    check.Id != CardId.AshBlossom &&
                    check.Id != CardId.GhostRabbit
                   )
                    && !check.HasType(CardType.Link))
                    return check;
            }
            return null;
        }
        private bool NotBotField()
        {           
            foreach(ClientCard check in Bot.GetSpells())
            {
                if(check.HasType(CardType.Field) && check.Id!=CardId.AreaZero &&
                    Bot.HasInHand(CardId.AreaZero) && check.IsFacedown() && Card==check)
                {
                    return true;
                }
            }
            return false;
        }
        private bool AreaZeroSetFirst()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!Bot.HasInSpellZone(CardId.AreaZero))
                    return true;
                return false;
            }
            return false;
        }

        private bool AreaZeroEffect()
        {            
            if (Card.Location == CardLocation.Grave &&(!Bot.HasInMonstersZone(CardId.BrandishMaidenRei) || Bot.GetMonsterCount()==0))
                return true;
            if(Card.Location==CardLocation.SpellZone)
            {                
                if (Bot.GetMonsterCount() == 0 && !BrandishMonsterRestart() &&
                    !Bot.HasInHandOrInSpellZone(CardId.MultiRoll) &&
                    !Bot.HasInHandOrInSpellZone(CardId.Typhoon) &&
                    !BrandishMonsterExist() && Bot.GetFieldCount() >= 2 && Bot.HasInHand(CardId.GhostRabbit))
                {
                    Logger.DebugWriteLine("****rabbitsummon");
                    AreaZero_used = true;                   
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.HerculesBase))
                {
                    AreaZero_used = true;
                    AI.SelectCard(CardId.HerculesBase);                   
                    return true;
                }
                ClientCard enemycard = GetNotBotMonster();
                if(enemycard!=null)
                {
                    if (GetPlan() && Duel.Phase != DuelPhase.Main2)
                        return false;
                    AreaZero_used = true;
                    AI.SelectCard(enemycard);                  
                    return true;
                }
                foreach(ClientCard check in Bot.GetMonsters())
                {
                    if((check.HasType(CardType.Tuner)||check.IsFacedown()) 
                        && Duel.Phase==DuelPhase.Main2)
                    {
                        AreaZero_used = true;
                        AI.SelectCard(check.Id);
                        return true;
                    }
                }
                if (Bot.HasInMonstersZone(CardId.BrandishMaidenRei, true) && Bot.GetMonstersExtraZoneCount() == 0)
                {
                    if (Rei_eff_used) return false;
                    AreaZero_used = true;
                    AI.SelectCard(CardId.BrandishMaidenRei);
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
                {
                    AreaZero_used = true;
                    AI.SelectCard(CardId.MetalfoesFusion);                   
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.TheMelodyOfAwakeningDragon))
                {
                    AreaZero_used = true;
                    AI.SelectCard(CardId.TheMelodyOfAwakeningDragon);                    
                    return true;
                }
                if(Bot.HasInSpellZone(CardId.SolemnStrike) && Bot.LifePoints<=1500)
                {
                    AreaZero_used = true;
                    AI.SelectCard(CardId.SolemnStrike);
                    return true;
                }
            }
            return false;
        }

        private bool MultiRollEffectFirst()
        {
            if(Card.Location==CardLocation.Hand)
            {
                if (!Bot.HasInSpellZone(CardId.MultiRoll))
                {
                   // AI.SelectPlace(Zones.z2, 2);
                    return true;
                }
                   
            }
            return false;
        }

        private bool MultiRollsummon()
        {
            return MultiRollEffect();
        }

        private bool MultiRollEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.Phase == DuelPhase.End)
                {
                    if (Duel.Player == 0)
                    {
                        int Engage_count = 0;
                        foreach(ClientCard target in Bot.GetGraveyardSpells())
                        {
                            if (target.Id == CardId.BrandishStartUpEngage)
                                Engage_count++;
                        }
                        if(Engage_count==1 && Bot.GetRemainingCount(CardId.BrandishStartUpEngage,3)>0 &&
                            Bot.HasInMonstersZone(CardId.BrandishMaidenShizuku))
                        {
                            AI.SelectCard(CardId.BrandishStartUpEngage);
                                return true;
                        }
                        foreach (ClientCard target in Bot.GetGraveyardSpells())
                        {
                            if (target.Id == CardId.WidowAnchor && !Bot.HasInSpellZone(CardId.WidowAnchor))
                            {
                                AI.SelectCard(target);
                                return true;
                            }
                        }
                        foreach (ClientCard target in Bot.GetGraveyardSpells())
                        {
                            if (target.Id == CardId.AreaZero && !Bot.HasInSpellZone(CardId.AreaZero))
                            {
                                AI.SelectCard(target);
                                return true;
                            }
                        }
                    }                    
                    return false;
                }
                else
                {
                    if (base_draw_first && Duel.Phase == DuelPhase.Main1)
                        return false;
                    if(Bot.HasInSpellZone(CardId.HerculesBase))
                    {
                        MultiRoll_used = true;
                        AI.SelectCard(CardId.HerculesBase);
                        return true;
                    }
                    if (Bot.HasInSpellZone(CardId.AreaZero) &&
                        !Bot.HasInMonstersZone(CardId.TopologicBomberDragon) &&
                        Bot.GetMonstersExtraZoneCount() == 0 &&
                        !BrandishMonsterExist() &&
                        Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0 &&
                        !(Bot.HasInHand(CardId.BrandishMaidenRei) && !summon_used))
                    {
                        MultiRoll_used = true;
                        AI.SelectCard(CardId.AreaZero);
                        return true;
                    }
                    ClientCard enemycard = GetNotBotMonster();
                    if (enemycard != null)
                    {
                        if (GetPlan() && Duel.Phase != DuelPhase.Main2)
                            return false;
                        MultiRoll_used = true;
                        AI.SelectCard(enemycard);
                        return true;
                    }
                    foreach (ClientCard check in Bot.GetMonsters())
                    {
                        if ((check.HasType(CardType.Tuner) ||check.IsFacedown()) && Duel.Phase == DuelPhase.Main2)
                        {
                            AreaZero_used = true;
                            AI.SelectCard(check.Id);
                            return true;
                        }
                    }
                    if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
                    {
                        MultiRoll_used = true;
                        AI.SelectCard(CardId.MetalfoesFusion);
                        return true;
                    }
                }                
            }
            return false;
        }

        private bool BrandishMaidenReiSummon()
        {
            if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon))
                return false;
            if (Bot.GetMonsterCount()==0)
            {
                summon_used = true;
                return true;
            }
            return false;
        }

        private bool BrandishMaidenReiEffect()
        {            
            if (Card.Location == CardLocation.Grave)
            {
                Rei_grave_used = true;
                return true;
            }
            if (Card.IsDisabled())
            {
                return false;
            }
            if (Util.IsChainTarget(Card) || 
                Util.ChainContainsCard(CardId.DarkHole) ||
                Util.ChainContainsCard(_CardId.UltimateConductorTytanno))
            {
                //AI.SelectPlace(Zones.z5, 3);
                Rei_eff_used = true;
                BrandishMaidenReiSelectTarget();
                return true;
            }
            if (Duel.LastChainPlayer == 0) return false;
            if(Duel.Player==0 && Duel.Phase==DuelPhase.Main1)
            {
                if (Bot.HasInHandOrInGraveyard(CardId.LordOfTheLair))
                {
                    //AI.SelectPlace(Zones.z5, 3);
                    Rei_eff_used = true;
                    BrandishMaidenReiSelectTarget();
                    return true;
                }
            }
            bool change = false;
            if (Util.GetWorstEnemyMonster(true) != null && Util.GetWorstEnemyMonster(true).Attack >= 1500)
                change = true;
            if ((Card.Attacked || change)&& Duel.Phase == DuelPhase.BattleStart)
            {
                //AI.SelectPlace(Zones.z5, 3);
                Rei_eff_used = true;
                BrandishMaidenReiSelectTarget();
                return true;
            }
            if (Card == Bot.BattlingMonster && Duel.Player == 1)
            {
                if (!KagariSummoned && Bot.GetRemainingCount(CardId.BrandishMaidenKagari, 3) >= 1 &&
                  Bot.HasInGraveyard(new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor,
                CardId.AreaZero,
                CardId.HerculesBase,
                CardId.MultiRoll,
                CardId.BrandishSkillAfterburner,
                CardId.BrandishSkillJammingWave
              }))
                {                    
                    KagariSummoned = true;
                    AI.SelectCard(CardId.BrandishMaidenKagari);
                }
                else if (!HayateSummoned && Bot.GetRemainingCount(CardId.BrandishMaidenHayate, 2) >= 1
                    && Duel.Turn > 1)
                {
                    HayateSummoned = true;
                    AI.SelectCard(new[] {
                    CardId.BrandishMaidenHayate
                });
                }
                //AI.SelectPlace(Zones.z5, 3);
                Rei_eff_used = true;
                return true;
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                //AI.SelectPlace(Zones.z5, 3);
                Rei_eff_used = true;
                BrandishMaidenReiSelectTarget();
                return true;
            }
            return false;
        }

        private void BrandishMaidenReiSelectTarget()
        {
            if (!KagariSummoned && Bot.GetRemainingCount(CardId.BrandishMaidenKagari,3)>=1 &&
                Bot.HasInGraveyard(new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor,
                CardId.AreaZero,
                CardId.HerculesBase,
                CardId.MultiRoll,
                CardId.BrandishSkillAfterburner,
                CardId.BrandishSkillJammingWave
            }))
            {
                KagariSummoned = true;
                AI.SelectCard(CardId.BrandishMaidenKagari);
            }
            else if(!HayateSummoned && Bot.GetRemainingCount(CardId.BrandishMaidenHayate,2)>=1 
                && Duel.Turn>1 && Duel.Player==0)
            {
                HayateSummoned = true;
                AI.SelectCard(new[] {                   
                    CardId.BrandishMaidenHayate
                });
            }
            else
            {
                ShizukuSummoned = true;
                AI.SelectCard(new[] {
                    CardId.BrandishMaidenShizuku
                });
            }
        }

        private bool KagariSp()
        {
            if (Plan_A || Plan_A_2)
            {
                if (Duel.Phase < DuelPhase.Main2)
                    return false;
            }
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenHayate, true)
                && Enemy.LifePoints <= 1500 && Duel.Phase == DuelPhase.Main1)
                return false;
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenRei) &&
                summon_used && !Rei_eff_used && Duel.Phase == DuelPhase.Main1 &&
                Enemy.GetMonsterCount() == 0 && Duel.Turn>1)
                return false;
            if (Bot.HasInGraveyard(new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor,
                CardId.AreaZero,
                CardId.HerculesBase,
                CardId.MultiRoll,
                CardId.BrandishSkillAfterburner,
                CardId.BrandishSkillJammingWave
            }))
            {
                //AI.SelectPlace(Zones.z5, 3);
                KagariSummoned = true;
                return true;
            }
            return false;
        }

        private bool KagariEffect()
        { 
            if(BaseGetFirst() && Bot.HasInGraveyard(CardId.HerculesBase))
            {
                AI.SelectCard(CardId.HerculesBase);
                return true;
            }

            if(Bot.HasInGraveyard(CardId.BrandishSkillJammingWave) && 
                SpellsCountInGrave()>=3 && Enemy.GetMonsterCount()>=1 &&
                Enemy.GetSpellCount()>=1)
            {
                AI.SelectCard(CardId.BrandishSkillJammingWave);
                return true;
            }
            int m_count = 0;
            int m_faceup_count = 0;
            foreach(ClientCard check in Enemy.GetMonsters())
            {

                if (check.Attack > 2000 || check.IsFloodgate() || check.IsMonsterDangerous() || check.IsMonsterInvincible() &&
                    !(check.IsShouldNotBeSpellTrapTarget() || check.IsShouldNotBeTarget()))
                    m_count++;
            }
            if (Util.GetProblematicEnemyMonster(2000,true) != null && Bot.HasInGraveyard(CardId.BrandishSkillAfterburner))
            {
                if (Bot.HasInGraveyard(CardId.BrandishStartUpEngage) &&
                    Bot.GetRemainingCount(CardId.BrandishSkillAfterburner, 2) > 0 &&
                    m_count<2)
                    AI.SelectCard(CardId.BrandishStartUpEngage);
                else
                    AI.SelectCard(CardId.BrandishSkillAfterburner);
                return true;
            }
            if (SpellsCountInGrave() < 3 && Bot.HasInGraveyard(CardId.BrandishStartUpEngage))
            {
                AI.SelectCard(CardId.BrandishStartUpEngage);
                return true;
            }
            if (Bot.HasInGraveyard(CardId.HornetBit))
            {
                AI.SelectCard(CardId.HornetBit);
                return true;
            }
            if (SpellsCountInGrave()>=4 && Bot.HasInGraveyard(CardId.BrandishStartUpEngage))
            {
                AI.SelectCard(CardId.BrandishStartUpEngage);
                return true;
            }
                        
            else
                AI.SelectCard(new[] {
                    CardId.BrandishStartUpEngage,
                    CardId.HornetBit,
                    CardId.WidowAnchor,                    
                    CardId.BrandishSkillAfterburner
                });
            return true;
        }

        private bool ShizukuSp()
        {
            if (Bot.Hand.Count >= 6 && Enemy.GetMonsterCount() <= 1)
            {
                save_resource = true;
                return false;
            }
            if (Plan_A ||Plan_A_1)
            {
                if (Duel.Phase < DuelPhase.Main2)
                    return false;
            }
            foreach (ClientCard check in Bot.GetMonstersInExtraZone())
            {
                if (check.Id == CardId.BrandishMaidenKagari 
                    || check.Id == CardId.BrandishMaidenHayate)
                {
                    if(check.Attack<1500)
                    {
                        //AI.SelectPlace(Zones.z5, 3);
                        ShizukuSummoned = true;
                        return true;
                    }
                }
            }
            if (Bot.HasInMonstersZone(CardId.CrystronNeedlefiber) && Bot.HasInMonstersZone(CardId.BrandishMaidenHayate))
                return false;
            if (Util.IsTurn1OrMain2())
            {
                //AI.SelectPlace(Zones.z5, 3);
                ShizukuSummoned = true;
                return true;
            }
            return false;
        }

        private bool ShizukuEffect()
        {
            if (Bot.Hand.Count >= 6) return false;
            int target = ShizukuGetCardToSearch();
            if (target != 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(new[] {
                    CardId.BrandishStartUpEngage,
                    CardId.HornetBit,
                    CardId.WidowAnchor,
                    CardId.BrandishSkillAfterburner,
                    CardId.BrandishSkillJammingWave,
                    CardId.MultiRoll,
                    CardId.AreaZero
                });
            return true;
        }      

        private bool HayateSp()
        {
            if ((Plan_B || Plan_B_1 || Plan_B_3 || Plan_B_5) && Duel.Phase == DuelPhase.Main1) return false;
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenRei) &&
                summon_used && !Rei_eff_used && Duel.Phase == DuelPhase.Main1 &&
                Enemy.GetMonsterCount() == 0 && Duel.Turn > 1)
                return false;
            if (Enemy.LifePoints<=1500 && Duel.Phase == DuelPhase.Main1)
            {
                //AI.SelectPlace(Zones.z5, 3);
                HayateSummoned = true;
                return true;
            }
            if (Bot.Hand.Count >= 6 && Enemy.GetMonsterCount() <= 1)
            {                
                save_resource = true;
                return false;
            }
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenKagari) && 
                !KagariSummoned && Bot.HasInExtra(CardId.BrandishMaidenKagari))
            {
                //AI.SelectPlace(Zones.z5, 3);
                HayateSummoned = true;
                return true;
            }
            if (Util.IsTurn1OrMain2())
                return false;
            if (Enemy.HasInMonstersZone(_CardId.BlueEyesChaosMAXDragon,true) && !KagariSummoned)
            {
                //AI.SelectPlace(Zones.z5, 3);
                HayateSummoned = true;
                return true;
            }
            if (Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.BrandishMaidenRei) &&
                Bot.GetRemainingCount(CardId.HerculesBase, 1) == 0 &&
                Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.BrandishStartUpEngage) && SpellsCountInGrave()>=3)
                return false;
            //AI.SelectPlace(Zones.z5, 3);
            HayateSummoned = true;
            return true;
        }

        private bool HayateEffect()
        {
            if (!Bot.HasInGraveyard(CardId.BrandishMaidenRei))
            {
                AI.SelectCard(CardId.BrandishMaidenRei);
                return true;
            }
            else if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.BrandishStartUpEngage) &&
                !KagariSummoned && Duel.Player==0) 
            {
                AI.SelectCard(CardId.BrandishStartUpEngage);
                return true;
            }            
            else if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.BrandishStartUpEngage))
            {
                AI.SelectCard(CardId.BrandishStartUpEngage);
                return true;
            }
            else if (!(SpellsCountInGrave() > 5))
            {
                AI.SelectCard(new[] { CardId.AreaZero});
                return true;
            }
            return false;
        }  

        private bool NingirsuTheWorldChaliceWarriorsp()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.Id == CardId.CrystronNeedlefiber)
                {
                    material_list.Add(m);
                    break;
                }
            }
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.Id == CardId.Linkuriboh || 
                    m.Id==CardId.BrandishMaidenHayate || m.Id == CardId.BrandishMaidenKagari ||
                    m.Id==CardId.BrandishMaidenShizuku)
                {
                    material_list.Add(m);
                    if (material_list.Count == 2)
                        break;
                }
            }
            if (Enemy.GetMonsterCount() == 0 && Duel.Phase == DuelPhase.Main1)
                return false;
            if (material_list.Count == 2)
            {
                //AI.SelectPlace(Zones.z5, 3);
                Logger.DebugWriteLine("******NingirsuTheWorldChaliceWarrior");
                return true;
            }                
            return false;
        }

        private bool WarriorCleaneff()
        {
            if(Bot.HasInSpellZone(CardId.HerculesBase))
            {
                AI.SelectCard(CardId.HerculesBase);
                AI.SelectNextCard(Util.GetBestEnemyCard(false, true));
                return true;
            }
            if (GetNotBotMonster()!=null)
            {
                AI.SelectCard(GetNotBotMonster().Id);
                AI.SelectNextCard(Util.GetBestEnemyCard(false, true));
                return true;
            }
            if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
            {
                AI.SelectCard(CardId.MetalfoesFusion);
                AI.SelectNextCard(Util.GetBestEnemyCard(false, true));
                return true;
            }
            if (Bot.HasInSpellZone(CardId.Typhoon) && enemy_no_used_spell)
            {
                AI.SelectCard(CardId.Typhoon);
                AI.SelectNextCard(Util.GetBestEnemyCard(false, true));
                return true;
            }
            return false;
        }
        private bool LordOfTheLaireff()
        {            
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
            {
                if (lord_can_do)
                {
                    if (Bot.MonsterZone[1] == null)
                        AI.SelectPlace(Zones.z1);
                    else
                        AI.SelectPlace(Zones.z3);
                    lord_sp_used = true;
                    return true;
                }
                if (Plan_A || Plan_A_1) return false;
                if (Duel.Player == 1 && Enemy.HasInMonstersZone(CardId.UltimateConductorTytanno))
                    return false;
                lord_sp_used = true;
                if(Bot.MonsterZone[1] == null)
                        AI.SelectPlace(Zones.z1);
                    else
                        AI.SelectPlace(Zones.z3);
                return true;
            }
            if (Duel.Turn == 1)
            {
                lord_eff_used = true;
                AI.SelectCard(CardId.LordOfTheLair);
                return true;
            }
            if(lord_had_exist)
            {
                lord_eff_used = true;
                AI.SelectCard(CardId.LordOfTheLair);
                lord_had_exist = false;
                return true;
            }
            if (( Bot.HasInHandOrInSpellZone(CardId.HornetBit) || BrandishMonsterExist()) && 
                Duel.Phase == DuelPhase.Main2)
            {
                lord_eff_used = true;
                AI.SelectCard(CardId.LordOfTheLair);
                return true;
            }              
            return false;
        }
        
        private int GetDiscardHand()
        {
            if (Bot.HasInHand(CardId.MetalfoesFusion))
                return CardId.MetalfoesFusion;
            if (Bot.HasInHand(CardId.LordOfTheLair))
                return CardId.LordOfTheLair;
            if (Bot.HasInHand(CardId.ReinforcementOfTheArmy))
                return CardId.ReinforcementOfTheArmy;
            if (Bot.HasInHand(CardId.TheMelodyOfAwakeningDragon))
                return CardId.TheMelodyOfAwakeningDragon;
            if (Bot.HasInHand(CardId.TwinTwisters))
                return CardId.TwinTwisters;
            if (Bot.HasInHand(CardId.Typhoon))
                return CardId.Typhoon;
            if (Bot.HasInHand(CardId.HerculesBase))
                return CardId.HerculesBase;
            if (Bot.HasInHand(CardId.BrandishSkillJammingWave))
                return CardId.BrandishSkillJammingWave;
            if (Bot.HasInHand(CardId.AreaZero))
                return CardId.AreaZero;
            if (Bot.HasInHand(CardId.FoolishBurialGoods))
                return CardId.FoolishBurialGoods;
            if (Bot.HasInHand(CardId.BrandishMaidenRei) && !Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                return CardId.BrandishMaidenRei;            
            return 0;
        }

        private int ShizukuGetCardToSearch()
        {           
            if (!Bot.HasInGraveyard(CardId.BrandishStartUpEngage))
                return CardId.BrandishStartUpEngage;
            if (HerculesBaseSearch() && Bot.GetRemainingCount(CardId.HerculesBase, 1) > 0)
                return CardId.HerculesBase;
            if (Util.GetProblematicEnemyMonster(2400, true) != null &&
                Bot.GetRemainingCount(CardId.BrandishSkillAfterburner, 2) > 0 &&
                !Bot.HasInGraveyard(CardId.BrandishSkillAfterburner))
                return CardId.BrandishSkillAfterburner;
            if(!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.WidowAnchor))
                return CardId.WidowAnchor;
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.HornetBit))
                return CardId.HornetBit;
            if(Bot.HasInSpellZone(CardId.HerculesBase) && 
                !Bot.HasInSpellZone(CardId.MultiRoll) &&
                Bot.GetRemainingCount(CardId.MultiRoll,2)>0)
                return CardId.MultiRoll;                  
            if (Util.GetProblematicEnemyMonster(1600, true) != null &&
                Bot.GetRemainingCount(CardId.BrandishSkillAfterburner, 2) > 0)
                return CardId.BrandishSkillAfterburner;            
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.AreaZero))
                return CardId.AreaZero;
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.MultiRoll))
                return CardId.MultiRoll; 
            return 0;
        }
        private int EngageGetCardToSearch()
        {            
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.BrandishMaidenRei) ||
                (!BrandishMonsterRestart() && Bot.GetMonsterCount()==0))
                return CardId.BrandishMaidenRei;
            if (HerculesBaseSearch() && Bot.GetRemainingCount(CardId.HerculesBase, 1) > 0)
                return CardId.HerculesBase;            
            if (!BrandishMonsterExist() && Bot.GetRemainingCount(CardId.HornetBit, 1) > 0)
                return CardId.HornetBit;
            if (Bot.HasInSpellZone(CardId.HerculesBase) &&
                !Bot.HasInSpellZone(CardId.MultiRoll) &&
                Bot.GetRemainingCount(CardId.MultiRoll, 2) > 0)
                return CardId.MultiRoll;
            if (!BrandishMonsterExist() && !Bot.HasInHand(CardId.BrandishMaidenRei) &&
                !Bot.HasInHandOrInSpellZone(CardId.HornetBit) &&
                Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0)
                return CardId.BrandishMaidenRei;
            if (EnemySetSpellCount()>0 && Enemy.GetMonsterCount()>0 &&
               SpellsCountInGrave() >= 2 &&
               Bot.GetRemainingCount(CardId.BrandishSkillJammingWave, 1) > 0)
                return CardId.BrandishSkillJammingWave;
            if (Util.GetProblematicEnemyMonster(2400, true) != null &&
                Bot.GetRemainingCount(CardId.BrandishSkillAfterburner, 2) > 0)
                return CardId.BrandishSkillAfterburner;
            if (!Bot.HasInHandOrInSpellZone(CardId.WidowAnchor) &&
                Bot.GetRemainingCount(CardId.WidowAnchor, 3) > 0)
                return CardId.WidowAnchor;
            if (Bot.GetRemainingCount(CardId.HornetBit, 1) > 0)
                return CardId.HornetBit;
            if (Bot.HasInSpellZone(CardId.HerculesBase) &&
               SpellsCountInGrave() >= 2 &&
               Bot.GetRemainingCount(CardId.BrandishSkillJammingWave, 1) > 0)
                return CardId.BrandishSkillJammingWave;            
            if (!Bot.HasInHandOrInSpellZone(CardId.AreaZero) && Bot.GetRemainingCount(CardId.AreaZero, 2) > 0)
                return CardId.AreaZero;
            if (!Bot.HasInHandOrInSpellZone(CardId.MultiRoll) && Bot.GetRemainingCount(CardId.MultiRoll, 2) > 0)
                return CardId.MultiRoll;
            return 0;
        }        

        
        private bool InfiniteImpermanenceset()
        {            
            if (Bot.GetSpellCountWithoutField() >= 4)
                return false;
            if (Bot.GetFieldCount() == 0)
                return false;
            return true;
        }

        private bool HornetBitset()
        {
            if (Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster())
                return false;
            if (Bot.HasInSpellZone(CardId.HornetBit) || Bot.GetSpellCountWithoutField()>=4)
                return false;
            if (Bot.Hand.Count > 6) return true;
            return false;            
        }

        private bool TorrentialTributeset()
        {
            if (Bot.GetSpellCountWithoutField() >= 4)
                return false;
            return true;
        }

        private bool WidowAnchorset()
        {
            if(Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster())
                return false;            
            if (Bot.GetSpellCountWithoutField() >= 4)
                return false;
            return true;
        }
        private bool SpellSet()
        {
            if (Card.Id == CardId.Sharkcannon && Bot.GetSpellCountWithoutField() < 4) return true;
            if (Card.Id == CardId.Typhoon && Bot.GetSpellCountWithoutField()<4) return true;
            if(Card.Id==CardId.TheMelodyOfAwakeningDragon && Bot.GetSpellCountWithoutField() < 4 &&
                Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LordOfTheLair))                                          
                return true;            
            if (Card.Id == CardId.TwinTwisters && !Bot.HasInSpellZone(CardId.TwinTwisters))
                return true;
            if (Card.Id == CardId.MetalfoesFusion && 
                (Bot.HasInSpellZone(CardId.MultiRoll) ||
                Bot.HasInSpellZone(CardId.AreaZero)))
            {               
                return true;
            }            
            return false;            
        }
        private bool BaseGetFirst()
        {
           
            int fire_count = 0;
            int water_count = 0;
            int wind_count = 0;
            int Rei_count = 0;
            foreach (ClientCard card in Bot.GetGraveyardMonsters())
            {
                if (card.Id == CardId.BrandishMaidenKagari)
                    fire_count++;
                if (card.Id == CardId.BrandishMaidenShizuku)
                    water_count++;
            }
            if (!Bot.HasInExtra(CardId.BrandishMaidenKagari))
                return true;
            if (Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) == 0 && Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                Rei_count = 1;
            if (!Bot.HasInExtra(CardId.BrandishMaidenHayate) && Bot.HasInGraveyard(CardId.BrandishMaidenHayate))
                wind_count = 1;
            if (fire_count + water_count + wind_count + Rei_count >= 3)           
                return true;          
            return false;
        }
        private bool BaseSetFirst()
        {
            basetargets.Clear();
            int fire_count = 0;
            int water_count = 0;
            int wind_count = 0;
            int Rei_count = 0;
            foreach(ClientCard card in Bot.GetGraveyardMonsters())
            {
                if (card.Id == CardId.BrandishMaidenKagari)
                    fire_count++;
                if (card.Id == CardId.BrandishMaidenShizuku)
                    water_count++;                
            }
            if (Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) == 0 && Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                Rei_count = 1;
            if (!Bot.HasInExtra(CardId.BrandishMaidenHayate) && Bot.HasInGraveyard(CardId.BrandishMaidenHayate))
                wind_count = 1;
            if (fire_count + water_count + wind_count + Rei_count >= 3)
            {
                if(!AreaZero_used || !MultiRoll_used)
                {
                    if (Rei_count ==1)
                    {
                        foreach (ClientCard card0 in Bot.GetGraveyardMonsters())
                        {
                            if (card0.Id == CardId.BrandishMaidenRei)
                            {
                                basetargets.Add(card0);
                                break;
                            }
                        }                       
                    }
                    foreach (ClientCard card2 in Bot.GetGraveyardMonsters())
                    {
                        if (card2.Id == CardId.BrandishMaidenKagari)
                        {
                            basetargets.Add(card2);
                        }                       
                    }
                    if (wind_count ==1)
                    {
                        foreach (ClientCard card3 in Bot.GetGraveyardMonsters())
                        {
                            if (card3.Id == CardId.BrandishMaidenHayate)
                            {
                                basetargets.Add(card3);
                                break;
                            }
                        }                        
                    }
                    foreach (ClientCard card4 in Bot.GetGraveyardMonsters())
                    {
                        if(card4.Id==CardId.BrandishMaidenShizuku)
                        {
                            basetargets.Add(card4);
                        }                       
                    }
                    return true;
                }
            }
            return false;            
        }
        private bool HerculesBaseSearch()
        {
            int count = 0;
            foreach (ClientCard card in Bot.GetGraveyardMonsters())
            {
                if (card.Id == CardId.BrandishMaidenKagari)
                    count++;
                if (card.Id == CardId.BrandishMaidenShizuku)
                    count++;
                if (card.Id == CardId.BrandishMaidenHayate)
                    count++;
            }
            if (count >= 3)
                return true;
            return false;
        }
        private bool ResourceRestart()
        {
            if (base_draw_first && Duel.Phase == DuelPhase.Main1)
                return false;
            if(Card.Id==CardId.AreaZero || Card.Id==CardId.MultiRoll)
            {
                if(Bot.HasInSpellZone(CardId.HerculesBase) && Card.Location==CardLocation.SpellZone)
                {
                    AI.SelectCard(CardId.HerculesBase);
                    return true;
                }
            }
            return false;
        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown()) return true;
            return DefaultMonsterRepos();
        }       

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {

            if (!(Plan_A || Plan_A_1) &&
                Bot.HasInGraveyard(CardId.BrandishMaidenRei) &&
                !Bot.HasInMonstersZone(CardId.BrandishMaidenRei) &&
                Bot.HasInHandOrInGraveyard(CardId.LordOfTheLair) &&
                !lord_sp_used && !lord_eff_used &&
                !Rei_eff_used && !Rei_grave_used &&
                Util.IsOneEnemyBetterThanValue(1500, true) &&
                (attacker.Id == CardId.BrandishMaidenHayate ||
                attacker.Id == CardId.BrandishMaidenKagari ||
                attacker.Id == CardId.BrandishMaidenShizuku)
                )
                attacker.RealPower = 8889;
            if ((Plan_A||Plan_A_1) && attacker.Id == CardId.BrandishMaidenHayate)
                attacker.RealPower = 9999;
            if (attacker.Id == CardId.BirrelswordDragon)
                attacker.RealPower +=defender.RealPower/2;
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {           
            if ((Plan_A || Plan_A_1) && attacker.Id == CardId.BrandishMaidenHayate)
            {                
                for (int i = 0; i < defenders.Count; ++i)
                {
                    ClientCard defender = defenders[i];

                    attacker.RealPower = attacker.Attack;
                    defender.RealPower = defender.GetDefensePower();
                    if (!OnPreBattleBetween(attacker, defender))
                        continue;
                    ClientCard HayateTarget = null;
                    List<ClientCard> Hayatecheck = new List<ClientCard>();
                    foreach (ClientCard check in Enemy.GetMonsters())
                    {
                        if (check.IsAttack() && !check.IsMonsterDangerous() && check.Attack >= 1500)
                            Hayatecheck.Add(check);
                    }
                    Hayatecheck.Sort(CardContainer.CompareDefensePower);
                    HayateTarget = Hayatecheck[0];

                    Logger.DebugWriteLine("******HayateTarget= " + HayateTarget.Name);
                    if (defender == HayateTarget)
                    {
                        Logger.DebugWriteLine("******defender= " + defender.Name);
                        return AI.Attack(attacker, defender);                        
                    }
                }
            }                
            return base.OnSelectAttackTarget(attacker, defenders);
        }
        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if(Plan_A || Plan_A_1 || Plan_A_2)
            {
                for (int i = 0; i < attackers.Count; ++i)
                {
                    ClientCard attacker = attackers[i];
                    if (attacker.Id == CardId.BrandishMaidenHayate) return attacker;
                }                
            }            
            return base.OnSelectAttacker(attackers, defenders);
        }
        public override bool OnSelectYesNo(int desc)
        {            
            bool check = false;
            foreach(ClientCard m in Enemy.GetMonsters())
            {
                if (m.GetDefensePower() < 1500 && !m.IsMonsterDangerous() && !m.IsMonsterInvincible())
                    check = true;
            }
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsAttack() && m.GetDefensePower() == 1500 && !m.IsMonsterDangerous() && !m.IsMonsterInvincible() &&!Rei_grave_used)
                    check = true;
            }
            if (desc == 31 && Enemy.LifePoints <= 1500)
                return true;
            if (desc == 31 && (Plan_A ||Plan_A_1 || check ))
                return false;           
            if (desc== 800083490)
            {
                if (Bot.Hand.Count >= 6) return false;
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.BrandishMaidenRei))
                {
                    
                    AI.SelectCard(new[] { CardId.BrandishStartUpEngage, CardId.BrandishMaidenRei,CardId.HornetBit });
                }
                else if (Enemy.GetMonsterCount() >= 2)
                {
                    
                    AI.SelectCard(new[] {  CardId.BrandishStartUpEngage, CardId.BrandishSkillAfterburner, CardId.WidowAnchor });
                }
                else if (Bot.HasInHandOrInSpellZone(CardId.HerculesBase) && !Bot.HasInHandOrInSpellZone(CardId.MultiRoll))
                {

                    AI.SelectCard(new[] { CardId.BrandishStartUpEngage, CardId.MultiRoll, CardId.BrandishSkillAfterburner });
                }
                else
                {                    
                    AI.SelectCard(new[]{
                    CardId.BrandishStartUpEngage,
                    CardId.WidowAnchor,
                    CardId.BrandishSkillAfterburner,
                    CardId.HornetBit});
                }                    
                return true;
            }
            if (desc == Util.GetStringId(CardId.SummonSorceress, 2)) // summon to the field of opponent?
                return false;
            if (desc == Util.GetStringId(CardId.BrandishStartUpEngage, 0)) // draw card?
                return true;
            if (desc == Util.GetStringId(CardId.WidowAnchor, 0)) // get control?
            {
                if (Enemy.HasInMonstersZone(CardId.ElShaddollWinda) && Duel.Player==0)
                    return false;
                if (Widow_control)
                    return true;
                return false;
            }            
            if (desc == Util.GetStringId(CardId.BrandishSkillJammingWave, 0)) // destroy monster?
            {
                ClientCard target = Util.GetBestEnemyMonster();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                else
                    return false;
            }
            if (desc == Util.GetStringId(CardId.BrandishSkillAfterburner, 0)) // destroy spell & trap?
            {
                if(Bot.HasInSpellZone(CardId.AreaZero) && 
                    Bot.GetMonsterCount()==0 && 
                    !Bot.HasInHand(CardId.BrandishMaidenRei) &&
                    !Bot.HasInHandOrInSpellZone(CardId.HornetBit) &&
                    !Bot.HasInHand(CardId.BrandishStartUpEngage))
                {
                    AI.SelectCard(CardId.AreaZero);
                    return true;
                }
                if (Util.GetPZone(1, 0) != null && Util.GetPZone(1, 0).Type == 16777218)
                {
                    
                    AI.SelectCard(Util.GetPZone(1, 0));
                    return UniqueFaceupSpell();
                }
                if (Util.GetPZone(1, 1) != null && Util.GetPZone(1, 1).Type == 16777218)
                {                    
                    AI.SelectCard(Util.GetPZone(1,1));
                    return UniqueFaceupSpell();
                }
                if(Bot.HasInSpellZone(CardId.HerculesBase))
                {
                    AI.SelectCard(CardId.HerculesBase);
                    return true;
                }
                if (Bot.HasInSpellZone(CardId.MetalfoesFusion))
                {
                    AI.SelectCard(CardId.MetalfoesFusion);
                    return true;
                }
                ClientCard target = Util.GetBestEnemySpell();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                else
                    return false;
            }
            return base.OnSelectYesNo(desc);
        }

       /* public override void OnMove()
        {
            CheckPlan();
            base.OnMove();
        }*/

        public override void OnChaining(int player, ClientCard card)
        {            
            base.OnChaining(player, card);
        }


        public override void OnChainEnd()
        {
            CheckPlan();
            if(Plan_A || Plan_A_1 || Plan_A_2 || Plan_A_3)
            {
                if(Util.GetOneEnemyBetterThanValue(1500, true) == null)
                {
                    Plan_A = false;
                    Plan_A_1 = false;
                    Plan_A_2 = false;
                    Plan_A_3 = false;
                }
            }
            foreach(ClientCard check in Bot.GetMonsters())
            {               
            }
            if (Util.GetPZone(1, 0) != null && Util.GetPZone(1, 0).Type == 16777218)
            {
                IsPendulumDeck = true;
            }
            if (Util.GetPZone(1, 1) != null && Util.GetPZone(1, 1).Type == 16777218)
            {
                IsPendulumDeck = true;
            }
            WaitHeavymetalfoes = false;
            if (IsPendulumDeck &&
                !Enemy.HasInGraveyard(CardId.HeavymetalfoesElectrumite) &&
                !Enemy.HasInBanished(CardId.HeavymetalfoesElectrumite) &&
                !Enemy.HasInMonstersZone(CardId.HeavymetalfoesElectrumite))
                WaitHeavymetalfoes = true;
            if (Util.ChainContainsCard(CardId.MaxxC))
                MaxxC_used = true;
            if (Util.ChainContainsCard(CardId.LockBird))
                Lockbird_used = true;
            base.OnChainEnd();
        }

        public override bool OnSelectHand()
        {            
            return true;
        }

        private bool GetPlan()
        {
            if (Plan_A || Plan_A_1 || Plan_A_3 || Plan_A_2 || Plan_B || Plan_B_1 || Plan_B_2 || Plan_B_3 ||
                Plan_B_4 || Plan_B_5 || Plan_B_6 || Plan_C || Plan_C_1 || Plan_D)
                return true;
            return false;
        }
        private bool SpellCanUse(int CardName)
        {
            if (Bot.HasInHand(CardName) && Bot.GetSpellCountWithoutField() <= 4)
                return true;
            if (Bot.HasInSpellZone(CardName))
                return true;
            return false;
        }

        private int SpellsCountInGrave()
        {
            int count = 0;
            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card.IsSpell())
                {
                    count++;
                }
            }
            return count;
        }

        private int NegateCount()
        {
            int count= 0;
            foreach (ClientCard check in Bot.Hand)
            {
                if (check.Id == CardId.EffectVeiler)
                    count++;
            }
            foreach(ClientCard check in Bot.GetSpells())
            {
                if (check.Id == CardId.InfiniteImpermanence ||
                    check.Id == CardId.WidowAnchor)
                    count++;
                if (check.Id == CardId.SolemnStrike && Bot.LifePoints > 1500)
                    count++;
            }            
            return count;
        }
        private int CuteCount()
        {
            int count = 0;
            foreach (ClientCard check in Bot.Hand)
            {
                if (check.Id == CardId.GhostRabbit||
                    check.Id==CardId.AshBlossom)
                    count++;
            }
            return count;
        }
        private int StopCardCount()
        {
            return CuteCount() + NegateCount();
        }
        
        private bool BrandishMonsterRestart()
        {
            if (Bot.HasInHand(CardId.BrandishMaidenRei))
                return true;
            if (Bot.HasInHandOrInSpellZone(CardId.HornetBit))
                return true;
            if (Bot.HasInHandOrInSpellZone(CardId.AreaZero) &&
                (Bot.HasInHandOrInSpellZone(CardId.MultiRoll) || Bot.HasInHandOrInSpellZone(CardId.Typhoon)))
                return true;
            return false;
        }

        private bool BrandishMonsterExist()
        {
            foreach (ClientCard check in Bot.GetMonsters())
            {
                if (check.Id == CardId.BrandishMaidenRei || check.Id == CardId.HornetBit + 1)
                    return true;
                if (check.Id == CardId.BrandishMaidenShizuku ||
                    check.Id == CardId.BrandishMaidenKagari ||
                    check.Id == CardId.BrandishMaidenHayate)
                {
                    return true;
                }
            }
            return false;
        }
    }
}