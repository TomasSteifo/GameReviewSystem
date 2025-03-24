using GameReviewSystem.Data;
using GameReviewSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GameReviewSystem.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(Game newGame)
        {
            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();
            return newGame;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _context.Games
                .Include(g => g.Reviews)
                .ToListAsync();
        }

        public async Task<Game?> GetGameByIdAsync(int gameId)
        {
            return await _context.Games
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.GameId == gameId);
        }

        public async Task<bool> UpdateGameAsync(Game updatedGame)
        {
            _context.Games.Update(updatedGame);
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            var existingGame = await _context.Games.FindAsync(gameId);
            if (existingGame == null)
            {
                return false; // Indicate that the game was not found
            }

            _context.Games.Remove(existingGame);
            await _context.SaveChangesAsync();
            return true; // Indicate success
        }

        public async Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre)
        {
            return await _context.Games
                .Where(g => g.Genre == genre)
                .Include(g => g.Reviews)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByStatusAsync(string status)
        {
            return await _context.Games
                .Where(g => g.Status == status)
                .Include(g => g.Reviews)
                .ToListAsync();
        }
    }
}
