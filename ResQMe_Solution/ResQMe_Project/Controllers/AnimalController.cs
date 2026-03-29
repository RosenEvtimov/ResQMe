namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Animal;

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
        public async Task<IActionResult> Index(
            string? searchTerm,
            List<int> selectedSpeciesIds,
            List<Gender> selectedGenders,
            List<BreedType> selectedBreedTypes,
            List<string> selectedCities,
            List<string> selectedAgeRanges,
            bool? showAdopted,
            int page = 1)
        {
            const int pageSize = 2;

            var results = await animalService.GetAllAnimalsAsync(
                searchTerm,
                selectedSpeciesIds,
                selectedGenders,
                selectedBreedTypes,
                selectedCities,
                selectedAgeRanges,
                showAdopted,
                page,
                pageSize);

            var model = new AnimalFilterViewModel
            {
                Results = results,
                AvailableSpecies = await animalService.GetSpeciesForDropdownAsync(),
                AvailableCities = await animalService.GetUniqueCitiesAsync(),
                SearchTerm = searchTerm,
                SelectedSpeciesIds = selectedSpeciesIds,
                SelectedGenders = selectedGenders,
                SelectedBreedTypes = selectedBreedTypes,
                SelectedCities = selectedCities,
                SelectedAgeRanges = selectedAgeRanges,
                ShowAdopted = showAdopted
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, int? fromShelterId, string? returnUrl, string? shelterReturnUrl)
        {
            var model = await animalService.GetAnimalDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.FromShelterId = fromShelterId;
            ViewBag.ShelterReturnUrl = shelterReturnUrl;

            /* Checking if the query is empty or not, for correct button visualisation on the Details View */
            var qs = Request.QueryString.Value;
            bool hasActiveFilters = false;

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "?")
            {
                var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(returnUrl.TrimStart('?'));

                hasActiveFilters =
                    (parsed.ContainsKey("searchTerm") && !string.IsNullOrEmpty(parsed["searchTerm"])) ||
                    (parsed.ContainsKey("selectedSpeciesIds") && parsed["selectedSpeciesIds"].Any(v => !string.IsNullOrEmpty(v))) ||
                    (parsed.ContainsKey("selectedGenders") && parsed["selectedGenders"].Any(v => !string.IsNullOrEmpty(v))) ||
                    (parsed.ContainsKey("selectedBreedTypes") && parsed["selectedBreedTypes"].Any(v => !string.IsNullOrEmpty(v))) ||
                    (parsed.ContainsKey("selectedCities") && parsed["selectedCities"].Any(v => !string.IsNullOrEmpty(v))) ||
                    (parsed.ContainsKey("selectedAgeRanges") && parsed["selectedAgeRanges"].Any(v => !string.IsNullOrEmpty(v))) ||
                    (parsed.ContainsKey("showAdopted") && !string.IsNullOrEmpty(parsed["showAdopted"]));
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.HasActiveFilters = hasActiveFilters;

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