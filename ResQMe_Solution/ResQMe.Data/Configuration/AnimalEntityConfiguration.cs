namespace ResQMe.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;

    public class AnimalEntityConfiguration : IEntityTypeConfiguration<Animal>
    {
        private readonly Animal[] animals = new Animal[]
        {
            new Animal
            {
                Id = 1,
                Name = "Max",
                Age = 3,
                Gender = Gender.Male,
                SpeciesId = 1, // Dog
                BreedType = BreedType.Purebred,
                BreedId = 3, // Golden Retriever
                Description = "Friendly and energetic dog.",
                ImageUrl = "https://www.borrowmydoggy.com/_next/image?url=https%3A%2F%2Fcdn.sanity.io%2Fimages%2F4ij0poqn%2Fproduction%2Fda89d930fc320dd912a2a25487b9ca86b37fcdd6-800x600.jpg&w=640&q=80",
                IsAdopted = false,
                ShelterId = 1
            },
            new Animal
            {
                Id = 2,
                Name = "Bella",
                Age = 2,
                Gender = Gender.Female,
                SpeciesId = 2, // Cat
                BreedType = BreedType.Mixed,
                BreedId = null,
                Description = "Calm indoor cat.",
                ImageUrl = "https://www.trupanion.com/images/trupanionwebsitelibraries/bg/mixed-breed-cat.jpg?sfvrsn=959c7b73_1",
                IsAdopted = false,
                ShelterId = 2
            },
            new Animal
            {
                Id = 3,
                Name = "Rocky",
                Age = 5,
                Gender = Gender.Male,
                SpeciesId = 1,
                BreedType = BreedType.Unknown,
                BreedId = null,
                Description = "Rescued stray dog.",
                ImageUrl = "https://media.4-paws.org/8/5/5/4/85545b5798b851ea1cf81552bc56a49d96a71882/VIER%20PFOTEN_2023-10-19_00151-2850x1900-2746x1900-1920x1328.jpg",
                IsAdopted = true,
                ShelterId = 1
            }
        };

        public void Configure(EntityTypeBuilder<Animal> entity)
        {
            entity.HasData(animals);
        }
    }
}