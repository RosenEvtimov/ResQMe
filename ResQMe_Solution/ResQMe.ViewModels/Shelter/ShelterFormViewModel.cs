namespace ResQMe.ViewModels.Shelter
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class ShelterFormViewModel
    {
        [Required]
        [StringLength(MaxShelterNameLength, MinimumLength = MinShelterNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(MaxShelterCityLength, MinimumLength = MinShelterCityLength)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(MaxShelterAddressLength, MinimumLength = MinShelterAddressLength)]
        public string Address { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(MaxShelterPhoneLength, MinimumLength = MinShelterPhoneLength)]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(MaxShelterEmailLength, MinimumLength = MinShelterEmailLength)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(MaxShelterDescriptionLength, MinimumLength = MinShelterDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;
    }
}