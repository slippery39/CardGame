using System.Collections.Generic;
//Other creatures you control get +1/+1
//This creature gets +0/+2 as long as its your turn
//Lhurgoyf's Power and Toughness are equal to the number of creatures in all graveyards (or boneyard wurm)

public class StaticAbility : CardAbility
{
    public override string RulesText { get; }

    //public StaticAbilityCondition Condition { get; set; } 

    public StaticAbilityType EffectType { get; set; }

    public List<StaticAbilityEffect> Effects { get; set; }

}

public enum StaticAbilityType
{
    Self,
    OtherCreaturesYouControl
}

//each unit should have an instance of the effect

public abstract class StaticAbilityEffect
{

}

public class StaticPumpEffect : StaticAbilityEffect
{
    public int Power { get; set; }
    public int Toughness { get; set; }
}







