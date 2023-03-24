using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviour
{
    GameSetupOptions gameSetupOptions;

    [SerializeField] TMP_Dropdown _player1Dropdown;
    [SerializeField] TMP_Dropdown _player2Dropdown;
    [SerializeField] TMP_InputField _startingLifeTotalInput;
    [SerializeField] Button _startGameButton;

    public void Awake()
    {
        _startGameButton.onClick.AddListener(() =>
        {
            StartGame();
        });
    }

    public void StartGame()
    {
        var decklistDB = new FamousDecks();

        var player1Deck = decklistDB.GetByName(_player1Dropdown.options[_player1Dropdown.value].text);
        var player2Deck = decklistDB.GetByName(_player2Dropdown.options[_player2Dropdown.value].text);

        var inputText = _startingLifeTotalInput.text.Trim();
        int startingLifeTotal = Convert.ToInt32(inputText);

        gameSetupOptions = new GameSetupOptions
        {
            Player1Deck = player1Deck,
            Player2Deck = player2Deck,
            StartingLifeTotal = startingLifeTotal
        };

        var validation = Validate(gameSetupOptions);
        if (validation.Success)
        {
            Debug.Log("Game should start now!");
        }
        else
        {
            Debug.Log(validation.Message);
        }
    }

    public ValidationInfo Validate(GameSetupOptions options)
    {
        if (options.StartingLifeTotal <= 0)
        {
            return new ValidationInfo()
            {
                Message = "Starting Life Total must be greater than 0",
                Success = false
            };
        }

        return new ValidationInfo()
        {
            Success = true,
            Message = ""
        };
    }


}

public class GameSetupOptions
{
    Decklist player1Deck;
    Decklist player2Deck;
    int startingLifeTotal;
    public Decklist Player1Deck { get => player1Deck; set => player1Deck = value; }
    public Decklist Player2Deck { get => player2Deck; set => player2Deck = value; }
    public int StartingLifeTotal { get => startingLifeTotal; set => startingLifeTotal = value; }
}

public class ValidationInfo
{
    public string Message { get; set; }
    public bool Success { get; set; }
}
