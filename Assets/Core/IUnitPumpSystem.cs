public interface IUnitPumpSystem
{
    void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitAbility pumpAbility);
}

public class DefaultUnitPumpSystem: IUnitPumpSystem
{
    public void PumpUnit(CardGame cardGame, CardInstance unit, PumpUnitAbility pumpAbility)
    {
        unit.Power += pumpAbility.Power;
        unit.Toughness += pumpAbility.Toughness;
    }
}