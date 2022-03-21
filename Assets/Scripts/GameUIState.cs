using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class GameUIStateMachine
{
    private CardGame _cardGame;
    //State history?
    public IGameUIState CurrentState;
    public GameUIStateMachine(CardGame cardGame)
    {
        _cardGame = cardGame;
        ToIdle(_cardGame.ActivePlayer);
    }

    public void ChangeState(IGameUIState stateTo)
    {
        CurrentState = stateTo;
    }

    public string GetMessage()
    {
        return CurrentState.GetMessage();
    }

    public void ToIdle(Player actingPlayer)
    {
        ChangeState(new GameUIIdleState(this, _cardGame, actingPlayer));
    }
}

public interface IGameUIState
{
    void HandleInput();
    string GetMessage();
}


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

    public GameUIIdleState(GameUIStateMachine stateMachine, CardGame cardGane, Player actingPlayer)
    {
        _cardGame = cardGane;
        _actingPlayer = actingPlayer;
        _stateMachine = stateMachine;
    }

    public string GetMessage()
    {
        return "Play a card from your hand (using numeric keys) or press B to fight";
    }

    public void HandleInput()
    {
        var keyCodeToCardInHand = KeyCodeToCardInHandMap();

        foreach (var key in keyCodeToCardInHand.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                var card = keyCodeToCardInHand[key];
                //Figure out what type of card it is, and move to the appropriate state.
                if (card.CurrentCardData is UnitCardData)
                {
                    _stateMachine.ChangeState(new GameUISummonUnitState(_stateMachine, _cardGame, _actingPlayer, card));
                }
                else
                {
                    //TODO - need to update this.
                    if (_cardGame.TargetSystem.SpellNeedsTargets(_cardGame, _actingPlayer, card))
                    {
                        _stateMachine.ChangeState(new GameUICastingSpellState(_stateMachine, _cardGame, _actingPlayer, card));
                    }
                    else
                    {
                        _cardGame.PlayCardFromHand(_actingPlayer, card, 0);
                    }
                }
            }
        }
    }
}

public class GameUICastingSpellState : IGameUIState
{

    private CardGame _cardGame;
    private Player _actingPlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _spellToCast;

    public GameUICastingSpellState(GameUIStateMachine stateMachine, CardGame cardGame, Player actingPlayer, CardInstance spellToCast)
    {
        _cardGame = cardGame;
        _stateMachine = stateMachine;
        _actingPlayer = actingPlayer;
        _spellToCast = spellToCast;
    }

    private List<KeyCode> inputKeys = new List<KeyCode>
    {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Q,
            KeyCode.W
    };

    public Dictionary<KeyCode, CardGameEntity> KeyCodeToEntityMap()
    {
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _actingPlayer, _spellToCast);
        var keysToValidTargets = new Dictionary<KeyCode, CardGameEntity>();

        if (inputKeys.Count < validTargets.Count)
        {
            throw new Exception("Not enough keys for the amount of targets");
        }

        for (var i = 0; i < validTargets.Count; i++)
        {
            keysToValidTargets.Add(inputKeys[i], validTargets[i]);
        }

        return keysToValidTargets;
    }

    public string GetMessage()
    {
        var targetsForMessage = KeyCodeToEntityMap().ToList().Select(keyValuePair =>
        {
            return $@"{Environment.NewLine} {keyValuePair.Key.ToString()} : {keyValuePair.Value.Name} ";
        }).ToList();

        return $@"Choose a target for spell {_spellToCast.Name}  to(use numeric keys) {String.Join(",", targetsForMessage)} ";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle(_actingPlayer);
            return;
        }

        var keyValuePair = KeyCodeToEntityMap().ToList();

        for (var i = 0; i < keyValuePair.Count; i++)
        {
            var keyCode = keyValuePair[i].Key;
            if (Input.GetKeyDown(keyCode))
            {
                _cardGame.PlayCardFromHand(_actingPlayer, _spellToCast, keyValuePair[i].Value.EntityId);
                _stateMachine.ToIdle(_actingPlayer);
            }
        }
    }
}

public class GameUISummonUnitState : IGameUIState
{
    private CardGame _cardGame;
    private Player _actingPlayer;
    private GameUIStateMachine _stateMachine;
    private CardInstance _unitToSummon;

    private List<KeyCode> _inputKeys = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    public GameUISummonUnitState(GameUIStateMachine stateMachine, CardGame cardGame, Player actingPlayer, CardInstance unitToSummon)
    {
        _stateMachine = stateMachine;
        _cardGame = cardGame;
        _actingPlayer = actingPlayer;
        _unitToSummon = unitToSummon;
    }

    private Dictionary<KeyCode, Lane> GetKeyCodeToLaneMap()
    {

        var map = new Dictionary<KeyCode, Lane>();

        for (var i = 0; i < _inputKeys.Count; i++)
        {
            map.Add(_inputKeys[i], _actingPlayer.Lanes[i]);
        }

        return map;

    }

    public string GetMessage()
    {
        return $@"Choose a lane to summon {_unitToSummon.Name} to(use numeric keys)";
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stateMachine.ToIdle(_actingPlayer);
            return;
        }

        var keyCodeToLaneMap = GetKeyCodeToLaneMap();

        foreach (var key in keyCodeToLaneMap.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                var lane = keyCodeToLaneMap[key];
                //Try to summon the unit.
                _cardGame.PlayCardFromHand(_cardGame.Player1, _unitToSummon, lane.EntityId);
                _stateMachine.ToIdle(_actingPlayer);
            }
        }
    }
}

