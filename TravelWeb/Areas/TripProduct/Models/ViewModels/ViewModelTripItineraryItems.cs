using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public class ViewModelTripItineraryItems
    {

        public int TripProductId { get; set; }
        [DisplayName("第幾天")]
        [Required(ErrorMessage ="請輸入天數")]
        public int? DayNumber { get; set; }
        [DisplayName("行程排序號碼")]
        [Required(ErrorMessage = "請輸入排序數字")]
        public int? SortOrder { get; set; }
        [DisplayName("景點名稱")]

        public string? AttractionName { get; set; }
        [DisplayName("活動名稱")]
        public string? ActivityName { get; set; }
       
        [DisplayName("自訂景點或活動")]
        public string? ResourceName { get; set; }
        [DisplayName("內容描述")]
        public string? CustomText { get; set; }
        [DisplayName("圖片預覽")]
        public string ?ImagePath { get; set; }
        [DisplayName("資源圖片上傳")]
        public IFormFile ?FileImage { get; set; }
        public int ?ResourceId { get; set; }
        public int ?AttractionId { get; set; }
        public int ?ActivityId { get; set; }
        public IEnumerable<SelectListItem>? AttractionList { get; set; }
        public IEnumerable<SelectListItem>? ActivityList { get; set; }
        public int MaxDays { get; set; }



    }
}
