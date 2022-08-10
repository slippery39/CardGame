using System;
using System.Collections.Generic;

public class AddTempManaEffect : Effect
{
    public override string RulesText => $"Add {ManaToAdd} until end of turn";
    public string ManaToAdd { get; set; } = "0";
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is Player))
            {
                throw new Exception("Error : only players can gain mana");
            }

            cardGame.ManaSystem.AddTemporaryMana(player, ManaToAdd);
        }

    }
}


