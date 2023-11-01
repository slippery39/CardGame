
/// <summary>
/// This enum describes which entities will be targeted by an effect basedd on the owner of the entity.
/// </summary>
public enum TargetOwnerType
{
    /// <summary>
    /// Target entities from any owner i.e. Destroy all creatures.
    /// </summary>
    Any,
    /// <summary>
    /// Only target our entities i.e. ex. Our Creatures get +1/+1 until end of turn.
    /// </summary>
    Ours,
    /// <summary>
    /// Only target their cards i.e. Deal 2 damage to target creature an opponent controls
    /// </summary>
    Theirs
}


