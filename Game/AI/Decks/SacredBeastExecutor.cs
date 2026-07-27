using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;
using static WindBot.Game.AI.Decks.TimeThiefExecutor;

namespace WindBot.Game.AI.Decks
{
    [Deck("SacredBeast", "AI_SacredBeast")]
    class SacredBeastExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int UnleashingTheSacredBeasts = 38776201;
            public const int HamonSacredBeastOfSinfulCatastrophe = 50251045;
            public const int RavielSacredBeastOfEndlessEternity = 96345184;
            public const int UriaSacredBeastOfCataclysmicFire = 23856331;
            public const int TheOrchestratorOfTheSacredBeasts = 22734799;
            public const int MartyrOfTheSacredBeasts = 59138498;
            public const int SkyfireOfTheSacredBeast = 1259915;
            public const int FallenParadiseOfTheSacredBeasts = 65861210;
            public const int DivineAbyssOfTheSacredBeast = 86132414;
            public const int DestructionChantOfTheSacredBeast = 50147815;

            public const int ThunderKingTheLightningstrikeKaiju = 48770333;
            public const int MulcharmyFuwalos = 42141493;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;
            public const int LightningCrash = 89753095;
            public const int HeavyPolymerization = 58570206;
            public const int CalledByTheGrave = 24224830;
            public const int CardOfTheSoul = 7044562;

            public const int PhantasmalSacredBeastsOfChaos = 7894706;
            public const int SuperVehicroidMobileBase = 17745969;
            public const int SaintAzamina = 85065943;
            public const int ThunderDragonColossus = 15291624;
            public const int SuperdreadnoughtRailCannonGustavRocket = 92359409;
            public const int SuperdreadnoughtRailCannonGustavMax = 56910167;
            public const int VarudrasTheFinalBringer = 70636044;
            public const int SPLittleKnight = 29301450;
            public const int Linkuriboh = 41999284;

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
        private readonly Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>
        {
            { 3, new List<int>
                {
                    CardId.UnleashingTheSacredBeasts,
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.RavielSacredBeastOfEndlessEternity,
                    CardId.MulcharmyFuwalos,
                    CardId.MartyrOfTheSacredBeasts,
                    CardId.SkyfireOfTheSacredBeast,
                    CardId.LightningCrash,
                    CardId.FallenParadiseOfTheSacredBeasts,
                    CardId.HeavyPolymerization,
                    CardId.DivineAbyssOfTheSacredBeast,
                    CardId.CardOfTheSoul
                }
            },

            { 2, new List<int>
                {
                    CardId.AshBlossom
                }
            },

            { 1, new List<int>
                {
                    CardId.UriaSacredBeastOfCataclysmicFire,
                    CardId.ThunderKingTheLightningstrikeKaiju,
                    CardId.TheOrchestratorOfTheSacredBeasts,
                    CardId.MaxxC,
                    CardId.CalledByTheGrave,
                    CardId.DestructionChantOfTheSacredBeast
                }
            },
        };

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;
        const int SetcodeDarkWorld = 0x6;
        const int SetcodeSkyStriker = 0x115;
        const int SetcodeSacredBeast = 0x1144;

        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        int SPLittleKnightRemoveStep = 0;

        int myTurnCount = 0;
        bool useHamonSearchEffectAlready = false;
        bool useLightningCrash = false;
        int paradise = 3;
        bool normalSummon = false;
        bool useRaviel = false;
        bool useOchestFromField = false;
        bool useOchestFromGY = false;
        bool Martyrx3 = false;
        bool resolvingUnleashing = false;
        bool resolvingUnleashingHamonLine = false;
        int fallenParadiseCostCode = 0;
        bool resolvingFallenParadise = false;
        int fallenParadiseTarget = 0;
        bool resolvingColossusSummon = false;
        bool resolvingRavielBoardWipe = false;
        bool resolvingSPLittleKnightSummon = false;
        List<ClientCard> spLittleKnightMaterialPlan = new List<ClientCard>();
        bool resolvingHeavyPolymerization = false;
        int heavyPolyMaterialPicked = 0;
        int heavyPolyMaterialNeed = 0;
        bool resolvingRank10Summon = false;
        List<ClientCard> rank10MaterialPlan = new List<ClientCard>();
        bool resolvingGustavRocketSummon = false;
        ClientCard gustavRocketDiscardPlan = null;
        bool gustavRocketDiscardSelected = false;
        bool gustavRocketMaxSelected = false;
        bool resolvingChantFusion = false;

