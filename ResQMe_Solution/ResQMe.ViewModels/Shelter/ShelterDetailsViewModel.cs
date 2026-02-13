namespace ResQMe.ViewModels.Shelter
{
    public class ShelterDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public IEnumerable<ShelterAnimalsViewModel> Animals { get; set; }
            = new List<ShelterAnimalsViewModel>();
    }
}