namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Species;

    public interface ISpeciesService
    {
        Task<IEnumerable<SpeciesListViewModel>> GetAllSpeciesAsync();

        Task<SpeciesFormViewModel?> GetSpeciesForEditAsync(int id);

        Task AddSpeciesAsync(SpeciesFormViewModel model);

        Task EditSpeciesAsync(SpeciesFormViewModel model);

        Task<bool> DeleteSpeciesAsync(int id);
    }
}