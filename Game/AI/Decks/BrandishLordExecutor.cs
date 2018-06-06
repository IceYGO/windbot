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
            public const int GlowUpBulb = 67441435;

            public const int BrandishSkillJammingWave = 25955749;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int FoolishBurialGoods = 35726888;
            public const int TheMelodyOfAwakeningDragon = 48800175;
            public const int BrandishStartUpEngage = 63166095;
            public const int MetalfoesFusion = 73594093;
            public const int Terraforming = 73628505;
            public const int BrandishSkillAfterburner = 99550630;
            public const int TwinTwisters = 43898403;
            public const int HornetBit = 52340444;
            public const int Token = 52340445;
            public const int WidowAnchor = 98338152;
            public const int MultiRoll = 24010609;
            public const int HerculesBase = 97616504;
            public const int AreaZero = 50005218;
            public const int InfiniteImpermanence = 10045474;
            
            public const int BorreloadDragon = 31833038;
            public const int BirrelswordDragon = 85289965;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int CrystronNeedlefiber = 50588353;           
            public const int KnightmareCerberus = 75452921;
            public const int BrandishMaidenHayate = 8491308;
            public const int BrandishMaidenShizuku = 90673288;
            public const int BrandishMaidenKagari = 63288573;            
            public const int Linkuriboh = 41999284;

            
            public const int GhostRabbit = 59438930;
            public const int JetSynchron = 9742784;
            public const int EffectVeiler = 97268402;
            public const int HiSpeedroidChanbara = 42110604;
            public const int TopologicBomberDragon = 5821478;
            public const int TopologicTrisbaena = 72529749;
            public const int SummonSorceress = 61665245;
            public const int TroymareUnicorn = 38342335;
            public const int TroymarePhoenix = 2857636;

            public const int SolemnJudgment = 41420027;
            public const int SolemnWarning = 84749824;
            public const int UpstartGoblin = 70368879;
            public const int MacroCosmos = 30241314;
            public const int LockBird = 94145021;
        }
        bool one_turn_kill = false;
        bool KagariSummoned = false;
        bool ShizukuSummoned = false;
        bool HayateSummoned = false;
        bool AreaZero_used = false;
        bool MultiRoll_used = false;
        bool MaxxC_used = false;
        bool Lockbird_used = false;
        ClientCard AreaZeroTarget = null;
        ClientCard WidowAnchorTarget = null;
        public BrandishLordExecutor(GameAI ai, Duel duel)
            : base(ai, duel)            
        {
            //counter
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, AshBlossomAndJoyousSpringeff);
            AddExecutor(ExecutorType.Activate, CardId.GhostRabbit, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCEffect);
            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence,DefaultInfiniteImpermanence);            
            AddExecutor(ExecutorType.Activate, CardId.TwinTwisters, TwinTwistersEffect);//
            //AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultBreakthroughSkill);
            //AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            //AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);          
            //first set
            AddExecutor(ExecutorType.SpellSet, SpellSetFirst);
            AddExecutor(ExecutorType.Activate, ResourceRestart);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, MultiRollEffectFirst);
            //restart resource
            AddExecutor(ExecutorType.Activate, CardId.HerculesBase, HerculesBaseEffect);//
            //first do
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurialGoods, FoolishBurialGoodsEffect);//
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy);
            AddExecutor(ExecutorType.Activate,CardId.TheMelodyOfAwakeningDragon, TheMelodyOfAwakeningDragoneff);
            //AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);            
            AddExecutor(ExecutorType.Activate, CardId.BrandishStartUpEngage, BrandishStartUpEngageEffect);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillAfterburner, BrandishSkillAfterburnerEffect);
            AddExecutor(ExecutorType.Activate, CardId.BrandishSkillJammingWave, BrandishSkillJammingWaveEffect);           

            AddExecutor(ExecutorType.Activate, CardId.HornetBit, HornetBitEffect);

            AddExecutor(ExecutorType.Activate, CardId.AreaZero, AreaZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.MultiRoll, MultiRollEffect);

            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenRei, BrandishMaidenReiEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenKagari, KagariSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenKagari, KagariEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSp);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect);
            
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenShizuku, ShizukuSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenShizuku, ShizukuEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BrandishMaidenHayate, HayateSp);
            AddExecutor(ExecutorType.Activate, CardId.BrandishMaidenHayate, HayateEffect);            

            AddExecutor(ExecutorType.Summon, CardId.BrandishMaidenRei, BrandishMaidenReiSummon);


            AddExecutor(ExecutorType.SpellSet, SpellSet);
            //otherthing
            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, CardId.LordOfTheLair, LordOfTheLaireff);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }

       

        public override void OnNewTurn()
        {
            KagariSummoned = false;
            ShizukuSummoned = false;
            HayateSummoned = false;
            WidowAnchorTarget = null;
            AreaZeroTarget = null;
            MaxxC_used = false;
            Lockbird_used = false;
        }

       
        private bool MaxxCEffect()
        {
            return Duel.Player == 1;
        }

        private bool AshBlossomAndJoyousSpringeff()
        {
            if (AI.Utils.GetLastChainCard().Id == CardId.MacroCosmos)
                return false;
            return Duel.LastChainPlayer == 1;
        }
        
        private bool WidowAnchorEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return UniqueFaceupSpell();
            }
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if (AI.Utils.GetLastChainCard() == check)
                    {
                        target = check;
                        break;
                    }
                }
                if (target != null)
                {
                    AI.SelectCard(target);
                    return UniqueFaceupSpell();
                }
            }
            return false;
        }
        private bool TwinTwistersEffect()
        {
            if (AI.Utils.ChainContainsCard(CardId.TwinTwisters))
                return false;
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard target in Enemy.GetSpells())
            {
                if (target.IsFloodgate())
                    targets.Add(target);
                if (targets.Count >= 2)
                    break;
            }
            if (targets.Count < 2)
            {
                foreach (ClientCard target in Enemy.GetSpells())
                {
                    targets.Add(target);
                    if (targets.Count >= 2)
                        break;
                }
            }
            if (targets.Count > 0)
            {
                AI.SelectCard(GetDiscardHand());
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        private bool FoolishBurialGoodsEffect()
        {
            AI.SelectCard(new[]{
                CardId.MetalfoesFusion,
                CardId.BrandishStartUpEngage,
                CardId.WidowAnchor,                
                CardId.HornetBit
            });
            return true;
        }


        private bool TheMelodyOfAwakeningDragoneff()
        {
            if(Bot.HasInHand(CardId.LordOfTheLair) && Bot.GetRemainingCount(CardId.LordOfTheLair,3)==2)
            {
                AI.SelectCard(CardId.LordOfTheLair);
                AI.SelectNextCard(new[] { CardId.LordOfTheLair, CardId.LordOfTheLair});
                return true;
            }
            return false;
            
        }        

        private bool MultiRollHandEffect()
        {
            return Card.Location == CardLocation.Hand;
        }

        private bool MultiRollEPEffect()
        {
            if (Duel.Phase != DuelPhase.End)
                return false;

            IList<int> targets = new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor
            };
            AI.SelectCard(targets);
            AI.SelectNextCard(targets);
            AI.SelectThirdCard(targets);
            return true;
        }

        private bool BrandishSkillAfterburnerEffect()
        {
            ClientCard target = AI.Utils.GetBestEnemyMonster(true);
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool BrandishSkillJammingWaveEffect()
        {           
            ClientCard target = null;
            foreach(ClientCard card in Enemy.GetSpells())
            {
                if (card.IsFacedown())
                {
                    target = card;
                    break;
                }
            }
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool BrandishStartUpEngageEffect()
        {
            int target = GetCardToSearch();
            if (target > 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(new[] {
                    CardId.MultiRoll,
                    CardId.AreaZero,
                    CardId.BrandishSkillAfterburner,
                    CardId.BrandishSkillJammingWave,
                    CardId.BrandishMaidenRei
                });

            return true;
        }

        private bool HornetBitEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (Duel.Player == 1)
            {
                return Duel.Phase == DuelPhase.End;
            }
            else
            {
                if (Duel.Phase != DuelPhase.Main1)
                    return false;
                if (Duel.CurrentChain.Count > 0)
                    return false;
                if (Bot.GetMonstersExtraZoneCount() == 0)
                    return true;
                if (Bot.HasInMonstersZone(CardId.SummonSorceress))
                    return true;
                if (Bot.HasInMonstersZone(CardId.TopologicBomberDragon) && Enemy.GetMonsterCount() > 1)
                    return true;
                if (!AI.Utils.IsTurn1OrMain2())
                {
                    foreach (ClientCard card in Bot.Hand)
                    {
                        if (card.IsTuner())
                            return true;
                    }
                }
            }
            return false;
        }
        

        private bool HerculesBaseEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                IList<ClientCard> targets = new List<ClientCard>();
                foreach(ClientCard card in Bot.GetGraveyardMonsters())
                {
                    if (card.Id == CardId.BrandishMaidenHayate || card.Id == CardId.BrandishMaidenKagari || card.Id == CardId.BrandishMaidenShizuku)
                        targets.Add(card);
                }
                if (targets.Count > 0)
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }           
            return false;
        }

        private bool AreaZeroEffect()
        {
            if(Card.Location==CardLocation.Hand)
            {
                if (!Bot.HasInSpellZone(CardId.AreaZero))
                    return true;
                return false;
            }
            if (Card.Location == CardLocation.Grave) return true;
            if(Card.Location==CardLocation.SpellZone)
            {
                if (Card.IsFacedown()) return true;
                if(AI.Utils.ChainContainsCard(CardId.BrandishMaidenRei))
                {
                    AI.SelectCard(CardId.BrandishMaidenRei);
                    return true;
                }
                if(Bot.HasInSpellZone(CardId.MetalfoesFusion))
                {
                    AI.SelectCard(CardId.MetalfoesFusion);
                    return true;                        
                }
                if(Bot.HasInSpellZone(CardId.HerculesBase))
                {
                    AI.SelectCard(CardId.HerculesBase);
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
                    return true;
            }
            return false;
        }

            private bool MultiRollEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                foreach (ClientCard target in Bot.GetMonsters())
                {
                    if (target == WidowAnchorTarget && Duel.Phase == DuelPhase.Main2)
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                foreach (ClientCard target in Bot.GetMonsters())
                {
                    if (target.Id == CardId.BrandishMaidenRei && Bot.GetMonstersExtraZoneCount() == 0)
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                foreach (ClientCard target in Bot.GetSpells())
                {
                    if (target.Id == CardId.AreaZero)
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                foreach (ClientCard target in Bot.GetSpells())
                {
                    if (target.Id != CardId.MultiRoll && target.Id != CardId.WidowAnchor && target.IsSpell())
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool BrandishMaidenReiSummon()
        {
            if (Bot.GetMonstersExtraZoneCount() == 0)
            {
                return true;
            }
            return false;
        }

        private bool BrandishMaidenReiEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            if (Card.IsDisabled())
            {
                return false;
            }
            if (AI.Utils.IsChainTarget(Card))
            {
                BrandishMaidenReiSelectTarget();
                return true;
            }
            if (Duel.LastChainPlayer == 0) return false;
            if (Card.Attacked && Duel.Phase == DuelPhase.BattleStart)
            {
                BrandishMaidenReiSelectTarget();
                return true;
            }
            if (Card == Bot.BattlingMonster && Duel.Player == 1)
            {
                BrandishMaidenReiSelectTarget();
                return true;
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                BrandishMaidenReiSelectTarget();
                return true;
            }
            return false;
        }

        private void BrandishMaidenReiSelectTarget()
        {
            if (!KagariSummoned && Bot.HasInGraveyard(new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor
            }))
            {
                AI.SelectCard(CardId.BrandishMaidenKagari);
            }
            else
            {
                AI.SelectCard(new[] {
                    CardId.BrandishMaidenShizuku,
                    CardId.BrandishMaidenKagari,
                    CardId.BrandishMaidenHayate
                });
            }
        }

        private bool KagariSp()
        {
            if (Bot.HasInGraveyard(new[] {
                CardId.BrandishStartUpEngage,
                CardId.HornetBit,
                CardId.WidowAnchor
            }))
            {
                KagariSummoned = true;
                return true;
            }
            return false;
        }

        private bool KagariEffect()
        {
            if (EmptyMainMonsterZone() && AI.Utils.GetProblematicEnemyMonster() != null && Bot.HasInGraveyard(CardId.BrandishSkillAfterburner))
            {
                AI.SelectCard(CardId.BrandishSkillAfterburner);
            }
            else if (EmptyMainMonsterZone() && AI.Utils.GetProblematicEnemySpell() != null && Bot.HasInGraveyard(CardId.BrandishSkillJammingWave))
            {
                AI.SelectCard(CardId.BrandishSkillJammingWave);
            }
            else
                AI.SelectCard(new[] {
                    CardId.BrandishStartUpEngage,
                    CardId.HornetBit,
                    CardId.WidowAnchor
                });
            return true;
        }

        private bool ShizukuSp()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                ShizukuSummoned = true;
                return true;
            }
            return false;
        }

        private bool ShizukuEffect()
        {
            int target = GetCardToSearch();
            if (target != 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(new[] {
                    CardId.BrandishStartUpEngage,
                    CardId.HornetBit,
                    CardId.WidowAnchor
                });
            return true;
        }

        private bool HayateSp()
        {
            if (AI.Utils.IsTurn1OrMain2())
                return false;
            HayateSummoned = true;
            return true;
        }

        private bool HayateEffect()
        {
            if (!Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                AI.SelectCard(CardId.BrandishMaidenRei);
            else if (!Bot.HasInGraveyard(CardId.HornetBit))
                AI.SelectCard(CardId.HornetBit);
            else if (!Bot.HasInGraveyard(CardId.WidowAnchor))
                AI.SelectCard(CardId.WidowAnchor);
            return true;
        }

        private bool TunerSummon()
        {
            return !Bot.HasInMonstersZone(new[] {
                CardId.AshBlossom,
                CardId.EffectVeiler,
                CardId.GhostRabbit,
                CardId.JetSynchron
            }) && !AI.Utils.IsTurn1OrMain2()
               && Bot.GetMonsterCount() > 0
               && Bot.HasInExtra(CardId.CrystronNeedlefiber);
        }

        private bool CrystronNeedlefiberSp()
        {
            return one_turn_kill;
        }

        private bool CrystronNeedlefiberEffect()
        {
            AI.SelectCard(CardId.JetSynchron);
            return true;
        }

        private bool SummonSorceressEffect()
        {
            if (ActivateDescription == -1)
                return false;
            return true;
        }

        private bool JetSynchronEffect()
        {
            if (Bot.HasInMonstersZone(CardId.BrandishMaidenRei) || Bot.HasInMonstersZone(CardId.CrystronNeedlefiber))
            {
                AI.SelectCard(GetDiscardHand());
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool LordOfTheLaireff()
        {
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave) return true;
            if (Duel.Turn == 1) return true;
            if (BrandishMonsterExist() && Duel.Phase == DuelPhase.Main2) return true;            
            return false;
        }

        private bool HandFull()
        {
            return Bot.GetSpellCountWithoutField() < 4 && Bot.Hand.Count > 4;
        }

        private int GetDiscardHand()
        {
            if (Bot.HasInHand(CardId.MetalfoesFusion))
                return CardId.MetalfoesFusion;
            if (Bot.HasInHand(CardId.BrandishMaidenRei) && !Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                return CardId.BrandishMaidenRei;
            if (Bot.HasInHand(CardId.JetSynchron))
                return CardId.JetSynchron;
            if (Bot.HasInHand(CardId.ReinforcementOfTheArmy))
                return CardId.ReinforcementOfTheArmy;
            if (Bot.HasInHand(CardId.FoolishBurialGoods))
                return CardId.FoolishBurialGoods;
            return 0;
        }

        private int GetCardToSearch()
        {
            if (!Bot.HasInHand(CardId.HornetBit) && Bot.GetRemainingCount(CardId.HornetBit, 3) > 0)
            {
                return CardId.HornetBit;
            }
            else if (AI.Utils.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.WidowAnchor, 3) > 0)
            {
                return CardId.WidowAnchor;
            }
            else if (EmptyMainMonsterZone() && AI.Utils.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.BrandishSkillAfterburner, 1) > 0)
            {
                return CardId.BrandishSkillAfterburner;
            }
            else if (EmptyMainMonsterZone() && AI.Utils.GetProblematicEnemySpell() != null && Bot.GetRemainingCount(CardId.BrandishSkillJammingWave, 1) > 0)
            {
                return CardId.BrandishSkillJammingWave;
            }
            else if (!Bot.HasInHand(CardId.BrandishMaidenRei) && !Bot.HasInMonstersZone(CardId.BrandishMaidenRei) && Bot.GetRemainingCount(CardId.BrandishMaidenRei, 3) > 0)
            {
                return CardId.BrandishMaidenRei;
            }
            else if (!Bot.HasInHand(CardId.WidowAnchor) && !Bot.HasInSpellZone(CardId.WidowAnchor) && Bot.GetRemainingCount(CardId.WidowAnchor, 3) > 0)
            {
                return CardId.WidowAnchor;
            }

            return 0;
        }

        private bool EmptyMainMonsterZone()
        {
            for (int i = 0; i < 5; i++)
            {
                if (Bot.MonsterZone[i] != null)
                    return false;
            }
            return true;
        }

        private bool HaveThreeSpellsInGrave()
        {
            int count = 0;
            foreach(ClientCard card in Bot.Graveyard)
            {
                if (card.IsSpell())
                {
                    count++;
                }
            }
            return count >= 3;
        }

        private bool BrandishMonsterExist()
        {
            foreach(ClientCard check in Bot.GetMonsters())
            {
                if (check.Id == CardId.BrandishMaidenRei)
                    return true;
                if(check.Id==CardId.BrandishMaidenShizuku ||
                    check.Id==CardId.BrandishMaidenKagari||
                    check.Id==CardId.BrandishMaidenHayate)
                {
                    if (Bot.HasInGraveyard(CardId.BrandishMaidenRei))
                        return true;
                }
            }
            return false;
        }

        private bool SpellSet()
        {
            return true;
        }

        private bool SpellSetFirst()
        {
            if(Card.Id==CardId.HerculesBase && (!AreaZero_used || !MultiRoll_used))
            return true;
            return false;
        }

        private bool ResourceRestart()
        {
            if(Card.Id==CardId.AreaZero || Card.Id==CardId.MultiRoll)
            {
                if(Bot.HasInSpellZone(CardId.HerculesBase))
                {
                    AI.SelectCard(CardId.HerculesBase);
                    return true;
                }
            }
            return false;
        }

        private bool MonsterRepos()
        {
            return DefaultMonsterRepos();
        }

        public override void OnChainEnd()
        {
            if (AI.Utils.ChainContainsCard(CardId.MaxxC))
                MaxxC_used = true;
            if (AI.Utils.ChainContainsCard(CardId.LockBird))
                Lockbird_used = true;
            base.OnChainEnd();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {            
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == AI.Utils.GetStringId(CardId.SummonSorceress, 2)) // summon to the field of opponent?
                return false;
            if (desc == AI.Utils.GetStringId(CardId.BrandishStartUpEngage, 0)) // draw card?
                return true;
            if (desc == AI.Utils.GetStringId(CardId.WidowAnchor, 0)) // get control?
                return true;
            if (desc == AI.Utils.GetStringId(CardId.BrandishSkillJammingWave, 0)) // destroy monster?
            {
                ClientCard target = AI.Utils.GetBestEnemyMonster();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                else
                    return false;
            }
            if (desc == AI.Utils.GetStringId(CardId.BrandishSkillAfterburner, 0)) // destroy spell & trap?
            {
                ClientCard target = AI.Utils.GetBestEnemySpell();
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

        public override bool OnSelectHand()
        {            
            return true;
        }
    }
}