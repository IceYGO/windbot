using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using static WindBot.Game.AI.Decks.TimeThiefExecutor;

namespace WindBot.Game.AI.Decks
{
    [Deck("AutoChess", "AI_Test", "Test")]
    public class AutoChessExecutor : DefaultExecutor
    {
        public AutoChessExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {

            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.Activate, ActivateFunction);

            AddExecutor(ExecutorType.Summon, MonsterSummon);
            AddExecutor(ExecutorType.MonsterSet, MonsterSet);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, MonsterRepos);

            AddExecutor(ExecutorType.Activate, _CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, _CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, _CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, _CardId.CallOfTheHaunted, DefaultCallOfTheHaunted);
            AddExecutor(ExecutorType.Activate, _CardId.Scapegoat, DefaultScapegoat);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, _CardId.GhostBelle, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, _CardId.BreakthroughSkill, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, _CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, _CardId.HeavyStorm, DefaultHeavyStorm);
            AddExecutor(ExecutorType.Activate, _CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, _CardId.HammerShot, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, _CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, _CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, _CardId.SmashingGround, DefaultSmashingGround);
            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires, DefaultPotOfDesires);
            AddExecutor(ExecutorType.Activate, _CardId.AllureofDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, _CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, _CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);

            AddExecutor(ExecutorType.SpSummon, _CardId.JizukirutheStarDestroyingKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GadarlatheMysteryDustKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.RadiantheMultidimensionalKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.KumongoustheStickyStringKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.ThunderKingtheLightningstrikeKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.DogorantheMadFlameKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.SuperAntiKaijuWarMachineMechaDogoran, DefaultKaijuSpsummon);

            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);

            AddExecutor(ExecutorType.Summon, _CardId.SandaionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.GabrionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.MichionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.ZaphionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.HailonTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.RaphionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.SadionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.MetaionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.KamionTheTimelord, DefaultTimelordSummon);
            AddExecutor(ExecutorType.Summon, _CardId.LazionTheTimelord, DefaultTimelordSummon);

            AddExecutor(ExecutorType.Summon, _CardId.LeftArmofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.RightLegofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.LeftLegofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.RightArmofTheForbiddenOne, JustDontIt);
            AddExecutor(ExecutorType.Summon, _CardId.ExodiaTheForbiddenOne, JustDontIt);
        }

        private List<int> HintMsgForEnemy = new List<int>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Disable
        };

        private List<int> HintMsgForDeck = new List<int>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.Remove, HintMsg.AddToHand, HintMsg.FusionMaterial
        };

        private List<int> HintMsgForSelf = new List<int>
        {
            HintMsg.Equip
        };

        private List<int> HintMsgForMaterial = new List<int>
        {
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Release
        };

        private List<int> HintMsgForMaxSelect = new List<int>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.AddToHand, HintMsg.FusionMaterial, HintMsg.Destroy
        };

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, int hint, bool cancelable)
        {
            if (Duel.Phase == DuelPhase.BattleStart)
                return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>(_cards);
            if (max > cards.Count)
                max = cards.Count;

            if (HintMsgForEnemy.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Program.Rand.Next(enemyCards.Count)];
                    selected.Add(card);
                    enemyCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForDeck.Contains(hint))
            {
                IList<ClientCard> deckCards = cards.Where(card => card.Location == CardLocation.Deck).ToList();

                // select deck's card first
                while (deckCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = deckCards[Program.Rand.Next(deckCards.Count)];
                    selected.Add(card);
                    deckCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForSelf.Contains(hint))
            {
                IList<ClientCard> botCards = cards.Where(card => card.Controller == 0).ToList();

                // select bot's card first
                while (botCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = botCards[Program.Rand.Next(botCards.Count)];
                    selected.Add(card);
                    botCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForMaterial.Contains(hint))
            {
                IList<ClientCard> materials = cards.OrderBy(card => card.Attack).ToList();

                // select low attack first
                while (materials.Count > 0 && selected.Count < min)
                {
                    ClientCard card = materials[0];
                    selected.Add(card);
                    materials.Remove(card);
                    cards.Remove(card);
                }
            }

            // select random cards
            while (selected.Count < min)
            {
                ClientCard card = cards[Program.Rand.Next(cards.Count)];
                selected.Add(card);
                cards.Remove(card);
            }

            if (HintMsgForMaxSelect.Contains(hint))
            {
                // select max cards
                while (selected.Count < max)
                {
                    ClientCard card = cards[Program.Rand.Next(cards.Count)];
                    selected.Add(card);
                    cards.Remove(card);
                }
            }
            if (hint == HintMsg.SpSummon && max == 1)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(60461804))
                        return new List<ClientCard>(new[] { card });
                }
            }

            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            return Program.Rand.Next(options.Count);
        }

        private bool ActivateFunction()
        {
            if (Card.Id == 60461804)
            {
                if (Card.Location == CardLocation.Grave)
                {
                    return true;
                }

                ClientCard target = Util.GetProblematicEnemyCard(2500);
                if (target != null && !Util.ChainContainPlayer(0))
                {
                    AI.SelectCard(Card);
                    AI.SelectNextCard(target);
                    return true;
                }

                return false;
            }
            return DefaultDontChainMyself();
        }


        private bool JustDontIt()
        {
            return false;
        }
        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (BlackmailAttackerSunmmon(cardId))
                    return CardPosition.FaceUpAttack;
                if (cardData.Attack < 0)
                    return CardPosition.FaceUpAttack;
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            int atk = attacker.Attack;
            List<ClientCard> cards = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup()).ToList();
            List<ClientCard> cards1 = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup() && ((defender.IsAttack() && defender.Attack >= atk) || (!defender.IsAttack() && defender.GetDefensePower() >= atk))).ToList();
            List<ClientCard> cards2 = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup() && defender.IsAttack() && defender.Attack < atk).ToList();
            cards2.Sort(CardContainer.CompareCardAttack);
            List<ClientCard> cards3 = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup() && !defender.IsAttack() && defender.GetDefensePower() < atk).ToList();
            List<ClientCard> cards4 = defenders.Where(defender => defender != null && defender.IsFacedown()).ToList();
            List<ClientCard> cards5 = defenders.Where(defender => defender != null && BlackmailAttacker(defender, Enemy) && defender.IsFaceup()).ToList();
            if (BlackmailAttacker(attacker, Bot))
            {
                if (attacker.CanDirectAttack)
                    return AI.Attack(attacker, null);

                if (cards1.Count() > 0 && cards2.Count() > 0)
                {
                    int dam1 = -1;
                    int dam2 = cards2[0].Attack;
                    if (cards1[0].IsAttack())
                        dam1 = cards1[0].Attack;
                    else
                        dam1 = cards1[0].GetDefensePower();

                    if (dam1 - atk > atk - dam2)
                        return AI.Attack(attacker, cards1[0]);
                    else
                        return AI.Attack(attacker, cards2[0]);
                }
                else if (cards1.Count() > 0)
                    return AI.Attack(attacker, cards1[0]);
                else if (cards2.Count() > 0)
                {
                    return AI.Attack(attacker, cards2[0]);
                }
                else if (cards4.Count() > 0)
                    return AI.Attack(attacker, cards4[0]);
                else if (cards3.Count() > 0)
                    return AI.Attack(attacker, cards3[0]);
                else if (cards5.Count() > 0)
                    return base.OnSelectAttackTarget(attacker, cards5);
            }

            if (cards5.Count() > 0 && cards5.Any(defender => defender != null && defender.HasSetcode(0x10)))
                {
                    List<ClientCard> scards = defenders.Where(defender => defender != null && defender.Id == 69058960 && !defender.IsDisabled() && defender.IsFaceup()).ToList();
                    return base.OnSelectAttackTarget(attacker, scards);
                }
            else if (cards2.Count() > 0)
                return base.OnSelectAttackTarget(attacker, cards2);
            else if (cards4.Count() > 0)
                return AI.Attack(attacker, cards4[0]);
            else if (cards3.Count() > 0)
                return base.OnSelectAttackTarget(attacker, cards3);
            return null;
        }

        private bool BlackmailAttacker(ClientCard card, ClientField player)
        {
            if (!card.IsDisabled())
            {
                if (card.HasSetcode(0x4a) || card.Id == 34031284 || card.Id == 35494087 || card.Id == 54366836 || card.Id == 94004268 || card.Id == 97403510
                    || (card.HasSetcode(0x18d) && card.EquipCards.Count()>0)
                    || (card.Id == 59627393 && card.Overlays.Count()>0)
                    || (card.Id == 93730230 && card.Overlays.Count()>0)
                    || (card.Id == 69058960 && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 95442074 && scard.IsFaceup()))
                    || (card.Id == 95442074 && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 69058960 && scard.IsFaceup()))
                )
                    return true;
            }

            if (card.IsFaceup())
            {
                if ((card.HasSetcode(0x1a5) && GetZoneCards(CardLocation.SpellZone, player).Any(scard => scard != null && scard.Id == 65261141 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.HasSetcode(0x2a) && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 17285476 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.Id == 24874631 && GetZoneCards(CardLocation.SpellZone, player).Any(scard => scard != null && scard.Id == 24874630 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.HasSetcode(0x10) && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 29552709 && scard.IsFaceup() && !scard.IsDisabled()))
                )
                    return true;
            }

            return false;
        }

        private bool BlackmailAttackerSunmmon(int cardId)
        {
            YGOSharp.OCGWrapper.NamedCard card = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            int[] cardsname = new[] {34031284, 35494087, 54366836, 94004268, 97403510, 59627393, 93730230, 69058960, 95442074, 24874631};
            foreach(int cardname in cardsname)
            {
                if (cardId == cardname) return true;
            }

            if ((card.HasSetcode(0x10) && GetZoneCards(CardLocation.MonsterZone, Bot).Any(scard => scard != null && scard.Id == 29552709 && scard.IsFaceup() && !scard.IsDisabled()) && Duel.Player == 0) || card.HasSetcode(0x2a) || card.HasSetcode(0x1a5) || card.HasSetcode(0x18d) || card.HasSetcode(0x4a))
                return true;

            return false;
        }
        private bool DontSummon(int cardId)
        {
            YGOSharp.OCGWrapper.NamedCard card = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (card.HasSetcode(0x40) || card.HasSetcode(0xa4)) return true;
            int[] cardsname = new[] {74762582, 90179822, 16759958, 26964762, 42352091, 2511, 74018812, 76214441, 62886670, 69105797, 32391566, 94076521, 73625877, 1980574, 42090294, 68823957, 34976176, 89785779, 76133574, 3248469
            , 57647597, 37961969, 51993760, 87988305, 38339996, 37629703, 58131925, 71133680, 42790071, 34475451, 63009228, 24725825, 48427163, 86028783, 51852507, 29280589, 87462901, 73640163, 68120130, 84813516, 55461064, 59042331, 26775203
            , 67750322, 68819554, 26084285, 15613529, 19096726, 59546797, 12235475, 38695361, 37742478, 26914168, 43534808, 13313278, 99581584, 04192696, 89662736, 81109178, 18444902, 04807253, 12423762, 72318602, 86613346, 82489470, 16223761, 08152834/*时尚小垃圾*/
            , 97268402/*效果遮蒙者*/, 24508238/*D.D.乌鸦*/, 94145021/*锁鸟*/
            , 14558127, 14558128, 52038441, 52038442, 59438930, 59438931, 60643553, 60643554, 62015408, 62015409, 73642296, 73642297/*手坑六姐妹*/
            , 15721123, 23434538, 25137581, 46502744, 80978111, 87170768, 94081496/*xx的G*/
            , 17266660, 21074344, 94689635/*byd原来宣告者没字段啊*/
            , 20450925, 19665973, 28427869, 27352108/*攻宣坑*/
            };
            foreach(int cardname in cardsname)
            {
                if (cardId == cardname) return true;
            }

            return false;
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
        private bool MonsterRepos()
        {
            if (Duel.Phase == DuelPhase.Main1 && Card.HasType(CardType.Flip) && Card.IsFacedown())
            {
                return true;
            }
            else if (Duel.Phase == DuelPhase.Main2)
            {
                if (BlackmailAttacker(Card, Bot))
                {
                    if (Card.IsFaceup() && Card.IsAttack())
                        return false;
                    if (Card.IsFaceup() && Card.IsDefense())
                        return true;
                }
                return DefaultMonsterRepos();
            }

            return false;
        }
        private bool MonsterSummon()
        {
            if (BlackmailAttackerSunmmon(Card.Id))
                return DefaultMonsterSummon();
            else if (Card.HasType(CardType.Flip))
                return false;
            else if (Card.Level > 4)
                return DefaultMonsterSummon();
            else if (Bot.LifePoints > 1500)
            {
                if (DontSummon(Card.Id))
                    return false;
                else
                    return DefaultMonsterSummon();
            }
            else if (Bot.LifePoints <= 1500 && GetZoneCards(CardLocation.MonsterZone, Enemy).Count(card => card != null && card.Attack < Card.Attack))
            return false;
        }
        private bool MonsterSet()
        {
            if (Card.HasType(CardType.Flip))
                return DefaultMonsterSummon();
            if (card.HasSetcode(0x40)) return false;
            return DefaultMonsterSummon() && (Bot.LifePoints <= 1500 || Card.HasType(CardType.Flip) || GetZoneCards(CardLocation.MonsterZone, Bot).Count() == 0 || GetZoneCards(CardLocation.MonsterZone, Bot).Count() <= GetZoneCards(CardLocation.MonsterZone, Enemy).Count());
        }
    }
} 