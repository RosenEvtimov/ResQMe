namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Shelter;

    public class ShelterController : Controller
    {
        private readonly IShelterService shelterService;

        public ShelterController(IShelterService shelterService)
        {
            this.shelterService = shelterService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await shelterService.GetAllSheltersAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await shelterService.GetShelterDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new ShelterFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ShelterFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await shelterService.AddShelterAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await shelterService.GetShelterForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShelterFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await shelterService.EditShelterAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await shelterService.GetShelterDetailsAsync(id);

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
            await shelterService.DeleteShelterAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}