using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHealingSystem
{
    void HealPlayer(CardGame cardGame, Player playerToHeal,int amount);
}
