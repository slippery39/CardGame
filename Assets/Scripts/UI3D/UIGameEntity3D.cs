using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameEntity3D : MonoBehaviour
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
}
