namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.ViewModels.AdoptionRequest;

    public interface IAdoptionRequestService
    {
        Task CreateAdoptionRequestAsync(string userId, AdoptionRequestFormViewModel model);

        Task<bool> HasUserAlreadyRequestedAsync(string userId, int animalId);

        Task<IEnumerable<AdoptionRequestAdminListViewModel>> GetAllRequestsForAdminAsync();

        Task<AdoptionRequestAdminDetailsViewModel?> GetRequestByIdForAdminAsync(int id);

        Task ApproveRequestAsync(int id);

        Task RejectRequestAsync(int id);
    }
}