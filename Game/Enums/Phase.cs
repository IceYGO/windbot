namespace WindBot.Game.Enums
{
    public enum Phase
    {
        Draw =      0x01,
        Standby =   0x02,
        Main1 =     0x04,
        Battle =    0x08,
        Damage =    0x10,
        DamageCal = 0x20,
        Main2 =     0x40,
        End =       0x80
    }
}