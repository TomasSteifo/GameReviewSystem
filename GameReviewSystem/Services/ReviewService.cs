using GameReviewSystem.Data;
using GameReviewSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GameReviewSystem.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateReviewAsync(Review newReview)
        {
            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();
            return newReview;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            // Optionally include related Game and User data
            return await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task<bool> UpdateReviewAsync(Review updatedReview)
        {
            _context.Reviews.Update(updatedReview);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var reviewToDelete = await _context.Reviews.FindAsync(id);
            if (reviewToDelete == null) return false;

            _context.Reviews.Remove(reviewToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Review>> GetReviewsByGameIdAsync(int gameId)
        {
            return await _context.Reviews
                .Where(r => r.GameId == gameId)
                .Include(r => r.Game)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.Game)
                .Include(r => r.User)
                .ToListAsync();
        }
    }
}
