namespace ResQMe_Project.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static ResQMe.GCommon.EntityValidation;
    using Models.Enums;

    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(MaxAnimalNameLength, MinimumLength = MinAnimalNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinAnimalAge, MaxAnimalAge)]
        public int Age { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [ForeignKey(nameof(Species))]
        public int SpeciesId { get; set; }
        public virtual Species Species { get; set; } = null!;

        [Required]
        public BreedType BreedType { get; set; }

        [ForeignKey(nameof(Breed))]
        public int? BreedId { get; set; }
        public Breed? Breed { get; set; }

        [Required]
        [StringLength(MaxAnimalDescriptionLength, MinimumLength = MinAnimalDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public bool IsAdopted { get; set; }

        [Required]
        [ForeignKey(nameof(Shelter))]
        public int ShelterId { get; set; }

        public virtual Shelter Shelter { get; set; } = null!;
    }
}