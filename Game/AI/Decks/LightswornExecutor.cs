using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Lightsworn", "AI_Lightsworn")]
    public class LightswornExecutor : DefaultExecutor
    {
        public enum CardId
        {
            裁决之龙 = 57774843,
            光道兽沃尔夫 = 58996430,
            妖精传姬白雪 = 55623480,
            光道暗杀者莱登 = 77558536,
            光道魔术师丽拉 = 22624373,
            娱乐法师戏法小丑 = 67696066,
            日食翼龙 = 51858306,
            哥布林德伯格 = 25259669,
            英豪挑战者千刀兵 = 1833916,
            光道弓手费莉丝 = 73176465,
            欧尼斯特 = 37742478,
            光道召唤师露米娜丝 = 95503687,
            光道少女密涅瓦 = 40164421,
            成长的鳞茎 = 67441435,
            太阳交换 = 691925,
            鹰身女妖的羽毛扫 = 18144506,
            增援 = 32807846,
            愚蠢的埋葬 = 81439173,
            死者苏生 = 83764718,
            光之援军 = 94886282,
            琰魔龙红莲魔渊 = 9753964,
            冰结界之龙三叉龙 = 52687916,
            红莲魔龙右红痕 = 80666118,
            PSY骨架王Ω = 74586817,
            光道主大天使米迦勒 = 4779823,
            幻透翼同调龙 = 82044279,
            闪光No39希望皇霍普电光皇 = 56832966,
            No39希望皇霍普 = 84013237,
            No101寂静荣誉方舟骑士 = 48739166,
            鸟铳士卡斯泰尔 = 82633039,
            辉光子帕拉迪奥斯 = 61344030,
            光道圣女密涅瓦 = 30100551,
            励辉士入魔蝇王 = 46772449
        }

        bool 已发动小丑 = false;

        public LightswornExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);
            AddExecutor(ExecutorType.Activate, (int)CardId.裁决之龙, DefaultDarkHole);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.裁决之龙);

            AddExecutor(ExecutorType.Activate, (int)CardId.增援, 增援效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.光之援军, 光之援军效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.太阳交换, 太阳交换效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.哥布林德伯格, 哥布林德伯格通常召唤);

            // 常用额外
            AddExecutor(ExecutorType.SpSummon, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.辉光子帕拉迪奥斯, 辉光子帕拉迪奥斯特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.辉光子帕拉迪奥斯, 辉光子帕拉迪奥斯效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.鸟铳士卡斯泰尔, 鸟铳士卡斯泰尔特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.鸟铳士卡斯泰尔, 鸟铳士卡斯泰尔效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.红莲魔龙右红痕, 红莲魔龙右红痕特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.红莲魔龙右红痕, 红莲魔龙右红痕效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.No39希望皇霍普, 电光皇特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.闪光No39希望皇霍普电光皇);
            AddExecutor(ExecutorType.Activate, (int)CardId.闪光No39希望皇霍普电光皇);

            AddExecutor(ExecutorType.Activate, (int)CardId.日食翼龙);
            AddExecutor(ExecutorType.Activate, (int)CardId.娱乐法师戏法小丑, 娱乐法师戏法小丑效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.英豪挑战者千刀兵);
            AddExecutor(ExecutorType.Activate, (int)CardId.欧尼斯特, 欧尼斯特效果);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已发动小丑 = false;
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
                if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand((int)CardId.欧尼斯特))
                    attacker.RealPower = attacker.RealPower + defender.Attack;
                if (attacker.Id == (int)CardId.闪光No39希望皇霍普电光皇 && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.No39希望皇霍普))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            if (max == 2 && min == 2 && cards[0].Location == CardLocation.MonsterZone)
            {
                Logger.DebugWriteLine("超量召唤.");
                IList<ClientCard> avail = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    // clone
                    avail.Add(card);
                }
                IList<ClientCard> result = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (!result.Contains(card) && (!已发动小丑 || card.Id != (int)CardId.娱乐法师戏法小丑))
                        result.Add(card);
                    if (result.Count >= 2)
                        break;
                }
                if (result.Count < 2)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (!result.Contains(card))
                            result.Add(card);
                        if (result.Count >= 2)
                            break;
                    }
                }
                return result;
            }
            Logger.DebugWriteLine("Use default.");
            return null;
        }

        private bool 增援效果()
        {
            if (!Bot.HasInHand((int)CardId.哥布林德伯格))
                AI.SelectCard((int)CardId.哥布林德伯格);
            else if (!Bot.HasInHand((int)CardId.光道暗杀者莱登))
                AI.SelectCard((int)CardId.光道暗杀者莱登);
            return true;
        }

        private bool 光之援军效果()
        {
            if (!Bot.HasInHand((int)CardId.光道召唤师露米娜丝))
                AI.SelectCard((int)CardId.光道召唤师露米娜丝);
            else
                AI.SelectCard(new[]
                {
                    (int)CardId.光道暗杀者莱登,
                    (int)CardId.光道召唤师露米娜丝,
                    (int)CardId.光道少女密涅瓦,
                    (int)CardId.光道魔术师丽拉
                });
            return true;
        }

        private bool 太阳交换效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.光道兽沃尔夫,
                    (int)CardId.光道弓手费莉丝,
                    (int)CardId.光道少女密涅瓦,
                    (int)CardId.光道魔术师丽拉,
                    (int)CardId.光道暗杀者莱登
                });
            return true;
        }

        private bool 哥布林德伯格通常召唤()
        {
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != Card && card.IsMonster() && card.Level == 4)
                    return true;
            }
            return false;
        }

        private bool 哥布林德伯格效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.光道弓手费莉丝,
                    (int)CardId.光道兽沃尔夫,
                    (int)CardId.光道暗杀者莱登,
                    (int)CardId.妖精传姬白雪,
                    (int)CardId.娱乐法师戏法小丑,
                    (int)CardId.英豪挑战者千刀兵
                });
            return true;
        }

        private bool 娱乐法师戏法小丑效果()
        {
            已发动小丑 = true;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 欧尼斯特效果()
        {
            return Duel.Phase != DuelPhase.Main1;
        }

        private bool 励辉士入魔蝇王特殊召唤()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && 励辉士入魔蝇王效果();
        }

        private bool 励辉士入魔蝇王效果()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();

            int selfAttack = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                selfAttack += monster.GetDefensePower();
            }

            int oppoAttack = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                oppoAttack += monster.GetDefensePower();
            }

            return (selfCount < oppoCount) || (selfAttack < oppoAttack);
        }

        private bool 红莲魔龙右红痕特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot, true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Enemy, false);
            return (selfBestAttack <= oppoBestAttack && oppoBestAttack <= 3000) || 红莲魔龙右红痕效果();
        }

        private bool 红莲魔龙右红痕效果()
        {
            int selfCount = 0;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!monster.Equals(Card) && monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    selfCount++;
            }

            int oppoCount = 0;
            monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                // 没有办法获取特殊召唤的状态，只好默认全部是特招的
                if (monster.HasType(CardType.Effect) && monster.Attack <= Card.Attack)
                    oppoCount++;
            }

            return (oppoCount > 0 && selfCount <= oppoCount) || oppoCount > 2;
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

        private bool 辉光子帕拉迪奥斯特殊召唤()
        {
            return 辉光子帕拉迪奥斯效果();
        }

        private bool 辉光子帕拉迪奥斯效果()
        {
            ClientCard result = AI.Utils.GetOneEnemyBetterThanValue(2000, true);
            if (result != null)
            {
                AI.SelectNextCard(result);
                return true;
            }
            return false;
        }

        private bool 电光皇特殊召唤()
        {
            return AI.Utils.IsEnemyBetter(false, false);
        }
    }
}