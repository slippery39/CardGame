using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(GameService))]
public class SimGameController : MonoBehaviour
{
    [SerializeField]
    private GameService _gameService;

    // Start is called before the first frame update
    void Start()
    {
        _gameService = GetComponent<GameService>();
    }

    public void StartSimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        _gameService.SetupGame(player1Deck, player2Deck);
        _gameService.OnGameOverObservable.Subscribe(c =>
        {
            Debug.Log($"Game has ended. {c.WinLoseSystem.GetGameOverInfo().Winner.Name} is the winner");
        });

        _gameService.StartGame();
    }

    public void SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {
        int gameId = 1;
        Debug.Log($"Simulation has started for game {gameId}");
        StartSimulateGame(player1Deck, player2Deck);
    }
}
