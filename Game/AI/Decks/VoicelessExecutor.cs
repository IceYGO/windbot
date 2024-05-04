using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;


namespace WindBot.Game.AI.Decks
{
    [Deck("Voiceless", "AI_Voiceless")]
    public class VoicelessExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Saffira = 51296484;
            public const int Lo = 25801745;
            public const int VVSauravis = 88284599;
            public const int VVSkullGuard = 10774240;
            public const int VVPrayer = 52472775;
            public const int Token = 52340445;
            public const int VVBarrier = 98477480;
            public const int VVRadiance = 86310763;

            public const int AshBlossom = 14558127;
            public const int EffectVeiler = 97268402;
            public const int InfiniteImperm = 10045474;
            public const int Droll = 94145021;
            public const int PrePrep = 13048472;
            public const int Diviner = 92919429;
            public const int Trias = 26866984;
            public const int CalledBy = 24224830;
            public const int Sauravis = 4810828;
            public const int SolemnStrike = 40605147;

            public const int DynaMondo = 73898890;
            public const int LittleKnight = 29301450;
            public const int Masquerena = 65741786;
            public const int UnderworldGoddess = 98127546;
            public const int Ntss = 80532587;
            public const int ChaosAngel = 22850702;
            public const int Herald = 79606837;
        }

        //bool KagariSummoned = false;
        //bool ShizukuSummoned = false;
        //bool HayateSummoned = false;
        bool DivinerCheck = false; // In case of Trias in Hand, add either spell/ritual or board break

        public VoicelessExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImperm, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.Droll);
            AddExecutor(ExecutorType.Activate, CardId.CalledBy, CalledByEffect);
            AddExecutor(ExecutorType.Activate, CardId.Sauravis);


            AddExecutor(ExecutorType.Activate, CardId.VVBarrier, BarrierFirst);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffEffect);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffiraRitual);
            AddExecutor(ExecutorType.Summon, CardId.Diviner, DivinerEffect);
            AddExecutor(ExecutorType.Activate, CardId.Diviner);

            AddExecutor(ExecutorType.SpSummon, CardId.Lo, LoPlace);
            AddExecutor(ExecutorType.Summon, CardId.Lo, LoSummon);
            AddExecutor(ExecutorType.Activate, CardId.Lo, LoEffect);
            AddExecutor(ExecutorType.Activate, CardId.VVSkullGuard, SkullSearch);
            AddExecutor(ExecutorType.Activate, CardId.PrePrep, PrePrepSearch);
            AddExecutor(ExecutorType.Activate, CardId.Herald, HeraldSearch);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel, ChaosAngelSummon);
            AddExecutor(ExecutorType.Activate, CardId.LittleKnight, LIttleKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.DynaMondo, DynaSummon);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddess);
        }

        private bool ChaosAngelSummon()
        {
            AI.SelectMaterials(new[]
            {
                CardId.Trias,
                CardId.Lo
            });
            return Util.GetProblematicEnemyMonster() != null;
        }
        private bool DynaSummon()
        {
            AI.SelectMaterials(new[]
            {
                CardId.VVSkullGuard,
                CardId.Lo
            });
            return Util.GetProblematicEnemyMonster() != null;
        }
        private bool LIttleKnightSummon()
        {
            return Util.GetProblematicEnemyMonster() != null;
        }

        private bool HeraldSearch()
        {
            if (Bot.HasInHand(CardId.VVPrayer))
                AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis);
            else
                AI.SelectCard(CardId.VVPrayer);
        }

        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false, List<int> avoid_list = null)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
                    AI.SelectPlace(zone);
                    return;
                };
            }
            AI.SelectPlace(0);
        }

        // check whether negate maxxc and InfiniteImpermanence
        public void CheckDeactiveFlag()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.MaxxC && Duel.LastChainPlayer == 1)
            {
                enemy_activate_MaxxC = false;
            }
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.DimensionShifter && Duel.LastChainPlayer == 1)
            {
                enemy_activate_DimensionShifter = false;
            }
        }

        public bool NegatedCheck(bool disablecheck = true)
        {
            if (Card.IsSpell() || Card.IsTrap())
            {
                if (SpellNegatable()) return true;
            }
            if (CheckCalledbytheGrave(Card.Id) > 0 || Card.Id == CrossoutDesignatorTarget)
            {
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Numbe41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null
                    || Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Numbe41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null)
                {
                    return true;
                }
            }
            if (disablecheck)
            {
                return Card.IsDisabled();
            }
            return false;
        }

        public int CheckCalledbytheGrave(int id)
        {
            if (!CalledbytheGraveCount.ContainsKey(id))
            {
                return 0;
            }
            return CalledbytheGraveCount[id];
        }
        // activate of CalledbytheGrave
        public bool CalledByEffect()
        {
            if (NegatedCheck(true)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().Id;
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0 || CrossoutDesignatorTarget == code) return false;
                    if (Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code)) != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(code);
                        CalledbytheGraveCount[code] = 2;
                        CheckDeactiveFlag();
                        return true;
                    }
                }

                // banish target
                foreach (ClientCard cards in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(cards))
                    {
                        int code = cards.Id;
                        AI.SelectCard(cards);
                        CalledbytheGraveCount[code] = 2;
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemy_monsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemy_monsters.Count > 0)
                    {
                        enemy_monsters.Sort(CardContainer.CompareCardAttack);
                        enemy_monsters.Reverse();
                        int code = enemy_monsters[0].Id;
                        AI.SelectCard(code);
                        CalledbytheGraveCount[code] = 2;
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0)
            {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                CalledbytheGraveCount[code] = 2;
                return true;
            }

            return false;
        }

        public override bool OnSelectHand()
        {
            //go first
            return true;
        }

        private bool BarrierFirst()
        {
            if Bot.HasInSpellZone(CardId.VVBarrier, 0)
                return false;
            int target = GetCardToSearch();
            if (GetCardToSearch() > 0)
                AI.SelectCard(GetCardToSearch());
            else
            {
                AI.SelectCard(CardId.Lo, CardId.VVSkullGuard, CardId.Saffira, CardId.VVRadiance);
            }
            return true;
        }

        private bool DivinerEffect()
        {
            if (!Bot.HasInHand(CardId.Trias) && !Bot.HasInGraveyard(CardId.Trias))
                AI.SelectCard(CardId.Trias);
            else if (Util.GetProblematicEnemyMonster() != null && !Bot.HasInGraveyard())
        }

        private bool SkullSearch()
        {
            if (Duel.CurrentChain.Count > 1) // chain blocking; don't search lo
                if (Bot.HasInHand(CardId.Sauravis))
                    AI.SelectCard(CardId.VVSauravis)
        }

        private bool SaffEffect()
        {
            AI.SelectCard(CardId.VVPrayer);
            if (GetCardToSearch() > 0)
                AI.SelectCard(GetCardToSearch());
            else
                AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis);
            return true;
        }

        private bool SaffiraRitual()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Saffira, 1))
                return false;
            if (Bot.HasInHand(CardId.VVSkullGuard) && !Bot.HasInMonstersZone(CardId.VVSkullGuard))
                AI.SelectCard(CardId.VVSkullGuard);
            else if (Bot.HasInHand(CardId.Sauravis))
                AI.SelectCard(CardId.Sauravis);

            // select sacrifice
            if (Bot.HasInHand(Trias) || Bot.HasInMonstersZone(Trias))
                AI.SelectCard(CardId.Trias)
            else if (Bot.HasInHand)
                return true;
        }


        public override bool OnSelectYesNo(long desc)
        {
            if (desc == Util.GetStringId(CardId.Saffira, 2)) // search ritual monster?
                return true;
            if (desc == Util.GetStringId(CardId.VVPrayer, 0)) // reinforce ritual monsters?
                return true;
            return base.OnSelectYesNo(desc);
        }

        private bool NoLo()
        {
            return !Bot.HasInMonstersZone(CardId.Lo) && !Bot.HasInGraveyard(CardId.Lo);
        }

        private bool LoSummon()
        {
            if (!Bot.HasInHand(CardId.Diviner) && !Bot.HasInHand(CardId.PrePrep) && )
                return true;
            return false;
        }

        private bool LoPlace()
        {
            if (!Bot.HasInSpellZone(CardId.VVBarrier) && !Bot.HasInHand(CardId.VVBarrier))
                AI.SelectCard(CardId.VVBarrier);
            else if (!Bot.HasInHand(CardId.VVRadiance))
                AI.SelectCard(CardId.VVRadiance);
            return true;
            // TODO: Add Blessing
        }

        private int PrePrepSearch()
        {
            if (GetCardToSearch() > 0)
                AI.SelectCard(GetCardToSearch());
            else
                AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis, CardId.VVPrayer);
        }

        private int GetCardToSearch()
        {
            if (NoLo() && !Bot.HasInHand(CardId.Lo) && !Bot.HasInHand(CardId.Diviner) && Bot.GetRemainingCount(CardId.Lo, 3) > 0)
            {
                return CardId.Lo;
            }
            else if (!Bot.HasInHand(CardId.Saffira) && Bot.GetRemainingCount(CardId.Saffira, 3) > 0)
            {
                return CardId.Saffira
            }
            else if (EmptyMainMonsterZone() && !Bot.HasInHand(CardId.VVSkullGuard) && Bot.GetRemainingCount(CardId.VVSkullGuard, 3) > 0)
            {
                return CardId.VVSkullGuard;
            }
            else if (Bot.GetRemainingCount(CardId.VVRadiance, 1) > 0)
            {
                return CardId.VVRadiance;
            }
            else if (Bot.GetRemainingCount(CardId.VVSauravis, 1) > 0) //if there's >= 2 spells in grave
            {
                return CardId.VVSauravis;
            }
            // else if (Util.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.WidowAnchor, 3) > 0)
            // {
            //     return CardId.WidowAnchor;
            // }

            return 0;
        }

        private bool LoEffect()
        {
            // todo
            // if (Bot.HasInSpellZone && Bot.HasInSpellZone())
            //     return false;
            if (Bot.HasInSpellZone(CardId.VVBarrier))
                AI.SelectCard(CardId.VVRadiance);
            else
                AI.SelectCard(CardId.VVBarrier);
            return true;
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
