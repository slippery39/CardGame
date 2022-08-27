using System;
using System.Collections.Generic;
using System.Linq;


public class SuspendAbility : ActivatedAbility
{
    public override string RulesText => $"Suspend {Turns}";
    public int Turns { get; set; }



    public SuspendAbility()
    {
        ActivateZone = ZoneType.Hand;
        Effects = new List<Effect>
        {
            new SuspendEffect
            {
                Turns = Turns
            }
        };
    }
};

public class SuspendEffect : Effect
{
    public override string RulesText => "";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        //Move to exile.
        cardGame.ZoneChangeSystem.MoveToZone(source, player.Exile);
        //Put a suspended component on it
        //suspended component will have counters
        
        //suspended component has an ITurnStart interface
        //on iturnstart remove a counter
        //if it has 0 counters, cast the card / put it into play.
    }
}



public class AffinityAbility : CardAbility
{
    public override string RulesText => $"Affinity for artifacts"; //only for artifacts right now.

    private string ChangeManaCost(CardGame cardGame, CardInstance cardInstance, string originalManaCost)
    {
        //We need to count the amount of artifacts in play for the controller.
        var cardOwner = cardGame.GetOwnerOfCard(cardInstance);
        var artifactCounts = cardGame.GetCardsInPlay(cardOwner).Where(c => c.Subtype.ToLower() == "artifact").Count();

        //Subtract the artifact counts from the colorless mana;
        var manaCostAsObj = new Mana(originalManaCost);

        manaCostAsObj.ColorlessMana -= artifactCounts;
        manaCostAsObj.ColorlessMana = Math.Max(0, manaCostAsObj.ColorlessMana);

        return manaCostAsObj.ToManaString();
    }

    public AffinityAbility()
    {
        this.Components.Add(new ModifyManaCostComponent(ChangeManaCost));
    }
}


