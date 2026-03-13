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
    [Deck("SuperheavySamurai", "AI_SuperheavySamurai")]
    public class SuperheavySamuraiExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Benkei = 19510093;//弁庆
            public const int Wagon = 34496660;//大八
            public const int Soulpiercer = 90361010;//岩融
            public const int Wakaushi = 82112494;//牛若
            public const int  Scales = 78391364;//天秤
            public const int Booster = 56727340;//地铠
            public const int Motorbike = 83334932;//摩托
            public const int Soulhorns = 14624296;//双角
            public const int Soulpeacemaker = 95500396;//仲裁

            public const int Regulus = 10604644;//轩辕十四

            public const int MaxxG = 23434538;//增殖的G
            public const int JoyousSpring = 14558127;//灰流丽
            public const int PsyFrameDriver = 49036338;//PSY骨架驱动者
            public const int PsyFramegearGamma = 38814750;//PSY骨架装备·γ
            public const int EffectVeiler = 97268402;//效果遮蒙者
            public const int HauntedMansion = 73642296;//屋敷童
            public const int SnowRabbit = 59438930;//幽鬼兔
            public const int LockBird = 94145021;//小丑与锁鸟

            //extra
            public const int Masurawo = 64193046;//益荒男
            public const int Fleur = 84815190;//鲜花女男爵
            public const int ASStardustDragon = 30983281;//加速同调星尘龙
            public const int StardustDragon = 30983281;//星尘龙
            public const int SavageDragon = 27548199;//狞猛龙
            public const int Sarutobi = 76471944;//猿飞
            public const int PSYFramelordOmega = 74586817;//PSY骨架王·Ω
            public const int GearGigant = 28912357;//齿轮齿巨人
            public const int Unicorn = 38342335;//独角兽
            public const int Elf = 27381364;//卫星闪灵·淘气精灵
            public const int Genius = 22423493;//路径灵
            public const int IP = 65741786;//I：P伪装舞会莱娜
            public const int Scarecrow = 33918636;//案山子

        }

        private bool normal_summon = false;
        private bool p_summoned = false;
        private bool p_summoning = false;
        private bool activate_Motorbike = false;//摩托
        private bool activate_Wakaushi = false;//神童
        private bool activate_Scales = false;//天秤
        private bool activate_Wagon = false;//大巴
        private bool activate_Booster = false;//地铠
        private bool activate_Soulpeacemaker = false;//仲裁
        private bool  activate_Benkei = false;//弁庆
        private bool  need_Gear = false;//齿轮齿巨人
        //案山子
        private bool activate_Scarecrow=false;
        private bool summon_Scarecrow=false;
        private bool summon_Scarecrow2=true;
        private bool activate_Sarutobi = false;//猿飞
        private bool activate_Genius = false;//路径灵
        //淘气精灵
        private bool activate_Elf = false;
        private bool summon_Elf = false;
        //手坑
        private bool activate_MaxxG = false;//增殖的G
        private bool activate_PSY = false;//PSY
        private bool activate_LockBird = false;//小丑与锁鸟
        private bool to_deck = false;

        public SuperheavySamuraiExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.Activate, CardId.PSYFramelordOmega,PSYFunction);
            AddExecutor(ExecutorType.Activate, CardId.IP,IPFunction);
            AddExecutor(ExecutorType.Activate, CardId.Sarutobi,SarutobiFunction);
            AddExecutor(ExecutorType.Activate, CardId.Unicorn,UnicornFunction);
            AddExecutor(ExecutorType.Activate, CardId.MaxxG,MaxxCFunction);
            AddExecutor(ExecutorType.Activate, CardId.JoyousSpring,DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.SnowRabbit,DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler,DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.LockBird,LockBirdFunction);
            AddExecutor(ExecutorType.Activate, CardId.PsyFramegearGamma,FunctionInHand);
            AddExecutor(ExecutorType.Activate, CardId.HauntedMansion,FunctionInHand);
            AddExecutor(ExecutorType.Activate, CardId.Masurawo,MasurawoFunction);
            AddExecutor(ExecutorType.Activate, CardId.Genius,GeniusFunction);

        //Motorbike's Effect
            AddExecutor(ExecutorType.Activate, CardId.Motorbike,MotorbikeFunction);

        //Scales's Effect
            AddExecutor(ExecutorType.SpSummon, CardId.Scales);
            AddExecutor(ExecutorType.Activate, CardId.Scales,ScalesFunction);

            //Synchron
            AddExecutor(ExecutorType.SpSummon, CardId.PSYFramelordOmega,PSYFramelordOmegaSynchronFunction);
            
        //Pendulum
            AddExecutor(ExecutorType.Activate, CardId.Wakaushi,WakaushiFunction);
            AddExecutor(ExecutorType.Activate, CardId.Wakaushi,WakaushiEffectFunction);
            AddExecutor(ExecutorType.Activate, CardId.Benkei,BenkeiFunction);
            AddExecutor(ExecutorType.Activate, CardId.Benkei,BenkeiEffectFunction);

        //Normal Summon & Effect
            AddExecutor(ExecutorType.Summon, CardId.Soulpiercer,NormalSummonFunction);
            AddExecutor(ExecutorType.Activate, CardId.Soulpiercer,SoulpiercerFunction);

            AddExecutor(ExecutorType.Summon, CardId.Wagon,NormalSummonFunction);
            AddExecutor(ExecutorType.Activate, CardId.Wagon,WagonFunction);
            AddExecutor(ExecutorType.Activate, CardId.Wagon,WagonFunction);

            AddExecutor(ExecutorType.Summon, CardId.Booster,BoosterNormalSummonFunction);
            AddExecutor(ExecutorType.Summon, CardId.Scales,ScalesNormalSummonFunction);

        //boost & Gear
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterEquipFunction);
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterFunction);
            AddExecutor(ExecutorType.SpSummon, CardId.GearGigant,GearGigantXyzFunction);
            AddExecutor(ExecutorType.Activate, CardId.GearGigant,GearGigantFunction);
            
        //equip Soulpiercer
            AddExecutor(ExecutorType.Activate, CardId.Soulpiercer,SoulpiercerEquipFunction);

        //Link Scarecrow
            AddExecutor(ExecutorType.SpSummon, CardId.Scarecrow,ScarecrowLinkFunction);
            AddExecutor(ExecutorType.Activate, CardId.Scarecrow,ScarecrowFunction);
            AddExecutor(ExecutorType.SpSummon, CardId.Scarecrow,ScarecrowLinkFunction2);

        //Synchron
            AddExecutor(ExecutorType.SpSummon, CardId.ASStardustDragon,ASStardustDragonSynchronFunction);

        //Effect After Synchron
            AddExecutor(ExecutorType.Activate, CardId.SavageDragon,SavageDragonFunction);
            AddExecutor(ExecutorType.Activate, CardId.ASStardustDragon,ASStardustDragonFunction);

        //Wakaushi's Effect After Synchron
            AddExecutor(ExecutorType.Activate, CardId.Wakaushi,WakaushiReturnPFunction);

        //Synchron
            AddExecutor(ExecutorType.SpSummon, CardId.Fleur,FleurSynchronFunction);
            AddExecutor(ExecutorType.Activate, CardId.Fleur,FleurFunction);

        //equip Soulpeacemaker
            AddExecutor(ExecutorType.Activate, CardId.Soulpeacemaker,SoulpeacemakerEquipFunction);
            AddExecutor(ExecutorType.Activate, CardId.Soulpeacemaker,SoulpeacemakerFunction);

        //Link
            AddExecutor(ExecutorType.SpSummon, CardId.Genius,GeniusLinkFunction);

            AddExecutor(ExecutorType.SpSummon, Psummon);

        //Link
            AddExecutor(ExecutorType.SpSummon, CardId.Elf,ElfLinkFunction);
            AddExecutor(ExecutorType.Activate, CardId.Elf,ElfFunction);

            AddExecutor(ExecutorType.Activate, CardId.Motorbike,MotorbikeFunction);

        //Synchron
            AddExecutor(ExecutorType.SpSummon, CardId.SavageDragon,SavageDragonSynchronFunction);

        //Link
            AddExecutor(ExecutorType.SpSummon, CardId.IP,IPLinkFunction);

        //Regulus's Effect
            AddExecutor(ExecutorType.Activate, CardId.Regulus,RegulusFunction);

        //booster
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterEquipFunction2);
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterFunction);
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterEquipFunction3);
            AddExecutor(ExecutorType.Activate, CardId.Booster,BoosterFunction);

        //Synchron
            AddExecutor(ExecutorType.SpSummon, CardId.Masurawo,MasurawoSynchronFunction);
            AddExecutor(ExecutorType.SpSummon, CardId.Sarutobi,DeSynchronFunction);

        //equip Soulhorns
            AddExecutor(ExecutorType.Activate, CardId.Soulhorns,SoulhornsEquipFunction);

        }
        public override void OnNewTurn()
        {
            normal_summon = false;
            p_summoned = false;
            p_summoning = false;
            activate_Motorbike = false;
            activate_Wakaushi = false;
            activate_Scales = false;
            activate_Wagon = false;
            activate_Booster = false;
            activate_Soulpeacemaker = false;
            activate_Benkei = false;
            need_Gear = false;
            activate_Scarecrow=false;
            summon_Scarecrow=false;
            summon_Scarecrow2=true;
            activate_Elf = false;
            summon_Elf = false;
            activate_MaxxG = false;
            activate_PSY = false;
            activate_LockBird = false;
            activate_Genius = false;
            activate_Sarutobi = false;
            to_deck = false;
            base.OnNewTurn();
        }
        public override bool OnSelectHand()
        {
            return true;
        }
        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.IsFaceup() && Card.IsAttack() && (Card.Id == CardId.Masurawo || Card.Id == CardId.Sarutobi))
                return true;
            return false;
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardId == CardId.Masurawo || cardId == CardId.Sarutobi)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.Scarecrow)
                {
                    int a=(Zones.z6 & available);
                    int b=(Zones.z5 & available);
                    if (Bot.MonsterZone[2] != null && Bot.MonsterZone[2].Controller == 0 && !FinalCards(Bot.MonsterZone[2].Id))
                        a = 0;
                    else if (Bot.MonsterZone[0] != null && Bot.MonsterZone[0].Controller == 0 && !FinalCards(Bot.MonsterZone[0].Id))
                        b = 0;
                    if (b > 0) return Zones.z5;
                    if (a > 0) return Zones.z6;
                }
                else if (cardId == CardId.Unicorn || cardId == CardId.Elf || cardId == CardId.IP)
                {
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                }
                else if (cardId == CardId.Genius)
                {
                    int a=(Zones.z6 & available);
                    int b=(Zones.z5 & available);
                    if (Bot.MonsterZone[4] != null && Bot.MonsterZone[4].Controller == 0 && !FinalCards(Bot.MonsterZone[4].Id))
                        a = 0;
                    else if (Bot.MonsterZone[0] != null && Bot.MonsterZone[0].Controller == 0 && !FinalCards(Bot.MonsterZone[0].Id))
                        b = 0;
                    if (a > 0) return Zones.z6;
                    if (b > 0) return Zones.z5;
                }
                else if (cardId == CardId.Regulus || cardId == CardId.GearGigant)
                {
                    if ((Zones.z3 & available) > 0) return Zones.z3;
                }
                else
                {
                    if ((Zones.z1 & available) > 0) return Zones.z1;
                    if ((Zones.z4 & available) > 0) return Zones.z4;
                    if ((Zones.z2 & available) > 0) return Zones.z2;
                    if ((Zones.z3 & available) > 0) return Zones.z3;
                    if ((Zones.z0 & available) > 0) return Zones.z0;
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (AI.HaveSelectedCards()) return null;
            if (p_summoning || ((Card == Bot.SpellZone[0] || Card == Bot.SpellZone[4]) && hint == HintMsg.SpSummon &&
                Card.HasType(CardType.Pendulum)))
            {
                List<ClientCard> result = new List<ClientCard>();
                List<ClientCard> scards = cards.Where(card => card != null && card.HasSetcode(0x9a) && card.Level == 4).ToList();
                if (scards.Count <2) scards = cards.Where(card => card != null && card.HasSetcode(0x9a)).ToList();
                p_summoning = false;
                if (scards.Count > 0) return Util.CheckSelectCount(result, scards, 1, 1);
                else if (min == 0) return result; // empty
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
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
        private bool FinalCards(int cname)
        {
            int[] cardsname = new[] {CardId.Masurawo,CardId.Fleur,CardId.SavageDragon,CardId.Sarutobi,CardId.Regulus,CardId.IP};
            foreach(var cardname in cardsname)
            {
                if (cname == cardname) return true;
            }
            return false;
        }
        private bool TurnerCards(int cname)
        {
            int[] cardsname =new[] {CardId.PsyFramegearGamma,CardId.Wakaushi,CardId.Motorbike};
            foreach(var cardname in cardsname)
            {
            if (cname == cardname) return true;
            }
            return false;
        }
        private bool Psummon()
        {
            List<ClientCard> cards = GetZoneCards(CardLocation.Hand, Bot).Where(card => card != null && card.HasSetcode(0x9a) && card.Level > 1 && card.Level < 8).ToList();
            if (cards.Count > 0 && Card.Location == CardLocation.SpellZone)
            {
                p_summoning = true;
                p_summoned = true;
                return true;
            }
            return false;
        }
        private bool MaxxCFunction()
        {
            activate_MaxxG = true;
            return DefaultMaxxC() && !activate_LockBird;
        }
        private bool FunctionInHand()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            return Duel.LastChainPlayer == 1;
        }
        private bool LockBirdFunction()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Duel.Player == 0 || activate_LockBird)
            {
                return false;
            }
            activate_LockBird = true;
            return !activate_MaxxG;
        }
        private bool MotorbikeFunction()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                int targetid = -1;
                List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup()).ToList();
                if (!(Bot.HasInHand(CardId.Wakaushi) || Bot.HasInMonstersZone(CardId.Wakaushi) || Bot.HasInSpellZone(CardId.Wakaushi)) && !activate_Wakaushi)
                {
                    targetid = CardId.Wakaushi;
                }
                else if (cards.Count() == 0 && !normal_summon)
                {
                    targetid = CardId.Soulpiercer;
                }
                else if (!Bot.HasInHand(CardId.Soulpeacemaker) && !Bot.HasInSpellZone(CardId.Soulpeacemaker) && !activate_Soulpeacemaker && (normal_summon || Bot.HasInMonstersZone(CardId.Scarecrow)))
                {
                    targetid = CardId.Soulpeacemaker;
                }
                else
                {
                    targetid = CardId.Soulpiercer;
                }
                if (targetid > 0) AI.SelectCard(targetid);
                activate_Motorbike = true;
                return true;
            }
            else if (Card.Location == CardLocation.MonsterZone && activate_Elf)
            {
                AI.SelectCard(Card);
                activate_Elf = false;
                return true;
            }
            return false;
        }
        private bool BoosterNormalSummonFunction()
        {
            List<ClientCard> cards = Bot.Hand.GetMonsters().Where(card => card != null && card.Id == CardId.Booster).ToList();
            return (NormalSummonFunction() && !activate_Booster && cards.Count >= 2);
        }
        private bool ScalesNormalSummonFunction()
        {
            return (NormalSummonFunction() && (Bot.HasInGraveyard(new[] {
                CardId.Soulpiercer,
                CardId.Motorbike,
                CardId.Wakaushi,
                CardId.Wagon,
                CardId.Booster,
            }) || (Bot.HasInHand(CardId.Booster) && !activate_Booster)));
        }
        private bool NormalSummonFunction()
        {
            normal_summon = true;
            return DefaultMonsterSummon();
        }
        private bool ScalesFunction()
        {
            AI.SelectCard(new[] {
                CardId.Soulpiercer,
                CardId.Motorbike,
                CardId.Wakaushi,
                CardId.Wagon,
                CardId.Booster,
            });
            activate_Scales = true;
            return true;
        }
        private bool WagonFunction()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Wagon, 0))
                return Card.IsAttack();
            if (ActivateDescription == Util.GetStringId(CardId.Wagon, 1))
            {
                int targetid = -1;
                if (!(Bot.HasInHand(CardId.Soulpiercer)||Bot.HasInMonstersZone(CardId.Soulpiercer)))
                {
                    targetid = CardId.Soulpiercer;
                }
                else if (!Bot.HasInHand(CardId.Soulpeacemaker) && !activate_Soulpeacemaker)
                {
                    targetid = CardId.Soulpeacemaker;
                }
                else if (!Bot.HasInHand(CardId.Booster) && !activate_Booster)
                {
                    targetid = CardId.Booster;
                }
                if (targetid > 0) AI.SelectCard(targetid);
                activate_Wagon = true;
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool SoulpiercerFunction()
        {
            if (Card.Location == CardLocation.Grave)
            {
                int CardCount = GetZoneCards(CardLocation.Hand, Bot).Count(card => card != null && card.HasSetcode(0x9a) && card.Level >= 2 && card.Level <= 7);
                int targetid = -1;
                if (!Bot.HasInHand(CardId.Motorbike) && !activate_Motorbike)
                {
                    targetid = CardId.Motorbike;
                }
                else if (!(Bot.HasInHand(CardId.Wakaushi) || Bot.HasInMonstersZone(CardId.Wakaushi) || Bot.HasInSpellZone(CardId.Wakaushi)) && !activate_Wakaushi)
                {
                    targetid = CardId.Wakaushi;
                }
                else if (!Bot.HasInHand(CardId.Soulpeacemaker) && !activate_Soulpeacemaker)
                {
                    targetid = CardId.Soulpeacemaker;
                }
                else if (!Bot.HasInHand(CardId.Scales) && !activate_Scales && (!normal_summon || !p_summoned) && (activate_Soulpeacemaker || (!Bot.HasInHand(CardId.Soulpeacemaker) && !Bot.HasInSpellZone(CardId.Soulpeacemaker))))
                {
                    targetid = CardId.Scales;
                }
                else if (!Bot.HasInHand(CardId.Wagon) && !activate_Wagon)
                {
                    targetid = CardId.Wagon;
                }
                else if (CardCount < 2 && !p_summoned)
                {
                    targetid = CardId.Wakaushi;
                }
                else if (!Bot.HasInHand(CardId.Booster) && !activate_Booster)
                {
                    targetid = CardId.Booster;
                }
                else if (!Bot.HasInHand(CardId.Soulhorns) && !Bot.HasInSpellZone(CardId.Soulhorns) && (Bot.HasInMonstersZone(CardId.Sarutobi) || Bot.HasInMonstersZone(CardId.Masurawo)))
                {
                    targetid = CardId.Soulhorns;
                }
                else {targetid = CardId.Wakaushi;}
                if (targetid > 0) AI.SelectCard(targetid);
                return true;
            }
            return false;
        }
        private bool WakaushiFunction()
        {
            if (Card.Location != CardLocation.Hand||Bot.HasInMonstersZone(CardId.Wakaushi))
                return false;
            ClientCard l = Util.GetPZone(0, 0);
            ClientCard r = Util.GetPZone(0, 1);
            if (l == null && r == null)
                return true;
            if (l == null && r.RScale != Card.LScale)
                return true;
            if (r == null && l.LScale != Card.RScale)
                return true;
            return false;
        }
        private bool WakaushiEffectFunction()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                AI.SelectCard(CardId.Benkei);
                activate_Wakaushi = true;
                return true;
            }
            return false;
        }
        private bool BenkeiFunction()
        {
            if (Card.Location != CardLocation.Hand || Bot.HasInSpellZone(CardId.Benkei)) return false;
            List<ClientCard> cards1 = GetZoneCards(CardLocation.Hand, Bot).Where(card => card != null && card.Id == CardId.Benkei).ToList();
            List<ClientCard> cards2 = GetZoneCards(CardLocation.Removed, Bot).Where(card => card != null && card.Id == CardId.Benkei).ToList();
            if (cards1.Count() >= 2 || Bot.HasInGraveyard(CardId.Benkei) || Bot.HasInExtra(CardId.Benkei) || cards2.Count() > 0)
                return true;
            return false;
        }
        private bool BenkeiEffectFunction()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                int targetid = -1;
                if (!(Bot.HasInHand(CardId.Soulpiercer) || Bot.HasInMonstersZone(CardId.Soulpiercer) || Bot.HasInSpellZone(CardId.Soulpiercer)) && !(Bot.HasInMonstersZone(CardId.Scarecrow) && !activate_Soulpeacemaker))
                {
                    targetid = CardId.Soulpiercer;
                }
                else if (!Bot.HasInHand(CardId.Soulpeacemaker) && !activate_Soulpeacemaker)
                {
                    targetid = CardId.Soulpeacemaker;
                }
                else if (!Bot.HasInHand(CardId.Booster) && !activate_Booster)
                {
                    targetid = CardId.Booster;
                }
                if (targetid > 0) AI.SelectCard(targetid);
                activate_Benkei = true;
                return true;
            }
            return false;
        }
        private bool WakaushiReturnPFunction()
        {
            if (Card.Location == CardLocation.Extra||Card.Location == CardLocation.Removed)
            {
                ClientCard l = Util.GetPZone(0, 0);
                ClientCard r = Util.GetPZone(0, 1);
                if (l == null && r == null)
                    return true;
                if (l == null && r.RScale != Card.LScale)
                    return true;
                if (r == null && l.LScale != Card.RScale)
                    return true;
            }
            return false;
        }
        private bool MasurawoFunction()
        {
            if (ActivateDescription == 96)
            {
                List<ClientCard> cards = GetZoneCards(CardLocation.SpellZone, Bot).Where(card => card != null && card.HasSetcode(0x9a)).ToList();
                if (cards.Count > 0)
                {
                    AI.SelectCard(cards);
                    return true;
                }
                else
                {
                    cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.HasSetcode(0x9a) && !FinalCards(card.Id)).ToList();
                    if (cards.Count > 0)
                    {
                        AI.SelectCard(cards);
                        return true;
                    }
                }
            }
            return true;

        }
        private bool MasurawoSynchronFunction()
        {
            bool chk = true;
            if (Bot.HasInMonstersZone(CardId.ASStardustDragon) || Bot.HasInMonstersZone(CardId.Benkei))
                chk = false;
            var materials_lists = Util.GetSynchroMaterials(Bot.MonsterZone,12,1,1,false,chk,null,
                card => { return !FinalCards(card.Id); });
            if (materials_lists.Count <= 0) return false;
            AI.SelectMaterials(materials_lists[0]);
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool FleurSynchronFunction()
        {
            bool chk = true;
            if (Bot.HasInMonstersZone(CardId.Motorbike) && (Bot.HasInMonstersZone(CardId.ASStardustDragon) || Bot.HasInMonstersZone(CardId.Benkei)))
                chk = false;
            var materials_lists = Util.GetSynchroMaterials(Bot.MonsterZone,10,1,1,false,chk,null,
                card => { return !FinalCards(card.Id); });
            if (materials_lists.Count <= 0) return false;
            AI.SelectMaterials(materials_lists[0]);
            return true;
        }
        private bool DeSynchronFunction()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool SavageDragonSynchronFunction()
        {
            if (Bot.HasInGraveyard(new[] {
                CardId.Scarecrow,
                CardId.IP,
                CardId.Genius,
                CardId.Unicorn,
                CardId.Elf,
            }))
            {
                return true;
            }
            return false;
        }
        private bool ASStardustDragonSynchronFunction()
        {
            if (Bot.HasInGraveyard(CardId.Motorbike) || Bot.HasInGraveyard(CardId.PsyFramegearGamma))
            {
                return (Bot.HasInExtra(CardId.Fleur) || Bot.HasInExtra(CardId.Masurawo));
            }
            else if (Bot.HasInMonstersZone(CardId.Motorbike))
            {
                AI.SelectMaterials(CardId.Motorbike);
                return true;
            }
            else if (Bot.HasInMonstersZone(CardId.PsyFramegearGamma))
            {
                AI.SelectMaterials(CardId.PsyFramegearGamma);
                return true;
            }
            return false;
        }
        private bool PSYFramelordOmegaSynchronFunction()
        {
            if (Bot.HasInMonstersZone(CardId.Motorbike))
                AI.SelectMaterials(CardId.Motorbike);
            else if (Bot.HasInMonstersZone(CardId.PsyFramegearGamma))
                AI.SelectMaterials(CardId.PsyFramegearGamma);
            return activate_PSY || activate_Scales;
        }
        private bool SavageDragonFunction()
        {
            if (Duel.LastChainPlayer == 1)
                return true;
            AI.SelectCard(new[]
                {
                    CardId.Unicorn,
                    CardId.Genius,
                    CardId.Elf,
                    CardId.IP,
                    CardId.Scarecrow
                });
            return true;
        }
        private bool ASStardustDragonFunction()
        {
            if (Duel.LastChainPlayer == 1 && ActivateDescription == Util.GetStringId(CardId.ASStardustDragon, 1))
            {
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.ASStardustDragon, 0))
            {

                int targetid = -1;
                if (Bot.HasInGraveyard(CardId.Motorbike))
                {
                    targetid = CardId.Motorbike;
                }
                else if (Bot.HasInGraveyard(CardId.PsyFramegearGamma))
                {
                    targetid = CardId.PsyFramegearGamma;
                }
                if (targetid > 0) AI.SelectCard(targetid);
                if (targetid == CardId.Motorbike && !Bot.HasInExtra(CardId.Fleur) && Bot.HasInExtra(CardId.Masurawo))
                    activate_Elf = true;
                return true;
            }
            return false;
        }
        private bool ScarecrowLinkFunction()
        {
            List<ClientCard> material = new List<ClientCard>();
            List<ClientCard> cards = Bot.GetMonstersInExtraZone().Where(card => card != null && card.Id == CardId.Scarecrow).ToList();
            if ((cards.Count() > 0 && !summon_Scarecrow) || summon_Scarecrow || activate_Scarecrow) return false;
            int targetid = -1;
            if (Bot.MonsterZone[0] != null && Bot.MonsterZone[2] != null) {
                if (Bot.MonsterZone[0].Id == CardId.Soulpiercer) material.Add(Bot.MonsterZone[0]);
                else if (Bot.MonsterZone[2].Id == CardId.Soulpiercer) material.Add(Bot.MonsterZone[2]);
                else if (!FinalCards(Bot.MonsterZone[0].Id) && Bot.MonsterZone[0].HasSetcode(0x9a)) material.Add(Bot.MonsterZone[0]);
                else if (!FinalCards(Bot.MonsterZone[2].Id) && Bot.MonsterZone[2].HasSetcode(0x9a)) material.Add(Bot.MonsterZone[2]);
            }
            else if (Bot.HasInMonstersZone(CardId.Soulpiercer))
            {
                targetid = CardId.Soulpiercer;
            }
            else if (Bot.HasInMonstersZone(CardId.Wagon))
            {
                targetid = CardId.Wagon;
            }
            if (material.Count > 0) AI.SelectMaterials(material);
            else if (targetid > 0) AI.SelectMaterials(targetid);
            summon_Scarecrow=true;
            return (Bot.HasInGraveyard(new[] {
                CardId.Soulpiercer,
                CardId.Wakaushi,
                CardId.Benkei,
                CardId.Wagon,
            })||Bot.HasInMonstersZone(new[] {
                CardId.Soulpiercer,
                CardId.Wagon,
                CardId.Wakaushi,
            }));
        }
        private bool DragonRavineField()
        {
            if (Card.Location == CardLocation.Hand)
                return DefaultField();
            return false;
        }

        private bool ScarecrowFunction()
        {
            int tributeId = -1;
            if (Bot.HasInHand(CardId.PsyFrameDriver))
            {tributeId = CardId.PsyFrameDriver;}
            else if (Bot.HasInHand(CardId.PsyFramegearGamma))
            {tributeId = CardId.PsyFramegearGamma;}
            else if (Bot.HasInHand(CardId.Benkei))
            {tributeId = CardId.Benkei;}
            else if (Bot.HasInHand(CardId.HauntedMansion))
            {tributeId = CardId.HauntedMansion;}
            else if (Bot.HasInHand(CardId.EffectVeiler))
            {tributeId = CardId.EffectVeiler;}
            else if (Bot.HasInHand(CardId.SnowRabbit))
            {tributeId = CardId.SnowRabbit;}
            else if (Bot.HasInHand(CardId.JoyousSpring))
            {tributeId = CardId.JoyousSpring;}
            else if (Bot.HasInHand(CardId.Booster))
            {tributeId = CardId.Booster;}
            else if (Bot.HasInHand(CardId.Wagon))
            {tributeId = CardId.Wagon;}
            else if (Bot.HasInHand(CardId.Scales))
            {tributeId = CardId.Scales;}
            else if (Bot.HasInHand(CardId.LockBird))
            {tributeId = CardId.LockBird;}
            else if (Bot.HasInHand(CardId.MaxxG))
            {tributeId = CardId.MaxxG;}
            int needId = -1;
            if (Bot.HasInGraveyard(CardId.Soulpiercer))
            {
                if (Bot.HasInGraveyard(CardId.Scales) && !activate_Scales)
                {needId = CardId.Scales;}
                else
                {needId = CardId.Soulpiercer;}
            }
            else if (Bot.HasInGraveyard(CardId.Masurawo))
            {needId = CardId.Masurawo;}
            else if (Bot.HasInGraveyard(CardId.Sarutobi))
            {needId = CardId.Sarutobi;}
            else if (Bot.HasInMonstersZone(CardId.Soulpiercer))
            {
                if (Bot.HasInGraveyard(CardId.Wakaushi))
                {needId = CardId.Wakaushi;}
                if (Bot.HasInGraveyard(CardId.Motorbike))
                {needId = CardId.Motorbike;}
            }
            if (GetZoneCards(CardLocation.Hand, Bot).Count(card => card != null && card.Id == CardId.Scales) + GetZoneCards(CardLocation.Grave, Bot).Count(card => card != null && card.Id == CardId.Scales) + GetZoneCards(CardLocation.Onfield, Bot).Count(card => card != null && card.Id == CardId.Scales) == 2 && GetZoneCards(CardLocation.Hand, Bot).Count(card => card != null && card.Id == CardId.Scales)>=1 && !activate_Scales)
            {
                tributeId = CardId.Scales;
                needId = CardId.Scales;
            }
            AI.SelectCard(tributeId);
            AI.SelectNextCard(needId);
            if (((!Bot.HasInHand(CardId.Wakaushi) && !Bot.HasInSpellZone(CardId.Wakaushi)) || activate_Wakaushi)
                && (!Bot.HasInHand(CardId.Motorbike) || activate_Motorbike)
                && ((!Bot.HasInHand(CardId.Soulpeacemaker) && !Bot.HasInSpellZone(CardId.Soulpeacemaker)) || activate_Soulpeacemaker)
                && (!Bot.HasInSpellZone(CardId.Benkei) || activate_Benkei)
                && (needId == CardId.Soulpiercer)
                && (!activate_Wakaushi || !activate_Motorbike || !activate_Soulpeacemaker || !activate_Benkei)
            )
            {
                summon_Scarecrow2 = false;
            }
            activate_Scarecrow = true;
            return true;
        }
        private bool ScarecrowLinkFunction2()
        {
            if (!summon_Scarecrow2)
            {
                summon_Scarecrow2 = true;
                return true;
            }
            return false;
        }
        private bool UnicornFunction()
        {
            List<ClientCard> Enemycards = GetZoneCards(CardLocation.Onfield,Enemy);
            if (Bot.Hand.Count == 0 || Enemycards.Count(card => card != null && !card.IsShouldNotBeTarget()) == 0)
            {
                if (to_deck) to_deck = false;
                return false;
            }
            int tributeId = -1;
            if (Bot.HasInHand(CardId.PsyFrameDriver))
            {tributeId = CardId.PsyFrameDriver;}
            else if (Bot.HasInHand(CardId.PsyFramegearGamma))
            {tributeId = CardId.PsyFramegearGamma;}
            else if (Bot.HasInHand(CardId.Benkei))
            {tributeId = CardId.Benkei;}
            else if (Bot.HasInHand(CardId.HauntedMansion))
            {tributeId = CardId.HauntedMansion;}
            else if (Bot.HasInHand(CardId.EffectVeiler))
            {tributeId = CardId.EffectVeiler;}
            else if (Bot.HasInHand(CardId.SnowRabbit))
            {tributeId = CardId.SnowRabbit;}
            else if (Bot.HasInHand(CardId.JoyousSpring))
            {tributeId = CardId.JoyousSpring;}
            else if (Bot.HasInHand(CardId.Booster))
            {tributeId = CardId.Booster;}
            else if (Bot.HasInHand(CardId.Wagon))
            {tributeId = CardId.Wagon;}
            else if (Bot.HasInHand(CardId.Scales))
            {tributeId = CardId.Scales;}
            else if (Bot.HasInHand(CardId.LockBird))
            {tributeId = CardId.LockBird;}
            else if (Bot.HasInHand(CardId.MaxxG))
            {tributeId = CardId.MaxxG;}
            if (to_deck) to_deck = false;
            AI.SelectCard(tributeId);
            return true;
        }
        private bool BoosterEquipFunction()
        {
            if (Card.Location != CardLocation.Hand || activate_Booster)
                return false;
            List<ClientCard> ChkCardsHand =  Bot.Hand.GetMonsters().ToList();
            foreach (var card in ChkCardsHand)
            {
                if (card.Id == CardId.Motorbike && !activate_Motorbike) return false;
                else if (card.Id == CardId.Soulpiercer) return false;
                else if (card.Id == CardId.Soulpeacemaker && !activate_Soulpeacemaker) return false;
                else if (card.Id == CardId.Wakaushi && !activate_Wakaushi) return false;
                else if (card.Id == CardId.Wagon && (!activate_Wagon || !normal_summon)) return false;
                else if (card.Id == CardId.Benkei && !activate_Benkei) return false;
            }
            List<ClientCard> ChkCardsSpell = GetZoneCards(CardLocation.SpellZone,Bot).Where(card => card != null && card.IsFaceup()).ToList();
            foreach (var card in ChkCardsSpell)
            {
                if (card.Id == CardId.Wakaushi && !activate_Wakaushi) return false;
                else if (card.Id == CardId.Soulpiercer) return false;
                else if (card.Id == CardId.Soulpeacemaker && !activate_Soulpeacemaker) return false;
                else if (card.Id == CardId.Wakaushi && !activate_Wakaushi) return false;
                else if (card.Id == CardId.Benkei && !activate_Benkei) return false;
            }
            List<ClientCard> ChkCardsMonster = GetZoneCards(CardLocation.MonsterZone,Bot).Where(card => card != null && card.IsFaceup() && card.Level == 4).ToList();
            if (ChkCardsMonster.Count == 0) return false;
            foreach (var card in ChkCardsMonster)
            {
                if (card.Id == CardId.Soulpiercer) return false;
            }
            List<ClientCard> ChkCardsGrave = GetZoneCards(CardLocation.Grave,Bot).ToList();
            foreach (var card in ChkCardsGrave)
            {
                if (card.Id == CardId.Soulpiercer && (Bot.HasInMonstersZone(CardId.Scarecrow) || Bot.HasInExtra(CardId.Scarecrow))) return false;
                else if (card.Level == 4 && card.HasRace(CardRace.Machine) && Bot.HasInHand(CardId.Scales) && !normal_summon) return false;
            }
            if (Bot.HasInExtra(CardId.IP) && p_summoned) return true;
            need_Gear = true;
            return true;
        }
        private bool BoosterEquipFunction2()
        {
            if (Bot.HasInExtra(CardId.IP) && p_summoned && !activate_Booster) return true;
            return false;
        }
        private bool BoosterEquipFunction3()
        {
            List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone,Bot).Where(card => card != null && card.IsFaceup() && !FinalCards(card.Id) && card.Id != CardId.Scarecrow).ToList();
            if (Bot.HasInMonstersZone(CardId.IP) && p_summoned && !activate_Booster && cards.Count() == 0) return true;
            return false;
        }
        private bool BoosterFunction()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                activate_Booster = true;
                return true;
            }
            return false;
        }
        private bool GearGigantXyzFunction()
        {
            if (need_Gear)
            {
                need_Gear = false;
                return true;
            }
            return false;
        }
        private bool GearGigantFunction()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                List<ClientCard> ChkCards = GetZoneCards(CardLocation.MonsterZone,Bot).Where(card => card != null && card.IsFaceup() && card.HasSetcode(0x9a)).ToList();
                int targetid = -1;
                if (!Bot.HasInHand(CardId.Motorbike) && !activate_Motorbike)
                {
                    targetid = CardId.Motorbike;
                }
                else if (!(Bot.HasInHand(CardId.Wakaushi) || Bot.HasInSpellZone(CardId.Wakaushi)) && !activate_Wakaushi)
                {
                    targetid = CardId.Wakaushi;
                }
                else if (!Bot.HasInHand(CardId.Soulpiercer) && (!normal_summon || (ChkCards.Count >= 1)))
                {
                    targetid = CardId.Soulpiercer;
                }
                if (targetid > 0) AI.SelectCard(targetid);
                return true;
            }
            return false;
        }
        private bool SoulpiercerEquipFunction()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            int tributeId = -1;
            if (Bot.HasInMonstersZone(CardId.Wagon))
            {tributeId = CardId.Wagon;}
            else if (Bot.HasInMonstersZone(CardId.Wakaushi))
            {tributeId = CardId.Wakaushi;}
            AI.SelectCard(tributeId);
            return Bot.HasInMonstersZone(new[] {
                CardId.Wakaushi,
                CardId.Wagon,
            });
        }
        private bool SoulpeacemakerEquipFunction()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            int tributeId = -1;
            List<ClientCard> cards = Bot.GetMonstersInExtraZone().Where(card => card != null && card.Id == CardId.Scarecrow).ToList();
            if (cards.Count() > 0)
                AI.SelectCard(cards);
            else
            {
                if (Bot.HasInMonstersZone(CardId.Scarecrow))
                {tributeId = CardId.Scarecrow;}
                else if (Bot.HasInMonstersZone(CardId.Soulpiercer))
                {tributeId = CardId.Soulpiercer;}
                AI.SelectCard(tributeId);
            }
            return Bot.HasInMonstersZone(new[] {
                CardId.Scarecrow,
                CardId.Soulpiercer,
            });
        }
        private bool SoulhornsEquipFunction()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            int tributeId = -1;
            if (Bot.HasInMonstersZone(CardId.Masurawo))
            {tributeId = CardId.Masurawo;}
            else if (Bot.HasInMonstersZone(CardId.Sarutobi))
            {tributeId = CardId.Sarutobi;}
            AI.SelectCard(tributeId);
            return Bot.HasInMonstersZone(new[] {
                CardId.Masurawo,
                CardId.Sarutobi,
            });
        }
        private bool SoulpeacemakerFunction()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                int tributeId = -1;
                if (Bot.HasInMonstersZone(CardId.Soulpiercer))
                {tributeId = CardId.Wakaushi;}
                else if (Bot.HasInGraveyard(CardId.Soulpiercer)||!activate_Scales)
                {tributeId = CardId.Scales;}
                else if (!Bot.HasInGraveyard(CardId.Soulpiercer)||activate_Scales)
                {tributeId = CardId.Soulpiercer;}
                AI.SelectCard(tributeId);
                activate_Soulpeacemaker = true;
                return true;
            }
            return false;
        }
        private bool GeniusLinkFunction()
        {
            if ((Bot.MonsterZone[4] != null && Bot.MonsterZone[4].Controller == 0 && !FinalCards(Bot.MonsterZone[4].Id)) && (Bot.MonsterZone[0] != null && Bot.MonsterZone[0].Controller == 0 && !FinalCards(Bot.MonsterZone[0].Id)))
                return false;
            List<ClientCard> Pcards = GetZoneCards(CardLocation.Hand, Bot).Where(card => card != null && card.HasSetcode(0x9a) && card.Level > 1 && card.Level < 8).ToList();
            if (Pcards.Count() < 2 && !Bot.HasInMonstersZone(CardId.Soulpiercer)) return false;
            List<ClientCard> Rcards = GetZoneCards(CardLocation.Removed, Bot).Where(card => card != null && card.Id == CardId.Regulus).ToList();
            if (Bot.HasInHand(CardId.Regulus) || Bot.HasInGraveyard(CardId.Regulus) || Bot.HasInSpellZone(CardId.Regulus) || Bot.HasInMonstersZone(CardId.Regulus) || Rcards.Count() > 0) return false;
            bool linkchk = false;
            List<ClientCard> materials = new List<ClientCard>();
            if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0 && Bot.MonsterZone[6].Id != CardId.Scarecrow && !FinalCards(Bot.MonsterZone[6].Id))
            {
                materials.Add(Bot.MonsterZone[6]);
                linkchk = true;
            }
            else if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Controller == 0 && Bot.MonsterZone[5].Id != CardId.Scarecrow && !FinalCards(Bot.MonsterZone[5].Id))
            {
                materials.Add(Bot.MonsterZone[5]);
                linkchk = true;
            }
            List<ClientCard> cards = Bot.GetMonstersInMainZone().Where(card => card != null && card.IsFaceup() && card.HasRace(CardRace.Machine)).ToList();
            foreach (var card in cards)
            {
                if (card == null || FinalCards(card.Id)) continue;
                else materials.Add(card);
            }
            if (materials.Count <=1) return false;
            AI.SelectMaterials(materials);
            return ((Bot.GetMonstersInExtraZone().Count == 0 || linkchk) && !p_summoned && !activate_Genius);
        }
        private bool GeniusFunction()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Genius,1))
            {
                AI.SelectCard(CardId.Regulus);
                activate_Genius = true;
                return true;
            }
            return false;
        }
        private bool ElfLinkFunction()
        {
            if (!Bot.HasInGraveyard(CardId.Motorbike)) return false;
            List<ClientCard> materials = new List<ClientCard>();
            if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0 && !FinalCards(Bot.MonsterZone[6].Id))
            {
                materials.Add(Bot.MonsterZone[6]);
            }
            else if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Controller == 0 && !FinalCards(Bot.MonsterZone[5].Id))
            {
                materials.Add(Bot.MonsterZone[5]);
            }
            List<ClientCard> TunrerCards = Bot.GetMonstersInMainZone().Where(card => card != null && card.IsFaceup() && TurnerCards(card.Id) && !FinalCards(card.Id)).ToList();
            List<ClientCard> UnTunrercards = Bot.GetMonstersInMainZone().Where(card => card != null && card.IsFaceup() && !TurnerCards(card.Id) && !FinalCards(card.Id)).ToList();
            if (UnTunrercards.Count == 0) return false;
            else if (TunrerCards.Count >= UnTunrercards.Count && UnTunrercards.Count > 0)
            {
                foreach (var card in TunrerCards)
                {
                    if (card == null) continue;
                    else if (materials.Count(ccard =>ccard != null && ccard.Id == card.Id) <= 0) materials.Add(card);
                }
            } 
            else
            {
                foreach (var card in UnTunrercards)
                {
                    if (card == null) continue;
                    else if (materials.Count(ccard =>ccard != null && ccard.Id == card.Id) <= 0) materials.Add(card);
                }
            }
            if (materials.Count <=1) return false;
            AI.SelectMaterials(materials);
            summon_Elf = true;
            return true;
        }
        private bool ElfFunction()
        {
            if (Duel.Player == 0)
            {
                activate_Elf = true;
                AI.SelectCard(CardId.Motorbike);
                return Bot.HasInGraveyard(CardId.Motorbike);
            }
            List<ClientCard> cards1 = GetZoneCards(CardLocation.MonsterZone, Enemy);
            List<ClientCard> cards2 = GetZoneCards(CardLocation.SpellZone, Enemy);
            if (cards1.Count() > 0 || cards2.Count() >= 3)
            {
                if (Bot.HasInExtra(CardId.Unicorn) && Bot.HasInGraveyard(CardId.IP))
                    AI.SelectCard(CardId.IP);
                else
                {
                    AI.SelectCard(CardId.Motorbike);
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                }
                activate_Elf = true;
                return Bot.HasInGraveyard(CardId.Motorbike) || Bot.HasInGraveyard(CardId.IP);
            }
            return false;
        }
        private bool RegulusFunction()
        {
            if (Card.Location == CardLocation.Hand)
            {
                int tributeId = -1;
                if (Bot.HasInGraveyard(CardId.Soulpiercer))
                {tributeId = CardId.Soulpiercer;}
                else if (Bot.HasInGraveyard(CardId.Motorbike))
                {tributeId = CardId.Motorbike;}
                AI.SelectCard(tributeId);
                activate_Genius = true;
                return true;
            }
            else if (Duel.LastChainPlayer == 1)
            {
                return true;
            }
            return false;
        }
        private bool FleurFunction()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Fleur, 0))
            {
                ClientCard card = Util.GetProblematicEnemyMonster(0, true);
                if (card != null)
                {
                    AI.SelectCard(card);
                    return true;
                }
                card = Util.GetBestEnemySpell(true);
                if (card != null)
                {
                    AI.SelectCard(card);
                    return true;
                }
                List<ClientCard> cards = GetZoneCards(CardLocation.Onfield, Enemy);
                cards = cards.Where(tcard => tcard != null && !tcard.IsShouldNotBeTarget()).ToList();
                if (cards.Count <= 0) return false;
                AI.SelectCard(cards);
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.Fleur, 1))
            {
                return Duel.LastChainPlayer == 1;
            }
            return false;
        }
        private bool IPLinkFunction()
        {
            List<ClientCard> materials = GetZoneCards(CardLocation.MonsterZone,Bot).Where(card => card != null && card.IsFaceup() && card.Id != CardId.Scarecrow && (card.Id != CardId.Elf || (card.Id == CardId.Elf && !summon_Elf)) && !FinalCards(card.Id)).ToList();
            if (materials.Count <=1) return false;
            if (Bot.MonsterZone[6] != null && Bot.MonsterZone[6].Controller == 0 && Bot.MonsterZone[6].HasType(CardType.Link))
            {
                if (Bot.MonsterZone[2] != null && FinalCards(Bot.MonsterZone[2].Id) && Bot.MonsterZone[4] != null && FinalCards(Bot.MonsterZone[4].Id))
                    return false;
            }
            else if (Bot.MonsterZone[5] != null && Bot.MonsterZone[5].Controller == 0 && Bot.MonsterZone[5].HasType(CardType.Link))
            {
                if (Bot.MonsterZone[2] != null && FinalCards(Bot.MonsterZone[2].Id) && Bot.MonsterZone[0] != null && FinalCards(Bot.MonsterZone[0].Id))
                    return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }
        private bool PSYFunction()
        {
            activate_PSY = true;
            return true;
        }
        private bool IPFunction()
        {
            if (Duel.LastChainPlayer == 0) return false;
            if (Bot.HasInExtra(CardId.Unicorn))
            {
                List<ClientCard> material = new List<ClientCard>();
                List<ClientCard> cards = GetZoneCards(CardLocation.MonsterZone,Bot).Where(card => card != null && card != Card && card.IsFaceup() && !FinalCards(card.Id) && card.Id != CardId.IP && card.Id != CardId.Scarecrow).ToList();
                List<ClientCard> Enemycards = GetZoneCards(CardLocation.MonsterZone,Enemy);
                if (activate_Sarutobi) Enemycards = GetZoneCards(CardLocation.Onfield,Enemy);
                if (Bot.Hand.Count == 0 || Enemycards.Count(card => card != null && !card.IsShouldNotBeTarget()) == 0 || cards.Count == 0) return false;
                bool linkchk = false;
                foreach (var card in cards)
                {
                    if (card != null && (card.Id != CardId.Elf || (card.Id == CardId.Elf && !summon_Elf)))
                    {
                        material.Add(card);
                        linkchk = true;
                        break;
                    }
                }
                AI.SelectCard(CardId.Unicorn);
                material.Insert(0,Card);
                AI.SelectMaterials(material);
                if (!to_deck) to_deck = true;
                return linkchk;
            }
            return false;
        }
        private bool SarutobiFunction()
        {
            List<ClientCard> Enemycards = GetZoneCards(CardLocation.SpellZone,Enemy);
            if (Enemycards.Count(card => card != null && !card.IsShouldNotBeTarget()) == 0 || to_deck) return false;
            AI.SelectCard(Enemycards);
            activate_Sarutobi = true;
            return true;
        }
    }
}
