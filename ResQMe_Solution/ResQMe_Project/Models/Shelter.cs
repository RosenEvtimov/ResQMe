namespace ResQMe_Project.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class Shelter
    {
        [Key]
        public int Id { get; set; }

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

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}