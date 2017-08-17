using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Zexal Weapons", "AI_ZexalWeapons")]
    class ZexalWeaponsExecutor : DefaultExecutor
    {
        public enum CardId
        {
            CyberDragon = 70095155,
            ZwTornadoBringer = 81471108,
            ZwLightningBlade = 45082499,
            ZwAsuraStrike = 40941889,
            SolarWindJammer = 33911264,
            PhotonTrasher = 65367484,
            StarDrawing = 24610207,
            SacredCrane = 30914564,
            Goblindbergh = 25259669,
            Honest = 37742478,
            Kagetokage = 94656263,
            HeroicChallengerExtraSword = 34143852,
            TinGoldfish = 18063928,
            SummonerMonk = 423585,
            InstantFusion = 1845204,
            Raigeki = 12580477,
            ReinforcementOfTheArmy = 32807846,
            DarkHole = 53129443,
            MysticalSpaceTyphoon = 5318639,
            BreakthroughSkill = 78474168,
            SolemnWarning = 84749824,
            SolemnStrike = 40605147,
            XyzChangeTactics = 11705261,

            FlameSwordsman = 45231177,
            DarkfireDragon = 17881964,
            GaiaDragonTheThunderCharger = 91949988,
            ZwLionArms = 60992364,
            AdreusKeeperOfArmageddon = 94119480,
            Number61Volcasaurus = 29669359,
            GemKnightPearl = 71594310,
            Number39Utopia = 84013237,
            NumberS39UtopiaOne= 86532744,
            NumberS39UtopiatheLightning = 56832966,
            MaestrokeTheSymphonyDjinn = 25341652,
            GagagaCowboy = 12014404
        }

        public ZexalWeaponsExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);

            // Spell cards
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmy);
            AddExecutor(ExecutorType.Activate, (int)CardId.XyzChangeTactics, XyzChangeTactics);

            // XYZ summons
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number61Volcasaurus, Number61Volcasaurus);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ZwLionArms);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.AdreusKeeperOfArmageddon);

            // XYZ effects
            AddExecutor(ExecutorType.Activate, (int)CardId.Number39Utopia, Number39Utopia);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiaOne);
            AddExecutor(ExecutorType.Activate, (int)CardId.NumberS39UtopiatheLightning, NumberS39UtopiatheLightning);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwLionArms, ZwLionArms);
            AddExecutor(ExecutorType.Activate, (int)CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number61Volcasaurus);

            // Weapons
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwTornadoBringer);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwLightningBlade);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwAsuraStrike);


            // Special summons
            AddExecutor(ExecutorType.SpSummon, (int)CardId.PhotonTrasher);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.SolarWindJammer, SolarWindJammer);

            AddExecutor(ExecutorType.Activate, (int)CardId.InstantFusion, InstantFusion);

            // Normal summons
            AddExecutor(ExecutorType.Summon, (int)CardId.Goblindbergh, GoblindberghFirst);
            AddExecutor(ExecutorType.Summon, (int)CardId.TinGoldfish, GoblindberghFirst);
            AddExecutor(ExecutorType.Summon, (int)CardId.StarDrawing);
            AddExecutor(ExecutorType.Summon, (int)CardId.SacredCrane);
            AddExecutor(ExecutorType.Summon, (int)CardId.HeroicChallengerExtraSword);
            AddExecutor(ExecutorType.Summon, (int)CardId.Goblindbergh);
            AddExecutor(ExecutorType.Summon, (int)CardId.TinGoldfish);
            AddExecutor(ExecutorType.Summon, (int)CardId.SummonerMonk);

            // Summons: Effects
            AddExecutor(ExecutorType.Activate, (int)CardId.Goblindbergh, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TinGoldfish, GoblindberghEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Kagetokage);
            AddExecutor(ExecutorType.Activate, (int)CardId.SummonerMonk, SummonerMonkEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Honest, Honest);

            // Reposition
            AddExecutor(ExecutorType.Repos, MonsterRepos);

            // Spummon GaiaDragonTheThunderCharger if Volcasaurus or ZwLionArms had been used
            AddExecutor(ExecutorType.SpSummon, (int)CardId.GaiaDragonTheThunderCharger);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.BreakthroughSkill, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnStrike, DefaultSolemnStrike);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible())
            {
                if (defender.IsMonsterDangerous() || defender.IsDefense())
                    return false;
            }
            if (!(defender.Id == (int)CardId.NumberS39UtopiatheLightning))
            {
                if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand((int)CardId.Honest))
                    attacker.RealPower = attacker.RealPower + defender.Attack;
                if (attacker.Id == (int)CardId.NumberS39UtopiatheLightning && !attacker.IsDisabled() && attacker.HasXyzMaterial(2, (int)CardId.Number39Utopia))
                    attacker.RealPower = 5000;
            }
            return attacker.RealPower > defender.GetDefensePower();
        }

        private bool Number39Utopia()
        {
            if (!HasChainedTrap(0) && Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Card.HasXyzMaterial(2))
                return true;
            return false;
        }

        private bool Number61Volcasaurus()
        {
            return AI.Utils.IsOneEnemyBetterThanValue(2000, false);
        }

        private bool ZwLionArms()
        {
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.ZwLionArms, 0))
                return true;
            if (ActivateDescription == AI.Utils.GetStringId((int)CardId.ZwLionArms, 1))
                return !Card.IsDisabled();
            return false;
        }

        private bool ReinforcementOfTheArmy()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.Goblindbergh,
                    (int)CardId.TinGoldfish,
                    (int)CardId.StarDrawing,
                    (int)CardId.Kagetokage,
                    (int)CardId.SacredCrane
                });
            return true;
        }

        private bool InstantFusion()
        {
            if (Duel.LifePoints[0] <= 1000)
                return false;
            List<ClientCard> monsters = Bot.GetMonsters();
            int count4 = 0;
            int count5 = 0;
            foreach (ClientCard card in monsters)
            {
                if (card.Level == 5)
                    ++count5;
                if (card.Level == 4)
                    ++count4;
            }
            if (count5 == 1)
            {
                AI.SelectCard((int)CardId.FlameSwordsman);
                return true;
            }
            else if (count4 == 1)
            {
                AI.SelectCard((int)CardId.DarkfireDragon);
                return true;
            }
            return false;
        }

        private bool XyzChangeTactics()
        {
            return Duel.LifePoints[0] > 500;
        }

        private bool NumberS39UtopiatheLightning()
        {
            return Card.Attack < 5000;
        }

        private bool Honest()
        {
            return Duel.Phase != DuelPhase.Main1 || Duel.Turn == 1;
        }

        private bool GoblindberghFirst()
        {
            IList<ClientCard> hand = Bot.Hand;
            foreach (ClientCard card in hand)
            {
                if (card != Card && card.IsMonster() && card.Level == 4)
                    return true;
            }
            return false;
        }

        private bool GoblindberghEffect()
        {
            AI.SelectCard(new[]
                {
                    (int)CardId.SacredCrane,
                    (int)CardId.HeroicChallengerExtraSword,
                    (int)CardId.StarDrawing,
                    (int)CardId.SummonerMonk
                });
            return true;
        }

        private bool SummonerMonkEffect()
        {
            if (Bot.HasInHand((int)CardId.InstantFusion) ||
                Bot.HasInHand((int)CardId.MysticalSpaceTyphoon))
            {
                AI.SelectCard(new[]
                    {
                        (int)CardId.InstantFusion,
                        (int)CardId.MysticalSpaceTyphoon
                    });
                return true;
            }
            AI.SelectNextCard(new[]
                {
                    (int)CardId.Goblindbergh,
                    (int)CardId.TinGoldfish,
                    (int)CardId.StarDrawing,
                    (int)CardId.Kagetokage,
                    (int)CardId.SacredCrane
                });
            return false;
        }

        private bool SolarWindJammer()
        {
            if (!Bot.HasInHand(new List<int> {
                    (int)CardId.StarDrawing,
                    (int)CardId.InstantFusion
                }))
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool MonsterRepos()
        {
            if (Card.Id == (int)CardId.NumberS39UtopiatheLightning)
                return false;
            return base.DefaultMonsterRepos();
        }
    }
}
