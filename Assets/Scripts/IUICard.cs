using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUICard
{
    void SetCardData(CardInstance cardInstance);
    void SetCardData(BaseCardData cardData);
    void SetAsHiddenCard();
}

