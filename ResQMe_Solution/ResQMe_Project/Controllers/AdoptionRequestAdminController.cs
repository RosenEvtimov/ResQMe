namespace ResQMe_Project.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ResQMe.Data.Models.Enums;
    using ResQMe.Services.Core.Interfaces;

    [Authorize(Roles = "Admin")]
    public class AdoptionRequestAdminController : Controller
    {
        private readonly IAdoptionRequestService adoptionRequestService;

        public AdoptionRequestAdminController(IAdoptionRequestService adoptionRequestService)
        {
            this.adoptionRequestService = adoptionRequestService;
        }

        public async Task<IActionResult> Index(string? status)
        {
            AdoptionRequestStatus? parsedStatus = null;

            /*If no status is provided, default to "Pending". 
              If "All" is provided, show all requests. 
              Otherwise, try to parse the provided status. */
            if (!Request.Query.ContainsKey("status"))
            {
                parsedStatus = AdoptionRequestStatus.Pending;
                status = "Pending";
            }
            else if (status == "All")
            {
                parsedStatus = null;
            }
            else if (!string.IsNullOrWhiteSpace(status) &&
                     Enum.TryParse<AdoptionRequestStatus>(status, out var result))
            {
                parsedStatus = result;
            }

            var model = await adoptionRequestService
                .GetAllRequestsForAdminAsync(parsedStatus);

            ViewBag.CurrentStatus = status;

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id, string? status)
        {
            ViewBag.CurrentStatus = status;

            var model = await adoptionRequestService.GetRequestByIdForAdminAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? status)
        {
            await adoptionRequestService.ApproveRequestAsync(id);

            TempData["AdminSuccess"] = "Request approved successfully.";

            return RedirectToAction(nameof(Index), new {status});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? status)
        {
            await adoptionRequestService.RejectRequestAsync(id);

            TempData["AdminSuccess"] = "Request rejected successfully.";

            return RedirectToAction(nameof(Index), new {status});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoApproval(int id, string? status)
        {
            await adoptionRequestService.UndoApprovalAsync(id);

            TempData["AdminSuccess"] = "Approval undone successfully.";

            return RedirectToAction(nameof(Index), new {status});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoRejection(int id, string? status)
        {
            await adoptionRequestService.UndoRejectionAsync(id);

            TempData["AdminSuccess"] = "Rejection undone successfully.";

            return RedirectToAction(nameof(Index), new { status });
        }
    }
}