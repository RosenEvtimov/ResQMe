using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResQMe.Data.Models;

namespace ResQMe_Project.Data
{
    public class ResQMeDbContext : IdentityDbContext
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
        }
    }
}
