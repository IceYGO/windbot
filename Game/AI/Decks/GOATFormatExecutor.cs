using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("GOATFormat", "AI_GOATFormat", "Easy")]
    public class UltraBeatdownExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int SummonedSkull = 70781052;
            public const int Jinzo = 77585513;
            public const int LabyrinthWall = 67284908;
            public const int GuardianSphinx = 40659562;
            public const int LusterDragon = 11091375;
            public const int GeminiElf = 69140098;
            public const int KycooTheGhostDestroyer = 88240808;
            public const int Opticlops = 14531242;
            public const int BreakerTheMagicalWarrior = 71413901;
            public const int BetaTheMagnetWarrior = 39256679;
            public const int GammaTheMagnetWarrior = 11549357;
            public const int BazooTheSoulEater = 40133511;
            public const int ExiledForce = 74131780;
            public const int ManEaterBug = 54652250;
            public const int MysticalElf = 15025844;
            public const int GiantSoldierOfStone = 13039848;
            public const int Sangan = 7572887;
            public const int SanganAlt = 130000045;

            // Spells
            public const int PotOfGreed = 55144522;
            public const int GracefulCharity = 79571449;
            public const int UpstartGoblin = 70368879;
            public const int HeavyStorm = 19613556;
            public const int MysticalSpaceTyphoon = 5318639;
            public const int UnitedWeStand = 56747793;
            public const int MagePower = 83746708;
            public const int AxeOfDespair = 40619825;
            public const int Fissure = 66788016;
            public const int SwordsOfRevealingLight = 72302403;
            public const int PrematureBurial = 70828912;

            // Traps
            public const int MirrorForce = 44095762;
            public const int TorrentialTribute = 53582587;
            public const int RingOfDestruction = 83555667;
            public const int CallOfTheHaunted = 83968380;
            public const int MagicCylinder = 97077563;
            public const int NegateAttack = 14315573;
            public const int DustTornado = 60082869;
        }

        public UltraBeatdownExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // --- DRAW & SEARCH LOGIC ---
            AddExecutor(ExecutorType.Activate, CardId.PotOfGreed, PotOfGreedLogic);
            AddExecutor(ExecutorType.Activate, CardId.GracefulCharity, GracefulCharityLogic);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin, () => Bot.Deck.Count > 5);
            AddExecutor(ExecutorType.Activate, CardId.Sangan, SanganSearchLogic);
            AddExecutor(ExecutorType.Activate, CardId.SanganAlt, SanganSearchLogic);

            // --- SPELL LOGIC (OFFENSIVE) ---
            AddExecutor(ExecutorType.Activate, CardId.HeavyStorm, HeavyStormLogic);
            AddExecutor(ExecutorType.Activate, CardId.Fissure, FissureLogic);
            AddExecutor(ExecutorType.Activate, CardId.SwordsOfRevealingLight, SwordsLogic);
            AddExecutor(ExecutorType.Activate, CardId.MysticalSpaceTyphoon, MSTLogic);
            AddExecutor(ExecutorType.Activate, CardId.PrematureBurial, PrematureBurialLogic);

            // --- EQUIPMENT LOGIC ---
            AddExecutor(ExecutorType.Activate, CardId.UnitedWeStand, UnitedWeStandLogic);
            AddExecutor(ExecutorType.Activate, CardId.MagePower, MagePowerLogic);
            AddExecutor(ExecutorType.Activate, CardId.AxeOfDespair, AxeOfDespairLogic);

            // --- TRAP LOGIC (PROTECTION) ---
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, MirrorForceLogic);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, TorrentialTributeLogic);
            AddExecutor(ExecutorType.Activate, CardId.RingOfDestruction, RingOfDestructionLogic);
            AddExecutor(ExecutorType.Activate, CardId.MagicCylinder, MagicCylinderLogic);
            AddExecutor(ExecutorType.Activate, CardId.NegateAttack, NegateAttackLogic);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, CallOfTheHauntedLogic);
            AddExecutor(ExecutorType.Activate, CardId.DustTornado, DustTornadoLogic);

            // --- MONSTER SUMMONS ---
            AddExecutor(ExecutorType.Summon, CardId.Jinzo, JinzoSummonLogic);
            AddExecutor(ExecutorType.Summon, CardId.BreakerTheMagicalWarrior, () => Enemy.GetSpellCount() > 0);
            AddExecutor(ExecutorType.Summon, CardId.SummonedSkull, TributeLogic);
            AddExecutor(ExecutorType.Summon, CardId.KycooTheGhostDestroyer);
            AddExecutor(ExecutorType.Summon, CardId.LusterDragon);
            AddExecutor(ExecutorType.Summon, CardId.GeminiElf);
            AddExecutor(ExecutorType.Summon, CardId.Opticlops);
            AddExecutor(ExecutorType.MonsterSet, CardId.GuardianSphinx);
            AddExecutor(ExecutorType.MonsterSet, CardId.Sangan);
            AddExecutor(ExecutorType.MonsterSet, CardId.SanganAlt);
            AddExecutor(ExecutorType.MonsterSet, CardId.ManEaterBug);
            AddExecutor(ExecutorType.MonsterSet, CardId.LabyrinthWall);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        // --- SPELL LOGIC METHODS ---

        private bool PotOfGreedLogic()
        {
            return Bot.Deck.Count > 2; // Prevent self-deck out
        }

        private bool GracefulCharityLogic()
        {
            // Discard Sangan or duplicate Normal monsters
            return true;
        }

        private bool HeavyStormLogic()
        {
            // Only use if opponent has 2+ cards or more than us
            if (Bot.HasInSpellZone(CardId.SwordsOfRevealingLight)) return false;
            return Enemy.GetSpellCount() >= 2 || (Enemy.GetSpellCount() > Bot.GetSpellCount());
        }

        private bool FissureLogic()
        {
            return Enemy.GetMonsterCount() > 0;
        }

        private bool SwordsLogic()
        {
            // Use if opponent has a stronger monster or we have 0 monsters
            ClientCard bestEnemy = Enemy.GetMonsters().GetHighestAttackMonster();
            ClientCard myBest = Bot.GetMonsters().GetHighestAttackMonster();
            return (bestEnemy != null && (myBest == null || bestEnemy.Attack > myBest.Attack)) || Bot.GetMonsterCount() == 0;
        }

        private bool MSTLogic()
        {
            // Targets only if opponent has backrow, prioritizes face-down
            return Enemy.GetSpellCount() > 0;
        }

        private bool PrematureBurialLogic()
        {
            if (Bot.LifePoints <= 800) return false;
            // Target high-impact monsters in GY
            return Bot.Graveyard.Any(m => m.Id == CardId.Jinzo || m.Id == CardId.SummonedSkull || m.Id == CardId.KycooTheGhostDestroyer);
        }

        private bool UnitedWeStandLogic()
        {
            // Only use if we have 2+ monsters to maximize boost
            return Bot.GetMonsterCount() >= 2 && DefaultAequip();
        }

        private bool MagePowerLogic()
        {
            // Only use if we have at least 2 spells/traps set
            return Bot.GetSpellCount() >= 2 && DefaultAequip();
        }

        private bool AxeOfDespairLogic()
        {
            return DefaultAequip();
        }

        // --- TRAP LOGIC METHODS ---

        private bool MirrorForceLogic()
        {
            if (Bot.HasInMonstersZone(CardId.Jinzo)) return false;
            // Save Mirror Force for when 2+ monsters attack, or a 2000+ ATK monster attacks
            return Duel.Phase == DuelPhase.BattleStart && (Enemy.GetMonsterCount() >= 2 || Enemy.GetMonsters().Any(m => m.Attack >= 2000));
        }

        private bool TorrentialTributeLogic()
        {
            if (Bot.HasInMonstersZone(CardId.Jinzo)) return false;
            // Activate only if it wipes more of theirs than ours
            return Enemy.GetMonsterCount() > Bot.GetMonsterCount();
        }

        private bool RingOfDestructionLogic()
        {
            if (Bot.HasInMonstersZone(CardId.Jinzo)) return false;
            ClientCard target = Enemy.GetMonsters().GetHighestAttackMonster();
            if (target == null) return false;
            // Don't kill ourselves, target 2000+ ATK monsters
            return Bot.LifePoints > target.Attack && target.Attack >= 2000;
        }

        private bool CallOfTheHauntedLogic()
        {
            // Activate in opponent's End Phase to bring back a boss
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.End)
                return Bot.Graveyard.Any(m => m.Id == CardId.Jinzo || m.Id == CardId.SummonedSkull);
            return false;
        }

        private bool MagicCylinderLogic()
        {
            if (Bot.HasInMonstersZone(CardId.Jinzo)) return false;
            // Use on attacks from 1800+ ATK monsters
            return Duel.Phase == DuelPhase.BattleStart && Enemy.GetMonsters().Any(m => m.Attack >= 1800);
        }

        private bool NegateAttackLogic()
        {
            if (Bot.HasInMonstersZone(CardId.Jinzo)) return false;
            // Use only if health is below 4000 or it's a game-ending attack
            return Bot.LifePoints < 4000 || AI.Utils.IsOpponentBetterThanMe();
        }

        private bool DustTornadoLogic()
        {
            // Activate on opponent's turn to disrupt backrow
            return Duel.Player == 1 && Enemy.GetSpellCount() > 0;
        }

        // --- MONSTER SEARCH & SUMMON LOGIC ---

        private bool SanganSearchLogic()
        {
            if (Enemy.GetMonsterCount() > 0) AI.SelectCard(CardId.ExiledForce);
            else if (Bot.Graveyard.Count >= 3) AI.SelectCard(CardId.BazooTheSoulEater);
            else AI.SelectCard(CardId.LusterDragon);
            return true;
        }

        private bool JinzoSummonLogic()
        {
            if (Bot.GetMonsters().Any(m => m.EquipCards.Count > 0)) return false;
            return Enemy.GetSpellCount() > 0 || AI.Utils.IsOpponentBetterThanMe();
        }

        private bool TributeLogic()
        {
            return Bot.GetMonsters().Any(m => m.Attack < 1700);
        }
    }
}
