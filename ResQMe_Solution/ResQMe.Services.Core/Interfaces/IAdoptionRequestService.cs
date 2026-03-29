namespace ResQMe.Services.Core.Interfaces
{
    using ResQMe.Data.Models.Enums;
    using ResQMe.ViewModels.AdoptionRequest;
    using ResQMe.ViewModels.Common;

    public interface IAdoptionRequestService
    {
        /* User methods */
        Task CreateAdoptionRequestAsync(string userId, AdoptionRequestFormViewModel model);

        Task<bool> HasUserAlreadyRequestedAsync(string userId, int animalId);

        Task<IEnumerable<MyAdoptionRequestsViewModel>> GetMyRequestsAsync(string userId);

        /* Admin methods */
        Task<PaginatedResultViewModel<AdoptionRequestAdminListViewModel>> GetAllRequestsForAdminAsync(
            AdoptionRequestStatus? status,
            int page,
            int pageSize);

        Task<AdoptionRequestAdminDetailsViewModel?> GetRequestByIdForAdminAsync(int id);

        Task ApproveRequestAsync(int id);

        Task RejectRequestAsync(int id);

        Task UndoApprovalAsync(int id);

        Task UndoRejectionAsync(int id);
    }
}