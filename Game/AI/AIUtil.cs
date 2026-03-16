using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
namespace WindBot.Game.AI
{
    public class AIUtil
    {
        public Duel Duel { get; private set; }
        public ClientField Bot { get; private set; }
        public ClientField Enemy { get; private set; }

        public AIUtil(Duel duel)
        {
            Duel = duel;
            Bot = Duel.Fields[0];
            Enemy = Duel.Fields[1];
        }

        /// <summary>
        /// Get the total ATK Monster of the player.
        /// </summary>
        public int GetTotalAttackingMonsterAttack(int player)
        {
            return Duel.Fields[player].GetMonsters().Where(m => m.IsAttack()).Sum(m => (int?)m.Attack) ?? 0;
        }
        /// <summary>
        /// Get the best ATK or DEF power of the field.
        /// </summary>
        /// <param name="field">Bot or Enemy.</param>
        /// <param name="onlyATK">Only calculate attack.</param>
        public int GetBestPower(ClientField field, bool onlyATK = false)
        {
            return field.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .Max(card => (int?)card.GetDefensePower()) ?? -1;
        }

        public int GetBestAttack(ClientField field)
        {
            return GetBestPower(field, true);
        }

        public bool IsOneEnemyBetterThanValue(int value, bool onlyATK)
        {
            return Enemy.MonsterZone.GetMonsters()
                .Any(card => card.GetDefensePower() > value && (!onlyATK || card.IsAttack()));
        }

        public bool IsAllEnemyBetterThanValue(int value, bool onlyATK)
        {
            List<ClientCard> monsters = Enemy.MonsterZone.GetMonsters();
            return monsters.Count > 0 && monsters
                .All(card => card.GetDefensePower() > value && (!onlyATK || card.IsAttack()));
        }

        /// <summary>
        /// Deprecated, use IsOneEnemyBetter and IsAllEnemyBetter instead.
        /// </summary>
        public bool IsEnemyBetter(bool onlyATK, bool all)
        {
            if (all)
                return IsAllEnemyBetter(onlyATK);
            else
                return IsOneEnemyBetter(onlyATK);
        }

        /// <summary>
        /// Is there an enemy monster who has better power than the best power of the bot's?
        /// </summary>
        /// <param name="onlyATK">Only calculate attack.</param>
        public bool IsOneEnemyBetter(bool onlyATK = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return IsOneEnemyBetterThanValue(bestBotPower, onlyATK);
        }

        /// <summary>
        /// Do all enemy monsters have better power than the best power of the bot's?
        /// </summary>
        /// <param name="onlyATK">Only calculate attack.</param>
        public bool IsAllEnemyBetter(bool onlyATK = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return IsAllEnemyBetterThanValue(bestBotPower, onlyATK);
        }

