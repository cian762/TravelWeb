using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IActivityTicketService
    {
        Task<IEnumerable<ActivityTicketViewModel>> GetAllActTicketsAsync();

        Task<ActivityTicketViewModel> GetActTicketByProductCodeAsync(string ProductCode);

        Task CreateActTicketAsync(ActivityTicketViewModel vm);

        Task EditActTicketAsync(ActivityTicketViewModel vm);

        //Task DeleteActTicket(int id);
    }
}
