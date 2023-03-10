using System.Collections.Generic;

public interface IGameUIState
{
    void HandleInput();
    string GetMessage();
    void OnApply();
    void OnDestroy();

    void OnUpdate()
    {

    }
    void HandleSelection(int entityId);
}
