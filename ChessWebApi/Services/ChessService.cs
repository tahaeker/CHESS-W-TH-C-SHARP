using ChessEngine.Core;


namespace ChessWebApi.Services
{
    public class ChessService
    {
        private readonly ChessContext _context;

        public ChessService()
        {
            _context = new ChessContext();
            _context.Board = new char[8, 8]{
                { 'r','n','b','q','k','b','n','r' },
                { 'p','p','p','.','.','.','p','p' },
                { '.','.','.','.','.','q','.','.' },
                { '.','.','b','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { 'P','P','P','.','.','.','P','P' },
                { 'R','.','.','Q','K','B','N','R' }
            };

            _context.WhitePlayer = new Player("Alice", true);
            _context.BlackPlayer = new Player("Bob", false);
        }

        public bool TryMove(string from, string to, out string message)
        {
            var result = ChessEngine.Core.ChessEngine.TryMove(from, to, _context);

            if (!result.Success)
            {
                message = result.ErrorMessage;
                return false;
            }

            message = "Move successful!";
            return true;
        }

        public IEnumerable<string> GetBoard()
        {
            var rows = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                string row = "";
                for (int j = 0; j < 8; j++)
                {
                    row += _context.Board[i, j] + " ";
                }
                rows.Add(row.Trim());
            }
            return rows;
        }

        public IEnumerable<Move> GetHistory()
        {
            return _context.MoveHistory;
        }
    }
}
