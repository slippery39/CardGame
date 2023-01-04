using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;


public class SimGameController : MonoBehaviour
{    
    [SerializeField] SimService _simService;
    // Start is called before the first frame update
    void Start()
    {
        //_simService = GetComponent<GameService>();
    }

    public void StartSimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        /*
        _simService.SetupGame(player1Deck, player2Deck);
        _simService.OnGameOverObservable.Subscribe(c =>
        {
            Debug.Log($"Game has ended. {c.WinLoseSystem.GetGameOverInfo().Winner.Name} is the winner");
        });

        _simService.StartGame();
        */
    }

    public void SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {
        int gameId = 1;
        Debug.Log($"Simulation has started for game {gameId}");
        StartSimulateGame(player1Deck, player2Deck);
    }
}
