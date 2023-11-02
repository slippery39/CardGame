using System;
using System.Collections.Generic;

public class AddTempAbilityEffect : Effect
{
    public override string RulesText => $@"Give {TempAbility.RulesText} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)} until end of turn";
    public CardAbility TempAbility { get; set; }
    public AddTempAbilityEffect(CardAbility tempAbility)
    {
        TempAbility = tempAbility;
        TempAbility.ThisTurnOnly = true;
        //Default Target Info
        TargetInfo = TargetInfoBuilder.TargetOwnUnit().Build();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only card instances can be effected with the add temp ability effect");
            }
            var card = (CardInstance)entity;
            //note, ideally the temp ability would be cloned from the AddTempAbilityEffect...
            card.Abilities.Add(TempAbility);
        }
    }
}


