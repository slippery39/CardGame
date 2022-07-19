using System;
using System.Linq;

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


