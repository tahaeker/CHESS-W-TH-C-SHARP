using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal abstract class Piece
    {
        public bool IsWhite { get; }
        public Piece(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public abstract List<(int, int)> BringPossibleMoves(ChessContext ctx, int row, int col);
        

    }
}
