public interface IUnitPumpSystem
{
    void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility);
}

public class DefaultUnitPumpSystem: IUnitPumpSystem
{
    public void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility)
    {
        unit.Power += pumpAbility.Power;
        unit.Toughness += pumpAbility.Toughness;
    }
}