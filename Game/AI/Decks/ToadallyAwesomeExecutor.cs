using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("ToadallyAwesome", "AI_ToadallyAwesome")]
    public class ToadallyAwesomeExecutor : DefaultExecutor
    {
        public enum CardId
        {
            冰结界的术者 = 23950192,
            冰结界的水影 = 90311614,
            鬼青蛙 = 9126351,
            冰结界的传道师 = 50088247,
            粹蛙 = 1357146,
            魔知青蛙 = 46239604,
            小灰篮史莱姆 = 80250319,
            银河旋风 = 5133471,
            鹰身女妖的羽毛扫 = 18144506,
            浮上 = 33057951,
            黑洞 = 53129443,
            手札抹杀 = 72892473,
            愚蠢的埋葬 = 81439173,
            死者苏生 = 83764718,
            冰结界的纹章 = 84206435,
            海上打捞 = 96947648,
            水舞台 = 29047353,
            虹光之宣告者 = 79606837,
            饼蛙 = 90809975,
            神骑矢车菊圣人马 = 36776089,
            大薰风凤凰 = 2766877,
            猫鲨 = 84224627,

            旋风 = 5318639,
            月之书 = 14087893,
            活死人的呼声 = 97077563,
            激流葬 = 53582587,

            闪光No39希望皇霍普电光皇 = 56832966
        }

        public ToadallyAwesomeExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, (int)CardId.银河旋风, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.鹰身女妖的羽毛扫);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);

            AddExecutor(ExecutorType.Activate, (int)CardId.水舞台, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.冰结界的纹章, 冰结界的纹章效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.愚蠢的埋葬, 愚蠢的埋葬效果);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.冰结界的传道师);
            AddExecutor(ExecutorType.Summon, (int)CardId.小灰篮史莱姆, 小灰篮史莱姆优先通常召唤);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.鬼青蛙, 鬼青蛙特殊召唤);

            AddExecutor(ExecutorType.Activate, (int)CardId.鬼青蛙, 鬼青蛙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.小灰篮史莱姆, 小灰篮史莱姆效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.粹蛙, 粹蛙效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.冰结界的传道师);
            AddExecutor(ExecutorType.Activate, (int)CardId.魔知青蛙);

            AddExecutor(ExecutorType.Activate, (int)CardId.浮上, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.死者苏生, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.海上打捞, 海上打捞效果);

            AddExecutor(ExecutorType.Summon, (int)CardId.鬼青蛙);
            AddExecutor(ExecutorType.Summon, (int)CardId.冰结界的水影, 冰结界下级通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.冰结界的术者, 冰结界下级通常召唤);

            AddExecutor(ExecutorType.Activate, (int)CardId.手札抹杀);

            AddExecutor(ExecutorType.Summon, (int)CardId.小灰篮史莱姆, 低攻怪兽通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.冰结界的传道师, 低攻怪兽通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.粹蛙, 低攻怪兽通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.魔知青蛙, 低攻怪兽通常召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.冰结界的传道师, 冰结界的传道师通常召唤);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.猫鲨, 猫鲨特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.猫鲨, 猫鲨效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.神骑矢车菊圣人马, 神骑矢车菊圣人马特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.神骑矢车菊圣人马);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.大薰风凤凰, 大薰风凤凰特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.大薰风凤凰);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.饼蛙);
            AddExecutor(ExecutorType.Activate, (int)CardId.饼蛙, 饼蛙效果);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.虹光之宣告者, 虹光之宣告者特殊召唤);
            AddExecutor(ExecutorType.Activate, (int)CardId.虹光之宣告者);

            AddExecutor(ExecutorType.MonsterSet, (int)CardId.小灰篮史莱姆);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.魔知青蛙);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.粹蛙);

            AddExecutor(ExecutorType.Repos, 改变攻守表示);
            // 饼蛙抢来的卡的发动
            AddExecutor(ExecutorType.Activate, (int)CardId.旋风, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.月之书, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.活死人的呼声, 死者苏生效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.激流葬, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, 其他魔法发动);
            AddExecutor(ExecutorType.Activate, 其他陷阱发动);
            AddExecutor(ExecutorType.Activate, 其他怪兽发动);
        }

        public override bool OnSelectHand()
        {
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
                if (attacker.Id == (int)CardId.神骑矢车菊圣人马 && !attacker.IsDisabled() && attacker.HasXyzMaterial())
                    attacker.RealPower = Duel.LifePoints[0] + attacker.Attack;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool 冰结界的纹章效果()
        {
            if (Duel.Fields[0].HasInHand(new List<int>
                {
                    (int)CardId.冰结界的术者,
                    (int)CardId.冰结界的水影
                }) || Duel.Fields[0].HasInMonstersZone(new List<int>
                {
                    (int)CardId.冰结界的术者,
                    (int)CardId.冰结界的水影
                }))
            {
                AI.SelectCard((int)CardId.冰结界的传道师);
            }
            else
            {
                AI.SelectCard(new[]
                    {
                    (int)CardId.冰结界的术者,
                    (int)CardId.冰结界的水影
                });
            }
            return true;
        }

        private bool 死者苏生效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.饼蛙,
                    (int)CardId.虹光之宣告者,
                    (int)CardId.鬼青蛙,
                    (int)CardId.冰结界的水影,
                    (int)CardId.冰结界的术者,
                    (int)CardId.魔知青蛙,
                    (int)CardId.粹蛙,
                    (int)CardId.小灰篮史莱姆
                });
            return true;
        }

        private bool 愚蠢的埋葬效果()
        {
            if (Duel.Fields[0].HasInHand((int)CardId.小灰篮史莱姆) && !Duel.Fields[0].HasInGraveyard((int)CardId.小灰篮史莱姆))
                AI.SelectCard((int)CardId.小灰篮史莱姆);
            else if (Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙))
                AI.SelectCard((int)CardId.魔知青蛙);
            else if (Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙))
                AI.SelectCard((int)CardId.粹蛙);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.小灰篮史莱姆,
                        (int)CardId.粹蛙,
                        (int)CardId.魔知青蛙,
                        (int)CardId.冰结界的术者,
                        (int)CardId.冰结界的水影,
                        (int)CardId.冰结界的传道师,
                        (int)CardId.鬼青蛙
                    });
            return true;
        }

        private bool 海上打捞效果()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.鬼青蛙,
                    (int)CardId.冰结界的传道师,
                    (int)CardId.小灰篮史莱姆
                });
            return true;
        }

        private bool 鬼青蛙特殊召唤()
        {
            if (Duel.Fields[0].GetCountCardInZone(Duel.Fields[0].Hand, (int)CardId.小灰篮史莱姆)>=2 && !Duel.Fields[0].HasInGraveyard((int)CardId.小灰篮史莱姆))
                AI.SelectCard((int)CardId.小灰篮史莱姆);
            else if (Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙))
                AI.SelectCard((int)CardId.魔知青蛙);
            else if (Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙))
                AI.SelectCard((int)CardId.粹蛙);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.粹蛙,
                        (int)CardId.魔知青蛙,
                        (int)CardId.冰结界的术者,
                        (int)CardId.冰结界的水影,
                        (int)CardId.冰结界的传道师,
                        (int)CardId.小灰篮史莱姆,
                        (int)CardId.鬼青蛙
                    });
            return true;
        }

        private bool 鬼青蛙效果()
        {
            if (ActivateDescription == -1)
            {
                return 愚蠢的埋葬效果();
            }
            else
            {
                if (Duel.Fields[0].HasInHand((int)CardId.魔知青蛙))
                {
                    AI.SelectCard(new[]
                        {
                            (int)CardId.冰结界的传道师,
                            (int)CardId.小灰篮史莱姆,
                            (int)CardId.鬼青蛙
                        });
                    return true;
                }
            }
            return false;
        }

        private bool 小灰篮史莱姆优先通常召唤()
        {
            return Duel.Fields[0].HasInGraveyard((int)CardId.小灰篮史莱姆);
        }

        private bool 小灰篮史莱姆效果()
        {
            AI.SelectCard((int)CardId.小灰篮史莱姆);
            AI.SelectNextCard(new[]
                {
                    (int)CardId.鬼青蛙,
                    (int)CardId.冰结界的术者,
                    (int)CardId.冰结界的水影,
                    (int)CardId.粹蛙,
                    (int)CardId.魔知青蛙,
                    (int)CardId.冰结界的传道师,
                    (int)CardId.小灰篮史莱姆
                });
            return true;
        }

        private bool 粹蛙效果()
        {
            if (低攻怪兽通常召唤())
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 低攻怪兽通常召唤()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Level==2)
                {
                    return true;
                }
            }
            return false;
        }

        private bool 冰结界下级通常召唤()
        {
            return Duel.Fields[0].GetCountCardInZone(Duel.Fields[0].Hand, (int)CardId.冰结界的传道师) > 0;
        }

        private bool 冰结界的传道师通常召唤()
        {
            return Duel.Fields[0].GetCountCardInZone(Duel.Fields[0].Hand, (int)CardId.冰结界的传道师) >= 2;
        }

        private bool 饼蛙效果()
        {
            if (CurrentChain.Count > 0)
            {
                List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
                List<int> 合适的COST = new List<int> {
                    (int)CardId.鬼青蛙,
                    (int)CardId.粹蛙,
                    (int)CardId.小灰篮史莱姆,
                    (int)CardId.冰结界的术者,
                    (int)CardId.冰结界的水影
                };
                foreach (ClientCard monster in monsters)
                {
                    if (合适的COST.Contains(monster.Id))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                bool 有水舞台 = Duel.Fields[0].HasInSpellZone((int)CardId.水舞台, true);
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.魔知青蛙 && !有水舞台)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                monsters = (List<ClientCard>)Duel.Fields[0].Hand;
                bool 手里有2个史莱姆 = Duel.Fields[0].GetCountCardInZone(Duel.Fields[0].Hand, (int)CardId.小灰篮史莱姆) >= 2;
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.小灰篮史莱姆 && 手里有2个史莱姆)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                bool 需要丢魔知 = Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.鬼青蛙);
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.魔知青蛙 && 需要丢魔知)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                foreach (ClientCard monster in monsters)
                {
                    if (monster.Id == (int)CardId.粹蛙 || monster.Id == (int)CardId.魔知青蛙)
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
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                if (!Duel.Fields[0].HasInExtra((int)CardId.饼蛙))
                {
                    AI.SelectCard((int)CardId.饼蛙);
                }
                else
                {
                    AI.SelectCard(new[]
                        {
                            (int)CardId.鬼青蛙,
                            (int)CardId.冰结界的传道师,
                            (int)CardId.小灰篮史莱姆
                        });
                }
                return true;
            }
            else if (Duel.Phase == DuelPhase.Standby)
            {
                选择取除超量素材(Card.Overlays);
                if (Duel.Player == 0)
                {
                    AI.SelectNextCard(new[]
                        {
                            (int)CardId.鬼青蛙,
                            (int)CardId.冰结界的术者,
                            (int)CardId.冰结界的水影,
                            (int)CardId.粹蛙,
                            (int)CardId.魔知青蛙,
                            (int)CardId.小灰篮史莱姆
                        });
                }
                else
                {
                    AI.SelectNextCard(new[]
                        {
                            (int)CardId.魔知青蛙,
                            (int)CardId.鬼青蛙,
                            (int)CardId.粹蛙,
                            (int)CardId.小灰篮史莱姆,
                            (int)CardId.冰结界的术者,
                            (int)CardId.冰结界的水影
                        });
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                return true;
            }
            return true;
        }

        private bool 猫鲨特殊召唤()
        {
            bool should = Duel.Fields[0].HasInMonstersZone((int)CardId.饼蛙)
                        && ((AI.Utils.IsEnnemyBetter(true, false)
                            && !Duel.Fields[0].HasInMonstersZone(new List<int>
                                {
                                    (int)CardId.猫鲨,
                                    (int)CardId.神骑矢车菊圣人马
                                }, true, true))
                        || !Duel.Fields[0].HasInExtra((int)CardId.饼蛙));
            if (should)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool 猫鲨效果()
        {
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Id == (int)CardId.饼蛙)
                {
                    选择取除超量素材(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.Id == (int)CardId.神骑矢车菊圣人马)
                {
                    选择取除超量素材(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.Id == (int)CardId.大薰风凤凰)
                {
                    选择取除超量素材(Card.Overlays);
                    AI.SelectNextCard(monster);
                    return true;
                }
            }
            return false;
        }

        private bool 神骑矢车菊圣人马特殊召唤()
        {
            int num = 0;
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.Level ==2)
                {
                    num++;
                }
            }
            return AI.Utils.IsEnnemyBetter(true, false)
                   && num < 4
                   && !Duel.Fields[0].HasInMonstersZone(new List<int>
                        {
                            (int)CardId.神骑矢车菊圣人马
                        }, true, true);
        }

        private bool 大薰风凤凰特殊召唤()
        {
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
                if (attack - 2000 - defence > Duel.LifePoints[1] && !AI.Utils.IsEnnemyBetter(true, false))
                    return true;
            }
            return false;
        }

        private bool 虹光之宣告者特殊召唤()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool 改变攻守表示()
        {
            bool ennemyBetter = AI.Utils.IsEnnemyBetter(true, true);

            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !ennemyBetter && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        private bool 其他魔法发动()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsSpell();
        }

        private bool 其他陷阱发动()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsTrap() && DefaultTrap();
        }

        private bool 其他怪兽发动()
        {
            foreach (CardExecutor exec in Executors)
            {
                if (exec.Type == Type && exec.CardId == Card.Id)
                    return false;
            }
            return Card.IsMonster();
        }

        private void 选择取除超量素材(List<int> Overlays)
        {
            if (Overlays.Contains((int)CardId.小灰篮史莱姆) && Duel.Fields[0].HasInHand((int)CardId.小灰篮史莱姆) && !Duel.Fields[0].HasInGraveyard((int)CardId.小灰篮史莱姆))
                AI.SelectCard((int)CardId.小灰篮史莱姆);
            else if (Overlays.Contains((int)CardId.魔知青蛙) && Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙))
                AI.SelectCard((int)CardId.魔知青蛙);
            else if (Overlays.Contains((int)CardId.粹蛙) && Duel.Fields[0].HasInGraveyard((int)CardId.魔知青蛙) && !Duel.Fields[0].HasInGraveyard((int)CardId.粹蛙))
                AI.SelectCard((int)CardId.粹蛙);
            else
                AI.SelectCard(new[]
                    {
                        (int)CardId.小灰篮史莱姆,
                        (int)CardId.粹蛙,
                        (int)CardId.魔知青蛙,
                        (int)CardId.冰结界的术者,
                        (int)CardId.冰结界的水影,
                        (int)CardId.冰结界的传道师,
                        (int)CardId.鬼青蛙
                    });
        }
    }
}
