namespace GameReviewSystem.DTOs
{
    // This DTO (Data Transfer Object) is used when creating a new Game.
    // It contains only the properties required to create a Game entity.
    public class CreateGameDto
    {
        // The title of the game.
        // Initialized to an empty string to ensure it is not null.
        public string Title { get; set; } = string.Empty;

        // The platform on which the game is available (e.g., "PC", "Xbox", "PlayStation").
        // Also initialized to an empty string.
        public string Platform { get; set; } = string.Empty;

        // The genre of the game (e.g., "Action", "RPG").
        // Initialized to an empty string.
        public string Genre { get; set; } = string.Empty;

        // The current status of the game (e.g., "Backlog", "Pågående", "Klar").
        // A default value of "Backlog" is provided.
        public string Status { get; set; } = "Backlog";
    }
}
