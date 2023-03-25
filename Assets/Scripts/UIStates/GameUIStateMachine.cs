using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IUIGameController))]
public class GameUIStateMachine : MonoBehaviour
{
    //State history?
    public IGameUIState CurrentState;
    private IUIGameController _gameController;
    public IUIGameController GameController { get => _gameController; set => _gameController = value; }
    public CardGame CardGame { get => _gameController.CardGame; }
    public GameService GameService { get => _gameController.GameService; }

    private void Awake()
    {
        _gameController = GetComponent<IUIGameController>();
    }

    private void Start()
    {
        ToIdle();
    }

    public void ChangeState(IGameUIState stateTo)
    {
        CurrentState?.OnDestroy();
        ResetUIState();
        CurrentState = stateTo;
        stateTo.OnApply();
    }

    public void ResetUIState()
    {
        //Which buttons show should depend on the state of the game UI.
        this.GameController.HideUIButtons();
    }

    public string GetMessage()
    {
        if (CurrentState == null)
        {
            return "";
        }
        return CurrentState.GetMessage();
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

    //TODO - We should actually just be passing in the action here. 
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
                    if (!playManaAction.IsValidAction(CardGame))
                    {
                        return;
                    }
                    GameService.ProcessAction(playManaAction);
                    ToIdle();
                    break;
                }
            case PlaySpellAction spellAction:
                {
                    ChangeState(new GameUICastingSpellState(this, spellAction.SourceCard, spellAction.CastModifiers));
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



