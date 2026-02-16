namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Species;

    [Authorize(Roles = "Admin")]
    public class SpeciesController : Controller
    {
        private readonly ISpeciesService speciesService;

        public SpeciesController(ISpeciesService speciesService)
        {
            this.speciesService = speciesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await speciesService.GetAllSpeciesAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new SpeciesFormViewModel());
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
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await speciesService.GetSpeciesForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await speciesService.GetSpeciesForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await speciesService.DeleteSpeciesAsync(id);

            if (!deleted)
            {
                var model = await speciesService.GetSpeciesForEditAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this species, because there are animals assigned to it.");

                return View("Delete", model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}