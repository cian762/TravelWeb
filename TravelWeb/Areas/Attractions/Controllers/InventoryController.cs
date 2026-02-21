using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using TravelWeb.Areas.Attractions.Models;


namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class InventoryController : Controller
    {
        private readonly AttractionsContext _context;

        public InventoryController(AttractionsContext context)
        {
            _context = context;
        }

        // 庫存總覽
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.ProductInventoryStatuses
                .Join(_context.AttractionProducts.Where(p => !p.IsDeleted),
                    inv => inv.ProductId,
                    p => p.ProductId,
                    (inv, p) => new InventoryViewModel
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        Title = p.Title,
                        InventoryMode = inv.InventoryMode,
                        DailyLimit = inv.DailyLimit,
                        SoldQuantity = inv.SoldQuantity,
                        LastUpdatedAt = inv.LastUpdatedAt
                    })
                .ToListAsync();

            return View(inventories);
        }

        // 調整庫存模式
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInventoryMode(int productId, string inventoryMode, int? dailyLimit)
        {
            var inventory = await _context.ProductInventoryStatuses
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory == null) return NotFound();

            inventory.InventoryMode = inventoryMode.ToUpper();

            // 只有 DAILY 模式才存每日上限，其他清空
            inventory.DailyLimit = inventoryMode == "DAILY" ? dailyLimit : null;
            inventory.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "庫存模式已更新！";
            return RedirectToAction(nameof(Index));
        }

        // 進貨紀錄列表
        public async Task<IActionResult> StockIn(string? productCode)
        {
            var query = _context.StockInRecords
                .OrderByDescending(s => s.StockInDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(productCode))
                query = query.Where(s => s.ProductCode == productCode);

            // 補上商品資訊給 View 顯示
            var product = await _context.AttractionProducts
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);

            ViewBag.ProductCode = productCode;
            ViewBag.ProductTitle = product?.Title ?? "";

            return View(await query.ToListAsync());
        }

        // 新增進貨紀錄
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStockIn(StockInRecord record)
        {
            ModelState.Remove("ProductCodeNavigation");

            if (ModelState.IsValid)
            {
                record.StockInDate = DateOnly.FromDateTime(DateTime.Now);
                record.InventoryType = record.InventoryType ?? "VIRTUAL";

                // 新增進貨紀錄
                _context.StockInRecords.Add(record);

                // 同步更新已售數量 (sold_quantity 不變，remaining_stock 增加)
                var inventory = await _context.ProductInventoryStatuses
                    .FirstOrDefaultAsync(i => i.ProductId ==
                        _context.AttractionProducts
                            .Where(p => p.ProductCode == record.ProductCode)
                            .Select(p => p.ProductId)
                            .FirstOrDefault());

                if (inventory != null)
                    inventory.LastUpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"進貨 {record.Quantity} 張，已記錄完成！";
                return RedirectToAction(nameof(StockIn), new { productCode = record.ProductCode });
            }

            return View(record);
        }


    }
}
