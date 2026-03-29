namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Breed;
    using ResQMe.ViewModels.Common;

    public interface IBreedService
    {
        Task<PaginatedResultViewModel<BreedListViewModel>> GetAllBreedsAsync(
            string? searchTerm,
            List<int> speciesIds,
            int page,
            int pageSize);

        Task<BreedFormViewModel?> GetBreedForEditAsync(int id);

        Task AddBreedAsync(BreedFormViewModel model);

        Task EditBreedAsync(BreedFormViewModel model);

        Task<bool> DeleteBreedAsync(int id);

        /* Creating dropdown support */
        Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync();
    }
}