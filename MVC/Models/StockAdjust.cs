using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class StockAdjust
{
    [Required]
    public int ItemId { get; set; }
    [Range(-100, 100)]
    public int Delta { get; set; }
}
