using System.Diagnostics;
using LoanManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoanManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LoanManager.Service.CustomerService _customerService;
        private readonly LoanManager.Service.LoanService _loanService;

        public HomeController(ILogger<HomeController> logger, LoanManager.Service.CustomerService customerService, LoanManager.Service.LoanService loanService)
        {
            _logger = logger;
            _customerService = customerService;
            _loanService = loanService;
        }

        public IActionResult Index()
        {
            // Get username from session
            ViewBag.Username = HttpContext.Session.GetString("Username");

            // Load loans from service and show on home page
            var loans = _loanService.GetAllLoans();
            ViewBag.Loans = loans;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Redirect to Customer Add page (keeps existing Home links working)
        public IActionResult AddCustomer()
        {
            return RedirectToAction("Add", "Customer");
        }

        // Placeholder for CreateLoan - simple placeholder to avoid 404 if link clicked
        public IActionResult CreateLoan()
        {
            // Redirect to the Loan/Create page
            return RedirectToAction("Create", "Loan");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
