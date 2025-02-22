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
    [Deck("Albaz", "AI_Albaz")]
    public class AlbazExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int TheBystialLubellion = 32731036;
            public const int AlbionTheShroudedDragon = 25451383;
            public const int BystialSaronir = 60242223;
            public const int AluberTheJesterOfDespia = 62962630;
            public const int FallenOfAlbaz = 68468459;
            public const int SpringansKitt = 45484331;
            public const int GuidingQuemTheVirtuous = 45883110;
            public const int BlazingCartesiaTheVirtuous = 95515789;
            public const int TriBrigadeMercourier = 19096726;
            // _CardId.AshBlossom = 14558127;
            // _CardId.MaxxC = 23434538;
            public const int DespianTragedy = 36577931;

            public const int NadirServant = 1984618;
            public const int FusionDeployment = 6498706;
            public const int BrandedInWhite = 34995106;
            public const int BrandedFusion = 44362883;
            public const int GoldSarcophagus = 75500286;
            public const int FoolishBurial = 81439173;
            // _CardId.CalledByTheGrave = 24224830;
            public const int BrandedInHighSpirits = 29948294;
            public const int BrandedOpening = 36637374;
            // _CardId.CrossoutDesignator = 65681983;
            public const int BrandedInRed = 82738008;
            public const int BrandedLost = 18973184;

            // _CardId.InfiniteImpermanence = 10045474;
            public const int BrightestBlazingBrandedKing = 19271881;
            public const int BrandedBeast = 32756828;
            public const int BrandedRetribution = 17751597;

            public const int GuardianChimera = 11321089;
            public const int AlbionTheSanctifireDragon = 38811586;
            public const int MirrorjadeTheIcebladeDragon = 44146295;
            public const int BorreloadFuriousDragon = 92892239;
            public const int LubellionTheSearingDragon = 70534340;
            public const int AlbaLenatusTheAbyssDragon = 3410461;
            public const int GranguignolTheDuskDragon = 24915933;
            public const int DespianQuaeritis = 72272462;
            public const int SprindTheIrondashDragon = 1906812;
            public const int TitanikladTheAshDragon = 41373230;
            public const int RindbrummTheStrikingDragon = 51409648;
            public const int AlbionTheBrandedDragon = 87746184;
            public const int DespianLuluwalilith = 53971455;

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
            public const int NibiruThePrimalBeing = 27204311;
        }

        public AlbazExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);

            // 
            AddExecutor(ExecutorType.Summon, AdvanceSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AlbaLenatusTheAbyssDragon, AlbaLenatusTheAbyssDragonSpSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.FallenOfAlbaz, FallenOfAlbazSet);
            AddExecutor(ExecutorType.Activate, CardId.BlazingCartesiaTheVirtuous, BlazingCartesiaTheVirtuousActivateInGrave);

            // quick effect
            AddExecutor(ExecutorType.Activate, CardId.BrandedRetribution, BrandedRetributionActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedBeast, BrandedBeastActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrightestBlazingBrandedKing, BrightestBlazingBrandedKingActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedOpening, BrandedOpeningActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInHighSpirits, BrandedInHighSpiritsActivate);
            AddExecutor(ExecutorType.Activate, CardId.RindbrummTheStrikingDragon, RindbrummTheStrikingDragonActivate);

            // remove
            AddExecutor(ExecutorType.Activate, CardId.GuardianChimera, GuardianChimeraActivate);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadFuriousDragon, BorreloadFuriousDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeMercourier, TriBrigadeMercourierActivate);
            AddExecutor(ExecutorType.Activate, CardId.DespianQuaeritis, DespianQuaeritisActivate);
            AddExecutor(ExecutorType.Activate, CardId.MirrorjadeTheIcebladeDragon, MirrorjadeTheIcebladeDragonActivate);

            // search
            AddExecutor(ExecutorType.Activate, CardId.TheBystialLubellion, TheBystialLubellionActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.TheBystialLubellion, TheBystialLubellionSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialActivate);
            AddExecutor(ExecutorType.Activate, CardId.GoldSarcophagus, GoldSarcophagusActivate);
            AddExecutor(ExecutorType.Activate, CardId.AluberTheJesterOfDespia, AluberTheJesterOfDespiaActivate);

            AddExecutor(ExecutorType.Activate, CardId.AlbionTheSanctifireDragon, AlbionTheSanctifireDragonActivate);

            // blazing
            AddExecutor(ExecutorType.Activate, CardId.FusionDeployment, FusionDeploymentActivate);

            AddExecutor(ExecutorType.Activate, CardId.AlbionTheShroudedDragon, AlbionTheShroudedDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialSaronir, BystialSaronirActivate);

            // summon for search
            AddExecutor(ExecutorType.Summon, CardId.AluberTheJesterOfDespia, AluberTheJesterOfDespiaSummon);
            AddExecutor(ExecutorType.Summon, CardId.GuidingQuemTheVirtuous, GuidingQuemTheVirtuousSummonForSearch);
            AddExecutor(ExecutorType.Activate, CardId.SpringansKitt, SpringansKittActivate);
            AddExecutor(ExecutorType.Summon, CardId.SpringansKitt, SpringansKittSummon);

            // fusion & lost
            AddExecutor(ExecutorType.Activate, CardId.BrandedLost, BrandedLostCardActivate);
            AddExecutor(ExecutorType.Activate, CardId.GranguignolTheDuskDragon, GranguignolTheDuskDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.AlbaLenatusTheAbyssDragon, AlbaLenatusTheAbyssDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.SprindTheIrondashDragon, SprindTheIrondashDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.TitanikladTheAshDragon, TitanikladTheAshDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon, AlbionTheBrandedDragonActivate);
            AddExecutor(ExecutorType.Activate, CardId.LubellionTheSearingDragon, LubellionTheSearingDragonActivate);
            AddExecutor(ExecutorType.Summon, CardId.BlazingCartesiaTheVirtuous, BlazingCartesiaTheVirtuousSummon);
            AddExecutor(ExecutorType.Activate, CardId.FallenOfAlbaz, FallenOfAlbazActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedFusion, BrandedFusionActivate);
            AddExecutor(ExecutorType.Activate, CardId.BlazingCartesiaTheVirtuous, BlazingCartesiaTheVirtuousActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInWhite, BrandedInWhiteActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInRed, BrandedInRedActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedLost, BrandedLostActivate);

            // albaz summon
            AddExecutor(ExecutorType.Summon, CardId.FallenOfAlbaz, FallenOfAlbazSummon);
            AddExecutor(ExecutorType.Summon, CardId.GuidingQuemTheVirtuous, GuidingQuemTheVirtuousSummon);

            // delay
            AddExecutor(ExecutorType.Activate, CardId.DespianTragedy, DespianTragedyActivate);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeMercourier, TriBrigadeMercourierActivateForSearch);
            AddExecutor(ExecutorType.Activate, CardId.NadirServant, NadirServantActivate);

            AddExecutor(ExecutorType.MonsterSet, SetForChimera);
            AddExecutor(ExecutorType.MonsterSet, CardId.DespianTragedy, DespianTragedySet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);
            AddExecutor(ExecutorType.Activate, CardId.GuidingQuemTheVirtuous, GuidingQuemTheVirtuousActivate);
            AddExecutor(ExecutorType.Activate, CardId.DespianLuluwalilith, DespianLuluwalilithActivate);

            AddExecutor(ExecutorType.Activate, FloogateActivate);

        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeBranded = 0x15d;
        const int SetcodeDespain = 0x164;
        const int SetcodeBystial = 0x188;
        const int SetcodeHorus = 0x19d;
        const int hintTimingMainEnd = 0x4;
        const int hintBattleStart = 0x8;
        List<int> notToNegateIdList = new List<int>{
            58699500, 20343502, CardId.AlbionTheShroudedDragon, 19403423
        };
        List<int> cannotBeFusionMaterialIdList = new List<int>
        {
            CardId.AlbaLenatusTheAbyssDragon, CardId.AlbionTheSanctifireDragon, 79229522, 65029288, 30864377, 33964637, 87116928, 13735899, 28226490, 80453041,
            _CardId.EaterOfMillions, 2992467, 16366810, 40217358, 47346782, 50893987, 71459017, 84433295, 85101097
        };
        List<int> albazFusionMonster = new List<int>
        {
            CardId.TitanikladTheAshDragon, CardId.SprindTheIrondashDragon, CardId.AlbionTheBrandedDragon, CardId.LubellionTheSearingDragon, CardId.AlbaLenatusTheAbyssDragon,
            CardId.MirrorjadeTheIcebladeDragon, CardId.RindbrummTheStrikingDragon, CardId.AlbionTheSanctifireDragon
        };
        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.AluberTheJesterOfDespia, _CardId.AshBlossom, _CardId.MaxxC, _CardId.InfiniteImpermanence }},
            {2, new List<int> { CardId.FallenOfAlbaz, CardId.NadirServant, CardId.FusionDeployment, _CardId.CalledByTheGrave }},
            {1, new List<int> { CardId.TheBystialLubellion, CardId.AlbionTheShroudedDragon, CardId.BystialSaronir, CardId.SpringansKitt, CardId.GuidingQuemTheVirtuous,
                                CardId.BlazingCartesiaTheVirtuous, CardId.TriBrigadeMercourier, CardId.DespianTragedy, CardId.BrandedInWhite,
                                CardId.BrandedFusion, CardId.GoldSarcophagus, CardId.FoolishBurial, CardId.BrandedInHighSpirits, CardId.BrandedOpening,
                                _CardId.CrossoutDesignator, CardId.BrandedInRed, CardId.BrandedLost, CardId.BrightestBlazingBrandedKing,
                                CardId.BrandedBeast, CardId.BrandedRetribution }}
        };
        List<int> dangerousDragonIdList = new List<int> { 27548199, 92892239, 98630720, 9753964, 99585850, 24361622, 27572350, 69120785, 96402918, 74294676, 42752141, 18511599, 35103106, 26268488 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };
        List<int> targetNegateIdList = new List<int> {
            _CardId.EffectVeiler, _CardId.InfiniteImpermanence, CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
            9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
        };

        bool summoned = false;
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        int dimensionShifterCount = 0;
        bool enemyActivateInfiniteImpermanenceFromHand = false;
        bool theBystialLubellionSelecting = false;
        bool albionTheShroudedDragonSelecting = false;
        bool nadirActivated = false;
        bool fusionToGYFlag = false;
        bool spSummoningAlbaz = false;
        int cartesiaSummonGoal = 0;
        int sanctifireSelectPositionCount = 0;
        int quemSummonFlag = 0;
        List<ClientCard> cartesiaMaterialList = new List<ClientCard>();
        List<ClientCard> brandedInRedMaterialList = new List<ClientCard>();
        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> sendToGYThisTurn = new List<ClientCard>();
        List<int> activatedCardIdList = new List<int>();
        ClientCard fusionTarget = null;
        List<ClientCard> selectedFusionMaterial = new List<ClientCard>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();

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
                if (Enemy.HasInSpellZone(CardId.SkillDrain, true, true)) return true;
            }
            if (disablecheck) return (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone) && Card.IsDisabled() && Card.IsFaceup();
            return false;
        }

        public bool CheckNumber41(ClientCard card)
        {
            return card != null && card.IsFaceup() && card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled();
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
        /// Check whether last chain card should be disabled.
        /// </summary>
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
            if (CheckAtAdvantage() && enemyActivateMaxxC && !enemyActivateLockBird && (Duel.Turn == 1 || Duel.Phase >= DuelPhase.Main2))
            {
                return true;
            }
            return false;
        }

        public bool CheckWhetherCanSummon()
        {
            return Duel.Player == 0 && Duel.Phase < DuelPhase.End && !summoned;
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
        /// Check whether it should be kept in grave to activate effect.
        /// If should, return true.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool CheckWhetherShouldKeepInGrave(ClientCard c)
        {
            if (c.IsCode(CardId.GranguignolTheDuskDragon) && c.Location == CardLocation.Grave) return true;
            if (!c.IsCode(new int[] { CardId.AlbaLenatusTheAbyssDragon, CardId.AlbionTheBrandedDragon, CardId.TitanikladTheAshDragon, CardId.DespianLuluwalilith,
                CardId.SprindTheIrondashDragon}))
            {
                return false;
            }
            return sendToGYThisTurn.Contains(c) && c.Location == CardLocation.Grave;
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

            ClientCard activatingAlbaz = Enemy.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.FallenOfAlbaz) && !c.IsDisabled()
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
                ClientCard abyssDragon = attackers.FirstOrDefault(c => c.IsCode(CardId.AlbaLenatusTheAbyssDragon) && !c.IsDisabled());
                if (abyssDragon != null)
                {
                    sortedAttacker.Remove(abyssDragon);
                    sortedAttacker.Insert(0, abyssDragon);
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

                // search operation
                if (hint == HintMsg.AddToHand)
                {
                    Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>();

                    switch (currentSolvingChain.Id)
                    {
                        case CardId.AluberTheJesterOfDespia:
                        case CardId.AluberTheJesterOfDespia + 1:
                        case CardId.SpringansKitt:
                            checkDict = new Dictionary<int, Func<bool>> {
                                {CardId.BrandedFusion, () => BrandedFusionActivateCheck()},
                                {CardId.BrandedLost, () => {
                                    if (Duel.Player == 0 && Duel.Phase >= DuelPhase.End) return false;
                                    if (Bot.HasInHandOrInSpellZone(CardId.BrandedFusion) && BrandedFusionActivateCheck()) return true;
                                    if (Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) && BrandedInWhiteActivateCheck()) return true;
                                    if (Bot.HasInHandOrInSpellZone(CardId.BrandedInRed) && BrandedInRedActivateCheck() != null) return true;
                                    if (!summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion()) return true;
                                    if ((Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) || (!summoned && Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous)))) return true;
                                    return false;
                                }
                                },
                                {CardId.BrandedInHighSpirits, BrandedInHighSpiritsActivateCheck},
                                {CardId.BrandedInRed, () => (Duel.Phase == DuelPhase.End && nadirActivated) || BrandedInRedActivateCheck() != null },
                                {CardId.BrandedInWhite, BrandedInWhiteActivateCheck },
                                {CardId.BrandedRetribution, () => cards.Any(c => c.IsCode(CardId.BrandedRetribution) && c.Location == CardLocation.Removed) },
                                {CardId.BrightestBlazingBrandedKing, () => Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(albazFusionMonster)) },
                                {CardId.BrandedOpening, () => Bot.Hand.Count > 2 }
                            };
                            break;
                        case CardId.NadirServant:
                            if (!summoned)
                            {
                                ClientCard quem = cards.FirstOrDefault(c => c.IsCode(CardId.GuidingQuemTheVirtuous));
                                if (quem != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { quem }, cards, min, max);
                                }
                            }
                            List<CardLocation> locList = new List<CardLocation> { CardLocation.Grave, CardLocation.Deck };
                            if (Bot.HasInGraveyard(CardId.RindbrummTheStrikingDragon) && cards.Where(c => c.IsOriginalCode(CardId.FallenOfAlbaz) && c.Location == CardLocation.Grave).Count() == 1)
                            {
                                locList = new List<CardLocation> { CardLocation.Deck, CardLocation.Grave };
                            }
                            foreach (int checkId in new[] { CardId.FallenOfAlbaz, CardId.GuidingQuemTheVirtuous })
                            {
                                foreach (CardLocation loc in locList)
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(checkId) && c.Location == loc);
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            break;
                        case CardId.BrandedLost:
                        case CardId.TriBrigadeMercourier:
                        case CardId.BrandedInHighSpirits:
                            checkDict = new Dictionary<int, Func<bool>>{
                                {CardId.TriBrigadeMercourier, () => Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(albazFusionMonster))
                                    || (Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) && Bot.HasInHandOrHasInMonstersZone(CardId.FallenOfAlbaz))},
                                {CardId.SpringansKitt, () => CheckWhetherCanSummon() && !activatedCardIdList.Contains(CardId.SpringansKitt + 1) },
                                {CardId.FallenOfAlbaz, () => (CheckWhetherCanSummon() && CheckAlbazFusion()) || Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) },
                                {CardId.GuidingQuemTheVirtuous, () => CheckWhetherCanSummon() },
                                {CardId.BlazingCartesiaTheVirtuous, () => CheckWhetherCanSummon() || (!CheckShouldNoMoreSpSummon() && Bot.HasInMonstersZoneOrInGraveyard(CardId.FallenOfAlbaz))},
                                {CardId.AlbionTheShroudedDragon, () => !CheckWhetherWillbeRemoved() && !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon) },
                            };
                            break;
                        case CardId.AlbaLenatusTheAbyssDragon:
                            checkDict = new Dictionary<int, Func<bool>>{
                                {CardId.BrandedFusion, () => BrandedFusionActivateCheck(false)},
                                {CardId.FusionDeployment, () => true}
                            };
                            break;
                        default:
                            break;
                    }

                    foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                    {
                        ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }
                }

                switch (currentSolvingChain.Id)
                {
                    // for lubellion
                    case CardId.TheBystialLubellion:
                        {
                            Dictionary<int, Func<bool>> lubellionCheckDict = new Dictionary<int, Func<bool>>();
                            if (hint == HintMsg.ToField)
                            {
                                lubellionCheckDict.Add(CardId.BrandedLost, () => {
                                    bool fusionFlag = Bot.HasInHandOrHasInMonstersZone(CardId.BlazingCartesiaTheVirtuous);
                                    if (!activatedCardIdList.Contains(CardId.BrandedFusion))
                                    {
                                        if (Bot.HasInHand(CardId.BrandedFusion) || (!summoned && CheckRemainInDeck(CardId.BrandedFusion) > 0 && Bot.HasInHand(new int[] {
                                    CardId.AluberTheJesterOfDespia, CardId.SpringansKitt
                                })))
                                        {
                                            fusionFlag = true;
                                        }
                                    }
                                    fusionFlag |= !summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion();
                                    fusionFlag |= Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) && BrandedInWhiteActivateCheck();
                                    fusionFlag |= Bot.HasInHandOrInSpellZone(CardId.BrandedInRed) && BrandedInRedActivateCheck() != null;

                                    return fusionFlag;
                                });
                                lubellionCheckDict.Add(CardId.BrandedBeast, () => true);
                            }
                            else if (hint == HintMsg.AddToHand)
                            {
                                lubellionCheckDict.Add(CardId.BystialSaronir, () => true);
                            }

                            foreach (KeyValuePair<int, Func<bool>> pair in lubellionCheckDict)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.Id == pair.Key);
                                if (target != null && pair.Value())
                                {
                                    SelectSTPlace(target, false);
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for albaz
                    case CardId.FallenOfAlbaz:
                    case CardId.FallenOfAlbaz + 1:
                        if (hint == HintMsg.SpSummon)
                        {
                            List<int> fusionTargetIdList = new List<int> {
                                CardId.MirrorjadeTheIcebladeDragon, CardId.AlbaLenatusTheAbyssDragon, CardId.AlbionTheBrandedDragon, CardId.AlbionTheSanctifireDragon,
                                CardId.LubellionTheSearingDragon, CardId.BorreloadFuriousDragon, CardId.TitanikladTheAshDragon, CardId.RindbrummTheStrikingDragon
                            };
                            foreach (int targetId in fusionTargetIdList)
                            {
                                if (targetId == CardId.LubellionTheSearingDragon && Bot.Hand.Count == 0) continue;
                                ClientCard target = cards.FirstOrDefault(c => c.IsCode(targetId));
                                if (target != null)
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        if (hint == HintMsg.FusionMaterial)
                        {
                            if (cards.Count == 1)
                            {
                                selectedFusionMaterial.AddRange(cards);
                                return Util.CheckSelectCount(cards, cards, min, max);
                            }
                            // select best enemy monster
                            List<ClientCard> sortedResult = cards.OrderByDescending(card => card.GetDefensePower()).ToList();
                            selectedFusionMaterial.Add(sortedResult[0]);
                            return Util.CheckSelectCount(sortedResult, cards, min, max);
                        }
                        break;

                    // for quem
                    case CardId.GuidingQuemTheVirtuous:
                        {
                            Dictionary<int, Func<bool>> quemCheckDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.BlazingCartesiaTheVirtuous, () => sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)) && CheckRemainInDeck(CardId.BrandedInHighSpirits) > 0 },
                                {CardId.BrandedFusion, () => Bot.HasInGraveyard(CardId.BrandedRetribution) },
                                {CardId.FallenOfAlbaz, () => !Bot.HasInGraveyard(CardId.FallenOfAlbaz) },
                                {CardId.TriBrigadeMercourier, () => Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) },
                                {CardId.BrandedRetribution, () => true },
                                {CardId.BrightestBlazingBrandedKing, () => !Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(albazFusionMonster) && fusionToGYFlag) },
                                {CardId.BrandedInHighSpirits, () => fusionToGYFlag },
                                {CardId.AlbionTheShroudedDragon, () => true },
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in quemCheckDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for cartesia
                    case CardId.BlazingCartesiaTheVirtuous:
                    case CardId.BlazingCartesiaTheVirtuous + 1:
                        if (hint == HintMsg.SpSummon)
                        {
                            // fix material list
                            cartesiaMaterialList = cartesiaMaterialList.Where(c => c != null && (c.Location == CardLocation.MonsterZone || c.Location == CardLocation.Hand)).ToList();

                            // filter normal material
                            List<ClientCard> materialList = Bot.MonsterZone.Where(c => c != null && c.GetDefensePower() <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList)).ToList();
                            materialList.AddRange(Bot.Hand.Where(c => c.IsMonster()
                                && !(CheckWhetherCanSummon() &&
                                    ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                                    || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                                    )
                                )
                            );

                            if (cartesiaSummonGoal > 0)
                            {
                                BlazingCartesiaTheVirtuousFusionCheck(cards, cartesiaSummonGoal, materialList, cartesiaMaterialList, out ClientCard _fusionTarget1, out _);
                                if (_fusionTarget1 != null)
                                {
                                    fusionTarget = _fusionTarget1;
                                    return Util.CheckSelectCount(new List<ClientCard> { fusionTarget }, cards, min, max);
                                }
                            }
                            BlazingCartesiaTheVirtuousFusionCheck(cards, 0, materialList, cartesiaMaterialList, out ClientCard _fusionTarget2, out _);
                            if (_fusionTarget2 != null)
                            {
                                fusionTarget = _fusionTarget2;
                                return Util.CheckSelectCount(new List<ClientCard> { fusionTarget }, cards, min, max);
                            }

                        }
                        if (hint == HintMsg.FusionMaterial)
                        {
                            List<ClientCard> mustSelectMaterialList = cartesiaMaterialList.Intersect(cards).ToList();
                            if (mustSelectMaterialList != null && mustSelectMaterialList.Count > 0)
                            {
                                selectedFusionMaterial.Add(mustSelectMaterialList[0]);
                                return Util.CheckSelectCount(mustSelectMaterialList, cards, min, max);
                            }

                            ClientCard lubellion = cards.FirstOrDefault(c => c != null && c.IsCode(CardId.TheBystialLubellion) && c.Location == CardLocation.MonsterZone);
                            if (lubellion != null && !Bot.HasInHandOrInSpellZone(CardId.BrandedBeast))
                            {
                                if (activatedCardIdList.Contains(CardId.TheBystialLubellion + 1) || CheckRemainInDeck(CardId.BrandedLost, CardId.BrandedBeast) == 0)
                                {
                                    if (Util.IsTurn1OrMain2() || Enemy.MonsterZone.Count(c => c != null && c.GetDefensePower() < 2500) > 0)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { lubellion }, cards, min, max);
                                    }
                                }
                            }

                            ClientCard selectTarget = cards
                                .Where(c => c.Attack <= 2500 && !(CheckWhetherCanSummon() &&
                                    ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                                    || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                                    )
                                )
                                .OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            if (selectTarget != null)
                            {
                                selectedFusionMaterial.Add(selectTarget);
                                return Util.CheckSelectCount(new List<ClientCard> { selectTarget }, cards, min, max);
                            }
                            selectTarget = cards.OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            if (selectTarget != null)
                            {
                                selectedFusionMaterial.Add(selectTarget);
                                return Util.CheckSelectCount(new List<ClientCard> { selectTarget }, cards, min, max);
                            }
                        }
                        break;

                    // for nadir
                    case CardId.NadirServant:
                        if (hint == HintMsg.ToGrave)
                        {
                            if (summoned)
                            {
                                if (CheckRemainInDeck(CardId.BlazingCartesiaTheVirtuous) > 0)
                                {
                                    ClientCard lulu = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.DespianLuluwalilith));
                                    if (lulu != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { lulu }, cards, min, max);
                                    }
                                }
                                if (!Bot.MonsterZone.Any(c => c != null && c.HasType(CardType.Fusion)) && CheckRemainInDeck(CardId.SpringansKitt) > 0)
                                {
                                    ClientCard ironDragon = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.SprindTheIrondashDragon));
                                    if (ironDragon != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { ironDragon }, cards, min, max);
                                    }
                                }
                            }
                            NadirServantActivateCheck(cards, true, out ClientCard target);
                            if (target != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                        break;

                    // for deployment
                    case CardId.FusionDeployment:
                        {
                            int summonId = FusionDeploymentSpSummonTarget();
                            if (summonId > 0)
                            {
                                if (hint == HintMsg.Confirm)
                                {
                                    if (summonId == CardId.BlazingCartesiaTheVirtuous)
                                    {
                                        ClientCard target = cards.FirstOrDefault(card => card.IsCode(CardId.GranguignolTheDuskDragon));
                                        if (target != null)
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                        }
                                    }
                                    else if (summonId == CardId.FallenOfAlbaz)
                                    {
                                        List<ClientCard> shuffleList = ShuffleList(new List<ClientCard>(cards));
                                        foreach (ClientCard target in shuffleList)
                                        {
                                            if (target.IsCode(albazFusionMonster))
                                            {
                                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                            }
                                        }
                                    }
                                }
                                if (hint == HintMsg.SpSummon)
                                {
                                    foreach (ClientCard target in cards)
                                    {
                                        if (target.IsCode(summonId))
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    // for branded in white
                    case CardId.BrandedInWhite:
                        if (hint == HintMsg.SpSummon)
                        {
                            BrandedInWhiteFusionTarget(cards, out ClientCard target);
                            if (target != null)
                            {
                                fusionTarget = target;
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                        if (hint == HintMsg.FusionMaterial && fusionTarget != null)
                        {
                            if (fusionTarget.IsCode(CardId.BorreloadFuriousDragon))
                            {
                                // select 2 dark dragon monster
                                foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                {
                                    List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc).OrderBy(c => c.GetDefensePower()).ToList();
                                    int banishedAlbazCount = Bot.Banished.Where(c => c.IsOriginalCode(CardId.FallenOfAlbaz)).Count();
                                    banishedAlbazCount += selectedFusionMaterial.Where(c => c.IsOriginalCode(CardId.FallenOfAlbaz)).Count();
                                    foreach (ClientCard target in cardsInLoc)
                                    {
                                        // keep albaz
                                        if (target.IsOriginalCode(CardId.FallenOfAlbaz) && banishedAlbazCount > 0)
                                        {
                                            continue;
                                        }
                                        selectedFusionMaterial.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            if (fusionTarget.IsCode(CardId.DespianQuaeritis))
                            {
                                // select despain
                                if (selectedFusionMaterial.Count == 0)
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                    {
                                        List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc && c.HasSetcode(SetcodeDespain) && (loc != CardLocation.Grave || !CheckWhetherShouldKeepInGrave(c)))
                                            .OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsInLoc.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(cardsInLoc[0]);
                                            return Util.CheckSelectCount(cardsInLoc, cards, min, max);
                                        }
                                    }
                                }
                                // select light/dark
                                else
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                    {
                                        List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark) && (loc != CardLocation.Grave || !CheckWhetherShouldKeepInGrave(c)))
                                            .OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsInLoc.Count > 0)
                                        {
                                            if (!activatedCardIdList.Contains(CardId.TriBrigadeMercourier + 1))
                                            {
                                                ClientCard mercourier = cardsInLoc.FirstOrDefault(c => c.IsCode(CardId.TriBrigadeMercourier));
                                                if (mercourier != null)
                                                {
                                                    selectedFusionMaterial.Add(mercourier);
                                                    return Util.CheckSelectCount(new List<ClientCard> { mercourier }, cards, min, max);
                                                }
                                            }
                                            if (!activatedCardIdList.Contains(CardId.DespianTragedy) && CheckRemainInDeck(CardId.AluberTheJesterOfDespia, CardId.GuidingQuemTheVirtuous) > 0)
                                            {
                                                ClientCard tragedy = cardsInLoc.FirstOrDefault(c => c.IsCode(CardId.DespianTragedy));
                                                if (tragedy != null)
                                                {
                                                    selectedFusionMaterial.Add(tragedy);
                                                    return Util.CheckSelectCount(new List<ClientCard> { tragedy }, cards, min, max);
                                                }
                                            }

                                            selectedFusionMaterial.Add(cardsInLoc[0]);
                                            return Util.CheckSelectCount(cardsInLoc, cards, min, max);
                                        }
                                    }
                                }
                            }
                            if (fusionTarget.IsCode(CardId.GuardianChimera))
                            {
                                List<ClientCard> goalMaterialList = ChimeraFusionMaterialList().Intersect(cards).ToList();
                                if (goalMaterialList.Count > 0)
                                {
                                    return Util.CheckSelectCount(goalMaterialList, cards, min, max);
                                }
                            }
                            if (fusionTarget.IsCode(albazFusionMonster))
                            {
                                // selecting albaz
                                if (selectedFusionMaterial.Count == 0)
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.MonsterZone, CardLocation.Hand })
                                    {
                                        ClientCard albaz = cards.Where(c => c.IsCode(CardId.FallenOfAlbaz)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                                        if (albaz != null)
                                        {
                                            selectedFusionMaterial.Add(albaz);
                                            return Util.CheckSelectCount(new List<ClientCard> { albaz }, cards, min, max);
                                        }
                                    }
                                }
                                else
                                {
                                    if (fusionTarget.IsOriginalCode(CardId.AlbaLenatusTheAbyssDragon) && cancelable)
                                    {
                                        return null;
                                    }
                                    if (Util.IsTurn1OrMain2() && !CheckWhetherWillbeRemoved())
                                    {
                                        ClientCard duskDragon = cards.FirstOrDefault(c => c.IsCode(CardId.GranguignolTheDuskDragon) && c.Location == CardLocation.MonsterZone);
                                        if (duskDragon != null)
                                        {
                                            selectedFusionMaterial.Add(duskDragon);
                                            return Util.CheckSelectCount(new List<ClientCard> { duskDragon }, cards, min, max);
                                        }
                                    }
                                    List<Func<ClientCard, bool>> funcList = new List<Func<ClientCard, bool>>
                                    {
                                        (c) => c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c),
                                        (c) => c.Location == CardLocation.MonsterZone && c.GetDefensePower() <= 2000,
                                        (c) => c.Location == CardLocation.Hand,
                                        (c) => c.Location == CardLocation.Grave,
                                        (c) => c.Location == CardLocation.MonsterZone
                                    };
                                    foreach (Func<ClientCard, bool> func in funcList)
                                    {
                                        List<ClientCard> targetList = cards.Where(c => func(c)).OrderBy(c => c.GetDefensePower()).ToList();
                                        if (targetList.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(targetList[0]);
                                            return Util.CheckSelectCount(new List<ClientCard> { targetList[0] }, cards, min, max);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    // for branded fusion
                    case CardId.BrandedFusion:
                        if (hint == HintMsg.SpSummon)
                        {
                            Dictionary<int, Func<bool>> brandedFusionCheckDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.TitanikladTheAshDragon, () => Enemy.HasInMonstersZone(CardId.KashtiraAriseHeart) },
                                {CardId.RindbrummTheStrikingDragon,  () => CheckWhetherWillbeRemoved() && CheckRemainInDeck(CardId.TriBrigadeMercourier) > 0},
                                {CardId.AlbionTheSanctifireDragon, () => CheckShouldNoMoreSpSummon()},
                                {CardId.AlbionTheBrandedDragon, () => {
                                    bool checkFlag = Bot.Graveyard.Any(c => c != null && c.IsMonster() && c.HasAttribute(CardAttribute.Dark) && !c.IsCode(cannotBeFusionMaterialIdList));
                                    checkFlag |= Bot.HasInHandOrHasInMonstersZone(CardId.TriBrigadeMercourier);
                                    checkFlag |= Bot.GetMonsters().Any(c => c.GetDefensePower() <= 1800 && c.HasAttribute(CardAttribute.Dark) && !c.IsCode(cannotBeFusionMaterialIdList));
                                    return checkFlag;
                                } },
                                {CardId.LubellionTheSearingDragon, () => !CheckWhetherNegated(true, true, CardType.Monster) && Bot.Hand.Count > 0 },
                                {CardId.MirrorjadeTheIcebladeDragon, () => Bot.HasInMonstersZone(new List<int>{
                                    CardId.GranguignolTheDuskDragon, CardId.AlbionTheBrandedDragon, CardId.LubellionTheSearingDragon}) }
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in brandedFusionCheckDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        if (hint == HintMsg.FusionMaterial)
                        {
                            // selecting albaz
                            if (selectedFusionMaterial.Count == 0)
                            {
                                foreach (CardLocation loc in new[] { CardLocation.Deck, CardLocation.Hand, CardLocation.MonsterZone })
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.FallenOfAlbaz) && c.Location == loc);
                                    if (target != null)
                                    {
                                        selectedFusionMaterial.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            // selecting another material
                            if (fusionTarget != null)
                            {
                                Dictionary<int, List<int>> materialDict = new Dictionary<int, List<int>>
                                {
                                    { CardId.AlbionTheSanctifireDragon, new List<int>{CardId.BlazingCartesiaTheVirtuous, _CardId.EffectVeiler, CardId.GuidingQuemTheVirtuous} },
                                    { CardId.MirrorjadeTheIcebladeDragon, new List<int>{CardId.GranguignolTheDuskDragon, CardId.AlbionTheBrandedDragon, CardId.DespianLuluwalilith } },
                                    { CardId.LubellionTheSearingDragon, new List<int>{
                                        CardId.DespianTragedy, CardId.BystialSaronir, CardId.AlbionTheShroudedDragon, CardId.AluberTheJesterOfDespia, CardId.TriBrigadeMercourier } },
                                    { CardId.TitanikladTheAshDragon, new List<int>{CardId.TheBystialLubellion, CardId.AlbionTheShroudedDragon, CardId.BystialSaronir} },
                                    { CardId.RindbrummTheStrikingDragon, new List<int>{CardId.TriBrigadeMercourier, CardId.SpringansKitt } },
                                    { CardId.AlbionTheBrandedDragon, new List<int>{CardId.TheBystialLubellion, CardId.BlazingCartesiaTheVirtuous, CardId.GuidingQuemTheVirtuous } }
                                };
                                materialDict.TryGetValue(fusionTarget.GetOriginCode(), out List<int> checkIdList);
                                if (checkIdList != null && checkIdList.Count > 0)
                                {
                                    foreach (CardLocation location in new List<CardLocation> { CardLocation.Deck, CardLocation.Hand, CardLocation.MonsterZone })
                                    {
                                        foreach (int checkId in checkIdList)
                                        {
                                            ClientCard target = cards.FirstOrDefault(c => c.Location == location && c.IsCode(checkId));
                                            if (target != null)
                                            {
                                                selectedFusionMaterial.Add(target);
                                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    // for Sarcophagus
                    case CardId.GoldSarcophagus:
                        {
                            GoldSarcophagusTarget(cards, out ClientCard sarcophagusTarget);
                            if (sarcophagusTarget != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { sarcophagusTarget }, cards, min, max);
                            }
                        }
                        break;

                    // for burial
                    case CardId.FoolishBurial:
                    case CardId.FoolishBurial + 1:
                        {
                            FoolishBurialTarget(cards, out ClientCard burialTarget);
                            if (burialTarget != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { burialTarget }, cards, min, max);
                            }
                        }
                        break;

                    // for high spirit
                    case CardId.BrandedInHighSpirits:
                        // confirm
                        if (hint == HintMsg.Confirm)
                        {
                            if (Duel.Phase == DuelPhase.End && Bot.HasInMonstersZone(CardId.GuidingQuemTheVirtuous))
                            {
                                ClientCard cartesia = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.BlazingCartesiaTheVirtuous));
                                if (cartesia != null)
                                {
                                    fusionTarget = cartesia;
                                    return Util.CheckSelectCount(new List<ClientCard> { cartesia }, cards, min, max);
                                }
                            }
                            bool activatingShroudedFlag = Duel.CurrentChain.Any(c => c.IsOriginalCode(CardId.AlbionTheShroudedDragon) && c.Location == CardLocation.Hand);
                            if (activatingShroudedFlag)
                            {
                                ClientCard shrouded = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.AlbionTheShroudedDragon));
                                if (shrouded != null)
                                {
                                    fusionTarget = shrouded;
                                    return Util.CheckSelectCount(new List<ClientCard> { shrouded }, cards, min, max);
                                }
                            }
                            List<int> discardIdList = new List<int> {
                                CardId.BystialSaronir, CardId.AlbionTheShroudedDragon, CardId.TheBystialLubellion, CardId.BlazingCartesiaTheVirtuous,
                                CardId.FallenOfAlbaz, CardId.TriBrigadeMercourier
                            };
                            foreach (int discardId in discardIdList)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(discardId));
                                if (target != null)
                                {
                                    // lubellion check
                                    if (discardId == CardId.TheBystialLubellion && Duel.Player == 0 && (Duel.Phase <= DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                                        && CheckRemainInDeck(CardId.BystialSaronir) > 0 && !activatedCardIdList.Contains(CardId.TheBystialLubellion))
                                    {
                                        continue;
                                    }
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        // discard
                        if (hint == HintMsg.ToGrave)
                        {
                            List<int> discardIdList = new List<int> {
                                CardId.AlbionTheBrandedDragon, CardId.TitanikladTheAshDragon, CardId.RindbrummTheStrikingDragon, CardId.AlbaLenatusTheAbyssDragon,
                                CardId.GranguignolTheDuskDragon
                            };
                            foreach (int discardId in discardIdList)
                            {
                                if (sendToGYThisTurn.Any(c => c.IsOriginalCode(discardId))) continue;
                                ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(discardId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for opening
                    case CardId.BrandedOpening:
                        if (hint == HintMsg.OperateCard)
                        {
                            Dictionary<int, Func<bool>> openingCheckDict = new Dictionary<int, Func<bool>>{
                                {CardId.AluberTheJesterOfDespia, () => !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && !DefaultCheckWhetherCardIdIsNegated(CardId.AluberTheJesterOfDespia)
                                    && !(CheckWhetherCanSummon() && Bot.HasInHand(CardId.AluberTheJesterOfDespia))},
                                {CardId.GuidingQuemTheVirtuous, () => true },
                                {CardId.DespianTragedy, () => true }
                            };
                            foreach (KeyValuePair<int, Func<bool>> pair in openingCheckDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for branded in red
                    case CardId.BrandedInRed:
                        if (hint == HintMsg.SpSummon)
                        {
                            // filter normal material
                            List<ClientCard> materialList = Bot.MonsterZone.Where(c => c != null && c.GetDefensePower() <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList)).ToList();
                            materialList.AddRange(Bot.Hand.Where(c => c.IsMonster()
                                && !(CheckWhetherCanSummon() &&
                                    ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                                    || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                                    )
                                )
                            );

                            BrandedInRedFusionCheck(cards, 0, materialList, brandedInRedMaterialList, out ClientCard _fusionTarget, out _);
                            if (_fusionTarget != null)
                            {
                                fusionTarget = _fusionTarget;
                                return Util.CheckSelectCount(new List<ClientCard> { fusionTarget }, cards, min, max);
                            }

                        }
                        if (hint == HintMsg.FusionMaterial)
                        {
                            List<ClientCard> mustSelectMaterialList = brandedInRedMaterialList.Intersect(cards).ToList();
                            if (mustSelectMaterialList != null && mustSelectMaterialList.Count > 0)
                            {
                                selectedFusionMaterial.Add(mustSelectMaterialList[0]);
                                return Util.CheckSelectCount(mustSelectMaterialList, cards, min, max);
                            }

                            ClientCard selectTarget = cards
                                .Where(c => c.Attack <= 2500 && !(CheckWhetherCanSummon() &&
                                    ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                                    || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                                    )
                                )
                                .OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            if (selectTarget != null)
                            {
                                selectedFusionMaterial.Add(selectTarget);
                                return Util.CheckSelectCount(new List<ClientCard> { selectTarget }, cards, min, max);
                            }
                            selectTarget = cards.OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            if (selectTarget != null)
                            {
                                selectedFusionMaterial.Add(selectTarget);
                                return Util.CheckSelectCount(new List<ClientCard> { selectTarget }, cards, min, max);
                            }
                        }
                        break;

                    // for branded king
                    case CardId.BrightestBlazingBrandedKing:
                        if (hint == HintMsg.Faceup)
                        {
                            List<int> targetIdList = new List<int>
                            {
                                CardId.MirrorjadeTheIcebladeDragon, CardId.AlbionTheSanctifireDragon, CardId.RindbrummTheStrikingDragon
                            };
                            List<Func<ClientCard, bool>> funcList = new List<Func<ClientCard, bool>>
                            {
                                (c) => Duel.CurrentChain.Contains(c),
                                (c) => true
                            };
                            foreach (Func<ClientCard, bool> func in funcList)
                            {
                                List<ClientCard> chainedList = cards.Where(c => func(c)).ToList();
                                if (chainedList.Count > 0)
                                {
                                    foreach (int checkId in targetIdList)
                                    {
                                        ClientCard target = chainedList.FirstOrDefault(c => c.IsOriginalCode(checkId));
                                        if (target != null)
                                        {
                                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                        }
                                    }
                                    ClientCard otherChainTarget = chainedList.FirstOrDefault(c => Duel.CurrentChain.Contains(c));
                                    if (otherChainTarget != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { otherChainTarget }, cards, min, max);
                                    }
                                }
                            }
                        }
                        break;

                    // for retribution
                    case CardId.BrandedRetribution:
                        {
                            ClientCard searing = cards.FirstOrDefault(c => c.IsCode(CardId.LubellionTheSearingDragon));
                            if (searing != null)
                            {
                                selectedFusionMaterial.Add(searing);
                                return Util.CheckSelectCount(new List<ClientCard> { searing }, cards, min, max);
                            }
                            List<int> checkIdList = new List<int> { CardId.AlbionTheBrandedDragon, CardId.MirrorjadeTheIcebladeDragon, CardId.TitanikladTheAshDragon, CardId.AlbaLenatusTheAbyssDragon,
                                CardId.AlbionTheSanctifireDragon, CardId.SprindTheIrondashDragon};
                            foreach (int checkId in checkIdList)
                            {
                                List<ClientCard> gravePriorityList = cards.Where(c => c != null && c.IsCode(checkId) && c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c)).ToList();
                                if (gravePriorityList.Count > 0)
                                {
                                    selectedFusionMaterial.Add(gravePriorityList[0]);
                                    return Util.CheckSelectCount(new List<ClientCard> { gravePriorityList[0] }, cards, min, max);
                                }
                            }
                            List<ClientCard> graveList = cards.Where(c => c != null && c.Location == CardLocation.Grave).ToList();
                            if (graveList.Count > 0)
                            {
                                selectedFusionMaterial.Add(graveList[0]);
                                return Util.CheckSelectCount(new List<ClientCard> { graveList[0] }, cards, min, max);
                            }
                            ClientCard monsterOnField = cards.Where(c => c != null && c.Location == CardLocation.MonsterZone).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            if (monsterOnField != null)
                            {
                                selectedFusionMaterial.Add(monsterOnField);
                                return Util.CheckSelectCount(new List<ClientCard> { monsterOnField }, cards, min, max);
                            }
                        }
                        break;

                    // for chimera
                    case CardId.GuardianChimera:
                        {
                            List<ClientCard> targetList = new List<ClientCard>();

                            targetList.AddRange(GetProblematicEnemyCardList(false, false, CardType.Monster));
                            int bestBotPower = Util.GetBestPower(Bot);
                            targetList.AddRange(Enemy.MonsterZone.Where(c => c != null && !targetList.Contains(c) && c.GetDefensePower() >= bestBotPower).OrderByDescending(c => c.GetDefensePower()));
                            targetList.AddRange(ShuffleList(enemyPlaceThisTurn));

                            return Util.CheckSelectCount(targetList, cards, min, max);
                        }
                        break;

                    // for sanctifire
                    case CardId.AlbionTheSanctifireDragon:
                        if (hint == Util.GetStringId(CardId.AlbionTheSanctifireDragon, 1))
                        {
                            ClientCard albaz = cards.FirstOrDefault(c => c.IsOriginalCode(CardId.FallenOfAlbaz));
                            if (albaz != null && CheckAlbazFusion())
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { albaz }, cards, min, max);
                            }

                            ClientCard floogate = cards.Where(c => c.IsFloodgate()).OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                            if (floogate != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { floogate }, cards, min, max);
                            }

                            return Util.CheckSelectCount(cards.OrderByDescending(c => c.GetDefensePower()).ToList(), cards, min, max);
                        }
                        break;

                    // for mirrorjade
                    case CardId.MirrorjadeTheIcebladeDragon:
                        {
                            List<ClientCard> floodgateList = ShuffleList(cards.Where(c => c.Controller == 1 && c.IsFloodgate()).ToList());
                            if (floodgateList.Count > 0)
                            {
                                return Util.CheckSelectCount(floodgateList, cards, min, max);
                            }

                            List<ClientCard> extraMonsterList = cards.Where(c => c.Controller == 1 && (
                                c.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                                    || (c.HasType(CardType.Link) && c.LinkCount >= 2))).OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (extraMonsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(extraMonsterList, cards, min, max);
                            }

                            ClientCard worstBotMonster = Util.GetWorstBotMonster();
                            int worstBotPower = worstBotMonster == null ? 0 : worstBotMonster.GetDefensePower();
                            List<ClientCard> betterMonsterList = cards.Where(c => c.Controller == 1 && c.GetDefensePower() >= worstBotPower).OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (betterMonsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(betterMonsterList, cards, min, max);
                            }

                            List<ClientCard> dangerMonsterList = cards.Where(c => c.Controller == 1 && (c.IsMonsterDangerous() || c.IsMonsterInvincible()))
                                .OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (dangerMonsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(dangerMonsterList, cards, min, max);
                            }

                            List<ClientCard> allEnemyMonsterList = cards.Where(c => c.Controller == 1).OrderByDescending(c => c.IsFacedown() ? 0 : c.GetDefensePower()).ToList();
                            if (allEnemyMonsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(allEnemyMonsterList, cards, min, max);
                            }

                            ClientCard botMonsterWithEffect = cards.FirstOrDefault(c => c.Controller == 0 && c.IsCode(new[] { CardId.DespianTragedy, CardId.TriBrigadeMercourier }));
                            if (botMonsterWithEffect != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { botMonsterWithEffect }, cards, min, max);
                            }
                            ClientCard botLubellion = cards.FirstOrDefault(c => c.Controller == 0 && c.IsCode(CardId.TheBystialLubellion));
                            if (botLubellion != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { botLubellion }, cards, min, max);
                            }


                            List<ClientCard> allBotMonster = cards.Where(c => c.Controller == 0).OrderBy(c => c.IsFacedown() ? 0 : c.GetDefensePower()).ToList();
                            if (allBotMonster.Count > 0)
                            {
                                return Util.CheckSelectCount(allBotMonster, cards, min, max);
                            }
                        }
                        break;

                    // for searing dragon
                    case CardId.LubellionTheSearingDragon:
                        if (hint == HintMsg.SpSummon)
                        {
                            LubellionTheSearingDragonFusionTarget(cards, out ClientCard target);
                            if (target != null)
                            {
                                fusionTarget = target;
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                        if (hint == HintMsg.FusionMaterial && fusionTarget != null)
                        {
                            if (fusionTarget.IsCode(CardId.BorreloadFuriousDragon))
                            {
                                // select 2 dark dragon monster
                                List<Func<ClientCard, bool>> furiousFuncList = new List<Func<ClientCard, bool>>
                            {
                                (c) => c.IsFaceup() && c.Location == CardLocation.Removed && c.IsCode(CardId.AlbionTheBrandedDragon),
                                (c) => c.IsFaceup() && c.Location == CardLocation.Removed && c.IsCode(CardId.FallenOfAlbaz),
                                (c) => c.Location == CardLocation.Grave && c.IsCode(CardId.AlbionTheBrandedDragon) && !CheckWhetherShouldKeepInGrave(c),
                                (c) => c.IsFaceup() && c.Location == CardLocation.Removed && c.IsCode(CardId.TitanikladTheAshDragon),
                                (c) => c.IsFaceup() && c.Location == CardLocation.Removed,
                                (c) => c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c),
                                (c) => c.Location == CardLocation.Grave,
                                (c) => c.Location == CardLocation.Hand || c.Location == CardLocation.MonsterZone
                            };

                                foreach (Func<ClientCard, bool> func in furiousFuncList)
                                {
                                    List<ClientCard> cardWithFunc = cards.Where(c => func(c)).OrderBy(c => c.GetDefensePower()).ToList();
                                    if (cardWithFunc.Count > 0)
                                    {
                                        selectedFusionMaterial.Add(cardWithFunc[0]);
                                        return Util.CheckSelectCount(new List<ClientCard> { cardWithFunc[0] }, cards, min, max);
                                    }
                                }
                            }
                            List<Func<ClientCard, bool>> funcList = new List<Func<ClientCard, bool>>
                            {
                                (c) => c.IsFaceup() && c.Location == CardLocation.Removed,
                                (c) => c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c),
                                (c) => c.IsCode(CardId.LubellionTheSearingDragon),
                                (c) => c.Location == CardLocation.Grave,
                                (c) => c.Location == CardLocation.MonsterZone,
                                (c) => c.Location == CardLocation.Hand,
                            };
                            if (selectedFusionMaterial.Count == 0)
                            {
                                if (fusionTarget.IsOriginalCode(CardId.DespianQuaeritis))
                                {
                                    foreach (Func<ClientCard, bool> func in funcList)
                                    {
                                        List<ClientCard> cardsWithFunc = cards.Where(c => func(c) && c.HasSetcode(SetcodeDespain)).OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsWithFunc.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(cardsWithFunc[0]);
                                            return Util.CheckSelectCount(cardsWithFunc, cards, min, max);
                                        }
                                    }
                                }
                                if (fusionTarget.IsCode(albazFusionMonster))
                                {
                                    foreach (Func<ClientCard, bool> func in funcList)
                                    {
                                        List<ClientCard> cardsWithFunc = cards.Where(c => func(c) && c.IsCode(CardId.FallenOfAlbaz)).OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsWithFunc.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(cardsWithFunc[0]);
                                            return Util.CheckSelectCount(cardsWithFunc, cards, min, max);
                                        }
                                    }
                                }
                            }
                            if (fusionTarget.IsCode(CardId.AlbaLenatusTheAbyssDragon))
                            {
                                // select non-albaz
                                if (selectedFusionMaterial.Count > 0)
                                {
                                    funcList = new List<Func<ClientCard, bool>>
                                {
                                    (c) => c.IsFaceup() && c.Location == CardLocation.Removed,
                                    (c) => c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c),
                                    (c) => c.IsCode(CardId.LubellionTheSearingDragon),
                                };
                                }
                            }
                            foreach (Func<ClientCard, bool> func in funcList)
                            {
                                List<ClientCard> cardsWithFunc = cards.Where(c => func(c)).OrderBy(c => c.GetDefensePower()).ToList();
                                if (cardsWithFunc.Count > 0)
                                {
                                    selectedFusionMaterial.Add(cardsWithFunc[0]);
                                    return Util.CheckSelectCount(cardsWithFunc, cards, min, max);
                                }
                            }
                            if (fusionTarget.IsOriginalCode(CardId.AlbaLenatusTheAbyssDragon) && cancelable)
                            {
                                return null;
                            }
                        }
                        break;

                    // for granguignol
                    case CardId.GranguignolTheDuskDragon:
                        if (hint == HintMsg.ToGrave)
                        {
                            GranguignolTheDuskDragonSendToGYTarget(cards, out ClientCard target);
                            if (target != null)
                            {
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                        if (hint == HintMsg.SpSummon)
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                        {
                            {CardId.DespianQuaeritis, () => Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.Attack >= Util.GetBestPower(Bot) && !(c.HasType(CardType.Fusion) && c.Level >= 8)) },
                            {CardId.GuidingQuemTheVirtuous, () => Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon) || Util.GetOneEnemyBetterThanValue(1500) == null },
                            {CardId.DespianLuluwalilith, () => !(Duel.Player == 0 && Bot.HasInHandOrInSpellZone(CardId.BrandedFusion)) }
                        };

                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for Quaeritis
                    case CardId.DespianQuaeritis:
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.FallenOfAlbaz, () => CheckAlbazFusion() },
                                {CardId.GuidingQuemTheVirtuous, () => !DefaultCheckWhetherCardIdIsNegated(CardId.GuidingQuemTheVirtuous) },
                                {CardId.AluberTheJesterOfDespia, () => !DefaultCheckWhetherCardIdIsNegated(CardId.AluberTheJesterOfDespia) },
                                {CardId.DespianTragedy, () => true }
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for irondash dragon
                    case CardId.SprindTheIrondashDragon:
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.FallenOfAlbaz, () => CheckAlbazFusion() },
                                {CardId.SpringansKitt, () => !DefaultCheckWhetherCardIdIsNegated(CardId.SpringansKitt) }
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for ash dragon
                    case CardId.TitanikladTheAshDragon:
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.FallenOfAlbaz, () => CheckAlbazFusion() },
                                {CardId.GuidingQuemTheVirtuous, () => !DefaultCheckWhetherCardIdIsNegated(CardId.GuidingQuemTheVirtuous) }
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for rindrumm
                    case CardId.RindbrummTheStrikingDragon:
                        if (hint == HintMsg.SpSummon)
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.FallenOfAlbaz, () => CheckAlbazFusion() },
                                {CardId.RindbrummTheStrikingDragon, () => true },
                                {CardId.AlbionTheShroudedDragon, () => true }
                            };
                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        if (hint == HintMsg.ReturnToHand)
                        {
                            List<ClientCard> problemList = GetProblematicEnemyCardList(false, true, CardType.Monster).Intersect(cards).OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (problemList.Count > 0)
                            {
                                return Util.CheckSelectCount(problemList, cards, min, max);
                            }
                            ClientCard worstBotMonster = Util.GetWorstBotMonster();
                            int worstBotPower = worstBotMonster == null ? 0 : worstBotMonster.GetDefensePower();
                            List<ClientCard> dangerList = cards.Where(c => c.IsFaceup() && c.Controller == 1 && c.GetDefensePower() > worstBotPower).OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (dangerList.Count > 0)
                            {
                                return Util.CheckSelectCount(dangerList, cards, min, max);
                            }
                            List<int> checkIdList = new List<int> { CardId.AluberTheJesterOfDespia, CardId.SpringansKitt };
                            foreach (int checkId in checkIdList)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.Controller == 0 && c.IsCode(checkId));
                                if (target != null)
                                {
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                            List<ClientCard> enemyMonsterList = cards.Where(c => c.Controller == 1).OrderByDescending(c => c.GetDefensePower()).ToList();
                            if (enemyMonsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(enemyMonsterList, cards, min, max);
                            }
                            return Util.CheckSelectCount(cards.Where(c => c.Controller == 0).OrderByDescending(c => c.GetDefensePower()).ToList(), cards, min, max);
                        }
                        break;

                    // for branded dragon
                    case CardId.AlbionTheBrandedDragon:
                        if (hint == HintMsg.SpSummon)
                        {
                            AlbionTheBrandedDragonFusionTarget(cards, out ClientCard target);
                            if (target != null)
                            {
                                fusionTarget = target;
                                return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                            }
                        }
                        if (hint == HintMsg.FusionMaterial && fusionTarget != null)
                        {
                            if (fusionTarget.IsCode(CardId.BorreloadFuriousDragon))
                            {
                                // select 2 dark dragon monster
                                foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                {
                                    List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc).OrderBy(c => c.GetDefensePower()).ToList();
                                    int banishedAlbazCount = Bot.Banished.Where(c => c.IsOriginalCode(CardId.FallenOfAlbaz)).Count();
                                    banishedAlbazCount += selectedFusionMaterial.Where(c => c.IsOriginalCode(CardId.FallenOfAlbaz)).Count();
                                    foreach (ClientCard target in cardsInLoc)
                                    {
                                        // keep albaz
                                        if (target.IsOriginalCode(CardId.FallenOfAlbaz) && banishedAlbazCount > 0)
                                        {
                                            continue;
                                        }
                                        selectedFusionMaterial.Add(target);
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                            if (fusionTarget.IsCode(CardId.DespianQuaeritis))
                            {
                                // select despain
                                if (selectedFusionMaterial.Count == 0)
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                    {
                                        List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc && c.HasSetcode(SetcodeDespain) && (loc != CardLocation.Grave || !CheckWhetherShouldKeepInGrave(c)))
                                            .OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsInLoc.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(cardsInLoc[0]);
                                            return Util.CheckSelectCount(cardsInLoc, cards, min, max);
                                        }
                                    }
                                }
                                // select light/dark
                                else
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.Hand, CardLocation.MonsterZone })
                                    {
                                        List<ClientCard> cardsInLoc = cards.Where(c => c.Location == loc && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark) && (loc != CardLocation.Grave || !CheckWhetherShouldKeepInGrave(c)))
                                            .OrderBy(c => c.GetDefensePower()).ToList();
                                        if (cardsInLoc.Count > 0)
                                        {
                                            if (!activatedCardIdList.Contains(CardId.TriBrigadeMercourier + 1))
                                            {
                                                ClientCard mercourier = cardsInLoc.FirstOrDefault(c => c.IsCode(CardId.TriBrigadeMercourier));
                                                if (mercourier != null)
                                                {
                                                    selectedFusionMaterial.Add(mercourier);
                                                    return Util.CheckSelectCount(new List<ClientCard> { mercourier }, cards, min, max);
                                                }
                                            }
                                            if (!activatedCardIdList.Contains(CardId.DespianTragedy) && CheckRemainInDeck(CardId.AluberTheJesterOfDespia, CardId.GuidingQuemTheVirtuous) > 0)
                                            {
                                                ClientCard tragedy = cardsInLoc.FirstOrDefault(c => c.IsCode(CardId.DespianTragedy));
                                                if (tragedy != null)
                                                {
                                                    selectedFusionMaterial.Add(tragedy);
                                                    return Util.CheckSelectCount(new List<ClientCard> { tragedy }, cards, min, max);
                                                }
                                            }

                                            selectedFusionMaterial.Add(cardsInLoc[0]);
                                            return Util.CheckSelectCount(cardsInLoc, cards, min, max);
                                        }
                                    }
                                }
                            }
                            if (fusionTarget.IsCode(albazFusionMonster))
                            {
                                // selecting albaz
                                if (selectedFusionMaterial.Count == 0)
                                {
                                    foreach (CardLocation loc in new[] { CardLocation.Grave, CardLocation.MonsterZone, CardLocation.Hand })
                                    {
                                        ClientCard albaz = cards.Where(c => c.IsCode(CardId.FallenOfAlbaz) && c.Location == loc).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                                        if (albaz != null)
                                        {
                                            selectedFusionMaterial.Add(albaz);
                                            return Util.CheckSelectCount(new List<ClientCard> { albaz }, cards, min, max);
                                        }
                                    }
                                }
                                else
                                {
                                    if (fusionTarget.IsOriginalCode(CardId.AlbaLenatusTheAbyssDragon) && cancelable)
                                    {
                                        return null;
                                    }
                                    List<Func<ClientCard, bool>> funcList = new List<Func<ClientCard, bool>>
                                    {
                                        (c) => c.Location == CardLocation.Grave && !CheckWhetherShouldKeepInGrave(c),
                                        (c) => c.Location == CardLocation.MonsterZone && c.GetDefensePower() <= 2000,
                                        (c) => c.Location == CardLocation.Grave,
                                        (c) => c.Location == CardLocation.Hand,
                                        (c) => c.Location == CardLocation.MonsterZone
                                    };
                                    foreach (Func<ClientCard, bool> func in funcList)
                                    {
                                        List<ClientCard> targetList = cards.Where(c => func(c)).OrderBy(c => c.GetDefensePower()).ToList();
                                        if (targetList.Count > 0)
                                        {
                                            selectedFusionMaterial.Add(targetList[0]);
                                            return Util.CheckSelectCount(new List<ClientCard> { targetList[0] }, cards, min, max);
                                        }
                                    }
                                }
                            }
                        }
                        if (hint == HintMsg.OperateCard)
                        {
                            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                            {
                                {CardId.BrandedInHighSpirits, () => Bot.HasInMonstersZone(CardId.GuidingQuemTheVirtuous) && BrandedInHighSpiritsActivateCheck() },
                                {CardId.BrightestBlazingBrandedKing, () => Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(albazFusionMonster)) },
                                {CardId.BrandedInRed, () => Bot.Graveyard.Any(c => c != null && (c.HasSetcode(SetcodeDespain) || c.IsCode(CardId.FallenOfAlbaz))) },
                                {CardId.BrandedRetribution, () => Bot.Graveyard.Where(c => c != null && c.IsCode(albazFusionMonster)).Count() > 1 },
                                {CardId.BrandedFusion, () => CheckRemainInDeck(CardId.FallenOfAlbaz) > 0 },
                                {CardId.BrandedBeast, () => Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeBystial)) },
                                {CardId.BrandedLost, () => true },
                                {CardId.BrandedInWhite, () => true }
                            };

                            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                            {
                                ClientCard target = cards.FirstOrDefault(c => c.IsOriginalCode(pair.Key));
                                if (target != null && pair.Value())
                                {
                                    fusionTarget = target;
                                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                }
                            }
                        }
                        break;

                    // for luluwa
                    case CardId.DespianLuluwalilith:
                        if (hint == HintMsg.Disable)
                        {
                            List<ClientCard> enemyCardList = cards.Where(c => c.IsFaceup() && c.Controller == 1).ToList();
                            List<ClientCard> problemCardList = GetProblematicEnemyCardList(false, false, 0).Intersect(enemyCardList).ToList();
                            if (problemCardList.Count > 0)
                            {
                                return Util.CheckSelectCount(ShuffleList(problemCardList), cards, min, max);
                            }
                            List<ClientCard> monsterList = GetMonsterListForTargetNegate(false, 0).Intersect(enemyCardList).ToList();
                            if (monsterList.Count > 0)
                            {
                                return Util.CheckSelectCount(ShuffleList(monsterList), cards, min, max);
                            }
                            if (enemyCardList.Count > 0)
                            {
                                return Util.CheckSelectCount(ShuffleList(enemyCardList), cards, min, max);
                            }
                        }
                        if (hint == HintMsg.SpSummon)
                        {
                            foreach (CardLocation loc in new[] { CardLocation.Deck, CardLocation.Hand })
                            {
                                foreach (int checkId in new List<int> { CardId.BlazingCartesiaTheVirtuous, CardId.GuidingQuemTheVirtuous })
                                {
                                    ClientCard target = cards.FirstOrDefault(c => c.Location == loc && c.IsOriginalCode(checkId));
                                    if (target != null)
                                    {
                                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            // drop 1 hand
            bool discardHand = hint == HintMsg.Discard;
            bool handToDeck = hint == HintMsg.ToDeck && cards.All(c => c.Location == CardLocation.Hand);
            if (min == 1 && max == 1 && (discardHand || handToDeck))
            {
                if (currentSolvingChain != null && currentSolvingChain.IsCode(CardId.BrandedOpening))
                {
                    ClientCard tragedy = cards.FirstOrDefault(card => card.IsCode(CardId.DespianTragedy));
                    if (tragedy != null)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { tragedy }, cards, min, max);
                    }
                }
                if (discardHand)
                {
                    // discard activating shrouded
                    foreach (ClientCard target in cards)
                    {
                        if (target.IsCode(CardId.AlbionTheShroudedDragon) && Duel.CurrentChain.Contains(target))
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }

                    List<int> discardList = new List<int> {
                        CardId.BrandedRetribution, CardId.AlbionTheShroudedDragon, CardId.BystialSaronir, CardId.BrightestBlazingBrandedKing,
                        CardId.BrandedInHighSpirits, CardId.BlazingCartesiaTheVirtuous, CardId.DespianTragedy };
                    foreach (int id in discardList)
                    {
                        ClientCard card = cards.FirstOrDefault(c => c.IsCode(id));
                        if (card != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                        }
                    }
                }
                // return dump card
                foreach (ClientCard card in cards)
                {
                    if (cards.Where(c => c.IsCode(card.Id)).Count() > 1)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { card }, cards, min, max);
                    }
                }
                List<int> improperCardIdList = new List<int>
                {
                    CardId.BrandedRetribution, CardId.BrandedInHighSpirits, CardId.DespianTragedy, CardId.FusionDeployment, CardId.BrandedBeast,
                    CardId.AlbionTheShroudedDragon, CardId.BrandedOpening, CardId.GoldSarcophagus, CardId.FoolishBurial, CardId.FallenOfAlbaz, 
                    CardId.BrandedInRed, _CardId.InfiniteImpermanence, CardId.TheBystialLubellion, CardId.BrandedLost, CardId.SpringansKitt,
                    CardId.GuidingQuemTheVirtuous, _CardId.CrossoutDesignator, _CardId.CalledByTheGrave, CardId.TriBrigadeMercourier, _CardId.AshBlossom, _CardId.MaxxC
                };
                foreach (int id in improperCardIdList)
                {
                    if (id == CardId.BrandedLost && Bot.HasInHand(CardId.BrandedFusion) && BrandedFusionActivateCheck()) continue;
                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(id));
                    if (target != null)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                    }
                }
            }
            else if (discardHand && min > 0 && min == max)
            {
                List<ClientCard> discardList = new List<ClientCard>();
                List<int> graveEffectIdList = new List<int> { CardId.AlbionTheShroudedDragon, CardId.BrandedRetribution, CardId.BrandedInHighSpirits,
                        CardId.BrightestBlazingBrandedKing, CardId.DespianTragedy };
                discardList.AddRange(ShuffleList(cards.Where(c => c.IsCode(graveEffectIdList)).ToList()));
                List<ClientCard> remainHandList = cards.Except(discardList).ToList();

                HashSet<int> seenIds = new HashSet<int>();
                for (int idx = remainHandList.Count - 1; idx >= 0; idx --)
                {
                    ClientCard currentCard = remainHandList[idx];
                    if (!seenIds.Add(currentCard.Id))
                    {
                        discardList.Add(currentCard);
                        remainHandList.Remove(currentCard);
                    }
                }

                List<int> improperCardIdList = new List<int>
                {
                    CardId.FusionDeployment, CardId.BrandedBeast, CardId.AlbionTheShroudedDragon, CardId.BrandedOpening, CardId.GoldSarcophagus,
                    CardId.FoolishBurial, CardId.FallenOfAlbaz, CardId.BrandedInRed, _CardId.InfiniteImpermanence, CardId.TheBystialLubellion,
                    CardId.BrandedLost, CardId.SpringansKitt, CardId.GuidingQuemTheVirtuous, _CardId.CrossoutDesignator, _CardId.CalledByTheGrave,
                    CardId.TriBrigadeMercourier, _CardId.AshBlossom, _CardId.MaxxC
                };
                foreach (int id in improperCardIdList)
                {
                    ClientCard target = remainHandList.FirstOrDefault(c => c.IsCode(id));
                    if (target != null)
                    {
                        discardList.Add(target);
                    }
                }
                if (discardList.Count > min)
                {
                    discardList = discardList.Take(min).ToList();
                }

                return Util.CheckSelectCount(discardList, cards, min, max);
            }

            // for The Bystial Lubellion
            if (theBystialLubellionSelecting)
            {
                theBystialLubellionSelecting = false;
                ClientCard target = TheBystialLubellionSpSummonCost(cards);
                if (target != null)
                {
                    return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                } else
                {
                    List<ClientCard> targetList = new List<ClientCard>(cards);
                    targetList.Sort(CardContainer.CompareCardAttack);
                    return Util.CheckSelectCount(targetList, cards, min, max);
                }
            }

            // for shrouded/saronir
            if (albionTheShroudedDragonSelecting || (currentSolvingChain != null && currentSolvingChain.IsCode(CardId.BystialSaronir)))
            {
                // send retribution first
                ClientCard retribution = cards.FirstOrDefault(c => c.IsCode(CardId.BrandedRetribution));
                if (retribution != null)
                {
                    if (retribution.Location == CardLocation.Deck || Bot.GetGraveyardMonsters().Where(c => c.IsCode(albazFusionMonster)).Count() < 2)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { retribution }, cards, min, max);
                    }
                }
                // send spells to recycle
                if (Bot.HasInGraveyard(CardId.BrandedRetribution) || (Bot.HasInGraveyard(CardId.DespianTragedy) && !activatedCardIdList.Contains(CardId.DespianTragedy)))
                {
                    Dictionary<int, Func<bool>> deckCheckDict = new Dictionary<int, Func<bool>>{
                            {CardId.BrandedFusion, () => BrandedFusionActivateCheck()},
                            {CardId.BrandedLost, () => {
                                if (Duel.Player == 0 && Duel.Phase >= DuelPhase.End) return false;
                                if (Bot.HasInHandOrInSpellZone(CardId.BrandedFusion) && BrandedFusionActivateCheck()) return true;
                                if (Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) && BrandedInWhiteActivateCheck()) return true;
                                if (Bot.HasInHandOrInSpellZone(CardId.BrandedInRed) && BrandedInRedActivateCheck() != null) return true;
                                if (!summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion()) return true;
                                if ((Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) || (!summoned && Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous)))) return true;
                                return false;
                            } },
                            {CardId.BrandedInHighSpirits, BrandedInHighSpiritsActivateCheck},
                            {CardId.BrandedInRed, () => BrandedInRedActivateCheck() != null },
                            {CardId.BrandedInWhite, BrandedInWhiteActivateCheck },
                            {CardId.BrandedRetribution, () => cards.Any(c => c.IsCode(CardId.BrandedRetribution) && c.Location == CardLocation.Removed) },
                            {CardId.BrightestBlazingBrandedKing, () => Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(albazFusionMonster)) },
                            {CardId.BrandedOpening, () => Bot.Hand.Count > 2 }
                        };
                    foreach (KeyValuePair<int, Func<bool>> pair in deckCheckDict)
                    {
                        ClientCard target = cards.FirstOrDefault(card => card.Location == CardLocation.Deck && card.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }
                }

                // for abyss dragon
                if (albionTheShroudedDragonSelecting &&
                    FallenOfAlbazSetCheck() && (summoned || !Bot.HasInHand(new List<int> { CardId.FallenOfAlbaz, CardId.BrandedInHighSpirits })))
                {
                    List<int> checkIdList = new List<int> {
                            CardId.BrandedRetribution, CardId.BrandedInHighSpirits, CardId.BrightestBlazingBrandedKing, CardId.BrandedInWhite, CardId.BrandedOpening,
                            CardId.BrandedInRed, CardId.BrandedBeast, CardId.BrandedLost
                        };
                    if (!BrandedFusionActivateCheck())
                    {
                        checkIdList.Add(CardId.BrandedFusion);
                    }
                    foreach (int checkId in checkIdList)
                    {
                        ClientCard target = cards.FirstOrDefault(c => c.Id == checkId);
                        if (target != null)
                        {
                            return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                        }
                    }
                }

                // send from deck
                List<int> checkList = new List<int> { CardId.BrandedInHighSpirits, CardId.BrandedOpening, CardId.BrightestBlazingBrandedKing, CardId.BrandedBeast, CardId.BrandedLost };
                foreach (int checkId in checkList)
                {
                    ClientCard target = cards.FirstOrDefault(c => c.IsCode(checkId) && c.Location == CardLocation.Deck);
                    if (target != null)
                    {
                        return Util.CheckSelectCount(new List<ClientCard> { target }, cards, min, max);
                    }
                }
                return Util.CheckSelectCount(ShuffleList(new List<ClientCard>(cards)), cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectOption(IList<int> options)
        {
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            if (currentSolvingChain != null)
            {
                // 1190=Add to Hand, 1152=Special Summon
                if (options.Count == 2 && options.Contains(1190) && options.Contains(1152))
                {
                    if (currentSolvingChain.IsCode(CardId.BrandedOpening))
                    {
                        return (CheckShouldNoMoreSpSummon() && !summoned && Duel.Player == 0) ? options.IndexOf(1190) : options.IndexOf(1152);
                    }

                    if (fusionTarget != null && (
                        currentSolvingChain.IsCode(CardId.DespianQuaeritis)
                        || currentSolvingChain.IsCode(CardId.TitanikladTheAshDragon)
                        || currentSolvingChain.IsCode(CardId.SprindTheIrondashDragon)
                        ))
                    {
                        if (fusionTarget.IsCode(CardId.FallenOfAlbaz))
                        {
                            return CheckAlbazFusion() ? options.IndexOf(1152) : options.IndexOf(1190);
                        }
                        if (fusionTarget.IsCode(CardId.GuidingQuemTheVirtuous, CardId.SpringansKitt))
                        {
                            return CheckShouldNoMoreSpSummon() ? options.IndexOf(1190) : options.IndexOf(1152);
                        }
                        if (fusionTarget.IsCode(CardId.AluberTheJesterOfDespia))
                        {
                            return activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) ? options.IndexOf(1190) : options.IndexOf(1152);
                        }
                        return (CheckShouldNoMoreSpSummon() && !summoned) ? options.IndexOf(1190) : options.IndexOf(1152);
                    }
                }

                // 1190=Add to Hand, 1153=Set
                if (currentSolvingChain.IsCode(CardId.AlbionTheBrandedDragon) && fusionTarget != null)
                {
                    if (fusionTarget.IsOriginalCode(CardId.BrandedInHighSpirits) && Duel.Player == 0)
                    {
                        return BrandedInHighSpiritsActivateCheck() ? options.IndexOf(1190) : options.IndexOf(1153);
                    }
                    if (fusionTarget.IsOriginalCode(CardId.BrandedInRed) && Duel.Player == 0)
                    {
                        if (nadirActivated) return options.IndexOf(1153);
                        return BrandedInRedActivateCheck() != null ? options.IndexOf(1190) : options.IndexOf(1153);
                    }
                    if (fusionTarget.Data != null)
                    {
                        bool setFlag = fusionTarget.Data.HasType(CardType.Trap);
                        setFlag |= fusionTarget.Data.HasType(CardType.QuickPlay) && Duel.Player == 0;
                        setFlag |= Bot.Hand.Count >= 6 && Duel.Player == 0;
                        return setFlag ? options.IndexOf(1153) : options.IndexOf(1190);
                    }
                }
            }

            return base.OnSelectOption(options);
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            ChainInfo currentSovingChain = Duel.GetCurrentSolvingChainInfo();
            if (currentSovingChain != null && currentSovingChain.ActivatePlayer == 0 && currentSovingChain.IsCode(CardId.SprindTheIrondashDragon))
            {
                return SprindTheIrondashDragonMoveZone(available, null);
            }

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
            if (desc == Util.GetStringId(CardId.BrandedInHighSpirits, 2))
            {
                if (CheckWhetherWillbeRemoved()) return false;
                if (fusionTarget != null && fusionTarget.IsOriginalCode(CardId.TriBrigadeMercourier))
                {
                    return !Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(albazFusionMonster));
                }
            }

            if (desc == Util.GetStringId(CardId.RindbrummTheStrikingDragon, 2))
            {
                bool checkFlag = Enemy.MonsterZone.Any(c => c != null);
                checkFlag |= Bot.MonsterZone.Any(c => c != null && (c.IsOriginalCode(CardId.AluberTheJesterOfDespia) || c.IsOriginalCode(CardId.SpringansKitt)));
                return checkFlag;
            }

            if (desc == Util.GetStringId(CardId.DespianLuluwalilith, 2))
            {
                bool checkFlag = Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled());
                checkFlag |= Enemy.SpellZone.Any(c => c != null && c.IsFaceup() && !c.IsDisabled());
                return checkFlag;
            }

            if (desc == Util.GetStringId(CardId.BrandedInRed, 0))
            {
                // fix material list
                brandedInRedMaterialList = brandedInRedMaterialList.Where(c => c != null && (c.Location == CardLocation.MonsterZone || c.Location == CardLocation.Hand)).ToList();

                List<ClientCard> materialList = Bot.MonsterZone.Where(c => c != null && c.Attack <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList)).ToList();
                materialList.AddRange(Bot.Hand.Where(c => c.IsMonster()
                    && !(CheckWhetherCanSummon() &&
                        ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                        || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                        )
                    )
                );

                BrandedInRedFusionCheck(Bot.ExtraDeck, 0,
                    materialList, brandedInRedMaterialList,
                    out ClientCard _fusionTarget, out _);

                return _fusionTarget != null;
            }

            if (desc == Util.GetStringId(CardId.SprindTheIrondashDragon, 2))
            {
                ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
                if (currentSolvingChain != null)
                {
                    int value = SprindTheIrondashDragonDestroyValue(currentSolvingChain.Sequence);
                    return value > 0;
                }
            }

            return base.OnSelectYesNo(desc);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null && currentSolvingChain.IsCode(CardId.AlbionTheSanctifireDragon))
            {
                sanctifireSelectPositionCount++;
                if (sanctifireSelectPositionCount >= 2)
                {
                    if (Duel.Phase <= DuelPhase.Main2)
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
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
            }

            summoned = false;
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            enemyActivateInfiniteImpermanenceFromHand = false;
            nadirActivated = false;
            fusionToGYFlag = false;
            spSummoningAlbaz = false;
            cartesiaSummonGoal = 0;
            sanctifireSelectPositionCount = 0;
            quemSummonFlag = 0;
            if (dimensionShifterCount > 0) dimensionShifterCount--;
            cartesiaMaterialList.Clear();
            brandedInRedMaterialList.Clear();
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            sendToGYThisTurn.Clear();
            activatedCardIdList.Clear();
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
            base.OnChaining(player, card);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ChainInfo currentCard = Duel.GetCurrentSolvingChainInfo();
            if (currentCard != null)
            {
                // if activation is negated, it can activate again.
                if (currentCard.ActivatePlayer == 0)
                {
                    List<int> activateCheck = new List<int> { CardId.NadirServant, CardId.FusionDeployment, CardId.BrandedFusion, CardId.BrandedInRed };
                    if (currentCard.IsCode(activateCheck))
                    {
                        activatedCardIdList.Add(currentCard.ActivateId);
                    }
                }
                if (!Duel.IsCurrentSolvingChainNegated())
                {
                    if (currentCard.ActivatePlayer == 1)
                    {
                        if (currentCard.IsCode(_CardId.MaxxC))
                            enemyActivateMaxxC = true;
                        if (currentCard.IsCode(_CardId.LockBird))
                            enemyActivateLockBird = true;
                        if (currentCard.IsCode(CardId.DimensionShifter))
                            dimensionShifterCount = 2;
                    }
                    if (currentCard.ActivatePlayer == 0 && currentCard.IsCode(CardId.NadirServant))
                    {
                        nadirActivated = true;
                    }
                }
            }
            fusionTarget = null;
            selectedFusionMaterial.Clear();
            sanctifireSelectPositionCount = 0;

            base.OnChainSolved(chainIndex);
        }

        public override void OnChainEnd()
        {
            cartesiaSummonGoal = 0;
            cartesiaMaterialList.Clear();
            brandedInRedMaterialList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyActivateInfiniteImpermanenceFromHand = false;
            theBystialLubellionSelecting = false;
            albionTheShroudedDragonSelecting = false;
            spSummoningAlbaz = false;
            for (int idx = enemyPlaceThisTurn.Count - 1; idx >= 0; idx--)
            {
                ClientCard checkTarget = enemyPlaceThisTurn[idx];
                if (checkTarget == null || (checkTarget.Location != CardLocation.SpellZone && checkTarget.Location != CardLocation.MonsterZone))
                {
                    enemyPlaceThisTurn.RemoveAt(idx);
                }
            }
            if (quemSummonFlag > 0) quemSummonFlag--;
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
                if (currentControler == 0)
                {
                    ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
                    if (previousLocation == (int)CardLocation.Grave && currentLocation != (int)CardLocation.Grave)
                    {
                        sendToGYThisTurn.Remove(card);
                    }
                    if (currentLocation == (int)CardLocation.Grave)
                    {
                        if (card.HasType(CardType.Fusion)) fusionToGYFlag = true;
                        sendToGYThisTurn.Add(card);
                    }
                    if (currentLocation == (int)CardLocation.MonsterZone && card != null && card.IsCode(CardId.GuidingQuemTheVirtuous))
                    {
                        quemSummonFlag = 2;
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

        public bool TheBystialLubellionSpSummon()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return false;
            }
            ClientCard costTarget = TheBystialLubellionSpSummonCost(Bot.GetMonsters());
            if (costTarget != null)
            {
                theBystialLubellionSelecting = true;
                activatedCardIdList.Add(Card.Id - 1);
                return true;
            }
            return false;
        }

        public ClientCard TheBystialLubellionSpSummonCost(IList<ClientCard> costList)
        {
            Dictionary<int, Func<ClientCard, bool>> checkDict = new Dictionary<int, Func<ClientCard, bool>>{
                {CardId.AlbionTheBrandedDragon, (card) => sendToGYThisTurn.All(c => !c.IsCode(CardId.AlbionTheBrandedDragon))},
                {CardId.BystialSaronir, (card) => !activatedCardIdList.Contains(CardId.BystialSaronir + 1) && !CheckWhetherWillbeRemoved() },
                {CardId.TitanikladTheAshDragon, (card) => Util.IsTurn1OrMain2() || card.GetDefensePower() < 2500 },
                {CardId.AlbaLenatusTheAbyssDragon, (card) => Util.IsTurn1OrMain2() || card.IsDisabled() || card.GetDefensePower() < 2500 },
                {CardId.AlbionTheShroudedDragon, (card) => Util.IsTurn1OrMain2() || card.GetDefensePower() < 2500 },
                {CardId.BorreloadFuriousDragon, (card) => card.IsDisabled() && CheckRemainInDeck(CardId.BrandedBeast, CardId.BrandedLost) > 0 },
            };

            foreach (KeyValuePair<int, Func<ClientCard, bool>> pair in checkDict)
            {
                List<ClientCard> targetList = costList.Where(card => card.IsCode(pair.Key)).ToList();
                foreach (ClientCard target in targetList)
                {
                    if (target != null && pair.Value(target))
                    {
                        return target;
                    }
                }
            }
            return null;
        }

        public bool TheBystialLubellionActivate()
        {
            if (CheckWhetherNegated(true, Card.Location == CardLocation.MonsterZone, CardType.Monster)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                activatedCardIdList.Add(Card.Id);
            } else
            {
                activatedCardIdList.Add(Card.Id + 1);
            }
            return true;
        }

        public bool AlbionTheShroudedDragonActivate()
        {
            if (CheckWhetherNegated(true, false, CardType.Monster) || CheckWhetherWillbeRemoved()) return false;
            bool checkFlag = CheckRemainInDeck(CardId.BrandedRetribution, CardId.BrandedOpening, CardId.BrightestBlazingBrandedKing, CardId.BrandedInHighSpirits) > 0;
            if (Bot.HasInGraveyard(CardId.BrandedRetribution))
            {
                checkFlag |= CheckRemainInDeck(CardId.BrandedFusion, CardId.BrandedBeast, CardId.BrandedInRed, CardId.BrandedInWhite, CardId.BrandedLost) > 0;
            }
            if (Bot.HasInSpellZone(CardId.BrandedBeast))
            {
                checkFlag |= CheckRemainInDeck(CardId.BrandedLost) > 0;
            }
            if (Card.Location == CardLocation.Grave)
            {
                checkFlag |= CheckRemainInDeck(CardId.BrandedInWhite) > 0;
            }
            // for abyss dragon
            if (FallenOfAlbazSetCheck() && (summoned || !Bot.HasInHand(new List<int> { CardId.FallenOfAlbaz, CardId.BrandedInHighSpirits })))
            {
                checkFlag |= Bot.HasInHand(new List<int> { CardId.BrandedBeast, CardId.BrandedInHighSpirits, CardId.BrandedInWhite, CardId.BrandedInRed, CardId.BrandedLost, CardId.BrandedOpening, CardId.BrandedRetribution, CardId.BrightestBlazingBrandedKing });
            }

            if (checkFlag)
            {
                activatedCardIdList.Add(Card.Id);
                albionTheShroudedDragonSelecting = true;
                return true;
            }
            return false;
        }

        public bool BystialSaronirActivate()
        {
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
            // banish & spsummon
            if (Card.Location == CardLocation.Hand)
            {
                // banish enemy target
                if (Util.GetLastChainCard() != null && Duel.LastChainPlayer == 1)
                {
                    List<ClientCard> chainTargetList = Duel.LastChainTargets.Where(c => CheckBystialCanBanish(c)).ToList();
                    if (chainTargetList.Count > 0)
                    {
                        AI.SelectCard(chainTargetList);
                        currentDestroyCardList.Add(chainTargetList[0]);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                List<ClientCard> enemyChainList = Duel.CurrentChain.Where(c => c != null && c.Controller == 1 && CheckBystialCanBanish(c) && !currentDestroyCardList.Contains(c))
                    .OrderByDescending(c => c.GetDefensePower()).ToList();
                if (enemyChainList.Count > 0)
                {
                    AI.SelectCard(enemyChainList);
                    currentDestroyCardList.Add(enemyChainList[0]);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }

                // banish cards with effect
                if (!CheckShouldNoMoreSpSummon())
                {
                    ClientCard mercourier = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.TriBrigadeMercourier));
                    if (mercourier != null && !activatedCardIdList.Contains(CardId.TriBrigadeMercourier + 1))
                    {
                        AI.SelectCard(mercourier);
                        currentDestroyCardList.Add(mercourier);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                    ClientCard tragedy = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.DespianTragedy));
                    if (tragedy != null && !activatedCardIdList.Contains(CardId.DespianTragedy))
                    {
                        AI.SelectCard(tragedy);
                        currentDestroyCardList.Add(tragedy);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                    if (Bot.HasInGraveyard(CardId.TheBystialLubellion) && !activatedCardIdList.Contains(CardId.TheBystialLubellion)
                        && Duel.Player == 0 && CheckRemainInDeck(CardId.BrandedLost, CardId.BrandedBeast) > 0
                        && CurrentTiming == -1)
                    {
                        List<ClientCard> targetList = Enemy.Graveyard.Where(c => c != null && CheckBystialCanBanish(c)).OrderByDescending(card => card.Attack).ToList();
                        targetList.AddRange(Bot.Graveyard
                            .Where(c => c != null && CheckBystialCanBanish(c) && !c.IsCode(CardId.TheBystialLubellion) && !CheckWhetherShouldKeepInGrave(c))
                            .OrderBy(card => card.Attack).ToList()
                            );
                        if (targetList.Count > 0)
                        {
                            AI.SelectCard(targetList);
                            currentDestroyCardList.Add(targetList[0]);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }

                // defense
                if (Bot.UnderAttack && Bot.BattlingMonster == null)
                {
                    List<ClientCard> targetList = Enemy.Graveyard.Where(c => CheckBystialCanBanish(c)).OrderByDescending(c => c.GetDefensePower()).ToList();
                    targetList.AddRange(Bot.Graveyard.Where(c => CheckBystialCanBanish(c)).OrderBy(c => c.GetDefensePower()));
                    if (targetList.Count > 0)
                    {
                        AI.SelectCard(targetList);
                        currentDestroyCardList.Add(targetList[0]);
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }

                // trigger beast
                if (Duel.Player == 1 && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                    && !activatedCardIdList.Contains(CardId.BrandedBeast) && Bot.SpellZone.Any(c => c != null && c.IsCode(CardId.BrandedBeast) && (c.IsFacedown() || !c.IsDisabled()))
                    && !Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeBystial)))
                {
                    List<ClientCard> dangerList = GetProblematicEnemyCardList(true, false, CardType.Trap);
                    if (dangerList.Count > 0)
                    {
                        List<ClientCard> targetList = Enemy.Graveyard.Where(c => CheckBystialCanBanish(c)).OrderByDescending(c => c.GetDefensePower()).ToList();
                        targetList.AddRange(Bot.Graveyard.Where(c => CheckBystialCanBanish(c)).OrderBy(c => c.GetDefensePower()));
                        if (targetList.Count > 0)
                        {
                            AI.SelectCard(targetList);
                            currentDestroyCardList.Add(targetList[0]);
                            activatedCardIdList.Add(Card.Id);
                            return true;
                        }
                    }
                }

            }
            // send to GY
            if (Card.Location == CardLocation.Grave && !CheckWhetherWillbeRemoved())
            {
                if (Bot.HasInGraveyard(CardId.BrandedRetribution))
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                } else if (CheckRemainInDeck(CardId.TheBystialLubellion, CardId.BrandedRetribution, CardId.BrandedInHighSpirits, CardId.BrightestBlazingBrandedKing, CardId.BrandedOpening) > 0)
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            }
            return false;
        }

        public bool CheckBystialCanBanish(ClientCard c)
        {
            return c != null && c.Location == CardLocation.Grave && c.IsMonster() && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark);
        }

        public bool AluberTheJesterOfDespiaSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || enemyActivateLockBird || activatedCardIdList.Contains(Card.Id)) return false;
            summoned = true;
            return true;
        }

        public bool AluberTheJesterOfDespiaActivate()
        {
            // search
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            // spsummon
            else
            {
                List<ClientCard> targetCardList = GetMonsterListForTargetNegate(true, CardType.Monster);
                ClientCard lastChainCard = Util.GetLastChainCard();
                // chain to protect
                if (lastChainCard != null && lastChainCard.Controller == 0)
                {
                    AI.SelectCard(targetCardList);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                AI.SelectCard(targetCardList);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
        }

        public bool FallenOfAlbazSummon()
        {
            if (CheckAlbazFusion(Card))
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool FallenOfAlbazSet()
        {
            if (FallenOfAlbazSetCheck())
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool FallenOfAlbazSetCheck()
        {
            if (!Bot.HasInExtra(CardId.AlbaLenatusTheAbyssDragon) || nadirActivated) return false;
            // check dangerous dragon
            if (!Bot.HasInSpellZone(CardId.BrandedLost, true, true) || Bot.GetHandCount() < 2)
            {
                foreach (int dangerId in dangerousDragonIdList)
                {
                    if (Enemy.HasInMonstersZone(dangerId, true, false, true))
                    {
                        return true;
                    }
                }
            }
            // check dragon count
            int dragonCount = Enemy.GetMonsters().Where(c => c != null && c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasRace(CardRace.Dragon)).Count();
            if (dragonCount > 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether should call albaz.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool CheckAlbazFusion(ClientCard exceptCost = null) {
            return CheckAlbazFusion(exceptCost, out _);
        }

        public bool CheckAlbazFusion(ClientCard exceptCost, out List<ClientCard> enemyMonsterList)
        {
            enemyMonsterList = null;
            int costHandCount = Bot.Hand.Where(c => c != exceptCost).Count();
            if (costHandCount <= 0 || Enemy.GetMonsterCount() == 0) return false;
            if (CheckWhetherNegated(true, true, CardType.Monster) || activatedCardIdList.Contains(CardId.FallenOfAlbaz) || nadirActivated) return false;
            if (!Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon)
                && Bot.HasInExtra(CardId.MirrorjadeTheIcebladeDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.AlbaLenatusTheAbyssDragon))
            {
                List<ClientCard> targetList = Enemy.GetMonsters().Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasRace(CardRace.Dragon))
                    .OrderByDescending(c => c.GetDefensePower()).ToList();
                if (targetList.Count > 0)
                {
                    enemyMonsterList = targetList;
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.AlbionTheBrandedDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.AlbionTheSanctifireDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light) && c.HasRace(CardRace.SpellCaster))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.LubellionTheSearingDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (costHandCount >= 2 && target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.BorreloadFuriousDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.TitanikladTheAshDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.Attack >= 2500)
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.RindbrummTheStrikingDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasRace(CardRace.Beast | CardRace.BeastWarrior | CardRace.WindBeast))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }
            if (Bot.HasInExtra(CardId.SprindTheIrondashDragon))
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c != null && c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && enemyPlaceThisTurn.Contains(c) && c.IsSpecialSummoned
                    && c.GetDefensePower() >= Util.GetBestPower(Bot))
                    .OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                if (target != null)
                {
                    enemyMonsterList = new List<ClientCard> { target };
                    return true;
                }
            }

            return false;
        }

        public bool FallenOfAlbazActivate()
        {
            if (Bot.HasInExtra(CardId.AlbaLenatusTheAbyssDragon) && Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(dangerousDragonIdList))) return false;
            if (CheckAlbazFusion())
            {
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool SpringansKittSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || enemyActivateLockBird || activatedCardIdList.Contains(Card.Id + 1)) return false;
            summoned = true;
            return true;
        }

        public bool SpringansKittActivate()
        {
            // spsummon
            if (Card.Location == CardLocation.Hand)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                if (CheckShouldNoMoreSpSummon())
                {
                    bool skipFlag = !summoned;
                    skipFlag |= activatedCardIdList.Contains(CardId.BrandedFusion);
                    skipFlag |= Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
                    if (skipFlag)
                    {
                        return false;
                    }
                }
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            // search
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public bool GuidingQuemTheVirtuousSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || CheckWhetherWillbeRemoved()) return false;
            if (activatedCardIdList.Contains(Card.Id)) return false;
            summoned = true;
            return true;
        }

        public bool GuidingQuemTheVirtuousSummonForSearch()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster) || CheckWhetherWillbeRemoved()) return false;
            if (activatedCardIdList.Contains(Card.Id)) return false;
            if (Bot.HasInGraveyard(CardId.BrandedRetribution) && CheckRemainInDeck(CardId.BrandedFusion, CardId.BrandedLost, CardId.BrandedInWhite, CardId.BrandedInRed) > 0)
            {
                summoned = true;
                return true;
            }
            if (Bot.HasInGraveyard(new[] { CardId.BrandedFusion, CardId.BrandedLost, CardId.BrandedBeast }) && CheckRemainInDeck(CardId.BrandedRetribution) > 0)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool GuidingQuemTheVirtuousActivate()
        {
            int desc = -1;
            if (ActivateDescription >= Util.GetStringId(CardId.GuidingQuemTheVirtuous, 0))
            {
                desc = ActivateDescription - Util.GetStringId(CardId.GuidingQuemTheVirtuous, 0);
            }
            Logger.DebugWriteLine("Guiding desc: " + desc.ToString());
            Logger.DebugWriteLine("Guiding timing: " + CurrentTiming.ToString());
            Logger.DebugWriteLine("Guiding flag: " + quemSummonFlag.ToString());

            // spsummon
            if ((ActivateDescription == -1 && quemSummonFlag == 0) || ActivateDescription == Util.GetStringId(CardId.GuidingQuemTheVirtuous, 1))
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                List<KeyValuePair<int, Func<ClientCard, bool>>> checkList = new List<KeyValuePair<int, Func<ClientCard, bool>>>
                {
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.AlbionTheSanctifireDragon, (c) => c.IsCanRevive() && !activatedCardIdList.Contains(CardId.AlbionTheSanctifireDragon) ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.MirrorjadeTheIcebladeDragon, (c) => c.IsCanRevive() ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.FallenOfAlbaz, (c) =>
                    {
                        bool albazCanFusionFlag = CheckAlbazFusion(null, out List<ClientCard> materialList);
                        if (albazCanFusionFlag && !Util.ChainContainsCard(new[] {CardId.AlbionTheBrandedDragon, CardId.LubellionTheSearingDragon}) && !spSummoningAlbaz)
                        {
                            bool albazFlag = materialList.Count > 1;
                            if (materialList.Count > 0)
                            {
                                ClientCard material = materialList[0];
                                albazFlag |= material.HasType(CardType.Ritual | CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
                                albazFlag |= material.IsFloodgate() || material.IsOneForXyz() || Util.GetWorstBotMonster().GetDefensePower() < material.Attack;
                            }
                            return albazFlag;
                        }
                        return false;
                    } ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.AlbionTheBrandedDragon, (c) => c.IsCanRevive() && Bot.HasInSpellZone(CardId.BrandedBeast)
                        && Bot.MonsterZone.Any(oc => oc != null && oc.IsFaceup() && oc.HasSetcode(SetcodeBystial)) ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.BlazingCartesiaTheVirtuous, (c) => Duel.Player == 0 || !activatedCardIdList.Contains(CardId.BlazingCartesiaTheVirtuous + 1) ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.TriBrigadeMercourier, (c) => Bot.MonsterZone.Any(oc => oc != null && oc.IsFaceup() && oc.IsCode(albazFusionMonster)) ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.AlbionTheSanctifireDragon, (c) => c.IsCanRevive() ),
                    new KeyValuePair<int, Func<ClientCard, bool>>(CardId.SpringansKitt, (c) => true )
                };
                foreach (KeyValuePair<int, Func<ClientCard, bool>> pair in checkList)
                {
                    ClientCard target = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsOriginalCode(pair.Key) && pair.Value(c));
                    if (target != null)
                    {
                        if (target.IsOriginalCode(CardId.FallenOfAlbaz))
                        {
                            spSummoningAlbaz = true;
                        }
                        AI.SelectCard(target);
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }
            }
            // send to GY
            if ((ActivateDescription == -1 && quemSummonFlag > 0) || ActivateDescription == Util.GetStringId(CardId.GuidingQuemTheVirtuous, 0))
            {
                if (CheckWhetherNegated(true, true, CardType.Monster) || CheckWhetherWillbeRemoved()) return false;
                quemSummonFlag = 0;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool BlazingCartesiaTheVirtuousSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            bool checkFlag = Bot.HasInHandOrInSpellZone(CardId.BrandedOpening) && !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && CheckRemainInDeck(CardId.AluberTheJesterOfDespia) > 0;
            checkFlag |= Bot.HasInHand(CardId.AlbionTheShroudedDragon) && !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon);
            checkFlag |= Bot.HasInHandOrHasInMonstersZone(CardId.BystialSaronir) && !activatedCardIdList.Contains(CardId.BystialSaronir + 1);
            if (Bot.HasInExtra(CardId.GranguignolTheDuskDragon))
            {
                bool hasMaterial = Bot.Hand.Any(c => c != Card && c.Attack < 2000 && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark));
                hasMaterial |= Bot.MonsterZone.Any(c => c != null && !c.IsCode(cannotBeFusionMaterialIdList) && c.Attack < 2000 && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark));
                checkFlag |= hasMaterial;
            }

            if (checkFlag)
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool BlazingCartesiaTheVirtuousActivate()
        {
            // sp summon
            if (Card.Location == CardLocation.Hand)
            {
                if (CheckShouldNoMoreSpSummon() || CheckWhetherNegated(true, true, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            // fusion
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                if (Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(CardId.BrandedInRed))) return false;
                List<ClientCard> materialList = Bot.MonsterZone.Where(c => c != null && c.Attack <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList)).ToList();
                materialList.AddRange(Bot.Hand.Where(c => c.IsMonster()
                    && !(CheckWhetherCanSummon() &&
                        ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                        || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                        )
                    )
                );

                // escape target
                ClientCard lastCahinCard = Util.GetLastChainCard();
                if (lastCahinCard != null && Duel.LastChainPlayer == 1)
                {
                    List<ClientCard> chainTargetList = Duel.LastChainTargets.Where(c => c.Controller == 0 && c.Location == CardLocation.MonsterZone
                            && (!c.IsCode(cannotBeFusionMaterialIdList) || c.Attack <= 2500)).ToList();
                    if (chainTargetList.Count > 0)
                    {
                        if (lastCahinCard.IsCode(targetNegateIdList))
                        {
                            chainTargetList = chainTargetList.Where(c => c.Attack <= 2500).ToList();
                        }
                        BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, 0, materialList, chainTargetList,
                            out ClientCard _fusionTarget, out List<ClientCard> usedMaterialList);

                        if (_fusionTarget != null)
                        {
                            Logger.DebugWriteLine("cartesia prepare fusion1: " + _fusionTarget.Name);
                            cartesiaMaterialList.AddRange(usedMaterialList.Intersect(chainTargetList));
                            activatedCardIdList.Add(Card.Id + 1);
                            return true;
                        }
                    }
                }

                // fusion shrouded
                bool skipShroudedFlag = CheckWhetherCanSummon() && !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && Bot.HasInHand(CardId.AluberTheJesterOfDespia);
                skipShroudedFlag |= CheckWhetherCanSummon() && !activatedCardIdList.Contains(CardId.SpringansKitt) && Bot.HasInHand(CardId.SpringansKitt);
                if (!skipShroudedFlag)
                {
                    ClientCard shrouded = Duel.CurrentChain.FirstOrDefault(c => c.Controller == 0 && c.Location == CardLocation.Hand && c.IsOriginalCode(CardId.AlbionTheShroudedDragon));
                    if (shrouded != null)
                    {
                        BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, 0, materialList, new List<ClientCard> { shrouded },
                            out ClientCard _fusionTarget, out List<ClientCard> usedMaterialList);

                        if (_fusionTarget != null)
                        {
                            Logger.DebugWriteLine("cartesia prepare fusion2: " + _fusionTarget.Name);
                            cartesiaMaterialList.AddRange(usedMaterialList.Intersect(new List<ClientCard> { shrouded }));
                            activatedCardIdList.Add(Card.Id + 1);
                            return true;
                        }
                    }
                }

                bool shouldActivateFlag = Duel.Player == 0 && !CheckShouldNoMoreSpSummon() || Duel.Player == 1;

                // summon mirrorjade
                bool checkMirrorJadeFlag = !(Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) || Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true))
                    && shouldActivateFlag;
                if (checkMirrorJadeFlag)
                {
                    BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, CardId.MirrorjadeTheIcebladeDragon, materialList, null,
                        out ClientCard _fusionTarget, out _);

                    if (_fusionTarget != null)
                    {
                        Logger.DebugWriteLine("cartesia prepare fusion3: " + _fusionTarget.Name);
                        cartesiaSummonGoal = CardId.MirrorjadeTheIcebladeDragon;
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }

                // summon dusk dragon
                if (shouldActivateFlag && Duel.Player == 0)
                {
                    Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                    {
                        {CardId.BystialSaronir, () => !activatedCardIdList.Contains(CardId.BystialSaronir + 1) && !DefaultCheckWhetherCardIdIsNegated(CardId.BystialSaronir) },
                        {CardId.DespianTragedy, () => !activatedCardIdList.Contains(CardId.DespianTragedy) && !DefaultCheckWhetherCardIdIsNegated(CardId.DespianTragedy) }
                    };
                    foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                    {
                        ClientCard targetMaterial = materialList.FirstOrDefault(c => c.IsCode(pair.Key));
                        if (targetMaterial != null && pair.Value())
                        {
                            BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, CardId.GranguignolTheDuskDragon, materialList, new List<ClientCard> { targetMaterial },
                                out ClientCard _fusionTarget, out List<ClientCard> usedMaterialList);
                            if (_fusionTarget != null)
                            {
                                Logger.DebugWriteLine("cartesia prepare fusion4: " + _fusionTarget.Name);
                                cartesiaSummonGoal = CardId.GranguignolTheDuskDragon;
                                cartesiaMaterialList.Add(targetMaterial);
                                activatedCardIdList.Add(Card.Id + 1);
                                return true;
                            }
                        }
                    }
                }

                // summon sanctifire
                if (shouldActivateFlag)
                {
                    BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, CardId.AlbionTheSanctifireDragon, materialList, new List<ClientCard> { Card},
                        out ClientCard _fusionTarget, out List<ClientCard> usedMaterialList);
                    if (_fusionTarget != null)
                    {
                        Logger.DebugWriteLine("cartesia prepare fusion5: " + _fusionTarget.Name);
                        cartesiaSummonGoal = CardId.AlbionTheSanctifireDragon;
                        cartesiaMaterialList.Add(Card);
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }

                if (shouldActivateFlag && GetProblematicEnemyMonster(0, true, true, CardType.Monster) != null)
                {
                    BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, CardId.BorreloadFuriousDragon, materialList, null,
                        out ClientCard _fusionTarget, out _);

                    if (_fusionTarget != null)
                    {
                        Logger.DebugWriteLine("cartesia prepare fusion6: " + _fusionTarget.Name);
                        cartesiaSummonGoal = CardId.BorreloadFuriousDragon;
                        activatedCardIdList.Add(Card.Id + 1);
                        return true;
                    }
                }

                if (shouldActivateFlag)
                {
                    bool checkFlag = Duel.Player == 0 && CurrentTiming == -1;
                    checkFlag |= Duel.Player == 1 && (CurrentTiming & hintTimingMainEnd) != 0;
                    if (checkFlag)
                    {
                        BlazingCartesiaTheVirtuousFusionCheck(Bot.ExtraDeck, 0, materialList, null,
                            out ClientCard _fusionTarget, out _);

                        if (_fusionTarget != null)
                        {
                            Logger.DebugWriteLine("cartesia prepare fusion7: " + _fusionTarget.Name);
                            activatedCardIdList.Add(Card.Id + 1);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool BlazingCartesiaTheVirtuousActivateInGrave()
        {
            // recycle
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 2);
                return true;
            }
            return false;
        }

        public void BlazingCartesiaTheVirtuousFusionCheck(
            IList<ClientCard> canSummonList, int mustSummonId,
            List<ClientCard> materialList, List<ClientCard> mustMaterialList,
            out ClientCard fusionTarget, out List<ClientCard> selectedFusionMaterialList)
        {
            fusionTarget = null;
            selectedFusionMaterialList = new List<ClientCard>();

            Dictionary<int, List<Func<ClientCard, bool>>> checkDict = new Dictionary<int, List<Func<ClientCard, bool>>>
            {
                {CardId.GranguignolTheDuskDragon, new List<Func<ClientCard, bool>>{
                    (c) => c.IsCode(CardId.BlazingCartesiaTheVirtuous),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark)
                } },
                {CardId.AlbionTheSanctifireDragon, new List<Func<ClientCard, bool>>{
                    (c) => c.IsCode(CardId.FallenOfAlbaz),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light) && c.HasRace(CardRace.SpellCaster)
                } },
                {CardId.MirrorjadeTheIcebladeDragon, new List<Func<ClientCard, bool>>
                {
                    (c) => c.IsCode(CardId.FallenOfAlbaz),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)
                } },
                {CardId.AlbionTheBrandedDragon, new List<Func<ClientCard, bool>>
                {
                    (c) => c.IsCode(CardId.FallenOfAlbaz),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light)
                } },
                {CardId.LubellionTheSearingDragon, new List<Func<ClientCard, bool>>
                {
                    (c) => c.IsCode(CardId.FallenOfAlbaz),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark)
                } },
                {CardId.DespianQuaeritis, new List<Func<ClientCard, bool>>
                {
                    (c) => c.HasSetcode(SetcodeDespain),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark)
                } },
                {CardId.BorreloadFuriousDragon, new List<Func<ClientCard, bool>>
                {
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon),
                    (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon)
                } }
            };
            Dictionary<int, Func<ClientCard, ClientCard, bool>> extraCheckDict = new Dictionary<int, Func<ClientCard, ClientCard, bool>>
            {
                {CardId.AlbionTheSanctifireDragon, (c1, c2) =>
                {
                    int reviveCount = Bot.Graveyard.Count(c => c != null && c.IsMonster() && c.IsCanRevive());
                    reviveCount += Enemy.Graveyard.Count(c => c != null && c.IsMonster() && c.IsCanRevive());
                    if (!CheckWhetherWillbeRemoved() ||
                        (CurrentTiming & hintTimingMainEnd) > 0 && Util.GetOneEnemyBetterThanValue(Card.GetDefensePower()) != null 
                            && Util.GetOneEnemyBetterThanValue(3000) == null)
                    {
                        reviveCount += 2;
                    }
                    return reviveCount >= 2;
                } },
                {CardId.LubellionTheSearingDragon, (c1, c2) => Bot.Hand.Count(c => c != c1 && c != c2) > 0 },
                {CardId.MirrorjadeTheIcebladeDragon, (c1, c2) => !CheckWhetherWillbeRemoved() &&
                    !Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) }
            };

            foreach (KeyValuePair<int, List<Func<ClientCard, bool>>> pair in checkDict)
            {
                if (mustSummonId > 0 && mustSummonId != pair.Key) continue;
                ClientCard currentFusionTarget = canSummonList.FirstOrDefault(c => c != null && c.IsCode(pair.Key));
                if (currentFusionTarget == null) continue;
                Func<ClientCard, bool> fusionFunc1 = pair.Value[0];
                Func<ClientCard, bool> fusionFunc2 = pair.Value[1];

                if (mustMaterialList != null && mustMaterialList.Count > 0)
                {
                    foreach (ClientCard mustMaterial in mustMaterialList)
                    {
                        if (!fusionFunc1(mustMaterial) && !fusionFunc2(mustMaterial)) continue;
                        foreach (ClientCard anotherMaterial in materialList)
                        {
                            if (anotherMaterial == mustMaterial) continue;
                            bool checkFlag = fusionFunc1(mustMaterial) && fusionFunc2(anotherMaterial);
                            checkFlag |= fusionFunc2(mustMaterial) && fusionFunc1(anotherMaterial);
                            extraCheckDict.TryGetValue(pair.Key, out Func<ClientCard, ClientCard, bool> extraCheckFunc);
                            checkFlag &= (extraCheckFunc == null || extraCheckFunc(mustMaterial, anotherMaterial));
                            if (checkFlag)
                            {
                                fusionTarget = currentFusionTarget;
                                selectedFusionMaterialList.Add(mustMaterial);
                                selectedFusionMaterialList.Add(anotherMaterial);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    for (int index1 = 0; index1 < materialList.Count - 1; ++index1)
                    {
                        ClientCard material1 = materialList[index1];
                        if (!fusionFunc1(material1) && !fusionFunc2(material1)) continue;
                        for (int index2 = index1 + 1; index2 < materialList.Count; ++index2)
                        {
                            ClientCard material2 = materialList[index2];
                            bool checkFlag = fusionFunc1(material1) && fusionFunc2(material2);
                            checkFlag |= fusionFunc2(material1) && fusionFunc1(material2);
                            extraCheckDict.TryGetValue(pair.Key, out Func<ClientCard, ClientCard, bool> extraCheckFunc);
                            checkFlag &= (extraCheckFunc == null || extraCheckFunc(material1, material2));
                            if (checkFlag)
                            {
                                fusionTarget = currentFusionTarget;
                                selectedFusionMaterial.Add(material1);
                                selectedFusionMaterial.Add(material2);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public bool TriBrigadeMercourierActivate()
        {
            // negate
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster) || !CheckLastChainShouldNegated()) return false;
                if (Util.GetLastChainCard().Location == CardLocation.MonsterZone) currentNegateCardList.Add(Util.GetLastChainCard());
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            // search
            if (Card.Location == CardLocation.Removed)
            {
                return false;
            }
            return false;
        }

        public bool TriBrigadeMercourierActivateForSearch()
        {
            // negate
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.MonsterZone)
            {
                return false;
            }
            // search
            if (Card.Location == CardLocation.Removed)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }
            return false;
        }

        public bool AshBlossomActivate()
        {
            if (CheckWhetherNegated() || !CheckLastChainShouldNegated()) return false;
            if (Util.GetLastChainCard().IsCode(_CardId.MaxxC)) return false;
            if (DefaultAshBlossomAndJoyousSpring())
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard.Location == CardLocation.MonsterZone || lastChainCard.Location == CardLocation.SpellZone) currentNegateCardList.Add(Util.GetLastChainCard());
                return true;
            }
            return false;
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }

        public bool DespianTragedyActivate()
        {
            // search
            if (ActivateDescription != Util.GetStringId(Card.Id, 1))
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            // set
            else {
                if (CheckWhetherNegated(true, false, CardType.Trap)) return false;
                Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>{
                    {CardId.BrandedFusion, () => BrandedFusionActivateCheck()},
                    {CardId.BrandedLost, () => {
                        if (Duel.Player == 0 && Duel.Phase >= DuelPhase.End) return false;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedFusion) && BrandedFusionActivateCheck()) return true;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) && BrandedInWhiteActivateCheck()) return true;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedInRed) && BrandedInRedActivateCheck() != null) return true;
                        if (!summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion()) return true;
                        if ((Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) || (!summoned && Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous)))) return true;
                        return false;
                    } },
                    {CardId.BrandedInHighSpirits, BrandedInHighSpiritsActivateCheck},
                    {CardId.BrandedInRed, () => BrandedInRedActivateCheck() != null },
                    {CardId.BrandedInWhite, BrandedInWhiteActivateCheck },
                    {CardId.BrightestBlazingBrandedKing, () => Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(albazFusionMonster)) },
                    {CardId.BrandedOpening, () => Bot.Hand.Count > 2 && !activatedCardIdList.Contains(CardId.BrandedOpening) }
                };
                foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                {
                    ClientCard target = Bot.Graveyard.FirstOrDefault(card => card.IsCode(pair.Key));
                    if (target != null && pair.Value())
                    {
                        activatedCardIdList.Add(Card.Id);
                        AI.SelectCard(target);
                        SelectSTPlace(target, true);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DespianTragedySet()
        {
            if (Bot.Graveyard.Any(c => c != null && c.HasType(CardType.Spell | CardType.Trap) && c.HasSetcode(SetcodeBranded)))
            {
                summoned = true;
                return true;
            }

            return false;
        }

        public bool NadirServantActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell) || CheckWhetherWillbeRemoved()) return false;
            bool checkResult = NadirServantActivateCheck(null, false, out _);
            if (checkResult)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool NadirServantActivateCheck(IList<ClientCard> cards, bool force, out ClientCard target)
        {
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.AlbionTheBrandedDragon, () => !sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)) },
                {CardId.DespianLuluwalilith, () => CheckRemainInDeck(CardId.BlazingCartesiaTheVirtuous, CardId.GuidingQuemTheVirtuous) > 0 },
                {CardId.TitanikladTheAshDragon, () => CheckRemainInDeck(CardId.GuidingQuemTheVirtuous) > 0 },
                {CardId.SprindTheIrondashDragon, () => CheckRemainInDeck(CardId.SpringansKitt) > 0 },
                {CardId.RindbrummTheStrikingDragon, () => Bot.Graveyard.Any(c => c != null && c.IsOriginalCode(CardId.FallenOfAlbaz)) },
                {CardId.AlbaLenatusTheAbyssDragon, () => force && CheckRemainInDeck(CardId.FusionDeployment, CardId.BrandedFusion) > 0 },
                {CardId.GranguignolTheDuskDragon, () => force},
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                if (cards == null)
                {
                    if (Bot.HasInExtra(pair.Key) && pair.Value())
                    {
                        target = null;
                        return true;
                    }
                } else
                {
                    ClientCard tg = cards.FirstOrDefault(c => c.IsOriginalCode(pair.Key));
                    if (tg != null && pair.Value())
                    {
                        target = tg;
                        return true;
                    }
                }
            }

            target = null;
            return false;
        }

        public bool FusionDeploymentActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (FusionDeploymentSpSummonTarget() > 0 && !Bot.HasInHand(CardId.BrandedLost))
            {
                SelectSTPlace(Card, true);
                return true;
            }

            return false;
        }

        public int FusionDeploymentSpSummonTarget()
        {
            if (CheckRemainInDeck(CardId.FallenOfAlbaz) > 0 && CheckAlbazFusion(Card) && GetProblematicEnemyMonster(0, false, false, CardType.Monster) != null)
            {
                return CardId.FallenOfAlbaz;
            }
            if (CheckRemainInDeck(CardId.BlazingCartesiaTheVirtuous) > 0 && Bot.HasInExtra(CardId.GranguignolTheDuskDragon))
            {
                if (Bot.Hand.Any(c => c.IsMonster() && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark))
                    || Bot.GetMonsters().Any(c => c.IsMonster() && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark) && !c.IsCode(cannotBeFusionMaterialIdList)))
                {
                    return CardId.BlazingCartesiaTheVirtuous;
                }
            }
            if (CheckRemainInDeck(CardId.FallenOfAlbaz) > 0 && CheckAlbazFusion(Card))
            {
                return CardId.FallenOfAlbaz;
            }
            return 0;
        }

        public bool BrandedInWhiteActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Spell)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                SelectSTPlace(Card);
                return true;
            } else
            {
                if (_BrandedInWhiteActivateCheck(true))
                {
                    activatedCardIdList.Add(Card.Id);
                    SelectSTPlace(Card, true);
                    return true;
                }
            }
            return false;
        }

        public bool BrandedInWhiteActivateCheck()
        {
            return _BrandedInWhiteActivateCheck(false);
        }

        public bool _BrandedInWhiteActivateCheck(bool activate = false)
        {
            if (CheckWhetherNegated(true, true, CardType.Spell) || activatedCardIdList.Contains(CardId.BrandedInWhite) || nadirActivated) return false;
            if (CheckShouldNoMoreSpSummon() && Bot.MonsterZone.Any(c => c != null && c.GetDefensePower() >= 2000)) return false;
            if (BrandedInWhiteFusionTarget(Bot.ExtraDeck, out ClientCard _fusionTarget) > 0)
            {
                if (activate) Logger.DebugWriteLine("White prepare fusion: " + _fusionTarget?.Name);
                return true;
            }

            return false;
        }

        public int BrandedInWhiteFusionTarget(IList<ClientCard> cards, out ClientCard target)
        {
            target = null;
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.MirrorjadeTheIcebladeDragon, () => {
                    if (Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) || Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true)) return false;
                    bool albazFlag = Bot.Graveyard.Any(c => c.IsCode(CardId.FallenOfAlbaz));
                    albazFlag |= Bot.MonsterZone.Any(c => c != null && c.IsOriginalCode(CardId.FallenOfAlbaz));
                    albazFlag |= Bot.Hand.Any(c => c.IsOriginalCode(CardId.FallenOfAlbaz));
                    if (!albazFlag) return false;

                    bool checkFlag = Bot.Graveyard.Any(c => c != null && !sendToGYThisTurn.Contains(c) && !c.IsCode(cannotBeFusionMaterialIdList)
                        && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
                    checkFlag |= Bot.MonsterZone.Any(c => c != null && !c.IsCode(cannotBeFusionMaterialIdList)
                        && (c.IsCode(albazFusionMonster) || c.IsCode(CardId.GranguignolTheDuskDragon)));
                    return checkFlag;
                } },
                {CardId.BorreloadFuriousDragon, () => {
                    if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                    {
                        List<ClientCard> darkDragonList = Bot.Hand.Where(c => c != null && c.IsMonster() && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon)).ToList();
                        darkDragonList.AddRange(Bot.MonsterZone.Where(c => c != null && c.IsMonster() && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon) && !c.IsCode(cannotBeFusionMaterialIdList)).ToList());
                        List<ClientCard> graveDarkDragonList = Bot.Graveyard.Where(c => c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark)
                            && !c.IsCode(cannotBeFusionMaterialIdList) && !CheckWhetherShouldKeepInGrave(c) ).ToList();

                        bool hasAlbaz = darkDragonList.Any(c => c.IsCode(CardId.FallenOfAlbaz)) || graveDarkDragonList.Any(c => c.IsCode(CardId.FallenOfAlbaz));
                        int darkDragonCount = darkDragonList.Count;
                        if (hasAlbaz)
                        {
                            darkDragonCount += graveDarkDragonList.Count;
                        }
                        return darkDragonCount >= 2;
                    }
                    return false;
                } },
                {CardId.GuardianChimera, () =>
                {
                    if (CheckWhetherNegated(true, true, CardType.Monster) || DefaultCheckWhetherCardIdIsNegated(CardId.GuardianChimera)) return false;
                    int enemyCardCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
                    if (enemyCardCount == 0) return false;

                    return ChimeraFusionMaterialList().Count > 0;
                } },
                {CardId.LubellionTheSearingDragon, () =>
                {
                    if (activatedCardIdList.Contains(CardId.LubellionTheSearingDragon)
                        || DefaultCheckWhetherCardIdIsNegated(CardId.LubellionTheSearingDragon)
                        || CheckWhetherNegated(true, true, CardType.Monster))
                    {
                        return false;
                    }
                    List<ClientCard> checkMaterialList = new List<ClientCard>(Bot.Graveyard.Where(c => c != null && c.IsMonster()).OrderBy(c => c.GetDefensePower())).ToList();
                    checkMaterialList.AddRange(Bot.GetMonsters().OrderBy(c => c.GetDefensePower()));
                    checkMaterialList.AddRange(Bot.Hand);
                    ClientCard albaz = checkMaterialList.Where(c => c.IsCode(CardId.FallenOfAlbaz)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                    ClientCard darkMonster = checkMaterialList.Where(c => c != albaz && c.HasAttribute(CardAttribute.Dark)).FirstOrDefault();
                    if (albaz == null || darkMonster == null) return false;
                    if (Bot.Hand.Count(c => c != albaz && c != darkMonster && !c.IsCode(CardId.BrandedInWhite)) == 0) return false;

                    return true;
                } },
                {CardId.AlbionTheSanctifireDragon, () =>
                {
                    List<ClientCard> checkMaterialList = new List<ClientCard>(Bot.Graveyard.Where(c => c != null && c.IsMonster()).OrderBy(c => c.GetDefensePower())).ToList();
                    checkMaterialList.AddRange(Bot.GetMonsters().OrderBy(c => c.GetDefensePower()));
                    checkMaterialList.AddRange(Bot.Hand);
                    ClientCard albaz = checkMaterialList.FirstOrDefault(c => c.IsCode(CardId.FallenOfAlbaz));
                    ClientCard lightSpellcaster = checkMaterialList.FirstOrDefault(c => c.HasRace(CardRace.SpellCaster) && c.HasAttribute(CardAttribute.Light));
                    if (albaz == null || lightSpellcaster == null) return false;

                    int remainMonsterCount = Enemy.GetGraveyardMonsters().Count;
                    remainMonsterCount += Bot.Graveyard.Where(c => c.IsMonster() && c != albaz && c != lightSpellcaster).Count();
                    remainMonsterCount += Bot.HasInHand(_CardId.MaxxC) ? 1 : 0;
                    return remainMonsterCount >= 2;
                } },
                {CardId.RindbrummTheStrikingDragon, () => {
                    if (!Bot.HasInGraveyard(CardId.TriBrigadeMercourier)) return false;
                    bool albazCheck = Bot.HasInHandOrHasInMonstersZone(CardId.FallenOfAlbaz);
                    albazCheck |= Bot.HasInGraveyard(CardId.FallenOfAlbaz);
                    return albazCheck;
                } },
                {CardId.DespianQuaeritis, () =>
                {
                    bool checkFlag = Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.Attack >= 2500 && !(c.HasType(CardType.Fusion) && c.Level >= 8));
                    if (checkFlag)
                    {
                        ClientCard despianInGrave = Bot.Graveyard.Where(c => c != null && c.HasSetcode(SetcodeDespain) && !CheckWhetherShouldKeepInGrave(c))
                            .OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        if (despianInGrave != null)
                        {
                            bool albazCheck = Bot.HasInHandOrHasInMonstersZone(CardId.FallenOfAlbaz);
                            albazCheck |= Bot.HasInGraveyard(CardId.FallenOfAlbaz);
                            return albazCheck;
                        }
                        List<ClientCard> fusionMaterialList = Bot.Hand.Where(c => c.IsMonster()).OrderBy(c => c.GetDefensePower()).ToList();
                        fusionMaterialList.AddRange(Bot.MonsterZone.Where(c => c != null && !c.IsCode(cannotBeFusionMaterialIdList)).OrderBy(c => c.GetDefensePower()).ToList());
                        ClientCard despian = fusionMaterialList.FirstOrDefault(c => c.HasSetcode(SetcodeDespain));
                        if (despian != null)
                        {
                            checkFlag = fusionMaterialList.Any(c => c != despian && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark));
                            checkFlag |= Bot.HasInGraveyard(CardId.FallenOfAlbaz);
                            return checkFlag;
                        }
                    }

                    return false;
                } },
                {CardId.TitanikladTheAshDragon, () =>
                {
                    List<ClientCard> checkMaterialList = new List<ClientCard>(Bot.Graveyard.Where(c => c != null && c.IsMonster()).OrderBy(c => c.GetDefensePower())).ToList();
                    checkMaterialList.AddRange(Bot.GetMonsters().OrderBy(c => c.GetDefensePower()));
                    checkMaterialList.AddRange(Bot.Hand);
                    ClientCard albaz = checkMaterialList.Where(c => c.IsCode(CardId.FallenOfAlbaz)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();

                    foreach (ClientCard material in checkMaterialList)
                    {
                        if (material != albaz && material.IsMonster() && material.Attack >= 2500 && !material.IsCode(cannotBeFusionMaterialIdList) && !Util.IsTurn1OrMain2())
                        {
                            bool checkFlag = Enemy.GetMonsterCount() == 0 && !CheckWhetherShouldKeepInGrave(material) && (material.IsFacedown() || material.Location != CardLocation.MonsterZone);

                            int expectedAttack = 2900 + material.Level * 100;
                            int botBestPower = Util.GetBestPower(Bot);
                            int beforeBetterCount = Enemy.MonsterZone.Count(c => c != null && c.GetDefensePower() >= botBestPower);
                            int afterBetterCount = Enemy.MonsterZone.Count(c => c != null && c.GetDefensePower() >= expectedAttack);
                            checkFlag |= afterBetterCount < beforeBetterCount;

                            return checkFlag;
                        }
                    }
                    return false;
                } },
                {CardId.AlbaLenatusTheAbyssDragon, () =>
                {
                    if (Util.GetOneEnemyBetterThanMyBest() == null && Duel.MainPhase.CanBattlePhase)
                    {
                        ClientCard albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsOriginalCode(CardId.FallenOfAlbaz));
                        if (albaz == null)
                        {
                            albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.FallenOfAlbaz));
                        }
                        if (albaz == null) return false;
                        foreach (ClientCard material in Bot.Graveyard)
                        {
                            if (material != null && material != albaz && material.IsMonster() && material.HasRace(CardRace.Dragon) && !material.IsCode(cannotBeFusionMaterialIdList))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                } }
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                if (target != null && pair.Value())
                {
                    return pair.Key;
                }
            }

            target = null;
            return 0;
        }

        public List<ClientCard> ChimeraFusionMaterialList(bool dragonCheck = true)
        {
            int enemyCardCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            List<ClientCard> fieldMonsterList = Bot.MonsterZone.Where(c => c != null && c.GetDefensePower() <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList))
                .OrderBy(c => c.GetDefensePower()).ToList();
            List<ClientCard> handMonsterList = Bot.Hand.Where(c => c.IsMonster())
                .OrderBy(c => c.GetDefensePower()).ToList();
            // 2 monsters on field + 1 monster in hand
            if (enemyCardCount >= 2 && fieldMonsterList.Count >= 2)
            {
                if (fieldMonsterList.Count < 2 || handMonsterList.Count < 1) return new List<ClientCard>() ;
                foreach (ClientCard handMonster in handMonsterList)
                {
                    for (int fieldIndex1 = 0; fieldIndex1 < fieldMonsterList.Count - 1; ++fieldIndex1)
                    {
                        ClientCard fieldMonster1 = fieldMonsterList[fieldIndex1];
                        if (fieldMonster1.IsCode(handMonster.Id) || handMonster.IsCode(fieldMonster1.Id)) continue;
                        for (int fieldIndex2 = fieldIndex1 + 1; fieldIndex2 < fieldMonsterList.Count; ++fieldIndex2)
                        {
                            ClientCard fieldMonster2 = fieldMonsterList[fieldIndex2];
                            if (fieldMonster2.IsCode(handMonster.Id) || handMonster.IsCode(fieldMonster2.Id)) continue;
                            if (fieldMonster2.IsCode(fieldMonster1.Id) || fieldMonster1.IsCode(fieldMonster2.Id)) continue;

                            List<ClientCard> materialList = new List<ClientCard> { handMonster, fieldMonster1, fieldMonster2 };
                            bool checkFlag = dragonCheck && materialList.Any(c => c.HasRace(CardRace.Dragon));
                            if (checkFlag)
                            {
                                return materialList;
                            }
                        }
                    }
                }
            }
            // 1 monster on field + 2 monsters in hand
            if (enemyCardCount == 1 || fieldMonsterList.Count == 1)
            {
                if (fieldMonsterList.Count < 1 || handMonsterList.Count < 2) return new List<ClientCard>();
                foreach (ClientCard fieldMonster in fieldMonsterList)
                {
                    for (int handIndex1 = 0; handIndex1 < handMonsterList.Count - 1; ++handIndex1)
                    {
                        ClientCard handMonster1 = handMonsterList[handIndex1];
                        if (handMonster1.IsCode(fieldMonster.Id) || fieldMonster.IsCode(handMonster1.Id)) continue;
                        for (int handIndex2 = handIndex1 + 1; handIndex2 < handMonsterList.Count; ++handIndex2)
                        {
                            ClientCard handMonster2 = handMonsterList[handIndex2];
                            if (handMonster2.IsCode(fieldMonster.Id) || fieldMonster.IsCode(handMonster2.Id)) continue;
                            if (handMonster2.IsCode(handMonster1.Id) || handMonster1.IsCode(handMonster2.Id)) continue;

                            List<ClientCard> materialList = new List<ClientCard> { fieldMonster, handMonster1, handMonster2 };
                            bool checkFlag = dragonCheck && materialList.Any(c => c.HasRace(CardRace.Dragon));
                            if (checkFlag)
                            {
                                return materialList;
                            }
                        }
                    }
                }
            }

            return new List<ClientCard>();
        }

        public bool BrandedFusionActivate()
        {
            if (BrandedFusionActivateCheck())
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public bool BrandedFusionActivateCheck(bool endPhaseCheck = true)
        {
            if (CheckWhetherNegated(true, true, CardType.Spell) || activatedCardIdList.Contains(CardId.BrandedFusion)) return false;
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.FallenOfAlbaz) && CheckRemainInDeck(CardId.FallenOfAlbaz) == 0) return false;
            if (endPhaseCheck && Duel.Phase >= DuelPhase.End) return false;
            return true;
        }

        public bool GoldSarcophagusActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (GoldSarcophagusTarget(null, out _) > 0)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public int GoldSarcophagusTarget(IList<ClientCard> cards, out ClientCard target)
        {
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.DespianTragedy, () => !activatedCardIdList.Contains(CardId.DespianTragedy) && !DefaultCheckWhetherCardIdIsNegated(CardId.DespianTragedy) },
                {CardId.TriBrigadeMercourier, () => !activatedCardIdList.Contains(CardId.TriBrigadeMercourier + 1) && !DefaultCheckWhetherCardIdIsNegated(CardId.TriBrigadeMercourier) }
            };
            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                int cardId = pair.Key;
                if (pair.Value())
                {
                    if (cards != null)
                    {
                        target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            return cardId;
                        }
                    }
                    else if (CheckRemainInDeck(cardId) > 0)
                    {
                        target = null;
                        return cardId;
                    }
                }
            }

            target = null;
            return 0;
        }

        public bool FoolishBurialActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell) || CheckWhetherWillbeRemoved()) return false;
            if (FoolishBurialTarget(null, out _) > 0)
            {
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public int FoolishBurialTarget(IList<ClientCard> cards, out ClientCard target)
        {
            // tragedy
            if (!activatedCardIdList.Contains(CardId.DespianTragedy) && !DefaultCheckWhetherCardIdIsNegated(CardId.DespianTragedy))
            {
                if (cards != null)
                {
                    target = cards.FirstOrDefault(c => c.IsCode(CardId.DespianTragedy));
                    if (target != null)
                    {
                        return CardId.DespianTragedy;
                    }
                } else
                {
                    if (CheckRemainInDeck(CardId.DespianTragedy) > 0)
                    {
                        target = null;
                        return CardId.DespianTragedy;
                    }
                }
            }

            // send to GY check
            bool sendToGYFlag = CheckRemainInDeck(CardId.BrandedRetribution) > 0;
            sendToGYFlag |= Bot.HasInGraveyard(CardId.BrandedRetribution) && CheckRemainInDeck(CardId.BrandedFusion) > 0;
            if (sendToGYFlag)
            {
                Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                {
                    {CardId.BystialSaronir, () => !activatedCardIdList.Contains(CardId.BystialSaronir + 1) && !DefaultCheckWhetherCardIdIsNegated(CardId.BystialSaronir) },
                    {CardId.AlbionTheShroudedDragon, () => !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon) && !DefaultCheckWhetherCardIdIsNegated(CardId.AlbionTheShroudedDragon) }
                };

                foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                {
                    int cardId = pair.Key;
                    if (pair.Value())
                    {
                        if (cards != null)
                        {
                            target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                            if (target != null && pair.Value())
                            {
                                return cardId;
                            }
                        } else if (CheckRemainInDeck(cardId) > 0)
                        {
                            target = null;
                            return cardId;
                        }
                    }
                }
            }

            // albaz
            if (!Bot.HasInGraveyard(CardId.FallenOfAlbaz))
            {
                bool albazCheckFlag = Bot.HasInHand(new List<int> { CardId.BrandedInRed, CardId.BrandedInWhite });
                albazCheckFlag |= Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous) && Bot.MonsterZone.Count(c => c != null && c.Sequence < 5) < 5;
                if (albazCheckFlag)
                {
                    int albazCountCheck = Bot.HasInHandOrInSpellZone(CardId.BrandedFusion) ? 2 : 1;
                    if (cards != null)
                    {
                        List<ClientCard> albazList = cards.Where(c => c.IsCode(CardId.FallenOfAlbaz)).ToList();
                        if (albazList.Count >= albazCountCheck)
                        {
                            target = albazList.First();
                            return CardId.FallenOfAlbaz;
                        }
                    } else
                    {
                        if (CheckRemainInDeck(CardId.FallenOfAlbaz) >= albazCountCheck)
                        {
                            target = null;
                            return CardId.FallenOfAlbaz;
                        }
                    }
                }
            }

            target = null;
            return 0;
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
                    if (DefaultCheckWhetherCardIdIsNegated(code)) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
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

        public bool BrandedInHighSpiritsActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Spell)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            } else
            {
                if (BrandedInHighSpiritsActivateCheck())
                {
                    activatedCardIdList.Add(Card.Id);
                    SelectSTPlace(Card, true);
                    return true;
                }
            }
            return false;
        }

        public bool BrandedInHighSpiritsActivateCheck()
        {
            bool lubellionCheck = Bot.HasInHand(CardId.TheBystialLubellion) && CheckRemainInDeck(CardId.BystialSaronir) > 0 && !activatedCardIdList.Contains(CardId.TheBystialLubellion)
                && Duel.Player == 0 && (Duel.Phase <= DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && !CheckWhetherWillbeRemoved();

            if (CheckWhetherNegated(true, true, CardType.Spell) || activatedCardIdList.Contains(CardId.BrandedInHighSpirits) || CheckWhetherWillbeRemoved()) return false;
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.AlbionTheBrandedDragon, () => !sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)) && !lubellionCheck
                    && Bot.Hand.Any(c => BrandedInHighSpiritDiscardDragonCheck(c) ) },
                {CardId.TitanikladTheAshDragon, () => !sendToGYThisTurn.Any(c => c.IsCode(CardId.TitanikladTheAshDragon)) && !lubellionCheck
                    && Bot.Hand.Any(c => BrandedInHighSpiritDiscardDragonCheck(c) )
                    && CheckRemainInDeck(CardId.GuidingQuemTheVirtuous, CardId.FallenOfAlbaz) > 0},
                {CardId.GranguignolTheDuskDragon, () => Bot.Hand.Any(c => c.HasRace(CardRace.SpellCaster) && !(CheckWhetherCanSummon() && c.IsOriginalCode(CardId.GuidingQuemTheVirtuous))) },
                {CardId.RindbrummTheStrikingDragon, () => Bot.Hand.Any(c => c.HasRace(CardRace.WindBeast)
                    && (!c.IsCode(CardId.TriBrigadeMercourier) || !Bot.MonsterZone.Any(c2 => c2 != null && c2.IsFaceup() && c2.IsCode(albazFusionMonster)))) }
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                if (Bot.HasInExtra(pair.Key) && pair.Value())
                {
                    return true;
                }
            }
            return false;
        }

        public bool BrandedInHighSpiritDiscardDragonCheck(ClientCard card)
        {
            if (!card.HasRace(CardRace.Dragon)) return false;
            if (Duel.Player == 0 && (Duel.Phase <= DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
            {
                if (card.IsOriginalCode(CardId.AlbionTheShroudedDragon) && !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon)) return false;
                if (card.IsOriginalCode(CardId.TheBystialLubellion) && !activatedCardIdList.Contains(CardId.TheBystialLubellion) && CheckRemainInDeck(CardId.BystialSaronir) > 0) return false;
            }

            return true;
        }

        public bool BrandedOpeningActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Duel.Player == 0)
            {
                if (Bot.HasInHand(CardId.AlbionTheShroudedDragon) && !CheckWhetherWillbeRemoved() && !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon)) return false;
                bool canCallCartesia = Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous) && !summoned;
                canCallCartesia |= !activatedCardIdList.Contains(CardId.FusionDeployment) && Bot.HasInHandOrInSpellZone(CardId.FusionDeployment)
                    && !CheckShouldNoMoreSpSummon() && Bot.HasInExtra(CardId.GranguignolTheDuskDragon) && CheckRemainInDeck(CardId.BlazingCartesiaTheVirtuous) > 0;
                if (canCallCartesia) return false;
            }
            bool goal = CheckRemainInDeck(CardId.AluberTheJesterOfDespia) > 0 && !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && !enemyActivateLockBird;
            goal |= CheckRemainInDeck(CardId.GuidingQuemTheVirtuous) > 0;
            if (goal)
            {
                SelectSTPlace(Card, true);
                activatedCardIdList.Add(Card.Id);
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
                    return true;
                }
            }
            return false;
        }

        public bool BrandedInRedActivate()
        {
            ClientCard targetCard = BrandedInRedActivateCheck(true);
            if (targetCard != null)
            {
                AI.SelectCard(targetCard);
                SelectSTPlace(Card, true);
                return true;
            }
            return false;
        }

        public ClientCard BrandedInRedActivateCheck(bool updateMaterialList = false)
        {
            if (CheckWhetherNegated(true, true, CardType.Spell) || activatedCardIdList.Contains(CardId.BrandedInRed)) return null;
            if (Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(CardId.BlazingCartesiaTheVirtuous))) return null;
            if (Duel.CurrentChain.Any(c => c != null && c.Controller == 0 && c.IsCode(CardId.AlbionTheBrandedDragon))
                && !Util.ChainContainPlayer(1)) return null;
            if (nadirActivated) return null;

            List<ClientCard> materialList = Bot.MonsterZone.Where(c => c != null && c.Attack <= 2500 && !c.IsCode(cannotBeFusionMaterialIdList)).ToList();
            materialList.AddRange(Bot.Hand.Where(c => c.IsMonster()
                && !(CheckWhetherCanSummon() &&
                    ((!activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) && c.IsCode(CardId.AluberTheJesterOfDespia))
                    || (!activatedCardIdList.Contains(CardId.SpringansKitt) && c.IsCode(CardId.SpringansKitt)))
                    )
                )
            );
            
            List<ClientCard> graveTargetList = Bot.Graveyard.Where(c => c != null && c.IsMonster() && !c.HasType(CardType.Fusion | CardType.Synchro)
                            && (c.IsCode(CardId.FallenOfAlbaz) || c.HasSetcode(SetcodeDespain))).ToList();

            // select targeted monster
            if (Duel.LastChainPlayer == 1)
            {
                // escape grave monster
                List<ClientCard> targetedList = Duel.LastChainTargets.Where(c => c != null && c.Location == CardLocation.Grave && c.Controller == 0
                    && !c.HasType(CardType.Fusion | CardType.Synchro) && (c.IsCode(CardId.FallenOfAlbaz) || c.HasSetcode(SetcodeDespain))).ToList();

                if (targetedList.Count > 0)
                {
                    foreach (ClientCard target in targetedList)
                    {
                        List<ClientCard> newMaterialList = new List<ClientCard>(materialList) { target };
                        BrandedInRedFusionCheck(Bot.ExtraDeck, 0, newMaterialList, new List<ClientCard> { target }, out ClientCard _fusionTarget, out _);
                        if (_fusionTarget != null)
                        {
                            if (updateMaterialList) Logger.DebugWriteLine("Red prepare fusion 1: " + _fusionTarget.Name);
                            return target;
                        }
                    }
                }

                // escape target
                ClientCard lastCahinCard = Util.GetLastChainCard();
                if (lastCahinCard != null)
                {
                    List<ClientCard> chainTargetList = Duel.LastChainTargets.Where(c => c.Controller == 0 && c.Location == CardLocation.MonsterZone
                            && (!c.IsCode(cannotBeFusionMaterialIdList) || c.Attack <= 2500)).ToList();
                    if (chainTargetList.Count > 0)
                    {
                        if (lastCahinCard.IsCode(targetNegateIdList))
                        {
                            chainTargetList = chainTargetList.Where(c => c.Attack <= 2500 && !c.IsCode(CardId.AlbionTheBrandedDragon)).ToList();
                        }

                        foreach (ClientCard target in graveTargetList)
                        {
                            List<ClientCard> newMaterialList = new List<ClientCard>(materialList) { target };
                            BrandedInRedFusionCheck(Bot.ExtraDeck, 0, newMaterialList, chainTargetList,
                                out ClientCard _fusionTarget, out List<ClientCard> usedMaterialList);

                            if (_fusionTarget != null)
                            {
                                if (updateMaterialList)
                                {
                                    Logger.DebugWriteLine("Red prepare fusion 2: " + _fusionTarget.Name);
                                    brandedInRedMaterialList.AddRange(usedMaterialList.Intersect(chainTargetList));
                                }
                                return target;
                            }
                        }
                    }
                }
            }

            bool shouldSummonFirst = Duel.Player == 0 && (Duel.Phase <= DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && !summoned
                && (Bot.HasInHand(new int[] { CardId.AluberTheJesterOfDespia, CardId.GuidingQuemTheVirtuous, CardId.SpringansKitt })
                || Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion(Card));
            bool idleFlag = Duel.Player == 1 || CurrentTiming == -1;

            if (shouldSummonFirst || !idleFlag) return null;

            // for fusion searing dragon
            if (!Bot.MonsterZone.Any(c => c != null && c.HasType(CardType.Fusion)) && Duel.LastChainPlayer != 0
                    && !CheckWhetherNegated(true, true, CardType.Monster) && !activatedCardIdList.Contains(CardId.LubellionTheSearingDragon))
            {
                foreach (ClientCard target in graveTargetList)
                {
                    List<ClientCard> newMaterialList = new List<ClientCard>(materialList) { target };
                    BrandedInRedFusionCheck(Bot.ExtraDeck, CardId.LubellionTheSearingDragon, newMaterialList, new List<ClientCard> { target }, out ClientCard _fusionTarget, out _);
                    if (_fusionTarget != null)
                    {
                        return target;
                    }
                }
            }

            // remove cards
            List<ClientCard> problemCardList = GetProblematicEnemyCardList(false, false, CardType.Monster);
            if (problemCardList.Count > 0 || (Duel.Phase == DuelPhase.End && Duel.Player == 1))
            {
                if (!enemyActivateLockBird)
                {
                    foreach (ClientCard target in graveTargetList)
                    {
                        List<ClientCard> newMaterialList = new List<ClientCard>(materialList) { target };
                        BrandedInRedFusionCheck(Bot.ExtraDeck, CardId.GuardianChimera, newMaterialList, new List<ClientCard> { target }, out ClientCard _fusionTarget, out _);
                        if (_fusionTarget != null)
                        {
                            if (updateMaterialList) Logger.DebugWriteLine("Red prepare fusion 3: " + _fusionTarget.Name);
                            return target;
                        }
                    }
                }

                foreach (ClientCard target in graveTargetList)
                {
                    List<ClientCard> newMaterialList = new List<ClientCard>(materialList) { target };
                    BrandedInRedFusionCheck(Bot.ExtraDeck, CardId.BorreloadFuriousDragon, newMaterialList, new List<ClientCard> { target }, out ClientCard _fusionTarget, out _);
                    if (_fusionTarget != null)
                    {
                        if (updateMaterialList) Logger.DebugWriteLine("Red prepare fusion 4: " + _fusionTarget.Name);
                        return target;
                    }
                }
            }

            return null;
        }

        public void BrandedInRedFusionCheck(
            IList<ClientCard> canSummonList, int mustSummonId,
            List<ClientCard> materialList, List<ClientCard> mustMaterialList,
            out ClientCard fusionTarget, out List<ClientCard> selectedFusionMaterialList)
        {
            fusionTarget = null;
            selectedFusionMaterialList = new List<ClientCard>();

            // fusion id
            // material condition
            // extra check function
            // search material function
            List<Tuple<
                int,
                List<Func<ClientCard, bool>>,
                Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>
            >> checkTupleList = new List<Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>
            >>
            {
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.MirrorjadeTheIcebladeDragon, new List<Func<ClientCard, bool>>
                    {
                        (c) => c.IsCode(CardId.FallenOfAlbaz),
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link)
                    },
                    (l) => !Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true),
                    BrandedInRedUsing2SubFunc
                ),
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.LubellionTheSearingDragon, new List<Func<ClientCard, bool>>
                    {
                        (c) => c.IsCode(CardId.FallenOfAlbaz),
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark)
                    },
                    (l) => !Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true)
                        && Bot.Hand.Count(c => !l.Contains(c)) > 0 && !activatedCardIdList.Contains(CardId.LubellionTheSearingDragon),
                    BrandedInRedUsing2SubFunc
                ),
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.GuardianChimera,
                    null,
                    null,
                    BrandedInRedForChimeraFunc
                ),
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.DespianQuaeritis, new List<Func<ClientCard, bool>>
                    {
                        (c) => c.HasSetcode(SetcodeDespain),
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light | CardAttribute.Dark)
                    },
                    null,
                    BrandedInRedUsing2SubFunc
                ),
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.BorreloadFuriousDragon, new List<Func<ClientCard, bool>>
                    {
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon),
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Dark) && c.HasRace(CardRace.Dragon)
                    },
                    null,
                    BrandedInRedUsing2SubFunc
                ),
                new Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>>(
                    CardId.AlbionTheSanctifireDragon, new List<Func<ClientCard, bool>>
                    {
                        (c) => c.IsCode(CardId.FallenOfAlbaz),
                        (c) => !c.IsCode(cannotBeFusionMaterialIdList) && c.HasAttribute(CardAttribute.Light) && c.HasRace(CardRace.SpellCaster)
                    },
                    (l) => Enemy.Graveyard.Count(c => c != null && c.IsMonster() && c.IsCanRevive())
                        + Bot.Graveyard.Count(c => c != null && !l.Contains(c)) >= 2,
                    BrandedInRedUsing2SubFunc
                ),
            };

            foreach (Tuple<int, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>,
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>>> tuple in checkTupleList)
            {
                if (mustSummonId > 0 && mustSummonId != tuple.Item1) continue;
                ClientCard currentFusionTarget = canSummonList.FirstOrDefault(c => c != null && c.IsCode(tuple.Item1));
                if (currentFusionTarget == null) continue;
                Func<List<ClientCard>, List<ClientCard>, List<Func<ClientCard, bool>>, Func<List<ClientCard>, bool>, List<ClientCard>> materialFunc = tuple.Item4;

                List<ClientCard> currentMaterialList = materialFunc(materialList, mustMaterialList, tuple.Item2, tuple.Item3);
                if (currentMaterialList.Count > 0)
                {
                    fusionTarget = currentFusionTarget;
                    selectedFusionMaterialList = currentMaterialList;
                    return;
                }
            }
        }

        public List<ClientCard> BrandedInRedUsing2SubFunc(List<ClientCard> materialList, List<ClientCard> mustMaterialList,
            List<Func<ClientCard, bool>> checkFuncList, Func<List<ClientCard>, bool> extraCheckFunc)
        {
            List<ClientCard> selectedFusionMaterialList = new List<ClientCard>();

            Func<ClientCard, bool> fusionFunc1 = checkFuncList[0];
            Func<ClientCard, bool> fusionFunc2 = checkFuncList[1];

            if (mustMaterialList != null && mustMaterialList.Count > 0)
            {
                foreach (ClientCard mustMaterial in mustMaterialList)
                {
                    if (!fusionFunc1(mustMaterial) && !fusionFunc2(mustMaterial)) continue;
                    foreach (ClientCard anotherMaterial in materialList)
                    {
                        if (anotherMaterial == mustMaterial) continue;
                        bool checkFlag = fusionFunc1(mustMaterial) && fusionFunc2(anotherMaterial);
                        checkFlag |= fusionFunc2(mustMaterial) && fusionFunc1(anotherMaterial);
                        checkFlag &= (extraCheckFunc == null || extraCheckFunc(new List<ClientCard> { mustMaterial, anotherMaterial }));
                        if (checkFlag)
                        {
                            selectedFusionMaterialList.Add(mustMaterial);
                            selectedFusionMaterialList.Add(anotherMaterial);
                            return selectedFusionMaterialList;
                        }
                    }
                }
            }
            else
            {
                for (int index1 = 0; index1 < materialList.Count - 1; ++index1)
                {
                    ClientCard material1 = materialList[index1];
                    if (!fusionFunc1(material1) && !fusionFunc2(material1)) continue;
                    for (int index2 = index1 + 1; index2 < materialList.Count; ++index2)
                    {
                        ClientCard material2 = materialList[index2];
                        bool checkFlag = fusionFunc1(material1) && fusionFunc2(material2);
                        checkFlag |= fusionFunc2(material1) && fusionFunc1(material2);
                        checkFlag &= (extraCheckFunc == null || extraCheckFunc(new List<ClientCard> { material1, material2 }));
                        if (checkFlag)
                        {
                            selectedFusionMaterial.Add(material1);
                            selectedFusionMaterial.Add(material2);
                            return selectedFusionMaterialList;
                        }
                    }
                }
            }

            return selectedFusionMaterial;
        }

        public List<ClientCard> BrandedInRedForChimeraFunc(List<ClientCard> materialList, List<ClientCard> mustMaterialList,
            List<Func<ClientCard, bool>> checkFuncList, Func<List<ClientCard>, bool> extraCheckFunc)
        {
            List<ClientCard> selectedFusionMaterialList = new List<ClientCard>();

            int enemyCardCount = Enemy.MonsterZone.Count(c => c != null);
            enemyCardCount += Enemy.SpellZone.Count(c => c != null && c.Type != (int)CardType.Spell && c.Type != (int)CardType.Trap);
            if (enemyCardCount == 0 || CheckWhetherNegated(true, true, CardType.Monster))
            {
                return selectedFusionMaterialList;
            }

            List<ClientCard> fieldMaterialList = materialList.Where(c => c.Location == CardLocation.MonsterZone).OrderBy(c => c.Attack).ToList();
            List<ClientCard> handMaterialList = materialList.Where(c => c.Location == CardLocation.Hand || c.Location == CardLocation.Grave).OrderBy(c => c.Attack).ToList();

            // 2 field monster + 1 hand monster
            if (enemyCardCount >= 2)
            {
                foreach (ClientCard handMonster in handMaterialList)
                {
                    for (int fieldIndex1 = 0; fieldIndex1 < fieldMaterialList.Count - 1; ++fieldIndex1)
                    {
                        ClientCard fieldMonster1 = fieldMaterialList[fieldIndex1];
                        if (fieldMonster1.IsCode(handMonster.Id) || handMonster.IsCode(fieldMonster1.Id)) continue;
                        for (int fieldIndex2 = fieldIndex1 + 1; fieldIndex2 < fieldMaterialList.Count; ++fieldIndex2)
                        {
                            ClientCard fieldMonster2 = fieldMaterialList[fieldIndex2];
                            if (fieldMonster2.IsCode(fieldMonster1.Id) || fieldMonster1.IsCode(fieldMonster2.Id)) continue;
                            if (fieldMonster2.IsCode(handMonster.Id) || handMonster.IsCode(fieldMonster2.Id)) continue;

                            return new List<ClientCard> { handMonster, fieldMonster1, fieldMonster2 };
                        }
                    }
                }
            }

            // 1 field monster + 2 hand monster
            foreach (ClientCard fieldMonster in fieldMaterialList)
            {
                for (int handIndex1 = 0; handIndex1 < handMaterialList.Count - 1; ++ handIndex1)
                {
                    ClientCard handMonster1 = handMaterialList[handIndex1];
                    if (handMonster1.IsCode(fieldMonster.Id) || fieldMonster.IsCode(handMonster1.Id)) continue;
                    for (int handIndex2 = handIndex1 + 1; handIndex2 < handMaterialList.Count; ++handIndex2)
                    {
                        ClientCard handMonster2 = handMaterialList[handIndex2];
                        if (handMonster2.IsCode(handMonster1.Id) || handMonster1.IsCode(handMonster2.Id)) continue;
                        if (handMonster2.IsCode(fieldMonster.Id) || fieldMonster.IsCode(handMonster2.Id)) continue;

                        return new List<ClientCard> { fieldMonster, handMonster1, handMonster2 };
                    }
                }
            }

            return selectedFusionMaterialList;
        } 

        public bool BrandedLostActivate()
        {
            // search
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                return true;
            }

            return false;
        }

        public bool BrandedLostCardActivate()
        {
            // search
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup()) return false;

            // activate
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion() && Bot.Hand.Count < 3) return false;
            SelectSTPlace(Card, true);
            return true;
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

        public bool BrightestBlazingBrandedKingActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Trap)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsFaceup() && (c.Location == CardLocation.MonsterZone || c.Location == CardLocation.SpellZone)
                    && !c.IsCode(albazFusionMonster))) 
                {
                    return false;
                }
                if (Duel.CurrentChain.Any(c => c.Controller == 1 && c.IsFaceup() && !currentNegateCardList.Contains(c) && !currentDestroyCardList.Contains(c)
                    && (c.Location == CardLocation.MonsterZone || c.Location == CardLocation.SpellZone)))
                {
                    currentNegateCardList.AddRange(Enemy.MonsterZone.Where(c => c != null && c.IsFaceup()));
                    currentNegateCardList.AddRange(Enemy.SpellZone.Where(c => c != null && c.IsFaceup()));
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            return false;
        }

        public bool BrandedBeastActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            int desc = -1;
            if (ActivateDescription >= Util.GetStringId(CardId.BrandedBeast, 0))
            {
                desc = ActivateDescription - Util.GetStringId(CardId.BrandedBeast, 0);
            }
            Logger.DebugWriteLine("Beast: " + desc.ToString());

            // destroy
            if (ActivateDescription == Util.GetStringId(Card.Id, 0))
            {
                ClientCard destroyTarget = null;
                ClientCard releaseMonster = null;
                List<ClientCard> dangerCardList = GetProblematicEnemyCardList(true, false, CardType.Trap);
                if (dangerCardList.Count > 0)
                {
                    destroyTarget = dangerCardList[0];
                }
                if (destroyTarget == null)
                {
                    if (Duel.Player == 1)
                    {
                        if ((CurrentTiming & hintTimingMainEnd) > 0)
                        {
                            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Trap);
                            if (targetList.Count > 0)
                            {
                                destroyTarget = targetList[0];
                            }
                        }
                    }
                    else
                    {
                        destroyTarget = Util.GetOneEnemyBetterThanMyBest(true, true);
                    }
                }

                bool forceActivateFlag = DefaultOnBecomeTarget();
                int bystialCount = Bot.MonsterZone.Count(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeBystial));
                forceActivateFlag |= bystialCount > 0 && Duel.ChainTargets.Count(c => c.Controller == 0 && c.Location == CardLocation.MonsterZone && c.IsFaceup() && c.HasSetcode(SetcodeBystial)) == bystialCount;
                forceActivateFlag |= Duel.CurrentChain.Any(c => c.Controller == 1 && c.Location == CardLocation.SpellZone && c.IsCode(_CardId.Raigeki));
                if (destroyTarget == null && forceActivateFlag)
                {
                    releaseMonster = Duel.ChainTargets.FirstOrDefault(c => c.Controller == 0 && c.Location == CardLocation.MonsterZone && c.IsFaceup() && c.HasSetcode(SetcodeBystial));
                    if (releaseMonster == null)
                    {
                        if (!activatedCardIdList.Contains(CardId.AlbionTheBrandedDragon + 1) && !sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)))
                        {
                            List<ClientCard> brandedDragonList = Bot.MonsterZone.Where(c => c != null && c.IsCode(CardId.AlbionTheBrandedDragon)).OrderBy(c => c.GetDefensePower()).ToList();
                            if (brandedDragonList.Count > 0)
                            {
                                releaseMonster = brandedDragonList[0];
                            }
                        }
                        if (releaseMonster == null)
                        {
                            releaseMonster = Bot.MonsterZone.Where(c => c != null && c.HasRace(CardRace.Dragon)
                                && !(c.IsOriginalCode(CardId.FallenOfAlbaz) && Util.ChainContainsCard(CardId.FallenOfAlbaz))).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        }
                    }

                    List<ClientCard> targetList = GetNormalEnemyTargetList(true, true, CardType.Trap);
                    if (targetList.Count > 0)
                    {
                        destroyTarget = targetList[0];
                    }
                }

                if (destroyTarget != null)
                {
                    if (!activatedCardIdList.Contains(CardId.AlbionTheBrandedDragon + 1) && !sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)))
                    {
                        List<ClientCard> brandedDragonList = Bot.MonsterZone.Where(c => c != null && c.IsCode(CardId.AlbionTheBrandedDragon)).OrderBy(c => c.GetDefensePower()).ToList();
                        if (brandedDragonList.Count > 0)
                        {
                            releaseMonster = brandedDragonList[0];
                        }
                    }
                    if (releaseMonster == null)
                    {
                        releaseMonster = Bot.MonsterZone.Where(c => c != null && c.HasRace(CardRace.Dragon)
                            && !(c.IsOriginalCode(CardId.FallenOfAlbaz) && Util.ChainContainsCard(CardId.FallenOfAlbaz))).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                    }
                }

                if (releaseMonster != null && destroyTarget != null)
                {
                    activatedCardIdList.Add(Card.Id);
                    AI.SelectCard(releaseMonster);
                    AI.SelectNextCard(destroyTarget);
                    currentDestroyCardList.Add(destroyTarget);
                    return true;
                }
            }

            // place
            if (Duel.Phase == DuelPhase.End && Bot.HasInGraveyard(CardId.BrandedLost))
            {
                activatedCardIdList.Add(Card.Id + 1);
                AI.SelectCard(CardId.BrandedLost);
                return true;
            }

            return false;
        }

        public bool BrandedRetributionActivate()
        {
            if (Card.Location == CardLocation.SpellZone && Duel.LastChainPlayer == 1)
            {
                if (CheckWhetherNegated(true, true, CardType.Trap)) return false;
                bool checkFlag = Bot.Graveyard.Where(c => c != null && c.IsCode(albazFusionMonster)).Count() > 1;
                checkFlag |= Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(new[] { CardId.LubellionTheSearingDragon, CardId.SprindTheIrondashDragon, CardId.AlbaLenatusTheAbyssDragon }));
                if (checkFlag)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Trap)) return false;
                Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>{
                    {CardId.BrandedFusion, () => BrandedFusionActivateCheck()},
                    {CardId.BrandedLost, () => {
                        if (Duel.Player == 0 && Duel.Phase >= DuelPhase.End) return false;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedFusion) && BrandedFusionActivateCheck()) return true;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) && BrandedInWhiteActivateCheck()) return true;
                        if (Bot.HasInHandOrInSpellZone(CardId.BrandedInRed) && BrandedInRedActivateCheck() != null) return true;
                        if (!summoned && Bot.HasInHand(CardId.FallenOfAlbaz) && CheckAlbazFusion()) return true;
                        if ((Bot.HasInMonstersZone(CardId.BlazingCartesiaTheVirtuous) || (!summoned && Bot.HasInHand(CardId.BlazingCartesiaTheVirtuous)))) return true;
                        return false;
                    } },
                    {CardId.BrandedInHighSpirits, () => !(Duel.Player == 1 && (fusionToGYFlag || Duel.Phase != DuelPhase.End)) && BrandedInHighSpiritsActivateCheck()},
                    {CardId.BrandedInRed, () => BrandedInRedActivateCheck() != null },
                    {CardId.BrandedInWhite, BrandedInWhiteActivateCheck },
                    {CardId.BrightestBlazingBrandedKing, () => !(Duel.Player == 1 && (fusionToGYFlag || Duel.Phase != DuelPhase.End)) && Bot.GetMonsters().Any(c => c.IsFaceup() && c.IsCode(albazFusionMonster)) },
                    {CardId.BrandedOpening, () => Bot.Hand.Count > 2 && !activatedCardIdList.Contains(CardId.BrandedOpening) }
                };
                foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                {
                    ClientCard target = Bot.Graveyard.FirstOrDefault(card => card.IsCode(pair.Key));
                    if (target != null && pair.Value())
                    {
                        activatedCardIdList.Add(Card.Id);
                        AI.SelectCard(target);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GuardianChimeraActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            return true;
        }

        public bool AlbionTheSanctifireDragonActivate()
        {
            // spsummon from grave
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                List<ClientCard> allTargetList = Enemy.Graveyard.Where(c => c != null && c.IsMonster() && c.IsCanRevive()).ToList();
                allTargetList.AddRange(Bot.Graveyard.Where(c => c != null && c.IsMonster() && c.IsCanRevive()).ToList());

                List<ClientCard> targetList = new List<ClientCard>();

                // spsummon albaz
                if (CheckAlbazFusion(null, out List<ClientCard> materialList) && !spSummoningAlbaz)
                {
                    bool albazFlag = materialList.Count > 1;
                    if (materialList.Count > 0)
                    {
                        ClientCard material = materialList[0];
                        albazFlag |= material.HasType(CardType.Ritual | CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
                        albazFlag |= material.IsFloodgate() || material.IsOneForXyz() || Util.GetWorstBotMonster().GetDefensePower() < material.Attack;
                        albazFlag |= Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.LastChainPlayer == -1;
                    }

                    if (albazFlag)
                    {
                        ClientCard albaz = allTargetList.FirstOrDefault(c => c.IsOriginalCode(CardId.FallenOfAlbaz));
                        ClientCard worstMonster = allTargetList.Where(c => c != albaz && !currentDestroyCardList.Contains(c)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        if (albaz != null && worstMonster != null
                            && (GetProblematicEnemyMonster(0, false, false, CardType.Monster) != null || Math.Max(worstMonster.Attack, worstMonster.Defense) <= albaz.Defense))
                        {
                            Logger.DebugWriteLine("Sanctifire 1");
                            targetList.AddRange(new[] { albaz, worstMonster });
                            spSummoningAlbaz = true;
                        }
                    }
                }

                // spsummon floogate
                if (targetList.Count == 0)
                {
                    ClientCard floogateCard = allTargetList.Where(c => c.IsFloodgate()).OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                    if (floogateCard != null)
                    {
                        // select worst monster
                        ClientCard worstMonster = allTargetList.Where(c => c != floogateCard).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        if (worstMonster != null)
                        {
                            Logger.DebugWriteLine("Sanctifire 2");
                            targetList.AddRange(new[] { floogateCard, worstMonster });
                        }
                    }
                }

                // spsummon target
                if (targetList.Count == 0 && Duel.LastChainPlayer == 1)
                {
                    List<ClientCard> targetedList = Duel.LastChainTargets.Intersect(allTargetList).ToList();
                    if (targetedList.Count > 0)
                    {
                        ClientCard target = ShuffleList(targetedList)[0];
                        ClientCard anotherTarget = null;
                        if (target.GetDefensePower() >= 2000)
                        {
                            // select worst
                            anotherTarget = allTargetList.Where(c => c != target).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        } else
                        {
                            // select best
                            anotherTarget = allTargetList.Where(c => c != target).OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();
                        }
                        if (anotherTarget != null)
                        {
                            Logger.DebugWriteLine("Sanctifire 3");
                            targetList.AddRange(new[] { target, anotherTarget });
                        }
                    }
                }

                // spsummon useful monster
                if (targetList.Count == 0 && Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                {
                    Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                    {
                        {CardId.BorreloadFuriousDragon, () => Enemy.GetSpellCount() > 0 },
                        {CardId.MirrorjadeTheIcebladeDragon, () => !Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) },
                        {CardId.AluberTheJesterOfDespia, () => !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) },
                        {CardId.SpringansKitt, () => !activatedCardIdList.Contains(CardId.SpringansKitt + 1) },
                    };

                    foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                    {
                        ClientCard target = allTargetList.FirstOrDefault(c => c.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            // select worst
                            ClientCard anotherTarget = allTargetList.Where(c => c != target).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                            targetList.Add(target);
                            targetList.Add(anotherTarget);
                            Logger.DebugWriteLine("Sanctifire 4");
                            break;
                        }
                    }
                }

                // avoid evenly match
                if (targetList.Count == 0 && Duel.Player == 1 && Enemy.Hand.Count > 0 && Enemy.GetMonsterCount() + Enemy.GetSpellCount() == 0 && (CurrentTiming & hintBattleStart) > 0)
                {
                    ClientCard summonTarget = allTargetList.OrderByDescending(c => c.Attack).FirstOrDefault();
                    Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                    {
                        {CardId.MirrorjadeTheIcebladeDragon, () => !Bot.HasInMonstersZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) && !Bot.HasInSpellZone(CardId.MirrorjadeTheIcebladeDragon, faceUp: true) },
                        {CardId.AluberTheJesterOfDespia, () => !activatedCardIdList.Contains(CardId.AluberTheJesterOfDespia) },
                    };
                    foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                    {
                        ClientCard target = allTargetList.FirstOrDefault(c => c.IsCode(pair.Key));
                        if (target != null && pair.Value())
                        {
                            summonTarget = target;
                            break;
                        }
                    }
                    if (summonTarget != null)
                    {
                        // select worst
                        ClientCard anotherTarget = allTargetList.Where(c => c != summonTarget).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        targetList.Add(summonTarget);
                        targetList.Add(anotherTarget);
                        Logger.DebugWriteLine("Sanctifire 5");
                    }
                }

                if (targetList.Count > 0)
                {
                    AI.SelectMaterials(targetList, HintMsg.SpSummon);
                    currentDestroyCardList.AddRange(targetList);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            // spsummon itself
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                List<ClientCard> botCards = new List<ClientCard> { Bot.MonsterZone[2], Bot.MonsterZone[5], Bot.MonsterZone[6] };
                List<ClientCard> enemyCards = new List<ClientCard> { Enemy.MonsterZone[2], Enemy.MonsterZone[5], Enemy.MonsterZone[6] };
                if (enemyCards.Any(c => c != null && (c.IsFloodgate() || c.IsMonsterDangerous()))) return true;
                return botCards.Select(c => c == null ? 0 : c.GetDefensePower()).Sum() < enemyCards.Select(c => c == null ? 0 : c.GetDefensePower()).Sum();
            }
            return false;
        }

        public bool MirrorjadeTheIcebladeDragonActivate()
        {
            if (Card.Location != CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                return true;
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                bool checkFlag = GetProblematicEnemyMonster(0, false, false, CardType.Monster) != null;
                if (Enemy.GetMonsterCount() > 0)
                {
                    checkFlag |= Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.LastChainPlayer != 0;
                    int enemyBattlePower = Enemy.BattlingMonster == null ? 0 : Enemy.BattlingMonster.GetDefensePower();
                    int botBattlePower = Bot.BattlingMonster == null ? 0 : Bot.BattlingMonster.GetDefensePower();
                    checkFlag |= enemyBattlePower > 0 && enemyBattlePower > botBattlePower && Duel.LastChainPlayer != 0 && !currentDestroyCardList.Contains(Enemy.BattlingMonster);
                    checkFlag |= DefaultOnBecomeTarget() && Duel.LastChainPlayer != 0;
                }
                if (Duel.CurrentChain.Any(c => c.IsCode(CardId.NibiruThePrimalBeing) && !DefaultCheckWhetherCardIdIsNegated(CardId.NibiruThePrimalBeing)))
                {
                    checkFlag |= Enemy.GetMonsterCount() > 0;
                    checkFlag |= Bot.HasInMonstersZone(new[] { CardId.TheBystialLubellion, CardId.DespianTragedy, CardId.TriBrigadeMercourier });
                }

                if (checkFlag)
                {
                    Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
                    {
                        {CardId.AlbionTheBrandedDragon,
                            () =>!sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)) && !DefaultCheckWhetherCardIdIsNegated(CardId.AlbionTheBrandedDragon)
                            && !(Duel.Player == 0 && Bot.HasInMonstersZone(CardId.AlbionTheBrandedDragon) && Bot.HasInGraveyard(CardId.TheBystialLubellion))},
                        {CardId.RindbrummTheStrikingDragon,
                            () => Duel.Player == 1 && Bot.Graveyard.Any(c => c.IsOriginalCode(CardId.FallenOfAlbaz)) && Bot.Hand.Count > 0
                                && !activatedCardIdList.Contains(CardId.FallenOfAlbaz) && !DefaultCheckWhetherCardIdIsNegated(CardId.FallenOfAlbaz)},
                        {CardId.TitanikladTheAshDragon, () => CheckRemainInDeck(CardId.GuidingQuemTheVirtuous) > 0},
                        {CardId.SprindTheIrondashDragon, () => CheckRemainInDeck(CardId.SpringansKitt) > 0},
                        {CardId.AlbaLenatusTheAbyssDragon, () => CheckRemainInDeck(CardId.FusionDeployment, CardId.BrandedFusion) > 0 },
                        {CardId.LubellionTheSearingDragon, () => true },
                        {CardId.AlbionTheSanctifireDragon, () => true }
                    };

                    foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
                    {
                        if (Bot.HasInExtra(pair.Key) && pair.Value())
                        {
                            AI.SelectCard(pair.Key);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool BorreloadFuriousDragonActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;

            // destroy dangerous card
            List<ClientCard> dangerList = GetProblematicEnemyCardList(true, false, CardType.Monster);
            if (dangerList.Count > 0)
            {
                ClientCard botTarget = Bot.GetMonsters().OrderBy(c => c.GetDefensePower() + (c.IsCode(albazFusionMonster) ? 1 : 0)).FirstOrDefault();
                if (botTarget != null)
                {
                    AI.SelectCard(botTarget);
                    AI.SelectNextCard(dangerList);
                    currentDestroyCardList.Add(botTarget);
                    currentDestroyCardList.Add(dangerList[0]);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }

            List<ClientCard> enemyTargetList = GetNormalEnemyTargetList(true, true, CardType.Monster);

            // become target
            if (Duel.LastChainPlayer == 1)
            {
                List<ClientCard> targetedBotMonsterList = Duel.LastChainTargets.Where(c => c.Location == CardLocation.MonsterZone && c.Controller == 0).ToList();
                ClientCard lastChainCard = Util.GetLastChainCard();
                // if it's a negate effect, only destroy not important monster
                if (lastChainCard != null && lastChainCard.IsCode(targetNegateIdList))
                {
                    targetedBotMonsterList = targetedBotMonsterList.Where(c => !c.IsCode(CardId.BlazingCartesiaTheVirtuous) || c.Attack < 2500).OrderBy(c => c.Attack).ToList();
                }
                if (targetedBotMonsterList.Count > 0)
                {
                    AI.SelectCard(targetedBotMonsterList);
                    AI.SelectNextCard(enemyTargetList);
                    currentDestroyCardList.Add(targetedBotMonsterList[0]);
                    currentDestroyCardList.Add(enemyTargetList[0]);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }

            // end phase
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
            {
                List<ClientCard> botTargetList = Bot.MonsterZone.Where(c => c != null && c.GetDefensePower() <= 2500).OrderBy(c => c.GetDefensePower()).ToList();
                if (botTargetList.Count > 0)
                {
                    AI.SelectCard(botTargetList);
                    AI.SelectNextCard(enemyTargetList);
                    currentDestroyCardList.Add(botTargetList[0]);
                    currentDestroyCardList.Add(enemyTargetList[0]);
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }

            return false;
        }

        public bool LubellionTheSearingDragonActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                int fusionTarget = LubellionTheSearingDragonFusionTarget(Bot.ExtraDeck, out _);
                if (fusionTarget > 0)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            return false;
        }

        public int LubellionTheSearingDragonFusionTarget(IList<ClientCard> cards, out ClientCard target)
        {
            target = null;
            bool hasAlbaz = Bot.Banished.Any(c => c != null && c.IsFaceup() && c.IsOriginalCode(CardId.FallenOfAlbaz));
            hasAlbaz |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.IsOriginalCode(CardId.FallenOfAlbaz));
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.MirrorjadeTheIcebladeDragon, () => hasAlbaz && !DefaultCheckWhetherCardIdIsNegated(CardId.MirrorjadeTheIcebladeDragon) },
                {CardId.GranguignolTheDuskDragon, () => Bot.HasInBanished(CardId.BlazingCartesiaTheVirtuous) },
                {CardId.BorreloadFuriousDragon, () =>
                {
                    int darkDragonCount = Bot.Banished.Where(c => c.IsFaceup() && c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark)
                            && !c.IsCode(cannotBeFusionMaterialIdList) ).Count();
                    darkDragonCount += Bot.Graveyard.Where(c => c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark)
                            && !c.IsCode(cannotBeFusionMaterialIdList)
                            && (Duel.Player == 1 || !CheckWhetherShouldKeepInGrave(c)) ).Count();
                    return darkDragonCount >= 2;
                } },
                {CardId.RindbrummTheStrikingDragon, () => hasAlbaz && Bot.HasInBanished(CardId.TriBrigadeMercourier) },
                {CardId.TitanikladTheAshDragon, () =>
                {
                    if (!hasAlbaz) return false;
                    ClientCard enemyMonster = Util.GetBestEnemyMonster(true);
                    ClientCard botMonster = Util.GetBestBotMonster(true);
                    int enemyPower = enemyMonster == null ? 0 : enemyMonster.GetDefensePower();
                    int botPower = botMonster == null ? 0 : botMonster.Attack;
                    if (enemyPower > 0 && enemyPower >= botPower)
                    {
                        List<ClientCard> materialList = new List<ClientCard>(Bot.Banished);
                        materialList.AddRange(Bot.Graveyard);
                        foreach (ClientCard material in materialList)
                        {
                            if (material != null && material.IsFaceup() && material.Attack >= 2500 && (2900 + material.Level >= enemyPower))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                } },
                {CardId.DespianQuaeritis, () => {
                    if (!Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.Attack > 2500 && !(c.HasType(CardType.Fusion) && c.Level >= 8))) return false;
                    bool checkFlag = Bot.Banished.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeDespain));
                    checkFlag |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeDespain));

                    return checkFlag;
                } },
                {CardId.AlbaLenatusTheAbyssDragon, () => hasAlbaz && !Bot.HasInExtra(CardId.AlbionTheBrandedDragon) },
                {CardId.AlbionTheBrandedDragon, () => {
                    if (activatedCardIdList.Contains(CardId.AlbionTheBrandedDragon) || DefaultCheckWhetherCardIdIsNegated(CardId.AlbionTheBrandedDragon)) return false;
                    bool checkFlag = Bot.Banished.Any(c => c != null && c.IsFaceup() && c.HasAttribute(CardAttribute.Light));
                    checkFlag |= Bot.Graveyard.Any(c => c != null && c.IsFaceup() && c.HasAttribute(CardAttribute.Light));

                    return checkFlag;
                }}
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                if (target != null && pair.Value())
                {
                    return pair.Key;
                }
            }

            target = null;
            return 0;
        }

        public bool AlbaLenatusTheAbyssDragonSpSummon()
        {
            // use albaz + enemy's dragon monster
            List<ClientCard> enemyDragon = Enemy.GetMonsters().Where(c => c != null && c.IsFaceup() && !c.IsCode(cannotBeFusionMaterialIdList) && c.HasRace(CardRace.Dragon)).ToList();
            if (enemyDragon.Count > 0)
            {
                bool successFlag = enemyDragon.Count > 1;
                int bestBotPower = Util.GetBestAttack(Bot);
                successFlag |= enemyDragon.Any(c => c.GetDefensePower() >= bestBotPower);
                if (successFlag)
                {
                    if (!enemyDragon.Any(c => c.IsCode(CardId.FallenOfAlbaz)))
                    {
                        ClientCard botAlbaz = Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.FallenOfAlbaz));
                        if (botAlbaz != null)
                        {
                            enemyDragon.Add(botAlbaz);
                        }
                    }
                    AI.SelectMaterials(enemyDragon);
                    return true;
                }
            }

            return false;
        }

        public bool AlbaLenatusTheAbyssDragonActivate()
        {
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
            activatedCardIdList.Add(Card.Id);
            return true;
        }

        public bool GranguignolTheDuskDragonActivate()
        {
            int desc = -1;
            if (ActivateDescription >= Util.GetStringId(CardId.GranguignolTheDuskDragon, 0))
            {
                desc = ActivateDescription - Util.GetStringId(CardId.GranguignolTheDuskDragon, 0);
            }
            Logger.DebugWriteLine("granguignol: " + desc.ToString());

            // send to GY
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.GranguignolTheDuskDragon, 0))
            {
                if (CheckWhetherNegated(true, true, CardType.Monster) || CheckWhetherWillbeRemoved()) return false;
                int checkId = GranguignolTheDuskDragonSendToGYTarget(null, out _);
                if (checkId > 0)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            // spsummon
            if (ActivateDescription == Util.GetStringId(CardId.GranguignolTheDuskDragon, 1))
            {
                if (CheckWhetherNegated(true, Card.Location == CardLocation.MonsterZone, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }

            return false;
        }

        public int GranguignolTheDuskDragonSendToGYTarget(IList<ClientCard> cards, out ClientCard target)
        {
            bool needSendBranded = Bot.HasInGraveyard(CardId.BrandedRetribution) && CheckRemainInDeck(CardId.BrandedFusion) > 0;
            if (CheckRemainInDeck(CardId.BrandedRetribution) > 0)
            {
                needSendBranded |= Bot.Graveyard.Any(c => c != null && c.HasType(CardType.Spell | CardType.Trap) && c.HasSetcode(SetcodeBranded)
                    && !(fusionToGYFlag && c.IsCode(CardId.BrightestBlazingBrandedKing, CardId.BrandedInHighSpirits)));
                needSendBranded |= !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon) && !CheckWhetherWillbeRemoved() && Bot.HasInHandOrInGraveyard(CardId.AlbionTheShroudedDragon);
                needSendBranded |= Duel.CurrentChain.Any(c => c.Controller == 0 && c.Location == CardLocation.Grave && c.IsCode(CardId.BystialSaronir));
            }
            List<KeyValuePair<int, Func<bool>>> checkList = new List<KeyValuePair<int, Func<bool>>>
            {
                new KeyValuePair<int, Func<bool>>(CardId.BystialSaronir, () => !activatedCardIdList.Contains(CardId.BystialSaronir) && needSendBranded),
                new KeyValuePair<int, Func<bool>>(CardId.AlbionTheShroudedDragon, () => !activatedCardIdList.Contains(CardId.AlbionTheShroudedDragon) && needSendBranded ),
                new KeyValuePair<int, Func<bool>>(CardId.AlbionTheBrandedDragon, () => !activatedCardIdList.Contains(CardId.AlbionTheBrandedDragon + 1) && !sendToGYThisTurn.Any(c => c.IsCode(CardId.AlbionTheBrandedDragon)) ),
                new KeyValuePair<int, Func<bool>>(CardId.TitanikladTheAshDragon, () => !activatedCardIdList.Contains(CardId.TitanikladTheAshDragon) && !sendToGYThisTurn.Any(c => c.IsCode(CardId.TitanikladTheAshDragon))
                    && CheckRemainInDeck(CardId.GuidingQuemTheVirtuous, CardId.FallenOfAlbaz) > 0),
                new KeyValuePair<int, Func<bool>>(CardId.TheBystialLubellion, () => Bot.HasInMonstersZone(new[] {CardId.AlbionTheBrandedDragon, CardId.TitanikladTheAshDragon})
                    && CheckRemainInDeck(CardId.BrandedLost, CardId.BrandedBeast) > 0 ),
                new KeyValuePair<int, Func<bool>>(CardId.SprindTheIrondashDragon, () => !activatedCardIdList.Contains(CardId.SprindTheIrondashDragon) && !sendToGYThisTurn.Any(c => c.IsCode(CardId.SprindTheIrondashDragon)) 
                    && CheckRemainInDeck(CardId.SpringansKitt) > 0),
                new KeyValuePair<int, Func<bool>>(CardId.DespianLuluwalilith, () => CheckRemainInDeck(CardId.BlazingCartesiaTheVirtuous, CardId.GuidingQuemTheVirtuous) > 0 ),
                new KeyValuePair<int, Func<bool>>(CardId.AlbionTheShroudedDragon, () => true ),
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkList)
            {
                if (cards == null)
                {
                    if ((CheckRemainInDeck(pair.Key) > 0 || Bot.HasInExtra(pair.Key)) && pair.Value())
                    {
                        target = null;
                        return pair.Key;
                    }
                } else
                {
                    ClientCard tg = cards.FirstOrDefault(c => c.IsOriginalCode(pair.Key));
                    if (tg != null && pair.Value())
                    {
                        target = tg;
                        return pair.Key;
                    }
                }
            }

            target = null;
            return 0;
        }

        public bool DespianQuaeritisActivate()
        {
            if (Card.Location == CardLocation.MonsterZone && Duel.Phase == DuelPhase.Main1 && !CheckWhetherNegated(true, true, CardType.Monster))
            {
                if ((CurrentTiming & hintTimingMainEnd) != 0 && Duel.Player == 1)
                {
                    int bestBotPower = Util.GetBestPower(Bot, false);
                    if (Enemy.GetMonsters().Any(c => c.GetDefensePower() >= bestBotPower && c.IsAttack() && !(c.HasType(CardType.Fusion) && c.Level >= 8))) {
                        activatedCardIdList.Add(Card.Id);
                        return true;
                    }
                }
                if (Duel.Player == 0 && Enemy.GetMonsters().Any(c => c.IsAttack() && !(c.HasType(CardType.Fusion) && c.Level >= 8)))
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            if (Card.Location != CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return true;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }
            return false;
        }

        public bool SprindTheIrondashDragonActivate()
        {
            // search
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }
            // destroy
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                int moveDest = SprindTheIrondashDragonMoveZone(0, Card);
                if (moveDest > 0)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            return false;
        }

        public int SprindTheIrondashDragonMoveZone(int available = 0, ClientCard selfCard = null)
        {
            int maxZone = -1;
            int maxValue = 0;
            for (int zoneId = 0; zoneId < 5; ++zoneId)
            {
                // check whether can move
                if (Bot.MonsterZone[zoneId] != null) continue;
                int zone = (int)System.Math.Pow(2, zoneId);
                if (available > 0 && (available & zone) == 0) continue;

                int currentValue = SprindTheIrondashDragonDestroyValue(zoneId, selfCard);
                if (currentValue > maxValue)
                {
                    maxZone = zone;
                    maxValue = currentValue;
                }
            }
            return maxZone;
        }

        public int SprindTheIrondashDragonDestroyValue(int zoneId, ClientCard selfCard = null)
        {
            int value = 0;
            if (zoneId == 1 || zoneId == 3)
            {
                ClientCard botMonsterInExtraZone = Bot.MonsterZone[(zoneId + 9) / 2];
                if (botMonsterInExtraZone != null && botMonsterInExtraZone != selfCard && botMonsterInExtraZone.IsFaceup()) value -= 5;

                ClientCard enemyMonserInExtraZone = Enemy.MonsterZone[(11 - zoneId) / 2];
                if (enemyMonserInExtraZone != null && enemyMonserInExtraZone.IsFaceup()) value += 2;
            }
            ClientCard botSpell = Bot.SpellZone[zoneId];
            if (botSpell != null && botSpell.IsFaceup()) value--;
            ClientCard enemyMonster = Enemy.MonsterZone[5 - zoneId];
            if (enemyMonster != null && enemyMonster.IsFaceup())
            {
                value++;
                if (enemyMonster.IsFloodgate() || enemyMonster.IsMonsterDangerous()) value += 5;
            }
            ClientCard enemySpell = Enemy.SpellZone[5 - zoneId];
            if (enemySpell != null && enemySpell.IsFaceup())
            {
                value++;
                if (enemySpell.IsFloodgate()) value += 5;
            }

            return value;
        }

        public bool TitanikladTheAshDragonActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            return false;
        }

        public bool RindbrummTheStrikingDragonActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                bool checkFlag = false;

                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null)
                {
                    checkFlag = Duel.LastChainPlayer == 1;
                    checkFlag |= Duel.LastChainPlayer == 0 && lastChainCard.IsCode(CardId.MirrorjadeTheIcebladeDragon) && lastChainCard.Location == CardLocation.MonsterZone && Enemy.GetMonsterCount() == 0;
                }

                if (checkFlag)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster) || Duel.CurrentChain.Any(c => c.Controller == 0 && c.IsCode(CardId.GuidingQuemTheVirtuous))) return false;

                ClientCard albaz = Bot.Graveyard.FirstOrDefault(c => c.IsOriginalCode(CardId.FallenOfAlbaz));
                bool checkFlag = Card.IsCanRevive() && Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
                bool albazFlag = CheckAlbazFusion(null, out List<ClientCard> materialList) && albaz != null && !spSummoningAlbaz;
                if (albazFlag)
                {
                    checkFlag |= materialList.Count > 1;
                    if (materialList.Count > 0)
                    {
                        ClientCard material = materialList[0];
                        checkFlag |= material.HasType(CardType.Ritual | CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link);
                        checkFlag |= material.IsFloodgate() || material.IsOneForXyz() || Util.GetWorstBotMonster()?.GetDefensePower() < material.Attack;
                        checkFlag |= Duel.Player == 1 && Duel.Phase == DuelPhase.End && Duel.LastChainPlayer == -1;
                    }
                }

                if (checkFlag)
                {
                    albaz = Bot.Graveyard.Where(c => c.IsCode(CardId.FallenOfAlbaz)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();

                    if (checkFlag && albaz != null)
                    {
                        spSummoningAlbaz = true;
                        activatedCardIdList.Add(Card.Id + 1);
                        AI.SelectCard(albaz);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AlbionTheBrandedDragonActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                int fusionTarget = AlbionTheBrandedDragonFusionTarget(Bot.ExtraDeck, out _);
                if (fusionTarget > 0)
                {
                    activatedCardIdList.Add(Card.Id);
                    return true;
                }
            }
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id + 1);
                return true;
            }
            return false;
        }

        public int AlbionTheBrandedDragonFusionTarget(IList<ClientCard> cards, out ClientCard target)
        {
            target = null;
            Dictionary<int, Func<bool>> checkDict = new Dictionary<int, Func<bool>>
            {
                {CardId.MirrorjadeTheIcebladeDragon, () => {
                    bool checkFlag = !CheckWhetherNegated() && CheckShouldNoMoreSpSummon();
                    checkFlag |= Bot.Graveyard.Any(c => c != null && !sendToGYThisTurn.Contains(c) && !c.IsCode(cannotBeFusionMaterialIdList)
                        && c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link));
                    return checkFlag;
                } },
                {CardId.LubellionTheSearingDragon, () =>
                {
                    if (!activatedCardIdList.Contains(CardId.LubellionTheSearingDragon))
                    {
                        List<ClientCard> checkMaterialList = new List<ClientCard>(Bot.GetMonsters());
                        checkMaterialList.AddRange(Bot.Graveyard);
                        bool albazChecked = false;
                        bool hasOriginalAlbaz = checkMaterialList.Any(c => c.IsOriginalCode(CardId.FallenOfAlbaz));
                        foreach (ClientCard checkCard in checkMaterialList)
                        {
                            if (!albazChecked && checkCard.IsCode(CardId.FallenOfAlbaz) && (!hasOriginalAlbaz || !checkCard.IsOriginalCode(CardId.AlbionTheShroudedDragon)))
                            {
                                albazChecked = true;
                                continue;
                            }
                            if (checkCard.HasAttribute(CardAttribute.Dark)) return true;
                        }
                        if (Bot.HasInHand(CardId.TriBrigadeMercourier) && Bot.Hand.Count >= 2) return true;
                    }
                    return false;
                } },
                {CardId.BorreloadFuriousDragon, () => {
                    if (Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                    {
                        int darkDragonCount = Bot.Graveyard.Where(c => c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark)
                            && !c.IsCode(cannotBeFusionMaterialIdList)
                            && (Duel.Player == 1 || !CheckWhetherShouldKeepInGrave(c)) ).Count();
                        if (Duel.Player == 1 && Bot.GetMonsters().Any(c => c.HasRace(CardRace.Dragon) && c.HasAttribute(CardAttribute.Dark) && !c.IsCode(cannotBeFusionMaterialIdList))) {
                            darkDragonCount ++;
                        }
                        return darkDragonCount >= 2;
                    }
                    return false;
                } },
                {CardId.AlbionTheSanctifireDragon, () =>
                {
                    ClientCard albaz = Bot.Graveyard.FirstOrDefault(c => c.IsCode(CardId.FallenOfAlbaz));
                    ClientCard lightSpellcaster = Bot.Graveyard.FirstOrDefault(c => c.HasRace(CardRace.SpellCaster) && c.HasAttribute(CardAttribute.Light));
                    int remainMonsterCount = Enemy.GetGraveyardMonsters().Count;
                    remainMonsterCount += Bot.Graveyard.Where(c => c.IsMonster() && c != albaz && c != lightSpellcaster).Count();
                    remainMonsterCount += Bot.HasInHand(_CardId.MaxxC) ? 1 : 0;
                    return remainMonsterCount >= 2;
                } },
                {CardId.RindbrummTheStrikingDragon, () => Bot.HasInGraveyard(CardId.TriBrigadeMercourier) },
                {CardId.DespianQuaeritis, () =>
                {
                    bool checkFlag = Enemy.MonsterZone.Any(c => c != null && c.IsFaceup() && c.Attack >= 2500 && !(c.HasType(CardType.Fusion) && c.Level >= 8));
                    if (checkFlag)
                    {
                        ClientCard despian = Bot.Graveyard.Where(c => c != null && c.HasSetcode(SetcodeDespain) && !CheckWhetherShouldKeepInGrave(c))
                            .OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        if (despian == null)
                        {
                            despian = Bot.MonsterZone.Where(c => c != null && c.HasSetcode(SetcodeDespain)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        }
                        if (despian != null)
                        {
                            return Bot.Graveyard.Any(c => c.HasAttribute(CardAttribute.Light | CardAttribute.Dark) && !CheckWhetherShouldKeepInGrave(c) && c != despian);
                        }
                    }

                    return false;
                } },
                {CardId.TitanikladTheAshDragon, () =>
                {
                    ClientCard albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsOriginalCode(CardId.FallenOfAlbaz));
                    if (albaz == null)
                    {
                        albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.FallenOfAlbaz));
                    }
                    foreach (ClientCard material in Bot.Graveyard)
                    {
                        if (material != null && material != albaz && material.IsMonster() && material.Attack >= 2500 && !material.IsCode(cannotBeFusionMaterialIdList))
                        {
                            bool checkFlag = !Util.IsTurn1OrMain2() && Enemy.GetMonsterCount() == 0;
                            checkFlag |= !CheckWhetherShouldKeepInGrave(material);
                            return checkFlag;
                        }
                    }
                    return false;
                } },
                {CardId.AlbaLenatusTheAbyssDragon, () =>
                {
                    if (Util.GetOneEnemyBetterThanMyBest() == null)
                    {
                        ClientCard albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsOriginalCode(CardId.FallenOfAlbaz));
                        if (albaz == null)
                        {
                            albaz = Bot.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.FallenOfAlbaz));
                        }
                        foreach (ClientCard material in Bot.Graveyard)
                        {
                            if (material != null && material != albaz && material.IsMonster() && material.HasRace(CardRace.Dragon) && !material.IsCode(cannotBeFusionMaterialIdList))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                } },
                {CardId.GranguignolTheDuskDragon, () =>
                {
                    if (cards == null) return false;
                    return true;
                } }
            };

            foreach (KeyValuePair<int, Func<bool>> pair in checkDict)
            {
                target = cards.FirstOrDefault(card => card.IsCode(pair.Key));
                if (target != null && pair.Value())
                {
                    return pair.Key;
                }
            }

            target = null;
            return 0;
        }

        public bool DespianLuluwalilithActivate()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (CheckWhetherNegated(true, true, CardType.Monster)) return false;
                activatedCardIdList.Add(Card.Id);
                return true;
            }
            if (Card.Location == CardLocation.Grave)
            {
                if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
                if (CheckRemainInDeck(CardId.GuidingQuemTheVirtuous, CardId.BlazingCartesiaTheVirtuous) > 0)
                {
                    activatedCardIdList.Add(Card.Id + 1);
                    return true;
                }
            }
            return false;
        }

        public bool SetForChimera()
        {
            if (Card.Level <= 4) return false;
            if (Bot.GetMonsterCount() > 0 || !Bot.HasInHandOrInSpellZone(CardId.BrandedInWhite) || !Bot.HasInExtra(CardId.GuardianChimera)) return false;
            if (DefaultCheckWhetherCardIdIsNegated(CardId.GuardianChimera) || CheckWhetherNegated(true, true, CardType.Monster)) return false;
            if (Enemy.MonsterZone.All(c => c == null) && Enemy.SpellZone.All(c => c == null)) return false;

            for (int handIndex1 = 0; handIndex1 < Bot.Hand.Count - 1; ++ handIndex1)
            {
                ClientCard hand1 = Bot.Hand[handIndex1];
                if (!hand1.IsMonster() || hand1.IsCode(Card.Id) || Card.IsCode(hand1.Id)) continue;
                for (int handIndex2 = handIndex1 + 1; handIndex2 < Bot.Hand.Count; ++ handIndex2)
                {
                    ClientCard hand2 = Bot.Hand[handIndex2];
                    if (!hand2.IsMonster()) continue;
                    if (hand2.IsCode(Card.Id) || Card.IsCode(hand2.Id) || hand2.IsCode(hand1.Id) || hand1.IsCode(hand2.Id)) continue;
                    bool checkFlag = Card.HasRace(CardRace.Dragon) || hand1.HasRace(CardRace.Dragon) || hand2.HasRace(CardRace.Dragon);
                    if (checkFlag)
                    {
                        summoned = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AdvanceSummon()
        {
            if (Card.Level < 5) return false;

            List<ClientCard> releaseGoal = Bot.MonsterZone.Where(c => c != null && c.IsFaceup() && !c.IsDisabled() && c.IsCode(10158145)).ToList();
            if (releaseGoal.Count > 0)
            {
                if (Card.Level <= 6)
                {
                    AI.SelectMaterials(releaseGoal);
                    summoned = true;
                    return true;
                }

                if (Card.Level >= 7)
                {
                    if (releaseGoal.Count < 2)
                    {
                        ClientCard anotherMaterial = Bot.MonsterZone.Where(c => c != null && !releaseGoal.Contains(c)).OrderBy(c => c.GetDefensePower()).FirstOrDefault();
                        if (anotherMaterial.GetDefensePower() > Card.Attack) return false;
                        releaseGoal.Add(anotherMaterial);
                    }
                    if (releaseGoal.Count >= 2)
                    {
                        AI.SelectMaterials(releaseGoal);
                        summoned = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            switch (Card.Id)
            {
                case CardId.BrandedInHighSpirits:
                    {
                        bool checkFlag = (Bot.HasInMonstersZone(CardId.GuidingQuemTheVirtuous)
                            || (CheckRemainInDeck(CardId.GuidingQuemTheVirtuous) > 0 && sendToGYThisTurn.Any(c => c.IsCode(CardId.TitanikladTheAshDragon))));
                        if (!checkFlag) return false;
                    }
                    break;
                case CardId.BrandedOpening:
                    {
                        bool checkFlag = CheckRemainInDeck(CardId.AluberTheJesterOfDespia, CardId.GuidingQuemTheVirtuous) > 0;
                        if (!checkFlag) return false;
                    }
                    break;
                case CardId.BrightestBlazingBrandedKing:
                    {
                        bool checkFlag = Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(albazFusionMonster));
                        if (!checkFlag) return false;
                    }
                    break;
                case CardId.BrandedBeast:
                    {
                        bool checkFlag = Bot.HasInGraveyard(CardId.BrandedLost) || Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.HasSetcode(SetcodeBystial));
                        checkFlag |= Bot.HasInHand(CardId.BystialSaronir) && (Enemy.Graveyard.Any(c => CheckBystialCanBanish(c)) || Bot.Graveyard.Any(c => CheckBystialCanBanish(c)));
                        if (!checkFlag) return false;
                    }
                    break;
                case CardId.BrandedRetribution:
                    {
                        bool checkFlag = Bot.Graveyard.Where(c => c.IsCode(albazFusionMonster)).Count() > 1;
                        checkFlag |= Bot.MonsterZone.Any(c => c != null && c.IsFaceup() && c.IsCode(albazFusionMonster));
                        if (!checkFlag) return false;
                    }
                    break;
                default:
                    break;
            }

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

        // for Sanctifire Dragon spsummoned monster
        public bool FloogateActivate()
        {
            if (Card.Owner != 1 && !Card.IsFloodgate()) return false;
            if (CheckWhetherNegated()) return false;
            if (Executors.Any(e => e != null && e.Type == ExecutorType.Activate && e.CardId == Card.Id)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard lastChainCard = Util.GetLastChainCard();
                if (lastChainCard != null && lastChainCard.IsFaceup() && CheckCanBeTargeted(lastChainCard, true, CardType.Monster)
                    && (lastChainCard.Location == CardLocation.MonsterZone || lastChainCard.Location == CardLocation.SpellZone))
                {
                    AI.SelectCard(lastChainCard);
                }
            }

            return Duel.LastChainPlayer == 1;
        }
    }
}