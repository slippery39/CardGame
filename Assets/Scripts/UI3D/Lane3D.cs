using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane3D : MonoBehaviour, IHighlightable
{
    [SerializeField] GameObject _laneSprite;
    [SerializeField] Card3D _card;

    [SerializeField] public AttackButton _attackButton;

    public void Awake()
    {
        _attackButton.HandleClick += HandleAttackButtonClick;
    }

    public void HandleAttackButtonClick()
    {
        UI3DController.Instance.DoAttack(GetComponent<UIGameEntity3D>().EntityId);
    }

    public void Highlight()
    {
        _laneSprite.SetActive(true);
    }

    public void Highlight(Color highlightColor)
    {
        _laneSprite.SetActive(true);
    }

    public void SetUnitInLane(CardInstance card, bool canAttack = false)
    {
        if (card == null)
        {
            _card.gameObject.SetActive(false);
            _attackButton.gameObject.SetActive(false);
        }
        else
        {
            _card.gameObject.SetActive(true);
            _card.SetCardInfo(card);
            _attackButton.gameObject.SetActive(true);

            if (canAttack)
            {
                _attackButton.Highlight();
            }
            else
            {
                _attackButton.gameObject.SetActive(false);
            }
        }
    }

    public void StopHighlight()
    {
        _laneSprite.SetActive(false);
    }


}
