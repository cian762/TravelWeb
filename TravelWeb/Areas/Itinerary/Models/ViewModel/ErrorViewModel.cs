namespace TravelWeb.Areas.Itinerary.Models.ViewModel
{
    public class ErrorViewModel
    {
        public List<ItineraryErrorItemVM> Errors { get; set; }
    }
    public class ItineraryErrorItemVM
    {
        public int ErrorId { get; set; }
        public int ItineraryId { get; set; }
        public int VersionId { get; set; }

        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool IsConfirmed { get; set; }

        public string ErrorReason { get; set; }
        public string AdminResponse { get; set; }
    }
    public class EditItineraryErrorViewModel
    {
        public int ErrorId { get; set; }

        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsConfirmed { get; set; }

        public string ErrorReason { get; set; }
        public string AdminResponse { get; set; }
    }
}
