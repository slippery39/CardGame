using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stack3D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStack(List<ResolvingActionInfo> _stack)
    {
        Debug.Log("Stack Count :" + _stack.Count);
        this.GetComponent<TextMeshProUGUI>().text = "Stack Count:" + _stack.Count;   
    }

    public void PlayAnimation(string text,Action onComplete = null)
    {
        this.GetComponent<TextMeshProUGUI>().text = text;

        if (onComplete == null)
        {
            return;
        }
        StartCoroutine(Wait(onComplete));        
    }

    public IEnumerator Wait(Action onComplete)
    {
        yield return new WaitForSeconds(1);        
        onComplete();
    }
}
