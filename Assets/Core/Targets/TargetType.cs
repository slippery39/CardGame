public enum TargetType
{
    None,
    PlayerSelf, //Player
    CardsInHand,
    UnitSelf, //Self Unit
    OpenLane,
    OpenLaneBesideUnit, //mainly for token creation, tries to place the token nearest left or right to the unit that is creating it.
    //NEW TARGET TYPES FOR UPDATED SYSTEM HERE:
    /// <summary>
    /// Targets units and/or players, for new TargetInfo System
    /// </summary>
    UnitsAndPlayers,
    /// <summary>
    /// Targets just units, for new TargetInfo System
    /// </summary>
    Units,
    /// <summary>
    /// Targets just players, for new TargetInfo System
    /// </summary>
    Players
}


