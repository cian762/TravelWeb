using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityTicketViewModel
    {
        [ValidateNever]
        [Display(Name = "商品代碼")]
        public string ProductCode { get; set; } = null!;

        [ValidateNever]
        [Display(Name = "商品名稱")]
        [Required(ErrorMessage ="請輸入產品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "票種分類")]
        [Required(ErrorMessage = "請選擇一種票種")]
        public string? TicketCategoryName { get; set; }

        [Display(Name = "販售起始日")]
        [Required(ErrorMessage = "請選擇販售起始日")]
        public DateOnly? StartDate { get; set; }

        [Display(Name = "販售截止日")]
        [Required(ErrorMessage = "請選擇販售截止日")]
        public DateOnly? ExpiryDate { get; set; }

        [Display(Name = "門票現價")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString ="{0:C0}")]
        [Required(ErrorMessage = "請定義門票現價")]
        public int? CurrentPrice { get; set; }
        
        [Display(Name ="販售狀態")]
        [Required(ErrorMessage = "請選擇販售狀態")]
        public string? Status { get; set; }



        [Display(Name ="活動標題")]
        [Required(ErrorMessage = "請選擇活動標題")]
        public int? AcivityId { get; set; }

        [Display(Name ="商品描述")]
        [Required(ErrorMessage = "請添加商品描述")]
        public string? ProdcutDescription { get; set; }

        [Display(Name ="服務條款")]
        [Required(ErrorMessage = "請詳述服務條款")]
        public string? TermsOfService { get; set; }


    }
}
