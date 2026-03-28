namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.Data.Models.Enums;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public interface IAnimalService
    {
        Task<PaginatedResultViewModel<AnimalListViewModel>> GetAllAnimalsAsync(
            string? searchTerm,
            List<int> speciesIds,
            List<Gender> genders,
            List<BreedType> breedTypes,
            List<string> cities,
            List<string> ageRanges,
            bool? showAdopted,
            int page,
            int pageSize);

        Task<AnimalDetailsViewModel?> GetAnimalDetailsAsync(int id);

        Task<AnimalFormViewModel?> GetAnimalForEditAsync(int id);

        Task AddAnimalAsync(AnimalFormViewModel model);

        Task EditAnimalAsync(AnimalFormViewModel model);

        Task<bool> DeleteAnimalAsync(int id);

        /* Creating Dropdown Support */
        Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync();

        Task<IEnumerable<DropdownItemViewModel>> GetBreedsBySpeciesAsync(int speciesId);

        Task<IEnumerable<DropdownItemViewModel>> GetSheltersForDropdownAsync();

        /* Filter Support */
        Task<IEnumerable<string>> GetUniqueCitiesAsync();
    }
}