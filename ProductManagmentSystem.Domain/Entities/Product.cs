﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagmentSystem.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int? StockQuantity { get; set; }
    }
}