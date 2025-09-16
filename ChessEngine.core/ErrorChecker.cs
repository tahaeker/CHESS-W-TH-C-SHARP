using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class MoveResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public MoveResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }

    public class ErrorChecker
    {
        public static string MoveError(string from, string to, ChessContext ctx)
        {
            if (from == "" && to == "")
            {
                return "";
            }

            Cell fromCell = BoardConverter.StringToCell(from, ctx);
            Cell toCell = BoardConverter.StringToCell(to, ctx);

            if(fromCell.CellIndex==toCell.CellIndex)
            {
                return "From and To cannot be the same!!";
            
            }

            if (fromCell.Stone == '.')
            {
                return "From Cell cannot be empty!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 1)
            {
                return "From or To cannot be empty!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 4)
            {
                return "From must be 2 characters long!!";
            }

            //if (!fromCell.IsTouchedCellMovable)
            //{
            //    return "from must be movable move";
            //}


            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 11)
            {
                return "To cannot be empty!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 5)
            {
                return "To must be 2 characters long!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 7)
            {
                return "To must be between 1-7!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 2)
            {
                return "From or To must be a valid letter!!";
            }
            if (InputHandler.IsValidFromToCondition(from, to, ctx) == 3)
            {
                return "Y Axis of From must be a valid between 1-7";
            }

            if (ctx.whiteTurn & !fromCell.IsWhite)
            {
                return "Row of White Stones";
            }
            else if (!ctx.whiteTurn & fromCell.IsWhite)
            {
                return "Row of black Stones";
            }


            if (fromCell.IsWhite & (toCell.IsWhite) & !toCell.IsEmpty)

            {

                return "Cannot Whites Move Owm Stone";
            }
            if (fromCell.IsBlack & (toCell.IsBlack & !toCell.IsEmpty))
            {
                return "Cannot Blacks Move Owm Stone";
            }

            switch (char.ToLower(fromCell.Stone))
            {
                case 'p':
                    if (!MoveValidator.IsValidPawnMove(fromCell, toCell, ctx))
                        return "Invalid pawn move!";
                    break;
                case 'r':
                    if (!MoveValidator.IsValidRookMove(fromCell, toCell, ctx))
                        return "Invalid rook move!";
                    break;
                case 'n':
                    if (!MoveValidator.IsValidKnightMove(fromCell, toCell, ctx))
                        return "Invalid knight move!";
                    break;
                case 'b':
                    if (!MoveValidator.IsValidBishopMove(fromCell, toCell, ctx))
                        return "Invalid bishop move!";
                    break;
                case 'q':
                    if (!MoveValidator.IsValidQueenMove(fromCell, toCell, ctx))
                        return "ınvalid queen move!";
                    break;
                case 'k':
                    if (!MoveValidator.IsValidKingMove(fromCell, toCell, ctx))
                        return "Invalid king move!";
                    break;

            }

            // check if the move puts the king in check
            if (!ctx.IsFakeMovement)
            {

                ChessContext nextCtx = BoardState.copyBoard(ctx);
                nextCtx.IsFakeMovement = true; // to prevent stack overflow exception

                ChessEngine.MoveStone(fromCell.CellString, toCell.CellString, nextCtx);

                if (nextCtx.whiteTurn && BoardState.IsBlackkingUnderThreat(nextCtx))
                    return "Black king is under threat";

                if (!nextCtx.whiteTurn && BoardState.IsWhitekingUnderThreat(nextCtx))
                    return "White king is under threat";




                ChessContext movableCtx2 = BoardState.copyBoard(ctx);
                movableCtx2.IsFakeMovement = true;
                if (MoveValidator.IsValidCastlingMove(fromCell, toCell, movableCtx2))
                {
                    bool isBlackKingUnderCheckInRouta = BoardState.IsBlackkingUnderThreat(movableCtx2);
                    bool isWhiteKingUnderCheckInRouta = BoardState.IsWhitekingUnderThreat(movableCtx2);

                    (Cell whiteKing, Cell blackKing) = BoardState.kingLocations(movableCtx2);
                    if (!BoardState.IsWhitekingUnderThreat(ctx))
                    {
                        //long white casling
                        if (toCell.CellIndex == (7, 2))
                        {
                            ChessEngine.MoveStone(whiteKing.CellString, "d1", movableCtx2);
                            isWhiteKingUnderCheckInRouta = BoardState.IsWhitekingUnderThreat(movableCtx2);
                            if (isWhiteKingUnderCheckInRouta)
                            {
                                return "White King Under Check in Long Casling Routa.";
                            }

                            ChessEngine.MoveStone("d1", "c1", movableCtx2);
                            isWhiteKingUnderCheckInRouta = BoardState.IsWhitekingUnderThreat(movableCtx2);
                            if (isWhiteKingUnderCheckInRouta)
                            {
                                return "White King Under Check in Long Casling Routa.";
                            }


                        }
                        //short white caslting
                        else if (toCell.CellIndex == (7, 6))
                        {

                            ChessEngine.MoveStone(whiteKing.CellString, "f1", movableCtx2);
                            isWhiteKingUnderCheckInRouta = BoardState.IsWhitekingUnderThreat(movableCtx2);
                            if (isWhiteKingUnderCheckInRouta)
                            {
                                return "White King Under Check in Short Casling Routa.";
                            }
                            ChessEngine.MoveStone("f1", "g1", movableCtx2);
                            isWhiteKingUnderCheckInRouta = BoardState.IsWhitekingUnderThreat(movableCtx2);
                            if (isWhiteKingUnderCheckInRouta)
                            {
                                return "White King Under Check in Short Casling Routa.";
                            }
                        }
                    }


                    if (!BoardState.IsBlackkingUnderThreat(ctx))
                    {
                        //long black castling
                        if (toCell.CellIndex == (0, 2))
                        {
                            ChessEngine.MoveStone(blackKing.CellString, "d8", movableCtx2);
                            isBlackKingUnderCheckInRouta = BoardState.IsBlackkingUnderThreat(movableCtx2);
                            if (isBlackKingUnderCheckInRouta)
                            {
                                return "Black King Under Check in Long Casling Routa.";
                            }
                            ChessEngine.MoveStone("d8", "c8", movableCtx2);
                            isBlackKingUnderCheckInRouta = BoardState.IsBlackkingUnderThreat(movableCtx2);
                            if (isBlackKingUnderCheckInRouta)
                            {
                                return "Black King Under Check in Long Casling Routa.";
                            }

                        }
                        // short black castling
                        else if (toCell.CellIndex == (0, 6))
                        {
                            ChessEngine.MoveStone(blackKing.CellString, "f8", movableCtx2);
                            isBlackKingUnderCheckInRouta = BoardState.IsBlackkingUnderThreat(movableCtx2);

                            if (isBlackKingUnderCheckInRouta)
                            {
                                return "Black King Under Check in Short Casling Routa.";
                            }

                            ChessEngine.MoveStone("f8", "g8", movableCtx2);
                            isBlackKingUnderCheckInRouta = BoardState.IsBlackkingUnderThreat(movableCtx2);

                            if (isBlackKingUnderCheckInRouta)
                            {
                                return "Black King Under Check in Short Casling Routa.";
                            }

                        }
                    }

                }
            }




            return "";


        }


    }
}
