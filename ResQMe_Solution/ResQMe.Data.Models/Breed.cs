namespace ResQMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static ResQMe.GCommon.EntityValidation;

    public class Breed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(MaxBreedNameLength, MinimumLength = MinBreedNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Species))]
        public int SpeciesId { get; set; }
        public virtual Species Species { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
