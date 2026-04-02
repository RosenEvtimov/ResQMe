namespace ResQMe.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core;
    using ResQMe.ViewModels.Breed;

    [TestFixture]
    public class BreedServiceTests
    {
        private DbContextOptions<ResQMeDbContext> CreateOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ResQMeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        private ResQMeDbContext CreateContext(string dbName)
            => new ResQMeDbContext(CreateOptions(dbName));

        [Test]
        public async Task GetAllBreedsAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering()
        {
            var dbName = nameof(GetAllBreedsAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering);

            using (var context = CreateContext(dbName))
            {
                context.Species.AddRange(
                    new Species { Id = 1, Name = "Canine" },
                    new Species { Id = 2, Name = "Feline" }
                );

                context.Breeds.AddRange(
                    new Breed { Id = 1, Name = "Bulldog", SpeciesId = 1 },
                    new Breed { Id = 2, Name = "Beagle", SpeciesId = 1 },
                    new Breed { Id = 3, Name = "Siamese", SpeciesId = 2 }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                // Search by "ull" should match "Bulldog"
                var resultSearch = await service.GetAllBreedsAsync(
                    searchTerm: "ull",
                    speciesIds: new List<int>(),
                    page: 1,
                    pageSize: 10);

                Assert.That(resultSearch.Items.Count(), Is.EqualTo(1));
                Assert.That(resultSearch.Items.First().Name, Is.EqualTo("Bulldog"));

                // Filter by speciesId = 1 should return Bulldog and Beagle ordered by Species.Name then Name
                var resultFilter = await service.GetAllBreedsAsync(
                    searchTerm: null,
                    speciesIds: new List<int> { 1 },
                    page: 1,
                    pageSize: 10);

                Assert.That(resultFilter.Items.Count(), Is.EqualTo(2));
                var names = resultFilter.Items.Select(i => i.Name).ToList();
                Assert.That(names, Is.EqualTo(new List<string> { "Beagle", "Bulldog" }));

                // Pagination: pageSize =1 should produce TotalPages = 3
                var paged = await service.GetAllBreedsAsync(
                    searchTerm: null,
                    speciesIds: new List<int>(),
                    page: 1,
                    pageSize: 1);

                Assert.That(paged.TotalPages, Is.EqualTo(3));
                Assert.That(paged.Items.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetBreedForEditAsync_Returns_Correct_Breed()
        {
            var dbName = nameof(GetBreedForEditAsync_Returns_Correct_Breed);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 5, Name = "Species5" });
                context.Breeds.Add(new Breed { Id = 10, Name = "TestBreed", SpeciesId = 5 });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var model = await service.GetBreedForEditAsync(10);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(10));
                Assert.That(model.Name, Is.EqualTo("TestBreed"));
                Assert.That(model.SpeciesId, Is.EqualTo(5));
            }
        }

        [Test]
        public async Task AddBreedAsync_Adds_New_Breed()
        {
            var dbName = nameof(AddBreedAsync_Adds_New_Breed);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 20, Name = "Sp20" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var form = new BreedFormViewModel
                {
                    Name = "NewBreed",
                    SpeciesId = 20
                };

                await service.AddBreedAsync(form);

                var breed = await context.Breeds.FirstOrDefaultAsync(b => b.Name == "NewBreed");
                Assert.That(breed, Is.Not.Null);
                Assert.That(breed!.SpeciesId, Is.EqualTo(20));
            }
        }

        [Test]
        public async Task EditBreedAsync_Updates_Existing_Breed()
        {
            var dbName = nameof(EditBreedAsync_Updates_Existing_Breed);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 40, Name = "Spec40" });
                context.Breeds.Add(new Breed { Id = 41, Name = "OldName", SpeciesId = 40 });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var form = new BreedFormViewModel
                {
                    Id = 41,
                    Name = "UpdatedName",
                    SpeciesId = 40
                };

                await service.EditBreedAsync(form);

                var breed = await context.Breeds.FindAsync(41);
                Assert.That(breed, Is.Not.Null);
                Assert.That(breed!.Name, Is.EqualTo("UpdatedName"));
            }
        }

        [Test]
        public async Task DeleteBreedAsync_Returns_False_When_The_Breed_Does_Not_Exist()
        {
            var dbName = nameof(DeleteBreedAsync_Returns_False_When_The_Breed_Does_Not_Exist);

            using (var context = CreateContext(dbName))
            {
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var result = await service.DeleteBreedAsync(999);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public async Task DeleteBreedAsync_Returns_False_When_The_Breed_Has_Animals()
        {
            var dbName = nameof(DeleteBreedAsync_Returns_False_When_The_Breed_Has_Animals);

            using (var context = CreateContext(dbName))
            {
                context.Breeds.Add(new Breed { Id = 60, Name = "HasAnimals", SpeciesId = 1 });
                context.Animals.Add(new Animal { Id = 700, Name = "A1", Age = 1, Gender = Data.Models.Enums.Gender.Male, SpeciesId = 1, BreedType = Data.Models.Enums.BreedType.Mixed, BreedId = 60, ShelterId = 1, Description = "d", ImageUrl = "u", IsAdopted = false });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var result = await service.DeleteBreedAsync(60);
                Assert.That(result, Is.False);

                var breed = await context.Breeds.FindAsync(60);
                Assert.That(breed, Is.Not.Null);
            }
        }

        [Test]
        public async Task DeleteBreedAsync_Returns_True_When_The_Breed_Has_No_Animals_And_Removes_It()
        {
            var dbName = nameof(DeleteBreedAsync_Returns_True_When_The_Breed_Has_No_Animals_And_Removes_It);

            using (var context = CreateContext(dbName))
            {
                context.Breeds.Add(new Breed { Id = 61, Name = "RemovableBreed", SpeciesId = 2 });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var result = await service.DeleteBreedAsync(61);
                Assert.That(result, Is.True);

                var breed = await context.Breeds.FindAsync(61);
                Assert.That(breed, Is.Null);
            }
        }

        [Test]
        public async Task GetSpeciesForDropdownAsync_Returns_All_Species()
        {
            var dbName = nameof(GetSpeciesForDropdownAsync_Returns_All_Species);

            using (var context = CreateContext(dbName))
            {
                context.Species.AddRange(
                    new Species { Id = 300, Name = "S300" },
                    new Species { Id = 301, Name = "S301" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BreedService(context);

                var items = (await service.GetSpeciesForDropdownAsync()).ToList();

                Assert.That(items.Count, Is.EqualTo(2));
                Assert.That(items.Any(i => i.Id == 300 && i.Name == "S300"), Is.True);
                Assert.That(items.Any(i => i.Id == 301 && i.Name == "S301"), Is.True);
            }
        }
    }
}