namespace TravelWeb.Areas.Activity.Models
{
    public class CloudinaryImage
    {
        public int ImageSetId { get; set; }

        public int? ActivityId { get; set; }

        public string? ImageUrl { get; set; }

        public string? PublicId { get; set; }
    }
}
