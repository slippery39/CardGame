using Assets.Core;
using System.Collections.Generic;

public class TriggeredAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            string text = "";

            switch (TriggerType)
            {
                case TriggerType.SelfEntersPlay:
                    text = "When this enters play, ";
                    break;
                case TriggerType.SelfDies:
                    text = "When this dies, ";
                    break;
                case TriggerType.SelfAttacks:
                    text = "When this attacks, ";
                    break;
                case TriggerType.AtTurnStart:
                    text = "At the start of your turn, ";
                    break;
                case TriggerType.AtTurnEnd:
                    text += "At the end of the turn, ";
                    break;
                case TriggerType.SelfManaPlayed:
                    text += "Whenever you play a mana card, ";
                    break;
                case TriggerType.SomethingDies:

                    var cardFilterString = Filter.RulesTextString();

                    text += $"When a {(cardFilterString != "" ? cardFilterString : "anything")} dies, ";
                    break;
                default:
                    text += "";
                    break;
            }

            foreach (var effect in Effects)
            {
                text += effect.RulesText.LowerFirst();
                text += ",";
            }

            //Get rid of the last ",";
            text = text.Substring(0, text.Length - 1);


            return text;
        }
    }
    public TriggerType TriggerType { get; set; }
    //Filter that causes the trigger only to apply if it meets the filtering requirements (i.e. Instead of when a creature dies, When a goblin dies, When a goblin comes into play etc).
    public CardFilter Filter { get; set; } = new CardFilter();

    public TriggeredAbility(TriggerType triggerType, Effect effect)
    {
        TriggerType = triggerType;
        Effects = new List<Effect>();
        Effects.Add(effect);
    }

    public TriggeredAbility()
    {

    }
}


