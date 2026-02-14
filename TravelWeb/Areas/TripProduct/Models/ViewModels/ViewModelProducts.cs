using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public partial class ViewModelProducts
    {
        [DisplayName("旅遊行程名稱")]
        [Required(ErrorMessage = "請輸入行程名稱")]
        public string? ProductName { get; set; }
        [DisplayName("行程大概描述")]
        [Required(ErrorMessage ="請輸入行程描述")]
        public string? Description { get; set; }
        [DisplayName("行程天數")]
        [Required(ErrorMessage = "請輸入行程天數")]
        public int? DurationDays { get; set; }
        [DisplayName("預設價格")]
        [Required(ErrorMessage = "請輸入價格")]
        [Range(0, 999999)]
        public decimal? DisplayPrice { get; set; }
        [DisplayName("上傳封面圖")]
        [NotMapped]
        [Required(ErrorMessage = "請選擇圖片檔案")] 
        public IFormFile? ImageFile { get; set; }
        public string? CoverImage { get; set; }
        [DisplayName("請選擇地區")]
        [Required(ErrorMessage = "請選擇地區")]
        public int? RegionId { get; set; }
        [DisplayName("請選擇退款規定")]
        [Required(ErrorMessage = "請選擇退款規定")]
        public int? PolicyId { get; set; }
        public List<SelectListItem>? RegionOptions { get; set; }
        public List<SelectListItem>? PolicyOptions { get; set; }
        public List<SelectListItem>? TagOptions { get; set; }
        [DisplayName("行程標籤")]
        public List<string> SelectedTags { get; set; } = new List<string>();

    }
}
