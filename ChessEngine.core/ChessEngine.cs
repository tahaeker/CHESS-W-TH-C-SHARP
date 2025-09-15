using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class ChessEngine
    {
        public class MoveResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static MoveResult TryMove(string from,string to, ChessContext ctx)
        {
            string error = ErrorChecker.MoveError(from, to, ctx);

            if(!string.IsNullOrEmpty(error))
            {
                return new MoveResult { Success = false, ErrorMessage = error };
            }

            MoveStone(from, to, ctx);

            return new MoveResult { Success = true, ErrorMessage = string.Empty };


        }


        public static void MoveStone(string from, string to, ChessContext ctx)
        {
            Cell fromCell = BoardConverter.StringToCell(from, ctx);
            Cell toCell = BoardConverter.StringToCell(to, ctx);



            if (MoveValidator.IsValidCastlingMove(fromCell, toCell, ctx))
            {
                //castling move
                if (fromCell.CellIndex == (7, 4) && toCell.CellIndex == (7, 2))//white queenside castling
                {
                    ctx.Board[7, 0] = '.';
                    ctx.Board[7, 3] = 'R';
                }
                else if (fromCell.CellIndex == (7, 4) && toCell.CellIndex == (7, 6))//white kingside castling
                {
                    ctx.Board[7, 7] = '.';
                    ctx.Board[7, 5] = 'R';
                }
                else if (fromCell.CellIndex == (0, 4) && toCell.CellIndex == (0, 2))//black queenside castling
                {
                    ctx.Board[0, 0] = '.';
                    ctx.Board[0, 3] = 'r';
                }
                else if (fromCell.CellIndex == (0, 4) && toCell.CellIndex == (0, 6))//black kingside castling
                {
                    ctx.Board[0, 7] = '.';
                    ctx.Board[0, 5] = 'r';
                }
            }




            ctx.Board[toCell.Row, toCell.Col] = ctx.Board[fromCell.Row, fromCell.Col];
            ctx.Board[fromCell.Row, fromCell.Col] = '.';




            // for passant movable
            if (char.ToLower(ctx.lastFromCell.Stone) == 'p' &&
                char.ToLower(ctx.lastToCell.Stone) == 'p')
            {
                int dir = ctx.whiteTurn ? -1 : 1;
                ctx.Board[toCell.Row - dir, toCell.Col] = '.';
            }


            //last movements
            ctx.lastFromCell = BoardConverter.CellToCell(fromCell, ctx);//burayı sor aynı heapta tutuyor o yüzden değişir mi diye düşündüm
            ctx.lastToCell = BoardConverter.CellToCell(toCell, ctx);
            ctx.lastToCell.Stone = toCell.Stone;//yani burası değişiyor mu diye soruyorsun
            ctx.lastFromCell.Stone = fromCell.Stone;//buranın etkisi var mı diye soruyorum????????????


            // did rooks move?
            if (!ctx.whiteQueensideRookMoved && ctx.lastFromCell.Stone == 'R' && ctx.lastFromCell.CellIndex == (7, 0))
            {
                ctx.whiteQueensideRookMoved = true;

            }
            else if (!ctx.whiteKingsideRookMoved && ctx.lastFromCell.Stone == 'R' && ctx.lastFromCell.CellIndex == (7, 7))
            {
                ctx.whiteKingsideRookMoved = true;

            }
            else if (!ctx.blackQueensideRookMoved && ctx.lastFromCell.Stone == 'r' && ctx.lastFromCell.CellIndex == (0, 0))
            {

                ctx.blackQueensideRookMoved = true;

            }
            else if (!ctx.blackKingsideRookMoved && ctx.lastFromCell.Stone == 'r' && ctx.lastFromCell.CellIndex == (0, 7))
            {
                ctx.blackKingsideRookMoved = true;
            }

            // did kings move?
            if (!ctx.blackKingMoved && ctx.lastFromCell.Stone == 'k' && ctx.lastFromCell.CellIndex == (0, 4))
            {

                ctx.blackKingMoved = true;

            }
            if (!ctx.whiteKingMoved && ctx.lastFromCell.Stone == 'K' && ctx.lastFromCell.CellIndex == (7, 4))
            {

                ctx.whiteKingMoved = true;

            }

            //how many the kimg moved?
            if (BoardState.IsThereJustKing(ctx).Item1 && ctx.lastFromCell.Stone == 'K')
            {
                // yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
                ctx.howMuchWhitekingMoved++;
            }
            else if (BoardState.IsThereJustKing(ctx).Item2 && ctx.lastFromCell.Stone == 'k')
            {
                //yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
                ctx.howMuchBlackkingMoved++;
            }


            //recording data to list
            var move = new Move
            {
                From = ctx.lastFromCell.CellString,
                To = ctx.lastToCell.CellString,
                Piece = ctx.lastFromCell.Stone,
                Captured = ctx.lastToCell.IsEmpty ? '.' : ctx.lastFromCell.Stone,
                TurnNumber = (ctx.MoveHistory.Count / 2) + 1,
                IsWhiteTurn = ctx.whiteTurn,
            };
            ctx.MoveHistory.Add(move);

            ctx.SaveBoardToHistory();

         

            





            //Turn change
            ctx.whiteTurn = !ctx.whiteTurn;
        }


    }
}
