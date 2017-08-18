using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Blackwing", "AI_Blackwing")]
    public class BlackwingExecutor : DefaultExecutor
    {
        public enum CardId
        {
            残夜之波刃剑鸟 = 81105204,
            晓之希洛克 = 75498415,
            苍炎之修罗 = 58820853,
            黑枪之布拉斯特 = 49003716,
            月影之卡鲁特 = 85215458,
            疾风之盖尔 = 2009101,
            极北之布利扎德 = 22835145,
            银盾之密史脱拉 = 46710683,
            Raigeki = 12580477,
            DarkHole = 53129443,
            MysticalSpaceTyphoon = 5318639,
            黑旋风 = 91351370,
            神圣防护罩 = 44095762,
            三角乌鸦阵 = 59839761,
            次元幽闭 = 70342110,
            孤高之银风鸦 = 33236860,
            黑羽龙 = 9012916,
            铠翼鸦 = 69031175,
            兵翼鸦 = 76913983,
            煌星之怒剑鸟 = 17377751
        }

        public BlackwingExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑旋风, 黑旋风);
            
            AddExecutor(ExecutorType.SpSummon, (int)CardId.残夜之波刃剑鸟);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.残夜之波刃剑鸟);
            AddExecutor(ExecutorType.Summon, (int)CardId.晓之希洛克, 晓之希洛克);
            AddExecutor(ExecutorType.Summon, (int)CardId.苍炎之修罗, 苍炎之修罗);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.苍炎之修罗);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.黑枪之布拉斯特);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.黑枪之布拉斯特);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.月影之卡鲁特, 月影之卡鲁特);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.疾风之盖尔);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.疾风之盖尔);
            AddExecutor(ExecutorType.Summon, (int)CardId.极北之布利扎德, 极北之布利扎德);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.银盾之密史脱拉);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.孤高之银风鸦);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.铠翼鸦);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.煌星之怒剑鸟);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.兵翼鸦);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.黑羽龙);

            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.次元幽闭, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.三角乌鸦阵, 三角乌鸦阵);
            AddExecutor(ExecutorType.Activate, (int)CardId.三角乌鸦阵, DefaultUniqueTrap);

            AddExecutor(ExecutorType.Activate, (int)CardId.极北之布利扎德);
            AddExecutor(ExecutorType.Activate, (int)CardId.苍炎之修罗);
            AddExecutor(ExecutorType.Activate, (int)CardId.月影之卡鲁特, 攻击力上升效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.晓之希洛克, 攻击力上升效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.疾风之盖尔, 疾风之盖尔效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.孤高之银风鸦);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑羽龙);
            AddExecutor(ExecutorType.Activate, (int)CardId.铠翼鸦);
            AddExecutor(ExecutorType.Activate, (int)CardId.兵翼鸦);
            AddExecutor(ExecutorType.Activate, (int)CardId.煌星之怒剑鸟);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (defender.IsMonsterInvincible() && !defender.IsMonsterDangerous() && attacker.Id == 83104731)
                return true;
            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool 苍炎之修罗()
        {
            if (Bot.HasInMonstersZone((int)CardId.晓之希洛克) && Bot.GetMonsters().GetHighestAttackMonster().Attack < 3800)
                return true;
            return false;
        }

        private bool 黑旋风()
        {
            if (Card.Location == CardLocation.Hand && Bot.HasInSpellZone(Card.Id))
                return false;
            if (ActivateDescription == AI.Utils.GetStringId((int)Card.Id,0))
                AI.SelectCard((int)CardId.疾风之盖尔);
            return true;
        }

        private bool 晓之希洛克()
        {
            int OpponentMonster = Enemy.GetMonsterCount();
            int AIMonster = Bot.GetMonsterCount();
            if (OpponentMonster != 0 && AIMonster == 0)
                return true;
            return false;
        }

        private bool 黑枪之布拉斯特()
        {
            List<ClientCard> monster = Bot.GetMonsters();
            foreach (ClientCard card in monster)
                if (card != null && card.Id == (int)CardId.残夜之波刃剑鸟 || card.Id == (int)CardId.月影之卡鲁特 || card.Id == (int)CardId.疾风之盖尔 || card.Id == (int)CardId.黑枪之布拉斯特 || card.Id == (int)CardId.晓之希洛克 || card.Id == (int)CardId.苍炎之修罗 || card.Id == (int)CardId.极北之布利扎德)
                    return true;
            return false;
        }

        private bool 月影之卡鲁特()
        {
            foreach (ClientCard card in Bot.Hand)
                if (card != null && card.Id == (int)CardId.残夜之波刃剑鸟 || card.Id == (int)CardId.疾风之盖尔 || card.Id == (int)CardId.黑枪之布拉斯特 || card.Id == (int)CardId.晓之希洛克 || card.Id == (int)CardId.苍炎之修罗 || card.Id == (int)CardId.极北之布利扎德)
                    return false;
            return true;
        }

        private bool 极北之布利扎德()
        {
            foreach (ClientCard card in Bot.Graveyard)
                if (card != null && card.Id == (int)CardId.月影之卡鲁特 || card.Id == (int)CardId.黑枪之布拉斯特 || card.Id == (int)CardId.苍炎之修罗 || card.Id == (int)CardId.残夜之波刃剑鸟)
                    return true;
            return false;
        }

        private bool 三角乌鸦阵()
        {
            int Count = 0;

            List<ClientCard> monster = Bot.GetMonsters();
            foreach (ClientCard card in monster)
                if (card != null && card.Id == (int)CardId.残夜之波刃剑鸟 || card.Id == (int)CardId.月影之卡鲁特 || card.Id == (int)CardId.疾风之盖尔 || card.Id == (int)CardId.黑枪之布拉斯特 || card.Id == (int)CardId.晓之希洛克 || card.Id == (int)CardId.苍炎之修罗 || card.Id == (int)CardId.极北之布利扎德)
                    Count++;

            if (Count == 3)
                return true;
            return false;
        }

        private bool 疾风之盖尔效果()
        {
            if (Card.Position == (int)CardPosition.FaceUp)
            {
                AI.SelectCard(Enemy.GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool 攻击力上升效果()
        {
            ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
            ClientCard bestEnemyATK = Enemy.GetMonsters().GetHighestAttackMonster();
            ClientCard bestEnemyDEF = Enemy.GetMonsters().GetHighestDefenseMonster();
            if (bestMy == null || (bestEnemyATK == null && bestEnemyDEF == null))
                return false;
            if (bestEnemyATK != null && bestMy.Attack < bestEnemyATK.Attack)
                return true;
            if (bestEnemyDEF != null && bestMy.Attack < bestEnemyDEF.Defense)
                return true;
            return false;
        }
    }
}