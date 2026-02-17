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
        public int ActivityId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? StartTime { get; set; }
        public DateOnly? EndTime { get; set; }
        public string? Address { get; set; }
        public string? OfficialLink { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
};


