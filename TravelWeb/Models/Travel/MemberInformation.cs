using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class MemberInformation
{
    public string MemberId { get; set; } = null!;

    public string MemberCode { get; set; } = null!;

    public string? Name { get; set; }

    public byte? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AttractionProductFavorite> AttractionProductFavorites { get; set; } = new List<AttractionProductFavorite>();

    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    public virtual ICollection<ItineraryProductCollection> ItineraryProductCollections { get; set; } = new List<ItineraryProductCollection>();

    public virtual Blocked Member { get; set; } = null!;

    public virtual MemberList MemberCodeNavigation { get; set; } = null!;

    public virtual ICollection<MemberComplaint> MemberComplaints { get; set; } = new List<MemberComplaint>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();

    public virtual ICollection<MemberInformation> Followeds { get; set; } = new List<MemberInformation>();

    public virtual ICollection<MemberInformation> Followers { get; set; } = new List<MemberInformation>();
}
