namespace ResQMe_Project.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Species;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpeciesController : Controller
    {
        private readonly ISpeciesService speciesService;

        public SpeciesController(ISpeciesService speciesService)
        {
            this.speciesService = speciesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            const int pageSize = 10;
            var model = await speciesService.GetAllSpeciesAsync(searchTerm, page, pageSize);
            ViewBag.SearchTerm = searchTerm;
            return View(model);
        }

        [HttpGet]
        public IActionResult Add(string? returnUrl)
        {
            return View(new SpeciesFormViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SpeciesFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await speciesService.AddSpeciesAsync(model);
                return Redirect("/Admin/Species/Index" + model.ReturnUrl);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string? returnUrl)
        {
            var model = await speciesService.GetSpeciesForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpeciesFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await speciesService.EditSpeciesAsync(model);
                return Redirect("/Admin/Species/Index" + model.ReturnUrl);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string? returnUrl)
        {
            var model = await speciesService.GetSpeciesForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            bool deleted = await speciesService.DeleteSpeciesAsync(id);

            if (!deleted)
            {
                var model = await speciesService.GetSpeciesForEditAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                model.ReturnUrl = returnUrl;

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this species, because there are animals assigned to it.");

                return View("Delete", model);
            }

            return Redirect("/Admin/Species/Index" + returnUrl);
        }
    }
}