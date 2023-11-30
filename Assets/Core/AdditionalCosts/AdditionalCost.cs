using System.Collections.Generic;

/// <summary>
/// We need a class to handle the additional information we need to pass when we are paying an additional cost...
/// But it also needs to be abstract enough to handle various types of situations..
/// 
/// 
/// List<CardGameEntity> entitiesChosen</CardGameEntity>
/// 
/// </summary>
public class CostInfo
{
    //do we need other information here??
    public List<CardGameEntity> EntitiesChosen { get; set; }

}

public abstract class AdditionalCost
{
    public AdditionalCostType Type { get; set; }
    public int Amount { get; set; }

    public CardFilter Filter { get; set; }

    public abstract string RulesText { get; }
    public abstract bool CanPay(CardGame cardGame, Player player, CardGameEntity source);

    //NOTE - we should get rid of this one, always implement the one with CostInfo.
    //public abstract void PayCost(CardGame cardGame, Player player,CardGameEntity sourceCard);

    //NOTE - we should always implement this one going forward.
    public abstract void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo);
    public bool NeedsChoice { get; set; } = false;
    public virtual List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        return new List<CardGameEntity>();
    }
};




