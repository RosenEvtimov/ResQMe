namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.AdoptionRequest;

    [Authorize(Roles = "User")]
    public class AdoptionRequestController : Controller
    {
        private readonly IAdoptionRequestService adoptionRequestService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ResQMeDbContext context;

        public AdoptionRequestController(IAdoptionRequestService adoptionRequestService, UserManager<ApplicationUser> userManager, ResQMeDbContext context)
        {
            this.adoptionRequestService = adoptionRequestService;
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int animalId)
        {
            if (animalId <= 0)
            {
                return BadRequest();
            }

            var animalExists = await context.Animals
                .AnyAsync(a => a.Id == animalId && !a.IsAdopted);

            if (!animalExists)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            /* Ensure the user has completed their profile before allowing them to send an adoption request. */
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Address) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                TempData["ProfileRequired"] = "Please complete your profile before sending an adoption request.";

                return RedirectToAction("Edit", "Profile", new { returnAnimalId = animalId});
            }

            var model = new AdoptionRequestFormViewModel
            {
                AnimalId = animalId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdoptionRequestFormViewModel model)
        {
            if (model.AnimalId <= 0)
            {
                return BadRequest();
            }

            var animalExists = await context.Animals
                .AnyAsync(a => a.Id == model.AnimalId && !a.IsAdopted);

            if (!animalExists)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                await adoptionRequestService.CreateAdoptionRequestAsync(user.Id, model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Animal", new { id = model.AnimalId });
            }

            TempData["Success"] = "Adoption request sent successfully.";

            return RedirectToAction("Details", "Animal", new { id = model.AnimalId });
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = await adoptionRequestService.GetMyRequestsAsync(user.Id);

            return View(model);
        }
    }
}