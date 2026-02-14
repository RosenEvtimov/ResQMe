namespace ResQMe.ViewModels.Breed
{
    using System.ComponentModel.DataAnnotations;
    using ResQMe.ViewModels.Common;
    using static ResQMe.GCommon.EntityValidation;

    public class BreedFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxBreedNameLength, MinimumLength = MinBreedNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        public int? SpeciesId { get; set; }

        public IEnumerable<DropdownItemViewModel> Species { get; set; }
            = new List<DropdownItemViewModel>();
    }
}