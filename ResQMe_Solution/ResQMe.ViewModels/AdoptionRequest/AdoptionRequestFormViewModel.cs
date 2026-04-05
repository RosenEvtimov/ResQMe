namespace ResQMe.ViewModels.AdoptionRequest
{
    using System.ComponentModel.DataAnnotations;
    using ResQMe.Data.Models.Enums;

    public class AdoptionRequestFormViewModel
    {
        public int AnimalId { get; set; }

        [Required]
        public PreviousAdoptionExperience? PreviousAdoptionExperience { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }

        public string? ReturnUrl { get; set; }
        public string? ShelterReturnUrl { get; set; }
        public int? FromShelterId { get; set; }
    }
}