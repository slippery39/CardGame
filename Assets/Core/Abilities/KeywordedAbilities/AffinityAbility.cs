using System;
using System.Linq;

    public class AffinityAbility : CardAbility, IModifyManaCost
    {
        public override string RulesText => $"Affinity for artifacts"; //only for artifacts right now.

        public string ModifyManaCost(CardGame cardGame, CardInstance cardInstance, string originalManaCost)
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
        }
    }



