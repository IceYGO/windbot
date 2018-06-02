namespace WindBot.Game.AI
{
    public static class Zones
    {
        public const int z0 = 0x1,
        z1 = 0x2,
        z2 = 0x4,
        z3 = 0x8,
        z4 = 0x10,
        z5 = 0x20,
        z6 = 0x40,

        MonsterZones = 0x7f,
        MainMonsterZones = 0x1f,
        ExtraMonsterZones = 0x60,

        SpellZones = 0x1f,

        PendulumZones = 0x3;

        public static int CheckLinkedPointZones = 0x8000,//save 0-6 from right to left
        CheckMutualBotZoneCount= 0x200000,//save 0-6 from left to right
        CheckMutualEnemyZoneCount= 0x200000;
        
    }
}