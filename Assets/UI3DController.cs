using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

[RequireComponent(typeof(GameService))]
public class UI3DController : MonoBehaviour
{
    private List<BaseCardData> cardDB = new CardDatabase().GetAll();
    private GameService _gameService;

    [SerializeField]
    private PlayerBoard3D _player1Board;
    [SerializeField]
    private PlayerBoard3D _player2Board;

    private void Awake()
    {
        _gameService = GetComponent<GameService>();
    }
    public void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _gameService.SetupGame(FamousDecks.RandomPremadeDeck(),FamousDecks.RandomPremadeDeck());
            _gameService.GetOnGameStateUpdatedObservable().Subscribe(cardGame =>
            {
                SetUIGameState(cardGame);
            });
            _gameService.StartGame();
            //SetUIGameState(_gameService.CardGame);            
        }


    }

    public void SetUIGameState(CardGame cardGame)
    {
        Debug.Log("Setting UI Game State");
        _player1Board.SetBoard(cardGame.Player1);
        _player2Board.SetBoard(cardGame.Player2);
    }

    public void HandleEvent(object evt)
    {
        //Run Appropriate Animations

        //Set the state of the game UI


        //SetGameState(evt.ResultingGameState);     
    }


    public void SetGameState(CardGame gameState)
    {
        //for each entity in the game State, go through and update the entity in the UI that corresponds to it.
        //cards that are not revealed (i.e. cards in your deck that are face down) or cards in the opponent hand should not correspond to a specific entity yet
        //we will use an entity id of -1 for this case.

        /*
         * 
         * foreach player ->
         * 
         * foreach(var card in hand){
         *  update the cards in the players hand
         * }
         * 
         * foreach(var card in graveyard){
         *  update the card in the players graveyard
         * }
         * 
         * foreach(var card in lane){
         *  update the cards in play
         * }
         * 
         * foreach (var card in items){
         *  update the cards in items
         * }
         * 
         * also deal with revealed cards
         * 
         */
    }
}
