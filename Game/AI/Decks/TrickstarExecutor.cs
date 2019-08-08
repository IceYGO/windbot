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
            public const int LockBird = 94145021;

            public const int Feather = 18144506;
            public const int Galaxy = 5133471;
            public const int Pot = 35261759;
            public const int Trans = 73628505;
            public const int Sheep = 73915051;
            public const int Crown = 22159429;
            public const int Stage = 35371948;
            public const int GraveCall = 24224830;
            public const int DarkHole = 53129443;

            public const int Re = 21076084;
            public const int Ring = 83555666;
            public const int Strike = 40605147;
            public const int Warn = 84749824;
            public const int Awaken = 10813327;

            public const int Linkuri = 41999284;
            public const int Linkspi = 98978921;
            public const int SafeDra = 99111753;
            public const int Crystal = 50588353;
            public const int Phoneix = 2857636;
            public const int Unicorn = 38342335;
            public const int Snake = 74997493;
            public const int Borrel = 31833038;
            public const int TG = 98558751;

            public const int Beelze = 34408491;
            public const int Abyss = 9753964;
            public const int Exterio = 99916754;
            public const int Ultimate = 86221741;
            public const int Cardian = 87460579;

            public const int Missus = 3987233;
        }

        public int getLinkMarker(int id)
        {
            if (id == CardId.Borrel || id == CardId.Snake) return 4;
            else if (id == CardId.Abyss || id == CardId.Beelze || id == CardId.Exterio || id == CardId.Ultimate || id == CardId.Cardian) return 5;
            else if (id == CardId.Unicorn) return 3;
            else if (id == CardId.Crystal || id == CardId.Phoneix || id == CardId.SafeDra || id == CardId.Missus) return 2;
            return 1;
        }

        bool NormalSummoned = false;
        ClientCard stage_locked = null;
        bool pink_ss = false;
        bool snake_four_s = false;
        bool tuner_eff_used = false;
        bool crystal_eff_used = false;
        int red_ss_count = 0;
        bool white_eff_used = false;
        bool lockbird_useful = false;
        bool lockbird_used = false;
        int GraveCall_id = 0;
        int GraveCall_count = 0;

        List<int> SkyStrike_list = new List<int> {
            26077387, 8491308, 63288573, 90673288,
            21623008, 25955749, 63166095, 99550630,
            25733157, 51227866, 52340444,98338152,
            24010609, 97616504, 50005218
        };

        public TrickstarExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // counter
            AddExecutor(ExecutorType.Activate, CardId.MG, G_act);
            AddExecutor(ExecutorType.Activate, CardId.Strike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.Warn, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.Awaken, Awaken_ss);
            AddExecutor(ExecutorType.Activate, CardId.Urara, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.Ghost, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.Ring, Ring_act);
            AddExecutor(ExecutorType.Activate, CardId.Abyss, Abyss_eff);
            AddExecutor(ExecutorType.Activate, CardId.Exterio, Exterio_counter);
            AddExecutor(ExecutorType.Activate, CardId.Cardian);
            AddExecutor(ExecutorType.Activate, CardId.GraveCall, GraveCall_eff);

            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DarkHole_eff);

            // spell clean
            AddExecutor(ExecutorType.Activate, field_activate);
            AddExecutor(ExecutorType.Activate, CardId.Stage, Stage_Lock);
            AddExecutor(ExecutorType.Activate, CardId.Feather, Feather_Act);
            AddExecutor(ExecutorType.Activate, CardId.Stage, Stage_act);
            AddExecutor(ExecutorType.Activate, CardId.Galaxy, GalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.TG, TG_eff);

            AddExecutor(ExecutorType.Activate, CardId.Tuner,Tuner_eff);
            AddExecutor(ExecutorType.SpellSet, Five_Rainbow);

            // ex ss
            AddExecutor(ExecutorType.SpSummon, CardId.Borrel, Borrel_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Missus, Missus_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Phoneix, Phoneix_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Snake, Snake_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Crystal, Crystal_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.SafeDra, Safedragon_ss);
            AddExecutor(ExecutorType.Activate, CardId.SafeDra, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, CardId.Linkuri, Linkuri_eff);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuri, Linkuri_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Unicorn, Unicorn_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkspi);

            // ex_monster act
            AddExecutor(ExecutorType.Activate, CardId.Beelze);
            AddExecutor(ExecutorType.Activate, CardId.Missus, Missus_eff);
            AddExecutor(ExecutorType.Activate, CardId.Crystal, Crystal_eff);
            AddExecutor(ExecutorType.Activate, CardId.Phoneix, Phoneix_eff);
            AddExecutor(ExecutorType.Activate, CardId.Unicorn, Unicorn_eff);
            AddExecutor(ExecutorType.Activate, CardId.Snake, Snake_eff);
            AddExecutor(ExecutorType.Activate, CardId.Borrel, Borrel_eff);

            // normal act
            AddExecutor(ExecutorType.Activate, CardId.Trans);
            AddExecutor(ExecutorType.SpSummon, CardId.BF, BF_pos);
            AddExecutor(ExecutorType.Activate, CardId.BF, BF_pos);
            AddExecutor(ExecutorType.Activate, CardId.Sheep, Sheep_Act);
            AddExecutor(ExecutorType.Activate, CardId.Eater,Eater_eff);
            AddExecutor(ExecutorType.Activate, CardId.LockBird, LockBird_act);

            // ts
            AddExecutor(ExecutorType.Activate, CardId.Pink, Pink_eff);
            AddExecutor(ExecutorType.Activate, CardId.Re, Reincarnation);
            AddExecutor(ExecutorType.Activate, CardId.Red, Red_ss);
            AddExecutor(ExecutorType.Activate, CardId.Yellow, Yellow_eff);
            AddExecutor(ExecutorType.Activate, CardId.White, White_eff);
            AddExecutor(ExecutorType.Activate, CardId.Crown, Crown_eff);
            AddExecutor(ExecutorType.Summon, CardId.Yellow, Yellow_sum);
            AddExecutor(ExecutorType.Summon, CardId.Red, Red_sum);
            AddExecutor(ExecutorType.Summon, CardId.Pink, Pink_sum);

            // normal
            AddExecutor(ExecutorType.SpSummon, CardId.Eater, Eater_ss);
            AddExecutor(ExecutorType.Summon, CardId.Tuner, Tuner_ns);
            AddExecutor(ExecutorType.Summon, CardId.Urara,Tuner_ns);
            AddExecutor(ExecutorType.Summon, CardId.Ghost, Tuner_ns);
            AddExecutor(ExecutorType.Activate, CardId.Pot, Pot_Act);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Red);
            AddExecutor(ExecutorType.SummonOrSet, CardId.Pink);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public bool Five_Rainbow()
        {
            if (Enemy.HasInSpellZone(19619755,true) || Bot.HasInSpellZone(19619755, true))
            {
                if (Card.HasType(CardType.Field)) return false;
                bool has_setcard = false;
                for (int i = 0; i < 5; ++i)
                {
                    ClientCard sp = Bot.SpellZone[i];
                    if (sp != null && sp.HasPosition(CardPosition.FaceDown))
                    {
                        has_setcard = true;
                        break;
                    }
                }
                if (has_setcard) return false;
                AI.SelectPlace(SelectSTPlace());
                return true;
            }
            return false;
        }

        public int SelectSTPlace()
        {
            List<int> list = new List<int>();
            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach(int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null) return zone;
            }
            return 0;
        }

        public bool SpellSet()
        {
            if (Card.IsCode(CardId.Sheep) && Bot.HasInSpellZone(CardId.Sheep)) return false;
            if (DefaultSpellSet())
            {
                AI.SelectPlace(SelectSTPlace());
                return true;
            } else if (Enemy.HasInSpellZone(58921041,true) || Bot.HasInSpellZone(58921041, true))
            {
                if (Card.IsCode(CardId.Stage)) return !Bot.HasInSpellZone(CardId.Stage);
                if (Card.IsSpell())
                {
                    AI.SelectPlace(SelectSTPlace());
                    return true;
                }
            }
            return false;
        }

        public bool IsTrickstar(int id)
        {
            return (id == CardId.Yellow || id == CardId.Red || id == CardId.Pink || id == CardId.White || id == CardId.Stage || id == CardId.Re || id == CardId.Crown);
        }

        public bool field_activate()
        {
            if (Card.HasPosition(CardPosition.FaceDown) && Card.HasType(CardType.Field) && Card.Location == CardLocation.SpellZone)
            {
                // field spells that forbid other fields' activate
                return !Card.IsCode(71650854, 78082039);
            }
            return false;
        }

        public bool spell_trap_activate()
        {
            if (Card.Location != CardLocation.SpellZone && Card.Location != CardLocation.Hand) return true;
            if (Enemy.HasInMonstersZone(CardId.Exterio,true) && !Bot.HasInHandOrHasInMonstersZone(CardId.Ghost)) return false;
            if (Card.IsSpell())
            {
                if (Enemy.HasInMonstersZone(33198837, true) && !Bot.HasInHandOrHasInMonstersZone(CardId.Ghost)) return false;
                if (Enemy.HasInSpellZone(61740673, true) || Bot.HasInSpellZone(61740673,true)) return false;
                if (Enemy.HasInMonstersZone(37267041, true) || Bot.HasInMonstersZone(37267041, true)) return false;
                return true;
            }
            if (Card.IsTrap())
            {
                if (Enemy.HasInSpellZone(51452091, true) || Bot.HasInSpellZone(51452091, true)) return false;
                if (Enemy.HasInSpellZone(51452091, true) || Bot.HasInSpellZone(51452091, true)) return false;
                return true;
            }
            // how to get here?
            return false;
        }

        public int[] Useless_List()
        {
            return new[]
            {
                CardId.Tuner,
                CardId.Awaken,
                CardId.Crown,
                CardId.Pink,
                CardId.Pot,
                CardId.BF,
                CardId.White,
                CardId.Trans,
                CardId.Galaxy,
                CardId.Feather,
                CardId.Sheep,
                CardId.Re,
                CardId.Red,
                CardId.Yellow,
                CardId.Eater,
                CardId.MG,
                CardId.Ghost,
                CardId.Urara,
                CardId.Stage,
                CardId.Ring,
                CardId.Warn,
                CardId.Strike
            };
        }

        public int GetTotalATK(IList<ClientCard> list)
        {
            int atk = 0;
            foreach(ClientCard c in list)
            {
                if (c == null) continue;
                atk += c.Attack;
            }
            return atk;
        }

        public bool Awaken_ss()
        {
            // judge whether can ss from exdeck
            bool judge = (Bot.ExtraDeck.Count > 0);
            if (Enemy.GetMonstersExtraZoneCount() > 1) judge = false; // exlink
            if (Bot.GetMonstersExtraZoneCount() >= 1)
            {
                foreach (ClientCard card in Bot.GetMonstersInExtraZone())
                {
                    if (getLinkMarker(card.Id) == 5) judge = false;
                }
            }
            // can ss from exdeck
            if (judge)
            {
                bool fornextss = Util.ChainContainsCard(CardId.Awaken);
                IList<ClientCard> ex = Bot.ExtraDeck;
                ClientCard ex_best = null;
                foreach (ClientCard ex_card in ex)
                {
                    if (!fornextss)
                    {
                        if (Bot.HasInExtra(CardId.Exterio))
                        {
                            bool has_skystriker = false;
                            foreach (ClientCard card in Enemy.Graveyard)
                            {
                                if (card != null && card.IsCode(SkyStrike_list))
                                {
                                    has_skystriker = true;
                                    break;
                                }
                            }
                            if (!has_skystriker)
                            {
                                foreach (ClientCard card in Enemy.GetSpells())
                                {
                                    if (card != null && card.IsCode(SkyStrike_list))
                                    {
                                        has_skystriker = true;
                                        break;
                                    }
                                }
                            }
                            if (!has_skystriker)
                            {
                                foreach (ClientCard card in Enemy.GetSpells())
                                {
                                    if (card != null && card.IsCode(SkyStrike_list))
                                    {
                                        has_skystriker = true;
                                        break;
                                    }
                                }
                            }
                            if (has_skystriker)
                            {
                                AI.SelectCard(CardId.Exterio);
                                return true;
                            } else
                            {
                                if (ex_best == null || ex_card.Attack > ex_best.Attack) ex_best = ex_card;
                            }
                        }
                        else
                        {
                            if (ex_best == null || ex_card.Attack > ex_best.Attack) ex_best = ex_card;
                        }
                    } 
                    else
                    {
                        if (getLinkMarker(ex_card.Id) != 5 && (ex_best == null || ex_card.Attack > ex_best.Attack)) ex_best = ex_card;
                    }
                }
                if (ex_best != null)
                {
                    AI.SelectCard(ex_best);
                }
            }
            if (!judge || Util.ChainContainsCard(CardId.Awaken))
            {
                // cannot ss from exdeck or have more than 1 grass in chain
                int[] secondselect = new[]
                {
                    CardId.Borrel,
                    CardId.Ultimate,
                    CardId.Abyss,
                    CardId.Cardian,
                    CardId.Exterio,
                    CardId.Ghost,
                    CardId.White,
                    CardId.Red,
                    CardId.Yellow,
                    CardId.Pink
                };
                if (!Util.ChainContainsCard(CardId.Awaken))
                {
                    if (!judge && Bot.GetRemainingCount(CardId.Ghost, 2) > 0)
                    {
                        AI.SelectCard(CardId.Ghost);
                        AI.SelectPosition(CardPosition.FaceUpDefence);
                    }
                    else
                        AI.SelectCard(secondselect);
                }
                else
                {
                    if (!judge)
                        AI.SelectCard(secondselect);
                    AI.SelectNextCard(secondselect);
                    AI.SelectThirdCard(secondselect);
                }
            }
            return true;
        }

        public bool Abyss_eff()
        {
            // tuner ss
            if (ActivateDescription == -1)
            {
                AI.SelectCard(CardId.Ghost, CardId.TG, CardId.Tuner, CardId.Urara, CardId.BF);
                return true;
            };
            // counter
            if (!Enemy.HasInMonstersZone(CardId.Ghost) || Enemy.GetHandCount() <= 1)
            {
                ClientCard tosolve = Util.GetProblematicEnemyCard();
                if (tosolve == null)
                {
                    if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard() != null)
                    {
                        ClientCard target = Util.GetLastChainCard();
                        if (target.HasPosition(CardPosition.FaceUp) && (target.Location == CardLocation.MonsterZone || target.Location == CardLocation.SpellZone)) tosolve = target;
                    }
                }
                if (tosolve != null)
                {
                    AI.SelectCard(tosolve);
                    return true;
                }
            }
            return false;
        }

        public void RandomSort(List<ClientCard> list) {

            int n = list.Count;
            while(n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                ClientCard temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
        }

        public bool Stage_Lock()
        {
            if (Card.Location != CardLocation.SpellZone) return false;
            List<ClientCard> spells = Enemy.GetSpells();
            RandomSort(spells);
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
                selected = Util.GetBestEnemySpell(true);
            }
            else
            {
                if (!spell_trap_activate()) return false;
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
            AI.SelectPlace(SelectSTPlace());
            return true;
        }

        public bool BF_pos()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public bool Feather_Act()
        {
            if (!spell_trap_activate()) return false;
            if (Util.GetProblematicEnemySpell() != null)
            {
                List<ClientCard> grave = Bot.GetGraveyardSpells();
                foreach (ClientCard self_card in grave)
                {
                    if (self_card.IsCode(CardId.Galaxy))
                        return false;
                }
                AI.SelectPlace(SelectSTPlace());
                return true;
            }
            // activate when more than 2 cards
            if (Enemy.GetSpellCount() <= 1)
                return false;
            AI.SelectPlace(SelectSTPlace());
            return true;
        }

        public bool Sheep_Act()
        {
            if (!spell_trap_activate()) return false;
            if (Duel.Player == 0) return false;
            if (Duel.Phase == DuelPhase.End) return true;
            if (Duel.LastChainPlayer == 1 && (Util.IsChainTarget(Card) || (Util.GetLastChainCard().IsCode(CardId.Feather) && !Bot.HasInSpellZone(CardId.Awaken)))) return true;
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int total_atk = 0;
                List<ClientCard> enemy_monster = Enemy.GetMonsters();
                foreach (ClientCard m in enemy_monster)
                {
                    if (m.IsAttack() && !m.Attacked) total_atk += m.Attack;
                }
                if (total_atk >= Bot.LifePoints) return true;
            }
            return false;
        }

        public bool Stage_act()
        {
            if (Card.Location == CardLocation.SpellZone && Card.HasPosition(CardPosition.FaceUp)) return false;
            if (!spell_trap_activate()) return false;
            if (!NormalSummoned)
            {
                if (!Bot.HasInHand(CardId.Yellow))
                {
                    AI.SelectCard(CardId.Yellow, CardId.Pink, CardId.Red, CardId.White);
                    stage_locked = null;
                    return true;
                }
                else if (Enemy.LifePoints <= 1000 && Bot.GetRemainingCount(CardId.Pink,1) > 0)
                {
                    AI.SelectCard(CardId.Pink, CardId.Yellow, CardId.Red, CardId.White);
                    stage_locked = null;
                    return true;
                }
                else if (Bot.HasInHand(CardId.Yellow) && ! Bot.HasInHand(CardId.Red))
                {
                    AI.SelectCard(CardId.Red, CardId.Pink, CardId.Yellow, CardId.White);
                    stage_locked = null;
                    return true;
                }
                else if (Enemy.GetMonsterCount() > 0 && Util.GetBestEnemyMonster().Attack >= Util.GetBestAttack(Bot))
                {
                    AI.SelectCard(CardId.White, CardId.Yellow, CardId.Pink, CardId.Red);
                    stage_locked = null;
                    return true;
                }
                else if (!Bot.HasInSpellZone(CardId.Stage))
                {
                    AI.SelectCard(CardId.Yellow, CardId.Pink, CardId.Red, CardId.White);
                    stage_locked = null;
                    return true;
                }
                return false;
            }

            if (NormalSummoned)
            {
                if (Enemy.LifePoints <= 1000 && !pink_ss && Bot.GetRemainingCount(CardId.Pink,1) > 0)
                {
                    AI.SelectCard(CardId.Pink, CardId.Yellow, CardId.Red, CardId.White);
                    stage_locked = null;
                    return true;
                }
                if (Enemy.GetMonsterCount() > 0 && Util.GetBestEnemyMonster().Attack >= Util.GetBestAttack(Bot) && !Bot.HasInHand(CardId.White))
                {
                    AI.SelectCard(CardId.White, CardId.Yellow, CardId.Pink, CardId.Red);
                    stage_locked = null;
                    return true;
                }
                if (Bot.HasInMonstersZone(CardId.Yellow) && !Bot.HasInHand(CardId.Red))
                {
                    AI.SelectCard(CardId.Red, CardId.Pink, CardId.Yellow, CardId.White);
                    stage_locked = null;
                    return true;
                }
                AI.SelectCard(CardId.Yellow, CardId.Pink, CardId.Red, CardId.White);
                stage_locked = null;
                return true;
            }
            stage_locked = null;
            return false;
        }

        public bool Pot_Act()
        {
            if (!spell_trap_activate()) return false;
            if (Bot.Deck.Count > 15)
            {
                AI.SelectPlace(SelectSTPlace());
                return true;
            }
            return false;
        }

        public bool Hand_act_eff()
        {
            if (GraveCall_count > 0 && GraveCall_id == Card.Id) return false;
            if (Card.IsCode(CardId.Urara) && Bot.HasInHand(CardId.LockBird) && Bot.HasInSpellZone(CardId.Re)) return false;
            if (Card.IsCode(CardId.Ghost) && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.Ghost)) return false;
            return (Duel.LastChainPlayer == 1);
        }

        public bool Exterio_counter()
        {
            if (Duel.LastChainPlayer == 1)
            {
                AI.SelectCard(Useless_List());
                return true;
            }
            return false;
        }

        public bool G_act()
        {
            return (Duel.Player == 1 && !(GraveCall_count > 0 && GraveCall_id == Card.Id));
        }

        public bool Pink_eff()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if ((Enemy.LifePoints <= 1000 && Bot.HasInSpellZone(CardId.Stage))
                || Enemy.LifePoints <= 800
                || (!NormalSummoned && Bot.HasInGraveyard(CardId.Red))
                )
                {
                    pink_ss = true;
                    return true;
                }
                else if (Enemy.GetMonsterCount() > 0 && (Util.GetBestEnemyMonster().Attack - 800 >= Bot.LifePoints)) return false;
                pink_ss = true;
                return true;
            }
            else if (Card.Location == CardLocation.Onfield)
            {
                if (!NormalSummoned && Bot.HasInGraveyard(CardId.Yellow))
                {
                    AI.SelectCard(CardId.Yellow, CardId.Red, CardId.White);
                    return true;
                } else
                {
                    AI.SelectCard(CardId.Red, CardId.Yellow, CardId.White);
                    return true;
                }
            }
            return true;
        }

        public bool Eater_ss()
        {
            if (Util.GetProblematicEnemyMonster() == null && Bot.ExtraDeck.Count < 5) return false;
            if (Bot.GetMonstersInMainZone().Count >= 5) return false;
            if (Util.IsTurn1OrMain2()) return false;
            AI.SelectPosition(CardPosition.FaceUpAttack);
            IList<ClientCard> targets = new List<ClientCard>();
            if (Bot.SpellZone[5] != null && !Bot.SpellZone[5].IsCode(CardId.Stage))
            {
                targets.Add(Bot.SpellZone[5]);
            }
            if (Bot.SpellZone[5] != null && Bot.SpellZone[5].IsCode(CardId.Stage) && Bot.HasInHand(CardId.Stage))
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
            Logger.DebugWriteLine("*** Eater use up the extra deck.");
            foreach (ClientCard s_c in Bot.GetSpells())
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

        public bool Eater_eff()
        {
            if (Enemy.BattlingMonster.HasPosition(CardPosition.Attack) && (Bot.BattlingMonster.Attack - Enemy.BattlingMonster.GetDefensePower() >= Enemy.LifePoints)) return false;
            return true;
        }

        public void Red_SelectPos(ClientCard return_card = null)
        {
            int self_power = (Bot.HasInHand(CardId.White) && !white_eff_used) ? 3200 : 1600;
            if (Duel.Player == 0)
            {
                List<ClientCard> monster_list = Bot.GetMonsters();
                monster_list.Sort(CardContainer.CompareCardAttack);
                monster_list.Reverse();
                foreach(ClientCard card in monster_list)
                {
                    if (IsTrickstar(card.Id) && card != return_card && card.HasPosition(CardPosition.Attack))
                    {
                        int this_power = (Bot.HasInHand(CardId.White) && !white_eff_used) ? (card.RealPower + card.Attack) : card.RealPower;
                        if (this_power >= self_power) self_power = this_power;
                    } else if (card.RealPower >= self_power) self_power = card.RealPower;
                }
            }
            ClientCard bestenemy = Util.GetOneEnemyBetterThanValue(self_power, true);
            if (bestenemy != null) AI.SelectPosition(CardPosition.FaceUpDefence);
            else AI.SelectPosition(CardPosition.FaceUpAttack);
            return;
        }

        public bool Red_ss()
        {
            if (red_ss_count >= 6) return false;
            if ((Util.ChainContainsCard(CardId.DarkHole) || Util.ChainContainsCard(99330325) || Util.ChainContainsCard(53582587)) && Util.ChainContainsCard(CardId.Red)) return false;
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Red))
            {
                foreach (ClientCard m in Bot.GetMonsters())
                {
                    if (Util.IsChainTarget(m) && IsTrickstar(m.Id))
                    {
                        red_ss_count += 1;
                        AI.SelectCard(m);
                        Red_SelectPos();
                        return true;
                    }
                }
            }
            if (Duel.LastChainPlayer == 1) return true;
            if (Duel.Player == 0)
            {
                if (Util.IsTurn1OrMain2()) return false;
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    List<ClientCard> self_m = Bot.GetMonsters();
                    ClientCard tosolve_enemy = Util.GetOneEnemyBetterThanMyBest();
                    foreach (ClientCard c in self_m)
                    {
                        if (IsTrickstar(c.Id) && !c.IsCode(CardId.Red))
                        {
                            if (c.Attacked)
                            {
                                AI.SelectCard(c);
                                Red_SelectPos(c);
                                red_ss_count += 1;
                                return true;
                            }
                            if (c.IsCode(CardId.Pink)) return false;
                            if (tosolve_enemy != null)
                            {
                                if (Bot.HasInHand(CardId.White) && c.Attack + c.BaseAttack < tosolve_enemy.Attack)
                                {
                                    if (tosolve_enemy.Attack > 3200) AI.SelectPosition(CardPosition.FaceUpDefence);
                                    AI.SelectCard(c);
                                    Red_SelectPos(c);
                                    red_ss_count += 1;
                                    return true;
                                }
                                if (!Bot.HasInHand(CardId.White) && tosolve_enemy.Attack <= 3200 && c.IsCode(CardId.White))
                                {
                                    AI.SelectCard(c);
                                    Red_SelectPos(c);
                                    red_ss_count += 1;
                                    return true;
                                }
                                if (!Bot.HasInHand(CardId.White) && c.Attack < tosolve_enemy.Attack)
                                {
                                    if (!c.Attacked)
                                    {
                                        ClientCard badatk = Enemy.GetMonsters().GetLowestAttackMonster();
                                        ClientCard baddef = Enemy.GetMonsters().GetLowestDefenseMonster();
                                        int enemy_power = 99999;
                                        if (badatk != null && badatk.Attack <= enemy_power) enemy_power = badatk.Attack;
                                        if (baddef != null && baddef.Defense <= enemy_power) enemy_power = baddef.Defense;
                                        if (c.Attack > enemy_power) return false;
                                    }
                                    if (tosolve_enemy.Attack > 1600) AI.SelectPosition(CardPosition.FaceUpDefence);
                                    AI.SelectCard(c);
                                    Red_SelectPos(c);
                                    red_ss_count += 1;
                                    return true;
                                }
                            }
                        }
                    }
                }
            } else
            {
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (Util.GetOneEnemyBetterThanMyBest() != null)
                    {
                        List<ClientCard> self_monster = Bot.GetMonsters();
                        self_monster.Sort(CardContainer.CompareDefensePower);
                        foreach(ClientCard card in self_monster)
                        {
                            if (IsTrickstar(card.Id) && !card.IsCode(CardId.Red))
                            {
                                AI.SelectCard(card);
                                Red_SelectPos(card);
                                red_ss_count += 1;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool Yellow_eff()
        {
            if (!Bot.HasInHand(CardId.Stage) && !Bot.HasInSpellZone(CardId.Stage) && Bot.GetRemainingCount(CardId.Stage, 3) > 0)
            {
                AI.SelectCard(CardId.Stage, CardId.Red, CardId.White, CardId.Pink, CardId.Re, CardId.Crown, CardId.Yellow);
                return true;
            }
            if (Enemy.LifePoints <= 1000)
            {
                if (Bot.GetRemainingCount(CardId.Pink, 1) > 0 && !pink_ss)
                {
                    AI.SelectCard(CardId.Pink, CardId.Stage, CardId.Red, CardId.White, CardId.Re, CardId.Crown, CardId.Yellow);
                    return true;
                }
                else if (Bot.HasInGraveyard(CardId.Pink) && Bot.GetRemainingCount(CardId.Crown, 1) > 0)
                {
                    AI.SelectCard(CardId.Crown, CardId.Pink, CardId.Re, CardId.Stage, CardId.Red, CardId.White, CardId.Yellow);
                    return true;
                }
            }
            if (Enemy.GetMonsterCount() == 0 && !Util.IsTurn1OrMain2())
            {
                if (Bot.HasInGraveyard(CardId.Red) && Bot.GetRemainingCount(CardId.Pink, 1) > 0 && !pink_ss)
                {
                    AI.SelectCard(CardId.Pink, CardId.Red, CardId.White, CardId.Re, CardId.Stage, CardId.Crown, CardId.Yellow);
                }
                else if (Bot.HasInGraveyard(CardId.Pink) && Bot.HasInGraveyard(CardId.Red) && Bot.GetRemainingCount(CardId.Ring, 1) > 0)
                {
                    AI.SelectCard(CardId.Crown, CardId.Red, CardId.White, CardId.Re, CardId.Stage, CardId.Pink, CardId.Yellow);
                }
                else if (Bot.GetRemainingCount(CardId.White, 2) > 0 && Enemy.LifePoints <= 4000)
                {
                    AI.SelectCard(CardId.White, CardId.Red, CardId.Pink, CardId.Re, CardId.Stage, CardId.Crown, CardId.Yellow);
                }
                else if (Bot.HasInGraveyard(CardId.White) && Bot.GetRemainingCount(CardId.Crown, 1) > 0)
                {
                    AI.SelectCard(CardId.Crown, CardId.Red, CardId.Pink, CardId.Re, CardId.Stage, CardId.White, CardId.Yellow);
                }
                else
                {
                    AI.SelectCard(CardId.Red, CardId.Pink, CardId.Re, CardId.Crown, CardId.Stage, CardId.White, CardId.Yellow);
                }
                return true;
            }
            if (Util.GetProblematicEnemyMonster() != null)
            {
                int power = Util.GetProblematicEnemyMonster().GetDefensePower();
                if (power >= 1800 && power <= 3600 && Bot.GetRemainingCount(CardId.White, 2) > 0 && !Bot.HasInHand(CardId.White))
                {
                    AI.SelectCard(CardId.White, CardId.Red, CardId.Pink, CardId.Re, CardId.Stage, CardId.Crown, CardId.Yellow);
                }
                else
                {
                    AI.SelectCard(CardId.Red, CardId.Pink, CardId.Re, CardId.Crown, CardId.Stage, CardId.White, CardId.Yellow);
                }
                return true;
            }
            if ((Bot.HasInHand(CardId.Red) || Bot.HasInHand(CardId.Stage) || Bot.HasInHand(CardId.Yellow)) && Bot.GetRemainingCount(CardId.Re,1) > 0) {
                AI.SelectCard(CardId.Re, CardId.Red, CardId.White, CardId.Crown, CardId.Pink, CardId.Stage, CardId.Yellow);
                return true;
            }
            AI.SelectCard(CardId.Red, CardId.Pink, CardId.Re, CardId.Crown, CardId.Stage, CardId.White, CardId.Yellow);
            return true;
        }

        public bool White_eff()
        {
            if (Duel.Phase >= DuelPhase.Main2) return false;
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                if (Bot.BattlingMonster == null || Enemy.BattlingMonster == null || !IsTrickstar(Bot.BattlingMonster.Id) || Bot.BattlingMonster.HasPosition(CardPosition.Defence)) return false;
                if (Bot.BattlingMonster.Attack <= Enemy.BattlingMonster.RealPower && Bot.BattlingMonster.Attack + Bot.BattlingMonster.BaseAttack >= Enemy.BattlingMonster.RealPower)
                {
                    white_eff_used = true;
                    return true;
                }
                return false; 
            } else
            {
                if (Enemy.GetMonsterCount() == 0 && !Util.IsTurn1OrMain2()) {
                    white_eff_used = true;
                    return true;
                }
                else if (Enemy.GetMonsterCount() != 0)
                {
                    ClientCard tosolve = Util.GetBestEnemyMonster(true);
                    ClientCard self_card = Bot.GetMonsters().GetHighestAttackMonster();
                    if (tosolve == null || self_card == null || (tosolve != null && self_card != null && !IsTrickstar(self_card.Id)))
                    {
                        if (Enemy.GetMonsters().GetHighestAttackMonster()== null ||
                            Enemy.GetMonsters().GetHighestDefenseMonster() == null ||
                            Enemy.GetMonsters().GetHighestAttackMonster().GetDefensePower() < 2000 ||
                            Enemy.GetMonsters().GetHighestDefenseMonster().GetDefensePower() < 2000)
                        {
                            white_eff_used = true;
                            return true;
                        }
                        else return false;
                    }
                    if (tosolve != null && self_card != null && IsTrickstar(self_card.Id) && !tosolve.IsMonsterHasPreventActivationEffectInBattle())
                    {
                        int defender_power = tosolve.GetDefensePower();
                        Logger.DebugWriteLine("battle check 0:" + Duel.Phase.ToString());
                        Logger.DebugWriteLine("battle check 1:" + self_card.Attack.ToString());
                        Logger.DebugWriteLine("battle check 2:" + (self_card.Attack+self_card.BaseAttack).ToString());
                        Logger.DebugWriteLine("battle check 3:" + defender_power.ToString());
                        if (self_card.Attack <= defender_power && self_card.Attack + self_card.BaseAttack >= defender_power)
                        {
                            return false;
                        }
                        else if (defender_power <= 2000)
                        {
                            white_eff_used = true;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool LockBird_act()
        {
            if (Duel.Player == 0 || lockbird_used) return false;
            lockbird_useful = true;
            if (Bot.HasInSpellZone(CardId.Re))
            {
                if (Util.ChainContainsCard(CardId.Re)) lockbird_used = true;
                return Util.ChainContainsCard(CardId.Re);
            }
            lockbird_used = true;
            return true;
        }

        public bool Reincarnation()
        {
            if (Card.Location == CardLocation.Grave) return Ts_reborn();
            if (!spell_trap_activate()) return false;
            if (Bot.HasInHand(CardId.LockBird))
            {
                if (lockbird_useful || Util.IsChainTarget(Card) || (Duel.Player == 1 && Util.ChainContainsCard(CardId.Feather))) {
                    lockbird_useful = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool Crown_eff()
        {
            if (Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.HasPosition(CardPosition.FaceDown)))
            {
                if (!spell_trap_activate()) return false;
                if (Duel.Phase <= DuelPhase.Main1 && Ts_reborn())
                {
                    AI.SelectPlace(SelectSTPlace());
                    return true;
                }
                return false;
            }
            if (Bot.HasInHand(CardId.Pink) && GraveCall_id != CardId.Pink)
            {
                AI.SelectCard(CardId.Pink);
                return true;
            }
            if (Enemy.GetMonsterCount() == 0)
            {
                foreach(ClientCard hand in Bot.Hand)
                {
                    if (hand.IsMonster() && IsTrickstar(hand.Id))
                    {
                        if (hand.Attack >= Enemy.LifePoints) return true;
                        if (!hand.IsCode(CardId.Yellow))
                        {
                            if (Util.GetOneEnemyBetterThanValue(hand.Attack, false) == null) return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Ts_reborn()
        {
            if (Util.IsTurn1OrMain2()) return false;
            if (Duel.Player == 0 && Enemy.LifePoints <= 1000)
            {
                AI.SelectCard(CardId.Pink);
                return true;
            }
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
                    AI.SelectCard(CardId.Red, CardId.White, CardId.Yellow, CardId.Pink);
                    return true;
                }
            }
            else
            {
                
                AI.SelectCard(CardId.Red, CardId.White, CardId.Yellow, CardId.Pink);
                return true;
            }
        }

        public bool Yellow_sum()
        {
            NormalSummoned = true;
            return true;
        }

        public bool Red_sum()
        {
            if ((Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 1800) || (Duel.Turn == 1 && Bot.HasInHand(CardId.Re)))
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool Pink_sum()
        {
            if (Enemy.LifePoints <= 1000)
            {
                NormalSummoned = true;
                return true;
            }
            else if (!Util.IsTurn1OrMain2() && (Bot.HasInGraveyard(CardId.Yellow) || Bot.HasInGraveyard(CardId.Red)))
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool Tuner_ns()
        {
            if ((Card.IsCode(CardId.Tuner) && Bot.HasInExtra(CardId.Crystal) && !tuner_eff_used) || Tuner_ss())
            {
                NormalSummoned = true;
                return true;
            }
            return false;
        }

        public bool Tuner_ss()
        {
            if (crystal_eff_used || Bot.HasInMonstersZone(CardId.Crystal)) return false;
            if (Bot.GetMonsterCount() == 0 || !Bot.HasInExtra(CardId.Crystal)) return false;
            if (Card.IsCode(CardId.Ghost) && Bot.GetRemainingCount(CardId.Ghost, 2) <= 0) return false;
            int count = 0;
            if (!Card.IsCode(CardId.Urara)) count += 1;
            foreach(ClientCard hand in Bot.Hand)
            {
                if (hand.IsCode(Card.Id)) count += 1;
            }
            if (count < 2) return false;
            foreach(ClientCard m in Bot.GetMonsters())
            {
                if (!m.IsCode(CardId.Eater) && getLinkMarker(m.Id) <= 2) return true;
            }
            return false;
        }

        public bool Tuner_eff()
        {
            tuner_eff_used = true;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public bool Ring_act()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.Ghost)) return false;
            if (!spell_trap_activate()) return false;
            ClientCard target = Util.GetProblematicEnemyMonster();
            if (target == null && Util.IsChainTarget(Card))
            {
                target = Util.GetBestEnemyMonster();
            }
            if (target != null)
            {
                if (Bot.LifePoints <= target.Attack) return false;
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        public bool Linkuri_ss()
        {
            foreach(ClientCard c in Bot.GetMonsters())
            {
                if (!c.IsCode(CardId.Eater, CardId.Linkuri, CardId.Linkspi) && c.Level == 1)
                {
                    AI.SelectCard(c);
                    return true;
                }
            }
            return false;
        }

        public bool Linkuri_eff()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.Linkuri)) return false;
            AI.SelectCard(CardId.Tuner, CardId.BF + 1);
            return true;
        }

        public bool Crystal_ss()
        {
            if (crystal_eff_used) return false;
            if (Bot.HasInMonstersZone(CardId.BF) && Bot.HasInMonstersZone(CardId.BF + 1))
            {
                AI.SelectCard(CardId.BF, CardId.BF + 1);
                return true;
            }
            foreach(ClientCard extra_card in Bot.GetMonstersInExtraZone())
            {
                if (getLinkMarker(extra_card.Id) >= 5) return false;
            }
            IList<ClientCard> targets = new List<ClientCard>();
            foreach(ClientCard t_check in Bot.GetMonsters())
            {
                if (t_check.IsFacedown()) continue;
                if (t_check.IsCode(CardId.BF, CardId.Tuner, CardId.Urara, CardId.Ghost))
                {
                    targets.Add(t_check);
                    break;
                }
            }
            if (targets.Count == 0) return false;
            List<ClientCard> m_list = new List<ClientCard>(Bot.GetMonsters());
            m_list.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard e_check in m_list)
            {
                if (e_check.IsFacedown()) continue;
                if (targets[0] != e_check && getLinkMarker(e_check.Id) <= 2 && !e_check.IsCode(CardId.Eater, CardId.Crystal))
                {
                    targets.Add(e_check);
                    break;
                }
            }
            if (targets.Count <= 1) return false;
            AI.SelectMaterials(targets);
            return true;
        }

        public bool Crystal_eff()
        {
            if (Duel.Player == 0)
            {
                crystal_eff_used = true;
                AI.SelectCard(CardId.Tuner, CardId.Ghost, CardId.Urara);
                return true;
            }
            else if (Util.IsChainTarget(Card) || Util.GetProblematicEnemySpell() != null) return true;
            else if (Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Util.IsOneEnemyBetterThanValue(1500,true)) {
                if (Util.IsOneEnemyBetterThanValue(1900, true))
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                else
                {
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                }
                return true;
            }
            return false;
        }

        public bool TG_eff()
        {
            if (Card.Location != CardLocation.MonsterZone) return true;
            ClientCard target = Util.GetProblematicEnemySpell();
            IList<ClientCard> list = new List<ClientCard>();
            if (target != null) list.Add(target);
            foreach(ClientCard spells in Enemy.GetSpells())
            {
                if (spells != null && !list.Contains(spells)) list.Add(spells);
            }
            AI.SelectCard(list);
            return true;
        }

        public bool Safedragon_ss()
        {
            if (Util.IsTurn1OrMain2()) return false;
            ClientCard m = Util.GetProblematicEnemyMonster();
            foreach(ClientCard ex_m in Bot.GetMonstersInExtraZone())
            {
                if (getLinkMarker(ex_m.Id) >= 4) return false;
            }
            if ((m == null || m.HasPosition(CardPosition.FaceDown)) && Enemy.LifePoints <= 1100)
            {
                if (Enemy.GetMonsterCount() == 0 && Duel.Phase < DuelPhase.Battle)
                {
                    IList<ClientCard> list = new List<ClientCard>();
                    foreach(ClientCard monster in Bot.GetMonsters())
                    {
                        if (getLinkMarker(monster.Id) <= 2) list.Add(monster);
                        if (list.Count == 2) break;
                    }
                    if (list.Count == 2 && GetTotalATK(list) <= 1100)
                    {
                        AI.SelectMaterials(list);
                        return true;
                    }
                    return false;
                }
            }
            ClientCard ex_1 = Bot.MonsterZone[5];
            ClientCard ex_2 = Bot.MonsterZone[6];
            ClientCard ex = null;
            if (ex_1 != null && ex_1.Controller == 0) ex = ex_1;
            if (ex_2 != null && ex_2.Controller == 0) ex = ex_2;
            if (ex == null) return false;
            if (!ex.HasLinkMarker(2)) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard s_m in Bot.GetMonsters())
            {
                if (s_m.IsCode(CardId.Eater)) continue;
                if (s_m != Bot.MonsterZone[5] && s_m != Bot.MonsterZone[6]) targets.Add(s_m);
                if (targets.Count == 2) break;
            }
            if (targets.Count == 2)
            {
                AI.SelectMaterials(targets);
                return true;
            }
            return false;
        }

        public bool Phoneix_ss()
        {
            ClientCard m = Util.GetProblematicEnemySpell();
            if (m == null)
            {
                if (Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 1900 && Duel.Phase == DuelPhase.Main1) 
                {
                    IList<ClientCard> m_list = new List<ClientCard>();
                    List<ClientCard> list = new List<ClientCard>(Bot.GetMonsters());
                    list.Sort(CardContainer.CompareCardAttack);
                    foreach(ClientCard monster in list)
                    {
                        if (getLinkMarker(monster.Id) == 1 && monster.IsFaceup()) m_list.Add(monster);
                        if (m_list.Count == 2) break;
                    }
                    if (m_list.Count == 2 && GetTotalATK(m_list) <= 1900)
                    {
                        AI.SelectMaterials(m_list);
                        return true;
                    }
                }
                return false;
            }
            if (Bot.Hand.Count == 0) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            List<ClientCard> main_list = new List<ClientCard>(Bot.GetMonstersInMainZone());
            main_list.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard s_m in main_list)
            {
                if (s_m.IsFacedown()) continue;
                if ((!s_m.IsCode(CardId.Eater) || (s_m.IsCode(CardId.Eater) && s_m.IsDisabled())) && !targets.ContainsCardWithId(s_m.Id))
                {
                    targets.Add(s_m);
                };
                if (targets.Count == 2) break;
            }
            if (targets.Count < 2)
            {
                foreach (ClientCard s_m in Bot.GetMonstersInExtraZone())
                {
                    if (s_m.IsFacedown()) continue;
                    if (!s_m.IsCode(CardId.Eater) && !targets.ContainsCardWithId(s_m.Id))
                    {
                        targets.Add(s_m);
                    };
                    if (targets.Count == 2) break;
                }
            }
            if (targets.Count < 2) return false;
            AI.SelectMaterials(targets);
            return true;
        }

        public bool Phoneix_eff()
        {
            AI.SelectCard(Useless_List());
            ClientCard target = Util.GetProblematicEnemySpell();
            if (target != null)
            {
                AI.SelectNextCard(target);
            } else
            {
                List<ClientCard> spells = Enemy.GetSpells();
                RandomSort(spells);
                foreach (ClientCard card in spells)
                {
                    if ((card != stage_locked || card.HasPosition(CardPosition.FaceUp)) && !(card.IsShouldNotBeTarget() || card.IsShouldNotBeMonsterTarget())) AI.SelectNextCard(card);
                }
            }
            return true;
        }

        public bool Unicorn_ss() {
            ClientCard m = Util.GetProblematicEnemyCard();
            int link_count = 0;
            if (m == null)
            {
                if (Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 2200 && Duel.Phase == DuelPhase.Main1)
                {
                    IList<ClientCard> m_list = new List<ClientCard>();
                    List<ClientCard> _sort_list = new List<ClientCard>(Bot.GetMonsters());
                    _sort_list.Sort(CardContainer.CompareCardAttack);
                    foreach(ClientCard monster in _sort_list)
                    {
                        if (getLinkMarker(monster.Id) == 2)
                        {
                            link_count += 2;
                            m_list.Add(monster);
                        } else if (getLinkMarker(monster.Id) == 1 && monster.IsFaceup())
                        {
                            link_count += 1;
                            m_list.Add(monster);
                        }
                        if (link_count >= 3) break;
                    }
                    if (link_count >= 3 && GetTotalATK(m_list) <= 2200)
                    {
                        AI.SelectMaterials(m_list);
                        return true;
                    }
                }
                return false;
            }
            if (Bot.Hand.Count == 0) return false;
            IList<ClientCard> targets = new List<ClientCard>();
            List<ClientCard> sort_list = Bot.GetMonsters();
            sort_list.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard s_m in sort_list)
            {
                if ((!s_m.IsCode(CardId.Eater) || (s_m.IsCode(CardId.Eater) && m.IsMonsterHasPreventActivationEffectInBattle())) && getLinkMarker(s_m.Id) <= 2 && s_m.IsFaceup())
                {
                    if (!targets.ContainsCardWithId(s_m.Id))
                    {
                        targets.Add(s_m);
                        link_count += getLinkMarker(s_m.Id);
                    }
                    if (link_count >= 3) break;
                }
            }
            if (link_count < 3) return false;
            AI.SelectMaterials(targets);
            return true;
        }

        public bool Unicorn_eff()
        {
            ClientCard m = Util.GetProblematicEnemyCard();
            if (m == null) return false;
            // avoid cards that cannot target.
            AI.SelectCard(Useless_List());
            IList<ClientCard> enemy_list = new List<ClientCard>();
            if (!m.IsShouldNotBeMonsterTarget() && !m.IsShouldNotBeTarget()) enemy_list.Add(m);
            foreach(ClientCard enemy in Enemy.GetMonstersInExtraZone())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            foreach (ClientCard enemy in Enemy.GetMonstersInMainZone())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            foreach (ClientCard enemy in Enemy.GetSpells())
            {
                if (enemy != null && !enemy_list.Contains(enemy) && !enemy.IsShouldNotBeMonsterTarget() && !enemy.IsShouldNotBeTarget()) enemy_list.Add(enemy);
            }
            if (enemy_list.Count > 0)
            {
                AI.SelectNextCard(enemy_list);
                return true;
            } else
            {
                return false;
            }
        }

        public bool Snake_ss()
        {
            IList<ClientCard> targets = new List<ClientCard>();
            // exzone fo first
            foreach (ClientCard e_m in Bot.GetMonstersInExtraZone())
            {
                if (e_m.Attack < 1900 && !targets.ContainsCardWithId(e_m.Id) && e_m.IsFaceup())
                {
                    targets.Add(e_m);
                }
            }
            List<ClientCard> sort_main_list = new List<ClientCard>(Bot.GetMonstersInMainZone());
            sort_main_list.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard m in sort_main_list)
            {
                if (m.Attack < 1900 && !targets.ContainsCardWithId(m.Id) && m.IsFaceup())
                {
                    targets.Add(m);
                }
                if (targets.Count >= 4)
                {
                    if (Enemy.LifePoints <= GetTotalATK(targets) && Enemy.GetMonsterCount() == 0) return false;
                    AI.SelectMaterials(targets);
                    AI.SelectYesNo(true);
                    snake_four_s = true;
                    return true;
                }
            }
            return false;
        }

        public bool Snake_eff()
        {
            if (snake_four_s)
            {
                snake_four_s = false;
                AI.SelectCard(Useless_List());
                return true;
            }
            //if (ActivateDescription == Util.GetStringId(CardId.Snake, 2)) return true;
            if (ActivateDescription == Util.GetStringId(CardId.Snake, 1))
            {
                foreach(ClientCard hand in Bot.Hand)
                {
                    if (hand.IsCode(CardId.Red, CardId.Pink))
                    {
                        AI.SelectCard(hand);
                        return true;
                    }
                    if (hand.IsCode(CardId.Urara, CardId.Ghost))
                    {
                        if (Tuner_ss())
                        {
                            AI.SelectCard(hand);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Missus_ss()
        {
            IList<ClientCard> material_list = new List<ClientCard>();
            foreach(ClientCard monster in Bot.GetMonsters())
            {
                if (monster.HasAttribute(CardAttribute.Earth) && getLinkMarker(monster.Id) == 1) material_list.Add(monster);
                if (material_list.Count == 2) break;
            }
            if (material_list.Count < 2) return false;
            if (Enemy.GetMonsterCount() == 0 || Util.GetProblematicEnemyMonster(2000) == null)
            {
                AI.SelectMaterials(material_list);
                return true;
            } else if (Util.GetProblematicEnemyMonster(2000) != null && Bot.HasInExtra(CardId.Borrel) && !Bot.HasInMonstersZone(CardId.Missus))
            {
                AI.SelectMaterials(material_list);
                return true;
            }
            return false;
        }

        public bool Missus_eff()
        {
            AI.SelectCard(CardId.MG, CardId.Missus, CardId.Snake);
            return true;
        }

        public bool Borrel_ss()
        {
            bool already_link2 = false;
            IList<ClientCard> material_list = new List<ClientCard>();
            if (Util.GetProblematicEnemyMonster(2000) == null) Logger.DebugWriteLine("***borrel:null");
            else Logger.DebugWriteLine("***borrel:" + (Util.GetProblematicEnemyMonster(2000).Name ?? "unknown"));
            if (Util.GetProblematicEnemyMonster(2000) != null || (Enemy.GetMonsterCount() == 0 && Duel.Phase == DuelPhase.Main1 && Enemy.LifePoints <= 3000))
            {
                foreach(ClientCard e_m in Bot.GetMonstersInExtraZone())
                {
                    if (getLinkMarker(e_m.Id) < 3)
                    {
                        if (getLinkMarker(e_m.Id) == 2) already_link2 = true;
                        material_list.Add(e_m);
                    }
                }
                List<ClientCard> sort_list = new List<ClientCard>(Bot.GetMonstersInMainZone());
                sort_list.Sort(CardContainer.CompareCardAttack);

                foreach(ClientCard m in sort_list)
                {
                    if (getLinkMarker(m.Id) < 3)
                    {
                        if (getLinkMarker(m.Id) == 2 && !already_link2)
                        {
                            already_link2 = true;
                            material_list.Add(m);
                        } else if (!m.IsCode(CardId.Sheep + 1, CardId.Eater))
                        {
                            material_list.Add(m);
                        }
                        if ((already_link2 && material_list.Count == 3) || (!already_link2 && material_list.Count == 4)) break;
                    }
                }
                if ((already_link2 && material_list.Count == 3) || (!already_link2 && material_list.Count == 4))
                {
                    if (Enemy.GetMonsterCount() == 0 && Duel.Phase == DuelPhase.Main1 && Enemy.LifePoints <= 3000)
                    {
                        if (GetTotalATK(material_list) >= 3000) return false;
                    }
                    AI.SelectMaterials(material_list);
                    return true;
                }
            }
            return false;
        }

        public bool Borrel_eff()
        {
            if (ActivateDescription == -1) {
                ClientCard enemy_monster = Enemy.BattlingMonster;
                if (enemy_monster != null && enemy_monster.HasPosition(CardPosition.Attack))
                {
                    return (Card.Attack - enemy_monster.Attack < Enemy.LifePoints);
                }
                return true;
            };
            ClientCard BestEnemy = Util.GetBestEnemyMonster(true);
            ClientCard WorstBot = Bot.GetMonsters().GetLowestAttackMonster();
            if (BestEnemy == null || BestEnemy.HasPosition(CardPosition.FaceDown)) return false;
            if (WorstBot == null || WorstBot.HasPosition(CardPosition.FaceDown)) return false;
            if (BestEnemy.Attack >= WorstBot.RealPower)
            {
                AI.SelectCard(BestEnemy);
                return true;
            }
            return false;
        }

        // unfinished.
        public bool GraveCall_eff()
        {
            if (!spell_trap_activate()) return false;
            if (Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().IsMonster() && Enemy.HasInGraveyard(Util.GetLastChainCard().Id))
                {
                    GraveCall_id = Util.GetLastChainCard().Id;
                    GraveCall_count = 2;
                    AI.SelectCard(GraveCall_id);
                    return true;
                }
            }
            return false;
        }

        public bool DarkHole_eff()
        {
            if (!spell_trap_activate()) return false;
            if (Bot.GetMonsterCount() == 0)
            {
                
                int bestPower = -1;
                foreach (ClientCard hand in Bot.Hand)
                {
                    if (hand.IsMonster() && hand.Attack > bestPower) bestPower = hand.Attack;
                }
                int bestenemy = -1;
                foreach (ClientCard enemy in Enemy.GetMonsters())
                {
                    if (enemy.IsMonsterDangerous())
                    {
                        AI.SelectPlace(SelectSTPlace());
                        return true;
                    }
                    if (enemy.IsFaceup() && (enemy.GetDefensePower() > bestenemy)) bestenemy = enemy.GetDefensePower();
                }
                if (bestPower <= bestenemy)
                {
                    AI.SelectPlace(SelectSTPlace());
                    return true;
                }
            }
            return false;
        }

        public bool IsAllEnemyBetter()
        {
            int bestPower = -1;
            for (int i = 0; i < 7; ++i)
            {
                ClientCard card = Bot.MonsterZone[i];
                if (card == null || card.Data == null) continue;
                int newPower = card.Attack;
                if (IsTrickstar(card.Id) && Bot.HasInHand(CardId.White) && !white_eff_used) newPower += card.RealPower;
                if (newPower > bestPower)
                    bestPower = newPower;
            }
            return Util.IsAllEnemyBetterThanValue(bestPower,true);
        }

        public bool MonsterRepos()
        {
            if (Card.IsCode(CardId.Eater)) return (!Card.HasPosition(CardPosition.Attack));

            if (IsTrickstar(Card.Id) && !white_eff_used && Bot.HasInHand(CardId.White) && Card.IsAttack() && Duel.Phase == DuelPhase.Main1) return false;

            if (Card.IsFaceup() && Card.IsDefense() && Card.Attack == 0)
                return false;

            if (Card.IsCode(CardId.Pink))
            {
                if ((Bot.HasInSpellZone(CardId.Stage, true) && Enemy.LifePoints <= 1000) || (!Bot.HasInSpellZone(CardId.Stage, true) && Enemy.LifePoints <= 800))
                {
                    return !Card.HasPosition(CardPosition.Attack);
                }
            }

            bool enemyBetter = IsAllEnemyBetter();

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
            crystal_eff_used = false;
            red_ss_count = 0;
            white_eff_used = false;
            lockbird_useful = false;
            lockbird_used = false;
            if (GraveCall_count > 0)
            {
                if (--GraveCall_count <= 0)
                {
                    GraveCall_id = 0;
                }                
            }
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            ClientCard lowestattack = null;
            for (int i = defenders.Count - 1; i >= 0; --i)
            {
                ClientCard defender = defenders[i];
                if (defender.HasPosition(CardPosition.Attack) && !defender.IsMonsterDangerous() && (lowestattack == null || defender.Attack < lowestattack.Attack)) lowestattack = defender;
            }
            if (lowestattack != null && attacker.Attack - lowestattack.Attack >= Enemy.LifePoints) return AI.Attack(attacker, lowestattack);
            for (int i = 0; i < defenders.Count; ++i)
            {
                ClientCard defender = defenders[i];

                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();

                if (!defender.IsMonsterHasPreventActivationEffectInBattle() && !attacker.IsDisabled())
                {
                    if ((attacker.IsCode(CardId.Eater) && !defender.HasType(CardType.Token)) || attacker.IsCode(CardId.Borrel)) return AI.Attack(attacker, defender);
                    if ((attacker.IsCode(CardId.Ultimate, CardId.Cardian)) && attacker.RealPower > defender.RealPower) return AI.Attack(attacker, defender);
                }

                if (!OnPreBattleBetween(attacker, defender))
                    continue;

                if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            return null;
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.IsCode(CardId.Borrel, CardId.Eater)) return attacker;
            }
            return null;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (IsTrickstar(attacker.Id) && Bot.HasInHand(CardId.White) && !white_eff_used)
                    attacker.RealPower = attacker.RealPower + attacker.Attack;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }
    }
}
