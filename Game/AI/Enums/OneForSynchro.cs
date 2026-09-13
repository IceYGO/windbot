namespace WindBot.Game.AI.Enums
{
    /// <summary>
    /// Monsters that can use cards in the hand as Synchro Material
    /// when this card on the field is used as Synchro Material.
    /// </summary>
    public enum OneForSynchro
    {
        EccentricBoy = 16825874,
        MaleficParallelGear = 74509280,
        MaraoftheNordicAlfar = 73417207,
        TGCyberMagician = 64910482,
        MechaPhantomBeastBlueImpala = 67489919,
        Tatsunoko = 55863245,
        Tatsunecro = 3096468,
        KewlTuneMix = 16509007,
        KewlTuneClip = 43904702,
        KewlTuneReco = 89392810,
        KewlTuneCue = 16387555,
        KewlTuneTrackMaker = 42781164,
        KewlTuneRotary = 17209452
    }

    /// <summary>
    /// Activate descriptions or HINT_OPSELECTED announces for effects that can
    /// Synchro Summon even when the field does not currently have Tuner + non-Tuner.
    /// Values are Util.GetStringId(cardId, offset) = cardId * 16 + offset.
    /// </summary>
    public enum OneForSynchroEffect
    {
        CrystronQuan = 93665266 * 16 + 0,
        CrystronCitree = 20050865 * 16 + 0,
        CrystronRion = 66938505 * 16 + 0,
        CrystronTristaros = 99471856 * 16 + 0,
        JunkAnchor = 25148255 * 16 + 0,
        SpeedroidOhajikid = 89326990 * 16 + 0,
        KewlTuneClip = 43904702 * 16 + 0,
        KewlTuneRemix = 88170262 * 16 + 0,
        KewlTuneB2B = 65961304 * 16 + 0,
        SpeedroidMaliciousmagnet = 62899696 * 16 + 1,
        TurboTaintedHotRodGT19 = 16769305 * 16 + 1,
        AshtraInsectPoison = 64455720 * 16 + 1,
        AshtraCursedRoar = 91444835 * 16 + 1,
        AshtraDivineDomain = 37839434 * 16 + 1
    }
}
