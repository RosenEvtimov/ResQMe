namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public class AnimalController : Controller
    {
        private readonly IAnimalService animalService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAdoptionRequestService adoptionRequestService;

        public AnimalController(IAnimalService animalService, UserManager<ApplicationUser> userManager, IAdoptionRequestService adoptionRequestService)
        {
            this.animalService = animalService;
            this.userManager = userManager;
            this.adoptionRequestService = adoptionRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await animalService.GetAllAnimalsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, int? fromShelterId)
        {
            var model = await animalService.GetAnimalDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.FromShelterId = fromShelterId;

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                var user = await userManager.GetUserAsync(User);

                if (user != null)
                {
                    bool hasAlreadyRequested =
                        await adoptionRequestService.HasUserAlreadyRequestedAsync(user.Id, id);

                    ViewBag.HasAlreadyRequested = hasAlreadyRequested;
                }
            }
            else
            {
                ViewBag.HasAlreadyRequested = false;
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new AnimalFormViewModel
            {
                Species = await animalService.GetSpeciesForDropdownAsync(),
                Breeds = new List<DropdownItemViewModel>(),
                Shelters = await animalService.GetSheltersForDropdownAsync()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBreeds(int speciesId)
        {
            var breeds = await animalService.GetBreedsBySpeciesAsync(speciesId);
            return Json(breeds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AnimalFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                /* Reloading dropdowns */
                model.Species = await animalService.GetSpeciesForDropdownAsync();
                model.Shelters = await animalService.GetSheltersForDropdownAsync();

                /* Only reload breeds if species was selected */
                if (model.SpeciesId.HasValue)
                {
                    model.Breeds = await animalService.GetBreedsBySpeciesAsync(model.SpeciesId.Value);
                }

                return View(model);
            }

            await animalService.AddAnimalAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await animalService.GetAnimalForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.Species = await animalService.GetSpeciesForDropdownAsync();
            model.Shelters = await animalService.GetSheltersForDropdownAsync();

            if (model.SpeciesId.HasValue)
            {
                model.Breeds = await animalService.GetBreedsBySpeciesAsync(model.SpeciesId.Value);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AnimalFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Species = await animalService.GetSpeciesForDropdownAsync();
                model.Shelters = await animalService.GetSheltersForDropdownAsync();

                if (model.SpeciesId.HasValue)
                {
                    model.Breeds = await animalService.GetBreedsBySpeciesAsync(model.SpeciesId.Value);
                }

                return View(model);
            }

            await animalService.EditAnimalAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await animalService.GetAnimalDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await animalService.DeleteAnimalAsync(id);

            if (!deleted)
            {
                var model = await animalService.GetAnimalDetailsAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this animal, because it either has adoption requests or doesn't exist.");

                return View("Delete", model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}