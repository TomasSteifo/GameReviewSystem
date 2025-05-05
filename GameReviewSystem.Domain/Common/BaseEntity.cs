using System;

namespace GameReviewSystem.Domain.Common
{
    /// <summary>
    /// Bas‐klass som ger alla entiteter ett Id samt skapar‑ och uppdateringstider.
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
