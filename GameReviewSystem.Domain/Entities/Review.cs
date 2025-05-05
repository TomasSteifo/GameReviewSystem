using System;
using GameReviewSystem.Domain.Common;

namespace GameReviewSystem.Domain.Entities
{
    public class Review : BaseEntity
    {
        // 1–10
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;

        // ► Foreign Keys
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }

        // ► Navigations
        public Game Game { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
