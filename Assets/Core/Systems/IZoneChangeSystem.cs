using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IZoneChangeSystem
{
    public void MoveToZone(CardInstance card, IZone zoneTo);
}


public class DefaultZoneChangeSystem : IZoneChangeSystem
{
    private CardGame cardGame;

    public DefaultZoneChangeSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void MoveToZone(CardInstance card, IZone zoneTo)
    {
        var currentZone = cardGame.GetZones().Where(zone => zone.Cards.Contains(card)).FirstOrDefault();

        if (currentZone == null)
        {
            throw new Exception("Could not find current zone of card in DefaultZoneChangeSystem");
        }

        //reset summoning sickness when changing zones.
        card.IsSummoningSick = true;

        //remove any continuous effects on the card
        card.ContinuousEffects = new List<ContinuousEffect>();

        //remove any continuous effects in play that are from the associated card
        var unitsInPlay = cardGame.GetUnitsInPlay();

        //Its right here?? yay.
        /*
        foreach (var unit in unitsInPlay)
        {
            unit.ContinuousEffects = unit.ContinuousEffects.Where(ce => ce.SourceCard != card).ToList();
        }
        */

        currentZone.Remove(card);
        zoneTo.Add(card);

        //Apply Death Triggers
        if ((currentZone.ZoneType == ZoneType.InPlay || currentZone.ZoneType == ZoneType.Items) && zoneTo is DiscardPile)
        {
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

        cardsInPlay.SelectMany(card => card.GetAbilitiesAndComponents<TriggeredAbility>().Where(ab => ab.TriggerType == TriggerType.SomethingDies)).ToList().ForEach(ab =>
          {
              //fire the trigger...

              var thingThatDies = new List<CardInstance> { card };
              var filteredList = CardFilter.ApplyFilter(thingThatDies, ab.Filter);

              if (filteredList.Any())
              {
                  cardGame.EffectsProcessor.ApplyEffects(cardGame.GetOwnerOfCard(card), card, ab.Effects, new List<CardGameEntity>());
              }
          });
    }
}

