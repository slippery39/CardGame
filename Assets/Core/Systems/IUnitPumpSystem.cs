public interface IUnitPumpSystem
{
    void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility);
}

public class DefaultUnitPumpSystem: IUnitPumpSystem
{
    public void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility)
    {
        unit.AddModification(new ModAddToPowerToughness
        {
            Power = pumpAbility.Power,
            Toughness = pumpAbility.Toughness,
            OneTurnOnly = true            
        });
    }
}

//TemporaryInPlayEffects
//  -could just have a "remove at end of turn flag"
//

//Modifications - Card Instances can now have "modifications" which will allow us to add temporary or permanent modifications to CardInstances.

public class ModAddToPowerToughness
{
    public bool OneTurnOnly { get; set; } = true;
    public int Power { get; set; }
    public int Toughness { get; set; }
}



//ContinuousAbilities
//-another source is giving the unit the ability.