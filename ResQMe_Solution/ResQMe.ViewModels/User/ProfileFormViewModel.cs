namespace ResQMe.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class ProfileFormViewModel
    {
        [Required]
        [StringLength(MaxUserFirstNameLength, MinimumLength = MinUserFirstNameLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(MaxUserLastNameLength, MinimumLength = MinUserLastNameLength)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(MaxUserAddressLength,MinimumLength = MinUserAddressLength)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(MaxUserCityLength, MinimumLength = MinUserCityLength)]
        public string City { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}