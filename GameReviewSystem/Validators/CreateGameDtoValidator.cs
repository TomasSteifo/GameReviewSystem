using FluentValidation;
using GameReviewSystem.DTOs;

namespace GameReviewSystem.Validators
{
    public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
    {
        public CreateGameDtoValidator()
        {
            // Example rules:

            // 1) Title must not be empty, max length 100
            RuleFor(g => g.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            // 2) Platform must not be empty
            RuleFor(g => g.Platform)
                .NotEmpty().WithMessage("Platform is required.");

            // 3) Genre must not be empty
            RuleFor(g => g.Genre)
                .NotEmpty().WithMessage("Genre is required.");

            // 4) Status must not be empty, and must be one of known values
            RuleFor(g => g.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Backlog", "Pågående", "Klar" }.Contains(status))
                .WithMessage("Status must be one of: Backlog, Pågående, or Klar.");
        }
    }
}
