using LoanManager.Models;
using LoanManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LoanManager.Controllers
{
    public class LoanController : Controller
    {
        private readonly LoanService _loanService;
        private readonly ILogger<LoanController> _logger;

        public LoanController(LoanService loanService, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        // AJAX: check if customer exists by identity number
        [HttpGet]
        public IActionResult CheckCustomer(string identityNumber)
        {
            if (string.IsNullOrWhiteSpace(identityNumber))
                return Json(new { exists = false });

            var customerId = _loanService.GetCustomerIdByIdentityNumber(identityNumber);
            return Json(new { exists = customerId != null, customerId });
        }

        // GET: /Loan/Create
        public IActionResult Create()
        {
            var types = _loanService.GetLoanTypes();
            ViewBag.LoanTypes = types;
            return View(new LoanAccount());
        }

        // POST: /Loan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoanAccount model, string IdentityNumber)
        {
            // Remove server-only or server-populated fields from ModelState so validation won't fail
            ModelState.Remove(nameof(model.CreatedBy));
            ModelState.Remove(nameof(model.CreatedAt));
            ModelState.Remove(nameof(model.LoanId));
            ModelState.Remove(nameof(model.CustomerId));
            ModelState.Remove(nameof(model.CurrentFlow));

            if (!ModelState.IsValid)
            {
                // Log and surface validation errors to help debugging when form appears to just refresh
                var errors = ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(err => (Field: kvp.Key, Error: err.ErrorMessage ?? err.Exception?.Message)))
                    .ToList();
                if (errors.Any())
                {
                    var msg = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Error}"));
                    _logger.LogWarning("Loan create failed ModelState invalid: {Errors}", msg);
                    TempData["ErrorMessage"] = "Validation failed: " + msg;
                }
                ViewBag.LoanTypes = _loanService.GetLoanTypes();
                return View(model);
            }

            // Lookup customer by identity number
            var customerId = _loanService.GetCustomerIdByIdentityNumber(IdentityNumber);
            if (customerId == null)
            {
                TempData["ErrorMessage"] = "Customer not found. Please add the user first.";
                return RedirectToAction("Add", "Customer");
            }

            // Fill server-side fields
            model.CustomerId = customerId.Value;
            var lt = _loanService.GetLoanTypeById(model.LoanTypeId);
            model.InterestRate = lt?.InterestRate ?? model.InterestRate;
            model.CurrentFlow = "New";
            model.CreatedBy = HttpContext.Session.GetString("Username") ?? "System";
            model.CreatedAt = DateTime.Now;

            // Simple monthly rental calculation (principal + interest) / months
            try
            {
                var total = model.LoanAmount + (model.LoanAmount * (model.InterestRate / 100m));
                model.MonthlyRental = model.LoanDuration > 0 ? Math.Round(total / model.LoanDuration, 2) : 0m;

                _loanService.CreateLoan(model);
                TempData["SuccessMessage"] = "Loan created successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the loan.");
                ViewBag.LoanTypes = _loanService.GetLoanTypes();
                return View(model);
            }
        }
    }
}