        public ClientCard GetBestBotMonster(bool onlyATK = false)
        {
            return Bot.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderByDescending(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetWorstBotMonster(bool onlyATK = false)
        {
            return Bot.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderBy(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetOneEnemyBetterThanValue(int value, bool onlyATK = false, bool canBeTarget = false)
        {
            return Enemy.MonsterZone.GetMonsters()
                .FirstOrDefault(card => card.GetDefensePower() >= value && (!onlyATK || card.IsAttack()) && (!canBeTarget || !card.IsShouldNotBeTarget()));
        }

        public ClientCard GetOneEnemyBetterThanMyBest(bool onlyATK = false, bool canBeTarget = false)
        {
            int bestBotPower = GetBestPower(Bot, onlyATK);
            return GetOneEnemyBetterThanValue(bestBotPower, onlyATK, canBeTarget);
        }

        public ClientCard GetProblematicEnemyCard(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.SpellZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;

            if (attack == 0)
                attack = GetBestAttack(Bot);
            return GetOneEnemyBetterThanValue(attack, true, canBeTarget);
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            ClientCard card = Enemy.MonsterZone.GetFloodgate(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetDangerousMonster(canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetInvincibleMonster(canBeTarget);
            if (card != null)
                return card;

            if (attack == 0)
                attack = GetBestAttack(Bot);
            return GetOneEnemyBetterThanValue(attack, true, canBeTarget);
        }

        public ClientCard GetProblematicEnemySpell()
        {
            ClientCard card = Enemy.SpellZone.GetFloodgate();
            return card;
        }

        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null)
                return card;

            card = GetBestEnemySpell(onlyFaceup);
            if (card != null)
                return card;

            return null;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count > 0 && !onlyFaceup)
                return monsters[0];

            return null;
        }

        public ClientCard GetWorstEnemyMonster(bool onlyATK = false)
        {
            return Enemy.MonsterZone.GetMonsters()
                .Where(card => !onlyATK || card.IsAttack())
                .OrderBy(card => card.GetDefensePower())
                .FirstOrDefault();
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false)
        {
            ClientCard card = GetProblematicEnemySpell();
            if (card != null)
                return card;

            var spells = Enemy.GetSpells();

            card = spells.FirstOrDefault(ecard => ecard.IsFaceup() && (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field)));
            if (card != null)
                return card;

            if (spells.Count > 0 && !onlyFaceup)
                return spells[0];

            return null;
        }

        public ClientCard GetPZone(int player, int id)
        {
            if (Duel.IsNewRule)
            {
                return Duel.Fields[player].SpellZone[id * 4];
            }
            else
            {
                return Duel.Fields[player].SpellZone[6 + id];
            }
        }

        public int GetStringId(int id, int option)
        {
            return id * 16 + option;
        }

        public bool IsTurn1OrMain2()
        {
            return Duel.Turn == 1 || Duel.Phase == DuelPhase.Main2;
        }

        public int GetBotAvailZonesFromExtraDeck(IList<ClientCard> remove)
        {
            ClientCard[] BotMZone = (ClientCard[])Bot.MonsterZone.Clone();
            ClientCard[] EnemyMZone = (ClientCard[])Enemy.MonsterZone.Clone();
            for (int i = 0; i < 7; i++)
            {
                if (remove.Contains(BotMZone[i])) BotMZone[i] = null;
                if (remove.Contains(EnemyMZone[i])) EnemyMZone[i] = null;
            }

            if (!Duel.IsNewRule || Duel.IsNewRule2020)
                return Zones.MainMonsterZones;

            int result = 0;

            if (BotMZone[5] == null && BotMZone[6] == null)
            {
                if (EnemyMZone[5] == null)
                    result |= Zones.z6;
                if (EnemyMZone[6] == null)
                    result |= Zones.z5;
            }

            if (BotMZone[0] == null &&
                ((BotMZone[1]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.BottomLeft) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.TopRight) ?? false)))
                result |= Zones.z0;

            if (BotMZone[1] == null &&
                ((BotMZone[0]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[2]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.Bottom) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.Top) ?? false)))
                result |= Zones.z1;

