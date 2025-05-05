using System;
using System.Collections.Generic;
using GameReviewSystem.Domain.Common;

namespace GameReviewSystem.Domain.Entities
{
    public class Game : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }

        // ► Navigations
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
