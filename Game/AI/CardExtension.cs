using System;
using System.Linq;
using WindBot.Game.AI.Enums;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI
{
    public static class CardExtension
    {
        /// <summary>
        /// Is this monster is invincible to battle?
        /// </summary>
        public static bool IsMonsterInvincible(this ClientCard card)
        {
            return !card.IsDisabled() &&
                (card.Controller == 0 && Enum.IsDefined(typeof(InvincibleBotMonster), card.Id) ||
                 card.Controller == 1 && Enum.IsDefined(typeof(InvincibleEnemyMonster), card.Id));
        }

        /// <summary>
        /// Is this monster is dangerous to attack?
        /// </summary>
        public static bool IsMonsterDangerous(this ClientCard card)
        {
            return !card.IsDisabled() &&
                (Enum.IsDefined(typeof(DangerousMonster), card.Id) || (card.HasSetcode(0x18d) && (card.HasType(CardType.Ritual) || card.EquipCards.Count > 0)));
        }

        /// <summary>
        /// Do this monster prevents activation of opponent's effect monsters in battle?
        /// </summary>
        public static bool IsMonsterHasPreventActivationEffectInBattle(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(PreventActivationEffectInBattle), card.Id);
        }

        /// <summary>
        /// Can this monster attack while it is in face-up Defense Position?
        /// </summary>
        public static bool IsMonsterAttackWhileInDefPos(this ClientCard card)
        {
            return card.IsFaceup() && card.IsDefense() && !card.IsDisabled()
                && Enum.IsDefined(typeof(DefenseAttackMonster), card.Id);
        }

        /// <summary>
        /// Get the power this monster uses to attack in its current position.
        /// </summary>
        public static int GetAttackPower(this ClientCard card)
        {
            if (card.IsMonsterAttackWhileInDefPos()
                && !Enum.IsDefined(typeof(DefenseAttackWithAttackValueMonster), card.Id))
                return card.Defense;
            return card.Attack;
        }

        /// <summary>
        /// Is this card shouldn't be tried to be selected as target?
        /// </summary>
        public static bool IsShouldNotBeTarget(this ClientCard card)
        {
            return !card.IsDisabled() && !card.HasType(CardType.Normal)
                && (Enum.IsDefined(typeof(ShouldNotBeTarget), card.Id) || card.Overlays.Any(code => code == 91025875));
        }

        /// <summary>
        /// Is this card shouldn't be tried to be selected as target of monster?
        /// </summary>
        public static bool IsShouldNotBeMonsterTarget(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(ShouldNotBeMonsterTarget), card.Id)
                || card.EquipCards.Any(c => c.IsCode(89812483) && !c.IsDisabled());
        }

        /// <summary>
        /// Is this card shouldn't be tried to be selected as target of spell & trap?
        /// </summary>
        public static bool IsShouldNotBeSpellTrapTarget(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(ShouldNotBeSpellTrapTarget), card.Id)
                || card.EquipCards.Any(c => c.IsCode(89812483) && !c.IsDisabled());
        }

        /// <summary>
        /// Is this monster should be disabled (with Breakthrough Skill) before it use effect and release or banish itself?
        /// </summary>
        public static bool IsMonsterShouldBeDisabledBeforeItUseEffect(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(ShouldBeDisabledBeforeItUseEffectMonster), card.Id);
        }

        public static bool IsFloodgate(this ClientCard card)
        {
            return Enum.IsDefined(typeof(Floodgate), card.Id);
        }

        public static bool IsOneForXyz(this ClientCard card)
        {
            return Enum.IsDefined(typeof(OneForXyz), card.Id);
        }

        /// <summary>
        /// Whether this monster on the field can use cards in the hand as Synchro Material.
        /// </summary>
        public static bool IsOneForSynchro(this ClientCard card)
        {
            return Enum.IsDefined(typeof(OneForSynchro), card.Id);
        }

        /// <summary>
        /// Whether this chain's activated effect can Synchro Summon even without Tuner + non-Tuner on the field.
        /// Matches ActivateDescription, HINT_OPSELECTED announces, or -1 when that is the card's only synchro activate.
        /// </summary>
        /// <param name="chain">The chain snapshot to check.</param>
        /// <returns>True if the chain should be treated as a OneForSynchro effect.</returns>
        public static bool IsOneForSynchroEffect(this ChainInfo chain)
        {
            if (chain == null)
                return false;

            // During chain building, cards like "Ashtra" etc. use SelectFromOptions, with options written into Announces
            if (chain.Announces != null)
            {
                foreach (int announce in chain.Announces)
                {
                    if (Enum.IsDefined(typeof(OneForSynchroEffect), announce))
                        return true;
                }
            }

            int desc = chain.ActivateDescription;
            if (desc != -1)
                return Enum.IsDefined(typeof(OneForSynchroEffect), desc);

            // Some optionally activated effects may set the description to -1.
            // Fall back only for cards that are almost certainly the Synchro-related one.
            return IsOneForSynchroEffectWhenDescriptionUnknown(chain);
        }

        /// <summary>
        /// Fallback when ActivateDescription is -1: the card has only one relevant activate, or location distinguishes it.
        /// </summary>
        private static bool IsOneForSynchroEffectWhenDescriptionUnknown(ChainInfo chain)
        {
            return IsOneForSynchroEffectUnknownDescId(chain, chain.ActivateId)
                || (chain.ActivateAlias != 0
                    && chain.ActivateAlias != chain.ActivateId
                    && IsOneForSynchroEffectUnknownDescId(chain, chain.ActivateAlias));
        }

        private static bool IsOneForSynchroEffectUnknownDescId(ChainInfo chain, int id)
        {
            switch (id)
            {
                case 93665266: // Crystron Quan
                case 20050865: // Crystron Citree
                case 66938505: // Crystron Rion
                case 25148255: // Junk Anchor
                case 89326990: // Speedroid Ohajikid
                case 92932860: // Performapal Miss Director, no SetDescription
                case 88170262: // Kewl Tune Remix
                case 65961304: // Kewl Tune B2B
                case 40493210: // Magikey Locking, no SetDescription
                case 89974904: // Synchro Call, no SetDescription
                case 9402966:  // Superheavy Samurai Battleball, no SetDescription
                case 14507213: // Synchro Material, no SetDescription
                case 62899696: // Speedroid Maliciousmagnet
                    return true;
                case 43904702: // Kewl Tune Clip: ① from hand, ② from GY
                    return chain.HasLocation(CardLocation.Hand);
                case 99471856: // Crystron Tristaros: ① from field, ② from GY
                    return chain.HasLocation(CardLocation.MonsterZone);
                default:
                    return false;
            }
        }

        public static bool IsFusionSpell(this ClientCard card)
        {
            return Enum.IsDefined(typeof(FusionSpell), card.Id);
        }

        /// <summary>
        /// Is this monster not be synchro material?
        /// </summary>
        public static bool IsMonsterNotBeSynchroMaterial(this ClientCard card)
        {
            return Enum.IsDefined(typeof(NotBeSynchroMaterialMonster), card.Id);
        }

        /// <summary>
        /// Is this monster not be xyz material?
        /// </summary>
        public static bool IsMonsterNotBeXyzMaterial(this ClientCard card)
        {
            return Enum.IsDefined(typeof(NotBeXyzMaterialMonster), card.Id);
        }

        public static bool IsMonsterNotBeSummonTribute(this ClientCard card)
        {
            return Enum.IsDefined(typeof(NotBeSummonTributeMonster), card.Id);
        }
    }
}
