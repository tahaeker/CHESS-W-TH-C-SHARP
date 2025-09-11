using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class Player
    {
        public string Name { get; set; }

        public bool IsWhite { get; set; }

        public int Wins { get; set; } = 0;

        public int Losses { get; set; } = 0;

        public Player(string name, bool isWhite)
        {
            Name = name;
            IsWhite = isWhite;

        }



    }
}
