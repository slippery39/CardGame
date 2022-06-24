public interface IUnitPumpSystem
{
    void PumpUnit(CardInstance unit, PumpUnitEffect pumpAbility);
}

public class DefaultUnitPumpSystem: IUnitPumpSystem
{
    private CardGame cardGame;

    public DefaultUnitPumpSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void PumpUnit(CardInstance unit, PumpUnitEffect pumpAbility)
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