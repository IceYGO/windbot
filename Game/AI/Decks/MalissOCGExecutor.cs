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
    [Deck("MalissOCG", "AI_MalissOCG")]
    public class MalissOCGExecutor : DefaultExecutor
    {
        public class SetCode
        {
            public const int Maliss = 0x1bf;
        }
        public class CardId
        {
            public const int Artifact_Lancea = 34267821;//古遗物-圣枪
            public const int Dimension_Shifter = 91800273; //大宇宙人
            public const int MaxxG = 23434538; //增殖的G
            public const int Mulcharmy_Fuwalos = 42141493; //欢聚友伴·茸茸长尾山雀
            public const int Infinite_Impermanence = 10045474; //无限泡影
            public const int Dominus_Impulse = 40366667; //灵王的波动
            public const int AshBlossom = 14558127; //灰流丽
            public const int CalledbytheGrave = 24224830; //墓穴的指名
            public const int Gold_Sarcophagus = 75500286; //封印之黄金柜
            public const int Wizard_Ignister = 3723262; //男巫@火灵天星
            public const int Backup_Ignister = 30118811; //备份员@火灵天星
            public const int Maliss_Chessy_Cat = 96676583; //码丽丝<兵卒>柴郡猫
            public const int Maliss_White_Rabbit = 69272449; //码丽丝<兵卒>白兔
            public const int Maliss_Dormouse = 32061192; //码丽丝<兵卒>睡鼠
            public const int Maliss_March_Hare = 20938824; //码丽丝<兵卒>三月兔
            public const int Maliss_in_the_Mirror = 93453053; //码丽丝镜中奇像
            public const int Maliss_in_Underground = 68337209; //码丽丝梦游地下界
            public const int Maliss_GWC_06 = 20726052; //码丽丝<代码>GWC-06
            public const int Maliss_TB_11 = 57111661; //码丽丝<代码>TB-11
            public const int Maliss_MTP_07 = 94722358; //码丽丝<代码>MTP-07

            public const int Mereologic_Aggregator = 9940036;
            public const int Cyberse_Desavewurm = 92422871;
            public const int Allied_Code_Talker_Ignister = 39138610; //协心代码语者@火灵天星
            public const int Firewall_Dragon = 64211118; //防火龙·暗流体-新电磁泄密风
            public const int Accesscode_Talker = 86066372; //访问码语者
            public const int Maliss_Hearts_Crypter = 21848500; //码丽丝<王后>红心加密
            public const int Maliss_Red_Ransom = 68059897; //码丽丝<王后>红棋勒索
            public const int Maliss_White_Binder = 95454996; //码丽丝<王后>白棋捆绑
            public const int Transcode_Talker = 46947713; //转码语者
            public const int Splash_Mage = 59859086; //飞溅闪屏法师
            public const int Haggard_Lizardose = 9763474; //盛悴之致命毒蜥
            public const int Cyberse_Wicckid = 52698008; //电子界小男巫
            public const int Link_Decoder = 30342076; //连接解码员
        }

        public CardCount Count = new CardCount();
        public MalissOCGExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.Dimension_Shifter, Effect_Enemy_Turn);
            AddExecutor(ExecutorType.Activate, CardId.Mulcharmy_Fuwalos, Effect_Enemy_Turn);
            AddExecutor(ExecutorType.Activate, CardId.MaxxG, Effect_Enemy_Turn);
            AddExecutor(ExecutorType.Activate, CardId.Infinite_Impermanence, Effect_Infinite_Impermanence);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.Dominus_Impulse, Effect_Enemy_Chain);
            AddExecutor(ExecutorType.Activate, CardId.CalledbytheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_White_Rabbit, Effect_White_Rabbit);
            AddExecutor(ExecutorType.Activate, CardId.Haggard_Lizardose, Effect_Haggard_Lizardose);
            AddExecutor(ExecutorType.Activate, CardId.Splash_Mage);
            AddExecutor(ExecutorType.Activate, CardId.Cyberse_Wicckid);
            AddExecutor(ExecutorType.Activate, CardId.Cyberse_Desavewurm);
            AddExecutor(ExecutorType.Activate, CardId.Transcode_Talker);
            AddExecutor(ExecutorType.Activate, CardId.Mereologic_Aggregator, Effect_Mereologic_Aggregator);
            AddExecutor(ExecutorType.Activate, CardId.Firewall_Dragon, Effect_Firewall_Dragon);
            AddExecutor(ExecutorType.Activate, CardId.Allied_Code_Talker_Ignister, Effect_Allied_Code_Talker_Ignister);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_White_Binder, Effect_Maliss_Link);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_Red_Ransom, Effect_Maliss_Link);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_Hearts_Crypter, Effect_Maliss_Hearts_Crypter);
            AddExecutor(ExecutorType.Activate, CardId.Link_Decoder);
            
            AddExecutor(ExecutorType.Summon, CardId.Maliss_Dormouse, Summon_Maliss_Dormouse);
            AddExecutor(ExecutorType.Summon, CardId.Maliss_White_Rabbit, Summon_Maliss_White_Rabbit);
            AddExecutor(ExecutorType.Summon, CardId.Maliss_Chessy_Cat, Summon_Maliss_Chessy_Cat);
            AddExecutor(ExecutorType.Summon, CardId.Backup_Ignister, Summon_Backup_Ignister);

            AddExecutor(ExecutorType.Activate, CardId.Maliss_Dormouse, Effect_Maliss_Dormouse);

            AddExecutor(ExecutorType.SpSummon, CardId.Maliss_Red_Ransom, SP_Maliss_Link);
            AddExecutor(ExecutorType.SpellSet, SpellSet_Maliss);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_Chessy_Cat, Effect_Maliss_Chessy_Cat);

            AddExecutor(ExecutorType.SpSummon, CardId.Splash_Mage, SP_Splash_Mage);
            AddExecutor(ExecutorType.SpSummon, CardId.Haggard_Lizardose, SP_Haggard_Lizardose);
            AddExecutor(ExecutorType.SpSummon, CardId.Link_Decoder, SP_Link_Decoder);

            AddExecutor(ExecutorType.SpSummon, CardId.Cyberse_Wicckid, SP_Cyberse_Wicckid);

            AddExecutor(ExecutorType.Activate, CardId.Maliss_in_Underground, Effect_Remove);
            AddExecutor(ExecutorType.Activate, CardId.Gold_Sarcophagus, Effect_Remove);

            AddExecutor(ExecutorType.Activate, CardId.Maliss_TB_11, Effect_Maliss_TB_11);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_MTP_07, Effect_Maliss_MTP_07);

            AddExecutor(ExecutorType.Activate, CardId.Maliss_March_Hare, Effect_Maliss_March_Hare);
            AddExecutor(ExecutorType.Activate, CardId.Backup_Ignister);
            AddExecutor(ExecutorType.Activate, CardId.Wizard_Ignister, Effect_Wizard_Ignister);
            
            AddExecutor(ExecutorType.Activate, CardId.Maliss_in_the_Mirror, Effect_Maliss_in_the_Mirror);
            
            AddExecutor(ExecutorType.SpSummon, CardId.Maliss_Hearts_Crypter, SP_Maliss_Hearts_Crypter);
            AddExecutor(ExecutorType.SpSummon, CardId.Maliss_White_Binder, SP_Maliss_White_Binder);
            AddExecutor(ExecutorType.Activate, CardId.Maliss_GWC_06, Effect_Maliss_GWC_06);
            AddExecutor(ExecutorType.SpSummon, CardId.Firewall_Dragon, SP_Firewall_Dragon);
            AddExecutor(ExecutorType.SpSummon, CardId.Allied_Code_Talker_Ignister, SP_Allied_Code_Talker_Ignister);
            AddExecutor(ExecutorType.SpellSet, CardId.Maliss_in_the_Mirror);
            AddExecutor(ExecutorType.SpSummon, CardId.Transcode_Talker, SP_Transcode_Talker);
            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public class CardCount
        {
            public int Dimension_Shifter = 0;
            public int Summon = 0;
            public int Phase = 0;
            public List<int> Activate = new List<int>();
            public List<int> ActivateRemoved = new List<int>();
            public List<int> Position = new List<int>();
            public List<int> Set = new List<int>();
            public List<int> Oppo = new List<int>();
            public void Clear()
            {
                Activate.Clear();
                ActivateRemoved.Clear();
                Position.Clear();
                Set.Clear();
                Oppo.Clear();
                if (Dimension_Shifter > 0)
                    Dimension_Shifter --;
                if (Summon > 0)
                    Summon --;
            }
            public void AddActivateOppo(int id)
            {
                Oppo.Add(id);
            }
            public bool CheckActivateOppo(int id)
            {
                return !Oppo.Contains(id);
            }
            public void AddSummon()
            {
                Summon = 1;
            }
            public void AddCard(int id)
            {
                if (id == CardId.Dimension_Shifter)
                    Dimension_Shifter = 2;
                else
                    Activate.Add(id);
            }
            public void AddSet(int id)
            {
                Set.Add(id);
            }
            public bool CheckSet(int id)
            {
                return !Set.Contains(id);
            }
            public void AddCardRemoved(int id)
            {
                ActivateRemoved.Add(id);
            }
            public void AddPosition(int id)
            {
                Position.Add(id);
            }
            public void AddPhase()
            {
                Phase ++;
            }
            public bool CheckCard(int id)
            {
                if (id == CardId.Dimension_Shifter)
                    return Dimension_Shifter == 0;
                else
                    return !Activate.Contains(id);
            }
            public bool CheckCardRemoved(int id)
            {
                return !ActivateRemoved.Contains(id);
            }
            public bool CheckPosition(int id)
            {
                return !Position.Contains(id);
            }
            public int CheckPhase()
            {
                return Phase;
            }
            public bool CheckSummon()
            {
                return Summon == 0;
            }
        }
        public override void OnNewTurn()
        {
            Count.AddPhase();
            Count.Clear();
            base.OnNewTurn();
        }
        public override void OnChaining(int player, ClientCard card)
        {
            if (card.Id == CardId.Dimension_Shifter || card.Id == CardId.Artifact_Lancea)
                Count.AddCard(card.Id);
            else if (player == 0)
                if (card.Location == CardLocation.Removed)
                    Count.AddCardRemoved(card.Id);
                else
                    Count.AddCard(card.Id);
            if (player == 1)
                Count.AddActivateOppo(card.Id);
        }
        public override void OnChainEnd()
        {
            if (DefaultCheckWhetherCardIdIsNegated(CardId.Dimension_Shifter) && !Count.CheckCard(Card.Id))
                Count.Dimension_Shifter = 0;
            Count.Oppo.Clear();
        }
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.Maliss_White_Binder, 3))
                return true;
            if (desc == Util.GetStringId(CardId.Maliss_MTP_07, 3))
                return Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && (i.HasType(CardType.Field | CardType.Continuous | CardType.Equip) || i.IsFacedown())) > 0;
            return base.OnSelectYesNo(desc);
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if (new List<int>{CardId.Maliss_Red_Ransom, CardId.Splash_Mage}.Contains(cardId))
                    AI.SendCustomChat(0);

                if (new List<int>{CardId.Cyberse_Wicckid, CardId.Allied_Code_Talker_Ignister}.Contains(cardId))
                {
                    if ((Zones.z6 & available) > 0 && (Bot.MonsterZone[3] == null || Bot.MonsterZone[4] == null)) return Zones.z6;
                    if ((Zones.z5 & available) > 0 && (Bot.MonsterZone[0] == null || Bot.MonsterZone[1] == null)) return Zones.z5;
                }
                if (Bot.HasInMonstersZone(CardId.Cyberse_Wicckid) && Count.CheckCard(CardId.Cyberse_Wicckid))
                {
                    int seq = 0;
                    for (int i = 0; i < 7; ++i)
                        if (Bot.MonsterZone[i] != null && Bot.MonsterZone[i].IsCode(CardId.Cyberse_Wicckid)) seq = i;
                    if (seq == 5)
                    {
                        if ((Zones.z1 & available) > 0) return Zones.z1;
                        if ((Zones.z2 & available) > 0) return Zones.z2;
                    }
                    else if (seq == 6)
                    {
                        if ((Zones.z3 & available) > 0) return Zones.z3;
                        if ((Zones.z4 & available) > 0) return Zones.z4;
                    }
                }
                if (cardId == CardId.Transcode_Talker)
                {
                    if ((Zones.z6 & available) > 0 && Bot.MonsterZone[3] == null) return Zones.z6;
                    if ((Zones.z5 & available) > 0 && Bot.MonsterZone[1] == null) return Zones.z5;
                    if ((Zones.z0 & available) > 0 && Bot.MonsterZone[1] == null) return Zones.z0;
                    if ((Zones.z1 & available) > 0 && Bot.MonsterZone[2] == null) return Zones.z1;
                    if ((Zones.z2 & available) > 0 && Bot.MonsterZone[3] == null) return Zones.z2;
                    if ((Zones.z3 & available) > 0 && Bot.MonsterZone[4] == null) return Zones.z3;
                }
                if (cardId == CardId.Allied_Code_Talker_Ignister)
                {
                    var zones = new Dictionary<(int zone, ClientCard[] chk_zone), int>();
                    var updates = new Dictionary<(int zone, ClientCard[] chk_zone), int>();
                    zones[(Zones.z0, new ClientCard[] { Bot.MonsterZone[1] })] = 0;
                    zones[(Zones.z1, new ClientCard[] { Bot.MonsterZone[0], Bot.MonsterZone[2] })] = 0;
                    zones[(Zones.z2, new ClientCard[] { Bot.MonsterZone[1], Bot.MonsterZone[3] })] = 0;
                    zones[(Zones.z3, new ClientCard[] { Bot.MonsterZone[2], Bot.MonsterZone[4] })] = 0;
                    zones[(Zones.z4, new ClientCard[] { Bot.MonsterZone[3] })] = 0;
                    zones[(Zones.z5, new ClientCard[] { Bot.MonsterZone[0], Bot.MonsterZone[1], Bot.MonsterZone[2] })] = 0;
                    zones[(Zones.z6, new ClientCard[] { Bot.MonsterZone[2], Bot.MonsterZone[3], Bot.MonsterZone[4] })] = 0;
                    foreach (var entry in zones)
                    {
                        if ((entry.Key.zone & available) == 0)
                            continue;
                        
                        ClientCard[] checkZone = entry.Key.chk_zone;
                        int nullCount = checkZone.Count(card => card == null);
                        updates[entry.Key] = nullCount;
                    }
                    var maxEntry = updates.OrderByDescending(entry => entry.Value).FirstOrDefault();
                    if (maxEntry.Key != default)
                    {
                        var (zone, checkZone) = maxEntry.Key;
                        return zone;
                    }
                }
                if ((Zones.z6 & available) > 0) return Zones.z6;
                if ((Zones.z5 & available) > 0) return Zones.z5;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (AI.HaveSelectedCards()) return null;
            ClientCard card = Duel.GetCurrentSolvingChainCard();
            if (card == null)
                card = Card;
            switch (card.Id)
            {
                case CardId.Maliss_White_Rabbit:
                    if (cards.Any(i => i.Id == CardId.Maliss_TB_11) && Count.CheckCard(CardId.Maliss_TB_11))
                        return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_TB_11).ToList(), cards, min, max);
                    if (cards.Any(i => i.Id == CardId.Maliss_GWC_06) && Count.CheckCard(CardId.Maliss_GWC_06))
                        return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_GWC_06).ToList(), cards, min, max);
                    if (cards.Any(i => i.Id == CardId.Maliss_MTP_07) && Count.CheckCard(CardId.Maliss_MTP_07))
                        return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_MTP_07).ToList(), cards, min, max);
                    break;
                case CardId.Maliss_in_Underground:
                    if (Count.CheckSummon())
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse && i.Location == CardLocation.Deck)
                            && !Bot.HasInHand(CardId.Maliss_Dormouse)
                                && Check_Maliss_Dormouse()
                                    && Count.CheckCardRemoved(CardId.Maliss_Dormouse))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse && i.Location == CardLocation.Deck).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit && i.Location == CardLocation.Deck)
                                && !Bot.HasInHand(CardId.Maliss_White_Rabbit)
                                    && Check_Maliss_White_Rabbit()
                                        && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit && i.Location == CardLocation.Deck).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare && i.Location == CardLocation.Deck) && Check_Maliss_March_Hare(CardLocation.Removed) && Count.CheckCard(CardId.Maliss_March_Hare))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat && i.Location == CardLocation.Deck)
                                && !Bot.HasInHand(CardId.Maliss_Chessy_Cat)
                                    && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat && i.Location == CardLocation.Deck).ToList(), cards, min, max);
                    }
                    else
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse && i.Location == CardLocation.Deck)
                            && Count.CheckCardRemoved(CardId.Maliss_Dormouse) && Check_Maliss_Dormouse())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit && i.Location == CardLocation.Deck)
                            && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare && i.Location == CardLocation.Deck) && Check_Maliss_March_Hare(CardLocation.Removed) && Count.CheckCard(CardId.Maliss_March_Hare))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat && i.Location == CardLocation.Deck)
                            && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                        
                    }
                    return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Deck).ToList(), cards, min, max);
                case CardId.Gold_Sarcophagus:
                    if (Count.CheckSummon())
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse)
                            && !Bot.HasInHand(CardId.Maliss_Dormouse)
                                && Check_Maliss_Dormouse()
                                    && Count.CheckCardRemoved(CardId.Maliss_Dormouse))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit)
                                && !Bot.HasInHand(CardId.Maliss_White_Rabbit)
                                    && Check_Maliss_White_Rabbit()
                                        && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat)
                                && !Bot.HasInHand(CardId.Maliss_Chessy_Cat)
                                    && Check_Maliss_Chessy_Cat()
                                        && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare)
                            && Check_Maliss_March_Hare(CardLocation.Removed))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                    }
                    else
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse)
                            && Count.CheckCardRemoved(CardId.Maliss_Dormouse) && Check_Maliss_Dormouse())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit)
                            && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat)
                            && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat) && Check_Maliss_Chessy_Cat())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Removed))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_Dormouse:
                    if (Count.CheckSummon())
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit)
                                && !Bot.HasInHand(CardId.Maliss_White_Rabbit)
                                    && Check_Maliss_White_Rabbit()
                                        && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat)
                                && !Bot.HasInHand(CardId.Maliss_Chessy_Cat)
                                    && Check_Maliss_Chessy_Cat()
                                        && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Removed))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                    }
                    else
                    {
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit)
                            && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat)
                            && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat) && Check_Maliss_Chessy_Cat())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Removed))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_TB_11:
                    if (hint == HintMsg.SpSummon)
                    {
                        if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                        {
                            if (cards.Any(i => i.Id == CardId.Maliss_Dormouse) && Count.CheckCardRemoved(CardId.Maliss_Dormouse))
                                return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                            if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit) && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit))
                                return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                            if (cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Count.CheckCardRemoved(CardId.Maliss_March_Hare))
                                return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                            if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)))
                                return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)).ToList(), cards, min, max);
                        }
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse) && Count.CheckCard(CardId.Maliss_Dormouse))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit) && Count.CheckCard(CardId.Maliss_White_Rabbit))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat) && Count.CheckCard(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Remove)
                    {
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && i.HasType(CardType.Link)).ToList(), cards, min, max);
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_MTP_07:
                    if (hint == HintMsg.AddToHand)
                    {
                        if (Duel.Player == 1 && cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Count.CheckCard(CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Hand))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse) && Count.CheckCard(CardId.Maliss_Dormouse))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit) && Count.CheckCard(CardId.Maliss_White_Rabbit))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat) && Count.CheckCard(CardId.Maliss_Chessy_Cat))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Remove)
                    {
                        if (cards.Any(i => i.Controller == 1))
                            return Util.CheckSelectCount(cards.Where(i => i.Controller == 1).ToList(), cards, min, max);
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && i.HasType(CardType.Link)).ToList(), cards, min, max);
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Splash_Mage:
                case CardId.Haggard_Lizardose:
                case CardId.Cyberse_Wicckid:
                    if (hint == HintMsg.Remove)
                    {
                        if (cards.Any(i => Count.CheckCard(i.Id) && Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss) && i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCard(i.Id) && Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                        if (cards.Any(i => i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Grave).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.SpSummon)
                    {
                        if (cards.Any(i => i.HasSetcode(SetCode.Maliss)))
                            return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Target)
                    {
                        if (cards.Any(i => i.IsCode(CardId.Haggard_Lizardose)))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Haggard_Lizardose)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_Red_Ransom:
                    if (hint == HintMsg.AddToHand)
                    {
                        List<ClientCard> chk_cards = Bot.Graveyard.ToList();
                        chk_cards.AddRange(Bot.GetSpells());
                        chk_cards.AddRange(Bot.Hand);
                        if (cards.Any(i => i.IsCode(CardId.Maliss_in_the_Mirror))
                            && Check_Maliss_in_the_Mirror(CardLocation.Removed)
                                && chk_cards.Any(i => i.HasType(CardType.Trap))
                                    && (((Bot.HasInHand(CardId.Maliss_Chessy_Cat) && Count.CheckSummon()) || Bot.HasInMonstersZone(CardId.Maliss_Chessy_Cat)) && Count.CheckCard(CardId.Maliss_Chessy_Cat)
                                    || Bot.HasInHand(CardId.Maliss_March_Hare) && Count.CheckCard(CardId.Maliss_March_Hare)
                                    ) && Count.CheckCard(CardId.Maliss_in_the_Mirror)
                        )
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_in_the_Mirror)).ToList(), cards, min, max);
                        if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_in_the_Mirror)).ToList(), cards, min, max);
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_in_Underground)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Remove)
                    {
                        if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                        {
                            if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat) && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat))
                                return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                            if (cards.Any(i => !i.IsCode(CardId.Maliss_Chessy_Cat) && Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)))
                                return Util.CheckSelectCount(cards.Where(i => !i.IsCode(CardId.Maliss_Chessy_Cat) && Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)).ToList(), cards, min, max);
                        }
                        if (cards.Any(i => i.Id == CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Removed))
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_March_Hare).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Dormouse)
                            && Count.CheckCardRemoved(CardId.Maliss_Dormouse) && Check_Maliss_Dormouse())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Dormouse).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_White_Rabbit)
                            && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_White_Rabbit).ToList(), cards, min, max);
                        if (cards.Any(i => i.Id == CardId.Maliss_Chessy_Cat)
                            && Count.CheckCardRemoved(CardId.Maliss_Chessy_Cat) && Check_Maliss_Chessy_Cat())
                            return Util.CheckSelectCount(cards.Where(i => i.Id == CardId.Maliss_Chessy_Cat).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_Chessy_Cat:
                    if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                    {
                        if (cards.Any(i => i.IsCode(CardId.Maliss_March_Hare)) && !Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss)) && !Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id)))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)))
                            return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)))
                            return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id)))
                            return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                    }
                    if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.IsCode(CardId.Maliss_in_the_Mirror)) && Check_Maliss_in_the_Mirror(CardLocation.Grave))
                        return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && i.IsCode(CardId.Maliss_in_the_Mirror)).ToList(), cards, min, max);
                    if (cards.Any(i => Count.CheckCardRemoved(i.Id)))
                        return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                    break;
                case CardId.Maliss_White_Binder:
                    if (hint == HintMsg.Remove)
                    {
                        List<ClientCard> result = new List<ClientCard>();
                        int ct = 5 - Bot.GetMonstersInMainZone().Count;
                        if (ct > 0 && Count.CheckCard(CardId.Allied_Code_Talker_Ignister))
                        {
                            result.AddRange(cards.Where(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id) && i.Controller == 0 && i.HasType(CardType.Link)));
                            if (Duel.Player == 1)
                                result.AddRange(cards.Where(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id) && i.Controller == 0 && i.IsCode(CardId.Maliss_White_Rabbit)));
                            result.AddRange(cards.Where(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id) && i.Controller == 0 && i.HasType(CardType.Monster)));
                            result.AddRange(cards.Where(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id) && i.Controller == 0 && i.HasType(CardType.Spell)));
                        }
                        result.AddRange(cards.Where(i => i.Controller == 1));
                        result.AddRange(cards.Where(i => TrashCards(i.Id, CardLocation.Grave)));
                        result.AddRange(cards.Where(i => !i.HasSetcode(SetCode.Maliss) && !i.HasType(CardType.Trap)));
                        result.AddRange(cards.Where(i => !i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Trap)));
                        if (result.Count() > max)
                            result = result.Take(max).ToList();
                        if (result.Count() > 0)
                            return Util.CheckSelectCount(result, cards, result.Count(), result.Count());
                        if (cards.Any(i => TrashCards(i.Id, CardLocation.Grave)))
                            return Util.CheckSelectCount(cards.Where(i => TrashCards(i.Id, CardLocation.Grave)).ToList(), cards, min, min);

                        return Util.CheckSelectCount(cards, cards, min, min);
                    }
                    else if (hint == HintMsg.Set)
                    {
                        if (cards.Any(i => i.IsCode(CardId.Maliss_GWC_06)))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_GWC_06)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_March_Hare:
                    if (hint == HintMsg.AddToHand)
                    {
                        if (cards.Any(i => Count.CheckCard(i.Id) && i.IsCode(CardId.Maliss_March_Hare)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCard(i.Id) && i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        if (cards.Any(i => Count.CheckCard(i.Id) && !i.HasType(CardType.Link)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCard(i.Id) && !i.HasType(CardType.Link)).ToList(), cards, min, max);
                        if (cards.Any(i => !i.HasType(CardType.Link)))
                            return Util.CheckSelectCount(cards.Where(i => !i.HasType(CardType.Link)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Remove)
                    {
                        if (Duel.Player == 1)
                        {
                            if (Bot.GetMonstersInMainZone().Count() > 3)
                            {
                                if (cards.Any(i => i.HasType(CardType.Spell) && i.Location == CardLocation.Grave))
                                    return Util.CheckSelectCount(cards.Where(i => i.HasType(CardType.Spell) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                                if (cards.Any(i => !i.HasType(CardType.Link) && i.Location == CardLocation.Grave))
                                    return Util.CheckSelectCount(cards.Where(i => !i.HasType(CardType.Link) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                            }
                            else
                            {
                                if (cards.Any(i => Count.CheckCardRemoved(i.Id) && i.HasType(CardType.Link) && i.Location == CardLocation.Grave))
                                    return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && i.HasType(CardType.Link) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                            }
                        }
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && !i.HasType(CardType.Trap) && i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && !i.HasType(CardType.Trap) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                        if (cards.Any(i => !i.HasType(CardType.Trap) && i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => !i.HasType(CardType.Trap) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id) && !i.HasType(CardType.Trap)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id) && !i.HasType(CardType.Trap)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Maliss_in_the_Mirror:
                    if (hint == HintMsg.Remove)
                    {
                        if (!cards.Any(i => i.Location != CardLocation.Grave))
                        {
                            if (cards.Any(i => i.HasType(CardType.Trap)) && !Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.Maliss_GWC_06) && !Bot.HasInBanished(CardId.Maliss_GWC_06))
                                return Util.CheckSelectCount(cards.Where(i => i.HasType(CardType.Trap)).ToList(), cards, min, max);
                        }
                        else
                        {
                            if (cards.Any(i => i.IsCode(CardId.Maliss_Red_Ransom)) && Count.CheckCardRemoved(CardId.Maliss_Red_Ransom))
                                return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_Red_Ransom)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasType(CardType.Link) && i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasType(CardType.Link) && i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                        }
                    }
                    else if (hint == HintMsg.AddToHand)
                    {
                        if (cards.Any(i => i.IsCode(CardId.Maliss_GWC_06)))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_GWC_06)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Disable)
                    {
                        if (cards.Contains(Util.GetLastChainCard()))
                            return Util.CheckSelectCount(new List<ClientCard>() { Util.GetLastChainCard() }, cards, min, max);
                    }
                    break;
                case CardId.Maliss_GWC_06:
                    if (hint == HintMsg.Remove)
                    {
                        if (cards.Any(i => Count.CheckCardRemoved(i.Id)))
                            return Util.CheckSelectCount(cards.Where(i => Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.SpSummon)
                    {
                        if (cards.Any(i => i.IsCode(CardId.Maliss_White_Binder) && i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_White_Binder) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                        if (cards.Any(i => i.HasType(CardType.Link) && !Count.CheckCardRemoved(i.Id) && i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => i.HasType(CardType.Link) && !Count.CheckCardRemoved(i.Id) && i.Location == CardLocation.Grave).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Mereologic_Aggregator:
                    if (cards.Any(i => i.Controller == 1 && Count.CheckActivateOppo(i.Id)))
                        return Util.CheckSelectCount(cards.Where(i => i.Controller == 1 && Count.CheckActivateOppo(i.Id)).ToList(), cards, min, max);
                    return Util.CheckSelectCount(cards.Where(i => i.Controller == 1).ToList(), cards, min, max);
                case CardId.Firewall_Dragon:
                    if (Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget() && i.IsFaceup()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && i.HasType(CardType.Field | CardType.Continuous | CardType.Equip)) > 0
                        && Duel.Player == 1 && cards.Any(i => i.IsCode(CardId.Mereologic_Aggregator))
                    )
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Mereologic_Aggregator)).ToList(), cards, min, max);
                    if (cards.Any(i => i.IsCode(CardId.Cyberse_Desavewurm)))
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Cyberse_Desavewurm)).ToList(), cards, min, max);
                    return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Deck).ToList(), cards, min, max);
                case CardId.Allied_Code_Talker_Ignister:
                    if (hint == HintMsg.SpSummon)
                    {
                        if (cards.Any(i => !i.IsCode(CardId.Maliss_White_Binder)))
                            return Util.CheckSelectCount(cards.Where(i => !i.IsCode(CardId.Maliss_White_Binder)).ToList(), cards, max, max);
                        return base.OnSelectCard(cards, max, max, hint, false);
                    }
                    else if (hint == HintMsg.Release)
                    {
                        if (cards.Any(i => i.LinkCount < 4))
                            return Util.CheckSelectCount(cards.Where(i => i.LinkCount < 4).ToList(), cards, max, max);
                    }
                    break;
                case CardId.Backup_Ignister:
                    if (hint == HintMsg.AddToHand)
                    {
                        if (card.Id == CardId.Dimension_Shifter || card.Id == CardId.Artifact_Lancea)
                        {
                            if (!Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare))
                                && cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare))
                            )
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                            if (!Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare))
                                && cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare))
                            )
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        }
                        if (cards.Any(i => i.IsCode(CardId.Wizard_Ignister)) && Bot.Hand.Count() > 0
                            && (Bot.Graveyard.Any(i => i.HasRace(CardRace.Cyberse))
                                || (Bot.HasInExtra(CardId.Link_Decoder) && Bot.GetMonsters().Any(i => i.Level <= 4 && i.HasRace(CardRace.Cyberse)) && Count.CheckCard(CardId.Dimension_Shifter))
                                || (Bot.HasInExtra(CardId.Haggard_Lizardose)
                                    && !Count.CheckCard(CardId.Dimension_Shifter)
                                        && Count.CheckCard(CardId.Artifact_Lancea)
                                            && Bot.GetMonsters()
                                                .Where(i => i.IsFaceup() && (!i.HasType(CardType.Link) || i.LinkCount < 2)).ToList()
                                                .GroupBy(i => i.Id)
                                                .Select(i => i.First())
                                                .Count() >= 2)
                            )
                        )
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Wizard_Ignister)).ToList(), cards, min, max);
                        if (Bot.HasInHand(CardId.Maliss_March_Hare))
                        {
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                        }
                        else
                            if (cards.Any(i => i.IsCode(CardId.Maliss_March_Hare)))
                                return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        if (cards.Any(i => i.HasSetcode(SetCode.Maliss)))
                            return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss)).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.Discard)
                    {
                        if (card.Id == CardId.Dimension_Shifter || card.Id == CardId.Artifact_Lancea)
                        {
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                        }
                        if (Bot.HasInHand(CardId.Maliss_March_Hare) && !Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss)))
                        {
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare) && Count.CheckCardRemoved(i.Id)).ToList(), cards, min, max);
                            if (cards.Any(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare)))
                                return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss) && !i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                            if (cards.Count(i => i.IsCode(CardId.Maliss_March_Hare)) > 1)
                                return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Maliss_March_Hare)).ToList(), cards, min, max);
                        }
                        if (cards.Any(i => TrashCards(i.Id, CardLocation.Hand)))
                            return Util.CheckSelectCount(cards.Where(i => TrashCards(i.Id, CardLocation.Hand)).ToList(), cards, min, max);
                        if (cards.Any(i => !i.HasType(CardType.Monster)))
                            return Util.CheckSelectCount(cards.Where(i => !i.HasType(CardType.Monster)).ToList(), cards, min, max);
                        if (cards.Any(i => !i.HasRace(CardRace.Cyberse)))
                            return Util.CheckSelectCount(cards.Where(i => !i.HasRace(CardRace.Cyberse)).ToList(), cards, min, max);
                    }
                    break;
                case CardId.Wizard_Ignister:
                    if (cards.Any(i => i.HasSetcode(SetCode.Maliss)))
                        return Util.CheckSelectCount(cards.Where(i => i.HasSetcode(SetCode.Maliss)).ToList(), cards, min, max);
                    break;
                case CardId.Maliss_Hearts_Crypter:
                    if (hint == HintMsg.Remove && cards.Any(i => i.Controller == 1))
                        return Util.CheckSelectCount(cards.Where(i => i.Controller == 1).ToList(), cards, min, max);
                    break;
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        private bool TrashCards(int code, CardLocation loc)
        {
            if (loc == CardLocation.Grave)
            {
                List<int> list = new List<int>{
                    CardId.MaxxG,
                    CardId.Artifact_Lancea,
                    CardId.Dimension_Shifter,
                    CardId.Mulcharmy_Fuwalos,
                    CardId.Infinite_Impermanence,
                    CardId.Dominus_Impulse,
                    CardId.AshBlossom,
                    CardId.CalledbytheGrave,
                    CardId.Gold_Sarcophagus,

                };
                return list.Contains(code);
            }
            else if (loc == CardLocation.Hand)
            {
                if (Bot.GetFieldCount() > 0 && code == CardId.Mulcharmy_Fuwalos)
                    return true;
                if (Bot.Graveyard.Count > 0 && code == CardId.Dimension_Shifter)
                    return true;
            }
            return false;
        }
        private bool MonsterRepos()
        {
            if (!Enemy.GetMonsters().Any(i => i.IsDefense())
                && Util.GetTotalAttackingMonsterAttack(0) + Card.Attack >= Enemy.LifePoints + Util.GetTotalAttackingMonsterAttack(1)
                && Card.IsDefense()
            )
                return true;
            return Card.IsFacedown();
        }
        private bool SpellSet()
        {
            return Card.HasType(CardType.Trap | CardType.QuickPlay);
        }
        private bool SpellSet_Maliss()
        {
            return Card.HasType(CardType.Trap) && Card.HasSetcode(SetCode.Maliss) && Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss));
        }
        private bool Effect_Enemy_Turn()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Id == CardId.Dimension_Shifter)
            {
                if (Duel.Player == 1 && Count.CheckCard(Card.Id))
                {
                    Count.AddCard(Card.Id);
                    return true;
                }
            }
            return Duel.Player == 1;
        }
        private bool Effect_Enemy_Chain()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            ClientCard LastChainCard = Util.GetLastChainCard();
            return LastChainCard != null && LastChainCard.Controller == 1;
        }
        private bool Effect_Infinite_Impermanence()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (Count.CheckPosition(this_seq)) return false;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || Util.IsChainTarget(Card)
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    Count.AddPosition(this_seq);
                    return true;
                }
            }
            else
            {
                if (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone)
                {
                    AI.SelectCard(LastChainCard);
                    return true;
                }
            }
            return false;
        }
        private bool Effect_Maliss_Removed(int lp = 300)
        {
            int ct = 5 - Bot.GetMonstersInMainZone().Count;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.HasType(CardType.Monster) && !Card.IsCode(CardId.Maliss_March_Hare)
                && ct - Duel.CurrentChain.Count(i => i.HasSetcode(SetCode.Maliss)
                    && i.Location == CardLocation.Removed
                    && i.HasType(CardType.Monster)
                ) <= 0
            ) return false;
            if (Bot.LifePoints > lp && Card.Location == CardLocation.Removed)
            {
                Count.AddCardRemoved(Card.Id);
                return true;
            }
            return false;
        }
        private bool Effect_Maliss_Chessy_Cat()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.Maliss_Chessy_Cat, 0))
            {
                if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                {
                    if (Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss)
                    && Count.CheckCardRemoved(i.Id)
                        && !i.HasType(CardType.Trap)
                            && (!i.IsCode(CardId.Maliss_in_the_Mirror) || Check_Maliss_in_the_Mirror(CardLocation.Removed))
                    ))
                    {
                        Count.AddCard(Card.Id);
                        return true;
                    }
                    if (Bot.HasInHand(CardId.Maliss_March_Hare) && !Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss)))
                    {
                        Count.AddCard(Card.Id);
                        return true;
                    }
                    return false;
                }

                if (Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss)
                    && Count.CheckCardRemoved(i.Id)
                        && !i.HasType(CardType.Trap)
                ))
                {
                    Count.AddCard(Card.Id);
                    return true;
                }
                return false;
            }
            else
                return Effect_Maliss_Removed();
        }
        private bool Effect_Maliss_March_Hare()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.Allied_Code_Talker_Ignister)) return false;
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Duel.Player == 1
                    && (!Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link))
                    || Bot.GetMonstersInMainZone().Count() > 3)
                )
                    return false;
                if (Bot.HasInMonstersZone(CardId.Maliss_Chessy_Cat) && Count.CheckCard(CardId.Maliss_Chessy_Cat))
                    return false;
                if (Check_Maliss_March_Hare(CardLocation.Hand))
                {
                    Count.AddCard(Card.Id);
                    return true;
                }
                return false;
            }
            else
                return Effect_Maliss_Removed();
        }
        private bool Effect_Maliss_Dormouse()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                Count.AddCard(Card.Id);
                return true;
            }
            else 
                return Effect_Maliss_Removed();
        }
        private bool Summon_Maliss_Chessy_Cat()
        {
            if (Check_Maliss_Chessy_Cat())
            {
                Count.AddSummon();
                return true;
            }
            return false;
        }
        private bool Effect_White_Rabbit()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                Count.AddCard(Card.Id);
                return true;
            }
            else 
                return Effect_Maliss_Removed();
        }
        private bool Summon_Maliss_Dormouse()
        {
            if (Check_Maliss_Dormouse())
            {
                Count.AddSummon();
                return true;
            }
            return false;
        }
        private bool Summon_Maliss_White_Rabbit()
        {
            if (Check_Maliss_White_Rabbit())
            {
                Count.AddSummon();
                return true;
            }
            return false;
        }
        private bool Check_Maliss_in_the_Mirror(CardLocation loc)
        {
            if (loc == CardLocation.Removed)
            {
                if (!Count.CheckCard(CardId.Artifact_Lancea) || !Count.CheckCardRemoved(CardId.Maliss_March_Hare))
                    return false;
                List<ClientCard> cards = Bot.Hand.ToList();
                cards.AddRange(Bot.GetMonsters());
                cards.AddRange(Bot.GetSpells());
                cards.AddRange(Bot.Graveyard);
                cards.AddRange(Bot.Banished);
                return Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss)
                    && (
                        (i.HasType(CardType.Monster) && cards.Count(j => j.HasType(CardType.Monster)) < 10)
                        || (i.HasType(CardType.Spell) && cards.Count(j => j.HasType(CardType.Spell)) < 4)
                        || (i.HasType(CardType.Trap) && cards.Count(j => j.HasType(CardType.Trap)) < 3)
                    )
                );
            }
            else
            {
                List<ClientCard> cards = Bot.Hand.GetMonsters();
                cards.AddRange(Bot.GetMonsters());
                return cards.Any(i => Count.CheckCardRemoved(i.Id));
            }
        }
        private bool Check_Maliss_Chessy_Cat()
        {
            return Bot.Hand.Any(i => i.HasSetcode(SetCode.Maliss)
                    && !i.HasType(CardType.Trap)
                        && i != Card && Count.CheckCardRemoved(i.Id)
                ) && Count.CheckCard(CardId.Maliss_Chessy_Cat);
        }
        private bool Check_Maliss_White_Rabbit()
        {
            return Bot.Graveyard.Count(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Trap)) < 3
                && Count.CheckCard(CardId.Maliss_White_Rabbit);
        }
        private bool Check_Maliss_Dormouse()
        {
            List<ClientCard> cards = Bot.Hand.ToList();
            cards.AddRange(Bot.GetMonsters());
            cards.AddRange(Bot.GetSpells());
            cards.AddRange(Bot.Graveyard);
            cards.AddRange(Bot.Banished);
            return cards.Count(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster)) < 9
                && Count.CheckCard(CardId.Maliss_Dormouse);
        }
        private bool Check_Maliss_March_Hare(CardLocation loc)
        {
            if (loc == CardLocation.Removed)
            {
                if (!Count.CheckCard(CardId.Artifact_Lancea))
                    return false;
                return (Bot.Banished.Any(i => i.HasSetcode(SetCode.Maliss)
                    && i.HasType(CardType.Monster)
                ) || Count.CheckCard(CardId.Maliss_March_Hare)) && Count.CheckCardRemoved(CardId.Maliss_March_Hare);
            }
            else
            {
                if (!Count.CheckCard(CardId.Artifact_Lancea))
                    return false;
                return Bot.Graveyard.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss));
            }
        }
        private bool Effect_Maliss_TB_11()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Bot.GetMonsterCount() > 1
                && (!Bot.HasInMonstersZone(CardId.Maliss_Red_Ransom) || !Bot.HasInMonstersZone(CardId.Maliss_White_Binder))
            )
                return false;
            if (Bot.GetMonsters().Any(i => Count.CheckCardRemoved(i.Id)))
            {
                Count.AddCard(Card.Id);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Maliss_Red_Ransom) && Count.CheckCardRemoved(CardId.Maliss_Red_Ransom))
            {
                Count.AddCard(Card.Id);
                return true;
            }
            return false;
        }
        private bool Effect_Maliss_MTP_07()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (DefaultCheckWhetherCardIsNegated(Card) || Duel.Player == 0 || Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && (i.HasType(CardType.Field | CardType.Continuous | CardType.Equip) || i.IsFacedown())) == 0 || !Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link))) return false;
            if (Bot.GetMonsterCount() > 1
                && (!Bot.HasInMonstersZone(CardId.Maliss_Red_Ransom) || !Bot.HasInMonstersZone(CardId.Maliss_White_Binder))
            )
                return false;
            if (Bot.GetMonsters().Any(i => Count.CheckCardRemoved(i.Id))
                && ((Count.CheckCard(CardId.Maliss_March_Hare) && !Bot.HasInHand(CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Hand))
                    || (Count.CheckSummon() && (
                            (Count.CheckCard(CardId.Maliss_Dormouse) && !Bot.HasInMonstersZone(CardId.Maliss_Dormouse) && Check_Maliss_Dormouse())
                            || (Count.CheckCard(CardId.Maliss_White_Rabbit) && !Bot.HasInMonstersZone(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                            || (Count.CheckCard(CardId.Maliss_Chessy_Cat) && !Bot.HasInMonstersZone(CardId.Maliss_Chessy_Cat) && Check_Maliss_Chessy_Cat())
                        )
                    )
                )
            )
            {
                Count.AddCard(Card.Id);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Maliss_Red_Ransom) && Count.CheckCardRemoved(CardId.Maliss_Red_Ransom))
            {
                Count.AddCard(Card.Id);
                return true;
            }
            return false;
        }
        private bool Effect_Maliss_GWC_06()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Duel.Player == 0 && Bot.HasInGraveyard(CardId.Maliss_White_Binder))
                return false;
            if (Bot.GetMonsters().Any(i => Count.CheckCardRemoved(i.Id)) && Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link)))
            {
                Count.AddCard(Card.Id);
                return true;
            }
            return false;
        }
        private bool Effect_Remove()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return (
                (!Bot.HasInMonstersZone(CardId.Maliss_Dormouse) && Count.CheckCard(CardId.Maliss_Dormouse) && Count.CheckCardRemoved(CardId.Maliss_Dormouse) && Check_Maliss_Dormouse())
                || (!Bot.HasInMonstersZone(CardId.Maliss_White_Rabbit) && Count.CheckCard(CardId.Maliss_White_Rabbit) && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_White_Rabbit())
                || (!Bot.HasInMonstersZone(CardId.Maliss_Chessy_Cat) && Count.CheckCard(CardId.Maliss_Chessy_Cat) && Count.CheckCardRemoved(CardId.Maliss_White_Rabbit) && Check_Maliss_Chessy_Cat())
                || (Count.CheckCardRemoved(CardId.Maliss_March_Hare) && Check_Maliss_March_Hare(CardLocation.Removed) && Bot.Banished.Any(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Monster)))
            );
        }
        private bool SP_Splash_Mage()
        {
            if (Bot.GetMonsters().Count(i => !i.HasType(CardType.Link) || i.LinkCount < 2) < 2
                    && !(Bot.HasInMonstersZone(CardId.Maliss_Red_Ransom)
                        && Count.CheckCardRemoved(CardId.Maliss_Red_Ransom)
                            && Bot.GetMonsters().Count() == 2
                                && Count.CheckCard(CardId.Maliss_White_Binder))
                )
                return false;
    
            bool chk = false;
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                chk = Bot.Graveyard.Any(i => i.HasType(CardType.Monster) && i.HasRace(CardRace.Cyberse) && !i.HasType(CardType.Link));
            else
                chk = true;

            if (chk)
            {
                List<ClientCard> materials = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && !card.HasType(CardType.Link)).ToList();
                AI.SelectMaterials(materials);
            }
            return chk;
        }
        private bool SP_Cyberse_Wicckid()
        {
            if (!Count.CheckCard(CardId.Artifact_Lancea))
                return false;
            if (!Count.CheckCard(CardId.Dimension_Shifter) && !Bot.Graveyard.Any(i => i.HasRace(CardRace.Cyberse)))
                return false;
            if (Bot.HasInHand(CardId.Backup_Ignister) || !Count.CheckCard(CardId.Backup_Ignister))
                return false;
            if (Bot.GetMonsters().Any(i => i.IsFaceup() && i.Level <= 4 && i.HasRace(CardRace.Cyberse))
                    && Bot.GetMonsterCount() == 3 && Count.CheckCard(CardId.Backup_Ignister) && Bot.Hand.Count > 0
                        && Bot.HasInExtra(CardId.Maliss_Hearts_Crypter) && Bot.HasInExtra(CardId.Link_Decoder))
            {
                List<ClientCard> materials = Bot.GetMonsters().Where(card => card.IsFaceup() && card.Sequence > 4).ToList();
                materials.AddRange(Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Link)).ToList());
                AI.SelectMaterials(materials);
                return true;
            }
            if (Bot.GetMonsters().Count(i => !i.HasType(CardType.Link) || i.LinkCount < 2) < 2
                || (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].HasType(CardType.Link) && Bot.MonsterZone[5].LinkCount > 3)
                    || (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].HasType(CardType.Link) && Bot.MonsterZone[6].LinkCount > 3)
            )
                return false;
            if ((Bot.HasInHand(CardId.Backup_Ignister) && Count.CheckCard(CardId.Backup_Ignister) && Bot.GetMonstersInMainZone().Count() < 5)
                || (Bot.HasInHand(CardId.Wizard_Ignister) && Count.CheckCard(CardId.Wizard_Ignister)
                    && Bot.Graveyard.Any(i => i.HasRace(CardRace.Cyberse) && i.HasAttribute(CardAttribute.Dark))
                        && Bot.Graveyard.Count(i => i.HasRace(CardRace.Cyberse)) > 1
                            && Bot.GetMonstersInMainZone().Count() < 4)
                || (Bot.HasInHand(CardId.Maliss_March_Hare) && Count.CheckCard(CardId.Maliss_March_Hare)
                    && Check_Maliss_March_Hare(CardLocation.Hand)
                        && Bot.Graveyard.Count(i => i.HasRace(CardRace.Cyberse)) > 1
                            && Bot.GetMonstersInMainZone().Count() < 4)
            )
            {
                List<ClientCard> materials = Bot.GetMonsters().Where(card => card.IsFaceup() && card.Sequence > 4).ToList();
                materials.AddRange(Bot.GetMonsters().Where(card => card.IsFaceup() && !card.HasType(CardType.Link)).ToList());
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }
        private bool Effect_Haggard_Lizardose()
        {
            return Bot.Graveyard.Any(i => i.HasType(CardType.Monster) && i.Attack <= 2000);
        }
        private bool SP_Haggard_Lizardose()
        {
            List<ClientCard> cards = Bot.GetMonsters().Where(i => i.IsFaceup() && (!i.HasType(CardType.Link) || i.LinkCount < 2)).ToList()
                .GroupBy(i => i.Id)
                .Select(i => i.First())
                .ToList();
            if (cards.Count < 2)
                return false;
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea) && cards.Any(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id)))
            {
                List<ClientCard> materials = cards.Where(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)).ToList();
                materials.AddRange(cards.Where(i => !Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)));
                materials.AddRange(cards.Where(i => ! i.HasSetcode(SetCode.Maliss)));
                AI.SelectMaterials(materials);
                return true;
            }
            if (Bot.HasInExtra(CardId.Splash_Mage))
                return false;
            bool chk = false;
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                chk = Bot.Graveyard.Any(i => i.HasType(CardType.Monster) && i.BaseAttack <= 2000 && Count.CheckCardRemoved(i.Id) && Count.CheckCard(i.Id) && i.HasSetcode(SetCode.Maliss));
            else
                chk = Bot.GetMonsters().Any(i => i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && i.BaseAttack <= 2000 && i.HasSetcode(SetCode.Maliss))
                    || Bot.Graveyard.Any(i => i.HasType(CardType.Monster) && Count.CheckCardRemoved(i.Id) && i.BaseAttack <= 2000 && i.HasSetcode(SetCode.Maliss));
            if (chk)
            {
                List<ClientCard> materials = cards.Where(i => i.BaseAttack <= 2000 && Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss)).ToList();
                materials.AddRange(cards.Where(i => !materials.Contains(i)));
                AI.SelectMaterials(materials);
            }
            return chk;
        }
        private bool SP_Maliss_Link()
        {
            if (Bot.GetMonsters().Any(i => i.HasType(CardType.Link) && i.LinkCount == 2) && Bot.GetMonsters().Any(i => !i.HasType(CardType.Link) && i.HasSetcode(SetCode.Maliss))
                || (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea)
                    && !(Bot.HasInExtra(new int[] {CardId.Cyberse_Wicckid, CardId.Splash_Mage})
                        && Bot.Graveyard.Any(i => i.HasRace(CardRace.Cyberse))
                    ) && !(Bot.HasInExtra(CardId.Haggard_Lizardose)
                        && Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss)
                            && i.HasType(CardType.Monster)
                                && Count.CheckCardRemoved(i.Id)
                        )
                    )
                )
            )
            {
                List<ClientCard> materials = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.LinkCount == 2).ToList();
                List<ClientCard> cards = Bot.GetMonsters().Where(card => card != null && card.IsFaceup() && card.LinkCount < 2 && card.HasSetcode(SetCode.Maliss)).ToList();
                foreach (var card in cards)
                {
                    if (materials.Count == 2)
                        break;
                    if (card.LinkCount > 2)
                        continue;
                    materials.Add(card);
                }
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }
        private bool Effect_Maliss_Link()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Card.IsCode(CardId.Maliss_White_Binder))
                {
                    if (ActivateDescription == Util.GetStringId(Card.Id, 1))
                    {
                        Count.AddCard(Card.Id);
                        return true;
                    }
                    else if(!Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Maliss) && Count.CheckCardRemoved(i.Id))
                        && Enemy.Graveyard.Count() < 3)
                    {
                        Count.AddCard(Card.Id);
                        return true;
                    }
                }
                Count.AddCard(Card.Id);
                return true;
            }
            else 
                return Effect_Maliss_Removed(900);
        }
        private bool SP_Link_Decoder()
        {
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
            {
                if (Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss) && i.Level <= 4 && Count.CheckCardRemoved(i.Id))){
                    AI.SelectMaterials(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && i.Level <= 4 && Count.CheckCardRemoved(i.Id)).ToList());
                    return true;
                }
                return false;
            }
            if (Bot.HasInHand(CardId.Maliss_March_Hare) && Count.CheckCard(CardId.Maliss_March_Hare) && Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss) && i.Level <= 4 && Count.CheckCardRemoved(i.Id)))
            {
                AI.SelectMaterials(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && i.Level <= 4 && Count.CheckCardRemoved(i.Id)).ToList());
                return true;
            }
            if (Bot.GetMonsters().Any(i => i.LinkCount < 3 && i.HasSetcode(SetCode.Maliss)) && Bot.GetMonsters().Count(i => i.LinkCount < 3) >= 3)
            {
                AI.SelectMaterials(Bot.GetMonsters().Where(i => i.LinkCount < 3 && i.HasSetcode(SetCode.Maliss)).ToList());
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Cyberse_Wicckid))
            {
                AI.SelectMaterials(CardId.Cyberse_Wicckid);
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Backup_Ignister) && Bot.HasInHand(CardId.Wizard_Ignister))
            {
                AI.SelectMaterials(CardId.Backup_Ignister);
                return true;
            }

            return false;
        }
        private bool Effect_Maliss_in_the_Mirror()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.Maliss_in_the_Mirror, 0))
            {
                ClientCard LastChainCard = Util.GetLastChainCard();
                return Duel.Player == 1 && LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.MonsterZone;
            }
            else
                return Effect_Maliss_Removed(0);
        }
        private bool SP_Maliss_Hearts_Crypter()
        {
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
            {
                if (Bot.HasInExtra(CardId.Maliss_Red_Ransom) || Bot.GetMonsters().Count(i => !i.HasType(CardType.Link) || i.LinkCount < 2) < (Bot.GetMonsters().Any(i => i.HasType(CardType.Link) && i.LinkCount == 2) ? 1 : 3))
                    return false;
                AI.SelectMaterials(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link) || i.LinkCount < 2).ToList());
                return true;
            }
            if ((Bot.HasInMonstersZone(CardId.Link_Decoder) && Bot.GetMonsters().Count(i => !i.HasType(CardType.Link) || i.LinkCount <= 2) > 2)
                || Bot.GetMonsters().Count(i => !i.HasType(CardType.Link) || i.LinkCount <= 2) > 4
            )
            {
                AI.SelectMaterials(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link) || i.LinkCount < 2).ToList());
                return true;
            }
            return false;
        }
        private bool SP_Maliss_White_Binder()
        {
            if (Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss) && i.LinkCount == 3 && Count.CheckCardRemoved(i.Id)))
            {
                AI.SelectMaterials(Bot.GetMonsters().Where(i => Count.CheckCardRemoved(i.Id) && i.HasSetcode(SetCode.Maliss) || i.LinkCount < 3).ToList());
                return true;
            }
            return false;
        }
        private bool Effect_Maliss_Hearts_Crypter()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && (i.HasType(CardType.Field | CardType.Continuous | CardType.Equip) || i.IsFacedown())) > 0 && Duel.LastChainPlayer != 0)
                {
                    Count.AddCard(Card.Id);
                    return true;
                }
                return false;
            }
            else
                return Effect_Maliss_Removed(900);
        }
        private bool SP_Firewall_Dragon()
        {
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                return false;
            List<ClientCard> materials = Bot.GetMonsters().Where(i => i.IsCode(CardId.Maliss_White_Binder)).ToList();
            materials.AddRange(Bot.GetMonsters().Where(i => i.Sequence > 4 && i.HasType(CardType.Link) && i.LinkCount <= 3));
            materials.AddRange(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && !i.HasType(CardType.Link)));
            materials.AddRange(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link)));
            materials.AddRange(Bot.GetMonsters().Where(i => i.Sequence < 5 && i.HasType(CardType.Link) && i.LinkCount <= 3));
            materials.AddRange(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link)));
            if (materials.Count > 3)
                materials = materials.Take(3).ToList();
            AI.SelectMaterials(materials);
            return true;
        }
        private bool SP_Allied_Code_Talker_Ignister()
        {
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                return false;
            if (Bot.GetMonsters().Count(i => i.LinkCount <= 3) < 3) return false;
            List<ClientCard> materials = Bot.GetMonsters().Where(i => i.IsCode(CardId.Maliss_White_Binder)).ToList();
            materials.AddRange(Bot.GetMonsters().Where(i => i.Sequence > 4 && i.HasType(CardType.Link) && i.LinkCount <= 3));
            materials.AddRange(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && !i.HasType(CardType.Link)));
            materials.AddRange(Bot.GetMonsters().Where(i => i.HasSetcode(SetCode.Maliss) && i.HasType(CardType.Link)));
            materials.AddRange(Bot.GetMonsters().Where(i => i.Sequence < 5 && i.HasType(CardType.Link) && i.LinkCount <= 3));
            materials.AddRange(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link)));
            if (materials.Count > 3)
                materials = materials.Take(3).ToList();
            AI.SelectMaterials(materials);
            return true;
        }
        private bool Effect_Allied_Code_Talker_Ignister()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Allied_Code_Talker_Ignister, 1) && Duel.LastChainPlayer == 1)
            {
                if (Card.Sequence > 4)
                {
                    return Bot.GetMonsters().Any(i => i.Sequence < 3 && (!i.HasType(CardType.Link) || (i.LinkCount <= 3 && Count.CheckCard(i.Id))));
                }
                else
                {
                    return Bot.GetMonsters().Any(i => i.Sequence < 5 && (i.Sequence - Card.Sequence == 1 || Card.Sequence - i.Sequence == 1) && (!i.HasType(CardType.Link) || (i.LinkCount <= 3 && Count.CheckCard(i.Id))));
                }
            }
            return true;
        }
        private bool Effect_Mereologic_Aggregator()
        {
            if (Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && (i.HasType(CardType.Field | CardType.Continuous | CardType.Equip) || i.IsFacedown())) > 0)
            {
                ClientCard LastChainCard = Util.GetLastChainCard();
                if (LastChainCard != null && LastChainCard.Controller == 1 && (LastChainCard.Location == CardLocation.MonsterZone || LastChainCard.Location == CardLocation.SpellZone))
                    AI.SelectCard(LastChainCard);
                Count.AddCard(Card.Id);
                return true;
            }
            return false;
        }
        private bool Effect_Firewall_Dragon()
        {
            if (!Count.CheckCard(CardId.Dimension_Shifter) && Count.CheckCard(CardId.Artifact_Lancea))
                return false;
            return Duel.Player == 0 || (
                Enemy.GetMonsters().Count(i => !i.IsShouldNotBeTarget() && i.IsFaceup()) + Enemy.GetSpells().Count(i => !i.IsShouldNotBeTarget() && i.HasType(CardType.Field | CardType.Continuous | CardType.Equip)) > 0
                && Duel.LastChainPlayer != 0
            );
        }
        private bool Summon_Backup_Ignister()
        {
            if (Bot.GetMonsters().Any(i => i.HasType(CardType.Link)))
                return false;
            Count.AddSummon();
            return true;
        }
        private bool SP_Transcode_Talker()
        {
            if (Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Maliss)))
                return false;
            if (!Bot.GetMonsters().Any(i => !i.HasType(CardType.Link) || i.LinkCount < 2))
                return false;
            if (!Bot.GetMonsters().Any(i => i.HasType(CardType.Link) && i.LinkCount == 2))
                return false;
            List<ClientCard> materials = Bot.GetMonsters().Where(i => i.HasType(CardType.Link) && i.LinkCount == 2).ToList();
            materials.AddRange(Bot.GetMonsters().Where(i => i.IsCode(CardId.Link_Decoder)));
            materials.AddRange(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link) || i.LinkCount < 2 && i.Sequence == (materials[0].Sequence > 4 ? (materials[0].Sequence == 5 ? 1 : 3) : materials[0].Sequence + 1)));
            materials.AddRange(Bot.GetMonsters().Where(i => !i.HasType(CardType.Link) || i.LinkCount < 2));
            if (materials.Count > 2)
                materials = materials.Take(2).ToList();
            AI.SelectMaterials(materials);
            return true;
        }
        private bool Effect_Wizard_Ignister()
        {
            return Card.Location == CardLocation.Hand;
        }
        private bool GoToBattlePhase()
        {           
            if (!Enemy.GetMonsters().Any(i => i.IsDefense()))
            {
                if (Util.GetTotalAttackingMonsterAttack(0) >= Enemy.LifePoints + Util.GetTotalAttackingMonsterAttack(1))
                {                   
                    return true;
                }
            }
            return false;
        }
    }
}
