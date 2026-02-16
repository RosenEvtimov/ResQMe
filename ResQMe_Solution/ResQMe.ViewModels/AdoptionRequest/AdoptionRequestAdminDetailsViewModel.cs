namespace ResQMe.ViewModels.AdoptionRequest
{
    using ResQMe.Data.Models.Enums;

    public class AdoptionRequestAdminDetailsViewModel
    {
        public int Id { get; set; }

        public string AnimalName { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;

        public string? Message { get; set; }

        public PreviousAdoptionExperience PreviousAdoptionExperience { get; set; }

        public DateTime CreatedOn { get; set; }

        public AdoptionRequestStatus Status { get; set; }
    }
}