namespace ResQMe.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class AnimalServiceTests
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
        public async Task GetAllAnimalsAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering()
        {
            var dbName = nameof(GetAllAnimalsAsync_Returns_Correct_Result_Through_Filters_Search_Pagination_And_Ordering);

            using (var context = CreateContext(dbName))
            {
                // Seed lookups
                context.Species.Add(new Species { Id = 1, Name = "Dog" });
                context.Breeds.AddRange(
                    new Breed { Id = 1, Name = "Bulldog", SpeciesId = 1 },
                    new Breed { Id = 2, Name = "Beagle", SpeciesId = 1 }
                );

                context.Shelters.AddRange(
                    new Shelter { Id = 1, Name = "ShelterA", City = "CityA", Address = "AddrA", Description = "DescA", ImageUrl = "http://imgA", Phone = "123-000", Email = "a@shelter.com" },
                    new Shelter { Id = 2, Name = "ShelterB", City = "CityB", Address = "AddrB", Description = "DescB", ImageUrl = "http://imgB", Phone = "123-001", Email = "b@shelter.com" }
                );

                // Non-adopted named "Alpha" and adopted "Beta" -> ordering by IsAdopted then Name
                context.Animals.AddRange(
                    new Animal { Id = 1, Name = "Alpha", Age = 2, Gender = Gender.Male, SpeciesId = 1, BreedType = BreedType.Purebred, BreedId = 1, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = false },
                    new Animal { Id = 2, Name = "Beta", Age = 4, Gender = Gender.Female, SpeciesId = 1, BreedType = BreedType.Purebred, BreedId = 2, Description = "d", ImageUrl = "u", ShelterId = 2, IsAdopted = true }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                // Search by breed name "Beagle" should match Beta
                var resultSearch = await service.GetAllAnimalsAsync(
                    searchTerm: "Beagle",
                    speciesIds: new List<int>(),
                    genders: new List<Gender>(),
                    breedTypes: new List<BreedType>(),
                    cities: new List<string>(),
                    ageRanges: new List<string>(),
                    showAdopted: null,
                    page: 1,
                    pageSize: 10);

                Assert.That(resultSearch.Items.Count(), Is.EqualTo(1));
                Assert.That(resultSearch.Items.First().Name, Is.EqualTo("Beta"));

                // Filter by city CityA should return Alpha
                var resultCity = await service.GetAllAnimalsAsync(
                    searchTerm: null,
                    speciesIds: new List<int>(),
                    genders: new List<Gender>(),
                    breedTypes: new List<BreedType>(),
                    cities: new List<string> { "CityA" },
                    ageRanges: new List<string>(),
                    showAdopted: null,
                    page: 1,
                    pageSize: 10);

                Assert.That(resultCity.Items.Count(), Is.EqualTo(1));
                Assert.That(resultCity.Items.First().Name, Is.EqualTo("Alpha"));

                // showAdopted = false should return only non-adopted (Alpha)
                var resultNotAdopted = await service.GetAllAnimalsAsync(
                    searchTerm: null,
                    speciesIds: new List<int>(),
                    genders: new List<Gender>(),
                    breedTypes: new List<BreedType>(),
                    cities: new List<string>(),
                    ageRanges: new List<string>(),
                    showAdopted: false,
                    page: 1,
                    pageSize: 10);

                Assert.That(resultNotAdopted.Items.Count(), Is.EqualTo(1));
                Assert.That(resultNotAdopted.Items.First().IsAdopted, Is.False);

                // Pagination: pageSize 1 should produce correct total pages = 2
                var paged = await service.GetAllAnimalsAsync(
                    searchTerm: null,
                    speciesIds: new List<int>(),
                    genders: new List<Gender>(),
                    breedTypes: new List<BreedType>(),
                    cities: new List<string>(),
                    ageRanges: new List<string>(),
                    showAdopted: null,
                    page: 1,
                    pageSize: 1);

                Assert.That(paged.TotalPages, Is.EqualTo(2));
                Assert.That(paged.TotalPages, Is.GreaterThan(0));
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
                    new Shelter { Id = 1, Name = "S1", City = "BCity", Address = "A1", Description = "D1", ImageUrl = "http://i1", Phone = "111", Email = "s1@example.com" },
                    new Shelter { Id = 2, Name = "S2", City = "ACity", Address = "A2", Description = "D2", ImageUrl = "http://i2", Phone = "112", Email = "s2@example.com" },
                    new Shelter { Id = 3, Name = "S3", City = "BCity", Address = "A3", Description = "D3", ImageUrl = "http://i3", Phone = "113", Email = "s3@example.com" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var cities = (await service.GetUniqueCitiesAsync()).ToList();

                Assert.That(cities.Count, Is.EqualTo(2));
                Assert.That(cities[0], Is.EqualTo("ACity"));
                Assert.That(cities[1], Is.EqualTo("BCity"));
            }
        }

        [Test]
        public async Task GetAnimalDetailsAsync_Returns_Correct_Details()
        {
            var dbName = nameof(GetAnimalDetailsAsync_Returns_Correct_Details);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 1, Name = "Cat" });
                context.Breeds.Add(new Breed { Id = 1, Name = "Siamese", SpeciesId = 1 });
                context.Shelters.Add(new Shelter { Id = 1, Name = "MainShelter", City = "Town", Address = "Addr", Description = "Desc", ImageUrl = "http://img", Phone = "555-1234", Email = "main@shelter.com" });

                context.Animals.Add(new Animal
                {
                    Id = 11,
                    Name = "Whiskers",
                    Age = 3,
                    Gender = Gender.Female,
                    SpeciesId = 1,
                    BreedType = BreedType.Purebred,
                    BreedId = 1,
                    Description = "desc",
                    ImageUrl = "img",
                    ShelterId = 1,
                    IsAdopted = true
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var model = await service.GetAnimalDetailsAsync(11);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(11));
                Assert.That(model.Name, Is.EqualTo("Whiskers"));
                Assert.That(model.BreedDisplay, Is.EqualTo("Siamese"));
                Assert.That(model.Species, Is.EqualTo("Cat"));
                Assert.That(model.ShelterName, Is.EqualTo("MainShelter"));
                Assert.That(model.IsAdopted, Is.True);
            }
        }

        [Test]
        public async Task GetAnimalForEditAsync_Returns_Correct_Animal()
        {
            var dbName = nameof(GetAnimalForEditAsync_Returns_Correct_Animal);

            using (var context = CreateContext(dbName))
            {
                // Provide shelter referenced by the animal to avoid FK issues in some providers
                context.Shelters.Add(new Shelter { Id = 2, Name = "Shel2", City = "Town2", Address = "Addr2", Description = "Desc2", ImageUrl = "http://img2", Phone = "222", Email = "shel2@example.com" });

                context.Animals.Add(new Animal
                {
                    Id = 21,
                    Name = "EditMe",
                    Age = 5,
                    Gender = Gender.Male,
                    SpeciesId = 2,
                    BreedType = BreedType.Mixed,
                    BreedId = null,
                    Description = "desc",
                    ImageUrl = "img",
                    ShelterId = 2,
                    IsAdopted = false
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var model = await service.GetAnimalForEditAsync(21);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(21));
                Assert.That(model.Name, Is.EqualTo("EditMe"));
                Assert.That(model.BreedType, Is.EqualTo(BreedType.Mixed));
                Assert.That(model.IsAdopted, Is.False);
            }
        }

        [Test]
        public async Task AddAnimalAsync_Adds_Animal_With_IsAdopted_Flag_Set_To_False()
        {
            var dbName = nameof(AddAnimalAsync_Adds_Animal_With_IsAdopted_Flag_Set_To_False);

            using (var context = CreateContext(dbName))
            {
                context.Species.Add(new Species { Id = 10, Name = "SpeciesX" });
                context.Breeds.Add(new Breed { Id = 10, Name = "BreedX", SpeciesId = 10 });
                context.Shelters.Add(new Shelter { Id = 10, Name = "ShelX", City = "C", Address = "AddrX", Description = "DescX", ImageUrl = "http://imgX", Phone = "999", Email = "x@shelter.com" });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var form = new AnimalFormViewModel
                {
                    Name = "NewAnimal",
                    Age = 1,
                    Gender = Gender.Female,
                    SpeciesId = 10,
                    BreedType = BreedType.Purebred,
                    BreedId = 10,
                    Description = "d",
                    ImageUrl = "u",
                    ShelterId = 10
                };

                await service.AddAnimalAsync(form);

                var animal = await context.Animals.FirstOrDefaultAsync(a => a.Name == "NewAnimal");
                Assert.That(animal, Is.Not.Null);
                Assert.That(animal!.IsAdopted, Is.False);
                Assert.That(animal.BreedId, Is.EqualTo(10));
            }
        }

        [Test]
        public async Task EditAnimalAsync_Updates_Existing_Animal()
        {
            var dbName = nameof(EditAnimalAsync_Updates_Existing_Animal);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal
                {
                    Id = 31,
                    Name = "Old",
                    Age = 2,
                    Gender = Gender.Male,
                    SpeciesId = 5,
                    BreedType = BreedType.Mixed,
                    BreedId = null,
                    Description = "old",
                    ImageUrl = "img",
                    ShelterId = 5,
                    IsAdopted = false
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var form = new AnimalFormViewModel
                {
                    Id = 31,
                    Name = "Updated",
                    Age = 4,
                    Gender = Gender.Female,
                    SpeciesId = 5,
                    BreedType = BreedType.Mixed,
                    BreedId = null,
                    Description = "updated",
                    ImageUrl = "img2",
                    ShelterId = 5,
                    IsAdopted = true
                };

                await service.EditAnimalAsync(form);

                var animal = await context.Animals.FindAsync(31);
                Assert.That(animal, Is.Not.Null);
                Assert.That(animal!.Name, Is.EqualTo("Updated"));
                Assert.That(animal.Age, Is.EqualTo(4));
                Assert.That(animal.IsAdopted, Is.True);
            }
        }

        [Test]
        public async Task DeleteAnimalAsync_Returns_False_When_The_Animal_Does_Not_Exist()
        {
            var dbName = nameof(DeleteAnimalAsync_Returns_False_When_The_Animal_Does_Not_Exist);

            using (var context = CreateContext(dbName))
            {
                // no animals
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var result = await service.DeleteAnimalAsync(999);
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public async Task DeleteAnimalAsync_Returns_False_When_The_Animal_Has_AdoptionRequests()
        {
            var dbName = nameof(DeleteAnimalAsync_Returns_False_When_The_Animal_Has_AdoptionRequests);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 41, Name = "ToDelete", Age = 1, Gender = Gender.Male, SpeciesId = 1, BreedType = BreedType.Mixed, ShelterId = 1, Description = "d", ImageUrl = "u", IsAdopted = false });
                context.AdoptionRequests.Add(new AdoptionRequest { Id = 500, AnimalId = 41, UserId = "u1", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "m", CreatedOn = DateTime.UtcNow, Status = AdoptionRequestStatus.Pending });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var result = await service.DeleteAnimalAsync(41);
                Assert.That(result, Is.False);

                var animal = await context.Animals.FindAsync(41);
                Assert.That(animal, Is.Not.Null);
            }
        }

        [Test]
        public async Task DeleteAnimalAsync_Returns_True_When_The_Animal_Has_No_AdoptionRequests_And_Removes_It()
        {
            var dbName = nameof(DeleteAnimalAsync_Returns_True_When_The_Animal_Has_No_AdoptionRequests_And_Removes_It);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 42, Name = "Removable", Age = 2, Gender = Gender.Female, SpeciesId = 1, BreedType = BreedType.Mixed, ShelterId = 1, Description = "d", ImageUrl = "u", IsAdopted = false });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var result = await service.DeleteAnimalAsync(42);
                Assert.That(result, Is.True);

                var animal = await context.Animals.FindAsync(42);
                Assert.That(animal, Is.Null);
            }
        }

        [Test]
        public async Task GetSpeciesForDropdownAsync_Returns_All_Species()
        {
            var dbName = nameof(GetSpeciesForDropdownAsync_Returns_All_Species);

            using (var context = CreateContext(dbName))
            {
                context.Species.AddRange(
                    new Species { Id = 100, Name = "S100" },
                    new Species { Id = 101, Name = "S101" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var items = (await service.GetSpeciesForDropdownAsync()).ToList();

                Assert.That(items.Count, Is.EqualTo(2));
                Assert.That(items.Any(i => i.Id == 100 && i.Name == "S100"), Is.True);
                Assert.That(items.Any(i => i.Id == 101 && i.Name == "S101"), Is.True);
            }
        }

        [Test]
        public async Task GetBreedsBySpeciesAsync_Returns_Breeds_Filtered_By_Species()
        {
            var dbName = nameof(GetBreedsBySpeciesAsync_Returns_Breeds_Filtered_By_Species);

            using (var context = CreateContext(dbName))
            {
                context.Breeds.AddRange(
                    new Breed { Id = 200, Name = "B200", SpeciesId = 20 },
                    new Breed { Id = 201, Name = "B201", SpeciesId = 21 }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var breeds = (await service.GetBreedsBySpeciesAsync(20)).ToList();

                Assert.That(breeds.Count, Is.EqualTo(1));
                Assert.That(breeds.First().Id, Is.EqualTo(200));
                Assert.That(breeds.First().Name, Is.EqualTo("B200"));
            }
        }

        [Test]
        public async Task GetSheltersForDropdownAsync_Returns_All_Shelters()
        {
            var dbName = nameof(GetSheltersForDropdownAsync_Returns_All_Shelters);

            using (var context = CreateContext(dbName))
            {
                context.Shelters.AddRange(
                    new Shelter { Id = 300, Name = "Sh300", City = "C", Address = "A300", Description = "D300", ImageUrl = "http://i300", Phone = "300", Email = "sh300@example.com" },
                    new Shelter { Id = 301, Name = "Sh301", City = "D", Address = "A301", Description = "D301", ImageUrl = "http://i301", Phone = "301", Email = "sh301@example.com" }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AnimalService(context);

                var items = (await service.GetSheltersForDropdownAsync()).ToList();

                Assert.That(items.Count, Is.EqualTo(2));
                Assert.That(items.Any(i => i.Id == 300 && i.Name == "Sh300"), Is.True);
                Assert.That(items.Any(i => i.Id == 301 && i.Name == "Sh301"), Is.True);
            }
        }
    }
}