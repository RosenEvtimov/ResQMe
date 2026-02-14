namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Breed;
    using ResQMe.ViewModels.Common;

    public interface IBreedService
    {
        Task<IEnumerable<BreedListViewModel>> GetAllBreedsAsync();

        Task<BreedFormViewModel?> GetBreedForEditAsync(int id);

        Task AddBreedAsync(BreedFormViewModel model);

        Task EditBreedAsync(BreedFormViewModel model);

        Task<bool> DeleteBreedAsync(int id);

        /* Creating dropdown support */
        Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync();
    }
}