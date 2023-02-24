using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameEntity3D : MonoBehaviour, IUIGameEntity
{
    [SerializeField]
    int _entityId;
    public int EntityId { get => _entityId; set => _entityId = value; }

    public static UIGameEntity3D AddToCard3D(Card3D card3D, CardInstance cardInstance)
    {
        var entity = card3D.gameObject.GetComponent<UIGameEntity3D>();
        if (entity == null)
        {
            entity = card3D.gameObject.AddComponent<UIGameEntity3D>();
        }
        entity.EntityId = cardInstance.EntityId;
        return entity;
    }
    public static UIGameEntity3D AddToLane(Lane3D lane3D, Lane lane)
    {
        var entity = lane3D.gameObject.GetComponent<UIGameEntity3D>();
        if (entity == null)
        {
            entity = lane3D.gameObject.AddComponent<UIGameEntity3D>();
        }
        entity.EntityId = lane.EntityId;
        return entity;
    }

    public virtual void Highlight()
    {
        var highlight = this.GetComponent<IHighlightable>();
        if (highlight != null)
        {
            highlight.Highlight();
        }
    }

    public virtual void Highlight(Color highlightColor)
    {
        var highlight = this.GetComponent<IHighlightable>();
        if (highlight != null)
        {
            highlight.Highlight(highlightColor);
        }
    }

    public virtual void StopHighlight()
    {
        var highlight = this.GetComponent<IHighlightable>();
        if (highlight != null)
        {
            highlight.StopHighlight();
        }
    }
}
