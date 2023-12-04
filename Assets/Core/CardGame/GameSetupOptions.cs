using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core
{
    public class GameSetupOptions
    {
        Decklist player1Deck;
        Decklist player2Deck;
        int startingLifeTotal;
        public Decklist Player1Deck { get => player1Deck; set => player1Deck = value; }
        public Decklist Player2Deck { get => player2Deck; set => player2Deck = value; }
        public int StartingLifeTotal { get => startingLifeTotal; set => startingLifeTotal = value; }
    }

}
