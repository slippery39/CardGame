using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages all the different screens and menus for the game.
/// </summary>
public class AppController : MonoBehaviour
{
    [SerializeField] private GameSetupController _gameSetupController;
    [SerializeField] private UI3DController _uiGameController;

    public void Awake()
    {
        _gameSetupController.Initialize(this);
        _uiGameController.Initialize(this);
    }

    public void GoToMainMenu()
    {
        _uiGameController.EndGame();
        _gameSetupController.Show();
    }

    public void StartGame(GameSetupOptions options)
    {
        _gameSetupController.gameObject.SetActive(false);
        _uiGameController.StartGame(options);
    }
}
