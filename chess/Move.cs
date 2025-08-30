using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class Move
    {
        public string From { get; set; }   // e2
        public string To { get; set; }     // e4
        public char Piece { get; set; }    // oynayan taş (örn. 'P')
        public char Captured { get; set; } // varsa alınan taş, yoksa '.'

        public int TurnNumber { get; set; } // hangi turda yapıldı
        public bool IsWhiteTurn { get; set; }

        public override string ToString()// içerisinde return olmalo
        {
            string capturePart = Captured != '.' ? $"x{Captured}" : "";
            string text = $" {From}-{To} ({Piece}{capturePart})";
            return IsWhiteTurn ? $"{TurnNumber}. {text}" : $" - {text}";

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




