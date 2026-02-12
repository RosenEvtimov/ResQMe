namespace ResQMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static ResQMe.GCommon.EntityValidation;
    using Models.Enums;

    public class AdoptionRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Animal))]
        public int AnimalId { get; set; }
        public virtual Animal Animal { get; set; } = null!;

        [Required]
        [StringLength(MaxAdoptionRequestFirstNameLength, MinimumLength = MinAdoptionRequestFirstNameLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(MaxAdoptionRequestLastNameLength, MinimumLength = MinAdoptionRequestLastNameLength)]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        [StringLength(MaxAdoptionRequestEmailLength, MinimumLength = MinAdoptionRequestEmailLength)]
        public string? Email { get; set; }

        [Required]
        [Phone]
        [StringLength(MaxAdoptionRequestPhoneLength, MinimumLength = MinAdoptionRequestPhoneLength)]
        public string Phone { get; set; } = null!;

        [MaxLength(MaxAdoptionRequestMessageLength)]
        public string? Message { get; set; }  // message FROM user

        [Required]
        public PreviousAdoptionExperience PreviousAdoptionExperience { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;
    }
}