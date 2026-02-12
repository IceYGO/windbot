using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

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
            public const int PotofDesires = 35261759;
            public const int PotofIndulgence = 49238328;
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

            public const int SecretVillage = 68462976;

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
        bool ss_other_monster = false;
        List<ClientCard> attacked_Meluseek = new List<ClientCard>();

        List<int> SkyStrike_list = new List<int> {
            CardId.Raye, CardId.Hayate, CardId.Kagari, CardId.Shizuku,
            21623008, 25955749, 63166095, 99550630,
            25733157, 51227866, CardId.Drones_Token-1,98338152,
            24010609, 97616504, 50005218
        };

        List<int> cards_improper = new List<int>
        {
            0,CardId.WakingtheDragon, CardId.SolemnStrike, CardId.Spoofing,   CardId.OneForOne, CardId.PotofDesires,
            CardId.Manifestation, CardId.SecretVillage, CardId.ImperialOrder,   _CardId.HarpiesFeatherDuster, CardId.GR_WC,
            CardId.Protocol, CardId.SolemnJudgment, CardId.Storm, CardId.GO_SR, CardId.Silquitous,
            CardId.MaxxC,  CardId.Impermanence, CardId.Meluseek,   CardId.AB_JS, CardId.Kunquery,
            CardId.Marionetter, CardId.Multifaker
        };

        List<int> normal_counter = new List<int>
        {
            53262004, 98338152, 32617464, 45041488, CardId.SolemnStrike,
            61257789, 23440231, 27354732, 12408276, 82419869, CardId.Impermanence,
            49680980, 18621798, 38814750, 17266660, 94689635,CardId.AB_JS,
            74762582, 75286651, 4810828,  44665365, 21123811, _CardId.CrystalWingSynchroDragon,
            82044279, 82044280, 79606837, 10443957, 1621413,  CardId.Protocol,
            90809975, 8165596,  9753964,  53347303, 88307361, _CardId.GamecieltheSeaTurtleKaiju,
            5818294,  2948263,  6150044,  26268488, 51447164, _CardId.JizukirutheStarDestroyingKaiju,
            97268402
        };

        List<int> should_not_negate = new List<int>
        {
            81275020, 28985331
        };

        public AltergeistExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // negate
            AddExecutor(ExecutorType.Activate, _CardId.ChickenGame, ChickenGame);
            AddExecutor(ExecutorType.Repos, EvenlyMatched_Repos);

            AddExecutor(ExecutorType.Activate, CardId.MaxxC, G_activate);
            AddExecutor(ExecutorType.Activate, CardId.Anti_Spell, Anti_Spell_activate);
            AddExecutor(ExecutorType.Activate, CardId.PotofIndulgence, PotofIndulgence_activate);

            AddExecutor(ExecutorType.Activate, field_activate);
            AddExecutor(ExecutorType.Activate, CardId.SecretVillage, SecretVillage_activate);

            AddExecutor(ExecutorType.Activate, CardId.Hexstia, Hexstia_eff);
            AddExecutor(ExecutorType.Activate, CardId.NaturalExterio, NaturalExterio_eff);
            AddExecutor(ExecutorType.Activate, CardId.TripleBurstDragon, TripleBurstDragon_eff);
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialOrder_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, SolemnStrike_activate);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, SolemnJudgment_activate);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_negate_better);
            AddExecutor(ExecutorType.Activate, CardId.Impermanence, Impermanence_activate);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_negate);
            AddExecutor(ExecutorType.Activate, CardId.Protocol, Protocol_activate_not_use);
            AddExecutor(ExecutorType.Activate, CardId.AB_JS, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.GB_HM, Hand_act_eff);
            AddExecutor(ExecutorType.Activate, CardId.GO_SR, Hand_act_eff);

            AddExecutor(ExecutorType.Activate, CardId.GR_WC, GR_WC_activate);
            AddExecutor(ExecutorType.Activate, CardId.WakingtheDragon, WakingtheDragon_eff);

            // clear
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched, EvenlyMatched_activate);
            AddExecutor(ExecutorType.Activate, _CardId.HarpiesFeatherDuster, Feather_activate);
            AddExecutor(ExecutorType.Activate, CardId.Storm, Storm_activate);

            AddExecutor(ExecutorType.Activate, CardId.Meluseek, Meluseek_eff);
            AddExecutor(ExecutorType.Activate, CardId.Silquitous, Silquitous_eff);

            AddExecutor(ExecutorType.Activate, CardId.Borrelsword, Borrelsword_eff);

            // spsummon
            AddExecutor(ExecutorType.Activate, CardId.Multifaker, Multifaker_handss);
            AddExecutor(ExecutorType.Activate, CardId.Manifestation, Manifestation_eff);
            AddExecutor(ExecutorType.SpSummon, CardId.Anima, Anima_ss);
            AddExecutor(ExecutorType.Activate, CardId.Anima);
            AddExecutor(ExecutorType.Activate, CardId.Needlefiber, Needlefiber_eff);

            // effect
            AddExecutor(ExecutorType.Activate, CardId.Spoofing, Spoofing_eff);
            AddExecutor(ExecutorType.Activate, CardId.Kunquery, Kunquery_eff);
            AddExecutor(ExecutorType.Activate, CardId.Multifaker, Multifaker_deckss);

            // summon
            AddExecutor(ExecutorType.SpSummon, CardId.Hexstia, Hexstia_ss);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, Linkuriboh_ss);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, Linkuriboh_eff);
            AddExecutor(ExecutorType.Activate, CardId.Marionetter, Marionetter_eff);
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
            AddExecutor(ExecutorType.Summon, MonsterSummon);
            AddExecutor(ExecutorType.MonsterSet, MonsterSet);
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

        public bool has_altergeist_left()
        {
            return (Bot.GetRemainingCount(CardId.Marionetter, 3) > 0
                || Bot.GetRemainingCount(CardId.Multifaker, 2) > 0
                || Bot.GetRemainingCount(CardId.Meluseek,3) > 0
                || Bot.GetRemainingCount(CardId.Silquitous,2) > 0
                || Bot.GetRemainingCount(CardId.Kunquery,1) > 0);
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

        public bool isAltergeist(ClientCard card)
        {
            return card != null && card.HasSetcode(0x103);
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

        public bool trap_can_activate(int id)
        {
            if (id == CardId.WakingtheDragon || id == CardId.EvenlyMatched) return false;
            if (id == CardId.SolemnStrike && Bot.LifePoints <= 1500) return false;
            return true;
        }

        public bool Should_counter()
        {
            if (Duel.CurrentChain.Count < 2) return true;
            if (!Protocol_activing()) return true;
            ClientCard self_card = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
            if (self_card?.Controller != 0
                || !(self_card.Location == CardLocation.MonsterZone || self_card.Location == CardLocation.SpellZone)
                || !isAltergeist(self_card)) return true;
            ClientCard enemy_card = Duel.CurrentChain[Duel.CurrentChain.Count - 1];
            if (enemy_card?.Controller != 1
                || !enemy_card.IsCode(normal_counter)) return true;
            return false;
        }

        public bool Should_activate_Protocol()
        {
            if (Duel.CurrentChain.Count < 2) return false;
            if (Protocol_activing()) return false;
            ClientCard self_card = Duel.CurrentChain[Duel.CurrentChain.Count - 2];
            if (self_card?.Controller != 0
                || !(self_card.Location == CardLocation.MonsterZone || self_card.Location == CardLocation.SpellZone)
                || !isAltergeist(self_card)) return false;
            ClientCard enemy_card = Duel.CurrentChain[Duel.CurrentChain.Count - 1];
            if (enemy_card?.Controller != 1
                || !enemy_card.IsCode(normal_counter)) return false;
            return true;
        }

        public bool is_should_not_negate()
        {
            ClientCard last_card = Util.GetLastChainCard();
            if (last_card != null
                && last_card.Controller == 1 && last_card.IsCode(should_not_negate))
                return true;
            return false;
        }
        
        public bool Multifaker_can_ss()
        {
            foreach (ClientCard sp in Bot.GetSpells())
            {
                if (sp.IsTrap() && sp.IsFacedown() && trap_can_activate(sp.Id))
                {
                    return true;
                }
            }
            foreach (ClientCard h in Bot.Hand)
            {
                if (h.IsTrap() && trap_can_activate(h.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Multifaker_candeckss()
        {
            return (!Multifaker_ssfromdeck && !ss_other_monster);
        }

        public bool Protocol_activing()
        {
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(CardId.Protocol) && card.IsFaceup() && !card.IsDisabled() && !Duel.CurrentChain.Contains(card)) return true;
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
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(list.Count);
                int nextIndex = (index + Program.Rand.Next(list.Count - 1)) % list.Count;
                int tempInt = list[index];
                list[index] = list[nextIndex];
                list[nextIndex] = tempInt;
            }
            if (avoid_Impermanence && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled()))
            {
                foreach (int seq in list)
                {
                    ClientCard enemySpell = Enemy.SpellZone[4 - seq];
                    if (enemySpell != null && enemySpell.IsFacedown()) continue;
                    return (int)System.Math.Pow(2, seq);
                }
            }
            foreach (int seq in list)
            {
                return (int)System.Math.Pow(2, seq);
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
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
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

        public bool get_linked_by_Hexstia(int place)
        {
            if (place == 0) return false;
            if (place == 2 || place == 4)
            {
                int last_place = place - 1;
                return (Bot.MonsterZone[last_place] != null && Bot.MonsterZone[last_place].IsCode(CardId.Hexstia));
            }
            if (place == 1 || place == 3)
            {
                int last_place_1, last_place_2;
                if (place == 1)
                {
                    last_place_1 = 0;
                    last_place_2 = 5;
                } else
                {
                    last_place_1 = 2;
                    last_place_2 = 6;
                }
                if (Bot.MonsterZone[last_place_1] != null && Bot.MonsterZone[last_place_1].IsCode(CardId.Hexstia)) return true;
                if (Bot.MonsterZone[last_place_2] != null && Bot.MonsterZone[last_place_2].IsCode(CardId.Hexstia)) return true;
                return false;
            }
            return false;
        }

        public ClientCard GetFloodgate_Alter(bool canBeTarget = false, bool is_bounce = true)
        {
            foreach (ClientCard card in Enemy.GetSpells())
            {
                if (card != null && card.IsFloodgate() && card.IsFaceup() &&
                    !card.IsCode(CardId.Anti_Spell, CardId.ImperialOrder)
                    && (!is_bounce || card.IsTrap())
                    && (!canBeTarget || !card.IsShouldNotBeTarget()))
                    return card;
            }
            return null;
        }

        public ClientCard GetProblematicEnemyCard_Alter(bool canBeTarget = false, bool is_bounce = true)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = GetFloodgate_Alter(canBeTarget, is_bounce);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;
            List<ClientCard> enemy_monsters = Enemy.GetMonsters();
            enemy_monsters.Sort(CardContainer.CompareCardAttack);
            enemy_monsters.Reverse();
            foreach(ClientCard target in enemy_monsters)
            {
                if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || (target.HasType(CardType.Link) && target.LinkCount >= 2) )
                {
                    if (target.IsCode(CardId.Kagari, CardId.Shizuku)) continue;
                    if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                }
            }

            return null;
        }

        public ClientCard GetBestEnemyCard_random()
        {
            // monsters
            ClientCard card = Util.GetProblematicEnemyMonster(0, true);
            if (card != null)
                return card;
            if (Util.GetOneEnemyBetterThanMyBest() != null)
            {
                card = Enemy.MonsterZone.GetHighestAttackMonster(true);
                if (card != null)
                    return card;
            }

            // spells
            List<ClientCard> enemy_spells = Enemy.GetSpells();
            RandomSort(enemy_spells);
            foreach(ClientCard sp in enemy_spells)
            {
                if (sp.IsFaceup() && !sp.IsDisabled()) return sp;
            }
            if (enemy_spells.Count > 0) return enemy_spells[0];

            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count > 0)
            {
                RandomSort(monsters);
                return monsters[0];
            }

            return null;
        }

        public bool bot_can_s_Meluseek()
        {
            if (Duel.Player != 0) return false;
            foreach(ClientCard card in Bot.GetMonsters())
            {
                if (card.IsCode(CardId.Meluseek) && !card.IsDisabled() && !card.Attacked) return true;
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
            if (Card.IsCode(CardId.EvenlyMatched) && !Bot.HasInHandOrInSpellZone(CardId.Spoofing)
                && !Bot.HasInHandOrInSpellZone(CardId.Protocol) && !Bot.HasInHandOrInSpellZone(CardId.ImperialOrder)) return false;
            if (Card.IsCode(CardId.EvenlyMatched) && Bot.HasInSpellZone(CardId.EvenlyMatched)) return false;
            if (Card.IsCode(CardId.SolemnStrike) && Bot.LifePoints <= 1500) return false;
            if (Card.IsCode(CardId.Spoofing) && Bot.HasInSpellZone(CardId.Spoofing)) return false;
            if (Card.IsCode(CardId.Manifestation) && Bot.HasInHandOrInSpellZone(CardId.Spoofing))
            {
                bool can_activate = false;
                foreach(ClientCard g in Bot.GetGraveyardMonsters())
                {
                    if (g.IsMonster() && isAltergeist(g))
                    {
                        can_activate = true;
                        break;
                    }
                }
                Logger.DebugWriteLine("Manifestation_set: " + can_activate.ToString());
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
                    if (Card.IsCode(CardId.Impermanence))
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
                if (Card.IsSpell() && (!Card.IsCode(CardId.OneForOne) || Bot.GetRemainingCount(CardId.Meluseek,3) > 0))
                {
                    AI.SelectPlace(SelectSTPlace());
                    return true;
                }
            }
            return false;
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

        public bool ChickenGame()
        {
            Logger.DebugWriteLine("Use override");
            if (!spell_trap_activate()) return false;
            if (Bot.LifePoints <= 1000)
                return false;
            if (Bot.LifePoints - 1000 <= Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 0))
            {
                return true;
            }
            if (Bot.LifePoints - 1000 > Enemy.LifePoints && ActivateDescription == Util.GetStringId(_CardId.ChickenGame, 1))
            {
                return true;
            }
            return false;
        }

        public bool Anti_Spell_activate()
        {
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (card.IsCode(CardId.Anti_Spell) && card.IsFaceup() && Duel.LastChainPlayer == 0) return false;
            }
            return true;
        }

        public bool SecretVillage_activate()
        {
            if (!spell_trap_activate()) return false;
            if (Bot.SpellZone[5] != null && Bot.SpellZone[5].IsFaceup() && Bot.SpellZone[5].IsCode(CardId.SecretVillage) && Bot.SpellZone[5].Disabled==0) return false;

            if (Multifaker_can_ss() && Bot.HasInHand(CardId.Multifaker)) return true;
            foreach(ClientCard card in Bot.GetMonsters())
            {
                if (card != null && card.IsFaceup() && (card.Race & (int)CardRace.SpellCaster) != 0 && !card.IsCode(CardId.Meluseek)) return true;
            }
            return false;   
        }

        public bool G_activate()
        {
            return (Duel.Player == 1) && !DefaultCheckWhetherCardIsNegated(Card);
        }

        public bool NaturalExterio_eff()
        {
            if (Duel.LastChainPlayer != 0)
            {
                AI.SelectCard(
                    _CardId.HarpiesFeatherDuster,
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
                    );
                return true;
            }
            return false;
        }

        public bool SolemnStrike_activate()
        {
            if (!Should_counter()) return false;
            return (DefaultSolemnStrike() && spell_trap_activate(true));
        }

        public bool SolemnJudgment_activate()
        {
            if (Util.IsChainTargetOnly(Card) && (Bot.HasInHand(CardId.Multifaker) || Multifaker_candeckss())) return false;
            if (!Should_counter()) return false;
            if ((DefaultSolemnJudgment() && spell_trap_activate(true)))
            {
                ClientCard target = Util.GetLastChainCard();
                if (target != null && !target.IsMonster() && !spell_trap_activate(false, target)) return false;
                return true;
            }
            return false;
        }

        public bool Impermanence_activate()
        {
            if (!Should_counter()) return false;
            if (!spell_trap_activate()) return false;
            // negate before effect used
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

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (LastChainCard == null
                && !(Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && Multifaker_candeckss() && !Multifaker_ssfromhand))
                return false;
            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ( (this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster))
                    || (Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && Multifaker_candeckss() && !Multifaker_ssfromhand))
                {
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(CardContainer.CompareCardAttack);
                    enemy_monsters.Reverse();
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
            if ( (LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget())
                && !(Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && Multifaker_candeckss() && !Multifaker_ssfromhand) )
                return false;
            // negate monsters
            if (is_should_not_negate() && LastChainCard.Location == CardLocation.MonsterZone) return false;
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
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                enemy_monsters.Sort(CardContainer.CompareCardAttack);
                enemy_monsters.Reverse();
                foreach (ClientCard card in enemy_monsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }

        public bool Hand_act_eff()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.IsCode(CardId.AB_JS) && Util.GetLastChainCard().HasSetcode(0x11e) && Util.GetLastChainCard().Location == CardLocation.Hand) // Danger! archtype hand effect
                return false;
            if (Card.IsCode(CardId.GO_SR) && Card.Location == CardLocation.Hand && Bot.HasInMonstersZone(CardId.GO_SR)) return false;
            return (Duel.LastChainPlayer == 1);
        }

        public bool WakingtheDragon_eff()
        {
            if (Bot.HasInExtra(CardId.NaturalExterio) && !Multifaker_ssfromdeck)
            {
                bool has_skystriker = false;
                foreach(ClientCard card in Enemy.Graveyard)
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
                    AI.SelectCard(CardId.NaturalExterio);
                    ss_other_monster = true;
                    return true;
                }
            }
            IList<int> ex_list = new[] {
                CardId.UltimateFalcon,
                CardId.Borrelsword,
                CardId.NaturalExterio,
                CardId.FWD,
                CardId.TripleBurstDragon,
                CardId.HeavymetalfoesElectrumite,
                CardId.Isolde,
                CardId.Hexstia,
                CardId.Needlefiber,
                CardId.Multifaker,
                CardId.Kunquery
            };
            foreach(int id in ex_list)
            {
                if (Bot.HasInExtra(id))
                {
                    if (!isAltergeist(id))
                    {
                        if (Multifaker_ssfromdeck) continue;
                        ss_other_monster = true;
                    }
                    Logger.DebugWriteLine(id.ToString());
                    AI.SelectCard(id);
                    return true;
                }
            }
            return true;
        }

        public bool GR_WC_activate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            int warrior_count = 0;
            int pendulum_count = 0;
            int link_count = 0;
            int altergeis_count = 0;
            bool has_skystriker_acer = false;
            bool has_tuner = false;
            bool has_level_1 = false;
            foreach (ClientCard card in Enemy.MonsterZone)
            {
                if (card == null) continue;
                if (card.IsCode(CardId.Kagari, CardId.Shizuku, CardId.Hayate, CardId.Raye, CardId.Drones_Token)) has_skystriker_acer = true;
                if (card.HasType(CardType.Pendulum)) pendulum_count ++;
                if ((card.Race & (int)CardRace.Warrior) != 0) warrior_count ++;
                if (card.IsTuner() && (Enemy.GetMonsterCount() >= 2)) has_tuner = true;
                if (isAltergeist(card)) altergeis_count++;
                if (!card.HasType(CardType.Link) && !card.HasType(CardType.Xyz) && card.Level == 1) has_level_1 = true;
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
            if (pendulum_count >= 2 && !(Enemy.HasInMonstersZoneOrInGraveyard(CardId.HeavymetalfoesElectrumite) || Enemy.HasInBanished(CardId.HeavymetalfoesElectrumite)) && Bot.HasInExtra(CardId.HeavymetalfoesElectrumite))
            {
                AI.SelectCard(CardId.HeavymetalfoesElectrumite);
                return true;
            }
            if (warrior_count >= 2 && !(Enemy.HasInMonstersZoneOrInGraveyard(CardId.Isolde) || Enemy.HasInBanished(CardId.Isolde)) && Bot.HasInExtra(CardId.Isolde))
            {
                AI.SelectCard(CardId.Isolde);
                return true;
            }
            if (has_tuner && !Enemy.HasInBanished(CardId.Needlefiber) && Bot.HasInExtra(CardId.Needlefiber) && !Enemy.HasInMonstersZone(CardId.Needlefiber))
            {
                AI.SelectCard(CardId.Needlefiber);
                return true;
            }
            if (has_level_1 && !Enemy.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Linkuriboh) && !Enemy.HasInBanished(CardId.Linkuriboh) && Bot.HasInExtra(CardId.Linkuriboh))
            {
                AI.SelectCard(CardId.Linkuriboh);
                return true;
            }
            if (altergeis_count > 0 && !Enemy.HasInBanished(CardId.Hexstia) && Bot.HasInExtra(CardId.Hexstia))
            {
                AI.SelectCard(CardId.Hexstia);
                return true;
            }
            if (link_count >= 4)
            {
                if ((Bot.HasInMonstersZone(CardId.UltimateFalcon) || Bot.HasInMonstersZone(CardId.NaturalExterio)) && !(Enemy.HasInMonstersZoneOrInGraveyard(CardId.Borrelsword) || Enemy.HasInBanished(CardId.Borrelsword)) && Bot.HasInExtra(CardId.Borrelsword))
                {
                    AI.SelectCard(CardId.Borrelsword);
                    return true;
                }
                if (!(Enemy.HasInMonstersZoneOrInGraveyard(CardId.FWD) || Enemy.HasInBanished(CardId.FWD)) && Bot.HasInExtra(CardId.FWD))
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
            if (Duel.Player == 1 && Duel.Phase > DuelPhase.Main2 && Bot.HasInHand(CardId.Multifaker) && (!Multifaker_ssfromhand && Multifaker_candeckss())) return true;
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
            if (Util.GetProblematicEnemySpell() != null)
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
                if (card != null && card.IsFacedown())
                {
                    select_list.Add(card);
                }
            }
            foreach (ClientCard card in spells)
            {
                if (card != null && card.IsFaceup() && !select_list.Contains(card))
                {
                    select_list.Add(card);
                }
            }
            if (Duel.Phase == DuelPhase.End 
                || activate_immediately >= 2 
                || (Util.IsChainTarget(Card) 
                    || (Util.GetLastChainCard() != null && Util.GetLastChainCard().Controller == 1 && Util.GetLastChainCard().IsCode(_CardId.HarpiesFeatherDuster))))
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
                    if (Util.ChainContainsCard(CardId.Linkuriboh)) return false;
                    if (Bot.BattlingMonster == null || (Enemy.BattlingMonster.Attack >= Bot.BattlingMonster.GetDefensePower()) || Enemy.BattlingMonster.IsMonsterDangerous())
                    {
                        AI.SelectPosition(CardPosition.FaceUpDefence);
                        return true;
                    }
                }
                return false;
            } else
            {
                ClientCard target = GetProblematicEnemyCard_Alter(true, false);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                List<ClientCard> spells = Enemy.GetSpells();
                RandomSort(spells);
                foreach(ClientCard card in spells)
                {
                    if (card.IsFaceup() && !card.IsDisabled())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
                List<ClientCard> monsters = Enemy.GetMonsters();
                RandomSort(monsters);
                foreach (ClientCard card in monsters)
                {
                    if (card.IsFaceup() && !card.IsDisabled() 
                        && !(card.IsShouldNotBeMonsterTarget() || card.IsShouldNotBeTarget()))
                    {
                        AI.SelectCard(card);
                        return true;
                    }
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
                    AI.SelectCard(CardId.Protocol, CardId.Manifestation);
                    AI.SelectPlace(SelectSetPlace());
                    return true;
                } else
                {
                    AI.SelectCard(CardId.Manifestation, CardId.Protocol);
                    AI.SelectPlace(SelectSetPlace());
                    return true;
                }
            }
            else
            {
                if (Card.IsDisabled() && !Protocol_activing()) return false;
                int next_card = 0;
                bool choose_other = false;
                bool can_choose_other = false;
                foreach(ClientCard card in Bot.GetSpells())
                {
                    if (card.IsFaceup() && isAltergeist(card))
                    {
                        can_choose_other = true;
                        break;
                    }
                }
                if (!can_choose_other){
                    foreach(ClientCard card in Bot.GetMonsters())
                    {
                        if (card.IsFaceup() && card != Card && isAltergeist(card))
                        {
                            can_choose_other = true;
                        }
                    }
                }
                if (!Util.IsTurn1OrMain2())
                {
                    ClientCard self_best = Util.GetBestBotMonster();
                    ClientCard enemy_best = Util.GetProblematicEnemyCard(self_best.Attack, true);
                    ClientCard enemy_target = GetProblematicEnemyCard_Alter(true,false);

                    if ((enemy_best != null || enemy_target != null)
                        && Bot.HasInGraveyard(CardId.Meluseek)) next_card = CardId.Meluseek;
                    else if (Enemy.GetMonsterCount() <= 1 && Bot.HasInGraveyard(CardId.Meluseek) && Enemy.GetFieldCount() > 0) next_card = CardId.Meluseek;
                    else if (Bot.HasInGraveyard(CardId.Hexstia) && Util.GetProblematicEnemySpell() == null && Util.GetOneEnemyBetterThanValue(3100, true) == null && can_choose_other)
                    {
                        next_card = CardId.Hexstia;
                        choose_other = (Util.GetOneEnemyBetterThanMyBest(true) != null);
                    }
                }
                else
                {
                    if (!Meluseek_searched && !Bot.HasInMonstersZone(CardId.Meluseek) && Bot.HasInGraveyard(CardId.Meluseek))
                    {
                        if (Multifaker_candeckss() && Bot.HasInGraveyard(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Meluseek,3) > 0)
                        {
                            next_card = CardId.Multifaker;
                        } else
                        {
                            next_card = CardId.Meluseek;
                        }
                    }
                    else if (Multifaker_candeckss() && Bot.HasInGraveyard(CardId.Multifaker) && has_altergeist_left())
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
                            if (isAltergeist(card) && (card.IsTrap() || (!summoned && card.IsMonster()))) alter_count ++;
                        }
                        foreach (ClientCard s in Bot.GetSpells())
                        {
                            if (isAltergeist(s)) alter_count++;
                        }
                        foreach(ClientCard m in Bot.GetMonsters())
                        {
                            if (isAltergeist(m) && m != Card) alter_count++;
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
                        if (h.IsCode(CardId.Protocol)) Protocol_count++;
                    }
                    foreach (ClientCard s in Bot.GetSpells())
                    {
                        if (s.IsCode(CardId.Protocol)) Protocol_count += (s.IsFaceup() ? 11 : 1);
                    }
                    if (Protocol_count >= 12)
                    {
                        AI.SelectCard(CardId.Protocol);
                        AI.SelectNextCard(next_card);
                        Marionetter_reborn = true;
                        if (next_card == CardId.Meluseek && Util.IsTurn1OrMain2()) AI.SelectPosition(CardPosition.FaceUpDefence);
                        return true;
                    }
                    List<ClientCard> list = Bot.GetMonsters();
                    list.Sort(CardContainer.CompareCardAttack);
                    foreach (ClientCard card in list)
                    {
                        if (isAltergeist(card) && !(choose_other && card == Card))
                        {
                            AI.SelectCard(card);
                            AI.SelectNextCard(next_card);
                            if (next_card == CardId.Meluseek && Util.IsTurn1OrMain2()) AI.SelectPosition(CardPosition.FaceUpDefence);
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
                ClientCard target =  Util.GetLastChainCard();
                if (target != null && !spell_trap_activate(false, target)) return false;
                if (!Should_counter()) return false;
                // check
                int this_seq = GetSequence(Card);
                if (this_seq != -1) this_seq = get_Hexstia_linkzone(this_seq);
                if (this_seq != -1)
                {
                    ClientCard linked_card = Bot.MonsterZone[this_seq];
                    if (linked_card != null && linked_card.IsCode(CardId.Hexstia))
                    {
                        int next_seq = get_Hexstia_linkzone(this_seq);
                        if (next_seq != -1 && Bot.MonsterZone[next_seq] != null && isAltergeist(Bot.MonsterZone[next_seq])) return false;
                    }
                }
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.Hexstia,0)) return false;
            if (Enemy.HasInSpellZone(82732705) && Bot.GetRemainingCount(CardId.Protocol,3) > 0 && !Bot.HasInHandOrInSpellZone(CardId.Protocol))
            {
                AI.SelectCard(CardId.Protocol);
                return true;
            }
            if (Duel.Player == 0 && !summoned && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
            {
                AI.SelectCard(CardId.Marionetter);
                return true;
            }
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 2) > 0 && Multifaker_can_ss())
            {
                AI.SelectCard(CardId.Multifaker);
                return true;
            }
            if (!Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter,3) > 0)
            {
                AI.SelectCard(CardId.Marionetter);
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
            AI.SelectCard(
                CardId.Meluseek,
                CardId.Kunquery,
                CardId.Marionetter,
                CardId.Multifaker,
                CardId.Manifestation,
                CardId.Protocol,
                CardId.Silquitous
                );
            return true;
        }

        public bool Meluseek_eff()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Meluseek,0)
                || (ActivateDescription == -1 && Card.Location == CardLocation.MonsterZone))
            {
                attacked_Meluseek.Add(Card);
                ClientCard target = GetProblematicEnemyCard_Alter(true);
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
                target = Util.GetOneEnemyBetterThanMyBest(true, true);
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
                target = GetBestEnemyCard_random();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            } else
            {
                if (Duel.Player == 1)
                {
                    if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 2) > 0 && Multifaker_candeckss() && Multifaker_can_ss())
                    {
                        foreach(ClientCard set_card in Bot.GetSpells())
                        {
                            if (set_card.IsFacedown() && !set_card.IsCode(CardId.WakingtheDragon))
                            {
                                AI.SelectCard(CardId.Multifaker);
                                return true;
                            }
                        }
                    }
                    if (Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
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
                    if (!Bot.HasInHandOrHasInMonstersZone(CardId.Multifaker) && Bot.GetRemainingCount(CardId.Multifaker, 2) > 0 && Multifaker_can_ss())
                    {
                        AI.SelectCard(CardId.Multifaker);
                        return true;
                    }
                    if (!Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter,3) > 0)
                    {
                        AI.SelectCard(CardId.Marionetter);
                        return true;
                    }
                }
                AI.SelectCard(
                    CardId.Kunquery,
                    CardId.Marionetter,
                    CardId.Multifaker,
                    CardId.Silquitous
                    );
                return true;
            }
            return false;
        }

        public bool Multifaker_handss()
        {
            if (!Multifaker_candeckss() || Card.Location != CardLocation.Hand) return false;
            Multifaker_ssfromhand = true;
            if (Duel.Player != 0 && Util.GetOneEnemyBetterThanMyBest() != null) AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public bool Multifaker_deckss()
        {
            if (Card.Location != CardLocation.Hand)
            {
                ClientCard Silquitous_target = GetProblematicEnemyCard_Alter(true);
                if (Duel.Player == 1 && Duel.Phase >= DuelPhase.Main2 && GetProblematicEnemyCard_Alter(true) == null && Bot.GetRemainingCount(CardId.Meluseek,3) > 0)
                {
                    AI.SelectCard(CardId.Meluseek);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (!Silquitous_bounced && !Bot.HasInMonstersZone(CardId.Silquitous) && Bot.GetRemainingCount(CardId.Silquitous,2) > 0
                    && !(Duel.Player == 0 && Silquitous_target==null))
                {
                    AI.SelectCard(CardId.Silquitous);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (!Meluseek_searched && !Bot.HasInMonstersZone(CardId.Meluseek) && Bot.GetRemainingCount(CardId.Meluseek, 3) > 0)
                {
                    AI.SelectCard(CardId.Meluseek);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
                else if (Bot.GetRemainingCount(CardId.Kunquery,1) > 0)
                {
                    AI.SelectCard(CardId.Kunquery);
                    Multifaker_ssfromdeck = true;
                    return true;
                } else
                {
                    AI.SelectCard(CardId.Marionetter);
                    Multifaker_ssfromdeck = true;
                    return true;
                }
            }
            return false;
        }

        public bool Silquitous_eff()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Silquitous,0))
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
                    if (spell.IsCode(CardId.Protocol))
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
                    if (spell.IsCode(CardId.Manifestation) && spell.IsFaceup()) faceup_Manifestation = spell;
                    if (Duel.LastChainPlayer != 0 && Util.IsChainTarget(spell) && spell.IsFaceup() && isAltergeist(spell))
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
                monster_list.Sort(CardContainer.CompareCardAttack);
                foreach(ClientCard card in monster_list)
                {
                    if (card.IsFaceup() && isAltergeist(card) && card != Card)
                    {
                        if (Duel.LastChainPlayer != 0 && Util.IsChainTarget(card) && card.IsFaceup())
                        {
                            selected_target = card;
                        }
                        if (faceup_Multifaker == null && card.IsCode(CardId.Multifaker)) faceup_Multifaker = card;
                        if (faceup_monster == null && !card.IsCode(CardId.Hexstia)) faceup_monster = card;
                    }
                }
                if (bounce_self == null)
                {
                    if (selected_target != null && selected_target != Card) bounce_self = selected_target;
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
                        if (Util.ChainContainsCard(CardId.Linkuriboh) || Bot.HasInHand(CardId.Kunquery)) return false;
                        if (Enemy.BattlingMonster != null && Bot.BattlingMonster != null && Enemy.BattlingMonster.GetDefensePower() >= Bot.BattlingMonster.GetDefensePower())
                        {
                            if (Bot.HasInMonstersZone(CardId.Kunquery)) AI.SelectCard(CardId.Kunquery);
                            else AI.SelectCard(bounce_self);
                            List<ClientCard> enemy_list = Enemy.GetMonsters();
                            enemy_list.Sort(CardContainer.CompareCardAttack);
                            enemy_list.Reverse();
                            foreach(ClientCard target in enemy_list)
                            {
                                if (target.IsAttack() && !target.IsShouldNotBeMonsterTarget() && !target.IsShouldNotBeTarget())
                                {
                                    AI.SelectNextCard(target);
                                    return true;
                                }
                            }
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
                        Logger.DebugWriteLine("Silquitous decide:" + bounce_self?.Name);
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
                if (Util.ChainContainsCard(CardId.Silquitous)) return false;
                if (!Bot.HasInHandOrInSpellZone(CardId.Protocol) && !Util.ChainContainsCard(CardId.Protocol))
                {
                    AI.SelectCard(CardId.Protocol);
                    return true;
                }
                return false;
            }
            else
            {
                if (Util.ChainContainsCard(CardId.Manifestation) || Util.ChainContainsCard(CardId.Spoofing)) return false;
                if (Duel.LastChainPlayer == 0 && !(Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.Hexstia))) return false;

                if (Bot.HasInMonstersZone(CardId.Hexstia))
                {
                    bool has_position = false;
                    for (int i = 0; i < 7; ++i)
                    {
                        ClientCard target = Bot.MonsterZone[i];
                        if (target != null && target.IsCode(CardId.Hexstia))
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

                if (Enemy.HasInMonstersZone(94977269) && Bot.HasInGraveyard(CardId.Silquitous))
                {
                    AI.SelectCard(CardId.Silquitous);
                    return true;
                }

                if (Multifaker_candeckss() && Bot.HasInGraveyard(CardId.Multifaker) && has_altergeist_left())
                {
                    if (Bot.HasInHand(CardId.Multifaker) && Bot.HasInGraveyard(CardId.Silquitous) && Bot.GetRemainingCount(CardId.Silquitous,2) == 0)
                    {
                        AI.SelectCard(CardId.Silquitous);
                        return true;
                    } else
                    {
                        AI.SelectCard(CardId.Multifaker);
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
                        if (id == CardId.Kunquery
                            && (!Bot.HasInHand(CardId.Multifaker) || !Multifaker_candeckss())) continue;
                        AI.SelectCard(id);
                        return true;
                    }
                }

            }
            return false;
        }

        public bool Protocol_negate_better()
        {
            // skip if no one of enemy's monsters is better
            if (ActivateDescription == Util.GetStringId(CardId.Protocol, 1))
            {
                if (Util.GetOneEnemyBetterThanMyBest(true) == null) return false;
            }
            return Protocol_negate();
        }

        public bool Protocol_negate()
        {
            // negate
            if (ActivateDescription == Util.GetStringId(CardId.Protocol, 1) && (!Card.IsDisabled() || Protocol_activing()))
            {
                if (!Should_counter()) return false;
                if (is_should_not_negate()) return false;
                if (Should_activate_Protocol()) return false;
                foreach (ClientCard card in Bot.GetSpells())
                {
                    if (card.IsCode(CardId.Protocol) && card.IsFaceup() && card != Card
                        && (Card.IsFacedown() || !Card.IsDisabled()))
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
                if (Bot.HasInMonstersZone(CardId.Hexstia))
                {
                    for (int i = 0; i < 7; ++i)
                    {
                        ClientCard target = Bot.MonsterZone[i];
                        if (target != null && isAltergeist(target) && target.IsFaceup())
                        {
                            if (target.IsCode(CardId.Hexstia))
                            {
                                int next_index = get_Hexstia_linkzone(i);
                                if (next_index != -1 && Bot.MonsterZone[next_index] != null && Bot.MonsterZone[next_index].IsFaceup() && isAltergeist(Bot.MonsterZone[next_index])) continue;
                            }
                            if (!get_linked_by_Hexstia(i))
                            {
                                Logger.DebugWriteLine("negate_index: " + i.ToString());
                                AI.SelectCard(target);
                                return true;
                            }
                        }
                    }
                }
                List<int> cost_list = new List<int>();
                if (Util.ChainContainsCard(CardId.Manifestation)) cost_list.Add(CardId.Manifestation);
                if (!Card.IsDisabled()) cost_list.Add(CardId.Protocol);
                cost_list.Add(CardId.Multifaker);
                cost_list.Add(CardId.Marionetter);
                cost_list.Add(CardId.Kunquery);
                if (Meluseek_searched) cost_list.Add(CardId.Meluseek);
                if (Silquitous_bounced) cost_list.Add(CardId.Silquitous);
                for (int i = 0; i < 7; ++i)
                {
                    ClientCard card = Bot.MonsterZone[i];
                    if (card != null && card.IsCode(CardId.Hexstia))
                    {
                        int nextzone = get_Hexstia_linkzone(i);
                        if (nextzone != -1)
                        {
                            ClientCard linkedcard = Bot.MonsterZone[nextzone];
                            if (linkedcard == null || !isAltergeist(linkedcard))
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
                if (!Util.ChainContainsCard(CardId.Manifestation)) cost_list.Add(CardId.Manifestation);
                AI.SelectCard(cost_list);
                return true;
            }
            return false;
        }

        public bool Protocol_activate_not_use()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Controller == 0 && Util.GetLastChainCard().IsTrap()) return false;
            if (ActivateDescription != Util.GetStringId(CardId.Protocol, 1))
            {
                if (Util.IsChainTarget(Card) && Card.IsFacedown()) return true;
                if (Should_activate_Protocol()) return true;
                if (!Multifaker_ssfromhand && Multifaker_candeckss() && (Bot.HasInHand(CardId.Multifaker) || Bot.HasInSpellZone(CardId.Spoofing)))
                {
                    if (!Bot.HasInMonstersZone(CardId.Hexstia)) return true;
                    for (int i = 0; i < 7; ++i)
                    {
                        if (i == 4) continue;
                        if (Bot.MonsterZone[i] != null && Bot.MonsterZone[i].IsCode(CardId.Hexstia))
                        {
                            int next_id = get_Hexstia_linkzone(i);
                            if (next_id != -1)
                            {
                                if (Bot.MonsterZone[next_id] == null) return true;
                            }
                        }
                    }
                }
                int can_bounce = 0;
                bool should_disnegate = false;
                foreach(ClientCard card in Bot.GetMonsters())
                {
                    if (isAltergeist(card))
                    {
                        if (card.IsCode(CardId.Silquitous) && card.IsFaceup() && !Silquitous_bounced) can_bounce += 10;
                        else if (card.IsFaceup() && !card.IsCode(CardId.Hexstia)) can_bounce++;
                        if (card.IsDisabled() && !Protocol_activing()) should_disnegate = true;
                    }
                }
                if (can_bounce == 10 || should_disnegate) return true;
                if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2 && Bot.HasInHand(CardId.Kunquery) && Util.GetOneEnemyBetterThanMyBest() != null) return true;
            }
            return false;
        }

        public void Spoofing_select(IList<int> list)
        {
            foreach(ClientCard card in Duel.CurrentChain)
            {
                if (card != null
                    && card.Location == CardLocation.SpellZone && card.Controller == 0 && card.IsFaceup())
                {
                    if (card.IsCode(CardId.Manifestation))
                    {
                        AI.SelectCard(card);
                        return;
                    }
                }
            }
            foreach (ClientCard card in Bot.Hand)
            {
                foreach (int id in list)
                {
                    if (card.IsCode(id) && !(id == CardId.Multifaker && Util.GetLastChainCard() == card))
                    {
                        AI.SelectCard(card);
                        return;
                    }
                }
            }
            foreach(ClientCard card in Bot.GetSpells())
            {
                foreach (int id in list)
                {
                    if (card.IsFaceup() && card.IsCode(id))
                    {
                        AI.SelectCard(card);
                        return;
                    }
                }
            }
            foreach (ClientCard card in Bot.GetMonsters())
            {
                foreach (int id in list)
                {
                    if (card.IsFaceup() && card.IsCode(id))
                    {
                        AI.SelectCard(card);
                        return;
                    }
                }
            }
            AI.SelectCard((ClientCard)null);
        }

        public bool Spoofing_eff()
        {
            if (Util.ChainContainsCard(CardId.Spoofing)) return false;
            if (Card.IsDisabled()) return false;
            if (!Util.ChainContainPlayer(0) && !Multifaker_ssfromhand && Multifaker_candeckss() && Bot.HasInHand(CardId.Multifaker) && Card.HasPosition(CardPosition.FaceDown))
            {
                AI.SelectYesNo(false);
                return true;
            }
            bool has_cost = false;
            bool can_ss_Multifaker = Multifaker_can_ss() || Card.IsFacedown();
            // cost check(not select)
            if (Card.IsFacedown())
            {
                foreach(ClientCard card in Bot.Hand)
                {
                    if (isAltergeist(card))
                    {
                        has_cost = true;
                        break;
                    }
                }
                if (!has_cost)
                {
                    foreach(ClientCard card in Bot.GetSpells())
                    {
                        if (isAltergeist(card) && card.IsFaceup())
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
                        if (isAltergeist(card) && card.IsFaceup())
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
                        if (isAltergeist(card) && card.IsFaceup())
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
                if (!Multifaker_ssfromhand && Multifaker_candeckss() && !Bot.HasInHand(CardId.Multifaker) && can_ss_Multifaker)
                {
                    if (Bot.HasInHand(CardId.Silquitous))
                    {
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (card.IsCode(CardId.Silquitous))
                            {
                                AI.SelectCard(card);
                                AI.SelectNextCard(CardId.Multifaker, CardId.Kunquery);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Spoofing_select(new[]
                        {
                            CardId.Silquitous,
                            CardId.Manifestation,
                            CardId.Kunquery,
                            CardId.Marionetter,
                            CardId.Multifaker,
                            CardId.Protocol,
                            CardId.Meluseek
                        });
                        AI.SelectNextCard(
                            CardId.Multifaker,
                            CardId.Marionetter,
                            CardId.Meluseek,
                            CardId.Kunquery,
                            CardId.Silquitous
                            );
                        return true;
                    }
                }
            }
            else
            {
                ClientCard self_best = Util.GetBestBotMonster();
                int best_atk = self_best == null ? 0 : self_best.Attack;
                ClientCard enemy_best = Util.GetProblematicEnemyCard(best_atk, true);
                ClientCard enemy_target = GetProblematicEnemyCard_Alter(true, false);

                if (!Multifaker_ssfromhand && Multifaker_candeckss() && can_ss_Multifaker)
                {
                    Spoofing_select(new[]{
                        CardId.Silquitous,
                        CardId.Manifestation,
                        CardId.Kunquery,
                        CardId.Marionetter,
                        CardId.Multifaker,
                        CardId.Protocol,
                        CardId.Meluseek
                    });
                    AI.SelectNextCard(
                        CardId.Multifaker,
                        CardId.Marionetter,
                        CardId.Meluseek,
                        CardId.Kunquery,
                        CardId.Silquitous
                        );
                }
                else if (!summoned && !Bot.HasInGraveyard(CardId.Meluseek) && Bot.GetRemainingCount(CardId.Meluseek,3) > 0 && !Bot.HasInHand(CardId.Meluseek)
                    && (enemy_best != null || enemy_target != null) )
                {
                    if (Bot.HasInHand(CardId.Silquitous))
                    {
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (card.IsCode(CardId.Silquitous))
                            {
                                AI.SelectCard(card);
                                AI.SelectNextCard(
                                    CardId.Meluseek,
                                    CardId.Marionetter
                                    );
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Spoofing_select(new[]
                        {
                            CardId.Silquitous,
                            CardId.Manifestation,
                            CardId.Kunquery,
                            CardId.Multifaker,
                            CardId.Protocol,
                            CardId.Meluseek,
                            CardId.Marionetter,
                        });
                        AI.SelectNextCard(
                            CardId.Meluseek,
                            CardId.Marionetter,
                            CardId.Multifaker,
                            CardId.Kunquery
                            );
                        return true;
                    }
                }
                else if (!summoned && !Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter,3) > 0)
                {
                    if (Bot.HasInHand(CardId.Silquitous))
                    {
                        foreach (ClientCard card in Bot.Hand)
                        {
                            if (card.IsCode(CardId.Silquitous))
                            {
                                AI.SelectCard(card);
                                AI.SelectNextCard(
                                    CardId.Marionetter,
                                    CardId.Meluseek
                                    );
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Spoofing_select(new[]
                        {
                            CardId.Silquitous,
                            CardId.Manifestation,
                            CardId.Kunquery,
                            CardId.Multifaker,
                            CardId.Protocol,
                            CardId.Meluseek,
                            CardId.Marionetter,
                        });
                        AI.SelectNextCard(
                            CardId.Marionetter,
                            CardId.Meluseek,
                            CardId.Multifaker,
                            CardId.Kunquery
                            );
                        return true;
                    }
                }
            }
            // target protect
            bool go = false;
            foreach(ClientCard card in Bot.GetSpells())
            {
                if ( (Util.ChainContainsCard(_CardId.HarpiesFeatherDuster) || Util.IsChainTarget(card)) 
                    && card.IsFaceup() && Duel.LastChainPlayer != 0 && isAltergeist(card))
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
                    if ( (Util.IsChainTarget(card) || Util.ChainContainsCard(CardId.DarkHole) || (!Protocol_activing() && card.IsDisabled()))
                        && card.IsFaceup() && Duel.LastChainPlayer != 0 && isAltergeist(card))
                    {
                        Logger.DebugWriteLine("Spoofing target:" + card?.Name);
                        AI.SelectCard(card);
                        go = true;
                        break;
                    }
                }
            }
            if (go)
            {
                AI.SelectNextCard(
                    CardId.Marionetter,
                    CardId.Meluseek,
                    CardId.Multifaker,
                    CardId.Kunquery
                    );
                return true;
            }
            return false;
        }

        public bool OneForOne_activate()
        {
            if (!spell_trap_activate()) return false;
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Meluseek) && !Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Multifaker))
            {
                AI.SelectCard(
                    CardId.GR_WC,
                    CardId.MaxxC,
                    CardId.Kunquery,
                    CardId.GO_SR
                    );
                if (Util.IsTurn1OrMain2()) AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            if (!summoned && !Meluseek_searched && !Bot.HasInHand(CardId.Marionetter))
            {
                AI.SelectCard(
                    CardId.GR_WC,
                    CardId.MaxxC,
                    CardId.Kunquery,
                    CardId.GO_SR
                    );
                return true;
            }
            return false;
        }

        public bool Meluseek_summon()
        {
            if (EvenlyMatched_ready()) return false;
            if (Bot.HasInHand(CardId.Marionetter) && Bot.HasInGraveyard(CardId.Meluseek) && !Marionetter_reborn) return false;
            summoned = true;
            return true;
        }

        public bool Marionetter_summon()
        {
            if (EvenlyMatched_ready()) return false;
            summoned = true;
            return true;
        }

        public bool Silquitous_summon()
        {
            if (EvenlyMatched_ready()) return false;
            bool can_summon = false;
            if (Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 800) return true;
            foreach (ClientCard card in Bot.Hand)
            {
                if (isAltergeist(card) && card.IsTrap())
                {
                    can_summon = true;
                    break;
                }
            }
            foreach(ClientCard card in Bot.GetMonstersInMainZone())
            {
                if (isAltergeist(card))
                {
                    can_summon = true;
                    break;
                }
            }
            foreach(ClientCard card in Bot.GetSpells())
            {
                if (isAltergeist(card))
                {
                    can_summon = true;
                    break;
                }
            }
            if (can_summon)
            {
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
            if (Enemy.GetMonsterCount() == 0 && Enemy.LifePoints <= 1200) return true;
            if (Bot.HasInMonstersZone(CardId.Silquitous) || Bot.HasInHandOrInSpellZone(CardId.Spoofing))
            {
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

        public bool PotofIndulgence_activate()
        {
            if (!spell_trap_activate()) return false;
            if (!Bot.HasInGraveyard(CardId.Linkuriboh) && !Bot.HasInGraveyard(CardId.Hexstia))
            {
                int important_count = 0;
                foreach (ClientCard card in Bot.ExtraDeck)
                {
                    if (card.Id == CardId.Linkuriboh || card.Id == CardId.Hexstia)
                    {
                        important_count++;
                    }
                }
                if (important_count > 0)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                    AI.SelectOption(1);
                    return true;
                }
                return false;
            }
            AI.SelectPlace(SelectSTPlace(Card, true));
            AI.SelectOption(1);
            return true;
        }

        public bool Anima_ss()
        {
            if (Duel.Phase != DuelPhase.Main2) return false;
            ClientCard card_ex_left = Enemy.MonsterZone[6];
            ClientCard card_ex_right = Enemy.MonsterZone[5];
            if (card_ex_left != null && card_ex_left.HasLinkMarker((int)CardLinkMarker.Top))
            {
                ClientCard self_card_1 = Bot.MonsterZone[1];
                if (self_card_1 == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z1);
                    ss_other_monster = true;
                    return true;
                } else if (self_card_1.IsCode(CardId.Meluseek))
                {
                    AI.SelectMaterials(self_card_1);
                    AI.SelectPlace(Zones.z1);
                    ss_other_monster = true;
                    return true;
                }
            }
            if (card_ex_right != null && card_ex_right.HasLinkMarker((int)CardLinkMarker.Top))
            {
                ClientCard self_card_2 = Bot.MonsterZone[3];
                if (self_card_2 == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z3);
                    ss_other_monster = true;
                    return true;
                }
                else if (self_card_2.IsCode(CardId.Meluseek))
                {
                    AI.SelectMaterials(self_card_2);
                    AI.SelectPlace(Zones.z3);
                    ss_other_monster = true;
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
                    ss_other_monster = true;
                    return true;
                }
            }
            if (card_left != null && card_right == null)
            {
                if (Bot.MonsterZone[5] == null)
                {
                    AI.SelectMaterials(CardId.Meluseek);
                    AI.SelectPlace(Zones.z5);
                    ss_other_monster = true;
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
                ss_other_monster = true;
                return true;
            }
            return false;
        }

        public bool Linkuriboh_ss()
        {
            if (Bot.GetMonstersExtraZoneCount() > 0) return false;
            if (Util.IsTurn1OrMain2() && !Meluseek_searched)
            {
                AI.SelectPlace(Zones.z5);
                ss_other_monster = true;
                return true;
            }
            return false;
        }

        public bool Linkuriboh_eff()
        {
            if (Util.ChainContainsCard(CardId.Linkuriboh)) return false;
            if (Util.ChainContainsCard(CardId.Multifaker)) return false;
            if (Duel.Player == 1)
            {
                if (Card.Location == CardLocation.Grave)
                {
                    AI.SelectCard(new[] { CardId.Meluseek });
                    ss_other_monster = true;
                    return true;
                } else
                {
                    if (Card.IsDisabled() && !Enemy.HasInSpellZone(82732705, true)) return false;
                    ClientCard enemy_card = Enemy.BattlingMonster;
                    if (enemy_card == null) return false;
                    ClientCard self_card = Bot.BattlingMonster;
                    if (self_card == null) return (!enemy_card.IsCode(CardId.Hayate));
                    return (enemy_card.Attack > self_card.GetDefensePower());
                }
            }
            else
            {
                if (!summoned && !Bot.HasInHand(CardId.Marionetter) && !Meluseek_searched && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                {
                    AI.SelectCard(new[] { CardId.Meluseek });
                    ss_other_monster = true;
                    AI.SelectPlace(Zones.z0 | Zones.z4);
                    return true;
                }
                else if (Util.IsTurn1OrMain2())
                {
                    AI.SelectCard(new[] { CardId.Meluseek });
                    ss_other_monster = true;
                    AI.SelectPlace(Zones.z0 | Zones.z4);
                    return true;
                }
                else if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
                {
                    if (Duel.Player != 0 || attacked_Meluseek.Count == 0 || Enemy.GetMonsterCount() > 0) return false;
                    foreach (ClientCard card in attacked_Meluseek)
                    {
                        if (card != null && card.Location == CardLocation.MonsterZone)
                        {

                            ss_other_monster = true;
                            AI.SelectPlace(Zones.z0 | Zones.z4);
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
            list.Sort(CardContainer.CompareCardAttack);
            //list.Reverse();
            bool Meluseek_selected = false;
            bool Silquitous_selected = false;
            bool Hexstia_selected = false;
            int altergeist_count = 0;
            foreach (ClientCard card in list)
            {
                if (isAltergeist(card)) altergeist_count++;
                if (card.IsCode(CardId.Meluseek) && targets.Count < 2 && card.IsFaceup())
                {
                    if ((!Meluseek_searched || !Meluseek_selected) && (!summoned || Duel.Phase == DuelPhase.Main2))
                    {
                        Meluseek_selected = true;
                        targets.Add(card);
                    }
                }
                else if (card.IsCode(CardId.Silquitous) && targets.Count < 2 && card.IsFaceup() && !Bot.HasInGraveyard(CardId.Silquitous))
                {
                    if (!Silquitous_recycled || !Silquitous_selected)
                    {
                        Silquitous_selected = true;
                        targets.Add(card);
                    }
                }
                else if (card.IsCode(CardId.Hexstia) && targets.Count < 2 && card.IsFaceup())
                {
                    if ((!Hexstia_searched || !Hexstia_selected) && !summoned && !Bot.HasInHand(CardId.Marionetter) && Bot.GetRemainingCount(CardId.Marionetter, 3) > 0)
                    {
                        Hexstia_selected = true;
                        targets.Add(card);
                    }
                }
                else if (isAltergeist(card) && targets.Count < 2 && card.IsFaceup()) targets.Add(card);
                else if (card.IsCode(CardId.Silquitous) && targets.Count < 2 && card.IsFaceup())
                {
                    if (!Silquitous_recycled || !Silquitous_selected)
                    {
                        Silquitous_selected = true;
                        targets.Add(card);
                    }
                }
            }
            if (targets.Count >= 2)
            {
                if (Duel.Phase < DuelPhase.Main2)
                {
                    if (GetTotalATK(targets) >= 1500 && (summoned || (!Meluseek_selected && !Hexstia_selected))) return false;
                }
                bool can_have_Multifaker = (Bot.HasInHand(CardId.Multifaker) 
                    || (Bot.GetRemainingCount(CardId.Multifaker, 2) > 0 
                        && ( (Meluseek_selected && !Meluseek_searched) 
                            || (Hexstia_selected && !Hexstia_searched) )));
                if (can_have_Multifaker && Multifaker_can_ss()) altergeist_count++;
                if (Bot.HasInHandOrInSpellZone(CardId.Manifestation)) altergeist_count++;
                Logger.DebugWriteLine("Multifaker_ss_check: count = " + altergeist_count.ToString());
                if (altergeist_count <= 2) return false;
                AI.SelectMaterials(targets);
                return true;
            }
            return false;
        }

        public bool TripleBurstDragon_eff()
        {
            if (ActivateDescription != Util.GetStringId(CardId.TripleBurstDragon,0)) return false;
            return (Duel.LastChainPlayer != 0);
        }

        public bool TripleBurstDragon_ss()
        {
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = Util.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = Util.GetBestEnemyMonster(true);
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
            if (Enemy.HasInMonstersZone(CardId.Shizuku) && Enemy.GetGraveyardSpells().Count >= 9) return false;
            List<ClientCard> list = new List<ClientCard>();
            if (Bot.HasInMonstersZone(CardId.Needlefiber))
            {
                foreach(ClientCard card in Bot.GetMonsters())
                {
                    if (card.IsCode(CardId.Needlefiber))
                    {
                        list.Add(card);
                        link_count += 2;
                    }
                }
            }
            List<ClientCard> monsters = Bot.GetMonsters();
            monsters.Sort(CardContainer.CompareCardAttack);
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
                ss_other_monster = true;
                return true;
            }
            return false;

        }

        public bool Needlefiber_ss()
        {
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = Util.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = Util.GetBestEnemyMonster(true);
                int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
                if (enemy_power < self_power) return false;
                if (Bot.GetMonsterCount() <= 2 && enemy_power >= 2401) return false;
            }
            foreach(ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }
            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> monsters = Bot.GetMonsters();
            monsters.Sort(CardContainer.CompareCardAttack);
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
            ss_other_monster = true;
            return true;
        }

        public bool Needlefiber_eff()
        {
            AI.SelectCard(
                CardId.GR_WC,
                CardId.GO_SR,
                CardId.AB_JS
                );
            return true;
        }

        public bool Borrelsword_ss()
        {
            if (Duel.Phase != DuelPhase.Main1) return false;

            ClientCard self_best = Util.GetBestBotMonster(true);
            int self_power = (self_best != null) ? self_best.Attack : 0;
            ClientCard enemy_best = Util.GetBestEnemyMonster(true);
            int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
            if (enemy_power < self_power) return false;

            foreach (ClientCard card in Bot.GetMonstersInExtraZone())
            {
                if (!card.HasType(CardType.Link)) return false;
            }

            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> bot_monster = Bot.GetMonsters();
            bot_monster.Sort(CardContainer.CompareCardAttack);
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
                ss_other_monster = true;
                return true;
            }
            return false;
        }

        public bool Borrelsword_eff()
        {
            if (ActivateDescription == -1) return true;
            else if ((Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2) || Util.IsChainTarget(Card))
                {
                    List<ClientCard> enemy_list = Enemy.GetMonsters();
                    enemy_list.Sort(CardContainer.CompareCardAttack);
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
                    bot_list.Sort(CardContainer.CompareCardAttack);
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
                if (card != null && !card.HasType(CardType.Link)) return false;
            }
            if (!Enemy.HasInGraveyard(CardId.Raye))
            {
                ClientCard self_best = Util.GetBestBotMonster(true);
                int self_power = (self_best != null) ? self_best.Attack : 0;
                ClientCard enemy_best = Util.GetBestEnemyMonster(true);
                int enemy_power = (enemy_best != null) ? enemy_best.GetDefensePower() : 0;
                Logger.DebugWriteLine("Tuner: enemy: " + enemy_power.ToString() + ", bot: " + self_power.ToString());
                if (enemy_power < self_power || enemy_power == 0) return false;
                int real_count = (Bot.HasInExtra(CardId.Needlefiber)) ? Bot.GetMonsterCount() + 2 : Bot.GetMonsterCount() + 1;
                if ((real_count <= 3 && enemy_power >= 2400)
                    || !(Bot.HasInExtra(CardId.TripleBurstDragon) || Bot.HasInExtra(CardId.Borrelsword)) ) return false;
            }
            if (Multifaker_ssfromdeck) return false;
            foreach(ClientCard card in Bot.GetMonsters())
            {
                if (card.IsFaceup())
                {
                    summoned = true;
                    AI.SelectPlace(Zones.z5);
                    return true;
                }
            }
            return false;
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            List<ClientCard> Meluseek_list = new List<ClientCard>();
            for (int i = 0; i < attackers.Count; ++i)
            {
                ClientCard attacker = attackers[i];
                if (attacker.IsCode(CardId.Meluseek) && !attacker.IsDisabled())
                {
                    if (Enemy.GetMonsterCount() > 0) return attacker;
                    // Meluseek attack first even in direct attack
                    else Meluseek_list.Add(attacker);
                }
                if (attacker.IsCode(CardId.Borrelsword) && !attacker.IsDisabled()) return attacker;
            }
            if (Meluseek_list.Count > 0)
            {
                foreach(ClientCard card in Meluseek_list)
                {
                    attackers.Remove(card);
                    attackers.Add(card);
                }
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
            ss_other_monster = false;
            Impermanence_list.Clear();
            attacked_Meluseek.Clear();
            base.OnNewTurn();
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;

            if (player == 1)
            {
                if (card.IsCode(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == card)
                        {
                            Impermanence_list.Add(4-i);
                            break;
                        }
                    }
                }
            }
            base.OnChaining(player, card);
        }

        public bool MonsterRepos()
        {
            if (Card.Attack == 0) return (Card.IsAttack());

            if (Card.IsCode(CardId.Meluseek) || Bot.HasInMonstersZone(CardId.Meluseek))
            {
                return Card.HasPosition(CardPosition.Defence);
            }
           
            if (isAltergeist(Card) && Bot.HasInHandOrInSpellZone(CardId.Protocol) && Card.IsFacedown())
                return true;

            bool enemyBetter = Util.IsAllEnemyBetter(true);
            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsDefense() && !enemyBetter)
                return true;
            return false;
        }

        public bool MonsterSet()
        {
            if (Util.GetOneEnemyBetterThanMyBest() == null && Bot.GetMonsterCount() > 0) return false;
            if (Card.Level > 4) return false;
            int rest_lp = Bot.LifePoints;
            int count = Bot.GetMonsterCount();
            List<ClientCard> list = Enemy.GetMonsters();
            list.Sort(CardContainer.CompareCardAttack);
            foreach(ClientCard card in list)
            {
                if (!card.HasPosition(CardPosition.Attack)) continue;
                if (count-- > 0) continue;
                rest_lp -= card.Attack;
            }
            if (rest_lp < 1700)
            {
                AI.SelectPosition(CardPosition.FaceDownDefence);
                return true;
            }
            return false;
        }

        public bool MonsterSummon()
        {
            if (Enemy.GetMonsterCount() + Bot.GetMonsterCount() > 0) return false;
            return Card.Attack >= Enemy.LifePoints;
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (EvenlyMatched_ready())
            {
                List<ClientCard> enemy_m = Enemy.GetMonsters();
                enemy_m.Sort(CardContainer.CompareCardAttack);
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
                if (attacker.IsCode(CardId.Borrelsword) && !attacker.IsDisabled())
                    return AI.Attack(attacker, defender);
                if (!OnPreBattleBetween(attacker, defender))
                    continue;
                if (attacker.RealPower == defender.RealPower && Bot.GetMonsterCount() < Enemy.GetMonsterCount())
                    continue;
                if (attacker.RealPower > defender.RealPower || (attacker.RealPower >= defender.RealPower && attacker.IsLastAttacker && defender.IsAttack()))
                    return AI.Attack(attacker, defender);
            }

            if (attacker.CanDirectAttack && (Enemy.GetMonsterCount() == 0 || !attacker.IsDisabled()))
                return AI.Attack(attacker, null);

            return null;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (max == 1 && cards[0].Location == CardLocation.Deck 
                && Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(23002292) && Bot.GetRemainingCount(CardId.WakingtheDragon,1) > 0)
            {
                IList<ClientCard> result = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(CardId.WakingtheDragon))
                    {
                        result.Add(card);
                        AI.SelectPlace(SelectSetPlace());
                        break;
                    }
                }
                if (result.Count > 0) return result;
            }
            else if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.EvenlyMatched) && Duel.LastChainPlayer != 0)
            {
                Logger.DebugWriteLine("EvenlyMatched: min=" + min.ToString() + ", max=" + max.ToString());
            }
            else if (cards[0].Location == CardLocation.Hand && cards[cards.Count - 1].Location == CardLocation.Hand
                && (hint == HintMsg.Discard || hint == HintMsg.ToGrave) && min == max)
            {
                if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.OneForOne)) return null;
                Logger.DebugWriteLine("Hand drop except OneForOne");
                int todrop = min;
                IList<ClientCard> result = new List<ClientCard>();
                IList<ClientCard> ToRemove = new List<ClientCard>(cards);
                // throw redundance
                List<int> record = new List<int>();
                foreach(ClientCard card in ToRemove)
                {
                    if (card?.Id != 0 && !record.Contains(card.Id)) record.Add(card.Id);
                    else
                    {
                        result.Add(card);
                        if (--todrop <= 0) break;
                    }
                }
                if (todrop <= 0) return result;
                foreach (ClientCard card in result) ToRemove.Remove(card);
                // throw improper
                foreach (int throw_id in cards_improper)
                {
                    foreach(ClientCard card in ToRemove)
                    {
                        if (card.IsCode(throw_id))
                        {
                            result.Add(card);
                            if (--todrop <= 0) return result;
                        }
                    }
                }
                // throw all??
                return null;
            }
            
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (Util.IsTurn1OrMain2()
                && (cardId == CardId.Meluseek || cardId == CardId.Silquitous))
            {
                return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0)
            {
                if (location == CardLocation.SpellZone)
                {
                    // unfinished
                }
                else if (location == CardLocation.MonsterZone)
                {
                    if(cardId == CardId.Linkuriboh)
                    {
                        if ((Zones.z5 & available) > 0) return Zones.z5;
                        if ((Zones.z6 & available) > 0) return Zones.z6;
                        for (int i = 4; i >= 0; --i)
                        {
                            if (Bot.MonsterZone[i] == null)
                            {
                                int place = (int)System.Math.Pow(2, i);
                                return place;
                            }
                        }
                    }
                    if (isAltergeist(cardId))
                    {
                        if (Bot.HasInMonstersZone(CardId.Hexstia))
                        {
                            for (int i = 0; i < 7; ++i)
                            {
                                if (i == 4) continue;
                                if (Bot.MonsterZone[i] != null && Bot.MonsterZone[i].IsCode(CardId.Hexstia))
                                {
                                    int next_index = get_Hexstia_linkzone(i);
                                    if (next_index != -1 && (available & (int)(System.Math.Pow(2, next_index))) > 0)
                                    {
                                        return (int)(System.Math.Pow(2, next_index));
                                    }
                                }
                            }
                        }
                        if (cardId == CardId.Hexstia)
                        {
                            // ex zone
                            if ((Zones.z5 & available) > 0 && Bot.MonsterZone[1] != null && isAltergeist(Bot.MonsterZone[1])) return Zones.z5;
                            if ((Zones.z6 & available) > 0 && Bot.MonsterZone[3] != null && isAltergeist(Bot.MonsterZone[3])) return Zones.z6;
                            if ( ((Zones.z6 & available) > 0 && Bot.MonsterZone[3] != null && !isAltergeist(Bot.MonsterZone[3]))
                                || ((Zones.z5 & available) > 0 && Bot.MonsterZone[1] == null) ) return Zones.z5;
                            if (((Zones.z5 & available) > 0 && Bot.MonsterZone[1] != null && !isAltergeist(Bot.MonsterZone[1]))
                                || ((Zones.z6 & available) > 0 && Bot.MonsterZone[3] == null)) return Zones.z6;
                            // main zone
                            for (int i = 1; i < 5; ++i)
                            {
                                if (Bot.MonsterZone[i] != null && isAltergeist(Bot.MonsterZone[i]))
                                {
                                    if ((available & (int)System.Math.Pow(2, i - 1)) > 0) return (int)System.Math.Pow(2, i - 1);
                                }
                            }
                        }
                        // 1 or 3
                        if ((Zones.z1 & available) > 0) return Zones.z1;
                        if ((Zones.z3 & available) > 0) return Zones.z3;
                    }
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        protected override bool DefaultSetForDiabellze()
        {
            if (base.DefaultSetForDiabellze())
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;
            }
            return false;
        }
    }
}