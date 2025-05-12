using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AgriEnergy_ConnectApp.Data;
using AgriEnergy_ConnectApp.Models;
using System.Threading.Tasks;
using System.Linq;

namespace AgriEnergy_ConnectApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Display the Employee Dashboard
        public async Task<IActionResult> EmployeeDashboard()
        {
            // Retrieve logged-in user from session
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user details
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the Farmer records for the logged-in employee
            var farmers = await _context.Farmers
                .Where(f => f.AddedById == user.UserId)
                .ToListAsync();

            // Pass the user's FullName and UserType to the View
            ViewBag.FullName = user.FullName;
            ViewBag.UserType = user.UserType;

            return View(farmers);
        }

        [HttpGet]
        public IActionResult CreateFarmer()
        {
            return View(); // Render the form for creating a farmer
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFarmer([Bind("FullName,Email,Phone,Address")] Farmer farmer)
        {
            // Retrieve the logged-in Employee's UserId from the session
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in as an Employee to add a Farmer.";
                return RedirectToAction("Login", "User");
            }

            // Set the AddedById field for tracking purposes
            farmer.AddedById = userId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Farmers.Add(farmer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("EmployeeDashboard", "User");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while creating the Farmer: {ex.Message}";
                }
            }

            return View(farmer);
        }

        [HttpGet]
        public async Task<IActionResult> EditFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                TempData["ErrorMessage"] = "Farmer not found.";
                return RedirectToAction("EmployeeDashboard");
            }

            return View(farmer); // Load the edit form with existing farmer details
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFarmer([Bind("FarmerId,FullName,Email,Phone,Address")] Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Attach the farmer object to the context
                    _context.Attach(farmer);

                    // Mark the specific properties as modified
                    _context.Entry(farmer).Property(f => f.FullName).IsModified = true;
                    _context.Entry(farmer).Property(f => f.Email).IsModified = true;
                    _context.Entry(farmer).Property(f => f.Phone).IsModified = true;
                    _context.Entry(farmer).Property(f => f.Address).IsModified = true;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Farmer updated successfully!";
                    return RedirectToAction("EmployeeDashboard", "User");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while updating the Farmer: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Validation failed. Please check the input fields.";
            }

            return View(farmer);
        }
        // GET: Farmer/DeleteProduct/{id}
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Find the product by its ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            // Pass the product details to the DeleteProduct view
            return View("~/Views/Farmer/DeleteProduct.cshtml", product);
        }

        // POST: Farmer/DeleteProduct
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the product again before deletion
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            try
            {
                // Remove the product from the database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the product: {ex.Message}";
            }

            // Redirect back to the FarmerDashboard after deletion
            return RedirectToAction("FarmerDashboard", "User");
        }


        [HttpGet]
        public async Task<IActionResult> ReadFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                TempData["ErrorMessage"] = "Farmer not found.";
                return RedirectToAction("EmployeeDashboard", "User");
            }

            return View(farmer); // Ensure a view named ReadFarmer.cshtml exists
        }


    }

}