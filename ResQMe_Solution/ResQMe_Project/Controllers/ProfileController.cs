namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Data.Models.Identity;
    using ResQMe.ViewModels.User;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileFormViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email!
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? returnAnimalId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.ReturnAnimalId = returnAnimalId;

            var model = new ProfileFormViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email!
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormViewModel model, int? returnAnimalId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnAnimalId = returnAnimalId;
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.City = model.City;
            user.PhoneNumber = model.PhoneNumber;

            if (user.Email != model.Email)
            {
                await userManager.SetEmailAsync(user, model.Email);
                await userManager.SetUserNameAsync(user, model.Email);
            }

            await userManager.UpdateAsync(user);

            /* Fixes the issue with the non-updated greeting */
            await signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Profile updated successfully.";

            if (returnAnimalId.HasValue)
            {
                return RedirectToAction("Create", "AdoptionRequest",
                    new { animalId = returnAnimalId.Value });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}