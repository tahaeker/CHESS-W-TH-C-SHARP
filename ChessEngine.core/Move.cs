
namespace ChessEngine.Core
{
    public class Move
    {
        public int Id { get; set; } // veritabanı için primary key

        public string From { get; set; }   // e2
        public string To { get; set; }     // e4
        public char Piece { get; set; }    // oynayan taş (örn. 'P')
        public char Captured { get; set; } // varsa alınan taş, yoksa '.'
        public int TurnNumber { get; set; } // hangi turda yapıldı
        public bool IsWhiteTurn { get; set; }

        public int gameId { get; set; } // hangi oyuna ait olduğunu belirtir


        public override string ToString()// içerisinde return olmalo
        {
            string capturePart = Captured != '.' ? $"x{Captured}" : "";
            string moveText = $" {From}-{To} ({Piece}{capturePart})";
            return IsWhiteTurn 
                ? $"{TurnNumber}. {moveText}" 
                : $" - {moveText}";

        }


    }


}




