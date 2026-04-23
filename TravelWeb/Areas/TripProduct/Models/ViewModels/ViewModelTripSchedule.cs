using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public partial class ViewModelTripSchedule
    {
        // --- 檔期核心 (TripSchedules 表) ---
        public string? ProductCode { get; set; } // PK
        public int TripProductId { get; set; } // FK
        [Required(ErrorMessage = "請選擇出發日期")]
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "請輸入人數上限")]
        public int MaxCapacity { get; set; }
        [Required(ErrorMessage = "請輸入售價")]
        [Range(0, 999999, ErrorMessage = "價格不能為負數")]
        public decimal Price { get; set; }

        // 💡 關鍵修正：配合資料庫的 nvarchar(20)，這裡改成 string
        public string? Status { get; set; }

        // --- 票種關聯 (處理 TripAndTicketRelation) ---
        public List<int> SelectedTicketIds { get; set; } = new List<int>();
        public List<SelectListItem>? AllTicketCategories { get; set; }

        // --- 輔助資訊 (從 TripProducts 抓) ---
        public string? ProductName { get; set; }
        public int DurationDays { get; set; }
        // 報名人數 (顯示進度條用)
        public int SoldQuantity { get; set; }
    }
}
