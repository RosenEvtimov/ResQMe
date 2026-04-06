namespace ResQMe.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe.Data.Models;
    using static System.Net.WebRequestMethods;

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
            },
            new Shelter()
            {
                Id = 3,
                Name = "Hope Haven",
                City = "Varna",
                Address = "ul.Hristo Botev 42",
                Description = "Welcoming rescue center founded with the purpose of giving abandoned animals a second chance.",
                ImageUrl = "https://i.ibb.co/F4vkpv4x/Hope-Haven.jpg",
                Phone = "056555666",
                Email = "hopehaven@gmail.com"
            },
            new Shelter()
            {
                Id = 4,
                Name = "Furever Home",
                City = "Sofia",
                Address = "ul.James Bourchier 85",
                Description = "Compassionate animal shelter focused on rescuing, rehabilitating, and rehoming animals in need of care and love.",
                ImageUrl = "https://i.ibb.co/6cmWSMT9/Furever-Home.png",
                Phone = "056777888",
                Email = "fureverhome@gmail.com"
            }
        };

        public void Configure(EntityTypeBuilder<Shelter> entity)
        {
            entity.HasData(shelters);
        }
    }
}