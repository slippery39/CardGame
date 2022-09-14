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
        ToIdle();
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

    public void HandleAction(CardGameAction actionSelected)
    {
        switch (actionSelected)
        {
            case PlayUnitAction unitAction:
                {
                    ChangeState(new GameUISummonUnitState(this, unitAction.SourceCard));
                    break;
                }
            case PlayManaAction playManaAction:
                {
                    if (!playManaAction.IsValidAction(_cardGame))
                    {
                        return;
                    }
                    _cardGame.ProcessAction(playManaAction);
                    ToIdle();
                    break;
                }
            case PlaySpellAction spellAction:
                {
                    ChangeState(new GameUICastingSpellState(this, spellAction.SourceCard));
                    break;
                }
            //TODO - still need to make it so cards might have more than 1 ability.
            case ActivateAbilityAction activateAbilityAction:
                {
                    ChangeState(new GameUIActivatedAbilityState(this, activateAbilityAction.SourceCard));
                    break;
                }
            default:
                {
                    Debug.Log($"Unknown action type {actionSelected.GetType()} in GameUIChooseCardActionState");
                    break;
                }
        }
    }

    public void HandleSelection(int entityId)
    {
        CurrentState.HandleSelection(entityId);
    }

    public void ToIdle()
    {
        ChangeState(new GameUIIdleState(this));
    }


}

