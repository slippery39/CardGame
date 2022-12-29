using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIGameController : MonoBehaviour
{
    [SerializeField]
    private CardGame _cardGame;

    [SerializeField]
    private TextMeshPro _turnIndicator;

    [SerializeField]
    private TextMeshProUGUI _actionStateIndicator;

    [SerializeField]
    private GameUIStateMachine _stateMachine;

    [SerializeField]
    private UIPlayerBoard _player1Board;

    [SerializeField]
    private UIPlayerBoard _player2Board;

    [SerializeField]
    private UIChooseActionPopup _chooseActionChoicePopup;

    [SerializeField]
    private Button _nextTurnButton;


    //private ZoneViewer _zonePopupWindow;

    [SerializeField]
    private UICard2D _cardPopup;

    [SerializeField]
    private GameObject _zonePopupWindow;

    [SerializeField]
    private UIEventLog _gameLog;


    [SerializeField]
    private GameService _gameService;
    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }
    public GameService GameService { get => _gameService; set => _gameService = value; }

    //Singleton Pattern, should only be one game controller per unity scene.
    public static UIGameController Instance;

    private void Awake()
    {
        Instance = this;

        //TODO - Game Service Update - This should be made through the GameService and updated via an Observable.
        //_cardGame = new CardGame();
    }
    internal void ShowActionChoicePopup(List<CardGameAction> actions)
    {
        //TODO - have a list of actions 
        _chooseActionChoicePopup.gameObject.SetActive(true);
        _chooseActionChoicePopup.SetCard(actions);
    }

    internal void CloseActionChoicePopup()
    {
        _chooseActionChoicePopup.gameObject.SetActive(false);
    }




    void Start()
    {
        InitEventHandlers();
        LogCardStats();
        //Check to see if any cards exist that don't have images.
        CheckForCardsWithoutImages();
        CheckForCardsWithoutManaCosts();
        Tests.Test_CloningReferencesAreCorrect();
        _stateMachine = GetComponent<GameUIStateMachine>();
    }

    private void InitEventHandlers()
    {
        _nextTurnButton.onClick.AddListener(() => HandleNextTurnButtonClick());
    }

    private void LogCardStats()
    {
        var allCards = new CardDatabase();
        Debug.Log($"Total Number of cards : {allCards.GetAll().Count}");
    }

    private FightAction CreateFightAction(int laneIndex)
    {
        return new FightAction
        {
            Player = _cardGame.ActivePlayer,
            LaneIndex = laneIndex
        };
    }


    //TODO - Game Service Update - Actions should be sent through the Service instead of being directly sent to the card game.
    private void Update()
    {
        if (!_gameService.HasGameStarted || _cardGame == null)
        {
            return;
        }

        if (_cardGame.CurrentGameState == GameState.WaitingForAction)
        {
            if (_stateMachine?.CurrentState is GameUIChoiceAsPartOfResolveState)
            {
                _stateMachine.ToIdle();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _gameService.ProcessAction(CreateFightAction(0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _gameService.ProcessAction(CreateFightAction(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _gameService.ProcessAction(CreateFightAction(2));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _gameService.ProcessAction(CreateFightAction(3));
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _gameService.ProcessAction(CreateFightAction(4));
            }
            //TODO - Allow cheats locally.
            /*
            if (Input.GetKeyDown(KeyCode.D))
            {
                _gameService.CardDrawSystem.DrawCard(_cardGame.ActivePlayer);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                _cardGame.ManaSystem.AddMana(_cardGame.ActivePlayer, "1*");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                _cardGame.TestFillGraveyard();
            }
            */

            _stateMachine?.CurrentState.HandleInput();

        }
        else if (_cardGame.CurrentGameState == GameState.WaitingForChoice)
        {
            if (!(_stateMachine.CurrentState is GameUIChoiceAsPartOfResolveState))
            {
                _stateMachine.ChangeState(new GameUIChoiceAsPartOfResolveState(_stateMachine, _cardGame.ChoiceInfoNeeded));
            }
        }

        UpdateUI();
        _stateMachine.CurrentState.OnUpdate();
    }

    public void StartGame(Decklist player1Deck, Decklist player2Deck)
    {
        //TODO - This should go through the service

        _gameService.SetupGame(player1Deck, player2Deck);
        _gameService.StartGame();

        _gameService.GetGameEventLogsObservable().Subscribe(ev =>
        {
            _gameLog.AppendLog(ev.Log);
        });

        _gameService.GetOnGameStateUpdatedObservable().Subscribe(cardGame =>
        {
            _cardGame = cardGame;
            _player1Board.SetBoard(cardGame.Player1);
            _player2Board.SetBoard(cardGame.Player2);
        });

        UpdateUI();
    }

    public IEnumerable<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();

        entities.AddRange(_player1Board.GetUIEntities());
        entities.AddRange(_player2Board.GetUIEntities());
        entities.AddRange(_zonePopupWindow.GetComponentsInChildren<UIGameEntity>());

        return entities;
    }

    public void ViewChoiceWindow(IEnumerable<ICard> cardsToView, string title)
    {
        _zonePopupWindow.SetActive(true);
        _zonePopupWindow.GetComponent<CardsViewer2D>().SetCards(cardsToView, title);
        _zonePopupWindow.GetComponent<CardsViewer2D>().ShowExitButton = false;
    }

    public void CloseChoiceWindow()
    {
        _zonePopupWindow.SetActive(false);
    }

    public void HandleClick(UIGameControllerClickEvent clickInfo)
    {
        //Do something here?
        _stateMachine.HandleSelection(clickInfo.EntityId);
    }

    public void HandleCastChoice(int castChoiceInfo)
    {
        if (_stateMachine.CurrentState is IGameUIStateHandleCastChoice)
        {
            Debug.Log($"Cast Choice Handled {castChoiceInfo}");
            (_stateMachine.CurrentState as IGameUIStateHandleCastChoice).HandleCastChoiceSelection(castChoiceInfo);
        }
    }

    public void HandlePointerEnter(int entityId)
    {
        var entity = _cardGame.GetEntities<CardInstance>().Where(c => c.EntityId == entityId).FirstOrDefault();
        if (entity == null)
        {
            return;
        }

        _cardPopup.SetCardData(entity);
    }

    public void HandleViewGraveyardClick(Player player)
    {
        _zonePopupWindow.GetComponent<ICardsViewer>().Show(player.DiscardPile.Cards, $"{player.Name}'s Discard");
        _zonePopupWindow.GetComponent<ICardsViewer>().ShowExitButton = true;
    }

    public void HandleNextTurnButtonClick()
    {
        var nextTurnAction = new NextTurnAction();
        _gameService.ProcessAction(nextTurnAction);
        _stateMachine.ToIdle();
    }

    #region Private Methods

    private void UpdateUI()
    {
        //State Machine UI
        _actionStateIndicator.text = _stateMachine.GetMessage();
        if (_turnIndicator != null)
        {
            _turnIndicator.text = $"Player {_cardGame.ActivePlayerId}'s Turn ({_cardGame.TurnSystem.TurnId})";
        }
        //Game Board.
        _player1Board.HideHiddenInfo = _cardGame.ActivePlayer != _cardGame.Player1;
        _player2Board.HideHiddenInfo = _cardGame.ActivePlayer != _cardGame.Player2;
    }

    //TEST METHODS
    private void CheckForCardsWithoutImages()
    {
        var allCards = new CardDatabase().GetAll();
        foreach (var card in allCards)
        {
            Sprite art = Resources.Load<Sprite>(card.ArtPath);

            if (art == null)
            {
                Debug.LogWarning($@"Warning: Could not find card art for {card.Name}");
            }
        }
    }

    private void CheckForCardsWithoutManaCosts()
    {
        var allCards = new CardDatabase().GetAll();
        foreach (var card in allCards)
        {
            if (!(card is ManaCardData) && card.ManaCost == null)
            {
                Debug.LogWarning($@"Warning: Mana cost is null for {card.Name}");
            }
        }
    }

    #endregion
}

