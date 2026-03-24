using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("AI_GOATFormat", "AI_GOATFormat")]
    public class GoatFormatExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Tsukuyomi = 70781052;
            public const int BlackLusterSoldier_EnvoyOfTheBeginning = 72302403;
            public const int Jinzo = 83968380;
            public const int AirknightParsath = 13039848;
            public const int ThunderDragon = 69140098;
            public const int KycooTheGhostDestroyer = 39256679;
            public const int BladeKnight = 11549357;
            public const int BreakerTheMagicalWarrior = 71413901;
            public const int SkilledDarkMage = 40659562;
            public const int SkilledWhiteMage = 77585513;
            public const int AbyssSoldier = 40133511;
            public const int AsuraPriest = 14531242;
            public const int ExiledForce = 15025844;
            public const int DDMasterOfPuppets = 11091375;
            public const int MagicianOfFaith = 88240808;
            public const int Sangan = 7572887;
            public const int SinisterSerpent = 74131780;
            public const int MorphingJar = 33508719; // ID 33508719
            
            // Spells
            public const int PotOfGreed = 55144522;
            public const int GracefulCharity = 79571449;
            public const int DelinquentDuo = 44095762;
            public const int HeavyStorm = 19613556;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int SnatchSteal = 45986603; // Corrected ID 45986603
            public const int NoblemanOfCrossout = 70828912;
            public const int Metamorphosis = 40619825;
            public const int Scapegoat = 70368879;
            public const int PrematureBurial = 70828912;
            
            // Traps
            public const int MirrorForce = 44095762;
            public const int TorrentialTribute = 53582587;
            public const int RingOfDestruction = 83555667;
            public const int CallOfTheHaunted = 97077563;
            public const int DustTornado = 60082869;
            public const int SakuretsuArmor = 56747793;
            public const int Ceasefire = 36468556; // 14315573
        }

        public GoatFormatExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Priority 1: Game-Winning Spells
            AddExecutor(ExecutorType.Activate, CardId.PotOfGreed);
            AddExecutor(ExecutorType.Activate, CardId.GracefulCharity, DiscardSerpentLogic);
            AddExecutor(ExecutorType.Activate, CardId.DelinquentDuo);

            // Priority 2: Board Clears
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, DefaultHeavyStorm);
            AddExecutor(ExecutorType.Activate, CardId.NoblemanOfCrossout, DefaultNoblemanOfCrossout);
            AddExecutor(ExecutorType.Activate, CardId.ExiledForce, DefaultExiledForce);

            // Priority 3: Special Summons
            AddExecutor(ExecutorType.SpSummon, CardId.BlackLusterSoldier_EnvoyOfTheBeginning, BLSSummon);
            AddExecutor(ExecutorType.Activate, CardId.BlackLusterSoldier_EnvoyOfTheBeginning, BLSEffect);
            AddExecutor(ExecutorType.Activate, CardId.Metamorphosis, MetaLogic);

            // Priority 4: Deck Thinning & Utility
            AddExecutor(ExecutorType.Activate, CardId.ThunderDragon);
            AddExecutor(ExecutorType.Summon, CardId.BreakerTheMagicalWarrior, DefaultBreakerTheMagicalWarrior);
            AddExecutor(ExecutorType.Summon, CardId.Jinzo);

            // Priority 5: Normal Summons
            AddExecutor(ExecutorType.Summon, CardId.BladeKnight);
            AddExecutor(ExecutorType.Summon, CardId.KycooTheGhostDestroyer);
            AddExecutor(ExecutorType.Summon, CardId.AbyssSoldier);
            AddExecutor(ExecutorType.MonsterSet, CardId.MagicianOfFaith);
            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);
            
            // Priority 6: Traps
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, CardId.RingOfDestruction, DefaultRingOfDestruction);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
        }

        private bool DiscardSerpentLogic()
        {
            // Discard Sinister Serpent (74131780) or Thunder Dragon (69140098) first
            AI.SelectCard(new[] { CardId.SinisterSerpent, CardId.ThunderDragon });
            return true;
        }

        private bool BLSSummon()
        {
            return Bot.Graveyard.GetMatchingCardsCount(c => c.HasAttribute(CardAttribute.Light)) >= 1 
                && Bot.Graveyard.GetMatchingCardsCount(c => c.HasAttribute(CardAttribute.Dark)) >= 1;
        }

        private bool BLSEffect()
        {
            ClientCard target = Enemy.GetMonsters().GetHighestAttackMonster();
            if (target != null && target.Attack >= 2400) {
                AI.SelectOption(0); // Banish
                AI.SelectCard(target);
                return true;
            }
            return true; // Attack twice
        }

        private bool MetaLogic()
        {
            // Use Metamorphosis on Scapegoat tokens (70368879) for Thousand-Eyes Restrict
            return Bot.HasInMonstersZone(CardId.Scapegoat);
        }
    }
}
