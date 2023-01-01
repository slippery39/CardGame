using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (GameService))]
public class SimGameController : MonoBehaviour
{
    [SerializeField]
    private GameService _gameService;

    // Start is called before the first frame update
    void Start()
    {
        _gameService = GetComponent<GameService>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSimulateGame(Decklist player1Deck, Decklist player2Deck)
    {
        _gameService.SetupGame(player1Deck, player2Deck);
        _gameService.StartGame();
    }
}
