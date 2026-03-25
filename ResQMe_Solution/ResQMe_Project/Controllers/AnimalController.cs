namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;
    using ResQMe.ViewModels.Common;

    public class AnimalController : Controller
    {
        private readonly IAnimalService animalService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAdoptionRequestService adoptionRequestService;

        public AnimalController(IAnimalService animalService, UserManager<ApplicationUser> userManager, IAdoptionRequestService adoptionRequestService)
        {
            this.animalService = animalService;
            this.userManager = userManager;
            this.adoptionRequestService = adoptionRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await animalService.GetAllAnimalsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, int? fromShelterId)
        {
            var model = await animalService.GetAnimalDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.FromShelterId = fromShelterId;

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                var user = await userManager.GetUserAsync(User);

                if (user != null)
                {
                    bool hasAlreadyRequested =
                        await adoptionRequestService.HasUserAlreadyRequestedAsync(user.Id, id);

                    ViewBag.HasAlreadyRequested = hasAlreadyRequested;
                }
            }
            else
            {
                ViewBag.HasAlreadyRequested = false;
            }

            return View(model);
        }        
    }
}