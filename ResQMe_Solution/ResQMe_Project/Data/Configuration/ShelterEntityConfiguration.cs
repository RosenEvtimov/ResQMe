namespace ResQMe_Project.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe_Project.Models;

    public class ShelterEntityConfiguration : IEntityTypeConfiguration<Shelter>
    {
        private readonly Shelter[] shelters = new Shelter[]
        {
            new Shelter()
            {
                Id = 1,
                Name = "Furry Friends Refuge",
                City = "Burgas",
                Address = "ul.Mariya Luiza 10",
                Phone = "056111222",
                Email = "furryfriends@gmail.com"
            },
            new Shelter()
            {
                Id = 2,
                Name = "Safe Paws",
                City = "Burgas",
                Address = "ul.Tsar Kaloyan 190",
                Phone = "056333444",
                Email = "safepaws@gmail.com"
            }
        };

        public void Configure(EntityTypeBuilder<Shelter> entity)
        {
            entity.HasData(shelters);
        }
    }
}