using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Common methods that are needed for state machines...
//Not sure if the interface name is correct
public interface IUIGameController
{
    IEnumerable<IUIGameEntity> GetUIEntities();
    void ViewChoiceWindow(IEnumerable<ICard> cardsToView, string title, bool showCancel = false);

    void SetStateLabel(string text);
    void ShowGameOverScreen();
    void CloseChoiceWindow();
    void ShowActionChoicePopup(List<CardGameAction> actions);
    void CloseActionChoicePopup();

    void ShowEndTurnButton();
    void ShowCancelButton();
    void HideUIButtons();

    public CardGame CardGame { get; }
    public CardGame CurrentUICardGame { get; }
    public GameService GameService { get; }
}