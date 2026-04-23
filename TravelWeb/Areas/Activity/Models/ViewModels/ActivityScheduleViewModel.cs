using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityScheduleViewModel
    {
        
        public int ActivityId { get; set; }

        [Display(Name ="活動標題")]
        public string? Title { get; set; }
        
        [Display(Name="起始日期")]
        public DateOnly? StartTime { get; set; }
       
        [Display(Name="結束日期")]
        public DateOnly? EndTime { get; set; }
        
        [Display(Name="發布日期")]
        public DateTime? PublishTime { get; set; }
        
        [Display(Name="下架日期")]
        public DateTime? UnPublishTime { get; set; }
        
        [Display(Name="刊登狀態")]
        public string? Status { get; set; }
    }
}
