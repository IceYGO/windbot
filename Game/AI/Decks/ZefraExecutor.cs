using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
namespace WindBot.Game.AI.Decks
{
    [Deck("Zefra", "AI_Zefra")]
    class ZefraExecutor : DefaultExecutor
    {
        public class CardId
        {
            //main code
            public const int PSY_FrameDriver = 49036338;
            public const int Zefraath = 29432356;
            public const int TheMightyMasterofMagic = 3611830;
            public const int AstrographSorcerer = 76794549;
            public const int DestrudotheLostDragon_Frisson = 5560911;
            public const int SupremeKingGateZero = 96227613;
            public const int MythicalBeastJackalKing = 27354732;
            public const int SecretoftheYangZing = 58990362;
            public const int FlameBeastoftheNekroz = 20773176;
            public const int StellarknightZefraxciton = 22617205;
            public const int SupremeKingDragonDarkwurm = 69610326;
            public const int Blackwing_ZephyrostheElite = 14785765;
            public const int ShaddollZefracore = 95401059;
            public const int Raidraptor_SingingLanius = 31314549;
            public const int SatellarknightZefrathuban = 96223501;
            public const int Raider_Wing = 52159691;
            public const int Zefraxi_TreasureoftheYangZing = 21495657;
            public const int RitualBeastTamerZeframpilica = 57777714;
            public const int ServantofEndymion = 92559258;
            public const int PSY_FramegearGamma = 38814750;
            public const int MechaPhantomBeastO_Lion = 72291078;
            public const int MaxxC = 23434538;
            public const int Deskbot001 = 94693857;
            public const int JetSynchron = 9742784;
            public const int DDLamia = 19580308;
            public const int DDSavantKepler = 11609969;
            public const int LightoftheYangZing = 61488417;
            public const int Rank_Up_MagicSoulShaveForce = 23581825;
            public const int SpellPowerMastery = 38943357;
            public const int DragonShrine = 41620959;
            public const int Terraforming = 73628505;
            public const int ZefraProvidence = 74580251;
            public const int FoolishBurial = 81439173;
            public const int CalledbytheGrave = 24224830;
            public const int DarkContractwiththGate = 46372010;
            public const int OracleofZefra = 32354768;
            public const int ZefraWar = 96073342;
            public const int ZefraDivineStrike = 35561352;
            public const int NinePillarsofYangZing = 57831349;
            public const int OneforOne = 2295440;

            //extra code
            public const int BorreloadSavageDragon = 27548199;
            public const int Odd_EyesMeteorburstDragon = 80696379;
            public const int F_A_DawnDragster = 33158448;
            public const int Denglong_FirstoftheYangZing = 65536818;
            public const int HeraldoftheArcLight = 79606837;
            public const int TruKingofAllCalamities = 88581108;
            public const int Raidraptor_ArsenalFalcon = 96157835;
            public const int Raidraptor_ForceStrix = 73347079;
            public const int SaryujaSkullDread = 74997493;
            public const int MechaPhantomBeastAuroradon = 44097050;
            public const int HeavymetalfoesElectrumite = 24094258;
            public const int CrystronHalqifibrax = 50588353;
            public const int Raidraptor_WiseStrix = 36429703;
            public const int Linkuriboh = 41999284;
            public const int PSY_FramelordOmega = 74586817;

