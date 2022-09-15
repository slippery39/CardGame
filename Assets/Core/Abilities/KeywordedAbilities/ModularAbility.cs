using System.Linq;


public class ModularAbility : CardAbility, IOnSummon, IOnDeath
{
    public int Amount { get; set; }
    public override string RulesText => $"Modular {Amount}";

    public void OnSummoned(CardGame cardGame, CardInstance source)
    {
        cardGame.CountersSystem.AddPlusOnePlusOneCounter(source, Amount);
    }

    public void OnDeath(CardGame cardGame, CardInstance source)
    {
        //We want to move our +1/+1 counters to a new artifact creature, prioritizing another modular creature if available.
        var creaturesInPlay = cardGame.GetOwnerOfCard(source).GetUnitsInPlay();

        var modularCreatures = creaturesInPlay.Where(c => c.GetAbilitiesAndComponents<ModularAbility>().Any());

        var amountOfCounters = source.Counters.GetOfType<PlusOnePlusOneCounter>().Count();

        if (modularCreatures.Any())
        {
            //pick a random one
            var selectedCreature = modularCreatures.Randomize().ToList()[0];
            cardGame.CountersSystem.AddPlusOnePlusOneCounter(selectedCreature, amountOfCounters);
        }
        else
        {
            var artifactCreatures = creaturesInPlay.Where(c => c.Subtype == "Artifact").ToList();

            if (artifactCreatures.Any())
            {
                var selectedCreature = artifactCreatures.Randomize().ToList()[0];
                cardGame.CountersSystem.AddPlusOnePlusOneCounter(selectedCreature, amountOfCounters);
            }
        }
    }
}


