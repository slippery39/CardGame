
/// <summary>
/// This enum descibes which entities will be targeted based on a list of valid choices.
/// ex. If you want to target a single creature, you would use Target.
/// If you want to target all creatures at once you would use All./// 
/// </summary>
public enum TargetMode
{
    /// <summary>
    /// IN PROGRESS - NOT SURE ABOUT THIS ONE YET, MAY NEED TO CHANGE
    /// Card has no targets at all ex. Divination - Draw 2 Cards
    /// </summary>
    None,
    /// <summary>
    /// Card selects random targets ex. Deal 2 damage to 2 random creatures
    /// </summary>
    Random,
    /// <summary>
    /// You select the targets for the effect. ex. Deal 2 damage to a target creature
    /// </summary>
    Target,
    /// <summary>
    /// All targets are selected for the effect ex. Deal 2 damage to each creature
    /// </summary>
    All
}


