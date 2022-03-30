using UnityEngine;

[RequireComponent(typeof(UIGameController))]
public class GameUIStateMachine : MonoBehaviour
{
    private CardGame _cardGame;
    //State history?
    public IGameUIState CurrentState;
    private UIGameController _gameController;
    public UIGameController GameController { get => _gameController; set => _gameController = value; }
    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }

    private void Start()
    {
        _gameController = GetComponent<UIGameController>();
        _cardGame = _gameController.CardGame;
        ToIdle(_cardGame.ActivePlayer);
    }

    public void ChangeState(IGameUIState stateTo)
    {
        CurrentState?.OnDestroy();
        CurrentState = stateTo;
        stateTo.OnApply();
    }

    public string GetMessage()
    {
        if (CurrentState == null)
        {
            return "";
        }
        return CurrentState.GetMessage();
    }

    public void HandleSelection(int entityId)
    {
        CurrentState.HandleSelection(entityId);
    }

    public void ToIdle(Player actingPlayer)
    {
        ChangeState(new GameUIIdleState(this, actingPlayer));
    }
}

