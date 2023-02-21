using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stack3D : MonoBehaviour
{
    [SerializeField]
    private Card3D actionCard; //an action as it would appear on the stack represented as a card

    public void SetStack(List<ResolvingActionInfo> _stack)
    {
        Debug.Log("Stack Count :" + _stack.Count);
        this.GetComponent<TextMeshProUGUI>().text = "Stack Count:" + _stack.Count;
    }


    //We need to know more about what we should display

    public void PlayAnimation(Card3D otherCard, Action onComplete = null)
    {
        actionCard.SetCardInfo(otherCard);
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
