namespace ResQMe_Project.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ResQMe_Project.Models;

    public class BreedEntityConfiguration : IEntityTypeConfiguration<Breed>
    {
        private readonly Breed[] breeds = new Breed[]
        {
            /* Dog Breeds */
            new Breed()
            {
                Id = 1,
                Name = "French Bulldog",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 2,
                Name = "Labrador Retriever",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 3,
                Name = "Golden Retriever",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 4,
                Name = "German Shepherd",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 5,
                Name = "Poodle",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 6,
                Name = "English Bulldog",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 7,
                Name = "Beagle",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 8,
                Name = "Rottweiler",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 9,
                Name = "German Shorthaired Pointer",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 10,
                Name = "Dachshund",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 11,
                Name = "Pembroke Welsh Corgi",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 12,
                Name = "Australian Shepherd",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 13,
                Name = "Yorkshire Terrier",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 14,
                Name = "Cavalier King Charles Spaniel",
                SpeciesId = 1
            },
            new Breed()
            {
                Id = 15,
                Name = "Doberman Pinscher",
                SpeciesId = 1
            },

            /* Cat breeds */
            new Breed()
            {
                Id = 16,
                Name = "Ragdoll",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 17,
                Name = "Maine Coon",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 18,
                Name = "Persian",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 19,
                Name = "Exotic Shorthair",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 20,
                Name = "British Shorthair",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 21,
                Name = "Devon Rex",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 22,
                Name = "Abyssinian",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 23,
                Name = "Scottish Fold",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 24,
                Name = "Sphynx",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 25,
                Name = "Siberian",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 26,
                Name = "American Shorthair",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 27,
                Name = "Russian Blue",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 28,
                Name = "Norwegian Forest Cat",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 29,
                Name = "Oriental Shorthair",
                SpeciesId = 2
            },
            new Breed()
            {
                Id = 30,
                Name = "Bengal",
                SpeciesId = 2
            }

            /* Dogs Special */
            //new Breed()
            //{
            //    Id = 31,
            //    Name = "Mixed",
            //    SpeciesId = 1
            //},
            //new Breed()
            //{
            //    Id = 32,
            //    Name = "Unknown",
            //    SpeciesId = 1
            //},

            /* Cats Special */
            //new Breed()
            //{
            //    Id = 33,
            //    Name = "Mixed",
            //    SpeciesId = 2
            //},
            //new Breed()
            //{
            //    Id = 34,
            //    Name = "Unknown",
            //    SpeciesId = 2
            //}
        };

        public void Configure(EntityTypeBuilder<Breed> entity)
        {
            entity.HasData(breeds);
        }
    }
}