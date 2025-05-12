using AgriEnergy_ConnectApp.Data;
using AgriEnergy_ConnectApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy_ConnectApp.Controllers
{
    [Route("Farmer/[action]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Farmer/FarmerDashboard
        public async Task<IActionResult> FarmerDashboard()
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

            // Get products associated with the logged-in user
            var products = await _context.Products
                .Where(p => p.FarmerId == user.UserId)
                .ToListAsync();

            // Pass user details and products to the view
            ViewBag.FullName = user.FullName;
            ViewBag.UserType = user.UserType;

            return View(products);
        }

        // GET: Farmer/AddProducts
        public IActionResult AddProducts()
        {
            // Populate the dropdown for farmers using FarmerId and FullName
            ViewBag.FarmerList = new SelectList(_context.Farmers, "FarmerId", "FullName");
            return View("~/Views/Farmer/AddProducts.cshtml");
        }

        // POST: Farmer/AddProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add product to the database
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    // Success message
                    TempData["SuccessMessage"] = "Product added successfully!";
                    return RedirectToAction("FarmerDashboard", "User");
                }
                catch (Exception ex)
                {
                    // Handle database or application errors
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                // Log validation errors
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                TempData["ErrorMessage"] = "Please correct the errors and try again.";
            }

            // Repopulate the Farmer dropdown in case of validation errors
            ViewBag.FarmerList = new SelectList(_context.Farmers, "FarmerId", "FullName", product.FarmerId);
            return View("~/Views/Farmer/AddProducts.cshtml", product);
        }

        [HttpGet]
        [Route("Farmer/ReadProducts/{id}")]
        public async Task<IActionResult> ReadProducts(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            return View("~/Views/Farmer/ReadProducts.cshtml", product);
        }


        [Route("Farmer/DeleteProduct/{id?}")]
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Product ID.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            return View("~/Views/Farmer/DeleteProduct.cshtml", product); // Ensure the correct view is returned
        }


        [Route("Farmer/ConfirmDeleteProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteProduct(int ProductId)
        {
            var product = await _context.Products.FindAsync(ProductId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("FarmerDashboard", "User");
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            }

            return RedirectToAction("FarmerDashboard", "User");
        }




        // GET: Edit Product
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Load farmers for the dropdown
            await LoadFarmersDropdown();

            return View("~/Views/Farmer/EditProduct.cshtml", product); // Specify the full path to the view
        }

        // POST: Edit Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(EditProduct), new { id = product.ProductId });
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Error updating product.";
                }
            }

            // Reload farmers for the dropdown in case of an error
            await LoadFarmersDropdown();

            return View("~/Views/Farmer/EditProduct.cshtml", product); // Specify the full path to the view
        }

        // Helper method: Load Farmers Dropdown
        private async Task LoadFarmersDropdown()
        {
            // Assuming "Farmers" is the entity and contains a FullName field for display
            var farmers = await _context.Farmers
                .Select(f => new SelectListItem
                {
                    Value = f.FarmerId.ToString(),
                    Text = f.FullName
                })
                .ToListAsync();

            // This will populate the ViewBag with a list of farmers to be used in the dropdown
            ViewBag.FarmerList = new SelectList(farmers, "Value", "Text");
        }
    }
}