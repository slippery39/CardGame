public interface IGameUIState
{
    void HandleInput();
    string GetMessage();
    void OnApply();
    void OnDestroy();
}

