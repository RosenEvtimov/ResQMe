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

        public async Task<PaginatedResultViewModel<BreedListViewModel>> GetAllBreedsAsync(
            string? searchTerm,
            List<int> speciesIds,
            int page,
            int pageSize)
        {
            var query = context.Breeds
                .Include(b => b.Species)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(term));
            }

            if (speciesIds.Any())
            {
                query = query.Where(b => speciesIds.Contains(b.SpeciesId));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderBy(b => b.Species.Name)
                .ThenBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BreedListViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    SpeciesName = b.Species.Name
                })
                .ToListAsync();

            return new PaginatedResultViewModel<BreedListViewModel>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };
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

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                var speciesName = await context.Species
                    .Where(s => s.Id == model.SpeciesId!.Value)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                throw new InvalidOperationException($"A breed with the name '{model.Name}' already exists for {speciesName}.");
            }
        }

        public async Task EditBreedAsync(BreedFormViewModel model)
        {
            var breed = await context.Breeds.FindAsync(model.Id);

            if (breed == null)
            {
                return;
            }

            breed.Name = model.Name;
            breed.SpeciesId = model.SpeciesId!.Value;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                var speciesName = await context.Species
                    .Where(s => s.Id == model.SpeciesId!.Value)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                throw new InvalidOperationException($"A breed with the name '{model.Name}' already exists for {speciesName}.");
            }
        }

        public async Task<bool> DeleteBreedAsync(int id)
        {
            var breed = await context.Breeds.FindAsync(id);

            if (breed == null)
            {
                return false;
            }

            bool hasAnimals = await context.Animals
                .AnyAsync(a => a.BreedId == id);

            if (hasAnimals)
            {
                return false;
            }

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