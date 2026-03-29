namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Common;
    using ResQMe.ViewModels.Species;

    public interface ISpeciesService
    {
        Task<PaginatedResultViewModel<SpeciesListViewModel>> GetAllSpeciesAsync(
            string? searchTerm,
            int page,
            int pageSize);

        Task<SpeciesFormViewModel?> GetSpeciesForEditAsync(int id);

        Task AddSpeciesAsync(SpeciesFormViewModel model);

        Task EditSpeciesAsync(SpeciesFormViewModel model);

        Task<bool> DeleteSpeciesAsync(int id);
    }
}