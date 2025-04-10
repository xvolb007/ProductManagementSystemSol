using System.ComponentModel.DataAnnotations;

namespace ProductManagmentSystem.Contracts.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [Url]
        public required string ImageUrl { get; set; }

        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public int? StockQuantity { get; set; }
    }
}