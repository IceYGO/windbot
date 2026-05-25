using System;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    // CORI Sacred Beast Executor
    // Routed from user-corrected combo notes:
    // 1) Starter = Lightning Crash / Card of the Soul -> Hamon.
    // 2) Hamon searches Unleashing first. If Unleashing is already available, search Skyfire. If both are available, search Paradise as spare cost.
    // 3) Unleashing is the real core: search 3 different pieces, never discard Uria, and intentionally discard Hamon/Orchestrator when following the Hamon route.
    // 4) If no starter/searcher, Normal Martyr first; Martyr on-summon must search Skyfire only.
    // 5) Skyfire places Skyfire x2 + Fallen Paradise; never activate Fallen Paradise from hand as opener. Fallen Paradise pays Skyfire x3 first and summons Orchestrator.
    // 6) Opponent turn: Destruction Chant / Divine Abyss / Raviel wipe are held as interrupts, with Raviel wipe only if Martyr x2 can be tributed.
    [Deck("SacredBot", "AI_SacredBot")]
    class SacredExecutor : DefaultExecutor
    {
        public class CardId
        {
            // ===== Main Deck: CORI Sacred Beast core =====
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

            // ===== Main Deck: utility / hand traps =====
            public const int ThunderKingTheLightningstrikeKaiju = 48770333;
            public const int MulcharmyFuwalos = 42141493;
            public const int AshBlossom = 14558127;
            public const int MaxxC = 23434538;
            public const int LightningCrash = 89753095;       // user เรียก Thunder Clap: ใช้เปิดหา Hamon ได้
            public const int Thunderclap = LightningCrash;    // alias กันชื่อเก่าในโค้ด
            public const int HeavyPolymerization = 58570206;
            public const int CalledByTheGrave = 24224830;
            public const int CardOfTheSoul = 7044562;         // user พิมพ์ Call of the soul; ID นี้คือ Card of the Soul

            // ===== Extra Deck =====
            public const int PhantasmalSacredBeastsOfChaos = 7894706;
            public const int SuperVehicroidMobileBase = 17745969;
            public const int SaintAzamina = 85065943;
            public const int ThunderDragonColossus = 15291624;
            public const int SuperdreadnoughtRailCannonGustavRocket = 92359409;
            public const int SuperdreadnoughtRailCannonGustavMax = 56910167;
            public const int VarudrasTheFinalBringer = 70636044;
            public const int SPLittleKnight = 29301450;
            public const int Linkuriboh = 41999284;
        }

        private readonly HashSet<int> SacredBeastMonsterIds = new HashSet<int>
        {
            CardId.HamonSacredBeastOfSinfulCatastrophe,
            CardId.RavielSacredBeastOfEndlessEternity,
            CardId.UriaSacredBeastOfCataclysmicFire
        };

        private readonly HashSet<int> SacredBeastSpellIds = new HashSet<int>
        {
            CardId.UnleashingTheSacredBeasts,
            CardId.SkyfireOfTheSacredBeast,
            CardId.FallenParadiseOfTheSacredBeasts
        };

        private readonly HashSet<int> SacredBeastTrapIds = new HashSet<int>
        {
            CardId.DivineAbyssOfTheSacredBeast,
            CardId.DestructionChantOfTheSacredBeast
        };

        private readonly HashSet<int> SacredBeastSupportMonsterIds = new HashSet<int>
        {
            CardId.TheOrchestratorOfTheSacredBeasts,
            CardId.MartyrOfTheSacredBeasts
        };

        private readonly Dictionary<int, int> DeckCountTable = new Dictionary<int, int>
        {
            { CardId.UnleashingTheSacredBeasts, 3 },
            { CardId.HamonSacredBeastOfSinfulCatastrophe, 3 },
            { CardId.RavielSacredBeastOfEndlessEternity, 3 },
            { CardId.UriaSacredBeastOfCataclysmicFire, 1 },
            { CardId.ThunderKingTheLightningstrikeKaiju, 1 },
            { CardId.TheOrchestratorOfTheSacredBeasts, 1 },
            { CardId.MulcharmyFuwalos, 3 },
            { CardId.AshBlossom, 2 },
            { CardId.MaxxC, 1 },
            { CardId.MartyrOfTheSacredBeasts, 3 },
            { CardId.SkyfireOfTheSacredBeast, 3 },
            { CardId.LightningCrash, 3 },
            { CardId.FallenParadiseOfTheSacredBeasts, 3 },
            { CardId.HeavyPolymerization, 3 },
            { CardId.CalledByTheGrave, 1 },
            { CardId.DivineAbyssOfTheSacredBeast, 3 },
            { CardId.DestructionChantOfTheSacredBeast, 1 },
            { CardId.CardOfTheSoul, 3 }
        };

        private readonly List<int> CurrentNegateCardList = new List<int>();
        private int SPLittleKnightRemoveStep = 0;

        // Lightweight route memory. This makes Unleashing know whether Hamon was actually used first,
        // instead of guessing from whether Unleashing was already in the opening hand.
        private bool HamonSearchQueuedForRoute = false;

        public SacredExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // ===== Defensive chain / interruption first =====
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByTheGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, FuwalosActivate);
            AddExecutor(ExecutorType.Activate, CardId.VarudrasTheFinalBringer, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.PhantasmalSacredBeastsOfChaos, PhantasmalSacredBeastsOfChaosActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPLittleKnightActivate);
            AddExecutor(ExecutorType.Activate, CardId.DivineAbyssOfTheSacredBeast, DivineAbyssActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestructionChantOfTheSacredBeast, DestructionChantActivate);

            // ===== Going second / board breaker =====
            AddExecutor(ExecutorType.Activate, CardId.HeavyPolymerization, HeavyPolymerizationActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderKingTheLightningstrikeKaiju, ThunderKingKaijuSummon);

            // ===== Main starters / searchers =====
            // IMPORTANT: split by actual effect/location. Do not use one giant Activate per card.
            // Route should decide “what should be next”; each executor should answer only one prompt/effect.
            AddExecutor(ExecutorType.Activate, CardId.LightningCrash, LightningCrash_Starter_SearchHamon);
            AddExecutor(ExecutorType.Activate, CardId.CardOfTheSoul, CardOfTheSoul_Starter_SearchHamonOrRaviel);

            AddExecutor(ExecutorType.Activate, CardId.HamonSacredBeastOfSinfulCatastrophe, Hamon_Hand_SearchSpell);
            AddExecutor(ExecutorType.Activate, CardId.RavielSacredBeastOfEndlessEternity, Raviel_Hand_SearchUria);
            AddExecutor(ExecutorType.Activate, CardId.RavielSacredBeastOfEndlessEternity, Raviel_Field_BoardWipeOnlyWithMartyr2);
            AddExecutor(ExecutorType.Activate, CardId.UriaSacredBeastOfCataclysmicFire, Uria_Hand_SearchDestructionChant);
            AddExecutor(ExecutorType.Activate, CardId.UriaSacredBeastOfCataclysmicFire, Uria_Field_DestroyFaceupST);

            AddExecutor(ExecutorType.Activate, CardId.UnleashingTheSacredBeasts, Unleashing_Main_Search3Discard2);
            AddExecutor(ExecutorType.Activate, CardId.UnleashingTheSacredBeasts, Unleashing_GY_RecoverySearch);

            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_OnSummon_PlaceSkyfireOnly);
            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_Field_SummonTwoMartyr);
            AddExecutor(ExecutorType.Activate, CardId.MartyrOfTheSacredBeasts, Martyr_GY_EndPhaseRecovery);

            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_Hand_ActivateCardOnly);
            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_Field_Place2RevealPlaceParadise);
            AddExecutor(ExecutorType.Activate, CardId.SkyfireOfTheSacredBeast, Skyfire_GY_EndPhaseRecovery);

            AddExecutor(ExecutorType.Activate, CardId.FallenParadiseOfTheSacredBeasts, FallenParadise_Field_SummonByCost3);
            AddExecutor(ExecutorType.Activate, CardId.FallenParadiseOfTheSacredBeasts, FallenParadise_Field_Draw2AfterSetup);

            AddExecutor(ExecutorType.Activate, CardId.TheOrchestratorOfTheSacredBeasts, Orchestrator_Hand_SummonSacredBeast);
            AddExecutor(ExecutorType.Activate, CardId.TheOrchestratorOfTheSacredBeasts, Orchestrator_Field_ReviveRouteTarget);
            AddExecutor(ExecutorType.Activate, CardId.TheOrchestratorOfTheSacredBeasts, Orchestrator_GY_ReviveLevel10);

            // ===== Normal Summon plan =====
            AddExecutor(ExecutorType.Summon, CardId.MartyrOfTheSacredBeasts, MartyrSummon);
            AddExecutor(ExecutorType.Summon, CardId.TheOrchestratorOfTheSacredBeasts, OrchestratorSummon);

            // ===== Extra Deck plan =====
            AddExecutor(ExecutorType.SpSummon, CardId.PhantasmalSacredBeastsOfChaos, PhantasmalSacredBeastsOfChaosSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.VarudrasTheFinalBringer, Rank10NegateSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonGustavRocket, GustavRocketSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SuperdreadnoughtRailCannonGustavMax, GustavMaxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus, ThunderDragonColossusSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPLittleKnightSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, LinkuribohSummon);

            // ===== Generic fallback =====
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        #region Helper

        private int CheckRemainInDeck(int id)
        {
            if (!DeckCountTable.ContainsKey(id)) return 0;
            return Bot.GetRemainingCount(id, DeckCountTable[id]);
        }

        private int CheckRemainInDeck(params int[] ids)
        {
            int result = 0;
            foreach (int id in ids) result += CheckRemainInDeck(id);
            return result;
        }

        private bool CheckWhetherNegated(bool disablecheck = true, bool toFieldCheck = false, CardType type = 0)
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return true;
            if (disablecheck
                && (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.SpellZone)
                && Card.IsFaceup()
                && Card.IsDisabled())
            {
                return true;
            }
            return false;
        }

        private int GetBotLifePointsSafe()
        {
            // WindBot หลาย fork ใช้ชื่อ property ไม่เหมือนกัน เลยใช้ reflection เพื่อไม่ผูกกับ API ชื่อเดียว
            string[] names = { "LifePoints", "LP", "Lp", "lifePoints" };
            foreach (string name in names)
            {
                var prop = Bot.GetType().GetProperty(name);
                if (prop != null)
                {
                    object value = prop.GetValue(Bot, null);
                    if (value != null) return Convert.ToInt32(value);
                }

                var field = Bot.GetType().GetField(name);
                if (field != null)
                {
                    object value = field.GetValue(Bot);
                    if (value != null) return Convert.ToInt32(value);
                }
            }

            // ถ้าอ่าน LP ไม่ได้ ให้ถือว่า 8000 เพื่อให้ Card of the Soul ยังทำงานตอนเปิดเกมได้
            return 8000;
        }

        private bool IsSacredBeastMonster(ClientCard card)
        {
            return card != null && SacredBeastMonsterIds.Contains(card.Id);
        }

        private bool IsSacredBeastSupport(ClientCard card)
        {
            return card != null && (SacredBeastMonsterIds.Contains(card.Id) || SacredBeastSupportMonsterIds.Contains(card.Id));
        }

        private bool HasSacredBeastOnField()
        {
            return Bot.GetMonsters().Any(IsSacredBeastMonster);
        }

        private bool HasSacredBeastInHand()
        {
            return Bot.Hand.Any(IsSacredBeastMonster);
        }

        private bool HasSacredBeastInGrave()
        {
            return Bot.Graveyard.Any(IsSacredBeastMonster);
        }

        private bool HasFreeMonsterZone()
        {
            return Bot.GetMonsterCount() < 5;
        }

        private bool HasTwoFreeMonsterZones()
        {
            return Bot.GetMonsterCount() <= 3;
        }

        private bool HasCoreFieldReady()
        {
            return HasSacredBeastOnField() && Bot.HasInSpellZone(CardId.FallenParadiseOfTheSacredBeasts, true);
        }

        private bool HasPhantasmalOnField()
        {
            return Bot.HasInMonstersZone(CardId.PhantasmalSacredBeastsOfChaos, true);
        }

        private int CountSacredBeastsOnField()
        {
            return Bot.GetMonsters().Count(IsSacredBeastMonster);
        }

        private int CountLevel10MonstersOnField()
        {
            return Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && c.Level == 10);
        }

        private int CountFaceupSpellTrap(int id)
        {
            return Bot.GetSpells().Count(c => c != null && c.IsFaceup() && c.IsCode(id));
        }

        private int CountDifferentSacredBeastsRemainInDeck()
        {
            int count = 0;
            foreach (int id in SacredBeastMonsterIds)
            {
                if (CheckRemainInDeck(id) > 0) count++;
            }
            return count;
        }

        private int CountCardsByTypeForFallenParadise(CardType type)
        {
            int hand = Bot.Hand.Count(c => c != null && c.HasType(type));
            int field = Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && c.HasType(type));
            field += Bot.GetSpells().Count(c => c != null && c.IsFaceup() && c.HasType(type));
            return hand + field;
        }

        private bool CanPayFallenParadiseCost()
        {
            return CountCardsByTypeForFallenParadise(CardType.Monster) >= 3
                || CountCardsByTypeForFallenParadise(CardType.Spell) >= 3
                || CountCardsByTypeForFallenParadise(CardType.Trap) >= 3;
        }

        private bool IsBoardWipedRecoveryState()
        {
            // หลังโดน board wipe: สนามโล่งหรือเกือบโล่ง แต่ GY ยังมี engine ให้กู้เกม
            if (Bot.GetMonsterCount() > 0) return false;
            return HasSacredBeastInGrave()
                || Bot.Graveyard.Any(c => c != null && c.IsCode(
                    CardId.TheOrchestratorOfTheSacredBeasts,
                    CardId.MartyrOfTheSacredBeasts,
                    CardId.UnleashingTheSacredBeasts,
                    CardId.SkyfireOfTheSacredBeast,
                    CardId.DivineAbyssOfTheSacredBeast,
                    CardId.DestructionChantOfTheSacredBeast));
        }

        private bool IsSacredEndBoardReady()
        {
            // กันปัญหา search/extend หลังบอร์ดจบ เช่น Hamon/Uria/Raviel ยังพยายามกดบนมือหลังได้บอสแล้ว
            if (HasPhantasmalOnField()) return true;
            if (HasCoreFieldReady() && CountSacredBeastsOnField() >= 2 && Bot.HasInSpellZone(CardId.DivineAbyssOfTheSacredBeast, true)) return true;
            return false;
        }

        private bool ShouldOpenOrExtendCombo()
        {
            if (IsSacredEndBoardReady()) return false;
            return HasFreeMonsterZone() || IsBoardWipedRecoveryState();
        }

        private int[] SacredBeastMonsterPriority()
        {
            return new int[]
            {
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.UriaSacredBeastOfCataclysmicFire
            };
        }

        private int[] SacredBeastMonsterSearchPriority()
        {
            // Combo 1 ต้องเข้า Hamon ก่อนเพื่อหา Skyfire/Fallen
            // Combo 2 / recovery ต้องมี Raviel เพื่อหา monster piece ต่อ
            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe) && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                return new int[]
                {
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.RavielSacredBeastOfEndlessEternity,
                    CardId.UriaSacredBeastOfCataclysmicFire
                };
            }

            if (!Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity) && CheckRemainInDeck(CardId.RavielSacredBeastOfEndlessEternity) > 0)
            {
                return new int[]
                {
                    CardId.RavielSacredBeastOfEndlessEternity,
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.UriaSacredBeastOfCataclysmicFire
                };
            }

            return SacredBeastMonsterPriority();
        }

        private int[] RavielSearchPriority()
        {
            // Raviel หา Sacred Beast monster ยกเว้นตัวเอง: เอา Hamon ก่อน ถ้ามีแล้วค่อย Orchestrator/Martyr/Uria
            return new int[]
            {
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.TheOrchestratorOfTheSacredBeasts,
                CardId.MartyrOfTheSacredBeasts,
                CardId.UriaSacredBeastOfCataclysmicFire
            };
        }

        private int[] SacredBeastSpellPriority()
        {
            int hamonTarget = PickHamonSpellSearchTarget();
            if (hamonTarget != 0)
            {
                return new int[] { hamonTarget };
            }

            return new int[]
            {
                CardId.UnleashingTheSacredBeasts,
                CardId.SkyfireOfTheSacredBeast,
                CardId.FallenParadiseOfTheSacredBeasts
            };
        }

        private int[] SacredBeastTrapPriority()
        {
            return new int[]
            {
                CardId.DivineAbyssOfTheSacredBeast,
                CardId.DestructionChantOfTheSacredBeast
            };
        }

        private int[] MartyrPlacePriority()
        {
            // User route: Martyr summon must search/place Skyfire only.
            // Do not let Martyr fetch Fallen/Divine/Chant during the main combo, because Skyfire is the bridge into Fallen Paradise.
            return new int[] { CardId.SkyfireOfTheSacredBeast };
        }

        private ClientCard GetBestEnemyMonster(bool faceupOnly = true)
        {
            IEnumerable<ClientCard> monsters = Enemy.GetMonsters().Where(c => c != null);
            if (faceupOnly) monsters = monsters.Where(c => c.IsFaceup());
            return monsters
                .OrderByDescending(c => c.IsFloodgate() ? 100000 : 0)
                .ThenByDescending(c => c.IsMonsterDangerous() ? 50000 : 0)
                .ThenByDescending(c => c.Attack)
                .FirstOrDefault();
        }

        private ClientCard GetBestEnemyCard(bool faceupOnly = false)
        {
            ClientCard monster = GetBestEnemyMonster(faceupOnly);
            if (monster != null) return monster;

            IEnumerable<ClientCard> spells = Enemy.GetSpells().Where(c => c != null);
            if (faceupOnly) spells = spells.Where(c => c.IsFaceup());
            return spells.FirstOrDefault();
        }

        private bool CheckLastChainShouldNegated()
        {
            ClientCard last = Util.GetLastChainCard();
            if (last == null || last.Controller != 1) return false;
            if (DefaultCheckWhetherCardIsNegated(last) || last.IsDisabled()) return false;
            if (last.IsCode(CardId.MulcharmyFuwalos, CardId.MaxxC)) return false;
            return true;
        }

        private int GetTurnSafe()
        {
            string[] names = { "Turn", "TurnCount", "CurrentTurn" };
            foreach (string name in names)
            {
                var prop = Duel.GetType().GetProperty(name);
                if (prop != null)
                {
                    object value = prop.GetValue(Duel, null);
                    if (value != null) return Convert.ToInt32(value);
                }

                var field = Duel.GetType().GetField(name);
                if (field != null)
                {
                    object value = field.GetValue(Duel);
                    if (value != null) return Convert.ToInt32(value);
                }
            }
            return 1;
        }

        private bool IsLikelyGoingFirst()
        {
            return GetTurnSafe() <= 1 && Enemy.GetMonsterCount() == 0;
        }

        private bool HasCardAccessible(int id)
        {
            return Bot.HasInHand(id)
                || Bot.HasInSpellZone(id, true)
                || Bot.HasInMonstersZone(id, true)
                || Bot.Graveyard.Any(c => c != null && c.IsCode(id));
        }

        private bool HasPrimaryStarterOrSearcher()
        {
            // Main route starters/searchers that can naturally reach Hamon -> Unleashing.
            // Raviel is intentionally not counted here, because the corrected route uses Raviel after Skyfire/Fallen is online.
            if (Bot.HasInHand(CardId.LightningCrash)) return true;
            if (Bot.HasInHand(CardId.CardOfTheSoul) && GetBotLifePointsSafe() == 8000) return true;
            if (Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)) return true;
            if (Bot.HasInHand(CardId.UnleashingTheSacredBeasts)) return true;
            return false;
        }

        private bool ShouldStartFromMartyrFallback()
        {
            // Bad hand fallback from user note:
            // no starter/searcher -> Normal Martyr first, search/place Skyfire, and never open with Paradise from hand.
            if (HasPrimaryStarterOrSearcher()) return false;
            if (!Bot.HasInHand(CardId.MartyrOfTheSacredBeasts)) return false;
            if (Bot.HasInMonstersZone(CardId.MartyrOfTheSacredBeasts, true)) return false;
            if (Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)) return false;
            return HasFreeMonsterZone();
        }

        private bool IsCoreStartedByMartyr()
        {
            return Bot.HasInMonstersZone(CardId.MartyrOfTheSacredBeasts, true)
                || Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)
                || Bot.HasInSpellZone(CardId.FallenParadiseOfTheSacredBeasts, true);
        }

        private int PickHamonSpellSearchTarget()
        {
            // Corrected route: Hamon -> Unleashing first. If Unleashing exists, get Skyfire.
            // If both Unleashing and Skyfire exist, Paradise becomes a spare cost/resource.
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

        private bool HasUsedHamonLine()
        {
            // Do not infer this from "Unleashing is accessible" because Unleashing can be in the opening hand.
            // It counts only if Hamon activation was queued or Hamon has already reached GY.
            return HamonSearchQueuedForRoute
                || Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe));
        }

        private bool IsMartyrNegatedOrInterrupted()
        {
            ClientCard martyr = Bot.GetMonsters()
                .FirstOrDefault(c => c != null && c.IsFaceup() && c.IsCode(CardId.MartyrOfTheSacredBeasts));
            if (martyr == null) return false;
            if (DefaultCheckWhetherCardIsNegated(martyr) || martyr.IsDisabled()) return true;

            // Practical checkpoint from the route: Martyr is on field, but no face-up Skyfire was placed.
            return CountFaceupSpellTrap(CardId.SkyfireOfTheSacredBeast) == 0
                && !Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)
                && HasCardAccessible(CardId.UnleashingTheSacredBeasts);
        }

        private int CountFaceupMartyrOnField()
        {
            return Bot.GetMonsters().Count(c => c != null && c.IsFaceup() && c.IsCode(CardId.MartyrOfTheSacredBeasts));
        }

        private bool IsNeverDiscard(int id)
        {
            // Uria must stay in hand for the Destruction Chant line.
            if (id == CardId.UriaSacredBeastOfCataclysmicFire) return true;

            // Never pay important Extra Deck monsters as Fallen/other generic cost if a fork can place them in hand/field somehow.
            return id == CardId.PhantasmalSacredBeastsOfChaos
                || id == CardId.ThunderDragonColossus
                || id == CardId.VarudrasTheFinalBringer
                || id == CardId.SPLittleKnight
                || id == CardId.SuperdreadnoughtRailCannonGustavRocket
                || id == CardId.SuperdreadnoughtRailCannonGustavMax;
        }

        private int DiscardScore(ClientCard card, HashSet<int> protectedIds, bool preferHamon = false, bool preferOrchestrator = false)
        {
            if (card == null) return 9999;
            if (IsNeverDiscard(card.Id)) return 9999;
            if (protectedIds != null && protectedIds.Contains(card.Id)) return 9999;

            if (preferHamon && card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)) return -100;
            if (preferOrchestrator && card.IsCode(CardId.TheOrchestratorOfTheSacredBeasts)) return -90;

            bool duplicate = Bot.Hand.Count(c => c != null && c.Id == card.Id) >= 2;
            if (duplicate) return 0;

            if (card.IsCode(CardId.DivineAbyssOfTheSacredBeast)) return 1; // brick in hand
            if (card.IsCode(CardId.CardOfTheSoul) && GetBotLifePointsSafe() != 8000) return 2;
            if (card.IsCode(CardId.HeavyPolymerization) && IsLikelyGoingFirst()) return 3;
            if (card.IsCode(CardId.ThunderKingTheLightningstrikeKaiju) && IsLikelyGoingFirst()) return 4;
            if (card.IsCode(CardId.MulcharmyFuwalos)) return 5;
            if (card.IsCode(CardId.SkyfireOfTheSacredBeast) && CountFaceupSpellTrap(CardId.SkyfireOfTheSacredBeast) >= 2) return 6;
            if (card.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe, CardId.RavielSacredBeastOfEndlessEternity)) return 50;
            if (card.IsCode(CardId.AshBlossom, CardId.MaxxC, CardId.CalledByTheGrave)) return 70;
            return 20;
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

        private List<ClientCard> GetBestDiscardCosts(int count, IEnumerable<int> protectedIds = null)
        {
            HashSet<int> protectedSet = protectedIds == null
                ? new HashSet<int>()
                : new HashSet<int>(protectedIds);

            return Bot.Hand
                .Where(c => c != null)
                .OrderBy(c => DiscardScore(c, protectedSet))
                .Where(c => DiscardScore(c, protectedSet) < 9999)
                .Take(count)
                .ToList();
        }

        private void SelectDiscardCost(params int[] protectedIds)
        {
            ClientCard discard = GetBestDiscardCost(protectedIds);
            if (discard != null) AI.SelectCard(discard);
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

        private void QueueUnleashingSearchAndCost()
        {
            bool hamonAlreadyUsed = HasUsedHamonLine()
                || Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe));

            if (hamonAlreadyUsed)
            {
                // Case 1: Hamon was used first.
                // Search Raviel + Martyr + Orchestrator, then discard Hamon + Orchestrator.
                AI.SelectCard(new[]
                {
                    CardId.RavielSacredBeastOfEndlessEternity,
                    CardId.MartyrOfTheSacredBeasts,
                    CardId.TheOrchestratorOfTheSacredBeasts
                });
                AI.SelectNextCard(new[]
                {
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.TheOrchestratorOfTheSacredBeasts
                });
                return;
            }

            // Case 2: Hamon has not been used.
            // Search Raviel + Martyr + Hamon. Cost is flexible, but never Uria and never the searched/core cards.
            AI.SelectCard(new[]
            {
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.MartyrOfTheSacredBeasts,
                CardId.HamonSacredBeastOfSinfulCatastrophe
            });

            List<ClientCard> costs = GetBestDiscardCosts(2, new int[]
            {
                CardId.RavielSacredBeastOfEndlessEternity,
                CardId.MartyrOfTheSacredBeasts,
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.UriaSacredBeastOfCataclysmicFire
            });

            if (costs.Count >= 2)
                AI.SelectNextCard(costs.Take(2).ToList());
        }

        private List<ClientCard> GetFallenParadiseCostCards()
        {
            // Priority 1: Skyfire x3.
            List<ClientCard> skyfires = Bot.GetSpells()
                .Where(c => c != null && c.IsFaceup() && c.IsCode(CardId.SkyfireOfTheSacredBeast))
                .Take(3)
                .ToList();
            if (skyfires.Count >= 3) return skyfires;

            // Priority 2: Martyr x3 if the Martyr line was not interrupted.
            List<ClientCard> martyrs = Bot.GetMonsters()
                .Where(c => c != null && c.IsFaceup() && c.IsCode(CardId.MartyrOfTheSacredBeasts)
                    && !DefaultCheckWhetherCardIsNegated(c) && !c.IsDisabled())
                .Take(3)
                .ToList();
            if (martyrs.Count >= 3) return martyrs;

            // Priority 3: if Martyr was stopped, use expendable monsters from hand/field, especially Fuwalos or duplicates.
            List<ClientCard> monsters = Bot.GetMonsters()
                .Where(c => c != null && c.IsFaceup() && c.HasType(CardType.Monster) && !IsNeverDiscard(c.Id))
                .Concat(Bot.Hand.Where(c => c != null && c.HasType(CardType.Monster) && !IsNeverDiscard(c.Id)))
                .OrderBy(c =>
                {
                    if (c.IsCode(CardId.MulcharmyFuwalos)) return 0;
                    if (Bot.Hand.Count(h => h != null && h.Id == c.Id) >= 2) return 1;
                    if (c.IsCode(CardId.ThunderKingTheLightningstrikeKaiju) && IsLikelyGoingFirst()) return 2;
                    if (c.IsCode(CardId.AshBlossom, CardId.MaxxC)) return 8;
                    if (c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe, CardId.RavielSacredBeastOfEndlessEternity)) return 20;
                    return 5;
                })
                .Take(3)
                .ToList();
            if (monsters.Count >= 3) return monsters;

            // Last resort: spare spells, but never consume Unleashing before it starts the combo if it is still in hand.
            List<ClientCard> spells = Bot.GetSpells()
                .Where(c => c != null && c.IsFaceup() && c.HasType(CardType.Spell))
                .Concat(Bot.Hand.Where(c => c != null && c.HasType(CardType.Spell)
                    && !c.IsCode(CardId.UnleashingTheSacredBeasts)))
                .Where(c => c != null && !IsNeverDiscard(c.Id))
                .OrderBy(c => c.IsCode(CardId.HeavyPolymerization) && IsLikelyGoingFirst() ? 0 : 5)
                .Take(3)
                .ToList();
            if (spells.Count >= 3) return spells;

            return new List<ClientCard>();
        }

        private int CountAccessibleSkyfireCopiesForEffect()
        {
            // Skyfire can place 2 copies from hand, Deck, and/or GY.
            // Do not count the current face-up Skyfire that is activating its effect.
            int count = CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast);
            count += Bot.Hand.Count(c => c != null && c.IsCode(CardId.SkyfireOfTheSacredBeast));
            count += Bot.Graveyard.Count(c => c != null && c.IsCode(CardId.SkyfireOfTheSacredBeast));
            return count;
        }

        private int PickSkyfireRevealTarget()
        {
            // Route checkpoint wants Raviel revealed here when possible, because Raviel then searches Uria.
            if (Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity))
                return CardId.RavielSacredBeastOfEndlessEternity;

            if (Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe))
                return CardId.HamonSacredBeastOfSinfulCatastrophe;

            if (Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire))
                return CardId.UriaSacredBeastOfCataclysmicFire;

            return 0;
        }

        private int PickFallenParadiseSummonTarget()
        {
            // Correct route target: first Fallen Paradise summon should produce Orchestrator.
            if (!Bot.HasInMonstersZone(CardId.TheOrchestratorOfTheSacredBeasts, true)
                && !Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.TheOrchestratorOfTheSacredBeasts))
                && CheckRemainInDeck(CardId.TheOrchestratorOfTheSacredBeasts) > 0)
                return CardId.TheOrchestratorOfTheSacredBeasts;

            if (!Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity, true)
                && (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity))
                    || CheckRemainInDeck(CardId.RavielSacredBeastOfEndlessEternity) > 0))
                return CardId.RavielSacredBeastOfEndlessEternity;

            if (!Bot.HasInMonstersZone(CardId.HamonSacredBeastOfSinfulCatastrophe, true)
                && (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe))
                    || CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0))
                return CardId.HamonSacredBeastOfSinfulCatastrophe;

            return SacredBeastMonsterSearchPriority().FirstOrDefault();
        }

        private bool CanMakePhantasmalFusion()
        {
            int fieldSacred = Bot.GetMonsters().Count(IsSacredBeastMonster);
            int fieldLevel10 = CountLevel10MonstersOnField();
            int handLevel10 = Bot.Hand.Count(c => c != null && c.Level == 10);
            return fieldSacred >= 3 || fieldLevel10 + handLevel10 >= 3;
        }

        private bool HasOpponentRelevantChain()
        {
            return Duel.LastChainPlayer == 1 && CheckLastChainShouldNegated();
        }

        #endregion

        #region Hand traps / generic defense

        private bool AshBlossomActivate()
        {
            return HasOpponentRelevantChain();
        }

        private bool MaxxCActivate()
        {
            return Duel.Player == 1;
        }

        private bool FuwalosActivate()
        {
            return Duel.Player == 1 && Bot.GetMonsterCount() == 0 && Bot.GetSpellCount() == 0;
        }

        private bool CalledByTheGraveActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            ClientCard last = Util.GetLastChainCard();
            if (last != null && last.Controller == 1 && last.Location == CardLocation.Grave)
            {
                AI.SelectCard(last);
                return true;
            }

            ClientCard target = Enemy.Graveyard.FirstOrDefault(c => c != null && c.IsCode(CardId.AshBlossom, CardId.MaxxC, CardId.MulcharmyFuwalos));
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }

            return false;
        }

        #endregion

        #region Main Deck core - effect split executors

        // =====================================================================
        // Effect-split executors
        // ---------------------------------------------------------------------
        // Each method below answers exactly ONE card effect / prompt family.
        // Do not merge hand / field / GY effects into one method again.
        // If a card still refuses to activate, log ActivateDescription for only
        // that specific method and fix the string id, instead of changing route.
        // =====================================================================

        private bool LightningCrash_Starter_SearchHamon()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;
            if (Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)) return false;
            if (CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) <= 0) return false;

            AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
            return true;
        }

        private bool CardOfTheSoul_Starter_SearchHamonOrRaviel()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;
            if (GetBotLifePointsSafe() != 8000) return false;

            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            if (!Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity)
                && CheckRemainInDeck(CardId.RavielSacredBeastOfEndlessEternity) > 0)
            {
                AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                return true;
            }

            return false;
        }

        private bool Hamon_Hand_SearchSpell()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (!ShouldOpenOrExtendCombo()) return false;

            int searchTarget = PickHamonSpellSearchTarget();
            if (searchTarget == 0) return false;

            HamonSearchQueuedForRoute = true;

            // Normal route: never discard Hamon or searched card.
            // If Martyr already got stopped and Hamon is still usable, the route explicitly wants Hamon as cost.
            if (IsMartyrNegatedOrInterrupted())
            {
                AI.SelectCard(searchTarget);
                AI.SelectNextCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            return QueueSearchThenDiscard(
                searchTarget,
                CardId.HamonSacredBeastOfSinfulCatastrophe,
                CardId.UriaSacredBeastOfCataclysmicFire);
        }

        private bool Raviel_Hand_SearchUria()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (!ShouldOpenOrExtendCombo()) return false;
            if (ShouldStartFromMartyrFallback()) return false;
            if (Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire)) return false;
            if (CheckRemainInDeck(CardId.UriaSacredBeastOfCataclysmicFire) <= 0) return false;

            AI.SelectCard(CardId.UriaSacredBeastOfCataclysmicFire);
            AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
            return true;
        }

        private bool Raviel_Field_BoardWipeOnlyWithMartyr2()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (Duel.Player != 1) return false;
            if (Enemy.GetMonsterCount() <= 0) return false;

            // User rule: use Raviel wipe ONLY when Martyr x2 can be tributed.
            return CountFaceupMartyrOnField() >= 2;
        }

        private bool Uria_Hand_SearchDestructionChant()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (!ShouldOpenOrExtendCombo()) return false;
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

        private bool Uria_Field_DestroyFaceupST()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            ClientCard target = Enemy.GetSpells().FirstOrDefault(c => c != null && c.IsFaceup());
            if (target == null) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool Unleashing_Main_Search3Discard2()
        {
            if (Card.Location != CardLocation.Hand && Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;

            QueueUnleashingSearchAndCost();
            return true;
        }

        private bool Unleashing_GY_RecoverySearch()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (!IsBoardWipedRecoveryState()) return false;

            int target = SacredBeastMonsterSearchPriority().FirstOrDefault(id => CheckRemainInDeck(id) > 0);
            if (target == 0) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool Martyr_OnSummon_PlaceSkyfireOnly()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            // Effect 0: on-summon place/search S/T. Correct route says Skyfire ONLY.
            if (!(ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 0)))
                return false;

            if (Bot.GetSpellCount() >= 5) return false;
            if (CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) <= 0) return false;

            AI.SelectCard(CardId.SkyfireOfTheSacredBeast);
            return true;
        }

        private bool Martyr_Field_SummonTwoMartyr()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;

            // Effect 1: summon two Martyrs.
            if (ActivateDescription != Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 1)) return false;
            if (!HasSacredBeastOnField()) return false;
            if (!HasTwoFreeMonsterZones()) return false;
            if (CheckRemainInDeck(CardId.MartyrOfTheSacredBeasts) + Bot.Graveyard.Count(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)) < 2) return false;

            // Let system pick the two Martyrs if script only asks yes/no; otherwise preselect Martyr ID twice.
            AI.SelectCard(new[] { CardId.MartyrOfTheSacredBeasts, CardId.MartyrOfTheSacredBeasts });
            return true;
        }

        private bool Martyr_GY_EndPhaseRecovery()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Duel.Player != 1 || Duel.Phase != DuelPhase.End) return false;
            return Bot.Graveyard.Any(IsSacredBeastMonster);
        }

        private bool Skyfire_Hand_ActivateCardOnly()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (ShouldStartFromMartyrFallback()) return false;
            if (Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)) return false;
            if (PickSkyfireRevealTarget() == 0) return false;

            // Only activate/set the Continuous Spell. The field effect is a different executor.
            SelectSTPlace(null, true);
            return true;
        }

        private bool Skyfire_Field_Place2RevealPlaceParadise()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (IsSacredEndBoardReady()) return false;
            if (Duel.Player != 0) return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2) return false;

            int revealTarget = PickSkyfireRevealTarget();
            if (revealTarget == 0) return false;
            if (CountAccessibleSkyfireCopiesForEffect() < 2) return false;

            // Prompt 1: choose/place 2 Skyfire.
            AI.SelectCard(new[]
            {
                CardId.SkyfireOfTheSacredBeast,
                CardId.SkyfireOfTheSacredBeast
            });

            // Prompt 2: reveal LV10 Sacred Beast in hand.
            AI.SelectNextCard(revealTarget);

            // Prompt 3: place Fallen Paradise.
            if (CheckRemainInDeck(CardId.FallenParadiseOfTheSacredBeasts) > 0)
                AI.SelectNextCard(CardId.FallenParadiseOfTheSacredBeasts);

            return true;
        }

        private bool Skyfire_GY_EndPhaseRecovery()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (Duel.Player != 1 || Duel.Phase != DuelPhase.End) return false;
            return true;
        }

        private bool FallenParadise_Field_SummonByCost3()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (ShouldStartFromMartyrFallback()) return false;
            if (!HasFreeMonsterZone()) return false;

            List<ClientCard> costCards = GetFallenParadiseCostCards();
            int target = PickFallenParadiseSummonTarget();
            if (costCards.Count < 3 || target == 0) return false;

            AI.SelectCard(costCards.Take(3).ToList());
            AI.SelectNextCard(target);
            return true;
        }

        private bool FallenParadise_Field_Draw2AfterSetup()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!HasSacredBeastOnField()) return false;
            if (CountLevel10MonstersOnField() < 1) return false;

            // Draw should happen after setup, not before summon route.
            if (!(Bot.HasInMonstersZone(CardId.ThunderDragonColossus, true)
                || Bot.HasInMonstersZone(CardId.SPLittleKnight, true)
                || CountSacredBeastsOnField() >= 2)) return false;

            return true;
        }

        private bool Orchestrator_Hand_SummonSacredBeast()
        {
            if (Card.Location != CardLocation.Hand) return false;
            if (CheckWhetherNegated()) return false;
            if (!HasFreeMonsterZone()) return false;
            if (IsSacredEndBoardReady()) return false;
            if (!HasSacredBeastInHand()) return false;

            int target = Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity)
                ? CardId.RavielSacredBeastOfEndlessEternity
                : SacredBeastMonsterPriority().FirstOrDefault(id => Bot.HasInHand(id));
            if (target == 0) return false;

            ClientCard discard = GetBestDiscardCost(new int[] { CardId.UriaSacredBeastOfCataclysmicFire, target });
            if (discard == null) return false;

            AI.SelectCard(target);
            AI.SelectNextCard(discard);
            return true;
        }

        private bool Orchestrator_Field_ReviveRouteTarget()
        {
            if (Card.Location != CardLocation.MonsterZone) return false;
            if (CheckWhetherNegated()) return false;
            if (!HasFreeMonsterZone()) return false;
            if (IsSacredEndBoardReady()) return false;

            int target = 0;
            if (IsMartyrNegatedOrInterrupted()
                && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)))
                target = CardId.MartyrOfTheSacredBeasts;
            else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
                target = CardId.RavielSacredBeastOfEndlessEternity;
            else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
                target = CardId.HamonSacredBeastOfSinfulCatastrophe;
            else if (HasSacredBeastInHand())
                target = SacredBeastMonsterPriority().FirstOrDefault(id => Bot.HasInHand(id));

            if (target == 0) return false;

            ClientCard discard = GetBestDiscardCost(new int[] { CardId.UriaSacredBeastOfCataclysmicFire, target });
            if (discard == null) return false;

            AI.SelectCard(target);
            AI.SelectNextCard(discard);
            return true;
        }

        private bool Orchestrator_GY_ReviveLevel10()
        {
            if (Card.Location != CardLocation.Grave) return false;
            if (!HasFreeMonsterZone()) return false;
            if (IsSacredEndBoardReady()) return false;

            if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
            {
                AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                return true;
            }

            if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            return false;
        }

        #endregion

        #region Main Deck core

        private bool UnleashingTheSacredBeastsActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;

            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.SpellZone)
            {
                // Core combo starts here. Search 3 non-duplicate pieces, with route-specific cost.
                QueueUnleashingSearchAndCost();
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                // Recovery: only after board wipe / resource loss.
                if (!IsBoardWipedRecoveryState()) return false;
                int target = SacredBeastMonsterSearchPriority().FirstOrDefault(id => CheckRemainInDeck(id) > 0);
                if (target == 0) return false;
                AI.SelectCard(target);
                return true;
            }

            return false;
        }


        private bool LightningCrashActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;

            // Starter only: Thunder Clap / Lightning Crash opens by finding Hamon.
            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            return false;
        }


        private bool ThunderclapActivate()
        {
            return LightningCrashActivate();
        }

        private bool CardOfTheSoulActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (!ShouldOpenOrExtendCombo()) return false;

            // Card of the Soul is starter/extender only when LP is exactly 8000.
            // After LP changes, treat it as discard/cost fodder instead.
            if (GetBotLifePointsSafe() != 8000) return false;

            if (!Bot.HasInHand(CardId.HamonSacredBeastOfSinfulCatastrophe)
                && CheckRemainInDeck(CardId.HamonSacredBeastOfSinfulCatastrophe) > 0)
            {
                AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                return true;
            }

            if (!Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity)
                && CheckRemainInDeck(CardId.RavielSacredBeastOfEndlessEternity) > 0)
            {
                AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                return true;
            }

            return false;
        }


        private bool HamonActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!ShouldOpenOrExtendCombo()) return false;

                int searchTarget = PickHamonSpellSearchTarget();
                if (searchTarget == 0) return false;

                // Normal Hamon line: do not discard Hamon or the searched card.
                // Exception handled naturally later: if Martyr gets stopped and Hamon has not been used, Hamon may search Skyfire and discard Hamon.
                bool martyrStoppedFallback = IsMartyrNegatedOrInterrupted()
                    && !Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe));

                if (martyrStoppedFallback)
                {
                    HamonSearchQueuedForRoute = true;
                    AI.SelectCard(searchTarget);
                    AI.SelectNextCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                    return true;
                }

                HamonSearchQueuedForRoute = true;
                return QueueSearchThenDiscard(
                    searchTarget,
                    CardId.HamonSacredBeastOfSinfulCatastrophe,
                    CardId.UriaSacredBeastOfCataclysmicFire);
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                return false;
            }

            return false;
        }


        private bool RavielActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!ShouldOpenOrExtendCombo()) return false;
                if (ShouldStartFromMartyrFallback()) return false;

                // Correct checkpoint: after Skyfire/Fallen setup, Raviel searches Uria and discards Raviel.
                if (!Bot.HasInHand(CardId.UriaSacredBeastOfCataclysmicFire)
                    && CheckRemainInDeck(CardId.UriaSacredBeastOfCataclysmicFire) > 0)
                {
                    AI.SelectCard(CardId.UriaSacredBeastOfCataclysmicFire);
                    AI.SelectNextCard(CardId.RavielSacredBeastOfEndlessEternity);
                    return true;
                }

                // Fallback only if Uria is already available: find Orchestrator/Martyr/Hamon, but still never discard Uria.
                int target = new int[]
                {
                    CardId.TheOrchestratorOfTheSacredBeasts,
                    CardId.MartyrOfTheSacredBeasts,
                    CardId.HamonSacredBeastOfSinfulCatastrophe
                }.FirstOrDefault(id => !Bot.HasInHand(id) && CheckRemainInDeck(id) > 0);

                if (target == 0) return false;
                return QueueSearchThenDiscard(target, CardId.UriaSacredBeastOfCataclysmicFire, target);
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                // Important: Raviel board wipe only if Martyr x2 can be tributed. Otherwise never fire it.
                if (Duel.Player != 1) return false;
                if (Enemy.GetMonsterCount() <= 0) return false;
                return CountFaceupMartyrOnField() >= 2;
            }

            return false;
        }


        private bool UriaActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!ShouldOpenOrExtendCombo()) return false;
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

            if (Card.Location == CardLocation.MonsterZone)
            {
                ClientCard target = Enemy.GetSpells().Where(c => c != null && c.IsFaceup()).FirstOrDefault();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }

            return false;
        }


        private bool OrchestratorActivate()
        {
            if (CheckWhetherNegated()) return false;
            if (!HasFreeMonsterZone()) return false;
            if (IsSacredEndBoardReady()) return false;

            if (Card.Location == CardLocation.Hand)
            {
                if (!HasSacredBeastInHand()) return false;

                int target = Bot.HasInHand(CardId.RavielSacredBeastOfEndlessEternity)
                    ? CardId.RavielSacredBeastOfEndlessEternity
                    : SacredBeastMonsterPriority().FirstOrDefault(id => Bot.HasInHand(id));
                if (target == 0) return false;

                ClientCard discard = GetBestDiscardCost(new int[] { CardId.UriaSacredBeastOfCataclysmicFire, target });
                if (discard == null) return false;

                AI.SelectCard(target);
                AI.SelectNextCard(discard);
                return true;
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                // If Martyr was not stopped, revive Raviel first.
                // If Martyr was stopped, revive Martyr to continue the line.
                int target = 0;
                if (IsMartyrNegatedOrInterrupted()
                    && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)))
                    target = CardId.MartyrOfTheSacredBeasts;
                else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
                    target = CardId.RavielSacredBeastOfEndlessEternity;
                else if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
                    target = CardId.HamonSacredBeastOfSinfulCatastrophe;
                else if (HasSacredBeastInHand())
                    target = SacredBeastMonsterPriority().FirstOrDefault(id => Bot.HasInHand(id));

                if (target == 0) return false;

                ClientCard discard = GetBestDiscardCost(new int[] { CardId.UriaSacredBeastOfCataclysmicFire, target });
                if (discard == null) return false;

                AI.SelectCard(target);
                AI.SelectNextCard(discard);
                return true;
            }

            if (Card.Location == CardLocation.Grave)
            {
                // After Colossus / Link line: revive Raviel first, then Hamon.
                if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.RavielSacredBeastOfEndlessEternity)))
                {
                    AI.SelectCard(CardId.RavielSacredBeastOfEndlessEternity);
                    return true;
                }

                if (Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.HamonSacredBeastOfSinfulCatastrophe)))
                {
                    AI.SelectCard(CardId.HamonSacredBeastOfSinfulCatastrophe);
                    return true;
                }
            }

            return false;
        }


        private bool MartyrActivate()
        {
            if (CheckWhetherNegated()) return false;

            if (Card.Location == CardLocation.MonsterZone)
            {
                // On summon: Skyfire only. Do not place Fallen/Divine/Chant here.
                if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 0))
                {
                    if (Bot.GetSpellCount() >= 5) return false;
                    if (CheckRemainInDeck(CardId.SkyfireOfTheSacredBeast) <= 0) return false;
                    AI.SelectCard(CardId.SkyfireOfTheSacredBeast);
                    return true;
                }

                // Combo step 8: if Martyr remains on field, summon two more Martyrs.
                if (ActivateDescription == Util.GetStringId(CardId.MartyrOfTheSacredBeasts, 1))
                {
                    return HasSacredBeastOnField() && HasTwoFreeMonsterZones();
                }
            }

            if (Card.Location == CardLocation.Grave)
            {
                return Duel.Player == 1 && Duel.Phase == DuelPhase.End && Bot.Graveyard.Any(IsSacredBeastMonster);
            }

            return false;
        }


        private bool SkyfireActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;
            if (IsSacredEndBoardReady()) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                // Skyfire effect is not one flat selection.
                // Prompt 1: place 2 Skyfire.
                // Prompt 2: reveal 1 Level 10 Sacred Beast in hand.
                // Prompt 3: place Fallen Paradise from Deck.
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

            if (Card.Location == CardLocation.Hand)
            {
                if (ShouldStartFromMartyrFallback()) return false;

                // Activating Skyfire from hand only places the Continuous Spell on the field.
                // The ignition effect will be handled by the SpellZone branch on the next action window.
                if (Bot.HasInSpellZone(CardId.SkyfireOfTheSacredBeast, true)) return false;
                if (PickSkyfireRevealTarget() == 0) return false;

                SelectSTPlace(null, true);
                return true;
            }

            if (Card.Location == CardLocation.Grave && Duel.Player == 1 && Duel.Phase == DuelPhase.End)
            {
                return true;
            }

            return false;
        }


        private bool FallenParadiseActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            // Important bad-hand rule from user note:
            // Do not activate Fallen Paradise from hand as an opener. It must be placed by Skyfire/Martyr line first.
            if (Card.Location == CardLocation.Hand) return false;
            if (Card.Location != CardLocation.SpellZone) return false;
            if (ShouldStartFromMartyrFallback()) return false;

            // Summon effect comes before draw in the corrected route.
            if (HasFreeMonsterZone())
            {
                List<ClientCard> costCards = GetFallenParadiseCostCards();
                int target = PickFallenParadiseSummonTarget();

                if (costCards.Count >= 3 && target != 0)
                {
                    // Cost priority: Skyfire x3 -> Martyr x3 -> expendable monsters. Never pay Uria.
                    AI.SelectCard(costCards.Take(3).ToList());
                    AI.SelectNextCard(target);
                    return true;
                }
            }

            // Draw 2 only after the combo has put a Level 10 / revived body on board.
            if (HasSacredBeastOnField()
                && CountLevel10MonstersOnField() >= 1
                && (Bot.HasInMonstersZone(CardId.ThunderDragonColossus, true)
                    || Bot.HasInMonstersZone(CardId.SPLittleKnight, true)
                    || CountSacredBeastsOnField() >= 2))
            {
                return true;
            }

            return false;
        }


        private bool DivineAbyssActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            if (Card.Location == CardLocation.SpellZone)
            {
                // Opponent turn interrupt line. First place copies, then flip enemy monster.
                if (Duel.Player == 1 && CountFaceupSpellTrap(CardId.DivineAbyssOfTheSacredBeast) < 3 && Bot.GetSpellCount() < 5)
                {
                    AI.SelectCard(CardId.DivineAbyssOfTheSacredBeast, CardId.DivineAbyssOfTheSacredBeast);
                    return true;
                }

                ClientCard target = GetBestEnemyMonster(true);
                if (Duel.Player == 1 && target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }

            if (Card.Location == CardLocation.Grave && Duel.Player == 1 && Duel.Phase == DuelPhase.End)
            {
                return true;
            }

            return false;
        }


        private bool DestructionChantActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Trap)) return false;

            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.SpellZone)
            {
                // Main use is opponent turn.
                if (Duel.Player != 1 && !HasOpponentRelevantChain()) return false;

                if (HasFreeMonsterZone() && (HasSacredBeastInHand() || HasSacredBeastInGrave()))
                {
                    // If only 1 Level 10, revive another Level 10 with a different name.
                    // If already 2 Level 10, revive Martyr for the follow-up Divine Abyss line.
                    if (CountLevel10MonstersOnField() >= 2
                        && Bot.Graveyard.Any(c => c != null && c.IsCode(CardId.MartyrOfTheSacredBeasts)))
                    {
                        AI.SelectCard(CardId.MartyrOfTheSacredBeasts);
                        return true;
                    }

                    int target = new int[]
                    {
                        CardId.RavielSacredBeastOfEndlessEternity,
                        CardId.HamonSacredBeastOfSinfulCatastrophe,
                        CardId.UriaSacredBeastOfCataclysmicFire
                    }.FirstOrDefault(id => !Bot.HasInMonstersZone(id, true)
                        && (Bot.Graveyard.Any(c => c != null && c.IsCode(id)) || Bot.HasInHand(id)));

                    if (target == 0) return false;
                    AI.SelectCard(target);
                    return true;
                }

                if (CountSacredBeastsOnField() >= 2 && GetBestEnemyCard(true) != null)
                {
                    AI.SelectCard(GetBestEnemyCard(true));
                    return true;
                }
            }

            if (Card.Location == CardLocation.Grave)
            {
                // If Raviel can tribute Martyr x2 for board wipe, wait. Otherwise use GY fusion line.
                if (Bot.HasInMonstersZone(CardId.RavielSacredBeastOfEndlessEternity, true)
                    && CountFaceupMartyrOnField() >= 2
                    && Enemy.GetMonsterCount() > 0)
                    return false;

                if (CanMakePhantasmalFusion())
                {
                    AI.SelectCard(CardId.PhantasmalSacredBeastsOfChaos);
                    return true;
                }
            }

            return false;
        }


        private bool HeavyPolymerizationActivate()
        {
            if (CheckWhetherNegated(true, true, CardType.Spell)) return false;

            // ตาม note: ใช้ตอนฝ่ายตรงข้ามมี monster 3 ใบ เพื่อ surprise เป็นตัวใหญ่
            // ไม่ใช้เป็น extender ฝั่งเราเอง เพราะจะเสีย LP และอาจตัด resource โดยไม่จำเป็น
            if (Enemy.GetMonsterCount() < 3) return false;

            AI.SelectCard(
                CardId.PhantasmalSacredBeastsOfChaos,
                CardId.SaintAzamina,
                CardId.SaintAzamina,
                CardId.SaintAzamina);
            return true;
        }

        #endregion

        #region Normal Summon / Special Summon

        private bool MartyrSummon()
        {
            // Bad hand fallback: if there is no starter/searcher, Martyr is the first play.
            // Its on-summon effect is locked to Skyfire in MartyrActivate().
            if (IsSacredEndBoardReady()) return false;
            if (Bot.HasInMonstersZone(CardId.MartyrOfTheSacredBeasts, true) && HasCoreFieldReady()) return false;
            if (ShouldStartFromMartyrFallback()) return true;
            return true;
        }

        private bool OrchestratorSummon()
        {
            if (IsSacredEndBoardReady()) return false;
            if (HasCoreFieldReady() && CountSacredBeastsOnField() >= 2) return false;
            return HasSacredBeastInHand() || HasSacredBeastInGrave();
        }

        private bool ThunderKingKaijuSummon()
        {
            // ใช้เฉพาะมีตัวน่ากลัวจริง ไม่เอาไปตัด resource opponent โดยไม่จำเป็นตอนเรารำได้อยู่แล้ว
            ClientCard target = GetBestEnemyMonster(true);
            if (target == null) return false;
            if (!target.IsMonsterDangerous() && !target.IsFloodgate() && Enemy.GetMonsterCount() < 2) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool PhantasmalSacredBeastsOfChaosSummon()
        {
            if (HasPhantasmalOnField()) return false;
            return CountSacredBeastsOnField() >= 3 || CountLevel10MonstersOnField() >= 3;
        }

        private bool Rank10NegateSummon()
        {
            if (HasPhantasmalOnField()) return false;
            if (Bot.HasInMonstersZone(CardId.VarudrasTheFinalBringer, true)) return false;
            return CountLevel10MonstersOnField() >= 2;
        }

        private bool GustavMaxSummon()
        {
            if (HasPhantasmalOnField()) return false;
            if (CountLevel10MonstersOnField() < 2) return false;

            // ปิดเกม / หลังมี negate rank10 แล้วเท่านั้น ไม่แย่ง material ก่อน combo จบ
            return Bot.HasInMonstersZone(CardId.VarudrasTheFinalBringer, true) || Enemy.GetMonsterCount() == 0;
        }

        private bool GustavRocketSummon()
        {
            if (HasPhantasmalOnField()) return false;
            return Bot.HasInMonstersZone(CardId.SuperdreadnoughtRailCannonGustavMax, true)
                || CountLevel10MonstersOnField() >= 3;
        }

        private bool ThunderDragonColossusSummon()
        {
            if (HasPhantasmalOnField()) return false;
            if (Bot.HasInMonstersZone(CardId.ThunderDragonColossus, true)) return false;

            // Corrected route step 7: fuse/convert with Orchestrator after its field effect has been used.
            // Approximation: Orchestrator is face-up on field and we already have a Sacred Beast in GY/field from the route.
            return Bot.HasInMonstersZone(CardId.TheOrchestratorOfTheSacredBeasts, true)
                && (HasSacredBeastInGrave() || HasSacredBeastOnField());
        }


        private bool SPLittleKnightSummon()
        {
            if (HasPhantasmalOnField()) return false;
            if (GetBestEnemyCard() == null) return false;

            int martyrCount = CountFaceupMartyrOnField();
            bool hasLinkuriboh = Bot.HasInMonstersZone(CardId.Linkuriboh, true);

            // Corrected route step 9: use Martyr x2, or Linkuriboh + Martyr only.
            return martyrCount >= 2 || (hasLinkuriboh && martyrCount >= 1);
        }


        private bool LinkuribohSummon()
        {
            // Only link the Martyr that got stopped, as in the corrected checkpoint.
            if (!IsMartyrNegatedOrInterrupted()) return false;
            return Bot.GetMonsters().Any(c => c != null && c.IsFaceup()
                && c.Level == 1
                && c.IsCode(CardId.MartyrOfTheSacredBeasts));
        }


        #endregion

        #region Extra Deck effects

        private bool PhantasmalSacredBeastsOfChaosActivate()
        {
            if (CheckWhetherNegated()) return false;

            // negate monster ฝั่งตรงข้ามเท่านั้น ไม่กดใส่ตัวเอง
            ClientCard target = GetBestEnemyMonster(true);
            if (target == null) return false;
            if (Duel.Player == 0 && !target.IsMonsterDangerous() && !HasOpponentRelevantChain()) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool VarudrasActivate()
        {
            if (CheckWhetherNegated()) return false;
            return HasOpponentRelevantChain();
        }

        private bool SPLittleKnightActivate()
        {
            if (CheckWhetherNegated()) return false;

            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 0))
            {
                ClientCard target = GetBestEnemyCard();
                if (target == null) return false;

                AI.SelectCard(target);
                SPLittleKnightRemoveStep = 1;
                return true;
            }

            if (ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 1))
            {
                ClientCard self = Bot.GetMonsters().Where(c => c != null && Duel.ChainTargets.Contains(c)).FirstOrDefault();
                ClientCard enemy = GetBestEnemyMonster(true);
                if (self != null && enemy != null)
                {
                    AI.SelectCard(self);
                    AI.SelectNextCard(enemy);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Fallback

        private bool MonsterRepos()
        {
            if (Card == null || Card.IsFacedown()) return false;
            if (Card.HasType(CardType.Link)) return false;

            bool enemyBetter = Enemy.GetMonsters().Any(c => c != null && c.IsAttack() && c.Attack >= Card.GetDefensePower());
            if (Card.IsAttack() && enemyBetter && Card.Defense > Card.Attack) return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack > Card.Defense) return true;
            return false;
        }

        private bool SpellSet()
        {
            if (Card == null) return false;

            if (Card.IsTrap() || Card.HasType(CardType.QuickPlay))
            {
                SelectSTPlace();
                return true;
            }

            return false;
        }
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
        #endregion
    }
}
