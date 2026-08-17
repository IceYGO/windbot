using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Rank8", "AI_Rank8")]
    class Rank8Executor : DefaultExecutor
    {
        public class CardId
        {
            // 荷鲁斯族 Level 8 怪兽
            public const int ImsetyGloryOfHorus = 84941194;
            public const int QebehsenuefProtectionOfHorus = 74725513;
            public const int HapiGuidanceOfHorus = 47330808;
            public const int DuamutefBlessingOfHorus = 11335209;
            // 机界骑士 Level 8 怪兽
            public const int MekkKnightPurpleNightfall = 28692962;
            public const int MekkKnightIndigoEclipse = 92204263;
            // Level 4 引擎
            public const int NemesesCorridor = 72090076;
            public const int AleisterTheInvoker = 86120751;
            // 魔法/陷阱
            public const int Raigeki = 12580477;                      // 雷击
            public const int HarpiesFeatherDuster = 18144506;         // 鹰身女妖的羽毛扫
            public const int HeavyStorm = 19613556;                   // 大风暴
            public const int TradeIn = 38120068;                      // 抵价购物
            public const int Terraforming = 73628505;                 // 星球改造
            public const int Invocation = 74063034;                   // 召唤魔术
            public const int FoolishBurial = 81439173;                 // 愚蠢的埋葬
            public const int MonsterReborn = 83764718;                // 死者苏生
            public const int KingsSarcophagus = 16528181;             // 王之棺
            public const int WallsOfTheImperialTomb = 26984177;       // 王墓的石壁
            public const int MagicalMeltdown = 47679935;              // 暴走魔法阵
            // 额外卡组
            public const int InvokedMechaba = 75286621;                           // 召唤兽 梅尔卡巴 (融合)
            public const int ThunderDragonColossus = 15291624;                    // 超雷龙-雷龙
            public const int DivineArsenalAAZEUSSkyThunder = 90448279;            // 天霆号 阿宙斯 (XYZ)
            public const int TheZombieVampire = 73082255;                          // 真血公 吸血鬼 (XYZ Rank 8)
            public const int Number38HopeHarbingerDragonTitanicGalaxy = 63767246; // No.38 银河巨神 (XYZ Rank 8)
            public const int GarunixEternityHyangOfTheFireKings = 64182380;       // 炎王神 永炎大鹏 (XYZ Rank 8)
            public const int DingirsuTheOrcustOfTheEveningStar = 93854893;        // 宵星之机神 丁吉尔苏 (XYZ Rank 8)
            public const int NumberS39UtopiaTheLightning = 56832966;              // 闪光No.39 希望皇 电光皇 (XYZ Rank 5)
            public const int Number39Utopia = 84013237;                           // No.39 希望皇 霍普 (XYZ Rank 4)
            public const int MinervaTheExaltedLightsworn = 30100551;              // 光道圣女 密涅瓦 (XYZ Rank 4)
            public const int SPLittleKnight = 29301450;                           // S：P小夜骑士 (Link 2)
            public const int ArtemisTheMagistusMoonMaiden = 34755994;             // 圣魔之少女 阿耳特弥斯 (Link 1)
        }

        // 荷鲁斯族 Level 8 怪兽 ID 列表
        private static readonly int[] HorusMonsterIds = {
            CardId.ImsetyGloryOfHorus,
            CardId.QebehsenuefProtectionOfHorus,
            CardId.HapiGuidanceOfHorus,
            CardId.DuamutefBlessingOfHorus,
        };

        private bool _aleisterSearchedInvocation = false;
        private readonly HashSet<int> _horusSpecialSummonedThisTurn = new HashSet<int>();
        private bool _purpleNightfallSummoned = false;
        private bool _indigoEclipseSummoned = false;
        private bool _kingsSarcophagusBattleEffectUsed = false;
        private bool _zombieVampireEffectUsed = false;

        public Rank8Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // ─── 清场前特殊召唤机界骑士 ────────────────────────────────
            AddExecutor(ExecutorType.SpSummon, CardId.MekkKnightPurpleNightfall, MekkKnightSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.MekkKnightIndigoEclipse, MekkKnightSpSummon);

            // ─── 全场清除 ─────────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, HarpiesFeatherDusterEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, HeavyStormEffect);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);

            // ─── 召唤兽 梅尔卡巴 效果 ────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.InvokedMechaba, InvokedMechabaEffect);

            // ─── 场地与荷鲁斯引擎 ─────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.MagicalMeltdown, MagicalMeltdownEffect);
            AddExecutor(ExecutorType.Activate, CardId.KingsSarcophagus, KingsSarcophagusActivate);
            AddExecutor(ExecutorType.Activate, CardId.WallsOfTheImperialTomb, WallsOfTheImperialTombEffect);

            // ─── 搜索场地魔法 ─────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);

            // ─── 墓地准备 ─────────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);

            // ─── 抽牌 ────────────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.TradeIn, TradeInEffect);

            // ─── 死者苏生 ──────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);

            // ─── 荷鲁斯怪兽效果 ───────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.ImsetyGloryOfHorus, ImsetyEffect);
            AddExecutor(ExecutorType.Activate, CardId.HapiGuidanceOfHorus, HapiEffect);
            AddExecutor(ExecutorType.Activate, CardId.DuamutefBlessingOfHorus);
            AddExecutor(ExecutorType.Activate, CardId.QebehsenuefProtectionOfHorus);

            // ─── 机界骑士效果 ─────────────────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightPurpleNightfall, MekkKnightPurpleNightfallEffect);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightIndigoEclipse, MekkKnightIndigoEclipseEffect);

            // ─── 普通召唤 ────────────────────────────────────────────
            AddExecutor(ExecutorType.Summon, CardId.AleisterTheInvoker, AleisterSummon);
            AddExecutor(ExecutorType.Activate, CardId.AleisterTheInvoker, AleisterEffect);
            AddExecutor(ExecutorType.Activate, CardId.NemesesCorridor, NemesesCorridorEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonColossus);

            // 先把阿莱斯特变成光属性连接怪兽。
            AddExecutor(ExecutorType.SpSummon, CardId.ArtemisTheMagistusMoonMaiden, ArtemisSummon);

            // 用暂时不发动的魔法制造机界骑士的召唤纵列。
            AddExecutor(ExecutorType.SpellSet, MekkKnightSetupSpellSet);

            // ─── 召唤魔术（融合梅尔卡巴）─────────────────────────────
            AddExecutor(ExecutorType.Activate, CardId.Invocation, InvocationEffect);

            // 阿莱斯特相关动作均已完成后，王之棺才开始丢手堆墓。
            AddExecutor(ExecutorType.Activate, CardId.KingsSarcophagus, KingsSarcophagusEffect);

            // ─── 特殊召唤：荷鲁斯怪兽（王之棺触发）────────────────
            AddExecutor(ExecutorType.SpSummon, CardId.ImsetyGloryOfHorus, HorusMonsterSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HapiGuidanceOfHorus, HorusMonsterSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DuamutefBlessingOfHorus, HorusMonsterSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.QebehsenuefProtectionOfHorus, HorusMonsterSpSummon);

            // 阿宙斯候选只会在超量怪兽已经进行过战斗后出现。
            AddExecutor(ExecutorType.SpSummon, CardId.DivineArsenalAAZEUSSkyThunder, ZeusSummon);
            AddExecutor(ExecutorType.Activate, CardId.DivineArsenalAAZEUSSkyThunder, ZeusEffect);

            // ─── XYZ 召唤（额外卡组）────────────────────────────────
            // 密涅瓦：用两只 Level 4 堆墓，补充荷鲁斯墓地资源
            AddExecutor(ExecutorType.SpSummon, CardId.MinervaTheExaltedLightsworn, MinervaSummon);
            AddExecutor(ExecutorType.Activate, CardId.MinervaTheExaltedLightsworn, MinervaEffect);

            // No.38：压制场上发动的魔法效果
            AddExecutor(ExecutorType.SpSummon, CardId.Number38HopeHarbingerDragonTitanicGalaxy, Number38Summon);
            AddExecutor(ExecutorType.Activate, CardId.Number38HopeHarbingerDragonTitanicGalaxy, Number38Effect);

            // 炎王神永炎大鹏：超量召唤时破坏场上其他怪兽
            AddExecutor(ExecutorType.SpSummon, CardId.GarunixEternityHyangOfTheFireKings, GarunixSummon);
            AddExecutor(ExecutorType.Activate, CardId.GarunixEternityHyangOfTheFireKings, GarunixEffect);

            // 真血公 吸血鬼：双方各挖4张，SS怪兽
            AddExecutor(ExecutorType.SpSummon, CardId.TheZombieVampire, ZombieVampireSummon);
            AddExecutor(ExecutorType.Activate, CardId.TheZombieVampire, ZombieVampireEffect);

            // 宵星之机神 丁吉尔苏：不取对象送墓1张牌
            AddExecutor(ExecutorType.SpSummon, CardId.DingirsuTheOrcustOfTheEveningStar, DingirsuSummon);
            AddExecutor(ExecutorType.Activate, CardId.DingirsuTheOrcustOfTheEveningStar, DingirsuEffect);

            // 希望皇霍普（为电光皇做铺垫）
            AddExecutor(ExecutorType.SpSummon, CardId.Number39Utopia, Number39UtopiaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NumberS39UtopiaTheLightning);
            AddExecutor(ExecutorType.Activate, CardId.NumberS39UtopiaTheLightning, DefaultNumberS39UtopiaTheLightningEffect);

            // S:P小夜骑士
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPLittleKnightSummon);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPLittleKnightEffect);

            // ─── 调整攻守站位 ─────────────────────────────────────────
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        // ============================================================
        // 回合控制
        // ============================================================

        public override bool OnSelectHand()
        {
            return true; // 始终先手
        }

        public override void OnNewTurn()
        {
            _aleisterSearchedInvocation = false;
            _horusSpecialSummonedThisTurn.Clear();
            _purpleNightfallSummoned = false;
            _indigoEclipseSummoned = false;
            _kingsSarcophagusBattleEffectUsed = false; // 实际上不是卡名一回合一次，不过Bot一般不会同时场上多张
            _zombieVampireEffectUsed = false;
            base.OnNewTurn();
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ChainInfo currentChain = Duel.GetCurrentSolvingChainInfo();
            if (hint == HintMsg.SpSummon
                && min == 1
                && max == 1
                && currentChain != null
                && currentChain.IsActivateCode(CardId.TheZombieVampire))
            {
                ClientCard target = cards
                    .Where(c => c.Controller == 1 && c.Level >= 4)
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault()
                    ?? cards
                        .Where(c => c.Controller == 0
                            && c.IsCode(HorusMonsterIds)
                            && _horusSpecialSummonedThisTurn.Contains(c.Id))
                        .OrderByDescending(c => c.Attack)
                        .FirstOrDefault()
                    ?? cards.OrderByDescending(c => c.Attack).FirstOrDefault();
                if (target != null)
                    return new List<ClientCard> { target };
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        // ============================================================
        // 辅助方法
        // ============================================================

        private List<ClientCard> GetRank8Materials()
        {
            return Bot.GetMonsters()
                .Where(m => m.IsFaceup()
                    && m.Level == 8
                    && !m.HasType(CardType.Xyz)
                    && (!m.IsCode(CardId.ThunderDragonColossus) || m.IsDisabled()))
                .OrderBy(m => m.IsCode(CardId.QebehsenuefProtectionOfHorus) ? 0
                    : m.IsCode(CardId.ImsetyGloryOfHorus) ? 1 : 2)
                .ThenBy(m => m.Attack)
                .ToList();
        }

        private bool HasTwoLevel8ForXyz()
        {
            return GetRank8Materials().Count >= 2;
        }

        private bool SelectRank8Materials()
        {
            List<ClientCard> materials = GetRank8Materials();
            if (materials.Count < 2)
                return false;

            AI.SelectMaterials(materials.Take(2).ToList());
            return true;
        }

        private bool HasActiveKingsSarcophagus()
        {
            return Bot.HasInSpellZone(CardId.KingsSarcophagus, true, true)
                || Bot.HasInSpellZone(CardId.WallsOfTheImperialTomb, true, true);
        }

        private int GetColumnCardCount(int column)
        {
            return Bot.GetColumnCount(column) + Enemy.GetColumnCount(4 - column);
        }

        private int GetMekkKnightSummonZones()
        {
            int zones = 0;
            for (int column = 0; column < 5; ++column)
            {
                if (Bot.MonsterZone[column] == null && GetColumnCardCount(column) >= 2)
                    zones |= 1 << column;
            }
            return zones;
        }

        private int GetMekkKnightSetupSpellZones()
        {
            int zones = 0;
            for (int column = 0; column < 5; ++column)
            {
                if (Bot.MonsterZone[column] == null
                    && Bot.SpellZone[column] == null
                    && GetColumnCardCount(column) >= 1)
                    zones |= 1 << column;
            }
            return zones;
        }

        private bool HasMekkKnightSummonInHand()
        {
            return (Bot.HasInHand(CardId.MekkKnightPurpleNightfall) && !_purpleNightfallSummoned)
                || (Bot.HasInHand(CardId.MekkKnightIndigoEclipse) && !_indigoEclipseSummoned);
        }

        private bool ShouldSummonArtemis()
        {
            if (!Bot.HasInHand(CardId.Invocation)
                || Bot.HasInMonstersZone(CardId.ArtemisTheMagistusMoonMaiden, faceUp: true))
                return false;

            bool hasAleisterOnField = Bot.GetMonsters().Any(c =>
                c.IsFaceup() && c.IsCode(CardId.AleisterTheInvoker));
            if (!hasAleisterOnField)
                return false;

            bool needsLightMaterial = !Bot.Graveyard.Concat(Enemy.Graveyard)
                .Any(c => c.HasAttribute(CardAttribute.Light));
            bool preparesMekkKnightColumn = HasMekkKnightSummonInHand()
                && GetMekkKnightSummonZones() == 0;
            return needsLightMaterial || preparesMekkKnightColumn;
        }

        private ClientCard GetHorusEngineDiscard(ClientCard excluded = null)
        {
            int[] priority = {
                CardId.HapiGuidanceOfHorus,
                CardId.DuamutefBlessingOfHorus,
                CardId.QebehsenuefProtectionOfHorus,
                CardId.ImsetyGloryOfHorus,
                CardId.Terraforming,
                CardId.WallsOfTheImperialTomb,
                CardId.MagicalMeltdown,
                CardId.TradeIn,
                CardId.MekkKnightIndigoEclipse,
                CardId.MekkKnightPurpleNightfall,
                CardId.NemesesCorridor,
                CardId.AleisterTheInvoker,
                CardId.Raigeki,
                CardId.HeavyStorm,
                CardId.HarpiesFeatherDuster,
            };
            foreach (int id in priority)
            {
                ClientCard discard = Bot.Hand.FirstOrDefault(c => c != excluded && c.IsCode(id));
                if (discard != null)
                    return discard;
            }
            return Bot.Hand.FirstOrDefault(c =>
                c != excluded
                && !c.IsCode(CardId.MonsterReborn)
                && !c.IsCode(CardId.Invocation));
        }

        private List<int> GetHorusMonsterPriority()
        {
            return HorusMonsterIds
                .OrderBy(id => Bot.HasInHand(id)
                    || Bot.HasInGraveyard(id)
                    || Bot.HasInMonstersZone(id) ? 1 : 0)
                .ToList();
        }

        // ============================================================
        // 魔法/陷阱效果
        // ============================================================

        private bool HarpiesFeatherDusterEffect()
        {
            return Enemy.GetSpellCount() > 0;
        }

        private bool HeavyStormEffect()
        {
            return !HasActiveKingsSarcophagus() && DefaultHeavyStorm();
        }

        private bool MagicalMeltdownEffect()
        {
            if (Bot.HasInSpellZone(CardId.MagicalMeltdown, true, true)) return false;
            return DefaultField();
        }

        private bool KingsSarcophagusActivate()
        {
            if (Card.Location != CardLocation.Hand)
                return false;

            bool hasKingsSarcophagus = Bot.SpellZone.Take(5).Any(c =>
                c != null
                && c.IsCode(CardId.KingsSarcophagus)
                && c.IsFaceup()
                && !c.IsDisabled());
            if (hasKingsSarcophagus)
                return false;

            if (HasMekkKnightSummonInHand())
            {
                int zones = GetMekkKnightSetupSpellZones();
                if (zones != 0)
                    AI.SelectPlace(zones);
            }
            return true;
        }

        private bool KingsSarcophagusEffect()
        {
            if (Card.Location != CardLocation.SpellZone
                || DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (!(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
            {
                if (Bot.BattlingMonster == null || Enemy.BattlingMonster == null)
                    return false;
                bool shouldActivate = Enemy.BattlingMonster.IsFacedown() || Enemy.BattlingMonster.IsMonsterInvincible()
                    || Bot.BattlingMonster.GetDefensePower() <= Enemy.BattlingMonster.GetDefensePower();
                if (shouldActivate)
                    _kingsSarcophagusBattleEffectUsed = true;
                return shouldActivate;
            }

            if (Bot.GetMonsterCount() >= 5)
                return false;

            List<int> worthwhileHorusMonsters = HorusMonsterIds
                .Where(id => !_horusSpecialSummonedThisTurn.Contains(id)
                    && !Bot.HasInGraveyard(id)
                    && Bot.HasInDeck(id))
                .ToList();
            if (worthwhileHorusMonsters.Count == 0)
                return false;

            ClientCard discard = GetHorusEngineDiscard();
            if (discard == null)
                return false;

            AI.SelectCard(discard);
            AI.SelectNextCard(worthwhileHorusMonsters);
            return true;
        }

        private bool WallsOfTheImperialTombEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.SpellZone[5] == null)
                    return true;

                if (Bot.SpellZone[5].IsCode(CardId.WallsOfTheImperialTomb, CardId.KingsSarcophagus))
                    return false;

                // 暴走魔法阵已经完成检索后，换成石壁。
                return Bot.SpellZone[5].IsCode(CardId.MagicalMeltdown)
                    && (Bot.HasInHandOrHasInMonstersZone(CardId.AleisterTheInvoker)
                        || Bot.HasInGraveyard(CardId.AleisterTheInvoker)
                        || _aleisterSearchedInvocation);
            }

            if (DefaultCheckWhetherCardIsNegated(Card))
                return false;

            if (Bot.Hand.Count < 2)
                return false;

            bool searchImsety = !Bot.HasInHand(CardId.ImsetyGloryOfHorus)
                && !Bot.HasInGraveyard(CardId.ImsetyGloryOfHorus);

            bool needLevel8 = Bot.HasInHand(CardId.TradeIn)
                && !Bot.Hand.IsExistingMatchingCard(c => c.Level == 8);

            if (!searchImsety && !needLevel8)
                return false;

            AI.SelectCard(GetHorusMonsterPriority());
            AI.SelectNextCard(
                CardId.WallsOfTheImperialTomb,
                CardId.HeavyStorm,
                CardId.HarpiesFeatherDuster,
                CardId.Raigeki,
                CardId.QebehsenuefProtectionOfHorus,
                CardId.HapiGuidanceOfHorus,
                CardId.DuamutefBlessingOfHorus
            );
            return true;
        }

        private bool TerraformingEffect()
        {
            bool hasAleisterAccess = Bot.HasInHandOrHasInMonstersZone(CardId.AleisterTheInvoker)
                || Bot.HasInGraveyard(CardId.AleisterTheInvoker);
            if (!hasAleisterAccess
                && !Bot.HasInHandOrInSpellZone(CardId.MagicalMeltdown))
            {
                AI.SelectCard(CardId.MagicalMeltdown, CardId.WallsOfTheImperialTomb);
                return true;
            }

            if (!HasActiveKingsSarcophagus()
                && !Bot.HasInHandOrInSpellZone(CardId.WallsOfTheImperialTomb))
            {
                AI.SelectCard(CardId.WallsOfTheImperialTomb, CardId.MagicalMeltdown);
                return true;
            }

            if (!Bot.HasInHandOrInSpellZone(CardId.MagicalMeltdown))
            {
                AI.SelectCard(CardId.MagicalMeltdown, CardId.WallsOfTheImperialTomb);
                return true;
            }

            AI.SelectCard(CardId.WallsOfTheImperialTomb, CardId.MagicalMeltdown);
            return true;
        }

        private bool FoolishBurialEffect()
        {
            AI.SelectCard(GetHorusMonsterPriority());
            return true;
        }

        private bool TradeInEffect()
        {
            // 荷鲁斯在墓地可以借王之棺自跳，优先作为抵价购物的 cost。
            int[] discardPriority = {
                CardId.HapiGuidanceOfHorus,
                CardId.DuamutefBlessingOfHorus,
                CardId.QebehsenuefProtectionOfHorus,
                CardId.ImsetyGloryOfHorus,
                CardId.MekkKnightIndigoEclipse,
                CardId.MekkKnightPurpleNightfall,
            };
            foreach (int id in discardPriority)
            {
                if (Bot.HasInHand(id))
                {
                    AI.SelectCard(id);
                    return true;
                }
            }
            return false;
        }

        private bool MonsterRebornEffect()
        {
            IEnumerable<ClientCard> ownTargets = Bot.Graveyard
                .Where(c => !c.IsCode(HorusMonsterIds) || !HasActiveKingsSarcophagus());
            ClientCard best = ownTargets.Concat(Enemy.Graveyard)
                .Where(c => c.HasType(CardType.Monster)
                    && c.IsCanRevive())
                .OrderByDescending(c => c.Attack)
                .FirstOrDefault();
            if (best == null)
                return false;

            AI.SelectCard(best);
            return true;
        }

        private bool InvocationEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(CardId.AleisterTheInvoker);
                return true;
            }

            bool canSummonArtemis = Duel.MainPhase.SpecialSummonableCards.Any(c =>
                c.IsCode(CardId.ArtemisTheMagistusMoonMaiden));
            if (ShouldSummonArtemis()
                && (canSummonArtemis
                    || Duel.MainPhase.SummonableCards.Any(c => c.IsCode(CardId.AleisterTheInvoker))))
                return false;

            if (Bot.HasInMonstersZone(CardId.ArtemisTheMagistusMoonMaiden, faceUp: true)
                && HasMekkKnightSummonInHand()
                && (GetMekkKnightSummonZones() != 0
                    || GetMekkKnightSetupSpellZones() != 0))
                return false;

            ClientCard aleister =
                Bot.Graveyard.FirstOrDefault(c => c.IsCode(CardId.AleisterTheInvoker))
                ?? Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.AleisterTheInvoker))
                ?? Bot.Hand.FirstOrDefault(c => c.IsCode(CardId.AleisterTheInvoker))
                ?? Enemy.Graveyard.FirstOrDefault(c => c.IsCode(CardId.AleisterTheInvoker));
            if (aleister == null) return false;

            ClientCard lightMat =
                Enemy.Graveyard.Where(c => c.HasAttribute(CardAttribute.Light)).OrderByDescending(c => c.Attack).FirstOrDefault()
                ?? Bot.Graveyard.FirstOrDefault(c => c.HasAttribute(CardAttribute.Light))
                ?? Bot.MonsterZone.FirstOrDefault(c => c != null && c.IsCode(CardId.ArtemisTheMagistusMoonMaiden))
                ?? Bot.Hand.FirstOrDefault(c => c.HasAttribute(CardAttribute.Light))
                ?? Bot.MonsterZone.FirstOrDefault(c => c != null && c.HasAttribute(CardAttribute.Light));

            if (lightMat == null) return false;

            AI.SelectCard(CardId.InvokedMechaba);
            List<ClientCard> mats = new List<ClientCard> { aleister, lightMat };
            AI.SelectMaterials(mats);
            AI.SelectPosition(CardPosition.FaceUpAttack);
            return true;
        }

        private bool InvokedMechabaEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Duel.LastChainPlayer != 1) return false;

            // 选择消息只会给出与被连锁效果同种类的手牌，按资源价值提供统一优先级即可。
            AI.SelectCard(
                CardId.Terraforming,
                CardId.WallsOfTheImperialTomb,
                CardId.MagicalMeltdown,
                CardId.TradeIn,
                CardId.NemesesCorridor,
                CardId.MekkKnightIndigoEclipse,
                CardId.HapiGuidanceOfHorus,
                CardId.DuamutefBlessingOfHorus,
                CardId.QebehsenuefProtectionOfHorus,
                CardId.MekkKnightPurpleNightfall,
                CardId.ImsetyGloryOfHorus,
                CardId.AleisterTheInvoker,
                CardId.Raigeki,
                CardId.HeavyStorm,
                CardId.HarpiesFeatherDuster,
                CardId.MonsterReborn,
                CardId.Invocation);
            return true;
        }

        // ============================================================
        // 荷鲁斯怪兽效果
        // ============================================================

        private bool HorusMonsterSpSummon()
        {
            if (Card.IsCode(CardId.DuamutefBlessingOfHorus))
                AI.SelectPosition(CardPosition.FaceUpAttack);
            _horusSpecialSummonedThisTurn.Add(Card.Id);
            return true;
        }

        private bool ImsetyEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (Card.Location == CardLocation.Hand)
            {
                ClientCard discard = GetHorusEngineDiscard(Card)
                    ?? Bot.Hand.FirstOrDefault(c => c != Card);
                if (discard == null)
                    return false;
                AI.SelectCard(discard);
                return true;
            }

            ClientCard target = Util.GetProblematicEnemyCard(0, true)
                ?? Util.GetBestEnemyMonster(true, true)
                ?? Enemy.GetSpells().FirstOrDefault();
            if (target == null) return false;
            AI.SelectCard(target);
            return true;
        }

        private bool HapiEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            List<ClientCard> ownTargets = Bot.Graveyard.Concat(Bot.Banished)
                .Where(c => !HasActiveKingsSarcophagus() || !c.IsCode(HorusMonsterIds))
                .OrderByDescending(c => c.IsCode(
                    CardId.HarpiesFeatherDuster,
                    CardId.HeavyStorm,
                    CardId.Raigeki,
                    CardId.MonsterReborn,
                    CardId.Invocation,
                    CardId.AleisterTheInvoker))
                .ThenByDescending(c => c.Attack)
                .Take(2)
                .ToList();
            if (ownTargets.Count == 2)
            {
                AI.SelectCard(ownTargets);
                AI.SelectOption(0);
                return true;
            }

            List<ClientCard> enemyTargets = Enemy.Graveyard.Concat(Enemy.Banished)
                .OrderByDescending(c => c.IsMonsterDangerous() || c.IsMonsterInvincible())
                .ThenByDescending(c => c.Attack)
                .Take(2 - ownTargets.Count)
                .ToList();
            if (ownTargets.Count + enemyTargets.Count < 2)
                return false;

            AI.SelectCard(ownTargets.Concat(enemyTargets).ToList());
            AI.SelectOption(1);
            return true;
        }

        // ============================================================
        // 机界骑士效果
        // ============================================================

        private bool MekkKnightPurpleNightfallEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            bool needsTradeInMaterial =
                Bot.HasInHand(CardId.TradeIn) &&
                !Bot.Hand.Any(card => card.Level == 8);
            int sourceColumn = Card.Sequence < 5 ? Card.Sequence
                : Card.Sequence == 5 ? 1
                : Card.Sequence == 6 ? 3
                : -1;
            bool preparesMekkKnightColumn = false;
            if (!_indigoEclipseSummoned)
            {
                for (int column = 0; column < 5; ++column)
                {
                    bool monsterZoneEmptyAfterBanish =
                        Bot.MonsterZone[column] == null ||
                        Card.Sequence == column;
                    int columnCardCountAfterBanish =
                        GetColumnCardCount(column) -
                        (column == sourceColumn ? 1 : 0);
                    if (monsterZoneEmptyAfterBanish &&
                        columnCardCountAfterBanish >= 2)
                    {
                        preparesMekkKnightColumn = true;
                        break;
                    }
                }
            }
            if (!needsTradeInMaterial && !preparesMekkKnightColumn)
                return false;

            AI.SelectCard(Card);
            AI.SelectNextCard(CardId.MekkKnightIndigoEclipse);
            return true;
        }

        private bool MekkKnightIndigoEclipseEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (!HasMekkKnightSummonInHand())
                return false;

            // 选择移走后原纵列仍保留两张卡的机界骑士，继续满足手卡中机界骑士的纵列条件。
            ClientCard target = Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.IsCode(
                    CardId.MekkKnightPurpleNightfall,
                    CardId.MekkKnightIndigoEclipse))
                .OrderByDescending(c => c == Card)
                .FirstOrDefault(c =>
                {
                    int sourceColumn = c.Sequence < 5 ? c.Sequence
                        : c.Sequence == 5 ? 1
                        : c.Sequence == 6 ? 3
                        : -1;
                    return sourceColumn >= 0
                        && GetColumnCardCount(sourceColumn) >= 3;
                });
            if (target == null)
                return false;

            AI.SelectCard(target);
            // 卡组里两种机界骑士都只会存在于主怪兽区域，不考虑额外怪兽区域，不用选具体移动目标位置
            return true;
        }

        private bool MekkKnightSpSummon()
        {
            int zones = GetMekkKnightSummonZones();
            if (zones != 0)
                AI.SelectPlace(zones);
            if (Card.IsCode(CardId.MekkKnightPurpleNightfall))
                _purpleNightfallSummoned = true;
            else if (Card.IsCode(CardId.MekkKnightIndigoEclipse))
                _indigoEclipseSummoned = true;
            return true;
        }

        // ============================================================
        // 普通召唤
        // ============================================================

        private bool AleisterSummon()
        {
            // 若还未搜索召唤魔术，召唤阿莱斯特
            if (!_aleisterSearchedInvocation) return true;
            // 已经搜索过但还没使用召唤魔术，也可以召唤
            if (!Bot.HasInGraveyard(CardId.Invocation)
                && !Bot.HasInSpellZone(CardId.Invocation)
                && !Bot.HasInHand(CardId.Invocation))
                return false;
            return !Bot.HasInMonstersZone(CardId.InvokedMechaba);
        }

        private bool AleisterEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                ClientCard fusion = Bot.BattlingMonster;
                ClientCard opponent = Enemy.BattlingMonster;
                if (fusion == null || !fusion.HasType(CardType.Fusion))
                    return false;

                bool changesBattle = opponent != null
                    && fusion.GetDefensePower() <= opponent.GetDefensePower()
                    && fusion.GetDefensePower() + 1000 > opponent.GetDefensePower();
                bool enablesLethal = opponent == null
                    && fusion.IsAttack()
                    && fusion.Attack < Enemy.LifePoints
                    && fusion.Attack + 1000 >= Enemy.LifePoints;
                if (!changesBattle && !enablesLethal)
                    return false;

                AI.SelectCard(fusion);
                return true;
            }

            if (!_aleisterSearchedInvocation)
            {
                AI.SelectCard(CardId.Invocation);
                _aleisterSearchedInvocation = true;
                return true;
            }
            return false;
        }

        private bool NemesesCorridorEffect()
        {
            ClientCard banished = Bot.Banished
                .Where(c => c.IsFaceup() && !c.IsCode(CardId.NemesesCorridor))
                .OrderBy(c => c.IsCode(CardId.AleisterTheInvoker) ? 2
                    : c.IsCode(CardId.MekkKnightPurpleNightfall) ? 1 : 0)
                .FirstOrDefault();
            if (banished != null)
            {
                AI.SelectCard(banished);
                return true;
            }
            return false;
        }

        // ============================================================
        // XYZ 召唤
        // ============================================================

        private bool MinervaSummon()
        {
            List<ClientCard> materials = Bot.GetMonsters()
                .Where(m => m.IsFaceup() && m.Level == 4 && !m.HasType(CardType.Xyz))
                .OrderBy(m => m.Attack)
                .Take(2)
                .ToList();
            if (!Util.IsTurn1OrMain2())
                return false;

            AI.SelectMaterials(materials);
            return true;
        }

        private bool MinervaEffect()
        {
            return !DefaultCheckWhetherCardIsNegated(Card);
        }

        private bool Number38Summon()
        {
            if (!HasTwoLevel8ForXyz()) return false;
            if (!Util.IsTurn1OrMain2()) return false;
            return SelectRank8Materials();
        }

        private bool Number38Effect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (ActivateDescription == Util.GetStringId(CardId.Number38HopeHarbingerDragonTitanicGalaxy, 0))
                return Duel.LastChainPlayer == 1;

            if (ActivateDescription == Util.GetStringId(CardId.Number38HopeHarbingerDragonTitanicGalaxy, 1))
                return true;

            if (ActivateDescription == Util.GetStringId(CardId.Number38HopeHarbingerDragonTitanicGalaxy, 2))
            {
                AI.SelectCard(Card);
                return true;
            }

            return false;
        }

        private bool GarunixSummon()
        {
            if (!HasTwoLevel8ForXyz()) return false;
            if (Enemy.GetMonsterCount() < 2) return false;

            int remainingOwnMonsters = Bot.GetMonsterCount() - 2;
            if (remainingOwnMonsters >= Enemy.GetMonsterCount())
                return false;
            return SelectRank8Materials();
        }

        private bool GarunixEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (ActivateDescription == Util.GetStringId(CardId.GarunixEternityHyangOfTheFireKings, 0))
                return Enemy.GetMonsterCount() > 0;

            if (ActivateDescription != Util.GetStringId(CardId.GarunixEternityHyangOfTheFireKings, 1)
                || !Card.HasXyzMaterial())
                return false;

            ClientCard target = Util.GetProblematicEnemyCard(0, true);
            if (target == null || target.Location != CardLocation.SpellZone)
                target = Enemy.GetSpells().FirstOrDefault();
            if (target == null) return false;
            AI.SelectCard(target);
            return true;
        }

        private bool DingirsuSummon()
        {
            if (!HasTwoLevel8ForXyz()) return false;
            if (Util.GetProblematicEnemyCard() == null && !Util.IsTurn1OrMain2()) return false;
            return SelectRank8Materials();
        }

        private bool DingirsuEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (ActivateDescription == 96)
                return true;

            ClientCard target = Util.GetProblematicEnemyCard()
                ?? Util.GetBestEnemyMonster(true)
                ?? Enemy.GetSpells().FirstOrDefault();
            if (target != null)
            {
                AI.SelectOption(0);
                AI.SelectCard(target);
                return true;
            }

            ClientCard material = Bot.Banished.FirstOrDefault(c => c.IsFaceup()
                && c.HasRace(CardRace.Machine)
                && c.HasType(CardType.Monster));
            if (material == null)
                return false;

            // 只有吸收素材这一项可选时，它仍是选项列表中的第 0 项。
            AI.SelectOption(0);
            AI.SelectCard(material);
            return true;
        }

        private bool ZombieVampireSummon()
        {
            if (!HasTwoLevel8ForXyz()) return false;
            if (_zombieVampireEffectUsed || Bot.HasInMonstersZone(CardId.TheZombieVampire))
                return false;
            if (!Util.IsTurn1OrMain2() || Util.GetProblematicEnemyCard() != null)
                return false;
            return SelectRank8Materials();
        }

        private bool ZombieVampireEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            _zombieVampireEffectUsed = true;
            return true;
        }

        private bool Number39UtopiaSummon()
        {
            if (!DefaultNumberS39UtopiaTheLightningSummon())
                return false;

            List<ClientCard> materials = Bot.GetMonsters()
                .Where(m => m.IsFaceup() && m.Level == 4 && !m.HasType(CardType.Xyz))
                .OrderBy(m => m.IsCode(CardId.AleisterTheInvoker) ? 1 : 0)
                .ThenBy(m => m.Attack)
                .Take(2)
                .ToList();
            AI.SelectMaterials(materials);
            return true;
        }

        private bool ZeusSummon()
        {
            if (!ShouldUseZeusBoardWipe())
                return false;

            ClientCard material = Bot.GetMonsters()
                .Where(c => c.IsFaceup()
                    && c.HasType(CardType.Xyz)
                    && c.HasXyzMaterial())
                .OrderByDescending(c => c.Overlays.Count)
                .ThenBy(c => c.Attack)
                .FirstOrDefault();
            if (material == null)
                return false;

            AI.SelectMaterials(new List<ClientCard> { material });
            return true;
        }

        private bool ShouldUseZeusBoardWipe()
        {
            int enemyCards = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
            int ownCardsLost = Math.Max(0, Bot.GetMonsterCount() + Bot.GetSpellCount() - 1);
            return enemyCards > ownCardsLost
                || Util.GetProblematicEnemyCard() != null;
        }

        private bool ZeusEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (ActivateDescription == Util.GetStringId(CardId.DivineArsenalAAZEUSSkyThunder, 2))
            {
                AI.SelectCard(
                    CardId.ArtemisTheMagistusMoonMaiden,
                    CardId.MinervaTheExaltedLightsworn,
                    CardId.Number39Utopia,
                    CardId.NumberS39UtopiaTheLightning,
                    CardId.TheZombieVampire);
                return true;
            }

            return ActivateDescription == Util.GetStringId(CardId.DivineArsenalAAZEUSSkyThunder, 1)
                && (ShouldUseZeusBoardWipe()
                    || Util.IsChainTarget(Card));
        }

        private bool SPLittleKnightSummon()
        {
            if (Util.GetProblematicEnemyCard(3001, true) == null)
                return false;

            List<ClientCard> materials = Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.HasType(CardType.Effect))
                .ToList();

            List<ClientCard> extraDeckMaterials = materials
                .Where(c => c.HasType(CardType.Fusion | CardType.Synchro | CardType.Xyz | CardType.Link))
                .OrderBy(c => c.Attack)
                .ToList();
            ClientCard extraDeckMaterial = extraDeckMaterials
                .FirstOrDefault(c => c.IsDisabled()
                    || c.IsCode(CardId.ArtemisTheMagistusMoonMaiden)
                    || (c.HasType(CardType.Xyz) && !c.HasXyzMaterial()));
            if (extraDeckMaterial == null)
                extraDeckMaterial = extraDeckMaterials.FirstOrDefault();
            if (extraDeckMaterial == null)
                return false;

            ClientCard otherMaterial = materials
                .Where(c => c != extraDeckMaterial)
                .OrderBy(c => c.Attack)
                .FirstOrDefault();
            if (otherMaterial == null)
                return false;

            AI.SelectMaterials(new List<ClientCard> { extraDeckMaterial, otherMaterial });
            return true;
        }

        private bool SPLittleKnightEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            if (ActivateDescription == -1
                || ActivateDescription == Util.GetStringId(CardId.SPLittleKnight, 0))
            {
                ClientCard target = Util.GetProblematicEnemyCard(3000, true)
                    ?? Util.GetBestEnemyMonster(false, true)
                    ?? Enemy.Graveyard.Where(c => c.Id != 0).OrderByDescending(c => c.Attack).FirstOrDefault();
                if (target == null) return false;
                AI.SelectCard(target);
                return true;
            }

            if (ActivateDescription != Util.GetStringId(CardId.SPLittleKnight, 1)
                || Duel.LastChainPlayer != 1)
                return false;

            ClientCard selfTarget = Bot.GetMonsters().FirstOrDefault(c => Util.IsChainTarget(c)) ?? Card;
            ClientCard enemyTarget = Util.GetProblematicEnemyCard(0, true);
            if (enemyTarget == null
                || enemyTarget.Location != CardLocation.MonsterZone
                || !enemyTarget.IsFaceup())
                enemyTarget = Enemy.GetMonsters().Where(c => c.IsFaceup()).OrderByDescending(c => c.Attack).FirstOrDefault();
            if (selfTarget == null || enemyTarget == null)
                return false;

            AI.SelectCard(selfTarget);
            AI.SelectNextCard(enemyTarget);
            return true;
        }

        private bool ArtemisSummon()
        {
            if (!ShouldSummonArtemis())
                return false;

            AI.SelectMaterials(new[] { CardId.AleisterTheInvoker });
            return true;
        }

        private bool MekkKnightSetupSpellSet()
        {
            if (!HasMekkKnightSummonInHand()
                || GetMekkKnightSummonZones() != 0
                || Card.IsCode(CardId.MagicalMeltdown, CardId.WallsOfTheImperialTomb))
                return false;

            int zones = GetMekkKnightSetupSpellZones();
            if (zones == 0)
                return false;

            AI.SelectPlace(zones);
            return true;
        }

        // ============================================================
        // 战斗力预估：王之棺可以处理战斗对象，手牌阿莱斯特可以让梅尔卡巴上升 1000
        // ============================================================

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!_kingsSarcophagusBattleEffectUsed
                && attacker.IsCode(HorusMonsterIds)
                && Bot.HasInSpellZone(CardId.KingsSarcophagus, true, true)
                && !defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                attacker.RealPower = 9999;
                if (defender.IsMonsterInvincible())
                    return true;
            }

            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.InvokedMechaba)
                    && Bot.HasInHand(CardId.AleisterTheInvoker))
                {
                    attacker.RealPower = attacker.Attack + 1000;
                }
            }
            return base.OnPreBattleBetween(attacker, defender);
        }
    }
}
