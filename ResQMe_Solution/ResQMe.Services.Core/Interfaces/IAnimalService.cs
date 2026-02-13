namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public interface IAnimalService
    {
        Task<IEnumerable<AnimalListViewModel>> GetAllAnimalsAsync();

        Task<AnimalDetailsViewModel?> GetAnimalDetailsAsync(int id);

        Task<AnimalFormViewModel?> GetAnimalForEditAsync(int id);

        Task AddAnimalAsync(AnimalFormViewModel model);

        Task EditAnimalAsync(AnimalFormViewModel model);

        Task DeleteAnimalAsync(AnimalDetailsViewModel model);

        /* Creating Dropdown Support */
        Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync();

        Task<IEnumerable<DropdownItemViewModel>> GetBreedsBySpeciesAsync(int speciesId);

        Task<IEnumerable<DropdownItemViewModel>> GetSheltersForDropdownAsync();

    }
}