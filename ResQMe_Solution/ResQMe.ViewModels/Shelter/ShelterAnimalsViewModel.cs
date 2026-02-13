using ResQMe.Data.Models.Enums;

namespace ResQMe.ViewModels.Shelter
{
    public class ShelterAnimalsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}