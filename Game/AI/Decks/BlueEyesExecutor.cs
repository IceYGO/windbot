using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Blue-Eyes", "AI_BlueEyes")]
    class BlueEyesExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int WhiteDragon = 89631139;
            public const int AlternativeWhiteDragon = 38517737;
            public const int DragonSpiritOfWhite = 45467446;
            public const int WhiteStoneOfAncients = 71039903;
            public const int WhiteStoneOfLegend = 79814787;
            public const int SageWithEyesOfBlue = 8240199;
            public const int EffectVeiler = 97268402;
            public const int GalaxyCyclone = 5133471;
            public const int HarpiesFeatherDuster = 18144506;
            public const int ReturnOfTheDragonLords = 6853254;
            public const int PotOfDesires = 35261759;
            public const int TradeIn = 38120068;
            public const int CardsOfConsonance = 39701395;
            public const int DragonShrine = 41620959;
            public const int MelodyOfAwakeningDragon = 48800175;
            public const int SoulCharge = 54447022;
            public const int MonsterReborn = 83764718;
            public const int SilversCry = 87025064;

            public const int Giganticastle = 63422098;
            public const int AzureEyesSilverDragon = 40908371;
            public const int BlueEyesSpiritDragon = 59822133;
            public const int GalaxyEyesDarkMatterDragon = 58820923;
            public const int GalaxyEyesCipherBladeDragon = 2530830;
            public const int GalaxyEyesFullArmorPhotonDragon = 39030163;
            public const int GalaxyEyesPrimePhotonDragon = 31801517;
            public const int GalaxyEyesCipherDragon = 18963306;
            public const int HopeHarbingerDragonTitanicGalaxy = 63767246;
            public const int SylvanPrincessprite = 33909817;
        }

        private List<ClientCard> UsedAlternativeWhiteDragon = new List<ClientCard>();
        ClientCard UsedGalaxyEyesCipherDragon;
        bool AlternativeWhiteDragonSummoned = false;
        bool SoulChargeUsed = false;

        public BlueEyesExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // destroy traps
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, CardId.DragonShrine, DragonShrineEffect);

            // Sage search
            AddExecutor(ExecutorType.Summon, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueSummon);

            // search Alternative White Dragon
            AddExecutor(ExecutorType.Activate, CardId.MelodyOfAwakeningDragon, MelodyOfAwakeningDragonEffect);

            AddExecutor(ExecutorType.Activate, CardId.CardsOfConsonance, CardsOfConsonanceEffect);

            AddExecutor(ExecutorType.Activate, CardId.TradeIn, TradeInEffect);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires, DefaultPotOfDesires);

            // spsummon Alternative White Dragon if possible
            AddExecutor(ExecutorType.SpSummon, CardId.AlternativeWhiteDragon, AlternativeWhiteDragonSummon);

            // reborn
            AddExecutor(ExecutorType.Activate, CardId.ReturnOfTheDragonLords, RebornEffect);
            AddExecutor(ExecutorType.Activate, CardId.SilversCry, RebornEffect);
            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, RebornEffect);

            // monster effects
            AddExecutor(ExecutorType.Activate, CardId.AlternativeWhiteDragon, AlternativeWhiteDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffect);
            AddExecutor(ExecutorType.Activate, CardId.WhiteStoneOfAncients, WhiteStoneOfAncientsEffect);
            AddExecutor(ExecutorType.Activate, CardId.DragonSpiritOfWhite, DragonSpiritOfWhiteEffect);
            AddExecutor(ExecutorType.Activate, CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.HopeHarbingerDragonTitanicGalaxy, HopeHarbingerDragonTitanicGalaxyEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyEyesCipherDragon, GalaxyEyesCipherDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyEyesPrimePhotonDragon, GalaxyEyesPrimePhotonDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyEyesFullArmorPhotonDragon, GalaxyEyesFullArmorPhotonDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyEyesCipherBladeDragon, GalaxyEyesCipherBladeDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxyEyesDarkMatterDragon, GalaxyEyesDarkMatterDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.AzureEyesSilverDragon, AzureEyesSilverDragonEffect);
            AddExecutor(ExecutorType.Activate, CardId.SylvanPrincessprite, SylvanPrincesspriteEffect);

            // normal summon
            AddExecutor(ExecutorType.Summon, CardId.SageWithEyesOfBlue, WhiteStoneSummon);
            AddExecutor(ExecutorType.Summon, CardId.WhiteStoneOfAncients, WhiteStoneSummon);
            AddExecutor(ExecutorType.Summon, CardId.WhiteStoneOfLegend, WhiteStoneSummon);

            // special summon from extra
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyEyesCipherDragon, GalaxyEyesCipherDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyEyesPrimePhotonDragon, GalaxyEyesPrimePhotonDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyEyesFullArmorPhotonDragon, GalaxyEyesFullArmorPhotonDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyEyesCipherBladeDragon, GalaxyEyesCipherBladeDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GalaxyEyesDarkMatterDragon, GalaxyEyesDarkMatterDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Giganticastle, GiganticastleSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HopeHarbingerDragonTitanicGalaxy, HopeHarbingerDragonTitanicGalaxySummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SylvanPrincessprite, SylvanPrincesspriteSummon);

            // if we don't have other things to do...
            AddExecutor(ExecutorType.Activate, CardId.SoulCharge, SoulChargeEffect);
            AddExecutor(ExecutorType.Repos, Repos);
            // summon White Stone to use the hand effect of Sage
            AddExecutor(ExecutorType.Summon, CardId.WhiteStoneOfLegend, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Summon, CardId.WhiteStoneOfAncients, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Summon, CardId.SageWithEyesOfBlue, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Activate, CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffectInHand);
            // set White Stone of Legend frist
            AddExecutor(ExecutorType.MonsterSet, CardId.WhiteStoneOfLegend);
            AddExecutor(ExecutorType.MonsterSet, CardId.WhiteStoneOfAncients);

            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public override void OnNewTurn()
        {
            // reset
            UsedAlternativeWhiteDragon.Clear();
            UsedGalaxyEyesCipherDragon = null;
            AlternativeWhiteDragonSummoned = false;
            SoulChargeUsed = false;
            base.OnNewTurn();
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);
            if (max == 2 && cards[0].Location == CardLocation.Deck)
            {
                Logger.DebugWriteLine("OnSelectCard MelodyOfAwakeningDragon");
                List<ClientCard> result = new List<ClientCard>();
                if (!Bot.HasInHand(CardId.WhiteDragon))
                    result.AddRange(cards.Where(card => card.IsCode(CardId.WhiteDragon)).Take(1));
                result.AddRange(cards.Where(card => card.IsCode(CardId.AlternativeWhiteDragon)));
                return Util.CheckSelectCount(result, cards, min, max);
            }
            Logger.DebugWriteLine("Use default.");

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override IList<ClientCard> OnSelectXyzMaterial(IList<ClientCard> cards, int min, int max)
        {
            Logger.DebugWriteLine("OnSelectXyzMaterial " + cards.Count + " " + min + " " + max);
            IList<ClientCard> result = Util.SelectPreferredCards(UsedAlternativeWhiteDragon, cards, min, max);
            return Util.CheckSelectCount(result, cards, min, max);
        }

        public override IList<ClientCard> OnSelectSynchroMaterial(IList<ClientCard> cards, int sum, int min, int max)
        {
            Logger.DebugWriteLine("OnSelectSynchroMaterial " + cards.Count + " " + sum + " " + min + " " + max);
            if (sum != 8)
                return null;

            foreach (ClientCard AlternativeWhiteDragon in UsedAlternativeWhiteDragon)
            {
                if (cards.IndexOf(AlternativeWhiteDragon) > 0)
                {
                    UsedAlternativeWhiteDragon.Remove(AlternativeWhiteDragon);
                    Logger.DebugWriteLine("select UsedAlternativeWhiteDragon");
                    return new[] { AlternativeWhiteDragon };
                }
            }

            return null;
        }

        public override void OnSpSummoned()
        {
            // not special summoned by chain
            if (Duel.GetCurrentSolvingChainCard() == null)
            {
                foreach (ClientCard card in Duel.LastSummonedCards)
                {
                    if (card.Controller == 0 && card.IsCode(CardId.AlternativeWhiteDragon))
                    {
                        AlternativeWhiteDragonSummoned = true;
                    }
                }
            }
            base.OnSpSummoned();
        }

        private bool DragonShrineEffect()
        {
            AI.SelectCard(
                CardId.DragonSpiritOfWhite,
                CardId.WhiteDragon,
                CardId.WhiteStoneOfAncients,
                CardId.WhiteStoneOfLegend
                );
            if (!Bot.HasInHand(CardId.WhiteDragon))
            {
                AI.SelectNextCard(CardId.WhiteStoneOfLegend);
            }
            else
            {
                AI.SelectNextCard(
                    CardId.WhiteStoneOfAncients,
                    CardId.DragonSpiritOfWhite,
                    CardId.WhiteStoneOfLegend
                    );
            }
            return true;
        }

        private bool MelodyOfAwakeningDragonEffect()
        {
            AI.SelectCard(
                CardId.WhiteStoneOfAncients,
                CardId.DragonSpiritOfWhite,
                CardId.WhiteStoneOfLegend,
                CardId.GalaxyCyclone,
                CardId.EffectVeiler,
                CardId.TradeIn,
                CardId.SageWithEyesOfBlue
                );
            return true;
        }

        private bool CardsOfConsonanceEffect()
        {
            if (!Bot.HasInHand(CardId.WhiteDragon))
            {
                AI.SelectCard(CardId.WhiteStoneOfLegend);
            }
            else if (Bot.HasInHand(CardId.TradeIn))
            {
                AI.SelectCard(CardId.WhiteStoneOfLegend);
            }
            else
            {
                AI.SelectCard(CardId.WhiteStoneOfAncients);
            }
            return true;
        }

        private bool TradeInEffect()
        {
            if (Bot.HasInHand(CardId.DragonSpiritOfWhite))
            {
                AI.SelectCard(CardId.DragonSpiritOfWhite);
                return true;
            }
            else if (HasTwoInHand(CardId.WhiteDragon))
            {
                AI.SelectCard(CardId.WhiteDragon);
                return true;
            }
            else if (HasTwoInHand(CardId.AlternativeWhiteDragon))
            {
                AI.SelectCard(CardId.AlternativeWhiteDragon);
                return true;
            }
            else if (!Bot.HasInHand(CardId.WhiteDragon) || !Bot.HasInHand(CardId.AlternativeWhiteDragon))
            {
                AI.SelectCard(CardId.WhiteDragon, CardId.AlternativeWhiteDragon);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AlternativeWhiteDragonEffect()
        {
            ClientCard target = Util.GetProblematicEnemyMonster(Card.GetDefensePower());
            if (target != null)
            {
                AI.SelectCard(target);
                UsedAlternativeWhiteDragon.Add(Card);
                return true;
            }
            if (Util.GetBotAvailZonesFromExtraDeck(Card) > 0
                && (Bot.HasInMonstersZone(new[]
                {
                    CardId.SageWithEyesOfBlue,
                    CardId.WhiteStoneOfAncients,
                    CardId.WhiteStoneOfLegend,
                    CardId.WhiteDragon,
                    CardId.DragonSpiritOfWhite
                }) || Bot.GetCountCardInZone(Bot.MonsterZone, CardId.AlternativeWhiteDragon) >= 2))
            {
                target = Util.GetBestEnemyMonster(false, true);
                AI.SelectCard(target);
                UsedAlternativeWhiteDragon.Add(Card);
                return true;
            }
            return false;
        }

        private bool RebornEffect()
        {
            if (Duel.Player == 0 && Duel.CurrentChain.Count > 0)
            {
                // Silver's Cry spsummon Dragon Spirit at chain 2 will miss the timing
                return false;
            }
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
            {
                // Let Azure-Eyes spsummon first
                return false;
            }
            IList<int> targets = new[] {
                    CardId.HopeHarbingerDragonTitanicGalaxy,
                    CardId.GalaxyEyesDarkMatterDragon,
                    CardId.AlternativeWhiteDragon,
                    CardId.AzureEyesSilverDragon,
                    CardId.BlueEyesSpiritDragon,
                    CardId.WhiteDragon,
                    CardId.DragonSpiritOfWhite
                };
            if (!Bot.HasInGraveyard(targets))
            {
                return false;
            }
            ClientCard floodgate = Enemy.SpellZone.GetFloodgate();
            if (floodgate != null && Bot.HasInGraveyard(CardId.DragonSpiritOfWhite))
            {
                AI.SelectCard(CardId.DragonSpiritOfWhite);
            }
            else
            {
                AI.SelectCard(targets);
            }
            return true;
        }

        private bool AzureEyesSilverDragonEffect()
        {
            if (Enemy.GetSpellCount() > 0)
            {
                AI.SelectCard(CardId.DragonSpiritOfWhite);
            }
            else
            {
                AI.SelectCard(CardId.WhiteDragon);
            }
            return true;
        }

        private bool SageWithEyesOfBlueSummon()
        {
            return !Bot.HasInHand(new[]
                {
                    CardId.WhiteStoneOfAncients,
                    CardId.WhiteStoneOfLegend
                });
        }

        private bool SageWithEyesOfBlueEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return false;
            }
            AI.SelectCard(
                CardId.WhiteStoneOfAncients,
                CardId.EffectVeiler,
                CardId.WhiteStoneOfLegend
                );
            return true;
        }

        private bool WhiteStoneSummonForSage()
        {
            return Bot.HasInHand(CardId.SageWithEyesOfBlue);
        }

        private bool SageWithEyesOfBlueEffectInHand()
        {
            if (Card.Location != CardLocation.Hand)
            {
                return false;
            }
            if (!Bot.HasInMonstersZone(new[]
                {
                    CardId.WhiteStoneOfLegend,
                    CardId.WhiteStoneOfAncients
                }) || Bot.HasInMonstersZone(new[]
                {
                    CardId.AlternativeWhiteDragon,
                    CardId.WhiteDragon,
                    CardId.DragonSpiritOfWhite
                }))
            {
                return false;
            }
            AI.SelectCard(CardId.WhiteStoneOfLegend, CardId.WhiteStoneOfAncients);
            if (Enemy.GetSpellCount() > 0)
            {
                AI.SelectNextCard(CardId.DragonSpiritOfWhite);
            }
            else
            {
                AI.SelectNextCard(CardId.WhiteDragon);
            }
            return true;
        }

        private bool DragonSpiritOfWhiteEffect()
        {
            if (ActivateDescription == -1)
            {
                ClientCard target = Util.GetBestEnemySpell();
                AI.SelectCard(target);
                return true;
            }
            else if (HaveEnoughWhiteDragonInHand())
            {
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                {
                    return Card.Attacked;
                }
                if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                {
                    return Bot.HasInMonstersZone(CardId.AzureEyesSilverDragon, true)
                        && !Bot.HasInGraveyard(CardId.DragonSpiritOfWhite)
                        && !Bot.HasInGraveyard(CardId.WhiteDragon);
                }
                if (Util.IsChainTarget(Card))
                {
                    return true;
                }
            }
            return false;
        }

        private bool BlueEyesSpiritDragonEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.BlueEyesSpiritDragon, 0))
            {
                return Duel.LastChainPlayer == 1;
            }
            else if (Duel.Player == 1 && (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End))
            {
                AI.SelectCard(CardId.AzureEyesSilverDragon);
                return true;
            }
            else
            {
                if (Util.IsChainTarget(Card))
                {
                    AI.SelectCard(CardId.AzureEyesSilverDragon);
                    return true;
                }
                return false;
            }
        }

        private bool HopeHarbingerDragonTitanicGalaxyEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.HopeHarbingerDragonTitanicGalaxy, 0))
            {
                return Duel.LastChainPlayer == 1;
            }
            return true;
        }

        private bool WhiteStoneOfAncientsEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.WhiteStoneOfAncients, 0))
            {
                if (Bot.HasInHand(CardId.TradeIn)
                    && !Bot.HasInHand(CardId.WhiteDragon)
                    && !Bot.HasInHand(CardId.AlternativeWhiteDragon))
                {
                    AI.SelectCard(CardId.WhiteDragon);
                    return true;
                }
                if (AlternativeWhiteDragonSummoned)
                {
                    return false;
                }
                if (Bot.HasInHand(CardId.WhiteDragon)
                    && !Bot.HasInHand(CardId.AlternativeWhiteDragon)
                    && Bot.HasInGraveyard(CardId.AlternativeWhiteDragon))
                {
                    AI.SelectCard(CardId.AlternativeWhiteDragon);
                    return true;
                }
                if (Bot.HasInHand(CardId.AlternativeWhiteDragon)
                    && !Bot.HasInHand(CardId.WhiteDragon)
                    && Bot.HasInGraveyard(CardId.WhiteDragon))
                {
                    AI.SelectCard(CardId.WhiteDragon);
                    return true;
                }
                return false;
            }
            else
            {
                if (Enemy.GetSpellCount() > 0)
                {
                    AI.SelectCard(CardId.DragonSpiritOfWhite);
                }
                else
                {
                    AI.SelectCard(CardId.WhiteDragon);
                }
                return true;
            }
        }

        private bool AlternativeWhiteDragonSummon()
        {
            return true;
        }

        private bool WhiteStoneSummon()
        {
            return Bot.HasInMonstersZone(new[]
                {
                    CardId.SageWithEyesOfBlue,
                    CardId.WhiteStoneOfAncients,
                    CardId.WhiteStoneOfLegend,
                    CardId.AlternativeWhiteDragon,
                    CardId.WhiteDragon,
                    CardId.DragonSpiritOfWhite
                });
        }

        private bool GalaxyEyesCipherDragonSummon()
        {
            if (Duel.Turn == 1 || SoulChargeUsed)
            {
                return false;
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count == 1 && !monsters[0].IsFacedown() && ((monsters[0].IsDefense() && monsters[0].GetDefensePower() >= 3000) && monsters[0].HasType(CardType.Xyz)))
            {
                return true;
            }
            if (monsters.Count >= 3)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.IsFacedown() && ((monster.IsDefense() && monster.GetDefensePower() >= 3000) || monster.HasType(CardType.Xyz)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool GalaxyEyesPrimePhotonDragonSummon()
        {
            if (Duel.Turn == 1)
            {
                return false;
            }
            if (Util.IsOneEnemyBetterThanValue(2999, false))
            {
                return true;
            }
            return false;
        }

        private bool GalaxyEyesFullArmorPhotonDragonSummon()
        {
            if (Bot.HasInMonstersZone(CardId.GalaxyEyesCipherDragon))
            {
                foreach (ClientCard monster in Bot.GetMonsters())
                {
                    if ((monster.IsDisabled() && monster.HasType(CardType.Xyz) && !monster.Equals(UsedGalaxyEyesCipherDragon))
                        || (Duel.Phase == DuelPhase.Main2 && monster.Equals(UsedGalaxyEyesCipherDragon)))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            if (Bot.HasInMonstersZone(CardId.GalaxyEyesPrimePhotonDragon))
            {
                if (!Util.IsOneEnemyBetterThanValue(4000, false))
                {
                    AI.SelectCard(CardId.GalaxyEyesPrimePhotonDragon);
                    return true;
                }
            }
            return false;
        }

        private bool GalaxyEyesCipherBladeDragonSummon()
        {
            if (Bot.HasInMonstersZone(CardId.GalaxyEyesFullArmorPhotonDragon) && Util.GetProblematicEnemyCard() != null)
            {
                AI.SelectCard(CardId.GalaxyEyesFullArmorPhotonDragon);
                return true;
            }
            return false;
        }

        private bool GalaxyEyesDarkMatterDragonSummon()
        {
            if (Bot.HasInMonstersZone(CardId.GalaxyEyesFullArmorPhotonDragon))
            {
                AI.SelectCard(CardId.GalaxyEyesFullArmorPhotonDragon);
                return true;
            }
            return false;
        }

        private bool GalaxyEyesPrimePhotonDragonEffect()
        {
            return true;
        }

        private bool GalaxyEyesCipherDragonEffect()
        {
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.HasType(CardType.Xyz))
                {
                    AI.SelectCard(monster);
                    UsedGalaxyEyesCipherDragon = Card;
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsDefense())
                {
                    AI.SelectCard(monster);
                    UsedGalaxyEyesCipherDragon = Card;
                    return true;
                }
            }
            UsedGalaxyEyesCipherDragon = Card;
            return true;
        }

        private bool GalaxyEyesFullArmorPhotonDragonEffect()
        {
            ClientCard target = Util.GetProblematicEnemySpell();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            target = Util.GetProblematicEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            foreach (ClientCard spell in Enemy.GetSpells())
            {
                if (spell.IsFaceup())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            if (monsters.Count >= 2)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsDefense())
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
                return true;
            }
            if (monsters.Count == 2)
            {
                foreach (ClientCard monster in monsters)
                {
                    if (monster.IsMonsterInvincible() || monster.IsMonsterDangerous() || monster.GetDefensePower() > 4000)
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            if (monsters.Count == 1)
            {
                return true;
            }
            return false;
        }

        private bool GalaxyEyesCipherBladeDragonEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }
            ClientCard target = Util.GetProblematicEnemyCard();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> monsters = Enemy.GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (monster.IsDefense())
                {
                    AI.SelectCard(monster);
                    return true;
                }
            }
            foreach (ClientCard monster in monsters)
            {
                AI.SelectCard(monster);
                return true;
            }
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
            {
                if (spell.IsFacedown())
                {
                    AI.SelectCard(spell);
                    return true;
                }
            }
            foreach (ClientCard spell in spells)
            {
                AI.SelectCard(spell);
                return true;
            }
            return false;
        }

        private bool GalaxyEyesDarkMatterDragonEffect()
        {
            AI.SelectCard(
                CardId.WhiteStoneOfAncients,
                CardId.WhiteStoneOfLegend,
                CardId.DragonSpiritOfWhite,
                CardId.WhiteDragon
                );
            AI.SelectNextCard(
                CardId.WhiteStoneOfAncients,
                CardId.WhiteStoneOfLegend,
                CardId.DragonSpiritOfWhite,
                CardId.WhiteDragon
                );
            return true;
        }

        private bool GiganticastleSummon()
        {
            if (Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1 || SoulChargeUsed)
                return false;
            int bestSelfAttack = Util.GetBestAttack(Bot);
            int bestEnemyAttack = Util.GetBestPower(Enemy);
            return bestSelfAttack <= bestEnemyAttack && bestEnemyAttack > 2500 && bestEnemyAttack <= 3100;
        }

        private bool BlueEyesSpiritDragonSummon()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                if (UsedAlternativeWhiteDragon.Count > 0)
                {
                    return true;
                }
                if (Duel.Turn == 1 || SoulChargeUsed)
                {
                    AI.SelectPosition(CardPosition.FaceUpDefence);
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }

        private bool HopeHarbingerDragonTitanicGalaxySummon()
        {
            if (Duel.Phase == DuelPhase.Main1)
            {
                if (UsedAlternativeWhiteDragon.Count > 0)
                {
                    return true;
                }
                if (Duel.Turn == 1 || SoulChargeUsed)
                {
                    return true;
                }
            }
            if (Duel.Phase == DuelPhase.Main2)
            {
                return true;
            }
            return false;
        }

        private bool SylvanPrincesspriteSummon()
        {
            if (Duel.Turn == 1)
            {
                return true;
            }
            if (Duel.Phase == DuelPhase.Main1 && !Bot.HasInMonstersZone(new[]
                {
                    CardId.AlternativeWhiteDragon,
                    CardId.WhiteDragon,
                    CardId.DragonSpiritOfWhite
                }))
            {
                return true;
            }
            if (Duel.Phase == DuelPhase.Main2 || SoulChargeUsed)
            {
                return true;
            }
            return false;
        }

        private bool SylvanPrincesspriteEffect()
        {
            AI.SelectCard(CardId.WhiteStoneOfLegend, CardId.WhiteStoneOfAncients);
            return true;
        }

        private bool SoulChargeEffect()
        {
            if (Bot.HasInMonstersZone(CardId.BlueEyesSpiritDragon, true))
                return false;
            int count = Bot.GetGraveyardMonsters().Count;
            int space = 5 - Bot.GetMonstersInMainZone().Count;
            if (count < space)
                count = space;
            if (count < 2 || Bot.LifePoints < count*1000)
                return false;
            if (Duel.Turn != 1)
            {
                int attack = 0;
                int defence = 0;
                foreach (ClientCard monster in Bot.GetMonsters())
                {
                    if (!monster.IsDefense())
                    {
                        attack += monster.Attack;
                    }
                }
                foreach (ClientCard monster in Enemy.GetMonsters())
                {
                    defence += monster.GetDefensePower();
                }
                if (attack - defence > Enemy.LifePoints)
                    return false;
            }
            AI.SelectCard(
                CardId.BlueEyesSpiritDragon,
                CardId.HopeHarbingerDragonTitanicGalaxy,
                CardId.AlternativeWhiteDragon,
                CardId.WhiteDragon,
                CardId.DragonSpiritOfWhite,
                CardId.AzureEyesSilverDragon,
                CardId.WhiteStoneOfAncients,
                CardId.WhiteStoneOfLegend
                );
            SoulChargeUsed = true;
            return true;
        }

        private bool Repos()
        {
            bool enemyBetter = Util.IsAllEnemyBetter(true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
                return true;
            if (Card.IsDefense() && Card.IsCode(CardId.BlueEyesSpiritDragon, CardId.AzureEyesSilverDragon))
                return true;
            if (Card.IsAttack() && Card.IsCode(CardId.SageWithEyesOfBlue, CardId.WhiteStoneOfAncients, CardId.WhiteStoneOfLegend))
                return true;
            return false;
        }

        private bool SpellSet()
        {
            return (Card.IsTrap() || Card.IsCode(CardId.SilversCry)) && Bot.GetSpellCountWithoutField() < 4;
        }

        private bool HasTwoInHand(int id)
        {
            int num = 0;
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && card.IsCode(id))
                    num++;
            }
            return num >= 2;
        }

        private bool HaveEnoughWhiteDragonInHand()
        {
            return HasTwoInHand(CardId.WhiteDragon) || (
                Bot.HasInGraveyard(CardId.WhiteDragon)
                && Bot.HasInGraveyard(CardId.WhiteStoneOfAncients)
                );
        }
    }
}
