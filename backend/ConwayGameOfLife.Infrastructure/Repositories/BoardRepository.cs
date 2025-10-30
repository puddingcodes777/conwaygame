using ConwayGameOfLife.Infrastructure.Data;
using ConwayGameOfLife.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Infrastructure.Repositories
{
    // Repository for accessing and managing GameBoard entities in the database.
    public class BoardRepository
    {
        private readonly GameOfLifeDbContext _dbContext;

        public BoardRepository(GameOfLifeDbContext dbContext    )
        {
            _dbContext = dbContext?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Gets a Gameboard by Its Id.
        public async Task<GameBoard?> GetByIdAsync(int id)
        {
            return await _dbContext.GameBoards.FindAsync(id);
        }

        // Gets all saved Gameboards.
        public async Task<List<GameBoard>> GetAllAsync()
        {
            return await _dbContext.GameBoards.ToListAsync();
        }


        // Adds a new Gameboard to the database.
        public async Task AddAsync(GameBoard board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));

            await _dbContext.GameBoards.AddAsync(board);
            await _dbContext.SaveChangesAsync();
        }

        // Updates an existing GameBoard in the database.
        public async Task UpdateAsync(GameBoard board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));

            _dbContext.GameBoards.Update(board);
            await _dbContext.SaveChangesAsync();
        }

        // Deletes a GameBoard by Its Id.
        public async Task DeleteAsync(int id)
        {
            var board = await _dbContext.GameBoards.FindAsync(id);
            if (board != null)
            {
                _dbContext.GameBoards.Remove(board);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
