using GameReviewSystem.Models;

namespace GameReviewSystem.Services
{
    public interface IGameService
    {
        // Basic CRUD
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<Game?> GetGameByIdAsync(int gameId);
        Task<Game> CreateGameAsync(Game newGame);
        Task<bool> UpdateGameAsync(Game updatedGame);
        Task<bool> DeleteGameAsync(int gameId);

        // Additional queries
        Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre);
        Task<IEnumerable<Game>> GetGamesByStatusAsync(string status);
    }
}
