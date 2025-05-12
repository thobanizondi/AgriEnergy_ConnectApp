using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergy_ConnectApp.Models
{
    public class Product
    {
        public int ProductId { get; set; } // Primary Key

        [Required(ErrorMessage = "Farmer ID is required.")]
        public int FarmerId { get; set; } // Foreign Key for Farmer

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Type is required.")]
        [StringLength(50, ErrorMessage = "Product Type cannot exceed 50 characters.")]
        public string ProductType { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow; // Auto-initialized Date
    }


}
