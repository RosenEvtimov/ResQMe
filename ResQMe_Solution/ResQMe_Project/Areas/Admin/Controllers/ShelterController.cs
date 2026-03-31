namespace ResQMe_Project.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;
    using ResQMe.ViewModels.Shelter;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShelterController : Controller
    {
        private readonly IShelterService shelterService;

        public ShelterController(IShelterService shelterService)
        {
            this.shelterService = shelterService;
        }

        [HttpGet]
        public IActionResult Add(string? returnUrl)
        {
            return View(new ShelterFormViewModel { ReturnUrl = returnUrl});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ShelterFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await shelterService.AddShelterAsync(model);

                TempData["AdminSuccess"] = "Shelter added successfully!";

                return Redirect("/Shelter/Index" + model.ReturnUrl);
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
            var model = await shelterService.GetShelterForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShelterFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await shelterService.EditShelterAsync(model);

                TempData["AdminSuccess"] = "Shelter edited successfully!";

                return Redirect("/Shelter/Index" + model.ReturnUrl);
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
            var model = await shelterService.GetShelterDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            bool deleted = await shelterService.DeleteShelterAsync(id);

            if (!deleted)
            {
                var model = await shelterService.GetShelterDetailsAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                ViewBag.ReturnUrl = returnUrl;

                ModelState.AddModelError(string.Empty,
                    "You cannot delete this shelter, because it still has animals assigned to it.");

                return View("Delete", model);
            }

            TempData["AdminSuccess"] = "Shelter deleted successfully!";

            return Redirect("/Shelter/Index" + returnUrl);
        }
    }
}