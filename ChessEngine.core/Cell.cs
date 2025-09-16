namespace ChessEngine.Core
{
    public struct Cell
    {

        public int Row { get; }
        public int Col { get; }
        public char Stone { get; set; }

        public (int, int) CellIndex => (Row, Col);
        public string CellString => BoardConverter.IndexToString(Row, Col);

        public bool IsEmpty => Stone == '.';

        private static readonly char[] WhitePieces = { 'P', 'R', 'N', 'B', 'Q', 'K' };
        private static readonly char[] BlackPieces = { 'p', 'r', 'n', 'b', 'q', 'k' };

        public bool IsWhite => WhitePieces.Contains(Stone);
        public bool IsBlack => BlackPieces.Contains(Stone);

        public bool IsTouchedCellMovable;


        public Cell(int i, int j, ChessContext ctx)
        {

            Row = i;
            Col = j;
            if (ctx != null && i > -1 && j > -1)
            {//chess context is not null
                Stone = ctx.Board[i, j];
            }
            else
            {
                Stone = '.';// default empty stone
            }


            

        }

        //public void TouchedCellMovable(int i,int j, ChessContext ctx)
        //{
        //    /// Can move Touched from Cell  
        //    ChessContext tempCtx = BoardState.copyBoard(ctx);
        //    tempCtx.IsFakeMovement = false;
        //    bool state = false;
        //    string from = BoardConverter.IndexToString(i, j);
        //    for (int rowT = 0; rowT<8; rowT++)
        //    {
        //        for (int colT = 0; colT<8; colT++)
        //        {

        //            string to = BoardConverter.IndexToString(rowT, colT);
        //            Cell fromCell = BoardConverter.StringToCell(from, tempCtx);
        //            bool stoneColorW = fromCell.IsWhite ? true : false;
        //            string error = ErrorChecker.MoveError(from, to, tempCtx);
        //            if (stoneColorW)
        //            {
        //                tempCtx.whiteTurn =true;
        //                state = error == "" ? true : false;
        //                if (state)
        //                    IsTouchedCellMovable =true;

        //            }
        //            else if (!stoneColorW)
        //            {
        //                tempCtx.whiteTurn =false;
        //                state = error == "" ? true : false;
        //                if (state)
        //                    IsTouchedCellMovable =true;
        //            }
        //            tempCtx.IsFakeMovement = false;

        //        }

        //    }
        //}



    }
}

        
    
