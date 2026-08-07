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
    }
}