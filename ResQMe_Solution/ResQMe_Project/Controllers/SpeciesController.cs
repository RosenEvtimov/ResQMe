namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Species;

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

            await speciesService.AddSpeciesAsync(model);

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, SpeciesFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await speciesService.EditSpeciesAsync(id, model);

            return RedirectToAction(nameof(Index));
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
                ModelState.AddModelError("",
                    "Cannot delete this species because there are animals assigned to it.");

                var model = await speciesService.GetSpeciesForEditAsync(id);
                return View("Delete", model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}