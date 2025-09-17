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



        public Cell(int i, int j, ChessContext ctx)
        {

            Row = i;
            Col = j;
           
            if (ctx != null && i >-1 && j > -1 && i < Math.Sqrt(ctx.Board.Length) && j< Math.Sqrt(ctx.Board.Length))
            {//chess context is not null
                Stone = ctx.Board[i, j];
            }
            else
            {
                Stone = '.';// default empty stone
            }


            

        }

        



    }
}

        
    
