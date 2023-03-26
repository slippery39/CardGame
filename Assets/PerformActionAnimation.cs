using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Animation that plays when an action is performed.. ie. playing a card or potentially activating an ability
 * 
 */

public class PerformActionAnimation : MonoBehaviour
{
    [SerializeField]
    private Card3D actionCard; //an action as it would appear on the stack represented as a card

    //We need to know more about what we should display
    public void PlayAnimation(Card3D otherCard, Action onComplete = null)
    {
        otherCard.gameObject.SetActive(false);
        actionCard.SetCardInfo(otherCard);

        if (actionCard.GetComponent<UIGameEntity3D>() == null)
        {
            var entity3D = actionCard.gameObject.AddComponent<UIGameEntity3D>();
            entity3D.EntityId = otherCard.GetComponent<UIGameEntity3D>().EntityId;
        }
        else
        {
            actionCard.GetComponent<UIGameEntity3D>().EntityId = otherCard.GetComponent<UIGameEntity3D>().EntityId;
        }

        actionCard.gameObject.SetActive(true);

        if (onComplete == null)
        {
            actionCard.gameObject.SetActive(false);
            return;
        }
        StartCoroutine(Wait(() =>
        {
            actionCard.gameObject.SetActive(false);
            onComplete();
        }));
    }

    //Candidate for a static toolbox method
    public IEnumerator Wait(Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);
        onComplete();
    }
}
