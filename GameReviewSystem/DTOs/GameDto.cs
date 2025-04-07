namespace GameReviewSystem.DTOs
{
    // Data Transfer Object (DTO) for representing game information.
    // This DTO is used to send game data from the server to the client.
    public class GameDto
    {
        // Unique identifier for the game.
        public int GameId { get; set; }

        // The title of the game.
        // Initialized to an empty string to ensure the property is never null.
        public string Title { get; set; } = string.Empty;

        // The platform on which the game is available (e.g., "PC", "Xbox", "PlayStation").
        // Initialized to an empty string.
        public string Platform { get; set; } = string.Empty;

        // The genre of the game (e.g., "Action", "RPG", "Puzzle").
        // Initialized to an empty string.
        public string Genre { get; set; } = string.Empty;

        // The current status of the game (e.g., "Backlog", "Pågående", "Klar").
        // Initialized to an empty string.
        public string Status { get; set; } = string.Empty;

        // The average rating for the game, calculated from its reviews.
        // This is a computed value that can be displayed to the user.
        public double AverageRating { get; set; }
    }
}
