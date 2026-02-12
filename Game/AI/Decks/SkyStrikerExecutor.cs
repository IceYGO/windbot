using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("SkyStriker", "AI_SkyStriker")]
    public class SkyStrikerExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Raye = 26077387;
            public const int Kagari = 63288573;
            public const int Shizuku = 90673288;
            public const int Hayate = 8491308;
            public const int Token = 52340445;

            public const int Engage = 63166095;
            public const int HornetDrones = 52340444;
            public const int WidowAnchor = 98338152;
            public const int Afterburners = 99550630;
            public const int JammingWave = 25955749;
            public const int Multirole = 24010609;
            public const int HerculesBase = 97616504;
            public const int AreaZero = 50005218;

            public const int AshBlossom = 14558127;
            public const int GhostRabbit = 59438930;
            public const int MaxxC = 23434538;
            public const int JetSynchron = 9742784;
            public const int EffectVeiler = 97268402;

            public const int ReinforcementOfTheArmy = 32807846;
            public const int FoolishBurialGoods = 35726888;
            public const int UpstartGoblin = 70368879;
            public const int MetalfoesFusion = 73594093;
            public const int TwinTwisters = 43898403;
            public const int SolemnJudgment = 41420027;
            public const int SolemnWarning = 84749824;

            public const int HiSpeedroidChanbara = 42110604;
            public const int TopologicBomberDragon = 5821478;
            public const int TopologicTrisbaena = 72529749;
            public const int SummonSorceress = 61665245;
            public const int TroymareUnicorn = 38342335;
            public const int TroymarePhoenix = 2857636;
            public const int CrystronNeedlefiber = 50588353;
            public const int Linkuriboh = 41999284;
        }

        bool KagariSummoned = false;
        bool ShizukuSummoned = false;
        bool HayateSummoned = false;
        ClientCard WidowAnchorTarget = null;

        public SkyStrikerExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, DefaultSolemnJudgment);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCEffect);

            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurialGoods, FoolishBurialGoodsEffect);

            AddExecutor(ExecutorType.Activate, CardId.TwinTwisters, TwinTwistersEffect);

            //
            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleHandEffect);

            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffectFirst);

            AddExecutor(ExecutorType.Activate, CardId.Afterburners, AfterburnersEffect);
            AddExecutor(ExecutorType.Activate, CardId.JammingWave, JammingWaveEffect);

            AddExecutor(ExecutorType.Activate, CardId.Engage, EngageEffectFirst);

            AddExecutor(ExecutorType.Activate, CardId.HornetDrones, HornetDronesEffect);

            AddExecutor(ExecutorType.Activate, CardId.WidowAnchor, WidowAnchorEffect);

            AddExecutor(ExecutorType.Activate, CardId.HerculesBase, HerculesBaseEffect);
            AddExecutor(ExecutorType.Activate, CardId.AreaZero, AreaZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleEffect);

            AddExecutor(ExecutorType.Activate, CardId.Engage, EngageEffect);

            //
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.GhostRabbit, TunerSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, TunerSummon);

            AddExecutor(ExecutorType.Activate, CardId.Raye, RayeEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Kagari, KagariSummon);
            AddExecutor(ExecutorType.Activate, CardId.Kagari, KagariEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.CrystronNeedlefiber, CrystronNeedlefiberSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrystronNeedlefiber, CrystronNeedlefiberEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.SummonSorceress);
            AddExecutor(ExecutorType.Activate, CardId.SummonSorceress, SummonSorceressEffect);
            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchronEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.HiSpeedroidChanbara);

            AddExecutor(ExecutorType.SpSummon, CardId.Shizuku, ShizukuSummon);
            AddExecutor(ExecutorType.Activate, CardId.Shizuku, ShizukuEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Hayate, HayateSummon);
            AddExecutor(ExecutorType.Activate, CardId.Hayate, HayateEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.TopologicBomberDragon, Util.IsTurn1OrMain2);

            AddExecutor(ExecutorType.Summon, CardId.Raye, RayeSummon);

            //
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnJudgment);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning);
            AddExecutor(ExecutorType.SpellSet, CardId.WidowAnchor);
            AddExecutor(ExecutorType.SpellSet, CardId.HerculesBase);

            AddExecutor(ExecutorType.SpellSet, CardId.TwinTwisters, HandFull);
            AddExecutor(ExecutorType.SpellSet, CardId.HornetDrones, HandFull);

            //
            AddExecutor(ExecutorType.Activate, CardId.MetalfoesFusion);
            AddExecutor(ExecutorType.Activate, CardId.Multirole, MultiroleEPEffect);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            KagariSummoned = false;
            ShizukuSummoned = false;
            HayateSummoned = false;
            WidowAnchorTarget = null;
            base.OnNewTurn();
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.HiSpeedroidChanbara) && !attacker.IsDisabled())
                    attacker.RealPower = attacker.RealPower + 200;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.SummonSorceress, 2)) // summon to the field of opponent?
                return false;
            if (desc == Util.GetStringId(CardId.Engage, 0)) // draw card?
                return true;
            if (desc == Util.GetStringId(CardId.WidowAnchor, 0)) // get control?
                return true;
            if (desc == Util.GetStringId(CardId.JammingWave, 0)) // destroy monster?
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
            if (desc == Util.GetStringId(CardId.Afterburners, 0)) // destroy spell & trap?
            {
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

        private bool MaxxCEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player == 1;
        }

        private bool TwinTwistersEffect()
        {
            if (Util.ChainContainsCard(CardId.TwinTwisters))
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
                    if (target.IsFacedown() || target.HasType(CardType.Continuous) || target.HasType(CardType.Pendulum))
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
            AI.SelectCard(
                CardId.MetalfoesFusion,
                CardId.WidowAnchor,
                CardId.Engage,
                CardId.HornetDrones
                );
            return true;
        }

        private bool MultiroleHandEffect()
        {
            return Card.Location == CardLocation.Hand;
        }

        private bool MultiroleEPEffect()
        {
            if (Duel.Phase != DuelPhase.End)
                return false;

            IList<int> targets = new[] {
                CardId.Engage,
                CardId.HornetDrones,
                CardId.WidowAnchor
            };
            AI.SelectCard(targets);
            AI.SelectNextCard(targets);
            AI.SelectThirdCard(targets);
            return true;
        }

        private bool AfterburnersEffect()
        {
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool JammingWaveEffect()
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

        private bool WidowAnchorEffectFirst()
        {
            if (Util.ChainContainsCard(CardId.WidowAnchor))
                return false;
            ClientCard target = Util.GetProblematicEnemyMonster(0, true);
            if (target != null)
            {
                WidowAnchorTarget = target;
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool EngageEffectFirst()
        {
            if (!HaveThreeSpellsInGrave())
                return false;

            int target = GetCardToSearch();
            if (target > 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(
                    CardId.Multirole,
                    CardId.AreaZero,
                    CardId.Afterburners,
                    CardId.JammingWave,
                    CardId.Raye
                    );

            return true;
        }


        private bool EngageEffect()
        {
            int target = GetCardToSearch();
            if (target > 0)
                AI.SelectCard(target);
            else
                AI.SelectCard(
                    CardId.Multirole,
                    CardId.AreaZero,
                    CardId.Afterburners,
                    CardId.JammingWave,
                    CardId.Raye
                    );

            return true;
        }

        private bool HornetDronesEffect()
        {
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
                if (!Util.IsTurn1OrMain2())
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

        private bool WidowAnchorEffect()
        {
            if (DefaultBreakthroughSkill())
            {
                WidowAnchorTarget = Util.GetLastChainCard();
                return true;
            }

            if (!HaveThreeSpellsInGrave() || Duel.Player == 1 || Duel.Phase < DuelPhase.Main1 || Duel.Phase >= DuelPhase.Main2 || Util.ChainContainsCard(CardId.WidowAnchor))
                return false;

            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if (target != null && !target.IsDisabled() && !target.HasType(CardType.Normal))
            {
                WidowAnchorTarget = target;
                AI.SelectCard(target);
                return true;
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
                    if (card.IsCode(CardId.Hayate, CardId.Kagari, CardId.Shizuku))
                        targets.Add(card);
                }
                if (targets.Count > 0)
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            else
            {
                if (Util.IsTurn1OrMain2())
                    return false;
                ClientCard bestBotMonster = Util.GetBestBotMonster(true);
                if (bestBotMonster != null)
                {
                    int bestPower = bestBotMonster.Attack;
                    int count = 0;
                    bool have3 = HaveThreeSpellsInGrave();
                    foreach (ClientCard target in Enemy.GetMonsters())
                    {
                        if (target.GetDefensePower() < bestPower && !target.IsMonsterInvincible())
                        {
                            count++;
                            if (count > 1 || have3)
                            {
                                AI.SelectCard(bestBotMonster);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool AreaZeroEffect()
        {
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
            {
                return true;
            }
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
                if (target.IsCode(CardId.Raye) && Bot.GetMonstersExtraZoneCount() == 0)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            foreach (ClientCard target in Bot.GetSpells())
            {
                if (!target.IsCode(CardId.AreaZero, CardId.Multirole, CardId.WidowAnchor) && target.IsSpell())
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        private bool MultiroleEffect()
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
                    if (target.IsCode(CardId.Raye) && Bot.GetMonstersExtraZoneCount() == 0)
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                foreach (ClientCard target in Bot.GetSpells())
                {
                    if (target.IsCode(CardId.AreaZero))
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
                foreach (ClientCard target in Bot.GetSpells())
                {
                    if (!target.IsCode(CardId.Multirole, CardId.WidowAnchor) && target.IsSpell())
                    {
                        AI.SelectCard(target);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool RayeSummon()
        {
            if (Bot.GetMonstersExtraZoneCount() == 0)
            {
                return true;
            }
            return false;
        }

        private bool RayeEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            if (Card.IsDisabled())
            {
                return false;
            }
            if (Util.IsChainTarget(Card))
            {
                RayeSelectTarget();
                return true;
            }
            if (Card.Attacked && Duel.Phase == DuelPhase.BattleStart)
            {
                RayeSelectTarget();
                return true;
            }
            if (Card == Bot.BattlingMonster && Duel.Player == 1)
            {
                RayeSelectTarget();
                return true;
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                RayeSelectTarget();
                return true;
            }
            return false;
        }

        private void RayeSelectTarget()
        {
            if (!KagariSummoned && Bot.HasInGraveyard(new[] {
                CardId.Engage,
                CardId.HornetDrones,
                CardId.WidowAnchor
            }))
            {
                AI.SelectCard(CardId.Kagari);
            }
            else
            {
                AI.SelectCard(CardId.Shizuku, CardId.Kagari, CardId.Hayate);
            }
        }

        private bool KagariSummon()
        {
            if (Bot.HasInGraveyard(new[] {
                CardId.Engage,
                CardId.HornetDrones,
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
            if (EmptyMainMonsterZone() && Util.GetProblematicEnemyMonster() != null && Bot.HasInGraveyard(CardId.Afterburners))
            {
                AI.SelectCard(CardId.Afterburners);
            }
            else if (EmptyMainMonsterZone() && Util.GetProblematicEnemySpell() != null && Bot.HasInGraveyard(CardId.JammingWave))
            {
                AI.SelectCard(CardId.JammingWave);
            }
            else
                AI.SelectCard(CardId.Engage, CardId.HornetDrones, CardId.WidowAnchor);
            return true;
        }

        private bool ShizukuSummon()
        {
            if (Util.IsTurn1OrMain2())
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
                AI.SelectCard(CardId.Engage, CardId.HornetDrones, CardId.WidowAnchor);
            return true;
        }

        private bool HayateSummon()
        {
            if (Util.IsTurn1OrMain2())
                return false;
            HayateSummoned = true;
            return true;
        }

        private bool HayateEffect()
        {
            if (!Bot.HasInGraveyard(CardId.Raye))
                AI.SelectCard(CardId.Raye);
            else if (!Bot.HasInGraveyard(CardId.HornetDrones))
                AI.SelectCard(CardId.HornetDrones);
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
            }) && !Util.IsTurn1OrMain2()
               && Bot.GetMonsterCount() > 0
               && Bot.HasInExtra(CardId.CrystronNeedlefiber);
        }

        private bool CrystronNeedlefiberSummon()
        {
            return !Util.IsTurn1OrMain2();
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
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.HasInMonstersZone(CardId.Raye) || Bot.HasInMonstersZone(CardId.CrystronNeedlefiber))
            {
                AI.SelectCard(GetDiscardHand());
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
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
            if (Bot.HasInHand(CardId.Raye) && !Bot.HasInGraveyard(CardId.Raye))
                return CardId.Raye;
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
            if (!Bot.HasInHand(CardId.HornetDrones) && Bot.GetRemainingCount(CardId.HornetDrones, 3) > 0)
            {
                return CardId.HornetDrones;
            }
            else if (Util.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.WidowAnchor, 3) > 0)
            {
                return CardId.WidowAnchor;
            }
            else if (EmptyMainMonsterZone() && Util.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.Afterburners, 1) > 0)
            {
                return CardId.Afterburners;
            }
            else if (EmptyMainMonsterZone() && Util.GetProblematicEnemySpell() != null && Bot.GetRemainingCount(CardId.JammingWave, 1) > 0)
            {
                return CardId.JammingWave;
            }
            else if (!Bot.HasInHand(CardId.Raye) && !Bot.HasInMonstersZone(CardId.Raye) && Bot.GetRemainingCount(CardId.Raye, 3) > 0)
            {
                return CardId.Raye;
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

        private bool DefaultNoExecutor()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return true;
        }

    }
}