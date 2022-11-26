using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
namespace WindBot.Game.AI.Decks
{
    [Deck("Kashtira", "AI_Kashtira")]
    class KashtiraExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Nibiru = 27204311;
            public const int KashtiraUnicorn = 68304193;
            public const int KashtiraFenrir = 32909498;
            public const int KashtiraNCA = 4928565;
            public const int KashtiraNCB = 78534861;
            public const int DimensionShifter = 91800273;
            public const int NemesesCorridor = 72090076;
            public const int KashtiraNCC = 31149212;
            public const int G = 23434538;
            public const int AshBlossom = 14558127;
            public const int MechaPhantom = 31480215;
            public const int Terraforming = 73628505;
            public const int PotofProsperity = 84211599;
            public const int KashtiraNCD = 34447918;
            public const int CalledbytheGrave = 24224830;
            public const int CrossoutDesignator = 65681983;
            public const int KashtiraBirth = 69540484;
            public const int KashtiraNCE = 71832012;
            public const int KashtiraNCF = 33925864;
            public const int InfiniteImpermanence = 10045474;

            public const int ThunderDragonColossus = 15291624;
            public const int BorreloadSavageDragon = 27548199;
            public const int CupidPitch = 21915012;
            public const int KashtiraNCG = 48626373;
            public const int DiablosistheMindHacker = 95474755;
            public const int KashtiraShangriIra = 73542331;
            public const int GalaxyTomahawk = 10389142;
            public const int BagooskatheTerriblyTiredTapir = 90590303;
            public const int MekkKnightCrusadiaAvramax = 21887175;
            public const int MechaPhantomBeastAuroradon = 44097050;
            public const int QliphortGenius = 22423493;
            public const int IP = 65741786;

