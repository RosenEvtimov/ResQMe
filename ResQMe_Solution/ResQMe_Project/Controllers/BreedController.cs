namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Breed;

    [Authorize(Roles = "Admin")]
    public class BreedController : Controller
    {
        private readonly IBreedService breedService;

        public BreedController(IBreedService breedService)
        {
            this.breedService = breedService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await breedService.GetAllBreedsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new BreedFormViewModel
            {
                Species = await breedService.GetSpeciesForDropdownAsync()
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
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await breedService.GetBreedForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.Species = await breedService.GetSpeciesForDropdownAsync();

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
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Species = await breedService.GetSpeciesForDropdownAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await breedService.GetBreedForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await breedService.DeleteBreedAsync(id);

            if (!deleted)
            {
                var model = await breedService.GetBreedForEditAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this breed, because there are animals assigned to it.");

                return View("Delete", model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}