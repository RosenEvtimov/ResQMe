namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public class AnimalService : IAnimalService
    {
        private readonly ResQMeDbContext context;

        public AnimalService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<AnimalListViewModel>> GetAllAnimalsAsync()
        {
            return await context.Animals
                .Include(a => a.Shelter)
                .Include(a => a.Breed)
                .Include(a => a.Species)
                .Select(a => new AnimalListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Age = a.Age,
                    Gender = a.Gender,
                    BreedDisplay = a.BreedType == BreedType.Purebred && a.Breed != null
                        ? a.Breed.Name
                        : a.BreedType.ToString(),
                    Species = a.Species.Name,
                    Shelter = a.Shelter.Name,
                    IsAdopted = a.IsAdopted,
                    ImageUrl = a.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<AnimalDetailsViewModel?> GetAnimalDetailsAsync(int id)
        {
            return await context.Animals
                .Include(a => a.Shelter)
                .Include(a => a.Breed)
                .Include(a => a.Species)
                .Where(a => a.Id == id)
                .Select(a => new AnimalDetailsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Age = a.Age,
                    Gender = a.Gender,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    IsAdopted = a.IsAdopted,
                    BreedDisplay = a.BreedType == BreedType.Purebred && a.Breed != null
                        ? a.Breed.Name
                        : a.BreedType.ToString(),
                    Species = a.Species.Name,
                    ShelterName = a.Shelter.Name,
                    ShelterCity = a.Shelter.City,
                    ShelterAddress = a.Shelter.Address,
                    ShelterPhone = a.Shelter.Phone,
                    ShelterEmail = a.Shelter.Email
                })
                .FirstOrDefaultAsync();

        }

        public async Task<AnimalFormViewModel?> GetAnimalForEditAsync(int id)
        {
            return await context.Animals
                .Where(a => a.Id == id)
                .Select(a => new AnimalFormViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Age = a.Age,
                    Gender = a.Gender,
                    SpeciesId = a.SpeciesId,
                    BreedType = a.BreedType,
                    BreedId = a.BreedId,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    IsAdopted = a.IsAdopted,
                    ShelterId = a.ShelterId
                })
                .FirstOrDefaultAsync();
        }


        public async Task AddAnimalAsync(AnimalFormViewModel model)
        {
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

            await context.Animals.AddAsync(animal);
            await context.SaveChangesAsync();
        }

        public async Task EditAnimalAsync(AnimalFormViewModel model)
        {
            var animal = await context.Animals.FindAsync(model.Id);

            if (animal == null)
                return;

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

            await context.SaveChangesAsync();
        }

        public async Task DeleteAnimalAsync(AnimalDetailsViewModel model)
        {
            var animal = await context.Animals.FindAsync(model.Id);

            if (animal != null)
            {
                context.Animals.Remove(animal);
                await context.SaveChangesAsync();
            }
        }

        /* Species Dropdown */
        public async Task<IEnumerable<DropdownItemViewModel>> GetSpeciesForDropdownAsync()
        {
            return await context.Species
                .Select(s => new DropdownItemViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        /* Breeds Dropdown */
        public async Task<IEnumerable<DropdownItemViewModel>> GetBreedsBySpeciesAsync(int speciesId)    
        {
            return await context.Breeds
                .Where(b => b.SpeciesId == speciesId)
                .Select(b => new DropdownItemViewModel
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }


        /* Shelters Dropdown */
        public async Task<IEnumerable<DropdownItemViewModel>> GetSheltersForDropdownAsync()
        {
            return await context.Shelters
                .Select(s => new DropdownItemViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

    }
}