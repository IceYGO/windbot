using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Gravekeeper", "AI_Gravekeeper")]
    public class GravekeeperExecutor : DefaultExecutor
    {
        public enum CardId
        {
            守墓的审神者 = 25524823,
            罪星尘龙 = 36521459,
            守墓的大神官 = 3825890,
            守墓之长 = 62473983,
            雷王 = 71564252,
            守墓的司令官 = 17393207,
            守墓的暗杀者 = 25262697,
            守墓的末裔 = 30213599,
            守墓的侦察者 = 24317029,
            守墓的召唤师 = 93023479,
            暗之诱惑 = 1475311,
            黑洞 = 53129443,
            王家的牲祭 = 72405967,
            守墓的石板 = 99523325,
            旋风 = 5318639,
            月之书 = 14087893,
            王家长眠之谷的祭殿 = 70000776,
            王家长眠之谷 = 47355498,
            奈落的落穴 = 29401950,
            降灵的仪式 = 30450531,
            激流葬 = 53582587,
            次元幽闭 = 70342110,
            神之警告 = 84749824,
            王家长眠之谷的王墓 = 90434657
        }

        public GravekeeperExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Activate, (int)CardId.暗之诱惑);
            AddExecutor(ExecutorType.Activate, (int)CardId.黑洞, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.王家的牲祭);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的石板);
            AddExecutor(ExecutorType.Activate, (int)CardId.旋风, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.月之书, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, (int)CardId.王家长眠之谷的祭殿, 王家长眠之谷的祭殿);
            AddExecutor(ExecutorType.Activate, (int)CardId.王家长眠之谷, 王家长眠之谷);

            AddExecutor(ExecutorType.Activate, (int)CardId.奈落的落穴, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神之警告, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.次元幽闭, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.降灵的仪式, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.王家长眠之谷的王墓, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.激流葬, DefaultTorrentialTribute);

            AddExecutor(ExecutorType.Summon, (int)CardId.守墓的审神者);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.罪星尘龙, 罪星尘龙);
            AddExecutor(ExecutorType.Summon, (int)CardId.守墓的大神官);
            AddExecutor(ExecutorType.Summon, (int)CardId.守墓之长);
            AddExecutor(ExecutorType.Summon, (int)CardId.雷王);
            AddExecutor(ExecutorType.Summon, (int)CardId.守墓的司令官, 守墓的司令官召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.守墓的暗杀者);
            AddExecutor(ExecutorType.Summon, (int)CardId.守墓的末裔);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.守墓的侦察者);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.守墓的召唤师);

            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的审神者);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的大神官);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓之长);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的司令官, 守墓的司令官);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的暗杀者, 守墓的暗杀者);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的末裔, 守墓的末裔);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的侦察者, 检索守墓的末裔);
            AddExecutor(ExecutorType.Activate, (int)CardId.守墓的召唤师, 检索守墓的末裔);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool 王家长眠之谷的祭殿()
        {
            if (Card.Location == CardLocation.Hand && Duel.Fields[0].HasInSpellZone((int)Card.Id))
                return false;
            return true;
        }

        private bool 王家长眠之谷()
        {
            if (Duel.Fields[0].SpellZone[5] != null)
                return false;
            return true;
        }

        private bool 罪星尘龙()
        {
            if (Duel.Fields[0].SpellZone[5] != null)
                return true;
            return false;
        }

        private bool 守墓的司令官()
        {
            if (!Duel.Fields[0].HasInHand((int)CardId.王家长眠之谷) && !Duel.Fields[0].HasInSpellZone((int)CardId.王家长眠之谷))
                return true;
            return false;
        }

        private bool 守墓的司令官召唤()
        {
            return !守墓的司令官();
        }

        private bool 守墓的暗杀者()
        {
            if (!Card.IsAttack())
                return false;
            foreach (ClientCard card in Duel.Fields[1].MonsterZone)
                if (card != null && card.IsDefense() && card.Defense > 1500 && card.Attack < 1500 || card.Attack > 1500 && card.Defense < 1500)
                    return true;
            return false;
        }

        private bool 守墓的末裔()
        {
            int bestatk = Duel.Fields[0].GetMonsters().GetHighestAttackMonster().Attack;
            if (AI.Utils.IsOneEnemyBetterThanValue(bestatk, true))
            {
                AI.SelectCard(Duel.Fields[1].GetMonsters().GetHighestAttackMonster());
                return true;
            }
            return false;
        }

        private bool 检索守墓的末裔()
        {
            AI.SelectCard((int)CardId.守墓的末裔);
            return true;
        }
    }
}