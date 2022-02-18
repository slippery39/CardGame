using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject _uiCardTemplate;
    [SerializeField]
    private CardGame _gameState;
    [SerializeField]
    private Transform _player1LanePosition;
    [SerializeField]
    private Transform _player2LanePosition;

    void Start()
    {
        _gameState = new CardGame();
        //Spawn our cards in some "lanes" for the UI

        FillLaneWithCards(_player1LanePosition, _gameState.Player1Lane);
        FillLaneWithCards(_player2LanePosition, _gameState.Player2Lane);
    }

    #region Private Methods
    private GameObject CreateCardWithData(BaseCardData data)
    {
        var newCard = Instantiate(_uiCardTemplate);
        newCard.GetComponent<UICard>().SetFromCardData(data);
        return newCard;
    }

    private void FillLaneWithCards(Transform laneInScene, List<CardInstance> cardsInLane)
    {
        int cardIndex = 0;
        foreach (var cardInstance in cardsInLane)
        {
            var card = CreateCardWithData(cardInstance.CurrentCardData);
            card.transform.SetParent(laneInScene);
            var cardXSize = card.GetComponent<UICard>().CardArtRenderer.bounds.size.x;
            var cardPosition = (cardIndex * (cardXSize + cardXSize/2));
            card.transform.localPosition = new Vector3(cardPosition, 0, 0);
            cardIndex++;
        }
    }
    #endregion
}

