using GameReviewSystem.Models;

namespace GameReviewSystem.Services
{
    public interface IReviewService
    {
        // CREATE
        Task<Review> CreateReviewAsync(Review newReview);

        // READ
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int id);

        // UPDATE
        Task<bool> UpdateReviewAsync(Review updatedReview);

        // DELETE
        Task<bool> DeleteReviewAsync(int id);

        // Extra: Get reviews by GameId or UserId
        Task<IEnumerable<Review>> GetReviewsByGameIdAsync(int gameId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);
    }
}
