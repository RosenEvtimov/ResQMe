namespace ResQMe.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using ResQMe.Data;
    using ResQMe.Data.Models;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core;
    using ResQMe.ViewModels.AdoptionRequest;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class AdoptionRequestServiceTests
    {
        private DbContextOptions<ResQMeDbContext> CreateOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ResQMeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        private ResQMeDbContext CreateContext(string dbName)
            => new ResQMeDbContext(CreateOptions(dbName));

        /* User methods tests */
        [Test]
        public async Task CreateAdoptionRequestAsync_Creates_Request_When_No_Existing_Request()
        {
            var dbName = nameof(CreateAdoptionRequestAsync_Creates_Request_When_No_Existing_Request);

            // Arrange: seed animal and user
            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal
                {
                    Id = 1,
                    Name = "Buddy",
                    Age = 3,
                    Gender = Gender.Male,
                    SpeciesId = 1,
                    BreedType = 0,
                    Description = "Nice dog",
                    ImageUrl = "url",
                    IsAdopted = false,
                    ShelterId = 1
                });

                context.Users.Add(new ApplicationUser
                {
                    Id = "user-1",
                    UserName = "user1",
                    Email = "user1@example.com",
                    FirstName = "User",
                    LastName = "One"
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                var model = new AdoptionRequestFormViewModel
                {
                    AnimalId = 1,
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "Please let me adopt"
                };

                // Act
                await service.CreateAdoptionRequestAsync("user-1", model);

                // Assert (NUnit 4 constraint-style)
                var request = await context.AdoptionRequests.FirstOrDefaultAsync();
                Assert.That(request, Is.Not.Null, "Adoption request should be created.");
                Assert.That(request!.UserId, Is.EqualTo("user-1"));
                Assert.That(request.AnimalId, Is.EqualTo(1));
                Assert.That(request.Status, Is.EqualTo(AdoptionRequestStatus.Pending));
            }
        }

        [Test]
        public async Task CreateAdoptionRequestAsync_Throws_InvalidOperationException_On_Duplicate_Request()
        {
            var dbName = nameof(CreateAdoptionRequestAsync_Throws_InvalidOperationException_On_Duplicate_Request);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal
                {
                    Id = 2,
                    Name = "Milo",
                    Age = 2,
                    Gender = Gender.Male,
                    SpeciesId = 1,
                    BreedType = 0,
                    Description = "Good cat",
                    ImageUrl = "url",
                    IsAdopted = false,
                    ShelterId = 1
                });

                context.Users.Add(new ApplicationUser
                {
                    Id = "user-2",
                    UserName = "user2",
                    Email = "user2@example.com",
                    FirstName = "User",
                    LastName = "Two"
                });

                context.AdoptionRequests.Add(new AdoptionRequest
                {
                    Id = 1,
                    AnimalId = 2,
                    UserId = "user-2",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "first",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Pending
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                var model = new AdoptionRequestFormViewModel
                {
                    AnimalId = 2,
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "duplicate"
                };

                // Act & Assert using constraint-style
                Assert.That(async () => await service.CreateAdoptionRequestAsync("user-2", model),
                    Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public async Task HasUserAlreadyRequestedAsync_Returns_Correct_Value()
        {
            var dbName = nameof(HasUserAlreadyRequestedAsync_Returns_Correct_Value);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 3, Name = "Luna", Age = 1, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = false });
                context.Users.Add(new ApplicationUser { Id = "user-3", UserName = "u3" });
                context.AdoptionRequests.Add(new AdoptionRequest
                {
                    AnimalId = 3,
                    UserId = "user-3",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "hey",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Pending
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                bool existing = await service.HasUserAlreadyRequestedAsync("user-3", 3);
                bool notExisting = await service.HasUserAlreadyRequestedAsync("user-unknown", 3);

                Assert.That(existing, Is.True);
                Assert.That(notExisting, Is.False);
            }
        }

        [Test]
        public async Task GetMyRequestsAsync_Returns_All_User_Requests_Ordered()
        {
            var dbName = nameof(GetMyRequestsAsync_Returns_All_User_Requests_Ordered);

            using (var context = CreateContext(dbName))
            {
                context.Animals.AddRange(
                    new Animal { Id = 9, Name = "A", Age = 1, Gender = Gender.Male, SpeciesId = 1, BreedType = 0, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = false },
                    new Animal { Id = 10, Name = "B", Age = 2, Gender = Gender.Male, SpeciesId = 1, BreedType = 0, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = false }
                );

                context.Users.Add(new ApplicationUser { Id = "uH", UserName = "h" });

                context.AdoptionRequests.AddRange(
                    new AdoptionRequest { Id = 60, AnimalId = 9, UserId = "uH", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "m1", CreatedOn = DateTime.UtcNow.AddMinutes(-5), Status = AdoptionRequestStatus.Pending },
                    new AdoptionRequest { Id = 61, AnimalId = 10, UserId = "uH", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "m2", CreatedOn = DateTime.UtcNow, Status = AdoptionRequestStatus.Pending }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                var results = (await service.GetMyRequestsAsync("uH")).ToList();

                Assert.That(results.Count, Is.EqualTo(2));
                // ordered by CreatedOn descending
                Assert.That(results[0].Id, Is.EqualTo(61));
                Assert.That(results[1].Id, Is.EqualTo(60));
            }
        }

        /* Admin methods tests */
        [Test]
        public async Task GetAllRequestsForAdminAsync_Returns_Correct_Result_Through_Pagination_And_Filtering()
        {
            var dbName = nameof(GetAllRequestsForAdminAsync_Returns_Correct_Result_Through_Pagination_And_Filtering);

            using (var context = CreateContext(dbName))
            {
                // Seed animals
                context.Animals.AddRange(
                    new Animal { Id = 100, Name = "Alpha", Age = 2, Gender = Gender.Male, SpeciesId = 1, BreedType = 0, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = false },
                    new Animal { Id = 101, Name = "Beta", Age = 3, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = false }
                );

                // Seed users
                context.Users.AddRange(
                    new ApplicationUser { Id = "admin-u1", FirstName = "A", LastName = "One", UserName = "a1" },
                    new ApplicationUser { Id = "admin-u2", FirstName = "B", LastName = "Two", UserName = "b2" }
                );

                // Seed adoption requests with different statuses and created dates
                context.AdoptionRequests.AddRange(
                    new AdoptionRequest { Id = 1000, AnimalId = 100, UserId = "admin-u1", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "r1", CreatedOn = DateTime.UtcNow.AddMinutes(-30), Status = AdoptionRequestStatus.Pending },
                    new AdoptionRequest { Id = 1001, AnimalId = 101, UserId = "admin-u2", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "r2", CreatedOn = DateTime.UtcNow.AddMinutes(-20), Status = AdoptionRequestStatus.Approved },
                    new AdoptionRequest { Id = 1002, AnimalId = 100, UserId = "admin-u2", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "r3", CreatedOn = DateTime.UtcNow.AddMinutes(-10), Status = AdoptionRequestStatus.Rejected }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                // page=1, pageSize=2 should return 2 items, totalItems = 3
                var page1 = await service.GetAllRequestsForAdminAsync(null, 1, 2);

                Assert.That(page1.TotalItems, Is.EqualTo(3));
                Assert.That(page1.TotalPages, Is.EqualTo(2));
                Assert.That(page1.Items.Count(), Is.EqualTo(2));

                // Filtering by Pending should return only the pending request
                var pending = await service.GetAllRequestsForAdminAsync(AdoptionRequestStatus.Pending, 1, 10);
                Assert.That(pending.TotalItems, Is.EqualTo(1));
                Assert.That(pending.Items.Count(), Is.EqualTo(1));
                Assert.That(pending.Items.First().Status, Is.EqualTo(AdoptionRequestStatus.Pending));
            }
        }

        [Test]
        public async Task GetRequestByIdForAdminAsync_Returns_Correct_Result_Including_Animal_IsAdopted_Flag()
        {
            var dbName = nameof(GetRequestByIdForAdminAsync_Returns_Correct_Result_Including_Animal_IsAdopted_Flag);

            using (var ctx = CreateContext(dbName))
            {
                // Seed animal (adopted = true)
                ctx.Animals.Add(new Animal { Id = 200, Name = "Gamma", Age = 4, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "d", ImageUrl = "u", ShelterId = 1, IsAdopted = true });

                // Seed user
                ctx.Users.Add(new ApplicationUser { Id = "detail-u1", FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "123456", Address = "Addr", City = "City" });

                // Seed adoption request
                ctx.AdoptionRequests.Add(new AdoptionRequest
                {
                    Id = 2000,
                    AnimalId = 200,
                    UserId = "detail-u1",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.Yes,
                    Message = "please approve",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Pending
                });

                await ctx.SaveChangesAsync();
            }

            using (var ctx = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(ctx);

                var model = await service.GetRequestByIdForAdminAsync(2000);

                Assert.That(model, Is.Not.Null);
                Assert.That(model!.Id, Is.EqualTo(2000));
                Assert.That(model.AnimalName, Is.EqualTo("Gamma"));
                Assert.That(model.FirstName, Is.EqualTo("John"));
                Assert.That(model.LastName, Is.EqualTo("Doe"));
                Assert.That(model.Email, Is.EqualTo("john@example.com"));
                Assert.That(model.PhoneNumber, Is.EqualTo("123456"));
                Assert.That(model.Address, Is.EqualTo("Addr"));
                Assert.That(model.City, Is.EqualTo("City"));
                Assert.That(model.Status, Is.EqualTo(AdoptionRequestStatus.Pending));
                // The service maps the animal's IsAdopted flag into the view model property (IsAnimalAdopted).
                Assert.That(model.IsAnimalAdopted, Is.True);
            }
        }
        [Test]
        public async Task ApproveRequestAsync_Approves_Current_Request_And_Rejects_Others()
        {
            var dbName = nameof(ApproveRequestAsync_Approves_Current_Request_And_Rejects_Others);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 4, Name = "Rocky", Age = 4, Gender = Gender.Male, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = false });

                context.Users.AddRange(
                    new ApplicationUser { Id = "uA", UserName = "a" },
                    new ApplicationUser { Id = "uB", UserName = "b" }
                );

                context.AdoptionRequests.AddRange(
                    new AdoptionRequest { Id = 10, AnimalId = 4, UserId = "uA", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "a", CreatedOn = DateTime.UtcNow.AddMinutes(-2), Status = AdoptionRequestStatus.Pending },
                    new AdoptionRequest { Id = 11, AnimalId = 4, UserId = "uB", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "b", CreatedOn = DateTime.UtcNow.AddMinutes(-1), Status = AdoptionRequestStatus.Pending }
                );

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                // Act
                await service.ApproveRequestAsync(10);

                // Assert
                var req1 = await context.AdoptionRequests.FindAsync(10);
                var req2 = await context.AdoptionRequests.FindAsync(11);
                var animal = await context.Animals.FindAsync(4);

                Assert.That(req1!.Status, Is.EqualTo(AdoptionRequestStatus.Approved));
                Assert.That(req2!.Status, Is.EqualTo(AdoptionRequestStatus.Rejected));
                Assert.That(animal!.IsAdopted, Is.True);
            }
        }

        [Test]
        public async Task ApproveRequestAsync_Should_Not_Approve_The_Request_When_The_Animal_Is_Already_Adopted()
        {
            var dbName = nameof(ApproveRequestAsync_Should_Not_Approve_The_Request_When_The_Animal_Is_Already_Adopted);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 5, Name = "Zoe", Age = 2, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = true });

                context.Users.Add(new ApplicationUser { Id = "uC", UserName = "c" });

                context.AdoptionRequests.Add(new AdoptionRequest
                {
                    Id = 20,
                    AnimalId = 5,
                    UserId = "uC",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "please",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Pending
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                await service.ApproveRequestAsync(20);

                var request = await context.AdoptionRequests.FindAsync(20);
                Assert.That(request!.Status, Is.EqualTo(AdoptionRequestStatus.Pending), "Request should remain pending when animal already adopted.");
            }
        }

        [Test]
        public async Task RejectRequestAsync_Should_Mark_Request_As_Rejected()
        {
            var dbName = nameof(RejectRequestAsync_Should_Mark_Request_As_Rejected);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 6, Name = "Nala", Age = 3, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = false });
                context.Users.Add(new ApplicationUser { Id = "uD", UserName = "d" });

                context.AdoptionRequests.Add(new AdoptionRequest
                {
                    Id = 30,
                    AnimalId = 6,
                    UserId = "uD",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "ok",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Pending
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                await service.RejectRequestAsync(30);

                var request = await context.AdoptionRequests.FindAsync(30);
                Assert.That(request!.Status, Is.EqualTo(AdoptionRequestStatus.Rejected));
            }
        }

        [Test]
        public async Task UndoApprovalAsync_Should_Set_Current_Request_To_Pending_Reopen_Rejected_Requests_And_Make_Animal_Available()
        {
            var dbName = nameof(UndoApprovalAsync_Should_Set_Current_Request_To_Pending_Reopen_Rejected_Requests_And_Make_Animal_Available);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 7, Name = "Bella", Age = 5, Gender = Gender.Female, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = true });
                context.Users.AddRange(new ApplicationUser { Id = "uE", UserName = "e" }, new ApplicationUser { Id = "uF", UserName = "f" });

                // Approved request for uE
                context.AdoptionRequests.Add(new AdoptionRequest { Id = 40, AnimalId = 7, UserId = "uE", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "a", CreatedOn = DateTime.UtcNow.AddMinutes(-10), Status = AdoptionRequestStatus.Approved });

                // Rejected request for uF
                context.AdoptionRequests.Add(new AdoptionRequest { Id = 41, AnimalId = 7, UserId = "uF", PreviousAdoptionExperience = PreviousAdoptionExperience.No, Message = "b", CreatedOn = DateTime.UtcNow.AddMinutes(-5), Status = AdoptionRequestStatus.Rejected });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                await service.UndoApprovalAsync(40);

                var reqApproved = await context.AdoptionRequests.FindAsync(40);
                var reqRejected = await context.AdoptionRequests.FindAsync(41);
                var animal = await context.Animals.FindAsync(7);

                Assert.That(reqApproved!.Status, Is.EqualTo(AdoptionRequestStatus.Pending));
                Assert.That(reqRejected!.Status, Is.EqualTo(AdoptionRequestStatus.Pending));
                Assert.That(animal!.IsAdopted, Is.False);
            }
        }

        [Test]
        public async Task UndoRejectionAsync_Should_Set_Current_Request_To_Pending()
        {
            var dbName = nameof(UndoRejectionAsync_Should_Set_Current_Request_To_Pending);

            using (var context = CreateContext(dbName))
            {
                context.Animals.Add(new Animal { Id = 8, Name = "Finn", Age = 2, Gender = Gender.Male, SpeciesId = 1, BreedType = 0, Description = "desc", ImageUrl = "url", ShelterId = 1, IsAdopted = false });
                context.Users.Add(new ApplicationUser { Id = "uG", UserName = "g" });

                context.AdoptionRequests.Add(new AdoptionRequest
                {
                    Id = 50,
                    AnimalId = 8,
                    UserId = "uG",
                    PreviousAdoptionExperience = PreviousAdoptionExperience.No,
                    Message = "msg",
                    CreatedOn = DateTime.UtcNow,
                    Status = AdoptionRequestStatus.Rejected
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new AdoptionRequestService(context);

                await service.UndoRejectionAsync(50);

                var request = await context.AdoptionRequests.FindAsync(50);
                Assert.That(request!.Status, Is.EqualTo(AdoptionRequestStatus.Pending));
            }
        }      
    }
}