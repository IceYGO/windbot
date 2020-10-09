﻿using System;
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
            return !card.IsDisabled() && Enum.IsDefined(typeof(InvincibleMonster), card.Id);
        }

        /// <summary>
        /// Is this monster is dangerous to attack?
        /// </summary>
        public static bool IsMonsterDangerous(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(DangerousMonster), card.Id);
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
            return !card.IsDisabled() && !card.HasType(CardType.Normal) && Enum.IsDefined(typeof(ShouldNotBeTarget), card.Id);
        }

        /// <summary>
        /// Is this card shouldn't be tried to be selected as target of monster?
        /// </summary>
        public static bool IsShouldNotBeMonsterTarget(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(ShouldNotBeMonsterTarget), card.Id);
        }

        /// <summary>
        /// Is this card shouldn't be tried to be selected as target of spell & trap?
        /// </summary>
        public static bool IsShouldNotBeSpellTrapTarget(this ClientCard card)
        {
            return !card.IsDisabled() && Enum.IsDefined(typeof(ShouldNotBeSpellTrapTarget), card.Id);
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
    }
}