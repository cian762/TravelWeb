using System.ComponentModel;

namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public partial class TripIndexViewModel
    {
        [DisplayName("行程ID")]
        public int TripProductId { get; set; }
        [DisplayName("圖片")]
        public string? ImagePath { get; set; }
        [DisplayName("旅遊行程名稱")]
        public string? ProductName { get; set; }
        [DisplayName("地區")]
        public string? RegionName { get; set; }
        [DisplayName("價格")]
        public string? DisplayPrice { get; set; }
        [DisplayName("狀態")]
        public string? Status { get; set; }      
    }
}
