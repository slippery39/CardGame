using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This MonoBehaviour should be inherited from any UI entity that corresponds to 
/// an entity in the actual game that can be interacted with.
/// 
/// Will be used by our targetting system to highlight which things can be targeted by certain spells.
/// 
/// Examples: Cards, Players, Lanes, etc...
/// </summary>
public class UIGameEntity : MonoBehaviour
{
    [SerializeField]
    int _entityId;
    public int EntityId { get => _entityId; set => _entityId = value; }

    //Override these as necessary
    public virtual void Highlight()
    {
        Debug.Log($@"{EntityId} should be getting highlighted!");
    }

    public virtual void StopHighlight()
    {
        Debug.Log($@"{EntityId} should stop being highlighted!");
    }

}
