using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Chaos408", "AI_Chaos408")]
    class Chaos408Executor : DefaultExecutor
    {
        public class CardId
        {
            public const int ChaosSorcerer = 9596126; // 混沌巫师
            public const int ZaborgTheThunderMonarch = 51945556; // 雷帝 扎博尔格
            public const int CyberDragon = 70095154; // 电子龙
            public const int BreakerTheMagicalWarrior = 71413901; // 魔导战士 破坏者
            public const int DDWarriorLady = 7572887; // 异次元的女战士
            public const int MysticTomato = 83011277; // 杀人番茄
            public const int Tsukuyomi = 34853266; // 月读命
            public const int ExiledForce = 74131780; // 流氓佣兵部队
            public const int Sangan = 26202165; // 三眼怪
            public const int Marshmallon = 31305911; // 棉花糖
            public const int SpiritReaper = 23205979; // 削魂的死灵
            public const int MagicianOfFaith = 31560081; // 圣魔术师
            public const int GracefulCharity = 79571449; // 天使的施舍
            public const int Confiscation = 17375316; // 收押
            public const int HeavyStorm = 19613556; // 大风暴
            public const int CreatureSwap = 31036355; // 强制转移
            public const int PotOfAvarice = 67169062; // 贪欲之壶
            public const int NoblemanOfCrossout = 71044499; // 抹杀之使徒
            public const int SmashingGround = 97169186; // 地碎
            public const int MysticalSpaceTyphoon = 5318639; // 旋风
            public const int BookOfMoon = 14087893; // 月之书
            public const int EnemyController = 98045062; // 敌人操纵器
            public const int SnatchSteal = 45986603; // 强夺
            public const int PrematureBurial = 70828912; // 过早的埋葬
            public const int BottomlessTrapHole = 29401950; // 奈落的落穴
            public const int MirrorForce = 44095762; // 神圣防护罩 -反射镜力-
            public const int TorrentialTribute = 53582587; // 激流葬
            public const int CallOfTheHaunted = 97077563; // 活死人的呼声
        }

        public Chaos408Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // 先清后场和干扰手牌；这两项不会改变电子龙的特召条件。
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, HeavyStormActivate);
            AddExecutor(ExecutorType.Activate, CardId.Confiscation);

            // 先开天使的施舍过牌。
            AddExecutor(ExecutorType.Activate, CardId.GracefulCharity);

            // 电子龙必须先于除去、夺取和复活执行，否则这些动作可能让双方都有怪兽。
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.ChaosSorcerer, ChaosSorcererSummon);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSorcerer, ChaosSorcererActivate);

            AddExecutor(ExecutorType.Activate, CardId.NoblemanOfCrossout, NoblemanActivate);
            AddExecutor(ExecutorType.Activate, CardId.SnatchSteal, SnatchStealActivate);
            AddExecutor(ExecutorType.Activate, CardId.PrematureBurial, PrematureBurialActivate);
            AddExecutor(ExecutorType.Activate, CardId.SmashingGround, DefaultSmashingGround);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, MysticalSpaceTyphoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, BookOfMoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.EnemyController, EnemyControllerActivate);
            AddExecutor(ExecutorType.Activate, CardId.CreatureSwap, CreatureSwapActivate);

            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, MirrorForceActivate);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedActivate);

            AddExecutor(ExecutorType.Activate, CardId.BreakerTheMagicalWarrior, BreakerActivate);
            AddExecutor(ExecutorType.Activate, CardId.ExiledForce, ExiledForceActivate);
            AddExecutor(ExecutorType.Activate, CardId.MysticTomato, MysticTomatoActivate);
            AddExecutor(ExecutorType.Activate, CardId.DDWarriorLady, DDWarriorLadyActivate);

            // 先处理反转召唤，避免直接把尚未发动反转效果的怪兽解放掉。
            AddExecutor(ExecutorType.Repos, MonsterRepos);

            // 针对性通常召唤必须先于通用打手。
            AddExecutor(ExecutorType.Summon, CardId.ZaborgTheThunderMonarch, ZaborgSummon);
            AddExecutor(ExecutorType.Summon, CardId.ExiledForce, ExiledForceSummon);
            AddExecutor(ExecutorType.Summon, CardId.Tsukuyomi, TsukuyomiSummon);
            AddExecutor(ExecutorType.Summon, CardId.SpiritReaper, SpiritReaperSummon);
            AddExecutor(ExecutorType.Summon, CardId.BreakerTheMagicalWarrior);
            AddExecutor(ExecutorType.SummonOrSet, CardId.DDWarriorLady);
            AddExecutor(ExecutorType.SummonOrSet, CardId.MysticTomato);

            // 等混沌召唤和复活判断结束后，再洗回墓地资源。
            AddExecutor(ExecutorType.Activate, CardId.PotOfAvarice, PotOfAvariceActivate);

            AddExecutor(ExecutorType.MonsterSet, CardId.MagicianOfFaith, MagicianOfFaithSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);
            AddExecutor(ExecutorType.MonsterSet, CardId.Marshmallon);
            AddExecutor(ExecutorType.MonsterSet, CardId.SpiritReaper);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentChainCard = Duel.GetCurrentChainCard();
            ClientCard solvingChainCard = Duel.GetCurrentSolvingChainCard();

            if (solvingChainCard != null &&
                solvingChainCard.Controller == 0 &&
                solvingChainCard.IsCode(CardId.GracefulCharity) &&
                hint == HintMsg.Discard)
            {
                List<ClientCard> selected = new List<ClientCard>();
                List<ClientCard> graveyardMonsters = Bot.GetGraveyardMonsters();
                bool graveyardHasLight = graveyardMonsters
                    .Any(c => c.HasAttribute(CardAttribute.Light));
                bool graveyardHasDark = graveyardMonsters
                    .Any(c => c.HasAttribute(CardAttribute.Dark));
                bool handHasChaosSorcerer = Bot.HasInHand(CardId.ChaosSorcerer);

                if (handHasChaosSorcerer && !graveyardHasLight)
                    AddGracefulCharityDiscards(selected, cards, c =>
                        c.IsMonster() && c.HasAttribute(CardAttribute.Light));
                if (handHasChaosSorcerer && !graveyardHasDark)
                    AddGracefulCharityDiscards(selected, cards, c =>
                        c.IsMonster() &&
                        c.HasAttribute(CardAttribute.Dark) &&
                        !c.IsCode(CardId.ChaosSorcerer));

                IEnumerable<ClientCard> duplicateCards = cards
                    .GroupBy(c => c.Id)
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group.Skip(1));
                AddGracefulCharityDiscards(
                    selected, duplicateCards, c => true, 2);

                if (!Bot.Hand.Any(c => c.IsMonster()) &&
                    Bot.GetMonsterCount() == 0)
                    AddGracefulCharityDiscards(selected, cards,
                        c => c.IsCode(CardId.CreatureSwap));

                if (Bot.HasInHand(CardId.MagicianOfFaith) &&
                    !Bot.Graveyard.Any(c => c.IsSpell()))
                    AddGracefulCharityDiscards(
                        selected, cards, c => c.IsSpell());

                // 已选中的怪兽也会进入墓地，后续不再重复补同属性。
                if (!graveyardHasLight && !selected.Any(c =>
                    c.IsMonster() && c.HasAttribute(CardAttribute.Light)))
                    AddGracefulCharityDiscards(selected, cards, c =>
                        c.IsMonster() && c.HasAttribute(CardAttribute.Light));
                if (!graveyardHasDark && !selected.Any(c =>
                    c.IsMonster() && c.HasAttribute(CardAttribute.Dark)))
                    AddGracefulCharityDiscards(selected, cards, c =>
                        c.IsMonster() &&
                        c.HasAttribute(CardAttribute.Dark) &&
                        !c.IsCode(CardId.ChaosSorcerer));

                if (Bot.GetMonsterCount() > 0 || Util.IsTurn1OrMain2())
                    AddGracefulCharityDiscards(selected, cards,
                        c => c.IsCode(CardId.CyberDragon));
                if (Bot.GetMonsterCount() == 0)
                    AddGracefulCharityDiscards(selected, cards,
                        c => c.IsCode(CardId.ZaborgTheThunderMonarch));

                if (Bot.Hand.Count(c => c.IsMonster()) == 1)
                    AddGracefulCharityDiscards(selected, cards,
                        c => c.IsSpell() || c.IsTrap());
                if (graveyardMonsters.Count < 3)
                    AddGracefulCharityDiscards(selected, cards,
                        c => c.IsCode(CardId.PotOfAvarice));

                int[] discardOrder =
                {
                    CardId.CyberDragon,
                    CardId.ChaosSorcerer,
                    CardId.SpiritReaper,
                    CardId.SmashingGround,
                    CardId.ExiledForce,
                    CardId.MysticTomato,
                    CardId.CreatureSwap,
                    CardId.PotOfAvarice
                };
                foreach (int cardId in discardOrder)
                    AddGracefulCharityDiscards(
                        selected, cards, c => c.IsCode(cardId));

                return Util.CheckSelectCount(selected, cards, min, max);
            }

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.ZaborgTheThunderMonarch) &&
                hint == HintMsg.Destroy)
            {
                List<ClientCard> targets = new List<ClientCard>();

                ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
                if (problematic != null && cards.Contains(problematic) &&
                    !problematic.IsShouldNotBeTarget() &&
                    !problematic.IsShouldNotBeMonsterTarget())
                    targets.Add(problematic);

                if (targets.Count < max)
                    targets.AddRange(cards
                        .Where(c => c.Controller == 1 && c != problematic &&
                            !c.IsShouldNotBeTarget() && !c.IsShouldNotBeMonsterTarget())
                        .OrderByDescending(c => c.IsFaceup())
                        .ThenByDescending(c => c.GetDefensePower()));

                if (targets.Count < max)
                    targets.AddRange(cards
                        .Where(c => c.Controller == 1 && !targets.Contains(c))
                        .OrderByDescending(c => c.IsFaceup())
                        .ThenByDescending(c => c.GetDefensePower()));

                if (targets.Count < max)
                    targets.AddRange(cards
                        .Where(c => c.Controller == 0 &&
                            !c.IsCode(CardId.ZaborgTheThunderMonarch) &&
                            !c.IsShouldNotBeMonsterTarget())
                        .OrderBy(c => c.IsCode(CardId.Sangan) ? 0 : 1)
                        .ThenBy(c => c.GetDefensePower()));

                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.Tsukuyomi) &&
                hint == HintMsg.Faceup)
            {
                ClientCard target = GetTsukuyomiTarget(cards) ??
                    cards.FirstOrDefault(c => c == currentChainCard);
                if (target != null)
                    return Util.CheckSelectCount(
                        new List<ClientCard> { target }, cards, min, max);
            }

            if (currentChainCard != null &&
                currentChainCard.Controller == 0 &&
                currentChainCard.IsCode(CardId.MagicianOfFaith) &&
                hint == HintMsg.AddToHand)
            {
                List<int> priority = new List<int>();
                if (Enemy.GetMonsters().Any(c => c.IsFaceup()))
                    priority.Add(CardId.SnatchSteal);
                if (Bot.LifePoints > 800 && Bot.GetGraveyardMonsters()
                    .Any(c => c.IsCanRevive() && !c.IsCode(CardId.Tsukuyomi)))
                    priority.Add(CardId.PrematureBurial);
                if (Bot.GetGraveyardMonsters().Count >= 5)
                    priority.Add(CardId.PotOfAvarice);

                int enemySpellCount = Enemy.GetSpellCount();
                int botSpellCount = Bot.GetSpellCount();
                if (enemySpellCount >= botSpellCount + 2 ||
                    Enemy.SpellZone.GetFloodgate() != null && botSpellCount == 0)
                    priority.Add(CardId.HeavyStorm);
                if (Enemy.GetMonsterCount() > 0)
                    priority.Add(CardId.SmashingGround);
                if (Enemy.GetMonsters().Any(c => c.IsFacedown()))
                    priority.Add(CardId.NoblemanOfCrossout);
                if (enemySpellCount > 0)
                    priority.Add(CardId.MysticalSpaceTyphoon);
                if (Enemy.Hand.Count > 0 && Bot.LifePoints > 1000)
                    priority.Add(CardId.Confiscation);

                priority.AddRange(new[]
                {
                    CardId.SnatchSteal,
                    CardId.PrematureBurial,
                    CardId.PotOfAvarice,
                    CardId.HeavyStorm,
                    CardId.CreatureSwap,
                    CardId.Confiscation,
                    CardId.SmashingGround,
                    CardId.NoblemanOfCrossout,
                    CardId.MysticalSpaceTyphoon,
                    CardId.EnemyController,
                    CardId.BookOfMoon
                });
                IList<ClientCard> targets = Util.SelectPreferredCards(
                    priority.Distinct().ToList(), cards, min, max);
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            if (solvingChainCard != null &&
                solvingChainCard.Controller == 0 &&
                solvingChainCard.IsCode(CardId.Sangan) &&
                hint == HintMsg.AddToHand)
            {
                List<int> priority = Duel.Player == 0
                    ? new List<int>
                    {
                        CardId.Marshmallon,
                        CardId.SpiritReaper,
                        CardId.DDWarriorLady,
                        CardId.MysticTomato,
                        CardId.MagicianOfFaith,
                        CardId.Tsukuyomi,
                        CardId.ExiledForce,
                        CardId.Sangan
                    }
                    : new List<int>
                    {
                        CardId.DDWarriorLady,
                        CardId.Tsukuyomi,
                        CardId.ExiledForce,
                        CardId.MagicianOfFaith,
                        CardId.SpiritReaper,
                        CardId.Marshmallon,
                        CardId.MysticTomato,
                        CardId.Sangan
                    };
                IList<ClientCard> targets = Util.SelectPreferredCards(
                    priority, cards, min, max);
                return Util.CheckSelectCount(targets, cards, min, max);
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        private void AddGracefulCharityDiscards(
            List<ClientCard> selected,
            IEnumerable<ClientCard> cards,
            Func<ClientCard, bool> predicate,
            int count = 1)
        {
            foreach (ClientCard card in cards)
            {
                if (selected.Count >= 2 || count == 0)
                    return;
                if (selected.Contains(card) || !predicate(card))
                    continue;

                selected.Add(card);
                count--;
            }
        }

        public override bool OnSelectMonsterSummonOrSet(ClientCard card)
        {
            if (Duel.Turn == 1 && Enemy.GetMonsterCount() == 0 &&
                card.IsCode(CardId.DDWarriorLady, CardId.MysticTomato))
                return true;
            return base.OnSelectMonsterSummonOrSet(card);
        }

        private bool MonsterRepos()
        {
            if (Card.IsCode(CardId.MagicianOfFaith) && Card.IsFacedown() &&
                !Bot.Graveyard.Any(c => c.IsSpell()))
                return false;

            if (Card.IsCode(CardId.SpiritReaper))
            {
                int attackers = Bot.GetMonsters().Count(c =>
                    c.IsFaceup() && c.IsAttack() && c.Attack >= 1500);
                bool shouldAttack = attackers >= Enemy.GetMonsterCount();
                return Card.IsDefense()
                    ? shouldAttack
                    : !shouldAttack;
            }

            if (Card.IsCode(CardId.Marshmallon))
            {
                if (Card.IsFacedown())
                    return Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= Card.Attack;
                return Card.IsDefense()
                    ? Enemy.GetMonsterCount() == 0
                    : Enemy.GetMonsterCount() > 0;
            }

            return DefaultMonsterRepos();
        }

        private bool ShouldUseSetQuickPlayForMagicianOfFaith()
        {
            return Duel.Player == 1 &&
                Card.Location == CardLocation.SpellZone &&
                Card.IsFacedown() &&
                Bot.GetMonsters().Any(c =>
                    c.IsCode(CardId.MagicianOfFaith) && c.IsFacedown());
        }

        private bool ShouldStopAttack(ClientCard attacker)
        {
            if (attacker == null) return false;
            if (Bot.BattlingMonster != null)
            {
                if (Bot.BattlingMonster.IsCode(
                    CardId.Sangan,
                    CardId.MysticTomato,
                    CardId.Marshmallon,
                    CardId.SpiritReaper))
                    return attacker.IsMonsterDangerous() ||
                        Bot.BattlingMonster.IsAttack() &&
                        attacker.Attack - Bot.BattlingMonster.Attack >= Bot.LifePoints;
                return attacker.GetDefensePower() >= Bot.BattlingMonster.GetDefensePower();
            }

            int totalEnemyAttack = Util.GetTotalAttackingMonsterAttack(1);
            return attacker.IsMonsterDangerous() ||
                attacker.IsCode(CardId.SpiritReaper) ||
                attacker.Attack >= Bot.LifePoints ||
                totalEnemyAttack >= Bot.LifePoints &&
                totalEnemyAttack - attacker.Attack < Bot.LifePoints;
        }

        // ===== 魔法激活 =====

        // 大风暴：取得明确卡差，或必须处理表侧压制卡时激活
        private bool HeavyStormActivate()
        {
            int enemyCount = Enemy.GetSpellCount();
            int myCount = Bot.GetSpellCount() -
                (Card.Location == CardLocation.SpellZone ? 1 : 0);
            return enemyCount >= myCount + 2 || Enemy.SpellZone.GetFloodgate() != null;
        }

        // 抹杀之使徒：目标为对方背面防守怪兽
        private bool NoblemanActivate()
        {
            ClientCard target = Enemy.GetMonsters().FirstOrDefault(c => c.IsFacedown());
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool MysticalSpaceTyphoonActivate()
        {
            ClientCard lastChainCard = Util.GetLastChainCard();
            bool equipTargetsSpiritReaper = lastChainCard != null &&
                lastChainCard.Controller == 1 &&
                lastChainCard.IsSpell() &&
                lastChainCard.HasType(CardType.Equip) &&
                Duel.LastChainTargets.Any(c => c.IsCode(CardId.SpiritReaper));
            if (equipTargetsSpiritReaper)
                return false;

            if (DefaultMysticalSpaceTyphoon())
                return true;

            if (ShouldUseSetQuickPlayForMagicianOfFaith())
            {
                ClientCard target = Enemy.GetSpells()
                    .FirstOrDefault(c => c.IsFacedown());
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        private bool BookOfMoonActivate()
        {
            ClientCard endangered = Bot.GetMonsters()
                .FirstOrDefault(c => c.IsFaceup() && !c.HasType(CardType.Link) && Util.IsChainTarget(c));
            if (endangered != null)
            {
                AI.SelectCard(endangered);
                return true;
            }

            ClientCard attacker = Enemy.BattlingMonster;
            if (ShouldStopAttack(attacker) && attacker.IsFaceup() &&
                !attacker.HasType(CardType.Link))
            {
                AI.SelectCard(attacker);
                return true;
            }

            ClientCard magician = Bot.GetMonsters()
                .FirstOrDefault(c => c.IsFaceup() && c.IsCode(CardId.MagicianOfFaith));
            if (magician != null && !Bot.HasInHand(CardId.Tsukuyomi) &&
                Bot.Graveyard.Any(c => c.IsSpell()))
            {
                AI.SelectCard(magician);
                return true;
            }

            ClientCard threat = Util.GetProblematicEnemyMonster(0, true);
            if (threat != null && threat.IsFaceup() && !threat.HasType(CardType.Link) &&
                !threat.IsShouldNotBeTarget() &&
                !threat.IsShouldNotBeSpellTrapTarget())
            {
                AI.SelectCard(threat);
                return true;
            }

            if (ShouldUseSetQuickPlayForMagicianOfFaith())
            {
                ClientCard target = Enemy.GetMonsters()
                    .Where(c => c.IsFaceup() &&
                        !c.HasType(CardType.Link | CardType.Token) &&
                        !c.IsShouldNotBeTarget() &&
                        !c.IsShouldNotBeSpellTrapTarget())
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        // 贪欲之壶：回收低价值怪兽，同时尽量保留混沌和复活所需的墓地资源
        private bool PotOfAvariceActivate()
        {
            List<ClientCard> monsters = Bot.GetGraveyardMonsters();

            List<ClientCard> keep = new List<ClientCard>();
            if (Bot.HasInHand(CardId.ChaosSorcerer))
            {
                int[] lightKeepOrder =
                {
                    CardId.MagicianOfFaith,
                    CardId.Marshmallon,
                    CardId.DDWarriorLady,
                    CardId.CyberDragon,
                    CardId.ZaborgTheThunderMonarch
                };
                int[] darkKeepOrder =
                {
                    CardId.Tsukuyomi,
                    CardId.Sangan,
                    CardId.MysticTomato,
                    CardId.SpiritReaper,
                    CardId.BreakerTheMagicalWarrior,
                    CardId.ChaosSorcerer
                };
                ClientCard light = lightKeepOrder
                    .Select(id => monsters.FirstOrDefault(c =>
                        c.HasAttribute(CardAttribute.Light) && c.IsCode(id)))
                    .FirstOrDefault(c => c != null);
                ClientCard dark = monsters
                    .FirstOrDefault(c => c.HasAttribute(CardAttribute.Dark) &&
                        c.IsCode(CardId.ChaosSorcerer) && !c.IsCanRevive())
                    ?? darkKeepOrder
                        .Select(id => monsters.FirstOrDefault(c =>
                            c.HasAttribute(CardAttribute.Dark) && c.IsCode(id)))
                        .FirstOrDefault(c => c != null);
                if (light != null) keep.Add(light);
                if (dark != null) keep.Add(dark);
            }

            bool hasUsableRevival = Bot.HasInHand(CardId.PrematureBurial) ||
                Bot.HasInHand(CardId.CallOfTheHaunted) ||
                Bot.GetSpells().Any(c => c.IsFacedown() &&
                    c.IsCode(CardId.PrematureBurial, CardId.CallOfTheHaunted));
            if (hasUsableRevival)
            {
                ClientCard revive = monsters
                    .Where(c => c.IsCanRevive() && !c.IsCode(CardId.Tsukuyomi))
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault();
                if (revive != null && !keep.Contains(revive)) keep.Add(revive);
            }

            int[] shuffleOrder =
            {
                CardId.Tsukuyomi,
                CardId.MagicianOfFaith,
                CardId.Sangan,
                CardId.MysticTomato,
                CardId.ExiledForce,
                CardId.SpiritReaper,
                CardId.Marshmallon,
                CardId.DDWarriorLady,
                CardId.BreakerTheMagicalWarrior,
                CardId.CyberDragon,
                CardId.ZaborgTheThunderMonarch,
                CardId.ChaosSorcerer
            };

            List<ClientCard> targets = shuffleOrder
                .SelectMany(id => monsters.Where(c => c.IsCode(id) && !keep.Contains(c)))
                .Concat(monsters.Where(c => !keep.Contains(c) && !shuffleOrder.Any(id => c.IsCode(id))))
                .Concat(keep)
                .Distinct()
                .ToList();
            AI.SelectCard(targets);
            return true;
        }

        private bool SnatchStealActivate()
        {
            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(c => c.IsFaceup() && !c.IsShouldNotBeTarget() &&
                    !c.IsShouldNotBeSpellTrapTarget() &&
                    !c.IsCode(CardId.SpiritReaper))
                .ToList();
            if (candidates.Count == 0) return false;

            ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
            if (!candidates.Contains(problematic)) problematic = null;
            ClientCard target = problematic ?? candidates
                .OrderByDescending(c => c.GetDefensePower())
                .First();

            int currentAttack = Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.IsAttack())
                .Sum(c => c.Attack);
            bool clearsForLethal = Enemy.GetMonsterCount() == 1 &&
                currentAttack + target.Attack >= Enemy.LifePoints;
            bool tributePlay = Bot.HasInHand(CardId.ZaborgTheThunderMonarch) &&
                Enemy.GetMonsterCount() >= 2;
            bool worthwhile = problematic != null ||
                target.GetDefensePower() >= Math.Max(1800, Util.GetBestAttack(Bot)) ||
                clearsForLethal ||
                tributePlay;
            if (!worthwhile) return false;

            AI.SelectCard(target);
            return true;
        }

        private List<ClientCard> GetReviveTargets()
        {
            int[] reviveOrder =
            {
                CardId.ChaosSorcerer,
                CardId.ZaborgTheThunderMonarch,
                CardId.CyberDragon,
                CardId.DDWarriorLady,
                CardId.ExiledForce,
                CardId.Sangan,
                CardId.MysticTomato,
                CardId.BreakerTheMagicalWarrior,
                CardId.SpiritReaper,
                CardId.Marshmallon,
                CardId.MagicianOfFaith
            };
            List<ClientCard> monsters = Bot.GetGraveyardMonsters()
                .Where(c => c.IsCanRevive() && !c.IsCode(CardId.Tsukuyomi))
                .ToList();
            return reviveOrder
                .SelectMany(id => monsters.Where(c => c.IsCode(id)))
                .Concat(monsters.Where(c => !reviveOrder.Any(id => c.IsCode(id))))
                .Distinct()
                .ToList();
        }

        private bool PrematureBurialActivate()
        {
            // 等于800时规则上可以自杀
            if (Bot.LifePoints <= 800) return false;

            List<ClientCard> targets = GetReviveTargets();
            if (targets.Count == 0) return false;

            ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
            ClientCard target = targets.First();
            if (problematic != null && target.Attack < 2000)
            {
                target = targets.FirstOrDefault(c =>
                    c.IsCode(CardId.ExiledForce, CardId.DDWarriorLady)) ?? target;
            }

            int currentAttack = Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.IsAttack())
                .Sum(c => c.Attack);
            bool clearsForLethal = Enemy.GetMonsterCount() == 0 &&
                currentAttack + target.Attack >= Enemy.LifePoints;
            bool tributePlay = target.IsCode(CardId.Sangan) &&
                Bot.HasInHand(CardId.ZaborgTheThunderMonarch) &&
                Bot.GetMonsterCount() == 0 &&
                Enemy.GetMonsterCount() > 0;
            bool worthwhile = target.Attack >= 2000 ||
                problematic != null &&
                    target.IsCode(CardId.ExiledForce, CardId.DDWarriorLady) ||
                clearsForLethal ||
                tributePlay;
            if (!worthwhile) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool CallOfTheHauntedActivate()
        {
            List<ClientCard> targets = GetReviveTargets();
            if (targets.Count == 0) return false;

            ClientCard target = targets.First();
            bool protectCall = Util.IsChainTarget(Card);
            bool stopBattle = Duel.Player == 1 &&
                Duel.Phase > DuelPhase.Main1 &&
                Duel.Phase < DuelPhase.Main2 &&
                ShouldStopAttack(Enemy.BattlingMonster);
            if (stopBattle)
                target = targets.OrderByDescending(c => c.Attack).First();
            bool reviveForOwnTurn = Duel.Player == 1 &&
                Duel.Phase == DuelPhase.End &&
                target.Attack >= 1800;
            bool useOnOwnTurn = Duel.Player == 0 &&
                target.Attack >= 2000 &&
                (Enemy.GetMonsterCount() == 0 || Util.IsAllEnemyBetter(true));
            if (!protectCall && !stopBattle && !reviveForOwnTurn && !useOnOwnTurn)
                return false;

            AI.SelectCard(target);
            return true;
        }

        private bool EnemyControllerActivate()
        {
            List<ClientCard> targets = Enemy.GetMonsters()
                .Where(c => c.IsFaceup() && !c.IsShouldNotBeTarget() &&
                    !c.IsShouldNotBeSpellTrapTarget())
                .ToList();
            if (targets.Count == 0) return false;

            List<ClientCard> switchable = targets
                .Where(c => !c.HasType(CardType.Link))
                .ToList();

            if (Util.IsChainTarget(Card) && switchable.Count > 0)
            {
                AI.SelectOption(0);
                AI.SelectCard(switchable.OrderByDescending(c => c.GetDefensePower()).First());
                return true;
            }

            ClientCard attacker = Enemy.BattlingMonster;
            if (Duel.Player == 1 && ShouldStopAttack(attacker) &&
                switchable.Contains(attacker))
            {
                AI.SelectOption(0);
                AI.SelectCard(attacker);
                return true;
            }

            if (ShouldUseSetQuickPlayForMagicianOfFaith())
            {
                ClientCard target = switchable
                    .Where(c => c.IsAttack())
                    .OrderByDescending(c => c.Attack)
                    .FirstOrDefault();
                if (target != null)
                {
                    AI.SelectOption(0);
                    AI.SelectCard(target);
                    return true;
                }
            }

            if (Duel.Player == 0)
            {
                ClientCard sangan = Bot.GetMonsters().FirstOrDefault(c => c.IsCode(CardId.Sangan));
                ClientCard steal = targets.OrderByDescending(c => c.Attack).FirstOrDefault();
                if (sangan != null && steal != null && steal.Attack >= 1800)
                {
                    AI.SelectOption(1);
                    AI.SelectCard(sangan);
                    AI.SelectNextCard(steal);
                    return true;
                }

                if (Duel.Phase == DuelPhase.Main1)
                {
                    int bestAttack = Util.GetBestAttack(Bot);
                    ClientCard battleTarget = switchable
                        .Where(c => c.IsAttack()
                            ? c.Attack >= bestAttack && c.Defense < bestAttack
                            : c.Defense >= bestAttack && c.Attack < bestAttack)
                        .OrderByDescending(c => c.GetDefensePower())
                        .FirstOrDefault();
                    if (battleTarget != null)
                    {
                        AI.SelectOption(0);
                        AI.SelectCard(battleTarget);
                        return true;
                    }
                }
            }
            return false;
        }

        // 对手会自行选择交出的怪兽，因此只按其最弱怪兽估算收益
        private bool CreatureSwapActivate()
        {
            ClientCard give = Bot.GetMonsters()
                .FirstOrDefault(c => c.IsCode(CardId.Sangan, CardId.MysticTomato))
                ?? Bot.GetMonsters()
                    .FirstOrDefault(c => c.IsFaceup() && c.IsCode(CardId.Tsukuyomi))
                ?? Bot.GetMonsters()
                    .Where(c => c.IsFaceup() && c.IsCode(CardId.MagicianOfFaith))
                    .FirstOrDefault()
                ?? Bot.GetMonsters()
                    .FirstOrDefault(c => c.IsFaceup() &&
                        (c.IsCode(CardId.ExiledForce) ||
                        c.IsCode(CardId.BreakerTheMagicalWarrior) && c.Attack <= 1600));
            ClientCard enemyWorst = Enemy.GetMonsters()
                .OrderBy(c => c.GetDefensePower())
                .FirstOrDefault();

            if (give == null || enemyWorst == null ||
                enemyWorst.GetDefensePower() <= give.GetDefensePower())
                return false;

            AI.SelectCard(give);
            return true;
        }

        // ===== 陷阱激活 =====

        private bool MirrorForceActivate()
        {
            ClientCard attacker = Enemy.BattlingMonster;
            if (attacker == null) return false;

            int attackPositionCount = Enemy.GetMonsters().Count(c => c.IsAttack());
            bool recruiterCanTrade = Bot.BattlingMonster != null &&
                Bot.BattlingMonster.IsCode(
                    CardId.Sangan,
                    CardId.MysticTomato,
                    CardId.Marshmallon,
                    CardId.SpiritReaper);
            bool lethalBattleDamage = Bot.BattlingMonster != null &&
                Bot.BattlingMonster.IsAttack() &&
                attacker.Attack - Bot.BattlingMonster.Attack >= Bot.LifePoints;
            bool worthwhile = attackPositionCount >= 2 ||
                attacker.IsMonsterDangerous() ||
                attacker.IsCode(CardId.SpiritReaper) ||
                lethalBattleDamage ||
                !recruiterCanTrade && (
                    attacker.Attack >= 1800 ||
                    Util.GetTotalAttackingMonsterAttack(1) >= Bot.LifePoints);
            return worthwhile && DefaultUniqueTrap();
        }

        // ===== 怪兽召唤条件 =====

        // 混沌巫师：选择不能复活或低价值的光、暗怪兽作为除外成本
        private bool ChaosSorcererSummon()
        {
            List<ClientCard> lights = Bot.Graveyard
                .Where(c => c.HasAttribute(CardAttribute.Light))
                .ToList();
            List<ClientCard> darks = Bot.Graveyard
                .Where(c => c.HasAttribute(CardAttribute.Dark))
                .ToList();

            int[] lightOrder =
            {
                CardId.MagicianOfFaith,
                CardId.Marshmallon,
                CardId.DDWarriorLady,
                CardId.CyberDragon,
                CardId.ZaborgTheThunderMonarch
            };
            int[] darkOrder =
            {
                CardId.Tsukuyomi,
                CardId.Sangan,
                CardId.MysticTomato,
                CardId.SpiritReaper,
                CardId.BreakerTheMagicalWarrior,
                CardId.ChaosSorcerer
            };

            ClientCard light = lightOrder
                .Select(id => lights.FirstOrDefault(c => c.IsCode(id)))
                .FirstOrDefault(c => c != null) ?? lights.OrderBy(c => c.Attack).First();
            ClientCard dark = darks
                .FirstOrDefault(c => c.IsCode(CardId.ChaosSorcerer) && !c.IsCanRevive())
                ?? darkOrder
                    .Select(id => darks.FirstOrDefault(c => c.IsCode(id)))
                    .FirstOrDefault(c => c != null)
                ?? darks.OrderBy(c => c.Attack).First();

            AI.SelectCard(new List<ClientCard> { light, dark });
            return true;
        }

        // 雷帝扎博尔格：优先解放能产生收益或已经失去价值的怪兽
        private bool ZaborgSummon()
        {
            ClientCard target = Enemy.GetMonsters()
                .Where(c => !c.IsShouldNotBeTarget() && !c.IsShouldNotBeMonsterTarget())
                .OrderByDescending(c => c.IsFaceup())
                .ThenByDescending(c => c.GetDefensePower())
                .FirstOrDefault();
            if (target == null) return false;

            ClientCard tribute = Bot.GetMonsters()
                .OrderBy(c =>
                {
                    if (c.IsCode(CardId.Sangan)) return 0;
                    if (c.IsCode(CardId.MagicianOfFaith))
                    {
                        if (c.IsFaceup()) return 1;
                        return Bot.Graveyard.Any(card => card.IsSpell()) ? 7 : 4;
                    }
                    if (c.IsCode(CardId.BreakerTheMagicalWarrior) && c.Attack <= 1600) return 2;
                    if (c.IsCode(CardId.ExiledForce)) return 3;
                    if (c.IsCode(CardId.MysticTomato)) return 5;
                    if (c.IsCode(CardId.Marshmallon, CardId.SpiritReaper)) return 7;
                    if (c.Attack <= 1400) return 4;
                    if (c.IsCode(CardId.DDWarriorLady)) return 6;
                    if (c.IsCode(CardId.CyberDragon)) return 8;
                    if (c.IsCode(CardId.ChaosSorcerer)) return 9;
                    return 7;
                })
                .ThenBy(c => c.GetDefensePower())
                .FirstOrDefault();
            if (tribute == null) return false;

            bool worthwhile = tribute.IsCode(CardId.Sangan) ||
                tribute.IsCode(CardId.ExiledForce) ||
                tribute.IsCode(CardId.MagicianOfFaith) && tribute.IsFaceup() ||
                tribute.IsCode(CardId.BreakerTheMagicalWarrior) && tribute.Attack <= 1600 ||
                target.IsMonsterDangerous() ||
                target.GetDefensePower() >= Math.Max(1800, tribute.GetDefensePower());
            if (!worthwhile) return false;

            AI.SelectCard(tribute);
            return true;
        }

        // 流氓佣兵部队：有可取对象且对方怪兽压场时召唤
        private bool ExiledForceSummon()
        {
            ClientCard target = Enemy.GetMonsters()
                .FirstOrDefault(c => !c.IsShouldNotBeTarget() &&
                    !c.IsShouldNotBeMonsterTarget() &&
                    (c.IsMonsterDangerous() || c.IsMonsterInvincible()));
            return target != null ||
                (Enemy.GetMonsters().Any(c => !c.IsShouldNotBeTarget() &&
                    !c.IsShouldNotBeMonsterTarget()) && Util.IsOneEnemyBetter());
        }

        private bool TsukuyomiSummon()
        {
            return GetTsukuyomiTarget() != null;
        }

        // 月读命：重置圣魔术师，或把难以战斗处理的怪兽变成里侧守备表示
        private ClientCard GetTsukuyomiTarget(IEnumerable<ClientCard> cards = null)
        {
            IEnumerable<ClientCard> candidates = cards != null
                ? cards.AsEnumerable()
                : Bot.GetMonsters().Concat(Enemy.GetMonsters());
            ClientCard magician = candidates
                .FirstOrDefault(c => c.Controller == 0 &&
                    c.IsCode(CardId.MagicianOfFaith) && c.IsFaceup());
            if (magician != null && Bot.Graveyard.Any(c => c.IsSpell()))
                return magician;

            List<ClientCard> enemyCandidates = candidates
                .Where(c => c.Controller == 1 && c.IsFaceup() &&
                    !c.HasType(CardType.Flip) &&
                    !c.HasType(CardType.Link | CardType.Token) &&
                    !c.IsShouldNotBeTarget() && !c.IsShouldNotBeMonsterTarget())
                .ToList();
            ClientCard problematic = enemyCandidates
                .FirstOrDefault(c => c.IsFloodgate() || c.IsMonsterDangerous());
            if (problematic != null)
                return problematic;

            ClientCard tsukuyomiBattleTarget = enemyCandidates
                .Where(c => c.Defense < 1100)
                .OrderByDescending(c => c.Attack)
                .FirstOrDefault();
            if (tsukuyomiBattleTarget != null)
                return tsukuyomiBattleTarget;

            int bestAttack = Math.Max(1100, Util.GetBestAttack(Bot));
            return enemyCandidates
                .Where(c => c.Defense < bestAttack)
                .OrderByDescending(c => c.Attack)
                .FirstOrDefault();
        }

        private bool SpiritReaperSummon()
        {
            return Enemy.GetMonsterCount() == 0 &&
                !Util.IsTurn1OrMain2();
        }

        // ===== 怪兽效果 =====

        // 混沌巫师效果：除外1只表侧怪兽；发动效果的回合自身不能攻击
        private bool ChaosSorcererActivate()
        {
            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(c => c.IsFaceup() && !c.IsShouldNotBeTarget() &&
                    !c.IsShouldNotBeMonsterTarget())
                .ToList();
            ClientCard problematic = Util.GetProblematicEnemyMonster(0, true);
            if (!candidates.Contains(problematic)) problematic = null;
            ClientCard target = problematic ??
                candidates.OrderByDescending(c => c.GetDefensePower()).FirstOrDefault();

            if (target == null) return false;

            bool shouldBanish = problematic != null ||
                target.IsMonsterDangerous() ||
                target.IsMonsterInvincible() ||
                target.GetDefensePower() >= Card.Attack ||
                Duel.Phase == DuelPhase.Main2;
            if (!shouldBanish) return false;

            AI.SelectCard(target);
            return true;
        }

        // 魔导战士破坏者效果：消耗魔力计数器破坏对方最优魔法/陷阱
        private bool BreakerActivate()
        {
            if (Duel.LastSummonedCards.Contains(Card) && Card.Attack <= 1600)
                return true;

            ClientCard target = Util.GetBestEnemySpell(true);
            if (target != null && (target.IsShouldNotBeTarget() ||
                target.IsShouldNotBeMonsterTarget()))
                target = null;
            if (target == null)
                target = Enemy.GetSpells()
                    .FirstOrDefault(c => c.IsFacedown() && !c.IsShouldNotBeTarget() &&
                        !c.IsShouldNotBeMonsterTarget());
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool ExiledForceActivate()
        {
            List<ClientCard> candidates = Enemy.GetMonsters()
                .Where(c => !c.IsShouldNotBeTarget() && !c.IsShouldNotBeMonsterTarget())
                .ToList();
            if (candidates.Count == 0) return false;

            ClientCard target = candidates
                .OrderByDescending(c => c.IsMonsterDangerous())
                .ThenByDescending(c => c.IsMonsterInvincible())
                .ThenByDescending(c => c.IsFaceup())
                .ThenByDescending(c => c.GetDefensePower())
                .First();
            int bestAttack = Math.Max(Card.Attack, Util.GetBestAttack(Bot));
            bool worthwhile = target.IsMonsterDangerous() ||
                target.IsMonsterInvincible() ||
                target.GetDefensePower() >= bestAttack;
            if (!worthwhile) return false;

            AI.SelectCard(target);
            return true;
        }

        private bool MagicianOfFaithSet()
        {
            return Bot.Graveyard.Any(c => c.IsSpell()) ||
                Bot.GetSpells().Any(c => c.IsFacedown() && c.HasType(CardType.QuickPlay));
        }

        // 杀人番茄被战斗破坏效果：从牌组特殊召唤暗属性怪兽（攻击力≤1500）
        private bool MysticTomatoActivate()
        {
            // 月读命不能特殊召唤，不是杀人番茄的合法选择
            AI.SelectCard(
                CardId.Sangan,
                CardId.SpiritReaper,
                CardId.MysticTomato
            );
            return true;
        }

        // 异次元的女战士：伤害计算后除外自身与战斗对象
        private bool DDWarriorLadyActivate()
        {
            ClientCard battlingEnemy = Enemy.BattlingMonster;
            if (battlingEnemy == null) return false;

            return battlingEnemy.IsMonsterDangerous() ||
                battlingEnemy.IsMonsterInvincible() ||
                battlingEnemy.GetDefensePower() >= Card.Attack;
        }
    }
}
