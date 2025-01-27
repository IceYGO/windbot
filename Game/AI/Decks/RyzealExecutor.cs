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
            public const int TwinsOfTheEclipse = 45852939;
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
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, EffectVeilerActivate);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, GhostOgreAndSnowRabbitActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);

            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightActivate);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDeadnader, RyzealDeadnaderActivate);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDuodrive, RyzealDuodriveActivate);
            AddExecutor(ExecutorType.Activate, CardId.TwinsOfTheEclipse, TwinsOfTheEclipseActivate);
            AddExecutor(ExecutorType.Activate, CardId.AbyssDweller, AbyssDwellerActivate);
            AddExecutor(ExecutorType.Activate, CardId.TornadoDragon, TornadoDragonActivate);

            // hand effect
            AddExecutor(ExecutorType.Activate, _CardId.LockBird, LockBirdActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyPurulia, MulcharmyPuruliaActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyNyalus, MulcharmyNyalusActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MulcharmyFuwalos, MulcharmyFuwalosActivate);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);

            // pre-action activate
            AddExecutor(ExecutorType.Activate, CardId.Bonfire, BonfireActivate);
            AddExecutor(ExecutorType.Activate, CardId.DonnerDaggerFurHire, DonnerDaggerFurHireActivate);
            AddExecutor(ExecutorType.Activate, CardId.Number60DugaresTheTimeless, Number60DugaresTheTimelessActivate);
            AddExecutor(ExecutorType.Activate, CardId.RyzealCross, RyzealCrossActivateCard);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent, TripleTacticsTalentActivate);
            AddExecutor(ExecutorType.Activate, CardId.Bonfire, BonfireActivateToSearchNecessary);
            AddExecutor(ExecutorType.Activate, CardId.SeventhTachyon, SeventhTachyonActivate);

            AddExecutor(ExecutorType.Repos, ChangePositionFirst);

            // xyz summon
            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSpSummon);
            AddExecutor(ExecutorType.SpSummon, LessSpSummonExtra);
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealDuodrive, FirstRyzealDuodriveSpSummon);
            AddExecutor(ExecutorType.SpSummon, SecondXyzSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.TwinsOfTheEclipse, TwinsOfTheEclipseSpSummon);
            AddExecutor(ExecutorType.SpSummon, FinalXyzSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DonnerDaggerFurHire, DonnerDaggerFurHireSpSummon);

            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires, PotOfDesireActivateForContinue);

            AddExecutor(ExecutorType.Activate, CardId.RyzealPlugIn, RyzealPlugInActivateFirst);
            AddExecutor(ExecutorType.Activate, CardId.NodeRyzeal, NodeRyzealActivateFirst);
            AddExecutor(ExecutorType.Activate, CardId.RyzealCross, RyzealCrossActivateRecycleFirst);

            // summon/spsummon
            AddExecutor(ExecutorType.SpSummon, CardId.IceRyzeal, IceRyzealSpSummonFirst);
            AddExecutor(ExecutorType.SpSummon, CardId.NodeRyzeal, NodeRyzealSpSummonFirst);

            AddExecutor(ExecutorType.Summon, CardId.ExRyzeal, ExRyzealSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ExRyzeal, ExRyzealSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ThodeRyzeal, ThodeRyzealSpSummon);
            AddExecutor(ExecutorType.Summon, CardId.IceRyzeal, IceRyzealSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ExRyzeal, ExRyzealSpSummonLater);
            AddExecutor(ExecutorType.Summon, CardId.ThodeRyzeal, ThodeRyzealSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NodeRyzeal, NodeRyzealSpSummon);
            AddExecutor(ExecutorType.Summon, Level4Summon);

            AddExecutor(ExecutorType.Activate, CardId.NodeRyzeal, NodeRyzealActivate);
            AddExecutor(ExecutorType.Activate, CardId.RyzealPlugIn, RyzealPlugInActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.IceRyzeal, IceRyzealSpSummon);

            // activate
            AddExecutor(ExecutorType.Activate, CardId.MereologicAggregator, MereologicAggregatorActivateFirst);
            AddExecutor(ExecutorType.Activate, CardId.IceRyzeal, IceRyzealActivate);
            AddExecutor(ExecutorType.Activate, CardId.ThodeRyzeal, ThodeRyzealActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExRyzeal, ExRyzealActivate);
            AddExecutor(ExecutorType.Activate, CardId.MereologicAggregator, MereologicAggregatorActivateLater);

            // after summon
            AddExecutor(ExecutorType.Activate, CardId.RyzealCross, RyzealCrossActivateRecycleLater);
            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires, PotOfDesiresActivate);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);

        }

        const int attrbuteLightDark = (int)CardAttribute.Light | (int)CardAttribute.Dark;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodeAtlantean = 0x77;
        const int SetcodeInfernoid = 0xbb;
        const int SetcodeMajespecter = 0xd0;        
        const int SetcodePhantomKnight = 0x10db;
        const int SetcodeSkyStriker = 0x115;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeSangen = 0x1a9;
        const int SetcodeTenpaiDragon = 0x1aa;
        const int SetcodeBranded = 0x15d;
        const int SetcodeFloowandereeze = 0x16d;
        const int SetcodeLabrynth = 0x17e;
        const int SetcodeTearlaments = 0x181;
        const int SetcodeHorus = 0x19d;
        const int SetcodeRyzeal = 0x1be;
        const int hintTimingMainEnd = 0x4;
        List<int> NotToNegateIdList = new List<int>
        {
            58699500, 20343502, 25451383, 19403423
        };
        List<int> AlbazFusionList = new List<int>
        {
            1906812, 38811586, 41373230, 44146295, 51409648, 51409648, 87746184
        };
        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal, _CardId.AshBlossom, _CardId.EffectVeiler, CardId.SeventhTachyon,
                                _CardId.InfiniteImpermanence}},
            {2, new List<int> { _CardId.MulcharmyFuwalos, _CardId.GhostOgreAndSnowRabbit, _CardId.MaxxC, _CardId.PotOfDesires, _CardId.CalledByTheGrave }},
            {1, new List<int> { CardId.NodeRyzeal, _CardId.MulcharmyPurulia, _CardId.MulcharmyNyalus, _CardId.LockBird, CardId.TripleTacticsTalent,
                                CardId.Bonfire, CardId.RyzealPlugIn, _CardId.CrossoutDesignator, CardId.RyzealCross}}
        };
        List<int> NotToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int> {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, CardId.MereologicAggregator, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };
        List<int> NeedIceToSolveIdList = new List<int> { 80978111, 87170768 };
        List<ClientCard> currentCanActivateEffect = new List<ClientCard>();

        int maxSummonCount = 1;
        int summonCount = 1;
        bool enemyActivateMaxxC = false;
        bool enemyActivatePurulia = false;
        bool enemyActivateFuwalos = false;
        bool enemyActivateNyalus = false;
        bool lockBirdSolved = false;
        int dimensionShifterCount = 0;
        bool botActivateMulcharmy = false;
        bool botSolvingCross = false;
        List<int> CheckSetcodeList = new List<int> { SetcodePhantomKnight, SetcodeOrcust, SetcodeAtlantean, SetcodeRyzeal, SetcodeTenpaiDragon, SetcodeSangen,
            SetcodeInfernoid, SetcodeSkyStriker, SetcodeLabrynth, SetcodeTearlaments };
        List<int> CheckBotSolvedList = new List<int> { _CardId.MaxxC, _CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus,
            CardId.AbyssDweller, _CardId.EvilswarmExcitonKnight, CardId.RyzealPlugIn };

        bool enemyActivateInfiniteImpermanenceFromHand = false;
        ClientCard deadnaderDestroySelf = null;

        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        List<int> spSummonedCardIdList = new List<int>();
        List<int> botSolvedCardIdList = new List<int>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        List<ClientCard> summonThisTurn = new List<ClientCard>();

        List<ClientCard> hardToDestroyCardList = new List<ClientCard>();
        List<ClientCard> cannotDestroyCardList = new List<ClientCard>();
        HashSet<int> enemyDeckTypeRecord = new HashSet<int>();

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

        public bool CheckWhetherHaveFinalMonster()
        {
            foreach (ClientCard monster in Bot.MonsterZone)
            {
                if (monster == null) continue;
                if (monster.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir) && monster.IsDefense()) return true;
                if (monster.IsCode(CardId.AbyssDweller) && monster.Overlays.Count() > 0) return true;
                if (monster.IsCode(CardId.RyzealDeadnader) && monster.Overlays.Count() > 0) return true;
            }

            return false;
        }

        /// <summary>
        /// Check whether'll be negated
        /// </summary>
        /// <param name="isCounter">check whether card itself is disabled.</param>
        public bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0, bool ignore41 = false)
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
            return CheckCardShouldNegate(lastcard);
        }

        public bool CheckCardShouldNegate(ClientCard card)
        {
            if (card == null) return false;
            if (card.IsMonster() && card.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (NotToNegateIdList.Contains(card.Id)) return false;
            if (card.HasSetcode(_Setcode.Danger) && card.Location == CardLocation.Hand) return false;
            if (card.IsMonster() && card.Location == CardLocation.MonsterZone && card.HasPosition(CardPosition.Defence))
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

        public bool CheckCanContinueSummon(bool skipDuodriver = false)
        {
            bool checkFlag = summonCount > 0 && !activatedCardIdList.Contains(CardId.IceRyzeal) && Bot.HasInHand(CardId.IceRyzeal)
                && !DefaultCheckWhetherCardIdIsNegated(CardId.IceRyzeal);
            if (Bot.HasInHand(CardId.ThodeRyzeal) && !spSummonedCardIdList.Contains(CardId.ThodeRyzeal)
                && !activatedCardIdList.Contains(CardId.ThodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.ThodeRyzeal))
            {
                checkFlag |= Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal));
                checkFlag |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal));
            }
            checkFlag |= !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !activatedCardIdList.Contains(CardId.ExRyzeal)
                && Bot.HasInHand(CardId.ExRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.ExRyzeal) && !CheckWhetherWillbeRemoved();

            checkFlag |= !activatedCardIdList.Contains(CardId.RyzealDuodrive + 1) && Bot.HasInExtra(CardId.RyzealDuodrive)
                && !DefaultCheckWhetherCardIdIsNegated(CardId.RyzealDuodrive) && !CheckWhetherNegated(true, true, CardType.Monster)
                && summonCount > 0 && Bot.Hand.Count(c => c.Level == 4) > 0 && GetLevel4CountOnField() == 1 && !lockBirdSolved
                && !skipDuodriver;

            return checkFlag;
        }

        /// <summary>
        /// check enemy's dangerous card in grave
        /// </summary>
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
                (!onlyMonster || card.IsMonster()) && (card.HasSetcode(SetcodeOrcust) || card.HasSetcode(SetcodePhantomKnight) || card.HasSetcode(SetcodeHorus))).ToList();
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
                && !NotToDestroySpellTrap.Contains(c.Id)).ToList();
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
        }

        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0, bool forNegate = false)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))
                && (!forNegate || (!card.IsDisabled() && card.HasType(CardType.Effect)))
                ).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card) && card.IsFacedown()).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown()
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))
                && (!forNegate || (!card.IsDisabled() && card.HasType(CardType.Effect)))
                ).ToList()));

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
                    if (chainingCard.HasPosition(CardPosition.Defence))
                    {
                        bool have41 = Bot.MonsterZone.Any(c => CheckNumber41(c)) | Enemy.MonsterZone.Any(c => CheckNumber41(c));
                    }
                    resultList.Add(chainingCard);
                }
            }

            return resultList;
        }

        public List<ClientCard> GetLevel4OnField(Func<ClientCard, bool> filter)
        {
            return Bot.GetMonsters().Where(c => (filter == null || filter(c))
                && c.IsFaceup() && !c.HasType(CardType.Xyz | CardType.Link) && c.Level == 4).OrderBy(c => c.GetDefensePower()).ToList();
        }

        public int GetLevel4CountOnField()
        {
            return Bot.GetMonsters().Count(c => c.IsFaceup() && !c.HasType(CardType.Xyz | CardType.Link) && c.Level == 4);
        }

        public int GetLevel4FinalCountOnField(bool checkSupport, out bool hasNode)
        {
            // check whether have 4 monsters for material.
            // if not, skip the second xyz monster.
            int level4Count = GetLevel4CountOnField();
            if (Bot.HasInHand(CardId.ExRyzeal) && !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !CheckWhetherWillbeRemoved())
            {
                if (checkSupport ||
                    (!activatedCardIdList.Contains(CardId.ExRyzeal) && Bot.MonsterZone.All(c => c != null && (c.IsFacedown() || !c.HasType(CardType.Link) && c.Level == 4))))
                {
                    level4Count++;
                }
            }
            if (Bot.HasInHand(CardId.ThodeRyzeal) && !spSummonedCardIdList.Contains(CardId.ThodeRyzeal))
            {
                bool flag = Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal));
                flag |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal));
                if (flag)
                {
                    if (checkSupport || !activatedCardIdList.Contains(CardId.ThodeRyzeal))
                    {
                        level4Count++;
                    }
                }
            }
            hasNode = Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(CardId.NodeRyzeal) && !c.IsDisabled());
            if (Bot.HasInHand(CardId.NodeRyzeal) && !spSummonedCardIdList.Contains(CardId.NodeRyzeal))
            {
                bool flag = Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz));
                if (flag)
                {
                    hasNode = true;
                    level4Count++;
                }
            }
            if (Bot.HasInHand(CardId.RyzealPlugIn) && !CheckWhetherNegated(true, true, CardType.Spell) && checkSupport)
            {
                bool flag = false;
                List<ClientCard> graveTargetList = Bot.Graveyard.Where(
                    c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && !c.HasType(CardType.Xyz) && c.Level == 4).ToList();
                flag |= graveTargetList.Count() > 0;
                hasNode |= graveTargetList.Any(c => c.IsCode(CardId.NodeRyzeal));

                List<ClientCard> banishedTargetList = Bot.Banished.Where(
                    c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && !c.HasType(CardType.Xyz) && c.Level == 4).ToList();
                flag |= banishedTargetList.Count() > 0;
                hasNode |= banishedTargetList.Any(c => c.IsCode(CardId.NodeRyzeal));

                if (flag) level4Count++;
            }
            hasNode &= !CheckWhetherWillbeRemoved() && !activatedCardIdList.Contains(CardId.NodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.NodeRyzeal);
            hasNode &= Bot.Graveyard.Any(c => c != null && c.IsMonster() && c.HasSetcode(SetcodeRyzeal) && !c.IsCode(CardId.NodeRyzeal) && c.Level == 4);
            if (hasNode)
            {
                bool flag = Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && !c.HasType(CardType.Xyz) && c.Level == 4 && !c.IsCode(CardId.NodeRyzeal));
                if (flag)
                {
                    if (checkSupport || GetCostFromHandAndField(null, false).Count() > 0) level4Count++;
                }
            }
            if (checkSupport)
            {
                int checkHandCount = 2;
                if (summonCount > 0 && Bot.Hand.Any(c => c.Level == 4 && !c.IsCode(CardId.ExRyzeal, CardId.ThodeRyzeal, CardId.NodeRyzeal)))
                {
                    level4Count++;
                    checkHandCount++;
                }
                if (Bot.Hand.Count() >= checkHandCount && Bot.HasInHand(CardId.IceRyzeal) && !spSummonedCardIdList.Contains(CardId.IceRyzeal) && !CheckWhetherWillbeRemoved())
                {
                    level4Count++;
                }
            }

            return level4Count;
        }

        public List<ClientCard> GetCostFromHandAndFieldFirst(ClientCard exceptCard)
        {
            return Bot.MonsterZone.Where(c => c != null && !c.IsDisabled() && c.IsCode(NeedIceToSolveIdList) && c != exceptCard && !c.HasType(CardType.Token)).ToList();
        }

        public List<ClientCard> GetCostFromHandAndField(ClientCard exceptCard, bool sendNotNecessary)
        {
            List<ClientCard> resultList = GetCostFromHandAndFieldFirst(exceptCard);
            if (!activatedCardIdList.Contains(CardId.TwinsOfTheEclipse + 1))
            {
                List<ClientCard> xyzList = Bot.Graveyard.Where(c => c.HasType(CardType.Xyz)).ToList();
                // sending twins
                ClientCard twins = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.TwinsOfTheEclipse) && !resultList.Contains(c));
                if (twins == null)
                {
                    twins = Bot.SpellZone.FirstOrDefault(c => c != null && c.IsCode(CardId.TwinsOfTheEclipse) && !resultList.Contains(c));
                }
                if (twins != null)
                {
                    int twinsXyzCount = 0;
                    foreach (int cardId in twins.Overlays)
                    {
                        YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
                        if (cardData != null && cardData.HasType(CardType.Xyz)) twinsXyzCount++;
                    }
                    bool flag = twinsXyzCount >= 2;
                    flag |= twinsXyzCount > 0 && xyzList.Count() > 0;
                    flag |= xyzList.Count() > 1 && xyzList.Count(c => c.IsCanRevive()) > 0;
                    if (flag)
                    {
                        resultList.Add(twins);
                    }
                }
            }

            if (Bot.HasInSpellZone(CardId.RyzealCross, true, true))
            {
                // sending duodrive because not enough material on field
                if (Bot.HasInExtra(CardId.RyzealDuodrive) && !activatedCardIdList.Contains(CardId.RyzealDuodrive + 1) && !lockBirdSolved)
                {
                    bool checkOverlay = true;
                    ClientCard duoDrive = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.RyzealDuodrive) && !resultList.Contains(c));
                    if (duoDrive == null)
                    {
                        checkOverlay = false;
                        duoDrive = Bot.SpellZone.FirstOrDefault(c => c != null && c.IsCode(CardId.RyzealDuodrive) && !resultList.Contains(c));
                    }
                    if (duoDrive != null)
                    {
                        int overlayCount = Bot.MonsterZone.Sum(c => c != null ? c.Overlays.Count() : 0);
                        if (!checkOverlay || overlayCount < 2)
                        {
                            resultList.Add(duoDrive);
                        }
                    }
                }
                // send deadnader with no overlay
                if (Bot.HasInExtra(CardId.RyzealDeadnader))
                {
                    ClientCard deadnader = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.RyzealDeadnader) && c.Overlays.Count() == 0 && !resultList.Contains(c));
                    if (deadnader != null)
                    {
                        resultList.Add(deadnader);
                    }
                }
            }

            // sending monsters in spell zone
            List<ClientCard> monstersInSpellZone = Bot.SpellZone.Where(c => c != null && c.Data != null
                && c.Data.HasType(CardType.Monster) && !c.Data.HasType(CardType.Pendulum | CardType.Token) && !resultList.Contains(c)).ToList();
            resultList.AddRange(monstersInSpellZone);

            // send enemy monsters
            List<ClientCard> enemyMonsters = Bot.MonsterZone.Where(c => c != null && !resultList.Contains(c) && c.Owner == 1).ToList();
            resultList.AddRange(enemyMonsters);

            if (sendNotNecessary)
            {
                // send xyz monster with no material
                List<ClientCard> xyzMonsterWithNoMaterial = Bot.MonsterZone.Where(
                    c => c != null && c.HasType(CardType.Xyz) && c.GetDefensePower() < 2500 && c.Overlays.Count() == 0 && !resultList.Contains(c))
                    .OrderBy(c => c.GetDefensePower()).ToList();
                resultList.AddRange(xyzMonsterWithNoMaterial);

                // sending unimportant card in hand
                List<int> unimportantList = new List<int> { _CardId.MulcharmyNyalus, _CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, CardId.SeventhTachyon };
                resultList.AddRange(Bot.Hand.Where(c => c != null && c.IsCode(unimportantList) && !resultList.Contains(c)));

                // sending activated ryzeal monster
                List<int> checkRyzealIdList = new List<int> { CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal };
                foreach (int checkId in checkRyzealIdList)
                {
                    if (summonCount == 0 && spSummonedCardIdList.Contains(checkId))
                    {
                        List<ClientCard> ryzealList = Bot.Hand.Where(c => c != null && c != exceptCard && !resultList.Contains(c) && c.IsCode(checkId)).ToList();
                        resultList.AddRange(ryzealList);
                    }
                }

                // sending dump cards
                foreach (ClientCard card in Bot.Hand)
                {
                    if (Bot.Hand.Count(c => c != null && !resultList.Contains(c) && c.IsCode(card.Id)) > 1)
                    {
                        resultList.Add(card);
                    }
                }

                if (resultList.Count() == 0)
                {
                    // sending other cards
                    List<int> checkIdList = new List<int> { _CardId.CrossoutDesignator, _CardId.CalledByTheGrave, _CardId.InfiniteImpermanence,
                        _CardId.GhostOgreAndSnowRabbit, _CardId.LockBird, _CardId.AshBlossom, _CardId.MaxxC };
                    foreach (int checkId in checkIdList)
                    {
                        List<ClientCard> costList = Bot.Hand.Where(c => c != null && c != exceptCard && !resultList.Contains(c) && c.IsCode(checkId)).ToList();
                        resultList.AddRange(costList);
                    }
                }
            }

            return resultList;
        }

        public int GetBotCurrentTotalAttack(List<ClientCard> exceptList = null)
        {
            if (Util.IsTurn1OrMain2() || botSolvedCardIdList.Contains(_CardId.EvilswarmExcitonKnight)) return 0;
            int result = 0;
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (exceptList != null && exceptList.Contains(monster)) continue;
                if (monster.IsAttack() || !summonThisTurn.Contains(monster)) result += monster.Attack;
            }
            return result;
        }

        public int GetNegateEffectCount()
        {
            int count = 0;
            count += GetCalledbytheGraveIdCount(_CardId.MaxxC) < 2 && Bot.HasInHand(_CardId.MaxxC) ? 1 : 0;
            count += GetCalledbytheGraveIdCount(_CardId.AshBlossom) < 2 && Bot.HasInHand(_CardId.AshBlossom) ? 1 : 0;
            count += GetCalledbytheGraveIdCount(_CardId.EffectVeiler) < 2 && Bot.HasInHand(_CardId.EffectVeiler) ? 1 : 0;
            count += GetCalledbytheGraveIdCount(_CardId.GhostOgreAndSnowRabbit) < 2 && Bot.HasInHand(_CardId.GhostOgreAndSnowRabbit) ? 1 : 0;
            count += GetCalledbytheGraveIdCount(_CardId.LockBird) < 2 && Bot.HasInHand(_CardId.LockBird) ? 1 : 0;
            count += Bot.SpellZone.Count(c => c != null && c.IsFacedown() && c.IsCode(_CardId.InfiniteImpermanence));
            count += Math.Min(4 - Bot.GetSpellCountWithoutField(), Bot.Hand.Count(c => c.IsCode(_CardId.InfiniteImpermanence)));

            return count;
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

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            ClientCard twin = attackers.FirstOrDefault(c => c.IsCode(CardId.TwinsOfTheEclipse) && !c.IsDisabled());
            if (twin != null)
            {
                if (Enemy.MonsterZone.Any(c => c != null && c.GetDefensePower() <= 2500))
                {
                    return twin;
                }
            }
            return null;
        }

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            if (cards != null && cards.Count() > 0)
            {
                currentCanActivateEffect.Clear();
                currentCanActivateEffect.AddRange(cards);
            }
            base.OnSelectChain(cards);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null)
            {
                if (botSolvingCross)
                {
                    if (hint == HintMsg.DeattachFrom)
                    {
                        List<Func<ClientCard, bool>> funcList = new List<Func<ClientCard, bool>>
                        {
                            (c) => c.IsDisabled() && c.IsCode(CardId.RyzealDuodrive),
                            (c) => c.IsCode(CardId.RyzealDuodrive),
                            (c) => c.IsDisabled() && c.IsCode(CardId.RyzealDeadnader),
                            (c) => c.IsCode(CardId.RyzealDeadnader)
                        };
                        foreach (Func<ClientCard, bool> func in funcList)
                        {
                            ClientCard target = cards.FirstOrDefault(c => c != null && func(c));
                            if (target != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                    }

                    if (hint == HintMsg.RemoveXyz)
                    {
                        List<ClientCard> targets = cards.OrderBy(c => c.Attack).ToList();
                        botSolvingCross = false;
                        return Util.CheckSelectCount(targets, cards, min, max);
                    }
                }

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
                    bool canUsePluginToSpSummonDeadnader = Bot.Graveyard.Any(c => c != null && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));
                    canUsePluginToSpSummonDeadnader |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));

                    List<ClientCard> faceUpSpells = Bot.GetSpells().Where(c => c.IsFaceup()).ToList();
                    banishList.AddRange(ShuffleList(faceUpSpells));
                    List<ClientCard> faceDownSpells = Bot.GetSpells().Where(c => c.IsFacedown()
                        && !(canUsePluginToSpSummonDeadnader && c.IsCode(CardId.RyzealPlugIn))).ToList();
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

                if (currentSolvingChain.Controller == 0)
                {
                    if (hint == HintMsg.AddToHand)
                    {
                        if (currentSolvingChain.IsCode(CardId.ThodeRyzeal))
                        {
                            ClientCard ice = cards.FirstOrDefault(c => c.IsCode(CardId.IceRyzeal));
                            ClientCard ex = cards.FirstOrDefault(c => c.IsCode(CardId.ExRyzeal));
                            if (ice != null)
                            {
                                bool canSummonAndActivateIce = Duel.Player == 0 && summonCount > 0 && Duel.Phase < DuelPhase.End;
                                bool flag = canSummonAndActivateIce && !Bot.HasInHand(CardId.IceRyzeal)
                                    && !activatedCardIdList.Contains(CardId.IceRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.IceRyzeal);
                                flag |= ex == null;
                                flag |= Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && c.IsCode(NeedIceToSolveIdList))
                                    && !spSummonedCardIdList.Contains(CardId.IceRyzeal) && !CheckWhetherWillbeRemoved();
                                if (!canSummonAndActivateIce)
                                {
                                    flag |= DefaultCheckWhetherCardIdIsNegated(CardId.ExRyzeal);
                                    flag |= spSummonedCardIdList.Contains(CardId.ExRyzeal) || activatedCardIdList.Contains(CardId.ExRyzeal);
                                }
                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { ice }, cards, min, max);
                                }
                            }
                            if (ex != null)
                            {
                                bool flag = !Bot.HasInHand(CardId.ExRyzeal) && !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !activatedCardIdList.Contains(CardId.ExRyzeal);
                                flag |= Bot.HasInHand(CardId.IceRyzeal);
                                flag |= ice == null;
                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { ex }, cards, min, max);
                                }
                            }
                        }

                        if (currentSolvingChain.IsCode(CardId.ExRyzeal))
                        {
                            ClientCard thode = cards.FirstOrDefault(c => c.IsCode(CardId.ThodeRyzeal));
                            ClientCard node = cards.FirstOrDefault(c => c.IsCode(CardId.NodeRyzeal));
                            if (thode != null)
                            {
                                bool flag = node == null;
                                flag |= !Bot.HasInHand(CardId.ThodeRyzeal) && !spSummonedCardIdList.Contains(CardId.ThodeRyzeal) && !activatedCardIdList.Contains(CardId.ThodeRyzeal);

                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { thode }, cards, min, max);
                                }
                            }
                            if (node != null)
                            {
                                bool flag = thode == null;
                                flag |= spSummonedCardIdList.Contains(CardId.ThodeRyzeal) && activatedCardIdList.Contains(CardId.ThodeRyzeal)
                                    && !DefaultCheckWhetherCardIdIsNegated(CardId.ThodeRyzeal);
                                flag |= CheckShouldNoMoreSpSummon(CardLocation.Hand) && !CheckShouldNoMoreSpSummon(CardLocation.Grave)
                                    && !spSummonedCardIdList.Contains(CardId.NodeRyzeal);

                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { node }, cards, min, max);
                                }
                            }
                        }

                        if (currentSolvingChain.IsCode(CardId.Bonfire) || currentSolvingChain.IsCode(CardId.SeventhTachyon))
                        {
                            if (!Bot.HasInHand(CardId.ExRyzeal) && !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !CheckWhetherWillbeRemoved())
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(CardId.ExRyzeal));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            if (!Bot.HasInHand(CardId.IceRyzeal))
                            {
                                bool flag = summonCount > 0 && !activatedCardIdList.Contains(CardId.IceRyzeal);
                                flag |= !spSummonedCardIdList.Contains(CardId.IceRyzeal) && Bot.Hand.Count > 0;
                                if (flag)
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(CardId.IceRyzeal));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            List<int> searchTargetList = new List<int> { CardId.ExRyzeal, CardId.IceRyzeal };
                            foreach (int targetId in searchTargetList)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }

                        if (currentSolvingChain.IsCode(CardId.RyzealDuodrive))
                        {
                            // search spells
                            if (!CheckWhetherNegated(true, true, CardType.Spell))
                            {
                                ClientCard cross = cards.FirstOrDefault(c => c.IsCode(CardId.RyzealCross));
                                if (cross != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { cross }, cards, min, max);
                                }

                                ClientCard plugin = cards.FirstOrDefault(c => c.IsCode(CardId.RyzealPlugIn));
                                if (plugin != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { plugin }, cards, min, max);
                                }
                            }

                            // search for spsummon
                            List<KeyValuePair<int, Func<bool>>> checkList = new List<KeyValuePair<int, Func<bool>>>
                            {
                                new KeyValuePair<int, Func<bool>>(CardId.IceRyzeal,
                                () => Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && c.IsCode(NeedIceToSolveIdList))
                                    && !spSummonedCardIdList.Contains(CardId.IceRyzeal) && !CheckWhetherWillbeRemoved() ),
                                new KeyValuePair<int, Func<bool>>(CardId.ExRyzeal,
                                () => !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !activatedCardIdList.Contains(CardId.ExRyzeal)
                                    && !DefaultCheckWhetherCardIdIsNegated(CardId.ExRyzeal) && !CheckWhetherWillbeRemoved() ),
                                new KeyValuePair<int, Func<bool>>(CardId.IceRyzeal,
                                () => summonCount > 0 && !activatedCardIdList.Contains(CardId.IceRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.IceRyzeal) ),
                                new KeyValuePair<int, Func<bool>>(CardId.ThodeRyzeal,
                                () => !spSummonedCardIdList.Contains(CardId.ThodeRyzeal) && !activatedCardIdList.Contains(CardId.ThodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.ThodeRyzeal) ),
                                new KeyValuePair<int, Func<bool>>(CardId.NodeRyzeal,
                                () => !spSummonedCardIdList.Contains(CardId.NodeRyzeal) )
                            };
                            foreach (KeyValuePair<int, Func<bool>> pair in checkList)
                            {
                                if (!Bot.HasInHand(pair.Key) && pair.Value())
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(pair.Key));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }

                            // search not exists
                            List<int> searchList = new List<int> { CardId.ExRyzeal, CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.NodeRyzeal };
                            foreach (int id in searchList)
                            {
                                if (!Bot.HasInHand(id))
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(id));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }

                            // random search
                            return Util.CheckSelectCount(ShuffleList(cards.ToList()), cards, min, max);
                        }
                    }

                    if (hint == HintMsg.SpSummon)
                    {
                        if (currentSolvingChain.IsCode(CardId.IceRyzeal))
                        {
                            ClientCard thode = cards.FirstOrDefault(c => c.IsCode(CardId.ThodeRyzeal));
                            ClientCard ex = cards.FirstOrDefault(c => c.IsCode(CardId.ExRyzeal));
                            ClientCard node = cards.FirstOrDefault(c => c.IsCode(CardId.NodeRyzeal));
                            if (thode != null)
                            {
                                bool flag = !activatedCardIdList.Contains(CardId.ThodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.ThodeRyzeal);
                                flag |= Bot.HasInHand(CardId.ExRyzeal) && !spSummonedCardIdList.Contains(CardId.ExRyzeal);
                                flag |= ex == null && node == null;
                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { thode }, cards, min, max);
                                }
                            }
                            if (ex != null)
                            {
                                bool flag = !activatedCardIdList.Contains(CardId.ExRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.ExRyzeal);
                                flag |= Bot.HasInHand(CardId.ThodeRyzeal) && !spSummonedCardIdList.Contains(CardId.ThodeRyzeal);
                                flag |= thode == null && node == null;
                                if (flag)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { ex }, cards, min, max);
                                }
                            }
                            if (node != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { node }, cards, min, max);
                            }
                        }

                        if (currentSolvingChain.IsCode(CardId.TwinsOfTheEclipse))
                        {
                            ClientCard target = TwinsOfTheEclipseRebornTarget(new List<ClientCard>(cards));

                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }

                    if (hint == HintMsg.ToDeck)
                    {
                        if (currentSolvingChain.IsCode(CardId.TripleTacticsTalent))
                        {
                            foreach (ClientCard hand in cards)
                            {
                                foreach (int setcode in CheckSetcodeList)
                                {
                                    if (hand.HasSetcode(setcode))
                                    {
                                        enemyDeckTypeRecord.Add(setcode);
                                    }
                                }
                            }
                            return Util.CheckSelectCount(ShuffleList(cards.ToList()), cards, min, max);
                        }
                    }

                    if (hint == HintMsg.XyzMaterial)
                    {
                        if (currentSolvingChain.IsCode(CardId.RyzealDeadnader, CardId.RyzealDuodrive, CardId.RyzealPlugIn))
                        {
                            // material that have effect
                            ClientCard effectTarget = cards.FirstOrDefault(c => c.IsCode(CardId.TwinsOfTheEclipse, CardId.MereologicAggregator));
                            if (effectTarget != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { effectTarget }, cards, min, max);
                            }
                            // cards in hand
                            foreach (ClientCard card in cards)
                            {
                                if (Bot.Hand.Count(c => c.IsCode(card.Id)) > 0) 
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                                }
                            }
                            // dump card
                            foreach (ClientCard card in cards)
                            {
                                if (cards.Count(c => c.IsCode(card.Id)) > 1) {
                                    return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                                }
                            }
                        }
                    }

                    if (hint == HintMsg.RemoveXyz)
                    {
                        if (currentSolvingChain.IsCode(CardId.RyzealDuodrive))
                        {
                            List<ClientCard> resultList = new List<ClientCard>();

                            List<int> detachOwnerIdList = new List<int> { CardId.StarliegePhotonBlastDragon, CardId.RyzealDuodrive, CardId.TwinsOfTheEclipse,
                                CardId.FullArmoredUtopicRayLancer, _CardId.EvilswarmExcitonKnight, CardId.Number60DugaresTheTimeless, CardId.RyzealDeadnader };
                            foreach (int ownerId in  detachOwnerIdList)
                            {
                                // detach from DuoDrive first
                                List<ClientCard> detachMaterialList = cards.Where(c => c.OwnTargets.Any(oc => oc.IsCode(ownerId))).ToList();
                                if (detachMaterialList.Count() > 0)
                                {
                                    // deadnader
                                    ClientCard deadnader = detachMaterialList.FirstOrDefault(c => c.IsCode(CardId.RyzealDeadnader));
                                    if (deadnader != null)
                                    {
                                        resultList.Add(deadnader);
                                    }
                                    List<Func<ClientCard, bool>> filterList = new List<Func<ClientCard, bool>>
                                    {
                                        (c) => !resultList.Contains(c)
                                            && !c.IsCode(CardId.MereologicAggregator, CardId.TwinsOfTheEclipse)
                                            && Bot.HasInSpellZone(CardId.RyzealCross) && c.HasSetcode(SetcodeRyzeal)
                                            && (c.Data == null || ((c.Data.Attribute & attrbuteLightDark) != 0)),
                                        (c) => !resultList.Contains(c)
                                            && !c.IsCode(CardId.MereologicAggregator, CardId.TwinsOfTheEclipse)
                                            && Bot.HasInSpellZone(CardId.RyzealCross) && c.HasSetcode(SetcodeRyzeal),
                                        (c) => !resultList.Contains(c)
                                            && !c.IsCode(CardId.MereologicAggregator, CardId.TwinsOfTheEclipse)
                                            && (c.Data == null || ((c.Data.Attribute & attrbuteLightDark) != 0)),
                                        (c) => !resultList.Contains(c)
                                            && !c.IsCode(CardId.MereologicAggregator, CardId.TwinsOfTheEclipse),
                                        (c) => !resultList.Contains(c)
                                    };
                                    foreach (Func<ClientCard, bool> filter in filterList)
                                    {
                                        foreach (ClientCard material in detachMaterialList)
                                        {
                                            if (filter(material))
                                            {
                                                resultList.Add(material);
                                            }
                                        }
                                    }
                                }
                            }
                            return Util.CheckSelectCount(resultList, cards, min, max);
                        }
                    }

                    // gain material by plugin
                    if (currentSolvingChain.IsCode(CardId.RyzealPlugIn) && cards.All(c => c.Location == CardLocation.MonsterZone))
                    {
                        ClientCard abyssDweller = cards.FirstOrDefault(c => c != null && !c.IsDisabled() && c.IsCode(CardId.AbyssDweller) && c.Overlays.Count() < 2);
                        if (abyssDweller != null && AbyssDwellerSummonCheck())
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { abyssDweller }, cards, min, max);
                        }

                        ClientCard duoDriver = cards.FirstOrDefault(c => c != null && !c.IsDisabled() && c.IsCode(CardId.RyzealDuodrive) && c.Overlays.Count() == 1);
                        if (duoDriver != null && Bot.HasInMonstersZone(CardId.StarliegePhotonBlastDragon, true, false, true))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { duoDriver }, cards, min, max);
                        }

                        ClientCard deadnader = cards.FirstOrDefault(c => c != null && !c.IsDisabled() && c.IsCode(CardId.RyzealDeadnader));
                        if (deadnader != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { deadnader }, cards, min, max);
                        }

                        if (Bot.HasInSpellZone(CardId.RyzealCross, true, true))
                        {
                            ClientCard ryzealXyz = cards.FirstOrDefault(c => c != null && c.HasSetcode(SetcodeRyzeal));
                            if (ryzealXyz != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { ryzealXyz }, cards, min, max);
                            }
                        }

                        ClientCard tornadoDragon = cards.FirstOrDefault(c => c != null && !c.IsDisabled() && c.IsCode(CardId.TornadoDragon) && c.Overlays.Count() == 1);
                        if (tornadoDragon != null && TornadoDragonSummonCheck())
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { tornadoDragon }, cards, min, max);
                        }

                        ClientCard no41 = cards.FirstOrDefault(c => c != null && c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
                        if (no41 != null && Number41BagooskatheTerriblyTiredTapirSummonCheck())
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { no41 }, cards, min, max);
                        }

                        duoDriver = cards.FirstOrDefault(c => c != null && !c.IsDisabled() && c.IsCode(CardId.RyzealDuodrive));
                        if (duoDriver != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { duoDriver }, cards, min, max);
                        }
                    }

                    // double attack
                    if (currentSolvingChain.IsCode(CardId.Number60DugaresTheTimeless) && cards.All(c => c.Location == CardLocation.MonsterZone))
                    {
                        ClientCard maxAttackMonster = cards.Where(c => c != null && (c.HasPosition(CardPosition.FaceUpAttack) || !summonThisTurn.Contains(c)))
                            .OrderByDescending(c => c.Attack).FirstOrDefault();
                        if (maxAttackMonster != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { maxAttackMonster }, cards, min, max);
                        }
                    }
                }

                // hand solve
                if (hint == HintMsg.ToDeck || hint == HintMsg.ToGrave || hint == HintMsg.Discard)
                {
                    bool allBotHand = cards.All(c => c.Controller == 0 && c.Location == CardLocation.Hand);
                    if (allBotHand)
                    {
                        List<ClientCard> resultList = new List<ClientCard>();
                        List<int> returnList = new List<int> { _CardId.MulcharmyNyalus, _CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos,
                                CardId.SeventhTachyon
                            };
                        foreach (int code in returnList)
                        {
                            List<ClientCard> targetList = cards.Where(c => c.IsCode(code) && !resultList.Contains(c)).ToList();
                            if (targetList.Count() > 0)
                            {
                                resultList.AddRange(targetList);
                            }
                        }
                        // return dump card
                        foreach (ClientCard card in cards)
                        {
                            if (!resultList.Contains(card) && cards.Where(c => c.IsCode(card.Id) && !resultList.Contains(c)).Count() > 1)
                            {
                                resultList.Add(card);
                            }
                        }
                        List<int> unproperCardList = new List<int> { _CardId.EffectVeiler, _CardId.InfiniteImpermanence, _CardId.GhostOgreAndSnowRabbit, CardId.TripleTacticsTalent,
                                CardId.NodeRyzeal, _CardId.LockBird, CardId.RyzealPlugIn, _CardId.CrossoutDesignator, _CardId.CalledByTheGrave, CardId.RyzealCross,
                                CardId.ThodeRyzeal, CardId.ExRyzeal, CardId.IceRyzeal
                            };
                        foreach (int code in unproperCardList)
                        {
                            ClientCard target = cards.FirstOrDefault(c => c.IsCode(code) && !resultList.Contains(c));
                            if (target != null)
                            {
                                resultList.Add(target);
                            }
                        }
                        return Util.CheckSelectCount(resultList, cards, min, max);
                    }
                }
            }

            if (currentSolvingChain == null)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null)
                {
                    // handle for RyzealDeadnader
                    if (lastChainCard.Controller == 0 && lastChainCard.IsCode(CardId.RyzealDeadnader))
                    {
                        if (hint == HintMsg.RemoveXyz)
                        {
                            if (deadnaderDestroySelf != null)
                            {
                                ClientCard detachTarget = cards.FirstOrDefault(c => c.IsCode(CardId.MereologicAggregator, CardId.TwinsOfTheEclipse));
                                if (detachTarget != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { detachTarget }, cards, min, max);
                                }
                            }
                            List<ClientCard> targets = cards.OrderBy(c => c.Attack).ToList();
                            return Util.CheckSelectCount(targets, cards, min, max);
                        } else if (hint == HintMsg.Destroy)
                        {
                            if (deadnaderDestroySelf != null && cards.Contains(deadnaderDestroySelf))
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { deadnaderDestroySelf }, cards, min, max);
                            }
                            List<ClientCard> targetList = CanDestroyList();
                            foreach (ClientCard target in targetList)
                            {
                                if (cards.Contains(target))
                                {
                                    currentDestroyCardList.Add(target);
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                    }

                    if (hint == HintMsg.RemoveXyz && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                    {
                        List<int> checkRyzealIdList = new List<int> { CardId.NodeRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal };
                        foreach (int checkId in checkRyzealIdList)
                        {
                            if (!activatedCardIdList.Contains(checkId))
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(checkId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                    }
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        /// <summary>
        /// go first
        /// </summary>
        public override bool OnSelectHand()
        {
            HashSet<int> tenpaiList = new HashSet<int> { SetcodeTenpaiDragon, SetcodeSangen };
            bool maybeTenpai = enemyDeckTypeRecord.Count() > 0 && enemyDeckTypeRecord.All(c => tenpaiList.Contains(c));
            return !maybeTenpai;
        }

        public override int OnSelectOption(IList<int> options)
        {
            bool tripleCheck = false;
            for (int opt = 0; opt < 3; ++ opt)
            {
                if (options.Contains(Util.GetStringId(CardId.TripleTacticsTalent, opt)))
                {
                    tripleCheck = true;
                    break;
                }
            }
            if (tripleCheck)
            {
                return TripleTacticsTalentDecision(options);
            }

            bool no60Check = false;
            for (int opt = 0; opt < 3; ++ opt)
            {
                if (options.Contains(Util.GetStringId(CardId.Number60DugaresTheTimeless, opt)))
                {
                    no60Check = true;
                    break;
                }
            }
            if (no60Check)
            {
                // double attack
                if (options.Contains(Util.GetStringId(CardId.Number60DugaresTheTimeless, 2)))
                {
                    if (Number60DugaresTheTimelessDoubleTarget() != null)
                    {
                        int res = options.IndexOf(Util.GetStringId(CardId.Number60DugaresTheTimeless, 2));
                        if (res >= 0)
                        {
                            return res;
                        }
                    }
                }
                // draw effect
                if (options.Contains(Util.GetStringId(CardId.Number60DugaresTheTimeless, 0)))
                {
                    if (Number60DugaresTheTimelessDrawEffect())
                    {
                        int res = options.IndexOf(Util.GetStringId(CardId.Number60DugaresTheTimeless, 0));
                        if (res >= 0)
                        {
                            return res;
                        }
                    }
                }
                // reborn
                if (options.Contains(Util.GetStringId(CardId.Number60DugaresTheTimeless, 1)))
                {
                    if (Number60DugaresTheTimelessRebornEffect())
                    {
                        int res = options.IndexOf(Util.GetStringId(CardId.Number60DugaresTheTimeless, 1));
                        if (res >= 0)
                        {
                            return res;
                        }
                    }
                }
            }

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
            if (desc == Util.GetStringId(CardId.RyzealPlugIn, 1))
            {
                Logger.DebugWriteLine("** plugin set material");
                return true;
            }

            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == _CardId.Number41BagooskatheTerriblyTiredTapir && (Util.IsTurn1OrMain2() || Duel.Player == 1))
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

                if (Bot.HasInExtra(CardId.Number60DugaresTheTimeless) && GetLevel4FinalCountOnField(true, out _) >= 2)
                {
                    bestBotAttack *= 2;
                }

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
                // for doom bot
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
            deadnaderDestroySelf = null;
            botSolvingCross = false;
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            activatedCardIdList.Clear();
            spSummonedCardIdList.Clear();
            botSolvedCardIdList.Clear();
            enemyPlaceThisTurn.Clear();
            summonThisTurn.Clear();
            currentCanActivateEffect.Clear();
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

                if (card.HasSetcode(SetcodeFloowandereeze))
                {
                    enemyDeckTypeRecord.Add(SetcodeFloowandereeze);
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
            botSolvingCross = false;
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
                    if (currentCard.Controller == 0)
                    {
                        foreach (int checkId in CheckBotSolvedList)
                        {
                            if (currentCard.IsCode(checkId))
                            {
                                botSolvedCardIdList.Add(checkId);
                            }
                        }
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
            currentCanActivateEffect.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            botSolvingCross = false;
            deadnaderDestroySelf = null;
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
                if (previousControler == 1)
                {
                    if (card.IsCode(_CardId.InfiniteImpermanence) && previousLocation == (int)CardLocation.Hand && currentLocation == (int)CardLocation.SpellZone)
                        enemyActivateInfiniteImpermanenceFromHand = true;
                }
                if (card.Owner == 1)
                {
                    foreach (int setcode in CheckSetcodeList)
                    {
                        if (card.HasSetcode(setcode))
                        {
                            enemyDeckTypeRecord.Add(setcode);
                        }
                    }
                    if (card.IsCode(AlbazFusionList))
                    {
                        enemyDeckTypeRecord.Add(SetcodeBranded);
                    }
                }
                if (currentControler == 1 && (currentLocation == (int)CardLocation.MonsterZone || currentLocation == (int)CardLocation.SpellZone))
                {
                    enemyPlaceThisTurn.Add(card);
                }

                if (previousControler == 0 && previousLocation == (int)CardLocation.MonsterZone && currentLocation != (int)CardLocation.MonsterZone && summonThisTurn.Contains(card))
                {
                    summonThisTurn.Remove(card);
                }
                if (currentControler == 0 && currentLocation == (int)CardLocation.MonsterZone)
                {
                    summonThisTurn.Add(card);
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

        public bool IceRyzealSpSummonFirst()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= GetLevel4CountOnField() >= 2;
                if (flag) return false;
            }
            List<ClientCard> costList = GetCostFromHandAndFieldFirst(Card);
            if (costList.Count() > 0)
            {
                AI.SelectCard(costList);
                return true;
            }

            return false;
        }

        public bool IceRyzealSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= GetLevel4CountOnField() >= 2;
                if (flag) return false;
            }
            if (Card.Level != 4) return false;
            if (summonCount <= 0 && GetLevel4CountOnField() == 1)
            {
                List<ClientCard> firstCostList = GetCostFromHandAndField(Card, false);
                if (firstCostList.Count() > 0)
                {
                    AI.SelectCard(firstCostList);
                    return true;
                }
                if (Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled() && c.IsFloodgate()) || !CheckWhetherHaveFinalMonster())
                {
                    List<ClientCard> costList = GetCostFromHandAndField(Card, true);
                    if (costList.Count() > 0)
                    {
                        AI.SelectCard(costList);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IceRyzealSummon()
        {
            if (CheckWhetherNegated(true, true)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= GetLevel4CountOnField() >= 2;
                if (flag) return false;
            }
            summonCount -= 1;
            return true;
        }

        public bool IceRyzealActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Deck) && GetLevel4CountOnField() >= 2)
            {
                return false;
            }
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool ThodeRyzealSpSummon()
        {
            int lv4Count = GetLevel4CountOnField();
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= lv4Count >= 2;
                flag |= lv4Count == 1 && summonCount > 0;
                if (flag) return false;
            }
            bool spsummonFlag = lv4Count == 1;
            spsummonFlag |= !CheckWhetherNegated(true, true, CardType.Monster) && CheckRemainInDeck(CardId.IceRyzeal, CardId.ExRyzeal) > 0
                && !activatedCardIdList.Contains(CardId.ThodeRyzeal) && !lockBirdSolved;
            if (GetLevel4CountOnField() == 0)
            {
                spsummonFlag |= GetLevel4FinalCountOnField(true, out _) >= 2 && !CheckWhetherHaveFinalMonster();
            }

            return spsummonFlag;
        }

        public bool ThodeRyzealSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra))
            {
                int lv4Count = GetLevel4CountOnField();
                if (lv4Count == 1)
                {
                    bool flag = !activatedCardIdList.Contains(CardId.ThodeRyzeal);
                    flag &= !(Bot.HasInHand(CardId.ExRyzeal) && !activatedCardIdList.Contains(CardId.ExRyzeal));
                    if (flag)
                    {
                        summonCount -= 1;
                        return true;
                    }
                }
                bool skipFlag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                skipFlag |= lv4Count >= 2;
                if (skipFlag) return false;
            }
            if (CheckWhetherNegated(true)) return false;

            summonCount -= 1;
            return true;
        }

        public bool ThodeRyzealActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool NodeRyzealSpSummon()
        {
            int lv4Count = GetLevel4CountOnField();
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= lv4Count >= 2;
                flag |= lv4Count == 1 && summonCount > 0;
                if (flag) return false;
            }
            bool spsummonFlag = lv4Count == 1;
            spsummonFlag |= !CheckWhetherNegated(true, true, CardType.Monster) && Bot.Graveyard.Any(c => !c.HasType(CardType.Xyz) && c.HasSetcode(SetcodeRyzeal) && c.Level == 4);
            return spsummonFlag;
        }

        public bool NodeRyzealSpSummonFirst()
        {
            int lv4Count = GetLevel4CountOnField();
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Deck | CardLocation.Extra))
            {
                bool flag = Bot.GetMonsters().Any(c => c.IsFaceup() && c.HasType(CardType.Xyz));
                flag |= lv4Count >= 2;
                flag |= lv4Count == 1 && summonCount > 0;
                if (flag) return false;
            }
            if (!activatedCardIdList.Contains(Card.Id) && GetCostFromHandAndField(Card, false).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public bool NodeRyzealActivate()
        {
            if (NodeRyzealActivateInner(true))
            {
                return true;
            }

            return false;
        }

        public bool NodeRyzealActivateFirst()
        {
            if (NodeRyzealActivateInner(false))
            {
                return true;
            }

            return false;
        }

        public bool NodeRyzealActivateInner(bool sendNotNessary)
        {
            if (CheckWhetherNegated(true)) return false;
            if (CheckShouldNoMoreSpSummon(CardLocation.Grave | CardLocation.Extra))
            {
                if (Bot.GetMonsters().Count(c => c.HasType(CardType.Xyz) && c.IsFaceup()) > 0) return false;
            }

            ClientCard nonLightDarkTarget = Bot.Graveyard.Where(c => c != null && !c.HasType(CardType.Xyz) && c.HasSetcode(SetcodeRyzeal) && c.Level == 4
                && !c.HasAttribute((CardAttribute)attrbuteLightDark)).OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
            ClientCard normalTarget = Bot.Graveyard.Where(c => c != null && !c.HasType(CardType.Xyz) && c.HasSetcode(SetcodeRyzeal) && c.Level == 4
                && c != nonLightDarkTarget).OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
            if (nonLightDarkTarget == null || normalTarget == null) return false;
            List<ClientCard> rebornTarget = new List<ClientCard> { nonLightDarkTarget, normalTarget };

            List<ClientCard> firstCostList = GetCostFromHandAndField(Card, false);
            if (firstCostList.Count() > 0)
            {
                AI.SelectCard(firstCostList);
                AI.SelectNextCard(rebornTarget);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            if (GetLevel4CountOnField() == 1 && sendNotNessary)
            {
                List<ClientCard> nextCostList = GetCostFromHandAndField(Card, true);
                if (nextCostList.Count() > 0)
                {
                    AI.SelectCard(nextCostList);
                    AI.SelectNextCard(rebornTarget);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }

            return false;
        }

        public bool ExRyzealSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Extra))
            {
                bool checkFlag = !CheckWhetherHaveFinalMonster() && GetLevel4CountOnField() == 1;
                if (checkFlag && ExRyzealDiscardExtra())
                {
                    return true;
                }
                return false;
            }
            if (Duel.Turn == 1)
            {
                bool checkFlag = !activatedCardIdList.Contains(CardId.ExRyzeal) && !lockBirdSolved && !DefaultCheckWhetherCardIdIsNegated(CardId.ExRyzeal) && !Bot.HasInMonstersZone(_CardId.Number41BagooskatheTerriblyTiredTapir);
                checkFlag |= !Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz)) && GetLevel4CountOnField() == 1;
                if (checkFlag)
                {
                    // discard ryzeal
                    AI.SelectCard(CardId.RyzealDeadnader, CardId.RyzealDuodrive);
                    return true;
                }
            }
            if (ExRyzealDiscardExtra())
            {
                return true;
            }

            return false;
        }

        public bool ExRyzealDiscardExtra()
        {
            List<int> discardIdCheckList = new List<int> { CardId.MereologicAggregator, CardId.TwinsOfTheEclipse, CardId.Number104Masquerade, CardId.StarliegePhotonBlastDragon,
                CardId.TornadoDragon, CardId.AbyssDweller, _CardId.EvilswarmExcitonKnight, CardId.FullArmoredUtopicRayLancer, CardId.Number60DugaresTheTimeless,
                CardId.RyzealDuodrive, CardId.RyzealDeadnader};

            // delay id
            List<int> discardIdList = new List<int>();
            foreach (int discardId in discardIdCheckList)
            {
                if (discardId == CardId.MereologicAggregator)
                {
                    if (!Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeMonsterTarget())
                        && Enemy.SpellZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled() && !c.IsShouldNotBeMonsterTarget()))
                    {
                        continue;
                    }
                }
                if (discardId == CardId.TwinsOfTheEclipse)
                {
                    if (Bot.Graveyard.Count(c => c.HasType(CardType.Xyz)) < 2 || !Bot.Graveyard.Any(c => c.HasType(CardType.Xyz) && c.IsCanRevive()))
                    {
                        continue;
                    }
                }
                if (discardId == CardId.Number104Masquerade)
                {
                    if (CheckRemainInDeck(CardId.SeventhTachyon) > 0 || Bot.HasInHandOrInSpellZone(CardId.Number104Masquerade))
                    {
                        continue;
                    }
                }
                if (discardId == CardId.TornadoDragon)
                {
                    if (Enemy.GetSpellCount() > 0)
                    {
                        continue;
                    }
                }
                discardIdList.Add(discardId);
            }
            discardIdList.AddRange(discardIdCheckList);

            foreach (int id in discardIdList)
            {
                if (Bot.HasInExtra(id))
                {
                    AI.SelectCard(id);
                    return true;
                }
            }

            return false;
        }

        public bool ExRyzealSpSummonLater()
        {
            // TODO
            return false;
        }

        public bool ExRyzealSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra))
            {
                if (GetLevel4CountOnField() == 1)
                {
                    bool flag = !activatedCardIdList.Contains(CardId.ExRyzeal);
                    flag &= !(Bot.HasInHand(CardId.ThodeRyzeal) && !activatedCardIdList.Contains(CardId.ThodeRyzeal));
                    if (flag)
                    {
                        summonCount -= 1;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ExRyzealActivate()
        {
            if (CheckWhetherNegated(true))
            {
                // use mero to negate No.41?
                int count41 = Enemy.GetMonsters().Count(c => c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir) && c.IsFaceup()
                    && !c.IsDisabled() && c.HasPosition(CardPosition.FaceUpDefence));
                bool canNegate41 = count41 == 1 && currentCanActivateEffect.Any(c => c != null && c.IsCode(CardId.MereologicAggregator));

                if (canNegate41)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                } else
                {
                    return false;
                }
            }
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool MulcharmyFuwalosActivate()
        {
            if (CheckWhetherNegated(true) || Duel.Player == 0) return false;
            if (lockBirdSolved || Duel.CurrentChain.Any(c => c.IsCode(_CardId.LockBird))) return false;
            if (Duel.Phase > DuelPhase.Main1) return false;

            botActivateMulcharmy = true;
            return true;
        }

        public bool MulcharmyPuruliaActivate()
        {
            if (CheckWhetherNegated(true) || Duel.Player == 0) return false;
            if (lockBirdSolved || Duel.CurrentChain.Any(c => c.IsCode(_CardId.LockBird))) return false;
            if (Duel.Phase > DuelPhase.Main1) return false;
            if (botActivateMulcharmy) return false;

            botActivateMulcharmy = true;
            return true;
        }

        public bool MulcharmyNyalusActivate()
        {
            if (CheckWhetherNegated(true) || Duel.Player == 0) return false;
            if (lockBirdSolved || Duel.CurrentChain.Any(c => c.IsCode(_CardId.LockBird))) return false;
            if (Duel.Phase > DuelPhase.Main1) return false;
            if (botActivateMulcharmy) return false;

            botActivateMulcharmy = true;
            return true;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
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
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard == null || lastChainCard.IsDisabled()) return false;
            if (lastChainCard.IsMonster() && !lastChainCard.HasType(CardType.Link | CardType.Fusion | CardType.Synchro | CardType.Xyz)) return false;
            return true;
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0 || lockBirdSolved) return false;
            return DefaultMaxxC();
        }

        public bool LockBirdActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0) return false;
            List<int> mulcharmyCheckIdList = new List<int> { _CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos };
            if (mulcharmyCheckIdList.Intersect(botSolvedCardIdList).Any())
            {
                int enemyFieldCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
                if (enemyFieldCount + 6 < Bot.Hand.Count()) return false;
            }
            if (botSolvedCardIdList.Contains(_CardId.MaxxC))
            {
                if (!activatedCardIdList.Contains(_CardId.AshBlossom) || !activatedCardIdList.Contains(_CardId.EffectVeiler)) return false;
            }
            return true;
        }

        public bool EffectVeilerActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            // negate monster
            List<ClientCard> shouldNegateList = GetMonsterListForTargetNegate(true, CardType.Monster);
            if (shouldNegateList.Count > 0)
            {
                ClientCard negateTarget = shouldNegateList[0];
                currentNegateCardList.Add(negateTarget);
                AI.SelectCard(negateTarget);
                return true;
            }

            return false;
        }

        public bool SeventhTachyonActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            bool flag = !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !Bot.HasInHand(CardId.ExRyzeal);
            flag &= !(!activatedCardIdList.Contains(CardId.IceRyzeal) && summonCount > 0 && Bot.HasInHand(CardId.IceRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.IceRyzeal));
            if (flag)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool TripleTacticsTalentActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (TripleTacticsTalentDecision(null) == -1) return false;
            SelectSTPlace(Card, true);
            return true;
        }

        public int TripleTacticsTalentDecision(IList<int> options)
        {
            // gain control?
            bool dangerFlag = Enemy.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled() && (c.IsFloodgate()
            || c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir) && c.HasPosition(CardPosition.FaceUpDefence)));
            if (dangerFlag)
            {
                if (options == null) return 1;
                int res = options.IndexOf(Util.GetStringId(CardId.TripleTacticsTalent, 1));
                if (res >= 0) return res;
            }
            // draw?
            if (!lockBirdSolved)
            {
                bool checkFlag = CheckCanContinueSummon();
                if (!checkFlag)
                {
                    if (options == null) return 1;
                    int res = options.IndexOf(Util.GetStringId(CardId.TripleTacticsTalent, 0));
                    if (res >= 0) return res;
                }
            }
            // shuffle hand
            if (Enemy.Hand.Count() > 0)
            {
                if (options == null) return 1;
                int res = options.IndexOf(Util.GetStringId(CardId.TripleTacticsTalent, 2));
                if (res >= 0) return res;
            }
            return -1;
        }

        public bool PotOfDesiresActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Bot.Deck.Count >= 15)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool PotOfDesireActivateForContinue()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Bot.Deck.Count >= 15 && !CheckCanContinueSummon() && CheckRemainInDeck(CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal) > 0)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool BonfireActivateToSearchNecessary()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            bool flag = !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !Bot.HasInHand(CardId.ExRyzeal);
            flag |= !activatedCardIdList.Contains(CardId.IceRyzeal) && summonCount > 0 && !Bot.HasInHand(CardId.IceRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.IceRyzeal);
            if (flag)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool BonfireActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            // activate before no60
            if (!activatedCardIdList.Contains(CardId.Number60DugaresTheTimeless))
            {
                ClientCard no60 = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.Number60DugaresTheTimeless) && !c.IsDisabled());
                if (no60 != null && no60.Overlays.Count() >= 2)
                {
                    SelectSTPlace(Card, true);
                    return true;
                }
            }

            return false;
        }

        public bool CalledbytheGraveActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            ClientCard lastChainCard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (lastChainCard != null && lastChainCard.IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (DefaultCheckWhetherCardIdIsNegated(code)) return false;

                    // not to negate same card in hand
                    List<int> mulcharmyIdList = new List<int> { _CardId.MulcharmyPurulia, _CardId.MulcharmyFuwalos, _CardId.MulcharmyNyalus };
                    if (Duel.Player == 0 && Bot.HasInHand(code) && !mulcharmyIdList.Contains(code)) return false;

                    ClientCard graveTarget = Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.GetOriginCode() == code);
                    if (graveTarget != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(graveTarget);
                        currentDestroyCardList.Add(graveTarget);
                        currentNegateCardList.AddRange(Enemy.MonsterZone.Where(c => c != null && c.IsFaceup() && c.IsCode(code)));
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
            if (CheckWhetherNegated(true)) return false;

            // spsummon lv4
            if (Duel.Player == 0 && CurrentTiming == -1)
            {
                bool summonFlag = GetLevel4CountOnField() == 1;
                if (GetLevel4CountOnField() == 0)
                {
                    summonFlag |= GetLevel4FinalCountOnField(true, out _) >= 2 && !CheckWhetherHaveFinalMonster();
                }

                // summon extra level4 monster to xyz summon
                if (summonFlag)
                {
                    List<int> checkIdList = new List<int> { CardId.NodeRyzeal, CardId.ExRyzeal, CardId.ThodeRyzeal, CardId.IceRyzeal };
                    foreach (int id in checkIdList)
                    {
                        if (activatedCardIdList.Contains(id)) continue;
                        ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        if (target == null)
                        {
                            target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        }
                        if (target != null)
                        {
                            AI.SelectCard(target);
                            SelectSTPlace(Card, true);
                            return true;
                        }
                    }

                    foreach (int id in checkIdList)
                    {
                        ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        if (target == null)
                        {
                            target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        }
                        if (target != null)
                        {
                            AI.SelectCard(target);
                            SelectSTPlace(Card, true);
                            return true;
                        }
                    }
                }
            }

            // spsummon deadnader
            if (!Bot.HasInMonstersZone(CardId.RyzealDeadnader, true, true, true) && !Duel.CurrentChain.Any(c => c.IsCode(CardId.TwinsOfTheEclipse))
                && !DefaultCheckWhetherCardIdIsNegated(CardId.RyzealDeadnader) && !Util.ChainContainPlayer(0))
            {
                ClientCard deadnader = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.RyzealDeadnader) && c.IsCanRevive());
                if (deadnader == null)
                {
                    deadnader = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.RyzealDeadnader) && c.IsCanRevive());
                }
                if (deadnader != null)
                {
                    AI.SelectCard(deadnader);
                    SelectSTPlace(Card, true);
                    return true;
                }
            }

            // chain to negate monster effect
            if (Bot.HasInSpellZone(CardId.RyzealCross, true, true) && !activatedCardIdList.Contains(CardId.RyzealCross + 2)
                && CheckRemainInDeck(CardId.ExRyzeal, CardId.IceRyzeal, CardId.NodeRyzeal, CardId.ThodeRyzeal) > 0)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.IsMonster() && lastChainCard.Controller == 1 && CheckCardShouldNegate(lastChainCard))
                {
                    bool activateFlag = false;
                    bool shouldRebornXyz = false;
                    bool duodriverActivating = Duel.CurrentChain.Any(c => c.IsCode(CardId.RyzealDuodrive) && c.Controller == 0) && activatedCardIdList.Contains(CardId.RyzealDuodrive + 1);
                    if (duodriverActivating)
                    {
                        // check whether have 3+ material
                        activateFlag = Bot.MonsterZone.Where(c => c != null && c.HasType(CardType.Xyz)).Sum(c => c.Overlays.Count()) >= 3;
                    } else
                    {
                        if (!Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && c.Overlays.Count() > 0))
                        {
                            activateFlag |= Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz) && c.HasSetcode(SetcodeRyzeal));
                            if (!activateFlag)
                            {
                                bool hasXyzTarget = Bot.Banished.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && c.IsCanRevive() && c.HasType(CardType.Xyz));
                                hasXyzTarget |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && c.IsCanRevive() && c.HasType(CardType.Xyz));
                                if (hasXyzTarget)
                                {
                                    activateFlag = true;
                                    shouldRebornXyz = true;
                                }
                            }
                        }
                    }

                    if (activateFlag)
                    {
                        ClientCard deadnader = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));
                        if (deadnader == null)
                        {
                            deadnader = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));
                        }
                        if (deadnader != null)
                        {
                            AI.SelectCard(deadnader);
                            SelectSTPlace(Card, true);
                            return true;
                        }

                        if (shouldRebornXyz)
                        {
                            ClientCard duoDriver = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDuodrive));
                            if (duoDriver == null)
                            {
                                duoDriver = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDuodrive));
                            }
                            if (duoDriver != null)
                            {
                                AI.SelectCard(duoDriver);
                                SelectSTPlace(Card, true);
                                return true;
                            }
                        } else
                        {
                            List<int> checkIdList = new List<int> { CardId.NodeRyzeal, CardId.ExRyzeal, CardId.ThodeRyzeal, CardId.IceRyzeal };
                            foreach (int id in checkIdList)
                            {
                                if (activatedCardIdList.Contains(id)) continue;
                                ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                                if (target == null)
                                {
                                    target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                                }
                                if (target != null)
                                {
                                    AI.SelectCard(target);
                                    SelectSTPlace(Card, true);
                                    return true;
                                }
                            }
                            foreach (int id in checkIdList)
                            {
                                ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                                if (target == null)
                                {
                                    target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                                }
                                if (target != null)
                                {
                                    AI.SelectCard(target);
                                    SelectSTPlace(Card, true);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            bool becomeTargetFlag = DefaultOnBecomeTarget() && Card.Location == CardLocation.SpellZone;
            bool endPhaseFlag = Duel.Player == 1 && Duel.Phase == DuelPhase.End;
            if (becomeTargetFlag || endPhaseFlag)
            {
                if (!(Duel.CurrentChain.Any(c => c != null && c.Controller == 1 && c.IsCode(_CardId.EvenlyMatched)) && deadnaderDestroySelf != null))
                {
                    ClientCard deadnader = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));
                    if (deadnader == null)
                    {
                        deadnader = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDeadnader));
                    }
                    if (deadnader != null)
                    {
                        AI.SelectCard(deadnader);
                        SelectSTPlace(Card, true);
                        return true;
                    }

                    ClientCard duoDriver = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDuodrive));
                    if (duoDriver == null)
                    {
                        duoDriver = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCanRevive() && c.IsCode(CardId.RyzealDuodrive));
                    }
                    if (duoDriver != null)
                    {
                        AI.SelectCard(duoDriver);
                        SelectSTPlace(Card, true);
                        return true;
                    }

                    List<int> checkIdList = new List<int> { CardId.NodeRyzeal, CardId.ExRyzeal, CardId.ThodeRyzeal, CardId.IceRyzeal };
                    foreach (int id in checkIdList)
                    {
                        if (activatedCardIdList.Contains(id)) continue;
                        ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        if (target == null)
                        {
                            target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                        }
                        if (target != null)
                        {
                            AI.SelectCard(target);
                            SelectSTPlace(Card, true);
                            return true;
                        }
                    }

                    if (!endPhaseFlag)
                    {
                        foreach (int id in checkIdList)
                        {
                            ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                            if (target == null)
                            {
                                target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(id));
                            }
                            if (target != null)
                            {
                                AI.SelectCard(target);
                                SelectSTPlace(Card, true);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool RyzealPlugInActivateFirst()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Duel.Player == 0 && CurrentTiming == -1 && !activatedCardIdList.Contains(CardId.NodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.NodeRyzeal))
            {
                List<ClientCard> nodeCostList = GetCostFromHandAndField(Card, false);
                if (nodeCostList.Count() > 0)
                {
                    ClientCard target = Bot.Banished.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.NodeRyzeal));
                    if (target == null)
                    {
                        target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.NodeRyzeal));
                    }
                    if (target != null)
                    {
                        AI.SelectCard(target);
                        SelectSTPlace(Card, true);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool RyzealCrossActivateCard()
        {
            // whether to negate by cross
            if (ActivateDescription == Util.GetStringId(CardId.RyzealCross, 3))
            {
                ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
                if (currentSolvingChain != null && !Duel.IsCurrentSolvingChainNegated())
                {
                    if (CheckCardShouldNegate(currentSolvingChain))
                    {
                        Logger.DebugWriteLine("** cross negate");
                        activatedCardIdList.Add(CardId.RyzealCross + 2);
                        botSolvingCross = true;
                        return true;
                    }
                }
                return false;
            }

            if (CheckWhetherNegated(true)) return false;
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup()) return false;
            bool flag = RyzealCrossActivateRecycleFirst();
            bool canSetMaterial = Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn) && CheckRemainInDeck(CardId.IceRyzeal, CardId.ExRyzeal, CardId.NodeRyzeal, CardId.ThodeRyzeal) > 0
                && (Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeRyzeal) && (c.IsCanRevive() || !c.HasType(CardType.Xyz))) ||
                    Bot.Banished.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && (c.IsCanRevive() || !c.HasType(CardType.Xyz))));
            flag |= Bot.MonsterZone.Count(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz) && c.HasSetcode(SetcodeRyzeal) && (c.Overlays.Count() > 0 || canSetMaterial)) > 0;

            return flag;
        }

        public bool RyzealCrossActivateRecycleFirst()
        {
            if (CheckWhetherNegated(true) || !(Card.Location == CardLocation.SpellZone && Card.IsFaceup())) return false;
            if (ActivateDescription == Util.GetStringId(CardId.RyzealCross, 3))
            {
                return false;
            }
            List<int> checkIdList = new List<int> { CardId.RyzealPlugIn, CardId.RyzealDuodrive, CardId.RyzealDeadnader, CardId.NodeRyzeal, CardId.ExRyzeal, CardId.IceRyzeal, CardId.ThodeRyzeal };
            List<ClientCard> targetList = new List<ClientCard>();
            foreach (int id in checkIdList)
            {
                ClientCard target = Bot.Graveyard.FirstOrDefault(c => c.IsCode(id));
                if (target != null && (CheckRemainInDeck(id) + Bot.ExtraDeck.Count(c => c.IsCode(id)) + Bot.Hand.Count(c => c.IsCode(id))) == 0)
                {
                    if (target.HasType(CardType.Xyz) && GetLevel4CountOnField() == 1) continue;
                    targetList.Add(target);
                }

                if (targetList.Count() >= 2)
                {
                    AI.SelectCard(targetList);
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            }

            return false;
        }

        public bool RyzealCrossActivateRecycleLater()
        {
            if (CheckWhetherNegated(true) || !(Card.Location == CardLocation.SpellZone && Card.IsFaceup())) return false;
            if (ActivateDescription == Util.GetStringId(CardId.RyzealCross, 3))
            {
                return false;
            }
            SortedDictionary<int, List<int>> countDict = new SortedDictionary<int, List<int>>();
            List<int> checkIdList = new List<int> { CardId.RyzealPlugIn, CardId.RyzealDuodrive, CardId.RyzealDeadnader, CardId.NodeRyzeal, CardId.ExRyzeal, CardId.IceRyzeal, CardId.ThodeRyzeal };
            foreach (int id in checkIdList)
            {
                int remainCount = CheckRemainInDeck(id) + Bot.ExtraDeck.Count(c => c.IsCode(id));
                if (!countDict.ContainsKey(remainCount))
                {
                    countDict.Add(remainCount, new List<int>());
                }
                countDict[remainCount].Add(id);
            }

            List<ClientCard> targetList = new List<ClientCard>();
            foreach (KeyValuePair<int, List<int>> pair in countDict)
            {
                foreach (int id in pair.Value)
                {
                    ClientCard target = Bot.Graveyard.FirstOrDefault(c => c.IsCode(id));
                    if (target != null) targetList.Add(target);
                }
            }

            if (targetList.Count() >= 2)
            {
                AI.SelectCard(targetList);
                activatedCardIdList.Add(Card.Id + 1);
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
                    currentNegateCardList.AddRange(Enemy.MonsterZone.Where(c => c != null && c.IsFaceup() && c.IsCode(code)));
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

        // TODO extra summon

        public bool AbyssDwellerSummonCheck()
        {
            bool flag = enemyDeckTypeRecord.Contains(SetcodeAtlantean);
            flag |= enemyDeckTypeRecord.Contains(SetcodeOrcust);
            flag |= enemyDeckTypeRecord.Contains(SetcodePhantomKnight);
            flag |= enemyDeckTypeRecord.Count() > 0 && enemyDeckTypeRecord.All(c => c == SetcodeTearlaments);

            int enemyDeckTotalCount = Enemy.Hand.Count() + Enemy.Deck.Count() + Enemy.Graveyard.Count() + Enemy.Banished.Count() + Enemy.ExtraDeck.Count();
            if (enemyDeckTotalCount > 65)
            {
                flag |= !enemyDeckTypeRecord.Contains(SetcodeInfernoid);
            }

            return flag;
        }

        public bool Number41BagooskatheTerriblyTiredTapirSummonCheck()
        {
            bool flag = enemyDeckTypeRecord.Contains(SetcodeFloowandereeze);
            flag |= enemyDeckTypeRecord.Contains(SetcodeBranded);
            flag |= enemyDeckTypeRecord.Contains(SetcodeRyzeal);
            flag &= Util.IsTurn1OrMain2();

            return flag;
        }

        public bool TornadoDragonSummonCheck()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            bool flag = enemyDeckTypeRecord.Contains(SetcodeLabrynth);
            flag |= Enemy.SpellZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled() && c.IsFloodgate());
            flag |= Enemy.SpellZone.Count(c => c != null && !c.IsShouldNotBeMonsterTarget() && !NotToDestroySpellTrap.Contains(c.Id)) >= 3;
            flag |= !Util.IsTurn1OrMain2() && !botSolvedCardIdList.Contains(_CardId.EvilswarmExcitonKnight) && Enemy.GetMonsterCount() == 0
                   && Enemy.SpellZone.Count(c => c != null && !c.IsShouldNotBeMonsterTarget() && !NotToDestroySpellTrap.Contains(c.Id)) > 0;

            return flag;
        }

        public bool EvilswarmExcitonKnightSpSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || Duel.Turn == 1) return false;

            return DefaultEvilswarmExcitonKnightSummon();
        }

        public bool LessSpSummonExtra()
        {
            if (!CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;

            // No.41
            ClientCard no41 = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
            if (no41 != null)
            {
                if (Number41BagooskatheTerriblyTiredTapirSummonCheck())
                {
                    if (Card != no41) return false;

                    // Select 2 monster with lowest defense
                    List<ClientCard> materialList = GetLevel4OnField(null);
                    if (materialList.Count() >= 2)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }
            // abyss
            ClientCard abyss = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.AbyssDweller));
            if (abyss != null)
            {
                if (AbyssDwellerSummonCheck())
                {
                    if (Card != abyss) return false;

                    // Select 2 monster with lowest defense
                    List<ClientCard> materialList = GetLevel4OnField(null);
                    if (materialList.Count() >= 2)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }
            // deadnader
            ClientCard deadnader = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.RyzealDeadnader));
            if (deadnader != null)
            {
                List<ClientCard> materialList = GetLevel4OnField(c => c.HasSetcode(SetcodeRyzeal));
                if (materialList.Count() >= 2)
                {
                    if (Card != deadnader) return false;
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }

            return false;
        }

        public bool FirstRyzealDuodriveSpSummon()
        {
            if (!RyzealDuodriveSpSummonCheck()) return false;
            if (Bot.Graveyard.Count(c => c.HasSetcode(SetcodeRyzeal) && c.IsMonster()) == 0)
            {
                if (!CheckShouldNoMoreSpSummon(CardLocation.Hand) && Bot.HasInHand(CardId.ExRyzeal)
                    && !spSummonedCardIdList.Contains(CardId.ExRyzeal) && Duel.MainPhase.SpecialSummonableCards.Any(c => c.IsCode(CardId.ExRyzeal))
                    && Bot.ExtraDeck.Count(c => c.IsCode(CardId.RyzealDeadnader, CardId.RyzealDuodrive)) > 2)
                {
                    return false;
                }
            }

            List<ClientCard> materialList = GetLevel4OnField(null);
            List<ClientCard> materialExceptNode = materialList
                .Where(c => !(c.IsCode(CardId.NodeRyzeal) && !c.IsDisabled() && !activatedCardIdList.Contains(CardId.NodeRyzeal))).ToList();

            if (materialExceptNode.Count() >= 2)
            {
                AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                return true;
            }
            if (materialList.Count() > 2 && !CheckCanContinueSummon())
            {
                AI.SelectMaterials(materialList.Take(2).ToList());
                return true;
            }
            if (materialList.Count() >= 2 && !CheckCanContinueSummon(true))
            {
                AI.SelectMaterials(materialList.Take(2).ToList());
                return true;
            }

            return false;
        }

        public bool RyzealDuodriveSpSummonCheck()
        {
            bool checkFlag = Duel.MainPhase.SpecialSummonableCards.Any(c => c.IsCode(CardId.RyzealDuodrive));
            checkFlag &= !Bot.HasInMonstersZone(CardId.RyzealDuodrive, true, true, true);
            checkFlag &= CheckRemainInDeck(CardId.IceRyzeal, CardId.ThodeRyzeal, CardId.NodeRyzeal, CardId.ExRyzeal, CardId.RyzealPlugIn, CardId.RyzealCross) >= 2;
            checkFlag &= !DefaultCheckWhetherCardIdIsNegated(CardId.RyzealDuodrive);
            checkFlag &= !activatedCardIdList.Contains(CardId.RyzealDuodrive + 1);
            checkFlag &= !CheckWhetherNegated(true, true, CardType.Monster);
            checkFlag &= !lockBirdSolved;
            checkFlag &= !CheckShouldNoMoreSpSummon(CardLocation.Extra);

            return checkFlag;
        }

        public bool SecondXyzSummon()
        {
            if (Card.Location != CardLocation.Extra) return false;

            int level4Count = GetLevel4FinalCountOnField(true, out _);
            bool result = SecondXyzSummonInner();
            Logger.DebugWriteLine("Second Xyz Count: " + level4Count.ToString());
            Logger.DebugWriteLine("Second Xyz Summon: " + result.ToString());
            return result;
        }

        public bool SecondXyzSummonInner()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;

            // summon after duo driver
            if (RyzealDuodriveSpSummonCheck()) {
                Logger.DebugWriteLine("Second: summon duodriver first");
                return false;
            }

            // check whether have 4 monsters for material.
            // if not, skip the second xyz monster.
            int level4Count = GetLevel4FinalCountOnField(true, out bool hasNode);
            if (level4Count < 4) return false;

            // select which monster to summon
            List<ClientCard> materialList = GetLevel4OnField(null);
            List<ClientCard> materialExceptNode = materialList
                .Where(c => !(c.IsCode(CardId.NodeRyzeal) && !c.IsDisabled() && !activatedCardIdList.Contains(CardId.NodeRyzeal))).ToList();

            // abyss
            ClientCard abyss = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.AbyssDweller));
            if (abyss != null)
            {
                if (AbyssDwellerSummonCheck())
                {
                    if (Card != abyss) return false;

                    if (materialExceptNode.Count() >= 2)
                    {
                        AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                        return true;
                    }
                    if (materialList.Count() > 2 && !CheckCanContinueSummon())
                    {
                        AI.SelectMaterials(materialList.Take(2).ToList());
                        return true;
                    }
                }
            }
            // 41
            ClientCard no41 = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
            if (no41 != null)
            {
                bool flag = hasNode;
                flag &= Util.IsTurn1OrMain2();
                flag &= Bot.HasInExtra(CardId.TwinsOfTheEclipse) && Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz));
                flag &= (GetNegateEffectCount() >= 2 || lockBirdSolved);

                if (flag)
                {
                    if (Card != no41) return false;

                    if (materialExceptNode.Count() >= 2)
                    {
                        AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                        return true;
                    }
                    if (materialList.Count() >= 2 && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                    {
                        AI.SelectMaterials(materialList.Take(2).ToList());
                        return true;
                    }
                }
            }

            // photon blast dragon
            ClientCard photonDragon = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.StarliegePhotonBlastDragon));
            bool have2MaterialDuo = false;
            if (photonDragon != null)
            {
                int duoDriveOverlayCount = 0;
                foreach (ClientCard monster in Bot.MonsterZone)
                {
                    if (monster == null || !monster.IsCode(CardId.RyzealDuodrive)) continue;
                    duoDriveOverlayCount += monster.Overlays.Count();
                }

                if (Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn)) duoDriveOverlayCount++;
                have2MaterialDuo = duoDriveOverlayCount >= 2;
            }

            if (photonDragon != null && have2MaterialDuo && enemyDeckTypeRecord.Contains(SetcodeSkyStriker))
            {
                if (Card != photonDragon) return false;

                if (materialExceptNode.Count() >= 2)
                {
                    AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                    return true;
                }
            }

            // 60
            ClientCard no60 = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.Number60DugaresTheTimeless));
            if (no60 != null && !lockBirdSolved)
            {
                bool flag = Bot.Deck.Count() > 2;

                if (flag)
                {
                    if (Card != no60) return false;

                    if (materialExceptNode.Count() >= 2)
                    {
                        AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                        return true;
                    }
                    if (materialList.Count() >= 2 && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                    {
                        AI.SelectMaterials(materialList.Take(2).ToList());
                        return true;
                    }
                }
            }

            if (photonDragon != null && have2MaterialDuo)
            {
                if (Card != photonDragon) return false;

                if (materialExceptNode.Count() >= 2)
                {
                    AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                    return true;
                }
            }

            // tornado dragon
            ClientCard deadnader = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.RyzealDeadnader));
            if (deadnader == null)
            {
                ClientCard tornadoDragon = Duel.MainPhase.SummonableCards.FirstOrDefault(c => c.IsCode(CardId.TornadoDragon));
                if (tornadoDragon != null && TornadoDragonSummonCheck() && Card == tornadoDragon)
                {
                    if (materialExceptNode.Count() >= 2)
                    {
                        AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                        return true;
                    }
                    if (materialList.Count() >= 2 && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                    {
                        AI.SelectMaterials(materialList.Take(2).ToList());
                        return true;
                    }
                }
            }

            // deadnader
            if (deadnader != null && Card == deadnader)
            {
                if (materialExceptNode.Count() >= 2)
                {
                    AI.SelectMaterials(materialExceptNode.Take(2).ToList());
                    return true;
                }
                if (materialList.Count() >= 2 && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                {
                    AI.SelectMaterials(materialList.Take(2).ToList());
                    return true;
                }
            }

            // TODO
            Logger.DebugWriteLine("Second: no monster to spsummon");


            return false;
        }

        public bool TwinsOfTheEclipseSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra)) return false;
            if (Util.IsTurn1OrMain2())
            {
                bool hasNode = Bot.HasInHand(CardId.NodeRyzeal) && !spSummonedCardIdList.Contains(CardId.NodeRyzeal);
                hasNode |= Bot.HasInMonstersZone(CardId.NodeRyzeal, true, false, true);
                if (Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
                {
                    hasNode |= Bot.Graveyard.Any(c => c.IsCode(CardId.NodeRyzeal));
                    hasNode |= Bot.Banished.Any(c => c.IsFaceup() && c.IsCode(CardId.NodeRyzeal));
                }
                hasNode &= !activatedCardIdList.Contains(CardId.NodeRyzeal) && !DefaultCheckWhetherCardIdIsNegated(CardId.NodeRyzeal);

                List<ClientCard> materialList = new List<ClientCard>();
                ClientCard duoDriver = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.RyzealDuodrive));
                if (activatedCardIdList.Contains(CardId.RyzealDuodrive + 1) && duoDriver != null)
                {
                    materialList.Add(duoDriver);
                    hasNode |= !CheckWhetherWillbeRemoved() && duoDriver.Overlays.Any(id => id == CardId.NodeRyzeal);
                }
                ClientCard no60 = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.Number60DugaresTheTimeless));
                if (no60 != null && (activatedCardIdList.Contains(CardId.Number60DugaresTheTimeless) || no60.IsDisabled()))
                {
                    materialList.Add(no60);
                    hasNode |= !CheckWhetherWillbeRemoved() && no60.Overlays.Any(id => id == CardId.NodeRyzeal);
                }
                ClientCard no41 = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
                if (no41 != null)
                {
                    materialList.Add(no41);
                    hasNode |= !CheckWhetherWillbeRemoved() && no41.Overlays.Any(id => id == CardId.NodeRyzeal);
                }

                if (materialList.Count() >= 2 && hasNode)
                {
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }
            else
            {
                if (botSolvedCardIdList.Contains(_CardId.EvilswarmExcitonKnight))
                {
                    return false;
                }

                List<ClientCard> materialList = GetLevel4OnField(null);
                List<ClientCard> xyzMonsterList = Bot.MonsterZone.Where(c => c != null && c.IsFaceup() && c.HasType(CardType.Xyz) && c.Rank == 4 && c.Attack < 2500).ToList();
                int level4Count = GetLevel4FinalCountOnField(true, out bool hasNode) + xyzMonsterList.Count();
                
                if (level4Count < 4) return false;

                materialList.AddRange(xyzMonsterList);
                materialList = materialList.Where(c => c != null && c.Attack < 2500).OrderBy(c => c.GetDefensePower()).Take(2).ToList();
                if (materialList.Count() >= 2)
                {
                    // check whether enemy have monster with 2more less than 2500
                    bool checkFlag = Enemy.MonsterZone.Count(c => c != null && c.GetDefensePower() < 2500) >= 2 && !CheckWhetherNegated(true, true, CardType.Monster)
                        && !DefaultCheckWhetherCardIdIsNegated(CardId.TwinsOfTheEclipse);
                    if (checkFlag && materialList.Sum(c => c.Attack) < 5000)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                    else if (materialList.Sum(c => botSolvedCardIdList.Contains(CardId.RyzealPlugIn) && !c.HasType(CardType.Xyz) ? 0 : c.Attack) < 2500
                        && !Duel.MainPhase.SpecialSummonableCards.Any(c => c.IsCode(CardId.RyzealDeadnader)))
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool FinalXyzSummon()
        {
            if (Card.Location != CardLocation.Extra) return false;

            int level4Count = GetLevel4FinalCountOnField(false, out _);
            bool result = FinalXyzSummonInner();
            Logger.DebugWriteLine("Final Xyz Count: " + level4Count.ToString());
            Logger.DebugWriteLine("Final Xyz Summon: " + result.ToString());

            return result;
        }

        public bool FinalXyzSummonInner()
        {
            // summon after duo driver
            if (RyzealDuodriveSpSummonCheck())
            {
                Logger.DebugWriteLine("Final: summon duodriver first");
                return false;
            }

            int level4Count = GetLevel4FinalCountOnField(false, out _);
            if (level4Count >= 4) return false;

            // No.41
            ClientCard no41 = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
            if (no41 != null)
            {
                if (Number41BagooskatheTerriblyTiredTapirSummonCheck())
                {
                    if (Card != no41) return false;

                    // Select 2 monster with lowest defense
                    List<ClientCard> materialList = GetLevel4OnField(null);
                    if (materialList.Count() >= 2)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }

            // abyss
            ClientCard abyss = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.AbyssDweller));
            if (abyss != null)
            {
                if (AbyssDwellerSummonCheck())
                {
                    if (Card != abyss) return false;

                    // Select 2 monster with lowest defense
                    List<ClientCard> materialList = GetLevel4OnField(null);
                    if (materialList.Count() >= 2)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }
            // deadnader
            ClientCard deadnader = Duel.MainPhase.SpecialSummonableCards.FirstOrDefault(c => c.IsCode(CardId.RyzealDeadnader));
            if (deadnader != null)
            {
                List<ClientCard> materialList = GetLevel4OnField(c => c.HasSetcode(SetcodeRyzeal));
                if (materialList.Count() >= 2)
                {
                    if (Card != deadnader) return false;
                    AI.SelectMaterials(materialList);
                    return true;
                }
            }

            // tornado dragon
            if (deadnader == null)
            {
                ClientCard tornadoDragon = Duel.MainPhase.SummonableCards.FirstOrDefault(c => c.IsCode(CardId.TornadoDragon));
                if (tornadoDragon != null && TornadoDragonSummonCheck())
                {
                    // Select 2 monster with lowest defense
                    List<ClientCard> materialList = GetLevel4OnField(null);
                    if (materialList.Count() >= 2)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DonnerDaggerFurHireSpSummon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Extra))
            {
                return false;
            }
            bool haveEnemyTarget = Enemy.MonsterZone.Any(c => c != null && !c.IsShouldNotBeMonsterTarget()) && !CheckWhetherNegated(true, true, CardType.Monster);

            List<ClientCard> illegalList = Bot.GetMonsters().Where(card => card.IsFaceup() && card.Level != 4 && card.Rank != 4).OrderBy(c => c.GetDefensePower()).ToList();
            bool necessary = Bot.HasInHand(CardId.ExRyzeal) && !spSummonedCardIdList.Contains(CardId.ExRyzeal) && !activatedCardIdList.Contains(CardId.ExRyzeal)
                && illegalList.Count() > 0;
            bool needDestory = !CheckWhetherNegated(true, true, CardType.Monster)
                && Enemy.MonsterZone.Any(c => c != null && !c.IsShouldNotBeMonsterTarget() && c.IsFloodgate() && !c.IsDisabled());
            necessary |= needDestory;

            if (necessary)
            {
                if (illegalList.Count() == 1 && haveEnemyTarget)
                {
                    List<ClientCard> otherMaterialList = Bot.GetMonsters().Where(card => card.IsFaceup() && !illegalList.Contains(card) && (card.Owner == 1 || !card.HasType(CardType.Xyz))).ToList();
                    otherMaterialList.Sort(CardContainer.CompareCardAttack);
                    illegalList.AddRange(otherMaterialList);
                }

                if (illegalList.Count() > 1)
                {
                    List<ClientCard> materialList = illegalList.Take(2).ToList();
                    if (Util.GetBotAvailZonesFromExtraDeck(materialList) > 0)
                    {
                        AI.SelectMaterials(materialList);
                        return true;
                    }
                }
            }

            if (Duel.Phase == DuelPhase.Main2)
            {
                List<ClientCard> enemyOwnerMonsters = Bot.MonsterZone.Where(c => c != null && c.IsFaceup() && c.Owner == 1).OrderBy(c => c.GetDefensePower()).ToList();
                if (enemyOwnerMonsters.Count() > 0 && haveEnemyTarget)
                {
                    if (enemyOwnerMonsters.Count() == 1)
                    {
                        List<ClientCard> otherMaterialList = Bot.GetMonsters()
                            .Where(card => card.IsFaceup() && !enemyOwnerMonsters.Contains(card) && (!card.HasType(CardType.Xyz) || card.Overlays.Count() == 0))
                            .OrderBy(c => c.GetDefensePower()).ToList();
                        enemyOwnerMonsters.AddRange(otherMaterialList);
                    }

                    if (enemyOwnerMonsters.Count() > 1)
                    {
                        List<ClientCard> materialList = enemyOwnerMonsters.Take(2).ToList();
                        if (Util.GetBotAvailZonesFromExtraDeck(materialList) > 0)
                        {
                            AI.SelectMaterials(materialList);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // TODO extra effect


        public bool MereologicAggregatorActivateFirst()
        {
            List<Func<ClientCard, bool>> multiNegateFuncList = new List<Func<ClientCard, bool>> {
                {c => c.IsCode(44665365, 48546368, 54178659) && c.IsMonster() },
                {c => c.IsCode(4280258) && c.Attack >= 800 },
                {c => c.IsCode(47297616) && c.Attack >= 500 && c.Defense >= 500 },
                {c => c.IsCode(19652159) && c.Attack >= 1000 && c.Defense >= 1000 },
                {c => c.IsCode(79600447) && Enemy.MonsterZone.Any(m => m != null && m.IsFaceup() && m.IsCode(23288411) && m.Attack >= 1000) }
            };
            List<ClientCard> searchCardList = new List<ClientCard>(Enemy.GetMonsters());
            searchCardList.AddRange(Enemy.GetSpells());
            foreach (ClientCard card in searchCardList)
            {
                if (card == null || card.IsFacedown() || card.IsDisabled()) continue;
                foreach (Func<ClientCard, bool> func in multiNegateFuncList)
                {
                    if (func(card))
                    {
                        AI.SelectCard(card);
                        currentNegateCardList.Add(card);
                        activatedCardIdList.Add(Card.Id + 2);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool MereologicAggregatorActivateLater()
        {
            if (CheckWhetherNegated(true)) return false;

            ClientCard lastChainCard = Util.GetLastChainCard();

            // for Chain1 ExRyzeal Chain2 Mereo to negate No41
            if (lastChainCard != null && lastChainCard.Controller == 0 && lastChainCard.IsCode(CardId.ExRyzeal))
            {
                ClientCard no41 = Enemy.GetMonsters().FirstOrDefault(c =>
                    c.IsFaceup() && !c.IsDisabled() && c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir) && c.HasPosition(CardPosition.FaceUpDefence) && !currentNegateCardList.Contains(c));
                if (no41 != null)
                {
                    currentNegateCardList.Add(no41);
                    AI.SelectCard(no41);
                    activatedCardIdList.Add(Card.Id + 2);
                    return true;
                }
            }

            List<ClientCard> targetList = GetNormalEnemyTargetList(true, false, CardType.Monster, true).Where(c => c.IsFaceup() && !c.IsDisabled()).ToList();
            if (targetList.Count() > 0)
            {
                currentNegateCardList.Add(targetList[0]);
                AI.SelectCard(targetList);
                activatedCardIdList.Add(Card.Id + 2);
                return true;
            }

            // protect chain
            if (lastChainCard != null && lastChainCard.Controller == 0 && lastChainCard.IsCode(CardId.ExRyzeal))
            {
                foreach (ClientCard card in Bot.GetMonsters())
                {
                    if (card.IsFacedown() || Duel.CurrentChain.Contains(card) || card.IsDisabled() || !card.HasType(CardType.Effect)) continue;
                    bool flag = card.IsCode(CardId.IceRyzeal, CardId.ThodeRyzeal);
                    flag |= card.IsCode(CardId.NodeRyzeal) && activatedCardIdList.Contains(CardId.NodeRyzeal);
                    flag |= card.HasType(CardType.Xyz) && !card.HasXyzMaterial() && !card.IsCode(CardId.RyzealDeadnader, CardId.RyzealDuodrive, CardId.FullArmoredUtopicRayLancer);
                    if (flag)
                    {
                        AI.SelectCard(card);
                        activatedCardIdList.Add(Card.Id + 2);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool RyzealDeadnaderActivate()
        {
            if (ActivateDescription == 96)
            {
                Logger.DebugWriteLine("** deadnader replace destroy");
                if (deadnaderDestroySelf != Card)
                {
                    activatedCardIdList.Add(CardId.RyzealDeadnader + 2);
                    return true;
                }
                return false;
            }
            if (CheckWhetherNegated(true)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.RyzealDeadnader, 1))
            {
                // destroy self
                bool shouldDestroySelf = false;
                bool willBeNegated = false;
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.Controller == 1 && lastChainCard.IsCode(targetNegateIdList))
                {
                    shouldDestroySelf = true;
                    willBeNegated = true;
                }
                shouldDestroySelf |= Duel.CurrentChain.Any(c => c != null && c.Controller == 1 && !c.IsDisabled() && !DefaultCheckWhetherCardIdIsNegated(c.Id)
                    && c.IsCode(_CardId.EvenlyMatched, 35480699));
                shouldDestroySelf |= Card.Overlays.Count() == 1 && !activatedCardIdList.Contains(CardId.RyzealDeadnader)
                    && GetProblematicEnemyCardList(true, false, CardType.Monster).Count() == 0;

                if (shouldDestroySelf)
                {
                    bool canRebornSelf = Bot.SpellZone.Count(c => c != null && c.IsFacedown() && c.IsCode(CardId.RyzealPlugIn) && !Duel.ChainTargets.Contains(c)) > 0;
                    bool canActivateTwin = !activatedCardIdList.Contains(CardId.TwinsOfTheEclipse + 1) && !DefaultCheckWhetherCardIdIsNegated(CardId.TwinsOfTheEclipse)
                        && !CheckWhetherWillbeRemoved();
                    canRebornSelf |= canActivateTwin && Card.Overlays.Contains(CardId.TwinsOfTheEclipse);
                    if (Duel.CurrentChain.Any(c => c != null && c.Controller == 1 && !c.IsDisabled() && !DefaultCheckWhetherCardIdIsNegated(c.Id)
                        && c.IsCode(_CardId.EvenlyMatched)))
                    {
                        canRebornSelf |= Bot.MonsterZone.Any(c => c != null && c.HasType(CardType.Xyz) && c.Overlays.Contains(CardId.TwinsOfTheEclipse));
                    }

                    if (canRebornSelf)
                    {
                        deadnaderDestroySelf = Card;
                        return true;
                    }
                }


                // destroy
                if (CanDestroyList(willBeNegated).Count() > 0)
                {
                    return true;
                }
            }
            else
            {
                // attach
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;

        }

        public bool RyzealDuodriveActivate()
        {
            if (CheckWhetherNegated(true)) return false;

            if (ActivateDescription == Util.GetStringId(CardId.RyzealDuodrive, 1))
            {
                int overlayCount = 0;
                foreach (ClientCard card in Bot.MonsterZone)
                {
                    if (card == null || card.Overlays.Count() == 0) continue;
                    if (card.IsCode(CardId.Number60DugaresTheTimeless) && !card.IsDisabled() && !activatedCardIdList.Contains(CardId.Number60DugaresTheTimeless))
                    {
                        continue;
                    }
                    overlayCount += card.Overlays.Count();
                }
                if (overlayCount >= 2)
                {
                    // deattach and search
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            } else
            {
                // attach
                activatedCardIdList.Add(Card.Id);
                return true;
            }

            return false;
        }

        public bool TwinsOfTheEclipseActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                // double attack
                // dump remove material
                if (CheckWhetherNegated(true)) return Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn);
                activatedCardIdList.Add(Card.Id);
                return true;

            } else if (Card.Location == CardLocation.Grave)
            {
                // spsummon
                if (CheckWhetherNegated(true)) return false;

                activatedCardIdList.Add(Card.Id + 1);
                ClientCard rebornTarget = TwinsOfTheEclipseRebornTarget(null);
                if (rebornTarget != null)
                {
                    ClientCard mereo = Bot.Graveyard.FirstOrDefault(c => c.IsCode(CardId.MereologicAggregator));
                    if (mereo != null)
                    {
                        AI.SelectCard(new List<ClientCard> { rebornTarget, mereo });
                        return true;
                    }
                    ClientCard nonLightDark = Bot.Graveyard.FirstOrDefault(c => c.HasType(CardType.Xyz) && !c.HasAttribute((CardAttribute)attrbuteLightDark));
                    if (nonLightDark != null)
                    {
                        AI.SelectCard(new List<ClientCard> { rebornTarget, nonLightDark });
                        return true;
                    }
                    ClientCard xyzMonster = Bot.Graveyard.FirstOrDefault(c => c.HasType(CardType.Xyz));
                    if (xyzMonster != null)
                    {
                        AI.SelectCard(new List<ClientCard> { rebornTarget, xyzMonster });
                        return true;
                    }
                }

                // although cannot find target, still should activate.
                Logger.DebugWriteLine("** Twins of The Eclipse: although cannot find target, still should activate.");
                return true;
            }

            return false;
        }

        public ClientCard TwinsOfTheEclipseRebornTarget(List<ClientCard> targetList)
        {
            if (targetList == null)
            {
                targetList = Bot.Graveyard.Where(c => c.HasType(CardType.Xyz) && c.IsCanRevive()).ToList();
            }
            ClientCard duoDriver = targetList.FirstOrDefault(c => c.IsCode(CardId.RyzealDuodrive));
            ClientCard deadnader = targetList.FirstOrDefault(c => c.IsCode(CardId.RyzealDeadnader));
            ClientCard no41 = targetList.FirstOrDefault(c => c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir));
            ClientCard abyssDweller = targetList.FirstOrDefault(c => c.IsCode(CardId.AbyssDweller));

            if (no41 != null && !DefaultCheckWhetherCardIdIsNegated(_CardId.Number41BagooskatheTerriblyTiredTapir)
                && !(deadnader != null && !activatedCardIdList.Contains(CardId.RyzealDeadnader))
                && !(Duel.Turn == 1 && duoDriver != null))
            {
                return no41;
            }

            if (abyssDweller != null && !DefaultCheckWhetherCardIdIsNegated(CardId.AbyssDweller) && !botSolvedCardIdList.Contains(CardId.AbyssDweller)
                && AbyssDwellerSummonCheck())
            {
                return abyssDweller;
            }

            if (deadnader != null)
            {
                return deadnader;
            }
            if (duoDriver != null && (!activatedCardIdList.Contains(CardId.RyzealDuodrive + 1) || Bot.HasInHandOrInSpellZone(CardId.RyzealCross)))
            {
                return duoDriver;
            }

            // random spsummon
            if (targetList.Count() > 0)
            {
                return ShuffleList(targetList)[0];
            }
            return null;
        }

        public List<ClientCard> CanDestroyList(bool ignoreCurrentDestroy = false)
        {
            List<ClientCard> destroyTargetList = GetNormalEnemyTargetList(true, ignoreCurrentDestroy, CardType.Monster).Except(currentNegateCardList).ToList();

            List<int> cannotDestroyList = new List<int>(NotToDestroySpellTrap);
            destroyTargetList.RemoveAll(c => c.IsCode(cannotDestroyList));

            List<int> undestoryableCardIdlist = new List<int> { 94977269, 58604027, 8062132, 10817524, 53315891, 10000090, 86221741, 71222868,
                83257450, 97489701, 97165977, 24550676, 55410871, 72664875, 85908279, 13331639, 20654247, 43228023, 99585850, 92770064, 10497636, 77313225 };
            destroyTargetList.RemoveAll(c => !c.IsDisabled() && c.IsCode(undestoryableCardIdlist));

            destroyTargetList.RemoveAll(c => !c.IsDisabled() && c.HasSetcode(SetcodeMajespecter));
            
            if (Enemy.GetSpells().Any(c => c.IsFacedown()) || Enemy.GetMonsters().Any(c => c.IsFacedown()))
            {
                destroyTargetList.RemoveAll(c => c.IsCode(81497285));
            }
            destroyTargetList.RemoveAll(c => !c.IsDisabled() && c.HasSetcode(SetcodeMajespecter));


            return destroyTargetList;
        }

        public bool TornadoDragonActivate()
        {
            if (CheckWhetherNegated(true)) return false;

            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;

            // destroy faceup card first
            ClientCard selected = Enemy.SpellZone.GetFloodgate();
            if (selected == null && Duel.Player == 1)
            {
                List<ClientCard> targetList = spells.Where(c => c.IsFaceup() && !NotToDestroySpellTrap.Contains(c.Id) && !currentDestroyCardList.Contains(c)
                    && c.HasType(CardType.Continuous | CardType.Equip | CardType.Field | CardType.Pendulum)).ToList();
                if (targetList.Count() > 0)
                {
                    selected = ShuffleList(targetList)[0];
                }
            }

            if (selected != null)
            {
                currentDestroyCardList.Add(selected);
                AI.SelectCard(selected);
                return true;
            }

            // destroy face-down card
            if (selected == null)
            {
                List<ClientCard> setThisTurnList = Enemy.SpellZone.Where(c => c != null && c.IsFacedown() && !currentDestroyCardList.Contains(c)
                    && enemyPlaceThisTurn.Contains(c)).ToList();
                if (setThisTurnList.Count() > 0)
                {
                    selected = ShuffleList(setThisTurnList)[0];
                }
            }
            if (selected == null)
            {
                List<ClientCard> setThisTurnList = Enemy.SpellZone.Where(c => c != null && c.IsFacedown() && !currentDestroyCardList.Contains(c)).ToList();
                if (setThisTurnList.Count() > 0)
                {
                    selected = ShuffleList(setThisTurnList)[0];
                }
            }

            bool flag = Duel.Player == 0;
            flag |= Duel.Player == 1 && Duel.Phase == DuelPhase.End;
            flag |= DefaultOnBecomeTarget();

            if (flag)
            {
                currentDestroyCardList.Add(selected);
                AI.SelectCard(selected);
                return true;
            }

            return false;
        }

        public bool EvilswarmExcitonKnightActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            return DefaultEvilswarmExcitonKnightEffect();
        }

        public bool AbyssDwellerActivate()
        {
            if (botSolvedCardIdList.Contains(CardId.AbyssDweller)) return false;

            if (Duel.Player == 0 && Bot.HasInHandOrInSpellZone(CardId.RyzealPlugIn))
            {
                List<int> checkIdList = new List<int> { CardId.NodeRyzeal, CardId.ThodeRyzeal, CardId.ExRyzeal };
                foreach (int checkId in checkIdList)
                {
                    if (Card.Overlays.Contains(checkId) && !Bot.HasInHand(checkId) && !activatedCardIdList.Contains(checkId))
                    {
                        return true;
                    }
                    return false;
                }
            }
            if (Duel.Player == 1)
            {
                if (CheckWhetherNegated(true)) return false;
                if (enemyDeckTypeRecord.Contains(SetcodeAtlantean)) return true;
                return Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0;
            }

            return false;
        }

        public bool Number60DugaresTheTimelessActivate()
        {
            if (CheckWhetherNegated(true)) return false;
            if (Number60DugaresTheTimelessDrawEffect() || Number60DugaresTheTimelessDoubleTarget() != null || Number60DugaresTheTimelessRebornEffect())
            {
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool Number60DugaresTheTimelessDrawEffect()
        {
            if (lockBirdSolved || Bot.Deck.Count < 2) return false;
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public ClientCard Number60DugaresTheTimelessDoubleTarget()
        {
            if (Util.IsTurn1OrMain2()) return null;
            ClientCard maxAttackMonster = Bot.MonsterZone.Where(c => c != null && (c.HasPosition(CardPosition.FaceUpAttack) || !summonThisTurn.Contains(c)))
                .OrderByDescending(c => c.Attack).FirstOrDefault();

            if (maxAttackMonster != null)
            {
                int maxBotAttack = maxAttackMonster.Attack;

                // defeat enemy monster
                ClientCard bestEnemyMonster = Enemy.MonsterZone.Where(c => c != null && c.IsFaceup() && (c.IsDisabled() || !c.IsMonsterInvincible()))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (bestEnemyMonster != null)
                {
                    int maxEnemyPower = bestEnemyMonster.GetDefensePower();
                    if (bestEnemyMonster.IsAttack()) maxEnemyPower -= 1;
                    if (maxBotAttack < maxEnemyPower && maxBotAttack * 2 > maxEnemyPower)
                    {
                        return maxAttackMonster;
                    }
                }

                // direct attack
                if (!botSolvedCardIdList.Contains(_CardId.EvilswarmExcitonKnight))
                {
                    int currentAttack = GetBotCurrentTotalAttack();
                    if (currentAttack < Enemy.LifePoints && currentAttack + maxBotAttack >= Enemy.LifePoints)
                    {
                        return maxAttackMonster;
                    }
                }
            }

            return null;
        }

        public bool Number60DugaresTheTimelessRebornEffect()
        {
            // not used

            return false;
        }

        public bool DonnerDaggerFurHireActivate()
        {
            if (CheckAtAdvantage() && !Bot.HasInHand(CardId.ExRyzeal))
            {
                return false;
            }

            ClientCard targetCard = GetProblematicEnemyMonster(canBeTarget: true, selfType: CardType.Monster);
            if (targetCard == null)
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                if (enemyMonsters.Count() > 0)
                {
                    enemyMonsters.Sort(CardContainer.CompareCardAttack);
                    enemyMonsters.Reverse();
                    targetCard = enemyMonsters[0];
                }
            }

            if (targetCard != null)
            {
                AI.SelectCard(Card);
                AI.SelectNextCard(targetCard);
                currentDestroyCardList.Add(targetCard);
                return true;
            }

            return false;
        }



        public bool Level4Summon()
        {
            if (CheckShouldNoMoreSpSummon(CardLocation.Hand | CardLocation.Extra)) return false;
            ClientCard leastAttackLevel4 = Bot.Hand.Where(c => c.Level == 4).OrderBy(c => c.Attack).FirstOrDefault();
            if (leastAttackLevel4 == null || Card != leastAttackLevel4) return false;

            if (GetLevel4CountOnField() == 1)
            {
                ClientCard target = Duel.MainPhase.SummonableCards.Where(c => c != null && c.Level == 4).OrderBy(c => c.Attack).FirstOrDefault();
                if (Card != target) return false;
                summonCount -= 1;
                return true;
            }

            return false;
        }


        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;

            // select place
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                // do not set infinite impermanence if don't need to set other cards
                if (Card.IsCode(_CardId.InfiniteImpermanence) && Bot.GetMonsterCount() == 0 && Bot.GetSpellCount() == 0
                    && !Bot.Hand.Any(c => !c.IsCode(_CardId.InfiniteImpermanence) && (c.IsTrap() || c.HasType(CardType.QuickPlay)))
                    && Bot.Hand.Count() <= 6)
                {
                    return false;
                }

                if (Card.IsCode(CardId.RyzealPlugIn))
                {
                    bool targetFlag = Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && (c.Level == 4 || c.IsCanRevive()));
                    targetFlag |= Bot.Banished.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeRyzeal) && (c.Level == 4 || c.IsCanRevive()));
                    if (!targetFlag)
                    {
                        return false;
                    }
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

        public bool ChangePositionFirst()
        {
            if (Card.IsFacedown() && Card.Level == 4)
            {
                return true;
            }

            if (Enemy.MonsterZone.Any(c => c != null && c.HasPosition(CardPosition.FaceUpDefence) && !c.IsDisabled() && c.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir)))
            {
                return false;
            }

            if (Card.IsCode(_CardId.Number41BagooskatheTerriblyTiredTapir))
            {
                bool haveDangerMonster = Enemy.MonsterZone.Any(c => c != null && c.IsFloodgate() && !c.IsDisabled());
                if (Card.IsDefense())
                {
                    return !haveDangerMonster && !Util.IsTurn1OrMain2();
                } else
                {
                    return haveDangerMonster || Util.IsTurn1OrMain2();
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