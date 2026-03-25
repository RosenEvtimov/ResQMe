namespace ResQMe_Project.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnimalController : Controller
    {
        private readonly IAnimalService animalService;

        public AnimalController(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        [HttpGet]
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
        public async Task<IActionResult> GetBreeds(int speciesId)
        {
            var breeds = await animalService.GetBreedsBySpeciesAsync(speciesId);
            return Json(breeds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AnimalFormViewModel model)
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

            await animalService.AddAnimalAsync(model);

            return RedirectToAction("Index", "Animal", new { area = "" });
        }

        [HttpGet]
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

            return RedirectToAction("Index", "Animal", new { area = "" });
        }

        [HttpGet]
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

            return RedirectToAction("Index", "Animal", new { area = "" });
        }
    }
}