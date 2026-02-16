namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.AdoptionRequest;

    public class AdoptionRequestService : IAdoptionRequestService
    {
        private readonly ResQMeDbContext context;

        public AdoptionRequestService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAdoptionRequestAsync(string userId, AdoptionRequestFormViewModel model)
        {
            /* Check if the user already applied for the current animal */
            bool alreadyRequested = await context.AdoptionRequests
                .AnyAsync(ar => ar.UserId == userId && ar.AnimalId == model.AnimalId);

            if (alreadyRequested)
            {
                throw new InvalidOperationException("You have already sent an adoption request for this animal.");
            }

            var request = new AdoptionRequest
            {
                AnimalId = model.AnimalId,
                UserId = userId,
                PreviousAdoptionExperience = model.PreviousAdoptionExperience,
                Message = model.Message
            };

            await context.AdoptionRequests.AddAsync(request);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasUserAlreadyRequestedAsync(string userId, int animalId)
        {
            return await context.AdoptionRequests
                .AnyAsync(ar => ar.UserId == userId && ar.AnimalId == animalId);
        }

        public async Task<IEnumerable<AdoptionRequestAdminListViewModel>> GetAllRequestsForAdminAsync()
        {
            return await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .Include(ar => ar.User)
                .Select(ar => new AdoptionRequestAdminListViewModel
                {
                    Id = ar.Id,
                    AnimalName = ar.Animal.Name,
                    ApplicantFullName = ar.User.FirstName + " " + ar.User.LastName,
                    City = ar.User.City!,
                    CreatedOn = ar.CreatedOn,
                    Status = ar.Status
                })
                .ToListAsync();
        }

        public async Task<AdoptionRequestAdminDetailsViewModel?> GetRequestByIdForAdminAsync(int id)
        {
            return await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .Include(ar => ar.User)
                .Where(ar => ar.Id == id)
                .Select(ar => new AdoptionRequestAdminDetailsViewModel
                {
                    Id = ar.Id,
                    AnimalName = ar.Animal.Name,
                    PreviousAdoptionExperience = ar.PreviousAdoptionExperience,
                    Message = ar.Message,
                    CreatedOn = ar.CreatedOn,
                    Status = ar.Status,

                    FirstName = ar.User.FirstName!,
                    LastName = ar.User.LastName!,
                    Email = ar.User.Email!,
                    PhoneNumber = ar.User.PhoneNumber!,
                    Address = ar.User.Address!,
                    City = ar.User.City!
                })
                .FirstOrDefaultAsync();
        }

        public async Task ApproveRequestAsync(int id)
        {
            var request = await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (request == null)
            {
                return;
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return;
            }

            /* Mark this request approved */
            request.Status = AdoptionRequestStatus.Approved;

            /* Mark the animal as adopted */
            request.Animal.IsAdopted = true;

            /* Reject all other pending requests for the same animal */
            var otherRequests = await context.AdoptionRequests
                .Where(ar => ar.AnimalId == request.AnimalId
                             && ar.Id != id
                             && ar.Status == AdoptionRequestStatus.Pending)
                .ToListAsync();

            foreach (var other in otherRequests)
            {
                other.Status = AdoptionRequestStatus.Rejected;
            }

            await context.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(int id)
        {
            var request = await context.AdoptionRequests
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (request == null)
            {
                return;
            }

            if (request.Status != AdoptionRequestStatus.Pending)
            {
                return;
            }

            request.Status = AdoptionRequestStatus.Rejected;

            await context.SaveChangesAsync();
        }
    }
}