using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UIPlayerBoard : MonoBehaviour, IUIPlayerBoard
{
    [SerializeField]  
    private bool _hideHiddenInfo;
    public bool HideHiddenInfo { get; set; }
    public abstract void SetPlayer(Player player);
    public abstract List<UIGameEntity> GetUIEntities();
}

