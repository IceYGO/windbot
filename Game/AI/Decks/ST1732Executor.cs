using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ST1732", "AI_ST1732")]
    public class ST1732Executor : DefaultExecutor
    {
        public enum CardId
        {
            数字机灵 = 32295838,
            比特机灵 = 36211150,
            双汇编亚龙 = 7445307,
            引导交错鹿 = 70950698,
            猞猁连接杀手 = 35595518,
            RAM云雄羊 = 9190563,
            ROM云雌羊 = 44956694,
            均衡负载王 = 8567955,
            反向连接兽 = 71172240,
            克莱因客户端蚁 = 45778242,
            网络小龙 = 62706865,
            点阵图跳离士 = 18789533,
            精神操作 = 37520316,
            黑洞 = 53129443,
            死者苏生 = 83764718,
            旋风 = 5318639,
            宇宙旋风 = 8267140,
            月之书 = 14087893,
            电脑网后门 = 43839002,
            月镜盾 = 19508728,
            电脑网宇宙 = 61583217,
            奈落的落穴 = 29401950,
            神圣防护罩反射镜力 = 44095762,
            激流葬 = 53582587,
            重编码存活 = 70238111,
            次元障壁 = 83326048,
            强制脱出装置 = 94192409,
            神之通告 = 40605147,

            解码语者 = 1861629,
            编码语者 = 6622715,
            三栅极男巫 = 32617464,
            蜜罐机器人 = 34472920,
            二进制女巫 = 79016563,
            连接蜘蛛 = 98978921,

            引导鹿衍生物 = 70950699
        }

        bool 已发动均衡负载王 = false;

        public ST1732Executor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.宇宙旋风, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.旋风, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.月之书, DefaultBookOfMoon);

            AddExecutor(ExecutorType.Activate, (int)CardId.电脑网宇宙, 电脑网宇宙效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.猞猁连接杀手);
            AddExecutor(ExecutorType.Activate, (int)CardId.猞猁连接杀手, 猞猁连接杀手效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.连接蜘蛛);
            AddExecutor(ExecutorType.Activate, (int)CardId.连接蜘蛛);

            AddExecutor(ExecutorType.Activate, (int)CardId.精神操作, 精神操作效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.反向连接兽);
            AddExecutor(ExecutorType.Activate, (int)CardId.反向连接兽, 反向连接兽效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.引导交错鹿, 引导交错鹿效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.死者苏生, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.月镜盾, 月镜盾效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.电脑网后门, 电脑网后门效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.重编码存活);

            AddExecutor(ExecutorType.Summon, (int)CardId.均衡负载王, 均衡负载王通常召唤);

            AddExecutor(ExecutorType.Summon, (int)CardId.ROM云雌羊, ROM云雌羊通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.ROM云雌羊, ROM云雌羊效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.网络小龙, 网络小龙通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.网络小龙, 网络小龙效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.克莱因客户端蚁);
            AddExecutor(ExecutorType.Activate, (int)CardId.克莱因客户端蚁, 克莱因客户端蚁效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.RAM云雄羊);
            AddExecutor(ExecutorType.Activate, (int)CardId.RAM云雄羊, RAM云雄羊效果);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.点阵图跳离士);
            AddExecutor(ExecutorType.Activate, (int)CardId.点阵图跳离士, 点阵图跳离士效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.均衡负载王);
            AddExecutor(ExecutorType.Summon, (int)CardId.ROM云雌羊);
            AddExecutor(ExecutorType.Summon, (int)CardId.网络小龙);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.反向连接兽);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.数字机灵);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.比特机灵);

            AddExecutor(ExecutorType.Activate, (int)CardId.均衡负载王, 均衡负载王效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.解码语者, 连接召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.解码语者);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.三栅极男巫, 连接召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.三栅极男巫);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.编码语者, 连接召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.编码语者);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.蜜罐机器人, 连接召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.二进制女巫, 连接召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.二进制女巫);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.电脑网后门, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.重编码存活, DefaultSpellSet);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强制脱出装置, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.次元障壁, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.激流葬, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神圣防护罩反射镜力, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.奈落的落穴, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.月之书, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.宇宙旋风, DefaultSpellSet);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.旋风, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.神之通告, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, (int)CardId.强制脱出装置, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, (int)CardId.次元障壁, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, (int)CardId.激流葬, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩反射镜力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.奈落的落穴, DefaultUniqueTrap);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // 抢后攻
            return false;
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已发动均衡负载王 = false;
        }

        public override int OnSelectOption(IList<int> options)
        {
            // 月镜盾回卡组底
            return options.Count == 2 ? 1 : 0;
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == 210) //是否要继续选择？
                return false;
            if (desc == 31) //是否直接攻击？
                return true;
            return base.OnSelectYesNo(desc);
        }

        private bool 猞猁连接杀手效果()
        {
            IList<ClientCard> targets = Enemy.GetSpells();
            if (targets.Count > 0)
            {
                AI.SelectCard(new[]{
                    (int)CardId.双汇编亚龙,
                    (int)CardId.比特机灵,
                    (int)CardId.数字机灵,
                    (int)CardId.重编码存活
                });
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }

        private bool 精神操作效果()
        {
            ClientCard target = AI.Utils.GetAnyEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool 反向连接兽效果()
        {
            return (Bot.MonsterZone[5] == null) && (Bot.MonsterZone[6] == null);
        }

        private bool 引导交错鹿效果()
        {
            if (Card.Location != CardLocation.Hand)
                AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 死者苏生效果()
        {
            List<int> targets = new List<int> {
                    (int)CardId.解码语者,
                    (int)CardId.编码语者,
                    (int)CardId.三栅极男巫,
                    (int)CardId.二进制女巫,
                    (int)CardId.蜜罐机器人,
                    (int)CardId.双汇编亚龙,
                    (int)CardId.引导交错鹿,
                    (int)CardId.均衡负载王,
                    (int)CardId.ROM云雌羊,
                    (int)CardId.猞猁连接杀手,
                    (int)CardId.RAM云雄羊,
                    (int)CardId.反向连接兽,
                    (int)CardId.克莱因客户端蚁
                };
            if (!Bot.HasInGraveyard(targets))
            {
                return false;
            }
            AI.SelectCard(targets);
            return true;
        }

        private bool 月镜盾效果()
        {
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            return false;
        }

        private bool 电脑网宇宙效果()
        {
            if (Card.Location == CardLocation.Hand)
                return DefaultField();
            IList<ClientCard> cards = Enemy.Graveyard;
            foreach (ClientCard card in cards)
            {
                if (card.IsMonster())
                {
                    AI.SelectCard(card);
                    return true;
                }
            }
            return false;
        }

        private bool 电脑网后门效果()
        {
            if (!(Duel.Player == 0 && Duel.Phase == DuelPhase.Main2) &&
                !(Duel.Player == 1 && (Duel.Phase == DuelPhase.Battle || Duel.Phase == DuelPhase.End)))
            {
                return false;
            }
            foreach (ClientCard card in Bot.SpellZone)
            {
                if (card != null &&
                    card.Id == Card.Id &&
                    card.HasPosition(CardPosition.FaceUp))
                    return false;
            }
            bool selected = false;
            List<ClientCard> monsters = Bot.GetMonstersInExtraZone();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Attack > 1000)
                {
                    AI.SelectCard(monster);
                    selected = true;
                    break;
                }
            }
            if (!selected)
            {
                monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.均衡负载王)
                    {
                        AI.SelectCard(monster);
                        selected = true;
                        break;
                    }
                }
                if (!selected)
                {
                    foreach (ClientCard monster in monsters)
                    {
                        if (monster.Attack >= 1700)
                        {
                            AI.SelectCard(monster);
                            selected = true;
                            break;
                        }
                    }
                }
            }
            if (selected)
            {
                AI.SelectNextCard(new[]
                {
                    (int)CardId.ROM云雌羊,
                    (int)CardId.均衡负载王,
                    (int)CardId.克莱因客户端蚁,
                    (int)CardId.网络小龙,
                    (int)CardId.反向连接兽
                });
                return true;
            }
            return false;
        }

        private bool 均衡负载王通常召唤()
        {
            return !已发动均衡负载王;
        }

        private bool 均衡负载王效果()
        {
            if (Card.Location == CardLocation.Removed)
                return true;
            bool hastarget = Bot.HasInHand(new List<int> {
                    (int)CardId.网络小龙,
                    (int)CardId.克莱因客户端蚁,
                    (int)CardId.均衡负载王,
                    (int)CardId.ROM云雌羊,
                    (int)CardId.RAM云雄羊,
                    (int)CardId.点阵图跳离士
                });
            if (hastarget && !已发动均衡负载王)
            {
                已发动均衡负载王 = true;
                return true;
            }
            return false;
        }

        private bool ROM云雌羊通常召唤()
        {
            return Bot.HasInGraveyard(new List<int> {
                    (int)CardId.引导交错鹿,
                    (int)CardId.均衡负载王,
                    (int)CardId.克莱因客户端蚁,
                    (int)CardId.猞猁连接杀手,
                    (int)CardId.网络小龙,
                    (int)CardId.RAM云雄羊
                });
        }

        private bool ROM云雌羊效果()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(new[]{
                    (int)CardId.引导交错鹿,
                    (int)CardId.均衡负载王,
                    (int)CardId.克莱因客户端蚁,
                    (int)CardId.猞猁连接杀手,
                    (int)CardId.网络小龙,
                    (int)CardId.RAM云雄羊
                });
                return true;
            }
            else
            {
                AI.SelectCard(new[]{
                    (int)CardId.均衡负载王,
                    (int)CardId.克莱因客户端蚁,
                    (int)CardId.RAM云雄羊,
                    (int)CardId.点阵图跳离士
                });
                return true;
            }
        }

        private bool 网络小龙通常召唤()
        {
            return Bot.GetRemainingCount((int)CardId.数字机灵, 1) > 0
                || Bot.GetRemainingCount((int)CardId.比特机灵, 1) > 0;
        }

        private bool 网络小龙效果()
        {
            AI.SelectCard((int)CardId.比特机灵);
            return true;
        }

        private bool 克莱因客户端蚁效果()
        {
            IList<int> targets = new[] {
                (int)CardId.双汇编亚龙,
                (int)CardId.比特机灵,
                (int)CardId.数字机灵,
                (int)CardId.点阵图跳离士
            };
            foreach (ClientCard monster in Bot.Hand)
            {
                if (targets.Contains(monster.Id))
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            IList<int> targets2 = new[] {
                (int)CardId.引导鹿衍生物,
                (int)CardId.比特机灵,
                (int)CardId.数字机灵,
                (int)CardId.点阵图跳离士
            };
            foreach (ClientCard monster in Bot.GetMonsters())
            {
                if (targets2.Contains(monster.Id))
                {
                    AI.SelectCard(targets2);
                    return true;
                }
            }
            return false;
        }

        private bool RAM云雄羊效果()
        {
            AI.SelectCard(new[]{
                    (int)CardId.引导鹿衍生物,
                    (int)CardId.比特机灵,
                    (int)CardId.数字机灵,
                    (int)CardId.点阵图跳离士,
                    (int)CardId.网络小龙,
                    (int)CardId.反向连接兽,
                    (int)CardId.RAM云雄羊
                });
            AI.SelectNextCard(new[]{
                    (int)CardId.解码语者,
                    (int)CardId.编码语者,
                    (int)CardId.三栅极男巫,
                    (int)CardId.二进制女巫,
                    (int)CardId.蜜罐机器人,
                    (int)CardId.双汇编亚龙,
                    (int)CardId.引导交错鹿,
                    (int)CardId.均衡负载王,
                    (int)CardId.ROM云雌羊,
                    (int)CardId.猞猁连接杀手,
                    (int)CardId.RAM云雄羊
                });
            return true;
        }

        private bool 点阵图跳离士效果()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 连接召唤()
        {
            return (AI.Utils.IsTurn1OrMain2() || AI.Utils.IsEnemyBetter(false, false))
                && AI.Utils.GetBestAttack(Bot, true) < Card.Attack;
        }
    }
}