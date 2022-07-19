using System;
using System.Collections.Generic;
using System.Linq;

public class SwitchPowerToughnessEffect : Effect
{
    public override string RulesText
    {
        get
        {
            return "Switch power and toughness";
        }
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with the SwitchPowerToughness effect");
            }

            var card = (CardInstance)entity;
            var mod = new ModSwitchPowerandToughness();

            //If they already have a switch power and toughness effect then it should cancel out (we will remove the existing one)
            if (card.Modifications.GetOfType<ModSwitchPowerandToughness>().Any())
            {

                card.Modifications = card.Modifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
            }
            else
            {
                cardGame.ModificationsSystem.AddModification(card, mod);
            }
        }
    }
}



