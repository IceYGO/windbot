using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Altergeist", "AI_Altergeist")]
    public class AltergeistExecutor : DefaultExecutor
    {

        public class CardId
        {
            public const int Kunquery = 52927340;
            public const int Marionetter = 53143898;
            public const int Multifaker = 42790071;
            public const int AB_JS = 14558127;
            public const int GO_SR = 59438930;
            public const int GR_WC = 62015408;
            public const int GB_HM = 73642296;
            public const int Silquitous = 89538537;
            public const int MaxxC = 23434538;
            public const int Meluseek = 25533642;
            public const int OneForOne = 2295440;
            public const int Feather = 18144506;
            public const int PotofDesires = 35261759;
            public const int Impermanence = 10045474;
            public const int WakingtheDragon = 10813327;
            public const int EvenlyMatched = 15693423;
            public const int Storm = 23924608;
            public const int Manifestation = 35146019;
            public const int Protocol = 27541563;
            public const int Spoofing = 53936268;
            public const int ImperialOrder = 61740673;
            public const int SolemnStrike = 40605147;
            public const int SolemnJudgment = 41420027;
            public const int NaturalExterio = 99916754;
            public const int UltimateFalcon = 86221741;
            public const int Borrelsword = 85289965;
            public const int FWD = 05043010;
            public const int TripleBurstDragon = 49725936;
            public const int HeavymetalfoesElectrumite = 24094258;
            public const int Isolde = 59934749;
            public const int Hexstia = 1508649;
            public const int Needlefiber = 50588353;
            public const int Kagari = 63288573;
            public const int Shizuku = 90673288;
            public const int Linkuriboh = 41999284;
            public const int Anima = 94259633;


            public const int DarkHole = 53129443;
            public const int NaturalBeast = 33198837;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecreel = 51452091;
            public const int Anti_Spell = 58921041;
            public const int Hayate = 8491308;
            public const int Raye = 26077387;
            public const int Drones_Token = 52340445;
            public const int Iblee = 10158145;
        }

        List<int> Impermanence_list = new List<int>();
        bool Multifaker_ssfromhand = false;
        bool Multifaker_ssfromdeck = false;
        bool Marionetter_reborn = false;
        bool Hexstia_searched = false;
        bool Meluseek_searched = false;
        bool summoned = false;
        bool Silquitous_bounced = false;
        bool Silquitous_recycled = false;

        List<int> SkyStrike_list = new List<int> {
            CardId.Raye, CardId.Hayate, CardId.Kagari, CardId.Shizuku,
            21623008, 25955749, 63166095, 99550630,
            25733157, 51227866, CardId.Drones_Token-1,98338152,
            24010609, 97616504, 50005218
        };

        public AltergeistExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // negate
            AddExecutor(ExecutorType.Activate, _CardId.ChickenGame, ChickenGame);
            AddExecutor(ExecutorType.Repos, EvenlyMatched_Repos);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, G_activate);
            AddExecutor(ExecutorType.Activate, CardId.Anti_Spell, Anti_Spell_activate);
            AddExecutor(ExecutorType.Activate, CardId.Hexstia, Hexstia_eff);
            AddExecutor(ExecutorType.Activate, CardId.NaturalExterio, NaturalExterio_eff);
            AddExecutor(ExecutorType.Activate, CardId.TripleBurstDragon, TripleBurstDragon_eff);
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialOrder_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrike_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, SolemnJudgment_activate);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_negate_better);
            AddExecutor(ExecutorType.Activate, CardId.Impermanence, Impermanence_activate);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_negate);
            AddExecutor(ExecutorType.Activate, CardId.AB_JS, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.GB_HM, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.GO_SR, Hand_act_eff);

            AddExecutor(ExecutorType.Activate, CardId.GR_WC, GR_WC_activate);
            AddExecutor(ExecutorType.Activate, CardId.WakingtheDragon, WakingtheDragon_eff);

            // clear
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched, EvenlyMatched_activate);
            AddExecutor(ExecutorType.Activate, CardId.Feather, Feather_activate);
            AddExecutor(ExecutorType.Activate, CardId.Storm, Storm_activate);

            AddExecutor(ExecutorType.Activate, CardId.Silquitous, Silquitous_eff);

            AddExecutor(ExecutorType.Activate, CardId.Borrelsword, Borrelsword_eff);

            // spsummon
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboh_eff);
            AddExecutor(ExecutorType.Activate, CardId.Multifaker, Multifaker_handss);
            AddExecutor(ExecutorType.Activate, CardId.Manifestation, Manifestation_eff);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_activate_not_use);
            AddExecutor(ExecutorType.SpSummon, CardId.Anima, Anima_ss);
            AddExecutor(ExecutorType.Activate, CardId.Anima);
            AddExecutor(ExecutorType.Activate, CardId.Needlefiber, Needlefiber_eff);
            
            AddExecutor(ExecutorType.SpSummon, CardId.Hexstia, Hexstia_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuriboh_ss);

            // effect
            AddExecutor(ExecutorType.Activate, CardId.Spoofing, Spoofing_eff);
            AddExecutor(ExecutorType.Activate, CardId.Kunquery, Kunquery_eff);
            AddExecutor(ExecutorType.Activate, CardId.Marionetter, Marionetter_eff);
            AddExecutor(ExecutorType.Activate, CardId.Meluseek, Meluseek_eff);
            AddExecutor(ExecutorType.Activate, CardId.Multifaker, Multifaker_deckss);

            // summon
            AddExecutor(ExecutorType.Activate, CardId.OneForOne, OneForOne_activate);
            AddExecutor(ExecutorType.Summon, CardId.Meluseek, Meluseek_summon);
            AddExecutor(ExecutorType.Summon, CardId.Marionetter, Marionetter_summon);
            AddExecutor(ExecutorType.Summon, CardId.GR_WC, tuner_summon);
            AddExecutor(ExecutorType.SpSummon, CardId.Needlefiber, Needlefiber_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Borrelsword, Borrelsword_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.TripleBurstDragon, TripleBurstDragon_ss);

            // normal
            AddExecutor(ExecutorType.Activate, CardId.PotofDesires, PotofDesires_activate);
            AddExecutor(ExecutorType.Summon, CardId.Silquitous, Silquitous_summon);
            AddExecutor(ExecutorType.Summon, CardId.Multifaker, Multifaker_summon);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public bool EvenlyMatched_ready()
        {
            if (Bot.HasInHand(CardId.EvenlyMatched) && Bot.GetSpellCount() == 0)
            {
                if (Duel.Phase < DuelPhase.Main2 && Enemy.GetFieldCount() >= 3
                    && Bot.HasInMonstersZone(CardId.Iblee)) return true;
            }
            return false;
        }

        public bool EvenlyMatched_Repos()
        {
            if (EvenlyMatched_ready())
            {
                return (!Card.HasPosition(CardPosition.Attack));
            }
            return false;
        }

        public bool isAltergeist(int id)
        {
            return (id == CardId.Marionetter || id == CardId.Hexstia || id == CardId.Protocol
                || id == CardId.Multifaker || id == CardId.Meluseek || id == CardId.Kunquery
                || id == CardId.Manifestation || id == CardId.Silquitous);
        }

        public int GetSequence(ClientCard card)
        {
            if (Card.Location != CardLocation.MonsterZone) return -1;
            for (int i = 0; i < 7; ++i)
            {
                if (Bot.MonsterZone[i] == card) return i;
            }
            return -1;
        }

        public bool Protocol_activing()
        {
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (card.Id == CardId.Protocol && card.IsFaceup() && !card.IsDisabled()) return true;
            }
            return false;
        }

        public int GetTotalATK(IList<ClientCard> list)
        {
            int atk = 0;
            foreach (ClientCard c in list)
            {
                if (c == null) continue;
                atk += c.Attack;
            }
            return atk;
        }

        public int SelectSTPlace(ClientCard card=null, bool avoid_Impermanence = false)
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
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    return zone;
                };
            }
            return 0;
        }

        public int SelectSetPlace(List<int> avoid_list=null)
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
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
                    return zone;
                };
            }
            return 0;
        }

        public bool spell_trap_activate(bool isCounter = false, ClientCard target = null)
        {
            if (target == null) target = Card;
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return true;
            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !Bot.HasInHandOrHasInMonstersZone(CardId.GO_SR) && !isCounter && !Bot.HasInSpellZone(CardId.SolemnStrike)) return false;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true) && !Bot.HasInHandOrHasInMonstersZone(CardId.GO_SR) && !isCounter && !Bot.HasInSpellZone(CardId.SolemnStrike)) return false;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return false;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return false;
                return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(CardId.RoyalDecreel, true) || Bot.HasInSpellZone(CardId.RoyalDecreel, true)) return false;
                return true;
            }
            // how to get here?
            return false;
        }

        public void SelectAlterLocation(int id, List<ClientCard> ignore_cards = null, bool from_extra = false)
        {
            if (Bot.HasInMonstersZone(CardId.Hexstia))
            {
                for (int i = 0; i < 7; ++i)
                {
                    if (i == 4) continue;
                    ClientCard card = Bot.MonsterZone[i];
                    if (card != null && card.Id == CardId.Hexstia)
                    {
                        int next_index = get_Hexstia_linkzone(i);
                        ClientCard nextcard = Bot.MonsterZone[next_index];
                        if (nextcard == null || (ignore_cards != null && ignore_cards.Contains(nextcard))){
                            Logger.DebugWriteLine("Select Place(next to hex): " + next_index.ToString());
                            AI.SelectPlace((int)System.Math.Pow(2, next_index));
                            return;
                        }
                    }
                }
            }
            if (id == CardId.Hexstia)
            {
                if (from_extra)
                {
                    bool to_extra = (Bot.GetMonstersExtraZoneCount() == 0);
                    foreach(ClientCard card in Bot.GetMonstersInExtraZone())
                    {
                        if (ignore_cards.Contains(card))
                        {
                            to_extra = true;
                            break;
                        }
                    }
                    if (to_extra)
                    {
                        if (   (Bot.MonsterZone[3] != null && !isAltergeist(Bot.MonsterZone[3].Id)) // occupied
                            || (Bot.MonsterZone[1] != null && isAltergeist(Bot.MonsterZone[1].Id) && !ignore_cards.Contains(Bot.MonsterZone[1])) ) // point to
                        {
                            AI.SelectPlace(Zones.z5);
                        } else if ((Bot.MonsterZone[1] != null && !isAltergeist(Bot.MonsterZone[1].Id)) // occupied
                            || (Bot.MonsterZone[3] != null && isAltergeist(Bot.MonsterZone[3].Id) && !ignore_cards.Contains(Bot.MonsterZone[3]))) // point to
                        {
                            AI.SelectPlace(Zones.z6);
                        } else if (Bot.MonsterZone[1] == null)
                        {
                            AI.SelectPlace(Zones.z5);
                        } else
                        {
                            AI.SelectPlace(Zones.z6);
                        }
                        return;
                    }
                }
                for (int i = 1; i < 5; ++i)
                {
                    ClientCard card = Bot.MonsterZone[i];
                    if (card != null && isAltergeist(card.Id))
                    {
                        ClientCard nextcard = Bot.MonsterZone[i - 1];
                        if (nextcard == null || (ignore_cards != null && ignore_cards.Contains(nextcard)))
                        {
                            Logger.DebugWriteLine("Select Place(hex): " + (i - 1).ToString());
                            AI.SelectPlace((int)System.Math.Pow(2, i - 1));
                            return;
                        }
                    }
                }
            }
            return;
        }

        public void RandomSort(List<ClientCard> list)
        {

            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                ClientCard temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
        }

        public int get_Hexstia_linkzone(int zone)
        {
            if (zone >= 0 && zone < 4) return zone + 1;
            if (zone == 5) return 1;
            if (zone == 6) return 3;
            return -1;
        }

        public ClientCard GetFloodgate_Alter(bool canBeTarget = false)
        {
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card != null && card.IsFloodgate() && card.IsFaceup() 
                    && card.IsTrap()
                    && (!canBeTarget || !card.IsShouldNotBeTarget()))
                    return card;
            }
            return null;
        }

        public ClientCard GetProblematicEnemyCard_Alter(bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = GetFloodgate_Alter(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;
            List<ClientCard> enemy_monsters = Enemy.GetMonsters();
            enemy_monsters.Sort(AIFunctions.CompareCardAttack);
            enemy_monsters.Reverse();
            foreach(ClientCard target in enemy_monsters)
            {
                if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || target.HasType(CardType.Link))
                {
                    if (target.Id == CardId.Kagari || target.Id == CardId.Shizuku) continue;
                    if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                }
            }

            return null;
        }

        public ClientCard GetBestEnemyCard_random()
        {
            // monsters
            ClientCard card = AI.Utils.GetProblematicEnemyMonster(0, true);
            if (card != null)
                return card;
            card = Enemy.MonsterZone.GetHighestAttackMonster(true);
            if (card != null)
                return card;
            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count > 0)
            {
                RandomSort(monsters);
                return monsters[0];
            }

            // spells
            List<ClientCard> enemy_spells = Enemy.GetSpells();
            RandomSort(enemy_spells);
            foreach(ClientCard sp in enemy_spells)
            {
                if (sp.IsFaceup()) return sp;
            }
            if (enemy_spells.Count > 0) return enemy_spells[0];

            return null;
        }

        public bool bot_can_s_Meluseek()
        {
            if (Duel.Player != 0) return false;
            foreach(ClientCard card in Bot.GetMonsters())
            {
                if (card.Id == CardId.Meluseek && !card.IsDisabled() && !card.Attacked) return true;
            }
            if (Bot.HasInMonstersZone(CardId.Meluseek)) return true;
            if (Bot.HasInMonstersZone(CardId.Marionetter) && !Marionetter_reborn && Bot.HasInGraveyard(CardId.Meluseek)) return true;
            if (!summoned
                && (Bot.HasInHand(CardId.Meluseek)
                || (Bot.HasInHand(CardId.Marionetter) && Bot.HasInGraveyard(CardId.Meluseek)))
                )return true;
            return false;
        }

        public bool SpellSet()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            if (Card.Id == CardId.EvenlyMatched && !Bot.HasInHandOrInSpellZone(CardId.Spoofing)
                && !Bot.HasInHandOrInSpellZone(CardId.Protocol) && !Bot.HasInHandOrInSpellZone(CardId.ImperialOrder)) return false;
            if (Card.Id == CardId.EvenlyMatched && Bot.HasInSpellZone(CardId.EvenlyMatched)) return false;
            if (Card.Id == CardId.SolemnStrike && Bot.LifePoints <= 1500) return false;
            if (Card.Id == CardId.Spoofing && Bot.HasInSpellZone(CardId.Spoofing)) return false;
            if (Card.Id == CardId.Manifestation && Bot.HasInHandOrInSpellZone(CardId.Spoofing))
            {
                bool can_activate = false;
                foreach(ClientCard g in Bot.Graveyard)
                {
                    if (g.IsMonster() && isAltergeist(g.Id))
                    {
                        can_activate = true;
                        break;
                    }
                }
                if (!can_activate) return false;
            }
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int Impermanence_set = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        Impermanence_set += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(CardId.Impermanence))
                {
                    if (Card.Id == CardId.Impermanence)
                    {
                        AI.SelectPlace(Impermanence_set);
                        return true;
                    } else
                    {
                        AI.SelectPlace(SelectSetPlace(avoid_list));
                        return true;
                    }
                } else
                {
                    AI.SelectPlace(SelectSTPlace());
                }
                return true;
            }
            else if (Enemy.HasInSpellZone(CardId.Anti_Spell, true) || Bot.HasInSpellZone(CardId.Anti_Spell, true))
            {
                if (Card.IsSpell())
                {
                    AI.SelectPlace(SelectSTPlace());
                    return true;
                }
            }
            return false;
        }

        public bool ChickenGame()
        {
            Logger.DebugWriteLine("Use override");
            if (!spell_trap_activate()) return false;
            if (Bot.LifePoints <= 1000)
                return false;
            if (Bot.LifePoints - 1000 <= Enemy.LifePoints && ActivateDescription == AI.Utils.GetStringId(_CardId.ChickenGame, 0))
            {
                Logger.DebugWriteLine("CG: draw");
                return true;
            }
            if (Bot.LifePoints - 1000 > Enemy.LifePoints && ActivateDescription == AI.Utils.GetStringId(_CardId.ChickenGame, 1))
            {
                Logger.DebugWriteLine("CG: drstroy");
                return true;
            }
            return false;
        }

        public bool Anti_Spell_activate()
        {
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (card.Id == CardId.Anti_Spell && card.IsFaceup()) return false;
            }
            return true;
        }

        public bool G_activate()
        {
            return (Duel.Player == 1);
        }

        public bool NaturalExterio_eff()
        {
            if (Duel.LastChainPlayer != 0)
            {
                AI.SelectCard(new[]
                {
                    CardId.Feather,
                    CardId.PotofDesires,
                    CardId.OneForOne,
                    CardId.GO_SR,
                    CardId.AB_JS,
                    CardId.GR_WC,
                    CardId.MaxxC,
                    CardId.Spoofing,
                    CardId.SolemnJudgment,
                    CardId.SolemnStrike,
                    CardId.ImperialOrder,
                    CardId.Spoofing,
                    CardId.Storm,
                    CardId.EvenlyMatched,
                    CardId.WakingtheDragon,
                    CardId.Impermanence,
                    CardId.Marionetter
                });
                return true;
            }
            return false;
        }

        public bool SolemnStrike_activate()
        {
            return (DefaultSolemnStrike() && spell_trap_activate(true));
        }

        public bool SolemnJudgment_activate()
        {
            if ((DefaultSolemnJudgment() && spell_trap_activate(true)))
            {
                ClientCard target = AI.Utils.GetLastChainCard();
                if (target != null && !target.IsMonster() && !spell_trap_activate(false, target)) return false;
                return true;
            }
            return false;
        }

        public bool Impermanence_activate()
        {
            if (!spell_trap_activate()) return false;
            foreach(ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++ i)
                        {
                            if (Bot.SpellZone[i] == Card)
                            {
                                Impermanence_list.Add(i);
                                break;
                            }
                        }
                    }
                    if (Card.Location == CardLocation.Hand)
                    {
                        AI.SelectPlace(SelectSTPlace(Card, true));
                    }
                    AI.SelectCard(m);
                    return true;
                }
            }

            ClientCard LastChainCard = AI.Utils.GetLastChainCard();

            if (LastChainCard == null)
                return false;
            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone)
                    {
                        if (Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    } else if (Duel.Player == 0 && AI.Utils.GetProblematicEnemySpell() != null)
                    {
                        if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                    }
                }
                if ((this_seq * that_seq >= 0 && (this_seq + that_seq == 4))
                    || (AI.Utils.IsChainTarget(Card))
                    || (Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && !Multifaker_ssfromdeck && !Multifaker_ssfromhand))
                {
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(AIFunctions.CompareCardAttack);
                    foreach(ClientCard card in enemy_monsters)
                    {
                        if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                        {
                            AI.SelectCard(card);
                            Impermanence_list.Add(this_seq);
                            return true;
                        }
                    }
                }
            }
            if (LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget())
                return false;
            // negate monsters
            if (Card.Location == CardLocation.SpellZone)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card)
                    {
                        Impermanence_list.Add(i);
                        break;
                    }
                }
            }
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }
            AI.SelectCard(LastChainCard);
            return true;
        }

        public bool Hand_act_eff()
        {
            if (Card.Id == CardId.GO_SR && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.GO_SR)) return false;
            return (Duel.LastChainPlayer == 1);
        }

        public bool WakingtheDragon_eff()
        {
            if (Bot.HasInExtra(CardId.NaturalExterio))
            {
                bool has_skystriker = false;
                foreach(ClientCard card in Enemy.Graveyard)
                {
                    if (card != null && SkyStrike_list.Contains(card.Id))
                    {
                        has_skystriker = true;
                        break;
                    }
                }
                if (!has_skystriker)
                {
                    foreach (ClientCard card in Enemy.GetSpells())
                    {
                        if (card != null && SkyStrike_list.Contains(card.Id))
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
                        if (card != null && SkyStrike_list.Contains(card.Id))
                        {
                            has_skystriker = true;
                            break;
                        }
                    }
                }
                if (has_skystriker)
                {
                    AI.SelectCard(CardId.NaturalExterio);
                    return true;
                }
            }
            List<ClientCard> ex_list = new List<ClientCard>();
            foreach(ClientCard card in Bot.ExtraDeck)
            {
                if (card != null) ex_list.Add(card);
            }
            ex_list.Sort(AIFunctions.CompareCardAttack);
            ex_list.Reverse();
            AI.SelectCard(ex_list);
            return true;
        }

        public bool GR_WC_activate()
        {
            int warrior_count = 0;
            int pendulum_count = 0;
            int link_count = 0;
            int altergeis_count = 0;
            bool has_skystriker_acer = false;
            bool has_tuner = false;
            foreach (ClientCard card in Enemy.MonsterZone)
            {
                if (card == null) continue;
                if (card.Id == CardId.Kagari || card.Id == CardId.Shizuku || card.Id == CardId.Hayate || card.Id == CardId.Raye || card.Id == CardId.Drones_Token) has_skystriker_acer = true;
                if (card.HasType(CardType.Pendulum)) pendulum_count ++;
                if ((card.Race & (int)CardRace.Warrior) != 0) warrior_count ++;
                if (card.IsTuner() && (Enemy.GetMonsterCount() >= 2)) has_tuner = true;
                if (isAltergeist(card.Id)) altergeis_count++;
                link_count += (card.HasType(CardType.Link) ? card.LinkCount : 1);
            }
            if (has_skystriker_acer)
            {
                if (!Enemy.HasInBanished(CardId.Kagari) && Bot.HasInExtra(CardId.Kagari))
                {
                    AI.SelectCard(CardId.Kagari);
                    return true;
                } else if (!Enemy.HasInBanished(CardId.Shizuku) && Bot.HasInExtra(CardId.Shizuku))
                {
                    AI.SelectCard(CardId.Shizuku);
                    return true;
                }
            }
            if (pendulum_count >= 2 && !Enemy.HasInBanished(CardId.HeavymetalfoesElectrumite) && Bot.HasInExtra(CardId.HeavymetalfoesElectrumite))
            {
                AI.SelectCard(CardId.HeavymetalfoesElectrumite);
                return true;
            }
            if (warrior_count >= 2 && !Enemy.HasInBanished(CardId.Isolde) && Bot.HasInExtra(CardId.Isolde) && !Enemy.HasInMonstersZone(CardId.Isolde))
            {
                AI.SelectCard(CardId.Isolde);
                return true;
            }
            if (has_tuner && !Enemy.HasInBanished(CardId.Needlefiber) && Bot.HasInExtra(CardId.Needlefiber) && !Enemy.HasInMonstersZone(CardId.Needlefiber))
            {
                AI.SelectCard(CardId.Needlefiber);
                return true;
            }
            if (altergeis_count > 0 && !Enemy.HasInBanished(CardId.Hexstia) && Bot.HasInExtra(CardId.Hexstia))
            {
                AI.SelectCard(CardId.Hexstia);
                return true;
            }
            if (link_count >= 4)
            {
                if ((Bot.HasInMonstersZone(CardId.UltimateFalcon) || Bot.HasInMonstersZone(CardId.NaturalExterio)) && !Enemy.HasInBanished(CardId.Borrelsword) && Bot.HasInExtra(CardId.Borrelsword) && !Enemy.HasInMonstersZone(CardId.Borrelsword))
                {
                    AI.SelectCard(CardId.Borrelsword);
                    return true;
                }
                if (!Enemy.HasInBanished(CardId.FWD) && Bot.HasInExtra(CardId.FWD) && !Enemy.HasInMonstersZone(CardId.FWD))
                {
                    AI.SelectCard(CardId.FWD);
                    return true;
                }
            }

            return false;
        }

        public bool ImperialOrder_activate()
        {
            if (!Card.HasPosition(CardPosition.FaceDown)) return true;
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card.IsSpell() && spell_trap_activate()) return true;
            }
            if (Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && (!Multifaker_ssfromhand && !Multifaker_ssfromdeck)) return true;
            return false;
        }

        public bool EvenlyMatched_activate()
        {
            if (!spell_trap_activate()) return false;
            return true;

            // use after ToBattle fix
            int bot_count = Bot.GetFieldCount();
            if (Card.Location == CardLocation.Hand) bot_count += 1;
            int enemy_count = Enemy.GetFieldCount();
            if (enemy_count - bot_count < 2) return false;

            if (Card.Location == CardLocation.Hand) AI.SelectPlace(SelectSTPlace(Card, true));
            return true;
        }

        public bool Feather_activate()
        {
            if (!spell_trap_activate()) return false;
            if (AI.Utils.GetProblematicEnemySpell() != null)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;
            }
            // activate when more than 2 cards
            if (Enemy.GetSpellCount() <= 1)
                return false;
            AI.SelectPlace(SelectSTPlace(Card, true));
            return true;
        }

        public bool Storm_activate()
        {
            if (!spell_trap_activate()) return false;
            List<ClientCard> select_list = new List<ClientCard>();
            int activate_immediately = 0;
            List<ClientCard> spells = Enemy.GetSpells();
            RandomSort(spells);
            foreach(ClientCard card in spells)
            {
                if (card != null)
                {
                    if (card.IsFaceup())
                    {
                        if (card.HasType(CardType.Equip) || card.HasType(CardType.Pendulum) || card.HasType(CardType.Field) || card.HasType(CardType.Continuous))
                        {
                            select_list.Add(card);
                            activate_immediately += 1;
                        }
                    }
                }
            }
            foreach (ClientCard card in spells)
            {
                if (card != null)
                {
                    if (card.IsFacedown())
                    {
                        select_list.Add(card);
                    }
                }
            }
            if (Duel.Phase == DuelPhase.End || activate_immediately >= 2)
            {
                if (select_list.Count > 0)
                {
                    AI.SelectCard(select_list);
                    return true;
                }
            }
            return false;
        }

        public bool Kunquery_eff()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (AI.Utils.ChainContainsCard(CardId.Linkuriboh)) return false;
                    if (Bot.BattlingMonster == null || (Enemy.BattlingMonster.Attack >= Bot.BattlingMonster.GetDefensePower()) || Enemy.BattlingMonster.IsMonsterDangerous())
                    {
                        SelectAlterLocation(CardId.Kunquery);
                        AI.SelectPosition(CardPosition.FaceUpDefence);
                        return true;
                    }
                }
                return false;
            } else
            {
                ClientCard target = AI.Utils.GetBestEnemyCard(true, true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                return false;
            }
        }

        public bool Marionetter_eff()
        {
            if (ActivateDescription == -1)
            {
                if (!Bot.HasInHandOrInSpellZone(CardId.Protocol) && Bot.GetRemainingCount(CardId.Protocol,2) > 0)
                {
                    AI.SelectCard(new[] {
                        CardId.Protocol,
                        CardId.Manifestation
                    });
                    AI.SelectPlace(SelectSetPlace());
                    return true;
                } else
                {
                    AI.SelectCard(new[] {
                        CardId.Manifestation,
                        CardId.Protocol
                    });
                    AI.SelectPlace(SelectSetPlace());
                    return true;
                }
            }
            else
            {
                int next_card = 0;
                bool choose_other = false;
                if (!AI.Utils.IsTurn1OrMain2())
                {
                    if (Bot.HasInGraveyard(CardId.Hexstia) && AI.Utils.GetProblematicEnemySpell() == null && AI.Utils.GetOneEnemyBetterThanValue(3100, true) == null)
                    {
                        next_card = CardId.Hexstia;
                        choose_other = true;
                    } else
                    {
                        ClientCard self_best = AI.Utils.GetBestBotMonster(true);
                        ClientCard enemy_best = AI.Utils.GetProblematicEnemyCard(self_best.Attack + 1, true);
                        if (enemy_best != null && Bot.HasInGraveyard(CardId.Meluseek))
                        {
                            next_card = CardId.Meluseek;
                        }
                    }
                    
                }
                else
                {
                    if (!Meluseek_searched && !Bot.HasInMonstersZone(CardId.Meluseek) && Bot.HasInGraveyard(CardId.Meluseek))
                    {
                        if (!Multifaker_ssfromdeck && Bot.HasInGraveyard(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Meluseek,3) > 0)
                        {
                            next_card = CardId.Multifaker;
                        } else
                        {
                            next_card = CardId.Meluseek;
                        }
                    }
                    else if (!Multifaker_ssfromdeck && Bot.HasInGraveyard(CardId.Multifaker))
                    {
                        next_card = CardId.Multifaker;
                    }
                    else if (Bot.HasInGraveyard(CardId.Hexstia))
                    {
                        next_card = CardId.Hexstia;
                        choose_other = !(Bot.GetMonsterCount() > 1 || Bot.HasInHand(CardId.Multifaker));
                    }
                    else if (Bot.HasInGraveyard(CardId.Silquitous))
                    {
                        int alter_count = 0;
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (isAltergeist(card.Id) && (card.IsTrap() || (!summoned && card.IsMonster()))) alter_count ++;
                        }
                        foreach (ClientCard s in Bot.GetSpells())
                        {
                            if (isAltergeist(s.Id)) alter_count++;
                        }
                        foreach(ClientCard m in Bot.GetMonsters())
                        {
                            if (isAltergeist(m.Id) && m != Card) alter_count++;
                        }
                        if (alter_count > 0)
                        {
                            next_card = CardId.Silquitous;
                        }
                    }
                }
                if (next_card != 0)
                {
                    int Protocol_count = 0;
                    foreach (ClientCard h in Bot.Hand)
                    {
                        if (h.Id == CardId.Protocol) Protocol_count++;
                    }
                    foreach (ClientCard s in Bot.GetSpells())
                    {
                        if (s.Id == CardId.Protocol) Protocol_count += (s.IsFaceup() ? 11 : 1);
                    }
                    if (Protocol_count >= 12)
                    {
                        AI.SelectCard(CardId.Protocol);
                        AI.SelectNextCard(next_card);
                        SelectAlterLocation(next_card);
                        Marionetter_reborn = true;
                        return true;
                    }
                    List<ClientCard> list = Bot.GetMonsters();
                    list.Sort(AIFunctions.CompareCardAttack);
                    foreach (ClientCard card in list)
                    {
                        if (isAltergeist(card.Id) && !(choose_other && card == Card))
                        {
                            AI.SelectCard(card);
                            AI.SelectNextCard(next_card);
                            SelectAlterLocation(next_card, new List<ClientCard> { card });
                            Marionetter_reborn = true;
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                };
            }
            return false;
        }

        public bool Hexstia_eff()
        {
            if (Card.Location == CardLocation.MonsterZone && Duel.LastChainPlayer != 0 && (Protocol_activing() || !Card.IsDisabled()))
            {
                ClientCard target =  AI.Utils.GetLastChainCard();
                if (target != null && !spell_trap_activate(false, target)) return false;
                // check
                int this_seq = GetSequence(Card);
                if (this_seq != -1) this_seq = get_Hexstia_linkzone(this_seq);
                if (this_seq != -1)
                {
                    ClientCard linked_card = Bot.MonsterZone[this_seq];
                    if (linked_card != null && linked_card.Id == CardId.Hexstia)
                    {
                        int next_seq = get_Hexstia_linkzone(this_seq);
                        if (next_seq != -1 && Bot.MonsterZone[next_seq] != null && isAltergeist(Bot.MonsterZone[next_seq].Id)) return false;
                    }
                }
                return true;
            }
            if (Card.Location == CardLocation.MonsterZone) return false;
            if (Enemy.HasInSpellZone(82732705) && Bot.GetRemainingCount(CardId.Protocol,3) > 0)
            {
                AI.SelectCard(CardId.Protocol);
                return true;
            }
            if (Duel.Player == 0 && !summoned && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
            {
                AI.SelectCard(CardId.Marionetter);
                return true;
            }
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 3) > 0)
            {
                AI.SelectCard(CardId.Multifaker);
                return true;
            }
            if (!Bot.HasInHandOrInSpellZone(CardId.Manifestation) && Bot.GetRemainingCount(CardId.Manifestation,2) > 0)
            {
                AI.SelectCard(CardId.Manifestation);
                return true;
            }
            if (!Bot.HasInHandOrInSpellZone(CardId.Protocol) && Bot.GetRemainingCount(CardId.Protocol, 2) > 0)
            {
                AI.SelectCard(CardId.Protocol);
                return true;
            }
            AI.SelectCard(new[]
            {
                    CardId.Meluseek,
                    CardId.Kunquery,
                    CardId.Marionetter,
                    CardId.Multifaker,
                    CardId.Manifestation,
                    CardId.Protocol,
                    CardId.Silquitous
                });
            return true;
        }

        public bool Meluseek_eff()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                ClientCard target = AI.Utils.GetProblematicEnemyCard(0, true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                target = AI.Utils.GetOneEnemyBetterThanMyBest(true, true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                List<ClientCard> targets = Enemy.GetSpells();
                RandomSort(targets);
                if (targets.Count > 0)
                {
                    AI.SelectCard(targets[0]);
                    return true;
                }
                target = AI.Utils.GetBestEnemyCard(false, true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            } else
            {
                if (Duel.Player == 1)
                {
                    if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 3) > 0)
                    {
                        foreach(ClientCard set_card in Bot.GetSpells())
                        {
                            if (set_card.IsFacedown() && set_card.Id != CardId.WakingtheDragon)
                            {
                                AI.SelectCard(CardId.Multifaker);
                                return true;
                            }
                        }
                    }
                    if (!summoned && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
                    {
                        AI.SelectCard(CardId.Marionetter);
                        return true;
                    }
                }
                else
                {
                    if (!summoned && !Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
                    {
                        AI.SelectCard(CardId.Marionetter);
                        return true;
                    }
                    if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 3) > 0)
                    {
                        AI.SelectCard(CardId.Multifaker);
                        return true;
                    }
                }
                AI.SelectCard(new[]
                {
                    CardId.Kunquery,
                    CardId.Silquitous
                });
                return true;
            }
            return false;
        }

        public bool Multifaker_handss()
        {
            if (Multifaker_ssfromdeck || Card.Location != CardLocation.Hand) return false;
            Multifaker_ssfromhand = true;
            SelectAlterLocation(CardId.Multifaker);
            if (Duel.Player != 0 && AI.Utils.GetOneEnemyBetterThanMyBest() != null) AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public bool Multifaker_deckss()
        {
            if (Card.Location != CardLocation.Hand)
            {
                if (!Silquitous_bounced && !Bot.HasInMonstersZone(CardId.Silquitous) && Bot.GetRemainingCount(CardId.Silquitous,2) > 0)
                {
                    AI.SelectCard(CardId.Silquitous);
                    SelectAlterLocation(CardId.Silquitous);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (!Meluseek_searched && !Bot.HasInMonstersZone(CardId.Meluseek) && Bot.GetRemainingCount(CardId.Meluseek, 3) > 0)
                {
                    AI.SelectCard(CardId.Meluseek);
                    SelectAlterLocation(CardId.Meluseek);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (Bot.GetRemainingCount(CardId.Kunquery,1) > 0)
                {
                    AI.SelectCard(CardId.Kunquery);
                    SelectAlterLocation(CardId.Kunquery);
                    Multifaker_ssfromdeck = true;
                    return true;
                } else
                {
                    AI.SelectCard(CardId.Marionetter);
                    SelectAlterLocation(CardId.Marionetter);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
            }
            return false;
        }

        public bool Silquitous_eff()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInHandOrInSpellZone(CardId.Manifestation) && Bot.HasInGraveyard(CardId.Manifestation))
                {
                    AI.SelectCard(CardId.Manifestation);
                } else
                {
                    AI.SelectCard(CardId.Protocol);
                }
                Silquitous_recycled = true;
                return true;
            }
            else {
                ClientCard bounce_self = null;
                int Protocol_count = 0;
                ClientCard faceup_Protocol = null;
                ClientCard faceup_Manifestation = null;
                ClientCard selected_target = null;
                foreach (ClientCard spell in Bot.GetSpells())
                {
                    if (spell.Id == CardId.Protocol)
                    {
                        if (spell.IsFaceup())
                        {
                            faceup_Protocol = spell;
                            Protocol_count += 11;
                        } else
                        {
                            Protocol_count++;
                        }
                    }
                    if (spell.Id == CardId.Manifestation && spell.IsFaceup()) faceup_Manifestation = spell;
                    if (Duel.LastChainPlayer != 0 && AI.Utils.IsChainTarget(spell) && spell.IsFaceup())
                    {
                        selected_target = spell;
                    }
                }
                if (Protocol_count >= 12)
                {
                    bounce_self = faceup_Protocol;
                } else if (Duel.Player == 0 && faceup_Protocol != null)
                {
                    bounce_self = faceup_Protocol;
                } else if (faceup_Manifestation != null)
                {
                    bounce_self = faceup_Manifestation;
                }
                ClientCard faceup_Multifaker = null;
                ClientCard faceup_monster = null;
                List<ClientCard> monster_list = Bot.GetMonsters();
                monster_list.Sort(AIFunctions.CompareCardAttack);
                //monster_list.Reverse();
                foreach(ClientCard card in monster_list)
                {
                    if (card.IsFaceup() && isAltergeist(card.Id) && card != Card)
                    {
                        if (Duel.LastChainPlayer != 0 && AI.Utils.IsChainTarget(card) && card.IsFaceup())
                        {
                            selected_target = card;
                        }
                        if (faceup_Multifaker == null && card.Id == CardId.Multifaker) faceup_Multifaker = card;
                        if (faceup_monster == null && card.Id != CardId.Hexstia) faceup_monster = card;
                    }
                }
                if (bounce_self == null)
                {
                    if (selected_target != null) bounce_self = selected_target;
                    else if (faceup_Multifaker != null) bounce_self = faceup_Multifaker;
                    else bounce_self = faceup_monster;
                }

                ClientCard card_should_bounce_immediately = GetProblematicEnemyCard_Alter(true);
                if (card_should_bounce_immediately != null && Duel.LastChainPlayer != 0 && !bot_can_s_Meluseek())
                {
                    Logger.DebugWriteLine("Silquitous: dangerous");
                    AI.SelectCard(bounce_self);
                    AI.SelectNextCard(card_should_bounce_immediately);
                    return true;
                }
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (Duel.LastChainPlayer != 0)
                    {
                        Logger.DebugWriteLine("Silquitous: battle");
                        if (AI.Utils.ChainContainsCard(CardId.Linkuriboh)) return false;
                        if (Enemy.BattlingMonster != null && Bot.BattlingMonster != null && Enemy.BattlingMonster.Attack >= Bot.BattlingMonster.GetDefensePower() && !Enemy.BattlingMonster.IsShouldNotBeTarget() && !Enemy.BattlingMonster.IsShouldNotBeMonsterTarget())
                        {
                            if (Bot.HasInMonstersZone(CardId.Kunquery)) AI.SelectCard(CardId.Kunquery);
                            else AI.SelectCard(bounce_self);
                            AI.SelectNextCard(Enemy.BattlingMonster);
                            return true;
                        }
                    }
                } 
                else if (Duel.Phase > DuelPhase.Main2)
                {
                    if (Duel.LastChainPlayer != 0)
                    {
                        Logger.DebugWriteLine("Silquitous: end");
                        ClientCard enemy_card = GetBestEnemyCard_random();
                        if (enemy_card != null)
                        {
                            AI.SelectCard(bounce_self);
                            AI.SelectNextCard(enemy_card);
                            return true;
                        }
                    }
                } else if (Duel.Player == 0)
                {
                    Logger.DebugWriteLine("Silquitous: orenoturn");
                    if (Duel.Phase < DuelPhase.Main2 && summoned && bounce_self.IsMonster()) return false;
                    ClientCard enemy_card = GetBestEnemyCard_random();
                    if (enemy_card != null)
                    {
                        Logger.DebugWriteLine("Silquitousdecide:" + bounce_self.Name);
                        AI.SelectCard(bounce_self);
                        AI.SelectNextCard(enemy_card);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Manifestation_eff()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (AI.Utils.ChainContainsCard(CardId.Silquitous)) return false;
                if (!Bot.HasInHandOrInSpellZone(CardId.Protocol) && !AI.Utils.ChainContainsCard(CardId.Protocol))
                {
                    AI.SelectCard(CardId.Protocol);
                    return true;
                }
                return false;
            }
            else
            {
                if (AI.Utils.ChainContainsCard(CardId.Manifestation)) return false;

                if (Bot.HasInMonstersZone(CardId.Hexstia))
                {
                    bool has_position = false;
                    for (int i = 0; i < 7; ++i)
                    {
                        ClientCard target = Bot.MonsterZone[i];
                        if (target != null && target.Id == CardId.Hexstia)
                        {
                            int next_id = get_Hexstia_linkzone(i);
                            if (next_id != -1)
                            {
                                ClientCard next_card = Bot.MonsterZone[next_id];
                                if (next_card == null)
                                {
                                    has_position = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!has_position) return false;
                }

                if (!Multifaker_ssfromdeck && Bot.HasInGraveyard(CardId.Multifaker))
                {
                    if (Bot.HasInHand(CardId.Multifaker) && Bot.HasInGraveyard(CardId.Silquitous) && Bot.GetRemainingCount(CardId.Silquitous,2) == 0)
                    {
                        AI.SelectCard(CardId.Silquitous);
                        SelectAlterLocation(CardId.Silquitous);
                        return true;
                    } else
                    {
                        AI.SelectCard(CardId.Multifaker);
                        SelectAlterLocation(CardId.Multifaker);
                        return true;
                    }
                }
                List<int> choose_list = new List<int>();
                choose_list.Add(CardId.Hexstia);
                choose_list.Add(CardId.Silquitous);
                choose_list.Add(CardId.Meluseek);
                choose_list.Add(CardId.Marionetter);
                choose_list.Add(CardId.Kunquery);
                foreach(int id in choose_list)
                {
                    if (Bot.HasInGraveyard(id)){
                        AI.SelectCard(id);
                        SelectAlterLocation(id);
                        return true;
                    }
                }

            }
            return false;
        }

        public bool Protocol_negate_better()
        {
            // skip if no one of enemy's monsters is better
            if (ActivateDescription == AI.Utils.GetStringId(CardId.Protocol, 1))
            {
                if (AI.Utils.GetOneEnemyBetterThanMyBest(true) == null) return false;
            }
            return Protocol_negate();
        }

        public bool Protocol_negate()
        {
            // negate
            if (ActivateDescription == AI.Utils.GetStringId(CardId.Protocol, 1))
            {
                List<int> cost_list = new List<int>();
                if (AI.Utils.ChainContainsCard(CardId.Manifestation)) cost_list.Add(CardId.Manifestation);
                cost_list.Add(CardId.Protocol);
                cost_list.Add(CardId.Multifaker);
                cost_list.Add(CardId.Marionetter);
                cost_list.Add(CardId.Kunquery);
                if (Meluseek_searched) cost_list.Add(CardId.Meluseek);
                if (Silquitous_bounced) cost_list.Add(CardId.Silquitous);
                for (int i = 0; i < 7; ++i)
                {
                    ClientCard card = Bot.MonsterZone[i];
                    if (card != null && card.Id == CardId.Hexstia)
                    {
                        int nextzone = get_Hexstia_linkzone(i);
                        if (nextzone != -1)
                        {
                            ClientCard linkedcard = Bot.MonsterZone[nextzone];
                            if (linkedcard == null || !isAltergeist(linkedcard.Id))
                            {
                                cost_list.Add(CardId.Hexstia);
                            }
                        } else
                        {
                            cost_list.Add(CardId.Hexstia);
                        }
                    }
                }
                if (!Silquitous_bounced) cost_list.Add(CardId.Silquitous);
                if (!Meluseek_searched) cost_list.Add(CardId.Meluseek);
                if (!AI.Utils.ChainContainsCard(CardId.Manifestation)) cost_list.Add(CardId.Manifestation);
                AI.SelectCard(cost_list);
                return true;
            }
            return false;
        }

        public bool Protocol_activate_not_use()
        {
            if (AI.Utils.GetLastChainCard() != null && AI.Utils.GetLastChainCard().Controller == 0 && AI.Utils.GetLastChainCard().IsTrap()) return false;
            if (ActivateDescription != AI.Utils.GetStringId(CardId.Protocol, 1))
            {
                if (AI.Utils.IsChainTarget(Card) && Card.IsFacedown())
                {
                    return true;
                }
                if (!Multifaker_ssfromhand && !Multifaker_ssfromdeck && (Bot.HasInHand(CardId.Multifaker) || Bot.HasInSpellZone(CardId.Spoofing)))
                {
                    return true;
                }
                int can_bounce = 0;
                bool should_disnegate = false;
                foreach(ClientCard card in Bot.GetMonsters())
                {
                    if (isAltergeist(card.Id))
                    {
                        if (card.Id == CardId.Silquitous && card.IsFaceup() && !Silquitous_bounced) can_bounce += 10;
                        else if (card.IsFaceup() && card.Id != CardId.Hexstia) can_bounce++;
                        if (card.IsDisabled()) should_disnegate = true;
                    }
                }
                if (can_bounce == 10 || should_disnegate) return true;
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && Bot.HasInHand(CardId.Kunquery) && AI.Utils.GetOneEnemyBetterThanMyBest() != null) return true;
            }
            return false;
        }

        public bool Spoofing_eff()
        {
            if (AI.Utils.ChainContainsCard(CardId.Spoofing)) return false;
            if (!AI.Utils.ChainContainPlayer(0) && !Multifaker_ssfromhand && !Multifaker_ssfromdeck && Bot.HasInHand(CardId.Multifaker) && Card.HasPosition(CardPosition.FaceDown))
            {
                AI.SelectYesNo(false);
                return true;
            }
            bool has_cost = false;
            // cost check(not select)
            if (Card.IsFacedown())
            {
                foreach(ClientCard card in Bot.Hand)
                {
                    if (isAltergeist(card.Id))
                    {
                        has_cost = true;
                        break;
                    }
                }
                if (!has_cost)
                {
                    foreach(ClientCard card in Bot.GetSpells())
                    {
                        if (isAltergeist(card.Id) && card.IsFaceup())
                        {
                            has_cost = true;
                            break;
                        }
                    }
                }
                if (!has_cost)
                {
                    foreach(ClientCard card in Bot.GetMonsters())
                    {
                        if (isAltergeist(card.Id) && card.IsFaceup())
                        {
                            has_cost = true;
                            break;
                        }
                    }
                }
                if (!has_cost)
                {
                    foreach (ClientCard card in Bot.GetSpells())
                    {
                        if (isAltergeist(card.Id) && card.IsFaceup())
                        {
                            has_cost = true;
                            break;
                        }
                    }
                }
                if (!has_cost) return false;
            }
            if (Duel.Player == 1)
            {
                if (!Multifaker_ssfromhand && !Multifaker_ssfromdeck && !Bot.HasInHand(CardId.Multifaker))
                {
                    if (Bot.HasInHand(CardId.Silquitous))
                    {
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (card.Id == CardId.Silquitous)
                            {
                                AI.SelectCard(card);
                                AI.SelectNextCard(new[] {
                                    CardId.Multifaker,
                                    CardId.Kunquery
                                });
                                return true;
                            }
                        }
                    }
                    else
                    {
                        AI.SelectCard(new[]
                        {
                            CardId.Silquitous,
                            CardId.Manifestation,
                            CardId.Kunquery,
                            CardId.Marionetter,
                            CardId.Multifaker,
                            CardId.Protocol,
                            CardId.Meluseek
                        });
                        AI.SelectNextCard(new[]{
                            CardId.Multifaker,
                            CardId.Marionetter,
                            CardId.Meluseek,
                            CardId.Kunquery,
                            CardId.Silquitous
                        });
                        return true;
                    }
                }
            }
            else
            {
                if (Card.IsFacedown() && !Multifaker_ssfromhand && !Multifaker_ssfromdeck)
                {
                    AI.SelectCard(new[]
{
                        CardId.Silquitous,
                        CardId.Manifestation,
                        CardId.Kunquery,
                        CardId.Marionetter,
                        CardId.Multifaker,
                        CardId.Protocol,
                        CardId.Meluseek
                    });
                    AI.SelectNextCard(new[]{
                        CardId.Multifaker,
                        CardId.Marionetter,
                        CardId.Meluseek,
                        CardId.Kunquery,
                        CardId.Silquitous
                    });
                }
                else if (!summoned && !Bot.HasInHand(CardId.Marionetter))
                {
                    if (Bot.HasInHand(CardId.Silquitous))
                    {
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (card.Id == CardId.Silquitous)
                            {
                                AI.SelectCard(card);
                                AI.SelectNextCard(new[]{
                                    CardId.Marionetter,
                                    CardId.Meluseek
                                });
                                return true;
                            }
                        }
                    } else
                    {
                        AI.SelectCard(new[]
                        {
                            CardId.Silquitous,
                            CardId.Manifestation,
                            CardId.Kunquery,
                            CardId.Multifaker,
                            CardId.Protocol,
                            CardId.Meluseek,
                            CardId.Marionetter,
                        });
                        AI.SelectNextCard(new[]{
                            CardId.Marionetter,
                            CardId.Meluseek,
                            CardId.Multifaker,
                            CardId.Kunquery
                        });
                        return true;
                    }
                }
            }
            bool go = false;
            foreach(ClientCard card in Bot.GetSpells())
            {
                if ( (AI.Utils.ChainContainsCard(CardId.Feather) || AI.Utils.IsChainTarget(card)) 
                    && card.IsFaceup() && Duel.LastChainPlayer != 0 && isAltergeist(card.Id))
                {
                    AI.SelectCard(card);
                    go = true;
                    break;
                }
            }
            if (!go)
            {
                foreach (ClientCard card in Bot.GetMonsters())
                {
                    if ( (AI.Utils.IsChainTarget(card) || AI.Utils.ChainContainsCard(CardId.DarkHole))
                        && card.IsFaceup() && Duel.LastChainPlayer != 0 && isAltergeist(card.Id))
                    {
                        AI.SelectCard(card);
                        go = true;
                        break;
                    }
                }
            }
            if (go)
            {
                AI.SelectNextCard(new[]{
                    CardId.Marionetter,
                    CardId.Meluseek,
                    CardId.Multifaker,
                    CardId.Kunquery
                });
                return true;
            }
            return false;
        }

        public bool OneForOne_activate()
        {
            if (!spell_trap_activate()) return false;
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Meluseek) && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Multifaker))
            {
                AI.SelectCard(new[]
                {
                    CardId.GR_WC,
                    CardId.MaxxC,
                    CardId.Kunquery,
                    CardId.GO_SR
                });
                SelectAlterLocation(CardId.Meluseek);
                return true;
            }
            if (!summoned && !Meluseek_searched && !Bot.HasInHand(CardId.Marionetter))
            {
                AI.SelectCard(new[]
                {
                    CardId.GR_WC,
                    CardId.MaxxC,
                    CardId.Kunquery,
                    CardId.GO_SR
                });
                SelectAlterLocation(CardId.Meluseek);
                return true;
            }
            return false;
        }

        public bool Meluseek_summon()
        {
            if (EvenlyMatched_ready()) return false;
            if (Bot.HasInHand(CardId.Marionetter) && Bot.HasInGraveyard(CardId.Meluseek) && !Marionetter_reborn) return false;
            SelectAlterLocation(CardId.Meluseek);
            summoned = true;
            return true;
        }

        public bool Marionetter_summon()
        {
            if (EvenlyMatched_ready()) return false;
            SelectAlterLocation(CardId.Marionetter);
            summoned = true;
            return true;
        }

        public bool Silquitous_summon()
        {
            if (EvenlyMatched_ready()) return false;
            bool can_summon = false;
            foreach (ClientCard card in Bot.Hand)
            {
                if (isAltergeist(card.Id) && card.IsTrap())
                {
                    can_summon = true;
                    break;
                }
            }
            foreach(ClientCard card in Bot.GetMonstersInMainZone())
            {
                if (isAltergeist(card.Id))
                {
                    can_summon = true;
                    break;
                }
            }
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (isAltergeist(card.Id))
                {
                    can_summon = true;
                    break;
                }
            }
            if (can_summon)
            {
                SelectAlterLocation(CardId.Silquitous);
                summoned = true;
                return true;
            } else
            {
                return false;
            }
        }

        public bool Multifaker_summon()
        {
            if (EvenlyMatched_ready()) return false;
            if (Bot.HasInMonstersZone(CardId.Silquitous) || Bot.HasInHandOrInSpellZone(CardId.Spoofing))
            {
                SelectAlterLocation(CardId.Multifaker);
                summoned = true;
                return true;
            }
            return false;
        }

        public bool PotofDesires_activate()
        {
            if (Bot.Deck.Count > 15 && spell_trap_activate())
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;
            }
            return false;
        }

        public bool Anima_ss()
        {
            if (Duel.Phase != DuelPhase.Main2) return false;
            ClientCard card_ex_left = Enemy.MonsterZone[6];
            ClientCard card_ex_right = Enemy.MonsterZone[5];
            if (card_ex_left != null && card_ex_left.HasLinkMarker((int)LinkMarker.Top))
            {
                ClientCard self_card_1 = Bot.MonsterZone[1];
                if (self_card_1 == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z1);
                    return true;
                } else if (self_card_1.Id == CardId.Meluseek)
                {
                    AI.SelectMaterials(self_card_1);
                    AI.SelectPlace(Zones.z1);
                    return true;
                }
            }
            if (card_ex_right != null && card_ex_right.HasLinkMarker((int)LinkMarker.Top))
            {
                ClientCard self_card_2 = Bot.MonsterZone[3];
                if (self_card_2 == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z3);
                    return true;
                }
                else if (self_card_2.Id == CardId.Meluseek)
                {
                    AI.SelectMaterials(self_card_2);
                    AI.SelectPlace(Zones.z3);
                    return true;
                }
            }
            ClientCard card_left = Enemy.MonsterZone[3];
            ClientCard card_right = Enemy.MonsterZone[1];
            if (card_left != null && card_left.IsFacedown()) card_left = null;
            if (card_right != null && card_right.IsFacedown()) card_right = null;
            if (card_left != null && (card_left.IsShouldNotBeMonsterTarget() || card_left.IsShouldNotBeTarget())) card_left = null;
            if (card_right != null && (card_right.IsShouldNotBeMonsterTarget() || card_right.IsShouldNotBeTarget())) card_right = null;
            if (Enemy.MonsterZone[6] != null) card_left = null;
            if (Enemy.MonsterZone[5] != null) card_right = null;
            if (card_left == null && card_right != null)
            {
                if (Bot.MonsterZone[6] == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z6);
                    return true;
                }
            }
            if (card_left != null && card_right == null)
            {
                if (Bot.MonsterZone[5] == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z5);
                    return true;
                }
            }
            if (card_left != null && card_right != null && Bot.GetMonstersExtraZoneCount() == 0)
            {
                int selection = 0;
                if (card_left.IsFloodgate() && !card_right.IsFloodgate()) selection = Zones.z5;
                else if (!card_left.IsFloodgate() && card_right.IsFloodgate()) selection = Zones.z6;
                else
                {
                    if (card_left.GetDefensePower() >= card_right.GetDefensePower()) selection = Zones.z5;
                    else selection = Zones.z6;
                }
                AI.SelectPlace(selection);
                AI.SelectMaterials(CardId.Meluseek);
                return true;
            }
            return false;
        }

        public bool Linkuriboh_ss()
        {
            if (AI.Utils.IsTurn1OrMain2() && !Meluseek_searched)
            {
                AI.SelectPlace(Zones.z5);
                Multifaker_ssfromdeck = true;
                return true;
            }
            return false;
        }

        public bool Linkuriboh_eff()
        {
            if (AI.Utils.ChainContainsCard(CardId.Linkuriboh)) return false;
            if (Duel.Player == 1)
            {
                AI.SelectCard(new[] { CardId.Meluseek });
                Multifaker_ssfromdeck = true;
                return true;
            } else
            {
                if (!summoned && !Bot.HasInHand(CardId.Marionetter) && !Meluseek_searched && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                {
                    AI.SelectCard(new[] { CardId.Meluseek });
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (AI.Utils.IsTurn1OrMain2())
                {
                    AI.SelectCard(new[] { CardId.Meluseek });
                    Multifaker_ssfromdeck = true;
                    return true;
                } else if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && Bot.GetMonsterCount() == 1)
                {
                    foreach (ClientCard card in Bot.GetMonsters())
                    {
                        if (card != null && card.Attacked && card.Id == CardId.Meluseek)
                        {
                            Multifaker_ssfromdeck = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Hexstia_ss()
        {
            List<ClientCard> targets = new List<ClientCard>();
            List<ClientCard> list = Bot.GetMonsters();
            list.Sort(AIFunctions.CompareCardAttack);
            //list.Reverse();
            bool Meluseek_selected = false;
            bool Silquitous_selected = false;
            bool Hexstia_selected = false;
            foreach (ClientCard card in list)
            {
                if (card.Id == CardId.Meluseek)
                {
                    if ((!Meluseek_searched || !Meluseek_selected) && (!summoned || Duel.Phase == DuelPhase.Main2))
                    {
                        Meluseek_selected = true;
                        targets.Add(card);
                    }
                }
                else if (card.Id == CardId.Silquitous)
                {
                    if (!Silquitous_recycled || !Silquitous_selected)
                    {
                        Silquitous_selected = true;
                        targets.Add(card);
                    }
                }
                else if (card.Id == CardId.Hexstia)
                {
                    if ((!Hexstia_searched || !Hexstia_selected) && !summoned && !Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
                    {
                        Hexstia_selected = true;
                        targets.Add(card);
                    }
                }
                else if (isAltergeist(card.Id)) targets.Add(card);
                if (targets.Count >= 2) break;
            }
            if (targets.Count >= 2)
            {
                if (Duel.Phase < DuelPhase.Main2)
                {
                    if (GetTotalATK(targets) >= 1500 && (summoned || (!Meluseek_selected && !Hexstia_selected))) return false;
                }
                if (Hexstia_selected)
                {
                    int count = Bot.GetMonsterCount();
                    if (Bot.HasInHand(CardId.Multifaker)) count++;
                    if (count <= 2) return false;
                }
                AI.SelectMaterials(targets);
                SelectAlterLocation(CardId.Hexstia, targets,true);
                return true;
            }
            return false;
        }

        public bool TripleBurstDragon_eff()
        {
            if (ActivateDescription != AI.Utils.GetStringId(CardId.TripleBurstDragon,0)) return false;
            return (Duel.LastChainPlayer != 0);
        }

        public bool TripleBurstDragon_ss()
        {
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = AI.Utils.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = AI.Utils.GetBestEnemyMonster(true);
                int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
                if (enemy_power <= self_power) return false;
                Logger.DebugWriteLine("Three: enemy: " + enemy_power.ToString() + ", bot: " + self_power.ToString());
                if (enemy_power >= 2401) return false;
            };
            foreach (ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }
            int link_count = 0;
            List<ClientCard> list = new List<ClientCard>();
            if (Bot.HasInMonstersZone(CardId.Needlefiber))
            {
                foreach(ClientCard card in Bot.GetMonsters())
                {
                    if (card.Id == CardId.Needlefiber)
                    {
                        list.Add(card);
                        link_count += 2;
                    }
                }
            }
            List<ClientCard> monsters = Bot.GetMonsters();
            monsters.Sort(AIFunctions.CompareCardAttack);
            //monsters.Reverse();
            foreach(ClientCard card in monsters)
            {
                if (!list.Contains(card) && card.LinkCount < 3)
                {
                    list.Add(card);
                    link_count += (card.HasType(CardType.Link) ? card.LinkCount : 1);
                    if (link_count >= 3) break;
                }
            }
            if (link_count >= 3)
            {
                AI.SelectMaterials(list);
                Multifaker_ssfromdeck = true;
                return true;
            }
            return false;

        }

        public bool Needlefiber_ss()
        {
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = AI.Utils.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = AI.Utils.GetBestEnemyMonster(true);
                int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
                if (enemy_power < self_power) return false;
                if (Bot.GetMonsterCount() >= 4 && enemy_power >= 2501) return false;
                if (Bot.GetMonsterCount() >= 3 && enemy_power >= 2401) return false;
            }
            foreach(ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }
            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> monsters = Bot.GetMonsters();
            monsters.Sort(AIFunctions.CompareCardAttack);
            //monsters.Reverse();
            foreach(ClientCard t in monsters)
            {
                if (t.IsTuner())
                {
                    material_list.Add(t);
                    break;
                }
            }
            foreach(ClientCard m in monsters)
            {
                if (!material_list.Contains(m) && m.LinkCount <= 2)
                {
                    material_list.Add(m);
                    if (material_list.Count >= 2) break;
                }
            }
            AI.SelectMaterials(material_list);
            Multifaker_ssfromdeck = true;
            return true;
        }

        public bool Needlefiber_eff()
        {
            AI.SelectCard(new[] {
                CardId.GR_WC,
                CardId.GO_SR,
                CardId.AB_JS
            });
            return true;
        }

        public bool Borrelsword_ss()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;

            ClientCard self_best = AI.Utils.GetBestBotMonster(true);
            int self_power = (self_best != null) ? self_best.Attack : 0;
            ClientCard enemy_best = AI.Utils.GetBestEnemyMonster(true);
            int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
            if (enemy_power < self_power) return false;

            foreach (ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }

            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> bot_monster = Bot.GetMonsters();
            bot_monster.Sort(AIFunctions.CompareCardAttack);
            //bot_monster.Reverse();
            int link_count = 0;
            foreach(ClientCard card in bot_monster)
            {
                if (card.IsFacedown()) continue;
                if (!material_list.Contains(card) && card.LinkCount < 3)
                {
                    material_list.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                    if (link_count >= 4) break;
                }
            }
            if (link_count >= 4)
            {
                AI.SelectMaterials(material_list);
                Multifaker_ssfromdeck = true;
                return true;
            }
            return false;
        }

        public bool Borrelsword_eff()
        {
            if (ActivateDescription == -1) return true;
            else if ((Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2) || AI.Utils.IsChainTarget(Card))
                {
                    List<ClientCard> enemy_list = Enemy.GetMonsters();
                    enemy_list.Sort(AIFunctions.CompareCardAttack);
                    enemy_list.Reverse();
                    foreach(ClientCard card in enemy_list)
                    {
                        if (card.HasPosition(CardPosition.Attack) && !card.HasType(CardType.Link))
                        {
                            AI.SelectCard(card);
                            return true;
                        }
                    }
                    List<ClientCard> bot_list = Bot.GetMonsters();
                    bot_list.Sort(AIFunctions.CompareCardAttack);
                    //bot_list.Reverse();
                    foreach (ClientCard card in bot_list)
                    {
                        if (card.HasPosition(CardPosition.Attack) && !card.HasType(CardType.Link))
                        {
                            AI.SelectCard(card);
                            return true;
                        }
                    }
                }
            return false;
        }

        public bool tuner_summon()
        {
            if (EvenlyMatched_ready()) return false;
            foreach (ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = AI.Utils.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = AI.Utils.GetBestEnemyMonster(true);
                int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
                Logger.DebugWriteLine("Tuner: enemy: " + enemy_power.ToString() + ", bot: " + self_power.ToString());
                if (enemy_power < self_power) return false;
                if (Bot.GetMonsterCount() >= 3 && enemy_power >= 2501) return false;
                if (Bot.GetMonsterCount() >= 2 && enemy_power >= 2401) return false;
            }
            if (Multifaker_ssfromdeck) return false;
            foreach(ClientCard card in Bot.GetMonsters())
            {
                if (card.IsFaceup())
                {
                    summoned = true;
                    return true;
                }
            }
            return false;
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.Id == CardId.Meluseek) return attacker;
            }
            return null;
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            Multifaker_ssfromhand = false;
            Multifaker_ssfromdeck = false;
            Marionetter_reborn = false;
            Hexstia_searched = false;
            Meluseek_searched = false;
            summoned = false;
            Silquitous_bounced = false;
            Silquitous_recycled = false;
            Impermanence_list.Clear();
        }

        public bool MonsterRepos()
        {
            if (Card.Attack == 0) return (Card.IsAttack());

            if (Card.Id == CardId.Meluseek || Bot.HasInMonstersZone(CardId.Meluseek))
            {
                return Card.HasPosition(CardPosition.Defence);
            }
           
            if (isAltergeist(Card.Id) && Bot.HasInHandOrInSpellZone(CardId.Protocol) && Card.IsFacedown()) return true;

            bool enemyBetter = AI.Utils.IsAllEnemyBetter(true);
            if (Card.IsAttack() && enemyBetter)
                    return true;
            if (Card.IsDefense() && !enemyBetter)
                return true;
            return false;
        }

        // just a test
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (EvenlyMatched_ready())
            {
                List<ClientCard> enemy_m = Enemy.GetMonsters();
                enemy_m.Sort(AIFunctions.CompareCardAttack);
                //enemy_m.Reverse();
                foreach (ClientCard e_card in enemy_m)
                {
                    if (e_card.HasPosition(CardPosition.Attack))
                    {
                        return AI.Attack(attacker, e_card);
                    }
                }
            }
            for (int i = 0; i < defenders.Count; ++i)
            {
                ClientCard defender = defenders[i];
                attacker.RealPower = attacker.Attack;
                defender.RealPower = defender.GetDefensePower();
                if (!OnPreBattleBetween(attacker, defender))
                    continue;
                if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack && (Enemy.GetMonsterCount() == 0 || !attacker.IsDisabled()))
                return AI.Attack(attacker, null);

            return null;
        }
    }
}