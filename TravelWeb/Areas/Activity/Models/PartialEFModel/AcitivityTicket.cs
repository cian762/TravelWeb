using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.Activity.Models.EFModel 
{
    [ModelMetadataType(typeof(ActivityTicketMetadata))]
    public partial class AcitivityTicket
    {

    }

    public class ActivityTicketMetadata 
    {
        public string ProductCode { get; set; } = null!;

        [Display(Name ="產品名稱")]
        public string? ProductName { get; set; }

        public int? TicketCategoryId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public int? CurrentPrice { get; set; }

        public string? ProductLink { get; set; }

        public string? CoverImageUrl { get; set; }

        public string? Status { get; set; }
    }
}