        public SacredBeastExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.VarudrasTheFinalBringer, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.PhantasmalSacredBeastsOfChaos, PhantasmalSacredBeastsOfChaosActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPLittleKnightActivate);
            AddExecutor(ExecutorType.Activate, CardId.DivineAbyssOfTheSacredBeast, DivineAbyssActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestructionChantOfTheSacredBeast, DestructionChantActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CrossoutDesignator, CrossoutDesignatorActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, CardId.UriaSacredBeastOfCataclysmicFire, Uria_Field_DestroyST);
            AddExecutor(ExecutorType.Activate, CardId.RavielSacredBeastOfEndlessEternity, Raviel_Field_BoardWipeOnlyWithMartyr2);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, LinkuribohActivate);
            AddExecutor(ExecutorType.Activate, CardId.SuperdreadnoughtRailCannonGustavMax);
            AddExecutor(ExecutorType.Activate, CardId.SuperdreadnoughtRailCannonGustavRocket, GustavRocketActivate);

            AddExecutor(ExecutorType.Activate, CardId.UnleashingTheSacredBeasts, Unleashing_GY_Recovery);
            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_GY_EndPhaseRecovery);
            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_GY_EndPhaseRecovery);
            AddExecutor(ExecutorType.Activate, CardId.FallenParadiseOfTheSacredBeasts, FallenParadise_Field_Draw2AfterSetup);
            AddExecutor(ExecutorType.Activate, CardId.HeavyPolymerization, HeavyPolymerizationActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderKingTheLightningstrikeKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.Activate, CardId.CardOfTheSoul, CardOfTheSoul_Starter_SearchHamonOrRaviel);
            AddExecutor(ExecutorType.Activate, CardId.LightningCrash, LightningCrash_Starter_SearchHamon);
            AddExecutor(ExecutorType.Activate, CardId.HamonSacredBeastOfSinfulCatastrophe, Hamon_Hand_SearchSpell);
            AddExecutor(ExecutorType.Activate, CardId.UnleashingTheSacredBeasts, Unleashing_Main_Search3Discard2);
            AddExecutor(ExecutorType.Summon, CardId.MartyrOfTheSacredBeasts, MartyrSummon);
            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_OnSummon_Place);
            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_Field_Place2RevealPlaceParadise);
            AddExecutor(ExecutorType.Activate, CardId.RavielSacredBeastOfEndlessEternity, Raviel_Hand_SearchUria);
            AddExecutor(ExecutorType.Activate, CardId.UriaSacredBeastOfCataclysmicFire, Uria_Hand_SearchDestructionChant);
            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_Field_SummonTwoMartyr);
            AddExecutor(ExecutorType.Activate, CardId.TheOrchestratorOfTheSacredBeasts, Orchestrator_Field_ReviveRouteTarget);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus, ThunderDragonColossusSummon);
            AddExecutor(ExecutorType.Activate, CardId.TheOrchestratorOfTheSacredBeasts, Orchestrator_GY_ReviveLevel10);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPLittleKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, LinkuribohSummon);
            AddExecutor(ExecutorType.Activate, CardId.FallenParadiseOfTheSacredBeasts, FallenParadise_Field_SummonByCost3);
            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_Hand_ActivateCardOnly);
            AddExecutor(ExecutorType.SpSummon, CardId.PhantasmalSacredBeastsOfChaos, PhantasmalSacredBeastsOfChaosSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.VarudrasTheFinalBringer, VarudrasSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonGustavMax, GustavMaxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonGustavRocket, GustavRocketSummonOnMax);

            

            AddExecutor(ExecutorType.Repos, Repos);
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
            SPLittleKnightRemoveStep = 0;
            useLightningCrash = false;
            useHamonSearchEffectAlready = false;
            infiniteImpermanenceList.Clear();
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyPlaceThisTurn.Clear();
            paradise = 3;
            normalSummon = false;
            useRaviel = false;
            useOchestFromField = false;
            useOchestFromGY = false;
            resolvingUnleashing = false;
            resolvingUnleashingHamonLine = false;
            resolvingFallenParadise = false;
            fallenParadiseTarget = 0;
            fallenParadiseCostCode = 0;
            Martyrx3 = false;
            resolvingColossusSummon = false;
            resolvingRavielBoardWipe = false;
            resolvingSPLittleKnightSummon = false;
            spLittleKnightMaterialPlan.Clear();
            resolvingHeavyPolymerization = false;
            heavyPolyMaterialPicked = 0;
            heavyPolyMaterialNeed = 0;
            resolvingRank10Summon = false;
            rank10MaterialPlan.Clear();
            resolvingGustavRocketSummon = false;
            gustavRocketDiscardPlan = null;
            gustavRocketDiscardSelected = false;
            gustavRocketMaxSelected = false;
            resolvingChantFusion = false;

            base.OnNewTurn();
        }
        public override bool OnSelectHand() { return true; /* Go first by default.*/}
        public override int OnSelectOption(IList<int> options)
        {
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            Logger.DebugWriteLine($"OnSelectOption: CurrentSolving={currentSolvingChain} count={options.Count} options=[{string.Join(", ", options.Select((v, i) => $"{i}:{v}"))}]");
            if (Duel.Phase == DuelPhase.End && Duel.Player == 0 && Bot.HasInMonstersZone(CardId.SuperdreadnoughtRailCannonGustavRocket, true))
            {
                ClientCard rocket = Bot.GetMonsters()
                    .FirstOrDefault(c => c != null
                        && c.IsFaceup()
                        && c.IsCode(CardId.SuperdreadnoughtRailCannonGustavRocket));

                if (rocket != null && rocket.Overlays != null && rocket.Overlays.Count > 0)
                {
                    Logger.DebugWriteLine("Gustav Rocket End Phase: detach overlay YES");
                    return 0;
                }
            }
            if (resolvingFallenParadise)
            {
                Logger.DebugWriteLine("Fallen Paradise SelectYesNo: YES");
                return 0;
            }
            Logger.DebugWriteLine("OnSelectOption Default");
            return base.OnSelectOption(options);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            Logger.DebugWriteLine( "OnSelectCard " + cards.Count + " " + min + " " + max + " hint=" + hint  + " cancelable=" + cancelable + " cards=["+string.Join(", ", cards.Select(c => c == null ? "null" : $"{c.Name}({c.Id}) C{c.Controller} L{c.Location}")) + "]");

            ClientCard trigger = Util.GetLastChainCard();
            if (resolvingChantFusion)
            {
                if (hint == 509)
                {
                    ClientCard fusion = cards.FirstOrDefault(c =>
                        c != null
                        && c.Location == CardLocation.Extra
                        && c.IsCode(CardId.PhantasmalSacredBeastsOfChaos));

                    if (fusion != null)
                    {
                        Logger.DebugWriteLine("Chant GY fusion target: " + fusion.Id);
                        return new List<ClientCard> { fusion };
                    }
                }

                if (hint == 511)
                {
                    ClientCard material = cards
                        .Where(c => c != null && IsPhantasmalChaosMaterial(c))
                        // เอาจากมือก่อน เพื่อรักษาบอร์ด ถ้า core อนุญาต
                        .OrderBy(c => c.Location == CardLocation.Hand ? 0 : 10)
                        .ThenBy(c => ChantFusionMaterialScore(c))
                        .FirstOrDefault();

                    if (material != null)
                    {
                        Logger.DebugWriteLine("Chant GY fusion material pick: " + material.Id);
                        return new List<ClientCard> { material };
                    }

                    resolvingChantFusion = false;
                }
            }
            if (resolvingGustavRocketSummon)
            {
                // discard cost จากมือ
                ClientCard discard = null;

                if (!gustavRocketDiscardSelected)
                {
                    if (gustavRocketDiscardPlan != null && cards.Contains(gustavRocketDiscardPlan))
                        discard = gustavRocketDiscardPlan;

                    if (discard == null)
                    {
                        discard = cards
                            .Where(c => c != null && c.Location == CardLocation.Hand)
                            .OrderBy(c => DiscardScore(c, new HashSet<int>
                            {
                    CardId.UriaSacredBeastOfCataclysmicFire,
                    CardId.RavielSacredBeastOfEndlessEternity,
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.MartyrOfTheSacredBeasts,
                    CardId.DestructionChantOfTheSacredBeast,
                    CardId.UnleashingTheSacredBeasts
                            }))
                            .FirstOrDefault();
                    }

                    if (discard != null && discard.Location == CardLocation.Hand)
                    {
                        Logger.DebugWriteLine("Gustav Rocket discard cost pick: " + discard.Id);
                        gustavRocketDiscardSelected = true;

                        if (gustavRocketMaxSelected)
                        {
                            resolvingGustavRocketSummon = false;
                            gustavRocketDiscardPlan = null;
                        }

                        return new List<ClientCard> { discard };
                    }
                }

                // เลือก Gustav Max เป็นตัวให้ Rocket ทับ
                ClientCard gmax = cards.FirstOrDefault(c =>
                    c != null
                    && c.Controller == 0
                    && c.Location == CardLocation.MonsterZone
                    && c.IsFaceup()
                    && c.IsCode(CardId.SuperdreadnoughtRailCannonGustavMax));

                if (gmax != null)
                {
                    Logger.DebugWriteLine("Gustav Rocket overlay pick: Gustav Max");
                    gustavRocketMaxSelected = true;

                    if (gustavRocketDiscardSelected)
                    {
                        resolvingGustavRocketSummon = false;
                        gustavRocketDiscardPlan = null;
                    }

                    return new List<ClientCard> { gmax };
                }
            }
            if (resolvingRank10Summon && hint == 500)
            {
                List<ClientCard> picked = rank10MaterialPlan
                    .Where(c => c != null && cards.Contains(c))
                    .Take(max)
                    .ToList();

                if (picked.Count < min)
                {
                    picked = cards
                        .Where(c => c != null
                            && c.Controller == 0
                            && c.Location == CardLocation.MonsterZone
                            && c.IsFaceup()
                            && c.Level == 10)
                        .OrderBy(c => Rank10MaterialScore(c))
                        .Where(c => Rank10MaterialScore(c) < 9999)
                        .Take(max)
                        .ToList();
                }

                if (picked.Count >= min)
                {
                    Logger.DebugWriteLine("Rank10 material pick: "
                        + string.Join(", ", picked.Select(c => c.Id)));

                    foreach (ClientCard c in picked)
                        rank10MaterialPlan.Remove(c);

                    if (rank10MaterialPlan.Count == 0 || picked.Count >= 2)
                    {
                        resolvingRank10Summon = false;
                        rank10MaterialPlan.Clear();
                    }

                    return picked;
                }

                Logger.DebugWriteLine("Rank10 material no safe pick.");
                resolvingRank10Summon = false;
                rank10MaterialPlan.Clear();
            }
            if (hint == 527 && cards.Any(c => c != null && c.Location == CardLocation.Deck && (c.IsCode(CardId.DivineAbyssOfTheSacredBeast) || c.IsCode(CardId.FallenParadiseOfTheSacredBeasts) || c.IsCode(CardId.SkyfireOfTheSacredBeast))))
            {
                int target = 0;

                if (Duel.Player == 1)
                {
                    if (!Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast)
                        && CheckRemainInDeck(CardId.DivineAbyssOfTheSacredBeast) > 0)
                    {
                        target = CardId.DivineAbyssOfTheSacredBeast;
                    }
                }

                if (target == 0 && Duel.Player == 0)
                {
                    if (CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) > 0)
                        target = CardId.SkyfireOfTheSacredBeast;
                }

                ClientCard pick = cards.FirstOrDefault(c =>
                    c != null
                    && c.Location == CardLocation.Deck
                    && c.IsCode(target));

                if (pick != null)
                {
                    Logger.DebugWriteLine("Martyr place pick: " + pick.Id);
                    return new List<ClientCard> { pick };
                }
            }

            if (resolvingHeavyPolymerization)
            {
                if (hint == 509)
                {
                    ClientCard fusion = cards.FirstOrDefault(c =>
                        c != null
                        && c.Location == CardLocation.Extra
                        && c.IsCode(CardId.PhantasmalSacredBeastsOfChaos));

                    if (fusion != null)
                    {
                        Logger.DebugWriteLine("Heavy Poly fusion target: " + fusion.Id);
                        return new List<ClientCard> { fusion };
                    }
                }

                // เลือก material
                if (hint == 511)
                {
                    ClientCard zeroExtra = cards.FirstOrDefault(c =>
                        c != null
                        && c.Location == CardLocation.Extra
                        && (
                            c.IsCode(CardId.SuperVehicroidMobileBase)
                            || c.IsCode(CardId.SaintAzamina)
                        ));

                    if (zeroExtra != null)
                    {
                        heavyPolyMaterialPicked++;

                        Logger.DebugWriteLine("Heavy Poly material pick extra 0: " + zeroExtra.Id);

                        if (heavyPolyMaterialPicked >= heavyPolyMaterialNeed)
                        {
                            resolvingHeavyPolymerization = false;
                            heavyPolyMaterialPicked = 0;
                            heavyPolyMaterialNeed = 0;
                        }

                        return new List<ClientCard> { zeroExtra };
                    }

                    ClientCard ownSafe = cards
                        .Where(c => c != null
                            && !c.IsCode(CardId.PhantasmalSacredBeastsOfChaos)
                            && IsPhantasmalChaosMaterial(c))
                        .OrderBy(c => HeavyPolyOwnMaterialScore(c))
                        .FirstOrDefault();

                    if (ownSafe != null)
                    {
                        heavyPolyMaterialPicked++;

                        Logger.DebugWriteLine("Heavy Poly material pick own: " + ownSafe.Id);

                        if (heavyPolyMaterialPicked >= heavyPolyMaterialNeed)
                        {
                            resolvingHeavyPolymerization = false;
                            heavyPolyMaterialPicked = 0;
                            heavyPolyMaterialNeed = 0;
                        }

                        return new List<ClientCard> { ownSafe };
                    }

                    Logger.DebugWriteLine("Heavy Poly no safe material.");
                    resolvingHeavyPolymerization = false;
                    heavyPolyMaterialPicked = 0;
                    heavyPolyMaterialNeed = 0;
                }
            }
            if (resolvingSPLittleKnightSummon && hint == 533)
            {
                ClientCard pick = spLittleKnightMaterialPlan
                    .FirstOrDefault(c => c != null && cards.Contains(c));

                if (pick == null)
                {
                    pick = cards
                        .Where(c => c != null
                            && c.Controller == 0
                            && c.Location == CardLocation.MonsterZone)
                        .OrderBy(c => SPLittleKnightMaterialScore(c))
                        .FirstOrDefault(c => SPLittleKnightMaterialScore(c) < 9999);
                }

                if (pick != null)
                {
                    Logger.DebugWriteLine("S:P material pick: " + pick.Id);

                    spLittleKnightMaterialPlan.Remove(pick);

                    if (spLittleKnightMaterialPlan.Count == 0)
                        resolvingSPLittleKnightSummon = false;

                    return new List<ClientCard> { pick };
                }

                Logger.DebugWriteLine("S:P material no safe pick.");
                resolvingSPLittleKnightSummon = false;
                spLittleKnightMaterialPlan.Clear();
            }
            if (resolvingRavielBoardWipe)
            {
                List<ClientCard> martyrs = cards
                    .Where(c => c != null
                        && c.Controller == 0
                        && c.Location == CardLocation.MonsterZone
                        && c.IsFaceup()
                        && c.IsCode(CardId.MartyrOfTheSacredBeasts))
                    .Take(max)
                    .ToList();

                if (martyrs.Count >= min)
                {
                    Logger.DebugWriteLine("Raviel board wipe cost pick: "
                        + string.Join(", ", martyrs.Select(c => c.Id)));

                    if (martyrs.Count >= 2 || max == 1)
                        resolvingRavielBoardWipe = false;

                    return martyrs.Take(max).ToList();
                }

                resolvingRavielBoardWipe = false;
            }
            if (resolvingColossusSummon)
            {
                ClientCard orchest = cards.FirstOrDefault(c =>
                    c != null
                    && c.IsFaceup()
                    && c.IsCode(CardId.TheOrchestratorOfTheSacredBeasts)
                    && c.Location == CardLocation.MonsterZone);

                if (orchest != null)
                {
                    Logger.DebugWriteLine("Colossus material pick: Orchestrator");
                    resolvingColossusSummon = false;
                    return new List<ClientCard> { orchest };
                }
                resolvingColossusSummon = false;
            }
            // ===== Unleashing: prompt search / prompt discard =====
            if (resolvingUnleashing)
            {
                Logger.DebugWriteLine(
                    "Resolving Unleashing. HamonLine=" + resolvingUnleashingHamonLine
                    + " min=" + min
                    + " max=" + max
                    + " cards=[" + string.Join(", ", cards.Select(c =>
                        c == null ? "null" : $"{c.Id} L{c.Location}"
                    )) + "]"
                );

                int[] searchIds = resolvingUnleashingHamonLine
                    ? new[]
                    {
                        CardId.RavielSacredBeastOfEndlessEternity,
                        CardId.MartyrOfTheSacredBeasts,
                        CardId.TheOrchestratorOfTheSacredBeasts
                    }
                    : new[]
                    {
                        CardId.RavielSacredBeastOfEndlessEternity,
                        CardId.MartyrOfTheSacredBeasts,
                        CardId.HamonSacredBeastOfSinfulCatastrophe
                    };

                bool looksLikeSearchPrompt = cards.Any(c =>
                    c != null
                    && c.Location == CardLocation.Deck
                    && searchIds.Any(id => c.IsCode(id)));

                if (looksLikeSearchPrompt)
                {
                    List<ClientCard> picked = PickCardsByIdPriority(cards, searchIds, Math.Min(3, max));

                    if (picked.Count >= min)
                    {
                        Logger.DebugWriteLine(
                            "Unleashing search pick: "
                            + string.Join(", ", picked.Select(c => c.Id))
                        );

                        return picked;
                    }
                }
                bool looksLikeDiscardPrompt = cards.Any(c =>
                    c != null
                    && c.Location == CardLocation.Hand);

                if (looksLikeDiscardPrompt && min <= 2 && max >= 2)
                {
                    List<ClientCard> discard = new List<ClientCard>();

                    if (resolvingUnleashingHamonLine)
                    {
                        discard = PickCardsByIdPriority(cards, new[]
                        {
                            CardId.TheOrchestratorOfTheSacredBeasts,
                            CardId.HamonSacredBeastOfSinfulCatastrophe
                        }, 2);

                        if (discard.Count < 2)
                        {
                            HashSet<int> protect = new HashSet<int>
                                                                    {
                                                                        CardId.RavielSacredBeastOfEndlessEternity,
                                                                        CardId.MartyrOfTheSacredBeasts,
                                                                        CardId.UriaSacredBeastOfCataclysmicFire
                                                                    };

                            discard.AddRange(cards
                                .Where(c => c != null && c.Location == CardLocation.Hand && !discard.Contains(c))
                                .OrderBy(c => DiscardScore(
                                    c,
                                    protect,
                                    preferHamon: true,
                                    preferOrchestrator: true))
                                .Where(c => DiscardScore(
                                    c,
                                    protect,
                                    preferHamon: true,
                                    preferOrchestrator: true) < 9999)
                                .Take(2 - discard.Count));
                        }
                    }
                    else
                    {
                        HashSet<int> protect = new HashSet<int>
                                                                {
                                                                    CardId.RavielSacredBeastOfEndlessEternity,
                                                                    CardId.MartyrOfTheSacredBeasts,
                                                                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                                                                    CardId.UriaSacredBeastOfCataclysmicFire
                                                                };

                        discard = cards
                            .Where(c => c != null && c.Location == CardLocation.Hand)
                            .OrderBy(c => DiscardScore(c, protect))
                            .Where(c => DiscardScore(c, protect) < 9999)
                            .Take(2)
                            .ToList();
                    }

                    if (discard.Count >= 2)
                    {
                        Logger.DebugWriteLine(
                            "Unleashing discard pick: "
                            + string.Join(", ", discard.Take(2).Select(c => c.Id))
                        );

                        resolvingUnleashing = false;
                        resolvingUnleashingHamonLine = false;

                        return discard.Take(2).ToList();
                    }
                }
                Logger.DebugWriteLine("Unleashing prompt not handled, keep state.");
            }
            // ===== Fallen Paradise: cost 3 1 by 1 / summon target =====
            if (resolvingFallenParadise)
            {
                // cost prompt: Lua select cost hint=504
                if (hint == 504 && fallenParadiseCostCode != 0)
                {
                    ClientCard cost = cards.FirstOrDefault(c =>
                        c != null
                        && c.IsFaceup()
                        && c.IsCode(fallenParadiseCostCode)
                        && (
                            c.Location == CardLocation.SpellZone
                            || c.Location == CardLocation.MonsterZone
                        ));

                    if (cost != null)
                    {
                        Logger.DebugWriteLine("Fallen Paradise cost pick: " + cost.Id);
                        return new List<ClientCard> { cost };
                    }
                }

                // summon target prompt: hint=509
                if (hint == 509 && fallenParadiseTarget != 0)
                {
                    ClientCard target = cards.FirstOrDefault(c =>
                        c != null && c.IsCode(fallenParadiseTarget));

                    if (target != null)
                    {
                        Logger.DebugWriteLine("Fallen Paradise summon pick: " + target.Id);

                        resolvingFallenParadise = false;
                        fallenParadiseTarget = 0;
                        fallenParadiseCostCode = 0;

                        return new List<ClientCard> { target };
                    }
                }
            }
            if (trigger != null && trigger.IsCode(CardId.DestructionChantOfTheSacredBeast))
            {
                List<ClientCard> enemyTargets = cards
                    .Where(c => c != null && c.Controller == 1 && c.IsOnField())
                    .ToList();

                if (enemyTargets.Count > 0)
                {
                    ClientCard target = enemyTargets
                        .OrderByDescending(c => c.IsMonsterDangerous() ? 100 : 0)
                        .ThenByDescending(c => c.IsFloodgate() ? 80 : 0)
                        .ThenByDescending(c => c.Attack)
                        .FirstOrDefault();

                    if (target != null)
                        return new List<ClientCard> { target };
                }
            }

            if (trigger != null && trigger.IsCode(CardId.DivineAbyssOfTheSacredBeast))
            {
                List<ClientCard> enemyMonsterTargets = cards
                    .Where(c => c != null
                        && c.Controller == 1
                        && c.Location == CardLocation.MonsterZone
                        && c.IsFaceup())
                    .ToList();

                if (enemyMonsterTargets.Count > 0)
                {
                    ClientCard target = enemyMonsterTargets
                        .OrderByDescending(c => c.IsMonsterDangerous() ? 100 : 0)
                        .ThenByDescending(c => c.Attack)
                        .FirstOrDefault();

                    if (target != null)
                        return new List<ClientCard> { target };
                }

                List<ClientCard> abyssCopies = cards
                    .Where(c => c != null && c.IsCode(CardId.DivineAbyssOfTheSacredBeast))
                    .Take(max)
                    .ToList();

                if (abyssCopies.Count >= min)
                    return abyssCopies;
            }

            if (trigger != null && trigger.IsCode(CardId.SPLittleKnight))
            {
                List<ClientCard> targetList = cards
                    .Where(c => c != null && c.Controller == 1)
                    .OrderByDescending(c => c.IsMonsterDangerous() ? 100 : 0)
                    .ThenByDescending(c => c.IsFloodgate() ? 80 : 0)
                    .ThenByDescending(c => c.Attack)
                    .Take(max)
                    .ToList();

                if (targetList.Count >= min)
                    return targetList;
            }


            Logger.DebugWriteLine("Use default.");
            return base.OnSelectCard(cards, min, max, hint, cancelable);
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
                        SelectSTPlace(Card, true, avoid_list);
                        return true;
                    }
                }
                else
                {
                    SelectSTPlace(Card, true);
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
            if (Card.IsAttack() && Card.IsCode(CardId.MartyrOfTheSacredBeasts))
                return true;
            if (Card == null || Card.IsFacedown()) return false;
            if (Card.HasType(CardType.Link)) return false;

            return false;
        }
        #endregion
        #region Work Space
        private bool VarudrasActivate()
        {
            if (CheckWhetherNegated()) return false;

            List<ClientCard> targetList = GetNormalEnemyTargetList(true, true);
            int desc = ActivateDescription;
            int d1 = Util.GetStringId(CardId.VarudrasTheFinalBringer, 1);
            int d2 = Util.GetStringId(CardId.VarudrasTheFinalBringer, 2);

            var enemyPick = targetList.FirstOrDefault(c => c != null && c.Controller == 1);

            if (desc == d1 && Duel.LastChainPlayer == 1 && Duel.CurrentChain.Count > 0)
            {
                if (!CheckLastChainShouldNegated()) return false;
                return true; 
            }

            if (desc == d1 || desc == d2 || desc == -1)
            {
                List<ClientCard> enemyTargets = GetNormalEnemyTargetList(true, true)
                                                .Where(c => c != null && c.Controller == 1)
                                                .ToList();

                if (enemyTargets.Count == 0)
                    return false;

                AI.SelectCard(enemyTargets);
                return true;
            }

            return false;
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
                AI.SelectCard(problemCardList);
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 1))
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
                        if (selfMonster != null && !Util.IsOneEnemyBetterThanValue(selfMonster.Attack, true)) selfMonster = null;
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
                        return true;
                    }
                }
            }

            return false;
        }
        public bool SPLittleKnightSummon()
        {
            if (CheckWhetherNegated(true, true, CardType.Monster)) return false;

            bool forceForZone = ShouldForceSPLittleKnightForZone();

            if (!forceForZone && !HasSPLittleKnightTargetNow())
                return false;

            List<ClientCard> materials = PickSPLittleKnightMaterials();

            if (materials.Count < 2) return false;

            spLittleKnightMaterialPlan = materials.Take(2).ToList();
            resolvingSPLittleKnightSummon = true;

            AI.SelectMaterials(spLittleKnightMaterialPlan);
            return true;
        }
        private List<ClientCard> PickSPLittleKnightMaterials()
        {
            return Bot.GetMonsters()
                .Where(c => c != null
                    && c.IsFaceup()
                    && c.HasType(CardType.Effect))
                .Select(c => new
                {
                    Card = c,
                    Score = SPLittleKnightMaterialScore(c)
                })
                .Where(x => x.Score < 9999)
                .OrderBy(x => x.Score)
                .Select(x => x.Card)
                .Take(2)
                .ToList();
        }
        private int SPLittleKnightMaterialScore(ClientCard card)
        {
            if (card == null) return 9999;
            if (card.Level == 10 && card.HasSetcode(SetcodeSacredBeast))
                return 9999;

            if (card.IsCode(
                CardId.PhantasmalSacredBeastsOfChaos,
                CardId.VarudrasTheFinalBringer,
                CardId.ThunderDragonColossus,
                CardId.SuperdreadnoughtRailCannonGustavRocket,
                CardId.SuperdreadnoughtRailCannonGustavMax))
                return 9999;

            if (card.IsCode(CardId.TheOrchestratorOfTheSacredBeasts) && Bot.HasInExtra(CardId.ThunderDragonColossus))
                return 9999;

            if (card.IsCode(CardId.MartyrOfTheSacredBeasts))
                return 1;

            if (card.IsCode(CardId.Linkuriboh))
                return 2;

            if (card.IsCode(CardId.TheOrchestratorOfTheSacredBeasts))
                return 30;

            return 50;
        }
        public int CompareUsableAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA == null && cardB == null)
                return 0;
            if (cardA == null)
                return -1;
            if (cardB == null)
                return 1;
            int powerA = (cardA.IsDefense()) ? 0 : cardA.Attack;
            int powerB = (cardB.IsDefense()) ? 0 : cardB.Attack;
            if (powerA < powerB)
                return -1;
            if (powerA == powerB)
                return CardContainer.CompareCardLevel(cardA, cardB);
            return 1;
        }
        private bool IsMartyrNegatedOrInterrupted()
        {
            ClientCard martyr = Bot.GetMonsters()
                .FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.MartyrOfTheSacredBeasts));
            if (martyr == null) return false;
            if (DefaultCheckWhetherCardIsNegated(martyr) || martyr.IsDisabled()) return true;
            
            /*return CountFaceupSpellTrap(CardId.SkyfireOfTheSacredBeast) == 0
                && !Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)
                && HasCardAccessible(CardId.UnleashingTheSacredBeasts);*/
            return false;
        }
        private bool HasCardAccessible(int id)
        {
            return Bot.HasInHand(id)
                || Bot.HasInSpellZone(id, true)
                || Bot.HasInMonstersZone(id, true)
                || Bot.Graveyard.Any(c => c != null && c.IsCode(id));
        }
        private int CountFaceupSpellTrap(int id)
        {
            return Bot.GetSpells().Count(c => c != null && c.IsFaceup() && c.IsCode(id));
        }
        private bool LinkuribohSummon()
        {
            if (!IsMartyrNegatedOrInterrupted()) return false;

            ClientCard martyr = Bot.GetMonsters()
                .FirstOrDefault(c => c != null
                    && c.IsFaceup()
                    && c.Level == 1
                    && c.IsCode(CardId.MartyrOfTheSacredBeasts));

            if (martyr == null) return false;

            AI.SelectMaterials(new List<ClientCard> { martyr });
            return true;
        }
        private bool PhantasmalSacredBeastsOfChaosActivate()
        {
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;

            List<ClientCard> targetList = GetMonsterListForTargetNegate(true, CardType.Monster);
            if (targetList.Count == 0) return false;

            ClientCard target = targetList.FirstOrDefault(c =>
                c != null
                && c.Controller == 1
                && c.IsFaceup()
                && !c.IsDisabled()
                && c.HasType(CardType.Effect)
            );

            if (target == null) return false;

            if (Duel.LastChainPlayer != 1
                && !target.IsMonsterShouldBeDisabledBeforeItUseEffect()
                && !target.IsMonsterDangerous())
            {
                return false;
            }

            AI.SelectCard(target);
            currentNegateCardList.Add(target);
            return true;
        }
        private bool DivineAbyssActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.Player != 1) return false;
                if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer != 1)
                    return false;
                if (CountFaceupSpellTrap(CardId.DivineAbyssOfTheSacredBeast) < 3
                    && Bot.GetSpellCount() <= 3
                    && CheckRemainInDeck(CardId.DivineAbyssOfTheSacredBeast) > 0)
                {
                    AI.SelectCard(new List<int>{ CardId.DivineAbyssOfTheSacredBeast,
                                                 CardId.DivineAbyssOfTheSacredBeast });
                    return true;
                }
                List<ClientCard> targetList = GetNormalEnemyTargetList( canBeTarget: true, ignoreCurrentDestroy: true, selfType: CardType.Trap )
                .Where(c => c != null && c.Controller == 1 && c.Location == CardLocation.MonsterZone && c.IsFaceup()).ToList();

                if (targetList.Count == 0) return false;

                AI.SelectCard(targetList);
                return true;
            }

            if (Card.Location == CardLocation.Grave
                && Duel.Player == 1
                && Duel.Phase == DuelPhase.End)
            {
                return true;
            }

            return false;
        }
        private bool DestructionChantActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.LastChainPlayer != 1) return false;
                if (!CheckLastChainShouldNegated()) return false;
                if (!HasSacredBeastInGYForDestructionChant()) return false;

                if (!HasFreeMonsterZone()) return false;

                int summonTarget = PickDestructionChantSummonTarget();
                if (summonTarget == 0) return false;

                AI.SelectCard(summonTarget);
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                if (ShouldWaitRavielBoardWipe())
                    return false;
                
                if (!CanMakePhantasmalFusion()) return false;
                resolvingChantFusion = true;
                AI.SelectCard(CardId.PhantasmalSacredBeastsOfChaos);
                return true;
            }

            return false;
        }
        private bool HasSacredBeastInGYForDestructionChant()
        {
            return Bot.Graveyard.Any(c => c != null && c.HasSetcode(SetcodeSacredBeast));
        }
        private int PickDestructionChantSummonTarget()
        {
            if (CountLevel10MonstersOnField() >= 2
                && Bot.HasInGraveyard(CardId.MartyrOfTheSacredBeasts))
            {
                return CardId.MartyrOfTheSacredBeasts;
            }

            int[] priority =
            {
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.UriaSacredBeastOfCataclysmicFire
            };

            foreach (int id in priority)
            {
                if (Bot.HasInMonstersZone(id, true)) continue;
                if (Bot.HasInGraveyard(id)) return id;
            }
            return 0;
        }
        private bool CanMakePhantasmalFusion()
        {
            int materialCount = Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && IsPhantasmalChaosMaterial(c))
                                + Bot.Hand.Count(c => c != null && IsPhantasmalChaosMaterial(c));

            return materialCount >= 3;
        }
        private bool PhantasmalSacredBeastsOfChaosSummon()
        {
            return Bot.GetMonsters()
                .Count(c => c != null && c.IsFaceup() && IsPhantasmalChaosMaterial(c)) >= 3;
        }
        private bool IsPhantasmalChaosMaterial(ClientCard card)
        {
            return card != null
                && card.IsMonster()
                && card.Level == 10
                && !card.IsCode(CardId.PhantasmalSacredBeastsOfChaos)
                && (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)
                    || card.IsCode(CardId.RavielSacredBeastOfEndlessEternity)
                    || card.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)
                );
        }
        private int CountLevel10MonstersOnField()
        {
            return Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && c.Level == 10 && c.HasSetcode(SetcodeSacredBeast));
        }
        private bool HasFreeMonsterZone()
        {
            return Bot.GetMonstersInMainZone().Count(c => c != null) < 5;
        }
        private int CountFaceupMartyrOnField()
        {
            return Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && c.IsCode(CardId.MartyrOfTheSacredBeasts));
        }
        private bool CardOfTheSoul_Starter_SearchHamonOrRaviel()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Bot.LifePoints != 8000) return false;

            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(null, true);
            }
            bool hasHamon = Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe);
            bool hasRaviel = Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity);

            if (!hasHamon && !useHamonSearchEffectAlready && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            if ((hasHamon || useHamonSearchEffectAlready) && !hasRaviel && CheckRemainInDeck(CardId.RavielSacredBeastOfEndlessEternity) > 0)
            {
                AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                return true;
            }

            return false;
        }
        private bool LightningCrash_Starter_SearchHamon()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            bool hasHamon = Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe);
            bool hasKaiju = Bot.HasInHand(CardId.ThunderKingTheLightningstrikeKaiju);

            if (!hasHamon && !useHamonSearchEffectAlready && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                SelectSTPlace(null, true);
                useLightningCrash = true;
                return true;
            }

            if (Enemy.GetMonsterCount() > 0 && !hasKaiju && CheckRemainInDeck(CardId.ThunderKingTheLightningstrikeKaiju) > 0)
            {
                AI.SelectCard(CardId.ThunderKingTheLightningstrikeKaiju);
                SelectSTPlace(null, true);
                useLightningCrash = true;
                return true;
            }
            return false;
        }
        private bool Hamon_Hand_SearchSpell()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (useHamonSearchEffectAlready) return false;

            int searchTarget = PickHamonSpellSearchTarget();
            if (searchTarget == 0) return false;


            if (IsMartyrNegatedOrInterrupted())
            {
                AI.SelectCard(searchTarget);
                AI.SelectNextCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                useHamonSearchEffectAlready = true;
                return true;
            }

            bool result = QueueSearchThenDiscard( searchTarget, CardId.HamonSacredBeastOfSinfulCatastrophe, CardId.UriaSacredBeastOfCataclysmicFire);
            if (result) useHamonSearchEffectAlready = true;
            return result;
        }
        private int PickHamonSpellSearchTarget()
        {
            if (!HasCardAccessible(CardId.UnleashingTheSacredBeasts)
                && CheckRemainInDeck(CardId.UnleashingTheSacredBeasts) > 0)
                return CardId.UnleashingTheSacredBeasts;

            if (!HasCardAccessible(CardId.SkyfireOfTheSacredBeast)
                && CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) > 0)
                return CardId.SkyfireOfTheSacredBeast;

            if (!HasCardAccessible(CardId.FallenParadiseOfTheSacredBeasts)
                && CheckRemainInDeck(CardId.FallenParadiseOfTheSacredBeasts) > 0)
                return CardId.FallenParadiseOfTheSacredBeasts;

            if (CheckRemainInDeck(CardId.UnleashingTheSacredBeasts) > 0)
                return CardId.UnleashingTheSacredBeasts;

            if (CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) > 0)
                return CardId.SkyfireOfTheSacredBeast;

            if (CheckRemainInDeck(CardId.FallenParadiseOfTheSacredBeasts) > 0)
                return CardId.FallenParadiseOfTheSacredBeasts;

            return 0;
        }
        private bool QueueSearchThenDiscard(int searchTarget, params int[] protectedIds)
        {
            if (searchTarget == 0) return false;

            ClientCard discard = GetBestDiscardCost(protectedIds.Concat(new int[] { searchTarget }));
            if (discard == null) return false;

            AI.SelectCard(searchTarget);
            AI.SelectNextCard(discard);
            return true;
        }
        private ClientCard GetBestDiscardCost(IEnumerable<int> protectedIds = null, bool preferHamon = false, bool preferOrchestrator = false)
        {
            HashSet<int> protectedSet = protectedIds == null
                ? new HashSet<int>()
                : new HashSet<int>(protectedIds);

            return Bot.Hand
                .Where(c => c != null)
                .OrderBy(c => DiscardScore(c, protectedSet, preferHamon, preferOrchestrator))
                .FirstOrDefault(c => DiscardScore(c, protectedSet, preferHamon, preferOrchestrator) < 9999);
        }
        private int DiscardScore(ClientCard card, HashSet<int> protectedIds, bool preferHamon = false, bool preferOrchestrator = false)
        {
            if (card == null) return 9999;
            if (IsNeverDiscard(card.Id)) return 9999;
            if (protectedIds != null && protectedIds.Contains(card.Id)) return 9999;

            if (preferHamon && card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)) return -90;
            if (preferOrchestrator && card.IsCode(CardId.TheOrchestratorOfTheSacredBeasts)) return -100;

            bool duplicate = Bot.Hand.Count(c => c != null && c.Id == card.Id) >= 2;
            if (duplicate) return 0;

            if (card.IsCode(CardId.DivineAbyssOfTheSacredBeast)) return 1; // brick in hand
            if (card.IsCode(CardId.CardOfTheSoul) && Bot.LifePoints != 8000) return 2;
            if (card.IsCode(CardId.HeavyPolymerization) && Enemy.GetMonsterCount() == 0) return 3;
            if (card.IsCode(CardId.ThunderKingTheLightningstrikeKaiju) && Enemy.GetMonsterCount() == 0) return 4;
            if (card.IsCode(CardId.MulcharmyFuwalos) && Duel.Player == 0) return 5;
            if (card.IsCode(CardId.SkyfireOfTheSacredBeast) && CountFaceupSpellTrap(CardId.SkyfireOfTheSacredBeast) >= 2) return 6;
            if (card.IsCode(CardId.LightningCrash) && useLightningCrash) return 7;
            if (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe, CardId.RavielSacredBeastOfEndlessEternity)) return 50;
            if (card.IsCode(CardId.AshBlossom, CardId.MaxxC, CardId.CalledByTheGrave)) return 70;
            return 20;
        }
        private bool IsNeverDiscard(int id)
        {
            if (id == CardId.UriaSacredBeastOfCataclysmicFire) return true;

            return id == CardId.PhantasmalSacredBeastsOfChaos
                || id == CardId.ThunderDragonColossus
                || id == CardId.VarudrasTheFinalBringer
                || id == CardId.SPLittleKnight
                || id == CardId.SuperdreadnoughtRailCannonGustavRocket
                || id == CardId.SuperdreadnoughtRailCannonGustavMax;
        }
        private bool Unleashing_Main_Search3Discard2()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            resolvingUnleashing = true;
            resolvingUnleashingHamonLine = useHamonSearchEffectAlready;

            return true;
        }
        private bool Unleashing_GY_Recovery()
        {
            if (Card.Location != CardLocation.Grave) return false;

            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0
                && CheckRemainInDeck(CardId.UnleashingTheSacredBeasts) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            foreach (int id in SacredBeastMonsterSearchPriority())
            {
                if (Bot.HasInHand(id)) continue;

                AI.SelectCard(id);
                return true;
            }

            return false;
        }
        private int[] SacredBeastMonsterSearchPriority()
        {
            return new int[]
            {
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.UriaSacredBeastOfCataclysmicFire
            };
        }
        
        private bool MartyrSummon()
        {
            if ((Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe) ||
                Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity) ||
                Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire)) 
                && !Bot.HasInMonstersZone(CardId.MartyrOfTheSacredBeasts)
                && Bot.GetMonstersInMainZone().Count <= 2)
            {
                normalSummon = true;
                return true;
            }
            return false;
        }
        private bool Uria_Field_DestroyST()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
            if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer != 1) return false;

            List<ClientCard> targets = Enemy.GetSpells()
                .Where(c => c != null
                    && CheckCanBeTargeted(c, true, CardType.Monster)
                    && !currentDestroyCardList.Contains(c)
                    && !notToDestroySpellTrap.Contains(c.Id))
                .ToList();

            if (targets.Count == 0) return false;

            ClientCard target =
                targets.FirstOrDefault(c => c.IsFaceup() && c.IsFloodgate())
                ?? targets.FirstOrDefault(c => c.IsFaceup()
                    && c.HasType(CardType.Continuous | CardType.Field | CardType.Pendulum | CardType.Equip))
                ?? targets.FirstOrDefault(c => enemyPlaceThisTurn.Contains(c))
                ?? targets.FirstOrDefault(c => c.IsFacedown())
                ?? targets.FirstOrDefault();

            if (target == null) return false;

            AI.SelectCard(target);
            currentDestroyCardList.Add(target);
            return true;
        }
        private bool Martyr_OnSummon_Place()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            if (!(ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 0)))
                return false;

            if (Bot.GetSpellCount() >= 3) return false;

            int target = 0;

            if (Duel.Player == 1)
            {
                if (!Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast)
                    && CheckRemainInDeck(CardId.DivineAbyssOfTheSacredBeast) > 0)
                {
                    target = CardId.DivineAbyssOfTheSacredBeast;
                }
            }
            if (target == 0 && Duel.Player == 0)
            {
                if (CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) > 0)
                    target = CardId.SkyfireOfTheSacredBeast;
            }

            if (target == 0) return false;

            AI.SelectCard(target);
            return true;
        }
        private bool Skyfire_Field_Place2RevealPlaceParadise()
        {
            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe) &&
                !Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity) &&
                !Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire)) return false;
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Duel.Player != 0) return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
            int revealTarget = PickSkyfireRevealTarget();
            if (revealTarget == 0) return false;
            if (CountAccessibleSkyfireCopiesForEffect() < 2) return false;

            AI.SelectCard(new[]
            {
                CardId.SkyfireOfTheSacredBeast,
                CardId.SkyfireOfTheSacredBeast
            });
            AI.SelectNextCard(revealTarget);

            if (CheckRemainInDeck(CardId.FallenParadiseOfTheSacredBeasts) > 0)
                AI.SelectNextCard(CardId.FallenParadiseOfTheSacredBeasts);

            return true;
        }
        private int CountAccessibleSkyfireCopiesForEffect()
        {
            int count = CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast);
            count += Bot.Hand.Count(c => c != null && c.IsCode(CardId.SkyfireOfTheSacredBeast));
            count += Bot.Graveyard.Count(c => c != null && c.IsCode(CardId.SkyfireOfTheSacredBeast));
            return count;
        }
        private int PickSkyfireRevealTarget()
        {
            if (Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity))
                return CardId.RavielSacredBeastOfEndlessEternity;

            if (Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe))
                return CardId.HamonSacredBeastOfSinfulCatastrophe;

            if (Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire))
                return CardId.UriaSacredBeastOfCataclysmicFire;

            return 0;
        }
        private bool Skyfire_GY_EndPhaseRecovery()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Duel.Player != 1 || Duel.Phase != DuelPhase.End) return false;
            SelectSTPlace(null, true);
            return true;
        }
        private bool FallenParadise_Field_Draw2AfterSetup()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            int drawDesc = Util.GetStringId(CardId.FallenParadiseOfTheSacredBeasts, 1);
            if (ActivateDescription != drawDesc) return false;

            if (!Bot.HasInMonstersZone(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && !Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity)
                && !Bot.HasInMonstersZone(CardId.UriaSacredBeastOfCataclysmicFire))
            {
                return false;
            }

            return true;
        }
        private bool ThunderDragonColossusSummon()
        {
            if (!useHamonSearchEffectAlready) return false;
            if (!useOchestFromField) return false;

            ClientCard orchest = Bot.GetMonsters()
                .FirstOrDefault(c => c != null
                    && c.IsFaceup()
                    && c.IsCode(CardId.TheOrchestratorOfTheSacredBeasts));

            if (orchest == null) return false;

            resolvingColossusSummon = true;

            AI.SelectMaterials(new List<ClientCard> { orchest });
            return true;
        }
        private bool Raviel_Hand_SearchUria()
        {
            if (useRaviel) return false;
            if (Card.Location != CardLocation.Hand) return false;
            if (!Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire) &&
                CheckRemainInDeck(CardId.UriaSacredBeastOfCataclysmicFire) > 0 &&
                useHamonSearchEffectAlready
                )
            {
                AI.SelectCard(CardId.UriaSacredBeastOfCataclysmicFire);
                AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                useRaviel = true;
                return true;
            }
            else if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe) &&
                CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0 &&
                !useHamonSearchEffectAlready
                )
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                useRaviel = true;
                return true;
            }
            else if ( useHamonSearchEffectAlready && 
                      !normalSummon &&
                      !Bot.HasInHand(CardId.MartyrOfTheSacredBeasts) &&
                      (CheckRemainInDeck(CardId.MartyrOfTheSacredBeasts) > 0) &&
                      HasOtherSacredBeastInHandForRavielCost())
            {
                AI.SelectCard(CardId.MartyrOfTheSacredBeasts);
                AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                useRaviel = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool HasOtherSacredBeastInHandForRavielCost()
        {
            return Bot.Hand.Any(c => c != null
                && !ReferenceEquals(c, Card)
                && (
                    c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)
                    || c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)
                    || c.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)
                ));
        }
        private bool Raviel_Field_BoardWipeOnlyWithMartyr2()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;

            int wipeDesc = Util.GetStringId(CardId.RavielSacredBeastOfEndlessEternity, 1);
            if (ActivateDescription != wipeDesc && ActivateDescription != -1) return false;

            if (Enemy.GetMonsterCount() <= 0) return false;

            if (CountFaceupMartyrOnField() < 2) return false;

            resolvingRavielBoardWipe = true;
            return true;
        }
        private bool Martyr_GY_EndPhaseRecovery()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Duel.Player != 1 || Duel.Phase != DuelPhase.End) return false;
            return true;
        }
        private bool Uria_Hand_SearchDestructionChant()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckRemainInDeck(CardId.DestructionChantOfTheSacredBeast) <= 0) return false;

            ClientCard discard = GetBestDiscardCost(new int[]
            {
                CardId.UriaSacredBeastOfCataclysmicFire,
                CardId.DestructionChantOfTheSacredBeast
            });
            if (discard == null) return false;

            AI.SelectCard(CardId.DestructionChantOfTheSacredBeast);
            AI.SelectNextCard(discard);
            return true;
        }
        private bool Martyr_Field_SummonTwoMartyr()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            if (ActivateDescription != Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 1)) return false;
            if (Bot.GetMonstersInMainZone().Count(c => c != null) >= 3) return false;
            if (CheckRemainInDeck(CardId.MartyrOfTheSacredBeasts) + Bot.Graveyard.Count(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)) < 2) return false;

            AI.SelectCard(new[] { CardId.MartyrOfTheSacredBeasts, CardId.MartyrOfTheSacredBeasts });
            Martyrx3 = true;
            return true;
        }
        private bool Skyfire_Hand_ActivateCardOnly()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)) return false;
            if (PickSkyfireRevealTarget() == 0) return false;

            SelectSTPlace(null, true);
            return true;
        }
        private bool Orchestrator_Field_ReviveRouteTarget()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;
            if (!HasFreeMonsterZone()) return false;

            int target = 0;
            if (IsMartyrNegatedOrInterrupted() && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)))
                target = CardId.MartyrOfTheSacredBeasts;
            else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
                target = CardId.RavielSacredBeastOfEndlessEternity;
            else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
                target = CardId.HamonSacredBeastOfSinfulCatastrophe;
            else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)))
                target = CardId.UriaSacredBeastOfCataclysmicFire;
            else if (HasSacredBeastInHand())
                target = SacredBeastMonsterPriority().FirstOrDefault(id => Bot.HasInHand(id));

            if (target == 0) return false;

            ClientCard discard = GetBestDiscardCost(new int[] { CardId.UriaSacredBeastOfCataclysmicFire, target });
            if (discard == null) return false;

            AI.SelectCard(discard);
            AI.SelectNextCard(target);
            useOchestFromField = true;
            return true;
        }
        private bool HasSacredBeastInHand()
        {
            return Bot.Hand.Any(IsSacredBeastMonster);
        }
        private bool IsSacredBeastMonster(ClientCard card)
        {
            return card != null && card.HasSetcode(SetcodeSacredBeast);
        }
        private int[] SacredBeastMonsterPriority()
        {
            return new int[]
            {
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.UriaSacredBeastOfCataclysmicFire,
                CardId.MartyrOfTheSacredBeasts
            };
        }
        private bool Orchestrator_GY_ReviveLevel10()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (!HasFreeMonsterZone()) return false;

            if (useHamonSearchEffectAlready)
            {

                if (!Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity) && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
                {
                    AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                    useOchestFromGY = true;
                    return true;
                }

                if (!Bot.HasInMonstersZone(CardId.HamonSacredBeastOfSinfulCatastrophe) && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
                {
                    AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                    useOchestFromGY = true;
                    return true;
                }
            }

            return false;
        }
        private bool HeavyPolymerizationActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            int enemyMonsterCount = Enemy.GetMonsterCount();

            if (enemyMonsterCount < 2) return false;

            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(null, true);
            }

            int zeroExtraCount = CountInExtraDeck(CardId.SuperVehicroidMobileBase) + CountInExtraDeck(CardId.SaintAzamina);

            if (zeroExtraCount < 2) return false;

            if (CountInExtraDeck(CardId.PhantasmalSacredBeastsOfChaos) <= 0)
                return false;

            resolvingHeavyPolymerization = true;
            heavyPolyMaterialPicked = 0;
            heavyPolyMaterialNeed = 3;

            AI.SelectCard(CardId.PhantasmalSacredBeastsOfChaos);
            return true;
        }
        private int CountInExtraDeck(int id)
        {
            return Bot.ExtraDeck.Count(c => c != null && c.IsCode(id));
        }
        private int HeavyPolyOwnMaterialScore(ClientCard card)
        {
            if (card == null) return 9999;

            if (card.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)) return 100;
            if (card.IsCode(CardId.RavielSacredBeastOfEndlessEternity)) return 10;
            if (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)) return 20;

            return 50;
        }
        private bool FallenParadise_Field_SummonByCost3()
        {
            if (paradise <= 0) return false;
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            int summonDesc = Util.GetStringId(CardId.FallenParadiseOfTheSacredBeasts, 0);
            if (ActivateDescription != summonDesc) return false;

            if (!HasFreeMonsterZone()) return false;

            int costCode = PickFallenParadiseCostCode();
            if (costCode == 0) return false;

            int target = PickFallenParadiseSummonTarget();
            if (target == 0) return false;

            resolvingFallenParadise = true;
            fallenParadiseTarget = target;
            fallenParadiseCostCode = costCode;

            paradise--;
            return true;
        }
        private int PickFallenParadiseSummonTarget()
        {
            if (!Bot.HasInMonstersZone(CardId.TheOrchestratorOfTheSacredBeasts))
                return CardId.TheOrchestratorOfTheSacredBeasts;

            if (!Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity))
                return CardId.RavielSacredBeastOfEndlessEternity;

            if (!Bot.HasInMonstersZone(CardId.HamonSacredBeastOfSinfulCatastrophe))
                return CardId.HamonSacredBeastOfSinfulCatastrophe;

            return SacredBeastMonsterSearchPriority().FirstOrDefault();
        }
        private List<ClientCard> PickCardsByIdPriority(IList<ClientCard> cards, IEnumerable<int> ids, int count)
        {
            List<ClientCard> result = new List<ClientCard>();

            foreach (int id in ids)
            {
                foreach (ClientCard card in cards.Where(c => c != null && c.IsCode(id)))
                {
                    if (result.Contains(card)) continue;

                    result.Add(card);
                    if (result.Count >= count)
                        return result;
                }
            }

            return result;
        }
        private int PickFallenParadiseCostCode()
        {
            if (Bot.GetSpells().Count(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.SkyfireOfTheSacredBeast)) >= 3)
            {
                return CardId.SkyfireOfTheSacredBeast;
            }
            if (Bot.GetMonsters().Count(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.MartyrOfTheSacredBeasts)
                && !DefaultCheckWhetherCardIsNegated(c)
                && !c.IsDisabled()) >= 3)
            {
                return CardId.MartyrOfTheSacredBeasts;
            }
            if (Bot.GetSpells().Count(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.DivineAbyssOfTheSacredBeast)
                && !DefaultCheckWhetherCardIsNegated(c)
                && !c.IsDisabled()) >= 3)
            {
                return CardId.DivineAbyssOfTheSacredBeast;
            }

            return 0;
        }
        private bool HasSPLittleKnightTargetNow()
        {
            if (GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count > 0)
                return true;

            if (GetProblematicEnemyMonster(0, true, false, CardType.Monster) != null)
                return true;

            return Enemy.GetMonsterCount() > 0
                || Enemy.GetSpellCount() > 0
                || Enemy.Graveyard.Count > 0;
        }
        private bool ShouldForceSPLittleKnightForZone()
        {
            if (Duel.Turn != 1) return false;
            if (Duel.Player != 0) return false;
            if (!Martyrx3) return false;

            if (Bot.GetMonstersInMainZone().Count(c => c != null) < 5)
                return false;

            int martyrCount = Bot.GetMonstersInMainZone().Count(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.MartyrOfTheSacredBeasts));

            bool hasOrchestrator = Bot.GetMonstersInMainZone().Any(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.TheOrchestratorOfTheSacredBeasts));

            int level10Count = Bot.GetMonstersInMainZone().Count(c =>
                c != null
                && c.IsFaceup()
                && c.Level == 10
                && c.HasSetcode(SetcodeSacredBeast));

            return martyrCount >= 3
                && hasOrchestrator
                && level10Count >= 1;
        }
        private bool ShouldWaitRavielBoardWipe()
        {
            if (Duel.Player != 1) return false;
            if (Enemy.GetMonsterCount() <= 0) return false;

            ClientCard raviel = Bot.GetMonsters()
                .FirstOrDefault(c => c != null
                    && c.IsFaceup()
                    && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity));

            if (raviel == null) return false;
            if (DefaultCheckWhetherCardIsNegated(raviel) || raviel.IsDisabled()) return false;

            return CountFaceupMartyrOnField() >= 2;
        }

        private bool LinkuribohActivate()
        {
            Logger.DebugWriteLine(
    $"LinkuribohActivate loc={Card.Location} player={Duel.Player} phase={Duel.Phase} desc={ActivateDescription}"
);
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player != 1) return false;

                Logger.DebugWriteLine("Linkuriboh field effect: use on enemy attack.");
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                if (Duel.Player != 0) return false;
                if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;
                if (!ShouldUseLinkuribohGYFallback()) return false;

                ClientCard martyr = Bot.GetMonsters()
                    .FirstOrDefault(c => c != null
                        && c.IsFaceup()
                        && c.Level == 1
                        && c.IsCode(CardId.MartyrOfTheSacredBeasts));

                if (martyr == null) return false;

                AI.SelectCard(martyr);
                return true;
            }

            return false;
        }
        private bool ShouldUseLinkuribohGYFallback()
        {
            ClientCard martyr = Bot.GetMonsters()
                .FirstOrDefault(c => c != null
                    && c.IsFaceup()
                    && c.IsCode(CardId.MartyrOfTheSacredBeasts));

            if (martyr == null) return false;

            if (DefaultCheckWhetherCardIsNegated(martyr) || martyr.IsDisabled())
                return true;

            if (HasSPLittleKnightTargetNow()
                && Bot.HasInExtra(CardId.SPLittleKnight)
                && PickSPLittleKnightMaterials().Count < 2)
            {
                return true;
            }

            return false;
        }
        private bool VarudrasSummon()
        {
            if (Bot.HasInMonstersZone(CardId.VarudrasTheFinalBringer, true))
                return false;

            if (ShouldKeepCurrentBigBoard())
                return false;

            if (!ShouldSummonVarudras())
                return false;

            List<ClientCard> materials = PickRank10Materials();
            if (materials.Count < 2) return false;

            resolvingRank10Summon = true;
            rank10MaterialPlan = materials.Take(2).ToList();

            AI.SelectMaterials(rank10MaterialPlan);
            return true;
        }
        private bool ShouldSummonVarudras()
        {
            if (CountLevel10MonstersOnField() < 2)
                return false;

            if (Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity, true)
                && CountFaceupMartyrOnField() >= 2
                && Enemy.GetMonsterCount() > 0)
                return false;

            if (Duel.Player == 0 && (Enemy.GetMonsterCount() > 0 || Enemy.GetSpellCount() > 0))
                return true;

            if (IsGoingFirstRank10Situation() && !HasComfortableRocketCost())
                return true;

            if (CountLevel10MonstersOnField() >= 3)
                return true;

            return false;
        }
        private bool HasComfortableRocketCost()
        {
            ClientCard cost = GetBestRocketDiscardCost();
            if (cost == null) return false;

            return Bot.Hand.Count(c => c != null) >= 2;
        }

        private bool IsGoingFirstRank10Situation()
        {
            return Duel.Player == 0
                && Enemy.GetMonsterCount() == 0
                && Enemy.GetSpellCount() == 0;
        }
        private bool GustavMaxSummon()
        {
            if (Bot.HasInMonstersZone(CardId.SuperdreadnoughtRailCannonGustavMax, true))
                return false;

            if (ShouldKeepCurrentBigBoard())
                return false;

            if (CountLevel10MonstersOnField() < 2)
                return false;

            bool wantRocket = ShouldMakeGustavRocket();
            if (!wantRocket && Enemy.LifePoints > 2000)
                return false;

            if (ShouldSummonVarudras() && !wantRocket)
                return false;

            List<ClientCard> materials = PickRank10Materials();
            if (materials.Count < 2) return false;

            resolvingRank10Summon = true;
            rank10MaterialPlan = materials.Take(2).ToList();

            AI.SelectMaterials(rank10MaterialPlan);
            return true;
        }

        private List<ClientCard> PickRank10Materials()
        {
            return Bot.GetMonsters()
                .Where(c => c != null && c.IsFaceup() && c.Level == 10)
                .Select(c => new
                {
                    Card = c,
                    Score = Rank10MaterialScore(c)
                })
                .Where(x => x.Score < 9999)
                .OrderBy(x => x.Score)
                .Select(x => x.Card)
                .Take(2)
                .ToList();
        }

        private int Rank10MaterialScore(ClientCard card)
        {
            if (card == null) return 9999;
            if (!card.IsFaceup()) return 9999;
            if (card.Level != 10) return 9999;

            if (card.IsCode(
                CardId.PhantasmalSacredBeastsOfChaos,
                CardId.VarudrasTheFinalBringer,
                CardId.ThunderDragonColossus,
                CardId.SuperdreadnoughtRailCannonGustavRocket,
                CardId.SuperdreadnoughtRailCannonGustavMax))
                return 9999;

            if (card.IsCode(CardId.RavielSacredBeastOfEndlessEternity)
                && CountFaceupMartyrOnField() >= 2
                && Enemy.GetMonsterCount() > 0)
                return 9999;

            bool duplicate = Bot.GetMonsters()
                .Count(c => c != null && c.IsFaceup() && c.Id == card.Id) >= 2;

            if (duplicate) return 1;

            if (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)) return 10;
            if (card.IsCode(CardId.RavielSacredBeastOfEndlessEternity)) return 20;
            if (card.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)) return 30;

            return 50;
        }
        private bool GustavRocketSummonOnMax()
        {
            if (!Bot.HasInExtra(CardId.SuperdreadnoughtRailCannonGustavRocket))
                return false;

            ClientCard max = Bot.GetMonsters()
                .FirstOrDefault(c => c != null
                    && c.IsFaceup()
                    && c.IsCode(CardId.SuperdreadnoughtRailCannonGustavMax));

            if (max == null) return false;

            ClientCard discard = GetBestRocketDiscardCost();
            if (discard == null) return false;

            if (!ShouldMakeGustavRocket())
                return false;

            resolvingGustavRocketSummon = true;
            gustavRocketDiscardPlan = discard;
            gustavRocketDiscardSelected = false;
            gustavRocketMaxSelected = false;

            AI.SelectCard(discard);
            AI.SelectMaterials(new List<ClientCard> { max });
            return true;
        }
        private bool ShouldMakeGustavRocket()
        {
            if (!Bot.HasInExtra(CardId.SuperdreadnoughtRailCannonGustavRocket))
                return false;

            if (!HasComfortableRocketCost())
                return false;

            if (IsGoingFirstRank10Situation())
                return true;

            if (Enemy.GetMonsterCount() > 0)
                return true;

            if (Enemy.Graveyard.Any(c => c != null && c.IsMonster()))
                return true;

            return false;
        }
        private ClientCard GetBestRocketDiscardCost()
        {
            HashSet<int> protect = new HashSet<int>
            {
                CardId.UriaSacredBeastOfCataclysmicFire,
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.MartyrOfTheSacredBeasts,
                CardId.DestructionChantOfTheSacredBeast,
                CardId.UnleashingTheSacredBeasts
            };

            if (!Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast, true))
                protect.Add(CardId.DivineAbyssOfTheSacredBeast);

            return Bot.Hand
                .Where(c => c != null)
                .OrderBy(c => DiscardScore(c, protect))
                .FirstOrDefault(c => DiscardScore(c, protect) < 9999);
        }
        private bool GustavRocketActivate()
        {
            if (CheckWhetherNegated(true, false, CardType.Monster)) return false;
            if (Card.Location != CardLocation.MonsterZone) return false;

            int negateDesc = Util.GetStringId(CardId.SuperdreadnoughtRailCannonGustavRocket, 1);
            if (ActivateDescription != negateDesc && ActivateDescription != -1)
                return false;

            if (Card.Overlays == null || Card.Overlays.Count == 0)
                return false;

            if (Duel.LastChainPlayer != 1) return false;

            ClientCard last = Util.GetLastChainCard();
            if (last == null) return false;
            if (!last.IsMonster()) return false;

            if (!CheckLastChainShouldNegated()) return false;

            currentNegateCardList.Add(last);
            currentDestroyCardList.Add(last);
            return true;
        }
        private int ChantFusionMaterialScore(ClientCard card)
        {
            if (card == null) return 9999;

            if (card.IsCode(CardId.UriaSacredBeastOfCataclysmicFire)) return 1;

            if (card.IsCode(CardId.RavielSacredBeastOfEndlessEternity)) return 2;

            if (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)) return 3;

            return 50;
        }
        private bool ShouldKeepCurrentBigBoard()
        {
            bool hasRaviel = Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity);
            bool hasHamon = Bot.HasInMonstersZone(CardId.HamonSacredBeastOfSinfulCatastrophe);
            bool hasColossus = Bot.HasInMonstersZone(CardId.ThunderDragonColossus);
            bool hasChant = Bot.HasInHand(CardId.DestructionChantOfTheSacredBeast)
                || Bot.HasInSpellZone(CardId.DestructionChantOfTheSacredBeast);
            bool hasMartyr = Bot.HasInGraveyard(CardId.MartyrOfTheSacredBeasts);
            bool hasUria = Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire);

            return (hasRaviel && hasHamon && hasColossus && hasChant && hasMartyr && hasUria);
        }
        #endregion
    }
}
