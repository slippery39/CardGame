using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHealingSystem
{
    void HealPlayer(Player playerToHeal,int amount);
}
