namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.Data.Models.Enums;
    using ResQMe.ViewModels.AdoptionRequest;

    public interface IAdoptionRequestService
    {
        /* User methods */
        Task CreateAdoptionRequestAsync(string userId, AdoptionRequestFormViewModel model);

        Task<bool> HasUserAlreadyRequestedAsync(string userId, int animalId);

        Task<IEnumerable<MyAdoptionRequestsViewModel>> GetMyRequestsAsync(string userId);

        /* Admin methods */
        Task<IEnumerable<AdoptionRequestAdminListViewModel>> GetAllRequestsForAdminAsync(AdoptionRequestStatus? status);

        Task<AdoptionRequestAdminDetailsViewModel?> GetRequestByIdForAdminAsync(int id);

        Task ApproveRequestAsync(int id);

        Task RejectRequestAsync(int id);

        Task UndoApprovalAsync(int id);

        Task UndoRejectionAsync(int id);
    }
}