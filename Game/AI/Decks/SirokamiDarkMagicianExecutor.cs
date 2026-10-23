using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    /// <summary>
    /// 「阿图姆」——黑魔术师 / 混沌黑魔术骑士（DIY）卡组。
    ///
    /// 主展开线（幕帘线）：
    ///   杖 → 检索「黑魔导的幕帘」→ 幕帘特召黑魔术师 + 追加检索
    ///   → 本回合已发动魔法卡，除外黑魔术师特召「毁灭之黑魔术师」
    ///   → 毁灭②检索（永远之魂 / 黑魔导阵 / 蒂迈欧之眼光）
    ///   → 蒂迈欧之眼光把毁灭变成「龙骑士 黑魔术师」
    ///   终场：黑魔导阵 + 龙骑士 黑魔术师 + 永远之魂
    ///
    /// 备用线（幕帘组件不全时）：
    ///   超魔导师-黑魔术师徒（融合）+ 师徒的牵绊 → 少女 + 盖「黑·爆·裂·破·魔·导」
    ///
    /// 注意：本卡组含 2 张 DIY 卡，启动时必须同时加载 DIY 卡库：
    ///   DbPath=cards.cdb;DIY_Sirokami.cdb
    /// </summary>
    [Deck("SirokamiDarkMagician", "AI_SirokamiDarkMagician", "Normal")]
    public class SirokamiDarkMagicianExecutor : DarkMagicianExecutor
    {
        public class CardId
        {
            // 怪兽
            public const int DarkMagician = 46986414;                 // 黑魔术师
            public const int ChaosBlackMagicKnight = 98920831;        // 混沌黑魔术骑士（DIY）
            public const int Timaeus = 3078380;                       // 合体龙 蒂迈欧
            public const int Radian = 28674152;                       // 多次元坏兽 拉迪安
            public const int PalladiumOracleMahad = 71703785;         // 守护神官 马哈德
            public const int ServantOfTheKing = 88570004;             // 王之仆人-黑魔术师
            public const int PalladiumOracleMana = 42006475;          // 守护神官 玛娜
            public const int DarkMagicianGirl = 38033129;             // 黑魔术少女
            public const int MagiciansRod = 7084129;                  // 魔术师之杖
            public const int GhostMourner = 52038441;                 // 朔夜时雨
            public const int Magikuriboh = 31699677;                  // 魔术栗子球
            public const int MagiciansSouls = 97631303;               // 魔术师双魂
            public const int IllusionOfChaos = 12266229;              // 混沌之幻想魔术师

            // 魔法
            public const int DarkMagicCoffin = 64500034;              // 黑魔术之转生棺（DIY）
            public const int DarkMagicAttack = 2314238;               // 黑·魔·导
            public const int ThousandKnives = 63391643;               // 千把刀
            public const int DarkBurningMagic = 75190122;             // 黑·爆·裂·破·魔·导
            public const int SoulServant = 23020408;                  // 魂之仆人
            public const int DarkMagicalCircle = 47222536;            // 黑魔导阵
            public const int KnightsTitle = 87210505;                 // 骑士的称号
            public const int DarkMagicCurtain = 41350418;             // 黑魔导的幕帘
            public const int BondBetweenTeacherAndStudent = 60709218; // 师徒的牵绊
            public const int MagiciansSalvation = 95477924;           // 魔术师的救出
            public const int MonsterReborn = 28958464;                // 魔法卡「死者苏生」
            public const int SecretsOfDarkMagic = 59514117;           // 黑魔术的秘仪
            public const int EyeOfTimaeus = 22283204;                 // 蒂迈欧之眼光

            // 陷阱
            public const int EternalSoul = 48680970;                  // 永远之魂
            public const int DestinedRivals = 22634473;               // 对峙的宿命
            public const int MindPowerShield = 66247040;              // 神圣心灵防护罩 -心灵之力-
            public const int MagiciansCombination = 86509711;         // 魔术师的配合
            public const int MagicianNavigation = 7922915;            // 魔术师的导门阵

            // 额外
            public const int TimaeusFusion = 85899505;                // 合体魔龙 蒂迈欧
            public const int MasterOfChaos = 85059922;                // 超魔导战士-混沌制驭者
            public const int DarkMagicianTheDragonKnight = 41721210;  // 龙骑士 黑魔术师
            public const int DarkPaladin = 98502115;                  // 超魔导剑士-黑魔导剑士
            public const int AmuletDragon = 75380687;                 // 咒符龙
            public const int DarkMagicianOfDestruction = 59400890;    // 毁灭之黑魔术师
            public const int TheDarkMagicians = 50237654;             // 超魔导师-黑魔术师徒
            public const int Number11BigEye = 80117527;               // No.11 巨眼
            public const int EbonIllusionMagician = 96471335;         // 幻想之黑魔导师
            public const int EbonHighMagician = 85551711;             // 虚空之黑魔导师
            public const int NoritoTheMoralLeader = 14152862;         // 风纪宫司 祝词
            public const int MagiMagiMagicianGal = 10000030;          // 眼盯盯☆魔术辣妹
            public const int AccesscodeTalker = 86066372;             // 访问码语者
            public const int Selene = 45819647;                       // 神圣魔皇后 塞勒涅
            public const int RelinquishedAnima = 94259633;            // 纳祭之魔·阿尼玛

            // 2026-10 卡表新增
            public const int SwordsOfConcealingLight = 12923641;      // 暗之护封剑
            public const int PrimiteRoar = 92501449;                  // 原石的鸣狞
            public const int Necromancer = 69176851;                  // 瘴烟之死灵术师（Link-2）
            public const int Sebek = 39767432;                        // 塞贝克的魔导士（Link-2）
            public const int Dharc = 8264361;                         // 暗影之暗灵使 达克（Link-2）
            public const int Daybreaker = 91336701;                   // 魔导耀士 破晓者（Link-2）
            public const int ApprenticeWitchling = 71384012;          // 见习魔娘（Link-2）

            // 魔导书轴（2026-10 翻新卡组）
            public const int Crowley = 50756327;                      // 魔导原典 克劳利（Link-2）
            public const int SpellbookOfSecrets = 89739383;           // 奥义之魔导书
            public const int SpellbookOfKnowledge = 23314220;         // 冰火之魔导书
            public const int SpellbookMagician = 14824019;            // 魔导书士 巴特尔
            public const int UpstartGoblin = 70368879;                // 成金哥布林
        }

        private const int SetcodeDarkMagician = 0xa2;

        private static readonly int[] FusionPreference =
        {
            CardId.DarkMagicianTheDragonKnight,
            CardId.TheDarkMagicians,
            CardId.AmuletDragon,
            CardId.DarkPaladin,
            CardId.DarkMagicianOfDestruction,
            CardId.TimaeusFusion,
            CardId.MasterOfChaos
        };

        /// <summary>备用线优先出「超魔导师-黑魔术师徒」。</summary>
        private static readonly int[] FusionPreferenceDarkMagicians =
        {
            CardId.TheDarkMagicians,
            CardId.DarkMagicianTheDragonKnight,
            CardId.AmuletDragon,
            CardId.DarkPaladin,
            CardId.DarkMagicianOfDestruction,
            CardId.TimaeusFusion,
            CardId.MasterOfChaos
        };

        /// <summary>对方场面很宽、墓地又有光属性时，优先出「混沌制驭者」滚掉全场。</summary>
        private static readonly int[] FusionPreferenceMasterOfChaos =
        {
            CardId.MasterOfChaos,
            CardId.DarkMagicianTheDragonKnight,
            CardId.TheDarkMagicians,
            CardId.AmuletDragon,
            CardId.DarkPaladin,
            CardId.DarkMagicianOfDestruction,
            CardId.TimaeusFusion
        };

        private bool _chaosKnightByKnightsTitle;
        private bool _spellActivatedThisTurn;

        /// <summary>本回合是否发过「魂之仆人」：阵要等它先放顶，别抢在前面把①用掉。</summary>
        private bool _soulServantUsedThisTurn;

        /// <summary>本回合是否发动过「蒂迈欧之眼光」（它拉出来的怪兽下个回合结束时会被除外）。</summary>
        private bool _eyeActivatedThisTurn;

        /// <summary>眼光拉出来的融合怪兽 id，用来优先把它 link 掉。</summary>
        private int _eyeFusionId;

        /// <summary>幕帘结算阶段：1 = 正在等「特召哪只」的选卡。</summary>
        private int _curtainStage;

        /// <summary>正在选融合怪兽（从额外卡组选），用来保证选出我们想要的融合怪。</summary>
        private bool _pickingFusion;

        /// <summary>幻想① 结算中：1 = 等「从卡组拿卡」或「把手卡放回卡组」的选卡。</summary>
        /// <summary>眼光要点的目标（用来在 OnSelectCard 里强制挑选）</summary>
        /// <summary>王仆人盖放时的目标优先列表（用来在 OnSelectCard 里直接挑）</summary>
        private List<int> _servantSetTargets;

        private ClientCard _eyeTargetCard;

        private int _illusionStage;
        private int _illusionSearchId;

        public SirokamiDarkMagicianExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            int baseCount = Executors.Count;   // 基类已注册的条数
            // 「神圣心灵防护罩 -心灵之力-」优先盖：对方 5 张以上表侧卡时它盖放当回合就能发动
            // 开局先清场：成金先抽，再雷击/羽毛扫
            // 先把起手怪拍下去：杖/巴特尔
            // 坏兽最优先：能解就解（不取对象、不破坏，解大怪最稳）
            AddExecutor(ExecutorType.SpSummon, 36956512, KaijuSummonGadarla);
            AddExecutor(ExecutorType.SpSummon, CardId.Radian, KaijuSummon);

                        AddExecutor(ExecutorType.SpSummon, CardId.Selene, SeleneSummon);
            AddExecutor(ExecutorType.Activate, CardId.Selene, SeleneEffect);
AddExecutor(ExecutorType.Summon, CardId.MagiciansRod, () => true);
            AddExecutor(ExecutorType.Summon, CardId.DarkMagicianGirl, () => true);   // 卡手局也要有只怪站场
            // 朔夜时雨（手坑）：对面特召时丢弃，无效那只怪兽并埋伏伤害
            AddExecutor(ExecutorType.Activate, 52038441, SayoEffect);
            AddExecutor(ExecutorType.Activate, 52038442, SayoEffect);
            AddExecutor(ExecutorType.Activate, 5318639, MysticalSpaceTyphoonEffect);      // 旋风
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicAttack, DarkMagicAttackEffect);   // 黑·魔·导

            // 旋风没炸的时候就盖下去，不要捏在手里当废牌
            AddExecutor(ExecutorType.SpellSet, 5318639, () => NotOnField(5318639)
                && !Enemy.SpellZone.Any(card => card != null)
                && Bot.SpellZone.Count(card => card != null) < 3);   // 后场不要堆满
            AddExecutor(ExecutorType.Activate, 74117290, DarkWorldDealingsEffect);      // 暗黑界的取引
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin, UpstartGoblinEffect);                     // 成金哥布林
            AddExecutor(ExecutorType.Activate, 12580477, () => Enemy.GetMonsterCount() >= 1);                 // 雷击
            AddExecutor(ExecutorType.Activate, 18144507, () => Enemy.SpellZone.Any(card => card != null));     // 羽毛扫

            AddExecutor(ExecutorType.SpellSet, CardId.MindPowerShield, SetTrap);

            AddExecutor(ExecutorType.Activate, CardId.MindPowerShield, MindPowerShieldEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansCombination, CombinationEffect);
            AddExecutor(ExecutorType.Activate, CardId.DestinedRivals, DestinedRivalsEffect);
            AddExecutor(ExecutorType.Activate, CardId.GhostMourner, GhostMournerEffect);
            AddExecutor(ExecutorType.Activate, CardId.Magikuriboh, MagikuribohEffect);
            AddExecutor(ExecutorType.Activate, CardId.ChaosBlackMagicKnight, ChaosKnightEffect);
            AddExecutor(ExecutorType.Activate, CardId.PalladiumOracleMana, PalladiumOracleManaEffect);
            AddExecutor(ExecutorType.Activate, CardId.PalladiumOracleMahad, MahadEffect);

            AddExecutor(ExecutorType.Activate, CardId.DarkMagicalCircle, DarkMagicalCircleEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Selene, SeleneSummon);
            AddExecutor(ExecutorType.Activate, CardId.Selene, SeleneEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansSalvation, SalvationEffect);
            AddExecutor(ExecutorType.Activate, CardId.SoulServant, SoulServantEffect);
            AddExecutor(ExecutorType.Activate, CardId.IllusionOfChaos, IllusionOfChaosEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansSouls, MagiciansSoulsEffect);
            AddExecutor(ExecutorType.Activate, CardId.ServantOfTheKing, ServantOfTheKingEffect);
            AddExecutor(ExecutorType.Activate, CardId.Timaeus, TimaeusEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagiciansRod, RodEffect);

            AddExecutor(ExecutorType.Summon, CardId.MagiciansRod);
            AddExecutor(ExecutorType.Summon, CardId.DarkMagician, DarkMagicianSummon);
            AddExecutor(ExecutorType.Summon, CardId.DarkMagicianGirl, DarkMagicianGirlSummon);

            // 坏兽：解对面大怪/不取对象不破坏的怪（如青眼混沌极龙）
            // 访问码语者：凑够连接值 4 且能拿来斩杀/清场时出场
            AddExecutor(ExecutorType.SpSummon, 86066373, AccesscodeSummon);
            AddExecutor(ExecutorType.Activate, 86066373, AccesscodeEffect);
            // 魔导书轴放在主轴之后（见下方），先不开
            // 新加的两张陷阱：确保会盖下去（默认 SpellSet 对反击陷阱有时会跳过）
            AddExecutor(ExecutorType.SpellSet, 96404912, () => true);   // 无偿交换
            AddExecutor(ExecutorType.SpellSet, 77538567, () => true);   // 魔宫的贿赂
            // 对方发动卡片时连锁发动（反击陷阱）
            AddExecutor(ExecutorType.Activate, 96404912, () => Duel.LastChainPlayer == 1);
            AddExecutor(ExecutorType.Activate, 77538567, () => Duel.LastChainPlayer == 1
                && !ChainTopIs(CardId.UpstartGoblin)
                // 龙骑士在场：后场不会被效果破坏，羽毛扫不值得康
                && !(ChainTopIs(18144507) && Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)));
            // 毁灭优先：先把场上的黑魔术师变成 2800 的毁灭，再谈 link
            AddExecutor(ExecutorType.SpSummon, CardId.DarkMagicianOfDestruction, DestructionSummon);
            // 魔导书轴放在毁灭之后：只在主轴走完后才拿它们的通招
            AddExecutor(ExecutorType.Summon, CardId.SpellbookMagician, () => true);
            // 魔导书轴放在主轴之后：只在主轴跑完、场面有余裕时才动，避免提前吃掉场值
            AddExecutor(ExecutorType.Activate, CardId.SpellbookMagician, SpellbookMagicianEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Selene, SeleneSummon);
            AddExecutor(ExecutorType.Activate, CardId.Selene, SeleneEffect);
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfSecrets, SpellbookOfSecretsEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Selene, SeleneSummon);
            AddExecutor(ExecutorType.Activate, CardId.Selene, SeleneEffect);
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfKnowledge, SpellbookOfKnowledgeEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Crowley, CrowleySummon);
            AddExecutor(ExecutorType.Activate, CardId.Crowley, CrowleyEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishedAnima, AnimaSummon);
            AddExecutor(ExecutorType.Activate, CardId.RelinquishedAnima, AnimaEffect);

            // 2026-10 新卡（链接怪兽放在毁灭之后，避免抢先烧掉关键怪）
            AddExecutor(ExecutorType.Activate, CardId.SwordsOfConcealingLight, SwordsOfConcealingLightEffect);
            AddExecutor(ExecutorType.Activate, CardId.PrimiteRoar, PrimiteRoarEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Necromancer, NecromancerSummon);
            // 注意：死灵术师①会把卡加手并封锁同名卡效果，这条线不用它，所以不注册 Activate
            AddExecutor(ExecutorType.SpSummon, CardId.ApprenticeWitchling, WitchlingSummon);
            AddExecutor(ExecutorType.Activate, CardId.ApprenticeWitchling, WitchlingEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Dharc, DharcSummon);
            AddExecutor(ExecutorType.Activate, CardId.Dharc, DharcEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Daybreaker, DaybreakerSummon);
            AddExecutor(ExecutorType.Activate, CardId.Daybreaker, DaybreakerEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Sebek, SebekSummon);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicCoffin, CoffinEffect);
            AddExecutor(ExecutorType.Activate, CardId.KnightsTitle, KnightsTitleEffect);
            AddExecutor(ExecutorType.Activate, CardId.EternalSoul, EternalSoulEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagicianNavigation, NavigationEffect);
            AddExecutor(ExecutorType.Activate, CardId.BondBetweenTeacherAndStudent, BondEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicCurtain, CurtainEffect);

            AddExecutor(ExecutorType.Activate, CardId.SecretsOfDarkMagic, SecretsOfDarkMagicEffect);
            AddExecutor(ExecutorType.Activate, CardId.EyeOfTimaeus, EyeOfTimaeusEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkBurningMagic, DarkBurningMagicEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicAttack, DarkMagicAttackEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThousandKnives, ThousandKnivesEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.EbonIllusionMagician, Rank7Summon);
            AddExecutor(ExecutorType.SpSummon, CardId.EbonHighMagician, EbonHighSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Number11BigEye, BigEyeSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NoritoTheMoralLeader, Rank6Summon);
            AddExecutor(ExecutorType.SpSummon, CardId.MagiMagiMagicianGal, Rank6Summon);

            AddExecutor(ExecutorType.SpSummon, CardId.AccesscodeTalker, AccesscodeSummon);

            AddExecutor(ExecutorType.Activate, CardId.TheDarkMagicians, TheDarkMagiciansEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkPaladin, DarkPaladinEffect);
            AddExecutor(ExecutorType.Activate, CardId.AmuletDragon, AmuletDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkMagicianOfDestruction, DarkMagicianOfDestructionEffect);
            AddExecutor(ExecutorType.Activate, CardId.MasterOfChaos, MasterOfChaosEffect);
            AddExecutor(ExecutorType.Activate, CardId.TimaeusFusion, TimaeusFusionEffect);
            AddExecutor(ExecutorType.Activate, CardId.EbonIllusionMagician, EbonIllusionEffect);
            AddExecutor(ExecutorType.Activate, CardId.EbonHighMagician, EbonHighEffect);
            AddExecutor(ExecutorType.Activate, CardId.Number11BigEye, BigEyeEffect);
            AddExecutor(ExecutorType.Activate, CardId.NoritoTheMoralLeader, NoritoEffect);
            AddExecutor(ExecutorType.Activate, CardId.MagiMagiMagicianGal, MagiMagiEffect);
            AddExecutor(ExecutorType.Activate, CardId.Selene, SeleneEffect);
            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, AccesscodeEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.MagiciansCombination, () => NotOnField(CardId.MagiciansCombination));
            AddExecutor(ExecutorType.SpellSet, CardId.DestinedRivals, () => NotOnField(CardId.DestinedRivals));
            AddExecutor(ExecutorType.SpellSet, CardId.MagicianNavigation, () => NotOnField(CardId.MagicianNavigation));
            // 永魂只需一张：魔陷区已经有（不管表侧还是盖着）就不再盖第二张
            AddExecutor(ExecutorType.SpellSet, CardId.DarkMagicalCircle, () => NotOnField(CardId.DarkMagicalCircle));
            // 眼光只在「本回合没法攻击」时才盖：否则盖下去当回合不能发动，
            // 会白白错过战阶的混沌斩杀 / 龙骑士
            AddExecutor(ExecutorType.SpellSet, CardId.EyeOfTimaeus, EyeTimaeusSet);
            AddExecutor(ExecutorType.SpellSet, CardId.EternalSoul,
                () => !Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EternalSoul)));
            AddExecutor(ExecutorType.Repos, ReposMonster);
            // 进战阶兜底：不加就永远不攻击
            // 能斩杀时立刻进战阶（不再继续铺场）
            AddExecutor(ExecutorType.GoToBattlePhase, () => CanLethal());
            AddExecutor(ExecutorType.GoToBattlePhase, () => true);

        

            // 把我们自己的规则提到最前面：基类的通用逻辑不认识新卡，会抢先决定行动
            System.Collections.Generic.List<CardExecutor> mine =
                Executors.Skip(baseCount).ToList();
            if (mine.Count > 0)
            {
                foreach (CardExecutor executor in mine)
                    Executors.Remove(executor);
                for (int index = mine.Count - 1; index >= 0; index--)
                    Executors.Insert(0, mine[index]);
            }}

        // =====================================================================
        //  通用
        // =====================================================================

        private static bool IsDarkMagician(ClientCard card)
        {
            if (card == null)
                return false;
            return card.IsCode(CardId.DarkMagician)
                || card.IsCode(CardId.ServantOfTheKing)
                || card.IsCode(CardId.DarkMagicianTheDragonKnight)
                || card.IsCode(CardId.DarkMagicianOfDestruction);
        }

        private bool HasDarkMagician()
        {
            return Bot.GetMonsters().Any(IsDarkMagician);
        }

        private static bool IsMagicCaster(ClientCard card)
        {
            return card != null && card.IsMonster() && card.HasRace(CardRace.SpellCaster);
        }

        private ClientCard GetCheapestSpellcaster(bool allowDarkMagician = false)
        {
            return Bot.GetMonsters()
                .Where(card => IsMagicCaster(card) && (allowDarkMagician || !IsDarkMagician(card)))
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
        }

        private ClientCard GetCheapestSpellInHand()
        {
            return Bot.Hand
                .Where(card => card != null && card.IsSpell())
                .OrderBy(card => card.IsCode(CardId.DarkMagicAttack, CardId.ThousandKnives, CardId.DarkBurningMagic) ? 1 : 0)
                .FirstOrDefault();
        }

        /// <summary>按询问顺序排队卡号：最后一张用 SelectCard，其余逆序 SelectNextCard。</summary>
        private void QueueIds(params int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;
            AI.SelectCard(ids[ids.Length - 1]);
            for (int i = ids.Length - 2; i >= 0; i--)
                AI.SelectNextCard(ids[i]);
        }

        private void QueueCardThenIds(ClientCard card, IList<int> ids)
        {
            AI.SelectCard(ids);
            AI.SelectNextCard(card);
        }

        private void QueueCardThenCard(ClientCard card, ClientCard after)
        {
            AI.SelectCard(after);
            AI.SelectNextCard(card);
        }

        private ClientCard PickEnemyCard()
        {
            ClientCard target = Util.GetBestEnemyCard(false, true);
            if (target != null)
                return target;
            return Enemy.GetMonsters().FirstOrDefault();
        }

        private int CountEnemyFaceup()
        {
            return Enemy.GetMonsters().Count(card => card.IsFaceup())
                + Enemy.GetSpells().Count(card => card.IsFaceup());
        }

        private int CountBotLevel7()
        {
            return Bot.GetMonsters().Count(card => card.Level == 7);
        }

        private int CountBotLevel6Spellcaster()
        {
            return Bot.GetMonsters().Count(card => card.Level == 6 && IsMagicCaster(card));
        }

        private int CountSoulServantKinds()
        {
            HashSet<int> kinds = new HashSet<int>();
            foreach (ClientField field in new[] { Bot, Enemy })
            {
                foreach (ClientCard card in field.GetMonsters().Concat(field.Graveyard))
                {
                    if (card == null)
                        continue;
                    if (card.IsCode(CardId.PalladiumOracleMahad))
                        kinds.Add(1);
                    else if (card.IsCode(CardId.PalladiumOracleMana))
                        kinds.Add(2);
                    else if (card.IsCode(CardId.DarkMagician))
                        kinds.Add(3);
                    else if (card.IsCode(CardId.DarkMagicianGirl))
                        kinds.Add(4);
                }
            }
            return kinds.Count;
        }

        private bool CanAccessDarkMagician()
        {
            return HasDarkMagician()
                || Bot.HasInHand(CardId.DarkMagician)
                || Bot.HasInGraveyard(CardId.DarkMagician);
        }

        /// <summary>
        /// 大怪（黑魔术师 / 毁灭 / 师徒 / 龙骑士）优先放在主要怪兽区（左·中·右），
        /// 把额外怪兽区留给链接怪兽（阿尼玛 / 死灵术师 / 塞勒涅）。
        /// 传区域掩码后由引擎在掩码内按「中→左→右」挑位。
        /// </summary>
        private void PlaceMainZone()
        {
            AI.SelectPlace(Zones.MainMonsterZones);
        }

        /// <summary>把关键决策写进 Atem-trace.log，方便复盘（出错时把这个文件发我）。</summary>
        private static void Trace(string message)
        {
            try
            {
                System.IO.File.AppendAllText(
                    "Atem-trace.log",
                    DateTime.Now.ToString("HH:mm:ss") + "  " + message + Environment.NewLine);
            }
            catch
            {
                // 写日志失败不影响对局
            }
        }

        // =====================================================================
        //  对局线路
        // =====================================================================

        private enum Plan
        {
            Setup,
            ChaosKnight,
            DarkBurning,
            DarkMagicians,
            Lethal
        }

        private Plan CurrentPlan()
        {
            if (CanLethal())
                return Plan.Lethal;
            if (Bot.HasInHand(CardId.KnightsTitle) && ChaosKnightStillAvailable())
                return Plan.ChaosKnight;
            if (Bot.HasInHand(CardId.DarkBurningMagic))
                return Plan.DarkBurning;
            if (!HasCurtainLinePieces())
                return Plan.DarkMagicians;
            return Plan.Setup;
        }

        /// <summary>主线组件：有杖或幕帘，且幕帘还没用掉。</summary>
        private bool HasCurtainLinePieces()
        {
            if (Bot.HasInGraveyard(CardId.DarkMagicCurtain))
                return false;
            if (Bot.HasInHand(CardId.DarkMagicCurtain))
                return true;
            return Bot.HasInHand(CardId.MagiciansRod)
                || Bot.HasInMonstersZone(CardId.MagiciansRod);
        }

        private bool ChaosKnightStillAvailable()
        {
            return !Bot.HasInMonstersZone(CardId.ChaosBlackMagicKnight)
                && !Bot.HasInBanished(CardId.ChaosBlackMagicKnight);
        }

        /// <summary>「骑士的称号」现在就能用：手里有、场上也有黑魔术师。</summary>
        private bool TitleIsLive()
        {
            return Bot.HasInHand(CardId.KnightsTitle)
                && HasDarkMagician()
                && ChaosKnightStillAvailable();
        }

        private bool CanLethal()
        {
            if (Duel.Turn <= 1 || Duel.Phase == DuelPhase.Main2)
                return false;

            int damage = Util.GetTotalAttackingMonsterAttack(0);
            if (damage <= 0)
                return false;

            int shield = Enemy.GetMonsters().Sum(card => card.GetDefensePower());
            return damage - shield >= Enemy.LifePoints;
        }

        private int NextSearchTarget()
        {
            if (WantsCurtain())
                return CardId.DarkMagicCurtain;

            // 检索优先级：幕帘 > 眼光 > 魂之仆人 > 阵 > 其他
            if (!Bot.HasInHand(CardId.EyeOfTimaeus) && !Bot.HasInSpellZone(CardId.EyeOfTimaeus)
                && !Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)
                && !Bot.HasInMonstersZone(CardId.TheDarkMagicians))
                return CardId.EyeOfTimaeus;

            if (!Bot.HasInHand(CardId.SoulServant) && !Bot.HasInSpellZone(CardId.SoulServant))
                return CardId.SoulServant;

            if (!Bot.HasInHand(CardId.DarkMagicalCircle) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                return CardId.DarkMagicalCircle;

            // 杖 + 双魂：检索合体龙，吃掉场上的魔陷/怪兽跳出来，再融合龙骑士保护后场
            if (Bot.HasInHand(CardId.MagiciansSouls)
                && !Bot.HasInHand(CardId.Timaeus)
                && !Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)
                && Bot.GetSpells().Any())
                return CardId.Timaeus;

            Plan plan = CurrentPlan();

            bool hasCircle = Bot.HasInSpellZone(CardId.DarkMagicalCircle)
                || Bot.HasInHand(CardId.DarkMagicalCircle);
            bool hasEternal = Bot.HasInSpellZone(CardId.EternalSoul)
                || Bot.HasInHand(CardId.EternalSoul)
                || Bot.HasInSpellZone(CardId.MagiciansSalvation)
                || Bot.HasInHand(CardId.MagiciansSalvation);

            if (plan == Plan.ChaosKnight && !HasDarkMagician()
                && !Bot.HasInHand(CardId.DarkMagician) && !Bot.HasInGraveyard(CardId.DarkMagician))
                return CardId.MagiciansSalvation;

            if (plan == Plan.DarkMagicians)
            {
                if (!hasCircle)
                    return CardId.DarkMagicalCircle;
                if (!hasEternal)
                    return CardId.MagiciansSalvation;
                // 师徒是站场抽卡引擎，先做出来；爆破的两块料由它被破坏时的②负责
                if (!Bot.HasInHand(CardId.SecretsOfDarkMagic))
                    return CardId.SecretsOfDarkMagic;
                if (!Bot.GetMonsters().Any(card => card.IsCode(CardId.DarkMagicianGirl))
                    && !Bot.HasInHand(CardId.BondBetweenTeacherAndStudent))
                    return CardId.BondBetweenTeacherAndStudent;
                return CardId.SoulServant;
            }

            if (plan == Plan.DarkBurning && HasDarkMagician()
                && !Bot.GetMonsters().Any(card => card.IsCode(CardId.DarkMagicianGirl)))
                return CardId.BondBetweenTeacherAndStudent;

            if (!hasCircle)
                return CardId.DarkMagicalCircle;
            if (!hasEternal)
                return CardId.MagiciansSalvation;
            if (!Bot.HasInHand(CardId.SoulServant))
                return CardId.SoulServant;
            if (ChaosKnightStillAvailable() && !Bot.HasInHand(CardId.KnightsTitle))
                return CardId.KnightsTitle;
            if (Enemy.GetSpells().Any(card => card.IsFaceup()))
                return CardId.DarkMagicAttack;
            if (Enemy.GetMonsters().Any())
                return CardId.ThousandKnives;

            return CardId.DarkMagicCoffin;
        }

        private List<int> SearchPriorityForSpellTrap()
        {
            List<int> list = new List<int> { NextSearchTarget() };
            list.AddRange(new[]
            {
                CardId.DarkMagicalCircle,
                CardId.MagiciansSalvation,
                CardId.EternalSoul,
                CardId.SoulServant,
                CardId.DarkMagicCoffin,
                CardId.KnightsTitle,
                CardId.MagicianNavigation,
                CardId.BondBetweenTeacherAndStudent,
                CardId.DarkBurningMagic,
                CardId.DarkMagicAttack,
                CardId.ThousandKnives,
                CardId.SecretsOfDarkMagic,
                CardId.EyeOfTimaeus,
                CardId.DarkMagicCurtain
            });
            return list;
        }

        /// <summary>永魂已经有着落（手卡 / 场上含盖着 / 手卡有救出）——不再去拿永魂。</summary>
        private bool EternalSoulCovered()
        {
            if (Bot.HasInHand(CardId.EternalSoul))
                return true;
            if (Bot.HasInHand(CardId.MagiciansSalvation))
                return true;
            return Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EternalSoul));
        }

        /// <summary>主轴第一步：没幕帘、场上也没黑魔术师时优先找幕帘。</summary>
        private bool WantsCurtain()
        {
            // 场上已有黑魔术师但**只有 1 只怪** → 仍然要幕帘，
            // 因为这只 DM 很快会被变成毁灭或 Link 掉，需要第二只顶上
            // 幕帘是主轴启动点：只要家里（手卡/墓地）还没有，就该拿
            // （不再按「场面够不够」判断——场面够也需要第二只 DM 去 Link/变毁灭）
            if (Bot.HasInHand(CardId.DarkMagicCurtain))
                return false;
            if (Bot.HasInGraveyard(CardId.DarkMagicCurtain))
                return false;
            return true;
        }

        /// <summary>幕帘的追加检索：先补终场缺件。</summary>
        private int CurtainBonusTarget()
        {
            bool hasCircle = Bot.HasInSpellZone(CardId.DarkMagicalCircle)
                || Bot.HasInHand(CardId.DarkMagicalCircle);
            bool hasEternal = Bot.HasInSpellZone(CardId.EternalSoul)
                || Bot.HasInHand(CardId.EternalSoul)
                || Bot.HasInSpellZone(CardId.MagiciansSalvation)
                || Bot.HasInHand(CardId.MagiciansSalvation);

            // 主轴：帘幕拉黑魔术师后先拿「蒂迈欧之眼光」，下一环直接点墓地出师徒/龙骑士
            if (!Bot.HasInHand(CardId.EyeOfTimaeus)
                && !Bot.HasInMonstersZone(CardId.EyeOfTimaeus))
                return CardId.EyeOfTimaeus;

            // 其次「魂之仆人」（后面靠它放王之仆人 + ②抽卡）
            if (!Bot.HasInHand(CardId.SoulServant)
                && !Bot.HasInMonstersZone(CardId.SoulServant))
                return CardId.SoulServant;

            if (!hasEternal)
                return CardId.MagiciansSalvation;
            if (!hasCircle)
                return CardId.DarkMagicalCircle;
            if (!Bot.HasInHand(CardId.BondBetweenTeacherAndStudent))
                return CardId.BondBetweenTeacherAndStudent;
            return CardId.DarkMagicalCircle;
        }

        /// <summary>
        /// 毁灭② 的检索。
        /// 注：曾把「帘幕」放第一位（原石线要它），但 100 局从 39% 掉到 27% —— 帘幕在
        /// 幕帘起手的线里早就在手/墓地了，反而把「王之仆人」这个真正的铺场点挤掉。已回滚。
        /// 用户给的原石线（原石→毁灭拿帘幕→帘幕拉少女→魂仆放眼光→抽2→师徒→塞勒涅拉回）
        /// 要按线路做，不能靠全局优先级。
        /// </summary>
        private int DestructionSearchTarget()
        {
            // 王之仆人 > 黑魔导阵 > 魔术师的救出 > 对峙的宿命 > 魂之仆人
            if (!Bot.HasInHand(CardId.ServantOfTheKing) && !Bot.HasInMonstersZone(CardId.ServantOfTheKing))
                return CardId.ServantOfTheKing;
            if (!Bot.HasInHand(CardId.EyeOfTimaeus) && !Bot.HasInSpellZone(CardId.EyeOfTimaeus)
                && !Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)
                && !Bot.HasInMonstersZone(CardId.TheDarkMagicians))
                return CardId.EyeOfTimaeus;
            if (!Bot.HasInHand(CardId.DarkMagicalCircle) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                return CardId.DarkMagicalCircle;
            if (!Bot.HasInHand(CardId.MagiciansSalvation) && !Bot.HasInSpellZone(CardId.MagiciansSalvation))
                return CardId.MagiciansSalvation;
            if (!Bot.HasInHand(CardId.DestinedRivals) && !Bot.HasInSpellZone(CardId.DestinedRivals))
                return CardId.DestinedRivals;
            if (!Bot.HasInHand(CardId.SoulServant) && !Bot.HasInSpellZone(CardId.SoulServant))
                return CardId.SoulServant;
            return CardId.SoulServant;
        }

        // =====================================================================
        //  手牌诱发 / 反制
        // =====================================================================

        private bool GhostMournerEffect()
        {
            if (Duel.LastChainPlayer == 0)
                return false;

            ClientCard target = null;
            if (Duel.LastChainPlayer == 1)
                target = Util.GetLastChainCard();

            if (target == null && Duel.LastSummonPlayer == 1)
                target = Duel.LastSummonedCards
                    .Where(card => card != null && card.Controller == 1 && card.IsMonster() && card.IsFaceup())
                    .OrderByDescending(card => card.IsExtraCard() ? 1 : 0)
                    .ThenByDescending(card => card.Attack)
                    .FirstOrDefault();

            if (target == null || !target.IsMonster() || target.Controller != 1)
                return false;
            if (target.Attack < 1500 && !target.HasType(CardType.Effect))
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool MagikuribohEffect()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Card.Location != CardLocation.Hand)
                return false;
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2
                && Duel.Phase != DuelPhase.BattleStart && Duel.Phase != DuelPhase.Battle)
                return false;
            if (HasDarkMagician() || Bot.HasInMonstersZone(CardId.DarkMagicianGirl))
                return false;

            if (Bot.HasInGraveyard(CardId.DarkMagician))
                AI.SelectCard(CardId.DarkMagician);
            else
                AI.SelectCard(new List<int> { CardId.DarkMagician, CardId.DarkMagicianGirl });
            return true;
        }

        private bool MahadEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(CardId.DarkMagician);
                return true;
            }
            return false;
        }

        private bool PalladiumOracleManaEffect()
        {
            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
                return true;
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(new List<int> { CardId.DarkMagicianGirl, CardId.DarkMagician });
                return true;
            }
            return false;
        }

        private bool ChaosKnightEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                ClientCard target = PickEnemyCard();
                if (target == null)
                    return false;

                List<int> deckTargets = new List<int>
                {
                    CardId.DarkMagician,
                    CardId.IllusionOfChaos,
                    CardId.SoulServant,
                    CardId.MagiciansSalvation
                };

                if (_chaosKnightByKnightsTitle)
                {
                    _chaosKnightByKnightsTitle = false;
                    QueueCardThenIds(target, deckTargets);
                    return true;
                }

                AI.SelectCard(target);
                return true;
            }

            if (Card.Location == CardLocation.Hand || Card.Location == CardLocation.Grave)
            {
                if (Bot.HasInMonstersZone(CardId.ChaosBlackMagicKnight))
                    return false;

                ClientCard tribute = GetChaosKnightTribute();
                if (tribute == null)
                    return false;

                AI.SelectCard(tribute);
                return true;
            }

            return false;
        }

        private ClientCard GetChaosKnightTribute()
        {
            ClientCard spare = Bot.GetMonsters()
                .Where(card => card.HasSetcode(SetcodeDarkMagician) && !IsDarkMagician(card))
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
            if (spare != null)
                return spare;

            if (Bot.HasInSpellZone(CardId.DarkMagicalCircle) || Bot.HasInSpellZone(CardId.EternalSoul))
                return null;

            return Bot.GetMonsters().FirstOrDefault(IsDarkMagician);
        }

        private bool CombinationEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard target = PickEnemyCard();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }

            if (Card.Location != CardLocation.SpellZone)
                return false;
            if (Duel.LastChainPlayer != 1)
                return false;

            ClientCard tribute = Bot.GetMonsters()
                .FirstOrDefault(card => IsDarkMagician(card) || card.IsCode(CardId.DarkMagicianGirl));
            if (tribute == null)
                return false;

            int substitute = IsDarkMagician(tribute) ? CardId.DarkMagicianGirl : CardId.DarkMagician;
            QueueCardThenIds(tribute, new List<int> { substitute, CardId.DarkMagician, CardId.DarkMagicianGirl });
            return true;
        }

        private bool DestinedRivalsEffect()
        {
            if (!HasDarkMagician())
                return false;
            if (Duel.LastChainPlayer != 1 && Duel.Player != 1)
                return false;
            // 只在对面发动「怪兽的效果」时开
            IList<ClientCard> chain = Duel.CurrentChain;
            if (chain != null && chain.Count > 0)
            {
                ClientCard top = chain[chain.Count - 1];
                if (top != null && !top.HasType(CardType.Monster))
                    return false;
            }
            return Enemy.GetMonsters().Any(card => card.IsFaceup() && card.HasType(CardType.Effect));
        }

        private bool MindPowerShieldEffect()
        {
            if (Card.Location != CardLocation.SpellZone)
                return false;
            if (CountEnemyFaceup() < 5)
                return false;
            return Bot.UnderAttack
                || Duel.LastChainPlayer == 1
                || (Duel.Player == 0 && Duel.LastSummonPlayer == 1);
        }

        // =====================================================================
        //  检索 / 铺场
        // =====================================================================

        private bool DarkMagicalCircleEffect()
        {
            // 曾试过「有魂仆在手上就先让魂仆放顶、阵等下再开」：实测这种情况
            // 一局只出现 0~2 次，而且会让阵晚一回合，100 局反而从 41% 掉到 35%，
            // 所以不拦阵。魂仆自己会在阵开出来之后再放顶（见 SoulServantEffect）。

            bool dmJustSummoned = Duel.LastSummonedCards.Any(card =>
                card != null && card.Controller == 0 && IsDarkMagician(card));
            if (dmJustSummoned && Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                // 优先除外对方的额外怪兽 / 大怪；没有的话再挑最麻烦的那张，别白白浪费这一次②
                ClientCard target = PickCircleTarget()
                    ?? Util.GetProblematicEnemyCard(0, true)
                    ?? PickEnemyCard();
                if (target == null)
                    return false;
                Trace("Circle: banish " + target.Id);
                AI.SelectCard(target);
                return true;
            }

            // 阵①的检索优先级：幕帘 > 眼光 > 救出
            List<int> circleTargets = new List<int>();
            if (!Bot.HasInHand(CardId.DarkMagicCurtain) && !Bot.HasInGraveyard(CardId.DarkMagicCurtain))
                circleTargets.Add(CardId.DarkMagicCurtain);
            if (!Bot.HasInHand(CardId.EyeOfTimaeus) && !Bot.HasInSpellZone(CardId.EyeOfTimaeus))
                circleTargets.Add(CardId.EyeOfTimaeus);
            if (!Bot.HasInHand(CardId.MagiciansSalvation) && !Bot.HasInSpellZone(CardId.MagiciansSalvation))
                circleTargets.Add(CardId.MagiciansSalvation);
            circleTargets.AddRange(SearchPriorityForSpellTrap());
            AI.SelectCard(circleTargets);
            return UniqueFaceupSpell();
        }

        private bool SalvationEffect()
        {
            bool justSummoned = Duel.LastSummonedCards.Any(card => card != null && card.Controller == 0
                && (IsDarkMagician(card) || card.IsCode(CardId.DarkMagicianGirl)));
            // ② 自己把黑魔术师 / 黑魔术少女召唤·特殊召唤时，从墓地特召另一只
            if (justSummoned && Card.Location == CardLocation.SpellZone && Card.IsFaceup())
            {
                bool needGirl = CurrentPlan() == Plan.DarkBurning
                    && !Bot.GetMonsters().Any(card => card.IsCode(CardId.DarkMagicianGirl));
                ClientCard target = Bot.GetGraveyardMonsters()
                    .Where(card => IsDarkMagician(card) || card.IsCode(CardId.DarkMagicianGirl))
                    .OrderByDescending(card => needGirl && card.IsCode(CardId.DarkMagicianGirl))
                    .FirstOrDefault();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }

            // ① 场地：不急着发。发动魔陷会触发「师徒」①抽卡，
            //   所以等师徒站场了再发；这回合还能做出师徒就先捏着。
            // 幕帘还在手 → 让幕帘先走，别用场地把「场上有永远之魂」这个前置条件占掉
            if (Bot.HasInHand(CardId.DarkMagicCurtain))
            {
                Trace("Salvation held: curtain first");
                return false;
            }

            // 永魂还没有着落（场上没有、手里也没有）→ 现在就把永魂从卡组盖下来。
            // 救出 ① 本身就是「从卡组盖 1 张永远之魂」，捏着等 ② 的结果是
            // 空场站着看对面铺 6 个回合（对 LV3 的复盘里 7 连「held」然后输掉）。
            bool soulOnField = Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EternalSoul));
            if (!soulOnField && !Bot.HasInHand(CardId.EternalSoul))
            {
                Trace("Salvation: grab eternal soul");
                LineDone(Act.Activate, CardId.MagiciansSalvation);
                return UniqueFaceupSpell();
            }

            if (Bot.HasInMonstersZone(CardId.TheDarkMagicians))
            {
                LineDone(Act.Activate, CardId.MagiciansSalvation);
                return UniqueFaceupSpell();
            }

            if (Bot.HasInHand(CardId.SecretsOfDarkMagic) || Bot.HasInHand(CardId.EyeOfTimaeus))
            {
                Trace("Salvation held: waiting for the dark magicians");
                return false;
            }

            LineDone(Act.Activate, CardId.MagiciansSalvation);
            return UniqueFaceupSpell();
        }

        /// <summary>朔夜时雨：对面特召时丢弃，无效那只怪兽的效果。</summary>
        private bool SayoEffect()
        {
            // 朔夜时雨只该无效「还有效果可无效」的怪兽：
            //   · 通常怪兽（没有效果）— 无效了也是白丢
            //   · 已经被无效过的（IsDisabled）— 再无效一次是浪费
            ClientCard target = Duel.LastSummonedCards
                .Where(card => card != null
                    && card.Controller == 1
                    && card.HasType(CardType.Effect)
                    && !card.IsDisabled())
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            Trace("Sayo: negate " + target.Id + " atk=" + target.Attack);
            AI.SelectCard(target);
            return true;
        }

        /// <summary>是不是「永续 / 场地 / 装备」这类持续卡。</summary>
        private static bool IsOngoingSpellTrap(ClientCard card)
        {
            return card != null
                && (card.HasType(CardType.Continuous) || card.HasType(CardType.Field) || card.HasType(CardType.Equip));
        }

        /// <summary>旋风：对面后场有卡时拆一张。</summary>
        private bool MysticalSpaceTyphoonEffect()
        {
            // 旋风就是「炸一张」：对面魔陷区有卡就用
            IList<ClientCard> chain = Duel.CurrentChain;
            ClientCard top = (chain != null && chain.Count > 0) ? chain[chain.Count - 1] : null;

            // 同一个连锁里已经发过一张旋风 → 第二张不要再炸同一个目标（白丢一张）
            if (chain != null && chain.Any(card => card != null && card.IsCode(5318639)))
                return false;

            // 但不连锁对面正在发动的「通常魔法 / 通常陷阱」
            // （它们结算完就进墓地，炸了也不占便宜，还白丢一张）
            if (top != null && top.Controller == 1 && top.IsFaceup() && !IsOngoingSpellTrap(top))
                return false;

            // 优先持续卡（永续/场地/装备），其次盖着的，最后随便一张
            ClientCard target = Enemy.SpellZone.FirstOrDefault(card => card != null && IsOngoingSpellTrap(card))
                ?? Enemy.SpellZone.FirstOrDefault(card => card != null && card.IsFacedown())
                ?? Enemy.SpellZone.FirstOrDefault(card => card != null);

            if (target == null)
                return false;

            Trace("MST: destroy " + target.Id);
            AI.SelectCard(target);
            return true;
        }

        /// <summary>暗黑界的取引：双方抽1丢1，丢卡优先丢黑魔术少女。</summary>
        private bool DarkWorldDealingsEffect()
        {
            ClientCard discard = Bot.Hand.FirstOrDefault(card => card != null && card.IsCode(CardId.DarkMagicianGirl))
                ?? Bot.Hand.FirstOrDefault(card => card != null && card.IsCode(CardId.SoulServant));
            if (discard == null)
                return false;
            AI.SelectCard(discard.Id);
            return true;
        }

        /// <summary>混沌之幻想魔术师：手卡展示检索。
        /// 目标记在 _illusionSearchId，由 OnSelectCard 直接挑。</summary>
        private bool IllusionOfChaosEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                bool soulsHeld = Bot.HasInHand(CardId.MagiciansSouls)
                    || Bot.HasInMonstersZone(CardId.MagiciansSouls);

                int rodsKnown = Bot.Hand.Count(card => card != null && card.IsCode(CardId.MagiciansRod))
                    + Bot.GetMonsters().Count(card => card.IsCode(CardId.MagiciansRod))
                    + Bot.Graveyard.Count(card => card != null && card.IsCode(CardId.MagiciansRod))
                    + Bot.Banished.Count(card => card != null && card.IsCode(CardId.MagiciansRod));

                int searchId;
                bool rodInHand = Bot.HasInHand(CardId.MagiciansRod);
                bool servantHeld = Bot.HasInHand(CardId.ServantOfTheKing)
                    || Bot.HasInMonstersZone(CardId.ServantOfTheKing);

                if (!rodInHand && rodsKnown < 3)
                    searchId = CardId.MagiciansRod;          // 没杖 → 拿杖
                else if (!servantHeld)
                    searchId = CardId.ServantOfTheKing;      // 有杖 → 拿王仆人
                else if (!soulsHeld)
                    searchId = CardId.MagiciansSouls;
                else if (!HasDarkMagician() && !Bot.HasInHand(CardId.DarkMagician))
                    searchId = CardId.DarkMagician;
                else
                    searchId = CardId.DarkMagician;

                _illusionStage = 1;
                _illusionSearchId = searchId;
                Trace("Illusion: pick=" + searchId);
                return true;
            }
            return false;
        }

        /// <summary>魔术师双魂：把手上的黑魔术少女/魂之仆人送墓，检索黑魔术师。</summary>
        private bool MagiciansSoulsEffect()
        {
            ClientCard dump = Bot.Hand.FirstOrDefault(card => card != null && card.IsCode(CardId.DarkMagicianGirl))
                ?? Bot.Hand.FirstOrDefault(card => card != null && card.IsCode(CardId.SoulServant));

            if (dump == null)
                return false;

            AI.SelectCard(dump.Id);
            AI.SelectNextCard(CardId.DarkMagician);
            return true;
        }

        /// <summary>
        /// 魂之仆人①放顶优先级：融合点 → 展开起点 → 铺场点 → 检索 → 盖永魂 → 黑魔术师。
        /// 阵要用它来判断「值不值得为魂仆让路」。
        /// </summary>
        private static readonly int[] SoulPlaceOrder =
        {
            CardId.EyeOfTimaeus,          // 融合点（伪检索，最优先）
            CardId.DarkMagicCurtain,      // 展开起点
            CardId.ServantOfTheKing,      // 铺场点
            CardId.DarkMagicalCircle,     // 检索/除外
            CardId.MagiciansSalvation,    // 盖永魂
            CardId.DarkMagician           // 其次黑魔术师
        };

        private bool SoulServantEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                // 种类 ≥1 就抽（主轴常常是「毁灭在墓地」这一张，抽上来接眼光）
                int kinds = CountSoulServantKinds();
                Trace("SoulServant GY: kinds=" + kinds);
                return kinds >= 1;
            }

            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;

            // 什么时候才值得发：放顶的卡得有人帮我拿上来。
            //   ① 阵已经在场上  → 阵②除外/①检索都能拿
            //   ② 师徒在场上    → 师徒①抽卡
            //   ③ 阵还在手里    → 先放顶，再开阵，阵①翻三拿一必中（这是「阵+魂仆」两卡启动）
            bool circleInHand = Bot.HasInHand(CardId.DarkMagicalCircle);
            bool soulEngine = Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.DarkMagicalCircle))
                || Bot.HasInMonstersZone(CardId.TheDarkMagicians)
                || circleInHand;
            if (!soulEngine)
            {
                Trace("SoulServant held: no circle/tdm");
                return false;
            }

            // ① 放置卡组顶的正确用法：把「下一张需要的卡」放上去，
            // 靠 师徒①抽卡 或 黑魔导阵②翻三拿一 把它拿到手。
            int[] soulOrder = SoulPlaceOrder;

            // 放顶优先级：
            //   墓地 2 张永魂 → 放永魂
            //   墓地 2 张眼光 → 放眼光
            //   否则           → 放幕帘
            int soulsInGrave = Bot.Graveyard.Count(card => card != null && card.IsCode(CardId.EternalSoul));
            int eyesInGrave = Bot.Graveyard.Count(card => card != null && card.IsCode(CardId.EyeOfTimaeus));

            if (soulsInGrave >= 2
                && !Bot.HasInHand(CardId.EternalSoul)
                && !Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EternalSoul)))
            {
                Trace("SoulServant: souls in grave=" + soulsInGrave + ", place soul");
                AI.SelectCard(CardId.EternalSoul);
                return true;
            }

            // 眼光优先（伪检索）：先攻常规就是把眼光放顶，靠师徒/阵拿上来
            if (!Bot.HasInHand(CardId.EyeOfTimaeus)
                && !Bot.HasInSpellZone(CardId.EyeOfTimaeus)
                && !Bot.HasInMonstersZone(CardId.DarkMagicianTheDragonKnight)
                && !Bot.HasInMonstersZone(CardId.TheDarkMagicians))
            {
                Trace("SoulServant: place eye (eyes in grave=" + eyesInGrave + ")");
                AI.SelectCard(CardId.EyeOfTimaeus);
                return true;
            }

            foreach (int wanted in soulOrder)
            {
                if (Bot.HasInHand(wanted) || Bot.HasInSpellZone(wanted) || Bot.HasInMonstersZone(wanted))
                    continue;

                Trace("SoulServant: place " + wanted);
                AI.SelectCard(wanted);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 双魂①的堆墓目标：优先堆黑魔术少女；
        /// 如果展开链自己就能调度到少女（牵绊 / 幻想 / 救出 / 师徒），就改堆玛娜。
        /// 目的是把「魂之仆人②」的卡名种类（黑魔术师 / 黑魔术少女 / 守护神官）尽量凑满。
        /// </summary>
        private int SoulsSendTarget()
        {
            bool dmgInGrave = Bot.HasInGraveyard(CardId.DarkMagicianGirl);
            bool manaInGrave = Bot.HasInGraveyard(CardId.PalladiumOracleMana);
            bool dmgCanBeFetched = CanFetchDarkMagicianGirl();

            if (!dmgInGrave && !dmgCanBeFetched)
                return CardId.DarkMagicianGirl;
            if (!manaInGrave)
                return CardId.PalladiumOracleMana;
            if (!dmgInGrave)
                return CardId.DarkMagicianGirl;
            return CardId.DarkMagician;
        }

        private bool CanFetchDarkMagicianGirl()
        {
            if (Bot.HasInHand(CardId.IllusionOfChaos))
                return true;
            if (Bot.HasInHand(CardId.BondBetweenTeacherAndStudent) && HasDarkMagician())
                return true;
            if (Bot.HasInSpellZone(CardId.MagiciansSalvation))
                return true;
            if (Bot.HasInMonstersZone(CardId.TheDarkMagicians))
                return true;
            return false;
        }

        /// <summary>
        /// 阿尼玛（Link-1，箭头朝上）能吃怪时选哪个额外怪兽区：
        /// 左边 z5 指向对方 z3，右边 z6 指向对方 z1。
        /// </summary>
        private int AnimaPlaceZone()
        {
            if (Bot.MonsterZone[5] == null
                && Enemy.MonsterZone[3] != null && Enemy.MonsterZone[3].IsFaceup())
                return Zones.z5;
            if (Bot.MonsterZone[6] == null
                && Enemy.MonsterZone[1] != null && Enemy.MonsterZone[1].IsFaceup())
                return Zones.z6;
            return 0;
        }

        private bool CanMakeAnimaEat()
        {
            return AnimaPlaceZone() != 0;
        }

        private bool AnimaSummon()
        {
            int zone = AnimaPlaceZone();
            if (zone == 0)
                return false;

            AI.SelectPlace(zone);
            return true;
        }

        /// <summary>阿尼玛①：把连接区的对方怪兽装备过来。</summary>
        private bool AnimaEffect()
        {
            ClientCard target = Enemy.GetMonsters()
                .Where(card => card.IsFaceup())
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool ServantOfTheKingEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!LineAllows(Act.Activate, CardId.ServantOfTheKing))
                    return false;
                ClientCard spell = GetCheapestSpellInHand();
                if (spell == null)
                    return false;

                // 盖放优先：缺什么盖什么（手里已经有的不重复拿）
                List<int> setTargets = new List<int>();
                if (!Bot.HasInHand(CardId.MagiciansSalvation) && !Bot.HasInSpellZone(CardId.MagiciansSalvation))
                    setTargets.Add(CardId.MagiciansSalvation);
                if (!Bot.HasInHand(CardId.DarkMagicalCircle) && !Bot.HasInSpellZone(CardId.DarkMagicalCircle))
                    setTargets.Add(CardId.DarkMagicalCircle);
                if (!Bot.HasInHand(CardId.EyeOfTimaeus) && !Bot.HasInSpellZone(CardId.EyeOfTimaeus))
                    setTargets.Add(CardId.EyeOfTimaeus);
                if (!Bot.HasInHand(CardId.SoulServant) && !Bot.HasInSpellZone(CardId.SoulServant))
                    setTargets.Add(CardId.SoulServant);
                // 永魂保护：手卡/场上(含盖着)有永魂、或手卡有救出 → 不拿永魂
                if (!EternalSoulCovered())
                    setTargets.Add(CardId.EternalSoul);
                // 五张都已经有了 → 宁可多拿一张阵，也不要去拿永魂重复
                if (setTargets.Count == 0)
                    setTargets.Add(CardId.DarkMagicalCircle);
                QueueCardThenIds(spell, setTargets);
                Trace("STEP servant | reveal=" + spell.Id
                    + " | setTargets=[" + string.Join(",", setTargets) + "]"
                    + " | hand=[" + Ids(Bot.Hand) + "]"
                    + " | spellzone=[" + Ids(Bot.SpellZone) + "]");
                LineDone(Act.Activate, CardId.ServantOfTheKing);
                return true;
            }

            if (Card.Location != CardLocation.MonsterZone)
                return false;
            // ③ 自己·对方回合都能发；对面魔陷区有卡就值
            // （不能只看表侧：羽毛扫效果主要就是吹盖着的）
            int enemySpellTraps = Enemy.SpellZone.Count(card => card != null);

            // 优先级 1：对面正在发动「永续 / 装备 / 场地」魔法 → 直接连锁吹掉
            IList<ClientCard> chainNow = Duel.CurrentChain;
            ClientCard chainTop = (chainNow != null && chainNow.Count > 0) ? chainNow[chainNow.Count - 1] : null;
            bool chainToContinuous = chainTop != null
                && chainTop.HasType(CardType.Continuous)
                || (chainTop != null && chainTop.HasType(CardType.Equip))
                || (chainTop != null && chainTop.HasType(CardType.Field));

            // 优先级 2：对面后场 ≥ 3 张
            bool wideBackrow = enemySpellTraps >= 3;

            if (!chainToContinuous && !wideBackrow && !(enemySpellTraps >= 2 && Duel.Player == 0))
                return false;

            ClientCard discard = GetCheapestSpellInHand();
            if (discard == null)
                return false;

            AI.SelectCard(discard);
            return true;
        }

        private bool TimaeusEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                ClientCard fodder = GetCheapestSpellcaster();
                if (fodder == null)
                    return false;
                AI.SelectCard(fodder);
                return true;
            }

            if (Card.Location == CardLocation.MonsterZone)
            {
                if (!CanAccessDarkMagician())
                    return false;
                if (TitleIsLive())
                    return false;
                AI.SelectCard(FusionPreference);
                return true;
            }

            return false;
        }

        private bool RodEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Duel.Player != 1 || Duel.LastChainPlayer != 0)
                    return false;
                ClientCard tribute = GetCheapestSpellcaster();
                if (tribute == null)
                    return false;
                AI.SelectCard(tribute);
                return true;
            }

            // 线路里要求检索特定卡时，直接拿它（杖的检索是必发效果，不能被线路挡掉）
            int wanted = LineSearchTarget();
            if (wanted != 0 && LineAllows(Act.Search, CardId.MagiciansRod))
            {
                Trace("Line: rod searches " + wanted);
                AI.SelectCard(wanted);
                LineDone(Act.Search, CardId.MagiciansRod);
                return true;
            }

            AI.SelectCard(SearchPriorityForSpellTrap());
            return true;
        }

        // =====================================================================
        //  通常召唤
        // =====================================================================

        private bool DarkMagicianSummon()
        {
            if (Bot.HasInHand(CardId.MagiciansRod))
                return false;
            if (Bot.HasInHand(CardId.DarkMagicCurtain))
                return false;
            if (HasDarkMagician())
                return false;

            Plan plan = CurrentPlan();
            if (plan == Plan.ChaosKnight || plan == Plan.DarkBurning)
                return true;

            return Bot.HasInSpellZone(CardId.DarkMagicalCircle)
                || Bot.HasInSpellZone(CardId.EternalSoul)
                || Bot.GetMonsterCount() == 0
                || Util.IsOneEnemyBetter(true);
        }

        private bool DarkMagicianGirlSummon()
        {
            if (!HasDarkMagician())
                return false;
            return Bot.HasInHand(CardId.DarkBurningMagic);
        }

        // =====================================================================
        //  解放 / 特殊召唤
        // =====================================================================

        private bool KaijuSummon()
        {
            if (Enemy.GetMonsterCount() == 0)
                return false;
            if (Bot.HasInMonstersZone(CardId.Radian))
                return false;

            ClientCard target = Util.GetProblematicEnemyMonster(0, false) ?? Util.GetBestEnemyMonster();
            if (target == null)
                return false;
            if (target.Attack < 2800 && target.GetDefensePower() < 2800 && !target.HasType(CardType.Effect))
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool CoffinEffect()
        {
            if (Card.Location != CardLocation.SpellZone && Card.Location != CardLocation.Hand)
                return false;

            ClientCard tribute = GetCheapestSpellcaster();
            if (tribute == null)
                return false;
            if (HasDarkMagician() && !Bot.HasInGraveyard(CardId.DarkMagician))
                return false;
            if (Bot.GetMonsterCount() == 1 && IsDarkMagician(tribute))
                return false;

            QueueCardThenIds(tribute, new List<int> { CardId.DarkMagician });
            return true;
        }

        private bool KnightsTitleEffect()
        {
            ClientCard darkMagician = Bot.GetMonsters().FirstOrDefault(IsDarkMagician);
            if (darkMagician == null)
                return false;
            if (Bot.HasInMonstersZone(CardId.ChaosBlackMagicKnight))
                return false;
            if (Bot.HasInBanished(CardId.ChaosBlackMagicKnight))
                return false;

            _chaosKnightByKnightsTitle = true;
            AI.SelectCard(darkMagician);
            return true;
        }

        /// <summary>
        /// 永远之魂：
        /// - 盖着的 → 直接翻开（②给黑魔术师抗性，①捏在手里留着）；
        /// - 表侧的 → ①是二速起动，只在「阵在场 + 对方有值得除外的怪」时拉黑魔术师，
        ///   靠阵②把对方的额外怪兽/大怪除外（阵魂连携）。
        /// </summary>
        private bool EternalSoulEffect()
        {
            if (Card.Location != CardLocation.SpellZone)
                return false;

            // 还在盖着：先翻开，不急着用①
            if (Card.IsFacedown())
            {
                Trace("EternalSoul: flip up");
                return true;
            }

            ClientCard revive = PickEternalSoulTarget();
            bool canSummon = revive != null && Bot.GetMonsterCount() < 5;
            bool circleUp = Bot.HasInSpellZone(CardId.DarkMagicalCircle);

            // 阵魂连携：阵在场、对方有额外怪或 2500 以上的怪，才拉黑魔术师去触发阵②
            bool wantsDragonKnight = revive != null && revive.IsCode(CardId.DarkMagicianTheDragonKnight);
            if (circleUp && canSummon && (!HasDarkMagician() || wantsDragonKnight))
            {
                ClientCard threat = PickCircleTarget();
                if (threat != null)
                {
                    Trace("EternalSoul: ss " + revive.Id + " to banish " + threat.Id);
                    AI.SelectOption(0);
                    AI.SelectCard(revive);
                    return true;
                }
                // 没有值得除外的目标就先捏着，别浪费阵②这一次/回合
                return false;
            }

            // 没有阵：只在空场或斩杀时拿它当身位
            if (!circleUp && canSummon && (!HasDarkMagician() || wantsDragonKnight)
                && (Bot.GetMonsterCount() == 0 || CurrentPlan() == Plan.Lethal || wantsDragonKnight))
            {
                AI.SelectOption(0);
                AI.SelectCard(revive);
                return true;
            }

            // 拉不出黑魔术师、又缺攻击魔法 → 检索
            if (!canSummon
                && !Bot.HasInHand(CardId.DarkMagicAttack)
                && !Bot.HasInHand(CardId.ThousandKnives))
            {
                AI.SelectOption(1);
                if (Enemy.GetSpells().Any(card => card.IsFaceup()))
                    AI.SelectCard(CardId.DarkMagicAttack);
                else
                    AI.SelectCard(new List<int> { CardId.ThousandKnives, CardId.DarkMagicAttack });
                return true;
            }

            return false;
        }

        /// <summary>
        /// 永远之魂①能拉的对象：墓地/手卡的「黑魔术师」（含卡名当作黑魔术师的怪兽）。
        /// 优先龙骑士（3000，卡名也视为黑魔术师），其次毁灭、王之仆人、普通黑魔术师。
        /// </summary>
        private ClientCard PickEternalSoulTarget()
        {
            ClientCard card = Bot.GetGraveyardMonsters()
                .FirstOrDefault(c => c.IsCode(CardId.DarkMagicianTheDragonKnight));
            if (card != null)
                return card;

            card = Bot.GetGraveyardMonsters().FirstOrDefault(c => c.IsCode(CardId.DarkMagicianOfDestruction));
            if (card != null)
                return card;

            card = Bot.GetGraveyardMonsters().FirstOrDefault(c => c.IsCode(CardId.ServantOfTheKing));
            if (card != null)
                return card;

            card = Bot.GetGraveyardMonsters().FirstOrDefault(c => c.IsCode(CardId.DarkMagician));
            if (card != null)
                return card;

            return Bot.Hand.FirstOrDefault(c => c != null && c.IsCode(CardId.DarkMagician));
        }

        /// <summary>阵②想除外的目标：优先对方额外卡组怪兽，其次 2500 以上的大怪。</summary>
        private ClientCard PickCircleTarget()        {
            return Enemy.GetMonsters()
                .Where(card => card.IsFaceup() && (card.IsExtraCard() || card.Attack >= 2500))
                .OrderByDescending(card => card.IsExtraCard() ? 1 : 0)
                .ThenByDescending(card => card.Attack)
                .FirstOrDefault();
        }

        private bool NavigationEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!HasDarkMagician())
                    return false;
                ClientCard target = Enemy.GetSpells().FirstOrDefault(card => card.IsFaceup());
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }

            if (Card.Location != CardLocation.SpellZone)
                return false;
            if (!Bot.HasInHand(CardId.DarkMagician))
                return false;

            ClientCard darkMagician = Bot.Hand.FirstOrDefault(card => card != null && card.IsCode(CardId.DarkMagician));
            if (darkMagician == null)
                return false;

            QueueCardThenIds(darkMagician, new List<int>
            {
                CardId.MagiciansRod,
                CardId.PalladiumOracleMahad,
                CardId.MagiciansSouls,
                CardId.PalladiumOracleMana,
                CardId.DarkMagicianGirl
            });
            return true;
        }

        private bool BondEffect()
        {
            if (!HasDarkMagician())
                return false;

            QueueIds(CardId.DarkMagicianGirl, CardId.DarkBurningMagic);
            return true;
        }

        private bool CurtainEffect()
        {
            // 幕帘不受线路闸门限制：只要场上没黑魔术师、LP 够，就该开
            // （之前被 LineAllows 卡住，线路一旦停在别的步骤就永远不发）
            // 线路明确要求开帘幕时，即使场上已经有黑魔术师也照开（再多一只给毁灭/Link 用）
            Step step = CurrentStep();
            bool lineWantsCurtain = step != null && step.Act == Act.Activate && step.Card == CardId.DarkMagicCurtain;
            // 场上有 DM 但只有 1 只怪 → 仍然要开（第二只给 Link/变毁灭）
            if (HasDarkMagician() && Bot.GetMonsterCount() >= 2 && !lineWantsCurtain)
                return false;

            // 关键：幕帘只能拉「手卡·卡组」的怪，拉不到就会退而求其次抓杖之类的凑数。
            // 正常情况下要黑魔术师（黑魔术少女线要按线路做，不能全局放宽）。
            if (!Bot.HasInHand(CardId.DarkMagician) && !DarkMagicianStillInDeck())
            {
                Trace("Curtain skipped: no Dark Magician in hand/deck");
                return false;
            }

            // 用状态位直接拦截「特召哪只」的那次选卡，避免队列被别的询问吃掉
            PlaceMainZone();
            _curtainStage = 1;
            LineDone(Act.Activate, CardId.DarkMagicCurtain);
            Trace("Curtain: armed");
            return true;
        }

        /// <summary>黑魔术师是否还可能在卡组里（卡组共 3 张：2 张本体 + 1 张异画）。</summary>
        private bool DarkMagicianStillInDeck()
        {
            int known = 0;
            known += Bot.Hand.Count(card => card != null && card.IsCode(CardId.DarkMagician));
            known += Bot.Graveyard.Count(card => card != null && card.IsCode(CardId.DarkMagician));
            known += Bot.Banished.Count(card => card != null && card.IsCode(CardId.DarkMagician));
            known += Bot.GetMonsters().Count(IsDarkMagician);

            return known < 3;
        }

        /// <summary>毁灭之黑魔术师：本回合发动过魔法卡时，除外6星以上暗属性魔法师族特召。</summary>
        private bool DestructionSummon()
        {
            if (!LineAllows(Act.Transform, CardId.DarkMagicianOfDestruction))
                return false;
            if (!_spellActivatedThisTurn)
                return false;
            if (Bot.HasInMonstersZone(CardId.DarkMagicianOfDestruction))
                return false;
            if (TitleIsLive())
                return false;

            ClientCard fodder = Bot.GetMonsters()
                .Where(card => card.Level >= 6
                    && IsMagicCaster(card)
                    && card.HasAttribute(CardAttribute.Dark))
                .OrderBy(card => IsDarkMagician(card) ? 0 : 1)
                .FirstOrDefault();
            if (fodder == null)
                return false;

            AI.SelectCard(fodder);
            PlaceMainZone();
            Trace("STEP destroy");
            LineDone(Act.Transform, CardId.DarkMagicianOfDestruction);
            return true;
        }

        // =====================================================================
        //  融合 / 攻击魔法
        // =====================================================================

        private bool SecretsOfDarkMagicEffect()
        {
            if (!CanAccessDarkMagician())
                return false;
            if (TitleIsLive())
                return false;

            AI.SelectOption(0);
            PlaceMainZone();
            _pickingFusion = true;
            AI.SelectCard(CurrentPlan() == Plan.DarkMagicians ? FusionPreferenceDarkMagicians : FusionPreference);
            return true;
        }

        /// <summary>
        /// 眼光的目标：优先点墓地的黑魔术师/少女。
        /// 点墓地不会占场上的身位，融合怪兽落地后场上直接 +1，更容易凑够 Link-3。
        /// </summary>
        private ClientCard FindEyeTarget()
        {
            // 战斗阶段：优先点「已经攻击过」的黑魔术师系，
            // 这样不会浪费它这一拳的伤害
            if (Duel.Phase == DuelPhase.Battle)
            {
                ClientCard attacked = Bot.GetMonsters().FirstOrDefault(
                    card => card != null && card.Attacked && IsDarkMagician(card)
                        && !card.IsCode(CardId.DarkMagicianTheDragonKnight));
                if (attacked != null)
                    return attacked;
            }
            // 优先点墓地的卡，但**除外龙骑士**（他要留着给永魂拉）
            return Bot.GetGraveyardMonsters()
                .FirstOrDefault(card => IsDarkMagician(card) && !card.IsCode(CardId.DarkMagicianTheDragonKnight))
                ?? Bot.GetGraveyardMonsters().FirstOrDefault(card => card.IsCode(CardId.DarkMagicianGirl))
                ?? Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.DarkMagicianOfDestruction))
                ?? Bot.GetMonsters().FirstOrDefault(IsDarkMagician)
                ?? Bot.GetMonsters().FirstOrDefault(card => card.IsCode(CardId.DarkMagicianGirl));
        }

        /// <summary>
        /// 蒂迈欧之眼光拉出来的怪兽在下个回合结束时会被除外，所以：
        /// - 自己回合：只有能立刻 link 成塞勒涅时才发动；
        /// - 否则盖下去，留到对方回合再发动，这样它能活到自己回合。
        /// </summary>
        private bool EyeOfTimaeusEffect()
        {
            if (TitleIsLive())
                return false;

            // 自己回合：必须等主轴走到「眼光」这一步（王仆人/毁灭已经进墓地），否则先捏着
            // （战斗阶段例外：点已经攻击过的怪，出混沌再打两拳）

            // 战斗阶段的混沌斩杀算法：
            //   ① 自己场上的怪都已经攻击过
            //   ② 对面 LP ≤ 3000（混沌自身）+ 墓地最高攻击力（① 拉回来那只）
            if (Duel.Phase == DuelPhase.Battle)
            {
                bool allAttacked = Bot.GetMonsters().All(card => card != null && card.Attacked);
                int maxGraveAttack = Bot.GetGraveyardMonsters()
                    .Where(card => card != null)
                    .Select(card => card.Attack)
                    .DefaultIfEmpty(0)
                    .Max();
                bool chaosLethal = Enemy.LifePoints <= 3000 + maxGraveAttack;

                if (!(allAttacked && chaosLethal))
                    return false;
            }

            ClientCard target = FindEyeTarget();
            if (target == null)
                return false;

            // 眼光优先点墓地：场上的目标先留着做 Link 素材，
            // 让它进墓再点，这样白赚一个场值。
            if (target.Location == CardLocation.MonsterZone && !CanLethal())
            {
                bool spareMaterial = Bot.GetMonsters().Any(card => card != null && !card.Equals(target));
                if (spareMaterial)
                {
                    Trace("Eye hold: link " + target.Id + " first");
                    return false;
                }
            }

            // 自己回合：能 link3 就发；能做出 3000 的龙骑士/对方场面宽时也直接发，
            // 不再为了「活到下回合」白白盖着（盖着常常被对面拖成废牌）。
            bool canLink = CanLinkIntoSeleneAfterEye(target);
            Trace("Eye: target=" + (target.Name ?? target.Id.ToString())
                + " onField=" + (target.Location == CardLocation.MonsterZone)
                + " canLink=" + canLink
                + " counters=" + SpellCounterCount());

            // 自己回合做龙骑士很亏：结束阶段就被除外，等于白丢一张眼光
            // 只有能立刻 link 掉（当素材）或能斩杀才发
            if (Duel.Player == 0 && !canLink && CurrentPlan() != Plan.Lethal)
                return false;

            _eyeActivatedThisTurn = true;
            _eyeFusionId = CardId.DarkMagicianTheDragonKnight;
            PlaceMainZone();
            _eyeTargetCard = target;
            _pickingFusion = true;
            LineDone(Act.Activate, CardId.EyeOfTimaeus);
            QueueCardThenIds(target, EyeFusionPreference());
            return true;
        }

        /// <summary>
        /// 眼光的融合选择：对方场上有 3 只以上、墓地又有光属性时，
        /// 出「混沌制驭者」→ ①拉回光 → ②解放光+暗除外对方全部怪兽。
        /// </summary>
        /// <summary>
        /// 混沌斩杀判定：对面 LP &lt; 我方场上攻击力之和 + 3000（混沌）+ 墓地最高攻击力（① 拉回来那只）。
        /// 满足就直接出混沌，不再做师徒 / 龙骑士。
        /// </summary>
        /// <summary>
        /// 混沌清场斩杀：对面有怪挡着打不到脸，但我方攻击力总和已经够斩杀
        /// → 出混沌制驭者，②（解放光+暗各1只）把对面怪兽全部除外，剩下的全体直击。
        /// 实战 R#5358：对面 10000 LP、场上有怪，我们做了龙骑士被挡住，只打到剩 500。
        /// </summary>
        private bool ChaosWipeLethal()
        {
            if (Enemy.GetMonsterCount() == 0)
                return false;   // 没怪挡路时龙骑士/师徒一样能斩杀，不必烧两只怪出混沌
            int myAttack = Bot.GetMonsters()
                .Where(card => card != null)
                .Sum(card => Math.Max(0, card.Attack));
            // 素材会被换掉，但混沌自己 3000 顶上；②的代价（光+暗各1只）已经算在保守估计里
            return myAttack + 3000 >= Enemy.LifePoints;
        }

        private bool ChaosLethalPossible()
        {
            int myAttack = Bot.GetMonsters()
                .Where(card => card != null)
                .Sum(card => Math.Max(0, card.Attack));
            int maxGraveAttack = Bot.GetGraveyardMonsters()
                .Where(card => card != null)
                .Select(card => card.Attack)
                .DefaultIfEmpty(0)
                .Max();
            return Enemy.LifePoints < myAttack + 3000 + maxGraveAttack;
        }

        private int[] EyeFusionPreference()
        {
            // 1) 战斗阶段：混沌制驭者（① 拉墓地 → 再打两拳）
            if (Duel.Phase == DuelPhase.Battle)
                return FusionPreferenceMasterOfChaos;

            // 1.5) 对面有怪挡路、但我方攻击力够斩杀 → 混沌②清场，全体直击
            if (ChaosWipeLethal())
            {
                Trace("Eye: chaos wipe lethal (enemyLP=" + Enemy.LifePoints + ")");
                return FusionPreferenceMasterOfChaos;
            }

            // 2) 对面龙族堆积 → 帕拉丁
            int dragons = Enemy.GetMonsters().Count(card => card != null && card.HasRace(CardRace.Dragon))
                + Enemy.GetGraveyardMonsters().Count(card => card.HasRace(CardRace.Dragon))
                + Bot.GetMonsters().Count(card => card != null && card.HasRace(CardRace.Dragon))
                + Bot.GetGraveyardMonsters().Count(card => card.HasRace(CardRace.Dragon));
            if (dragons >= 3)
                return FusionPreferenceDarkPaladin;

            // 2.5) 混沌斩杀：对面血量低于「我方场上攻击力 + 3000 + 墓地最高攻」→ 直接出混沌
            //      （帕拉丁已在上面处理，不受影响）
            if (ChaosLethalPossible())
                return FusionPreferenceMasterOfChaos;

            // 3) 双阵齐活（阵 + 永魂/救出）→ 龙骑士
            //    下限最大场：龙骑士 + Link-2 → Link-3 → ③ 拉回王仆人，对面回合永魂再拉龙骑士
            bool engineOnline = Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.DarkMagicalCircle))
                && Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EternalSoul));
            if (engineOnline)
                return FusionPreference;

            // 4) 自己回合 + 场上有 Link-2 → 师徒（接 Link-3）
            if (Duel.Player == 0
                && Bot.GetMonsters().Any(card => card != null && card.HasType(CardType.Link) && card.LinkCount == 2))
                return FusionPreferenceDarkMagicians;

            // 5) 幕帘开线：墓地有王仆人 → 师徒
            bool servantInGrave = Bot.GetGraveyardMonsters()
                .Any(card => card != null && card.IsCode(CardId.ServantOfTheKing));
            if (servantInGrave && Duel.Player == 0)
                return FusionPreferenceDarkMagicians;

            // 6) 默认：龙骑士（对面回合也是这个）
            return FusionPreference;
        }

        /// <summary>自己回合凑不出 Link-3 的场值时，把眼光盖下去等对方回合。</summary>
        private bool EyeTimaeusSet()
        {
            // 自己回合、场上没眼光、且能点到目标 → 默认就该盖
            // （对面回合从手里发也行，但盖下来不占手牌、还能诱导）
            if (Duel.Player != 0)
                return false;
            if (Bot.SpellZone.Any(card => card != null && card.IsCode(CardId.EyeOfTimaeus)))
                return false;
            if (FindEyeTarget() == null)
                return false;

            // 唯一例外：这回合能靠混沌斩杀 → 留在手里战阶用
            int myAttack = Bot.GetMonsters().Where(card => card != null).Sum(card => Math.Max(0, card.Attack));
            int maxGraveAttack = Bot.GetGraveyardMonsters()
                .Where(card => card != null)
                .Select(card => card.Attack)
                .DefaultIfEmpty(0)
                .Max();
            if (myAttack + 3000 + maxGraveAttack >= Enemy.LifePoints)
                return false;

            return true;
        }

        /// <summary>眼光结算后场上还能凑出塞勒涅（Link-3，且至少一只魔法师族）。</summary>
        private bool CanLinkIntoSeleneAfterEye(ClientCard target)
        {
            int remaining;
            if (target.Location == CardLocation.MonsterZone)
                remaining = Bot.GetMonsters().Count(card => !card.Equals(target));
            else
                remaining = Bot.GetMonsterCount();

            bool hasSpellcaster = Bot.GetMonsters()
                .Any(card => !card.Equals(target) && IsMagicCaster(card));

            return remaining + 1 >= 3 && hasSpellcaster;
        }

        private bool MonsterRebornEffect()
        {
            ClientCard target = Bot.GetGraveyardMonsters()
                .Where(card => (IsDarkMagician(card) || card.IsCode(CardId.DarkMagicianGirl))
                    || card.Attack >= 2500)
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool DarkBurningMagicEffect()
        {
            if (!HasDarkMagician())
                return false;
            if (!Bot.GetMonsters().Any(card => card.IsCode(CardId.DarkMagicianGirl)))
                return false;

            if (CurrentPlan() == Plan.Lethal)
                return Enemy.GetFieldCount() >= 1;
            return Enemy.GetFieldCount() >= 2;
        }

        private bool DarkMagicAttackEffect()
        {
            if (!HasDarkMagician())
                return false;

            if (CurrentPlan() == Plan.Lethal)
                return Enemy.GetSpells().Any();

            int faceup = Enemy.GetSpells().Count(card => card.IsFaceup());
            return faceup >= 2
                || Enemy.GetSpells().Any(card => card.IsFaceup()
                    && card.HasType(CardType.Continuous | CardType.Field));
        }

        private bool ThousandKnivesEffect()
        {
            if (!HasDarkMagician())
                return false;

            ClientCard target = Util.GetProblematicEnemyMonster(0, true);
            if (target == null && CurrentPlan() == Plan.Lethal)
                target = Util.GetBestEnemyMonster(true, true);
            if (target == null)
                return false;

            if (target.Attack < 2500 && target.GetDefensePower() < 2500
                && !target.HasType(CardType.Effect)
                && CurrentPlan() != Plan.Lethal)
                return false;

            AI.SelectCard(target);
            return true;
        }

        // =====================================================================
        //  额外卡组展开
        // =====================================================================

        private bool Rank7Summon()
        {
            return CountBotLevel7() >= 2 && !TitleIsLive();
        }

        private bool EbonHighSummon()
        {
            return CountBotLevel7() >= 2
                && !TitleIsLive()
                && Bot.HasInMonstersZone(CardId.EbonIllusionMagician);
        }

        private bool BigEyeSummon()
        {
            if (CountBotLevel7() < 2 || TitleIsLive())
                return false;
            return Enemy.GetMonsters().Any(card => card.IsFaceup() && card.Attack >= 3000);
        }

        private bool Rank6Summon()
        {
            return CountBotLevel6Spellcaster() >= 2;
        }

        /// <summary>
        /// 按「连接值最小化」挑 Link 素材：Link-2 + 1 只 = 3，不要拿 3 只去凑。
        /// 用 AI.SelectMaterials 显式指定（和码丽丝同一套写法），不再靠 OnSelectLinkMaterial 的返回值。
        /// </summary>
        private bool AssignSeleneMaterials(int required = 3)
        {
            List<ClientCard> pool = Bot.GetMonsters()
                .Where(card => card != null)
                .OrderByDescending(card => card.HasType(CardType.Link) ? card.LinkCount : 0)
                .ToList();
            if (pool.Count == 0)
                return false;

            List<ClientCard> mats = new List<ClientCard>();
            int value = 0;
            foreach (ClientCard card in pool)
            {
                mats.Add(card);
                value += card.HasType(CardType.Link) ? Math.Max(1, card.LinkCount) : 1;
                if (value >= required)
                    break;
            }
            if (value < required || mats.Count < 2)
                return false;

            try
            {
                AI.SelectMaterials(mats);
            }
            catch
            {
                AI.SelectCard(mats[0]);
            }
            Trace("Selene: materials=" + mats.Count + " value=" + value);
            return true;
        }

        private bool SeleneSummon()
        {

            List<ClientCard> monsters = Bot.GetMonsters();
            // 连接值：Link 怪兽按它的 Link 数算（Link-2 + 1 只 = 3，够出 Link-3）
            int linkValue = monsters.Sum(card => card.HasType(CardType.Link) ? card.LinkCount : 1);
            int counters = SpellCounterCount();

            // 眼光召唤出来的怪兽结束阶段会被除外：它在场就优先 Link 掉
            bool mustSaveEyeFusion = _eyeActivatedThisTurn
                && monsters.Any(card => card.IsCode(_eyeFusionId));

            Trace("Selene check: monsters=" + monsters.Count + " linkValue=" + linkValue
                + " casters=" + monsters.Count(IsMagicCaster) + " counters=" + counters
                + " eye=" + (mustSaveEyeFusion ? 1 : 0));

            if (!mustSaveEyeFusion && linkValue < 3 && !LineAllows(Act.Link, CardId.Selene))
                return false;
            if (linkValue < 3)
                return false;
            if (!monsters.Any(IsMagicCaster))
                return false;

            // 对面有需要访问码语者处理的大怪：直接出塞勒涅当跳板（指示物不够也出）
            if (Enemy.GetMonsters().Any(card => card != null && card.Id != 0 && card.Attack >= 3001))
            {
                Trace("Selene: link3 for accesscode");
                return AssignSeleneMaterials();
            }

            if (counters < 3)
            {
                Trace("Selene skipped: only " + counters + " counters");
                return false;
            }

            // ③ 得能拉回点东西，否则纯亏
            if (Bot.GetGraveyardMonsters().Any(IsMagicCaster) || mustSaveEyeFusion)
            {
                Trace("STEP selene");
                return AssignSeleneMaterials();
            }
            return false;
        }

        /// <summary>双方场上 + 墓地里的魔法卡数量（塞勒涅①的指示物来源）。</summary>
        private int SpellCounterCount()
        {
            int count = 0;
            count += Bot.GetSpells().Count(card => card.IsSpell());
            count += Enemy.GetSpells().Count(card => card.IsSpell());
            count += Bot.Graveyard.Count(card => card != null && card.IsSpell());
            count += Enemy.Graveyard.Count(card => card != null && card.IsSpell());
            return count;
        }

        /// <summary>访问码语者的指定素材（塞勒涅 + 1 只），在 OnSelectLinkMaterial 里直接用掉。</summary>
        private List<ClientCard> _accesscodeMats;

        private bool AccesscodeSummon()
        {
            // 现有场面直接攻击就能打死 → 不做访问，直接打
            int totalAttack = Bot.GetMonsters()
                .Where(card => card != null && card.Attack > 0)
                .Sum(card => card.Attack);
            if (Enemy.GetMonsterCount() == 0 && totalAttack >= Enemy.LifePoints)
                return false;

            // 访问只有在「能在打点上超过对面」时才值得做
            if (!CanLethal() && Enemy.LifePoints > 3500)
                return false;

            // 必须有一只作为素材的连接怪在墓地（才能用①加攻）
            bool hasLinkMaterial = Bot.GetGraveyardMonsters().Any(card => card.HasType(CardType.Link));
            if (!hasLinkMaterial)
                return false;

            // 素材只能是「塞勒涅(Link-3) + 1 只怪兽」：
            // 4 只怪凑 Link-4 会把整个场面烧光（实战 R#5358 就是这么输掉的资源）。
            // 没有塞勒涅就先别出访问 —— 先做塞勒涅再转。
            ClientCard selene = Bot.GetMonsters()
                .FirstOrDefault(card => card != null && card.IsCode(CardId.Selene));
            if (selene == null)
            {
                Trace("Accesscode skipped: no Selene");
                return false;
            }
            ClientCard partner = Bot.GetMonsters()
                .Where(card => card != null && !card.Equals(selene) && !IsKeyMonster(card))
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
            if (partner == null)
            {
                Trace("Accesscode skipped: no partner for Selene");
                return false;
            }

            _accesscodeMats = new List<ClientCard> { selene, partner };
            Trace("Accesscode: materials=Selene+" + partner.Id);
            return true;
        }

        // =====================================================================
        //  额外卡组效果
        // =====================================================================

        /// <summary>师徒：守备站场，靠①尽量抽卡；被破坏时②会拉回黑魔术师+少女。</summary>
        private bool TheDarkMagiciansEffect()
        {
            // 引擎只在「魔法·陷阱卡的效果发动」时才询问，能抽就抽
            AI.SelectYesNo(true);
            return true;
        }

        private bool DarkPaladinEffect()
        {
            if (Duel.LastChainPlayer != 1)
                return false;
            ClientCard chainCard = Util.GetLastChainCard();
            if (chainCard == null || !chainCard.IsSpell())
                return false;

            ClientCard discard = Bot.Hand
                .Where(card => card != null && !card.IsCode(CardId.DarkMagician))
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
            if (discard == null)
                return false;

            AI.SelectCard(discard);
            return true;
        }

        private bool AmuletDragonEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                List<ClientCard> spells = Bot.Graveyard.Where(card => card != null && card.IsSpell()).ToList();
                spells.AddRange(Enemy.Graveyard.Where(card => card != null && card.IsSpell()));
                if (spells.Count == 0)
                    return false;
                AI.SelectCard(spells.Take(6).ToList());
                return true;
            }

            ClientCard target = Bot.GetGraveyardMonsters()
                .Where(IsMagicCaster)
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool DarkMagicianOfDestructionEffect()
        {
            int target = DestructionSearchTarget();
            Trace("Destroy: search " + target + " (curtain hand=" + Bot.HasInHand(CardId.DarkMagicCurtain)
                + " grave=" + Bot.HasInGraveyard(CardId.DarkMagicCurtain) + ")");
            AI.SelectCard(target);
            return true;
        }

        /// <summary>
        /// 超魔导战士-混沌制驭者：
        /// ① 融合召唤时拉墓地 1 只光/暗属性（T2 主轴：拉回魔术师之杖继续做场）；
        /// ③ 被破坏时回收墓地 1 张魔法卡。
        /// ②（解放光+暗除外对方全部怪兽）暂不使用。
        /// </summary>
        private bool MasterOfChaosEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard spell = Bot.Graveyard
                    .Where(card => card != null && card.IsSpell())
                    .OrderByDescending(card => card.IsCode(CardId.SecretsOfDarkMagic, CardId.EyeOfTimaeus) ? 1 : 0)
                    .FirstOrDefault();
                if (spell == null)
                    return false;
                AI.SelectCard(spell);
                return true;
            }

            bool justSummoned = Duel.LastSummonedCards
                .Any(card => card != null && card.IsCode(CardId.MasterOfChaos));
            if (justSummoned)
            {
                // ① 融合召唤时：拉墓地 1 只光/暗属性
                // 斩杀优先：拉**攻击力最高**的（多一个打点就多一分斩杀可能）
                // 没有斩杀时：优先光属性（方便接② 解放光+暗 除外对面全场）
                bool lethalNow = CurrentPlan() == Plan.Lethal;
                ClientCard target = Bot.GetGraveyardMonsters()
                    .Where(card => card.HasAttribute(CardAttribute.Light) || card.HasAttribute(CardAttribute.Dark))
                    .OrderByDescending(card => lethalNow ? card.Attack : (card.HasAttribute(CardAttribute.Light) ? 1 : 0))
                    .ThenByDescending(card => card.Attack)
                    .FirstOrDefault();
                if (target == null)
                    return false;

                Trace("MasterOfChaos: revive " + target.Id);
                AI.SelectCard(target);
                return true;
            }

            // ② 把自己场上的光·暗属性怪兽各 1 只解放，对方场上怪兽全部除外
            ClientCard light = Bot.GetMonsters()
                .FirstOrDefault(card => card.HasAttribute(CardAttribute.Light));
            ClientCard dark = Bot.GetMonsters()
                .FirstOrDefault(card => card.HasAttribute(CardAttribute.Dark) && !card.Equals(light));
            if (light == null || dark == null)
                return false;
            // 只剩 1 只阻挡怪时也值得清：只要这一波能斩杀
            if (Enemy.GetMonsterCount() < 2 && CurrentPlan() != Plan.Lethal && !ChaosWipeLethal())
                return false;
            if (Enemy.GetMonsterCount() == 0)
                return false;

            Trace("MasterOfChaos: banish all, enemy monsters=" + Enemy.GetMonsterCount());
            AI.SelectCard(new List<ClientCard> { light, dark });
            return true;
        }

        private bool TimaeusFusionEffect()
        {
            if (Duel.Player == 0)
                return true;

            ClientCard target = Enemy.GetSpells().FirstOrDefault(card => card.IsFaceup())
                ?? Enemy.GetSpells().FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool EbonIllusionEffect()
        {
            if (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.Battle)
            {
                ClientCard target = PickEnemyCard();
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }

            if (!Card.HasXyzMaterial())
                return false;
            if (Bot.HasInMonstersZone(CardId.DarkMagician))
                return false;

            AI.SelectCard(CardId.DarkMagician);
            return true;
        }

        private bool EbonHighEffect()
        {
            if (Card.Location != CardLocation.Grave)
                return false;

            ClientCard target = PickEnemyCard();
            if (target == null)
            {
                AI.SelectCard(new List<int> { CardId.DarkMagician, CardId.MagiciansRod });
                return true;
            }

            AI.SelectCard(target);
            AI.SelectNextCard(new List<int> { CardId.DarkMagician, CardId.MagiciansRod });
            return true;
        }

        private bool BigEyeEffect()
        {
            ClientCard target = Enemy.GetMonsters()
                .Where(card => card.IsFaceup())
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool NoritoEffect()
        {
            if (Duel.LastChainPlayer != 1)
                return false;
            ClientCard chainCard = Util.GetLastChainCard();
            return chainCard != null && (chainCard.IsSpell() || chainCard.IsTrap());
        }

        private bool MagiMagiEffect()
        {
            ClientCard target = Util.GetBestEnemyMonster(true);
            int option = 0;
            if (target == null)
            {
                target = Enemy.GetGraveyardMonsters().OrderByDescending(card => card.Attack).FirstOrDefault();
                option = 1;
            }
            if (target == null)
                return false;

            AI.SelectOption(option);

            ClientCard cost = Bot.Hand.Where(card => card != null)
                .OrderBy(card => card.IsSpell() ? 1 : 0)
                .FirstOrDefault();
            if (cost != null)
            {
                AI.SelectCard(target);
                AI.SelectNextCard(cost);
            }
            else
            {
                AI.SelectCard(target);
            }
            return true;
        }

        private bool SeleneEffect()
        {

            // 幕帘开：拉回师徒；杖开：拉回王仆人
            int[] priority = { CardId.TheDarkMagicians, CardId.ServantOfTheKing, CardId.DarkMagician };
            foreach (int id in priority)
            {
                ClientCard card = Bot.GetGraveyardMonsters().FirstOrDefault(c => c.IsCode(id));
                if (card != null)
                {
                    Trace("Selene revive " + id);
                    AI.SelectCard(card);
                    return true;
                }
            }

            ClientCard target = Bot.GetGraveyardMonsters()
                .Where(IsMagicCaster)
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool AccesscodeEffect()
        {
            // ① 只能用「作为连接素材的墓地连接怪」加攻，优先 Link 数最高的（塞勒涅 Link-3 = +3000）
            // 只能从墓地选，且**排除访问本体**（否则第一个目标会把自己除外）
            // 优先 Link-3（塞勒涅）→ Link-2（克劳利）
            ClientCard link = Bot.GetGraveyardMonsters()
                .Where(card => card != null && card.HasType(CardType.Link)
                    && !card.IsCode(CardId.AccesscodeTalker))
                .OrderByDescending(card => card.LinkCount)
                .FirstOrDefault();
            if (link == null)
                return false;

            // ① 先加攻（墓地连接怪的 Link 数 × 1000）
            // 对面空场也必须加：加上去才能斩杀
            ClientCard target = PickEnemyCard();
            if (target == null)
            {
                AI.SelectCard(link);
                return true;
            }

            _eyeTargetCard = link;      // 重用拦截：必须除外我们选的这只
            QueueCardThenCard(link, target);
            return true;
        }

        // =====================================================================
        //  2026-10 新卡
        // =====================================================================

        /// <summary>暗之护封剑：对方表侧怪兽全部盖下，并锁住表示形式变更。</summary>
        private bool SwordsOfConcealingLightEffect()
        {
            if (Duel.Phase != DuelPhase.Main1 && Duel.Phase != DuelPhase.Main2)
                return false;
            return Enemy.GetMonsters().Count(card => card.IsFaceup()) >= 1;
        }

        /// <summary>原石的鸣狞：宣言「黑魔术师」。空场优先（直接从卡组拉），非空场也给黑魔术师战斗抗性。</summary>
        private bool PrimiteRoarEffect()
        {
            if (Bot.LifePoints <= 2000)
                return false;

            Trace("PrimiteRoar: declare DarkMagician, empty=" + (Bot.GetMonsterCount() == 0));
            AI.SelectAnnounceID(CardId.DarkMagician);
            return true;
        }

        /// <summary>
        /// 瘴烟之死灵术师（Link-2，2 只含暗属性魔法师族）：
        /// 只当 Link-3 的中转，②效果不发动（会把卡加手并封锁同名卡效果）。
        /// </summary>
        private bool NecromancerSummon()
        {
            if (!LineAllows(Act.Link, CardId.Necromancer))
                return false;
            // 只用杂鱼做，不能烧掉黑魔术师/师徒
            if (DisposableMonsters() < 2)
                return false;
            bool hasDarkCaster = Bot.GetMonsters()
                .Any(card => !IsKeyMonster(card) && IsMagicCaster(card) && card.HasAttribute(CardAttribute.Dark));
            if (!hasDarkCaster)
                return false;
            // 有 3 个杂鱼就直接塞勒涅，只有 2 个才用死灵术师中转
            return DisposableMonsters() < 3;
        }

        private bool WitchlingSummon()
        {
            if (DisposableMonsters() < 2)
                return false;
            if (Bot.GetMonsters().Count(card => !IsKeyMonster(card) && card.HasAttribute(CardAttribute.Dark)) < 2)
                return false;
            return Enemy.GetMonsters().Any(card => card.IsFaceup() && card.HasAttribute(CardAttribute.Light))
                || CurrentPlan() == Plan.Lethal;
        }

        private bool WitchlingEffect()
        {
            ClientCard target = Bot.GetGraveyardMonsters()
                .Where(card => card.HasAttribute(CardAttribute.Dark))
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool DharcSummon()
        {
            if (DisposableMonsters() < 2)
                return false;
            if (!Bot.GetMonsters().Any(card => !IsKeyMonster(card) && card.HasAttribute(CardAttribute.Dark)))
                return false;
            return Enemy.GetGraveyardMonsters().Any(card => card.HasAttribute(CardAttribute.Dark));
        }

        private bool DharcEffect()
        {
            ClientCard target = Enemy.GetGraveyardMonsters()
                .Where(card => card.HasAttribute(CardAttribute.Dark))
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        private bool DaybreakerSummon()
        {
            return DisposableMonsters() >= 2
                && Bot.GetMonsters().Count(card => !IsKeyMonster(card) && IsMagicCaster(card)) >= 2;
        }

        private bool DaybreakerEffect()
        {
            ClientCard target = PickEnemyCard();
            if (target == null)
                return false;
            AI.SelectCard(target);
            return true;
        }

        /// <summary>
        /// 塞贝克的魔导士（Link-2，效果怪兽 2 只）：
        /// ① 打伤害回血 → ② 双方各扣 1000。
        /// - 平时：拿攻击力合计不到 2000 的两只废件去换；
        /// - 对方血量 &lt; 1000 且这回合能打到人：也拿低攻怪做，靠 1000 烧血收头。
        /// </summary>
        private bool SebekSummon()
        {
            List<ClientCard> weakest = Bot.GetMonsters()
                .Where(card => !IsKeyMonster(card))
                .OrderBy(card => card.Attack)
                .Take(2)
                .ToList();
            if (weakest.Count < 2)
                return false;
            if (!weakest.All(card => card.HasType(CardType.Effect)))
                return false;

            // 情况一：两个废件合计不到 2000，换一个 2000 打点
            if (weakest.Sum(card => card.Attack) < 2000)
            {
                AI.SelectCard(weakest);
                Trace("Sebek: trade weak bodies (" + weakest[0].Id + "," + weakest[1].Id + ")");
                return true;
            }

            // 情况二：对方血量不足 1000，做出来打一下触发②烧 1000
            if (Enemy.LifePoints < 1000 && Bot.LifePoints > 1000)
            {
                bool canDealDamage = Enemy.GetMonsterCount() == 0
                    || Enemy.GetMonsters().Any(card => card.IsFaceup() && card.GetDefensePower() < 2000);
                if (canDealDamage)
                {
                    AI.SelectCard(weakest);
                    Trace("Sebek: burn finish, enemy lp=" + Enemy.LifePoints);
                    return true;
                }
            }

            return false;
        }

        // =====================================================================
        //  线路表（数据驱动）
        // =====================================================================

        private enum Act
        {
            Summon,     // 通常召唤
            Activate,   // 发动卡/效果
            Search,     // 检索
            Transform,  // 变身（黑魔术师 → 毁灭）
            Link        // 连接召唤
        }

        private sealed class Step
        {
            public readonly Act Act;
            public readonly int Card;
            public readonly int Target;

            public Step(Act act, int card, int target = 0)
            {
                Act = act;
                Card = card;
                Target = target;
            }
        }

        /// <summary>
        /// 共通主轴（杖开 / 帘幕开 / 阵+魂仆开 三条起手都汇到这里）：
        /// 杖 → 帘幕拿眼光 → 拉黑魔术师 → 变毁灭 → 毁灭拿王仆人
        /// → 王仆人展示起跳盖场地 → 毁灭+王仆人 link2 克劳利
        /// → 眼光点墓地 → 龙骑士/师徒 → +link2 出塞勒涅 → 塞勒涅③拉回
        /// </summary>
        private static readonly Step[] LineCurtain =
        {
            new Step(Act.Summon, CardId.MagiciansRod),
            new Step(Act.Search, CardId.MagiciansRod, CardId.DarkMagicCurtain),
            new Step(Act.Activate, CardId.DarkMagicCurtain, CardId.EyeOfTimaeus),
            new Step(Act.Transform, CardId.DarkMagicianOfDestruction),
            new Step(Act.Search, CardId.DarkMagicianOfDestruction, CardId.ServantOfTheKing),
            new Step(Act.Activate, CardId.ServantOfTheKing),
            new Step(Act.Link, CardId.Crowley),
            new Step(Act.Activate, CardId.EyeOfTimaeus),
            new Step(Act.Activate, CardId.MagiciansSalvation),
            new Step(Act.Link, CardId.Selene)
        };

        /// <summary>帕拉丁：攻击力 = 2900 + 双方场上·墓地龙族数量 × 500。打龙族卡组时是最优融合点。</summary>
        private static readonly int[] FusionPreferenceDarkPaladin =
        {
            CardId.DarkPaladin,
            CardId.DarkMagicianTheDragonKnight,
            CardId.TheDarkMagicians,
            CardId.MasterOfChaos
        };

        private static readonly Step[][] ComboLines = { LineCurtain };

        private Step[] _line;
        private int _lineStep;

        /// <summary>每回合开始时挑一条前提满足的线路。</summary>
        private void PickLine()
        {
            _line = null;
            _lineStep = 0;

            bool curtainLine = Bot.HasInHand(CardId.MagiciansRod)
                || Bot.HasInMonstersZone(CardId.MagiciansRod)
                || Bot.HasInHand(CardId.DarkMagicCurtain);

            if (curtainLine && !Bot.HasInGraveyard(CardId.DarkMagicCurtain))
                _line = LineCurtain;
        }

        private Step CurrentStep()
        {
            if (_line == null || _lineStep >= _line.Length)
                return null;
            return _line[_lineStep];
        }

        /// <summary>这一步还做不做得了（卡完全看不见就当放弃，避免卡死）。</summary>
        private bool StepPossible(Step step)
        {
            if (step.Card == 0)
                return true;

            // 检索目标在卡组里、变身是额外卡组的事，都看不见，不要因此判定「做不了」
            if (step.Act == Act.Search || step.Act == Act.Transform)
                return true;

            // 发动类步骤只看手卡/怪兽区/魔陷区；已经进墓地的卡不能算「还能发动」
            return Bot.HasInHand(step.Card)
                || Bot.HasInMonstersZone(step.Card)
                || Bot.HasInSpellZone(step.Card);
        }

        /// <summary>把已经完成的「召唤」步骤自动跳过（例如杖已经在场上）。</summary>
        private void AdvanceLine()
        {
            while (true)
            {
                Step step = CurrentStep();
                if (step == null)
                    return;
                if (step.Act == Act.Summon)
                {
                    // 召唤过就算数：在场，或者已经不在手卡
                    // （被奈落除外、被破坏都属于「已召唤」）
                    bool onField = Bot.HasInMonstersZone(step.Card);
                    bool stillInHand = Bot.HasInHand(step.Card);
                    if (onField || !stillInHand)
                    {
                        _lineStep++;
                        continue;
                    }
                }
                return;
            }
        }

        /// <summary>线路闸门：false = 这一步不该做（把动作让给别的线路/默认逻辑）。</summary>
        private bool LineAllows(Act act, int cardId)
        {
            return true;   // 实验：阻断全开
            AdvanceLine();
            Step step = CurrentStep();
            if (step == null)
                return true;

            // 循环跳过所有做不了的步骤，直到找到真正能做的那一步
            while (step != null && !StepPossible(step))
            {
                _lineStep++;
                step = CurrentStep();
            }

            if (step == null)
                return true;

            return step.Act == act && (step.Card == 0 || step.Card == cardId);
        }

        private void LineDone(Act act, int cardId)
        {
            Step step = CurrentStep();
            if (step != null && step.Act == act && (step.Card == 0 || step.Card == cardId))
                _lineStep++;
        }

        /// <summary>当前线路想要的检索目标（没有就返回 0）。</summary>
        private int LineSearchTarget()
        {
            AdvanceLine();
            Step step = CurrentStep();
            if (step == null || step.Act != Act.Search)
                return 0;

            // 目标已经在手/场/墓地 → 这张不用再检索了，跳过这一步，交给默认优先表
            int target = step.Target;
            if (target != 0
                && (Bot.HasInHand(target) || Bot.HasInMonstersZone(target)
                    || Bot.HasInSpellZone(target) || Bot.HasInGraveyard(target)))
            {
                _lineStep++;
                return 0;
            }

            return target;
        }

        // =====================================================================
        //  盖放 / 表示形式
        // =====================================================================

        /// <summary>同名卡已经在魔陷区（含盖着）就不再盖第二张。</summary>
        private bool NotOnField(int cardId)
        {
            return !Bot.SpellZone.Any(card => card != null && card.IsCode(cardId));
        }

        private bool SetTrap()
        {
            return true;
        }

        private bool ReposMonster()
        {
            return DefaultMonsterRepos();
        }

        // =====================================================================
        //  覆盖
        // =====================================================================

        public override bool OnSelectHand()
        {
            return true;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            // 师徒守备站场：2800/2300 不靠它打人，站着抽卡更稳
            // 师徒：看局势决定攻/守
            if (cardId == CardId.TheDarkMagicians)
            {
                // 对面有 3000+ 的怪 → 守备避战破；否则攻击表示抢血
                bool scary = Enemy.GetMonsters().Any(card => card != null && card.Id != 0 && card.Attack >= 3000);
                if (scary && positions.Contains(CardPosition.FaceUpDefence))
                    return CardPosition.FaceUpDefence;
                if (positions.Contains(CardPosition.FaceUpAttack))
                    return CardPosition.FaceUpAttack;
                if (positions.Contains(CardPosition.FaceUpDefence))
                    return CardPosition.FaceUpDefence;
            }

            YGOSharp.OCGWrapper.NamedCard data = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (data != null && data.Attack == 0 && positions.Contains(CardPosition.FaceUpDefence))
                return CardPosition.FaceUpDefence;

            // 师徒② 被破坏时拉回来的黑魔术师 / 黑魔术少女：
            // 在对方战斗阶段特召，如果对面还有 2500 以上的打点，就守备表示上来
            // （攻表当场被战破，等于白拉；守备至少能挡一拳）
            if ((cardId == CardId.DarkMagician || cardId == CardId.DarkMagicianGirl)
                && Duel.Player == 1
                && (Duel.Phase == DuelPhase.Battle || Duel.Phase == DuelPhase.BattleStart)
                && positions.Contains(CardPosition.FaceUpDefence)
                && Enemy.GetMonsters().Any(card => card != null && card.Id != 0 && card.Attack >= 2500))
            {
                Trace("Position: " + cardId + " defense (enemy >=2500 in battle phase)");
                return CardPosition.FaceUpDefence;
            }

            if (cardId == CardId.MagiciansRod
                || cardId == CardId.MagiciansSouls
                || cardId == CardId.PalladiumOracleMana
                || cardId == CardId.GhostMourner
                || cardId == CardId.Magikuriboh)
            {
                if (positions.Contains(CardPosition.FaceUpDefence))
                    return CardPosition.FaceUpDefence;
            }

            if (positions.Contains(CardPosition.FaceUpAttack))
                return CardPosition.FaceUpAttack;

            return base.OnSelectPosition(cardId, positions);
        }

        /// <summary>融合素材：先用黑魔术师/少女，其余按攻击力从低往高。</summary>
        public override IList<ClientCard> OnSelectFusionMaterial(IList<ClientCard> cards, int min, int max)
        {
            List<ClientCard> ordered = new List<ClientCard>();

            ClientCard darkMagician = cards.FirstOrDefault(card => IsDarkMagician(card));
            if (darkMagician != null)
                ordered.Add(darkMagician);

            ClientCard darkMagicianGirl = cards.FirstOrDefault(card => card.IsCode(CardId.DarkMagicianGirl));
            if (darkMagicianGirl != null && !ordered.Contains(darkMagicianGirl))
                ordered.Add(darkMagicianGirl);

            foreach (ClientCard card in cards.OrderBy(card => card.Attack))
            {
                if (!ordered.Contains(card))
                    ordered.Add(card);
            }

            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        /// <summary>
        /// Link 素材：永远先用杂鱼，黑魔术师系 / 师徒 / 龙骑士这类关键怪排到最后。
        /// 眼光拉出来的怪兽本来下回合会被除外，所以优先拿它当素材。
        /// </summary>
        public override IList<ClientCard> OnSelectLinkMaterial(IList<ClientCard> cards, int min, int max)
        {
            if (cards == null || cards.Count == 0)
                return null;

            // 访问码语者：只认「塞勒涅 + 1 只」这一组素材（其余情况宁可不出）
            if (_accesscodeMats != null)
            {
                // 引擎传进来的可能是同一张卡的另一个包装实例，按 id+位置匹配
                List<ClientCard> forced = cards
                    .Where(card => card != null && _accesscodeMats.Any(m => m != null
                        && m.Id == card.Id && m.Location == card.Location && m.Sequence == card.Sequence))
                    .ToList();
                _accesscodeMats = null;
                if (forced.Count == 2)
                {
                    Trace("Accesscode: use forced materials");
                    return Util.CheckSelectCount(forced, cards, min, max);
                }
            }

            List<ClientCard> ordered = new List<ClientCard>();

            if (_eyeActivatedThisTurn)
            {
                ClientCard temporary = cards.FirstOrDefault(card => card.IsCode(_eyeFusionId));
                if (temporary != null)
                    ordered.Add(temporary);
            }

            // 有连接怪兽可当素材时，只选最少数量（不要拿 3 只去凑 Link-3）
            // Link-3 目标 + Link-2 素材 = 只需再加 1 只
            // 素材排序：
            //   1. 连接怪兽优先（Link-2 当 2 个素材用，不要拿 3 只小怪去凑 Link-3）
            //   2. 毁灭（进墓地后能被眼光点起来融合）
            //   3. 杂鱼优先（攻击力从低到高）
            //   4. 关键怪最后
            foreach (ClientCard card in cards
                .OrderByDescending(card => card.HasType(CardType.Link) ? card.LinkCount + 10 : 0)
                .ThenBy(card => card.IsCode(CardId.DarkMagicianOfDestruction) ? 0 : 1)
                .ThenBy(card => IsKeyMonster(card) ? 1 : 0)
                .ThenBy(card => card.Attack))
            {
                if (!ordered.Contains(card))
                    ordered.Add(card);
            }

            // 有连接怪兽时只取最少数量的素材：
            //   目标 Link-3 + 素材里有 Link-2 → 只要 Link-2 + 1 只（2 张），不用 3 只小怪
            // 目标连接数推断：引擎一般把 max 传成目标 Link 数
            //   塞勒涅(Link-3)：min=2 max=3 → required=3 → 克劳利(2)+师徒(1)=3 → 只选 2 张
            //   克劳利(Link-2)：min=2 max=2 → required=2 → 两只小怪就停
            int required = Math.Max(min, max);
            int value = 0;
            List<ClientCard> minimal = new List<ClientCard>();
            foreach (ClientCard card in ordered)
            {
                minimal.Add(card);
                value += card.HasType(CardType.Link) ? Math.Max(1, card.LinkCount) : 1;
                if (value >= required && minimal.Count >= min)
                    break;
            }
            if (minimal.Count > 0 && minimal.Count < ordered.Count)
                return Util.CheckSelectCount(minimal, cards, min, max);

            return Util.CheckSelectCount(ordered, cards, min, max);
        }

        /// <summary>可以随便拿去当 Link 素材的杂鱼数量（不含黑魔术师系 / 师徒 / 龙骑士）。</summary>
        private int DisposableMonsters()
        {
            return Bot.GetMonsters().Count(card => !IsKeyMonster(card));
        }

        /// <summary>不能拿去当 Link 素材的关键怪兽（毁灭可以被眼光回收，算消耗品）。</summary>
        private static bool IsKeyMonster(ClientCard card)
        {
            if (card == null)
                return false;
            return card.IsCode(CardId.DarkMagician)
                || card.IsCode(CardId.ServantOfTheKing)
                || card.IsCode(CardId.DarkMagicianTheDragonKnight)
                || card.IsCode(CardId.TheDarkMagicians)
                || card.IsCode(CardId.ChaosBlackMagicKnight);
        }

        /// <summary>记录眼光到底拉出了哪只融合怪兽。</summary>
        public override void OnSpSummoned()
        {
            base.OnSpSummoned();
            if (!_eyeActivatedThisTurn)
                return;

            ClientCard summoned = Duel.LastSummonedCards
                .FirstOrDefault(card => card != null && card.Controller == 0 && card.IsExtraCard());
            if (summoned != null)
                _eyeFusionId = summoned.Id;
        }

        /// <summary>记录每次选卡请求的候选，便于复盘（不改变选择结果）。</summary>
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            Trace("SelectCard hint=" + hint + " min=" + min + " max=" + max
                + " offered=" + string.Join(",", cards.Where(card => card != null)
                    .Select(card => card.Id.ToString()).ToArray()));

            // 幕帘的特召选择：直接在这里挑黑魔术师，不依赖选卡队列
            if (_curtainStage == 1)
            {
                _curtainStage = 0;

                // 优先级：黑魔术师 > 黑魔术少女 > 其他（暗属性法师）
                ClientCard picked = cards.FirstOrDefault(card => card != null && card.IsCode(CardId.DarkMagician))
                    ?? cards.FirstOrDefault(card => card != null && card.IsCode(CardId.DarkMagicianGirl))
                    ?? cards.FirstOrDefault(card => card != null);

                if (picked != null)
                {
                    // 拿到的是 DM / 少女时，才能拿「追加检索」
                    if (picked.IsCode(CardId.DarkMagician) || picked.IsCode(CardId.DarkMagicianGirl))
                        AI.SelectCard(CurtainBonusTarget());
                    Trace("Curtain: picked " + picked.Id);
                    return new List<ClientCard> { picked };
                }

                Trace("Curtain: nothing offered, fallback");
                return null;
            }

            // 融合怪兽的选择（从额外卡组挑）：直接按我们的优先表选，避免被队列错位带跑
            if (_pickingFusion && cards.All(card => card != null && card.Location == CardLocation.Extra))
            {
                _pickingFusion = false;
                int[] pref = CurrentPlan() == Plan.DarkMagicians
                    ? FusionPreferenceDarkMagicians
                    : FusionPreference;
                foreach (int id in pref)
                {
                    ClientCard pick = cards.FirstOrDefault(card => card.IsCode(id));
                    if (pick != null)
                    {
                        Trace("Fusion pick: " + id);
                        return new List<ClientCard> { pick };
                    }
                }
            }

            // 王仆人盖放：按优先级从候选里挑（不靠队列）
            if (_servantSetTargets != null)
            {
                List<int> targets = _servantSetTargets;
                _servantSetTargets = null;
                foreach (int id in targets)
                {
                    ClientCard pick = cards.FirstOrDefault(card => card != null && card.IsCode(id));
                    if (pick != null)
                    {
                        Trace("Servant: set " + id);
                        return new List<ClientCard> { pick };
                    }
                }
            }

            // 眼光的目标选择：直接挑我们算好的那张（不靠队列）
            if (_eyeTargetCard != null)
            {
                ClientCard wanted = cards.FirstOrDefault(card => card != null && card.Id == _eyeTargetCard.Id);
                if (wanted != null)
                {
                    Trace("Eye: pick target " + wanted.Id);
                    _eyeTargetCard = null;
                    return new List<ClientCard> { wanted };
                }
                _eyeTargetCard = null;
            }

            // 幻想① 结算：两次选卡（从卡组拿卡 / 把手卡放回卡组），按候选来源判断是哪一次
            if (_illusionStage == 1 && Card.Location == CardLocation.Hand)
            {
                bool fromDeck = cards.Any(card => card != null && card.Location == CardLocation.Deck);
                if (fromDeck)
                {
                    _illusionStage = 0;
                    ClientCard pick = cards.FirstOrDefault(card => card != null && card.IsCode(_illusionSearchId));
                    if (pick != null)
                    {
                        Trace("Illusion: took " + _illusionSearchId);
                        return new List<ClientCard> { pick };
                    }
                    Trace("Illusion: target not offered");
                    return null;
                }

                // 手卡 → 卡组：挑最没用的还回去
                _illusionStage = 0;
                // 优先把「幻想自己」放回去：它结算完留在手里就是废牌
                ClientCard spare = cards.FirstOrDefault(card => card != null && card.IsCode(CardId.IllusionOfChaos))
                    ?? cards
                        .Where(card => card != null
                            && !IsKeyMonster(card)
                            && !card.IsCode(CardId.MagiciansRod)
                            && !card.IsCode(CardId.MagiciansSouls)
                            && !card.IsCode(CardId.EyeOfTimaeus)
                            && !card.IsCode(CardId.SpellbookOfSecrets)
                            && !card.IsCode(CardId.SpellbookOfKnowledge))
                        .OrderBy(card => card.IsSpell() ? 0 : 1)
                        .ThenBy(card => card.Attack)
                        .FirstOrDefault();
                if (spare != null)
                {
                    Trace("Illusion: return " + spare.Id);
                    return new List<ClientCard> { spare };
                }
                return null;
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        /// <summary>当前连锁最上面那张卡是不是指定的卡（用来放过对面的成金哥布林）。</summary>
        private bool ChainTopIs(int cardId)
        {
            IList<ClientCard> chain = Duel.CurrentChain;
            if (chain == null || chain.Count == 0)
                return false;
            ClientCard top = chain[chain.Count - 1];
            return top != null && top.IsCode(cardId);
        }

        /// <summary>魔导原典 克劳利：展示卡组顶 3 张，按「奥义 > 冰火 > 其他魔导书」拿 1 张。</summary>
        private bool CrowleyEffect()
        {
            AI.SelectCard(new[]
            {
                CardId.SpellbookOfSecrets,
                CardId.SpellbookOfKnowledge
            });
            return true;
        }

        /// <summary>怪粉坏兽 加达拉：把对方场上 1 只怪兽解放（不取对象、不破坏）。专门解“不能成为效果对象/不会被效果破坏”的大怪。</summary>
        private bool KaijuSummonGadarla()
        {
            ClientCard target = Enemy.GetMonsters()
                .Where(card => card != null && card.Id != 0)
                .OrderByDescending(card => card.Attack)
                .FirstOrDefault();
            if (target == null)
                return false;

            // 坏兽只留给 2500+ 的怪（不取对象、不破坏的大怪）
            if (target.Attack < 2500)
                return false;

            Trace("Kaiju: tribute " + target.Id + " atk=" + target.Attack);
            AI.SelectCard(target);
            return true;
        }

        /// <summary>成金哥布林：开局先用，抽 1（对面回 1000），顺便制造「本回合发动过魔法」让毁灭能起跳。</summary>
        private bool UpstartGoblinEffect()
        {
            Trace("SB: upstart tried");
            if (Bot.LifePoints <= 1500)
                return false;
            if (Bot.Hand.Count >= 6)
                return false;
            return true;
        }

        /// <summary>魔导书士 巴特尔：召唤成功时从卡组拿 1 张魔导书（优先奥义）。</summary>
        private bool SpellbookMagicianEffect()
        {
            Trace("SB: bartel search");
            AI.SelectCard(CardId.SpellbookOfSecrets);
            return true;
        }

        /// <summary>奥义之魔导书：拿 1 张魔导书（优先冰火）。</summary>
        private bool SpellbookOfSecretsEffect()
        {
            Trace("SB: secrets search");
            AI.SelectCard(new[] { CardId.SpellbookOfKnowledge, CardId.SpellbookOfSecrets });
            return true;
        }

        /// <summary>冰火之魔导书：只能以黑魔术师或 4 星以下的魔法师族为对象，抽 2。
        /// 实际只拿来吃 4 星以下的（巴特尔/杖/双魂），黑魔术师留着做主轴。</summary>
        private bool SpellbookOfKnowledgeEffect()
        {
            Trace("SB: knowledge tried");
            // 解放对象只能是这四类，且**绝不能选连接怪兽**（否则场值不够做塞勒涅）
            //   巴特尔 > 黑魔术师 > 杖 > 王仆人
            int[] order =
            {
                CardId.SpellbookMagician,
                CardId.DarkMagician,
                CardId.MagiciansRod,
                CardId.ServantOfTheKing
            };
            ClientCard fodder = null;
            foreach (int id in order)
            {
                fodder = Bot.GetMonsters().FirstOrDefault(card => card != null
                    && card.IsCode(id)
                    && !card.HasType(CardType.Link)
                    // 毁灭别名字是「黑魔术师」，但它是 2800 的打手，不能当祭品
                    && !card.IsCode(CardId.DarkMagicianOfDestruction)
                    && !card.IsCode(CardId.DarkMagicianTheDragonKnight)
                    && !card.IsCode(CardId.TheDarkMagicians));
                if (fodder != null)
                    break;
            }
            if (fodder == null)
                return false;
            Trace("SB: knowledge tribute " + fodder.Id);
            AI.SelectCard(fodder);
            return true;
        }

        /// <summary>魔导原典 克劳利：Link-2（2 只魔法师族）。</summary>
        private bool CrowleySummon()
        {
            if (!LineAllows(Act.Link, CardId.Crowley))
                return false;
            // 路线的 Link 步骤：毁灭 + 王仆人 就是 2 只，保底不适用
            bool routeMaterial = Bot.GetMonsters().Any(card => card.IsCode(CardId.DarkMagicianOfDestruction))
                && Bot.GetMonsters().Any(card => card.IsCode(CardId.ServantOfTheKing));

            // 保底：Link 完不能让场上空掉（除非能斩杀或路线要求）
            if (!routeMaterial && CurrentPlan() != Plan.Lethal && Bot.GetMonsterCount() < 3)
                return false;
            if (Bot.GetMonsters().Count(card => IsMagicCaster(card)) < 2)
                return false;
            Trace("STEP crowley");
            LineDone(Act.Link, CardId.Crowley);
            return true;
        }

        /// <summary>把卡列表压成 id 串，写日志用。</summary>
        private static string Ids(IEnumerable<ClientCard> cards)
        {
            return string.Join(",", cards.Where(card => card != null)
                .Select(card => card.Id.ToString()).ToArray());
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            _chaosKnightByKnightsTitle = false;
            _spellActivatedThisTurn = false;
            _soulServantUsedThisTurn = false;
            _eyeActivatedThisTurn = false;
            _eyeFusionId = 0;
            _curtainStage = 0;
            _pickingFusion = false;
            PickLine();

            // 每回合开头记一次场面快照，复盘时能直接对上
            Trace("==== TURN " + Duel.Turn + " player=" + Duel.Player
                + " LP=" + Bot.LifePoints + "/" + Enemy.LifePoints
                + " hand=[" + Ids(Bot.Hand) + "]"
                + " myfield=[" + Ids(Bot.GetMonsters()) + "]"
                + " oppfield=[" + Ids(Enemy.GetMonsters()) + "]"
                + " mygy=[" + Ids(Bot.Graveyard) + "]");
        }

        /// <summary>记录本回合自己是否发动过魔法卡（毁灭的出场条件）。</summary>
        public override void OnChaining(int player, ClientCard card)
        {
            base.OnChaining(player, card);
            if (player == 0 && card != null && card.IsSpell())
                _spellActivatedThisTurn = true;
            if (player == 0 && card != null && card.IsCode(CardId.SoulServant))
                _soulServantUsedThisTurn = true;
        }
    }

}
