namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Common;

    public class AnimalService : IAnimalService
    {
        private readonly ResQMeDbContext context;

        public AnimalService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await context.Animals
                .Include(a => a.Shelter)
                .Include(a => a.Breed)
                .Include(a => a.Species)
                .ToListAsync();
        }

        public async Task<Animal?> GetAnimalByIdAsync(int id)
        {
            return await context.Animals
                .Include(a => a.Shelter)
                .Include(a => a.Breed)
                .Include(a => a.Species)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAnimalAsync(Animal animal)
        {
            await context.Animals.AddAsync(animal);
            await context.SaveChangesAsync();
        }

        public async Task EditAnimalAsync(Animal animal)
        {
            context.Animals.Update(animal);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAnimalAsync(int id)
        {
            var animal = await context.Animals.FindAsync(id);

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