using ChessEngine.Core;



namespace ChessWebApi.Models
{
    public class Game
    {
        public int Id { get; set; }// Primary key
        public DateTime CreatedAt { get; set; } // Oyun oluşturulma tarihi
        public string Status { get; set; } // ongoing, white_won, black_won, draw

        public List<Move> Moves { get; set; } = new ();// Hamleleri tutan liste
    }
}
