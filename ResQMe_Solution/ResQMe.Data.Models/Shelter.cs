namespace ResQMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class Shelter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxShelterNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(MaxShelterCityLength)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(MaxShelterAddressLength)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(MaxShelterDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(MaxShelterPhoneLength)]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(MaxShelterEmailLength)]
        public string Email { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}