using System.Linq;

public class ContinuousEffect
{
    public CardInstance SourceCard { get; set; }
    
    /// <summary>
    /// The ID of the source ability that is causing this continuous effect.
    /// Note that this assumes the source ability will have an AbilityIdentifier Component attached to it.
    /// </summary>
    public int SourceAbilityId { get; set; }
    public StaticAbility SourceAbility { get; set; }
    public Effect SourceEffect { get; set; }
}