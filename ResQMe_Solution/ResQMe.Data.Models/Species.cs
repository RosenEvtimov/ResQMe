namespace ResQMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class Species
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxSpeciesNameLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Breed> Breeds { get; set; } = new HashSet<Breed>();

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}