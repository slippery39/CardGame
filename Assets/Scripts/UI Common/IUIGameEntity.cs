using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IUIGameEntity
{
    public int EntityId { get; set; }
    //Note that these methods are shared with the IHighlightable interface, but 
    //we don't want to have the IUIGameEntity be an IHighlightable by default
    void Highlight();
    void Highlight(Color highlightColor);
    void StopHighlight();
}

