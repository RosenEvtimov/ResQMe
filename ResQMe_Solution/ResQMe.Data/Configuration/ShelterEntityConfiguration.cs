namespace ResQMe.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe.Data.Models;

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
                Description = "Volunteer-run shelter dedicated to rescuing abandoned animals in Burgas.",
                ImageUrl = "https://i.ibb.co/Hf3QV5Kx/Furry-Friends-Refuge-AI.png",
                Phone = "056111222",
                Email = "furryfriends@gmail.com"
            },
            new Shelter()
            {
                Id = 2,
                Name = "Safe Paws",
                City = "Burgas",
                Address = "ul.Tsar Kaloyan 190",
                Description = "Non-profit organisation dedicated to fostering homeless animals in Burgas.",
                ImageUrl = "https://i.ibb.co/dsSj025K/Safe-Paws-AI.png",
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