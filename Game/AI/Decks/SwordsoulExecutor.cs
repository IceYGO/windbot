using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("Swordsoul", "AI_Swordsoul")]

    public class SwordsoulExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int NibiruThePrimalBeing = 27204311;
            public const int TenyiSpirit_Ashuna = 87052196;
            public const int TenyiSpirit_Vishuda = 23431858;
            public const int SwordsoulStrategistLongyuan = 93490856;
            public const int SwordsoulOfTaia = 56495147;
            public const int SwordsoulOfMoYe = 20001443;
            public const int IncredibleEcclesiaTheVirtuous = 55273560;
            // _CardId.AshBlossom = 14558127;
            // _CardId.MaxxC = 23434538;
            // _CardId.EffectVeiler = 97268402;
            public const int TenyiSpirit_Adhara = 98159737;

            // _CardId.PotOfDesires = 35261759;
            public const int SwordsoulEmergence = 56465981;
            public const int SwordsoulSacredSummit = 93850690;
            // _CardId.CalledByTheGrave = 24224830;
            public const int CrossoutDesignator = 65681983;

            // _CardId.InfiniteImpermanence = 10045474;
            public const int SwordsoulBlackout = 14821890;

            public const int GeomathmechFinalSigma = 42632209;
            public const int PsychicEndPunisher = 60465049;
            public const int SwordsoulSupremeSovereign_Chengying = 96633955;
            public const int BaronneDeFleur = 84815190;
            public const int SwordsoulSinisterSovereign_QixingLongyuan = 47710198;
            public const int AdamancipatorRisen_Dragite = 9464441;
            public const int DracoBerserkerOfTheTenyi = 5041348;
            public const int SwordsoulGrandmaster_Chixiao = 69248256;
            public const int BaxiaBrightnessOfTheYangZing = 83755611;
            public const int YaziEvilOfTheYangZing = 43202238;
            public const int ShamanOfTheTenyi = 78917791;
            public const int MonkOfTheTenyi = 32519092;

            public const int SwordsoulToken = 20001444;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
        }

        public SwordsoulExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // startup effect
            AddExecutor(ExecutorType.Activate, CardId.BaxiaBrightnessOfTheYangZing,  BaxiaBrightnessOfTheYangZingActivate);
            AddExecutor(ExecutorType.Activate, CardId.YaziEvilOfTheYangZing,         YaziEvilOfTheYangZingActivate);
            AddExecutor(ExecutorType.Activate, CardId.IncredibleEcclesiaTheVirtuous, IncredibleEcclesiaTheVirtuousActivate);

            // quick effect
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave,                         CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator,                        CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom,                               AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulGrandmaster_Chixiao,              SwordsoulGrandmaster_ChixiaoActivate);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler,                             EffectVeilerActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence,                     InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSinisterSovereign_QixingLongyuan, SwordsoulSinisterSovereign_QixingLongyuanActivate);
            AddExecutor(ExecutorType.Activate, CardId.DracoBerserkerOfTheTenyi,                  DracoBerserkerOfTheTenyiActivate);
            AddExecutor(ExecutorType.Activate, CardId.AdamancipatorRisen_Dragite,                AdamancipatorRisen_DragiteActivate);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur,                            BaronneDeFleurActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSupremeSovereign_Chengying,       SwordsoulSupremeSovereign_ChengyingActivate);

            // free chain
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.NibiruThePrimalBeing, NibiruThePrimalBeingActivate);

            // startup effect
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulEmergence,  SwordsoulEmergenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulBlackout,   SwordsoulBlackoutActivate);

            // synchro
            AddExecutor(ExecutorType.SpSummon, CardId.YaziEvilOfTheYangZing,        YaziEvilOfTheYangZingSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BaxiaBrightnessOfTheYangZing, BaxiaBrightnessOfTheYangZingSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SwordsoulGrandmaster_Chixiao, SwordsoulGrandmaster_ChixiaoSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AdamancipatorRisen_Dragite,   AdamancipatorRisen_DragiteSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DracoBerserkerOfTheTenyi,     DracoBerserkerOfTheTenyiSpSummon);

            AddExecutor(ExecutorType.SpSummon, Level10SpSummonCheckInit);
            AddExecutor(ExecutorType.SpSummon, Level10SpSummonCheckCount);
            AddExecutor(ExecutorType.SpSummon, Level10SpSummonCheckDecide);
            AddExecutor(ExecutorType.SpSummon, Level10SpSummonCheckFinal);

            // startup effect
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulOfMoYe,   SwordsoulOfMoYeActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulOfTaia,   SwordsoulOfTaiaActivate);

            // summon
            AddExecutor(ExecutorType.Activate, TenyiForShamanSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.IncredibleEcclesiaTheVirtuous, IncredibleEcclesiaTheVirtuousSpSummon);
            AddExecutor(ExecutorType.Summon, CardId.SwordsoulOfMoYe,                 SwordsoulOfMoYeSummon);
            AddExecutor(ExecutorType.Summon, CardId.IncredibleEcclesiaTheVirtuous,   IncredibleEcclesiaTheVirtuousSummon);
            AddExecutor(ExecutorType.Summon, CardId.SwordsoulOfTaia,                 SwordsoulOfTaiaSummon);

            // activate
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulStrategistLongyuan, SwordsoulStrategistLongyuanActivate);
            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires,               PotOfDesiresActivate);
            AddExecutor(ExecutorType.Activate, CardId.ShamanOfTheTenyi,  ShamanOfTheTenyiActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSacredSummit,       SwordsoulSacredSummitActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenyiSpirit_Vishuda,         TenyiSpirit_VishudaActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenyiSpirit_Ashuna,          TenyiSpirit_AshunaActivate);
            AddExecutor(ExecutorType.Activate, CardId.TenyiSpirit_Adhara,          TenyiSpirit_AdharaActivate);
            AddExecutor(ExecutorType.Activate, TenyiForBlackoutSpSummon);

            // other
            AddExecutor(ExecutorType.SpSummon, CardId.GeomathmechFinalSigma, GeomathmechFinalSigmaSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PsychicEndPunisher,    PsychicEndPunisherSpSummon);
            AddExecutor(ExecutorType.Summon, TunerForSynchroSummon);
            AddExecutor(ExecutorType.Summon, WyrmForBlackoutSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ShamanOfTheTenyi,   ShamanOfTheTenyiSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.MonkOfTheTenyi,     MonkOfTheTenyiSpSummon);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, CardId.PsychicEndPunisher, PsychicEndPunisherActivate);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodeYangZing = 0x9e;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeTenyi = 0x12c;
        const int SetcodeSwordsoul = 0x16b;
        const int SetcodeFloowandereeze = 0x16d;
        List<int> normalCounterList = new List<int>
        {
            _CardId.AshBlossom, CardId.BaronneDeFleur, 27548199, 4280258, 53262004
        };
        List<int> notToNegateIdList = new List<int>{
            58699500
        };
        const int hintTimingMainEnd = 0x4;
        const int hintReplaceDestroy = 96;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.IncredibleEcclesiaTheVirtuous,
                                _CardId.AshBlossom, _CardId.MaxxC, _CardId.EffectVeiler, CardId.SwordsoulEmergence, _CardId.InfiniteImpermanence }},
            {2, new List<int> { CardId.TenyiSpirit_Ashuna, _CardId.PotOfDesires, _CardId.CalledByTheGrave, CardId.SwordsoulBlackout }},
            {1, new List<int> { CardId.NibiruThePrimalBeing, CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Adhara, CardId.SwordsoulSacredSummit,
                                CardId.CrossoutDesignator }},
        };

        Dictionary<int, int> calledbytheGraveCount = new Dictionary<int, int>();
        List<int> CrossoutDesignatorTargetList = new List<int>();
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        List<int> infiniteImpermanenceList = new List<int>();

        bool summoned = false;
        bool onlyWyrmSpSummon = false;
        List<int> activatedCardIdList = new List<int>();
        List<int> canSpSummonLevel10IdList = new List<int>();

        List<ClientCard> effectUsedBaronneDeFleurList = new List<ClientCard>();
        List<ClientCard> currentNegateMonsterList = new List<ClientCard>();

        /// <summary>
        /// Shuffle List<ClientCard> and return a random-order card list
        /// </summary>
        public List<ClientCard> ShuffleCardList(List<ClientCard> list)
        {
            List<ClientCard> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(result.Count);
                int nextIndex = (index + Program.Rand.Next(result.Count - 1)) % result.Count;
                ClientCard tempCard = result[index];
                result[index] = result[nextIndex];
                result[nextIndex] = tempCard;
            }
            return result;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            List<ClientCard> floodagateList = Enemy.GetMonsters().Where(c => c?.Data != null &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count() > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count() > 0)
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count() > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            if (attack == 0)
                attack = Util.GetBestAttack(Bot);
            List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                .Where(card => card.GetDefensePower() >= attack && card.IsAttack() && (!canBeTarget || !card.IsShouldNotBeTarget())).ToList();
            if (betterList.Count() > 0)
            {
                betterList.Sort(CardContainer.CompareCardAttack);
                betterList.Reverse();
                return betterList[0];
            }
            return null;
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreNormalSpell = false)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count() > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                resultList.AddRange(floodagateList);
            }
            
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count() > 0)
            {
                resultList.AddRange(ShuffleCardList(problemEnemySpellList));
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count() > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                resultList.AddRange(dangerList);
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count() > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                resultList.AddRange(invincibleList);
            }

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().ToList();
            if (enemyMonsters.Count() > 0)
            {
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach(ClientCard target in enemyMonsters)
                {
                    if (target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                        || (target.HasType(CardType.Link) && target.LinkCount >= 2) )
                    {
                        if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) 
                        {
                            if (!resultList.Contains(target))
                            {
                                resultList.Add(target);
                            }
                        }
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup()
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous)).ToList();
            if (spells.Count() > 0 && !ignoreNormalSpell)
            {
                resultList.AddRange(ShuffleCardList(spells));
            }

            return resultList;
        }
        
        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count() > 0 && !onlyFaceup)
                return ShuffleCardList(monsters)[0];

            return null;
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false, bool canBeTarget = false)
        {
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count() > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(card => !(card.IsFaceup() && card.IsCode(_CardId.EvenlyMatched))).ToList();

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() &&
                ecard.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous)).ToList();
            if (faceUpList.Count() > 0)
            {
                return ShuffleCardList(faceUpList)[0];
            }

            if (spells.Count() > 0 && !onlyFaceup)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }

        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false, bool checkGrave = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null)
            {
                return card;
            }

            card = GetBestEnemySpell(onlyFaceup, canBeTarget);
            if (card != null)
            {
                return card;
            }
            
            if (checkGrave && Enemy.Graveyard.Count() > 0)
            {
                List<ClientCard> graveMonsterList = Enemy.Graveyard.GetMatchingCards(c => c.IsMonster()).ToList();
                if (graveMonsterList.Count() > 0)
                {
                    graveMonsterList.Sort(CardContainer.CompareCardAttack);
                    graveMonsterList.Reverse();
                    return graveMonsterList[0];
                }
                return ShuffleCardList(Enemy.Graveyard.ToList())[0];
            }

            return null;
        }

        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleCardList(Enemy.GetSpells()));
            targetList.AddRange(ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFacedown()).ToList()));

            return targetList;
        }

        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeMonsterTarget = false, bool canBeTrapTarget = false)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            if (CheckWhetherNegated(true))
            {
                return resultList;
            }

            // negate before used
            ClientCard target = Enemy.MonsterZone.FirstOrDefault(card => card?.Data != null
                    && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && !card.IsShouldNotBeTarget()
                    && (!canBeMonsterTarget || !card.IsShouldNotBeMonsterTarget()) && (!canBeTrapTarget || !card.IsShouldNotBeSpellTrapTarget())
                    && !currentNegateMonsterList.Contains(card));
            if (target != null)
            {
                resultList.Add(target);
            }

            // negate monster effect on the field
            foreach (ClientCard chainingCard in Duel.CurrentChain)
            {
                if (chainingCard.Location == CardLocation.MonsterZone && chainingCard.Controller == 1 && !chainingCard.IsDisabled()
                && (!canBeMonsterTarget || !chainingCard.IsShouldNotBeMonsterTarget()) && (!canBeTrapTarget || !chainingCard.IsShouldNotBeSpellTrapTarget())
                && !chainingCard.IsShouldNotBeTarget() && !currentNegateMonsterList.Contains(chainingCard))
                {
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Check whether negate opposite's effect and clear flag
        /// </summary>
        public void CheckDeactiveFlag()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard != null && Duel.LastChainPlayer == 1)
            {
                if (lastChainCard.IsCode(_CardId.MaxxC))
                {
                    enemyActivateMaxxC = false;
                }
                if (lastChainCard.IsCode(_CardId.LockBird))
                {
                    enemyActivateLockBird = false;
                }
                if (lastChainCard.IsCode(_CardId.CalledByTheGrave) && !CrossoutDesignatorTargetList.Contains(_CardId.CalledByTheGrave))
                {
                    foreach (ClientCard target in Duel.LastChainTargets)
                    {
                        if (target.IsMonster() && target.Controller == 0 && target.Location == CardLocation.Grave)
                        {
                            calledbytheGraveCount[target.Id] = 0;
                        }
                    }
                }
                if (lastChainCard.Controller == 1 && lastChainCard.Location == CardLocation.MonsterZone)
                {
                    currentNegateMonsterList.Add(lastChainCard);
                }
            }
        }

        /// <summary>
        /// Check negated turn count of id
        /// </summary>
        public int CheckCalledbytheGrave(int id)
        {
            if (!calledbytheGraveCount.ContainsKey(id))
            {
                return 0;
            }
            return calledbytheGraveCount[id];
        }

        /// <summary>
        /// Check remain cards in deck
        /// </summary>
        /// <param name="id">Card's ID</param>
        public int CheckRemainInDeck(int id)
        {
            for (int count = 1; count < 4; ++count)
            {
                if (DeckCountTable[count].Contains(id)) {
                    return Bot.GetRemainingCount(id, count);
                }
            }
            return 0;
        }

        /// <summary>
        /// Whether spell or trap will be negate. If so, return true.
        /// </summary>
        /// <param name="isCounter">is counter trap</param>
        /// <param name="target">check target</param>
        /// <returns></returns>
        public bool CheckSpellWillBeNegate(bool isCounter = false, ClientCard target = null)
        {
            // target default set
            if (target == null) target = Card;
            // won't negate if not on field
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            // negate judge
            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true)) return true;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(CardId.RoyalDecree, true) || Bot.HasInSpellZone(CardId.RoyalDecree, true)) return true;
            }
            if (target.Location == CardLocation.SpellZone && (target.IsSpell() || target.IsTrap()))
            {
                int selfSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) selfSeq = i;
                }
                if (infiniteImpermanenceList.Contains(selfSeq)) {
                    return true;
                }
            }
            // how to get here?
            return false;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        /// <param name="isCounter">check whether card itself is disabled.</param>
        public bool CheckWhetherNegated(bool disablecheck = true)
        {
            if ((Card.IsSpell() || Card.IsTrap()) && CheckSpellWillBeNegate()){
                return true;
            }
            if (CheckCalledbytheGrave(Card.Id) > 0 || CrossoutDesignatorTargetList.Contains(Card.Id)){
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card)))
                {
                    return true;
                }
            }
            if (disablecheck){
                return Card.IsDisabled();
            }
            return false;
        }

        public bool CheckNumber41(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled();
        }

        /// <summary>
        /// Check whether bot is at advantage.
        /// </summary>
        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && 
                (Bot.GetMonsters().Any(card => card.IsFaceup()) || (Duel.Player == 0 && Duel.Turn == 1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether last chain card should be disabled.
        /// </summary>
        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(lastcard.Id)) return false;
            if (Duel.CurrentChain.Count >= 2)
            {
                ClientCard lastlastChainCard = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
                ClientCard lastChainCard = Duel.CurrentChain[Duel.CurrentChain.Count - 1];
                if (lastlastChainCard?.Controller == 0 && lastChainCard?.Controller == 1 && lastChainCard.IsCode(normalCounterList))
                {
                    bool notImportantFlag = lastlastChainCard.Location == CardLocation.Grave
                        && (lastlastChainCard.IsCode(CardId.SwordsoulOfTaia) || lastlastChainCard.IsCode(CardId.SwordsoulOfMoYe) || lastlastChainCard.IsCode(CardId.SwordsoulStrategistLongyuan));
                    notImportantFlag |= lastlastChainCard.IsCode(CardId.PsychicEndPunisher) && Bot.LifePoints < Enemy.LifePoints;
                    if (notImportantFlag)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// check enemy's dangerous card in grave
        /// </summary>
        public List<ClientCard> CheckDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom))).ToList();
            List<int> dangerMonsterIdList = new List<int>{
                99937011, 63542003, CardId.TenyiSpirit_Adhara, CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Vishuda,
                9411399, 28954097, 30680659
            };
            return result;
        }

        /// <summary>
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            // Geomathmech Final Sigma always place on extra monster zone
            if (cardId == CardId.GeomathmechFinalSigma && location == CardLocation.MonsterZone)
            {
                if ((Zones.z5 & available) > 0) return Zones.z5;
                if ((Zones.z6 & available) > 0) return Zones.z6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Id == CardId.PsychicEndPunisher)
                {
                    return CardPosition.FaceUpAttack;
                }
                if (Util.IsTurn1OrMain2())
                {
                    bool turnDefense = false;
                    if (!cardData.HasType(CardType.Synchro) || cardData.Attack <= cardData.Defense)
                    {
                        turnDefense = true;
                    }
                    if (turnDefense)
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                if (Duel.Player == 1)
                {
                    if (!cardData.HasType(CardType.Synchro) || cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                } else if (cardData.HasType(CardType.Synchro))
                {
                    return CardPosition.FaceUpAttack;
                }
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardData.Attack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    return CardPosition.FaceUpDefence;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {   
            if (Util.ChainContainPlayer(1) && hint == HintMsg.Remove && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int botCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
                int oppositeCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
                if (botCount - oppositeCount == min && min == max)
                {
                    Logger.DebugWriteLine("=== Evenly Matched activated.");
                    List<ClientCard> banishList = new List<ClientCard>();
                    List<ClientCard> botMonsters = Bot.GetMonsters().Where(card => !card.HasType(CardType.Token)).ToList();
                    
                    // non-synchro monster
                    List<ClientCard> faceDownMonsters = botMonsters.Where(card => card.IsFacedown()).ToList();
                    banishList.AddRange(faceDownMonsters);
                    List<ClientCard> nonSynchroMonsters = botMonsters.Where(card => !card.HasType(CardType.Synchro) && !banishList.Contains(card)).ToList();
                    nonSynchroMonsters.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(nonSynchroMonsters);

                    // spells
                    List<ClientCard> spells = Bot.GetSpells();
                    banishList.AddRange(ShuffleCardList(spells));

                    // synchro monster
                    List<ClientCard> synchroMonsters = botMonsters.Where(card => card.HasType(CardType.Synchro) && !banishList.Contains(card)).ToList();
                    synchroMonsters.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(synchroMonsters);
 
                    return Util.CheckSelectCount(banishList, cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1) calledbytheGraveCount.Clear();
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;

            List<int> keyList = calledbytheGraveCount.Keys.ToList();
            foreach (int dic in keyList)
            {
                if (calledbytheGraveCount[dic] > 1)
                {
                    calledbytheGraveCount[dic] -= 1;
                }
            }
            CrossoutDesignatorTargetList.Clear();
            infiniteImpermanenceList.Clear();

            summoned = false;
            onlyWyrmSpSummon = false;
            activatedCardIdList.Clear();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;

            if (player == 1)
            {
                if (card.IsCode(_CardId.MaxxC) && CheckCalledbytheGrave(_CardId.MaxxC) == 0 && !CrossoutDesignatorTargetList.Contains(_CardId.MaxxC))
                {
                    enemyActivateMaxxC = true;
                }
                if (card.IsCode(_CardId.LockBird) && CheckCalledbytheGrave(_CardId.LockBird) == 0 && !CrossoutDesignatorTargetList.Contains(_CardId.LockBird))
                {
                    enemyActivateLockBird = true;
                }
                if (card.IsCode(_CardId.InfiniteImpermanence) && !CrossoutDesignatorTargetList.Contains(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == card)
                        {
                            infiniteImpermanenceList.Add(4-i);
                            break;
                        }
                    }
                }
                if (card.IsCode(_CardId.CalledByTheGrave) && !CrossoutDesignatorTargetList.Contains(_CardId.CalledByTheGrave))
                {
                    foreach (ClientCard target in Duel.LastChainTargets)
                    {
                        if (target.IsMonster() && target.Controller == 0 && target.Location == CardLocation.Grave)
                        {
                            calledbytheGraveCount[target.Id] = 2;
                        }
                    }
                }
            }
            base.OnChaining(player, card);
        }

        public override void OnChainEnd()
        {
            currentNegateMonsterList.Clear();
            for (int idx = effectUsedBaronneDeFleurList.Count() - 1; idx >= 0; -- idx)
            {
                ClientCard checkTarget = effectUsedBaronneDeFleurList[idx];
                if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                {
                    effectUsedBaronneDeFleurList.RemoveAt(idx);
                }
            }
            base.OnChainEnd();
        }

        /// <summary>
        /// Select spell/trap's place randomly to avoid InfiniteImpermanence and so on.
        /// </summary>
        /// <param name="card">Card to set(default current card)</param>
        /// <param name="avoidImpermanence">Whether need to avoid InfiniteImpermanence</param>
        /// <param name="avoidList">Whether need to avoid set in this place</param>
        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(list.Count);
                int nextIndex = (index + Program.Rand.Next(list.Count - 1)) % list.Count;
                int tempInt = list[index];
                list[index] = list[nextIndex];
                list[nextIndex] = tempInt;
            }
            if (avoidImpermanence && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled()))
            {
                foreach (int seq in list)
                {
                    ClientCard enemySpell = Enemy.SpellZone[4 - seq];
                    if (enemySpell != null && enemySpell.IsFacedown()) continue;
                    int zone = (int)System.Math.Pow(2, seq);
                    AI.SelectPlace(zone);
                    return;
                }
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                AI.SelectPlace(zone);
                return;
            }
            AI.SelectPlace(0);
        }


        public bool NibiruThePrimalBeingActivate()
        {
            if (Duel.Player == 0 || Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasType(CardType.Synchro)))
            {
                return false;
            }
            
            if (Util.GetBestAttack(Enemy) > Util.GetBestAttack(Bot))
            {
                // end main phase
                if ((CurrentTiming & hintTimingMainEnd) != 0)
                {
                    SelectNibiruPosition();
                    return true;
                }
                
                // avoid Baronne de Fleur
                List<ClientCard> tunerList = Enemy.GetMonsters().Where(card => card.IsFaceup() && card.IsTuner() && !card.HasType(CardType.Xyz | CardType.Link)).ToList();
                List<ClientCard> nonTunerList = Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsTuner() && !card.HasType(CardType.Xyz | CardType.Link)).ToList();
                foreach (ClientCard tuner in tunerList)
                {
                    foreach (ClientCard nonTuner in nonTunerList)
                    {
                        if (tuner.Level + nonTuner.Level == 10)
                        {
                            SelectNibiruPosition();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void SelectNibiruPosition()
        {
            int totalAttack = Bot.GetMonsters().Where(card => card.IsFaceup()).Sum(m => (int?)m.Attack) ?? 0;
            totalAttack += Enemy.GetMonsters().Where(card => card.IsFaceup()).Sum(m => (int?)m.Attack) ?? 0;
            Logger.DebugWriteLine("Nibiru token attack: " + totalAttack.ToString());
            if (totalAttack >= 3000)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                AI.SelectPosition(CardPosition.FaceUpDefence);
            } else {
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectPosition(CardPosition.FaceUpAttack);
            }
        }

        public bool TenyiSpirit_AshunaActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Ashuna, 0))
            {
                // special summon
                if (TenyiSpSummonForTaiaCheck() || Level7TenyiSpSummonCheck())
                {
                    return true;
                }

            } else if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Ashuna, 1) && Card.Location == CardLocation.Grave && CheckCalledbytheGrave(Card.Id) == 0)
            {
                // deck summon

                // trigger blackout
                if (Bot.HasInHandOrInSpellZone(CardId.SwordsoulBlackout))
                {
                    List<ClientCard> blackoutTarget = Bot.GetMonsters().Where(card => card.IsFaceup()
                       && !card.HasType(CardType.Synchro) && card.HasRace(CardRace.Wyrm)).ToList();
                    if (blackoutTarget.Count() == 0)
                    {
                        AI.SelectCard(CardId.TenyiSpirit_Adhara, CardId.TenyiSpirit_Vishuda);
                        onlyWyrmSpSummon = true;
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                // for level8/10 synchro
                List<int> tunerIdList = new List<int>{_CardId.AshBlossom, _CardId.EffectVeiler, CardId.TenyiSpirit_Adhara};
                bool hasTuner = Bot.GetMonsters().Any(card => card.IsFaceup() && card.IsCode(tunerIdList));
                hasTuner |= !summoned && Bot.HasInHand(tunerIdList);
                if (hasTuner && CheckRemainInDeck(CardId.TenyiSpirit_Vishuda) > 0)
                {
                    AI.SelectCard(CardId.TenyiSpirit_Ashuna);
                    onlyWyrmSpSummon = true;
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                if (Bot.HasInMonstersZone(CardId.TenyiSpirit_Ashuna, false, false, true) && CheckRemainInDeck(CardId.TenyiSpirit_Adhara) > 0)
                {
                    AI.SelectCard(CardId.TenyiSpirit_Adhara);
                    onlyWyrmSpSummon = true;
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            return false;
        }

        public bool TenyiSpirit_VishudaActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Vishuda, 0))
            {
                // special summon
                if (TenyiSpSummonForTaiaCheck() || Level7TenyiSpSummonCheck())
                {
                    return true;
                }

            } else if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Vishuda, 1) && Card.Location == CardLocation.Grave && CheckCalledbytheGrave(Card.Id) == 0)
            {
                // bounce
                List<ClientCard> dangerList = GetProblematicEnemyCardList(true, true);
                if (dangerList.Count() > 0)
                {
                    AI.SelectCard(dangerList);
                    return true;
                }
            }
            return false;
        }

        public bool Level7TenyiSpSummonCheck()
        {
            List<int> advanceSummonCheckList = new List<int>{CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia, CardId.IncredibleEcclesiaTheVirtuous};
            List<int> tunerList = new List<int>{CardId.TenyiSpirit_Adhara, _CardId.EffectVeiler, _CardId.AshBlossom};
            if (!summoned && !Bot.HasInHand(advanceSummonCheckList) && Bot.HasInHand(tunerList))
            {
                return true;
            }
            if (Bot.HasInExtra(CardId.PsychicEndPunisher) && Bot.HasInMonstersZone(CardId.SwordsoulToken) && !onlyWyrmSpSummon)
            {
                return true;
            }

            return false;
        }

        public bool SwordsoulStrategistLongyuanActivate()
        {
            // damage effect
            if (Card.Location != CardLocation.Hand)
            {
                return true;
            }

            // special summon token
            if (CheckWhetherNegated() || (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2()))
            {
                return false;
            }
            List<int> discardIdList = new List<int>();

            // discard Taia to SS
            if (CheckAtAdvantage())
            {
                if (Bot.HasInHand(CardId.SwordsoulSacredSummit) && Bot.HasInHand(CardId.SwordsoulOfTaia)
                    && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && !activatedCardIdList.Contains(CardId.SwordsoulSacredSummit))
                {
                    discardIdList.Add(CardId.SwordsoulOfTaia);
                }
            }

            // discard tenyi
            if (discardIdList.Count() == 0)
            {
                List<int> tenyiList = new List<int>{CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Adhara};
                foreach (int tenyiId in tenyiList)
                {
                    if (Bot.HasInHand(tenyiId))
                    {
                        discardIdList.Add(tenyiId);
                    }
                }
            }

            // discard dump card
            if (discardIdList.Count() == 0)
            {
                List<int> checkIdList = new List<int>{
                    CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.SwordsoulBlackout, CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulEmergence
                };
                foreach (int checkId in checkIdList)
                {
                    if (Bot.Hand.Count(card => card != Card && card.IsCode(checkIdList)) > 1)
                    {
                        discardIdList.Add(checkId);
                    }
                }
            }

            // discard card
            if (discardIdList.Count() == 0)
            {
                List<int> checkIdList = new List<int>{
                    CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.SwordsoulBlackout, CardId.SwordsoulStrategistLongyuan,
                    CardId.SwordsoulSacredSummit, CardId.SwordsoulEmergence
                };
                foreach (int checkId in checkIdList)
                {
                    if (Bot.Hand.Count(card => card != Card && card.IsCode(checkIdList)) >= 1)
                    {
                        discardIdList.Add(checkId);
                    }
                }
            }

            if (discardIdList.Count() > 0)
            {
                AI.SelectCard(discardIdList);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            return false;
        }

        public bool SwordsoulOfTaiaActivate()
        {
            // send to GY effect
            if (Card.Location != CardLocation.MonsterZone)
            {
                List<int> sendToGYTarget = new List<int>();

                // send Mo Ye to SS
                if (!Bot.HasInGraveyard(CardId.SwordsoulOfMoYe) && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0)
                {
                    bool sendMoYe = false;
                    // baxia
                    if (Bot.HasInMonstersZone(CardId.BaxiaBrightnessOfTheYangZing, true, false, true
                    && !activatedCardIdList.Contains(CardId.BaxiaBrightnessOfTheYangZing)))
                    {
                        sendMoYe = true;
                        
                    } 
                    if (Bot.HasInHand(CardId.SwordsoulSacredSummit) && !activatedCardIdList.Contains(CardId.SwordsoulSacredSummit)) {
                        if (Bot.Hand.Any(card => card.Id != CardId.SwordsoulSacredSummit && (card.HasSetcode(SetcodeSwordsoul) || card.HasRace(CardRace.Wyrm))))
                        {
                            sendMoYe = true;
                        }
                    }

                    if (sendMoYe)
                    {
                        sendToGYTarget.Add(CardId.SwordsoulOfMoYe);
                    }
                }
                
                // send Tenyi
                List<int> checkTenyiList = new List<int> {CardId.TenyiSpirit_Adhara, CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna};
                foreach (int id in checkTenyiList)
                {
                    if (CheckRemainInDeck(id) > 0)
                    {
                        sendToGYTarget.Add(id);
                    }
                }

                if (sendToGYTarget.Count() > 0)
                {
                    AI.SelectCard(sendToGYTarget);
                    return true;
                }
                return false;
            }
            
            // special summon token
            // ignore negate check if blackout in GY
            if (Bot.HasInGraveyard(CardId.SwordsoulBlackout) && !activatedCardIdList.Contains(CardId.SwordsoulBlackout))
            {
                AI.SelectCard(CardId.SwordsoulBlackout);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            if (CheckWhetherNegated())
            {
                return false;
            }
            List<int> banishIdList = new List<int>();

            List<int> checkIdList = new List<int>{
                CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulEmergence, CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.MonkOfTheTenyi,
                CardId.ShamanOfTheTenyi, CardId.SwordsoulSacredSummit, CardId.SwordsoulGrandmaster_Chixiao, CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Vishuda,
                CardId.SwordsoulSinisterSovereign_QixingLongyuan, CardId.SwordsoulSupremeSovereign_Chengying, CardId.DracoBerserkerOfTheTenyi,
                CardId.TenyiSpirit_Adhara
            };

            // dump check
            foreach (int checkId in checkIdList)
            {
                if (Bot.Graveyard.Count(card => card.IsCode(checkId)) > 1)
                {
                    banishIdList.Add(checkId);
                }
            }

            // priority check
            if (banishIdList.Count() == 0)
            {
                foreach (int checkId in checkIdList)
                {
                    if (Bot.HasInGraveyard(checkId))
                    {
                        banishIdList.Add(checkId);
                    }
                }
            }

            if (banishIdList.Count() > 0)
            {
                AI.SelectCard(banishIdList);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            return false;
        }
        
        public bool SwordsoulOfTaiaSummon()
        {
            if (Bot.HasInGraveyard(CardId.SwordsoulBlackout))
            {
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && !activatedCardIdList.Contains(CardId.SwordsoulBlackout))
                {
                    summoned = true;
                    return true;
                }
            }
            if (SummonLevel4ForSynchro())
            {
                summoned = true;
                return true;
            }
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (SwordsoulOfTaiaEffectCheck() && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia))
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool SwordsoulOfTaiaEffectCheck(ClientCard exceptTarget = null)
        {
            if (exceptTarget == null)
            {
                exceptTarget = Card;
            }
            return Bot.Graveyard.Count(card => card != exceptTarget && (card.HasSetcode(SetcodeSwordsoul) || card.HasRace(CardRace.Wyrm))) > 0;
        }

        public bool SwordsoulOfMoYeActivate()
        {
            // draw effect
            if (Card.Location != CardLocation.MonsterZone)
            {
                return true;
            }

            // special summon token
            if (CheckWhetherNegated())
            {
                return false;
            }
            List<ClientCard> revealList = Bot.Hand.Where(card => card.HasSetcode(SetcodeSwordsoul) || card.HasRace(CardRace.Wyrm)).ToList();
            if (revealList.Count() > 0)
            {
                revealList = ShuffleCardList(revealList);
                AI.SelectCard(revealList);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool SwordsoulOfMoYeSummon()
        {
            if (SummonLevel4ForSynchro())
            {
                summoned = true;
                return true;
            }
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (SwordsoulOfMoYeEffectCheck() && !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe))
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool SwordsoulOfMoYeEffectCheck(List<ClientCard> exceptList = null)
        {
            if (exceptList == null)
            {
                exceptList = new List<ClientCard>{Card};
            }
            return Bot.Hand.Count(card => !exceptList.Contains(card) && (card.HasSetcode(SetcodeSwordsoul) || card.HasRace(CardRace.Wyrm))) > 0;
        }

        public bool SummonLevel4ForSynchro()
        {
            bool hasNonTuner = Bot.GetMonsters().Any(card => card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link) && !card.IsTuner());
            if (hasNonTuner)
            {
                return false;
            }
            List<ClientCard> tunerList = Bot.GetMonsters().Where(card =>
                card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link) && card.IsTuner()).ToList();
            if (tunerList.Count() > 0)
            {
                foreach (ClientCard tuner in tunerList)
                {
                    int checkLevel = tuner.Level + 4;
                    if (Bot.ExtraDeck.Any(card => card.HasType(CardType.Synchro) && card.Level == checkLevel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IncredibleEcclesiaTheVirtuousActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            if (Duel.Player == 0 && !CheckWhetherNegated())
            {
                bool canActivateMoye = !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0 && SwordsoulOfMoYeEffectCheck();
                bool canActivateTaia = !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0 && SwordsoulOfTaiaEffectCheck();
                if (canActivateMoye && !summoned && !Bot.HasInHand(CardId.SwordsoulOfMoYe))
                {
                    AI.SelectCard(CardId.SwordsoulOfMoYe);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                if (canActivateTaia && !summoned && !Bot.HasInHand(CardId.SwordsoulOfTaia))
                {
                    AI.SelectCard(CardId.SwordsoulOfTaia);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                if (canActivateMoye)
                {
                    AI.SelectCard(CardId.SwordsoulOfMoYe);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                if (canActivateTaia)
                {
                    AI.SelectCard(CardId.SwordsoulOfTaia);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }

            return false;
        }

        public bool IncredibleEcclesiaTheVirtuousSummon()
        {
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (SwordsoulOfMoYeSummon() && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0)
            {
                summoned = true;
                return true;
            }
            if (SwordsoulOfTaiaSummon() && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0)
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool IncredibleEcclesiaTheVirtuousSpSummon()
        {
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
            {
                return false;
            }

            return true;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (CheckAtAdvantage() && Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                return false;
            }
            if (DefaultAshBlossomAndJoyousSpring())
            {
                CheckDeactiveFlag();
                return true;
            }
            return false;
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }

        public bool EffectVeilerActivate()
        {
            if (CheckWhetherNegated(true)) return false;

            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true);
            if (shouldNegateList.Count() > 0)
            {
                ClientCard target = shouldNegateList[0];
                currentNegateMonsterList.Add(target);
                AI.SelectCard(target);
                return true;
            }

            return false;
        }

        public bool TunerForSynchroSummon()
        {
            if (!Card.IsCode(_CardId.AshBlossom) && !Card.IsCode(CardId.TenyiSpirit_Adhara) && !Card.IsCode(_CardId.EffectVeiler))
            {
                return false;
            }
            // taia check
            if (Bot.HasInExtra(CardId.MonkOfTheTenyi) && Bot.HasInHand(CardId.SwordsoulOfTaia)
                && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
            {
                return false;
            }

            // already has tuner, skip (maybe affected by Dimensional Barrier)
            if (Bot.GetMonsters().Any(card => card.IsFaceup() && card.IsTuner()))
            {
                return false;
            }

            // level10 check
            List<int> checkOnField = new List<int>{CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna};
            bool hasLevel7NonTuner = Bot.GetMonsters().Any(card => card.IsFaceup() && checkOnField.Contains(card.Id) && !card.IsTuner());
            if (hasLevel7NonTuner)
            {
                int totalLevel = Card.Level + 7;
                if (Bot.ExtraDeck.Any(card => card.HasType(CardType.Synchro) && card.Level == totalLevel && (!onlyWyrmSpSummon || card.HasRace(CardRace.Wyrm))))
                {
                    summoned = true;
                    return true;
                }
            }

            List<ClientCard> checkNonTuner = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.IsTuner()).ToList();
            checkNonTuner.Sort(CardContainer.CompareCardAttack);
            // level7 check
            if (Bot.HasInExtra(CardId.YaziEvilOfTheYangZing) && GetProblematicEnemyCardList(true, true).Count() > 0)
            {
                foreach (ClientCard checkCard in checkNonTuner)
                {
                    if (Card.Level + checkCard.Level == 7)
                    {
                        summoned = true;
                        return true;
                    }
                }
            }
            // level 11 check
            if (Bot.HasInExtra(CardId.PsychicEndPunisher))
            {
                foreach (ClientCard checkCard in checkNonTuner)
                {
                    if ((checkCard.IsDisabled() || !checkCard.HasType(CardType.Synchro)) && (Card.Level + checkCard.Level == 11))
                    {
                        summoned = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool WyrmForBlackoutSummon()
        {
            if (Card.Level > 4 || !Card.HasRace(CardRace.Wyrm))
            {
                return false;
            }
            if (Bot.HasInHandOrInSpellZone(CardId.SwordsoulBlackout) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasRace(CardRace.Wyrm)))
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool TenyiSpirit_AdharaActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Adhara, 0))
            {
                // special summon
                if (TenyiSpSummonForTaiaCheck())
                {
                    return true;
                }

            }
            else if (ActivateDescription == Util.GetStringId(CardId.TenyiSpirit_Adhara, 1) && Card.Location == CardLocation.Grave && CheckCalledbytheGrave(Card.Id) == 0)
            {
                // recycle
                if (!activatedCardIdList.Contains(CardId.SwordsoulStrategistLongyuan) && SwordsoulOfMoYeEffectCheck()
                    && Bot.HasInBanished(CardId.SwordsoulStrategistLongyuan))
                {
                    AI.SelectCard(CardId.SwordsoulStrategistLongyuan);
                    return true;
                }
                if (!summoned)
                {
                    if (!activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && SwordsoulOfMoYeEffectCheck()
                        && Bot.HasInBanished(CardId.SwordsoulOfMoYe))
                    {
                        AI.SelectCard(CardId.SwordsoulOfMoYe);
                        return true;
                    }
                    if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && SwordsoulOfTaiaEffectCheck()
                        && Bot.HasInBanished(CardId.SwordsoulOfTaia))
                    {
                        AI.SelectCard(CardId.SwordsoulOfTaia);
                        return true;
                    }
                }
                List<int> recycleList = new List<int>{CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna};
                foreach (int recycle in recycleList)
                {
                    if (Bot.HasInBanished(recycle))
                    {
                        AI.SelectCard(recycle);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TenyiSpSummonForTaiaCheck()
        {
            if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
            {
                bool hasTaia = (!summoned && Bot.HasInHand(CardId.SwordsoulOfTaia)) || Bot.HasInMonstersZone(CardId.SwordsoulOfTaia);
                bool noTargetInGrave = !Bot.Graveyard.Any(card => card.HasRace(CardRace.Wyrm) || card.HasSetcode(SetcodeSwordsoul));
                bool hasInExtra = Bot.HasInExtra(CardId.MonkOfTheTenyi);
                bool notLongyuan = activatedCardIdList.Contains(CardId.SwordsoulStrategistLongyuan) || !Bot.HasInHand(CardId.SwordsoulStrategistLongyuan);

                if (hasTaia && noTargetInGrave && hasInExtra && notLongyuan)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TenyiForShamanSpSummon()
        {
            List<int> checkEffectDesc = new List<int>{
                Util.GetStringId(CardId.TenyiSpirit_Adhara, 0), Util.GetStringId(CardId.TenyiSpirit_Vishuda, 0),
                Util.GetStringId(CardId.TenyiSpirit_Ashuna, 0)
            };
            if (!checkEffectDesc.Contains(ActivateDescription) || summoned || !Bot.HasInExtra(CardId.ShamanOfTheTenyi)
            || (CheckAtAdvantage() && enemyActivateMaxxC))
            {
                return false;
            }
            ClientCard toSummonMoye = Bot.Hand.FirstOrDefault(card => card.IsCode(CardId.SwordsoulOfMoYe));
            if (toSummonMoye == null)
            {
                return false;
            }
            List<ClientCard> notRevealCheckList = new List<ClientCard>{Card, toSummonMoye};
            if (!SwordsoulOfMoYeEffectCheck(notRevealCheckList) || activatedCardIdList.Contains(CardId.SwordsoulOfMoYe))
            {
                return false;
            }
            if (activatedCardIdList.Contains(CardId.SwordsoulOfTaia) || !Bot.HasInHandOrInGraveyard(CardId.SwordsoulOfTaia))
            {
                return false;
            }

            return true;
        }

        public bool TenyiForBlackoutSpSummon()
        {
            List<int> checkEffectDesc = new List<int>{
                Util.GetStringId(CardId.TenyiSpirit_Adhara, 0), Util.GetStringId(CardId.TenyiSpirit_Vishuda, 0),
                Util.GetStringId(CardId.TenyiSpirit_Ashuna, 0)
            };
            if (!checkEffectDesc.Contains(ActivateDescription))
            {
                return false;
            }
            if (CheckAtAdvantage() && enemyActivateMaxxC)
            {
                return false;
            }
            if (Bot.HasInHandOrInSpellZone(CardId.SwordsoulBlackout) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasRace(CardRace.Wyrm)))
            {
                return true;
            }
            return false;
        }

        public bool PotOfDesiresActivate()
        {
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (CheckAtAdvantage())
            {
                bool result = Bot.Deck.Count() >= 15;
                if (result)
                {
                    SelectSTPlace(null, true);
                }
                return result;
            }
            SelectSTPlace(null, true);
            return true;
        }

        public bool SwordsoulEmergenceActivate()
        {
            if (Card.Location == CardLocation.Removed)
            {
                return SwordsoulSpellBanishedEffect();
            }

            // Mo Ye
            if (!Bot.HasInHand(CardId.SwordsoulOfMoYe) && !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe)
                && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0 && SwordsoulOfMoYeEffectCheck())
            {
                AI.SelectCard(CardId.SwordsoulOfMoYe);
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }

            // Taia
            if (!Bot.HasInHand(CardId.SwordsoulOfTaia) && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia)
                && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0 && SwordsoulOfTaiaEffectCheck())
            {
                AI.SelectCard(CardId.SwordsoulOfTaia);
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }

            // Longyuan
            if (!Bot.HasInHand(CardId.SwordsoulStrategistLongyuan) && !activatedCardIdList.Contains(CardId.SwordsoulStrategistLongyuan)
                && CheckRemainInDeck(CardId.SwordsoulStrategistLongyuan) > 0 && SwordsoulOfMoYeEffectCheck())
            {
                AI.SelectCard(CardId.SwordsoulStrategistLongyuan);
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }

            // dump check
            if (!Bot.HasInHand(CardId.SwordsoulOfMoYe) && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0 && SwordsoulOfMoYeEffectCheck())
            {
                AI.SelectCard(CardId.SwordsoulOfMoYe);
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }
            List<int> checkIdList = new List<int>{CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.SwordsoulStrategistLongyuan};
            foreach (int checkId in checkIdList)
            {
                if (CheckRemainInDeck(checkId) > 0)
                {
                    AI.SelectCard(checkId);
                    activatedCardIdList.Add(Card.Id);
                    SelectSTPlace(null, true);
                    return true;
                }
            }

            return false;
        }

        public bool SwordsoulSacredSummitActivate()
        {
            if (Card.Location == CardLocation.Removed)
            {
                return SwordsoulSpellBanishedEffect();
            }
            if (CheckAtAdvantage())
            {
                if (enemyActivateMaxxC && Util.IsTurn1OrMain2())
                {
                    return false;
                }
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && Bot.HasInGraveyard(CardId.SwordsoulOfMoYe)
                    && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0 && SwordsoulOfMoYeEffectCheck())
                {
                    AI.SelectCard(CardId.SwordsoulOfMoYe);
                    activatedCardIdList.Add(Card.Id);
                    SelectSTPlace(null, true);
                    return true;
                }
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
                {
                    ClientCard taia = Bot.Graveyard.FirstOrDefault(card => card.IsCode(CardId.SwordsoulOfTaia));
                    if (taia != null && SwordsoulOfTaiaEffectCheck(taia))
                    {
                        AI.SelectCard(CardId.SwordsoulOfTaia);
                        activatedCardIdList.Add(Card.Id);
                        SelectSTPlace(null, true);
                        return true;
                    }
                }
            }
            bool controlSynchro = Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasType(CardType.Synchro));

            List<ClientCard> rebornTargetList = Bot.Graveyard.Where(card =>
                card.IsMonster() && (card.HasSetcode(SetcodeSwordsoul) || (controlSynchro && card.HasRace(CardRace.Wyrm)))).ToList();
            rebornTargetList.Sort(CardContainer.CompareCardAttack);
            rebornTargetList.Reverse();

            if (rebornTargetList.Count() > 0)
            {
                ClientCard rebornTarget = rebornTargetList[0];
                if (rebornTarget.IsCode(CardId.SwordsoulOfMoYe) && (activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) || !SwordsoulOfMoYeEffectCheck()))
                {
                    return false;
                }
                if (rebornTarget.IsCode(CardId.SwordsoulOfTaia) && activatedCardIdList.Contains(CardId.SwordsoulOfTaia))
                {
                    return false;
                }
                AI.SelectCard(rebornTargetList);
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }

        public bool SwordsoulSpellBanishedEffect()
        {
            // TODO
            return false;
        }

        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (CheckAtAdvantage() && Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().Id;
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0 || CrossoutDesignatorTargetList.Contains(code)) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
                    {
                        return false;
                    }
                    if (Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code)) != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(code);
                        calledbytheGraveCount[code] = 2;
                        CheckDeactiveFlag();
                        return true;
                    }
                }
                
                // banish target
                foreach (ClientCard cards in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(cards) && cards.IsMonster())
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        int code = cards.Id;
                        AI.SelectCard(cards);
                        calledbytheGraveCount[code] = 2;
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemyMonsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemyMonsters.Count() > 0)
                    {
                        enemyMonsters.Sort(CardContainer.CompareCardAttack);
                        enemyMonsters.Reverse();
                        int code = enemyMonsters[0].Id;
                        AI.SelectCard(code);
                        calledbytheGraveCount[code] = 2;
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0) {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                calledbytheGraveCount[code] = 2;
                return true;
            }

            return false;
        }

        public bool CrossoutDesignatorActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            // negate 
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().Id;
                int alias = Util.GetLastChainCard().Alias;
                if (alias != 0 && alias - code < 10) code = alias;
                if (code == 0) return false;
                if (CheckCalledbytheGrave(code) > 0 || CrossoutDesignatorTargetList.Contains(code)) return false;
                if (CheckRemainInDeck(code) > 0)
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    AI.SelectAnnounceID(code);
                    CrossoutDesignatorTargetList.Add(code);
                    CheckDeactiveFlag();
                    return true;
                }
            }
            return false;
        }

        public bool InfiniteImpermanenceActivate()
        {
            if (CheckWhetherNegated()) return false;

            ClientCard LastChainCard = Util.GetLastChainCard();

            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ( (this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || Util.IsChainTarget(Card)
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    CheckDeactiveFlag();
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    if (target != null)
                    {
                        AI.SelectCard(target);
                    } else {
                        AI.SelectCard(Enemy.GetMonsters());
                    }
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }
            
            // negate monster
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(false, true);
            if (shouldNegateList.Count() > 0)
            {
                ClientCard negateTarget = shouldNegateList[0];
                currentNegateMonsterList.Add(negateTarget);

                if (Card.Location == CardLocation.SpellZone)
                {
                    for (int i = 0; i < 5; ++ i)
                    {
                        if (Bot.SpellZone[i] == Card)
                        {
                            infiniteImpermanenceList.Add(i);
                            break;
                        }
                    }
                }
                if (Card.Location == CardLocation.Hand)
                {
                    SelectSTPlace(Card, true);
                }
                AI.SelectCard(negateTarget);
                return true;
            }

            return false;
        }

        public bool SwordsoulBlackoutActivate()
        {
            // sp summon token
            if (Card.Location == CardLocation.Removed)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            // self destroy targer
            List<ClientCard> selfDestroyList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasRace(CardRace.Wyrm)).ToList();
            selfDestroyList.Sort(CardContainer.CompareCardAttack);
            ClientCard selfDestroyTarget = selfDestroyList[0];
            bool selfTargetIsImportant = selfDestroyTarget.HasType(CardType.Synchro);

            // choose Chengying
            List<ClientCard> chengyingList = Bot.GetMonsters().Where(card =>
                card.IsCode(CardId.SwordsoulSupremeSovereign_Chengying) && card.IsFaceup() && !card.IsDisabled()).ToList();
            if (chengyingList.Count() > 0 && Bot.Graveyard.Count() > 0)
            {
                selfDestroyTarget = chengyingList[0];
                selfTargetIsImportant = false;
            }
            foreach (ClientCard selfCard in selfDestroyList)
            {
                if (Duel.LastChainTargets.Contains(selfCard))
                {
                    selfDestroyTarget = selfCard;
                    selfTargetIsImportant = false;
                }
            }

            // destroy problem card
            List<ClientCard> problemCardList = GetProblematicEnemyCardList(true);
            if (problemCardList.Count() >= 2 && Duel.Player == 1)
            {
                AI.SelectCard(selfDestroyTarget);
                AI.SelectNextCard(problemCardList);
                return true;
            }
            
            List<ClientCard> faceUpEnemyMonsterList = Enemy.GetMonsters().Where(card => card.IsFaceup()).ToList();
            faceUpEnemyMonsterList.Sort(CardContainer.CompareCardAttack);
            faceUpEnemyMonsterList.Reverse();
            if (!selfTargetIsImportant && Duel.Player == 1)
            {
                // destroy multi monster
                if (faceUpEnemyMonsterList.Count() >= 2)
                {
                    AI.SelectCard(selfDestroyTarget);
                    AI.SelectNextCard(GetNormalEnemyTargetList());
                    return true;
                }

                // destroy card in EP
                if (Duel.Phase == DuelPhase.End)
                {
                    AI.SelectCard(selfDestroyTarget);
                    AI.SelectNextCard(GetNormalEnemyTargetList());
                    return true;
                }
            }

            // destroy attack monster
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && faceUpEnemyMonsterList.Count() > 0)
            {
                int botBestAttack = Util.GetBestAttack(Bot);
                int enemyBestAttack = faceUpEnemyMonsterList[0].GetDefensePower();
                if (enemyBestAttack >= botBestAttack)
                {
                    AI.SelectCard(selfDestroyTarget);
                    AI.SelectNextCard(GetNormalEnemyTargetList());
                    return true;
                }
            }

            return false;
        }


        public bool GeomathmechFinalSigmaSpSummon()
        {
            if (Bot.GetMonstersExtraZoneCount() > 0) return false;
            bool hasFloowandereeze = Enemy.GetMonsters().Any(card => card.HasSetcode(SetcodeFloowandereeze));
            hasFloowandereeze |= Enemy.GetSpells().Any(card => card.HasSetcode(SetcodeFloowandereeze));
            hasFloowandereeze |= Enemy.Graveyard.Any(card => card.HasSetcode(SetcodeFloowandereeze));
            hasFloowandereeze |= Enemy.Banished.Any(card => card.HasSetcode(SetcodeFloowandereeze));
            if (hasFloowandereeze)
            {
                AI.SelectMaterials(GetSynchroMaterial(12));
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            return false;
        }

        public bool PsychicEndPunisherSpSummon()
        {
            List<ClientCard> materialList = GetSynchroMaterial(11);
            if (materialList.Count() > 1)
            {
                AI.SelectMaterials(materialList);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            return false;
        }

        /// <summary>
        /// remove level10 summon filter
        /// </summary>
        public bool Level10SpSummonCheckInit()
        {
            canSpSummonLevel10IdList.Clear();
            return false;
        }

        /// <summary>
        /// check which level10 monster can be summoned
        /// </summary>
        public bool Level10SpSummonCheckCount()
        {
            List<int> checkIdList = new List<int>{
                CardId.SwordsoulSupremeSovereign_Chengying, CardId.SwordsoulSinisterSovereign_QixingLongyuan, CardId.BaronneDeFleur
            };
            foreach (int checkId in checkIdList)
            {
                if (Card.IsCode(checkId))
                {
                    canSpSummonLevel10IdList.Add(checkId);
                }
            }
            return false;
        }

        /// <summary>
        /// decide which level10 monster to summon
        /// </summary>
        public bool Level10SpSummonCheckDecide()
        {
            if (canSpSummonLevel10IdList.Count <= 1)
            {
                return false;
            }
            List<int> decideIdList = new List<int>();

            // BaronneDeFleur
            if (canSpSummonLevel10IdList.Contains(CardId.BaronneDeFleur))
            {
                // protect maxxc
                if (Bot.HasInHand(_CardId.MaxxC))
                {
                    canSpSummonLevel10IdList.Clear();
                    canSpSummonLevel10IdList.Add(CardId.BaronneDeFleur);
                    return false;
                }

                // continue to use taia for synchro
                ClientCard taia = Bot.Graveyard.FirstOrDefault(card => card.IsCode(CardId.SwordsoulOfTaia));
                if (taia != null && SwordsoulOfTaiaEffectCheck(taia) && Bot.HasInHand(CardId.SwordsoulSacredSummit))
                {
                    canSpSummonLevel10IdList.Clear();
                    canSpSummonLevel10IdList.Add(CardId.BaronneDeFleur);
                    return false;
                }

                decideIdList.Add(CardId.BaronneDeFleur);
            }

            // QixingLongyuan
            if (canSpSummonLevel10IdList.Contains(CardId.SwordsoulSinisterSovereign_QixingLongyuan))
            {
                if (CheckAtAdvantage())
                {
                    decideIdList.Add(CardId.SwordsoulSinisterSovereign_QixingLongyuan);
                }
            }

            // Chengying
            if (canSpSummonLevel10IdList.Contains(CardId.SwordsoulSupremeSovereign_Chengying))
            {
                int banishCount = Bot.Banished.Count() + Enemy.Banished.Count();
                // use blackout or chixiao to trigger
                bool decideFlag = Bot.HasInHandOrInSpellZone(CardId.SwordsoulBlackout)
                    || Bot.HasInMonstersZone(CardId.SwordsoulGrandmaster_Chixiao, true, false, true);
                if (CheckAtAdvantage())
                {
                    // overkill
                    if (3000 + banishCount * 100 >= Enemy.LifePoints)
                    {
                        decideFlag = true;
                    }
                } else {
                    ClientCard enemyMonster = GetBestEnemyMonster(true);
                    if (enemyMonster != null && decideIdList.Count() == 0)
                    {
                        // for high-power monster
                        if (3000 + banishCount * 200 >= enemyMonster.GetDefensePower())
                        {
                            decideFlag = true;
                        }
                    }
                }

                if (decideFlag)
                {
                    decideIdList.Add(CardId.SwordsoulSupremeSovereign_Chengying);
                }
            }

            if (decideIdList.Count() > 0)
            {
                // if multi selections, select randomly
                canSpSummonLevel10IdList.Clear();
                int index = Program.Rand.Next(decideIdList.Count());
                int lastDecide = decideIdList[index];
                canSpSummonLevel10IdList.Add(lastDecide);
            }
            return false;
        }

        /// <summary>
        /// perform level10 monster's synchro summon
        /// </summary>
        public bool Level10SpSummonCheckFinal()
        {
            if (canSpSummonLevel10IdList.Count() == 1)
            {
                int finalDecideId = canSpSummonLevel10IdList[0];
                if (Card.IsCode(finalDecideId))
                {
                    List<ClientCard> materialList = GetSynchroMaterial(10, Card.IsCode(CardId.SwordsoulSinisterSovereign_QixingLongyuan));
                    if (materialList.Count() > 1)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                    return true;
                }
            }

            return false;
        }

        public bool AdamancipatorRisen_DragiteSpSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.SwordsoulGrandmaster_Chixiao, true))
            {
                return false;
            }
            bool containWaterMonsterInGY = Bot.Graveyard.Any(card => card.IsMonster() && card.HasAttribute(CardAttribute.Water));
            bool canContainWaterInGY = containWaterMonsterInGY;
            canContainWaterInGY |= Bot.GetMonsters().Any(card => card.HasAttribute(CardAttribute.Water) && card.IsFaceup());
            if (!canContainWaterInGY)
            {
                return false;
            }

            SelectLevel8SynchroMaterial(false, !containWaterMonsterInGY);
            return true;
        }

        public bool DracoBerserkerOfTheTenyiSpSummon()
        {
            if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
            {
                return false;
            }

            SelectLevel8SynchroMaterial(true);
            return true;
        }

        public bool SwordsoulGrandmaster_ChixiaoSpSummon()
        {
            if (CheckAtAdvantage() && enemyActivateLockBird)
            {
                return false;
            }
            if (!activatedCardIdList.Contains(CardId.SwordsoulGrandmaster_Chixiao))
            {
                SelectLevel8SynchroMaterial(true);
                return true;
            }

            return false;
        }

        public bool BaxiaBrightnessOfTheYangZingSpSummon()
        {
            if (CheckAtAdvantage())
            {
                return false;
            }

            List<ClientCard> problemList = GetProblematicEnemyCardList(true);
            if (problemList.Count() > 1 && !activatedCardIdList.Contains(CardId.BaxiaBrightnessOfTheYangZing + 1))
            {
                SelectLevel8SynchroMaterial(true);
                return true;
            }
            if (problemList.Count() == 1 && Bot.GetSpellCount() > 0 && !activatedCardIdList.Contains(CardId.BaxiaBrightnessOfTheYangZing + 2))
            {
                bool checkFlag = false;
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && SwordsoulOfMoYeEffectCheck() && Bot.HasInGraveyard(CardId.SwordsoulOfMoYe))
                {
                    checkFlag = true;
                }
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && Bot.HasInGraveyard(CardId.SwordsoulOfTaia))
                {
                    checkFlag = true;
                }
                if (checkFlag)
                {
                    SelectLevel8SynchroMaterial(true);
                    return true;
                }
            }

            return false;
        }

        public void SelectLevel8SynchroMaterial(bool needWyrmNonTuner = false, bool needWaterNonTuner = false)
        {
            List<ClientCard> tunerList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.IsTuner() && card.Level < 8).ToList();
            List<ClientCard> nonTunerList = Bot.GetMonsters().Where(card => card.IsFaceup() && !card.IsTuner() && card.Level < 8
                && (!needWyrmNonTuner || card.HasRace(CardRace.Wyrm) && (!needWaterNonTuner || card.HasAttribute(CardAttribute.Water)))).ToList();
            tunerList.Sort(CardContainer.CompareCardAttack);
            nonTunerList.Sort(CardContainer.CompareCardAttack);

            List<ClientCard> materialList = new List<ClientCard>();
            foreach (ClientCard tuner in tunerList)
            {
                materialList.Clear();
                materialList.Add(tuner);
                if (tuner.Level == 4)
                {
                    // use moye first
                    if (activatedCardIdList.Contains(CardId.SwordsoulOfMoYe))
                    {
                        ClientCard moye = nonTunerList.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulOfMoYe));
                        if (moye != null)
                        {
                            materialList.Add(moye);
                            AI.SelectMaterials(materialList);
                            return;
                        }
                    }

                    // use taia
                    if (activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && !needWaterNonTuner)
                    {
                        ClientCard taia = nonTunerList.GetFirstMatchingCard(card => card.IsCode(CardId.SwordsoulOfTaia));
                        if (taia != null)
                        {
                            materialList.Add(taia);
                            AI.SelectMaterials(materialList);
                            return;
                        }
                    }
                }

                foreach (ClientCard nonTuner in nonTunerList)
                {
                    if (tuner.Level + nonTuner.Level == 8)
                    {
                        materialList.Add(nonTuner);
                        AI.SelectMaterials(materialList);
                        return;
                    }
                }
            }
        }

        public bool YaziEvilOfTheYangZingSpSummon()
        {
            if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0)
            {
                return false;
            }
            bool shouldSummon = GetProblematicEnemyCardList(true, true).Count() > 0;
            shouldSummon |= !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0
                && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0 && SwordsoulOfMoYeEffectCheck();
            shouldSummon |= !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0
                && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0;
            
            if (shouldSummon)
            {
                List<ClientCard> materialList = GetSynchroMaterial(7);
                if (materialList.Count() > 1)
                {
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }

            return false;
        }

        public List<ClientCard> GetSynchroMaterial(int level, bool needWyrmNonTuner = false)
        {
            List<ClientCard> tunerList = Bot.GetMonsters().Where(card =>
                card.IsFaceup() && card.IsTuner() && !card.HasType(CardType.Xyz | CardType.Link)).ToList();
            List<ClientCard> nonTunerList = Bot.GetMonsters().Where(card =>
                card.IsFaceup() && !card.IsTuner() && !card.HasType(CardType.Xyz | CardType.Link) && (!needWyrmNonTuner || card.HasRace(CardRace.Wyrm))).ToList();
            tunerList.Sort(CardContainer.CompareCardAttack);
            nonTunerList.Sort(CardContainer.CompareCardAttack);
            List<ClientCard> selectList = new List<ClientCard>();
            foreach (ClientCard tuner in tunerList)
            {
                selectList.Clear();
                selectList.Add(tuner);
                foreach (ClientCard nonTuner in nonTunerList)
                {
                    if (tuner.Level + nonTuner.Level == level && (nonTuner.IsDisabled() || !nonTuner.HasType(CardType.Synchro)))
                    {
                        selectList.Add(nonTuner);
                        return selectList;
                    }
                }
            }
            selectList.Clear();
            return selectList;
        }

        public bool ShamanOfTheTenyiSpSummon()
        {
            if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
            {
                Logger.DebugWriteLine("[Shaman] advantage & maxxc, skip");
                return false;
            }
            // check extra summon
            List<ClientCard> extraZoneMonsters = Bot.GetMonstersInExtraZone();
            if (extraZoneMonsters.Count() > 0 && extraZoneMonsters.Any(card => card.IsFacedown() || !card.HasType(CardType.Link) || !card.HasRace(CardRace.Wyrm)))
            {
                Logger.DebugWriteLine("[Shaman] extra zone occupied, skip");
                return false;
            }
            // check spsummon target
            bool hasSpSummonTaret = !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0
                && Bot.HasInHandOrInGraveyard(CardId.SwordsoulOfTaia);
            hasSpSummonTaret |= !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0
                && Bot.HasInGraveyard(CardId.SwordsoulOfMoYe) && SwordsoulOfMoYeEffectCheck();
            hasSpSummonTaret |= Bot.GetGraveyardMonsters().Any(card => card.HasType(CardType.Synchro) && card.IsCanRevive() && card.HasRace(CardRace.Wyrm));
            if (!hasSpSummonTaret)
            {
                Logger.DebugWriteLine("[Shaman] no target, skip");
                return false;
            }

            // select material
            List<ClientCard> materialList = new List<ClientCard>(extraZoneMonsters);
            List<ClientCard> mainMonsterZoneMonsters = Bot.GetMonstersInMainZone().Where(card =>
                card.IsFaceup() && !card.HasType(CardType.Synchro) && card.HasRace(CardRace.Wyrm)).ToList();
            mainMonsterZoneMonsters.Sort(CardContainer.CompareCardAttack);
            materialList.AddRange(mainMonsterZoneMonsters);
            if (materialList.Count() >= 2)
            {
                AI.SelectMaterials(materialList.GetRange(0, 2));
                return true;
            }
            
            return false;
        }

        public bool MonkOfTheTenyiSpSummon()
        {
            List<ClientCard> materialList = Bot.GetMonsters().Where(card => 
                card.IsFaceup() && !card.HasType(CardType.Synchro | CardType.Link) && card.HasSetcode(SetcodeTenyi)).ToList();
            if (materialList.Count() > 0)
            {
                materialList.Sort(CardContainer.CompareCardAttack);
                AI.SelectMaterials(materialList);
                return true;
            }
            return false;
        }


        public bool PsychicEndPunisherActivate()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                return true;
            }
            if (Bot.LifePoints <= 1500 || CheckWhetherNegated())
            {
                return false;
            }
            List<ClientCard> selfBanishTarget = Bot.GetMonsters().Where(card => card != Card && (card.IsFacedown() || card.GetDefensePower() <= 1000)).ToList();
            if (selfBanishTarget.Count() == 0)
            {
                return false;
            }
            selfBanishTarget.Sort(CardContainer.CompareCardAttack);
            AI.SelectCard(selfBanishTarget);
            AI.SelectNextCard(GetNormalEnemyTargetList(true));
            return true;
        }

        public bool SwordsoulSupremeSovereign_ChengyingActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SwordsoulSupremeSovereign_Chengying, 0) || ActivateDescription == -1)
            {
                activatedCardIdList.Add(Card.Id);
                List<ClientCard> banishTargetList = Duel.CurrentChain.Where(card => card.Controller == 1 && card.Location == CardLocation.Grave).ToList();
                banishTargetList.AddRange(CheckDangerousCardinEnemyGrave(false));
                if (banishTargetList.Count() > 0)
                {
                    ClientCard graveTarget = banishTargetList[0];
                    Logger.DebugWriteLine("Chengying banish grave: " + graveTarget?.Name);
                }
                List<ClientCard> fieldTargetList = GetNormalEnemyTargetList();
                if (fieldTargetList.Count() > 0)
                {
                    ClientCard fieldTarget = fieldTargetList[0];
                    Logger.DebugWriteLine("Chengying banish field: " + fieldTarget?.Name);
                }
                banishTargetList.AddRange(fieldTargetList);
                AI.SelectCard(banishTargetList);

            } else if (ActivateDescription == hintReplaceDestroy)
            {
                List<int> removeCardIdList = new List<int>{
                    _CardId.CalledByTheGrave, CardId.CrossoutDesignator, _CardId.InfiniteImpermanence, _CardId.AshBlossom,
                    _CardId.MaxxC, _CardId.EffectVeiler, CardId.MonkOfTheTenyi, CardId.ShamanOfTheTenyi, CardId.SwordsoulGrandmaster_Chixiao,
                    CardId.SwordsoulOfTaia, CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulOfMoYe
                };
                AI.SelectCard(removeCardIdList);
            } else 
            {
                Logger.DebugWriteLine("Chengying desc: " + ActivateDescription.ToString());
            }

            return true;
        }

        public bool BaronneDeFleurActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BaronneDeFleur, 1))
            {
                // negate
                if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
                if (Duel.LastChainPlayer == 1)
                {
                    ClientCard lastChainCard = Util.GetLastChainCard();
                    if (CheckAtAdvantage() && lastChainCard.IsCode(_CardId.MaxxC))
                    {
                        return false;
                    }
                    if (Duel.LastChainTargets.Contains(Card) && lastChainCard.IsCode(_CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.BreakthroughSkill))
                    {
                        return false;
                    }
                }
                CheckDeactiveFlag();
                effectUsedBaronneDeFleurList.Add(Card);
                return true;
            } else if (Duel.Phase == DuelPhase.Standby)
            {
                // special summon after effect used
                if (effectUsedBaronneDeFleurList.Contains(Card) && !CheckWhetherNegated(true))
                {
                    if (Duel.Player == 1)
                    {
                        if (!Bot.HasInMonstersZone(CardId.SwordsoulGrandmaster_Chixiao) && Bot.HasInGraveyard(CardId.SwordsoulGrandmaster_Chixiao))
                        {
                            AI.SelectCard(CardId.SwordsoulGrandmaster_Chixiao);
                            return true;
                        }
                    } else if (GetProblematicEnemyCardList(true, true).Count() > 0)
                    {
                        return false;
                    } else if (CheckAtAdvantage()) {
                        if (Bot.ExtraDeck.Any(card => card.IsFacedown() && card.HasType(CardType.Synchro) && card.Level == 8))
                        {
                            if (Bot.HasInGraveyard(CardId.SwordsoulOfMoYe) && SwordsoulOfMoYeEffectCheck() && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0)
                            {
                                AI.SelectCard(CardId.SwordsoulOfMoYe);
                                return true;
                            }
                            if (CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
                            {
                                ClientCard taia = Bot.Graveyard.FirstOrDefault(card => card.IsCode(CardId.SwordsoulOfTaia));
                                if (taia != null && SwordsoulOfTaiaEffectCheck(taia))
                                {
                                    AI.SelectCard(CardId.SwordsoulOfTaia);
                                    return true;
                                }
                            }
                        }
                    }
                }
            } else {
                // destroy
                List<ClientCard> targetList = GetNormalEnemyTargetList();
                if (targetList.Count() > 0)
                {
                    AI.SelectCard(targetList);
                    return true;
                }
            }

            return false;
        }

        public bool SwordsoulSinisterSovereign_QixingLongyuanActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SwordsoulSinisterSovereign_QixingLongyuan, 0))
            {
                // draw
                return true;
            } else if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SwordsoulSinisterSovereign_QixingLongyuan, 1))
            {
                // remove monster
                return true;
            } else if (ActivateDescription == Util.GetStringId(CardId.SwordsoulSinisterSovereign_QixingLongyuan, 2))
            {
                // remove spell/trap
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.Controller == 1)
                {
                    bool activateFlag = DefaultOnBecomeTarget();
                    activateFlag |= Enemy.LifePoints <= 1200;
                    activateFlag |= lastChainCard.HasType(CardType.Continuous | CardType.Equip | CardType.Field | CardType.Pendulum);
                    if (activateFlag)
                    {
                        return true;
                    }
                }
            } else
            {
                Logger.DebugWriteLine("qixinglongyuan desc: " + ActivateDescription);
            }

            return false;
        }

        public bool AdamancipatorRisen_DragiteActivate()
        {
            // bounce
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.AdamancipatorRisen_Dragite, 0))
            {
                if (CheckWhetherNegated())
                {
                    return false;
                }
                if (CheckRemainInDeck(CardId.NibiruThePrimalBeing) > 0 && (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0))
                {
                    AI.SelectCard(GetNormalEnemyTargetList(false));
                    return true;
                }
                return false;
            }
            
            // negate
            if (CheckWhetherNegated())
            {
                return false;
            }

            return true;
        }

        public bool DracoBerserkerOfTheTenyiActivate()
        {
            // do not banish Nibiru
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard != null && lastChainCard.IsCode(CardId.NibiruThePrimalBeing) && lastChainCard.Controller == 1)
            {
                return false;
            }
            return true;
        }

        public bool SwordsoulGrandmaster_ChixiaoActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SwordsoulGrandmaster_Chixiao, 1))
            {
                // negate
                if (CheckWhetherNegated(true)) return false;

                List<ClientCard> negateTargetList = new List<ClientCard>();

                List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true);
                if (shouldNegateList.Count() > 0)
                {
                    ClientCard target = shouldNegateList[0];
                    currentNegateMonsterList.Add(target);
                    negateTargetList.AddRange(shouldNegateList);
                }

                // negate unbreakable monster
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    bool botCanAttack = Bot.GetMonsters().Any(card => card.IsAttack());
                    if (Duel.Player == 0 && botCanAttack)
                    {
                        negateTargetList.AddRange(Enemy.GetMonsters().Where(card => card.IsFaceup() && card.IsMonsterDangerous()).ToList());
                    }
                    if (Duel.Player == 1)
                    {
                        ClientCard enemyMonster = Enemy.BattlingMonster;
                        if (enemyMonster != null && enemyMonster.IsMonsterInvincible())
                        {
                            negateTargetList.Add(enemyMonster);
                        }
                    }
                }

                // trigger Chengying
                if (Bot.HasInMonstersZone(CardId.SwordsoulSupremeSovereign_Chengying, true, false, true)
                && !activatedCardIdList.Contains(CardId.SwordsoulSupremeSovereign_Chengying) && Enemy.Graveyard.Count() > 0)
                {
                    if (GetProblematicEnemyMonster() != null || (Duel.Phase == DuelPhase.End && Duel.Player == 1))
                    {
                        bool triggerFlag = true;
                        List<ClientCard> enemyTargetList = Enemy.GetMonsters().Where(card =>
                            card.IsFaceup() && card.HasType(CardType.Effect) && !card.IsShouldNotBeMonsterTarget() && card.IsShouldNotBeTarget()).ToList();
                        if (enemyTargetList.Count() == 0)
                        {
                            List<ClientCard> botTargetList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasType(CardType.Effect)
                                && !card.IsDisabled() && card != Card && !card.IsCode(CardId.SwordsoulSupremeSovereign_Chengying)).ToList();
                            if (botTargetList.Count() == 0)
                            {
                                triggerFlag = false;
                            } else {
                                botTargetList.Sort(CardContainer.CompareCardAttack);
                                enemyTargetList.AddRange(botTargetList);
                            }
                        } else {
                            enemyTargetList.Sort(CardContainer.CompareCardAttack);
                            enemyTargetList.Reverse();
                        }
                        if (triggerFlag)
                        {
                            negateTargetList.AddRange(enemyTargetList);
                        }
                    }
                }

                if (negateTargetList.Count() > 0)
                {
                    // select banish card
                    List<ClientCard> graveBanishList = Bot.Graveyard.Where(card => card.HasSetcode(SetcodeSwordsoul) || card.HasRace(CardRace.Wyrm)).ToList();

                    if (graveBanishList.Count() > 0)
                    {
                        bool selectFlag = false;
                        // trigger blackout
                        ClientCard blackOut = graveBanishList.FirstOrDefault(card => card.IsCode(CardId.SwordsoulBlackout));
                        if (Duel.Player == 0 && !activatedCardIdList.Contains(CardId.SwordsoulBlackout) && blackOut != null)
                        {
                            AI.SelectCard(blackOut);
                            selectFlag = true;
                        } 
                        if (!selectFlag)
                        {
                            // banish dump card
                            List<int> checkIdList = new List<int>{
                                CardId.SwordsoulEmergence, CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe, CardId.SwordsoulStrategistLongyuan, CardId.MonkOfTheTenyi,
                                CardId.TenyiSpirit_Adhara, CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna
                            };
                            foreach (int checkId in checkIdList)
                            {
                                List<ClientCard> checkCardList = graveBanishList.Where(card => card.IsCode(checkId)).ToList();
                                if (checkCardList.Count() > 1)
                                {
                                    AI.SelectCard(checkCardList);
                                    selectFlag = true;
                                    break;
                                }
                            }
                        }
                        if (!selectFlag)
                        {
                            // banish exists card
                            List<int> checkIdList = new List<int>{
                                CardId.SwordsoulEmergence, CardId.MonkOfTheTenyi, CardId.ShamanOfTheTenyi, CardId.SwordsoulOfTaia,
                                CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulOfMoYe, CardId.TenyiSpirit_Adhara, CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna
                            };
                            foreach (int checkId in checkIdList)
                            {
                                List<ClientCard> checkCardList = graveBanishList.Where(card => card.IsCode(checkId)).ToList();
                                if (checkCardList.Count() > 0)
                                {
                                    AI.SelectCard(checkCardList);
                                    selectFlag = true;
                                    break;
                                }
                            }
                        }
                        if (!selectFlag)
                        {
                            AI.SelectCard(ShuffleCardList(graveBanishList));
                        }
                    }
                    AI.SelectNextCard(negateTargetList);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }

            } else {
                // search
                if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
                {
                    if (CheckRemainInDeck(CardId.SwordsoulBlackout) > 0)
                    {
                        AI.SelectCard(CardId.SwordsoulBlackout);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }

                    List<int> searchIdList = new List<int>{
                        CardId.SwordsoulBlackout, CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia, CardId.SwordsoulEmergence,
                        CardId.SwordsoulStrategistLongyuan
                    };
                    foreach (int checkId in searchIdList)
                    {
                        if (CheckRemainInDeck(checkId) > 0 && !Bot.HasInHand(checkId))
                        {
                            AI.SelectCard(checkId);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }

                if (CheckAtAdvantage())
                {
                    if (!activatedCardIdList.Contains(CardId.SwordsoulStrategistLongyuan) && !Bot.HasInHand(CardId.SwordsoulStrategistLongyuan)
                    && SwordsoulOfMoYeEffectCheck() && CheckRemainInDeck(CardId.SwordsoulStrategistLongyuan) > 0)
                    {
                        AI.SelectCard(CardId.SwordsoulStrategistLongyuan);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }

                    if (!activatedCardIdList.Contains(CardId.SwordsoulStrategistLongyuan) && Bot.HasInHand(CardId.SwordsoulStrategistLongyuan)
                    && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && !activatedCardIdList.Contains(CardId.SwordsoulSacredSummit))
                    {
                        // ready for another level 8 synchro
                        if (Bot.HasInHandOrInGraveyard(CardId.SwordsoulOfTaia) && !Bot.HasInHand(CardId.SwordsoulSacredSummit))
                        {
                            if (CheckRemainInDeck(CardId.SwordsoulSacredSummit) > 0)
                            {
                                AI.SelectCard(CardId.SwordsoulSacredSummit);
                                activatedCardIdList.Add(Card.Id);
                                return true;
                            }
                        }
                        if (!Bot.HasInHandOrInGraveyard(CardId.SwordsoulOfTaia) && Bot.HasInHand(CardId.SwordsoulSacredSummit))
                        {
                            if (CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0)
                            {
                                AI.SelectCard(CardId.SwordsoulOfTaia);
                                activatedCardIdList.Add(Card.Id);
                                return true;
                            }
                        }
                    }
                }
            
                if (!Bot.HasInMonstersZone(CardId.SwordsoulToken) && Bot.HasInMonstersZone(CardId.SwordsoulStrategistLongyuan)
                    && Bot.HasInMonstersZone(CardId.SwordsoulStrategistLongyuan) && CheckRemainInDeck(CardId.SwordsoulBlackout) > 0
                    && !activatedCardIdList.Contains(CardId.SwordsoulBlackout))
                {
                    Logger.DebugWriteLine("Chixiao banish blackout");
                    AI.SelectCard(CardId.SwordsoulBlackout);
                    AI.SelectOption(1);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }

                if (CheckAtAdvantage())
                {
                    List<int> searchIdList = new List<int>{
                        CardId.SwordsoulBlackout, CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia, CardId.SwordsoulEmergence,
                        CardId.SwordsoulStrategistLongyuan
                    };
                    foreach (int checkId in searchIdList)
                    {
                        if (CheckRemainInDeck(checkId) > 0 && !Bot.HasInHand(checkId))
                        {
                            AI.SelectCard(checkId);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }
            
                List<int> checkIdList = new List<int>{
                    CardId.SwordsoulBlackout, CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia, CardId.SwordsoulEmergence,
                    CardId.SwordsoulStrategistLongyuan
                };
                foreach (int checkId in checkIdList)
                {
                    if (CheckRemainInDeck(checkId) > 0 && !Bot.HasInHand(checkId))
                    {
                        AI.SelectCard(checkId);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool BaxiaBrightnessOfTheYangZingActivate()
        {
            Logger.DebugWriteLine("Baxia desc: " + ActivateDescription.ToString());

            if (ActivateDescription == Util.GetStringId(CardId.BaxiaBrightnessOfTheYangZing, 0))
            {
                List<ClientCard> enemyTargetList = GetNormalEnemyTargetList(true);
                if (enemyTargetList.Count() > 0)
                {
                    AI.SelectCard(enemyTargetList);
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            } else 
            {
                List<ClientCard> destroyTarget = Bot.GetSpells();
                destroyTarget.AddRange(Bot.GetMonsters().Where(card => card.IsFacedown() || card.Attack <= 1000).ToList());
                if (destroyTarget.Count() == 0)
                {
                    return false;
                }

                bool canUseMoye = !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe)
                    && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0 && SwordsoulOfMoYeEffectCheck();
                bool canUseTaia = !activatedCardIdList.Contains(CardId.SwordsoulOfTaia)
                    && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0 && SwordsoulOfTaiaEffectCheck();

                if (canUseMoye && Bot.HasInGraveyard(CardId.SwordsoulOfMoYe))
                {
                    AI.SelectCard(destroyTarget);
                    AI.SelectNextCard(CardId.SwordsoulOfMoYe);
                    activatedCardIdList.Add(Card.Id + 2);
                    return true;
                }
                if (canUseTaia && Bot.HasInGraveyard(CardId.SwordsoulOfTaia))
                {
                    AI.SelectCard(destroyTarget);
                    AI.SelectNextCard(CardId.SwordsoulOfTaia);
                    activatedCardIdList.Add(Card.Id + 2);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.IncredibleEcclesiaTheVirtuous))
                {
                    // sp summon ecclesia for moye/taia
                    if (!activatedCardIdList.Contains(CardId.IncredibleEcclesiaTheVirtuous))
                    {
                        if ((canUseMoye && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0)
                        || (canUseTaia && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0))
                        {
                            AI.SelectCard(destroyTarget);
                            AI.SelectNextCard(CardId.IncredibleEcclesiaTheVirtuous);
                            activatedCardIdList.Add(Card.Id + 2);
                            return true;
                        }
                    }
                    // sp summon ecclesia as tuner
                    if (Bot.GetMonsters().Any(card => card.IsFaceup() && !card.IsTuner() && card.Level == 4))
                    {
                        AI.SelectCard(destroyTarget);
                        AI.SelectNextCard(CardId.IncredibleEcclesiaTheVirtuous);
                        activatedCardIdList.Add(Card.Id + 2);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool YaziEvilOfTheYangZingActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                // special summon
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0 && SwordsoulOfMoYeEffectCheck())
                {
                    AI.SelectCard(CardId.SwordsoulOfMoYe);
                    return true;
                }
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
                {
                    AI.SelectCard(CardId.SwordsoulOfTaia);
                    return true;
                }
                if (Bot.HasInMonstersZone(CardId.SwordsoulToken))
                {
                    List<int> specialSummonIdListForSynchro = new List<int>{CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia};
                    foreach (int checkId in specialSummonIdListForSynchro)
                    {
                        if (CheckRemainInDeck(checkId) > 0)
                        {
                            AI.SelectCard(checkId);
                            return true;
                        }
                    }
                }
                List<int> specialSummonIdList = new List<int>{
                    CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Adhara,
                    CardId.SwordsoulStrategistLongyuan, CardId.SwordsoulOfMoYe, CardId.SwordsoulOfTaia
                };
                foreach (int checkId in specialSummonIdList)
                {
                    if (CheckRemainInDeck(checkId) > 0)
                    {
                        AI.SelectCard(checkId);
                        return true;
                    }
                }
            } else 
            {
                // destroy
                if (CheckWhetherNegated(true))
                {
                    return false;
                }
                bool selfDestroy = false;
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfMoYe) && CheckRemainInDeck(CardId.SwordsoulOfMoYe) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0 && SwordsoulOfMoYeEffectCheck())
                {
                    selfDestroy = true;
                }
                if (!activatedCardIdList.Contains(CardId.SwordsoulOfTaia) && CheckRemainInDeck(CardId.SwordsoulOfTaia) > 0
                    && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0)
                {
                    selfDestroy = true;
                }
                if (selfDestroy)
                {
                    AI.SelectCard(Card);
                } else
                {
                    List<ClientCard> YangZingList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasSetcode(SetcodeYangZing)).ToList();
                    YangZingList.Sort(CardContainer.CompareCardAttack);
                    AI.SelectCard(YangZingList);
                }
                AI.SelectNextCard(GetNormalEnemyTargetList(true));
                return true;
            }

            return false;
        }

        public bool ShamanOfTheTenyiActivate()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                // destroy
                AI.SelectCard(GetNormalEnemyTargetList());
                return true;
            } else 
            {
                // special summon
                if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
                {
                    return false;
                }

                bool canUseMoye = Bot.HasInGraveyard(CardId.SwordsoulOfMoYe) && CheckCalledbytheGrave(CardId.SwordsoulOfMoYe) == 0
                    && !activatedCardIdList.Contains(CardId.SwordsoulOfMoYe);
                bool canUseTaia = Bot.HasInHandOrInGraveyard(CardId.SwordsoulOfTaia) && CheckCalledbytheGrave(CardId.SwordsoulOfTaia) == 0
                    && !activatedCardIdList.Contains(CardId.SwordsoulOfTaia);
                bool shouldDiscardTaia = !Bot.HasInGraveyard(CardId.SwordsoulOfTaia) && Bot.HasInHand(CardId.SwordsoulOfTaia);
                List<ClientCard> sortedReviveTargetList = Bot.GetGraveyardMonsters().Where(card =>
                    card.IsCanRevive() && card.HasRace(CardRace.Wyrm)).ToList();
                sortedReviveTargetList.Sort(CardContainer.CompareCardAttack);
                sortedReviveTargetList.Reverse();

                if (CheckAtAdvantage())
                {
                    // try to kill
                    if (Duel.Turn > 1 && Enemy.GetMonsterCount() == 0)
                    {
                        int currentAttack = Util.GetTotalAttackingMonsterAttack(0);
                        if (currentAttack < Enemy.LifePoints)
                        {
                            List<ClientCard> overkillList = sortedReviveTargetList.Where(card =>
                                 card.Attack + currentAttack >= Enemy.LifePoints).ToList();
                            if (overkillList.Count() > 0)
                            {
                                SelectDiscardForShamanOfTheTenyi(shouldDiscardTaia);
                                AI.SelectNextCard(overkillList);
                                return true;
                            }
                        }
                    }

                    // for next synchro
                    if (canUseMoye)
                    {
                        SelectDiscardForShamanOfTheTenyi();
                        AI.SelectNextCard(CardId.SwordsoulOfMoYe);
                        return true;
                    }
                    if (canUseTaia)
                    {
                        SelectDiscardForShamanOfTheTenyi(shouldDiscardTaia);
                        AI.SelectNextCard(CardId.SwordsoulOfTaia);
                        return true;
                    }

                    // choose max attack
                    SelectDiscardForShamanOfTheTenyi();
                    AI.SelectNextCard(sortedReviveTargetList);
                    return true;

                } else {
                    // reborn synchro monster
                    List<ClientCard> synchroMonsterList = sortedReviveTargetList.Where(card => card.HasType(CardType.Synchro)).ToList();
                    if (synchroMonsterList.Count() > 0)
                    {
                        SelectDiscardForShamanOfTheTenyi();
                        AI.SelectNextCard(synchroMonsterList);
                        return true;
                    }

                    // for next synchro
                    if (canUseMoye)
                    {
                        SelectDiscardForShamanOfTheTenyi();
                        AI.SelectNextCard(CardId.SwordsoulOfMoYe);
                        return true;
                    }
                    if (canUseTaia)
                    {
                        SelectDiscardForShamanOfTheTenyi(shouldDiscardTaia);
                        AI.SelectNextCard(CardId.SwordsoulOfTaia);
                        return true;
                    }
                }
            }

            return false;
        }

        public void SelectDiscardForShamanOfTheTenyi(bool useTaia = false)
        {
            if (useTaia)
            {
                AI.SelectCard(CardId.SwordsoulOfTaia);
            } else {
                // drop tenyi
                List<int> tenyiList = new List<int>{CardId.TenyiSpirit_Vishuda, CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Adhara};
                foreach (int tenyiId in tenyiList)
                {
                    if (Bot.HasInHand(tenyiId))
                    {
                        AI.SelectCard(tenyiId);
                        return;
                    }
                }

                // drop dump card
                foreach (ClientCard hand in Bot.Hand)
                {
                    if (Bot.Hand.Where(card => card.IsCode(hand.Id)).Count() > 1)
                    {
                        AI.SelectCard(hand);
                        return;
                    }
                }

                // check discard list
                List<int> discardList = new List<int>{
                    CardId.CrossoutDesignator, _CardId.PotOfDesires, CardId.TenyiSpirit_Ashuna, CardId.TenyiSpirit_Vishuda,
                    CardId.TenyiSpirit_Adhara, CardId.NibiruThePrimalBeing, CardId.SwordsoulSacredSummit, CardId.IncredibleEcclesiaTheVirtuous,
                    _CardId.InfiniteImpermanence, _CardId.CalledByTheGrave, CardId.SwordsoulOfTaia, CardId.SwordsoulOfMoYe,
                    CardId.SwordsoulStrategistLongyuan, _CardId.AshBlossom, _CardId.MaxxC, _CardId.EffectVeiler, 
                    CardId.SwordsoulEmergence,  CardId.SwordsoulBlackout
                };
                foreach (int discardCheck in discardList)
                {
                    if (Bot.HasInHand(discardCheck))
                    {
                        AI.SelectCard(discardCheck);
                        return;
                    }
                }

            }
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int>{
                CardId.SwordsoulBlackout
            };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }

            // select place
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int setFornfiniteImpermanence = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        setFornfiniteImpermanence += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(_CardId.InfiniteImpermanence))
                {
                    if (Card.IsCode(_CardId.InfiniteImpermanence))
                    {
                        AI.SelectPlace(setFornfiniteImpermanence);
                        return true;
                    } else
                    {
                        SelectSTPlace(Card, false, avoid_list);
                        return true;
                    }
                } else
                {
                    SelectSTPlace();
                }
                return true;
            }

            else if (Enemy.HasInSpellZone(_CardId.AntiSpellFragrance, true) || Bot.HasInSpellZone(_CardId.AntiSpellFragrance, true))
            {
                if (Card.IsSpell() && !Bot.HasInSpellZone(Card.Id))
                {
                    SelectSTPlace();
                    return true;
                }
            }

            return false;
        }

    }
}