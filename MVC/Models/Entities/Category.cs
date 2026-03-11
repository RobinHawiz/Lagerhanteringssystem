using System.ComponentModel.DataAnnotations;

namespace MVC.Models.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    // Category 1 -> many Items
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
