using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System;

[RequireComponent(typeof(GameService))]
public class UI3DController : MonoBehaviour, IUIGameController
{
    [SerializeField] private int playerId = 1;

    private List<BaseCardData> cardDB = new CardDatabase().GetAll();
    private GameService _gameService;

    [SerializeField]
    private AppController _appController;

    [SerializeField]
    private UI3DGameEventManager _gameEventManager;

    [SerializeField]
    private GameUIStateMachine _gameUIStateMachine;

    [SerializeField]
    private PlayerBoard3D _player1Board;
    [SerializeField]
    private PlayerBoard3D _player2Board;

    [SerializeField]
    private UIChooseActionPopup3D _chooseActionPopup;

    [Header("UI Buttons")]
    [SerializeField]
    private Button _endTurnButton;

    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    private Button _backToMainMenuButton;

    [Header("Other")]
    [SerializeField]
    private Stack3D _stack3D;

    [SerializeField]
    private DeveloperPanel _developerPanel;

    [SerializeField]
    private CardViewerModal3D _cardViewerModal;

    [SerializeField]
    private GameObject _gameViewContainer;

    [SerializeField]
    private UIStateDescriptionLabel _stateDescriptionLabel;

    public CardGame CardGame => _gameService.CardGame;

    public GameService GameService => _gameService;

    public static UI3DController Instance { get; private set; }
    public GameUIStateMachine GameUIStateMachine { get => _gameUIStateMachine; set => _gameUIStateMachine = value; }

    private void Awake()
    {
        Instance = this;
        _gameService = GetComponent<GameService>();
        _endTurnButton.onClick.AddListener(() =>
        {
            this.EndTurn();         
        });

        _cancelButton.onClick.AddListener(() =>
        {
            var cancellableState = this._gameUIStateMachine.CurrentState as IGameUICancellable;

            if (cancellableState != null)
            {
                cancellableState.HandleCancel();
            }
        });

        _backToMainMenuButton.onClick.AddListener(() =>
        {
            _appController.GoToMainMenu();
        });

        _chooseActionPopup.Initialize(this);
        _cardViewerModal.Initialize(this);
        _player1Board.Initialize(this);
        _player2Board.Initialize(this);
        _developerPanel.Initialize(this);
    }

    public void Start()
    {
        if (CardGame != null)
        {
            this.SetUIGameState(CardGame);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartGame(new GameSetupOptions()
            {
                Player1Deck = Decklist.CreateRandomDecklist(),
                Player2Deck = Decklist.CreateRandomDecklist(),
                StartingLifeTotal = 20
            });
        }
        //Hacked in just like our old one, but is there not a better way to do this?
        //The card game should be triggering this state
        if (CardGame != null && CardGame.CurrentGameState == GameState.WaitingForAction)
        {
            if (GameUIStateMachine?.CurrentState is GameUIChoiceAsPartOfResolveState)
            {
                GameUIStateMachine.ToIdle();
            }
        }

        if (CardGame != null && CardGame.CurrentGameState == GameState.WaitingForChoice)
        {
            if (GameUIStateMachine.CurrentState is not GameUIChoiceAsPartOfResolveState)
            {
                GameUIStateMachine.ChangeState(new GameUIChoiceAsPartOfResolveState(GameUIStateMachine, CardGame.ChoiceInfoNeeded));
            }
        }
    }

    public void Initialize(AppController appController)
    {
        _appController = appController;
    }
    public void StartGame(GameSetupOptions options)
    {
        HideUIButtons(); //all ui buttons should be determined by the ui game state
        _gameViewContainer.SetActive(true);
        _gameEventManager.Reset();
        _gameService.SetupGame(options);
        _gameService.StartGame();
        _gameUIStateMachine.ToIdle();
    }
    public void EndGame()
    {
        _gameViewContainer.SetActive(false);
        _gameEventManager.Reset();
    }

