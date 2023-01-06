using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(GameService))]
public class ComputerAI : MonoBehaviour
{
    [SerializeField]
    private int playerId;

    [SerializeField]
    private string previousDebugMessage = "";

    [SerializeField]
    private int _calculations;
    [SerializeField]
    private bool _disabled;

    [SerializeField]
    private GameService _gameService;

    private IBrain _brain;

    private void Awake()
    {
        _brain = new DefaultBrain();
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameService = this.GetComponent<GameService>();
        //The AI will attempt to select his action every 0.5 seconds.
        Observable.Interval(TimeSpan.FromSeconds(0.4)).Subscribe((_) =>
        {
                TryChooseAction();
        });
    }

    private void TryChooseAction()
    {
        if (!_gameService.HasGameStarted)
        {
            return;
        }

        if (_gameService.GameOver)
        {
            return;
        }

        var cardGame = _gameService.CardGame;

        if (cardGame.ActivePlayer.PlayerId == playerId)
        {
            _gameService.ProcessAction(_brain.GetNextAction(cardGame));
        }
    }
}
