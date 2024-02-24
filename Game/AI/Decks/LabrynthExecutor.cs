using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Labrynth", "AI_Labrynth")]
    public class LabrynthExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int LadyLabrynthOfTheSilverCastle = 81497285;
            public const int LovelyLabrynthOfTheSilverCastle = 2347656;
            public const int UnchainedSoulOfSharvara = 41165831;
            public const int AriasTheLabrynthButler = 73602965;
            public const int ArianeTheLabrynthServant = 75730490;
            public const int AriannaTheLabrynthServant = 1225009;
            public const int LabrynthChandraglier = 37629703;
            // _CardId.AshBlossom = 14558127;
            // _CardId.MaxxC = 23434538;
            public const int LabrynthStovieTorbie = 74018812;
            public const int LabrynthCooclock = 2511;

            public const int PotOfExtravagance = 49238328;

            public const int WelcomeLabrynth = 5380979;
            public const int TransactionRollback = 6351147;
            // _CardId.InfiniteImpermanence = 10045474;
            public const int DestructiveDarumaKarmaCannon = 30748475;
            public const int EscapeOfTheUnchained = 53417695;
            // _CardId.DimensionalBarrier = 83326048;
            public const int BigWelcomeLabrynth = 92714517;

            public const int ChaosAngel = 22850702;
            public const int SuperStarslayerTYPHON = 93039339;
            public const int UnchainedAbomination = 29479265;
            public const int UnchainedSoulOfAnguish = 93084621;
            public const int UnchainedSoulLordOfYama = 24269961;
            public const int UnchainedSoulOfRage = 67680512;
            public const int SPLittleKnight = 29301450;
            public const int MuckrakerFromTheUnderworld = 71607202;
            public const int RelinquishedAnima = 94259633;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
            public const int SkillDrain = 82732705;

            public const int DimensionShifter = 91800273;
            public const int MacroCosmos = 30241314;
            public const int DimensionalFissure = 81674782;
            public const int BanisheroftheRadiance = 94853057;
            public const int BanisheroftheLight = 61528025;
            public const int KashtiraAriseHeart = 48626373;
            public const int AccesscodeTalker = 86066372;
            public const int GhostMournerMoonlitChill = 52038441;
        }

        public LabrynthExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // startup effect/triggered chain
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC,                          MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfExtravagance,               PotOfExtravaganceActivate);
            AddExecutor(ExecutorType.Repos, CardId.LovelyLabrynthOfTheSilverCastle, ReposForLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel,                      ChaosAngelActivate);
            AddExecutor(ExecutorType.Activate, CardId.LovelyLabrynthOfTheSilverCastle, LovelyLabrynthOfTheSilverCastleActivate);
            AddExecutor(ExecutorType.Activate, CardId.RelinquishedAnima,               RelinquishedAnimaActivate);
            AddExecutor(ExecutorType.Activate, CardId.AriannaTheLabrynthServant,       AriannaTheLabrynthServantActivate);
            AddExecutor(ExecutorType.Activate, CardId.ArianeTheLabrynthServant,        ArianeTheLabrynthServantActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthChandraglier,            RecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthStovieTorbie,            RecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthCooclock,                RecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulLordOfYama,         UnchainedSoulLordOfYamaActivate);
            AddExecutor(ExecutorType.Activate, CardId.WelcomeLabrynth,                 RecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperStarslayerTYPHON,           SuperStarslayerTYPHONActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedAbomination,            UnchainedAbominationActivate);

            // repos
            AddExecutor(ExecutorType.Repos, CardId.ArianeTheLabrynthServant,        ReposForLabrynth);
            AddExecutor(ExecutorType.Repos, CardId.AriannaTheLabrynthServant,       ReposForLabrynth);

            // negate/chain
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom,                   AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.LadyLabrynthOfTheSilverCastle, LadyLabrynthOfTheSilverCastleFieldActivate);
            AddExecutor(ExecutorType.Activate, CardId.AriasTheLabrynthButler,        RecycleActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight,                SPLittleKnightActivate);
            AddExecutor(ExecutorType.Activate, _CardId.DimensionalBarrier,           DimensionalBarrierActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence,         InfiniteImpermanenceActivate);

            AddExecutor(ExecutorType.Activate, CardId.MuckrakerFromTheUnderworld,   MuckrakerFromTheUnderworldActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulOfRage,          UnchainedSoulOfRageActivate);
            AddExecutor(ExecutorType.Activate, CardId.TransactionRollback,          TransactionRollbackActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonActivate);
            AddExecutor(ExecutorType.Activate, CardId.EscapeOfTheUnchained,         EscapeOfTheUnchainedActivate);

            // sp summon
            AddExecutor(ExecutorType.Activate, CardId.LadyLabrynthOfTheSilverCastle, LadyLabrynthOfTheSilverCastleHandActivate);
            AddExecutor(ExecutorType.Activate, CardId.BigWelcomeLabrynth,            BigWelcomeLabrynthBecomeTargetActivate);
            AddExecutor(ExecutorType.Activate, CardId.WelcomeLabrynth,               WelcomeLabrynthActivate);
            AddExecutor(ExecutorType.Activate, CardId.BigWelcomeLabrynth,            BigWelcomeLabrynthActivate);

            // clock
            AddExecutor(ExecutorType.Activate, CardId.AriasTheLabrynthButler, AriasTheLabrynthButlerActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthCooclock,       LabrynthCooclockActivate);
            AddExecutor(ExecutorType.Activate, CardId.BigWelcomeLabrynth,     BigWelcomeLabrynthGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulOfAnguish, UnchainedSoulOfAnguishActivate);

            // summon step
            AddExecutor(ExecutorType.SpellSet, SpellSetForCooClockCheck);
            AddExecutor(ExecutorType.Summon,   CardId.ArianeTheLabrynthServant,  ArianeTheLabrynthServantForRollbackSummon);
            AddExecutor(ExecutorType.Summon,   CardId.AriannaTheLabrynthServant, AriannaTheLabrynthServantSummon);
            AddExecutor(ExecutorType.Summon,   CardId.ArianeTheLabrynthServant,  ArianeTheLabrynthServantSummon);
            AddExecutor(ExecutorType.Summon,   LabrynthForCooClockSummon);
            AddExecutor(ExecutorType.Summon,   ForLinkSummon);
            AddExecutor(ExecutorType.Summon,   ForSynchroSummon);
            AddExecutor(ExecutorType.Summon,   CardId.LabrynthCooclock,          ForAnimaSummon);

            // furniture set
            AddExecutor(ExecutorType.Activate, CardId.LabrynthChandraglier,   FurnitureSetWelcomeActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthStovieTorbie,   FurnitureSetWelcomeActivate);

            // sp summon from extra
            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel,                 ChaosAngelSpSummonWith2Monster);
            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishedAnima,          RelinquishedAnimaSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulLordOfYama,    UnchainedSoulLordOfYamaSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulOfAnguish,     UnchainedSoulOfAnguishSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulOfRage,        UnchainedSoulOfRageSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedAbomination,       UnchainedAbominationSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight,             SPLittleKnightSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel,                 ChaosAngelSpSummonWith3Monster);
            AddExecutor(ExecutorType.SpSummon, CardId.MuckrakerFromTheUnderworld, MuckrakerFromTheUnderworldSpSummon);

            // hand eff
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulOfSharvara, UnchainedSoulOfSharvaraActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.SuperStarslayerTYPHON, SuperStarslayerTYPHONSpSummon);
            AddExecutor(ExecutorType.Repos,       MonsterRepos);
            AddExecutor(ExecutorType.Summon,      SummonForTYPHONCheck);
            AddExecutor(ExecutorType.SummonOrSet, ForBigWelcomeSummon);
            AddExecutor(ExecutorType.SpellSet,    SpellSetCheck);
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeUnchained = 0x130;
        const int SetcodeLabrynth = 0x17e;
        const int SetcodeHorus = 0x19d;
        const int hintTimingMainEnd = 0x4;
        const int hintBattleStart = 0x8;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.AriannaTheLabrynthServant, CardId.LabrynthChandraglier, _CardId.AshBlossom, _CardId.MaxxC,
                                CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, _CardId.InfiniteImpermanence, CardId.BigWelcomeLabrynth }},
            {2, new List<int> { CardId.LadyLabrynthOfTheSilverCastle, CardId.AriasTheLabrynthButler, CardId.PotOfExtravagance, CardId.WelcomeLabrynth,
                                CardId.TransactionRollback }},
            {1, new List<int> { CardId.LovelyLabrynthOfTheSilverCastle, CardId.UnchainedSoulOfSharvara, CardId.ArianeTheLabrynthServant,
                                CardId.DestructiveDarumaKarmaCannon, CardId.EscapeOfTheUnchained, _CardId.DimensionalBarrier }}
        };
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

        bool enemyActivateMaxxC = false;
        List<int> infiniteImpermanenceList = new List<int>();
        bool summoned = false;
        List<int> activatedCardIdList = new List<int>();
        List<ClientCard> currentNegateMonsterList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> setTrapThisTurn = new List<ClientCard>();
        List<ClientCard> summonThisTurn = new List<ClientCard>();
        List<ClientCard> enemySetThisTurn = new List<ClientCard>();
        List<ClientCard> escapeTargetList = new List<ClientCard>();
        List<ClientCard> summonInChainList = new List<ClientCard>();
        bool cooclockAffected = false;
        bool cooclockActivating = false;
        bool furnitureActivating = false;
        bool dimensionBarrierAnnouncing = false;
        int banSpSummonExceptFiendCount = 0;
        int dimensionShifterCount = 0;
        int enemySpSummonFromExLastTurn = 0;
        int enemySpSummonFromExThisTurn = 0;
        bool enemyActivateInfiniteImpermanenceFromHand = false;
        int rollbackCopyCardId = 0;
        List<int> dimensionalBarrierAnnouced = new List<int>();
        List<int> chainSummoningIdList = new List<int>(3);
        ClientCard bigwelcomeEscaseTarget = null;

        /// <summary>
        /// Shuffle List<ClientCard> and return a random-order card list
        /// </summary>
        public List<T> ShuffleList<T>(List<T> list)
        {
            List<T> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(result.Count);
                int nextIndex = (index + Program.Rand.Next(result.Count - 1)) % result.Count;
                T tempCard = result[index];
                result[index] = result[nextIndex];
                result[nextIndex] = tempCard;
            }
            return result;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> floodagateList = Enemy.GetMonsters().Where(c => c?.Data != null &&
                c.IsFloodgate() && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count() > 0) return floodagateList[0];

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

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count() > 0) resultList.AddRange(floodagateList);
            
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

        /// <summary>
        /// check enemy's dangerous card in grave
        /// </summary>
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom) || card.HasSetcode(SetcodeHorus))).ToList();
            List<int> dangerMonsterIdList = new List<int>{
                99937011, 63542003, 9411399, 28954097, 30680659
            };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public int GetEmptyMainMonsterZoneCount()
        {
            int remainCount = 0;
            for (int idx = 0; idx < 5; ++idx)
            {
                if (Bot.MonsterZone[idx] == null) remainCount++;
            }
            return remainCount;
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

        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeTarget = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            if (CheckWhetherNegated())
            {
                return resultList;
            }

            // negate before used
            ClientCard target = Enemy.MonsterZone.FirstOrDefault(card => card?.Data != null
                    && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && !card.IsShouldNotBeTarget()
                    && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && !currentNegateMonsterList.Contains(card));
            if (target != null)
            {
                resultList.Add(target);
            }

            // negate monster effect on the field
            foreach (ClientCard chainingCard in Duel.CurrentChain)
            {
                if (chainingCard.Location == CardLocation.MonsterZone && chainingCard.Controller == 1 && !chainingCard.IsDisabled()
                && CheckCanBeTargeted(chainingCard, canBeTarget, selfType) && !currentNegateMonsterList.Contains(chainingCard))
                {
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
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

        public int GetBotCurrentTotalAttack(List<ClientCard> exceptList = null)
        {
            if (Util.IsTurn1OrMain2()) return 0;
            int result = 0;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (exceptList != null && exceptList.Contains(monster)) continue;
                if (monster.IsAttack() || !summonThisTurn.Contains(monster)) result += monster.Attack;
            }
            return result;
        }

        public List<ClientCard> GetCanBeUsedForLinkMaterial(bool useAdvancedMonster = false, Func<ClientCard, bool> exceptRule = null)
        {
            List<ClientCard> materialList = Bot.GetMonsters().Where(card => {
                if (card.IsFacedown() || (exceptRule != null && exceptRule(card))) return false;
                if (card.IsCode(CardId.MuckrakerFromTheUnderworld) && summonThisTurn.Contains(card)) return false;
                if (card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle) && !card.IsDisabled() && Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth)) return false;
                if ((card.IsCode(CardId.ChaosAngel) || card.IsCode(CardId.LadyLabrynthOfTheSilverCastle))
                    && !useAdvancedMonster && (card.IsAttack() || !summonThisTurn.Contains(card))) return false;

                return true;
            }).ToList();
            materialList.Sort(CompareUsableAttack);
            return materialList;
        }

        public bool CheckCanDirectAttack()
        {
            return Enemy.GetMonsterCount() == 0 && !activatedCardIdList.Contains(CardId.SPLittleKnight) && Duel.Turn > 1 && Duel.Player == 0 && Duel.Phase < DuelPhase.Main2;
        }

        /// <summary>
        /// Check negated turn count of id
        /// </summary>
        public int CheckCalledbytheGrave(int id)
        {
            if (DefaultCheckWhetherCardIdIsNegated(id)) return 1;
            return 0;
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
                    && (card.IsShouldNotBeSpellTrapTarget() || (!card.IsDisabled() && notToBeTrapTargetList.Contains(card.Id)))) return false;
            }
            return true;
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
        public int CheckRemainInDeck(params int[] ids)
        {
            int sumResult = 0;
            foreach (int id in ids)
            {
                sumResult += CheckRemainInDeck(id);
            }

            return sumResult;
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
            if (target.IsTrap() && (Enemy.HasInSpellZone(CardId.RoyalDecree, true) || Bot.HasInSpellZone(CardId.RoyalDecree, true))) return true;
            if (target.Location == CardLocation.SpellZone && (target.IsSpell() || target.IsTrap()))
            {
                int selfSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) selfSeq = i;
                }
                if (infiniteImpermanenceList.Contains(selfSeq)) return true;
            }
            // how to get here?
            return false;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        /// <param name="isCounter">check whether card itself is disabled.</param>
        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0)
        {
            if ((Card.IsSpell() || Card.IsTrap() || (((int)type & (int)CardType.Spell) == 0) || (((int)type & (int)CardType.Trap) == 0)) && CheckSpellWillBeNegate())
                return true;
            if (CheckCalledbytheGrave(Card.Id) > 0) return true;
            if ((Card.IsMonster() || (((int)type & (int)CardType.Monster) == 0)) && (toFieldCheck || Card.Location == CardLocation.MonsterZone))
            {
                if ((toFieldCheck && (((int)type & (int)CardType.Link) == 0)) || Card.IsDefense())
                {
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card))) return true;
                }
                if (Enemy.HasInSpellZone(CardId.SkillDrain, true, true)) return true;
            }
            if (disablecheck) return Card.IsDisabled();
            return false;
        }

        public bool CheckNumber41(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled();
        }

        /// <summary>
        /// Check whether cards will be removed. If so, do not send cards to grave.
        /// </summary>
        public bool CheckWhetherWillbeRemoved()
        {
            if (dimensionShifterCount > 0) return true;
            List<int> checkIdList = new List<int> { CardId.BanisheroftheRadiance, CardId.BanisheroftheLight, CardId.MacroCosmos, CardId.DimensionalFissure,
                CardId.KashtiraAriseHeart, 58481572 };
            foreach (int cardid in checkIdList)
            {
                List<ClientField> fields = new List<ClientField> { Bot, Enemy };
                foreach (ClientField cf in fields)
                {
                    if (cf.HasInMonstersZone(cardid, true, false, true) || cf.HasInSpellZone(cardid, true, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether bot is at advantage.
        /// </summary>
        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && (Duel.Player == 0 || Bot.GetMonsterCount() > 0)) return true;
            return false;
        }

        public bool CheckShouldNoMoreSpSummon(bool isLabrynth = true)
        {
            if (CheckAtAdvantage() && enemyActivateMaxxC && (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2))
            {
                if (!isLabrynth) return true;
                if (cooclockAffected)
                {
                    if (Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth))) return true;
                    if (Duel.Player == 0 && !summoned) return true;
                    if (setTrapThisTurn.Count() == 0) return true;
                    return false;
                }
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

            return true;
        }

        public bool CheckChainContainEnemyMaxxC()
        {
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Controller == 1 && card.IsCode(_CardId.MaxxC)) return true;
            }

            return false;
        }

        public bool CheckBigWelcomeCanSpSummon(int cardId)
        {
            return Bot.HasInHandOrInGraveyard(cardId) || CheckRemainInDeck(cardId) > 0;
        }

        public int CompareUsableAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA == null && cardB == null)
                return 0;
            if (cardA == null)
                return -1;
            if (cardB == null)
                return 1;
            int powerA = (cardA.IsDefense() && summonThisTurn.Contains(cardA)) ? 0 : cardA.Attack;
            int powerB = (cardB.IsDefense() && summonThisTurn.Contains(cardB)) ? 0 : cardB.Attack;
            if (powerA < powerB)
                return -1;
            if (powerA == powerB)
                return CardContainer.CompareCardLevel(cardA, cardB);
            return 1;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null)
            {
                if (currentSolvingChain.Controller == 1 && currentSolvingChain.IsCode(_CardId.EvenlyMatched))
                {
                    Logger.DebugWriteLine("=== Evenly Matched activated.");
                    List<ClientCard> banishList = new List<ClientCard>();
                    List<ClientCard> botMonsters = Bot.GetMonsters().Where(card => !card.HasType(CardType.Token)).ToList();

                    // monster
                    List<ClientCard> faceDownMonsters = botMonsters.Where(card => card.IsFacedown()).ToList();
                    banishList.AddRange(faceDownMonsters);
                    List<ClientCard> notImportantMonster = botMonsters.Where(card => !banishList.Contains(card)
                        && ((card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && Bot.HasInExtra(card.Id))
                        || CheckRemainInDeck(card.Id) > 0)).ToList();
                    notImportantMonster.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(notImportantMonster);

                    // spells
                    List<ClientCard> faceUpSpells = Bot.GetSpells().Where(c => c.IsFaceup()).ToList();
                    banishList.AddRange(ShuffleList(faceUpSpells));
                    List<ClientCard> faceDownSpells = Bot.GetSpells().Where(c => c.IsFacedown()).ToList();
                    banishList.AddRange(ShuffleList(faceDownSpells));

                    List<ClientCard> importantMonster = botMonsters.Where(card => !banishList.Contains(card) && !card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)
                        && ((card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && !Bot.HasInExtra(card.Id))
                        || CheckRemainInDeck(card.Id) == 0)).ToList();
                    importantMonster.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(importantMonster);

                    // lovely
                    List<ClientCard> lovelyList = botMonsters.Where(card => !banishList.Contains(card) && card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)).ToList();
                    lovelyList.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(lovelyList);

                    return Util.CheckSelectCount(banishList, cards, min, max);
                }
            
                if (currentSolvingChain.IsCode(CardId.LadyLabrynthOfTheSilverCastle) && min == 1 && max == 1 && hint == HintMsg.Set)
                {
                    SortedDictionary<int, Func<bool>> trapCheckDict = new SortedDictionary<int, Func<bool>>{
                        {_CardId.DimensionalBarrier, DimensionalBarrierActivate},
                        {CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonSetCheck},
                        {_CardId.InfiniteImpermanence, InfiniteImpermanenceSetCheck}
                    };
                    foreach (KeyValuePair<int, Func<bool>> pair in trapCheckDict)
                    {
                        ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            SelectSTPlace(null, true);
                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }

                    ClientCard rollback = cards.FirstOrDefault(card => card.IsCode(CardId.TransactionRollback));
                    if (rollback != null)
                    {
                        bool haveUnchainSoul = false;
                        if (!activatedCardIdList.Contains(CardId.UnchainedSoulOfSharvara))
                        {
                            haveUnchainSoul |= Bot.HasInHand(CardId.UnchainedSoulOfSharvara);
                            haveUnchainSoul |= Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2
                                && Bot.HasInExtra(CardId.UnchainedSoulLordOfYama) && !activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama)
                                && (CheckRemainInDeck(CardId.UnchainedSoulOfSharvara) > 0 || Bot.HasInGraveyard(CardId.UnchainedSoulOfSharvara))
                                && Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasRace(CardRace.Fiend) && card.Level <= 4).Count() >= 2;
                        }
                        bool haveAriane = false;
                        if (!activatedCardIdList.Contains(CardId.ArianeTheLabrynthServant) && Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2)
                        {
                            haveAriane |= Bot.HasInMonstersZone(CardId.ArianeTheLabrynthServant);
                            haveAriane |= Bot.HasInHand(CardId.ArianeTheLabrynthServant) && !summoned;
                            haveAriane |= Bot.GetSpells().Any(card => card.IsFacedown() &&
                                (
                                    (card.IsCode(CardId.WelcomeLabrynth) && !activatedCardIdList.Contains(CardId.WelcomeLabrynth) && (cooclockAffected || !setTrapThisTurn.Contains(card)))
                                    ||
                                    (card.IsCode(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && (cooclockAffected || !setTrapThisTurn.Contains(card)))
                                )
                            );
                        }

                        if (haveUnchainSoul || haveAriane)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { rollback }, cards, min, max);
                        }
                    }

                    // welcome check
                    SortedDictionary<int, ClientCard> welcomeCheck = new SortedDictionary<int, ClientCard> {
                    {CardId.BigWelcomeLabrynth, cards.FirstOrDefault(card => card.IsCode(CardId.BigWelcomeLabrynth))},
                    {CardId.WelcomeLabrynth, cards.FirstOrDefault(card => card.IsCode(CardId.WelcomeLabrynth))}
                };
                    List<int> welcomeCheckIdList = new List<int> { CardId.BigWelcomeLabrynth, CardId.WelcomeLabrynth };
                    foreach (KeyValuePair<int, ClientCard> checkPair in welcomeCheck)
                    {
                        if (checkPair.Value != null && !Bot.HasInHand(checkPair.Key) && !Bot.HasInGraveyard(checkPair.Key)
                            && !Bot.GetSpells().Any(card => card.IsCode(checkPair.Key) && card.IsFacedown()))
                        {
                            SelectSTPlace(null, true);
                            return Util.CheckSelectCount(new List<ClientCard> { checkPair.Value }, cards, min, max);
                        }
                    }
                    if (welcomeCheck[CardId.BigWelcomeLabrynth] != null &&
                        !Bot.HasInHand(CardId.BigWelcomeLabrynth) && !Bot.GetSpells().Any(card => card.IsCode(CardId.BigWelcomeLabrynth) && card.IsFacedown()))
                    {
                        SelectSTPlace(null, true);
                        return Util.CheckSelectCount(new List<ClientCard> { welcomeCheck[CardId.BigWelcomeLabrynth] }, cards, min, max);
                    }

                    // normal set
                    List<int> checkIdList = new List<int>{_CardId.InfiniteImpermanence, _CardId.DimensionalBarrier, CardId.DestructiveDarumaKarmaCannon,
                        CardId.BigWelcomeLabrynth, CardId.TransactionRollback, CardId.WelcomeLabrynth};
                    foreach (int checkId in checkIdList)
                    {
                        ClientCard checkCard = cards.FirstOrDefault(card => card.IsCode(checkId));
                        if (checkCard != null)
                        {
                            SelectSTPlace(null, true);
                            return Util.CheckSelectCount(new List<ClientCard> { checkCard }, cards, min, max);
                        }
                    }
                }

                if (currentSolvingChain.IsCode(CardId.WelcomeLabrynth))
                {
                    banSpSummonExceptFiendCount = 2;
                }

                if (currentSolvingChain.IsCode(CardId.WelcomeLabrynth) || (currentSolvingChain.IsCode(CardId.TransactionRollback) && rollbackCopyCardId == CardId.WelcomeLabrynth))
                {
                    Logger.DebugWriteLine("rewrite welcome's select.");
                    List<ClientCard> selection = new List<ClientCard>();

                    ClientCard ariane = GetWelcomeOrBigWelcomeTarget(cards, CardId.ArianeTheLabrynthServant);
                    if (ariane != null && !summonInChainList.Any(card => card.IsCode(CardId.ArianeTheLabrynthServant)))
                    {
                        if ((Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2 || Duel.Player == 1 && Duel.Phase >= DuelPhase.Main2)
                        && Bot.HasInHandOrInSpellZone(CardId.TransactionRollback))
                        {
                            selection.Add(ariane);
                        }
                    }

                    ClientCard arianna = GetWelcomeOrBigWelcomeTarget(cards, CardId.AriannaTheLabrynthServant);
                    if (arianna != null && !summonInChainList.Any(card => card.IsCode(CardId.AriannaTheLabrynthServant)))
                    {
                        bool canActivateCheck = !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !CheckWhetherNegated(true, true, CardType.Monster);
                        if (canActivateCheck)
                        {
                            bool checkFlag = !(!activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) &&
                                (Bot.HasInGraveyard(CardId.BigWelcomeLabrynth) || Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth))));
                            checkFlag |= !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)
                                && (CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle) || Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true))
                                && Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth)
                                    && (!setTrapThisTurn.Contains(card) || cooclockAffected));
                            checkFlag |= !(Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true)
                                || CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle));
                            if (checkFlag)
                            {
                                selection.Add(arianna);
                            }
                        }
                    }

                    ClientCard arias = GetWelcomeOrBigWelcomeTarget(cards, CardId.AriasTheLabrynthButler);
                    if (arias != null && !summonInChainList.Any(card => card.IsCode(CardId.AriasTheLabrynthButler)) && !Bot.HasInHandOrHasInMonstersZone(CardId.AriasTheLabrynthButler))
                    {
                        bool canActivateCheck = !activatedCardIdList.Contains(CardId.AriasTheLabrynthButler) && !CheckWhetherNegated(true, true, CardType.Monster);
                        if (canActivateCheck && Bot.HasInHand(CardId.LovelyLabrynthOfTheSilverCastle))
                        {
                            selection.Add(arias);
                        }
                    }

                    ClientCard lovely = GetWelcomeOrBigWelcomeTarget(cards, CardId.LovelyLabrynthOfTheSilverCastle);
                    if (lovely != null && !summonInChainList.Any(card => card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)))
                    {
                        if (Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth))
                        {
                            selection.Add(lovely);
                        }
                    }

                    ClientCard lady = GetWelcomeOrBigWelcomeTarget(cards, CardId.LadyLabrynthOfTheSilverCastle);
                    if (lady != null)
                    {
                        if (Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)
                        && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasRace(CardRace.Fiend) && card.Level >= 8 && !card.HasType(CardType.Xyz | CardType.Link)))
                        {
                            selection.Add(lady);
                        }
                    }

                    bool attackFlag = CheckCanDirectAttack();
                    bool defenseFlag = Bot.UnderAttack && Bot.GetMonsterCount() == 0;
                    if (attackFlag || defenseFlag)
                    {
                        ClientCard bestPowerMonster = null;
                        int bestPower = -1;
                        foreach (ClientCard target in cards)
                        {
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(target.Id);
                            if (cardData != null)
                            {
                                int power = attackFlag ? cardData.Attack : Math.Max(cardData.Attack, cardData.Defense);
                                if (bestPowerMonster == null || power > bestPower)
                                {
                                    bestPowerMonster = target;
                                    bestPower = power;
                                }
                            }
                        }
                        if (defenseFlag || GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack() + bestPower >= Enemy.LifePoints)
                        {
                            ClientCard realTarget = GetWelcomeOrBigWelcomeTarget(cards, bestPowerMonster.Id);
                            if (realTarget != null) selection.Add(realTarget);
                        }
                    }

                    List<int> checkIdList = new List<int>{CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock, CardId.AriasTheLabrynthButler};
                    foreach (int checkId in checkIdList)
                    {
                        if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId))
                        {
                            ClientCard target = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                            if (target != null) selection.Add(target);
                        }
                    }

                    List<int> fullCheckIdList = new List<int>{
                        CardId.LadyLabrynthOfTheSilverCastle, CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock,
                        CardId.AriasTheLabrynthButler, CardId.ArianeTheLabrynthServant, CardId.AriannaTheLabrynthServant
                    };
                    foreach (int checkId in fullCheckIdList)
                    {
                        ClientCard target = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                        if (target != null && !selection.Contains(target)) selection.Add(target);
                    }

                    if (selection.Count() > 0) return Util.CheckSelectCount(selection, cards, min, max);
                }

                bool searchFlag = currentSolvingChain.IsCode(CardId.AriannaTheLabrynthServant) && hint == HintMsg.AddToHand;
                bool bigwelcomeSoving = currentSolvingChain.IsCode(CardId.BigWelcomeLabrynth) || (currentSolvingChain.IsCode(CardId.TransactionRollback) && rollbackCopyCardId == CardId.BigWelcomeLabrynth);
                searchFlag |= bigwelcomeSoving && hint == HintMsg.SpSummon && Bot.GetMonsterCount() == 0;
                if (searchFlag)
                {
                    Logger.DebugWriteLine("rewrite search.");
                    List<ClientCard> selection = new List<ClientCard>();

                    List<int> furnitureCheckIdList = new List<int> { CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, CardId.LabrynthChandraglier };
                    ClientCard bigWelcome = GetWelcomeOrBigWelcomeTarget(cards, CardId.BigWelcomeLabrynth);
                    ClientCard welcome = GetWelcomeOrBigWelcomeTarget(cards, CardId.WelcomeLabrynth);
                    ClientCard arianna = GetWelcomeOrBigWelcomeTarget(cards, CardId.AriannaTheLabrynthServant);

                    // search big welcome to activate this turn
                    if (Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2)
                    {
                        if (!summoned && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !CheckWhetherNegated(true, true, CardType.Monster)
                            && CheckCalledbytheGrave(CardId.AriannaTheLabrynthServant) == 0 && arianna != null && !Bot.HasInHand(CardId.AriannaTheLabrynthServant))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { arianna }, cards, min, max);
                        }
                        if (!CheckShouldNoMoreSpSummon())
                        {
                            if (bigWelcome != null && !activatedCardIdList.Contains(CardId.AriasTheLabrynthButler)
                                && Bot.HasInHandOrHasInMonstersZone(CardId.AriasTheLabrynthButler))
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { bigWelcome }, cards, min, max);
                            }

                            bool canActivateSetTrap = (cooclockAffected || Bot.HasInHand(CardId.LabrynthCooclock))
                                && Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth));
                            if (canActivateSetTrap && !Bot.HasInHandOrInSpellZone(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth))
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { AriannaSearchWelcomeTrap(cards, CardId.BigWelcomeLabrynth) }, cards, min, max);
                            }
                        }
                    }

                    // search cooclock/bulter to activate trap
                    ClientCard arias = null;
                    ClientCard cooclock = null;
                    if (!activatedCardIdList.Contains(CardId.AriasTheLabrynthButler) && CheckRemainInDeck(CardId.AriasTheLabrynthButler) > 0
                        && !Bot.HasInHand(CardId.AriasTheLabrynthButler))
                    {
                        arias = GetWelcomeOrBigWelcomeTarget(cards, CardId.AriasTheLabrynthButler);
                    }
                    if (!activatedCardIdList.Contains(CardId.LabrynthCooclock) && CheckRemainInDeck(CardId.LabrynthCooclock) > 0
                        && !Bot.HasInHand(CardId.LabrynthCooclock))
                    {
                        cooclock = GetWelcomeOrBigWelcomeTarget(cards, CardId.LabrynthCooclock);
                    }
                    if (arias != null || cooclock != null)
                    {
                        SortedDictionary<int, Func<bool>> trapCheckDict = new SortedDictionary<int, Func<bool>> {
                            {CardId.BigWelcomeLabrynth, BigWelcomeLabrynthSetCheck},
                            {_CardId.DimensionalBarrier, DimensionalBarrierActivate},
                            {CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonSetCheck},
                            {CardId.WelcomeLabrynth, WelcomeLabrynthSetCheck}
                        };
                        foreach (KeyValuePair<int, Func<bool>> checkPair in trapCheckDict)
                        {
                            if (Bot.GetSpells().Any(card => card.IsFacedown() && !setTrapThisTurn.Contains(card) && card.IsCode(checkPair.Key))) continue;
                            if (!activatedCardIdList.Contains(checkPair.Key))
                            {
                                if (Bot.GetSpells().Any(card => card.IsFacedown() && setTrapThisTurn.Contains(card) && card.IsCode(checkPair.Key))
                                    && cooclock != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { cooclock }, cards, min, max);
                                }
                                else if (Bot.HasInHand(checkPair.Key))
                                {
                                    if (checkPair.Value())
                                    {
                                        if (arias != null)
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { arias }, cards, min, max);
                                        }
                                        if (Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2 && cooclock != null)
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { cooclock }, cards, min, max);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    bool lackUnimportantCost = !Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth));
                    if (lackUnimportantCost)
                    {
                        List<ClientCard> handCost = Bot.Hand.Where(card => card != Card).ToList();
                        lackUnimportantCost &= handCost.Count() <= 2 && !handCost.Any(card => !card.IsCode(_CardId.MaxxC, _CardId.AshBlossom, CardId.LabrynthCooclock));
                    }
                    if (!lackUnimportantCost && cooclock != null && bigWelcome != null)
                    {
                        foreach (int furnitureId in furnitureCheckIdList)
                        {
                            if (furnitureId == CardId.LabrynthCooclock) continue;
                            if (CheckCalledbytheGrave(furnitureId) == 0 && !activatedCardIdList.Contains(furnitureId) && Bot.HasInHand(furnitureId))
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { cooclock }, cards, min, max);
                            }
                        }
                    }

                    if (Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2)
                    {
                        // search not exist furniture
                        if (!lackUnimportantCost)
                        {
                            foreach (int checkId in furnitureCheckIdList)
                            {
                                ClientCard furniture = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId) && furniture != null)
                                {
                                    if (checkId == CardId.LabrynthCooclock)
                                    {
                                        if (Enemy.GetMonsterCount() > 0 && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LabrynthChandraglier)) continue;
                                    }
                                    return Util.CheckSelectCount(new List<ClientCard> { furniture }, cards, min, max);
                                }
                            }
                        }

                        // search big welcome
                        if (bigWelcome != null)
                        {
                            bool needSpSummonLovely = !Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true)
                                && !Bot.HasInHandOrInSpellZone(CardId.BigWelcomeLabrynth) && cards.Any(c => c.IsCode(CardId.LovelyLabrynthOfTheSilverCastle));
                            needSpSummonLovely |= Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) && !Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth);
                            if (needSpSummonLovely)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { AriannaSearchWelcomeTrap(cards, CardId.BigWelcomeLabrynth) }, cards, min, max);
                            }
                        }
                        // search welcome
                        if (welcome != null && Bot.HasInHandOrInSpellZone(CardId.BigWelcomeLabrynth) && !Bot.HasInHandOrInSpellZone(CardId.WelcomeLabrynth))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { AriannaSearchWelcomeTrap(cards, CardId.WelcomeLabrynth) }, cards, min, max);
                        }
                    }

                    // search big welcome/arias
                    if (Duel.Player == 1 && (Duel.Phase <= DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                        && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.AriasTheLabrynthButler)
                        && !Bot.HasInSpellZone(CardId.BigWelcomeLabrynth))
                    {
                        if (Bot.HasInHand(CardId.BigWelcomeLabrynth) && !Bot.HasInHandOrHasInMonstersZone(CardId.AriasTheLabrynthButler) && arias != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { arias }, cards, min, max);
                        }
                        if (Bot.HasInHand(CardId.AriasTheLabrynthButler) && !Bot.HasInHandOrHasInMonstersZone(CardId.BigWelcomeLabrynth) && bigWelcome != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { bigWelcome }, cards, min, max);
                        }
                    }

                    // search lady
                    ClientCard lady = GetWelcomeOrBigWelcomeTarget(cards, CardId.LadyLabrynthOfTheSilverCastle);
                    bool haveTrap = Duel.Player == 0 && Bot.Hand.Any(card => card.Type == (int)CardType.Trap) && Duel.Phase <= DuelPhase.Main2;
                    haveTrap |= Bot.GetSpells().Any(card => card.IsFacedown() && card.Type == (int)CardType.Trap);
                    if (!Bot.HasInHandOrHasInMonstersZone(CardId.LadyLabrynthOfTheSilverCastle) && !activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle)
                        && haveTrap && lady != null)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { lady }, cards, min, max);
                    }

                    if (!activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !CheckWhetherNegated(true, true, CardType.Monster)
                        && CheckCalledbytheGrave(CardId.AriannaTheLabrynthServant) == 0 && arianna != null && !Bot.HasInHand(CardId.AriannaTheLabrynthServant))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { arianna }, cards, min, max);
                    }

                    // search not exist furniture
                    if (!lackUnimportantCost)
                    {
                        foreach (int checkId in furnitureCheckIdList)
                        {
                            ClientCard furniture = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId) && furniture != null)
                            {
                                if (checkId == CardId.LabrynthCooclock)
                                {
                                    if (Enemy.GetMonsterCount() > 0 && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LabrynthChandraglier)) continue;
                                }
                                return Util.CheckSelectCount(new List<ClientCard> { furniture }, cards, min, max);
                            }
                        }
                    }

                    // search not exist card
                    List<int> uniqueCheckIdList = new List<int>{
                        CardId.BigWelcomeLabrynth, CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, CardId.LabrynthChandraglier,
                        CardId.LadyLabrynthOfTheSilverCastle, CardId.AriasTheLabrynthButler, CardId.ArianeTheLabrynthServant, CardId.WelcomeLabrynth};
                    foreach (int checkId in uniqueCheckIdList)
                    {
                        ClientCard targetCard = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                        if (!Bot.HasInMonstersZone(checkId) && !Bot.HasInHandOrInSpellZone(checkId) && targetCard != null)
                        {
                            if (checkId == CardId.BigWelcomeLabrynth || checkId == CardId.WelcomeLabrynth)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { AriannaSearchWelcomeTrap(cards, checkId) }, cards, min, max);
                            }
                            else
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { targetCard }, cards, min, max);
                            }
                        }
                    }

                    foreach (int checkId in uniqueCheckIdList)
                    {
                        ClientCard targetCard = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                        if (CheckRemainInDeck(checkId) > 0)
                        {
                            if (checkId == CardId.BigWelcomeLabrynth || checkId == CardId.WelcomeLabrynth)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { AriannaSearchWelcomeTrap(cards, checkId) }, cards, min, max);
                            }
                            else
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { targetCard }, cards, min, max);
                            }
                        }
                    }
                }

                // solved when have more than 1 monster
                if (bigwelcomeSoving && hint == HintMsg.SpSummon)
                {
                    bool activateTimingFlag = Duel.Phase > DuelPhase.Main2 || (Card.IsCode(CardId.AriasTheLabrynthButler) && (CurrentTiming & hintTimingMainEnd) > 0);

                    bool needDestroyFlag = GetProblematicEnemyCardList(false).Count() > 0;
                    needDestroyFlag |= activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && activateTimingFlag;
                    needDestroyFlag |= Bot.UnderAttack && (Bot.BattlingMonster?.GetDefensePower() ?? 0) <= (Enemy.BattlingMonster?.GetDefensePower() ?? 0) && Duel.LastChainPlayer != 0;
                    needDestroyFlag |= Duel.Turn == 1 && Duel.Player == 0 && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1);
                    needDestroyFlag |= Duel.Turn == 1 && Enemy.GetMonsterCount() == 0 && Enemy.GetSpellCount() == 0 && Enemy.Hand.Count > 0
                        && (CurrentTiming & hintTimingMainEnd) > 0;

                    if (needDestroyFlag && cards.Any(c => c.IsCode(CardId.LovelyLabrynthOfTheSilverCastle))
                        && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, CardId.LovelyLabrynthOfTheSilverCastle) }, cards, min, max);
                    }
                    if (cards.Any(c => c.IsCode(CardId.AriannaTheLabrynthServant))
                        && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !Bot.HasInMonstersZone(CardId.AriannaTheLabrynthServant))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, CardId.AriannaTheLabrynthServant) }, cards, min, max);
                    }
                    if (cards.Any(c => c.IsCode(CardId.LovelyLabrynthOfTheSilverCastle))
                        && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, CardId.LovelyLabrynthOfTheSilverCastle) }, cards, min, max);
                    }
                    if (cards.Any(c => c.IsCode(CardId.AriannaTheLabrynthServant))
                        && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, CardId.AriannaTheLabrynthServant) }, cards, min, max);
                    }
                    if (cards.Any(c => c.IsCode(CardId.LadyLabrynthOfTheSilverCastle))
                        && Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2 && Duel.Player == 0 && Enemy.GetMonsterCount() == 0)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, CardId.LadyLabrynthOfTheSilverCastle) }, cards, min, max);
                    }
                    List<int> furnitureCheckIdList = new List<int> { CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, CardId.LabrynthChandraglier, CardId.AriasTheLabrynthButler };
                    foreach (int furniture in furnitureCheckIdList)
                    {
                        if (cards.Any(c => c.IsCode(furniture)) && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(furniture))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, furniture) }, cards, min, max);
                        }
                    }
                    List<int> checkIdList = new List<int>{CardId.ArianeTheLabrynthServant, CardId.LadyLabrynthOfTheSilverCastle, CardId.AriannaTheLabrynthServant,
                        CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, CardId.LabrynthChandraglier, CardId.AriasTheLabrynthButler};
                    foreach (int checkId in checkIdList)
                    {
                        if (cards.Any(c => c.IsCode(checkId)))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { GetWelcomeOrBigWelcomeTarget(cards, checkId) }, cards, min, max);
                        }
                    }
                    // should not get here
                    Logger.DebugWriteLine("[warning] call BigWelcomeSpSummon with no select.");
                }

                if (bigwelcomeSoving && hint == HintMsg.ReturnToHand)
                {
                    if (bigwelcomeEscaseTarget != null && cards.Contains(bigwelcomeEscaseTarget))
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { bigwelcomeEscaseTarget }, cards, min, max);
                    }

                    ClientCard cooclock = cards.FirstOrDefault(c => c.IsCode(CardId.LabrynthCooclock));
                    bool canSearchWelcome = CheckRemainInDeck(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth) > 0
                                        && (Bot.HasInHandOrHasInMonstersZone(new List<int> { CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie })
                                            || summonInChainList.Any(c => c.IsCode(CardId.AriannaTheLabrynthServant))
                                            || (Duel.Player == 0 && !summoned && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && Bot.HasInHand(CardId.AriannaTheLabrynthServant)));
                    if (cooclock != null)
                    {
                        if (setTrapThisTurn.Count() > 0
                            || (Duel.Turn == 1 && (
                                    (!activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle) && Bot.HasInHandOrHasInMonstersZone(CardId.LadyLabrynthOfTheSilverCastle))
                                    || canSearchWelcome
                                )
                            )
                            || Duel.Turn == 0 && canSearchWelcome
                        )
                        return Util.CheckSelectCount(new List<ClientCard> { cooclock }, cards, min, max);
                    }

                    ClientCard defenseLady = cards.FirstOrDefault(c => c.IsDefense() && c.IsCode(CardId.LadyLabrynthOfTheSilverCastle));
                    ClientCard attackLady = cards.FirstOrDefault(c => c.IsAttack() && c.IsCode(CardId.LadyLabrynthOfTheSilverCastle));
                    if (Bot.GetMonsters().Any(card => (Duel.Player == 1 || card.IsDefense()) && card.IsCode(CardId.LadyLabrynthOfTheSilverCastle))
                        && (!activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle) || activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle + 1)))
                    {
                        if (defenseLady != null) return Util.CheckSelectCount(new List<ClientCard> { defenseLady }, cards, min, max);
                        if (attackLady != null) return Util.CheckSelectCount(new List<ClientCard> { attackLady }, cards, min, max);
                    }
                    if (summonInChainList.Any(c => c.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)))
                    {
                        List<int> returnCheckIdList = new List<int>{
                                _CardId.MaxxC, CardId.AriannaTheLabrynthServant, _CardId.AshBlossom, CardId.LabrynthCooclock, CardId.LadyLabrynthOfTheSilverCastle,
                                CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie, CardId.AriasTheLabrynthButler, CardId.UnchainedSoulOfSharvara, CardId.ArianeTheLabrynthServant
                            };
                        foreach (int checkId in returnCheckIdList)
                        {
                            ClientCard returnTarget = cards.FirstOrDefault(c => c.IsCode(checkId));
                            if (returnTarget != null) return Util.CheckSelectCount(new List<ClientCard> { returnTarget }, cards, min, max);
                        }
                        return Util.CheckSelectCount(cards.OrderBy(card => card.Attack).ToList(), cards, min, max);
                    }
                    if (cards.Count() == 1) return Util.CheckSelectCount(cards.OrderBy(card => card.Attack).ToList(), cards, min, max);
                    ClientCard ariannaNotSummon = cards.FirstOrDefault(c => c.IsCode(CardId.AriannaTheLabrynthServant) && !summonInChainList.Contains(c));
                    if (ariannaNotSummon != null) return Util.CheckSelectCount(new List<ClientCard> { ariannaNotSummon }, cards, min, max);
                    else
                    {
                        // compare which have lower attack
                        ClientCard fieldTarget = Bot.GetMonsters().Where(card => !card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle))
                            .OrderBy(card => card.Attack).FirstOrDefault();
                        if (fieldTarget != null) return Util.CheckSelectCount(new List<ClientCard> { fieldTarget }, cards, min, max);
                    }
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public ClientCard GetWelcomeOrBigWelcomeTarget(IList<ClientCard> cards, int cardId)
        {
            ClientCard graveTarget = cards.FirstOrDefault(card => card.IsCode(cardId) && card.Location == CardLocation.Grave);
            if (graveTarget != null) return graveTarget;
            ClientCard deckTarget = cards.FirstOrDefault(card => card.IsCode(cardId) && card.Location == CardLocation.Deck);
            if (deckTarget != null) return deckTarget;
            ClientCard handTarget = cards.FirstOrDefault(card => card.IsCode(cardId) && card.Location == CardLocation.Hand);
            if (handTarget != null) return handTarget;
            return null;
        }

        public ClientCard AriannaSearchWelcomeTrap(IList<ClientCard> cards, int welcomeId)
        {
            bool haveCostToSolve = Bot.HasInHand(new List<int> { CardId.LovelyLabrynthOfTheSilverCastle, CardId.TransactionRollback, CardId.AriasTheLabrynthButler });
            if (haveCostToSolve)
            {
                List<int> checkIdList = new List<int> { CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier };
                foreach (int checkId in checkIdList)
                {
                    ClientCard targetCard = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                    if (targetCard != null && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId) && CheckCalledbytheGrave(checkId) == 0 && !activatedCardIdList.Contains(checkId))
                    {
                        return targetCard;
                    }
                }
                foreach (int checkId in checkIdList)
                {
                    ClientCard targetCard = GetWelcomeOrBigWelcomeTarget(cards, checkId);
                    if (targetCard != null && CheckCalledbytheGrave(checkId) == 0 && !activatedCardIdList.Contains(checkId))
                    {
                        return targetCard;
                    }
                }
            }

            return GetWelcomeOrBigWelcomeTarget(cards, welcomeId);
        }

        /// <summary>
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
        }
        
        public override bool OnSelectMonsterSummonOrSet(ClientCard card)
        {
            if (card.Attack > 0 && CheckCanDirectAttack()) return false;
            if (card.Attack <= 1000) return true;

            return base.OnSelectMonsterSummonOrSet(card);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                // selected in summon process
                if (cardId == CardId.RelinquishedAnima)
                {
                    return base.OnSelectPlace(cardId, player, location, available);
                }
                if (cardId == CardId.UnchainedSoulLordOfYama || cardId == CardId.UnchainedSoulOfAnguish)
                {
                    if (Bot.MonsterZone[0] != null && Bot.MonsterZone[2] != null && (Zones.z6 & available) != 0) return Zones.z6;
                    if (Bot.MonsterZone[2] != null && Bot.MonsterZone[4] != null && (Zones.z5 & available) != 0) return Zones.z5;
                }
                if (cardId == CardId.MuckrakerFromTheUnderworld || cardId == CardId.UnchainedSoulOfRage)
                {
                    if (Bot.MonsterZone[1] != null && (Zones.z6 & available) != 0) return Zones.z6;
                    if (Bot.MonsterZone[3] != null && (Zones.z5 & available) != 0) return Zones.z5;
                }

                List<int> zoneIdList = ShuffleList(new List<int> { 0, 2, 4 });
                zoneIdList.AddRange(ShuffleList(new List<int> { 1, 3 }));
                zoneIdList.AddRange(ShuffleList(new List<int> { 5, 6 }));
                foreach (int zoneId in zoneIdList)
                {
                    int zone = (int)System.Math.Pow(2, zoneId);
                    if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                    {
                        return zone;
                    }
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2)
                {
                    bool turnDefense = false;
                    if (cardData.Attack <= cardData.Defense)
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
                    if (cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                int cardAttack = cardData.Attack;
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardAttack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    return CardPosition.FaceUpDefence;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public override int OnSelectOption(IList<int> options)
        {
            // override for cooclock
            if (options.Count() == 2 && options.Contains(1190) && options.Contains(1152))
            {
                // 1190=Add to Hand, 1152=Special Summon
                // return to hand to activate trap set this turn
                bool canLink = Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2;
                if (!canLink && !Bot.HasInHand(CardId.LabrynthCooclock) && Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth))
                    && !activatedCardIdList.Contains(CardId.LabrynthCooclock) && !CheckWhetherWillbeRemoved()
                    && (activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) || Bot.GetSpells().All(card => setTrapThisTurn.Contains(card) || !card.IsCode(CardId.BigWelcomeLabrynth)))
                    && setTrapThisTurn.Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth, _CardId.DimensionalBarrier, _CardId.InfiniteImpermanence, CardId.DestructiveDarumaKarmaCannon)))
                {
                    return options.IndexOf(1190);
                }
                if (!enemyActivateMaxxC) return options.IndexOf(1152);
                if (activatedCardIdList.Contains(CardId.LabrynthCooclock))
                {
                    if (!CheckShouldNoMoreSpSummon()) return options.IndexOf(1152);
                }
                return options.IndexOf(1190);
            }

            // overrride for lovely
            if (options.Contains(Util.GetStringId(CardId.LovelyLabrynthOfTheSilverCastle, 3)) && options.Contains(Util.GetStringId(CardId.LovelyLabrynthOfTheSilverCastle, 4)))
            {
                int botWorstAttack = 0;
                ClientCard botWorstMonster = Util.GetWorstBotMonster(true);
                if (botWorstMonster != null)
                {
                    botWorstAttack = botWorstMonster.Attack;
                }
                List<ClientCard> targetList = GetProblematicEnemyCardList(false);
                List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                    && card.GetDefensePower() >= botWorstAttack && !currentDestroyCardList.Contains(card)).ToList();
                enemyMonster.Sort(CardContainer.CompareCardAttack);
                enemyMonster.Reverse();
                targetList.AddRange(enemyMonster);
                targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => !currentDestroyCardList.Contains(card) && card.IsFacedown()).ToList()));
                if (targetList.Count() > 0)
                {
                    currentDestroyCardList.Add(targetList[0]);
                    AI.SelectCard(targetList);
                    return options.IndexOf(Util.GetStringId(CardId.LovelyLabrynthOfTheSilverCastle, 4));
                }
                else
                {
                    return options.IndexOf(Util.GetStringId(CardId.LovelyLabrynthOfTheSilverCastle, 3));
                }
            }

            // override for dimensional barrier
            if (options.IndexOf(HintMsg.RITUAL) >= 0 || options.IndexOf(HintMsg.FUSION) >= 0 || options.IndexOf(HintMsg.SYNCHRO) >= 0
                || options.IndexOf(HintMsg.XYZ) >= 0 || options.IndexOf(HintMsg.PENDULUM) >= 0)
            {
                Dictionary<int, Func<bool>> barrierCheckDict = new Dictionary<int, Func<bool>>
                {
                    {HintMsg.RITUAL, DimensionalBarrierForRitual},
                    {HintMsg.FUSION, DimensionalBarrierForFusion},
                    {HintMsg.SYNCHRO, DimensionalBarrierForSynchro},
                    {HintMsg.XYZ, DimensionalBarrierForXyz},
                    {HintMsg.PENDULUM, DimensionalBarrierForPendulum},
                };
                dimensionBarrierAnnouncing = true;
                foreach (KeyValuePair<int, Func<bool>> checkPair in barrierCheckDict)
                {
                    if (options.Contains(checkPair.Key) && checkPair.Value())
                    {
                        dimensionBarrierAnnouncing = false;
                        dimensionalBarrierAnnouced.Add(checkPair.Key);
                        return options.IndexOf(checkPair.Key);
                    }
                }
                dimensionBarrierAnnouncing = false;
                List<ClientCard> enemyMonsterList = new List<ClientCard>(Enemy.GetMonsters());
                enemyMonsterList.AddRange(Enemy.GetGraveyardMonsters());
                Dictionary<int, bool> barrierCheckSecondark = new Dictionary<int, bool>
                {
                    {HintMsg.RITUAL, enemyMonsterList.Any(card => card.HasType(CardType.Ritual))},
                    {HintMsg.FUSION, enemyMonsterList.Any(card => card.HasType(CardType.Fusion))},
                    {HintMsg.SYNCHRO, enemyMonsterList.Any(card => card.HasType(CardType.Synchro))},
                    {HintMsg.XYZ, enemyMonsterList.Any(card => card.HasType(CardType.Xyz))},
                    {HintMsg.PENDULUM, enemyMonsterList.Any(card => card.HasType(CardType.Pendulum))},
                };
                foreach (KeyValuePair<int, bool> checkPair in barrierCheckSecondark)
                {
                    if (options.Contains(checkPair.Key) && checkPair.Value)
                    {
                        dimensionBarrierAnnouncing = false;
                        dimensionalBarrierAnnouced.Add(checkPair.Key);
                        return options.IndexOf(checkPair.Key);
                    }
                }
                List<int> annouceList = new List<int> { HintMsg.XYZ, HintMsg.SYNCHRO, HintMsg.FUSION, HintMsg.PENDULUM, HintMsg.RITUAL };
                foreach (int annouce in annouceList)
                {
                    if (options.Contains(annouce))
                    {
                        return options.IndexOf(annouce);
                    }
                }
            }

            // override for servant
            // sp summon
            if (options.Contains(Util.GetStringId(CardId.AriannaTheLabrynthServant, 2)) || options.Contains(Util.GetStringId(CardId.ArianeTheLabrynthServant, 2)))
            {
                if (GetEmptyMainMonsterZoneCount() > chainSummoningIdList.Count())
                {
                    bool checkFlag = false;
                    if (!activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && Bot.HasInHand(CardId.AriannaTheLabrynthServant)
                        && !CheckWhetherNegated(true, true, CardType.Monster) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                    {
                        checkFlag = true;
                        AI.SelectCard(CardId.AriannaTheLabrynthServant);
                    }
                    if (!checkFlag)
                    {
                        List<int> checkIdList = new List<int> { CardId.LovelyLabrynthOfTheSilverCastle, CardId.LadyLabrynthOfTheSilverCastle };
                        foreach (int checkId in checkIdList)
                        {
                            if (Bot.HasInHand(checkId))
                            {
                                checkFlag = true;
                                AI.SelectCard(checkId);
                                break;
                            }
                        }
                    }
                    if (!checkFlag && Duel.Player == 0 && Duel.Phase < DuelPhase.End)
                    {
                        List<ClientCard> linkMaterialList = GetCanBeUsedForLinkMaterial(true, card => !card.HasRace(CardRace.Fiend));
                        if (linkMaterialList.Count() + chainSummoningIdList.Count() == 2)
                        {
                            // summon less atk for link
                            ClientCard selected = null;
                            int attack = 0;
                            foreach (ClientCard hand in Bot.Hand)
                            {
                                YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(hand.Id);
                                if (cardData == null || cardData.Race != (int)CardRace.Fiend) continue;
                                if (selected == null || attack > hand.Attack)
                                {
                                    selected = hand;
                                    attack = hand.Attack;
                                }
                            }
                            if (selected != null)
                            {
                                checkFlag = true;
                                AI.SelectCard(selected);
                            }
                        }
                    }
                    if (!checkFlag && CheckCanDirectAttack())
                    {
                        // summon best attack for attack
                        ClientCard selected = null;
                        int attack = 0;
                        foreach (ClientCard hand in Bot.Hand)
                        {
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(hand.Id);
                            if (cardData == null || cardData.Race != (int)CardRace.Fiend) continue;
                            if (selected == null || attack < hand.Attack)
                            {
                                selected = hand;
                                attack = hand.Attack;
                            }
                        }
                        if (selected != null)
                        {
                            checkFlag = true;
                            AI.SelectCard(selected);
                        }
                    }

                    if (checkFlag)
                    {
                        if (options.Contains(Util.GetStringId(CardId.AriannaTheLabrynthServant, 2))) return options.IndexOf(Util.GetStringId(CardId.AriannaTheLabrynthServant, 2));
                        if (options.Contains(Util.GetStringId(CardId.ArianeTheLabrynthServant, 2))) return options.IndexOf(Util.GetStringId(CardId.ArianeTheLabrynthServant, 2));
                    }
                }
            }

            // set
            if (options.Contains(Util.GetStringId(CardId.AriannaTheLabrynthServant, 3)) || options.Contains(Util.GetStringId(CardId.ArianeTheLabrynthServant, 3)))
            {
                if (!Util.ChainContainsCard(CardId.WelcomeLabrynth) || Bot.GetSpellCountWithoutField() < 4)
                {
                    List<int> checkIdList = new List<int> { CardId.BigWelcomeLabrynth, CardId.WelcomeLabrynth, _CardId.InfiniteImpermanence, _CardId.DimensionalBarrier, CardId.DestructiveDarumaKarmaCannon };
                    foreach (int checkId in checkIdList)
                    {
                        if (Bot.HasInHand(checkId) && (checkId == _CardId.InfiniteImpermanence || !Bot.HasInSpellZone(checkId, faceUp:false)))
                        {
                            AI.SelectCard(checkId);
                            if (options.Contains(Util.GetStringId(CardId.AriannaTheLabrynthServant, 3))) return options.IndexOf(Util.GetStringId(CardId.AriannaTheLabrynthServant, 3));
                            if (options.Contains(Util.GetStringId(CardId.ArianeTheLabrynthServant, 3))) return options.IndexOf(Util.GetStringId(CardId.ArianeTheLabrynthServant, 3));
                        }
                    }
                }
            }

            // do nothing
            if (options.Contains(Util.GetStringId(CardId.AriannaTheLabrynthServant, 4))) return options.IndexOf(Util.GetStringId(CardId.AriannaTheLabrynthServant, 4));
            if (options.Contains(Util.GetStringId(CardId.ArianeTheLabrynthServant, 4))) return options.IndexOf(Util.GetStringId(CardId.ArianeTheLabrynthServant, 4));

            return base.OnSelectOption(options);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == 96)
            {
                Logger.DebugWriteLine("*** muckraker replace.");
                AI.SelectCard(Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasRace(CardRace.Fiend)).OrderBy(card => card.Attack).ToList());
                return true;
            }
            return base.OnSelectYesNo(desc);
        }

        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1)
            {
                dimensionShifterCount = 0;

                enemySpSummonFromExLastTurn = 0;
                enemySpSummonFromExThisTurn = 0;
                banSpSummonExceptFiendCount = 0;
            }
            enemyActivateMaxxC = false;
            enemySpSummonFromExLastTurn = enemySpSummonFromExThisTurn;
            enemySpSummonFromExThisTurn = 0;
            rollbackCopyCardId = 0;

            if (dimensionShifterCount > 0) dimensionShifterCount--;
            if (banSpSummonExceptFiendCount > 0) banSpSummonExceptFiendCount--;
            infiniteImpermanenceList.Clear();

            summoned = false;
            cooclockAffected = false;
            activatedCardIdList.Clear();
            setTrapThisTurn.Clear();
            summonThisTurn.Clear();
            enemySetThisTurn.Clear();
            dimensionalBarrierAnnouced.Clear();
            summonInChainList.Clear();
            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;
            if (chainSummoningIdList.Count() > 0)
                Logger.DebugWriteLine("[Welcome] Summoning: " + string.Join(",", chainSummoningIdList) + "\n");

            if (player == 1)
            {
                if (card.IsCode(_CardId.InfiniteImpermanence))
                {
                    if (enemyActivateInfiniteImpermanenceFromHand)
                    {
                        enemyActivateInfiniteImpermanenceFromHand = false;
                    }
                    else {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Enemy.SpellZone[i] == card)
                            {
                                infiniteImpermanenceList.Add(4-i);
                                break;
                            }
                        }
                    }
                }
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            if (currentCard != null && !Duel.IsCurrentSolvingChainNegated())
            {
                if (currentCard.Controller == 1)
                {
                    if (currentCard.IsCode(_CardId.MaxxC))
                        enemyActivateMaxxC = true;
                    if (currentCard.IsCode(CardId.DimensionShifter))
                        dimensionShifterCount = 2;
                }
                if (currentCard.Controller == 0)
                {
                    if (currentCard.IsCode(CardId.LabrynthCooclock))
                        cooclockAffected = true;
                }
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            rollbackCopyCardId = 0;
            currentNegateMonsterList.Clear();
            currentDestroyCardList.Clear();
            escapeTargetList.Clear();
            chainSummoningIdList.Clear();
            summonInChainList.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            for (int idx = enemySetThisTurn.Count() - 1; idx >= 0; idx --)
            {
                ClientCard checkTarget = enemySetThisTurn[idx];
                if (checkTarget == null || checkTarget.Location != CardLocation.SpellZone || checkTarget.HasPosition(CardPosition.FaceUp))
                {
                    enemySetThisTurn.RemoveAt(idx);
                }
            }
            if (cooclockActivating)
                cooclockActivating = false;
            furnitureActivating = false;
            dimensionBarrierAnnouncing = false;
            bigwelcomeEscaseTarget = null;
            base.OnChainEnd();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (previousControler == 1)
            {
                if (previousLocation == (int)CardLocation.Extra && currentLocation == (int)CardLocation.MonsterZone) enemySpSummonFromExThisTurn ++;
                if (card != null)
                {
                    if (card.IsCode(_CardId.InfiniteImpermanence) && previousLocation == (int)CardLocation.Hand && currentLocation == (int)CardLocation.SpellZone)
                        enemyActivateInfiniteImpermanenceFromHand = true;
                    if (card.Location == CardLocation.SpellZone && card.HasPosition(CardPosition.FaceDown))
                        enemySetThisTurn.Add(card);
                }
            }
            if (card != null)
            {
                if (previousControler == 0)
                {
                    if (previousLocation == (int)CardLocation.MonsterZone && currentLocation != (int)CardLocation.MonsterZone)
                    {
                        if (summonThisTurn.Contains(card)) summonThisTurn.Remove(card);
                        if (summonInChainList.Contains(card)) summonInChainList.Remove(card);
                    }
                    if (previousLocation == (int)CardLocation.SpellZone && currentLocation != (int)CardLocation.SpellZone)
                    {
                        if (setTrapThisTurn.Contains(card)) setTrapThisTurn.Remove(card);
                    }
                }
                if (currentControler == 0)
                {
                    ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
                    if (currentLocation == (int)CardLocation.SpellZone && (currentSolvingChain == null || !currentSolvingChain.IsCode(CardId.AriasTheLabrynthButler))
                        && (card.HasType(CardType.Trap) || card.IsCode(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth))
                    )
                    {
                        Logger.DebugWriteLine("[setTrapThisTurn]set " + card.Name ?? "UnknowCard");
                        setTrapThisTurn.Add(card);
                    }
                    if (currentLocation == (int)CardLocation.MonsterZone)
                    {
                        summonThisTurn.Add(card);
                        if (currentSolvingChain != null) summonInChainList.Add(card);
                    }
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
       {
            if (attackers.Count() > 0 && defenders.Count() > 0)
            {
                List<ClientCard> sortedAttacker = attackers.OrderBy(card => card.Attack).ToList();
                for (int k = 0; k < sortedAttacker.Count; ++k)
                {
                    ClientCard attacker = sortedAttacker[k];
                    attacker.IsLastAttacker = k == sortedAttacker.Count - 1;
                    BattlePhaseAction result = OnSelectAttackTarget(attacker, defenders);
                    if (result != null)
                        return result;
                }
            }

            return base.OnBattle(attackers, defenders);
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

        public void ResetCooclockEffect(bool onlyCheck)
        {
            if (!onlyCheck && cooclockAffected && setTrapThisTurn.Contains(Card))
            {
                cooclockAffected = false;
                setTrapThisTurn.Remove(Card);
            }
        }





        public bool LadyLabrynthOfTheSilverCastleFieldActivate()
        {
            if (Card.Location == CardLocation.MonsterZone && (Util.GetLastChainCard() == null || !Util.GetLastChainCard().IsCode(_CardId.EvenlyMatched))
                && (!CheckWhetherNegated() || Enemy.HasInMonstersZone(CardId.LadyLabrynthOfTheSilverCastle)))
            {
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }
            return false;
        }

        public bool LadyLabrynthOfTheSilverCastleHandActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                // sp summon from hand
                if (CheckShouldNoMoreSpSummon(true) || Util.ChainContainsCard(_CardId.EvenlyMatched)) return false;
                bool activateFlag = false;
                activateFlag |= CheckChainContainEnemyMaxxC();
                if (!activateFlag && GetEmptyMainMonsterZoneCount() + chainSummoningIdList.Count() <= 0)
                {
                    return false;
                }
                activateFlag |= cooclockAffected && setTrapThisTurn.Count() > 0 && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth));
                activateFlag |= Bot.GetSpells().Any(card => card.IsFacedown() && card.HasType(CardType.Trap) && !setTrapThisTurn.Contains(card))
                    && !Bot.HasInMonstersZone(Card.Id, true, faceUp: true);
                activateFlag |= Duel.Player == 1 && Duel.Phase >= DuelPhase.End;
                activateFlag |= setTrapThisTurn.Count() > 0 && Duel.Phase >= DuelPhase.End;
                activateFlag |= Bot.UnderAttack && Bot.GetMonsterCount() == 0 && !Util.ChainContainsCard(CardId.DestructiveDarumaKarmaCannon);
                // for link
                if (Bot.HasInExtra(CardId.UnchainedSoulLordOfYama) && Duel.Player == 0 && Duel.Phase < DuelPhase.End)
                {
                    // check whether need summon for material count
                    List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => !card.HasRace(CardRace.Fiend));
                    int materialCount = materialList.Count();
                    if (!activatedCardIdList.Contains(CardId.UnchainedSoulOfSharvara) && Bot.GetSpells().Any(card => card.IsFacedown())
                        && (!activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama) || Bot.HasInHand(CardId.UnchainedSoulOfSharvara))) materialCount++;
                    if ( (materialCount == 2 || materialCount == 1 && materialList.Any(card => card.IsCode(CardId.UnchainedSoulLordOfYama)))
                        && Bot.HasInExtra(CardId.UnchainedSoulLordOfYama) || materialList.Any(card => card.HasSetcode(SetcodeUnchained)))
                    {
                        activateFlag |= Enemy.GetMonsterCount() > 0 && Bot.HasInExtra(CardId.UnchainedSoulOfAnguish);
                        activateFlag |= Bot.HasInExtra(CardId.UnchainedSoulOfRage);
                    }
                }

                if (activateFlag)
                {
                    activatedCardIdList.Add(Card.Id);
                    chainSummoningIdList.Add(Card.Id);
                    return true;
                }
            }
            return false;
        }

        public bool LovelyLabrynthOfTheSilverCastleActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.LovelyLabrynthOfTheSilverCastle, 0))
            {
                // set from GY
                List<int> checkIdList = new List<int>{CardId.BigWelcomeLabrynth, _CardId.InfiniteImpermanence, _CardId.DimensionalBarrier,
                    CardId.DestructiveDarumaKarmaCannon, CardId.WelcomeLabrynth};
                foreach (int checkId in checkIdList)
                {
                    if (Bot.HasInGraveyard(checkId) && !Bot.HasInHandOrInSpellZone(checkId))
                    {
                        AI.SelectCard(checkId);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
                foreach (int checkId in checkIdList)
                {
                    if (Bot.HasInGraveyard(checkId))
                    {
                        AI.SelectCard(checkId);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                if (GetCanBeUsedForLinkMaterial(true, card => !card.HasRace(CardRace.Fiend)).Count() == 2
                    && !activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama)
                    && (Bot.HasInGraveyard(CardId.UnchainedSoulOfSharvara) || CheckRemainInDeck(CardId.UnchainedSoulOfSharvara) > 0))
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            else
            {
                // destroy
                if (Enemy.GetHandCount() == 0)
                {
                    int botWorstAttack = 0;
                    ClientCard botWorstMonster = Util.GetWorstBotMonster(true);
                    if (botWorstMonster != null)
                    {
                        botWorstAttack = botWorstMonster.Attack;
                    }
                    List<ClientCard> targetList = GetProblematicEnemyCardList(false);
                    List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                        && card.GetDefensePower() >= botWorstAttack && !currentDestroyCardList.Contains(card)).ToList();
                    enemyMonster.Sort(CardContainer.CompareCardAttack);
                    enemyMonster.Reverse();
                    targetList.AddRange(enemyMonster);
                    targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => !currentDestroyCardList.Contains(card)).ToList()));

                    if (targetList.Count() > 0)
                    {
                        currentDestroyCardList.Add(targetList[0]);
                        AI.SelectCard(targetList);
                        AI.SelectOption(1);
                    }
                    else
                    {
                        AI.SelectOption(0);
                    }
                }
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool UnchainedSoulOfSharvaraActivate()
        {
            // search
            if (Card.Location == CardLocation.Grave)
            {
                activatedCardIdList.Add(Card.Id + 1);
                SelectSTPlace(null, false);
                return true;
            }

            // sp summon
            if (Bot.HasInSpellZone(CardId.TransactionRollback) && GetEmptyMainMonsterZoneCount() > chainSummoningIdList.Count()
                    && !CheckWhetherWillbeRemoved() && !CheckShouldNoMoreSpSummon(false))
            {
                AI.SelectCard(CardId.TransactionRollback);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            // escape target
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard chainCard = Util.GetLastChainCard();
                if (chainCard != null && chainCard.IsCode(targetNegateIdList))
                {
                    if (Duel.LastChainTargets.Any(card => card.Controller == 0 && card.IsFaceup() && card.HasRace(CardRace.Fiend)
                        && Duel.CurrentChain.Any(chain => chain == card) && !card.IsCode(CardId.UnchainedSoulOfRage, CardId.UnchainedSoulOfAnguish)))
                    {
                        escapeTargetList.AddRange(Duel.LastChainTargets);
                        AI.SelectCard(Duel.LastChainTargets);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
            }

            // for link
            bool destroySpells = Duel.Player == 0 && GetEmptyMainMonsterZoneCount() > chainSummoningIdList.Count() && Bot.GetMonsterCount() > 0 && CurrentTiming <= 0;
            if (destroySpells)
            {
                List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => !card.HasRace(CardRace.Fiend));
                destroySpells = CheckAtAdvantage() && !Bot.HasInMonstersZone(CardId.UnchainedSoulOfRage)
                        && Bot.HasInExtra(CardId.UnchainedSoulOfRage) && materialList.Count() == 1;
                if (Bot.HasInExtra(CardId.UnchainedSoulOfAnguish) && !Bot.HasInMonstersZone(CardId.UnchainedSoulOfAnguish)
                    && !activatedCardIdList.Contains(CardId.UnchainedSoulOfAnguish) && Enemy.GetMonsters().Where(card => card.IsFaceup()).Count() > 0)
                {
                    destroySpells |= materialList.Count() == 2;
                    destroySpells |= materialList.Count() == 1 && materialList.Any(card => card.HasType(CardType.Link) && card.LinkCount == 2);
                }
            }
            // for attack
            destroySpells |= CheckCanDirectAttack() && GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack() + 2000 >= Enemy.LifePoints
                && GetEmptyMainMonsterZoneCount() > chainSummoningIdList.Count();
            // for avoid lose
            destroySpells |= Duel.Player == 1 && Duel.Phase == DuelPhase.Main1 && Bot.GetMonsterCount() == 0 && (CurrentTiming & hintTimingMainEnd) != 0
                && Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints;
            if (destroySpells)
            {
                List<int> destroyIdList = new List<int>{_CardId.InfiniteImpermanence, CardId.TransactionRollback, CardId.WelcomeLabrynth,
                    _CardId.DimensionalBarrier, CardId.DestructiveDarumaKarmaCannon, CardId.BigWelcomeLabrynth};
                foreach (int checkId in destroyIdList)
                {
                    ClientCard target = Bot.GetSpells().FirstOrDefault(card => card.IsFacedown() && card.IsCode(checkId));
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AriasTheLabrynthButlerActivate()
        {
            if (Card.Location != CardLocation.Grave)
            {
                if (Util.ChainContainsCard(new int[] { _CardId.DivineArsenalAAZEUS_SkyThunder, _CardId.EvenlyMatched, _CardId.EvilswarmExcitonKnight })) return false;
                if (Duel.CurrentChain.Any(card => card.Controller == 0 && card.IsCode(CardId.AriannaTheLabrynthServant))) return false;

                // set trap to activate
                SortedList<int, Func<bool>> checkList = new SortedList<int, Func<bool>> {
                    {CardId.BigWelcomeLabrynth, BigWelcomeLabrynthSetCheck},
                    {CardId.WelcomeLabrynth, WelcomeLabrynthSetCheck},
                    {CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonSetCheck},
                    {_CardId.DimensionalBarrier, DimensionalBarrierActivate}
                };
                foreach (KeyValuePair<int, Func<bool>> pair in checkList)
                {
                    ClientCard setTarget = Bot.Hand.FirstOrDefault(card => card.IsCode(pair.Key));
                    if (setTarget != null && !activatedCardIdList.Contains(pair.Key) && pair.Value())
                    {
                        AI.SelectOption(1);
                        AI.SelectCard(pair.Key);
                        activatedCardIdList.Add(Card.Id);
                        SelectSTPlace(setTarget, true);
                        return true;
                    }
                }
                
                // special summon monster
                if (Bot.HasInHand(CardId.LovelyLabrynthOfTheSilverCastle))
                {
                    // before main end
                    if (Duel.Player == 0 || (CurrentTiming & hintTimingMainEnd) != 0)
                    {
                        AI.SelectOption(0);
                        AI.SelectCard(CardId.LovelyLabrynthOfTheSilverCastle);
                        chainSummoningIdList.Add(CardId.LovelyLabrynthOfTheSilverCastle);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
                if (Bot.HasInHand(CardId.AriannaTheLabrynthServant) && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)
                    && !CheckWhetherNegated(true, true) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                {
                    bool searchFlag = false;
                    if (Duel.Player == 1)
                    {
                        searchFlag |= (CurrentTiming & hintTimingMainEnd) != 0;
                        searchFlag |= GetProblematicEnemyCardList(false).Count() > 0
                            && (Bot.HasInMonstersZoneOrInGraveyard(CardId.LovelyLabrynthOfTheSilverCastle) || CheckRemainInDeck(CardId.LovelyLabrynthOfTheSilverCastle) > 0)
                            && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1);
                    }
                    if (Duel.Player == 0) searchFlag |= summoned && !CheckShouldNoMoreSpSummon();
                    if (searchFlag)
                    {
                        AI.SelectOption(0);
                        AI.SelectCard(CardId.AriannaTheLabrynthServant);
                        chainSummoningIdList.Add(CardId.AriannaTheLabrynthServant);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ArianeTheLabrynthServantSummon()
        {
            // for attack
            if (Duel.Turn > 1 && Enemy.GetMonsterCount() == 0)
            {
                summoned = true;
                return true;
            }
            // for activate effect
            if (!activatedCardIdList.Contains(Card.Id) && !CheckWhetherNegated(true, true) && !CheckWhetherWillbeRemoved())
            {
                bool haveCost = Bot.Hand.Any(card => card.Type == (int)CardType.Trap) || Bot.GetSpells().Any(card => card.IsFacedown() && card.Type == (int)CardType.Trap);
                if (haveCost && !CheckShouldNoMoreSpSummon(true))
                {
                    summoned = true;
                    return true;
                }
            }

            return false;
        }
        public bool ArianeTheLabrynthServantForRollbackSummon()
        {
            if (activatedCardIdList.Contains(Card.Id)) return false;
            if (Bot.HasInHandOrInSpellZone(CardId.TransactionRollback) && !CheckWhetherWillbeRemoved())
            {
                summoned = true;
                return true;
            }

            return false;
        }
        public bool ArianeTheLabrynthServantActivate()
        {
            // special summon
            if (ActivateDescription == Util.GetStringId(Card.Id, 0))
            {
                bool haveRollback = Bot.HasInHandOrInSpellZone(CardId.TransactionRollback);
                if (CheckWhetherNegated() && !haveRollback) return false;
                if (CheckShouldNoMoreSpSummon() && !(haveRollback && Bot.Graveyard.Any(card => card.IsCode(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth)))) return false;
                int specialSummonId = 0;
                // arianna
                if (!activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && CheckRemainInDeck(CardId.AriannaTheLabrynthServant) > 0)
                {
                    specialSummonId = CardId.AriannaTheLabrynthServant;
                }
                // sp summon not used furniture
                if (specialSummonId == 0)
                {
                    List<int> checkIdList = new List<int>{CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock};
                    foreach (int checkId in checkIdList)
                    {
                        if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId) && CheckRemainInDeck(checkId) > 0)
                        {
                            specialSummonId = checkId;
                            break;
                        }
                    }
                }
                // for attack/link
                if (specialSummonId == 0)
                {
                    List<int> checkIdList = new List<int>();
                    if (Enemy.GetMonsterCount() == 0) checkIdList.AddRange(new List<int>{
                        CardId.AriannaTheLabrynthServant, CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock
                    });
                    else checkIdList.AddRange(new List<int>{
                        CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie, CardId.LabrynthCooclock, CardId.AriannaTheLabrynthServant
                    });
                    foreach (int checkId in checkIdList)
                    {
                        if (CheckRemainInDeck(checkId) > 0)
                        {
                            specialSummonId = checkId;
                            break;
                        }
                    }
                }

                if (specialSummonId > 0)
                {
                    bool costSelected = false;
                    if (haveRollback)
                    {
                        AI.SelectCard(CardId.TransactionRollback);
                        costSelected = true;
                    }
                    if (!costSelected) {
                        ClientCard welcome = Bot.GetSpells().FirstOrDefault(card => card.IsCode(CardId.WelcomeLabrynth));
                        if (welcome != null)
                        {
                            AI.SelectCard(welcome);
                            costSelected = true;
                        }
                    }
                    List<ClientCard> costCheckList = Bot.Hand.Where(card => card.IsFacedown() && card.Type == (int)CardType.Trap).ToList();
                    costCheckList.AddRange(Bot.GetSpells().Where(card => card.IsFacedown() && card.Type == (int)CardType.Trap).ToList());
                    if (!costSelected)
                    {
                        List<int> checkIdList = new List<int>{_CardId.InfiniteImpermanence, CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth};
                        foreach (int checkId in checkIdList)
                        {
                            ClientCard dumpCard = costCheckList.FirstOrDefault(card => card.IsCode(checkId));
                            if (costCheckList.Count(card => card.IsCode(checkId)) > 1 && dumpCard != null)
                            {
                                AI.SelectCard(dumpCard);
                                costSelected = true;
                                break;
                            }
                        }
                    }
                    if (!costSelected)
                    {
                        List<int> checkIdList = new List<int>{_CardId.InfiniteImpermanence, _CardId.DimensionalBarrier, CardId.DestructiveDarumaKarmaCannon,
                            CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth};
                        foreach (int checkId in checkIdList)
                        {
                            ClientCard checkCard = costCheckList.FirstOrDefault(card => card.IsCode(checkId));
                            if (checkCard != null)
                            {
                                AI.SelectCard(checkCard);
                                costSelected = true;
                                break;
                            }
                        }
                    }
                }
            } else {
                // draw
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool AriannaTheLabrynthServantSummon()
        {
            // summon for search
            if (!CheckWhetherNegated(true, true) && !activatedCardIdList.Contains(Card.Id))
            {
                summoned = true;
                return true;
            }

            // summon for attack
            if (Duel.Turn > 1 && Duel.Player == 0 && Duel.Phase < DuelPhase.Main2 && Enemy.GetMonsterCount() == 0 && !Bot.HasInHand(CardId.ArianeTheLabrynthServant))
            {
                summoned = true;
                return true;
            }

            return false;
        }
        public bool AriannaTheLabrynthServantActivate()
        {
            if (CheckWhetherNegated()) return false;
            // search or draw
            // search target is overrided in OnSelectCard()
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (Util.GetLastChainCard().IsCode(_CardId.MaxxC)) return false;
            if (DefaultAshBlossomAndJoyousSpring())
            {
                if (Util.GetLastChainCard().Location == CardLocation.MonsterZone) currentNegateMonsterList.Add(Util.GetLastChainCard());
                return true;
            }
            return false;
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }

        public bool FurnitureSetWelcomeActivate()
        {
            if (furnitureActivating && (Card.Location == CardLocation.Hand || !DefaultOnBecomeTarget())) return false;
            if (Util.ChainContainsCard(new int[]{ _CardId.DivineArsenalAAZEUS_SkyThunder, _CardId.EvenlyMatched, _CardId.EvilswarmExcitonKnight })) return false;

            if (CheckWhetherNegated()) return false;
            if (Card.Location != CardLocation.Grave)
            {
                bool becomeTarget = Card.Location == CardLocation.MonsterZone && DefaultOnBecomeTarget() && !escapeTargetList.Contains(Card);
                bool lackUnimportantCost = Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth));
                if (lackUnimportantCost)
                {
                    List<ClientCard> handCost = Bot.Hand.Where(card => card != Card).ToList();
                    lackUnimportantCost &= handCost.Count() <= 2 && handCost.All(card => card.IsCode(_CardId.MaxxC, _CardId.AshBlossom));
                }
                bool activateFlag = becomeTarget;
                // set big welcome for lovely
                bool canActivateSetBigWelcomeThisTurn = CheckRemainInDeck(CardId.BigWelcomeLabrynth) > 0 && cooclockAffected && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)
                    && (Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) || CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle))
                    && (!Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth) && !setTrapThisTurn.Contains(card)))
                    && (
                        Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth))
                        || (Bot.HasInGraveyard(CardId.LabrynthCooclock) && !activatedCardIdList.Contains(CardId.LabrynthCooclock + 1))
                        || (Bot.HasInHand(CardId.LadyLabrynthOfTheSilverCastle) && !activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle))
                    );
                if (canActivateSetBigWelcomeThisTurn && ShouldSetBigWelcome())
                {
                    bool force = becomeTarget | GetProblematicEnemyCardList(false).Count() > 0;
                    ClientCard cost = FurnitureGetCost(force);
                    if (cost != null)
                    {
                        AI.SelectCard(cost);
                        AI.SelectNextCard(CardId.BigWelcomeLabrynth);
                        activatedCardIdList.Add(Card.Id);
                        furnitureActivating = true;
                        SelectSTPlace(null, true);
                        return true;
                    }
                }
                bool keepOnField = (cooclockActivating || cooclockAffected) && activatedCardIdList.Contains(CardId.LabrynthCooclock + 1)
                    && Card.Location == CardLocation.MonsterZone && !Bot.GetMonsters().Any(card => card.IsFaceup() && card != Card && card.HasSetcode(SetcodeLabrynth))
                    && setTrapThisTurn.Count() > 0;
                // normal set
                activateFlag |= Duel.Phase > DuelPhase.Main2 && !lackUnimportantCost && !keepOnField;
                activateFlag |= Bot.HasInGraveyard(new List<int> { CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth })
                        && Bot.HasInHand(CardId.TransactionRollback) && !activatedCardIdList.Contains(CardId.TransactionRollback);
                if (Duel.CurrentChain.Any(card => card != null && card.Controller == 0 && card.IsCode(CardId.BigWelcomeLabrynth) && card.Location == CardLocation.SpellZone)
                    && !(Bot.GetMonsterCount() == 1 && Card.Location == CardLocation.MonsterZone))
                {
                    activateFlag |= !lackUnimportantCost && Bot.GetMonsters().Any(card => card != Card) && !Bot.HasInGraveyard(Card.Id) && !activatedCardIdList.Contains(Card.Id + 1);
                }
                // trigger cooclock to defense
                activateFlag |= !Util.ChainContainPlayer(0) && Duel.Player == 1 && Bot.UnderAttack && Bot.GetMonsterCount() == 0
                    && Bot.HasInGraveyard(CardId.LabrynthCooclock) && !activatedCardIdList.Contains(CardId.LabrynthCooclock + 1)
                    && !(Bot.HasInHand(CardId.LadyLabrynthOfTheSilverCastle) && !activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle));

                if (activateFlag)
                {
                    ClientCard cost = FurnitureGetCost(becomeTarget);
                    if (cost != null)
                    {
                        AI.SelectCard(cost);
                        activatedCardIdList.Add(Card.Id);
                        furnitureActivating = true;
                        bool setWelcome = Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth));
                        setWelcome |= Bot.GetMonsterCount() == 0 && !Bot.HasInHandOrInSpellZone(CardId.WelcomeLabrynth)
                            && (!Bot.HasInGraveyard(CardId.LabrynthCooclock) || activatedCardIdList.Contains(CardId.LabrynthCooclock + 1))
                            && ((Duel.Player == 0 && Duel.Phase > DuelPhase.Main2) || !Bot.Hand.Any(card => card != Card && card.Level <= 4));
                        if (setWelcome)
                        {
                            AI.SelectNextCard(CardId.WelcomeLabrynth);
                        } else {
                            AI.SelectNextCard(CardId.BigWelcomeLabrynth);
                        }
                        SelectSTPlace(null, true);
                        return true;
                    }
                }
            }

            return false;
        }

        public ClientCard FurnitureGetCost(bool force = false, List<ClientCard> ignoreList = null)
        {
            if (ignoreList == null) ignoreList = new List<ClientCard>();
            // advance cost
            List<int> advancedCostIdList = new List<int>{
                CardId.TransactionRollback, CardId.LovelyLabrynthOfTheSilverCastle, CardId.AriasTheLabrynthButler, CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie,
                CardId.WelcomeLabrynth
            };
            foreach (int checkId in advancedCostIdList)
            {
                ClientCard cost = Bot.Hand.FirstOrDefault(card => !ignoreList.Contains(card) && card.IsCode(checkId) && card != Card);
                if (cost != null) return cost;
            }
            // dump cost
            List<ClientCard> canCostHand = Bot.Hand.Where(card => !ignoreList.Contains(card)).ToList();
            List<int> appearedCode = new List<int>(canCostHand.Count());
            foreach (ClientCard hand in canCostHand)
            {
                if (Duel.CurrentChain.Contains(hand)) continue;
                if (appearedCode.Contains(hand.Id)) return hand;
                appearedCode.Add(hand.Id);
            }
            List<int> costIdList = new List<int>{
                _CardId.InfiniteImpermanence, _CardId.DimensionalBarrier, CardId.UnchainedSoulOfSharvara, CardId.EscapeOfTheUnchained, CardId.DestructiveDarumaKarmaCannon,
                CardId.LabrynthCooclock, CardId.ArianeTheLabrynthServant, CardId.WelcomeLabrynth, CardId.PotOfExtravagance, CardId.LadyLabrynthOfTheSilverCastle
            };
            if (force) costIdList.AddRange(new List<int>{CardId.AriannaTheLabrynthServant, _CardId.AshBlossom, CardId.BigWelcomeLabrynth, _CardId.MaxxC});
            foreach (int checkId in costIdList)
            {
                ClientCard target = canCostHand.FirstOrDefault(card => !Duel.CurrentChain.Contains(card) && card.IsCode(checkId) && !Duel.CurrentChain.Contains(card));
                if (target != null) return target;
            }
            foreach (int checkId in costIdList)
            {
                ClientCard target = canCostHand.FirstOrDefault(card => card.IsCode(checkId) && !Duel.CurrentChain.Contains(card));
                if (target != null) return target;
            }

            return null;
        }

        public bool ShouldSetBigWelcome(bool checkArianna = true)
        {
            if (CheckWhetherWillbeRemoved()) return false;
            bool shouldTriggerBigWelcomeFlag = GetProblematicEnemyCardList(false).Count() > 0;
            shouldTriggerBigWelcomeFlag |= Duel.Player == 1 && Duel.Phase > DuelPhase.Main2;
            shouldTriggerBigWelcomeFlag |= Duel.Player == 1 && GetProblematicEnemyCardList(false).Count() == 0 && GetProblematicEnemyMonster(selfType: CardType.Monster) == null
                && Enemy.Hand.Count() == 1;
            if (checkArianna) shouldTriggerBigWelcomeFlag |= Duel.Player == 0 && !summoned && Bot.HasInHandOrHasInMonstersZone(CardId.AriannaTheLabrynthServant)
                && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant);
            shouldTriggerBigWelcomeFlag |= Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2;
            return shouldTriggerBigWelcomeFlag;
        }

        public bool LabrynthCooclockActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                bool shouldTriggerBigWelcomeFlag = false;
                if (Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) || CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle)) {
                    shouldTriggerBigWelcomeFlag |= ShouldSetBigWelcome();
                }
                shouldTriggerBigWelcomeFlag &= !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth);
                if (shouldTriggerBigWelcomeFlag && !Bot.GetSpells().Any(card => card.IsFacedown() && !setTrapThisTurn.Contains(card) && card.IsCode(CardId.BigWelcomeLabrynth)))
                {
                    // whether have labrynth to trigger cooclock
                    bool haveBigWelcome = Duel.Player == 0 && Bot.HasInHand(CardId.BigWelcomeLabrynth)
                        && (Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth)) || (!summoned && Bot.Hand.Any(card => card != Card && card.HasType(CardType.Monster) && card.Level <= 4 && card.HasSetcode(SetcodeLabrynth))));
                    if (CheckRemainInDeck(CardId.BigWelcomeLabrynth) > 0)
                    {
                        foreach (int checkId in new List<int> { CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie })
                        {
                            if (activatedCardIdList.Contains(checkId) || CheckCalledbytheGrave(checkId) > 0) continue;
                            if (Bot.HasInHand(checkId) && Bot.Hand.Count > 2 || Bot.GetMonsters().Any(card => card.IsFaceup() && !card.IsDisabled() && card.IsCode(checkId)) && Bot.Hand.Count > 1)
                            {
                                haveBigWelcome = true;
                                break;
                            }
                        }
                    }
                    if (haveBigWelcome)
                    {
                        activatedCardIdList.Add(Card.Id);
                        cooclockActivating = true;
                        return true;
                    }
                }

                bool haveLabrynth = Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth));
                bool triggerFlag = Duel.Player == 1 && Duel.Phase <= DuelPhase.Main2 && setTrapThisTurn.Any(card => !activatedCardIdList.Contains(card.Id)) && haveLabrynth;
                triggerFlag |= Duel.Player == 1 && activatedCardIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle + 1)
                    && (Bot.GetSpells().Any(card => card.IsFacedown() && card.Type == (int)CardType.Trap) || Util.ChainContainsCard(CardId.LadyLabrynthOfTheSilverCastle));
                triggerFlag |= setTrapThisTurn.Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)) && haveLabrynth;
                triggerFlag |= setTrapThisTurn.Any(card => card.IsFacedown() && card.IsCode(CardId.WelcomeLabrynth) && !activatedCardIdList.Contains(CardId.WelcomeLabrynth)) && haveLabrynth;

                if (triggerFlag)
                {
                    activatedCardIdList.Add(Card.Id);
                    cooclockActivating = true;
                    return true;
                }
            }

            return false;
        }

        public bool RecycleActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Card.IsCode(CardId.LabrynthStovieTorbie, CardId.AriasTheLabrynthButler))
                {
                    if (CheckShouldNoMoreSpSummon() || GetEmptyMainMonsterZoneCount() + chainSummoningIdList.Count() <= 0) return false;
                    chainSummoningIdList.Add(Card.Id);
                }
                if (Card.IsCode(CardId.WelcomeLabrynth)) SelectSTPlace(Card, false);
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool ForLinkSummon()
        {
            if (Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained))) return false;
            if (Card.Level > 4) return false;
            if (CheckShouldNoMoreSpSummon()) return false;
            if (!Bot.HasInExtra(CardId.UnchainedSoulLordOfYama)) return false;

            // check whether need summon for material count
            List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => !card.HasRace(CardRace.Fiend));
            int materialCount = materialList.Count();
            if (!activatedCardIdList.Contains(CardId.UnchainedSoulOfSharvara) && Bot.GetSpells().Any(card => card.IsFacedown())
                && (!activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama) || Bot.HasInHand(CardId.UnchainedSoulOfSharvara))) materialCount++;
            if (materialCount != 2)
            {
                if (materialCount != 1 || !materialList.Any(card => card.IsCode(CardId.UnchainedSoulLordOfYama))) return false;
            }

            if (!Bot.HasInExtra(CardId.UnchainedSoulLordOfYama) && !materialList.Any(card => card.HasSetcode(SetcodeUnchained))) return false;
            bool needSummon = false;
            needSummon |= Enemy.GetMonsterCount() > 0 && Bot.HasInExtra(CardId.UnchainedSoulOfAnguish);
            needSummon |= Bot.HasInExtra(CardId.UnchainedSoulOfRage);
            if (needSummon)
            {
                // use monster with least attack
                YGOSharp.OCGWrapper.NamedCard thisCardData = YGOSharp.OCGWrapper.NamedCard.Get(Card.Id);
                if (thisCardData == null) return false;
                if (thisCardData.Race != (int)CardRace.Fiend) return false;
                foreach (ClientCard hand in Bot.Hand)
                {
                    YGOSharp.OCGWrapper.NamedCard compareCardData = YGOSharp.OCGWrapper.NamedCard.Get(hand.Id);
                    if (compareCardData == null) continue;
                    if (!compareCardData.HasType(CardType.Monster) || compareCardData.Level > 4) continue;
                    if (compareCardData.Attack < thisCardData.Attack) return false;
                }
                summoned = true;
                return true;
            }

            return false;
        }
        public bool ForSynchroSummon()
        {
            if (Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained))) return false;
            if (!Card.IsCode(new List<int> { CardId.LabrynthStovieTorbie, CardId.ArianeTheLabrynthServant, CardId.AriannaTheLabrynthServant })) return false;
            if (CheckShouldNoMoreSpSummon()) return false;
            if (!Bot.HasInExtra(CardId.ChaosAngel) || dimensionalBarrierAnnouced.Contains(HintMsg.SYNCHRO)) return false;

            bool checkFlag = GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0 && !CheckWhetherNegated(true, true, CardType.Monster);
            if (Card.IsCode(CardId.LabrynthStovieTorbie))
            {
                if (!Bot.GetMonsters().Any(card => card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link)
                    && card.Level == 8 && card.HasAttribute(CardAttribute.Light | CardAttribute.Dark))) return false;
                summoned = true;
                return true;
            }
            else
            {
                if (!Bot.GetMonsters().Any(card => card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link)
                    && card.Level == 6 && card.HasAttribute(CardAttribute.Light | CardAttribute.Dark))) return false;
                summoned = true;
                return true;
            }
        }
        public bool ForAnimaSummon()
        {
            if (banSpSummonExceptFiendCount > 0 || !Bot.HasInExtra(CardId.RelinquishedAnima)) return false;
            if (CheckWhetherNegated() || Duel.Turn == 1) return false;

            bool checkFlag = Bot.MonsterZone[1] == null && Enemy.MonsterZone[6] != null && Enemy.MonsterZone[6].HasType(CardType.Link) && Enemy.MonsterZone[6].HasLinkMarker(CardLinkMarker.Top);
            checkFlag |= Bot.MonsterZone[3] == null && Enemy.MonsterZone[5] != null && Enemy.MonsterZone[5].HasType(CardType.Link) && Enemy.MonsterZone[5].HasLinkMarker(CardLinkMarker.Top);
            if (Bot.GetMonstersExtraZoneCount() == 0) checkFlag |= Enemy.MonsterZone[1] != null || Enemy.MonsterZone[3] != null;

            return checkFlag;
        }

        public bool LabrynthForCooClockSummon()
        {
            if (!cooclockAffected) return false;
            if (Card.Level > 4 || !Card.HasSetcode(SetcodeLabrynth)) return false;
            bool welcomeFlag = Bot.Hand.Any(card => (card.IsCode(CardId.WelcomeLabrynth) && !activatedCardIdList.Contains(CardId.WelcomeLabrynth))
                || (card.IsCode(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)));
            welcomeFlag |= Bot.GetSpells().Any(card => card.IsFacedown() && setTrapThisTurn.Contains(card) && (
                (card.IsCode(CardId.WelcomeLabrynth) && !activatedCardIdList.Contains(CardId.WelcomeLabrynth))
                || (card.IsCode(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth))));
            if (welcomeFlag && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth)))
            {
                // summon highest attack
                int currentAttack = 0;
                YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(Card.Id);
                if (cardData != null) currentAttack = cardData.Attack;
                List<ClientCard> summonList = Bot.Hand.Where(card => card.IsMonster() && card.Level <= 4 && card.HasSetcode(SetcodeLabrynth)).ToList();
                foreach (ClientCard checkCard in summonList)
                {
                    cardData = YGOSharp.OCGWrapper.NamedCard.Get(checkCard.Id);
                    if (cardData != null && cardData.Attack < currentAttack) return false;
                }

                return true;
            }
            return false;
        }

        public bool ForBigWelcomeSummon()
        {
            if (Bot.HasInSpellZone(CardId.BigWelcomeLabrynth) && Bot.GetMonsterCount() == 0 && Card.Level <= 4)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool PotOfExtravaganceActivate()
        {
            if (CheckWhetherNegated()) return false;
            SelectSTPlace(Card, true);
            activatedCardIdList.Add(Card.Id);
            AI.SelectOption(1);
            return true;
        }

        public bool WelcomeLabrynthActivate()
        {
            return WelcomeLabrynthActivateCheck(false);
        }
        public bool WelcomeLabrynthActivateCopy()
        {
            return WelcomeLabrynthActivateCheck(true);
        }
        public bool WelcomeLabrynthSetCheck()
        {
            return !CheckShouldNoMoreSpSummon() && WelcomeLabrynthActivateCheck(true, true);
        }
        public bool WelcomeLabrynthActivateCheck(bool onlyCheck = false, bool noSelect = false)
        {
            if (Card.Location == CardLocation.SpellZone || onlyCheck)
            {
                if (GetEmptyMainMonsterZoneCount() == 0) return false;
                if (CheckShouldNoMoreSpSummon()) return false;
                bool activateTimingFlag = Duel.Phase > DuelPhase.Main2 || (Card.IsCode(CardId.AriasTheLabrynthButler) && (CurrentTiming & hintTimingMainEnd) > 0);

                bool becomeTarget = Card.Location == CardLocation.SpellZone && DefaultOnBecomeTarget();
                if ((Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2 || Duel.Player == 1 && activateTimingFlag)
                    && CheckRemainInDeck(CardId.ArianeTheLabrynthServant) > 0 && Bot.HasInHandOrInSpellZone(CardId.TransactionRollback)
                    && !chainSummoningIdList.Contains(CardId.ArianeTheLabrynthServant))
                {
                    if (!noSelect)
                    {
                        chainSummoningIdList.Add(CardId.ArianeTheLabrynthServant);
                        activatedCardIdList.Add(Card.Id);
                    }
                    return true;
                }
                bool ariannaCheck = !Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth)
                    || !(Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) || CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle));
                ariannaCheck |= Duel.Player == 1 && activateTimingFlag;
                ariannaCheck |= Duel.Player == 0;
                if (ariannaCheck)
                {
                    if (CheckRemainInDeck(CardId.AriannaTheLabrynthServant) > 0 && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)
                        && !CheckWhetherNegated(true, true, CardType.Monster) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                    {
                        if (!noSelect)
                        {
                            chainSummoningIdList.Add(CardId.AriannaTheLabrynthServant);
                            activatedCardIdList.Add(Card.Id);
                        }
                        return true;
                    }
                }
                if (Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth))
                {
                    if (Bot.HasInHand(CardId.LovelyLabrynthOfTheSilverCastle) && CheckRemainInDeck(CardId.AriasTheLabrynthButler) > 0
                         && !chainSummoningIdList.Contains(CardId.AriasTheLabrynthButler) && !Bot.HasInMonstersZone(CardId.AriasTheLabrynthButler, true, false, true))
                    {
                        if (!noSelect)
                        {
                            chainSummoningIdList.Add(CardId.AriasTheLabrynthButler);
                            activatedCardIdList.Add(Card.Id);
                        }
                        return true;
                    }
                }
                
                bool activateFlag = becomeTarget;
                activateFlag |= Bot.UnderAttack && Bot.GetMonsterCount() == 0;
                activateFlag |= ShouldSetBigWelcome(false);
                if (activateFlag)
                {
                    if (!noSelect)
                    {
                        if (Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)
                            && CheckRemainInDeck(CardId.LovelyLabrynthOfTheSilverCastle) > 0 && !chainSummoningIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle))
                        {
                            chainSummoningIdList.Add(CardId.LovelyLabrynthOfTheSilverCastle);
                        }
                        else if (!activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && CheckRemainInDeck(CardId.AriannaTheLabrynthServant) > 0
                            && !CheckWhetherNegated(true, true, CardType.Monster) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                        {
                            chainSummoningIdList.Add(CardId.AriannaTheLabrynthServant);
                        }
                        else if (Bot.HasInGraveyard(CardId.BigWelcomeLabrynth) && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth)
                            && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasRace(CardRace.Fiend) && card.Level >= 8)
                            && CheckRemainInDeck(CardId.LadyLabrynthOfTheSilverCastle) > 0 && !chainSummoningIdList.Contains(CardId.LadyLabrynthOfTheSilverCastle))
                        {
                            chainSummoningIdList.Add(CardId.LadyLabrynthOfTheSilverCastle);
                        }
                        else {
                            int selectId = 0;
                            List<int> checkIdList = new List<int>{CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock};
                            foreach (int checkId in checkIdList)
                            {
                                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(checkId) && CheckRemainInDeck(checkId) > 0
                                     && !chainSummoningIdList.Contains(checkId))
                                {
                                    selectId = checkId;
                                    break;
                                }
                            }
                            List<int> fullCheckIdList = new List<int>{
                                CardId.LadyLabrynthOfTheSilverCastle, CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock,
                                CardId.AriasTheLabrynthButler, CardId.ArianeTheLabrynthServant, CardId.AriannaTheLabrynthServant
                            };
                            if (selectId == 0)
                            {
                                foreach (int checkId in fullCheckIdList)
                                {
                                    if (CheckRemainInDeck(checkId) > 0 && !chainSummoningIdList.Contains(checkId))
                                    {
                                        selectId = checkId;
                                        break;
                                    }
                                }
                            }
                            if (selectId > 0) {
                                chainSummoningIdList.Add(selectId);
                            }
                        }
                        ResetCooclockEffect(onlyCheck);

                        activatedCardIdList.Add(Card.Id);
                    }
                    return true;
                }

            }

            return false;
        }

        public bool TransactionRollbackActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                SortedList<int, Func<bool>> checkList = new SortedList<int, Func<bool>> {
                    {CardId.BigWelcomeLabrynth, BigWelcomeLabrynthActivateCopy},
                    {_CardId.DimensionalBarrier, DimensionalBarrierActivate},
                    {CardId.EscapeOfTheUnchained, EscapeOfTheUnchainedActivateCopy},
                    {_CardId.InfiniteImpermanence, InfiniteImpermanenceActivateCopy},
                    {CardId.WelcomeLabrynth, WelcomeLabrynthActivateCopy},
                    {CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonActivate}
                };
                foreach (KeyValuePair<int, Func<bool>> pair in checkList)
                {
                    if (Bot.HasInGraveyard(pair.Key) && pair.Value())
                    {
                        rollbackCopyCardId = pair.Key;
                        AI.SelectCard(pair.Key);
                        return true;
                    }
                }
            }
            if (Card.Location == CardLocation.SpellZone)
            {
                if (CheckWhetherNegated()) return false;
                SortedList<int, Func<bool>> checkList = new SortedList<int, Func<bool>> {
                    {CardId.WelcomeLabrynth, WelcomeLabrynthActivateCopy},
                    {_CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice },
                    {CardId.DestructiveDarumaKarmaCannon, DestructiveDarumaKarmaCannonActivate},
                    {_CardId.DimensionalBarrier, DimensionalBarrierActivate},
                    {CardId.EscapeOfTheUnchained, EscapeOfTheUnchainedActivateCopy},
                    {_CardId.InfiniteImpermanence, InfiniteImpermanenceActivateCopy},
                    {_CardId.BreakthroughSkill, DefaultBreakthroughSkill},
                    {CardId.BigWelcomeLabrynth, BigWelcomeLabrynthActivateCopy}
                };
                foreach (KeyValuePair<int, Func<bool>> pair in checkList)
                {
                    if (Enemy.HasInGraveyard(pair.Key) && pair.Value())
                    {
                        rollbackCopyCardId = pair.Key;
                        AI.SelectCard(pair.Key);
                        ResetCooclockEffect(false);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool InfiniteImpermanenceActivate()
        {
            return InfiniteImpermanenceActivateCheck(false);
        }
        public bool InfiniteImpermanenceActivateCopy()
        {
            return InfiniteImpermanenceActivateCheck(true);
        }
        public bool InfiniteImpermanenceSetCheck()
        {
            return InfiniteImpermanenceActivateCheck(true, true);
        }
        public bool InfiniteImpermanenceActivateCheck(bool onlyCheck = false, bool noSelect = false)
        {
            if (CheckWhetherNegated()) return false;

            ClientCard LastChainCard = Util.GetLastChainCard();

            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int thisSeq = -1;
                int thatSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) thisSeq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) thatSeq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) thatSeq = i;
                }
                if ( (thisSeq * thatSeq >= 0 && thisSeq + thatSeq == 4)
                    || Util.IsChainTarget(Card)
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true, selfType: CardType.Trap);
                    if (!noSelect)
                    {
                        if (target != null)
                        {
                            AI.SelectCard(target);
                        } else {
                            AI.SelectCard(Enemy.GetMonsters());
                        }
                    }
                    if (!onlyCheck)
                    {
                        infiniteImpermanenceList.Add(thatSeq);
                        if (cooclockAffected && setTrapThisTurn.Contains(Card))
                        {
                            cooclockAffected = false;
                            setTrapThisTurn.Remove(Card);
                        }
                    }
                    return true;
                }
            }
            
            // negate monster
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true, CardType.Trap);
            if (shouldNegateList.Count() > 0)
            {
                ClientCard negateTarget = shouldNegateList[0];
                currentNegateMonsterList.Add(negateTarget);

                if (Card.Location == CardLocation.SpellZone && !onlyCheck)
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
                if (!noSelect) AI.SelectCard(negateTarget);
                currentDestroyCardList.Add(negateTarget);
                ResetCooclockEffect(onlyCheck);
                return true;
            }

            return false;
        }

        public bool DestructiveDarumaKarmaCannonActivate()
        {
            return DestructiveDarumaKarmaCannonActivateCheck(false);
        }
        public bool DestructiveDarumaKarmaCannonSetCheck()
        {
            return DestructiveDarumaKarmaCannonActivateCheck(true);
        }
        public bool DestructiveDarumaKarmaCannonActivateCheck(bool noSelect = false)
        {
            bool becomeTarget = Card.Location == CardLocation.SpellZone && DefaultOnBecomeTarget();
            bool activateFlag = becomeTarget && Util.IsOneEnemyBetter(true);
            bool canTriggerLovely =
                (!activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && Bot.GetSpells().Any(card => card.IsFacedown() && card.IsCode(CardId.BigWelcomeLabrynth) && (!cooclockAffected || !setTrapThisTurn.Contains(card)))
                    || Util.ChainContainsCard(CardId.BigWelcomeLabrynth))
                && (Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) || (CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle) && Bot.GetMonsterCount() > 0))
                && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1);

            activateFlag |= Bot.UnderAttack && (Bot.BattlingMonster?.GetDefensePower() ?? 0) <= (Enemy.BattlingMonster?.GetDefensePower() ?? 0) && !Util.ChainContainPlayer(0) && !canTriggerLovely;
            activateFlag |= Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() > 0;
            activateFlag |= Enemy.HasInMonstersZone(CardId.AccesscodeTalker, true) && !Util.ChainContainPlayer(0);
            int linkCount = 0;
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (monster.IsFacedown()) continue;
                if (!monster.HasType(CardType.Link)) linkCount++;
                else linkCount += monster.LinkCount;
            }
            activateFlag |= linkCount >= 6 && Util.IsOneEnemyBetter(true);
            if (activateFlag)
            {
                if (!noSelect)
                {
                    currentDestroyCardList.AddRange(Enemy.GetMonsters());
                    escapeTargetList.AddRange(Bot.GetMonsters());
                }
                return true;
            }

            return false;
        }

        public bool EscapeOfTheUnchainedActivate()
        {
            return EscapeOfTheUnchainedActivateCheck(false);
        }
        public bool EscapeOfTheUnchainedActivateCopy()
        {
            return EscapeOfTheUnchainedActivateCheck(true);
        }
        public bool EscapeOfTheUnchainedActivateCheck(bool onlyCheck = false, bool noSelect = false)
        {
            if (Card.Location == CardLocation.SpellZone || onlyCheck)
            {
                // select targeted unchained
                ClientCard selfTarget = Bot.GetMonsters().FirstOrDefault(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained)
                    && Duel.ChainTargets.Contains(card) && !escapeTargetList.Contains(card));
                if (selfTarget == null)
                {
                    selfTarget = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained))
                        .OrderBy(card => card.Attack).FirstOrDefault();
                }
                if (selfTarget == null) return false;
                // destroy danger card
                List<ClientCard> dangerList = GetProblematicEnemyCardList(true, selfType: CardType.Trap);
                if (dangerList.Count() > 0 && Duel.LastChainPlayer != 0)
                {
                    if (!noSelect)
                    {
                        AI.SelectCard(selfTarget);
                        AI.SelectNextCard(dangerList);
                        escapeTargetList.Add(selfTarget);
                        currentDestroyCardList.Add(dangerList[0]);
                        activatedCardIdList.Add(Card.Id);
                    }
                    return true;
                }

                // best monster
                int botBestPower = Util.GetBestPower(Bot);
                if (Duel.Player == 1 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    List<ClientCard> dangerMonsters = Enemy.GetMonsters().Where(card => card.IsFaceup() && card.Attack >= botBestPower
                        && !currentDestroyCardList.Contains(card) && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                        .OrderByDescending(card => card.Attack).ToList();
                    if (dangerMonsters.Count() > 0)
                    {
                        if (!noSelect)
                        {
                            AI.SelectCard(selfTarget);
                            AI.SelectNextCard(dangerMonsters);
                            escapeTargetList.Add(selfTarget);
                            currentDestroyCardList.Add(dangerMonsters[0]);
                            activatedCardIdList.Add(Card.Id);
                        }
                        return true;
                    }
                }

                // end phase
                bool activateFlag = Duel.Player == 1 && Duel.Phase > DuelPhase.Main2
                    && ((Bot.HasInGraveyard(CardId.UnchainedSoulLordOfYama) && !activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama + 1))
                    || (Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1)));
                activateFlag |= DefaultOnBecomeTarget() && Card.Location == CardLocation.SpellZone && !Util.ChainContainsCard(_CardId.EvenlyMatched);
                if (activateFlag)
                {
                    List<ClientCard> destroyTarget = GetNormalEnemyTargetList(true, true, CardType.Trap);
                    if (destroyTarget.Count() > 0)
                    {
                        if (!noSelect)
                        {
                            AI.SelectCard(selfTarget);
                            AI.SelectNextCard(destroyTarget);
                            escapeTargetList.Add(selfTarget);
                            currentDestroyCardList.Add(destroyTarget[0]);
                            activatedCardIdList.Add(Card.Id);
                        }
                        return true;
                    }
                }

            } else {
                if (!noSelect)
                {
                    AI.SelectCard(CardId.UnchainedSoulOfSharvara);
                    activatedCardIdList.Add(Card.Id + 1);
                }
                return true;
            }

            return false;
        }

        public bool DimensionalBarrierActivate()
        {
            if (Duel.Player == 0 && Duel.Turn == 1) return false;
            if (CheckWhetherNegated()) return false;
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {HintMsg.RITUAL, DimensionalBarrierForRitual},
                {HintMsg.FUSION, DimensionalBarrierForFusion},
                {HintMsg.SYNCHRO, DimensionalBarrierForSynchro},
                {HintMsg.XYZ, DimensionalBarrierForXyz},
                {HintMsg.PENDULUM, DimensionalBarrierForPendulum},
            };
            foreach (KeyValuePair<int, Func<bool>> checkType in checkDict)
            {
                if (dimensionalBarrierAnnouced.Contains(checkType.Key)) continue;
                if (checkType.Value()) {
                    ResetCooclockEffect(false);
                    return true;
                }
            }

            return DefaultOnBecomeTarget();
        }
        public bool DimensionalBarrierForRitual()
        {
            foreach (ClientCard chainCard in Duel.CurrentChain)
            {
                if (chainCard != null && chainCard.Controller == 1 && !chainCard.IsDisabled() && chainCard.HasType(CardType.Ritual)
                    && (chainCard.HasType(CardType.Spell) || (chainCard.Location == CardLocation.MonsterZone && !currentNegateMonsterList.Contains(chainCard))))
                {
                    if (dimensionBarrierAnnouncing) currentNegateMonsterList.Add(chainCard);
                    return true;
                }
            }

            return false;
        }
        public bool DimensionalBarrierForFusion()
        {
            foreach (ClientCard chainCard in Duel.CurrentChain)
            {
                if (chainCard != null && chainCard.Controller == 1 && !chainCard.IsDisabled() && (chainCard.IsFusionSpell()
                    || (chainCard.HasType(CardType.Fusion) && chainCard.Location == CardLocation.MonsterZone && !currentNegateMonsterList.Contains(chainCard))))
                {
                    if (dimensionBarrierAnnouncing) currentNegateMonsterList.Add(chainCard);
                    return true;
                }
            }

            return false;
        }
        public bool DimensionalBarrierForSynchro()
        {
            foreach (ClientCard chainCard in Duel.CurrentChain)
            {
                if (chainCard != null && chainCard.Controller == 1 && !chainCard.IsDisabled()
                    && chainCard.HasType(CardType.Synchro) && chainCard.Location == CardLocation.MonsterZone && !currentNegateMonsterList.Contains(chainCard))
                {
                    if (dimensionBarrierAnnouncing) currentNegateMonsterList.Add(chainCard);
                    return true;
                }
            }
            if (Duel.Player == 1 && !Util.ChainContainsCard(CardId.DestructiveDarumaKarmaCannon) && Enemy.ExtraDeck.Count() > 0)
            {
                bool tunerCheck = false;
                bool nontunerCheck = false;
                foreach (ClientCard monster in Enemy.GetMonsters())
                {
                    if (monster.IsFacedown() || monster.HasType(CardType.Xyz | CardType.Link)) continue;
                    if (monster.HasType(CardType.Tuner)) tunerCheck = true;
                    else nontunerCheck = true;
                }
                if (tunerCheck && nontunerCheck) return true;
            }

            return false;
        }
        public bool DimensionalBarrierForXyz()
        {
            foreach (ClientCard chainCard in Duel.CurrentChain)
            {
                if (chainCard != null && chainCard.Controller == 1 && !chainCard.IsDisabled()
                    && chainCard.HasType(CardType.Xyz) && chainCard.Location == CardLocation.MonsterZone && !currentNegateMonsterList.Contains(chainCard))
                {
                    if (dimensionBarrierAnnouncing) currentNegateMonsterList.Add(chainCard);
                    return true;
                }
            }
            if (Duel.Player == 1 && !Util.ChainContainsCard(CardId.DestructiveDarumaKarmaCannon) && Enemy.ExtraDeck.Count() > 0)
            {
                List<int> existsLevel = new List<int>(6);
                foreach (ClientCard monster in Enemy.GetMonsters())
                {
                    if (monster.IsFacedown()) continue;
                    if (monster.IsOneForXyz()) return true;
                    if (monster.HasType(CardType.Xyz | CardType.Token)) continue;
                    int level = monster.Level;
                    if (level != 2 && monster.HasType(CardType.Link)) continue;
                    if (existsLevel.Contains(level)) return true;
                    existsLevel.Add(level);
                }
            }

            return false;
        }
        public bool DimensionalBarrierForPendulum()
        {
            foreach (ClientCard chainCard in Duel.CurrentChain)
            {
                if (chainCard != null && chainCard.Controller == 1 && !chainCard.IsDisabled()
                    && chainCard.HasType(CardType.Pendulum) && chainCard.Location == CardLocation.MonsterZone && !currentNegateMonsterList.Contains(chainCard))
                {
                    if (dimensionBarrierAnnouncing) currentNegateMonsterList.Add(chainCard);
                    return true;
                }
            }

            ClientCard l = Enemy.SpellZone[6];
            ClientCard r = Enemy.SpellZone[7];
            if (l != null && r != null && l.LScale != r.RScale) return true;

            return false;
        }

        public bool BigWelcomeLabrynthActivate()
        {
            return BigWelcomeLabrynthActivateCheck(false);
        }
        public bool BigWelcomeLabrynthBecomeTargetActivate()
        {
            if (DefaultOnBecomeTarget()) return BigWelcomeLabrynthActivateCheck(false);
            return false;
        }
        public bool BigWelcomeLabrynthActivateCopy()
        {
            return BigWelcomeLabrynthActivateCheck(true);
        }
        public bool BigWelcomeLabrynthSetCheck()
        {
            return !CheckShouldNoMoreSpSummon() && BigWelcomeLabrynthActivateCheck(true, true);
        }
        public bool BigWelcomeLabrynthActivateCheck(bool onlyCheck = false, bool noSelect = false)
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location != CardLocation.SpellZone && !onlyCheck) return false;
            if (GetEmptyMainMonsterZoneCount() == 0) return false;
            bool activateTimingFlag = Duel.Phase > DuelPhase.Main2 || (Card.IsCode(CardId.AriasTheLabrynthButler) && (CurrentTiming & hintTimingMainEnd) > 0);

            bool needDestroyFlag = GetProblematicEnemyCardList(false).Count() > 0;
            needDestroyFlag |= activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && activateTimingFlag;
            needDestroyFlag |= Bot.UnderAttack && (Bot.BattlingMonster?.GetDefensePower() ?? 0) <= (Enemy.BattlingMonster?.GetDefensePower() ?? 0) && Duel.LastChainPlayer != 0;
            needDestroyFlag |= Duel.Turn == 1 && Duel.Player == 0 && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1);
            needDestroyFlag |= Duel.Turn == 1 && Enemy.GetMonsterCount() == 0 && Enemy.GetSpellCount() == 0 && Enemy.Hand.Count > 0
                && (CurrentTiming & hintTimingMainEnd) > 0;

            // do not activate when welcome is activating
            bool haveEnemyChain = false;
            bool haveWelcome = false;
            foreach (ClientCard chain in Duel.CurrentChain)
            {
                if (chain != null)
                {
                    if (chain.Controller == 1)
                    {
                        haveEnemyChain = true;
                        break;
                    }
                    if (chain.IsCode(CardId.WelcomeLabrynth, CardId.TransactionRollback, CardId.LadyLabrynthOfTheSilverCastle)) haveWelcome = true;
                }
            }
            if (haveWelcome && !haveEnemyChain) return false;

            // escape target
            List<ClientCard> targetList = Bot.GetMonsters();
            foreach (ClientCard target in targetList)
            {
                if (Duel.ChainTargets.Contains(target) && !escapeTargetList.Contains(target)
                        && !(target.IsCode(CardId.UnchainedSoulOfRage, CardId.UnchainedSoulOfAnguish) && Duel.CurrentChain.Contains(target)))
                {
                    Logger.DebugWriteLine("[BigWelcome]escape target");
                    if (!noSelect)
                    {
                        bigwelcomeEscaseTarget = target;
                        escapeTargetList.Add(target);
                        activatedCardIdList.Add(Card.Id);
                    }
                    return true;
                }
            }

            if (Bot.GetMonsterCount() > 0)
            {
                bool flag1 = needDestroyFlag && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1) && (Util.ChainContainPlayer(1) || Duel.LastChainPlayer != 0);
                bool flag2 = DefaultOnBecomeTarget();
                bool flag3 = Duel.Player == 1 && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && activateTimingFlag;
                bool flag4 = Duel.Player == 0 && Duel.LastChainPlayer != 0 && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth);
                Logger.DebugWriteLine("[BigWelcome count>0]flag: "+ flag1 + " " + flag2 + " " + flag3 + " " + flag4);
                needDestroyFlag |= flag3;
                if (flag1 || flag2 || flag3 || flag4)
                {
                    bool spSummonLovely = CheckBigWelcomeCanSpSummon(CardId.LovelyLabrynthOfTheSilverCastle) && !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1);
                    bool haveLovely = Bot.GetMonsters().Any(card => card.IsFaceup() && card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle));
                    if (!noSelect)
                    {
                        activatedCardIdList.Add(Card.Id);
                    }
                    ResetCooclockEffect(onlyCheck);
                    return true;
                }
            }
            else {
                bool activateFlag = DefaultOnBecomeTarget();
                activateFlag |= Duel.Player == 1 && !activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && activateTimingFlag;
                activateFlag |= Duel.Player == 0 && !summoned && !Bot.HasInHand(CardId.AriannaTheLabrynthServant) && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)
                    && !(Duel.Phase < DuelPhase.Main1 && Bot.HasInHand(CardId.PotOfExtravagance) && Bot.ExtraDeck.Count() >= 3)
                    && !(Duel.CurrentChain.Any(card => card.IsCode(CardId.PotOfExtravagance) && card.Controller == 0));
                if (activateFlag && !noSelect)
                {
                    activatedCardIdList.Add(Card.Id);
                    ResetCooclockEffect(onlyCheck);
                    return true;
                }
            }

            return false;
        }

        public bool BigWelcomeLabrynthGraveActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                // bounce enemy
                if (Bot.GetMonsters().Any(card => card.Level >= 8 && card.IsFaceup() && card.HasRace(CardRace.Fiend) && !card.HasType(CardType.Xyz | CardType.Link)))
                {
                    // danger monster
                    ClientCard problemCard = GetProblematicEnemyMonster(-1, true, true, CardType.Trap);
                    if (problemCard != null)
                    {
                        AI.SelectCard(problemCard);
                        currentDestroyCardList.Add(problemCard);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                    // problem spell
                    if (!Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true) ||
                            activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle) && activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle + 1))
                    {
                        List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && c.IsFaceup()
                        && c.IsFloodgate()
                        && !c.IsShouldNotBeTarget() && (c.HasType(CardType.Trap) || Duel.Player == 0)).ToList();

                        problemEnemySpellList.AddRange(Enemy.SpellZone.Where(c => c?.Data != null && c.IsFaceup() && !problemEnemySpellList.Contains(c)
                        && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous)
                        && !c.IsShouldNotBeTarget() && (c.HasType(CardType.Trap) || Duel.Player == 0)).ToList());

                        if (problemEnemySpellList.Count() > 0)
                        {
                            AI.SelectCard(problemEnemySpellList);
                            currentDestroyCardList.Add(problemEnemySpellList[0]);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                    // best monster
                    int botBestPower = Util.GetBestPower(Bot);
                    if (Duel.Player == 1 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                    {
                        List<ClientCard> dangerMonsters = Enemy.GetMonsters().Where(card => card.IsFaceup() && card.Attack >= botBestPower
                            && !currentDestroyCardList.Contains(card) && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                            .OrderByDescending(card => card.Attack).ToList();
                        if (dangerMonsters.Count() > 0)
                        {
                            AI.SelectCard(dangerMonsters);
                            currentDestroyCardList.Add(dangerMonsters[0]);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                    // end phase
                    if (Duel.Phase > DuelPhase.Main2)
                    {
                        List<ClientCard> returnList = GetNormalEnemyTargetList(true, true, CardType.Trap);
                        if (returnList.Count() > 0)
                        {
                            AI.SelectCard(returnList);
                            currentDestroyCardList.Add(returnList[0]);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }

                // escape target
                List<ClientCard> targetList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasRace(CardRace.Fiend)).ToList();
                foreach (ClientCard target in targetList)
                {
                    if (Duel.ChainTargets.Contains(target) && !escapeTargetList.Contains(target)
                        && !(target.IsCode(CardId.UnchainedSoulOfRage, CardId.UnchainedSoulOfAnguish) && Duel.CurrentChain.Contains(target)))
                    {
                        AI.SelectCard(target);
                        escapeTargetList.Add(target);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                // bounce arianna
                if (Duel.Player == 0 && Duel.Phase <= DuelPhase.Main2 && !summoned && !Bot.HasInHand(CardId.AriannaTheLabrynthServant)
                    && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant) && !chainSummoningIdList.Contains(CardId.AriannaTheLabrynthServant))
                {
                    ClientCard target = targetList.FirstOrDefault(card => card.IsCode(CardId.AriannaTheLabrynthServant));
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        escapeTargetList.Add(target);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                // trigger furniture/welcome
                List<ClientCard> checkFurnitureList = new List<ClientCard>(Bot.Hand);
                checkFurnitureList.AddRange(Bot.GetMonsters());
                if ((CheckRemainInDeck(CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth) == 0
                    || !checkFurnitureList.Any(card => card.IsCode(CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie)))
                    && Duel.LastChainPlayer < 0 && Duel.Player == 0 && !Bot.HasInMonstersZone(CardId.LovelyLabrynthOfTheSilverCastle, true, false, true)
                    && !(cooclockAffected && Bot.HasInHandOrInSpellZone(CardId.BigWelcomeLabrynth)))
                {
                    int checkCount = 0;
                    List<int> checkIdList = new List<int> { CardId.LabrynthChandraglier, CardId.LabrynthStovieTorbie, CardId.WelcomeLabrynth };
                    foreach (int checkId in checkIdList)
                    {
                        if (Bot.HasInGraveyard(checkId) && !activatedCardIdList.Contains(checkId + 1)) checkCount++;
                    }
                    if (checkCount > 0)
                    {
                        ClientCard target = targetList.FirstOrDefault(card => card.IsFaceup() && card.HasRace(CardRace.Fiend) &&
                            ((card.Level <= 4 && !card.HasType(CardType.Link | CardType.Xyz | CardType.Synchro)) || card.IsCode(CardId.LadyLabrynthOfTheSilverCastle)));
                        if (target != null)
                        {
                            AI.SelectCard(target);
                            escapeTargetList.Add(target);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ChaosAngelSpSummonWith2Monster()
        {
            if (CheckShouldNoMoreSpSummon(false)) return false;

            List<ClientCard> level2MonsterList = new List<ClientCard>();
            List<ClientCard> level4MonsterList = new List<ClientCard>();
            List<ClientCard> level6MonsterList = new List<ClientCard>();
            List<ClientCard> level8MonsterList = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.IsFaceup() && !monster.HasType(CardType.Xyz | CardType.Link) && monster.HasAttribute(CardAttribute.Light | CardAttribute.Dark))
                {
                    if (monster.Level == 2) level2MonsterList.Add(monster);
                    if (monster.Level == 4) level4MonsterList.Add(monster);
                    if (monster.Level == 6) level6MonsterList.Add(monster);
                    if (monster.Level == 8) level8MonsterList.Add(monster);
                }
            }
            level2MonsterList.Sort(CompareUsableAttack);
            level4MonsterList.Sort(CompareUsableAttack);
            level6MonsterList.Sort(CompareUsableAttack);
            level8MonsterList.Sort(CompareUsableAttack);
            bool checkFlag = GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0 && !CheckWhetherNegated(true, true, CardType.Monster);
            ClientCard BestEnemyMonster = Util.GetBestEnemyMonster();
            if (BestEnemyMonster != null && Util.GetBestPower(Bot, true) <= Util.GetBestPower(Enemy))
            {
                checkFlag |= Util.GetBestPower(Enemy) <= 3500;
                checkFlag |= !BestEnemyMonster.IsShouldNotBeTarget() && !BestEnemyMonster.IsShouldNotBeMonsterTarget();
            }
            // 4+6
            if (level4MonsterList.Count() > 0 && level6MonsterList.Count() > 0)
            {
                List<ClientCard> materials = new List<ClientCard>{level4MonsterList[0], level6MonsterList[0]};
                bool summonFlag = checkFlag;
                if (Enemy.GetMonsterCount() == 0 && Duel.Phase < DuelPhase.Main2)
                    summonFlag |= GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(materials) + 3500 >= Enemy.LifePoints;
                if (summonFlag)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            // 2+8
            if (level2MonsterList.Count() > 0 && level8MonsterList.Count() > 0)
            {
                foreach (ClientCard level2 in level2MonsterList)
                {
                    foreach (ClientCard level8 in level8MonsterList)
                    {
                        List<ClientCard> materials = new List<ClientCard>{level2, level8};
                        if (checkFlag && !(level8.IsCode(CardId.LovelyLabrynthOfTheSilverCastle) && !level8.IsDisabled() && Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth)))
                        {
                            AI.SelectMaterials(materials);
                            return true;
                        }
                        if (Enemy.GetMonsterCount() == 0 && GetMaterialAttack(materials) < 3500 && Duel.Phase < DuelPhase.Main2)
                        {
                            if (GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(materials) + 3500 >= Enemy.LifePoints)
                            {
                                AI.SelectMaterials(materials);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool ChaosAngelSpSummonWith3Monster()
        {
            if (CheckShouldNoMoreSpSummon(false)) return false;

            List<ClientCard> level2MonsterList = new List<ClientCard>();
            List<ClientCard> level4MonsterList = new List<ClientCard>();
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (monster.IsFaceup() && !monster.HasType(CardType.Xyz | CardType.Link) && monster.HasAttribute(CardAttribute.Light | CardAttribute.Dark))
                {
                    if (monster.Level == 2) level2MonsterList.Add(monster);
                    if (monster.Level == 4) level4MonsterList.Add(monster);
                }
            }
            level2MonsterList.Sort(CompareUsableAttack);
            level4MonsterList.Sort(CompareUsableAttack);
            bool checkFlag = GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0 && !CheckWhetherNegated(true, true, CardType.Monster);
            ClientCard BestEnemyMonster = Util.GetBestEnemyMonster();
            if (BestEnemyMonster != null && Util.GetBestPower(Bot, true) <= Util.GetBestPower(Enemy))
            {
                checkFlag |= Util.GetBestPower(Enemy) <= 3500;
                checkFlag |= !BestEnemyMonster.IsShouldNotBeTarget() && !BestEnemyMonster.IsShouldNotBeMonsterTarget();
            }
            // 2+4+4
            if (level2MonsterList.Count() >= 1 && level4MonsterList.Count() >= 2)
            {
                foreach (ClientCard level2 in level2MonsterList)
                {
                    for (int level4Index1 = 0; level4Index1 < level4MonsterList.Count() - 1; ++level4Index1)
                    {
                        ClientCard level41 = level4MonsterList[level4Index1];
                        for (int level4Index2 = level4Index1 + 1; level4Index2 < level4MonsterList.Count(); ++level4Index2)
                        {
                            ClientCard level42 = level4MonsterList[level4Index2];
                            List<ClientCard> materials = new List<ClientCard> { level2, level41, level42 };
                            bool summonFlag = checkFlag;
                            if (Enemy.GetMonsterCount() == 0 && Duel.Phase < DuelPhase.Main2)
                                summonFlag |= GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(materials) + 3500 >= Enemy.LifePoints;
                            if (summonFlag)
                            {
                                AI.SelectMaterials(materials);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
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

        public bool SummonForTYPHONCheck()
        {
            if (!Bot.HasInExtra(CardId.SuperStarslayerTYPHON) || Bot.GetMonsters().Any(card => card.IsFaceup())) return false;
            if (enemySpSummonFromExLastTurn < 2 && enemySpSummonFromExThisTurn < 2) return false;
            if (Card.Level > 4) return false;
            
            int currentAttack = 0;
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(Card.Id);
            if (cardData != null) currentAttack = cardData.Attack;
            List<ClientCard> summonList = Bot.Hand.Where(card => card.IsMonster() && card.Level <= 4).ToList();
            foreach (ClientCard checkCard in summonList)
            {
                cardData = YGOSharp.OCGWrapper.NamedCard.Get(checkCard.Id);
                if (cardData != null && cardData.Attack < currentAttack) return false;
            }

            return true;
        }
        public bool SuperStarslayerTYPHONSpSummon()
        {
            ClientCard material = Bot.GetMonsters().Where(card => card.IsFaceup()).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (material == null || (material.Attack >= 2900 && material.Owner == 0)) return false;

            bool checkFlag = GetProblematicEnemyMonster(material.Attack) != null;
            checkFlag |= material.Level <= 4;
            checkFlag &= !(material.HasType(CardType.Link) && Duel.Phase >= DuelPhase.Main2);
            if (checkFlag)
            {
                Logger.DebugWriteLine("*** TYPHON select: " + material.Name ?? "UnkonwCard");
                AI.SelectMaterials(material);
                return true;
            }

            return false;
        }
        public bool SuperStarslayerTYPHONActivate()
        {
            if (CheckWhetherNegated()) return false;
            List<ClientCard> targetList = new List<ClientCard>();
            targetList.AddRange(Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c) &&
                c.IsFloodgate() && c.IsFaceup()).OrderByDescending(card => card.Attack));
            targetList.AddRange(Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c) &&
                c.IsMonsterDangerous() && c.IsFaceup()).OrderByDescending(card => card.Attack));
            targetList.AddRange(Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c) &&
                c.IsMonsterInvincible() && c.IsFaceup()).OrderByDescending(card => card.Attack));
            targetList.AddRange(Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c) &&
                c.GetDefensePower() >= Util.GetBestAttack(Bot) && c.IsAttack()).OrderByDescending(card => card.Attack));
            if (Duel.Phase >= DuelPhase.Main2)
                targetList.AddRange(Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c) &&
                c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link | CardType.SpSummon)).OrderByDescending(card => card.Attack));

            if (targetList.Count() > 0)
            {
                targetList.AddRange(Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)).OrderByDescending(card => card.Attack));
                targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown() && !targetList.Contains(card)).ToList()));
                targetList.AddRange(ShuffleList(Bot.GetMonsters().Where(card => card.IsFacedown() && !targetList.Contains(card)).ToList()));
                targetList.AddRange(Bot.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)).OrderBy(card => card.Attack));
                AI.SelectCard(Card.Overlays);
                Logger.DebugWriteLine("TYPHON first target: " + targetList[0]?.Name ?? "UNKNOWN");
                AI.SelectNextCard(targetList);
                return true;
            }

            return false;
        }

        public bool UnchainedAbominationSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(false)) return false;
            if (Enemy.GetMonsterCount() > 0 && Bot.HasInMonstersZone(CardId.UnchainedSoulOfAnguish) && !activatedCardIdList.Contains(CardId.UnchainedSoulOfAnguish)) return false;
            List<List<ClientCard>> usableMaterialMultiList = new List<List<ClientCard>>();
            // anguish + 1
            ClientCard anguish = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.UnchainedSoulOfAnguish));
            if (anguish != null)
            {
                List<ClientCard> materials = GetCanBeUsedForLinkMaterial(true, card => card == anguish);
                if (materials.Count() > 0)
                {
                    usableMaterialMultiList.Add(new List<ClientCard> { anguish, materials[0] });
                }
            }
            // link2 + 1 + 1 or link2 + link2
            List<ClientCard> link2List = Bot.GetMonsters().Where(card => card.HasType(CardType.Link) && card.LinkCount == 2
                && !(card.IsCode(CardId.MuckrakerFromTheUnderworld) && summonThisTurn.Contains(card))).OrderBy(card => card.Attack).ToList();
            if (link2List.Count() > 0)
            {
                ClientCard link2Material = null;
                ClientCard littleKnight = link2List.FirstOrDefault(card => card.Sequence >= 5 && card.IsCode(CardId.SPLittleKnight));
                if (littleKnight != null) link2Material = littleKnight;
                else link2Material = link2List[0];
                if (link2List.Count() >= 2)
                {
                    usableMaterialMultiList.Add(new List<ClientCard> { link2Material, link2List.FirstOrDefault(card => card != link2Material) });
                }
                List<ClientCard> remainList = GetCanBeUsedForLinkMaterial(false, card => card != link2Material && !(card.HasType(CardType.Link) && card.LinkMarker > 2));
                if (remainList.Count() >= 2)
                {
                    usableMaterialMultiList.Add(new List<ClientCard> { link2Material, remainList[0], remainList[1] });
                }
            }

            // check material list
            foreach (List<ClientCard> currMaterials in usableMaterialMultiList)
            {
                bool summonFlag = CheckCanDirectAttack() && GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(currMaterials) + 3000 >= Enemy.LifePoints;
                summonFlag |= GetProblematicEnemyMonster(0) != null && GetProblematicEnemyMonster(3000) == null;

                if (summonFlag)
                {
                    AI.SelectMaterials(currMaterials);
                    return true;
                }
            }

            return false;
        }
        public bool UnchainedAbominationActivate()
        {
            if (CheckWhetherNegated()) return false;
            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Monster);
            if (targetList.Count() == 0) return false;
            int logDesc = ActivateDescription;
            if (logDesc >= Util.GetStringId(CardId.UnchainedAbomination, 0))
            {
                logDesc = Util.GetStringId(CardId.UnchainedAbomination, 0) - 10;
            }
            Logger.DebugWriteLine("[UnchainedAbomination]desc: " + logDesc + ", timing = " + CurrentTiming);
            if (ActivateDescription == Util.GetStringId(CardId.UnchainedAbomination, 0)) activatedCardIdList.Add(Card.Id);
            if (ActivateDescription == Util.GetStringId(CardId.UnchainedAbomination, 1) || ActivateDescription == -1) activatedCardIdList.Add(Card.Id + 1);
            if (ActivateDescription == Util.GetStringId(CardId.UnchainedAbomination, 2)) activatedCardIdList.Add(Card.Id + 2);
            AI.SelectCard(targetList);

            return true;
        }

        public bool UnchainedSoulOfAnguishSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(false)) return false;

            ClientCard unchainedNonLink = Bot.GetMonsters().FirstOrDefault(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained) && !card.HasType(CardType.Link));
            ClientCard unchainedLink2 = Bot.GetMonsters().FirstOrDefault(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained) && card.HasType(CardType.Link) && card.LinkCount == 2);
            Logger.DebugWriteLine("[Anguish summon] unchainedNonLink = " + unchainedNonLink?.Name + ", unchainedLink2 = " + unchainedLink2?.Name);
            if (unchainedNonLink == null && unchainedLink2 == null) return false;
            int needMonsterCount = 2;
            if (unchainedLink2 != null) needMonsterCount = 1;
            if (needMonsterCount == 2 && Bot.HasInExtra(CardId.UnchainedSoulLordOfYama)) return false;
            bool needAnguish = !Bot.HasInMonstersZone(CardId.UnchainedSoulOfAnguish) && !activatedCardIdList.Contains(CardId.UnchainedSoulOfAnguish)
                && Enemy.GetMonsters().Any(card => card.IsFaceup());
            if (needAnguish)
            {
                needAnguish = Bot.HasInExtra(CardId.UnchainedSoulOfRage);
                needAnguish |= Bot.HasInExtra(CardId.UnchainedAbomination);
                needAnguish |= Bot.HasInExtra(CardId.SPLittleKnight) && banSpSummonExceptFiendCount == 0;
            }
            Logger.DebugWriteLine("[Anguish summon] needAnguish = " + needAnguish.ToString());

            // check material
            if (needMonsterCount == 1)
            {
                List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(needAnguish, card => card == unchainedLink2);
                Logger.DebugWriteLine("[Anguish summon 1] material count = " + materialList.Count().ToString());
                if (materialList.Count() == 0) return false;
                List<ClientCard> selectMaterials = new List<ClientCard>{ unchainedLink2, materialList[0]};
                bool summonFlag = needAnguish;
                summonFlag |= CheckCanDirectAttack() && GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(selectMaterials) + 2400 >= Enemy.LifePoints;
                Logger.DebugWriteLine("[Anguish summon 1] summon flag " + summonFlag.ToString());
                if (summonFlag)
                {
                    AI.SelectMaterials(selectMaterials);
                    return true;
                }
            }
            if (needMonsterCount == 2)
            {
                List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(needAnguish, card => card == unchainedNonLink);
                Logger.DebugWriteLine("[Anguish summon 2] material count = " + materialList.Count().ToString());
                if (materialList.Count() >= 2)
                {
                    List<ClientCard> selectMaterials = new List<ClientCard> { unchainedNonLink, materialList[0], materialList[1] };
                    if (needAnguish || GetMaterialAttack(selectMaterials) < 2400)
                    {
                        AI.SelectMaterials(selectMaterials);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool UnchainedSoulOfAnguishActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated()) return false;
                List<ClientCard> targetList = Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeMonsterTarget()).OrderByDescending(card => card.Attack).ToList();
                if (targetList.Count() > 0)
                {
                    currentDestroyCardList.Add(targetList[0]);
                    int summonId = 0;
                    if (Bot.HasInExtra(CardId.UnchainedAbomination) && GetProblematicEnemyMonster(3000, ignoreCurrentDestroy:true) == null)
                        summonId = CardId.UnchainedAbomination;
                    else if (banSpSummonExceptFiendCount == 0 && Bot.HasInExtra(CardId.SPLittleKnight) && GetProblematicEnemyCardList(true, false, CardType.Monster).Count() > 0)
                        summonId = CardId.SPLittleKnight;
                    else if (Bot.HasInExtra(CardId.UnchainedSoulOfRage)) summonId = CardId.UnchainedSoulOfRage;
                    if (summonId > 0)
                    {
                        List<ClientCard> materialList = new List<ClientCard>(targetList){Card};
                        Logger.DebugWriteLine("*** Anguish select: " + summonId.ToString());

                        AI.SelectCard(targetList);
                        AI.SelectNextCard(summonId);
                        AI.SelectMaterials(materialList);
                        activatedCardIdList.Add(Card.Id);
                    }
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                return UnchainRecycleActivate();
            }

            return false;
        }

        public bool UnchainedSoulLordOfYamaSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(false)) return false;
            if (Bot.HasInMonstersZone(CardId.UnchainedSoulLordOfYama) || activatedCardIdList.Contains(CardId.UnchainedSoulLordOfYama)) return false;

            bool need3Monster = Bot.HasInExtra(CardId.UnchainedSoulOfAnguish) && !Bot.HasInMonstersZone(CardId.UnchainedSoulOfAnguish)
                && !activatedCardIdList.Contains(CardId.UnchainedSoulOfAnguish) && GetProblematicEnemyMonster(canBeTarget:true, selfType: CardType.Monster) != null;
            need3Monster |= CheckAtAdvantage() && Duel.Phase == DuelPhase.Main2
                && Bot.HasInExtra(CardId.UnchainedSoulOfRage) && !Bot.HasInMonstersZone(CardId.UnchainedSoulOfRage);
            bool haveUnchainSoul = Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained));
            if (need3Monster)
            {
                need3Monster = Bot.HasInExtra(CardId.UnchainedSoulOfRage);
                need3Monster |= Bot.HasInExtra(CardId.UnchainedAbomination);
                need3Monster |= Bot.HasInExtra(CardId.SPLittleKnight) && banSpSummonExceptFiendCount == 0;
            }
            // check material
            List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(need3Monster,
                    card => !card.HasRace(CardRace.Fiend) || (card.HasType(CardType.Link) && card.HasSetcode(SetcodeUnchained)));
            Logger.DebugWriteLine("[Yama Summon]need3Monster = " + need3Monster.ToString() + ", material count = " + materialList.Count());
            for (int index1 = 0; index1 < materialList.Count() - 1; ++ index1)
            {
                ClientCard material1 = materialList[index1];
                for (int index2 = index1 + 1; index2 < materialList.Count(); ++ index2)
                {
                    ClientCard material2 = materialList[index2];
                    List<ClientCard> selectMaterials = new List<ClientCard>{material1, material2};
                    if (need3Monster && materialList.Count() == 2 && (activatedCardIdList.Contains(CardId.UnchainedSoulOfSharvara) || Bot.GetSpells().Count() == 0))
                    {
                        // only for attack
                        if (GetProblematicEnemyMonster() != null || !CheckCanDirectAttack() || GetMaterialAttack(selectMaterials) >= 2000) return false;
                    }
                    bool summonFlag = need3Monster;
                    summonFlag |= Enemy.GetMonsterCount() == 0 && GetMaterialAttack(selectMaterials) < 2000;
                    summonFlag |= CheckAtAdvantage() && !haveUnchainSoul;
                    if (summonFlag)
                    {
                        AI.SelectMaterials(selectMaterials);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool UnchainedSoulLordOfYamaActivate()
        {
            if (Card.Location == CardLocation.MonsterZone && (ActivateDescription == Util.GetStringId(CardId.UnchainedSoulLordOfYama, 0) || ActivateDescription == -1))
            {
                // search
                if (CheckWhetherNegated()) return false;
                AI.SelectCard(CardId.UnchainedSoulOfSharvara, CardId.UnchainedAbomination, CardId.UnchainedSoulOfAnguish, CardId.UnchainedSoulOfRage);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                // spsummon & destroy
                ClientCard chaosAngel = null;
                ClientCard abomination = null;
                ClientCard lady = null;
                ClientCard lovely = null;
                ClientCard arianna = null;
                ClientCard bestAttack = null;
                ClientCard rage = null;
                foreach (ClientCard grave in Bot.Graveyard)
                {
                    if (grave.IsCode(CardId.ChaosAngel) && grave.ProcCompleted != 0 && !dimensionalBarrierAnnouced.Contains(HintMsg.SYNCHRO)) chaosAngel = grave;
                    if (grave.IsCode(CardId.UnchainedSoulOfRage) && grave.ProcCompleted != 0) rage = grave;
                    if (grave.IsCode(CardId.UnchainedAbomination) && grave.ProcCompleted != 0) abomination = grave;
                    if (grave.IsCode(CardId.LadyLabrynthOfTheSilverCastle)) lady = grave;
                    if (grave.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)) lovely = grave;
                    if (grave.IsCode(CardId.AriannaTheLabrynthServant)) arianna = grave;
                    if (Card != grave && grave.IsMonster() && grave.HasRace(CardRace.Fiend))
                    {
                        if (!grave.IsCanRevive()) continue;
                        if (bestAttack == null || grave.Attack > bestAttack.Attack) bestAttack = grave;
                    }
                }

                ClientCard select = null;
                bool destroyWelcome = false;
                if (chaosAngel != null && (GetProblematicEnemyCardList(selfType: CardType.Monster).Count() > 0 ||
                    (Bot.GetMonsterCount() == 0 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                {
                    select = chaosAngel;
                }
                if (select == null && abomination != null && (GetProblematicEnemyCardList(selfType: CardType.Monster).Count() > 0 ||
                    (Bot.GetMonsterCount() == 0 && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                {
                    select = abomination;
                    Logger.DebugWriteLine("[Yama] timing: " + CurrentTiming.ToString());
                    if (Bot.HasInSpellZone(CardId.WelcomeLabrynth) && !(Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                        && !activatedCardIdList.Contains(CardId.UnchainedAbomination))
                    {
                        destroyWelcome = true;
                    }
                }
                if (select == null && rage != null && (Duel.Player == 0 || (!activatedCardIdList.Contains(CardId.UnchainedSoulOfRage) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)))
                    && Bot.HasInExtra(new List<int> { CardId.UnchainedSoulOfAnguish, CardId.SPLittleKnight })) select = rage;
                if (select == null && arianna != null && Duel.Player == 0 && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)) select = arianna;
                if (select == null && lovely != null && Duel.Player == 1 && Util.GetBestAttack(Enemy) < 2900) select = lovely;
                if (select == null && lady != null && Duel.Player == 1 && Util.GetBestAttack(Enemy) < 3000) select = lady;
                if (select == null && arianna != null && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)) select = arianna;
                if (select == null && bestAttack != null) select = bestAttack;

                if (select != null)
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    AI.SelectCard(select);
                    if (destroyWelcome)
                    {
                        AI.SelectYesNo(true);
                        AI.SelectNextCard(CardId.WelcomeLabrynth);
                    } else {
                        AI.SelectYesNo(false);
                    }
                    return true;
                }
            }

            return false;
        }

        public bool UnchainedSoulOfRageSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(false) || CheckWhetherNegated(true, true, CardType.Monster | CardType.Link)) return false;
            if (Bot.HasInMonstersZone(CardId.UnchainedSoulOfRage)) return false;

            ClientCard unchained = Bot.GetMonsters().FirstOrDefault(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained)
                && !card.IsCode(CardId.UnchainedSoulOfAnguish, CardId.UnchainedAbomination));
            if (unchained == null) return false;

            bool summonFlag = CheckAtAdvantage() && Util.IsTurn1OrMain2();
            summonFlag |= !(Bot.HasInExtra(CardId.UnchainedSoulOfAnguish) && !activatedCardIdList.Contains(CardId.UnchainedSoulOfAnguish)) && Util.IsTurn1OrMain2();
            if (summonFlag)
            {
                summonFlag = Bot.HasInExtra(CardId.UnchainedSoulOfAnguish);
                summonFlag |= Bot.HasInExtra(CardId.SPLittleKnight) && banSpSummonExceptFiendCount == 0;
            }

            List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(Util.IsTurn1OrMain2(),
                card => !card.HasRace(CardRace.Fiend) || card == unchained);
            if (materialList.Count() > 0)
            {
                List<ClientCard> selectMaterials = new List<ClientCard>{unchained, materialList[0]};
                summonFlag |= Enemy.GetMonsterCount() == 0 && GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(selectMaterials) + 1800 >= Enemy.LifePoints;
                if (summonFlag)
                {
                    AI.SelectMaterials(selectMaterials);
                    return true;
                }
            }

            return false;
        }
        public bool UnchainedSoulOfRageActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated()) return false;
                bool activateFlag = DefaultOnBecomeTarget() && !Util.ChainContainsCard(CardId.EscapeOfTheUnchained);
                ClientCard problemMonster = GetProblematicEnemyMonster(-1, true, true, CardType.Monster);
                List<ClientCard> targetList = Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeTarget()).OrderBy(card => card.Attack).ToList();
                if (problemMonster != null) targetList.Insert(0, problemMonster);

                activateFlag |= (CurrentTiming & hintTimingMainEnd) > 0 && Util.IsOneEnemyBetterThanValue(Card.Attack, true);
                activateFlag |= problemMonster != null;

                if (activateFlag && targetList.Count() > 0)
                {
                    ClientCard target = targetList[0];
                    int summonId = 0;
                    if (Bot.HasInExtra(CardId.UnchainedAbomination) && GetProblematicEnemyMonster(3000) == null
                        && target.HasType(CardType.Link) && target.LinkCount == 2) summonId = CardId.UnchainedSoulOfAnguish;
                    else if (banSpSummonExceptFiendCount == 0 && Bot.HasInExtra(CardId.SPLittleKnight)) summonId = CardId.SPLittleKnight;
                    else if (Bot.HasInExtra(CardId.UnchainedSoulOfAnguish) && GetProblematicEnemyMonster(2400) == null) summonId = CardId.UnchainedSoulOfAnguish;
                    List<ClientCard> materialList = new List<ClientCard>(targetList){Card};

                    AI.SelectCard(targetList);
                    AI.SelectNextCard(summonId);
                    AI.SelectMaterials(materialList);
                    activatedCardIdList.Add(Card.Id);
                    escapeTargetList.Add(Card);
                    currentDestroyCardList.Add(target);
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                return UnchainRecycleActivate();
            }

            return false;
        }

        public bool UnchainRecycleActivate()
        {
            AI.SelectCard(CardId.UnchainedSoulOfSharvara, CardId.LovelyLabrynthOfTheSilverCastle, CardId.AriannaTheLabrynthServant,
                CardId.UnchainedAbomination, CardId.LabrynthStovieTorbie, CardId.LabrynthChandraglier, CardId.LabrynthCooclock,
                CardId.AriasTheLabrynthButler, CardId.ArianeTheLabrynthServant);
            activatedCardIdList.Add(Card.Id + 1);

            return true;
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
            } else if (!CheckWhetherNegated(true, true, CardType.Monster | CardType.Link) && GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0)
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
        public List<ClientCard> SPLittleKnightSelectMaterial(bool needToUseEffect = false)
        {
            List<ClientCard> usedMaterialList = new List<ClientCard>();
            if (Bot.GetMonstersExtraZoneCount() > 0)
            {
                ClientCard botMonsterExtraZome = Bot.GetMonstersInExtraZone()[0];
                if (botMonsterExtraZome.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Pendulum) || botMonsterExtraZome.IsCode(CardId.RelinquishedAnima))
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

        public bool MuckrakerFromTheUnderworldSpSummon()
        {
            List<ClientCard> materialList = GetCanBeUsedForLinkMaterial(true, card => card.HasType(CardType.Link));
            if (materialList.Count() < 2) return false;
            bool willBeNegated = CheckWhetherNegated(true, true, CardType.Monster | CardType.Link) && Bot.Hand.Count() > 0;
            bool canRebornAngel = Bot.Graveyard.Any(card => card.IsCanRevive() && card.IsCode(CardId.ChaosAngel)) && !willBeNegated;
            bool canRebornLovely = Bot.Graveyard.Any(card => card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)) && !willBeNegated;
            int bestAttackGrave = 0;
            bool chaosAngelFlag = GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0 && !CheckWhetherNegated(true, true, CardType.Monster);
            foreach (ClientCard grave in Bot.Graveyard)
            {
                if (grave.IsMonster() && grave.HasRace(CardRace.Fiend))
                {
                    if (!grave.IsCanRevive()) continue;
                    if (grave.Attack > bestAttackGrave) bestAttackGrave = grave.Attack;
                }
            }
            for (int idx1 = 0; idx1 < materialList.Count() - 1; ++ idx1)
            {
                ClientCard material1 = materialList[idx1];
                for (int idx2 = idx1 + 1; idx2 < materialList.Count(); ++ idx2)
                {
                    ClientCard material2 = materialList[idx1];
                    List<ClientCard> currentList = new List<ClientCard> { material1, material2 };
                    bool summonFlag = chaosAngelFlag && (canRebornAngel || (currentList.Any(card => card.IsCode(CardId.ChaosAngel)) && !willBeNegated));
                    summonFlag |= Enemy.GetMonsterCount() == 0 && canRebornLovely;
                    summonFlag |= !activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle) && Bot.Graveyard.Any(card => card.Type == (int)CardType.Trap)
                        && currentList.Any(card => card.IsDisabled() && card.IsCode(CardId.LovelyLabrynthOfTheSilverCastle));
                    if (CheckCanDirectAttack())
                    {
                        summonFlag |= GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack(currentList) + bestAttackGrave >= Enemy.LifePoints;
                        summonFlag |= GetMaterialAttack(currentList) < 1000;
                    }
                    if (summonFlag)
                    {
                        AI.SelectMaterials(currentList);
                        return true;
                    }
                }
            }
            return false;
        }
        public bool MuckrakerFromTheUnderworldActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.MuckrakerFromTheUnderworld, 0))
            {
                if (CheckWhetherNegated()) return false;
                ClientCard chaosAngel = null;
                ClientCard lovely = null;
                ClientCard arianna = null;
                ClientCard bestAttack = null;
                foreach (ClientCard grave in Bot.Graveyard)
                {
                    if (grave.IsCode(CardId.ChaosAngel) && grave.ProcCompleted != 0 && !dimensionalBarrierAnnouced.Contains(HintMsg.SYNCHRO)) chaosAngel = grave;
                    if (grave.IsCode(CardId.LovelyLabrynthOfTheSilverCastle)) lovely = grave;
                    if (grave.IsCode(CardId.AriannaTheLabrynthServant)) arianna = grave;
                    if (Card != grave && grave.IsMonster() && grave.HasRace(CardRace.Fiend))
                    {
                        if (!grave.IsCanRevive()) continue;
                        if (bestAttack == null || grave.Attack > bestAttack.Attack) bestAttack = grave;
                    }
                }

                ClientCard rebornTarget = null;
                if (chaosAngel != null && (GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0)) rebornTarget = chaosAngel;
                if (rebornTarget == null && lovely != null && Util.GetBestAttack(Enemy) < 2900 &&
                    (!activatedCardIdList.Contains(CardId.LovelyLabrynthOfTheSilverCastle) || Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth))) rebornTarget = lovely;
                if (rebornTarget == null && bestAttack != null && CheckCanDirectAttack()
                        && GetBotCurrentTotalAttack() < Enemy.LifePoints && GetBotCurrentTotalAttack() + bestAttack.Attack >= Enemy.LifePoints) rebornTarget = bestAttack;
                if (rebornTarget == null && arianna != null && Duel.Player == 0 && !activatedCardIdList.Contains(CardId.AriannaTheLabrynthServant)) rebornTarget = arianna;
                if (rebornTarget == null && bestAttack != null) rebornTarget = bestAttack;
                if (rebornTarget != null)
                {
                    AI.SelectCard(rebornTarget);
                    AI.SelectNextCard(FurnitureGetCost());
                    activatedCardIdList.Contains(Card.Id);
                    banSpSummonExceptFiendCount = Math.Max(1, banSpSummonExceptFiendCount);
                    return true;
                }
            }

            return false;
        }

        public bool RelinquishedAnimaSpSummon()
        {
            if (CheckWhetherNegated()) return false;
            // summon to use effect
            ClientCard enemyLeftEx = Enemy.MonsterZone[6];
            if (enemyLeftEx != null && enemyLeftEx.HasLinkMarker((int)CardLinkMarker.Top) && !enemyLeftEx.IsShouldNotBeTarget() && !enemyLeftEx.IsShouldNotBeMonsterTarget())
            {
                ClientCard selfMonsterZone1 = Bot.MonsterZone[1];
                if (selfMonsterZone1 == null)
                {
                    AI.SelectMaterials(CardId.LabrynthCooclock);
                    AI.SelectPlace(Zones.z1);
                    return true;
                }
                else if (!selfMonsterZone1.HasType(CardType.Xyz | CardType.Link | CardType.Token) && selfMonsterZone1.Level == 1)
                {
                    AI.SelectMaterials(selfMonsterZone1);
                    AI.SelectPlace(Zones.z1);
                    return true;
                }
            }
            ClientCard enemyRightEx = Enemy.MonsterZone[5];
            if (enemyRightEx != null && enemyRightEx.HasLinkMarker((int)CardLinkMarker.Top) && !enemyRightEx.IsShouldNotBeTarget() && !enemyRightEx.IsShouldNotBeMonsterTarget())
            {
                ClientCard selfMonsterZone3 = Bot.MonsterZone[3];
                if (selfMonsterZone3 == null)
                {
                    AI.SelectMaterials(CardId.LabrynthCooclock);
                    AI.SelectPlace(Zones.z3);
                    return true;
                }
                else if (!selfMonsterZone3.HasType(CardType.Xyz | CardType.Link | CardType.Token) && selfMonsterZone3.Level == 1)
                {
                    AI.SelectMaterials(selfMonsterZone3);
                    AI.SelectPlace(Zones.z3);
                    return true;
                }
            }
            
            if (Bot.MonsterZone[5] != null || Bot.MonsterZone[6] != null) return false;
            ClientCard enemyMonsterLeft = Enemy.MonsterZone[3];
            ClientCard enemyMonsterRight = Enemy.MonsterZone[1];
            if (Enemy.MonsterZone[6] != null) enemyMonsterLeft = null;
            if (enemyMonsterLeft != null && enemyMonsterLeft.IsFacedown()) enemyMonsterLeft = null;
            if (enemyMonsterLeft != null && (enemyMonsterLeft.IsShouldNotBeMonsterTarget() || enemyMonsterLeft.IsShouldNotBeTarget())) enemyMonsterLeft = null;

            if (Enemy.MonsterZone[5] != null) enemyMonsterRight = null;
            if (enemyMonsterRight != null && (enemyMonsterRight.IsShouldNotBeMonsterTarget() || enemyMonsterRight.IsShouldNotBeTarget())) enemyMonsterRight = null;
            if (enemyMonsterRight != null && enemyMonsterRight.IsFacedown()) enemyMonsterRight = null;

            int place = -1;
            if (enemyMonsterLeft != null && enemyMonsterRight == null) place = Zones.z5;
            if (enemyMonsterLeft == null && enemyMonsterRight != null) place = Zones.z6;
            if (enemyMonsterLeft != null && enemyMonsterRight != null)
            {
                if (enemyMonsterLeft.IsFloodgate() && !enemyMonsterRight.IsFloodgate()) place = Zones.z5;
                else if (!enemyMonsterLeft.IsFloodgate() && enemyMonsterRight.IsFloodgate()) place = Zones.z6;
                else
                {
                    if (enemyMonsterLeft.GetDefensePower() >= enemyMonsterRight.GetDefensePower()) place = Zones.z5;
                    else place = Zones.z6;
                }
            }
            if (place >= 0)
            {
                AI.SelectMaterials(Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link | CardType.Token) && card.Level == 1)
                    .OrderBy(card => card.Attack).ToList());
                AI.SelectPlace(place);
                return true;
            }

            // summon for little knight
            if (Bot.HasInExtra(CardId.SPLittleKnight) && Bot.GetMonsters().Count(card => card.IsFaceup()) >= 2
                    && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)))
            {
                if (GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() > 0)
                {
                    AI.SelectMaterials(Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Xyz | CardType.Link | CardType.Token) && card.Level == 1)
                        .OrderBy(card => card.Attack).ToList());
                    return true;
                }
            }
            return false;
        }
        public bool RelinquishedAnimaActivate()
        {
            if (CheckWhetherNegated()) return false;
            activatedCardIdList.Add(Card.Id);
            Dictionary<int, int> placeList = new Dictionary<int, int>{ {1, 6}, {3, 5}, {5, 3}, {6, 1} };
            foreach (KeyValuePair<int, int> placePair in placeList)
            {
                if (Bot.MonsterZone[placePair.Key] == Card && Enemy.MonsterZone[placePair.Value] != null)
                {
                    currentDestroyCardList.Add(Enemy.MonsterZone[placePair.Value]);
                    break;
                }
            }
            return true;
        }

        public bool MonsterRepos()
        {
            int selfAttack = Card.Attack + 1;

            if (selfAttack <= 1)
                return !Card.IsDefense();

            int bestAttack = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                int attack = card.Attack;
                if (attack >= bestAttack)
                {
                    bestAttack = attack;
                }
            }

            bool enemyBetter = Util.IsAllEnemyBetterThanValue(bestAttack, true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter)
                return true;
            return false;
        }

        public bool ReposForLabrynth()
        {
            if (!activatedCardIdList.Contains(CardId.BigWelcomeLabrynth) && Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrynth))
                return Card.IsFacedown();
            return false;
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            if (Card.IsCode(CardId.BigWelcomeLabrynth) && Bot.HasInSpellZone(Card.Id)) return false;
            if (Card.IsCode(CardId.TransactionRollback) && !Bot.HasInSpellZone(CardId.TransactionRollback))
            {
                // check enemy grave trap
                bool haveCopyTrap = false;
                if (Enemy.Graveyard.Any(card => card.IsCode(
                    CardId.WelcomeLabrynth, CardId.BigWelcomeLabrynth, _CardId.InfiniteImpermanence, _CardId.DimensionalBarrier, CardId.DestructiveDarumaKarmaCannon,
                    _CardId.CompulsoryEvacuationDevice, _CardId.BreakthroughSkill
                )))
                {
                    haveCopyTrap = true;
                }

                if (!haveCopyTrap && !Bot.HasInHand(CardId.UnchainedSoulOfSharvara)) return false;
            }
            if (Card.IsCode(CardId.EscapeOfTheUnchained) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeUnchained))) return false;

            // select place
            if (Card.IsTrap() || Card.HasType(CardType.QuickPlay))
            {
                List<int> avoidList = new List<int>();
                int setFornfiniteImpermanence = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoidList.Add(4 - i);
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
                        SelectSTPlace(Card, false, avoidList);
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
    
        public bool SpellSetForCooClockCheck()
        {
            // set to destroy for Sharvara
            if (Card.IsCode(CardId.PotOfExtravagance, CardId.TransactionRollback, CardId.WelcomeLabrynth) && Bot.HasInHand(CardId.UnchainedSoulOfSharvara)
                && !activatedCardIdList.Contains(CardId.UnchainedSoulOfSharvara))
            {
                SelectSTPlace(Card, false);
                return true;
            }
            // set to activate by cooclock
            bool haveLabrynth = Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeLabrynth));
            if (!cooclockAffected || (haveLabrynth && Bot.HasInHand(CardId.LabrynthCooclock) && !activatedCardIdList.Contains(CardId.LabrynthCooclock))) return false;
            if (!Card.IsCode(CardId.BigWelcomeLabrynth, CardId.WelcomeLabrynth, _CardId.InfiniteImpermanence, _CardId.DimensionalBarrier)) return false;
            if (haveLabrynth)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            if (!Card.IsCode(CardId.BigWelcomeLabrynth, CardId.WelcomeLabrynth)) return false;
            if (!summoned && Bot.Hand.Any(card => card.IsMonster() && card.Level <= 4 && card.HasSetcode(SetcodeLabrynth)))
            {
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }
    }
}