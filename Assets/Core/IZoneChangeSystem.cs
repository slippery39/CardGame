using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IZoneChangeSystem
{
    public void MoveToZone(CardGame cardGame,CardInstance card, IZone zoneTo);
}


public class DefaultZoneChangeSystem : IZoneChangeSystem
{
    public void MoveToZone(CardGame cardGame, CardInstance card, IZone zoneTo)
    {

        var currentZone = cardGame.GetZones().Where(zone => zone.Cards.Contains(card)).FirstOrDefault();

        if (currentZone == null)
        {
            throw new Exception("Could not find current zone of card in DefaultZoneChangeSystem");
        }

        //reset summoning sickness when changing zones.
        card.IsSummoningSick = true;

        currentZone.Remove(card);
        zoneTo.Add(card);
        
        //TODO - in which zone is the card currently located?
    }
}

