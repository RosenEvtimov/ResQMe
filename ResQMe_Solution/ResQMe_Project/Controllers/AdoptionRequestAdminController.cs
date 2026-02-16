namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Services.Core.Interfaces;

    [Authorize(Roles = "Admin")]
    public class AdoptionRequestAdminController : Controller
    {
        private readonly IAdoptionRequestService adoptionRequestService;

        public AdoptionRequestAdminController(IAdoptionRequestService adoptionRequestService)
        {
            this.adoptionRequestService = adoptionRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await adoptionRequestService.GetAllRequestsForAdminAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await adoptionRequestService.GetRequestByIdForAdminAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await adoptionRequestService.ApproveRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            await adoptionRequestService.RejectRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}