namespace ResQMe.ViewModels.Species
{
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class SpeciesFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxSpeciesNameLength, MinimumLength = MinSpeciesNameLength)]
        public string Name { get; set; } = null!;
    }
}
