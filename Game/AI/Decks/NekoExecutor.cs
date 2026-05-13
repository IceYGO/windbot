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
    [Deck("Neko", "AI_Neko")]
    public class NekoExecutor : DefaultExecutor
    {
        public class SetCode
        {
            public const int Neko = 0x1ca;
        }
        public class CardId
        {
            public const int LinkSpider = 98978921;
            public const int MaxxG = 23434538; //增殖的G
            public const int Mulcharmy_Fuwalos = 42141493; //欢聚友伴·茸茸长尾山雀
            public const int Mulcharmy_Purulia = 84192580; //欢聚友伴·抖抖海月水母
            public const int Infinite_Impermanence = 10045474; //无限泡影
            public const int EffectVeiler = 97268402;
            public const int AshBlossom = 14558127; //灰流丽
            public const int CalledbytheGrave = 24224830; //墓穴的指名
            public const int Dominus_Purge = 97045737; //圣王的粉碎
            public const int One_for_One = 2295440; //一对一
            public const int Linkuriboh = 41999284;
            public const int Meteorite = 27204311;
            public const int Obedience_Schooled = 72537897;
            public const int Gravity_Controller = 23656668;
            public const int Shamisen_Samsara_Sorrowcat = 46057733;
            public const int Herald_of_the_Arc_Light = 79606837;
            public const int Neko_Cookie = 68810435;
            public const int Neko_Marshmallow = 10966439;
            public const int Neko_Lollipop = 4215180;
            public const int Neko_Cake = 31425736;
            public const int Neko_Link = 30581601;
            public const int Neko_Field = 66975205;
            public const int Neko_Field_II = 93360904;
            public const int Neko_Quick = 29369059;
            public const int Neko_Sycro_Cookie = 67098897;
            public const int Neko_Sycro_Lollipop = 93192592;
            public const int Neko_Sycro_Cake = 31603289;
        }
        public class CardCount
        {
            public int Summon = 0;
            public List<int> Activate = new List<int>();
            public List<int> SPSummon = new List<int>();
            public List<int> Position = new List<int>();
            public bool BeastOnly = false;
            public void Clear()
            {
                Position.Clear();
                SPSummon.Clear();
                Activate.Clear();
                BeastOnly = false;
                Summon = 0;
            }
            public void SetBeastOnly()
            {
                BeastOnly = true;
            }
            public bool ChkBeastOnly()
            {
                return BeastOnly;
            }
            public void AddSummon()
            {
                Summon = 1;
            }
            public void AddSpSummon(int id)
            {
                SPSummon.Add(id);
            }
            public void AddActivate(int id)
            {
                Activate.Add(id);
            }
            public bool CheckActivate(int id)
            {
                return !Activate.Contains(id);
            }
            public bool CheckSpSummon(int id)
            {
                return !SPSummon.Contains(id);
            }
            public bool CheckPosition(int id)
            {
                return !Position.Contains(id);
            }
            public void AddPosition(int id)
            {
                Position.Add(id);
            }
            public bool CheckSummon()
            {
                return Summon == 0;
            }
        }
        private CardCount Count = new CardCount();
        private bool Activate_Neko_Quick = false;
        public NekoExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.Activate, CardId.Meteorite, EffectMeteorite);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.Infinite_Impermanence, EffectInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.Dominus_Purge, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.CalledbytheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.Mulcharmy_Purulia, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.Mulcharmy_Fuwalos, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.MaxxG, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.Herald_of_the_Arc_Light, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.SpSummon, CardId.Gravity_Controller);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Link, SpNekoLinkII);
            AddExecutor(ExecutorType.Activate, CardId.Obedience_Schooled, ActivateObedienceSchooled);
            AddExecutor(ExecutorType.Activate, CardId.One_for_One, Summon);
            AddExecutor(ExecutorType.Activate, Activate);
            AddExecutor(ExecutorType.Activate, CardId.Linkuriboh, ActivateLinkuriboh);
            AddExecutor(ExecutorType.Activate, CardId.Shamisen_Samsara_Sorrowcat, ActivateShamisenSamsaraSorrowcat);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Link, ActivateNekoLink);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Field, ActivateNekoField);
            AddExecutor(ExecutorType.SpSummon, CardId.Herald_of_the_Arc_Light, SPHeraldoftheArcLight);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Quick, ActivateNekoQuick);
            AddExecutor(ExecutorType.SpSummon, CardId.Shamisen_Samsara_Sorrowcat, SPShamisenSamsaraSorrowcat);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Cake, SPNekoSycroCake);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Lollipop, SPNekoSycroLollipop);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Cookie, SPNekoSycroCookie);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Marshmallow, ActivateNekoMarshmallowII);
            AddExecutor(ExecutorType.Summon, CardId.Neko_Cake, Summon);
            AddExecutor(ExecutorType.Summon, CardId.Neko_Cookie, Summon);
            AddExecutor(ExecutorType.Summon, CardId.Neko_Lollipop, Summon);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Marshmallow, ActivateNekoMarshmallow);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Cake, SpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Cookie, SpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Lollipop, SpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Link, SpNekoLink);
            AddExecutor(ExecutorType.SpellSet, CardId.Neko_Quick);
            AddExecutor(ExecutorType.SpellSet, CardId.Infinite_Impermanence);
            AddExecutor(ExecutorType.SpellSet, CardId.Dominus_Purge);
            AddExecutor(ExecutorType.SpellSet, CardId.CalledbytheGrave);
            AddExecutor(ExecutorType.SpSummon, CardId.Linkuriboh, SPLinkuriboh);
            AddExecutor(ExecutorType.Activate, CardId.Neko_Field_II, ActivateNekoFieldII);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Cake, SPSycro);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Lollipop, SPSycro);
            AddExecutor(ExecutorType.SpSummon, CardId.Neko_Sycro_Cookie, SPSycro);
        }
        public override void OnNewTurn()
        {
            Count.Clear();
            base.OnNewTurn();
        }
        private bool MonsterRepos()
        {
            if (!Enemy.GetMonsters().Any(i => i.IsDefense())
                && Util.GetTotalAttackingMonsterAttack(0) + Card.Attack >= Enemy.LifePoints + Util.GetTotalAttackingMonsterAttack(1)
                && Card.IsDefense()
                && !Count.ChkBeastOnly()
            )
                return true;
            return Card.IsFacedown();
        }
        private bool GoToBattlePhase()
        {
            if (!Enemy.GetMonsters().Any(i => i.IsDefense()))
            {
                if (Util.GetTotalAttackingMonsterAttack(0) >= Enemy.LifePoints + Util.GetTotalAttackingMonsterAttack(1)
                && !Count.ChkBeastOnly())
                {
                    return true;
                }
            }
            return false;
        }
        public override void OnChainEnd()
        {
            Activate_Neko_Quick = false;
            base.OnChainEnd();
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (Duel.Turn < 1)
                return CardPosition.FaceUpDefence;
            if (Enemy.GetMonsterCount() == 0)
                return CardPosition.FaceUpAttack;
            if (cardId == CardId.Herald_of_the_Arc_Light)
                return CardPosition.FaceUpDefence;
            if (Bot.HasInSpellZone(CardId.Neko_Field) || (Bot.HasInHand(CardId.Neko_Field) && ActivateNekoField()
                && new[]
                    {
                        CardId.Neko_Cake,
                        CardId.Neko_Cookie,
                        CardId.Neko_Marshmallow,
                        CardId.Neko_Lollipop
                    }.Contains(cardId))
            )
                return CardPosition.FaceUpAttack;
            return base.OnSelectPosition(cardId, positions);
        }
        public override int OnSelectOption(IList<int> options)
        {
            for (int idx = 0; idx < options.Count(); ++ idx)
            {
                int option = options[idx];
                if (option == Util.GetStringId(CardId.Neko_Marshmallow, Duel.Player + 1))
                    return idx;
                
                if (option == Util.GetStringId(CardId.Neko_Quick, 1) && (Duel.Player == 1 || Duel.LastChainPlayer == 1))
                {
                    Activate_Neko_Quick = true;
                    Count.AddActivate(CardId.Neko_Quick);
                    return idx;
                }
                else if (option == Util.GetStringId(CardId.Neko_Quick, 2))
                {
                    Count.AddActivate(CardId.Neko_Quick + 1);
                    return idx;
                }
            }
            return base.OnSelectOption(options);
        }
        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.Neko_Cake, 2))
                return Bot.Hand.Any(i => i.HasSetcode(SetCode.Neko) && !i.HasType(CardType.Field));
            else if (desc == Util.GetStringId(CardId.Neko_Lollipop, 2)
                || desc == Util.GetStringId(CardId.Neko_Cookie, 2))
                return true;
            else if (desc == Util.GetStringId(CardId.Neko_Marshmallow, 2))
            {
                if (Duel.Player == 0 && (!Bot.HasInGraveyard(CardId.Neko_Quick) || (Bot.HasInHand(CardId.Neko_Quick) && Count.CheckActivate(CardId.Neko_Quick)))
                    && (!Count.CheckActivate(CardId.Neko_Field)
                    || Bot.GetSpells().Any(i => i.IsCode(CardId.Neko_Field)))
                )
                    return false;
                else if (Duel.Player == 1 && Bot.GetSpells().Any(i => i.IsCode(CardId.Neko_Field_II)))
                    return false;
                return true;
            }

            return base.OnSelectYesNo(desc);
        }
        public override int OnSelectPlace(int cardId, int player, CardLocation location, int available)
        {
            if (player == 0 && location == CardLocation.MonsterZone)
            {
                if (cardId == CardId.Shamisen_Samsara_Sorrowcat && Bot.HasInExtra(CardId.Gravity_Controller) && Count.CheckActivate(cardId))
                {
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                }
                else if (cardId == CardId.Neko_Link)
                {
                    if ((Zones.z6 & available) > 0 && Bot.MonsterZone[3] == null) return Zones.z6;
                    if ((Zones.z5 & available) > 0 && Bot.MonsterZone[1] == null) return Zones.z5;
                }
                int[] cards = new[]
                    {
                        CardId.Neko_Sycro_Cake,
                        CardId.Neko_Sycro_Cookie,
                        CardId.Neko_Sycro_Lollipop
                    };
                if (Duel.CurrentChain.Any(i => i.Controller == 0 && i.IsCode(cards))
                    && Duel.GetCurrentSolvingChainCard() != null
                    && !Duel.GetCurrentSolvingChainCard().IsCode(cards)
                    && Bot.GetMonstersInMainZone().Count() > 3
                )
                {
                    if ((Zones.z6 & available) > 0) return Zones.z6;
                    if ((Zones.z5 & available) > 0) return Zones.z5;
                }
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            if (AI.HaveSelectedCards()) return null;
            ClientCard card = Duel.GetCurrentSolvingChainCard();
            if (card == null)
                card = Card;
            switch (hint)
            {
                case HintMsg.Discard:
                    {
                        IList<ClientCard> result = cards.GroupBy(i => i.Id)
                            .Where(g => g.Count() > 1)
                            .SelectMany(g => g.Skip(1))
                            .ToList()
                            .Concat(cards.Where(i => i.IsCode(new[]
                            {
                                CardId.MaxxG,
                                CardId.Mulcharmy_Fuwalos,
                                CardId.Infinite_Impermanence,
                                CardId.AshBlossom,
                                CardId.CalledbytheGrave,
                                CardId.Dominus_Purge
                            })).ToList())
                            .Concat(cards.Where(i => !Count.CheckActivate(i.Id) || !Count.CheckActivate(i.Id + 1)).ToList())
                            .ToList()
                            .ToList();
                        return Util.CheckSelectCount(result, cards, min, min);
                    }
                case HintMsg.LinkMaterial:
                case HintMsg.SynchroMaterial:
                    {
                        if (!cards.Any(i => !i.HasType(CardType.Link) && !i.HasType(CardType.Tuner)))
                        {
                            IList<ClientCard> result = cards.Where(i => Duel.ChainTargets.Contains(i))
                                .Concat(cards.Where(i => DefaultCheckWhetherCardIsNegated(i)))
                                .Concat(cards.Where(i => i.LinkCount == 1 && !i.HasSetcode(SetCode.Neko)))
                                .Concat(cards.Where(i => i.LinkCount == 1 && i.HasSetcode(SetCode.Neko)))
                                .ToList();
                            return Util.CheckSelectCount(result, cards, min, min);
                        }
                        else
                        {
                            IList<ClientCard> result = cards.Where(i => Duel.ChainTargets.Contains(i))
                                .Concat(cards.Where(i => DefaultCheckWhetherCardIsNegated(i))).ToList();
                            result = result.OrderBy(i => i.Level).ToList();
                            result = result
                                .Concat(cards.Where(i => !(i.HasType(CardType.Synchro) || i.HasType(CardType.Link))))
                                .ToList();
                            return Util.CheckSelectCount(result, cards, min, min);
                        }
                    }
                
                case HintMsg.Destroy:
                case HintMsg.Target:
                case HintMsg.ReturnToHand:
                    if (!cards.Any(i => i.Controller == 1))
                    {
                        if (Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && !DefaultCheckWhetherCardIsNegated(i)))
                        {
                            IList<ClientCard> result = cards.Where(i => DefaultCheckWhetherCardIsNegated(i) && i.Level == 1).ToList();
                            if (result.Count() == 0)
                                result = cards.Where(i => i.Level == 1).ToList();
                            if (result.Count() > 0)
                            {
                                result = result
                                    .Concat(cards.Where(i => i.LinkCount == 1 && !i.HasSetcode(SetCode.Neko)))
                                    .Concat(cards.Where(i => i.LinkCount == 1 && i.HasSetcode(SetCode.Neko)))
                                    .ToList();
                                return Util.CheckSelectCount(result, cards, min, min);
                            }
                                
                        }
                        {
                            IList<ClientCard> result = cards.Where(i => DefaultCheckWhetherCardIsNegated(i)).ToList();
                            result = result.OrderBy(i => i.Level).ToList();
                            result = result
                                .Concat(cards.Where(i => !(i.HasType(CardType.Synchro) || i.HasType(CardType.Link))))
                                .ToList();
                            if (result.Count() > 0)
                                return Util.CheckSelectCount(result, cards, min, min);
                        }
                    }
                    else
                    {
                        List<ClientCard> result = cards.Where(i => !Duel.ChainTargets.Contains(i) && i.Controller == 1)
                            .OrderByDescending(i => i.IsFaceup()).ToList();
                        if (result.Count() > 0)
                            return Util.CheckSelectCount(result, cards, Math.Min(min, result.Count()), max);
                        return Util.CheckSelectCount(cards.Where(i => i.Controller == 1).ToList(), cards, Math.Min(min, result.Count()), max);
                    }
                    break;
            }
            switch (card.Id)
            {
                case CardId.Neko_Cake:
                    if (cards.Any(i => i.IsCode(CardId.Neko_Quick)) && !Bot.HasInHand(CardId.Neko_Quick))
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Neko_Quick)).ToList(), cards, min, max);
                    if (cards.Any(i => i.HasType(CardType.Monster) && Count.CheckActivate(i.Id + 1)) && !Bot.Hand.Any(i => i.HasType(CardType.Monster) && i.HasSetcode(SetCode.Neko) && Count.CheckActivate(i.Id)))
                        return Util.CheckSelectCount(cards.Where(i => i.HasType(CardType.Monster) && Count.CheckActivate(i.Id + 1)).ToList(), cards, min, max);
                    if (cards.Any(i => i.IsCode(CardId.Neko_Field_II)) && !Bot.HasInHand(CardId.Neko_Field_II))
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Neko_Field_II)).ToList(), cards, min, max);
                    break;
                case CardId.Neko_Link:
                    if (hint == HintMsg.ToField)
                        return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Neko_Field)).ToList(), cards, min, max);
                    else
                    {
                        if (cards.Any(i => i.IsCode(CardId.Herald_of_the_Arc_Light))
                            && ((Bot.GetMonsters().Any(i => i.Level == 1 && i.HasSetcode(SetCode.Neko))
                                    && Bot.GetMonsters().Count(i => i.Level == 1) >= 3
                                    && Bot.GetMonsters().Any(i => i.Level == 2 && i.HasType(CardType.Tuner))
                                    )
                                || (Bot.GetMonsters().Any(i => i.Level == 1 && i.HasSetcode(SetCode.Neko))
                                    && Bot.GetMonsters().Count(i => i.Level == 1) >= 2
                                    && Bot.GetMonsters().Any(i => i.Level == 3 && i.HasType(CardType.Tuner))
                                    )
                                || (Bot.GetMonsters().Count(i => i.Level == 1) >= 2
                                    && Bot.GetMonsters().Any(i => i.Level == 2 && i.HasType(CardType.Tuner)
                                    && Bot.GetMonsters().Any(j => j != i && j.HasType(CardType.Synchro) && !DefaultCheckWhetherCardIsNegated(j)))
                                    )
                                )
                            )
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Herald_of_the_Arc_Light)).ToList(), cards, min, max);
                        if (Enemy.GetMonsters().Count(i => i.IsFaceup() && !i.HasType(CardType.Link)) >= 2 && Count.CheckActivate(CardId.Neko_Sycro_Cookie + 1))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Neko_Sycro_Cookie)).ToList(), cards, min, max);
                        List<ClientCard> result = cards.Where(i => Count.CheckActivate(i.Id + 1)).ToList();
                        if (cards.Any(i => i.IsCode(CardId.Neko_Sycro_Cookie)) && SPNekoSycroCookie() && Count.CheckActivate(CardId.Neko_Sycro_Cookie) && Count.CheckActivate(CardId.Neko_Sycro_Cookie + 1))
                            return Util.CheckSelectCount(cards.Where(i => i.IsCode(CardId.Neko_Sycro_Cookie)).ToList(), cards, min, max);
                        result = result.Where(i => Count.CheckActivate(i.Id + 1) && i.Level == 2)
                            .Concat(result.Where(i => i.IsCode(CardId.Neko_Sycro_Lollipop)))
                            .Concat(result.Where(i => i.IsCode(CardId.Neko_Sycro_Cake)))
                            .Concat(result.Where(i => i.Level == 2))
                            .ToList();
                        return Util.CheckSelectCount(result, cards, min, max);
                    }
                case CardId.Neko_Field:
                case CardId.Neko_Field_II:
                    if (hint == HintMsg.SpSummon)
                        return Util.CheckSelectCount(cards.Where(i => Count.CheckActivate(i.Id)).ToList(), cards, min, max);
                    else if (hint == HintMsg.ToDeck)
                        return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Removed).ToList(), cards, min, max);
                    break;
                case CardId.Neko_Sycro_Cookie:
                case CardId.Neko_Sycro_Cake:
                    if (!cards.Any(i => i.Location != CardLocation.Grave))
                    {
                        if (!Bot.HasInMonstersZone(CardId.Herald_of_the_Arc_Light)
                            && Bot.HasInMonstersZone(CardId.Neko_Link)
                            && ((Bot.GetMonsters().Any(i => i.HasType(CardType.Synchro) && i.HasSetcode(SetCode.Neko))
                                    && Bot.GetMonsters().Any(i => i.Level == 1 && i.HasSetcode(SetCode.Neko))
                                ) || Bot.GetMonsters().Any(i => i.HasType(CardType.Tuner) && i.Level == 3)
                            )
                        )
                            return Util.CheckSelectCount(cards.Where(i => i.Level == 1).ToList(), cards, max, max);
                        List<ClientCard> result = cards.Where(i => Count.CheckActivate(i.Id))
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Link)))
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Cake)))
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Marshmallow)))
                            .Concat(cards.Where(i => !i.IsCode(new [] {CardId.Neko_Marshmallow, CardId.Neko_Cake})))
                            .ToList();
                        return Util.CheckSelectCount(result.GroupBy(x => x.Id).Select(g => g.First()).ToList(), cards, max, max);
                    }
                    else if (hint == HintMsg.AddToHand)
                    {
                        List<ClientCard> result = cards.Where(i => Count.CheckActivate(i.Id)).ToList();
                        result = result.Where(i => i.IsCode(CardId.Neko_Marshmallow))
                            .Concat(result.Where(i => i.IsCode(CardId.Neko_Cake)))
                            .Concat(result.Where(i => !i.IsCode(new [] {CardId.Neko_Marshmallow, CardId.Neko_Cake})))
                            .ToList();
                        return Util.CheckSelectCount(result.GroupBy(x => x.Id).Select(g => g.First()).ToList(), cards, min, max);
                    }
                    else if (hint == HintMsg.PosChange)
                    {
                        List<ClientCard> result = cards.Where(i =>i.Controller == 1).ToList();
                        return Util.CheckSelectCount(result, cards, Math.Min(min, result.Count()), max);
                    }
                    break;
                case CardId.Neko_Quick:
                    if (hint == HintMsg.SpSummon)
                    {
                        if (cards.Any(i => i.Location == CardLocation.Grave && i.Id == CardId.Neko_Marshmallow))
                            return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Grave && i.Id == CardId.Neko_Marshmallow).ToList(), cards, min, max);
                        if (cards.Any(i => i.Location == CardLocation.Grave))
                            return Util.CheckSelectCount(cards.Where(i => i.Location == CardLocation.Grave).ToList(), cards, min, max);
                    }
                    break;
                case CardId.One_for_One:
                    {
                        List<ClientCard> result = cards.Where(i => Count.CheckActivate(i.Id) && i.HasSetcode(SetCode.Neko)).ToList();
                        result = result.Where(i => i.IsCode(CardId.Neko_Cake))
                            .Concat(result.Where(i => i.IsCode(CardId.Neko_Marshmallow)))
                            .Concat(result.Where(i => !i.IsCode(new [] {CardId.Neko_Marshmallow, CardId.Neko_Cake})))
                            .ToList();
                        return Util.CheckSelectCount(result, cards, min, max);
                    }
                case CardId.Neko_Sycro_Lollipop:
                    if (card.Location == CardLocation.MonsterZone)
                    {
                        List<ClientCard> result = new List<ClientCard>();
                        if (Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && !DefaultCheckWhetherCardIsNegated(i))
                            || (Duel.Player == 1 && cards.Count(i => i.IsCode(CardId.Neko_Link)) > 1 && Bot.GetMonsters().Any(i => i.HasType(CardType.Synchro) && i.HasSetcode(SetCode.Neko) && !DefaultCheckWhetherCardIsNegated(i)))
                        )
                            result = cards.Where(i => i.IsCode(CardId.Neko_Link)).ToList();
                        result = result
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Cake)))
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Marshmallow)))
                            .Concat(cards.Where(i => !i.HasType(CardType.Link) && !i.IsCode(new [] {CardId.Neko_Marshmallow, CardId.Neko_Cake})))
                            .Concat(cards.Where(i => i.HasType(CardType.Link)))
                            .ToList();
                        return Util.CheckSelectCount(result.GroupBy(x => x.Id).Select(g => g.First()).ToList(), cards, max, max);
                    }
                    else
                    {
                        List<ClientCard> result = cards.Where(i => Count.CheckActivate(i.Id))
                            .Concat(Duel.Player == 0 || Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && !DefaultCheckWhetherCardIsNegated(i))
                                ? cards.Where(i => i.IsCode(CardId.Neko_Link)) : new List<ClientCard>()
                            )
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Cake)))
                            .Concat(cards.Where(i => i.IsCode(CardId.Neko_Marshmallow)))
                            .Concat(cards.Where(i => !i.IsCode(new [] {CardId.Neko_Marshmallow, CardId.Neko_Cake})))
                            .ToList();
                        return Util.CheckSelectCount(result.GroupBy(x => x.Id).Select(g => g.First()).ToList(), cards, max, max);
                    }

            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }
        private bool ActivateNekoLink()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (Duel.CurrentChain.Any(i => i.Controller == 0 && i.IsCode(new[] {CardId.Linkuriboh, CardId.Neko_Link}))
                && (LastChainCard == null || LastChainCard.Controller == 0)) return false;
            if (ActivateDescription == Util.GetStringId(CardId.Neko_Link, 1))// 同调召唤
            {
                if (Duel.Player == 1 && ActivateNekoQuick())
                    return false;
                if (Duel.CurrentChain.Any(i => i.IsCode(CardId.Neko_Quick)) && (Duel.Player == 1 || Activate_Neko_Quick))
                    return true;
                if (Bot.HasInExtra(CardId.Herald_of_the_Arc_Light)
                    && Duel.Player == 1
                    && ((Bot.GetMonsters().Any(i => i.Level == 1 && i.HasSetcode(SetCode.Neko))
                            && Bot.GetMonsters().Count(i => i.Level == 1) >= 3
                            && Bot.GetMonsters().Any(i => i.Level == 2 && i.HasType(CardType.Tuner))
                        )
                        || (Bot.GetMonsters().Any(i => i.Level == 1 && i.HasSetcode(SetCode.Neko))
                            && Bot.GetMonsters().Count(i => i.Level == 1) >= 2
                            && Bot.GetMonsters().Any(i => i.Level == 3 && i.HasType(CardType.Tuner))
                            )
                        || (Bot.GetMonsters().Count(i => i.Level == 1) >= 2
                            && Bot.GetMonsters().Any(i => i.Level == 2 && i.HasType(CardType.Tuner)
                                && Bot.GetMonsters().Any(j => j != i && j.HasType(CardType.Synchro) && !DefaultCheckWhetherCardIsNegated(j))))
                        ))
                    return true;
                if (Duel.Player == 1
                    && !Bot.GetMonsters().Any(i => i.HasType(CardType.Synchro) && i.HasSetcode(SetCode.Neko) && Count.CheckActivate(i.Id + 1))
                    && !Bot.ExtraDeck.Any(i => i.HasType(CardType.Synchro) && i.HasSetcode(SetCode.Neko) && Count.CheckActivate(i.Id + 1))
                )
                    return true;
                return ((
                        (Bot.HasInExtra(CardId.Neko_Sycro_Cake) && SPNekoSycroCake() && Count.CheckActivate(CardId.Neko_Sycro_Cake + 1))
                        || (Bot.HasInExtra(CardId.Neko_Sycro_Cookie) && SPNekoSycroCookie() && Count.CheckActivate(CardId.Neko_Sycro_Cookie + 1))
                        || (Bot.HasInExtra(CardId.Neko_Sycro_Lollipop) && SPNekoSycroLollipop() && Count.CheckActivate(CardId.Neko_Sycro_Lollipop + 1))
                    ) && Duel.Player == 1 && !Bot.GetMonsters().Any(i => i.HasType(CardType.Synchro) && i.HasSetcode(SetCode.Neko) && Count.CheckActivate(i.Id + 1)))
                    || Duel.LastChainTargets.Any(i => i.HasSetcode(SetCode.Neko) && !i.HasType(CardType.Synchro) && Bot.GetMonsters().Contains(i));
            }
            if (Duel.Player == 0 && (!Count.CheckActivate(CardId.Neko_Field)
                || Bot.GetSpells().Any(i => i.IsCode(CardId.Neko_Field))))
                return false;
            else if (Duel.Player == 1 && Bot.GetSpells().Any(i => i.IsCode(CardId.Neko_Field_II)))
                return false;
            // 放置场地魔法
            Count.AddActivate(CardId.Neko_Link);
            return true;
        }
        private bool SpNekoLinkII()
        {
            if (Bot.HasInMonstersZone(new[] {
                CardId.Neko_Sycro_Cake,
                CardId.Neko_Sycro_Cookie,
                CardId.Neko_Sycro_Lollipop
            }))
                return false;
            return !Bot.GetMonsters().Any(i => i.LinkCount == 1) && !Bot.GetMonsters().Any(i => i.Level != 1);
        }
        private bool SpNekoLink()
        {
            return !Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && !DefaultCheckWhetherCardIsNegated(i))
                && (Count.CheckActivate(CardId.Neko_Link)
                    || Bot.GetMonsters().Count(i => i.IsCode(CardId.Neko_Link)) > 2
                        || Bot.GetMonsters().Any(i => 
                                i.IsCode(CardId.Neko_Link) && i.HasType(CardType.Synchro)
                                    && Bot.GetMonsters().Any(j => j != i && j.IsCode(CardId.Neko_Link))
                            )
                        );
        }
        private bool ActivateNekoField()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
                return Count.CheckActivate(Card.Id);
            else if (Card.Location == CardLocation.SpellZone)
                Count.AddActivate(CardId.Neko_Field);
            else if (Card.Location == CardLocation.Grave)
                return !Count.CheckActivate(CardId.Neko_Sycro_Lollipop);
            return true;
        }
        private bool ActivateNekoQuick()
        {
            if (Duel.CurrentChain.Any(i => i.IsCode(Card.Id)))
                return false;
            if (Duel.LastChainPlayer == 0 && !Duel.LastChainTargets.Any(i => i.HasSetcode(SetCode.Neko)
                && Bot.GetMonsters().Contains(i)))
                return false;
            if (Duel.CurrentChain.Any(i => i.IsCode(CardId.Neko_Link))
                && Bot.GetMonsters().Count(i => i.HasSetcode(SetCode.Neko)) < 4
            )
                return false;
            else if (Duel.LastChainPlayer == 1 && Duel.LastChainTargets.Any(i => i.HasSetcode(SetCode.Neko)
                && Bot.GetMonsters().Contains(i)) && Enemy.GetFieldCount() >= 2 && Count.CheckActivate(Card.Id + 1))
                return true;
            if (Duel.Player == 0
                && Count.CheckActivate(Card.Id)
                && ((Bot.HasInGraveyard(CardId.Neko_Marshmallow)
                    && Count.CheckActivate(CardId.Neko_Marshmallow)
                ) || (
                    Bot.GetMonsters().Any(i => i.LinkCount == 1)
                    && !Bot.GetMonsters().Any(i => i.Level == 1)
                    && !Bot.Hand.Any(i => i.HasSetcode(SetCode.Neko) && i.HasType(CardType.Monster) && Count.CheckActivate(i.Id + 1))
                    && Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Neko) && i.Level == 1)
                )))
                return true;
            if (Duel.Player == 1
                && Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && !DefaultCheckWhetherCardIsNegated(i))
                && Bot.GetMonsters().Any(i => i.Level == 1)
                && Bot.GetMonsters().Count(i => (i.Level == 1 || i.LinkCount == 1) && i.HasAttribute(CardAttribute.Light) && i.HasRace(CardRace.Beast)) >= 2
                && Enemy.GetMonsters().Count(i => i.IsFaceup()) + Enemy.GetSpellCount() >= 2
                && Count.CheckActivate(Card.Id + 1)
            )
                return true;
            return false;
        }
        private bool ActivateNekoFieldII()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (Card.Location == CardLocation.Hand)
                return !Count.CheckActivate(CardId.Neko_Field) || (
                        !Count.CheckActivate(CardId.Neko_Link)
                        && !Bot.HasInHand(CardId.Neko_Field)
                        && !Bot.HasInSpellZone(CardId.Neko_Field)
                    );
            else if (Card.Location == CardLocation.Grave)
                return !Count.CheckActivate(CardId.Neko_Sycro_Lollipop);
            return true;
        }
        private bool ActivateNekoMarshmallow()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIdIsNegated(Card.Id)
                    || Bot.GetMonsterCount() >= 3) return false;
                Count.AddActivate(CardId.Neko_Marshmallow + 1);
                return true;
            }
            else
            {
                if (DefaultCheckWhetherCardIsNegated(Card)) return false;
                Count.AddActivate(CardId.Neko_Marshmallow);
                return true;
            }
        }
        private bool ActivateNekoMarshmallowII()
        {
            if (Card.Location == CardLocation.Hand)
            {
                if (DefaultCheckWhetherCardIdIsNegated(Card.Id)
                    || Bot.HasInGraveyard(Card.Id)) return false;
                if (Bot.HasInHand(CardId.Neko_Quick) && Count.CheckActivate(Card.Id)) return false;
                Count.AddActivate(CardId.Neko_Marshmallow + 1);
                return true;
            }
            return false;
        }
        private bool SPNekoSycroCake()
        {
            return Count.CheckActivate(Card.Id);
        }
        private bool SPNekoSycroLollipop()
        {
            return Count.CheckActivate(Card.Id) && Bot.Graveyard.Any(i => i.HasSetcode(SetCode.Neko) && !i.HasType(CardType.Link));
        }
        private bool SPNekoSycroCookie()
        {
            return Count.CheckActivate(Card.Id) && Enemy.GetMonsters().Count(i => i.IsFaceup() && !i.HasType(CardType.Link) && !Duel.ChainTargets.Contains(i)) > 1;
        }
        private bool SPSycro()
        {
            return !Bot.HasInMonstersZone(new[]{
                CardId.Neko_Sycro_Cake,
                CardId.Neko_Sycro_Cookie,
                CardId.Neko_Sycro_Lollipop
            }) && !(Bot.HasInMonstersZone(CardId.Neko_Link) && Bot.GetMonsters().Any(i => i.Level == 1));
        }
        private bool SpSummon()
        {
            if (Card.Location == CardLocation.Hand
                && ((Bot.GetMonsterCount() < 3
                    && Count.CheckActivate(Card.Id)
                ) || (!Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Neko) && i.HasType(CardType.Synchro))
                    && (
                        !Bot.GetMonsters().Any(i => i.Level == 1)
                        || !Bot.GetMonsters().Any(i => i.LinkCount == 1)
                    ))
                )
            )
            {
                Count.AddSpSummon(Card.Id);
                return true;
            }
            return false;
        }
        private bool Summon()
        {
            if ((Bot.GetMonsterCount() < 3 && Count.CheckActivate(Card.Id)) || (!Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Neko) && i.HasType(CardType.Synchro))
                    && (
                        !Bot.GetMonsters().Any(i => i.Level == 1)
                        || !Bot.GetMonsters().Any(i => i.LinkCount == 1)
                    )))
            {
                Count.AddSummon();
                return true;
            }
            return false;
        }
        private bool Activate()
        {
            if (DefaultCheckWhetherCardIsNegated(Card)) return false;
            if (new[]{
                CardId.Neko_Cake,
                CardId.Neko_Cookie,
                CardId.Neko_Lollipop
            }.Contains(Card.Id)){
                Count.AddActivate(Card.Id);
                return true;
            }
            if (!new[]{
                CardId.Neko_Sycro_Cake,
                CardId.Neko_Sycro_Cookie,
                CardId.Neko_Sycro_Lollipop
            }.Contains(Card.Id))
                return false;
            if (!Enemy.GetMonsters().Concat(Enemy.GetSpells()).Any(i => !Duel.ChainTargets.Contains(i)))
                return false;
            if (ActivateDescription == Util.GetStringId(Card.Id, 1))
            {
                if (Duel.LastChainTargets.Any(i => i.HasSetcode(SetCode.Neko)
                    && Bot.GetMonsters().Contains(i)) && Duel.LastChainTargets.Contains(Card))
                {
                    Count.AddActivate(Card.Id + 1);
                    return true;
                }
                if (Bot.Graveyard.Count(i => i.HasSetcode(SetCode.Neko) && i.HasType(CardType.Monster)) < 2
                    && !Bot.HasInMonstersZoneOrInGraveyard(CardId.Neko_Link)
                )
                    return false;
                if (Count.CheckActivate(CardId.Neko_Cake)
                    || Count.CheckActivate(CardId.Neko_Cookie)
                    || Count.CheckActivate(CardId.Neko_Lollipop)
                    || Count.CheckActivate(CardId.Neko_Marshmallow)
                    || Bot.HasInGraveyard(CardId.Neko_Link)
                    || Bot.GetMonsters().Any(i => i.IsCode(CardId.Neko_Link) && (!DefaultCheckWhetherCardIsNegated(i) || Duel.Player == 0)))
                {
                    Count.AddActivate(Card.Id + 1);
                    return true;
                }
                return false;
            }
            if (Card.IsCode(CardId.Neko_Sycro_Cookie) && !Enemy.GetMonsters().Any(i => i.IsFaceup() && !i.HasType(CardType.Link)))
                return false;
            Count.AddActivate(Card.Id);
            return true;
        }
        private bool ActivateLinkuriboh()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Duel.LastChainTargets.Any(i => i.Level == 0) && Duel.LastChainPlayer == 1)
                    return true;
                return Bot.GetMonsters().Count(i => i.Level == 1) > Bot.GetMonsters().Count(i => i.LinkCount == 1) + 1;
            }
            return false;
        }
        private bool SPShamisenSamsaraSorrowcat()
        {
            if (!Bot.HasInExtra(CardId.Herald_of_the_Arc_Light))
                return false;
            int[] cards = new[]
            {
                CardId.Neko_Sycro_Cake,
                CardId.Neko_Sycro_Cookie,
                CardId.Neko_Sycro_Lollipop
            };
            int ct = Bot.GetMonsters().Count(i => i.Level == 1);
            return (Bot.HasInMonstersZone(cards) || Bot.HasInGraveyard(cards))
                && (ct > 2
                    || (ct > 1 && Bot.HasInMonstersZone(CardId.Neko_Link))
                    || (Bot.Hand.Any(i => (Count.CheckSpSummon(i.Id) || Count.CheckSummon()) && i.HasType(CardType.Monster) && i.HasSetcode(SetCode.Neko))
                        && (ct > 1 || Bot.GetMonsters().Count(i => i.Level == 2 && i.HasType(CardType.Synchro)) > 1)
                    )
                );
        }
        private bool ActivateShamisenSamsaraSorrowcat()
        {
            if (Card.Location == CardLocation.Grave)
            {
                Count.AddActivate(Card.Id);
                return true;
            }
            return false;
        }
        private bool SPHeraldoftheArcLight()
        {
            if (!Bot.HasInMonstersZone(CardId.Shamisen_Samsara_Sorrowcat) && Bot.HasInExtra(CardId.Shamisen_Samsara_Sorrowcat))
                return false;
            if ((Bot.GetMonsters().Count(i => i.Level == 1) > 1
                && Bot.GetMonsters().Any(i => i.LinkCount == 1))
                || !Bot.GetMonsters().Any(i => i.HasSetcode(SetCode.Neko) && i.HasType(CardType.Synchro))
            )
                return true;
            return false;
        }
        private bool ActivateObedienceSchooled()
        {
            Count.SetBeastOnly();
            return true;
        }
        private bool EffectMeteorite()
        {
            return Bot.GetMonsterCount() == 0 || Enemy.GetMonsterCount() == Bot.GetMonsterCount() + 2;
        }
        private bool EffectInfiniteImpermanence()
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
        private bool SPLinkuriboh()
        {
            int[] cards = new[]
            {
                CardId.Neko_Sycro_Cake,
                CardId.Neko_Sycro_Cookie,
                CardId.Neko_Sycro_Lollipop
            };
            return Bot.GetMonsters().Count(i => i.Level == 1) > 1 || (
                Bot.HasInMonstersZone(CardId.Neko_Link) && (Bot.HasInMonstersZone(cards) || Bot.GetMonsters().Count(i => i.Level == 1) > 1)
            );
        }
    }
}