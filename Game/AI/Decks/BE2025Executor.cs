using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.Network.Enums;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("BE2025", "AI_BE2025")]
    class BE2025Executor : DefaultExecutor
    {
        public class CardId
        {
            // Main Deck
            public const int BlueEyesWhiteDragon = 89631139;
            public const int DeepEyes = 67886895;
            public const int BlueEyesJetDragon = 30576089;
            public const int MulcharmyFuwalos = 42141493;
            public const int KaibamanTheLegend = 67768675;
            public const int MaidenOfWhite = 17947697;
            public const int SageWithEyesOfBlue = 8240199;
            public const int EffectVeiler = 97268402;
            public const int BlueEyesChaosMAXDragon = 55410871;
            public const int MausoleumOfWhite = 24382602;
            public const int Wishes = 80326401;
            public const int RoarOfTheBlueEyedDragons = 17725109;
            public const int UltimateFusion = 71143015;
            public const int SynchroRumble = 88901994;
            public const int MajestyOfTheWhiteDragons = 43219114;
            public const int TrueLight = 62089826;
            public const int Gogo = 93437091;

            // Extra Deck
            public const int BlueEyesUltimateDragon = 23995346;
            public const int DragonMasterMagia = 12381100;
            public const int NeoBlueEyesUltimateDragon = 56532353;
            public const int StardustSifrDivineDragon = 26268488;
            public const int BlueEyesUltimateSpiritDragon = 89604813;
            public const int CrimsonDragon = 63436931;
            public const int ChaosAngel = 22850702;
            public const int BaronneDeFleur = 84815190;
            public const int BlueEyesSpiritDragon = 59822133;
            public const int SpiritWithEyesOfBlue = 42097666;
            public const int LightstromDragon = 10515412;
            public const int CosmicBlazar = 21123811;

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

        }

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>
        {
            { 3, new List<int>
                {
                    CardId.BlueEyesWhiteDragon,
                    CardId.DeepEyes,
                    CardId.MulcharmyFuwalos,
                    CardId.MaidenOfWhite,
                    CardId.SageWithEyesOfBlue,
                    CardId.Wishes,
                    CardId.Gogo,
                    CardId.BlueEyesSpiritDragon
                }
            },

            { 2, new List<int>
                {
                    _CardId.AshBlossom,
                    CardId.EffectVeiler,
                    _CardId.InfiniteImpermanence,
                    CardId.BlueEyesUltimateSpiritDragon,
                    CardId.SpiritWithEyesOfBlue
                }
            },

            { 1, new List<int>
                {
                    CardId.BlueEyesJetDragon,
                    _CardId.MaxxC,
                    CardId.KaibamanTheLegend,
                    _CardId.LockBird,
                    CardId.BlueEyesChaosMAXDragon,
                    CardId.MausoleumOfWhite,
                    CardId.RoarOfTheBlueEyedDragons,
                    CardId.UltimateFusion,
                    CardId.SynchroRumble,
                    _CardId.CalledByTheGrave,
                    _CardId.CrossoutDesignator,
                    CardId.MajestyOfTheWhiteDragons,
                    CardId.TrueLight,
                    CardId.CosmicBlazar,
                    CardId.BlueEyesUltimateDragon,
                    CardId.DragonMasterMagia,
                    CardId.NeoBlueEyesUltimateDragon,
                    CardId.StardustSifrDivineDragon,
                    CardId.CrimsonDragon,
                    CardId.ChaosAngel,
                    CardId.BaronneDeFleur,
                    CardId.LightstromDragon
                }
            },
        };

        private static readonly int[] PreferDiscard =
        {
            CardId.TrueLight,
            CardId.MulcharmyFuwalos,
            CardId.NibiruThePrimalBeing,
            CardId.BlueEyesChaosMAXDragon,
            CardId.BlueEyesJetDragon,
            CardId.Gogo,
            CardId.BlueEyesWhiteDragon,
            CardId.MaidenOfWhite,
            CardId.DeepEyes,
            _CardId.InfiniteImpermanence,
            CardId.EffectVeiler,
            _CardId.AshBlossom,
            _CardId.CrossoutDesignator,
            _CardId.CalledByTheGrave,
            _CardId.MaxxC
        };

        const int SetcodeMaliss = 0x1bf;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;
        const int SetcodeDarkWorld = 0x6;
        const int SetcodeSkyStriker = 0x115;

        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

        //Flags
        int myTurnCount = 0;
        int normalSummon = 0;
        bool useDeepEyes = false;
        bool useMaidenSearch = false;
        bool useWishes = false;
        bool needBE = false;
        bool nsSage = false;
        bool ActivateSpiritWithEyesOfBlue = false;
        bool SSMaiden = false;
        bool maidenRebornThisTurn = false;


        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();

        public BE2025Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // ===== Generic counters =====
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.StardustSifrDivineDragon, StardustEff);
            AddExecutor(ExecutorType.Activate, CardId.BlueEyesUltimateSpiritDragon, UltimateSpiritEff);
            AddExecutor(ExecutorType.Activate, CardId.NeoBlueEyesUltimateDragon, DontSelfNG);
            AddExecutor(ExecutorType.Activate, CardId.CosmicBlazar, DontSelfNG);
            AddExecutor(ExecutorType.Activate, CardId.DragonMasterMagia, MagiaEff);
            AddExecutor(ExecutorType.Activate, CardId.MajestyOfTheWhiteDragons, MajestyEff);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);

            AddExecutor(ExecutorType.Summon, CardId.MaidenOfWhite, EmergencyMaidenNS);
            AddExecutor(ExecutorType.Activate, CardId.MaidenOfWhite, MaidenGY);
            AddExecutor(ExecutorType.Activate, CardId.DeepEyes, DeepEyes);
            AddExecutor(ExecutorType.Activate, CardId.Wishes, WishesEff);
            AddExecutor(ExecutorType.SpSummon, CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.MausoleumOfWhite, FieldSpellFromHandOnly);
            AddExecutor(ExecutorType.Activate, CardId.MausoleumOfWhite, MausoleumOfWhiteEff);
            AddExecutor(ExecutorType.Activate, CardId.MaidenOfWhite, MaidenSearch);
            AddExecutor(ExecutorType.Summon, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SpiritWithEyesOfBlue, SpSpiritWithEyesOfBlue);
            AddExecutor(ExecutorType.Summon, CardId.KaibamanTheLegend, KaibamanTheLegendSummon);
            AddExecutor(ExecutorType.Activate, CardId.KaibamanTheLegend, KaibamanTheLegend);
            AddExecutor(ExecutorType.Activate, CardId.SpiritWithEyesOfBlue, SpiritWithEyesOfBluEffect);
            AddExecutor(ExecutorType.Activate, CardId.TrueLight, TrueLightEff);
            AddExecutor(ExecutorType.Activate, CardId.UltimateFusion, UltimateFusionEff);

            AddExecutor(ExecutorType.Activate, CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.CrimsonDragon, CrimsonDragonEffect);

            AddExecutor(ExecutorType.Activate, CardId.RoarOfTheBlueEyedDragons, RoarOfTheBlueEyedDragonsEff);
            AddExecutor(ExecutorType.Activate, CardId.SynchroRumble, SynchroRumbleEff);

            AddExecutor(ExecutorType.Activate, CardId.MaidenOfWhite, MaidenRebornEff);

            AddExecutor(ExecutorType.Activate, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.BlueEyesJetDragon, JetEff);

            // if we don't have other things to do...
            AddExecutor(ExecutorType.Repos, Repos);
            AddExecutor(ExecutorType.Activate, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffectInHand);
            AddExecutor(ExecutorType.Summon, CardId.EffectVeiler, Nscount); //no option
            AddExecutor(ExecutorType.Activate, CardId.Gogo, GogoEff); //no option
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
        }
        #region Default
        public override void OnNewTurn()
        {
            if (Duel.Player == 0)
            {
                myTurnCount++;
            }
            // reset
            normalSummon = 0;

            useDeepEyes = false;
            useMaidenSearch = false;
            useWishes = false;
            needBE = false;
            nsSage = false;
            ActivateSpiritWithEyesOfBlue = false;
            SSMaiden = false;
            maidenRebornThisTurn = false;

            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyPlaceThisTurn.Clear();



            base.OnNewTurn();
        }
        public override bool OnSelectHand() { return true; /* Go first by default.*/}
        public override int OnSelectOption(IList<int> options)
        {
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            Logger.DebugWriteLine($"OnSelectOption: CurrentSolving={currentSolvingChain} count={options.Count} options=[{string.Join(", ", options.Select((v, i) => $"{i}:{v}"))}]");
            /*if (currentSolvingChain != null)
            {
                Logger.DebugWriteLine("Custom select Option");
                // 1190=Add to Hand, 1152=Special Summon, 1153=Set
                if (currentSolvingChain.IsCode(CardId.TrueLight) && 
                    options.Count == 2)
                {
                    Logger.DebugWriteLine("OnSelectOption True Light");
                    bool maidenInGY = Bot.Graveyard.Any(c => c.Id == CardId.MaidenOfWhite);
                    bool bewdInHandOrGY = Bot.HasInHand(new[] { CardId.BlueEyesWhiteDragon }) || Bot.Graveyard.Any(c => c.Id == CardId.BlueEyesWhiteDragon);

                    if (!useWishes && Duel.Player == 0)
                    {
                        return 2;
                    }
                    if (useWishes && needBE && Duel.Player == 0)
                    {
                        return 2;
                    }
                    else if (Bot.HasInGraveyard(CardId.BlueEyesWhiteDragon))
                    {
                        return 2;
                    }
                    else if (needBE && Bot.HasInHandOrInGraveyard(CardId.BlueEyesWhiteDragon))
                    {
                        return 1;
                    }
                }
            }*/
            Logger.DebugWriteLine("OnSelectOption Default");
            return base.OnSelectOption(options);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);

            Logger.DebugWriteLine("Use default.");

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        public override void OnSpSummoned()
        {
            // not special summoned by chain
            if (Duel.GetCurrentSolvingChainCard() == null)
            {
                /*foreach (ClientCard card in Duel.LastSummonedCards)
                {
                    if (card.Controller == 0 && card.IsCode(CardId.AlternativeWhiteDragon))
                    {
                        AlternativeWhiteDragonSummoned = true;????
                    }
                }*/
            }
            base.OnSpSummoned();
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
            if (DefaultCheckWhetherCardIsNegated(lastcard)) return false;
            if (Duel.Turn == 1 && lastcard.IsCode(_CardId.MaxxC)) return false;

            return true;
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
        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0)
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
        public List<ClientCard> ShuffleCardList(List<ClientCard> list)
        {
            List<ClientCard> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                ClientCard temp = result[index];
                result[index] = result[n];
                result[n] = temp;
            }
            return result;
        }
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
        private bool Repos()
        {
            bool enemyBetter = Util.IsAllEnemyBetter(true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
                return true;
            if (Card.IsDefense() && Card.IsCode(CardId.BlueEyesSpiritDragon, CardId.StardustSifrDivineDragon, CardId.BlueEyesUltimateSpiritDragon, CardId.CosmicBlazar))
                return true;
            if (Card.IsAttack() && Card.IsCode(CardId.SageWithEyesOfBlue, CardId.KaibamanTheLegend, CardId.MaidenOfWhite))
                return true;
            return false;
        }
        private bool DontSelfNG() { return Duel.LastChainPlayer != 0; }
        #endregion

        #region WorkSpace
        private bool MaidenSearch()
        {
            if (ActivateDescription != Util.GetStringId(CardId.MaidenOfWhite, 0))
            {
                return false;
            }
            if (Card.Location != CardLocation.Hand && !Bot.HasInSpellZone(CardId.TrueLight) && !Bot.HasInBanished(CardId.TrueLight))
            {
                SelectSTPlace(null, true);
                useMaidenSearch = true;
                return true;
            }
            else if (Card.Location != CardLocation.MonsterZone && !Bot.HasInSpellZone(CardId.TrueLight) && !Bot.HasInBanished(CardId.TrueLight))
            {
                SelectSTPlace(null, true);
                useMaidenSearch = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool MaidenGY()
        {
            if (Card.Location == CardLocation.Grave)
            {
                SSMaiden = true;
                return true;
            }
            else return false;
        }
        private bool DeepEyes()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (useDeepEyes) return false;

            if (!Bot.HasInHand(CardId.MaidenOfWhite) && !useMaidenSearch && CheckRemainInDeck(CardId.MaidenOfWhite) > 0)
            {
                AI.SelectCard(CardId.MaidenOfWhite);
                useDeepEyes = true;
                return true;
            }
            else if (!Bot.HasInHand(CardId.SageWithEyesOfBlue) && !nsSage && CheckRemainInDeck(CardId.SageWithEyesOfBlue) > 0)
            {
                AI.SelectCard(CardId.SageWithEyesOfBlue);
                useDeepEyes = true;
                return true;
            }
            else if (!Bot.HasInHand(CardId.KaibamanTheLegend) && normalSummon < 2 && nsSage && CheckRemainInDeck(CardId.KaibamanTheLegend) > 0)
            {
                AI.SelectCard(CardId.KaibamanTheLegend);
                useDeepEyes = true;
                return true;
            }
            else if (CheckRemainInDeck(CardId.EffectVeiler) > 0)
            {
                AI.SelectCard(CardId.EffectVeiler);
                useDeepEyes = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool MaidenRebornEff()
        {
            if (ActivateDescription != Util.GetStringId(CardId.MaidenOfWhite, 2))
                return false;

            if (Card.Location != CardLocation.MonsterZone) return false;
            if (Bot.GetMonsterCount() >= 5) return false;

            var gy = Bot.Graveyard.ToList();

            var be = gy.FirstOrDefault(c => c.IsCode(CardId.BlueEyesWhiteDragon));

            var tuner = gy.FirstOrDefault(c =>
                c.Level == 1 && c.IsTuner() && c.HasAttribute(CardAttribute.Light)
            );

            if (be == null && tuner == null) return false;

            if (!Bot.HasInMonstersZone(CardId.BlueEyesWhiteDragon))
                AI.SelectCard(be ?? tuner);
            else
                AI.SelectCard(tuner ?? be);
            maidenRebornThisTurn = true;
            return true;
        }
        private bool SageWithEyesOfBlueSummon()
        {
            if (Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue) || Bot.HasInMonstersZone(CardId.SageWithEyesOfBlue))
            {
                return false;
            }
            if ((Duel.Turn == 1 || Duel.Turn == 2) && Duel.Player == 0)
            {
                normalSummon += 1;
                nsSage = true;
                return true;
            }
            else if (Duel.Turn > 2 && Duel.Player == 0 && (CheckRemainInDeck(CardId.KaibamanTheLegend) > 0 || CheckRemainInDeck(CardId.EffectVeiler) > 0))
            {
                normalSummon += 1;
                nsSage = true;
                return true;
            }
            else
            { return false; }
        }
        private bool KaibamanTheLegendSummon()
        {
            /*if (Bot.HasInSpellZone(CardId.MausoleumOfWhite) && normalSummon < 2)
            {
                normalSummon += 1;
                return true;
            }
            return false;*/
            normalSummon += 1;
            return true;
        }
        private bool KaibamanTheLegend()
        {
            if (Card.Location == CardLocation.MonsterZone) { return true; }
            else if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInSpellZone(CardId.TrueLight) && !Bot.HasInGraveyard(CardId.MaidenOfWhite) && useDeepEyes == false && CheckRemainInDeck(CardId.DeepEyes) > 0)
                {
                    AI.SelectCard(CardId.DeepEyes);
                    return true;
                }
                else if (!Bot.HasInHand(CardId.BlueEyesChaosMAXDragon) && !Bot.HasInGraveyard(CardId.BlueEyesChaosMAXDragon) &&
                         CheckRemainInDeck(CardId.BlueEyesChaosMAXDragon) > 0 && !Bot.HasInMonstersZone(CardId.DragonMasterMagia) &&
                         Bot.HasInGraveyard(CardId.BlueEyesUltimateDragon))
                {
                    AI.SelectCard(CardId.BlueEyesChaosMAXDragon);
                    return true;
                }
                if (useDeepEyes == false && CheckRemainInDeck(CardId.DeepEyes) > 0)
                {
                    AI.SelectCard(CardId.DeepEyes);
                    return true;
                }
                else if (!Bot.HasInHand(CardId.BlueEyesJetDragon) && !Bot.HasInGraveyard(CardId.BlueEyesJetDragon) && CheckRemainInDeck(CardId.BlueEyesJetDragon) > 0)
                {
                    AI.SelectCard(CardId.BlueEyesJetDragon);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private bool SageWithEyesOfBlueEffect()
        {
            if (Card.Location == CardLocation.Hand) return false;
            if (!useMaidenSearch && CheckRemainInDeck(CardId.MaidenOfWhite) > 0)
            {
                Logger.DebugWriteLine("Sage > Maiden");
                AI.SelectCard(CardId.MaidenOfWhite);
            }
            else if (normalSummon < 2 && CheckRemainInDeck(CardId.KaibamanTheLegend) > 0)
            {
                Logger.DebugWriteLine("Sage > Kaiba");
                AI.SelectCard(CardId.KaibamanTheLegend);
            }
            else if (CheckRemainInDeck(CardId.EffectVeiler) > 0)
            {
                Logger.DebugWriteLine("Sage > Veiler");
                AI.SelectCard(CardId.EffectVeiler);
            }
            else
            {
                Logger.DebugWriteLine("Sage x");
                return false;
            }
            return true;
        }
        private bool SageWithEyesOfBlueEffectInHand()
        {
            if (Card.Location != CardLocation.Hand)
            {
                return false;
            }
            if (!Bot.HasInMonstersZone(new[]
                {
                    CardId.MaidenOfWhite,
                    CardId.SpiritWithEyesOfBlue,
                    CardId.DeepEyes
                }))
            {
                return false;
            }
            AI.SelectCard(CardId.MaidenOfWhite, CardId.SpiritWithEyesOfBlue, CardId.DeepEyes);
            if (Bot.HasInGraveyard(CardId.BlueEyesWhiteDragon))
            {
                AI.SelectNextCard(CardId.BlueEyesJetDragon);
            }
            else
            {
                AI.SelectNextCard(CardId.BlueEyesWhiteDragon);
            }
            return true;
        }
        private bool BlueEyesSpiritDragonEffect()
        {
            if (!DontSelfNG()) return false;
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            if ((ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.BlueEyesSpiritDragon, 0)) &&
                ((Duel.LastChainPlayer == 1) || (Duel.LastChainPlayer == 0 && currentCard != null && currentCard.IsCode(CardId.TrueLight))))
            {
                Logger.DebugWriteLine("Spirit 1 negate");
                return true;
            }
            else if (Duel.Player == 1 && Util.IsChainTarget(Card))
            {
                Logger.DebugWriteLine("Spirit 2");
                AI.SelectCard(CardId.BlueEyesUltimateSpiritDragon);
                return true;
            }
            else if (Duel.Player == 1 && Bot.HasInMonstersZone(CardId.BlueEyesUltimateSpiritDragon) &&
                     Bot.HasInMonstersZone(CardId.StardustSifrDivineDragon) &&
                     (Duel.Phase == DuelPhase.Standby || Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.End))

            {
                Logger.DebugWriteLine("Spirit 3");
                AI.SelectCard(CardId.CrimsonDragon);
                return true;
            }
            else if (Duel.Player == 0 && !Bot.HasInMonstersZone(CardId.BlueEyesUltimateSpiritDragon) && Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue))
            {
                Logger.DebugWriteLine("Spirit 4");
                AI.SelectCard(CardId.BlueEyesUltimateSpiritDragon);
                return true;
            }
            else if (Duel.Player == 0 && !Bot.HasInMonstersZone(CardId.BlueEyesUltimateSpiritDragon) &&
                     (Bot.HasInGraveyard(CardId.MaidenOfWhite) || Bot.HasInGraveyard(CardId.SageWithEyesOfBlue) || Bot.HasInGraveyard(CardId.EffectVeiler)))
            {
                Logger.DebugWriteLine("Spirit 5");
                AI.SelectCard(CardId.CrimsonDragon);
                return true;
            }
            else if (Duel.Player == 0 && Bot.HasInMonstersZone(CardId.BlueEyesUltimateSpiritDragon) && !Bot.HasInMonstersZone(CardId.StardustSifrDivineDragon))
            {
                Logger.DebugWriteLine("Spirit 6");
                AI.SelectCard(CardId.CrimsonDragon);
                return true;
            }
            else
            {
                if (Util.IsChainTarget(Card))
                {
                    Logger.DebugWriteLine("Spirit 7");
                    AI.SelectCard(CardId.BlueEyesUltimateSpiritDragon);
                    return true;
                }
                return false;
            }
        }
        private bool CrimsonDragonEffect()
        {
            ClientCard target = Bot.MonsterZone.Concat(Enemy.MonsterZone).FirstOrDefault(c => c != null && c.HasType(CardType.Synchro) && c.Level == 12);

            if (ActivateDescription == -1)
            {
                return true;
            }
            else if (target != null && !Bot.HasInMonstersZone(CardId.StardustSifrDivineDragon))
            {
                AI.SelectCard(target);
                AI.SelectNextCard(CardId.StardustSifrDivineDragon);
                return true;
            }
            else if (target != null && !Bot.HasInMonstersZone(CardId.CosmicBlazar))
            {
                AI.SelectCard(target);
                AI.SelectNextCard(CardId.CosmicBlazar);
                return true;
            }
            else if (target != null && !Bot.HasInMonstersZone(CardId.BlueEyesUltimateSpiritDragon))
            {
                AI.SelectCard(target);
                AI.SelectNextCard(CardId.BlueEyesUltimateSpiritDragon);
                return true;
            }
            else if (target == null)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        private bool BlueEyesSpiritDragonSummon()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                if (Duel.Turn == 1)
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    needBE = false;
                    return true;
                }
                else
                {
                    needBE = false;
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                needBE = false;
                return true;
            }
            if (useWishes && (
                Bot.HasInMonstersZone(CardId.EffectVeiler) ||
                Bot.HasInMonstersZone(CardId.SageWithEyesOfBlue) ||
                //(Bot.HasInMonstersZone(CardId.KaibamanTheLegend) && ) ||
                Bot.HasInMonstersZoneOrInGraveyard(CardId.MaidenOfWhite)

                ))
            {
                needBE = true;
            }
            return false;
        }
        private bool FieldSpellFromHandOnly()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            return true;
        }
        private bool MausoleumOfWhiteEff()
        {
            if (Bot.HasInMonstersZone(CardId.MaidenOfWhite) && Bot.HasInGraveyard(CardId.BlueEyesWhiteDragon))
            {
                AI.SelectCard(CardId.MaidenOfWhite);
                return true;
            }
            else if (Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue) && Bot.HasInGraveyard(CardId.MaidenOfWhite) &&
                !Bot.HasInHandOrInGraveyard(CardId.BlueEyesWhiteDragon) && !Bot.HasInHand(CardId.KaibamanTheLegend))
            {
                AI.SelectCard(CardId.SpiritWithEyesOfBlue);
                return true;
            }
            return false;
        }
        private bool TrueLightEff()
        {
            if (!DontSelfNG()) return false;
            bool maidenInGY = Bot.Graveyard.Any(c => c.Id == CardId.MaidenOfWhite);
            bool bewdInHandOrGY = Bot.HasInHand(new[] { CardId.BlueEyesWhiteDragon }) || Bot.Graveyard.Any(c => c.Id == CardId.BlueEyesWhiteDragon);

            if (!Bot.HasInHand(CardId.Wishes) && !useWishes && Duel.Player == 0 && CheckRemainInDeck(CardId.Wishes) > 0)
            {
                Logger.DebugWriteLine("TrueLight Search Wishes");
                AI.SelectOption(1);
                SelectSTPlace(null, true);
                AI.SelectCard(CardId.Wishes);
                return true;
            }
            if (useWishes && needBE && Duel.Player == 0 && CheckRemainInDeck(CardId.RoarOfTheBlueEyedDragons) > 0)
            {
                Logger.DebugWriteLine("TrueLight Search Roar");
                AI.SelectOption(1);
                SelectSTPlace(null, true);
                AI.SelectCard(CardId.RoarOfTheBlueEyedDragons);
                return true;
            }
            else if (Bot.HasInHandOrInGraveyard(CardId.BlueEyesWhiteDragon) && Bot.HasInMonstersZoneOrInGraveyard(CardId.MaidenOfWhite) &&
                Bot.HasInHand(CardId.SynchroRumble) && !Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue))
            {
                Logger.DebugWriteLine("TrueLight SSBE with rumble");
                AI.SelectOption(0);
                return true;
            }
            else if (Bot.HasInGraveyard(CardId.BlueEyesWhiteDragon) && CheckRemainInDeck(CardId.MajestyOfTheWhiteDragons) > 0 &&
                     Bot.GetMonsterCount() >= 2 && Bot.HasInGraveyard(CardId.SpiritWithEyesOfBlue))
            {
                Logger.DebugWriteLine("TrueLight Search Majesty");
                AI.SelectOption(1);
                SelectSTPlace(null, true);
                AI.SelectCard(CardId.MajestyOfTheWhiteDragons);
                return true;
            }
            else if (needBE && Bot.HasInHandOrInGraveyard(CardId.BlueEyesWhiteDragon) && !Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue))
            {
                Logger.DebugWriteLine("TrueLight SSBE");
                AI.SelectOption(0);
                return true;
            }
            else if (Bot.HasInHandOrInGraveyard(CardId.BlueEyesWhiteDragon) && !Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue) &&
                     Bot.HasInHandOrHasInMonstersZone(CardId.MaidenOfWhite) && !Bot.HasInGraveyard(CardId.MaidenOfWhite) &&
                     !Bot.HasInMonstersZone(CardId.BlueEyesWhiteDragon))
            {
                Logger.DebugWriteLine("TrueLight Emergency SSBE");
                AI.SelectOption(0);
                return true;
            }
            Logger.DebugWriteLine("TrueLight Do Nothing");
            return false;
        }
        private bool WishesEff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Wishes, 0))
            {
                var discard = PreferDiscard.Select(id => Bot.Hand.FirstOrDefault(c => c != null && c.Id == id)).FirstOrDefault(c => c != null);
                if (discard != null)
                {
                    AI.SelectCard(discard); //1 Discard
                }
                //if not then use system default
                int st = 0;
                if ((CheckRemainInDeck(CardId.UltimateFusion) > 0) && Bot.HasInSpellZone(CardId.TrueLight)) st = CardId.UltimateFusion;
                else if (CheckRemainInDeck(CardId.RoarOfTheBlueEyedDragons) > 0) st = CardId.RoarOfTheBlueEyedDragons;
                else if (CheckRemainInDeck(CardId.MajestyOfTheWhiteDragons) > 0) st = CardId.MajestyOfTheWhiteDragons;
                if (st == 0) return false;
                AI.SelectNextCard(st); //2 Select S/T
                int tuner = 0;
                if (CheckRemainInDeck(CardId.SageWithEyesOfBlue) > 0 && !nsSage && !Bot.HasInHand(CardId.SageWithEyesOfBlue)) tuner = CardId.SageWithEyesOfBlue;
                else if (CheckRemainInDeck(CardId.MaidenOfWhite) > 0 && !useMaidenSearch && !Bot.HasInHand(CardId.MaidenOfWhite)) tuner = CardId.MaidenOfWhite;
                else if (CheckRemainInDeck(CardId.KaibamanTheLegend) > 0 && normalSummon < 2) tuner = CardId.KaibamanTheLegend;
                else if (CheckRemainInDeck(CardId.EffectVeiler) > 0) tuner = CardId.EffectVeiler;
                if (tuner == 0) return false;
                AI.SelectNextCard(tuner);//3 Select Tuner
                SelectSTPlace(null, true);
                useWishes = true;
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.Wishes, 1))
            {
                Logger.DebugWriteLine("Wishes GY");
                if (Card.Location != CardLocation.Grave) return false;
                ClientCard be = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.BlueEyesWhiteDragon));
                if (be == null) return false;
                if (Bot.GetSpellCount() >= 5) return false;
                if (Bot.HasInHandOrInSpellZone(CardId.UltimateFusion) && Bot.HasInExtra(CardId.BlueEyesUltimateDragon) && Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.KaibamanTheLegend))
                {
                    Logger.DebugWriteLine("Magia");
                    AI.SelectCard(CardId.BlueEyesWhiteDragon);
                    AI.SelectNextCard(CardId.BlueEyesUltimateDragon);
                }
                else if (!Bot.HasInHandOrInSpellZone(CardId.UltimateFusion) && Bot.HasInExtra(CardId.NeoBlueEyesUltimateDragon))
                {
                    Logger.DebugWriteLine("No Magia");
                    AI.SelectCard(CardId.BlueEyesWhiteDragon);
                    AI.SelectNextCard(CardId.NeoBlueEyesUltimateDragon);
                }
                else
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        private bool SpSpiritWithEyesOfBlue()
        {
            if (Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue) || ActivateSpiritWithEyesOfBlue == true)
            {
                return false;
            }
            return true;
        }
        private bool SpiritWithEyesOfBluEffect()
        {
            if (ActivateDescription == -1)
            {
                Logger.DebugWriteLine("Link1 on Summon");
                if (CheckRemainInDeck(CardId.MausoleumOfWhite) <= 0)
                    return false;
                AI.SelectOption(0);
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.SpiritWithEyesOfBlue, 2))
            {
                ActivateSpiritWithEyesOfBlue = true;
                Logger.DebugWriteLine("Link1 Active");
                if (Bot.GetMonsterCount() >= 5) return false;
                ClientCard pick = null;
                if (Bot.HasInGraveyard(CardId.BlueEyesWhiteDragon))
                {
                    var c = Bot.Graveyard.FirstOrDefault(x => x != null && x.IsCode(CardId.BlueEyesWhiteDragon));
                    if (c != null) pick = c;
                }
                else if (Bot.HasInHand(CardId.BlueEyesWhiteDragon) && Bot.HasInGraveyard(CardId.KaibamanTheLegend))
                {
                    var c = Bot.Hand.FirstOrDefault(x => x != null && x.IsCode(CardId.BlueEyesWhiteDragon));
                    if (c != null) pick = c;
                }
                if (pick == null) return false;
                AI.SelectCard(pick);
                return true;
            }
            return false;
        }
        private bool UltimateFusionEff()
        {
            if (!DontSelfNG()) return false;
            if (Bot.GetMonsterCount() >= 5) return false;
            int targetFusionId = CardId.DragonMasterMagia;
            if (!Bot.HasInExtra(targetFusionId)) return false;
            if (Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.BlueEyesChaosMAXDragon) && Bot.HasInMonstersZoneOrInGraveyard(CardId.BlueEyesUltimateDragon))
            {
                AI.SelectCard(targetFusionId);
                AI.SelectNextCard(CardId.BlueEyesChaosMAXDragon);
                AI.SelectNextCard(CardId.BlueEyesUltimateDragon);
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }
        private bool RoarOfTheBlueEyedDragonsEff()
        {
            if (Bot.GetMonsterCount() >= 5) return false;
            if (Card.Location == CardLocation.Grave) return false;
            if (Bot.HasInGraveyard(CardId.MaidenOfWhite) && SSMaiden == false &&
                CheckRemainInDeck(CardId.BlueEyesWhiteDragon) > 0 && !Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue))
            {
                AI.SelectCard(CardId.BlueEyesWhiteDragon);
                SelectSTPlace(null, true);
                return true;
            }
            return false;
        }
        private bool SynchroRumbleEff()
        {
            if (Bot.HasInGraveyard(CardId.MaidenOfWhite) && Bot.HasInMonstersZoneOrInGraveyard(CardId.BlueEyesWhiteDragon)
                && maidenRebornThisTurn == false && !Bot.HasInMonstersZone(CardId.SpiritWithEyesOfBlue))
            {
                AI.SelectCard(CardId.MaidenOfWhite);
                SelectSTPlace(null, true);
                return true;
            }
            return false;
        }
        public bool StardustEff()
        {
            if (CheckWhetherNegated(true, true, 0)) return false;
            ClientCard target = GetBestEnemyCard(onlyFaceup: true, canBeTarget: true, checkGrave: false);
            if (target == null) return false;
            AI.SelectCard(target);
            return true;
        }
        public bool Nscount()
        {
            if (Bot.GetMonsterCount() == 0 && Bot.GetMonstersExtraZoneCount() == 0)
            {
                normalSummon += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void AddGogoPick(List<int> picks, int cardId, int want = 1)
        {
            int remain = CheckRemainInDeck(cardId);
            int already = picks.Count(x => x == cardId);
            int canAdd = Math.Max(0, Math.Min(want, remain - already));

            for (int i = 0; i < canAdd; ++i)
                picks.Add(cardId);
        }
        public bool GogoEff()
        {
            SelectSTPlace(null, true);
            List<int> picks = new List<int>();
            bool noStarterInHand = !Bot.HasInHand(CardId.SageWithEyesOfBlue) && !Bot.HasInHand(CardId.MaidenOfWhite);

            if (!useDeepEyes && !Bot.HasInHand(CardId.DeepEyes) && noStarterInHand)
            {
                AddGogoPick(picks, CardId.DeepEyes, 3);
            }
            else if (!useWishes && !Bot.HasInHand(CardId.Wishes) && noStarterInHand)
            {
                AddGogoPick(picks, CardId.Wishes, 3);
            }
            else
            {
                if (!Bot.HasInHand(CardId.MajestyOfTheWhiteDragons))
                    AddGogoPick(picks, CardId.MajestyOfTheWhiteDragons);
                if (!Bot.HasInHand(CardId.RoarOfTheBlueEyedDragons))
                    AddGogoPick(picks, CardId.RoarOfTheBlueEyedDragons);
                if (!Bot.HasInHand(CardId.UltimateFusion))
                    AddGogoPick(picks, CardId.UltimateFusion);
                if (picks.Count < 3 &&
                    !useDeepEyes &&
                    !Bot.HasInHand(CardId.DeepEyes) &&
                    !Bot.HasInHand(CardId.EffectVeiler))
                {
                    AddGogoPick(picks, CardId.DeepEyes, 3 - picks.Count);
                }
            }
            if (picks.Count < 3) AddGogoPick(picks, CardId.Wishes, 3 - picks.Count);
            if (picks.Count < 3) AddGogoPick(picks, CardId.MajestyOfTheWhiteDragons, 3 - picks.Count);
            if (picks.Count < 3) AddGogoPick(picks, CardId.RoarOfTheBlueEyedDragons, 3 - picks.Count);
            if (picks.Count < 3) AddGogoPick(picks, CardId.UltimateFusion, 3 - picks.Count);
            if (picks.Count < 3) AddGogoPick(picks, CardId.DeepEyes, 3 - picks.Count);

            if (picks.Count >= 3)
                AI.SelectCard(picks.Take(3).ToArray());
            return true;
        }
        public bool MajestyEff()
        {
            if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer == 0) return false;
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            List<ClientCard> revealCards = new List<ClientCard>();
            revealCards.AddRange(Bot.Graveyard.Where(c => c != null && c.IsCode(CardId.BlueEyesWhiteDragon)));
            revealCards.AddRange(Bot.GetMonsters().Where(c => c != null && c.IsFaceup() && c.IsCode(CardId.BlueEyesWhiteDragon)));
            revealCards.AddRange(Bot.Hand.Where(c => c != null && c.IsCode(CardId.BlueEyesWhiteDragon)));

            int ct = Math.Min(3, Math.Min(revealCards.Count, Enemy.GetFieldCount()));
            if (ct == 0) return false;

            AI.SelectCard(revealCards.Take(ct).ToArray());

            List<ClientCard> targets = GetNormalEnemyTargetList(canBeTarget: false, ignoreCurrentDestroy: false)
                .Where(c => c != null)
                .Distinct()
                .Take(ct)
                .ToList();

            if (targets.Count < ct) return false;

            AI.SelectNextCard(targets);
            return true;
        }
        private bool MagiaEff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.DragonMasterMagia, 0))
            {
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.DragonMasterMagia, 1))
            {
                ClientCard target = GetUltimateDragonMagiaFloatTarget();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }
            return false;

        }
        private ClientCard GetUltimateDragonMagiaFloatTarget()
        {
            List<ClientCard> candidates = new List<ClientCard>();

            foreach (ClientCard card in Bot.Graveyard)
            {
                if (card != null && UltimateDragonMagiaFloatPriority.Contains(card.Id))
                    candidates.Add(card);
            }

            foreach (ClientCard card in Bot.ExtraDeck)
            {
                if (card != null && UltimateDragonMagiaFloatPriority.Contains(card.Id))
                    candidates.Add(card);
            }

            if (candidates.Count == 0)
                return null;

            foreach (int id in UltimateDragonMagiaFloatPriority)
            {
                ClientCard pick = candidates.FirstOrDefault(c => c.Id == id);
                if (pick != null)
                    return pick;
            }

            return candidates.FirstOrDefault();
        }
        private readonly int[] UltimateDragonMagiaFloatPriority =
        {
            CardId.BlueEyesUltimateSpiritDragon,
            CardId.BlueEyesWhiteDragon,
            CardId.BlueEyesChaosMAXDragon,
            CardId.BlueEyesJetDragon
        };
        private bool UltimateSpiritEff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BlueEyesUltimateSpiritDragon, 1))
            {
                if (CheckWhetherNegated()) return false;
                if (Duel.LastChainPlayer != 1) return false;
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.BlueEyesUltimateSpiritDragon, 2))
            {
                return true;
            }
            return false;
        }
        private bool JetEff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BlueEyesJetDragon, 1))
            {
                ClientCard target = GetBestEnemyCard(false, true, false);
                if (target == null) return false;
                AI.SelectCard(target);
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool EmergencyMaidenNS()
        {
            Logger.DebugWriteLine("CheckAtAdvantage: " + CheckAtAdvantage().ToString());
            if (myTurnCount >= 2 && CheckAtAdvantage() == false &&
                (Bot.HasInSpellZone(CardId.TrueLight) || Bot.HasInBanished(CardId.TrueLight)))
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}
