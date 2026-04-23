using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public class ViewModelTripItineraryItems
    {
        public int ItineraryItemId { get; set; }
        public int TripProductId { get; set; }

        [DisplayName("第幾天")]
        [Required(ErrorMessage = "請輸入天數")]
        public int? DayNumber { get; set; }

        [DisplayName("行程排序號碼")]
        [Required(ErrorMessage = "請輸入排序數字")]
        public int? SortOrder { get; set; }

        // --- 名稱相關 ---
        [DisplayName("景點名稱")]
        public string? AttractionName { get; set; }

        [DisplayName("活動名稱")]
        public string? ActivityName { get; set; }

        [DisplayName("自訂資源名稱")]
        public string? ResourceName { get; set; }

       
        [DisplayName("備註")]
        public string? CustomText { get; set; }
        // --- 內容描述 (這是要存進 Resources 表的 DefaultDescription) ---
        public string? ResourceDescription { get; set; }

        // --- 圖片處理關鍵欄位 ---
        public string? ImagePath { get; set; }

        [DisplayName("圖片路徑")]
        public List<string> AllImagePaths { get; set; } = new List<string>();

        [DisplayName("上傳多張資源圖片")]
        public List<IFormFile>? FileImages { get; set; } // 💡 修改：從 IFormFile 改成 List，支援多圖上傳

        // --- 關聯 ID ---
        public int? ResourceId { get; set; }
        public int? AttractionId { get; set; }
        public int? ActivityId { get; set; }

        // --- 選單與設定 ---
        public IEnumerable<SelectListItem>? AttractionList { get; set; }
        public IEnumerable<SelectListItem>? ActivityList { get; set; }
        public int MaxDays { get; set; }

        // --- 預覽用 (Ajax 回傳) ---
        public List<string>? ResourceImages { get; set; }




    }
}
