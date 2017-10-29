using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("CyberDragon", "AI_CyberDragon")]
    public class CyberDragonExecutor : DefaultExecutor
    {
        bool PowerBondUsed = false;

        public enum CardId
        {
            CyberLaserDragon = 4162088,
            CyberBarrierDragon = 68774379,
            CyberDragon = 70095154,
            CyberDragonDrei = 59281922,
            CyberPhoenix = 3370104,
            ArmoredCybern = 67159705,
            ProtoCyberDragon = 26439287,
            CyberKirin = 76986005,
            CyberDragonCore = 23893227,
            CyberValley = 3657444,
            Raigeki = 12580477,
            DarkHole = 53129443,
            DifferentDimensionCapsule = 11961740,
            Polymerization = 24094653,
            PowerBond = 37630732,
            EvolutionBurst = 52875873,
            PhotonGeneratorUnit = 66607691,
            DeFusion = 95286165,
            BottomlessTrapHole = 29401950,
            MirrorForce = 44095762,
            AttackReflectorUnit = 91989718,
            CyberneticHiddenTechnology = 92773018,
            CallOfTheHaunted = 97077563,
            SevenToolsOfTheBandit = 3819470,
            CyberTwinDragon = 74157028,
            CyberEndDragon = 1546123,
            CyberDragonNova = 58069384
        }

        public CyberDragonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, (int)CardId.DeFusion);

            AddExecutor(ExecutorType.Activate, (int)CardId.DifferentDimensionCapsule, Capsule);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.Polymerization, PolymerizationEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.PowerBond, PowerBondEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvolutionBurst, EvolutionBurstEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.PhotonGeneratorUnit);
            AddExecutor(ExecutorType.Activate, (int)CardId.DeFusion, DeFusionEffect);

            AddExecutor(ExecutorType.Activate, (int)CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.AttackReflectorUnit);
            AddExecutor(ExecutorType.Activate, (int)CardId.SevenToolsOfTheBandit, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.CallOfTheHaunted, DefaultCallOfTheHaunted);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.CyberDragonDrei, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.CyberPhoenix, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.CyberValley, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.CyberDragonCore, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.ArmoredCybern, ArmoredCybernSet);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.ProtoCyberDragon, ProtoCyberDragonSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.CyberKirin, CyberKirinSummon);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberEndDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberTwinDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberBarrierDragon);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.CyberLaserDragon);

            AddExecutor(ExecutorType.Activate, (int)CardId.CyberBarrierDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberLaserDragon);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberDragonDrei);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberPhoenix);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberKirin);
            AddExecutor(ExecutorType.Activate, (int)CardId.ArmoredCybern, ArmoredCybernEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.CyberValley);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool CyberDragonInHand()  { return Bot.HasInHand((int)CardId.CyberDragon); }
        private bool CyberDragonInGraveyard()  { return Bot.HasInGraveyard((int)CardId.CyberDragon); }
        private bool CyberDragonInMonsterZone() { return Bot.HasInMonstersZone((int)CardId.CyberDragon); }
        private bool CyberDragonIsBanished() { return Bot.HasInBanished((int)CardId.CyberDragon); }

        private bool Capsule()
        {
            List<int> SelectedCard = new List<int>();
            SelectedCard.Add((int)CardId.PowerBond);
            SelectedCard.Add((int)CardId.DarkHole);
            SelectedCard.Add((int)CardId.Raigeki);
            AI.SelectCard(SelectedCard);
            return true;
        }

        private bool PolymerizationEffect()
        {
            if (Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.ProtoCyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.CyberDragon) >= 3)
                AI.SelectCard((int)CardId.CyberEndDragon);
            else
                AI.SelectCard((int)CardId.CyberTwinDragon);
            return true;
        }

        private bool PowerBondEffect()
        {
            PowerBondUsed = true;
            if (Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.ProtoCyberDragon) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.CyberDragonCore) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.CyberDragonCore) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragonDrei) >= 3)
                AI.SelectCard((int)CardId.CyberEndDragon);
            else
                AI.SelectCard((int)CardId.CyberTwinDragon);
            return true;
        }

        private bool EvolutionBurstEffect()
        {
            ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
            if (bestMy == null || !AI.Utils.IsOneEnemyBetterThanValue(bestMy.Attack, false))
                return false;
            else
                AI.SelectCard(Enemy.MonsterZone.GetHighestAttackMonster());     
            return true;
        }

        private bool NoCyberDragonSpsummon()
        {
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0))
                return false;
            return true;
        }

        private bool ArmoredCybernSet()
        {
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand((int)CardId.CyberDragonDrei) || Bot.HasInHand((int)CardId.CyberPhoenix)) && !AI.Utils.IsOneEnemyBetterThanValue(1800,true))
                return false;
            return true;
        }

        private bool ProtoCyberDragonSummon()
        {
            if (Bot.GetCountCardInZone(Bot.Hand, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragonCore) >= 1 && Bot.HasInHand((int)CardId.Polymerization) || Bot.GetCountCardInZone(Bot.Hand, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.CyberDragonCore) >= 1 && Bot.HasInHand((int)CardId.PowerBond))
                return true;
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand((int)CardId.CyberDragonDrei) || Bot.HasInHand((int)CardId.CyberPhoenix)) && !AI.Utils.IsOneEnemyBetterThanValue(1800, true))
                return false;
            return true;
        }

        private bool CyberKirinSummon()
        {
            return PowerBondUsed;
        }

        private bool ArmoredCybernEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            else if (Card.Location == CardLocation.SpellZone)
            {
                if (AI.Utils.IsOneEnemyBetterThanValue(Bot.GetMonsters().GetHighestAttackMonster().Attack, true))
                    if (ActivateDescription == AI.Utils.GetStringId((int)CardId.ArmoredCybern, 2))
                        return true;
                return false;
            }
            return false;
        }

        private bool DeFusionEffect()
        {
            if (Duel.Phase == DuelPhase.Battle)
            {
                if (!Bot.HasAttackingMonster())
                    return true;
            }
            return false;
        }
    }
}