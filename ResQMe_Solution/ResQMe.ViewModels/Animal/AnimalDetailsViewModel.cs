using ResQMe.Data.Models.Enums;

namespace ResQMe.ViewModels.Animal
{
    public class AnimalDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string BreedDisplay { get; set; } = null!;

        public string Species { get; set; } = null!;

        public bool IsAdopted { get; set; }

        // Shelter info
        public string ShelterName { get; set; } = null!;
        public string ShelterCity { get; set; } = null!;
        public string ShelterAddress { get; set; } = null!;
        public string ShelterPhone { get; set; } = null!;
        public string ShelterEmail { get; set; } = null!;
    }
}
