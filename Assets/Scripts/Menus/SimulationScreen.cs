using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SimulationScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _player1DeckSelect;
    [SerializeField]
    private TMP_Dropdown _player2DeckSelect;
    [SerializeField]
    private TMP_InputField _numberOfGames;

    private SimService _simService;

    private bool _isSimulating = false;

    private void Awake()
    {
        _simService = new SimService();
    }

    public void SimulateGame()
    {
        Debug.Log("Game is simulating...");


        if (_numberOfGames.text.Trim() == "")
        {
            Debug.LogError("Number of games needs to be at least 1");
            return;
        }
        var numberOfGames = Convert.ToInt32(_numberOfGames.text);
        if (numberOfGames == 0)
        {
            Debug.LogError("Number of games needs to be at least 1");
        }

        var deckDB = new FamousDecks();
        var player1Deck = deckDB.GetByName(_player1DeckSelect.options[_player1DeckSelect.value].text);
        var player2Deck = deckDB.GetByName(_player2DeckSelect.options[_player2DeckSelect.value].text);

        _isSimulating = true;
        Task.Run(() =>
        {
            _simService.SimulateNGames(player1Deck, player2Deck, numberOfGames);
            _isSimulating = false;
        });
    }
}
