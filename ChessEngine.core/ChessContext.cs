using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class ChessContext
    {

        public char[,] Board { get; set; }
        public string inputFrom = "";
        public string inputTo = "";
        public char empty = '.';
        public bool whiteTurn = true;


        public Cell touchedCell = new Cell(0, 0, null);
        public Cell lastFromCell = new Cell(0, 0, null);
        public Cell lastToCell = new Cell(0, 0, null);


        public bool whiteKingMoved = false;
        public bool blackKingMoved = false;
        public bool whiteQueensideRookMoved = false;
        public bool whiteKingsideRookMoved = false;
        public bool blackQueensideRookMoved = false;
        public bool blackKingsideRookMoved = false;


        public bool IsFakeMovement = false; // for testing sub purposes without stack overflow exception

        public bool whiteWins = false;
        public bool blackWins = false;
        public bool drawStuation = false;
        public bool isGameEnd = false;



        public int howMuchWhitekingMoved = 0;
        public int howMuchBlackkingMoved = 0;



        public List<Move> MoveHistory { get; set; } = new List<Move>();


        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }



        public ChessContext()
        {
            Board = new char[8, 8]; // 8x8 satranç tahtası
        }


    }
}
