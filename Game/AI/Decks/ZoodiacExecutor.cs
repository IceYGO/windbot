using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace MycardBot.Game.AI.Decks
{
    [Deck("Zoodiac", "AI_Zoodiac")]
    class ZoodiacExecutor : DefaultExecutor
    {
        public enum CardId
        {
            坏星坏兽席兹奇埃鲁 = 63941210,
            怪粉坏兽加达拉 = 36956512,
            海龟坏兽加美西耶勒 = 55063751,
            多次元坏兽拉迪安 = 28674152,
            黏丝坏兽库莫古斯 = 29726552,
            光子斩击者 = 65367484,
            十二兽马剑 = 77150143,
            十二兽蛇笞 = 31755044,
            召唤师阿莱斯特 = 86120751,
            十二兽鼠骑 = 78872731,
            鹰身女妖的羽毛扫 = 18144506,
            黑洞 = 53129443,
            星球改造 = 73628505,
            召唤魔术 = 74063034,
            死者苏生 = 83764718,
            遭受妨碍的坏兽安眠 = 99330325,
            十二兽的会局 = 46060017,
            炎舞天玑 = 57103969,
            暴走魔法阵 = 47679935,
            十二兽的方合 = 73881652,
            召唤兽梅尔卡巴 = 75286621,
            召唤兽墨瓦腊泥加 = 48791583,
            闪光No39希望皇霍普电光皇 = 56832966,
            No39希望皇霍普 = 84013237,
            大薰风骑士翠玉 = 581014,
            十二兽虎炮 = 11510448,
            十二兽狗环 = 41375811,
            十二兽龙枪 = 48905153,
            十二兽牛犄 = 85115440,

            雷击坏兽雷鸣龙王 = 48770333,
            怒炎坏兽多哥兰 = 93332803,
            对坏兽用决战兵器超级机械多哥兰 = 84769941
        }

        bool 已特殊召唤虎炮 = false;
        bool 已特殊召唤狗环 = false;
        bool 已特殊召唤牛犄 = false;
        int 蛇笞发动次数 = 0;

        public ZoodiacExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);
            AddExecutor(ExecutorType.Activate, (int)CardId.遭受妨碍的坏兽安眠, 遭受妨碍的坏兽安眠效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.海龟坏兽加美西耶勒, 坏兽特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.黏丝坏兽库莫古斯, 坏兽特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.怪粉坏兽加达拉, 坏兽特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.多次元坏兽拉迪安, 坏兽特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.坏星坏兽席兹奇埃鲁, 坏兽特殊召唤);

            AddExecutor(ExecutorType.Activate, (int)CardId.星球改造);
            AddExecutor(ExecutorType.Activate, (int)CardId.暴走魔法阵);
            AddExecutor(ExecutorType.Activate, (int)CardId.炎舞天玑, 炎舞天玑效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽的会局, 十二兽的会局效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.大薰风骑士翠玉);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.光子斩击者, 光子斩击者特殊召唤);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.No39希望皇霍普, 电光皇特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.闪光No39希望皇霍普电光皇);
            AddExecutor(ExecutorType.Activate, (int)CardId.闪光No39希望皇霍普电光皇);

            AddExecutor(ExecutorType.Activate, (int)CardId.召唤兽梅尔卡巴, DefaultTrap);

            AddExecutor(ExecutorType.Activate, 十二兽鼠骑素材效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽龙枪, 十二兽龙枪效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽牛犄, 十二兽牛犄效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽虎炮, 十二兽虎炮效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽狗环, 十二兽狗环效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.十二兽狗环, 十二兽狗环特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.十二兽虎炮, 十二兽虎炮特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.十二兽牛犄, 十二兽牛犄特殊召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.十二兽龙枪, 十二兽龙枪特殊召唤);

            AddExecutor(ExecutorType.Summon, (int)CardId.十二兽鼠骑);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽鼠骑, 十二兽鼠骑效果);
            AddExecutor(ExecutorType.Summon, (int)CardId.十二兽马剑);
            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽马剑, 十二兽鼠骑效果);
            AddExecutor(ExecutorType.Summon, (int)CardId.召唤师阿莱斯特);
            AddExecutor(ExecutorType.Activate, (int)CardId.召唤师阿莱斯特, 召唤师阿莱斯特效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.大薰风骑士翠玉, 大薰风骑士翠玉特殊召唤);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.十二兽牛犄, 十二兽牛犄超量召唤);

            AddExecutor(ExecutorType.Activate, (int)CardId.死者苏生, 死者苏生效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.光子斩击者);
            AddExecutor(ExecutorType.Summon, (int)CardId.十二兽蛇笞);

            AddExecutor(ExecutorType.Activate, (int)CardId.召唤魔术, 召唤魔术效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽蛇笞, 十二兽蛇笞效果);

            AddExecutor(ExecutorType.Activate, (int)CardId.十二兽的方合, 十二兽的方合效果);

            AddExecutor(ExecutorType.SpellSet, (int)CardId.十二兽的方合);

            AddExecutor(ExecutorType.Repos, MonsterRepos);
        }

        public override bool OnSelectHand()
        {
            // 抢先攻
            return true;
        }

        public override void OnNewTurn()
        {
            // 回合开始时重置状况
            已特殊召唤虎炮 = false;
            已特殊召唤狗环 = false;
            已特殊召唤牛犄 = false;
            蛇笞发动次数 = 0;
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
                //if (attacker.HasType(CardType.Fusion) && Bot.HasInHand((int)CardId.召唤师阿莱斯特))
                //    attacker.RealPower = attacker.RealPower + 1000;
                if (attacker.Id == (int)CardId.闪光No39希望皇霍普电光皇 && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.No39希望皇霍普))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool 遭受妨碍的坏兽安眠效果()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.海龟坏兽加美西耶勒,
                    (int)CardId.黏丝坏兽库莫古斯,
                    (int)CardId.多次元坏兽拉迪安,
                    (int)CardId.怪粉坏兽加达拉
                });
                return true;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.坏星坏兽席兹奇埃鲁,
                    (int)CardId.多次元坏兽拉迪安,
                    (int)CardId.怪粉坏兽加达拉,
                    (int)CardId.黏丝坏兽库莫古斯
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.海龟坏兽加美西耶勒,
                    (int)CardId.黏丝坏兽库莫古斯,
                    (int)CardId.怪粉坏兽加达拉,
                    (int)CardId.多次元坏兽拉迪安
                });
            return DefaultDarkHole();
        }

        private bool 坏兽特殊召唤()
        {
            IList<int> kaijus = new[] {
                (int)CardId.坏星坏兽席兹奇埃鲁,
                (int)CardId.怪粉坏兽加达拉,
                (int)CardId.海龟坏兽加美西耶勒,
                (int)CardId.多次元坏兽拉迪安,
                (int)CardId.黏丝坏兽库莫古斯,
                (int)CardId.雷击坏兽雷鸣龙王,
                (int)CardId.怒炎坏兽多哥兰,
                (int)CardId.对坏兽用决战兵器超级机械多哥兰
            };
            foreach (ClientCard monster in Enemy.GetMonsters())
            {
                if (kaijus.Contains(monster.Id))
                    return Card.GetDefensePower() > monster.GetDefensePower();
            }
            ClientCard card = Enemy.MonsterZone.GetFloodgate();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = Enemy.MonsterZone.GetDangerousMonster();
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            card = AI.Utils.GetOneEnemyBetterThanValue(Card.GetDefensePower(), false);
            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }
            return false;
        }

        private bool 电光皇特殊召唤()
        {
            int selfBestAttack = AI.Utils.GetBestAttack(Bot, true);
            int oppoBestAttack = AI.Utils.GetBestAttack(Enemy, false);
            return selfBestAttack < oppoBestAttack;
        }

        private bool 光子斩击者特殊召唤()
        {
            return Bot.HasInHand((int)CardId.召唤师阿莱斯特)
                && !Bot.HasInHand((int)CardId.十二兽鼠骑)
                && !Bot.HasInHand((int)CardId.十二兽马剑);
        }

        private bool 召唤师阿莱斯特效果()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (!(Duel.Phase == DuelPhase.BattleStep
                    || Duel.Phase == DuelPhase.BattleStart
                    || Duel.Phase == DuelPhase.Damage))
                    return false;
                return Duel.Player==0
                    || AI.Utils.IsEnemyBetter(false, false);
            }
            return true;
        }

        private bool 召唤魔术效果()
        {
            if (Card.Location == CardLocation.Grave)
                return true;
            IList<ClientCard> materials0 = Bot.Graveyard;
            IList<ClientCard> materials1 = Enemy.Graveyard;
            ClientCard mat = null;
            foreach (ClientCard card in materials0)
            {
                if (card.HasAttribute(CardAttribute.Light))
                {
                    mat = card;
                    break;
                }
            }
            foreach (ClientCard card in materials1)
            {
                if (card.HasAttribute(CardAttribute.Light))
                {
                    mat = card;
                    break;
                }
            }
            if (mat != null)
            {
                AI.SelectCard((int)CardId.召唤兽梅尔卡巴);
                选择墓地里的召唤师();
                AI.SelectThirdCard(mat);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            foreach (ClientCard card in materials0)
            {
                if (card.HasAttribute(CardAttribute.Earth))
                {
                    mat = card;
                    break;
                }
            }
            foreach (ClientCard card in materials1)
            {
                if (card.HasAttribute(CardAttribute.Earth))
                {
                    mat = card;
                    break;
                }
            }
            if (mat != null)
            {
                AI.SelectCard((int)CardId.召唤兽墨瓦腊泥加);
                选择墓地里的召唤师();
                AI.SelectThirdCard(mat);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }
            return false;
        }

        private void 选择墓地里的召唤师()
        {
            IList<ClientCard> materials0 = Bot.Graveyard;
            IList<ClientCard> materials1 = Enemy.Graveyard;
            foreach (ClientCard card in materials1)
            {
                if (card.Id == (int)CardId.召唤师阿莱斯特)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            foreach (ClientCard card in materials0)
            {
                if (card.Id == (int)CardId.召唤师阿莱斯特)
                {
                    AI.SelectNextCard(card);
                    return;
                }
            }
            AI.SelectNextCard((int)CardId.召唤师阿莱斯特);
        }

        private bool 十二兽狗环特殊召唤()
        {
            if (Bot.HasInMonstersZone((int)CardId.十二兽鼠骑) && !已特殊召唤狗环)
            {
                AI.SelectCard((int)CardId.十二兽鼠骑);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤狗环 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽牛犄) && !已特殊召唤狗环)
            {
                AI.SelectCard((int)CardId.十二兽牛犄);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤狗环 = true;
                return true;
            }
            return false;
        }

        private bool 十二兽狗环效果()
        {
            if (Bot.HasInGraveyard((int)CardId.十二兽蛇笞) || Bot.HasInGraveyard((int)CardId.十二兽马剑))
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.十二兽牛犄,
                    (int)CardId.十二兽虎炮,
                    (int)CardId.十二兽狗环,
                    (int)CardId.十二兽马剑,
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽蛇笞
                });
                AI.SelectNextCard(new[]
                {
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.十二兽马剑
                });
                return true;
            }
            return false;
        }

        private bool 十二兽虎炮特殊召唤()
        {
            if (Bot.HasInMonstersZone((int)CardId.十二兽狗环) && !已特殊召唤虎炮)
            {
                AI.SelectCard((int)CardId.十二兽狗环);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤虎炮 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽鼠骑) && !已特殊召唤虎炮)
            {
                AI.SelectCard((int)CardId.十二兽鼠骑);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤虎炮 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽马剑) && !已特殊召唤虎炮
                && Bot.HasInGraveyard(new List<int>
                {
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.十二兽鼠骑
                }))
            {
                AI.SelectCard((int)CardId.十二兽马剑);
                AI.SelectYesNo(true);
                已特殊召唤虎炮 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽蛇笞) && !已特殊召唤虎炮
                && Bot.HasInGraveyard((int)CardId.十二兽鼠骑))
            {
                AI.SelectCard((int)CardId.十二兽蛇笞);
                AI.SelectYesNo(true);
                已特殊召唤虎炮 = true;
                return true;
            }
            return false;
        }

        private bool 十二兽虎炮效果()
        {
            //if (Card.HasXyzMaterial((int)CardId.十二兽鼠骑) || !Bot.HasInGraveyard((int)CardId.十二兽鼠骑))
            //    return false;
            AI.SelectCard((int)CardId.十二兽狗环);
            AI.SelectNextCard((int)CardId.十二兽虎炮);
            AI.SelectThirdCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.十二兽马剑
                });
            return true;
        }

        private bool 十二兽牛犄特殊召唤()
        {
            if (Bot.HasInMonstersZone((int)CardId.十二兽虎炮) && !已特殊召唤牛犄)
            {
                AI.SelectCard((int)CardId.十二兽虎炮);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤牛犄 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽狗环) && !已特殊召唤牛犄)
            {
                AI.SelectCard((int)CardId.十二兽狗环);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤牛犄 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽鼠骑) && !已特殊召唤牛犄)
            {
                AI.SelectCard((int)CardId.十二兽鼠骑);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤牛犄 = true;
                return true;
            }
            if (Bot.HasInMonstersZone((int)CardId.十二兽马剑) && !已特殊召唤牛犄)
            {
                AI.SelectCard((int)CardId.十二兽马剑);
                AI.SelectYesNo(true);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                已特殊召唤牛犄 = true;
                return true;
            }
            return false;
        }

        private bool 十二兽牛犄效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽虎炮,
                    (int)CardId.十二兽狗环,
                    (int)CardId.十二兽龙枪,
                    (int)CardId.召唤师阿莱斯特,
                    (int)CardId.光子斩击者
                });
            if (Bot.HasInHand((int)CardId.十二兽蛇笞) && !Bot.HasInHand((int)CardId.十二兽鼠骑))
                AI.SelectNextCard((int)CardId.十二兽鼠骑);
            else
                AI.SelectNextCard((int)CardId.十二兽蛇笞);
            return true;
        }

        private bool 十二兽牛犄超量召唤()
        {
            AI.SelectYesNo(false);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.光子斩击者,
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.召唤师阿莱斯特
                });
            return true;
        }

        private bool 十二兽龙枪特殊召唤()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽牛犄,
                    (int)CardId.十二兽虎炮,
                    (int)CardId.十二兽狗环,
                    (int)CardId.十二兽马剑
                });
            return true;
        }

        private bool 十二兽鼠骑素材效果()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.十二兽鼠骑, 1))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 十二兽蛇笞效果()
        {
            if (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
                return false;
            if (Card.IsDisabled() || 蛇笞发动次数 >= 3)
                return false;
            ClientCard target = null;
            List<ClientCard> monsters = Bot.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsFaceup() && monster.Id == (int)CardId.十二兽龙枪 && !monster.HasXyzMaterial())
                {
                    target = monster;
                    break;
                }
            }
            /*if (target == null)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsFaceup() && monster.Type == (int)CardType.Xyz && monster.Id != (int)CardId.大薰风骑士翠玉 && !monster.HasXyzMaterial())
                    {
                        target = monster;
                        break;
                    }
                }
            }*/
            if (target == null)
            {
                AI.SelectCard(new[]
                    {
                        (int)CardId.十二兽龙枪
                    });
            }
            蛇笞发动次数++;
            return true;
        }

        private bool 十二兽鼠骑效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽的方合,
                    (int)CardId.十二兽马剑,
                    (int)CardId.十二兽的会局
                });
            return true;
        }

        private bool 十二兽龙枪效果()
        {
            if (LastChainPlayer == 0)
                return false;
            ClientCard target = AI.Utils.GetProblematicCard();
            if (target == null)
            {
                List<ClientCard> monsters = Enemy.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsFaceup())
                    {
                        target=monster;
                        break;
                    }
                }
            }
            if (target == null)
            {
                List<ClientCard> spells = Enemy.GetSpells();
                foreach (ClientCard spell in spells)
                {
                    if (spell.IsFaceup() && spell.IsSpellNegateAttack())
                    {
                        target = spell;
                        break;
                    }
                }
            }
            if (target == null)
            {
                List<ClientCard> spells = Enemy.GetSpells();
                foreach (ClientCard spell in spells)
                {
                    if (spell.IsFaceup() && (spell.HasType(CardType.Continuous)
                        || spell.HasType(CardType.Equip)
                        || spell.HasType(CardType.Field)
                        || spell.HasType(CardType.Pendulum)))
                    {
                        target = spell;
                        break;
                    }
                }
            }
            if (target == null)
                return false;
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽牛犄,
                    (int)CardId.十二兽虎炮,
                    (int)CardId.十二兽狗环,
                    (int)CardId.十二兽马剑,
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽蛇笞
                });
            AI.SelectNextCard(target);
            return true;
        }

        private bool 大薰风骑士翠玉特殊召唤()
        {
            return Bot.GetGraveyardMonsters().Count >= 3;
        }

        private bool 大薰风骑士翠玉效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.召唤师阿莱斯特,
                    (int)CardId.十二兽蛇笞
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.大薰风骑士翠玉
                });
            return true;
        }

        private bool 炎舞天玑效果()
        {
            if (Bot.HasInHand((int)CardId.十二兽的会局)
               || Bot.HasInSpellZone((int)CardId.十二兽的会局)
               || Bot.HasInHand((int)CardId.十二兽鼠骑))
            {
                AI.SelectCard((int)CardId.十二兽蛇笞);
            }
            else
            {
                AI.SelectCard((int)CardId.十二兽鼠骑);
            }
            AI.SelectYesNo(true);
            return true;
        }

        private bool 十二兽的会局效果()
        {
            IList<ClientCard> spells = Bot.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.Id == (int)CardId.十二兽的会局 && !Card.Equals(spell))
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.炎舞天玑,
                    (int)CardId.暴走魔法阵,
                    (int)CardId.十二兽的会局
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.十二兽马剑
                });
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 十二兽的方合效果()
        {
            if (CurrentChain.Count > 0)
                return false;
            if (Card.Location != CardLocation.Grave)
            {
                AI.SelectCard((int)CardId.十二兽龙枪);
                AI.SelectNextCard(new[]
                {
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽马剑
                });
            }
            return true;
        }

        private bool 死者苏生效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.十二兽鼠骑,
                    (int)CardId.十二兽蛇笞,
                    (int)CardId.召唤兽梅尔卡巴,
                    (int)CardId.坏星坏兽席兹奇埃鲁,
                    (int)CardId.召唤兽墨瓦腊泥加,
                    (int)CardId.十二兽虎炮,
                    (int)CardId.十二兽狗环,
                    (int)CardId.十二兽牛犄
                });
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.Id == (int)CardId.闪光No39希望皇霍普电光皇)
                return false;
            return base.DefaultMonsterRepos();
        }
    }
}
