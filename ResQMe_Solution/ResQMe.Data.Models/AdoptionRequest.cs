namespace ResQMe.Data.Models
{
    using Models.Enums;
    using ResQMe.Data.Models.Identity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static ResQMe.GCommon.EntityValidation;

    public class AdoptionRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Animal))]
        public int AnimalId { get; set; }
        public virtual Animal Animal { get; set; } = null!;

        /* Link to logged in User */
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        [MaxLength(MaxAdoptionRequestMessageLength)]
        public string? Message { get; set; }

        [Required]
        public PreviousAdoptionExperience PreviousAdoptionExperience { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public AdoptionRequestStatus Status { get; set; } = AdoptionRequestStatus.Pending;
    }
}