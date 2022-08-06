using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUIPlayerBoard
{
    void SetPlayer(Player player);    
     List<UIGameEntity> GetUIEntities();
    public bool HideHiddenInfo { get; set; }
}

