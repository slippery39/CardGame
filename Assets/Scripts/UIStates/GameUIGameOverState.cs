using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameUIGameOverState : IGameUIState
{
    private GameUIStateMachine _stateMachine;
    public GameUIGameOverState(GameUIStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public string GetMessage()
    {
        return "";
    }

    public void HandleInput()
    {
        
    }

    public void HandleSelection(int entityId)
    {
        
    }

    public void OnApply()
    {
        Debug.Log("Game Over State has been applied");
        _stateMachine.GameController.ShowGameOverScreen();
    }

    public void OnDestroy()
    {
        
    }
}

