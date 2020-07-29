using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;

/* Translation Guide
 * Dragma = Dogmatika
 * Bastard = Titaniklad the Ash Dragon
 */

namespace WindBot.Game.AI.Decks
{
    [Deck("Dogmatika", "AI_Dogmatika")]
    class DragmaExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int InvokedAleister = 86120751;
            public const int InvokedInvocation = 74063034;
            public const int InvokedMeltdown = 47679935;
            public const int InvokedTerraforming = 73628505;
            public const int InvokedAlmiraj = 60303245;
            public const int InvokedGardna = 2220237;
            public const int InvokedMechaba = 75286621;
            public const int InvokedCaliga = 13529466;
            public const int InvokedTower = 97300502;

            public const int DragmaEcclesia = 60303688;
            public const int DragmaMaximus = 95679145;
            public const int DragmaFleur = 69680031;
            public const int DragmaNadir = 1984618;
            public const int DragmaPunish = 82956214;
            public const int DragmaBastard = 41373230;

            public const int ShaddollApkallone = 50907446;
            public const int ShaddollConstruct = 20366274;
            public const int ShaddollWinda = 94977269;
            public const int ShaddollRuq = 21011044;
            public const int ShaddollBeast = 3717252;

            public const int StapleAsh = 14558127;
            public const int StapleTalents = 25311006;
            public const int StapleCalled = 24224830;
            public const int StapleSuperPoly = 48130397;
            public const int StapleImperm = 10045474;
            public const int StapleJudgment = 41420027;
            public const int StapleVenom = 41209827;
            public const int StapleDragostapelia = 69946549;
            public const int StapleNtss = 80532587;
            public const int StapleOmega = 74586817;
            public const int StaplePegasus = 98506199;
        }

        public DragmaExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // priority 1 - interaction
            AddExecutor(ExecutorType.Activate, CardId.StapleAsh, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.StapleCalled, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.StapleImperm, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.StapleJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, CardId.StapleSuperPoly, SuperPolyEffect);
            AddExecutor(ExecutorType.Activate, CardId.InvokedMechaba, MechabaNegate);
            AddExecutor(ExecutorType.Activate, CardId.InvokedAleister, AleisterEffect);
            AddExecutor(ExecutorType.Activate, CardId.DragmaFleur, FleurSummon);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollRuq, RuqEffect);
            AddExecutor(ExecutorType.Activate, CardId.DragmaPunish, PunishEffect);
            AddExecutor(ExecutorType.Activate, CardId.StapleTalents, TalentsEffect);
            AddExecutor(ExecutorType.Activate, CardId.StapleDragostapelia, DragoEff);

            // priority 2 - primary combo (invoked)
            AddExecutor(ExecutorType.Activate, CardId.InvokedTerraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.InvokedMeltdown, MeltdownEffect);
            AddExecutor(ExecutorType.Summon, CardId.InvokedAleister, AleisterSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.InvokedAlmiraj, AlmirajSummon);
            // Aleister search handled above
            AddExecutor(ExecutorType.SpSummon, CardId.InvokedGardna, GardnaSummon);
            AddExecutor(ExecutorType.Activate, CardId.InvokedInvocation, InvocationEffect);

            // priority 3 - primary combo (dragma)
            AddExecutor(ExecutorType.Activate, CardId.DragmaNadir, NadirEffect);
            AddExecutor(ExecutorType.Summon, CardId.DragmaEcclesia, EcclesiaNormal);
            AddExecutor(ExecutorType.Activate, CardId.DragmaEcclesia, EcclesiaEffect);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollApkallone, ApkalloneSearch);
            AddExecutor(ExecutorType.Activate, CardId.DragmaMaximus, MaximusSummon);
            AddExecutor(ExecutorType.Activate, CardId.DragmaMaximus, MaximusMill);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollConstruct, ConstructRecover);
            AddExecutor(ExecutorType.Activate, CardId.DragmaBastard, BastardSearch);

            // priority 4 - misc. nadir/maximus targets
            AddExecutor(ExecutorType.Activate, CardId.StapleNtss, NtssPop);
            AddExecutor(ExecutorType.Activate, CardId.StapleOmega, OmegaRecur);
            AddExecutor(ExecutorType.Activate, CardId.StaplePegasus, PegasusSpin);

            // priority 5 - niche scenarios
            AddExecutor(ExecutorType.Activate, CardId.ShaddollBeast, BeastEffect);
            AddExecutor(ExecutorType.Activate, CardId.ShaddollConstruct, ConstructMill);
            AddExecutor(ExecutorType.Activate, CardId.StapleVenom, VenomEff);
            AddExecutor(ExecutorType.Activate, CardId.InvokedTower, TowerEff);

            // priority 6 - set cards
            AddExecutor(ExecutorType.SpellSet, CardId.ShaddollRuq, RuqSet);
            AddExecutor(ExecutorType.SpellSet, CardId.StapleCalled, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.StapleImperm, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.StapleSuperPoly, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.StapleJudgment, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.DragmaPunish, TrapSet);
        }

        private bool FleurAttackUsed;
        private bool RuqUsed;
        private bool BastardSentThisTurn;
        private bool MaximusUsed;
        private bool EcclesiaUsed;

        // generic actions
        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            // reset tracking variables
            FleurAttackUsed = false;
            BastardSentThisTurn = false;
            MaximusUsed = false;
            EcclesiaUsed = false;
            // Ruq use is not reset because we only expect to use it once
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable)
        {
            // select cards
            return null;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            //YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardId == CardId.DragmaMaximus && !Duel.MainPhase.CanBattlePhase)
                return CardPosition.FaceUpDefence;
            return 0;
        }

        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                return available & ~Bot.GetLinkedZones();
            }
            return 0;
        }

        // update stats for battle prediction based on effects
        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (!defender.IsMonsterHasPreventActivationEffectInBattle())
            {
                if (attacker.IsCode(CardId.ShaddollConstruct) && !attacker.IsDisabled() && defender.IsSpecialSummoned) // NOTE: Possible to check destruction immunity?
                    attacker.RealPower = 9999;
                if (attacker.IsCode(CardId.DragmaFleur) && !attacker.IsDisabled() && !FleurAttackUsed)
                    attacker.RealPower += 500;
                if (attacker.HasType(CardType.Fusion) && Bot.HasInHand(CardId.InvokedAleister))
                    attacker.RealPower += 1000;
            }
            return base.OnPreBattleBetween(attacker, defender);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            // attack with Winda first because if summoned by Ruq it cannot attack directly
            ClientCard windaCard = attackers.GetFirstMatchingCard(card => card.IsCode(CardId.ShaddollWinda)); 
            if (!(windaCard == null) && defenders.IsExistingMatchingCard(card => card.Attack < windaCard.Attack))
            {
                return windaCard;
            }

            // attack with Construct first because if summoned by Ruq it cannot attack directly, and to use its effect
            ClientCard constructCard = attackers.GetFirstMatchingCard(card => card.IsCode(CardId.ShaddollConstruct));
            if (!(constructCard == null) && !constructCard.IsDisabled()
                && defenders.IsExistingMatchingCard(card => (card.IsSpecialSummoned && !card.IsMonsterHasPreventActivationEffectInBattle())
                || card.Attack < constructCard.Attack))
            {
                return constructCard;
            }

            // attack with Fleur-de-lis first to get attack buff on all Dragmas
            ClientCard fleurCard = attackers.GetFirstMatchingCard(card => card.IsCode(CardId.DragmaFleur));
            if (!(fleurCard == null))
            {
                if (defenders.IsExistingMatchingCard(card => card.Attack < fleurCard.RealPower))
                {
                    return fleurCard;
                }
            }

            // if caliga is restricting attacks, swing with the strongest first
            if (attackers.ContainsCardWithId(CardId.InvokedCaliga))
            {
                return Util.GetBestBotMonster();
            }

            return base.OnSelectAttacker(attackers, defenders);
        }

        // priority 1 - interaction

        private readonly int[] discardBlacklist =
            {
                CardId.InvokedAleister,
                CardId.InvokedInvocation,
                CardId.DragmaFleur
            };

        private bool SuperPolyEffect()
        {
            // check that we won't be discarding an important card to activate
            IList<ClientCard> list = Bot.Hand.GetMatchingCards(card => !card.IsCode(discardBlacklist));
            if (list.Count == 0)
            {
                return false;
            }
            AI.SelectCard(list);

            // make sure we're using opponent's materials
            List<ClientCard> enemyMonsters = Enemy.GetMonsters();

            // Dragostapelia = 1 Fusion + 1 DARK
            if (Bot.ExtraDeck.ContainsCardWithId(CardId.StapleDragostapelia)
                && enemyMonsters.IsExistingMatchingCard(card => card.HasType(CardType.Fusion))
                && (enemyMonsters.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Dark) && !card.HasType(CardType.Fusion))
                || enemyMonsters.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Dark), 2)))
            {
                AI.SelectNextCard(CardId.StapleDragostapelia);
                AI.SelectMaterials(enemyMonsters);
                return true;
            }

            // Starving Venom = 2 DARK
            if (Bot.ExtraDeck.ContainsCardWithId(CardId.StapleVenom)
                && enemyMonsters.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Dark), 2))
            {
                AI.SelectNextCard(CardId.StapleVenom);
                AI.SelectMaterials(enemyMonsters);
                return true;
            }

            return false;
        }

        private bool MechabaNegate()
        {
            // don't negate self
            if (Duel.LastChainPlayer == 0)
            {
                return false;
            }

            // check that we won't be discarding an important card to activate
            IList<ClientCard> list = Bot.Hand.GetMatchingCards(card => !card.IsCode(discardBlacklist));
            if (list.Count == 0)
            {
                return false;
            }
            AI.SelectCard(list);

            return true;
        }

        private bool AleisterEffect()
        {
            // search effect
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }

            // activate only in damage calc
            if (!(Duel.Phase == DuelPhase.DamageCal))
                return false;

            // activate only if fighting a monster where it makes the difference
            ClientCard myMonster = Bot.BattlingMonster;
            if (!myMonster.HasType(CardType.Fusion)) {
                return false;
            }
            ClientCard oppMonster = Enemy.BattlingMonster;
            if (oppMonster != null)
            {
                int diff = oppMonster.RealPower - myMonster.Attack;
                if (diff > 0 && (diff < 1000) || Bot.LifePoints - diff < 0)
                {
                    AI.SelectCard(myMonster);
                    return true;
                }
            }
            if (Enemy.LifePoints - myMonster.Attack <= 1000)
            {
                AI.SelectCard(myMonster);
                return true;
            }

            return false;
        }

        private bool FleurSummon()
        {
            // battle phase buff
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }

            // summon for body at end of main
            if (Duel.Player == 1 && Duel.MainPhaseEnd)
            {
                AI.SelectCard(Util.GetBestEnemyMonster(true));
                return true;
            }

            // summon to negate
            ClientCard chainCard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 1 && chainCard != null && chainCard.Location == CardLocation.MonsterZone)
            {
                AI.SelectCard(chainCard);
                return true;
            }

            return false;
        }

        private bool PunishEffect()
        {
            // don't lock ourselves out of extra if we have Ruq to use
            if (Bot.HasInSpellZone(CardId.ShaddollRuq) && (!RuqUsed || Duel.CurrentChain.ContainsCardWithId(CardId.ShaddollRuq)))
            {
                return false;
            }

            ClientCard enemyMon = Util.GetProblematicEnemyMonster(0, true);
            if (enemyMon != null)
            {
                // don't use if could wait for N'tss pop to be live
                if (Bot.HasInExtra(CardId.StapleNtss) && enemyMon.Attack <= 2500 && Enemy.GetFieldCount() > 1)
                {
                    AI.SelectCard(enemyMon);
                    AI.SelectNextCard(CardId.StapleNtss);
                    return true;   
                }

                if (Bot.HasInExtra(CardId.StapleOmega) && enemyMon.Attack <= 2800 && Bot.Graveyard.Count > 0)
                {
                    AI.SelectCard(enemyMon);
                    AI.SelectNextCard(CardId.StapleOmega);
                    return true;
                }

                if (Bot.HasInExtra(CardId.StaplePegasus) && enemyMon.Attack <= 2300)
                {
                    AI.SelectCard(enemyMon);
                    AI.SelectNextCard(CardId.StaplePegasus);
                }
            }

            return false;
        }

        private bool RuqEffect()
        {
            // flip faceup immediately to help the AI realise to fusion summon
            if (Card.IsFacedown())
            {
                AI.SelectYesNo(false);
                return true;
            }
            // Winda = 1 Shaddoll + 1 DARK
            if (Bot.ExtraDeck.ContainsCardWithId(CardId.ShaddollWinda)
                && Bot.Graveyard.IsExistingMatchingCard(card => card.HasSetcode(0x9d))
                && (Bot.Graveyard.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Dark) && !card.HasSetcode(0x9d))
                || Bot.Graveyard.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Dark), 2)))
            {
                AI.SelectCard(CardId.ShaddollWinda);
                AI.SelectMaterials(CardLocation.Grave);
                RuqUsed = true;
                return true;
            }

            // Construct = 1 Shaddoll + 1 LIGHT
            if (Bot.ExtraDeck.ContainsCardWithId(CardId.ShaddollConstruct)
                && Bot.Graveyard.IsExistingMatchingCard(card => card.HasSetcode(0x9d))
                && (Bot.Graveyard.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Light) && !card.HasSetcode(0x9d))
                || Bot.Graveyard.IsExistingMatchingCard(card => card.HasAttribute(CardAttribute.Light), 2)))
            {
                AI.SelectCard(CardId.ShaddollConstruct);
                AI.SelectMaterials(CardLocation.Grave);
                RuqUsed = true;
                return true;
            }

            return false;
        }

        private static class TalentOptions
        {
            public const int draw = 0;
            public const int control = 1;
            public const int hand = 2;
        }

        private bool TalentsEffect()
        {
            // take control
            ClientCard enemyMon = Util.GetBestEnemyMonster();
            if (enemyMon != null)
            {
                AI.SelectOption(TalentOptions.control);
                AI.SelectCard(enemyMon);
                return true;
            }

            // hand loop
            if (Enemy.GetHandCount() > 0)
            {
                AI.SelectOption(TalentOptions.hand);
                AI.SelectCard(CardLocation.Hand);
                return true;
            }

            // draw
            AI.SelectOption(TalentOptions.draw);
            return true;
        }

        private bool DragoEff()
        {
            // summon to negate
            ClientCard chainCard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 1 && chainCard != null && !chainCard.IsShouldNotBeMonsterTarget()) // NOTE: Can check for already has counter?
            {
                AI.SelectCard(chainCard);
                return true;
            }

            if (Duel.Phase == DuelPhase.End)
            {
                ClientCard enemyMon = Util.GetBestEnemyMonster(true, true);
                if (enemyMon != null)
                {
                    AI.SelectCard(enemyMon);
                }
                return true;
            }

            return false;
        }

        // priority 2 - combo (invoked)

        private bool TerraformingEffect()
        {
            AI.SelectCard(CardId.InvokedMeltdown);
            return true;
        }

        private bool MeltdownEffect()
        {
            AI.SelectYesNo(true);
            return true;
        }

        private bool AleisterSummon()
        {
            return true;
        }

        private bool AlmirajSummon()
        {
            if (Bot.HasInMonstersZone(CardId.InvokedAleister))
            {
                AI.SelectMaterials(CardId.InvokedAleister);
                return true;
            }

            return false;
        }

        private bool GardnaSummon()
        {
            if (Bot.HasInMonstersZone(CardId.InvokedAlmiraj))
            {
                AI.SelectMaterials(CardId.InvokedAlmiraj);
                return true;
            }

            return false;
        }

        private bool InvocationEffect()
        {
            // shuffle effect
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            IList<ClientCard> lightCards = Enemy.Graveyard.GetMatchingCards(card => card.HasAttribute(CardAttribute.Light));
            if (lightCards.Count > 0)
            {
                AI.SelectCard(CardId.InvokedMechaba);
                AI.SelectMaterials(lightCards);
                return true;
            }

            if (Bot.HasInMonstersZoneOrInGraveyard(CardId.InvokedGardna))
            {
                AI.SelectCard(CardId.InvokedMechaba);
                AI.SelectMaterials(CardId.InvokedGardna);
                return true;
            }

            IList<ClientCard> fusionCards = Enemy.Graveyard.GetMatchingCards(card => card.HasType(CardType.Fusion));
            if (fusionCards.Count > 0)
            {
                AI.SelectCard(CardId.InvokedTower);
                AI.SelectMaterials(fusionCards);
                return true;
            }

            if (Bot.Graveyard.GetMatchingCardsCount(card => card.HasAttribute(CardAttribute.Light)) > 0)
            {
                AI.SelectCard(CardId.InvokedMechaba);
                return true;
            }

            // don't banish a reserved combo piece for Augoeides
            int[] reservedBanish =
            {
                CardId.ShaddollApkallone,
                CardId.ShaddollConstruct,
                CardId.DragmaBastard
            };
            IList<ClientCard> selfFusionCards = Bot.Graveyard.GetMatchingCards(card => card.HasType(CardType.Fusion) && !card.IsCode(reservedBanish));
            if (selfFusionCards.Count > 0)
            {
                AI.SelectCard(CardId.InvokedTower);
                AI.SelectMaterials(selfFusionCards);
                return true;
            }

            IList<ClientCard> darkCards = Enemy.Graveyard.GetMatchingCards(card => card.HasAttribute(CardAttribute.Dark));
            if (darkCards.Count > 0)
            {
                AI.SelectCard(CardId.InvokedCaliga);
                AI.SelectMaterials(darkCards);
                return true;
            }

            return false;
        }

        // priority 3 - dragma combo

        private bool NadirEffect()
        {
            bool use = false;
            if (Bot.HasInExtra(CardId.ShaddollApkallone))
            {
                AI.SelectCard(CardId.ShaddollApkallone);
                use = true;
            } else if (Bot.HasInExtra(CardId.StapleNtss) && Enemy.GetFieldCount() > 0)
            {
                AI.SelectCard(CardId.StapleNtss);
                use = true;
            } else if (Bot.HasInExtra(CardId.StapleOmega) && Bot.Graveyard.Count > 0)
            {
                AI.SelectCard(CardId.StapleOmega);
                use = true;
            } else if (Bot.HasInExtra(CardId.StaplePegasus))
            {
                AI.SelectCard(CardId.StaplePegasus);
                use = true;
            } else if (Bot.HasInExtra(CardId.StapleOmega))
            {
                AI.SelectCard(CardId.StapleOmega);
                use = true;
            }

            if (!use)
                return false;

            if (!Bot.HasInHand(CardId.DragmaEcclesia))
            {
                AI.SelectNextCard(CardId.DragmaEcclesia);
            } else if (!Bot.HasInHand(CardId.DragmaMaximus))
            {
                AI.SelectNextCard(CardId.DragmaMaximus);
            } else
            {
                AI.SelectNextCard(CardId.DragmaFleur);
            }

            return true;
        }

        private bool EcclesiaNormal()
        {
            return true;
        }

        private bool EcclesiaEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return true;
            }

            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DragmaMaximus))
            {
                AI.SelectCard(CardId.DragmaMaximus);
                EcclesiaUsed = true;
                return true;
            }

            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DragmaFleur) && !MaximusUsed && !BastardSentThisTurn)
            {
                AI.SelectCard(CardId.DragmaFleur);
                EcclesiaUsed = true;
                return true;
            }

            if (!Bot.HasInHand(CardId.DragmaPunish))
            {
                AI.SelectCard(CardId.DragmaPunish);
                EcclesiaUsed = true;
                return true;
            }

            return false;
        }

        private bool ApkalloneSearch()
        {
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.ShaddollRuq))
            {
                AI.SelectCard(CardId.ShaddollRuq);
                if (!MaximusUsed && Bot.HasInExtra(CardId.ShaddollConstruct))
                {
                    // discard and fetch with construct
                    AI.SelectNextCard(CardId.ShaddollRuq);
                }
                // get hand before adding Ruq, so shouldn't discard
                // TODO: Refine what not to discard
                AI.SelectNextCard(Bot.Hand);
                return true;
            }

            AI.SelectCard(CardId.ShaddollBeast);
            AI.SelectNextCard(CardId.ShaddollBeast);
            return true;
        }

        private bool MaximusSummon()
        {
            if (Bot.HasInGraveyard(CardId.InvokedAlmiraj))
            {
                AI.SelectCard(CardId.InvokedAlmiraj);
                return true;
            }

            if (Bot.HasInGraveyard(CardId.ShaddollApkallone))
            {
                AI.SelectCard(CardId.ShaddollApkallone);
                return true;
            }

            if (Bot.HasInGraveyard(CardId.DragmaBastard) && !BastardSentThisTurn)
            {
                AI.SelectCard(CardId.DragmaBastard);
                return true;
            }

            int[] miscBanishes =
            {
                CardId.StapleNtss,
                CardId.StapleVenom,
                CardId.StapleDragostapelia,
                CardId.ShaddollWinda,
                CardId.ShaddollConstruct,
                CardId.InvokedMechaba,
                CardId.InvokedCaliga,
                CardId.InvokedTower
            };

            if (Bot.HasInGraveyard(miscBanishes))
            {
                AI.SelectCard(miscBanishes);
                return true;
            }

            return false;
        }

        private bool MaximusMill()
        {
            int cards = 0;
            if (Bot.HasInExtra(CardId.ShaddollApkallone))
            {
                AI.SelectCard(CardId.ShaddollApkallone);
                cards++;
            }

            if (Bot.HasInGraveyard(CardId.ShaddollRuq) && Bot.HasInExtra(CardId.ShaddollConstruct))
            {
                if (cards == 0)
                    AI.SelectCard(CardId.ShaddollConstruct);
                else
                    AI.SelectNextCard(CardId.ShaddollConstruct);
                cards++;
            }
            if (cards == 2)
            {
                MaximusUsed = true;
                return true;
            }

            if (Bot.HasInExtra(CardId.DragmaBastard))
            {
                if (cards == 0)
                    AI.SelectCard(CardId.DragmaBastard);
                else
                    AI.SelectNextCard(CardId.DragmaBastard);
                cards++;
                BastardSentThisTurn = true;
            }
            if (cards == 2)
            {
                MaximusUsed = true;
                return true;
            }

            if (Bot.HasInExtra(CardId.StapleNtss) && Enemy.GetFieldCount() > 0)
            {
                if (cards == 0)
                    AI.SelectCard(CardId.StapleNtss);
                else
                    AI.SelectNextCard(CardId.StapleNtss);
                cards++;
            }
            if (cards == 2)
            {
                MaximusUsed = true;
                return true;
            }

            if (Bot.HasInExtra(CardId.StapleOmega) && Bot.Graveyard.Count > 0)
            {
                if (cards == 0)
                    AI.SelectCard(CardId.StapleOmega);
                else
                    AI.SelectNextCard(CardId.StapleOmega);
                cards++;
            }
            if (cards == 2)
            {
                MaximusUsed = true;
                return true;
            }

            if (Bot.HasInExtra(CardId.StaplePegasus))
            {
                if (cards == 0)
                    AI.SelectCard(CardId.StaplePegasus);
                else
                    AI.SelectNextCard(CardId.StaplePegasus);
                cards++;
            }
            if (cards == 2)
            {
                MaximusUsed = true;
                return true;
            }

            if (cards == 1 && Bot.HasInExtra(CardId.StapleOmega))
            {
                AI.SelectNextCard(CardId.DragmaBastard);
                MaximusUsed = true;
                return true;
            }

            return false;
        }

        private bool ConstructRecover()
        {
            if (Bot.HasInGraveyard(CardId.ShaddollRuq))
            {
                AI.SelectCard(CardId.ShaddollRuq);
                return true;
            }

            return false;
        }

        private static class BastardOptions
        {
            public const int search = 0;
            public const int summon = 1;
        }

        private bool BastardSearch()
        {
            if (!EcclesiaUsed)
            {
                AI.SelectCard(CardId.DragmaEcclesia);
                AI.SelectOption(BastardOptions.summon);
                return true;
            }

            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.DragmaFleur))
            {
                // let ecclesia search fleur
                BastardSentThisTurn = false;
                AI.SelectCard(CardId.DragmaFleur);
                AI.SelectOption(BastardOptions.search);
                return true;
            }

            return false;
        }

        // priority 4 - misc send targets
        private bool NtssPop()
        {
            ClientCard bestCard = Util.GetBestEnemyCard(false, true);
            if (bestCard != null)
            {
                AI.SelectCard(bestCard);
                return true;
            }
            return false;
        }

        private bool OmegaRecur()
        {
            // TODO: Add priority list
            return true;
        }

        private bool PegasusSpin()
        {
            AI.SelectCard(Util.GetBestEnemyCard(false, true));
            return true;
        }

        // priority 5 - niche scenarios
        private bool BeastEffect()
        {
            // draw 1
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            // draw 2
            // TODO: Refine what not to discard
            return true;
        }

        private bool ConstructMill()
        {
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.ShaddollBeast))
            {
                AI.SelectCard(CardId.ShaddollBeast);
                return true;
            }
            return false;
        }

        private bool VenomEff()
        {
            // float
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            AI.SelectCard(Util.GetBestEnemyMonster());
            return true;
        }

        private bool TowerEff()
        {
            int[] reservedBanish =
            {
                CardId.ShaddollApkallone,
                CardId.ShaddollConstruct,
                CardId.DragmaBastard
            };

            // buff
            if (ActivateDescription == Util.GetStringId(CardId.InvokedTower, 0))
            {
                // don't banish a reserved combo piece for Augoeides
                IList<ClientCard> fusionCards = Bot.Graveyard.GetMatchingCards(card => card.HasType(CardType.Fusion) && !card.IsCode(reservedBanish));
                if (fusionCards.Count > 0)
                {
                    AI.SelectCard(fusionCards);
                    return true;
                }
                return false;
            }

            AI.SelectCard(Util.GetBestEnemyMonster(false, true));
            return true;
        }

        // priority 6 - set backrow

        private bool RuqSet()
        {
            return true;
        }

        private bool TrapSet()
        {
            if ((Bot.HasInHandOrInSpellZone(CardId.StapleSuperPoly) || Bot.HasInMonstersZone(CardId.InvokedMechaba)) && Bot.GetHandCount() == 1)
                return false;
            if (!Util.IsTurn1OrMain2())
                return false;
            AI.SelectPlace(Zones.z0 + Zones.z1 + Zones.z3 + Zones.z4);
            return true;
        }
    }
}
