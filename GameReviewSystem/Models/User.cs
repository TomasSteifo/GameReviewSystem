using GameReviewSystem.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;  // <-- add this line
    // Missing property that code expects:
    public ICollection<Review> Reviews { get; set; }
}
