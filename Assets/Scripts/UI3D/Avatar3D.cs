using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avatar3D : MonoBehaviour, IHighlightable
{

    [SerializeField]
    private SpriteRenderer _highlight;

    [SerializeField]
    private TextMeshPro text;

    public void Highlight()
    {
        Highlight(Color.green);
    }

    public void Highlight(Color highlightColor)
    {
        _highlight.gameObject.SetActive(true);
        _highlight.color = highlightColor;
    }

    public void SetLifeTotal(int amount)
    {
        text.text = amount.ToString();
    }

    public void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
