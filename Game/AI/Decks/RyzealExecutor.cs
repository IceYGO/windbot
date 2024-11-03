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
    [Deck("Ryzeal", "AI_Ryzeal")]
    public class RyzealExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int IceRyzeal = 8633261;
            public const int ThodeRyzeal = 35844557;
            public const int NodeRyzeal = 72238166;
            public const int ExRyzeal = 34022970;
            // _CardId.MulcharmyFuwalos;
            // _CardId.MulcharmyPurulia;
            // _CardId.MulcharmyNyalus;
            // _CardId.AshBlossom;
            // _CardId.GhostOgreAndSnowRabbit;
            // _CardId.MaxxC;
            // _CardId.LockBird;
            // _CardId.EffectVeiler;

            public const int SeventhTachyon = 7477101;
            public const int TripleTacticsTalent = 25311006;
            // _CardId.PotOfDesires;
            public const int Bonfire = 85106525;
            // _CardId.CalledByTheGrave;
            public const int RyzealPlugIn = 60394026;
            // _CardId.CrossoutDesignator;
            public const int RyzealCross = 6798031;

            // _CardId.InfiniteImpermanence = 10045474;

            public const int MereologicAggregator = 9940036;
            public const int RyzealDeadnader = 34909328;
            public const int Number104Masquerade = 2061963;
            public const int RyzealDuodrive = 7511613;
            public const int TwinsOfTheEclipse = 101207047;
            public const int FullArmoredUtopicRayLancer = 1269512;
            public const int TornadoDragon = 6983839;
            // _CardId.Number41BagooskatheTerriblyTiredTapir
            // _CardId.EvilswarmExcitonKnight
            public const int StarliegePhotonBlastDragon = 16643334;
            public const int AbyssDweller = 21044178;
            public const int Number60DugaresTheTimeless = 66011101;
            public const int DonnerDaggerFurHire = 8728498;
        }

        public RyzealExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, EffectVeilerActivate);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, GhostOgreAndSnowRabbitActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);

            // hand effect
            AddExecutor(ExecutorType.Activate, _CardId.LockBird, LockBirdActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyPurulia, MulcharmyPuruliaActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyNyalus, MulcharmyNyalusActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyFuwalos, MulcharmyFuwalosActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);

            // activate

            // xyz

            // summon/spsummon

            // activate

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);

        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeBranded = 0x15d;
        const int SetcodeHorus = 0x19d;
        const int hintTimingMainEnd = 0x4;
        List<int> notToNegateIdList = new List<int>{
            58699500, 20343502, 25451383, 19403423
        };
        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal, _CardId.AshBlossom, _CardId.EffectVeiler, CardId.SeventhTachyon,
                                _CardId.InfiniteImpermanence}},
            {2, new List<int> { _CardId.MulcharmyFuwalos, _CardId.GhostOgreAndSnowRabbit, _CardId.MaxxC, _CardId.PotOfDesires, _CardId.CalledByTheGrave }},
            {1, new List<int> { CardId.NodeRyzeal, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus, _CardId.LockBird, CardId.TripleTacticsTalent,
                                CardId.Bonfire, CardId.RyzealPlugIn, _CardId.CrossoutDesignator, CardId.RyzealCross}}
        };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int> {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };

        int maxSummonCount = 1;
        int summonCount = 1;
        bool enemyActivateMaxxC = false;
        bool enemyActivatePurulia = false;
        bool enemyActivateFuwalos = false;
        bool enemyActivateNyalus = false;
        bool lockBirdSolved = false;
        int dimensionShifterCount = 0;
        bool enemyActivateInfiniteImpermanenceFromHand = false;
        bool botActivateMulcharmy = false;

        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        List<int> spSummonedCardIdList = new List<int>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        List<ClientCard> hardToDestroyCardList = new List<ClientCard>();
        List<ClientCard> cannotDestroyCardList = new List<ClientCard>();

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

        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0
                    && (card.IsShouldNotBeSpellTrapTarget() && !card.IsDisabled())) return false;
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
            int sum = 0;
            foreach (int id in ids)
            {
                sum += CheckRemainInDeck(id);
            }
            return sum;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        /// <param name="isCounter">check whether card itself is disabled.</param>
        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0, bool ignoreSelf41 = false)
        {
            bool isMonster = type == 0 && Card.IsMonster();
            isMonster |= ((int)type & (int)CardType.Monster) != 0;
            bool isSpellOrTrap = type == 0 && (Card.IsSpell() || Card.IsTrap());
            isSpellOrTrap |= (((int)type & (int)CardType.Spell) != 0) || (((int)type & (int)CardType.Trap) != 0);
            bool isCounter = ((int)type & (int)CardType.Counter) != 0;
            if (isSpellOrTrap && toFieldCheck && CheckSpellWillBeNegate(isCounter))
                return true;
            if (DefaultCheckWhetherCardIsNegated(Card)) return true;
            if (isMonster && (toFieldCheck || Card.Location == CardLocation.MonsterZone))
            {
                if ((toFieldCheck && (((int)type & (int)CardType.Link) != 0)) || Card.IsDefense())
                {
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card, ignoreSelf41)) || Bot.MonsterZone.Any(card => CheckNumber41(card, ignoreSelf41))) return true;
                }
                if (Enemy.HasInSpellZone(_CardId.SkillDrain, true, true)) return true;
            }
            if (disablecheck) return (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone) && Card.IsDisabled() && Card.IsFaceup();
            return false;
        }

        public bool CheckNumber41(ClientCard card, bool ignoreSelf41 = false)
        {
            return card != null && card.IsFaceup() && card.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled()
                && (!ignoreSelf41 || card.Controller == 0);
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
            if (Enemy.HasInMonstersZone(_CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(_CardId.NaturiaBeast, true)) return true;
                if (Enemy.HasInSpellZone(_CardId.ImperialOrder, true) || Bot.HasInSpellZone(_CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(_CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(_CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap() && (Enemy.HasInSpellZone(_CardId.RoyalDecreel, true) || Bot.HasInSpellZone(_CardId.RoyalDecreel, true))) return true;
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
        /// Check whether last chain card should be disabled.
        /// </summary>
        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(lastcard.Id)) return false;
            if (lastcard.HasSetcode(_Setcode.Danger) && lastcard.Location == CardLocation.Hand) return false;
            if (lastcard.IsMonster() && lastcard.Location == CardLocation.MonsterZone && lastcard.HasPosition(CardPosition.Defence))
            {
                if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card))) return false;
            }
            if (DefaultCheckWhetherCardIsNegated(lastcard)) return false;

            return true;
        }

        /// <summary>
        /// Check whether bot is at advantage.
        /// </summary>
        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && (Duel.Player == 0 || Bot.GetMonsterCount() > 0)) return true;
            return false;
        }

        public bool CheckShouldNoMoreSpSummon()
        {
            if (CheckAtAdvantage() && enemyActivateMaxxC && !lockBirdSolved && (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2))
            {
                return true;
            }
            return false;
        }

        public bool CheckShouldNoMoreSpSummon(CardLocation loc)
        {
            if (CheckShouldNoMoreSpSummon()) return true;
            if (lockBirdSolved || (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2)) return false;
            if (enemyActivatePurulia && (loc & CardLocation.Hand) != 0) return true;
            if (enemyActivateFuwalos && (loc & (CardLocation.Deck | CardLocation.Extra)) != 0) return true;
            if (enemyActivateNyalus && (loc & (CardLocation.Grave | CardLocation.Removed)) != 0) return true;

            return false;
        }

        public bool CheckWhetherCanSummon()
        {
            return Duel.Player == 0 && Duel.Phase < DuelPhase.End && summonCount > 0;
        }

        /// <summary>
        /// Check whether cards will be removed. If so, do not send cards to grave.
        /// </summary>
        public bool CheckWhetherWillbeRemoved()
        {
            if (dimensionShifterCount > 0) return true;
            List<int> checkIdList = new List<int> { _CardId.BanisheroftheRadiance, _CardId.BanisheroftheLight, _CardId.MacroCosmos, _CardId.DimensionalFissure,
                _CardId.KashtiraAriseHeart, _CardId.MaskedHERODarkLaw };
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

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            ClientCard floodagateCard = Enemy.GetMonsters().Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsFloodgate() && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (floodagateCard != null) return floodagateCard;

            ClientCard dangerCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (dangerCard != null) return dangerCard;

            ClientCard invincibleCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (invincibleCard != null) return invincibleCard;

            ClientCard equippedCard = Enemy.MonsterZone.Where(c => c?.Data != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && c.EquipCards.Count > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (equippedCard != null) return equippedCard;

            ClientCard enemyExtraMonster = Enemy.MonsterZone.Where(c => c != null && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))
                && (c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz) || (c.HasType(CardType.Link) && c.LinkCount >= 2))
                && CheckCanBeTargeted(c, canBeTarget, selfType) && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (enemyExtraMonster != null) return enemyExtraMonster;

            ClientCard activatingAlbaz = Enemy.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(68468459) && !c.IsDisabled()
                && !currentDestroyCardList.Contains(c) && !currentNegateCardList.Contains(c) && Duel.CurrentChain.Contains(c));
            if (activatingAlbaz != null) return activatingAlbaz;

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = Util.GetBestAttack(Bot);
                ClientCard betterCard = Enemy.MonsterZone.Where(card => card != null
                    && card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && (ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).OrderByDescending(card => card.Attack).FirstOrDefault();
                if (betterCard != null) return betterCard;
            }
            return null;
        }

        public bool CheckShouldNotIgnore(ClientCard cards, bool ignore = false)
        {
            return !ignore || (!currentDestroyCardList.Contains(cards) && !currentNegateCardList.Contains(cards));
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

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count > 0) resultList.AddRange(floodagateList);

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count > 0) resultList.AddRange(ShuffleList(problemEnemySpellList));

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count > 0)
            {
                foreach (ClientCard target in enemyMonsters)
                {
                    if ((target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
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
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
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
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card)).ToList()));
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
                    && !currentNegateCardList.Contains(card));
            if (target != null)
            {
                resultList.Add(target);
            }

            // negate monster effect on the field
            foreach (ClientCard chainingCard in Duel.CurrentChain)
            {
                if (chainingCard.Location == CardLocation.MonsterZone && chainingCard.Controller == 1 && !chainingCard.IsDisabled()
                && CheckCanBeTargeted(chainingCard, canBeTarget, selfType) && !currentNegateCardList.Contains(chainingCard))
                {
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if (attackers.Count() > 0 && defenders.Count() > 0)
            {
                List<ClientCard> sortedAttacker = attackers.OrderBy(card => card.Attack).ToList();
                ClientCard rayLancer = attackers.FirstOrDefault(c => c.IsCode(CardId.FullArmoredUtopicRayLancer) && !c.IsDisabled());
                if (rayLancer != null)
                {
                    sortedAttacker.Remove(rayLancer);
                    sortedAttacker.Insert(0, rayLancer);
                }
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
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            return true;
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
                    List<ClientCard> dumpMainMonsterList = botMonsters.Where(card => !banishList.Contains(card)
                        && CheckRemainInDeck(card.Id) > 0).ToList();
                    dumpMainMonsterList.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(dumpMainMonsterList);

                    // spells
                    List<ClientCard> faceUpSpells = Bot.GetSpells().Where(c => c.IsFaceup()).ToList();
                    banishList.AddRange(ShuffleList(faceUpSpells));
                    List<ClientCard> faceDownSpells = Bot.GetSpells().Where(c => c.IsFacedown()).ToList();
                    banishList.AddRange(ShuffleList(faceDownSpells));

                    List<ClientCard> uniqueMainMonster = botMonsters.Where(card => !banishList.Contains(card)
                        && !card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && CheckRemainInDeck(card.Id) == 0).ToList();
                    uniqueMainMonster.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(uniqueMainMonster);

                    List<ClientCard> dumpExtraMonsterList = botMonsters.Where(card => !banishList.Contains(card)
                        && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && Bot.HasInExtra(card.Id)).ToList();
                    dumpExtraMonsterList.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(dumpExtraMonsterList);

                    List<ClientCard> uniqueExtraMonsterList = botMonsters.Where(card => !banishList.Contains(card)
                        && card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link) && !Bot.HasInExtra(card.Id)).ToList();
                    uniqueExtraMonsterList.Sort(CardContainer.CompareCardAttack);
                    banishList.AddRange(uniqueExtraMonsterList);

                    return Util.CheckSelectCount(banishList, cards, min, max);
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectOption(IList<int> options)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null)
            {
                // TODO
            }

            return base.OnSelectOption(options);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                List<int> zoneIdList = ShuffleList(new List<int> { 5, 6 });
                zoneIdList.AddRange(ShuffleList(new List<int> { 0, 2, 4 }));
                zoneIdList.AddRange(ShuffleList(new List<int> { 1, 3 }));
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

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.RyzealCross, 3))
            {
                // TODO whether to negate
            }

            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == _CardId.Number41BagooskatheTerriblyTiredTapir && Util.IsTurn1OrMain2())
            {
                return CardPosition.FaceUpDefence;
            }

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

        public override void OnNewTurn()
        {
            if (Duel.Turn <= 1)
            {
                dimensionShifterCount = 0;
                // TODO for doom bot
                maxSummonCount = 1;
                hardToDestroyCardList.Clear();
                cannotDestroyCardList.Clear();
            }

            summonCount = maxSummonCount;
            enemyActivateMaxxC = false;
            enemyActivatePurulia = false;
            enemyActivateFuwalos = false;
            enemyActivateNyalus = false;
            lockBirdSolved = false;
            if (dimensionShifterCount > 0) dimensionShifterCount--;
            enemyActivateInfiniteImpermanenceFromHand = false;
            botActivateMulcharmy = false;
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            activatedCardIdList.Clear();
            spSummonedCardIdList.Clear();
            enemyPlaceThisTurn.Clear();
            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            Duel.LastChainTargets.Clear();
            if (card == null) return;
            
            if (player == 1)
            {
                if (card.IsCode(_CardId.InfiniteImpermanence))
                {
                    if (enemyActivateInfiniteImpermanenceFromHand)
                    {
                        enemyActivateInfiniteImpermanenceFromHand = false;
                    }
                    else
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Enemy.SpellZone[i] == card)
                            {
                                infiniteImpermanenceList.Add(4 - i);
                                break;
                            }
                        }
                    }
                }
            }

            if (player == 0)
            {
                if (card.IsCode(_CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus))
                {
                    botActivateMulcharmy = true;
                }
            }
            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            if (currentCard != null)
            {
                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentCard.IsCode(_CardId.LockBird))
                        lockBirdSolved = true;
                    if (currentCard.IsCode(_CardId.DimensionShifter))
                        dimensionShifterCount = 2;
                    if (currentCard.Controller == 1)
                    {
                        if (currentCard.IsCode(_CardId.MaxxC))
                            enemyActivateMaxxC = true;
                        if (currentCard.IsCode(_CardId.MulcharmyPurulia))
                            enemyActivatePurulia = true;
                        if (currentCard.IsCode(_CardId.MulcharmyFuwalos))
                            enemyActivateFuwalos = true;
                        if (currentCard.IsCode(_CardId.MulcharmyNyalus))
                            enemyActivateNyalus = true;
                    }
                }
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            for (int idx = cannotDestroyCardList.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = cannotDestroyCardList[idx];
                if (checkTarget == null || !checkTarget.IsOnField())
                {
                    cannotDestroyCardList.RemoveAt(idx);
                }
            }
            for (int idx = hardToDestroyCardList.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = hardToDestroyCardList[idx];
                if (checkTarget == null || !checkTarget.IsOnField())
                {
                    hardToDestroyCardList.RemoveAt(idx);
                }
            }
            foreach (ClientCard card in currentDestroyCardList)
            {
                if (card != null && card.IsOnField())
                {
                    if (hardToDestroyCardList.Contains(card))
                    {
                        cannotDestroyCardList.Add(card);
                    } else
                    {
                        hardToDestroyCardList.Add(card);
                    }
                }
            }

            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            for (int idx = enemyPlaceThisTurn.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = enemyPlaceThisTurn[idx];
                if (checkTarget == null || !checkTarget.IsOnField())
                {
                    enemyPlaceThisTurn.RemoveAt(idx);
                }
            }
            base.OnChainEnd();
        }

        public override void OnMove(ClientCard card, int previousControler, int previousLocation, int currentControler, int currentLocation)
        {
            if (previousControler == 1)
            {
                if (card != null)
                {
                    if (card.IsCode(_CardId.InfiniteImpermanence) && previousLocation == (int)CardLocation.Hand && currentLocation == (int)CardLocation.SpellZone)
                        enemyActivateInfiniteImpermanenceFromHand = true;
                }
            }
            if (card != null)
            {
                if (currentControler == 1 && (currentLocation == (int)CardLocation.MonsterZone || currentLocation == (int)CardLocation.SpellZone))
                {
                    enemyPlaceThisTurn.Add(card);
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
        }

        public override void OnSpSummoned()
        {
            // not special summoned by chain
            if (Duel.GetCurrentSolvingChainCard() == null)
            {
                foreach (ClientCard card in Duel.LastSummonedCards)
                {
                    if (card.Controller == 0 && card.IsCode(CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.NodeRyzeal, CardId.ExRyzeal))
                    {
                        spSummonedCardIdList.Add(card.GetOriginCode());
                    }
                }
            }
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

        public bool IceRyzealSpSummon()
        {
            // TODO
            return false;
        }

        public bool IceRyzealSummon()
        {
            // TODO
            return false;
        }

        public bool IceRyzealActivate()
        {
            // TODO
            return false;
        }

        public bool ThodeRyzealSpSummon()
        {
            // TODO
            return false;
        }

        public bool ThodeRyzealSummon()
        {
            // TODO
            return false;
        }

        public bool ThodeRyzealActivate()
        {
            // TODO
            return false;
        }

        public bool NodeRyzealSpSummon()
        {
            // TODO
            return false;
        }

        public bool NodeRyzealSummon()
        {
            // TODO
            return false;
        }

        public bool NodeRyzealActivate()
        {
            // TODO
            return false;
        }

        public bool ExRyzealSpSummon()
        {
            // TODO
            return false;
        }

        public bool ExRyzealSummon()
        {
            // TODO
            return false;
        }

        public bool ExRyzealActivate()
        {
            // TODO
            return false;
        }

        public bool MulcharmyFuwalosActivate()
        {
            // TODO
            return false;
        }

        public bool MulcharmyPuruliaActivate()
        {
            // TODO
            return false;
        }

        public bool MulcharmyNyalusActivate()
        {
            // TODO
            return false;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (DefaultAshBlossomAndJoyousSpring())
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard.Location == CardLocation.MonsterZone || lastChainCard.Location == CardLocation.SpellZone) currentNegateCardList.Add(Util.GetLastChainCard());
                return true;
            }
            return false;
        }

        public bool GhostOgreAndSnowRabbitActivate()
        {
            // TODO
            return false;
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0 || lockBirdSolved) return false;
            return DefaultMaxxC();
        }

        public bool LockBirdActivate()
        {
            // TODO
            return false;
        }

        public bool EffectVeilerActivate()
        {
            // TODO
            return false;
        }

        public bool SeventhTachyonActivate()
        {
            // TODO
            return false;
        }

        public bool TripleTacticsTalentActivate()
        {
            // TODO
            return false;
        }

        public bool PotOfDesiresActivate()
        {
            // TODO
            return false;
        }

        public bool BonfireActivate()
        {
            // TODO
            return false;
        }

        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (CheckAtAdvantage() && Duel.LastChainPlayer == 1 && lastChainCard != null && lastChainCard.IsCode(_CardId.MaxxC))
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (lastChainCard != null && lastChainCard.IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
                    {
                        return false;
                    }

                    // not to negate same card in hand
                    if (Duel.Player == 0 && Bot.HasInHand(code)) return false;

                    ClientCard graveTarget = Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.GetOriginCode() == code);
                    if (graveTarget != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(graveTarget);
                        currentDestroyCardList.Add(graveTarget);
                        return true;
                    }
                }

                // banish target
                foreach (ClientCard graveCard in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(graveCard) && graveCard.IsMonster())
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        int code = graveCard.Id;
                        AI.SelectCard(graveCard);
                        currentDestroyCardList.Add(graveCard);
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemyMonsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemyMonsters.Count > 0)
                    {
                        enemyMonsters.Sort(CardContainer.CompareCardAttack);
                        enemyMonsters.Reverse();
                        int code = enemyMonsters[0].Id;
                        AI.SelectCard(code);
                        currentDestroyCardList.Add(enemyMonsters[0]);
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = GetDangerousCardinEnemyGrave(true);
            if (targets.Count > 0)
            {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                currentDestroyCardList.Add(targets[0]);
                return true;
            }

            return false;
        }

        public bool RyzealPlugInActivate()
        {
            // TODO
            return false;
        }

        public bool RyzealCrossActivate()
        {
            // TODO
            return false;
        }

        public bool CrossoutDesignatorActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            // negate 
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().Id;
                int alias = Util.GetLastChainCard().Alias;
                if (alias != 0 && alias - code < 10) code = alias;
                if (code == 0) return false;
                if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                if (CheckRemainInDeck(code) > 0)
                {
                    if (!(Card.Location == CardLocation.SpellZone))
                    {
                        SelectSTPlace(null, true);
                    }
                    AI.SelectAnnounceID(code);
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
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || Util.IsChainTarget(Card)
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    if (target != null)
                    {
                        AI.SelectCard(target);
                    }
                    else
                    {
                        AI.SelectCard(Enemy.GetMonsters());
                    }
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }

            // negate monster
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true, CardType.Trap);
            if (shouldNegateList.Count > 0)
            {
                ClientCard negateTarget = shouldNegateList[0];
                currentNegateCardList.Add(negateTarget);

                if (Card.Location == CardLocation.SpellZone)
                {
                    for (int i = 0; i < 5; ++i)
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

        // TODO extra

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;

            // select place
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int setForInfiniteImpermanence = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        setForInfiniteImpermanence += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(_CardId.InfiniteImpermanence))
                {
                    if (Card.IsCode(_CardId.InfiniteImpermanence))
                    {
                        AI.SelectPlace(setForInfiniteImpermanence);
                        return true;
                    }
                    else
                    {
                        SelectSTPlace(Card, false, avoid_list);
                        return true;
                    }
                }
                else
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

        protected override bool DefaultSetForDiabellze()
        {
            if (base.DefaultSetForDiabellze())
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }
    }
}