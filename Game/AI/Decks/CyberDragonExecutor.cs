using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("CyberDragon", "AI_CyberDragon")]
    public class CyberDragonExecutor : DefaultExecutor
    {
        bool 力量结合效果 = false;

        public enum CardId
        {
            电子镭射龙 = 4162088,
            电子障壁龙 = 68774379,
            电子龙 = 70095154,
            电子龙三型 = 59281922,
            电子凤凰 = 3370104,
            装甲电子翼 = 67159705,
            原始电子龙 = 26439287,
            电子麒麟 = 76986005,
            电子龙核 = 23893227,
            电子多变龙 = 3657444,
            Raigeki = 12580477,
            DarkHole = 53129443,
            时间胶囊 = 11961740,
            融合 = 24094653,
            力量结合 = 37630732,
            进化光焰 = 52875873,
            光子发生装置 = 66607691,
            融合解除 = 95286165,
            奈落的落穴 = 29401950,
            神圣防护罩反射镜力 = 44095762,
            攻击反射装置 = 91989718,
            电子隐秘技术 = 92773018,
            活死人的呼声 = 97077563,
            盗贼的七道具 = 3819470,
            电子双生龙 = 74157028,
            电子终结龙 = 1546123,
            电子龙新星 = 58069384
        }

        public CyberDragonExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.SpellSet, (int)CardId.融合解除);

            AddExecutor(ExecutorType.Activate, (int)CardId.时间胶囊, Capsule);
            AddExecutor(ExecutorType.Activate, (int)CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, (int)CardId.融合, 融合);
            AddExecutor(ExecutorType.Activate, (int)CardId.力量结合, 力量结合);
            AddExecutor(ExecutorType.Activate, (int)CardId.进化光焰, 进化光焰);
            AddExecutor(ExecutorType.Activate, (int)CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, (int)CardId.光子发生装置);
            AddExecutor(ExecutorType.Activate, (int)CardId.融合解除, 融合解除);

            AddExecutor(ExecutorType.Activate, (int)CardId.奈落的落穴, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.神圣防护罩反射镜力, DefaultUniqueTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.攻击反射装置);
            AddExecutor(ExecutorType.Activate, (int)CardId.盗贼的七道具, DefaultTrap);
            AddExecutor(ExecutorType.Activate, (int)CardId.活死人的呼声, DefaultCallOfTheHaunted);

            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.电子龙三型, 没有电子龙可特殊召唤);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.电子凤凰, 没有电子龙可特殊召唤);
            AddExecutor(ExecutorType.Summon, (int)CardId.电子多变龙, 没有电子龙可特殊召唤);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.电子龙核, 没有电子龙可特殊召唤);
            AddExecutor(ExecutorType.MonsterSet, (int)CardId.装甲电子翼, 装甲电子翼);
            AddExecutor(ExecutorType.SummonOrSet, (int)CardId.原始电子龙, 原始电子龙);
            AddExecutor(ExecutorType.Summon, (int)CardId.电子麒麟, 电子麒麟);

            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子龙);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子终结龙);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子双生龙);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子障壁龙);
            AddExecutor(ExecutorType.SpSummon, (int)CardId.电子镭射龙);

            AddExecutor(ExecutorType.Activate, (int)CardId.电子障壁龙);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子镭射龙);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子龙三型);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子凤凰);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子麒麟);
            AddExecutor(ExecutorType.Activate, (int)CardId.装甲电子翼, 装甲电子翼效果);
            AddExecutor(ExecutorType.Activate, (int)CardId.电子多变龙);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private bool CyberDragonInHand()  { return Bot.HasInHand((int)CardId.电子龙); }
        private bool CyberDragonInGraveyard()  { return Bot.HasInGraveyard((int)CardId.电子龙); }
        private bool CyberDragonInMonsterZone() { return Bot.HasInMonstersZone((int)CardId.电子龙); }
        private bool CyberDragonIsBanished() { return Bot.HasInBanished((int)CardId.电子龙); }

        private bool Capsule()
        {
            List<int> SelectedCard = new List<int>();
            SelectedCard.Add((int)CardId.力量结合);
            SelectedCard.Add((int)CardId.DarkHole);
            SelectedCard.Add((int)CardId.Raigeki);
            AI.SelectCard(SelectedCard);
            return true;
        }

        private bool 融合()
        {
            if (Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.原始电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙三型) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙三型) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.电子龙) >= 3)
                AI.SelectCard((int)CardId.电子终结龙);
            else
                AI.SelectCard((int)CardId.电子双生龙);
            return true;
        }

        private bool 力量结合()
        {
            力量结合效果 = true;
            if (Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.原始电子龙) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.Hand, (int)CardId.电子龙核) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.电子龙核) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.电子龙三型) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙三型) >= 3)
                AI.SelectCard((int)CardId.电子终结龙);
            else
                AI.SelectCard((int)CardId.电子双生龙);
            return true;
        }

        private bool 进化光焰()
        {
            ClientCard bestMy = Bot.GetMonsters().GetHighestAttackMonster();
            if (bestMy == null || !AI.Utils.IsOneEnemyBetterThanValue(bestMy.Attack, false))
                return false;
            else
                AI.SelectCard(Enemy.MonsterZone.GetHighestAttackMonster());     
            return true;
        }

        private bool 没有电子龙可特殊召唤()
        {
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0))
                return false;
            return true;
        }

        private bool 装甲电子翼()
        {
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand((int)CardId.电子龙三型) || Bot.HasInHand((int)CardId.电子凤凰)) && !AI.Utils.IsOneEnemyBetterThanValue(1800,true))
                return false;
            return true;
        }

        private bool 原始电子龙()
        {
            if (Bot.GetCountCardInZone(Bot.Hand, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙核) >= 1 && Bot.HasInHand((int)CardId.融合) || Bot.GetCountCardInZone(Bot.Hand, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.MonsterZone, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.电子龙) + Bot.GetCountCardInZone(Bot.Graveyard, (int)CardId.电子龙核) >= 1 && Bot.HasInHand((int)CardId.力量结合))
                return true;
            if (CyberDragonInHand() && (Bot.GetMonsterCount() == 0 && Enemy.GetMonsterCount() != 0) || (Bot.HasInHand((int)CardId.电子龙三型) || Bot.HasInHand((int)CardId.电子凤凰)) && !AI.Utils.IsOneEnemyBetterThanValue(1800, true))
                return false;
            return true;
        }

        private bool 电子麒麟()
        {
            return 力量结合效果;
        }

        private bool 装甲电子翼效果()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            else if (Card.Location == CardLocation.SpellZone)
            {
                if (AI.Utils.IsOneEnemyBetterThanValue(Bot.GetMonsters().GetHighestAttackMonster().Attack, true))
                    if (ActivateDescription == AI.Utils.GetStringId((int)CardId.装甲电子翼, 2))
                        return true;
                return false;
            }
            return false;
        }

        private bool 融合解除()
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