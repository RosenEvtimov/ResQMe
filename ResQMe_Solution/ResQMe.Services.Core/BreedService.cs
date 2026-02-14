namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Breed;
    using ResQMe.ViewModels.Common;

    public class BreedService : IBreedService
    {
        private readonly ResQMeDbContext context;

        public BreedService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<BreedListViewModel>> GetAllBreedsAsync()
        {
            return await context.Breeds
                .Include(b => b.Species)
                .Select(b => new BreedListViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    SpeciesName = b.Species.Name
                })
                .ToListAsync();
        }

        public async Task<BreedFormViewModel?> GetBreedForEditAsync(int id)
        {
            return await context.Breeds
                .Where(b => b.Id == id)
                .Select(b => new BreedFormViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    SpeciesId = b.SpeciesId
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddBreedAsync(BreedFormViewModel model)
        {
            var breed = new Breed
            {
                Name = model.Name,
                SpeciesId = model.SpeciesId!.Value
            };

            await context.Breeds.AddAsync(breed);
            await context.SaveChangesAsync();
        }

        public async Task EditBreedAsync(BreedFormViewModel model)
        {
            var breed = await context.Breeds.FindAsync(model.Id);

            if (breed == null)
                return;

            breed.Name = model.Name;
            breed.SpeciesId = model.SpeciesId!.Value;

            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBreedAsync(int id)
        {
            var breed = await context.Breeds.FindAsync(id);

            if (breed == null)
                return false;

            bool hasAnimals = await context.Animals
                .AnyAsync(a => a.BreedId == id);

            if (hasAnimals)
                return false;

            context.Breeds.Remove(breed);
            await context.SaveChangesAsync();

            return true;
        }

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
    }
}