namespace ResQMe.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe.Data.Models;

    public class SpeciesEntityConfiguration : IEntityTypeConfiguration<Species>
    {
        private readonly Species[] species = new Species[]
        {
            new Species()
            {
                Id = 1,
                Name = "Dog"
            },
            new Species()
            {   
                Id = 2,
                Name = "Cat"
            }
        };

        public void Configure(EntityTypeBuilder<Species> entity)
        {
            entity.HasData(species);
        }
    }
}