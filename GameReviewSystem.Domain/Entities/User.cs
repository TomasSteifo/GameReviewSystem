using System;
using System.Collections.Generic;
using GameReviewSystem.Domain.Common;

namespace GameReviewSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // ► Enkel RBAC – roller som strängar
        public ICollection<string> Roles { get; set; } = new List<string>();

        // ► Navigations
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
