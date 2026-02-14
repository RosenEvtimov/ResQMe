namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.Shelter;

    public interface IShelterService
    {
        Task<IEnumerable<ShelterListViewModel>> GetAllSheltersAsync();

        Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id);

        Task<ShelterFormViewModel?> GetShelterForEditAsync(int id);

        Task AddShelterAsync(ShelterFormViewModel model);

        Task EditShelterAsync(ShelterFormViewModel model);

        Task<bool> DeleteShelterAsync(int id);
    }
}