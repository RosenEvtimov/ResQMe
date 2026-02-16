namespace ResQMe.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Identity;

    public class ResQMeDbContext : IdentityDbContext<ApplicationUser>
    {
        public ResQMeDbContext(DbContextOptions<ResQMeDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<AdoptionRequest> AdoptionRequests { get; set; } = null!;

        public virtual DbSet<Animal> Animals { get; set; } = null!;

        public virtual DbSet<Breed> Breeds { get; set; } = null!;

        public virtual DbSet<Shelter> Shelters { get; set; } = null!;

        public virtual DbSet<Species> Species { get; set; } = null!;

        /* Enabling Fluent API */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ResQMeDbContext).Assembly);

            // Prevents: Two species both named "Dog"
            builder.Entity<Species>()
                .HasIndex(s => s.Name)
                .IsUnique();

            // Allows: "Labrador" for Dogs AND "Labrador" for Cats (different SpeciesId)
            // Prevents: Two "Labrador" breeds both assigned to Dogs (same SpeciesId)
            builder.Entity<Breed>()
                .HasIndex(b => new { b.Name, b.SpeciesId })
                .IsUnique();

            // Allows: "AppleCare" at "123 Main St" AND "AppleCare" at "456 Oak Ave"
            // Prevents: Two shelters both named "AppleCare" at "123 Main St"
            builder.Entity<Shelter>()
                .HasIndex(s => new { s.Name, s.Address })
                .IsUnique();


            builder.Entity<AdoptionRequest>()
                .HasIndex(ar => new { ar.UserId, ar.AnimalId })
                .IsUnique();
        }
    }
}