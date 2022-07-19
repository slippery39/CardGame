using System.Collections.Generic;
using System.Linq;

public class FlashbackAbility : CardAbility
{
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; }
    public override string RulesText => $"Flashback : {ManaCost},{AdditionalCost.RulesText}";

    //Need to change things to have a cost, not just a mana cost.
    private string ChangeManaCost(CardGame cardGame, CardInstance cardInstance, string originalManaCost)
    {
        if (cardGame.GetZoneOfCard(cardInstance).ZoneType == ZoneType.Discard)
        {
            return ManaCost;
        }
        else
        {
            return originalManaCost;
        }
    }

    private List<ZoneType> ChangeCastZones(CardGame cardGame, CardInstance cardInstance, List<ZoneType> originalCastZones)
    {
        var modifiedCastZones = originalCastZones.ToList();
        if (!originalCastZones.Contains(ZoneType.Discard))
        {
            modifiedCastZones.Add(ZoneType.Discard);
        }

        return modifiedCastZones;
    }

    private AdditionalCost ChangeAdditionalCost(CardGame cardGame, CardInstance cardInstance, AdditionalCost originalAdditionalCost)
    {

        if (cardGame.GetZoneOfCard(cardInstance).ZoneType == ZoneType.Discard)
        {
            return new PayLifeAdditionalCost
            {
                Amount = 3
            };
        }
        else
        {
            return originalAdditionalCost;
        }
    }

    public FlashbackAbility()
    {
        this.Components.Add(new ModifyManaCostComponent(ChangeManaCost));
        this.Components.Add(new ModifyCastZonesComponent(ChangeCastZones));
        this.Components.Add(new ModifyAdditionalCostComponent(ChangeAdditionalCost));
    }
}


