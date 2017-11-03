using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Blue-Eyes", "AI_BlueEyes")]
    class BlueEyesExecutor : DefaultExecutor
    {
        public enum CardId
        {
            WhiteDragon = 89631139,
            AlternativeWhiteDragon = 38517737,
            DragonSpiritOfWhite = 45467446,
            WhiteStoneOfAncients = 71039903,
            WhiteStoneOfLegend = 79814787,
            SageWithEyesOfBlue = 8240199,
            EffectVeiler = 97268402,
            GalaxyCyclone = 5133471,
            HarpiesFeatherDuster = 18144506,
            ReturnOfTheDragonLords = 6853254,
            PotOfDesires = 35261759,
            TradeIn = 38120068,
            CardsOfConsonance = 39701395,
            DragonShrine = 41620959,
            MelodyOfAwakeningDragon = 48800175,
            SoulCharge = 54447022,
            MonsterReborn = 83764718,
            SilversCry = 87025064,

            Giganticastle = 63422098,
            AzureEyesSilverDragon = 40908371,
            BlueEyesSpiritDragon = 59822133,
            GalaxyEyesDarkMatterDragon = 58820923,
            GalaxyEyesCipherBladeDragon = 2530830,
            GalaxyEyesFullArmorPhotonDragon = 39030163,
            GalaxyEyesPrimePhotonDragon = 31801517,
            GalaxyEyesCipherDragon = 18963306,
            HopeHarbingerDragonTitanicGalaxy = 63767246,
            SylvanPrincessprite = 33909817
        }

        private List<ClientCard> UsedAlternativeWhiteDragon = new List<ClientCard>();
        ClientCard UsedGalaxyEyesCipherDragon;
        bool AlternativeWhiteDragonSummoned = false;
        bool SoulChargeUsed = false;

        public BlueEyesExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // destroy traps
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, (int)CardId.HarpiesFeatherDuster);

            AddExecutor(ExecutorType.Activate, (int)CardId.DragonShrine, DragonShrineEffect);

            // Sage search
            AddExecutor(ExecutorType.Summon, (int)CardId.SageWithEyesOfBlue, SageWithEyesOfBlueSummon);

            // search Alternative White Dragon
            AddExecutor(ExecutorType.Activate, (int)CardId.MelodyOfAwakeningDragon, MelodyOfAwakeningDragonEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.CardsOfConsonance, CardsOfConsonanceEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.TradeIn, TradeInEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.PotOfDesires, DefaultPotOfDesires);

            // spsummon Alternative White Dragon if possible
            AddExecutor(ExecutorType.SpSummon, (int)CardId.AlternativeWhiteDragon, AlternativeWhiteDragonSummon);

            // reborn
            AddExecutor(ExecutorType.Activate, (int)CardId.ReturnOfTheDragonLords, RebornEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SilversCry, RebornEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.MonsterReborn, RebornEffect);

            // monster effects
            AddExecutor(ExecutorType.Activate, (int)CardId.AlternativeWhiteDragon, AlternativeWhiteDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.WhiteStoneOfAncients, WhiteStoneOfAncientsEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DragonSpiritOfWhite, DragonSpiritOfWhiteEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.HopeHarbingerDragonTitanicGalaxy, HopeHarbingerDragonTitanicGalaxyEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesCipherDragon, GalaxyEyesCipherDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesPrimePhotonDragon, GalaxyEyesPrimePhotonDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesFullArmorPhotonDragon, GalaxyEyesFullArmorPhotonDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesCipherBladeDragon, GalaxyEyesCipherBladeDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.GalaxyEyesDarkMatterDragon, GalaxyEyesDarkMatterDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.AzureEyesSilverDragon, AzureEyesSilverDragonEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.SylvanPrincessprite, SylvanPrincesspriteEffect);

            // normal summon
            AddExecutor(ExecutorType.Summon, (int)CardId.SageWithEyesOfBlue, WhiteStoneSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.WhiteStoneOfAncients, WhiteStoneSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.WhiteStoneOfLegend, WhiteStoneSummon);

            // special summon from extra
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GalaxyEyesCipherDragon, GalaxyEyesCipherDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GalaxyEyesPrimePhotonDragon, GalaxyEyesPrimePhotonDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GalaxyEyesFullArmorPhotonDragon, GalaxyEyesFullArmorPhotonDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GalaxyEyesCipherBladeDragon, GalaxyEyesCipherBladeDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GalaxyEyesDarkMatterDragon, GalaxyEyesDarkMatterDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Giganticastle, GiganticastleSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.BlueEyesSpiritDragon, BlueEyesSpiritDragonSummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.HopeHarbingerDragonTitanicGalaxy, HopeHarbingerDragonTitanicGalaxySummon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SylvanPrincessprite, SylvanPrincesspriteSummon);

            // if we don't have other things to do...
            AddExecutor(ExecutorType.Activate, (int)CardId.SoulCharge, SoulChargeEffect);
            AddExecutor(ExecutorType.Repos, Repos);
            // summon White Stone to use the hand effect of Sage
            AddExecutor(ExecutorType.Summon, (int)CardId.WhiteStoneOfLegend, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Summon, (int)CardId.WhiteStoneOfAncients, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Summon, (int)CardId.SageWithEyesOfBlue, WhiteStoneSummonForSage);
            AddExecutor(ExecutorType.Activate, (int)CardId.SageWithEyesOfBlue, SageWithEyesOfBlueEffectInHand);
            // set White Stone of Legend frist
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.WhiteStoneOfLegend);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.WhiteStoneOfAncients);

            AddExecutor(ExecutorType.SpellSet, SpellSet);
        }

        public override bool OnSelectHand()
        {
            return Program.Rand.Next(2) > 0;
        }

        public override void OnNewTurn()
        {
            // reset
            UsedAlternativeWhiteDragon.Clear();
            UsedGalaxyEyesCipherDragon = null;
            AlternativeWhiteDragonSummoned = false;
            SoulChargeUsed = false;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, bool cancelable)
        {
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);
            if (max == 2 && cards[0].Location == CardLocation.Deck)
            {
                Logger.DebugWriteLine("OnSelectCard MelodyOfAwakeningDragon");
                IList<ClientCard> result = new List<ClientCard>();
                if (!Bot.HasInHand((int)CardId.WhiteDragon))
                {
                    foreach (ClientCard card in cards)
                    {
                        if (card.Id == (int)CardId.WhiteDragon)
                        {
                            result.Add(card);
                            break;
                        }
                    }
                }
                foreach (ClientCard card in cards)
                {
                    if (card.Id == (int)CardId.AlternativeWhiteDragon && result.Count < max)
                    {
                        result.Add(card);
                    }
                }
                if (result.Count < min)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (!result.Contains(card))
                            result.Add(card);
                        if (result.Count >= min)
                            break;
                    }
                }
                while (result.Count > max)
                {
                    result.RemoveAt(result.Count - 1);
                }
                return result;
            }
            if (max == 2 && min == 2 && cards[0].Location == CardLocation.MonsterZone)
            {
                Logger.DebugWriteLine("OnSelectCard XYZ");
                IList<ClientCard> avail = new List<ClientCard>();
                foreach (ClientCard card in cards)
                {
                    // clone
                    avail.Add(card);
                }
                IList<ClientCard> result = new List<ClientCard>();
                while (UsedAlternativeWhiteDragon.Count > 0 && avail.IndexOf(UsedAlternativeWhiteDragon[0]) > 0)
                {
                    Logger.DebugWriteLine("select UsedAlternativeWhiteDragon");
                    ClientCard card = UsedAlternativeWhiteDragon[0];
                    UsedAlternativeWhiteDragon.Remove(card);
                    avail.Remove(card);
                    result.Add(card);
                }
                if (result.Count < 2)
                {
                    foreach (ClientCard card in cards)
                    {
                        if (!result.Contains(card))
                            result.Add(card);
                        if (result.Count >= 2)
                            break;
                    }
                }
                return result;
            }
            Logger.DebugWriteLine("Use default.");
            return null;
        }

        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, bool mode)
        {
            Logger.DebugWriteLine("OnSelectSum " + cards.Count + " " + sum + " " + min + " " + max);
            if (sum != 8 || !mode)
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

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            return attacker.Attack > 0;
        }

        private bool DragonShrineEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.WhiteDragon,
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend
                });
            if (!Bot.HasInHand((int)CardId.WhiteDragon))
            {
                AI.SelectNextCard((int)CardId.WhiteStoneOfLegend);
            }
            else
            {
                AI.SelectNextCard(new[]
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.WhiteStoneOfLegend
                });
            }
            return true;
        }

        private bool MelodyOfAwakeningDragonEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.GalaxyCyclone,
                    (int)CardId.EffectVeiler,
                    (int)CardId.TradeIn,
                    (int)CardId.SageWithEyesOfBlue
                });
            return true;
        }

        private bool CardsOfConsonanceEffect()
        {
            if (!Bot.HasInHand((int)CardId.WhiteDragon))
            {
                AI.SelectCard((int)CardId.WhiteStoneOfLegend);
            }
            else if (Bot.HasInHand((int)CardId.TradeIn))
            {
                AI.SelectCard((int)CardId.WhiteStoneOfLegend);
            }
            else
            {
                AI.SelectCard((int)CardId.WhiteStoneOfAncients);
            }
            return true;
        }

        private bool TradeInEffect()
        {
            if (Bot.HasInHand((int)CardId.DragonSpiritOfWhite))
            {
                AI.SelectCard((int)CardId.DragonSpiritOfWhite);
                return true;
            }
            else if (HasTwoInHand((int)CardId.WhiteDragon))
            {
                AI.SelectCard((int)CardId.WhiteDragon);
                return true;
            }
            else if (HasTwoInHand((int)CardId.AlternativeWhiteDragon))
            {
                AI.SelectCard((int)CardId.AlternativeWhiteDragon);
                return true;
            }
            else if (!Bot.HasInHand((int)CardId.WhiteDragon) || !Bot.HasInHand((int)CardId.AlternativeWhiteDragon))
            {
                AI.SelectCard(new[]
                {
                    (int)CardId.WhiteDragon,
                    (int)CardId.AlternativeWhiteDragon
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AlternativeWhiteDragonEffect()
        {
            ClientCard card = AI.Utils.GetProblematicEnemyMonster(Card.GetDefensePower());
            if (card != null)
            {
                AI.SelectCard(card);
                UsedAlternativeWhiteDragon.Add(Card);
                return true;
            }
            if (CanDealWithUsedAlternativeWhiteDragon())
            {
                card = AI.Utils.GetBestEnemyMonster();
                AI.SelectCard(card);
                UsedAlternativeWhiteDragon.Add(Card);
                return true;
            }
            return false;
        }

        private bool RebornEffect()
        {
            if (Duel.Player == 0 && CurrentChain.Count > 0)
            {
                // Silver's Cry spsummon Dragon Spirit at chain 2 will miss the timing
                return false;
            }
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Draw || Duel.Phase == DuelPhase.Standby))
            {
                // Let Azure-Eyes spsummon first
                return false;
            }
            List<int> targets = new List<int> {
                    (int)CardId.HopeHarbingerDragonTitanicGalaxy,
                    (int)CardId.GalaxyEyesDarkMatterDragon,
                    (int)CardId.AlternativeWhiteDragon,
                    (int)CardId.AzureEyesSilverDragon,
                    (int)CardId.BlueEyesSpiritDragon,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite
                };
            if (!Bot.HasInGraveyard(targets))
            {
                return false;
            }
            ClientCard floodgate = Enemy.SpellZone.GetFloodgate();
            if (floodgate != null && Bot.HasInGraveyard((int)CardId.DragonSpiritOfWhite))
            {
                AI.SelectCard((int)CardId.DragonSpiritOfWhite);
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
                AI.SelectCard((int)CardId.DragonSpiritOfWhite);
            }
            else
            {
                AI.SelectCard((int)CardId.WhiteDragon);
            }
            return true;
        }

        private bool SageWithEyesOfBlueSummon()
        {
            return !Bot.HasInHand(new List<int>
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend
                });
        }

        private bool SageWithEyesOfBlueEffect()
        {
            if (Card.Location == CardLocation.Hand)
            {
                return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.EffectVeiler,
                    (int)CardId.WhiteStoneOfLegend
                });
            return true;
        }

        private bool WhiteStoneSummonForSage()
        {
            return Bot.HasInHand((int)CardId.SageWithEyesOfBlue);
        }

        private bool SageWithEyesOfBlueEffectInHand()
        {
            if (Card.Location != CardLocation.Hand)
            {
                return false;
            }
            if (!Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.WhiteStoneOfAncients
                }) || Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.AlternativeWhiteDragon,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite
                }))
            {
                return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.WhiteStoneOfAncients
                });
            List<ClientCard> spells = Enemy.GetSpells();
            if (spells.Count == 0)
            {
                AI.SelectNextCard((int)CardId.WhiteDragon);
            }
            else
            {
                AI.SelectNextCard((int)CardId.DragonSpiritOfWhite);
            }
            return true;
        }

        private bool DragonSpiritOfWhiteEffect()
        {
            if (ActivateDescription == -1)
            {
                ClientCard target = AI.Utils.GetBestEnemySpell();
                AI.SelectCard(target);
                return true;
            }
            else
            {
                if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                {
                    return HaveEnoughWhiteDragonInHand() && Card.Attacked;
                }
                if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                {
                    return HaveEnoughWhiteDragonInHand()
                        && Bot.HasInMonstersZone((int)CardId.AzureEyesSilverDragon, true)
                        && !Bot.HasInGraveyard((int)CardId.DragonSpiritOfWhite)
                        && !Bot.HasInGraveyard((int)CardId.WhiteDragon);
                }
                if (AI.Utils.IsChainTarget(Card))
                {
                    return HaveEnoughWhiteDragonInHand();
                }
                return false;
            }
        }

        private bool BlueEyesSpiritDragonEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == AI.Utils.GetStringId((int)CardId.BlueEyesSpiritDragon, 0))
            {
                return LastChainPlayer == 1;
            }
            else if (Duel.Player == 1 && (Duel.Phase == DuelPhase.BattleStart || Duel.Phase == DuelPhase.End))
            {
                AI.SelectCard((int)CardId.AzureEyesSilverDragon);
                return true;
            }
            else
            {
                if (AI.Utils.IsChainTarget(Card))
                {
                    AI.SelectCard((int)CardId.AzureEyesSilverDragon);
                    return true;
                }
                return false;
            }
        }

        private bool HopeHarbingerDragonTitanicGalaxyEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == AI.Utils.GetStringId((int)CardId.HopeHarbingerDragonTitanicGalaxy, 0))
            {
                return LastChainPlayer == 1;
            }
            return true;
        }

        private bool WhiteStoneOfAncientsEffect()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.WhiteStoneOfAncients, 0))
            {
                if (Bot.HasInHand((int)CardId.TradeIn)
                    && !Bot.HasInHand((int)CardId.WhiteDragon)
                    && !Bot.HasInHand((int)CardId.AlternativeWhiteDragon))
                {
                    AI.SelectCard((int)CardId.WhiteDragon);
                    return true;
                }
                if (AlternativeWhiteDragonSummoned)
                {
                    return false;
                }
                if (Bot.HasInHand((int)CardId.WhiteDragon)
                    && !Bot.HasInHand((int)CardId.AlternativeWhiteDragon)
                    && Bot.HasInGraveyard((int)CardId.AlternativeWhiteDragon))
                {
                    AI.SelectCard((int)CardId.AlternativeWhiteDragon);
                    return true;
                }
                if (Bot.HasInHand((int)CardId.AlternativeWhiteDragon)
                    && !Bot.HasInHand((int)CardId.WhiteDragon)
                    && Bot.HasInGraveyard((int)CardId.WhiteDragon))
                {
                    AI.SelectCard((int)CardId.WhiteDragon);
                    return true;
                }
                return false;
            }
            else
            {
                List<ClientCard> spells = Enemy.GetSpells();
                if (spells.Count == 0)
                {
                    AI.SelectCard((int)CardId.WhiteDragon);
                }
                else
                {
                    AI.SelectCard((int)CardId.DragonSpiritOfWhite);
                }
                return true;
            }
        }

        private bool AlternativeWhiteDragonSummon()
        {
            AlternativeWhiteDragonSummoned = true;
            return true;
        }

        private bool WhiteStoneSummon()
        {
            return Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.SageWithEyesOfBlue,
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.AlternativeWhiteDragon,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite
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
            if (AI.Utils.IsOneEnemyBetterThanValue(2999, false))
            {
                return true;
            }
            return false;
        }

        private bool GalaxyEyesFullArmorPhotonDragonSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.GalaxyEyesCipherDragon))
            {
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if ((monster.IsDisabled() && monster.HasType(CardType.Xyz) && !monster.Equals(UsedGalaxyEyesCipherDragon))
                        || (Duel.Phase == DuelPhase.Main2 && monster.Equals(UsedGalaxyEyesCipherDragon)))
                    {
                        AI.SelectCard(monster);
                        return true;
                    }
                }
            }
            if (Bot.HasInMonstersZone((int)CardId.GalaxyEyesPrimePhotonDragon))
            {
                if (!AI.Utils.IsOneEnemyBetterThanValue(4000, false))
                {
                    AI.SelectCard((int)CardId.GalaxyEyesPrimePhotonDragon);
                    return true;
                }
            }
            return false;
        }

        private bool GalaxyEyesCipherBladeDragonSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.GalaxyEyesFullArmorPhotonDragon) && AI.Utils.GetProblematicEnemyCard() != null)
            {
                AI.SelectCard((int)CardId.GalaxyEyesFullArmorPhotonDragon);
                return true;
            }
            return false;
        }

        private bool GalaxyEyesDarkMatterDragonSummon()
        {
            if (Bot.HasInMonstersZone((int)CardId.GalaxyEyesFullArmorPhotonDragon))
            {
                AI.SelectCard((int)CardId.GalaxyEyesFullArmorPhotonDragon);
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
            ClientCard target = AI.Utils.GetProblematicEnemySpell();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            target = AI.Utils.GetProblematicEnemyMonster();
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            List<ClientCard> spells = Enemy.GetSpells();
            foreach (ClientCard spell in spells)
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
            ClientCard target = AI.Utils.GetProblematicEnemyCard();
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
            AI.SelectCard(new[]
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.WhiteDragon
                });
            AI.SelectNextCard(new[]
                {
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.WhiteDragon
                });
            return true;
        }

        private bool GiganticastleSummon()
        {
            if (Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1 || SoulChargeUsed)
                return false;
            int bestSelfAttack = AI.Utils.GetBestAttack(Bot);
            int bestEnemyAttack = AI.Utils.GetBestPower(Enemy);
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
            if (Duel.Phase == DuelPhase.Main1 && !Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.AlternativeWhiteDragon,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite
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
            AI.SelectCard(new[]
                {
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.WhiteStoneOfAncients
                });
            return true;
        }

        private bool SoulChargeEffect()
        {
            if (Bot.HasInMonstersZone((int)CardId.BlueEyesSpiritDragon, true))
                return false;
            int count = Bot.GetGraveyardMonsters().Count;
            int space = 5 - Bot.GetMonsterCount();
            if (count < space)
                count = space;
            if (count < 2 || Duel.LifePoints[0] < count*1000)
                return false;
            if (Duel.Turn != 1)
            {
                int attack = 0;
                int defence = 0;
                List<ClientCard> monsters = Bot.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    if (!monster.IsDefense())
                    {
                        attack += monster.Attack;
                    }
                }
                monsters = Enemy.GetMonsters();
                foreach (ClientCard monster in monsters)
                {
                    defence += monster.GetDefensePower();
                }
                if (attack - defence > Duel.LifePoints[1])
                    return false;
            }
            AI.SelectCard(new[]
                {
                    (int)CardId.BlueEyesSpiritDragon,
                    (int)CardId.HopeHarbingerDragonTitanicGalaxy,
                    (int)CardId.AlternativeWhiteDragon,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite,
                    (int)CardId.AzureEyesSilverDragon,
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend
                });
            SoulChargeUsed = true;
            return true;
        }

        private bool Repos()
        {
            bool enemyBetter = AI.Utils.IsAllEnemyBetter(true);

            if (Card.IsAttack() && enemyBetter)
                return true;
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense() && !enemyBetter && Card.Attack >= Card.Defense)
                return true;
            if (Card.IsDefense() && (
                   Card.Id == (int)CardId.BlueEyesSpiritDragon
                || Card.Id == (int)CardId.AzureEyesSilverDragon
                ))
                return true;
            if (Card.IsAttack() && (
                   Card.Id == (int)CardId.SageWithEyesOfBlue
                || Card.Id == (int)CardId.WhiteStoneOfAncients
                || Card.Id == (int)CardId.WhiteStoneOfLegend
                ))
                return true;
            return false;
        }

        private bool SpellSet()
        {
            return (Card.IsTrap() || (Card.Id==(int)CardId.SilversCry)) && Bot.GetSpellCountWithoutField() < 4;
        }

        private bool HasTwoInHand(int id)
        {
            int num = 0;
            foreach (ClientCard card in Bot.Hand)
            {
                if (card != null && card.Id == id)
                    num++;
            }
            return num >= 2;
        }

        private bool CanDealWithUsedAlternativeWhiteDragon()
        {
            return Bot.HasInMonstersZone(new List<int>
                {
                    (int)CardId.SageWithEyesOfBlue,
                    (int)CardId.WhiteStoneOfAncients,
                    (int)CardId.WhiteStoneOfLegend,
                    (int)CardId.WhiteDragon,
                    (int)CardId.DragonSpiritOfWhite
                }) || Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.AlternativeWhiteDragon)>=2 ;
        }

        private bool HaveEnoughWhiteDragonInHand()
        {
            return HasTwoInHand((int)CardId.WhiteDragon) || (
                Bot.HasInGraveyard((int)CardId.WhiteDragon)
                && Bot.HasInGraveyard((int)CardId.WhiteStoneOfAncients)
                );
        }
    }
}
