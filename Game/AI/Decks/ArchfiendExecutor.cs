using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Archfiend", "AI_Archfiend")]
    public class ArchfiendExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int SMSkull = 70781052;
            public const int RegenArch = 95718355;
            public const int RegenSage = 22938501;
            public const int Origin = 79621896;
            public const int Royal = 58769832;
            public const int PMBeryl = 63198739;
            public const int Highness = 11248645;
            public const int Makourai = 15725501;
            public const int Strategy = 90764871;
            public const int Usurpation = 82997779;
            public const int PMLL = 56506740;
            public const int PMDR = 29095457;
            public const int Simul = 94423983;
            public const int Playtime = 87985506;


            //Black list
            public const int Lancea = 34267821;
            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
            public const int SkillDrain = 82732705;
            public const int DivineArsenalAAZEUS_SkyThunder = 90448279;
            public const int DimensionShifter = 91800273;
            public const int MacroCosmos = 30241314;
            public const int DimensionalFissure = 81674782;
            public const int BanisheroftheRadiance = 94853057;
            public const int BanisheroftheLight = 61528025;
            public const int KashtiraAriseHeart = 48626373;
            public const int GhostMournerMoonlitChill = 52038441;
            public const int NibiruThePrimalBeing = 27204311;
            public const int AmeNoMurakumoNoMitsurugi = 19899073;
        }

        //Setcode
        const int SetcodeArchfiend = 0x45; 
        const int SetcodeMaliss = 0x1bf;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;
        const int SetcodeDarkWorld = 0x6;
        const int SetcodeSkyStriker = 0x115;
        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>
        {
            { 3, new List<int>
                {
                    CardId.RegenArch,
                    CardId.Origin,
                    CardId.Royal,
                    CardId.Highness,
                    CardId.PMBeryl,
                    CardId.Makourai,
                    CardId.Strategy,
                    CardId.Usurpation,
                    CardId.PMLL,
                }
            },

            { 2, new List<int>
                {
                    _CardId.AshBlossom,
                    CardId.SMSkull,
                    _CardId.PotOfExtravagance,
                    _CardId.InfiniteImpermanence
                }
            },

            { 1, new List<int>
                {
                    _CardId.MaxxC,
                    //_CardId.CalledByTheGrave,
                    CardId.PMDR,
                    CardId.Simul,
                    CardId.RegenSage,
                    CardId.Playtime

                }
            },
        };

        private static readonly int[] PreferDiscard =
        {
            CardId.Playtime,
            CardId.Highness,
            CardId.Makourai,
            CardId.Usurpation,
            CardId.Royal,
            CardId.Strategy,
            CardId.RegenArch,
            CardId.Origin,
            CardId.SMSkull
        };

        //Flags
        int myTurnCount = 0;
        int summonCount = 1;
        const int hintTimingMainEnd = 0x4;
        const int hintToHand = 0x200000;
        bool activatingLodeSpSummonEffect = false;


        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();

        public ArchfiendExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.PotOfExtravagance, PotOfExtravaganceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            //AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.PMDR, PrimiteDrillbeamActivate);
            AddExecutor(ExecutorType.Activate, CardId.RegenSage);
            
            AddExecutor(ExecutorType.Activate, CardId.Usurpation, Usurpation);
            AddExecutor(ExecutorType.Activate, CardId.Strategy, StrategyActivate);
            AddExecutor(ExecutorType.Activate, CardId.Playtime, Playtime);
            AddExecutor(ExecutorType.Activate, CardId.Royal, RoyalActivate);
            AddExecutor(ExecutorType.Activate, CardId.Highness, HighnessActivate);
            AddExecutor(ExecutorType.Summon, Level4MonsterSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.RegenArch);
            AddExecutor(ExecutorType.Activate, CardId.RegenArch);
            AddExecutor(ExecutorType.SpSummon, CardId.RegenSage);
            AddExecutor(ExecutorType.Activate, CardId.Origin, OriginActivate);
            AddExecutor(ExecutorType.Activate, CardId.Makourai, MakouraiActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.Origin, OriginSpSummon);

            AddExecutor(ExecutorType.Activate, CardId.Simul, Simul);
            AddExecutor(ExecutorType.SpSummon, CardId.RegenArch);
            AddExecutor(ExecutorType.SpSummon, CardId.RegenSage); 
            AddExecutor(ExecutorType.Activate, CardId.PMLL, PrimiteLordlyLodeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PMBeryl, PrimiteDragonEtherBerylActivate);
            AddExecutor(ExecutorType.Activate, CardId.PMLL, PrimiteLordlyLodeSpSummon);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        #region Default
        public override void OnNewTurn()
        {
            if (Duel.Player == 0)
            {
                myTurnCount++;
            }
            // reset
            summonCount = 1;
            activatingLodeSpSummonEffect = false;
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyPlaceThisTurn.Clear();
            activatedCardIdList.Clear();

            base.OnNewTurn();
        }
        public override bool OnSelectHand() { return true; /* Go first by default.*/}
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.AmeNoMurakumoNoMitsurugi, 3))
            {
                bool shouldDiscard = Bot.Hand.Count >= 2;

                Logger.DebugWriteLine($"[MURAKUMO] Archfiend choose discard={shouldDiscard}, " + $"hand={Bot.Hand.Count}");
                return shouldDiscard;
            }

            return base.OnSelectYesNo(desc);
        }

        public int CheckRemainInDeck(int id)
        {
            for (int count = 1; count < 4; ++count)
            {
                if (DeckCountTable[count].Contains(id))
                {
                    return Bot.GetRemainingCount(id, count);
                }
            }
            return 0;
        }
        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && Bot.GetMonsters().Any(card => card.IsFaceup()))
            {
                return true;
            }
            return false;
        }
        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                if (CheckAtAdvantage() && Duel.Turn > 1)
                {
                    return false;
                }
            }
            return DefaultAshBlossomAndJoyousSpring();
        }
        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }
        public bool CrossoutDesignatorActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                int code = Util.GetLastChainCard().Id;
                int alias = Util.GetLastChainCard().Alias;
                ClientCard last = Util.GetLastChainCard();
                if (last.IsMonster() && (last.HasType(CardType.Fusion) || last.HasType(CardType.Synchro) || last.HasType(CardType.Xyz) || last.HasType(CardType.Link)))
                {
                    return false;
                }
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
                    currentNegateCardList.AddRange(Enemy.MonsterZone.Where(c => c != null && c.IsFaceup() && c.IsCode(code)));
                    return true;
                }
            }
            return false;
        }
        public bool InfiniteImpermanenceActivate()
        {
            if (CheckWhetherNegated()) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
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
                    AI.SelectCard(m);
                    return true;
                }
            }
            ClientCard LastChainCard = Util.GetLastChainCard();
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
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                    AI.SelectCard(target);
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;

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
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard card in enemyMonsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }
        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated())
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage() && Duel.Turn > 1)
                    {
                        return false;
                    }
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
        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int> { };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }
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
            return false;
        }
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantom) || card.HasSetcode(SetcodeHorus) || card.HasSetcode(SetcodeDarkWorld) || card.HasSetcode(SetcodeSkyStriker))).ToList();
            List<int> dangerMonsterIdList = new List<int> { 99937011, 63542003, 9411399, 28954097, 30680659, 32731036 };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }
        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0)
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
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card)) || Bot.MonsterZone.Any(card => CheckNumber41(card))) return true;
                }
                if (Enemy.HasInSpellZone(CardId.SkillDrain, true)) return true;
            }
            if (disablecheck) return (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone) && Card.IsDisabled() && Card.IsFaceup();
            return false;
        }
        public bool CheckNumber41(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled();
        }
        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    //if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            list = ShuffleList(list);
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
        public bool CheckSpellWillBeNegate(bool isCounter = false, ClientCard target = null)
        {
            if (target == null) target = Card;
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

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
            return false;
        }
        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(lastcard.Id)) return false;

            return CheckCardShouldNegate(lastcard);
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
        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0 && (card.IsShouldNotBeSpellTrapTarget() && !card.IsDisabled())) return false;
            }
            return true;
        }
        public bool CheckShouldNotIgnore(ClientCard cards)
        {
            return !currentDestroyCardList.Contains(cards) && !currentNegateCardList.Contains(cards);
        }
        public List<T> ShuffleList<T>(List<T> list) => Util.ShuffleList(list);
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
            if (dangerList.Count > 0 && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

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
                        && !resultList.Contains(target) && CheckCanBeTargeted(target, canBeTarget, selfType))
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
        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0, bool forNegate = false)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card)).ToList()));
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

            ClientCard target = Enemy.MonsterZone.FirstOrDefault(card => card?.Data != null
                    && card.IsMonsterShouldBeDisabledBeforeItUseEffect() && card.IsFaceup() && !card.IsShouldNotBeTarget()
                    && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && !currentNegateCardList.Contains(card));
            if (target != null)
            {
                resultList.Add(target);
            }

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
        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null) return card;
            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null) return card;
            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count > 0 && !onlyFaceup) return ShuffleCardList(monsters)[0];
            return null;
        }
        public ClientCard GetBestEnemySpell(bool onlyFaceup = false, bool canBeTarget = false)
        {
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(card => !(card.IsFaceup() && card.IsCode(_CardId.EvenlyMatched))).ToList();

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() && (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field) || ecard.HasType(CardType.Pendulum))).ToList();
            if (faceUpList.Count > 0)
            {
                return ShuffleCardList(faceUpList)[0];
            }

            if (spells.Count > 0 && !onlyFaceup)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }
        public List<ClientCard> ShuffleCardList(List<ClientCard> list) => Util.ShuffleCardList(list);
        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false, bool checkGrave = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null) return card;

            card = GetBestEnemySpell(onlyFaceup, canBeTarget);
            if (card != null) return card;

            if (checkGrave && Enemy.Graveyard.Count > 0)
            {
                List<ClientCard> graveMonsterList = Enemy.Graveyard.GetMatchingCards(c => c.IsMonster()).ToList();
                if (graveMonsterList.Count > 0)
                {
                    graveMonsterList.Sort(CardContainer.CompareCardAttack);
                    graveMonsterList.Reverse();
                    return graveMonsterList[0];
                }
                return ShuffleCardList(Enemy.Graveyard.ToList())[0];
            }
            return null;
        }
        public bool CheckCardShouldNegate(ClientCard card)
        {
            if (card == null) return false;
            if (card.IsMonster() && card.HasSetcode(_Setcode.TimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(card.Id)) return false;
            if (card.HasSetcode(_Setcode.Danger) && card.Location == CardLocation.Hand) return false;
            if (card.IsMonster() && card.Location == CardLocation.MonsterZone && card.HasPosition(CardPosition.Defence))
            {
                if (Enemy.MonsterZone.Any(c => CheckNumber41(c)) || Bot.MonsterZone.Any(c => CheckNumber41(c))) return false;
            }
            if (DefaultCheckWhetherCardIsNegated(card)) return false;
            if (card.Location == CardLocation.SpellZone)
            {
                int sequence = card.Sequence;
                if (card.Controller == 1) sequence = 4 - sequence;
                if (infiniteImpermanenceNegatedColumns.Contains(sequence)) return false;
            }
            if (card.IsCode(_CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus, _CardId.MaxxC)) return false;
            if (card.IsDisabled()) return false;

            return true;
        }

        public bool PotOfExtravaganceActivate()
        {
            if (CheckWhetherNegated()) return false;
            SelectSTPlace(Card, true);
            AI.SelectOption(1);
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain == null)
            {
                // for activating target
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.Controller == 0)
                {
                    switch (lastChainCard.Id)
                    {
                        case CardId.PMDR:
                            {
                                // negate cards on chain
                                foreach (ClientCard card in Duel.CurrentChain)
                                {
                                    if (card.Controller == 1 && card.IsOnField() && card.IsFaceup() && !card.IsDisabled() && !currentNegateCardList.Contains(card)
                                        && CheckCanBeTargeted(card, true, CardType.Spell) && CheckCardShouldNegate(card) && cards.Contains(card))
                                    {
                                        currentNegateCardList.Add(card);
                                        return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                                    }
                                }
                                // remove problematic enemy cards
                                List<ClientCard> targetList = GetNormalEnemyTargetList(canBeTarget: true, ignoreCurrentDestroy: true, selfType: CardType.Monster, forNegate: false);
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                                // select enemy cards
                                List<ClientCard> enemyCards = ShuffleList(cards.Where(c => c.Controller == 1).ToList());
                                foreach (ClientCard target in enemyCards)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                                break;
                            }
                        case _CardId.InfiniteImpermanence:
                            {
                                int sequence = lastChainCard.Sequence;
                                List<ClientCard> targetList = GetMonsterListForTargetNegate(true, CardType.Trap);
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        if (sequence >= 0)
                                        {
                                            ClientCard spell = Enemy.SpellZone[sequence];
                                            if (spell != null && spell.IsFaceup())
                                            {
                                                currentNegateCardList.Add(spell);
                                            }
                                        }
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                                targetList = GetProblematicEnemyCardList(canBeTarget: true, ignoreSpells: false, selfType: CardType.Trap);
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        if (sequence >= 0)
                                        {
                                            ClientCard spell = Enemy.SpellZone[sequence];
                                            if (spell != null && spell.IsFaceup())
                                            {
                                                currentNegateCardList.Add(spell);
                                            }
                                        }
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                                break;
                            }
                        
                    }
                }
            }

            if (currentChain != null)
            {
                if (currentChain.ActivateController == 0)
                {
                    // for solving chain
                    switch (currentChain.ActivateId)
                    {
                        case CardId.PMBeryl:
                            if (hint == HintMsg.Set)
                            {
                                int targetId = CardId.PMLL;
                                if (activatedCardIdList.Contains(CardId.PMLL) || !DefaultCheckWhetherBotCanSearch() || Bot.HasInSpellZone(CardId.PMLL))
                                {
                                    targetId = CardId.PMDR;
                                }
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            break;
                        
                        case CardId.PMLL:
                            {
                                if (hint == HintMsg.AddToHand)
                                {
                                    List<int> targetIdList = new List<int>();
                                    if (ShouldPMLLSearchDrillbeam())
                                    {
                                        targetIdList.Add(CardId.PMDR);
                                        targetIdList.Add(CardId.PMBeryl);
                                    }
                                    else
                                    {
                                        targetIdList.Add(CardId.PMBeryl);
                                        targetIdList.Add(CardId.PMDR);
                                    }

                                    foreach (int targetId in targetIdList)
                                    {
                                        if (CheckRemainInDeck(targetId) <= 0) continue;

                                        ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                        if (target != null)
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                        }
                                    }
                                }
                                break;
                            }
                        case CardId.Strategy:
                            {
                                if (hint == HintMsg.Remove)
                                {
                                    ClientCard cost = cards
                                        .Where(c => c != null && c.Controller == 0)
                                        .OrderBy(c => GetArchfiendCostPriority(c))
                                        .FirstOrDefault();

                                    if (cost != null)
                                        return Util.CheckSelectCount(new List<ClientCard> { cost }, cards, min, max);
                                }

                                if (hint == HintMsg.AddToHand)
                                {
                                    int targetId = GetArchfiendSearchTarget(exceptId: CardId.Strategy);
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                    if (target != null)
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                                break;
                            }

                        case CardId.Royal:
                            {
                                if (hint == HintMsg.AddToHand)
                                {
                                    int targetId = GetArchfiendSearchTarget(exceptId: CardId.Royal);
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                    if (target != null)
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                                break;
                            }

                        case CardId.Highness:
                            {
                                if (hint == HintMsg.Remove)
                                {
                                    ClientCard cost = cards
                                        .Where(c => c != null && c.Controller == 0 && c.HasSetcode(SetcodeArchfiend))
                                        .OrderBy(c => GetArchfiendCostPriority(c))
                                        .FirstOrDefault();

                                    if (cost != null)
                                        return Util.CheckSelectCount(new List<ClientCard> { cost }, cards, min, max);
                                }

                                if (hint == HintMsg.AddToHand)
                                {
                                    List<int> targetIds = GetHighnessSearchTargets();
                                    List<ClientCard> selected = new List<ClientCard>();

                                    foreach (int id in targetIds)
                                    {
                                        ClientCard target = cards.FirstOrDefault(c => c.IsCode(id) && !selected.Contains(c));
                                        if (target != null) selected.Add(target);
                                    }

                                    if (selected.Count > 0)
                                        return Util.CheckSelectCount(selected, cards, min, max);
                                }
                                break;
                            }
                        case CardId.Makourai:
                            {
                                if (hint == HintMsg.AddToHand)
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(CardId.SMSkull));
                                    if (target != null)
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }

                                break;
                            }
                        default:
                            break;
                    }
                }
                if (currentChain.ActivateController == 1)
                {
                    switch (currentChain.ActivateId)
                    {
                        case CardId.AmeNoMurakumoNoMitsurugi:
                            {
                                if (hint == HintMsg.Discard
                                    && cards != null
                                    && cards.Count > 0
                                    && cards.All(c =>
                                        c != null
                                        && c.Controller == 0
                                        && c.Location == CardLocation.Hand))
                                {
                                    ClientCard discard = cards.OrderBy(GetMurakumoDiscardPriority).FirstOrDefault();

                                    if (discard != null)
                                    {
                                        Logger.DebugWriteLine($"[MURAKUMO] Archfiend discard => " + $"{discard.Name}({discard.Id})");
                                        return Util.CheckSelectCount(new List<ClientCard> { discard }, cards, min, max);
                                    }
                                }

                                break;
                            }
                        case _CardId.EvenlyMatched:
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
                                // other monster
                                List<ClientCard> otherMonsters = botMonsters.Where(card => !banishList.Contains(card)).ToList();
                                otherMonsters.Sort(CardContainer.CompareCardAttack);
                                banishList.AddRange(otherMonsters);
                                List<ClientCard> faceDownSpells = Bot.GetSpells().Where(c => c.IsFacedown()).ToList();
                                banishList.AddRange(ShuffleList(faceDownSpells));
                                return Util.CheckSelectCount(banishList, cards, min, max);
                            }
                        default:
                            break;
                    }
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        #endregion

        #region Primite
        public bool PrimiteDragonEtherBerylActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return !CheckWhetherNegated();
            }
            // to grave
            if (ActivateDescription == Util.GetStringId(CardId.PMBeryl, 1))
            {
                return false;
                
            }
            else
            {
                // search
                return !CheckWhetherNegated();
            }
        }

        public bool PrimiteDrillbeamActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated()) return false;

                // check whether can active
                bool canActivate = Bot.HasInHand(CardId.PMLL) || Bot.HasInHand(CardId.PMBeryl);
                canActivate |= Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(CardId.SMSkull));
                canActivate |= Bot.GetSpellCountWithoutField() <= 3;

                if (canActivate)
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }

                // cannot activate
                return false;
            }

            // negate
            if (CheckWhetherNegated(true, true, CardType.Spell))
            {
                return false;
            }

            bool activateFlag = false;

            // negate problematic enemy card
            List<ClientCard> problematicEnemyCardList = GetProblematicEnemyCardList(true, false, CardType.Spell);
            if (problematicEnemyCardList.Count(c => !c.IsDisabled()) > 0)
            {
                problematicEnemyCardList.RemoveAll(c => currentNegateCardList.Contains(c));
                if (problematicEnemyCardList.Count > 0)
                {
                    activateFlag = true;
                }
            }

            // negate cards on chain
            foreach (ClientCard card in Duel.CurrentChain)
            {
                if (card.Controller == 1 && card.IsOnField() && card.IsFaceup() && !card.IsDisabled() && !currentNegateCardList.Contains(card)
                    && CheckCanBeTargeted(card, true, CardType.Spell) && CheckCardShouldNegate(card))
                {
                    activateFlag = true;
                }
            }

            // can recycle, so activate it
            if (Bot.HasInMonstersZone(CardId.PMBeryl, faceUp: true) && !activatedCardIdList.Contains(CardId.PMDR + 1)
                && (CurrentTiming & hintToHand) == 0)
            {
                List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Spell, true);
                if (targetList.Count > 0)
                {
                    activateFlag = true;
                }
            }

            // become target
            if (DefaultOnBecomeTarget())
            {
                activateFlag |= Enemy.GetSpells().Any(c => c.IsFaceup() && !c.IsDisabled() && CheckCanBeTargeted(c, true, CardType.Spell));
                activateFlag |= Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && CheckCanBeTargeted(c, true, CardType.Spell));
            }

            if (Duel.Phase == DuelPhase.BattleStep)
            {
                // remove problematic enemy monster
                ClientCard problematicEnemyMonster = GetProblematicEnemyMonster(0, true, true, CardType.Spell);
                if (problematicEnemyMonster != null && !problematicEnemyMonster.IsDisabled())
                {
                    activateFlag = true;
                }
            }

            if (activateFlag)
            {
                activatedCardIdList.Add(Card.Id);
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }

        public bool PrimiteLordlyLodeActivate()
        {
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                return false;
            }

            // activate
            bool activateFlag = PrimiteLordlyLodeActivateCheck();
            Logger.DebugWriteLine("PrimiteLordlyLodeActivate: " + activateFlag);
            if (activateFlag)
            {
                SelectSTPlace(Card, true);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool PrimiteLordlyLodeActivateCheck()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            bool activateFlag = false;
            if (Bot.HasInHandOrHasInMonstersZone(CardId.PMBeryl) && DefaultCheckWhetherBotCanSearch())
            {
                // for search drillbeam
                activateFlag |= CheckRemainInDeck(CardId.PMDR) > 0;
                activateFlag |= summonCount <= 0 && Card.Location == CardLocation.SpellZone && Card.IsFacedown();
            }
            if (summonCount > 0 && !Bot.HasInHand(CardId.PMBeryl) && CheckRemainInDeck(CardId.PMBeryl) > 0 && DefaultCheckWhetherBotCanSearch())
            {
                // for search ether beryl
                activateFlag |= Bot.HasInGraveyard(CardId.PMDR);
                activateFlag |= !CheckWhetherNegated(true, true, CardType.Monster);
            }
            if (Bot.HasInHandOrHasInMonstersZone(CardId.SMSkull) && DefaultCheckWhetherBotCanSearch())
            {
                // for search drillbeam
                activateFlag |= CheckRemainInDeck(CardId.PMDR) > 0;
                activateFlag |= summonCount <= 0 && Card.Location == CardLocation.SpellZone && Card.IsFacedown();
            }
            if (!Bot.HasInSpellZone(CardId.PMLL, true, true))
            {
                // for activate it
                activateFlag |= DefaultCheckWhetherBotCanSearch();

                // for special summon
                bool hasSMSkull = Bot.HasInHand(CardId.SMSkull)
                                 || CheckRemainInDeck(CardId.SMSkull) > 0
                                 || Bot.HasInGraveyard(CardId.SMSkull);

                if (!hasSMSkull)
                {
                    return false;
                }
            }
            if (Card.Location == CardLocation.SpellZone && Card.IsFacedown())
            {
                activateFlag |= DefaultCheckWhetherBotCanSearch();
            }
            return activateFlag;
        }

        public bool PrimiteLordlyLodeSpSummon()
        {
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                // add activating flag
                if (CheckWhetherNegated()) return false;
                if (!PrimiteLordlyLodeSpSummonCheck()) return false;
                activatingLodeSpSummonEffect = true;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool PrimiteLordlyLodeSpSummonCheck()
        {
            if (Bot.HasInMonstersZone(CardId.SMSkull, faceUp: true))
            {
                return false;
            }
            if (!IsBadHandForPrimiteLodeSpSummon())
                return false;
            // special summon
            CardLocation loc;
            if (Bot.HasInHand(CardId.SMSkull))
            {
                loc = CardLocation.Hand;
            }
            else if (CheckRemainInDeck(CardId.SMSkull) > 0)
            {
                loc = CardLocation.Deck;
            }
            else if (Bot.HasInGraveyard(CardId.SMSkull))
            {
                loc = CardLocation.Grave;
            }
            else
            {
                return false;
            }
            int drawCount = GetSpecialSummonDrawCount(loc);
            return drawCount < 2;
        }
        private bool IsBadHandForPrimiteLodeSpSummon()
        {
            // ถ้ามี play หลักอื่นอยู่ ยังไม่ถือว่ามือเน่า
            if (HasArchfiendPlayableStarter())
                return false;

            // ถ้ายัง Normal Beryl/Highness ได้ ยังไม่ต้องใช้ LL SS
            if (summonCount >= 0 && Bot.HasInHand(CardId.PMBeryl))
                return false;

            if (summonCount >= 0 && Bot.HasInHand(CardId.Highness) && HasArchfiendCostInGrave())
                return false;

            // ถ้ามี Origin + มี cost ให้เล่นเองได้ ก็ยังไม่เน่า
            if (Bot.HasInHand(CardId.Origin) && GetOriginTributeCost() != null)
                return false;

            // ถ้ามี Royal แล้วยังไม่ได้ใช้ ก็ยังมี play
            if (Bot.HasInHand(CardId.Royal) && !IsArchfiendCardUsedThisTurn(CardId.Royal))
                return false;

            // ถ้ามี Strategy/Usurpation แล้วยังไม่ได้ใช้ ก็ยังมีทางต่อ
            if (Bot.HasInHand(CardId.Strategy) && !IsArchfiendCardUsedThisTurn(CardId.Strategy))
                return false;

            if (Bot.HasInHand(CardId.Usurpation) && !IsArchfiendCardUsedThisTurn(CardId.Usurpation))
                return false;

            // ถ้า PMDR อยู่ในมือ/เซ็ตไว้แล้ว ก็ยังมี interaction ไม่จำเป็นต้องฝืน SS
            if (Bot.HasInHand(CardId.PMDR) || Bot.HasInSpellZone(CardId.PMDR))
                return false;

            return true;
        }
        private bool HasArchfiendPlayableStarter()
        {
            if (Bot.HasInHand(CardId.Royal) && !IsArchfiendCardUsedThisTurn(CardId.Royal))
                return true;

            if (Bot.HasInHand(CardId.Strategy)
                && !IsArchfiendCardUsedThisTurn(CardId.Strategy)
                && HasArchfiendCostForStrategy())
                return true;

            if (Bot.HasInHand(CardId.Usurpation)
                && !IsArchfiendCardUsedThisTurn(CardId.Usurpation)
                && !IsArchfiendCardUsedThisTurn(CardId.Playtime)
                && GetUsurpationTrapToSet() != 0)
                return true;

            if (Bot.HasInHand(CardId.Origin) && GetOriginTributeCost() != null)
                return true;

            if (HasRegenArchPlayableWithoutOther2500())
                return true;


            return false;
        }
        private bool HasRegenArchPlayableWithoutOther2500()
        {
            bool hasRegenArch =
                Bot.HasInHand(CardId.RegenArch)
                || Bot.HasInMonstersZone(CardId.RegenArch, faceUp: true);

            if (!hasRegenArch)
                return false;

            bool hasOtherAtk2500Monster =
                Bot.GetMonsters().Any(c =>
                    c != null
                    && c.IsFaceup()
                    && !c.IsCode(CardId.RegenArch)
                    && c.Attack == 2500);

            return !hasOtherAtk2500Monster;
        }
        public int GetSpecialSummonDrawCount(CardLocation loc)
        {
            int res = 0;
            if (!DefaultCheckWhetherEnemyCanDraw())
            {
                return 0;
            }
            if (enemyResolvedEffectIdList.Contains(_CardId.MaxxC))
            {
                res++;
            }

            if ((loc & CardLocation.Hand) != 0)
            {
                res += enemyResolvedEffectIdList.Count(id => id == _CardId.MulcharmyPurulia);
            }
            if ((loc & (CardLocation.Deck | CardLocation.Extra)) != 0)
            {
                res += enemyResolvedEffectIdList.Count(id => id == _CardId.MulcharmyFuwalos);
            }
            if ((loc & (CardLocation.Grave | CardLocation.Removed)) != 0)
            {
                res += enemyResolvedEffectIdList.Count(id => id == _CardId.MulcharmyNyalus);
            }

            return res;
        }

        public bool Level4MonsterSummon()
        {
            if (!Card.IsCode(CardId.PMBeryl, CardId.Highness))
            {
                return false;
            }

            bool canSummonDragon = Bot.HasInHand(CardId.PMBeryl);
            if (!activatedCardIdList.Contains(CardId.PMLL) && DefaultCheckWhetherBotCanSearch())
            {
                canSummonDragon |= Bot.HasInHand(CardId.PMLL) && Bot.GetSpellCountWithoutField() < 5;
                canSummonDragon |= Bot.GetSpells().Any(c => c.IsCode(CardId.PMLL) && c.IsFacedown());
            }
            if (canSummonDragon)
            {
                bool summonFlag = false;
                // summon to search?
                if (!CheckWhetherNegated(true, true))
                {
                    summonFlag |= !activatedCardIdList.Contains(CardId.PMLL) && !Bot.HasInHandOrInSpellZone(CardId.PMLL) && CheckRemainInDeck(CardId.PMLL) > 0;
                    summonFlag |= CheckRemainInDeck(CardId.PMDR) > 0;
                }

                // summon to recycle beam
                if (!Bot.HasInMonstersZone(CardId.PMBeryl, faceUp: true) && !activatedCardIdList.Contains(CardId.PMDR + 1)
                    && Bot.HasInGraveyard(CardId.PMDR) && Bot.GetSpellCountWithoutField() < 5)
                {
                    summonFlag = true;
                }

                if (summonFlag && Card.IsCode(CardId.PMBeryl))
                {
                    summonCount--;
                    return true;
                }
            }
            bool canSummonHighness = ShouldNormalSummonHighness();

            if (canSummonHighness && Card.IsCode(CardId.Highness))
            {
                summonCount--;
                return true;
            }

            return false;
        }
        private bool ShouldNormalSummonHighness()
        {
            if (!Bot.HasInHand(CardId.Highness)) return false;

            if (IsArchfiendCardUsedThisTurn(CardId.Highness)) return false;

            if (Bot.HasInMonstersZone(CardId.Highness, faceUp: true)) return false;

            if (IsArchfiendBoardReady()) return false;

            if (!DefaultCheckWhetherBotCanSearch()) return false;

            if (!Bot.Graveyard.Any(card => card != null && card.HasSetcode(SetcodeArchfiend))) return false;

            return true;
        }
        private bool ShouldPMLLSearchDrillbeam()
        {
            if (CheckRemainInDeck(CardId.PMDR) <= 0) return false;

            if (Bot.HasInHand(CardId.PMDR) || Bot.HasInSpellZone(CardId.PMDR))
                return false;

            if (Bot.HasInHandOrHasInMonstersZone(CardId.PMBeryl))
                return true;

            if (Bot.HasInHandOrHasInMonstersZone(CardId.SMSkull))
                return true;

            if (Bot.GetSpells().Any(c => c.IsFacedown() || Duel.CurrentChain.Contains(c)))
                return true;

            if (summonCount <= 0)
                return true;

            return false;
        }
        #endregion

        #region original code

        private bool Playtime()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;
            if (IsArchfiendBoardReady()) return false;
            if (Duel.Player == 0 && Bot.HasInHand(CardId.Royal)) return false;

            int target = GetPlaytimeTarget();
            if (target == 0) return false;

            AI.SelectCard(target);
            activatedCardIdList.Add(Card.Id);
            return true;
        }
        private int GetPlaytimeTarget()
        {
            if (Duel.Player == 0)
            {
                if (CanPlaytimeSummon(CardId.Royal)) return CardId.Royal;
                if (!Bot.HasInMonstersZone(CardId.Origin, faceUp: true) && CanPlaytimeSummon(CardId.Origin)) return CardId.Origin;
                if (CanPlaytimeSummon(CardId.SMSkull)) return CardId.SMSkull;
                return 0;
            }

            if (Bot.HasInMonstersZone(CardId.Origin, faceUp: true) && Bot.HasInMonstersZone(CardId.SMSkull, faceUp: true))
                return 0;

            if (!Bot.HasInMonstersZone(CardId.Origin, faceUp: true) && CanPlaytimeSummon(CardId.Origin)) return CardId.Origin;
            if (!Bot.HasInMonstersZone(CardId.SMSkull, faceUp: true) && CanPlaytimeSummon(CardId.SMSkull)) return CardId.SMSkull;
            if (CanPlaytimeSummon(CardId.Royal)) return CardId.Royal;

            return 0;
        }
        private bool CanPlaytimeSummon(int id)
        {
            return Bot.HasInHand(id) || CheckRemainInDeck(id) > 0;
        }
        private bool Usurpation()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Bot.GetSpellCountWithoutField() >= 5) return false;
            int trapId = GetUsurpationTrapToSet();
            if (trapId == 0) return false;

            SelectSTPlace(Card, true);
            AI.SelectCard(trapId);
            activatedCardIdList.Add(Card.Id);
            return true;
        }
        private int GetUsurpationTrapToSet()
        {
            if(IsArchfiendBoardReady() && CanSetArchfiendTrap(CardId.Simul))
                return CardId.Simul;

            if(Bot.HasInHand(CardId.Royal) && CanSetArchfiendTrap(CardId.Simul))
                return CardId.Simul;

            if (IsArchfiendCardUsedThisTurn(CardId.Royal) && CanSetArchfiendTrap(CardId.Simul))
                return CardId.Simul;

            if (Duel.Player == 1 && CanSetArchfiendTrap(CardId.Simul) && ShouldUseSimul())
                return CardId.Simul;

            if (CanSetArchfiendTrap(CardId.Playtime))
                return CardId.Playtime;

            if (CanSetArchfiendTrap(CardId.Simul))
                return CardId.Simul;

            return 0;
        }
        private bool Simul()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;
            activatedCardIdList.Add(Card.Id);
            return ShouldUseSimul();
        }

        private bool ShouldUseSimul()
        {
            if (Duel.Player == 1)
            {
                if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
                return Enemy.GetMonsters().Any(IsRitualFusionSynchroXyzLink);
            }
            return false;
        }

        private bool IsRitualFusionSynchroXyzLink(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsMonster()
                && (card.HasType(CardType.Ritual)
                    || card.HasType(CardType.Fusion)
                    || card.HasType(CardType.Synchro)
                    || card.HasType(CardType.Xyz)
                    || card.HasType(CardType.Link));
        }  

        private bool CanSetArchfiendTrap(int id)
        {
            return Bot.HasInGraveyard(id) || CheckRemainInDeck(id) > 0;
        }

        private bool StrategyActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            if (!Bot.HasInSpellZone(CardId.Strategy, true, true))
            {
                SelectSTPlace(Card, true);
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                if (!DefaultCheckWhetherBotCanSearch()) return false;
                if (!HasArchfiendCostForStrategy()) return false;

                int target = GetArchfiendSearchTarget(exceptId: CardId.Strategy);
                if (target == 0) return false;

                activatedCardIdList.Add(Card.Id + 1);

                AI.SelectCard(target);
                return true;
            }

            return false;
        }

        private bool RoyalActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (!DefaultCheckWhetherBotCanSearch()) return false;

            int target = GetArchfiendSearchTarget(exceptId: CardId.Royal);
            if (target == 0) return false;

            activatedCardIdList.Add(Card.Id);
            AI.SelectCard(target);
            return true;
        }

        private bool HighnessActivate()
        {
            if (CheckWhetherNegated()) return false;

            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            // Normal Summon effect
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (!DefaultCheckWhetherBotCanSearch()) return false;
                if (!HasArchfiendCostInGrave()) return false;

                List<int> targets = GetHighnessSearchTargets();
                if (targets.Count < 2) return false;

                activatedCardIdList.Add(Card.Id);
                AI.SelectCard(targets[0], targets[1]);
                return true;
            }

            return false;
        }
        private bool HasArchfiendCostForStrategy()
        {
            return Bot.Hand.Any(c => c != null && (c.IsMonster() && c.HasRace(CardRace.Fiend) || c.HasSetcode(SetcodeArchfiend)))
                || Bot.Graveyard.Any(c => c != null && (c.IsMonster() && c.HasRace(CardRace.Fiend) || c.HasSetcode(SetcodeArchfiend)));
        }

        private bool HasArchfiendCostInGrave()
        {
            return Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeArchfiend));
        }

        private int GetArchfiendSearchTarget(int exceptId = 0)
        {
            List<int> priority = new List<int>
            {
                CardId.Royal,
                CardId.Highness,
                CardId.Origin,
                CardId.RegenArch,
                CardId.Usurpation,
                CardId.Makourai
            };

            foreach (int id in priority)
            {
                if (id == exceptId) continue;
                if (CheckRemainInDeck(id) <= 0) continue;
                if (HasUnusedInHand(id)) continue;

                return id;
            }

            return 0;
        }
        private bool HasUnusedInHand(int id)
        {
            return Bot.HasInHand(id) && !IsArchfiendCardUsedThisTurn(id);
        }

        private List<int> GetHighnessSearchTargets()
        {
            return new List<int>
            {
                CardId.Origin,
                CardId.RegenArch,
                CardId.Makourai,
                CardId.Usurpation,
                CardId.Strategy,
                CardId.Royal
            }
            .Where(id => id != CardId.Highness)
            .Where(id => CheckRemainInDeck(id) > 0)
            .OrderBy(id => GetHighnessSearchPriority(id))
            .Take(2)
            .ToList();
        }
        private int GetHighnessSearchPriority(int id)
        {
            int score = 0;

            // ใบที่ใช้แล้วในเทิร์นนี้ ลด priority หนัก ๆ
            if (IsArchfiendCardUsedThisTurn(id))
                score += 1000;

            // มีอยู่แล้วในมือก็ไม่ค่อยอยากหยิบซ้ำ
            if (Bot.HasInHand(id))
                score += 200;

            // มี face-up อยู่แล้วก็ลด priority
            if (Bot.HasInMonstersZone(id, faceUp: true) || Bot.HasInSpellZone(id, true))
                score += 300;

            // base priority
            if (id == CardId.Royal) score += 1;
            else if (id == CardId.Usurpation) score += 2;
            else if (id == CardId.Origin) score += 3;
            else if (id == CardId.Makourai) score += 4;
            else if (id == CardId.Strategy) score += 5;
            else if (id == CardId.Playtime) score += 6;
            else if (id == CardId.Simul) score += 7;
            else if (id == CardId.SMSkull) score += 8;
            else score += 100;

            return score;
        }

        private bool IsArchfiendCardUsedThisTurn(int id)
        {
            if (activatedCardIdList.Contains(id)) return true;
            if (activatedCardIdList.Contains(id + 1)) return true;

            return false;
        }
        private int GetArchfiendCostPriority(ClientCard card)
        {
            if (card == null) return 999;

            int locationScore = 0;

            if (card.Location == CardLocation.Grave) locationScore = 0;
            else if (card.Location == CardLocation.Hand) locationScore = 100;
            else locationScore = 500;

            int cardScore = 100;


            if (card.IsCode(CardId.Playtime) && (card.Location == CardLocation.Hand)) cardScore = 1;
            else if (card.IsCode(CardId.Usurpation)) cardScore = 2;
            else if (card.IsCode(CardId.Strategy)) cardScore = 3;
            else if (card.IsCode(CardId.Highness)) cardScore = 4;
            else if (card.IsCode(CardId.Makourai)) cardScore = 6;
            else if (card.IsCode(CardId.Royal)) cardScore = 7;
            else if (card.IsCode(CardId.Origin)) cardScore = 8;
            else if (card.IsCode(CardId.SMSkull)) cardScore = 9;
            else if (card.IsCode(CardId.Playtime) && (card.Location == CardLocation.Grave)) cardScore = 501;
            else if (card.IsCode(CardId.Simul) && (card.Location == CardLocation.Grave)) cardScore = 502;
            
            return locationScore + cardScore;
        }
        private bool OriginActivate()
        {
            if (CheckWhetherNegated()) return false;

            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
            {
                ClientCard last = Util.GetLastChainCard();

                if (last.IsMonster()
                    && Bot.HasInMonstersZone(CardId.SMSkull, faceUp: true)
                    && CheckLastChainShouldNegated())
                {
                    currentNegateCardList.Add(last);
                    return true;
                }

                return false;
            }

            if (Card.Location == CardLocation.Hand)
            {
                ClientCard tribute = GetOriginTributeCost();
                if (tribute == null) return false;

                AI.SelectCard(tribute);
                return true;
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.HasInHand(CardId.SMSkull) || CheckRemainInDeck(CardId.SMSkull) > 0 || Bot.HasInGraveyard(CardId.SMSkull))
                {
                    AI.SelectCard(CardId.SMSkull);
                    return true;
                }
            }

            return false;
        }

        private bool OriginSpSummon()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckWhetherNegated()) return false;

            ClientCard tribute = GetOriginTributeCost();
            if (tribute == null) return false;

            AI.SelectCard(tribute);
            return true;
        }

        private ClientCard GetOriginTributeCost()
        {
            List<ClientCard> candidates = new List<ClientCard>();

            candidates.AddRange(Bot.Hand.Where(c =>
                c != null
                && c != Card
                && c.HasSetcode(SetcodeArchfiend)));

            candidates.AddRange(Bot.GetMonsters().Where(c =>
                c != null
                && c != Card
                && c.HasSetcode(SetcodeArchfiend)));

            return candidates
                .OrderBy(c => GetOriginTributePriority(c))
                .FirstOrDefault(c => GetOriginTributePriority(c) < 9999);
        }

        private int GetOriginTributePriority(ClientCard card)
        {
            if (card == null) return 9999;

            if (card.IsCode(CardId.RegenArch) && card.Location == CardLocation.MonsterZone)
                return 9999;

            int locationScore = 0;

            if (card.Location == CardLocation.Hand) locationScore = 0;
            else if (card.Location == CardLocation.MonsterZone) locationScore = 100;
            else return 9999;

            int cardScore = 100;

            if (card.IsCode(CardId.Highness)&&(card.Location == CardLocation.MonsterZone)) cardScore = -99;
            else if (card.IsCode(CardId.SMSkull)) cardScore = 2;
            else if (card.IsCode(CardId.Origin)) cardScore = 3;
            else if (card.IsCode(CardId.Highness)) cardScore = 4;
            else if (card.IsCode(CardId.RegenArch)) cardScore = 5;
            else if (card.IsCode(CardId.Royal)) cardScore = 6;

            return locationScore + cardScore;
        }
        private bool MakouraiActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            if (Card.Location == CardLocation.Grave)
            {
                if (Duel.Player != 0) return false;
                if (!Bot.HasInGraveyard(CardId.SMSkull)) return false;

                AI.SelectCard(CardId.SMSkull);
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            ClientCard target = GetBestMakouraiTarget();
            if (target == null) return false;

            bool canDestroySomething = Enemy.GetMonsters().Any(e =>
                e != null
                && e.IsFaceup()
                && e.Attack >= 0
                && e.Attack < target.Attack + 600);

            bool battlePush = Duel.Phase == DuelPhase.BattleStep || Duel.Phase == DuelPhase.Damage;

            if (!canDestroySomething && !battlePush) return false;

            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }

            AI.SelectCard(target);
            activatedCardIdList.Add(Card.Id);
            return true;
        }
        private ClientCard GetBestMakouraiTarget()
        {
            List<ClientCard> archfiends = Bot.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && c.HasSetcode(SetcodeArchfiend)
                    && !c.IsDisabled())
                .OrderByDescending(c => CountMakouraiDestroyTargets(c))
                .ThenByDescending(c => c.Attack)
                .ToList();

            if (archfiends.Count <= 0) return null;

            return archfiends[0];
        }

        private int CountMakouraiDestroyTargets(ClientCard target)
        {
            if (target == null) return 0;

            int afterBoostAtk = target.Attack + 600;

            return Enemy.GetMonsters().Count(e =>
                e != null
                && e.IsFaceup()
                && e.Attack >= 0
                && e.Attack < afterBoostAtk);
        }
        private bool IsArchfiendBoardReady()
        {
            bool hasOrigin = Bot.HasInMonstersZone(CardId.Origin, faceUp: true);
            bool hasSMSkull = Bot.HasInMonstersZone(CardId.SMSkull, faceUp: true);
            bool hasRegen = Bot.HasInHandOrHasInMonstersZone(CardId.RegenArch) && Bot.HasInMonstersZone(CardId.RegenArch, faceUp: true);

            if (hasOrigin && hasSMSkull && hasRegen)
                return true;

            return false;
        }
        private int GetMurakumoDiscardPriority(ClientCard card)
        {
            if (card == null)
                return int.MaxValue;

            bool hasDuplicate = Bot.Hand.Count(c =>
                c != null && c.IsCode(card.Id)) > 1;

            int duplicateScore = hasDuplicate ? 0 : 10000;

            return duplicateScore
                + GetArchfiendCostPriority(card);
        }
        #endregion
    }
}