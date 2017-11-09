using System;
using WindBot.Game.AI.Enums;

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