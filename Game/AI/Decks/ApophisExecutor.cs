using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System;
using System.Linq;
using System.Diagnostics;

namespace WindBot.Game.AI.Decks
{
    [Deck("Apophis", "AI_Apophis")]
    public class ApophisExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int LabradoriteDragon = 62514770;
            public const int AnubisTheLastJudge = 60411677;
            public const int PrimiteDragonEtherBeryl = 63198739;
            public const int TheManWithTheMark = 97522863;
            // PotOfExtravagance = 49238328;
            public const int Terraforming = 73628505;
            public const int PrimiteDrillbeam = 29095457;
            public const int PrimiteLordlyLode = 56506740;
            public const int TreasuresOfTheKings = 69299029;
            public const int DominusSpark = 6325660;
            // InfiniteImpermanence = 10045474;
            public const int DominusImpulse = 40366667;
            public const int SongsOfTheDominators = 58053438;
            public const int DominusPurge = 97045737;
            public const int ApophisTheSwampDeity = 85888377;
            public const int ApophisTheSerpent = 95561146;
            public const int VerdictOfAnubis = 59576447;
            public const int SolemnReport = 78114463;
            public const int DivineSerpentApophis = 97800311;
            public const int SwordsoulSupremeSovereignChengying = 96633955;
            public const int BaronneDeFleur = 84815190;
            public const int SuperdreadnoughtRailCannonJuggernautLiebe = 26096328;
            public const int SuperdreadnoughtRailCannonFlyingLauncher = 38354018;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int SPLittleKnight = 46772449;
            public const int SilhouhatteRabbit = 1528054;
            public const int LinkSpider = 98978921;
        }

        public ApophisExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, CardId.SolemnReport, SolemnReportBanishActivate);
            AddExecutor(ExecutorType.Activate, CardId.ApophisTheSwampDeity, ApophisTheSwampDeityActivate);
            AddExecutor(ExecutorType.Activate, CardId.SongsOfTheDominators, SongsOfTheDominatorsActivateFirst);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.SongsOfTheDominators, DominusNegateTrapActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusPurge, DominusNegateTrapActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusImpulse, DominusNegateTrapActivate);
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur, BaronneDeFleurNegateEffect);
            AddExecutor(ExecutorType.Activate, CardId.PrimiteDrillbeam, PrimiteDrillbeamActivate);
            AddExecutor(ExecutorType.Activate, CardId.VerdictOfAnubis, SpellNegateActivate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnReport, SpellNegateActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPLittleKnightActivate);
            AddExecutor(ExecutorType.Activate, CardId.DominusSpark, DominusSparkActivate);

            // first in main phase
            AddExecutor(ExecutorType.Activate, _CardId.PotOfExtravagance, PotOfExtravaganceActivate);
            AddExecutor(ExecutorType.Repos, MonstetReposForImportantMonsters);

            // startup effect
            AddExecutor(ExecutorType.Activate, CardId.BaronneDeFleur, BaronneDeFleurActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperdreadnoughtRailCannonJuggernautLiebe, SuperdreadnoughtRailCannonJuggernautLiebeActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperdreadnoughtRailCannonFlyingLauncher, SuperdreadnoughtRailCannonFlyingLauncherActivate);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.SpellSet, SpellSetFirst);
            AddExecutor(ExecutorType.Activate, CardId.SilhouhatteRabbit, SilhouhatteRabbitActivate);
            AddExecutor(ExecutorType.Activate, CardId.DivineSerpentApophis, DivineSerpentApophisActivate);
            AddExecutor(ExecutorType.Activate, CardId.SwordsoulSupremeSovereignChengying, SwordsoulSupremeSovereignChengyingActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheManWithTheMark, TheManWithTheMarkActivate);

            // spsummon
            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AnubisTheLastJudge, AnubisTheLastJudgeSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonJuggernautLiebe, SuperdreadnoughtRailCannonJuggernautLiebeSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonFlyingLauncher, SuperdreadnoughtRailCannonFlyingLauncherSpSummon);
            AddExecutor(ExecutorType.SpSummon, Level10SynchroSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DivineSerpentApophis, DivineSerpentApophisSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpiderSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPLittleKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SilhouhatteRabbit, SilhouhatteRabbitSummon);
            AddExecutor(ExecutorType.Activate, CardId.ApophisTheSerpent, ApophisTheSerpentActivate);

            // search
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingActivate);
            AddExecutor(ExecutorType.Activate, CardId.TreasuresOfTheKings, TreasuresOfTheKingsActivate);
            AddExecutor(ExecutorType.Activate, CardId.PrimiteLordlyLode, PrimiteLordlyLodeActivate);
            AddExecutor(ExecutorType.Summon, Level4MonsterSummon);
            AddExecutor(ExecutorType.Activate, CardId.AnubisTheLastJudge, AnubisTheLastJudgeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PrimiteDragonEtherBeryl, PrimiteDragonEtherBerylActivate);
            AddExecutor(ExecutorType.Activate, CardId.PrimiteLordlyLode, PrimiteLordlyLodeSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.ApophisTheSwampDeity, ApophisTheSwampDeityActivateForAttack);

            // next turn prepare
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        List<int> NotToNegateIdList = new List<int>
        {
            58699500, 20343502, 25451383, 19403423
        };
        List<int> solemnReportBanishIdList = new List<int> {
            _CardId.MysticalSpaceTyphoon, 63166095, 9726840, 5380979, 92714517, 6153210, 32548318, 30271097, 45171524, 81560239
        };

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.TheManWithTheMark, CardId.PrimiteLordlyLode, CardId.TreasuresOfTheKings, CardId.DominusSpark,
                                _CardId.InfiniteImpermanence, CardId.SongsOfTheDominators, CardId.DominusPurge, CardId.ApophisTheSerpent}},
            {2, new List<int> { CardId.AnubisTheLastJudge, CardId.PrimiteDragonEtherBeryl, _CardId.PotOfExtravagance, CardId.DominusImpulse,
                                CardId.ApophisTheSwampDeity, CardId.SolemnReport}},
            {1, new List<int> { CardId.LabradoriteDragon, CardId.Terraforming, CardId.PrimiteDrillbeam, CardId.VerdictOfAnubis }}
        };
        const int hintTimingMainEnd = 0x4;
        const int hintToHand = 0x200000;

        int maxSummonCount = 1;
        int summonCount = 1;
        bool enemyActivateMonsterEffectFromHandGrave = false;
        int dimensionShifterCount = 0;
        int songsOfTheDominatorsResolvedCount = 0;
        bool activatingLodeSpSummonEffect = false;
        bool lodeSpSummonEffectResolved = false;
        bool songsOfTheDominatorsActivatedFromHand = false;
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        List<ClientCard> summonThisTurn = new List<ClientCard>();
        List<ClientCard> placedThisTurn = new List<ClientCard>();
        List<ClientCard> activatedDivineSerpent1stList = new List<ClientCard>();
        List<ClientCard> activatedDivineSerpent2ndList = new List<ClientCard>();

        int anubisTheLastJudgeSpSummoningStep = 0;
        int SPLittleKnightRemoveStep = 0;
        int currentSummoningCount = 0;

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
                if (((int)selfType & (int)CardType.Trap) > 0 && card.IsShouldNotBeSpellTrapTarget()
                    && !card.IsDisabled()) return false;
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
        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0, bool ignore41 = false)
        {
            bool isMonster = type == 0 && Card.IsMonster();
            isMonster |= (type & CardType.Monster) != 0;
            bool isSpellOrTrap = type == 0 && (Card.IsSpell() || Card.IsTrap());
            isSpellOrTrap |= (type & (CardType.Spell | CardType.Trap)) != 0;
            bool isCounter = (type & CardType.Counter) != 0;
            if (isSpellOrTrap && toFieldCheck)
            {
                if (CheckSpellWillBeNegate(isCounter)) return true;
                if (DefaultCheckWhetherSpellActivateWillBeNegated(Card)) return true;
            }
            if (DefaultCheckWhetherCardIsNegated(Card)) return true;
            if (isMonster && (toFieldCheck || Card.Location == CardLocation.MonsterZone))
            {
                if ((toFieldCheck && ((type & CardType.Link) != 0)) || Card.IsDefense())
                {
                    if (Enemy.MonsterZone.Any(card => CheckNumber41(card, ignore41)) || Bot.MonsterZone.Any(card => CheckNumber41(card, ignore41))) return true;
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
                if (infiniteImpermanenceNegatedColumns.Contains(selfSeq)) return true;
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
            return CheckCardShouldNegate(lastcard);
        }

        public bool CheckCardShouldNegate(ClientCard card)
        {
            if (card == null) return false;
            if (card.IsMonster() && card.HasSetcode(_Setcode.TimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (NotToNegateIdList.Contains(card.Id)) return false;
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

        public bool CheckCardShouldNegate(ChainInfo chainInfo)
        {
            if (chainInfo == null) return false;
            ClientCard card = chainInfo.RelatedCard;

            if (card == null) return false;
            if (card.IsMonster() && card.HasSetcode(_Setcode.TimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (NotToNegateIdList.Contains(card.Id)) return false;
            if (card.HasSetcode(_Setcode.Danger) && card.Location == CardLocation.Hand) return false;
            if (card.IsMonster() && chainInfo.HasLocation(CardLocation.MonsterZone) && chainInfo.HasPosition(CardPosition.Defence))
            {
                if (Enemy.MonsterZone.Any(c => CheckNumber41(c)) || Bot.MonsterZone.Any(c => CheckNumber41(c))) return false;
            }
            if (DefaultCheckWhetherCardIsNegated(card)) return false;
            if (Duel.Player == 1 && card.IsCode(_CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus)) return false;
            if (card.IsDisabled()) return false;

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
            if (CheckAtAdvantage() && enemyResolvedEffectIdList.Contains(_CardId.MaxxC) && DefaultCheckWhetherEnemyCanDraw() && (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2))
            {
                return true;
            }
            return false;
        }

        public bool CheckShouldNoMoreSpSummon(CardLocation loc)
        {
            if (CheckShouldNoMoreSpSummon()) return true;
            if (!DefaultCheckWhetherEnemyCanDraw() || (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2)) return false;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyPurulia) && (loc & CardLocation.Hand) != 0) return true;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyFuwalos) && (loc & (CardLocation.Deck | CardLocation.Extra)) != 0) return true;
            if (enemyResolvedEffectIdList.Contains(_CardId.MulcharmyNyalus) && (loc & (CardLocation.Grave | CardLocation.Removed)) != 0) return true;

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

        public bool CheckWhetherCanActivateMonsterEffect(CardAttribute attribute)
        {
            if (Bot.HintDescriptions.Contains(Util.GetStringId(CardId.DominusSpark, 3))
            && (attribute & (CardAttribute.Earth | CardAttribute.Water | CardAttribute.Fire | CardAttribute.Wind)) != 0)
            {
                return false;
            }
            if (Bot.HintDescriptions.Contains(Util.GetStringId(CardId.DominusImpulse, 2))
            && (attribute & (CardAttribute.Light | CardAttribute.Earth | CardAttribute.Wind)) != 0)
            {
                return false;
            }
            if (Bot.HintDescriptions.Contains(Util.GetStringId(CardId.DominusPurge, 2))
            && (attribute & (CardAttribute.Dark | CardAttribute.Water | CardAttribute.Fire)) != 0)
            {
                return false;
            }
            return true;
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

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = true, CardType selfType = 0)
        {
            ClientCard floodagateCard = Enemy.GetMonsters().Where(c => c?.Data != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(c))
                && c.IsFloodgate() && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (floodagateCard != null) return floodagateCard;

            ClientCard dangerCard = Enemy.MonsterZone.Where(c => c?.Data != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(c))
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (dangerCard != null) return dangerCard;

            ClientCard invincibleCard = Enemy.MonsterZone.Where(c => c?.Data != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(c))
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (invincibleCard != null) return invincibleCard;

            ClientCard equippedCard = Enemy.MonsterZone.Where(c => c?.Data != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(c))
                && c.EquipCards.Count > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)
                && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (equippedCard != null) return equippedCard;

            ClientCard enemyExtraMonster = Enemy.MonsterZone.Where(c => c != null && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(c))
                && (c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz) || (c.HasType(CardType.Link) && c.LinkCount >= 2))
                && CheckCanBeTargeted(c, canBeTarget, selfType) && CheckShouldNotIgnore(c)).OrderByDescending(card => card.Attack).FirstOrDefault();
            if (enemyExtraMonster != null) return enemyExtraMonster;

            ClientCard activatingAlbaz = Enemy.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(68468459) && !c.IsDisabled()
                && !currentNegateCardList.Contains(c) && Duel.CurrentChain.Contains(c));
            if (activatingAlbaz != null) return activatingAlbaz;

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = GetBotBestPower();
                ClientCard betterCard = Enemy.MonsterZone.Where(card => card != null
                    && card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))).OrderByDescending(card => card.Attack).FirstOrDefault();
                if (betterCard != null) return betterCard;
            }
            return null;
        }

        public bool CheckShouldNotIgnore(ClientCard cards, bool ignore = false)
        {
            return !ignore || (!currentNegateCardList.Contains(cards) && !currentNegateCardList.Contains(cards));
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !currentNegateCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count > 0) resultList.AddRange(floodagateList);

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentNegateCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count > 0) resultList.AddRange(ShuffleList(problemEnemySpellList));

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentNegateCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentNegateCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentNegateCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
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

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !currentNegateCardList.Contains(c)
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous) && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
        }

        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = true, CardType selfType = 0, bool forNegate = false)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && (!forNegate || (!card.IsDisabled() && card.HasType(CardType.Effect)))
                ).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown()
                && (!ignoreCurrentDestroy || !currentNegateCardList.Contains(card))
                && (!forNegate || (!card.IsDisabled() && card.HasType(CardType.Effect) && card.IsFaceup()))
                ).ToList()));

            return targetList;
        }

        public List<ClientCard> GetNormalEnemySpellTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = true, CardType selfType = 0, bool forNegate = false)
        {
            List<ClientCard> targetList = GetNormalEnemyTargetList(canBeTarget, ignoreCurrentDestroy, selfType, forNegate);
            return targetList.Where(card => card.HasType(CardType.Spell | CardType.Trap) || card.Location == CardLocation.SpellZone).ToList();
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
                    if (chainingCard.HasPosition(CardPosition.Defence))
                    {
                        bool have41 = Bot.MonsterZone.Any(c => CheckNumber41(c)) | Enemy.MonsterZone.Any(c => CheckNumber41(c));
                    }
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
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

        public int GetBotBestPower(bool onlyATK = false)
        {
            return Bot.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || (!summonThisTurn.Contains(card) && Duel.Player == 0) || card.IsAttack())
                .Max(card => (int?)card.GetDefensePower()) ?? -1;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain == null) {
                if (anubisTheLastJudgeSpSummoningStep > 0) {
                    List<int> cardIdList = new List<int> {
                        CardId.VerdictOfAnubis, CardId.ApophisTheSwampDeity, CardId.SongsOfTheDominators, CardId.DominusSpark, CardId.DominusImpulse,
                        CardId.ApophisTheSerpent, CardId.DominusPurge, _CardId.InfiniteImpermanence, CardId.SolemnReport };
                    foreach (int cardId in cardIdList) {
                        ClientCard card = cards.FirstOrDefault(c => c.IsCode(cardId));
                        if (card != null) {
                            if (anubisTheLastJudgeSpSummoningStep == 1)
                            {
                                anubisTheLastJudgeSpSummoningStep = 2;
                            } else 
                            {
                                anubisTheLastJudgeSpSummoningStep = 0;
                            }
                            return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                        }
                    }
                }

                // for activating target
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.Controller == 0)
                {
                    switch (lastChainCard.Id)
                    {
                        case CardId.AnubisTheLastJudge:
                        {
                            List<ClientCard> targetList = GetNormalEnemyTargetList(canBeTarget: true, ignoreCurrentDestroy: true, selfType: CardType.Monster, forNegate: false);
                            foreach (ClientCard target in targetList)
                            {
                                if (cards.Contains(target))
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            // destroy set this turn
                            foreach (ClientCard target in cards)
                            {
                                if (enemyPlaceThisTurn.Contains(target))
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            break;
                        }
                        case CardId.PrimiteDrillbeam:
                        {
                            // negate cards on chain
                            foreach (ClientCard card in Duel.CurrentChain) {
                                if (card.Controller == 1 && card.IsOnField() && card.IsFaceup() && !card.IsDisabled() && !currentNegateCardList.Contains(card)
                                    && CheckCanBeTargeted(card, true, CardType.Spell) && CheckCardShouldNegate(card) && cards.Contains(card)) {
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
                        case CardId.DominusSpark:
                        {
                            List<ClientCard> targetList = GetNormalEnemyTargetList(ignoreCurrentDestroy: false);
                            foreach (ClientCard target in targetList)
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
                        case CardId.DivineSerpentApophis:
                        {
                            if (hint == HintMsg.Destroy)
                            {
                                List<ClientCard> targetList = GetNormalEnemyTargetList();
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            if (hint == HintMsg.Set)
                            {
                                int targetId = CardId.ApophisTheSwampDeity;
                                // if no other continuous trap can be found, then use ApophisTheSerpent
                                bool hasOtherContinuousTrap = false;
                                if (Bot.GetSpellCountWithoutField() <= 3 && Bot.HasInGraveyard(CardId.ApophisTheSerpent))
                                {
                                    hasOtherContinuousTrap = true;
                                }
                                if (Bot.GetSpells().Any(c => c.HasType(CardType.Continuous)) || Bot.GetMonsters().Any(c => c.HasType(CardType.Continuous)))
                                {
                                    hasOtherContinuousTrap = true;
                                }
                                if (!hasOtherContinuousTrap)
                                {
                                    targetId = CardId.ApophisTheSerpent;
                                }
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    currentNegateCardList.Add(target);
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                                // select remaining cards
                                return Util.CheckSelectCount(cards, cards, min, max);
                            }
                            break;
                        }
                        case CardId.BaronneDeFleur:
                        {
                            if (hint == HintMsg.Destroy)
                            {
                                List<ClientCard> targetList = GetNormalEnemyTargetList();
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            break;
                        }
                        case CardId.SuperdreadnoughtRailCannonFlyingLauncher:
                        {
                            List<ClientCard> targetList = GetNormalEnemySpellTargetList(true, false, CardType.Monster);
                            if (hint == HintMsg.RemoveXyz)
                            {
                                return Util.CheckSelectCount(cards, cards, min, Math.Min(targetList.Count, max));
                            }
                            if (hint == HintMsg.Destroy)
                            {
                                List<ClientCard> destroyList = new List<ClientCard>();
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        destroyList.Add(target);
                                        if (destroyList.Count >= max)
                                        {
                                            currentNegateCardList.AddRange(destroyList);
                                            return Util.CheckSelectCount(destroyList, cards, min, Math.Min(targetList.Count, max));
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        case CardId.SPLittleKnight:
                        {
                            if (Duel.CurrentChainInfo.Count > 0)
                            {
                                ChainInfo lastChain = Duel.CurrentChainInfo[Duel.CurrentChainInfo.Count - 1];
                                if (lastChain.ActivateDescription == -1 || lastChain.ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 0))
                                {
                                    // remove enemy cards
                                    List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
                                    problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
                                    problemCardList.AddRange(Enemy.Graveyard.Where(card => card.HasType(CardType.Monster)).OrderByDescending(card => card.Attack));
                                    problemCardList.AddRange(Enemy.Graveyard.Where(card => !card.HasType(CardType.Monster)));
                                    foreach (ClientCard target in problemCardList)
                                    {
                                        if (cards.Contains(target))
                                        {
                                            currentNegateCardList.Add(target);
                                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                        }
                                    }
                                } else if (lastChain.ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 1))
                                {
                                    switch (SPLittleKnightRemoveStep)
                                    {
                                        case 1:
                                            // remove target
                                            foreach (ClientCard target in Bot.GetMonsters())
                                            {
                                                if (target.HasType(CardType.Continuous))
                                                {
                                                    continue;
                                                }
                                                if (Duel.ChainTargets.Contains(target) && cards.Contains(target))
                                                {
                                                    SPLittleKnightRemoveStep = 2;
                                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                                }
                                            }
                                            // remove weak monster
                                            if (Duel.Player == 1)
                                            {
                                                foreach (ClientCard card in Bot.GetMonsters().Where(c => c.IsAttack() && !c.HasType(CardType.Continuous) && !c.IsCode(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity })).OrderBy(c => c.Attack))
                                                {
                                                    if (Util.IsOneEnemyBetterThanValue(card.Attack, true) && cards.Contains(card))
                                                    {
                                                        SPLittleKnightRemoveStep = 2;
                                                        return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                                                    }
                                                }
                                            }
                                            break;
                                        case 2:
                                            {
                                                // select problematic enemy monster
                                                if (Enemy.GetMonsterCount() > 0)
                                                {
                                                    List<ClientCard> problemList = GetProblematicEnemyCardList(true, true, CardType.Monster);
                                                    if (problemList.Count() > 0)
                                                    {
                                                        foreach (ClientCard target in problemList)
                                                        {
                                                            if (cards.Contains(target))
                                                            {
                                                                currentNegateCardList.Add(target);
                                                                SPLittleKnightRemoveStep = 0;
                                                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                                            }
                                                        }
                                                    }
                                                }
                                                // remove bot's target
                                                foreach (ClientCard target in Duel.ChainTargets)
                                                {
                                                    if (target.IsCode(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity }))
                                                    {
                                                        continue;
                                                    }
                                                    if (cards.Contains(target))
                                                    {
                                                        SPLittleKnightRemoveStep = 0;
                                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                                    }
                                                }
                                                // remove bot's monster
                                                if (Enemy.GetMonsterCount() == 0)
                                                {
                                                    List<ClientCard> otherOwn = Bot.GetMonsters().Where(c => !c.HasType(CardType.Continuous)).ToList();
                                                    otherOwn.Sort(CompareUsableAttack);
                                                    foreach (ClientCard c in otherOwn)
                                                    {
                                                        if (cards.Contains(c))
                                                        {
                                                            SPLittleKnightRemoveStep = 0;
                                                            return Util.CheckSelectCount(new List<ClientCard> { c }, cards, min, max);
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            SPLittleKnightRemoveStep = 0;
                                            break;
                                    }
                                }
                            }
                            break;
                        }
                        case CardId.SilhouhatteRabbit:
                        {
                            if (hint == HintMsg.Destroy)
                            {
                                List<ClientCard> targetList = GetNormalEnemySpellTargetList(true, false, CardType.Monster, false);
                                foreach (ClientCard target in targetList)
                                {
                                    if (cards.Contains(target))
                                    {
                                        currentNegateCardList.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            break;
                        }
                        default:
                            break;
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
                        case CardId.PrimiteDragonEtherBeryl:
                            if (hint == HintMsg.Set)
                            {
                                int targetId = CardId.PrimiteLordlyLode;
                                if (activatedCardIdList.Contains(CardId.PrimiteLordlyLode) || !DefaultCheckWhetherBotCanSearch() || Bot.HasInSpellZone(CardId.PrimiteLordlyLode))
                                {
                                    targetId = CardId.PrimiteDrillbeam;
                                }
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            break;
                        case CardId.TheManWithTheMark:
                            if (hint == HintMsg.OperateCard)
                            {
                                int targetId = CardId.TreasuresOfTheKings;
                                if (activatedCardIdList.Contains(CardId.TreasuresOfTheKings) || Bot.HasInHandOrInSpellZone(CardId.TreasuresOfTheKings))
                                {
                                    targetId = CardId.VerdictOfAnubis;
                                }
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            break;
                        case CardId.TreasuresOfTheKings:
                        {
                            List<int> targetIdList = new List<int>();
                            if (hint == HintMsg.Set)
                            {
                                targetIdList.AddRange(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity });
                            } else if (hint == HintMsg.AddToHand) {
                                if (songsOfTheDominatorsResolvedCount > 0)
                                {
                                    targetIdList.AddRange(new List<int> { CardId.TheManWithTheMark, CardId.AnubisTheLastJudge });
                                } else 
                                {
                                    targetIdList.AddRange(new List<int> { CardId.AnubisTheLastJudge, CardId.TheManWithTheMark });
                                }
                            }
                            foreach (int targetId in targetIdList)
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            break;
                        }
                        case CardId.PrimiteLordlyLode:
                        {
                            if (hint == HintMsg.AddToHand)
                            {
                                List<int> targetIdList = new List<int> { CardId.PrimiteDragonEtherBeryl, CardId.PrimiteDrillbeam };
                                if (summonCount == 0 || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth))
                                {
                                    // whether need to search drillbeam
                                    bool canTriggerPrimiteBeam = Bot.Hand.Any(c => c.IsCode(CardId.PrimiteDragonEtherBeryl, CardId.PrimiteLordlyLode, CardId.LabradoriteDragon));
                                    canTriggerPrimiteBeam |= Bot.HasInMonstersZone(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity, CardId.LabradoriteDragon }, faceUp: true);
                                    canTriggerPrimiteBeam |= Bot.GetSpells().Any(c => c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) && (c.IsFacedown() || Duel.CurrentChain.Contains(c)));
                                    if (canTriggerPrimiteBeam)
                                    {
                                        targetIdList.Insert(0, CardId.PrimiteDrillbeam);
                                    }
                                }
                                foreach (int targetId in targetIdList)
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            break;
                        }
                        case CardId.SongsOfTheDominators:
                        {
                            List<int> checkIdList = new List<int> { CardId.DominusPurge, CardId.DominusImpulse, CardId.DominusSpark };
                            // 1. Prefer a card NOT in activatedCardIdList and not in hand / spell&trap of bot
                            foreach (int checkId in checkIdList)
                            {
                                if (!activatedCardIdList.Contains(checkId) && !Bot.HasInHandOrInSpellZone(checkId) && !Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(checkId)))
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(checkId));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            // 2. Next, prefer a card not in hand/spell&trap
                            foreach (int checkId in checkIdList)
                            {
                                if (!Bot.HasInHandOrInSpellZone(checkId))
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(checkId));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            // 3. Finally, pick by id order if any exists
                            foreach (int checkId in checkIdList)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(checkId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            break;
                        }
                        case CardId.ApophisTheSwampDeity:
                        {
                            List<ClientCard> negateCardList = new List<ClientCard>();
                            // select cards to negate
                            for (int i = Duel.CurrentChain.Count - 1; i >= 0; i--)
                            {
                                ClientCard card = Duel.CurrentChain[i];
                                if (card != null && card.IsFaceup() && cards.Contains(card))
                                {
                                    negateCardList.Add(card);
                                }
                            }
                            // negate other face-up cards
                            foreach (ClientCard card in cards)
                            {
                                if (card != null && card.IsFaceup() && !negateCardList.Contains(card))
                                {
                                    negateCardList.Add(card);
                                }
                            }
                            return Util.CheckSelectCount(negateCardList, cards, min, max);
                        }
                        case CardId.SwordsoulSupremeSovereignChengying:
                        {
                            List<ClientCard> fieldTargetList = GetNormalEnemyTargetList(false);
                            foreach (ClientCard target in fieldTargetList)
                            {
                                if (cards.Contains(target))
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            // banish fron grave
                            List<ClientCard> graveTargetList = Duel.CurrentChain.Where(c => c != null && c.Controller == 1 && c.Location == CardLocation.Grave).ToList();
                            graveTargetList.AddRange(Duel.ChainTargets.Where(c => c != null && c.Controller == 1 && c.Location == CardLocation.Grave).ToList());
                            graveTargetList.AddRange(Enemy.Graveyard.Where(c => c.IsMonster()).OrderByDescending(c => c.Attack).ToList());
                            return Util.CheckSelectCount(graveTargetList, cards, min, max);
                        }
                        case CardId.SilhouhatteRabbit:
                        {
                            List<int> targetIdList = new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity };
                            foreach (int targetId in targetIdList)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
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

        public override bool OnSelectHand()
        {
            return true;
        }

        public override int OnSelectOption(IList<int> options)
        {
            if (options.Contains(Util.GetStringId(_CardId.PotOfExtravagance, 1)))
            {
                return options.IndexOf(Util.GetStringId(_CardId.PotOfExtravagance, 1));
            }
            if (options.Contains(Util.GetStringId(CardId.SolemnReport, 1)) && options.Contains(Util.GetStringId(CardId.SolemnReport, 2)))
            {
                int destroyOpt = options.IndexOf(Util.GetStringId(CardId.SolemnReport, 1));
                int banishOpt = options.IndexOf(Util.GetStringId(CardId.SolemnReport, 2));
                bool avoidDestroyFlag = false;
                if (Duel.CurrentChain.Count >= 2)
                {
                    ClientCard secondLastChainCard = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
                    if (secondLastChainCard != null)
                    {
                        // avoid destroy if bot have the same cards that can activate
                        switch (secondLastChainCard.Id)
                        {
                            case CardId.DominusSpark:
                            {
                                bool canActivateFlag = !activatedCardIdList.Contains(CardId.DominusSpark) && !Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(CardId.DominusSpark));
                                if (canActivateFlag)
                                {
                                    canActivateFlag &= Bot.HasInHand(CardId.DominusSpark) && enemyActivateMonsterEffectFromHandGrave
                                        || Bot.GetSpells().Any(c => c.IsCode(CardId.DominusSpark) && !placedThisTurn.Contains(c));
                                }
                                avoidDestroyFlag |= canActivateFlag;
                                break;
                            }
                            case CardId.DominusImpulse:
                            case CardId.DominusPurge:
                            {
                                bool canActivateFlag = !activatedCardIdList.Contains(secondLastChainCard.Id) && !Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(secondLastChainCard.Id));
                                if (canActivateFlag)
                                {
                                    canActivateFlag &= Bot.HasInHand(secondLastChainCard.Id) || Bot.GetSpells().Any(c => c.IsCode(secondLastChainCard.Id) && !placedThisTurn.Contains(c));
                                }
                                avoidDestroyFlag |= canActivateFlag;
                                break;
                            }
                            case CardId.SongsOfTheDominators:
                            {
                                bool canActivateFlag = !activatedCardIdList.Contains(secondLastChainCard.Id) && !Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(secondLastChainCard.Id));
                                if (canActivateFlag)
                                {
                                    canActivateFlag &= Bot.HasInHand(secondLastChainCard.Id) && Bot.Graveyard.Count(c => c.IsMonster()) == 0
                                        || Bot.GetSpells().Any(c => c.IsCode(secondLastChainCard.Id) && !placedThisTurn.Contains(c));
                                }
                                avoidDestroyFlag |= canActivateFlag;
                                break;
                            }
                            case _CardId.InfiniteImpermanence:
                            case CardId.ApophisTheSwampDeity:
                                avoidDestroyFlag |= Bot.GetSpells().Any(c => c.IsCode(secondLastChainCard.Id) && !placedThisTurn.Contains(c));
                                break;
                            case CardId.ApophisTheSerpent:
                                avoidDestroyFlag |= Bot.GetSpells().Any(c => c.IsCode(secondLastChainCard.Id) && !placedThisTurn.Contains(c)) && !activatedCardIdList.Contains(secondLastChainCard.Id);
                                break;
                        }
                    
                        if (avoidDestroyFlag)
                        {
                            return banishOpt;
                        }
                        if (secondLastChainCard.IsCode(solemnReportBanishIdList))
                        {
                            return banishOpt;
                        }
                    }
                }

                return destroyOpt;
            }

            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null)
            {
                if (currentChain.ActivateController == 0)
                {
                    switch (currentChain.ActivateId)
                    {
                        case CardId.TheManWithTheMark:
                            if (options.Contains(1190))
                            {
                                return options.IndexOf(1190);
                            }
                            break;
                        default:
                            break;
                    }
                }
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
                        List<int> infiniteImpermanenceList = Bot.GetSpells().Where(c => c.IsCode(_CardId.InfiniteImpermanence)).Select(c => c.Sequence).ToList();
                        infiniteImpermanenceList.AddRange(infiniteImpermanenceNegatedColumns);
                        // trap monster do not summon to infinite permanence negated columns
                        if ((cardId == CardId.ApophisTheSerpent || cardId == CardId.ApophisTheSwampDeity)
                            && infiniteImpermanenceList.Contains(zoneId))
                        {
                            continue;
                        }
                        if (cardId == CardId.ApophisTheSerpent || cardId == CardId.ApophisTheSwampDeity)
                        {
                            Logger.DebugWriteLine("Apophis select zone: " + zoneId);
                            Logger.DebugWriteLine("infiniteImpermanenceNegatedColumns: " + string.Join(", ", infiniteImpermanenceList));
                        }
                        return zone;
                    }
                }
            }
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null && currentChain.ActivateController == 0)
            {
                switch (currentChain.ActivateId)
                {
                    case CardId.TreasuresOfTheKings:
                    {
                        List<int> zoneIdList = ShuffleList(new List<int> { 0, 1, 2, 3, 4 });
                        foreach (int zoneId in zoneIdList)
                        {
                            int zone = (int)System.Math.Pow(2, zoneId);
                            if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null)
                            {
                                return zone;
                            }
                        }
                        break;
                    }
                    case CardId.ApophisTheSerpent:
                    {
                        List<int> zoneIdList = ShuffleList(new List<int> { 0, 1, 2, 3, 4 });
                        foreach (int zoneId in zoneIdList)
                        {
                            int zone = (int)System.Math.Pow(2, zoneId);
                            if ((available & zone) != 0 && Bot.MonsterZone[zoneId] == null && !infiniteImpermanenceNegatedColumns.Contains(zoneId))
                            {
                                return zone;
                            }
                        }
                        break;
                    }
                    default:
                        break;
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.PrimiteDrillbeam, 0))
            {
                return false;
            }

            return base.OnSelectYesNo(desc);
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
                if (cardId == CardId.SuperdreadnoughtRailCannonJuggernautLiebe && CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth))
                {
                    cardAttack += 2000;
                }
                if (cardId == CardId.SwordsoulSupremeSovereignChengying)
                {
                    int removeCount = Bot.Banished.Count() + Enemy.Banished.Count();
                    cardAttack += removeCount * 200;
                }
                int bestBotAttack = Math.Max(GetBotBestPower(true), cardAttack);

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
                songsOfTheDominatorsResolvedCount = 0;
                // for doom bot
                maxSummonCount = 1;
            }

            summonCount = maxSummonCount;
            enemyActivateMonsterEffectFromHandGrave = false;
            anubisTheLastJudgeSpSummoningStep = 0;
            SPLittleKnightRemoveStep = 0;
            activatingLodeSpSummonEffect = false;
            lodeSpSummonEffectResolved = false;
            songsOfTheDominatorsActivatedFromHand = false;
            if (dimensionShifterCount > 0) dimensionShifterCount--;
            if (songsOfTheDominatorsResolvedCount > 0) songsOfTheDominatorsResolvedCount--;
            currentNegateCardList.Clear();
            activatedCardIdList.Clear();
            enemyPlaceThisTurn.Clear();
            summonThisTurn.Clear();
            placedThisTurn.Clear();
            activatedDivineSerpent1stList.Clear();
            activatedDivineSerpent2ndList.Clear();
            currentSummoningCount = 0;

            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 1 && card != null && card.IsMonster() && (card.Location == CardLocation.Hand || card.Location == CardLocation.Grave))
            {
                enemyActivateMonsterEffectFromHandGrave = true;
            }

            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (currentChain != null)
            {
                if (currentChain.ActivateController == 0)
                {
                    switch (currentChain.ActivateId)
                    {
                        case CardId.DominusSpark:
                        case CardId.DominusImpulse:
                        case CardId.DominusPurge:
                        case CardId.SongsOfTheDominators:
                            activatedCardIdList.Add(currentChain.ActivateId);
                            break;
                        case CardId.TreasuresOfTheKings:
                            if (currentChain.ActivateDescription != Util.GetStringId(CardId.TreasuresOfTheKings, 0))
                            {
                                activatedCardIdList.Add(currentChain.ActivateId);
                            }
                            break;
                    }
                }
                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentChain.ActivateController == 0)
                    {
                        switch (currentChain.ActivateId)
                        {
                            case CardId.PrimiteLordlyLode:
                            {
                                if (activatingLodeSpSummonEffect)
                                {
                                    lodeSpSummonEffectResolved = true;
                                }
                                break;
                            }
                            case CardId.SongsOfTheDominators:
                            {
                                if (songsOfTheDominatorsActivatedFromHand)
                                {
                                    songsOfTheDominatorsResolvedCount = 2;
                                }
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    if (currentChain.IsActivateCode(_CardId.DimensionShifter))
                        dimensionShifterCount = 2;
                }
            }

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            currentSummoningCount = 0;
            currentNegateCardList.Clear();
            activatingLodeSpSummonEffect = false;
            songsOfTheDominatorsActivatedFromHand = false;
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
            if (card != null)
            {
                if (currentControler == 1 && (currentLocation == (int)CardLocation.MonsterZone || currentLocation == (int)CardLocation.SpellZone))
                {
                    enemyPlaceThisTurn.Add(card);
                }

                if (previousControler == 0 && previousLocation == (int)CardLocation.MonsterZone && currentLocation != (int)CardLocation.MonsterZone)
                {
                    if (summonThisTurn.Contains(card))
                        summonThisTurn.Remove(card);
                    if (activatedDivineSerpent1stList.Contains(card))
                        activatedDivineSerpent1stList.Remove(card);
                    if (activatedDivineSerpent2ndList.Contains(card))
                        activatedDivineSerpent2ndList.Remove(card);
                }
                if (currentControler == 0 && currentLocation == (int)CardLocation.MonsterZone)
                {
                    summonThisTurn.Add(card);
                }
                if (currentControler == 0 && previousLocation == (int)CardLocation.SpellZone && currentLocation != (int)CardLocation.SpellZone && placedThisTurn.Contains(card))
                {
                    placedThisTurn.Remove(card);
                }
                if (currentControler == 0 && currentLocation == (int)CardLocation.SpellZone)
                {
                    ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
                    if (currentChain != null && currentChain.ActivateController == 0 && currentChain.ActivateId == CardId.ApophisTheSerpent && card.IsCode(CardId.ApophisTheSwampDeity))
                    {
                        placedThisTurn.Remove(card);
                    } else
                    {
                        placedThisTurn.Add(card);
                    }
                }
            }

            base.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
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
            if (card.Location == CardLocation.SpellZone)
            {
                return;
            }
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceNegatedColumns.Contains(seq)) continue;
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

        public bool AnubisTheLastJudgeActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (CheckWhetherNegated()) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            // select target on enemy field
            List<ClientCard> targetList = GetNormalEnemyTargetList(canBeTarget: true, ignoreCurrentDestroy: false, selfType: CardType.Monster, forNegate: false);
            if (targetList.Count > 0)
            {
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool AnubisTheLastJudgeSpSummon() {
            if (CheckShouldNoMoreSpSummon(CardLocation.Grave))
            {
                return false;
            }

            if (Duel.MainPhase.ActivableCards.Contains(Card))
            {
                // whether should activate
                if (!CheckWhetherNegated() || !CheckWhetherWillbeRemoved())
                {
                    return false;
                }
            }

            anubisTheLastJudgeSpSummoningStep = 1;
            return true;
        }

        public bool PrimiteDragonEtherBerylActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return !CheckWhetherNegated();
            }
            // to grave
            if (ActivateDescription == Util.GetStringId(CardId.PrimiteDragonEtherBeryl, 1))
            {
                if (!Util.IsTurn1OrMain2())
                {
                    return false;
                }

                // summoning Labradorite instead?
                bool notCalledLode = !activatedCardIdList.Contains(CardId.PrimiteLordlyLode + 1);
                if (notCalledLode)
                {
                    // can summon?
                    bool enabledLodeFlag = Bot.GetSpells().Any(c => c.IsCode(CardId.PrimiteLordlyLode) && c.IsFaceup() && !c.IsDisabled());
                    bool existsLode = Bot.GetSpells().Any(c => c.IsCode(CardId.PrimiteLordlyLode) && c.IsFacedown());
                    existsLode = Bot.HasInHand(CardId.PrimiteLordlyLode) && Bot.GetSpellCount() < 5;
                    enabledLodeFlag |= existsLode && !activatedCardIdList.Contains(CardId.PrimiteLordlyLode);

                    if (enabledLodeFlag)
                    {
                        return false;
                    }
                }

                // whether can recycle next turn?
                List<int> apophisCardIdList = new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity };
                bool recycleFlag = Bot.HasInHand(apophisCardIdList)
                    || Bot.HasInSpellZone(apophisCardIdList)
                    || Bot.GetMonsters().Any(c => c.IsFaceup() && apophisCardIdList.Contains(c.Id));
                return !CheckWhetherWillbeRemoved() && (!CheckWhetherNegated() || recycleFlag);
            } else
            {
                // search
                return !CheckWhetherNegated();
            }
        }

        public bool TheManWithTheMarkActivate()
        {
            return !CheckWhetherNegated() && DefaultCheckWhetherBotCanSearch();
        }

        public bool Level4MonsterSummon()
        {
            if (!Card.IsCode(CardId.PrimiteDragonEtherBeryl, CardId.TheManWithTheMark))
            {
                return false;
            }
            
            bool canSummonDragon = Bot.HasInHand(CardId.PrimiteDragonEtherBeryl);
            if (!activatedCardIdList.Contains(CardId.PrimiteLordlyLode) && DefaultCheckWhetherBotCanSearch())
            {
                canSummonDragon |= Bot.HasInHand(CardId.PrimiteLordlyLode) && Bot.GetSpellCountWithoutField() < 5;
                canSummonDragon |= Bot.GetSpells().Any(c => c.IsCode(CardId.PrimiteLordlyLode) && c.IsFacedown());
            }
            if (canSummonDragon)
            {
                bool summonFlag = false;
                // summon to search?
                if (!CheckWhetherNegated(true, true) && CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth))
                {
                    summonFlag |= !activatedCardIdList.Contains(CardId.PrimiteLordlyLode) && !Bot.HasInHandOrInSpellZone(CardId.PrimiteLordlyLode) && CheckRemainInDeck(CardId.PrimiteLordlyLode) > 0;
                    summonFlag |= CheckRemainInDeck(CardId.PrimiteDrillbeam) > 0;
                }

                // summon to recycle beam
                if (!Bot.HasInMonstersZone(CardId.PrimiteDragonEtherBeryl, faceUp: true) && !activatedCardIdList.Contains(CardId.PrimiteDrillbeam + 1)
                    && Bot.HasInGraveyard(CardId.PrimiteDrillbeam) && Bot.GetSpellCountWithoutField() < 5)
                {
                    summonFlag = true;
                }

                if (summonFlag && Card.IsCode(CardId.PrimiteDragonEtherBeryl))
                {
                    summonCount --;
                    return true;
                }
            }

            bool canSummonMan = Bot.HasInHand(CardId.TheManWithTheMark);
            canSummonMan |= Bot.HasInHand(CardId.AnubisTheLastJudge) && DefaultCheckWhetherBotCanSearch() && CheckRemainInDeck(CardId.TheManWithTheMark) > 0 && !activatedCardIdList.Contains(CardId.AnubisTheLastJudge);
            if (Bot.HasInHandOrInSpellZone(CardId.TreasuresOfTheKings) && !activatedCardIdList.Contains(CardId.TreasuresOfTheKings + 1) && DefaultCheckWhetherBotCanSearch() && CheckRemainInDeck(CardId.TheManWithTheMark) > 0)
            {
                canSummonMan |= Bot.Graveyard.Any(c => c.IsTrap());
                int facedownCardCount = Bot.GetSpells().Count(c => c.IsFacedown());
                facedownCardCount += Bot.GetMonsters().Count(c => c.IsFacedown());
                facedownCardCount += GetCanSetSpells().Count();
                canSummonMan |= facedownCardCount >= 2;
            }
            if (canSummonMan && Card.IsCode(CardId.TheManWithTheMark))
            {
                summonCount --;
                return true;
            }

            // summon for synchro level 10 monster
            if (Bot.ExtraDeck.Any(c => c.HasType(CardType.Synchro) && c.Level == 10))
            {
                // have labradorite dragon
                bool haveTunerFlag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(CardId.LabradoriteDragon) && c.IsTuner());
                haveTunerFlag |= !activatedCardIdList.Contains(CardId.PrimiteLordlyLode + 1) && Bot.HasInSpellZone(CardId.PrimiteLordlyLode, true, true);
                if (haveTunerFlag)
                {
                    summonCount --;
                    return true;
                }
            }

            return false;
        }

        public bool PotOfExtravaganceActivate()
        {
            if (CheckWhetherNegated()) return false;
            SelectSTPlace(Card, true);
            AI.SelectOption(1);
            return true;
        }

        public bool TerraformingActivate()
        {
            if (CheckWhetherNegated()) return false;
            SelectSTPlace(Card, true);
            return true;
        }

        public bool PrimiteDrillbeamActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated()) return false;

                // check whether can active
                bool canActivate = Bot.HasInHand(CardId.PrimiteLordlyLode) || Bot.HasInHand(CardId.PrimiteDragonEtherBeryl);
                canActivate |= Bot.HasInSpellZone(CardId.ApophisTheSerpent) || Bot.HasInSpellZone(CardId.ApophisTheSwampDeity);
                canActivate |= Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity, CardId.LabradoriteDragon));
                canActivate |= Bot.HasInHand(CardId.ApophisTheSwampDeity) && Bot.GetSpellCountWithoutField() <= 3;

                if (canActivate) {
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
            if (problematicEnemyCardList.Count(c => !c.IsDisabled()) > 0) {
                problematicEnemyCardList.RemoveAll(c => currentNegateCardList.Contains(c));
                if (problematicEnemyCardList.Count > 0) {
                    activateFlag = true;
                }
            }

            // negate cards on chain
            foreach (ClientCard card in Duel.CurrentChain) {
                if (card.Controller == 1 && card.IsOnField() && card.IsFaceup() && !card.IsDisabled() && !currentNegateCardList.Contains(card)
                    && CheckCanBeTargeted(card, true, CardType.Spell) && CheckCardShouldNegate(card)) {
                    activateFlag = true;
                }
            }

            // can recycle, so activate it
            if (Bot.HasInMonstersZone(CardId.PrimiteDragonEtherBeryl, faceUp: true) && !activatedCardIdList.Contains(CardId.PrimiteDrillbeam + 1)
                && (CurrentTiming & hintToHand) == 0)
            {
                List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Spell, true);
                if (targetList.Count > 0) {
                    activateFlag = true;
                }
            }

            // become target
            if (DefaultOnBecomeTarget()) {
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
            if (Bot.HasInHandOrHasInMonstersZone(CardId.PrimiteDragonEtherBeryl) && DefaultCheckWhetherBotCanSearch())
            {
                // for search drillbeam
                activateFlag |= CheckRemainInDeck(CardId.PrimiteDrillbeam) > 0;
                activateFlag |= summonCount <= 0 && Card.Location == CardLocation.SpellZone && Card.IsFacedown();
            }
            if (summonCount > 0 && !Bot.HasInHand(CardId.PrimiteDragonEtherBeryl) && CheckRemainInDeck(CardId.PrimiteDragonEtherBeryl) > 0 && DefaultCheckWhetherBotCanSearch())
            {
                // for search ether beryl
                activateFlag |= Bot.HasInGraveyard(CardId.PrimiteDrillbeam);
                activateFlag |= CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth) && !CheckWhetherNegated(true, true, CardType.Monster);
            }
            if (!Bot.HasInSpellZone(CardId.PrimiteLordlyLode, true, true))
            {
                // for activate it
                activateFlag |= DefaultCheckWhetherBotCanSearch();

                // for special summon
                CardLocation loc;
                if (Bot.HasInHand(CardId.LabradoriteDragon))
                {
                    loc = CardLocation.Hand;
                }
                else if (CheckRemainInDeck(CardId.LabradoriteDragon) > 0)
                {
                    loc = CardLocation.Deck;
                }
                else if (Bot.HasInGraveyard(CardId.LabradoriteDragon))
                {
                    loc = CardLocation.Grave;
                } else {
                    return false;
                }
                int drawCount = GetSpecialSummonDrawCount(loc);
                if (drawCount < 2)
                {
                    activateFlag |= Bot.GetMonsters().Any(c => c.IsFaceup() && c.Level == 4 && !c.HasType(CardType.Xyz | CardType.Link))
                        && Bot.ExtraDeck.Any(c => c.IsFaceup() && c.Level == 10 && c.HasType(CardType.Synchro));
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
            // special summon
            CardLocation loc;
            if (Bot.HasInHand(CardId.LabradoriteDragon))
            {
                loc = CardLocation.Hand;
            }
            else if (CheckRemainInDeck(CardId.LabradoriteDragon) > 0)
            {
                loc = CardLocation.Deck;
            }
            else if (Bot.HasInGraveyard(CardId.LabradoriteDragon))
            {
                loc = CardLocation.Grave;
            } else {
                return false;
            }
            int drawCount = GetSpecialSummonDrawCount(loc);
            return drawCount < 2;
        }

        public bool TreasuresOfTheKingsActivate()
        {
            if (CheckWhetherNegated()) return false;

            // search
            if (ActivateDescription == Util.GetStringId(CardId.TreasuresOfTheKings, 0))
            {
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            // activate
            bool activateFlag = false;
            if (Bot.GetSpellCountWithoutField() == 5)
            {
                // for search
                if (!DefaultCheckWhetherBotCanSearch() || CheckRemainInDeck(CardId.TheManWithTheMark, CardId.AnubisTheLastJudge) == 0)
                {
                    activateFlag = false;
                }
                else
                {
                    bool hasOtherSpellTrapInHand =
                        Bot.Hand.Any(c => (c.IsSpell() || c.IsTrap()) && c != Card);

                    bool gyHasTrap =
                        Bot.Graveyard.Any(c => c.IsTrap());

                    int otherFacedownOnField = 
                        Bot.GetMonsters().Count(m => m.IsFacedown())
                        + Bot.GetSpells().Count(s => s.IsFacedown() && s != Card);

                    activateFlag = gyHasTrap || hasOtherSpellTrapInHand || otherFacedownOnField >= 2;
                }
            }
            else
            {
                // for set
                if (CheckRemainInDeck(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) > 0)
                    activateFlag = true;
            }

            return activateFlag;
        }

        public bool DominusSparkActivate()
        {
            if (CheckWhetherNegated()) return false;
            // activate from field first
            if (Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.DominusSpark) && c.IsOnField() && c != Card
                && !infiniteImpermanenceNegatedColumns.Contains(c.Sequence)))
            {
                return false;
            }

            bool shouldActivate = false;
            // check whether there is a dangerous monster
            ClientCard dangerousMonster = GetProblematicEnemyMonster(-1, true, false, CardType.Trap);            
            shouldActivate |= dangerousMonster != null;

            if (Duel.Phase == DuelPhase.BattleStep && Duel.Player == 1)
            {
                dangerousMonster = GetProblematicEnemyMonster(0, true, false, CardType.Trap);
                shouldActivate |= dangerousMonster != null;
            }

            // check whether it is the end phase and there is a monster on the enemy field
            if (Duel.Phase == DuelPhase.End && Duel.Player == 1 && Duel.Turn == 1)
            {
                List<ClientCard> endPhaseTargets = GetNormalEnemyTargetList(canBeTarget: true, ignoreCurrentDestroy: true, selfType: CardType.Trap);
                shouldActivate |= endPhaseTargets.Any(c => c.IsMonster() && c.IsFaceup());
            }

            shouldActivate |= DefaultOnBecomeTarget() && !CheckWhetherNegated();

            if (shouldActivate)
            {
                SelectSTPlace(Card, true);
                return true;
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
                int this_seq = Card.Sequence;
                int that_seq = -1;
                if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone) that_seq = LastChainCard.Sequence;
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || Util.IsChainTarget(Card)
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    return true;
                }
            }

            // negate monster
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true, CardType.Trap);
            if (shouldNegateList.Count > 0)
            {
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }

        public bool DominusNegateTrapActivate()
        {
            if (CheckWhetherNegated()) return false;
            // activate from field first
            if (Duel.MainPhase.ActivableCards.Any(c => c.IsCode(Card.Id) && c.IsOnField() && c != Card
                && !infiniteImpermanenceNegatedColumns.Contains(c.Sequence)))
            {
                return false;
            }

            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && LastChainCard.Controller == 1 && CheckCardShouldNegate(LastChainCard))
            {
                currentNegateCardList.Add(LastChainCard);
                SelectSTPlace(Card, true);
                if (Card.IsCode(CardId.SongsOfTheDominators) && Card.Location == CardLocation.Hand)
                {
                    songsOfTheDominatorsActivatedFromHand = true;
                }
                return true;
            }

            return false;
        }

        public bool SongsOfTheDominatorsActivateFirst()
        {
            return Bot.Graveyard.Any(c => c.HasType(CardType.Trap)) && DominusNegateTrapActivate();
        }

        public bool ApophisTheSwampDeityActivate()
        {
            return ApophisTheSwampDeityActivateCheck(1);
        }

        public bool ApophisTheSwampDeityActivateForAttack()
        {
            return ApophisTheSwampDeityActivateCheck(2);
        }

        public bool ApophisTheSwampDeityActivateCheck(int activatePriority = 0)
        {
            if (CheckWhetherNegated()) return false;
            if (currentSummoningCount + Bot.GetMonsters().Count(c => c.Sequence < 5) >= 5) return false;

            int canNegateCount = Bot.GetSpells().Count(c => c != Card &&
                (c.IsCode(CardId.ApophisTheSwampDeity)
                    || (c.IsCode(CardId.ApophisTheSerpent) &&
                        (c.IsFaceup() || !activatedCardIdList.Contains(CardId.ApophisTheSerpent)))));
            canNegateCount += Bot.GetMonsters().Count(c => c.IsCode(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity }) && c.IsFaceup());
            
            bool shouldActivate = false;
            // for negate
            if (Duel.CurrentChain.Count > 0 && canNegateCount > 0)
            {
                // exists cards to negate?
                foreach (ClientCard chain in Duel.CurrentChain)
                {
                    if (chain.IsFaceup() && chain.IsOnField() && chain.Controller == 1 && !currentNegateCardList.Contains(chain))
                    {
                        Logger.DebugWriteLine("[ApophisTheSwampDeity] Negate card on chain: " + chain.Name);
                        shouldActivate = true;
                        break;
                    }
                }
            }

            // for summon divine serpent
            if (DivineSerpentApophisSpSummonCheck()) {
                int newApophisCount = Duel.MainPhase.ActivableCards.Count(c => c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity));
                int currentApophisCount = Bot.GetMonsters().Count(c => c.IsFaceup() &&
                    (c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) || (c.IsCode(CardId.DivineSerpentApophis) && activatedDivineSerpent1stList.Contains(c))));
                bool checkResult = currentApophisCount < 2 && currentApophisCount + newApophisCount >= 2;
                if (checkResult)
                {
                    Logger.DebugWriteLine("[ApophisTheSwampDeity] DivineSerpentApophisSpSummonCheck: " + checkResult);
                }
                shouldActivate |= checkResult;
            }

            // for little knight
            if (Bot.ExtraDeck.Any(c => c.IsCode(CardId.SPLittleKnight)) && !SPLittleKnightSummonCheck() && SPLittleKnightSummonCheck(true)
            && Duel.Player == 0 && Bot.ExtraDeck.Any(c => c.IsCode(CardId.LinkSpider)))
            {
                bool checkResult = Bot.GetMonsters().Count(c => c.IsFaceup() && c.HasType(CardType.Effect)) == 1;
                if (checkResult)
                {
                    Logger.DebugWriteLine("[ApophisTheSwampDeity] SPSmallKnightSummonCheck: " + checkResult);
                }
                shouldActivate |= checkResult;
            }

            // for triggering divine serpent apophis
            if (CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth) && !CheckWhetherNegated(true, true, CardType.Monster)
                && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && c.IsCode(CardId.DivineSerpentApophis) && !activatedDivineSerpent2ndList.Contains(c)))
            {
                bool checkResult = GetProblematicEnemyCardList(true, false, CardType.Monster).Count() > 0
                    && !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity))
                    && !Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.ApophisTheSerpent));
                if ((CurrentTiming & hintTimingMainEnd) != 0 && Duel.Player == 1)
                {
                    checkResult |= GetNormalEnemyTargetList(true, true, CardType.Monster).Count() > 0 
                        && !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity))
                        && !Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.ApophisTheSerpent));
                }
                if (checkResult)
                {
                    Logger.DebugWriteLine("[ApophisTheSwampDeity] DivineSerpentApophisActivate: " + checkResult);
                }
                shouldActivate |= checkResult;
            }

            // for triggering silhouette hat rabbit
            if (CheckWhetherCanActivateMonsterEffect(CardAttribute.Light) && !CheckWhetherNegated(true, true, CardType.Monster, true)
                && !activatedCardIdList.Contains(CardId.SilhouhatteRabbit + 1)
                && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && c.IsCode(CardId.SilhouhatteRabbit)))
            {
                bool checkResult = GetProblematicEnemyCardList(true, false, CardType.Monster).Count(c => c.HasType(CardType.Spell | CardType.Trap)) > 0 
                    && !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity))
                    && !Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.ApophisTheSerpent));
                if ((CurrentTiming & hintTimingMainEnd) != 0 && Duel.Player == 1)
                {
                    checkResult |= GetNormalEnemySpellTargetList(true, true, CardType.Monster).Count() > 0 
                        && !Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity))
                        && !Duel.MainPhase.ActivableCards.Any(c => c.IsCode(CardId.ApophisTheSerpent));
                }
                if (checkResult)
                {
                    Logger.DebugWriteLine("[ApophisTheSwampDeity] SilhouhatteRabbitSummonCheck: " + checkResult);
                }
                shouldActivate |= checkResult;
            }

            // for triggering primite drillbeam
            if (Bot.HasInHandOrInSpellZone(CardId.PrimiteDrillbeam) && !activatedCardIdList.Contains(CardId.PrimiteDrillbeam))
            {
                // whether have other cards to trigger primite beam
                bool canTriggerPrimiteBeam = Bot.Hand.Any(c => c.IsCode(CardId.PrimiteDragonEtherBeryl, CardId.PrimiteLordlyLode, CardId.LabradoriteDragon));
                canTriggerPrimiteBeam |= Bot.HasInMonstersZone(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity, CardId.LabradoriteDragon }, faceUp: true);
                canTriggerPrimiteBeam |= Bot.GetSpells().Any(c => c != Card && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) && (c.IsFacedown() || Duel.CurrentChain.Contains(c)));
                
                bool checkResult = !canTriggerPrimiteBeam;
                if (checkResult)
                {
                    Logger.DebugWriteLine("[ApophisTheSwampDeity] PrimiteDrillbeamSummonCheck: " + checkResult);
                }
                shouldActivate |= !canTriggerPrimiteBeam;
            }

            // TODO for other summon

            // for attack
            if (Duel.Turn > 1 && Duel.Phase == DuelPhase.Main1 && Enemy.GetMonsterCount() == 0 && Duel.Player == 0 && Duel.CurrentChain.Count == 0
                && activatePriority >= 2)
            {
                Logger.DebugWriteLine("[ApophisTheSwampDeity] Attack: " + true);
                shouldActivate |= true;
            }

            // for defense
            if (Duel.Phase == DuelPhase.Main1 && Bot.GetMonsterCount() == 0 && Enemy.GetMonsters().Sum(c => c.Attack) >= Bot.LifePoints
                && (CurrentTiming & hintTimingMainEnd) != 0)
            {
                Logger.DebugWriteLine("[ApophisTheSwampDeity] Defense: " + true);
                shouldActivate |= true;
            }

            if (shouldActivate)
            {
                // mark cards to negate
                int negateCount = Bot.GetSpells().Count(c => c != Card && c.IsFaceup() && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap));
                negateCount += Bot.GetMonsters().Count(c => c.IsFaceup() && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap));
                // negate cards on chain fist
                if (negateCount > 0)
                {
                    foreach (ClientCard chain in Duel.CurrentChain)
                    {
                        if (chain.IsFaceup() && chain.IsOnField() && !currentNegateCardList.Contains(chain) && CheckCardShouldNegate(chain))
                        {
                            negateCount--;
                            currentNegateCardList.Add(chain);
                            if (negateCount <= 0) break;
                        }
                    }
                }

                if (negateCount > 0)
                {
                    // Find negatable enemy spell/trap cards on field that are not yet negated
                    List<ClientCard> negatableSpellsTraps = Enemy.GetSpells().Where(c =>
                        c.IsFaceup() &&
                        !c.IsDisabled() &&
                        !currentNegateCardList.Contains(c)
                    ).ToList();

                    // Mark negatable spell/trap cards as negated, up to the available count
                    foreach (ClientCard card in negatableSpellsTraps)
                    {
                        currentNegateCardList.Add(card);
                        negateCount--;
                        if (negateCount <= 0) break;
                    }
                }

                if (negateCount > 0)
                {
                    // Find negatable enemy monsters on field that are not yet negated
                    List<ClientCard> negatableMonsters = Enemy.GetMonsters().Where(c =>
                        c.IsFaceup() &&
                        !c.IsDisabled() &&
                        !currentNegateCardList.Contains(c)
                    ).ToList();

                    // Mark negatable monsters as negated, up to the remaining count
                    foreach (ClientCard card in negatableMonsters)
                    {
                        currentNegateCardList.Add(card);
                        negateCount--;
                        if (negateCount <= 0) break;
                    }
                }

                currentSummoningCount++;
                return true;
            }

            return false;
        }

        public bool ApophisTheSerpentActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (currentSummoningCount + Bot.GetMonsters().Count(c => c.Sequence < 5) >= 5) return false;
            if (Bot.GetSpells().Any(c => c.IsFacedown() && c.IsCode(CardId.ApophisTheSwampDeity) && !infiniteImpermanenceNegatedColumns.Contains(c.Sequence)))
            {
                if (!(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                {
                    return false;
                }
            }
            currentSummoningCount++;
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool SpellNegateActivate()
        {
            if (CheckWhetherNegated() || Duel.LastChainPlayer != 1) return false;
            
            ClientCard prevChainCard = Util.GetLastChainCard();
            if (prevChainCard != null && !CheckCardShouldNegate(prevChainCard))
            {
                return false;
            }
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool SolemnReportBanishActivate()
        {
            if (SpellNegateActivate())
            {
                ClientCard prevChainCard = Util.GetLastChainCard();
                if (prevChainCard != null && prevChainCard.IsCode(solemnReportBanishIdList))
                {
                    return true;
                }
            }
            return false;
        }

        public bool DivineSerpentApophisSpSummonCheck()
        {
            if (Duel.Player != 0) return false;
            bool checkFlag = false;
            // recycle
            if (CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth) && !CheckWhetherNegated(true, true, CardType.Monster)
                && Bot.GetSpellCountWithoutField() < 5)
            {
                checkFlag |= Bot.HasInGraveyard(new List<int> { CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity });
                checkFlag |= !CheckWhetherWillbeRemoved();
            }

            // for attack
            checkFlag |= GetBotBestPower(true) <= Util.GetBestPower(Enemy) && Util.GetBestPower(Enemy) < 2800;

            return checkFlag;
        }

        public bool DivineSerpentApophisSpSummon()
        {
            if (!DivineSerpentApophisSpSummonCheck()) return false;
            // select material with different name
            ClientCard apophisTheSerpent = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup() && c.IsCode(CardId.ApophisTheSerpent));
            ClientCard apophisTheSwampDeity = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup() && c.IsCode(CardId.ApophisTheSwampDeity));
            AI.SelectMaterials(new List<ClientCard> { apophisTheSerpent, apophisTheSwampDeity }, HintMsg.Release);
            return true;
        }

        public bool DivineSerpentApophisActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.DivineSerpentApophis, 0))
            {
                // set trap
                activatedDivineSerpent1stList.Add(Card);
            }
            else
            {
                // destroy
                List<ClientCard> targetList = GetNormalEnemyTargetList(true, false, CardType.Monster);
                if (targetList.Count() == 0)
                {
                    return false;
                }
                activatedDivineSerpent2ndList.Add(Card);
            }
            return true;
        }

        public bool Level10SynchroSummon()
        {
            if (!Level10SynchroSummonCheck()) return false;
            if (!Card.IsCode(new List<int> { CardId.BaronneDeFleur, CardId.SwordsoulSupremeSovereignChengying }))
            {
                return false;
            }

            int decideSummonId = 0;
            // select which to summon
            if (GetBotBestPower(true) < Util.GetBestPower(Enemy) && Util.GetBestPower(Enemy) > 3000
                && (!CheckWhetherCanActivateMonsterEffect(CardAttribute.Wind) || lodeSpSummonEffectResolved)
                && Duel.Phase == DuelPhase.Main1 && Duel.Turn > 1)
            {
                int banishCount = Bot.Banished.Count() + Enemy.Banished.Count();
                if (3000 + banishCount * 200 >= Util.GetBestPower(Enemy)
                    && Duel.MainPhase.SummonableCards.Any(c => c.IsCode(CardId.SwordsoulSupremeSovereignChengying))
                    && !Card.IsCode(CardId.SwordsoulSupremeSovereignChengying))
                {
                    return false;
                }
                decideSummonId = CardId.SwordsoulSupremeSovereignChengying;
            }
            if (decideSummonId == 0 && CheckWhetherCanActivateMonsterEffect(CardAttribute.Wind)
                && Duel.MainPhase.SummonableCards.Any(c => c.IsCode(CardId.BaronneDeFleur))
                && !Card.IsCode(CardId.BaronneDeFleur))
            {
                return false;
            }
            if (decideSummonId == 0 && !CheckWhetherCanActivateMonsterEffect(CardAttribute.Wind)
                && Duel.MainPhase.SummonableCards.Any(c => c.IsCode(CardId.SwordsoulSupremeSovereignChengying))
                && !Card.IsCode(CardId.SwordsoulSupremeSovereignChengying))
            {
                return false;
            }

            ClientCard level4Monster = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup() && c.Level == 4 && c.IsCode(CardId.PrimiteDragonEtherBeryl));

            if (level4Monster == null)
            {
                // find level4 monster with lowest power
                level4Monster = Bot.GetMonsters().OrderBy(c => c.GetDefensePower()).FirstOrDefault(c => c.IsFaceup() && c.Level == 4 && !c.HasType(CardType.Xyz | CardType.Link));
            }
            if (level4Monster == null)
            {
                return false;
            }
            
            ClientCard level6Tuner = Bot.GetMonsters().OrderBy(c => c.GetDefensePower()).FirstOrDefault(c => c.IsFaceup() && c.Level == 6 && c.IsTuner());
            if (level6Tuner == null)
            {
                return false;
            }
            AI.SelectMaterials(new List<ClientCard> { level4Monster, level6Tuner });
            return true;
        }

        public bool Level10SynchroSummonCheck()
        {
            if (CheckShouldNoMoreSpSummon()) return false;
            if (GetSpecialSummonDrawCount(CardLocation.Extra) > 1 && CheckAtAdvantage()) return false;

            return true;
        }

        public bool SwordsoulSupremeSovereignChengyingActivate()
        {
            if (CheckWhetherNegated()) return false;
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool BaronneDeFleurNegateEffect()
        {
            if (ActivateDescription != Util.GetStringId(CardId.BaronneDeFleur, 1))
            {
                return false;
            }
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 1 && lastChainCard != null)
            {
                if (CheckAtAdvantage() && lastChainCard.IsCode(new List<int> {_CardId.MaxxC, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus, _CardId.MulcharmyPurulia}))
                {
                    return false;
                }
                if (Duel.LastChainTargets.Contains(Card) && lastChainCard.IsCode(_CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.BreakthroughSkill))
                {
                    return false;
                }
            }
            currentNegateCardList.Add(lastChainCard);
            return true;
        }

        public bool BaronneDeFleurActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BaronneDeFleur, 1))
            {
                // negate
                return false;
            } else if (Duel.Phase == DuelPhase.Standby)
            {
                // special summon
                // TODO waiting for handle OnCardHint
                return false;
            } else {
                // destroy
                List<ClientCard> targetList = GetNormalEnemyTargetList();
                if (targetList.Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool SuperdreadnoughtRailCannonJuggernautLiebeActivate()
        {
            if (CheckWhetherNegated()) return false;
            return true;
        }

        public bool SuperdreadnoughtRailCannonJuggernautLiebeSpSummon()
        {
            if (!SuperdreadnoughtRailCannonJuggernautLiebeSpSummonCheck()) return false;
            return true;
        }

        public bool SuperdreadnoughtRailCannonJuggernautLiebeSpSummonCheck()
        {
            int enemyPower = Util.GetBestPower(Enemy);
            int botPower = GetBotBestPower(true);
            if (botPower < enemyPower)
            {
                int currentAttack = 4000;
                if (!CheckWhetherNegated(true, true, CardType.Monster) && CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth) && !lodeSpSummonEffectResolved)
                {
                    currentAttack += 2000;
                }
                if (currentAttack >= enemyPower)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SuperdreadnoughtRailCannonFlyingLauncherActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SuperdreadnoughtRailCannonFlyingLauncher, 2))
            {
                List<ClientCard> targetList = GetNormalEnemySpellTargetList(true, false, CardType.Monster);
                if (targetList.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SuperdreadnoughtRailCannonFlyingLauncherSpSummon()
        {
            bool hasLiebeInExtra = Bot.ExtraDeck.Any(c => c.IsCode(CardId.SuperdreadnoughtRailCannonJuggernautLiebe));
            int enemyPower = Util.GetBestPower(Enemy);
            int botPower = GetBotBestPower(true);
            bool needSummonLiebe = SuperdreadnoughtRailCannonJuggernautLiebeSpSummonCheck();

            if ((hasLiebeInExtra && needSummonLiebe)
                || (!hasLiebeInExtra && botPower < enemyPower && enemyPower <= 3800)
                || (CheckWhetherCanActivateMonsterEffect(CardAttribute.Earth)
                    && !CheckWhetherNegated(true, true, CardType.Monster)
                    && !lodeSpSummonEffectResolved
                    && GetNormalEnemySpellTargetList(true, false, CardType.Monster).Count() > 0))
            {
                return true;
            }
            return false;
        }

        public bool EvilswarmExcitonKnightSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Light)) return false;
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && DefaultEvilswarmExcitonKnightEffect();
        }

        public bool SPLittleKnightActivate()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 0))
            {
                // banish card
                List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
                problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => card.HasType(CardType.Monster)).OrderByDescending(card => card.Attack));
                problemCardList.AddRange(Enemy.Graveyard.Where(card => !card.HasType(CardType.Monster)));
                if (problemCardList.Count() > 0)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            } else if (ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 1))
            {
                ClientCard selfMonster = null;
                foreach (ClientCard target in Bot.GetMonsters())
                {
                    if (Duel.ChainTargets.Contains(target))
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
                    }
                    if (Enemy.GetMonsterCount() > 0)
                    {
                        nextMonster = GetProblematicEnemyMonster(0, true, false, CardType.Monster);
                    }
                    if (nextMonster != null)
                    {
                        SPLittleKnightRemoveStep = 1;
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool SPLittleKnightSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster, true) || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Dark)) return false;

            List<ClientCard> effectMonsters = Bot.GetMonsters().Where(c => c.IsFaceup() && c.HasType(CardType.Effect)
                && (!c.IsCode(CardId.SilhouhatteRabbit) || !summonThisTurn.Contains(c))
                ).OrderBy(c => c.GetDefensePower()).ToList();

            // summon to remove problem cards
            if (SPLittleKnightSummonCheck() || Util.IsTurn1OrMain2() && (Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.Graveyard.Count > 0))
            {
                ClientCard extraMonster = null;
                ClientCard otherMonster = null;
                foreach (ClientCard monster in effectMonsters)
                {
                    // skip important monster if needed
                    if (Util.IsTurn1OrMain2() && monster.IsCode(new List<int> { CardId.BaronneDeFleur, CardId.SwordsoulSupremeSovereignChengying, CardId.DivineSerpentApophis }))
                    {
                        continue;
                    }

                    if (monster.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
                    {
                        if (extraMonster != null)
                        {
                            otherMonster = monster;
                        }
                        else
                        {
                            extraMonster = monster;
                        }
                    }
                    else
                    {
                        otherMonster = monster;
                    }

                    if (extraMonster != null && otherMonster != null)
                    {
                        break;
                    }
                }

                if (extraMonster != null && otherMonster != null)
                {
                    AI.SelectMaterials(new List<ClientCard> { extraMonster, otherMonster });
                    return true;
                }
            }

            return false;
        }

        public bool SPLittleKnightSummonCheck(bool ignoreMaterialCheck = false)
        {
            if (CheckWhetherNegated(true, true, CardType.Monster, true)
                || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Dark)
                || lodeSpSummonEffectResolved) return false;
            // banish card
            List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster, ignoreSpells: !Util.IsTurn1OrMain2());
            bool summonFlag = problemCardList.Count() > 0;
            ClientCard problematicEnemyMonster = GetProblematicEnemyMonster(0, true, false, CardType.Monster);
            if (problematicEnemyMonster != null)
            {
                summonFlag = true;
            }
            if (summonFlag)
            {
                if (ignoreMaterialCheck)
                {
                    return true;
                }
                // check material
                return Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)
                    && (!c.IsCode(CardId.SilhouhatteRabbit) || !summonThisTurn.Contains(c)))
                    && Bot.GetMonsters().Count(c => c.IsFaceup() && c.HasType(CardType.Effect)) >= 2;
            }
            
            return false;
        }

        public bool SilhouhatteRabbitActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.SilhouhatteRabbit, 0) || ActivateDescription == -1)
            {
                // set
                // default to true
                activatedCardIdList.Add(Card.Id);
                return true;
            } else
            {
                // destroy
                List<ClientCard> targetList = GetNormalEnemySpellTargetList(true, false, CardType.Monster, false);
                if (targetList.Count() > 0)
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            }
            return false;
        }

        public bool SilhouhatteRabbitSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster, true) || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Light)) return false;
            if (!SilhouhatteRabbitSummonCheck()) return false;
            // select material
            ClientCard anubis = Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup() && c.IsCode(CardId.AnubisTheLastJudge));
            List<ClientCard> materialList = new List<ClientCard> {  };
            if (anubis != null)
            {
                materialList.Add(anubis);
            }
            // select remain material
            List<ClientCard> effectMonsters = Bot.GetMonsters().Where(c => c.IsFaceup() && c.HasType(CardType.Effect)
                && (!c.IsCode(CardId.SilhouhatteRabbit) || !summonThisTurn.Contains(c))
                ).OrderBy(c => c.GetDefensePower()).ToList();
            if (effectMonsters.Count() > 0)
            {
                foreach (ClientCard monster in effectMonsters)
                {
                    // skip important monster if needed
                    if (monster.IsCode(new List<int> { CardId.BaronneDeFleur, CardId.SwordsoulSupremeSovereignChengying, CardId.DivineSerpentApophis, CardId.SPLittleKnight }))
                    {
                        continue;
                    }
                    materialList.Add(monster);
                    if (materialList.Count() >= 2)
                    {
                        break;
                    }
                }
            }

            if (materialList.Count() >= 2)
            {
                AI.SelectMaterials(materialList);
                return true;
            }
            return false;
        }

        public bool SilhouhatteRabbitSummonCheck(bool ignoreMaterialCheck = false)
        {
            if (CheckWhetherNegated(true, true, CardType.Monster, true)
                || !CheckWhetherCanActivateMonsterEffect(CardAttribute.Light)
                || lodeSpSummonEffectResolved) return false;

            List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
            if (problemCardList.Count() > 0)
            {
                return false;
            }
            int remainApophisCount = CheckRemainInDeck(CardId.ApophisTheSwampDeity);
            if (remainApophisCount == 0)
            {
                return false;
            }
            if (Bot.Hand.Any(c => c.IsCode(CardId.ApophisTheSwampDeity)) || Bot.GetSpells().Any(c => c.IsCode(CardId.ApophisTheSwampDeity) && c.IsFacedown()))
            {
                return false;
            }
            if (activatedCardIdList.Contains(CardId.SilhouhatteRabbit))
            {
                return false;
            }
            if (ignoreMaterialCheck)
            {
                return true;
            }
            return Bot.GetMonsters().Count(c => c.IsFaceup() && c.HasType(CardType.Effect)
                && (!c.IsCode(CardId.SilhouhatteRabbit) || !summonThisTurn.Contains(c))) >= 2;
        }

        public bool LinkSpiderSummon()
        {
            // check whether monster is enough
            int monsterCount = Bot.GetMonsterCount() + Duel.MainPhase.ActivableCards.Count(c => c.IsOnField() && c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity));
            if (monsterCount < 2)
            {
                return false;
            }

            // summon little knight check
            if (Bot.ExtraDeck.Any(c => c.IsCode(CardId.SPLittleKnight)))
            {
                if (SPLittleKnightSummonCheck())
                {
                    return false;
                }
                if (SPLittleKnightSummonCheck(true))
                {
                    return true;
                }
            }
            // summon silhouhatte rabbit check
            if (Bot.ExtraDeck.Any(c => c.IsCode(CardId.SilhouhatteRabbit)))
            {
                if (SilhouhatteRabbitSummonCheck())
                {
                    return false;
                }
                if (SilhouhatteRabbitSummonCheck(true))
                {
                    return true;
                }
            }
            return false;
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

        public bool MonstetReposForImportantMonsters()
        {
            if (!Card.IsFacedown())
            {
                return false;
            }
            return Card.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz);
        }

        public bool SpellSetFirst()
        {
            // pretend to have solemn report
            int setCount = Bot.GetSpells().Count(c => c.IsFacedown() && c.Sequence < 5);
            if (setCount < 2 && Bot.LifePoints > 1500)
            {
                if (setCount == 1 && !Bot.HasInSpellZone(CardId.SolemnReport) && Bot.HasInHand(CardId.SolemnReport) && Bot.GetSpells().Any(c => c.IsTrap() && c.IsFacedown()))
                {
                    if (!Card.IsCode(CardId.SolemnReport))
                    {
                        return false;
                    }
                    SelectSTPlace(Card, true);
                    return true;
                }
                // if card should set
                List<ClientCard> canSetSpells = GetCanSetSpells();
                if (setCount + canSetSpells.Count() > 0 && canSetSpells.Contains(Card) && (!Bot.HasInHand(CardId.SolemnReport) || Card.IsTrap()))
                {
                    bool avoidImpermanence = Card.IsCode(CardId.SolemnReport);
                    SelectSTPlace(Card, avoidImpermanence);
                    return true;
                }
            }
            return false;
        }

        public List<ClientCard> GetCanSetSpells()
        {
            List<ClientCard> canSetSpells = new List<ClientCard>();
            foreach (ClientCard card in Bot.Hand)
            {
                bool setFlag = false;
                switch (Card.Id)
                {
                    case CardId.Terraforming:
                        setFlag |= CheckRemainInDeck(CardId.TreasuresOfTheKings) > 0 && DefaultCheckWhetherBotCanSearch();
                        break;
                    case CardId.PrimiteLordlyLode:
                        setFlag |= PrimiteLordlyLodeActivateCheck() && !canSetSpells.Any(c => c.IsCode(CardId.PrimiteLordlyLode));
                        break;
                    case CardId.DominusSpark:
                        setFlag |= !enemyActivateMonsterEffectFromHandGrave;
                        break;
                    case _CardId.InfiniteImpermanence:
                    case CardId.VerdictOfAnubis:
                    case CardId.SolemnReport:
                        setFlag = true;
                        break;
                    case CardId.DominusPurge:
                    case CardId.DominusImpulse:
                        setFlag |= Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0;
                        break;
                    case CardId.SongsOfTheDominators:
                        setFlag |= !Bot.Graveyard.Any(c => c.IsMonster());
                        break;
                    case CardId.ApophisTheSwampDeity:
                        setFlag |= Bot.Hand.Any(c => c != card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap));
                        setFlag |= Bot.GetSpells().Any(c => c != card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap));
                        setFlag |= Bot.GetMonsters().Any(c => c != card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap));
                        break;
                    case CardId.ApophisTheSerpent:
                        setFlag |= CheckRemainInDeck(CardId.ApophisTheSwampDeity) > 0;
                        setFlag |= Bot.HasInHandOrInSpellZone(CardId.ApophisTheSwampDeity);
                        break;
                    default:
                        setFlag = false;
                        break;
                }
                if (setFlag)
                {
                    canSetSpells.Add(card);
                }
            }
            return canSetSpells;
        }

        public bool SpellSet()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;

            // select place
            if (Card.IsTrap() || Card.HasType(CardType.QuickPlay))
            {
                switch (Card.Id)
                {
                    case _CardId.InfiniteImpermanence:
                        // do not set infinite impermanence if don't need to set other cards
                        if (Bot.GetMonsterCount() == 0 && Bot.GetSpellCount() == 0
                            && !Bot.Hand.Any(c => !c.IsCode(_CardId.InfiniteImpermanence) && (c.IsTrap() || c.HasType(CardType.QuickPlay)))
                            && Bot.Hand.Count() <= 6)
                        {
                            return false;
                        }
                        break;
                    case CardId.SongsOfTheDominators:
                        // do not set songs of the dominators if don't need to set
                        if (!Bot.Graveyard.Any(c => c.IsMonster()) && Bot.GetMonsterCount() == 0)
                        {
                            return false;
                        }
                        if (Bot.GetSpells().Any(c => c.IsCode(CardId.SongsOfTheDominators) && c.IsFacedown()))
                        {
                            return false;
                        }
                        break;
                    case CardId.ApophisTheSwampDeity:
                        // do not set swamp deity if don't need to set
                        if (
                            !Bot.Hand.Any(c => c != Card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap)) &&
                            !Bot.GetSpells().Any(c => c != Card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap)) &&
                            !Bot.GetMonsters().Any(c => c != Card && c.HasType(CardType.Continuous) && c.HasType(CardType.Trap)) && 
                            !Bot.HasInHandOrInSpellZone(CardId.PrimiteDrillbeam) &&
                            !(Bot.GetMonsterCount() == 0
                                && !Bot.Hand.Any(c => c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) && c != Card)
                                && !Bot.GetSpells().Any(c => c.IsCode(CardId.ApophisTheSerpent, CardId.ApophisTheSwampDeity) && c != Card && c.IsFacedown())
                            )
                        )
                        {
                            return false;
                        }
                        break;
                    case CardId.SolemnReport:
                    {
                        if (Bot.LifePoints <= 1500)
                        {
                            return false;
                        }
                        break;
                    }
                    case CardId.ApophisTheSerpent:
                    case CardId.DominusImpulse:
                    case CardId.DominusPurge:
                    case CardId.DominusSpark:
                        if (Bot.GetSpells().Any(c => c.IsCode(Card.Id) && c.IsFacedown()))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }

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

    }
}