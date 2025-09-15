using Microsoft.EntityFrameworkCore;
using ChessEngine.Core;
using ChessWebApi.Models;




namespace ChessWebApi.Data
{
    public class ChessDbContext : DbContext
    {
        public ChessDbContext(DbContextOptions<ChessDbContext> options) : base(options)
        {
        }


        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }




    }
}
