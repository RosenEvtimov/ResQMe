namespace ResQMe.ViewModels.AdoptionRequest
{
    using ResQMe.Data.Models.Enums;

    public class AdoptionRequestAdminListViewModel
    {
        public int Id { get; set; }

        public string AnimalName { get; set; } = null!;

        public string ApplicantFullName { get; set; } = null!;

        public string City { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public AdoptionRequestStatus Status { get; set; }
    }
}