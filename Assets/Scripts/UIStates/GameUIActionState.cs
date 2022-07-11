using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class GameUIActionState : IGameUIState
{
    //IGameUIState Interface Methods.
    public abstract string GetMessage();
    public abstract void HandleInput();
    public abstract void HandleSelection(int entityId);
    public abstract void OnApply();
    public abstract void OnDestroy();

    protected IGameUIState _internalState;

    //Game UI Action Methods.
    public List<CardGameEntity> SelectedTargets { get; set; }
    public List<CardGameEntity> SelectedChoices { get; set; }

    public bool NeedsTargets { get; set; }
    public bool NeedsCostChoices { get; set; }

    public abstract void ChangeToSelectTargetState();
    public abstract void ChangeToCostChoosingState();
    public abstract void DoAction();

    public void ChangeState(IGameUIState stateTo)
    {
        _internalState?.OnDestroy();
        _internalState = stateTo;
        stateTo.OnApply();
    }
}

