using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour, IHighlightable
{
    [SerializeField]
    public GameObject _highlight;

    public Action HandleClick;
    private Vector3 _initialLocalScale;

    [SerializeField]
    private float hoverScaleFactor = 1.2f;

    public void Awake()
    {
        _initialLocalScale = transform.localScale;
    }

    public void OnMouseUpAsButton()
    {
        HandleClick?.Invoke();
    }

    public void OnMouseOver()
    {
        transform.localScale = _initialLocalScale * hoverScaleFactor;
    }

    public void OnMouseExit()
    {
        transform.localScale = _initialLocalScale;
    }

    public void Highlight()
    {
        _highlight.SetActive(true);
    }

    public void Highlight(Color highlightColor)
    {
        _highlight.SetActive(true);
    }

    public void StopHighlight()
    {
        _highlight.SetActive(false);
    }
}
