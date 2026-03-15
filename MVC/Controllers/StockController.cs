using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    [Authorize(Roles = "Warehouse")]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockController
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Items.Include(i => i.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: StockController/Adjust
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjust stockAdjust)
        {
            if (ModelState.IsValid)
            {
                var item = await _context.Items.FirstOrDefaultAsync(e => e.Id == stockAdjust.ItemId);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                item.Amount += stockAdjust.Delta;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Updated '{item.Name}' stock.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid adjustment value. Enter a number between -100 and 100.";
                return RedirectToAction(nameof(Index));
            }
        }
    }

}