public interface IUnitPumpSystem
{
    void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility);
}

public class DefaultUnitPumpSystem: IUnitPumpSystem
{
    public void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitEffect pumpAbility)
    {
        if (pumpAbility.Power > 0)
        {
            unit.Power += pumpAbility.Power;
        }
        if (pumpAbility.Toughness > 0)
        {
            unit.Toughness += pumpAbility.Toughness;
        }
    }
}