            public const int MechaPhantomBeastToken = 44097051;
        }
        private bool opt_0 = false;
        private bool opt_1 = false;
        private bool opt_2 = false;
        //edo false
        private const bool IS_YGOPRO = true;
        private const int P_ACTIVATE_DESC = 1160;
        //private const int P_SPSUMMON_DESC = 1163;
        private int p_count = 0;
        private int spell_activate_count = 0;
        private bool summoned = false;
        private bool link_summoned = false;
        private bool p_summoned = false;
        private bool p_summoning = false;
        private bool activate_SupremeKingDragonDarkwurm_1 = false;
        private bool activate_p_Zefraath = false;
        private bool activate_OracleofZefra = false;
        private bool activate_ZefraProvidence = false;
        private bool activate_SupremeKingDragonDarkwurm_2 = false;
        private bool activate_JetSynchron = false;
        private bool activate_Blackwing_ZephyrostheElite = false;
        private bool activate_DragonShrine = false;
        private bool activate_SpellPowerMastery = false;
        private bool activate_DestrudotheLostDragon_Frisson = false;
        private bool activate_DarkContractwiththGate = false;
        private bool activate_SecretoftheYangZing = false;
        private bool activate_ShaddollZefracore = false;
        private bool activate_DDLamia = false;
        private bool xyz_mode = false;
        private bool Blackwing_ZephyrostheElite_activate = false;
        private bool HeavymetalfoesElectrumite_activate = false;
        private bool should_destory = false;
        private List<ClientCard> Odd_EyesMeteorburstDragon_materials = new List<ClientCard>();
        private bool duel_start = true;
        private int activate_count = 0;
        private int summon_count = 0;
        private bool enemy_activate = false;
        private enum CustomMessage
        {
            Happy,
            Angry,
            Surprise
        }
        private static class Toos
        {
            public delegate bool Delegate(ClientCard card);
            private static bool DefaultFunc(ClientCard card)
            {
                return true;
            }
            public static bool LinqAny(IList<ClientCard> cards, Delegate @delegate = null)
            {
                if (cards == null) return false;
                @delegate = @delegate ?? DefaultFunc;
                return cards.Any(card => card != null && @delegate(card));
            }
            public static bool LinqAll(IList<ClientCard> cards, Delegate @delegate = null, bool flag = true)
            {
                if (cards == null) return false;
                IList<ClientCard> rcards = new List<ClientCard>(cards);
                if (flag) rcards = cards.Where(card => card != null).ToList();
                @delegate = @delegate ?? DefaultFunc;
                return rcards.All(card => card != null && @delegate(card));
            }
            public static int LinqCount(IList<ClientCard> cards, Delegate @delegate = null)
            {
                if (cards == null) return -1;
                @delegate = @delegate ?? DefaultFunc;
                return cards.Count(card => card != null && @delegate(card));
            }
            public static List<ClientCard> LinqWhere(IList<ClientCard> cards, Delegate @delegate = null)
            {
                if (cards == null) return new List<ClientCard>();
                @delegate = @delegate ?? DefaultFunc;
                return cards.Where(card => card != null && @delegate(card)).ToList();
            }
        }
        private class Func
        {
            private IList<object> _parameters = new List<object>();
            private List<int> no_p_spsummon_ids = new List<int>()
            {
                CardId.Zefraath
            };
            private List<ClientCard> selectCardList = null;
            private List<int> selectCardIdList = null;
            public List<ClientCard> GetSelectCardList()
            {
                if (selectCardList == null)
                {
                    selectCardList = new List<ClientCard>();
                }
                else
                {
                    selectCardList.Clear();
                }
                return selectCardList;
            }
            public List<int> GetSelectCardIdList()
            {
                if (selectCardIdList == null)
                {
                    selectCardIdList = new List<int>();
                }
                else
                {
                    selectCardIdList.Clear();
                }
                return selectCardIdList;
            }
            public bool IsLocation(ClientCard card)
            {
                return card.Location == (CardLocation)_parameters[0];
            }
            public bool IsCode(ClientCard card)
            {
                return card.IsCode((int)_parameters[0]);
            }
            public static bool IsCode(ClientCard card, params int[] ids)
            {
                if (card == null) return false;
                foreach (var id in ids)
                {
                    if (card.IsCode(id)) return true;
                }
                return false;
            }
            public bool HasSetCode(ClientCard card)
            {
                return card.HasSetcode((int)_parameters[0]);
            }
            public static bool HasSetCode(ClientCard card, params int[] set_codes)
            {
                if (card == null) return false;
                foreach (var set_code in set_codes)
                {
                    if (card.HasSetcode(set_code)) return true;
                }
                return false;
            }
            public static bool IsFaceUp(ClientCard card)
            {
                return card.IsFaceup();
            }
            public bool HasAttribute(ClientCard card)
            {
                return card.HasAttribute((CardAttribute)_parameters[0]);
            }
            public bool HasRace(ClientCard card)
            {
                return card.HasRace((CardRace)_parameters[0]);
            }
            public bool HasLevel(ClientCard card)
            {
                return card.Level == (int)_parameters[0];
            }
            public bool HasType(ClientCard card)
            {
                return card.HasType((CardType)_parameters[0]);
            }
            public static bool IsOnfield(ClientCard card)
            {
                return (card.Location & CardLocation.MonsterZone) > 0 || (card.Location & CardLocation.SpellZone) > 0;
            }
            public static Toos.Delegate NegateFunc(Toos.Delegate @delegate)
            {
                return card => { return !@delegate(card); };
            }
            private void SetParameters(IList<object> parameters)
            {
                ClearParameters();
                for (int i = 0; i < parameters?.Count(); ++i)
                {
                    _parameters.Add(parameters[i]);
                }
            }
            private void ClearParameters()
            {
                _parameters.Clear();
            }
            public bool CardsCheckAny(IList<ClientCard> cards, Toos.Delegate @delegate = null, params object[] parameters)
            {
                SetParameters(parameters);
                return Toos.LinqAny(cards, @delegate);
            }
            public bool CardsCheckALL(IList<ClientCard> cards, Toos.Delegate @delegate = null, bool all = true, params object[] parameters)
            {
                SetParameters(parameters);
                return Toos.LinqAll(cards, @delegate, all);
            }
            public int CardsCheckCount(IList<ClientCard> cards, Toos.Delegate @delegate = null, params object[] parameters)
            {
                SetParameters(parameters);
                return Toos.LinqCount(cards, @delegate);
            }
            public List<ClientCard> CardsCheckWhere(IList<ClientCard> cards, Toos.Delegate @delegate = null, params object[] parameters)
            {
                SetParameters(parameters);
                return Toos.LinqWhere(cards, @delegate);
            }
            public static List<T> MergeList<T>(params List<T>[] lists)
            {
                List<T> result = new List<T>();
                foreach (var list in lists)
                {
                    if (list == null) continue;
                    result.AddRange(list);
                }
                return result;
            }
            public List<ClientCard> CardsIdToClientCards(IList<int> cardsId, IList<ClientCard> cardsList, bool uniqueId = true)
            {
                if (cardsList?.Count() <= 0 || cardsId?.Count() <= 0) return new List<ClientCard>();
                List<ClientCard> result = new List<ClientCard>();
                cardsId = cardsId.Distinct().ToList();
                foreach (var cardid in cardsId)
                {
                    List<ClientCard> cards = CardsCheckWhere(cardsList, IsCode, cardid);
                    if (cards.Count <= 0) continue;
                    if (uniqueId) result.Add(cards.First());
                    else result.AddRange(cards);
                }
                return result;
            }
            public static List<int> ClientCardsToCardsId(IList<ClientCard> cardsList, bool uniqueId = false, bool alias = false)
            {
                if (cardsList?.Count <= 0) return new List<int>();
                List<int> res = new List<int>();
                foreach (var card in cardsList)
                {
                    if (card == null) continue;
                    if (card.Alias != 0 && alias && !(res.Contains(card.Alias) & uniqueId)) res.Add(card.Alias);
                    else if (card.Id != 0 && !(res.Contains(card.Id) & uniqueId)) res.Add(card.Id);
                }
                return res;
            }
            //AIUtil
            public static IList<ClientCard> CheckSelectCount(AIUtil util, IList<ClientCard> _selected, IList<ClientCard> cards, int min, int max)
            {
                return _selected?.Count() <= 0 ? null : util.CheckSelectCount(_selected, cards, min, max);
            }
            public static List<ClientCard> GetZoneCards(ClientField player, CardLocation loc, bool feceup = false, bool disable = false)
            {
                if (!feceup) disable = false;
                List<ClientCard> result = new List<ClientCard>();
                if ((loc & CardLocation.Hand) > 0) result.AddRange(Toos.LinqWhere(player.Hand));
                if ((loc & CardLocation.MonsterZone) > 0) result.AddRange(Toos.LinqWhere(player.MonsterZone, card => !(!card.IsFaceup() & feceup) && !(!card.IsDisabled() & disable)));
                if ((loc & CardLocation.SpellZone) > 0) result.AddRange(Toos.LinqWhere(player.SpellZone, card => !(!card.IsFaceup() & feceup) && !(!card.IsDisabled() & disable)));
                if ((loc & CardLocation.PendulumZone) > 0) result.AddRange(Toos.LinqWhere(new List<ClientCard>() { player.SpellZone[0], player.SpellZone[4] }, card => !(!card.IsFaceup() & feceup) && !(!card.IsDisabled() & disable)));
                if ((loc & CardLocation.Grave) > 0) result.AddRange(Toos.LinqWhere(player.Graveyard));
                if ((loc & CardLocation.Removed) > 0) result.AddRange(Toos.LinqWhere(player.Banished, card => !(!card.IsFaceup() & feceup)));
                if ((loc & CardLocation.Extra) > 0) result.AddRange(Toos.LinqWhere(player.ExtraDeck, card => !(!card.IsFaceup() & feceup)));
                result = result.Distinct().ToList();
                return result;
            }
            public bool HasInZone(ClientField player, CardLocation loc, int id, bool feceup = false, bool disable = false)
            {
                return CardsCheckAny(GetZoneCards(player, loc, feceup, disable), IsCode, id);
            }
            public static bool SpellActivate(ClientCard card)
            {
                return card.Location == CardLocation.Hand || (card.Location == CardLocation.SpellZone && card.IsFacedown());
            }
            public static bool PendulumActivate(int desc, ClientCard card)
            {
                return desc == P_ACTIVATE_DESC && card.Location == CardLocation.Hand;
            }
            private static Toos.Delegate GetPSpSummonLimilt(ClientCard pcard)
            {
                int setcode = -1;
                int setcode2 = -1;
                switch (pcard.Id)
                {
                    case CardId.SecretoftheYangZing:
                    case CardId.Zefraxi_TreasureoftheYangZing: { setcode = 0xc4; setcode2 = 0x9e; break; }
                    case CardId.FlameBeastoftheNekroz: { setcode = 0xc4; setcode2 = 0xb4; break; }
                    case CardId.StellarknightZefraxciton:
                    case CardId.SatellarknightZefrathuban: { setcode = 0xc4; setcode2 = 0x109c; break; }
                    case CardId.RitualBeastTamerZeframpilica: { setcode = 0xc4; setcode2 = 0x10b5; break; }
                    case CardId.ShaddollZefracore: { setcode = 0xc4; setcode2 = 0x9d; break; }
                    case CardId.DDSavantKepler: { setcode = 0xaf; break; }
                    default: break;
                }
                return card => {
                    return (setcode == -1 ? true : card.HasSetcode(setcode))
                        || (setcode2 == -1 ? true : card.HasSetcode(setcode2));
                };
            }
            public static int[] GetPScales(ClientField bot)
            {
                int[] pScales = new int[2];
                ClientCard lcard = bot.SpellZone[0];
                ClientCard rcard = bot.SpellZone[4];
                pScales[0] = (lcard == null || lcard.IsFacedown() || !lcard.HasType(CardType.Pendulum)) ? -1 : lcard.RScale;
                pScales[1] = (rcard == null || rcard.IsFacedown() || !rcard.HasType(CardType.Pendulum)) ? -1 : rcard.LScale;
                return pScales;
            }
            public static int GetPScale(ClientField bot, int id)
            {
                bool rscale = false;
                ClientCard pcard = null;
                if (bot.SpellZone[0] != null && bot.SpellZone[0].Id == id)
                {
                    pcard = bot.SpellZone[4];
                }
                else
                {
                    pcard = bot.SpellZone[0];
                    rscale = true;
                }
                if (pcard == null || pcard.IsFacedown() || !pcard.HasType(CardType.Pendulum)) return -1;
                return rscale ? pcard.RScale : pcard.LScale;
            }
            public List<ClientCard> GetPSpSummonMonster(ClientField bot, ClientCard lcard, ClientCard rcard)
            {
                if (lcard == null || rcard == null || !lcard.HasType(CardType.Pendulum) || !rcard.HasType(CardType.Pendulum) || (IsOnfield(lcard) & lcard.IsFacedown()) || (IsOnfield(lcard) & rcard.IsFacedown())) return null;
                int MaxScale = Math.Max(lcard.RScale, rcard.LScale);
                int MinScale = Math.Min(lcard.RScale, rcard.LScale);
                Toos.Delegate @llimit = GetPSpSummonLimilt(lcard);
                Toos.Delegate @rlimit = GetPSpSummonLimilt(rcard);
                return CardsCheckWhere(GetZoneCards(bot, CardLocation.Hand | CardLocation.Extra, true),
                    card => {
                        return card != lcard && card != rcard && card.HasType(CardType.Monster) && card.Level > MinScale && card.Level < MaxScale
                   && !no_p_spsummon_ids.Contains(card.Id) && @llimit(card) && @rlimit(card);
                    }); ;
            }
            public bool IsActivateScale(ClientField bot, ClientCard card)
            {
                ClientCard lcard = bot.SpellZone[0];
                ClientCard rcard = bot.SpellZone[4];
                List<ClientCard> spSummonMonster = null;
                if (lcard != null && rcard != null) return false;
                if (lcard == null && rcard == null) return true;

                spSummonMonster = lcard == null ? GetPSpSummonMonster(bot, card, rcard) : GetPSpSummonMonster(bot, lcard, card);
                return spSummonMonster?.Count() > 0;
            }
            public static int CompareCardScale(ClientCard cardA, ClientCard cardB)
            {
                if (cardA.RScale < cardB.RScale)
                    return -1;
                if (cardA.RScale == cardB.RScale)
                    return 0;
                return 1;
            }
            public static List<int> GetCardsRepeatCardsId(IList<ClientCard> cards)
            {
                if (cards?.Count <= 0) return new List<int>() { -1 };
                IList<int> cardsid = new List<int>();
                List<int> res = new List<int>();
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    cardsid.Add(card.Id);
                }
                for (int i = 0; i < cardsid.Count; i++)
                {
                    if (res.Count >= 0 && res.Contains(cardsid[i])) continue;
                    int times = 0;
                    for (int j = 0; j < cardsid.Count; j++)
                    {
                        if (times > 1) { res.Add(cardsid[i]); break; }
                        if (cardsid[i] == cardsid[j]) ++times;
                    }
                }
                if (res.Count <= 0) return new List<int>() { -1 };
                return res;
            }
        }
        private Func func = new Func();
        public ZefraExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.CalledbytheGrave, CalledbytheGraveEffect);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.F_A_DawnDragster, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.NinePillarsofYangZing, NinePillarsofYangZingEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZefraDivineStrike, ZefraDivineStrikeEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeraldoftheArcLight, HeraldoftheArcLightEffect);
            AddExecutor(ExecutorType.Activate, CardId.TruKingofAllCalamities, TruKingofAllCalamitiesEffect);
            AddExecutor(ExecutorType.Activate, CardId.PSY_FramegearGamma, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.SupremeKingDragonDarkwurm, SupremeKingDragonDarkwurmEffect);
            AddExecutor(ExecutorType.Activate, CardId.ServantofEndymion, ServantofEndymionEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.SpellPowerMastery, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.DragonShrine, DragonShrineEffect);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, FoolishBurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkContractwiththGate, DarkContractwiththGateEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.OracleofZefra, OracleofZefraEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZefraProvidence, ZefraProvidenceEffect);
            AddExecutor(ExecutorType.Activate, CardId.AstrographSorcerer, AstrographSorcererEffect);
            AddExecutor(ExecutorType.Activate, CardId.HeavymetalfoesElectrumite, HeavymetalfoesElectrumiteEffect);
            AddExecutor(ExecutorType.Summon, CardId.SupremeKingDragonDarkwurm, SupremeKingDragonDarkwurmSummon);
            AddExecutor(ExecutorType.Activate, CardId.SupremeKingGateZero, SupremeKingGateZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.Zefraxi_TreasureoftheYangZing, Zefraxi_TreasureoftheYangZingEffect);
            AddExecutor(ExecutorType.Activate, CardId.SatellarknightZefrathuban, SatellarknightZefrathubanEffect);
            AddExecutor(ExecutorType.Activate, CardId.RitualBeastTamerZeframpilica, RitualBeastTamerZeframpilicaEffect);
            AddExecutor(ExecutorType.Activate, CardId.SecretoftheYangZing, SecretoftheYangZingEffect);
            AddExecutor(ExecutorType.Activate, CardId.FlameBeastoftheNekroz, FlameBeastoftheNekrozEffect);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollZefracore, ShaddollZefracoreEffect);
            AddExecutor(ExecutorType.Activate, CardId.StellarknightZefraxciton, StellarknightZefraxcitonEffect);
            AddExecutor(ExecutorType.Activate, CardId.SupremeKingDragonDarkwurm, SupremeKingGateZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.AstrographSorcerer, SupremeKingGateZeroEffect);
            AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathEffect);
            AddExecutor(ExecutorType.Activate, CardId.DDSavantKepler, DDSavantKeplerEffect);
            AddExecutor(ExecutorType.Summon, CardId.DDSavantKepler, DDSavantKeplerSummon);
            AddExecutor(ExecutorType.Activate, CardId.ServantofEndymion, ServantofEndymionEffect_3);
            AddExecutor(ExecutorType.Activate, CardId.MythicalBeastJackalKing, MythicalBeastJackalKingEffect);
            AddExecutor(ExecutorType.SpSummon, Psummon);
            AddExecutor(ExecutorType.Activate, CardId.OneforOne, OneforOneEffect);
            AddExecutor(ExecutorType.Activate, CardId.ServantofEndymion, ServantofEndymionEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.CrystronHalqifibrax, CrystronHalqifibraxEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Raidraptor_ArsenalFalcon, Raidraptor_ArsenalFalconSummon);
            AddExecutor(ExecutorType.Activate, CardId.Raidraptor_ArsenalFalcon, Raidraptor_ArsenalFalconEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.HeavymetalfoesElectrumite, HeavymetalfoesElectrumiteSummon);
            //xyz mode
            AddExecutor(ExecutorType.SpSummon, CardId.Odd_EyesMeteorburstDragon, Odd_EyesMeteorburstDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.Odd_EyesMeteorburstDragon, Odd_EyesMeteorburstDragonEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Raidraptor_WiseStrix, Raidraptor_WiseStrixSummon);
            AddExecutor(ExecutorType.Activate, CardId.Raidraptor_WiseStrix, Raidraptor_WiseStrixEffect);
            AddExecutor(ExecutorType.Activate, CardId.Blackwing_ZephyrostheElite, Blackwing_ZephyrostheEliteEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Raidraptor_ForceStrix, Raidraptor_ForceStrixSummon);
            AddExecutor(ExecutorType.Activate, CardId.Raidraptor_ForceStrix, Raidraptor_ForceStrixEffect);
            AddExecutor(ExecutorType.Activate, CardId.Rank_Up_MagicSoulShaveForce, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.Raider_Wing, Raider_WingEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Raidraptor_SingingLanius);
            //xyz mode
            AddExecutor(ExecutorType.SpSummon, CardId.SaryujaSkullDread, SaryujaSkullDreadSummon);
            AddExecutor(ExecutorType.Activate, CardId.SaryujaSkullDread, SaryujaSkullDreadEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Denglong_FirstoftheYangZing, Denglong_FirstoftheYangZingSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.CrystronHalqifibrax, CrystronHalqifibraxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, LinkuribohSummon);
            AddExecutor(ExecutorType.Activate, CardId.DDLamia, DDLamiaEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonSummon);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastAuroradon, MechaPhantomBeastAuroradonEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Denglong_FirstoftheYangZing, Denglong_FirstoftheYangZingSummon);
            AddExecutor(ExecutorType.Activate, CardId.Denglong_FirstoftheYangZing, Denglong_FirstoftheYangZingEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.BorreloadSavageDragon, BorreloadSavageDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadSavageDragon, BorreloadSavageDragonEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.HeraldoftheArcLight);
            AddExecutor(ExecutorType.SpSummon, CardId.F_A_DawnDragster, F_A_DawnDragsterSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PSY_FramelordOmega, BorreloadSavageDragonSummon);
            AddExecutor(ExecutorType.Activate, CardId.PSY_FramelordOmega, PSY_FramelordOmegaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, LinkuribohEffect);
            AddExecutor(ExecutorType.Activate, CardId.MechaPhantomBeastO_Lion);
            AddExecutor(ExecutorType.Activate, CardId.JetSynchron, JetSynchronEffect);
            AddExecutor(ExecutorType.Activate, CardId.Blackwing_ZephyrostheElite, Blackwing_ZephyrostheEliteEffect_2);
            AddExecutor(ExecutorType.Summon, CardId.JetSynchron, DDLamiaSummon);
            AddExecutor(ExecutorType.Summon, CardId.DDLamia, DDLamiaSummon);
            AddExecutor(ExecutorType.Summon, CardId.Deskbot001, DDLamiaSummon);
            AddExecutor(ExecutorType.Summon, CardId.LightoftheYangZing, DDLamiaSummon);
            List<int> p_summon_ids = new List<int>() {CardId.Zefraxi_TreasureoftheYangZing,CardId.SatellarknightZefrathuban,CardId.ServantofEndymion,CardId.RitualBeastTamerZeframpilica,
            CardId.DDSavantKepler,CardId.StellarknightZefraxciton,CardId.ShaddollZefracore,CardId.SupremeKingDragonDarkwurm};
            for (int i = 0; i < p_summon_ids.Count; ++i) AddExecutor(ExecutorType.Summon, p_summon_ids[i], DefaultSummon);
            AddExecutor(ExecutorType.Summon, DefaultSummon);
            AddExecutor(ExecutorType.Activate, CardId.Deskbot001, ResetFlag);
            AddExecutor(ExecutorType.Activate, CardId.TheMightyMasterofMagic, TheMightyMasterofMagicEffect);
            AddExecutor(ExecutorType.Activate, CardId.DestrudotheLostDragon_Frisson, DestrudotheLostDragon_FrissonEffect);
            AddExecutor(ExecutorType.Summon, CardId.Blackwing_ZephyrostheElite, DefaultSummon_2);
            AddExecutor(ExecutorType.Summon, DefaultSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
            AddExecutor(ExecutorType.SpellSet, SpellSet_2);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.Activate, DefaultPActivate);
            AddExecutor(ExecutorType.GoToEndPhase, GoToEndPhase);
        }
        public override void OnNewTurn()
        {
            if (duel_start)
            {
                duel_start = false;
                AI.SendCustomChat((int)CustomMessage.Happy);
            }
            activate_SupremeKingDragonDarkwurm_1 = false;
            activate_SupremeKingDragonDarkwurm_2 = false;
            activate_JetSynchron = false;
            activate_DestrudotheLostDragon_Frisson = false;
            activate_ZefraProvidence = false;
            activate_OracleofZefra = false;
            activate_DragonShrine = false;
            activate_p_Zefraath = false;
            p_summoned = false;
            summoned = false;
            activate_DarkContractwiththGate = false;
            activate_SecretoftheYangZing = false;
            activate_ShaddollZefracore = false;
            activate_SpellPowerMastery = false;
            link_summoned = false;
            activate_DDLamia = false;
            xyz_mode = false;
            Blackwing_ZephyrostheElite_activate = false;
            HeavymetalfoesElectrumite_activate = false;
            spell_activate_count = 0;
            p_count = 0;
            activate_count = 0;
            summon_count = 0;
            enemy_activate = false;
        }
        private bool ZefraProvidenceEffect()
        {
            if (ActivateDescription == 96)
            {
                if (should_destory)
                {
                    should_destory = false;
                    return false;
                }
                return BeforeResult(ExecutorType.Activate);
            }
            else
            {
                activate_ZefraProvidence = true;
                return BeforeResult(ExecutorType.Activate);
            }

        }
        private List<int> CheckShouldSpsummonExtraMonster()
        {
            List<int> extra_ids = new List<int>() { CardId.HeavymetalfoesElectrumite, CardId.CrystronHalqifibrax };
            if (!Bot.HasInExtra(CardId.HeavymetalfoesElectrumite)) extra_ids.Remove(CardId.HeavymetalfoesElectrumite);
            if (!Bot.HasInExtra(CardId.CrystronHalqifibrax)) extra_ids.Remove(CardId.CrystronHalqifibrax);
            if (extra_ids.Count <= 0) return extra_ids;
            bool DD_summon_check = false;
            if (Bot.HasInExtra(CardId.CrystronHalqifibrax) && ((!summoned && HasInDeck(CardId.DDSavantKepler) && (HasInDeck(CardId.DarkContractwiththGate) || Bot.HasInHandOrInSpellZone(CardId.DarkContractwiththGate)
                ) && !activate_DarkContractwiththGate && HasInDeck(CardId.DDLamia)) || (func.CardsCheckAny(Bot.Hand, func.HasType, CardType.Tuner) &&
                (HasInDeck(CardId.AstrographSorcerer) || Bot.HasInHand(CardId.AstrographSorcerer)))))
            {
                DD_summon_check = true;
            }
            if (Bot.SpellZone[0] != null && Bot.SpellZone[4] != null)
            {
                List<ClientCard> spSummonMonster = func.GetPSpSummonMonster(Bot, Bot.SpellZone[0], Bot.SpellZone[4]);
                if (DD_summon_check && spSummonMonster != null)
                {

                    List<ClientCard> pSpsummonMonster = func.CardsCheckWhere(spSummonMonster, func.HasType, CardType.Pendulum);
                    List<ClientCard> monsterCards = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone), card =>
                    { return card.IsFaceup() && card.HasType(CardType.Pendulum); });
                    if (Func.MergeList(pSpsummonMonster, monsterCards).Count <= 0) extra_ids.Remove(CardId.HeavymetalfoesElectrumite);
                }
                else
                {
                    extra_ids.Remove(CardId.HeavymetalfoesElectrumite);
                }
            }
            else
            {
                if (!((Bot.HasInHand(CardId.OracleofZefra) && !activate_OracleofZefra) || (Bot.HasInHand(CardId.ZefraProvidence)
                      && !activate_ZefraProvidence) || (Bot.HasInHand(CardId.Zefraath) && !activate_p_Zefraath)))
                {
                    extra_ids.Clear();
                }
            }
            if (!DD_summon_check) extra_ids.Remove(CardId.HeavymetalfoesElectrumite);
            return extra_ids;
        }
        private bool DDLamiaSummon()
        {
            if (!IsCanSynchroSummon(Card.Level)) return false;
            if (Bot.HasInExtra(CardId.Linkuriboh) || (Bot.HasInExtra(CardId.CrystronHalqifibrax) &&
                Func.GetZoneCards(Bot, CardLocation.MonsterZone, true).Count > 0))
            {
                summoned = true;
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        private bool XyzModeCheck(bool flag1 = false)
        {
            return !link_summoned && !(!Bot.HasInExtra(CardId.Raidraptor_ArsenalFalcon) & flag1) && HasInDeck(CardId.Blackwing_ZephyrostheElite) && Bot.HasInExtra(CardId.Raidraptor_ForceStrix) && Bot.HasInExtra(CardId.Raidraptor_WiseStrix)
                && Bot.HasInExtra(CardId.TruKingofAllCalamities) && (HasInDeck(CardId.Raider_Wing) || Bot.HasInHand(CardId.Raider_Wing))
                && (HasInDeck(CardId.Raidraptor_SingingLanius) || Bot.HasInHand(CardId.Raidraptor_SingingLanius))
                && (HasInDeck(CardId.Rank_Up_MagicSoulShaveForce) || Bot.HasInHand(CardId.Rank_Up_MagicSoulShaveForce));
        }
        private bool Raidraptor_ForceStrixEffect()
        {
            AI.SelectCard(CardId.Raider_Wing);
            AI.SelectNextCard(CardId.Raidraptor_SingingLanius);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool Raidraptor_ForceStrixSummon()
        {
            return xyz_mode && BeforeResult(ExecutorType.Summon);
        }

        private bool Blackwing_ZephyrostheEliteEffect_2()
        {
            if (!xyz_mode && Bot.GetMonstersInMainZone().Count > 4) return false;
            List<ClientCard> cards = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.Onfield, true), card => {
                return !IsNoLinkCards(card) && !card.IsExtraCard() && !(card.Location == CardLocation.SpellZone
                && Func.IsCode(card, CardId.SaryujaSkullDread, CardId.MechaPhantomBeastAuroradon, CardId.HeavymetalfoesElectrumite, CardId.CrystronHalqifibrax, CardId.Raidraptor_WiseStrix,
                CardId.Linkuriboh));
            });
            if (cards.Count <= 0 || (cards.Count < 2 && func.CardsCheckCount(cards, func.HasLevel, 4) == cards.Count))
            {
                Blackwing_ZephyrostheElite_activate = true;
                return false;
            }
            cards.Sort((cardA, cardB) =>
            {
                if (cardA.Location != CardLocation.MonsterZone && cardB.Location == CardLocation.MonsterZone) return -1;
                if (cardA.Location == CardLocation.MonsterZone && cardB.Location != CardLocation.MonsterZone) return 1;
                if (cardA.Location == CardLocation.SpellZone && cardB.Location == CardLocation.SpellZone)
                {
                    if (cardA.IsCode(CardId.OracleofZefra) && !cardB.IsCode(CardId.OracleofZefra)) return -1;
                    if (!cardA.IsCode(CardId.OracleofZefra) && cardB.IsCode(CardId.OracleofZefra)) return 1;
                    return 0;
                }
                if (xyz_mode)
                {
                    if (cardA.Level == 4 && cardB.Level != 4) return 1;
                    if (cardA.Level != 4 && cardB.Level == 4) return -1;
                    return CardContainer.CompareCardAttack(cardA, cardB);
                }
                else
                {
                    return CardContainer.CompareCardAttack(cardA, cardB);
                }
            });
            Blackwing_ZephyrostheElite_activate = false;
            AI.SelectCard(cards);
            return BeforeResult(ExecutorType.Activate);
        }
        public override void OnChaining(int player, ClientCard card)
        {
            if (card == null) return;
            if (player == 1 && Func.IsCode(card, 14558127, 59438930, 94145021, 38814750, 73642296, 97268402))
                enemy_activate = true;
            base.OnChaining(player, card);
        }
        private bool BeforeResult(ExecutorType type)
        {
            if (type == ExecutorType.Activate)
            {
                ResetFlag();
                ++activate_count;
            }
            if (type == ExecutorType.Summon)
            {
                ++summon_count;
            }
            return true;
        }
        private bool GoToEndPhase()
        {
            if (Duel.Player == 0 && Duel.Turn == 1 && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), IsNoLinkCards) <= 0
                && activate_count + summon_count < 5 && !enemy_activate)
            {

                AI.SendCustomChat((int)CustomMessage.Angry);
                return true;
            }
            return false;

        }
        private bool DefaultPActivate()
        {
            if (PendulumActivate() && Func.IsCode(Card, CardId.Zefraxi_TreasureoftheYangZing, CardId.SecretoftheYangZing))
            {
                return Bot.HasInHandOrInSpellZone(CardId.NinePillarsofYangZing) && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.SpellZone,
                    true), card => { return Func.IsCode(Card, CardId.Zefraxi_TreasureoftheYangZing, CardId.SecretoftheYangZing); }) <= 0 && BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool Blackwing_ZephyrostheEliteEffect()
        {
            if (!xyz_mode) return false;
            return Blackwing_ZephyrostheEliteEffect_2();
        }
        private bool Raidraptor_WiseStrixSummon()
        {
            if (!xyz_mode) return false;
            AI.SelectMaterials(CardId.Raidraptor_ArsenalFalcon, CardId.Blackwing_ZephyrostheElite);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool Raidraptor_WiseStrixEffect()
        {
            if (ActivateDescription == -1)
            {
                int count = 0;
                if (HasInDeck(CardId.Raidraptor_SingingLanius)) ++count;
                if (HasInDeck(CardId.Blackwing_ZephyrostheElite)) ++count;
                if (HasInDeck(CardId.Raider_Wing)) ++count;
                if (count <= 1) return false;
                AI.SelectCard(CardId.Raider_Wing);
                return BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private bool Raidraptor_ArsenalFalconEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(CardId.SupremeKingGateZero);
                AI.SelectNextCard(new int[] { CardId.Blackwing_ZephyrostheElite, CardId.Raider_Wing, CardId.Raidraptor_SingingLanius });
                return BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool Raidraptor_ArsenalFalconSummon()
        {
            if (!XyzModeCheck(true)) return false;
            var materials_lists = Util.GetXyzMaterials(Bot.MonsterZone, 7, 2, false,
                 card => { return !card.IsCode(CardId.F_A_DawnDragster) && !card.IsCode(CardId.TheMightyMasterofMagic); });
            if (materials_lists.Count <= 0) return false;
            AI.SelectMaterials(materials_lists[0]);
            xyz_mode = true;
            return BeforeResult(ExecutorType.Summon);
        }
        private bool Odd_EyesMeteorburstDragonCheck()
        {
            if (!XyzModeCheck()) return false;
            var materials_lists = Util.GetXyzMaterials(Func.MergeList(new List<ClientCard>() { Card },
                Func.GetZoneCards(Bot, CardLocation.MonsterZone | CardLocation.PendulumZone)), 7, 2, false,
                card => { return !card.IsCode(CardId.F_A_DawnDragster) && !card.IsCode(CardId.TheMightyMasterofMagic); });
            if (materials_lists.Count <= 0) return false;
            List<ClientCard> pre_materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), SecretoftheYangZingCheck);
            var materials_sy_lists = Util.GetSynchroMaterials(pre_materials, 7, 1, 1, false, true, null, card => { return !card.IsCode(CardId.MythicalBeastJackalKing) && !card.IsCode(CardId.HeraldoftheArcLight); });
            if (materials_sy_lists.Count <= 0) return false;
            Odd_EyesMeteorburstDragon_materials.Clear();
            foreach (var materials in materials_sy_lists)
            {
                if (func.CardsCheckCount(materials, func.IsCode, CardId.SupremeKingGateZero) > 0)
                {
                    Odd_EyesMeteorburstDragon_materials.AddRange(materials);
                    return true;
                }
            }
            Odd_EyesMeteorburstDragon_materials.AddRange(materials_sy_lists[0]);
            return true;
        }
        private bool Odd_EyesMeteorburstDragonSummon()
        {
            if (!Odd_EyesMeteorburstDragonCheck()) return false;
            AI.SelectMaterials(Odd_EyesMeteorburstDragon_materials);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool Odd_EyesMeteorburstDragonEffect()
        {
            AI.SelectCard(CardId.SupremeKingGateZero);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool DDSavantKeplerSummon()
        {
            if (HasInDeck(CardId.DarkContractwiththGate))
            {
                summoned = true;
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        //        private void DebugCards(string msg, IList<ClientCard> cards)
        //        {
        //#if DEBUG
        //            Logger.DebugWriteLine(cards.Count + msg);
        //            foreach (var card in cards)
        //            {
        //                if (card == null) continue;
        //                NamedCard namedCard = NamedCard.Get(card.Id);
        //                if (namedCard == null) continue;
        //                Logger.DebugWriteLine(msg + namedCard.Name);
        //            }
        //#endif
        //        }
        private bool ServantofEndymionEffect_2()
        {
            if (Card.Location == CardLocation.SpellZone) return BeforeResult(ExecutorType.Activate);
            return false;
        }
        private bool IsSpsummonPMonster(ClientCard card)
        {
            return IsZefraScaleAbove(card) || IsZefraScaleBelow(card) || card.Id == CardId.SupremeKingGateZero || card.Id == CardId.ServantofEndymion;
        }
        private int GetSpellActivateCount()
        {
            int count = 0;
            if (!activate_DragonShrine && func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.DragonShrine) &&
                (HasInDeck(CardId.FlameBeastoftheNekroz) || HasInDeck(CardId.DestrudotheLostDragon_Frisson) || HasInDeck(CardId.SupremeKingDragonDarkwurm))) ++count;
            if (!activate_SpellPowerMastery && func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.SpellPowerMastery)
                && (HasInDeck(CardId.TheMightyMasterofMagic) || HasInDeck(CardId.ServantofEndymion))) ++count;
            if (func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.DarkContractwiththGate)) ++count;
            if (!activate_ZefraProvidence && func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.ZefraProvidence))
            {
                if (func.CardsCheckCount(Bot.Hand, func.IsCode, CardId.OracleofZefra) <= 0 && !activate_OracleofZefra
                    && HasInDeck(CardId.OracleofZefra))
                {
                    count += 2;
                }
                else
                {
                    ++count;
                }
            }
            if (!activate_OracleofZefra && func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.OracleofZefra)) ++count;
            if (func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.Terraforming) && HasInDeck(CardId.OracleofZefra)) ++count;
            if (func.CardsCheckAny(Bot.Hand, func.IsCode, CardId.FoolishBurial)) ++count;
            if (func.CardsCheckCount(Bot.Hand, func.HasType, CardType.Pendulum) > 1 && Bot.SpellZone[0] == null &&
                Bot.SpellZone[4] == null) ++count;
            if (!summoned && Bot.HasInHand(CardId.DDSavantKepler) && HasInDeck(CardId.DarkContractwiththGate)) ++count;
            return count;
        }
        private bool ServantofEndymionEffect_3()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool ZefraDivineStrikeEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            AI.SelectCard(CardId.RitualBeastTamerZeframpilica, CardId.SatellarknightZefrathuban, CardId.StellarknightZefraxciton, CardId.FlameBeastoftheNekroz, CardId.ShaddollZefracore,
                CardId.SecretoftheYangZing, CardId.Zefraxi_TreasureoftheYangZing);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool NinePillarsofYangZingEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            List<ClientCard> cards = func.CardsIdToClientCards(new List<int> { CardId.SecretoftheYangZing }, Bot.MonsterZone);
            cards.AddRange(func.CardsIdToClientCards(new List<int> { CardId.SecretoftheYangZing, CardId.Zefraxi_TreasureoftheYangZing }, Bot.SpellZone));
            AI.SelectCard(cards);
            should_destory = true;
            return BeforeResult(ExecutorType.Activate);
        }
        private bool IsActivateBlackwing_ZephyrostheElite()
        {
            return (Blackwing_ZephyrostheElite_activate || HeavymetalfoesElectrumite_activate) && Func.GetZoneCards(Bot, CardLocation.PendulumZone, true).Count <= 0;
        }
        private bool PendulumDefaultActivate()
        {
            return IsActivateBlackwing_ZephyrostheElite() || (checkPActivate() && IsActivateScale());
        }
        private bool ServantofEndymionEffect()
        {
            if (PendulumActivate())
            {
                if (IsActivateBlackwing_ZephyrostheElite()) return BeforeResult(ExecutorType.Activate);
                if ((!HasInDeck(CardId.TheMightyMasterofMagic) && !HasInDeck(CardId.MythicalBeastJackalKing) || GetSpellActivateCount() < 2)) return false;
                return BeforeResult(ExecutorType.Activate);
            }
            else if (Card.Location == CardLocation.SpellZone)
            {
                if (func.HasInZone(Bot, CardLocation.Hand | CardLocation.PendulumZone, CardId.Zefraath, true))
                {
                    return func.CardsCheckAny(Bot.Hand, IsSpsummonPMonster) && BeforeResult(ExecutorType.Activate);
                }
                return BeforeResult(ExecutorType.Activate);
            }
            else if (Card.Location == CardLocation.MonsterZone) return BeforeResult(ExecutorType.Activate);
            else return false;
        }
        private bool IsZefraScaleAbove(ClientCard card)
        {
            return Func.IsCode(card, CardId.StellarknightZefraxciton, CardId.SecretoftheYangZing, CardId.FlameBeastoftheNekroz, CardId.ShaddollZefracore);
        }
        private bool IsZefraScaleBelow(ClientCard card)
        {
            return Func.IsCode(card, CardId.RitualBeastTamerZeframpilica, CardId.Zefraxi_TreasureoftheYangZing, CardId.SatellarknightZefrathuban);
        }
        private bool TerraformingEffect()
        {
            return Bot.HasInHand(CardId.OracleofZefra) && BeforeResult(ExecutorType.Activate);
        }
        private bool DDSavantKeplerEffect()
        {
            if (PendulumActivate()) return false;
            return BeforeResult(ExecutorType.Activate);
        }
        private bool FoolishBurialEffect()
        {
            return BeforeResult(ExecutorType.Activate);
        }
        private List<ClientCard> GetSynchroMaterials()
        {
            List<ClientCard> materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true),
                card => { return !IsNoLinkCards(card) && !card.HasType(CardType.Link | CardType.Xyz); });
            return materials;
        }
        private bool DestrudotheLostDragon_FrissonEffect()
        {
            if (Bot.HasInExtra(CardId.CrystronHalqifibrax)) return BeforeResult(ExecutorType.Activate);
            if (!Bot.HasInExtra(CardId.F_A_DawnDragster) && !Bot.HasInExtra(CardId.Odd_EyesMeteorburstDragon)) return false;
            List<ClientCard> pre_materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), card => { return SecretoftheYangZingCheck(card) && !IsNoLinkCards(card) && !card.HasType(CardType.Tuner) && card.Level > 0; });
            if (pre_materials.Count <= 0) return false;
            List<ClientCard> cards = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), card =>
            {
                return !IsNoLinkCards(card) && card.Level > 0 && !card.HasType(CardType.Tuner);
            });
            if (cards.Count <= 0) return false;
            AI.SelectCard(cards);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool IsCanSynchroSummon(int level)
        {
            return func.CardsCheckAny(GetSynchroMaterials(), card => {
                return (card.Level + level == 8
                && func.CardsCheckAny(Bot.ExtraDeck, synchro_card => { return Func.IsCode(synchro_card, CardId.BorreloadSavageDragon, CardId.PSY_FramelordOmega); }))
                || (card.Level + level == 7 && SecretoftheYangZingCheck(card) && func.CardsCheckAny(Bot.ExtraDeck, synchro_card => { return Func.IsCode(synchro_card, CardId.Odd_EyesMeteorburstDragon, CardId.F_A_DawnDragster); }))
                || (card.Level + level == 5 && Bot.HasInExtra(CardId.Denglong_FirstoftheYangZing))
                || (card.Level + level == 4 && Bot.HasInExtra(CardId.HeraldoftheArcLight));
            });
        }
        private bool DDLamiaEffect()
        {
            if (Bot.HasInExtra(CardId.MechaPhantomBeastAuroradon) && Bot.GetMonstersInMainZone().Count >= 3) return false;
            if (!Bot.HasInExtra(CardId.CrystronHalqifibrax) && !IsCanSynchroSummon(Card.Level)) return false;
            AI.SelectCard(CardId.DarkContractwiththGate, CardId.DDSavantKepler);
            activate_DDLamia = true;
            return true;
        }
        private bool DragonShrineEffect()
        {
            return BeforeResult(ExecutorType.Activate);
        }
        private bool ZefraathEffect()
        {
            if (PendulumActivate()) return !activate_p_Zefraath || IsActivateBlackwing_ZephyrostheElite();
            if (Card.Location == CardLocation.SpellZone)
            {
                activate_p_Zefraath = true;
                return BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool RitualBeastTamerZeframpilicaEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private bool BorreloadSavageDragonSummon_2()
        {
            if (xyz_mode) return BorreloadSavageDragonSummon();
            return false;
        }
        private bool BorreloadSavageDragonSummon()
        {
            var materials_lists = Util.GetSynchroMaterials(Bot.MonsterZone, Card.Level, 1, 1, false, true, null,
                 card => { return !card.IsCode(CardId.F_A_DawnDragster) && !card.IsCode(CardId.TheMightyMasterofMagic) && !card.IsCode(CardId.HeraldoftheArcLight); });
            if (materials_lists.Count <= 0) return false;
            foreach (var materials in materials_lists)
            {
                if (func.CardsCheckAny(materials, func.IsCode, CardId.MechaPhantomBeastToken))
                {
                    AI.SelectMaterials(materials);
                    return BeforeResult(ExecutorType.Summon);
                }
            }
            AI.SelectMaterials(materials_lists[0]);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool BorreloadSavageDragonEffect()
        {
            /*
             * effect1 bug: Unable to read card info in the func "OnselectCard" 
             *              Unable to run in the "ActivateDescription == -1"
             */
            AI.SelectCard(new[]
                {
                    CardId.SaryujaSkullDread,
                    CardId.MechaPhantomBeastAuroradon,
                    CardId.HeavymetalfoesElectrumite,
                    CardId.CrystronHalqifibrax,
                    CardId.Raidraptor_WiseStrix
                });
            return BeforeResult(ExecutorType.Activate);
        }
        private bool TheMightyMasterofMagicEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            else if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.LastChainPlayer == 0) return false;
                AI.SelectCard(CardId.ServantofEndymion, CardId.TheMightyMasterofMagic);
                return BeforeResult(ExecutorType.Activate);
            }
            else
            {
                return BeforeResult(ExecutorType.Activate);
            }
        }
        private bool checkPActivate()
        {
            if (p_summoned) return false;
            if (func.HasInZone(Bot, CardLocation.PendulumZone, CardId.Zefraath, true)) return true;
            if (Bot.HasInHand(CardId.Zefraath) && (Bot.SpellZone[0] != null || Bot.SpellZone[4] != null)) return false;
            if (Bot.SpellZone[0] == null && Bot.SpellZone[4] == null)
            {
                if (!Bot.HasInHand(CardId.Zefraath) && !func.CardsCheckAny(Bot.Hand, card => {
                    return IsSpsummonPMonster(card) &&
(Card.LScale >= 5 ? card.LScale < 5 : card.LScale > 5) && func.GetPSpSummonMonster(Bot, card, Card)?.Count > 0;
                })) return false;
            }
            else
            {
                if (func.GetPSpSummonMonster(Bot, Bot.SpellZone[0], Card)?.Count <= 0 && func.GetPSpSummonMonster(Bot, Bot.SpellZone[4], Card)?.Count <= 0) return false;
            }

            return true;
        }
        private bool SecretoftheYangZingEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            activate_SecretoftheYangZing = true;
            return BeforeResult(ExecutorType.Activate);
        }
        private bool SatellarknightZefrathubanEffect()
        {

            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool BorreloadSavageDragonEffect_2()
        {
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard card = Util.GetLastChainCard();
                return card != null && !card.HasType(CardType.Continuous | CardType.Field) && card.HasType(CardType.Spell | CardType.Trap) && BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool Zefraxi_TreasureoftheYangZingEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private bool OracleofZefraEffect()
        {
            activate_OracleofZefra = true;
            return BeforeResult(ExecutorType.Activate);

        }
        private bool FlameBeastoftheNekrozEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            else
            {
                return BeforeResult(ExecutorType.Activate);
            }

        }
        private bool AstrographSorcererEffect()
        {
            if (PendulumActivate()) return false;
            return BeforeResult(ExecutorType.Activate);
        }
        private bool StellarknightZefraxcitonEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            else
            {
                return BeforeResult(ExecutorType.Activate);
            }
        }
        private bool IsNoLinkCards(ClientCard card)
        {
            if (card == null) return false;
            return ((card.IsCode(CardId.MythicalBeastJackalKing) || card.IsCode(CardId.TheMightyMasterofMagic)) && !card.IsDisabled())
                || card.IsCode(CardId.BorreloadSavageDragon) || card.IsCode(CardId.PSY_FramelordOmega) || card.IsCode(CardId.F_A_DawnDragster)
                || card.IsCode(CardId.TruKingofAllCalamities) || card.IsCode(CardId.HeraldoftheArcLight) || card.LinkCount >= 3;
        }
        private bool LinkuribohSummon()
        {
            List<ClientCard> materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasLevel, 1);
            if (func.CardsCheckCount(materials, Func.NegateFunc(func.HasType), CardType.Tuner) <= 0 &&
                func.CardsCheckCount(materials, func.HasType, CardType.Tuner) <= 1) return false;
            materials.Sort((cardA, cardB) =>
            {
                if (cardA.HasType(CardType.Tuner) && !cardB.HasType(CardType.Tuner)) return 1;
                if (!cardA.HasType(CardType.Tuner) && cardB.HasType(CardType.Tuner)) return -1;
                return 0;
            });
            AI.SelectMaterials(materials);
            return true;
        }
        private bool SpellSet()
        {
            if (Card.HasType(CardType.Trap))
            {
                AI.SelectPlace(Zones.z1 | Zones.z2 | Zones.z3 | Zones.z4 | Zones.z0);
                return true;
            }
            return false;
        }
        private bool SpellSet_2()
        {
            if (Card.HasType(CardType.QuickPlay))
            {
                AI.SelectPlace(Zones.z1 | Zones.z2 | Zones.z3 | Zones.z4 | Zones.z0);
                return true;
            }
            return false;
        }
        private bool ShaddollZefracoreEffect()
        {
            if (PendulumActivate())
            {
                return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            }
            else
            {
                return BeforeResult(ExecutorType.Activate);
            }
        }
        private bool PSY_FramelordOmegaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Duel.Player == 0) return BeforeResult(ExecutorType.Activate);
                else
                {
                    if (Bot.Banished.Count <= 0) return false;
                    AI.SelectCard(func.CardsIdToClientCards(new List<int> { CardId.JetSynchron, CardId.DDLamia }, Bot.Banished));
                    return BeforeResult(ExecutorType.Activate);
                }
            }
            else
            {
                if (Bot.Graveyard.Count <= 0) return false;
                AI.SelectCard(func.CardsIdToClientCards(new List<int> { CardId.Zefraath, CardId.CrystronHalqifibrax, CardId.Denglong_FirstoftheYangZing, CardId.BorreloadSavageDragon, CardId.DDLamia }, Bot.Graveyard));
                return BeforeResult(ExecutorType.Activate);
            }
        }
        private bool Psummon()
        {
            //if (ActivateDescription == P_SPSUMMON_DESC)
            if (Card.Location == CardLocation.SpellZone)
            {
                p_summoning = true;
                p_summoned = true;
                return true;
            }
            return false;
        }
        private bool IsExtraZoneCard(ClientCard card)
        {
            if (card == null) return false;
            ClientCard ex_card = Bot.MonsterZone[5];
            if (ex_card == card) return true;
            ex_card = Bot.MonsterZone[6];
            if (ex_card == card) return true;
            return false;
        }
        private bool HeavymetalfoesElectrumiteSummon()
        {
            if (Odd_EyesMeteorburstDragonCheck()) return false;
            List<ClientCard> materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasType, CardType.Pendulum);
            if (materials.Count > 0)
            {
                materials.Sort((cardA, cardB) =>
                {
                    if ((cardA.Level == 3 || cardA.HasType(CardType.Tuner)) && cardB.Level != 3 && !cardB.HasType(CardType.Tuner)) return -1;
                    if (cardA.Level != 3 && !cardA.HasType(CardType.Tuner) && (cardB.Level == 3 || cardB.HasType(CardType.Tuner))) return 1;
                    return CardContainer.CompareCardLevel(cardA, cardB);
                });
                materials.Reverse();
                List<ClientCard> result = new List<ClientCard>();
                foreach (var material in materials)
                {
                    if (IsExtraZoneCard(material)) result.Insert(0, material);
                    else if (IsNoLinkCards(material) || (material.HasType(CardType.Tuner) && Bot.HasInExtra(CardId.CrystronHalqifibrax)
                        && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasType, CardType.Tuner) <= 0)) continue;
                    else result.Add(material);
                }
                if (result.Count < 2) return false;
                AI.SelectMaterials(result);
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        private bool SecretoftheYangZingCheck(ClientCard card)
        {
            if (card.IsCode(CardId.SecretoftheYangZing) && Bot.HasInHandOrInSpellZone(CardId.NinePillarsofYangZing))
            {
                return func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.Hand | CardLocation.PendulumZone | CardLocation.MonsterZone, true), p_card => {
                    return p_card.HasSetcode(0xc4) && p_card.HasType(CardType.Pendulum);
                }) <= 0;
            }
            return true;
        }
        private bool F_A_DawnDragsterSummon()
        {
            List<ClientCard> pre_materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), SecretoftheYangZingCheck);
            var materials_lists = Util.GetSynchroMaterials(pre_materials, 7, 1, 1, false, true, null, card => { return !card.IsCode(CardId.MythicalBeastJackalKing) && !card.IsCode(CardId.HeraldoftheArcLight); });
            if (materials_lists.Count <= 0) return false;
            foreach (var materials in materials_lists)
            {
                if (func.CardsCheckCount(materials, card =>
                {
                    return card.HasType(CardType.Tuner) && card.HasRace(CardRace.Machine);
                }) <= 0)
                {
                    AI.SelectMaterials(materials);
                    return BeforeResult(ExecutorType.Summon);
                }
            }
            AI.SelectMaterials(materials_lists[0]);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool CrystronHalqifibraxEffect()
        {
            return BeforeResult(ExecutorType.Activate);
        }
        private bool MechaPhantomBeastAuroradonSummon()
        {
            if (Bot.GetMonstersInMainZone().Count >= 4 || (!HasInDeck(CardId.MechaPhantomBeastO_Lion) && !IsCanSPSummonTunerLevel1()
                && !func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.MonsterZone | CardLocation.Grave, true), func.IsCode, CardId.Deskbot001))) return false;
            if (XyzModeCheck())
            {
                List<ClientCard> pre_materials = new List<ClientCard>();
                List<ClientCard> key_materials = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.Grave), card => {
                    return (card.IsCode(CardId.DDLamia) && !activate_DDLamia && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.Onfield | CardLocation.Hand, true), scard =>
                    { return Func.HasSetCode(scard, 0xaf, 0xae) && scard.Id != CardId.DDLamia; }) > 0);
                });
                List<ClientCard> key_materials_2 = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.Grave), card => { return card.IsCode(CardId.JetSynchron) && !activate_JetSynchron; });
                pre_materials.AddRange(key_materials);
                pre_materials.AddRange(key_materials_2);
                if (!summoned) pre_materials.AddRange(func.CardsCheckWhere(Bot.Hand, card => { return !card.IsCode(CardId.DestrudotheLostDragon_Frisson) && card.Level < 5; }));
                pre_materials.AddRange(Bot.MonsterZone);
                var synchro_materials_lists = Util.GetSynchroMaterials(pre_materials, 7, 1, 1, false, true, null, card => { return !card.IsCode(CardId.MythicalBeastJackalKing); });
                var xyz_materials_lists = Util.GetXyzMaterials(Func.GetZoneCards(Bot, CardLocation.MonsterZone | CardLocation.PendulumZone), 7, 1, false,
                card => { return !card.IsCode(CardId.F_A_DawnDragster) && !card.IsCode(CardId.TheMightyMasterofMagic); });
                var xyz_materials_lists_2 = Util.GetXyzMaterials(Func.GetZoneCards(Bot, CardLocation.MonsterZone), 7, 2, false,
               card => { return !card.IsCode(CardId.F_A_DawnDragster) && !card.IsCode(CardId.TheMightyMasterofMagic); });
                if ((synchro_materials_lists.Count > 0 && xyz_materials_lists.Count > 0) || xyz_materials_lists_2.Count > 0) return false;
            }
            List<ClientCard> m = new List<ClientCard>();
            int link_count = 0;
            List<ClientCard> cards = Bot.GetMonsters();
            cards.Sort(CardContainer.CompareCardLink);
            cards.Reverse();
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null) continue;
                if (card.IsFacedown() || !card.HasRace(CardRace.Machine) || IsNoLinkCards(card)) continue;
                m.Add(card);
                link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                if (link_count >= 3) break;
            }
            if (link_count < 3) return false;
            AI.SelectMaterials(m);
            return true;
        }
        private bool SaryujaSkullDreadEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.SaryujaSkullDread, 2))
            {
                AI.SelectCard(GetSendToDeckIds());
                return BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private bool SaryujaSkullDreadSummon()
        {
            if (Bot.GetMonstersInMainZone().Count < 4 || (!Bot.HasInExtra(CardId.CrystronHalqifibrax) && !xyz_mode)) return false;
            List<ClientCard> materials = new List<ClientCard>();
            int link_count = 0;
            int materials_count = 0;
            int tuner_count = func.CardsCheckCount(Bot.MonsterZone, func.HasType, CardType.Tuner);
            List<ClientCard> temp_materials = Bot.GetMonsters();
            temp_materials.Sort((cardA, cardB) =>
            {
                if ((cardA.HasType(CardType.Tuner) && cardB.HasType(CardType.Tuner))
                || (!cardA.HasType(CardType.Tuner) && !cardB.HasType(CardType.Tuner)))
                {
                    return CardContainer.CompareCardLevel(cardA, cardB);
                }
                else if (cardA.HasType(CardType.Tuner) && !cardB.HasType(CardType.Tuner)) return 1;
                return -1;
            });
            foreach (var material in temp_materials)
            {
                ++materials_count;
                if (IsExtraZoneCard(material)) materials.Insert(0, material);
                else if (IsNoLinkCards(material)) { --materials_count; continue; }
                else materials.Add(material);
                link_count += material.HasType(CardType.Link) ? material.LinkCount : 1;
                if (link_count >= 4)
                {
                    if (materials_count == 3 && Bot.Deck.Count > 4 && ((func.CardsCheckCount(Bot.Hand, func.HasType, CardType.Tuner) > 0
                        || (Bot.HasInMonstersZone(CardId.DDLamia, false, false, true) && !activate_DDLamia && func.CardsCheckCount(Func.GetZoneCards
                        (Bot, CardLocation.Onfield | CardLocation.Hand, true), card => { return Func.HasSetCode(card, 0xaf, 0xae) && card.Id != CardId.DDLamia; })
                        > 0) || (Bot.HasInMonstersZone(CardId.JetSynchron, false, false, true) && !activate_JetSynchron)) || xyz_mode))
                    {
                        --link_count;
                        continue;
                    }
                    break;
                }
            }
            if (materials.Count < 3) return false;
            AI.SelectMaterials(materials);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool CrystronHalqifibraxSummon()
        {
            List<ClientCard> materials = new List<ClientCard>();
            if (Bot.HasInExtra(CardId.MechaPhantomBeastAuroradon))
            {
                materials.Add(Bot.MonsterZone[5]);
                materials.Add(Bot.MonsterZone[6]);
            }
            List<ClientCard> mainMonsters = Bot.GetMonstersInMainZone();
            mainMonsters.Sort(CardContainer.CompareCardAttack);
            materials.AddRange(mainMonsters);
            AI.SelectMaterials(materials);
            if (materials.Distinct().Count() <= 3)
            {
                AI.SendCustomChat((int)CustomMessage.Surprise);
            }
            return true;
        }
        private bool PendulumActivate()
        {
            return Func.PendulumActivate(ActivateDescription, Card);
        }
        private bool IsActivateScale()
        {
            return func.IsActivateScale(Bot, Card);
        }
        private bool SpellActivate()
        {
            return Func.SpellActivate(Card);
        }
        private bool SupremeKingGateZeroEffect()
        {
            if (PendulumActivate()) return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            return false;
        }
        private bool MythicalBeastJackalKingEffect()
        {
            if (PendulumActivate()) return PendulumDefaultActivate() && BeforeResult(ExecutorType.Activate);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool Denglong_FirstoftheYangZingSummon_2()
        {
            if (xyz_mode) return Denglong_FirstoftheYangZingSummon();
            return false;
        }
        private bool Denglong_FirstoftheYangZingSummon()
        {
            var materials_lists = Util.GetSynchroMaterials(Bot.MonsterZone, 5, 1, 1, false, true, null,
                card => { return !card.IsCode(CardId.HeraldoftheArcLight); });
            if (materials_lists.Count <= 0) return false;
            AI.SelectMaterials(materials_lists[0]);
            return BeforeResult(ExecutorType.Summon);
        }
        private bool Denglong_FirstoftheYangZingEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Denglong_FirstoftheYangZing, 1)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(CardId.NinePillarsofYangZing, CardId.SecretoftheYangZing, CardId.Zefraxi_TreasureoftheYangZing);
            }
            else
            {
                AI.SelectCard(CardId.SecretoftheYangZing, CardId.Zefraxi_TreasureoftheYangZing, CardId.LightoftheYangZing);
            }
            return true;
        }
        private bool DarkContractwiththGateEffect()
        {
            if (SpellActivate())
            {
                return (HasInDeck(CardId.DDLamia) || func.HasInZone(Bot, CardLocation.PendulumZone, CardId.ServantofEndymion, true, true)) && BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private int DisabledSort(ClientCard cardA, ClientCard cardB)
        {
            bool RitualBeastTamerZeframpilica_flag = !summoned && Bot.HasInExtra(CardId.HeraldoftheArcLight) && IsCanSPSummonTunerLevel1();
            if (((cardA.IsCode(CardId.RitualBeastTamerZeframpilica) && RitualBeastTamerZeframpilica_flag) || Func.IsCode(cardA, CardId.MaxxC, CardId.Zefraath, CardId.MythicalBeastJackalKing, CardId.TheMightyMasterofMagic) || cardA.HasType(CardType.Trap) || cardA.HasType(CardType.Tuner))
                && !cardB.IsCode(CardId.RitualBeastTamerZeframpilica) && !cardB.HasType(CardType.Trap) && !Func.IsCode(cardB, CardId.MaxxC, CardId.Zefraath, CardId.MythicalBeastJackalKing, CardId.TheMightyMasterofMagic) && !cardB.HasType(CardType.Tuner)) return 1;
            else if (!cardA.IsCode(CardId.RitualBeastTamerZeframpilica) && !cardA.HasType(CardType.Trap) && !Func.IsCode(cardA, CardId.MaxxC, CardId.Zefraath, CardId.MythicalBeastJackalKing, CardId.TheMightyMasterofMagic) && !cardA.HasType(CardType.Tuner)
                && ((cardB.IsCode(CardId.RitualBeastTamerZeframpilica) && RitualBeastTamerZeframpilica_flag) || Func.IsCode(cardB, CardId.MaxxC, CardId.Zefraath, CardId.MythicalBeastJackalKing, CardId.TheMightyMasterofMagic) || cardB.HasType(CardType.Trap) || cardB.HasType(CardType.Tuner))) return -1;
            return 0;
        }
        private List<int> GetDisabledIds()
        {
            List<int> ids = new List<int>();
            ids.Add(CardId.DestrudotheLostDragon_Frisson);
            ids.Add(CardId.Blackwing_ZephyrostheElite);
            ids.Add(CardId.Raider_Wing);
            ids.Add(CardId.Raidraptor_SingingLanius);
            ids.Add(CardId.PSY_FrameDriver);
            if (!Bot.HasInGraveyard(CardId.Raidraptor_ArsenalFalcon) || !Bot.HasInExtra(CardId.TruKingofAllCalamities)) ids.Add(CardId.Rank_Up_MagicSoulShaveForce);
            if (Bot.HasInBanished(CardId.PSY_FrameDriver)) ids.Add(CardId.PSY_FramegearGamma);
            ids.Add(CardId.LightoftheYangZing);
            ids.Add(CardId.DDLamia);
            ids.AddRange(Func.GetCardsRepeatCardsId(Bot.Hand));
            List<ClientCard> hands = Func.GetZoneCards(Bot, CardLocation.Hand);
            hands.Sort(DisabledSort);
            List<int> hand_ids = Func.ClientCardsToCardsId(hands, true);
            ids.AddRange(hand_ids);
            return ids;
        }
        private List<int> GetSendToDeckIds()
        {
            List<int> ids = new List<int>();
            List<int> repeat_ids = Func.GetCardsRepeatCardsId(Func.GetZoneCards(Bot, CardLocation.Hand));
            ids.Add(CardId.MechaPhantomBeastO_Lion);
            ids.AddRange(repeat_ids);
            ids.Add(CardId.Raidraptor_SingingLanius);
            ids.Add(CardId.Raider_Wing);
            ids.Add(CardId.Blackwing_ZephyrostheElite);
            ids.Add(CardId.PSY_FrameDriver);
            ids.Add(CardId.LightoftheYangZing);
            ids.Add(CardId.Rank_Up_MagicSoulShaveForce);
            if (activate_ZefraProvidence) ids.Add(CardId.ZefraProvidence);
            if (activate_OracleofZefra) ids.Add(CardId.OracleofZefra);
            if (activate_DragonShrine) ids.Add(CardId.DragonShrine);
            if (activate_SpellPowerMastery) ids.Add(CardId.SpellPowerMastery);
            List<ClientCard> hands = Func.GetZoneCards(Bot, CardLocation.Hand);
            hands.Sort(DisabledSort);
            List<int> hand_ids = Func.ClientCardsToCardsId(hands, true);
            ids.AddRange(hand_ids);
            return ids;
        }
        private bool TruKingofAllCalamitiesEffect()
        {
            if (Duel.Player == 1)
            {
                AI.SelectAttributes(new CardAttribute[] { CardAttribute.Divine });
                return BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool JetSynchronEffect()
        {

            if (Card.Location == CardLocation.Grave)
            {
                if (!IsCanSynchroSummon(Card.Level)) return false;
                if (func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.Extra), card => { return card.HasType(CardType.Synchro) || Func.IsCode(card, CardId.CrystronHalqifibrax, CardId.Linkuriboh); }))
                {
                    activate_JetSynchron = true;
                    List<ClientCard> dcards = func.CardsIdToClientCards(GetDisabledIds(), Bot.Hand);
                    if (!Bot.HasInExtra(CardId.CrystronHalqifibrax) && dcards.Count <= 0) return false;
                    AI.SelectCard(dcards);
                    return BeforeResult(ExecutorType.Activate);
                }
            }
            return false;
        }
        private bool MechaPhantomBeastAuroradonEffect()
        {
            if (ActivateDescription == -1) { link_summoned = true; return true; }
            else
            {
                if (!HasInDeck(CardId.MechaPhantomBeastO_Lion)
                    && Func.GetZoneCards(Enemy, CardLocation.Onfield).Count <= 0) return false;
                List<ClientCard> tRelease = new List<ClientCard>();
                List<ClientCard> nRelease = new List<ClientCard>();
                foreach (var card in Bot.GetMonsters())
                {
                    if (card == null || IsNoLinkCards(card)) continue;
                    if (card.Id == CardId.MechaPhantomBeastToken) tRelease.Add(card); else nRelease.Add(card);
                }
                int count = tRelease.Count() + nRelease.Count();
                opt_0 = false;
                opt_1 = false;
                opt_2 = false;
                if (count >= 3 && func.CardsCheckCount(Bot.Graveyard, func.HasType, CardType.Trap) > 0) opt_2 = true;
                if (count >= 2 && CheckRemainInDeck(CardId.MechaPhantomBeastO_Lion) > 0) opt_1 = true;
                if (count >= 1 && Func.GetZoneCards(Enemy, CardLocation.Onfield).Count > 0) opt_0 = true;
                if (!opt_0 && !opt_1 && !opt_2) return false;
                return true;
            }
        }
        private bool SupremeKingDragonDarkwurmEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                activate_SupremeKingDragonDarkwurm_1 = true;
                return BeforeResult(ExecutorType.Activate);
            }
            if (Card.Location == CardLocation.MonsterZone)
            {
                activate_SupremeKingDragonDarkwurm_2 = true;
                return BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool SupremeKingDragonDarkwurmSummon()
        {
            if ((!activate_p_Zefraath && Bot.HasInHand(CardId.Zefraath) && !activate_SupremeKingDragonDarkwurm_1 && HasInDeck(CardId.SupremeKingGateZero) && func.CardsCheckAny(Bot.Hand, func.HasType, CardType.Tuner))
                || (func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.Hand), card => { return card.LinkCount > 5; }) &&
                !Bot.HasInHand(CardId.SupremeKingGateZero) && !activate_SupremeKingDragonDarkwurm_2))
            {
                summoned = true;
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        private bool DefaultSummon_2()
        {
            if (Card.Location == CardLocation.Hand && Card.Level <= 4
                && Bot.HasInExtra(CardId.CrystronHalqifibrax) && (func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), card => { return Card.HasType(CardType.Tuner) ? true : card.HasType(CardType.Tuner); })))
            {
                summoned = true;
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        private bool IsCanSPSummonTunerLevel1()
        {
            return func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.MonsterZone | CardLocation.Grave, true), card => {
                return (card.IsCode(CardId.DDLamia) && !activate_DDLamia && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.Onfield | CardLocation.Hand, true), scard => { return Func.HasSetCode(scard, 0xaf, 0xae) && scard.Id != CardId.DDLamia; })
                    > 0) || (card.IsCode(CardId.JetSynchron) && !activate_JetSynchron) && Bot.GetMonstersInMainZone().Count <= 3;
            });
        }
        private bool DefaultSummon()
        {
            if (Card.Level > 4) return false;
            if ((!link_summoned && Bot.HasInExtra(CardId.HeavymetalfoesElectrumite) && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasType, CardType.Pendulum) > 0
                && Card.HasType(CardType.Pendulum))
                || (IsCanSPSummonTunerLevel1() && ((Card.Level == 3 && Bot.HasInExtra(CardId.HeraldoftheArcLight)) || (
                Card.Level == 4 && Bot.HasInExtra(CardId.Denglong_FirstoftheYangZing)))) ||
                (Card.Id == CardId.SupremeKingDragonDarkwurm && !activate_SupremeKingDragonDarkwurm_2)
                || (Bot.HasInExtra(CardId.CrystronHalqifibrax) && Bot.HasInHandOrInGraveyard(CardId.DestrudotheLostDragon_Frisson) && !activate_DestrudotheLostDragon_Frisson))
            {
                summoned = true;
                return BeforeResult(ExecutorType.Summon);
            }
            return false;
        }
        private bool OneforOneEffect()
        {
            AI.SelectCard(GetDisabledIds());
            AI.SelectNextCard(CardId.JetSynchron, CardId.LightoftheYangZing, CardId.DDLamia);
            return BeforeResult(ExecutorType.Activate);
        }
        private void HeavymetalfoesElectrumiteAddIds(List<int> ids)
        {
            if (!summoned && HasInDeck(CardId.DarkContractwiththGate) && HasInDeck(CardId.DDLamia))
            {
                if (!func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasType, CardType.Tuner))
                {
                    ids.Add(CardId.DDSavantKepler);
                }
                else
                {
                    ids.Add(CardId.AstrographSorcerer);
                    ids.Add(CardId.DDSavantKepler);
                }
            }
            ids.Add(CardId.AstrographSorcerer);
            ids.Add(CardId.FlameBeastoftheNekroz);
            ids.Add(CardId.DDSavantKepler);
        }
        private bool LinkuribohEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Duel.Player != 0) return BeforeResult(ExecutorType.Activate);
                return false;
            }
            return BeforeResult(ExecutorType.Activate);
        }
        private bool Raider_WingEffect()
        {
            if (!Bot.HasInMonstersZone(CardId.Raidraptor_ForceStrix, false, true, true)) return false;
            AI.SelectCard(CardId.Raidraptor_ForceStrix);
            return BeforeResult(ExecutorType.Activate);
        }
        private bool HeavymetalfoesElectrumiteEffect()
        {
            if (ActivateDescription != -1)
            {
                List<ClientCard> cards = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.SpellZone, true), Func.NegateFunc(func.IsCode), CardId.DarkContractwiththGate);
                if (cards.Count <= 0)
                {
                    HeavymetalfoesElectrumite_activate = true;
                    return false;
                }
                HeavymetalfoesElectrumite_activate = false;
                return BeforeResult(ExecutorType.Activate);
            }
            return BeforeResult(ExecutorType.Activate);
        }
        public override bool OnSelectHand()
        {
            return true;
        }
        private bool ResetFlag()
        {
            should_destory = false;
            return true;
        }
        private bool HeraldoftheArcLightEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                return Duel.LastChainPlayer != 0 && BeforeResult(ExecutorType.Activate);
            }
            return false;
        }
        private bool CalledbytheGraveEffect()
        {
            if ((Bot.SpellZone[5] == Card || Bot.SpellZone[0] == Card) && Duel.Player == 0) return BeforeResult(ExecutorType.Activate);
            ClientCard card = Util.GetLastChainCard();
            if (card == null) return false;
            int id = card.Id;
            List<ClientCard> g_cards = func.CardsCheckWhere(Enemy.Graveyard, func.IsCode, id);
            if (Duel.LastChainPlayer != 0)
            {
                if (card.Location == CardLocation.Grave && card.HasType(CardType.Monster))
                {
                    AI.SelectCard(card);
                    return BeforeResult(ExecutorType.Activate);
                }
                else if (g_cards.Count() > 0 && card.HasType(CardType.Monster))
                {
                    AI.SelectCard(g_cards);
                    return BeforeResult(ExecutorType.Activate);
                }
            }
            return false;
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            NamedCard card = NamedCard.Get(cardId);
            if (cardId == CardId.F_A_DawnDragster && Duel.Turn > 1) return CardPosition.FaceUpAttack;
            if (card.Attack <= 1000) return CardPosition.FaceUpDefence;
            return base.OnSelectPosition(cardId, positions);
        }
        public override int OnSelectOption(IList<int> options)
        {
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
            NamedCard card = NamedCard.Get(cardId);
            if (player == 0)
            {
                if (location == CardLocation.SpellZone)
                {
                    if (card.HasType(CardType.Pendulum))
                    {
                        if ((available & Zones.z4) > 0) return Zones.z4;
                        if ((available & Zones.z0) > 0) return Zones.z0;
                    }
                    else
                    {
                        List<int> keys = new List<int>() { 1, 2, 3 };
                        while (keys.Count > 0)
                        {
                            int index = Program.Rand.Next(keys.Count);
                            int key = keys[index];
                            int zone = 1 << key;
                            if ((zone & available) > 0) return zone;
                            keys.Remove(key);
                        }
                    }
                }
                else if (location == CardLocation.MonsterZone)
                {
                    if (card.HasType(CardType.Link))
                    {
                        if ((available & Zones.z5) > 0) return Zones.z5;
                        if ((available & Zones.z6) > 0) return Zones.z6;
                    }
                }
            }

            return base.OnSelectPlace(cardId, player, location, available);
        }
        private IList<ClientCard> _OnSelectPendulumSummon(IList<ClientCard> cards, int min, int max)
        {
            List<int> ids = func.GetSelectCardIdList();
            List<ClientCard> result = func.GetSelectCardList();
            List<ClientCard> exs = func.CardsCheckWhere(cards, func.IsLocation, CardLocation.Extra);
            List<ClientCard> hs = func.CardsCheckWhere(cards, Func.NegateFunc(func.IsLocation), CardLocation.Extra);
            if (func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.PendulumZone, true), card => {
                return card.HasSetcode(0xc4) && !card.IsCode(CardId.Zefraath);
            }) && func.CardsCheckAny(exs, func.IsCode, CardId.ShaddollZefracore)) ids.Add(CardId.ShaddollZefracore);
            result = func.CardsIdToClientCards(ids, cards);
            List<ClientCard> temp_cards = func.CardsCheckWhere(cards, Func.NegateFunc(func.IsCode), CardId.MaxxC);
            result.AddRange(temp_cards);
            if (result.Count <= 0) return Func.CheckSelectCount(Util, result, cards, min, min);
            if (result[0] != null && result[0].Location != CardLocation.Extra) ++p_count;
            return Func.CheckSelectCount(Util, result, cards, max, max);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (AI.HaveSelectedCards()) return null;
            List<int> ids = func.GetSelectCardIdList();
            List<ClientCard> result = func.GetSelectCardList();
            if (hint == HintMsg.AddToHand)
            {
                if (func.CardsCheckAny(cards, card => { return card.Location == CardLocation.Deck && card.HasSetcode(0xc4); }))
                {
                    if (!activate_ZefraProvidence) ids.Add(CardId.ZefraProvidence);
                    if (p_summoned)
                    {
                        if (!summoned && Bot.HasInExtra(CardId.HeavymetalfoesElectrumite) && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.MonsterZone, true), func.HasType, CardType.Pendulum) == 1)
                        {
                            List<int> pre_ids = new List<int> {CardId.Zefraxi_TreasureoftheYangZing,CardId.StellarknightZefraxciton,CardId.RitualBeastTamerZeframpilica,CardId.NinePillarsofYangZing
                            ,CardId.StellarknightZefraxciton,CardId.ShaddollZefracore};
                            ids.AddRange(pre_ids);
                        }
                        ids.Add(CardId.ZefraDivineStrike);
                    }
                    if (!activate_OracleofZefra) ids.Add(CardId.OracleofZefra);
                    if (!activate_p_Zefraath && !func.HasInZone(Bot, CardLocation.Hand | CardLocation.PendulumZone, CardId.Zefraath, true)) ids.Add(CardId.Zefraath);
                    if (func.HasInZone(Bot, CardLocation.Hand | CardLocation.PendulumZone, CardId.SupremeKingGateZero, true) && !func.CardsCheckAny(Bot.Hand, func.HasType, CardType.Tuner)
                        && !Bot.HasInHand(CardId.Zefraxi_TreasureoftheYangZing)) ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                    List<ClientCard> pMonsters = func.CardsCheckWhere(Func.GetZoneCards(Bot, CardLocation.Hand), card =>
                    {
                        return card.HasType(CardType.Pendulum) && !card.IsCode(CardId.Zefraath);
                    });
                    if (pMonsters.Count > 0)
                    {
                        List<ClientCard> zefraPMonsters = func.CardsCheckWhere(pMonsters, func.HasSetCode, 0xc4);
                        if (zefraPMonsters.Count > 0)
                        {
                            zefraPMonsters.Sort(Func.CompareCardScale);
                            int minScale = zefraPMonsters[0].RScale;
                            if (Bot.HasInHand(CardId.Zefraath))
                            {
                                if (minScale < 5)
                                {
                                    if (func.CardsCheckCount(cards, IsZefraScaleAbove) > 1)
                                    {
                                        ids.Add(CardId.ShaddollZefracore);
                                        if (!Bot.HasInHand(CardId.Zefraxi_TreasureoftheYangZing)) ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                                        ids.Add(CardId.SecretoftheYangZing);
                                        ids.Add(CardId.FlameBeastoftheNekroz);
                                        ids.Add(CardId.StellarknightZefraxciton);
                                        ids.Add(CardId.SatellarknightZefrathuban);
                                        ids.Add(CardId.RitualBeastTamerZeframpilica);
                                    }
                                    else
                                    {
                                        ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                                        ids.Add(CardId.RitualBeastTamerZeframpilica);
                                        ids.Add(CardId.SatellarknightZefrathuban);
                                    }
                                }
                                else
                                {
                                    if (func.CardsCheckCount(cards, IsZefraScaleBelow) > 1)
                                    {
                                        ids.Add(CardId.ShaddollZefracore);
                                        if (!Bot.HasInHand(CardId.Zefraxi_TreasureoftheYangZing)) ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                                        ids.Add(CardId.SecretoftheYangZing);
                                        ids.Add(CardId.FlameBeastoftheNekroz);
                                        ids.Add(CardId.StellarknightZefraxciton);
                                        ids.Add(CardId.SatellarknightZefrathuban);
                                        ids.Add(CardId.RitualBeastTamerZeframpilica);
                                    }
                                    else
                                    {
                                        ids.Add(CardId.StellarknightZefraxciton);
                                        ids.Add(CardId.SecretoftheYangZing);
                                        ids.Add(CardId.FlameBeastoftheNekroz);
                                        ids.Add(CardId.ShaddollZefracore);
                                    }
                                }
                            }
                            else
                            {
                                if (Bot.HasInGraveyard(CardId.FlameBeastoftheNekroz)) ids.Add(CardId.RitualBeastTamerZeframpilica);
                                ids.Add(CardId.SecretoftheYangZing);
                                ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                            }
                        }
                        else
                        {
                            ids.Add(CardId.SecretoftheYangZing);
                            ids.Add(CardId.FlameBeastoftheNekroz);
                            ids.Add(CardId.StellarknightZefraxciton);
                            ids.Add(CardId.SatellarknightZefrathuban);
                            ids.Add(CardId.RitualBeastTamerZeframpilica);
                            ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                        }
                    }
                    else
                    {
                        if (func.HasInZone(Bot, CardLocation.Hand | CardLocation.PendulumZone, CardId.Zefraath, true) &&
                            !activate_p_Zefraath)
                        {
                            ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                            ids.Add(CardId.SatellarknightZefrathuban);
                            ids.Add(CardId.RitualBeastTamerZeframpilica);
                        }
                        ids.Add(CardId.SecretoftheYangZing);
                        ids.Add(CardId.FlameBeastoftheNekroz);
                        ids.Add(CardId.StellarknightZefraxciton);
                        ids.Add(CardId.SatellarknightZefrathuban);
                        ids.Add(CardId.RitualBeastTamerZeframpilica);
                        ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                    }
                    result = func.CardsIdToClientCards(ids, cards);
                }
                else if (func.CardsCheckALL(cards, func.IsLocation, true, CardLocation.Extra))
                {
                    HeavymetalfoesElectrumiteAddIds(ids);
                    result = func.CardsIdToClientCards(ids, cards);
                }
                else if (func.CardsCheckALL(cards, func.HasSetCode, true, 0x12a))
                {
                    if (!func.HasInZone(Bot, CardLocation.PendulumZone | CardLocation.Hand, CardId.ServantofEndymion, true) ||
                        (func.HasInZone(Bot, CardLocation.PendulumZone | CardLocation.Hand, CardId.ServantofEndymion, true) && (!HasInDeck(CardId.TheMightyMasterofMagic) || !HasInDeck(CardId.MythicalBeastJackalKing)))) ids.Add(CardId.ServantofEndymion);
                    ids.Add(CardId.TheMightyMasterofMagic);
                    ids.Add(CardId.MythicalBeastJackalKing);
                    result = func.CardsIdToClientCards(ids, cards);
                }
                else if (func.CardsCheckALL(cards, func.HasSetCode, true, 0xaf))
                {
                    ids.Add(CardId.DDLamia);
                    ids.Add(CardId.DDSavantKepler);
                    result = func.CardsIdToClientCards(ids, cards);
                }
            }
            else if (hint == HintMsg.ToDeck && func.CardsCheckALL(cards, func.IsLocation, true, CardLocation.Hand) && min == 3 && max == 3)
            {
                result = func.CardsIdToClientCards(GetSendToDeckIds(), cards);
            }
            else if (hint == HintMsg.ToGrave && func.CardsCheckALL(cards, func.IsLocation, true, CardLocation.Deck))
            {
                List<int> extra_ids = CheckShouldSpsummonExtraMonster();
                //if (func.CardsCheckAny(cards, Func.HasRace, CardRace.Dragon))
                if (extra_ids.Count <= 0)
                {
                    if (!activate_SupremeKingDragonDarkwurm_2 && Bot.GetMonsterCount() <= 0) ids.Add(CardId.SupremeKingDragonDarkwurm);
                    if (!activate_DestrudotheLostDragon_Frisson) ids.Add(CardId.DestrudotheLostDragon_Frisson);
                    if (!activate_JetSynchron) ids.Add(CardId.JetSynchron);
                    ids.Add(CardId.FlameBeastoftheNekroz);
                }
                else if (extra_ids.Count > 1)
                {
                    if (Bot.GetMonsterCount() <= 0 && !activate_SupremeKingDragonDarkwurm_2) ids.Add(CardId.SupremeKingDragonDarkwurm);
                    if (func.CardsCheckAny(Bot.Hand, card => { return card.Level < 7 && card.HasType(CardType.Monster); })) ids.Add(CardId.DestrudotheLostDragon_Frisson);
                    if (Bot.GetHandCount() > 0) ids.Add(CardId.JetSynchron);
                    if (!summoned && Bot.HasInHand(CardId.RitualBeastTamerZeframpilica)) ids.Add(CardId.FlameBeastoftheNekroz);
                    ids.Add(CardId.DestrudotheLostDragon_Frisson);
                    ids.Add(CardId.JetSynchron);
                    ids.Add(CardId.SupremeKingDragonDarkwurm);
                    ids.Add(CardId.FlameBeastoftheNekroz);
                }
                else if (extra_ids.Contains(CardId.HeavymetalfoesElectrumite))
                {
                    if (Bot.GetMonsterCount() <= 0 && !activate_SupremeKingDragonDarkwurm_2) ids.Add(CardId.SupremeKingDragonDarkwurm);
                    if (!summoned && Bot.HasInHand(CardId.RitualBeastTamerZeframpilica)) ids.Add(CardId.FlameBeastoftheNekroz);
                    //if (!summoned && func.CardsCheckAny(Bot.Hand, card => { return card.Level < 7 && card.HasType(CardType.Monster); })) ids.Add(CardId.DestrudotheLostDragon_Frisson);
                    //if (Bot.GetHandCount() > 0) ids.Add(CardId.JetSynchron);
                    ids.Add(CardId.DestrudotheLostDragon_Frisson);
                    ids.Add(CardId.JetSynchron);
                    ids.Add(CardId.SupremeKingDragonDarkwurm);
                    ids.Add(CardId.FlameBeastoftheNekroz);
                }
                else if (extra_ids.Contains(CardId.CrystronHalqifibrax))
                {
                    if (func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.Hand | CardLocation.MonsterZone, true), func.HasType, CardType.Tuner))
                    {
                        if (Bot.GetMonsterCount() <= 0 && !activate_SupremeKingDragonDarkwurm_2) ids.Add(CardId.SupremeKingDragonDarkwurm);
                        ids.Add(CardId.DestrudotheLostDragon_Frisson);
                        ids.Add(CardId.JetSynchron);
                        ids.Add(CardId.SupremeKingDragonDarkwurm);
                        ids.Add(CardId.FlameBeastoftheNekroz);
                    }
                    else
                    {
                        ids.Add(CardId.DestrudotheLostDragon_Frisson);
                        ids.Add(CardId.JetSynchron);
                        ids.Add(CardId.SupremeKingDragonDarkwurm);
                        ids.Add(CardId.FlameBeastoftheNekroz);

                    }
                }
                result = func.CardsIdToClientCards(ids, cards);
            }
            else if (hint == Util.GetStringId(CardId.Zefraath, 1))
            {
                int[] pScales = Func.GetPScales(Bot);
                int rScale = pScales[0];
                int lScale = pScales[1];
                int pScale = (rScale != 5) ? rScale : lScale;
                if (pScale < 5)
                {
                    if (!activate_SecretoftheYangZing && !func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.Hand | CardLocation.Extra, true), func.IsCode, CardId.SecretoftheYangZing)) ids.Add(CardId.SecretoftheYangZing);
                    if (!activate_ShaddollZefracore && func.CardsCheckAny(Func.GetZoneCards(Bot, CardLocation.PendulumZone, true), card => { return !card.IsCode(CardId.Zefraath) && card.HasSetcode(0xc4); })) ids.Add(CardId.ShaddollZefracore);
                    ids.Add(CardId.StellarknightZefraxciton);
                    ids.Add(CardId.SecretoftheYangZing);
                    ids.Add(CardId.ShaddollZefracore);
                }
                else
                {
                    ids.Add(CardId.Zefraxi_TreasureoftheYangZing);
                    ids.Add(CardId.SatellarknightZefrathuban);
                    ids.Add(CardId.RitualBeastTamerZeframpilica);
                }
                result = func.CardsIdToClientCards(ids, cards);
            }
            else if (hint == Util.GetStringId(CardId.HeavymetalfoesElectrumite, 3))
            {
                HeavymetalfoesElectrumiteAddIds(ids);
                result = func.CardsIdToClientCards(ids, cards);
            }
            else if (!(IS_YGOPRO & !(hint == HintMsg.SpSummon)) && func.CardsCheckALL(cards, card => {
                return card.IsCode(CardId.TheMightyMasterofMagic) || card.IsCode(CardId.MythicalBeastJackalKing);
            }, true))
            {
                ids.Add(CardId.MythicalBeastJackalKing);
                ids.Add(CardId.TheMightyMasterofMagic);
                result = func.CardsIdToClientCards(ids, cards);
            }
            else if (p_summoning || ((Card == Bot.SpellZone[0] || Card == Bot.SpellZone[4]) && hint == HintMsg.SpSummon &&
                Card.HasType(CardType.Pendulum)))
            {
                p_summoning = false;
                if (p_count >= 3 && !Bot.HasInExtra(CardId.SaryujaSkullDread) && Bot.HasInExtra(CardId.MechaPhantomBeastAuroradon)) return Func.CheckSelectCount(Util, result, cards, min, min);
                return _OnSelectPendulumSummon(cards, min, max);
            }
            else if (hint == HintMsg.Destroy)
            {
                if (func.CardsCheckALL(cards, card => { return card.Controller == 0 && card.IsFaceup(); }, true))
                {
                    should_destory = true;
                    if (func.CardsCheckALL(cards, func.HasSetCode, true, 0x9e))
                    {
                        if (!activate_SecretoftheYangZing) result = func.CardsIdToClientCards(new List<int> { CardId.SecretoftheYangZing }, func.CardsCheckWhere(cards,
                            func.IsLocation, CardLocation.MonsterZone));
                        result.AddRange(func.CardsIdToClientCards(new List<int> { CardId.SecretoftheYangZing, CardId.Zefraxi_TreasureoftheYangZing }, func.CardsCheckWhere(cards,
                            Func.NegateFunc(func.IsLocation), CardLocation.MonsterZone)));
                    }
                    else
                    {
                        List<ClientCard> scards = func.CardsCheckWhere(cards, card => { return card.Location == CardLocation.SpellZone; });
                        scards.Sort((cardA, cardB) =>
                        {
                            if (Func.IsCode(cardA, CardId.OracleofZefra, CardId.DarkContractwiththGate) && !Func.IsCode(cardB, CardId.OracleofZefra, CardId.DarkContractwiththGate)) return 1;
                            if (!Func.IsCode(cardA, CardId.OracleofZefra, CardId.DarkContractwiththGate) && Func.IsCode(cardB, CardId.OracleofZefra, CardId.DarkContractwiththGate)) return -1;
                            return 0;
                        });
                        result.AddRange(scards);
                    }

                }
                else if (func.CardsCheckAny(cards, card => { return card.Controller == 1 && (card.Location & CardLocation.Onfield) > 0; }) && min == 1 && max == 1)
                {
                    ClientCard card = Util.GetBestEnemyCard();
                    if (card != null && cards.Contains(card)) result.Add(card);
                    else
                    {
                        result = new List<ClientCard>(func.CardsCheckWhere(cards, ecard => { return ecard.Controller == 1; }));
                        if (result.Count <= 0) return null;
                        result.Sort(CardContainer.CompareCardAttack);
                        result.Reverse();
                    }
                }
            }
            else if (hint == HintMsg.SpSummon)
            {
                List<int> tuner_ids = new List<int>()
                {
                    CardId.DestrudotheLostDragon_Frisson, CardId.PSY_FrameDriver, CardId.JetSynchron, CardId.PSY_FramegearGamma,CardId.LightoftheYangZing
                };
                List<int> no_tuner_ids = new List<int>()
                {
                    CardId.TheMightyMasterofMagic, CardId.MythicalBeastJackalKing, CardId.SecretoftheYangZing
                };
                if (func.CardsCheckALL(cards, func.IsLocation, true, CardLocation.Hand))
                {
                    if (summoned && Bot.HasInExtra(CardId.CrystronHalqifibrax) && func.CardsCheckCount(Bot.MonsterZone, card => {
                        return card.IsFaceup() && card.HasType(CardType.Tuner);
                    }) <= 0 && !(Bot.HasInGraveyard(CardId.DDLamia) && !activate_DDLamia
                        && func.CardsCheckCount(Func.GetZoneCards(Bot, CardLocation.Onfield | CardLocation.Hand, true), card => { return Func.HasSetCode(card, 0xaf, 0xae) && card.Id != CardId.DDLamia; })
                    <= 0) && !(Bot.HasInGraveyard(CardId.JetSynchron) && !activate_JetSynchron)
                    && !(Bot.HasInGraveyard(CardId.DestrudotheLostDragon_Frisson) && !activate_DestrudotheLostDragon_Frisson))
                    {
                        ids.AddRange(tuner_ids);
                        ids.AddRange(no_tuner_ids);
                    }
                    else
                    {
                        ids.AddRange(no_tuner_ids);
                        ids.AddRange(tuner_ids);
                    }
                    result = func.CardsIdToClientCards(ids, cards);
                }
                else if (func.CardsCheckALL(cards, card => {
                    return Func.IsCode(card, CardId.LightoftheYangZing, CardId.PSY_FramegearGamma,
                    CardId.MechaPhantomBeastO_Lion, CardId.JetSynchron, CardId.Deskbot001, CardId.DDLamia);
                }))
                {
                    if (Bot.GetMonstersInMainZone().Count <= 1) ids.Add(CardId.Deskbot001);
                    ids.Add(CardId.JetSynchron);
                    ids.Add(CardId.Deskbot001);
                    ids.Add(CardId.LightoftheYangZing);
                    ids.Add(CardId.PSY_FramegearGamma);
                    result = func.CardsIdToClientCards(ids, cards);
                }

            }
            else if (hint == HintMsg.Release && func.CardsCheckAny(cards, func.IsLocation, CardLocation.MonsterZone))
            {
                List<ClientCard> tRelease = new List<ClientCard>();
                List<ClientCard> nRelease = new List<ClientCard>();
                foreach (var card in cards)
                {
                    if (card == null || IsNoLinkCards(card)) continue;
                    if (card.Id == CardId.MechaPhantomBeastToken) tRelease.Add(card);
                    else if (card.Id == CardId.Raidraptor_WiseStrix) tRelease.Insert(0, card);
                    else nRelease.Add(card);
                }
                result.AddRange(tRelease);
                result.AddRange(nRelease);
            }
            IList<ClientCard> selectResult = Func.CheckSelectCount(Util, result, cards, min, max);
            if (selectResult == null) return base.OnSelectCard(cards, min, max, hint, cancelable);
            return selectResult;
        }
        private bool HasInDeck(int id)
        {
            return CheckRemainInDeck(id) > 0;
        }
        private int CheckRemainInDeck(int id)
        {
            switch (id)
            {
                case CardId.PSY_FrameDriver:
                    return Bot.GetRemainingCount(CardId.PSY_FrameDriver, 1);
                case CardId.Zefraath:
                    return Bot.GetRemainingCount(CardId.Zefraath, 3);
                case CardId.TheMightyMasterofMagic:
                    return Bot.GetRemainingCount(CardId.TheMightyMasterofMagic, 1);
                case CardId.AstrographSorcerer:
                    return Bot.GetRemainingCount(CardId.AstrographSorcerer, 1);
                case CardId.DestrudotheLostDragon_Frisson:
                    return Bot.GetRemainingCount(CardId.DestrudotheLostDragon_Frisson, 1);
                case CardId.SupremeKingGateZero:
                    return Bot.GetRemainingCount(CardId.SupremeKingGateZero, 2);
                case CardId.MythicalBeastJackalKing:
                    return Bot.GetRemainingCount(CardId.MythicalBeastJackalKing, 1);
                case CardId.SecretoftheYangZing:
                    return Bot.GetRemainingCount(CardId.SecretoftheYangZing, 3);
                case CardId.FlameBeastoftheNekroz:
                    return Bot.GetRemainingCount(CardId.FlameBeastoftheNekroz, 1);
                case CardId.StellarknightZefraxciton:
                    return Bot.GetRemainingCount(CardId.StellarknightZefraxciton, 1);
                case CardId.SupremeKingDragonDarkwurm:
                    return Bot.GetRemainingCount(CardId.SupremeKingDragonDarkwurm, 1);
                case CardId.Blackwing_ZephyrostheElite:
                    return Bot.GetRemainingCount(CardId.Blackwing_ZephyrostheElite, 1);
                case CardId.ShaddollZefracore:
                    return Bot.GetRemainingCount(CardId.ShaddollZefracore, 1);
                case CardId.Raidraptor_SingingLanius:
                    return Bot.GetRemainingCount(CardId.Raidraptor_SingingLanius, 1);
                case CardId.SatellarknightZefrathuban:
                    return Bot.GetRemainingCount(CardId.SatellarknightZefrathuban, 1);
                case CardId.Raider_Wing:
                    return Bot.GetRemainingCount(CardId.Raider_Wing, 1);
                case CardId.Zefraxi_TreasureoftheYangZing:
                    return Bot.GetRemainingCount(CardId.Zefraxi_TreasureoftheYangZing, 2);
                case CardId.RitualBeastTamerZeframpilica:
                    return Bot.GetRemainingCount(CardId.RitualBeastTamerZeframpilica, 1);
                case CardId.ServantofEndymion:
                    return Bot.GetRemainingCount(CardId.ServantofEndymion, 3);
                case CardId.PSY_FramegearGamma:
                    return Bot.GetRemainingCount(CardId.PSY_FramegearGamma, 3);
                case CardId.MechaPhantomBeastO_Lion:
                    return Bot.GetRemainingCount(CardId.MechaPhantomBeastO_Lion, 1);
                case CardId.MaxxC:
                    return Bot.GetRemainingCount(CardId.MaxxC, 3);
                case CardId.Deskbot001:
                    return Bot.GetRemainingCount(CardId.Deskbot001, 1);
                case CardId.JetSynchron:
                    return Bot.GetRemainingCount(CardId.JetSynchron, 1);
                case CardId.DDLamia:
                    return Bot.GetRemainingCount(CardId.DDLamia, 1);
                case CardId.DDSavantKepler:
                    return Bot.GetRemainingCount(CardId.DDSavantKepler, 1);
                case CardId.LightoftheYangZing:
                    return Bot.GetRemainingCount(CardId.LightoftheYangZing, 1);
                case CardId.Rank_Up_MagicSoulShaveForce:
                    return Bot.GetRemainingCount(CardId.Rank_Up_MagicSoulShaveForce, 1);
                case CardId.SpellPowerMastery:
                    return Bot.GetRemainingCount(CardId.SpellPowerMastery, 3);
                case CardId.DragonShrine:
                    return Bot.GetRemainingCount(CardId.DragonShrine, 3);
                case CardId.Terraforming:
                    return Bot.GetRemainingCount(CardId.Terraforming, 1);
                case CardId.ZefraProvidence:
                    return Bot.GetRemainingCount(CardId.ZefraProvidence, 3);
                case CardId.FoolishBurial:
                    return Bot.GetRemainingCount(CardId.FoolishBurial, 1);
                case CardId.CalledbytheGrave:
                    return Bot.GetRemainingCount(CardId.CalledbytheGrave, 2);
                case CardId.DarkContractwiththGate:
                    return Bot.GetRemainingCount(CardId.DarkContractwiththGate, 1);
                case CardId.OracleofZefra:
                    return Bot.GetRemainingCount(CardId.OracleofZefra, 3);
                case CardId.ZefraWar:
                    return Bot.GetRemainingCount(CardId.ZefraWar, 1);
                case CardId.ZefraDivineStrike:
                    return Bot.GetRemainingCount(CardId.ZefraDivineStrike, 1);
                case CardId.NinePillarsofYangZing:
                    return Bot.GetRemainingCount(CardId.NinePillarsofYangZing, 1);
                default:
                    return 0;
            }
        }
    }
}
