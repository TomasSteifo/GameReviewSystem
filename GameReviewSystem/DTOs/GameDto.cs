namespace GameReviewSystem.DTOs
{
    public class GameDto
    {
        public int GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Possibly you want to return the average rating or 
        // the list of reviews. It's your choice:
        public double AverageRating { get; set; }
    }
}