            public const int Token = 10389143;
            public const int Token_2 = 44097051;
        }
        bool isSummoned = false;
        bool onlyXyzSummon = false;
        bool activate_KashtiraUnicorn_1 = false;
        bool activate_KashtiraFenrir_1 = false;
        bool activate_KashtiraNCC_1 = false;
        bool activate_KashtiraNCC_2 = false;
        bool activate_KashtiraNCE = false;
        bool activate_KashtiraNCB_1 = false;
        bool activate_KashtiraShangriIra = false;
        bool activate_KashtiraNCA_1 = false;
        bool activate_DimensionShifter = false;
        bool activate_pre_KashtiraNCE = false;
        bool active_KashtiraNCD_1 = false;
        bool active_KashtiraNCD_2 = false;
        bool active_KashtiraBirth = false;
        bool active_NemesesCorridor = false;
        bool select_CalledbytheGrave = false;
        bool summon_KashtiraUnicorn = false;
        bool summon_KashtiraFenrir = false;
        bool link_mode = false;
        bool opt_0 = false;
        bool opt_1 = false;
        bool opt_2 = false;

        int flag = -1;
        int pre_link_mode = -1;
        List<ClientCard> select_Cards = new List<ClientCard>();
        List<int> Impermanence_list = new List<int>();
        List<int> should_not_negate = new List<int>
        {
            81275020, 28985331
        };
        public KashtiraExecutor(GameAI ai, Duel duel)
          : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonColossus);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus);
            AddExecutor(ExecutorType.Activate, CardId.Nibiru, NibiruEffect);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, Impermanence_activate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter, DimensionShifterEffect);
            AddExecutor(ExecutorType.Activate, CardId.G, () => { return Duel.Player != 0; });
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, () => { return Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer != 0; });
            AddExecutor(ExecutorType.Activate, CardId.CalledbytheGrave, CalledbytheGraveEffect);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutDesignatorEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.PotofProsperity, PotofProsperityEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraUnicorn, KashtiraUnicornEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraFenrir, KashtiraFenrirEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCE, KashtiraNCEEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraBirth, KashtiraBirthEffect);
            AddExecutor(ExecutorType.Activate, CardId.DiablosistheMindHacker, DiablosistheMindHackerEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraFenrir, KashtiraFenrirSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraUnicorn,()=> { summon_KashtiraUnicorn = true;return true; });
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraFenrir, () => { summon_KashtiraFenrir = true; return true; });
            AddExecutor(ExecutorType.Summon, CardId.KashtiraUnicorn, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.KashtiraFenrir, DefaultSummon);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCD, KashtiraNCDEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCA, KashtiraNCAEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCB, KashtiraNCBEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraBirth, KashtiraBirthEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyTomahawk);
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyTomahawk, GalaxyTomahawkSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraShangriIra, KashtiraShangriIraSummon);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraShangriIra, KashtiraShangriIraEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraNCG, KashtiraNCGSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.DiablosistheMindHacker, DiablosistheMindHackerSummon_2);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCG, KashtiraNCGEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KashtiraNCG, KashtiraNCGSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DiablosistheMindHacker, DiablosistheMindHackerSummon);
            //link mode
            AddExecutor(ExecutorType.SpSummon, CardId.QliphortGenius, QliphortGeniusSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.IP, IPSummon);
            AddExecutor(ExecutorType.Activate, CardId.IP, IPEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonSummon);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CupidPitch, CupidPitchSummon);
            AddExecutor(ExecutorType.Activate, CardId.CupidPitch);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.NemesesCorridor, NemesesCorridorEffect);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxSummon);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxEffect);
            //link mode
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCC, KashtiraNCCEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCC, KashtiraNCCEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCF, KashtiraNCFEffect);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraNCD, KashtiraNCDEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.KashtiraBirth, KashtiraBirthEffect_3);
            AddExecutor(ExecutorType.Summon, CardId.KashtiraNCC, KashtiraNCCSummon);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
            AddExecutor(ExecutorType.Repos, DefaultRepos);
        }
        public override void OnNewTurn()
        {
            if (pre_link_mode < 0) pre_link_mode = Program.Rand.Next(2);
            isSummoned = false;
            onlyXyzSummon = false;
            activate_KashtiraUnicorn_1 = false;
            activate_KashtiraFenrir_1 = false;
            activate_KashtiraNCC_1 = false;
            activate_KashtiraNCC_2 = false;
            activate_KashtiraNCE = false;
            activate_KashtiraNCB_1 = false;
            activate_KashtiraNCA_1 = false;
            activate_KashtiraShangriIra = false;
            active_KashtiraNCD_1 = false;
            active_KashtiraNCD_2 = false;
            active_KashtiraBirth = false;
            active_NemesesCorridor = false;
            link_mode = false;
            summon_KashtiraUnicorn = false;
            summon_KashtiraFenrir = false;
            opt_0 = false;
            opt_1 = false;
            opt_2 = false;
            if (flag >= 0) ++flag;
            if (flag >= 2) { flag = -1; activate_DimensionShifter = false; }
        }
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == 1149312192)
            {
                activate_pre_KashtiraNCE = true;
            }
            return base.OnSelectYesNo(desc);
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == 27204312|| cardId==CardId.MechaPhantom)
            {
                return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }
        public override int OnSelectOption(IList<int> options)
        {
            if (options.Count == 2 && options[1] == Util.GetStringId(CardId.KashtiraBirth, 0))
                return 1;
            if (options.Count == 2 && options.Contains(Util.GetStringId(CardId.KashtiraNCA, 1)))
            {
                return (isEffectByRemove() || Enemy.Deck.Count <= 3) ? 1 : 0;
            }
            if (options.Contains(Util.GetStringId(CardId.MechaPhantomBeastAuroradon, 3)))
            {
                if (opt_1) return options.IndexOf(Util.GetStringId(CardId.MechaPhantomBeastAuroradon, 3));
                else if (opt_0) return 0;
                return options[options.Count - 1];
            }
            return base.OnSelectOption(options);
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (cardId == 0 && player == 1)
            {
                int zone = 0;
                for (int i = 4; i >= 0; --i)
                {
                    zone = (int)System.Math.Pow(2, i);
                    if ((available & zone) > 0) return zone;
                }
            }
            if (cardId == CardId.GalaxyTomahawk)
            {
                if ((available & Zones.z5) > 0) return Zones.z5;
                if ((available & Zones.z6) > 0) return Zones.z6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (cards.Any(card => card != null && card.Location == CardLocation.Extra
                 && hint == HintMsg.Remove && min == 1 && max == 1))
            {
                //Can't get card info in enemy extral.
                int index = Program.Rand.Next(cards.Count());
                if (index < 0 || index >= cards.Count()) return null;
                IList<ClientCard> res = new List<ClientCard>();
                res.Add(cards.ElementAtOrDefault(index));
                return Util.CheckSelectCount(res, cards, min, max);
            }
            if (cards.Any(card => card != null && card.Location == CardLocation.Grave && card.Controller == 1)
                && hint == HintMsg.Remove && ((min == 1 && max == 1) || (min == 3 && max == 3)))
            {
                if (select_CalledbytheGrave) { select_CalledbytheGrave = false; return null; }
                List<ClientCard> copyCards = new List<ClientCard>(cards);
                List<int> keyCardsId = new List<int>()
                {
                    44097050,15291624,63288573,70369116,83152482,72329844,
                    24094258,86066372,74997493,85289965,21887175,11738489,
                    98127546,50588353,10389142,90590303,27548199,
                };
                List<ClientCard> preCards = new List<ClientCard>();
                List<ClientCard> resCards = new List<ClientCard>();
                foreach (var card in copyCards)
                {
                    if (card != null && (keyCardsId.Contains(card.Id) || (card.Alias != 0 && keyCardsId.Contains(card.Alias))))
                        preCards.Add(card);
                    else resCards.Add(card);
                }
                if (preCards.Count > 0) return Util.CheckSelectCount(preCards, cards, min, max);
                resCards = FilterdRepeatIdCards(resCards);
                if (resCards != null)
                {
                    resCards.Sort(CardContainer.CompareCardAttack);
                    resCards.Reverse();
                    return Util.CheckSelectCount(resCards, cards, min, max);
                }
                copyCards.Sort(CardContainer.CompareCardAttack);
                copyCards.Reverse();
                return Util.CheckSelectCount(copyCards, cards, min, max);
            }
            if (cards.Any(card => card != null && card.Location == CardLocation.Extra && card.Controller == 0)
                && hint == HintMsg.Remove && min == 3 && max == 3)
            {
                //pre_link_mode == 0 xyz_mode
                //pre_link_mode == 1 link_mode
                List<ClientCard> repeatIdCards = FilterdRepeatIdCards(cards);
                if (repeatIdCards?.Count >= 3)
                {
                    if (pre_link_mode == 1) return Util.CheckSelectCount(repeatIdCards, cards, min, max);
                }
                List<ClientCard> resCards = new List<ClientCard>();
                List<int> cardsId = new List<int>()
                {
                    CardId.BagooskatheTerriblyTiredTapir,CardId.BorreloadSavageDragon,CardId.CupidPitch,
                    CardId.QliphortGenius,CardId.IP,CardId.MechaPhantomBeastAuroradon,CardId.MekkKnightCrusadiaAvramax,
                    CardId.ThunderDragonColossus
                };
                List<ClientCard> filterCards = CardsIdToClientCards(cardsId, cards).ToList();
                if (repeatIdCards?.Count > 0 && pre_link_mode == 0) resCards.AddRange(repeatIdCards);
                if (filterCards?.Count > 0) resCards.AddRange(filterCards);
                if (repeatIdCards?.Count > 0 && pre_link_mode == 1) resCards.AddRange(repeatIdCards);
                if (resCards.Count > 0)
                {
                    return Util.CheckSelectCount(resCards, cards, min, max);
                }
                return null;
            }
            if (hint == HintMsg.Remove && cards.Any(card => card != null && (card.Location == CardLocation.Hand || card.Location == CardLocation.Grave) && card.Controller == 0) && min == 1 && max == 1)
            {
                if (select_Cards.Count > 0)
                {
                    return Util.CheckSelectCount(select_Cards, cards, min, max);
                }
                else
                {
                    IList<ClientCard> grave_cards = cards.GetMatchingCards(card => card != null && card.Location == CardLocation.Grave);
                    if (grave_cards.Count > 0) return Util.CheckSelectCount(grave_cards, cards, min, max);
                    return null;
                }

            }
            if (hint == HintMsg.XyzMaterial && cards.Any(card => card != null && card.Location == CardLocation.Removed) && min == 1 && max == 1)
            {
                List<ClientCard> m_cards = new List<ClientCard>();
                List<ClientCard> e_cards_u = new List<ClientCard>();
                List<ClientCard> e_cards_d = new List<ClientCard>();
                foreach (var card in cards)
                {
                    if (card != null && card.Controller == 0)
                        m_cards.Add(card);
                    if (card != null && card.Controller == 1 && card.IsFaceup())
                        e_cards_u.Add(card);
                    if (card != null && card.Controller == 1 && card.IsFacedown())
                        e_cards_d.Add(card);
                }
                List<ClientCard> res = new List<ClientCard>();
                if (e_cards_u.Count >= 0)
                {
                    e_cards_u.Sort(CardContainer.CompareCardAttack);
                    e_cards_u.Reverse();
                    res.AddRange(e_cards_u);
                }
                IList<int> cardsId = new List<int>() { CardId.KashtiraNCF, CardId.KashtiraNCD };
                IList<ClientCard> m_pre_cards = CardsIdToClientCards(cardsId, m_cards, false);
                if (m_pre_cards?.Count >= 0) res.AddRange(m_pre_cards);
                else if (m_cards.Count >= 0) res.AddRange(m_cards);
                if (e_cards_d.Count >= 0) res.AddRange(e_cards_d);
                if (res.Count <= 0) return null;
                return Util.CheckSelectCount(res, cards, min, max);
            }
            if (hint == HintMsg.Release && cards.Any(card => card != null && card.Location == CardLocation.MonsterZone))
            {
                List<ClientCard> tRelease = new List<ClientCard>();
                List<ClientCard> nRelease = new List<ClientCard>();
                foreach (var card in cards)
                {
                    if (card == null || card.IsExtraCard() || card.IsFacedown()) continue;
                    if (card.Id == CardId.Token || card.Id == CardId.Token_2)
                        tRelease.Add(card);
                    else nRelease.Add(card);
                }
                if (opt_1)
                {
                    IList<int> cardsId = new List<int>() { CardId.Token, CardId.Token_2 };
                    tRelease = CardsIdToClientCards(cardsId, tRelease, false).ToList();
                    if(tRelease?.Count > 0) nRelease.AddRange(tRelease);
                    if (nRelease.Count <= 0) return null;
                    return Util.CheckSelectCount(nRelease, cards, min, max);
                }
                else
                {
                    tRelease.AddRange(nRelease);
                    if(tRelease.Count <= 0) return null;
                    return Util.CheckSelectCount(tRelease, cards, min, max);
                }
            }
            if (hint == HintMsg.Destroy && cards.Any(card => card != null && card.Controller == 1 && (card.Location & CardLocation.Onfield) > 0) && min==1 && max==1)
            {
                ClientCard card = Util.GetBestEnemyCard();
                List<ClientCard> res = new List<ClientCard>();
                if (card != null && cards.Contains(card))
                {
                    res.Add(card);
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                res = cards.Where(_card => _card != null && _card.Controller == 1).ToList();
                if (res.Count <= 0) return null; 
                res.Sort(CardContainer.CompareCardAttack);
                res.Reverse();
                return Util.CheckSelectCount(res, cards, min, max);
            }
            if (activate_pre_KashtiraNCE)
            {
                activate_pre_KashtiraNCE = false;
                IList<int> cardsId = new List<int>();
                if (!Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1 && CheckRemainInDeck(CardId.KashtiraUnicorn) > 0) cardsId.Add(CardId.KashtiraUnicorn);
                if (!Bot.HasInHand(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1 && CheckRemainInDeck(CardId.KashtiraFenrir) > 0) cardsId.Add(CardId.KashtiraFenrir);
                if (!Bot.HasInHand(CardId.KashtiraNCB) && !activate_KashtiraNCB_1 && CheckRemainInDeck(CardId.KashtiraNCB) > 0) cardsId.Add(CardId.KashtiraNCB);
                if (!Bot.HasInHand(CardId.KashtiraNCA) && !activate_KashtiraNCA_1 && CheckRemainInDeck(CardId.KashtiraNCA) > 0) cardsId.Add(CardId.KashtiraNCA);
                if (!Bot.HasInHand(CardId.KashtiraNCC) && (!activate_KashtiraNCC_2 || !activate_KashtiraNCC_1) && CheckRemainInDeck(CardId.KashtiraNCC) > 0) cardsId.Add(CardId.KashtiraNCC);
                IList<ClientCard> copyCards = new List<ClientCard>(cards);
                IList<ClientCard> res = CardsIdToClientCards(cardsId, copyCards);
                if (res?.Count <= 0) return null;
                return Util.CheckSelectCount(res, cards, min, max);

            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        private int CheckRemainInDeck(int id)
        {
            switch (id)
            {
                case CardId.Nibiru:
                    return Bot.GetRemainingCount(CardId.Nibiru, 1);
                case CardId.KashtiraUnicorn:
                    return Bot.GetRemainingCount(CardId.KashtiraUnicorn, 3);
                case CardId.KashtiraFenrir:
                    return Bot.GetRemainingCount(CardId.KashtiraFenrir, 3);
                case CardId.KashtiraNCA:
                    return Bot.GetRemainingCount(CardId.KashtiraNCA, 1);
                case CardId.KashtiraNCB:
                    return Bot.GetRemainingCount(CardId.KashtiraNCB, 2);
                case CardId.DimensionShifter:
                    return Bot.GetRemainingCount(CardId.DimensionShifter, 2);
                case CardId.NemesesCorridor:
                    return Bot.GetRemainingCount(CardId.NemesesCorridor, 1);
                case CardId.KashtiraNCC:
                    return Bot.GetRemainingCount(CardId.KashtiraNCC, 3);
                case CardId.G:
                    return Bot.GetRemainingCount(CardId.G, 2);
                case CardId.AshBlossom:
                    return Bot.GetRemainingCount(CardId.AshBlossom, 3);
                case CardId.MechaPhantom:
                    return Bot.GetRemainingCount(CardId.MechaPhantom, 1);
                case CardId.Terraforming:
                    return Bot.GetRemainingCount(CardId.Terraforming, 1);
                case CardId.PotofProsperity:
                    return Bot.GetRemainingCount(CardId.PotofProsperity, 2);
                case CardId.KashtiraNCD:
                    return Bot.GetRemainingCount(CardId.KashtiraNCD, 3);
                case CardId.CalledbytheGrave:
                    return Bot.GetRemainingCount(CardId.CalledbytheGrave, 2);
                case CardId.CrossoutDesignator:
                    return Bot.GetRemainingCount(CardId.CrossoutDesignator, 1);
                case CardId.KashtiraBirth:
                    return Bot.GetRemainingCount(CardId.KashtiraBirth, 3);
                case CardId.KashtiraNCE:
                    return Bot.GetRemainingCount(CardId.KashtiraNCE, 3);
                case CardId.KashtiraNCF:
                    return Bot.GetRemainingCount(CardId.KashtiraNCF, 1);
                case CardId.InfiniteImpermanence:
                    return Bot.GetRemainingCount(CardId.InfiniteImpermanence, 2);
                default:
                    return 0;
            }
        }
        #region CopyImpermanence
        public bool Impermanence_activate()
        {
            // negate before effect used
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
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
                    AI.SelectCard(m);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

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
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    List<ClientCard> enemy_monsters = Enemy.GetMonsters();
                    enemy_monsters.Sort(CardContainer.CompareCardAttack);
                    enemy_monsters.Reverse();
                    foreach (ClientCard card in enemy_monsters)
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
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
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
        public int SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
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
        public bool is_should_not_negate()
        {
            ClientCard last_card = Util.GetLastChainCard();
            if (last_card != null
                && last_card.Controller == 1 && last_card.IsCode(should_not_negate))
                return true;
            return false;
        }
        #endregion
        private List<ClientCard> FilterdRepeatIdCards(IList<ClientCard> cards)
        {
            IList<ClientCard> temp = new List<ClientCard>();
            List<ClientCard> res = new List<ClientCard>();
            foreach (var card in cards)
            {
                if (card == null) continue;
                if (temp.Count(_card => _card != null && _card.Id == card.Id) > 0)
                    res.Add(card);
                else
                    temp.Add(card);
            }
            return res.Count < 0 ? null : res;
        }
        private IList<ClientCard> CardsIdToClientCards(IList<int> cardsId, IList<ClientCard> cardsList, bool uniqueId = true, bool alias = true)
        {
            if (cardsList?.Count() <= 0 || cardsId?.Count() <= 0) return new List<ClientCard>();
            List<ClientCard> res = new List<ClientCard>();
            foreach (var cardid in cardsId)
            {
                List<ClientCard> cards = cardsList.Where(card => card != null && (card.Id == cardid || ((card.Alias != 0 && cardid == card.Alias) & alias))).ToList();
                if (cards?.Count <= 0) continue;
                cards.Sort(CardContainer.CompareCardAttack);
                if (uniqueId) res.Add(cards.First());
                else res.AddRange(cards);
            }
            return res;
        }
        private IList<int> ClientCardsToCardsId(IList<ClientCard> cardsList, bool uniqueId = false, bool alias = false)
        {
            if (cardsList == null) return null;
            if (cardsList.Count <= 0) return new List<int>();
            IList<int> res = new List<int>();
            foreach (var card in cardsList)
            {
                if (card == null) continue;
                if (card.Alias != 0 && alias && !(res.Contains(card.Alias) & uniqueId)) res.Add(card.Alias);
                else if (card.Id != 0 && !(res.Contains(card.Id) & uniqueId)) res.Add(card.Id);
            }
            return res.Count < 0 ? null : res;
        }
        private bool DefaultRepos()
        {
            if (Card.Id == CardId.KashtiraNCB || (Card.Id == CardId.KashtiraShangriIra && Card.Attack<2000)) return false;
            return DefaultMonsterRepos();
        }
        private bool CrossoutDesignatorCheck(ClientCard LastChainCard, int id)
        {
            if (LastChainCard.IsCode(id) && CheckRemainInDeck(id) > 0)
            {
                AI.SelectAnnounceID(id);
                return true;
            }
            return false;
        }
        private bool CrossoutDesignatorEffect()
        {
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard == null || Duel.LastChainPlayer != 1) return false;
            return CrossoutDesignatorCheck(LastChainCard, CardId.Nibiru)
                || CrossoutDesignatorCheck(LastChainCard, CardId.AshBlossom)
                || CrossoutDesignatorCheck(LastChainCard, CardId.G)
                || CrossoutDesignatorCheck(LastChainCard, CardId.NemesesCorridor)
                || CrossoutDesignatorCheck(LastChainCard, CardId.InfiniteImpermanence)
                || CrossoutDesignatorCheck(LastChainCard, CardId.CalledbytheGrave)
                || CrossoutDesignatorCheck(LastChainCard, CardId.Terraforming)
                || CrossoutDesignatorCheck(LastChainCard, CardId.PotofProsperity)
                || CrossoutDesignatorCheck(LastChainCard, CardId.KashtiraNCD)
                || CrossoutDesignatorCheck(LastChainCard, CardId.KashtiraUnicorn)
                || CrossoutDesignatorCheck(LastChainCard, CardId.KashtiraFenrir)
                || CrossoutDesignatorCheck(LastChainCard, CardId.KashtiraBirth);
        }
        private bool MekkKnightCrusadiaAvramaxEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                List<ClientCard> cards = Enemy.GetMonsters();
                cards.Sort(CardContainer.CompareCardAttack);
                cards.Reverse();
                cards.AddRange(Enemy.GetSpells());
                if (cards.Count <= 0) return false;
                AI.SelectCard(cards);
                return true;
            }
            else return true;
        }
        private bool SpellSet()
        {
            return Card.HasType(CardType.QuickPlay) || Card.HasType(CardType.Trap);
        }
        private bool NibiruEffect()
        {
            if (Bot.HasInMonstersZone(CardId.KashtiraNCG, true, false, true) && Util.GetBestAttack(Bot) > Util.GetBestAttack(Enemy)) return false;
            return Bot.GetMonsterCount() <= 0 || Bot.GetMonsterCount() < Enemy.GetMonsterCount();
        }
        private bool CalledbytheGraveEffect()
        {
            ClientCard card = Util.GetLastChainCard();
            if (Duel.LastChainPlayer != 0 && card != null && card.Location == CardLocation.Grave
                && card.HasType(CardType.Monster))
            {
                AI.SelectCard(card);
                select_CalledbytheGrave = true;
                return true;
            }
            return false;
        }
        private bool CupidPitchSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.Token_2) && !Bot.HasInMonstersZone(CardId.MechaPhantom)) return false;
            IList<int> cardsId = new List<int>() { CardId.MechaPhantom, CardId.Token_2 };
            IList<ClientCard> cards = CardsIdToClientCards(cardsId, Bot.GetMonsters(), false);
            if (cards?.Count <= 0) return false;
            AI.SelectMaterials(cards);
            return true;

        }
        private bool BorreloadSavageDragonSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.Token_2) && !Bot.HasInMonstersZone(CardId.CupidPitch)) return false;
            IList<int> cardsId = new List<int>() { CardId.CupidPitch, CardId.Token_2 };
            IList<ClientCard> cards = CardsIdToClientCards(cardsId, Bot.GetMonsters(), false);
            if (cards?.Count <= 0) return false;
            AI.SelectMaterials(cards);
            link_mode = false;
            return true;
        }
        private bool BorreloadSavageDragonEffect()
        {
            if (ActivateDescription == -1)
            {
                AI.SelectCard(new[] { CardId.MekkKnightCrusadiaAvramax, CardId.MechaPhantomBeastAuroradon, CardId.IP, CardId.QliphortGenius});
                return true;
            }
            return true;

        }
        private bool IPSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.Token) && !Bot.HasInMonstersZone(CardId.Token_2)) return false;
            if (!Bot.HasInExtra(CardId.MechaPhantomBeastAuroradon) || !(Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax) && (Bot.HasInExtra(CardId.QliphortGenius) || Bot.HasInMonstersZone(CardId.QliphortGenius, false, false, true)))) return false;
            List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && !card.HasType(CardType.Link) && card.IsFaceup() && card.HasType(CardType.Monster) && !card.HasType(CardType.Xyz)).ToList();
            if (cards?.Count < 2 && !link_mode) return false;
            IList<int> cardsId = new List<int> { CardId.GalaxyTomahawk, CardId.Token };
            IList<ClientCard> pre_cards = CardsIdToClientCards(cardsId, cards, false);
            if (pre_cards?.Count >= 2) { AI.SelectMaterials(pre_cards); return true; }
            cards.Sort(CardContainer.CompareCardAttack);
            AI.SelectMaterials(cards);
            return true;
        }
        private bool IPEffect()
        {
            if (!Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax) && !(Bot.HasInMonstersZone(CardId.QliphortGenius,false,false,true) ||
                Bot.HasInMonstersZone(CardId.MechaPhantomBeastAuroradon, false, false, true))) return false;
            AI.SelectCard(CardId.MekkKnightCrusadiaAvramax);
            IList<int> cardsId = new List<int> {CardId.MechaPhantomBeastAuroradon,CardId.IP,CardId.QliphortGenius};
            List<ClientCard> m = new List<ClientCard>();
            IList<ClientCard> pre_m = CardsIdToClientCards(cardsId,Bot.GetMonsters());
            if (pre_m?.Count <= 0) return false;
            int link_count = 0;
            foreach (var card in pre_m)
            {
                m.Add(card);
                link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                if (link_count >= 4) break;
            }
            if (link_count < 4) return false;
            AI.SelectMaterials(m);
            return true;

        }
        private bool MechaPhantomBeastAuroradonEffect()
        {
            if (ActivateDescription == -1) return true;
            else
            {
                if (CheckRemainInDeck(CardId.MechaPhantom) <= 0
                    && GetEnemyOnFields().Count <= 0 && Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Trap))<=0) return false;
                List<ClientCard> tRelease = new List<ClientCard>();
                List<ClientCard> nRelease = new List<ClientCard>();
                foreach (var card in Bot.GetMonsters())
                {
                    if (card == null|| card.IsExtraCard()|| card.IsFacedown()) continue;
                    if (card.Id == CardId.Token || card.Id == CardId.Token_2)
                        tRelease.Add(card);
                    else nRelease.Add(card);
                }
                int count = tRelease.Count() + nRelease.Count();
                opt_0 = false;
                opt_1 = false;
                opt_2 = false;
                if (count >= 3 && Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Trap)) >0 ) opt_2 = true;
                if (count >= 2 && CheckRemainInDeck(CardId.MechaPhantom) > 0) opt_1 = true;
                if (count >= 1 && GetEnemyOnFields().Count > 0) opt_0 = true;
                if (!opt_0 && !opt_1 && !opt_2) return false;
                return true;
            }

        }
        private bool QliphortGeniusSummon()
        {
            List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.Id==CardId.Token).ToList();
            if (cards.Count <= 2) return false;
            AI.SelectMaterials(cards);
            return true;
        }
        private bool MekkKnightCrusadiaAvramaxSummon()
        {
            IList<int> cardsId = new List<int>() {CardId.MechaPhantomBeastAuroradon,CardId.IP,CardId.QliphortGenius};
            List<ClientCard> cards = CardsIdToClientCards(cardsId,Bot.GetMonsters()).ToList();
            if (cards.Count <= 0) return false;
            List<ClientCard> m = new List<ClientCard>();
            int link_count = 0;
            foreach (var card in cards)
            {
                m.Add(card);
                link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                if (link_count >= 4) break;
            }
            if (link_count < 4 || m.Count<2) return false;
            AI.SelectMaterials(m);
            return true;
        }
        private bool MechaPhantomBeastAuroradonSummon()
        {
            if (!Bot.HasInMonstersZone(CardId.QliphortGenius,false,false,true) && !Bot.HasInMonstersZone(CardId.Token, false, false, true)) return false;
            List<ClientCard> m = new List<ClientCard>();
            List<ClientCard> m1 = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.QliphortGenius).ToList();
            List<ClientCard> m2 = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.Token).ToList();
            if (m1.Count > 0) m.AddRange(m1);
            if (m2.Count > 0) m.AddRange(m2);
            m1.Clear();
            int link_count = 0;
            foreach (var card in m)
            {
                m1.Add(card);
                link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                if (link_count >= 3) break;
            }
            if (link_count < 3) return false;
            AI.SelectMaterials(m1);
            return true;
        }
        private bool KashtiraFenrirSummon()
        {
            if (Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth) && Bot.HasInHandOrInSpellZone(CardId.KashtiraNCD)) { summon_KashtiraFenrir = true; return true; }
            return false;
        }
        private bool DiablosistheMindHackerEffect()
        {
            AI.SelectCard(CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA);
            return true;
        }
        private bool KashtiraNCCSummon()
        {
            isSummoned = true;
            return !activate_KashtiraNCC_2;
        }
        private bool DimensionShifterEffect()
        {
            if (activate_DimensionShifter) return false;
            flag = -1;
            ++flag;
            activate_DimensionShifter = true;
            return true;
        }
        private bool KashtiraNCFEffect()
        {
            if (Card.Location == CardLocation.Removed)
            {
                AI.SelectCard(CardId.KashtiraShangriIra);
                AI.SelectNextCard(CardId.KashtiraUnicorn,CardId.KashtiraFenrir,CardId.KashtiraNCA,CardId.KashtiraNCB);
                return true;
            }
            else if(Card.Location==CardLocation.SpellZone)
            {
                if (Enemy.GetMonsterCount() <= 0) return false;
                if (Bot.GetMonsterCount() <= 1) return true;
                return Bot.GetMonsterCount()< Enemy.GetMonsterCount();
            }
            return false;
        }
        private bool SpellActivate()
        {
            return Card.Location == CardLocation.Hand || (Card.IsFacedown() && (Card.Location == CardLocation.SpellZone || Card.Location == CardLocation.FieldZone));
        }
        private bool KashtiraNCEEffect()
        {
            if (SpellActivate()) return true;
            if (activate_pre_KashtiraNCE) return false;
            List<ClientCard> cards = GetEnemyOnFields().Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
            if (cards == null || cards.Count <= 0) return false;
            return true;
        }
        private bool DiablosistheMindHackerSummon()
        {
            List<ClientCard> cards = Bot.GetMonsters();
            cards.Sort(CardContainer.CompareCardAttack);
            AI.SelectMaterials(cards);
            return true;
        }
        private bool XyzCheck()
        {
            if (Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && card.Level == 7) >= 4 && Bot.HasInExtra(CardId.KashtiraShangriIra)) return false;
            if ((active_KashtiraNCD_1 || !Bot.HasInHandOrInSpellZone(CardId.KashtiraNCD))
                && (activate_KashtiraUnicorn_1 || !Bot.HasInHand(CardId.KashtiraUnicorn) || isSummoned || !Bot.HasInSpellZone(CardId.KashtiraBirth, true, true))
                && (activate_KashtiraFenrir_1 || !Bot.HasInHand(CardId.KashtiraFenrir) || isSummoned || !Bot.HasInSpellZone(CardId.KashtiraBirth, true, true))
                && (activate_KashtiraNCB_1 || !Bot.HasInHand(CardId.KashtiraNCB) || isSummoned || !Bot.HasInSpellZone(CardId.KashtiraBirth, true, true))
                && (activate_KashtiraNCA_1 || !Bot.HasInHand(CardId.KashtiraNCA) || isSummoned || !Bot.HasInSpellZone(CardId.KashtiraBirth, true, true))
                && (activate_KashtiraNCC_2 || !Bot.HasInHand(CardId.KashtiraNCC) || activate_KashtiraNCC_1 || isSummoned)) return true;
            return false;
        
        }
        private bool GalaxyTomahawkSummon()
        {
            if (CheckRemainInDeck(CardId.MechaPhantom) < 0) return false;
            if (Bot.GetMonsterCount() >= 4) return false;
            if (onlyXyzSummon || activate_DimensionShifter || Bot.HasInMonstersZone(CardId.KashtiraNCG,true,false,true)) return false;
            if (!Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax) && !(Bot.HasInExtra(CardId.CupidPitch) || Bot.HasInExtra(CardId.BorreloadSavageDragon))) return false;
            //if (!XyzCheck()) return false;
            link_mode = true;
            return DiablosistheMindHackerSummon();
        }
        private bool DiablosistheMindHackerSummon_2()
        {
            if (Bot.HasInMonstersZone(CardId.DiablosistheMindHacker)) return false;
            return DiablosistheMindHackerSummon();
        }
        private bool isEffectByRemove()
        {
            return activate_DimensionShifter || Bot.HasInMonstersZone(CardId.KashtiraNCG, true, false, true) || Enemy.HasInMonstersZone(CardId.KashtiraNCG, true, false, true);
        }
        private bool NemesesCorridorEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if(Bot.GetMonsterCount()<=0) return true;
                if (onlyXyzSummon || !Bot.HasInExtra(CardId.ThunderDragonColossus)) return false;
                else return true;
            }
            return false;
        }
        private bool KashtiraNCAEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player != 0) return false;
                if (Duel.CurrentChain.Count > 0) return false;
                if (!ActivateLimit(Card.Id)) return false;
                activate_KashtiraNCA_1 = true;
                return true;
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (isEffectByRemove() && Enemy.Deck.Count >= 3) return true;
                if (!isEffectByRemove() && Bot.Deck.Count > 10) return true;
                return false;
            }
            if (Card.Location == CardLocation.Grave)
            {
                if (isEffectByRemove()) return false;
                return Bot.Deck.Count > 10;
            }
            return false;
        }
        private bool ActivateLimit(int cardId)
        {
            if (Bot.MonsterZone.Count() <= 0
                && ((Bot.HasInHand(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1)
                || (Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1))) return false;
            if (Bot.HasInHand(CardId.KashtiraNCE) && !activate_KashtiraNCE) return false;
            List<ClientCard> cards = new List<ClientCard>();
            List<ClientCard> hand_cards = Bot.Hand.GetMatchingCards(card=>card!=null && card.HasSetcode(0x189)).ToList();
            List<ClientCard> grave_cards = Bot.Graveyard.GetMatchingCards(card => card != null && card.HasSetcode(0x189)).ToList();
            List<int> cardsid = new List<int>();
            if (grave_cards.Count <= 0)
            {
                if ((Bot.HasInSpellZone(CardId.KashtiraBirth, true, true) && hand_cards.Count(card=>card!=null && card.Id ==  CardId.KashtiraBirth)>0)
                    || hand_cards.Count(card => card != null && card.Id == CardId.KashtiraBirth) > 1)
                    cardsid.Add(CardId.KashtiraBirth);
                if ((active_KashtiraNCD_1 && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCD) > 0)
                    || hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCD) > 1)
                    cardsid.Add(CardId.KashtiraNCD);
                if (((activate_KashtiraFenrir_1 || summon_KashtiraFenrir) && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraFenrir) > 0)
                    || hand_cards.Count(card => card != null && card.Id == CardId.KashtiraFenrir) > 1)
                    cardsid.Add(CardId.KashtiraFenrir);
                if (((activate_KashtiraUnicorn_1 || summon_KashtiraUnicorn) && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraUnicorn) > 0)
                    || hand_cards.Count(card => card != null && card.Id == CardId.KashtiraUnicorn) > 1)
                    cardsid.Add(CardId.KashtiraUnicorn);
                if (cardId != CardId.KashtiraNCB && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCB) > 0)
                    cardsid.Add(CardId.KashtiraNCB);
                if ((activate_KashtiraNCC_2 && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCC) > 0)
                    || hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCC) > 1)
                    cardsid.Add(CardId.KashtiraNCC);
                if (cardId != CardId.KashtiraNCA && hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCA) > 0)
                    cardsid.Add(CardId.KashtiraNCA);
                if(hand_cards.Count(card => card != null && card.Id == CardId.KashtiraNCF) > 0)
                   cardsid.Add(CardId.KashtiraNCF);
                if (cardsid.Count <= 0) return false;
            }
            if (Bot.HasInHand(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1 && Bot.GetMonsterCount() <= 0) return false;
            if (Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1 && Bot.GetMonsterCount() <= 0) return false;
            select_Cards.Clear();
            select_Cards.AddRange(grave_cards);
            select_Cards.AddRange(CardsIdToClientCards(cardsid, hand_cards,false));
            return true;
        }
        private bool KashtiraNCBEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player != 0) return false;
                if (Duel.CurrentChain.Count > 0) return false;
                if (!ActivateLimit(Card.Id)) return false;
                activate_KashtiraNCB_1 = true;
                return true;
            }
            return false;

        }
        private bool KashtiraNCGEffect()
        {
            if (Card.IsDisabled()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.KashtiraNCG, 1))
            {
                return true;
            }
            else
            {
                return SelectEnemyCard(false,true);
            }
        }
        private bool KashtiraNCGSummon_2()
        {
            int xcount = 0;
            int xcount_2 = 0;
            int xcount_3 = 0;
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null || card.IsFacedown()) continue;
                if (card.Level == 7) ++xcount;
                if (card.Level == 7 && card.HasSetcode(0x189))++xcount_2;
                if (card.Level != 7 && card.HasSetcode(0x189) && !card.HasType(CardType.Xyz)) ++xcount_3;
            }
            if (xcount >= 2 && (xcount_3 > 0 || xcount - xcount_2 > 0)) return KashtiraNCGSummon();
            return false;
        }
        private bool KashtiraNCGSummon()
        {
            if (Bot.HasInMonstersZone(CardId.KashtiraShangriIra,false,false,true) && !activate_KashtiraShangriIra) return false;
            if (activate_KashtiraShangriIra)
            {
                List<ClientCard> materials = Bot.GetMonsters().GetMatchingCards(card => !card.IsExtraCard() && card.IsFaceup() && Card.HasSetcode(0x189)).ToList();
                if (materials.Count() <= 0) return false;
                materials.Sort(CardContainer.CompareCardAttack);
                materials.Sort(CardContainer.CompareCardLevel);
                AI.SelectMaterials(materials);
                return true;
            }
            else
            {
                return DiablosistheMindHackerSummon();
            }
        }
        private bool DefaultSummon()
        {
            if (Bot.HasInSpellZone(CardId.KashtiraBirth, true, true) && Bot.GetMonstersInMainZone().Count<5)
            { 
                isSummoned = true;
                if (Card.Id == CardId.KashtiraUnicorn) summon_KashtiraUnicorn = true;
                else summon_KashtiraFenrir = true;
                return true; 
            }
            return false;
        }
        private bool KashtiraShangriIraSummon()
        {
            if (Bot.HasInMonstersZone(CardId.KashtiraShangriIra, true, false, true)) return false;
            List<ClientCard> materials = new List<ClientCard>();
            foreach (var card in Bot.GetMonsters())
            {
                if (materials.Count() >= 2) break;
                if (card != null && card.IsFaceup() && card.Level == 7)
                {
                    materials.Add(card);
                }
            }
            if (materials.Count() < 2) return false;
            materials.Sort(CardContainer.CompareCardAttack);
            AI.SelectMaterials(materials);
            return true;
        }
        private List<ClientCard> GetEnemyOnFields()
        {
            List<ClientCard> res = new List<ClientCard>();
            List<ClientCard> m_cards = Enemy.GetMonsters();
            List<ClientCard> s_cards = Enemy.GetSpells();
            if (m_cards.Count > 0) res.AddRange(m_cards);
            if(s_cards.Count > 0) res.AddRange(s_cards);
            return res;
        }
        private bool KashtiraShangriIraEffect()
        {
            if (!Bot.HasInMonstersZone(CardId.KashtiraUnicorn, true, false, true) && Enemy.ExtraDeck.Count > 0 && CheckRemainInDeck(CardId.KashtiraUnicorn) > 0)
            {
                AI.SelectCard(CardId.KashtiraUnicorn);
            }
            else if (!Bot.HasInMonstersZone(CardId.KashtiraFenrir, true, false, true) && CheckRemainInDeck(CardId.KashtiraFenrir) > 0)
            {
                AI.SelectCard(CardId.KashtiraFenrir);
            }
            else if (!Bot.HasInMonstersZone(CardId.KashtiraNCB, true, false, true) && CheckRemainInDeck(CardId.KashtiraNCB) > 0)
            {
                AI.SelectCard(CardId.KashtiraNCB);
            }
            else
            {
                AI.SelectCard(CardId.KashtiraNCA, CardId.KashtiraUnicorn, CardId.KashtiraFenrir, CardId.KashtiraNCB);
            }
            activate_KashtiraShangriIra = true;
            return true;
            
        }
        private void DefaultAddCardId(List<int> cardsid)
        {
            if (!Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1) cardsid.Add(CardId.KashtiraUnicorn);
            if (!Bot.HasInHand(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1) cardsid.Add(CardId.KashtiraFenrir);
            if (!Bot.HasInHand(CardId.KashtiraNCC) && !activate_KashtiraNCC_2) cardsid.Add(CardId.KashtiraNCC);
            if (!Bot.HasInHand(CardId.KashtiraNCA) && !activate_KashtiraNCA_1) cardsid.Add(CardId.KashtiraNCA);
            if (!Bot.HasInHand(CardId.KashtiraNCB) && !activate_KashtiraNCB_1) cardsid.Add(CardId.KashtiraNCB);
        }
        private bool KashtiraNCDEffect_2()
        {
            if (Card.Location == CardLocation.Removed)
            {
                List<int> cardsid = new List<int>();
                DefaultAddCardId(cardsid);
                if (!Bot.HasInExtra(CardId.KashtiraNCG)) cardsid.Add(CardId.KashtiraNCG);
                if (!Bot.HasInExtra(CardId.KashtiraShangriIra)) cardsid.Add(CardId.KashtiraShangriIra);
                cardsid.AddRange(new List<int>() { CardId.KashtiraNCF, CardId.KashtiraUnicorn, CardId.KashtiraFenrir, CardId.KashtiraNCC, CardId.KashtiraNCB, CardId.KashtiraNCA });
                AI.SelectCard(cardsid);
                active_KashtiraNCD_2 = true;
                return true;
            }
            else
            {
                AI.SelectCard(CardId.KashtiraFenrir);
                List<int> cardsid = new List<int>();
                DefaultAddCardId(cardsid);
                cardsid.AddRange(new List<int>() { CardId.KashtiraUnicorn, CardId.KashtiraFenrir, CardId.KashtiraNCC, CardId.KashtiraNCB, CardId.KashtiraNCA });
                AI.SelectNextCard(cardsid);
                active_KashtiraNCD_1 = true;
                onlyXyzSummon = true;
                return true;
            }
        }
        private bool KashtiraNCDEffect()
        {
            if (link_mode) return false;
            return KashtiraNCDEffect_2();
        }
        private bool SelectEnemyCard(bool faceUp = true, bool isXyz= false)
        {
            ClientCard card = Util.GetLastChainCard();
            if (card != null && card.Controller == 1 && card.IsFaceup() && (card.HasType(CardType.Monster)
                || card.HasType(CardType.Continuous) || card.HasType(CardType.Equip) || card.HasType(CardType.Field))
                && (card.Location & CardLocation.Onfield)>0 && !card.IsShouldNotBeTarget())
            { AI.SelectCard(card);if (isXyz)AI.SelectNextCard(card); return true; }
            if (GetEnemyOnFields().Count(_card => _card != null && !_card.IsShouldNotBeTarget() && !(faceUp & !_card.IsFaceup()) && !_card.HasType(CardType.Token)) <= 0) return false;
            ClientCard dcard = GetEnemyOnFields().GetDangerousMonster(true);
            if (Duel.Phase >= DuelPhase.Battle || Util.GetBestAttack(Enemy) >= Util.GetBestAttack(Bot) || dcard != null)
            {
                if (dcard != null){ AI.SelectCard(dcard); if(isXyz)AI.SelectNextCard(dcard) ; return true; }
                List<ClientCard> cards = GetEnemyOnFields().Where(_card => _card != null && !_card.IsShouldNotBeTarget() && !(!_card.IsFaceup() & faceUp)).ToList();
                cards.Sort(CardContainer.CompareCardAttack);
                cards.Reverse();
                if (cards.Count <= 0) return false;
                AI.SelectCard(cards);
                if (isXyz) AI.SelectNextCard(cards);
                return true;
            }
            return false;
        }
        private bool KashtiraFenrirEffect()
        {
            if (Card.IsDisabled()) return false;
            if (Duel.Phase == DuelPhase.Battle || Duel.CurrentChain.Count > 0)
            {
                if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard() != null &&
                    Util.GetLastChainCard().Id == CardId.KashtiraNCE) return false;
                return SelectEnemyCard();
            }
            else
            {
                IList<int> cardsId = new List<int>();
                if ((!Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth) || isSummoned)
                    && !Bot.HasInHand(CardId.KashtiraNCC) && (!activate_KashtiraNCC_2 && (!activate_KashtiraNCC_1 || !isSummoned)) && CheckRemainInDeck(CardId.KashtiraNCC) > 0)
                    cardsId.Add(CardId.KashtiraNCC);
                if(Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth) && !isSummoned && !Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1 && CheckRemainInDeck(CardId.KashtiraUnicorn) > 0)
                    cardsId.Add(CardId.KashtiraUnicorn);
                if(!Bot.HasInHand(CardId.KashtiraNCA) && !activate_KashtiraNCA_1 && CheckRemainInDeck(CardId.KashtiraNCA) > 0)
                    cardsId.Add(CardId.KashtiraNCA);
                if (!Bot.HasInHand(CardId.KashtiraNCB) && !activate_KashtiraNCB_1 && CheckRemainInDeck(CardId.KashtiraNCB) > 0)
                    cardsId.Add(CardId.KashtiraNCB);
                cardsId.Add(CardId.KashtiraUnicorn);
                cardsId.Add(CardId.KashtiraNCC);
                activate_KashtiraFenrir_1 = true;
                AI.SelectCard(cardsId);
                return true;
            }
        }
        private bool PotofProsperityEffect()
        {
            if (Bot.ExtraDeck.Count <= 3) return false;
            List<int> cardsId = new List<int>();
            if (!Bot.HasInHandOrInSpellZone(CardId.KashtiraNCE) && !activate_KashtiraNCE)
                cardsId.Add(CardId.KashtiraNCE);
            if (!Bot.HasInHandOrInSpellZone(CardId.KashtiraNCE) && !activate_KashtiraNCE && CheckRemainInDeck(CardId.KashtiraNCE) > 0)
                cardsId.Add(CardId.Terraforming);
            if (!Bot.HasInHand(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1)
                cardsId.Add(CardId.KashtiraUnicorn);
            if(!Bot.HasInHand(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1)
                cardsId.Add(CardId.KashtiraFenrir);
            if (!Bot.HasInHand(CardId.KashtiraNCD) && !active_KashtiraNCD_1)
                cardsId.Add(CardId.KashtiraNCD);
            if (!Bot.HasInHand(CardId.KashtiraNCC) && !activate_KashtiraNCC_2)
                cardsId.Add(CardId.KashtiraNCC);
            if(!Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth))
                cardsId.Add(CardId.KashtiraBirth);
            if(!Bot.HasInHand(CardId.KashtiraNCB) && !activate_KashtiraNCB_1)
                cardsId.Add(CardId.KashtiraNCB);
            if(!Bot.HasInHand(CardId.KashtiraNCA) && !activate_KashtiraNCA_1)
                cardsId.Add(CardId.KashtiraNCA);
            if (Bot.HasInExtra(CardId.ThunderDragonColossus) && Bot.Banished.Count(card=>card!=null && card.IsFaceup() && card.HasType(CardType.Monster))>0)
                cardsId.Add(CardId.NemesesCorridor);
            if (!Bot.HasInHand(CardId.G))
                cardsId.Add(CardId.G);
            if (!Bot.HasInHand(CardId.AshBlossom))
                cardsId.Add(CardId.AshBlossom);
            cardsId.AddRange(new List<int>() { CardId.CrossoutDesignator, CardId.CalledbytheGrave, CardId.Nibiru, CardId.InfiniteImpermanence });
            AI.SelectCard(cardsId);
            return true;
        }
        private bool KashtiraNCCEffect_2()
        {
            if (Card.Location != CardLocation.Hand)
            {
                if (CheckRemainInDeck(CardId.KashtiraNCF) > 0 && Bot.GetMonsters().GetMatchingCards(card => card != null && card.HasType(CardType.Xyz)
                        && card.HasSetcode(0x189) && card.IsFaceup() && card.Overlays.Count > 0).Count > 0)
                {
                    AI.SelectCard(CardId.KashtiraNCF);
                }
                else if (Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth) && !active_KashtiraBirth)
                {
                    if (!Bot.HasInGraveyardOrInBanished(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1
                        && CheckRemainInDeck(CardId.KashtiraUnicorn) > 0 && !active_KashtiraNCD_1
                         && !Bot.HasInHand(CardId.KashtiraNCD) && CheckRemainInDeck(CardId.KashtiraNCD) > 0)
                        AI.SelectCard(CardId.KashtiraUnicorn);
                    else if (!Bot.HasInGraveyardOrInBanished(CardId.KashtiraFenrir) && !activate_KashtiraFenrir_1
                        && CheckRemainInDeck(CardId.KashtiraFenrir) > 0)
                        AI.SelectCard(CardId.KashtiraFenrir);
                    else if (!Bot.HasInGraveyardOrInBanished(CardId.KashtiraUnicorn) && !activate_KashtiraUnicorn_1
                        && CheckRemainInDeck(CardId.KashtiraUnicorn) > 0)
                        AI.SelectCard(CardId.KashtiraFenrir);
                    else if (Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Monster) && card.HasSetcode(0x189) && !card.HasType(CardType.Xyz))
                    + Bot.Banished.Count(card_2 => card_2 != null && card_2.HasType(CardType.Monster) && card_2.HasSetcode(0x189) && !card_2.HasType(CardType.Xyz)) <= 0)
                        AI.SelectCard(CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA, CardId.KashtiraNCC);
                    else
                        AI.SelectCard(CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA, CardId.KashtiraNCC);
                }
                else if (Bot.HasInHand(CardId.NemesesCorridor) && !active_NemesesCorridor &&
                   Bot.Banished.Count(card_2 => card_2 != null && card_2.HasType(CardType.Monster)) <= 0 && Bot.HasInExtra(CardId.ThunderDragonColossus))
                {
                    AI.SelectCard(CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA, CardId.KashtiraNCC);
                }
                else if (!active_KashtiraNCD_2 && CheckRemainInDeck(CardId.KashtiraNCD) > 0 && Bot.Banished.GetMatchingCardsCount(card => card != null && card.IsFaceup() && card.HasSetcode(0x189) && card.Id != CardId.KashtiraNCD) > 0)
                {
                    AI.SelectCard(CardId.KashtiraNCD, CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA, CardId.KashtiraNCC);
                }
                else
                {
                    AI.SelectCard(CardId.KashtiraFenrir, CardId.KashtiraUnicorn, CardId.KashtiraNCB, CardId.KashtiraNCA, CardId.KashtiraNCC);
                }
                activate_KashtiraNCC_2 = true;
                return true;
            }
            return false;
        }
        private bool KashtiraNCCEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                activate_KashtiraNCC_1 = true;
                onlyXyzSummon = true;
                return true;
            }
            return false;
        }
        private bool KashtiraBirthEffect()
        {
            if (Card.Location == CardLocation.Hand || (Card.Location==CardLocation.SpellZone && Card.IsFacedown())) return !Bot.HasInSpellZone(CardId.KashtiraBirth, true, true);
            return false;
        }
        private bool KashtiraBirthEffect_2()
        {
            if (link_mode) return false;
            return KashtiraBirthEffect_3();
        }
        private bool KashtiraBirthEffect_3()
        {
            if (Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.IsFacedown())) return false;
            List<int> cardsid = new List<int>();
            if (!activate_KashtiraUnicorn_1 && !active_KashtiraNCD_1 && (Bot.HasInHand(CardId.KashtiraNCD) || CheckRemainInDeck(CardId.KashtiraNCD) > 0)) cardsid.Add(CardId.KashtiraNCD);
            if (!activate_KashtiraFenrir_1) cardsid.Add(CardId.KashtiraFenrir);
            if (!activate_KashtiraUnicorn_1) cardsid.Add(CardId.KashtiraUnicorn);
            if (!activate_KashtiraNCC_2) cardsid.Add(CardId.KashtiraNCC);
            cardsid.Add(CardId.KashtiraFenrir);
            cardsid.Add(CardId.KashtiraUnicorn);
            cardsid.Add(CardId.KashtiraNCA);
            cardsid.Add(CardId.KashtiraNCB);
            cardsid.Add(CardId.KashtiraNCC);
            AI.SelectCard(cardsid);
            return true;
        }
        private bool KashtiraUnicornEffect()
        {
            if (Card.IsDisabled()) return false;
            if (ActivateDescription == Util.GetStringId(CardId.KashtiraUnicorn, 1))
            {
                if ((!Bot.HasInHand(CardId.KashtiraNCD) && !active_KashtiraNCD_1)
                    ||(Bot.HasInHandOrInSpellZone(CardId.KashtiraBirth) && !Bot.HasInHand(CardId.KashtiraNCD)))
                    AI.SelectCard(CardId.KashtiraNCD, CardId.KashtiraBirth);
                else
                    AI.SelectCard(CardId.KashtiraBirth, CardId.KashtiraNCD);
                activate_KashtiraUnicorn_1 = true;
                return true;
            }
            else 
            {
                return true;
            }
        }
    }

}