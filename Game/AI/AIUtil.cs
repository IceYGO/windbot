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
        /// Get the total attacking power of the player's monsters.
        /// </summary>
        public int GetTotalAttackingMonsterAttack(int player)
        {
            return Duel.Fields[player].GetMonsters()
                .Where(m => m.IsAttack() || m.IsMonsterAttackWhileInDefPos())
                .Sum(m => (int?)m.GetAttackPower()) ?? 0;
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

            if (!onlyFaceup)
                return Enemy.GetMonsters().FirstOrDefault(monster =>
                    !canBeTarget || !monster.IsShouldNotBeTarget());

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
            if (cards.IndexOf(preferred) >= 0 && selected.Count < max)
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
            IList<ClientCard> available = cards.ToList();
            foreach (ClientCard card in preferred)
            {
                if (selected.Count >= max)
                    break;
                if (available.Remove(card))
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
                    if (card.IsCode(id) && selected.Count < max && !selected.Contains(card))
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
            if (max == 0)
                Logger.WriteErrorLine("CheckSelectCount called with max = 0.");
            if (_selected.Count == 0 && min == 0)
                Logger.DebugWriteLine("CheckSelectCount called without preferred cards when min is 0.", true);
            if (_selected.Any(card => !cards.Contains(card)))
                Logger.DebugWriteLine("Selected cards contain cards outside the available candidates.", true);

            var selected = _selected.Where(cards.Contains).Distinct().ToList();
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
        /// Enumerates Synchro material combinations whose levels sum to the target level and satisfy the counts and filters.
        /// </summary>
        /// <param name="candidateMaterials">Candidate materials whose locations and positions have been checked by the caller.</param>
        /// <param name="synchroLevel">Target Synchro monster's level.</param>
        /// <param name="tunerCount">Required Tuner count.</param>
        /// <param name="nonTunerCount">Required non-Tuner count.</param>
        /// <param name="allowMoreTuners">True requires at least tunerCount Tuners; false requires exactly that many.</param>
        /// <param name="allowMoreNonTuners">True requires at least nonTunerCount non-Tuners; false requires exactly that many.</param>
        /// <param name="tunerFilter">Optional additional filter for Tuners. Null applies no additional restriction.</param>
        /// <param name="nonTunerFilter">Optional additional filter for non-Tuners. Null applies no additional restriction.</param>
        /// <returns>
        /// Distinct combinations in depth-first search order over candidates sorted by increasing level,
        /// preserving input order for equal levels. This order does not prioritize fewer materials.
        /// Returns an empty list for null input, invalid numeric arguments or no matching combination.
        /// </returns>
        public List<List<ClientCard>> GetSynchroMaterials(IList<ClientCard> candidateMaterials, int synchroLevel, int tunerCount, int nonTunerCount, bool allowMoreTuners = false, bool allowMoreNonTuners = true, Func<ClientCard, bool> tunerFilter = null, Func<ClientCard, bool> nonTunerFilter = null)
        {
            List<List<ClientCard>> result = new List<List<ClientCard>>();
            if (candidateMaterials == null || synchroLevel <= 0 || tunerCount < 0 || nonTunerCount < 0)
                return result;

            List<ClientCard> eligibleMaterials = candidateMaterials
                .Where(card => card != null
                    && card.Level > 0 && card.Level <= synchroLevel && !card.IsMonsterNotBeSynchroMaterial())
                .Distinct()
                .Where(card => card.HasType(CardType.Tuner)
                    ? tunerFilter == null || tunerFilter(card)
                    : nonTunerFilter == null || nonTunerFilter(card))
                .OrderBy(card => card.Level)
                .ToList();
            int availableTuners = eligibleMaterials.Count(card => card.HasType(CardType.Tuner));
            if (availableTuners < tunerCount || eligibleMaterials.Count - availableTuners < nonTunerCount)
                return result;

            Stack<int> selectedIndexes = new Stack<int>();
            List<ClientCard> materials = new List<ClientCard>();
            int nextIndex = 0;
            int sum = 0;
            int tuners = 0;
            int nonTuners = 0;
            while (true)
            {
                if (sum == synchroLevel)
                {
                    if (tuners >= tunerCount && nonTuners >= nonTunerCount)
                        result.Add(new List<ClientCard>(materials));
                }
                else if (nextIndex < eligibleMaterials.Count)
                {
                    int index = nextIndex++;
                    ClientCard material = eligibleMaterials[index];
                    bool isTuner = material.HasType(CardType.Tuner);
                    if (sum + material.Level <= synchroLevel)
                    {
                        if ((!isTuner || allowMoreTuners || tuners < tunerCount)
                            && (isTuner || allowMoreNonTuners || nonTuners < nonTunerCount))
                        {
                            selectedIndexes.Push(index);
                            materials.Add(material);
                            sum += material.Level;
                            if (isTuner) tuners++;
                            else nonTuners++;
                        }
                        continue;
                    }
                    // Later candidates cannot fit either because levels are sorted ascending.
                }

                if (selectedIndexes.Count == 0)
                    break;
                int previousIndex = selectedIndexes.Pop();
                ClientCard previousMaterial = eligibleMaterials[previousIndex];
                materials.RemoveAt(materials.Count - 1);
                sum -= previousMaterial.Level;
                if (previousMaterial.HasType(CardType.Tuner)) tuners--;
                else nonTuners--;
                nextIndex = previousIndex + 1;
            }
            return result;
        }

        /// <summary>
        /// Enumerates Xyz material combinations matching the required material level, count and filter.
        /// </summary>
        /// <param name="candidateMaterials">Candidate materials whose locations and positions have been checked by the caller.</param>
        /// <param name="materialLevel">Required level of each material.</param>
        /// <param name="materialCount">Required material count.</param>
        /// <param name="allowMoreMaterials">True requires at least materialCount materials; false requires exactly that many.</param>
        /// <param name="materialFilter">Optional additional filter for each material. Null applies no additional restriction.</param>
        /// <returns>
        /// Distinct combinations ordered by increasing material count, then lexicographically by candidate input indexes.
        /// Cards within each combination retain their input order.
        /// Returns an empty list for null input, invalid numeric arguments or no matching combination.
        /// </returns>
        public List<List<ClientCard>> GetXyzMaterials(IList<ClientCard> candidateMaterials, int materialLevel, int materialCount, bool allowMoreMaterials = false, Func<ClientCard, bool> materialFilter = null)
        {
            List<List<ClientCard>> result = new List<List<ClientCard>>();
            if (candidateMaterials == null || materialLevel <= 0 || materialCount <= 0)
                return result;

            List<ClientCard> eligibleMaterials = candidateMaterials
                .Where(card => card != null
                    && card.Level == materialLevel && !card.HasType(CardType.Token) && !card.IsMonsterNotBeXyzMaterial())
                .Distinct()
                .Where(card => materialFilter == null || materialFilter(card))
                .ToList();
            if (eligibleMaterials.Count < materialCount)
                return result;

            int maxCount = allowMoreMaterials ? eligibleMaterials.Count : materialCount;
            for (int count = materialCount; count <= maxCount; count++)
            {
                int[] indexes = Enumerable.Range(0, count).ToArray();
                while (true)
                {
                    List<ClientCard> materials = new List<ClientCard>(count);
                    foreach (int index in indexes)
                        materials.Add(eligibleMaterials[index]);
                    result.Add(materials);

                    // Advance the rightmost index that leaves room for all following materials.
                    int position = count - 1;
                    while (position >= 0 && indexes[position] == eligibleMaterials.Count - count + position)
                        position--;
                    if (position < 0)
                        break;
                    indexes[position]++;
                    for (int i = position + 1; i < count; i++)
                        indexes[i] = indexes[i - 1] + 1;
                }
            }
            return result;
        }

        /// <summary>
        /// Check whether the materials group can make the exact Link Rating.
        /// A Link Monster can count as either 1 or its own Link Rating.
        /// </summary>
        /// <param name="materials">The complete material combination to validate.</param>
        /// <param name="linkRating">The Link Rating of the Link monster to summon.</param>
        /// <returns>Whether the materials can produce the exact target Link Rating.</returns>
        public bool CanMakeLinkRating(IList<ClientCard> materials, int linkRating)
        {
            if (materials == null || materials.Count == 0 || linkRating <= 0 || materials.Any(card => card == null))
                return false;

            // Keep every total reachable after each material.
            HashSet<int> totals = new HashSet<int> { 0 };
            foreach (ClientCard material in materials)
            {
                HashSet<int> nextTotals = new HashSet<int>();
                foreach (int total in totals)
                {
                    if (total + 1 <= linkRating)
                        nextTotals.Add(total + 1);

                    // A Link Monster branches into counting as 1 and counting as its own Link Rating.
                    if (material.HasType(CardType.Link) && material.LinkCount > 1 && total + material.LinkCount <= linkRating)
                        nextTotals.Add(total + material.LinkCount);
                }
                totals = nextTotals;
                if (totals.Count == 0)
                    return false;
            }
            return totals.Contains(linkRating);
        }

        /// <summary>
        /// Get all material subsets that can make the exact Link Rating.
        /// </summary>
        /// <param name="candidateMaterials">The candidate cards from which material combinations are selected.</param>
        /// <param name="linkRating">The Link Rating of the Link monster to summon.</param>
        /// <param name="minCount">The minimum number of materials required by the Link monster.</param>
        /// <param name="maxCount">The maximum number of materials that may be selected. 0 for using linkRating as the maximum.</param>
        /// <param name="materialFilter">An optional additional eligibility filter for individual materials.</param>
        /// <returns>All eligible combinations whose Link values can produce the exact target Link Rating.</returns>
        public List<List<ClientCard>> GetLinkMaterials(IList<ClientCard> candidateMaterials, int linkRating,
            int minCount, int maxCount = 0, Func<ClientCard, bool> materialFilter = null)
        {
            if (candidateMaterials == null || linkRating <= 0 || minCount <= 0)
                return new List<List<ClientCard>>();

            if (maxCount <= 0)
                maxCount = linkRating;

            materialFilter = materialFilter ?? (card => true);
            // Normalize the input before subset enumeration so every bit represents one
            // distinct candidate accepted by the caller while retaining its priority order.
            List<ClientCard> eligibleMaterials = candidateMaterials
                .Where(card => card != null && materialFilter(card))
                .Distinct()
                .ToList();
            if (eligibleMaterials.Count < minCount)
                return new List<List<ClientCard>>();

            // Exhaustively enumerate all 2^n - 1 subsets with a bit mask.
            // Link candidates normally come from the field, so n stays small.
            List<List<ClientCard>> result = new List<List<ClientCard>>();
            int subsetCount = 1 << eligibleMaterials.Count;
            for (int mask = 1; mask < subsetCount; mask++)
            {
                List<ClientCard> materials = new List<ClientCard>();
                for (int i = 0; i < eligibleMaterials.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        materials.Add(eligibleMaterials[i]);
                }
                if (materials.Count >= minCount && materials.Count <= maxCount && CanMakeLinkRating(materials, linkRating))
                    result.Add(materials);
            }
            return result;
        }

        /// <summary>
        /// Shuffle a list using Fisher–Yates shuffle
        /// </summary>
        /// <param name="list">The original list</param>
        /// <returns>The shuffled copy of the list</returns>
        public List<T> ShuffleList<T>(IList<T> list)
        {
            List<T> result = new List<T>(list);
            ShuffleListInPlace(result);
            return result;
        }

        /// <summary>
        /// Shuffle a list in place using Fisher–Yates shuffle
        /// </summary>
        /// <param name="list">The list to shuffle</param>
        public void ShuffleListInPlace<T>(IList<T> list)
        {
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                (list[n], list[index]) = (list[index], list[n]);
            }
        }
    }
}
