using ResQMe.Data.Models.Enums;

namespace ResQMe.ViewModels.Animal
{
    public class AnimalListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public string Species { get; set; } = null!;

        public string BreedDisplay { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public bool IsAdopted { get; set; }

        public string Shelter { get; set; } = null!;
    }
}