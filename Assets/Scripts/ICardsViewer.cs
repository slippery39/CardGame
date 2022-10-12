using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICardsViewer
{
    void SetCards(IEnumerable<ICard> cards, string name, bool setReverse = false, bool hiddenInfo = false);
    void Show(IEnumerable<ICard> cards, string name, bool setReverse = false, bool hiddenInfo = false);
    bool ShowExitButton { get; set; }
}

