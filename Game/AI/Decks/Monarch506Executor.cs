using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Monarch506", "AI_Monarch506")]
    class Monarch506Executor : DefaultExecutor
    {
        private const int HeroSetcode = 0x8;

        public class CardId
        {
            public const int LightAndDarknessDragon = 47297616; // 光与暗之龙
            public const int GorzTheEmissaryOfDarkness = 44330098; // 冥府之使者 格斯
            public const int RaizaTheStormMonarch = 73125233; // 风帝 莱扎
            public const int DestinyHEROMalicious = 9411399; // 命运英雄 魔性人
            public const int CyberDragon = 70095154; // 电子龙
            public const int ElementalHEROStratos = 40044918; // 元素英雄 天空侠
            public const int BreakerTheMagicalWarrior = 71413901; // 魔导战士 破坏者
            public const int DDWarriorLady = 7572887; // 异次元的女战士
            public const int GravekeepersSpy = 24317029; // 守墓的侦察者
            public const int DestinyHEROFearMonger = 80744121; // 命运英雄 破灭人
            public const int Sangan = 26202165; // 三眼怪
            public const int NeoSpacianGrandMole = 80344569; // 新空间侠·大地鼹鼠
            public const int SpiritReaper = 23205979; // 削魂的死灵
            public const int DestinyHERODiskCommander = 56570271; // 命运英雄 圆盘人
            public const int TreebornFrog = 12538374; // 黄泉青蛙
            public const int DDCrow = 24508238; // D.D.乌鸦
            public const int HeavyStorm = 19613556; // 大风暴
            public const int ReinforcementOfTheArmy = 32807846; // 增援
            public const int DestinyDraw = 45809008; // 命运抽卡
            public const int SoulExchange = 68005187; // 灵魂交错
            public const int FoolishBurial = 81439173; // 愚蠢的埋葬
            public const int BrainControl = 87910978; // 洗脑
            public const int MysticalSpaceTyphoon = 5318639; // 旋风
            public const int BookOfMoon = 14087893; // 月之书
            public const int PrematureBurial = 70828912; // 过早的埋葬
            public const int MirrorForce = 44095762; // 神圣防护罩 -反射镜力-
            public const int TorrentialTribute = 53582587; // 激流葬
            public const int CrushCardVirus = 57728570; // 死之卡组破坏病毒
            public const int PhoenixWingWindBlast = 63356631; // 凤翼的爆风
            public const int TrapDustshoot = 64697231; // 滑槽
            public const int CallOfTheHaunted = 97077563; // 活死人的呼声
        }

        private bool _normalSummoned;
        private bool _diskCommanderEffectUsed;
        private int _diskCommanderSentToGraveTurn = -1;
        private int _soulExchangeTurn = -1;

        protected bool UseNerfedCardEffects { get; set; }

        private bool DiskCommanderEffectAvailable
        {
            get { return !UseNerfedCardEffects || !_diskCommanderEffectUsed; }
        }

        public Monarch506Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // 光暗龙的无效效果是强制效果，必须先于所有可选响应处理。
            AddExecutor(ExecutorType.Activate, CardId.LightAndDarknessDragon);
            AddExecutor(ExecutorType.Activate, CardId.GorzTheEmissaryOfDarkness);
            AddExecutor(ExecutorType.Activate, CardId.DDCrow, DDCrowActivate);

            AddExecutor(ExecutorType.Activate, CardId.TrapDustshoot, TrapDustshootActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrushCardVirus, CrushCardVirusActivate);
            AddExecutor(ExecutorType.Activate, CardId.PhoenixWingWindBlast, PhoenixWingWindBlastActivate);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, MirrorForceActivate);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, TorrentialTributeActivate);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, MysticalSpaceTyphoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, BookOfMoonActivate);

            // 先完成过牌、检索和墓地准备，再把场面交给光暗龙封锁。
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, HeavyStormActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestinyDraw, DestinyDrawActivate);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyActivate);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialActivate);
            AddExecutor(ExecutorType.Activate, CardId.SoulExchange, SoulExchangeActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrainControl, BrainControlActivate);
            AddExecutor(ExecutorType.Activate, CardId.PrematureBurial, PrematureBurialActivate);

            AddExecutor(ExecutorType.Activate, CardId.TreebornFrog, TreebornFrogActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestinyHEROMalicious, DestinyHEROMaliciousActivate);
            AddExecutor(ExecutorType.Activate, CardId.DestinyHERODiskCommander, DestinyHERODiskCommanderActivate);

            // 先只反转侦察者赚取解放素材，避免通用上级召唤直接解放里侧怪兽。
            AddExecutor(ExecutorType.Repos, MonsterFlipSummon);

            AddExecutor(ExecutorType.Summon, CardId.LightAndDarknessDragon, LightAndDarknessDragonSummon);
            AddExecutor(ExecutorType.Summon, CardId.RaizaTheStormMonarch, RaizaTheStormMonarchSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragon);

            AddExecutor(ExecutorType.Summon, CardId.ElementalHEROStratos, ElementalHEROStratosSummon);
            AddExecutor(ExecutorType.Summon, CardId.BreakerTheMagicalWarrior, BreakerTheMagicalWarriorSummon);
            AddExecutor(ExecutorType.Summon, CardId.NeoSpacianGrandMole, NeoSpacianGrandMoleSummon);
            AddExecutor(ExecutorType.Summon, CardId.DDWarriorLady, DDWarriorLadySummon);
            AddExecutor(ExecutorType.Summon, CardId.SpiritReaper, SpiritReaperSummon);

            AddExecutor(ExecutorType.Activate, CardId.ElementalHEROStratos, ElementalHEROStratosActivate);
            AddExecutor(ExecutorType.Activate, CardId.BreakerTheMagicalWarrior, BreakerTheMagicalWarriorActivate);
            AddExecutor(ExecutorType.Activate, CardId.DDWarriorLady, DDWarriorLadyActivate);
            AddExecutor(ExecutorType.Activate, CardId.NeoSpacianGrandMole, NeoSpacianGrandMoleActivate);
            AddExecutor(ExecutorType.Activate, CardId.SpiritReaper);

            AddExecutor(ExecutorType.SpellSet, SpellSetForHandLimit);
            AddExecutor(ExecutorType.MonsterSet, CardId.GravekeepersSpy);
            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);
            AddExecutor(ExecutorType.MonsterSet, CardId.DestinyHEROFearMonger);
            AddExecutor(ExecutorType.MonsterSet, CardId.SpiritReaper);
            AddExecutor(ExecutorType.MonsterSet, CardId.DDWarriorLady);
            AddExecutor(ExecutorType.MonsterSet, CardId.DestinyHERODiskCommander);
            AddExecutor(ExecutorType.MonsterSet, CardId.TreebornFrog);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override void OnNewTurn()
        {
            _soulExchangeTurn = -1;
            _normalSummoned = false;
            base.OnNewTurn();
        }

        public override void OnSummoning()
        {
            if (Duel.LastSummonPlayer == 0)
                _normalSummoned = true;
            base.OnSummoning();
        }

        public override void OnMove(
            ClientCard card, int previousControler, int previousLocation,
            int currentControler, int currentLocation)
        {
            if (card != null &&
                Duel.Player == 0 &&
                previousControler == 0 &&
                previousLocation == (int)CardLocation.Hand &&
                currentControler == 0 &&
                currentLocation == (int)CardLocation.MonsterZone &&
                card.IsFacedown())
            {
                _normalSummoned = true;
            }

            if (UseNerfedCardEffects &&
                card != null &&
                card.IsCode(CardId.DestinyHERODiskCommander) &&
                currentControler == 0 &&
                currentLocation == (int)CardLocation.Grave &&
                previousLocation != (int)CardLocation.Grave)
            {
                _diskCommanderSentToGraveTurn = Duel.Turn;
            }

            base.OnMove(
                card, previousControler, previousLocation,
                currentControler, currentLocation);
        }

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            ChainInfo solvingChain = Duel.GetCurrentSolvingChainInfo();

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.LightAndDarknessDragon) &&
                hint == HintMsg.SpSummon)
            {
                List<ClientCard> targets = GetReviveTargets()
                    .Where(cards.Contains)
                    .ToList();
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.DestinyHEROFearMonger) &&
                hint == HintMsg.SpSummon)
            {
                List<int> priority = !DiskCommanderEffectAvailable
                    ? new List<int>
                    {
                        CardId.DestinyHEROMalicious,
                        CardId.DestinyHERODiskCommander
                    }
                    : new List<int>
                    {
                        CardId.DestinyHERODiskCommander,
                        CardId.DestinyHEROMalicious
                    };
                IList<ClientCard> targets = Util.SelectPreferredCards(
                    priority, cards, min, max);
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.RaizaTheStormMonarch) &&
                hint == HintMsg.ToDeck)
            {
                List<ClientCard> targets = new List<ClientCard>();
                ClientCard problematic = Util.GetProblematicEnemyCard(0, true);
                if (problematic != null &&
                    cards.Contains(problematic) &&
                    !problematic.HasType(CardType.Token) &&
                    !problematic.IsShouldNotBeTarget() &&
                    !problematic.IsShouldNotBeMonsterTarget())
                    targets.Add(problematic);

                ClientCard floodgate = Enemy.SpellZone.GetFloodgate();
                if (floodgate != null &&
                    cards.Contains(floodgate) &&
                    !floodgate.IsShouldNotBeTarget() &&
                    !floodgate.IsShouldNotBeMonsterTarget() &&
                    !targets.Contains(floodgate))
                    targets.Add(floodgate);
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 1 &&
                        card.IsSpell() &&
                        card.IsFacedown() &&
                        !card.IsShouldNotBeTarget() &&
                        !card.IsShouldNotBeMonsterTarget() &&
                        !targets.Contains(card)));
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 1 &&
                        card.IsMonster() &&
                        !card.HasType(CardType.Token) &&
                        !card.IsShouldNotBeTarget() &&
                        !card.IsShouldNotBeMonsterTarget() &&
                        !targets.Contains(card))
                    .OrderByDescending(card => card.IsMonsterDangerous())
                    .ThenByDescending(card => card.IsFaceup())
                    .ThenByDescending(card => card.GetDefensePower()));
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 1 &&
                        card.IsSpell() &&
                        !card.IsShouldNotBeTarget() &&
                        !card.IsShouldNotBeMonsterTarget() &&
                        !targets.Contains(card)));
                // 风帝的效果是必发；对方仍有合法候选时，宁可选通常不建议
                // 取对象的卡，也不能提前落入己方卡兜底。
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 1 &&
                        !targets.Contains(card))
                    .OrderByDescending(card => card.IsMonsterDangerous())
                    .ThenByDescending(card => card.IsFacedown())
                    .ThenByDescending(card => card.GetDefensePower()));
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 0 &&
                        card.IsSpell() &&
                        !targets.Contains(card))
                    .OrderBy(card => card.IsFaceup())
                    .ThenBy(card => card.Id));
                targets.AddRange(cards
                    .Where(card =>
                        card.Controller == 0 &&
                        card != currentChainCard &&
                        card.IsMonster() &&
                        !targets.Contains(card))
                    .OrderBy(card => card.GetDefensePower()));
                if (cards.Contains(currentChainCard))
                    targets.Add(currentChainCard);

                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (solvingChain != null &&
                solvingChain.ActivatePlayer == 0 &&
                solvingChain.IsActivateCode(CardId.TrapDustshoot) &&
                hint == HintMsg.ToDeck)
            {
                List<ClientCard> targets = cards
                    .Where(card => card.IsMonster())
                    .OrderByDescending(card => card.IsCode(CardId.GorzTheEmissaryOfDarkness))
                    .ThenByDescending(card => card.IsMonsterDangerous())
                    .ThenByDescending(card => card.Level)
                    .ThenByDescending(card => card.Attack)
                    .ToList();
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (solvingChain != null &&
                solvingChain.ActivatePlayer == 0 &&
                solvingChain.IsActivateCode(CardId.GravekeepersSpy) &&
                hint == HintMsg.SpSummon)
            {
                List<ClientCard> targets = cards
                    .Where(card => card.IsCode(CardId.GravekeepersSpy))
                    .ToList();
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (solvingChain != null &&
                solvingChain.ActivatePlayer == 0 &&
                solvingChain.IsActivateCode(CardId.Sangan) &&
                hint == HintMsg.AddToHand)
            {
                List<int> priority = new List<int>();
                if (Util.GetProblematicEnemyMonster(0, true) != null ||
                    Util.IsOneEnemyBetter())
                {
                    priority.Add(CardId.NeoSpacianGrandMole);
                    priority.Add(CardId.DDWarriorLady);
                }
                if (!Bot.HasInGraveyard(CardId.TreebornFrog) &&
                    Bot.HasInDeck(CardId.TreebornFrog))
                    priority.Add(CardId.TreebornFrog);
                if (!UseNerfedCardEffects &&
                    Bot.HasInHand(CardId.DestinyDraw) &&
                    !Bot.HasInGraveyard(CardId.DestinyHEROMalicious) &&
                    Bot.HasInDeck(CardId.DestinyHEROMalicious))
                    priority.Add(CardId.DestinyHEROMalicious);
                if (DiskCommanderEffectAvailable &&
                    Bot.HasInGraveyard(CardId.DestinyHERODiskCommander))
                    priority.Add(CardId.DestinyHEROFearMonger);
                if (!UseNerfedCardEffects &&
                    Enemy.GetGraveyardMonsters().Count > 0)
                    priority.Add(CardId.DDCrow);
                priority.AddRange(new[]
                {
                    CardId.SpiritReaper,
                    CardId.DDWarriorLady,
                    CardId.NeoSpacianGrandMole,
                    CardId.DestinyHEROFearMonger,
                    CardId.DestinyHEROMalicious,
                    CardId.DestinyHERODiskCommander,
                    CardId.TreebornFrog,
                    CardId.DDCrow,
                    CardId.Sangan
                });
                IList<ClientCard> targets = Util.SelectPreferredCards(
                    priority, cards, min, max);
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override CardPosition OnSelectPosition(
            int cardId, IList<CardPosition> positions)
        {
            if (cardId == CardId.GravekeepersSpy &&
                Duel.Player == 1 &&
                positions.Contains(CardPosition.FaceUpDefence))
            {
                return CardPosition.FaceUpDefence;
            }

            if (cardId == CardId.DestinyHEROMalicious ||
                cardId == CardId.TreebornFrog ||
                cardId == CardId.DestinyHERODiskCommander ||
                cardId == CardId.DestinyHEROFearMonger)
            {
                if (positions.Contains(CardPosition.FaceUpDefence))
                    return CardPosition.FaceUpDefence;
            }

            return base.OnSelectPosition(cardId, positions);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!attacker.IsDisabled() &&
                !defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.NeoSpacianGrandMole) &&
                    !OwnLightAndDarknessDragonCanNegate())
                {
                    attacker.RealPower = 9999;
                    return true;
                }

                if (attacker.IsCode(CardId.DDWarriorLady) &&
                    !OwnLightAndDarknessDragonCanNegate() &&
                    (defender.IsMonsterDangerous() ||
                    defender.IsMonsterInvincible() ||
                    defender.GetDefensePower() >= attacker.Attack))
                {
                    attacker.RealPower = 9999;
                    return true;
                }
            }

            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool OwnLightAndDarknessDragonCanNegate()
        {
            return !Duel.CurrentChain.Any(card =>
                    card.IsCode(CardId.LightAndDarknessDragon) &&
                    card.Controller == 0)
                && Bot.GetMonsters().Any(card =>
                    card.IsFaceup() &&
                    card.IsCode(CardId.LightAndDarknessDragon) &&
                    !card.IsDisabled() &&
                    card.Attack >= 500 &&
                    card.Defense >= 500);
        }

        private bool HasTributeMonsterInHand()
        {
            return Bot.HasInHand(new[]
            {
                CardId.LightAndDarknessDragon,
                CardId.RaizaTheStormMonarch
            });
        }

        private bool HasRevivalAvailable()
        {
            return (Bot.LifePoints > 800 &&
                Bot.HasInHand(CardId.PrematureBurial)) ||
                Bot.HasInHand(CardId.CallOfTheHaunted) ||
                Bot.GetSpells().Any(card =>
                    card.IsFacedown() &&
                    card.IsCode(CardId.CallOfTheHaunted));
        }

        private bool IsLowValueTribute(ClientCard card)
        {
            if (card == null)
                return false;
            // 对方持有的怪兽会在洗脑的结束阶段归还，应优先作为解放素材。
            if (card.Owner == 1)
                return true;
            if (card.IsCode(
                CardId.TreebornFrog,
                CardId.DestinyHEROMalicious,
                CardId.Sangan,
                CardId.DestinyHERODiskCommander))
                return true;
            if (card.IsCode(CardId.BreakerTheMagicalWarrior) && card.Attack <= 1600)
                return true;
            if (card.IsCode(
                CardId.ElementalHEROStratos,
                CardId.GravekeepersSpy,
                CardId.DestinyHEROFearMonger) && card.IsFaceup())
                return true;
            return false;
        }

        private List<ClientCard> GetLikelyTributes(int count)
        {
            // GameAI 的通用上级召唤选择会按攻击力从低到高解放。
            return Bot.GetMonsters()
                .OrderBy(card => card.Attack)
                .Take(count)
                .ToList();
        }

        private List<ClientCard> GetReviveTargets()
        {
            List<int> priority = new List<int>();
            if (DiskCommanderEffectAvailable)
                priority.Add(CardId.DestinyHERODiskCommander);
            priority.AddRange(new[]
            {
                CardId.ElementalHEROStratos,
                CardId.GorzTheEmissaryOfDarkness,
                CardId.RaizaTheStormMonarch,
                CardId.CyberDragon,
                CardId.DDWarriorLady,
                CardId.BreakerTheMagicalWarrior,
                CardId.NeoSpacianGrandMole,
                CardId.Sangan,
                CardId.DestinyHEROFearMonger,
                CardId.GravekeepersSpy,
                CardId.SpiritReaper,
                CardId.DestinyHEROMalicious,
                CardId.TreebornFrog
            });
            if (!DiskCommanderEffectAvailable)
                priority.Add(CardId.DestinyHERODiskCommander);

            List<ClientCard> monsters = Bot.GetGraveyardMonsters()
                .Where(card =>
                    card.IsCanRevive() &&
                    !card.IsCode(CardId.LightAndDarknessDragon) &&
                    (!card.IsCode(CardId.DestinyHERODiskCommander) ||
                    !UseNerfedCardEffects ||
                    _diskCommanderSentToGraveTurn != Duel.Turn))
                .ToList();
            return priority
                .SelectMany(id => monsters.Where(card => card.IsCode(id)))
                .Concat(monsters.Where(card => !priority.Any(id => card.IsCode(id))))
                .Distinct()
                .ToList();
        }

        private bool ShouldStopAttack(ClientCard attacker)
        {
            if (attacker == null)
                return false;

            if (Bot.BattlingMonster != null)
            {
                if (Bot.BattlingMonster.IsCode(
                    CardId.Sangan,
                    CardId.DestinyHEROFearMonger,
                    CardId.SpiritReaper))
                {
                    return attacker.IsMonsterDangerous() ||
                        Bot.BattlingMonster.IsAttack() &&
                        attacker.Attack - Bot.BattlingMonster.Attack >= Bot.LifePoints;
                }
                return attacker.GetDefensePower() >=
                    Bot.BattlingMonster.GetDefensePower();
            }

            int totalAttack = Util.GetTotalAttackingMonsterAttack(1);
            return attacker.IsMonsterDangerous() ||
                attacker.IsCode(CardId.SpiritReaper) ||
                attacker.Attack >= Bot.LifePoints ||
                totalAttack >= Bot.LifePoints &&
                totalAttack - attacker.Attack < Bot.LifePoints;
        }

        // ===== 魔法与陷阱 =====

        private bool HeavyStormActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            int enemyCount = Enemy.GetSpellCount();
            int botCount = Bot.GetSpellCount() -
                (Card.Location == CardLocation.SpellZone ? 1 : 0);
            return enemyCount >= botCount + 2 ||
                Enemy.SpellZone.GetFloodgate() != null;
        }

        private bool DestinyDrawActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<int> discardPriority = new List<int>();
            bool hasMaliciousRemaining = Bot.HasInDeck(CardId.DestinyHEROMalicious);
            if (hasMaliciousRemaining)
                discardPriority.Add(CardId.DestinyHEROMalicious);
            if (DiskCommanderEffectAvailable && HasRevivalAvailable())
                discardPriority.Add(CardId.DestinyHERODiskCommander);
            if (!hasMaliciousRemaining)
                discardPriority.Add(CardId.DestinyHEROMalicious);
            if (!DiskCommanderEffectAvailable)
                discardPriority.Add(CardId.DestinyHERODiskCommander);
            discardPriority.Add(CardId.DestinyHEROFearMonger);
            discardPriority.Add(CardId.DestinyHEROMalicious);
            discardPriority.Add(CardId.DestinyHERODiskCommander);
            AI.SelectCard(discardPriority);
            return true;
        }

        private bool ReinforcementOfTheArmyActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<int> priority = new List<int>();
            if (Bot.HasInDeck(CardId.ElementalHEROStratos))
                priority.Add(CardId.ElementalHEROStratos);
            if (Util.GetProblematicEnemyMonster(0, true) != null ||
                Util.IsOneEnemyBetter())
                priority.Add(CardId.DDWarriorLady);
            if (DiskCommanderEffectAvailable &&
                Bot.HasInGraveyard(CardId.DestinyHERODiskCommander))
                priority.Add(CardId.DestinyHEROFearMonger);
            if (Bot.HasInHand(CardId.DestinyDraw))
                priority.Add(CardId.DestinyHERODiskCommander);
            priority.AddRange(new[]
            {
                CardId.DDWarriorLady,
                CardId.DestinyHEROFearMonger,
                CardId.DestinyHERODiskCommander
            });
            AI.SelectCard(priority);
            return true;
        }

        private bool FoolishBurialActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<int> priority = new List<int>();
            if (!Bot.HasInGraveyard(CardId.TreebornFrog) &&
                Bot.HasInDeck(CardId.TreebornFrog))
                priority.Add(CardId.TreebornFrog);
            if (Bot.GetCardCountInDeck(CardId.DestinyHEROMalicious) >= 2)
                priority.Add(CardId.DestinyHEROMalicious);
            if (DiskCommanderEffectAvailable &&
                HasRevivalAvailable() &&
                Bot.HasInDeck(CardId.DestinyHERODiskCommander))
                priority.Add(CardId.DestinyHERODiskCommander);
            if (priority.Count == 0)
                return false;

            AI.SelectCard(priority);
            return true;
        }

        private bool SoulExchangeActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                Enemy.GetMonsterCount() == 0)
                return false;

            bool enablesRaiza =
                Bot.GetMonsterCount() == 0 &&
                Bot.HasInHand(CardId.RaizaTheStormMonarch);
            bool enablesLightAndDarknessDragon =
                Bot.GetMonsterCount() == 1 &&
                Bot.HasInHand(CardId.LightAndDarknessDragon);
            if (!enablesRaiza && !enablesLightAndDarknessDragon)
                return false;

            List<ClientCard> targets = Enemy.GetMonsters()
                .OrderByDescending(card =>
                    enablesRaiza &&
                    card.HasType(CardType.Token))
                .ThenByDescending(card => card.IsMonsterDangerous())
                .ThenByDescending(card => card.IsMonsterInvincible())
                .ThenByDescending(card => card.IsFaceup())
                .ThenByDescending(card => card.GetDefensePower())
                .ToList();
            AI.SelectCard(targets);
            _soulExchangeTurn = Duel.Turn;
            return true;
        }

        private bool BrainControlActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                Bot.LifePoints <= 800)
                return false;

            List<ClientCard> targets = Enemy.GetMonsters()
                .Where(card => card.IsFaceup() &&
                    (!UseNerfedCardEffects ||
                    !card.IsExtraCard() &&
                    !card.HasType(
                        CardType.Ritual |
                        CardType.SpSummon |
                        CardType.Token)) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeSpellTrapTarget())
                .OrderByDescending(card => card.IsMonsterDangerous())
                .ThenByDescending(card => card.GetDefensePower())
                .ToList();
            if (targets.Count == 0)
                return false;

            ClientCard target = targets.First();
            List<ClientCard> projectedTributes = Bot.GetMonsters()
                .Concat(new[] { target })
                .OrderBy(card => card.Attack)
                .ToList();
            bool raizaHasTargetAfterControl =
                Enemy.GetMonsters().Any(card =>
                    card != target &&
                    !card.HasType(CardType.Token) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget()) ||
                Enemy.GetSpells().Any(card =>
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget());
            bool enablesRaiza =
                Bot.GetMonsterCount() == 0 &&
                Bot.HasInHand(CardId.RaizaTheStormMonarch) &&
                raizaHasTargetAfterControl &&
                ShouldSummonRaizaTheStormMonarch(
                    projectedTributes.Take(1).ToList());
            bool enablesLightAndDarknessDragon =
                Bot.GetMonsterCount() == 1 &&
                Bot.HasInHand(CardId.LightAndDarknessDragon) &&
                ShouldSummonLightAndDarknessDragon(
                    projectedTributes.Take(2).ToList());
            if (Duel.Phase == DuelPhase.Main2 &&
                !enablesRaiza &&
                !enablesLightAndDarknessDragon)
                return false;

            int currentAttack = Bot.GetMonsters()
                .Where(card => card.IsFaceup() && card.IsAttack())
                .Sum(card => card.Attack);
            bool clearsForLethal =
                Enemy.GetMonsterCount() == 1 &&
                currentAttack + target.Attack >= Enemy.LifePoints;
            bool worthwhile =
                enablesRaiza ||
                enablesLightAndDarknessDragon ||
                clearsForLethal ||
                target.IsMonsterDangerous() ||
                target.GetDefensePower() >= 2100;
            if (!worthwhile)
                return false;

            AI.SelectCard(targets);
            return true;
        }

        private bool PrematureBurialActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                Bot.LifePoints <= 800)
                return false;

            List<ClientCard> targets = GetReviveTargets();
            if (targets.Count == 0)
                return false;

            ClientCard target = null;
            foreach (ClientCard candidate in targets)
            {
                List<ClientCard> projectedTributes = Bot.GetMonsters()
                    .Concat(new[] { candidate })
                    .OrderBy(card => card.Attack)
                    .ToList();
                bool tributePlay =
                    (Bot.HasInHand(CardId.LightAndDarknessDragon) &&
                    ShouldSummonLightAndDarknessDragon(
                        projectedTributes.Take(2).ToList())) ||
                    (Bot.HasInHand(CardId.RaizaTheStormMonarch) &&
                    ShouldSummonRaizaTheStormMonarch(
                        projectedTributes.Take(1).ToList()));
                bool effectValue =
                    candidate.IsCode(CardId.ElementalHEROStratos) ||
                    DiskCommanderEffectAvailable &&
                    candidate.IsCode(CardId.DestinyHERODiskCommander);
                bool battleValue = candidate.Attack >= 1800 ||
                    Enemy.GetMonsterCount() == 0 &&
                    Bot.GetMonsters().Where(card => card.IsAttack())
                        .Sum(card => card.Attack) +
                    candidate.Attack >= Enemy.LifePoints;
                if (tributePlay || effectValue || battleValue)
                {
                    target = candidate;
                    break;
                }
            }
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool MysticalSpaceTyphoonActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            ClientCard lastChainCard = Util.GetLastChainCard();
            bool equipTargetsSpiritReaper =
                lastChainCard != null &&
                lastChainCard.Controller == 1 &&
                lastChainCard.IsSpell() &&
                lastChainCard.HasType(CardType.Equip) &&
                Duel.LastChainTargets.Any(card =>
                    card.IsCode(CardId.SpiritReaper));
            if (equipTargetsSpiritReaper)
                return false;

            return DefaultMysticalSpaceTyphoon();
        }

        private bool BookOfMoonActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            ClientCard attacker = Enemy.BattlingMonster;
            if (ShouldStopAttack(attacker) &&
                attacker.IsFaceup() &&
                !attacker.HasType(CardType.Link) &&
                !IsCardAlreadyHandledInCurrentChain(attacker))
            {
                AI.SelectCard(attacker);
                return true;
            }

            ClientCard threat = Duel.LastSummonedCards
                .FirstOrDefault(card =>
                    card.Controller == 1 &&
                    card.IsFloodgate() &&
                    card.IsFaceup() &&
                    !card.HasType(CardType.Link) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeSpellTrapTarget() &&
                    !IsCardAlreadyHandledInCurrentChain(card));
            if (threat == null &&
                Duel.Phase > DuelPhase.Main1 &&
                Duel.Phase < DuelPhase.Main2)
            {
                threat = Util.GetProblematicEnemyMonster(0, true);
                if (threat != null &&
                    (!threat.IsFaceup() ||
                    threat.HasType(CardType.Link) ||
                    threat.IsShouldNotBeTarget() ||
                    threat.IsShouldNotBeSpellTrapTarget() ||
                    IsCardAlreadyHandledInCurrentChain(threat)))
                {
                    threat = null;
                }
            }
            if (threat == null)
                return false;

            AI.SelectCard(threat);
            return true;
        }

        private bool TrapDustshootActivate()
        {
            return !OwnLightAndDarknessDragonCanNegate();
        }

        private bool CrushCardVirusActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            bool protectVirus = Util.IsChainTarget(Card);
            // 削弱后的脚本会把对方受到的伤害归零至下个回合结束。
            if (UseNerfedCardEffects &&
                Duel.Player == 0 &&
                Duel.Phase < DuelPhase.Main2 &&
                !protectVirus)
                return false;

            // SelectTribute 不消费预选队列；发动判断必须按通用逻辑实际会选的
            // 最低攻击力怪兽评估，不能假设这里能指定三眼怪等费用。
            ClientCard tribute = Bot.GetMonsters()
                .Where(card =>
                    card.HasAttribute(CardAttribute.Dark) &&
                    card.Attack <= 1000)
                .OrderBy(card => card.Attack)
                .FirstOrDefault();
            if (tribute == null)
                return true; // 虽然没找到祭品但反正可以发动

            bool lowValueCost =
                IsLowValueTribute(tribute) ||
                tribute.IsCode(CardId.DestinyHEROFearMonger) ||
                tribute.IsCode(CardId.DDCrow) &&
                Enemy.GetGraveyardMonsters().Count == 0;
            bool strongMonster = Enemy.GetMonsters().Any(card =>
                card.IsFaceup() && card.Attack >= 1500);
            int handThreshold = UseNerfedCardEffects
                ? (lowValueCost ? 3 : 4)
                : (lowValueCost ? 2 : 3);
            bool worthwhile =
                protectVirus ||
                Enemy.Hand.Count >= handThreshold ||
                strongMonster ||
                Enemy.GetMonsterCount() >= 2;
            if (!worthwhile)
                return false;

            return true;
        }

        private bool PhoenixWingWindBlastActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<ClientCard> targets = new List<ClientCard>();
            ClientCard problematic = Util.GetProblematicEnemyCard(0, true);
            if (problematic != null &&
                (problematic.HasType(CardType.Token) ||
                IsCardAlreadyHandledInCurrentChain(problematic)))
                problematic = null;
            if (problematic != null &&
                !problematic.IsShouldNotBeTarget() &&
                !problematic.IsShouldNotBeSpellTrapTarget())
                targets.Add(problematic);

            ClientCard attacker = Enemy.BattlingMonster;
            bool stopAttack =
                ShouldStopAttack(attacker) &&
                attacker != null &&
                !attacker.HasType(CardType.Token) &&
                !attacker.IsShouldNotBeTarget() &&
                !attacker.IsShouldNotBeSpellTrapTarget() &&
                !IsCardAlreadyHandledInCurrentChain(attacker);
            if (stopAttack && !targets.Contains(attacker))
                targets.Add(attacker);

            targets.AddRange(Enemy.GetMonsters()
                .Where(card =>
                    !targets.Contains(card) &&
                    !card.HasType(CardType.Token) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeSpellTrapTarget() &&
                    !IsCardAlreadyHandledInCurrentChain(card))
                .OrderByDescending(card => card.IsMonsterDangerous())
                .ThenByDescending(card => card.IsFaceup())
                .ThenByDescending(card => card.GetDefensePower()));
            targets.AddRange(Enemy.GetSpells()
                .Where(card =>
                    !targets.Contains(card) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeSpellTrapTarget() &&
                    !IsCardAlreadyHandledInCurrentChain(card))
                .OrderByDescending(card => card.IsFloodgate())
                .ThenByDescending(card => card.IsFacedown()));
            if (targets.Count == 0)
                return false;

            List<ClientCard> discards = Bot.Hand
                .OrderBy(card =>
                {
                    if (card.IsCode(CardId.TreebornFrog)) return 0;
                    if (card.IsCode(CardId.DestinyHEROMalicious) &&
                        Bot.HasInDeck(CardId.DestinyHEROMalicious))
                        return 1;
                    if (card.IsCode(CardId.DestinyHERODiskCommander) &&
                        (!DiskCommanderEffectAvailable || HasRevivalAvailable()))
                        return 2;
                    if (Bot.Hand.Count(other => other.Id == card.Id) > 1) return 3;
                    if (card.IsCode(CardId.DestinyHEROFearMonger)) return 4;
                    if (card.IsCode(CardId.SpiritReaper)) return 5;
                    if (card.IsCode(CardId.DDCrow) &&
                        Enemy.GetGraveyardMonsters().Count == 0)
                        return 6;
                    if (card.IsTrap()) return 7;
                    if (card.IsSpell()) return 8;
                    if (card.IsCode(CardId.RaizaTheStormMonarch)) return 10;
                    if (card.IsCode(CardId.LightAndDarknessDragon)) return 11;
                    if (card.IsCode(CardId.GorzTheEmissaryOfDarkness)) return 12;
                    return 9;
                })
                .ThenBy(card => card.Attack)
                .ToList();

            int discardPriority = discards.Count > 0
                ? (discards[0].IsCode(
                    CardId.TreebornFrog,
                    CardId.DestinyHEROMalicious,
                    CardId.DestinyHERODiskCommander,
                    CardId.DestinyHEROFearMonger) ? 0 : 1)
                : 2;
            bool protectedDiscard = discards.Count > 0 &&
                discards[0].IsCode(
                    CardId.GorzTheEmissaryOfDarkness,
                    CardId.LightAndDarknessDragon,
                    CardId.RaizaTheStormMonarch);
            bool endPhaseLock =
                Duel.Player == 1 &&
                Duel.Phase == DuelPhase.End &&
                !protectedDiscard &&
                (targets[0].IsFacedown() ||
                targets[0].IsMonsterDangerous() ||
                targets[0].GetDefensePower() >= 1800);
            bool useNow =
                Util.IsChainTarget(Card) ||
                problematic != null ||
                stopAttack ||
                endPhaseLock ||
                targets.Count >= 2 && discardPriority == 0;
            if (!useNow)
                return false;

            // 脚本先选择丢弃费用，再选择回到卡组顶端的对象。
            AI.SelectCard(discards);
            AI.SelectNextCard(targets);
            return true;
        }

        private bool MirrorForceActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            ClientCard attacker = Enemy.BattlingMonster;
            int attackPositionCount =
                Enemy.GetMonsters().Count(card => card.IsAttack());
            bool canTrade = Bot.BattlingMonster != null &&
                Bot.BattlingMonster.IsCode(
                    CardId.Sangan,
                    CardId.DestinyHEROFearMonger,
                    CardId.SpiritReaper);
            bool lethalDamage = Bot.BattlingMonster != null &&
                Bot.BattlingMonster.IsAttack() &&
                attacker.Attack - Bot.BattlingMonster.Attack >= Bot.LifePoints;
            bool worthwhile =
                attackPositionCount >= 2 ||
                attacker.IsMonsterDangerous() ||
                attacker.IsCode(CardId.SpiritReaper) ||
                lethalDamage ||
                !canTrade &&
                (attacker.Attack >= 1800 ||
                Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints);
            return worthwhile && DefaultUniqueTrap();
        }

        private bool TorrentialTributeActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                Util.HasChainedTrap(0))
                return false;

            int enemyCount = Enemy.GetMonsterCount();
            int botCount = Bot.GetMonsterCount();
            if (enemyCount > 0 &&
                Enemy.GetMonsters().All(IsCardAlreadyHandledInCurrentChain))
                return false;

            bool dangerousSummon = Duel.LastSummonedCards.Any(card =>
                card.Controller == 1 &&
                (card.IsMonsterDangerous() || card.IsMonsterInvincible()));
            bool recoverableField = Bot.GetMonsters().All(card =>
                IsLowValueTribute(card) ||
                card.IsCode(
                    CardId.DestinyHEROFearMonger,
                    CardId.SpiritReaper));
            return dangerousSummon ||
                enemyCount >= 2 && enemyCount > botCount ||
                enemyCount > botCount && recoverableField ||
                Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints;
        }

        private bool CallOfTheHauntedActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<ClientCard> targets = GetReviveTargets();
            if (targets.Count == 0)
                return false;

            bool protectCall = Util.IsChainTarget(Card);
            bool stopDirectAttack =
                Bot.UnderAttack &&
                Bot.BattlingMonster == null &&
                Enemy.BattlingMonster != null &&
                targets.Any(card => card.IsCode(
                    CardId.DDWarriorLady,
                    CardId.Sangan));
            bool stopBattle =
                Duel.Player == 1 &&
                Duel.Phase > DuelPhase.Main1 &&
                Duel.Phase < DuelPhase.Main2 &&
                ShouldStopAttack(Enemy.BattlingMonster);
            bool reviveEffectMonster = targets.Any(card =>
                card.IsCode(CardId.ElementalHEROStratos) ||
                DiskCommanderEffectAvailable &&
                card.IsCode(CardId.DestinyHERODiskCommander));
            bool prepareTribute =
                Duel.Player == 0 &&
                !_normalSummoned &&
                HasTributeMonsterInHand() &&
                Bot.GetMonsterCount() == 0;
            bool opponentEndPhase =
                Duel.Player == 1 &&
                Duel.Phase == DuelPhase.End &&
                (reviveEffectMonster || targets.Any(card => card.Attack >= 1800));
            if (!protectCall &&
                !stopDirectAttack &&
                !stopBattle &&
                !prepareTribute &&
                !opponentEndPhase &&
                !(Duel.Player == 0 && reviveEffectMonster))
                return false;

            if (stopDirectAttack)
            {
                targets = targets
                    .OrderByDescending(card =>
                        card.IsCode(CardId.DDWarriorLady))
                    .ThenByDescending(card =>
                        card.IsCode(CardId.Sangan))
                    .ToList();
            }
            else if (stopBattle)
            {
                targets = targets
                    .OrderByDescending(card => card.Attack)
                    .ToList();
            }
            AI.SelectCard(targets);
            return true;
        }

        private bool SpellSet()
        {
            if (!DefaultSpellSet() ||
                Bot.GetSpellCountWithoutField() >= 3)
                return false;

            // 格斯需要完全空场；在没有怪兽保护时不要用普通盖卡关闭它。
            if (Bot.HasInHand(CardId.GorzTheEmissaryOfDarkness) &&
                Bot.GetMonsterCount() == 0 &&
                Bot.GetSpellCount() == 0)
            {
                return Card.IsCode(CardId.TrapDustshoot) &&
                    Enemy.Hand.Count >= 4;
            }

            bool needsTreeborn =
                Bot.HasInGraveyard(CardId.TreebornFrog) &&
                Bot.GetMonsterCount() == 0 &&
                HasTributeMonsterInHand();
            if (!needsTreeborn)
                return true;

            if (Card.IsCode(CardId.TrapDustshoot))
                return Enemy.Hand.Count >= 4;
            if (Card.IsCode(CardId.PhoenixWingWindBlast))
                return Bot.Hand.Count >= 2 && Enemy.GetMonsterCount() > 0;
            if (Card.IsCode(CardId.CrushCardVirus))
                return Bot.GetMonsters().Any(card =>
                    card.HasAttribute(CardAttribute.Dark) &&
                    card.Attack <= 1000);
            if (Card.IsCode(CardId.CallOfTheHaunted))
                return Bot.HasInGraveyard(CardId.ElementalHEROStratos) ||
                    DiskCommanderEffectAvailable &&
                    Bot.HasInGraveyard(CardId.DestinyHERODiskCommander);
            if (Card.IsCode(CardId.MysticalSpaceTyphoon))
                return Enemy.GetSpellCount() > 0;
            return false;
        }

        private bool SpellSetForHandLimit()
        {
            return Duel.Phase == DuelPhase.Main2 &&
                Bot.Hand.Count > 6 &&
                Card.IsSpell();
        }

        // ===== 怪兽的召唤与效果 =====

        private bool LightAndDarknessDragonSummon()
        {
            return ShouldSummonLightAndDarknessDragon(
                GetLikelyTributes(2));
        }

        private bool ShouldSummonLightAndDarknessDragon(
            IList<ClientCard> tributes)
        {
            if (_normalSummoned ||
                OwnLightAndDarknessDragonCanNegate())
                return false;

            bool soulExchangeSuppliesOne =
                _soulExchangeTurn == Duel.Turn &&
                tributes.Count == 1;
            if (tributes.Count < 2 && !soulExchangeSuppliesOne)
                return false;

            int lowValueCount = tributes.Count(IsLowValueTribute);
            bool underPressure =
                Util.GetProblematicEnemyCard() != null ||
                Util.IsAllEnemyBetter(true) ||
                Enemy.GetMonsterCount() >= 2;
            if (soulExchangeSuppliesOne)
                return lowValueCount > 0 || underPressure;
            if (lowValueCount >= 2)
                return Bot.GetSpellCount() <= 1 || underPressure;

            int tributePower = tributes.Sum(card => card.GetDefensePower());
            return (underPressure ||
                tributes.Any(card => card.Owner == 1)) &&
                tributePower <= 3000 &&
                Bot.GetSpellCount() <= 1;
        }

        private bool RaizaTheStormMonarchSummon()
        {
            return ShouldSummonRaizaTheStormMonarch(
                GetLikelyTributes(1));
        }

        private bool ShouldSummonRaizaTheStormMonarch(
            IList<ClientCard> tributes)
        {
            int returnableMonsterCount = Enemy.GetMonsters()
                .Count(card =>
                    !card.HasType(CardType.Token) &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget());
            int returnableSpellCount = Enemy.GetSpells()
                .Count(card =>
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget());
            if (_normalSummoned ||
                OwnLightAndDarknessDragonCanNegate() ||
                returnableMonsterCount + returnableSpellCount == 0)
                return false;

            bool soulExchangeUsed = _soulExchangeTurn == Duel.Turn;
            if (soulExchangeUsed &&
                returnableSpellCount == 0 &&
                (Enemy.GetMonsterCount() < 2 ||
                returnableMonsterCount == 0))
                return false;

            if (tributes.Count == 0)
                return soulExchangeUsed;

            ClientCard tribute = tributes[0];
            bool valuableTarget =
                Util.GetProblematicEnemyCard(0, true) != null ||
                Enemy.GetMonsters().Any(card =>
                    card.IsMonsterDangerous() ||
                    card.GetDefensePower() >= 1800) ||
                Enemy.GetSpells().Any(card =>
                    card.IsFacedown() || card.IsFloodgate());
            return IsLowValueTribute(tribute) ||
                valuableTarget &&
                tribute.GetDefensePower() < 2400;
        }

        private bool ElementalHEROStratosSummon()
        {
            return !OwnLightAndDarknessDragonCanNegate();
        }

        private bool ElementalHEROStratosActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            List<int> searchPriority = new List<int>();
            bool maliciousInGrave =
                Bot.HasInGraveyard(CardId.DestinyHEROMalicious);
            bool hasDestinyDraw =
                Bot.HasInHandOrInSpellZone(CardId.DestinyDraw);
            bool hasPhoenixWingWindBlast =
                Bot.HasInHandOrInSpellZone(CardId.PhoenixWingWindBlast);
            if (hasDestinyDraw &&
                !maliciousInGrave &&
                Bot.HasInDeck(CardId.DestinyHEROMalicious))
                searchPriority.Add(CardId.DestinyHEROMalicious);
            if (DiskCommanderEffectAvailable &&
                Bot.HasInGraveyard(CardId.DestinyHERODiskCommander) &&
                Bot.HasInDeck(CardId.DestinyHEROFearMonger))
                searchPriority.Add(CardId.DestinyHEROFearMonger);
            if (hasDestinyDraw &&
                Bot.HasInDeck(CardId.DestinyHERODiskCommander))
                searchPriority.Add(CardId.DestinyHERODiskCommander);
            if (!hasDestinyDraw &&
                !hasPhoenixWingWindBlast &&
                DiskCommanderEffectAvailable &&
                Bot.HasInDeck(CardId.DestinyHERODiskCommander))
            {
                searchPriority.Add(CardId.DestinyHERODiskCommander);
            }
            if (!maliciousInGrave &&
                Bot.HasInDeck(CardId.DestinyHEROMalicious))
                searchPriority.Add(CardId.DestinyHEROMalicious);
            if (Bot.HasInDeck(CardId.DestinyHEROFearMonger))
                searchPriority.Add(CardId.DestinyHEROFearMonger);

            int otherHeroCount = Bot.GetMonsters().Count(card =>
                !card.Equals(Card) &&
                card.IsFaceup() &&
                card.HasSetcode(HeroSetcode));
            List<ClientCard> enemySpells = Enemy.GetSpells();
            ClientCard floodgate = Enemy.SpellZone.GetFloodgate();
            if (otherHeroCount > 0 &&
                enemySpells.Count > 0 &&
                (searchPriority.Count == 0 ||
                floodgate != null ||
                enemySpells.Count >= 2))
            {
                AI.SelectOption(0);
                List<ClientCard> targets = new List<ClientCard>();
                if (floodgate != null)
                    targets.Add(floodgate);
                targets.AddRange(enemySpells
                    .Where(card => card.IsFacedown() && !targets.Contains(card)));
                targets.AddRange(enemySpells
                    .Where(card => !targets.Contains(card)));
                AI.SelectCard(targets);
                return true;
            }

            if (searchPriority.Count == 0)
                return false;

            AI.SelectOption(1);
            AI.SelectCard(searchPriority);
            return true;
        }

        private bool BreakerTheMagicalWarriorSummon()
        {
            return !OwnLightAndDarknessDragonCanNegate() &&
                (Enemy.GetSpellCount() > 0 ||
                Enemy.GetMonsterCount() == 0 ||
                !Util.IsAllEnemyBetterThanValue(1900, true));
        }

        private bool BreakerTheMagicalWarriorActivate()
        {
            // 放置魔力指示物的召唤成功效果没有目标，也会经过同一执行器。
            if (ActivateDescription ==
                Util.GetStringId(CardId.BreakerTheMagicalWarrior, 0) ||
                (ActivateDescription == -1 &&
                Duel.LastSummonedCards.Contains(Card) &&
                Card.Attack <= 1600))
                return true;

            if (OwnLightAndDarknessDragonCanNegate())
                return false;

            ClientCard target = Util.GetBestEnemySpell(true);
            if (target != null &&
                (target.IsShouldNotBeTarget() ||
                target.IsShouldNotBeMonsterTarget()))
                target = null;
            if (target == null)
            {
                target = Enemy.GetSpells().FirstOrDefault(card =>
                    card.IsFacedown() &&
                    !card.IsShouldNotBeTarget() &&
                    !card.IsShouldNotBeMonsterTarget());
            }
            if (target == null)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool NeoSpacianGrandMoleSummon()
        {
            return !OwnLightAndDarknessDragonCanNegate() &&
                Enemy.GetMonsters().Any(card =>
                card.IsMonsterDangerous() ||
                card.IsMonsterInvincible() ||
                card.GetDefensePower() >= 1800);
        }

        private bool NeoSpacianGrandMoleActivate()
        {
            ClientCard target = Enemy.BattlingMonster;
            return !OwnLightAndDarknessDragonCanNegate() &&
                target != null &&
                (target.IsMonsterDangerous() ||
                target.IsMonsterInvincible() ||
                target.GetDefensePower() >= Card.Attack);
        }

        private bool DDWarriorLadySummon()
        {
            return Enemy.GetMonsterCount() > 0 ||
                Enemy.GetMonsterCount() == 0 &&
                !Util.IsTurn1OrMain2();
        }

        private bool DDWarriorLadyActivate()
        {
            ClientCard target = Enemy.BattlingMonster;
            return !OwnLightAndDarknessDragonCanNegate() &&
                target != null &&
                (target.IsMonsterDangerous() ||
                target.IsMonsterInvincible() ||
                target.GetDefensePower() >= Card.Attack);
        }

        private bool SpiritReaperSummon()
        {
            return !OwnLightAndDarknessDragonCanNegate() &&
                Enemy.GetMonsterCount() == 0 &&
                !Util.IsTurn1OrMain2();
        }

        private bool TreebornFrogActivate()
        {
            return !OwnLightAndDarknessDragonCanNegate();
        }

        private bool DestinyHEROMaliciousActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                _normalSummoned)
                return false;

            // 按魔性人已经特殊召唤后的场面判断，避免拉出后又放弃上级召唤。
            List<ClientCard> projectedTributes = Bot.GetMonsters()
                .Concat(new[] { Card })
                .OrderBy(card => card.Attack)
                .ToList();

            if (Bot.HasInHand(CardId.LightAndDarknessDragon) &&
                ShouldSummonLightAndDarknessDragon(
                    projectedTributes.Take(2).ToList()))
                return true;

            if (Bot.HasInHand(CardId.RaizaTheStormMonarch) &&
                ShouldSummonRaizaTheStormMonarch(
                    projectedTributes.Take(1).ToList()))
                return true;

            return false;
        }

        private bool DestinyHERODiskCommanderActivate()
        {
            // 原版是必发效果；削弱版才可以为了避开光暗龙无效而不发动。
            if (UseNerfedCardEffects && OwnLightAndDarknessDragonCanNegate())
                return false;

            if (UseNerfedCardEffects)
                _diskCommanderEffectUsed = true;
            return true;
        }

        private bool DDCrowActivate()
        {
            if (OwnLightAndDarknessDragonCanNegate() ||
                Duel.LastChainPlayer != 1)
                return false;

            List<ClientCard> targets = Duel.LastChainTargets
                .Where(card =>
                    card.Controller == 1 &&
                    card.Location == CardLocation.Grave)
                .OrderByDescending(card => card.IsMonsterDangerous())
                .ThenByDescending(card => card.Attack)
                .ToList();
            if (targets.Count == 0)
                return false;

            AI.SelectCard(targets);
            return true;
        }

        private bool MonsterFlipSummon()
        {
            return Card.IsFacedown() && MonsterRepos();
        }

        private bool MonsterRepos()
        {
            if (Card.IsCode(CardId.GravekeepersSpy) && Card.IsFacedown())
                return Bot.HasInDeck(CardId.GravekeepersSpy);

            if (Card.IsCode(
                CardId.TreebornFrog,
                CardId.DestinyHEROMalicious,
                CardId.DestinyHERODiskCommander))
                return Card.IsAttack();

            if (Card.IsCode(CardId.SpiritReaper))
            {
                int otherAttackers = Bot.GetMonsters().Count(card =>
                    !card.Equals(Card) &&
                    card.IsFaceup() &&
                    card.IsAttack());
                bool canAttackDirectly =
                    !Util.IsTurn1OrMain2() &&
                    otherAttackers >= Enemy.GetMonsterCount();
                if (Card.IsFacedown())
                    return canAttackDirectly;
                return Card.IsDefense()
                    ? canAttackDirectly
                    : !canAttackDirectly;
            }

            if (Card.IsCode(CardId.DestinyHEROFearMonger) &&
                Card.IsFacedown())
                return Enemy.GetMonsterCount() == 0 &&
                    !Util.IsTurn1OrMain2();

            return DefaultMonsterRepos();
        }
    }
}
