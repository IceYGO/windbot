using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("Dogmatika", "AI_Dogmatika")]

    public class DogmatikaExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DogmatikaAlbaZoa = 51522296;
            public const int ThesIrisSwordsoul = 62849088;
            public const int DogmatikaFleurdelis = 69680031;
            public const int DogmatikaMaximus = 95679145;
            public const int DiabellstarTheBlackWitch = 72270339;
            public const int DogmatikaEcclesia = 60303688;
            // _CardId.AshBlossom = 14558127;
            // _CardId.MaxxC = 23434538;
            public const int KnightmareCorruptorIblee = 10158145;

            public const int NadirServant = 1984618;
            public const int DogmatikaLamity = 31002402;
            public const int DogmatikaMacabre = 60921537;
            public const int SinfulSpoilsOfDoom_Rciela = 16240772;
            // _CardId.CalledByTheGrave = 24224830;
            // _CardId.CrossoutDesignator = 65681983;
            public const int WANTED_SeekerOfSinfulSpoils = 80845034;
            public const int DogmatikaMatrix = 35569555;

            // _CardId.InfiniteImpermanence = 10045474;
            public const int DogmatikaPunishment = 82956214;

            public const int GranguignolTheDuskDragon = 24915933;
            public const int TitanikladTheAshDragon = 41373230;
            public const int GaruraWingsOfResonantLife = 11765832;
            public const int ElderEntityNtss = 80532587;
            public const int DespianLuluwalilith = 53971455;
            public const int PSYFramelordOmega = 74586817;
            public const int HeraldOfTheArcLight = 79606837;
            public const int SuperStarslayerTYPHON = 93039339;
            public const int SPLittleKnight = 29301450;
            public const int SecureGardna = 2220237;
            public const int Linguriboh = 24842059;
            public const int SalamangreatAlmiraj = 60303245;

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
            public const int GhostMournerMoonlitChill = 52038441;
        }

        public DogmatikaExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // startup effect
            AddExecutor(ExecutorType.Activate, CardId.WANTED_SeekerOfSinfulSpoils, WANTED_SeekerOfSinfulSpoilsActivate);
            AddExecutor(ExecutorType.Activate, CardId.SalamangreatAlmiraj,         SalamangreatAlmirajActivate);

            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordOmega, PSYFramelordOmegaActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaAlbaZoa,  DogmatikaAlbaZoaActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaMaximus,  DogmatikaMaximusActivate);
            AddExecutor(ExecutorType.Activate, CardId.DiabellstarTheBlackWitch, DiabellstarTheBlackWitchActivate);

            // quick effect
            AddExecutor(ExecutorType.Activate, CardId.ThesIrisSwordsoul,         ThesIrisSwordsoulActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave,         CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator,        CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom,               AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.Linguriboh,                LinguribohActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaPunishment,       DogmatikaPunishmentActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaFleurdelis,       DogmatikaFleurdelisActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence,     InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.SinfulSpoilsOfDoom_Rciela, SinfulSpoilsOfDoom_RcielaActivate);
            AddExecutor(ExecutorType.Activate, ClearIrisFlag);

            AddExecutor(ExecutorType.Activate, CardId.HeraldOfTheArcLight,       HeraldOfTheArcLightActivate);
            AddExecutor(ExecutorType.Activate, CardId.ElderEntityNtss,           ElderEntityNtssActivate);
            AddExecutor(ExecutorType.Activate, CardId.GranguignolTheDuskDragon,  GranguignolTheDuskDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.GaruraWingsOfResonantLife, GaruraWingsOfResonantLifeActivate);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareCorruptorIblee,  KnightmareCorruptorIbleeActivate);
            AddExecutor(ExecutorType.Activate, CardId.TitanikladTheAshDragon,    TitanikladTheAshDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.DespianLuluwalilith,       DespianLuluwalilithActivate);

            // free chain
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperStarslayerTYPHON,  SuperStarslayerTYPHONActivate);

            // spsummon
            AddExecutor(ExecutorType.SpSummon, CardId.Linguriboh,            LinguribohSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatAlmiraj,   SalamangreatAlmirajSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SecureGardna,          SecureGardnaSpSummon);

            // startup effect
            AddExecutor(ExecutorType.Summon, CardId.KnightmareCorruptorIblee,   KnightmareCorruptorIbleeSummon);
            AddExecutor(ExecutorType.Activate, CardId.NadirServant,             NadirServantActivate);

            // summon
            AddExecutor(ExecutorType.Summon, CardId.DogmatikaEcclesia,          DogmatikaEcclesiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaEcclesia,        DogmatikaEcclesiaActivate);

            // ritual
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaMatrix,  DogmatikaMatrixActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaLamity,  DogmatikaLamityActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaMacabre, DogmatikaMacabreActivate);

            // other
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaFleurdelis,      DogmatikaFleurdelisDelayActivate);
            AddExecutor(ExecutorType.Repos,    MonsterRepos);
            AddExecutor(ExecutorType.Summon,   SummonForTYPHONCheck);
            AddExecutor(ExecutorType.SpSummon, CardId.DiabellstarTheBlackWitch, DiabellstarTheBlackWitchSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaLamity,          DogmatikaLamityDelayActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperStarslayerTYPHON,    SuperStarslayerTYPHONSpSummon);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeDogmatika = 0x145;
        const int hintTimingMainEnd = 0x4;
        const int hintDamageStep = 0x2000;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.DogmatikaEcclesia, _CardId.AshBlossom, _CardId.MaxxC, CardId.KnightmareCorruptorIblee, CardId.NadirServant,
                                CardId.WANTED_SeekerOfSinfulSpoils, CardId.DogmatikaMatrix, _CardId.InfiniteImpermanence, CardId.DogmatikaPunishment }},
            {2, new List<int> { CardId.DogmatikaAlbaZoa, CardId.DogmatikaFleurdelis, _CardId.CalledByTheGrave }},
            {1, new List<int> { CardId.ThesIrisSwordsoul, CardId.DogmatikaMaximus, CardId.DiabellstarTheBlackWitch, CardId.DogmatikaLamity, CardId.DogmatikaMacabre,
                                CardId.SinfulSpoilsOfDoom_Rciela, _CardId.CrossoutDesignator }},
        };
        List<int> notToNegateIdList = new List<int>{
            58699500, 20343502
        };
        List<int> discardEnemyExtraIdList = new List<int>{
            _CardId.DivineArsenalAAZEUS_SkyThunder, CardId.SPLittleKnight, CardId.Number41BagooskatheTerriblyTiredTapir,
            70534340, 60465049, 24094258, 86066372
        };

        List<int> currentNegatingIdList = new List<int>();
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        List<int> infiniteImpermanenceList = new List<int>();
        bool summoned = false;
        List<int> activatedCardIdList = new List<int>();
        List<ClientCard> currentNegateMonsterList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<int> discardExtraThisTurn = new List<int>();
        int banSpSummonFromExTurn = 0;
        List<ClientCard> activatedMatrixList = new List<ClientCard>();
        List<int> maximusDiscardExtraIdList = new List<int>();
        bool checkedEnemyExtra = false;
        bool matrixActivating = false;
        bool avoid2Monster = true;
        bool confirmLink2 = false;
        int omegaActivateCount = 0;
        int dimensionShifterCount = 0;

        int enemySpSummonFromExLastTurn = 0;
        int enemySpSummonFromExThisTurn = 0;
        bool enemySpSummonFromDeck = false;
        bool enemySpSummonFromExtra = false;

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

        public void UpdateBanSpSummonFromExTurn(int newTurn)
        {
            if (Duel.Player == 1) newTurn -= 1;
            banSpSummonFromExTurn = Math.Max(banSpSummonFromExTurn, newTurn);
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false)
        {
            List<ClientCard> floodagateList = Enemy.GetMonsters().Where(c => c?.Data != null &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).ToList();
            if (floodagateList.Count() > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).ToList();
            if (dangerList.Count() > 0)
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).ToList();
            if (invincibleList.Count() > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = Util.GetBestAttack(Bot);
                List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                    .Where(card => card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && (!canBeTarget || !card.IsShouldNotBeTarget())
                    && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
                if (betterList.Count() > 0)
                {
                    betterList.Sort(CardContainer.CompareCardAttack);
                    betterList.Reverse();
                    return betterList[0];
                }
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
        
        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false, bool ignoreCurrentDestroy = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget, ignoreCurrentDestroy);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.Where(c => c?.Data != null && c.HasType(CardType.Monster) && c.IsFaceup()
                && !(canBeTarget && c.IsShouldNotBeTarget()) && (!ignoreCurrentDestroy || currentDestroyCardList.Contains(c)))
                .OrderByDescending(c => c.Attack).FirstOrDefault();
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters().Where(c => !ignoreCurrentDestroy || currentDestroyCardList.Contains(c)).ToList();

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

        /// <summary>
        /// check enemy's dangerous card in grave
        /// </summary>
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom))).ToList();
            List<int> dangerMonsterIdList = new List<int>{
                99937011, 63542003, 9411399, 28954097, 30680659, CardId.PSYFramelordOmega
            };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool targetKnightmare = true, bool ignoreCurrentDestroy = false)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && !card.IsCode(CardId.KnightmareCorruptorIblee) && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleCardList(Enemy.GetSpells().Where(card => !ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)).ToList()));
            targetList.AddRange(ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFacedown() && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList()));
            if (targetKnightmare)
            {
                List<ClientCard> enemyKnightmare = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card) && card.IsCode(CardId.KnightmareCorruptorIblee)
                    && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
                targetList.AddRange(enemyKnightmare);
            }

            return targetList;
        }

        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeMonsterTarget = false, bool canBeTrapTarget = false)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            if (CheckWhetherNegated())
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
        /// Get ritual monster/spell's ids that need to search, in order to perform ritual summon.
        /// </summary>
        public List<int> GetNeedSearchRitualCardIdList()
        {
            List<int> result = new List<int>();

            bool canSearchAlbaZoa = !Bot.HasInHand(CardId.DogmatikaAlbaZoa) && CheckRemainInDeck(CardId.DogmatikaAlbaZoa) > 0;
            int totalLevelInGY = Bot.Graveyard.Where(card => card != null && card.HasType(CardType.Fusion | CardType.Synchro)).Sum(c => (int?)c.Level ?? 0);

            bool needSearchAlbaZoa = Bot.HasInHandOrInSpellZone(CardId.DogmatikaLamity) && Bot.HasInExtra(CardId.DespianLuluwalilith) && canSearchAlbaZoa;
            if (Bot.HasInHandOrInGraveyard(CardId.DogmatikaMacabre))
            {
                needSearchAlbaZoa |= totalLevelInGY >= 12 && canSearchAlbaZoa;
            }
            if (needSearchAlbaZoa)
            {
                result.Add(CardId.DogmatikaAlbaZoa);
            }

            if (Bot.HasInHand(CardId.DogmatikaAlbaZoa) && Bot.HasInExtra(CardId.DespianLuluwalilith) && !Bot.HasInHandOrInSpellZone(CardId.DogmatikaLamity)
                && CheckRemainInDeck(CardId.DogmatikaLamity) > 0)
            {
                result.Add(CardId.DogmatikaLamity);
            }

            if (Bot.HasInHand(CardId.DogmatikaAlbaZoa) && !Bot.HasInHandOrInSpellZone(CardId.DogmatikaMacabre) && CheckRemainInDeck(CardId.DogmatikaMacabre) > 0
                && totalLevelInGY >= 12)
            {
                result.Add(CardId.DogmatikaMacabre);
            }

            return result;
        }

        public ClientCard GetExtraToDiscard(int baseAtk = 0, ClientCard avoidDestroyEnemyCard = null)
        {
            ClientCard selectResult = null;
            // Ntss
            if (baseAtk <= 2500 && Bot.HasInExtra(CardId.ElderEntityNtss) && CheckCalledbytheGrave(CardId.ElderEntityNtss) == 0)
            {
                List<ClientCard> destroyList = GetNormalEnemyTargetList(true, false);
                if (destroyList.Count() > 0)
                {
                    if (!(destroyList.Count() == 1 && destroyList[0] == avoidDestroyEnemyCard))
                    {
                        selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.ElderEntityNtss));
                        if (selectResult != null)
                        {
                            return selectResult;
                        }
                    }
                }
            }

            // Garura
            if (baseAtk <= 1500 && Bot.HasInExtra(CardId.GaruraWingsOfResonantLife) && CheckCalledbytheGrave(CardId.GaruraWingsOfResonantLife) == 0
                && !activatedCardIdList.Contains(CardId.GaruraWingsOfResonantLife) && !enemyActivateLockBird)
            {
                selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.GaruraWingsOfResonantLife));
                if (selectResult != null)
                {
                    return selectResult;
                }
            }

            // Ash Dragon
            if (baseAtk <= 2500 && Bot.HasInExtra(CardId.TitanikladTheAshDragon) && CheckCalledbytheGrave(CardId.TitanikladTheAshDragon) == 0
                && !discardExtraThisTurn.Contains(CardId.TitanikladTheAshDragon) && !enemyActivateLockBird)
            {
                bool successFlag = !activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0;
                successFlag |= Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)) && !Bot.HasInHand(CardId.DogmatikaFleurdelis) && CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0;
                if (successFlag)
                {
                    selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.TitanikladTheAshDragon));
                    if (selectResult != null)
                    {
                        return selectResult;
                    }
                }
            }

            // dusk dragon
            if (baseAtk <= 2500 && Bot.HasInExtra(CardId.GranguignolTheDuskDragon))
            {
                bool successFlag = Bot.HasInExtra(CardId.DespianLuluwalilith);
                successFlag |= CheckRemainInDeck(CardId.DogmatikaEcclesia, CardId.DogmatikaFleurdelis, CardId.DogmatikaMaximus) > 0;
                if (successFlag)
                {
                    selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.GranguignolTheDuskDragon));
                    if (selectResult != null)
                    {
                        return selectResult;
                    }
                }
            }

            if (baseAtk <= 600 && Bot.HasInExtra(CardId.HeraldOfTheArcLight) && !enemyActivateLockBird)
            {
                if (GetNeedSearchRitualCardIdList().Count() > 0)
                {
                    selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.HeraldOfTheArcLight));
                    if (selectResult != null)
                    {
                        return selectResult;
                    }
                }
            }

            if (baseAtk <= 2800 && Bot.HasInExtra(CardId.PSYFramelordOmega))
            {
                selectResult = Bot.ExtraDeck.FirstOrDefault(card => card.IsCode(CardId.PSYFramelordOmega));
                if (selectResult != null)
                {
                    return selectResult;
                }
            }

            List<ClientCard> discardableList = Bot.ExtraDeck.Where(card => card != null && card.Attack >= baseAtk).ToList();
            if (discardableList.Count() > 0)
            {
                discardableList.Sort(CardContainer.CompareCardAttack);
                return discardableList[0];
            }

            return selectResult;
        }

        /// <summary>
        /// Check whether negate opposite's effect and clear flag
        /// </summary>
        public void CheckDeactiveFlag()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard != null && Duel.LastChainPlayer == 1)
            {
                if (lastChainCard.IsCode(_CardId.MaxxC)) enemyActivateMaxxC = false;
                if (lastChainCard.IsCode(_CardId.LockBird)) enemyActivateLockBird = false;
                if (lastChainCard.IsCode(CardId.DimensionShifter)) dimensionShifterCount = 0;
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
            if (currentNegatingIdList.Contains(id)) return 1;
            if (DefaultCheckWhetherCardIdIsNegated(id)) return 1;
            return 0;
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
        public bool CheckWhetherNegated(bool toFieldCheck = false)
        {
            if ((Card.IsSpell() || Card.IsTrap()) && CheckSpellWillBeNegate()){
                return true;
            }
            if (DefaultCheckWhetherCardIsNegated(Card))
            {
                return true;
            }
            if (Card.IsMonster() && (toFieldCheck || Card.Location == CardLocation.MonsterZone))
            {
                if (toFieldCheck || Card.IsDefense())
                {
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card)))
                    {
                        return true;
                    }
                }
                if (Enemy.HasInSpellZone(CardId.SkillDrain, true, true))
                {
                    return true;
                }
            }
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
            List<int> checkIdList = new List<int> { CardId.BanisheroftheRadiance, CardId.BanisheroftheLight, CardId.MacroCosmos, CardId.DimensionalFissure };
            foreach (int cardid in checkIdList)
            {
                List<ClientField> fields = new List<ClientField> { Bot, Enemy };
                foreach (ClientField cf in fields)
                {
                    if (cf.HasInMonstersZone(cardid, true) || cf.HasInSpellZone(cardid, true))
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
            if (GetProblematicEnemyMonster() == null && 
                (!Bot.GetMonsters().Any(card => card.IsFaceup() && !card.IsCode(CardId.KnightmareCorruptorIblee)) || (Duel.Player == 0 && Duel.Turn == 1)))
            {
                return true;
            }
            return false;
        }

        public bool CheckShouldNoMoreSpSummon()
        {
            if (CheckAtAdvantage() && enemyActivateMaxxC && Util.IsTurn1OrMain2())
            {
                bool successFlag = false;
                successFlag |= Bot.HasInHandOrInSpellZone(CardId.DogmatikaPunishment);
                successFlag |= Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level >= 7 && card.HasRace(CardRace.SpellCaster));
                successFlag |= Bot.HasInHand(CardId.DogmatikaFleurdelis)
                    && Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika));
                return successFlag;
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
            if (lastcard.IsCode(_CardId.LockBird))
            {
                bool needToSearch = false;
                List<int> searchEffectIdList = new List<int>{ CardId.DogmatikaEcclesia, CardId.NadirServant };
                foreach (int checkId in searchEffectIdList)
                {
                    if (Bot.HasInHandOrInSpellZone(checkId) && !activatedCardIdList.Contains(checkId)) needToSearch = true;
                }
                if (discardExtraThisTurn.Contains(CardId.TitanikladTheAshDragon)) needToSearch = true;

                if (!needToSearch) return false;
            }

            return true;
        }

        public bool CheckHasExtraOnField(ClientCard exceptCard = null)
        {
            List<ClientCard> fieldMonsterList = Bot.GetMonsters();
            fieldMonsterList.AddRange(Enemy.GetMonsters());
            bool hasExtraOnField = fieldMonsterList.Any(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && card != exceptCard);
            return hasExtraOnField;
        }

        /// <summary>
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
        }

        public override int OnSelectOption(IList<int> options)
        {
            // override for iris
            List<int> checkOptionList = new List<int>{Util.GetStringId(CardId.ThesIrisSwordsoul, 4), Util.GetStringId(CardId.ThesIrisSwordsoul, 2)};
            foreach (int checkOption in checkOptionList)
            {
                for (int i = 0; i < options.Count(); ++ i)
                {
                    if (options[i] == checkOption) return i;
                }
            }
            return base.OnSelectOption(options);
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
                    if (!cardData.HasType(CardType.Ritual) || cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                } else if (cardData.HasType(CardType.Ritual))
                {
                    return CardPosition.FaceUpAttack;
                }
                int cardAttack = cardData.Attack;
                if (cardId == CardId.DogmatikaFleurdelis && !activatedCardIdList.Contains(cardId + 1) && Duel.Player == 0) cardAttack += 500;
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardAttack);
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
                    
                    // non-ritual monster
                    List<ClientCard> faceDownMonsters = botMonsters.Where(card => card.IsFacedown()).ToList();
                    banishList.AddRange(faceDownMonsters);
                    List<ClientCard> nonSynchroMonsters = botMonsters.Where(card => !card.HasType(CardType.Ritual) && !banishList.Contains(card)).ToList();
                    nonSynchroMonsters.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(nonSynchroMonsters);

                    // spells
                    List<ClientCard> spells = Bot.GetSpells();
                    banishList.AddRange(ShuffleCardList(spells));

                    // ritual monster
                    List<ClientCard> synchroMonsters = botMonsters.Where(card => card.HasType(CardType.Ritual) && !banishList.Contains(card)).ToList();
                    synchroMonsters.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(synchroMonsters);
 
                    return Util.CheckSelectCount(banishList, cards, min, max);
                }
            }
            if (maximusDiscardExtraIdList.Count() > 0 && min == 1 && max == 1 && hint == HintMsg.ToGrave)
            {
                List<ClientCard> discardList = new List<ClientCard>();
                foreach (int checkId in maximusDiscardExtraIdList)
                {
                    ClientCard discardTarget = cards.FirstOrDefault(card => card.IsCode(checkId));
                    if (discardTarget != null) discardList.Add(discardTarget);
                }
                if (discardList.Count() >= max)
                {
                    if (discardList.Count() > 0) discardExtraThisTurn.Add(discardList[0]?.Id ?? 0);
                    return Util.CheckSelectCount(discardList, cards, min, max);
                }
            }
            if (matrixActivating && hint == HintMsg.ToGrave && min == 1 && max == 1)
            {
                bool extraFlag = true;
                // 0=not yet, 1=only bot, 2=only enemy, 3=mixed & skip
                int enemyFlag = 0;
                foreach (ClientCard card in cards)
                {
                    extraFlag &= card.Location == CardLocation.Extra;
                    if (enemyFlag == 0) enemyFlag = card.Controller + 1;
                    else if (enemyFlag < 3 && enemyFlag != card.Controller + 1) enemyFlag = 3;
                }
                Logger.DebugWriteLine("===Matrix: extraFlag = " + extraFlag.ToString() + ", enemyFlag = " + enemyFlag.ToString());
                if (extraFlag && enemyFlag < 3)
                {
                    List<ClientCard> discardList = new List<ClientCard>();
                    // discard bot's extra
                    if (enemyFlag == 1)
                    {
                        ClientCard elder = null;
                        ClientCard ashDragon = null;
                        ClientCard garura = null;
                        ClientCard arcLight = null;
                        ClientCard psy = null;
                        ClientCard duskDragon = null;
                        ClientCard lilith = null;
                        foreach (ClientCard card in cards)
                        {
                            if (card.Id == CardId.ElderEntityNtss) elder = card;
                            if (card.Id == CardId.TitanikladTheAshDragon) ashDragon = card;
                            if (card.Id == CardId.GaruraWingsOfResonantLife) garura = card;
                            if (card.Id == CardId.HeraldOfTheArcLight) arcLight = card;
                            if (card.Id == CardId.PSYFramelordOmega) psy = card;
                            if (card.Id == CardId.GranguignolTheDuskDragon) duskDragon = card;
                            if (card.Id == CardId.DespianLuluwalilith) lilith = card;
                        }

                        List<ClientCard> destroyList = GetNormalEnemyTargetList(true, true, true);
                        if (elder != null && destroyList.Count() > 0) discardList.Add(elder);
                        if (ashDragon != null && !activatedCardIdList.Contains(CardId.TitanikladTheAshDragon) && !discardEnemyExtraIdList.Contains(CardId.TitanikladTheAshDragon))
                        {
                            bool checkFlag = !activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0
                                && CheckCalledbytheGrave(CardId.DogmatikaEcclesia) == 0;
                            checkFlag |= CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0 && !Bot.HasInHand(CardId.DogmatikaFleurdelis) && !enemyActivateLockBird;
                            if (checkFlag) discardList.Add(ashDragon);
                        }
                        if (garura != null && !activatedCardIdList.Contains(CardId.GaruraWingsOfResonantLife) && !enemyActivateLockBird) discardList.Add(garura);
                        if (arcLight != null && GetNeedSearchRitualCardIdList().Count() > 0) discardList.Add(arcLight);
                        if (psy != null) discardList.Add(psy);
                        if (duskDragon != null) discardList.Add(duskDragon);
                        if (lilith != null && !activatedCardIdList.Contains(CardId.DespianLuluwalilith) && !discardEnemyExtraIdList.Contains(CardId.DespianLuluwalilith))
                        {
                            discardList.Add(lilith);
                        }

                        if (discardList.Count() > 0) discardExtraThisTurn.Add(discardList[0]?.Id ?? 0);
                    }
                    // discard enemy's extra
                    if (enemyFlag == 2)
                    {
                        checkedEnemyExtra = true;
                        avoid2Monster = false;
                        confirmLink2 = false;
                        List<int> discardIfKnightmare = new List<int>{ 96380700, 48068378, 14812471, 32995276, 30342076, CardId.Linguriboh, 3679218 };
                        foreach (ClientCard card in cards)
                        {
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(card.Id);
                            if (cardData != null)
                            {
                                confirmLink2 |= cardData.HasType(CardType.Link) && cardData.Level <= 2;
                                avoid2Monster |= (cardData.HasType(CardType.Link) && cardData.Level <= 2) || cardData.HasType(CardType.Synchro | CardType.Xyz);
                                if (Enemy.HasInMonstersZone(CardId.KnightmareCorruptorIblee))
                                {
                                    if (discardIfKnightmare.Contains(card.Id)) discardList.Add(card);
                                }
                            }
                        }
                        discardList = ShuffleCardList(discardList);

                        // avoid link summon
                        foreach (ClientCard card in cards)
                        {
                            if (discardList.Contains(card)) continue;
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(card.Id);
                            if (cardData != null && Enemy.HasInMonstersZone(CardId.KnightmareCorruptorIblee)
                                && cardData.HasType(CardType.Link) && cardData.Level <= Enemy.GetMonsterCount())
                            {
                                discardList.Add(card);
                            }
                        }

                        // discard important card
                        foreach (ClientCard card in cards)
                        {
                            if (discardList.Contains(card)) continue;
                            if (discardEnemyExtraIdList.Contains(card.Id)) discardList.Add(card);
                        }

                        // discard single card first
                        List<ClientCard> singleCardList = new List<ClientCard>();
                        List<ClientCard> multiCardList = new List<ClientCard>();
                        foreach (ClientCard card in cards)
                        {
                            if (discardList.Contains(card)) continue;
                            if (cards.Any(oc => card != oc && card.IsCode(oc.Id))) multiCardList.Add(card);
                            else singleCardList.Add(card);
                        }

                        discardList.AddRange(singleCardList.OrderByDescending(c => {
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(c.Id);
                            return cardData?.Attack ?? 0;
                        }));
                        discardList.AddRange(multiCardList.OrderByDescending(c => {
                            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(c.Id);
                            return cardData?.Attack ?? 0;
                        }));
                    }

                    if (discardList.Count() > 0) return Util.CheckSelectCount(discardList, cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1)
            {
                banSpSummonFromExTurn = 0;
                checkedEnemyExtra = false;
                avoid2Monster = true;
                dimensionShifterCount = 0;

                enemySpSummonFromExLastTurn = 0;
                enemySpSummonFromExThisTurn = 0;
            }
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            omegaActivateCount = 0;
            enemySpSummonFromExLastTurn = enemySpSummonFromExThisTurn;
            enemySpSummonFromExThisTurn = 0;
            currentNegatingIdList.Clear();

            if (dimensionShifterCount > 0) dimensionShifterCount--;
            infiniteImpermanenceList.Clear();

            summoned = false;
            activatedCardIdList.Clear();
            discardExtraThisTurn.Clear();
            activatedMatrixList.Clear();
            if (Duel.Player == 1 && banSpSummonFromExTurn > 0)
            {
                banSpSummonFromExTurn -= 1;
            }
            base.OnNewTurn();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (previousControler == 1 && currentLocation == (int)CardLocation.MonsterZone)
            {
                if (previousLocation == (int)CardLocation.Deck) enemySpSummonFromDeck = true;
                if (previousLocation == (int)CardLocation.Extra)
                {
                    enemySpSummonFromExtra = true;
                    enemySpSummonFromExThisTurn ++;
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (attackers.Count() == 1 && defenders.Count() == 1)
            {
                if (defenders[0].IsCode(CardId.KnightmareCorruptorIblee) && !confirmLink2) return new BattlePhaseAction(BattlePhaseAction.BattleAction.ToMainPhaseTwo);
            }
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

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            foreach (ClientCard defender in defenders)
            {
                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();
                if (!OnPreBattleBetween(attacker, defender))
                    continue;

                if (attacker.RealPower > defender.RealPower)
                    return AI.Attack(attacker, defender);
                
                if (attacker.RealPower == defender.RealPower && defender.IsAttack() && Bot.GetMonsterCount() >= Enemy.GetMonsterCount())
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            return null;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (!activatedCardIdList.Contains(CardId.DogmatikaFleurdelis + 1) && Bot.HasInMonstersZone(CardId.DogmatikaFleurdelis, true, false, true)
                    && attacker.HasSetcode(SetcodeDogmatika)) 
                {
                    attacker.RealPower += 500;
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;

            if (player == 1)
            {
                if (card.IsCode(_CardId.InfiniteImpermanence) && !DefaultCheckWhetherCardIdIsNegated(_CardId.InfiniteImpermanence))
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
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            if (currentCard != null && !Duel.IsCurrentSolvingChainNegated() && currentCard.Controller == 1)
            {
                if (currentCard.IsCode(_CardId.MaxxC))
                    enemyActivateMaxxC = true;
                if (currentCard.IsCode(_CardId.LockBird))
                    enemyActivateLockBird = true;
                if (currentCard.IsCode(CardId.DimensionShifter))
                    dimensionShifterCount = 2;
                if (currentCard.IsCode(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == currentCard)
                        {
                            infiniteImpermanenceList.Add(4 - i);
                            break;
                        }
                    }
                }
            }
        }

        public override void OnChainEnd()
        {
            currentNegateMonsterList.Clear();
            currentDestroyCardList.Clear();
            maximusDiscardExtraIdList.Clear();
            matrixActivating = false;
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

        public bool DogmatikaAlbaZoaActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Enemy.GetMonsters().Any(card => card != null && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)) 
                && Duel.Phase == DuelPhase.Main1 && Enemy.ExtraDeck.Count() > 1)
            {
                return false;
            }
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool ThesIrisSwordsoulActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (CheckShouldNoMoreSpSummon())
                {
                    return false;
                }
                return true;
            }
            if (enemySpSummonFromDeck || enemySpSummonFromExtra) return true;

            return false;
        }
    
        public bool ClearIrisFlag()
        {
            enemySpSummonFromDeck = false;
            enemySpSummonFromExtra = false;
            
            return false;
        }

        public bool DogmatikaFleurdelisActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location == CardLocation.Hand)
            {
                bool canNegate = Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)) && Enemy.GetMonsters().Any(card => card.IsFaceup());
                if (canNegate)
                {
                    // for negate effect
                    List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true);
                    if (shouldNegateList.Count() > 0)
                    {
                        ClientCard target = shouldNegateList[0];
                        currentNegateMonsterList.Add(target);
                        AI.SelectYesNo(true);
                        AI.SelectCard(target);
                        activatedCardIdList.Add(CardId.DogmatikaFleurdelis);
                        return true;
                    }
                    // for iris
                    if (Bot.HasInHand(CardId.ThesIrisSwordsoul))
                    {
                        ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                        if (target != null)
                        {
                            AI.SelectYesNo(true);
                            AI.SelectCard(target);
                        } else {
                            List<ClientCard> enemyTargetList = ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsDisabled()).ToList());
                            if (enemyTargetList.Count() > 0)
                            {
                                AI.SelectYesNo(true);
                                AI.SelectCard(enemyTargetList);
                            } else AI.SelectYesNo(false);
                        }
                        activatedCardIdList.Add(CardId.DogmatikaFleurdelis);
                        return true;
                    }
                }
                // for total attack
                if (Duel.Player == 0 && Enemy.GetMonsterCount() == 0) {
                    int totalAttack = Util.GetTotalAttackingMonsterAttack(0);
                    if (totalAttack < Enemy.LifePoints)
                    {
                        totalAttack += Bot.GetMonsters().Where(card => card.HasSetcode(SetcodeDogmatika)).Count() * 500 + 3000;
                        if (totalAttack >= Enemy.LifePoints)
                        {
                            activatedCardIdList.Add(CardId.DogmatikaFleurdelis);
                            AI.SelectYesNo(false);
                            return true;
                        }
                    }
                }
                // for avoid lose
                if (Duel.Player == 1 && Bot.GetMonsterCount() == 0 && Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints
                    && Duel.Phase == DuelPhase.Main1 && (CurrentTiming & hintTimingMainEnd) != 0 && Duel.Turn > 1)
                {
                    activatedCardIdList.Add(CardId.DogmatikaFleurdelis);
                    List<ClientCard> enemyTargetList = ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsDisabled()).ToList());
                    if (enemyTargetList.Count() > 0)
                    {
                        AI.SelectYesNo(true);
                        AI.SelectCard(enemyTargetList);
                    }
                    else
                    {
                        AI.SelectYesNo(false);
                    }
                    return true;
                }
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool DogmatikaFleurdelisDelayActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location == CardLocation.Hand)
            {
                bool checkFlag = false;
                bool notQuickTiming = Duel.LastChainPlayer == -1 && CurrentTiming <= 0;
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.Main1 && notQuickTiming && Duel.Turn > 1)
                {
                    Logger.DebugWriteLine("=== timing: " + CurrentTiming.ToString());
                    int attack = Util.GetBestAttack(Bot);
                    List<ClientCard> currentBetterList = Enemy.MonsterZone.GetMonsters().Where(card => card.GetDefensePower() >= attack).ToList();
                    List<ClientCard> newBetterList = Enemy.MonsterZone.GetMonsters().Where(card => card.GetDefensePower() >= 3000).ToList();
                    if (currentBetterList.Count() > newBetterList.Count()) checkFlag = true;
                }
                if ((Bot.HasInHandOrInSpellZone(CardId.SinfulSpoilsOfDoom_Rciela) && Duel.Player == 0 && notQuickTiming)
                    || Bot.GetSpells().Any(card => card.IsCode(CardId.SinfulSpoilsOfDoom_Rciela) && card.IsFacedown()))
                {
                    if (!Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level >= 7 && card.HasRace(CardRace.SpellCaster))) checkFlag = true;
                }
                if (checkFlag)
                {
                    List<ClientCard> enemyTargetList = ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsDisabled()).ToList());
                    if (enemyTargetList.Count() > 0)
                    {
                        AI.SelectYesNo(true);
                        AI.SelectCard(enemyTargetList);
                    }
                    else AI.SelectYesNo(false);
                    activatedCardIdList.Add(CardId.DogmatikaFleurdelis);
                    return true;
                }
            }
            return false;
        }

        public bool DogmatikaMaximusActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (CheckShouldNoMoreSpSummon()) return false;
                // banish dump extra
                List<int> dumpIdCheck = new List<int>{ CardId.ElderEntityNtss, CardId.GaruraWingsOfResonantLife, CardId.DespianLuluwalilith };
                foreach (int dumpId in dumpIdCheck)
                {
                    IEnumerable<ClientCard> checkList = Bot.GetGraveyardMonsters().Where(card => card.IsCode(dumpId));
                    if (checkList.Count() > 1)
                    {
                        IEnumerable<ClientCard> notSummonList = checkList.Where(card => card.ProcCompleted == 0);
                        if (notSummonList.Count() > 0)
                        {
                            AI.SelectCard(notSummonList.ToList());
                            return true;
                        }
                        AI.SelectCard(checkList.ToList());
                        return true;
                    }
                }

                // find not summoned card
                List<ClientCard> notSummonedList = ShuffleCardList(Bot.Graveyard.Where(card => card != null && card.IsMonster() && card.ProcCompleted == 0
                    && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)).ToList());
                if (notSummonedList.Count() > 0)
                {
                    AI.SelectCard(notSummonedList);
                    return true;
                }

                // sort by attack
                List<ClientCard> graveTargetList = Bot.Graveyard.Where(card => card != null && card.IsMonster()
                    && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)).OrderBy(card => card.Attack).ToList();
                if (graveTargetList.Count() > 0)
                {
                    AI.SelectCard(graveTargetList);
                    return true;
                }
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherWillbeRemoved()) return false;
                List<int> decidedToDiscard = new List<int>();
                List<int> checkDiscardIdList = new List<int>{ CardId.ElderEntityNtss, CardId.HeraldOfTheArcLight, CardId.GaruraWingsOfResonantLife,
                    CardId.TitanikladTheAshDragon, CardId.GranguignolTheDuskDragon, CardId.PSYFramelordOmega, CardId.DespianLuluwalilith };
                foreach (int checkId in checkDiscardIdList)
                {
                    if (Bot.HasInExtra(checkId) && !activatedCardIdList.Contains(checkId))
                    {
                        if (checkId == CardId.ElderEntityNtss)
                        {
                            List<ClientCard> destroyList = GetNormalEnemyTargetList(true, false);
                            if (destroyList.Count() == 0) continue;
                        }
                        if (enemyActivateLockBird && (checkId == CardId.HeraldOfTheArcLight || checkId == CardId.GaruraWingsOfResonantLife))
                        {
                            continue;
                        }
                        if (checkId == CardId.HeraldOfTheArcLight)
                        {
                            if (Bot.HasInMonstersZone(CardId.DogmatikaAlbaZoa)) continue;
                            if (GetNeedSearchRitualCardIdList().Count() == 0) continue;
                        }
                        if (checkId == CardId.GaruraWingsOfResonantLife && activatedCardIdList.Contains(CardId.GaruraWingsOfResonantLife))
                        {
                            continue;
                        }
                        if (discardExtraThisTurn.Contains(checkId) && (checkId == CardId.TitanikladTheAshDragon || checkId == CardId.DespianLuluwalilith))
                        {
                            continue;
                        }
                        decidedToDiscard.Add(checkId);
                    }
                }

                maximusDiscardExtraIdList.AddRange(decidedToDiscard);
                activatedCardIdList.Add(Card.Id);
                UpdateBanSpSummonFromExTurn(1);
                return true;
            }

            return false;
        }
    
        public bool DiabellstarTheBlackWitchSpSummon()
        {
            bool hasEmptyMonsterZone = false;
            for (int i = 0; i < 5; ++ i)
            {
                if (Bot.MonsterZone[i] == null)
                {
                    hasEmptyMonsterZone = true;
                    break;
                }
            }
            if (hasEmptyMonsterZone)
            {
                if (Bot.HasInHandOrInSpellZone(CardId.WANTED_SeekerOfSinfulSpoils))
                {
                    AI.SelectCard(CardId.WANTED_SeekerOfSinfulSpoils);
                    return true;
                }
                if (activatedMatrixList.Count() > 0)
                {
                    AI.SelectCard(activatedMatrixList);
                    return true;
                }
                if (Bot.GetSpells().Where(card => card.IsCode(CardId.DogmatikaMatrix)).Count() > 1)
                {
                    AI.SelectCard(CardId.DogmatikaMatrix);
                    return true;
                }
            }
            if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) || Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)).Count() > 1)
            {
                List<int> checkIdList = new List<int>{ CardId.DogmatikaEcclesia, CardId.DogmatikaMaximus};
                foreach (int checkId in checkIdList)
                {
                    ClientCard costMonster = null;
                    if (activatedCardIdList.Contains(checkId)) costMonster = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(checkId));
                    if (costMonster == null) costMonster = Bot.GetMonsters().FirstOrDefault(card => card.IsCode(checkId) && card.IsDisabled());
                    if (costMonster != null)
                    {
                        AI.SelectCard(costMonster);
                        return true;
                    }
                }
            }
            if (hasEmptyMonsterZone)
            {
                List<int> checkIdList = new List<int> { CardId.KnightmareCorruptorIblee, CardId.ThesIrisSwordsoul };
                foreach (int checkId in checkIdList)
                {
                    if (Bot.HasInHand(checkId))
                    {
                        AI.SelectCard(checkId);
                        return true;
                    }
                }
            }
            List<ClientCard> faceDownMonsters = Bot.GetMonsters().Where(card => card.IsFacedown()).OrderBy(card => {
                YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(card.Id);
                if (cardData != null)
                {
                    return cardData.Attack;
                }
                return card.Attack;
            }).ToList();
            if (faceDownMonsters.Count() > 0)
            {
                AI.SelectCard(faceDownMonsters);
                return true;
            }
            if (hasEmptyMonsterZone)
            {
                if (Bot.HasInHand(CardId.DogmatikaFleurdelis) && Bot.GetMonsters().Where(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)).Count() == 0)
                {
                    AI.SelectCard(CardId.DogmatikaFleurdelis);
                    return true;
                }
                if (CheckRemainInDeck(CardId.DogmatikaMacabre) > 0)
                {
                    ClientCard albaZoaInHand = Bot.Hand.FirstOrDefault(card => card.IsCode(CardId.DogmatikaAlbaZoa));
                    if (albaZoaInHand != null)
                    {
                        AI.SelectCard(albaZoaInHand);
                        return true;
                    }
                }
                List<int> dumpIdList = new List<int>{ CardId.DogmatikaPunishment, _CardId.InfiniteImpermanence, _CardId.CalledByTheGrave, _CardId.AshBlossom, _CardId.MaxxC };
                foreach (int dumpId in dumpIdList)
                {
                    int checkCount = Bot.Hand.Where(card => card.IsCode(dumpId)).Count();
                    checkCount += Bot.SpellZone.Where(card => card != null && card.IsCode(dumpId)).Count();
                    if (checkCount > 1)
                    {
                        AI.SelectCard(dumpId);
                        return true;
                    }
                }
                List<ClientCard> extraCheckList = Bot.GetMonsters().Where(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Link))
                    .OrderBy(card => card.Attack).ToList();
                foreach (ClientCard checkCard in extraCheckList)
                {
                    if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) || CheckHasExtraOnField(checkCard))
                    {
                        AI.SelectCard(checkCard);
                        return true;
                    }
                }
            }
            if (Bot.GetMonsterCount() == 0 || CheckRemainInDeck(CardId.SinfulSpoilsOfDoom_Rciela) > 0)
            {
                List<int> spellIdList = new List<int>{ _CardId.CrossoutDesignator, _CardId.InfiniteImpermanence, _CardId.CalledByTheGrave, 
                    CardId.DogmatikaPunishment, CardId.DogmatikaMacabre, CardId.DogmatikaLamity };
                foreach (int spellId in spellIdList)
                {
                    if (Bot.HasInHandOrInSpellZone(spellId))
                    {
                        AI.SelectCard(spellId);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DiabellstarTheBlackWitchActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(CardId.SinfulSpoilsOfDoom_Rciela, CardId.WANTED_SeekerOfSinfulSpoils);
                SelectSTPlace();
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            return false;
        }

        public bool DogmatikaEcclesiaSummon()
        {
            if (enemyActivateLockBird) return false;
            if (CheckWhetherNegated()) return false;
            if (activatedCardIdList.Contains(Card.Id)) return false;

            summoned = true;
            return true;
        }

        public bool DogmatikaEcclesiaActivate()
        {
            if (CheckWhetherNegated()) return false;
            // sp summon
            if (Card.Location == CardLocation.Hand)
            {
                if (activatedCardIdList.Contains(Card.Id)) return false;
                if (CheckShouldNoMoreSpSummon())
                {
                    if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) || Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)))
                    {
                        return false;
                    }
                }
                if (enemyActivateLockBird)
                {
                    if (Bot.HasInHand(CardId.DogmatikaFleurdelis) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            // search
            if (Card.Location == CardLocation.MonsterZone)
            {
                if ((Duel.Player == 0 && Duel.Phase == DuelPhase.End) || (Duel.Player == 1 && Duel.Phase < DuelPhase.End))
                {
                    if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) && CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0)
                    {
                        AI.SelectCard(CardId.DogmatikaFleurdelis);
                        activatedCardIdList.Add(Card.Id);
                        UpdateBanSpSummonFromExTurn(1);
                        return true;
                    }
                }

                // for maxxc
                if (CheckAtAdvantage() && enemyActivateMaxxC)
                {
                    List<int> checkIdListFirstPart = new List<int>{ CardId.DogmatikaPunishment, CardId.DogmatikaFleurdelis };
                    if (DogmatikaMatrixCanActivate())
                    {
                        checkIdListFirstPart.Add(CardId.DogmatikaMatrix);
                    }
                    checkIdListFirstPart.AddRange(new List<int>{ CardId.DogmatikaMaximus, CardId.DogmatikaAlbaZoa, CardId.DogmatikaMacabre, CardId.DogmatikaLamity });
                    checkIdListFirstPart.Add(CardId.DogmatikaMatrix);
                    foreach (int checkId in checkIdListFirstPart)
                    {
                        if (!Bot.HasInHandOrInSpellZone(checkId) && CheckRemainInDeck(checkId) > 0)
                        {
                            AI.SelectCard(checkId);
                            activatedCardIdList.Add(Card.Id);
                            UpdateBanSpSummonFromExTurn(1);
                            return true;
                        }
                    }
                }

                // search matrix
                bool canSearchMatrix = DogmatikaMatrixCanActivate() && !activatedCardIdList.Contains(CardId.DogmatikaMatrix)
                    && CheckRemainInDeck(CardId.DogmatikaMatrix) > 0 && !Bot.HasInHand(CardId.DogmatikaMatrix);
                if (canSearchMatrix && Enemy.GetMonsterCount() > 0)
                {
                    AI.SelectCard(CardId.DogmatikaMatrix);
                    activatedCardIdList.Add(Card.Id);
                    UpdateBanSpSummonFromExTurn(1);
                    return true;
                }

                // search for ritual
                List<int> needSearchRitualIdList = GetNeedSearchRitualCardIdList();
                bool canSearchMaximus = CheckRemainInDeck(CardId.DogmatikaMaximus) > 0 && !activatedCardIdList.Contains(CardId.DogmatikaMaximus)
                    && Bot.Graveyard.Where(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)).Count() > 0;
                if (needSearchRitualIdList.Count() > 0)
                {
                    // matrix
                    if (canSearchMatrix)
                    {
                        AI.SelectCard(CardId.DogmatikaMatrix);
                        activatedCardIdList.Add(Card.Id);
                        UpdateBanSpSummonFromExTurn(1);
                        return true;
                    }
                    // maximus
                    if (canSearchMaximus && Bot.HasInExtra(CardId.HeraldOfTheArcLight))
                    {
                        AI.SelectCard(CardId.DogmatikaMaximus);
                        activatedCardIdList.Add(Card.Id);
                        UpdateBanSpSummonFromExTurn(1);
                        return true;
                    }

                }

                // search maximus
                if (canSearchMaximus)
                {
                    AI.SelectCard(CardId.DogmatikaMaximus);
                    activatedCardIdList.Add(Card.Id);
                    UpdateBanSpSummonFromExTurn(1);
                    return true;
                }

                List<int> checkIdListSecondPart = new List<int>{ CardId.DogmatikaPunishment, CardId.DogmatikaFleurdelis };
                if (DogmatikaMatrixCanActivate())
                {
                    checkIdListSecondPart.Add(CardId.DogmatikaMatrix);
                }
                checkIdListSecondPart.AddRange(new List<int>{ CardId.DogmatikaMaximus, CardId.DogmatikaAlbaZoa, CardId.DogmatikaMacabre, CardId.DogmatikaLamity });
                checkIdListSecondPart.Add(CardId.DogmatikaMatrix);
                foreach (int checkId in checkIdListSecondPart)
                {
                    if (!Bot.HasInHandOrInSpellZone(checkId) && CheckRemainInDeck(checkId) > 0)
                    {
                        AI.SelectCard(checkId);
                        activatedCardIdList.Add(Card.Id);
                        UpdateBanSpSummonFromExTurn(1);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
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
            if (CheckWhetherNegated() || Duel.LastChainPlayer == 0) return false;
            if (Enemy.HasInMonstersZone(CardId.KnightmareCorruptorIblee, true, false, true) && !confirmLink2) return false;
            return DefaultMaxxC();
        }

        public bool KnightmareCorruptorIbleeSummon()
        {
            if (banSpSummonFromExTurn > 0) return false;
            if (CheckWhetherWillbeRemoved()) return false;
            if (activatedCardIdList.Contains(CardId.KnightmareCorruptorIblee)) return false;
            if (Bot.HasInExtra(CardId.SalamangreatAlmiraj) || Bot.HasInExtra(CardId.Linguriboh))
            {
                summoned = true;
                return true;
            }

            if (Bot.HasInExtra(CardId.SPLittleKnight))
            {
                // TODO
            }

            return false;
        }

        public bool KnightmareCorruptorIbleeActivate()
        {
            if (Util.IsTurn1OrMain2()) return true;
            if (Duel.Turn > 1)
            {
                if (Bot.HasInHand(CardId.DogmatikaMatrix) && DogmatikaMatrixCanActivate())
                {
                    return true;
                }
                if (Enemy.GetMonsterCount() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    
        public bool NadirServantActivate()
        {
            if (CheckWhetherNegated() ||  CheckWhetherWillbeRemoved()) return false;
            ClientCard discardExtra = null;
            int searchId = 0;

            // search ecclesia
            if (!activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckCalledbytheGrave(CardId.DogmatikaEcclesia) == 0 && !Bot.HasInHand(CardId.DogmatikaEcclesia))
            {
                if (CheckHasExtraOnField() || !summoned)
                {
                    if (Bot.HasInGraveyard(CardId.DogmatikaEcclesia) || CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0)
                    {
                        searchId = CardId.DogmatikaEcclesia;
                        discardExtra = GetExtraToDiscard(1500, null);
                    }
                }
            }

            // search maximus
            if (searchId == 0 || discardExtra == null)
            {
                if (!activatedCardIdList.Contains(CardId.DogmatikaMaximus) && CheckCalledbytheGrave(CardId.DogmatikaMaximus) == 0
                    && Bot.HasInGraveyard(CardId.DogmatikaMaximus) || CheckRemainInDeck(CardId.DogmatikaMaximus) > 0)
                {
                    searchId = CardId.DogmatikaMaximus;
                    discardExtra = GetExtraToDiscard(1500, null);
                }
            }

            // search Fleurdelis
            if (searchId == 0 || discardExtra == null)
            {
                if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) && Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika))
                    && Bot.HasInGraveyard(CardId.DogmatikaFleurdelis) || CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0)
                {
                    searchId = CardId.DogmatikaFleurdelis;
                    discardExtra = GetExtraToDiscard(2500, null);
                }
            }

            // search ecclesia for next turn
            if (searchId == 0 || discardExtra == null)
            {
                if (Bot.HasInGraveyard(CardId.DogmatikaEcclesia) || CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0)
                {
                    searchId = CardId.DogmatikaEcclesia;
                    discardExtra = GetExtraToDiscard(1500, null);
                }
            }

            if (discardExtra != null && searchId > 0)
            {
                discardExtraThisTurn.Add(discardExtra?.Id ?? 0);
                AI.SelectCard(discardExtra);
                ClientCard targetInGY = Bot.Graveyard.FirstOrDefault(card => card != null && card.IsCode(searchId));
                if (targetInGY != null)
                {
                    AI.SelectNextCard(targetInGY);
                } else {
                    AI.SelectNextCard(searchId);
                }
                activatedCardIdList.Add(Card.Id);
                UpdateBanSpSummonFromExTurn(1);
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }

        public bool DogmatikaLamityActivate()
        {
            if (CheckWhetherNegated()) return false;
            // use lilith
            if (Bot.HasInExtra(CardId.DespianLuluwalilith))
            {
                AI.SelectYesNo(true);
                AI.SelectCard(CardId.DogmatikaAlbaZoa);
                AI.SelectNextCard(CardId.DespianLuluwalilith);
                discardExtraThisTurn.Add(CardId.DespianLuluwalilith);
                activatedCardIdList.Add(Card.Id);
                UpdateBanSpSummonFromExTurn(1);
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }

        public bool DogmatikaLamityDelayActivate()
        {
            if (CheckWhetherNegated() || Bot.HasInExtra(CardId.DespianLuluwalilith)) return false;
            if (Bot.HasInMonstersZone(CardId.DogmatikaAlbaZoa, false, false, true)) return false;
            List<ClientCard> materialList = new List<ClientCard>();
            int totalLevel = 0;
            List<ClientCard> faceDownMonsterList = Bot.GetMonsters().Where(card => !card.HasType(CardType.Xyz) && card.IsFacedown()).OrderByDescending(card => card.Level).ToList();
            foreach (ClientCard faceDownMonster in faceDownMonsterList)
            {
                materialList.Add(faceDownMonster);
                totalLevel += faceDownMonster.Level;
                if (totalLevel >= 12) break;
            }

            ClientCard handSummonTarget = Bot.Hand.FirstOrDefault(card => card.IsCode(CardId.DogmatikaAlbaZoa));
            if (handSummonTarget == null) return false;
            int extraUseCount = 0;
            List<ClientCard> extraMonsterList = Bot.GetMonsters().Where(card => !card.HasType(CardType.Xyz | CardType.Link) && card.IsFaceup())
                .OrderByDescending(card => card.Level).ToList();
            extraMonsterList.AddRange(Bot.Hand.Where(card => card.IsMonster() && card != handSummonTarget).OrderByDescending(card => card.Level).ToList());
            foreach (ClientCard faceUpMonster in extraMonsterList)
            {
                if (totalLevel >= 12 || extraUseCount >= 1) break;
                materialList.Add(faceUpMonster);
                totalLevel += faceUpMonster.Level;
                extraUseCount ++;
            }

            if (totalLevel >= 12)
            {
                AI.SelectYesNo(false);
                AI.SelectCard(CardId.DogmatikaAlbaZoa);
                AI.SelectNextCard(materialList);
                activatedCardIdList.Add(Card.Id);
                UpdateBanSpSummonFromExTurn(1);
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }

        public bool DogmatikaMacabreActivate()
        {
            if (Bot.HasInMonstersZone(CardId.DogmatikaAlbaZoa)) return false;
            if (CheckWhetherNegated()) return false;
            List<ClientCard> gyMaterialList = Bot.Graveyard.Where(card => card != null && card.HasType(CardType.Fusion | CardType.Synchro))
                .OrderByDescending(card => card.Level).ToList();
            List<ClientCard> selectMaterialList = new List<ClientCard>();
            int totalLevel = 0;
            List<int> checkDiscardThisTurnIdList = new List<int> { CardId.DespianLuluwalilith, CardId.TitanikladTheAshDragon };
            foreach (ClientCard material in gyMaterialList)
            {
                if (material.IsCode(CardId.PSYFramelordOmega)) continue;
                if (CheckAtAdvantage())
                {
                    foreach (int checkId in checkDiscardThisTurnIdList)
                    {
                        if (material.IsCode(checkId) && discardExtraThisTurn.Contains(checkId)) continue;
                    }
                }
                totalLevel += material.Level;
                selectMaterialList.Add(material);
                if (totalLevel >= 12) break;
            }
            if (totalLevel >= 12)
            {
                ClientCard graveAlbaZoa = Bot.Graveyard.FirstOrDefault(card => card.IsCode(CardId.DogmatikaAlbaZoa));
                if (graveAlbaZoa != null)
                {
                    AI.SelectCard(graveAlbaZoa);
                } else {
                    AI.SelectCard(CardId.DogmatikaAlbaZoa);
                }
                AI.SelectMaterials(selectMaterialList, HintMsg.Release);
                SelectSTPlace(null, true);
                return true;
            }

            return false;
        }
    
        public bool SinfulSpoilsOfDoom_RcielaActivate()
        {
            // select self target
            ClientCard selfTarget = null;
            bool activateFlag = false;
            List<ClientCard> selfCasterList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.Level >= 7 && card.HasRace(CardRace.SpellCaster))
                .OrderByDescending(card => card.Attack).ThenByDescending(card => card.Level).ToList();
            bool onlyAlbaZoa = selfCasterList.Count() == 1 && selfCasterList[0].IsCode(CardId.DogmatikaAlbaZoa);
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard != null && lastChainCard.Controller == 1 && lastChainCard.IsMonster())
            {
                bool negateFlag = lastChainCard.IsCode(_CardId.EffectVeiler, CardId.GhostMournerMoonlitChill);
                if (Duel.Turn > 1 || !negateFlag)
                {
                    foreach (ClientCard chainTarget in Duel.LastChainTargets)
                    {
                        if (selfCasterList.Contains(chainTarget) && (!negateFlag || !chainTarget.IsCode(CardId.DiabellstarTheBlackWitch)))
                        {
                            selfTarget = chainTarget;
                            activateFlag = true;
                            break;
                        }
                    }
                }
            }
            if (selfTarget == null && !onlyAlbaZoa)
            {
                selfTarget = selfCasterList.FirstOrDefault(card => !card.IsCode(CardId.DogmatikaAlbaZoa));
            }
            if (DefaultOnBecomeTarget() && !onlyAlbaZoa)
            {
                activateFlag = true;
            }

            if (selfTarget != null)
            {
                int targetAttack = selfTarget.Attack;
                // destroy danger monster
                ClientCard dangerCard = GetProblematicEnemyMonster(-1, true, true);
                if (dangerCard != null)
                {
                    activateFlag = true;
                }

                // destroy multi monster
                if (!onlyAlbaZoa)
                {
                    List<ClientCard> toDestroyMonsterList = Enemy.GetMonsters().Where(card => card.IsFaceup() 
                        && card.Attack > 0 && card.Attack <= targetAttack && !currentDestroyCardList.Contains(card)
                        && (Duel.Player == 1 || card != Enemy.BattlingMonster)).ToList();
                    if (toDestroyMonsterList.Count() > 1)
                    {
                        activateFlag = true;
                        currentDestroyCardList.AddRange(toDestroyMonsterList);
                    }
                }

                // decrease attack
                int botWorstPower = Util.GetWorstBotMonster()?.GetDefensePower() ?? 0;
                bool decreaseFlag = Duel.Player == 1 && Enemy.GetMonsters().Any(card => card.Attack >= botWorstPower
                    && card.IsMonsterHasPreventActivationEffectInBattle()) && Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2;
                decreaseFlag |= (!onlyAlbaZoa || (Bot.BattlingMonster?.IsCode(CardId.DogmatikaAlbaZoa) ?? false))
                    && (Bot.BattlingMonster?.GetDefensePower() ?? 0) <= (Enemy.BattlingMonster?.GetDefensePower() ?? 0)
                    && Duel.LastChainPlayer != 0 && (CurrentTiming & hintDamageStep) != 0 && CurrentTiming > 0;
                if (decreaseFlag)
                {
                    activateFlag = true;
                }
            }

            if (activateFlag)
            {
                SelectSTPlace(null, true);
                AI.SelectCard(selfTarget);
                return true;
            }

            return false;
        }
    
        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (CheckAtAdvantage() && Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0) return false;
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
                        currentNegatingIdList.Add(code);
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
                        int code = cards.GetOriginCode();
                        AI.SelectCard(cards);
                        currentNegatingIdList.Add(code);
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
                        AI.SelectCard(enemyMonsters);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = GetDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0) {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(targets);
                currentNegatingIdList.Add(code);
                return true;
            }

            return false;
        }

        public bool CrossoutDesignatorActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            // negate 
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().GetOriginCode();
                if (code == 0) return false;
                // do not negate black witch
                if (code == CardId.DiabellstarTheBlackWitch) return false;
                if (CheckCalledbytheGrave(code) > 0) return false;
                if (CheckRemainInDeck(code) > 0)
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    AI.SelectAnnounceID(code);
                    currentNegatingIdList.Add(code);
                    CheckDeactiveFlag();
                    return true;
                }
            }
            return false;
        }

        public bool WANTED_SeekerOfSinfulSpoilsActivate()
        {
            if (Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.HasPosition(CardPosition.FaceDown)))
            {
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(null, true);
                return true;
            }

            return true;
        }

        public bool DogmatikaMatrixCanActivate()
        {
            return CheckRemainInDeck(CardId.DogmatikaAlbaZoa, CardId.DogmatikaLamity, CardId.DogmatikaMacabre) > 0;
        }

        public bool DogmatikaMatrixActivate()
        {
            if (CheckWhetherNegated()) return false;

            // activate for search
            if (Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.HasPosition(CardPosition.FaceDown)))
            {
                List<int> neededRitualCardIdList = GetNeedSearchRitualCardIdList();
                // can only search ritual
                if (Enemy.GetMonsterCount() == 0)
                {
                    if (!Bot.MonsterZone.Any(card => card != null && card.IsFaceup() && card.HasType(CardType.Ritual) && card.HasSetcode(SetcodeDogmatika))
                    && neededRitualCardIdList.Count() <= 0)
                    {
                        return false;
                    }
                    SelectSTPlace(null, true);
                    AI.SelectCard(neededRitualCardIdList);
                    // in case enemy summon monster after activated
                    AI.SelectYesNo(true);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                // can search both
                else
                {
                    if (neededRitualCardIdList.Count() <= 0)
                    {
                        SelectSTPlace(null, true);
                        AI.SelectYesNo(true);
                        // search both monster and spell
                        if (CheckRemainInDeck(CardId.DogmatikaAlbaZoa) > 0 && CheckRemainInDeck(CardId.DogmatikaLamity, CardId.DogmatikaMacabre) > 0)
                        {
                            AI.SelectCard(CardId.DogmatikaAlbaZoa);
                            AI.SelectNextCard(CardId.DogmatikaLamity, CardId.DogmatikaMacabre);
                        }
                        else
                        {
                            AI.SelectCard(CardId.DogmatikaAlbaZoa, CardId.DogmatikaLamity, CardId.DogmatikaMacabre);
                            DogmatikaMatrixNextSearch();
                        }
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                    else
                    {
                        SelectSTPlace(null, true);
                        AI.SelectCard(neededRitualCardIdList);
                        AI.SelectYesNo(true);
                        DogmatikaMatrixNextSearch();
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
            }
            // discard extra
            else 
            {
                int option = 0;
                if (CheckWhetherWillbeRemoved()) option = 1;
                if (!checkedEnemyExtra && Enemy.ExtraDeck.Count() > 0) option = 1;
                if (Enemy.HasInMonstersZone(CardId.KnightmareCorruptorIblee) && avoid2Monster) option = 1;
                if (!Bot.HasInExtra(CardId.ElderEntityNtss) || GetNormalEnemyTargetList(true, false, false).Count() <= 0)
                {
                    List<int> checkActivatedIdList = new List<int>{ CardId.GaruraWingsOfResonantLife, CardId.DespianLuluwalilith,
                        CardId.TitanikladTheAshDragon, CardId.GranguignolTheDuskDragon, CardId.PSYFramelordOmega };
                    bool checkFlag = false;
                    foreach (int checkId in checkActivatedIdList)
                    {
                        checkFlag |= !discardExtraThisTurn.Contains(checkId) && !activatedCardIdList.Contains(checkId) && Bot.HasInExtra(checkId);
                    }
                    if (!checkFlag)
                    {
                        option = 1;
                    }
                }

                Logger.DebugWriteLine("===Matrix option: " + option.ToString());
                matrixActivating = true;
                AI.SelectOption(option);
                activatedMatrixList.Add(Card);
                return true;
            }
        }

        public void DogmatikaMatrixNextSearch()
        {
            List<ClientCard> fieldMonsterList = Bot.GetMonsters();
            fieldMonsterList.AddRange(Enemy.GetMonsters());
            bool hasExtraOnField = fieldMonsterList.Any(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));

            // search ecclesia
            if (!activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckCalledbytheGrave(CardId.DogmatikaEcclesia) == 0)
            {
                if (hasExtraOnField || !summoned)
                {
                    if (CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0)
                    {
                        AI.SelectNextCard(CardId.DogmatikaEcclesia);
                        return;
                    }
                }
            }

            // seach Maximus
            if (CheckRemainInDeck(CardId.DogmatikaMaximus) > 0 && !activatedCardIdList.Contains(CardId.DogmatikaMaximus)
                && Bot.Graveyard.Where(card => card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)).Count() > 0)
            {
                AI.SelectNextCard(CardId.DogmatikaMaximus);
                return;
            }

            // search Fleurdelis
            if (CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0 && !Bot.HasInHand(CardId.DogmatikaFleurdelis) && hasExtraOnField
                && Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)))
            {
                AI.SelectNextCard(CardId.DogmatikaFleurdelis);
                return;
            }
            
            List<int> searchIdList = new List<int>{ CardId.DogmatikaPunishment, CardId.DogmatikaEcclesia, CardId.DogmatikaMatrix,
                    CardId.DogmatikaMaximus, CardId.DogmatikaFleurdelis, CardId.DogmatikaAlbaZoa, CardId.DogmatikaLamity, CardId.DogmatikaMacabre };
            foreach (int searchId in searchIdList)
            {
                if (CheckRemainInDeck(searchId) > 0)
                {
                    AI.SelectNextCard(searchId);
                    return;
                }
            }
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

        public bool DogmatikaPunishmentActivate()
        {
            if (CheckWhetherNegated() || CheckWhetherWillbeRemoved()) return false;

            ClientCard targetCard = null;
            ClientCard extraToDiscard = null;

            // destroy problem card by ntss
            List<ClientCard> targetList = GetProblematicEnemyCardList(true, true);
            if (targetList.Count() > 0 && Duel.LastChainPlayer != 0 && Bot.HasInExtra(CardId.ElderEntityNtss))
            {
                foreach (ClientCard target in targetList)
                {
                    if (target.IsFaceup() && target.IsMonster() && target.Attack <= 2500)
                    {
                        targetCard = target;
                        extraToDiscard = GetExtraToDiscard(2500, target);
                        if (extraToDiscard != null)
                        {
                            break;
                        }
                    }
                }
                if (targetCard == null || extraToDiscard == null)
                {
                    List<ClientCard> enemyMonsterList = Enemy.GetMonsters().Where(card => card.IsFaceup()
                        && !card.IsShouldNotBeTarget() && card.IsShouldNotBeSpellTrapTarget()).ToList();
                    enemyMonsterList.Sort(CardContainer.CompareCardAttack);
                    foreach (ClientCard target in enemyMonsterList)
                    {
                        if (target.IsFaceup() && target.IsMonster() && target.Attack <= 2500)
                        {
                            targetCard = target;
                            extraToDiscard = GetExtraToDiscard(2500, target);
                            if (extraToDiscard != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (targetCard == null || extraToDiscard == null)
            {
                targetCard = GetProblematicEnemyMonster(0, true, true);
                if (targetCard != null)
                {
                    extraToDiscard = GetExtraToDiscard(targetCard.Attack, targetCard);
                }
            }

            if (targetCard == null || extraToDiscard == null)
            {
                bool check1 = DefaultOnBecomeTarget();
                bool check2 = Bot.UnderAttack && (Bot.BattlingMonster?.GetDefensePower() ?? 0) <= (Enemy.BattlingMonster?.GetDefensePower() ?? 0) && Duel.LastChainPlayer != 0;;
                bool check3 = Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.LastChainPlayer != 0;
                bool check4 = Duel.Player == 1 && avoid2Monster && Enemy.GetMonsterCount() >= 2 && Duel.LastChainPlayer != 0;
                Logger.DebugWriteLine("===punishment check flag: " + check1 + " " + check2 + " " + check3 + " " + check4);
                if (check1 || check2 || check3 || check4)
                {
                    List<ClientCard> checkList = Enemy.GetMonsters().Where(card => card.IsFaceup() && !card.IsShouldNotBeTarget() && !currentDestroyCardList.Contains(card))
                        .OrderByDescending(c => c.Attack).ToList();
                    foreach (ClientCard checkTarget in checkList)
                    {
                        extraToDiscard = GetExtraToDiscard(checkTarget.Attack, checkTarget);
                        if (extraToDiscard != null)
                        {
                            targetCard = checkTarget;
                            break;
                        }
                    }
                }
            }

            if (targetCard != null && extraToDiscard != null)
            {
                AI.SelectCard(targetCard);
                AI.SelectNextCard(extraToDiscard);
                currentDestroyCardList.Add(targetCard);
                discardExtraThisTurn.Add(extraToDiscard?.Id ?? 0);
                activatedCardIdList.Add(Card.Id);
                UpdateBanSpSummonFromExTurn(2);
                return true;
            }

            return false;
        }


        public bool GranguignolTheDuskDragonActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(CardId.DespianLuluwalilith, CardId.DogmatikaEcclesia, CardId.DogmatikaMaximus, CardId.DogmatikaFleurdelis);
                return true;
            }
            return false;
        }

        public bool TitanikladTheAshDragonActivate()
        {
            if (!activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0 && CheckCalledbytheGrave(CardId.DogmatikaEcclesia) == 0)
            {
                AI.SelectOption(1);
                AI.SelectCard(CardId.DogmatikaEcclesia);
                return true;
            }
            if (CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0)
            {
                if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) && !enemyActivateLockBird)
                {
                    if (Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)))
                    {
                        AI.SelectOption(0);
                        AI.SelectCard(CardId.DogmatikaFleurdelis);
                        return true;
                    }
                }
                if (Duel.Player == 1 && Enemy.GetMonsterCount() == 0)
                {
                    AI.SelectOption(1);
                    AI.SelectCard(CardId.DogmatikaFleurdelis);
                    return true;
                }
            }
            if (CheckRemainInDeck(CardId.DogmatikaMaximus) > 0)
            {
                AI.SelectOption(1);
                AI.SelectCard(CardId.DogmatikaMaximus);
                return true;
            }
            
            return false;
        }

        public bool GaruraWingsOfResonantLifeActivate()
        {
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool ElderEntityNtssActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                List<ClientCard> destroyList = GetNormalEnemyTargetList(true, true, true);
                if (destroyList.Count() > 0)
                {
                    currentDestroyCardList.Add(destroyList[0]);
                    AI.SelectCard(destroyList);
                    return true;
                }
            }

            return false;
        }

        public bool DespianLuluwalilithActivate()
        {
            // spsummon
            if (Card.Location == CardLocation.Grave)
            {
                if (!activatedCardIdList.Contains(CardId.DogmatikaEcclesia) && CheckRemainInDeck(CardId.DogmatikaEcclesia) > 0
                    && CheckCalledbytheGrave(CardId.DogmatikaEcclesia) == 0 && !enemyActivateLockBird)
                {
                    AI.SelectCard(CardId.DogmatikaEcclesia);
                    return true;
                }
                if (CheckRemainInDeck(CardId.ThesIrisSwordsoul) > 0)
                {
                    AI.SelectCard(CardId.ThesIrisSwordsoul);
                    return true;
                }
                if (Duel.Turn > 1 && Enemy.GetMonsterCount() == 0 && CheckRemainInDeck(CardId.DogmatikaFleurdelis) > 0)
                {
                    AI.SelectCard(CardId.DogmatikaFleurdelis);
                    return true;
                }
                if (Bot.HasInHand(CardId.DogmatikaFleurdelis) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.HasSetcode(SetcodeDogmatika)))
                {
                    List<int> checkIdList = new List<int>{ CardId.DogmatikaMaximus, CardId.DogmatikaEcclesia, CardId.DogmatikaFleurdelis };
                    foreach (int checkId in checkIdList)
                    {
                        if (CheckRemainInDeck(checkId) > 0)
                        {
                            AI.SelectCard(checkId);
                            return true;
                        }
                    }
                }
            }
            // increase atk & negate
            if (Card.Location == CardLocation.MonsterZone)
            {
                List<ClientCard> currentChainEnemyCard = Duel.CurrentChain.Where(card => card.Controller == 1 && !currentNegateMonsterList.Contains(card)
                    && (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone)).ToList();
                currentChainEnemyCard.AddRange(GetProblematicEnemyCardList(false, false));
                currentChainEnemyCard.AddRange(ShuffleCardList(Enemy.GetSpells().Where(card => card.IsFaceup()).ToList()));
                currentChainEnemyCard.AddRange(ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFaceup()).ToList()));
                if (currentChainEnemyCard.Count() > 0)
                {
                    currentNegateMonsterList.Add(currentChainEnemyCard[0]);
                    AI.SelectYesNo(true);
                    AI.SelectCard(currentChainEnemyCard);
                }
                else AI.SelectYesNo(false);
                return true;
            }
            return false;
        }

        public bool PSYFramelordOmegaActivate()
        {
            if (Card.Location == CardLocation.Grave && omegaActivateCount <= 5)
            {
                if (CheckWhetherNegated()) return false;
                List<ClientCard> targets = GetDangerousCardinEnemyGrave(true);
                if (targets.Count() > 0) {
                    AI.SelectCard(targets);
                    omegaActivateCount ++;
                    return true;
                }

                List<int> recycleExtraIdList = new List<int>{ CardId.GaruraWingsOfResonantLife, CardId.ElderEntityNtss };
                foreach (int checkId in recycleExtraIdList)
                {
                    if (!Bot.HasInExtra(checkId) && Bot.HasInGraveyard(checkId))
                    {
                        AI.SelectCard(checkId);
                        omegaActivateCount ++;
                        return true;
                    }
                }

                List<int> recycleMainIdList = new List<int>{ CardId.DogmatikaMacabre, CardId.DogmatikaLamity, CardId.DogmatikaAlbaZoa, CardId.DogmatikaPunishment,
                    CardId.DogmatikaEcclesia, CardId.DogmatikaFleurdelis, CardId.DogmatikaMatrix };
                foreach (int checkId in recycleMainIdList)
                {
                    if (CheckRemainInDeck(checkId) <= 0 && Bot.HasInGraveyard(checkId))
                    {
                        AI.SelectCard(checkId);
                        omegaActivateCount ++;
                        return true;
                    }
                }

                recycleExtraIdList.AddRange(new List<int> {
                    CardId.SuperStarslayerTYPHON, CardId.TitanikladTheAshDragon, CardId.HeraldOfTheArcLight, CardId.DespianLuluwalilith, CardId.Linguriboh,
                    CardId.SalamangreatAlmiraj, CardId.SecureGardna
                });
                foreach (int checkId in recycleExtraIdList)
                {
                    if (!Bot.HasInExtra(checkId) && Bot.HasInGraveyard(checkId))
                    {
                        AI.SelectCard(checkId);
                        omegaActivateCount ++;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HeraldOfTheArcLightActivate()
        {
            AI.SelectCard(GetNeedSearchRitualCardIdList());
            return true;
        }

        public bool SuperStarslayerTYPHONSpSummon()
        {
            ClientCard material = Bot.GetMonsters().Where(card => card.IsFaceup()).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (material == null || material.Attack >= 3000) return false;

            bool checkFlag = GetProblematicEnemyMonster(material.Attack) != null;
            checkFlag |= material.HasType(CardType.Link);
            checkFlag |= material.Level <= 4;
            if (checkFlag)
            {
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
                targetList.AddRange(ShuffleCardList(Enemy.GetMonsters().Where(card => card.IsFacedown() && !targetList.Contains(card)).ToList()));
                targetList.AddRange(ShuffleCardList(Bot.GetMonsters().Where(card => card.IsFacedown() && !targetList.Contains(card)).ToList()));
                targetList.AddRange(Bot.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)).OrderBy(card => card.Attack));
                AI.SelectCard(Card.Overlays);
                AI.SelectNextCard(targetList);
                return true;
            }

            return false;
        }

        public bool SPLittleKnightActivate()
        {
            // TODO
            return false;
        }

        public bool SecureGardnaSpSummon()
        {
            if (Bot.HasInHand(CardId.DogmatikaMaximus))
            {
                if (!Bot.Graveyard.Any(card => card.IsMonster() && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool LinguribohSpSummon()
        {
            if (Enemy.GetSpells().Any(card => card.IsFacedown())) return true;
            if (!Bot.HasInExtra(CardId.SalamangreatAlmiraj)) return true;
            return false;
        }

        public bool LinguribohActivate()
        {
            if (CheckLastChainShouldNegated()) return true;
            return false;
        }

        public bool SalamangreatAlmirajSpSummon()
        {
            if (Bot.HasInMonstersZone(CardId.KnightmareCorruptorIblee, faceUp: true))
            {
                AI.SelectMaterials(CardId.KnightmareCorruptorIblee);
                return true;
            }
            if (Bot.HasInHand(new List<int>{ CardId.DogmatikaEcclesia, CardId.DogmatikaMaximus, CardId.NadirServant }))
            {
                List<ClientCard> materialList = Bot.MonsterZone.Where(card => card != null && card.IsFaceup() && card.Attack <= 1000
                    && !card.HasType(CardType.Ritual | CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)).ToList();
                if (materialList.Count() > 0)
                {
                    materialList.Sort(CardContainer.CompareCardAttack);
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }

            return false;
        }

        public bool SalamangreatAlmirajActivate()
        {
            if (Card.Location == CardLocation.Grave) return true;
            bool checkFlag = Duel.Player == 1 && (!Bot.HasInHand(CardId.DogmatikaFleurdelis) || activatedCardIdList.Contains(CardId.DogmatikaFleurdelis));
            checkFlag |= DefaultOnBecomeTarget();
            checkFlag |= Bot.UnderAttack && Bot.BattlingMonster == Card;
            if (checkFlag)
            {
                AI.SelectCard(Util.GetBestBotMonster());
                return true;
            }
            if (!Util.ChainContainsCard(CardId.SinfulSpoilsOfDoom_Rciela))
            {
                List<ClientCard> checkList = Bot.GetMonsters().Where(card => card.IsFaceup() && card != Card).OrderByDescending(card => card.Attack).ToList();
                checkList.AddRange(Bot.GetMonsters().GetMatchingCards(card => card.IsFacedown()));
                foreach (ClientCard card in checkList)
                {
                    if (Util.IsChainTarget(card))
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }

            return false;
        }


        public bool SummonForTYPHONCheck()
        {
            if (!Bot.HasInExtra(CardId.SuperStarslayerTYPHON) || Bot.GetMonsters().Any(card => card.IsFaceup()) || banSpSummonFromExTurn > 0) return false;
            if (enemySpSummonFromExLastTurn < 2 && enemySpSummonFromExThisTurn < 2) return false;
            if (Card.IsCode(CardId.KnightmareCorruptorIblee) && !CheckWhetherNegated()) return false;
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

        public bool MonsterRepos()
        {
            int selfAttack = Card.Attack + 1;
            int extraAttackForDogmatika = 0;
            if (!activatedCardIdList.Contains(CardId.DogmatikaFleurdelis + 1) && Bot.HasInMonstersZone(CardId.DogmatikaFleurdelis, true, false, true)) extraAttackForDogmatika += 500;
            if (Card.HasSetcode(SetcodeDogmatika))
            {
                selfAttack += extraAttackForDogmatika;
            }

            if (Card.IsFaceup() && Card.IsDefense() && selfAttack <= 1)
                return false;

            int bestAttack = 0;
            foreach (ClientCard card in Bot.GetMonsters())
            {
                int attack = card.Attack;
                if (card.HasSetcode(SetcodeDogmatika))
                {
                    attack += extraAttackForDogmatika;
                }
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

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            if (Card.IsCode(CardId.DogmatikaPunishment) && Bot.HasInSpellZone(Card.Id)) return false;
            if (Card.IsCode(CardId.SinfulSpoilsOfDoom_Rciela))
            {
                if (!Bot.HasInHand(CardId.DogmatikaFleurdelis) && !Bot.GetMonsters().Any(card => card.IsFaceup() && card.Level >= 7 && card.HasRace(CardRace.SpellCaster)))
                {
                    return false;
                }
            }

            // select place
            if (Card.IsTrap() || Card.HasType(CardType.QuickPlay))
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