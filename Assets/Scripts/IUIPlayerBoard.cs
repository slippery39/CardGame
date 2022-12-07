using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUIPlayerBoard
{
    List<UIGameEntity> GetUIEntities();
    public bool HideHiddenInfo { get; set; }
}

