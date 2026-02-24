using LoanManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LoanManager.Controllers
{
    public class LoginController : Controller
    {

        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }
        // GET: Login page
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (_authService.ValidateUser(username, password))
            {

                // <-- Store username in session here -->
                HttpContext.Session.SetString("Username", username);

                // Redirect to Home page if login is successful
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

    }
}
