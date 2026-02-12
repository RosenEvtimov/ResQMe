namespace ResQMe.ViewModels.Animal
{
    using System.ComponentModel.DataAnnotations;
    using ResQMe.Data.Models.Enums;
    using ResQMe.ViewModels.Common;
    using static ResQMe.GCommon.EntityValidation;

    public class AnimalFormViewModel
    {
        [Required]
        [StringLength(MaxAnimalNameLength, MinimumLength = MinAnimalNameLength)]
        public string Name { get; set; } = null!;

        [Range(MinAnimalAge, MaxAnimalAge)]
        public int Age { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public int? SpeciesId { get; set; }

        [Required]
        public BreedType? BreedType { get; set; }

        public int? BreedId { get; set; }

        [Required]
        [StringLength(MaxAnimalDescriptionLength, MinimumLength = MinAnimalDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;

        public bool IsAdopted { get; set; }

        [Required]
        public int? ShelterId { get; set; }
            
        /* Dropdown collections */
        public IEnumerable<DropdownItemViewModel> Species { get; set; } = new List<DropdownItemViewModel>();

        public IEnumerable<DropdownItemViewModel> Breeds { get; set; } = new List<DropdownItemViewModel>();

        public IEnumerable<DropdownItemViewModel> Shelters { get; set; } = new List<DropdownItemViewModel>();
    }
}