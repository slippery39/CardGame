using System.Collections.Generic;

public class SuspendAbility : ActivatedAbility
{
    public override string RulesText => $"Suspend for {Turns} turns";
    public int Turns { get; set; }

    public SuspendAbility(int turnsToSuspend)
    {
        ActivationZone = ZoneType.Hand;
        Turns = turnsToSuspend;
        Effects = new List<Effect>
        {
            new SuspendEffect
            {
                Turns = turnsToSuspend
            }
        };
    }
};

public class SuspendEffect : Effect
{
    public override string RulesText => "";
    public int Turns { get; set; }

    public CardInstance SourceCard { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        cardGame.ZoneChangeSystem.MoveToZone(source, player.Exile);
        source.Modifications.Add(new SuspendedComponent
        {
            TurnsLeft = Turns,
            Source = source
        });
    }
}

public class SuspendedComponent : Modification, IOnTurnStart
{
    public int TurnsLeft { get; set; } //the counters on it

    public CardInstance Source { get; set; }
    public void OnTurnStart(CardGame cardGame, Player player, CardInstance source)
    {

        //Remove a 'counter'  from the source.
        TurnsLeft--;

        cardGame.Log($"Turns left for suspend : {this.TurnsLeft}");

        if (TurnsLeft == 0)
        {
            cardGame.Log($"Suspend has been finished!, trying to add to stack");
            //Also remove this component from the card.
            Source.RemoveModification(this);
            //Move the card onto the stack.

            var playCardAction = CardGameAction.CreatePlayCardAction(source);
            cardGame.ResolvingSystem.Add(playCardAction,source);
        }
    }
}



