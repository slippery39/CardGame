using UnityEngine;

public interface IHighlightable
{
    void Highlight();
    void Highlight(Color highlightColor);
    void StopHighlight();
}