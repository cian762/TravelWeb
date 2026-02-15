using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityTicketViewModel
    {
        [Display(Name = "商品代碼")]
        public string ProductCode { get; set; } = null!;

        [Display(Name = "商品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "票種分類")]
        public string? TicketCategoryName { get; set; }

        [Display(Name = "販售起始日")]
        public DateOnly? StartDate { get; set; }

        [Display(Name = "販售截止日")]
        public DateOnly? ExpiryDate { get; set; }

        [Display(Name = "門票現價")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString ="{0:C0}")]
        public int? CurrentPrice { get; set; }
        
        [Display(Name ="販售狀態")]
        public string? Status { get; set; }
    }
}
