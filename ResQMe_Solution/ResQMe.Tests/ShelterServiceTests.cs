namespace ResQMe.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core;
    using ResQMe.ViewModels.Common;
    using ResQMe.ViewModels.Shelter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class ShelterServiceTests
    {
        private DbContextOptions<ResQMeDbContext> CreateOptions(string dbName)
        {
            var uniqueName = $"{GetType().Name}_{dbName}";
            return new DbContextOptionsBuilder<ResQMeDbContext>()
                .UseInMemoryDatabase(databaseName: uniqueName)
                .Options;
        }

        private ResQMeDbContext CreateContext(string dbName)
            => new ResQMeDbContext(CreateOptions(dbName));

        [Test]
        public async Task GetAllSheltersAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering()
        {
            var dbName = nameof(GetAllSheltersAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.AddRange(
                    new Shelter { Id = 1, Name = "AlphaShelter", City = "CityB", Address = "A1", Description = "D1", ImageUrl = "http://a", Phone = "111", Email = "a@ex.com" },
                    new Shelter { Id = 2, Name = "BetaShelter", City = "CityA", Address = "A2", Description = "D2", ImageUrl = "http://b", Phone = "222", Email = "b@ex.com" },
                    new Shelter { Id = 3, Name = "GammaShelter", City = "CityA", Address = "A3", Description = "D3", ImageUrl = "http://c", Phone = "333", Email = "c@ex.com" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                // Search by name "BetaShelter"
                var search = await service.GetAllSheltersAsync(
                    searchTerm: "Beta",
                    cities: new List<string>(),
                    page: 1,
                    pageSize: 10);

                Assert.That(search.Items.Count(), Is.EqualTo(1));
                Assert.That(search.Items.First().Name, Is.EqualTo("BetaShelter"));

                // Filter by city CityA -> should return BetaShelter and GammaShelter ordered by Name
                var byCity = await service.GetAllSheltersAsync(
                    searchTerm: null,
                    cities: new List<string> { "CityA" },
                    page: 1,
                    pageSize: 10);

                Assert.That(byCity.Items.Count(), Is.EqualTo(2));
                var orderedNames = byCity.Items.Select(i => i.Name).ToList();
                Assert.That(orderedNames, Is.EqualTo(new List<string> { "BetaShelter", "GammaShelter" }));

                // Pagination: pageSize 1 for CityA should yield TotalPages = 2
                var paged = await service.GetAllSheltersAsync(
                    searchTerm: null,
                    cities: new List<string> { "CityA" },
                    page: 1,
                    pageSize: 1);

                Assert.That(paged.TotalPages, Is.EqualTo(2));
                Assert.That(paged.Items.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetUniqueCitiesAsync_Returns_Unique_Cities_Ordered()
        {
            var dbName = nameof(GetUniqueCitiesAsync_Returns_Unique_Cities_Ordered);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.AddRange(
                    new Shelter { Id = 10, Name = "S1", City = "ZCity", Address = "A1", Description = "D1", ImageUrl = "http://i1", Phone = "1", Email = "s1@ex.com" },
                    new Shelter { Id = 11, Name = "S2", City = "ACity", Address = "A2", Description = "D2", ImageUrl = "http://i2", Phone = "2", Email = "s2@ex.com" },
                    new Shelter { Id = 12, Name = "S3", City = "ZCity", Address = "A3", Description = "D3", ImageUrl = "http://i3", Phone = "3", Email = "s3@ex.com" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var cities = (await service.GetUniqueCitiesAsync()).ToList();

                Assert.That(cities.Count, Is.EqualTo(2));
                Assert.That(cities[0], Is.EqualTo("ACity"));
                Assert.That(cities[1], Is.EqualTo("ZCity"));
            }
        }

        [Test]
        public async Task GetShelterDetailsAsync_Returns_Correct_Details_With_Available_Animals()
        {
            var dbName = nameof(GetShelterDetailsAsync_Returns_Correct_Details_With_Available_Animals);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.Add(new Shelter { Id = 20, Name = "DetailShelter", City = "Town", Address = "Addr", Description = "Desc", ImageUrl = "http://img", Phone = "000", Email = "d@ex.com" });

                context.Animals.AddRange(
                    new Animal { Id = 100, Name = "Free", Age = 2, Gender = Gender.Male, SpeciesId = 1, BreedType = Data.Models.Enums.BreedType.Mixed, ShelterId = 20, Description = "d", ImageUrl = "u", IsAdopted = false },
                    new Animal { Id = 101, Name = "Adopted", Age = 3, Gender = Gender.Female, SpeciesId = 1, BreedType = Data.Models.Enums.BreedType.Mixed, ShelterId = 20, Description = "d", ImageUrl = "u", IsAdopted = true }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var model = await service.GetShelterDetailsAsync(20);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Name, Is.EqualTo("DetailShelter"));
                var animals = model.Animals.ToList();
                Assert.That(animals.Count, Is.EqualTo(1));
                Assert.That(animals[0].Name, Is.EqualTo("Free"));
            }
        }

        [Test]
        public async Task GetShelterForEditAsync_Returns_Correct_Shelter()
        {
            var dbName = nameof(GetShelterForEditAsync_Returns_Correct_Shelter);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.Add(new Shelter { Id = 30, Name = "EditShel", City = "C", Address = "Addr30", Description = "Desc30", ImageUrl = "http://i30", Phone = "30", Email = "e30@ex.com" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var model = await service.GetShelterForEditAsync(30);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(30));
                Assert.That(model.Name, Is.EqualTo("EditShel"));
                Assert.That(model.Address, Is.EqualTo("Addr30"));
            }
        }

        [Test]
        public async Task AddShelterAsync_Adds_New_Shelter()
        {
            var dbName = nameof(AddShelterAsync_Adds_New_Shelter);

            using (var context = CreateContext(dbName))
            {
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var form = new ShelterFormViewModel
                {
                    Name = "NewShel",
                    City = "NCity",
                    Address = "NAddr",
                    Phone = "999",
                    Email = "n@ex.com",
                    Description = "New",
                    ImageUrl = "http://new"
                };

                await service.AddShelterAsync(form);

                var shelter = await context.Shelters.FirstOrDefaultAsync(s => s.Name == "NewShel");
                Assert.That(shelter, Is.Not.Null);
                Assert.That(shelter!.City, Is.EqualTo("NCity"));
            }
        }

        [Test]
        public async Task EditShelterAsync_Updates_Existing_Shelter()
        {
            var dbName = nameof(EditShelterAsync_Updates_Existing_Shelter);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.Add(new Shelter { Id = 40, Name = "ToEdit", City = "OldCity", Address = "OldAddr", Description = "Old", ImageUrl = "http://old", Phone = "111", Email = "old@ex.com" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var form = new ShelterFormViewModel
                {
                    Id = 40,
                    Name = "Edited",
                    City = "NewCity",
                    Address = "NewAddr",
                    Phone = "222",
                    Email = "new@ex.com",
                    Description = "NewDesc",
                    ImageUrl = "http://new"
                };

                await service.EditShelterAsync(form);

                var shelter = await context.Shelters.FindAsync(40);
                Assert.That(shelter, Is.Not.Null);
                Assert.That(shelter!.Name, Is.EqualTo("Edited"));
                Assert.That(shelter.City, Is.EqualTo("NewCity"));
            }
        }

        [Test]
        public async Task DeleteShelterAsync_Returns_False_When_The_Shelter_Does_Not_Exist()
        {
            var dbName = nameof(DeleteShelterAsync_Returns_False_When_The_Shelter_Does_Not_Exist);

            using (var context = CreateContext(dbName))
            {
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var result = await service.DeleteShelterAsync(999);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public async Task DeleteShelterAsync_Returns_False_When_The_Shelter_Has_Animals()
        {
            var dbName = nameof(DeleteShelterAsync_Returns_False_When_The_Shelter_Has_Animals);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.Add(new Shelter { Id = 50, Name = "HasAnimals", City = "C", Address = "A", Description = "D", ImageUrl = "http://i", Phone = "p", Email = "e@ex.com" });
                context.Animals.Add(new Animal { Id = 201, Name = "A1", Age = 1, Gender = Gender.Male, SpeciesId = 1, BreedType = Data.Models.Enums.BreedType.Mixed, ShelterId = 50, Description = "d", ImageUrl = "u", IsAdopted = false });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var result = await service.DeleteShelterAsync(50);
                Assert.That(result, Is.False);

                var shelter = await context.Shelters.FindAsync(50);
                Assert.That(shelter, Is.Not.Null);
            }
        }

        [Test]
        public async Task DeleteShelterAsync_Returns_True_When_The_Shelter_Has_No_Animals_And_Removes_It()
        {
            var dbName = nameof(DeleteShelterAsync_Returns_True_When_The_Shelter_Has_No_Animals_And_Removes_It);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.Add(new Shelter { Id = 51, Name = "RemShel", City = "C", Address = "A", Description = "D", ImageUrl = "http://i", Phone = "p", Email = "e@ex.com" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new ShelterService(context);

                var result = await service.DeleteShelterAsync(51);
                Assert.That(result, Is.True);

                var shelter = await context.Shelters.FindAsync(51);
                Assert.That(shelter, Is.Null);
            }
        }
    }
}