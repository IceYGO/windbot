using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace DevBot.Game.AI.Decks
{
    [Deck("Rank V", "AI_Rank5")]
    public class Rank5Executor : DefaultExecutor
    {
        public enum CardId
        {
            迷雾恶魔 = 28601770,
            电子龙 = 70095154,
            异热同心武器荒鹫激神爪 = 29353756,
            太阳风帆船 = 33911264,
            速攻同调士 = 20932152,
            发条士兵 = 12299841,
            画星宝宝 = 24610207,
            先史遗产黄金航天飞机 = 88552992,
            简易融合 = 1845204,
            二重召唤 = 43422537,
            旋风 = 5318639,
            月之书 = 14087893,
            超量组件 = 13032689,
            超量苏生 = 26708437,
            神圣防护罩反射镜力 = 44095762,
            激流葬 = 53582587,
            超量遮护罩 = 96457619,

            重装机甲装甲车龙 = 72959823,
            迅雷之骑士盖亚龙骑士 = 91949988,
            电子龙无限 = 10443957,
            始祖守护者提拉斯 = 31386180,
            No61火山恐龙 = 29669359,
            鲨鱼要塞 = 50449881,
            电子龙新星 = 58069384
        }

        private bool 已通常召唤 = false;
        private bool 已发动简易融合 = false;
        private bool 已发动二重召唤 = false;
        private bool 已特殊召唤电子龙无限 = false;
        private bool 已发动火山恐龙 = false;

        public Rank5Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.月之书, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.旋风, DefaultMysticalSpaceTyphoon);

            // 优先出的超量怪兽
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子龙新星, 电子龙新星特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子龙新星, 电子龙新星效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子龙无限, 电子龙无限特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子龙无限, 电子龙无限效果);

            // 无副作用的5星怪兽
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子龙);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.太阳风帆船, 太阳风帆船特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.异热同心武器荒鹫激神爪);
            AddExecutor(ExecutorType.Summon, (int)CardId.先史遗产黄金航天飞机, 通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.先史遗产黄金航天飞机, 先史遗产黄金航天飞机效果);
            AddExecutor(ExecutorType.Summon, (int)CardId.画星宝宝, 通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.发条士兵, 通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.发条士兵, 发条士兵效果);

            // XYZ Monsters: Summon
            AddExecutor(ExecutorType.SpSummon, (int)CardId.No61火山恐龙, No61火山恐龙特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.No61火山恐龙, No61火山恐龙效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.始祖守护者提拉斯);
            AddExecutor(ExecutorType.Activate, (int)CardId.始祖守护者提拉斯, 始祖守护者提拉斯效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.鲨鱼要塞);
            AddExecutor(ExecutorType.Activate, (int)CardId.鲨鱼要塞);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.迅雷之骑士盖亚龙骑士, 迅雷之骑士盖亚龙骑士特殊召唤);


            // 有副作用的5星怪兽
            AddExecutor(ExecutorType.SpSummon, (int)CardId.速攻同调士, 速攻同调士特殊召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.迷雾恶魔, 迷雾恶魔通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.简易融合, 简易融合效果);

            // Useful spells
            AddExecutor(ExecutorType.Activate, (int)CardId.二重召唤, 二重召唤效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.超量组件);


            AddExecutor(ExecutorType.Activate, (int)CardId.超量苏生, 超量苏生效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.重装机甲装甲车龙, 重装机甲装甲车龙效果);

            // Reposition
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.超量遮护罩, 超量遮护罩效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.激流葬, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩反射镜力, DefaultTrap);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已通常召唤 = false;
            已发动简易融合 = false;
            已发动二重召唤 = false;
            已特殊召唤电子龙无限 = false;
            已发动火山恐龙 = false;
        }

        private bool 特殊召唤不重复的超量怪兽()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
                if (monster.Id == Card.Id)
                    return false;
            return true;
        }

        private bool 通常召唤()
        {
            已通常召唤 = true;
            return true;
        }

        private bool 太阳风帆船特殊召唤()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 速攻同调士特殊召唤()
        {
            if (!需要出5星())
                return false;
            AI.SelectCard(new[]
                {
                    (int)CardId.速攻同调士,
                    (int)CardId.异热同心武器荒鹫激神爪,
                    (int)CardId.太阳风帆船,
                    (int)CardId.电子龙,
                    (int)CardId.迷雾恶魔,
                    (int)CardId.发条士兵,
                    (int)CardId.画星宝宝,
                    (int)CardId.先史遗产黄金航天飞机
                });
            return true;
        }

        private bool 迷雾恶魔通常召唤()
        {
            if (!需要出5星())
                return false;
            AI.SelectOption(1);
            已通常召唤 = true;
            return true;
        }

        private bool 简易融合效果()
        {
            if (!需要出5星())
                return false;
            已发动简易融合 = true;
            return true;
        }

        private bool 需要出5星()
        {
            if (场上有5星怪兽())
                return true;
            int 其他的5星资源数量 = 0;
            IList<ClientCard> hand = Duel.Fields[0].Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.简易融合 && !已发动简易融合)
                    ++其他的5星资源数量;
                if (card.Id == (int)CardId.速攻同调士 && Duel.Fields[0].Hand.ContainsMonsterWithLevel(4))
                    ++其他的5星资源数量;
                if (card.Id == (int)CardId.迷雾恶魔 && !已通常召唤)
                    ++其他的5星资源数量;
                if (card.Id == (int)CardId.二重召唤 && 二重召唤效果())
                    ++其他的5星资源数量;
            }
            if (其他的5星资源数量 >= 2)
                return true;
            return false;
        }

        private bool 发条士兵效果()
        {
            return 场上有5星怪兽();
        }

        private bool 先史遗产黄金航天飞机效果()
        {
            return Card.Level == 4;
        }

        private bool 二重召唤效果()
        {
            if (!已通常召唤 || 已发动二重召唤)
                return false;
            IList<ClientCard> hand = Duel.Fields[0].Hand;
            foreach (ClientCard card in hand)
            {
                if (card.Id == (int)CardId.迷雾恶魔 ||
                    card.Id == (int)CardId.发条士兵 ||
                    card.Id == (int)CardId.画星宝宝 ||
                    card.Id == (int)CardId.先史遗产黄金航天飞机)
                {
                    已通常召唤 = false;
                    已发动二重召唤 = true;
                    return true;
                }
            }
            return false;
        }

        private bool 电子龙新星特殊召唤()
        {
            return !已特殊召唤电子龙无限;
        }

        private bool 电子龙新星效果()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.电子龙新星, 0))
            {
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool 电子龙无限特殊召唤()
        {
            已特殊召唤电子龙无限 = true;
            return true;
        }

        private bool 电子龙无限效果()
        {
            if (CurrentChain.Count > 0)
            {
                return LastChainPlayer == 1;
            }
            else
            {
                List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
                ClientCard bestmonster = null;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsAttack() && (bestmonster == null || monster.Attack >= bestmonster.Attack))
                        bestmonster = monster;
                }
                if (bestmonster != null)
                {
                    AI.SelectCard(bestmonster);
                    return true;
                }
            }
            return false;
        }

        private bool No61火山恐龙特殊召唤()
        {
            return AI.Utils.IsOneEnnemyBetterThanValue(2000, false);
        }

        private bool No61火山恐龙效果()
        {
            ClientCard target = Duel.Fields[1].MonsterZone.GetFloodgate();
            if (target == null)
                target = AI.Utils.GetOneEnnemyBetterThanValue(2000, false);
            if (target != null)
            {
                AI.SelectCard((int)CardId.电子龙);
                AI.SelectNextCard(target);
                已发动火山恐龙 = true;
                return true;
            }
            return false;
        }

        private bool 始祖守护者提拉斯效果()
        {
            ClientCard target = AI.Utils.GetProblematicCard();
            if (target != null)
            {
                AI.SelectCard(target);
            }
            return true;
        }

        private bool 迅雷之骑士盖亚龙骑士特殊召唤()
        {
            if (已发动火山恐龙 && Duel.Fields[0].HasInMonstersZone((int)CardId.No61火山恐龙))
            {
                AI.SelectCard((int)CardId.No61火山恐龙);
                return true;
            }
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz) && !monster.HasXyzMaterial())
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            return false;
        }

        private bool 超量苏生效果()
        {
            foreach (ClientCard card in Duel.Fields[0].SpellZone)
            {
                if (card != null &&
                    card.Id == Card.Id &&
                    card.HasPosition(CardPosition.FaceUp))
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.电子龙无限,
                    (int)CardId.电子龙新星,
                    (int)CardId.始祖守护者提拉斯,
                    (int)CardId.鲨鱼要塞,
                    (int)CardId.No61火山恐龙
                });
            return true;
        }

        private bool 重装机甲装甲车龙效果()
        {
            ClientCard target = AI.Utils.GetProblematicCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            foreach (ClientCard spell in spells)
            {
                AI.SelectCard(spell);
                return true;
            }
            return false;
        }

        private bool 超量遮护罩效果()
        {
            List<ClientCard> spells = Duel.Fields[0].GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == (int)CardId.超量遮护罩 && !spell.IsFacedown())
                    return false;
            }
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                    return true;
            }
            return false;
        }

        private bool 场上有5星怪兽()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Monster) &&
                    !monster.HasType(CardType.Xyz) &&
                    (monster.Level == 5
                    || monster.Id == (int)CardId.画星宝宝
                    || (monster.Id == (int)CardId.发条士兵) && !monster.Equals(Card)))
                    return true;
            }
            return false;
        }

        private ClientCard GetBestEnnemyCard()
        {
            ClientCard card = AI.Utils.GetProblematicCard();
            if (card != null)
                return card;
            card = Duel.Fields[1].MonsterZone.GetHighestAttackMonster();
            if (card != null)
                return card;
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            if (spells.Count > 0)
                return spells[0];
            return null;
        }
    }
}
