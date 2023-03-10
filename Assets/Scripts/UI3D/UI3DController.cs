using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.UI;

[RequireComponent(typeof(GameService))]
public class UI3DController : MonoBehaviour, IUIGameController
{
    private List<BaseCardData> cardDB = new CardDatabase().GetAll();
    private GameService _gameService;
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

    [SerializeField]
    private Button _endTurnButton;

    [SerializeField]
    private Stack3D _stack3D;

    [SerializeField]
    private DeveloperPanel _developerPanel;

    public CardGame CardGame => _gameService.CardGame;

    public GameService GameService => _gameService;

    public static UI3DController Instance { get => _instance; set => _instance = value; }
    public GameUIStateMachine GameUIStateMachine { get => _gameUIStateMachine; set => _gameUIStateMachine = value; }

    private static UI3DController _instance;

    private void Awake()
    {
        Instance = this;
        _gameService = GetComponent<GameService>();
        _endTurnButton.onClick.AddListener(() =>
        {
            this.EndTurn();
        }
        );
    }

    public void Start()
    {
        _developerPanel.Initialize(this);
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            _gameEventManager.Reset();
            _gameService.SetupGame(FamousDecks.AllLands(), FamousDecks.AllLands());
            //_gameService.SetupGame(FamousDecks.TokenTest(), FamousDecks.TokenTest());
            /*
            _gameService.GetOnGameStateUpdatedObservable().Subscribe(cardGame =>
            {
                SetUIGameState(cardGame);
            });
            */
            _gameService.StartGame();
            //Make sure this works with our animations as well.
            _gameUIStateMachine.ToIdle();
            //SetUIGameState(_gameService.CardGame);            
        }
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
        var lane = CardGame.ActivePlayer.Lanes.Where(l => l.EntityId == entityId).FirstOrDefault();
        var index = CardGame.ActivePlayer.Lanes.IndexOf(lane);
        GameService.ProcessAction(CreateFightAction(index));
    }

    public void DoAbility(int entityId)
    {

    }


    public List<PlayerBoard3D> GetPlayerBoards()
    {
        return new List<PlayerBoard3D> { _player1Board, _player2Board };
    }



    public void SetUIGameState(CardGame cardGame)
    {
        //Debug.Log("Setting UI Game State");
        _player1Board.SetBoard(cardGame.Player1);
        _player2Board.SetBoard(cardGame.Player2);
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

    public void ViewChoiceWindow(IEnumerable<ICard> cardsToView, string title)
    {
        //TODO
        return;
    }

    public void ShowGameOverScreen()
    {
        //TODO 
        return;
    }

    public void CloseChoiceWindow()
    {
        //TODO
        return;
    }

    public void ShowActionChoicePopup(List<CardGameAction> actions)
    {
        //TODO
        _chooseActionPopup.gameObject.SetActive(true);
        _chooseActionPopup.SetCard(actions);
        return;
    }

    public void CloseActionChoicePopup()
    {
        //TODO
        _chooseActionPopup.gameObject.SetActive(false);
        return;
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


    public void EndTurn()
    {
        //reset the game state in case they are currently in another state
        //ideally they shouldn't be able to end the turn unless they are in the idle state
        //OR (possibly better solution:) the state should update based on game events from the game service
        _gameUIStateMachine.ToIdle();
        _gameService.EndTurn();
    }
}
