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

        public async Task<PaginatedResultViewModel<AnimalListViewModel>> GetAllAnimalsAsync(
           string? searchTerm,
           List<int> speciesIds,
           List<Gender> genders,
           List<BreedType> breedTypes,
           List<string> cities,
           List<string> ageRanges,
           bool? showAdopted,
           int page,
           int pageSize)
        {
            var query = context.Animals
                .Include(a => a.Shelter)
                .Include(a => a.Breed)
                .Include(a => a.Species)
                .AsQueryable();

            /* Name or breed search */
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(a =>
                    a.Name.ToLower().Contains(term) ||
                    (a.Breed != null && a.Breed.Name.ToLower().Contains(term)));
            }

            /* Species filter */
            if (speciesIds.Any())
            {
                query = query.Where(a => speciesIds.Contains(a.SpeciesId));
            }

            /* Gender filter */
            if (genders.Any())
            {
                query = query.Where(a => genders.Contains(a.Gender));
            }

            /* Breed type filter */
            if (breedTypes.Any())
            {
                query = query.Where(a => breedTypes.Contains(a.BreedType));
            }

            /* City filter */
            if (cities.Any())
            {
                query = query.Where(a => cities.Contains(a.Shelter.City));
            }

            /* Age range filter */
            if (ageRanges.Any())
            {
                query = query.Where(a =>
                    (ageRanges.Contains("under 1") && a.Age < 1) ||
                    (ageRanges.Contains("1-3") && a.Age >= 1 && a.Age <= 3) ||
                    (ageRanges.Contains("4-6") && a.Age >= 4 && a.Age <= 6) ||
                    (ageRanges.Contains("7-9") && a.Age >= 7 && a.Age <= 9) ||
                    (ageRanges.Contains("10-12") && a.Age >= 10 && a.Age <= 12) ||
                    (ageRanges.Contains("13-15") && a.Age >= 13 && a.Age <= 15) ||
                    (ageRanges.Contains("over 15") && a.Age > 15));
            }

            /* Adopted filter */
            if (showAdopted.HasValue)
            {
                query = query.Where(a => a.IsAdopted == showAdopted.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderBy(a => a.IsAdopted)
                .ThenBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new PaginatedResultViewModel<AnimalListViewModel>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };
        }

        /* Unique Cities for Filter */
        public async Task<IEnumerable<string>> GetUniqueCitiesAsync()
        {
            return await context.Shelters
                .Select(s => s.City)
                .Distinct()
                .OrderBy(c => c)
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
                Age = model.Age!.Value,
                Gender = model.Gender!.Value,
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
            {
                return;
            }

            animal.Name = model.Name;
            animal.Age = model.Age!.Value;
            animal.Gender = model.Gender!.Value;
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

        public async Task<bool> DeleteAnimalAsync(int id)
        {
            var animal = await context.Animals.FindAsync(id);

            if (animal == null)
            {
                return false;
            }

            bool hasRequests = await context.AdoptionRequests
                .AnyAsync(ar => ar.AnimalId == id);

            if (hasRequests)
            {
                return false;
            }

            context.Animals.Remove(animal);
            await context.SaveChangesAsync();

            return true;
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