using GameReviewSystem.Models;

namespace GameReviewSystem.Services
{
    public interface IUserService
    {
        // CREATE
        Task<User> CreateUserAsync(User newUser);

        // READ
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);

        // UPDATE
        Task<bool> UpdateUserAsync(User updatedUser);

        // DELETE
        Task<bool> DeleteUserAsync(int id);

        // Extra: Maybe a method to get a user by email or username
        Task<User?> GetUserByEmailAsync(string email);
    }
}
