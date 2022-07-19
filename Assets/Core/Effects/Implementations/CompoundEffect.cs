using System.Collections.Generic;
using System.Linq;
//Represents an effect with multiple components.
public class CompoundEffect : Effect
{
    public List<Effect> Effects { get; set; }
    public override string RulesText => string.Join("\r\n", Effects.Select(e => e.RulesText));
}


