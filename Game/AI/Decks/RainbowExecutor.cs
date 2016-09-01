using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace MycardBot.Game.AI.Decks
{
    [Deck("Rainbow", "AI_Rainbow")]
    class RainbowExecutor : DefaultExecutor
    {
        public enum CardId
        {
            幻壳龙 = 18108166,
            幻之狮鹫 = 74852097,
            龙剑士卓辉星灵摆 = 75195825,
            曼陀罗天使号手 = 87979586,
            炼装勇士金驰 = 33256280,
            打喷嚏的河马龙 = 51934376,
            救援兔 = 85138716,

            粗人预料 = 911883,
            鹰身女妖的羽毛扫 = 18144506,
            强欲而贪欲之壶 = 35261759,
            死者苏生 = 83764718,
            地碎 = 97169186,

            沙尘防护罩尘埃之力 = 40838625,
            波纹防护罩波浪之力 = 47475363,
            业炎防护罩火焰之力 = 75249652,
            神风防护罩大气之力 = 5650082,
            神圣防护罩反射镜力 = 44095762,
            邪恶防护罩暗黑之力 = 20522190,
            奈落的落穴 = 29401950,
            虫惑的落穴 = 29616929,
            星光大道 = 58120309,

            红莲魔龙右红痕 = 80666118,
            爆龙剑士点火星日珥 = 18239909,
            星尘龙 = 44508094,
            闪光No39希望皇霍普电光皇 = 56832966,
            No37希望织龙蜘蛛鲨 = 37279508,
            No39希望皇霍普 = 84013237,
            进化帝半鸟龙 = 74294676,
            No59背反之料理人 = 82697249,
            鸟铳士卡斯泰尔 = 82633039,
            辉光子帕拉迪奥斯 = 61344030,
            电光千鸟 = 22653490,
            励辉士入魔蝇王 = 46772449,
            我我我枪手 = 12014404,
            入魔梦魇骑士 = 359563,
            芙莉西亚之虫惑魔 = 6511113
        }

        private bool 已通常召唤 = false;

        public RainbowExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);

            AddExecutor(ExecutorType.Activate, (int)CardId.粗人预料, 粗人预料效果);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.星光大道);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.沙尘防护罩尘埃之力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.波纹防护罩波浪之力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.业炎防护罩火焰之力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神风防护罩大气之力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神圣防护罩反射镜力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.邪恶防护罩暗黑之力);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.奈落的落穴);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虫惑的落穴);

            AddExecutor(ExecutorType.Summon, (int)CardId.救援兔);
            AddExecutor(ExecutorType.Activate, (int)CardId.救援兔, 救援兔效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.强欲而贪欲之壶, 强欲而贪欲之壶效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.曼陀罗天使号手, 曼陀罗天使号手通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.打喷嚏的河马龙, 打喷嚏的河马龙通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.龙剑士卓辉星灵摆, 龙剑士卓辉星灵摆通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.幻壳龙, 幻壳龙通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.幻之狮鹫, 幻之狮鹫通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.炼装勇士金驰, 炼装勇士金驰通常召唤);

            AddExecutor(ExecutorType.Summon, 通常召唤);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.我我我枪手, 我我我枪手特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.我我我枪手);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.进化帝半鸟龙, 进化帝半鸟龙特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.进化帝半鸟龙, 进化帝半鸟龙效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.入魔梦魇骑士, 入魔梦魇骑士特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.入魔梦魇骑士);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电光千鸟, 电光千鸟特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.电光千鸟, 电光千鸟效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.No37希望织龙蜘蛛鲨);
            AddExecutor(ExecutorType.Activate, (int)CardId.No37希望织龙蜘蛛鲨);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.芙莉西亚之虫惑魔, 芙莉西亚之虫惑魔特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.芙莉西亚之虫惑魔);

            AddExecutor(ExecutorType.Activate, (int)CardId.地碎, 地碎效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.鸟铳士卡斯泰尔, 鸟铳士卡斯泰尔特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.鸟铳士卡斯泰尔, 鸟铳士卡斯泰尔效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.爆龙剑士点火星日珥, 爆龙剑士点火星日珥特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.爆龙剑士点火星日珥, 爆龙剑士点火星日珥效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.红莲魔龙右红痕, 红莲魔龙右红痕特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.红莲魔龙右红痕, 红莲魔龙右红痕效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.No39希望皇霍普, 电光皇特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.闪光No39希望皇霍普电光皇);
            AddExecutor(ExecutorType.Activate, (int)CardId.闪光No39希望皇霍普电光皇);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.星尘龙);
            AddExecutor(ExecutorType.Activate, (int)CardId.星尘龙, 星尘龙效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.No59背反之料理人, No59背反之料理人特殊召唤);

            AddExecutor(ExecutorType.Activate, (int)CardId.星光大道, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.沙尘防护罩尘埃之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.波纹防护罩波浪之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.业炎防护罩火焰之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神风防护罩大气之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩反射镜力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.邪恶防护罩暗黑之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.奈落的落穴, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.虫惑的落穴, DefaultUniqueTrap);

        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已通常召唤 = false;
        }
        
        public override bool OnSelectHand()
        {
            // 随机先后攻
            return Program.Rand.Next(2) > 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == (int)CardId.闪光No39希望皇霍普电光皇))
            {
                if (attacker.Id == (int)CardId.闪光No39希望皇霍普电光皇 && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.No39希望皇霍普))
                    attacker.RealPower = 5000;
                if (Duel.Fields[0].HasInMonstersZone((int)CardId.No37希望织龙蜘蛛鲨, true, true))
                    attacker.RealPower = attacker.RealPower + 1000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool 粗人预料效果()
        {
            if (Duel.Fields[0].HasInHand((int)CardId.救援兔) || 已通常召唤)
                AI.SelectCard(new[]
                {
                    (int)CardId.幻壳龙,
                    (int)CardId.幻之狮鹫
                });
            else if (AI.Utils.IsTurn1OrMain2())
            {
                if (Duel.Fields[0].HasInHand((int)CardId.幻壳龙))
                    AI.SelectCard((int)CardId.幻壳龙);
                else if (Duel.Fields[0].HasInHand((int)CardId.打喷嚏的河马龙))
                    AI.SelectCard((int)CardId.打喷嚏的河马龙);
                else if (Duel.Fields[0].HasInHand((int)CardId.曼陀罗天使号手))
                    AI.SelectCard((int)CardId.曼陀罗天使号手);
            }
            else
            {
                if (Duel.Fields[0].HasInHand((int)CardId.打喷嚏的河马龙))
                    AI.SelectCard((int)CardId.打喷嚏的河马龙);
                else if (Duel.Fields[0].HasInHand((int)CardId.龙剑士卓辉星灵摆))
                    AI.SelectCard((int)CardId.龙剑士卓辉星灵摆);
                else if (Duel.Fields[0].HasInHand((int)CardId.幻之狮鹫))
                    AI.SelectCard((int)CardId.幻之狮鹫);
                else if (Duel.Fields[0].HasInHand((int)CardId.曼陀罗天使号手))
                    AI.SelectCard(new[]
                    {
                        (int)CardId.炼装勇士金驰,
                        (int)CardId.龙剑士卓辉星灵摆
                    });
            }
            return true;
        }

        private bool 救援兔效果()
        {
            if (AI.Utils.IsTurn1OrMain2())
                AI.SelectCard(new[]
                    {
                        (int)CardId.打喷嚏的河马龙,
                        (int)CardId.幻壳龙
                    });
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.龙剑士卓辉星灵摆,
                        (int)CardId.幻之狮鹫,
                        (int)CardId.打喷嚏的河马龙,
                        (int)CardId.炼装勇士金驰,
                        (int)CardId.曼陀罗天使号手
                    });
            return true;
        }

        private bool 幻壳龙通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.幻壳龙);
        }
        private bool 幻之狮鹫通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.幻之狮鹫);
        }
        private bool 龙剑士卓辉星灵摆通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.龙剑士卓辉星灵摆);
        }
        private bool 曼陀罗天使号手通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.曼陀罗天使号手);
        }
        private bool 炼装勇士金驰通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.炼装勇士金驰);
        }
        private bool 打喷嚏的河马龙通常召唤()
        {
            return Duel.Fields[0].HasInMonstersZone((int)CardId.打喷嚏的河马龙);
        }
        private bool 通常召唤()
        {
            return true;
        }

        private bool 我我我枪手特殊召唤()
        {
            if (Duel.LifePoints[1] <= 800)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 励辉士入魔蝇王特殊召唤()
        {
            int selfCount = Duel.Fields[0].GetMonsterCount() + Duel.Fields[0].GetSpellCount() + Duel.Fields[0].GetHandCount();
            int oppoCount = Duel.Fields[1].GetMonsterCount() + Duel.Fields[1].GetSpellCount() + Duel.Fields[1].GetHandCount();
            return (selfCount - 1 < oppoCount) && 励辉士入魔蝇王效果();
        }

        private bool 励辉士入魔蝇王效果()
        {
            int selfCount = Duel.Fields[0].GetMonsterCount() + Duel.Fields[0].GetSpellCount();
            int oppoCount = Duel.Fields[1].GetMonsterCount() + Duel.Fields[1].GetSpellCount();
            return selfCount < oppoCount;
        }

        private bool 红莲魔龙右红痕特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Duel.Fields[0], true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Duel.Fields[1], false);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || 红莲魔龙右红痕效果();
        }

        private bool 红莲魔龙右红痕效果()
        {
            int selfCount = 0;
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!monster.Equals(Card) && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    selfCount++;
            }

            int oppoCount = 0;
            monsters = Duel.Fields[1].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                // 没有办法获取特殊召唤的状态，只好默认全部是特招的
                if (monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    oppoCount++;
            }

            return selfCount <= oppoCount || oppoCount > 2;
        }

        private bool 鸟铳士卡斯泰尔特殊召唤()
        {
            return AI.Utils.GetProblematicCard() != null;
        }

        private bool 鸟铳士卡斯泰尔效果()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.鸟铳士卡斯泰尔, 0))
                return false;
            AI.SelectNextCard(AI.Utils.GetProblematicCard());
            return true;
        }

        private bool 爆龙剑士点火星日珥特殊召唤()
        {
            return AI.Utils.GetProblematicCard() != null;
        }

        private bool 爆龙剑士点火星日珥效果()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.爆龙剑士点火星日珥, 1))
                return true;
            AI.SelectNextCard(AI.Utils.GetProblematicCard());
            return true;
        }

        private bool 电光千鸟特殊召唤()
        {
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsFacedown())
                {
                    return true;
                }
            }
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    return true;
                }
            }

            return AI.Utils.GetProblematicCard() != null;
        }

        private bool 电光千鸟效果()
        {
            ClientCard problematicCard = AI.Utils.GetProblematicCard();
            AI.SelectCard(problematicCard);
            return true;
        }

        private bool 星尘龙效果()
        {
            return (Card.Location==CardLocation.Grave) || DefaultTrap();
        }

        private bool 进化帝半鸟龙特殊召唤()
        {
            return !AI.Utils.IsOneEnnemyBetterThanValue(2400, false);
        }

        private bool 入魔梦魇骑士特殊召唤()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 芙莉西亚之虫惑魔特殊召唤()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool No59背反之料理人特殊召唤()
        {
            return AI.Utils.IsTurn1OrMain2();
        }

        private bool 进化帝半鸟龙效果()
        {
            return DefaultTrap();
        }

        private bool 辉光子帕拉迪奥斯效果()
        {
            ClientCard result = AI.Utils.GetOneEnnemyBetterThanValue(2000, true);
            if (result != null)
            {
                AI.SelectCard(result);
                return true;
            }
            return false;
        }

        private bool 电光皇特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Duel.Fields[0], true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Duel.Fields[1], false);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool 强欲而贪欲之壶效果()
        {
            return Duel.Fields[0].Deck.Count > 15;
        }

        private bool 地碎效果()
        {
            return AI.Utils.IsEnnemyBetter(false, false);
        }
    }
}
