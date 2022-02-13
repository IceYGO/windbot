using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("[ImaginaryArk]", "AI_[ImaginaryArk]")]
    public class ImaginaryArkExecutor : DefaultExecutor
    {
        public class Monster
        {
            public const int
                // main deck monsters
                Grace       = 160204007, // Gracesaurus
                Painter     = 160204008, // Serpainter
                Dancer      = 160204009, // Sword Dancer
                Juggler     = 160204010, // Magic Juggler
                Actor       = 160204006, // Imaginary Actor
                Sewkyrie    = 160007001, // Valkyrian Sewkyrie
                Yamiruler   = 160001029, // Yamiruler the Dark Delayer
                // fusions
                Yamiterasu  = 160007036, // Yamiterasu the Divine Delayer
                Asura       = 160204001, // Metallion Asurastar
                Vritra      = 160204002, // Metallion Vritrastar
                Eracle      = 160204003, // Metallion Eraclestar
                Ladon       = 160204004; // Metallion Ladonstar
        }

        public class Spell
        {
            public const int
                TCB     = 160004039, // Type Change Beam
                Turbo   = 160204011, // Imaginary Ark Turbo
                Restart = 160204045, // Star Restart
                Charity = 160204049, // Graceful Charity
                Fusion  = 160204050, // Fusion
                Tower   = 160204012; // Imaginary Ark Tower
        }

        public class Trap
        {
            public const int
                Turnback = 160204014; // Imaginary Ark Turnback
        }

        public ImaginaryArkExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, Trap.Turnback, TurnbackActivate);
            AddExecutor(ExecutorType.Repos, ReposFusionMaterial);

            // Fusion Summon a Metallion monster (no prioritization for now)
            AddExecutor(ExecutorType.Activate, Spell.Fusion, FusionMetallion);
            AddExecutor(ExecutorType.Summon, NSMetallionMaterial);
            AddExecutor(ExecutorType.Activate, Spell.TCB, RevealMaterialBeforeSummon);
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMetallionMaterial);
            AddExecutor(ExecutorType.Activate, Spell.Restart, FetchFusionForMetallion);

            // Fusion Summon Yamiterasu the Divine Delayer
            AddExecutor(ExecutorType.Activate, Spell.Fusion);
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMateriableSewky);
            AddExecutor(ExecutorType.Activate, Spell.Restart, FetchFusionForYami);
            AddExecutor(ExecutorType.Summon, Monster.Sewkyrie, NSMateriableSewky);
            AddExecutor(ExecutorType.Summon, Monster.Yamiruler, TSMateriableYamiUsingNonSewky);
            AddExecutor(ExecutorType.Summon, Monster.Yamiruler, TSMateriableYamiUsingRevivableSewky);
            AddExecutor(ExecutorType.Summon, NSNonSewkyAsMateriableYamiFodder);
            AddExecutor(ExecutorType.Summon, Monster.Sewkyrie, NSSewkyAsMateriableYamiFodder);
            AddExecutor(ExecutorType.Activate, Spell.Restart, ReviveMateriableYamiFodder);

            // Tribute Summon Yamiterasu
            AddExecutor(ExecutorType.Summon, NSMiscYamiFodder);
            AddExecutor(ExecutorType.Summon, Monster.Yamiruler, TSMiscYami);

            AddExecutor(ExecutorType.Activate, Spell.Charity);
            AddExecutor(ExecutorType.Activate, Spell.TCB, TCBActivate);
            AddExecutor(ExecutorType.Activate, Monster.Eracle, EracleFieldSpin);
            AddExecutor(ExecutorType.Activate, Monster.Vritra, VritraPop);
            AddExecutor(ExecutorType.Activate, Monster.Asura, AsuraBoost);
            AddExecutor(ExecutorType.Activate, Monster.Ladon, LadonReduceAll);
            AddExecutor(ExecutorType.Activate, Monster.Vritra, VritraRepos);
            AddExecutor(ExecutorType.Activate, Monster.Asura, AsuraPop);
            AddExecutor(ExecutorType.Activate, Monster.Eracle, EracleGraveSpin);
            AddExecutor(ExecutorType.Activate, Monster.Ladon, LadonReduceOne);
            AddExecutor(ExecutorType.Activate, Spell.Turbo, TurboActivate);
            AddExecutor(ExecutorType.Activate, Spell.Tower);
            AddExecutor(ExecutorType.Activate, Monster.Yamiruler, YamirulerActivate);
            AddExecutor(ExecutorType.Activate, Monster.Yamiterasu, YamiterasuRepos);
            AddExecutor(ExecutorType.Activate, Monster.Yamiterasu, YamiterasuFloodgate);
            AddExecutor(ExecutorType.Summon, NSLowlevelMisc);
            AddExecutor(ExecutorType.Repos, FinalRepos);
            AddExecutor(ExecutorType.MonsterSet, DefensiveMonsterSet);

            // Set Spell/Traps
            AddExecutor(ExecutorType.SpellSet, Trap.Turnback, FeelshockSet);
            AddExecutor(ExecutorType.SpellSet, Spell.Fusion, RetrievableFusionSet);
            AddExecutor(ExecutorType.SpellSet, SpellTrapSet);
            AddExecutor(ExecutorType.SpellSet, Spell.Restart, RestartSet);

        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override int OnSelectPlace(
            long cardId,
            int player,
            CardLocation location,
            int available
        )
        {
            List<int> valid = new List<int>();
            for (int z = Zones.z1; z <= available; z <<= 1)
            {
                if ((z & available) > 0)
                    valid.Add(z);
            }
            return valid.Count > 0 ? valid[Program.Rand.Next(valid.Count)] : available;
        }

        public override bool OnPreActivate(ClientCard card)
        {
            return !card.HasType(CardType.Spell)
                || card.IsCode(Spell.Restart)
                || card.Location == CardLocation.Hand
                || Bot.GetSpellCountWithoutField() > 2
                || !Bot.HasInHand(card.Id);
        }

        public override bool OnSelectYesNo(long desc)
        {
            return Duel.LastChainPlayer == 0 || base.OnSelectYesNo(desc);
        }

        public override IList<ClientCard> OnSelectCard(
            IList<ClientCard> cards,
            int min,
            int max,
            long hint,
            bool cancelable
        )
        {
            if (min == max && max == cards.Count)
                return cards;

            if (hint == HintMsg.Release)
            {
                List<ClientCard> choices = cards.ToList();
                if (TributeFodder != null && TributeFodder.Count > 0)
                {
                    TributeFodder = TributeFodder.GetMatchingCards(c => cards.Contains(c));
                    if (TributeFodder.Count == min)
                        return TributeFodder;
                    if (TributeFodder.Count > 0)
                        choices = TributeFodder.ToList();
                }
                choices.Sort(CardContainer.CompareCardAttack);
                choices.Reverse();

                return Util.CheckSelectCount(choices, cards, min, min);
            }

            ClientCard act = Util.GetLastChainCard();
            if (act == null || Duel.LastChainPlayer != 0)
                return null;

            // summon random fusion if we can summon more than 1,
            // or try to see if the opponent controls a corresponding type
            if (act.IsCode(Spell.Fusion) && cards[0].Location == CardLocation.Extra)
            {
                if (cards.GetCardCount(cards[0].Id) == cards.Count)
                    return null;
                int highest = 0;
                List<ClientCard> select = new List<ClientCard>();
                foreach (KeyValuePair<int, CardRace> pair in TargetRaces)
                {
                    if (!cards.ContainsCardWithId(pair.Key))
                        continue;
                    int targets =
                        Enemy.MonsterZone.GetMatchingCardsCount(c => c.HasRace(pair.Value));
                    if (targets > highest)
                    {
                        select.Clear();
                        highest = targets;
                    }
                    if (targets == highest)
                        select.AddRange(cards.GetMatchingCards(c => c.IsCode(pair.Key)));
                }
                return new List<ClientCard>() { select[Program.Rand.Next(select.Count)] };
            }

            if (act.IsCode(Spell.Charity))
                return Util.CheckSelectCount(GetHandFodder(2), cards, min, min);

            if (act.IsCode(Spell.TCB) && cards[0].Location != CardLocation.Hand)
                return cards.GetMatchingCards(c => c.Controller == 1);

            return null;
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

        private bool IsCyborg(ClientCard c)
        {
            // workaround for now (c.Race & 0x20000000 != 0 doesn't work for some reason)
            foreach (KeyValuePair<int, CardRace> pair in TargetRaces)
            {
                if (c.IsCode(pair.Key))
                    return true;
            }
            return false;
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

        private readonly int[] HandFodderPriority = {
            Monster.Sewkyrie,
            Spell.Tower,
            Spell.TCB,
            Spell.Turbo,
            Monster.Yamiruler,
            Monster.Grace,
            Monster.Painter,
            Monster.Juggler,
            Monster.Dancer,
            Trap.Turnback,
            Monster.Actor,
            Spell.Fusion,
            Spell.Restart
        };

        private IList<ClientCard> GetHandFodder(int count)
        {
            IList<ClientCard> hand = new List<ClientCard>(Bot.Hand);
            IList<ClientCard> select = new List<ClientCard>();
            // try to get rid of duplicates
            foreach (int id in HandFodderPriority)
            {
                ClientCard match = FirstMatch(hand, id);
                if (match == null)
                    continue;
                int ct = hand.GetCardCount(id) + Bot.SpellZone.GetCardCount(id);
                if (ct < 2)
                    continue;
                select.Add(match);
                if (select.Count == count)
                    return select;
                hand.Remove(match);
            }
            // fallback importance
            while (select.Count < count)
            {
                int prev = select.Count;
                foreach (int id in HandFodderPriority)
                {
                    ClientCard match = FirstMatch(hand, id);
                    if (match == null)
                        continue;
                    select.Add(match);
                    if (select.Count == count)
                        return select;
                    hand.Remove(match);
                }
                if (prev == select.Count)
                    break;
            }
            // safeguard
            while (select.Count < count)
            {
                select.Add(hand[0]);
                hand.RemoveAt(0);
            }
            return select;
        }

        private bool TurnbackActivate()
        {
            IList<ClientCard> cyborg = Bot.Graveyard.GetMatchingCards(IsCyborg);
            if (cyborg.Count > 0)
            {
                AI.SelectCard(cyborg[Program.Rand.Next(cyborg.Count)]);
                AI.SelectPosition(CardPosition.FaceUpAttack);
                return true;
            }

            bool fusion = HasUsableSpell(Spell.Fusion);
            bool restart = HasUsableSpell(Spell.Restart);
            bool gyfusion = Bot.HasInGraveyard(Spell.Fusion);
            int lowatk = Enemy.MonsterZone.GetMatchingCardsCount(c => {
                return c.IsAttack() && c.Attack <= 1400;
            });
            bool haslowatk = Enemy.GetMonsterCount() - lowatk <= lowatk;
            if (Bot.HasInGraveyard(StarMaterial)
                && ((Bot.HasInHand(Monster.Actor) && fusion)
                || (Bot.HasInGraveyard(Monster.Actor) && restart && (fusion || gyfusion))))
            {
                AI.SelectCard(StarMaterial);
                if (haslowatk)
                    AI.SelectPosition(CardPosition.FaceUpAttack);
                else
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            if (Bot.HasInGraveyard(Monster.Actor)
                && ((Bot.HasInHand(StarMaterial) && fusion)
                || (Bot.HasInGraveyard(StarMaterial) && restart && (fusion || gyfusion))))
            {
                AI.SelectCard(Monster.Actor);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }

            if (Bot.GetSpellCountWithoutField() < 3 && Bot.GetMonsterCount() > 0)
                return false;
            if (haslowatk && Bot.HasInGraveyard(StarMaterial))
            {
                AI.SelectCard(StarMaterial);
                AI.SelectPosition(CardPosition.FaceUpAttack);
            }
            else
            {
                AI.SelectCard(Monster.Actor);
                AI.SelectPosition(CardPosition.FaceUpDefence);
            }
            
            return true;
        }

        private readonly int[] StarMaterial = {
            Monster.Grace,
            Monster.Painter,
            Monster.Dancer,
            Monster.Juggler
        };

        private bool ReposFusionMaterial()
        {
            if (Card.IsFaceup() || !HasUsableSpell(Spell.Fusion))
                return false;
            if (Card.IsCode(Monster.Sewkyrie))
                return Bot.HasInMonstersZone(Monster.Yamiruler);
            if (Card.IsCode(Monster.Yamiruler))
                return Bot.HasInMonstersZone(Monster.Sewkyrie);
            if (Card.IsCode(Monster.Actor))
                return Bot.HasInMonstersZone(StarMaterial);
            return Card.IsCode(StarMaterial) && Bot.HasInMonstersZone(Monster.Actor);
        }

        // make sure we're summoning a Metallion monster
        private bool FusionMetallion()
        {
            return Bot.HasInMonstersZone(Monster.Actor) && Bot.HasInMonstersZone(StarMaterial);
        }

        private bool ReviveMetallionMaterial()
        {
            bool actor = false, gypair = false, fieldpair = false, handpair = false;
            if (Bot.HasInGraveyard(Monster.Actor))
            {
                actor = true;
                gypair = Bot.HasInGraveyard(StarMaterial);
                fieldpair = Bot.HasInMonstersZone(StarMaterial);
                handpair = Bot.HasInHand(StarMaterial);
            }
            else if (Bot.HasInGraveyard(StarMaterial))
            {
                gypair = Bot.HasInGraveyard(Monster.Actor);
                fieldpair = Bot.HasInMonstersZone(Monster.Actor);
                handpair = Bot.HasInHand(Monster.Actor);
            }
            else
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

            if (!fieldpair)
            {
                if (Bot.GetMonsterCount() > 1)
                    return false;
                if (handpair)
                {
                    if (actor)
                        cost.Remove(FirstMatch(cost, c => c.IsCode(StarMaterial)));
                    else
                        cost.Remove(FirstMatch(cost, Monster.Actor));
                }
                else if (gypair)
                {
                    ClientCard otherrestart = FirstMatch(Bot.GetSpells(), Spell.Restart, Card);
                    if (otherrestart == null)
                        otherrestart = FirstMatch(cost, Spell.Restart, Card);
                    if (otherrestart == null)
                        return false;
                    cost.Remove(otherrestart);
                }
                else
                    return false;
            }

            if (cost.Count < 1)
                return false;

            AI.SelectCard(GetStarRestartCost(cost));
            if (actor)
                AI.SelectNextCard(Monster.Actor);
            else
                AI.SelectNextCard(StarMaterial);
            return true;
        }

        private int CostExcIfNSMetallionMaterial()
        {
            bool gypair = false, fieldpair = false, handpair = false;
            if (Card.IsCode(Monster.Actor))
            {
                if (Bot.HasInMonstersZone(Monster.Actor))
                    return -1;
                gypair = Bot.HasInGraveyard(StarMaterial);
                fieldpair = Bot.HasInMonstersZone(StarMaterial);
                handpair = Bot.HasInHand(StarMaterial);
            }
            else if (Card.IsCode(StarMaterial))
            {
                gypair = Bot.HasInGraveyard(Monster.Actor);
                fieldpair = Bot.HasInMonstersZone(Monster.Actor);
                handpair = Bot.HasInHand(Monster.Actor);
            }
            else
                return -1;

            int costexc = 1;
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // don't use 1 Fusion in hand as cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return -1;
                costexc--;
            }

            int mcount = Bot.GetMonsterCount();

            if (!fieldpair)
            {
                if (mcount > 1)
                    return -1;
                if (gypair && HasUsableRestart(costexc))
                    return costexc;
                if (!handpair)
                    return -1;
                costexc++;
            }

            // everything is ready and we can fuse immediately after
            if (HasUsableSpell(Spell.Fusion))
                return 0;

            // there's a recoverable Fusion in the GY
            // or we can revive a normal monster to recover it
            if (mcount < 1 && Bot.HasInGraveyard(Spell.Fusion)
                && Bot.Graveyard.IsExistingMatchingCard(IsNormal)
                && HasUsableRestart(costexc))
                return costexc;

            return -1;
        }

        private ClientCard revealBeforeSummon = null;
        private bool NSMetallionMaterial()
        {
            revealBeforeSummon = null;
            int costexc = CostExcIfNSMetallionMaterial();
            if (costexc < 0)
                return false;

            if (Card.IsCode(Monster.Actor))
                return true;

            if (!Bot.HasInSpellZone(Spell.TCB))
            {
                if (!Bot.HasInHand(Spell.TCB))
                    return true;
                if (costexc > 0 && Bot.Hand.Count - costexc < 2)
                    return true;
            }

            if (Bot.Hand.GetMatchingCardsCount(c => c.Race == Card.Race) > 1
                || !Enemy.MonsterZone.IsExistingMatchingCard(c => c.Race != Card.Race))
                return true;

            revealBeforeSummon = Card;
            return false;
        }

        private bool RevealMaterialBeforeSummon()
        {
            if (revealBeforeSummon == null)
                return false;
            AI.SelectCard(revealBeforeSummon);
            revealBeforeSummon = null;
            return true;
        }

        private bool FetchFusionForMetallion()
        {
            return Bot.HasInGraveyard(Spell.Fusion) && FusionMetallion();
        }

        private ClientCard GetStarRestartCost(IList<ClientCard> choices)
        {
            // extra high level
            if (choices.GetMatchingCardsCount(c => !IsLowLevel(c)) > 1)
                return FirstMatch(choices, c => !IsLowLevel(c));

            // Fusion, if we can recover one from the GY
            ClientCard cost = FirstMatch(choices, Spell.Fusion);
            if (cost != null && Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
                return cost;

            // try to get rid of duplicates
            foreach (int id in HandFodderPriority)
            {
                cost = FirstMatch(choices, id);
                int count = choices.GetCardCount(id)
                    + Bot.SpellZone.GetCardCount(id)
                    + Bot.MonsterZone.GetCardCount(id);
                if (cost != null
                    && count + (IsNormal(cost) ? Bot.Graveyard.GetCardCount(id) : 0) > 1)
                    return cost;
            }
            // fallback importance
            foreach (int id in HandFodderPriority)
            {
                cost = FirstMatch(choices, id);
                if (cost != null)
                    return cost;
            }

            return choices[Program.Rand.Next(choices.Count)];
        }

        // revive any Normal Monster just to recover Fusion to summon Yamiterasu
        private bool FetchFusionForYami()
        {
            return Bot.HasInGraveyard(Spell.Fusion)
                && Bot.HasInMonstersZone(Monster.Yamiruler)
                && Bot.HasInMonstersZone(Monster.Sewkyrie);
        }

        // Revive Sewkyrie using Star Restart to be used as fusion material
        private bool ReviveMateriableSewky()
        {
            if (!Bot.HasInGraveyard(Monster.Sewkyrie)
                || !Bot.HasInMonstersZone(Monster.Yamiruler))
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
            AI.SelectNextCard(Monster.Sewkyrie);
            return true;
        }

        // Normal Summon Sewkyrie to be used immediately as fusion material
        private bool NSMateriableSewky()
        {
            if (!Bot.HasInMonstersZone(Monster.Yamiruler))
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
        // Tribute Summon Yamiterasu to be used as fusion material,
        // using non-Sewkyrie monsters as tribute (unless there's more than 1)
        private bool TSMateriableYamiUsingNonSewky()
        {
            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => !c.HasType(CardType.Fusion));
            if (TributeFodder.Count < 2)
                return false;

            // excluding the Yamiruler itself
            int costexc = 1;
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand if there are no others available
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            // there is a Sewkyrie in the GY that we can revive
            if (Bot.HasInGraveyard(Monster.Sewkyrie) && HasUsableRestart(costexc))
                return true;

            // reserve 1 copy of Sewkyrie in case we can fuse immediately after summoning.
            // Cases where we NEED to tribute Sewkyrie itself is handled in another function
            if (Bot.HasInMonstersZone(Monster.Sewkyrie))
            {
                if (TributeFodder.Count < 3)
                {
                    if (!Bot.HasInHand(Monster.Sewkyrie))
                        return false;
                    costexc++;
                }
                else
                    TributeFodder.Remove(FirstMatch(TributeFodder, Monster.Sewkyrie));
            }
            // the only available Sewkyrie is in hand, so exclude it as Restart cost
            // and make sure there's space to normal summon it later
            else if (Bot.HasInHand(Monster.Sewkyrie))
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
            if (Bot.Graveyard.IsExistingMatchingCard(IsNormal))
                return true;
            if (!TributeFodder.IsExistingMatchingCard(IsNormal))
                return false;
            if (TributeFodder.GetMatchingCardsCount(c => !IsNormal(c)) > 1)
                TributeFodder.Remove(FirstMatch(TributeFodder, c => !IsNormal(c)));
            return TributeFodder.Count > 1;
        }

        // Tribute Summon Yamiruler to be used as fusion material,
        // using a Sewky which can be revived using Star Restart
        private bool TSMateriableYamiUsingRevivableSewky()
        {
            TributeFodder = null;

            if (!Bot.HasInMonstersZone(Monster.Sewkyrie))
                return false;

            TributeFodder = Bot.MonsterZone.GetMatchingCards(c => !c.HasType(CardType.Fusion));
            if (TributeFodder.Count < 2)
                return false;

            // excluding the Yamiruler itself
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

            if (TributeFodder.GetMatchingCardsCount(c => !c.IsCode(Monster.Sewkyrie)) > 1)
            {
                TributeFodder.Remove(FirstMatch(TributeFodder, c => {
                    return !c.IsCode(Monster.Sewkyrie);
                }));
            }

            return TributeFodder.Count > 1;
        }

        // Normal Summon a low-level monster to be used as tribute for Yamiruler,
        // except Sewkyrie (unless there's another Sewkyrie available)
        private bool NSNonSewkyAsMateriableYamiFodder()
        {
            if (!IsLowLevel(Card) || !Bot.HasInHand(Monster.Yamiruler))
                return false;

            // excluding Psyphickupper and the fodder monster itself
            int costexc = 2;
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand if there are no others available
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            int mcount = Bot.GetMonsterCount();
            if (Bot.MonsterZone.GetMatchingCardsCount(c => !c.HasType(CardType.Fusion)) < 1)
            {
                if (mcount > 1 || Bot.Hand.GetMatchingCardsCount(IsLowLevel) < 1)
                    return false;
                costexc++;
            }

            // there's a Sewkyrie in the GY that we can revive
            if (Bot.HasInGraveyard(Monster.Sewkyrie) && HasUsableRestart(costexc))
                return true;

            bool onfield = Bot.HasInMonstersZone(Monster.Sewkyrie);
            if (!onfield)
            {
                // the only available Sewkyrie is in hand (if any)
                int onhand = Bot.Hand.GetCardCount(Monster.Sewkyrie);
                if (Card.IsCode(Monster.Sewkyrie))
                    onhand--;
                if (onhand < 0)
                    return false;
                costexc++;
            }

            // everything is ready
            if (HasUsableSpell(Spell.Fusion))
                return true;

            // we must be able to revive a normal monster from the GY to recover Fusion
            return (mcount < 3 || onfield) && HasUsableRestart(costexc)
                && (IsNormal(Card) || Bot.Graveyard.IsExistingMatchingCard(IsNormal));
        }

        // Normal Summon the only Sewkyrie in hand to be used as tribute
        // for Yamiruler, so we can revive it with Star Restart later
        private bool NSSewkyAsMateriableYamiFodder()
        {
            if (!Bot.HasInHand(Monster.Yamiruler))
                return false;

            // excluding the Yamiruler and Sewkyrie in hand
            int costexc = 2;
            // we must have Fusion ready or we can recover it
            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // exclude 1 Fusion in hand as Restart cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                costexc++;
            }

            if (Bot.MonsterZone.GetMatchingCardsCount(c => !c.HasType(CardType.Fusion)) < 1)
            {
                if (Bot.GetMonsterCount() > 1
                    || Bot.Hand.GetMatchingCardsCount(IsLowLevel) < 1)
                    return false;
                costexc++;
            }

            return HasUsableRestart(costexc);
        }

        // revive any normal monster to be tributed for Yamiruler
        // which will be used as fusion material later
        private bool ReviveMateriableYamiFodder()
        {
            if (!Bot.HasInHand(Monster.Yamiruler))
                return false;

            // exclude the Star Restart to be activated (if activating from the hand)
            IList<ClientCard> cost = Bot.Hand.ToList();
            cost.Remove(FirstMatch(cost, Monster.Yamiruler));
            if (cost.Contains(Card))
                cost.Remove(Card);

            if (!Bot.HasInSpellZoneOrInGraveyard(Spell.Fusion))
            {
                // don't use 1 Fusion in hand as cost
                if (!Bot.HasInHand(Spell.Fusion))
                    return false;
                cost.Remove(FirstMatch(cost, Spell.Fusion));
            }

            int mcount = Bot.GetMonsterCount();
            int fodders =
                Bot.MonsterZone.GetMatchingCardsCount(c => !c.HasType(CardType.Fusion));
            if (fodders < 1)
            {
                if (mcount > 1 || Bot.Hand.GetMatchingCardsCount(IsLowLevel) < 1)
                    return false;
                cost.Remove(FirstMatch(cost, IsLowLevel));
            }

            if (cost.Count < 1)
                return false;

            bool fg = Bot.HasInMonstersZoneOrInGraveyard(Monster.Sewkyrie);
            // there's a Sewkyrie in the GY and we have 2 available Star Restart
            if (fg || (Bot.HasInHand(Monster.Sewkyrie) && mcount < 2))
            {
                IList<ClientCard> ncost = new List<ClientCard>(cost);
                ClientCard otherrestart = FirstMatch(Bot.GetSpells(), Spell.Restart, Card);
                if (otherrestart == null)
                    otherrestart = FirstMatch(ncost, Spell.Restart, Card);
                if (otherrestart != null)
                {
                    if (ncost.Contains(otherrestart))
                        ncost.Remove(otherrestart);
                    if (!fg)
                        ncost.Remove(FirstMatch(cost, Monster.Sewkyrie));
                    // there must be another cost for the other Restart
                    if (ncost.Count > 1)
                    {
                        AI.SelectCard(ncost);
                        return true;
                    }
                }
            }

            if (!Bot.HasInMonstersZone(Monster.Sewkyrie))
            {
                // there must be an available Sewkyrie is in hand
                if (!Bot.Hand.ContainsCardWithId(Monster.Sewkyrie))
                    return false;
                cost.Remove(FirstMatch(cost, Monster.Sewkyrie));
            }

            if (cost.Count < 1)
                return false;

            AI.SelectCard(GetStarRestartCost(cost));
            return true;
        }

        private bool NSMiscYamiFodder()
        {
            if (!IsLowLevel(Card) || !Bot.HasInHand(Monster.Yamiruler))
                return false;

            int fodders = Bot.MonsterZone.GetMatchingCardsCount(IsLowLevel);
            if (fodders > 1)
                return false;
            return Bot.GetMonsterCount() < 2
                && (fodders + Bot.Hand.GetMatchingCardsCount(IsLowLevel)) > 1;
        }

        private bool TSMiscYami()
        {
            TributeFodder = Bot.MonsterZone.GetMatchingCards(IsLowLevel);
            return TributeFodder.Count > 1;
        }

        private bool TurboActivate()
        {
            int boost = 500;
            bool useactor = false;
            if (Bot.HasInGraveyard(Monster.Actor))
                boost = 1400;
            else if (Bot.HasInHand(Monster.Actor))
            {
                useactor = true;
                boost = 1400;
            }

            List<ClientCard> toboost = Bot.MonsterZone.GetMatchingCards(c => {
                return c.IsAttack() && c.HasAttribute(CardAttribute.Light)
                    && (c.HasType(CardType.Normal) || c.Level == 9);
            }).ToList();
            if (toboost.Count < 1)
                return false;
            toboost.Sort(CardContainer.CompareCardAttack);
            toboost.Reverse();

            bool hasAtk = Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack());
            bool hasDef = !Enemy.MonsterZone.IsExistingMatchingCard(c => !c.IsAttack());
            int remlp = Enemy.LifePoints - Util.GetTotalAttackingMonsterAttack(0);
            if (hasAtk)
                remlp += Util.GetTotalAttackingMonsterAttack(1);
            if ((boost == 1400 && (!hasDef || hasAtk)
                && toboost.IsExistingMatchingCard(IsCyborg))
                || (!hasDef && remlp > 0 && remlp <= boost))
            {
                if (useactor)
                    AI.SelectCard(Monster.Actor);
                else
                    AI.SelectCard(GetHandFodder(1));
                AI.SelectNextCard(toboost[0]);
            }

            toboost = toboost.GetMatchingCards(c => {
                return Enemy.MonsterZone.IsExistingMatchingCard(oc => {
                    int val = oc.IsAttack() ? oc.Attack : oc.Defense;
                    return val >= c.Attack && val <= c.Attack + boost;
                });
            }).ToList();
            if (toboost.Count < 1)
                return false;
            if (useactor)
                AI.SelectCard(Monster.Actor);
            else
                AI.SelectCard(GetHandFodder(1));
            AI.SelectNextCard(toboost[0]);
            return true;
        }

        private readonly Dictionary<int, CardRace> TargetRaces = new Dictionary<int, CardRace>()
        {
            { Monster.Eracle, CardRace.SpellCaster },
            { Monster.Vritra, CardRace.Dragon },
            { Monster.Ladon, CardRace.Dinosaur },
            { Monster.Asura, CardRace.Warrior }
        };

        private bool TCBActivate()
        {
            foreach (KeyValuePair<int, CardRace> pair in TargetRaces)
            {
                if (!Bot.HasInMonstersZone(pair.Key))
                    continue;
                ClientCard match = FirstMatch(Bot.Hand, c => c.HasRace(pair.Value));
                if (match == null)
                    continue;
                IList<ClientCard> targets = Enemy.MonsterZone.GetMatchingCards(c => {
                    return c.IsFaceup() && !c.HasRace(pair.Value);
                });
                if (pair.Key == Monster.Ladon)
                {
                    targets = targets.GetMatchingCards(c => c.IsAttack());
                    if (targets.Count <= Bot.GetMonsterCount())
                        continue;
                }
                else if (pair.Key == Monster.Vritra)
                {
                    if (targets.Count < Bot.MonsterZone.GetMatchingCardsCount(IsCyborg))
                        continue;
                }
                if (targets.Count < 1)
                    continue;
                AI.SelectCard(match);
                return true;
            }

            return false;
        }

        private bool AsuraBoost()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Asura, 0))
                return false;

            int fd = Enemy.MonsterZone.GetMatchingCardsCount(c => !c.IsFaceup())
                + Enemy.SpellZone.GetMatchingCardsCount(c => !c.IsFaceup());
            if (fd < 1)
                return true;

            int boost = Enemy.MonsterZone.GetMatchingCards(c => c.HasRace(CardRace.Warrior))
                .Sum(c => c.Attack);

            if (Enemy.MonsterZone.IsExistingMatchingCard(c => {
                int val = GetBattleValue(c);
                return val >= Card.Attack && val <= Card.Attack + boost;
            }))
                return true;

            return fd < 3 && Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack());
        }

        private bool AsuraPop()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Asura, 1))
                return false;

            IList<ClientCard> backrow = Enemy.SpellZone.GetMatchingCards(c => !c.IsFaceup());
            IList<ClientCard> monsters =
                Enemy.MonsterZone.GetMatchingCards(c => !c.IsFaceup());

            if (monsters.Count >= Bot.GetMonsterCount() || monsters.Count > backrow.Count)
                AI.SelectCard(monsters);
            else
                AI.SelectCard(backrow);
            return true;
        }

        private bool VritraPop()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Vritra, 0))
                return false;
            List<ClientCard> targets =
                Enemy.MonsterZone.GetMatchingCards(c => c.HasRace(CardRace.Dragon)).ToList();
            targets.Sort(CardContainer.CompareCardAttack);
            targets.Reverse();

            if (targets[0].Attack < Card.Attack
                && Enemy.MonsterZone.GetMatchingCardsCount(c => !c.IsAttack())
                >= Bot.MonsterZone.GetMatchingCardsCount(c => c.IsAttack()))
                return false;

            int tc = Bot.MonsterZone.GetMatchingCardsCount(IsCyborg);
            if (targets.Count > tc)
            {
                targets.Sort(CardContainer.CompareCardAttack);
                targets.Reverse();
                while (targets.Count > tc)
                    targets.RemoveAt(targets.Count - 1);
            }

            AI.SelectCard(targets);
            return true;
        }

        private bool VritraRepos()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Vritra, 1))
                return false;
            List<ClientCard> targets = Enemy.GetMonsters().ToList();
            targets.Sort(CardContainer.CompareCardAttack);
            targets.Reverse();

            int best = Util.GetBestAttack(Bot);
            foreach (ClientCard t in targets.GetMatchingCards(c => c.IsAttack()))
            {
                if (t.Attack >= best && t.Defense < best)
                {
                    AI.SelectCard(t);
                    return true;
                }
            }

            targets = targets.GetMatchingCards(c => !c.IsAttack()).ToList();
            if (targets.Count < 1)
                return false;

            AI.SelectCard(targets[targets.Count - 1]);
            return true;
        }

        private readonly int[] EracleCostPriority =
        {
            Spell.Restart,
            Trap.Turnback,
            Spell.Turbo,
            Spell.Tower,
            Spell.TCB,
            Monster.Yamiruler,
            Monster.Grace,
            Monster.Painter,
            Monster.Dancer,
            Monster.Juggler,
            Monster.Sewkyrie,
            Monster.Actor,
            Spell.Fusion
        };

        private int GetEracleCost()
        {
            if (Bot.Graveyard.GetCardCount(Spell.Restart)
                + Bot.Hand.GetCardCount(Spell.Restart)
                + Bot.MonsterZone.GetCardCount(Spell.Restart) > 2)
                return Spell.Restart;

            if (Bot.HasInGraveyard(Spell.Charity) && Bot.Deck.Count > 10)
                return Spell.Charity;

            if (Bot.Graveyard.GetCardCount(Spell.Fusion) > 1)
                return Spell.Fusion;

            if (Bot.Graveyard.GetCardCount(Monster.Actor) > 1)
                return Monster.Actor;

            int select = 0;
            int maxCount = 0;
            foreach (int id in EracleCostPriority)
            {
                int count = Bot.Graveyard.GetCardCount(id);
                if (count > maxCount)
                {
                    select = id;
                    maxCount = count;
                }
            }
            return select;
        }

        private bool EracleFieldSpin()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Eracle, 0))
                return false;
            List<ClientCard> targets =
                Enemy.MonsterZone.GetMatchingCards(c => c.HasRace(CardRace.SpellCaster))
                .ToList();
            targets.Sort(CardContainer.CompareCardAttack);
            targets.Reverse();
            while (targets.Count > 2)
                targets.RemoveAt(targets.Count - 1);

            AI.SelectCard(GetEracleCost());
            AI.SelectNextCard(targets);
            return true;
        }

        private readonly Func<ClientCard, bool>[] EracleGraveFilters = {
            c => c.HasRace(CardRace.SpellCaster),
            c => c.HasRace(CardRace.Dragon),
            c => c.HasRace(CardRace.Machine),
            c => c.HasType(CardType.Fusion),
            c => c.Level > 4,
            c => c.HasType(CardType.Normal)
        };

        private bool EracleGraveSpin()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Eracle, 0))
                return false;
            AI.SelectCard(GetEracleCost());

            List<ClientCard> targets
                = Enemy.Graveyard.GetMatchingCards(c => c.IsMonster()).ToList();
            if (targets.Count < 4)
            {
                AI.SelectNextCard(targets);
                return true;
            }
            targets.Sort(CardContainer.CompareCardAttack);
            targets.Reverse();

            IList<ClientCard> select = new List<ClientCard>();
            foreach (Func<ClientCard, bool> filter in EracleGraveFilters)
            {
                foreach (ClientCard c in targets)
                {
                    if (!filter.Invoke(c))
                        continue;
                    select.Add(c);
                    targets.Remove(c);
                    if (select.Count < 3)
                        continue;
                    AI.SelectNextCard(select);
                    return true;
                }
            }

            while (select.Count < 3)
            {
                select.Add(targets[0]);
                targets.RemoveAt(0);
            }
            
            AI.SelectCard(select);
            return true;
        }

        private bool LadonReduceAll()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Ladon, 0))
                return false;
            if (!Enemy.MonsterZone.IsExistingMatchingCard(c => {
                return c.HasRace(CardRace.Dinosaur) && c.IsAttack() && c.Attack > 0;
            }))
                return false;
            AI.SelectCard(GetHandFodder(1));
            return true;
        }

        private bool LadonReduceOne()
        {
            if (ActivateDescription == Util.GetStringId(Monster.Ladon, 1))
                return false;
            List<ClientCard> targets =
                Enemy.MonsterZone.GetMatchingCards(c => c.IsAttack()).ToList();
            if (targets.Count < 1)
                return false;
            targets.Sort(CardContainer.CompareCardAttack);
            AI.SelectCard(GetHandFodder(1));
            AI.SelectNextCard(targets[targets.Count - 1]);
            return true;
        }

        private bool YamirulerActivate()
        {
            if (Duel.Turn == 1)
                return true;
            if (Bot.MonsterZone.IsExistingMatchingCard(c => {
                return c.IsCode(Monster.Yamiruler) && c.IsDefense();
            }))
                return false;
            bool highest = !Bot.MonsterZone.IsExistingMatchingCard(c => {
                return c != Card && c.Attack >= Card.Attack;
            });
            if (highest && Enemy.LifePoints <= Card.Attack && Enemy.GetMonsterCount() < 1)
                return false;
            if (highest && Enemy.MonsterZone.IsExistingMatchingCard(c => {
                return c.IsAttack() && Enemy.LifePoints - (Card.Attack - c.Attack) <= 0;
            }))
                return false;
            if (Enemy.GetSpellCountWithoutField() < 1 && Enemy.LifePoints
                - (Util.GetTotalAttackingMonsterAttack(0)
                - Util.GetTotalAttackingMonsterAttack(1)) <= 0)
                return false;
            return true;
        }

        private bool YamiterasuRepos()
        {
            if (Duel.Turn > 1)
                return false;
            if (ActivateDescription == Util.GetStringId(Monster.Yamiterasu, 0))
                return false;
            ClientCard highVal = FirstMatch(Enemy.GetMonsters(), c => {
                if (GetBattleValue(c) < Card.Attack + 500)
                    return false;
                int rval = c.IsAttack() ? c.Defense : c.Attack;
                return rval < Card.Attack;
            });
            if (highVal != null)
            {
                AI.SelectCard(highVal);
                return true;
            }
            if (Enemy.MonsterZone.IsExistingMatchingCard(c => c.IsAttack()))
                return false;
            ClientCard forceAtk = FirstMatch(Enemy.GetMonsters(), c => {
                return Enemy.LifePoints - (Card.Attack - c.Attack) <= 0;
            });
            if (forceAtk != null)
            {
                AI.SelectCard(forceAtk);
                return true;
            }
            return false;
        }
        private bool YamiterasuFloodgate()
        {
            return ActivateDescription == Util.GetStringId(Monster.Yamiterasu, 0);
        }

        private bool NSLowlevelMisc()
        {
            if (Duel.Turn < 1 || !IsLowLevel(Card))
                return false;

            if (Card.Attack <= 500)
                return false;
            IList<ClientCard> mzone = Enemy.GetMonsters();
            return mzone.Count < 1
                || mzone.IsExistingMatchingCard(c => GetBattleValue(c) <= Card.Attack);
        }

        private bool FinalRepos()
        {
            return Card.Attack > 500 && DefaultMonsterRepos();
        }

        private bool DefensiveMonsterSet()
        {
            // try to reserve Actor
            if (Card.IsCode(Monster.Actor))
            {
                if (Bot.Hand.GetCardCount(Monster.Actor) > 1)
                    return true;
                if (Bot.Hand.GetMatchingCardsCount(IsLowLevel) > 1)
                    return false;
                return Bot.GetMonsterCount() < 1 || HasUsableSpell(Spell.Restart);
            }
            return IsLowLevel(Card);
        }

        private bool FeelshockSet()
        {
            return !Bot.HasInSpellZone(Trap.Turnback) && SpellTrapSet();
        }

        private bool RetrievableFusionSet()
        {
            return HasUsableRestart() && SpellTrapSet();
        }

        private bool SpellTrapSet()
        {
            if (Card.IsCode(Spell.Tower))
                return Bot.GetFieldSpellCard() == null;
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