    private FightAction CreateFightAction(int laneIndex)
    {
        return new FightAction
        {
            Player = CardGame.ActivePlayer,
            LaneIndex = laneIndex
        };
    }
    public void DoAttack(int entityId)
    {
        var lane = CardGame.ActivePlayer.Lanes.Find(l => l.EntityId == entityId);
        var index = CardGame.ActivePlayer.Lanes.IndexOf(lane);
        GameService.ProcessAction(CreateFightAction(index));
    }
    public void DoAbility(int entityId)
    {
        throw new NotImplementedException();
    }

    public List<PlayerBoard3D> GetPlayerBoards()
    {
        return new List<PlayerBoard3D> { _player1Board, _player2Board };
    }

    public void SetUIGameState(CardGame cardGame)
    {
        //If you want to be able to see everyone's cards then change this playerId to cardGame.ActivePlayerId
        var gameCopy = GameService.GetGameViewForPlayer(cardGame, playerId);
        _stack3D.SetCards(gameCopy.ResolvingSystem.Stack.Cards);
        _player1Board.SetBoard(gameCopy.Player1);
        _player2Board.SetBoard(gameCopy.Player2);
    }

    public void HandleSelection(int entityID)
    {
        _gameUIStateMachine.CurrentState.HandleSelection(entityID);
    }

    public IEnumerable<IUIGameEntity> GetUIEntities()
    {
        //TODO - better way of doing this
        return FindObjectsOfType<UIGameEntity3D>().Cast<IUIGameEntity>();
    }

    public void ViewChoiceWindow(IEnumerable<ICard> cardsToView, string title, bool showCancel = true)
    {
        _cardViewerModal.Show(cardsToView.ToList(), title, showCancel);
    }

    public void CloseChoiceWindow()
    {
        _cardViewerModal.Hide();
    }

    public void ShowGameOverScreen()
    {
        HideUIButtons();
        _backToMainMenuButton.gameObject.SetActive(true);
    }

    public void ShowActionChoicePopup(List<CardGameAction> actions)
    {
        //TODO
        _chooseActionPopup.gameObject.SetActive(true);
        _chooseActionPopup.SetCard(actions);
    }

    public void CloseActionChoicePopup()
    {
        //TODO
        _chooseActionPopup.gameObject.SetActive(false);
    }

    public void HandleCastChoice(int castChoiceInfo)
    {
        //TODO - handle this in the state itself instead of doing this..
        if (_gameUIStateMachine.CurrentState is IGameUIStateHandleCastChoice)
        {
            Debug.Log($"Cast Choice Handled {castChoiceInfo}");
            (_gameUIStateMachine.CurrentState as IGameUIStateHandleCastChoice).HandleCastChoiceSelection(castChoiceInfo);
        }
    }

    /*
     * To simplify the state required for these buttons, we will have a rule that if the cancel button is showing, then the end turn button
     * cannot be showing. This way if a game state shows one or the other, they don't need to worry about also hiding the other button as well.
     * Hopefully this will reduce any potential bugs where a button is showing up when it shouldn't
     */
    public void ShowEndTurnButton()
    {
        _endTurnButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(false);
    }

    public void ShowCancelButton()
    {
        _cancelButton.gameObject.SetActive(true);
        _endTurnButton.gameObject.SetActive(false);
    }
    public void HideUIButtons()
    {
        _backToMainMenuButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(false);
        _cancelButton.gameObject.SetActive(false);
    }
    public void SetStateLabel(string text)
    {
        if (text == "" || text == null)
        {
            _stateDescriptionLabel.Hide();
            _stateDescriptionLabel.SetLabel("");
        }
        else
        {
            _stateDescriptionLabel.Show();
            _stateDescriptionLabel.SetLabel(text);
        }
    }

    public void EndTurn()
    {
        //reset the game state in case they are currently in another state
        //ideally they shouldn't be able to end the turn unless they are in the idle state
        //OR (possibly better solution:) the state should update based on game events from the game service
        _gameUIStateMachine.ToIdle();
        _gameService.EndTurn();
    }
}
