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
    }
}