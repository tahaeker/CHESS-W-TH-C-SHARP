using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class BoardState
    {

        public static (Cell, Cell) kingLocations(ChessContext ctx)
        {
            Cell whiteKingL = new Cell(-1, -1, ctx);
            Cell blackKingL = new Cell(-1, -1, ctx);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ctx.board[i, j] == 'K')
                    {

                        whiteKingL = new Cell(i, j, ctx);

                    }

                    if (ctx.board[i, j] == 'k')
                    {

                        blackKingL = new Cell(i, j, ctx);

                    }

                }

            }

            return (whiteKingL, blackKingL);
        }


        public static ChessContext copyBoard(ChessContext ctx)
        {
            ChessContext newCtx = new ChessContext();
            // Tahta kopyalama
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    newCtx.board[i, j] = ctx.board[i, j];

            newCtx.inputFrom = ctx.inputFrom;
            newCtx.inputTo = ctx.inputTo;
            newCtx.empty = ctx.empty;
            newCtx.whiteTurn = ctx.whiteTurn;
            newCtx.whiteKingMoved = ctx.whiteKingMoved;
            newCtx.whiteQueensideRookMoved = ctx.whiteQueensideRookMoved;
            newCtx.whiteKingsideRookMoved = ctx.whiteKingsideRookMoved;
            newCtx.blackKingMoved = ctx.blackKingMoved;
            newCtx.blackQueensideRookMoved = ctx.blackQueensideRookMoved;
            newCtx.blackKingsideRookMoved = ctx.blackKingsideRookMoved;
            newCtx.IsFakeMovement = ctx.IsFakeMovement;

            // Cell nesnelerini de kopyala (gerekirse)
            newCtx.touchedCell = new Cell(ctx.touchedCell.Row, ctx.touchedCell.Col, newCtx);
            newCtx.lastFromCell = new Cell(ctx.lastFromCell.Row, ctx.lastFromCell.Col, newCtx);
            newCtx.lastToCell = new Cell(ctx.lastToCell.Row, ctx.lastToCell.Col, newCtx);

            return newCtx;
        }


        public static bool IsWhitekingUnderThreat(ChessContext ctx)
        {
            (Cell whiteKing, _) = kingLocations(ctx);
            //fro each order stone to white king location
            for (int checkI = 0; checkI < 8; checkI++)
            {
                for (int checkJ = 0; checkJ < 8; checkJ++)
                {
                    if (checkI == 1 & checkJ == 3)
                    {

                    }
                    string fromCellOfThreat = BoardConverter.IndexToString(checkI, checkJ);
                    ChessContext tempCtx = copyBoard(ctx);
                    Cell fromCellOfThreatCell = BoardConverter.StringToCell(fromCellOfThreat, tempCtx);

                    if (fromCellOfThreatCell.stone == 'q' && whiteKing.cellString == "d1")
                    {

                    }

                    // kendi taşın kendini tehtid edemeyeceği için renk değiştirdik
                    tempCtx.whiteTurn = false;

                    if (!fromCellOfThreatCell.IsWhite && ErrorChecker.MoveError(fromCellOfThreat, whiteKing.cellString, tempCtx) == "")

                        return true;

                }


            }
            return false;
        }

        public static bool IsBlackkingUnderThreat(ChessContext ctx)
        {
            (Cell whiteKing, Cell blackKing) = kingLocations(ctx);


            //fro each order stone to white king location
            for (int checkI = 0; checkI < 8; checkI++)
            {
                for (int checkJ = 0; checkJ < 8; checkJ++)
                {
                    if (checkI == 7 & checkJ == 3)
                    {

                    }
                    string fromCellOfThreat = BoardConverter.IndexToString(checkI, checkJ);
                    ChessContext tempCtx = copyBoard(ctx);
                    Cell fromCellOfThreatCell = BoardConverter.StringToCell(fromCellOfThreat, tempCtx);

                    // kendi taşın kendini tehtid edemeyeceği için renk değiştirdik
                    tempCtx.whiteTurn = true;
                    if (fromCellOfThreatCell.IsWhite && ErrorChecker.MoveError(fromCellOfThreat, blackKing.cellString, tempCtx) == "")
                        return true;

                }

            }
            return false;
        }

        public static (bool, bool) IsThereJustKing(ChessContext ctx)
        {
            bool whiteKing = false;
            bool blackKing = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char ch = ctx.board[i, j];
                    if (ch == 'K')
                        whiteKing = true;
                    if (ch == 'k')
                        blackKing = true;
                    if (ch != 'K' && IsStoneWhiteC(ch))
                        whiteKing = false;
                    if (ch != 'k' && IsStoneWhiteC(ch))
                        blackKing = false;


                }
            }
            // Sadece iki şah varsa true, true dön
            return (whiteKing, blackKing);
        }

        public static void CheckGameEnd(ChessContext ctx)
        {

            bool hasLegalMove = false;
            bool isWhiteTurn = ctx.whiteTurn;
            bool kingUnderThreat = isWhiteTurn ? IsWhitekingUnderThreat(ctx) : IsBlackkingUnderThreat(ctx);

            //the point in order for fromCEll
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char fromch = ctx.board[i, j];

                    if (fromch == '.') continue;

                    if ((isWhiteTurn && !IsStoneWhiteC(fromch)) || (!isWhiteTurn && IsStoneWhiteC(fromch)))
                        continue;

                    string from = BoardConverter.IndexToString(i, j);
                    //toCell in order
                    for (int iTo = 0; iTo < 8; iTo++)
                    {
                        for (int jTo = 0; jTo < 8; jTo++)
                        {

                            if (i == iTo && j == jTo) continue;
                            string to = BoardConverter.IndexToString(iTo, jTo);

                            ChessContext tempCtx = copyBoard(ctx);
                            string error = ErrorChecker.MoveError(from, to, tempCtx);

                            if (error == "")
                            {
                                hasLegalMove = true;
                                break;
                            }
                        }
                        if (hasLegalMove) break;
                    }
                    if (hasLegalMove) break;
                }
                if (hasLegalMove) break;
            }
            ctx.whiteWins = false;
            ctx.blackWins = false;
            ctx.drawStuation = false;

            if (!hasLegalMove && kingUnderThreat)
            {
                if (isWhiteTurn)
                    ctx.blackWins = true;
                else
                    ctx.whiteWins = true;
            }
            else if (!hasLegalMove && !kingUnderThreat)
            {
                ctx.drawStuation = true;
            }
            else if (ctx.howMuchBlackkingMoved == 50 || ctx.howMuchWhitekingMoved == 50 ||
                     (IsThereJustKing(ctx).Item1 && IsThereJustKing(ctx).Item2))
            {
                ctx.drawStuation = true;
            }


        }


        public static bool IsStoneWhiteC(char ch)
        {
            char stone = ch;

            if (stone == 'P' | stone == 'R' |
                stone == 'N' | stone == 'B' |
                stone == 'Q' | stone == 'K')
            {
                return true;
            }

            return false;

        }


    }


}

