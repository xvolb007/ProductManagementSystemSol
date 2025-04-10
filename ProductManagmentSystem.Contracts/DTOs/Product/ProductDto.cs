

namespace ProductManagmentSystem.Contracts.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
