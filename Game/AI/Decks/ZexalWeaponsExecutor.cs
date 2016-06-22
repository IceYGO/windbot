using OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot.Game;
using WindBot.Game.AI;

namespace DevBot.Game.AI.Decks
{
    [Deck("Zexal Weapons", "AI_ZexalWeapons")]
    class ZexalWeaponsExecutor : DefaultExecutor
    {
        public enum CardId
        {
            CyberDragon = 70095155,
            ZwTornadoBringer = 81471108,
            ZwLightningBlade = 45082499,
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

            FlameSwordsman = 45231177,
            DarkfireDragon = 17881964,
            GaiaDragonTheThunderCharger = 91949988,
            ZwLionArms = 60992364,
            AdreusKeeperOfArmageddon = 94119480,
            Number61Volcasaurus = 29669359,
            GemKnightPearl = 71594310,
            Number39Utopia = 84013237,
            MaestrokeTheSymphonyDjinn = 25341652,
            GagagaCowboy = 12014404
        }

        public ZexalWeaponsExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Quick spells
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);

            // XYZ summons
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number39Utopia);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.ZwLionArms);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Number61Volcasaurus);

            // XYZ effects
            AddExecutor(ExecutorType.Activate, (int)CardId.Number39Utopia, Number39Utopia);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwLionArms, ZwLionArms);
            AddExecutor(ExecutorType.Activate, (int)CardId.AdreusKeeperOfArmageddon);
            AddExecutor(ExecutorType.Activate, (int)CardId.Number61Volcasaurus);

            // Weapons
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwTornadoBringer);
            AddExecutor(ExecutorType.Activate, (int)CardId.ZwLightningBlade);

            // Spell cards
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmy);

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

            // Reposition
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            // Set and activate traps
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.BreakthroughSkill, BreakthroughSkill);
            AddExecutor(ExecutorType.Activate, (int)CardId.SolemnWarning, DefaultTrap);
        }

        public override bool OnSelectHand()
        {
            return false;
        }

        private bool Number39Utopia()
        {
            if (!HasChainedTrap(0) && Duel.Player == 1 && Duel.Phase == DuelPhase.BattleStart && Card.Overlays.Count > 1)
                return true;
            return false;
        }

        private bool ZwLionArms()
        {
            if (ActivateDescription == (int)CardId.ZwLionArms * 16 + 0)
                return true;
            if (ActivateDescription == (int)CardId.ZwLionArms * 16 + 1)
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
            List<ClientCard> monsters = Duel.Fields[0].GetMonsters();
            int count = 0;
            foreach (ClientCard card in monsters)
            {
                if (card.Level == 5)
                    ++count;
            }
            if (count == 1)
                AI.SelectCard((int)CardId.FlameSwordsman);
            else
                AI.SelectCard((int)CardId.DarkfireDragon);
            return true;
        }

        private bool GoblindberghFirst()
        {
            IList<ClientCard> hand = Duel.Fields[0].Hand;
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
            if (Duel.Fields[0].HasInHand((int)CardId.InstantFusion) ||
                Duel.Fields[0].HasInHand((int)CardId.MysticalSpaceTyphoon))
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
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool BreakthroughSkill()
        {
            return (CurrentChain.Count > 0 && DefaultTrap());
        }
    }
}
