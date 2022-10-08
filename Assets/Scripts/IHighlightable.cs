using UnityEngine;

internal interface IHighlightable
{
    void Highlight();
    void Highlight(Color highlightColor);
    void StopHighlight();
}