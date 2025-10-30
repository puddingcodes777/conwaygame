using ConwayGameOfLife.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Infrastructure.Data
{
    public class GameOfLifeDbContext : DbContext
    {
        public GameOfLifeDbContext(DbContextOptions<GameOfLifeDbContext> options) : base(options) { }
        public DbSet<GameBoard> GameBoards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameBoard>().ToTable("GameBoards");
            modelBuilder.Entity<GameBoard>().HasKey(x => x.Id);
        }
    }
}
