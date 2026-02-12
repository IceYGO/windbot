using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    // NOT FINISHED YET
    [Deck("CyberDragon", "AI_CyberDragon", "NotFinished")]
    public class CyberDragonExecutor : DefaultExecutor
    {
        bool PowerBondUsed = false;

        public class CardId
        {
            public const int CyberLaserDragon = 4162088;
            public const int CyberBarrierDragon = 68774379;
            public const int CyberDragon = 70095154;
            public const int CyberDragonDrei = 59281922;
            public const int CyberPhoenix = 3370104;
            public const int ArmoredCybern = 67159705;
            public const int ProtoCyberDragon = 26439287;
            public const int CyberKirin = 76986005;
            public const int CyberDragonCore = 23893227;
            public const int CyberValley = 3657444;
            public const int Raigeki = 12580477;
            public const int DarkHole = 53129443;
            public const int DifferentDimensionCapsule = 11961740;
            public const int Polymerization = 24094653;
            public const int PowerBond = 37630732;
            public const int EvolutionBurst = 52875873;
            public const int PhotonGeneratorUnit = 66607691;
            public const int DeFusion = 95286165;
            public const int BottomlessTrapHole = 29401950;
            public const int MirrorForce = 44095762;
            public const int AttackReflectorUnit = 91989718;
            public const int CyberneticHiddenTechnology = 92773018;
            public const int CallOfTheHaunted = 97077563;
            public const int SevenToolsOfTheBandit = 3819470;
            public const int CyberTwinDragon = 74157028;
            public const int CyberEndDragon = 1546123;
            public const int CyberDragonNova = 58069384;
        }

        public CyberDragonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, CardId.DeFusion);

            AddExecutor(ExecutorType.Activate, CardId.DifferentDimensionCapsule, Capsule);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.Polymerization, PolymerizationEffect);
            AddExecutor(ExecutorType.Activate, CardId.PowerBond, PowerBondEffect);
            AddExecutor(ExecutorType.Activate, CardId.EvolutionBurst, EvolutionBurstEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, CardId.PhotonGeneratorUnit);
            AddExecutor(ExecutorType.Activate, CardId.DeFusion, DeFusionEffect);

            AddExecutor(ExecutorType.Activate, CardId.BottomlessTrapHole, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.MirrorForce, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, CardId.AttackReflectorUnit);
            AddExecutor(ExecutorType.Activate, CardId.SevenToolsOfTheBandit, DefaultTrap);
            AddExecutor(ExecutorType.Activate, CardId.CallOfTheHaunted, DefaultCallOfTheHaunted);

            AddExecutor(ExecutorType.SummonOrSet, CardId.CyberDragonDrei, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.CyberPhoenix, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.Summon, CardId.CyberValley, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.CyberDragonCore, NoCyberDragonSpsummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.ArmoredCybern, ArmoredCybernSet);
            AddExecutor(ExecutorType.SummonOrSet, CardId.ProtoCyberDragon, ProtoCyberDragonSummon);
            AddExecutor(ExecutorType.Summon, CardId.CyberKirin, CyberKirinSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberEndDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberTwinDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberBarrierDragon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberLaserDragon);

            AddExecutor(ExecutorType.Activate, CardId.CyberBarrierDragon);
            AddExecutor(ExecutorType.Activate, CardId.CyberLaserDragon);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonDrei);
            AddExecutor(ExecutorType.Activate, CardId.CyberPhoenix);
            AddExecutor(ExecutorType.Activate, CardId.CyberKirin);
            AddExecutor(ExecutorType.Activate, CardId.ArmoredCybern, ArmoredCybernEffect);
            AddExecutor(ExecutorType.Activate, CardId.CyberValley);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool CyberDragonInHand()  { return Bot.HasInHand(CardId.CyberDragon); }
        private bool CyberDragonInGraveyard()  { return Bot.HasInGraveyard(CardId.CyberDragon); }
        private bool CyberDragonInMonsterZone() { return Bot.HasInMonstersZone(CardId.CyberDragon); }
        private bool CyberDragonIsBanished() { return Bot.HasInBanished(CardId.CyberDragon); }

        private bool Capsule()
        {
            IList<int> SelectedCard = new List<int>();
            SelectedCard.Add(CardId.PowerBond);
            SelectedCard.Add(CardId.DarkHole);
            SelectedCard.Add(CardId.Raigeki);
            AI.SelectCard(SelectedCard);
            return true;
        }

        private bool PolymerizationEffect()
        {
            if (Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.ProtoCyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.Hand, CardId.CyberDragon) >= 3)
                AI.SelectCard(CardId.CyberEndDragon);
            else
                AI.SelectCard(CardId.CyberTwinDragon);
            return true;
        }

        private bool PowerBondEffect()
        {
            PowerBondUsed = true;
            if (Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.ProtoCyberDragon) + Bot.GetCountCardInZone(Bot.Hand, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Hand, CardId.CyberDragonCore) + Bot.GetCountCardInZone(Bot.Graveyard, CardId.CyberDragonCore) + Bot.GetCountCardInZone(Bot.Graveyard, CardId.CyberDragonDrei) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragonDrei) >= 3)
                AI.SelectCard(CardId.CyberEndDragon);
            else
                AI.SelectCard(CardId.CyberTwinDragon);
            return true;
        }

        private bool EvolutionBurstEffect()
        {
            ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
            if (bestMy == null || !Util.IsOneEnemyBetterThanValue(bestMy.Attack, false))
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
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand(CardId.CyberDragonDrei) || Bot.HasInHand(CardId.CyberPhoenix)) && !Util.IsOneEnemyBetterThanValue(1800,true))
                return false;
            return true;
        }

        private bool ProtoCyberDragonSummon()
        {
            if (Bot.GetCountCardInZone(Bot.Hand, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragonCore) >= 1 && Bot.HasInHand(CardId.Polymerization) || Bot.GetCountCardInZone(Bot.Hand, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.MonsterZone, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, CardId.CyberDragon) + Bot.GetCountCardInZone(Bot.Graveyard, CardId.CyberDragonCore) >= 1 && Bot.HasInHand(CardId.PowerBond))
                return true;
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand(CardId.CyberDragonDrei) || Bot.HasInHand(CardId.CyberPhoenix)) && !Util.IsOneEnemyBetterThanValue(1800, true))
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
                if (Util.IsOneEnemyBetterThanValue(Bot.GetMonsters().GetHighestAttackMonster().Attack, true))
                    if (ActivateDescription == Util.GetStringId(CardId.ArmoredCybern, 2))
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