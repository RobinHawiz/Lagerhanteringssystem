using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Entities;

public class Item
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = "";

    [Column(TypeName = "decimal(10, 2)")]
    [Display(Name = "Price (kr)")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Amount { get; set; }

    // FK -> Category
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}
