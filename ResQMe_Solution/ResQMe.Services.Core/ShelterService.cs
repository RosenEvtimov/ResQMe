namespace ResQMe.Services.Core
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.ViewModels.Shelter;
    using ResQMe.Services.Core.Interfaces;
    public class ShelterService : IShelterService
    {
        private readonly ResQMeDbContext context;

        public ShelterService(ResQMeDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ShelterListViewModel>> GetAllSheltersAsync()
        {
            return await context.Shelters
                .Select(s => new ShelterListViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    ImageUrl = s.ImageUrl
                })
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
            await context.SaveChangesAsync();
        }

        public async Task EditShelterAsync(int id, ShelterFormViewModel model)
        {
            var shelter = await context.Shelters.FindAsync(id);

            if (shelter == null)
                return;

            shelter.Name = model.Name;
            shelter.City = model.City;
            shelter.Address = model.Address;
            shelter.Phone = model.Phone;
            shelter.Email = model.Email;
            shelter.Description = model.Description;
            shelter.ImageUrl = model.ImageUrl;

            await context.SaveChangesAsync();
        }

        public async Task DeleteShelterAsync(int id)
        {
            var shelter = await context.Shelters.FindAsync(id);

            if (shelter != null)
            {
                context.Shelters.Remove(shelter);
                await context.SaveChangesAsync();
            }
        }
    }
}