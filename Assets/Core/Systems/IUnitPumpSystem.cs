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


//ContinuousAbilities
//-another source is giving the unit the ability.