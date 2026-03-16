using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI; 

namespace WindBot.Game.AI.Decks 
{
    [Deck("ThunderDragon", "AI_ThunderDragon")]
    class ThunderDragonExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int ThunderDragonlord = 5206415;
            public const int TheBystialLubellion = 32731036;
            public const int TheChaosCreator = 90488465;
            public const int BystialDruiswurm = 6637331;
            public const int BystialMagnamhut = 33854624;
            public const int ThunderDragonroar = 29596581;
            public const int ThunderDragonhawk = 83107873;
            public const int NormalThunderDragon = 31786629;
            public const int ThunderDragondark = 56713174;
            public const int BlackDragonCollapserpent = 61901281;
            public const int WhiteDragonWyverburster = 99234526;
            public const int AloofLupine = 92998610;
            public const int BatterymanSolar = 44586426;
            public const int AshBlossom = 14558127;
            public const int G = 23434538;
            public const int DragonBusterDestructionSword = 76218313;
            public const int ThunderDragonmatrix = 20318029;
            public const int AllureofDarkness = 1475311;
            public const int GoldSarcophagus = 75500286;
            public const int ThunderDragonFusion = 95238394;
            public const int ChaosSpace = 99266988;
            public const int CalledbytheGrave = 24224830;
            public const int BrandedRegained = 34090915;
            public const int InfiniteImpermanence = 10045474;
            public const int BatterymanToken = 44586427;

