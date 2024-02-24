using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System;
namespace WindBot.Game.AI.Decks
{
    [Deck("Tearlaments", "AI_Tearlaments")]
    class TearlamentsExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int ShaddollBeast = 3717252;
            public const int ShaddollDragon = 77723643;
            public const int TearlamentsScheiren = 572850;
            public const int TearlamentsReinoheart = 73956664;
            public const int KelbektheAncientVanguard = 25926710;
            public const int MudoratheSwordOracle = 99937011;
            public const int AgidotheAncientSentinel = 62320425;
            public const int KeldotheSacredProtector = 63542003;
            public const int NaelshaddollAriel = 97518132;
            public const int TearlamentsHavnis = 37961969;
            public const int TearlamentsMerrli = 74078255;
            public const int DivineroftheHerald = 92919429;
            public const int HeraldofOrangeLight = 17266660;
            public const int HeraldofGreenLight = 21074344;
            public const int Eva = 40177746;
            public const int TearlamentsScream = 6767771;
            public const int PrimevalPlanetPerlereino = 77103950;
            public const int TearlamentsSulliek = 74920585;

            public const int TearlamentsKaleidoHeart = 28226490;
            public const int TearlamentsRulkallos = 84330567;
            public const int PredaplantDragostapelia = 69946549;
            public const int TearlamentsKitkallos = 92731385;
            public const int ElShaddollWinda = 94977269;
            public const int ElderEntityNtss = 80532587;
            public const int BaronnedeFleur = 84815190;
            public const int FADawnDragster = 33158448;
            public const int AbyssDweller = 21044178;
            public const int UnderworldGoddessoftheClosedWorld = 98127546;
            public const int MekkKnightCrusadiaAvramax = 21887175;
            public const int KnightmareUnicorn = 38342335;
            public const int SprightElf = 27381364;
            public const int IP = 65741786;
        }
        // false: EDOPro
        const bool IS_YGOPRO = true;
        // YGOPro: 0x181
        // EDOPro: 0x182
        int SETCODE = 0x181;

        bool activate_TearlamentsScheiren_1 = false;
        bool activate_TearlamentsScheiren_2 = false;
        bool activate_TearlamentsReinoheart_1 = false;
        bool activate_TearlamentsReinoheart_2 = false;
        bool activate_TearlamentsHavnis_1 = false;
        bool activate_TearlamentsHavnis_2 = false;
        bool activate_TearlamentsMerrli_1 = false;
        bool activate_TearlamentsMerrli_2 = false;

        bool activate_TearlamentsKitkallos_1 = false;
        bool activate_TearlamentsKitkallos_2 = false;
        bool activate_TearlamentsKitkallos_3 = false;
        bool TearlamentsKitkallostohand = true;

        bool activate_TearlamentsScream_1 = false;
        bool activate_TearlamentsScream_2 = false;

        bool activate_TearlamentsSulliek_1 = false;
        bool activate_TearlamentsSulliek_2 = false;

        bool activate_AgidotheAncientSentinel_2 = false;
        bool activate_KelbektheAncientVanguard_2 = false;
        bool activate_TearlamentsRulkallos_1 = false;
        bool activate_TearlamentsRulkallos_2 = false;
        bool activate_MudoratheSwordOracle_2 = false;
        bool activate_KeldotheSacredProtector_2 = false;
        bool activate_PrimevalPlanetPerlereino_1 = false;
        bool activate_PrimevalPlanetPerlereino_2 = false;
        bool activate_TearlamentsKaleidoHeart_1 = false;
        bool activate_TearlamentsKaleidoHeart_2 = false;
        bool activate_Eva = false;
        bool activate_DivineroftheHerald = false;
        bool summoned = false;
        bool spsummoned = false;
        bool TearlamentsKitkallos_summoned = false;

        bool summon_SprightElf = false;
        bool chainlist = false;

        bool pre_activate_PrimevalPlanetPerlereino = false;

        bool select_TearlamentsKitkallos = false;
        private enum Flag
        {
            TearlamentsKitkallos = 0x1,
            TearlamentsRulkallos = 0x2,
            TearlamentsKaleidoHeart = 0x4,
            PredaplantDragostapelia = 0x8,
            ElShaddollWinda = 0x10
        }

        List<ClientCard> remainCards = new List<ClientCard>();
        List<ClientCard> fusionExtra = new List<ClientCard>();
        List<ClientCard> fusionMaterial = new List<ClientCard>();
        List<ClientCard> on_chaining_cards = new List<ClientCard>();
        List<ClientCard> mcard_0 = new List<ClientCard>() { null, null, null };
        List<ClientCard> mcard_1 = new List<ClientCard>() { null, null, null };
        List<ClientCard> mcard_2 = new List<ClientCard>() { null, null, null };
        List<ClientCard> mcard_3 = new List<ClientCard>() { null, null, null };
        List<bool> ran_fusion_mode_0 = new List<bool>() { false, false, false };
        List<bool> ran_fusion_mode_1 = new List<bool>() { false, false, false };
        List<bool> ran_fusion_mode_2 = new List<bool>() { false, false, false };
        List<bool> ran_fusion_mode_3 = new List<bool>() { false, false, false };

        ClientCard _PredaplantDragostapelia = null;
        ClientCard chain_PredaplantDragostapelia = null;
        ClientCard chain_TearlamentsSulliek = null;
        ClientCard tgcard = null;
        ClientCard no_fusion_card = null;
        List<ClientCard> e_PredaplantDragostapelia_cards = new List<ClientCard>();

        ClientCard link_card = null;
        List<int> no_link_ids = new List<int>() { CardId.TearlamentsRulkallos, CardId.BaronnedeFleur, CardId.MekkKnightCrusadiaAvramax, CardId.AbyssDweller, CardId.PredaplantDragostapelia, CardId.UnderworldGoddessoftheClosedWorld, CardId.FADawnDragster, CardId.TearlamentsKaleidoHeart };

        List<int> key_send_to_deck_ids = new List<int>()
        {
            CardId.TearlamentsMerrli,CardId.TearlamentsScheiren,CardId.TearlamentsHavnis,CardId.TearlamentsReinoheart,8736823,
            98715423,17484499
        };
        List<int> all_key_card_ids = new List<int>()
        {
             55623480,86682165,60461804,10000090,28651380,97565997,87074380,80208158,95440946,93880808,16261341,
             91749600,26866984,5141117,50383626,30576089,22586618,7445307,73478096,18558867,
             51617185,60880471,34172284,88774734,25451383,71197066,24226942,78077209,98787535,
             29601381,83203672,62383431,89552119,92418590,31042659,83303851,17502671,11366199,
             46668237,64382839,46290741,25607552,1295442,5560911,56174248,7407724,1855886,27198001,
             11074235,48372950,94142993,81866673,34966096,42006475,99733359,20799347,71985676,
             55787576,43266605,3422200,97962972,20773176,84976088,55151012,80208323,98881700,56677752,
             24506253,6180710,20056760,44928016,45702014,60316373,37683547,62962630,20065259,8571567,
             21351206,35998832,26077387,37351133,94730900,83682209,38695361,59707204,12469386,1833916,
             98806751,82496097,27182739,28762303,74578072,30303854,5370235,50546208,44818,67436768,29169993,
             10286023,19667590,47897376,74891384,66752837,43534808,56815977,81344070,59724555,93708824,
             43411796,7084129,51993760,87988305,23619206,86962245,48144778,30068120,9047460,25538345,69764158,
             70645913,55702233,10928224,79531196,23893227,57421866,94693857,78080961,9742784,59185998,
             93169863,46576366,2830693,68543408,50820852,2511,8972398,61488417,71734607,63180841,51447164,
             74586817,28403802,91575236,286392,97584719,60195675,24701066,20665527,73345237,23732205,146746,
             72218246,41999284,60303245
        };
        List<int> key_no_send_to_deck_ids = new List<int>()
        {
            CardId.TearlamentsRulkallos,50588353,44097050
        };
        List<int> key_remove_ids = new List<int>()
        {
            CardId.TearlamentsMerrli,CardId.TearlamentsScheiren,CardId.TearlamentsHavnis,CardId.TearlamentsReinoheart,15291624,
            11738489,44097050,15291624,63288573,70369116,83152482,72329844,24094258,86066372,
            74997493,85289965,21887175,11738489, 98127546,50588353,10389142,90590303,27548199,
        };
        List<int> key_no_remove_ids = new List<int>()
        {
           18743376,72355272,81555617,45960523,29596581,56713174,80280944,61103515,28297833,
           90020780,8736823
        };
        List<int> bot_send_to_deck_ids = new List<int>();
        public TearlamentsExecutor(GameAI ai, Duel duel)
  : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsScream, TearlamentsScreamEffect_1);
            AddExecutor(ExecutorType.Activate, CardId.AbyssDweller, () => { return Duel.Player != 0; });
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsKaleidoHeart, TearlamentsKaleidoHeartEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddessoftheClosedWorld);
            AddExecutor(ExecutorType.SpSummon, CardId.FADawnDragster, FADawnDragsterSummon);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsKitkallos, TearlamentsKitkallosEffect_2);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddessoftheClosedWorld, UnderworldGoddessoftheClosedWorldSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SprightElf, SprightElfSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.IP, IPSummon_2);
            AddExecutor(ExecutorType.Activate, CardId.BaronnedeFleur, BaronnedeFleurEffect);
            AddExecutor(ExecutorType.Activate, CardId.ElderEntityNtss, ElderEntityNtssEffect);
            AddExecutor(ExecutorType.Activate, CardId.PredaplantDragostapelia, PredaplantDragostapeliaEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeraldofOrangeLight, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.HeraldofGreenLight, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsRulkallos, TearlamentsRulkallosEffect);
            AddExecutor(ExecutorType.Activate, CardId.FADawnDragster);
            AddExecutor(ExecutorType.Activate, CardId.PrimevalPlanetPerlereino, PrimevalPlanetPerlereinoEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsScheiren, TearlamentsScheirenEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsKitkallos, TearlamentsKitkallosEffect);
            AddExecutor(ExecutorType.Activate, CardId.SprightElf, SprightElfEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddessoftheClosedWorld, UnderworldGoddessoftheClosedWorldSummon_3);
            AddExecutor(ExecutorType.Activate, CardId.MudoratheSwordOracle, MudoratheSwordOracleEffect);
            AddExecutor(ExecutorType.Activate, CardId.DivineroftheHerald, DivineroftheHeraldEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsMerrli, TearlamentsMerrliEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsHavnis, TearlamentsHavnisEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsReinoheart, TearlamentsReinoheartEffect);
            AddExecutor(ExecutorType.Summon, CardId.DivineroftheHerald, DivineroftheHeraldSummon);
            AddExecutor(ExecutorType.Summon, CardId.TearlamentsMerrli, () => { summoned = true; return true; });
            AddExecutor(ExecutorType.Summon, CardId.TearlamentsReinoheart, () => { summoned = true; return true; });
            AddExecutor(ExecutorType.Activate, CardId.AgidotheAncientSentinel, AgidotheAncientSentinelEffect);
            AddExecutor(ExecutorType.Activate, CardId.KelbektheAncientVanguard, KelbektheAncientVanguardEffect);
            AddExecutor(ExecutorType.Activate, CardId.NaelshaddollAriel, NaelshaddollArielEffect);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollDragon, ShaddollDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eva, EvaEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsSulliek, TearlamentsSulliekEffect);
            AddExecutor(ExecutorType.Activate, CardId.TearlamentsScream,()=> { return !AllActivated(); });
            AddExecutor(ExecutorType.Activate, CardId.KeldotheSacredProtector, MudoratheSwordOracleEffect);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollBeast, () => { return Bot.Deck.Count > 0; });
            AddExecutor(ExecutorType.Summon, CardId.DivineroftheHerald, ()=> { return !Bot.HasInHand(CardId.HeraldofGreenLight) && !Bot.HasInHand(CardId.HeraldofOrangeLight); });
            AddExecutor(ExecutorType.SpSummon, CardId.AbyssDweller, AbyssDwellerSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.SprightElf, SprightElfSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronnedeFleur, BaronnedeFleurSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FADawnDragster, () => { SetSpSummon(); return true; });
            AddExecutor(ExecutorType.SpSummon, CardId.AbyssDweller, AbyssDwellerSummon);
            AddExecutor(ExecutorType.Activate, CardId.IP, IPEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.IP, IPSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddessoftheClosedWorld, UnderworldGoddessoftheClosedWorldSummon_2);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronnedeFleur, BaronnedeFleurSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.BaronnedeFleur, () => { return Bot.HasInMonstersZone(CardId.SprightElf, true, false, true); });
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            List<ClientCard> cards = Bot.ExtraDeck.Where(card => card != null && card.Id == CardId.PredaplantDragostapelia).ToList();
            if (_PredaplantDragostapelia == null && cards.Count > 0) _PredaplantDragostapelia = cards.FirstOrDefault();
            if (!key_send_to_deck_ids.Contains(all_key_card_ids[0])) key_send_to_deck_ids.AddRange(all_key_card_ids);
            if (!key_remove_ids.Contains(all_key_card_ids[0])) key_remove_ids.AddRange(all_key_card_ids);
            if (_PredaplantDragostapelia != null && (_PredaplantDragostapelia.Location != CardLocation.MonsterZone || _PredaplantDragostapelia.IsFacedown()))
                e_PredaplantDragostapelia_cards.Clear();
            List<ClientCard> temp = new List<ClientCard>(e_PredaplantDragostapelia_cards);
            foreach (var card in temp)
            {
                if (card == null || card.Location != CardLocation.MonsterZone || card.IsFacedown()) e_PredaplantDragostapelia_cards.Remove(card);
            }
            activate_TearlamentsScheiren_1 = false;
            activate_TearlamentsScheiren_2 = false;
            activate_TearlamentsReinoheart_1 = false;
            activate_TearlamentsReinoheart_2 = false;
            activate_TearlamentsHavnis_1 = false;
            activate_TearlamentsHavnis_2 = false;
            activate_TearlamentsMerrli_1 = false;
            activate_TearlamentsMerrli_2 = false;
            activate_PrimevalPlanetPerlereino_1 = false;
            activate_PrimevalPlanetPerlereino_2 = false;

            activate_TearlamentsKitkallos_1 = false;
            activate_TearlamentsKitkallos_2 = false;
            activate_TearlamentsKitkallos_3 = false;
            activate_TearlamentsScream_1 = false;
            activate_TearlamentsScream_2 = false;
            activate_TearlamentsSulliek_1 = false;
            activate_TearlamentsSulliek_2 = false;
            activate_TearlamentsRulkallos_1 = false;
            activate_TearlamentsKaleidoHeart_1 = false;
            activate_TearlamentsKaleidoHeart_2 = false;
            activate_AgidotheAncientSentinel_2 = false;
            activate_KelbektheAncientVanguard_2 = false;
            activate_Eva = false;
            activate_DivineroftheHerald = false;
            summoned = false;
            spsummoned = false;
            summon_SprightElf = false;
            TearlamentsKitkallos_summoned = false;
            base.OnNewTurn();
        }
        private List<ClientCard> GetZoneCards(CardLocation loc, ClientField player)
        {
            List<ClientCard> res = new List<ClientCard>();
            List<ClientCard> temp = new List<ClientCard>();
            if ((loc & CardLocation.Hand) > 0) { temp = player.Hand.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.MonsterZone) > 0) { temp = player.GetMonsters(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.SpellZone) > 0) { temp = player.GetSpells(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Grave) > 0) { temp = player.Graveyard.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Removed) > 0) { temp = player.Banished.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Extra) > 0) { temp = player.ExtraDeck.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            return res;
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
            res = res.Distinct().ToList();
            return res;
        }
        private List<ClientCard> GetKeyFusionCard(int key)
        {
            int id = 0;
            if (Duel.Player == 0)
            {
                switch (key)
                {
                    case 0:
                    case 2: id = CardId.TearlamentsKitkallos; break;
                    case 1:
                    case 6: id = CardId.TearlamentsRulkallos; break;
                    case 3: id = CardId.PredaplantDragostapelia; break;
                    case 4: id = CardId.TearlamentsKaleidoHeart; break;
                    case 5: id = CardId.PredaplantDragostapelia; break;
                    default: break;
                }
            }
            else
            {
                switch (key)
                {
                    case 0:
                    case 2: id = CardId.ElShaddollWinda; break;
                    case 1:
                    case 4: id = CardId.TearlamentsKitkallos; break;
                    case 3: id = CardId.TearlamentsRulkallos; break;
                    case 5: id = CardId.TearlamentsKaleidoHeart; break;
                    case 6: id = CardId.PredaplantDragostapelia; break;
                    default: break;
                }
            }
            List<ClientCard> res = new List<ClientCard>();
            int index = -1;
            for (int i = 0; i < fusionExtra.Count; ++i)
            {
                ClientCard card = fusionExtra[i];
                if (card == null) continue;
                if (card.Id == id)
                {
                    index = i;
                    res.Add(card);
                    break;
                }
            }
            if (index > -1 && index < fusionExtra.Count) fusionExtra.RemoveAt(index);
            return res;
        }
        private bool IsLastFusionCard()
        {
            int count = 0;
            if (activate_TearlamentsScheiren_2) ++count;
            if (activate_TearlamentsHavnis_2) ++count;
            if (activate_TearlamentsMerrli_2) ++count;
            return count >= 2;
        }
        private bool CheckFusion(int listindex, int id)
        {
            int key = -1;
            if (Duel.Player == 0)
            {
                if (fusionExtra.Count > 0)
                {
                    bool flag_4 = fusionExtra.Count(card => card != null && card.Id == CardId.PredaplantDragostapelia) > 0 && Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true);
                    bool flag_1 = ((Duel.Phase < DuelPhase.End && (!activate_TearlamentsKitkallos_1 || !activate_TearlamentsKitkallos_2)) || (Duel.Phase == DuelPhase.End && !activate_TearlamentsKitkallos_1 && !activate_TearlamentsKitkallos_2)) && fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0;
                    bool flag_2 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsRulkallos) > 0 && (Bot.HasInGraveyard(CardId.TearlamentsKitkallos) || Bot.GetMonsters().Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 1 || Duel.CurrentChain.Any(card=>card!=null && card.Controller==0 && card!=Card && (card.Id==CardId.TearlamentsScheiren || card.Id==CardId.TearlamentsHavnis || card.Id==CardId.TearlamentsMerrli)));
                    // && !activate_TearlamentsKaleidoHeart_1 && GetZoneCards(CardLocation.Onfield, Enemy).Count(card => card != null && !card.IsShouldNotBeTarget()) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart)
                     //&& (!Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) || (Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) && activate_PrimevalPlanetPerlereino_2)
                    bool flag_5 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart);
                    //&& Enemy.GetMonsters().Any(card => card != null && card.HasType(CardType.Effect) && !card.IsDisabled() && card.IsFaceup() && !card.IsShouldNotBeTarget())
                    bool flag_6 = fusionExtra.Count(card => card != null && card.Id == CardId.PredaplantDragostapelia) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.PredaplantDragostapelia);
                    bool flag_7 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsRulkallos) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsRulkallos) && IsLastFusionCard();
                    bool flag_3 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0;
                    if (flag_4) key = 3;
                    else if (flag_1) key = 0;
                    else if (flag_2) key = 1;
                    else if (flag_5) key = 4;
                    else if (flag_6) key = 5;
                    else if (flag_7) key = 6;
                    else if (flag_3) key = 2;
                    if (key > -1)
                    {
                        List<ClientCard> fusionMaterialTemp = new List<ClientCard>(fusionMaterial);
                        fusionMaterialTemp.Remove(Card);
                        switch (key)
                        {
                            case 0:
                            case 2:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.HasRace(CardRace.Aqua) || (card.HasType(CardType.Monster) && card.HasSetcode(SETCODE)))).ToList();
                                break;
                            case 1:
                            case 6:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                                break;
                            case 3:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && card.Id == CardId.ElShaddollWinda).ToList();
                                break;
                            case 4:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && card.HasType(CardType.Monster) && card.HasSetcode(SETCODE)).ToList();
                                break;
                            case 5:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.Id != CardId.TearlamentsRulkallos || card.IsDisabled()) && card.HasType(CardType.Fusion)).ToList();
                                break;
                            default:
                                return false;
                        }
                        fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.HasRace(CardRace.Aqua) || (card.HasType(CardType.Monster) && card.HasSetcode(SETCODE)))).ToList();
                        int chaining_key_count = on_chaining_cards.Count(card => card != null && (card.Id == CardId.AgidotheAncientSentinel || card.Id == CardId.KelbektheAncientVanguard));
                        int chaining_key_count_2 = on_chaining_cards.Count(card => card != null && card.Id == CardId.TearlamentsSulliek);
                        List<ClientCard> current_chain_cards = new List<ClientCard>(Duel.CurrentChain);
                        IList<ClientCard> current_chain_key_cards = new List<ClientCard>();
                        foreach (var card in current_chain_cards)
                        {
                            if (card == null || card.Controller == 1) continue;
                            if ((card.Id == CardId.TearlamentsMerrli && !HasInList(current_chain_key_cards, CardId.TearlamentsMerrli))
                                || (card.Id == CardId.TearlamentsHavnis && !HasInList(current_chain_key_cards, CardId.TearlamentsHavnis)
                                || (card.Id == CardId.TearlamentsScheiren && !HasInList(current_chain_key_cards, CardId.TearlamentsScheiren))))
                                current_chain_key_cards.Add(card);
                        }
                        if ((fusionMaterialTemp.Count <= 0 && chaining_key_count <= 0 && chaining_key_count_2 <= 0) || current_chain_key_cards.Count() >= 2)
                        {
                            if (!remainCards.Contains(Card)) remainCards.Add(Card);
                            return false;
                        }
                        switch (id)
                        {
                            case CardId.TearlamentsScheiren: activate_TearlamentsScheiren_2 = true; break;
                            case CardId.TearlamentsHavnis: activate_TearlamentsHavnis_2 = true; break;
                            case CardId.TearlamentsMerrli: activate_TearlamentsMerrli_2 = true; break;
                            default: break;
                        }
                        if (chaining_key_count > 0 || chaining_key_count_2 > 0)
                        {
                            ran_fusion_mode_0[listindex] = true;
                            ran_fusion_mode_1[listindex] = true;
                            ran_fusion_mode_2[listindex] = true;
                            return true;
                        }
                        fusionMaterialTemp.Sort(CardContainer.CompareCardAttack);
                        fusionMaterialTemp = GetDefaultMaterial(fusionMaterialTemp);
                        List<ClientCard> res = GetKeyFusionCard(key);
                        mcard_0[listindex] = res.ElementAtOrDefault(0);
                        mcard_1[listindex] = Card;
                        mcard_2[listindex] = fusionMaterialTemp.ElementAtOrDefault(0);
                        if (res.Any(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart))
                        {
                            if (fusionMaterialTemp.Count <= 1)
                            {
                                mcard_0[listindex] = null;
                                mcard_1[listindex] = null;
                                mcard_2[listindex] = null;
                                ran_fusion_mode_0[listindex] = true;
                                ran_fusion_mode_1[listindex] = true;
                                ran_fusion_mode_2[listindex] = true;
                                return true;
                            }
                            if (mcard_2[listindex] != null && mcard_2[listindex].Id == CardId.TearlamentsReinoheart)
                            {
                                mcard_3[listindex] = fusionMaterialTemp.ElementAtOrDefault(1);
                            }
                            else
                            {
                                List<ClientCard> temp = fusionMaterialTemp.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart).ToList();
                                if (temp.Count <= 0)
                                {
                                    mcard_0[listindex] = null;
                                    mcard_1[listindex] = null;
                                    mcard_2[listindex] = null;
                                    ran_fusion_mode_0[listindex] = true;
                                    ran_fusion_mode_1[listindex] = true;
                                    ran_fusion_mode_2[listindex] = true;
                                    return true;
                                }
                                mcard_3[listindex] = temp.ElementAtOrDefault(0);
                            }
                        }
                        if (mcard_1[listindex] != null) fusionMaterial.Remove(mcard_1[listindex]);
                        if (mcard_2[listindex] != null) fusionMaterial.Remove(mcard_2[listindex]);
                        if (mcard_3[listindex] != null) fusionMaterial.Remove(mcard_3[listindex]);
                        //foreach (var card in fusionMaterial)
                        //{
                        //    if (card == null) continue;
                        //    Logger.DebugWriteLine("remain" + card.Id);
                        //}
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (fusionExtra.Count > 0)
                {
                    bool flag_1 = (fusionExtra.Count(card => card != null && card.Id == CardId.ElShaddollWinda) > 0
                                    && (fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) <= 0 ||
                                    activate_TearlamentsKitkallos_1 || Bot.GetMonstersInMainZone().Count >= 4 || TearlamentsKitkallos_summoned) && Bot.GetGraveyardMonsters().Any(card => card != null && card.HasSetcode(0x9d))
                                    && Card.HasAttribute(CardAttribute.Dark));
                    bool flag_2 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0 && !activate_TearlamentsKitkallos_1;
                    bool flag_3 = fusionExtra.Count(card => card != null && card.Id == CardId.ElShaddollWinda) > 0 && Bot.GetGraveyardMonsters().Any(card => card != null && card.HasSetcode(0x9d))
                                    && Card.HasAttribute(CardAttribute.Dark);
                    bool flag_4 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsRulkallos) > 0 && (Bot.HasInGraveyard(CardId.TearlamentsKitkallos) || Bot.HasInMonstersZone(CardId.TearlamentsKitkallos, false, false, true));
                    bool flag_6 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart) > 0 && !activate_TearlamentsKaleidoHeart_1 && GetZoneCards(CardLocation.Onfield, Enemy).Count(card => card != null && !card.IsShouldNotBeTarget()) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart)
                        && (!Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) || (Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) && activate_PrimevalPlanetPerlereino_2));
                    //&& Enemy.GetMonsters().Any(card => card != null && card.HasType(CardType.Effect) && !card.IsDisabled() && card.IsFaceup() && !card.IsShouldNotBeTarget())
                    bool flag_7 = fusionExtra.Count(card => card != null && card.Id == CardId.PredaplantDragostapelia) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.PredaplantDragostapelia);
                    bool flag_5 = fusionExtra.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0;
                    if (flag_1) key = 0;
                    else if (flag_2) key = 1;
                    else if (flag_3) key = 2;
                    else if (flag_4) key = 3;
                    else if (flag_6) key = 5;
                    else if (flag_7) key = 6;
                    else if (flag_5) key = 4;
                    if (key > -1)
                    {
                        List<ClientCard> fusionMaterialTemp = new List<ClientCard>(fusionMaterial);
                        fusionMaterialTemp.Remove(Card);
                        switch (key)
                        {
                            case 0:
                            case 2:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.HasType(CardType.Monster) && card.HasSetcode(0x9d))).ToList();
                                break;
                            case 1:
                            case 4:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.HasRace(CardRace.Aqua) || (card.HasType(CardType.Monster) && card.HasSetcode(SETCODE)))).ToList();
                                break;
                            case 3:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                                break;
                            case 5:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && card.HasType(CardType.Monster) && card.HasSetcode(SETCODE)).ToList();
                                break;
                            case 6:
                                fusionMaterialTemp = fusionMaterialTemp.Where(card => card != null && (card.Id != CardId.TearlamentsRulkallos || card.IsDisabled()) && card.HasType(CardType.Fusion)).ToList();
                                break;
                            default:
                                return false;
                        }
                        int chaining_key_count = on_chaining_cards.Count(card => card != null && (card.Id == CardId.AgidotheAncientSentinel || card.Id == CardId.KelbektheAncientVanguard));
                        int chaining_key_count_2 = on_chaining_cards.Count(card => card != null && card.Id == CardId.TearlamentsSulliek);
                        if (fusionMaterialTemp.Count <= 0 && chaining_key_count <= 0 && chaining_key_count_2 <= 0)
                        {
                            if (!remainCards.Contains(Card)) remainCards.Add(Card);
                            return false;
                        }
                        switch (id)
                        {
                            case CardId.TearlamentsScheiren: activate_TearlamentsScheiren_2 = true; break;
                            case CardId.TearlamentsHavnis: activate_TearlamentsHavnis_2 = true; break;
                            case CardId.TearlamentsMerrli: activate_TearlamentsMerrli_2 = true; break;
                            default: break;
                        }
                        if (chaining_key_count > 0 || chaining_key_count_2 > 0)
                        {
                            ran_fusion_mode_0[listindex] = true;
                            ran_fusion_mode_1[listindex] = true;
                            ran_fusion_mode_2[listindex] = true;
                            return true;
                        }
                        fusionMaterialTemp.Sort(CardContainer.CompareCardAttack);
                        fusionMaterialTemp = GetDefaultMaterial(fusionMaterialTemp);
                        List<ClientCard> res = GetKeyFusionCard(key);
                        mcard_0[listindex] = res.ElementAtOrDefault(0);
                        mcard_1[listindex] = Card;
                        mcard_2[listindex] = fusionMaterialTemp.ElementAtOrDefault(0);
                        if (res.Any(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart))
                        {
                            if (fusionMaterialTemp.Count <= 1)
                            {
                                mcard_0[listindex] = null;
                                mcard_1[listindex] = null;
                                mcard_2[listindex] = null;
                                ran_fusion_mode_0[listindex] = true;
                                ran_fusion_mode_1[listindex] = true;
                                ran_fusion_mode_2[listindex] = true;
                                return true;
                            }
                            if (mcard_2[listindex] != null && mcard_2[listindex].Id == CardId.TearlamentsReinoheart)
                            {
                                mcard_3[listindex] = fusionMaterialTemp.ElementAtOrDefault(1);
                            }
                            else
                            {
                                List<ClientCard> temp = fusionMaterialTemp.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart).ToList();
                                if (temp.Count <= 0)
                                {
                                    mcard_0[listindex] = null;
                                    mcard_1[listindex] = null;
                                    mcard_2[listindex] = null;
                                    ran_fusion_mode_0[listindex] = true;
                                    ran_fusion_mode_1[listindex] = true;
                                    ran_fusion_mode_2[listindex] = true;
                                    return true;
                                }
                                mcard_3[listindex] = temp.ElementAtOrDefault(0);
                            }
                        }
                        if (mcard_1[listindex] != null) fusionMaterial.Remove(mcard_1[listindex]);
                        if (mcard_2[listindex] != null) fusionMaterial.Remove(mcard_2[listindex]);
                        if (mcard_3[listindex] != null) fusionMaterial.Remove(mcard_3[listindex]);
                        if (listindex > 0 && mcard_0[0] != null && mcard_0[listindex] != null && mcard_0[listindex].Id == CardId.ElShaddollWinda)
                        {
                            ClientCard temp = mcard_0[0];
                            mcard_0[0] = mcard_0[listindex];
                            mcard_0[listindex] = temp;

                        }
                        return true;

                    }
                }
                return false;
            }
        }
        private bool FusionEffect(int id)
        {
            if (!chainlist)
            {
                chainlist = true;
                fusionExtra = Bot.ExtraDeck.Where(fcard => fcard != null).ToList();
                fusionMaterial = GetZoneCards(CardLocation.MonsterZone, Bot).Where(mcard => mcard != null && mcard.IsFaceup()).ToList();
                fusionMaterial.AddRange(GetZoneCards(CardLocation.Hand | CardLocation.Grave, Bot));

            }
            int index = 0;
            if (id == CardId.TearlamentsScheiren) index = 0;
            else if (id == CardId.TearlamentsHavnis) index = 1;
            else if (id == CardId.TearlamentsMerrli) index = 2;
            if (!CheckFusion(index, id)) return false;
            SetSpSummon();
            return true;
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            NamedCard card = NamedCard.Get(cardId);
            if (Duel.Turn > 1 && Enemy.GetMonsterCount() <= 0 && (card.Attack > 0 || cardId == CardId.FADawnDragster)
                && Duel.Player == 0)
            {
                return CardPosition.FaceUpAttack;
            }
            else if (Duel.Player == 1)
            {
                if (card.Attack < 2000) return CardPosition.FaceUpDefence;
                int eatk = Util.GetBestAttack(Enemy);
                if(eatk > card.Attack ) return CardPosition.FaceUpDefence;
                return CardPosition.FaceUpAttack;
            }
            else
            {
                if (card.Attack <= 1000) return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.SprightElf)
                {
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                }
                if (cardId == CardId.TearlamentsRulkallos && Bot.HasInExtra(CardId.SprightElf))
                {
                    if ((Zones.z2 & available) > 0) return Zones.z2;
                    if ((Zones.z0 & available) > 0) return Zones.z0;
                }
                ClientCard card = Bot.MonsterZone[5];
                if (card != null && card.Id == CardId.SprightElf)
                {
                    if ((Zones.z0 & available) > 0) return Zones.z0;
                    if ((Zones.z2 & available) > 0) return Zones.z2;
                }
                card = Bot.MonsterZone[6];
                if (card != null && card.Id == CardId.SprightElf)
                {
                    if ((Zones.z4 & available) > 0) return Zones.z4;
                    if ((Zones.z2 & available) > 0) return Zones.z2;

                }

            }
            if (player == 0 && location == CardLocation.SpellZone && location != CardLocation.FieldZone)
            {
                List<int> keys = new List<int>() { 0, 1, 2, 3, 4 };
                while (keys.Count > 0)
                {
                    int index = Program.Rand.Next(keys.Count);
                    int key = keys[index];
                    int zone = 1 << key;
                    if ((zone & available) > 0) return zone;
                    keys.Remove(key);
                }

            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override int OnSelectOption(IList<int> options)
        {
            if (options.Count == 2 && (IS_YGOPRO ? options.Contains(1190) : options.Contains(573)))
            {
                return TearlamentsKitkallostohand ? 0 : 1;
            }
            return base.OnSelectOption(options);
        }
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == 1233663200) pre_activate_PrimevalPlanetPerlereino = true;
            return base.OnSelectYesNo(desc);
        }
        public override void OnSelectChain(IList<ClientCard> cards)
        {
            if (on_chaining_cards.Count <= 0 && cards.Count > 0)
            {
                on_chaining_cards = new List<ClientCard>(cards);
            }
            base.OnSelectChain(cards);
        }
        public override void OnChaining(int player, ClientCard card)
        {
            if (!chainlist)
            {
                chainlist = true;
                remainCards.Clear();
                fusionExtra = Bot.ExtraDeck.Where(fcard => fcard != null).ToList();
                fusionMaterial = GetZoneCards(CardLocation.MonsterZone, Bot).Where(mcard => mcard != null && mcard.IsFaceup()).ToList();
                fusionMaterial.AddRange(GetZoneCards(CardLocation.Hand | CardLocation.Grave, Bot));

            }
            base.OnChaining(player, card);
        }
        public override void OnChainEnd()
        {
            remainCards.Clear();
            fusionExtra.Clear();
            fusionMaterial.Clear();
            on_chaining_cards.Clear();
            tgcard = null;
            no_fusion_card = null;
            for (int i = 0; i < mcard_0.Count; ++i) mcard_0[i] = null;
            for (int i = 0; i < mcard_1.Count; ++i) mcard_1[i] = null;
            for (int i = 0; i < mcard_2.Count; ++i) mcard_2[i] = null;
            for (int i = 0; i < mcard_3.Count; ++i) mcard_3[i] = null;
            for (int i = 0; i < ran_fusion_mode_0.Count; ++i) ran_fusion_mode_0[i] = false;
            for (int i = 0; i < ran_fusion_mode_1.Count; ++i) ran_fusion_mode_1[i] = false;
            for (int i = 0; i < ran_fusion_mode_2.Count; ++i) ran_fusion_mode_2[i] = false;
            chainlist = false;
            base.OnChainEnd();
        }
        private bool IsAvailableZone(int seq)
        {
            ClientCard card = Bot.MonsterZone[seq];
            if (seq == 5 || seq == 6)
            {
                ClientCard card1 = Bot.MonsterZone[5];
                ClientCard card2 = Bot.MonsterZone[6];
                if (card1 != null && card1.Controller == 0 && no_link_ids.Contains(card1.Id)) return false;
                if (card2 != null && card2.Controller == 0 && no_link_ids.Contains(card2.Id)) return false;
            }
            if (card == null) return true;
            if (card.Controller != 0) return false;
            if (card.IsFacedown()) return false;
            if (card.IsDisabled()) return true;
            if (card.Id == CardId.SprightElf && summon_SprightElf) return false;
            if (no_link_ids.Contains(card.Id)) return false;
            return true;
        }
        private bool IsAvailableLinkZone()
        {
            int zones = 0;
            List<ClientCard> cards = Bot.GetMonstersInMainZone().Where(card => card != null && card.IsFaceup()).ToList();
            foreach (var card in cards)
            {
                zones |= card.GetLinkedZones();
            }
            ClientCard e_card = Bot.MonsterZone[5];
            if (e_card != null && e_card.IsFaceup() && e_card.HasType(CardType.Link))
            {
                if (e_card.Controller == 0)
                {
                    if (e_card.HasLinkMarker(CardLinkMarker.BottomLeft))
                        zones |= 1 << 0;
                    if (e_card.HasLinkMarker(CardLinkMarker.Bottom))
                        zones |= 1 << 1;
                    if (e_card.HasLinkMarker(CardLinkMarker.BottomRight))
                        zones |= 1 << 2;
                }
                if (e_card.Controller == 1)
                {
                    if (e_card.HasLinkMarker(CardLinkMarker.TopLeft))
                        zones |= 1 << 2;
                    if (e_card.HasLinkMarker(CardLinkMarker.Top))
                        zones |= 1 << 1;
                    if (e_card.HasLinkMarker(CardLinkMarker.TopRight))
                        zones |= 1 << 0;
                }
            }
            e_card = Bot.MonsterZone[6];
            if (e_card != null && e_card.IsFaceup() && e_card.HasType(CardType.Link))
            {
                if (e_card.Controller == 0)
                {
                    if (e_card.HasLinkMarker(CardLinkMarker.BottomLeft))
                        zones |= 1 << 2;
                    if (e_card.HasLinkMarker(CardLinkMarker.Bottom))
                        zones |= 1 << 3;
                    if (e_card.HasLinkMarker(CardLinkMarker.BottomRight))
                        zones |= 1 << 4;
                }
                if (e_card.Controller == 1)
                {
                    if (e_card.HasLinkMarker(CardLinkMarker.TopLeft))
                        zones |= 1 << 4;
                    if (e_card.HasLinkMarker(CardLinkMarker.Top))
                        zones |= 1 << 3;
                    if (e_card.HasLinkMarker(CardLinkMarker.TopRight))
                        zones |= 1 << 2;
                }
            }
            zones &= 0x7f;
            link_card = null;
            if ((zones & Zones.z0) > 0 && IsAvailableZone(0)) return GetZoneLinkCards(0);
            if ((zones & Zones.z1) > 0 && IsAvailableZone(1)) return GetZoneLinkCards(1);
            if ((zones & Zones.z2) > 0 && IsAvailableZone(2)) return GetZoneLinkCards(2);
            if ((zones & Zones.z3) > 0 && IsAvailableZone(3)) return GetZoneLinkCards(3);
            if ((zones & Zones.z4) > 0 && IsAvailableZone(4)) return GetZoneLinkCards(4);
            if (IsAvailableZone(5)) return GetZoneLinkCards(5);
            if (IsAvailableZone(6)) return GetZoneLinkCards(6);
            return false;
        }
        private bool GetZoneLinkCards(int index)
        {
            if (index >= Bot.MonsterZone.Count()) index = 0;
            link_card = Bot.MonsterZone[index];
            return true;
        }

        private List<ClientCard> GetDefaultMaterial(IList<ClientCard> cards)
        {
            List<ClientCard> first_cards = new List<ClientCard>();
            List<ClientCard> first_mzone_cards = new List<ClientCard>();
            List<ClientCard> grave_cards = new List<ClientCard>();
            List<ClientCard> mzone_cards = new List<ClientCard>();
            List<ClientCard> hand_cards = new List<ClientCard>();
            List<ClientCard> last_cards = new List<ClientCard>();
            List<ClientCard> last_cards_2 = new List<ClientCard>();
            foreach (var card in Duel.CurrentChain)
            {
                if (card == null) continue;
                if (((card.Id == CardId.TearlamentsScheiren && !HasInList(last_cards, CardId.TearlamentsScheiren))
                    || (card.Id == CardId.TearlamentsMerrli && !HasInList(last_cards, CardId.TearlamentsMerrli))
                    || (card.Id == CardId.TearlamentsHavnis && !HasInList(last_cards, CardId.TearlamentsHavnis))
                    ) && cards.Contains(card))
                {
                    last_cards.Add(card);
                }

            }
            foreach (var card in remainCards)
            {
                if (card == null) continue;
                if (!cards.Contains(card)) first_cards.Add(card);
            }
            foreach (var card in cards)
            {
                if (card == null || first_mzone_cards.Contains(card) || first_cards.Contains(card) || last_cards.Contains(card) ||
                    grave_cards.Contains(card) || mzone_cards.Contains(card) || hand_cards.Contains(card)) continue;
                if (card.Id == CardId.ElShaddollWinda && card.IsFaceup() && card.Location == CardLocation.MonsterZone && !card.IsDisabled()) first_cards.Add(card);
                else if (card.Id != CardId.TearlamentsRulkallos && first_mzone_cards.Count() <= 0 && card.Location == CardLocation.MonsterZone && Bot.GetMonstersInMainZone().Count > 4)
                {
                    first_mzone_cards.Add(card);
                }
                else if (card.Location == CardLocation.Grave)
                {
                    if ((((card.Id == CardId.TearlamentsScheiren && !activate_TearlamentsScheiren_2)
                        || (card.Id == CardId.TearlamentsMerrli && !activate_TearlamentsMerrli_2)
                        || (card.Id == CardId.TearlamentsHavnis && !activate_TearlamentsHavnis_2))
                        && last_cards.Count(mcard => mcard != null && mcard.Id == card.Id) <= 0
                        && on_chaining_cards.Count(mcard => mcard != null && mcard.Id == card.Id) > 0)
                    || (on_chaining_cards.Count(ccard => ccard != null && ccard == card) > 0 && on_chaining_cards.Count(cccard => cccard != null && cccard.Id == card.Id) <= 1
                    && last_cards.Count(mcard => mcard != null && mcard.Id == card.Id) <= 0 && Duel.CurrentChain.Count(cccard => cccard != null && cccard.Id == card.Id) > 0)|| (no_fusion_card!=null && no_fusion_card==card))
                    {
                        last_cards.Add(card);
                    }
                    else grave_cards.Add(card);

                }
                else if (card.Location == CardLocation.Hand)
                {
                    if (card.Id == CardId.TearlamentsReinoheart) hand_cards.Insert(0, card);
                    else if (Duel.Player == 1 && card.Id == CardId.TearlamentsHavnis && !activate_TearlamentsHavnis_1) last_cards.Add(card);
                    else hand_cards.Add(card);
                }
                else if (card.Location == CardLocation.MonsterZone)
                {
                    if (card.Id == CardId.TearlamentsKitkallos && card.IsDisabled()) last_cards.Add(card);
                    else if (card.Id == CardId.TearlamentsKitkallos) last_cards_2.Insert(0, card);
                    else if (card.Id == CardId.TearlamentsRulkallos
                        || card.Id == CardId.TearlamentsKaleidoHeart || card.Id == CardId.PredaplantDragostapelia ||
                        (Duel.Player == 1 && card.Id == CardId.ElShaddollWinda)) last_cards_2.Add(card);
                    else mzone_cards.Add(card);
                }
                else mzone_cards.Add(card);
            }
            first_cards.AddRange(first_mzone_cards);
            first_cards.AddRange(grave_cards);
            first_cards.AddRange(hand_cards);
            first_cards.AddRange(mzone_cards);
            first_cards.AddRange(last_cards);
            first_cards.AddRange(last_cards_2);
            return first_cards;
        }
        public override IList<ClientCard> OnSelectFusionMaterial(IList<ClientCard> cards, int min, int max)
        {
            if (AI.HaveSelectedCards()) return null;
            for (int i = 2; i >= 0; --i)
            {
                List<ClientCard> keys = cards.Where(card => card != null && card.Location != CardLocation.MonsterZone && card.Id == CardId.TearlamentsReinoheart).ToList();
                if (ran_fusion_mode_1[i] || ran_fusion_mode_2[i] || ran_fusion_mode_3[i])
                {
                    if (ran_fusion_mode_1[i]) ran_fusion_mode_1[i] = false;
                    else if (ran_fusion_mode_2[i]) ran_fusion_mode_2[i] = false;
                    else if (ran_fusion_mode_3[i]) ran_fusion_mode_3[i] = false;
                    cards.ToList().Sort(CardContainer.CompareCardAttack);
                    return keys.Count > 0 ? Util.CheckSelectCount(keys, cards, min, max) : Util.CheckSelectCount(GetDefaultMaterial(cards), cards, min, max);
                }
                List<ClientCard> res = null;
                ClientCard mcard = mcard_1[i];
                if (mcard != null)
                {
                    if (keys.Count() > 0)
                    {
                        mcard_1[i] = null;
                        return Util.CheckSelectCount(keys, cards, min, max);
                    }
                    mcard_1[i] = null;
                    List<ClientCard> temp = new List<ClientCard>(cards);
                    temp.Sort(CardContainer.CompareCardAttack);
                    res = GetDefaultMaterial(temp);
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                mcard = mcard_2[i];
                if (mcard != null)
                {
                    mcard_2[i] = null;
                    List<ClientCard> temp = new List<ClientCard>(cards);
                    temp.Sort(CardContainer.CompareCardAttack);
                    res = GetDefaultMaterial(temp);
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                mcard = mcard_3[i];
                if (mcard != null)
                {
                    mcard_3[i] = null;
                    List<ClientCard> temp = new List<ClientCard>(cards);
                    temp.Sort(CardContainer.CompareCardAttack);
                    res = GetDefaultMaterial(temp);
                    return Util.CheckSelectCount(res, cards, min, max);
                }
            }
            return base.OnSelectFusionMaterial(cards, min, max);
        }
        private bool HasInList(IList<ClientCard> cards, int id)
        {
            if (cards == null || cards.Count <= 0) return false;
            return cards.Any(card => card != null && card.Id == id);
        }
        private bool IsCanSpSummon()
        {
            if ((Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true)
                || Enemy.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true)) && spsummoned) return false;
            return true;
        }
        private void SetSpSummon()
        {
            if (Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true) ||
                Enemy.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true)) spsummoned = true;
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if ((AI.HaveSelectedCards() && mcard_0.All(card => card == null) && ran_fusion_mode_0.All(flag => !flag))
                || (hint == HintMsg.FusionMaterial)) return null;
            if (pre_activate_PrimevalPlanetPerlereino)
            {
                pre_activate_PrimevalPlanetPerlereino = false;
                List<int> ids = GetCardsIdSendToHand();
                return Util.CheckSelectCount(CardsIdToClientCards(ids, cards, false, true), cards, min, max);

            }
            //!IS_YGOPRO && select_TearlamentsKitkallos && hint == HintMsg.AddToHand
            if ((IS_YGOPRO && hint == HintMsg.OperateCard) || (!IS_YGOPRO && select_TearlamentsKitkallos && hint == HintMsg.AddToHand))
            {
                if (!IS_YGOPRO) select_TearlamentsKitkallos = false;
                IList<int> ids = new List<int>();
                IList<ClientCard> res = new List<ClientCard>();
                if (Duel.Player == 0)
                {
                    if (!activate_TearlamentsScheiren_1 && !Bot.HasInHand(CardId.TearlamentsScheiren) && HasInList(cards, CardId.TearlamentsScheiren) && !AllActivated()) ids.Add(CardId.TearlamentsScheiren);
                    if (!activate_TearlamentsMerrli_1 && !Bot.HasInHand(CardId.TearlamentsMerrli) && HasInList(cards, CardId.TearlamentsMerrli) && !AllActivated()) ids.Add(CardId.TearlamentsMerrli);
                    if(!Bot.HasInHand(CardId.TearlamentsHavnis) && HasInList(cards, CardId.TearlamentsHavnis)) ids.Add(CardId.TearlamentsHavnis);
                    if (!activate_TearlamentsReinoheart_1 && !Bot.HasInHand(CardId.TearlamentsReinoheart) && HasInList(cards, CardId.TearlamentsReinoheart)) ids.Add(CardId.TearlamentsReinoheart);
                    if (ids.Count() <= 0 && !activate_TearlamentsKitkallos_2)
                    {
                        List<ClientCard> should_spsummon_cards = GetZoneCards(CardLocation.Grave | CardLocation.Hand, Bot);
                        should_spsummon_cards = should_spsummon_cards.Where(card => card != null && card.HasSetcode(SETCODE) && card.HasType(CardType.Monster)).ToList();
                        if (should_spsummon_cards.Count() <= 0)
                        {
                            if (HasInList(cards, CardId.TearlamentsScheiren)) ids.Add(CardId.TearlamentsScheiren);
                            if (HasInList(cards, CardId.TearlamentsMerrli)) ids.Add(CardId.TearlamentsMerrli);
                            if (HasInList(cards, CardId.TearlamentsReinoheart)) ids.Add(CardId.TearlamentsReinoheart);
                            if (HasInList(cards, CardId.TearlamentsHavnis)) ids.Add(CardId.TearlamentsHavnis);
                        }
                    }
                    if (HasInList(cards, CardId.TearlamentsScream) && !activate_TearlamentsScream_1) ids.Add(CardId.TearlamentsScream);
                    if (HasInList(cards, CardId.TearlamentsSulliek) && on_chaining_cards.Count(card => card != null && card.Id == CardId.TearlamentsScream) <= 0) ids.Add(CardId.TearlamentsSulliek);
                    if (HasInList(cards, CardId.PrimevalPlanetPerlereino) && !activate_PrimevalPlanetPerlereino_1) ids.Add(CardId.PrimevalPlanetPerlereino);
                    if (ids.Count > 0)
                    {
                        TearlamentsKitkallostohand = true;
                    }
                    else
                    {
                        if (IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua))
                        {
                            if (!activate_TearlamentsHavnis_2 && HasInList(cards, CardId.TearlamentsHavnis)) ids.Add(CardId.TearlamentsHavnis);
                            if (!activate_TearlamentsScheiren_2 && HasInList(cards, CardId.TearlamentsScheiren)) ids.Add(CardId.TearlamentsScheiren);
                            if (!activate_TearlamentsMerrli_2 && HasInList(cards, CardId.TearlamentsMerrli)) ids.Add(CardId.TearlamentsMerrli);
                            if (ids.Count > 0) TearlamentsKitkallostohand = false;
                        }
                    }

                }
                else
                {
                    if (IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua, true))
                    {
                        if (!activate_TearlamentsHavnis_2 && HasInList(cards, CardId.TearlamentsHavnis) && !Bot.HasInHand(CardId.TearlamentsHavnis)) ids.Add(CardId.TearlamentsHavnis);
                        if (!activate_TearlamentsScheiren_2 && HasInList(cards, CardId.TearlamentsScheiren) && !Bot.HasInHand(CardId.TearlamentsScheiren)) ids.Add(CardId.TearlamentsScheiren);
                        if (!activate_TearlamentsMerrli_2 && HasInList(cards, CardId.TearlamentsMerrli) && !Bot.HasInHand(CardId.TearlamentsMerrli)) ids.Add(CardId.TearlamentsMerrli);
                        if (!activate_TearlamentsHavnis_2 && HasInList(cards, CardId.TearlamentsHavnis) && !ids.Contains(CardId.TearlamentsHavnis)) ids.Add(CardId.TearlamentsHavnis);
                        if (!activate_TearlamentsScheiren_2 && HasInList(cards, CardId.TearlamentsScheiren) && !ids.Contains(CardId.TearlamentsScheiren)) ids.Add(CardId.TearlamentsScheiren);
                        if (!activate_TearlamentsMerrli_2 && HasInList(cards, CardId.TearlamentsMerrli) && !ids.Contains(CardId.TearlamentsMerrli)) ids.Add(CardId.TearlamentsMerrli);
                    }
                    if (ids.Count > 0) TearlamentsKitkallostohand = false;
                    else
                    {
                        if (!Bot.HasInHand(CardId.TearlamentsHavnis) && !activate_TearlamentsHavnis_1 && HasInList(cards, CardId.TearlamentsHavnis))
                        {
                            ids.Add(CardId.TearlamentsHavnis);
                            TearlamentsKitkallostohand = true;
                        }
                        else if (!activate_TearlamentsReinoheart_2 && HasInList(cards, CardId.TearlamentsReinoheart) && Bot.Hand.Any(card => card != null && card.HasSetcode(SETCODE)))
                        {
                            ids.Add(CardId.TearlamentsReinoheart);
                            TearlamentsKitkallostohand = false;
                        }
                        else if (!activate_TearlamentsSulliek_2 && HasInList(cards, CardId.TearlamentsSulliek))
                        {
                            ids.Add(CardId.TearlamentsSulliek);
                            TearlamentsKitkallostohand = false;
                        }
                        else
                        {
                            if (!Bot.HasInHand(CardId.PrimevalPlanetPerlereino) && HasInList(cards, CardId.PrimevalPlanetPerlereino)) ids.Add(CardId.PrimevalPlanetPerlereino);
                            if (!Bot.HasInHand(CardId.TearlamentsScheiren) && HasInList(cards, CardId.TearlamentsScheiren)) ids.Add(CardId.TearlamentsScheiren);
                            if (!Bot.HasInHand(CardId.TearlamentsMerrli) && HasInList(cards, CardId.TearlamentsMerrli)) ids.Add(CardId.TearlamentsMerrli);
                            if (!Bot.HasInHand(CardId.TearlamentsReinoheart) && HasInList(cards, CardId.TearlamentsReinoheart)) ids.Add(CardId.TearlamentsReinoheart);
                            if (!Bot.HasInHand(CardId.TearlamentsScream) && HasInList(cards, CardId.TearlamentsScream)) ids.Add(CardId.TearlamentsScream);
                            if (!Bot.HasInHand(CardId.TearlamentsSulliek) && HasInList(cards, CardId.TearlamentsSulliek)) ids.Add(CardId.TearlamentsSulliek);
                            TearlamentsKitkallostohand = true;
                        }
                    }

                }
                res = CardsIdToClientCards(ids, cards, false);
                if (res.Count <= 0 || (Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true) && spsummoned)) TearlamentsKitkallostohand = true;
                return res.Count > 0 ? Util.CheckSelectCount(res, cards, min, max) : null;
            }
            if (hint == HintMsg.Remove && cards.Any(card => card != null && ((card.HasAttribute(CardAttribute.Light) && card.HasRace(CardRace.Fairy)) || card.Controller == 1)))
            {
                IList<ClientCard> res = CardsIdToClientCards(new List<int>() { CardId.HeraldofGreenLight, CardId.HeraldofOrangeLight, CardId.DivineroftheHerald }, cards.Where(card => card != null && card.Location == CardLocation.Grave).ToList(), false);
                List<ClientCard> eres = cards.Where(card => card != null && card.Controller == 1 && !key_no_remove_ids.Contains(card.Id)).ToList();
                eres.Sort(CardContainer.CompareCardAttack);
                eres.Reverse();
                int emax = eres.Count > max ? max : eres.Count <= 0 ? min : eres.Count;
                int mmax = res.Count > max ? max : res.Count <= 0 ? min : res.Count;
                return eres.Count > 0 ? Util.CheckSelectCount(eres, cards, emax, emax) : res.Count > 0 ? Util.CheckSelectCount(res, cards, mmax, mmax) : null;
            }
            if (hint == HintMsg.AddToHand && cards.Any(card => card != null && card.Location == CardLocation.Deck))
            {
                List<int> ids = new List<int>() { CardId.HeraldofOrangeLight, CardId.HeraldofGreenLight, CardId.DivineroftheHerald };
                if (!Bot.HasInSpellZoneOrInGraveyard(CardId.DivineroftheHerald) && (Bot.HasInHand(CardId.HeraldofOrangeLight) || Bot.HasInHand(CardId.HeraldofGreenLight)))
                {
                    ids.Clear();
                    ids.AddRange(new List<int>() { CardId.DivineroftheHerald, CardId.HeraldofOrangeLight, CardId.HeraldofGreenLight });
                }
                ids.AddRange(GetCardsIdSendToHand().Distinct());
                IList<ClientCard> res = CardsIdToClientCards(ids, cards, false);
                return res.Count > 0 ? Util.CheckSelectCount(res, cards, max, max) : null;
            }
            if (hint == HintMsg.ToGrave && cards.Any(card => card != null && card.Location == CardLocation.MonsterZone) && min == 1 && max == 1)
            {
                List<ClientCard> h_cards = new List<ClientCard>();
                List<ClientCard> m_cards = new List<ClientCard>();
                List<ClientCard> s_cards = new List<ClientCard>();
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Location == CardLocation.Hand) h_cards.Add(card);
                    else if (card.Location == CardLocation.SpellZone) s_cards.Add(card);
                    else m_cards.Add(card);
                }
                List<ClientCard> mkeycards = new List<ClientCard>();
                List<ClientCard> hkeycards = new List<ClientCard>();
                mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                if (!activate_TearlamentsKitkallos_3 && mkeycards.Count > 0 && !AllActivated() && IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua)
                    && ((CheckRemainInDeck(CardId.TearlamentsHavnis) > 0 && !activate_TearlamentsHavnis_2)
                    || (CheckRemainInDeck(CardId.TearlamentsMerrli) > 0 && !activate_TearlamentsMerrli_2)
                    || (CheckRemainInDeck(CardId.TearlamentsScheiren) > 0 && !activate_TearlamentsScheiren_2)))
                    return Util.CheckSelectCount(mkeycards, cards, min, max);
                if (IsShouldSummonFusion())
                {
                    if (!activate_TearlamentsScheiren_2 && HasInList(cards, CardId.TearlamentsScheiren))
                    {
                        hkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsScheiren && card.Location == CardLocation.Hand).ToList();
                        mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsScheiren && card.Location == CardLocation.MonsterZone).ToList();
                    }
                    else if (!activate_TearlamentsHavnis_2 && HasInList(cards, CardId.TearlamentsHavnis))
                    {
                        hkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsHavnis && card.Location == CardLocation.Hand).ToList();
                        mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsHavnis && card.Location == CardLocation.MonsterZone).ToList();
                    }
                    else if (!activate_TearlamentsMerrli_2 && HasInList(cards, CardId.TearlamentsMerrli))
                    {
                        hkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsMerrli && card.Location == CardLocation.Hand).ToList();
                        mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsMerrli && card.Location == CardLocation.MonsterZone).ToList();
                    }
                    else if (!activate_TearlamentsReinoheart_2 && (HasinZoneKeyCard(CardLocation.Hand) || (Bot.Hand.Count(ccard => ccard != null && ccard.HasSetcode(SETCODE) && ccard != cards.Where(card => card != null && card.Location == CardLocation.Hand && card.Id == CardId.TearlamentsReinoheart).FirstOrDefault()) > 0 && HasinZoneKeyCard(CardLocation.Deck))))
                    {
                        hkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart && card.Location == CardLocation.Hand).ToList();
                        mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart && card.Location == CardLocation.MonsterZone).ToList();
                    }
                    if (mkeycards.Count > 0 || hkeycards.Count > 0)
                    {
                        if (Bot.GetMonstersInMainZone().Count < 5)
                        {
                            hkeycards.AddRange(mkeycards);
                            return Util.CheckSelectCount(hkeycards, cards, min, max);
                        }
                        else
                        {
                            mkeycards.AddRange(hkeycards);
                            return Util.CheckSelectCount(mkeycards, cards, min, max);
                        }
                    }
                }
                mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart).ToList();
                if (!activate_TearlamentsKaleidoHeart_2 && IsCanSpSummon() && mkeycards.Count > 0) return Util.CheckSelectCount(mkeycards, cards, min, max);
                mkeycards = cards.Where(card => card != null && card.Id == CardId.TearlamentsRulkallos).ToList();
                if (!activate_TearlamentsRulkallos_2 && IsCanSpSummon() && mkeycards.Count > 0) return Util.CheckSelectCount(mkeycards, cards, min, max);
                s_cards.AddRange(h_cards);
                m_cards.Sort(CardContainer.CompareCardAttack);
                s_cards.AddRange(m_cards);
                return Util.CheckSelectCount(s_cards, cards, min, max);
            }
            if (hint == HintMsg.ToGrave && cards.Any(card => card != null && card.Location == CardLocation.Hand) && min == 1 && max == 1)
            {
                IList<int> ids = new List<int>();
                if (!activate_AgidotheAncientSentinel_2) ids.Add(CardId.AgidotheAncientSentinel);
                if (!activate_KelbektheAncientVanguard_2) ids.Add(CardId.KelbektheAncientVanguard);
                if (IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua))
                {
                    if (activate_TearlamentsScheiren_1 && !activate_TearlamentsScheiren_2) ids.Add(CardId.TearlamentsScheiren);
                    if (summoned && !activate_TearlamentsMerrli_2) ids.Add(CardId.TearlamentsMerrli);
                    if (!activate_TearlamentsHavnis_2) ids.Add(CardId.TearlamentsHavnis);
                    if (!activate_TearlamentsMerrli_2) ids.Add(CardId.TearlamentsMerrli);
                    if (!activate_TearlamentsScheiren_2) ids.Add(CardId.TearlamentsScheiren);
                }
                if (Enemy.GetSpellCount() > 0) ids.Add(CardId.ShaddollDragon);
                if (Bot.Deck.Count > 0) ids.Add(CardId.ShaddollBeast);
                if (Enemy.Graveyard.Count > 0) ids.Add(CardId.NaelshaddollAriel);
                if (!activate_Eva && Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Light) && card.HasRace(CardRace.Fairy)) > 0
                   && (CheckRemainInDeck(CardId.HeraldofGreenLight) > 0 || CheckRemainInDeck(CardId.HeraldofOrangeLight) > 0)) ids.Add(CardId.Eva);
                if (((Bot.HasInHand(CardId.HeraldofOrangeLight) || Bot.HasInHand(CardId.HeraldofGreenLight))
                    && Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Fairy)) > 2)
                    || (!Bot.HasInHand(CardId.HeraldofOrangeLight) && !Bot.HasInHand(CardId.HeraldofGreenLight)))
                {
                    ids.Add(CardId.MudoratheSwordOracle);
                    ids.Add(CardId.KeldotheSacredProtector);
                }
                if (!activate_TearlamentsScream_2 && CheckRemainInDeck(CardId.TearlamentsSulliek) > 0) ids.Add(CardId.TearlamentsScream);
                if (!activate_TearlamentsSulliek_2) ids.Add(CardId.TearlamentsSulliek);
                if (!activate_TearlamentsReinoheart_2 && HasInList(cards, CardId.TearlamentsReinoheart)) ids.Add(CardId.TearlamentsReinoheart);
                if (!cards.Any(card => card != null && !card.HasRace(CardRace.Fairy)))
                {
                    ids.Add(CardId.HeraldofGreenLight);
                    ids.Add(CardId.HeraldofOrangeLight);
                }
                ids = ids.Distinct().ToList();
                IList<ClientCard> res = CardsIdToClientCards(ids, cards, false);
                List<ClientCard> temp = new List<ClientCard>(cards);
                foreach (var card in cards)
                {
                    if (temp.Count <= 1) break;
                    if ((!summoned && card.Id == CardId.DivineroftheHerald)
                        || card.Id == CardId.HeraldofOrangeLight || card.Id == CardId.HeraldofGreenLight
                        || (!summoned && card.Id == CardId.TearlamentsMerrli && !activate_TearlamentsHavnis_1))
                    {
                        temp.Remove(card);
                    }
                }
                temp.Sort(CardContainer.CompareCardAttack);
                return res.Count > 0 ? Util.CheckSelectCount(res, cards, min, max) : Util.CheckSelectCount(temp, cards, min, max);

            }
            if (hint == HintMsg.ToGrave && cards.Any(card => card != null && (card.Location == CardLocation.Deck || card.Location == CardLocation.Extra)) && min == 1 && max == 1)
            {
                List<int> cardsid = new List<int>();
                if (HasInList(cards, CardId.AgidotheAncientSentinel) && !activate_AgidotheAncientSentinel_2 && !AllActivated())
                {
                    if (HasInList(cards, CardId.KelbektheAncientVanguard) && !activate_KelbektheAncientVanguard_2 && Bot.HasInHand(CardId.AgidotheAncientSentinel))
                    {
                        cardsid.Add(CardId.KelbektheAncientVanguard);
                    }
                    else cardsid.Add(CardId.AgidotheAncientSentinel);
                }
                else if (HasInList(cards, CardId.KelbektheAncientVanguard) && !activate_KelbektheAncientVanguard_2 && !AllActivated())
                {
                    cardsid.Add(CardId.KelbektheAncientVanguard);
                }
                else if (HasInList(cards, CardId.ElderEntityNtss) && GetZoneCards(CardLocation.Onfield, Enemy).Count(card => card != null && !card.IsShouldNotBeTarget()) > 0)
                {
                    cardsid.Add(CardId.ElderEntityNtss);
                }
                if (HasInList(cards, CardId.Eva) && !activate_Eva &&
                    (CheckRemainInDeck(CardId.HeraldofOrangeLight) > 0 || CheckRemainInDeck(CardId.HeraldofGreenLight) > 0))
                    cardsid.Add(CardId.Eva);
                if (HasInList(cards, CardId.MudoratheSwordOracle)) cardsid.Add(CardId.MudoratheSwordOracle);
                if (HasInList(cards, CardId.KeldotheSacredProtector)) cardsid.Add(CardId.KeldotheSacredProtector);
                if (IsShouldSummonFusion())
                {
                    if (cards.Any(card => card != null && card.Id == CardId.TearlamentsHavnis) && !activate_TearlamentsHavnis_2) cardsid.Add(CardId.TearlamentsHavnis);
                    if (cards.Any(card => card != null && card.Id == CardId.TearlamentsMerrli) && !activate_TearlamentsMerrli_2) cardsid.Add(CardId.TearlamentsMerrli);
                    if (cards.Any(card => card != null && card.Id == CardId.TearlamentsScheiren) && !activate_TearlamentsScheiren_2) cardsid.Add(CardId.TearlamentsScheiren);
                }
                if (cards.Any(card => card != null && card.Id == CardId.TearlamentsSulliek) && !activate_TearlamentsSulliek_2) cardsid.Add(CardId.TearlamentsSulliek);
                if (cards.Any(card => card != null && card.Id == CardId.TearlamentsScream) && !activate_TearlamentsScream_2 && CheckRemainInDeck(CardId.TearlamentsSulliek) > 0) cardsid.Add(CardId.TearlamentsScream);
                IList<ClientCard> res = CardsIdToClientCards(cardsid, cards, false);
                if (res.Count > 0) no_fusion_card = res[0];
                return res.Count > 0 ? Util.CheckSelectCount(res, cards, min, max) : null;
            }
            if (hint == HintMsg.ToDeck && cards.Any(card => card != null && card.Location == CardLocation.Grave))
            {
                List<ClientCard> b_cards = new List<ClientCard>();
                List<ClientCard> e_cards = new List<ClientCard>();
                List<ClientCard> e_temp_1 = new List<ClientCard>();
                List<ClientCard> e_temp_2 = new List<ClientCard>();
                List<ClientCard> e_temp_3 = new List<ClientCard>();
                List<ClientCard> e_temp_4 = new List<ClientCard>();
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Controller == 0) b_cards.Add(card);
                    else e_cards.Add(card);
                }
                if (e_cards.Count <= 0 && Bot.Deck.Count > 2)
                {
                    return null;
                }
                int imax = e_cards.Count > max ? max : e_cards.Count < 0 ? min : e_cards.Count;
                if (Duel.CurrentChain == null || Duel.ChainTargets == null) return Util.CheckSelectCount(cards, cards, imax, imax);
                foreach (var card in Duel.CurrentChain)
                {
                    if (card == null || card.Controller == 0 || card.Location != CardLocation.Grave) continue;
                    if (cards.Contains(card))
                    {
                        if (key_no_send_to_deck_ids.Contains(card.Id)) continue;
                        else if (key_send_to_deck_ids.Contains(card.Id) && !e_temp_1.Contains(card)) e_temp_1.Add(card);
                        else if (!e_temp_2.Contains(card)) e_temp_2.Add(card);
                    }
                }
                foreach (var card in Duel.ChainTargets)
                {
                    if (card == null || card.Controller == 0 || card.Location != CardLocation.Grave) continue;
                    if (cards.Contains(card))
                    {
                        if (key_no_send_to_deck_ids.Contains(card.Id)) continue;
                        else if (key_send_to_deck_ids.Contains(card.Id) && !e_temp_1.Contains(card)) e_temp_1.Add(card);
                        else if (!e_temp_2.Contains(card)) e_temp_2.Add(card);
                    }
                }
                foreach (var card in cards)
                {
                    if (card == null || card.Controller == 0 || card.Location != CardLocation.Grave) continue;
                    if (key_send_to_deck_ids.Contains(card.Id) && !e_temp_1.Contains(card) && !e_temp_2.Contains(card)) e_temp_2.Add(card);
                }
                if (Bot.Deck.Count <= 0 && Duel.CurrentChain.Any(card => card != null && card.Controller == 0 && card.Id == CardId.ShaddollBeast))
                {
                    bot_send_to_deck_ids = new List<int> { CardId.TearlamentsScheiren, CardId.TearlamentsMerrli, CardId.TearlamentsHavnis };
                    IList<ClientCard> temp = CardsIdToClientCards(bot_send_to_deck_ids, b_cards);
                    if (temp.Count <= 0)
                    {
                        for (int i = 0; i < b_cards.Count; i++)
                        {
                            if (i >= 2) break;
                            if (b_cards[i] == null || e_temp_1.Contains(b_cards[i])) continue;
                            e_temp_1.Insert(0, b_cards[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (i >= 2) break;
                            if (temp[i] == null || e_temp_1.Contains(temp[i])) continue;
                            e_temp_1.Insert(0, temp[i]);
                        }
                    }
                }
                if (Bot.Deck.Count < 3)
                {
                    bot_send_to_deck_ids = new List<int>();
                    IList<ClientCard> temp = new List<ClientCard>();
                    if (b_cards.Count > 0 && Bot.ExtraDeck.Any(card => card != null && card.HasType(CardType.Fusion) && card.Id != CardId.ElderEntityNtss))
                    {
                        if (CheckRemainInDeck(CardId.TearlamentsScheiren) <= 0 && HasInList(cards, CardId.TearlamentsScheiren)) bot_send_to_deck_ids.Add(CardId.TearlamentsScheiren);
                        if (CheckRemainInDeck(CardId.TearlamentsMerrli) <= 0 && HasInList(cards, CardId.TearlamentsMerrli)) bot_send_to_deck_ids.Add(CardId.TearlamentsMerrli);
                        if (CheckRemainInDeck(CardId.TearlamentsHavnis) <= 0 && HasInList(cards, CardId.TearlamentsHavnis)) bot_send_to_deck_ids.Add(CardId.TearlamentsHavnis);
                    }
                    temp = CardsIdToClientCards(bot_send_to_deck_ids, b_cards);
                    if (temp.Count > 0)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (temp[i] == null || e_temp_4.Contains(temp[i]) || e_temp_1.Contains(temp[i])
                                || e_temp_2.Contains(temp[i])) continue;
                            e_temp_4.Add(temp[i]);
                        }
                    }
                    bot_send_to_deck_ids.Clear();
                    if (HasInList(cards, CardId.NaelshaddollAriel)) bot_send_to_deck_ids.Add(CardId.NaelshaddollAriel);
                    if (CheckRemainInDeck(CardId.KelbektheAncientVanguard) <= 0 && HasInList(cards, CardId.KelbektheAncientVanguard)) bot_send_to_deck_ids.Add(CardId.KelbektheAncientVanguard);
                    if (CheckRemainInDeck(CardId.AgidotheAncientSentinel) <= 0 && HasInList(cards, CardId.AgidotheAncientSentinel)) bot_send_to_deck_ids.Add(CardId.AgidotheAncientSentinel);
                    if (CheckRemainInDeck(CardId.HeraldofOrangeLight) <= 0 && HasInList(cards, CardId.HeraldofOrangeLight)) bot_send_to_deck_ids.Add(CardId.HeraldofOrangeLight);
                    if (CheckRemainInDeck(CardId.HeraldofGreenLight) <= 0 && HasInList(cards, CardId.HeraldofGreenLight)) bot_send_to_deck_ids.Add(CardId.HeraldofGreenLight);
                    temp = CardsIdToClientCards(bot_send_to_deck_ids, b_cards);
                    if (temp.Count > 0)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (temp[i] == null || e_temp_4.Contains(temp[i]) || e_temp_1.Contains(temp[i])
                                || e_temp_2.Contains(temp[i])) continue;
                            e_temp_4.Add(temp[i]);
                        }
                    }
                    foreach (var card in b_cards)
                    {
                        if (card == null || e_temp_4.Contains(card) || e_temp_1.Contains(card)
                                || e_temp_2.Contains(card)) continue;
                        e_temp_4.Add(card);
                    }
                }
                e_temp_3 = e_cards.Where(card => card != null && !e_temp_1.Contains(card) && !e_temp_2.Contains(card) && !e_temp_4.Contains(card)).ToList();
                e_temp_1.AddRange(e_temp_2);
                e_temp_1.AddRange(e_temp_4);
                e_temp_1.AddRange(e_temp_3);
                imax = e_temp_1.Count > max ? max : e_temp_1.Count < 0 ? min : e_temp_1.Count;
                return e_temp_1.Count > 0 ? Util.CheckSelectCount(e_temp_1, cards, imax, imax) : Util.CheckSelectCount(e_cards, cards, max, max);

            }
            if (hint == HintMsg.SpSummon && cards.Any(card => card != null && (card.Level == 2 || card.LinkCount == 2) && card.Location == CardLocation.Grave))
            {
                IList<ClientCard> res = new List<ClientCard>();
                List<int> ids = new List<int>();
                if (Duel.Player == 0)
                {
                    if (HasInList(cards, CardId.TearlamentsMerrli) && !activate_TearlamentsMerrli_1 && IsCanFusionSummon())
                    {
                        ids.Add(CardId.TearlamentsMerrli);
                    }
                    else if (HasInList(cards, CardId.DivineroftheHerald) && !activate_DivineroftheHerald)
                    {
                        ids.Add(CardId.DivineroftheHerald);
                    }
                    else if (HasInList(cards, CardId.TearlamentsMerrli) && !activate_TearlamentsMerrli_1)
                    {
                        ids.Add(CardId.TearlamentsMerrli);
                    }
                    else if (HasInList(cards, CardId.IP) && Bot.ExtraDeck.Count(card => card != null && card.LinkCount > 2) > 0)
                    {
                        ids.Add(CardId.IP);
                    }
                    else
                    {
                        ids = new List<int>() { CardId.DivineroftheHerald, CardId.TearlamentsMerrli, CardId.HeraldofOrangeLight, CardId.HeraldofGreenLight };
                    }
                }
                else
                {
                    if (HasInList(cards, CardId.DivineroftheHerald) && !activate_DivineroftheHerald && FusionDeckCheck()
                        && ((CheckRemainInDeck(CardId.AgidotheAncientSentinel) > 0 && !activate_AgidotheAncientSentinel_2)
                        || (CheckRemainInDeck(CardId.KelbektheAncientVanguard) > 0 && !activate_KelbektheAncientVanguard_2)))
                    {
                        ids.Add(CardId.DivineroftheHerald);
                    }
                    if (HasInList(cards, CardId.TearlamentsMerrli) && !activate_TearlamentsMerrli_1 &&
                        IsShouldSummonFusion(-1, -1, true) && FusionDeckCheck())
                    {
                        ids.Add(CardId.TearlamentsMerrli);
                    }
                    else if (HasInList(cards, CardId.DivineroftheHerald) && Bot.HasInExtra(CardId.ElderEntityNtss) && GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && !card.IsShouldNotBeTarget()))
                    {
                        ids.Add(CardId.DivineroftheHerald);
                    }
                    else if (HasInList(cards, CardId.IP) && Bot.ExtraDeck.Any(card => card != null && card.LinkCount > 2))
                    {
                        ids.Add(CardId.IP);
                    }
                    else
                    {
                        ids = new List<int>() { CardId.DivineroftheHerald, CardId.TearlamentsMerrli, CardId.HeraldofOrangeLight, CardId.HeraldofGreenLight };
                    }
                }
                res = CardsIdToClientCards(ids, cards, false);
                return res.Count > 0 ? Util.CheckSelectCount(res, cards, min, max) : null;
            }
            //(IS_YGOPRO && hint == HintMsg.Disable) || (!IS_YGOPRO && hint == HintMsg.Negate)
            if (IS_YGOPRO && hint == HintMsg.Disable)
            {
                if (chain_TearlamentsSulliek != null && cards.Contains(chain_TearlamentsSulliek))
                {
                    List<ClientCard> res = new List<ClientCard>() { chain_TearlamentsSulliek };
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                return null;
            }
            if (hint == HintMsg.Faceup && cards.Any(card => card != null && card.Controller == 1))
            {
                if (chain_PredaplantDragostapelia != null && cards.Contains(chain_PredaplantDragostapelia))
                {
                    List<ClientCard> res = new List<ClientCard>() { chain_PredaplantDragostapelia };
                    e_PredaplantDragostapelia_cards.Add(chain_PredaplantDragostapelia);
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                return null;
            }
            for (int i = 2; i >= 0; --i)
            {
                if (ran_fusion_mode_0[i])
                {
                    ran_fusion_mode_0[i] = false;
                    if (Duel.Player == 0)
                    {
                        List<ClientCard> res0 = cards.Where(card => card != null && card.Id == CardId.PredaplantDragostapelia).ToList();
                        if (res0.Count > 0 && Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true))
                        {
                            return Util.CheckSelectCount(res0, cards, min, max);
                        }
                        res0 = cards.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                        if (res0.Count > 0 && ((!activate_TearlamentsKitkallos_1 &&
                            ((!activate_TearlamentsScheiren_2 && CheckRemainInDeck(CardId.TearlamentsScheiren) > 0)
                            || (!activate_TearlamentsHavnis_2 && CheckRemainInDeck(CardId.TearlamentsHavnis) > 0)
                            || (!activate_TearlamentsMerrli_2 && CheckRemainInDeck(CardId.TearlamentsMerrli) > 0))) || !activate_TearlamentsKitkallos_2))
                        {
                            TearlamentsKitkallos_summoned = true;
                            return Util.CheckSelectCount(res0, cards, min, max);
                        }
                        res0 = cards.Where(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart).ToList();
                        //if (GetZoneCards(CardLocation.Onfield, Enemy).Count(card => card != null && !card.IsShouldNotBeTarget()) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart)
                        //    && !activate_TearlamentsKaleidoHeart_1 && (!Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) || (Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) && activate_PrimevalPlanetPerlereino_2)))
                        if(res0.Count>0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart))
                        {
                            ran_fusion_mode_3[i] = true;
                            return Util.CheckSelectCount(res0, cards, min, max);
                        }
                        res0 = cards.Where(card => card != null && card.Id == CardId.TearlamentsRulkallos).ToList();
                        if (Bot.Graveyard.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0 ||
                            Bot.MonsterZone.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos && card.IsFaceup()) > 0 && res0.Count > 0)
                        {
                            activate_TearlamentsRulkallos_2 = false;
                            return Util.CheckSelectCount(res0, cards, min, max);
                        }
                        res0 = cards.Where(card => card != null && card.Id == CardId.PredaplantDragostapelia).ToList();
                        //if (res0.Count > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.PredaplantDragostapelia) && Enemy.GetMonsters().Any(card => card != null && card.HasType(CardType.Effect) && !card.IsDisabled() && card.IsFaceup() && !card.IsShouldNotBeTarget()))
                        if (res0.Count > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.PredaplantDragostapelia))
                        {
                            return Util.CheckSelectCount(res0, cards, min, max);
                        }
                        List<ClientCard> res1 = cards.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                        if (res1.Count > 0)
                        {
                            List<ClientCard> material = GetDefaultMaterial(fusionMaterial);
                            if (((material.Count(mcard => mcard != null && mcard.Id == CardId.TearlamentsKitkallos && mcard.Location == CardLocation.MonsterZone) > 0 &&
                                activate_TearlamentsKitkallos_1) || (material.Count(mcard => mcard != null && mcard.Id == CardId.TearlamentsKitkallos && mcard.Location == CardLocation.MonsterZone) > 1)) &&
                               material.Count <= 2)
                            {
                                if (res0.Count > 0)
                                {
                                    activate_TearlamentsRulkallos_2 = false;
                                    return Util.CheckSelectCount(res0, cards, min, max);
                                }
                                TearlamentsKitkallos_summoned = true;
                                return Util.CheckSelectCount(res1, cards, min, max);
                            }
                            return Util.CheckSelectCount(res1, cards, min, max);
                        }

                    }
                    else
                    {
                        List<ClientCard> res = cards.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                        if (res.Count > 0 && !activate_TearlamentsKitkallos_1 &&
                            ((!activate_TearlamentsScheiren_2 && CheckRemainInDeck(CardId.TearlamentsScheiren) > 0)
                            || (!activate_TearlamentsHavnis_2 && CheckRemainInDeck(CardId.TearlamentsHavnis) > 0)
                            || (!activate_TearlamentsMerrli_2 && CheckRemainInDeck(CardId.TearlamentsMerrli) > 0)) && Bot.GetMonstersInMainZone().Count < 4)
                        {
                            TearlamentsKitkallos_summoned = true;
                            return Util.CheckSelectCount(res, cards, min, max);
                        } 
                        res = cards.Where(card => card != null && card.Id == CardId.TearlamentsRulkallos).ToList();
                        if (res.Count > 0) return Util.CheckSelectCount(res, cards, min, max);
                        res = cards.Where(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart).ToList();
                        if (res.Count>0 && GetZoneCards(CardLocation.Onfield, Enemy).Count(card => card != null && !card.IsShouldNotBeTarget()) > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart)
                           && !activate_TearlamentsKaleidoHeart_1 && (!Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) || (Bot.HasInSpellZone(CardId.PrimevalPlanetPerlereino, true, true) && activate_PrimevalPlanetPerlereino_2)))
                        {
                            ran_fusion_mode_3[i] = true;
                            return Util.CheckSelectCount(res, cards, min, max);
                        }
                        res = cards.Where(card => card != null && card.Id == CardId.ElShaddollWinda).ToList();
                        if (res.Count > 0) return Util.CheckSelectCount(res, cards, min, max);
                        res = cards.Where(card => card != null && card.Id == CardId.PredaplantDragostapelia).ToList();
                        if (res.Count > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.PredaplantDragostapelia))
                        {
                            return Util.CheckSelectCount(res, cards, min, max);
                        }
                        res = cards.Where(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart).ToList();
                        if (res.Count > 0 && IsShouldSummonFusion(-1, -1, false, (int)Flag.TearlamentsKaleidoHeart))
                        {
                            return Util.CheckSelectCount(res, cards, min, max);
                        }
                        res = cards.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList();
                        if (res.Count > 0)
                        {
                            TearlamentsKitkallos_summoned = true;
                            return Util.CheckSelectCount(res, cards, min, max);
                        }
                        return null;
                    }
                }
                if (mcard_0[i] == null) continue;
                if (cards.Contains(mcard_0[i]))
                {
                    List<ClientCard> res = new List<ClientCard>() { mcard_0[i] };
                    mcard_0[i] = null;
                    return Util.CheckSelectCount(res, cards, min, max);
                }
                else
                {
                    return null;
                }
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        private bool IsCanFusionSummon()
        {
            if ((!activate_TearlamentsMerrli_2 && CheckRemainInDeck(CardId.TearlamentsMerrli) > 0)
                || (!activate_TearlamentsHavnis_2 && CheckRemainInDeck(CardId.TearlamentsHavnis) > 0)
                || (!activate_TearlamentsScheiren_2 && CheckRemainInDeck(CardId.TearlamentsScheiren) > 0))
                return Bot.ExtraDeck.Count(card => card != null && card.HasType(CardType.Fusion) && card.Id != CardId.ElderEntityNtss) > 0;
            return false;

        }
        private bool BaronnedeFleurSummon_2()
        {
            if (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2) return false;
            List<ClientCard> scards = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.DivineroftheHerald && card.IsFaceup()).ToList();
            List<ClientCard> mcards = Bot.GetMonsters().Where(card => card != null && !card.HasType(CardType.Tuner) && card.IsFaceup()).ToList();
            if (scards.Any(card => card != null && card.Level == 2) && mcards.Count(card => card != null && card.Level == 4) > 1)
            {
                List<ClientCard> s = scards.Where(card => card != null && card.Level == 2 && card.Id == CardId.DivineroftheHerald && card.IsFaceup()).ToList();
                List<ClientCard> res = new List<ClientCard>() { s.FirstOrDefault() };
                res.AddRange(mcards.Where(card => card != null && card.Level == 4 && card.IsFaceup()));
                AI.SelectMaterials(res);
                SetSpSummon();
                return true;
            }
            return false;
        }
        private bool BaronnedeFleurSummon()
        {
            List<ClientCard> scards = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.DivineroftheHerald && card.IsFaceup()).ToList();
            List<ClientCard> mcards = Bot.GetMonsters().Where(card => card != null && !card.HasType(CardType.Synchro) && card.IsFaceup()).ToList();
            if (scards.Any(card => card != null && card.Level == 2) && mcards.Any(card => card != null && card.Level == 8 && card.Id != CardId.TearlamentsRulkallos && card.Id != CardId.PredaplantDragostapelia)) { SetSpSummon(); return true; }
            else if (scards.Any(card => card != null && card.Level == 5) && mcards.Any(card => card != null && card.Level == 5))
            {
                List<ClientCard> s = Bot.GetMonsters().Where(card => card != null && card.Level == 5 && card.Id == CardId.DivineroftheHerald && card.IsFaceup()).ToList();
                if (s.Count >= 0)
                {
                    List<ClientCard> res = new List<ClientCard>() { s.FirstOrDefault() };
                    res.AddRange(Bot.GetMonsters().Where(card => card != null && card.Level == 5 && card.Id == CardId.ElShaddollWinda && card.IsFaceup()));
                    res.AddRange(Bot.GetMonsters().Where(card => card != null && card.Level == 5 && card.IsFaceup()));
                    AI.SelectMaterials(res);
                    SetSpSummon();
                    return true;
                }
            }
            else if (scards.Any(card => card != null && card.Level == 6) && mcards.Any(card => card != null && card.Level == 4)) { SetSpSummon(); return true; }
            return false;

        }
        private bool SprightElfEffect()
        {
            ClientCard card = Util.GetLastChainCard();
            if (card != null && card.Controller == 0) return false;
            SetSpSummon();
            return true;
        }
        private bool FusionDeckCheck()
        {
            return (CheckRemainInDeck(CardId.TearlamentsHavnis) > 0 && !activate_TearlamentsHavnis_2)
                    || (CheckRemainInDeck(CardId.TearlamentsMerrli) > 0 && !activate_TearlamentsMerrli_2)
                    || (CheckRemainInDeck(CardId.TearlamentsScheiren) > 0 && !activate_TearlamentsScheiren_2);
        }
        private List<int> GetCardsIdSendToHand()
        {
            List<int> ids = new List<int>();
            if (!activate_TearlamentsScheiren_1 && !Bot.HasInHand(CardId.TearlamentsScheiren) && CheckRemainInDeck(CardId.TearlamentsScheiren) > 0 && Bot.Hand.Count(card => card != null && card.HasType(CardType.Monster)) > 0) ids.Add(CardId.TearlamentsScheiren);
            if (!activate_TearlamentsMerrli_1 && !Bot.HasInHand(CardId.TearlamentsMerrli) && CheckRemainInDeck(CardId.TearlamentsMerrli) > 0 && (!summoned ||!activate_TearlamentsKitkallos_2) ) ids.Add(CardId.TearlamentsMerrli);
            if (!activate_TearlamentsReinoheart_1 && !Bot.HasInHand(CardId.TearlamentsReinoheart) && CheckRemainInDeck(CardId.TearlamentsReinoheart) > 0) ids.Add(CardId.TearlamentsReinoheart);
            if (!activate_TearlamentsScheiren_1 && !Bot.HasInHand(CardId.TearlamentsScheiren) && CheckRemainInDeck(CardId.TearlamentsScheiren) > 0) ids.Add(CardId.TearlamentsScheiren);
            if (!activate_TearlamentsHavnis_1 && !Bot.HasInHand(CardId.TearlamentsHavnis) && CheckRemainInDeck(CardId.TearlamentsHavnis) > 0)
            {
                if (Duel.Player == 0 && (Duel.Phase != DuelPhase.End || AllActivated())) ids.Add(CardId.TearlamentsHavnis);
                else ids.Insert(0, CardId.TearlamentsHavnis);
            }
            ids.AddRange(new List<int>() { CardId.TearlamentsScheiren, CardId.TearlamentsMerrli, CardId.TearlamentsHavnis, CardId.TearlamentsReinoheart });
            return ids;
        }
        public int CheckRemainInDeck(int id)
        {
            switch (id)
            {
                case CardId.ShaddollBeast:
                    return Bot.GetRemainingCount(CardId.ShaddollBeast, 1);
                case CardId.ShaddollDragon:
                    return Bot.GetRemainingCount(CardId.ShaddollDragon, 1);
                case CardId.TearlamentsScheiren:
                    return Bot.GetRemainingCount(CardId.TearlamentsScheiren, 3);
                case CardId.TearlamentsReinoheart:
                    return Bot.GetRemainingCount(CardId.TearlamentsReinoheart, 2);
                case CardId.KelbektheAncientVanguard:
                    return Bot.GetRemainingCount(CardId.KelbektheAncientVanguard, 3);
                case CardId.MudoratheSwordOracle:
                    return Bot.GetRemainingCount(CardId.MudoratheSwordOracle, 3);
                case CardId.AgidotheAncientSentinel:
                    return Bot.GetRemainingCount(CardId.AgidotheAncientSentinel, 3);
                case CardId.KeldotheSacredProtector:
                    return Bot.GetRemainingCount(CardId.KeldotheSacredProtector, 2);
                case CardId.NaelshaddollAriel:
                    return Bot.GetRemainingCount(CardId.NaelshaddollAriel, 1);
                case CardId.TearlamentsHavnis:
                    return Bot.GetRemainingCount(CardId.TearlamentsHavnis, 3);
                case CardId.TearlamentsMerrli:
                    return Bot.GetRemainingCount(CardId.TearlamentsMerrli, 3);
                case CardId.DivineroftheHerald:
                    return Bot.GetRemainingCount(CardId.DivineroftheHerald, 3);
                case CardId.HeraldofOrangeLight:
                    return Bot.GetRemainingCount(CardId.HeraldofOrangeLight, 3);
                case CardId.HeraldofGreenLight:
                    return Bot.GetRemainingCount(CardId.HeraldofGreenLight, 3);
                case CardId.Eva:
                    return Bot.GetRemainingCount(CardId.Eva, 1);
                case CardId.TearlamentsScream:
                    return Bot.GetRemainingCount(CardId.TearlamentsScream, 1);
                case CardId.PrimevalPlanetPerlereino:
                    return Bot.GetRemainingCount(CardId.PrimevalPlanetPerlereino, 2);
                case CardId.TearlamentsSulliek:
                    return Bot.GetRemainingCount(CardId.TearlamentsSulliek, 2);
                default:
                    return 0;
            }
        }
        private bool PredaplantDragostapeliaEffect()
        {
            ClientCard card = Util.GetLastChainCard();
            if (card != null && card.Controller != 0 && card.Location == CardLocation.MonsterZone && !card.IsShouldNotBeTarget() && !e_PredaplantDragostapelia_cards.Contains(card))
            {
                chain_PredaplantDragostapelia = card;
                return true;
            }
            return false;
        }
        private bool UnderworldGoddessoftheClosedWorldSummon_2()
        {
            return UnderworldGoddessoftheClosedWorldLinkSummon(false);
        }
        private bool UnderworldGoddessoftheClosedWorldLinkSummon(bool filter = true)
        {
            if (Duel.Turn == 1 || Enemy.GetMonsterCount() <= 0) return false;
            List<ClientCard> e_cards = Enemy.GetMonsters().Where(card => card != null && card.IsFaceup() && card.IsAttack()).ToList();
            List<ClientCard> b_cards = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.IsAttack()).ToList();
            if ((e_cards.Count <= 0 || b_cards.Count <= 0) && Enemy.MonsterZone.GetDangerousMonster() == null) return false;
            e_cards.Sort(CardContainer.CompareCardAttack);
            e_cards.Reverse();
            b_cards.Sort(CardContainer.CompareCardAttack);
            b_cards.Reverse();
            if ((e_cards[0].Attack > b_cards[0].Attack && (e_cards[0].IsShouldNotBeTarget() || e_cards[0].Attack >= 2500)) || Enemy.MonsterZone.GetDangerousMonster() != null)
            {
                List<ClientCard> e_materials = new List<ClientCard>();
                List<ClientCard> m_materials = new List<ClientCard>();
                List<ClientCard> resMaterials = new List<ClientCard>();
                foreach (var card in Enemy.GetMonsters())
                {
                    if (card != null && card.HasType(CardType.Effect) && card.IsFaceup())
                        e_materials.Add(card);
                }
                if (e_materials.Count() <= 0) return false;
                List<ClientCard> mcards = Bot.GetMonsters();
                mcards.Sort(CardContainer.CompareCardAttack);
                foreach (var card in mcards)
                {
                    if (card == null || card.IsFacedown()) continue;
                    if (card.Id == CardId.SprightElf && summon_SprightElf) continue;
                    if (card.LinkCount < 3 && !(no_link_ids.Contains(card.Id) & filter) && card.IsFaceup() && card.HasType(CardType.Effect))
                    {
                        if (card.Id == CardId.ElShaddollWinda) m_materials.Insert(0, card);
                        else m_materials.Add(card);
                    }
                }
                if (m_materials.Count() < 3) return false;
                int link_count = 0;
                int e_link_count = 0;
                e_materials.Sort(CardContainer.CompareCardAttack);
                e_materials.Reverse();
                foreach (var card in e_materials)
                {
                    if (!resMaterials.Contains(card))
                        resMaterials.Add(card);
                    e_link_count += (card.HasType(CardType.Link)) ? (card.LinkCount == 2 ? 2 : 1) : 1;
                    if (e_link_count >= 1) break;
                }
                if (e_link_count <= 0) return false;
                link_count += e_link_count;
                foreach (var card in m_materials)
                {
                    if (e_link_count <= 1)
                    {
                        if (!resMaterials.Contains(card) && card.LinkCount < 3)
                        {
                            resMaterials.Add(card);
                            link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                            if (link_count >= 5) break;
                        }
                    }
                    else
                    {
                        resMaterials.Add(card);
                        link_count += 1;
                        if (link_count >= 5) break;
                    }
                }
                if (link_count >= 5) { AI.SelectMaterials(resMaterials); SetSpSummon(); return true; }
            }
            return false;
        }
        private bool TearlamentsKaleidoHeartEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                activate_TearlamentsKaleidoHeart_1 = true;
                return DestoryEnemyCard();
            }
            else
            {
                activate_TearlamentsKaleidoHeart_2 = true;
                return true;
            }
        }
        private bool UnderworldGoddessoftheClosedWorldSummon()
        {
            if (Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true))
            {
                return UnderworldGoddessoftheClosedWorldLinkSummon();
            }
            return false;
        }
        private bool UnderworldGoddessoftheClosedWorldSummon_3()
        {
            return UnderworldGoddessoftheClosedWorldLinkSummon();
        }
        private bool SpellActivate()
        {
            return Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.IsFacedown());
        }
        private bool SprightElfSummon_2()
        {
            List<ClientCard> key_cards = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.ElShaddollWinda && !card.IsDisabled() && card.IsFaceup()).ToList();
            if (key_cards.Count <= 0 || key_cards.Count > 1 || !IsAvailableLinkZone()) return false;
            List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.Level == 2 && card.Id != CardId.ElShaddollWinda && card.IsFaceup()).ToList();
            if (cards.Count <= 0) return false;
            cards.Sort(CardContainer.CompareCardAttack);
            cards.Insert(0, key_cards[0]);
            AI.SelectMaterials(cards);
            SetSpSummon();
            return true;
        }
        private bool FADawnDragsterSummon()
        {
            List<ClientCard> key_cards = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.ElShaddollWinda && !card.IsDisabled() && card.IsFaceup()).ToList();
            if (key_cards.Count <= 0 || key_cards.Count > 1 ) return false;
            SetSpSummon();
            IList<ClientCard> cards = CardsIdToClientCards(new List<int>() { CardId.DivineroftheHerald }, Bot.GetMonsters().Where(card => card != null && card.IsFaceup()).ToList());
            if (cards.Count > 0) key_cards.Insert(0, cards[0]);
            AI.SelectMaterials(key_cards);
            return true;
        }
        private bool IPSummon_2()
        {
            if ((Bot.GetMonsterCount() <= 2 && Bot.Hand.Count<=2 && !Bot.HasInHand(CardId.TearlamentsScheiren) && !activate_TearlamentsScheiren_1&& !Bot.HasInHand(CardId.TearlamentsMerrli) && !activate_TearlamentsMerrli_1 ) || AllActivated() ) return false;
            List<ClientCard> key_cards = Bot.GetMonsters().Where(card => card != null && card.Id == CardId.ElShaddollWinda && !card.IsDisabled() && card.IsFaceup()).ToList();
            if (key_cards.Count <= 0 || key_cards.Count > 1 || !IsAvailableLinkZone()) return false;
            List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && !card.HasType(CardType.Link) && !no_link_ids.Contains(card.Id) && card.IsFaceup()).ToList();
            cards.Sort(CardContainer.CompareCardAttack);
            cards.Insert(0, key_cards[0]);
            AI.SelectMaterials(cards);
            SetSpSummon();
            return true;
        }
        private bool SprightElfSummon()
        {
            if (!IsAvailableLinkZone()) return false;
            List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card=>card != null && card.IsFaceup()).ToList();
            cards.Sort(CardContainer.CompareCardAttack);
            List<ClientCard> materials1 = new List<ClientCard>();
            List<ClientCard> materials2 = new List<ClientCard>();
            List<ClientCard> materials3 = new List<ClientCard>();
            List<ClientCard> materials4 = new List<ClientCard>();
            foreach (var card in cards)
            {
                if (card == null) continue;
                if (card.Level == 2 && materials1.Count <= 0) materials1.Add(card);
                else if(link_card != null && card == link_card) materials2.Insert(0, link_card);
                else if (!card.IsDisabled() && no_link_ids.Contains(card.Id)) materials4.Add(card);
                else
                { 
                    if(card.Level <= 2) materials2.Add(card);
                    else materials3.Add(card);
                }

            }
            if (materials1.Count <= 0 || materials2.Count + materials3.Count <= 0) return false;
            materials3.Sort(CardContainer.CompareCardLevel);
            materials3.Reverse();
            materials1.AddRange(materials2);
            materials1.AddRange(materials3);
            summon_SprightElf = true;
            AI.SelectMaterials(materials1);
            SetSpSummon();
            return true;
        }
        private bool AbyssDwellerSummon()
        {
            if (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2) return false;
            SetSpSummon();
            return true;
        }
        private bool AbyssDwellerSummon_2()
        {
            if (Bot.GetMonsters().Any(card => card != null && card.IsFaceup() && (card.Id == CardId.TearlamentsRulkallos || card.Id == CardId.TearlamentsKaleidoHeart))) return true;
            return false;
        }
        private bool IPEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (!Bot.HasInExtra(CardId.KnightmareUnicorn) && !Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax) && !Bot.HasInExtra(CardId.UnderworldGoddessoftheClosedWorld)) return false;
            List<ClientCard> m = new List<ClientCard>();
            List<ClientCard> pre_m = new List<ClientCard>();
            if (Bot.HasInExtra(CardId.UnderworldGoddessoftheClosedWorld))
            {
                List<ClientCard> e_cards = Enemy.GetMonsters().Where(card => card != null && card.IsFaceup() && card.IsAttack()).ToList();
                List<ClientCard> b_cards = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.IsAttack()).ToList();
                if (e_cards.Count > 0 && b_cards.Count > 0)
                {
                    e_cards.Sort(CardContainer.CompareCardAttack);
                    e_cards.Reverse();
                    b_cards.Sort(CardContainer.CompareCardAttack);
                    b_cards.Reverse();
                    if ((e_cards[0].Attack > b_cards[0].Attack && (e_cards[0].IsShouldNotBeTarget() || e_cards[0].Attack >= 2500)) || Enemy.MonsterZone.GetDangerousMonster() != null)
                    {
                        pre_m = Bot.GetMonsters().Where(card => card != null && card != Card && card.IsFaceup() && ((card.HasType(CardType.Link) && card.LinkCount < 3) || (card.HasType(CardType.Fusion | CardType.Xyz | CardType.Synchro))) && (!no_link_ids.Contains(card.Id) || card.IsDisabled())).ToList();
                        List<ClientCard> pre_m2 = new List<ClientCard>();
                        pre_m2.Add(Enemy.MonsterZone.GetDangerousMonster() == null ? e_cards[0] : Enemy.MonsterZone.GetDangerousMonster());
                        pre_m2.AddRange(e_cards);
                        int link_count = 0;
                        foreach (var card in pre_m)
                        {
                            if (card == null || (card.Id == CardId.SprightElf && summon_SprightElf)) continue;
                            link_count += 1;
                            m.Add(card);
                            if (link_count >= 2) break;
                        }
                        if (link_count >= 2)
                        {
                            AI.SelectCard(CardId.UnderworldGoddessoftheClosedWorld);
                            m.Insert(0, Card);
                            m.Add(pre_m2.FirstOrDefault());
                            AI.SelectMaterials(m);
                            return true;
                        }
                    }
                }
            }
            if (Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax))
            {
                m.Clear();
                pre_m = Bot.GetMonsters().Where(card => card != null && card != Card && card.IsFaceup() && ((card.HasType(CardType.Link) && card.LinkCount < 3) || (card.HasType(CardType.Fusion|CardType.Xyz|CardType.Synchro))) && (!no_link_ids.Contains(card.Id) || card.IsDisabled())).ToList();
                if (pre_m.Count > 0)
                {
                    pre_m.Sort(CardContainer.CompareCardAttack);
                    int link_count = 0;
                    foreach (var card in pre_m)
                    {
                        if (card == null || (card.Id == CardId.SprightElf && summon_SprightElf)) continue;
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        m.Add(card);
                        if (link_count >= 2) break;
                    }
                    if (link_count >= 2)
                    {
                        AI.SelectCard(CardId.MekkKnightCrusadiaAvramax);
                        m.Insert(0, Card);
                        AI.SelectMaterials(m);
                        return true;
                    }
                }
            }
            if (Bot.HasInExtra(CardId.KnightmareUnicorn))
            {
                List<ClientCard> pre_cards = GetZoneCards(CardLocation.Onfield,Enemy);
                if (Bot.Hand.Count > 0 && pre_cards.Count(card => card != null && !card.IsShouldNotBeTarget()) > 0)
                {
                    m.Clear();
                    pre_m = Bot.GetMonsters().Where(card => card != null && card != Card &&  card.IsFaceup() && !no_link_ids.Contains(card.Id) && card.Id != Card.Id).ToList();
                    if (pre_m.Count > 0)
                    {
                        pre_m.Sort(CardContainer.CompareCardAttack);
                        int link_count = 0;
                        foreach (var card in pre_m)
                        {
                            if (card == null || (card.Id == CardId.SprightElf && summon_SprightElf)) continue;
                            link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                            m.Add(card);
                            if (link_count >= 1) break;
                        }
                        if (link_count >= 1)
                        {
                            AI.SelectCard(CardId.KnightmareUnicorn);
                            m.Insert(0, Card);
                            AI.SelectMaterials(m);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool IPSummon()
        {
            if (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2) return false;
            if (!Bot.HasInMonstersZone(CardId.SprightElf) || Bot.GetMonsterCount() <= 2) return false;
            if (Bot.ExtraDeck.Count(card => card != null && card.LinkCount > 2) <= 0 || !IsAvailableLinkZone()) return false;
            List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup() && !card.HasType(CardType.Link)).ToList();
            List<ClientCard> materials = new List<ClientCard>();
            foreach (var card in cards)
            {
                if (card == null || (!card.IsDisabled() && no_link_ids.Contains(card.Id))
                    || card.LinkCount >= 2) continue;
                if (link_card != null && card == link_card) materials.Insert(0, link_card);
                else materials.Add(card);
            }
            if (materials.Count <= 1) return false;
            materials.Sort(CardContainer.CompareCardAttack);
            AI.SelectMaterials(materials);
            SetSpSummon();
            return true;
        }
        private bool KnightmareUnicornEffect()
        {
            List<ClientCard> cards = GetZoneCards(CardLocation.Onfield,Enemy);
            cards = cards.Where(card => card != null && !card.IsShouldNotBeTarget() && (tgcard == null || card != tgcard)).ToList();
            if (cards.Count <= 0) return false;
            List<int> ids = new List<int>();
            if (!activate_KelbektheAncientVanguard_2) ids.Add(CardId.KelbektheAncientVanguard);
            if (!activate_AgidotheAncientSentinel_2) ids.Add(CardId.AgidotheAncientSentinel);
            ids.AddRange(new List<int>() { CardId.MudoratheSwordOracle, CardId.KeldotheSacredProtector, CardId.ShaddollBeast, CardId.ShaddollDragon, CardId.TearlamentsSulliek, CardId.Eva, CardId.TearlamentsReinoheart });
            cards.Sort(CardContainer.CompareCardAttack);
            cards.Reverse();
            AI.SelectCard(ids);
            AI.SelectNextCard(cards);
            return true;

        }
        private bool MekkKnightCrusadiaAvramaxSummon()
        {
            if (!IsAvailableLinkZone() || (Duel.Turn > 1 && Duel.Phase < DuelPhase.Main2)) return false;
            if (Bot.HasInMonstersZone(CardId.SprightElf, false, false, true) && !summon_SprightElf
                && Bot.HasInMonstersZone(CardId.IP, false, false, true))
            {
                List<ClientCard> m1 = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.Id == CardId.SprightElf).ToList();
                List<ClientCard> m2 = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.Id == CardId.IP).ToList();
                if (m1.Count <= 0 || m2.Count <= 0) return false;
                List<ClientCard> res = new List<ClientCard>() { m1[0] };
                res.Add(m2[0]);
                AI.SelectMaterials(res);
                SetSpSummon();
                return true;
            }
            else
            {
                List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.LinkCount != 2 &&card.IsFaceup() && (!no_link_ids.Contains(card.Id) || card.IsDisabled())
                                                                && card.IsExtraCard() && card.Id != CardId.KnightmareUnicorn).ToList();
                List<ClientCard> cards2 = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.Id == CardId.KnightmareUnicorn).ToList();
                if (cards.Count <= 0 || cards2.Count <= 0) return false;
                cards.Sort(CardContainer.CompareCardAttack);
                List<ClientCard> res = new List<ClientCard>();
                res.Add(cards.FirstOrDefault());
                res.Add(cards.FirstOrDefault());
                AI.SelectMaterials(res);
                SetSpSummon();
                return true;
            }
        }
        private bool MekkKnightCrusadiaAvramaxEffect()
        {
            if (Card.Location == CardLocation.MonsterZone) return true;
            else return DestoryEnemyCard();
        }
        private bool KnightmareUnicornSummon()
        {
            if (Bot.Hand.Count <= 0) return false;
            if (!IsAvailableLinkZone()) return false;
            List<ClientCard> ecards = GetZoneCards(CardLocation.Onfield, Enemy);
            if (ecards.Count(card => card != null && !card.IsShouldNotBeTarget()) <= 0) return false;
            List<ClientCard> tmepMaterials = new List<ClientCard>();
            List<ClientCard> resMaterials = new List<ClientCard>();
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null || card.IsFacedown() || (card.Id == CardId.SprightElf && summon_SprightElf)
                    || (no_link_ids.Contains(card.Id)) || card.Id == CardId.TearlamentsKitkallos) continue;
                if (tmepMaterials.Count(_card => _card != null && _card.Id == card.Id) <= 0)
                    tmepMaterials.Add(card);
            }
            int link_count = 0;
            tmepMaterials.Sort(CardContainer.CompareCardAttack);
            List<ClientCard> materials = new List<ClientCard>();
            List<ClientCard> link_materials = tmepMaterials.Where(card => card != null && card.LinkCount == 2).ToList();
            List<ClientCard> normal_materials = tmepMaterials.Where(card => card != null && card.LinkCount != 2).ToList();
            if (link_materials.Count() >= 1)
            {
                materials.Add(link_materials.FirstOrDefault());
                materials.AddRange(normal_materials);
            }
            else
            {
                materials.AddRange(normal_materials);
                materials.AddRange(link_materials);
            }
            if (materials.Count(card => card != null && card.LinkCount >= 2) > 1
                && materials.Count(card => card != null && card.LinkCount < 2) < 1) return false;
            foreach (var card in materials)
            {
                if (!resMaterials.Contains(card) && card.LinkCount < 3)
                {
                    resMaterials.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                    if (link_count >= 3) break;
                }
            }
            if (link_count >= 3) { AI.SelectMaterials(resMaterials);SetSpSummon(); return true; }
            return false;
        }
        private bool BaronnedeFleurEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.BaronnedeFleur, 0))
            {
                return DestoryEnemyCard();
            }
            if (ActivateDescription == Util.GetStringId(CardId.BaronnedeFleur, 1))
            {
                return Duel.LastChainPlayer != 0;
            }
            return false;
          
        }
        private bool PrimevalPlanetPerlereinoEffect()
        {
            if (SpellActivate()) return true;
            else
            {
                ClientCard card = Util.GetLastChainCard();
                if (card != null && card.Controller == 0 && card.Id == CardId.TearlamentsKaleidoHeart) return false;
                return DestoryEnemyCard();
            }
        }

        private bool DestoryEnemyCard()
        {
            if (Duel.Player == 0 && Card.Id == CardId.PrimevalPlanetPerlereino && !on_chaining_cards.Any(ccard => ccard != null && !ccard.IsDisabled() && ccard.Controller == 0 && ccard.Id == CardId.TearlamentsKitkallos)
                && Bot.HasInMonstersZone(CardId.TearlamentsKitkallos) && !activate_TearlamentsKitkallos_3
                && !AllActivated())
            {
                List<ClientCard> temp = new List<ClientCard>();
                List<ClientCard> temp2 = new List<ClientCard>();
                foreach (var ccard in Bot.GetMonsters())
                {
                    if (ccard == null) continue;
                    if (ccard.Id == CardId.TearlamentsKitkallos && ccard.IsDisabled()) temp.Add(ccard);
                    else if(ccard.Id == CardId.TearlamentsKitkallos) temp2.Add(ccard);
                }
                temp.AddRange(temp2);
                AI.SelectCard(temp);
                activate_PrimevalPlanetPerlereino_2 = true;
                return true;
            }
            ClientCard card = Util.GetProblematicEnemyMonster(0, true);
            if (card != null && (tgcard == null || tgcard != card))
            {
                AI.SelectCard(card);
                tgcard = card;
                return true;
            }
            card = Util.GetBestEnemySpell(true);
            if (card != null && (tgcard == null || tgcard != card))
            {
                AI.SelectCard(card);
                tgcard = card;
                return true;
            }
            List<ClientCard> cards = GetZoneCards(CardLocation.Onfield, Enemy);
            cards = cards.Where(tcard => tcard != null && !tcard.IsShouldNotBeTarget() && (tgcard == null || tcard != tgcard)).ToList();
            if (cards.Count <= 0) return false;
            cards.Sort(CardContainer.CompareCardAttack);
            cards.Reverse();
            tgcard = cards[0];
            AI.SelectCard(cards);
            if (Card.Id == CardId.PrimevalPlanetPerlereino) activate_PrimevalPlanetPerlereino_2 = true;
            return true;
        }
        private bool ElderEntityNtssEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return DestoryEnemyCard();
            }
            else return true;
           
        }
        private bool DivineroftheHeraldSummon()
        {
            if ((CheckRemainInDeck(CardId.KelbektheAncientVanguard) > 0 && !activate_KelbektheAncientVanguard_2)
                || (CheckRemainInDeck(CardId.AgidotheAncientSentinel) > 0 && !activate_AgidotheAncientSentinel_2))
            { 
                summoned = true;
                return true;
            }
            return false;
        }
        private bool EvaEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            List<ClientCard> cards = Bot.GetGraveyardMonsters().Where(card => card != null && card.HasAttribute(CardAttribute.Light) && card.HasRace(CardRace.Fairy) && card != Card).ToList();
            if (cards.Count <= 0) return false;
            activate_Eva = true;
            return true; 
        }
        private bool DivineroftheHeraldEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player == 0)
                {
                    if (!IsCanSpSummon() && CheckRemainInDeck(CardId.MudoratheSwordOracle) <= 0 && CheckRemainInDeck(CardId.KeldotheSacredProtector) <= 0) return false;
                    if ((AllActivated() || Bot.ExtraDeck.Count(card=>card!=null && card.HasType(CardType.Fusion) && card.Id!=CardId.ElderEntityNtss)<=0)
                        && CheckRemainInDeck(CardId.MudoratheSwordOracle)<=0 && CheckRemainInDeck(CardId.KeldotheSacredProtector)<=0) return false;
                    if ((activate_KelbektheAncientVanguard_2 || CheckRemainInDeck(CardId.KelbektheAncientVanguard) <= 0)
                       && (activate_AgidotheAncientSentinel_2 || CheckRemainInDeck(CardId.AgidotheAncientSentinel) <= 0) && Bot.HasInExtra(CardId.SprightElf))
                    {
                        List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && !no_link_ids.Contains(card.Id) && card != Card).ToList();
                        if (cards.Count() >= 1) return false;
                    }
                }
                activate_DivineroftheHerald = true;
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool TearlamentsScreamEffect_1()
        {
            return SpellActivate();
        }
        private bool MudoratheSwordOracleEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if ((Bot.Hand.Count(card => card != null && card.Id == CardId.AgidotheAncientSentinel) <= 0 || activate_AgidotheAncientSentinel_2)
                    && (Bot.Hand.Count(card => card != null && card.Id == CardId.KelbektheAncientVanguard) <= 0 || activate_KelbektheAncientVanguard_2) &&
                    !summoned && !Bot.HasInHand(CardId.MudoratheSwordOracle) && !Bot.HasInHand(CardId.KeldotheSacredProtector)) return false;
                IList<int> cardsid = new List<int>();
                if (!activate_AgidotheAncientSentinel_2) cardsid.Add(CardId.AgidotheAncientSentinel);
                if (!activate_KelbektheAncientVanguard_2) cardsid.Add(CardId.KelbektheAncientVanguard);
                cardsid.Add(CardId.KeldotheSacredProtector);
                cardsid.Add(CardId.MudoratheSwordOracle);
                if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                AI.SelectCard(cardsid);
                SetSpSummon();
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                ClientCard chain_card = Util.GetLastChainCard();
                if (chain_card != null && chain_card.Controller == 0) return false;
                if (GetZoneCards(CardLocation.Grave, Enemy).Count() <= 0 && Bot.Deck.Count >= 3) return false;
                if ((Duel.CurrentChain == null || Duel.CurrentChain.Count <= 0) && Bot.Deck.Count >= 3) return false;
                if (Duel.CurrentChain.Any(card => card != null && card.Controller == 0 && (card.Id == CardId.MudoratheSwordOracle || card.Id == CardId.KeldotheSacredProtector) || card.Id == CardId.NaelshaddollAriel)) return false;
                if (Duel.CurrentChain.Any(card => card != null && card.Controller == 0 && card.Id == CardId.ShaddollBeast) && Bot.Deck.Count <= 0)
                {
                    if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                    else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                    return true;
                }
                foreach (var card in Duel.ChainTargets)
                {
                    if (card == null) continue;
                    if ((card == Card && (Bot.Deck.Count < 5 || Enemy.Graveyard.Count(ccard => ccard != null && !key_no_send_to_deck_ids.Contains(ccard.Id)) > 0))
                        || (card.Controller == 1 && card.Location == CardLocation.Grave))
                    {
                        if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                        else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                        return true;
                    }
                }
                foreach (var card in Duel.CurrentChain)
                {
                    if (card == null || card.Controller == 0 || card.Location != CardLocation.Grave) continue;
                    if (key_send_to_deck_ids.Contains(card.Id))
                    {
                        if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                        else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                        return true;
                    }
                }
                if (Duel.Phase == DuelPhase.End && Bot.Deck.Count < 3 && Bot.Graveyard.Count > 0)
                {
                    if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                    else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                    return true;
                }
                return false;
            }
            else
            {
                if (Duel.Phase == DuelPhase.End && Bot.Deck.Count < 3 && Bot.Graveyard.Count > 0)
                {
                    if (Card.Id == CardId.MudoratheSwordOracle) activate_MudoratheSwordOracle_2 = true;
                    else if (Card.Id == CardId.KeldotheSacredProtector) activate_KeldotheSacredProtector_2 = true;
                    return true;
                }
                return false;
            }
        }
       
        private bool TearlamentsScheirenEffect() 
        {
            if (Card.Location == CardLocation.Grave)
            {
                return FusionEffect(CardId.TearlamentsScheiren);
            }
            else
            {
                if (AllActivated() && (!Bot.GetMonsters().Any(card=>card!=null && card.IsFaceup() && card.Level==4) || !Bot.HasInExtra(CardId.AbyssDweller))) return false;
                activate_TearlamentsScheiren_1 = true;
                SetSpSummon();
                return true;
            }
        }
        private bool NaelshaddollArielEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return Enemy.Graveyard.Count(card => card != null && !key_no_remove_ids.Contains(card.Id)) > 0;
            }
            else
            {
                SetSpSummon();
                return true;
            }
        }
        private bool IsShouldSummonFusion(int setcode = -1,int race = -1,bool all = false,int flag = 0x1f)
        {
            List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup()).ToList();
            cards.AddRange(GetZoneCards(CardLocation.Grave | CardLocation.Hand, Bot));
            int xcount_1 = 0;
            int xcount_2 = 0;
            int xcount_3 = 0;
            if ((flag & (int)Flag.TearlamentsKitkallos)>0 && Bot.ExtraDeck.Count(card => card != null && card.Id == CardId.TearlamentsKitkallos) > 0)
            {
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Id == CardId.TearlamentsRulkallos && card.Location == CardLocation.MonsterZone && !card.IsDisabled()) continue;
                    if (card.HasSetcode(SETCODE) &&  card.HasType(CardType.Monster))
                    {
                        ++xcount_1;
                        if (card.HasRace(CardRace.Aqua)) { --xcount_1; ++xcount_3; }
                    }
                    else if (card.HasRace(CardRace.Aqua)) ++xcount_2;
                }
                if (setcode == SETCODE)
                {
                    ++xcount_1;
                    if (race == (int)CardRace.Aqua) { --xcount_1; ++xcount_3; }
                } 
                else if (race == (int)CardRace.Aqua) ++xcount_2;
                if (xcount_3 > 1 || (xcount_1 > 0 && xcount_3 > 0) || (xcount_1 > 0 && xcount_2 > 0) || (xcount_2 > 0 && xcount_3 > 0))  return true;
            }
            if ((flag & (int)Flag.TearlamentsRulkallos) > 0 && Bot.ExtraDeck.Count(card => card != null && card.Id == CardId.TearlamentsRulkallos) > 0)
            {
                xcount_1 = 0;
                xcount_2 = 0;
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Id == CardId.TearlamentsKitkallos && xcount_1 <= 0) ++xcount_1;
                    else if (card.HasSetcode(SETCODE) && card.HasType(CardType.Monster))  ++xcount_2;
                }
                if (setcode == SETCODE) ++xcount_2;
                if (xcount_1 > 0 && xcount_2 > 0) return true;
            }
            if ((flag & (int)Flag.TearlamentsKaleidoHeart) > 0 && Bot.ExtraDeck.Count(card => card != null && card.Id == CardId.TearlamentsKaleidoHeart) > 0)
            {
                xcount_1 = 0;
                xcount_2 = 0;
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Id == CardId.TearlamentsRulkallos && card.Location == CardLocation.MonsterZone && !card.IsDisabled()) continue;
                    if (card.Id == CardId.TearlamentsReinoheart && xcount_1 <= 0) ++xcount_1;
                    else if (card.HasRace(CardRace.Aqua)) ++xcount_2;
                }
                if (race == (int)CardRace.Aqua) ++xcount_2;
                if (xcount_1 > 0 && xcount_2 > 0) return true;
            }
            if ((flag & (int)Flag.PredaplantDragostapelia) > 0 && Bot.ExtraDeck.Count(card => card != null && card.Id == CardId.PredaplantDragostapelia) > 0)
            {
                xcount_1 = 0;
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    if (card.Id == CardId.TearlamentsRulkallos && card.Location == CardLocation.MonsterZone && !card.IsDisabled()) continue;
                    if (((Bot.GetMonstersInMainZone().Count > 4 && card.Location == CardLocation.MonsterZone)|| Bot.GetMonstersInMainZone().Count<=4) && card.HasType(CardType.Fusion)) ++xcount_1;
                }
                if (xcount_1 > 0) return true;
            }
            if (all)
            {
                if ((flag & (int)Flag.ElShaddollWinda) > 0 && Bot.ExtraDeck.Count(card => card != null && card.Id == CardId.ElShaddollWinda) > 0)
                {
                    List<ClientCard> materials_1 = new List<ClientCard>();
                    List<ClientCard> materials_2 = new List<ClientCard>();
                    foreach (var card in cards)
                    {
                        if (card == null) continue;
                        if (card.HasSetcode(0x9d)) materials_1.Add(card);
                        else if ((card.Id == CardId.TearlamentsHavnis || card.Id == CardId.TearlamentsMerrli
                            || card.Id == CardId.TearlamentsScheiren) && card.HasAttribute(CardAttribute.Dark))
                            materials_2.Add(card);
                    }
                    xcount_1 = materials_1.Count;
                    xcount_2 = materials_2.Count;
                    if ((setcode == SETCODE) && race == (int)CardRace.Aqua)
                    {
                        ++xcount_2;
                    }
                    if (xcount_1 > 0 && xcount_2 > 0) return true;
                }
            }
            return false;
        }
        private bool TearlamentsRulkallosEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                activate_TearlamentsRulkallos_2 = true;
                SetSpSummon();
                return true;
            }
            else
            {
                if (Card.IsDisabled()) return false;
                activate_TearlamentsRulkallos_1 = true;
                return true;
            }
        }
        private bool AgidotheAncientSentinelEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player == 1)
                {
                    //if (!activate_AgidotheAncientSentinel_2 && (Bot.HasInHand(CardId.HeraldofOrangeLight) || Bot.HasInHand(CardId.HeraldofGreenLight))
                    //    && Bot.Hand.Count(card => card != null && card.Id == CardId.AgidotheAncientSentinel) <= 1) return false;
                    return false;
                }
                else
                { 
                    if((!Bot.HasInGraveyard(CardId.KelbektheAncientVanguard) && !Bot.HasInGraveyard(CardId.MudoratheSwordOracle)
                        && !Bot.HasInGraveyard(CardId.KeldotheSacredProtector) && Bot.Hand.Count(card => card != null && card.Id == CardId.AgidotheAncientSentinel)<=1)
                        || Bot.GetMonstersInMainZone().Count>=4) return false;
                }
                if(!activate_AgidotheAncientSentinel_2 && (Bot.HasInHand(CardId.HeraldofOrangeLight) || Bot.HasInHand(CardId.HeraldofGreenLight))
                   && Bot.Hand.Count(card => card != null && card.Id == CardId.AgidotheAncientSentinel) <= 1) return false;
                SetSpSummon();
                return true;
            }
            else
            {
                if (AllActivated() && !Duel.CurrentChain.Any(card => card != null && card.Controller == 0 && (card.Id == CardId.TearlamentsHavnis || card.Id == CardId.TearlamentsMerrli || card.Id == CardId.TearlamentsScheiren))) return false;
                activate_AgidotheAncientSentinel_2 = true;
                return true;
            }
        }
        private bool AllActivated()
        {
            return (activate_TearlamentsScheiren_2 || CheckRemainInDeck(CardId.TearlamentsScheiren) <= 0)
                && (activate_TearlamentsHavnis_2 || CheckRemainInDeck(CardId.TearlamentsHavnis) <= 0)
                && (activate_TearlamentsMerrli_2 || CheckRemainInDeck(CardId.TearlamentsMerrli) <= 0);
        }
        private bool KelbektheAncientVanguardEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.LastChainPlayer == 0) return false;
                if ((Bot.HasInHand(CardId.HeraldofOrangeLight) || Bot.HasInHand(CardId.HeraldofGreenLight))
                    && !activate_KelbektheAncientVanguard_2) return false;
                List<ClientCard> cards = Enemy.GetMonsters();
                cards.Sort(CardContainer.CompareCardAttack);
                cards.Reverse();
                AI.SelectCard(cards);
                
                SetSpSummon();
                return true;
            }
            else
            {
                if (AllActivated() && !Duel.CurrentChain.Any(card=>card!=null && card.Controller==0 && (card.Id==CardId.TearlamentsHavnis||card.Id==CardId.TearlamentsMerrli || card.Id==CardId.TearlamentsScheiren))) return false;
                activate_KelbektheAncientVanguard_2 = true;
                return true;
            }
        }
        private bool TearlamentsMerrliEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (AllActivated()) return false;
                activate_TearlamentsMerrli_1 = true;
                return true;
            }
            else
            {
                return FusionEffect(CardId.TearlamentsMerrli);
            }
        }
        private bool TearlamentsHavnisEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (AllActivated()) return false;
                activate_TearlamentsHavnis_1 = true;
                return true;
            }
            else
            {
                return FusionEffect(CardId.TearlamentsHavnis);
            }
        }
        private bool SpellSet()
        {
            if (Card.Id == CardId.TearlamentsSulliek) return !Bot.GetSpells().Any(card => card != null && card.Id == CardId.TearlamentsSulliek && (card.IsFacedown() || (card.IsFaceup() && !card.IsDisabled())));
            return Card.HasType(CardType.QuickPlay) || Card.HasType(CardType.Trap);
        }
        private bool TearlamentsReinoheartEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                List<ClientCard> cards = Bot.Hand.Where(card => card != null && card.HasSetcode(SETCODE)).ToList();
                List<ClientCard> temp = new List<ClientCard>(cards);
                foreach (var card in temp)
                {
                    if ((card.Id == CardId.TearlamentsScheiren && !activate_TearlamentsScheiren_1)
                        || (card.Id == CardId.TearlamentsMerrli && !summoned && !activate_TearlamentsMerrli_1)
                        ||(card.Id==CardId.TearlamentsHavnis && cards.Count(ccard=>ccard!=null && ccard.Id==CardId.TearlamentsHavnis) > 1))
                        cards.Remove(card);
                }
                if (cards.Count() <= 0) return false;
                activate_TearlamentsReinoheart_2 = true;
                SetSpSummon();
                return true;
            }
            else
            {
                if (AllActivated()) return false;
                activate_TearlamentsReinoheart_1 = true;
                return true;
            }
        }
        private bool ShaddollDragonEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                ClientCard card = Util.GetBestEnemySpell();
                List<ClientCard> cards = Enemy.GetSpells().Where(ccard=> tgcard == null || tgcard != ccard).ToList();
                if (card != null && (tgcard == null || tgcard != card)) { AI.SelectCard(card); return true; }
                else if (cards.Count > 0) { AI.SelectCard(cards); return true; }
                return false;
            }
            else return true;
        }
        private bool TearlamentsKitkallosEffect_2()
        {
            if (Bot.HasInMonstersZone(CardId.ElShaddollWinda, true, false, true)) return TearlamentsKitkallosEffect();
            return false;
        }
        private bool TearlamentsKitkallosEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.TearlamentsKitkallos, 1))
            {
                if (Card.IsDisabled()) return false;
                if (AllActivated() && !Bot.HasInGraveyard(CardId.TearlamentsKaleidoHeart) && !Bot.HasInGraveyard(CardId.TearlamentsRulkallos)
                    && !Bot.HasInGraveyard(CardId.TearlamentsKitkallos)) return false;
                if(Bot.HasInMonstersZone(CardId.ElShaddollWinda,true,false,true)) AI.SelectCard(CardId.ElShaddollWinda);
                else if (Bot.HasInMonstersZone(CardId.TearlamentsKitkallos) && !activate_TearlamentsKitkallos_3 && !AllActivated()) AI.SelectCard(CardId.TearlamentsKitkallos);
                else if (!activate_TearlamentsScheiren_2 && Bot.HasInMonstersZone(CardId.TearlamentsScheiren) && IsShouldSummonFusion()) AI.SelectCard(CardId.TearlamentsScheiren);
                else if (!activate_TearlamentsMerrli_2 && Bot.HasInMonstersZone(CardId.TearlamentsMerrli) && IsShouldSummonFusion()) AI.SelectCard(CardId.TearlamentsMerrli);
                else if (!activate_TearlamentsHavnis_2 && Bot.HasInMonstersZone(CardId.TearlamentsHavnis) && IsShouldSummonFusion()) AI.SelectCard(CardId.TearlamentsHavnis);
                else if (Bot.HasInMonstersZone(CardId.TearlamentsReinoheart) && !activate_TearlamentsReinoheart_1 && !AllActivated()) AI.SelectCard(CardId.TearlamentsReinoheart);
                else if (Bot.HasInMonstersZone(CardId.Eva) && Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Light) && card.HasRace(CardRace.Fairy)) > 0
                    && (CheckRemainInDeck(CardId.HeraldofGreenLight) > 0 || CheckRemainInDeck(CardId.HeraldofOrangeLight) > 0)) AI.SelectCard(CardId.Eva);
                else if ((Bot.HasInExtra(CardId.TearlamentsRulkallos) && !Bot.HasInGraveyard(CardId.TearlamentsKitkallos))
                    || (Bot.HasInExtra(CardId.PredaplantDragostapelia) && Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Fusion)) <= 0)) AI.SelectCard(CardId.TearlamentsKitkallos);
                else if (Bot.HasInMonstersZone(CardId.ShaddollDragon) && Enemy.GetSpellCount() > 0) AI.SelectCard(CardId.ShaddollDragon);
                else
                {
                    List<ClientCard> mcards = GetZoneCards(CardLocation.MonsterZone, Bot);
                    mcards.Sort(CardContainer.CompareCardAttack);
                    AI.SelectCard(mcards);
                }
                if(!activate_TearlamentsKitkallos_1 && Bot.HasInGraveyard(CardId.TearlamentsKitkallos)) AI.SelectNextCard(Bot.Graveyard.Where(card => card != null && card.Id == CardId.TearlamentsKitkallos).ToList());
                if (!activate_TearlamentsMerrli_1 && Bot.HasInGraveyard(CardId.TearlamentsMerrli) && !AllActivated()) AI.SelectNextCard(Bot.Graveyard.Where(card => card != null && card.Id == CardId.TearlamentsMerrli).ToList());
                else if (!activate_TearlamentsMerrli_1 && Bot.HasInHand(CardId.TearlamentsMerrli) && !AllActivated()) AI.SelectNextCard(Bot.Hand.Where(card => card != null && card.Id == CardId.TearlamentsMerrli).ToList());
                else if (!activate_TearlamentsReinoheart_1 && Bot.HasInGraveyard(CardId.TearlamentsReinoheart) && !AllActivated()) AI.SelectNextCard(Bot.Graveyard.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart).ToList());
                else if (!activate_TearlamentsReinoheart_1 && Bot.HasInHand(CardId.TearlamentsReinoheart) && !AllActivated()) AI.SelectNextCard(Bot.Hand.Where(card => card != null && card.Id == CardId.TearlamentsReinoheart).ToList());
                else AI.SelectNextCard(CardId.TearlamentsKaleidoHeart,CardId.TearlamentsRulkallos,CardId.TearlamentsKitkallos);
                activate_TearlamentsKitkallos_2 = true;
                SetSpSummon();
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                if (AllActivated()) return false;
                activate_TearlamentsKitkallos_3 = true;
                return true;
            }
            else
            {
                if (Card.IsDisabled()) return false;
                if (!IS_YGOPRO) select_TearlamentsKitkallos = true;
                activate_TearlamentsKitkallos_1 = true;
                return true;
            }
        }
        private bool HasinZoneKeyCard(CardLocation loc)
        {
            if (loc == CardLocation.Hand)
            {
                if (!activate_TearlamentsScheiren_2 && Bot.HasInHand(CardId.TearlamentsScheiren)) return true;
                if (!activate_TearlamentsHavnis_2 && Bot.HasInHand(CardId.TearlamentsHavnis)) return true;
                if (!activate_TearlamentsMerrli_2 && Bot.HasInHand(CardId.TearlamentsMerrli)) return true;
            }
            if (loc == CardLocation.Deck)
            {
                if (!activate_TearlamentsScheiren_2 && CheckRemainInDeck(CardId.TearlamentsScheiren)>0) return true;
                if (!activate_TearlamentsHavnis_2 && CheckRemainInDeck(CardId.TearlamentsHavnis)>0) return true;
                if (!activate_TearlamentsMerrli_2 && CheckRemainInDeck(CardId.TearlamentsMerrli)>0) return true;
            }
            return false;
        }
        private bool TearlamentsSulliekEffect()
        {
            if (SpellActivate()) return true;
            if (Card.Location == CardLocation.SpellZone)
            {
                ClientCard card = Util.GetLastChainCard();
                if (card != null && card.Controller == 1 && card.Location == CardLocation.MonsterZone
                    && !card.IsShouldNotBeTarget())
                {
                    if (_PredaplantDragostapelia != null && _PredaplantDragostapelia.Location == CardLocation.MonsterZone && _PredaplantDragostapelia.IsFaceup()
                        && e_PredaplantDragostapelia_cards.Contains(card)) return false;
                    chain_TearlamentsSulliek = card;
                    return true;
                }
                else
                {
                    if (Duel.Player == 1 || Duel.Phase == DuelPhase.End)
                    {
                        if (Duel.LastChainPlayer == 0  || Duel.CurrentChain.Count(ccard=>ccard!=null &&ccard.Controller==0
                            && (ccard.Id==CardId.TearlamentsHavnis || ccard.Id == CardId.TearlamentsMerrli || ccard.Id == CardId.TearlamentsScheiren))>0) return false;
                        if (((Bot.HasInMonstersZone(CardId.TearlamentsKitkallos,false,false,true) && !activate_TearlamentsKitkallos_3 && IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua)
                            && ((CheckRemainInDeck(CardId.TearlamentsHavnis) > 0 && !activate_TearlamentsHavnis_2)
                            || (CheckRemainInDeck(CardId.TearlamentsMerrli) > 0 && !activate_TearlamentsMerrli_2)
                            || (CheckRemainInDeck(CardId.TearlamentsScheiren) > 0 && !activate_TearlamentsScheiren_2)) && Bot.GetMonsterCount()<3)
                            ||(IsCanFusionSummon() && (Bot.HasInMonstersZone(CardId.TearlamentsScheiren) || Bot.HasInMonstersZone(CardId.TearlamentsHavnis)
                            || Bot.HasInMonstersZone(CardId.TearlamentsMerrli))) || (!activate_TearlamentsReinoheart_2 && Bot.HasInMonstersZone(CardId.TearlamentsReinoheart) && (HasinZoneKeyCard(CardLocation.Hand)
                            || (Bot.Hand.Count(ccard=> ccard != null && ccard.HasSetcode(SETCODE))>0 && HasinZoneKeyCard(CardLocation.Deck)))&& IsShouldSummonFusion(SETCODE, (int)CardRace.Aqua))) && IsCanSpSummon())
                        {
                            List<ClientCard> cards = Enemy.GetMonsters().Where(ecard=> ecard != null && !ecard.IsShouldNotBeTarget() && ecard.IsFaceup()).ToList();
                            if (cards.Count > 0)
                            {
                                cards.Sort(CardContainer.CompareCardAttack);
                                cards.Reverse();
                                chain_TearlamentsSulliek = cards[0];
                            }
                            else chain_TearlamentsSulliek = null;
                            return true;

                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}