using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameUIIdleState : IGameUIState
{

    private CardGame _cardGame;
    private Player _actingPlayer;
    private GameUIStateMachine _stateMachine;

    private List<KeyCode> inputKeys = new List<KeyCode>
    {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
    };

    private Dictionary<KeyCode, CardInstance> KeyCodeToCardInHandMap()
    {
        var cardsInHand = _actingPlayer.Hand.Cards;
        var dict = new Dictionary<KeyCode, CardInstance>();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            dict.Add(inputKeys[i], cardsInHand[i]);
        }

        return dict;
    }

    public GameUIIdleState(GameUIStateMachine stateMachine, Player actingPlayer)
    {
        _stateMachine = stateMachine;
        _cardGame = stateMachine.CardGame;
        _actingPlayer = actingPlayer;
    }

    public string GetMessage()
    {
        return "Play a card from your hand (using numeric keys) or press B to fight";
    }

    private void HandleCardSelectedFromHand(CardInstance card)
    {
        //Figure out what type of card it is, and move to the appropriate state.
        if (card.CurrentCardData is UnitCardData)
        {
            _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, _actingPlayer, card));
        }
        else
        {
            //TODO - need to update this.
            if (_cardGame.TargetSystem.SpellNeedsTargets(_cardGame, _actingPlayer, card))
            {
                _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, _actingPlayer, card));
            }
            else
            {
                _cardGame.PlayCardFromHand(_actingPlayer, card, 0);
            }
        }
    }

    public void HandleInput()
    {
        var keyCodeToCardInHand = KeyCodeToCardInHandMap();

        foreach (var key in keyCodeToCardInHand.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                var card = keyCodeToCardInHand[key];
                HandleCardSelectedFromHand(card);
            }
        }
    }

    public void OnApply()
    {
    }
    public void OnDestroy()
    {
    }

    public void HandleSelection(int entityId)
    {
        //we need to get the card from hand?
        var cardFromHand = _actingPlayer.Hand.Cards.Where(card => card.EntityId == entityId).FirstOrDefault();

        if (cardFromHand == null)
        {
            Debug.Log("Invalid Selection");
            return;
        }
        HandleCardSelectedFromHand(cardFromHand);
    }
}

