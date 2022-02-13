using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("[HealingJAM]", "AI_[HealingJAM]")]
    public class HealingJAMExecutor : DefaultExecutor
    {
        public class Monster
        {
            public const int
                // main deck monsters
                AmusiPerformer  = 160201031, // Amusi Performer
                Candy           = 160005002, // Can:D
                CureBlue        = 160004017, // Chemical Cure Blue
                CureRed         = 160204031, // Chemical Cure Red
                Giftarist       = 160306003, // Giftarist
                Guitarna        = 160001028, // Prima Guitarna the Shining Superstar
                HowlingBird     = 160201035, // Howling Bird
                PeaceHolder     = 160306009, // Peace Holder (currently unused)
                Phickup         = 160007014, // Psyphickupper
                // fusion monsters
                CanDLive        = 160007035, // Can:D Live
                CurePurple      = 160204030, // Chemical Cure Purple
                HowlingPerformer= 160204029; // Amusi Howling Performer
        }

        public class Spell
        {
            public const int
                BlueMedicine    = 160002044, // Blue Medicine
                Devastation     = 160007055, // Card Devastation (currently unused)
                Fusion          = 160204050, // Fusion
                Charity         = 160204049, // Graceful Charity
                JamPStart       = 160005040, // Jam:P Start (currently unused)
                JamPSet         = 160006041, // Jam:P Set
                RedMedicine     = 160204032, // Red Medicine
                Restart         = 160204045; // Star Restart
        }

        public class Trap
        {
            public const int
                ComebackFeelshock = 160007056, // Comeback! Feelshock!!
                PsychicTrapHole   = 160306026; // Psychic Trap Hole (currently unused)
        }

        private readonly Dictionary<int, int> Ratios = new Dictionary<int, int>()
        {
            { Monster.Candy, 3 },
            { Monster.HowlingBird, 3 },
            { Monster.Giftarist, 1 },
            { Monster.Guitarna, 3 },
            { Monster.Phickup, 3 },
            { Monster.CureRed, 3 },
            { Monster.CureBlue, 3 },
            { Monster.AmusiPerformer, 3 },
            { Monster.CanDLive, 3 },
            { Monster.CurePurple, 3 },
            { Monster.HowlingPerformer, 3 },
            { Spell.BlueMedicine, 3 },
            { Spell.JamPSet, 2 },
            { Spell.RedMedicine, 3 },
            { Spell.Restart, 3 },
            { Spell.Charity, 1 },
            { Spell.Fusion, 3 },
            { Trap.ComebackFeelshock, 3 }
        };

        public HealingJAMExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, Trap.ComebackFeelshock, FeelshockActivate);
            AddExecutor(ExecutorType.Activate, CureActivate);
            AddExecutor(ExecutorType.Activate, EarlyMedicine);
            AddExecutor(ExecutorType.Activate, Monster.Giftarist, EarlyGiftarist);
            AddExecutor(ExecutorType.Activate, Monster.Phickup, PhickupHeal);
            AddExecutor(ExecutorType.Repos, ReposFusionMaterial);

            // Fusion Summon Can:D Live
            AddExecutor(ExecutorType.Activate, Spell.Fusion, FusionCanDLive);
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMateriableCandy);
            AddExecutor(ExecutorType.Activate, Spell.Restart, FetchFusionForCanDLive);
            AddExecutor(ExecutorType.Summon, Monster.Candy, NSMateriableCandy);
            AddExecutor(ExecutorType.Summon, Monster.Phickup, TSMateriablePhickupUsingNonCandy);
            AddExecutor(ExecutorType.Summon, Monster.Phickup, TSMateriablePhickupUsingRevivableCandy);
            AddExecutor(ExecutorType.Summon, NSNonCandyAsMateriablePhickupFodder);
            AddExecutor(ExecutorType.Summon, Monster.Candy, NSCandyAsMateriablePhickupFodder);
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMateriablePhickupFodder);

            AddExecutor(ExecutorType.Activate, Spell.Charity);

            // Fusion Summon Chemical Cure Purple
            AddExecutor(ExecutorType.Activate, Monster.CurePurple, CurePurpleDraw);
            AddExecutor(ExecutorType.Activate, Spell.Fusion, FusionCurePurple);
            AddExecutor(ExecutorType.Activate, Spell.Restart, FetchFusionForCurePurple);
            AddExecutor(ExecutorType.Summon, NSMateriableCure);

            // Howling Performer might get tributed so we'll do it earlier
            AddExecutor(ExecutorType.Activate, Monster.HowlingPerformer);
            // Tribute Summon Guitarna/Giftarist
            AddExecutor(ExecutorType.Summon, NSLevel7Fodder);
            AddExecutor(ExecutorType.Summon, Monster.Guitarna, TSGuitarna);
            AddExecutor(ExecutorType.Summon, Monster.Giftarist, TSGiftarist);

            // Fusion Summon Amusi Howling Performer
            AddExecutor(ExecutorType.Activate, Spell.Fusion); // only fusion left
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMateriableHowlingBird);
            AddExecutor(ExecutorType.Activate, Spell.Restart, FetchFusionForHowlingPerformer);
            AddExecutor(ExecutorType.Summon, Monster.HowlingBird, NSMateriableHowlingBird);
            AddExecutor(ExecutorType.Summon, Monster.AmusiPerformer, NSMateriableAmusiPerformer);

            // Tribute Level 7 then revive
            AddExecutor(ExecutorType.Activate, Monster.CanDLive, CanDLiveRevive);
            AddExecutor(ExecutorType.Summon, TSHighLevelUsingRevivableLevel7);
            AddExecutor(ExecutorType.Summon, TSRevivableLevel7);
            AddExecutor(ExecutorType.Summon, NSExtraTributeAlongRevivableLevel7);
            AddExecutor(ExecutorType.Summon, NSTributeForRevivableLevel7);

            AddExecutor(ExecutorType.Activate, LateMedicine);
            AddExecutor(ExecutorType.Summon, NSCureMisc);
            AddExecutor(ExecutorType.Activate, Monster.CanDLive, CanDLiveReduce);
            AddExecutor(ExecutorType.Summon, Monster.AmusiPerformer, NSAmusiPerformerMisc);
            AddExecutor(ExecutorType.Summon, Monster.Phickup, TSPhickupMisc);
            AddExecutor(ExecutorType.Summon, NSPhickupFodderMisc);
            AddExecutor(ExecutorType.Summon, NSLowlevelMisc);
            AddExecutor(ExecutorType.Repos, ReposForBoost);
            AddExecutor(ExecutorType.Activate, Monster.Guitarna, GuitarnaBoost);
            AddExecutor(ExecutorType.Activate, Monster.Giftarist);
            AddExecutor(ExecutorType.Activate, Spell.JamPSet, JamPSetActivate);
            AddExecutor(ExecutorType.Activate, Monster.AmusiPerformer, AmusiPerformerBurn);
            AddExecutor(ExecutorType.Activate, Monster.CanDLive, CanDLiveReduceFallback);
            AddExecutor(ExecutorType.Activate, Monster.CanDLive, CanDLiveReviveFallback);
            AddExecutor(ExecutorType.Repos, FinalRepos);
            AddExecutor(ExecutorType.MonsterSet, DefensiveMonsterSet);

            // Set Spell/Traps
            AddExecutor(ExecutorType.SpellSet, Trap.ComebackFeelshock, FeelshockSet);
            AddExecutor(ExecutorType.SpellSet, Spell.Fusion, RetrievableFusionSet);
            AddExecutor(ExecutorType.SpellSet, SpellTrapSet);
            AddExecutor(ExecutorType.SpellSet, Spell.Restart, RestartSet);
        }

        public override bool OnSelectHand()
        {
            // go second
            return false;
        }

        // keep track of activated effects, since all Rush effects are soft OPT
        private int UsedOPTs = 0;

        public override void OnNewTurn()
        {
            // reset
            UsedOPTs = 0;
        }

        private int GetRandomZone(int choices)
        {
            List<int> valid = new List<int>();
            for (int z = Zones.z1; z <= choices; z <<= 1)
            {
                if ((z & choices) > 0)
                    valid.Add(z);
            }
            return valid.Count > 0 ? valid[Program.Rand.Next(valid.Count)] : 0;
        }

        public override int OnSelectPlace(
            long cardId,
            int player,
            CardLocation location,
            int available
        ){
            int zone = GetRandomZone(available);
            if (location == CardLocation.MonsterZone && player == 0)
                UsedOPTs &= ~zone;
            return zone;
        }

        public override bool OnPreActivate(ClientCard card)
        {
            return !card.HasType(CardType.Spell)
                || card.IsCode(Spell.Restart)
                || card.Location == CardLocation.Hand
                || Bot.GetSpellCountWithoutField() > 2
                || !Bot.HasInHand(card.Id);
        }

        public override void OnChaining(int player, ClientCard card)
        {
            if (player == 0 && card.IsMonster())
                UsedOPTs |= (1 << card.Sequence);
        }

        public override bool OnSelectYesNo(long desc)
        {
            return Duel.LastChainPlayer == 0 || base.OnSelectYesNo(desc);
        }

        public override BattlePhaseAction OnSelectAttackTarget(
            ClientCard attacker,
            IList<ClientCard> defenders
        ){
            List<ClientCard> sorted = defenders.ToList();
            sorted.Sort(CardContainer.CompareDefensePower);
            sorted.Reverse();

            IList<ClientCard> priority =
                sorted.GetMatchingCards(c => c.HasType(CardType.Fusion));
            if (priority.Count > 0)
            {
                BattlePhaseAction choice = base.OnSelectAttackTarget(attacker, priority);
                if (choice != null)
                    return choice;
            }

            priority = sorted.GetMatchingCards(c => {
                return c.BaseAttack >= attacker.Attack
                    && c.GetDefensePower() <= attacker.Attack;
            });
            if (priority.Count > 0)
            {
                BattlePhaseAction choice = base.OnSelectAttackTarget(attacker, priority);
                if (choice != null)
                    return choice;
            }

            return base.OnSelectAttackTarget(attacker, sorted);
        }

        private int[] TributeFallbackPriority = {
            Monster.HowlingBird,
            Monster.AmusiPerformer,
            Monster.PeaceHolder,
            Monster.Candy,
            Monster.CureBlue,
            Monster.CureRed,
            Monster.Phickup,
            Monster.HowlingPerformer,
            Monster.Giftarist,
            Monster.CurePurple,
            Monster.Guitarna,
        };

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards,
            int min,
            int max,
            long hint,
            bool cancelable
        ){
            if (min == max && max == cards.Count)
                return cards;

            if (hint == HintMsg.Release)
            {
                IList<ClientCard> choices = cards;
                if (TributeFodder != null && TributeFodder.Count > 0)
                {
                    TributeFodder = TributeFodder.GetMatchingCards(c => cards.Contains(c));
                    if (TributeFodder.Count == min)
                        return TributeFodder;
                    if (TributeFodder.Count > 0)
                        choices = TributeFodder;
                }

                IList<ClientCard> select = new List<ClientCard>();
                while (select.Count < min)
                {
                    int ct = select.Count;
                    foreach (int id in TributeFallbackPriority)
                    {
                        ClientCard match = FirstMatch(choices, id, select);
                        if (match != null)
                            select.Add(match);
                        if (select.Count == min)
                            return select;
                    }
                    // avoid looping infinitely if nothing is added in the last loop
                    if (ct == select.Count)
                        return null;
                }

                return null;
            }

            ClientCard act = Util.GetLastChainCard();
            if (act == null || Duel.LastChainPlayer != 0)
                return null;

            // prioritize shuffling back Amusi Performer,
            // since we might need a normal in the GY
            if (act.IsCode(Monster.HowlingPerformer))
            {
                ClientCard amusi = FirstMatch(cards, Monster.AmusiPerformer);
                if (amusi != null)
                    return new List<ClientCard>() { amusi };
                return null;
            }

            // recover all medicines possible
            if (act.IsCode(Monster.CureBlue) || act.IsCode(Monster.CureRed))
                return cards;

            if (act.IsCode(Spell.JamPSet) && cards[0].Controller == 1)
                return Util.CheckSelectCount(cards, cards, max, max);

            if (act.IsCode(Spell.Charity))
            {
                List<ClientCard> select = new List<ClientCard>();

                bool blueAdded = false;
                bool redAdded = false;

                // discard Blue Medicines that we can recover
                if (HasUnusedMonster(Monster.CureBlue) || Bot.HasInHand(Monster.CureBlue))
                {
                    blueAdded = true;
                    select.AddRange(cards.GetMatchingCards(c => c.IsCode(Spell.BlueMedicine)));
                }

                // discard Red Medicines that we can recover
                if (Enemy.GetMonsterCount() > 0
                    && (HasUnusedMonster(Monster.CureRed) || Bot.HasInHand(Monster.CureRed)))
                {
                    redAdded = true;
                    select.AddRange(cards.GetMatchingCards(c => c.IsCode(Spell.RedMedicine)));
                }

                // discard unusable Jam:P Set
                if (cards.ContainsCardWithId(Spell.JamPSet) &&
                    !Enemy.MonsterZone.IsExistingMatchingCard(c => {
                    return c.Level < 9 && GetBattleValue(c) >= Util.GetBestAttack(Bot);
                }))
                    select.Add(FirstMatch(cards, Spell.JamPSet));

                // discard a normal monster if Star Restart or Jam:P Set needs fodder
                if (!Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                    && cards.IsExistingMatchingCard(IsNormal)
                    && !cards.IsExistingMatchingCard(c => !IsLowLevel(c)))
                {
                    if (Bot.HasInHandOrInSpellZone(Spell.Restart)
                        || Bot.HasInHandOrInSpellZone(Spell.JamPSet))
                    {
                        List<ClientCard> normals = cards.GetMatchingCards(IsNormal).ToList();
                        normals.Sort(CardContainer.CompareCardAttack);
                        select.Add(normals[0]);
                    }
                }

                // discard extra Level 7, or discard one if we can revive it with Can:D Live
                int level7 = cards.GetMatchingCardsCount(IsLevel7);
                if (level7 > 1 || (level7 == 1 && HasUnusedMonster(Monster.CanDLive)))
                {
                    if (cards.ContainsCardWithId(Monster.Giftarist))
                        select.Add(FirstMatch(cards, Monster.Giftarist));
                    else
                        select.Add(FirstMatch(cards, IsLevel7));
                }

                // discard a Fusion if we can recover it with Star Restart
                if (cards.ContainsCardWithId(Spell.Fusion)
                    && Bot.HasInHandOrInSpellZone(Spell.Restart))
                    select.Add(FirstMatch(cards, Spell.Fusion));

                // discard medicines (even if we can't immediately recover them)
                if (!blueAdded)
                    select.AddRange(cards.GetMatchingCards(c => c.IsCode(Spell.BlueMedicine)));
                if (!redAdded)
                    select.AddRange(cards.GetMatchingCards(c => c.IsCode(Spell.RedMedicine)));

                if (select.Count > 1)
                    return Util.CheckSelectCount(select, cards, min, min);

                // try to get rid of duplicates
                foreach (int id in HandFodderFallbackPriority)
                {
                    ClientCard add = FirstMatch(cards, id, select);
                    if (add == null)
                        continue;
                    int count = Bot.SpellZone.GetCardCount(id) + cards.GetCardCount(id);
                    if (select.ContainsCardWithId(id))
                        count--;
                    if (count < 2)
                        continue;
                    select.Add(add);
                    if (select.Count > 1)
                        return Util.CheckSelectCount(select, cards, min, min);
                }

                while (select.Count < 2)
                {
                    // discard according to default importance
                    foreach (int id in HandFodderFallbackPriority)
                    {
                        ClientCard add = FirstMatch(cards, id, select);
                        if (add == null)
                            continue;
                        select.Add(add);
                        if (select.Count > 1)
                            return Util.CheckSelectCount(select, cards, min, min);
                    }
                }
            }

            return null;
        }

        // the medicines are not included since they're checked beforehand
        private readonly int[] HandFodderFallbackPriority = {
            Monster.HowlingBird,
            Monster.AmusiPerformer,
            Monster.Giftarist,
            Spell.JamPSet,
            Spell.Fusion,
            Trap.ComebackFeelshock,
            Monster.CureBlue,
            Monster.CureRed,
            Monster.Guitarna,
            Monster.Candy,
            Monster.Phickup,
            Spell.Restart
        };

        private ClientCard FirstMatch(IList<ClientCard> list, Func<ClientCard, bool> filter)
        {
            return list.GetFirstMatchingCard(c => filter.Invoke(c));
        }

        private ClientCard FirstMatch(IList<ClientCard> list, int id)
        {
            return FirstMatch(list, c => c.IsCode(id));
        }

        private ClientCard FirstMatch(IList<ClientCard> list, int id, ClientCard exc)
        {
            return list.GetFirstMatchingCard(c => c.IsCode(id) && exc != c);
        }

        private ClientCard FirstMatch(IList<ClientCard> list, int id, IList<ClientCard> exc)
        {
            return list.GetFirstMatchingCard(c => c.IsCode(id) && !exc.Contains(c));
        }

        private bool IsLowLevel(ClientCard c)
        {
            return c.IsMonster() && c.Level < 5;
        }

        private bool IsLevel7(ClientCard c)
        {
            return c.IsMonster() && c.Level == 7;
        }

        private bool IsPsychic(ClientCard c)
        {
            return c.IsMonster() && c.HasRace(CardRace.Psycho);
        }

        private bool IsNormal(ClientCard c)
        {
            return c.IsMonster() && c.HasType(CardType.Normal);
        }

        private int GetBattleValue(ClientCard c)
        {
            if (c.IsAttack())
                return c.Attack;
            return c.IsFaceup() ? c.Defense : 1200;
        }

        private int CountRetrievableMedicines(int code)
        {
            if (Bot.Deck.Count < 1)
                return 0;
            if (code == Monster.CureRed && Enemy.GetMonsterCount() > 0)
                return Bot.Graveyard.GetMatchingCardsCount(c => c.IsCode(Spell.RedMedicine));
            if (code == Monster.CureBlue)
                return Bot.Graveyard.GetMatchingCardsCount(c => c.IsCode(Spell.BlueMedicine));
            return 0;
        }

        private bool HasRemaining(int code)
        {
            return Ratios.ContainsKey(code) && Bot.GetRemainingCount(code, Ratios[code]) > 0;
        }

        private bool HasUnusedMonster(Func<ClientCard, bool> filter)
        {
            return Bot.MonsterZone.IsExistingMatchingCard(c => {
                return c.IsFaceup() && filter.Invoke(c) && (UsedOPTs & (1 << c.Sequence)) == 0;
            });
        }

        private bool HasUnusedMonster(int code)
        {
            return HasUnusedMonster(c => c.IsCode(code));
        }

        private int UnusedMonsterCount(Func<ClientCard, bool> filter = null)
        {
            return Bot.MonsterZone.GetMatchingCardsCount(c => {
                if (filter != null && !filter.Invoke(c))
                    return false;
                return c.IsFaceup() && (UsedOPTs & (1 << c.Sequence)) == 0;
            });
        }

        private int CountUnusedMonsters(int code)
        {
            return UnusedMonsterCount(c => c.IsCode(code));
        }

        // if we have the spell in our Spell/Trap zone,
        // or if we have it in our hand and we have a free zone
        private bool HasUsableSpell(int code, int szoneexc = 0)
        {
            if (Bot.HasInSpellZone(code))
                return true;
            return Bot.HasInHand(code) && Bot.GetSpellCountWithoutField() - szoneexc < 3;
        }

        // if we have a usable Star Restart, similar to HasUsableSpell,
        // but has to take into account cards in the hand that can be cost
        private bool HasUsableRestart(int handexc = 0, int szoneexc = 0)
        {
            if (Bot.HasInSpellZone(Spell.Restart))
                return Bot.Hand.Count - handexc > 0;
            return Bot.HasInHand(Spell.Restart)
                && Bot.Hand.Count - handexc > 1
                && Bot.GetSpellCountWithoutField() - szoneexc < 3;
        }

        // activate medicines early if we can recover them
        // (otherwise, we might want to use them as fodder for effects later)
        private bool EarlyMedicine()
        {
            if (Card.IsCode(Spell.RedMedicine))
                return Enemy.GetMonsterCount() > 0 && HasUnusedMonster(Monster.CureRed);
            return Card.IsCode(Spell.BlueMedicine) && HasUnusedMonster(Monster.CureBlue);
        }

        // Giftarist can achieve max ATK boost early
        private bool EarlyGiftarist()
        {
            return Bot.MonsterZone.GetMatchingCardsCount(IsPsychic) > 2
                || (Bot.GetMonsterCount() > 2 && Bot.HasInMonstersZone(Monster.CanDLive));
        }

        private bool PhickupHeal()
        {
            if (Bot.HasInMonstersZone(Monster.Candy) && HasUsableSpell(Spell.Fusion))
                return true;
            int atk = Card.Attack;
            int oppcount = Enemy.GetMonsterCount();
            if (oppcount < 1)
            {
                int diff = Util.GetTotalAttackingMonsterAttack(0) - Enemy.LifePoints;
                return diff < 0 || diff > 800;
            }
            if (HasUnusedMonster(Monster.Guitarna))
                atk += (oppcount * 300);
            return !Enemy.MonsterZone.IsExistingMatchingCard(c => {
                int diff = atk - GetBattleValue(c);
                return diff >= 0 && diff < 800;
            });
        }

        private int GetFeelshockRecoverChoice(bool fusableonly)
        {
            bool restart = HasUsableRestart(0, 1) && Bot.HasInGraveyard(Spell.Fusion);
            if (restart || HasUsableSpell(Spell.Fusion, 1))
            {
                if (Bot.HasInGraveyard(Monster.Phickup) && !Bot.HasInHand(Monster.Phickup)
                    && (Bot.HasInHand(Monster.Candy)
                    || (restart && Bot.HasInGraveyard(Monster.Candy))))
                    return Monster.Phickup;
                if (Bot.HasInHand(Monster.CureRed) && !Bot.HasInHand(Monster.CureBlue)
                    && Bot.HasInGraveyard(Monster.CureBlue))
                    return Monster.CureBlue;
                if (Bot.HasInHand(Monster.CureBlue) && !Bot.HasInHand(Monster.CureRed)
                    && Bot.HasInGraveyard(Monster.CureRed))
                    return Monster.CureRed;
            }
            if (!Bot.HasInHand(Monster.Phickup) && Bot.HasInGraveyard(Monster.Phickup)
                && (Bot.Graveyard.GetCardCount(Spell.Restart) < 3
                    || (Bot.Graveyard.GetCardCount(Monster.Candy) < 3
                    && Bot.Graveyard.GetCardCount(Spell.Fusion) < 3)))
                    return Monster.Phickup;
            if (fusableonly)
                return 0;

            if (Bot.HasInGraveyard(Monster.Phickup) && !Bot.HasInHand(Monster.Phickup))
                return Monster.Phickup;
            int red = Bot.HasInGraveyard(Monster.CureRed)
                ? CountRetrievableMedicines(Monster.CureRed) : -1;
            if (Bot.HasInGraveyard(Monster.CureBlue)
                && (red < 0 || CountRetrievableMedicines(Monster.CureBlue) > red))
                return Monster.CureBlue;
            return red >= 0 ? Monster.CureRed : 0;
        }

        // the selection will be handled in OnSelectCard,
        // so we don't have to repeatedly call AI.SelectCard
        private bool FeelshockActivate()
        {
            int tg = GetFeelshockRecoverChoice(true);
            if (tg > 0)
            {
                AI.SelectCard(tg);
                return true;
            }

            // TODO: try to consider piercing effects (might need hardcoding)
            int diff = 0;
            if (Enemy.BattlingMonster != null)
            {
                if (Bot.BattlingMonster == null)
                    diff = Enemy.BattlingMonster.Attack;
                else if (Bot.BattlingMonster.IsAttack())
                    diff = Enemy.BattlingMonster.Attack - Bot.BattlingMonster.Attack;
            }
            // TODO: predict actual remaining LP after all battles
            // (this is a temporary attempt at it and misses a lot of cases)
            int remlp = Bot.LifePoints + Util.GetTotalAttackingMonsterAttack(0);
            if (!Bot.MonsterZone.IsExistingMatchingCard(c => !c.IsAttack()))
                remlp -= Util.GetTotalAttackingMonsterAttack(1);
                
            if (Bot.GetSpellCountWithoutField() < 3 || remlp <= 0 || diff >= Bot.LifePoints
                || (tg > 0 && Bot.SpellZone.GetCardCount(Trap.ComebackFeelshock) < 2))
            {
                tg = GetFeelshockRecoverChoice(false);
                if (tg > 0)
                    AI.SelectCard(tg);
                else
                {
                    List<ClientCard> choices = Bot.Graveyard.GetMatchingCards(c => {
                        return IsPsychic(c) && c.Attack > 0 && c.Level < 7;
                    }).ToList();
                    choices.Sort(CardContainer.CompareCardAttack);
                    choices.Reverse();
                    AI.SelectCard(choices[0]);
                }
            }

            return false;
        }

        // activate Chemical Cure Blue/Red only if there's no corresponding medicine in hand
        private bool CureActivate()
        {
            // consider when spell zone is full
            return (Card.IsCode(Monster.CureRed) && !Bot.HasInHand(Spell.RedMedicine))
                || (Card.IsCode(Monster.CureBlue) && !Bot.HasInHand(Spell.BlueMedicine));
        }

        // Fusion needs the materials to be face-up
        private bool ReposFusionMaterial()
        {
            if (Card.IsFaceup() || !HasUsableSpell(Spell.Fusion))
                return false;
            if (Card.IsCode(Monster.Candy))
                return Bot.HasInMonstersZone(Monster.Phickup);
            if (Card.IsCode(Monster.Phickup))
                return Bot.HasInMonstersZone(Monster.Candy);
            if (Card.IsCode(Monster.CureBlue))
                return Bot.HasInMonstersZone(Monster.CureRed);
            if (Card.IsCode(Monster.CureRed))
                return Bot.HasInMonstersZone(Monster.CureBlue);
            if (Card.IsCode(Monster.AmusiPerformer))
                return Bot.HasInMonstersZone(Monster.HowlingBird);
            return Card.IsCode(Monster.HowlingBird)
                && Bot.HasInMonstersZone(Monster.AmusiPerformer);
        }

        // make sure we're summoning Can:D Live
        private bool FusionCanDLive()
        {
            return Bot.HasInMonstersZone(Monster.Candy)
                && Bot.HasInMonstersZone(Monster.Phickup);
        }

        // revive any Normal Monster just to recover Fusion to summon Can:D Live
        private bool FetchFusionForCanDLive()
        {
            return Bot.HasInGraveyard(Spell.Fusion) && FusionCanDLive();
        }

        private ClientCard GetStarRestartCost(IList<ClientCard> choices)
        {
            // unusable Jam:P Set
            ClientCard cost = FirstMatch(choices, Spell.JamPSet);
            if (cost != null && !Enemy.MonsterZone.IsExistingMatchingCard(c => {
                return c.Level < 9 && GetBattleValue(c) >= Util.GetBestAttack(Bot);
            }))
                return cost;

            // extra Level 7
            if (choices.GetMatchingCardsCount(IsLevel7) > 1)
            {
                cost = FirstMatch(choices, Monster.Giftarist);
                return cost == null ? FirstMatch(choices, IsLevel7) : cost;
            }

            // Fusion, if we can recover one from the GY
            cost = FirstMatch(choices, Spell.Fusion);
            if (cost != null && Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
                return cost;

            // spare medicines
            cost = FirstMatch(choices, Spell.BlueMedicine);
            if (cost != null)
                return cost;
            cost = FirstMatch(choices, Spell.RedMedicine);
            if (cost != null)
                return cost;

            // try to get rid of duplicates
            foreach (int id in HandFodderFallbackPriority)
            {
                cost = FirstMatch(choices, id);
                if (cost != null
                    && (Bot.SpellZone.GetCardCount(id) + choices.GetCardCount(id)) > 1)
                    return cost;
            }
            // fallback importance
            foreach (int id in HandFodderFallbackPriority)
            {
                cost = FirstMatch(choices, id);
                if (cost != null)
                    return cost;
            }

            return choices[Program.Rand.Next(choices.Count)];
        }

        // Revive Can:D using Star Restart to be used as fusion material
        private bool ReviveMateriableCandy()
        {
            if (!Bot.HasInGraveyard(Monster.Candy) || !Bot.HasInMonstersZone(Monster.Phickup))
                return false;

            // exclude the Star Restart to be activated (if activating from the hand)
            IList<ClientCard> cost = new List<ClientCard>(Bot.Hand);
            if (cost.Contains(Card))
                cost.Remove(Card);

            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // don't use 1 Fusion in hand as cost
                if (Bot.HasInHand(Spell.Fusion))
                    cost.Remove(FirstMatch(cost, Spell.Fusion));
                else
                    return false;
            }

            if (cost.Count < 1)
                return false;

            AI.SelectCard(GetStarRestartCost(cost));
            AI.SelectNextCard(Monster.Candy);
            return true;
        }

        // Normal Summon Can:D to be used immediately as fusion material
        private bool NSMateriableCandy()
        {
            if (!Bot.HasInMonstersZone(Monster.Phickup))
                return false;

            // everything is ready and we can fuse immediately after
            if (HasUsableSpell(Spell.Fusion))
                return true;

            // there's a recoverable Fusion in the GY
            // or we can revive a normal monster to recover it
            return Bot.GetMonsterCount() < 2 && Bot.HasInGraveyard(Spell.Fusion)
                && Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                && HasUsableRestart(handexc: 1);
        }

        private IList<ClientCard> TributeFodder = null;
        // Tribute Summon Psyphickupper to be used as fusion material,
        // using a non-Can:D monster as tribute (unless there's more than 1)
        private bool TSMateriablePhickupUsingNonCandy()
        {
            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => !c.IsCode(Monster.CanDLive));
            if (TributeFodder.Count < 1)
                return false;

            // excluding the Psyphickupper itself
            int costexc = 1;
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand if there are no others available
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            int mcount = Bot.GetMonsterCount();

            // there is a Can:D in the GY that we can revive
            if (Bot.HasInGraveyard(Monster.Candy) && mcount < 3 && HasUsableRestart(costexc))
                return true;

            // reserve 1 copy of Can:D in case we can fuse immediately after summoning.
            // Cases where we NEED to tribute Can:D itself is handled in another function
            if (Bot.HasInMonstersZone(Monster.Candy))
            {
                if (TributeFodder.Count < 2)
                {
                    if (!Bot.HasInHand(Monster.Candy) || mcount > 2)
                        return false;
                    costexc++;
                }
                else
                    TributeFodder.Remove(FirstMatch(TributeFodder, Monster.Candy));
            }
            // the only available Can:D is in hand, so exclude it as Restart cost
            // and make sure there's space to normal summon it later
            else if (Bot.HasInHand(Monster.Candy) && mcount < 3)
                costexc++;
            else
                return false;

            // everything is ready
            if (HasUsableSpell(Spell.Fusion))
                return true;

            // we have Star Restart (to recover Fusion)
            if (!HasUsableRestart(costexc))
                return false;

            // we must be able to revive a normal monster from the GY to recover Fusion
            if (!Bot.Graveyard.IsExistingMatchingCard(IsNormal))
                TributeFodder = TributeFodder.GetMatchingCards(IsNormal);
            return TributeFodder.Count > 0;
        }

        // Tribute Summon Psyphickupper to be used as fusion material,
        // using a Can:D which can be revived using Star Restart
        private bool TSMateriablePhickupUsingRevivableCandy()
        {
            TributeFodder = null;

            // we must control exactly 1 Can:D, and there must be space for reviving later
            if (Bot.MonsterZone.GetCardCount(Monster.Candy) != 1 || Bot.GetMonsterCount() > 2)
                return false;

            // excluding the Psyphickupper itself
            int costexc = 1;

            // we must have Fusion ready or we can recover it
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand as Restart cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            if (!HasUsableRestart(costexc))
                return false;

            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => c.IsCode(Monster.Candy));
            return true;
        }

        // Normal Summon a low-level monster to be used as tribute for Psyphickupper,
        // except Can:D (unless there's another Can:D available)
        private bool NSNonCandyAsMateriablePhickupFodder()
        {
            if (!IsLowLevel(Card) || !Bot.HasInHand(Monster.Phickup))
                return false;

            // excluding Psyphickupper and the fodder monster itself
            int costexc = 2 - CountRetrievableMedicines(Card.Id);
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand if there are no others available
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            int mcount = Bot.GetMonsterCount();

            // there's a Can:D in the GY that we can revive
            if (Bot.HasInGraveyard(Monster.Candy) && mcount < 2 && HasUsableRestart(costexc))
                return true;

            if (!Bot.HasInMonstersZone(Monster.Candy))
            {
                // the only available Can:D is in hand (if any)
                int handcandies = Bot.Hand.GetCardCount(Monster.Candy);
                if (Card.IsCode(Monster.Candy))
                    handcandies--;
                if (mcount > 1 || handcandies < 1)
                    return false;
                costexc++;
            }

            // everything is ready
            if (HasUsableSpell(Spell.Fusion))
                return true;

            // we must be able to revive a normal monster from the GY to recover Fusion
            return mcount < 2 && HasUsableRestart(costexc)
                && (IsNormal(Card) || Bot.Graveyard.IsExistingMatchingCard(IsNormal));
        }

        // Normal Summon the only Can:D in hand to be used as tribute
        // for Psyphickupper, we can revive it with Star Restart later
        private bool NSCandyAsMateriablePhickupFodder()
        {
            // we must not already control Can:D, and there must be space for reviving later
            if (Bot.HasInMonstersZone(Monster.Candy)
                || !Bot.HasInHand(Monster.Phickup)
                || Bot.GetMonsterCount() > 1)
                return false;

            // excluding the Psyphickupper and Can:D in hand
            int costexc = 2;

            // we must have Fusion ready or we can recover it
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand as Restart cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            return HasUsableRestart(costexc);
        }

        // revive any normal monster to be tributed for Psyphickupper
        // which will be used as fusion material later
        private bool ReviveMateriablePhickupFodder()
        {
            if (!Bot.HasInHand(Monster.Phickup))
                return false;

            // exclude the Star Restart to be activated (if activating from the hand)
            IList<ClientCard> cost = Bot.Hand.ToList();
            cost.Remove(FirstMatch(cost, Monster.Phickup));
            if (cost.Contains(Card))
                cost.Remove(Card);

            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // don't use 1 Fusion in hand as cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                cost.Remove(FirstMatch(cost, Spell.Fusion));
            }

            if (cost.Count < 1)
                return false;

            int mcount = Bot.GetMonsterCount();

            // there's a Can:D in the GY and we have 2 available Star Restart
            if (Bot.HasInGraveyard(Monster.Candy) && mcount < 2)
            {
                ClientCard otherrestart = FirstMatch(Bot.GetSpells(), Spell.Restart, Card);
                if (otherrestart == null)
                    otherrestart = FirstMatch(cost, Spell.Restart, Card);
                if (otherrestart != null)
                {
                    // there must be another cost for the other Restart
                    if (cost.Count > 2 || (cost.Contains(otherrestart) && cost.Count > 1))
                    {
                        cost.Remove(otherrestart);
                        AI.SelectCard(GetStarRestartCost(cost));
                        return true;
                    }
                } 
            }

            if (!Bot.HasInMonstersZone(Monster.Candy))
            {
                // there must be an available Can:D is in hand
                if (mcount > 1 || !Bot.Hand.ContainsCardWithId(Monster.Candy))
                    return false;
                cost.Remove(FirstMatch(cost, Monster.Candy));
            }

            if (cost.Count < 1)
                return false;

            AI.SelectCard(GetStarRestartCost(cost));
            AI.SelectNextCard(Monster.Candy, Monster.HowlingBird, Monster.PeaceHolder);
            return true;
        }

        // make sure we're summoning Chemical Cure Purple
        private bool FusionCurePurple()
        {
            return Bot.HasInMonstersZone(Monster.CureRed)
                && Bot.HasInMonstersZone(Monster.CureBlue);
        }

        // revive any Normal Monster just to recover Fusion to summon Chemical Cure Purple
        private bool FetchFusionForCurePurple()
        {
            return Bot.HasInGraveyard(Spell.Fusion) && FusionCurePurple();
        }

        // Normal Summon Chemical Cure Red/Blue to be used as fusion material
        private bool NSMateriableCure()
        {
            int pair = 0;
            if (Card.IsCode(Monster.CureRed))
                pair = Monster.CureBlue;
            else if (Card.IsCode(Monster.CureBlue))
                pair = Monster.CureRed;
            else
                return false;

            int costexc = 1 - CountRetrievableMedicines(Card.Id);
            int mcount = Bot.GetMonsterCount();
            bool mustNormalPair = false;
            if (!Bot.HasInMonstersZone(pair))
            {
                if (Bot.HasInHand(pair) && mcount < 2)
                {
                    mustNormalPair = true;
                    costexc -= CountRetrievableMedicines(pair) + 1;
                }
                else
                    return false;
            }

            if (HasUsableSpell(Spell.Fusion) && (!mustNormalPair || mcount < 2))
                return true;

            return Bot.HasInGraveyard(Spell.Fusion)
                && HasUsableRestart(handexc: costexc)
                && (!mustNormalPair || mcount < 1)
                && Bot.Graveyard.IsExistingMatchingCard(IsNormal);
        }

        // Normal Summon a low-level monster to be tributed for Guitarna or Giftarist
        private bool NSLevel7Fodder()
        {
            if (!IsLowLevel(Card) || !Bot.Hand.IsExistingMatchingCard(IsLevel7))
                return false;

            int fodders = Bot.MonsterZone.GetMatchingCardsCount(c => c.Level < 7);
            return fodders < 2 && (fodders + Bot.Hand.GetMatchingCardsCount(IsLowLevel)) > 1;
        }

        private bool IsGuitarnaFodderable(ClientCard c, int boosted)
        {
            if (c.IsCode(Monster.CanDLive) || c.IsCode(Monster.Guitarna))
                return false;
            if (c.Attack < 2200)
                return true;
            return Enemy.MonsterZone.IsExistingMatchingCard(oc => {
                int val = GetBattleValue(oc);
                return val >= c.Attack && val < boosted;
            });
        }

        private bool TSGuitarna()
        {
            // doesn't take into account continuous atk reduction
            int boosted = 2200 + Enemy.GetMonsterCount() * 300;
            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => {
                return IsGuitarnaFodderable(c, boosted);
            });
            return TributeFodder.Count > 1;
        }

        private bool IsGiftaristFodderable(ClientCard c, int boosted)
        {
            if (c.IsCode(Monster.CanDLive) || c.IsCode(Monster.Giftarist))
                return false;
            if (c.Attack < 2400)
                return true;
            return Enemy.MonsterZone.IsExistingMatchingCard(oc => {
                int val = GetBattleValue(oc);
                return val >= c.Attack && val < boosted;
            });
        }

        private bool TSGiftarist()
        {
            // doesn't take into account continuous atk reduction
            int boosted = 2400;
            if (Bot.Deck.Count > 0)
            {
                int otherpsychics = Bot.MonsterZone.GetMatchingCardsCount(IsPsychic)
                    + Bot.Hand.GetMatchingCardsCount(c => IsLowLevel(c) && IsPsychic(c));
                boosted += (Math.Min(2, otherpsychics - 2) * 400);
            }
            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => {
                return IsGiftaristFodderable(c, boosted);
            });
            return TributeFodder.Count > 1;
        }

        // revive any Normal Monster just to recover Fusion to summon Amusi Howling Performer
        private bool FetchFusionForHowlingPerformer()
        {
            return Bot.HasInGraveyard(Spell.Fusion)
                && Bot.HasInMonstersZone(Monster.AmusiPerformer)
                && Bot.HasInMonstersZone(Monster.HowlingBird);
        }

        // Revive Howling Bird using Star Restart to be used as fusion material
        private bool ReviveMateriableHowlingBird()
        {
            if (!Bot.HasInGraveyard(Monster.HowlingBird)
                || !Bot.HasInMonstersZone(Monster.AmusiPerformer))
                return false;

            // exclude the Star Restart to be activated (if activating from the hand)
            IList<ClientCard> cost = new List<ClientCard>(Bot.Hand);
            if (cost.Contains(Card))
                cost.Remove(Card);

            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // don't use 1 Fusion in hand as cost
                if (Bot.HasInHand(Spell.Fusion))
                    cost.Remove(FirstMatch(cost, Spell.Fusion));
                else
                    return false;
            }

            if (cost.Count < 1)
                return false;

            AI.SelectCard(GetStarRestartCost(cost));
            AI.SelectNextCard(Monster.HowlingBird);
            return true;
        }

        // Normal Summon Howling Bird to be used immediately as fusion material
        private bool NSMateriableHowlingBird()
        {
            if (!Bot.HasInMonstersZone(Monster.AmusiPerformer))
                return false;

            // everything is ready and we can fuse immediately after
            if (HasUsableSpell(Spell.Fusion))
                return true;

            // there's a recoverable Fusion in the GY
            // or we can revive a normal monster to recover it
            return Bot.GetMonsterCount() < 1 && Bot.HasInGraveyard(Spell.Fusion)
                && Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                && HasUsableRestart(handexc: 1);
        }

        // TODO: more complex check to predict final ATK, and when to prioritize each monster
        private bool CanDLiveRevive()
        {
            if (ActivateDescription != Util.GetStringId(Monster.CanDLive, 1)
                || Bot.GetMonsterCount() > 2
                || !Bot.Graveyard.IsExistingMatchingCard(IsLevel7))
                return false;

            if (CountUnusedMonsters(Monster.CanDLive) < 2)
            {
                // if we can't beat a monster even with Guitarna's boost,
                // we could use the other effect instead
                int atk = Card.Attack;
                if (Bot.HasInGraveyard(Monster.Guitarna) || HasUnusedMonster(Monster.Guitarna))
                    atk += Enemy.GetMonsterCount() * 300;
                if (Enemy.MonsterZone.IsExistingMatchingCard(c => {
                    return c.IsAttack() && c.Attack >= atk;
                }))
                    return false;
            }

            AI.SelectCard(
                Monster.Guitarna,
                Monster.CurePurple,
                Monster.Giftarist,
                Monster.HowlingPerformer
            );
            return true;
        }

        // tribute a Level 7 if we can revive it later with Can:D Live
        private bool TSHighLevelUsingRevivableLevel7()
        {
            TributeFodder = null;
            if (IsLowLevel(Card) || !HasUnusedMonster(Monster.CanDLive)
                || Bot.MonsterZone.GetCardCount(Monster.CanDLive) != 1
                || !Bot.MonsterZone.IsExistingMatchingCard(IsLevel7))
                return false;

            if (Card.IsCode(Monster.Phickup))
            {
                if (Bot.GetMonsterCount() > 2)
                    return false;
                TributeFodder = Bot.MonsterZone.GetMatchingCards(IsLevel7);
                return true;
            }

            if (Bot.GetMonsterCount() < 3)
                return false;
            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => !c.IsCode(Monster.CanDLive));
            return true;
        }

        // Tribute Summon a Level 7 which will be tributed then revived by Can:D Live
        private bool TSRevivableLevel7()
        {
            TributeFodder = null;
            if (Card.Level != 7 || !HasUnusedMonster(Monster.CanDLive)
                || Bot.GetMonsterCount() < 3
                || Bot.MonsterZone.GetCardCount(Monster.CanDLive) != 1)
                return false;

            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => !c.IsCode(Monster.CanDLive));
            if (Bot.HasInHand(Monster.Phickup))
                return true;

            if (Bot.Hand.GetMatchingCardsCount(IsLevel7) > 1
                && Bot.Hand.IsExistingMatchingCard(IsLowLevel))
                return true;

            TributeFodder = null;
            return false;
        }

        // Normal Summon a low level monster to be tributed along with
        // a Level 7 monster that will be revived by Can:D Live
        private bool NSExtraTributeAlongRevivableLevel7()
        {
            return IsLowLevel(Card) && HasUnusedMonster(Monster.CanDLive)
                && Bot.MonsterZone.GetCardCount(Monster.CanDLive) == 1
                && Bot.Hand.IsExistingMatchingCard(IsLevel7)
                && Bot.MonsterZone.IsExistingMatchingCard(IsLevel7);
        }

        // Normal Summon a low level monster to be tributed for a Level 7 monster,
        // which will later be tributed then revived by Can:D Live
        private bool NSTributeForRevivableLevel7()
        {
            if (!IsLowLevel(Card) || !HasUnusedMonster(Monster.CanDLive)
                || Bot.MonsterZone.GetCardCount(Monster.CanDLive) != 1
                || !Bot.Hand.IsExistingMatchingCard(IsLevel7))
                return false;

            // number of other low-level monsters that can be tributed in the hand
            int otherfodders = Bot.Hand.GetMatchingCardsCount(IsLowLevel) - 1;
            if (Bot.GetMonsterCount() < 2)
            {
                if (otherfodders < 1)
                    return false;
                otherfodders--;
            }

            if (Bot.HasInHand(Monster.Phickup))
                return true;
            return otherfodders > 0 && Bot.Hand.GetMatchingCardsCount(IsLevel7) > 1;
        }

        // Normal Summon Howling Bird to be used immediately as fusion material
        private bool NSMateriableAmusiPerformer()
        {
            int costexc = 1;
            int mcount = Bot.GetMonsterCount();
            bool mustNormalPerf = false;
            if (!Bot.HasInMonstersZone(Monster.AmusiPerformer))
            {
                if (Bot.HasInHand(Monster.AmusiPerformer) && mcount < 2)
                {
                    mustNormalPerf = true;
                    costexc++;
                }
                else
                    return false;
            }

            // everything is ready and we can fuse immediately after
            if (HasUsableSpell(Spell.Fusion) && (!mustNormalPerf || mcount < 2))
                return true;

            // there's a recoverable Fusion in the GY
            // or we can revive a normal monster to recover it
            return Bot.GetMonsterCount() < 1 && Bot.HasInGraveyard(Spell.Fusion)
                && Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                && HasUsableRestart(handexc: costexc);
        }

        private bool CurePurpleDraw()
        {
            if (Bot.Deck.Count < 7)
                return false;

            IList<ClientCard> choices = Bot.Hand.GetMatchingCards(c => !c.IsMonster());

            ClientCard reserve = FirstMatch(choices, Spell.Restart);
            // reserve a Star Restart for later Fusions
            if (reserve != null)
            {
                if (!Bot.HasInSpellZone(Spell.Restart))
                    choices.Remove(reserve);
            }
            else
            {
                // reserve a Fusion if we can't recover it
                reserve = FirstMatch(choices, Spell.Fusion);
                if (reserve != null
                    && !Bot.HasInSpellZone(Spell.Restart)
                    && !Bot.HasInSpellZone(Spell.Fusion))
                    choices.Remove(reserve);
            }

            // reserve Jam:P Set if we need to remove a monster
            reserve = FirstMatch(choices, Spell.JamPSet);
            if (reserve != null && !Bot.HasInSpellZone(Spell.JamPSet)
                && Enemy.MonsterZone.IsExistingMatchingCard(c => {
                    return c.Level < 9 && GetBattleValue(c) >= Util.GetBestAttack(Bot);
                }))
                choices.Remove(reserve);

            // reserve Comeback! Feelshock!! for recovery
            reserve = FirstMatch(choices, Trap.ComebackFeelshock);
            if (reserve != null && !Bot.HasInSpellZone(Trap.ComebackFeelshock))
                choices.Remove(reserve);

            if (choices.Count < 1)
                return false;

            if (choices.Count < 4)
            {
                AI.SelectCard(choices);
                return true;
            }

            IList<ClientCard> select = new List<ClientCard>();

            // prioritize spare medicines
            IList<ClientCard> blue =
                choices.GetMatchingCards(c => c.IsCode(Spell.BlueMedicine));
            foreach (ClientCard c in blue)
            {
                if (select.Count > 2)
                    break;
                select.Add(c);
            }
            IList<ClientCard> red = choices.GetMatchingCards(c => c.IsCode(Spell.RedMedicine));
            foreach (ClientCard c in red)
            {
                if (select.Count > 2)
                    break;
                choices.Add(c);
            }

            ClientCard match = null;
            foreach (int id in HandFodderFallbackPriority)
            {
                if (select.Count > 2)
                    break;
                match = FirstMatch(choices, id);
                if (match == null
                    || (Bot.SpellZone.GetCardCount(id) + choices.GetCardCount(id)) < 2)
                    continue;
                select.Add(match);
            }
            while (select.Count < 3)
            {
                int ct = select.Count;
                foreach (int id in HandFodderFallbackPriority)
                {
                    if (select.Count > 2)
                        break;
                    match = FirstMatch(choices, id, select);
                    if (match == null)
                        continue;
                    select.Add(match);
                }
                // avoid infinite loops if no card is added
                if (ct == select.Count)
                    break;
            }

            AI.SelectCard(select);
            return true;
        }

        private bool LateMedicine()
        {
            return Card.IsCode(Spell.RedMedicine) || Card.IsCode(Spell.BlueMedicine);
        }

        // Normal Summon Chemical Cure Red/Blue
        // handled in one function to have some randomness
        private bool NSCureMisc()
        {
            int mcount = Bot.GetMonsterCount();
            int redmed = CountRetrievableMedicines(Monster.CureRed);
            int bluemed = CountRetrievableMedicines(Monster.CureBlue);
            if (Card.IsCode(Monster.CureRed))
            {
                if (mcount < 2 || Enemy.GetMonsterCount() == 0
                    || !Bot.HasInHand(Monster.CureBlue))
                    return true;
                return redmed > bluemed;
            }
            if (Card.IsCode(Monster.CureBlue))
            {
                if (mcount < 2 || !Bot.HasInHand(Monster.CureRed))
                    return true;
                return bluemed > redmed;
            }
            return false;
        }

        // TODO: check if we can reduce the opponent's LP to 0 in this turn
        private bool CanDLiveReduce()
        {
            if (ActivateDescription != Util.GetStringId(Monster.CanDLive, 0)
                || Bot.LifePoints <= 2000
                || !Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack()))
                return false;

            int best = Util.GetBestAttack(Bot);
            if (HasUnusedMonster(Monster.Guitarna))
                best += Enemy.GetMonsterCount() * 300;

            return best <= Util.GetBestAttack(Enemy);
        }

        private bool CanDLiveReduceFallback()
        {
            if (ActivateDescription != Util.GetStringId(Monster.CanDLive, 0)
                || Bot.LifePoints <= 2000
                || !Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack()))
                return false;

            if (Util.GetTotalAttackingMonsterAttack(0) >= Enemy.LifePoints
                && !Enemy.MonsterZone.IsExistingMatchingCard(c => !c.IsAttack()))
                return true;
            int mcount = Bot.GetMonsterCount();
            if (mcount < 3 && Bot.Graveyard.IsExistingMatchingCard(IsLevel7))
                return false;

            return mcount > 2 && Bot.MonsterZone.IsExistingMatchingCard(c => {
                return c.IsAttack() && Util.IsAllEnemyBetterThanValue(c.Attack, true);
            });
        }

        private bool CanDLiveReviveFallback()
        {
            if (Bot.Graveyard.IsExistingMatchingCard(IsLevel7) && Bot.GetMonsterCount() < 3)
            {
                AI.SelectCard(
                    Monster.Guitarna,
                    Monster.CurePurple,
                    Monster.Giftarist,
                    Monster.HowlingPerformer
                );
            }
            return ActivateDescription == Util.GetStringId(Monster.CanDLive, 1);
        }

        // TODO: predict if remaining enemy LP after battle is below 800
        private bool NSAmusiPerformerMisc()
        {
            if (Bot.LifePoints - Enemy.LifePoints > 2500)
                return true;
            if (Duel.Turn == 1)
                return false;
            return Enemy.LifePoints <= 800 || Enemy.GetMonsterCount() < 1;
        }

        private bool TSPhickupMisc()
        {
            // do not tribute high level monsters
            TributeFodder = Bot.MonsterZone.GetMatchingCards(IsLowLevel);
            if (TributeFodder.Count < 1)
                return false;

            // we need something to use as cost for Jam:P Set
            if (HasUsableSpell(Spell.JamPSet)
                && !Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                && TributeFodder.IsExistingMatchingCard(IsNormal))
            {
                TributeFodder = TributeFodder.GetMatchingCards(IsNormal);
                return true;
            }

            // our hand is too clogged, or we have Comeback! Feelshock!! for recovery
            return Bot.Hand.GetCardCount(Monster.Phickup) > 1
                || Bot.Hand.GetMatchingCardsCount(IsLevel7) > 1
                || Bot.HasInHandOrInSpellZone(Trap.ComebackFeelshock);
        }

        private bool NSPhickupFodderMisc()
        {
            int phickup = Bot.Hand.GetCardCount(Monster.Phickup);
            if (!IsLowLevel(Card) || phickup < 1)
                return false;
            return phickup > 1 || Bot.Hand.GetMatchingCardsCount(IsLevel7) > 1
                || Bot.HasInHandOrInSpellZone(Trap.ComebackFeelshock);
        }

        // TODO: predict more battle outcomes, not just the best monsters
        private bool NSLowlevelMisc()
        {
            if (Duel.Turn < 1 ||
                (!IsLowLevel(Card) && Card.Location != CardLocation.MonsterZone))
                return false;

            IList<ClientCard> m = Bot.GetMonsters();
            IList<ClientCard> o = Enemy.GetMonsters();

            // we can boost the Giftarist's ATK to beat a higher ATK monster
            if (HasUnusedMonster(Monster.Giftarist) && IsPsychic(Card))
            {
                ClientCard gift = FirstMatch(m, Monster.Giftarist);
                int gatk = gift.Attack;
                if (m.IsExistingMatchingCard(c => IsPsychic(c) && c != m && c != Card))
                    gatk += 400;
                if (o.IsExistingMatchingCard(c => {
                    int val = GetBattleValue(c);
                    return val >= gatk && val <= gatk + 400;
                }))
                    return true;
            }

            int atk = Card.Attack;
            // we control Guitarna and we can boost the monster's ATK to beat one opponent
            if (HasUnusedMonster(Monster.Guitarna))
                atk += (300 * o.Count);

            if (atk <= 100)
                return false;
            return o.Count < 1 || o.IsExistingMatchingCard(c => GetBattleValue(c) <= atk);
        }

        private bool ReposForBoost()
        {
            return Card.IsFacedown() && NSLowlevelMisc();
        }

        private bool FinalRepos()
        {
            return Card.Attack > 100 && DefaultMonsterRepos();
        }

        private bool DefensiveMonsterSet()
        {
            // try to reserve Can:D
            if (Card.IsCode(Monster.Candy))
            {
                if (Bot.Hand.GetCardCount(Monster.Candy) > 1)
                    return true;
                if (Bot.Hand.GetMatchingCardsCount(IsLowLevel) > 1)
                    return false;
                return Bot.GetMonsterCount() < 1 || HasUsableSpell(Spell.Restart);
            }
            return IsLowLevel(Card);
        }

        // TODO: predict more battle outcomes, not just the best attackers
        private bool GuitarnaBoost()
        {
            int best = Util.GetBestAttack(Bot);
            if (!Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack())
                && !Enemy.MonsterZone.IsExistingMatchingCard(c => c.Defense >= best))
                return false;

            if (Bot.LifePoints >= 2000)
                return true;
            int opp = Util.GetBestPower(Enemy);
            return (best <= opp) && ((best + 300 * Enemy.GetMonsterCount()) > opp);
        }

        // TODO: decide which monsters to shuffle from the GY if using Can:D as cost
        private bool JamPSetActivate()
        {
            IList<ClientCard> spin = Enemy.MonsterZone.GetMatchingCards(c => {
                return c.IsFaceup() && c.Level < 9;
            });

            int mcount = Bot.GetMonsterCount();
            int worst = 0;
            if (mcount > 1)
            {
                worst = Util.GetWorstBotMonster().Attack;
                if (Enemy.MonsterZone.GetMatchingCardsCount(c => !c.IsFaceup()) >= mcount)
                    worst = Util.GetBestAttack(Bot);
            }

            IList<ClientCard> threats = spin.GetMatchingCards(c => GetBattleValue(c) >= worst);
            if (mcount > spin.Count || (threats.Count > 0 && mcount == spin.Count))
                spin = threats;
            if (spin.Count < 1)
                return false;

            if (Bot.Graveyard.GetCardCount(Monster.Candy) > 1
                && Enemy.Graveyard.GetMatchingCardsCount(c => c.IsMonster()) > 0)
                AI.SelectCard(Monster.Candy);
            else
                AI.SelectCard(Monster.HowlingBird, Monster.PeaceHolder, Monster.Candy);

            int max = -1;
            ClientCard select = null;
            foreach (ClientCard c in spin)
            {
                int val = GetBattleValue(c);
                if (val > max || (select != null && val == max && c.Level > select.Level))
                {
                    select = c;
                    max = val;
                }
            }
            if (select != null)
                AI.SelectNextCard(select);
            return true;
        }

        // TODO: predict if remaining enemy LP after battle is below 800
        private bool AmusiPerformerBurn()
        {
            return Enemy.LifePoints <= 300 || Bot.LifePoints - Enemy.LifePoints > 2500;
        }
        
        private bool FeelshockSet()
        {
            return !Bot.HasInSpellZone(Trap.ComebackFeelshock) && SpellTrapSet();
        }

        private bool RetrievableFusionSet()
        {
            return HasUsableRestart() && SpellTrapSet();
        }

        private bool SpellTrapSet()
        {
            if (Card.IsCode(Spell.Restart))
                return false;
            if (Bot.GetSpellCountWithoutField() > 2 && Bot.Hand.Count < 3)
                return false;

            int count = Bot.Hand.GetCardCount(Card.Id) - Bot.SpellZone.GetCardCount(Card.Id);
            return !Bot.Hand.IsExistingMatchingCard(c => {
                return !c.IsMonster() &&
                    Bot.Hand.GetCardCount(c.Id) - Bot.SpellZone.GetCardCount(c.Id) > count;
            });
        }

        private bool RestartSet()
        {
            return Bot.Hand.Count > 2 || Bot.Hand.GetCardCount(Spell.Restart) > 1;
        }
    }
}
