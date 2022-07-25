using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IStateBasedEffectSystem
{
    void CheckStateBasedEffects();
}


public class DefaultStateBasedEffectSystem : IStateBasedEffectSystem
{
    private CardGame cardGame;

    public DefaultStateBasedEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void CheckStateBasedEffects()
    {
        var units = cardGame.GetUnitsInPlay();

        //Check Toughness of all creatures in play.
        //Keep checking until there is no more to check?
        foreach (var unit in units)
        {
            //State Based effect, all units with current toughness 0 or less get moved to the discard pile.
            if (unit.Toughness - unit.DamageTaken <= 0)
            {
                var owner = cardGame.GetOwnerOfCard(unit);
                cardGame.ZoneChangeSystem.MoveToZone(unit, owner.DiscardPile);
            }
        }

        //Apply any static effects

        //TODO - Static Effects can also be applied from the graveyard and potentially other zones now...
        //need to check graveyard for static abilitys.

        var cardsInGraveyards = cardGame.Players.Select(p => p.DiscardPile).SelectMany(discard => discard.Cards);

        foreach (var unit in units.Concat(cardsInGraveyards))
        {
            var unitStaticAbilities = unit.GetAbilitiesAndComponents<StaticAbility>();
            if (unitStaticAbilities.Count > 0)
            {
                foreach (var sAbility in unitStaticAbilities)
                {
                    if (cardGame.IsInZone(unit, sAbility.ApplyWhenIn))
                    {
                        cardGame.ContinuousEffectSystem.Apply(unit, sAbility);
                    }
                }
            }
        }

        //Remove any static effects

        foreach (var unit in units)
        {
            var continousEffectsOnUnit = unit.ContinuousEffects;

            if (continousEffectsOnUnit.Count == 0)
            {
                continue;
            }


            //This is looking for effects where the unit is not in play.

            //What we need is to look for effects where the unit is not in the ability effect zone.

            //Working on this.

            Func<ContinuousEffect, bool> GetContinuousEffectsToRemove = (ContinuousEffect ce) =>
            {
                var cardsInPlayAndDiscard = units.Concat(cardsInGraveyards);
                var zoneOfSourceCard = cardGame.GetZoneOfCard(ce.SourceCard);
                return zoneOfSourceCard.ZoneType != ce.SourceAbility.ApplyWhenIn;
            };

            var continousEffectsToRemove = continousEffectsOnUnit
                 .Where(ce => GetContinuousEffectsToRemove(ce)
            ).ToList();

            foreach (var effect in continousEffectsToRemove)
            {
                cardGame.ContinuousEffectSystem.RemoveContinousEffects(effect.SourceCard);
            };


            /*
            continousEffectsOnUnit.ForEach(effect =>
            {
                if (!units.Contains(effect.SourceCard))
                {
                    cardGame.Log("Do i ever reach the point where I am removing continous effects?");
                    cardGame.ContinuousEffectSystem.RemoveContinousEffects(cardGame, effect.SourceCard);
                }
            });*/
        }
    }
}


