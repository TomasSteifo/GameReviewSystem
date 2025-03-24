namespace GameReviewSystem.DTOs
{
    public class CreateGameDto
    {
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Status { get; set; } = "Backlog";
    }
}
