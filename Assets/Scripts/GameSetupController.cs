using Assets.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviour
{
    GameSetupOptions gameSetupOptions;

    [Header("Decklist References")]
    [SerializeField] private ListOfDecklistsScriptableObject decklists;

    [Header("Control References")]
    [SerializeField] DeckSelectionDropdown _player1Dropdown;
    [SerializeField] DeckSelectionDropdown _player2Dropdown;
    [SerializeField] TMP_InputField _startingLifeTotalInput;
    [SerializeField] Button _startGameButton;

    [Header("Other References")]
    [SerializeField] AppController _appController;

    public void Awake()
    {
        if (decklists == null)
        {
            Debug.LogError("No decklists have been assigned to the GameSetupController");
        }

        _startGameButton.onClick.AddListener(() =>
        {
            StartGame();
        });

        _player1Dropdown.Decklists = decklists;
        _player2Dropdown.Decklists = decklists;
    }

    public void Initialize(AppController appController)
    {
        _appController = appController;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void StartGame()
    {

        DecklistScriptableObject deck1 = _player1Dropdown.Selected;
        DecklistScriptableObject deck2 = _player2Dropdown.Selected;

        var player1Deck = deck1.ToDecklist();
        var player2Deck = deck2.ToDecklist();

        //var player1Deck = decklistDB.GetByName(_player1Dropdown.dropdown.options[_player1Dropdown.dropdown.value].text);
        //var player2Deck = decklistDB.GetByName(_player2Dropdown.dropdown.options[_player2Dropdown.dropdown.value].text);

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
            _appController.StartGame(gameSetupOptions);
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

public class ValidationInfo
{
    public string Message { get; set; }
    public bool Success { get; set; }
}
