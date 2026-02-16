namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Species;

    public class SpeciesService : ISpeciesService
    {
        private readonly ResQMeDbContext context;

        public SpeciesService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SpeciesListViewModel>> GetAllSpeciesAsync()
        {
            return await context.Species
                .OrderBy(s => s.Name)
                .Select(s => new SpeciesListViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        public async Task<SpeciesFormViewModel?> GetSpeciesForEditAsync(int id)
        {
            return await context.Species
                .Where(s => s.Id == id)
                .Select(s => new SpeciesFormViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddSpeciesAsync(SpeciesFormViewModel model)
        {
            var species = new Species
            {
                Name = model.Name
            };

            await context.Species.AddAsync(species);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException($"A species with the name '{model.Name}' already exists.");
            }
        }

        public async Task EditSpeciesAsync(SpeciesFormViewModel model)
        {
            var species = await context.Species.FindAsync(model.Id);

            if (species == null)
            {
                return;
            }

            species.Name = model.Name;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException($"A species with the name '{model.Name}' already exists.");
            }
        }

        public async Task<bool> DeleteSpeciesAsync(int id)
        {
            var species = await context.Species.FindAsync(id);

            if (species == null)
            {
                return false;
            }

            bool hasAnimals = await context.Animals
                .AnyAsync(a => a.SpeciesId == id);

            if (hasAnimals)
            {
                return false;
            }

            context.Species.Remove(species);
            await context.SaveChangesAsync();

            return true;
        }
    }
}