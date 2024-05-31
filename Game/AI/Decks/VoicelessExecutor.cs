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
            AddExecutor(ExecutorType.Activate, CardId.LittleKnight, SPLittleKnightActivate); //Borrowed from Louse's Labrynth Executor; hope it's ok
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel, ChaosAngelActivate);
            AddExecutor(ExecutorType.Activate, CardId.Ntss, NtssActivate);

            AddExecutor(ExecutorType.Activate, CardId.VVBarrier, BarrierFirst);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffEffect);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffiraRitual);
            AddExecutor(ExecutorType.SpSummon, CardId.VVSauravis);
            AddExecutor(ExecutorType.Activate, CardId.VVSauravis);
            AddExecutor(ExecutorType.Summon, CardId.Diviner, DivinerEffect);
            AddExecutor(ExecutorType.Activate, CardId.Diviner);
            AddExecutor(ExecutorType.Activate, CardId.VVPrayer);
            AddExecutor(ExecutorType.Activate, CardId.Trias, TriasEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Lo, LoPlace);
            AddExecutor(ExecutorType.Summon, CardId.Lo, LoSummon);
            AddExecutor(ExecutorType.Activate, CardId.Lo, LoEffect);
            AddExecutor(ExecutorType.Activate, CardId.VVSkullGuard, SkullSearch);
            AddExecutor(ExecutorType.Activate, CardId.VVSkullGuard, SkullCounter);
            AddExecutor(ExecutorType.Activate, CardId.PrePrep, PrePrepSearch);
            AddExecutor(ExecutorType.Activate, CardId.Herald, HeraldSearch);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel, ChaosAngelSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.LittleKnight, SPLittleKnightSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.DynaMondo, DynaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddess);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddess);
            
            List<int> notToNegateIdList = new List<int>{
                58699500, 20343502
            };
            List<int> notToBeTrapTargetList = new List<int>{
                72144675, 86188410, 41589166, 11443677, 72566043, 1688285, 59071624, 6511113, 48183890, 952523, 22423493, 73639099
            };
            List<int> targetNegateIdList = new List<int> {
                _CardId.EffectVeiler, _CardId.InfiniteImpermanence, CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
                9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
            };
            List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

            bool summoned = false; //Some unused variables since code was borrowed from Louse's lab
            List<int> activatedCardIdList = new List<int>();
            List<ClientCard> currentNegateMonsterList = new List<ClientCard>();
            List<ClientCard> currentDestroyCardList = new List<ClientCard>();
            List<ClientCard> setTrapThisTurn = new List<ClientCard>();
            List<ClientCard> summonThisTurn = new List<ClientCard>();
            List<ClientCard> enemySetThisTurn = new List<ClientCard>();
            List<ClientCard> escapeTargetList = new List<ClientCard>();
            List<ClientCard> summonInChainList = new List<ClientCard>();
            int banSpSummonExceptFiendCount = 0;
            int enemySpSummonFromExLastTurn = 0;
            int enemySpSummonFromExThisTurn = 0;
            List<int> chainSummoningIdList = new List<int>(3);
        }

        public bool NtssActivate()
        {
            List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
            problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
            if (GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() == 0) //If there's no cards on opponent's field
                return false;
            else
            {
                AI.SelectCard(problemCardList);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
        }
        public bool ChaosAngelActivate()
        {
            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Monster);
            if (targetList.Count() > 0)
            {
                AI.SelectCard(targetList);
                currentDestroyCardList.Add(targetList[0]);
                return true;
            }
            return false;
        }
        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemySetThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemySetThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown() && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList()));
            return targetList;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count() > 0) return dangerList[0];

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count() > 0) return invincibleList[0];

            List<ClientCard> equippedList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.EquipCards.Count() > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (equippedList.Count() > 0) return equippedList[0];

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count() > 0)
            {
                foreach (ClientCard target in enemyMonsters)
                {
                    if ((target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                            || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                        && CheckCanBeTargeted(target, canBeTarget, selfType) && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(target))
                    ) return target;
                }
            }

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = Util.GetBestAttack(Bot);
                List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                    .Where(card => card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).OrderByDescending(card => card.Attack).ToList();
                if (betterList.Count() > 0) return betterList[0];
            }
            return null;
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count() > 0) resultList.AddRange(ShuffleList(problemEnemySpellList));

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count() > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count() > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count() > 0)
            {
                foreach(ClientCard target in enemyMonsters)
                {
                    if ( (target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                            || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                        && !resultList.Contains(target) && CheckCanBeTargeted(target, canBeTarget, selfType)
                        )
                    {
                        resultList.Add(target);
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !currentDestroyCardList.Contains(c)
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous) && CheckCanBeTargeted(c, canBeTarget, selfType)
                && !notToDestroySpellTrap.Contains(c.Id)).ToList();
            if (spells.Count() > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
        }

        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0
                    && (card.IsShouldNotBeSpellTrapTarget() || !card.IsDisabled())) return false;
            }
            return true;
        }
        public bool CheckCanDirectAttack()
        {
            return Enemy.GetMonsterCount() == 0 && !activatedCardIdList.Contains(CardId.LittleKnight) && Duel.Turn > 1 && Duel.Player == 0 && Duel.Phase < DuelPhase.Main2;
        }

        public bool SPLittleKnightSpSummon()
        {
            if (CheckCanDirectAttack())
            {
                // for attack
                List<ClientCard> materialList = SPLittleKnightSelectMaterial();
                if (materialList.Count() >= 2 && GetMaterialAttack(materialList) < 1600)
                {
                    AI.SelectMaterials(materialList);
                    return true;
                }
            } else if (!NegatedCheck(true, true, CardType.Monster | CardType.Link) && GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0)
            {
                // for remove 
                List<ClientCard> materialList = SPLittleKnightSelectMaterial(true);
                if (materialList.Count() >= 2 && materialList.Any(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)))
                {
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }
            return false;
        }
        public int GetMaterialAttack(List<ClientCard> materials)
        {
            if (Util.IsTurn1OrMain2()) return 0;
            int result = 0;
            foreach (ClientCard material in materials)
            {
                if (material.IsAttack() || !summonThisTurn.Contains(material)) result += material.Attack;
            }
            return result;
        }
        public List<ClientCard> GetCanBeUsedForLinkMaterial(bool useAdvancedMonster = false, Func<ClientCard, bool> exceptRule = null)
        {
            List<ClientCard> materialList = Bot.GetMonsters().Where(card => {
                if (card.IsFacedown() || (exceptRule != null && exceptRule(card))) return false;
                if (card.IsCode(CardId.ChaosAngel)
                    && !useAdvancedMonster && (card.IsAttack() || !summonThisTurn.Contains(card))) return false;
                return true;
            }).ToList();
            materialList.Sort();
            return materialList;
        }
        public List<ClientCard> SPLittleKnightSelectMaterial(bool needToUseEffect = false)
        {
            List<ClientCard> usedMaterialList = new List<ClientCard>();
            if (Bot.GetMonstersExtraZoneCount() > 0)
            {
                ClientCard botMonsterExtraZome = Bot.GetMonstersInExtraZone()[0];
                if (botMonsterExtraZome.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Pendulum))
                {
                    usedMaterialList.Add(botMonsterExtraZome);
                    if (botMonsterExtraZome.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)) needToUseEffect = false;
                }
                List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => card == botMonsterExtraZome);
                if (materialList.Count() > 0)
                {
                    foreach (ClientCard card in materialList)
                    {
                        if (!needToUseEffect || card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz) || (card.HasType(CardType.Link) && card.LinkCount <= 2))
                        {
                            usedMaterialList.Add(card);
                            if (card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)) needToUseEffect = false;
                        }
                        if (usedMaterialList.Count() >= 2) break;
                    }
                }
                if (usedMaterialList.Count() < 2) usedMaterialList.Clear();
            } else {
                List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => !needToUseEffect
                    || card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz) || (card.HasType(CardType.Link) && card.LinkCount <= 2));
                if (materialList.Count() >= 2)
                {
                    for (int idx1 = 0; idx1 < materialList.Count() - 1; ++ idx1)
                    {
                        ClientCard material1 = materialList[idx1];
                        if (material1.HasType(CardType.Link) && material1.LinkCount >= 3) continue;
                        bool flag1 = !needToUseEffect || material1.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
                        for (int idx2 = 0; idx2 < materialList.Count(); ++ idx2)
                        {
                            ClientCard material2 = materialList[idx2];
                            if (material2.HasType(CardType.Link) && material2.LinkCount >= 3) continue;
                            bool flag2 = !needToUseEffect || material2.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
                            if (flag1 || flag2)
                            {
                                return new List<ClientCard>{material1, material2};
                            }
                        }
                    }
                }
            }

            return usedMaterialList;
        }
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
                (!onlyMonster || card.IsMonster())).ToList(); //some specific matchup stuff removed
            List<int> dangerMonsterIdList = new List<int>{
                99937011, 63542003, 9411399, 28954097, 30680659
            };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget, ignoreCurrentDestroy, selfType);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.Where(c => c?.Data != null && c.HasType(CardType.Monster) && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType) && (!ignoreCurrentDestroy || currentDestroyCardList.Contains(c)))
                .OrderByDescending(c => c.Attack).FirstOrDefault();
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters().Where(c => !ignoreCurrentDestroy || currentDestroyCardList.Contains(c)).ToList();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count() > 0 && !onlyFaceup)
                return ShuffleList(monsters)[0];

            return null;
        }
        public bool SPLittleKnightActivate()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 0))
            {
                // banish card
                List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
                problemCardList.AddRange(GetDangerousCardinEnemyGrave(false));
                problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => card.HasType(CardType.Monster)).OrderByDescending(card => card.Attack));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => !card.HasType(CardType.Monster)));
                if (problemCardList.Count() > 0)
                {
                    AI.SelectCard(problemCardList);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            } else if (ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 1))
            {
                ClientCard selfMonster = null;
                foreach (ClientCard target in Bot.GetMonsters())
                {
                    if (Duel.ChainTargets.Contains(target) && !escapeTargetList.Contains(target))
                    {
                        selfMonster = target;
                        break;
                    }
                }
                if (selfMonster == null)
                {
                    if (Duel.Player == 1)
                    {
                        selfMonster = Bot.GetMonsters().Where(card => card.IsAttack()).OrderBy(card => card.Attack).FirstOrDefault();
                        if (!Util.IsOneEnemyBetterThanValue(selfMonster.Attack, true)) selfMonster = null;
                    }
                }
                if (selfMonster != null)
                {
                    ClientCard nextMonster = null;
                    List<ClientCard> selfTargetList = Bot.GetMonsters().Where(card => card != selfMonster).ToList();
                    if (Enemy.GetMonsterCount() == 0 && selfTargetList.Count() > 0)
                    {
                        selfTargetList.Sort(CompareUsableAttack);
                        nextMonster = selfTargetList[0];
                        escapeTargetList.Add(nextMonster);
                    }
                    if (Enemy.GetMonsterCount() > 0)
                    {
                        nextMonster = GetBestEnemyMonster(false, true, true);
                        currentDestroyCardList.Add(nextMonster);
                    }
                    if (nextMonster != null)
                    {
                        AI.SelectCard(selfMonster);
                        AI.SelectNextCard(nextMonster);
                        escapeTargetList.Add(selfMonster);
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SkullCounter()
        {
            return Duel.LastChainPlayer == 1;
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
        private bool LittleKnightSummon()
        {
            return Util.GetProblematicEnemyMonster() != null;
        }

        private bool HeraldSearch()
        {
            if (Bot.HasInHand(CardId.VVPrayer))
                AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis);
            else
                AI.SelectCard(CardId.VVPrayer);
            return true;
        }

        private bool TriasEffect()
        {
            if (Bot.HasInHand(CardId.Trias) && Util.ChainContainPlayer(0)) // Might need to change to when diviner is targetted by opponent; but don't know how to do that...
            {//maybe chaintargets?
                return false;
            }
            AI.SelectCard(CardId.Diviner);
            return true;
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
                AI.SelectCard(CardId.Herald);
            else
                AI.SelectCard(CardId.Ntss);
            return true;
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
