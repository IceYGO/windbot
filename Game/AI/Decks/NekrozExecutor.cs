using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Nekroz", "AI_Nekroz")]
    public class NekrozExecutor : DefaultExecutor
    {  
        public enum CardId
        {
            DancePrincess = 52738610,
            ThousandHands = 23401839,
            TenThousandHands = 95492061,
            Shurit = 90307777,
            MaxxC = 23434538,
            DecisiveArmor = 88240999,
            Trishula = 52068432,
            Valkyrus = 25857246,
            Gungnir = 74122412,
            Brionac = 26674724,
            Unicore = 89463537,
            Clausolas = 99185129,
            PhantomOfChaos = 30312361,

            DarkHole = 53129443,
            ReinforcementOfTheArmy = 32807846,
            TradeIn = 38120068,
            PreparationOfRites = 96729612,
            Mirror = 14735698,
            Kaleidoscope = 51124303,
            Cycle = 97211663,
            MysticalSpaceTyphoon = 5318639,
            RoyalDecree = 51452091,
            EvilswarmExcitonKnight = 46772449,
            HeraldOfTheArcLight = 79606837
        }

        List<int> NekrozRituelCard = new List<int>();
        List<int> NekrozSpellCard = new List<int>();

        public NekrozExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            NekrozRituelCard.Add((int)CardId.Clausolas); 
            NekrozRituelCard.Add((int)CardId.Unicore);
            NekrozRituelCard.Add((int)CardId.DecisiveArmor);
            NekrozRituelCard.Add((int)CardId.Brionac);
            NekrozRituelCard.Add((int)CardId.Trishula);
            NekrozRituelCard.Add((int)CardId.Gungnir);
            NekrozRituelCard.Add((int)CardId.Valkyrus);

            NekrozSpellCard.Add((int)CardId.Mirror);
            NekrozSpellCard.Add((int)CardId.Kaleidoscope);
            NekrozSpellCard.Add((int)CardId.Cycle);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.ReinforcementOfTheArmy, ReinforcementOfTheArmyEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TradeIn);
            AddExecutor(ExecutorType.Activate, (int)CardId.PreparationOfRites);
            AddExecutor(ExecutorType.Activate, (int)CardId.Mirror);
            AddExecutor(ExecutorType.Activate, (int)CardId.Kaleidoscope);
            AddExecutor(ExecutorType.Activate, (int)CardId.Cycle);
            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.RoyalDecree);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.DancePrincess, DancePrincessSummon);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.Shurit, ShuritSet);
            AddExecutor(ExecutorType.Summon, (int)CardId.ThousandHands, ThousandHandsSummom);
            AddExecutor(ExecutorType.Summon, (int)CardId.TenThousandHands, TenThousandHandsSummon);
            AddExecutor(ExecutorType.Summon, (int)CardId.PhantomOfChaos, PhantomOfChaosSummon);

            AddExecutor(ExecutorType.Activate, (int)CardId.Unicore, UnicoreEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.DecisiveArmor, DecisiveArmorEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Valkyrus, ValkyrusEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Gungnir, GungnirEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Brionac, BrionacEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Clausolas, ClausolasEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.Trishula);
            AddExecutor(ExecutorType.Activate, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.PhantomOfChaos, PhantomOfChaosEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.MaxxC);
            AddExecutor(ExecutorType.Activate, (int)CardId.ThousandHands, ThousandHandsEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.TenThousandHands, BrionacEffect);
            AddExecutor(ExecutorType.Activate, (int)CardId.HeraldOfTheArcLight);
            AddExecutor(ExecutorType.Activate, (int)CardId.Shurit);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.Trishula);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.DecisiveArmor);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Valkyrus);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Gungnir);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Brionac);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Unicore);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.Clausolas);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.EvilswarmExcitonKnight, EvilswarmExcitonKnightSummon);
        }

        private bool ThousandHandsSummom()
        {
            if (!Bot.HasInHand(NekrozRituelCard) || Bot.HasInHand((int)CardId.Shurit) || !Bot.HasInHand(NekrozSpellCard))  
                return true;
            foreach (ClientCard Card in Bot.Hand)
                if (Card != null && Card.Id == (int)CardId.Kaleidoscope && !Bot.HasInHand((int)CardId.Unicore))
                    return true;
                else if (Card.Id == (int)CardId.Trishula || Card.Id == (int)CardId.DecisiveArmor && !Bot.HasInHand((int)CardId.Mirror) || !Bot.HasInHand((int)CardId.Shurit))
                    return true;
            return false;
        }

        private bool ReinforcementOfTheArmyEffect()
        {
            if (!Bot.HasInGraveyard((int)CardId.Shurit) && !Bot.HasInHand((int)CardId.Shurit))
            {
                AI.SelectCard((int)CardId.Shurit);
                return true;
            }
            return false;
        }

        private bool TenThousandHandsSummon()
        {
                if (!Bot.HasInHand((int)CardId.ThousandHands) || !Bot.HasInHand((int)CardId.Shurit))
                return true;
            return false;
        }

        private bool DancePrincessSummon()
        {
            if (!Bot.HasInHand((int)CardId.ThousandHands) && !Bot.HasInHand((int)CardId.TenThousandHands))
                return true;
            return false;
        }

        private bool PhantomOfChaosSummon()
        {
            if (Bot.HasInGraveyard((int)CardId.Shurit) && Bot.HasInHand(NekrozSpellCard) && Bot.HasInHand(NekrozRituelCard))
                return true;
            return false;
        }

        private bool PhantomOfChaosEffect()
        {
            AI.SelectCard((int)CardId.Shurit);
            return true;
        }

        private bool ShuritSet()
        {
            if (!Bot.HasInHand((int)CardId.ThousandHands) && !Bot.HasInHand((int)CardId.TenThousandHands) && !Bot.HasInHand((int)CardId.DancePrincess))
                return true;
            return false;
        }

        private bool DecisiveArmorEffect()
        {
            if (AI.Utils.IsAllEnemyBetterThanValue(3300, true))
            {
                AI.SelectCard((int)CardId.DecisiveArmor);
                return true;
            }
            return false;
        }

        private bool EvilswarmExcitonKnightSummon()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount() + Bot.GetHandCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount() + Enemy.GetHandCount();
            return (selfCount - 1 < oppoCount) && EvilswarmExcitonKnightEffect();
        }

        private bool EvilswarmExcitonKnightEffect()
        {
            int selfCount = Bot.GetMonsterCount() + Bot.GetSpellCount();
            int oppoCount = Enemy.GetMonsterCount() + Enemy.GetSpellCount();
            return selfCount < oppoCount;
        }

        private bool ValkyrusEffect()
        {
            if (Duel.Phase == DuelPhase.Battle)
                return true;
            return false;
        }

        private bool GungnirEffect()
        {           
            if (AI.Utils.IsEnemyBetter(true, false) && Duel.Phase == DuelPhase.Main1)
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool BrionacEffect()
        {
            if (!Bot.HasInHand((int)CardId.Shurit))
            {
                AI.SelectCard((int)CardId.Shurit);
                return true;
            }
            else if (!Bot.HasInHand(NekrozSpellCard))
            {
                AI.SelectCard((int)CardId.Mirror);
                return true;
            }
            else if (AI.Utils.IsOneEnemyBetterThanValue(3300, true) && !Bot.HasInHand((int)CardId.Trishula))
            {
                AI.SelectCard((int)CardId.Trishula);
                return true;
            }
            else if (AI.Utils.IsAllEnemyBetterThanValue(2700,true) && !Bot.HasInHand((int)CardId.DecisiveArmor))
            {
                AI.SelectCard((int)CardId.DecisiveArmor);
                return true;
            }
            else if (Bot.HasInHand((int)CardId.Unicore) && !Bot.HasInHand((int)CardId.Kaleidoscope))
            {
                AI.SelectCard((int)CardId.Kaleidoscope);
                return true;
            }
            else if (!Bot.HasInHand((int)CardId.Unicore) && Bot.HasInHand((int)CardId.Kaleidoscope))
            {
                AI.SelectCard((int)CardId.Unicore);
                return true;
            }
            return true;
        }

        private bool ThousandHandsEffect()
        {
            if (AI.Utils.IsOneEnemyBetterThanValue(3300, true) && !Bot.HasInHand((int)CardId.Trishula))
            {
                AI.SelectCard((int)CardId.Trishula);
                return true;
            }
            else if (AI.Utils.IsAllEnemyBetterThanValue(2700, true) && !Bot.HasInHand((int)CardId.DecisiveArmor))
            {
                AI.SelectCard((int)CardId.DecisiveArmor);
                return true;
            }
            else if (!Bot.HasInHand((int)CardId.Unicore) && Bot.HasInHand((int)CardId.Kaleidoscope))
            {
                AI.SelectCard((int)CardId.Unicore);
                return true;
            }
            return true;
        }

        private bool UnicoreEffect()
        {
            if (Bot.HasInGraveyard((int)CardId.Shurit))
            {
                AI.SelectCard((int)CardId.Shurit);
                return true;
            }
            return false;
        }

        private bool ClausolasEffect()
        {
            if (!Bot.HasInHand(NekrozSpellCard))
            {
                AI.SelectCard((int)CardId.Mirror);
                return true;
            }
            return false;
        }

        private bool IsTheLastPossibility()
        {
            if (!Bot.HasInHand((int)CardId.DecisiveArmor) && !Bot.HasInHand((int)CardId.Trishula))
                return true;
            return false;
        }

        private bool SelectNekrozWhoInvoke()
        {
            List<int> NekrozCard = new List<int>();
            try
            {
                foreach (ClientCard Card in Bot.Hand)
                    if (Card != null && NekrozRituelCard.Contains((int)Card.Id))
                        NekrozCard.Add(Card.Id);

                foreach (int Id in NekrozCard)
                {
                    if (Id == (int)CardId.Trishula && AI.Utils.IsAllEnemyBetterThanValue(2700, true) && Bot.HasInHand((int)CardId.DecisiveArmor))
                    {
                        AI.SelectCard((int)CardId.Trishula);
                        return true;
                    }
                    else if (Id == (int)CardId.DecisiveArmor)
                    {
                        AI.SelectCard((int)CardId.DecisiveArmor);
                        return true;
                    }
                    else if (Id == (int)CardId.Unicore && Bot.HasInHand((int)CardId.Kaleidoscope) && !Bot.HasInGraveyard((int)CardId.Shurit))
                    {
                        AI.SelectCard((int)CardId.Unicore);
                        return true;
                    }
                    else if (Id == (int)CardId.Valkyrus)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.Valkyrus);
                            return true;
                        }
                    }
                    else if (Id == (int)CardId.Gungnir)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.Gungnir);
                            return true;
                        }
                    }
                    else if (Id == (int)CardId.Clausolas)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.Clausolas);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            { return false; }
        }
    }
}
