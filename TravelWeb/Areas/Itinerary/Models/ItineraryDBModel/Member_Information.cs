using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Areas.Itinerary.Models.ItineraryDBModel
{
    [Table("Member_Information", Schema = "Member")]
    public class Member_Information
    {
        [Key]
        public string MemberId {  get; set; }
        public string? MemberCode { get; set; }
        public string? Name { get; set; }
        public byte? Gender {  get; set; }
        public DateTime? BirthDate {  get; set; }
        public string? AvatarUrl {  get; set; }
        public string? Status { get; set; }
    }
}
