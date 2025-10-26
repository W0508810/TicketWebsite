using Microsoft.EntityFrameworkCore;
using TicketScam.Models;

namespace TicketScam
{
    public class TicketScamContext : DbContext
    {
        public TicketScamContext(DbContextOptions<TicketScamContext> options)
            : base(options)
        {
        }

        // DbSet properties - these match what your controllers are looking for
        public DbSet<Category> Category { get; set; }
        public DbSet<Show> Show { get; set; }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for TicketPrice
            modelBuilder.Entity<Show>()
                .Property(s => s.TicketPrice)
                .HasPrecision(18, 2);

            // Optional: Seed some initial data
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Concert" },
                new Category { CategoryId = 2, Name = "Sports" },
                new Category { CategoryId = 3, Name = "Theater" },
                new Category { CategoryId = 4, Name = "Comedy" }
            );

            modelBuilder.Entity<Venue>().HasData(
                new Venue { VenueId = 1, VenueName = "Scotia Bank Centre", Location = "Halifax, NS", VenueCapacity = 10000 },
                new Venue { VenueId = 2, VenueName = "Rebecca Cohn Auditorium", Location = "Halifax, NS", VenueCapacity = 1040 }
            );
        }
    }
}