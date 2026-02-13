namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public class AnimalController : Controller
    {
        private readonly IAnimalService animalService;

        public AnimalController(IAnimalService animalService)
        {
            this.animalService = animalService;
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

            return View(model);
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
        public async Task<IActionResult> Edit(int id, AnimalFormViewModel model)
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

            await animalService.EditAnimalAsync(id, model);

            return RedirectToAction(nameof(Index));
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
            await animalService.DeleteAnimalAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}