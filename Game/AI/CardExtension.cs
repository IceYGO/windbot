using System;
using WindBot.Game.AI.Enums;

namespace WindBot.Game.AI
{
    public static class CardExtension
    {
        public static bool IsMonsterInvincible(this ClientCard card)
        {
            return Enum.IsDefined(typeof(InvincibleMonster), card.Id);
        }

        public static bool IsMonsterDangerous(this ClientCard card)
        {
            return Enum.IsDefined(typeof(DangerousMonster), card.Id);
        }

        public static bool IsSpellNegateAttack(this ClientCard card)
        {
            return Enum.IsDefined(typeof(NegateAttackSpell), card.Id);
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