            public const int ThunderDragonTitan = 41685633;
            public const int ThunderDragonColossus = 15291624;
            public const int AbyssDweller = 21044178;
            public const int UnderworldGoddessoftheClosedWorld = 98127546;
            public const int MekkKnightCrusadiaAvramax = 21887175;
            public const int AccesscodeTalker = 86066372;
            public const int BowoftheGoddess = 4280258;
            public const int KnightmareUnicorn = 38342335;
            public const int UnionCarrier = 83152482;
            public const int IP = 65741786;
            public const int CrossSheep = 50277355;
            public const int PredaplantVerteAnaconda = 70369116;
            public const int StrikerDragon = 73539069;
            public const int Linkuriboh = 41999284;
        }
        private enum Select
        {
            NormalThunderDragon,
            TheChaosCreator,
            ChaosSpace_1,
            ChaosSpace_2,
            ThunderDragonColossus,
            AccesscodeTalker,
            DestroyReplace
        };
        private const int THUNDER_COUNTD = 18;
        List<bool> selectFlag = new List<bool>()
        {
            false,false,false,false,false,false,false
        };
        List<bool> selectAtt = new List<bool>()
        {
            false,false,false,false,false,false,false
        };

        bool isSummoned = false;
        bool handActivated = false;
        bool place_CrossSheep = false;
        bool place_ThunderDragonColossus = false;
        bool place_Link_4 = false;
        bool summon_WhiteDragonWyverburster = false;
        bool summon_BlackDragonCollapserpent = false;
        bool summon_UnionCarrier = false;
        bool summon_TheBystialLubellion = false;
        bool activate_ThunderDragonFusion = false;
        bool activate_ThunderDragondark = false;
        bool activate_ThunderDragonroar = false;
        bool activate_ThunderDragonhawk = false;
        bool activate_ThunderDragonmatrix = false;
        bool activate_TheBystialLubellion_hand = false;
        bool activate_BystialMagnamhut_hand = false;
        bool activate_BystialDruiswurm_hand = false;
        bool activate_ChaosSpace_grave = false;
        bool activate_ChaosSpace_hand = false;
        bool No_SpSummon = false;

        List<int> SpSummonCardsId = new List<int>()
        { 
            CardId.ThunderDragonmatrix,CardId.BatterymanSolar ,
            CardId.AshBlossom,CardId.G,
            CardId.DragonBusterDestructionSword,CardId.AloofLupine 
        };
        List<int> NotSpSummonCardsId = new List<int>()
        {
            CardId.ThunderDragonlord,CardId.TheBystialLubellion,CardId.TheChaosCreator,
            CardId.BlackDragonCollapserpent,CardId.WhiteDragonWyverburster
        };
        List<int> Impermanence_list = new List<int>();
        List<int> should_not_negate = new List<int>
        {
            81275020, 28985331
        };
        public ThunderDragonExecutor(GameAI ai, Duel duel)
          : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, Impermanence_activate);
            AddExecutor(ExecutorType.Activate, CardId.G, GEffect);
            AddExecutor(ExecutorType.Activate, CardId.BowoftheGoddess);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddessoftheClosedWorld);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, AshBlossomEffect);
            AddExecutor(ExecutorType.Activate, CardId.TheBystialLubellion, TheBystialLubellionEffect);
            AddExecutor(ExecutorType.Activate, CardId.CalledbytheGrave, CalledbytheGraveEffect);
            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, AccesscodeTalkerEffect);
            AddExecutor(ExecutorType.Activate, CardId.BrandedRegained, BrandedRegainedEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnionCarrier, UnionCarrierEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.TheBystialLubellion, TheBystialLubellionSummon);
            AddExecutor(ExecutorType.Activate, CardId.GoldSarcophagus, GoldSarcophagusEffect);
            AddExecutor(ExecutorType.Activate, CardId.NormalThunderDragon, NormalThunderDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonmatrix, ThunderDragonmatrixEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.StrikerDragon, StrikerDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus, ThunderDragonColossusSummon_2);
            AddExecutor(ExecutorType.SpSummon, CardId.CrossSheep, CrossSheepSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrossSheep, CrossSheepEffect);
            AddExecutor(ExecutorType.Activate, CardId.TheChaosCreator);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonTitan, ThunderDragonTitanEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonColossus, ThunderDragonColossusEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonColossus, ThunderDragonColossusSummon);
            AddExecutor(ExecutorType.Summon, CardId.AloofLupine, AloofLupineSummon);
            AddExecutor(ExecutorType.Activate, CardId.AloofLupine, AloofLupineEffect);
            AddExecutor(ExecutorType.Summon, CardId.BatterymanSolar, BatterymanSolarSummon);
            AddExecutor(ExecutorType.Activate, CardId.BatterymanSolar, BatterymanSolarEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.WhiteDragonWyverburster, WhiteDragonWyverbursterSummon);
            AddExecutor(ExecutorType.Activate, CardId.WhiteDragonWyverburster);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackDragonCollapserpent, BlackDragonCollapserpentSummon);
            AddExecutor(ExecutorType.Activate, CardId.BlackDragonCollapserpent);
            AddExecutor(ExecutorType.SpSummon, CardId.UnionCarrier, UnionCarrierSummon);
            AddExecutor(ExecutorType.Activate, CardId.AllureofDarkness, AllureofDarknessEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonhawk, ThunderDragonhawkEffect);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSpace, ChaosSpaceEffect);
            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialMagnamhutEffect);
            AddExecutor(ExecutorType.Activate, CardId.BystialDruiswurm, BystialDruiswurmEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ThunderDragonlord, ThunderDragonlordSummon);
            AddExecutor(ExecutorType.Activate, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaEffect);
            AddExecutor(ExecutorType.MonsterSet, CardId.ThunderDragonmatrix, ThunderDragonmatrixSet);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonFusion, ThunderDragonFusionEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragondark, ThunderDragondarkEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonroar, ThunderDragonroarEffect);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonlord, ThunderDragonlordEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.TheChaosCreator, TheChaosCreatorSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddessoftheClosedWorld, UnderworldGoddessoftheClosedWorldSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BowoftheGoddess, BowoftheGoddessSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnionCarrier, UnionCarrierSummon_2);
            AddExecutor(ExecutorType.Activate, CardId.IP, IPEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.IP,IPSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AccesscodeTalker, BowoftheGoddessSummon);
            AddExecutor(ExecutorType.Activate, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.MekkKnightCrusadiaAvramax, MekkKnightCrusadiaAvramaxSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PredaplantVerteAnaconda, PredaplantVerteAnacondaSummon);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh);
            AddExecutor(ExecutorType.SpSummon, CardId.AbyssDweller);
            AddExecutor(ExecutorType.Activate, CardId.AbyssDweller, GEffect);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSpace, ChaosSpaceEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragonmatrix, ThunderDragonmatrixEffect_2);
            AddExecutor(ExecutorType.Summon, CardId.ThunderDragonmatrix, ThunderDragonmatrixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BlackDragonCollapserpent, BlackDragonCollapserpentSummon_2);
            AddExecutor(ExecutorType.Summon, CardId.ThunderDragonroar, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.ThunderDragondark, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.NormalThunderDragon, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.DragonBusterDestructionSword, ThunderDragonmatrixSummon);
            AddExecutor(ExecutorType.Summon, CardId.AloofLupine, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, DefaultSummon);
            AddExecutor(ExecutorType.Summon, CardId.G, DefaultSummon);
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragondark, ThunderDragondarkEffect_2);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
            AddExecutor(ExecutorType.Activate, CardId.AllureofDarkness, AllureofDarknessEffect_2);
            AddExecutor(ExecutorType.Activate, CardId.UnionCarrier, UnionCarrierEffect_2);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

        }
        #region DeckCheck
        public int CheckRemainInDeck(int id)
        {
            switch (id)
            {
                case CardId.ThunderDragonlord:
                    return Bot.GetRemainingCount(CardId.ThunderDragonlord, 1);
                case CardId.TheBystialLubellion:
                    return Bot.GetRemainingCount(CardId.TheBystialLubellion, 2);
                case CardId.TheChaosCreator:
                    return Bot.GetRemainingCount(CardId.TheChaosCreator, 1);
                case CardId.BystialDruiswurm:
                    return Bot.GetRemainingCount(CardId.BystialDruiswurm, 2);
                case CardId.BystialMagnamhut:
                    return Bot.GetRemainingCount(CardId.BystialMagnamhut, 2);
                case CardId.ThunderDragonroar:
                    return Bot.GetRemainingCount(CardId.ThunderDragonroar, 2);
                case CardId.ThunderDragonhawk:
                    return Bot.GetRemainingCount(CardId.ThunderDragonhawk, 2);
                case CardId.NormalThunderDragon:
                    return Bot.GetRemainingCount(CardId.NormalThunderDragon, 3);
                case CardId.ThunderDragondark:
                    return Bot.GetRemainingCount(CardId.ThunderDragondark, 3);
                case CardId.BlackDragonCollapserpent:
                    return Bot.GetRemainingCount(CardId.BlackDragonCollapserpent, 2);
                case CardId.WhiteDragonWyverburster:
                    return Bot.GetRemainingCount(CardId.WhiteDragonWyverburster, 2);
                case CardId.AloofLupine:
                    return Bot.GetRemainingCount(CardId.AloofLupine, 2);
                case CardId.BatterymanSolar:
                    return Bot.GetRemainingCount(CardId.BatterymanSolar, 3);
                case CardId.AshBlossom:
                    return Bot.GetRemainingCount(CardId.AshBlossom, 2);
                case CardId.G:
                    return Bot.GetRemainingCount(CardId.G, 3);
                case CardId.DragonBusterDestructionSword:
                    return Bot.GetRemainingCount(CardId.DragonBusterDestructionSword, 1);
                case CardId.ThunderDragonmatrix:
                    return Bot.GetRemainingCount(CardId.ThunderDragonmatrix, 3);
                case CardId.AllureofDarkness:
                    return Bot.GetRemainingCount(CardId.AllureofDarkness, 3);
                case CardId.GoldSarcophagus:
                    return Bot.GetRemainingCount(CardId.GoldSarcophagus, 1);
                case CardId.ThunderDragonFusion:
                    return Bot.GetRemainingCount(CardId.ThunderDragonFusion, 2);
                case CardId.ChaosSpace:
                    return Bot.GetRemainingCount(CardId.ChaosSpace, 3);
                case CardId.CalledbytheGrave:
                    return Bot.GetRemainingCount(CardId.CalledbytheGrave, 2);
                case CardId.BrandedRegained:
                    return Bot.GetRemainingCount(CardId.BrandedRegained, 1);
                case CardId.InfiniteImpermanence:
                    return Bot.GetRemainingCount(CardId.InfiniteImpermanence, 2);
                default:
                    return 0;
            }
        }
        #endregion

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            handActivated = false;
            isSummoned = false;
            No_SpSummon = false;

            activate_ThunderDragonFusion = false;
            activate_ThunderDragondark = false;
            activate_ThunderDragonroar = false;
            activate_ThunderDragonhawk = false;
            activate_ThunderDragonmatrix = false; 
            activate_TheBystialLubellion_hand = false;
            activate_BystialMagnamhut_hand = false;
            activate_BystialDruiswurm_hand = false;
            activate_ChaosSpace_grave = false;

            summon_WhiteDragonWyverburster = false;
            summon_BlackDragonCollapserpent = false;
            summon_TheBystialLubellion = false;
            summon_UnionCarrier = false;

            for (int i = 0; i < selectAtt.Count; i++)
                selectAtt[i] = false;
            
            base.OnNewTurn();
        }
        private bool IsAvailableZone(int seq)
        {
            ClientCard card = Bot.MonsterZone[seq];
            if (seq == 5 && Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0) return false;
            if (seq == 6 && Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Controller == 0) return false;
            if (card == null) return true;
            if (card.Controller != 0) return false;
            if (card.IsFacedown()) return false;
            if (card.IsDisabled()) return true;
            if (card.Id == CardId.ThunderDragonColossus
                || card.Id == CardId.ThunderDragonTitan
                || card.Id == CardId.UnderworldGoddessoftheClosedWorld
                || card.Id == CardId.MekkKnightCrusadiaAvramax
                || card.Id == CardId.AccesscodeTalker
                || (card.Id == CardId.BowoftheGoddess && card.Attack > 800)
                || (card.Id == CardId.UnionCarrier && summon_UnionCarrier)) return false;
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
            if ((zones & Zones.z0) > 0 && IsAvailableZone(0)) return true;
            if ((zones & Zones.z1) > 0 && IsAvailableZone(1)) return true;
            if ((zones & Zones.z2) > 0 && IsAvailableZone(2)) return true;
            if ((zones & Zones.z3) > 0 && IsAvailableZone(3)) return true;
            if ((zones & Zones.z4) > 0 && IsAvailableZone(4)) return true;
            if (IsAvailableZone(5)) return true;
            if (IsAvailableZone(6)) return true;
            return false;
        }
        private void ResetFlag()
        {
            for (int i = 0; i < selectFlag.Count; ++i)
            {
                selectFlag[i] = false;
            }
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location==CardLocation.MonsterZone)
            {
                if (place_CrossSheep)
                {
                    place_CrossSheep = false;
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                }
                if (place_ThunderDragonColossus)
                {
                    place_ThunderDragonColossus = false;
                    if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].IsCode(CardId.CrossSheep))
                    {
                        if ((Zones.z0 & available) > 0) return Zones.z0;
                        if ((Zones.z2 & available) > 0) return Zones.z2;
                    }
                    if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].IsCode(CardId.CrossSheep))
                    {
                        if ((Zones.z2 & available) > 0) return Zones.z2;
                        if ((Zones.z4 & available) > 0) return Zones.z4;
                    }
                    if ((Zones.z2 & available) > 0) return Zones.z2;
                    if ((Zones.z0 & available) > 0) return Zones.z0;
                    if ((Zones.z4 & available) > 0) return Zones.z4;
                }
                if (place_Link_4)
                {
                    place_Link_4 = false;
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                }
                if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].IsCode(CardId.CrossSheep))
                {
                    if ((Zones.z0 & available) > 0 && Bot.MonsterZone[2] != null && Bot.MonsterZone[2].HasType(CardType.Fusion) && Bot.MonsterZone[2].IsFaceup()) return Zones.z0;
                    if ((Zones.z2 & available) > 0 && Bot.MonsterZone[0] != null && Bot.MonsterZone[0].HasType(CardType.Fusion) && Bot.MonsterZone[0].IsFaceup()) return Zones.z2;
                }
                if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].IsCode(CardId.CrossSheep))
                {
                    if ((Zones.z2 & available) > 0 && Bot.MonsterZone[4] != null && Bot.MonsterZone[4].HasType(CardType.Fusion) && Bot.MonsterZone[4].IsFaceup()) return Zones.z2;
                    if ((Zones.z4 & available) > 0 && Bot.MonsterZone[2] != null && Bot.MonsterZone[2].HasType(CardType.Fusion) && Bot.MonsterZone[2].IsFaceup()) return Zones.z4;
                }
                return base.OnSelectPlace(cardId, player, location, available);
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (selectFlag.Count(flag => flag) > 1)
            {
                if (selectFlag.Count(flag => flag) == 2 && selectFlag[(int)Select.TheChaosCreator] && !selectFlag[(int)Select.DestroyReplace])
                {
                    selectFlag[(int)Select.TheChaosCreator] = false;
                }
                else if (selectFlag.Count(flag => flag) == 2 && selectFlag[(int)Select.DestroyReplace])
                {
                    selectFlag[(int)Select.DestroyReplace] = false;
                }
                else
                {
                    ResetFlag();
                    return null;
                }
            }
            if (selectFlag[(int)Select.NormalThunderDragon])
            {
                selectFlag[(int)Select.NormalThunderDragon] = false;
                //ThunderDragonTitan
                if (cards.Any(card => card != null && card.Controller != 0)) return null;
                if (cards.Count <= 1) return null;
                if (Bot.HasInHand(CardId.ChaosSpace) && !activate_ChaosSpace_hand)
                    return Util.CheckSelectCount(cards, cards, max, max);
                if(Bot.HasInHand(CardId.AloofLupine) && !isSummoned && GetRemainingThunderCount(true)>0)
                    return Util.CheckSelectCount(cards, cards, max, max);
                if (Bot.HasInHand(CardId.ThunderDragonFusion) && !activate_ThunderDragonFusion)
                    return Util.CheckSelectCount(cards, cards, min, min);
                if(HasInZoneNoActivate(CardId.BlackDragonCollapserpent,CardLocation.Hand)
                   || HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Hand)
                   || HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Hand)
                   || HasInZoneNoActivate(CardId.TheBystialLubellion, CardLocation.Hand))
                   return Util.CheckSelectCount(cards, cards, min, min);
                if(HasInZoneNoActivate(CardId.ThunderDragonhawk,CardLocation.Hand))
                   return Util.CheckSelectCount(cards, cards, min, min);
                return Util.CheckSelectCount(cards, cards, max, max);
            }
            if (selectFlag[(int)Select.ChaosSpace_1])
            {
                selectFlag[(int)Select.ChaosSpace_1] = false;
                selectFlag[(int)Select.ChaosSpace_2] = true;
                List<ClientCard> res = new List<ClientCard>();
                if (cards.Any(card => card != null && card.IsCode(CardId.ThunderDragonroar)) && !activate_ThunderDragonroar)
                    res.AddRange(cards.Where(card => card != null && card.IsCode(CardId.ThunderDragonroar)).ToList());
                if (cards.Any(card => card != null && card.IsCode(CardId.ThunderDragondark)) && !activate_ThunderDragondark)
                    res.AddRange(cards.Where(card => card != null && card.IsCode(CardId.ThunderDragondark)).ToList());
                if (cards.Any(card => card != null && card.IsCode(CardId.ThunderDragondark)) && !activate_ThunderDragondark)
                    res.AddRange(cards.Where(card => card != null && card.IsCode(CardId.ThunderDragondark)).ToList());
                if (cards.Any(card => card != null && card.IsCode(CardId.NormalThunderDragon)))
                    res.AddRange(cards.Where(card => card != null && card.IsCode(CardId.NormalThunderDragon)).ToList());
                if (cards.Any(card => card != null && card.IsCode(CardId.ThunderDragonmatrix)))
                    res.AddRange(cards.Where(card => card != null && card.IsCode(CardId.ThunderDragonmatrix)).ToList());
                if (res.Count <= 0) return null;
                return Util.CheckSelectCount(res, cards, min, max);
            }
            if (selectFlag[(int)Select.ChaosSpace_2])
            {
                selectFlag[(int)Select.ChaosSpace_2] = false;
                List<ClientCard> res = new List<ClientCard>();
                //can't get CardAttributes in deck
                if (cards.Any(card => card != null && (card.Id == CardId.BlackDragonCollapserpent 
                    || card.Id == CardId.TheChaosCreator)))
                {
                    if (!summon_BlackDragonCollapserpent && cards.Any(card => card != null && card.IsCode(CardId.BlackDragonCollapserpent) && !Bot.HasInHand(CardId.BlackDragonCollapserpent)))
                    {
                        IList<ClientCard> cards_1 = cards.Where(card => card != null && card.IsCode(CardId.BlackDragonCollapserpent)).ToList();
                        IList<ClientCard> cards_2 = cards.Where(card => card != null && !card.IsCode(CardId.BlackDragonCollapserpent)).ToList();
                        res.AddRange(cards_1);
                        res.AddRange(cards_2);
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                    else
                    {
                        res = cards.ToList();
                        res.Sort(CardContainer.CompareCardLevel);
                        res.Reverse();
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                }
                else if (cards.Any(card => card != null && (card.Id == CardId.WhiteDragonWyverburster
                         ||card.Id == CardId.ThunderDragonlord || card.Id == CardId.TheBystialLubellion)))
                {
                    if (!summon_WhiteDragonWyverburster && cards.Any(card => card != null && card.IsCode(CardId.WhiteDragonWyverburster)
                     && (Bot.HasInExtra(CardId.StrikerDragon) || Bot.HasInExtra(CardId.UnionCarrier)) && !Bot.HasInHand(CardId.WhiteDragonWyverburster)))
                    {
                        IList<ClientCard> cards_1 = cards.Where(card => card != null && card.IsCode(CardId.WhiteDragonWyverburster)).ToList();
                        IList<ClientCard> cards_2 = cards.Where(card => card != null && !card.IsCode(CardId.WhiteDragonWyverburster)).ToList();
                        res.AddRange(cards_1);
                        res.AddRange(cards_2);
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                    else if (!activate_TheBystialLubellion_hand && (HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Deck)
                       || HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Deck) && !Bot.HasInHand(CardId.TheBystialLubellion))
                       && cards.Any(card => card != null && card.IsCode(CardId.TheBystialLubellion)))
                    {
                        IList<ClientCard> cards_1 = cards.Where(card => card != null && card.IsCode(CardId.TheBystialLubellion)).ToList();
                        IList<ClientCard> cards_2 = cards.Where(card => card != null && !card.IsCode(CardId.TheBystialLubellion)).ToList();
                        res.AddRange(cards_1);
                        res.AddRange(cards_2);
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                    else if (!summon_WhiteDragonWyverburster && !Bot.HasInHand(CardId.WhiteDragonWyverburster) && cards.Any(card => card != null && card.IsCode(CardId.WhiteDragonWyverburster)))
                    {
                        IList<ClientCard> cards_1 = cards.Where(card => card != null && card.IsCode(CardId.WhiteDragonWyverburster)).ToList();
                        IList<ClientCard> cards_2 = cards.Where(card => card != null && !card.IsCode(CardId.WhiteDragonWyverburster)).ToList();
                        res.AddRange(cards_1);
                        res.AddRange(cards_2);
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                    else
                    {
                        res = cards.ToList();
                        res.Sort(CardContainer.CompareCardLevel);
                        return Util.CheckSelectCount(res, cards, min, max);
                    }
                }
                return null;
            }
            if (selectFlag[(int)Select.ThunderDragonColossus])
            {
                selectFlag[(int)Select.ThunderDragonColossus] = false;
                if (cards.Count < 2) return null;
                List<ClientCard> copy_cards = new List<ClientCard>(cards);
                copy_cards.Sort(CardContainer.CompareCardAttack);
                IList<ClientCard> res = new List<ClientCard>();
                for (int i = 0; i < copy_cards.Count; ++i)
                {
                    if ((copy_cards[i].Id == CardId.ThunderDragonroar && HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.MonsterZone) && Bot.GetMonstersInMainZone().Count < 5)
                        || (copy_cards[i].Id == CardId.ThunderDragondark && HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.MonsterZone))
                         || (copy_cards[i].Id == CardId.ThunderDragonmatrix && HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.MonsterZone)))
                    {
                        if (i <= 0) continue;
                        ClientCard temp = copy_cards[0];
                        copy_cards[0] = copy_cards[i];
                        copy_cards[i] = temp;
                    }
                }
                return Util.CheckSelectCount(copy_cards, cards, min, max);
            }
            if (selectFlag[(int)Select.AccesscodeTalker])
            {
                selectFlag[(int)Select.AccesscodeTalker] = false;
                List<ClientCard> copy_cards = new List<ClientCard>(cards);
                copy_cards.Sort(CardContainer.CompareCardAttack);
                List<ClientCard> res = new List<ClientCard>();
                res.AddRange(copy_cards.Where(card => card != null && card.Location == CardLocation.Grave));
                res.AddRange(copy_cards.Where(card => card != null && card.Location != CardLocation.Grave));
                if (res.Count <= 0) return null;
                CardAttribute att = (CardAttribute)res[0].Attribute;
                if (GetAttIndex(att) > 0) selectAtt[GetAttIndex(att)] = true;
                return Util.CheckSelectCount(res, cards, min, max);
            }
            if (selectFlag[(int)Select.DestroyReplace])
            {
                selectFlag[(int)Select.DestroyReplace] = false;
                if (min == 1 && max == 1)
                {
                    List<ClientCard> copy_cards = new List<ClientCard>(cards);
                    copy_cards.Sort(CardContainer.CompareCardAttack);
                    List<ClientCard> res = new List<ClientCard>();
                    List<ClientCard> pre_res = new List<ClientCard>();
                    foreach (var card in copy_cards)
                    {
                        if (card == null) continue;
                        if (card.Id == CardId.ThunderDragonroar && !activate_ThunderDragonroar && Bot.GetMonstersInMainZone().Count < 5)
                            res.Add(card);
                        else if (card.Id == CardId.ThunderDragondark && !activate_ThunderDragondark)
                            res.Add(card);
                        else
                            pre_res.Add(card);
                    }
                    res.Reverse();
                    res.AddRange(pre_res);
                    if(res.Count>=0) return Util.CheckSelectCount(res, cards, min, max);
                    return null;
                }
                if (min == 2 && max == 2)
                {
                    List<ClientCard> res = new List<ClientCard>();
                    if (!activate_ThunderDragonroar && Bot.GetMonstersInMainZone().Count < 5)
                    {
                        foreach (var card in cards)
                            if (card.Id == CardId.ThunderDragonroar && res.Count(_card => _card != null && _card.Id == CardId.ThunderDragonroar) <= 0)
                                res.Add(card);
                    }
                    if (!activate_ThunderDragondark)
                    {
                        foreach (var card in cards)
                            if (card.Id == CardId.ThunderDragondark && res.Count(_card => _card != null && _card.Id == CardId.ThunderDragondark) <= 0)
                                res.Add(card);
                    }
                    if (!activate_ThunderDragonhawk && !GetZoneRepeatCardsId(0,Bot.Hand).Contains(-1))
                    {
                        foreach (var card in cards)
                            if (card.Id == CardId.ThunderDragonhawk && res.Count(_card => _card != null && _card.Id == CardId.ThunderDragonhawk) <= 0)
                                res.Add(card);
                    }
                    if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck))
                    {
                        foreach (var card in cards)
                            if (card.Id == CardId.ThunderDragonmatrix && res.Count(_card => _card != null && _card.Id == CardId.ThunderDragonmatrix) <= 0)
                                res.Add(card);
                    }
                    List<ClientCard> scards = cards.Where(card => card != null && card.Id != CardId.ChaosSpace && card.Id != CardId.ThunderDragonFusion).ToList();
                    if (scards.Count > 0) res.AddRange(scards);
                    List<ClientCard> mcards = cards.Where(card => card != null && !card.HasRace(CardRace.Thunder)).ToList();
                    mcards.Sort(CardContainer.CompareCardAttack);
                    mcards.Reverse();
                    if (mcards.Count > 0) res.AddRange(mcards);
                    if(res.Count>0) return Util.CheckSelectCount(res, cards, min, max);
                    return null;

                }
            }
            if (hint == HintMsg.FusionMaterial)
            {
                List<ClientCard> res = new List<ClientCard>();
                List<ClientCard> banish = cards.Where(card => card != null && card.Location == CardLocation.Removed).ToList();
                if (banish.Count > 0) res.AddRange(banish);
                List<ClientCard> grave_1 = cards.Where(card => card != null && card.Location == CardLocation.Grave && card.Id != CardId.ThunderDragonroar && card.Id != CardId.ThunderDragondark).ToList();
                List<ClientCard> grave_2 = cards.Where(card => card != null && card.Location == CardLocation.Grave && (card.Id == CardId.ThunderDragonroar || card.Id == CardId.ThunderDragondark)).ToList();
                if (grave_1.Count > 0) res.AddRange(grave_1);
                if (grave_2.Count > 0) res.AddRange(grave_2);
                List<ClientCard> monsters = cards.Where(card => card != null && card.Location == CardLocation.MonsterZone).ToList();
                monsters.Sort(CardContainer.CompareCardAttack);
                if (monsters.Count > 0) res.AddRange(monsters);
                if (res.Count > 0) return Util.CheckSelectCount(res, cards, min, max);
                return null;
            }
            if (Duel.Phase == DuelPhase.End && hint == HintMsg.AddToHand)
            {
                List<ClientCard> res = new List<ClientCard>();
                List<ClientCard> cards_1 = cards.Where(card => card != null && card.Id == CardId.BystialDruiswurm).ToList();
                List<ClientCard> cards_2 = cards.Where(card => card != null && (card.Id == CardId.WhiteDragonWyverburster || card.Id == CardId.BlackDragonCollapserpent)).ToList();
                List<ClientCard> cards_3 = cards.Where(card => card != null && card.Id != CardId.BystialDruiswurm && card.Id != CardId.WhiteDragonWyverburster && card.Id != CardId.BlackDragonCollapserpent).ToList();
                if (cards_1.Count > 0) res.AddRange(cards_1);
                if (cards_2.Count > 0) res.AddRange(cards_2);
                if (cards_3.Count > 0) res.AddRange(cards_3);
                if (res.Count > 0) return Util.CheckSelectCount(res, cards, min, max);
                return null;
            }
            if (hint == HintMsg.OperateCard)
            {
                if (cards.Any(card => card != null && card.Location == CardLocation.Removed))
                {
                    selectFlag[(int)Select.TheChaosCreator] = true;
                    List<ClientCard> res = new List<ClientCard>();
                    List<ClientCard> cards_1 = cards.Where(card => card != null && card.Controller == 0 && (card.IsCode(CardId.ThunderDragonColossus) || card.IsCode(CardId.ThunderDragonTitan))).ToList();
                    if (cards_1.Count > 0) res.AddRange(cards_1);
                    List<ClientCard> cards_2 = cards.Where(card => card != null && card.Controller == 0 && !card.IsCode(CardId.ThunderDragonColossus) && !card.IsCode(CardId.ThunderDragonTitan)).ToList();
                    if (cards_2.Count > 0) res.AddRange(cards_2);
                    List<ClientCard> cards_3 = cards.Where(card => card != null && card.Controller == 1).ToList();
                    if (cards_3.Count > 0) res.AddRange(cards_3);
                    if (res.Count > 0) return Util.CheckSelectCount(res, cards, min, max);
                    return null;
                }
                return null;
            }
            if (selectFlag[(int)Select.TheChaosCreator])
            {
                selectFlag[(int)Select.TheChaosCreator] = false;
                List<ClientCard> res = new List<ClientCard>(cards);
                res.Sort(CardContainer.CompareCardAttack);
                if (res.Count <= 0) return null;
                if(res[0].Attack<res[res.Count-1].Attack) res.Reverse();
                return Util.CheckSelectCount(res, cards, min, max);
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        private bool SpellSet()
        {
            if (!Bot.HasInHand(CardId.G) && Bot.HasInHand(CardId.AllureofDarkness))
                return Bot.GetSpellCountWithoutField() < 4 && Card.Id != CardId.AllureofDarkness;
            return Card.HasType(CardType.QuickPlay) || Card.HasType(CardType.Trap) || Card.Id == CardId.ThunderDragonFusion;
        }
        private bool BrandedRegainedEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }
            AI.SelectCard(CardId.BystialDruiswurm, CardId.BystialMagnamhut, CardId.TheBystialLubellion);
            return true;
        }
        private int GetAttIndex(CardAttribute att)
        {
            switch (att)
            {
                case CardAttribute.Earth:return 0;
                case CardAttribute.Water: return 1;
                case CardAttribute.Fire:return 2;
                case CardAttribute.Wind: return 3;
                case CardAttribute.Light:return 4;
                case CardAttribute.Dark: return 5;
                case CardAttribute.Divine: return 6;
                default:return -1;
            }
        }
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
        private bool ThunderDragonColossusEffect()
        {
            selectFlag[(int)Select.DestroyReplace] = true;
            return true;
        }
        private IList<CardAttribute> GetAttUsed()
        {
            IList<CardAttribute> attributes = new List<CardAttribute>();
            for (int i = 0; i < selectAtt.Count; ++i)
                if (selectAtt[i]) attributes.Add((CardAttribute)(System.Math.Pow(2, i)));
            if (attributes.Count > 0) return attributes;
            return null;
        }
        private int GetRemainingThunderCount(bool isOnlyTunder = false)
        {
            int remaining = THUNDER_COUNTD;
            if (isOnlyTunder) remaining -= 4;
            remaining = remaining - Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder) && !card.IsExtraCard() && !(isOnlyTunder & !Card.HasSetcode(0x11c)));
            remaining = remaining - Bot.SpellZone.Count(card => card != null && card.HasRace(CardRace.Thunder) && !card.IsExtraCard() && !(isOnlyTunder & !Card.HasSetcode(0x11c)));
            remaining = remaining - Bot.MonsterZone.Count(card => card != null && card.HasRace(CardRace.Thunder) && !card.IsExtraCard() && !(isOnlyTunder & !Card.HasSetcode(0x11c)));
            remaining = remaining - Bot.Graveyard.Count(card => card != null && card.HasRace(CardRace.Thunder) && !card.IsExtraCard() && !(isOnlyTunder & !Card.HasSetcode(0x11c)));
            remaining = remaining - Bot.Banished.Count(card => card != null && card.HasRace(CardRace.Thunder) && !card.IsExtraCard() && !(isOnlyTunder & !Card.HasSetcode(0x11c)));
            return (remaining < 0) ? 0 : remaining;
        }
        private int GetLinkMark(int cardId)
        {
            if (cardId == CardId.Linkuriboh || cardId == CardId.StrikerDragon) return 1;
            if (cardId == CardId.PredaplantVerteAnaconda || cardId == CardId.CrossSheep  || cardId == CardId.IP || cardId == CardId.UnionCarrier) return 2;
            if (cardId == CardId.KnightmareUnicorn) return 3;
            if (cardId == CardId.BowoftheGoddess || cardId == CardId.AccesscodeTalker || cardId == CardId.MekkKnightCrusadiaAvramax) return 4;
            if (cardId == CardId.UnderworldGoddessoftheClosedWorld) return 5;
            return 1;
        }
        private bool AshBlossomEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.CurrentChain.Count > 0 && Duel.LastChainPlayer != 0;
        }
        public int CompareCardLink(ClientCard cardA, ClientCard cardB)
        {
            if (cardA.LinkCount < cardB.LinkCount)
                return -1;
            if (cardA.LinkCount == cardB.LinkCount)
                return 0;
            return 1;
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
        private bool IPEffect()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (!Bot.HasInExtra(CardId.KnightmareUnicorn) && !Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax)) return false;
            int[] materials = new[] {
                CardId.PredaplantVerteAnaconda,
                CardId.UnionCarrier,
                CardId.CrossSheep
            };
            if (Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax))
            {
                List<ClientCard> m = new List<ClientCard>();
                IList<ClientCard> pre_m = CardsIdToClientCards(materials, Bot.GetMonsters().Where(card=>card!=null && card.IsFaceup()).ToList());
                if (pre_m?.Count <= 0) return false;
                int link_count = 0;
                foreach (var card in pre_m)
                {
                    m.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                    if (link_count >= 4) break;
                }
                if (link_count < 4) return false;
                AI.SelectCard(CardId.MekkKnightCrusadiaAvramax);
                AI.SelectMaterials(m);
                return true;
            }
            else if (Bot.HasInExtra(CardId.KnightmareUnicorn))
            {
                if (Bot.Hand.Count <= 0) return false;
                List<ClientCard> pre_cards = Enemy.GetMonsters();
                pre_cards.AddRange(Enemy.GetSpells());
                if (pre_cards.Count(card => card != null && !card.IsShouldNotBeTarget()) <= 0) return false;
                List<ClientCard> materials_2 = new List<ClientCard>();
                List<ClientCard> resMaterials = new List<ClientCard>();
                foreach (var card in Bot.GetMonsters())
                {
                    if (card == null) continue;
                    if (card.Id == CardId.UnionCarrier && summon_UnionCarrier) continue;
                    if ((GetLinkMark(card.Id) < 3 || (card.Id == CardId.BowoftheGoddess && card.Attack <= 800)) && card.Id != CardId.ThunderDragonTitan
                        && card.Id != CardId.ThunderDragonColossus && card.IsFaceup() && materials_2.Count(_card => _card != null && _card.Id == card.Id) <= 0)
                        materials_2.Add(card);
                }
                int link_count = 0;
                materials_2.Sort(CardContainer.CompareCardAttack);
                materials_2.Sort(CompareCardLink);
                materials_2.Reverse();
                if (materials_2.Count <= 0) return false;
                foreach (var card in materials_2)
                {
                    if (!resMaterials.Contains(card))
                    {
                        resMaterials.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 3) break;
                    }
                }
                if (link_count >= 3) { AI.SelectCard(CardId.KnightmareUnicorn); AI.SelectMaterials(resMaterials); return true; }
            }
            return false;
        }
        private bool AccesscodeTalkerEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.AccesscodeTalker, 1))
            {
                if (Card.IsDisabled()) return false;
                if (Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Link)) <= 0) return false;
                IList<CardAttribute> attributes = GetAttUsed();
                if (attributes == null || attributes.Count <= 0) { ResetFlag(); selectFlag[(int)Select.AccesscodeTalker] = true; return true; }
                if (Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Link) && !attributes.Contains((CardAttribute)card.Attribute)) <= 0) return false;
                ResetFlag();
                selectFlag[(int)Select.AccesscodeTalker] = true;
                return true;
            }
            else
            {
                List<ClientCard> cards = Bot.GetGraveyardMonsters();
                cards.Sort(CompareCardLink);
                cards.Reverse();
                AI.SelectCard(cards);
                return true;
            }
        }
        private bool CalledbytheGraveEffect()
        {
            ClientCard card = Util.GetLastChainCard();
            if (card == null) return false;
            int id = card.Id;
            List<ClientCard> g_cards = Enemy.GetGraveyardMonsters().Where(g_card => g_card != null && g_card.Id == id).ToList();
            if (Duel.LastChainPlayer != 0 && card != null)
            {
                if (Card.Location == CardLocation.Hand)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                }
                if (card.Location == CardLocation.Grave && card.HasType(CardType.Monster))
                {
                    AI.SelectCard(card);
                }
                else if (g_cards.Count() > 0 && card.HasType(CardType.Monster))
                {
                    AI.SelectCard(g_cards);
                }
                else return false;
                return true;
            }
            return false;
        }
        private bool MekkKnightCrusadiaAvramaxSummon()
        {
            List<int> materials_1 = new List<int>{
                CardId.PredaplantVerteAnaconda,CardId.CrossSheep,
                CardId.IP
            };
            List<int> materials_2 = new List<int>{
                CardId.KnightmareUnicorn
            };
            List<int> materials_3 = new List<int>{
                CardId.StrikerDragon,CardId.Linkuriboh,CardId.AbyssDweller
            };
            if (Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && card.Id == CardId.BowoftheGoddess && card.Attack <= 800) > 0)
                materials_3.Add(CardId.BowoftheGoddess);
            if (!summon_UnionCarrier) materials_1.Add(CardId.UnionCarrier);
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials_1) && card.IsFaceup()) >= 2)
            {
                AI.SelectMaterials(materials_1);
                place_Link_4 = true;
                return true;
            }
            else if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials_2) && card.IsFaceup()) > 0
                && Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials_3) && card.IsFaceup()) > 0)
            {
                materials_2.AddRange(materials_3);
                AI.SelectMaterials(materials_2);
                place_Link_4 = true;
                return true;
            }
            return false;
        }
        private bool GEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.Player != 0;
        }
        private bool ThunderDragonColossusSummon_2()
        {
            if (handActivated && activate_ThunderDragonmatrix)
                return ThunderDragonColossusSummon();
            return false;
        }
        private bool ThunderDragonTitanEffect()
        {
            //if (Duel.CurrentChain.Count > 0)
            if(ActivateDescription == Util.GetStringId(CardId.ThunderDragonTitan, 0))
            {
                List<ClientCard> res = new List<ClientCard>();
                List<ClientCard> mcards = Enemy.GetMonsters();
                List<ClientCard> scards = Enemy.GetSpells();
                if (mcards.Count <= 0 && scards.Count <= 0) return false;
                if (Duel.CurrentChain.Count(card => card != null && card.Controller == 1) > 0)
                {
                    foreach (var card in Duel.CurrentChain)
                    {
                        if (card != null && card.Controller == 1 && (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone)
                            && !card.IsDisabled() && (card.HasType(CardType.Monster)
                            || card.HasType(CardType.Field) || card.HasType(CardType.Continuous)
                            || card.HasType(CardType.Equip)))
                        {
                            res.Add(card);
                        }
                    }
                }
                mcards.Sort(CardContainer.CompareCardAttack);
                mcards.Reverse();
                res.AddRange(mcards);
                res.AddRange(scards);
                AI.SelectCard(res);
                return true;
            }
            else
            {
                selectFlag[(int)Select.DestroyReplace] = true;
                return true;
            }
        
        }
        private bool PredaplantVerteAnacondaEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.PredaplantVerteAnaconda, 1))
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (CheckRemainInDeck(CardId.ThunderDragonFusion) <= 0) return false;
                if (Bot.GetMonstersInMainZone().Count > 4 && Bot.GetMonstersInMainZone().Count(card => card != null && !card.IsExtraCard() && card.HasSetcode(0x11c) && card.HasType(CardType.Monster) && card.IsFaceup()) <= 0) return false;
                List<ClientCard> g_card = Bot.Graveyard.ToList();
                List<ClientCard> b_card = Bot.Banished.ToList();
                g_card.AddRange(b_card);
                int count = 0;
                int Lcount = 0;
                foreach (var card in g_card)
                {
                    if (card == null) continue;
                    if (card.HasType(CardType.Monster) && card.HasSetcode(0x11c))
                        ++count;
                    if (card.IsCode(CardId.NormalThunderDragon))
                        ++Lcount;
                }
                if (Bot.HasInExtra(CardId.ThunderDragonColossus) && Lcount > 0 && g_card.Count(card => card != null && card.HasRace(CardRace.Thunder)) > 1)
                {
                    AI.SelectCard(CardId.ThunderDragonFusion);
                    AI.SelectNextCard(CardId.ThunderDragonColossus,CardId.ThunderDragonTitan);
                    No_SpSummon = true;
                    return true;
                }
                else if (count >= 3 && Bot.HasInExtra(CardId.ThunderDragonTitan))
                {
                    AI.SelectCard(CardId.ThunderDragonFusion);
                    AI.SelectNextCard(CardId.ThunderDragonTitan, CardId.ThunderDragonColossus);
                    No_SpSummon = true;
                    return true;
                }
                return false;
            }
            return false;
        }
        private bool CrossSheepEffect()
        {
            if (Bot.HasInExtra(CardId.UnionCarrier) 
                && (Bot.HasInGraveyard(CardId.AloofLupine)|| Bot.HasInGraveyard(CardId.G)))
                AI.SelectCard(CardId.AloofLupine, CardId.G);
            else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Grave))
                AI.SelectCard(CardId.ThunderDragonmatrix);
            else if (Bot.HasInExtra(CardId.Linkuriboh))
                AI.SelectCard(CardId.ThunderDragonmatrix, CardId.DragonBusterDestructionSword);
            else AI.SelectCard(CardId.BatterymanSolar, CardId.AshBlossom,CardId.G);
            return true;
        }
        private bool KnightmareUnicornEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            List<ClientCard> cards = new List<ClientCard>();
            cards.AddRange(Enemy.SpellZone);
            cards.AddRange(Enemy.MonsterZone);
            cards = cards.Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
            if (cards.Count <= 0) return false;
            List<int> disCardId = new List<int>();
            IList<int> repeatId = GetZoneRepeatCardsId(0, Bot.Hand);
            if (!repeatId.Contains(-1)) disCardId.AddRange(repeatId);
            foreach (var card in Bot.Hand)
                if (card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster))
                    disCardId.Add(card.Id);
            AI.SelectCard(disCardId);
            cards.Sort(CardContainer.CompareCardAttack);
            cards.Reverse();
            AI.SelectNextCard(cards);
            return true;
        }
        private bool ThunderDragonlordEffect()
        {
            if (Duel.Phase == DuelPhase.End)
            {
                int count = Bot.Graveyard.Count(card => card != null && card.HasRace(CardRace.Thunder));
                if ((Bot.HasInGraveyard(CardId.ThunderDragonroar) || Bot.HasInGraveyard(CardId.ThunderDragondark) && count > 1) && CheckRemainInDeck(CardId.ThunderDragonFusion) >0 )
                    AI.SelectCard(CardId.ThunderDragonFusion);
                else if(!Bot.HasInGraveyard(CardId.ThunderDragonroar) && CheckRemainInDeck(CardId.ThunderDragonroar) > 0)
                    AI.SelectCard(CardId.ThunderDragonroar);
                else if(!Bot.HasInGraveyard(CardId.ThunderDragondark) && CheckRemainInDeck(CardId.ThunderDragondark) > 0)
                    AI.SelectCard(CardId.ThunderDragondark);
                else AI.SelectCard(CardId.ThunderDragonmatrix,CardId.NormalThunderDragon,CardId.BatterymanSolar);
                return true;
            }
            if (Duel.Phase == DuelPhase.Standby)
            { 
                List<ClientCard> Thundercards = Bot.Graveyard.Where(card => card != null && card.HasRace(CardRace.Thunder)).ToList();
                List<ClientCard> NoThundercards = Bot.Graveyard.Where(card => card != null && !card.HasRace(CardRace.Thunder) && !card.IsCode(CardId.ThunderDragonFusion) && !card.IsCode(CardId.ChaosSpace)).ToList();
                if (HasInZoneNoActivate(CardId.ThunderDragonroar,CardLocation.Grave) && Bot.GetMonstersInMainZone().Count < 5)
                    AI.SelectCard(CardId.ThunderDragonroar);
                else if(HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Grave))
                    AI.SelectCard(CardId.ThunderDragondark);
                else if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Grave) && !GetZoneRepeatCardsId(0,Bot.Hand).Contains(-1))
                    AI.SelectCard(CardId.ThunderDragonhawk);
                else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Grave))
                    AI.SelectCard(CardId.ThunderDragonmatrix);
                else if(Thundercards.Count > 0)
                    AI.SelectCard(Thundercards);
                else AI.SelectCard(CardId.ThunderDragonmatrix);

                List<ClientCard> Spellcards = Bot.GetGraveyardSpells().Where(card => card != null && !card.IsCode(CardId.ThunderDragonFusion) && !card.IsCode(CardId.ChaosSpace)).ToList();
                if (Spellcards.Count > 0) AI.SelectNextCard(Spellcards);
                else if(NoThundercards.Count > 0 ) AI.SelectNextCard(Spellcards);
                else if(Thundercards.Count > 0) AI.SelectNextCard(Thundercards);
                else AI.SelectNextCard(CardId.ChaosSpace);
                AI.SelectThirdCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan, CardId.ThunderDragonlord,CardId.TheChaosCreator);
                return true;
            }
            return false;
        }
        private bool PredaplantVerteAnacondaSummon()
        {
            if (CheckRemainInDeck(CardId.ThunderDragonFusion) <= 0) return false;
            List<ClientCard> g_card = Bot.Graveyard.ToList();
            List<ClientCard> b_card = Bot.Banished.ToList();
            g_card.AddRange(b_card);
            int count = 0;
            int Lcount = 0;
            foreach (var card in g_card)
            {
                if (card == null) continue;
                if (card.HasType(CardType.Monster) && card.HasSetcode(0x11c))
                    ++count;
                if (card.IsCode(CardId.NormalThunderDragon))
                    ++Lcount;
            }
            if (!IsAvailableLinkZone()) return false;
            if ((count >= 3 && Bot.HasInExtra(CardId.ThunderDragonTitan)) ||
                (Bot.HasInExtra(CardId.ThunderDragonColossus) && Lcount > 0 && g_card.Count(card => card != null && card.HasRace(CardRace.Thunder)) > 1))
            {
                List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && GetLinkMark(card.Id) < 3 && !card.HasType(CardType.Normal) && !card.IsCode(CardId.ThunderDragonColossus) && !card.IsCode(CardId.ThunderDragonTitan) && !card.IsCode(CardId.ThunderDragonlord) && !(card.IsCode(CardId.UnionCarrier) && summon_UnionCarrier)).ToList();
                if (cards.Count < 2) return false;
                cards.Sort(CardContainer.CompareCardAttack);
                if (cards.Count(card => card != null && card.Id == CardId.IP) > 0 && cards.Count <= 2) return false;
                AI.SelectMaterials(cards);
                return true;
            }
            return false;
        }
        private bool AllureofDarknessEffect_2()
        {
            return !Bot.HasInHand(CardId.G) && Bot.Hand.Count <= 3 && Bot.Deck.Count > 2;
        }
        private bool KnightmareUnicornSummon()
        {
            if (Bot.Hand.Count <= 0) return false;
            if (!IsAvailableLinkZone()) return false;
            List<ClientCard> pre_cards = Enemy.GetMonsters();
            pre_cards.AddRange(Enemy.GetSpells());
            if (pre_cards.Count(card => card != null && !card.IsShouldNotBeTarget()) <= 0) return false;
            List<ClientCard> tmepMaterials = new List<ClientCard>();
            List<ClientCard> resMaterials = new List<ClientCard>();
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null) continue;
                if (card.Id == CardId.UnionCarrier && summon_UnionCarrier) continue;
                if ((GetLinkMark(card.Id) < 3 || (card.Id == CardId.BowoftheGoddess && card.Attack <= 800)) && card.Id != CardId.ThunderDragonTitan
                    && card.Id != CardId.ThunderDragonColossus && card.IsFaceup() && tmepMaterials.Count(_card=> _card != null && _card.Id==card.Id) <= 0)
                    tmepMaterials.Add(card);
            }
            int link_count = 0;
            tmepMaterials.Sort(CardContainer.CompareCardAttack);
            List<ClientCard> materials = new List<ClientCard>();
            List<ClientCard> link_materials = tmepMaterials.Where(card => card != null && card.LinkCount == 2).ToList();
            List<ClientCard> normal_materials = tmepMaterials.Where(card => card != null && card.LinkCount != 2).ToList();
            if (link_materials.Count() >= 1)
            {
                link_materials.InsertRange(1, normal_materials);
                materials.AddRange(link_materials);
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
            if (link_count >= 3) { AI.SelectMaterials(resMaterials); return true; }
            return false;
        }
        private bool UnderworldGoddessoftheClosedWorldSummon()
        {
            if (Duel.Turn == 0 || Enemy.GetMonsterCount() <= 0) return false;
            if (Util.GetBestAttack(Bot) >= Util.GetBestAttack(Enemy) && Enemy.MonsterZone.GetDangerousMonster() == null) return false;
            List<ClientCard> e_materials = new List<ClientCard>();
            List<ClientCard> m_materials = new List<ClientCard>();
            List<ClientCard> resMaterials = new List<ClientCard>();
            foreach (var card in Enemy.GetMonsters())
            {
                if (card != null && card.HasType(CardType.Effect) && card.IsFaceup())
                    e_materials.Add(card);
            }
            if (e_materials.Count() <= 0) return false;
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null) continue;
                if (card.Id == CardId.UnionCarrier && summon_UnionCarrier) continue;
                if (GetLinkMark(card.Id) < 3 && card.Id != CardId.ThunderDragonTitan
                    && card.Id != CardId.ThunderDragonColossus && card.IsFaceup() && card.HasType(CardType.Effect))
                    m_materials.Add(card);
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
                e_link_count += (card.HasType(CardType.Link)) ? (card.LinkCount == 2 ? 2:1): 1;
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
            if (link_count >= 5) { AI.SelectMaterials(resMaterials); place_Link_4 = true; return true; }
            return false;
        }
        private bool BowoftheGoddessSummon()
        {
            if (!IsAvailableLinkZone()) return false;
            if (Card.Id == CardId.AccesscodeTalker)
            {
                if (Duel.Turn == 0 || Enemy.GetMonsterCount() + Enemy.GetSpellCount() <= 0) return false;
            }
            else
            {
                if (Duel.Turn > 0 && Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0
                    && (Bot.HasInExtra(CardId.UnderworldGoddessoftheClosedWorld) || Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax)
                    || Bot.HasInExtra(CardId.AccesscodeTalker))) return false;
            }
            List<ClientCard> tempmaterials = new List<ClientCard>();
            List<ClientCard> resMaterials = new List<ClientCard>();
            foreach (var card in Bot.GetMonsters())
            {
                if (card == null) continue;
                if(card.Id == CardId.UnionCarrier && summon_UnionCarrier) continue;
                if (GetLinkMark(card.Id) < 4 && card.Id != CardId.ThunderDragonTitan
                    && card.Id != CardId.ThunderDragonColossus && card.IsFaceup() && !card.HasType(CardType.Token))
                    tempmaterials.Add(card);
            }
            int link_count = 0;
            List<ClientCard> materials = new List<ClientCard>();
            List<ClientCard> link_materials = tempmaterials.Where(card => card != null && (card.LinkCount == 3 || card.LinkCount == 2)).ToList();
            List<ClientCard> normal_materials = tempmaterials.Where(card => card != null && card.LinkCount != 3 && card.LinkCount != 2).ToList();
            normal_materials.Sort(CardContainer.CompareCardAttack);
            if (link_materials.Count <= 0 && Card.Id == CardId.AccesscodeTalker) return false;
            if (link_materials.Count(card => card != null && card.LinkCount == 3) > 0 && normal_materials.Count() > 0)
            {
                int index = -1;
                for (int i = 0; i < link_materials.Count(); i++)
                {
                    if (link_materials[i] != null && link_materials[i].LinkCount == 3)
                    {
                        if (i > 0)
                        {
                            ClientCard temp = link_materials[0];
                            link_materials[0] = link_materials[i];
                            link_materials[i] = temp;
                        }
                        index = i;
                        break;
                    }
                }
                resMaterials.Sort(CardContainer.CompareCardAttack);
                if (index >= 0) link_materials.InsertRange(index + 1, normal_materials);
                materials.AddRange(link_materials);
            }
            else
            {
                link_materials.Sort(CompareCardLink);
                materials.AddRange(link_materials);
                materials.AddRange(normal_materials);
            }
            foreach (var card in materials)
            {
                if (!resMaterials.Contains(card) && card.LinkCount < 4)
                {
                    if (Card.Id == CardId.BowoftheGoddess && resMaterials.Count(_card => _card != null && _card.Id == card.Id) > 0) break;
                    resMaterials.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                    if (link_count >= 4) break;
                }
            }
            resMaterials.Sort(CardContainer.CompareCardAttack);
            if (link_count >= 4) { AI.SelectMaterials(resMaterials); place_Link_4 = true; return true; }
            return false;
        }
        private bool TheChaosCreatorSummon()
        {
            IList<int> cardsid = new List<int>();
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Grave) && Bot.GetMonstersInMainZone().Count < 4)
                cardsid.Add(CardId.ThunderDragonroar);
            if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Grave))
                cardsid.Add(CardId.ThunderDragondark);
            if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Grave))
                cardsid.Add(CardId.ThunderDragonmatrix);
            if (Bot.HasInGraveyard(CardId.ChaosSpace) && !activate_ChaosSpace_grave)
            {
                cardsid.Add(CardId.BlackDragonCollapserpent);
                cardsid.Add(CardId.WhiteDragonWyverburster);
                cardsid.Add(CardId.TheBystialLubellion);
                cardsid.Add(CardId.ThunderDragonlord);
            }
            if (!Bot.HasInSpellZone(CardId.BrandedRegained, true, true) 
                || Bot.GetCountCardInZone(Bot.GetGraveyardMonsters(),CardId.BystialMagnamhut) + Bot.GetCountCardInZone(Bot.GetGraveyardMonsters(), CardId.BystialDruiswurm) > 1)
            {
                cardsid.Add(CardId.BystialMagnamhut);
                cardsid.Add(CardId.BystialDruiswurm);
            }
            List<ClientCard> cards = Bot.GetGraveyardMonsters().Where(card => card != null && (card.HasAttribute(CardAttribute.Dark) || card.HasAttribute(CardAttribute.Light))).ToList();
            cards.Sort(CardContainer.CompareCardAttack);
            foreach (var card in cards)
                if (card != null) cardsid.Add(card.Id);
            AI.SelectCard(cardsid);
            AI.SelectCard(cardsid);
            return true;
        }
        private bool ThunderDragonColossusSummon()
        {
            ResetFlag();
            selectFlag[(int)Select.ThunderDragonColossus] = true;
            place_ThunderDragonColossus = true;
            return true;
        }
        private bool ThunderDragonmatrixSet()
        {
            if (handActivated && Bot.HasInExtra(CardId.ThunderDragonColossus)
                && Bot.GetMonsters().Count(card=>card != null && card.HasRace(CardRace.Thunder) 
                    && card.IsFaceup() && card.HasType(CardType.Effect)) <= 0)
            {
                isSummoned = true;
                return true;
            }
            return false;
        }
        private bool IPSummon()
        {
            if (Duel.Turn > 0 && Duel.Phase < DuelPhase.Main2) return false;
            if (Bot.GetMonsterCount() <= 2) return false;
            if (Bot.HasInMonstersZone(CardId.ThunderDragonColossus) && Bot.GetMonsterCount() <= 3) return false;
            if (!Bot.HasInExtra(CardId.KnightmareUnicorn) && !Bot.HasInExtra(CardId.BowoftheGoddess)
                && !Bot.HasInExtra(CardId.MekkKnightCrusadiaAvramax) && !Bot.HasInExtra(CardId.AccesscodeTalker) && !Bot.HasInExtra(CardId.UnderworldGoddessoftheClosedWorld)) return false;
            List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && GetLinkMark(card.Id) < 3 && card.Id != CardId.ThunderDragonTitan && card.Id != CardId.ThunderDragonColossus && !card.HasType(CardType.Link) && card.Attack <= 2500 && card.EquipCards.Count(ecard=> ecard != null && ecard.Id==CardId.DragonBusterDestructionSword && !ecard.IsDisabled())<=0).ToList();
            if (cards.Count < 2) return false;
            if (!IsAvailableLinkZone()) return false;
            cards.Sort(CardContainer.CompareCardAttack);
            List<int> cardsId = new List<int>();
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.MonsterZone))
                cardsId.Add(CardId.ThunderDragonroar);
            if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.MonsterZone))
                cardsId.Add(CardId.ThunderDragondark);
            if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.MonsterZone))
                cardsId.Add(CardId.ThunderDragonmatrix);
            foreach (var card in cards)
              if (card != null) cardsId.Add(card.Id);
            AI.SelectMaterials(cardsId);
            return true;
        }
        private bool ThunderDragonlordSummon()
        {
            if (Bot.GetMonstersInMainZone().Count > 4 && Bot.GetMonstersInMainZone().Count(card => card != null && card.Level <= 8 && card.HasType(CardType.Tuner) && !card.IsExtraCard() && card.IsFaceup()) <= 0) return false;
            IList<int> cardsId = new List<int>();
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Hand)
                || HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.MonsterZone, true))
                cardsId.Add(CardId.ThunderDragonroar);
            if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Hand)
                || HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.MonsterZone, true))
                cardsId.Add(CardId.ThunderDragondark);
            if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Hand)
                || HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.MonsterZone, true))
                cardsId.Add(CardId.ThunderDragonmatrix);
            List<ClientCard> handCards = Bot.Hand.Where(card=>card != null).ToList();
            handCards.Sort(CardContainer.CompareCardLevel);
            List<ClientCard> monsterCards = Bot.GetMonsters().ToList();
            monsterCards.Sort(CardContainer.CompareCardAttack);
            foreach (var card in handCards)
            {
                if (card != null && card.HasRace(CardRace.Thunder) && card.Level <= 8)
                    cardsId.Add(card.Id);
            }
            foreach (var card in monsterCards)
            {
                if (card != null && card.HasRace(CardRace.Thunder) && card.Level <= 8
                    && card.Id != CardId.ThunderDragonColossus && card.IsFaceup())
                    cardsId.Add(card.Id);
            }
            if (cardsId.Count <= 0) return false;
            AI.SelectCard(cardsId);
            return true;
        }
        private bool UnionCarrierEffect()
        {
            if (!Bot.HasInMonstersZone(CardId.ThunderDragonColossus)) return false;
            return UnionCarrierEffect_2();
        }
        private bool UnionCarrierEffect_2()
        { 
                IList<int> cardsId = new List<int>();
                cardsId.Add(CardId.ThunderDragonColossus);
                cardsId.Add(CardId.TheChaosCreator);
                List<ClientCard> cards_1 = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && (card.HasAttribute(CardAttribute.Dark) || card.HasRace(CardRace.Dragon))).ToList();
                if (cards_1.Count <= 0)
                {
                    List<ClientCard> cards_2 = Bot.GetMonsters();
                    cards_2.Sort(CardContainer.CompareCardAttack);
                    cards_2.Reverse();
                    foreach (var card in cards_2)
                      if (card != null && !cardsId.Contains(card.Id))
                          cardsId.Add(card.Id);
                }
                else
                {
                    cards_1.Sort(CardContainer.CompareCardAttack);
                    cards_1.Reverse();
                    foreach (var card in cards_1)
                       if (card != null && !cardsId.Contains(card.Id))
                          cardsId.Add(card.Id);
                }
                AI.SelectCard(cardsId);
                AI.SelectNextCard(CardId.DragonBusterDestructionSword,CardId.ThunderDragonroar,CardId.ThunderDragondark,CardId.ThunderDragonmatrix,CardId.NormalThunderDragon);
            return true;
        }
        private bool StrikerDragonSummon()
        {
            if ((summon_WhiteDragonWyverburster && summon_BlackDragonCollapserpent) || CheckRemainInDeck(CardId.WhiteDragonWyverburster) <= 0 || CheckRemainInDeck(CardId.BlackDragonCollapserpent) <= 0) return false;
            return Bot.GetMonsters().Count(card => card != null && card.HasRace(CardRace.Dragon) && card.Level > 1) > 0;
        }
        private bool DefaultSummon()
        {
            if (No_SpSummon) return false;
            if (Card.Id == CardId.AshBlossom || Card.Id == CardId.G)
            {
                if (Bot.GetMonsterCount() >= 2 || handActivated) return false;
                if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.MonsterZone, true)
                    || HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.MonsterZone, true))
                {
                    if (Bot.HasInExtra(CardId.PredaplantVerteAnaconda) || Bot.HasInExtra(CardId.IP) || Bot.HasInExtra(CardId.CrossSheep)) return true;
                }
                return false;
            }
            if (Card.Level == 1)
            {
                if (Bot.ExtraDeck.Count(card => card != null && card.LinkCount <= 2) <= 0) return false;
            }
            else
            {
                if (Bot.ExtraDeck.Count(card => card != null && card.LinkCount == 2) <= 0) return false;
            }
            if (Card.Id == CardId.ThunderDragonroar || Card.Id == CardId.ThunderDragondark || Card.Id == CardId.NormalThunderDragon)
            {
                if (!Bot.HasInExtra(CardId.ThunderDragonColossus) || !handActivated) return false;
            }
            if (Card.Level > 4)
            {
                List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && GetLinkMark(card.Id) < 3 && card.Id != CardId.ThunderDragonTitan && card.Id != CardId.ThunderDragonColossus && card.Id != CardId.IP && card.Id != CardId.UnionCarrier).ToList();
                if (cards.Count <= 0) return false;
                cards.Sort(CardContainer.CompareCardAttack);
                if (handActivated && cards[0].Attack >= Card.Attack && !Bot.HasInExtra(CardId.ThunderDragonColossus)) return false;
                AI.SelectCard(cards);
            }
            isSummoned = true;
            return true;
        }
        private bool CheckThunderRemove()
        {
            if(HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Hand) && GetRemainingThunderCount() > 0) return true;
            if(HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Hand) && GetRemainingThunderCount() > 0) return true;
            if(Bot.Hand.Any(card=> card != null && card.HasRace(CardRace.Thunder) && !card.IsOriginalCode(CardId.AloofLupine)
               && (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck) || HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck)))) return true;
            return false;
        }
        private bool ThunderDragonhawkEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                List<ClientCard> banish_cards = new List<ClientCard>();
                List<ClientCard> grave_cards = new List<ClientCard>();
                foreach (var card in Bot.Banished)
                    if (card != null && card.HasType(CardType.Monster) && card.HasSetcode(0x11c))
                        banish_cards.Add(card);
                foreach (var card in Bot.Graveyard)
                    if (card != null && card.HasType(CardType.Monster) && card.HasSetcode(0x11c))
                        grave_cards.Add(card);
                banish_cards.Sort(CardContainer.CompareCardAttack);
                banish_cards.Reverse();
                grave_cards.Sort(CardContainer.CompareCardAttack);
                grave_cards.Reverse();
                banish_cards.AddRange(grave_cards);
                List<ClientCard> res = new List<ClientCard>();
                foreach (var card in banish_cards)
                {
                    if (!activate_ThunderDragonroar && card != null && card.Id == CardId.ThunderDragonroar)
                        res.Add(card);
                    else if (!activate_ThunderDragondark && card != null && card.Id == CardId.ThunderDragondark)
                        res.Add(card);
                }
                res.AddRange(banish_cards);
                AI.SelectCard(res);
                handActivated = true;
                activate_ThunderDragonhawk = true;
                return true;

            }
            else
            {
                activate_ThunderDragonhawk = true;
                List<int> cardsid = new List<int>() { CardId.DragonBusterDestructionSword };
                cardsid.AddRange(GetZoneRepeatCardsId(0, Bot.Hand));
                List<ClientCard> resCards = new List<ClientCard>();
                foreach (var card in Bot.Hand)
                {
                    if (card != null && cardsid.Contains(card.Id) && resCards.Count(_card => _card != null && _card.Id == card.Id) <= 0)
                    {
                        resCards.Add(card);
                    }
                }
                if (resCards.Count() <= 0) return false;
                AI.SelectCard(resCards);
                return true;
            }
        }
        private bool BlackDragonCollapserpentSummon_2()
        {
            if (Bot.HasInGraveyard(CardId.WhiteDragonWyverburster) && Bot.HasInGraveyard(CardId.ChaosSpace)
                   && !activate_ChaosSpace_grave)
                AI.SelectCard(CardId.WhiteDragonWyverburster);
            else if (Bot.HasInGraveyard(CardId.ThunderDragonlord) && Bot.HasInGraveyard(CardId.ChaosSpace)
                && !activate_ChaosSpace_grave)
                AI.SelectCard(CardId.ThunderDragonlord);
            else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Grave))
                AI.SelectCard(CardId.ThunderDragonmatrix);
            else if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Grave) && !(GetZoneRepeatCardsId(0, Bot.Hand, false)).Contains(-1))
                AI.SelectCard(CardId.ThunderDragonhawk);
            else
                AI.SelectCard(CardId.WhiteDragonWyverburster, CardId.BatterymanSolar);
            summon_BlackDragonCollapserpent = true;
            return true;
        }
        private bool BlackDragonCollapserpentSummon()
        {
            if (Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Light)) <= 1
                && Bot.HasInGraveyard(CardId.TheBystialLubellion) && CheckRemainInDeck(CardId.BrandedRegained) > 0 && !summon_TheBystialLubellion)
                return false;
            return BlackDragonCollapserpentSummon_2();
        }
        private bool GoldSarcophagusEffect()
        {
            if (GetRemainingThunderCount() <= 0) return false;
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck) && Bot.GetMonstersInMainZone().Count < 5)
                AI.SelectCard(CardId.ThunderDragonroar);
            else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck))
                AI.SelectCard(CardId.ThunderDragondark);
            else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck))
                AI.SelectCard(CardId.ThunderDragonmatrix);
            else AI.SelectCard(CardId.ThunderDragonmatrix);
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }
            return true;
        }
        private bool UnionCarrierSummon()
        {
            if (CheckRemainInDeck(CardId.DragonBusterDestructionSword) <= 0 || !Bot.HasInMonstersZone(CardId.ThunderDragonColossus,false,false,true)) return false;
            return UnionCarrierSummon_2(); 
        }
        private bool LinkCheck(bool exZone_1)
        {
            int exSq = 0;
            int linkSq_1 = 0;
            int linkSq_2 = 0;
            if (exZone_1)
            {
                exSq = 5;
                linkSq_1 = 0;
                linkSq_2 = 2;
            }
            else
            {
                exSq = 6;
                linkSq_1 = 2;
                linkSq_2 = 4;
            }
            if (Bot.MonsterZone[exSq] != null && Bot.HasInMonstersZone(CardId.ThunderDragonColossus, false, false, true))
            {
                CardRace linkRace = (CardRace)Bot.MonsterZone[exSq].Race;
                CardAttribute linkAtt = (CardAttribute)Bot.MonsterZone[exSq].Attribute;
                int linkRaceCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasRace(linkRace));
                int linkAttCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasAttribute(linkAtt));
                if (Bot.MonsterZone[exSq].Id == CardId.CrossSheep
                   || Bot.MonsterZone[exSq].Id == CardId.PredaplantVerteAnaconda
                    || Bot.MonsterZone[exSq].Id == CardId.IP
                    || Bot.MonsterZone[exSq].Id == CardId.MekkKnightCrusadiaAvramax)
                {
                    if (Bot.MonsterZone[linkSq_1] != null && Bot.MonsterZone[linkSq_1].Id == CardId.ThunderDragonColossus)
                    {
                        if (Bot.MonsterZone[linkSq_2] != null)
                        {
                            CardRace race = (CardRace)Bot.MonsterZone[linkSq_2].Race;
                            CardAttribute att = (CardAttribute)Bot.MonsterZone[linkSq_2].Attribute;
                            int raceCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasRace(race));
                            int attCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasAttribute(att));
                            if (raceCount < 2 && attCount < 2 && linkRaceCount < 2 && linkAttCount < 2) return false;
                        }
                    }
                    else if (Bot.MonsterZone[linkSq_2] != null && Bot.MonsterZone[linkSq_2].Id == CardId.ThunderDragonColossus)
                    {
                        if (Bot.MonsterZone[linkSq_1] != null)
                        {
                            CardRace race = (CardRace)Bot.MonsterZone[linkSq_1].Race;
                            CardAttribute att = (CardAttribute)Bot.MonsterZone[linkSq_1].Attribute;
                            int raceCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasRace(race));
                            int attCount = Bot.GetMonsters().Count(card => card != null && card.IsFaceup() && !card.IsCode(CardId.ThunderDragonColossus) && card.HasAttribute(att));
                            if (raceCount < 2 && attCount < 2 && linkRaceCount < 2 && linkAttCount < 2) return false;
                        }

                    }
                }
            }
            return true;
        }
        private bool UnionCarrierSummon_2()
        {
            if (Bot.GetMonsterCount() <= 2 && (Bot.HasInMonstersZone(CardId.ThunderDragonColossus) || Bot.HasInMonstersZone(CardId.ThunderDragonTitan))) return false;
            List<ClientCard> attDarkCards = Bot.GetMonsters().Where(card => card != null && card.HasAttribute(CardAttribute.Dark) && card.IsFaceup() && !card.IsOriginalCode(CardId.ThunderDragonColossus) && GetLinkMark(card.Id) < 3).ToList();
            List<ClientCard> attLightCards = Bot.GetMonsters().Where(card => card != null && card.HasAttribute(CardAttribute.Light) && card.IsFaceup() && GetLinkMark(card.Id) < 3).ToList();
            List<ClientCard> attEarthCards = Bot.GetMonsters().Where(card => card != null && card.HasAttribute(CardAttribute.Earth) && card.IsFaceup() && GetLinkMark(card.Id) < 3).ToList();
            List<ClientCard> raceThunderCards = Bot.GetMonsters().Where(card => card != null && card.HasRace(CardRace.Thunder) && card.IsFaceup() && !card.IsOriginalCode(CardId.ThunderDragonColossus) && GetLinkMark(card.Id) < 3).ToList();
            List<ClientCard> raceDragonCards = Bot.GetMonsters().Where(card => card != null && card.HasRace(CardRace.Dragon) && card.IsFaceup() && GetLinkMark(card.Id) < 3).ToList();
            List<ClientCard> raceBeastCards = Bot.GetMonsters().Where(card => card != null && card.HasRace(CardRace.Beast) && card.IsFaceup() && GetLinkMark(card.Id) < 3).ToList();
            if (attDarkCards.Count() < 2 && attLightCards.Count() < 2 && attEarthCards.Count() < 2
                && raceThunderCards.Count() < 2 && raceDragonCards.Count() < 2 && raceBeastCards.Count() < 2)
                return false;
            if (!LinkCheck(false) || !LinkCheck(true)) return false;
            if (!IsAvailableLinkZone()) return false;
            if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0 && GetLinkMark(Bot.MonsterZone[6].Id) > 1) return false;
            int[] materials = new[] {
                CardId.StrikerDragon,CardId.BatterymanToken,CardId.BatterymanSolar,
                CardId.ThunderDragonmatrix,CardId.NormalThunderDragon, CardId.WhiteDragonWyverburster,
                CardId.ThunderDragonhawk,CardId.G, CardId.AloofLupine, CardId.CrossSheep,
                CardId.ThunderDragonroar,CardId.ThunderDragondark,CardId.BlackDragonCollapserpent,
                CardId.DragonBusterDestructionSword,CardId.BystialMagnamhut,CardId.BystialDruiswurm,
                CardId.TheChaosCreator,CardId.Linkuriboh,CardId.TheBystialLubellion,
                CardId.ThunderDragonlord,CardId.PredaplantVerteAnaconda,CardId.IP
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                summon_UnionCarrier = true;
                return true;
            }
            return false;
        }
        private bool BatterymanSolarSummon()
        {
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck) || HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck)
                     || HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck) || HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Hand)
                     || HasInZoneNoActivate(CardId.WhiteDragonWyverburster, CardLocation.Hand) || HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Hand)
                     || HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Hand) || Bot.HasInHand(CardId.TheChaosCreator))
            {
                isSummoned = true;
                return true;
            }
            return false;
        }
        private bool ThunderDragonroarEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (handActivated) return false;
                handActivated = true;
                activate_ThunderDragonroar = true;
                if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Grave)
                   || HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Removed))
                {
                    AI.SelectCard(CardId.ThunderDragonhawk);
                }
                else AI.SelectCard(CardId.ThunderDragonmatrix,CardId.ThunderDragondark,CardId.ThunderDragonhawk);
                return true;
            }
            else
            {
                if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragondark);
                else if(HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragonmatrix);
                else if(HasInZoneNoActivate(CardId.NormalThunderDragon, CardLocation.Deck))
                    AI.SelectCard(CardId.NormalThunderDragon);
                else
                    AI.SelectCard(CardId.NormalThunderDragon, CardId.ThunderDragondark, CardId.NormalThunderDragon, CardId.ThunderDragonmatrix);
                activate_ThunderDragonroar = true;
                return true;
            }
        }
        private bool S_SpSummon()
        {
            if (Duel.Player == 0)
            {
                if (Duel.CurrentChain.Count > 0) return false;
                List<ClientCard> cards = new List<ClientCard>();
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Grave)
                    && Bot.GetMonstersInMainZone().Count < 4)
                {
                    foreach (var card_1 in Bot.Graveyard)
                    {
                        if (card_1 != null && card_1.Id == CardId.ThunderDragonroar)
                            cards.Add(card_1);
                    }
                }
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Grave))
                {
                    foreach (var card_2 in Bot.Graveyard)
                    {
                        if (card_2 != null && card_2.Id == CardId.ThunderDragondark)
                            cards.Add(card_2);
                    }
                }
                else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Grave))
                {
                    foreach (var card_3 in Bot.Graveyard)
                    {
                        if (card_3 != null && card_3.Id == CardId.ThunderDragonmatrix)
                            cards.Add(card_3);
                    }
                }
                else if (Bot.HasInGraveyard(CardId.ChaosSpace) && !activate_ChaosSpace_grave)
                {
                    foreach (var card_4 in Bot.Graveyard)
                    {
                        if (card_4 != null && NotSpSummonCardsId.Contains(card_4.Id))
                            cards.Add(card_4);
                    }
                }
                else
                {
                    foreach (var card_5 in Enemy.Graveyard)
                    {
                        if (card_5 != null && (card_5.HasAttribute(CardAttribute.Light)
                            || card_5.HasAttribute(CardAttribute.Dark)))
                            cards.Add(card_5);
                    }
                    foreach (var card_6 in Bot.Graveyard)
                    {
                        if (card_6 != null && (card_6.HasAttribute(CardAttribute.Light)
                            || card_6.HasAttribute(CardAttribute.Dark)) && !card_6.IsCode(CardId.TheBystialLubellion) && !card_6.HasRace(CardRace.Thunder))
                            cards.Add(card_6);
                    }
                    foreach (var card_7 in Bot.Graveyard)
                    {
                        if (card_7 != null && (card_7.HasAttribute(CardAttribute.Light)
                            || card_7.HasAttribute(CardAttribute.Dark)) && card_7.HasRace(CardRace.Thunder))
                            cards.Add(card_7);
                    }
                }
                AI.SelectCard(cards);
                return true;
            }
            else
            {
                if (Duel.Phase < DuelPhase.Battle && Duel.CurrentChain.Count <= 0) return false;
                ClientCard card = Util.GetLastChainCard();
                if (card != null && card.Controller != 0 &&
                   card.Location == CardLocation.Grave
                   && (card.HasAttribute(CardAttribute.Dark)
                   || card.HasAttribute(CardAttribute.Light)))
                {
                    AI.SelectCard(card);
                }
                else if (Duel.CurrentChain.Count > 0 && Duel.CurrentChain.Count(_card => _card != null && (_card.Id == CardId.BystialDruiswurm || _card.Id == CardId.BystialMagnamhut)) <= 0
                        && Duel.LastChainPlayer == 1 && Enemy.Graveyard.Count(_card => _card != null && (_card.HasAttribute(CardAttribute.Dark) || _card.HasAttribute(CardAttribute.Light))) > 0)
                {
                    List<ClientCard> graveCards = Enemy.GetGraveyardMonsters();
                    graveCards.Reverse();
                    AI.SelectCard(graveCards);
                }
                else
                {
                    if (Duel.CurrentChain.Count > 0 && Duel.CurrentChain.Count(_card => _card != null && _card.Controller == 0 && (_card.Id == CardId.BystialDruiswurm || _card.Id == CardId.BystialMagnamhut)) > 0) return false;
                    List<ClientCard> res = new List<ClientCard>();
                    List<ClientCard> pre_res = new List<ClientCard>();
                    foreach (var mcard in Enemy.Graveyard)
                    {
                        if (mcard != null && (mcard.HasAttribute(CardAttribute.Dark)
                           || mcard.HasAttribute(CardAttribute.Light)))
                            res.Add(mcard);
                    }
                    foreach (var mcard in Bot.Graveyard)
                    {
                        if (mcard != null && (mcard.HasAttribute(CardAttribute.Dark)
                           || mcard.HasAttribute(CardAttribute.Light)))
                        {
                            if (mcard.Id == CardId.ThunderDragonroar && !activate_ThunderDragonroar && Bot.GetMonstersInMainZone().Count < 5)
                                res.Add(mcard);
                            else if(mcard.Id == CardId.ThunderDragondark && !activate_ThunderDragondark)
                                res.Add(mcard);
                            else
                                pre_res.Add(mcard);
                        }
                    }
                    if (res.Count() <= 0) return false;
                    if (pre_res.Count > 0) res.AddRange(pre_res);
                    AI.SelectCard(res);
                }
                return true;
            }
        }
        private bool BystialDruiswurmEffect()
        {
            if (Card.Location == CardLocation.Hand) return S_SpSummon();
            else
            {
                List<ClientCard> cards = Enemy.GetMonsters();
                cards.Sort(CardContainer.CompareCardAttack);
                cards.Reverse();
                AI.SelectCard(cards);
                return true;
            }
        }
        private bool BystialMagnamhutEffect()
        {
            if (Card.Location == CardLocation.Hand) return S_SpSummon();
            return true;
        }
        private bool CrossSheepSummon()
        {
            if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Controller == 0 && GetLinkMark(Bot.MonsterZone[5].Id) > 1) return false;
            if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0 && GetLinkMark(Bot.MonsterZone[6].Id) > 1) return false;
            if (!handActivated && Bot.Hand.Count(card => card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster))
                + Bot.MonsterZone.Count(card => card != null && card.HasSetcode(0x11c) && card.IsFaceup() && card.HasType(CardType.Monster))
                + Bot.Graveyard.Count(card => card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster))
                + Bot.Banished.Count(card => card != null && card.HasSetcode(0x11c) && card.IsFaceup() && card.HasType(CardType.Monster)) < 2) return false;
            if ((Bot.HasInMonstersZone(CardId.ThunderDragonColossus, false, false, true) || Bot.HasInMonstersZone(CardId.ThunderDragonTitan, false, false, true)))
            {
                bool isShoudlSummon_1 = false;
                int light_count = Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Light));
                int dark_count = Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Dark));
                if (HasInZoneNoActivate(CardId.WhiteDragonWyverburster, CardLocation.Hand) && dark_count > 0) isShoudlSummon_1 = true;
                else if (HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Hand) && light_count > 0) isShoudlSummon_1 = true;
                else if ((HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Hand) || HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Hand))
                        && (dark_count > 0 || light_count > 0)) isShoudlSummon_1 = true;
                else if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Hand))
                {
                    List<ClientCard> mcards = Bot.GetMonsters().ToList();
                    List<ClientCard> grave = Bot.Graveyard.ToList();
                    List<ClientCard> banish = Bot.Banished.ToList();
                    mcards.AddRange(grave);
                    mcards.AddRange(banish);
                    int mcount =  mcards.Count(card => card != null && card.HasType(CardType.Monster) && card.HasSetcode(0x11c) && !card.IsCode(CardId.ThunderDragonColossus) && !card.IsCode(CardId.ThunderDragonTitan));
                    isShoudlSummon_1 =  mcount > 0 ? true : false;
                } 
                else if(Bot.HasInHand(CardId.TheChaosCreator) && light_count > 0 && dark_count > 0) isShoudlSummon_1 = true;
                else if (Bot.HasInHand(CardId.ThunderDragonlord) && Bot.Hand.Count(card=>card != null && card.HasType(CardType.Monster) && card.HasSetcode(0x11c))>1) isShoudlSummon_1 = true;
                if (!isShoudlSummon_1) return false;

            }
            if (!IsAvailableLinkZone()) return false;
            IList<int> cardsid = GetZoneRepeatCardsId(0, Bot.MonsterZone,true);
            if (!cardsid.Contains(-1) && Bot.MonsterZone.Count(card => card != null && card.IsFaceup() && !card.IsOriginalCode(CardId.ThunderDragonColossus) && GetLinkMark(card.Id) <= 1)
                - cardsid.Count() < 2) return false;
            if (cardsid.Contains(-1) && Bot.MonsterZone.Count(card => card != null && card.IsFaceup() && !card.IsOriginalCode(CardId.ThunderDragonColossus) && !card.IsOriginalCode(CardId.ThunderDragonTitan) && GetLinkMark(card.Id) <= 1 ) < 2) return false;
            bool isShoudlSummon_2 = false;
            foreach (var card in Bot.GetMonsters())
                if (card != null && card.IsFaceup() && SpSummonCardsId.Contains(card.Id)) { isShoudlSummon_2 = true; break; }
            if (Bot.Graveyard.Count(card => card != null && card.HasType(CardType.Monster) &&  card.Level <= 4 && !card.IsCode(CardId.BlackDragonCollapserpent) && !card.IsCode(CardId.WhiteDragonWyverburster)) > 0) isShoudlSummon_2 = true;
            if (!isShoudlSummon_2) return false;
            List<ClientCard> cards = Bot.GetMonsters();
            if (cards.Count < 2) return false;
            cards.Sort(CardContainer.CompareCardAttack);
            HashSet<int> MaterialsIdSet = new HashSet<int>();
            foreach (var card in cards)
            {
                if (card == null) continue;
                if (card.Id == CardId.UnionCarrier && summon_UnionCarrier) continue;
                if (GetLinkMark(card.Id) <= 1 && card.Id != CardId.ThunderDragonColossus && card.Id != CardId.ThunderDragonTitan 
                    && (card.EquipCards == null || (card.EquipCards != null && card.EquipCards.Count(ecard => ecard != null
                    && ecard.Id == CardId.DragonBusterDestructionSword) <= 0)))
                    MaterialsIdSet.Add(card.Id);
            }
            if (MaterialsIdSet.Count() < 2) return false;
            List<int> material = new List<int>();
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.MonsterZone))
                material.Add(CardId.ThunderDragonroar);
            if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.MonsterZone))
                material.Add(CardId.ThunderDragondark);
            if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.MonsterZone))
                material.Add(CardId.ThunderDragonmatrix);
            IList<int> materials  =  MaterialsIdSet.ToList();
            material.AddRange(materials);
            AI.SelectMaterials(material);
            place_CrossSheep = true;
            return true;
        }
        private bool ThunderDragonmatrixEffect()
        {
            if (Card.Location != CardLocation.Hand)
            {
                activate_ThunderDragonmatrix = true;
                return true;
            }
            return false;
        }
        private bool IsShouldChainTunder()
        {
            ClientCard card = Util.GetLastChainCard();
            return card != null && card.Controller != 0 && Bot.HasInMonstersZone(CardId.ThunderDragonTitan, true, false, true)
                        && !card.IsDisabled() && (card.HasType(CardType.Monster)
                        || card.HasType(CardType.Continuous) || card.HasType(CardType.Equip) || card.HasType(CardType.Field)) && (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone);
        }
        private bool ThunderDragonmatrixEffect_2()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (Duel.Player == 0)
                {
                    if (IsShouldChainTunder())
                    {
                        activate_ThunderDragondark = true;
                        handActivated = true;
                        return true;
                    }
                    if (Duel.CurrentChain.Count > 0) return false;
                    List<ClientCard> cards = Bot.Graveyard.ToList();
                    cards.AddRange(Bot.Banished.ToList());
                    if (handActivated || (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Hand) && Bot.GetMonstersInMainZone().Count < 5
                        && cards.Count(card => card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster)
                        && !card.IsCode(CardId.ThunderDragonhawk) && !card.IsExtraCard() && !card.IsCode(CardId.ThunderDragonlord)) > 0)) return false;
                    activate_ThunderDragonmatrix = true;
                    handActivated = true;
                    return true;
                }
                else
                {
                    if (IsShouldChainTunder())
                    {
                        AI.SelectCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan);
                        activate_ThunderDragonmatrix = true;
                        handActivated = true;
                        return true;
                    }
                    else if (Duel.Phase == DuelPhase.Battle)
                    {
                        if (Bot.HasInMonstersZone(CardId.ThunderDragonTitan, true, false, true) && Enemy.GetMonsterCount() > 0)
                        {
                            AI.SelectCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan);
                            activate_ThunderDragonmatrix = true;
                            handActivated = true;
                            return true;
                        }
                    }
                    else if (Duel.Phase == DuelPhase.BattleStep)
                    {
                        if (Bot.BattlingMonster != null && Bot.BattlingMonster.HasRace(CardRace.Thunder)
                           && !Bot.BattlingMonster.IsShouldNotBeTarget())
                        {
                            AI.SelectCard(Bot.BattlingMonster);
                            activate_ThunderDragonmatrix = true;
                            handActivated = true;
                            return true;
                        }
                    }
                    else if(Duel.Phase == DuelPhase.End)
                    {
                         AI.SelectCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan);
                        activate_ThunderDragonmatrix = true;
                        handActivated = true;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        private bool WhiteDragonWyverbursterSummon()
        {
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Grave)
                && Bot.GetMonstersInMainZone().Count < 5)
                AI.SelectCard(CardId.ThunderDragonroar);
            else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Grave))
                AI.SelectCard(CardId.ThunderDragondark);
            else 
            {
                List<int> cardsid = new List<int>();
                cardsid.Add(CardId.BlackDragonCollapserpent);
                cardsid.Add(CardId.TheChaosCreator);
                foreach (var card in Bot.Graveyard)
                {
                    if (card != null && !card.HasSetcode(0x11c) && card.HasAttribute(CardAttribute.Dark))
                        cardsid.Add(card.Id);
                }
                foreach (var card in Bot.Graveyard)
                {
                    if (card != null && card.HasSetcode(0x11c) && card.HasAttribute(CardAttribute.Dark))
                        cardsid.Add(card.Id);
                }

                AI.SelectCard(cardsid);
            }
            summon_WhiteDragonWyverburster = true;
            return true;
        }
        private bool HasInZoneNoActivate(int cardId , CardLocation location , bool isFaceUp = false)
        {
            switch (location)
            {
                case CardLocation.Deck: if (CheckRemainInDeck(cardId) <= 0) return false;  break;
                case CardLocation.Hand: if (!Bot.HasInHand(cardId)) return false;  break;
                case CardLocation.Grave: if (!Bot.HasInGraveyard(cardId)) return false;  break;
                case CardLocation.Removed: if (!Bot.HasInBanished(cardId)) return false; break;
                case CardLocation.MonsterZone: if (!Bot.HasInMonstersZone(cardId,false,false, isFaceUp)) return false; break;
                default: return false;
            }
            switch (cardId)
            {
                case CardId.ThunderDragonroar: return !activate_ThunderDragonroar;
                case CardId.ThunderDragondark: return !activate_ThunderDragondark;
                case CardId.ThunderDragonhawk: return !activate_ThunderDragonhawk;
                case CardId.ThunderDragonmatrix: return !activate_ThunderDragonmatrix;
                case CardId.BystialDruiswurm: return !activate_BystialDruiswurm_hand;
                case CardId.BystialMagnamhut: return !activate_BystialMagnamhut_hand;
                case CardId.TheBystialLubellion: return !activate_TheBystialLubellion_hand;
                case CardId.BlackDragonCollapserpent: return !summon_BlackDragonCollapserpent;
                case CardId.WhiteDragonWyverburster: return !summon_WhiteDragonWyverburster;
                default: return false;
            }
        }
        private bool AloofLupineEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder)) <= 0) return false;
                bool _ThunderDragonroar = false, _ThunderDragondark = false, _ThunderDragonmatrix = false;
                if (Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder) && !(card.IsCode(CardId.ThunderDragonhawk) && activate_ThunderDragonhawk)) <= 0
                    && GetRemainingThunderCount(true) <= 0) return false;
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Hand) && Bot.GetMonstersInMainZone().Count() < 5
                    && GetRemainingThunderCount() > 0)
                {
                    AI.SelectCard(CardId.ThunderDragonroar);
                    _ThunderDragonroar = true;
                }
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Hand) && GetRemainingThunderCount() > 0)
                {
                    AI.SelectCard(CardId.ThunderDragondark);
                    _ThunderDragondark = true;
                }
                else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Hand) && GetRemainingThunderCount() > 0)
                {
                    AI.SelectCard(CardId.ThunderDragonmatrix);
                    _ThunderDragonmatrix = true;
                }
                else if (Bot.GetCountCardInZone(Bot.Hand, CardId.NormalThunderDragon) > 1)
                    AI.SelectCard(CardId.NormalThunderDragon);
                else
                {
                    IList<ClientCard> cards = Bot.Hand.Where(card => card != null && card.HasRace(CardRace.Thunder)).ToList();
                    if (cards.Count() > 0) AI.SelectCard(cards);
                    else AI.SelectCard(CardId.ThunderDragonlord, CardId.BatterymanSolar, CardId.TheChaosCreator, CardId.NormalThunderDragon);
                }
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck) && !_ThunderDragonroar
                    && Bot.GetMonstersInMainZone().Count < 5 && !Bot.HasInMonstersZone(CardId.ThunderDragonroar,false,false,true))
                    AI.SelectNextCard(CardId.ThunderDragonroar);
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck) && !_ThunderDragondark
                    && !Bot.HasInMonstersZone(CardId.ThunderDragondark, false, false, true))
                    AI.SelectNextCard(CardId.ThunderDragondark);
                else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck) && !_ThunderDragonmatrix
                      && !Bot.HasInMonstersZone(CardId.ThunderDragonmatrix, false, false, true))
                    AI.SelectNextCard(CardId.ThunderDragonmatrix);
                else if(Bot.HasInGraveyard(CardId.TheChaosCreator) && !activate_ChaosSpace_grave && CheckRemainInDeck(CardId.ThunderDragonlord) > 0)
                    AI.SelectNextCard(CardId.ThunderDragonlord);
                else
                    AI.SelectNextCard(CardId.NormalThunderDragon);
                return true;
            }
            else
            {
                int[] ids = new int[]
                {
                    CardId.ThunderDragonhawk,CardId.ThunderDragonColossus,
                    CardId.BystialMagnamhut,CardId.BystialDruiswurm,
                    CardId.BlackDragonCollapserpent,CardId.WhiteDragonWyverburster,
                    CardId.TheChaosCreator,CardId.TheBystialLubellion
                };
                AI.SelectCard(ids);
                return true;
            }
        
        }
        private bool ThunderDragonmatrixSummon()
        {
            if (No_SpSummon) return false;
            if (Bot.HasInExtra(CardId.Linkuriboh)) 
            {
                if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Id == CardId.UnionCarrier && Bot.MonsterZone[5].Controller == 0)
                {
                    if (Bot.MonsterZone[1] != null) return false;
                }
                if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Id == CardId.UnionCarrier && Bot.MonsterZone[6].Controller == 0)
                {
                    if (Bot.MonsterZone[3] != null) return false;
                }
               isSummoned = true;
               return true; 
            }
            if (Bot.GetMonsters().Count(card => card != null && card.Id != CardId.ThunderDragonTitan && card.Id != CardId.ThunderDragonColossus && GetLinkMark(card.Id) < 3 && card.IsFaceup()) < 1) return false;
            switch (Card.Id)
            {
                case CardId.ThunderDragonmatrix:
                    if((Bot.HasInExtra(CardId.CrossSheep) || Bot.HasInExtra(CardId.IP))
                       && (Bot.GetMonsterCount()>0 && !Bot.HasInMonstersZone(CardId.ThunderDragonColossus)
                       && !Bot.HasInMonstersZone(CardId.ThunderDragonTitan)))
                    { isSummoned = true; return true; }
                    break;
                default:
                    break;
            }
            return false;
        }
        private bool TheBystialLubellionEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (HasInZoneNoActivate(CardId.BystialMagnamhut,CardLocation.Deck) && !Bot.HasInHand(CardId.BystialMagnamhut))
                    AI.SelectCard(CardId.BystialMagnamhut);
                else if(HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Deck) && !Bot.HasInHand(CardId.BystialDruiswurm))
                    AI.SelectCard(CardId.BystialDruiswurm);
                else 
                AI.SelectCard(CardId.BystialDruiswurm, CardId.BystialMagnamhut);
                activate_TheBystialLubellion_hand = true;
                return true;
            }
            return Card.Location == CardLocation.MonsterZone;
        }
        private bool ThunderDragonFusionEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Dark)) > 0
                    && Bot.Graveyard.Count(card => card != null && card.HasAttribute(CardAttribute.Light)) > 0
                    && CheckRemainInDeck(CardId.TheChaosCreator) > 0)
                    AI.SelectCard(CardId.TheChaosCreator);
                else if(Bot.HasInGraveyardOrInBanished(CardId.ThunderDragonroar) || Bot.HasInGraveyardOrInBanished(CardId.ThunderDragondark)
                         || Bot.HasInGraveyardOrInBanished(CardId.ThunderDragonlord) || Bot.HasInGraveyardOrInBanished(CardId.ThunderDragonmatrix)
                         || Bot.HasInGraveyardOrInBanished(CardId.NormalThunderDragon) && CheckRemainInDeck(CardId.ThunderDragonhawk) > 0)
                         AI.SelectCard(CardId.ThunderDragonhawk);
                else if((HasInZoneNoActivate(CardId.BystialDruiswurm,CardLocation.Hand) || HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Hand)
                        || HasInZoneNoActivate(CardId.WhiteDragonWyverburster, CardLocation.Hand) || HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Hand)
                        || HasInZoneNoActivate(CardId.TheChaosCreator, CardLocation.Hand)) && CheckRemainInDeck(CardId.BatterymanSolar) > 0)
                        AI.SelectCard(CardId.BatterymanSolar);
                else if(Bot.HasInMonstersZone(CardId.ThunderDragonTitan,true,false,true) && CheckRemainInDeck(CardId.NormalThunderDragon)>1)
                        AI.SelectCard(CardId.NormalThunderDragon);
                else if(!HasInZoneNoActivate(CardId.ThunderDragonroar,CardLocation.Deck))
                        AI.SelectCard(CardId.ThunderDragonroar);
                else if(handActivated && CheckRemainInDeck(CardId.ThunderDragonlord) > 0)
                        AI.SelectCard(CardId.ThunderDragonlord);
                else
                    AI.SelectCard(CardId.TheChaosCreator,CardId.ThunderDragondark,CardId.ThunderDragonlord);
                return true;
            }
            else
            {
                if (Bot.GetMonstersInMainZone().Count > 4 && Bot.GetMonstersInMainZone().Count(card => card != null && !card.IsExtraCard() && card.HasSetcode(0x11c) && card.HasType(CardType.Monster) && card.IsFaceup()) <= 0) return false;
                List<ClientCard> cards = Bot.Graveyard.ToList();
                IList<ClientCard> banish = Bot.Banished;
                cards.AddRange(banish);
                if (Bot.HasInExtra(CardId.ThunderDragonColossus) && Bot.GetCountCardInZone(cards, CardId.NormalThunderDragon) >= 1 &&
                   Bot.GetCountCardInZone(cards, CardId.NormalThunderDragon) + cards.Count(card => card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster) && !card.IsCode(CardId.NormalThunderDragon)) > 1)
                {
                    AI.SelectCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan);
                    return true;
                }
                else if (Bot.HasInExtra(CardId.ThunderDragonTitan) && cards.Count(card => card != null && card.HasSetcode(0x11c) && card.HasType(CardType.Monster)) >= 3)
                {
                    AI.SelectCard(CardId.ThunderDragonColossus, CardId.ThunderDragonTitan);
                    return true;
                }
                if (Card.Location == CardLocation.Hand)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                }
                return false;
            }
        }
        private bool TheBystialLubellionSummon()
        {
            if (Bot.HasInGraveyard(CardId.TheBystialLubellion) && Card.Location == CardLocation.Hand) return false;
            if (Card.Location == CardLocation.Hand && activate_TheBystialLubellion_hand && !Bot.HasInGraveyard(CardId.TheBystialLubellion)) { summon_TheBystialLubellion = true; return true; }
            if (Card.Location == CardLocation.Grave) { summon_TheBystialLubellion = true; return true; }
            return false;
        }
        private bool CheckHandThunder()
        {
            if (HasInZoneNoActivate(CardId.ThunderDragonroar,CardLocation.Hand)) return true;
            if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Hand)) return true;
            if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Hand)) return true;
            if (Bot.HasInHand(CardId.NormalThunderDragon)) return true;
            return false;
        }
        private bool ChaosSpaceEffect_2()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if((CheckRemainInDeck(CardId.ThunderDragonFusion)>0 || Bot.HasInHandOrInSpellZone(CardId.ThunderDragonFusion))
                   && !Bot.HasInExtra(CardId.ThunderDragonTitan))
                    AI.SelectCard(CardId.ThunderDragonTitan,CardId.ThunderDragonColossus, CardId.BlackDragonCollapserpent, CardId.WhiteDragonWyverburster, CardId.TheBystialLubellion);
                else 
                    AI.SelectCard(CardId.ThunderDragonColossus,CardId.WhiteDragonWyverburster, CardId.BlackDragonCollapserpent, CardId.TheBystialLubellion);
                activate_ChaosSpace_grave = true;
                return true;
            }
            return false;
        }
        private bool ChaosSpaceEffect()
        {
            if (Card.Location != CardLocation.Grave)
            {
                if (Bot.GetCountCardInZone(Bot.Hand, CardId.ThunderDragonmatrix) > 1 && !activate_ThunderDragonmatrix
                    && !CheckHandThunder() && Bot.Hand.Any(card => card != null &&
                    (card.HasAttribute(CardAttribute.Dark) || card.HasAttribute(CardAttribute.Light))
                    && !card.IsCode(CardId.ThunderDragonroar) && !card.IsCode(CardId.ThunderDragondark) && !card.IsCode(CardId.ThunderDragonhawk)
                    && !card.IsCode(CardId.NormalThunderDragon))) return false;
                ResetFlag();
                selectFlag[(int)Select.ChaosSpace_1] = true;
                activate_ChaosSpace_hand = true;
                if (Card.Location == CardLocation.Hand)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                }
                return true;
            }
            return false;
        }
        private bool AloofLupineSummon()
        {
            if (Bot.Hand.Count <= 1 || !Bot.Hand.Any(card => card!=null && card.HasRace(CardRace.Thunder) && card != Card)) return false;
            if (CheckThunderRemove())
            {
                isSummoned = true;
                return true;
            }
            return false;
        }
        private IList<int> GetZoneRepeatCardsId(int att, IList<ClientCard> zoneCards,bool isFaceUp = false)
        {
            if (zoneCards.Count <= 0) return new List<int>() { -1 };
            IList<ClientCard> cards = zoneCards;
            if (att > 0) cards = cards.Where(card => card != null && card.HasAttribute((CardAttribute)att)).ToList();
            if(cards.Count <= 0) return new List<int>() { -1 };
            IList<int> cardsid = new List<int>();
            IList<int> res = new List<int>();
            foreach (var card in cards)
                if(card != null && !(!card.IsFaceup() & isFaceUp)) 
                    cardsid.Add(card.Id);
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
        private bool AllureofDarknessEffect()
        {
            if (Bot.Deck.Count <= 2) return false;
            if (Bot.Hand.Count(card => card != null && card.HasAttribute(CardAttribute.Dark)) <= 0) return false;
            if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Hand) && Bot.GetMonstersInMainZone().Count < 5)
                AI.SelectCard(CardId.ThunderDragonroar);
            else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Hand))
                AI.SelectCard(CardId.ThunderDragondark);
            else
            {
                List<int> cardsid = new List<int>();
                IList<int> cardsid_1 = GetZoneRepeatCardsId((int)CardAttribute.Dark, Bot.Hand).ToList();
                IList<int> cardsid_2 = new List<int>();
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck) && Bot.GetMonstersInMainZone().Count < 5) cardsid.Add(CardId.ThunderDragonroar);
                if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck)) cardsid.Add(CardId.ThunderDragondark);
                if (HasInZoneNoActivate(CardId.TheBystialLubellion, CardLocation.Hand)) { cardsid_2.Add(CardId.BystialDruiswurm); cardsid_2.Add(CardId.BystialMagnamhut); }
                if (!Bot.HasInExtra(CardId.Linkuriboh) || isSummoned) cardsid_2.Add(CardId.DragonBusterDestructionSword);
                if (!HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Hand)) cardsid_2.Add(CardId.BlackDragonCollapserpent);
                if (isSummoned) cardsid_2.Add(CardId.AloofLupine);
                cardsid.AddRange(cardsid_1);
                cardsid.AddRange(cardsid_2);
                AI.SelectCard(cardsid);
            }
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }
            return true;
        }
        private bool ThunderDragondarkEffect_2()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                if (Duel.Player == 0)
                {
                    if (IsShouldChainTunder())
                    {
                        activate_ThunderDragondark = true;
                        handActivated = true;
                        return true;
                    }
                    if (Duel.CurrentChain.Count > 0 || Duel.Phase < DuelPhase.Main1) return false;
                    if (handActivated || (Bot.HasInHand(CardId.NormalThunderDragon) && CheckRemainInDeck(CardId.NormalThunderDragon) > 0) || !Bot.HasInExtra(CardId.ThunderDragonColossus)) return false;
                    if (!isSummoned && (Bot.HasInHand(CardId.BatterymanSolar) || Bot.HasInHand(CardId.AloofLupine))) return false;
                    activate_ThunderDragondark = true;
                    handActivated = true;
                    return true;
                }
                else
                {
                    if (IsShouldChainTunder() || (Duel.Phase == DuelPhase.End
                        && Bot.HasInMonstersZone(CardId.ThunderDragonTitan, true, false, true)
                        && Enemy.GetMonsterCount() + Enemy.GetSpellCount() > 0)
                        || (!Bot.HasInMonstersZone(CardId.ThunderDragonTitan, true, false, true)
                        && !Bot.HasInGraveyard(CardId.ThunderDragondark)))
                    {
                        activate_ThunderDragondark = true;
                        handActivated = true;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        private bool ThunderDragondarkEffect()
        {
            if (Card.Location != CardLocation.Hand)
            {
                if (Duel.Player == 0)
                {
                    if (handActivated && CheckRemainInDeck(CardId.ThunderDragonlord) > 0 &&
                        Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder)) > 0 &&
                        (Bot.HasInMonstersZone(CardId.ThunderDragonColossus) || (!isSummoned && Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder)) > 1)))
                        AI.SelectCard(CardId.ThunderDragonlord);
                    else if (HasInZoneNoActivate(CardId.ThunderDragonhawk, CardLocation.Deck) && !Bot.HasInHand(CardId.ThunderDragonhawk))
                        AI.SelectCard(CardId.ThunderDragonhawk);
                    else if (handActivated && Bot.Hand.Count(card => card != null && card.HasRace(CardRace.Thunder) && card.Level < 8) > 0
                        && Bot.HasInMonstersZone(CardId.ThunderDragonColossus) && CheckRemainInDeck(CardId.ThunderDragonlord) > 0)
                        AI.SelectCard(CardId.ThunderDragonlord);
                    else if (HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck))
                        AI.SelectCard(CardId.ThunderDragonmatrix);
                    else if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Hand) && handActivated
                            && CheckRemainInDeck(CardId.ThunderDragonlord) > 0)
                        AI.SelectCard(CardId.ThunderDragonlord);
                    else if (CheckRemainInDeck(CardId.ThunderDragonlord) > 0 && CheckRemainInDeck(CardId.NormalThunderDragon) > 1)
                        AI.SelectCard(CardId.ThunderDragonlord);
                    else if (handActivated && Bot.HasInHand(CardId.ThunderDragonlord) && CheckRemainInDeck(CardId.ThunderDragonroar) > 0)
                        AI.SelectCard(CardId.ThunderDragonroar);
                    else if (CheckRemainInDeck(CardId.NormalThunderDragon) > 1 && !handActivated)
                        AI.SelectCard(CardId.NormalThunderDragon);
                    else
                        AI.SelectCard(CardId.ThunderDragonmatrix, CardId.ThunderDragondark, CardId.ThunderDragonroar, CardId.NormalThunderDragon);
                    activate_ThunderDragondark = true;
                    return true;
                }
                else
                {
                    if (!Bot.HasInHand(CardId.ThunderDragonmatrix) && HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck)
                        &&(Bot.HasInMonstersZone(CardId.ThunderDragonTitan, true, false, true)
                        || (Bot.HasInMonstersZone(CardId.ThunderDragonColossus, true, false, true) && Bot.GetGraveyardMonsters().Count(card=>card != null && card.HasRace(CardRace.Thunder)) < 2 )))
                        AI.SelectCard(CardId.ThunderDragonmatrix);
                    else AI.SelectCard(CardId.ThunderDragonFusion, CardId.ThunderDragonhawk,CardId.ThunderDragonroar,CardId.NormalThunderDragon) ;
                    activate_ThunderDragondark = true;
                    return true;
                }
            }
            return false;
        }
        private bool NormalThunderDragonEffect()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            handActivated = true;
            ResetFlag();
            selectFlag[(int)Select.NormalThunderDragon] = true;
            return true;
        }
        private bool BatterymanSolarEffect()
        {
            if (HasInZoneNoActivate(CardId.BystialDruiswurm, CardLocation.Hand)
                 || HasInZoneNoActivate(CardId.BystialMagnamhut, CardLocation.Hand))
            {
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragonroar);
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck) 
                    && !Bot.HasInMonstersZone(CardId.ThunderDragondark,false,false,true))
                    AI.SelectCard(CardId.ThunderDragondark);
                else if ((HasInZoneNoActivate(CardId.ThunderDragonmatrix, CardLocation.Deck)))
                    AI.SelectCard(CardId.ThunderDragonmatrix);
                else if (CheckRemainInDeck(CardId.NormalThunderDragon) > 0)
                    AI.SelectCard(CardId.NormalThunderDragon);
                else if (CheckRemainInDeck(CardId.TheChaosCreator) > 0)
                    AI.SelectCard(CardId.TheChaosCreator);
                else AI.SelectCard(CardId.ThunderDragonmatrix);
            }
            else if (HasInZoneNoActivate(CardId.WhiteDragonWyverburster, CardLocation.Hand))
            {
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragonroar);
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragondark);
                else if (CheckRemainInDeck(CardId.TheChaosCreator) > 0)
                    AI.SelectCard(CardId.TheChaosCreator);
                else AI.SelectCard(CardId.ThunderDragonmatrix);

            }
            else if (Bot.HasInHand(CardId.ChaosSpace) && !activate_ChaosSpace_hand)
            {
                if ((Bot.Hand.Count(card => card != null && card.HasAttribute(CardAttribute.Dark)) > 0
                    || Bot.Hand.Count(card => card != null && card.HasAttribute(CardAttribute.Light)) > 0)
                    && HasInZoneNoActivate(CardId.WhiteDragonWyverburster, CardLocation.Deck)
                    && HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Deck))
                {
                    if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck))
                        AI.SelectCard(CardId.ThunderDragonroar);
                    else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck))
                        AI.SelectCard(CardId.ThunderDragondark);
                    else
                        AI.SelectCard(CardId.ThunderDragonmatrix, CardId.ThunderDragonhawk, CardId.NormalThunderDragon);
                }
            }
            else if (HasInZoneNoActivate(CardId.BlackDragonCollapserpent, CardLocation.Hand))
                AI.SelectCard(CardId.ThunderDragonmatrix, CardId.ThunderDragonhawk);
            else
            {
                if (HasInZoneNoActivate(CardId.ThunderDragonroar, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragonroar);
                else if (HasInZoneNoActivate(CardId.ThunderDragondark, CardLocation.Deck))
                    AI.SelectCard(CardId.ThunderDragondark);
                else AI.SelectCard(CardId.ThunderDragonmatrix, CardId.ThunderDragonhawk);
            } 
            return true;
        }
    }
}
