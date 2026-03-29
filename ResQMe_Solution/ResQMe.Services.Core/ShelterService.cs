namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Common;
    using ResQMe.ViewModels.Shelter;

    public class ShelterService : IShelterService
    {
        private readonly ResQMeDbContext context;

        public ShelterService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<PaginatedResultViewModel<ShelterListViewModel>> GetAllSheltersAsync(
            string? searchTerm,
            List<string> cities,
            int page,
            int pageSize)
        {
            var query = context.Shelters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(term));
            }

            if (cities.Any())
            {
                query = query.Where(s => cities.Contains(s.City));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderBy(s => s.City)
                .ThenBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShelterListViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    ImageUrl = s.ImageUrl
                })
                .ToListAsync();

            return new PaginatedResultViewModel<ShelterListViewModel>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };
        }

        public async Task<IEnumerable<string>> GetUniqueCitiesAsync()
        {
            return await context.Shelters
                .Select(s => s.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id)
        {
            return await context.Shelters
                .Where(s => s.Id == id)
                .Select(s => new ShelterDetailsViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Address = s.Address,
                    Phone = s.Phone,
                    Email = s.Email,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,

                    Animals = s.Animals
                        .Where(a => !a.IsAdopted)
                        .Select(a => new ShelterAnimalsViewModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Age = a.Age,
                            Gender = a.Gender,
                            ImageUrl = a.ImageUrl
                        })
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ShelterFormViewModel?> GetShelterForEditAsync(int id)
        {
            return await context.Shelters
                .Where(s => s.Id == id)
                .Select(s => new ShelterFormViewModel
                {
                    Id= s.Id,
                    Name = s.Name,
                    City = s.City,
                    Address = s.Address,
                    Phone = s.Phone,
                    Email = s.Email,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddShelterAsync(ShelterFormViewModel model)
        {
            var shelter = new Shelter
            {
                Name = model.Name,
                City = model.City,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Description = model.Description,
                ImageUrl = model.ImageUrl
            };

            await context.Shelters.AddAsync(shelter);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException($"A shelter with the name '{model.Name}' at '{model.Address}' already exists.");
            }
        }

        public async Task EditShelterAsync(ShelterFormViewModel model)
        {
            var shelter = await context.Shelters.FindAsync(model.Id);

            if (shelter == null)
            {
                return;
            }

            shelter.Name = model.Name;
            shelter.City = model.City;
            shelter.Address = model.Address;
            shelter.Phone = model.Phone;
            shelter.Email = model.Email;
            shelter.Description = model.Description;
            shelter.ImageUrl = model.ImageUrl;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException($"A shelter with the name '{model.Name}' at '{model.Address}' already exists.");
            }
        }

        public async Task<bool> DeleteShelterAsync(int id)
        {
            var shelter = await context.Shelters.FindAsync(id);

            if (shelter == null)
            {
                return false;
            }

            bool hasAnimals = await context.Animals
                .AnyAsync(a => a.ShelterId == id);

            if (hasAnimals)
            {
                return false;
            }

            context.Shelters.Remove(shelter);
            await context.SaveChangesAsync();

            return true;
        }
    }
}