using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.TripProduct.Models;

public partial class TicketCategory
{
    public int TicketCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<OrderItemTicket> OrderItemTickets { get; set; } = new List<OrderItemTicket>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<TripSchedule> ProductCodes { get; set; } = new List<TripSchedule>();
}
