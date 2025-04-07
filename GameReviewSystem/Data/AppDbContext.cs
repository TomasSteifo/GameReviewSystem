using Microsoft.EntityFrameworkCore;
using GameReviewSystem.Models;
using System.Collections.Generic;

namespace GameReviewSystem.Data
{
    // Define AppDbContext, which inherits from DbContext.
    // This is the primary class for interacting with your database using EF Core.
    public class AppDbContext : DbContext
    {
        // The constructor takes DbContextOptions of type AppDbContext.
        // These options typically include the connection string and other configuration details.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) // Pass the options to the base DbContext class.
        {
        }

        // Each DbSet property corresponds to a table in your database.
        // EF Core uses these properties to perform CRUD operations on the underlying tables.

        // The Users table: maps User entities to the "Users" table.
        public DbSet<User> Users { get; set; }

        // The Games table: maps Game entities to the "Games" table.
        public DbSet<Game> Games { get; set; }

        // The Reviews table: maps Review entities to the "Reviews" table.
        public DbSet<Review> Reviews { get; set; }
    }
}
