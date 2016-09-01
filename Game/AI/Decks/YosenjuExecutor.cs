using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Yosenju", "AI_Yosenju")]
    public class YosenjuExecutor : DefaultExecutor
    {
        public enum CardId
        {
            妖仙兽镰壹太刀 = 65247798,
            妖仙兽镰贰太刀 = 92246806,
            妖仙兽镰叁太刀 = 28630501,
            妖仙兽辻斩风 = 25244515,
            鹰身女妖的羽毛扫 = 18144507,
            黑洞 = 53129443,
            削命的宝札 = 59750328,
            强欲而谦虚之壶 = 98645731,
            宇宙旋风 = 8267140,
            沙尘防护罩尘埃之力 = 40838625,
            波纹防护罩波浪之力 = 47475363,
            星光大道 = 58120309,
            虚无空间 = 5851097,
            大宇宙 = 30241314,
            神之通告 = 40605147,
            神之警告 = 84749824,
            神之宣告 = 41420027,
            魔力抽取 = 59344077,
            星尘龙 = 44508094,
            闪光No39希望皇霍普电光皇 = 56832966,
            闪光No39希望皇霍普一 = 86532744,
            暗叛逆超量龙 = 16195942,
            No39希望皇霍普 = 84013237,
            No103神葬零娘暮零 = 94380860,
            魁炎星王宋虎 = 96381979,
            No106巨岩掌巨手 = 63746411,
            鸟铳士卡斯泰尔 = 82633039,
            恐牙狼钻石恐狼 = 95169481,
            电光千鸟 = 22653490,
            励辉士入魔蝇王 = 46772449,
            深渊的潜伏者 = 21044178,
            我我我枪手 = 12014404
        }

        bool 已发动削命 = false;

        public YosenjuExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // 结束阶段优先丢卡
            AddExecutor(ExecutorType.Activate, (int)CardId.削命的宝札, 削命的宝札结束阶段);

            // 能烧就烧
            AddExecutor(ExecutorType.SpSummon, (int)CardId.我我我枪手, 我我我枪手特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.我我我枪手);

            // 清场
            AddExecutor(ExecutorType.Activate, (int)CardId.宇宙旋风, 宇宙旋风);
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);

            // 开壶
            AddExecutor(ExecutorType.Activate, (int)CardId.强欲而谦虚之壶, 强欲而谦虚之壶);

            // 通招
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰壹太刀, 优先出重复的妖仙兽);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰贰太刀, 优先出重复的妖仙兽);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰叁太刀, 优先出重复的妖仙兽);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰壹太刀);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰贰太刀);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽镰叁太刀);
            AddExecutor(ExecutorType.Summon, (int)CardId.妖仙兽辻斩风);

            // 妖仙兽效果无脑发动
            AddExecutor(ExecutorType.Activate, (int)CardId.妖仙兽镰壹太刀, 妖仙兽效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.妖仙兽镰贰太刀, 妖仙兽效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.妖仙兽镰叁太刀, 妖仙兽效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.妖仙兽辻斩风, 妖仙兽效果);

            // 盖坑
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之宣告, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之警告, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.大宇宙, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.魔力抽取, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.波纹防护罩波浪之力, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.沙尘防护罩尘埃之力, 优先盖不重复的坑);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.星光大道, 优先盖不重复的坑);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之宣告, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之警告, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.大宇宙, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.魔力抽取, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.波纹防护罩波浪之力, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.沙尘防护罩尘埃之力, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.星光大道, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.鹰身女妖的羽毛扫, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.黑洞, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强欲而谦虚之壶, 魔陷区有空余格子);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.宇宙旋风, 魔陷区有空余格子);

            // 开完削命继续盖坑
            AddExecutor(ExecutorType.SpellSet, (int)CardId.削命的宝札);
            AddExecutor(ExecutorType.Activate, (int)CardId.削命的宝札, 削命的宝札);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之宣告, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之通告, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.神之警告, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.大宇宙, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.虚无空间, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.魔力抽取, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.波纹防护罩波浪之力, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.沙尘防护罩尘埃之力, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.星光大道, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.鹰身女妖的羽毛扫, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.黑洞, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.强欲而谦虚之壶, 已发动过削命);
            AddExecutor(ExecutorType.SpellSet, (int)CardId.宇宙旋风, 已发动过削命);

            // 常用额外
            AddExecutor(ExecutorType.SpSummon, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.暗叛逆超量龙, 暗叛逆超量龙特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.暗叛逆超量龙, 暗叛逆超量龙效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.No39希望皇霍普, 电光皇特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.闪光No39希望皇霍普一);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.闪光No39希望皇霍普电光皇);
            AddExecutor(ExecutorType.Activate, (int)CardId.闪光No39希望皇霍普电光皇);

            AddExecutor(ExecutorType.Activate, (int)CardId.星尘龙, 星尘龙效果);

            // 坑人
            AddExecutor(ExecutorType.Activate, (int)CardId.星光大道, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.魔力抽取);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之警告, 神之警告);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之通告, 神之通告);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之宣告, 神之宣告);
            AddExecutor(ExecutorType.Activate, (int)CardId.大宇宙, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.虚无空间, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.波纹防护罩波浪之力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.沙尘防护罩尘埃之力, DefaultUniqueTrap);
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

        public override bool OnSelectYesNo(int desc)
        {
            // 镰贰太刀能不直击就不直击
            Logger.DebugWriteLine(Card.Name);
            if (Card.Id == (int)CardId.妖仙兽镰贰太刀)
                return Card.ShouldDirectAttack;
            else
                return true;
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
                if (attacker.Attribute == (int)CardAttribute.Wind && Duel.Fields[0].HasInHand((int)CardId.妖仙兽辻斩风))
                    attacker.RealPower = attacker.RealPower + 1000;
                if (attacker.Id == (int)CardId.闪光No39希望皇霍普电光皇 && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.No39希望皇霍普))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool 宇宙旋风()
        {
            return (Duel.LifePoints[0] > 1000) && DefaultMysticalSpaceTyphoon();
        }

        private bool 神之警告()
        {
            return (Duel.LifePoints[0] > 2000) && !(Duel.Player == 0 && LastChainPlayer == -1) && DefaultTrap();
        }

        private bool 神之通告()
        {
            return (Duel.LifePoints[0] > 1500) && !(Duel.Player == 0 && LastChainPlayer == -1) && DefaultTrap();
        }

        private bool 神之宣告()
        {
            return !(Duel.ChainTargets.Count == 1 && Card.Equals(Duel.ChainTargets[0])) && DefaultTrap();
        }

        private bool 强欲而谦虚之壶()
        {
            if (已发动削命)
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.星光大道,
                    (int)CardId.魔力抽取,
                    (int)CardId.神之宣告,
                    (int)CardId.虚无空间,
                    (int)CardId.鹰身女妖的羽毛扫,
                    (int)CardId.波纹防护罩波浪之力,
                    (int)CardId.沙尘防护罩尘埃之力,
                    (int)CardId.神之通告,
                    (int)CardId.神之警告,
                    (int)CardId.大宇宙,
                    (int)CardId.削命的宝札
                });
            }
            else
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.妖仙兽镰叁太刀,
                    (int)CardId.妖仙兽镰壹太刀,
                    (int)CardId.妖仙兽镰贰太刀,
                    (int)CardId.星光大道,
                    (int)CardId.魔力抽取,
                    (int)CardId.虚无空间,
                    (int)CardId.鹰身女妖的羽毛扫,
                    (int)CardId.波纹防护罩波浪之力,
                    (int)CardId.沙尘防护罩尘埃之力,
                    (int)CardId.神之通告,
                    (int)CardId.神之宣告,
                    (int)CardId.神之警告,
                    (int)CardId.大宇宙,
                    (int)CardId.削命的宝札,
                });
            }
            return true;
        }

        private bool 削命的宝札()
        {
            if (AI.Utils.IsTurn1OrMain2())
            {
                已发动削命 = true;
                return true;
            }
            return false;
        }

        private bool 优先出重复的妖仙兽()
        {
            foreach (ClientCard card in Duel.Fields[0].Hand)
            {
                if (card != null && !card.Equals(Card) && card.Id == Card.Id)
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

        private bool 妖仙兽效果()
        {
            // 妖仙兽结束阶段不优先回手
            if (Duel.Phase == DuelPhase.End)
                return false;
            AI.SelectCard(new[]
                {
                    (int)CardId.妖仙兽镰壹太刀,
                    (int)CardId.妖仙兽镰贰太刀,
                    (int)CardId.妖仙兽镰叁太刀
                });
            return true;
        }

        private bool 削命的宝札结束阶段()
        {
            // 削命宝札结束阶段在妖仙回手前丢手卡
            Logger.DebugWriteLine("削命的宝札" + (Duel.Phase == DuelPhase.End));
            return Duel.Phase == DuelPhase.End;
        }

        private bool 我我我枪手特殊召唤()
        {
            if (Duel.LifePoints[1] <= 800 || (Duel.Fields[0].GetMonsterCount()>=4 && Duel.LifePoints[1] <= 1600))
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

        private bool 暗叛逆超量龙特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Duel.Fields[0], true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Duel.Fields[1], true);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool 暗叛逆超量龙效果()
        {
            int oppoBestAttack = AI.Utils.GetBestAttack(Duel.Fields[1], true);
            ClientCard target = AI.Utils.GetOneEnnemyBetterThanValue(oppoBestAttack, true);
            if (target != null)
            {
                AI.SelectNextCard(target);
            }
            return true;
        }

        private bool 电光皇特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Duel.Fields[0], true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Duel.Fields[1], false);
            return selfBestAttack <= oppoBestAttack;
        }

        private bool 星尘龙效果()
        {
            return LastChainPlayer == 1;
        }

        private bool DontChainMyself()
        {
            return LastChainPlayer != 0;
        }
    }
}