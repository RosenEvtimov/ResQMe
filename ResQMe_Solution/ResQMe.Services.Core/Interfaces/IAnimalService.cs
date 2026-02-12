namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.Data.Models;
    using ResQMe.ViewModels.Common;

    public interface IAnimalService
    {
        Task<IEnumerable<Animal>> GetAllAnimalsAsync();

        Task<Animal?> GetAnimalByIdAsync(int id);

        Task AddAnimalAsync(Animal model);

        Task EditAnimalAsync(Animal model);

        Task DeleteAnimalAsync(int id);

        /* Creating Dropdown Support */
        Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync();

        Task<IEnumerable<DropdownItemViewModel>> GetBreedsBySpeciesAsync(int speciesId);

        Task<IEnumerable<DropdownItemViewModel>> GetSheltersForDropdownAsync();

    }
}