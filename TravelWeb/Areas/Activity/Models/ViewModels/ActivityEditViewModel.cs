using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TravelWeb.Areas.Activity.Models.EFModel;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityEditViewModel
    {
        [ValidateNever]
        public int ActivityId { get; set; }

        [Required(ErrorMessage ="提醒: 請輸入活動名稱")]
        [Display(Name ="活動名稱")]
        public string? Title { get; set; }

        
        [Display(Name ="活動描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "提醒: 請提供開始時間")]
        [Display(Name ="開始時間")]
        public DateOnly? StartTime { get; set; }

        [Required(ErrorMessage = "提醒: 請提供結束時間")]
        [Display(Name = "結束時間")]
        public DateOnly? EndTime { get; set; }

        [Required(ErrorMessage = "提醒: 請提供舉辦位置")]
        [Display(Name ="舉辦位置")]
        public string? Address { get; set; }

        [Display(Name ="官方連結")]
        public string? OfficialLink { get; set; }

        [ValidateNever]
        [Display(Name ="更新時間")]
        public DateTime? UpdateAt { get; set; }

        [Required(ErrorMessage = "提醒: 請提供至少一個區域標籤")]
        [Display(Name = "區域標籤")]
        public List<string>? RegionName { get; set; }

        [Required(ErrorMessage = "提醒: 請至少選擇一種類型標籤")]
        [Display(Name = "類型標籤")]
        public List<string>? TypeName { get; set; }


    }
}
