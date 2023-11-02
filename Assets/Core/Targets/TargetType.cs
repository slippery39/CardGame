public enum TargetType
{
    None,
    PlayerSelf, //Player
    //UnitSelf, //Self Unit
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
    Players,
    /// <summary>
    /// Targets cards in players hands
    /// </summary>
    CardsInHand,
    /// <summary>
    /// Targets the source that the effect came from. Ex. Self pump like Basking Rootwalla or Wild Mongrel
    /// </summary>
    Source
    //UnitSelf, //Self Unit

}


