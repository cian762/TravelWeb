using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;

namespace TravelWeb.Areas.TripProduct.Services.Implementation
{
    public class SOrder : IOrder
    {
        private readonly TripDbContext _order;
        public SOrder(TripDbContext order) 
        {
          _order=order;  
        }
        //這個方法會計算出你要求的「今日出團」、「未付款」與「待確認」三項指標。
        public async Task<OrderSummaryViewModel> GetOrderBoardSummaryAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // 1. 今日出團：這裡維持 Join 是正確的，因為「只有有檔期的行程」才會有「今日出團」的概念
            int todayTrips = await _order.OrderItems
                .Join(_order.TripSchedules,
                    oi => oi.ProductCode,
                    ts => ts.ProductCode,
                    (oi, ts) => new { oi.OrderId, ts.StartDate })
                .Where(x => x.StartDate == today)
                .Select(x => x.OrderId)
                .Distinct()
                .CountAsync();

            // 2. 未付款：包含「行程」與「景點活動」的所有待繳費訂單
            int unpaid = await _order.Orders
                .CountAsync(o => o.OrderStatus == "待處理" && o.PaymentStatus == "未付款");

            // 3. 待確認：這裡指的是客人付了錢，但商家還沒處理的（包含景點與行程）
            int pending = await _order.Orders
                .CountAsync(o => o.OrderStatus == "待處理" && o.PaymentStatus == "已付款");

            return new OrderSummaryViewModel
            {
                TodayTripsCount = todayTrips,
                UnpaidCount = unpaid,
                PendingConfirmationCount = pending
            };
        }

        public Task<OrderDetailViewModel> GetOrderDetailAsync(int orderId)
        {
            throw new NotImplementedException();
        }
        //訂單列表
        public async Task<IEnumerable<OrderIndexViewModel>> GetOrderIndexListAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // 1. 第一步：先用匿名型別把資料從資料庫抓出來 (這時候還是原始型別)
            var rawData = await (from o in _order.Orders
                                 join oi in _order.OrderItems on o.OrderId equals oi.OrderId
                                 join ts in _order.TripSchedules on oi.ProductCode equals ts.ProductCode into tsGroup
                                 from ts in tsGroup.DefaultIfEmpty()
                                 select new
                                 {
                                     o.OrderId,
                                     o.CreatedAt,
                                     o.TotalAmount,
                                     o.OrderStatus,
                                     o.PaymentStatus,
                                     o.ContactName,
                                     o.ContactPhone,
                                     o.CustomerNote,
                                     // 這裡先保持原始的 DateOnly?
                                     RawStartDate = (DateOnly?)ts.StartDate,
                                     oi.ProductNameSnapshot
                                 }).ToListAsync();

            // 2. 第二步：在記憶體中 (C# 端) 進行 ViewModel 轉換與型別調整
            var result = rawData.Select(x => new OrderIndexViewModel
            {
                OrderId = x.OrderId,
                OrderNumber = $"ORD{x.CreatedAt:yyyyMMdd}-{x.OrderId:D4}",
                CreatedAt = x.CreatedAt ?? DateTime.Now,
                TotalAmount = x.TotalAmount ?? 0m,
                OrderStatus = x.OrderStatus,
                PaymentStatus = x.PaymentStatus,
                CustomerName = x.ContactName,
                ContactPhone = x.ContactPhone,

                // 這裡在 C# 轉型，絕對不會有 SQL 翻譯問題！
                TravelDate = x.RawStartDate.HasValue
                    ? x.RawStartDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null,

                StatusLightClass = (x.RawStartDate == today) ? "bg-primary shadow-pulse" :
                                   (x.PaymentStatus == "未付款") ? "bg-danger" :
                                   (x.OrderStatus == "待處理") ? "bg-warning" : "bg-success",

                ProductSummary = x.ProductNameSnapshot,
                HasCustomerNote = !string.IsNullOrEmpty(x.CustomerNote)
            });

            return result;
        }

        public Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