            if (BotMZone[2] == null &&
                ((BotMZone[1]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[3]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[5]?.HasLinkMarker(CardLinkMarker.BottomRight) ?? false) ||
                 (EnemyMZone[6]?.HasLinkMarker(CardLinkMarker.TopLeft) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.BottomLeft) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.TopRight) ?? false)))
                result |= Zones.z2;

            if (BotMZone[3] == null &&
                ((BotMZone[2]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[4]?.HasLinkMarker(CardLinkMarker.Left) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.Bottom) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.Top) ?? false)))
                result |= Zones.z3;

            if (BotMZone[4] == null &&
                ((BotMZone[3]?.HasLinkMarker(CardLinkMarker.Right) ?? false) ||
                 (BotMZone[6]?.HasLinkMarker(CardLinkMarker.BottomRight) ?? false) ||
                 (EnemyMZone[5]?.HasLinkMarker(CardLinkMarker.TopLeft) ?? false)))
                result |= Zones.z4;

            return result;
        }

        public int GetBotAvailZonesFromExtraDeck(ClientCard remove)
        {
            return GetBotAvailZonesFromExtraDeck(new[] { remove });
        }

        public int GetBotAvailZonesFromExtraDeck()
        {
            return GetBotAvailZonesFromExtraDeck(new List<ClientCard>());
        }

        public bool IsChainTarget(ClientCard card)
        {
            return Duel.ChainTargets.Any(card.Equals);
        }

        public bool IsChainTargetOnly(ClientCard card)
        {
            return Duel.ChainTargetOnly.Count == 1 && card.Equals(Duel.ChainTargetOnly[0]);
        }

        public bool ChainContainsCard(int id)
        {
            return Duel.CurrentChain.Any(card => card.IsCode(id));
        }

        public bool ChainContainsCard(int[] ids)
        {
            return Duel.CurrentChain.Any(card => card.IsCode(ids));
        }

        public int ChainCountPlayer(int player)
        {
            return Duel.CurrentChain.Count(card => card.Controller == player);
        }

        public bool ChainContainPlayer(int player)
        {
            return Duel.CurrentChain.Any(card => card.Controller == player);
        }

        public bool HasChainedTrap(int player)
        {
            return Duel.CurrentChain.Any(card => card.Controller == player && card.HasType(CardType.Trap));
        }

        public ClientCard GetLastChainCard()
        {
            return Duel.CurrentChain.LastOrDefault();
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(ClientCard preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            if (cards.IndexOf(preferred) > 0 && selected.Count < max)
            {
                selected.Add(preferred);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(int preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            foreach (ClientCard card in cards)
            {
                if (card.IsCode(preferred) && selected.Count < max)
                    selected.Add(card);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(IList<ClientCard> preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> avail = cards.ToList(); // clone
            while (preferred.Count > 0 && avail.IndexOf(preferred[0]) > 0 && selected.Count < max)
            {
                ClientCard card = preferred[0];
                preferred.Remove(card);
                avail.Remove(card);
                selected.Add(card);
            }

            return selected;
        }

        /// <summary>
        /// Select cards listed in preferred.
        /// </summary>
        public IList<ClientCard> SelectPreferredCards(IList<int> preferred, IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            foreach (int id in preferred)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(id) && selected.Count < max && selected.IndexOf(card) <= 0)
                        selected.Add(card);
                }
                if (selected.Count >= max)
                    break;
            }

            return selected;
        }

        /// <summary>
        /// Check and fix selected to make sure it meet the count requirement.
        /// </summary>
        public IList<ClientCard> CheckSelectCount(IList<ClientCard> _selected, IList<ClientCard> cards, int min, int max)
        {
            var selected = _selected.Distinct().ToList();
            if (selected.Count < min)
            {
                foreach (ClientCard card in cards)
                {
                    if (!selected.Contains(card))
                        selected.Add(card);
                    if (selected.Count >= max)
                        break;
                }
                if (selected.Count < min)
                {
#if DEBUG
                    throw new Exception("Not enough cards to CheckSelectCount");
#else
                    Logger.WriteErrorLine("Not enough cards to CheckSelectCount, using default");
                    return null;
#endif
                }
            }
            while (selected.Count > max)
            {
                selected.RemoveAt(selected.Count - 1);
            }

            return selected;
        }

        /// <summary>
        /// Get all xyz materials lists that xyz monster required level in the 'pre_materials' list
        /// </summary>
        /// <param name="param_pre_materials">Original materials</param>
        /// <param name="level">Xyz monster required level</param>
        /// <param name="material_count">SpSummon rule:number of xyz materials</param>
        /// <param name="material_count_above">More xyz materials</param>
        /// <param name="material_func">Filter xyz materials func</param>
        /// <returns></returns>
        public List<List<ClientCard>> GetXyzMaterials(IList<ClientCard> param_pre_materials, int level, int material_count, bool material_count_above = false, Func<ClientCard, bool> material_func = null)
        {
            List<List<ClientCard>> result = new List<List<ClientCard>>();
            List<ClientCard> pre_materials = param_pre_materials?.Where(card => card != null && !(card.IsFacedown() & card.Location == CardLocation.MonsterZone) && card.Level == level && !card.IsMonsterNotBeXyzMaterial()).ToList();
            if (pre_materials?.Count() < material_count) return result;
            Func<ClientCard, bool> default_func = card => true;
            material_func = material_func ?? default_func;
            for (int i = 1; i < Math.Pow(2, pre_materials.Count); i++)
            {
                List<ClientCard> temp_materials = new List<ClientCard>();
                string binaryString = Convert.ToString(i, 2).PadLeft(pre_materials.Count, '0');
                char[] reversedBinaryChars = binaryString.Reverse().ToArray();
                for (int j = 0; j < pre_materials.Count; j++)
                {
                    if (reversedBinaryChars[j] == '1' && material_func(pre_materials[j]))
                    {
                        temp_materials.Add(pre_materials[j]);
                    }
                }
                if (material_count_above ? temp_materials.Count >= material_count : temp_materials.Count == material_count)
                {
                    result.Add(temp_materials);
                }
            }
            return result;
        }

        /// <summary>
        /// Get all synchro materials lists that synchro monster level == param 'level' in the 'pre_materials' list
        /// </summary>
        /// <param name="pre_materials">Original materials</param>
        /// <param name="level">Synchro monster level</param>
        /// <param name="tuner_count">SpSummon rule:number of tuner monsters </param>
        /// <param name="n_tuner_count">SpSummon rule:number of non-tuner monsters count</param>
        /// <param name="tuner_count_above">More tuner monsters</param>
        /// <param name="n_tuner_count_above">More non-tuner monsters</param>
        /// <param name="tuner_func">Filter tuner monsters func</param>
        /// <param name="n_tuner_func">Filter non-tuner monsters func</param>
        /// <returns></returns>
        public List<List<ClientCard>> GetSynchroMaterials(IList<ClientCard> param_pre_materials, int level, int tuner_count, int n_tuner_count, bool tuner_count_above = false, bool n_tuner_count_above = true, Func<ClientCard, bool> tuner_func = null, Func<ClientCard, bool> n_tuner_func = null)
        {
            List<List<ClientCard>> t_result = new List<List<ClientCard>>();
            List<ClientCard> pre_materials = param_pre_materials?.Where(card => card != null && !(card.IsFacedown() & card.Location == CardLocation.MonsterZone) && card.Level > 0 && !card.IsMonsterNotBeSynchroMaterial()).ToList();
            if (pre_materials?.Count() < tuner_count + n_tuner_count) return t_result;
            Func<ClientCard, bool> default_func = card => true;
            tuner_func = tuner_func ?? default_func;
            n_tuner_func = n_tuner_func ?? default_func;
            pre_materials.Sort(CardContainer.CompareCardLevel);
            Stack<object[]> materials_stack = new Stack<object[]>();
            for (var i = 0; i < pre_materials.Count; i++)
            {
                if (pre_materials[i].Level > level) break;
                materials_stack.Push(new object[] { pre_materials[i].Level, i, pre_materials[i].Level, new List<ClientCard> { pre_materials[i] } });
            }
            while (materials_stack.Count > 0)
            {
                object[] data = materials_stack.Pop();
                int num = (int)data[0];
                int index = (int)data[1];
                int sum = (int)data[2];
                List<ClientCard> temp_materials = (List<ClientCard>)data[3];
                if (sum == level)
                {
                    t_result.Add(temp_materials);
                }
                else if (sum < level)
                {
                    for (var i = index + 1; i < pre_materials.Count; i++)
                    {
                        if (pre_materials[i].Level > level - sum) break;
                        if (i > index + 1 && pre_materials[i].Level == pre_materials[i - 1].Level) continue;
                        var new_temp_materials = new List<ClientCard>(temp_materials);
                        new_temp_materials.Add(pre_materials[i]);
                        materials_stack.Push(new object[] { pre_materials[i].Level, i, sum + pre_materials[i].Level, new_temp_materials });
                    }
                }
            }
            List<List<ClientCard>> result = new List<List<ClientCard>>();
            for (int i = 0; i < t_result.Count; i++)
            {
                List<ClientCard> materials = t_result[i];
                List<ClientCard> tuner_materials = new List<ClientCard>();
                List<ClientCard> n_tuner_materials = new List<ClientCard>();
                foreach (ClientCard material in materials)
                {
                    if (material.HasType(CardType.Tuner) && tuner_func(material))
                    {
                        tuner_materials.Add(material);
                    }
                    else if (material.Level > 0 && n_tuner_func(material))
                    {
                        n_tuner_materials.Add(material);
                    }
                }
                if ((tuner_count_above ? tuner_materials.Count >= tuner_count : tuner_materials.Count == tuner_count)
                    && (n_tuner_count_above ? n_tuner_materials.Count >= n_tuner_count : n_tuner_materials.Count == n_tuner_count))
                    result.Add(materials);
            }
            return result;
        }
    }
}