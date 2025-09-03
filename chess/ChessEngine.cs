using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class ChessEngine
    {
        public static void MoveStone(string from, string to, ChessContext ctx)
        {
            Cell fromCell = BoardHelper.StringToCell(from, ctx);
            Cell toCell = BoardHelper.StringToCell(to, ctx);



            if (MoveValidator.IsValidCastlingMove(fromCell, toCell, ctx))
            {
                //castling move
                if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 2))//white queenside castling
                {
                    ctx.board[7, 0] = '.';
                    ctx.board[7, 3] = 'R';
                }
                else if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 6))//white kingside castling
                {
                    ctx.board[7, 7] = '.';
                    ctx.board[7, 5] = 'R';
                }
                else if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 2))//black queenside castling
                {
                    ctx.board[0, 0] = '.';
                    ctx.board[0, 3] = 'r';
                }
                else if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 6))//black kingside castling
                {
                    ctx.board[0, 7] = '.';
                    ctx.board[0, 5] = 'r';
                }
            }




            ctx.board[toCell.Row, toCell.Col] = ctx.board[fromCell.Row, fromCell.Col];
            ctx.board[fromCell.Row, fromCell.Col] = '.';




            // for passant movable
            if (char.ToLower(ctx.lastFromCell.stone) == 'p' &&
                char.ToLower(ctx.lastToCell.stone) == 'p')
            {
                int dir = ctx.whiteTurn ? -1 : 1;
                ctx.board[toCell.Row - dir, toCell.Col] = '.';
            }


            //last movements
            ctx.lastFromCell = BoardHelper.CellToCell(fromCell, ctx);//burayı sor aynı heapta tutuyor o yüzden değişir mi diye düşündüm
            ctx.lastToCell = BoardHelper.CellToCell(toCell, ctx);
            ctx.lastToCell.stone = toCell.stone;//yani burası değişiyor mu diye soruyorsun
            ctx.lastFromCell.stone = fromCell.stone;//buranın etkisi var mı diye soruyorum????????????


            // did rooks move?
            if (!ctx.whiteQueensideRookMoved && ctx.lastFromCell.stone == 'R' && ctx.lastFromCell.cellIndex == (7, 0))
            {
                ctx.whiteQueensideRookMoved = true;

            }
            else if (!ctx.whiteKingsideRookMoved && ctx.lastFromCell.stone == 'R' && ctx.lastFromCell.cellIndex == (7, 7))
            {
                ctx.whiteKingsideRookMoved = true;

            }
            else if (!ctx.blackQueensideRookMoved && ctx.lastFromCell.stone == 'r' && ctx.lastFromCell.cellIndex == (0, 0))
            {

                ctx.blackQueensideRookMoved = true;

            }
            else if (!ctx.blackKingsideRookMoved && ctx.lastFromCell.stone == 'r' && ctx.lastFromCell.cellIndex == (0, 7))
            {
                ctx.blackKingsideRookMoved = true;
            }

            // did kings move?
            if (!ctx.blackKingMoved && ctx.lastFromCell.stone == 'k' && ctx.lastFromCell.cellIndex == (0, 4))
            {

                ctx.blackKingMoved = true;

            }
            if (!ctx.whiteKingMoved && ctx.lastFromCell.stone == 'K' && ctx.lastFromCell.cellIndex == (7, 4))
            {

                ctx.whiteKingMoved = true;

            }

            //how many the kimg moved?
            if (BoardHelper.IsThereJustKing(ctx).Item1 && ctx.lastFromCell.stone == 'K')
            {
                // yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
                ctx.howMuchWhitekingMoved++;
            }
            else if (BoardHelper.IsThereJustKing(ctx).Item2 && ctx.lastFromCell.stone == 'k')
            {
                //yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
                ctx.howMuchBlackkingMoved++;
            }


            //recording data to list
            var move = new Move
            {
                From = ctx.lastFromCell.cellString,
                To = ctx.lastToCell.cellString,
                Piece = ctx.lastFromCell.stone,
                Captured = ctx.lastToCell.isEmpty ? '.' : ctx.lastFromCell.stone,
                TurnNumber = (ctx.MoveHistory.Count / 2) + 1,
                IsWhiteTurn = ctx.whiteTurn,
            };
            ctx.MoveHistory.Add(move);



            //Turn change
            ctx.whiteTurn = !ctx.whiteTurn;
        }



        public static void UndoMove(ChessContext ctx)
        {
            if (ctx.MoveHistory.Count == 0)
            {
                Console.WriteLine("There is not a move can be undo!");
                return;
            }


            var lastMove = ctx.MoveHistory.Last();


            var (fromRow, fromCol) = BoardHelper.StringToIndex(lastMove.From);
            var (toRow, toCol) = BoardHelper.StringToIndex(lastMove.To);


            //return 
            ctx.board[fromRow, fromCol] = ctx.board[toRow, toCol];
            ctx.board[toRow, toCol] = lastMove.Captured == '.' ? ctx.empty : lastMove.Captured;



            // deleting the move from the past
            ctx.MoveHistory.RemoveAt(ctx.MoveHistory.Count - 1);

            var BeforeLast = ctx.MoveHistory.Last();

            var (beforeFromRow, beforeFromCol) = BoardHelper.StringToIndex(BeforeLast.From);
            var (beforeToRow, beforeToCol) = BoardHelper.StringToIndex(BeforeLast.To);

            // updating
            ctx.lastFromCell = new Cell(beforeFromRow, beforeFromCol, ctx);
            ctx.lastToCell = new Cell(beforeToRow, beforeToCol, ctx);


            // Change order back
            ctx.whiteTurn = !ctx.whiteTurn;

            Console.WriteLine($"Hamle geri alındı: {lastMove}");

        }
    }
}
