using Microsoft.AspNetCore.Mvc;
using AgriEnergy_ConnectApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AgriEnergy_ConnectApp.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AgriEnergy_ConnectApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Validate the user credentials against the database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Store UserId and UserType in session for later use
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserType", user.UserType);

                // Create claims based on the user type
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.UserType), // "Employee" or "Farmer"
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Remember the user
                };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect based on UserType
                if (user.UserType == "Employee")
                {
                    return RedirectToAction("EmployeeDashboard", "User");
                }
                else if (user.UserType == "Farmer")
                {
                    return RedirectToAction("FarmerDashboard", "User");
                }
            }

            TempData["Error"] = "Invalid username or password!";
            return View();
        }

        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> EmployeeDashboard()
        {
            // Retrieve the logged-in Employee's UserId from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || !_context.Users.Any(u => u.UserId == userId && u.UserType == "Employee"))
            {
                TempData["Error"] = "Unauthorized access. You must be logged in as an Employee.";
                return RedirectToAction("Login");
            }

            // Get the logged-in Employee's details
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                TempData["Error"] = "Unable to find user details.";
                return RedirectToAction("Login");
            }

            // Query farmers added by the logged-in Employee
            var farmers = await _context.Farmers.Where(f => f.AddedById == user.UserId).ToListAsync();

            // Pass Employee details to the View
            ViewBag.FullName = user.FullName;
            ViewBag.UserType = user.UserType;

            return View(farmers);
        }

        [Authorize]
        public async Task<IActionResult> FarmerDashboard()
        {
            // Retrieve the logged-in Farmer's UserId from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || !_context.Users.Any(u => u.UserId == userId && u.UserType == "Farmer"))
            {
                TempData["Error"] = "Unauthorized access. You must be logged in as a Farmer.";
                return RedirectToAction("Login");
            }

            // Get the logged-in Farmer's details
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                TempData["Error"] = "Unable to find user details.";
                return RedirectToAction("Login");
            }

            // Query products added by the Farmer
            var products = await _context.Products.Where(p => p.FarmerId == user.UserId).ToListAsync();

            // Pass Farmer details to the View
            ViewBag.FullName = user.FullName;
            ViewBag.UserType = user.UserType;

            return View(products);
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                // For demo: Reset password to "123456" (in real apps: send email)
                user.Password = "123456";
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Password has been reset to '123456'. Please change it after logging in.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Username not found.";
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

    }
}

