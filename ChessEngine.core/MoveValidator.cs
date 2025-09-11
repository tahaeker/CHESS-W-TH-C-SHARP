using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class MoveValidator
    {
        public static bool IsValidPawnMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {
            if (char.ToLower(fromCell.Stone) != 'p')// parametreye dikkat et ilk başta hepsi string tanımlı
            {
                return false;
            }
            //direction -1 for white ,1for black
            int dir = ctx.whiteTurn ? -1 : 1;
            if (toCell.Row == 4 && toCell.Col == 2)
            {

            }


            if (fromCell.Col == toCell.Col &&
                toCell.Row - fromCell.Row == dir &&
                toCell.IsEmpty)
            {
                return true;
            } //start point of the pawm
            else if (
                fromCell.Col == toCell.Col && toCell.Row - fromCell.Row == 2 * dir &&
                (fromCell.Row == 1 || fromCell.Row == 6) &&
                toCell.IsEmpty &&
                ctx.Board[fromCell.Row + dir, fromCell.Col] == '.')
            {
                return true;
            }


            //çapraz yemek için
            if (
                (!toCell.IsEmpty && (toCell.Col - fromCell.Col == dir) && (toCell.Row - fromCell.Row == dir)) ||
                (!toCell.IsEmpty && (toCell.Col - fromCell.Col == -dir) && (toCell.Row - fromCell.Row == dir))
                )
            {
                return true;
            }
            // passant move
            if (char.ToLower(ctx.lastFromCell.Stone) == 'p' && ctx.lastToCell.Col == toCell.Col &&
                (
                (ctx.lastFromCell.Row == 6 && fromCell.Row == 4 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == -dir)) ||
                (ctx.lastFromCell.Row == 6 && fromCell.Row == 4 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == dir)) ||
                (ctx.lastFromCell.Row == 1 && fromCell.Row == 3 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == dir)) ||
                (ctx.lastFromCell.Row == 1 && fromCell.Row == 3 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == -dir))
                )
                )
            {
                return true;
            }

            return false;
        }

        public static bool IsValidRookMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {
            if (char.ToLower(fromCell.Stone) != 'r' &&
                char.ToLower(fromCell.Stone) != 'q')// parametreye dikkat et ilk başta hepsi string tanımlı
            {
                return false;
            }

            //yatay sağa doğru
            if (fromCell.Row == toCell.Row &&
                toCell.Col != fromCell.Col
                )
            {
                int yDirectionStep = (toCell.Col > fromCell.Col) ? 1 : -1;

                for (int j = (fromCell.Col + yDirectionStep); j != toCell.Col; j += yDirectionStep)
                {
                    if (ctx.Board[fromCell.Row, j] != '.')

                        return false;
                }


            }
            else if (fromCell.Row != toCell.Row &&
            fromCell.Col == toCell.Col)
            {
                int xDirectionStep = (toCell.Row > fromCell.Row) ? 1 : -1;//dikey

                for (int i = fromCell.Row + xDirectionStep; i != toCell.Row; i += xDirectionStep)
                {
                    if (ctx.Board[i, fromCell.Col] != '.') return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool IsValidKnightMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {

            if (toCell.Row == fromCell.Row - 2 &&
                ((toCell.Col == fromCell.Col - 1) || (toCell.Col == fromCell.Col + 1))
                )
            {
                return true;
            }
            else if (toCell.Col == fromCell.Col - 2 &&
                ((toCell.Row == fromCell.Row - 1) || toCell.Row == fromCell.Row + 1)
                )
            {
                return true;
            }
            else if (toCell.Row == fromCell.Row + 2 &&
                ((toCell.Col == fromCell.Col + 1) || (toCell.Col == fromCell.Col - 1))
                )
            {
                return true;
            }
            else if (toCell.Col == fromCell.Col + 2 &&
                ((toCell.Row == fromCell.Row - 1) || toCell.Row == fromCell.Row + 1)
                )
            {
                return true;
            }


            return false;
        }

        public static bool IsValidBishopMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {
            if (char.ToLower(fromCell.Stone) != 'b' &&
                char.ToLower(fromCell.Stone) != 'q')// parametreye dikkat et ilk başta hepsi string tanımlı
            {
                return false;
            }

            int deltaI = toCell.Row - fromCell.Row;
            int deltaJ = toCell.Col - fromCell.Col;
            //çapraz olmalı
            if (Math.Abs(deltaI) != Math.Abs(deltaJ))
            {
                return false;
            }


            // direction yönü neresi
            int dirI = deltaI > 0 ? 1 : -1;
            int dirJ = deltaJ > 0 ? 1 : -1;

            int steps = Math.Abs(deltaI);// kaç adım ilelrlesin
            for (int k = 1; k < steps; k++)// ara karelere bakıp taş varsa false döner
            {
                int checkI = fromCell.Row + dirI * k;
                int checkJ = fromCell.Col + dirJ * k;
                if (ctx.Board[checkI, checkJ] != '.')
                {
                    return false;
                }

            }

            return true;
        }

        public static bool IsValidQueenMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {
            if (IsValidBishopMove(fromCell, toCell, ctx) ||
                MoveValidator.IsValidRookMove(fromCell, toCell, ctx))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidKingMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {

            // direction 
            int deltaI = toCell.Row - fromCell.Row;
            int deltaJ = toCell.Col - fromCell.Col;
            int dirI = deltaI > 0 ? 1 : -1;
            int dirJ = deltaJ > 0 ? 1 : -1;

            //for normal movement 
            if ((Math.Abs(fromCell.Row - toCell.Row) == 1 && Math.Abs(fromCell.Col - toCell.Col) == 1) ||
            (Math.Abs(fromCell.Row - toCell.Row) == 1 && Math.Abs(fromCell.Col - toCell.Col) == 0) ||
            (Math.Abs(fromCell.Row - toCell.Row) == 0 && Math.Abs(fromCell.Col - toCell.Col) == 1))
            {
                return true;
            }

            // does king check in routa


            if (!ctx.IsFakeMovement)
            {
                ChessContext tempCtx = BoardState.copyBoard(ctx);
                tempCtx.IsFakeMovement = true;

                if (MoveValidator.IsValidCastlingMove(fromCell, toCell, tempCtx))
                {
                    return true;
                }
            }



            return false;

        }

        public static bool IsValidCastlingMove(Cell fromCell, Cell toCell, ChessContext ctx)
        {

            ChessContext tempCtx;
            tempCtx = BoardState.copyBoard(ctx);
            tempCtx.IsFakeMovement = true;

            if (ctx.whiteTurn && BoardState.IsWhitekingUnderThreat(ctx))
            {
                return false;
            }

            if (!ctx.whiteTurn && BoardState.IsBlackkingUnderThreat(ctx))
            {
                return false;
            }

            if (fromCell.CellIndex == (7, 4) && toCell.CellIndex == (7, 2) &&
            ctx.Board[7, 2] == ctx.empty && ctx.Board[7, 3] == ctx.empty &&
            !ctx.whiteQueensideRookMoved && ctx.Board[7, 0] != ctx.empty && !ctx.whiteKingMoved)
            {
                return true;
            }
            else if (fromCell.CellIndex == (7, 4) && toCell.CellIndex == (7, 6) &&
                ctx.Board[7, 5] == ctx.empty && ctx.Board[7, 6] == ctx.empty &&
                !ctx.whiteKingsideRookMoved && ctx.Board[7, 7] != ctx.empty && !ctx.whiteKingMoved)
            {
                return true;
            }



            //black king castling
            if (fromCell.CellIndex == (0, 4) && toCell.CellIndex == (0, 2) &&
            ctx.Board[0, 2] == ctx.empty && ctx.Board[0, 3] == ctx.empty &&
            !ctx.blackQueensideRookMoved && ctx.Board[0, 0] != ctx.empty && !ctx.blackKingMoved)
            {
                return true;
            }
            else if (fromCell.CellIndex == (0, 4) && toCell.CellIndex == (0, 6) &&
                ctx.Board[0, 5] == ctx.empty && ctx.Board[0, 6] == ctx.empty &&
                !ctx.blackKingsideRookMoved && ctx.Board[0, 7] != ctx.empty && !ctx.blackKingMoved)
            {
                return true;
            }





            return false;
        }

    }
}
