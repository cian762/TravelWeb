using TravelWeb.Models;

namespace TravelWeb.ViewModels
{
    public class MemberProfileViewModel
    {
        public string MemberId { get; set; }

        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? AvatarUrl { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        public bool IsOwner { get; set; }
        public bool IsAdmin { get; set; }
    }
}
