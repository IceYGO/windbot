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
            public const int AmeNoMurakumoNoMitsurugi = 19899073;
            public const int DogmatikaMaximus = 95679145;
        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodePhantom = 0xdb;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeHorus = 0x19d;
        const int SetcodeDarkWorld = 0x6;
        const int SetcodeSkyStriker = 0x115;
        const int SetcodeSacredBeast = 0x1144;

        List<int> notToNegateIdList = new List<int> { 58699500, 20343502, 19403423 };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

        List<ClientCard> currentNegateCardList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> enemyPlaceThisTurn = new List<ClientCard>();
        int myTurnCount = 0;
        bool useHamonSearchEffectAlready = false;
        bool useLightningCrash = false;
        int paradise = 3;
        bool normalSummon = false;
        bool useRaviel = false;
        bool useOchestFromField = false;
        bool useOchestFromGY = false;
        bool Martyrx3 = false;
        bool unleashingHamonLinePlan = false;
        int fallenParadiseCostCode = 0;
        int fallenParadiseTarget = 0;
        bool resolvingColossusSummon = false;
        bool resolvingSPLittleKnightSummon = false;
        List<ClientCard> spLittleKnightMaterialPlan = new List<ClientCard>();
        bool resolvingRank10Summon = false;
        List<ClientCard> rank10MaterialPlan = new List<ClientCard>();
        bool resolvingGustavRocketSummon = false;
        ClientCard gustavRocketDiscardPlan = null;
        bool gustavRocketDiscardSelected = false;
        bool gustavRocketMaxSelected = false;

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
            useLightningCrash = false;
            useHamonSearchEffectAlready = false;
            currentNegateCardList.Clear();
            currentDestroyCardList.Clear();
            enemyPlaceThisTurn.Clear();
            paradise = 3;
            normalSummon = false;
            useRaviel = false;
            useOchestFromField = false;
            useOchestFromGY = false;
            unleashingHamonLinePlan = false;
            fallenParadiseTarget = 0;
            fallenParadiseCostCode = 0;
            Martyrx3 = false;
            resolvingColossusSummon = false;
            resolvingSPLittleKnightSummon = false;
            spLittleKnightMaterialPlan.Clear();
            resolvingRank10Summon = false;
            rank10MaterialPlan.Clear();
            resolvingGustavRocketSummon = false;
            gustavRocketDiscardPlan = null;
            gustavRocketDiscardSelected = false;
            gustavRocketMaxSelected = false;

            base.OnNewTurn();
        }
        public override void OnChainEnd()
        {
            // Clear planned selections even when an effect is negated or stops resolving early.
            unleashingHamonLinePlan = false;
            fallenParadiseTarget = 0;
            fallenParadiseCostCode = 0;

            base.OnChainEnd();
        }
        public override bool OnSelectHand() { return true; /* Go first by default.*/}
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.AmeNoMurakumoNoMitsurugi, 3))
            {
                bool shouldDiscard = Bot.Hand.Count >= 2;
                Logger.DebugWriteLine($"[MURAKUMO] Sacred Beast choose discard={shouldDiscard}, hand={Bot.Hand.Count}");
                return shouldDiscard;
            }

            return base.OnSelectYesNo(desc);
        }
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
            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.FallenParadiseOfTheSacredBeasts))
            {
                Logger.DebugWriteLine("Fallen Paradise SelectYesNo: YES");
                return 0;
            }
            Logger.DebugWriteLine("OnSelectOption Default");
            return base.OnSelectOption(options);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            ChainInfo currentSolvingChain = Duel.GetCurrentSolvingChainInfo();
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max + " hint=" + hint + " cancelable=" + cancelable + " cards=[" + string.Join(", ", cards.Select(c => c == null ? "null" : $"{c.Name}({c.Id}) C{c.Controller} L{c.Location}")) + "]");

            if (currentSolvingChain != null && currentSolvingChain.ActivatePlayer == 1)
            {
                if (currentSolvingChain.IsActivateCode(CardId.AmeNoMurakumoNoMitsurugi) 
                    && cards != null && cards.Count > 0 && (hint == HintMsg.Discard || hint == HintMsg.ToGrave) 
                    && cards.All(c => c != null && c.Controller == 0 && c.Location == CardLocation.Hand))
                {
                    HashSet<int> protect = new HashSet<int>();

                    ClientCard discard = cards.OrderBy(c => DiscardScore(c, protect)).FirstOrDefault(c => DiscardScore(c, protect) < 9999);

                    if (discard != null)
                    {
                        Logger.DebugWriteLine($"[MURAKUMO] Sacred Beast discard => " + $"{discard.Name}({discard.Id})");
                        return new List<ClientCard> { discard };
                    }
                }

                if (currentSolvingChain.IsActivateCode(CardId.DogmatikaMaximus) && hint == HintMsg.ToGrave)
                {
                    List<ClientCard> sendCards = cards.Where(c => c != null).OrderBy(c => ExtraDeckSendScore(c)).Take(min).ToList();
                    Logger.DebugWriteLine($"[DogmatikaMaximus] Sacred Beast send to grave => " + string.Join(", ", sendCards.Select(c => $"{c.Name}({c.Id})")));
                    return sendCards;
                }
            }

            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.DestructionChantOfTheSacredBeast)
                && currentSolvingChain.HasLocation(CardLocation.Grave)
                && min <= 1 && max == 1)
            {
                if (hint == HintMsg.SpSummon)
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

                if (hint == HintMsg.FusionMaterial)
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
                }
            }
            if (resolvingGustavRocketSummon
                && min <= 1 && max == 1)
            {
                // discard cost จากมือ
                ClientCard discard = null;

                if (!gustavRocketDiscardSelected
                    && hint == HintMsg.Discard)
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
                if (hint == HintMsg.XyzMaterial)
                {
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
            }
            if (resolvingRank10Summon && hint == HintMsg.XyzMaterial)
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
            if (hint == HintMsg.ToField
                && min <= 1 && max == 1
                && cards.Any(c => c != null && c.Location == CardLocation.Deck && (c.IsCode(CardId.DivineAbyssOfTheSacredBeast) || c.IsCode(CardId.FallenParadiseOfTheSacredBeasts) || c.IsCode(CardId.SkyfireOfTheSacredBeast))))
            {
                int target = 0;

                if (Duel.Player == 1)
                {
                    if (!Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast)
                        && Bot.HasInDeck(CardId.DivineAbyssOfTheSacredBeast))
                    {
                        target = CardId.DivineAbyssOfTheSacredBeast;
                    }
                }

                if (target == 0 && Duel.Player == 0)
                {
                    if (Bot.HasInDeck(CardId.SkyfireOfTheSacredBeast))
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

            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.HeavyPolymerization)
                && min <= 1 && max == 1)
            {
                if (hint == HintMsg.SpSummon)
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
                if (hint == HintMsg.FusionMaterial)
                {
                    ClientCard material = cards
                        .Where(c => c != null
                            && !c.IsCode(CardId.PhantasmalSacredBeastsOfChaos)
                            && (c.Location == CardLocation.Extra
                                && c.IsCode(CardId.SuperVehicroidMobileBase, CardId.SaintAzamina)
                                || IsPhantasmalChaosMaterial(c)))
                        .OrderBy(c => c.Location == CardLocation.Extra ? 0 : 10)
                        .ThenBy(c => HeavyPolyOwnMaterialScore(c))
                        .FirstOrDefault();

                    if (material != null)
                    {
                        Logger.DebugWriteLine("Heavy Poly material pick: " + material.Id);
                        return new List<ClientCard> { material };
                    }

                    Logger.DebugWriteLine("Heavy Poly no safe material.");
                }
            }
            if (resolvingSPLittleKnightSummon
                && hint == HintMsg.LinkMaterial
                && min <= 1 && max == 1)
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
            if (currentChainCard != null
                && currentChainCard.Controller == 0
                && currentChainCard.IsCode(CardId.RavielSacredBeastOfEndlessEternity)
                && hint == HintMsg.Release)
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

                    return martyrs.Take(max).ToList();
                }
            }
            if (resolvingColossusSummon
                && hint == HintMsg.Release
                && min <= 1 && max == 1)
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
            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.UnleashingTheSacredBeasts)
                && !currentSolvingChain.HasLocation(CardLocation.Grave))
            {
                Logger.DebugWriteLine(
                    "Resolving Unleashing. HamonLine=" + unleashingHamonLinePlan
                    + " min=" + min
                    + " max=" + max
                    + " cards=[" + string.Join(", ", cards.Select(c =>
                        c == null ? "null" : $"{c.Id} L{c.Location}"
                    )) + "]"
                );

                int[] searchIds = unleashingHamonLinePlan
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

                    if (unleashingHamonLinePlan)
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

                        unleashingHamonLinePlan = false;

                        return discard.Take(2).ToList();
                    }
                }
                Logger.DebugWriteLine("Unleashing prompt not handled, keep state.");
            }
            // ===== Fallen Paradise: cost 3 / summon target =====
            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.FallenParadiseOfTheSacredBeasts)
                && min <= 1 && max == 1)
            {
                if (hint == HintMsg.ToGrave
                    && fallenParadiseCostCode != 0)
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

                if (hint == HintMsg.SpSummon
                    && fallenParadiseTarget != 0)
                {
                    ClientCard target = cards.FirstOrDefault(c =>
                        c != null && c.IsCode(fallenParadiseTarget));

                    if (target != null)
                    {
                        Logger.DebugWriteLine("Fallen Paradise summon pick: " + target.Id);

                        fallenParadiseTarget = 0;
                        fallenParadiseCostCode = 0;

                        return new List<ClientCard> { target };
                    }
                }
            }
            if (currentSolvingChain != null
                && currentSolvingChain.ActivatePlayer == 0
                && currentSolvingChain.IsActivateCode(CardId.DestructionChantOfTheSacredBeast)
                && min <= 1 && max == 1)
            {
                if (!currentSolvingChain.HasLocation(CardLocation.Grave)
                    && hint == HintMsg.SpSummon)
                {
                    int preferredTarget = PickDestructionChantSummonTarget();
                    List<int> priorities = new List<int>();
                    if (preferredTarget != 0)
                        priorities.Add(preferredTarget);
                    priorities.AddRange(new[]
                    {
                        CardId.RavielSacredBeastOfEndlessEternity,
                        CardId.HamonSacredBeastOfSinfulCatastrophe,
                        CardId.UriaSacredBeastOfCataclysmicFire,
                        CardId.MartyrOfTheSacredBeasts
                    });

                    ClientCard summonTarget = PickCardsByIdPriority(cards, priorities, 1).FirstOrDefault();
                    if (summonTarget != null)
                    {
                        Logger.DebugWriteLine("Chant summon target: " + summonTarget.Id);
                        return new List<ClientCard> { summonTarget };
                    }
                }

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

            if ((currentChainCard != null
                    && currentChainCard.Controller == 0
                    && currentChainCard.IsCode(CardId.DivineAbyssOfTheSacredBeast))
                || (currentSolvingChain != null
                    && currentSolvingChain.ActivatePlayer == 0
                    && currentSolvingChain.IsActivateCode(CardId.DivineAbyssOfTheSacredBeast)))
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

                    if (target != null && min <= 1 && max == 1)
                        return new List<ClientCard> { target };
                }

                List<ClientCard> abyssCopies = cards
                    .Where(c => c != null && c.IsCode(CardId.DivineAbyssOfTheSacredBeast))
                    .Take(max)
                    .ToList();

                if (abyssCopies.Count >= min)
                    return abyssCopies;
            }

            Logger.DebugWriteLine("Use default.");
            return base.OnSelectCard(cards, min, max, hint, cancelable);
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card) || !CheckLastChainShouldNegated()) return false;
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }
        public bool CrossoutDesignatorActivate()
        {
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card) || !CheckLastChainShouldNegated()) return false;
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
                if (DefaultCheckWhetherCardEffectIsNegated(Util.GetLastChainCard())) return false;
                if (Bot.HasInDeck(code))
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
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
                    return true;
                }
            }
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card) || !CheckLastChainShouldNegated())
            {
                return false;
            }
            if (Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (DefaultCheckWhetherCardEffectIsNegated(Util.GetLastChainCard())) return false;
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
        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (avoidImpermanence && infiniteImpermanenceNegatedColumns.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            Util.ShuffleListInPlace(list);
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
        public bool CheckLastChainShouldNegated()
        {
            ChainInfo lastChainInfo = Duel.CurrentChainInfo.LastOrDefault();
            ClientCard lastcard = lastChainInfo?.RelatedCard;
            if (lastcard == null || lastChainInfo.ActivatePlayer != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            if (notToNegateIdList.Contains(lastcard.Id)) return false;
            if (DefaultCheckWhetherCardEffectIsNegated(lastChainInfo)) return false;
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
        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();

            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (floodagateList.Count > 0) resultList.AddRange(floodagateList);

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count > 0) resultList.AddRange(Util.ShuffleList(problemEnemySpellList));

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
            if (spells.Count > 0 && !ignoreSpells) resultList.AddRange(Util.ShuffleList(spells));

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
            targetList.AddRange(Util.ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(Util.ShuffleList(Enemy.GetSpells().Where(card => (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemyPlaceThisTurn.Contains(card)).ToList()));
            targetList.AddRange(Util.ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown() && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList()));

            return targetList;
        }
        public List<ClientCard> GetMonsterListForTargetNegate(bool canBeTarget = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card))
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
            if (monsters.Count > 0 && !onlyFaceup) return Util.ShuffleList(monsters)[0];
            return null;
        }
        public ClientCard GetBestEnemySpell(bool onlyFaceup = false, bool canBeTarget = false)
        {
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return Util.ShuffleList(problemEnemySpellList)[0];
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(card => !(card.IsFaceup() && card.IsCode(_CardId.EvenlyMatched))).ToList();

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() && (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field) || ecard.HasType(CardType.Pendulum))).ToList();
            if (faceUpList.Count > 0)
            {
                return Util.ShuffleList(faceUpList)[0];
            }

            if (spells.Count > 0 && !onlyFaceup)
            {
                return Util.ShuffleList(spells)[0];
            }

            return null;
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
                return Util.ShuffleList(Enemy.Graveyard.ToList())[0];
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

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
                        AI.SelectCard(selfMonster);
                        AI.SelectNextCard(nextMonster);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool SPLittleKnightSummon()
        {
            if (DefaultCheckWhetherCardWillBeNegatedOnField(Card)) return false;

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
            if (DefaultCheckWhetherCardEffectIsNegated(martyr)) return true;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.Player != 1) return false;
                if (Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer != 1)
                    return false;
                if (CountFaceupSpellTrap(CardId.DivineAbyssOfTheSacredBeast) < 3
                    && Bot.GetSpellCount() <= 3
                    && Bot.HasInDeck(CardId.DivineAbyssOfTheSacredBeast))
                {
                    return true;
                }
                List<ClientCard> targetList = GetNormalEnemyTargetList(canBeTarget: true, ignoreCurrentDestroy: true, selfType: CardType.Trap)
                .Where(c => c != null && c.Controller == 1 && c.Location == CardLocation.MonsterZone && c.IsFaceup()).ToList();

                if (targetList.Count == 0) return false;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                if (Duel.LastChainPlayer != 1) return false;
                if (!CheckLastChainShouldNegated()) return false;
                if (!HasSacredBeastInGYForDestructionChant()) return false;

                if (!HasFreeMonsterZone()) return false;

                int summonTarget = PickDestructionChantSummonTarget();
                if (summonTarget == 0) return false;

                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                if (ShouldWaitRavielBoardWipe())
                    return false;

                if (!CanMakePhantasmalFusion()) return false;
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
            if (Bot.LifePoints != 8000) return false;

            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(null, true);
            }
            bool hasHamon = Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe);
            bool hasRaviel = Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity);

            if (!hasHamon && !useHamonSearchEffectAlready && Bot.HasInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe))
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            if ((hasHamon || useHamonSearchEffectAlready) && !hasRaviel && Bot.HasInDeck(CardId.RavielSacredBeastOfEndlessEternity))
            {
                AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                return true;
            }

            return false;
        }
        private bool LightningCrash_Starter_SearchHamon()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            bool hasHamon = Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe);
            bool hasKaiju = Bot.HasInHand(CardId.ThunderKingTheLightningstrikeKaiju);

            if (!hasHamon && !useHamonSearchEffectAlready && Bot.HasInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe))
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                SelectSTPlace(null, true);
                useLightningCrash = true;
                return true;
            }

            if (Enemy.GetMonsterCount() > 0 && !hasKaiju && Bot.HasInDeck(CardId.ThunderKingTheLightningstrikeKaiju))
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

            bool result = QueueSearchThenDiscard(searchTarget, CardId.HamonSacredBeastOfSinfulCatastrophe, CardId.UriaSacredBeastOfCataclysmicFire);
            if (result) useHamonSearchEffectAlready = true;
            return result;
        }
        private int PickHamonSpellSearchTarget()
        {
            if (!HasCardAccessible(CardId.UnleashingTheSacredBeasts)
                && Bot.HasInDeck(CardId.UnleashingTheSacredBeasts))
                return CardId.UnleashingTheSacredBeasts;

            if (!HasCardAccessible(CardId.SkyfireOfTheSacredBeast)
                && Bot.HasInDeck(CardId.SkyfireOfTheSacredBeast))
                return CardId.SkyfireOfTheSacredBeast;

            if (!HasCardAccessible(CardId.FallenParadiseOfTheSacredBeasts)
                && Bot.HasInDeck(CardId.FallenParadiseOfTheSacredBeasts))
                return CardId.FallenParadiseOfTheSacredBeasts;

            if (Bot.HasInDeck(CardId.UnleashingTheSacredBeasts))
                return CardId.UnleashingTheSacredBeasts;

            if (Bot.HasInDeck(CardId.SkyfireOfTheSacredBeast))
                return CardId.SkyfireOfTheSacredBeast;

            if (Bot.HasInDeck(CardId.FallenParadiseOfTheSacredBeasts))
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
        private int ExtraDeckSendScore(ClientCard card)
        {
            // TODO(foohyfooh): Account for conditions regarding the remaining count of cards or existing board such as
            // - keep 3 copies of Level 10 with 0 ATK if bot has The Chaotic Phantasmal Sacred Beasts still and Heavy Polymerization in hand
            // - Dimensional Barrier or Grisaille Prison preventing specific summon
            if (card.IsCode(CardId.SuperVehicroidMobileBase)) return 1;
            if (card.IsCode(CardId.SaintAzamina)) return 2;
            if (card.IsCode(CardId.SuperdreadnoughtRailCannonGustavRocket)) return 3;
            if (card.IsCode(CardId.SuperdreadnoughtRailCannonGustavMax)) return 4;
            if (card.IsCode(CardId.ThunderDragonColossus)) return 5;
            if (card.IsCode(CardId.VarudrasTheFinalBringer)) return 6;
            if (card.IsCode(CardId.Linkuriboh)) return 7;
            if (card.IsCode(CardId.SPLittleKnight)) return 8;
            if (card.IsCode(CardId.PhantasmalSacredBeastsOfChaos)) return int.MaxValue;
            // Send unknown cards first
            return 0;
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            unleashingHamonLinePlan = useHamonSearchEffectAlready;

            return true;
        }
        private bool Unleashing_GY_Recovery()
        {
            if (Card.Location != CardLocation.Grave) return false;

            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && Bot.HasInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && Bot.HasInDeck(CardId.UnleashingTheSacredBeasts))
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            if (!(ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 0)))
                return false;

            if (Bot.GetSpellCount() >= 3) return false;

            int target = 0;

            if (Duel.Player == 1)
            {
                if (!Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast)
                    && Bot.HasInDeck(CardId.DivineAbyssOfTheSacredBeast))
                {
                    target = CardId.DivineAbyssOfTheSacredBeast;
                }
            }
            if (target == 0 && Duel.Player == 0)
            {
                if (Bot.HasInDeck(CardId.SkyfireOfTheSacredBeast))
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
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

            if (Bot.HasInDeck(CardId.FallenParadiseOfTheSacredBeasts))
                AI.SelectNextCard(CardId.FallenParadiseOfTheSacredBeasts);

            return true;
        }
        private int CountAccessibleSkyfireCopiesForEffect()
        {
            int count = Bot.GetCardCountInDeck(CardId.SkyfireOfTheSacredBeast);
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

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
                Bot.HasInDeck(CardId.UriaSacredBeastOfCataclysmicFire) &&
                useHamonSearchEffectAlready
                )
            {
                AI.SelectCard(CardId.UriaSacredBeastOfCataclysmicFire);
                AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                useRaviel = true;
                return true;
            }
            else if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe) &&
                Bot.HasInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) &&
                !useHamonSearchEffectAlready
                )
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                useRaviel = true;
                return true;
            }
            else if (useHamonSearchEffectAlready &&
                      !normalSummon &&
                      !Bot.HasInHand(CardId.MartyrOfTheSacredBeasts) &&
                      Bot.HasInDeck(CardId.MartyrOfTheSacredBeasts) &&
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            int wipeDesc = Util.GetStringId(CardId.RavielSacredBeastOfEndlessEternity, 1);
            if (ActivateDescription != wipeDesc && ActivateDescription != -1) return false;

            if (Enemy.GetMonsterCount() <= 0) return false;

            if (CountFaceupMartyrOnField() < 2) return false;

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
            if (!Bot.HasInDeck(CardId.DestructionChantOfTheSacredBeast)) return false;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            if (ActivateDescription != Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 1)) return false;
            if (Bot.GetMonstersInMainZone().Count(c => c != null) >= 3) return false;
            if (Bot.GetCardCountInDeck(CardId.MartyrOfTheSacredBeasts) + Bot.Graveyard.Count(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)) < 2) return false;

            AI.SelectCard(new[] { CardId.MartyrOfTheSacredBeasts, CardId.MartyrOfTheSacredBeasts });
            Martyrx3 = true;
            return true;
        }
        private bool Skyfire_Hand_ActivateCardOnly()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
            if (Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)) return false;
            if (PickSkyfireRevealTarget() == 0) return false;

            SelectSTPlace(null, true);
            return true;
        }
        private bool Orchestrator_Field_ReviveRouteTarget()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;

            int summonDesc = Util.GetStringId(CardId.FallenParadiseOfTheSacredBeasts, 0);
            if (ActivateDescription != summonDesc) return false;

            if (!HasFreeMonsterZone()) return false;

            int costCode = PickFallenParadiseCostCode();
            if (costCode == 0) return false;

            int target = PickFallenParadiseSummonTarget();
            if (target == 0) return false;

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
                && !DefaultCheckWhetherCardEffectIsNegated(c)
                && !c.IsDisabled()) >= 3)
            {
                return CardId.MartyrOfTheSacredBeasts;
            }
            if (Bot.GetSpells().Count(c =>
                c != null
                && c.IsFaceup()
                && c.IsCode(CardId.DivineAbyssOfTheSacredBeast)
                && !DefaultCheckWhetherCardEffectIsNegated(c)
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
            if (DefaultCheckWhetherCardEffectIsNegated(raviel)) return false;

            return CountFaceupMartyrOnField() >= 2;
        }

        private bool LinkuribohActivate()
        {
            Logger.DebugWriteLine(
    $"LinkuribohActivate loc={Card.Location} player={Duel.Player} phase={Duel.Phase} desc={ActivateDescription}"
);
            if (DefaultCheckWhetherCardEffectWillBeNegated(
                Card, Card.Location != CardLocation.MonsterZone)) return false;

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

            if (DefaultCheckWhetherCardEffectIsNegated(martyr))
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
            if (DefaultCheckWhetherCardEffectWillBeNegated(Card)) return false;
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
