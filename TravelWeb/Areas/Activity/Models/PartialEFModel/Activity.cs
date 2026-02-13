using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.Activity.Models.EFModel 
{
    [ModelMetadataType(typeof(ActivityMetadata))]
    public partial class Activity
    {

    }

    public class ActivityMetadata
    {
        [Display(Name = "活動編號")]
        public int ActivityId { get; set; }

        [Display(Name ="活動標題")]
        [Required]
        public string? Title { get; set; }
        
        [Display(Name = "活動描述")]
        public string? Description { get; set; }

        [Display(Name = "起始時間")]
        [Required]
        public DateOnly? StartTime { get; set; }

        [Display(Name = "結束時間")]
        [Required]
        public DateOnly? EndTime { get; set; }
        
        [Display(Name = "舉辦地點")]
        [Required]
        public string? Address { get; set; }

        [Display(Name = "官方連結")]
        public string? OfficialLink { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdateAt { get; set; }
    }
};


