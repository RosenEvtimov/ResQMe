namespace ResQMe.ViewModels.Animal
{
    using ResQMe.Data.Models.Enums;
    using ResQMe.ViewModels.Common;

    public class AnimalFilterViewModel
    {
        /* Paginated results */
        public PaginatedResultViewModel<AnimalListViewModel> Results { get; set; } = new();

        /* Filter options for rendering checkboxes */
        public IEnumerable<DropdownItemViewModel> AvailableSpecies { get; set; } = new List<DropdownItemViewModel>();
        public IEnumerable<string> AvailableCities { get; set; } = new List<string>();

        /* Currently selected filters */
        public string? SearchTerm { get; set; }
        public List<int> SelectedSpeciesIds { get; set; } = new();
        public List<Gender> SelectedGenders { get; set; } = new();
        public List<BreedType> SelectedBreedTypes { get; set; } = new();
        public List<string> SelectedCities { get; set; } = new();
        public List<string> SelectedAgeRanges { get; set; } = new();
        public bool? ShowAdopted { get; set; }
    }
}