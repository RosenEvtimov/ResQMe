namespace ResQMe_Project.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class Species
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(MaxSpeciesNameLength, MinimumLength = MinSpeciesNameLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Breed> Breeds { get; set; } = new HashSet<Breed>();

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
