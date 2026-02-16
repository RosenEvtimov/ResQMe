namespace ResQMe.ViewModels.AdoptionRequest
{
    using ResQMe.Data.Models.Enums;

    public class MyAdoptionRequestsViewModel
    {
        public int Id { get; set; }

        public string AnimalName { get; set; } = null!;

        public string AnimalImageUrl { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public AdoptionRequestStatus Status { get; set; }
    }
}