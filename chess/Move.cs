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


    }


}




