using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Qliphort", "AI_Qliphort")]
    public class QliphortExecutor : DefaultExecutor
    {
        public enum CardId
        {
            机壳工具丑恶 = 65518099,
            机壳别名愚钝 = 13073850,
            机壳壳层拒绝 = 90885155,
            机壳基因组贪欲 = 37991342,
            机壳档案色欲 = 91907707,
            黑洞 = 53129443,
            削命的宝札 = 59750328,
            召唤师的技艺 = 79816536,
            强欲而谦虚之壶 = 98645731,
            机壳的牲祭 = 17639150,
            神圣防护罩反射镜力 = 44095762,
            激流葬 = 53582587,
            次元障壁 = 83326048,
            强制脱出装置 = 94192409,
            虚无空间 = 5851097,
            技能抽取 = 82732705,
            神之通告 = 40605147,
            反大革命 = 99188141
        }

        bool 已发动削命 = false;

        List<int> 低刻度 = new List<int>
        {
            (int)CardId.机壳别名愚钝,
            (int)CardId.机壳档案色欲
        };
        List<int> 高刻度 = new List<int>
        {
            (int)CardId.机壳工具丑恶,
            (int)CardId.机壳壳层拒绝,
            (int)CardId.机壳基因组贪欲
        };

        public QliphortExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {

            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.召唤师的技艺);

            AddExecutor(ExecutorType.Activate, (int)CardId.机壳工具丑恶, 机壳工具丑恶发动);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳工具丑恶, 机壳工具丑恶效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.机壳别名愚钝, 设置刻度);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳壳层拒绝, 设置刻度);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳基因组贪欲, 设置刻度);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳档案色欲, 设置刻度);

            AddExecutor(ExecutorType.Summon, 通常召唤);
            AddExecutor(ExecutorType.SpSummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.机壳的牲祭, 机壳的牲祭);

            AddExecutor(ExecutorType.Activate, (int)CardId.机壳别名愚钝, 机壳别名愚钝效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳基因组贪欲, 机壳基因组贪欲效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.机壳档案色欲, 机壳档案色欲效果);

            // 盖坑
            AddExecutor(ExecutorType.SpellSet, (int)CardId.技能抽取, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.次元障壁, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.激流葬, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神圣防护罩反射镜力, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强制脱出装置, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.反大革命, 优先盖不重复的坑);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.机壳的牲祭, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.技能抽取, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.次元障壁, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.激流葬, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神圣防护罩反射镜力, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强制脱出装置, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.反大革命, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.黑洞, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.召唤师的技艺, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强欲而谦虚之壶, 魔陷区有空余格子);

            // 开完削命继续盖坑
            AddExecutor(ExecutorType.Activate, (int)CardId.强欲而谦虚之壶, 强欲而谦虚之壶);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.削命的宝札);
            AddExecutor(ExecutorType.Activate, (int)CardId.削命的宝札, 削命的宝札);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.机壳的牲祭, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.技能抽取, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.次元障壁, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.激流葬, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神圣防护罩反射镜力, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强制脱出装置, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.反大革命, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.黑洞, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.召唤师的技艺, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强欲而谦虚之壶, 已发动过削命);

            // 坑人
            AddExecutor(ExecutorType.Activate, (int)CardId.反大革命, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之通告, 神之通告);
            AddExecutor(ExecutorType.Activate, (int)CardId.技能抽取, 技能抽取);
            AddExecutor(ExecutorType.Activate, (int)CardId.虚无空间, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.强制脱出装置, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, (int)CardId.次元障壁, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩反射镜力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.激流葬, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // 抢先攻
            return true;
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已发动削命 = false;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            if (max <= min)
            {
                return null;
            }
            IList<ClientCard> selected = new List<ClientCard>();

            // select the last cards
            for (int i = 1; i <= max; ++i)
                selected.Add(cards[cards.Count - i]);

            return selected;
        }

        private bool 通常召唤()
        {
            if (Card.Id == (int)CardId.机壳工具丑恶)
                return false;
            if (Card.Level < 8)
                AI.SelectOption(1);
            return true;
        }

        private bool 神之通告()
        {
            return (Duel.LifePoints[0] > 1500) && !(Duel.Player == 0 && LastChainPlayer == -1) && DefaultTrap();
        }

        private bool 技能抽取()
        {
            return (Duel.LifePoints[0] > 1000) && DefaultUniqueTrap();
        }

        private bool 强欲而谦虚之壶()
        {
            AI.SelectCard(new[]
                    {
                    (int)CardId.机壳工具丑恶,
                    (int)CardId.技能抽取,
                    (int)CardId.虚无空间,
                    (int)CardId.次元障壁,
                    (int)CardId.机壳别名愚钝,
                    (int)CardId.机壳壳层拒绝,
                    (int)CardId.机壳基因组贪欲,
                    (int)CardId.机壳档案色欲,
                    (int)CardId.神之通告,
                    (int)CardId.削命的宝札
                });
            return !应进行灵摆召唤();
        }

        private bool 削命的宝札()
        {
            if (AI.Utils.IsTurn1OrMain2() && !应进行灵摆召唤())
            {
                已发动削命 = true;
                return true;
            }
            return false;
        }
        
        private bool 优先盖不重复的坑()
        {
            foreach (ClientCard card in Duel.Fields[0].SpellZone)
            {
                if (card != null && card.Id == Card.Id)
                    return false;
            }
            return 魔陷区有空余格子();
        }

        private bool 魔陷区有空余格子()
        {
            return Duel.Fields[0].GetSpellCountWithoutField() < 4;
        }

        private bool 已发动过削命()
        {
            return 已发动削命;
        }

        private bool 机壳的牲祭()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard l = AI.Utils.GetPZone(0, 0);
                ClientCard r = AI.Utils.GetPZone(0, 1);
                if (l == null && r == null)
                    AI.SelectCard((int)CardId.机壳工具丑恶);
            }
            return true;
        }

        private bool 机壳工具丑恶发动()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
            if (l == null && r == null)
                return true;
            if (l == null && r.RScale != Card.LScale)
                return true;
            if (r == null && l.LScale != Card.RScale)
                return true;
            return false;
        }

        private bool 设置刻度()
        {
            if (!Card.HasType(CardType.Pendulum) || Card.Location != CardLocation.Hand)
                return false;
            int count = 0;
            foreach (ClientCard card in Duel.Fields[0].Hand.GetMonsters())
            {
                if (!Card.Equals(card))
                    count++;
            }
            foreach (ClientCard card in Duel.Fields[0].ExtraDeck.GetMonsters())
            {
                if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                    count++;
            }
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
            if (l == null && r == null)
            {
                if (已发动削命)
                    return true;
                bool pair = false;
                foreach (ClientCard card in Duel.Fields[0].Hand.GetMonsters())
                {
                    if (card.RScale != Card.LScale)
                    {
                        pair = true;
                        count--;
                        break;
                    }
                }
                return pair && count>1;
            }
            if (l == null && r.RScale != Card.LScale)
                return count > 1 || 已发动削命;
            if (r == null && l.LScale != Card.RScale)
                return count > 1 || 已发动削命;
            return false;
        }

        private bool 机壳工具丑恶效果()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            int count = 0;
            int handcount = 0;
            int fieldcount = 0;
            foreach (ClientCard card in Duel.Fields[0].Hand.GetMonsters())
            {
                count++;
                handcount++;
            }
            foreach (ClientCard card in Duel.Fields[0].MonsterZone.GetMonsters())
            {
                fieldcount++;
            }
            foreach (ClientCard card in Duel.Fields[0].ExtraDeck.GetMonsters())
            {
                if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                    count++;
            }
            if (count>0 && !Duel.Fields[0].HasInHand(低刻度))
            {
                AI.SelectCard(低刻度);
            }
            else if (handcount>0 || fieldcount>0)
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.机壳的牲祭,
                    (int)CardId.机壳壳层拒绝,
                    (int)CardId.机壳基因组贪欲
                });
            }
            else
            {
                AI.SelectCard(高刻度);
            }
            return Duel.LifePoints[0] > 800;
        }

        private bool 机壳别名愚钝效果()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
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

        private bool 机壳档案色欲效果()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = AI.Utils.GetProblematicMonsterCard();
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
            return false;
        }

        private bool 机壳基因组贪欲效果()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            ClientCard target = AI.Utils.GetProblematicSpellCard();
            if (target != null)
            {
                AI.SelectCard(target);
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

        private bool 应进行灵摆召唤()
        {
            ClientCard l = AI.Utils.GetPZone(0, 0);
            ClientCard r = AI.Utils.GetPZone(0, 1);
            if (l != null && r != null && l.LScale != r.RScale)
            {
                int count = 0;
                foreach (ClientCard card in Duel.Fields[0].Hand.GetMonsters())
                {
                    count++;
                }
                foreach (ClientCard card in Duel.Fields[0].ExtraDeck.GetMonsters())
                {
                    if (card.HasType(CardType.Pendulum) && card.IsFaceup())
                        count++;
                }
                return count > 1;
            }
            return false;
        }

        private bool DontChainMyself()
        {
            return LastChainPlayer != 0;
        }
    }
}