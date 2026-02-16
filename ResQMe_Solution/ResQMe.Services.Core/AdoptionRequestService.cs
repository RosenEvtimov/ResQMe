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

        /* User methods */
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
                PreviousAdoptionExperience = model.PreviousAdoptionExperience!.Value,
                Message = model.Message
            };

            await context.AdoptionRequests.AddAsync(request);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("You have already sent an adoption request for this animal.");
            }
        }

        public async Task<bool> HasUserAlreadyRequestedAsync(string userId, int animalId)
        {
            return await context.AdoptionRequests
                .AnyAsync(ar => ar.UserId == userId && ar.AnimalId == animalId);
        }

        public async Task<IEnumerable<MyAdoptionRequestsViewModel>> GetMyRequestsAsync(string userId)
        {
            return await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .Where(ar => ar.UserId == userId)
                .OrderByDescending(ar => ar.CreatedOn)
                .Select(ar => new MyAdoptionRequestsViewModel
                {
                    Id = ar.Id,
                    AnimalName = ar.Animal.Name,
                    AnimalImageUrl = ar.Animal.ImageUrl,
                    CreatedOn = ar.CreatedOn,
                    Status = ar.Status
                })
                .ToListAsync();
        }

        /* Admin methods */
        public async Task<IEnumerable<AdoptionRequestAdminListViewModel>> GetAllRequestsForAdminAsync(AdoptionRequestStatus? status)
        {
            var query = context.AdoptionRequests
                .Include(ar => ar.Animal)
                .Include(ar => ar.User)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(ar => ar.Status == status.Value);
            }

            return await query
                .OrderBy(ar => ar.CreatedOn)
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

            if (request.Animal.IsAdopted)
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

        public async Task UndoApprovalAsync(int id)
        {
            var request = await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (request == null)
            {
                return;
            }

            /* Only allow undo if the request is currently approved */
            if (request.Status != AdoptionRequestStatus.Approved)
            {
                return;
            }

            /* Set this request back to pending */
            request.Status = AdoptionRequestStatus.Pending;

            /* Make the animal available again */
            request.Animal.IsAdopted = false;

            /* Reopen all rejected requests for the same animal */
            var rejectedRequests = await context.AdoptionRequests
                .Where(ar => ar.AnimalId == request.AnimalId
                             && ar.Id != id
                             && ar.Status == AdoptionRequestStatus.Rejected)
                .ToListAsync();

            foreach (var rejectedRequest in rejectedRequests)
            {
                rejectedRequest.Status = AdoptionRequestStatus.Pending;
            }

            await context.SaveChangesAsync();
        }

        public async Task UndoRejectionAsync(int id)
        {
            var request = await context.AdoptionRequests
                .Include(ar => ar.Animal)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (request == null)
            {
                return;
            }

            if (request.Status != AdoptionRequestStatus.Rejected)
            {
                return;
            }

            request.Status = AdoptionRequestStatus.Pending;

            await context.SaveChangesAsync();
        }
    }
}