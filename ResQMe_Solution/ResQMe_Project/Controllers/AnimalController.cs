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
            var animals = await animalService.GetAllAnimalsAsync();

            var model = animals.Select(a => new AnimalListViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Age = a.Age,
                Gender = a.Gender.ToString(),
                // Display breed name if purebred, otherwise display breed type
                BreedDisplay = a.BreedType == BreedType.Purebred && a.Breed != null
                    ? a.Breed.Name
                    : a.BreedType.ToString(),
                Species = a.Species.Name,
                Shelter = a.Shelter.Name,
                IsAdopted = a.IsAdopted,
                ImageUrl = a.ImageUrl
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var animal = await animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            var model = new AnimalDetailsViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Age = animal.Age,
                Gender = animal.Gender,
                Description = animal.Description,
                ImageUrl = animal.ImageUrl,
                IsAdopted = animal.IsAdopted,
                // Display breed name if purebred, otherwise display breed type
                BreedDisplay = animal.BreedType == BreedType.Purebred && animal.Breed != null
                    ? animal.Breed.Name
                    : animal.BreedType.ToString(),
                Species = animal.Species.Name,

                ShelterName = animal.Shelter.Name,
                ShelterCity = animal.Shelter.City,
                ShelterAddress = animal.Shelter.Address,
                ShelterPhone = animal.Shelter.Phone,
                ShelterEmail = animal.Shelter.Email
            };

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

            var animal = new Animal
            {
                Name = model.Name,
                Age = model.Age,
                Gender = model.Gender,
                SpeciesId = model.SpeciesId!.Value,
                BreedType = model.BreedType!.Value,
                BreedId = model.BreedType == BreedType.Purebred
                    ? model.BreedId
                    : null,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                ShelterId = model.ShelterId!.Value,
                IsAdopted = false
            };

            await animalService.AddAnimalAsync(animal);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            var model = new AnimalFormViewModel
            {
                Name = animal.Name,
                Age = animal.Age,
                Gender = animal.Gender,
                SpeciesId = animal.SpeciesId,
                BreedType = animal.BreedType,
                BreedId = animal.BreedId,
                Description = animal.Description,
                ImageUrl = animal.ImageUrl,
                IsAdopted = animal.IsAdopted,
                ShelterId = animal.ShelterId
            };

            model.Species = await animalService.GetSpeciesForDropdownAsync();
            model.Shelters = await animalService.GetSheltersForDropdownAsync();

            if (model.SpeciesId.HasValue)
            {
                model.Breeds = await animalService.GetBreedsBySpeciesAsync(model.SpeciesId.Value);
            }

            return View(model);
        }

        [HttpPost]
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

            var animal = await animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            animal.Name = model.Name;
            animal.Age = model.Age;
            animal.Gender = model.Gender;
            animal.SpeciesId = model.SpeciesId!.Value;
            animal.BreedType = model.BreedType!.Value;
            animal.BreedId = model.BreedType == BreedType.Purebred
                ? model.BreedId
                : null;
            animal.Description = model.Description;
            animal.ImageUrl = model.ImageUrl;
            animal.IsAdopted = model.IsAdopted;
            animal.ShelterId = model.ShelterId!.Value;

            await animalService.EditAnimalAsync(animal);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            var model = new AnimalDetailsViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Age = animal.Age,
                Gender = animal.Gender,
                BreedDisplay = animal.BreedType == BreedType.Purebred && animal.Breed != null
                    ? animal.Breed.Name
                    : animal.BreedType.ToString(),
                Species = animal.Species.Name,
                ShelterName = animal.Shelter.Name,
                IsAdopted = animal.IsAdopted,
                ImageUrl = animal.ImageUrl
            };

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