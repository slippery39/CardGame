using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Common methods that are needed for state machines...
//Not sure if the interface name is correct
public interface IUIGameController
{
    IEnumerable<UIGameEntity> GetUIEntities();
    void ViewChoiceWindow(IEnumerable<ICard> cardsToView, string title);
    void ShowGameOverScreen();
    void CloseChoiceWindow();
    void ShowActionChoicePopup(List<CardGameAction> actions);
    void CloseActionChoicePopup();
    public CardGame CardGame { get; }
    public GameService GameService { get; }
}