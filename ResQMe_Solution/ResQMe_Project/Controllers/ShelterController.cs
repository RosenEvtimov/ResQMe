namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;

    public class ShelterController : Controller
    {
        private readonly IShelterService shelterService;

        public ShelterController(IShelterService shelterService)
        {
            this.shelterService = shelterService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            List<string> selectedCities,
            int page = 1)
        {
            const int pageSize = 2;

            var model = await shelterService.GetAllSheltersAsync(
                searchTerm,
                selectedCities,
                page,
                pageSize);

            ViewBag.AvailableCities = await shelterService.GetUniqueCitiesAsync();
            ViewBag.SelectedCities = selectedCities;
            ViewBag.SearchTerm = searchTerm;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string? returnUrl)
        {
            var model = await shelterService.GetShelterDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            bool hasActiveFilters = false;

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "?")
            {
                var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(returnUrl.TrimStart('?'));
                hasActiveFilters = 
                    (parsed.ContainsKey("searchTerm") && !string.IsNullOrEmpty(parsed["searchTerm"])) ||
                    (parsed.ContainsKey("selectedCities") && parsed["selectedCities"].Any(v => !string.IsNullOrEmpty(v)));
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.HasActiveFilters = hasActiveFilters;

            return View(model);
        }
    }
}