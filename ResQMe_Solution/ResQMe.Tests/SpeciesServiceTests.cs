namespace ResQMe.Tests
{
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Services.Core;
    using ResQMe.ViewModels.Species;

    [TestFixture]
    public class SpeciesServiceTests
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
        public async Task GetAllSpeciesAsync_Returns_Correct_Result_Through_Search_Pagination_And_Ordering()
        {
            var dbName = nameof(GetAllSpeciesAsync_Returns_Correct_Result_Through_Search_Pagination_And_Ordering);

            using (var context = CreateContext(dbName))
            {
                context.Species.AddRange(
                    new Species { Id = 1, Name = "Zebra" },
                    new Species { Id = 2, Name = "Alpha" },
                    new Species { Id = 3, Name = "Beta" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                // Search by term "eb" should match "Zebra"
                var searchResult = await service.GetAllSpeciesAsync("eb", 1, 10);
                Assert.That(searchResult.Items.Count(), Is.EqualTo(1));
                Assert.That(searchResult.Items.First().Name, Is.EqualTo("Zebra"));

                // No filter: ordered by Name -> Alpha, Beta, Zebra
                var all = await service.GetAllSpeciesAsync(null, 1, 10);
                var names = all.Items.Select(i => i.Name).ToList();
                Assert.That(names, Is.EqualTo(new List<string> { "Alpha", "Beta", "Zebra" }));

                // Pagination: pageSize = 1 -> TotalPages = 3
                var paged = await service.GetAllSpeciesAsync(null, 1, 1);
                Assert.That(paged.TotalPages, Is.EqualTo(3));
                Assert.That(paged.Items.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetSpeciesForEditAsync_Returns_Correct_Species()
        {
            var dbName = nameof(GetSpeciesForEditAsync_Returns_Correct_Species);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 10, Name = "Canine" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var model = await service.GetSpeciesForEditAsync(10);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(10));
                Assert.That(model.Name, Is.EqualTo("Canine"));
            }
        }

        [Test]
        public async Task AddSpeciesAsync_Adds_New_Species()
        {
            var dbName = nameof(AddSpeciesAsync_Adds_New_Species);

            using (var context = CreateContext(dbName))
            {
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var form = new SpeciesFormViewModel
                {
                    Name = "NewSpecies"
                };

                await service.AddSpeciesAsync(form);

                var species = await context.Species.FirstOrDefaultAsync(s => s.Name == "NewSpecies");
                Assert.That(species, Is.Not.Null);
                Assert.That(species!.Name, Is.EqualTo("NewSpecies"));
            }
        }

        [Test]
        public async Task EditSpeciesAsync_Updates_Existing_Species()
        {
            var dbName = nameof(EditSpeciesAsync_Updates_Existing_Species);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 20, Name = "OldName" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var form = new SpeciesFormViewModel
                {
                    Id = 20,
                    Name = "UpdatedName"
                };

                await service.EditSpeciesAsync(form);

                var species = await context.Species.FindAsync(20);
                Assert.That(species, Is.Not.Null);
                Assert.That(species!.Name, Is.EqualTo("UpdatedName"));
            }
        }

        [Test]
        public async Task DeleteSpeciesAsync_Returns_False_When_Species_Does_Not_Exist()
        {
            var dbName = nameof(DeleteSpeciesAsync_Returns_False_When_Species_Does_Not_Exist);

            using (var context = CreateContext(dbName))
            {
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var result = await service.DeleteSpeciesAsync(999);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public async Task DeleteSpeciesAsync_Returns_False_When_Species_Has_Animals()
        {
            var dbName = nameof(DeleteSpeciesAsync_Returns_False_When_Species_Has_Animals);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 40, Name = "HasAnimals" });
                context.Animals.Add(new Animal { Id = 800, Name = "A1", Age = 1, Gender = Data.Models.Enums.Gender.Male, SpeciesId = 40, BreedType = Data.Models.Enums.BreedType.Mixed, ShelterId = 1, Description = "d", ImageUrl = "u", IsAdopted = false });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var result = await service.DeleteSpeciesAsync(40);
                Assert.That(result, Is.False);

                var species = await context.Species.FindAsync(40);
                Assert.That(species, Is.Not.Null);
            }
        }

        [Test]
        public async Task DeleteSpeciesAsync_Returns_True_When_Species_Has_No_Animals_And_Removes_It()
        {
            var dbName = nameof(DeleteSpeciesAsync_Returns_True_When_Species_Has_No_Animals_And_Removes_It);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 41, Name = "Removable" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new SpeciesService(context);

                var result = await service.DeleteSpeciesAsync(41);
                Assert.That(result, Is.True);

                var species = await context.Species.FindAsync(41);
                Assert.That(species, Is.Null);
            }
        }
    }
}