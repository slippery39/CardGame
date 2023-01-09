using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IZoneChangeSystem
{
    public void MoveToZone(CardInstance card, IZone zoneTo);
}


public class DefaultZoneChangeSystem : CardGameSystem, IZoneChangeSystem
{
    public DefaultZoneChangeSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void MoveToZone(CardInstance card, IZone zoneTo)
    {
        var currentZone = card.CurrentZone;//cardGame.GetZones().Where(zone => zone.Cards.Contains(card)).FirstOrDefault();

        if (currentZone == null)
        {
            throw new Exception("Could not find current zone of card in DefaultZoneChangeSystem");
        }

        //reset summoning sickness when changing zones.
        card.IsSummoningSick = true;

        //remove any continuous effects on the card
        card.ContinuousEffects = new List<ContinuousEffect>();

        //remove any continuous effects in play that are from the associated card
        //var unitsInPlay = cardGame.GetUnitsInPlay();

        currentZone.Remove(card);
        zoneTo.Add(card);
        card.CurrentZone = zoneTo;

        //Apply any ETB Triggers
        if (currentZone.ZoneType != ZoneType.InPlay && zoneTo.ZoneType == ZoneType.InPlay)
        {
            cardGame.HandleTriggeredAbilities(new List<CardInstance> { card }, TriggerType.SelfEntersPlay);
        }

        //Apply Death Triggers
        if ((currentZone.ZoneType == ZoneType.InPlay) && zoneTo is DiscardPile)
        {
            card.DamageTaken = 0;
            OnDeathTriggers(card);
        }
    }

    private void OnDeathTriggers(CardInstance card)
    {

        //These are things that might happen when a card dies, that don't necessarily use the stack.
        //They would be defined in the ability itself, not as a trigger.
        var onDeathAbilities = card.Abilities.GetOfType<IOnDeath>();

        foreach (var ability in onDeathAbilities)
        {
            ability.OnDeath(cardGame, card);
        }

        //Self dies triggers
        card.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SelfDies).ToList().ForEach(ab =>
        {
            //fire the trigger... 
            cardGame.EffectsProcessor.ApplyEffects(cardGame.GetOwnerOfCard(card), card, ab.Effects, new List<CardGameEntity>());
        });

        //Something dies triggers, this needs to come from all the cards in play.
        //

        var cardsInPlay = cardGame.GetCardsInPlay();

        //We have to get all the abilities

        foreach (var card2 in cardsInPlay)
        {
            card2.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SomethingDies).ToList().ForEach(ab =>
            {
                //fire the trigger...
                var thingThatDies = new List<CardInstance> { card };
                var filteredList = CardFilter.ApplyFilter(thingThatDies, ab.Filter);

                if (filteredList.Any())
                {
                    cardGame.EffectsProcessor.ApplyEffects(cardGame.GetOwnerOfCard(card2), card2, ab.Effects, new List<CardGameEntity>());
                }
            });
        }
    }
}

