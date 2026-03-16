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

        FieldZone = 0x20,

        MonsterZones = 0x7f,
        MainMonsterZones = 0x1f,
        ExtraMonsterZones = 0x60,

        SpellZones = 0x1f,

        PendulumZones = 0x3,

        LinkedZones = 0x10000,
        NotLinkedZones = 0x20000;
    }
}