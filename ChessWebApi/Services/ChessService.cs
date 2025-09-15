using ChessEngine.Core;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using System;


namespace ChessWebApi.Services
{
    public class ChessService
    {
        private readonly ChessContext _context;
        public ChessService()
        {
            _context = new ChessContext();

            if (_context.BoardHistory == null || _context.BoardHistory.Count==0)
            {
                _context.Board = new char[8, 8]{
                { 'r','n','b','q','k','b','n','r' },
                { 'p','p','p','p','p','p','p','p' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { 'P','P','P','P','P','P','P','P' },
                { 'R','N','B','Q','K','B','N','R' }
                };
            }
            else
            {
                _context.Board = _context.BoardHistory.Last();
            }

        }
        public void StartNewGame()
        {
            
            _context.Board= new char[8, 8]
                {
                { 'r','n','b','q','k','b','n','r' },
                { 'p','p','p','p','p','p','p','p' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { 'P','P','P','P','P','P','P','P' },
                { 'R','N','B','Q','K','B','N','R' }
            };
            _context.WhitePlayer = new Player("Alice", true);
            _context.BlackPlayer = new Player("Bob", false);
            _context.MoveHistory.Clear();
            _context.whiteTurn = true;
        }

        public void takePlayerName(string nameWhite,string nameBlack)
        {
            _context.WhitePlayer = new Player(nameWhite, true);
            _context.BlackPlayer = new Player(nameBlack, false);
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
