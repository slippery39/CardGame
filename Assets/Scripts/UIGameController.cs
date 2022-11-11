using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
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

    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }

    //Singleton Pattern, should only be one game controller per unity scene.
    public static UIGameController Instance;


    [Header("Debug Information")]
    [SerializeField]
    public List<int> RegisteredEntities;

    private void Awake()
    {
        Instance = this;
        _cardGame = new CardGame();
    }

    public void SerializeTest()
    {
        string json = JsonConvert.SerializeObject(_cardGame, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });

        File.WriteAllText(@"./tempGameState", json);

        //Testing deserializing our cards
        _cardGame = (CardGame)JsonConvert.DeserializeObject(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });

        _player1Board.SetPlayer(_cardGame.Player1);
        _player2Board.SetPlayer(_cardGame.Player2);
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
        _stateMachine = GetComponent<GameUIStateMachine>();

    }

    private void InitEventHandlers()
    {
        _nextTurnButton.onClick.AddListener(()=>HandleNextTurnButtonClick());
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Serializing...");
            SerializeTest();
            Debug.Log("Deserializing");
            RegisteredEntities = _cardGame.RegisteredEntities.Select(e=>e.EntityId).ToList();
        }

        if (_cardGame.CurrentGameState == GameState.WaitingForAction)
        {
            if (_stateMachine?.CurrentState is GameUIChoiceAsPartOfResolveState)
            {
                _stateMachine.ToIdle();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _cardGame.ProcessAction(CreateFightAction(0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _cardGame.ProcessAction(CreateFightAction(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _cardGame.ProcessAction(CreateFightAction(2));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _cardGame.ProcessAction(CreateFightAction(3));
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _cardGame.ProcessAction(CreateFightAction(4));
            }
            //Testing card drawing
            if (Input.GetKeyDown(KeyCode.D))
            {
                _cardGame.CardDrawSystem.DrawCard(_cardGame.ActivePlayer);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                _cardGame.ManaSystem.AddMana(_cardGame.ActivePlayer, "1*");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                _cardGame.TestFillGraveyard();
            }

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
        _cardGame.SetupDecks(player1Deck, player2Deck);
        _cardGame.StartGame();
        //TODO - These are not being updated when we serialize / deserialize an object
        _player1Board.SetPlayer(_cardGame.Player1);
        _player2Board.SetPlayer(_cardGame.Player2);
        RegisteredEntities = _cardGame.RegisteredEntities.Select(e => e.EntityId).ToList();
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
        Debug.Log("processing next turn");
        var nextTurnAction = new NextTurnAction();
        _cardGame.ProcessAction(nextTurnAction);
        Debug.Log("done processing next turn");
        _stateMachine.ToIdle();
    }

    #region Private Methods

    private void UpdateUI()
    {
        _actionStateIndicator.text = _stateMachine.GetMessage();
        if (_turnIndicator != null)
        {
            _turnIndicator.text = $"Player {_cardGame.ActivePlayerId}'s Turn ({_cardGame.TurnSystem.TurnId})";
        }

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

