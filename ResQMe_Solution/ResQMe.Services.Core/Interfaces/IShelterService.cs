namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Common;
    using ResQMe.ViewModels.Shelter;

    public interface IShelterService
    {
        Task<PaginatedResultViewModel<ShelterListViewModel>> GetAllSheltersAsync(
            string? searchTerm,
            List<string> cities,
            int page,
            int pageSize);

        Task<IEnumerable<string>> GetUniqueCitiesAsync();

        Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id);

        Task<ShelterFormViewModel?> GetShelterForEditAsync(int id);

        Task AddShelterAsync(ShelterFormViewModel model);

        Task EditShelterAsync(ShelterFormViewModel model);

        Task<bool> DeleteShelterAsync(int id);
    }
}