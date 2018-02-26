using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Trickstar", "AI_Trickstar")]
    public class TrickstarExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int White = 98169343;
            public const int BF = 9929398;
            public const int Yellow = 61283655;
            public const int Red = 35199656;
            public const int Urara = 14558127;
            public const int Ghost = 59438930;
            public const int Pink = 98700941;
            public const int MG = 23434538;
            public const int Tuner = 67441435;
            public const int Eater = 63845230;

            public const int Feather = 18144506;
            public const int Galaxy = 5133471;
            public const int Pot = 35261759;
            public const int Trans = 73628505;
            public const int Sheep = 73915051;
            public const int Crown = 22159429;
            public const int Stage = 35371948;

            public const int Re = 21076084;
            public const int Ring = 83555666;
            public const int Strike = 40605147;
            public const int Warn = 84749824;

            public const int Linkuri = 41999284;
            public const int Linkspi = 98978921;
            public const int SafeDra = 99111753;
            public const int Crystal = 50588353;
            public const int downer = 77058170;
            public const int phoneix = 2857636;
            public const int unicorn = 38342335;
            public const int firewall = 5043010;
            public const int snake = 74997493;
            public const int borrel = 31833038;
            public const int boomer = 5821478;
            public const int TG = 98558751;

        }

        public int getLinkMarker(int id)
        {
            if (id == CardId.borrel || id == CardId.snake) return 4;
            else if (id == CardId.unicorn) return 3;
            else if (id == CardId.Crystal || id == CardId.phoneix || id == CardId.SafeDra) return 2;
            return 1;
        }

        bool NormalSummoned = false;
        ClientCard stage_locked = null;
        bool pink_ss = false;
        bool snake_four_s = false;

        public TrickstarExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, CardId.MG, G_act);
            AddExecutor(ExecutorType.Activate, CardId.Strike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.Warn, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.Urara, hand_act_kind);
            AddExecutor(ExecutorType.Activate, CardId.Ghost, hand_act_kind);
            AddExecutor(ExecutorType.Activate, CardId.Ring, DefaultCompulsoryEvacuationDevice);

            // spell clean
            AddExecutor(ExecutorType.Activate, CardId.Stage, Stage_Lock);
            AddExecutor(ExecutorType.Activate, CardId.Feather, Feather_Act);
            AddExecutor(ExecutorType.Activate, CardId.Galaxy, GalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.TG, TG_eff);

            // ex_monster act
            AddExecutor(ExecutorType.Activate, CardId.Crystal, crystal_eff);
            AddExecutor(ExecutorType.Activate, CardId.SafeDra, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, CardId.Linkuri, linkuri_eff);
            AddExecutor(ExecutorType.Activate, CardId.phoneix);
            AddExecutor(ExecutorType.Activate, CardId.unicorn, unicorn_eff);
            AddExecutor(ExecutorType.Activate, CardId.snake, snake_eff);

            AddExecutor(ExecutorType.Activate, CardId.Tuner);

            // ex ss
            AddExecutor(ExecutorType.SpSummon, CardId.snake, snake_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.borrel, borrel_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.phoneix, phoneix_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.unicorn, unicorn_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Crystal, crystal_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.SafeDra, safedragon_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuri, linkuri_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkspi);

            // normal act
            AddExecutor(ExecutorType.Activate, CardId.Trans);
            AddExecutor(ExecutorType.SpSummon, CardId.BF, BF_pos);
            AddExecutor(ExecutorType.Activate, CardId.BF, BF_pos);
            AddExecutor(ExecutorType.Activate, CardId.Sheep, Sheep_Act);
            AddExecutor(ExecutorType.Activate, CardId.Eater);

            // ts
            AddExecutor(ExecutorType.Activate, CardId.Stage, Stage_act);
            AddExecutor(ExecutorType.Activate, CardId.Pink, pink_p);
            AddExecutor(ExecutorType.Activate, CardId.Red, red_ss);
            AddExecutor(ExecutorType.Activate, CardId.Yellow, yellow_p);
            AddExecutor(ExecutorType.Activate, CardId.White, white_p);
            AddExecutor(ExecutorType.Activate, CardId.Re, Reincarnation);
            AddExecutor(ExecutorType.Summon, CardId.Yellow, yellow_sum);
            AddExecutor(ExecutorType.Summon, CardId.Red, red_sum);
            AddExecutor(ExecutorType.Summon, CardId.Pink, pink_sum);

            // normal
            AddExecutor(ExecutorType.SpSummon, CardId.Eater, eater_ss);
            AddExecutor(ExecutorType.Summon, CardId.Urara,tuner_ns);
            AddExecutor(ExecutorType.Summon, CardId.Ghost, tuner_ns);
            AddExecutor(ExecutorType.Summon, CardId.Tuner, tuner_ns);
            AddExecutor(ExecutorType.Activate, CardId.Pot, Pot_Act);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Red);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Pink);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            
        }

        public bool Stage_Lock()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0) return false;
            foreach (ClientCard card in spells)
            {
                if (card.IsFacedown())
                {
                    AI.SelectCard(card);
                    stage_locked = card;
                    return true;
                }
            }
            return false;
        }

        public bool GalaxyCyclone()
        {
            if (!Bot.HasInSpellZone(CardId.Stage)) stage_locked = null;
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
                return false;
            ClientCard selected = null;
            if (Card.Location == CardLocation.Grave)
            {
                selected = AI.Utils.GetBestEnemySpell(true);
            }
            else
            {
                foreach (ClientCard card in spells)
                {
                    if (card.IsFacedown() && card != stage_locked)
                    {
                        selected = card;
                        break;
                    }
                }
            }
            if (selected == null)
                return false;
            AI.SelectCard(selected);
            return true;
        }

        public bool BF_pos()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public bool Feather_Act()
        {
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard card in spells)
            {
                // 有常用贴纸
                if (card.Id == 5851097 || card.Id == 30241314 || card.Id == 81674782 || card.Id == 58921041 || card.Id == 59305593)
                {
                    List<ClientCard> grave = Bot.GetGraveyardSpells();
                    // 墓地有银河旋风则不发动
                    foreach (ClientCard self_card in grave)
                    {
                        if (self_card.Id == CardId.Galaxy)
                            return false;
                    }
                    return true;
                }
            }
            // 2张以上才发动
            if (spells.Count <= 1)
                return false;
            return true;
        }

        public bool Sheep_Act()
        {
            if (Duel.Player == 0) return false;
            if (Duel.Phase == DuelPhase.End) return true;
            if (LastChainPlayer == 1 && (AI.Utils.IsChainTarget(Card) || GetLastChainCard().Id == CardId.Feather)) return true;
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int total_atk = 0;
                List<ClientCard> enemy_monster = Enemy.GetMonsters();
                foreach (ClientCard m in enemy_monster)
                {
                    if (m.IsAttack() && !m.Attacked) total_atk += m.Attack;
                }
                if (total_atk >= Duel.LifePoints[0]) return true;
            }
            return false;
        }

        public bool Stage_act()
        {
            if (Card.Location != CardLocation.Hand) return false;

            if (!NormalSummoned)
            {
                if (!Bot.HasInHand(CardId.Yellow))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Yellow,
                        CardId.Pink,
                        CardId.Red,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                else if (Duel.LifePoints[1] <= 1000 && !pink_ss)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Pink,
                        CardId.Yellow,
                        CardId.Red,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                else if (Bot.HasInHand(CardId.Yellow) && ! Bot.HasInHand(CardId.Red))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.Pink,
                        CardId.Yellow,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                else if (Enemy.GetMonsterCount() > 0 && AI.Utils.GetBestEnemyMonster().Attack >= AI.Utils.GetBestAttack(Bot))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.White,
                        CardId.Yellow,
                        CardId.Pink,
                        CardId.Red
                        
                    });
                    stage_locked = null;
                    return true;
                }
                else if (!Bot.HasInSpellZone(CardId.Stage))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Yellow,
                        CardId.Pink,
                        CardId.Red,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                return false;
            }

            if (NormalSummoned)
            {
                if (Bot.HasInMonstersZone(CardId.Yellow) && !Bot.HasInHand(CardId.Red))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.Pink,
                        CardId.Yellow,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                else if (Duel.LifePoints[1] <= 1000 && !pink_ss)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Pink,
                        CardId.Yellow,
                        CardId.Red,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
                else if (Enemy.GetMonsterCount() > 0 && AI.Utils.GetBestEnemyMonster().Attack >= AI.Utils.GetBestAttack(Bot))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.White,
                        CardId.Yellow,
                        CardId.Pink,
                        CardId.Red
                    });
                    stage_locked = null;
                    return true;
                }
                else if (!Bot.HasInSpellZone(CardId.Stage))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Yellow,
                        CardId.Pink,
                        CardId.Red,
                        CardId.White
                    });
                    stage_locked = null;
                    return true;
                }
            }
            stage_locked = null;
            return false;
        }

        public bool Pot_Act()
        {
            return Bot.Deck.Count > 15;
        }

        public bool hand_act_kind()
        {
            return (LastChainPlayer == 1);
        }

        public bool G_act()
        {
            return (Duel.Player == 1);
        }

        public bool pink_p()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if ((Duel.LifePoints[1] <= 1000 && Bot.HasInSpellZone(CardId.Stage))
                || Duel.LifePoints[1] <= 800
                || (!NormalSummoned && Bot.HasInGraveyard(CardId.Red))
                )
                {
                    pink_ss = true;
                    return true;
                }
                else if (Enemy.GetMonsterCount() > 0 && (AI.Utils.GetBestEnemyMonster().Attack - 800 >= Duel.LifePoints[0])) return false;
                pink_ss = true;
                return true;
            }
            else if (Card.Location == CardLocation.Onfield)
            {
                if (!NormalSummoned && Bot.HasInGraveyard(CardId.Yellow))
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Yellow,
                        CardId.Red,
                        CardId.White
                    });
                    return true;
                } else
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.Yellow,
                        CardId.White
                    });
                    return true;
                }
            }
            return true;
        }

        public bool eater_ss()
        {
            if (AI.Utils.IsTurn1OrMain2()) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            if (Bot.SpellZone[5] != null && Bot.SpellZone[5].IsFacedown())
            {
                targets.Add(Bot.SpellZone[5]);
            }
            if (Bot.SpellZone[5] != null && Bot.SpellZone[5].Id == CardId.Stage && Bot.HasInHand(CardId.Stage))
            {
                targets.Add(Bot.SpellZone[5]);
            }
            foreach (ClientCard e_c in Bot.ExtraDeck)
            {
                targets.Add(e_c);
                if (targets.Count >= 5)
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            foreach (ClientCard s_c in Bot.SpellZone)
            {
                targets.Add(s_c);
                if (targets.Count >= 5)
                {
                    AI.SelectCard(targets);
                    return true;
                }
            }
            return false;
        }

        public bool red_ss()
        {
            if (Duel.Player == 0)
            {
                if (LastChainPlayer == 0 && GetLastChainCard().Id == CardId.Red)
                {
                    foreach(ClientCard m in Bot.GetMonsters())
                    {
                        if (AI.Utils.IsChainTarget(m))
                        {
                            AI.SelectCard(m);
                            return true;
                        }
                    }
                }
                if (LastChainPlayer == 1) return true;
                if (AI.Utils.IsTurn1OrMain2()) return false;
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    List<ClientCard> self_m = Bot.GetMonsters();
                    ClientCard tosolve_enemy = AI.Utils.GetOneEnemyBetterThanMyBest();
                    foreach (ClientCard c in self_m)
                    {
                        if (c.Id == CardId.Yellow || c.Id == CardId.Pink || c.Id == CardId.White)
                        {
                            if (c.Attacked)
                            {
                                AI.SelectCard(c);
                                return true;
                            }
                            if (tosolve_enemy != null)
                            {
                                if (Bot.HasInHand(CardId.White) && c.Attack * 2 < tosolve_enemy.Attack)
                                {
                                    AI.SelectCard(c);
                                    return true;
                                }
                                if (!Bot.HasInHand(CardId.White) && c.Attack < tosolve_enemy.Attack)
                                {
                                    AI.SelectCard(c);
                                    return true;
                                }
                            }
                        }
                    }
                }
            } else
            {
                if (LastChainPlayer == 1) return true;
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (AI.Utils.GetOneEnemyBetterThanMyBest() != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool yellow_p()
        {
            if (!Bot.HasInHand(CardId.Stage) && !Bot.HasInSpellZone(CardId.Stage) && Bot.GetRemainingCount(CardId.Stage,3) > 0)
            {
                AI.SelectCard(new[]
                {
                    CardId.Stage,
                    CardId.Red,
                    CardId.White,
                    CardId.Pink,
                    CardId.Re,
                    CardId.Ring,
                    CardId.Yellow
                });
                return true;
            }
            else if (Enemy.GetMonsterCount() == 0 && !AI.Utils.IsTurn1OrMain2())
            {
                if (Bot.HasInGraveyard(CardId.Red) && Bot.GetRemainingCount(CardId.Pink,1)>0 && !pink_ss)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Pink,
                        CardId.Red,
                        CardId.White,
                        CardId.Re,
                        CardId.Stage,
                        CardId.Ring,
                        CardId.Yellow
                    });
                }
                else if (Bot.HasInGraveyard(CardId.Pink) && Bot.HasInGraveyard(CardId.Red) && Bot.GetRemainingCount(CardId.Ring,1) > 0)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Ring,
                        CardId.Red,
                        CardId.White,
                        CardId.Re,
                        CardId.Stage,
                        CardId.Pink,
                        CardId.Yellow
                    });
                }
                else if (Bot.GetRemainingCount(CardId.White,2) > 0 && Duel.LifePoints[1] <= 4000)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.White,
                        CardId.Red,
                        CardId.Pink,
                        CardId.Re,
                        CardId.Stage,
                        CardId.Ring,
                        CardId.Yellow
                    });
                }
                else if (Bot.HasInGraveyard(CardId.White) && Bot.GetRemainingCount(CardId.Ring, 1) > 0)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Ring,
                        CardId.Red,
                        CardId.Pink,
                        CardId.Re,
                        CardId.Stage,
                        CardId.White,
                        CardId.Yellow
                    });
                }
                else
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.Pink,
                        CardId.Re,
                        CardId.Ring,
                        CardId.Stage,
                        CardId.White,
                        CardId.Yellow
                    });
                }
                return true;
            }
            else if (AI.Utils.GetProblematicEnemyMonster() != null)
            {
                int atk = AI.Utils.GetProblematicEnemyMonster().Attack;
                if (atk >= 1800 && atk <= 3600 && Bot.GetRemainingCount(CardId.White, 2) > 0)
                {
                    AI.SelectCard(new[]
                    {
                        CardId.White,
                        CardId.Red,
                        CardId.Pink,
                        CardId.Re,
                        CardId.Stage,
                        CardId.Ring,
                        CardId.Yellow
                    });
                }
                else
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.Pink,
                        CardId.Re,
                        CardId.Ring,
                        CardId.Stage,
                        CardId.White,
                        CardId.Yellow
                    });
                }
                return true;
            } else
            {
                AI.SelectCard(new[]
                {
                    CardId.Red,
                    CardId.Pink,
                    CardId.Re,
                    CardId.Ring,
                    CardId.Stage,
                    CardId.White,
                    CardId.Yellow
                });
                return true;
            }
        }

        public bool white_p()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                // from blackwing
                ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
                ClientCard bestEnemyATK = Enemy.GetMonsters().GetHighestAttackMonster();
                ClientCard bestEnemyDEF = Enemy.GetMonsters().GetHighestDefenseMonster();
                if (bestMy == null || (bestEnemyATK == null && bestEnemyDEF == null))
                    return false;
                if (bestEnemyATK != null && bestMy.Attack <= bestEnemyATK.Attack && bestMy.Attack * 2 >= bestEnemyATK.Attack)
                    return true;
                if (bestEnemyDEF != null && bestMy.Attack <= bestEnemyDEF.Defense && bestMy.Attack * 2 >= bestEnemyDEF.Defense)
                    return true;
                return false; 
            } else
            {
                if (Enemy.GetMonsterCount() == 0 && !AI.Utils.IsTurn1OrMain2()) return true;
                else if (Enemy.GetMonsterCount() != 0)
                {
                    return (Enemy.GetMonsters().GetLowestAttackMonster().Attack < 2000 ||
                        Enemy.GetMonsters().GetLowestDefenseMonster().Defense < 2000);
                }
                return false;
            }
        }

        public bool Reincarnation()
        {
            if (Card.Location == CardLocation.Grave) return ts_reborn();
            return true;
        }

        public bool ts_reborn()
        {
            bool can_summon = (Duel.Player == 0 && NormalSummoned);
            if (can_summon)
            {
                if ((Duel.Phase < DuelPhase.Main2) && Bot.HasInGraveyard(CardId.Pink))
                {
                    AI.SelectCard(CardId.Pink);
                    return true;
                }
                else
                {
                    AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.White,
                        CardId.Yellow,
                        CardId.Pink
                    });
                    return true;
                }
            }
            else
            {
                AI.SelectCard(new[]
                    {
                        CardId.Red,
                        CardId.White,
                        CardId.Yellow,
                        CardId.Pink
                    });
                return true;
            }
        }

        public bool yellow_sum()
        {
            NormalSummoned = true;
            return true;
        }

        public bool red_sum()
        {
            if ((Enemy.GetMonsterCount() == 0 && Duel.LifePoints[1] <= 1800) || (Duel.Turn == 1 && Bot.HasInHand(CardId.Re)))
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool pink_sum()
        {
            if (Duel.LifePoints[1] <= 1000)
            {
                NormalSummoned = true;
                return true;
            }
            else if (!AI.Utils.IsTurn1OrMain2() && (Bot.HasInGraveyard(CardId.Yellow) || Bot.HasInGraveyard(CardId.Red)))
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool tuner_ns()
        {
            if (tuner_ss())
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool tuner_ss()
        {
            if (Bot.GetMonsterCount() == 0 || !Bot.HasInExtra(CardId.Crystal)) return false;
            int count = 0;
            if (Card.Id != CardId.Urara) count += 1;
            foreach(ClientCard hand in Bot.Hand)
            {
                if (hand.Id == Card.Id) count += 1;
            }
            if (count >= 2) return true;
            return false;
        }

        public bool linkuri_ss()
        {
            foreach(ClientCard c in Bot.GetMonsters())
            {
                if (c.Id != CardId.Eater && c.Level == 1 && c.Id != CardId.Linkuri && c.Id != CardId.Linkspi)
                {
                    AI.SelectCard(c);
                    return true;
                }
            }
            return false;
        }

        public bool linkuri_eff()
        {
            if (LastChainPlayer == 0 && GetLastChainCard().Id == CardId.Linkuri) return false;
            AI.SelectCard(new[] { CardId.Tuner, CardId.BF + 1 });
            return true;
        }

        public bool crystal_ss()
        {
            if (Bot.HasInMonstersZone(CardId.BF) && Bot.HasInMonstersZone(CardId.BF + 1))
            {
                AI.SelectCard(new[]
                    {CardId.BF,
                    CardId.BF + 1
                    });
                return true;
            }
            IList<ClientCard> targets = new List<ClientCard>();
            foreach(ClientCard t_check in Bot.GetMonsters())
            {
                if (t_check.Id == CardId.BF || t_check.Id == CardId.Tuner || t_check.Id == CardId.Urara || t_check.Id == CardId.Ghost)
                {
                    targets.Add(t_check);
                    break;
                }
            }
            if (targets.Count == 0) return false;
            foreach(ClientCard e_check in Bot.GetMonsters())
            {
                if (targets[0] != e_check && getLinkMarker(e_check.Id) <= 2 && e_check.Id != CardId.Eater)
                {
                    targets.Add(e_check);
                    break;
                }
            }
            if (targets.Count <= 1) return false;
            return false;
        }

        public bool crystal_eff()
        {
            if (Duel.Player == 0)
            {
                AI.SelectCard(new[] { CardId.Tuner, CardId.Ghost , CardId.Urara});
                return true;
            }
            else if (AI.Utils.IsChainTarget(Card) || AI.Utils.GetProblematicEnemySpell() != null) return true;
            return false;
        }

        public bool TG_eff()
        {
            if (Card.Location != CardLocation.MonsterZone) return true;
            ClientCard target = AI.Utils.GetProblematicEnemySpell();
            if (target != null) AI.SelectCard(target);
            return true;
        }

        public bool safedragon_ss()
        {
            ClientCard m = AI.Utils.GetProblematicEnemyMonster();
            if (m == null)
            {
                return (Enemy.GetMonsterCount() == 0 && Duel.LifePoints[1] <= 1100);
            }
            ClientCard ex_1 = Bot.MonsterZone[5];
            ClientCard ex_2 = Bot.MonsterZone[6];
            if (!((ex_2 != null && ex_2.Controller == 0) || (ex_1 != null && ex_1.Controller == 0))) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard s_m in Bot.GetMonsters())
            {
                if (s_m != Bot.MonsterZone[5] && s_m != Bot.MonsterZone[6]) targets.Add(s_m);
                if (targets.Count >= 2) break;
            }
            if (targets.Count >= 2)
            {
                AI.SelectCard(targets);
                return true;
            }
            return false;
        }

        public bool phoneix_ss()
        {
            ClientCard m = AI.Utils.GetProblematicEnemySpell();
            if (m == null)
            {
                return (Enemy.GetMonsterCount() == 0 && Duel.LifePoints[1] <= 1900 && Duel.Phase == DuelPhase.Main1);
            }
            if (Bot.Hand.Count == 0) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            ClientCard ex_zone = null;
            foreach (ClientCard s_m in Bot.GetMonsters())
            {
                if (s_m == Bot.MonsterZone[5] && s_m != Bot.MonsterZone[6])
                {
                    ex_zone = s_m;
                }
                else
                {
                    if (s_m.Id != CardId.Eater)
                    {
                        bool same = false;
                        foreach(ClientCard checkcard in targets)
                        {
                            if (checkcard.Id == s_m.Id) same = true;break;
                        }
                        if (!same) targets.Add(s_m);
                    };
                };
                if (targets.Count >= 2) break;
            }
            if (targets.Count < 2)
            {
                targets.Add(ex_zone);
            }
            AI.SelectCard(targets);
            return true;
        }

        public bool unicorn_ss() {
            ClientCard m = AI.Utils.GetProblematicEnemyCard();
            if (m == null)
            {
                return (Enemy.GetMonsterCount() == 0 && Duel.LifePoints[1] <= 2200 && Duel.Phase == DuelPhase.Main1);
            }
            if (Bot.Hand.Count == 0) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            int link_count = 0;
            foreach (ClientCard s_m in Bot.GetMonsters())
            {
                if (s_m.Id != CardId.Eater && getLinkMarker(s_m.Id) <= 2)
                {
                    bool same = false;
                    foreach (ClientCard checkcard in targets)
                    {
                        if (checkcard.Id == s_m.Id) same = true; break;
                    }
                    if (!same)
                    {
                        targets.Add(s_m);
                        link_count += getLinkMarker(s_m.Id);
                    }
                    if (link_count >= 3) break;
                }
            }
            if (link_count < 3) return false;
            AI.SelectCard(targets);
            return true;
        }

        public bool unicorn_eff()
        {
            ClientCard m = AI.Utils.GetProblematicEnemyCard();
            Logger.DebugWriteLine("独角兽的效果对象：");
            Logger.DebugWriteLine(m.Name);
            if (m == null) return false;
            AI.SelectCard(Bot.Hand[0]);
            AI.SelectNextCard(m);
            return true;
        }

        public bool snake_ss()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard m in Bot.GetMonsters())
            {
                if (m.Attack < 1900)
                {
                    targets.Add(m);
                }
            }
            if (targets.Count >= 4)
            {

                AI.SelectCard(targets);
                snake_four_s = true;
                return true;
            }
            return false;
        }

        public bool snake_eff()
        {
            Logger.DebugWriteLine("锁龙蛇记录：");
            Logger.DebugWriteLine(ActivateDescription.ToString());
            Logger.DebugWriteLine(AI.Utils.GetStringId(CardId.snake, 2).ToString());
            if (snake_four_s)
            {
                snake_four_s = false;
                return true;
            }
            //if (ActivateDescription == AI.Utils.GetStringId(CardId.snake, 2)) return true;
            if (ActivateDescription == AI.Utils.GetStringId(CardId.snake, 1))
            {
                foreach(ClientCard hand in Bot.Hand)
                {
                    if (hand.Id == CardId.Red || hand.Id == CardId.Pink)
                    {
                        AI.SelectCard(hand);
                        return true;
                    }
                    if (hand.Id == CardId.Urara || hand.Id == CardId.Ghost)
                    {
                        if (tuner_ss())
                        {
                            AI.SelectCard(hand);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool borrel_ss()
        {
            return false;
        }

        public bool MonsterRepos()
        {
            if (Card.Id == CardId.Eater) return false;
            bool enemyBetter = AI.Utils.IsAllEnemyBetter(true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
                return true;
            return false;
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            NormalSummoned = false;
            stage_locked = null;
            pink_ss = false;
            snake_four_s = false;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand(CardId.White))
                    attacker.RealPower = attacker.RealPower * 2;
                else if (attacker.Id == CardId.Eater)
                    attacker.RealPower = 99999;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }
    }
}