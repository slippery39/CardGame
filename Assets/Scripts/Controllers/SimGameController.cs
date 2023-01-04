using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;


public class SimGameController : MonoBehaviour
{    
    [SerializeField] SimService _simService;

    private void Awake()
    {
        _simService = new SimService();
    }

    // Start is called before the first frame update
    void Start()
    {
        //_simService = GetComponent<GameService>();
    }

    public async void SimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        Debug.Log("Game has started simulation");
        await Task.Run(() =>
        {
            _simService.SimulateGame(player1Deck, player2Deck);
        });
        Debug.Log("Game has ended simulation");
    }

    /*
    public void SimulateNGames(Decklist player1Deck, Decklist player2Deck, int numberOfGames)
    {
        int gameId = 1;
        Debug.Log($"Simulation has started for game {gameId}");
        StartSimulateGame(player1Deck, player2Deck);
    }
    */
}
