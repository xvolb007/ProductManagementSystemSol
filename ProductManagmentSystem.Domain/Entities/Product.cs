using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagmentSystem.Domain.Entities
{
    public class Product
    {
        public double Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int? StockQuantity { get; set; }
    }
}