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
            影灵衣舞姬 = 52738610,
            千手神 = 23401839,
            万手神 = 95492061,
            影灵衣术士施里特 = 90307777,
            增殖的G = 23434538,
            决战兵器之影灵衣 = 88240999,
            三叉龙之影灵衣 = 52068432,
            瓦尔基鲁斯之影灵衣 = 25857246,
            天枪龙之影灵衣 = 74122412,
            光枪龙之影灵衣 = 26674724,
            尤尼科之影灵衣 = 89463537,
            辉剑鸟之影灵衣 = 99185129,
            混沌幻影 = 30312361,
            黑洞 = 53129443,
            增援 = 32807846,
            抵价购物 = 38120068,
            仪式的准备 = 96729612,
            影灵衣的降魔镜 = 14735698,
            影灵衣的万华镜 = 51124303,
            影灵衣的返魂术 = 97211663,
            旋风 = 5318639,
            王宫的通告 = 51452091,
            励辉士入魔蝇王 = 46772449,
            虹光之宣告者 = 79606837
        }

        List<int> NekrozRituelCard = new List<int>();
        List<int> NekrozSpellCard = new List<int>();

        public NekrozExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            NekrozRituelCard.Add((int)CardId.辉剑鸟之影灵衣); 
            NekrozRituelCard.Add((int)CardId.尤尼科之影灵衣);
            NekrozRituelCard.Add((int)CardId.决战兵器之影灵衣);
            NekrozRituelCard.Add((int)CardId.光枪龙之影灵衣);
            NekrozRituelCard.Add((int)CardId.三叉龙之影灵衣);
            NekrozRituelCard.Add((int)CardId.天枪龙之影灵衣);
            NekrozRituelCard.Add((int)CardId.瓦尔基鲁斯之影灵衣);

            NekrozSpellCard.Add((int)CardId.影灵衣的降魔镜);
            NekrozSpellCard.Add((int)CardId.影灵衣的万华镜);
            NekrozSpellCard.Add((int)CardId.影灵衣的返魂术);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);

            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.增援, 增援);
            AddExecutor(ExecutorType.Activate, (int)CardId.抵价购物);
            AddExecutor(ExecutorType.Activate, (int)CardId.仪式的准备);
            AddExecutor(ExecutorType.Activate, (int)CardId.影灵衣的降魔镜);
            AddExecutor(ExecutorType.Activate, (int)CardId.影灵衣的万华镜);
            AddExecutor(ExecutorType.Activate, (int)CardId.影灵衣的返魂术);
            AddExecutor(ExecutorType.Activate, (int)CardId.旋风, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.王宫的通告);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.影灵衣舞姬, 影灵衣舞姬召唤);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.影灵衣术士施里特, 影灵衣术士施里特);
            AddExecutor(ExecutorType.Summon, (int)CardId.千手神, 千手神召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.万手神, 万手神召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.混沌幻影, 混沌幻影);

            AddExecutor(ExecutorType.Activate, (int)CardId.尤尼科之影灵衣, 尤尼科之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.决战兵器之影灵衣, 决战兵器之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.瓦尔基鲁斯之影灵衣, 瓦尔基鲁斯之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.天枪龙之影灵衣, 天枪龙之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.光枪龙之影灵衣, 光枪龙之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.辉剑鸟之影灵衣, 辉剑鸟之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王);
            AddExecutor(ExecutorType.Activate, (int)CardId.混沌幻影, 混沌幻影效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.增殖的G);
            AddExecutor(ExecutorType.Activate, (int)CardId.千手神, 千手神效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.万手神, 光枪龙之影灵衣);
            AddExecutor(ExecutorType.Activate, (int)CardId.虹光之宣告者);
            AddExecutor(ExecutorType.Activate, (int)CardId.影灵衣术士施里特);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.三叉龙之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.决战兵器之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.瓦尔基鲁斯之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.天枪龙之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.光枪龙之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.尤尼科之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.辉剑鸟之影灵衣);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.励辉士入魔蝇王, 励辉士入魔蝇王);
        }

        private bool 千手神召唤()
        {
            if (!Duel.Fields[0].HasInHand(NekrozRituelCard) || Duel.Fields[0].HasInHand((int)CardId.影灵衣术士施里特) || !Duel.Fields[0].HasInHand(NekrozSpellCard))  
                return true;
            foreach (ClientCard Card in Duel.Fields[0].Hand)
                if (Card != null && Card.Id == (int)CardId.影灵衣的万华镜 && !Duel.Fields[0].HasInHand((int)CardId.尤尼科之影灵衣))
                    return true;
                else if (Card.Id == (int)CardId.三叉龙之影灵衣 || Card.Id == (int)CardId.决战兵器之影灵衣 && !Duel.Fields[0].HasInHand((int)CardId.影灵衣的降魔镜) || !Duel.Fields[0].HasInHand((int)CardId.影灵衣术士施里特))
                    return true;
            return false;
        }

        private bool 增援()
        {
            if (!Duel.Fields[0].HasInGraveyard((int)CardId.影灵衣术士施里特) && !Duel.Fields[0].HasInHand((int)CardId.影灵衣术士施里特))
            {
                AI.SelectCard((int)CardId.影灵衣术士施里特);
                return true;
            }
            return false;
        }

        private bool 万手神召唤()
        {
                if (!Duel.Fields[0].HasInHand((int)CardId.千手神) || !Duel.Fields[0].HasInHand((int)CardId.影灵衣术士施里特))
                return true;
            return false;
        }

        private bool 影灵衣舞姬召唤()
        {
            if (!Duel.Fields[0].HasInHand((int)CardId.千手神) && !Duel.Fields[0].HasInHand((int)CardId.万手神))
                return true;
            return false;
        }

        private bool 混沌幻影()
        {
            if (Duel.Fields[0].HasInGraveyard((int)CardId.影灵衣术士施里特) && Duel.Fields[0].HasInHand(NekrozSpellCard) && Duel.Fields[0].HasInHand(NekrozRituelCard))
                return true;
            return false;
        }

        private bool 混沌幻影效果()
        {
            AI.SelectCard((int)CardId.影灵衣术士施里特);
            return true;
        }

        private bool 影灵衣术士施里特()
        {
            if (!Duel.Fields[0].HasInHand((int)CardId.千手神) && !Duel.Fields[0].HasInHand((int)CardId.万手神) && !Duel.Fields[0].HasInHand((int)CardId.影灵衣舞姬))
                return true;
            return false;
        }

        private bool 三叉龙之影灵衣()
        {
            if (AI.Utils.IsAllEnnemyBetterThanValue(2700, true) && Duel.Fields[0].HasInHand((int)CardId.决战兵器之影灵衣))
                return false;
            return true;
        }

        private bool 决战兵器之影灵衣()
        {
            if (AI.Utils.IsAllEnnemyBetterThanValue(3300, true))
            {
                AI.SelectCard((int)CardId.决战兵器之影灵衣);
                return true;
            }
            return false;
        }

        private bool 励辉士入魔蝇王()
        {
            if (AI.Utils.IsAllEnnemyBetterThanValue(Duel.Fields[0].GetMonsters().GetHighestAttackMonster().Attack, true))
            {
                return true;
            }
            return false;
        }

        private bool 瓦尔基鲁斯之影灵衣()
        {
            if (Duel.Phase == DuelPhase.Battle)
                return true;
            return false;
        }

        private bool 天枪龙之影灵衣()
        {           
            if (AI.Utils.IsOneEnnemyBetterThanValue(Duel.Fields[0].GetMonsters().GetHighestAttackMonster().Attack,true) && Duel.Phase == DuelPhase.Main1)
            {
                AI.SelectCard(Duel.Fields[1].GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool 光枪龙之影灵衣()
        {
            if (!Duel.Fields[0].HasInHand((int)CardId.影灵衣术士施里特))
            {
                AI.SelectCard((int)CardId.影灵衣术士施里特);
                return true;
            }
            else if (!Duel.Fields[0].HasInHand(NekrozSpellCard))
            {
                AI.SelectCard((int)CardId.影灵衣的降魔镜);
                return true;
            }
            else if (AI.Utils.IsOneEnnemyBetterThanValue(3300, true) && !Duel.Fields[0].HasInHand((int)CardId.三叉龙之影灵衣))
            {
                AI.SelectCard((int)CardId.三叉龙之影灵衣);
                return true;
            }
            else if (AI.Utils.IsAllEnnemyBetterThanValue(2700,true) && !Duel.Fields[0].HasInHand((int)CardId.决战兵器之影灵衣))
            {
                AI.SelectCard((int)CardId.决战兵器之影灵衣);
                return true;
            }
            else if (Duel.Fields[0].HasInHand((int)CardId.尤尼科之影灵衣) && !Duel.Fields[0].HasInHand((int)CardId.影灵衣的万华镜))
            {
                AI.SelectCard((int)CardId.影灵衣的万华镜);
                return true;
            }
            else if (!Duel.Fields[0].HasInHand((int)CardId.尤尼科之影灵衣) && Duel.Fields[0].HasInHand((int)CardId.影灵衣的万华镜))
            {
                AI.SelectCard((int)CardId.尤尼科之影灵衣);
                return true;
            }
            return true;
        }

        private bool 千手神效果()
        {
            if (AI.Utils.IsOneEnnemyBetterThanValue(3300, true) && !Duel.Fields[0].HasInHand((int)CardId.三叉龙之影灵衣))
            {
                AI.SelectCard((int)CardId.三叉龙之影灵衣);
                return true;
            }
            else if (AI.Utils.IsAllEnnemyBetterThanValue(2700, true) && !Duel.Fields[0].HasInHand((int)CardId.决战兵器之影灵衣))
            {
                AI.SelectCard((int)CardId.决战兵器之影灵衣);
                return true;
            }
            else if (!Duel.Fields[0].HasInHand((int)CardId.尤尼科之影灵衣) && Duel.Fields[0].HasInHand((int)CardId.影灵衣的万华镜))
            {
                AI.SelectCard((int)CardId.尤尼科之影灵衣);
                return true;
            }
            return true;
        }

        private bool 尤尼科之影灵衣()
        {
            if (Duel.Fields[0].HasInGraveyard((int)CardId.影灵衣术士施里特))
            {
                AI.SelectCard((int)CardId.影灵衣术士施里特);
                return true;
            }
            return false;
        }

        private bool 辉剑鸟之影灵衣()
        {
            if (!Duel.Fields[0].HasInHand(NekrozSpellCard))
            {
                AI.SelectCard((int)CardId.影灵衣的降魔镜);
                return true;
            }
            return false;
        }

        private bool IsTheLastPossibility()
        {
            if (!Duel.Fields[0].HasInHand((int)CardId.决战兵器之影灵衣) && !Duel.Fields[0].HasInHand((int)CardId.三叉龙之影灵衣))
                return true;
            return false;
        }

        private bool SelectNekrozWhoInvoke()
        {
            List<int> NekrozCard = new List<int>();
            try
            {
                foreach (ClientCard Card in Duel.Fields[0].Hand)
                    if (Card != null && NekrozRituelCard.Contains((int)Card.Id))
                        NekrozCard.Add(Card.Id);

                foreach (int Id in NekrozCard)
                {
                    if (Id == (int)CardId.三叉龙之影灵衣 && AI.Utils.IsAllEnnemyBetterThanValue(2700, true) && Duel.Fields[0].HasInHand((int)CardId.决战兵器之影灵衣))
                    {
                        AI.SelectCard((int)CardId.三叉龙之影灵衣);
                        return true;
                    }
                    else if (Id == (int)CardId.决战兵器之影灵衣)
                    {
                        AI.SelectCard((int)CardId.决战兵器之影灵衣);
                        return true;
                    }
                    else if (Id == (int)CardId.尤尼科之影灵衣 && Duel.Fields[0].HasInHand((int)CardId.影灵衣的万华镜) && !Duel.Fields[0].HasInGraveyard((int)CardId.影灵衣术士施里特))
                    {
                        AI.SelectCard((int)CardId.尤尼科之影灵衣);
                        return true;
                    }
                    else if (Id == (int)CardId.瓦尔基鲁斯之影灵衣)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.瓦尔基鲁斯之影灵衣);
                            return true;
                        }
                    }
                    else if (Id == (int)CardId.天枪龙之影灵衣)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.天枪龙之影灵衣);
                            return true;
                        }
                    }
                    else if (Id == (int)CardId.辉剑鸟之影灵衣)
                    {
                        if (IsTheLastPossibility())
                        {
                            AI.SelectCard((int)CardId.辉剑鸟之影灵衣);
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
