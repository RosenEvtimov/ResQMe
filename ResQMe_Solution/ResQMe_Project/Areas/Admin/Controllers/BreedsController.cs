namespace ResQMe_Project.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Breed;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BreedsController : Controller
    {
        private readonly IBreedService breedService;

        public BreedsController(IBreedService breedService)
        {
            this.breedService = breedService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            List<int> selectedSpeciesIds,
            int page = 1)
        {
            const int pageSize = 10;

            var model = await breedService.GetAllBreedsAsync(
                searchTerm,
                selectedSpeciesIds,
                page,
                pageSize);

            ViewBag.AvailableSpecies = await breedService.GetSpeciesForDropdownAsync();
            ViewBag.SelectedSpeciesIds = selectedSpeciesIds;
            ViewBag.SearchTerm = searchTerm;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add(string? returnUrl)
        {
            var model = new BreedFormViewModel
            {
                Species = await breedService.GetSpeciesForDropdownAsync(),
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BreedFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }

            try
            {
                await breedService.AddBreedAsync(model);

                TempData["AdminSuccess"] = "Breed added successfully!";

                return Redirect("/Admin/Breeds/Index" + model.ReturnUrl);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string? returnUrl)
        {
            var model = await breedService.GetBreedForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.Species = await breedService.GetSpeciesForDropdownAsync();
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BreedFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }

            try
            {
                await breedService.EditBreedAsync(model);

                TempData["AdminSuccess"] = "Breed edited successfully!";

                return Redirect("/Admin/Breeds/Index" + model.ReturnUrl);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string? returnUrl)
        {
            var model = await breedService.GetBreedForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            bool deleted = await breedService.DeleteBreedAsync(id);

            if (!deleted)
            {
                var model = await breedService.GetBreedForEditAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                model.ReturnUrl = returnUrl;

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this breed, because there are animals assigned to it.");

                return View("Delete", model);
            }

            TempData["AdminSuccess"] = "Breed deleted successfully!";

            return Redirect("/Admin/Breeds/Index" + returnUrl);
        }
    }
}