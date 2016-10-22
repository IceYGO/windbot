using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace MycardBot.Game.AI.Decks
{
    [Deck("Blue-Eyes", "AI_BlueEyes")]
    class BlueEyesExecutor : DefaultExecutor
    {
        public enum CardId
        {
            青眼白龙 = 89631139,
            青眼亚白龙 = 38517737,
            白色灵龙 = 45467446,
            增殖的G = 23434538,
            太古的白石 = 71039903,
            传说的白石 = 79814787,
            青色眼睛的贤士 = 8240199,
            效果遮蒙者 = 97268402,
            银河旋风 = 5133471,
            鹰身女妖的羽毛扫 = 18144506,
            复活之福音 = 6853254,
            强欲而贪欲之壶 = 35261759,
            抵价购物 = 38120068,
            调和的宝札 = 39701395,
            龙之灵庙 = 41620959,
            龙觉醒旋律 = 48800175,
            灵魂补充 = 54447022,
            死者苏生 = 83764718,
            银龙的轰咆 = 87025064,

            鬼岩城 = 63422098,
            苍眼银龙 = 40908371,
            青眼精灵龙 = 59822133,
            银河眼暗物质龙 = 58820923,
            银河眼光波刃龙 = 2530830,
            银河眼重铠光子龙 = 39030163,
            银河眼光子龙皇 = 31801517,
            银河眼光波龙 = 18963306,
            希望魁龙银河巨神 = 63767246,
            森罗的姬牙宫 = 33909817
        }

        private List<ClientCard> 使用过的青眼亚白龙 = new List<ClientCard>();
        ClientCard 使用过的光波龙;
        bool 已特殊召唤青眼亚白龙 = false;
        bool 已发动灵魂补充 = false;

        public BlueEyesExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // 有坑先清
            AddExecutor(ExecutorType.Activate, (int)CardId.银河旋风, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);

            // 灵庙
            AddExecutor(ExecutorType.Activate, (int)CardId.龙之灵庙, 龙之灵庙效果);

            // 贤士检索
            AddExecutor(ExecutorType.Summon, (int)CardId.青色眼睛的贤士, 青色眼睛的贤士通常召唤);

            // 拿亚白
            AddExecutor(ExecutorType.Activate, (int)CardId.龙觉醒旋律, 龙觉醒旋律效果);

            // 调和
            AddExecutor(ExecutorType.Activate, (int)CardId.调和的宝札, 调和的宝札效果);

            // 八抽
            AddExecutor(ExecutorType.Activate, (int)CardId.抵价购物, 抵价购物效果);

            // 吸一口
            AddExecutor(ExecutorType.Activate, (int)CardId.强欲而贪欲之壶, 强欲而贪欲之壶效果);

            // 有亚白就跳
            AddExecutor(ExecutorType.SpSummon, (int)CardId.青眼亚白龙, 青眼亚白龙特殊召唤);

            // 苏生
            AddExecutor(ExecutorType.Activate, (int)CardId.复活之福音, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银龙的轰咆, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.死者苏生, 死者苏生效果);

            // 效果
            AddExecutor(ExecutorType.Activate, (int)CardId.青眼亚白龙, 青眼亚白龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.青色眼睛的贤士, 青色眼睛的贤士效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.太古的白石, 太古的白石效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.白色灵龙, 白色灵龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.青眼精灵龙, 青眼精灵龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.希望魁龙银河巨神, 希望魁龙银河巨神效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银河眼光波龙, 银河眼光波龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银河眼光子龙皇, 银河眼光子龙皇效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银河眼重铠光子龙, 银河眼重铠光子龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银河眼光波刃龙, 银河眼光波刃龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.银河眼暗物质龙, 银河眼暗物质龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.苍眼银龙, 苍眼银龙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.森罗的姬牙宫, 森罗的姬芽宫效果);

            // 通招
            AddExecutor(ExecutorType.Summon, (int)CardId.青色眼睛的贤士, 太古的白石通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.太古的白石, 太古的白石通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.传说的白石, 太古的白石通常召唤);

            // 出大怪
            AddExecutor(ExecutorType.SpSummon, (int)CardId.银河眼光波龙, 银河眼光波龙超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.银河眼光子龙皇, 银河眼光子龙皇超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.银河眼重铠光子龙, 银河眼重铠光子龙超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.银河眼光波刃龙, 银河眼光波刃龙超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.银河眼暗物质龙, 银河眼暗物质龙超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.鬼岩城, 鬼岩城同调召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.青眼精灵龙, 青眼精灵龙同调召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.希望魁龙银河巨神, 希望魁龙银河巨神超量召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.森罗的姬牙宫, 森罗的姬芽宫超量召唤);

            // 没别的可干
            AddExecutor(ExecutorType.Activate, (int)CardId.灵魂补充, 灵魂补充);
            AddExecutor(ExecutorType.Repos, 改变攻守表示);
            // 通招白石发动贤士手卡效果
            AddExecutor(ExecutorType.Summon, (int)CardId.传说的白石, 传说的白石通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.太古的白石, 传说的白石通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.青色眼睛的贤士, 传说的白石通常召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.青色眼睛的贤士, 青色眼睛的贤士手卡效果);
            // 优先盖传说白石以期望拿到白龙打开局面
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.传说的白石);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.太古的白石);
            AddExecutor(ExecutorType.SpellSet, 盖卡);

        }

        public override bool OnSelectHand()
        {
            // 随机先后攻
            return Program.Rand.Next(2) > 0;
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            使用过的青眼亚白龙.Clear();
            使用过的光波龙 = null;
            已特殊召唤青眼亚白龙 = false;
            已发动灵魂补充 = false;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);
            if (max == 2 && cards[0].Location == CardLocation.Deck)
            {
                Logger.DebugWriteLine("龙觉醒检索.");
                IList<ClientCard> result = new List<ClientCard>();
                if (!Duel.Fields[0].HasInHand((int)CardId.青眼白龙))
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card.Id == (int)CardId.青眼白龙)
                        {
                            result.Add(card);
                            break;
                        }
                    }
                }
                foreach (ClientCard card in cards)
                {
                    if (card.Id == (int)CardId.青眼亚白龙 && result.Count < max)
                    {
                        result.Add(card);
                    }
                }
                if (result.Count < min)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (!result.Contains(card))
                            result.Add(card);
                        if (result.Count >= min)
                            break;
                    }
                }
                while (result.Count > max)
                {
                    result.RemoveAt(result.Count - 1);
                }
                return result;
            }
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
                while (使用过的青眼亚白龙.Count > 0 && avail.IndexOf(使用过的青眼亚白龙[0]) > 0)
                {
                    Logger.DebugWriteLine("优先用使用过的亚白龙超量.");
                    ClientCard card = 使用过的青眼亚白龙[0];
                    使用过的青眼亚白龙.Remove(card);
                    avail.Remove(card);
                    result.Add(card);
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

        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max)
        {
            Logger.DebugWriteLine("OnSelectSum " + cards.Count + " " + sum + " " + min + " " + max);
            IList<ClientCard> avail = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                // clone
                avail.Add(card);
            }
            IList<ClientCard> selected = new List<ClientCard>();
            int trysum = 0;
            if (使用过的青眼亚白龙.Count > 0 && avail.IndexOf(使用过的青眼亚白龙[0]) > 0)
            {
                Logger.DebugWriteLine("优先用使用过的亚白龙同调.");
                ClientCard card = 使用过的青眼亚白龙[0];
                使用过的青眼亚白龙.Remove(card);
                avail.Remove(card);
                selected.Add(card);
                trysum = card.Level;
                if (trysum == sum)
                {
                    Logger.DebugWriteLine("直接选择了使用过的青眼亚白龙");
                    return selected;
                }
            }
            foreach (ClientCard card in cards)
            {
                // try level equal
                Logger.DebugWriteLine("同调素材可以选择: " + card.Name);
                if (card.Level == sum)
                {
                    Logger.DebugWriteLine("直接选择了" + card.Name);
                    return new[] { card };
                }
                // try level add
                if (trysum + card.Level > sum)
                {
                    Logger.DebugWriteLine("跳过了" + card.Name);
                    continue;
                }
                selected.Add(card);
                trysum += card.Level;
                Logger.DebugWriteLine("添加" + card.Name);
                Logger.DebugWriteLine(trysum + " selected " + sum);
                if (trysum == sum)
                {
                    return selected;
                }
            }
            IList<ClientCard> selected2 = new List<ClientCard>();
            foreach (ClientCard card in selected)
            {
                // clone
                selected2.Add(card);
            }
            foreach (ClientCard card in selected)
            {
                // try level sub
                selected2.Remove(card);
                trysum -= card.Level;
                Logger.DebugWriteLine("排除" + card.Name);
                Logger.DebugWriteLine(trysum + " selected2 " + sum);
                if (trysum == sum)
                {
                    return selected2;
                }
            }
            // try all
            return cards;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            return attacker.Attack > 0;
        }

        private bool 龙之灵庙效果()
        {
            Logger.DebugWriteLine("龙之灵庙.");
            AI.SelectCard(new[]
                {
                    (int)CardId.白色灵龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石
                });
            if (!Duel.Fields[0].HasInHand((int)CardId.青眼白龙))
            {
                Logger.DebugWriteLine("手里没有本体，堆白石.");
                AI.SelectNextCard((int)CardId.传说的白石);
            }
            else
            {
                Logger.DebugWriteLine("堆太古或灵龙或白石.");
                AI.SelectNextCard(new[]
                {
                    (int)CardId.太古的白石,
                    (int)CardId.白色灵龙,
                    (int)CardId.传说的白石
                });
            }
            return true;
        }

        private bool 龙觉醒旋律效果()
        {
            Logger.DebugWriteLine("龙觉醒选要丢的卡.");
            AI.SelectCard(new[]
                {
                    (int)CardId.太古的白石,
                    (int)CardId.白色灵龙,
                    (int)CardId.传说的白石,
                    (int)CardId.银河旋风,
                    (int)CardId.效果遮蒙者,
                    (int)CardId.抵价购物,
                    (int)CardId.青色眼睛的贤士
                });
            return true;
        }

        private bool 调和的宝札效果()
        {
            Logger.DebugWriteLine("调和选要丢的卡.");
            if (!Duel.Fields[0].HasInHand((int)CardId.青眼白龙))
            {
                Logger.DebugWriteLine("手里没有本体，丢白石.");
                AI.SelectCard((int)CardId.传说的白石);
            }
            else if (Duel.Fields[0].HasInHand((int)CardId.抵价购物))
            {
                Logger.DebugWriteLine("手里有本体，再拿一个喂八抽.");
                AI.SelectCard((int)CardId.传说的白石);
            }
            else
            {
                Logger.DebugWriteLine("手里有本体，优先丢太古.");
                AI.SelectCard((int)CardId.太古的白石);
            }
            return true;
        }

        private bool 抵价购物效果()
        {
            Logger.DebugWriteLine("抵价购物发动.");
            if (Duel.Fields[0].HasInHand((int)CardId.白色灵龙))
            {
                Logger.DebugWriteLine("手里有白灵龙，优先丢掉.");
                AI.SelectCard((int)CardId.白色灵龙);
                return true;
            }
            else if (手里有2个((int)CardId.青眼白龙))
            {
                Logger.DebugWriteLine("手里有2个青眼白龙，丢1个.");
                AI.SelectCard((int)CardId.青眼白龙);
                return true;
            }
            else if (手里有2个((int)CardId.青眼亚白龙))
            {
                Logger.DebugWriteLine("手里有2个青眼亚白龙，丢1个.");
                AI.SelectCard((int)CardId.青眼亚白龙);
                return true;
            }
            else if (!Duel.Fields[0].HasInHand((int)CardId.青眼白龙) || !Duel.Fields[0].HasInHand((int)CardId.青眼亚白龙))
            {
                Logger.DebugWriteLine("手里没有成对的青眼和亚白，丢1个.");
                AI.SelectCard(new[]
                {
                    (int)CardId.青眼白龙,
                    (int)CardId.青眼亚白龙
                });
                return true;
            }
            else
            {
                Logger.DebugWriteLine("手里只有一对，不能乱丢.");
                return false;
            }
        }

        private bool 青眼亚白龙效果()
        {
            Logger.DebugWriteLine("亚白龙效果.");
            ClientCard card = Duel.Fields[1].MonsterZone.GetFloodgate();
            if (card != null)
            {
                Logger.DebugWriteLine("炸坑怪.");
                AI.SelectCard(card);
                使用过的青眼亚白龙.Add(Card);
                return true;
            }
            card = Duel.Fields[1].MonsterZone.GetInvincibleMonster();
            if (card != null)
            {
                Logger.DebugWriteLine("炸打不死的怪.");
                AI.SelectCard(card);
                使用过的青眼亚白龙.Add(Card);
                return true;
            }
            card = Duel.Fields[1].MonsterZone.GetDangerousMonster();
            if (card != null)
            {
                Logger.DebugWriteLine("炸厉害的怪.");
                AI.SelectCard(card);
                使用过的青眼亚白龙.Add(Card);
                return true;
            }
            card = AI.Utils.GetOneEnnemyBetterThanValue(Card.GetDefensePower(), false);
            if (card != null)
            {
                Logger.DebugWriteLine("炸比自己强的怪.");
                AI.SelectCard(card);
                使用过的青眼亚白龙.Add(Card);
                return true;
            }
            if (能处理青眼亚白龙())
            {
                使用过的青眼亚白龙.Add(Card);
                return true;
            }
            Logger.DebugWriteLine("不炸.");
            return false;
        }

        private bool 死者苏生效果()
        {
            if (Duel.Player == 0 && CurrentChain.Count > 0)
            {
                Logger.DebugWriteLine("轰咆避免卡时点.");
                return false;
            }
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
            {
                Logger.DebugWriteLine("轰咆自己回合不和苍眼银龙抢对象.");
                return false;
            }
            List<int> targets = new List<int> {
                    (int)CardId.希望魁龙银河巨神,
                    (int)CardId.银河眼暗物质龙,
                    (int)CardId.青眼亚白龙,
                    (int)CardId.苍眼银龙,
                    (int)CardId.青眼精灵龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙
                };
            if (!Duel.Fields[0].HasInGraveyard(targets))
            {
                return false;
            }
            ClientCard floodgate = Duel.Fields[1].SpellZone.GetFloodgate();
            if (floodgate != null && Duel.Fields[0].HasInGraveyard((int)CardId.白色灵龙))
            {
                AI.SelectCard((int)CardId.白色灵龙);
            }
            else
            {
                AI.SelectCard(targets);
            }
            return true;
        }

        private bool 强欲而贪欲之壶效果()
        {
            return Duel.Fields[0].Deck.Count > 15;
        }

        private bool 苍眼银龙效果()
        {
            Logger.DebugWriteLine("苍眼银龙效果.");
            if (Duel.Fields[1].GetSpellCount() > 0)
            {
                AI.SelectCard((int)CardId.白色灵龙);
            }
            else
            {
                AI.SelectCard((int)CardId.青眼白龙);
            }
            return true;
        }

        private bool 青色眼睛的贤士通常召唤()
        {
            Logger.DebugWriteLine("手里没有白石，先贤士检索.");
            return !Duel.Fields[0].HasInHand(new List<int>
                {
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石
                });
        }

        private bool 青色眼睛的贤士效果()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.太古的白石,
                    (int)CardId.效果遮蒙者,
                    (int)CardId.传说的白石
                });
            return true;
        }

        private bool 传说的白石通常召唤()
        {
            Logger.DebugWriteLine("通招白石给贤士发手卡效果.");
            return Duel.Fields[0].HasInHand((int)CardId.青色眼睛的贤士);
        }

        private bool 青色眼睛的贤士手卡效果()
        {
            if (Card.Location != CardLocation.Hand)
            {
                return false;
            }
            if (!Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.传说的白石,
                    (int)CardId.太古的白石
                }) || Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.青眼亚白龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙
                }))
            {
                Logger.DebugWriteLine("能做其他展开，不发贤士效果.");
                return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.传说的白石,
                    (int)CardId.太古的白石
                });
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            if (spells.Count == 0)
            {
                Logger.DebugWriteLine("对面没坑，跳个本体.");
                AI.SelectNextCard((int)CardId.青眼白龙);
            }
            else
            {
                Logger.DebugWriteLine("对面有坑，拆.");
                AI.SelectNextCard((int)CardId.白色灵龙);
            }
            return true;
        }

        private bool 白色灵龙效果()
        {
            Logger.DebugWriteLine("白色灵龙" + ActivateDescription);
            if (ActivateDescription == -1)
            {
                Logger.DebugWriteLine("白色灵龙拆后场.");
                ClientCard target = Duel.Fields[1].SpellZone.GetFloodgate();
                AI.SelectCard(target);
                return true;
            }
            /*else if(Duel.Phase==DuelPhase.BattleStart)
            {
                Logger.DebugWriteLine("白色灵龙战阶变身.");
                return true;
            }*/
            else
            {
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                {
                    Logger.DebugWriteLine("白色灵龙打完变身");
                    return 手里有足够的青眼白龙() && Card.Attacked;
                }
                if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                {
                    Logger.DebugWriteLine("白色灵龙回合结束变身");
                    return 手里有足够的青眼白龙()
                        && Duel.Fields[0].HasInMonstersZone((int)CardId.苍眼银龙, true)
                        && !Duel.Fields[0].HasInGraveyard((int)CardId.白色灵龙)
                        && !Duel.Fields[0].HasInGraveyard((int)CardId.青眼白龙);
                }
                Logger.DebugWriteLine("白色灵龙特招手卡. 对象数量" + Duel.ChainTargets.Count);
                foreach (ClientCard card in Duel.ChainTargets)
                {
                    // Logger.DebugWriteLine("对象" + card.Id);
                    if (Card.Equals(card))
                    {
                        Logger.DebugWriteLine("白色灵龙被取对象，是否变身.");
                        return 手里有足够的青眼白龙();
                    }
                }
                return false;
            }
        }

        private bool 青眼精灵龙效果()
        {
            Logger.DebugWriteLine("青眼精灵龙" + ActivateDescription);
            if (ActivateDescription == -1 || ActivateDescription == AI.Utils.GetStringId((int)CardId.青眼精灵龙, 0))
            {
                Logger.DebugWriteLine("青眼精灵龙无效墓地.");
                return LastChainPlayer == 1;
            }
            else if (Duel.Player == 1 && (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End))
            {
                Logger.DebugWriteLine("青眼精灵龙主动变身.");
                AI.SelectCard((int)CardId.苍眼银龙);
                return true;
            }
            else
            {
                Logger.DebugWriteLine("青眼精灵龙变身. 对象数量" + Duel.ChainTargets.Count);
                foreach (ClientCard card in Duel.ChainTargets)
                {
                    // Logger.DebugWriteLine("对象" + card.Id);
                    if (Card.Equals(card))
                    {
                        Logger.DebugWriteLine("青眼精灵龙被取对象，变身.");
                        AI.SelectCard((int)CardId.苍眼银龙);
                        return true;
                    }
                }
                return false;
            }
        }

        private bool 希望魁龙银河巨神效果()
        {
            Logger.DebugWriteLine("希望魁龙银河巨神" + ActivateDescription);
            if (ActivateDescription == -1 || ActivateDescription == AI.Utils.GetStringId((int)CardId.希望魁龙银河巨神, 0))
            {
                Logger.DebugWriteLine("希望魁龙银河巨神无效魔法.");
                return LastChainPlayer == 1;
            }
            return true;
        }

        private bool 太古的白石效果()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.太古的白石, 0))
            {
                Logger.DebugWriteLine("太古白石回收效果.");
                if (Duel.Fields[0].HasInHand((int)CardId.抵价购物)
                    && !Duel.Fields[0].HasInHand((int)CardId.青眼白龙)
                    && !Duel.Fields[0].HasInHand((int)CardId.青眼亚白龙))
                {
                    Logger.DebugWriteLine("回收喂八抽.");
                    AI.SelectCard((int)CardId.青眼白龙);
                    return true;
                }
                if (已特殊召唤青眼亚白龙)
                {
                    Logger.DebugWriteLine("已经跳过亚白龙，下回合再回收.");
                    return false;
                }
                if (Duel.Fields[0].HasInHand((int)CardId.青眼白龙)
                    && !Duel.Fields[0].HasInHand((int)CardId.青眼亚白龙)
                    && Duel.Fields[0].HasInGraveyard((int)CardId.青眼亚白龙))
                {
                    Logger.DebugWriteLine("缺亚白龙，回收.");
                    AI.SelectCard((int)CardId.青眼亚白龙);
                    return true;
                }
                if (Duel.Fields[0].HasInHand((int)CardId.青眼亚白龙)
                    && !Duel.Fields[0].HasInHand((int)CardId.青眼白龙)
                    && Duel.Fields[0].HasInGraveyard((int)CardId.青眼白龙))
                {
                    Logger.DebugWriteLine("有亚白龙缺本体，回收.");
                    AI.SelectCard((int)CardId.青眼白龙);
                    return true;
                }
                Logger.DebugWriteLine("并没有应该回收的.");
                return false;
            }
            else
            {
                Logger.DebugWriteLine("太古白石特招效果.");
                List<ClientCard> spells = Duel.Fields[1].GetSpells();
                if (spells.Count == 0)
                {
                    Logger.DebugWriteLine("对面没坑，跳个本体.");
                    AI.SelectCard((int)CardId.青眼白龙);
                    //AI.SelectCard((int)CardId.白色灵龙);
                }
                else
                {
                    Logger.DebugWriteLine("对面有坑，拆.");
                    AI.SelectCard((int)CardId.白色灵龙);
                }
                return true;
            }
        }

        private bool 青眼亚白龙特殊召唤()
        {
            已特殊召唤青眼亚白龙 = true;
            return true;
        }

        private bool 太古的白石通常召唤()
        {
            Logger.DebugWriteLine("1星怪兽通常召唤.");
            return Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.青色眼睛的贤士,
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石,
                    (int)CardId.青眼亚白龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙
                });
        }

        private bool 银河眼光波龙超量召唤()
        {
            Logger.DebugWriteLine("银河眼光波龙超量召唤.");
            if (Duel.Turn == 1)
            {
                Logger.DebugWriteLine("先攻不叠银河眼，叠银河巨神.");
                return false;
            }
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            if (monsters.Count == 1 && !monsters[0].IsFacedown() && ((monsters[0].IsDefense() && monsters[0].GetDefensePower() >= 3000) && monsters[0].HasType(CardType.Xyz)))
            {
                Logger.DebugWriteLine("只有一个大怪兽，光波龙抢之.");
                return true;
            }
            if (monsters.Count >= 3)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.IsFacedown() && ((monster.IsDefense() && monster.GetDefensePower() >= 3000) || monster.HasType(CardType.Xyz)))
                    {
                        Logger.DebugWriteLine("貌似打不死，出个光波龙看看.");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool 银河眼光子龙皇超量召唤()
        {
            Logger.DebugWriteLine("银河眼光子龙皇超量召唤.");
            if (Duel.Turn == 1)
            {
                Logger.DebugWriteLine("先攻不叠银河眼，叠银河巨神.");
                return false;
            }
            if (AI.Utils.IsOneEnnemyBetterThanValue(2999, false))
            {
                Logger.DebugWriteLine("有高攻怪兽，出银河眼.");
                return true;
            }
            return false;
        }

        private bool 银河眼重铠光子龙超量召唤()
        {
            Logger.DebugWriteLine("银河眼重铠光子龙超量召唤.");
            if (Duel.Fields[0].HasInMonstersZone((int)CardId.银河眼光波龙))
            {
                List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if ((monster.IsDisabled() && monster.HasType(CardType.Xyz) && !monster.Equals(使用过的光波龙))
                        || (Duel.Phase == DuelPhase.Main2 && monster.Equals(使用过的光波龙)))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            if (Duel.Fields[0].HasInMonstersZone((int)CardId.银河眼光子龙皇))
            {
                if (!AI.Utils.IsOneEnnemyBetterThanValue(4000, false))
                {
                    Logger.DebugWriteLine("没有高攻怪兽，出重铠.");
                    AI.SelectCard((int)CardId.银河眼光子龙皇);
                    return true;
                }
            }
            return false;
        }

        private bool 银河眼光波刃龙超量召唤()
        {
            Logger.DebugWriteLine("银河眼光波刃龙超量召唤.");
            if (Duel.Fields[0].HasInMonstersZone((int)CardId.银河眼重铠光子龙) && AI.Utils.GetProblematicCard() != null)
            {
                AI.SelectCard((int)CardId.银河眼重铠光子龙);
                return true;
            }
            return false;
        }

        private bool 银河眼暗物质龙超量召唤()
        {
            Logger.DebugWriteLine("银河眼暗物质龙超量召唤.");
            if (Duel.Fields[0].HasInMonstersZone((int)CardId.银河眼重铠光子龙))
            {
                AI.SelectCard((int)CardId.银河眼重铠光子龙);
                return true;
            }
            return false;
        }

        private bool 银河眼光子龙皇效果()
        {
            return true;
        }

        private bool 银河眼光波龙效果()
        {
            Logger.DebugWriteLine("银河眼光波龙效果.");
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                {
                    AI.SelectCard(monster);
                    使用过的光波龙 = Card;
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsDefense())
                {
                    AI.SelectCard(monster);
                    使用过的光波龙 = Card;
                    return true;
                }
            }
            使用过的光波龙 = Card;
            return true;
        }

        private bool 银河眼重铠光子龙效果()
        {
            Logger.DebugWriteLine("重铠优先炸后场.");
            ClientCard floodgate = Duel.Fields[1].SpellZone.GetFloodgate();
            if (floodgate != null)
            {
                AI.SelectCard(floodgate);
                return true;
            }
            floodgate = Duel.Fields[1].MonsterZone.GetFloodgate();
            if (floodgate != null)
            {
                AI.SelectCard(floodgate);
                return true;
            }
            List<ClientCard> spells = Duel.Fields[1].GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (!spell.IsFacedown())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            if (monsters.Count >= 2)
            {
                Logger.DebugWriteLine("怪多就先炸守备的.");
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsDefense())
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                return true;
            }
            if (monsters.Count == 2)
            {
                Logger.DebugWriteLine("2只怪只炸打不过的，剩下留给暗物质打.");
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsMonsterInvincible() || monster.IsMonsterDangerous() || monster.GetDefensePower() > 4000)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            if (monsters.Count == 1)
            {
                return true;
            }
            return false;
        }

        private bool 银河眼光波刃龙效果()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            Logger.DebugWriteLine("光波刃龙优先炸前场.");
            ClientCard target = AI.Utils.GetProblematicCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> monsters = Duel.Fields[1].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsDefense())
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
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

        private bool 银河眼暗物质龙效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石,
                    (int)CardId.白色灵龙,
                    (int)CardId.青眼白龙
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石,
                    (int)CardId.白色灵龙,
                    (int)CardId.青眼白龙
                });
            return true;
        }

        private bool 鬼岩城同调召唤()
        {
            if (Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1 || 已发动灵魂补充)
                return false;
            int bestSelfAttack = AI.Utils.GetBestAttack(Duel.Fields[0], false);
            int bestEnemyAttack = AI.Utils.GetBestAttack(Duel.Fields[1], false);
            return bestSelfAttack <= bestEnemyAttack && bestEnemyAttack > 2500 && bestEnemyAttack <= 3100;
        }

        private bool 青眼精灵龙同调召唤()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                Logger.DebugWriteLine("主阶段1同调精灵龙.");
                if (使用过的青眼亚白龙.Count > 0)
                {
                    Logger.DebugWriteLine("有用过的亚白需要同调.");
                    return true;
                }
                if (Duel.Turn == 1 || 已发动灵魂补充)
                {
                    Logger.DebugWriteLine("先攻同调.");
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                Logger.DebugWriteLine("主阶段2同调精灵龙.");
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 希望魁龙银河巨神超量召唤()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                Logger.DebugWriteLine("主阶段1超量银河巨神.");
                if (使用过的青眼亚白龙.Count > 0)
                {
                    Logger.DebugWriteLine("有用过的亚白可以叠.");
                    return true;
                }
                if (Duel.Turn == 1 || 已发动灵魂补充)
                {
                    Logger.DebugWriteLine("先攻超量银河巨神.");
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                Logger.DebugWriteLine("主阶段2超量银河巨神.");
                return true;
            }
            return false;
        }

        private bool 森罗的姬芽宫超量召唤()
        {
            if (Duel.Turn == 1)
            {
                Logger.DebugWriteLine("先攻可以超量森罗的姬芽宫.");
                return true;
            }
            if (Duel.Phase == DuelPhase.Main1 && !Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.青眼亚白龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙
                }))
            {
                Logger.DebugWriteLine("不能出L9，只能叠森罗的姬芽宫.");
                return true;
            }
            if (Duel.Phase == DuelPhase.Main2 || 已发动灵魂补充)
            {
                Logger.DebugWriteLine("主阶段2超量森罗的姬芽宫.");
                return true;
            }
            return false;
        }

        private bool 森罗的姬芽宫效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.传说的白石,
                    (int)CardId.太古的白石
                });
            return true;
        }

        private bool 灵魂补充()
        {
            Logger.DebugWriteLine("灵魂补充.");
            if (Duel.Fields[0].HasInMonstersZone((int)CardId.青眼精灵龙, true))
                return false;
            int count = Duel.Fields[0].GetGraveyardMonsters().Count;
            int space = 5 - Duel.Fields[0].GetMonsterCount();
            if (count < space)
                count = space;
            if (count < 2 || Duel.LifePoints[0] < count*1000)
                return false;
            if (Duel.Turn != 1)
            {
                int attack = 0;
                int defence = 0;
                List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.IsDefense())
                    {
                        attack += monster.Attack;
                    }
                }
                monsters = Duel.Fields[1].GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    defence += monster.GetDefensePower();
                }
                if (attack - defence > Duel.LifePoints[1])
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.青眼精灵龙,
                    (int)CardId.希望魁龙银河巨神,
                    (int)CardId.青眼亚白龙,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙,
                    (int)CardId.苍眼银龙,
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石
                });
            已发动灵魂补充 = true;
            return true;
        }

        private bool 改变攻守表示()
        {
            bool ennemyBetter = AI.Utils.IsEnnemyBetter(true, true);

            if (Card.IsAttack() && ennemyBetter)
                return true;
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !ennemyBetter && Card.Attack >= Card.Defense)
                return true;
            if (Card.IsDefense() && (
                   Card.Id == (int)CardId.青眼精灵龙
                || Card.Id == (int)CardId.苍眼银龙
                ))
                return true;
            if (Card.IsAttack() && (
                   Card.Id == (int)CardId.青色眼睛的贤士
                || Card.Id == (int)CardId.太古的白石
                || Card.Id == (int)CardId.传说的白石
                ))
                return true;
            return false;
        }

        private bool 盖卡()
        {
            return (Card.IsTrap() || (Card.Id==(int)CardId.银龙的轰咆)) && Duel.Fields[0].GetSpellCountWithoutField() < 4;
        }

        private bool 手里有2个(int id)
        {
            int num = 0;
            foreach (ClientCard card in Duel.Fields[0].Hand)
            {
                if (card != null && card.Id == id)
                    num++;
            }
            return num >= 2;
        }

        private bool 能处理青眼亚白龙()
        {
            return Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.青色眼睛的贤士,
                    (int)CardId.太古的白石,
                    (int)CardId.传说的白石,
                    (int)CardId.青眼白龙,
                    (int)CardId.白色灵龙
                }) || Duel.Fields[0].GetCountCardInZone(Duel.Fields[0].MonsterZone, (int)CardId.青眼亚白龙)>=2 ;
        }

        private bool 手里有足够的青眼白龙()
        {
            return 手里有2个((int)CardId.青眼白龙) || (
                Duel.Fields[0].HasInGraveyard((int)CardId.青眼白龙)
                && Duel.Fields[0].HasInGraveyard((int)CardId.太古的白石)
                );
        }
    }
}
