using LoanManager.Models;
using LoanManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LoanManager.Controllers
{
    public class CustomerController : Controller
    {

        private readonly CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(CustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // GET: /Customer
        public IActionResult Index()
        {
            var customers = _customerService.GetAllCustomers();
            return View(customers);
        }

        // GET: /Customer/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Customer/Add
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Customer model)
        {
            // Remove server-only properties so they don't cause modelstate errors
            ModelState.Remove(nameof(model.CreatedBy));
            ModelState.Remove(nameof(model.CreatedAt));
            ModelState.Remove(nameof(model.CustomerId));

            if (ModelState.IsValid)
            {
                // Capture the creator from the currently logged-in user stored in session
                var username = HttpContext.Session.GetString("Username");
                model.CreatedBy = string.IsNullOrWhiteSpace(username) ? "System" : username;
                model.CreatedAt = DateTime.Now;

                try
                {
                    _customerService.AddCustomer(model);
                    TempData["SuccessMessage"] = "Customer added successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding customer");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the customer.");
                }
            }

            return View(model);
        }

    }
}
